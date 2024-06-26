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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ChooseRoom
{
    public class EmployeeADO : HIS_EMPLOYEE
    {
        public string USERNAME { get; set; }

        public EmployeeADO()
            : base()
        {
        }

        public EmployeeADO(HIS_EMPLOYEE userRoom, List<ACS.EFMODEL.DataModels.ACS_USER> users)
        {
            this.ACCOUNT_NUMBER = userRoom.ACCOUNT_NUMBER;
            this.ALLOW_UPDATE_OTHER_SCLINICAL = userRoom.ALLOW_UPDATE_OTHER_SCLINICAL;
            this.BANK = userRoom.BANK;
            this.CREATE_TIME = userRoom.CREATE_TIME;
            this.CREATOR = userRoom.CREATOR;
            this.DEFAULT_MEDI_STOCK_IDS = userRoom.DEFAULT_MEDI_STOCK_IDS;
            this.DEPARTMENT_ID = userRoom.DEPARTMENT_ID;
            this.DIPLOMA = userRoom.DIPLOMA;
            this.GROUP_CODE = userRoom.GROUP_CODE;
            this.ID = userRoom.ID;
            this.IS_ACTIVE = userRoom.IS_ACTIVE;
            this.IS_ADMIN = userRoom.IS_ADMIN;
            this.IS_DELETE = userRoom.IS_DELETE;
            this.IS_DOCTOR = userRoom.IS_DOCTOR;
            this.LOGINNAME = userRoom.LOGINNAME;
            this.MAX_BHYT_SERVICE_REQ_PER_DAY = userRoom.MAX_BHYT_SERVICE_REQ_PER_DAY;
            this.MEDICINE_TYPE_RANK = userRoom.MEDICINE_TYPE_RANK;
            this.MODIFIER = userRoom.MODIFIER;
            this.MODIFY_TIME = userRoom.MODIFY_TIME;
            var us = users != null ? users.FirstOrDefault(o => o.LOGINNAME == userRoom.LOGINNAME) : null;
            this.USERNAME = us != null ? us.USERNAME : "";
        }
    }
}
