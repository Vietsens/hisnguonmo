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
