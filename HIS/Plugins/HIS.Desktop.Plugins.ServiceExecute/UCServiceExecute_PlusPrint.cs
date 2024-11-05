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
using ACS.SDO;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class UCServiceExecute : UserControlBase
    {
        private void ButtonEdit_Print__PhieuKeKhai_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                onClickPhieuKeKhaiThuocVatTu(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickPhieuPTTT(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000033", DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void onClickKeThuocVatTuTieuHao(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                V_HIS_SERE_SERV sereServInput = new V_HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServInput, sereServ);
                HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(currentServiceReq.TREATMENT_ID, currentServiceReq.INTRUCTION_TIME, this.currentServiceReq.ID, sereServInput);
                assignServiceADO.IsCabinet = true;
                assignServiceADO.PatientDob = currentServiceReq.TDL_PATIENT_DOB;
                assignServiceADO.PatientName = currentServiceReq.TDL_PATIENT_NAME;
                assignServiceADO.GenderName = currentServiceReq.TDL_PATIENT_GENDER_NAME;
                assignServiceADO.TreatmentCode = currentServiceReq.TDL_TREATMENT_CODE;
                assignServiceADO.TreatmentId = currentServiceReq.TREATMENT_ID;

                assignServiceADO.IsAutoCheckExpend = true;

                listArgs.Add(assignServiceADO);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignPrescriptionCLS", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void onClickKhaiBaoThuocVTTH(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(this.sereServ.ID);

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisServiceReqMaty", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void onClickHuyPhieuThuocVatTuTieuHaoDaKe(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                HisServiceReqSDO sdo = new HisServiceReqSDO();
                sdo.Id = PopupMenuProcessor.sereServExt.SUBCLINICAL_PRES_ID;
                sdo.RequestRoomId = this.moduleData.RoomId;
                Inventec.Common.Logging.LogSystem.Debug("sdoHuyPhieuThuocVatTuTieuHaoDaKe_____________ " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                success = new BackendAdapter(param).Post<bool>("api/HisServiceReq/Delete", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuKeKhaiThuocVatTu(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000338", DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DeletegatePrintTemplate(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case "Mps000338":
                        InPhieuKeKhaiThuocVatTu(printCode, fileName, ref result);
                        break;
                    case "Mps000033":
                        InPhieuPTTT(printCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuKeKhaiThuocVatTu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var focus = (ADO.ServiceADO)gridViewSereServ.GetFocusedRow();
                if (focus != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();

                    MOS.EFMODEL.DataModels.HIS_SERVICE_REQ ServiceReq = new HIS_SERVICE_REQ();
                    MOS.Filter.HisServiceReqFilter ServiceReqFilter = new HisServiceReqFilter();
                    ServiceReqFilter.ID = focus.SERVICE_REQ_ID;
                    var serviceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, ServiceReqFilter, param);
                    if (serviceReqs != null)
                    {
                        ServiceReq = serviceReqs.FirstOrDefault();
                        Inventec.Common.Logging.LogSystem.Debug("ServiceReq: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ServiceReq), ServiceReq));
                    }

                    Inventec.Common.Logging.LogSystem.Debug("focus: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => focus), focus));

                    // thông tin chung
                    MOS.Filter.HisSereServViewFilter filterComm = new HisSereServViewFilter();
                    filterComm.ID = focus.ID;

                    V_HIS_SERE_SERV sereServComm = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, filterComm, param).FirstOrDefault();
                    Inventec.Common.Logging.LogSystem.Debug("get sereServ common");

                    // đính kèm
                    MOS.Filter.HisSereServViewFilter filter = new HisSereServViewFilter();
                    filter.PARENT_ID = focus.ID;
                    //filter.IS_EXPEND = true;
                    var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    Inventec.Common.Logging.LogSystem.Debug("filter dinh kem: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                    V_HIS_TREATMENT_FEE treatment = new V_HIS_TREATMENT_FEE();
                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                    V_HIS_BED_LOG hisBedLog = new V_HIS_BED_LOG();

                    if (focus.TDL_TREATMENT_ID.HasValue)
                    {
                        MOS.Filter.HisTreatmentFeeViewFilter treatmentFilter = new HisTreatmentFeeViewFilter();
                        treatmentFilter.ID = focus.TDL_TREATMENT_ID;
                        treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                        Inventec.Common.Logging.LogSystem.Debug("get treatment");
                    }


                    if (focus.TDL_TREATMENT_ID.HasValue)
                    {
                        MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                        patientTypeAlterFilter.TREATMENT_ID = focus.TDL_TREATMENT_ID.Value;
                        var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                        if (patientTypeAlters != null && patientTypeAlters.Count() > 0)
                        {
                            patientTypeAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                        }
                        Inventec.Common.Logging.LogSystem.Debug("get patientTypeAlter");
                    }

                    Inventec.Common.Logging.LogSystem.Debug("sereServs count" + sereServs.Count());

                    MOS.Filter.HisTreatmentBedRoomFilter treatmentBedroomFilter = new HisTreatmentBedRoomFilter();
                    treatmentBedroomFilter.TREATMENT_ID = focus.TDL_TREATMENT_ID;
                    var treatmentBedRooms = new BackendAdapter(param).Get<List<HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/Get", ApiConsumer.ApiConsumers.MosConsumer, treatmentBedroomFilter, null);

                    if (treatmentBedRooms != null && treatmentBedRooms.Count() > 0)
                    {
                        var lstTreID = treatmentBedRooms.Select(o => o.ID).Distinct().ToList();
                        if (lstBedLogData != null && lstBedLogData.Count > 0)
                        {
                            var begLogs = lstBedLogData.Where(bedLog => lstTreID.Contains(bedLog.TREATMENT_BED_ROOM_ID)).ToList();
                            hisBedLog = begLogs.OrderByDescending(o => o.START_TIME).FirstOrDefault();
                        }
                       
                    }

                    WaitingManager.Hide();
                    MPS.Processor.Mps000338.PDO.Mps000338PDO pdo = new MPS.Processor.Mps000338.PDO.Mps000338PDO(
                    sereServComm,
                    sereServs,
                    treatment,
                    patientTypeAlter,
                    ServiceReq,
                    hisBedLog
                    );

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((sereServComm != null ? sereServComm.TDL_TREATMENT_CODE : ""), printTypeCode, this.moduleData != null ? this.moduleData.RoomId : 0);
                    WaitingManager.Hide();
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuPTTT(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var focus = (ADO.ServiceADO)gridViewSereServ.GetFocusedRow();
                if (focus == null) return;

                CommonParam param = new CommonParam();
                WaitingManager.Show();

                MOS.Filter.HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                sereServFilter.ID = focus.ID;
                V_HIS_SERE_SERV_5 SereServ5 = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, param).FirstOrDefault();

                // Lấy thông tin bệnh nhân
                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = focus.TDL_PATIENT_ID;
                V_HIS_PATIENT patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                MPS.Processor.Mps000033.PDO.PatientADO currentPatient = new MPS.Processor.Mps000033.PDO.PatientADO(patient);
                //Lấy thông tin chuyển khoa
                var departmentTran = PrintGlobalStore.getDepartmentTran(focus.TDL_TREATMENT_ID ?? 0);

                //Thông tin Misu
                V_HIS_TREATMENT treatmentView = new V_HIS_TREATMENT();
                V_HIS_SERVICE_REQ ServiceReq = new V_HIS_SERVICE_REQ();

                //Khoa hien tai
                if (currentServiceReq != null)
                {
                    MOS.Filter.HisServiceReqViewFilter ServiceReqFilter = new HisServiceReqViewFilter();
                    ServiceReqFilter.ID = currentServiceReq.ID;
                    ServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, ServiceReqFilter, param).FirstOrDefault();
                    HIS_DEPARTMENT department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == currentServiceReq.REQUEST_DEPARTMENT_ID);
                    if (department != null)
                    {
                        ServiceReq.REQUEST_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                    }

                    V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentServiceReq.REQUEST_ROOM_ID);
                    if (room != null)
                    {
                        ServiceReq.REQUEST_ROOM_NAME = room.ROOM_NAME;
                    }

                    MOS.Filter.HisServiceReqViewFilter treatmentFilter = new HisServiceReqViewFilter();
                    treatmentFilter.ID = currentServiceReq.TREATMENT_ID;
                    treatmentView = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                }

                List<V_HIS_EKIP_USER> vEkipUsers = new List<V_HIS_EKIP_USER>();
                if (sereServ.EKIP_ID != null)
                {
                    HisEkipUserViewFilter ekipUserFilter = new HisEkipUserViewFilter();
                    ekipUserFilter.EKIP_ID = sereServ.EKIP_ID;
                    vEkipUsers = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER>>("api/HisEkipUser/GetView", ApiConsumers.MosConsumer, ekipUserFilter, param);
                }

                object dfdf = Activator.CreateInstance(vEkipUsers.GetType());

                MOS.Filter.HisSereServPtttViewFilter filter = new MOS.Filter.HisSereServPtttViewFilter();
                filter.SERE_SERV_ID = sereServ.ID;

                var sereServPttts = new BackendAdapter(param)
                   .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                HisSereServFileFilter fil = new HisSereServFileFilter();
                fil.SERE_SERV_ID = focus.ID;
                var sereServFile = new BackendAdapter(param)
                  .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE>>("api/HisSereServFile/Get", ApiConsumers.MosConsumer, fil, param);

                HisSesePtttMethodViewFilter filterSese = new HisSesePtttMethodViewFilter();
                filterSese.TDL_SERE_SERV_ID = focus.ID;
                var sesePtttMethod = new BackendAdapter(param)
                  .Get<List<MOS.EFMODEL.DataModels.V_HIS_SESE_PTTT_METHOD>>("api/HisSesePtttMethod/GetView", ApiConsumers.MosConsumer, filterSese, param);

                WaitingManager.Hide();
                MPS.Processor.Mps000033.PDO.Mps000033PDO rdo = new MPS.Processor.Mps000033.PDO.Mps000033PDO(currentPatient, departmentTran, ServiceReq, SereServ5, sereServExt, sereServPttts, treatmentView, vEkipUsers, null, null,null,null,null, sereServFile, sesePtttMethod);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentView != null ? treatmentView.TREATMENT_CODE : "", printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
        private void ProcessColumnMaping(HIS_SERE_SERV_TEMP sereServTemp, ref Inventec.Common.SignLibrary.ADO.InputADO emrInputADO)
        {
            try
            {
                if (sereServTemp != null && sereServTemp.ID > 0)
                {
                    if (!String.IsNullOrEmpty(sereServTemp.EMR_COLUMN_MAPPING))
                    {
                        try
                        {
                            List<HIS.Desktop.Plugins.ServiceExecute.ADO.EmrColumnMappingADO> emrColumnMappingADOs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HIS.Desktop.Plugins.ServiceExecute.ADO.EmrColumnMappingADO>>(sereServTemp.EMR_COLUMN_MAPPING);
                            if (emrColumnMappingADOs != null && emrColumnMappingADOs.Count > 0)
                            {
                                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrColumnMappingADOs), emrColumnMappingADOs));
                                System.Reflection.PropertyInfo[] piInputADOs = typeof(Inventec.Common.SignLibrary.ADO.InputADO).GetProperties();
                                foreach (var emrColumn in emrColumnMappingADOs)
                                {
                                    if (dicParam.ContainsKey(emrColumn.Key))
                                    {
                                        object value = dicParam[emrColumn.Key];
                                        string valueDataType = "";
                                        var pi = piInputADOs.FirstOrDefault(t => t.Name == emrColumn.EmrColumn);
                                        if (pi != null && value != null)
                                        {
                                            try
                                            {
                                                if (pi.PropertyType.Equals(typeof(short)) || pi.PropertyType.Equals(typeof(short?)))
                                                {
                                                    pi.SetValue(emrInputADO, (short)(value));
                                                    valueDataType = "short";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(decimal)) || pi.PropertyType.Equals(typeof(decimal?)))
                                                {
                                                    pi.SetValue(emrInputADO, (decimal)(value));
                                                    valueDataType = "decimal";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(long)) || pi.PropertyType.Equals(typeof(long?)))
                                                {
                                                    DateTime? dt = null;
                                                    if (value.ToString().Contains("/") || value.ToString().Contains("-"))
                                                    {
                                                        string strDate = "", hours = "";
                                                        if (value.ToString().Contains(":"))
                                                        {
                                                            var arrDate = value.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                                            if (arrDate != null && arrDate.Count() > 1)
                                                            {
                                                                foreach (var itemD in arrDate)
                                                                {
                                                                    if (itemD.Contains(":"))
                                                                    {
                                                                        hours = itemD.Trim().Replace("h", ":");
                                                                        if (hours.Length == 5)
                                                                        {
                                                                            hours += ":00";
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var arrDate1 = itemD.Trim().Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                                        if (arrDate1 != null && arrDate1.Count() > 0)
                                                                        {
                                                                            strDate = String.Format("{0}-{1}-{2}", arrDate1[2], arrDate1[1], arrDate1[0]);
                                                                        }
                                                                    }
                                                                }
                                                                dt = DateTime.Parse(String.Format("{0} {1}", strDate, hours));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var arrDate1 = value.ToString().Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                            if (arrDate1 != null && arrDate1.Count() > 0)
                                                            {
                                                                strDate = String.Format("{0}-{1}-{2}", arrDate1[2], arrDate1[1], arrDate1[0]);
                                                                hours = "00:00:00";
                                                                dt = DateTime.Parse(String.Format("{0} {1}", strDate, hours));
                                                            }
                                                        }
                                                        pi.SetValue(emrInputADO, Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dt));
                                                    }
                                                    else
                                                        pi.SetValue(emrInputADO, (long)(value));

                                                    valueDataType = "long";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(int)) || pi.PropertyType.Equals(typeof(int?)))
                                                {
                                                    pi.SetValue(emrInputADO, (int)(value));
                                                    valueDataType = "int";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(double)) || pi.PropertyType.Equals(typeof(double?)))
                                                {
                                                    pi.SetValue(emrInputADO, (double)(value));
                                                    valueDataType = "double";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(float)) || pi.PropertyType.Equals(typeof(float?)))
                                                {
                                                    pi.SetValue(emrInputADO, (float)(value));
                                                    valueDataType = "float";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(bool)) || pi.PropertyType.Equals(typeof(bool?)))
                                                {
                                                    pi.SetValue(emrInputADO, (bool)(value));
                                                    valueDataType = "bool";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(DateTime)) || pi.PropertyType.Equals(typeof(DateTime?)))
                                                {
                                                    DateTime? dt = null;
                                                    if (value.ToString().Contains("/") || value.ToString().Contains("-"))
                                                    {
                                                        string strDate = "", hours = "";
                                                        if (value.ToString().Contains(":"))
                                                        {
                                                            var arrDate = value.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                                            if (arrDate != null && arrDate.Count() > 1)
                                                            {
                                                                foreach (var itemD in arrDate)
                                                                {
                                                                    if (itemD.Contains(":"))
                                                                    {
                                                                        hours = itemD.Trim().Replace("h", ":");
                                                                        if (hours.Length == 5)
                                                                        {
                                                                            hours += ":00";
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var arrDate1 = itemD.Trim().Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                                        if (arrDate1 != null && arrDate1.Count() > 0)
                                                                        {
                                                                            strDate = String.Format("{0}-{1}-{2}", arrDate1[2], arrDate1[1], arrDate1[0]);
                                                                        }
                                                                    }
                                                                }
                                                                dt = DateTime.Parse(String.Format("{0} {1}", strDate, hours));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var arrDate1 = value.ToString().Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                            if (arrDate1 != null && arrDate1.Count() > 0)
                                                            {
                                                                strDate = String.Format("{0}-{1}-{2}", arrDate1[2], arrDate1[1], arrDate1[0]);
                                                                hours = "00:00:00";
                                                                dt = DateTime.Parse(String.Format("{0} {1}", strDate, hours));
                                                            }
                                                        }
                                                    }
                                                    else if (value is long || value is long?)
                                                        dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)value);
                                                    else if (value is DateTime || value is DateTime?)
                                                        dt = (DateTime?)value;
                                                    pi.SetValue(emrInputADO, dt);
                                                    valueDataType = "DateTime";
                                                }
                                                else
                                                {
                                                    pi.SetValue(emrInputADO, value);
                                                    valueDataType = "string";
                                                }
                                                object newValue = pi.GetValue(emrInputADO);
                                            }
                                            catch (Exception ex1)
                                            {
                                                Inventec.Common.Logging.LogSystem.Warn(ex1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex2)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void PrintResult(bool printNow)
        {
            try
            {
                if (!btnPrint.Enabled || lciForbtnPrint.Visibility == LayoutVisibility.Never) return;

                ProcessDicParamForPrint();

                //1: In sử dụng biểu mẫu. 2: In trực tiếp dữ liệu do người dùng nhập ở màn hình xử lý"
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.OptionPrint) == "1")
                {
                    PrintOption1(printNow);
                }
                else
                {
                    PrintOption2(printNow);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PreviewResult(bool preview)
        {
            try
            {
                if (!btnPrint.Enabled || lciForbtnPrint.Visibility == LayoutVisibility.Never) return;

                ProcessDicParamForPrint();
                PrintOption3(preview);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private RichEditControl ProcessDocumentBeforePrint(RichEditControl document)
        {
            RichEditControl result = new RichEditControl();
            try
            {
                if (document != null)
                {
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.ID = currentServiceReq.ID;
                    var lstServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    long? finishTime = null;
                    if (lstServiceReq != null && lstServiceReq.Count > 0)
                    {
                        finishTime = lstServiceReq.FirstOrDefault().FINISH_TIME;
                    }

                    result.RtfText = document.RtfText;
                    if (!String.IsNullOrWhiteSpace(thoiGianKetThuc))
                    {
                        foreach (var section in result.Document.Sections)
                        {
                            if (HideTimePrint == "1" || HideTimePrint == "2")
                            {
                                section.Margins.HeaderOffset = 50;
                                section.Margins.FooterOffset = 50;
                                var myHeader = section.BeginUpdateHeader(DevExpress.XtraRichEdit.API.Native.HeaderFooterType.Odd);
                                //xóa header nếu có dữ liệu
                                myHeader.Delete(myHeader.Range);
                                if (HideTimePrint == "1")
                                {
                                    myHeader.InsertText(myHeader.CreatePosition(0),
                                        String.Format(ResourceMessage.NgayIn, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                                }
                                else if (HideTimePrint == "2")
                                {
                                    timeSync = new BackendAdapter(new CommonParam()).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, new CommonParam());
                                    myHeader.InsertText(myHeader.CreatePosition(0),
                                        String.Format(ResourceMessage.NgayIn, (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeSync.LocalTime) ?? DateTime.Now).ToString("dd/MM/yyyy HH:mm:ss")));
                                }
                                myHeader.Fields.Update();
                                section.EndUpdateHeader(myHeader);
                            }

                            string finishTimeStr = "";
                            if (finishTime.HasValue)
                            {
                                finishTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(finishTime.Value);
                            }

                            var rangeSeperators = result.Document.FindAll(thoiGianKetThuc, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                            if (rangeSeperators != null && rangeSeperators.Length > 0)
                            {
                                for (int i = 0; i < rangeSeperators.Length; i++)
                                    result.Document.Replace(rangeSeperators[i], finishTimeStr);
                            }
                        }
                    }

                    //key hiển thị màu trắng khi in sẽ thay key
                    if (dicSereServExt.ContainsKey(sereServ.ID))
                    {
                        List<string> keyPrint = new List<string>() {
                            "<#CONCLUDE_PRINT;>",
                            "<#NOTE_PRINT;>",
                            "<#DESCRIPTION_PRINT;>",
                            "<#CURRENT_USERNAME_PRINT;>",
                            "<#SUBCLINICAL_RESULT_LOGINNAME;>",
                            "<#SUBCLINICAL_RESULT_USERNAME;>",
                            "<#END_TIME_FULL_STR;>",
                            "<#BEGIN_TIME_FULL_STR;>"
                        };
                        //đổi về màu đen để hiển thị.
                        foreach (var key in keyPrint)
                        {
                            var rangeSeperators = result.Document.FindAll(key, SearchOptions.CaseSensitive);
                            foreach (var rang in rangeSeperators)
                            {
                                CharacterProperties cp = result.Document.BeginUpdateCharacters(rang);
                                cp.ForeColor = Color.Black;
                                result.Document.EndUpdateCharacters(cp);
                            }
                        }

                        result.Document.ReplaceAll("<#CONCLUDE_PRINT;>", dicSereServExt[sereServ.ID].CONCLUDE, SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#NOTE_PRINT;>", dicSereServExt[sereServ.ID].NOTE, SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#DESCRIPTION_PRINT;>", dicSereServExt[sereServ.ID].DESCRIPTION, SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#SUBCLINICAL_RESULT_LOGINNAME;>", dicSereServExt[sereServ.ID].SUBCLINICAL_RESULT_LOGINNAME, SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#SUBCLINICAL_RESULT_USERNAME;>", dicSereServExt[sereServ.ID].SUBCLINICAL_RESULT_USERNAME, SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#END_TIME_FULL_STR;>", MPS.ProcessorBase.GlobalQuery.GetCurrentTimeSeparateNoSecond(
                                Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dicSereServExt[sereServ.ID].END_TIME ?? 0) ?? DateTime.Now), SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#BEGIN_TIME_FULL_STR;>", MPS.ProcessorBase.GlobalQuery.GetCurrentTimeSeparateNoSecond(
                                Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dicSereServExt[sereServ.ID].BEGIN_TIME ?? 0) ?? DateTime.Now), SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#CURRENT_USERNAME_PRINT;>", UserName, SearchOptions.CaseSensitive);

                        if (dicParam != null)
                        {
                            foreach (var item in dicParam)
                            {
                                if (item.Value != null && CheckType(item.Value))
                                {
                                    string key = string.Format("<#{0}_PRINT;>", item.Key);
                                    var rangeSeperators = result.Document.FindAll(key, SearchOptions.CaseSensitive);
                                    foreach (var rang in rangeSeperators)
                                    {
                                        CharacterProperties cp = result.Document.BeginUpdateCharacters(rang);
                                        cp.ForeColor = Color.Black;
                                        result.Document.EndUpdateCharacters(cp);
                                    }

                                    result.Document.ReplaceAll(key, item.Value.ToString(), SearchOptions.CaseSensitive);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckType(object value)
        {
            bool result = false;
            try
            {
                result = value.GetType() == typeof(long) || value.GetType() == typeof(int) || value.GetType() == typeof(string) || value.GetType() == typeof(short) || value.GetType() == typeof(decimal) || value.GetType() == typeof(double) || value.GetType() == typeof(float);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void PrintOption3(bool previewOp)
        {
            try
            {
                var printDocument = ProcessDocumentBeforePrint(GettxtDescription());
                if (printDocument == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("printDocument is null");
                    return;
                }

                if (previewOp)
                {
                    printDocument.ShowPrintPreview();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintOption2(bool printNow)
        {
            try
            {
                var printDocument = ProcessDocumentBeforePrint(GettxtDescription());
                if (printDocument == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("printDocument is null");
                    return;
                }

                if (printNow)
                {
                    printDocument.Print();
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printDocument.Print();
                }
                else
                {
                    printDocument.ShowPrintPreview();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintOption1(bool printNow)
        {
            try
            {

                Dictionary<string, string> dicRtfText = new Dictionary<string, string>();

                dicRtfText["DESCRIPTION_WORD"] = GettxtDescription().RtfText;

                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == ServiceExecuteCFG.MPS000354);

                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.TreatmentWithPatientTypeAlter != null ? TreatmentWithPatientTypeAlter.TREATMENT_CODE : ""), ServiceExecuteCFG.MPS000354, moduleData != null ? moduleData.RoomId : 0);

                richEditorMain.RunPrintTemplate(printTemplate.PRINT_TYPE_CODE, printTemplate.FILE_PATTERN, sereServ.TDL_SERVICE_NAME, null, null, dicParam, dicImage, inputADO, dicRtfText, printNow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDicParamForPrint()
        {
            try
            {
                ProcessDicParam();

                if (this.sereServExt.MACHINE_ID.HasValue)
                {
                    var machine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(o => o.ID == this.sereServExt.MACHINE_ID.Value);
                    if (machine != null)
                    {
                        dicParam["MACHINE_NAME"] = machine.MACHINE_NAME;
                    }
                }

                //bổ sung các key nhóm cha của dv
                var service = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                if (service.PARENT_ID.HasValue)
                {
                    var serviceParent = lstService.FirstOrDefault(o => o.ID == service.PARENT_ID);
                    if (serviceParent != null)
                    {
                        this.dicParam["SERVICE_CODE_PARENT"] = serviceParent.SERVICE_CODE;
                        this.dicParam["SERVICE_NAME_PARENT"] = serviceParent.SERVICE_NAME;
                        this.dicParam["HEIN_SERVICE_BHYT_CODE_PARENT"] = serviceParent.HEIN_SERVICE_BHYT_CODE;
                        this.dicParam["HEIN_SERVICE_BHYT_NAME_PARENT"] = serviceParent.HEIN_SERVICE_BHYT_NAME;
                    }
                }

                dicParam["IS_COPY"] = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
