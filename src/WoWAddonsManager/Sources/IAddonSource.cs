using System;
using System.Threading.Tasks;
using WoWAddonsManager.Config;

namespace WoWAddonsManager.Sources
{
    interface IAddonSource
    {
        Task<AddonConfigItem> GetAddonDetails(string addonUrl, bool awaitConfig = false);
        Task GetDownloadUrl(AddonConfigItem config);
        Task<string> GetZipFile(AddonConfigItem config);
        Task RefreshConfig(AddonConfig config, Action onComplete);
    }
}
