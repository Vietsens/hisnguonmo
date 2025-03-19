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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Aup.Utility
{
    public class AupConstant
    {
        public const string extensionFileLog = "|.log|.db|";
        public const string api__AupVersion__CheckForUpdate = "api/AupVersion/CheckForUpdate";
        public const string api__AupVersion__Update = "api/AupVersion/Update";
        public const string api__AupVersion__Update_Specify = "api/AupVersion/UpdateSpecify";
        public const string api__AupVersion__UpdateFixFile = "api/AupVersion/UpdateFixFile";
        public const string api__AupVersion__CleanZipFile = "api/AupVersion/CleanZipFile";

        public static string BASE_URI = ConfigurationManager.AppSettings["Aup.uri.base"];
        public static string UPLOAD_FOLDER = ConfigurationManager.AppSettings["Aup.Folder.Upload"];
        public static string SPECIFY_FOLDER = ConfigurationManager.AppSettings["Aup.Folder.AUP"];
        public static int TIME_OUT = int.Parse(ConfigurationManager.AppSettings["Aup.timeout"]);
        public static string APP_CODE = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];
        public static string CREATE_XML_TOOL_WEB_URL = ConfigurationManager.AppSettings["Aup.CreateXmlTools.WebUrl"];
        public static string API_VERSION = ConfigurationManager.AppSettings["Aup.ApiVersion"];

        public const string HEADER_CLIENT_CODE = "aup-client-code";
        
    }
}
