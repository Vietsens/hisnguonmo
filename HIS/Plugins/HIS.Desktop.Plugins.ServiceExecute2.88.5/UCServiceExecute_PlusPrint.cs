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
                        MOS.Filter.HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                        bedLogFilter.TREATMENT_BED_ROOM_IDs = treatmentBedRooms.Select(o => o.ID).Distinct().ToList();
                        var begLogs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogFilter, null);
                        hisBedLog = begLogs.OrderByDescending(o => o.START_TIME).FirstOrDefault();
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

                WaitingManager.Hide();
                MPS.Processor.Mps000033.PDO.Mps000033PDO rdo = new MPS.Processor.Mps000033.PDO.Mps000033PDO(currentPatient, departmentTran, ServiceReq, SereServ5, sereServExt, sereServPttts, treatmentView, vEkipUsers, null, null);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentView != null ? treatmentView.TREATMENT_CODE : "", printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void PrintResult(bool printNow)
        {
            try
            {
                if (!btnPrint.Enabled || lciForbtnPrint.Visibility == LayoutVisibility.Never) return;

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

        private RichEditControl ProcessDocumentBeforePrint(RichEditControl document)
        {
            RichEditControl result = new RichEditControl();
            try
            {
                if (document != null)
                {
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.ID = ServiceReqConstruct.ID;
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
                            if (HideTimePrint != "1")
                            {
                                section.Margins.HeaderOffset = 50;
                                section.Margins.FooterOffset = 50;
                                var myHeader = section.BeginUpdateHeader(DevExpress.XtraRichEdit.API.Native.HeaderFooterType.Odd);
                                //xóa header nếu có dữ liệu
                                myHeader.Delete(myHeader.Range);

                                myHeader.InsertText(myHeader.CreatePosition(0),
                                    String.Format(ResourceMessage.NgayIn, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
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
                ProcessDicParamForPrint();

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
                var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                if (service.PARENT_ID.HasValue)
                {
                    var serviceParent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == service.PARENT_ID);
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
