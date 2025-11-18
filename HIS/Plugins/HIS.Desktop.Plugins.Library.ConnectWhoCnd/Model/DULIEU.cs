using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ConnectWhoCnd.Model
{
    internal class credentials
    {
        public credentials()
        {
            program = "NCD";
        }
        public string program { get; set; }
        public string token { get; set; }
    }

    internal class DULIEU
    {
        public THA THA { get; set; }
        public DTD DTD { get; set; }
    }

    internal class NcdData
    {
        public DULIEU DU_LIEU { get; set; }
    }

    internal class SendData
    {
        public credentials credentials { get; set; }
        public List<NcdData> ncdData { get; set; }
    }

    internal class OImport
    {
        public string pID { get; set; }
        public string jobReference { get; set; }
        public DateTime createdAt { get; set; }
        public string validate { get; set; }
    }
}
