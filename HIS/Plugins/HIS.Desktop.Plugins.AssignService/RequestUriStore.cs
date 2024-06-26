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

namespace HIS.Desktop.Plugins.AssignService
{
    class RequestUriStore
    {
        internal const string HIS_SERVICE_REQ_GET_DYNAMIC = "api/HisServiceReq/GetDynamic";
        public const string HIS_SERE_SERV_GET = "api/HisSereServ/Get";
        public const string HIS_SERE_SERV_GETVIEW_1 = "api/HisSereServ/GetView1";
        public const string HIS_SERE_SERV_GETVIEW_7 = "api/HisSereServ/GetView7";
        public const string HIS_SERE_SERV_GETVIEW_8 = "api/HisSereServ/GetView8";
        public const string HIS_SERE_SERV_GETD_1 = "api/HisSereServ/GetViewD1";
        public const string HIS_TREATMENT_GETVIEW_7 = "api/HisTreatment/GetView7";
        public const string HIS_TREATMENT_GET_TREATMENT_WITH_PATIENT_TYPE_INFO_SDO = "api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo";
        public const string HIS_TEST_SERVICE_REQ_GETVIEW = "api/HisTestServiceReq/GetView";
        public const string HIS_PATIENT_GETVIEW = "api/HisPatient/GetView";
        public const string HIS_TEST_SERVICE_REQ_GET = "api/HisTestServiceReq/Get";
        public const string HIS_TREATMENT_GET = "api/HisTreatemnt/Get";
        public const string HIS_SERVICE_REQ_GET = "api/HisServiceReq/Get";
        public const string HIS_SERVICE_REQ_GETVIEW_6 = "api/HisServiceReq/GetView6";
        public const string HIS_SERVICE_REQ__ASSIGN_SERVICE = "api/HisServiceReq/AssignServiceByInstructionTimes";
        public const string HIS_PATIENT__GET_CARD_BALANCE = "api/HisPatient/GetCardBalance";
    }
}
