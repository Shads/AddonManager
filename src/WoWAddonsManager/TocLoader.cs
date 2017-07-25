using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WoWAddonsManager.Extensions;
using WoWAddonsManager.Models;

namespace WoWAddonsManager
{
    internal class TocLoader
    {
        private string _addonsPath;

        public TocLoader(string addonsPath)
        {
            _addonsPath = addonsPath;
        }

        public IEnumerable<Addon> GetAddons()
        {
            var files = Directory.GetDirectories(_addonsPath).Select(d => Directory.GetFiles(d, "*.toc", SearchOption.TopDirectoryOnly).FirstOrDefault());

            return files.Where(f => !string.IsNullOrWhiteSpace(f)).Select(f => GetAddonDetails(f)).Where(a => a != null);
        }

        public Addon GetAddonDetails(string tocFilePath)
        {
            Addon addon = null;

            if (!File.Exists(tocFilePath))
            {
                return addon;
            }

            var metaData = new Dictionary<string, string>();
            using (var reader = File.OpenText(tocFilePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.StartsWith("##"))
                    {
                        var d = line.Replace("##", "").Split(':');
                        metaData.AddOrConcat(d[0].Trim(), d[1].Trim());
                    }
                }
            }

            if (metaData.ContainsKey("Title") && metaData.ContainsKey("Version"))
            {
                addon = new Addon(metaData);
            }

            return addon;
        }
    }
}
