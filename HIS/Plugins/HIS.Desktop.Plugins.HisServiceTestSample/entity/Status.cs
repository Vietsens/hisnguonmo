using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisServiceTestSample.entity
{
    internal class Status
    {
        public long id { get; set; }
        public string statusName { get; set; }

        public Status(long id, string statusName)
        {
            this.id = id;
            this.statusName = statusName;
        }
    }
}
