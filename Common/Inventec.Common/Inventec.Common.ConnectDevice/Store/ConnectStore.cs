using Inventec.Common.ConnectDevice.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice.Store
{
    class ConnectStore
    {
        //Ma ung dung yeu cau thiet bi mo
        internal string APPCODE;

        //Message Id
        internal string messageIdWho;
        internal string messageIdConnect;
        internal string messageIdOpen;
        internal string messageIdDisconnect;
        internal string messageIdSend;

        //Thong tin ket noi thiet bi
        internal GetConnectTerminalInfo connectTerminalInfo;

        //phuc vu xu ly ban tin gui len
        internal bool isConnecting = false;
        internal bool isTimeout = false;

        //Key ma hoa va giai ma phuc vu cho phien lam viec hien tai
        internal string SessionKey;

        //timer phuc vu timeout thiet bi tra loi
        internal System.Threading.Timer timeout;
    }
}
