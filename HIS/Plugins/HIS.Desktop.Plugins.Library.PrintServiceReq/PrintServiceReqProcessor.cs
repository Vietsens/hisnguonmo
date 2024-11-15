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
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.Library.PrintServiceReq.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintServiceReq
{
    public class PrintServiceReqProcessor
    {
        private bool printNow;
        public bool IsView;
        private Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData;
        private Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData;
        private Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData;
        private HisServiceReqListResultSDO HisServiceReqListResultSDO { get; set; }
        private HisTreatmentWithPatientTypeInfoSDO HisTreatmentWithPatientTypeInfoSDO { get; set; }
        private List<V_HIS_BED_LOG> BedLogs { get; set; }
        private ChiDinhDichVuADO chiDinhDichVuADO = new ChiDinhDichVuADO();
        private long? roomId;
        private bool IsGroup = false;

        private bool IsProcessDataPrint;
        private string gate;
        private long TotalSereServPrint { get; set; }
        private long CountSereServPrinted { get; set; }
        private bool CancelPrint { get; set; }
        private const int TIME_OUT_PRINT_MERGE = 1200;
        private List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> GroupStreamPrint;
        private List<HIS_CONFIG> lstConfig { get; set; }
        private List<HIS_TRANS_REQ> transReq { get; set; }
        /// <summary>
        /// true: lưu in. Tất cả  dữ liệu truyền từ ngoài vào.
        /// false: in từng phiếu. phiếu xét nghiệm gộp sẽ lấy thêm dữ liệu
        /// </summary>
        private bool IsMethodSaveNPrint = false;
        private MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType = null;
        private List<HisServiceReqMaxNumOrderSDO> ReqMaxNumOrderSDO;
        private Action<Inventec.Common.SignLibrary.DTO.DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned { get; set; }
        public PrintServiceReqProcessor(HisServiceReqListResultSDO _ServiceReqResult,
           HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeInfo, List<V_HIS_BED_LOG> _bedLogs)
            : this(_ServiceReqResult, TreatmentWithPatientTypeInfo, _bedLogs, null)
        {
        }

        public PrintServiceReqProcessor(HisServiceReqListResultSDO _ServiceReqResult,
            HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeInfo, List<V_HIS_BED_LOG> _bedLogs, long? roomId)
        {
            try
            {
                this.HisServiceReqListResultSDO = _ServiceReqResult;
                this.HisTreatmentWithPatientTypeInfoSDO = TreatmentWithPatientTypeInfo;
                this.BedLogs = _bedLogs;
                this.roomId = roomId;
                Config.LoadConfig();
                ProcessDictionaryData.Reload();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public PrintServiceReqProcessor(HisServiceReqListResultSDO _ServiceReqResult,
    HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeInfo, List<V_HIS_BED_LOG> _bedLogs, long? roomId,string gate)
        {
            try
            {
                this.HisServiceReqListResultSDO = _ServiceReqResult;
                this.HisTreatmentWithPatientTypeInfoSDO = TreatmentWithPatientTypeInfo;
                this.BedLogs = _bedLogs;
                this.roomId = roomId;
                this.gate = gate;
                Config.LoadConfig();
                ProcessDictionaryData.Reload();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public PrintServiceReqProcessor(HisServiceReqListResultSDO _ServiceReqResult,
HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeInfo, List<V_HIS_BED_LOG> _bedLogs, long? roomId, string gate, MPS.ProcessorBase.PrintConfig.PreviewType _PreviewType)
        {
            try
            {
                this.HisServiceReqListResultSDO = _ServiceReqResult;
                this.HisTreatmentWithPatientTypeInfoSDO = TreatmentWithPatientTypeInfo;
                this.BedLogs = _bedLogs;
                this.roomId = roomId;
                this.gate = gate;
                Config.LoadConfig();
                this.PreviewType = _PreviewType;
                ProcessDictionaryData.Reload();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public PrintServiceReqProcessor(HisServiceReqListResultSDO _ServiceReqResult,
            HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeInfo, List<V_HIS_BED_LOG> _bedLogs, long? roomId, MPS.ProcessorBase.PrintConfig.PreviewType _PreviewType)
        {
            try
            {
                this.HisServiceReqListResultSDO = _ServiceReqResult;
                this.HisTreatmentWithPatientTypeInfoSDO = TreatmentWithPatientTypeInfo;
                this.BedLogs = _bedLogs;
                this.roomId = roomId;
                Config.LoadConfig();
                this.PreviewType = _PreviewType;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public PrintServiceReqProcessor(HisServiceReqListResultSDO _ServiceReqResult,
            HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeInfo, List<V_HIS_BED_LOG> _bedLogs, long? roomId, MPS.ProcessorBase.PrintConfig.PreviewType _PreviewType, Action<Inventec.Common.SignLibrary.DTO.DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned)
        {
            try
            {
                this.HisServiceReqListResultSDO = _ServiceReqResult;
                this.HisTreatmentWithPatientTypeInfoSDO = TreatmentWithPatientTypeInfo;
                this.BedLogs = _bedLogs;
                this.roomId = roomId;
                Config.LoadConfig();
                this.PreviewType = _PreviewType;
                this.DlgSendResultSigned = DlgSendResultSigned;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataPrintQrCode()
        {
            try
            {

                lstConfig = new List<HIS_CONFIG>();
                 var currentWorkingRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                if (currentWorkingRoom != null && !string.IsNullOrEmpty(currentWorkingRoom.QR_CONFIG_JSON))
                {
                    List<object> listArgs = new List<object>();
                    try
                    {
                        var json = Newtonsoft.Json.JsonConvert.DeserializeObject<BankInfo>(currentWorkingRoom.QR_CONFIG_JSON);
                        if (json != null)
                        {
                            lstConfig.Add(new HIS_CONFIG() { VALUE = json.VALUE, KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", json.BANK) });
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
                else
                {
                    lstConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();
                }
                if (Config.HisTranReqQRCodeTreatmentPrint)
                {
                    if (lstConfig != null && lstConfig.Count > 0 && HisServiceReqListResultSDO != null && HisServiceReqListResultSDO.ServiceReqs != null && HisServiceReqListResultSDO.ServiceReqs.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        HisTransReqFilter filter = new HisTransReqFilter();
                        filter.IDs = HisServiceReqListResultSDO.ServiceReqs.Select(o => o.TRANS_REQ_ID ?? 0).ToList();
                        transReq = new Inventec.Common.Adapter.BackendAdapter(param)
                          .Get<List<MOS.EFMODEL.DataModels.HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    }
                }
                else
                {
                    if (lstConfig != null && lstConfig.Count > 0 && HisServiceReqListResultSDO != null && HisServiceReqListResultSDO.ServiceReqs != null && HisServiceReqListResultSDO.ServiceReqs.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        HisTransReqFilter filter = new HisTransReqFilter();
                        filter.TREATMENT_ID = HisServiceReqListResultSDO.ServiceReqs[0].TREATMENT_ID;
                        transReq = new Inventec.Common.Adapter.BackendAdapter(param)
                          .Get<List<MOS.EFMODEL.DataModels.HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        if(transReq != null && transReq.Count > 0)
                            transReq = transReq.Where(o => o.TRANS_REQ_TYPE == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_SERVICE).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void UpdatePreviewType(MPS.ProcessorBase.PrintConfig.PreviewType _PreviewType)
        {
            this.PreviewType = _PreviewType;
        }

        /// <summary>
        /// Sử dụng cấu hình để in ngay ("HIS.Desktop.Plugins.Library.PrintServiceReq.Mps")
        /// </summary>
        public void Print()
        {
            try
            {

                Print(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.mps));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// lưu in chỉ định
        /// </summary>
        /// <param name="PrintMany"></param>
        public void SaveNPrint()
        {
            try
            {
                SaveNPrint(false);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SavePrintTreatmentList()
        {
            try
            {
                try
                {
                    WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Info("Begin PrintServiceReq SaveNPrint");

                    this.IsProcessDataPrint = false;

                    string IsViewBefore = ConfigApplicationWorker.Get<string>("CONFIG_KEY__CHE_DO_IN_CHO_CAC_CHUC_NANG_TRONG_PHAN_MEM");
                    this.IsView = false;

                    this.printNow = IsViewBefore == "2" ? true : false;

                    IsGroup = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.mpsGroup) == "1";
                    IsMethodSaveNPrint = true;

                    Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint 1");
                    this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                    this.TotalSereServPrint = HisServiceReqListResultSDO != null && HisServiceReqListResultSDO.SereServs != null ? HisServiceReqListResultSDO.SereServs.Count : 0;
                    if (this.printNow && !this.IsView)
                    {
                        Inventec.Common.Logging.LogSystem.Info("PrintServiceReq SavePrintTreatmentList printNow");
                        PrinNowInCacPhieuChiDinh();
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info("PrintServiceReq SavePrintTreatmentList PreView");
                        Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                        richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);
                        if (IsGroup)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000340__GOP_CHI_DINH, DelegateRunPrinter);
                        }
                        else InCacPhieuChiDinhProcess(richEditorMain);

                        if (Config.IsmergePrint)
                        {
                            Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint 2");
                            int countTimeOut = 0;
                            //in gộp xét nghiệp sẽ tăng số lượng dịch vụ
                            while (this.TotalSereServPrint != this.CountSereServPrinted && countTimeOut < TIME_OUT_PRINT_MERGE && !CancelPrint)
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

                            Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo adodata = this.GroupStreamPrint.First();

                            Inventec.Common.Logging.LogSystem.Debug("List MPS Group: " + string.Join("; ", this.GroupStreamPrint.Select(s => s.printTypeCode).Distinct()));
                            Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(adodata.saveMemoryStream, adodata.printerName, adodata.fileName, adodata.numCopy, true,
                                adodata.isAllowExport, adodata.TemplateKey, adodata.eventLog, adodata.eventPrint, adodata.EmrInputADO, adodata.PrintLog, adodata.ShowPrintLog, adodata.IsAllowEditTemplateFile, adodata.IsSingleCopy);
                            printProcess.SetPartialFile(this.GroupStreamPrint);
                            printProcess.PrintPreviewShow();
                        }
                    }

                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Info("End PrintServiceReq SaveNPrint");
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// lưu in chỉ định
        /// </summary>
        /// <param name="IsView">có xem không</param>
        public void SaveNPrint(bool isView)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Info("Begin PrintServiceReq SaveNPrint");

                this.IsProcessDataPrint = false;

                int IsViewBefore = ConfigApplicationWorker.Get<int>(AppConfig.CONFIG_KEY_HIS_DESKTOP_ASSIGN_SERVICE_PRINT_NOW);

                this.IsView = isView;

                this.printNow = IsViewBefore == 1 ? false : true;

                IsGroup = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.mpsGroup) == "1";
                IsMethodSaveNPrint = true;

                Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint 1");
                this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                this.TotalSereServPrint = HisServiceReqListResultSDO != null && HisServiceReqListResultSDO.SereServs != null ? HisServiceReqListResultSDO.SereServs.Count : 0;
                if (this.printNow && !this.IsView)
                {
                    Inventec.Common.Logging.LogSystem.Info("PrintServiceReq SaveNPrint printNow");
                    CreateThreadInCacPhieuChiDinh();
                }
                else
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                    richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);
                    if (IsGroup)
                    {
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000340__GOP_CHI_DINH, DelegateRunPrinter);
                    }
                    else InCacPhieuChiDinhProcess(richEditorMain);

                    if (Config.IsmergePrint)
                    {
                        Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint 2");
                        int countTimeOut = 0;
                        //in gộp xét nghiệp sẽ tăng số lượng dịch vụ
                        while (this.TotalSereServPrint != this.CountSereServPrinted && countTimeOut < TIME_OUT_PRINT_MERGE && !CancelPrint)
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

                        Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo adodata = this.GroupStreamPrint.First();

                        Inventec.Common.Logging.LogSystem.Debug("List MPS Group: " + string.Join("; ", this.GroupStreamPrint.Select(s => s.printTypeCode).Distinct()));
                        Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(adodata.saveMemoryStream, adodata.printerName, adodata.fileName, adodata.numCopy, true,
                            adodata.isAllowExport, adodata.TemplateKey, adodata.eventLog, adodata.eventPrint, adodata.EmrInputADO, adodata.PrintLog, adodata.ShowPrintLog, adodata.IsAllowEditTemplateFile, adodata.IsSingleCopy);
                        printProcess.SetPartialFile(this.GroupStreamPrint);
                        printProcess.PrintPreviewShow();
                    }
                }

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Info("End PrintServiceReq SaveNPrint");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        /// <summary>
        /// Mở form ký ERM
        /// </summary>
        public void ERMOpen()
        {
            try
            {
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Info("Begin PrintServiceReq ERMOpen");
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);
                InCacPhieuChiDinhProcess(richEditorMain);
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Info("End PrintServiceReq ERMOpen");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// in ngay
        /// </summary>
        /// <param name="PrintTypeCode">mã in</param>
        public void Print(string PrintTypeCode)
        {
            try
            {
                Print(PrintTypeCode, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PrintTypeCode">Mã in (8,10,11)</param>
        /// <param name="PrintNow">true/false</param>
        public void Print(string PrintTypeCode, bool PrintNow)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Begin PrintServiceReq Print");

                this.IsProcessDataPrint = false;

                this.printNow = PrintNow;

                Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint 1");
                this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                this.TotalSereServPrint = HisServiceReqListResultSDO != null && HisServiceReqListResultSDO.SereServs != null ? HisServiceReqListResultSDO.SereServs.Count : 0;
                //tạo thread trong th in ngay
                if (this.printNow && !this.IsView)
                {
                    Inventec.Common.Logging.LogSystem.Info("PrintServiceReq Print PrintNow");
                    CreateThreadPrintNow(PrintTypeCode);
                }
                else
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                    richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);
                    richEditorMain.RunPrintTemplate(PrintTypeCode, DelegateRunPrinter);

                    if (Config.IsmergePrint)
                    {
                        Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint 2");
                        int countTimeOut = 0;
                        //in gộp xét nghiệp sẽ tăng số lượng dịch vụ
                        while (this.TotalSereServPrint != this.CountSereServPrinted && countTimeOut < TIME_OUT_PRINT_MERGE && !CancelPrint)
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

                        Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo adodata = this.GroupStreamPrint.First();
                        Inventec.Common.Logging.LogSystem.Debug("List MPS Group: " + string.Join("; ", this.GroupStreamPrint.Select(s => s.printTypeCode).Distinct()));
                        Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(adodata.saveMemoryStream, adodata.printerName, adodata.fileName, adodata.numCopy, true,
                            adodata.isAllowExport, adodata.TemplateKey, adodata.eventLog, adodata.eventPrint, adodata.EmrInputADO, adodata.PrintLog, adodata.ShowPrintLog, adodata.IsAllowEditTemplateFile, adodata.IsSingleCopy);
                        printProcess.SetPartialFile(this.GroupStreamPrint);
                        printProcess.PrintPreviewShow();
                    }
                }

                Inventec.Common.Logging.LogSystem.Info("End PrintServiceReq Print");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadInCacPhieuChiDinh()
        {
            Thread inCacPhieuChiDinh = new Thread(PrinNowInCacPhieuChiDinh);
            try
            {
                inCacPhieuChiDinh.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrinNowInCacPhieuChiDinh()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);
                if (IsGroup)
                {
                    richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000340__GOP_CHI_DINH, DelegateRunPrinter);
                }
                else InCacPhieuChiDinhProcess(richEditorMain);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadPrintNow(string PrintTypeCode)
        {
            System.Threading.Thread printNow = new Thread(PrintNow);
            try
            {
                printNow.Start(PrintTypeCode);
            }
            catch (Exception ex)
            {
                printNow.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintNow(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                    richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);
                    richEditorMain.RunPrintTemplate(obj.ToString(), DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InCacPhieuChiDinhProcess(Inventec.Common.RichEditor.RichEditorStore richEditorMain)
        {
            try
            {
                //sắp xếp thứ tự in theo loại bhyt
                if (this.HisServiceReqListResultSDO != null && this.HisServiceReqListResultSDO.ServiceReqs != null && this.HisServiceReqListResultSDO.ServiceReqs.Count > 0)
                {
                    List<long> orderServiceReqTypeId = ProcessOrder();

                    foreach (var serviceReqTypeId in orderServiceReqTypeId)
                    {
                        if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000001__KHAM, DelegateRunPrinter);
                        }
                        else if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000026__XET_NGHIEM, DelegateRunPrinter);
                        }
                        else if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000167__GPBL, DelegateRunPrinter);
                        }
                        else if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000028__CHUAN_DOAN_HINH_ANH, DelegateRunPrinter);
                        }
                        else if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000029__NOI_SOI, DelegateRunPrinter);
                        }
                        else if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000030__SIEU_AM, DelegateRunPrinter);
                        }
                        else if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000031__THU_THUAT, DelegateRunPrinter);
                        }
                        else if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000036__PHAU_THUAT, DelegateRunPrinter);
                        }
                        else if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000038__THAM_DO_CHUC_NANG, DelegateRunPrinter);
                        }
                        else if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000040__DICH_VU_KHAC, DelegateRunPrinter);
                        }
                        else if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000042__GIUONG, DelegateRunPrinter);
                        }
                        else if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000053__PHUC_HOI_CHUC_NANG, DelegateRunPrinter);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<long> ProcessOrder()
        {
            List<long> result = new List<long>();
            try
            {
                if (this.HisServiceReqListResultSDO.SereServs != null && this.HisServiceReqListResultSDO.SereServs.Count > 0)
                {
                    this.HisServiceReqListResultSDO.SereServs = this.HisServiceReqListResultSDO.SereServs.OrderBy(o => o.TDL_SERVICE_TYPE_ID).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                    var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>();
                    if (serviceType != null && serviceType.Count > 0)
                    {
                        serviceType = serviceType.Where(o => this.HisServiceReqListResultSDO.SereServs.Exists(e => e.TDL_SERVICE_TYPE_ID == o.ID)).ToList();
                        serviceType = serviceType.OrderBy(o => o.NUM_ORDER ?? 9999).ThenBy(o => o.SERVICE_TYPE_CODE).ToList();
                        var group = this.HisServiceReqListResultSDO.SereServs.GroupBy(o => new { o.TDL_SERVICE_TYPE_ID, o.TDL_EXECUTE_ROOM_ID }).ToList();
                        List<long> serviceReqIds = new List<long>();
                        foreach (var type in serviceType)
                        {
                            var ss = group.Where(o => o.Key.TDL_SERVICE_TYPE_ID == type.ID).ToList();
                            if (ss != null && ss.Count() > 0)
                            {
                                foreach (var item in ss)
                                {
                                    result.AddRange(item.Select(s => s.TDL_SERVICE_REQ_TYPE_ID));
                                    serviceReqIds.AddRange(item.Select(s => s.SERVICE_REQ_ID ?? 0));
                                }
                            }
                        }

                        result = result.Distinct().ToList();

                        serviceReqIds = serviceReqIds.Distinct().ToList();
                        List<V_HIS_SERVICE_REQ> lstReq = new List<V_HIS_SERVICE_REQ>();
                        foreach (var item in serviceReqIds)
                        {
                            var req = this.HisServiceReqListResultSDO.ServiceReqs.FirstOrDefault(o => o.ID == item);
                            if (req != null)
                            {
                                lstReq.Add(req);
                            }
                        }

                        Inventec.Common.Logging.LogSystem.Info("ServiceReqIds before order: " + string.Join(",", this.HisServiceReqListResultSDO.ServiceReqs.Select(s => s.ID)));
                        if (lstReq.Count > 0 && lstReq.Count == this.HisServiceReqListResultSDO.ServiceReqs.Count)
                        {
                            this.HisServiceReqListResultSDO.ServiceReqs = lstReq;
                            Inventec.Common.Logging.LogSystem.Info("ServiceReqIds after order: " + string.Join(",", this.HisServiceReqListResultSDO.ServiceReqs.Select(s => s.ID)));
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Info("ServiceReqIds order: " + string.Join(",", lstReq));
                        }
                    }
                }
                else if (this.HisServiceReqListResultSDO.ServiceReqs != null && this.HisServiceReqListResultSDO.ServiceReqs.Count > 0)
                {
                    result = this.HisServiceReqListResultSDO.ServiceReqs.Select(s => s.SERVICE_REQ_TYPE_ID).Distinct().OrderBy(o => o).ToList();
                }
            }
            catch (Exception ex)
            {
                result = new List<long>();
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
                if (IsProcessDataPrint || ProcessDataBeforePrint())
                {
                    string treatmentCode = !String.IsNullOrEmpty(EmrDataStore.treatmentCode) ? ProcessDeleteZeroFromCode(EmrDataStore.treatmentCode) : "";
                    if (chiDinhDichVuADO != null && chiDinhDichVuADO.treament != null && treatmentCode != chiDinhDichVuADO.treament.TREATMENT_CODE)
                    {
                        EmrDataStore.treatmentCode = (chiDinhDichVuADO != null && chiDinhDichVuADO.treament != null ? chiDinhDichVuADO.treament.TREATMENT_CODE : "");
                    }
                    else if (!(chiDinhDichVuADO != null && chiDinhDichVuADO.treament != null))
                    {
                        EmrDataStore.treatmentCode = "";
                    }

                    if (Config.BARCODE_NO_ZERO)
                    {
                        if (dicServiceReqData != null && dicServiceReqData.Values != null && dicServiceReqData.Values.Count > 0)
                        {
                            foreach (var items in dicServiceReqData.Values)
                            {
                                foreach (var item_ServiceReq in items)
                                {
                                    item_ServiceReq.TDL_PATIENT_CODE = ProcessDeleteZeroFromCode(item_ServiceReq.TDL_PATIENT_CODE);
                                    item_ServiceReq.TREATMENT_CODE = ProcessDeleteZeroFromCode(item_ServiceReq.TREATMENT_CODE);
                                }
                            }
                        }

                        if (chiDinhDichVuADO != null && chiDinhDichVuADO.treament != null)
                        {
                            chiDinhDichVuADO.treament.TREATMENT_CODE = ProcessDeleteZeroFromCode(chiDinhDichVuADO.treament.TREATMENT_CODE);
                            chiDinhDichVuADO.treament.TDL_PATIENT_CODE = ProcessDeleteZeroFromCode(chiDinhDichVuADO.treament.TDL_PATIENT_CODE);
                        }
                    }
                    if (!string.IsNullOrEmpty(gate) && chiDinhDichVuADO != null)
                        chiDinhDichVuADO.Gate = gate;
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeStore.IN__MPS000001__KHAM:
                            new InPhieuYeuCauKham(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, printNow, ref result, roomId, this.PreviewType, ReqMaxNumOrderSDO, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000071__KHAM_CHUYEN_KHOA:
                            new InPhieuKhamChuyenKhoa(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, printNow, ref result, roomId, this.PreviewType, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000037__CHI_DINH_TONG_HOP:
                            new InPhieuYeuCauChiDinhTongHop(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, printNow, ref result, roomId, IsView, this.PreviewType, SetDataGroup, CancelChooseTemplate, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000027__PHIEU_XET_NGHIEM_DOM_SOI_TRUC_TIEP:
                            new InPhieuXetNghiemDomSoi(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, printNow, ref result, roomId, this.PreviewType, SetDataGroup, CancelChooseTemplate, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000026__XET_NGHIEM:
                            new InCacPhieuChiDinh(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000167__GPBL:
                            new InCacPhieuChiDinh(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000028__CHUAN_DOAN_HINH_ANH:
                            new InCacPhieuChiDinh(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000029__NOI_SOI:
                            new InCacPhieuChiDinh(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000030__SIEU_AM:
                            new InCacPhieuChiDinh(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000031__THU_THUAT:
                            new InCacPhieuChiDinh(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000036__PHAU_THUAT:
                            new InCacPhieuChiDinh(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000038__THAM_DO_CHUC_NANG:
                            new InCacPhieuChiDinh(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000040__DICH_VU_KHAC:
                            new InCacPhieuChiDinh(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000042__GIUONG:
                            new InCacPhieuChiDinh(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000053__PHUC_HOI_CHUC_NANG:
                            new InCacPhieuChiDinh(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, SetDataGroup, CancelChooseTemplate, transReq, lstConfig, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000340__GOP_CHI_DINH:
                            new InGopPhieuChiDinh(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, SetDataGroup, CancelChooseTemplate, DlgSendResultSigned);
                            break;
                        case PrintTypeCodeStore.IN__MPS000432__XET_NGHIEM_GOP_KHOA_XU_LY:
                            new InGopXetNghiem(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, this.BedLogs, printNow, ref result, roomId, IsView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, SetDataGroup, CancelChooseTemplate, DlgSendResultSigned);
                            break;
                        default:
                            break;
                    }
                }
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

        private bool ProcessDataBeforePrint()
        {
            bool result = false;
            try
            {
                CreateThreadLoadDataForService();
                if (dicServiceReqData != null && dicServiceReqData.Count > 0)
                {
                    result = true;
                    this.IsProcessDataPrint = true;
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
            Thread threadMaxNumOrder = new Thread(ProcessGetMaxNumOrder);
            Thread threadQrCode = new Thread(GetDataPrintQrCode);
            Thread threadCard= new Thread(GetDataCard);
            try
            {
                threadCard.Start();
                threadTreatment.Start();
                threadSereServ.Start();
                threadMaxNumOrder.Start();
                threadQrCode.Start();
                threadCard.Join();
                threadTreatment.Join();
                threadSereServ.Join();
                threadMaxNumOrder.Join();
                threadQrCode.Join();
            }
            catch (Exception ex)
            {
                threadCard.Abort();
                threadTreatment.Abort();
                threadSereServ.Abort();
                threadMaxNumOrder.Abort();
                threadQrCode.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataCard()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("GetDataCard");
                if (HisTreatmentWithPatientTypeInfoSDO == null || HisTreatmentWithPatientTypeInfoSDO.HAS_CARD != 1) return;
                CommonParam param = new CommonParam();
                HisCardFilter cardFilter = new HisCardFilter();
                cardFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                cardFilter.PATIENT_ID = HisTreatmentWithPatientTypeInfoSDO.PATIENT_ID;
                var ListCard = new Inventec.Common.Adapter.BackendAdapter(param)
                      .Get<List<MOS.EFMODEL.DataModels.HIS_CARD>>("api/HisCard/Get", ApiConsumer.ApiConsumers.MosConsumer, cardFilter, param);
                if (ListCard != null && ListCard.Count > 0)
                {
                    chiDinhDichVuADO.ListCard = ListCard;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataTreatment()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("LoadThreadDataTreatment");
                if (HisTreatmentWithPatientTypeInfoSDO == null) return;
                var hisTreatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(hisTreatment, HisTreatmentWithPatientTypeInfoSDO);
                chiDinhDichVuADO.treament = hisTreatment;

                var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o =>
                    o.ID == HisTreatmentWithPatientTypeInfoSDO.TDL_FIRST_EXAM_ROOM_ID);
                chiDinhDichVuADO.FirstExamRoomName = room != null ? room.ROOM_NAME : "";


                CommonParam param = new CommonParam();
                HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TREATMENT_ID = chiDinhDichVuADO.treament.ID;
                var HIS_DHSTs = new Inventec.Common.Adapter.BackendAdapter(param)
                      .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumer.ApiConsumers.MosConsumer, dhstFilter, param);
                if (HIS_DHSTs != null && HIS_DHSTs.Count > 0)
                {
                    chiDinhDichVuADO._HIS_DHST = HIS_DHSTs.OrderByDescending(o => o.EXECUTE_TIME ?? 0).FirstOrDefault();
                }

                HIS_PATIENT _patient = null;
                HisPatientFilter _patientFilter = new HisPatientFilter();
                _patientFilter.ID = chiDinhDichVuADO.treament.PATIENT_ID;
                var patients = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, _patientFilter, param);
                if (patients != null && patients.Count > 0)
                {
                    _patient = patients.FirstOrDefault();
                }

                if (_patient != null && _patient.WORK_PLACE_ID != null)
                {
                    chiDinhDichVuADO._WORK_PLACE = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>().FirstOrDefault(p => p.ID == _patient.WORK_PLACE_ID);
                }

                if (HisServiceReqListResultSDO.SereServs == null || HisServiceReqListResultSDO.SereServs.Count <= 0)
                    return;

                chiDinhDichVuADO.ListSereServBill = HisServiceReqListResultSDO.SereServBills;
                chiDinhDichVuADO.ListSereServDeposit = HisServiceReqListResultSDO.SereServDeposits;

                List<long> transactionIds = new List<long>();
                if (HisServiceReqListResultSDO.SereServBills != null && HisServiceReqListResultSDO.SereServBills.Count > 0)
                {
                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Select(s => s.BILL_ID));
                }

                if (HisServiceReqListResultSDO.SereServDeposits != null && HisServiceReqListResultSDO.SereServDeposits.Count > 0)
                {
                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Select(s => s.DEPOSIT_ID));
                }

                if (HisServiceReqListResultSDO.Transactions == null && transactionIds.Count > 0)
                {
                    transactionIds = transactionIds.Distinct().ToList();

                    chiDinhDichVuADO.ListTransaction = new List<V_HIS_TRANSACTION>();
                    int skip = 0;
                    while (transactionIds.Count - skip > 0)
                    {
                        List<long> listIds = transactionIds.Skip(skip).Take(100).ToList();
                        skip += 100;

                        CommonParam paramCommon = new CommonParam();

                        HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                        tranFilter.IDs = listIds;
                        var lstTran = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumer.ApiConsumers.MosConsumer, tranFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (lstTran != null && lstTran.Count > 0)
                        {
                            chiDinhDichVuADO.ListTransaction.AddRange(lstTran);
                        }
                    }
                }
                else
                {
                    chiDinhDichVuADO.ListTransaction = HisServiceReqListResultSDO.Transactions;
                }
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
                Inventec.Common.Logging.LogSystem.Info("LoadThreadDataSereServ");
                if (HisServiceReqListResultSDO == null)
                    return;

                if (HisServiceReqListResultSDO.ServiceReqs == null || HisServiceReqListResultSDO.ServiceReqs.Count <= 0)
                    return;

                var patientType = new V_HIS_PATIENT_TYPE_ALTER();
                var Type = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o =>
                    o.PATIENT_TYPE_CODE == this.HisTreatmentWithPatientTypeInfoSDO.PATIENT_TYPE_CODE);
                if (Type != null)
                {
                    patientType.PATIENT_TYPE_ID = Type.ID;
                    patientType.PATIENT_TYPE_CODE = Type.PATIENT_TYPE_CODE;
                    patientType.PATIENT_TYPE_NAME = Type.PATIENT_TYPE_NAME;
                }

                patientType.HEIN_CARD_FROM_TIME = HisTreatmentWithPatientTypeInfoSDO.HEIN_CARD_FROM_TIME;
                patientType.HEIN_CARD_NUMBER = HisTreatmentWithPatientTypeInfoSDO.HEIN_CARD_NUMBER;
                patientType.HEIN_CARD_TO_TIME = HisTreatmentWithPatientTypeInfoSDO.HEIN_CARD_TO_TIME;
                patientType.HEIN_MEDI_ORG_CODE = HisTreatmentWithPatientTypeInfoSDO.HEIN_MEDI_ORG_CODE;
                patientType.LEVEL_CODE = HisTreatmentWithPatientTypeInfoSDO.LEVEL_CODE;
                patientType.RIGHT_ROUTE_CODE = HisTreatmentWithPatientTypeInfoSDO.RIGHT_ROUTE_CODE;
                patientType.RIGHT_ROUTE_TYPE_CODE = HisTreatmentWithPatientTypeInfoSDO.RIGHT_ROUTE_TYPE_CODE;
                patientType.TREATMENT_TYPE_CODE = HisTreatmentWithPatientTypeInfoSDO.TREATMENT_TYPE_CODE;
                patientType.ADDRESS = HisTreatmentWithPatientTypeInfoSDO.HEIN_CARD_ADDRESS;

                if (!String.IsNullOrEmpty(patientType.HEIN_MEDI_ORG_CODE))
                {
                    var heinMediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == patientType.HEIN_MEDI_ORG_CODE && o.IS_ACTIVE == 1);
                    patientType.HEIN_MEDI_ORG_NAME = heinMediOrg != null ? heinMediOrg.MEDI_ORG_NAME : "";
                }

                chiDinhDichVuADO.patientTypeAlter = patientType;

                if (HisConfigs.Get<string>(Config.HIS_DEPOSIT__PRICE_FOR_BHYT_KEY) == "1")
                {
                    if (chiDinhDichVuADO.patientTypeAlter.PATIENT_TYPE_ID == Config.PatientTypeId__BHYT && chiDinhDichVuADO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && HisServiceReqListResultSDO.SereServs != null && HisServiceReqListResultSDO.SereServs.Count > 0)
                    {
                        foreach (var item in HisServiceReqListResultSDO.SereServs)
                        {
                            item.VIR_PRICE = MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPatientPriceForOutBhyt(item);
                        }
                    }
                }

                //Mức hưởng BHYT
                decimal ratio = 0;
                if (chiDinhDichVuADO.patientTypeAlter != null && !String.IsNullOrEmpty(chiDinhDichVuADO.patientTypeAlter.HEIN_CARD_NUMBER))
                {
                    ratio = GetDefaultHeinRatio(chiDinhDichVuADO.patientTypeAlter.HEIN_CARD_NUMBER, chiDinhDichVuADO.patientTypeAlter.TREATMENT_TYPE_CODE, chiDinhDichVuADO.patientTypeAlter.LEVEL_CODE, chiDinhDichVuADO.patientTypeAlter.RIGHT_ROUTE_CODE);
                }
                chiDinhDichVuADO.Ratio = ratio;

                var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o =>
                    o.ID == HisServiceReqListResultSDO.ServiceReqs.First().REQUEST_ROOM_ID);
                chiDinhDichVuADO.BedRoomName = room != null ? room.ROOM_NAME : "";
                chiDinhDichVuADO.DepartmentName = room != null ? room.DEPARTMENT_NAME : "";

                dicServiceReqData = new Dictionary<long, List<V_HIS_SERVICE_REQ>>();
                dicSereServData = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                foreach (var item_ServiceReq in this.HisServiceReqListResultSDO.ServiceReqs)
                {
                    if (!dicServiceReqData.ContainsKey(item_ServiceReq.SERVICE_REQ_TYPE_ID))
                        dicServiceReqData[item_ServiceReq.SERVICE_REQ_TYPE_ID] = new List<V_HIS_SERVICE_REQ>();
                    bool t = false;
                    if (dicServiceReqData[item_ServiceReq.SERVICE_REQ_TYPE_ID].Count > 0)
                    {
                        foreach (var item in dicServiceReqData[item_ServiceReq.SERVICE_REQ_TYPE_ID])
                        {
                            if (item.ID == item_ServiceReq.ID)
                            {
                                t = true;
                                break;
                            }
                        }
                    }
                    if (t)
                        continue;
                    else
                        dicServiceReqData[item_ServiceReq.SERVICE_REQ_TYPE_ID].Add(item_ServiceReq);

                    if (HisServiceReqListResultSDO.SereServs != null && HisServiceReqListResultSDO.SereServs.Count > 0)
                    {
                        var listSereServ = HisServiceReqListResultSDO.SereServs.Where(o => o.SERVICE_REQ_ID == item_ServiceReq.ID && o.IS_NO_EXECUTE != 1).ToList();
                        if (listSereServ != null && listSereServ.Count > 0)
                        {
                            dicSereServData[item_ServiceReq.ID] = listSereServ;
                        }
                    }
                }

                dicSereServExtData = GetDicSereServExt(HisServiceReqListResultSDO.SereServs);
            }
            catch (Exception ex)
            {
                dicServiceReqData = new Dictionary<long, List<V_HIS_SERVICE_REQ>>();
                dicSereServData = new Dictionary<long, List<V_HIS_SERE_SERV>>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Dictionary<long, HIS_SERE_SERV_EXT> GetDicSereServExt(List<V_HIS_SERE_SERV> list)
        {
            Dictionary<long, HIS_SERE_SERV_EXT> result = new Dictionary<long, HIS_SERE_SERV_EXT>();
            try
            {
                if (list != null && list.Count > 0)
                {
                    List<HIS_SERE_SERV_EXT> listSereServExt = new List<HIS_SERE_SERV_EXT>();
                    var listssId = list.Select(o => o.ID).ToList();

                    CommonParam paramCommon = new CommonParam();
                    var skip = 0;
                    while (listssId.Count - skip > 0)
                    {
                        var limit = listssId.Skip(skip).Take(100).ToList();
                        skip = skip + 100;
                        MOS.Filter.HisSereServExtFilter filter = new MOS.Filter.HisSereServExtFilter();
                        filter.SERE_SERV_IDs = limit;
                        var sereServExt = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        listSereServExt.AddRange(sereServExt);
                    }

                    if (listSereServExt != null && listSereServExt.Count > 0)
                    {
                        result = listSereServExt.ToDictionary(o => o.SERE_SERV_ID);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Dictionary<long, HIS_SERE_SERV_EXT>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private decimal GetDefaultHeinRatio(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            decimal result = 0;
            try
            {
                result = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string ProcessDeleteZeroFromCode(string p)
        {
            string result = p;
            try
            {
                if (!String.IsNullOrWhiteSpace(p))
                {
                    int i = 0;
                    for (; i < p.Length; i++)
                    {
                        if (p[i] != '0')
                            break;
                    }
                    result = p.Substring(i);
                }
            }
            catch (Exception ex)
            {
                result = p;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessGetMaxNumOrder()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("ProcessGetMaxNumOrder");
                if (HisConfigs.Get<string>(Config.OptionCurrentNumOrder) == "1")
                {
                    List<long> executeRoomIds = new List<long>();
                    if (this.HisServiceReqListResultSDO.ServiceReqs != null && this.HisServiceReqListResultSDO.ServiceReqs.Count > 0)
                    {
                        executeRoomIds = this.HisServiceReqListResultSDO.ServiceReqs.Select(s => s.EXECUTE_ROOM_ID).Distinct().ToList();
                    }

                    CommonParam paramCommon = new CommonParam();
                    if (executeRoomIds != null && executeRoomIds.Count > 0)
                    {
                        HisServiceReqMaxNumOrderFilter filter = new HisServiceReqMaxNumOrderFilter();
                        filter.EXECUTE_ROOM_IDs = executeRoomIds;
                        filter.INSTRUCTION_DATE = Inventec.Common.DateTime.Get.Now() ?? 0;
                        filter.IS_PRIORITY = false;
                        filter.SERVICE_REQ_STT_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL };
                        ReqMaxNumOrderSDO = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HisServiceReqMaxNumOrderSDO>>("api/HisServiceReq/GetMaxNumOrder", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    }
                }
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
                this.CountSereServPrinted += count;
                if (data != null)
                {
                    if (this.GroupStreamPrint == null)
                    {
                        this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                    }

                    this.GroupStreamPrint.Add(data);
                }

                if (this.TotalSereServPrint == this.CountSereServPrinted || CancelPrint)
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetDataGroup ClearData");
                    this.dicServiceReqData = null;
                    this.dicSereServData = null;
                    this.dicSereServExtData = null;
                    this.HisServiceReqListResultSDO = null;
                    this.HisTreatmentWithPatientTypeInfoSDO = null;
                    this.BedLogs = null;
                    this.chiDinhDichVuADO = null;
                    this.IsProcessDataPrint = false;
                }

                Inventec.Common.Logging.LogSystem.Debug("TotalSereServPrint:" + TotalSereServPrint);
                Inventec.Common.Logging.LogSystem.Debug("CountSereServPrinted:" + CountSereServPrinted);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    public class BankInfo
    {
        public BankInfo() { }
        public string BANK { get; set; }
        public string VALUE { get; set; }
    }
}
