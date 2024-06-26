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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.KskContract.ADO
{
    public class EmployeeADO : V_HIS_EMPLOYEE
    {
        public string USERNAME { get; set; }
        public string DOB_ForDisplay { get; set; }
        public bool isChecked { get; set; }

        public EmployeeADO()
        {
        }

        public EmployeeADO(V_HIS_EMPLOYEE data)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<EmployeeADO>(this, data);
                this.DOB_ForDisplay = data.DOB != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0) : "";
                this.USERNAME = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.LOGINNAME == data.LOGINNAME).Select(o => o.USERNAME).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
