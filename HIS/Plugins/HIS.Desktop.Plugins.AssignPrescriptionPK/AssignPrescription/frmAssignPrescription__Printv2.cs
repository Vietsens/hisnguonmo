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
using DevExpress.XtraEditors;
using HIS.Desktop.ApplicationFont;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.PrintTreatmentFinish;
using HIS.Desktop.Print;
using HIS.UC.MenuPrint.ADO;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using MPS.ProcessorBase;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using static MPS.ProcessorBase.PrintConfig;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        List<HIS.UC.MenuPrint.ADO.MenuPrintADO> menuPrintADOs;
        private async Task InitMenuToButtonPrint()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitMenuToButtonPrint .1");
                HIS.UC.MenuPrint.MenuPrintProcessor menuPrintProcessor = new HIS.UC.MenuPrint.MenuPrintProcessor();
                menuPrintADOs = new List<HIS.UC.MenuPrint.ADO.MenuPrintADO>();

                HIS.UC.MenuPrint.ADO.MenuPrintADO vttsd = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                vttsd.Caption = "In tem vật tư tái sử dụng";
                vttsd.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                vttsd.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_TEM_VTTSD__MPS000494;
                vttsd.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_TEM_VTTSD__MPS000494;
                menuPrintADOs.Add(vttsd);

                HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__ThuocTongHop = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                menuPrintADO__ThuocTongHop.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                string savePrintMpsDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS.Desktop.Plugins.AssignPrescriptionPK.Config.HisConfigCFG.SAVE_PRINT_MPS_DEFAULT);
                if (savePrintMpsDefault == "Mps000234")
                {
                    menuPrintADO__ThuocTongHop.PrintTypeCode = "Mps000234";
                    menuPrintADO__ThuocTongHop.Tag = "Mps000234";
                }
                else
                {
                    menuPrintADO__ThuocTongHop.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118;
                    menuPrintADO__ThuocTongHop.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118;
                }

                menuPrintADOs.Add(menuPrintADO__ThuocTongHop);

                HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__CacDonThuoc = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                menuPrintADO__CacDonThuoc.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                menuPrintADO__CacDonThuoc.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC__MPS000044;
                menuPrintADO__CacDonThuoc.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC__MPS000044;
                menuPrintADOs.Add(menuPrintADO__CacDonThuoc);
                
                HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__YHocCoTruyen = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                menuPrintADO__YHocCoTruyen.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                menuPrintADO__YHocCoTruyen.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC_Y_HOC_CO_TRUYEN__MPS000050;
                menuPrintADO__YHocCoTruyen.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC_Y_HOC_CO_TRUYEN__MPS000050;
                menuPrintADOs.Add(menuPrintADO__YHocCoTruyen);

                var treatUC = ((this.treatmentFinishProcessor != null && this.ucTreatmentFinish != null) ? this.treatmentFinishProcessor.GetDataOutput(this.ucTreatmentFinish) : null);
                //Nếu người dùng tích chọn kết thúc điều trị > Chọn "Lưu" = Lưu toa thuốc + kết thúc điều trị (tự động in giấy hẹn khám, bảng kê nếu tích chọn) + Tự động close form kê toa + xử lý khám (có option theo user).
                if (treatUC != null && treatUC.IsAutoTreatmentFinish)
                {
                    if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                    {
                        HIS.UC.MenuPrint.ADO.MenuPrintADO inGiayChuyenVienItem = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                        inGiayChuyenVienItem.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                        inGiayChuyenVienItem.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011;
                        inGiayChuyenVienItem.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011;

                        menuPrintADOs.Add(inGiayChuyenVienItem);

                        if ((this.currentTreatmentWithPatientType.TDL_TREATMENT_TYPE_ID ?? 0) != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                        {
                            HIS.UC.MenuPrint.ADO.MenuPrintADO inGiayRaVienItem = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                            inGiayRaVienItem.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008;
                            inGiayRaVienItem.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                            inGiayRaVienItem.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008;

                            menuPrintADOs.Add(inGiayRaVienItem);
                        }
                    }
                    else if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)//Hen kham
                    {
                        HIS.UC.MenuPrint.ADO.MenuPrintADO inGiayHenKhamLaiItem = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                        inGiayHenKhamLaiItem.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010;
                        inGiayHenKhamLaiItem.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                        inGiayHenKhamLaiItem.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010;

                        menuPrintADOs.Add(inGiayHenKhamLaiItem);
                    }
                    else if ((treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN || treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN)

                        && (this.currentTreatmentWithPatientType.TDL_TREATMENT_TYPE_ID ?? 0) != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)//ra vien
                    {
                        HIS.UC.MenuPrint.ADO.MenuPrintADO inGiayRaVienItem = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                        inGiayRaVienItem.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008;
                        inGiayRaVienItem.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                        inGiayRaVienItem.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008;

                        menuPrintADOs.Add(inGiayRaVienItem);
                    }
                }
                //Mps000478_Tóm tắt y lệnh phẫu thuật thủ thuật và đơn thuốc
                HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__TomTatYLenhPTTTVaDonThuoc = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                menuPrintADO__TomTatYLenhPTTTVaDonThuoc.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                menuPrintADO__TomTatYLenhPTTTVaDonThuoc.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__TOM_TAT_Y_LENH_PTTT_VA_DON_THUOC__MPS000478;
                menuPrintADO__TomTatYLenhPTTTVaDonThuoc.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__TOM_TAT_Y_LENH_PTTT_VA_DON_THUOC__MPS000478;
                menuPrintADOs.Add(menuPrintADO__TomTatYLenhPTTTVaDonThuoc);

                var printTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                HIS.UC.MenuPrint.ADO.MenuPrintInitADO menuPrintInitADO = new HIS.UC.MenuPrint.ADO.MenuPrintInitADO(menuPrintADOs, printTypes);
                //menuPrintInitADO.MinSizeHeight = this.lcibtnSave.MinSize.Height;
                //menuPrintInitADO.MaxSizeHeight = this.lcibtnSave.MaxSize.Height;

                int sizePlusByFontSize = 0, sizePlusByMinSize = 0;
                float fz = ApplicationFontWorker.GetFontSize();

                if (fz >= ApplicationFontConfig.FontSize1300)
                {
                    sizePlusByFontSize = 90;
                }
                else if (fz >= ApplicationFontConfig.FontSize1025)
                {
                    sizePlusByFontSize = 40;
                }
                else if (fz > ApplicationFontConfig.FontSize825)
                {
                    sizePlusByFontSize = 20;
                }

                foreach (var item in menuPrintADOs)
                {
                    var pt = printTypes.FirstOrDefault(o => o.PRINT_TYPE_CODE == item.Tag.ToString());
                    if (pt != null && pt.IS_NO_GROUP == GlobalVariables.CommonNumberTrue)
                    {
                        sizePlusByMinSize += 40;
                    }
                }

                if (this.lciPrintAssignPrescription.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (this.actionType != HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionView)
                    {
                        lciPrintAssignPrescription.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                        lciPrintAssignPrescription.MaxSize = new System.Drawing.Size(440 + sizePlusByFontSize, 40);
                        lciPrintAssignPrescription.MinSize = new System.Drawing.Size(140 + sizePlusByFontSize + sizePlusByMinSize, 40);
                    }
                    menuPrintInitADO.ControlContainer = this.layoutControlPrintAssignPrescription;
                }
                else
                {
                    lciPrintAssignPrescriptionExt.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                    lciPrintAssignPrescriptionExt.MaxSize = new System.Drawing.Size(440 + sizePlusByFontSize, 40);
                    lciPrintAssignPrescriptionExt.MinSize = new System.Drawing.Size(140 + sizePlusByFontSize + sizePlusByMinSize, 40);

                    menuPrintInitADO.ControlContainer = this.layoutControlPrintAssignPrescriptionExt;
                }

                var menuResultADO = menuPrintProcessor.Run(menuPrintInitADO) as MenuPrintResultADO;
                if (menuResultADO == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("menuPrintProcessor run fail. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => menuPrintInitADO), menuPrintInitADO));
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lcibtnSave.Size", lcibtnSave.Size));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lciPrintAssignPrescriptionExt.Size", lciPrintAssignPrescriptionExt.Size) + "____" + Inventec.Common.Logging.LogUtil.TraceData("lciPrintAssignPrescription.Size", lciPrintAssignPrescription.Size));
                Inventec.Common.Logging.LogSystem.Debug("InitMenuToButtonPrint .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm khi báo cho sự kiện nhấn nút in động
        /// Đặt đúng tên OnClickPrintWithPrintTypeCfg
        /// Trường hợp có nhiều nút in cần gen động trong 1 chức năng bổ sung sau, hiện tại chưa hỗ trợ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickPrintWithPrintTypeCfg(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(() => PrintWithPrintTypeCfg(sender, e), !GlobalStore.IsCabinet ? "PrintPresciption" : "PrintMedicalStore");
        }

        private void PrintWithPrintTypeCfg(object sender, EventArgs e) 
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("OnClickPrintWithPrintTypeCfg.1");
                //khi click nhiều lầu nút in sẽ chỉ hiển thị preview 1 lần
                bool isOpenPreview = false;
                foreach (Form item in Application.OpenForms)
                {
                    if (item.Name == "frmSetupPrintPreview")
                    {
                        isOpenPreview = true;
                        break;
                    }
                }
                if (isOpenPreview)
                {
                    return;
                }

                string printTypeCode = "";
                if (sender is DXMenuItem)
                {
                    var bbtnItem = sender as DXMenuItem;
                    printTypeCode = (bbtnItem.Tag ?? "").ToString();
                }
                else if (sender is SimpleButton)
                {
                    var bbtnItem = sender as SimpleButton;
                    printTypeCode = (bbtnItem.Tag ?? "").ToString();
                }
                if (!string.IsNullOrWhiteSpace(HisConfigCFG.MODULELINKS))
                {
                    //currentModule
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();

                    if (moduleData != null)
                    {
                        var allowedModules = HisConfigCFG.MODULELINKS.Split(',');
                             
                        if (allowedModules.Contains(moduleData.ModuleLink))
                        {
                            PrescriptionSavePrintShowHasClickSave(printTypeCode, false, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow);
                        }
                        else
                        {
                            PrescriptionSavePrintShowHasClickSave(printTypeCode, false, null);
                        }
                    }
                }
                else
                {
                    PrescriptionSavePrintShowHasClickSave(printTypeCode, false, null);
                }

                //PrescriptionPrintShowPrintOnly(printTypeCode, false);
                Inventec.Common.Logging.LogSystem.Debug("OnClickPrintWithPrintTypeCfg.2____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printTypeCode), printTypeCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrescriptionPrintShowPrintOnly(string printTypeCode, bool isPrintNow, MPS.ProcessorBase.PrintConfig.PreviewType? previewType = null)
        {
            try
            {
                var IsNotShow = lstConfig.Exists(o => o.IsChecked && o.ID == (int)ConfigADO.RowConfigID.KhongHienThiDonKhongLayODonThuocTH);
                if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008
                    || printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010
                    || printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011)
                {
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, currentTreatmentWithPatientType);

                    PrintTreatmentFinishProcessor printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, LoadServiceReq(treatment.ID), currentModule != null ? currentModule.RoomId : 0);
                    if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010)
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode, isPrintNow);
                    }
                    else if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011)
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000011.PDO.Mps000011PDO.printTypeCode, isPrintNow);
                    }
                    else if ((currentTreatmentWithPatientType.TDL_TREATMENT_TYPE_ID ?? 0) != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000008.PDO.Mps000008PDO.printTypeCode, isPrintNow);
                    }
                }
                else
                {
                    HIS.Desktop.Plugins.Library.PrintPrescription.PrintPrescriptionProcessor printPrescriptionProcessor;
                    if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                    {
                        List<InPatientPresResultSDO> InPatientPresResultSDOForPrints = new List<InPatientPresResultSDO>();
                        InPatientPresResultSDO inPatientPresResultSDO = new InPatientPresResultSDO();
                        if (inPrescriptionResultSDOs != null)
                            InPatientPresResultSDOForPrints.Add(inPrescriptionResultSDOs);
                        else if (this.oldServiceReq != null)
                        {
                            if (this.oldExpMest != null)
                            {
                                inPatientPresResultSDO.ExpMests = new List<HIS_EXP_MEST>();
                                inPatientPresResultSDO.ExpMests.Add(oldExpMest);
                            }

                            if (this.oldServiceReq != null)
                            {
                                inPatientPresResultSDO.ServiceReqs = new List<HIS_SERVICE_REQ>();
                                inPatientPresResultSDO.ServiceReqs.Add(this.oldServiceReq);
                            }

                            if (this.expMestMedicineEditPrints != null)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE, HIS_EXP_MEST_MEDICINE>();
                                inPatientPresResultSDO.Medicines = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MEDICINE>>(this.expMestMedicineEditPrints);
                            }

                            if (this.expMestMaterialEditPrints != null)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL, HIS_EXP_MEST_MATERIAL>();
                                inPatientPresResultSDO.Materials = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MATERIAL>>(this.expMestMaterialEditPrints);
                            }
                            if (this.serviceReqMetys != null && this.serviceReqMetys.Count > 0)
                            {
                                inPatientPresResultSDO.ServiceReqMeties = serviceReqMetys;
                            }
                            if (this.serviceReqMatys != null && this.serviceReqMatys.Count > 0)
                            {
                                inPatientPresResultSDO.ServiceReqMaties = serviceReqMatys;
                            }

                            InPatientPresResultSDOForPrints.Add(inPatientPresResultSDO);
                        }

                        printPrescriptionProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(InPatientPresResultSDOForPrints, IsNotShow, this.currentModule, true);
                        printPrescriptionProcessor.SetOutHospital((currentMediStockNhaThuocSelecteds != null && currentMediStockNhaThuocSelecteds.Count > 0));
                    }
                    else
                    {
                        List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
                        OutPatientPresResultSDO OutPatientPresResultSDO = new OutPatientPresResultSDO();

                        if (outPrescriptionResultSDOs != null)
                            OutPatientPresResultSDOForPrints.Add(outPrescriptionResultSDOs);
                        else if (this.oldServiceReq != null)
                        {
                            if (this.oldExpMest != null)
                            {
                                OutPatientPresResultSDO.ExpMests = new List<HIS_EXP_MEST>();
                                OutPatientPresResultSDO.ExpMests.Add(oldExpMest);
                            }

                            if (this.oldServiceReq != null)
                            {
                                OutPatientPresResultSDO.ServiceReqs = new List<HIS_SERVICE_REQ>();
                                OutPatientPresResultSDO.ServiceReqs.Add(this.oldServiceReq);
                            }

                            if (this.expMestMedicineEditPrints != null)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE, HIS_EXP_MEST_MEDICINE>();
                                OutPatientPresResultSDO.Medicines = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MEDICINE>>(this.expMestMedicineEditPrints);
                            }

                            if (this.expMestMaterialEditPrints != null)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL, HIS_EXP_MEST_MATERIAL>();
                                OutPatientPresResultSDO.Materials = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MATERIAL>>(this.expMestMaterialEditPrints);
                            }
                            if (this.serviceReqMetys != null && this.serviceReqMetys.Count > 0)
                            {
                                OutPatientPresResultSDO.ServiceReqMeties = serviceReqMetys;
                            }
                            if (this.serviceReqMatys != null && this.serviceReqMatys.Count > 0)
                            {
                                OutPatientPresResultSDO.ServiceReqMaties = serviceReqMatys;
                            }

                            OutPatientPresResultSDOForPrints.Add(OutPatientPresResultSDO);
                        }

                        printPrescriptionProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, IsNotShow, this.currentModule, true);
                        printPrescriptionProcessor.SetOutHospital((currentMediStockNhaThuocSelecteds != null && currentMediStockNhaThuocSelecteds.Count > 0));
                    }
                    if (chkPreviewBeforePrint.Checked)
                    {
                        isPrintNow = false;
                    }

                    if (isPrintNow)
                        printPrescriptionProcessor.Print(previewType);
                    else
                        printPrescriptionProcessor.Print(printTypeCode, isPrintNow, previewType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrescriptionSavePrintShowHasClickSave(string printTypeCode, bool isPrintNow, MPS.ProcessorBase.PrintConfig.PreviewType? previewType = null)
        {
            try
            {
                var IsNotShow = lstConfig.Exists(o => o.IsChecked && o.ID == (int)ConfigADO.RowConfigID.KhongHienThiDonKhongLayODonThuocTH);
                Inventec.Common.Logging.LogSystem.Debug("PrescriptionSavePrintShowHasClickSave.1____"
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printTypeCode), printTypeCode)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isPrintNow), isPrintNow)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => previewType), previewType)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => IsNotShow), IsNotShow));
                if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008
                    || printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010
                    || printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011
                    || printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__TOM_TAT_Y_LENH_PTTT_VA_DON_THUOC__MPS000478)
                {
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, currentTreatmentWithPatientType);

                    PrintTreatmentFinishProcessor printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, LoadServiceReq(treatment.ID), currentModule != null ? currentModule.RoomId : 0, previewType);
                    if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__TOM_TAT_Y_LENH_PTTT_VA_DON_THUOC__MPS000478)
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000478.PDO.Mps000478PDO.printTypeCode, isPrintNow);
                    }
                    else if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010)
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode, isPrintNow);
                    }
                    else if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011)
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000011.PDO.Mps000011PDO.printTypeCode, isPrintNow);
                    }
                    else if ((currentTreatmentWithPatientType.TDL_TREATMENT_TYPE_ID ?? 0) != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000008.PDO.Mps000008PDO.printTypeCode, isPrintNow);
                    }
                }
                else
                {
                    this.lstMatePrintMps494 = new List<HIS_EXP_MEST_MATERIAL>();
                    HIS.Desktop.Plugins.Library.PrintPrescription.PrintPrescriptionProcessor printPrescriptionProcessor;
                    if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                    {
                        //List<InPatientPresResultSDO> InPatientPresResultSDOForPrints = new List<InPatientPresResultSDO>();
                        //InPatientPresResultSDOForPrints.Add(inPrescriptionResultSDOs);
                        //printPrescriptionProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(InPatientPresResultSDOForPrints, this.currentModule);
                        List<InPatientPresResultSDO> InPatientPresResultSDOForPrints = new List<InPatientPresResultSDO>();
                        InPatientPresResultSDO inPatientPresResultSDO = new InPatientPresResultSDO();
                        if (inPrescriptionResultSDOs != null)
                        {
                            if(inPrescriptionResultSDOs.Materials != null && inPrescriptionResultSDOs.Materials.Count > 0)
                                lstMatePrintMps494.AddRange(inPrescriptionResultSDOs.Materials);
                            InPatientPresResultSDOForPrints.Add(inPrescriptionResultSDOs);
                        }
                        else if (this.oldServiceReq != null)
                        {
                            if (this.oldExpMest != null && this.oldExpMest.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                            {
                                inPatientPresResultSDO.ExpMests = new List<HIS_EXP_MEST>();
                                inPatientPresResultSDO.ExpMests.Add(oldExpMest);
                            }

                            if (this.oldServiceReq != null)
                            {
                                inPatientPresResultSDO.ServiceReqs = new List<HIS_SERVICE_REQ>();
                                inPatientPresResultSDO.ServiceReqs.Add(this.oldServiceReq);
                            }

                            if (this.expMestMedicineEditPrints != null)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE, HIS_EXP_MEST_MEDICINE>();
                                inPatientPresResultSDO.Medicines = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MEDICINE>>(this.expMestMedicineEditPrints);
                            }

                            if (this.expMestMaterialEditPrints != null)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL, HIS_EXP_MEST_MATERIAL>();
                                inPatientPresResultSDO.Materials = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MATERIAL>>(this.expMestMaterialEditPrints);
                                lstMatePrintMps494.AddRange(inPatientPresResultSDO.Materials);
                            }
                            if (this.serviceReqMetys != null && this.serviceReqMetys.Count > 0)
                            {
                                inPatientPresResultSDO.ServiceReqMeties = serviceReqMetys;
                            }
                            if (this.serviceReqMatys != null && this.serviceReqMatys.Count > 0)
                            {
                                inPatientPresResultSDO.ServiceReqMaties = serviceReqMatys;
                            }

                            InPatientPresResultSDOForPrints.Add(inPatientPresResultSDO);
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => InPatientPresResultSDOForPrints), InPatientPresResultSDOForPrints));
                        printPrescriptionProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(InPatientPresResultSDOForPrints, IsNotShow, this.currentModule, true);
                        printPrescriptionProcessor.SetOutHospital((currentMediStockNhaThuocSelecteds != null && currentMediStockNhaThuocSelecteds.Count > 0));
                    }
                    else
                    {
                        List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
                        OutPatientPresResultSDO OutPatientPresResultSDO = new OutPatientPresResultSDO();
                        string savePrintMpsDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS.Desktop.Plugins.AssignPrescriptionPK.Config.HisConfigCFG.SAVE_PRINT_MPS_DEFAULT);

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => savePrintMpsDefault), savePrintMpsDefault)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printTypeCode), printTypeCode)
                            + "((this.expMestPrints != null && this.expMestPrints.Count > 0) || (this.serviceReqPrints != null && this.serviceReqPrints.Count > 0)) =" + ((this.expMestPrints != null && this.expMestPrints.Count > 0) || (this.serviceReqPrints != null && this.serviceReqPrints.Count > 0)));

                        if ((String.IsNullOrEmpty(printTypeCode) || printTypeCode == "Mps000234")
                            //&& ((this.expMestPrints != null && this.expMestPrints.Count > 0) || (this.serviceReqPrints != null && this.serviceReqPrints.Count > 0))
                            && !GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                        {
                            List<HIS_EXP_MEST> expMestPrintPlus = new List<HIS_EXP_MEST>();
                            List<HIS_SERVICE_REQ> serviceReqPrintPlus = new List<HIS_SERVICE_REQ>();
                            List<HIS_EXP_MEST_MEDICINE> expMestMedicinePrintPlus = new List<HIS_EXP_MEST_MEDICINE>();
                            List<HIS_EXP_MEST_MATERIAL> expMestMaterialPrintPlus = new List<HIS_EXP_MEST_MATERIAL>();

                            if (this.expMestPrints != null && this.expMestPrints.Count > 0)
                            {
                                if (this.outPrescriptionResultSDOs != null && this.outPrescriptionResultSDOs.ExpMests != null && this.outPrescriptionResultSDOs.ExpMests.Count > 0)
                                {
                                    expMestPrintPlus = expMestPrints.Where(o => !this.outPrescriptionResultSDOs.ExpMests.Exists(k => k.ID == o.ID)).ToList();
                                    expMestPrintPlus.AddRange(this.outPrescriptionResultSDOs.ExpMests);

                                    if (this.expMestMedicinePrints != null && this.expMestMedicinePrints.Count > 0)
                                    {
                                        expMestMedicinePrintPlus = this.expMestMedicinePrints.Where(o => !this.outPrescriptionResultSDOs.ExpMests.Exists(k => k.ID == o.EXP_MEST_ID)).ToList();
                                    }

                                    if (this.expMestMaterialPrints != null && this.expMestMaterialPrints.Count > 0)
                                    {
                                        expMestMaterialPrintPlus = this.expMestMaterialPrints.Where(o => !this.outPrescriptionResultSDOs.ExpMests.Exists(k => k.ID == o.EXP_MEST_ID)).ToList();
                                    }
                                }
                                else
                                {
                                    expMestPrintPlus.AddRange(expMestPrints);
                                    if (this.expMestMedicinePrints != null && this.expMestMedicinePrints.Count > 0)
                                    {
                                        expMestMedicinePrintPlus.AddRange(this.expMestMedicinePrints);
                                    }

                                    if (this.expMestMaterialPrints != null && this.expMestMaterialPrints.Count > 0)
                                    {
                                        expMestMaterialPrintPlus.AddRange(this.expMestMaterialPrints);
                                    }
                                }
                            }
                            else if (this.outPrescriptionResultSDOs != null && this.outPrescriptionResultSDOs.ExpMests != null && this.outPrescriptionResultSDOs.ExpMests.Count > 0)
                            {
                                expMestPrintPlus.AddRange(this.outPrescriptionResultSDOs.ExpMests);
                            }

                            if (this.outPrescriptionResultSDOs.Medicines != null && this.outPrescriptionResultSDOs.Medicines.Count > 0)
                            {
                                expMestMedicinePrintPlus.AddRange(this.outPrescriptionResultSDOs.Medicines);
                            }
                            if (this.outPrescriptionResultSDOs.Materials != null && this.outPrescriptionResultSDOs.Materials.Count > 0)
                            {
                                expMestMaterialPrintPlus.AddRange(this.outPrescriptionResultSDOs.Materials);
                            }

                            if (this.serviceReqPrints != null && this.serviceReqPrints.Count > 0)
                            {
                                if (this.outPrescriptionResultSDOs != null && this.outPrescriptionResultSDOs.ServiceReqs != null && this.outPrescriptionResultSDOs.ServiceReqs.Count > 0)
                                {
                                    serviceReqPrintPlus = this.serviceReqPrints.Where(o => !this.outPrescriptionResultSDOs.ServiceReqs.Exists(k => k.ID == o.ID)).ToList();
                                    serviceReqPrintPlus.AddRange(this.outPrescriptionResultSDOs.ServiceReqs);
                                }
                                else
                                {
                                    serviceReqPrintPlus.AddRange(this.serviceReqPrints);
                                }
                            }
                            else if (this.outPrescriptionResultSDOs != null && this.outPrescriptionResultSDOs.ServiceReqs != null && this.outPrescriptionResultSDOs.ServiceReqs.Count > 0)
                            {
                                serviceReqPrintPlus.AddRange(this.outPrescriptionResultSDOs.ServiceReqs);
                            }

                            if (this.outPrescriptionResultSDOs != null && this.outPrescriptionResultSDOs.ServiceReqMaties != null && this.outPrescriptionResultSDOs.ServiceReqMaties.Count > 0)
                            {
                                OutPatientPresResultSDO.ServiceReqMaties = this.outPrescriptionResultSDOs.ServiceReqMaties;
                            }
                            if (this.outPrescriptionResultSDOs != null && this.outPrescriptionResultSDOs.ServiceReqMeties != null && this.outPrescriptionResultSDOs.ServiceReqMeties.Count > 0)
                            {
                                OutPatientPresResultSDO.ServiceReqMeties = this.outPrescriptionResultSDOs.ServiceReqMeties;
                            }

                            if (expMestPrintPlus != null && expMestPrintPlus.Count > 0)
                            {
                                expMestPrintPlus = expMestPrintPlus.Where(o => o.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT).ToList();
                            }

                            OutPatientPresResultSDO.ExpMests = expMestPrintPlus;
                            OutPatientPresResultSDO.Medicines = expMestMedicinePrintPlus;
                            OutPatientPresResultSDO.Materials = expMestMaterialPrintPlus;
                            if (OutPatientPresResultSDO.Materials != null && OutPatientPresResultSDO.Materials.Count > 0)
                                lstMatePrintMps494.AddRange(OutPatientPresResultSDO.Materials);
                            OutPatientPresResultSDO.ServiceReqs = serviceReqPrintPlus;
                            OutPatientPresResultSDOForPrints.Add(OutPatientPresResultSDO);
                        }
                        else
                        {
                            if (outPrescriptionResultSDOs != null)
                            {
                                if (outPrescriptionResultSDOs.Materials != null && outPrescriptionResultSDOs.Materials.Count > 0)
                                    lstMatePrintMps494.AddRange(outPrescriptionResultSDOs.Materials);
                                OutPatientPresResultSDOForPrints.Add(outPrescriptionResultSDOs);
                            }
                            else if (this.oldServiceReq != null)
                            {
                                if (this.oldExpMest != null)
                                {
                                    OutPatientPresResultSDO.ExpMests = new List<HIS_EXP_MEST>();
                                    OutPatientPresResultSDO.ExpMests.Add(oldExpMest);
                                }

                                if (this.oldServiceReq != null)
                                {
                                    OutPatientPresResultSDO.ServiceReqs = new List<HIS_SERVICE_REQ>();
                                    OutPatientPresResultSDO.ServiceReqs.Add(this.oldServiceReq);
                                }

                                if (this.expMestMedicineEditPrints != null)
                                {
                                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE, HIS_EXP_MEST_MEDICINE>();
                                    OutPatientPresResultSDO.Medicines = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MEDICINE>>(this.expMestMedicineEditPrints);
                                }

                                if (this.expMestMaterialEditPrints != null)
                                {
                                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL, HIS_EXP_MEST_MATERIAL>();
                                    OutPatientPresResultSDO.Materials = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MATERIAL>>(this.expMestMaterialEditPrints);
                                    if (OutPatientPresResultSDO.Materials != null && OutPatientPresResultSDO.Materials.Count > 0)
                                        lstMatePrintMps494.AddRange(OutPatientPresResultSDO.Materials);
                                }
                                if (this.serviceReqMetys != null && this.serviceReqMetys.Count > 0)
                                {
                                    OutPatientPresResultSDO.ServiceReqMeties = serviceReqMetys;
                                }
                                if (this.serviceReqMatys != null && this.serviceReqMatys.Count > 0)
                                {
                                    OutPatientPresResultSDO.ServiceReqMaties = serviceReqMatys;
                                }

                                OutPatientPresResultSDOForPrints.Add(OutPatientPresResultSDO);  
                            }
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => OutPatientPresResultSDOForPrints), OutPatientPresResultSDOForPrints));

                        printPrescriptionProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, IsNotShow, this.currentModule, true);
                        printPrescriptionProcessor.SetOutHospital((currentMediStockNhaThuocSelecteds != null && currentMediStockNhaThuocSelecteds.Count > 0));
                    }
                    this.isPrintNow = isPrintNow;
                    if (printTypeCode == "Mps000494")
                    {
                        Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                        richEditorMain.RunPrintTemplate("Mps000494", DelegateRunPrinter);
                    }
                    else if (isPrintNow)
                        printPrescriptionProcessor.Print(previewType);
                    else
                        printPrescriptionProcessor.Print(printTypeCode, isPrintNow, previewType);
                }
                Inventec.Common.Logging.LogSystem.Debug("PrescriptionSavePrintShowHasClickSave.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        bool isPrintNow = false;
        List<HIS_EXP_MEST_MATERIAL> lstMatePrintMps494 = new List<HIS_EXP_MEST_MATERIAL>();
        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case "Mps000494":
                        return PrintMps494(printCode, fileName, ref result, isPrintNow, currentModule.RoomId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool PrintMps494(string printCode, string fileName, ref bool result, bool printNow, long roomId)
        {
            try
            {
                if (lstMatePrintMps494 == null || lstMatePrintMps494.Count == 0)
                {
                    XtraMessageBox.Show("Không có vật tư tái sử dụng hoặc vật tư tái sử dụng đã hết số lần tái sử dụng.");
                    return false;
                }
                var expmestMate = lstMatePrintMps494.Where(o => !string.IsNullOrEmpty(o.SERIAL_NUMBER) && o.REMAIN_REUSE_COUNT != null).ToList();
                if (expmestMate == null || expmestMate.Count == 0)
                {
                    XtraMessageBox.Show("Không có vật tư tái sử dụng hoặc vật tư tái sử dụng đã hết số lần tái sử dụng.");
                    return false;
                }
                List<HIS_MATERIAL> mate = new List<HIS_MATERIAL>();
                var materialIdList = expmestMate.Select(p => p.MATERIAL_ID ?? 0).ToList();
                if (materialIdList != null && materialIdList.Count > 0)
                {
                    MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                    materialFilter.IDs = materialIdList;
                    mate = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, null);
                }
                List<MPS.Processor.Mps000494.PDO.SerialADO> lstSend = new List<MPS.Processor.Mps000494.PDO.SerialADO>();
                foreach (var m in expmestMate)
                {
                    MPS.Processor.Mps000494.PDO.SerialADO ado = new MPS.Processor.Mps000494.PDO.SerialADO();
                    var te = mate.FirstOrDefault(o => o.ID == m.MATERIAL_ID);
                    if (te != null)
                    {
                        ado.NEXT_REUSABLE_NUMBER = ((te.MAX_REUSE_COUNT ?? 0) - (m.REMAIN_REUSE_COUNT ?? 0)+ 2).ToString();
                        ado.SIZE = te.MATERIAL_SIZE;
                        if (Int32.Parse(ado.NEXT_REUSABLE_NUMBER) > (te.MAX_REUSE_COUNT ?? 0))
                            continue;
                    }
                    else
                        continue;
                    ado.SERIAL_NUMBER = m.SERIAL_NUMBER;
                    lstSend.Add(ado);
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstSend), lstSend));
                if (lstSend == null || lstSend.Count == 0)
                {
                    XtraMessageBox.Show("Không có vật tư tái sử dụng hoặc vật tư tái sử dụng đã hết số lần tái sử dụng.");
                    return false;
                }
                MPS.Processor.Mps000494.PDO.Mps000494PDO rdo = new MPS.Processor.Mps000494.PDO.Mps000494PDO(lstSend);

                result = RunPrint(printCode, fileName, rdo, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, printNow, roomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void EventLogPrint()
        {
            try
            {
                string message = "In tem vật tư tái sử dụng. Mã in : Mps000494" + "  TREATMENT_CODE: " + currentTreatmentWithPatientType.TREATMENT_CODE + "  Thời gian in: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + "  Người in: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        bool RunPrint(string printTypeCode, string fileName, object data, Inventec.Common.FlexCelPrint.DelegateEventLog EventLogPrint, bool result, bool _printNow, long? roomId)
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
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName, EventLogPrint));
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName, EventLogPrint));
                }
                else
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(currentTreatmentWithPatientType.TREATMENT_CODE, printTypeCode, roomId);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName, EventLogPrint) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return result;
        }

    }
}
