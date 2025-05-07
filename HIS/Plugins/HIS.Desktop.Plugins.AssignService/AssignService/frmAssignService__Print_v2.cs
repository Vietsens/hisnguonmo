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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        Library.PrintServiceReq.PrintServiceReqProcessor PrintServiceReqProcessor;
        Dictionary<long, List<DocumentSignedUpdateIGSysResultDTO>> dSignedList = new Dictionary<long, List<DocumentSignedUpdateIGSysResultDTO>>();
        private void InitMenuToButtonPrint()
        {
            try
            {
                HIS.UC.MenuPrint.MenuPrintProcessor menuPrintProcessor = new HIS.UC.MenuPrint.MenuPrintProcessor();
                List<HIS.UC.MenuPrint.ADO.MenuPrintADO> menuPrintADOs = new List<HIS.UC.MenuPrint.ADO.MenuPrintADO>();

                HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__TongHop = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                menuPrintADO__TongHop.EventHandler = new EventHandler(OnClickInChiDinhDichVu);
                menuPrintADO__TongHop.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037;
                menuPrintADO__TongHop.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037;
                menuPrintADO__TongHop.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.pnlPrintAssignService.InTongHop.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                menuPrintADO__TongHop.Tooltip = Inventec.Common.Resource.Get.Value("frmAssignService.pnlPrintAssignService.InTongHop.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                menuPrintADOs.Add(menuPrintADO__TongHop);

                HIS.UC.MenuPrint.ADO.MenuPrintInitADO menuPrintInitADO = new HIS.UC.MenuPrint.ADO.MenuPrintInitADO(menuPrintADOs, BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>());
                menuPrintInitADO.ControlContainer = pnlPrintAssignService;
                var uc = menuPrintProcessor.Run(menuPrintInitADO);
                if (uc == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("menuPrintProcessor run fail. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => menuPrintInitADO), menuPrintInitADO));
                }
                lciPrintAssignService.MinSize = new System.Drawing.Size(pnlPrintAssignService.Width, lciPrintAssignService.Height);
                lciPrintAssignService.MaxSize = new System.Drawing.Size(pnlPrintAssignService.Width, lciPrintAssignService.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMenuToButtonEMRSign()
        {
            try
            {
                //    HIS.UC.MenuPrint.MenuPrintProcessor menuPrintProcessor = new HIS.UC.MenuPrint.MenuPrintProcessor();
                //    List<HIS.UC.MenuPrint.ADO.MenuPrintADO> menuPrintADOs = new List<HIS.UC.MenuPrint.ADO.MenuPrintADO>();

                //    HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__TongHop = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                //    menuPrintADO__TongHop.EventHandler = new EventHandler(OnClickEMRShow);
                //    menuPrintADO__TongHop.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037;
                //    menuPrintADO__TongHop.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037;
                //    menuPrintADO__TongHop.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.pnlEMRSign.InTongHop.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //    menuPrintADO__TongHop.Tooltip = Inventec.Common.Resource.Get.Value("frmAssignService.pnlEMRSign.InTongHop.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //    menuPrintADOs.Add(menuPrintADO__TongHop);

                //    HIS.UC.MenuPrint.ADO.MenuPrintInitADO menuPrintInitADO = new HIS.UC.MenuPrint.ADO.MenuPrintInitADO(menuPrintADOs, BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>());
                //    menuPrintInitADO.ControlContainer = pnlEMRSign;
                //    menuPrintInitADO.IsUsingShortCut = false;
                //    var uc = menuPrintProcessor.Run(menuPrintInitADO);
                //    if (uc == null)
                //    {
                //        Inventec.Common.Logging.LogSystem.Warn("menuPrintProcessor run fail. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => menuPrintInitADO), menuPrintInitADO));
                //    }
                //    lciEMRSign.MinSize = new System.Drawing.Size(pnlEMRSign.Width, lciEMRSign.Height);
                //    lciEMRSign.MaxSize = new System.Drawing.Size(pnlEMRSign.Width, lciEMRSign.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInChiDinhDichVu(object sender, EventArgs e)
        {
            try
            {
                string printTypeCode = "";
                if (sender is DevExpress.Utils.Menu.DXMenuItem)
                {
                    var bbtnItem = sender as DevExpress.Utils.Menu.DXMenuItem;
                    printTypeCode = (bbtnItem.Tag ?? "").ToString();
                }
                else if (sender is DevExpress.XtraEditors.SimpleButton)
                {
                    var bbtnItem = sender as DevExpress.XtraEditors.SimpleButton;
                    printTypeCode = (bbtnItem.Tag ?? "").ToString();
                }
                LogTheadInSessionInfo(() => DelegateRunPrinter(printTypeCode, false, null), "PrintRequestAggregateDesignation");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickEMRShow(object sender, EventArgs e)
        {
            try
            {
                string printTypeCode = "";
                if (sender is DevExpress.Utils.Menu.DXMenuItem)
                {
                    var bbtnItem = sender as DevExpress.Utils.Menu.DXMenuItem;
                    printTypeCode = (bbtnItem.Tag ?? "").ToString();
                }
                else if (sender is DevExpress.XtraEditors.SimpleButton)
                {
                    var bbtnItem = sender as DevExpress.XtraEditors.SimpleButton;
                    printTypeCode = (bbtnItem.Tag ?? "").ToString();
                }

                DelegateRunPrinter(printTypeCode, false, MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, bool isSaveAndShow, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType)
        {
            bool result = false;
            try
            {
                // get bedLog
                //CommonParam param = new CommonParam();
                //MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                //bedLogViewFilter.DEPARTMENT_IDs = this.serviceReqComboResultSDO.ServiceReqs.Select(o => o.REQUEST_DEPARTMENT_ID).ToList();
                //bedLogViewFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                //var bedLogs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, param);

                if (PreviewType.HasValue)
                {
                    PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, null, currentModule != null ? currentModule.RoomId : 0, PreviewType.Value);
                    PrintServiceReqProcessor.IsView = isSaveAndShow;
                }
                else
                {

                    PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, null, currentModule != null ? currentModule.RoomId : 0);
                    PrintServiceReqProcessor.IsView = isSaveAndShow;
                }

                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__YEU_CAU_KHAM_CHUYEN_KHOA__MPS000071:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void InPhieuHuoangDanBenhNhan(bool isSaveAndShow)
        {
            try
            {
                
                var PrintServiceReqProcessor = new HIS.Desktop.Plugins.Library.PrintServiceReqTreatment.PrintServiceReqTreatmentProcessor(this.serviceReqComboResultSDO.ServiceReqs, currentModule != null ? this.currentModule.RoomId : 0);
                PrintServiceReqProcessor.DlgSendResultSigned = GetDocmentSigned;
                PrintServiceReqProcessor.Print("Mps000276", true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        bool IsSaveAndShowMps000102 = true;
        MPS.ProcessorBase.PrintConfig.PreviewType? PreviewTypeMps000102 = null;
        bool isPrinted = false;
        private void InTamUng(bool isSaveAndShow, MPS.ProcessorBase.PrintConfig.PreviewType? previewType)
        {
            try
            {
                var countPrintConfig = this.lstLoaiPhieu.Where(s => s.Check == true).Distinct().ToList().Count;
                // điều kiện in : có ít nhất 1 cấu hình và có giao dịch tạm ứng /// nếu có tạm ứng thì in trước
                if (serviceReqComboResultSDO.SereServDeposits != null && serviceReqComboResultSDO.SereServDeposits.Count > 0 && countPrintConfig > 0)
                {
                    this.IsSaveAndShowMps000102 = isSaveAndShow;
                    this.PreviewTypeMps000102 = previewType;
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), LocalStorage.LocalData.GlobalVariables.TemnplatePathFolder);
                    richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102, ProcessPrintMps000102);
                    isPrinted = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InPhieuYeuCauDichVu(bool isSaveAndShow, MPS.ProcessorBase.PrintConfig.PreviewType? previewType = null)
        {
            try
            {                                
                string configValue = HisConfigCFG.IsAllowSignaturePrint;

                if (!string.IsNullOrWhiteSpace(configValue))
                {
                    var allowedModules = configValue
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();

                    if (allowedModules.Contains("HIS.Desktop.Plugins.AssignService"))
                    {
                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview;
                    }
                    else
                    {
                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                    }
                }
                if (serviceReqComboResultSDO != null)
                {
                    CommonParam param = new CommonParam();
                    List<V_HIS_BED_LOG> bedLogs = new List<V_HIS_BED_LOG>();
                    // get bedLog
                    if (this.currentHisTreatment != null && this.serviceReqComboResultSDO != null && this.serviceReqComboResultSDO.ServiceReqs != null && this.serviceReqComboResultSDO.ServiceReqs.Count > 0)
                    {
                        MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                        bedLogViewFilter.TREATMENT_ID = currentHisTreatment.ID;
                        bedLogViewFilter.DEPARTMENT_IDs = this.serviceReqComboResultSDO.ServiceReqs.Select(o => o.REQUEST_DEPARTMENT_ID).Distinct().ToList();
                        bedLogs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, param);
                    }
                    var PrintServiceReqProcessor = previewType != null ? new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, bedLogs, (currentModule != null ? currentModule.RoomId : 0), previewType.Value, GetDocmentSigned)
                        : new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, bedLogs, (currentModule != null ? currentModule.RoomId : 0));
                    PrintServiceReqProcessor.SaveNPrint(isSaveAndShow);
                                           
                    if (this.serviceReqComboResultSDO.SereServs != null)
                    {
                        ProcessOpenVoBenhAn(serviceReqComboResultSDO.SereServs);
                        Inventec.Common.Logging.LogSystem.Debug("PRINT NOW serviceReqComboResultSDO.SereServs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.serviceReqComboResultSDO.SereServs), this.serviceReqComboResultSDO.SereServs));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void GetDocmentSigned(DocumentSignedUpdateIGSysResultDTO dTO)
        {
            try
            {
                if (!dSignedList.ContainsKey(this.serviceReqComboResultSDO.ServiceReqs[0].TREATMENT_ID))
                    dSignedList[this.serviceReqComboResultSDO.ServiceReqs[0].TREATMENT_ID] = new List<DocumentSignedUpdateIGSysResultDTO>();
                dSignedList[this.serviceReqComboResultSDO.ServiceReqs[0].TREATMENT_ID].Add(dTO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private bool ProcessPrintMps000102(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (serviceReqComboResultSDO != null && serviceReqComboResultSDO.SereServDeposits != null && serviceReqComboResultSDO.SereServDeposits.Count > 0 && serviceReqComboResultSDO.Transactions != null && serviceReqComboResultSDO.Transactions.Count > 0)
                {
                    V_HIS_TRANSACTION transactionPrint = new V_HIS_TRANSACTION();
                    List<HIS_SERE_SERV_DEPOSIT> ssDepositPrint = new List<HIS_SERE_SERV_DEPOSIT>();
                    transactionPrint = serviceReqComboResultSDO.Transactions.FirstOrDefault(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU);
                    if (transactionPrint == null)
                        return result;

                    ssDepositPrint = serviceReqComboResultSDO.SereServDeposits.Where(o => o.DEPOSIT_ID == transactionPrint.ID).ToList();

                    //chỉ định chưa có thời gian ra viện nên chưa cso số ngày điều trị
                    long? totalDay = null;
                    string departmentName = "";

                    //sử dụng DepositedSereServs để hiển thị thêm dịch vụ thanh toán cha
                    List<V_HIS_SERE_SERV> sereServs = new List<V_HIS_SERE_SERV>();
                    if (serviceReqComboResultSDO.DepositedSereServs != null && serviceReqComboResultSDO.DepositedSereServs.Count > 0)
                    {
                        sereServs = serviceReqComboResultSDO.DepositedSereServs.Where(o => ssDepositPrint.Exists(e => e.SERE_SERV_ID == o.ID)).ToList();
                    }

                    var SERVICE_REPORT_ID__HIGHTECH = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;

                    var sereServHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT_ID__HIGHTECH).ToList();
                    var sereServHitechADOs = PriceBHYTSereServAdoProcess(sereServHitechs);

                    //các sereServ trong nhóm vật tư
                    var SERVICE_REPORT__MATERIAL_VTTT_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT;
                    var sereServVTTTs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT__MATERIAL_VTTT_ID && o.IS_OUT_PARENT_FEE != null).ToList();
                    var sereServVTTTADOs = PriceBHYTSereServAdoProcess(sereServVTTTs);

                    var sereServNotHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID != SERVICE_REPORT_ID__HIGHTECH).ToList();

                    var servicePatyPrpos = lstService;

                    //Cộng các sereServ trong gói vào dv ktc
                    foreach (var sereServHitech in sereServHitechADOs)
                    {
                        List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServVTTTInKtcADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                        var sereServVTTTInKtcs = sereServs.Where(o => o.PARENT_ID == sereServHitech.ID && o.IS_OUT_PARENT_FEE == null).ToList();
                        sereServVTTTInKtcADOs = PriceBHYTSereServAdoProcess(sereServVTTTInKtcs);
                        if (sereServHitech.PRICE_POLICY != null)
                        {
                            var servicePatyPrpo = servicePatyPrpos.Where(o => o.ID == sereServHitech.SERVICE_ID && o.BILL_PATIENT_TYPE_ID == sereServHitech.PATIENT_TYPE_ID && o.PACKAGE_PRICE == sereServHitech.PRICE_POLICY).ToList();
                            if (servicePatyPrpo != null && servicePatyPrpo.Count > 0)
                            {
                                sereServHitech.VIR_PRICE = sereServHitech.PRICE;
                            }
                        }
                        else
                            sereServHitech.VIR_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);

                        sereServHitech.VIR_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);
                        sereServHitech.VIR_PATIENT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);

                        decimal totalHeinPrice = 0;
                        foreach (var sereServVTTTInKtcADO in sereServVTTTInKtcADOs)
                        {
                            totalHeinPrice += sereServVTTTInKtcADO.AMOUNT * sereServVTTTInKtcADO.PRICE_BHYT;
                        }
                        sereServHitech.PRICE_BHYT += totalHeinPrice;
                        sereServHitech.HEIN_LIMIT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.HEIN_LIMIT_PRICE);

                        sereServHitech.VIR_TOTAL_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);
                        sereServHitech.VIR_TOTAL_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        sereServHitech.VIR_TOTAL_PATIENT_PRICE = sereServHitech.VIR_TOTAL_PRICE - sereServHitech.VIR_TOTAL_HEIN_PRICE;
                        sereServHitech.SERVICE_UNIT_NAME = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == sereServHitech.TDL_SERVICE_UNIT_ID).SERVICE_UNIT_NAME;
                    }

                    //Lọc các sereServ nằm không nằm trong dịch vụ ktc và vật tư thay thế
                    //
                    var sereServDeleteADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    foreach (var sereServVTTTADO in sereServVTTTADOs)
                    {
                        var sereServADODelete = sereServHitechADOs.Where(o => o.ID == sereServVTTTADO.PARENT_ID).ToList();
                        if (sereServADODelete.Count == 0)
                        {
                            sereServDeleteADOs.Add(sereServVTTTADO);
                        }
                    }

                    foreach (var sereServDelete in sereServDeleteADOs)
                    {
                        sereServVTTTADOs.Remove(sereServDelete);
                    }
                    var sereServVTTTIds = sereServVTTTADOs.Select(o => o.ID);
                    sereServNotHitechs = sereServNotHitechs.Where(o => !sereServVTTTIds.Contains(o.ID)).ToList();
                    var sereServNotHitechADOs = PriceBHYTSereServAdoProcess(sereServNotHitechs);

                    string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentHisPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentHisPatientTypeAlter.HEIN_CARD_NUMBER, currentHisPatientTypeAlter.LEVEL_CODE, currentHisPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                    MPS.Processor.Mps000102.PDO.PatientADO patientAdo = new MPS.Processor.Mps000102.PDO.PatientADO(this.patientPrint);

                    if (sereServNotHitechADOs != null && sereServNotHitechADOs.Count > 0)
                    {
                        sereServNotHitechADOs = sereServNotHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    if (sereServHitechADOs != null && sereServHitechADOs.Count > 0)
                    {
                        sereServHitechADOs = sereServHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    if (sereServVTTTADOs != null && sereServVTTTADOs.Count > 0)
                    {
                        sereServVTTTADOs = sereServVTTTADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    V_HIS_SERVICE_REQ firsExamRoom = new V_HIS_SERVICE_REQ();
                    if (this.currentHisTreatment.TDL_FIRST_EXAM_ROOM_ID.HasValue)
                    {
                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentHisTreatment.TDL_FIRST_EXAM_ROOM_ID);
                        if (room != null)
                        {
                            firsExamRoom.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                        }
                    }

                    MPS.Processor.Mps000102.PDO.Mps000102PDO mps000102RDO = new MPS.Processor.Mps000102.PDO.Mps000102PDO(
                            patientAdo,
                            currentHisPatientTypeAlter,
                            departmentName,

                            sereServNotHitechADOs,
                            sereServHitechADOs,
                            sereServVTTTADOs,

                            null,//bản tin chuyển khoa, mps lấy ramdom thời gian vào khoa khi chỉ định tạm thời chưa cần
                            this.treatmentPrint,

                            BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>(),
                            transactionPrint,
                            ssDepositPrint,
                            totalDay,
                            ratio_text,
                            firsExamRoom
                            );
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.currentHisTreatment != null ? this.currentHisTreatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    if (this.PreviewTypeMps000102.HasValue)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, this.PreviewTypeMps000102.Value, printerName) { EmrInputADO = inputADO });
                    }
                    else if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2 || !this.IsSaveAndShowMps000102)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> PriceBHYTSereServAdoProcess(List<V_HIS_SERE_SERV> sereServs)
        {
            List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
            try
            {
                foreach (var item in sereServs)
                {
                    MPS.Processor.Mps000102.PDO.SereServGroupPlusADO sereServADO = new MPS.Processor.Mps000102.PDO.SereServGroupPlusADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>(sereServADO, item);

                    if (sereServADO.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
                    {
                        sereServADO.PRICE_BHYT = 0;
                    }
                    else
                    {
                        if (sereServADO.HEIN_LIMIT_PRICE != null && sereServADO.HEIN_LIMIT_PRICE > 0)
                            sereServADO.PRICE_BHYT = (item.HEIN_LIMIT_PRICE ?? 0);
                        else
                            sereServADO.PRICE_BHYT = item.VIR_PRICE_NO_ADD_PRICE ?? 0;
                    }

                    sereServADOs.Add(sereServADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServADOs;
        }

        private void InPhieuYeuCauDichVuSignAndPrint(MPS.ProcessorBase.PrintConfig.PreviewType previewType)
        {
            try
            {
                if (serviceReqComboResultSDO != null)
                {
                    CommonParam param = new CommonParam();
                    List<V_HIS_BED_LOG> bedLogs = new List<V_HIS_BED_LOG>();
                    // get bedLog
                    if (this.currentHisTreatment != null && this.serviceReqComboResultSDO != null && this.serviceReqComboResultSDO.ServiceReqs != null && this.serviceReqComboResultSDO.ServiceReqs.Count > 0)
                    {
                        MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                        bedLogViewFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                        bedLogViewFilter.DEPARTMENT_IDs = this.serviceReqComboResultSDO.ServiceReqs.Select(o => o.REQUEST_DEPARTMENT_ID).Distinct().ToList();
                        bedLogs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, param);
                    }
                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, bedLogs, currentModule != null ? currentModule.RoomId : 0, previewType);
                    PrintServiceReqProcessor.SaveNPrint(false);

                    if (this.serviceReqComboResultSDO.SereServs != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("PRINT NOW serviceReqComboResultSDO.SereServs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.serviceReqComboResultSDO.SereServs), this.serviceReqComboResultSDO.SereServs));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void InPhieuYeuCauDichVuVaERM()
        {
            try
            {
                if (serviceReqComboResultSDO != null)
                {
                    CommonParam param = new CommonParam();
                    List<V_HIS_BED_LOG> bedLogs = new List<V_HIS_BED_LOG>();
                    // get bedLog
                    if (this.currentHisTreatment != null && this.serviceReqComboResultSDO != null && this.serviceReqComboResultSDO.ServiceReqs != null && this.serviceReqComboResultSDO.ServiceReqs.Count > 0)
                    {
                        MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                        bedLogViewFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                        bedLogViewFilter.DEPARTMENT_IDs = this.serviceReqComboResultSDO.ServiceReqs.Select(o => o.REQUEST_DEPARTMENT_ID).Distinct().ToList();
                        bedLogs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, param);
                    }
                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, bedLogs, currentModule != null ? currentModule.RoomId : 0, MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow);
                    PrintServiceReqProcessor.ERMOpen();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void InPhieuYeuCauDichVu(string printTypeCode)
        {
            try
            {
                if (PrintServiceReqProcessor != null)
                {
                    PrintServiceReqProcessor.Print(printTypeCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void InYeuCauThanhToanQR(bool printTH, bool isSign, bool isPrintPreview)
        {
            try
            {
                
                if (serviceReqComboResultSDO != null || IsActionButtonPrintBill)
                {
                    BordereauInitData data = new BordereauInitData();
                    HIS.Desktop.Plugins.Library.PrintBordereau.PrintBordereauProcessor processor = new PrintBordereauProcessor(this.currentModule.RoomId, this.currentModule.RoomTypeId, treatmentId, patientPrint.ID, null, null, GetDocmentSigned);
                    if (IsActionButtonPrintBill)
                        processor.IsActionButtonPrintBill = true;
                    if (printTH && !isSign)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Mps000446_____ PRINT_NOW");
                        processor.Print("Mps000446", PrintOption.Value.PRINT_NOW, null);
                    }
                    else if (printTH && isSign)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Mps000446_____ PRINT_NOW_AND_EMR_SIGN_NOW");
                        processor.Print("Mps000446", PrintOption.Value.PRINT_NOW_AND_EMR_SIGN_NOW, null);
                    }
                    else if (isPrintPreview && isSign)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Mps000446_____ EMR_SIGN_AND_PRINT_PREVIEW");
                        processor.Print("Mps000446", PrintOption.Value.EMR_SIGN_AND_PRINT_PREVIEW, null);
                    }
                    else if (!printTH && isSign)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Mps000446_____ EMR_SIGN_NOW");
                        processor.Print("Mps000446", PrintOption.Value.EMR_SIGN_NOW, null);
                    }
                    else if (isPrintPreview)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Mps000446_____ NULL");
                        processor.Print("Mps000446", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadDataForPrint()
        {
            System.Threading.Thread thread = new System.Threading.Thread(LoadDataForPrint);
            System.Threading.Thread thread2 = new System.Threading.Thread(ProcessGetDataDepartment);
            try
            {
                thread.Start();
                thread2.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void LoadDataForPrint()
        {
            try
            {
                if (this.currentHisTreatment != null)
                {
                    MOS.Filter.HisPatientViewFilter patientViewFilter = new MOS.Filter.HisPatientViewFilter();
                    patientViewFilter.ID = this.currentHisTreatment.PATIENT_ID;
                    var patients = new BackendAdapter(null).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientViewFilter, null);
                    if (patients != null && patients.Count > 0)
                    {
                        this.patientPrint = patients.FirstOrDefault();
                    }

                    MOS.Filter.HisTreatmentFeeViewFilter filterTreatmentFee = new MOS.Filter.HisTreatmentFeeViewFilter();
                    filterTreatmentFee.ID = this.currentHisTreatment.ID;
                    var listTreatment = new BackendAdapter(null)
                      .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, filterTreatmentFee, null);
                    if (listTreatment != null && listTreatment.Count > 0)
                    {
                        this.treatmentPrint = listTreatment.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
