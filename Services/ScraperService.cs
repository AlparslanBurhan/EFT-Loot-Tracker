using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EFTLootTracker.Models;
using HtmlAgilityPack;

namespace EFTLootTracker.Services
{
    public class ScraperService
    {
        private const string WikiBaseUrl = "https://escapefromtarkov.fandom.com";
        private const string LootUrl = "https://escapefromtarkov.fandom.com/wiki/Loot";
        private readonly HttpClient _httpClient;

        public ScraperService()
        {
            var handler = new HttpClientHandler();
            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }

        public async Task<List<LootItem>> ScrapeAllItemsAsync()
        {
            var items = new List<LootItem>();
            try
            {
                var html = await _httpClient.GetStringAsync(LootUrl);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var tables = doc.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable')]");
                if (tables == null)
                {
                    await System.IO.File.AppendAllTextAsync("debug.log", "No wikitable found via XPath\n");
                    return items;
                }

                await System.IO.File.AppendAllTextAsync("debug.log", $"Found {tables.Count} tables\n");

                foreach (var table in tables)
                {
                    var rows = table.SelectNodes(".//tr");
                    if (rows == null || rows.Count < 2) continue;

                    await System.IO.File.AppendAllTextAsync("debug.log", $"Processing table with {rows.Count} rows\n");
                    string category = GetCategory(table);
                    int tableItems = 0;

                    foreach (var row in rows)
                    {
                        // Some columns use <th>, others <td>. Get both.
                        var cells = row.SelectNodes(".//*[self::td or self::th]");
                        if (cells == null || cells.Count < 5) continue; 

                        // Skip if it's the header row (contains "Icon" or "Name" text in first cells)
                        if (cells[0].InnerText.Contains("Icon") || cells[1].InnerText.Contains("Name")) continue;

                        var item = ParseRow(cells, category);
                        if (item != null && !string.IsNullOrEmpty(item.Name))
                        {
                            items.Add(item);
                            tableItems++;
                        }
                    }
                    await System.IO.File.AppendAllTextAsync("debug.log", $"Table processed. Items found: {tableItems}\n");
                }
                await System.IO.File.AppendAllTextAsync("debug.log", $"Total items found: {items.Count}\n");
            }
            catch (Exception ex)
            {
                await System.IO.File.AppendAllTextAsync("debug.log", $"Scrape Error: {ex.Message}\n{ex.StackTrace}\n");
            }

            return items;
        }

        private string GetCategory(HtmlNode table)
        {
            var heading = table.SelectSingleNode("preceding-sibling::h3[1] | preceding-sibling::h2[1]");
            return heading?.InnerText.Trim() ?? "Diğer";
        }

        private LootItem ParseRow(HtmlNodeCollection cells, string category)
        {
            var item = new LootItem { Category = category };

            // Icon: First column
            var img = cells[0].SelectSingleNode(".//img");
            if (img != null)
            {
                item.IconUrl = img.GetAttributeValue("data-src", img.GetAttributeValue("src", ""));
                if (item.IconUrl.Contains("/revision/"))
                    item.IconUrl = item.IconUrl.Split("/revision/")[0];
            }

            // Name: Second column
            var nameNode = cells[1].SelectSingleNode(".//a") ?? cells[1];
            item.Name = nameNode.InnerText.Trim();
            
            var link = cells[1].SelectSingleNode(".//a");
            if (link != null)
                item.WikiUrl = WikiBaseUrl + link.GetAttributeValue("href", "");

            // Notes: Last column (Parsing list items for quests and modules)
            var notesCell = cells[cells.Count - 1];
            var listItems = notesCell.SelectNodes(".//li");
            
            if (listItems != null)
            {
                foreach (var li in listItems)
                {
                    ParseListItem(li, item);
                }
            }
            else
            {
                // Fallback for non-list notes
                ParseRequirements(notesCell.InnerText, item);
            }

            return item;
        }

        private void ParseListItem(HtmlNode li, LootItem item)
        {
            string text = li.InnerText.Trim();
            
            // Extract count
            var countMatch = Regex.Match(text, @"(\d+)");
            int count = countMatch.Success ? int.Parse(countMatch.Value) : 0;

            if (count == 0) return;

            // Check if it's FIR (Look for "in raid" text or red font)
            bool isFir = text.Contains("in raid", StringComparison.OrdinalIgnoreCase) || 
                         li.InnerHtml.Contains("color=\"red\"") || 
                         li.InnerHtml.Contains("color:red");

            item.Requirements.Total += count;
            if (isFir) item.Requirements.FoundInRaid += count;

            // Extract Quest or Module name
            // Usually in an <a> tag
            var links = li.SelectNodes(".//a");
            if (links != null)
            {
                foreach (var a in links)
                {
                    string linkText = a.InnerText.Trim();
                    if (linkText.Equals("in raid", StringComparison.OrdinalIgnoreCase)) continue;
                    
                    string targetList = "";
                    if (text.Contains("quest", StringComparison.OrdinalIgnoreCase))
                    {
                        targetList = "Quest";
                    }
                    else if (text.Contains("Hideout", StringComparison.OrdinalIgnoreCase) || text.Contains("level", StringComparison.OrdinalIgnoreCase) || a.GetAttributeValue("href", "").Contains("Hideout#Modules"))
                    {
                        targetList = "Hideout";
                    }
                    else
                    {
                        // Fallback categorization
                        targetList = isFir ? "Quest" : "Hideout";
                    }

                    if (targetList == "Quest")
                    {
                        if (!item.Quests.Any(q => q.Name == linkText))
                            item.Quests.Add(new RequirementDetail { Name = linkText, Count = count, IsFir = isFir });
                    }
                    else if (targetList == "Hideout")
                    {
                        if (!item.HideoutModules.Any(m => m.Name == linkText))
                            item.HideoutModules.Add(new RequirementDetail { Name = linkText, Count = count, IsFir = isFir });
                    }
                }
            }
        }

        private void ParseRequirements(string notes, LootItem item)
        {
            var firRegex = new Regex(@"(\d+)\s+needs?\s+to\s+be\s+found\s+in\s+raid", RegexOptions.IgnoreCase);
            var obtainRegex = new Regex(@"(\d+)\s+needs?\s+to\s+be\s+obtained", RegexOptions.IgnoreCase);

            var firMatches = firRegex.Matches(notes);
            foreach (Match match in firMatches)
            {
                if (int.TryParse(match.Groups[1].Value, out int count))
                {
                    item.Requirements.FoundInRaid += count;
                    item.Requirements.Total += count;
                }
            }

            var obtainMatches = obtainRegex.Matches(notes);
            foreach (Match match in obtainMatches)
            {
                if (int.TryParse(match.Groups[1].Value, out int count))
                {
                    item.Requirements.Total += count;
                }
            }
        }

        public async Task<List<LootItem>> ScrapeCollectorItemsAsync()
        {
            var items = new List<LootItem>();
            try
            {
                var response = await _httpClient.GetAsync("https://escapefromtarkov.fandom.com/wiki/Collector");
                
                var html = await response.Content.ReadAsStringAsync();
                
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Find the table inside div.table-wide > div.table-wide-inner
                // This comes after an h2 heading
                var tableContainer = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'table-wide')]//div[contains(@class, 'table-wide-inner')]//table");
                
                if (tableContainer == null)
                {
                    await System.IO.File.AppendAllTextAsync("debug.log", "Could not find table-wide-inner table\n");
                    return items;
                }

                await System.IO.File.AppendAllTextAsync("debug.log", "Found table-wide-inner table\n");

                // Get all rows from tbody
                var rows = tableContainer.SelectNodes(".//tbody/tr");
                if (rows == null || rows.Count == 0)
                {
                    await System.IO.File.AppendAllTextAsync("debug.log", "No rows found in tbody\n");
                    return items;
                }

                await System.IO.File.AppendAllTextAsync("debug.log", $"Found {rows.Count} rows in tbody\n");

                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.SelectNodes(".//td");
                        if (cells == null || cells.Count < 4)
                        {
                            await System.IO.File.AppendAllTextAsync("debug.log", $"Row has insufficient cells: {cells?.Count ?? 0}\n");
                            continue;
                        }

                        var item = new LootItem { Category = "Collector" };

                        // Icon: First td contains span.mw-default-size with img
                        var iconSpan = cells[0].SelectSingleNode(".//span[contains(@class, 'mw-default-size')]//img");
                        if (iconSpan == null)
                        {
                            // Try without the span class requirement
                            iconSpan = cells[0].SelectSingleNode(".//img");
                        }
                        
                        if (iconSpan != null)
                        {
                            item.IconUrl = iconSpan.GetAttributeValue("data-src", iconSpan.GetAttributeValue("src", ""));
                            if (item.IconUrl.Contains("/revision/"))
                                item.IconUrl = item.IconUrl.Split("/revision/")[0];
                        }

                        // Name: Second td contains <a> tag
                        var nameLink = cells[1].SelectSingleNode(".//a");
                        if (nameLink != null)
                        {
                            item.Name = nameLink.InnerText.Trim();
                            item.WikiUrl = WikiBaseUrl + nameLink.GetAttributeValue("href", "");
                        }
                        else
                        {
                            item.Name = cells[1].InnerText.Trim();
                        }

                        if (string.IsNullOrEmpty(item.Name))
                        {
                            await System.IO.File.AppendAllTextAsync("debug.log", "Row has no name, skipping\n");
                            continue;
                        }

                        // Amount: Third td contains just the number
                        var amountText = cells[2].InnerText.Trim();
                        if (int.TryParse(Regex.Match(amountText, @"\d+").Value, out int amount))
                        {
                            item.Requirements.Total = amount;
                        }
                        else
                        {
                            item.Requirements.Total = 1; // Default to 1 if not found
                        }

                        // Find in Raid: Look for td with font color="red" and "Yes" text
                        // This is typically the 5th td (index 4)
                        bool isFir = false;
                        if (cells.Count > 4)
                        {
                            var firCell = cells[4];
                            var firHtml = firCell.InnerHtml.ToLower();
                            var firText = firCell.InnerText.ToLower();
                            
                            // Check for red "Yes"
                            isFir = firText.Contains("yes") && firHtml.Contains("color=\"red\"");
                        }

                        if (isFir)
                        {
                            item.Requirements.FoundInRaid = item.Requirements.Total;
                        }

                        // Add a Quest Detail for UI consistency
                        item.Quests.Add(new RequirementDetail 
                        { 
                            Name = "Collector", 
                            Count = item.Requirements.Total, 
                            IsFir = isFir 
                        });

                        items.Add(item);
                        await System.IO.File.AppendAllTextAsync("debug.log", $"Added: {item.Name} (Amount: {item.Requirements.Total}, FIR: {isFir})\n");
                    }
                    catch (Exception rowEx)
                    {
                        await System.IO.File.AppendAllTextAsync("debug.log", $"Error processing row: {rowEx.Message}\n");
                    }
                }
                
                await System.IO.File.AppendAllTextAsync("debug.log", $"Total Collector items: {items.Count}\n");
            }
            catch (Exception ex)
            {
                await System.IO.File.AppendAllTextAsync("debug.log", $"Collector Scrape Error: {ex.Message}\n{ex.StackTrace}\n");
            }

            return items;
        }
    }
}
