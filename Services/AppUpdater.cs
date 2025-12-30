using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace EFTLootTracker.Services;

public class AppUpdater
{
    private const string RepoOwner = "AlparslanBurhan";
    private const string RepoName = "EFT-Loot-Tracker";
    private const string ReleasesUrl = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";

    public async Task CheckAndInstallUpdateAsync()
    {
        try
        {
            using var client = new HttpClient();
            // GitHub API requires a User-Agent header
            client.DefaultRequestHeaders.Add("User-Agent", "EFT-Loot-Tracker-App");

            var response = await client.GetStringAsync(ReleasesUrl);
            var json = JObject.Parse(response);
            
            var tagName = json["tag_name"]?.ToString();
            if (string.IsNullOrEmpty(tagName)) return;

            // Remove 'v' prefix if exists for parsing
            var versionString = tagName.TrimStart('v');
            
            if (Version.TryParse(versionString, out var latestVersion))
            {
                var currentVersion = Assembly.GetEntryAssembly()?.GetName().Version;
                if (currentVersion == null) return;

                // Compare versions (Latest > Current)
                if (latestVersion > currentVersion)
                {
                    var result = MessageBox.Show(
                        $"Yeni bir güncelleme mevcut: {tagName}\n\nŞimdi indirip güncellemek istiyor musunuz?",
                        "Güncelleme Mevcut",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        var assets = json["assets"] as JArray;
                        var exeAsset = assets?.FirstOrDefault(a => a["name"]?.ToString().EndsWith(".exe") == true);

                        if (exeAsset != null)
                        {
                            var downloadUrl = exeAsset["browser_download_url"]?.ToString();
                            if (!string.IsNullOrEmpty(downloadUrl))
                            {
                                await InstallUpdateAsync(downloadUrl, exeAsset["name"]?.ToString() ?? "update.exe");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Güncelleme dosyası bulunamadı.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Silently fail or log, typically we don't want to annoy user on startup if internet is down
            Debug.WriteLine($"Update check failed: {ex.Message}");
        }
    }

    private async Task InstallUpdateAsync(string url, string fileName)
    {
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), fileName);

            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(tempPath, bytes);

            // Start the new exe
            Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });

            // Close current app
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Güncelleme indirilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
