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
using Inventec.Core;

namespace HIS.Desktop.Plugins.DebateDiagnostic.Base
{
    class GlobalStore
    {
        public static List<MOS.EFMODEL.DataModels.HIS_ICD> HisIcds
        {
            get
            {
                return HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
                        <MOS.EFMODEL.DataModels.HIS_ICD>().OrderByDescending(o => o.ICD_CODE).ToList();
            }
        }

        public static List<ACS.EFMODEL.DataModels.ACS_USER> HisAcsUser
        {
            get
            {
                return HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
                        <ACS.EFMODEL.DataModels.ACS_USER>().Where(p => !string.IsNullOrEmpty(p.USERNAME) && p.IS_ACTIVE ==1).OrderBy(o => o.USERNAME).ToList();
            }
        }
    }
}
