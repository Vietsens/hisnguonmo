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

namespace HIS.Desktop.Plugins.Library.PrintPrescription
{
    class RequestUriStore
    {
        internal const string HIS_SERE_SERV_GET = "api/HisSereServ/Get";
        internal const string HIS_EXP_MEST_MATERIAL_GET = "api/HisExpMestMaterial/Get";
        internal const string HIS_EXP_MEST_MATY_REQ_GET = "api/HisExpMestMatyReq/Get";
        internal const string HIS_EXP_MEST_MEDICINE_GET = "api/HisExpMestMedicine/Get";
        internal const string HIS_EXP_MEST_METY_REQ_GET = "api/HisExpMestMetyReq/Get";
        internal const string HIS_SERVICE_REQ_MATY_GET = "api/HisServiceReqMaty/Get";
        internal const string HIS_SERVICE_REQ_METY_GET = "api/HisServiceReqMety/Get";
        internal const string HIS_PATIENT_TYPE_ALTER_GET_APPLIED = "api/HisPatientTypeAlter/GetApplied";
        internal const string HIS_DHST_GET = "api/HisDhst/Get";
        internal const string HIS_SERVICE_REQ_GET = "api/HisServiceReq/Get";
        internal const string HIS_PATIENT_GETVIEW = "api/HisPatient/GetView";
        internal const string HIS_TREATMENT_GET = "api/HisTreatment/Get";
        internal const string HIS_MEDI_RECORD_GET = "api/HisMediRecord/Get";
    }
}
