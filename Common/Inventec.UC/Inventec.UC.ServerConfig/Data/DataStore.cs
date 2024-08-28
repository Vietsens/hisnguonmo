using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ServerConfig.Data
{
    class DataStore
    {
        internal static Inventec.Common.XmlConfig.XmlApplicationConfig SystemConfigXLM { get; set; }
        internal static string language;

        internal static void LoadSystemConfigKey()
        {
            try
            {
                string filePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                string pathXmlFileConfig = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), "ConfigSystem.xml");
                string lang = string.IsNullOrEmpty(language) ? "vi" : language;
                DataStore.SystemConfigXLM = new Inventec.Common.XmlConfig.XmlApplicationConfig(pathXmlFileConfig, lang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
