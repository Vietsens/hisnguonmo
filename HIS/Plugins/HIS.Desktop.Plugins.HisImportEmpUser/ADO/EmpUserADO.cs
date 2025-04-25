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
using MOS.EFMODEL.DataModels;
using ACS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisImportEmpUser.ADO
{
    class EmpUserADO : ACS_USER
    {
        public string ERROR { get; set; }
        public string DIPLOMA { get; set; }
        public string IS_ADMIN_STR { get; set; }
        public string IS_DOCTOR_STR { get; set; }
        public string CONG_VIEC { get; set; }
        public string IS_NURSE_STR { get; set; }
        public string MEDICINE_TYPE_RANK { get; set; }
        public string ACCOUNT_NUMBER { get; set; }
        public string BANK { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string MAX_BHYT { get; set; }
        public string IDENTIFICATION_NUMBER { get; set; }
        public string SOCIAL_INSURANCE_NUMBER { get; set; }
        public string DOB_STR { get; set; }
        public long? DOB { get; set; }
        public long? DIPLOMA_DATE { get; set; }
        public string TITLE { get; set; }
        public string ERX_LOGINNAME { get; set; }
        public string ERX_PASSWORD { get; set; }
        public string DIPLOMA_DATE_STR { get; set; }
        public string DIPLOMA_PLACE { get; set; }
        public string MAX_SERVICE_REQ_PER_DAY_STR { get; set; }
        public long? MAX_SERVICE_REQ_PER_DAY { get; set; }
        public string ALLOW_UPDATE_OTHER_SCLINICAL_STR { get; set; }
        public short? ALLOW_UPDATE_OTHER_SCLINICAL { get; set; }
        public short? DO_NOT_ALLOW_SIMULTANEITY { get; set; }
        public short? IS_LIMIT_SCHEDULE { get; set; }
        public short? IS_NEED_SIGN_INSTEAD { get; set; }
        public string DO_NOT_ALLOW_SIMULTANEITY_STR { get; set; }
        public string IS_LIMIT_SCHEDULE_STR { get; set; }
        public string IS_NEED_SIGN_INSTEAD_STR { get; set; }
        public string EMPLOYEE_CODE { get; set; }

    }
}
