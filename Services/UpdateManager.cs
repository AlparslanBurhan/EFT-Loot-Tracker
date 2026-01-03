using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EFTLootTracker.Models;

namespace EFTLootTracker.Services
{
    public class UpdateManager
    {
        private readonly ScraperService _scraper;
        private readonly DataService _data;
        private bool _isUpdating = false;

        public event Action<string>? OnStatusChanged;
        public event Action<int, int>? OnProgressChanged;

        public UpdateManager()
        {
            _scraper = new ScraperService();
            _data = new DataService();
        }

        public async Task<List<LootItem>> InitializeDataAsync()
        {
            if (_isUpdating) return new List<LootItem>();
            _isUpdating = true;

            try {
                OnStatusChanged?.Invoke("Loot verileri kontrol ediliyor...");

                var localItems = await _data.LoadItemsAsync();
                var lastUpdate = _data.GetLastUpdateTime();

                // Dosya yoksa veya eski ise (24 saatten fazla) internetten çek
                bool needsUpdate = localItems.Count == 0 || 
                                   lastUpdate == DateTime.MinValue || 
                                   (DateTime.Now - lastUpdate).TotalHours > 24;

                if (needsUpdate)
                {
                    return await UpdateLootNowAsync();
                }

                OnStatusChanged?.Invoke($"Loot verileri yüklendi ({localItems.Count} öğe)");
                return localItems;
            } finally {
                _isUpdating = false;
            }
        }

        public async Task<List<LootItem>> ForceUpdateLootDataAsync()
        {
            if (_isUpdating) return new List<LootItem>();
            _isUpdating = true;

            try {
                _data.DeleteLootManifest();
                return await UpdateLootNowAsync();
            } finally {
                _isUpdating = false;
            }
        }

        private async Task<List<LootItem>> UpdateLootNowAsync()
        {
            OnStatusChanged?.Invoke("Loot verileri Wiki'den çekiliyor...");
            var remoteItems = await _scraper.ScrapeAllItemsAsync();

            if (remoteItems != null && remoteItems.Count > 0)
            {
                OnStatusChanged?.Invoke($"Loot simgeleri indiriliyor... ({remoteItems.Count} öğe)");
                await _data.DownloadIconsAsync(remoteItems, (current, total) => {
                    OnProgressChanged?.Invoke(current, total);
                });

                await _data.SaveItemsAsync(remoteItems);
                OnStatusChanged?.Invoke($"Loot verileri güncellendi ({remoteItems.Count} öğe)");
                return remoteItems;
            }
            else
            {
                OnStatusChanged?.Invoke("Hata: Wiki'den loot verileri çekilemedi");
                return await _data.LoadItemsAsync();
            }
        }

        public async Task<List<LootItem>> InitializeCollectorDataAsync()
        {
            if (_isUpdating) return new List<LootItem>();
            _isUpdating = true;

            try {
                OnStatusChanged?.Invoke("Collector verileri kontrol ediliyor...");

                var localItems = await _data.LoadCollectorItemsAsync();
                var lastUpdate = _data.GetCollectorLastUpdateTime();

                // Dosya yoksa veya eski ise (haftalık) internetten çek
                bool needsUpdate = localItems.Count == 0 || 
                                   lastUpdate == DateTime.MinValue || 
                                   (DateTime.Now - lastUpdate).TotalHours > 168;

                if (needsUpdate) // Update collector weekly
                {
                    return await UpdateCollectorNowAsync();
                }

                OnStatusChanged?.Invoke($"Collector verileri yüklendi ({localItems.Count} öğe)");
                
                // Apply saved status
                var statuses = await _data.LoadCollectorStatusAsync();
                foreach (var item in localItems)
                {
                    if (statuses.TryGetValue(item.Name, out bool isFound))
                    {
                        item.IsFound = isFound;
                    }
                }

                return localItems;
            } finally {
                _isUpdating = false;
            }
        }

        public async Task<List<LootItem>> ForceUpdateCollectorDataAsync()
        {
            if (_isUpdating) return new List<LootItem>();
            _isUpdating = true;

            try {
                _data.DeleteCollectorManifest();
                return await UpdateCollectorNowAsync();
            } finally {
                _isUpdating = false;
            }
        }

        private async Task<List<LootItem>> UpdateCollectorNowAsync()
        {
            OnStatusChanged?.Invoke("Collector verileri yükleniyor...");
            var remoteItems = await _scraper.ScrapeCollectorItemsAsync();

            if (remoteItems != null && remoteItems.Count > 0)
            {
                OnStatusChanged?.Invoke($"Collector simgeleri indiriliyor... ({remoteItems.Count} öğe)");
                await _data.DownloadIconsAsync(remoteItems, (current, total) => {
                    OnProgressChanged?.Invoke(current, total);
                });

                // Apply saved status
                var statuses = await _data.LoadCollectorStatusAsync();
                foreach (var item in remoteItems)
                {
                    if (statuses.TryGetValue(item.Name, out bool isFound))
                    {
                        item.IsFound = isFound;
                    }
                }

                await _data.SaveCollectorItemsAsync(remoteItems);
                OnStatusChanged?.Invoke($"Collector verileri güncellendi ({remoteItems.Count} öğe)");
                return remoteItems;
            }
            else
            {
                OnStatusChanged?.Invoke("Hata: Collector verileri çekilemedi");
                var localItems = await _data.LoadCollectorItemsAsync();
                var statuses = await _data.LoadCollectorStatusAsync();
                foreach (var item in localItems)
                {
                    if (statuses.TryGetValue(item.Name, out bool isFound))
                    {
                        item.IsFound = isFound;
                    }
                }
                return localItems;
            }
        }
    }
}
