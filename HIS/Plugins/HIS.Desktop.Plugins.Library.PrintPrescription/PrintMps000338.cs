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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.Plugins.Library.PrintPrescription.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintPrescription
{
    class PrintMps000338
    {
        private HIS_SERVICE_REQ HisServiceReq;
        List<HIS_SERE_SERV> SereServs;
        List<V_HIS_SERE_SERV> sereServsDK;
        V_HIS_SERE_SERV sereServ;
        string treatmentCode = "";

        short IS_TRUE = 1;

        private int numCopy;
        private bool printNow;
        int VatTu = 2;

        Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        private Inventec.Desktop.Common.Modules.Module currentModule;
        MPS.ProcessorBase.PrintConfig.PreviewType? previewType;

        public PrintMps000338(string printTypeCode, string fileName, ref bool result,
            MOS.SDO.SubclinicalPresResultSDO currentSubclinicalPresResultSDO, V_HIS_SERE_SERV currentSereServ,
            bool printNow, bool hasMediMate,
            Inventec.Common.RichEditor.RichEditorStore _richEditorMain, Inventec.Desktop.Common.Modules.Module module,
            MPS.ProcessorBase.PrintConfig.PreviewType? previewType,
            Action<int> countData,
            Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint)
        {
            try
            {
                this.currentModule = module;
                this.sereServ = currentSereServ;
                this.printNow = printNow;
                this.previewType = previewType;
                this.richEditorMain = _richEditorMain;

                if (currentSubclinicalPresResultSDO != null)
                {
                    CommonParam param = new CommonParam();
                    List<long> serviceReqIds = new List<long>();
                    if (currentSubclinicalPresResultSDO != null &&
                        currentSubclinicalPresResultSDO.ServiceReqs != null &&
                        currentSubclinicalPresResultSDO.ServiceReqs.Count > 0)
                    {
                        HisServiceReq = currentSubclinicalPresResultSDO.ServiceReqs.FirstOrDefault();
                        serviceReqIds = currentSubclinicalPresResultSDO.ServiceReqs.Select(o => o.ID).ToList();
                    }

                    //
                    if (countData != null)
                    {
                        countData(1);
                    }

                    // thông tin chung
                    MOS.Filter.HisSereServViewFilter filterComm = new HisSereServViewFilter();
                    filterComm.ID = sereServ.ID;
                    param = new CommonParam();
                    V_HIS_SERE_SERV sereServComm = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, filterComm, param).FirstOrDefault();
                    Inventec.Common.Logging.LogSystem.Debug("get sereServ common");

                    // đính kèm
                    if (sereServ.ID > 0)
                    {
                        param = new CommonParam();
                        MOS.Filter.HisSereServViewFilter filter = new HisSereServViewFilter();
                        filter.PARENT_ID = sereServ.ID;
                        filter.SERVICE_REQ_IDs = serviceReqIds;
                        //filter.IS_EXPEND = true;
                        sereServsDK = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        Inventec.Common.Logging.LogSystem.Debug("filter dinh kem: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                    }

                    V_HIS_TREATMENT_FEE treatment = new V_HIS_TREATMENT_FEE();

                    MOS.Filter.HisTreatmentFeeViewFilter treatmentFilter = new HisTreatmentFeeViewFilter();
                    treatmentFilter.ID = HisServiceReq.TREATMENT_ID;
                    param = new CommonParam();
                    treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                    Inventec.Common.Logging.LogSystem.Debug("get treatment");

                    treatmentCode = (treatment != null ? treatment.TREATMENT_CODE : "");

                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();

                    MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                    patientTypeAlterFilter.TREATMENT_ID = HisServiceReq.TREATMENT_ID;
                    param = new CommonParam();
                    var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                    if (patientTypeAlters != null && patientTypeAlters.Count() > 0)
                    {
                        patientTypeAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                    }
                    Inventec.Common.Logging.LogSystem.Debug("get patientTypeAlter");

                    Inventec.Common.Logging.LogSystem.Debug("sereServs count" + sereServsDK.Count());

                    MOS.EFMODEL.DataModels.V_HIS_BED_LOG hisBedLog = new V_HIS_BED_LOG();

                    param = new CommonParam();
                    MOS.Filter.HisTreatmentBedRoomFilter treatmentBedroomFilter = new HisTreatmentBedRoomFilter();
                    treatmentBedroomFilter.TREATMENT_ID = HisServiceReq.TREATMENT_ID;
                    var treatmentBedRooms = new BackendAdapter(param).Get<List<HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/Get", ApiConsumer.ApiConsumers.MosConsumer, treatmentBedroomFilter, null);

                    if (treatmentBedRooms != null && treatmentBedRooms.Count() > 0)
                    {
                        MOS.Filter.HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                        bedLogFilter.TREATMENT_BED_ROOM_IDs = treatmentBedRooms.Select(o => o.ID).Distinct().ToList();
                        var begLogs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogFilter, null);
                        hisBedLog = begLogs.OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    MPS.Processor.Mps000338.PDO.Mps000338PDO pdo = new MPS.Processor.Mps000338.PDO.Mps000338PDO(
                    sereServComm,
                    sereServsDK,
                    treatment,
                    patientTypeAlter,
                    HisServiceReq,
                    hisBedLog
                    );

                    Print.PrintData(printTypeCode, fileName, pdo, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, 1, savedData);
                }
            }
            catch (Exception ex)
            {
                cancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
