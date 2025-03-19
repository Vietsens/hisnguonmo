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
