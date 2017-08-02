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
            if (_config == null && File.Exists(_standardConfig))
            {
                _config = JsonConvert.DeserializeObject<AddonConfig>(File.ReadAllText(_standardConfig));
            }

            return _config;
        }

        public static void SetConfig(AddonConfig config)
        {
            File.WriteAllText(_standardConfig, JsonConvert.SerializeObject(config));
        }
    }
}
