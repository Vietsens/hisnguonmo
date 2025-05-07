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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintTreatmentFinish
{
    class PrintMps000174
    {
        MPS.Processor.Mps000174.PDO.Mps000174PDO mps000174RDO { get; set; }
        bool printNow { get; set; }

        public PrintMps000174(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, V_HIS_PATIENT patient, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, bool _printNow, long? roomId)
        {
            try
            {
                if (HisTreatment == null || HisTreatment.ID <= 0)
                {
                    result = false;
                    return;
                }
                this.printNow = _printNow;


                WaitingManager.Show();
                CommonParam param = new CommonParam();

                //Lấy thông tin chuyển khoa
                HisDepartmentTranViewFilter departmentTranFilter = new HisDepartmentTranViewFilter();
                departmentTranFilter.TREATMENT_ID = HisTreatment.ID;
                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departmentTranFilter, param);

                //danh sach yeu cau kham
                HisServiceReqViewFilter serviceReqViewFilter = new HisServiceReqViewFilter();
                V_HIS_SERVICE_REQ ServiceReq = null;
                serviceReqViewFilter.TREATMENT_ID = HisTreatment.ID;
                serviceReqViewFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                ServiceReq = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, serviceReqViewFilter, param).OrderBy(o => o.INTRUCTION_TIME).FirstOrDefault();

                string requestDepartmentName = "";
                if (ServiceReq != null)
                    requestDepartmentName = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == ServiceReq.REQUEST_DEPARTMENT_ID).DEPARTMENT_NAME;
                List<HIS_ICD> listICD = new List<HIS_ICD>();
                if (HisTreatment.TRANSFER_IN_ICD_CODE != null)
                    listICD.Add(new HIS_ICD { ICD_CODE = HisTreatment.TRANSFER_IN_ICD_CODE, ICD_NAME = HisTreatment.TRANSFER_IN_ICD_NAME });
                //thuoc
                MOS.Filter.HisExpMestFilter prescriptionViewFIlter = new HisExpMestFilter();
                prescriptionViewFIlter.TDL_TREATMENT_ID = HisTreatment.ID;

                var prescriptions = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, prescriptionViewFIlter, param) ?? new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>();

                List<long> expMestIds = prescriptions.Select(o => o.ID).ToList();
                MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_IDs = expMestIds;//TODO
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, param) ?? new List<V_HIS_EXP_MEST_MEDICINE>();

                HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                sereServFilter.TDL_TREATMENT_ID = HisTreatment.ID;
                List<V_HIS_SERE_SERV_5> sereServMedis = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, param).Where(o => o.MEDICINE_ID != null).ToList();

                HisDhstFilter DhstFilter = new HisDhstFilter();
                DhstFilter.TREATMENT_ID = HisTreatment.ID;
                List<HIS_DHST> listDhst = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, DhstFilter, param).ToList();
                var Dhst = (listDhst != null && listDhst.Count > 0) ? listDhst.First() : new HIS_DHST();

                MPS.Processor.Mps000174.PDO.Mps000174PDO.Mps000174ADO ado = new MPS.Processor.Mps000174.PDO.Mps000174PDO.Mps000174ADO();
                if (HisTreatment.TREATMENT_RESULT_ID.HasValue)
                {
                    var treatmentResult = BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == HisTreatment.TREATMENT_RESULT_ID.Value);
                    ado.TREATMENT_RESULT_CODE = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_CODE : "";
                    ado.TREATMENT_RESULT_NAME = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_NAME : "";
                }

                // get sereServ
                var executeRoomIsExam = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.IS_EXAM == 1).ToList();
                MOS.Filter.HisSereServFilter sereServFilter1 = new MOS.Filter.HisSereServFilter();
                sereServFilter1.TREATMENT_ID = HisTreatment.ID;
                sereServFilter.TDL_SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
                    , IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT };

                var sereServList = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>(ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, sereServFilter1, null);

                if (sereServList != null && sereServList.Count > 0 && executeRoomIsExam != null && executeRoomIsExam.Count > 0)
                {
                    sereServList = sereServList.Where(o => executeRoomIsExam.Select(p => p.ROOM_ID).Contains(o.TDL_REQUEST_ROOM_ID)).ToList();
                }

                WaitingManager.Hide();

                //Cấn sửa lại in bệnh án 

                MPS.Processor.Mps000174.PDO.Mps000174PDO mps000174RDO = new MPS.Processor.Mps000174.PDO.Mps000174PDO(
                    patient,
                    departmentTrans,
                    patientTypeAlter,
                    ServiceReq,
                    Dhst,
                    HisTreatment,
                    listICD,
                    prescriptions,
                    expMestMedicines,
                    requestDepartmentName,
                    ado,
                    sereServList
                );

                result = Print.RunPrint(printTypeCode, fileName, mps000174RDO, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, _printNow, roomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //public PrintMps000174(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, V_HIS_PATIENT patient, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, MPS.ProcessorBase.PrintConfig.PreviewType? _previewType, long? roomId)
        //{
        //    try
        //    {
        //        if (HisTreatment == null || HisTreatment.ID <= 0)
        //        {
        //            result = false;
        //            return;
        //        }
        //        //this.printNow = _printNow;


        //        WaitingManager.Show();
        //        CommonParam param = new CommonParam();

        //        //Lấy thông tin chuyển khoa
        //        HisDepartmentTranViewFilter departmentTranFilter = new HisDepartmentTranViewFilter();
        //        departmentTranFilter.TREATMENT_ID = HisTreatment.ID;
        //        List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departmentTranFilter, param);

        //        //danh sach yeu cau kham
        //        HisServiceReqViewFilter serviceReqViewFilter = new HisServiceReqViewFilter();
        //        V_HIS_SERVICE_REQ ServiceReq = null;
        //        serviceReqViewFilter.TREATMENT_ID = HisTreatment.ID;
        //        serviceReqViewFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
        //        ServiceReq = new BackendAdapter(param)
        //            .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, serviceReqViewFilter, param).OrderBy(o => o.INTRUCTION_TIME).FirstOrDefault();

        //        string requestDepartmentName = "";
        //        if (ServiceReq != null)
        //            requestDepartmentName = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == ServiceReq.REQUEST_DEPARTMENT_ID).DEPARTMENT_NAME;
        //        List<HIS_ICD> listICD = new List<HIS_ICD>();
        //        if (HisTreatment.TRANSFER_IN_ICD_CODE != null)
        //            listICD.Add(new HIS_ICD { ICD_CODE = HisTreatment.TRANSFER_IN_ICD_CODE, ICD_NAME = HisTreatment.TRANSFER_IN_ICD_NAME });
        //        //thuoc
        //        MOS.Filter.HisExpMestFilter prescriptionViewFIlter = new HisExpMestFilter();
        //        prescriptionViewFIlter.TDL_TREATMENT_ID = HisTreatment.ID;

        //        var prescriptions = new BackendAdapter(param)
        //            .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, prescriptionViewFIlter, param) ?? new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>();

        //        List<long> expMestIds = prescriptions.Select(o => o.ID).ToList();
        //        MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
        //        medicineFilter.EXP_MEST_IDs = expMestIds;//TODO
        //        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
        //            .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, param) ?? new List<V_HIS_EXP_MEST_MEDICINE>();

        //        HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
        //        sereServFilter.TDL_TREATMENT_ID = HisTreatment.ID;
        //        List<V_HIS_SERE_SERV_5> sereServMedis = new BackendAdapter(param)
        //            .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, param).Where(o => o.MEDICINE_ID != null).ToList();

        //        HisDhstFilter DhstFilter = new HisDhstFilter();
        //        DhstFilter.TREATMENT_ID = HisTreatment.ID;
        //        List<HIS_DHST> listDhst = new BackendAdapter(param)
        //            .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, DhstFilter, param).ToList();
        //        var Dhst = (listDhst != null && listDhst.Count > 0) ? listDhst.First() : new HIS_DHST();

        //        MPS.Processor.Mps000174.PDO.Mps000174PDO.Mps000174ADO ado = new MPS.Processor.Mps000174.PDO.Mps000174PDO.Mps000174ADO();
        //        if (HisTreatment.TREATMENT_RESULT_ID.HasValue)
        //        {
        //            var treatmentResult = BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == HisTreatment.TREATMENT_RESULT_ID.Value);
        //            ado.TREATMENT_RESULT_CODE = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_CODE : "";
        //            ado.TREATMENT_RESULT_NAME = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_NAME : "";
        //        }

        //        // get sereServ
        //        var executeRoomIsExam = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.IS_EXAM == 1).ToList();
        //        MOS.Filter.HisSereServFilter sereServFilter1 = new MOS.Filter.HisSereServFilter();
        //        sereServFilter1.TREATMENT_ID = HisTreatment.ID;
        //        sereServFilter.TDL_SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
        //            , IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT };

        //        var sereServList = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>(ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, sereServFilter1, null);

        //        if (sereServList != null && sereServList.Count > 0 && executeRoomIsExam != null && executeRoomIsExam.Count > 0)
        //        {
        //            sereServList = sereServList.Where(o => executeRoomIsExam.Select(p => p.ROOM_ID).Contains(o.TDL_REQUEST_ROOM_ID)).ToList();
        //        }

        //        WaitingManager.Hide();

        //        //Cấn sửa lại in bệnh án 

        //        MPS.Processor.Mps000174.PDO.Mps000174PDO mps000174RDO = new MPS.Processor.Mps000174.PDO.Mps000174PDO(
        //            patient,
        //            departmentTrans,
        //            patientTypeAlter,
        //            ServiceReq,
        //            Dhst,
        //            HisTreatment,
        //            listICD,
        //            prescriptions,
        //            expMestMedicines,
        //            requestDepartmentName,
        //            ado,
        //            sereServList
        //        );

        //        result = Print.RunPrint(printTypeCode, fileName, mps000174RDO, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, _previewType, roomId);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private bool RunPrint(string printTypeCode, string fileName, bool result, bool _printNow)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (_printNow)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000174RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint));
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000174RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000174RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return result;
        }

        private void EventLogPrint()
        {
            try
            {
                string message = "In giấy nghỉ ốm. Mã in : Mps000174" + "  TREATMENT_CODE: " + this.mps000174RDO.Treatment.TREATMENT_CODE + "  Thời gian in: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + "  Người in: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
