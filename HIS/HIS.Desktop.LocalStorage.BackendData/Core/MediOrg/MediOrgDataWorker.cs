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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData
{
    public class MediOrgDataWorker
    {
        private static List<HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO> mediOrgADOs;
        public static List<HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO> MediOrgADOs
        {
            get
            {
                try
                {
                    if (mediOrgADOs == null)
                    {
                        var mediorgs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>();
                        mediOrgADOs = (from m in mediorgs select new HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO(m)).ToList();
                    }
                    if (mediOrgADOs == null) mediOrgADOs = new List<HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO>();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                return mediOrgADOs;
            }
            set
            {
                mediOrgADOs = value;
            }
        }
    }
}
