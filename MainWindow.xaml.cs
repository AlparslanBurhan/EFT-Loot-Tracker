using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EFTLootTracker.Models;
using EFTLootTracker.Services;

namespace EFTLootTracker;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private UpdateManager _updateManager;
    private List<LootItem> _allItems = new List<LootItem>();
    private List<LootItem> _collectorItems = new List<LootItem>();

    public MainWindow()
    {
        InitializeComponent();
        _updateManager = new UpdateManager();
        
        _updateManager.OnStatusChanged += (status) => {
            Dispatcher.Invoke(() => StatusText.Text = status);
        };

        _updateManager.OnProgressChanged += (current, total) => {
            Dispatcher.Invoke(() => {
                UpdateProgress.Visibility = Visibility.Visible;
                UpdateProgress.Maximum = total;
                UpdateProgress.Value = current;
            });
        };

        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // Check for updates first
            var updater = new AppUpdater();
            await updater.CheckAndInstallUpdateAsync();

            StatusText.Foreground = System.Windows.Media.Brushes.LightGray;
            UpdateProgress.Visibility = Visibility.Visible;
            
            // Load Loot items
            _allItems = await _updateManager.InitializeDataAsync();
            
            // Load Collector items
            _collectorItems = await _updateManager.InitializeCollectorDataAsync();

            PopulateFilters();
            ApplyFilters();
            
            CollectorListBox.ItemsSource = _collectorItems;
            
            // Hide progress bar and show final status
            UpdateProgress.Visibility = Visibility.Collapsed;
            StatusText.Text = $"Loot: {_allItems.Count} öğe • Collector: {_collectorItems.Count} öğe • Veriler güncel";
            StatusText.Foreground = System.Windows.Media.Brushes.LightGreen;
        }
        catch (Exception ex)
        {
            UpdateProgress.Visibility = Visibility.Collapsed;
            StatusText.Text = "Hata: " + ex.Message;
            StatusText.Foreground = System.Windows.Media.Brushes.Red;
        }
    }

    private void PopulateFilters()
    {
        if (_allItems == null) return;
        var filterItems = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in _allItems)
        {
            if (item.Quests != null) foreach (var q in item.Quests) filterItems.Add("Quest: " + q.Name);
            if (item.HideoutModules != null) foreach (var m in item.HideoutModules) filterItems.Add("Hideout: " + m.Name);
        }

        foreach (var filter in filterItems)
        {
            FilterCombo.Items.Add(new ComboBoxItem { Content = filter });
        }
    }

    private void ApplyFilters()
    {
        if (_allItems == null || ItemsListBox == null) return;

        var searchText = SearchBox.Text;
        var selectedFilter = (FilterCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();

        var filtered = _allItems.AsEnumerable();

        if (!string.IsNullOrEmpty(searchText))
        {
            filtered = filtered.Where(i => i.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        if (selectedFilter != null && selectedFilter != "Tümü")
        {
            if (selectedFilter.StartsWith("Quest: "))
            {
                var questName = selectedFilter.Replace("Quest: ", "");
                filtered = filtered.Where(i => i.Quests != null && i.Quests.Any(q => q.Name.Equals(questName, StringComparison.OrdinalIgnoreCase)));
            }
            else if (selectedFilter.StartsWith("Hideout: "))
            {
                var moduleName = selectedFilter.Replace("Hideout: ", "");
                filtered = filtered.Where(i => i.HideoutModules != null && i.HideoutModules.Any(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase)));
            }
        }

        // Update SelectedFilter for all items (so they can update their dynamic properties)
        foreach (var item in _allItems)
        {
            item.SelectedFilter = selectedFilter;
        }

        var result = filtered.ToList();
        ItemsListBox.ItemsSource = result;
    }

    private void FilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (SearchPlaceholder == null) return;
        SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        ApplyFilters();
    }

    private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
    {
        SearchPlaceholder.Visibility = Visibility.Collapsed;
    }

    private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(SearchBox.Text))
            SearchPlaceholder.Visibility = Visibility.Visible;
    }

    private void CollectorSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (CollectorSearchPlaceholder == null) return;
        CollectorSearchPlaceholder.Visibility = string.IsNullOrEmpty(CollectorSearchBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        ApplyCollectorFilter();
    }

    private void CollectorSearchBox_GotFocus(object sender, RoutedEventArgs e)
    {
        CollectorSearchPlaceholder.Visibility = Visibility.Collapsed;
    }

    private void CollectorSearchBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(CollectorSearchBox.Text))
            CollectorSearchPlaceholder.Visibility = Visibility.Visible;
    }

    private void ApplyCollectorFilter()
    {
        if (_collectorItems == null || CollectorListBox == null) return;

        var searchText = CollectorSearchBox.Text;
        var filtered = _collectorItems.AsEnumerable();

        if (!string.IsNullOrEmpty(searchText))
        {
            filtered = filtered.Where(i => i.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        CollectorListBox.ItemsSource = filtered.ToList();
    }

    private async void UpdateCollectorButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new HtmlInputDialog();
        if (dialog.ShowDialog() == true)
        {
            try
            {
                var htmlContent = dialog.HtmlContent;
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "collector_static.html");
                
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                await File.WriteAllTextAsync(filePath, htmlContent);
                
                StatusText.Text = "HTML dosyası güncellendi. Veriler yeniden yükleniyor...";
                StatusText.Foreground = System.Windows.Media.Brushes.LightGreen;
                
                // Reload Collector data
                _collectorItems = await _updateManager.InitializeCollectorDataAsync();
                ApplyCollectorFilter();
                
                StatusText.Text = "Collector verileri başarıyla güncellendi!";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Hata: " + ex.Message;
                StatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
    }

    private void AlwaysOnTopCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        this.Topmost = true;
    }

    private void AlwaysOnTopCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        this.Topmost = false;
    }
}