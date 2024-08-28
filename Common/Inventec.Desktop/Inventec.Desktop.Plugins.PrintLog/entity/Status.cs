using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Plugins.PrintLog.Entity
{
    public class Status
    {
        public long BranchCode { get; set; }
        public string BranchName { get; set; }

        public Status(long _BranchCode, string _BranchName)
        {
            this.BranchCode = _BranchCode;
            this.BranchName = _BranchName;
        }
    }
}
