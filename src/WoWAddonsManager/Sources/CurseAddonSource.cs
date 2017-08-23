using HtmlAgilityPack;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WoWAddonsManager.Config;

namespace WoWAddonsManager.Sources
{
    public class CurseAddonSource : IAddonSource
    {
        public async Task<AddonConfigItem> GetAddonDetails(string addonUrl, bool awaitConfig = false)
        {
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                response = await client.GetAsync(addonUrl).ConfigureAwait(awaitConfig);
            }

            AddonConfigItem addon = null;
            if (response.IsSuccessStatusCode)
            {
                using (var pageStream = await response.Content.ReadAsStreamAsync())
                {
                    var page = new HtmlDocument();
                    page.Load(pageStream);

                    addon = new AddonConfigItem
                    {
                        Name = page.DocumentNode.SelectSingleNode("//div[@id='project-overview']//h2")?.InnerText,
                        SiteVersion = page.DocumentNode.SelectSingleNode("//li[@class='newest-file']")?.InnerText?.Replace("Newest File: ", ""),
                        Supports = page.DocumentNode.SelectSingleNode("//li[@class='version']")?.InnerText?.Replace("Supports: ", ""),
                        Url = addonUrl
                    };
                }
            }

            if (addon != null)
            {
                await GetDownloadUrl(addon);
            }

            return addon;
        }

        public async Task GetDownloadUrl(AddonConfigItem config)
        {
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                response = await client.GetAsync(config.Url + "/download").ConfigureAwait(false);
            }

            if (response.IsSuccessStatusCode)
            {
                using (var pageStream = await response.Content.ReadAsStreamAsync())
                {
                    var page = new HtmlDocument();
                    page.Load(pageStream);
                    config.DownloadLink = page.DocumentNode.SelectSingleNode("//div[@id='file-download']//a[@class='download-link']").Attributes["data-href"].Value;
                }
            }
        }

        public async Task<string> GetZipFile(AddonConfigItem config)
        {
            if (!string.IsNullOrWhiteSpace(config.ArchivePath) && File.Exists(config.ArchivePath))
            {
                return config.ArchivePath;
            }

            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                response = await client.GetAsync(config.DownloadLink).ConfigureAwait(false);
            }

            var path = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                path = $"archive\\{config.Name.Replace(" ", "")}.{config.Version}.zip";
                using (var fileStream = File.Create(path))
                {
                    using (var zipStream = await response.Content.ReadAsStreamAsync())
                    {
                        zipStream.Seek(0, SeekOrigin.Begin);
                        zipStream.CopyTo(fileStream);
                    }
                }
            }

            return path;
        }

        public async Task RefreshConfig(AddonConfig config, Action onComplete)
        {
            if (config == null || config.Items.Count == 0)
            {
                return;
            }

            int itemCount = config.Items.Count;
            Task[] fetches = new Task[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                var index = i;
                var item = config.Items[index];
                fetches[index] = new TaskFactory().StartNew(() => 
                {
                    var deets = GetAddonDetails(item.Url, true).Result;
                    config.Items[index].SiteVersion = deets.SiteVersion;
                    config.Items[index].Url = deets.Url;
                    onComplete();
                });
            }

            await Task.WhenAll(fetches);
        }
    }
}
