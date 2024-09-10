using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ImageLib.Base
{
    public class LocalStore
    {
        /// <summary>
        /// Read folder store image while capture
        /// Default if not config "Inventec.UC.Base.LocalStore.FolderStorage" -> save local as ImageStorage
        /// </summary>
        public static string LocalStoragePathConfig = (System.Configuration.ConfigurationSettings.AppSettings["Inventec.UC.Base.LocalStore.FolderStorage"] ?? "Img/Camera");
        public static string LocalStoragePathConfigKiosk = (System.Configuration.ConfigurationSettings.AppSettings["Inventec.UC.Base.LocalStore.FolderStorageKiosk"] ?? "Img/Kiosk");

        static string strLocalStoragePath;
        public static string LOCAL_STORAGE_PATH
        {
            get
            {
                try
                {
                    if (System.String.IsNullOrEmpty(strLocalStoragePath))
                    {
                        strLocalStoragePath = System.IO.Path.Combine(ApplicationDirectory, LocalStoragePathConfig);
                    }

                    return strLocalStoragePath;
                }
                catch (System.Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug(ex);
                }
                return String.Empty;
            }
        }

        static string strLocalStoragePathKiosk;
        public static string LOCAL_STORAGE_PATH_KIOSK
        {
            get
            {
                try
                {
                    if (System.String.IsNullOrEmpty(strLocalStoragePathKiosk))
                    {
                        strLocalStoragePathKiosk = System.IO.Path.Combine(ApplicationDirectory, LocalStoragePathConfigKiosk);
                    }

                    return strLocalStoragePathKiosk;
                }
                catch (System.Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug(ex);
                }
                return String.Empty;
            }
        }

        static string strApplicationDirectory;
        internal static string ApplicationDirectory
        {
            get
            {
                try
                {
                    if (System.String.IsNullOrEmpty(strApplicationDirectory))
                    {
                        strApplicationDirectory = System.Windows.Forms.Application.StartupPath;
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
