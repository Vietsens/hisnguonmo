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
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Rs232
{
    internal class DeviceConfig
    {
        public static int BaudRate = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["PortConfig.BaudRate"]);
        public static Parity Parity = (Parity)Enum.Parse(typeof(Parity), System.Configuration.ConfigurationSettings.AppSettings["PortConfig.Parity"]);
        public static StopBits StopBits = (StopBits)Enum.Parse(typeof(StopBits), System.Configuration.ConfigurationSettings.AppSettings["PortConfig.StopBits"]);
        public static Handshake Handshake = (Handshake)Enum.Parse(typeof(Handshake), System.Configuration.ConfigurationSettings.AppSettings["PortConfig.Handshake"]);
        public static int DataBits = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["PortConfig.DataBits"]);
        public static string PortName = System.Configuration.ConfigurationSettings.AppSettings["PortConfig.PortName"];

        public static void SetPortNameToConfig(string portName)
        {
            try
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                //config.AppSettings.Settings["PortConfig.PortName"].Value = portName;
                //config.AppSettings.Settings["Mos.ServiceUri.Port"].Value = txtPort.Text;
                //config.Save(ConfigurationSaveMode.Modified, false);
                //ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                PortName = portName;
            }
            catch (Exception ex)
            {
            }
        }
    }
}
