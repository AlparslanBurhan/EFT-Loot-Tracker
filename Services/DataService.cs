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
        private const string DataFolder = "data";
        private const string CacheFolder = "cache/icons";
        private const string ManifestPath = "data/manifest.json";
        private readonly HttpClient _httpClient;

        public DataService()
        {
            _httpClient = new HttpClient();
            Directory.CreateDirectory(DataFolder);
            Directory.CreateDirectory(CacheFolder);
        }

        public async Task SaveItemsAsync(List<LootItem> items)
        {
            try {
                var json = JsonConvert.SerializeObject(items, Formatting.Indented);
                await File.WriteAllTextAsync(ManifestPath, json);
                await File.AppendAllTextAsync("debug.log", $"Saved {items.Count} items to {Path.GetFullPath(ManifestPath)}\n");
            } catch (Exception ex) {
                await File.AppendAllTextAsync("debug.log", $"Save Error: {ex.Message}\n");
            }
        }

        public async Task<List<LootItem>> LoadItemsAsync()
        {
            if (!File.Exists(ManifestPath))
                return new List<LootItem>();

            var json = await File.ReadAllTextAsync(ManifestPath);
            return JsonConvert.DeserializeObject<List<LootItem>>(json) ?? new List<LootItem>();
        }

        public async Task DownloadIconsAsync(List<LootItem> items, Action<int, int> progressCallback = null)
        {
            await File.AppendAllTextAsync("debug.log", $"Starting icon downloads for {items.Count} items...\n");
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
                } catch (Exception ex) {
                    await File.AppendAllTextAsync("debug.log", $"Icon Fallacy ({item.Name}): {ex.Message}\n");
                } finally {
                    System.Threading.Interlocked.Increment(ref count);
                    progressCallback?.Invoke(count, total);
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
            await File.AppendAllTextAsync("debug.log", "Icon downloads completed.\n");
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
    }
}
