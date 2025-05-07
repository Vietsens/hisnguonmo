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
using DevExpress.Utils.Menu;
using DevExpress.XtraTab;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Resources;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.Base;
using HIS.Desktop.Plugins.Library.PrintTreatmentFinish;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.UC.DHST.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private void BtnRefreshForFormOther()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("BtnRefreshForFormOther");
                drBtnOther.DropDownControl = null;
                //LoadTreatmentByPatient();
                //this.FillDataToButtonPrintAndAutoPrint();
                this.InitDrButtonOther();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitDrButtonOther()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                //DXMenuItem itemTruyenDich = new DXMenuItem("Truyền dịch", new EventHandler(onClickTruyenDich));
                //menu.Items.Add(itemTruyenDich);
                //qtcode
                DXMenuItem itemTraSoatHoSoBenhAn = new DXMenuItem("Tra soát hồ sơ bệnh án", new EventHandler(onClickTraSoatHoSoBenhAn));
                menu.Items.Add(itemTraSoatHoSoBenhAn);
                DXMenuItem itemChiTietBenhAn = new DXMenuItem("Chi tiết bệnh án", new EventHandler(onClickChiTietBenhAn));
                menu.Items.Add(itemChiTietBenhAn);
                // qtcode
                DXMenuItem itemPhieuVoBenhAn = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_PHIEU_VO_BENH_AN", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickPhieuVoBenhAn));
                menu.Items.Add(itemPhieuVoBenhAn);

                DXMenuItem itemTrackingList = new DXMenuItem("Danh sách tờ điều trị", new EventHandler(onClickTrackingList));
                menu.Items.Add(itemTrackingList);

                DXMenuItem itemTrackingCreate = new DXMenuItem(Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnTrackingCreateV2.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickTrackingCreate));
                menu.Items.Add(itemTrackingCreate);

                DXMenuItem itemAssBlood = new DXMenuItem(Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAssBlood.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickAssBlood));
                menu.Items.Add(itemAssBlood);


                DXMenuItem itemnAccidentHurt = new DXMenuItem(Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAccidentHurt.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickAccidenHurt));
                menu.Items.Add(itemnAccidentHurt);

                DXMenuItem itemnDebateDiagnostic = new DXMenuItem(Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnitemnDebateDiagnostic.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickDebateDiagnostic));
                menu.Items.Add(itemnDebateDiagnostic);

                DXMenuItem itemMedicalAssessment = new DXMenuItem("Giám định y khoa", new EventHandler(onClickMedicalAssessment));
                menu.Items.Add(itemMedicalAssessment);

                DXMenuItem itemnDebateDiagnosticList = new DXMenuItem(Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnitemnDebateDiagnosticList.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickDebateDiagnosticList));
                menu.Items.Add(itemnDebateDiagnosticList);

                DXMenuItem itemnDrugReaction = new DXMenuItem("Phản ứng thuốc", new EventHandler(onClickDrugReaction));
                menu.Items.Add(itemnDrugReaction);

                DXMenuItem itemnAssignPaan = new DXMenuItem(Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAssignPaan.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickAssignPaan));
                menu.Items.Add(itemnAssignPaan);

                DXMenuItem itemTruyenDich = new DXMenuItem("Truyền dịch", new EventHandler(onClickTruyenDich));
                menu.Items.Add(itemTruyenDich);

                DXMenuItem itemInCongKhaiDichVu = new DXMenuItem(Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnInCongKhaiDichVu", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(OnClickInCongKhaiDichVu));
                menu.Items.Add(itemInCongKhaiDichVu);

                DXMenuItem itemYcTamUng = new DXMenuItem(Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnRequestAdvance.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(OnclickOpenMuduleRequestForAdvance));
                menu.Items.Add(itemYcTamUng);

                DXMenuItem itemXemGiayChuyenTuyen = new DXMenuItem("Giấy tờ đính kèm", new EventHandler(OnClickOpenXemGiayChuyenTuyen));
                menu.Items.Add(itemXemGiayChuyenTuyen);

                HIS.Desktop.Plugins.Library.FormOtherTreatment.FormOtherTreatmentProcessor form = new HIS.Desktop.Plugins.Library.FormOtherTreatment.FormOtherTreatmentProcessor(this.treatment, BtnRefreshJsonUpdate);

                DXSubMenuItem subBenhAn = null;
                var LstBar = form.GetDXMenuItem();
                if (LstBar != null && LstBar.Count > 0)
                {
                    //Biểu mẫu khác phiếu phẫu thuật thủ thuật
                    subBenhAn = new DXSubMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_BENH_AN", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()));

                    DXMenuItem itemBenhAnNgoaiTruDayMat = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_BENH_AN_NGOAI_TRU_DAY_MAT", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(BenhAnNgoaiTruDayMatClick));
                    subBenhAn.Items.Add(itemBenhAnNgoaiTruDayMat);

                    DXMenuItem itemBenhAnNgoaiTruGlaucoma = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_BENH_AN_NGOAI_TRU_GLAUCOMA", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(BenhAnNgoaiTruGlaucomaClick));
                    subBenhAn.Items.Add(itemBenhAnNgoaiTruGlaucoma);
                }
                menu.Items.Add(subBenhAn);

                // 56405
                var _hisRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == this.moduleData.RoomId);
                if (HisConfigCFG.executeRoomPaymentOption == "2" && _hisRoom.DEFAULT_CASHIER_ROOM_ID != null && _hisRoom.BILL_ACCOUNT_BOOK_ID != null)
                {
                    DXSubMenuItem subPay = null;

                    //Biểu mẫu khác phiếu phẫu thuật thủ thuật
                    subPay = new DXSubMenuItem("Thanh toán");
                    if (this.treatment.IS_PAUSE == 1)
                    {
                        DXMenuItem itemThanhToanChiPhiVienPhi = new DXMenuItem("Thanh toán chi phí viện phí", new EventHandler(ThanhToanChiPhiVienPhiClick));
                        subPay.Items.Add(itemThanhToanChiPhiVienPhi);
                    }
                    DXMenuItem itemCauHinhKetNoiDauDocThe = new DXMenuItem("Cấu hình kết nối đầu đọc thẻ", new EventHandler(CauHinhKetNoiDauDocTheClick));
                    subPay.Items.Add(itemCauHinhKetNoiDauDocThe);

                    menu.Items.Add(subPay);
                }

                DXSubMenuItem itemOtherForm = null;
                List<string> listExclude = new List<string>();
                listExclude.Add("Frd000010");
                var lstBar = form.GetDXMenuItem(listExclude);
                if (lstBar != null && lstBar.Count > 0)
                {
                    //Biểu mẫu khác phiếu phẫu thuật thủ thuật
                    itemOtherForm = new DXSubMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_FROM_KHAC", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()));
                    foreach (var item in lstBar)
                    {
                        itemOtherForm.Items.Add(item);
                    }
                }

                if (HisServiceReqResult != null && HisServiceReqResult.ServiceReq != null)
                {
                    this.serviceReq = HisServiceReqResult.ServiceReq;
                }
                else
                {
                    this.serviceReq = GetServiceReq();
                }
                HIS.Desktop.Plugins.Library.FormOtherServiceReq.FormOtherProcessor form1 = new HIS.Desktop.Plugins.Library.FormOtherServiceReq.FormOtherProcessor(this.serviceReq, BtnRefreshForFormOther, BtnRefreshForFormOther);
                var lstBar1 = form1.GetDXMenuItem();
                if (lstBar1 != null && lstBar1.Count > 0)
                {
                    if (itemOtherForm == null) itemOtherForm = new DXSubMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_FROM_KHAC", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()));
                    foreach (var item in lstBar1)
                    {
                        itemOtherForm.Items.Add(item);
                    }
                }
                if (itemOtherForm != null)
                    menu.Items.Add(itemOtherForm);

                drBtnOther.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnRefreshJsonUpdate(object data)
        {
            try
            {
                if (data != null && data is HIS_TREATMENT)
                {
                    this.treatment = (HIS_TREATMENT)data;
                    if (this.treatment != null)
                    {
                        UpdateNeedSickLeaveCertControl(this.treatment.NEED_SICK_LEAVE_CERT);
                    }
                    InitDrButtonOther();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateNeedSickLeaveCertControl(short? needSickLeaveCert)
        {
            try
            {
                if (needSickLeaveCert.HasValue && needSickLeaveCert.Value == 1)
                {
                    lblNeedSickLeaveCert.Text = ResourceMessage.BenhNhanCoYeuCauCapGiayNghiOm;
                    lciNeedSickLeaveCert.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lblNeedSickLeaveCert.Text = "";
                    lciNeedSickLeaveCert.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadMenu(object data)
        {
            if (data != null)
            {
                if (data is DXMenuItem)
                {
                    DXPopupMenu menu = btnPrint_ExamService.DropDownControl as DXPopupMenu;
                    if (menu == null)
                        menu = new DXPopupMenu();
                    menu.Items.Add(data as DXMenuItem);
                    btnPrint_ExamService.DropDownControl = menu;
                }
            }
        }

        private void ReLoadPrintExamAddition()
        {
            try
            {
                DXPopupMenu menu = btnPrint_ExamService.DropDownControl as DXPopupMenu;
                DXMenuItem itemKhamThem = new DXMenuItem("Yêu cầu khám thêm", new EventHandler(onClickInPhieuKhamBenh));
                itemKhamThem.Tag = PrintType.YEU_CAU_KHAM_THEM;
                menu.Items.Add(itemKhamThem);
                btnPrint_ExamService.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadPrintOutHospital()
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {

                        DXPopupMenu menu = btnPrint_ExamService.DropDownControl as DXPopupMenu;
                        DXMenuItem itemGiayRaVien = new DXMenuItem("Giấy ra viện", new EventHandler(onClickInPhieuKhamBenh));
                        itemGiayRaVien.Tag = PrintType.IN_GIAY_RA_VIEN;
                        menu.Items.Add(itemGiayRaVien);
                        btnPrint_ExamService.DropDownControl = menu;

                        LoadMenuBordereau();

                    }));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadPrintTranPati()
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {

                        DXPopupMenu menu = btnPrint_ExamService.DropDownControl as DXPopupMenu;
                        DXMenuItem itemGiayChuyenVien = new DXMenuItem("Giấy chuyển viện", new EventHandler(onClickInPhieuKhamBenh));
                        itemGiayChuyenVien.Tag = PrintType.IN_GIAY_CHUYEN_VIEN;
                        menu.Items.Add(itemGiayChuyenVien);

                        if (this.treatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                        {
                            DXMenuItem itemGiayRaVien = new DXMenuItem("Giấy ra viện", new EventHandler(onClickInPhieuKhamBenh));
                            itemGiayRaVien.Tag = PrintType.IN_GIAY_RA_VIEN;
                            menu.Items.Add(itemGiayRaVien);
                        }

                        btnPrint_ExamService.DropDownControl = menu;

                        LoadMenuBordereau();
                    }));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadPrintGiayBaoTu()
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {

                        DXPopupMenu menu = btnPrint_ExamService.DropDownControl as DXPopupMenu;
                        DXMenuItem itemGiayBaoTu = new DXMenuItem("Giấy báo tử", new EventHandler(onClickInPhieuKhamBenh));
                        itemGiayBaoTu.Tag = PrintType.IN_GIAY_BAO_TU;
                        menu.Items.Add(itemGiayBaoTu);
                        btnPrint_ExamService.DropDownControl = menu;

                        LoadMenuBordereau();
                    }));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ReLoadPrintTreatmentEndTypeExt(PrintType printType, bool isPrint = false)
        {
            try
            {
                // PrintTreatmentEndTypeExtProcessor printTreatmentEndTypeExtProcessor = new PrintTreatmentEndTypeExtProcessor(this.treatment.ID, ReloadMenuTreatmentEndTypeExt);
                PrintTreatmentEndTypeExtProcessor printTreatmentEndTypeExtProcessor = new PrintTreatmentEndTypeExtProcessor(this.treatment.ID, ReloadMenuTreatmentEndTypeExt, CreateMenu.TYPE.NORMAL, currentModuleBase != null ? currentModuleBase.RoomId : 0);//xuandv
                switch (printType)
                {
                    case PrintType.IN_GIAY_NGHI_OM:
                        if (isPrint)
                        {
                            printTreatmentEndTypeExtProcessor.Print(PrintTreatmentEndTypeExPrintType.TYPE.NGHI_OM,
                                   HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.PrintTreatmentEndTypeExtProcessor.OPTION.PRINT__INIT_MENU);
                        }
                        else
                        {
                            printTreatmentEndTypeExtProcessor.Print(PrintTreatmentEndTypeExPrintType.TYPE.NGHI_OM,
                                HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.PrintTreatmentEndTypeExtProcessor.OPTION.INIT_MENU);
                        }
                        break;
                    case PrintType.IN_GIAY_NGHI_DUONG_THAI:
                        if (isPrint)
                        {
                            printTreatmentEndTypeExtProcessor.Print(PrintTreatmentEndTypeExPrintType.TYPE.NGHI_DUONG_THAI,
                                   HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.PrintTreatmentEndTypeExtProcessor.OPTION.PRINT__INIT_MENU);
                        }
                        else
                        {
                            printTreatmentEndTypeExtProcessor.Print(PrintTreatmentEndTypeExPrintType.TYPE.NGHI_DUONG_THAI,
                                HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.PrintTreatmentEndTypeExtProcessor.OPTION.INIT_MENU);
                        }
                        break;
                    case PrintType.IN_PHIEU_HEN_MO:
                        if (isPrint)
                        {
                            printTreatmentEndTypeExtProcessor.Print(PrintTreatmentEndTypeExPrintType.TYPE.HEN_MO,
                                   HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.PrintTreatmentEndTypeExtProcessor.OPTION.PRINT__INIT_MENU);
                        }
                        else
                        {
                            printTreatmentEndTypeExtProcessor.Print(PrintTreatmentEndTypeExPrintType.TYPE.HEN_MO,
                                HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.PrintTreatmentEndTypeExtProcessor.OPTION.INIT_MENU);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadMenuTreatmentEndTypeExt(object data)
        {
            try
            {
                DXMenuItem dXMenuItem = data as DXMenuItem;
                if (dXMenuItem != null)
                {
                    DXPopupMenu menu = btnPrint_ExamService.DropDownControl as DXPopupMenu;
                    if (!menu.Items.OfType<DXMenuItem>().Any(a => a.Tag == dXMenuItem.Tag))
                    {
                        menu.Items.Add(dXMenuItem);
                        btnPrint_ExamService.DropDownControl = menu;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadPrint()
        {
            try
            {
                LoadMenuBordereau();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadPrintHasAppoinment()
        {
            try
            {
                string printerName = "";
                string fileName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode];
                }
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        DXPopupMenu menu = btnPrint_ExamService.DropDownControl as DXPopupMenu;
                        DXMenuItem itemGiayHenKham = new DXMenuItem("Giấy hẹn khám", new EventHandler(onClickInPhieuKhamBenh));
                        itemGiayHenKham.Tag = PrintType.IN_GIAY_HEN_KHAM;
                        menu.Items.Add(itemGiayHenKham);

                        if ((IsAppointment_ExamServiceAdd && IsPrintAppointment_ExamServiceAdd) || (IsAppointment_ExamFinish && IsPrintAppointment_ExamFinish))
                        {
                            HIS_SERVICE_REQ ServiceReq = new HIS_SERVICE_REQ();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(ServiceReq, this.HisServiceReqView);

                            PrintTreatmentFinishProcessor printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(this.treatment, ServiceReq, currentModuleBase != null ? currentModuleBase.RoomId : 0);
                            printTreatmentFinishProcessor.Print(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode);
                        }

                        if ((this.isPrintAppoinment && !this.isSignAppoinment))
                        {
                            HIS_SERVICE_REQ ServiceReq = new HIS_SERVICE_REQ();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(ServiceReq, this.HisServiceReqView);

                            PrintTreatmentFinishProcessor printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(this.treatment, ServiceReq, currentModuleBase != null ? currentModuleBase.RoomId : 0);
                            printTreatmentFinishProcessor.Print(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode);
                        }
                        MPS.ProcessorBase.PrintConfig.PreviewType? previewType = null;
                        if (this.isSignAppoinment)
                        {
                            if (this.isPrintAppoinment)
                            {
                                previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                            }
                            else
                            {
                                previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                            }
                            DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010, false, previewType);
                        }

                        btnPrint_ExamService.DropDownControl = menu;
                        LoadMenuBordereau();
                        PrintMps = true;
                    }));
                }
            }
            catch (Exception ex)
            {
                PrintMps = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool DelegateRunPrinter(string printTypeCode, bool isSaveAndShow, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType)
        {
            bool result = false;
            try
            {
                PrintTreatmentFinishProcessor printTreatmentFinishProcessor;
                // get bedLog
                //CommonParam param = new CommonParam();
                //MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                //bedLogViewFilter.DEPARTMENT_IDs = this.serviceReqComboResultSDO.ServiceReqs.Select(o => o.REQUEST_DEPARTMENT_ID).ToList();
                //bedLogViewFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                //var bedLogs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, param);

                if (PreviewType.HasValue)
                {
                    printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, currentModuleBase != null ? currentModuleBase.RoomId : 0, PreviewType.Value);
                }
                else
                {
                    printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, currentModuleBase != null ? currentModuleBase.RoomId : 0);
                }

                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010:
                        InPhieuHenKham(printTypeCode, printTreatmentFinishProcessor);
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

        private void InPhieuHenKham(string printTypeCode, PrintTreatmentFinishProcessor _printTreatmentFinishProcessor)
        {
            try
            {
                _printTreatmentFinishProcessor.PrintMps000010(printTypeCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadMenuBordereau()
        {
            try
            {
                CommonParam param = new CommonParam();
                var currentHisPatientTypeAlter = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_LAST_BY_TREATMENTID, ApiConsumers.MosConsumer, treatmentId, param);
                BordereauInitData bordereauInitData = new BordereauInitData();
                bordereauInitData.PatientTypeAlter = currentHisPatientTypeAlter;
                ReloadMenuOption reloadMenuOption = new ReloadMenuOption();
                reloadMenuOption.ReloadMenu = ReloadMenu;
                reloadMenuOption.Type = ReloadMenuOption.MenuType.NORMAL;
                reloadMenuOption.BordereauPrint = BordereauPrint.Type.MPS_BASE;
                HIS.Desktop.Plugins.Library.PrintBordereau.PrintBordereauProcessor processor = new PrintBordereauProcessor(this.HisServiceReqView.TREATMENT_ID, this.HisServiceReqView.TDL_PATIENT_ID, bordereauInitData, reloadMenuOption);
                if (HisConfigCFG.IsAutoExitAfterFinish && this.isPrintBordereau)
                {
                    processor.Print(PrintOption.Value.PRINT_NOW);
                }
                else if (this.isPrintBordereau && !this.isSignBordereau)
                {
                    processor.Print(PrintOption.Value.PRINT_NOW_AND_INIT_MENU);
                }
                else if (this.isPrintBordereau && this.isSignBordereau)
                {
                    processor.Print(PrintOption.Value.PRINT_NOW_AND_EMR_SIGN_NOW);
                }
                else if (!this.isPrintBordereau && this.isSignBordereau)
                {
                    processor.Print(PrintOption.Value.EMR_SIGN_NOW);
                }
                else
                {
                    processor.Print(PrintOption.Value.INIT_MENU);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadProcessAndPrintBHXH()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                if (this.IsPrintBHXH && !this.IsSignBHXH)
                {
                    richEditorMain.RunPrintTemplate("Mps000298", DelegateRunPrinterPrint);
                }
                else if (!this.IsPrintBHXH && this.IsSignBHXH)
                {
                    richEditorMain.RunPrintTemplate("Mps000298", DelegateRunPrinterSign);
                }
                else if (this.IsPrintBHXH && this.IsSignBHXH)
                {
                    richEditorMain.RunPrintTemplate("Mps000298", DelegateRunPrinterSignAndPrint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region in và ký mps000298
        bool DelegateRunPrinterPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                Mps000298(1, printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        bool DelegateRunPrinterSign(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                Mps000298(2, printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        bool DelegateRunPrinterSignAndPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                Mps000298(3, printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// PrintOrSign = 1: in
        /// PrintOrSign = 2: ký
        /// PrintOrSign = 3: in và ký
        /// </summary>
        /// <param name="PrintOrSign"></param>
        /// <param name="printTypeCode"></param>
        /// <param name="fileName"></param>
        /// <param name="result"></param>
        private void Mps000298(long PrintOrSign, string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                V_HIS_TREATMENT treatmentView = new V_HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT>(treatmentView, this.treatment);

                CommonParam param = new CommonParam();
                HisSereServFilter hisSereServFilter = new HisSereServFilter();
                hisSereServFilter.TREATMENT_ID = treatmentView.ID;
                var lstHisSereServ = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilter, ProcessLostToken, param);

                HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                filter.TREATMENT_ID = treatmentView.ID;
                var lstPatientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filter, ProcessLostToken, param);

                HisPatientFilter patientFilter = new HisPatientFilter();
                patientFilter.ID = treatmentView.PATIENT_ID;
                HIS_PATIENT Patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                HIS_SERE_SERV HisSereServ = new HIS_SERE_SERV();
                V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                if (lstHisSereServ != null && lstHisSereServ.Count > 0)
                {
                    HisSereServ = lstHisSereServ.FirstOrDefault();
                }

                if (lstPatientTypeAlter != null && lstPatientTypeAlter.Count > 0)
                {
                    PatientTypeAlter = lstPatientTypeAlter.FirstOrDefault();
                }


                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatmentView != null ? treatmentView.TREATMENT_CODE : ""), printTypeCode, (this.moduleData != null ? this.moduleData.RoomId : 0));

                Inventec.Common.Logging.LogSystem.Info("inputADO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO), inputADO));
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                MPS.Processor.Mps000298.PDO.Mps000298PDO mps000298RDO = new MPS.Processor.Mps000298.PDO.Mps000298PDO(
                    treatmentView,
                    PatientTypeAlter,
                    Patient,
                    HisSereServ
                    );

                WaitingManager.Hide();

                if (PrintOrSign == 1)
                {
                    Inventec.Common.Logging.LogSystem.Warn(" PrintOrSign == 1 in ");
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000298RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else if (PrintOrSign == 2)
                {
                    Inventec.Common.Logging.LogSystem.Warn(" PrintOrSign == 2 ký ");
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000298RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName) { EmrInputADO = inputADO });
                }
                else if (PrintOrSign == 3)
                {
                    Inventec.Common.Logging.LogSystem.Warn(" PrintOrSign == 3 ký và in ");
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000298RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, printerName) { EmrInputADO = inputADO });
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void LoadDataFromTreatment()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.HisServiceReqView.ICD_CODE) || !String.IsNullOrEmpty(this.HisServiceReqView.ICD_SUB_CODE))
                {
                    this.LoadIcdToControl(this.HisServiceReqView.ICD_CODE, this.HisServiceReqView.ICD_NAME);
                    this.LoadIcdCauseToControl(this.HisServiceReqView.ICD_CAUSE_CODE, this.HisServiceReqView.ICD_CAUSE_NAME);
                }
                else if (this.treatment != null && !String.IsNullOrEmpty(this.treatment.ICD_CODE))
                {
                    this.LoadIcdToControl(this.treatment.ICD_CODE, this.treatment.ICD_NAME);
                    this.LoadIcdCauseToControl(this.treatment.ICD_CAUSE_CODE, this.treatment.ICD_CAUSE_NAME);
                }

                this.LoadDataToIcdSub();

                this.btnAggrExam.Enabled = (this.treatment != null && this.treatment.IS_PAUSE == 1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshServiceReqInfoAfterFinish(object dataResult)
        {
            try
            {
                if (dataResult == null)
                    return;

                string icdCode = null; string icdName = null; string icSubCode = null; string ictSubName = null;
                string icdCodeYHCT = null; string icdNameYHCT = null; string icSubCodeYHCT = null; string ictSubNameYHCT = null;
                if (dataResult is OutPatientPresResultSDO)
                {
                    var serviceReq = ((OutPatientPresResultSDO)dataResult).ServiceReqs[0];

                    icdCode = serviceReq.ICD_CODE;
                    icdName = serviceReq.ICD_NAME;
                    icSubCode = serviceReq.ICD_SUB_CODE;
                    ictSubName = serviceReq.ICD_TEXT;
                    icdCodeYHCT = serviceReq.TRADITIONAL_ICD_CODE;
                    icdNameYHCT = serviceReq.TRADITIONAL_ICD_NAME;
                    icSubCodeYHCT = serviceReq.TRADITIONAL_ICD_SUB_CODE;
                    ictSubNameYHCT = serviceReq.TRADITIONAL_ICD_TEXT;
                    
                    if (serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        this.HisServiceReqView.SERVICE_REQ_STT_ID = ((OutPatientPresResultSDO)dataResult).ServiceReqs[0].SERVICE_REQ_STT_ID;
                        if (this.reLoadServiceReq != null)
                            this.reLoadServiceReq(this.HisServiceReqView);

                        if (HisConfigCFG.IsAutoExitAfterFinish)
                        {
                            XtraTabControl main = SessionManager.GetTabControlMain();
                            XtraTabPage page = main.TabPages[GlobalVariables.SelectedTabPageIndex];
                            TabControlBaseProcess.CloseCurrentTabPage(page, main);
                        }
                    }
                }
                else if (dataResult is InPatientPresResultSDO )
                {
                    var serviceReq = ((InPatientPresResultSDO)dataResult).ServiceReqs[0];//.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT
                    icdCode = serviceReq.ICD_CODE;
                    icdName = serviceReq.ICD_NAME;
                    icSubCode = serviceReq.ICD_SUB_CODE;
                    ictSubName = serviceReq.ICD_TEXT;
                    icdCodeYHCT = serviceReq.TRADITIONAL_ICD_CODE;
                    icdNameYHCT = serviceReq.TRADITIONAL_ICD_NAME;
                    icSubCodeYHCT = serviceReq.TRADITIONAL_ICD_SUB_CODE;
                    ictSubNameYHCT = serviceReq.TRADITIONAL_ICD_TEXT;
                    if (serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        this.HisServiceReqView.SERVICE_REQ_STT_ID = ((InPatientPresResultSDO)dataResult).ServiceReqs[0].SERVICE_REQ_STT_ID;
                        if (this.reLoadServiceReq != null)
                            this.reLoadServiceReq(this.HisServiceReqView);

                        if (HisConfigCFG.IsAutoExitAfterFinish)
                        {
                            XtraTabControl main = SessionManager.GetTabControlMain();
                            XtraTabPage page = main.TabPages[GlobalVariables.SelectedTabPageIndex];
                            TabControlBaseProcess.CloseCurrentTabPage(page, main);
                        }
                    }
                }
                else if (dataResult is HisServiceReqListResultSDO)
                {
                    var serviceReq = ((HisServiceReqListResultSDO)dataResult).ServiceReqs[0];//.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT
                    icdCode = serviceReq.ICD_CODE;
                    icdName = serviceReq.ICD_NAME;
                    icSubCode = serviceReq.ICD_SUB_CODE;
                    ictSubName = serviceReq.ICD_TEXT;
                    icdCodeYHCT = serviceReq.TRADITIONAL_ICD_CODE;
                    icdNameYHCT = serviceReq.TRADITIONAL_ICD_NAME;
                    icSubCodeYHCT = serviceReq.TRADITIONAL_ICD_SUB_CODE;
                    ictSubNameYHCT = serviceReq.TRADITIONAL_ICD_TEXT;
                }
                RefeshIcd(icdCode, icdName, icSubCode, ictSubName, icdCodeYHCT, icdNameYHCT, icSubCodeYHCT, ictSubNameYHCT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlByExamMain()
        {
            try
            {
                if (this.HisServiceReqView != null)
                {
                    if ((this.HisServiceReqView.IS_MAIN_EXAM ?? 0) == 1)
                    {
                        chkExamFinish.Enabled = false;
                    }
                    else
                    {
                        chkHospitalize.Enabled = false;
                        chkTreatmentFinish.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void RefeshIcd(string icdCode, string icdMainText, string ictExtraCodes, string ictExtraNames)
        {
            try
            {
                RefeshIcd(icdCode, icdMainText, ictExtraCodes, ictExtraNames, null, null, null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void RefeshIcd(string icdCode, string icdMainText, string ictExtraCodes, string ictExtraNames, string icdCodeYHCT, string icdMainTextYHCT, string ictExtraCodesYHCT, string ictExtraNamesYHCT)
        {
            try
            {
                UC.Icd.ADO.IcdInputADO icdInput = new UC.Icd.ADO.IcdInputADO();
                if (!String.IsNullOrEmpty(icdCode))
                {
                    HIS_ICD icd = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ICD_CODE == icdCode);
                    icdInput.ICD_CODE = icd.ICD_CODE;
                }
                if (!String.IsNullOrEmpty(icdMainText))
                {
                    icdInput.ICD_NAME = icdMainText;
                }

                LoadIcdToControl(icdInput.ICD_CODE, icdInput.ICD_NAME);
                LoadIcdToControlIcdSub(ictExtraCodes, ictExtraNames);
                LoadIcdToControlIcdYHCT(icdCodeYHCT, icdMainTextYHCT, ictExtraCodesYHCT, ictExtraNamesYHCT);
                if (ucHospitalize != null)
                {
                    UC.Hospitalize.ADO.HospitalizeInitADO ado = new UC.Hospitalize.ADO.HospitalizeInitADO();
                    ado.IcdCode = icdInput.ICD_CODE;
                    ado.IcdName = icdInput.ICD_NAME;
                    ado.IcdSubCode = ictExtraCodes;
                    ado.IcdText = ictExtraNames;

                    ado.TraditionalIcdCode = icdCodeYHCT;
                    ado.TraditionalIcdName = icdMainTextYHCT;
                    ado.TraditionalIcdSubCode = ictExtraCodesYHCT;
                    ado.TraditionalIcdText = ictExtraNamesYHCT;

                    if (HisServiceReqResult != null && HisServiceReqResult.HospitalizeResult != null && HisServiceReqResult.HospitalizeResult.Treatment != null)
                    {
                        ado.InCode = HisServiceReqResult.HospitalizeResult.Treatment.IN_CODE;
                        ado.isAutoCheckChkHospitalizeExam = HisConfigCFG.IsAutoCheckPrintHospitalizeExam;
                    }
                    hospitalizeProcessor.ReLoad(ucHospitalize, ado);
                }
                GetTotalIcd();
                if (ucTreatmentFinish != null)
                {
                    UC.ExamTreatmentFinish.ADO.TreatmentFinishInitADO ado = new UC.ExamTreatmentFinish.ADO.TreatmentFinishInitADO();
                    ado.IcdCode = icdInput.ICD_CODE;
                    ado.IcdName = icdInput.ICD_NAME;
                    ado.TraditionalIcdCode = icdCodeYHCT;
                    ado.TraditionalIcdName = icdMainTextYHCT;
                    ado.TraditionalIcdSubCode = ictExtraCodesYHCT;
                    ado.TraditionalIcdText = ictExtraNamesYHCT;
                    if (dicIcd != null && dicIcd.Count > 0)
                    {
                        Dictionary<string, string> dicNotIcdMain = new Dictionary<string, string>();
                        if (dicIcd.ContainsKey(this.icdDefaultFinish.ICD_CODE))
                            dicNotIcdMain = dicIcd.Where(o => o.Key != this.icdDefaultFinish.ICD_CODE).ToDictionary(o => o.Key, o => o.Value);
                        else
                            dicNotIcdMain = dicIcd;
                        ado.IcdSubCode = String.Join(";", dicNotIcdMain.Keys);
                        ado.IcdText = String.Join(";", dicNotIcdMain.Values);
                    }
                    if (lstIcdText != null && lstIcdText.Count > 0)
                    {
                        ado.IcdText += ";" + String.Join(";", lstIcdText);
                    }
                    treatmentFinishProcessor.ReLoad(ucTreatmentFinish, ado);
                }

                this.icdDefaultFinish.ICD_CODE = icdInput.ICD_CODE;
                this.icdDefaultFinish.ICD_NAME = icdInput.ICD_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        Dictionary<string, string> dicIcd = new Dictionary<string, string>();
        List<string> lstIcdText = new List<string>();
        private void GetTotalIcd()
        {
            dicIcd = new Dictionary<string, string>();
            lstIcdText = new List<string>();
            try
            {
                string icdSubCode = null;
                string icdText = null;
                var icdValue = UcIcdGetValue();
                if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                {
                    icdSubCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    icdText = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    if (dicIcd.ContainsKey(icdSubCode))
                        dicIcd.Remove(icdSubCode);
                    dicIcd[icdSubCode] = icdText;
                }
                SecondaryIcdDataADO icdSub = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                icdSubCode = icdSub != null ? icdSub.ICD_SUB_CODE : null;
                icdText = icdSub != null ? icdSub.ICD_TEXT : null;
                if (!string.IsNullOrEmpty(icdSubCode))
                {
                    var arrIcdSub = icdSubCode.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    var arrText = icdText.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < arrIcdSub.Length; i++)
                    {
                        if (dicIcd.ContainsKey(arrIcdSub[i]))
                            dicIcd.Remove(arrIcdSub[i]);
                        dicIcd[arrIcdSub[i]] = arrText.Length - 1 >= i ? arrText[i] : null;
                    }
                    if (arrIcdSub.Length < arrText.Length)
                    {
                        for (int i = arrIcdSub.Length; i < arrText.Length; i++)
                        {
                            lstIcdText.Add(arrText[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetTabPageVisible(XtraTabControl tab)
        {
            try
            {
                long tabNum = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__EXAM_SERVICE_REQ_EXCUTE__TAB_INFO_SHOW_DEFAULT);
                XtraTabPage activeTab = new XtraTabPage();
                switch (tabNum)
                {
                    case 1:
                        activeTab = xtraTabPageChung;
                        break;
                    case 2:
                        activeTab = xtraTabTuanHoan;
                        break;
                    case 3:
                        activeTab = xtraTabHoHap;
                        break;
                    case 4:
                        activeTab = xtraTabTieuHoa;
                        break;
                    case 5:
                        activeTab = xtraTabThanTietNieu;
                        break;
                    case 6:
                        activeTab = xtraTabThanKinh;
                        break;
                    case 7:
                        activeTab = xtraTabCoXuongKhop;
                        break;
                    case 8:
                        activeTab = xtraTabTaiMuiHong;
                        break;
                    case 9:
                        activeTab = xtraTabRangHamMat;
                        break;
                    case 10:
                        activeTab = xtraTabMat;
                        break;
                    case 11:
                        activeTab = xtraTabNoiTiet;
                        break;
                    case 12:
                        activeTab = xtraTabTamThan;
                        break;
                    case 13:
                        activeTab = xtraTabDinhDuong;
                        break;
                    case 14:
                        activeTab = xtraTabVanDong;
                        break;
                    case 15:
                        activeTab = xtraTabSanPhuKhoa;
                        break;
                    case 16:
                        activeTab = xtraTabDaLieu;
                        break;
                    default:
                        activeTab = null;
                        break;
                }

                long key = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__EXAM_SERVICE_REQ_EXCUTE_HIDE_TABS_INFOMATION__APPLICATION);
                if (key == 1)
                {
                    for (int i = 0; i < tab.TabPages.Count; i++)
                    {
                        tab.TabPages[i].PageVisible = false;
                    }
                }

                activeTab.PageVisible = true;
                tab.SelectedTabPage = activeTab;

                //Gán boder cho layout control trong tab page mắt vùng thông tin thị trường
                //lcForTabpageMat__ThiTruong
                lcForTabpageMat__ThiTruong.OptionsView.ShareLookAndFeelWithChildren = false;
                lcForTabpageMat__ThiTruong.LookAndFeel.SetFlatStyle();
                lcForTabpageMat__ThiTruong.OptionsView.DrawItemBorders = true;
                lcForTabpageMat__ThiTruong.OptionsView.ItemBorderColor = System.Drawing.Color.Black;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                if (this.icdProcessorYHCT != null && this.ucIcdYHCT != null)
                {
                    var icdValue = this.icdProcessorYHCT.GetValue(this.ucIcdYHCT);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        //private async Task InitUC()
        //{
        //    try
        //    {
        //        InitUCDHST();
        //        InitNextTreatmentIntruction();
        //        InitIcd();
        //        InitUcSecondaryIcd();
        //        InitUcCauseIcd();

        //        //#if DEBUG
        //        //                {
        //        //                    Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .DEBUG = true");
        //        //                    InitUCDHST();
        //        //                    InitIcd();
        //        //                    InitUcSecondaryIcd();
        //        //                    InitUcCauseIcd();
        //        //                    InitNextTreatmentIntruction();
        //        //                }
        //        //#else
        //        //                {
        //        //                    Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .DEBUG = false");
        //        //                    List<Action> methods = new List<Action>();
        //        //                    methods.Add(this.InitUCDHST);
        //        //                    methods.Add(this.InitIcd);
        //        //                    methods.Add(this.InitUcSecondaryIcd);
        //        //                    methods.Add(this.InitUcCauseIcd);
        //        //                    methods.Add(this.InitNextTreatmentIntruction);
        //        //                    ThreadCustomManager.MultipleThreadWithJoin(methods);
        //        //                }
        //        //#endif
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private async Task InitUCDHST()
        //{
        //    try
        //    {
        //        dhstProcessor = new UC.DHST.DHSTProcessor();
        //        DHSTado = new DHSTInitADO();

        //        if (requiredControl == 1 && isChronic)
        //        {
        //            DHSTado.IsRequired = true;
        //        }

        //        if ((IsLessThan1YearOld(this.HisServiceReqView.TDL_PATIENT_DOB)))
        //        {
        //            CommonParam param = new CommonParam();
        //            HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
        //            filter.TreatmentId = treatmentId;
        //            filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
        //            V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = await new BackendAdapter(param).GetAsync<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);


        //            if (currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT || currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE)
        //                DHSTado.IsRequiredWeight = true;
        //        }
        //        if (IsThan16YearOld(this.HisServiceReqView.TDL_PATIENT_DOB))
        //        {
        //            DHSTado.IsThan16YearOld = true;
        //        }

        //        DHSTado.delegateOutFocus = OutFocus;
        //        this.ucDHST = (UserControl)dhstProcessor.Run(DHSTado);
        //        if (this.ucDHST != null)
        //        {
        //            this.xtraScrollableControl1.Controls.Add(this.ucDHST);
        //            this.ucDHST.Dock = DockStyle.Fill;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void InitUcCauseIcd()
        //{
        //    try
        //    {
        //        long autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(SdaConfigKeys.AUTO_CHECK_ICD);
        //        this.IcdCauseProcessor = new HIS.UC.Icd.IcdProcessor();
        //        HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
        //        ado.DelegateNextFocus = NextForcusSubIcdCause;
        //        ado.IsUCCause = true;
        //        ado.Width = 330;
        //        ado.LblIcdMain = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblIcdCauseText.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
        //        ado.Height = 24;
        //        ado.IsColor = false;
        //        ado.DataIcds = this.currentIcds.Where(o => o.IS_CAUSE == 1)
        //            .OrderBy(p => p.ICD_CODE).ToList();
        //        ado.AutoCheckIcd = autoCheckIcd == 1 ? true : false;
        //        ado.ToolTipsIcdMain = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblIcdCauseText.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
        //        this.ucIcdCause = (UserControl)this.IcdCauseProcessor.Run(ado);

        //        if (this.ucIcdCause != null)
        //        {
        //            this.panelControlCauseIcd.Controls.Add(this.ucIcdCause);
        //            this.ucIcdCause.Dock = DockStyle.Fill;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void InitIcd()
        //{
        //    try
        //    {
        //        long autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(SdaConfigKeys.AUTO_CHECK_ICD);

        //        IcdProcessor = new HIS.UC.Icd.IcdProcessor();
        //        HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
        //        ado.DelegateNextFocus = NextForcusSubIcd;
        //        ado.DelegateRequiredCause = LoadRequiredCause;
        //        ado.IsColor = true;
        //        ado.DataIcds = this.currentIcds;
        //        ado.AutoCheckIcd = autoCheckIcd == 1 ? true : false;
        //        this.ucIcd = (UserControl)this.IcdProcessor.Run(ado);
        //        if (this.ucIcd != null)
        //        {
        //            this.panelIcd.Controls.Add(this.ucIcd);
        //            this.ucIcd.Dock = DockStyle.Fill;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void InitUcSecondaryIcd()
        //{
        //    try
        //    {
        //        subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), this.currentIcds);
        //        HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
        //        ado.DelegateNextFocus = NextForcusOut;
        //        ado.DelegateGetIcdMain = GetIcdMainCode;
        //        ado.Width = 660;
        //        ado.Height = 24;
        //        ado.TextLblIcd = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem19.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
        //        ado.TextNullValue = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.txtIcdExtraName.Properties.NullValuePrompt", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
        //        ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
        //        ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

        //        if (ucSecondaryIcd != null)
        //        {
        //            this.panelControlSubIcd.Controls.Add(ucSecondaryIcd);
        //            ucSecondaryIcd.Dock = DockStyle.Fill;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        private void InitUcIcdYHCT()
        {
            try
            {
                this.icdProcessorYHCT = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                //ado.DelegateRequiredCause = DelegateRequiredCause;
                ado.LblIcdMain = "CĐ YHCT:";
                ado.ToolTipsIcdMain = "Chẩn đoán y học cổ truyền";
                ado.Width = 450;
                ado.Height = 30;
                ado.hisTreatment = treatment;
                //ado.LabelTextSize = 100;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_TRADITIONAL == 1).OrderBy(o => o.ICD_CODE).ToList();
                ado.AutoCheckIcd = HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue;
                this.ucIcdYHCT = (UserControl)this.icdProcessorYHCT.Run(ado);

                if (this.ucIcdYHCT != null)
                {
                    this.panelControlIcdYHCT.Controls.Add(this.ucIcdYHCT);
                    this.ucIcdYHCT.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcdYHCT()
        {
            try
            {
                var dataICD = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_TRADITIONAL == 1).OrderBy(o => o.ICD_CODE).ToList();
                this.subIcdProcessorYHCT = new SecondaryIcdProcessor(new CommonParam(), dataICD);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                //ado.DelegateNextFocus = NextForcusOut;
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.Width = 450;
                ado.Height = 30;
                ado.TextLblIcd = "CĐ YHCT Phụ:";
                ado.TootiplciIcdSubCode = "Chẩn đoán y học cổ truyền phụ";
                ado.hisTreatment = treatment;
                //ado.TextLblIcd = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdText.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                this.ucSecondaryIcdYHCT = (UserControl)this.subIcdProcessorYHCT.Run(ado);

                if (this.ucSecondaryIcdYHCT != null)
                {
                    this.panelControlUcSubIcdYHCT.Controls.Add(this.ucSecondaryIcdYHCT);
                    this.ucSecondaryIcdYHCT.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private async Task InitNextTreatmentIntruction()
        //{
        //    try
        //    {
        //        HisNextTreaIntrFilter filter = new HisNextTreaIntrFilter();
        //        filter.IS_ACTIVE = 1;
        //        CommonParam param = new CommonParam();
        //        List<HIS_NEXT_TREA_INTR> data = await new BackendAdapter(param)
        //            .GetAsync<List<MOS.EFMODEL.DataModels.HIS_NEXT_TREA_INTR>>("api/HisNextTreaIntr/Get", ApiConsumers.MosConsumer, filter, param);

        //        nextTreatmentIntructionProcessor = new HIS.UC.NextTreatmentInstruction.NextTreatmentInstructionProcessor();
        //        HIS.UC.NextTreatmentInstruction.ADO.NextTreatmentInstructionInitADO ado = new HIS.UC.NextTreatmentInstruction.ADO.NextTreatmentInstructionInitADO();
        //        ado.DelegateNextFocus = NextForcus;
        //        ado.DataNextTreatmentInstructions = data;
        //        this.ucNextTreatmentIntruction = (UserControl)this.nextTreatmentIntructionProcessor.Run(ado);
        //        if (this.ucNextTreatmentIntruction != null)
        //        {
        //            this.panelNextTreatmentIntruction.Controls.Add(this.ucNextTreatmentIntruction);
        //            this.ucNextTreatmentIntruction.Dock = DockStyle.Fill;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

    }
}
