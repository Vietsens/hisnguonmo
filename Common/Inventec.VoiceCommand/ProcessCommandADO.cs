using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.VoiceCommand
{
    public class ProcessCommandADO
    {
        public ProcessCommandADO() { }
        public byte[] Buffer { get; set; }
        public bool IsComplate { get; set; }
        public bool IsProcessing { get; set; }
        public int BytesRecorded { get; set; }
    }
}
