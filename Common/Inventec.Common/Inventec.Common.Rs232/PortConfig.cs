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
