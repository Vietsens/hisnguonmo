/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
