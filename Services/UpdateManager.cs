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
                OnStatusChanged?.Invoke("Veriler kontrol ediliyor...");
            
            var localItems = await _data.LoadItemsAsync();
            var lastUpdate = _data.GetLastUpdateTime();

            // Check if we need to update (older than 24h or no data)
            if (localItems.Count == 0 || (DateTime.Now - lastUpdate).TotalHours > 24)
            {
                OnStatusChanged?.Invoke("Güncel veriler Wiki'den çekiliyor...");
                var remoteItems = await _scraper.ScrapeAllItemsAsync();

                if (remoteItems != null && remoteItems.Count > 0)
                {
                    OnStatusChanged?.Invoke("Simgeler indiriliyor...");
                    await _data.DownloadIconsAsync(remoteItems, (current, total) => {
                        OnProgressChanged?.Invoke(current, total);
                    });

                    await _data.SaveItemsAsync(remoteItems);
                    OnStatusChanged?.Invoke("Veriler başarıyla güncellendi.");
                    return remoteItems;
                }
                else
                {
                    OnStatusChanged?.Invoke("Hata: Wiki'den veri çekilemedi.");
                    return localItems;
                }
            }

            OnStatusChanged?.Invoke("Veriler güncel.");
            _isUpdating = false;
            return localItems;
            } finally {
                _isUpdating = false;
            }
        }
    }
}
