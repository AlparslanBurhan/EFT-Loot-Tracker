using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EFTLootTracker.Services
{
    public class SettingsService
    {
        private readonly string _settingsPath;
        private readonly Dictionary<string, string> _settings;

        public SettingsService()
        {
            _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.ini");
            _settings = LoadSettings();
        }

        public bool AlwaysOnTop
        {
            get => GetValue("AlwaysOnTop", "False").Equals("True", StringComparison.OrdinalIgnoreCase);
            set => SetValue("AlwaysOnTop", value.ToString());
        }

        public double WindowWidth
        {
            get => double.TryParse(GetValue("WindowWidth", "1400"), out var w) ? w : 1400;
            set => SetValue("WindowWidth", value.ToString());
        }

        public double WindowHeight
        {
            get => double.TryParse(GetValue("WindowHeight", "1000"), out var h) ? h : 1000;
            set => SetValue("WindowHeight", value.ToString());
        }

        private string GetValue(string key, string defaultValue)
        {
            return _settings.TryGetValue(key, out var value) ? value : defaultValue;
        }

        private void SetValue(string key, string value)
        {
            _settings[key] = value;
            SaveSettings();
        }

        private Dictionary<string, string> LoadSettings()
        {
            var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(_settingsPath)) return settings;

            try
            {
                var lines = File.ReadAllLines(_settingsPath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";") || line.StartsWith("#"))
                        continue;

                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        settings[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }
            catch
            {
                // Silently ignore load errors
            }

            return settings;
        }

        private void SaveSettings()
        {
            try
            {
                var lines = _settings.Select(kvp => $"{kvp.Key}={kvp.Value}");
                File.WriteAllLines(_settingsPath, lines);
            }
            catch
            {
                // Silently ignore save errors
            }
        }
    }
}
