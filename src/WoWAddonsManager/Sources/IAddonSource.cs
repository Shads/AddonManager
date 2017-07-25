using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWAddonsManager.Models;

namespace WoWAddonsManager.Sources
{
    interface IAddonSource
    {
        Task GetAvailableVersion(Addon addon);
    }
}
