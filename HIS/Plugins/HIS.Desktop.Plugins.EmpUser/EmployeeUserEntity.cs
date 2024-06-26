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

namespace HIS.Desktop.Plugins.EmpUser
{
    class EmployeeUserEntity : ACS.EFMODEL.DataModels.ACS_USER
    {
        public string ERX_LOGINNAME { get; set; }
        public string ERX_PASSWORD { get; set; }
        public decimal? MAX_SERVICE_REQ_PER_DAY { get; set; }
        public EmployeeUserEntity()
        {

        }
        public EmployeeUserEntity(ACS.EFMODEL.DataModels.ACS_USER user, MOS.EFMODEL.DataModels.HIS_EMPLOYEE employee)
        {
            LOGINNAME = user.LOGINNAME;
            USERNAME = user.USERNAME;
            DOB = employee.DOB;
            EMAIL = user.EMAIL;
            MOBILE = user.MOBILE;
            G_CODE = user.G_CODE;
            IS_ACTIVE = user.IS_ACTIVE;
            CREATE_TIME = user.CREATE_TIME;
            DIPLOMA = employee.DIPLOMA;
            IS_DOCTOR = employee.IS_DOCTOR;
            IS_ADMIN = employee.IS_ADMIN;
            IS_NURSE = employee.IS_NURSE;
            BANK = employee.BANK;
            ACCOUNT_NUMBER = employee.ACCOUNT_NUMBER;
            DEPARTMENT_ID = employee.DEPARTMENT_ID;
            DEFAULT_MEDI_STOCK_IDS = employee.DEFAULT_MEDI_STOCK_IDS;
            ALLOW_UPDATE_OTHER_SCLINICAL = employee.ALLOW_UPDATE_OTHER_SCLINICAL;
            MEDICINE_TYPE_RANK = employee.MEDICINE_TYPE_RANK;
            MAX_BHYT_SERVICE_REQ_PER_DAY = employee.MAX_BHYT_SERVICE_REQ_PER_DAY;
            CREATOR = employee.CREATOR;
            MODIFY_TIME = employee.MODIFY_TIME;
            MODIFIER = employee.MODIFIER;
            ERX_LOGINNAME = employee.ERX_LOGINNAME;
            ERX_PASSWORD = employee.ERX_PASSWORD;
            MAX_SERVICE_REQ_PER_DAY = employee.MAX_SERVICE_REQ_PER_DAY;
            IS_SERVICE_REQ_EXAM = employee.IS_SERVICE_REQ_EXAM;
        }
        public EmployeeUserEntity(ACS.EFMODEL.DataModels.ACS_USER user)
        {
            LOGINNAME = user.LOGINNAME;
            USERNAME = user.USERNAME;
            EMAIL = user.EMAIL;
            MOBILE = user.MOBILE;
            G_CODE = user.G_CODE;
            IS_ACTIVE = user.IS_ACTIVE;
            CREATE_TIME = user.CREATE_TIME;
            CREATOR = user.CREATOR;
            MODIFIER = user.MODIFIER;
            MODIFY_TIME = user.MODIFY_TIME;
            DIPLOMA = null;
            IS_DOCTOR = 0;
            IS_NURSE = 0;
            IS_ADMIN = 0;
            MEDICINE_TYPE_RANK = null;
            MAX_BHYT_SERVICE_REQ_PER_DAY = null;
            DEPARTMENT_ID = null;
            DEFAULT_MEDI_STOCK_IDS = null;
            ALLOW_UPDATE_OTHER_SCLINICAL = null;
            BANK = null;
            ACCOUNT_NUMBER = null;


        }
        public string DIPLOMA { get; set; }
        public long ID { get; set; }
        public short? IS_ADMIN { get; set; }
        public short? IS_DOCTOR { get; set; }
        public short? IS_NURSE { get; set; }
        public string ACCOUNT_NUMBER { get; set; }
        public string BANK { get; set; }
        public long? DEPARTMENT_ID { get; set; }
        public string DEFAULT_MEDI_STOCK_IDS { get; set; }
        public short? ALLOW_UPDATE_OTHER_SCLINICAL { get; set; }
        public long? MAX_BHYT_SERVICE_REQ_PER_DAY { get; set; }
        public long? MEDICINE_TYPE_RANK { get; set; }
        public long? DOB { get; set; }
        public short? IS_SERVICE_REQ_EXAM { get; set; }
    }
}
