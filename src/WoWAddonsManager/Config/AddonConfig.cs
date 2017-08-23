using System;
using System.Collections.Generic;

namespace WoWAddonsManager.Config
{
    public class AddonConfig
    {
        public AddonConfig()
        {
            Items = new List<AddonConfigItem>();
        }

        public string FolderPath { get; set; }
        public List<AddonConfigItem> Items { get; set; }
    }

    public class AddonConfigItem
    {
        public AddonConfigItem()
        {
            Id = Guid.NewGuid();
        }

        internal Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Version { get; set; }
        public string SiteVersion { get; set; }
        public string Supports { get; set; }
        public string Url { get; set; }
        public string DownloadLink { get; set; }
        internal string ArchivePath { get; set; }
    }
}
