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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InvoiceCreateForTreatment.Config
{
    internal class AppConfig
    {
        private const string HFS_KEY__PAY_FORM_CODE = "HFS_KEY__PAY_FORM_CODE";

        private static string HisPayFormCode__Default;
        public static string HIS_PAY_FORM_CODE__DEFAULT
        {
            get
            {
                if (String.IsNullOrEmpty(HisPayFormCode__Default))
                {
                    HisPayFormCode__Default = String.IsNullOrEmpty(ConfigApplicationWorker.Get<string>(HFS_KEY__PAY_FORM_CODE)) ? GlobalVariables.HIS_PAY_FORM_CODE__CONSTANT : ConfigApplicationWorker.Get<string>(HFS_KEY__PAY_FORM_CODE);
                }
                return HisPayFormCode__Default;
            }
        }
    }
}
