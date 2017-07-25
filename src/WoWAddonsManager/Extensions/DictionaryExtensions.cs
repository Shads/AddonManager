using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWAddonsManager.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddOrConcat(this Dictionary<string, string> dict, string key, string val)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] += $",{val}";
            }
            else
            {
                dict.Add(key, val);
            }
        }
    }
}
