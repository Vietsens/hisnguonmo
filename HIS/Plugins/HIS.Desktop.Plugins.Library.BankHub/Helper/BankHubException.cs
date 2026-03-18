using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.BankHub.Helper
{
    /// <summary>Exception đặc thù cho BankHub Client</summary>
    public class BankHubException : Exception
    {
        public BankHubException(string message) : base(message) { }
        public BankHubException(string message, Exception inner) : base(message, inner) { }
    }
}
