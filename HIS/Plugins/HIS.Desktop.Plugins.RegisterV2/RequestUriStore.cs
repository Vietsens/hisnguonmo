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

namespace HIS.Desktop.Plugins.RegisterV2
{
    class RequestUriStore
    {
        public const string HIS_PATIEN_PROGRAM_CREATE = "/api/HisPatientProgram/Create";
        public const string HIS_PATIEN_PROGRAM_UPDATE = "/api/HisPatientProgram/Update";
        public const string HIS_PATIEN_PROGRAM_GET = "/api/HisPatientProgram/GetViewByCode";
        public const string HIS_PATIEN_PROGRAM_GET_VIEW = "/api/HisPatientProgram/GetView";
        public const string HIS_PATIEN_PROGRAM_DELETE = "/api/HisPatientProgram/Delete";
        public const string HIS_APPOINTMENT_GETV = "/api/HisAppointment/GetViewByCode";
        public const string HIS_HOUSE_HOLD_GET_BY_CODE = "api/HisHousehold/GetByCode";
        public const string HIS_PATIENT_HOUSE_HOLD_GET = "/api/HisPatientHouseHold/GetView";
        public const string HIS_CASHIER_ROOM_GET = "/api/HisCashierRoom/Get";
        public const string HIS_CARD_GETVIEWBYSERVICECODE = "api/HisCard/GetCardSdoByCode";
        public const string HIS_PATIENT_GETSDOADVANCE = "api/HisPatient/GetSdoAdvance";
        public const string HIS_SERE_SERV_GETVIEW_12 = "api/HisSereServ/GetView12";
        public const string HIS_TREATMENT_GET = "api/HisTreatment/Get";
        public const string HIS_PATIENT_TYPE_ALTER__GET = "api/HisPatientTypeAlter/Get";

        public const string HID_PERSON_GET = "api/HidPerson/Get";

        public const string HIS_TREATMENT__GET = "/api/HisTreatment/Get";
        public const string HIS_CARD__CREATE_BY_MSCODE = "api/HisCard/CreateByMSCode";//TODO
    }
}
