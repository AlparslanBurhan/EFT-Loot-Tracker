using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using EFTLootTracker.Models;
using Newtonsoft.Json;

namespace EFTLootTracker.Services
{
    public class DataService
    {
        private readonly string DataFolder;
        private readonly string CacheFolder;
        private readonly string ManifestPath;
        private readonly string CollectorManifestPath;
        private readonly HttpClient _httpClient;

        public DataService()
        {
            _httpClient = new HttpClient();
            
            // ProgramData klasörünü kullan
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "EFT-Loot-Tracker"
            );
            
            DataFolder = Path.Combine(appDataPath, "data");
            CacheFolder = Path.Combine(appDataPath, "cache", "icons");
            ManifestPath = Path.Combine(DataFolder, "manifest.json");
            CollectorManifestPath = Path.Combine(DataFolder, "collector.json");
            
            // Klasörleri oluştur
            Directory.CreateDirectory(DataFolder);
            Directory.CreateDirectory(CacheFolder);
        }

        public async Task SaveItemsAsync(List<LootItem> items)
        {
            try {
                var json = JsonConvert.SerializeObject(items, Formatting.Indented);
                await File.WriteAllTextAsync(ManifestPath, json);
            } catch {
            }
        }

        public async Task SaveCollectorItemsAsync(List<LootItem> items)
        {
            try {
                var json = JsonConvert.SerializeObject(items, Formatting.Indented);
                await File.WriteAllTextAsync(CollectorManifestPath, json);
            } catch {
            }
        }

        public async Task<List<LootItem>> LoadItemsAsync()
        {
            if (!File.Exists(ManifestPath))
                return new List<LootItem>();

            var json = await File.ReadAllTextAsync(ManifestPath);
            return JsonConvert.DeserializeObject<List<LootItem>>(json) ?? new List<LootItem>();
        }

        public async Task<List<LootItem>> LoadCollectorItemsAsync()
        {
            if (!File.Exists(CollectorManifestPath))
                return new List<LootItem>();

            var json = await File.ReadAllTextAsync(CollectorManifestPath);
            return JsonConvert.DeserializeObject<List<LootItem>>(json) ?? new List<LootItem>();
        }

        public async Task DownloadIconsAsync(List<LootItem> items, Action<int, int>? progressCallback = null)
        {
            int count = 0;
            int total = items.Count;
            
            using var semaphore = new System.Threading.SemaphoreSlim(10); // Max 10 parallel downloads
            var tasks = items.Select(async item => {
                if (string.IsNullOrEmpty(item.IconUrl)) return;

                await semaphore.WaitAsync();
                try {
                    string fileName = GetSafeFilename(item.Name) + ".png";
                    string fullPath = Path.Combine(CacheFolder, fileName);
                    item.LocalIconPath = Path.GetFullPath(fullPath);

                    if (!File.Exists(fullPath)) {
                        var bytes = await _httpClient.GetByteArrayAsync(item.IconUrl);
                        await File.WriteAllBytesAsync(fullPath, bytes);
                    }
                } catch {
                } finally {
                    System.Threading.Interlocked.Increment(ref count);
                    progressCallback?.Invoke(count, total);
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }

        private string GetSafeFilename(string filename)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return new string(filename.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
        }

        public DateTime GetLastUpdateTime()
        {
            if (File.Exists(ManifestPath))
            {
                return File.GetLastWriteTime(ManifestPath);
            }
            return DateTime.MinValue;
        }

        public DateTime GetCollectorLastUpdateTime()
        {
            if (File.Exists(CollectorManifestPath))
            {
                return File.GetLastWriteTime(CollectorManifestPath);
            }
            return DateTime.MinValue;
        }
    }
}
