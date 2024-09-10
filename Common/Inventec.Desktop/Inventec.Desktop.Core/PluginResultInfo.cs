using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Core
{
    public class PluginResultInfo
    {
        public PluginResultInfo() { }
        public PluginInfo PluginInfo { get; set; }
        public ExtensionInfo ExtensionInfo { get; set; }
        public ExtensionPointInfo ExtensionPointInfo { get; set; }
    }
}
