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

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    class RequestUriStore
    {
        internal const string HIS_SERE_SERV_TEMP__GET = "api/HisSereServTemp/Get";
        internal const string HIS_SERE_SERV_EXT_GET = "api/HisSereServExt/Get";
        internal const string HIS_SERE_SERV_EXT_CREATE = "api/HisSereServExt/CreateWithFile";
        internal const string HIS_SERE_SERV_EXT_UPDATE = "api/HisSereServExt/UpdateWithFile";
        internal const string HIS_SERE_SERV_EXT_CREATE_SDO = "api/HisSereServExt/CreateSdo";
        internal const string HIS_SERE_SERV_EXT_UPDATE_SDO = "api/HisSereServExt/UpdateSdo";
        internal const string HIS_SERVICE_REQ_FINISH = "api/HisServiceReq/Finish";
        internal const string SAR_PRINT_TYPE_GET = "api/SarPrintType/Get";
        internal const string HIS_SERE_SERV_UPDATE_WITH_FILE = "api/HisSereServ/UpdateWithFile";
        internal const string HIS_SERE_SERV_FILE_GET = "/api/HisSereServFile/Get";
        internal const string HIS_SERVICE_REQ_GETVIEW = "api/HisServiceReq/GetView";
        internal const string HIS_TREATMENT_GET = "api/HisTreatment/Get";
        internal const string HIS_SERE_SERV_GETVIEW_5 = "api/HisSereServ/GetView5";
        internal const string HIS_SERE_SERV_GET = "api/HisSereServ/Get";
        internal const string HIS_SERVICE_REQ_GET = "api/HisServiceReq/Get";
        internal const string HIS_TREATMENT_GET_TREATMENT_WITH_PATIENT_TYPE_INFO_SDO = "api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo";
        internal const string HIS_SERE_SERV_BILL_GET = "api/HisSereServBill/Get";
        internal const string HIS_SERE_SERV_DEPOSIT_GET = "api/HisSereServDeposit/Get";
        internal const string HIS_SESE_DEPO_REPAY_GET = "api/HisSeseDepoRepay/Get";
        internal const string PACS_SERIVCE__LAY_THONG_TIN_ANH = "api/His/LayThongTinHinhAnh";
        internal const string HIS_SERE_SERV_FILE_DELETE = "/api/HisSereServFile/Delete";
        internal const string HIS_SERE_SERV_FILE_CREATE = "/api/HisSereServFile/Create";
        internal const string HIS_SERE_SERV_FILE_UPDATE = "/api/HisSereServFile/Update";

        internal const string HIS_SERVICE_REQ__SURG_ASSIGN_AND_COPY = "api/HisServiceReq/SurgAssignAndCopy";
    }
}
