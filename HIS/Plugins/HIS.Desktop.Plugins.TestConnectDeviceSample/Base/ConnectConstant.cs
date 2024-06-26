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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.Base
{
    public class ConnectConstant
    {
        public const string HEADER = "$$$";
        public const string FOOTER = "%%%";

        //ban tin gui xuong thiet bi

        internal const string MESSAGE_CONNECT = "CONNECT";
        internal const string MESSAGE_DISCONNECT = "DISCONNECT";
        internal const string MESSAGE_PING = "PING";
        internal const string MESSAGE_PROCESS = "PROCESS";

        //phuc vu cat va noi message
        internal const char MESSAGE_SEPARATOR = ',';

        //response_code
        internal const string RESPONSE_SUCCESS = "0";
        internal const string RESPONSE_CORRECT = "-";
    }
}
