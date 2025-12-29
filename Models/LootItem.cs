using System;
using System.Collections.Generic;

namespace EFTLootTracker.Models
{
    public class ItemRequirement
    {
        public int Total { get; set; }
        public int FoundInRaid { get; set; }
        public int Collected { get; set; }
        public int FirCollected { get; set; }
    }

    public class LootItem
    {
        public string Name { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string LocalIconPath { get; set; } = string.Empty;
        public ItemRequirement Requirements { get; set; } = new ItemRequirement();
        public List<string> Quests { get; set; } = new List<string>();
        public List<string> HideoutModules { get; set; } = new List<string>();
        public string Category { get; set; } = string.Empty;
        public string WikiUrl { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }
}
