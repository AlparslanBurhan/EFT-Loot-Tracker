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
        private readonly System.Net.CookieContainer _cookieContainer;

        public ScraperService()
        {
            _cookieContainer = new System.Net.CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                UseCookies = true,
                AllowAutoRedirect = true,
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.All
            };
            
            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            _httpClient.DefaultRequestHeaders.Add("DNT", "1");
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        }

        public async Task<List<LootItem>> ScrapeAllItemsAsync()
        {
            var itemDict = new Dictionary<string, LootItem>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var html = await _httpClient.GetStringAsync(LootUrl);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Find all tables with wikitable class
                var tables = doc.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable')]");
                if (tables == null)
                {
                    // If wikitable class is not found, try finding any table and log classes
                    var allTables = doc.DocumentNode.SelectNodes("//table");
                    string tableInfo = allTables == null ? "No tables found at all." : $"Found {allTables.Count} tables, but none with 'wikitable' class.";
                    string snippet = html.Length > 300 ? html.Substring(0, 300) : html;
                    
                    throw new Exception($"Wiki page structure changed: {tableInfo}. HTML Length: {html.Length}. Snippet: {snippet}");
                }

                foreach (var table in tables)
                {
                    // Look for rows directly in the table or in its tbody
                    var rows = table.SelectNodes(".//tr"); 
                    if (rows == null) continue;

                    string category = GetCategory(table);

                    foreach (var row in rows)
                    {
                        // Get ONLY the top-level cells for this row to avoid picking up cells from nested tables
                        var cells = row.SelectNodes("./td | ./th");
                        if (cells == null || cells.Count < 5) continue; 

                        // Skip header rows by checking for header-like text
                        string firstCellText = cells[0].InnerText.Trim();
                        string secondCellText = cells[1].InnerText.Trim();
                        if (firstCellText.Equals("Icon", StringComparison.OrdinalIgnoreCase) || 
                            secondCellText.Equals("Name", StringComparison.OrdinalIgnoreCase)) continue;

                        ParseRow(cells, category, itemDict);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Wiki scraping failed: {ex.Message}", ex);
            }

            if (itemDict.Count == 0)
            {
                throw new Exception("Wiki scraper finished but found 0 items. The page structure might have changed significantly.");
            }

            return itemDict.Values.ToList();
        }

        private string GetCategory(HtmlNode table)
        {
            var heading = table.SelectSingleNode("preceding-sibling::h3[1] | preceding-sibling::h2[1]");
            return heading?.InnerText.Trim() ?? "Diğer";
        }

        private void ParseRow(HtmlNodeCollection cells, string category, Dictionary<string, LootItem> itemDict)
        {
            // Name: Second column
            var nameNode = cells[1].SelectSingleNode(".//a") ?? cells[1];
            string name = nameNode.InnerText.Trim();
            if (string.IsNullOrEmpty(name)) return;

            if (!itemDict.TryGetValue(name, out var item))
            {
                item = new LootItem { Name = name, Category = category };
                itemDict[name] = item;
                
                // Icon: First column
                var img = cells[0].SelectSingleNode(".//img");
                if (img != null)
                {
                    item.IconUrl = img.GetAttributeValue("data-src", img.GetAttributeValue("src", ""));
                    if (item.IconUrl.Contains("/revision/"))
                        item.IconUrl = item.IconUrl.Split("/revision/")[0];
                }

                var link = cells[1].SelectSingleNode(".//a");
                if (link != null)
                    item.WikiUrl = WikiBaseUrl + link.GetAttributeValue("href", "");
            }

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

            // Extract Quest or Module name
            // Usually in an <a> tag
            var links = li.SelectNodes(".//a");
            if (links != null)
            {
                foreach (var a in links)
                {
                    string linkText = a.InnerText.Trim();
                    if (string.IsNullOrEmpty(linkText) || linkText.Equals("in raid", StringComparison.OrdinalIgnoreCase)) continue;
                    
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
                        if (!item.Quests.Any(q => q.Name.Equals(linkText, StringComparison.OrdinalIgnoreCase)))
                        {
                            item.Quests.Add(new RequirementDetail { Name = linkText, Count = count, IsFir = isFir });
                            item.Requirements.Total += count;
                            if (isFir) item.Requirements.FoundInRaid += count;
                        }
                    }
                    else if (targetList == "Hideout")
                    {
                        if (!item.HideoutModules.Any(m => m.Name.Equals(linkText, StringComparison.OrdinalIgnoreCase)))
                        {
                            item.HideoutModules.Add(new RequirementDetail { Name = linkText, Count = count, IsFir = isFir });
                            item.Requirements.Total += count;
                            if (isFir) item.Requirements.FoundInRaid += count;
                        }
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
                string html = "";
                string staticPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "collector_static.html");
                
                if (System.IO.File.Exists(staticPath))
                {
                    html = await System.IO.File.ReadAllTextAsync(staticPath);
                }
                else
                {
                    return items; // Statik dosya yoksa boş döndür
                }
                
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Look for table with class "table-progress-tracking" or id that starts with "tpt-"
                var tableContainer = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'table-progress-tracking')]") 
                                     ?? doc.DocumentNode.SelectSingleNode("//table[contains(@id, 'tpt-')]")
                                     ?? doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'wikitable')]");
                
                if (tableContainer == null)
                {
                    return items;
                }

                var rows = tableContainer.SelectNodes(".//tr");
                if (rows == null) return items;

                // string category = "Collector"; // Unused
                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.SelectNodes("./td | ./th");
                        if (cells == null || cells.Count < 4) continue;

                        // Skip header rows or rows with insufficient cells
                        // Collector table has 6 columns: checkbox, icon, name, amount, requirement, FIR
                        if (cells.Count < 5)
                        {
                            continue;
                        }

                        var item = new LootItem { Category = "Collector" };

                        // Icon: Second cell (index 1) - first cell is checkbox
                        var iconImg = cells[1].SelectSingleNode(".//img");
                        
                        if (iconImg != null)
                        {
                            item.IconUrl = iconImg.GetAttributeValue("data-src", iconImg.GetAttributeValue("src", ""));
                            if (item.IconUrl.Contains("/revision/"))
                                item.IconUrl = item.IconUrl.Split("/revision/")[0];
                        }

                        // Name: Third cell (index 2)
                        var nameLink = cells[2].SelectSingleNode(".//a[@href]");
                        if (nameLink != null)
                        {
                            item.Name = nameLink.InnerText.Trim();
                            item.WikiUrl = WikiBaseUrl + nameLink.GetAttributeValue("href", "");
                        }
                        else
                        {
                            item.Name = cells[2].InnerText.Trim();
                        }

                        if (string.IsNullOrEmpty(item.Name))
                        {
                            continue;
                        }

                        // Amount: Fourth cell (index 3)
                        int amount = 1;
                        var amountText = cells[3].InnerText.Trim();
                        if (int.TryParse(amountText, out int parsed))
                        {
                            amount = parsed;
                        }
                        item.Requirements.Total = amount;

                        // Find in Raid: Last cell (index 5) contains <font color="red">Yes</font>
                        bool isFir = false;
                        if (cells.Count > 5)
                        {
                            var firCell = cells[5];
                            var firHtml = firCell.InnerHtml;
                            var firText = firCell.InnerText.Trim().ToLower();
                            
                            if (firText == "yes" && (firHtml.Contains("color=\"red\"") || firHtml.Contains("color:red")))
                            {
                                isFir = true;
                            }
                        }

                        if (isFir)
                        {
                            item.Requirements.FoundInRaid = item.Requirements.Total;
                        }

                        item.Quests.Add(new RequirementDetail 
                        { 
                            Name = "Collector", 
                            Count = item.Requirements.Total, 
                            IsFir = isFir 
                        });

                        items.Add(item);
                    }
                    catch
                    {
                        // Skip problematic rows
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Collector scraping failed: {ex.Message}", ex);
            }

            return items;
        }
    }
}
