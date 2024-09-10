using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.VoiceCommand
{
    public class ResultCommandADO
    {
        public ResultCommandADO() { }

        public string commandActionLink { get; set; }
        public int commandType { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public string messageError { get; set; }
        public bool success { get; set; }
        public string text { get; set; }
        public string _text { get; set; }
        public object entities { get; set; }
        public object traits { get; set; }
        public object[] intents { get; set; }
        public object control { get; set; }
        public object id { get; set; }
    }
}
