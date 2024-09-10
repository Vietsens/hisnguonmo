using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice.Data
{
    public class GetConnectTerminalInfo
    {
        public GetConnectTerminalInfo()
        {
        }

        public GetConnectTerminalInfo(string connectorCode, string terminalCode, string keyToEncrypt)
        {
            this.ConnectorCode = connectorCode;
            this.TerminalCode = terminalCode;
            this.KeyToEncrypt = keyToEncrypt;
        }

        public string ConnectorCode { get; set; }
        public string KeyCipher { get; set; }
        public string KeyToEncrypt { get; set; }
        public string LicenseCipher { get; set; }
        public string TerminalCode { get; set; }
    }
}
