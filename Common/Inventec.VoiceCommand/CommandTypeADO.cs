using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.VoiceCommand
{
    public class CommandTypeADO
    {
        public CommandTypeADO() { }

        public string ModuleLink { get; set; }
        public List<string> listText { get; set; }
        public string commandActionLink { get; set; }
        public int commandType { get; set; }
    }
}
