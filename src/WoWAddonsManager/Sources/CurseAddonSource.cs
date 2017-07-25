using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WoWAddonsManager.Models;

namespace WoWAddonsManager.Sources
{
    public class CurseAddonSource : IAddonSource
    {
        private string baseUrl = @"http://mods.curse.com/addons/wow/";

        public async Task GetAvailableVersion(Addon addon)
        {
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                response = await client.GetAsync(baseUrl + addon.Name.Replace(" ", "-")).ConfigureAwait(false);
            }

            if (response.IsSuccessStatusCode)
            {
                using (var pageStream = await response.Content.ReadAsStreamAsync())
                {
                    var page = new HtmlDocument();
                    page.Load(pageStream);

                    addon.AvailableVersion = page.DocumentNode.Descendants("li").Single(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "newest-file").InnerText;
                    addon.AvailableVersionSupport = page.DocumentNode.Descendants("li").Single(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("version")).InnerText;
                }
            }
        }
    }
}
