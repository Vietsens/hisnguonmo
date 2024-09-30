using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisExecuteRoom.ADO
{
    public class ConfigSettingsADO
    {
        public ConfigSettingsADO() { }
        public string NAME { get; set; }
        public string ID_CONFIG { get; set; }
        public string VALUE_CONFIG { get; set; }
        public bool IS_VALUE { get; set; }
    }
}
