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

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    class AcsUserADO : ACS.EFMODEL.DataModels.ACS_USER
    {
        public string DEPARTMENT_NAME { get; set; }

        public long? DOB { get; set; }
        public string DOB_STR { get; set; }
        public string DIPLOMA { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public long? DEPARTMENT_ID { get; set; }

        public short? IS_ACTIVE_EMPLOYEE { get; set; }
        public AcsUserADO() { }

        public AcsUserADO(ACS.EFMODEL.DataModels.ACS_USER data, MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE employee)
        {
            try
            {
                if (data != null)
                {
                    this.ID = data.ID;
                    this.IS_ACTIVE = data.IS_ACTIVE;
                    this.LOGINNAME = data.LOGINNAME;
                    this.USERNAME = data.USERNAME;
                    this.MOBILE = data.MOBILE;
                    if (employee != null)
                    {
                        this.DOB = employee.DOB;
                        this.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(employee.DOB ?? 0);
                        this.DIPLOMA = employee.DIPLOMA;
                        this.IS_ACTIVE_EMPLOYEE = employee.IS_ACTIVE;
                        this.DEPARTMENT_CODE = employee.DEPARTMENT_CODE;
                        this.DEPARTMENT_ID = employee.DEPARTMENT_ID;
                        this.DEPARTMENT_NAME = employee.DEPARTMENT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

