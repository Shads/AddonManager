using System.Collections.Generic;

namespace WoWAddonsManager.Models
{
    public class Addon
    {
        private Dictionary<string, string> _meta;

        public Addon(Dictionary<string, string> metadata)
        {
            _meta = metadata;
        }

        public string Name => _meta["Title"];
        public string Version => _meta["Version"];
        public string Path { get; set; }
        public string AvailableVersion { get; set; }
        public string AvailableVersionSupport { get; set; }
    }
}
