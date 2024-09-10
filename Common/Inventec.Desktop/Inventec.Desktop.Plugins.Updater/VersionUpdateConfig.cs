using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Plugins.Updater
{
    class VersionUpdateConfig
    {
        internal static string APPLICATION_CODE = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];
        static string strApplicationDirectory;
        internal static string ApplicationDirectory
        {
            get
            {
                try
                {
                    if (System.String.IsNullOrEmpty(strApplicationDirectory))
                    {
                        string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                        System.UriBuilder uri = new System.UriBuilder(codeBase);
                        string path = System.Uri.UnescapeDataString(uri.Path);
                        strApplicationDirectory = System.IO.Path.GetDirectoryName(path);
                    }

                    return strApplicationDirectory;
                }
                catch (System.Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug(ex);
                }
                return null;
            }
        }
    }
}
