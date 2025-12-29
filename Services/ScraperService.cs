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
    }
}
