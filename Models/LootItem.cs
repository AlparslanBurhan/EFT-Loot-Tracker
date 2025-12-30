using System;
using System.Collections.Generic;
using System.Linq;

namespace EFTLootTracker.Models
{
    public class ItemRequirement
    {
        public int Total { get; set; }
        public int FoundInRaid { get; set; }
    }

    public class RequirementDetail
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public bool IsFir { get; set; }

        public override string ToString() => IsFir ? $"{Name} (FIR x{Count})" : $"{Name} (x{Count})";
    }

    public class LootItem : System.ComponentModel.INotifyPropertyChanged
    {
        public string Name { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string LocalIconPath { get; set; } = string.Empty;
        public ItemRequirement Requirements { get; set; } = new ItemRequirement();
        public List<RequirementDetail> Quests { get; set; } = new List<RequirementDetail>();
        public List<RequirementDetail> HideoutModules { get; set; } = new List<RequirementDetail>();
        public string Category { get; set; } = string.Empty;
        public string WikiUrl { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }

        public bool HasQuests => Quests?.Any() == true;
        public bool HasHideoutModules => HideoutModules?.Any() == true;

        // Filtering support
        private string? _selectedFilter;
        public string? SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<RequirementDetail> FilteredQuests => 
            string.IsNullOrEmpty(SelectedFilter) || SelectedFilter == "Tümü" 
            ? Quests 
            : (SelectedFilter.StartsWith("Quest: ") 
                ? Quests.Where(q => SelectedFilter.EndsWith(q.Name, StringComparison.OrdinalIgnoreCase))
                : Enumerable.Empty<RequirementDetail>());

        public IEnumerable<RequirementDetail> FilteredHideoutModules => 
            string.IsNullOrEmpty(SelectedFilter) || SelectedFilter == "Tümü" 
            ? HideoutModules 
            : (SelectedFilter.StartsWith("Hideout: ") 
                ? HideoutModules.Where(m => SelectedFilter.EndsWith(m.Name, StringComparison.OrdinalIgnoreCase))
                : Enumerable.Empty<RequirementDetail>());

        public int FilteredTotal => 
            string.IsNullOrEmpty(SelectedFilter) || SelectedFilter == "Tümü"
            ? Requirements.Total
            : FilteredQuests.Sum(q => q.Count) + FilteredHideoutModules.Sum(m => m.Count);

        public int FilteredFir => 
            string.IsNullOrEmpty(SelectedFilter) || SelectedFilter == "Tümü"
            ? Requirements.FoundInRaid
            : FilteredQuests.Where(q => q.IsFir).Sum(q => q.Count) + FilteredHideoutModules.Where(m => m.IsFir).Sum(m => m.Count);

        public bool HasFilteredQuests => FilteredQuests.Any();
        public bool HasFilteredHideoutModules => FilteredHideoutModules.Any();

        // INotifyPropertyChanged implementation
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(FilteredQuests)));
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(FilteredHideoutModules)));
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(FilteredTotal)));
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(FilteredFir)));
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(HasFilteredQuests)));
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(HasFilteredHideoutModules)));
        }

        // Helper property for UI binding
        public string AllRequirements => string.Join(", ", 
            Quests.Select(q => q.ToString())
            .Concat(HideoutModules.Select(m => m.ToString())));
    }
}
