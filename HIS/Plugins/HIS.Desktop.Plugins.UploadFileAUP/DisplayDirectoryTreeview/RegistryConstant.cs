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

namespace HIS.Desktop.Plugins.UploadFileAUP
{
    internal class RegistryConstant
    {
        internal const string SOFTWARE_FOLDER = "SOFTWARE";
        internal const string COMPANY_FOLDER = "INVENTEC";
        internal static readonly string APP_FOLDER = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];

        internal const string DATA_KEY = "WorkingUploadFilePath";
    }
}
