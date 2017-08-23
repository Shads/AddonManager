using Newtonsoft.Json;
using System.IO;

namespace WoWAddonsManager.Config
{
    public static class ConfigManager
    {
        private static readonly string _standardConfig = "addons.config";
        private static AddonConfig _config = null;


        public static AddonConfig GetConfig()
        {
            if (_config == null)
            {
                _config = File.Exists(_standardConfig)
                        ? JsonConvert.DeserializeObject<AddonConfig>(File.ReadAllText(_standardConfig))
                        : new AddonConfig();
            }

            return _config;
        }

        public static void SetConfig(AddonConfig config)
        {
            File.WriteAllText(_standardConfig, JsonConvert.SerializeObject(config));
        }
    }
}
