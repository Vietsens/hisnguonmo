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
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Rs232
{
    internal class PortConfig
    {
        public static int BaudRate = LibConfigManager.GetIntConfig("PortConfig.BaudRate");
        public static Parity Parity = (Parity)Enum.Parse(typeof(Parity), LibConfigManager.GetStringConfig("PortConfig.Parity"));
        public static StopBits StopBits = (StopBits)Enum.Parse(typeof(StopBits), LibConfigManager.GetStringConfig("PortConfig.StopBits"));
        public static Handshake Handshake = (Handshake)Enum.Parse(typeof(Handshake), LibConfigManager.GetStringConfig("PortConfig.Handshake"));
        public static int DataBits = LibConfigManager.GetIntConfig("PortConfig.DataBits");
        public static string PortName = LibConfigManager.GetStringConfig("PortConfig.PortName");

        /// <summary>
        /// Ghi portname vao file config
        /// </summary>
        /// <param name="portName"></param>
        public static void SetPortNameToConfig(string portName)
        {
            try
            {
                LibConfigManager.Write("PortConfig.PortName", portName);
                PortConfig.PortName = portName;
            }
            catch (Exception ex)
            {
                //Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
