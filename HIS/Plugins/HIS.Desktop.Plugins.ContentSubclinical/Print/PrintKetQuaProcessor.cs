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
using DevExpress.XtraRichEdit;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ContentSubclinical.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ContentSubclinical.Print
{
    class PrintKetQuaProcessor
    {
        private bool printNow = false;
        public bool IsView = true;
        private List<HIS_SERVICE_REQ> listServiceReq;
        private List<HIS_SERE_SERV> listSereServ;
        private Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData;
        private Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData;
        private Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData;
        private long treatmentId = 0;
        public HIS_TREATMENT treament;
        private long? roomId;
        private bool IsGroup = false;
        HIS_SERVICE_REQ currentServiceReqPrint;
        HIS_SERE_SERV_EXT sereServExtPrint = null;
        Dictionary<string, object> dicParam;
        Dictionary<string, Image> dicImage;
        internal V_HIS_SERE_SERV_4 sereServ;
        private HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeAlter;
        List<string> keyPrint = new List<string>() { "<#CONCLUDE_PRINT;>", "<#NOTE_PRINT;>", "<#DESCRIPTION_PRINT;>", "<#CURRENT_USERNAME_PRINT;>" };

        private SAR.EFMODEL.DataModels.SAR_PRINT currentSarPrint;
        protected string currentBussinessCode;
        private long TotalSereServPrint { get; set; }
        private long CountSereServPrinted { get; set; }
        private long TotalServiceReqPrint;
        private long CountServiceReqPrinted = 0;
        private bool CancelPrint { get; set; }
        private const int TIME_OUT_PRINT_MERGE = 800;
        private List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> GroupStreamPrint;

        private MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType = null;

        public PrintKetQuaProcessor(List<HIS_SERVICE_REQ> _listServiceReq, List<HIS_SERE_SERV> _listSereServ, long _treatmentId, long _roomId)
        {
            try
            {
                this.listServiceReq = _listServiceReq;
                this.listSereServ = _listSereServ;
                this.treatmentId = _treatmentId;
                this.roomId = _roomId;
                if (_treatmentId > 0)
                {
                    this.treament = GetTreatmentById(_treatmentId);
                }
                Config.Config.LoadConfig();
                Config.Config.IsmergePrint = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Print()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Begin PrintServiceReq Print");
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("__listServiceReq", this.listServiceReq));
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("__listSereServ", this.listSereServ));
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("__treament", this.treament));
                if (Config.Config.IsmergePrint)
                {
                    Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint");
                    this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                    this.TotalSereServPrint = this.listSereServ != null ? this.listSereServ.Count : 0;
                    this.TotalServiceReqPrint = this.listServiceReq != null ? this.listServiceReq.Count : 0;
                }

                if (listServiceReq != null && listServiceReq.Count > 0)
                {

                    foreach (var item in listServiceReq)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));

                        if (item != null && item.IS_NO_EXECUTE != 1)
                        {
                            this.currentServiceReqPrint = new HIS_SERVICE_REQ();
                            this.currentServiceReqPrint = item;
                            WaitingManager.Show();

                            if (currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA ||
                                currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL ||
                                currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS ||
                                currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA ||
                                currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN)
                            {

                                MOS.Filter.HisSereServExtFilter hfilter = new MOS.Filter.HisSereServExtFilter();
                                hfilter.TDL_SERVICE_REQ_ID = currentServiceReqPrint.ID;
                                var sereServExt = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, hfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                                if (sereServExt != null && sereServExt.Count > 0)
                                {
                                    sereServExtPrint = sereServExt.FirstOrDefault();
                                    this.currentSarPrint = GetListPrintByDescriptionPrint(sereServExtPrint);
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentSarPrint), currentSarPrint));
                                    if (currentSarPrint != null && currentSarPrint.ID > 0)
                                    {

                                        LoadTreatmentWithPaty();
                                        ProcessDicParamForPrint();
                                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Utility.TextLibHelper.BytesToStringConverted(currentSarPrint.CONTENT)), Utility.TextLibHelper.BytesToStringConverted(currentSarPrint.CONTENT)));

                                        PrintDocumentKetQua(false, Utility.TextLibHelper.BytesToStringConverted(currentSarPrint.CONTENT), currentSarPrint);
                                    }
                                }

                            }
                            else if (currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                            {
                                Inventec.Common.Logging.LogSystem.Info("__IN__MPS000014__KQ_XET_NGHIEM:" + Inventec.Common.Logging.LogUtil.TraceData("currentServiceReqPrint: ", currentServiceReqPrint));
                                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                                richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);
                                WaitingManager.Hide();
                                richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000014__KQ_XET_NGHIEM, DelegateRunPrinter);
                                WaitingManager.Show();
                            }

                            WaitingManager.Hide();
                        }

                    }
                }

                //Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                //richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);
                //richEditorMain.RunPrintTemplate(PrintTypeCode, DelegateRunPrinter);

                if (Config.Config.IsmergePrint)
                {
                    Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint");
                    int countTimeOut = 0;
                    //in gộp xét nghiệp sẽ tăng số lượng dịch vụ
                    //while (this.TotalSereServPrint != this.CountSereServPrinted && countTimeOut < TIME_OUT_PRINT_MERGE && !CancelPrint)
                    //{
                    //    Thread.Sleep(50);
                    //    countTimeOut++;
                    //}
                    while (this.TotalServiceReqPrint != this.CountServiceReqPrinted && countTimeOut < TIME_OUT_PRINT_MERGE && !CancelPrint)
                    {
                        Thread.Sleep(50);
                        countTimeOut++;
                    }

                    if (countTimeOut > TIME_OUT_PRINT_MERGE)
                    {
                        throw new Exception("TimeOut");
                    }
                    if (CancelPrint)
                    {
                        throw new Exception("Cancel Print");
                    }

                    if (this.GroupStreamPrint != null && this.GroupStreamPrint.Count() > 0)
                    {
                        Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo adodata = this.GroupStreamPrint.First();
                        Inventec.Common.Logging.LogSystem.Debug("List MPS Group: " + string.Join("; ", this.GroupStreamPrint.Select(s => s.printTypeCode).Distinct()));
                        Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(adodata.saveMemoryStream, adodata.printerName, adodata.fileName, adodata.numCopy, true,
                            adodata.isAllowExport, adodata.TemplateKey, adodata.eventLog, adodata.eventPrint, adodata.EmrInputADO, adodata.PrintLog, adodata.ShowPrintLog, adodata.IsAllowEditTemplateFile, adodata.IsSingleCopy);
                        printProcess.SetPartialFile(this.GroupStreamPrint);
                        printProcess.PrintPreviewShow();
                        DisposePrint();
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy dữ liệu để in!", "Thông báo");
                        Inventec.Common.Logging.LogSystem.Error("Không tìm thấy dữ liệu để in!");
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("End PrintServiceReq Print");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintDocumentKetQua(bool printNow, string content, SAR.EFMODEL.DataModels.SAR_PRINT SarPrint)
        {
            try
            {
                if (string.IsNullOrEmpty(content)) return;
                var printDocument = ProcessDocumentBeforePrint(content);
                if (printDocument == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("printDocument is null");
                    return;
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = GenerateInputADO(SarPrint);

                using (MemoryStream pdfData = new MemoryStream())
                {
                    printDocument.ExportToPdf(pdfData);

                    Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo ado = null;
                    using (ado.saveMemoryStream = new System.IO.MemoryStream())
                    {
                        if (pdfData != null)
                        {
                            ado = new Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo();
                            pdfData.Position = 0;
                            pdfData.CopyTo(ado.saveMemoryStream);
                            ado.saveMemoryStream.Position = 0;
                            ado.IsPdfFile = true;
                            ado.fileName = SarPrint.TITLE;
                            ado.IsAllowEditTemplateFile = false;
                        }
                        SetDataGroup(1, ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Inventec.Common.SignLibrary.ADO.InputADO GenerateInputADO(SAR.EFMODEL.DataModels.SAR_PRINT SarPrint)
        {
            Inventec.Common.SignLibrary.ADO.InputADO resultInputADO = new Inventec.Common.SignLibrary.ADO.InputADO();
            try
            {
                Library.EmrGenerate.EmrGenerateProcessor generateProcessor = new Library.EmrGenerate.EmrGenerateProcessor();
                resultInputADO = generateProcessor.GenerateInputADOWithPrintTypeCode(this.treament.TREATMENT_CODE, "", true, this.roomId);

                resultInputADO.DocumentTypeCode = SarPrint.EMR_DOCUMENT_TYPE_CODE;
                resultInputADO.DocumentGroupCode = SarPrint.EMR_DOCUMENT_GROUP_CODE;

                if (!String.IsNullOrWhiteSpace(SarPrint.EMR_BUSINESS_CODES))
                {
                    var codes = SarPrint.EMR_BUSINESS_CODES.Split(';').ToList();
                    if (codes.Count() == 1)
                    {
                        resultInputADO.BusinessCode = codes[0];
                    }
                    else
                    {
                        var listBussiness = BackendDataWorker.Get<EMR.EFMODEL.DataModels.EMR_BUSINESS>().Where(o => codes.Contains(o.BUSINESS_CODE)).ToList();
                        MPS.ProcessorBase.EmrBusiness.frmChooseBusiness frmChooseBusiness = new MPS.ProcessorBase.EmrBusiness.frmChooseBusiness(ChooseBusinessClick, listBussiness);
                        frmChooseBusiness.ShowDialog();

                        resultInputADO.BusinessCode = currentBussinessCode;
                        SarPrint.EMR_BUSINESS_CODES = currentBussinessCode;
                        CreateThreadUpdateBusinessCode(SarPrint);
                    }
                }

                resultInputADO.DocumentName = (String.Format("{0} (Mã điều trị: {1})", SarPrint.TITLE, this.treament.TREATMENT_CODE));

                resultInputADO.HisCode = string.Format("SERVICE_REQ_CODE:{0} SER_SERV_ID:{1}", this.currentServiceReqPrint.SERVICE_REQ_CODE, this.sereServExtPrint.SERE_SERV_ID);
            }
            catch (Exception ex)
            {
                resultInputADO = new Inventec.Common.SignLibrary.ADO.InputADO();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return resultInputADO;
        }

        private void CreateThreadUpdateBusinessCode(SAR.EFMODEL.DataModels.SAR_PRINT data)
        {
            Thread update = new Thread(UpdateBusinessCode);
            try
            {
                update.Start(data);
            }
            catch (Exception ex)
            {
                update.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateBusinessCode(object obj)
        {
            try
            {
                if (obj != null && obj is SAR.EFMODEL.DataModels.SAR_PRINT)
                {
                    var data = (SAR.EFMODEL.DataModels.SAR_PRINT)obj;
                    CommonParam param = new CommonParam();
                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_UPDATE, ApiConsumer.ApiConsumers.SarConsumer, data, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChooseBusinessClick(EMR.EFMODEL.DataModels.EMR_BUSINESS dataBusiness)
        {
            try
            {
                if (dataBusiness != null)
                {
                    this.currentBussinessCode = dataBusiness != null ? dataBusiness.BUSINESS_CODE : "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private RichEditControl ProcessDocumentBeforePrint(string document)
        {
            RichEditControl result = null;
            try
            {
                if (document != null)
                {
                    result = new RichEditControl();
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.ID = sereServ.SERVICE_REQ_ID;
                    var lstServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    long? finishTime = null;
                    if (lstServiceReq != null && lstServiceReq.Count > 0)
                    {
                        finishTime = lstServiceReq.FirstOrDefault().FINISH_TIME;
                    }


                    result.RtfText = document;
                    if (string.IsNullOrEmpty(result.Text)) return null;
                    var tgkt = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.ThoiGianKetThuc");
                    string HideTimePrint = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.HideTimePrint");

                    if (!String.IsNullOrWhiteSpace(tgkt))
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
                                    String.Format(Inventec.Common.Resource.Get.Value("NgayIn",
                                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                                myHeader.Fields.Update();
                                section.EndUpdateHeader(myHeader);
                            }

                            string finishTimeStr = "";
                            if (finishTime.HasValue)
                            {
                                finishTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(finishTime.Value);
                            }

                            var rangeSeperators = result.Document.FindAll(tgkt, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                            if (rangeSeperators != null && rangeSeperators.Length > 0)
                            {
                                for (int i = 0; i < rangeSeperators.Length; i++)
                                    result.Document.Replace(rangeSeperators[i], finishTimeStr);
                            }
                        }
                    }

                    //key hiển thị màu trắng khi in sẽ thay key
                    if (sereServExtPrint != null)
                    {
                        //đổi về màu đen để hiển thị.
                        foreach (var key in keyPrint)
                        {
                            var rangeSeperators = result.Document.FindAll(key, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                            foreach (var rang in rangeSeperators)
                            {
                                DevExpress.XtraRichEdit.API.Native.CharacterProperties cp = result.Document.BeginUpdateCharacters(rang);
                                cp.ForeColor = Color.Black;
                                result.Document.EndUpdateCharacters(cp);
                            }
                        }

                        result.Document.ReplaceAll("<#CONCLUDE_PRINT;>", sereServExtPrint.CONCLUDE, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#NOTE_PRINT;>", sereServExtPrint.NOTE, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#DESCRIPTION_PRINT;>", sereServExtPrint.DESCRIPTION, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#CURRENT_USERNAME_PRINT;>", lstServiceReq.FirstOrDefault().EXECUTE_USERNAME, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);

                        foreach (var item in dicParam)
                        {
                            if (item.Value != null && CheckType(item.Value))
                            {
                                string key = string.Format("<#{0}_PRINT;>", item.Key);
                                var rangeSeperators = result.Document.FindAll(key, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                                foreach (var rang in rangeSeperators)
                                {
                                    DevExpress.XtraRichEdit.API.Native.CharacterProperties cp = result.Document.BeginUpdateCharacters(rang);
                                    cp.ForeColor = Color.Black;
                                    result.Document.EndUpdateCharacters(cp);
                                }
                                result.Document.ReplaceAll(key, item.Value.ToString(), DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
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

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                //if (ProcessDataBeforePrint())
                //{
                EmrDataStore.treatmentCode = (this.treament != null ? this.treament.TREATMENT_CODE : "");

                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.IN__MPS000014__KQ_XET_NGHIEM:
                        new InKetQuaXetNghiem(printTypeCode, fileName, this.currentServiceReqPrint, printNow, ref result, roomId, IsView, this.PreviewType, SetDataGroup, CancelChooseTemplate);
                        break;
                    default:
                        break;
                }
                //}
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void LoadTreatmentWithPaty()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("1. Begin LoadTreatmentWithPaty");
                CommonParam param = new CommonParam();

                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = currentServiceReqPrint.TREATMENT_ID;
                filter.INTRUCTION_TIME = currentServiceReqPrint.INTRUCTION_TIME;
                var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    TreatmentWithPatientTypeAlter = apiResult.FirstOrDefault();
                }
                Inventec.Common.Logging.LogSystem.Info("1. End LoadTreatmentWithPaty");
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

                //bổ sung các key nhóm cha của dv
                var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                if (service.PARENT_ID.HasValue)
                {
                    var serviceParent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == service.PARENT_ID);
                    if (serviceParent != null)
                    {
                        this.dicParam.Add("SERVICE_CODE_PARENT", serviceParent.SERVICE_CODE);
                        this.dicParam.Add("SERVICE_NAME_PARENT", serviceParent.SERVICE_NAME);
                        this.dicParam.Add("HEIN_SERVICE_BHYT_CODE_PARENT", serviceParent.HEIN_SERVICE_BHYT_CODE);
                        this.dicParam.Add("HEIN_SERVICE_BHYT_NAME_PARENT", serviceParent.HEIN_SERVICE_BHYT_NAME);
                    }
                }

                dicParam["IS_COPY"] = "BẢN SAO";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDicParam()
        {
            try
            {
                // chế biến dữ liệu thành các key đơn thêm vào biểu mẫu tương tự như mps excel
                this.dicParam = new Dictionary<string, object>();
                this.dicImage = new Dictionary<string, Image>();

                HIS.Desktop.Print.SetCommonKey.SetCommonSingleKey(this.dicParam);//commonkey
                if (currentServiceReqPrint != null)
                {
                    dicParam.Add("INTRUCTION_TIME_FULL_STR",
                            GetCurrentTimeSeparateBeginTime(
                            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                            currentServiceReqPrint.INTRUCTION_TIME) ?? DateTime.Now));

                    dicParam.Add("INTRUCTION_DATE_FULL_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        currentServiceReqPrint.INTRUCTION_TIME));

                    dicParam.Add("INTRUCTION_TIME_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(
                            currentServiceReqPrint.INTRUCTION_TIME));

                    dicParam.Add("START_TIME_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(
                            currentServiceReqPrint.START_TIME ?? 0));

                    dicParam.Add("START_TIME_FULL_STR",
                            GetCurrentTimeSeparateBeginTime(
                            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                            currentServiceReqPrint.START_TIME ?? 0) ?? DateTime.Now));

                    dicParam.Add("ICD_MAIN_TEXT", currentServiceReqPrint.ICD_NAME);

                    dicParam.Add("NATIONAL_NAME", currentServiceReqPrint.TDL_PATIENT_NATIONAL_NAME);
                    dicParam.Add("WORK_PLACE", currentServiceReqPrint.TDL_PATIENT_WORK_PLACE_NAME);
                    dicParam.Add("ADDRESS", currentServiceReqPrint.TDL_PATIENT_ADDRESS);
                    dicParam.Add("CAREER_NAME", currentServiceReqPrint.TDL_PATIENT_CAREER_NAME);
                    dicParam.Add("PATIENT_CODE", currentServiceReqPrint.TDL_PATIENT_CODE);
                    dicParam.Add("DISTRICT_CODE", currentServiceReqPrint.TDL_PATIENT_DISTRICT_CODE);
                    dicParam.Add("GENDER_NAME", currentServiceReqPrint.TDL_PATIENT_GENDER_NAME);
                    dicParam.Add("MILITARY_RANK_NAME", currentServiceReqPrint.TDL_PATIENT_MILITARY_RANK_NAME);
                    dicParam.Add("VIR_ADDRESS", currentServiceReqPrint.TDL_PATIENT_ADDRESS);
                    dicParam.Add("AGE", CalculatorAge(currentServiceReqPrint.TDL_PATIENT_DOB, false));
                    dicParam.Add("STR_YEAR", currentServiceReqPrint.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    dicParam.Add("VIR_PATIENT_NAME", currentServiceReqPrint.TDL_PATIENT_NAME);

                    var executeRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentServiceReqPrint.EXECUTE_ROOM_ID);
                    if (executeRoom != null)
                    {
                        dicParam.Add("EXECUTE_DEPARTMENT_CODE", executeRoom.DEPARTMENT_CODE);
                        dicParam.Add("EXECUTE_DEPARTMENT_NAME", executeRoom.DEPARTMENT_NAME);
                        dicParam.Add("EXECUTE_ROOM_CODE", executeRoom.ROOM_CODE);
                        dicParam.Add("EXECUTE_ROOM_NAME", executeRoom.ROOM_NAME);
                    }

                    var reqRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentServiceReqPrint.REQUEST_ROOM_ID);
                    if (reqRoom != null)
                    {
                        dicParam.Add("REQUEST_DEPARTMENT_CODE", reqRoom.DEPARTMENT_CODE);
                        dicParam.Add("REQUEST_DEPARTMENT_NAME", reqRoom.DEPARTMENT_NAME);
                        dicParam.Add("REQUEST_ROOM_CODE", reqRoom.ROOM_CODE);
                        dicParam.Add("REQUEST_ROOM_NAME", reqRoom.ROOM_NAME);
                    }
                }

                if (TreatmentWithPatientTypeAlter != null)
                {
                    if (!String.IsNullOrEmpty(TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        dicParam.Add("HEIN_CARD_NUMBER_SEPARATE",
                            HeinCardHelper.SetHeinCardNumberDisplayByNumber(TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER));
                        dicParam.Add("STR_HEIN_CARD_FROM_TIME",
                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(
                            TreatmentWithPatientTypeAlter.HEIN_CARD_FROM_TIME));
                        dicParam.Add("STR_HEIN_CARD_TO_TIME",
                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(
                            TreatmentWithPatientTypeAlter.HEIN_CARD_TO_TIME));
                        dicParam.Add("HEIN_CARD_ADDRESS", TreatmentWithPatientTypeAlter.HEIN_CARD_ADDRESS);
                    }
                    else
                    {
                        dicParam.Add("HEIN_CARD_NUMBER_SEPARATE", "");
                        dicParam.Add("STR_HEIN_CARD_FROM_TIME", "");
                        dicParam.Add("STR_HEIN_CARD_TO_TIME", "");
                        dicParam.Add("HEIN_CARD_ADDRESS", "");
                    }

                    var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == TreatmentWithPatientTypeAlter.PATIENT_TYPE_CODE);
                    if (patientType != null)
                        dicParam.Add("PATIENT_TYPE_NAME", patientType.PATIENT_TYPE_NAME);
                    else
                        dicParam.Add("PATIENT_TYPE_NAME", "");

                    var treatmentType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == TreatmentWithPatientTypeAlter.TREATMENT_TYPE_CODE);
                    if (treatmentType != null)
                        dicParam.Add("TREATMENT_TYPE_NAME", treatmentType.TREATMENT_TYPE_NAME);
                    else
                        dicParam.Add("TREATMENT_TYPE_NAME", "");

                    dicParam.Add("TREATMENT_ICD_CODE", TreatmentWithPatientTypeAlter.ICD_CODE);
                    dicParam.Add("TREATMENT_ICD_NAME", TreatmentWithPatientTypeAlter.ICD_NAME);
                    dicParam.Add("TREATMENT_ICD_SUB_CODE", TreatmentWithPatientTypeAlter.ICD_SUB_CODE);
                    dicParam.Add("TREATMENT_ICD_TEXT", TreatmentWithPatientTypeAlter.ICD_TEXT);

                    AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(TreatmentWithPatientTypeAlter, this.dicParam, false);

                    int AGE_NUM = Inventec.Common.DateTime.Calculation.Age(TreatmentWithPatientTypeAlter.TDL_PATIENT_DOB, TreatmentWithPatientTypeAlter.IN_TIME);
                    dicParam.Add("AGE_NUM", AGE_NUM);
                }
                else
                {
                    dicParam.Add("HEIN_CARD_NUMBER_SEPARATE", "");
                    dicParam.Add("STR_HEIN_CARD_FROM_TIME", "");
                    dicParam.Add("STR_HEIN_CARD_TO_TIME", "");
                    dicParam.Add("HEIN_CARD_ADDRESS", "");
                    var typeAlter = new HisTreatmentWithPatientTypeInfoSDO();
                    AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(typeAlter, this.dicParam, false);
                }

                //if (patient != null)
                //    AddKeyIntoDictionaryPrint<ADO.PatientADO>(patient, this.dicParam, false);
                CommonParam paramCommon = new CommonParam();



                MOS.Filter.HisSereServView4Filter filter = new MOS.Filter.HisSereServView4Filter();
                filter.ID = sereServExtPrint.SERE_SERV_ID;
                var rs = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<V_HIS_SERE_SERV_4>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GETVIEW_4, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (rs != null && rs.Count > 0)
                {
                    sereServ = rs[0];

                }

                HIS_SERE_SERV sereS = new HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereS, this.sereServ);
                AddKeyIntoDictionaryPrint<HIS_SERVICE_REQ>(this.currentServiceReqPrint, this.dicParam, true);
                AddKeyIntoDictionaryPrint<HIS_SERE_SERV>(sereS, this.dicParam, false);
                AddKeyIntoDictionaryPrint<HIS_SERE_SERV_EXT>(this.sereServExtPrint, this.dicParam, true);

                if (this.sereServExtPrint != null)
                {
                    if (!dicParam.ContainsKey("END_TIME_FULL_STR"))
                        dicParam.Add("END_TIME_FULL_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                                this.sereServExtPrint.END_TIME ?? 0));
                    else
                        dicParam["END_TIME_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                                this.sereServExtPrint.END_TIME ?? 0);
                    if (!dicParam.ContainsKey("BEGIN_TIME_FULL_STR"))
                        dicParam.Add("BEGIN_TIME_FULL_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                                this.sereServExtPrint.BEGIN_TIME ?? 0));
                    else
                        dicParam["BEGIN_TIME_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                                this.sereServExtPrint.BEGIN_TIME ?? 0);

                    if (this.sereServExtPrint.MACHINE_ID.HasValue)
                    {
                        var machine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(o => o.ID == this.sereServExtPrint.MACHINE_ID.Value);
                        if (machine != null)
                        {
                            dicParam["MACHINE_NAME"] = machine.MACHINE_NAME;
                        }
                    }

                    if (sereServExtPrint.END_TIME.HasValue)
                    {
                        dicParam["EXECUTE_DATE_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(sereServExtPrint.END_TIME.Value);
                        dicParam["EXECUTE_TIME_FULL_STR"] = GetCurrentTimeSeparateBeginTime(sereServExtPrint.END_TIME.Value);
                    }
                    else if (sereServExtPrint.MODIFY_TIME.HasValue)
                    {
                        dicParam["EXECUTE_DATE_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(sereServExtPrint.MODIFY_TIME.Value);
                        dicParam["EXECUTE_TIME_FULL_STR"] = GetCurrentTimeSeparateBeginTime(sereServExtPrint.MODIFY_TIME.Value);
                    }
                    else
                    {
                        dicParam["EXECUTE_DATE_FULL_STR"] = "";
                        dicParam["EXECUTE_TIME_FULL_STR"] = "";
                    }
                }
                else
                {
                    dicParam["EXECUTE_DATE_FULL_STR"] = "";
                    dicParam["EXECUTE_TIME_FULL_STR"] = "";
                    dicParam["MACHINE_NAME"] = "";
                }

                dicParam.Add("USER_NAME", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());

                //bỏ key để phục vụ đổ dữ liệu khi in
                foreach (var item in keyPrint)
                {
                    dicParam.Remove(item);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> dicParamPlus, bool autoOveride)
        {
            try
            {
                if (data != null)
                {
                    PropertyInfo[] pis = typeof(T).GetProperties();
                    if (pis != null && pis.Length > 0)
                    {
                        foreach (var pi in pis)
                        {
                            if (pi.GetGetMethod().IsVirtual) continue;

                            var searchKey = dicParamPlus.SingleOrDefault(o => o.Key == pi.Name);
                            if (String.IsNullOrEmpty(searchKey.Key))
                            {
                                dicParamPlus.Add(pi.Name, pi.GetValue(data));
                            }
                            else
                            {
                                if (autoOveride)
                                    dicParamPlus[pi.Name] = pi.GetValue(data);
                                else if (dicParamPlus[pi.Name] == null)
                                    dicParamPlus[pi.Name] = pi.GetValue(data);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private SAR.EFMODEL.DataModels.SAR_PRINT GetListPrintByDescriptionPrint(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt)
        {
            SAR.EFMODEL.DataModels.SAR_PRINT result = null;
            try
            {
                List<long> printIds = GetListPrintIdBySereServ(sereServExt);
                Inventec.Common.Logging.LogSystem.Debug("printIds________" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printIds), printIds));
                if (printIds != null && printIds.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    SAR.Filter.SarPrintFilter filter = new SAR.Filter.SarPrintFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.IDs = printIds;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>(ApiConsumer.SarRequestUriStore.SAR_PRINT_GET, ApiConsumer.ApiConsumers.SarConsumer, filter, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        result = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<long> GetListPrintIdBySereServ(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT item)
        {
            List<long> result = new List<long>();
            try
            {
                if (!String.IsNullOrEmpty(item.DESCRIPTION_SAR_PRINT_ID))
                {
                    var arrIds = item.DESCRIPTION_SAR_PRINT_ID.Split(',', ';');
                    if (arrIds != null && arrIds.Length > 0)
                    {
                        foreach (var id in arrIds)
                        {
                            long printId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                            if (printId > 0)
                            {
                                result.Add(printId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void CancelChooseTemplate(string printTypeCode)
        {
            try
            {
                this.CancelPrint = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessDataBeforePrint()
        {
            bool result = false;
            try
            {
                CreateThreadLoadDataForService();
                if (dicServiceReqData != null && dicServiceReqData.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void CreateThreadLoadDataForService()
        {
            Thread threadTreatment = new Thread(LoadThreadDataTreatment);
            Thread threadSereServ = new Thread(LoadThreadDataSereServ);
            threadTreatment.Priority = ThreadPriority.Normal;
            threadSereServ.Priority = ThreadPriority.Normal;
            try
            {
                threadTreatment.Start();
                threadSereServ.Start();

                threadTreatment.Join();
                threadSereServ.Join();
            }
            catch (Exception ex)
            {
                threadTreatment.Abort();
                threadSereServ.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataTreatment()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataSereServ()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataGroup(int count, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo data)
        {
            try
            {
                //this.CountSereServPrinted += count;
                this.CountServiceReqPrinted += count;
                if (data != null)
                {
                    if (this.GroupStreamPrint == null)
                    {
                        this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                    }

                    this.GroupStreamPrint.Add(data);
                    data = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static string GetCurrentTimeSeparateBeginTime(System.DateTime now)
        {
            string result = "";
            try
            {
                if (now != DateTime.MinValue)
                {
                    string month = string.Format("{0:00}", now.Month);
                    string day = string.Format("{0:00}", now.Day);
                    string hour = string.Format("{0:00}", now.Hour);
                    string hours = string.Format("{0:00}", now.Hour);
                    string minute = string.Format("{0:00}", now.Minute);
                    string strNgay = "ngày";
                    string strThang = "tháng";
                    string strNam = "năm";
                    result = string.Format("{0}" + ":" + "{1} " + strNgay + " {2} " + strThang + " {3} " + strNam + " {4}", hours, minute, now.Day, now.Month, now.Year);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        internal static string GetCurrentTimeSeparateBeginTime(long time)
        {
            string result = "";
            try
            {
                if (time > 0)
                {
                    string temp = time.ToString();
                    string year = string.Format("{0:00}", temp.Substring(0, 4));
                    string month = string.Format("{0:00}", temp.Substring(4, 2));
                    string day = string.Format("{0:00}", temp.Substring(6, 2));
                    string hours = string.Format("{0:00}", temp.Substring(8, 2));
                    string minute = string.Format("{0:00}", temp.Substring(10, 2));
                    result = string.Format("{0} giờ {1} phút ngày {2} tháng {3} năm {4}", hours, minute, day, month, year);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private string CalculatorAge(long ageYearNumber, bool isHl7)
        {
            string tuoi = "";
            try
            {
                string caption__Tuoi = "Tuổi";
                string caption__ThangTuoi = "Tháng tuổi";
                string caption__NgayTuoi = "Ngày tuổi";
                string caption__GioTuoi = "Giờ tuổi";

                if (isHl7)
                {
                    caption__Tuoi = "T";
                    caption__ThangTuoi = "TH";
                    caption__NgayTuoi = "NT";
                    caption__GioTuoi = "GT";
                }

                if (ageYearNumber > 0)
                {
                    System.DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ageYearNumber).Value;
                    if (dtNgSinh == System.DateTime.MinValue) throw new ArgumentNullException("dtNgSinh");

                    TimeSpan diff__hour = (System.DateTime.Now - dtNgSinh);
                    TimeSpan diff__month = (System.DateTime.Now.Date - dtNgSinh.Date);

                    //- Dưới 24h: tính chính xác đến giờ.
                    double hour = diff__hour.TotalHours;
                    if (hour < 24)
                    {
                        tuoi = ((int)hour + " " + caption__GioTuoi);
                    }
                    else
                    {
                        long tongsogiay__hour = diff__hour.Ticks;
                        System.DateTime newDate__hour = new System.DateTime(tongsogiay__hour);
                        int month__hour = ((newDate__hour.Year - 1) * 12 + newDate__hour.Month - 1);
                        if (month__hour == 0)
                        {
                            //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                            tuoi = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                        }
                        else
                        {
                            long tongsogiay = diff__month.Ticks;
                            System.DateTime newDate = new System.DateTime(tongsogiay);
                            int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);
                            if (month == 0)
                            {
                                //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                                tuoi = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                            }
                            else
                            {
                                //- Dưới 72 tháng tuổi: tính chính xác đến tháng như hiện tại
                                if (month < 72)
                                {
                                    tuoi = (month + " " + caption__ThangTuoi);
                                }
                                //- Trên 72 tháng tuổi: tính chính xác đến năm: tuổi= năm hiện tại - năm sinh
                                else
                                {
                                    int year = System.DateTime.Now.Year - dtNgSinh.Year;
                                    tuoi = (year + " " + caption__Tuoi);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                tuoi = "";
            }
            return tuoi;
        }

        private HIS_TREATMENT GetTreatmentById(long _treatmentId)
        {
            HIS_TREATMENT result = null;
            try
            {
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                treatmentFilter.ID = _treatmentId;
                result = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void DisposePrint()
        {
            try
            {
                GroupStreamPrint = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
