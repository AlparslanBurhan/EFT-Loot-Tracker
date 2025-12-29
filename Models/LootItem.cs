using System;
using System.Collections.Generic;

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

    public class LootItem
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

        // Helper property for UI binding
        public string AllRequirements => string.Join(", ", 
            Quests.Select(q => q.ToString())
            .Concat(HideoutModules.Select(m => m.ToString())));
    }
}
