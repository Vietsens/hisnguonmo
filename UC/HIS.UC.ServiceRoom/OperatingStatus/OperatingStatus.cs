using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ServiceRoom.OperatingStatus
{
    public static class OperatingStatus
    {
        public static class Status
        {
            public static Dictionary<string, MemoryStream> LayoutPerUser = new Dictionary<string, MemoryStream>();
        }
    }
}
