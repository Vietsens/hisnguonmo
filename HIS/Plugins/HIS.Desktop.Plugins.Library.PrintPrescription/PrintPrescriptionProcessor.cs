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
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintPrescription
{
    public class PrintPrescriptionProcessor
    {
        private bool printNow;
        private MPS.ProcessorBase.PrintConfig.PreviewType? previewType;
        private bool hasMediMate = false;
        private List<MOS.SDO.OutPatientPresResultSDO> OutPatientPresResultSDO { get; set; }
        private List<MOS.SDO.SubclinicalPresResultSDO> SubclinicalPresResultSDO { get; set; }
        private List<MOS.SDO.InPatientPresResultSDO> InPatientPresResultSDO { get; set; }
        private MOS.SDO.OutPatientPresResultSDO currentOutPresSDO;
        private MOS.SDO.SubclinicalPresResultSDO currentSubclinicalPresResultSDO;
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private V_HIS_SERE_SERV currentSereServ;
        private bool? hasOutHospital = false;

        private long TotalMediMatePrint { get; set; }
        private long CountMediMatePrinted { get; set; }
        private bool CancelPrint { get; set; }
        private const int TIME_OUT_PRINT_MERGE = 1200;
        private List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> GroupStreamPrint;

        /// <summary>
        /// true là được gọi từ kê đơn
        /// </summary>
        public bool CallFromPrescription = false;
        private bool IsNotShowTaken = false;
        private List<HIS_CONFIG> lstConfig;
        private List<HIS_TRANS_REQ> lstTransReq;

        /// <summary>
        /// tiennv
        /// In barcode
        /// In don phong kham tong hop se lay exp_mest tong hop de hien thi
        /// </summary>
        private HIS_EXP_MEST expMestPrimary { get; set; }

        private Inventec.Common.RichEditor.RichEditorStore richEditorMain;

        public PrintPrescriptionProcessor(List<MOS.SDO.OutPatientPresResultSDO> _OutPatientPresResultSDO, HIS_EXP_MEST _expMest, Inventec.Desktop.Common.Modules.Module module)
        {
            this.OutPatientPresResultSDO = _OutPatientPresResultSDO;
            this.expMestPrimary = _expMest;
            this.currentModule = module;
        }

        public PrintPrescriptionProcessor(List<MOS.SDO.OutPatientPresResultSDO> _OutPatientPresResultSDO, Inventec.Desktop.Common.Modules.Module module)
        {
            this.OutPatientPresResultSDO = _OutPatientPresResultSDO;
            this.currentModule = module;
        }

        public PrintPrescriptionProcessor(List<MOS.SDO.InPatientPresResultSDO> _InPatientPresResultSDO, Inventec.Desktop.Common.Modules.Module module)
        {
            this.InPatientPresResultSDO = _InPatientPresResultSDO;
            this.currentModule = module;
        }

        public PrintPrescriptionProcessor(List<MOS.SDO.OutPatientPresResultSDO> _OutPatientPresResultSDO, Inventec.Desktop.Common.Modules.Module module, bool callFromPrescription)
        {
            this.OutPatientPresResultSDO = _OutPatientPresResultSDO;
            this.currentModule = module;
            this.CallFromPrescription = callFromPrescription;
        }

        public PrintPrescriptionProcessor(List<MOS.SDO.InPatientPresResultSDO> _InPatientPresResultSDO, Inventec.Desktop.Common.Modules.Module module, bool callFromPrescription)
        {
            this.InPatientPresResultSDO = _InPatientPresResultSDO;
            this.currentModule = module;
            this.CallFromPrescription = callFromPrescription;
        }

        public PrintPrescriptionProcessor(List<MOS.SDO.OutPatientPresResultSDO> _OutPatientPresResultSDO, HIS_EXP_MEST _expMest)
        {
            this.OutPatientPresResultSDO = _OutPatientPresResultSDO;
            this.expMestPrimary = _expMest;
        }

        public PrintPrescriptionProcessor(List<MOS.SDO.OutPatientPresResultSDO> _OutPatientPresResultSDO)
        {
            this.OutPatientPresResultSDO = _OutPatientPresResultSDO;
        }

        public PrintPrescriptionProcessor(List<MOS.SDO.InPatientPresResultSDO> _InPatientPresResultSDO)
        {
            this.InPatientPresResultSDO = _InPatientPresResultSDO;
        }

        public PrintPrescriptionProcessor(List<MOS.SDO.SubclinicalPresResultSDO> _OutPatientPresResultSDO, V_HIS_SERE_SERV _SereServ, Inventec.Desktop.Common.Modules.Module module)
        {
            this.SubclinicalPresResultSDO = _OutPatientPresResultSDO;
            this.currentSereServ = _SereServ;
            this.currentModule = module;
        }

        public PrintPrescriptionProcessor(List<MOS.SDO.OutPatientPresResultSDO> _OutPatientPresResultSDO, bool _IsNotShowTaken, Inventec.Desktop.Common.Modules.Module module)
        {
            this.OutPatientPresResultSDO = _OutPatientPresResultSDO;
            this.IsNotShowTaken = _IsNotShowTaken;
            this.currentModule = module;
        }


        public PrintPrescriptionProcessor(List<MOS.SDO.OutPatientPresResultSDO> _OutPatientPresResultSDO, bool _IsNotShowTaken, Inventec.Desktop.Common.Modules.Module module, bool callFromPrescription)
        {
            this.OutPatientPresResultSDO = _OutPatientPresResultSDO;
            this.currentModule = module;
            this.CallFromPrescription = callFromPrescription;
            this.IsNotShowTaken = _IsNotShowTaken;
        }

        public PrintPrescriptionProcessor(List<MOS.SDO.InPatientPresResultSDO> _InPatientPresResultSDO, bool _IsNotShowTaken, Inventec.Desktop.Common.Modules.Module module, bool callFromPrescription)
        {
            this.InPatientPresResultSDO = _InPatientPresResultSDO;
            this.currentModule = module;
            this.CallFromPrescription = callFromPrescription;
            this.IsNotShowTaken = _IsNotShowTaken;
        }


        public void SetOutHospital(bool? _hasOutHospital)
        {
            this.hasOutHospital = _hasOutHospital;
        }

        /// <summary>
        /// Sử dụng cấu hình để in ngay ("HIS.Desktop.Plugins.Library.PrintPrescription.Mps")
        /// </summary>
        public void Print()
        {
            try
            {
                var printCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.mps);
                if (String.IsNullOrEmpty(printCode))
                {
                    printCode = MPS.Processor.Mps000118.PDO.Mps000118PDO.PrintTypeCode;
                }
                Print(printCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Print(MPS.ProcessorBase.PrintConfig.PreviewType? previewType)
        {
            try
            {
                var printCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.mps);
                if (String.IsNullOrEmpty(printCode))
                {
                    printCode = MPS.Processor.Mps000118.PDO.Mps000118PDO.PrintTypeCode;
                }
                Print(printCode, previewType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// in ngay
        /// </summary>
        /// <param name="PrintTypeCode">mã in</param>
        public void Print(string PrintTypeCode, MPS.ProcessorBase.PrintConfig.PreviewType? previewType)
        {
            try
            {
                Print(PrintTypeCode, true, previewType);
            }
            catch (Exception ex)
            {
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
        /// <param name="PrintTypeCode">Mã in (44,50,118)</param>
        /// <param name="PrintNow">true/false</param>
        public void Print(string PrintTypeCode, bool PrintNow)
        {
            try
            {
                if (Config.IsmergePrint)
                {
                    Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint");
                    this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                    //this.TotalMediMatePrint = HisServiceReqListResultSDO != null && HisServiceReqListResultSDO.SereServs != null ? HisServiceReqListResultSDO.SereServs.Count : 0;
                }

                Inventec.Common.Logging.LogSystem.Info("Begin Print Prescription");
                this.printNow = PrintNow;
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);
                richEditorMain.RunPrintTemplate(PrintTypeCode, DelegateRunPrinter);
                Inventec.Common.Logging.LogSystem.Info("End Print Prescription");


                if (Config.IsmergePrint)
                {
                    Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint");
                    int countTimeOut = 0;
                    //in gộp xét nghiệp sẽ tăng số lượng dịch vụ
                    while (this.TotalMediMatePrint != this.CountMediMatePrinted && countTimeOut < TIME_OUT_PRINT_MERGE && !CancelPrint)
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
                    Inventec.Common.Logging.LogSystem.Debug("List Group count: " + this.GroupStreamPrint.Count);
                    Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(adodata.saveMemoryStream, adodata.printerName, adodata.fileName, adodata.numCopy, true,
                        adodata.isAllowExport, adodata.TemplateKey, adodata.eventLog, adodata.eventPrint, adodata.EmrInputADO, adodata.PrintLog, adodata.ShowPrintLog, adodata.IsAllowEditTemplateFile, adodata.IsSingleCopy);
                    printProcess.SetPartialFile(this.GroupStreamPrint);
                    printProcess.PrintPreviewShow();
                }
            }
            catch (Exception ex)
            {
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

        public void Print(string PrintTypeCode, bool PrintNow, MPS.ProcessorBase.PrintConfig.PreviewType? previewType)
        {
            try
            {
                if (Config.IsmergePrint)
                {
                    Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint");
                    this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                    //this.TotalMediMatePrint = HisServiceReqListResultSDO != null && HisServiceReqListResultSDO.SereServs != null ? HisServiceReqListResultSDO.SereServs.Count : 0;
                }

                Inventec.Common.Logging.LogSystem.Info("Begin Print Prescription");
                this.printNow = PrintNow;
                this.previewType = previewType;
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);
                richEditorMain.RunPrintTemplate(PrintTypeCode, DelegateRunPrinter);
                Inventec.Common.Logging.LogSystem.Info("End Print Prescription");

                if (Config.IsmergePrint)
                {
                    Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint");
                    int countTimeOut = 0;
                    //in gộp xét nghiệp sẽ tăng số lượng dịch vụ
                    while (this.TotalMediMatePrint != this.CountMediMatePrinted && countTimeOut < TIME_OUT_PRINT_MERGE && !CancelPrint)
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

                    if (this.GroupStreamPrint != null && this.GroupStreamPrint.Count > 0)
                    {
                        Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo adodata = this.GroupStreamPrint.First();
                        Inventec.Common.Logging.LogSystem.Debug("List MPS Group: " + string.Join("; ", this.GroupStreamPrint.Select(s => s.printTypeCode).Distinct()));
                        Inventec.Common.Logging.LogSystem.Debug("List Group count: " + this.GroupStreamPrint.Count);
                        Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(adodata.saveMemoryStream, adodata.printerName, adodata.fileName, adodata.numCopy, true,
                            adodata.isAllowExport, adodata.TemplateKey, adodata.eventLog, adodata.eventPrint, adodata.EmrInputADO, adodata.PrintLog, adodata.ShowPrintLog, adodata.IsAllowEditTemplateFile, adodata.IsSingleCopy);
                        printProcess.SetPartialFile(this.GroupStreamPrint);
                        printProcess.PrintPreviewShow();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                if (ProcessDataForPrint())
                {
                    if (HisConfigs.Get<string>(Config.CONFIG_KEY__PRINT_BARCODE_NO_ZERO) == "1")
                    {
                        if (currentOutPresSDO != null && currentOutPresSDO.ServiceReqs != null && currentOutPresSDO.ServiceReqs.Count() > 0)
                        {
                            foreach (var item in currentOutPresSDO.ServiceReqs)
                            {
                                item.TDL_PATIENT_CODE = ProcessDeleteZeroFromCode(item.TDL_PATIENT_CODE);
                                item.TDL_TREATMENT_CODE = ProcessDeleteZeroFromCode(item.TDL_TREATMENT_CODE);
                            }
                        }

                        if (currentSubclinicalPresResultSDO != null && currentSubclinicalPresResultSDO.ServiceReqs != null && currentSubclinicalPresResultSDO.ServiceReqs.Count() > 0)
                        {
                            foreach (var item in currentSubclinicalPresResultSDO.ServiceReqs)
                            {
                                item.TDL_PATIENT_CODE = ProcessDeleteZeroFromCode(item.TDL_PATIENT_CODE);
                                item.TDL_TREATMENT_CODE = ProcessDeleteZeroFromCode(item.TDL_TREATMENT_CODE);
                            }
                        }
                    }

                    switch (printCode)
                    {
                        case MPS.Processor.Mps000044.PDO.Mps000044PDO.PrintTypeCode:
                            new PrintMps000044(printCode, fileName, ref result, currentOutPresSDO, printNow, hasMediMate, currentModule, richEditorMain, this.previewType, this.hasOutHospital, lstTransReq, lstConfig, SetTotalPrint, SetDataGroup, CancelChooseTemplate);
                            break;
                        case MPS.Processor.Mps000050.PDO.Mps000050PDO.PrintTypeCode:
                            new PrintMps000050(printCode, fileName, ref result, currentOutPresSDO, printNow, hasMediMate, currentModule, richEditorMain, this.previewType, SetTotalPrint, SetDataGroup, CancelChooseTemplate);
                            break;
                        case MPS.Processor.Mps000118.PDO.Mps000118PDO.PrintTypeCode:
                            new PrintMps000118(printCode, fileName, ref result, currentOutPresSDO, printNow, hasMediMate, richEditorMain, currentModule, this.previewType, this.hasOutHospital, lstTransReq, lstConfig, SetTotalPrint, SetDataGroup, CancelChooseTemplate, this.CallFromPrescription);
                            break;
                        case MPS.Processor.Mps000234.PDO.Mps000234PDO.PrintTypeCode:
                            new PrintMps000234(printCode, fileName, ref result, currentOutPresSDO, printNow, hasMediMate, richEditorMain, this.expMestPrimary, currentModule, this.previewType, SetTotalPrint, SetDataGroup, CancelChooseTemplate, this.CallFromPrescription, this.IsNotShowTaken);
                            break;
                        case MPS.Processor.Mps000296.PDO.Mps000296PDO.PrintTypeCode:
                            new PrintMps000296(printCode, fileName, ref result, currentOutPresSDO, printNow, hasMediMate, currentModule, this.previewType, SetTotalPrint, SetDataGroup, CancelChooseTemplate);
                            break;
                        case "Mps000338"://MPS.Processor.Mps000338.PDO.Mps000338PDO.PrintTypeCode
                            new PrintMps000338(printCode, fileName, ref result, currentSubclinicalPresResultSDO, currentSereServ, printNow, hasMediMate, richEditorMain, currentModule, this.previewType, SetTotalPrint, SetDataGroup, CancelChooseTemplate);
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
        class ConfigInfo
        {
            public string BANK { get; set; }
            public string VALUE { get; set; }
        }
        private void GetDataPrintQrCode(List<long> transReqId)
        {
            try
            {
                var currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(s => s.ID == this.currentModule.RoomId && !string.IsNullOrEmpty(s.QR_CONFIG_JSON)).FirstOrDefault();
                if(currentRoom != null)
                {
                    if (this.lstConfig == null)
                        lstConfig = new List<HIS_CONFIG>();
                    lstConfig.Clear();
                    ConfigInfo _config = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigInfo>(currentRoom.QR_CONFIG_JSON);
                    HIS_CONFIG _cf = new HIS_CONFIG();
                    if (string.IsNullOrWhiteSpace(_config.BANK))
                        Inventec.Common.Logging.LogSystem.Warn("Cau hinh thieu thong tin ngan hang.roomID: (" + this.currentModule.RoomId + "). Cau hinh: " + currentRoom.QR_CONFIG_JSON);
                    _cf.KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", _config.BANK.Trim());
                    _cf.VALUE = _config.VALUE;
                    lstConfig.Add(_cf);
                }
                else 
                    lstConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();
                if (lstConfig != null && lstConfig.Count > 0 && transReqId != null && transReqId.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisTransReqFilter filter = new HisTransReqFilter();
                    filter.IDs = transReqId;
                    lstTransReq = new Inventec.Common.Adapter.BackendAdapter(param)
                      .Get<List<MOS.EFMODEL.DataModels.HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool ProcessDataForPrint()
        {
            bool result = false;
            try
            {
                if (OutPatientPresResultSDO != null && OutPatientPresResultSDO.Count > 0)
                {
                    currentOutPresSDO = new MOS.SDO.OutPatientPresResultSDO();
                    currentOutPresSDO.ExpMests = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>();
                    currentOutPresSDO.Materials = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>();
                    currentOutPresSDO.Medicines = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE>();
                    currentOutPresSDO.ServiceReqMaties = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_MATY>();
                    currentOutPresSDO.ServiceReqMeties = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY>();
                    currentOutPresSDO.ServiceReqs = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>();
                    foreach (var item in OutPatientPresResultSDO)
                    {
                        if (item.ExpMests != null && item.ExpMests.Count > 0)
                            currentOutPresSDO.ExpMests.AddRange(item.ExpMests);
                        if (item.Materials != null && item.Materials.Count > 0)
                        {
                            currentOutPresSDO.Materials.AddRange(item.Materials);
                            hasMediMate = true;
                        }
                        if (item.Medicines != null && item.Medicines.Count > 0)
                        {
                            currentOutPresSDO.Medicines.AddRange(item.Medicines);
                            hasMediMate = true;
                        }
                        if (item.ServiceReqMaties != null && item.ServiceReqMaties.Count > 0)
                        {
                            currentOutPresSDO.ServiceReqMaties.AddRange(item.ServiceReqMaties);
                            hasMediMate = true;
                        }
                        if (item.ServiceReqMeties != null && item.ServiceReqMeties.Count > 0)
                        {
                            currentOutPresSDO.ServiceReqMeties.AddRange(item.ServiceReqMeties);
                            hasMediMate = true;
                        }
                        if (item.ServiceReqs != null && item.ServiceReqs.Count > 0)
                            currentOutPresSDO.ServiceReqs.AddRange(item.ServiceReqs);
                    }
                    GetDataPrintQrCode(currentOutPresSDO.ServiceReqs.Select(o => o.TRANS_REQ_ID ?? 0).ToList());
                    result = true;
                }
                else if (InPatientPresResultSDO != null && InPatientPresResultSDO.Count > 0)
                {
                    currentOutPresSDO = new MOS.SDO.OutPatientPresResultSDO();
                    currentOutPresSDO.ExpMests = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>();
                    currentOutPresSDO.Materials = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>();
                    currentOutPresSDO.Medicines = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE>();
                    currentOutPresSDO.ServiceReqMaties = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_MATY>();
                    currentOutPresSDO.ServiceReqMeties = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY>();
                    currentOutPresSDO.ServiceReqs = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>();
                    foreach (var item in InPatientPresResultSDO)
                    {
                        if (item.ExpMests != null && item.ExpMests.Count > 0)
                            currentOutPresSDO.ExpMests.AddRange(item.ExpMests);
                        if (item.Materials != null && item.Materials.Count > 0)
                        {
                            currentOutPresSDO.Materials.AddRange(item.Materials);
                            hasMediMate = true;
                        }
                        if (item.Medicines != null && item.Medicines.Count > 0)
                        {
                            currentOutPresSDO.Medicines.AddRange(item.Medicines);
                            hasMediMate = true;
                        }
                        if (item.ServiceReqMaties != null && item.ServiceReqMaties.Count > 0)
                        {
                            currentOutPresSDO.ServiceReqMaties.AddRange(item.ServiceReqMaties);
                            hasMediMate = true;
                        }
                        if (item.ServiceReqMeties != null && item.ServiceReqMeties.Count > 0)
                        {
                            currentOutPresSDO.ServiceReqMeties.AddRange(item.ServiceReqMeties);
                            hasMediMate = true;
                        }
                        if (item.ServiceReqs != null && item.ServiceReqs.Count > 0)
                            currentOutPresSDO.ServiceReqs.AddRange(item.ServiceReqs);
                    }
                    GetDataPrintQrCode(currentOutPresSDO.ServiceReqs.Select(o => o.TRANS_REQ_ID ?? 0).ToList());
                    result = true;
                }
                else if (SubclinicalPresResultSDO != null && SubclinicalPresResultSDO.Count > 0)
                {
                    currentSubclinicalPresResultSDO = new MOS.SDO.SubclinicalPresResultSDO();
                    currentSubclinicalPresResultSDO.ExpMests = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>();
                    currentSubclinicalPresResultSDO.Materials = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>();
                    currentSubclinicalPresResultSDO.Medicines = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE>();
                    currentSubclinicalPresResultSDO.ServiceReqs = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>();
                    foreach (var item in SubclinicalPresResultSDO)
                    {
                        if (item.ExpMests != null && item.ExpMests.Count > 0)
                            currentSubclinicalPresResultSDO.ExpMests.AddRange(item.ExpMests);
                        if (item.Materials != null && item.Materials.Count > 0)
                        {
                            currentSubclinicalPresResultSDO.Materials.AddRange(item.Materials);
                            hasMediMate = true;
                        }
                        if (item.Medicines != null && item.Medicines.Count > 0)
                        {
                            currentSubclinicalPresResultSDO.Medicines.AddRange(item.Medicines);
                            hasMediMate = true;
                        }
                        if (item.ServiceReqs != null && item.ServiceReqs.Count > 0)
                            currentSubclinicalPresResultSDO.ServiceReqs.AddRange(item.ServiceReqs);
                    }

                    if (currentSubclinicalPresResultSDO.ExpMests != null && currentSubclinicalPresResultSDO.ExpMests.Count > 0)
                    {
                        List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> ExpMests = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>();
                        foreach (var item in currentSubclinicalPresResultSDO.ExpMests)
                        {
                            if (item != null)
                            {
                                ExpMests.Add(item);
                            }
                        }
                        currentSubclinicalPresResultSDO.ExpMests = ExpMests;
                        var serviceReqId_ExpMest = currentSubclinicalPresResultSDO.ExpMests.Where(w => w.SERVICE_REQ_ID.HasValue).Select(o => o.SERVICE_REQ_ID.Value).ToList();
                        if (currentSubclinicalPresResultSDO.ServiceReqs != null && currentSubclinicalPresResultSDO.ServiceReqs.Count > 0)
                        {
                            var serviceReqs = currentSubclinicalPresResultSDO.ServiceReqs.Where(o => !serviceReqId_ExpMest.Contains(o.ID)).ToList();
                            var expMests = GetExpMestByServiceReq(serviceReqs);
                            if (expMests != null)
                            {
                                currentSubclinicalPresResultSDO.ExpMests.AddRange(expMests);
                            }
                        }
                    }
                    else if (currentSubclinicalPresResultSDO.ServiceReqs != null && currentSubclinicalPresResultSDO.ServiceReqs.Count > 0)
                    {
                        var expMests = GetExpMestByServiceReq(currentSubclinicalPresResultSDO.ServiceReqs);
                        if (expMests != null)
                        {
                            currentSubclinicalPresResultSDO.ExpMests.AddRange(expMests);
                        }
                    }
                    GetDataPrintQrCode(currentSubclinicalPresResultSDO.ServiceReqs.Select(o => o.TRANS_REQ_ID ?? 0).ToList());
                    result = true;
                }

                if (currentOutPresSDO != null)
                {
                    if (currentOutPresSDO.ExpMests != null && currentOutPresSDO.ExpMests.Count > 0)
                    {
                        List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> ExpMests = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>();
                        foreach (var item in currentOutPresSDO.ExpMests)
                        {
                            //in tại danh sách y lệnh đơn ngoài kho không có phiếu xuất. phiếu truyền vào là fake
                            if (item != null && (item.ID > 0 || item.SERVICE_REQ_ID > 0 || item.PRESCRIPTION_ID > 0))
                            {
                                ExpMests.Add(item);
                            }
                        }
                        currentOutPresSDO.ExpMests = ExpMests;
                        if (ExpMests.Count > 0)
                        {
                            var serviceReqCode_ExpMest = currentOutPresSDO.ExpMests.Where(w => !string.IsNullOrEmpty(w.TDL_SERVICE_REQ_CODE)).Select(o => o.TDL_SERVICE_REQ_CODE).ToList();
                            if (currentOutPresSDO.ServiceReqs != null && currentOutPresSDO.ServiceReqs.Count > 0)
                            {
                                var serviceReqs = currentOutPresSDO.ServiceReqs.Where(o => o.ID > 0 && !serviceReqCode_ExpMest.Contains(o.SERVICE_REQ_CODE)).ToList();
                                var expMests = GetExpMestByServiceReq(serviceReqs);
                                if (expMests != null)
                                {
                                    currentOutPresSDO.ExpMests.AddRange(expMests);
                                }
                                else
                                {
                                    //vãng lai không có thông tin y lệnh. y lệnh truyền vào là fake
                                    foreach (var item in currentOutPresSDO.ServiceReqs)
                                    {
                                        if (item.ID <= 0)
                                        {
                                            var exp = ExpMests.FirstOrDefault(o => o.SERVICE_REQ_ID == item.ID);
                                            if (exp != null)
                                            {
                                                long id = item.ID;
                                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(item, exp);
                                                item.ID = id;
                                                item.SERVICE_REQ_CODE = exp.TDL_SERVICE_REQ_CODE;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                currentOutPresSDO.ServiceReqs = new List<HIS_SERVICE_REQ>();
                                int count = 0;
                                foreach (var item in ExpMests)
                                {
                                    HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(req, item);
                                    req.ID = --count;
                                    req.SERVICE_REQ_CODE = item.TDL_SERVICE_REQ_CODE;
                                    currentOutPresSDO.ServiceReqs.Add(req);
                                }
                            }
                        }
                        else if (currentOutPresSDO.ServiceReqs != null && currentOutPresSDO.ServiceReqs.Count > 0)
                        {
                            foreach (var item in currentOutPresSDO.ServiceReqs)
                            {
                                currentOutPresSDO.ExpMests.Add(new HIS_EXP_MEST { ID = -1, SERVICE_REQ_ID = item.ID, TDL_SERVICE_REQ_CODE = item.SERVICE_REQ_CODE });
                            }
                        }
                    }
                    else if (currentOutPresSDO.ServiceReqs != null && currentOutPresSDO.ServiceReqs.Count > 0)
                    {
                        var expMests = GetExpMestByServiceReq(currentOutPresSDO.ServiceReqs);
                        if (expMests != null)
                        {
                            currentOutPresSDO.ExpMests.AddRange(expMests);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Nếu y lệnh không có phiếu xuất tương ứng thì kiểm tra phiếu xuất bán
        /// </summary>
        /// <param name="serviceReqs"></param>
        /// <returns></returns>
        private List<HIS_EXP_MEST> GetExpMestByServiceReq(List<HIS_SERVICE_REQ> serviceReqs)
        {
            List<HIS_EXP_MEST> result = null;
            if (serviceReqs != null && serviceReqs.Count > 0)
            {
                result = new List<HIS_EXP_MEST>();
                Inventec.Core.CommonParam paramCommon = new Inventec.Core.CommonParam();

                MOS.Filter.HisExpMestFilter filter = new MOS.Filter.HisExpMestFilter();
                filter.PRESCRIPTION_IDs = serviceReqs.Select(s => s.ID).ToList();
                var saleExpMest = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                foreach (var sevicereq in serviceReqs)
                {
                    var expMest = saleExpMest != null ? (saleExpMest.FirstOrDefault(o => o.PRESCRIPTION_ID == sevicereq.ID) ?? new MOS.EFMODEL.DataModels.HIS_EXP_MEST()) : new MOS.EFMODEL.DataModels.HIS_EXP_MEST();

                    expMest.SERVICE_REQ_ID = sevicereq.ID;
                    expMest.TDL_SERVICE_REQ_CODE = sevicereq.SERVICE_REQ_CODE;
                    result.Add(expMest);
                }
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

        private void SetDataGroup(int count, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo data)
        {
            try
            {
                this.CountMediMatePrinted += count;
                if (data != null)
                {
                    if (this.GroupStreamPrint == null)
                    {
                        this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                    }

                    this.GroupStreamPrint.Add(data);
                }

                if (this.TotalMediMatePrint == this.CountMediMatePrinted || CancelPrint)
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetDataGroup ClearData");
                    this.OutPatientPresResultSDO = null;
                    this.SubclinicalPresResultSDO = null;
                    this.InPatientPresResultSDO = null;
                    this.currentOutPresSDO = null;
                    this.currentSubclinicalPresResultSDO = null;
                    this.currentModule = null;
                    this.currentSereServ = null;
                }

                Inventec.Common.Logging.LogSystem.Debug("TotalMediMatePrint:" + TotalMediMatePrint);
                Inventec.Common.Logging.LogSystem.Debug("CountMediMatePrinted:" + CountMediMatePrinted);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTotalPrint(int count)
        {
            try
            {
                this.TotalMediMatePrint += count;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
