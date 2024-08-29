using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice.Store
{
    class ConnectConstant
    {
        //header and footer
        internal const string HEADER = "$$$";
        internal const string FOOTER = "%%%";

        //ban tin gui xuong thiet bi

        internal const string MESSAGE_WHO = "WHO";
        internal const string MESSAGE_CONNECT = "CONNECT";
        internal const string MESSAGE_OPEN = "OPEN";
        internal const string MESSAGE_DISCONNECT = "DISCONNECT";
        internal const string MESSAGE_READCARD = "READCARD";

        //phuc vu cat va noi message
        internal const char MESSAGE_SEPARATOR = ',';

        //response_code
        internal const string RESPONSE_SUCCESS = "0";
    }
}
