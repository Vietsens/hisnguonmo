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
using EMR_MAIN;
using EMR_MAIN.DATABASE.BenhAn;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.PrintOtherForm;
using HIS.Desktop.Plugins.Library.PrintOtherForm.Base;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Base;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Config;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.Processor.Mps000036.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    public partial class SurgServiceReqExecuteControl : UserControlBase
    {

        public enum PrintTypeSurg
        {
            PHIEU_THU_THUAT_PHAU_THUAT,
            GIAY_CAM_DOAN_CHAP_NHAN_PHAU_THUAT_THU_THUAT_VA_GAY_ME_HOI_SUC,
            IN_PHIEU_YEU_CAU_PHAU_THUAT,
            CACH_THUC_PHAU_THUAT,
            BIEU_MAU_KHAC_PHIEU_PTTT,
            GIAY_CHUNG_NHAN_PT,
            PHIEU_SU_DUNG_VAT_TU_GIA_TRI_LON,
            PHIEU_GAY_ME_HOI_SUC,
            PHIEU_THANH_TOAN_PT_TT,
            PHIEU_GAY_ME_TRUOC_MO,
            PHIEU_KE_KHAI_THUOC_VAT_TU,
            PHIEU_PHAU_THUAT_DUC_TTT,
            PHIEU_PHAU_THUAT_MONG,
            PHIEU_PHAU_THUAT_TAI_TAO_LE_QUAN,
            PHIEU_PHAU_THUAT_SUP_MI,
            PHIEU_PHAU_THUAT_GLOCOM,
            PHIEU_PHAU_THUAT_LASER_YAG,
            PHIEU_PHAU_THUAT_MONG_MAT,
            BANG_KIEM_AN_TOAN_PHAU_THUAT,
            BANG_GAY_ME_HOI_SUC,
            PHIEU_PHAU_THUAT,
            PHIEU_THU_THUAT,
            PHIEU_KIEM_KE_DUNG_CU_VTYT,
            PHIEU_KQ_DT_CAN_THIEP_DMV,
            PHIEU_VAT_TU_TSD
        }

        private async Task InitPrintSurgService()
        {
            DXPopupMenu menu = new DXPopupMenu();

            DXMenuItem menuItem18 = new DXMenuItem("Phiếu vật tư tái sử dụng", new EventHandler(onClickFormType));
            menuItem18.Tag = PrintTypeSurg.PHIEU_VAT_TU_TSD;
            menu.Items.Add(menuItem18);

            DXMenuItem menuItem17 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.PhieuKiemKeDungCuVtyt", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuItem17.Tag = PrintTypeSurg.PHIEU_KIEM_KE_DUNG_CU_VTYT;
            menu.Items.Add(menuItem17);

            DXMenuItem menuItem0 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.PhieuThanhToanThuThuatPhauThuat", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuItem0.Tag = PrintTypeSurg.PHIEU_THANH_TOAN_PT_TT;
            menu.Items.Add(menuItem0);

            DXMenuItem menuItem1 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.ThuThuatPhauThuat", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuItem1.Tag = PrintTypeSurg.PHIEU_THU_THUAT_PHAU_THUAT;
            menu.Items.Add(menuItem1);
            DXMenuItem menuItem2 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.GiayCamDoan", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuItem2.Tag = PrintTypeSurg.GIAY_CAM_DOAN_CHAP_NHAN_PHAU_THUAT_THU_THUAT_VA_GAY_ME_HOI_SUC;
            menu.Items.Add(menuItem2);

            DXMenuItem menuItem3 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.CachThucPTTT", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuItem3.Tag = PrintTypeSurg.CACH_THUC_PHAU_THUAT;
            menu.Items.Add(menuItem3);

            //Bieu mau khac
            DXSubMenuItem itemBieuMauKhac = new DXSubMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.BieuMauKhac", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()));

            DXMenuItem menuItem4 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.BieuMauKhacPhieuPTTT", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuItem4.Tag = PrintTypeSurg.BIEU_MAU_KHAC_PHIEU_PTTT;
            itemBieuMauKhac.Items.Add(menuItem4);

            DXMenuItem menuItem8 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.PhieuKhamGayMeTruocMo", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuItem8.Tag = PrintTypeSurg.PHIEU_GAY_ME_TRUOC_MO;
            itemBieuMauKhac.Items.Add(menuItem8);

            DXMenuItem menuBangAnToan = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.BangKiemAnToanPTTT", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuBangAnToan.Tag = PrintTypeSurg.BANG_KIEM_AN_TOAN_PHAU_THUAT;
            itemBieuMauKhac.Items.Add(menuBangAnToan);

            DXMenuItem menuBangGayMeHoiSuc = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.BangGayMeHoiSuc", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuBangGayMeHoiSuc.Tag = PrintTypeSurg.BANG_GAY_ME_HOI_SUC;
            itemBieuMauKhac.Items.Add(menuBangGayMeHoiSuc);

            DXMenuItem menuPhieuPhauThuat = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.PhieuPhauThuat", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuPhieuPhauThuat.Tag = PrintTypeSurg.PHIEU_PHAU_THUAT;
            itemBieuMauKhac.Items.Add(menuPhieuPhauThuat);

            DXMenuItem menuPhieuThuThuat = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.PhieuThuThuat", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuPhieuThuThuat.Tag = PrintTypeSurg.PHIEU_THU_THUAT;
            itemBieuMauKhac.Items.Add(menuPhieuThuThuat);

            menu.Items.Add(itemBieuMauKhac);

            //Het bm khac

            if (this.serviceReq != null && (this.serviceReq.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || this.serviceReq.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT))
            {
                DXMenuItem menuItem5 = new DXMenuItem("Giấy chứng nhận phẫu thuật, thủ thuật", new EventHandler(onClickFormType));
                menuItem5.Tag = PrintTypeSurg.GIAY_CHUNG_NHAN_PT;
                menu.Items.Add(menuItem5);

            }

            DXMenuItem menuItem6 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.PhieuSuDungVatTuGiaTriLon", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuItem6.Tag = PrintTypeSurg.PHIEU_SU_DUNG_VAT_TU_GIA_TRI_LON;
            menu.Items.Add(menuItem6);

            DXMenuItem menuItem7 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.PhieuGayMeHoiSuc", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuItem7.Tag = PrintTypeSurg.PHIEU_GAY_ME_HOI_SUC;
            menu.Items.Add(menuItem7);

            //DXMenuItem menuItem8 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.PhieuKhamGayMeTruocMo", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            //menuItem8.Tag = PrintTypeSurg.PHIEU_GAY_ME_TRUOC_MO;
            //menu.Items.Add(menuItem8);

            DXMenuItem menuItem9 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.PhieuKeKhaiThuocVatTu", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
            menuItem9.Tag = PrintTypeSurg.PHIEU_KE_KHAI_THUOC_VAT_TU;
            menu.Items.Add(menuItem9);

            //a. Sửa nút "In ấn", bổ sung các chức năng in các phiếu:
            //- "Phiếu tường trình phẫu thuật đục thủy tinh thể" (Mps000390)
            //- "Phiếu tường trình phẫu thuật mộng" (Mps000391)
            //- "Phiếu tường trình phẫu thuật tái tạo lệ quản" (Mps000392)
            //- "Phiếu tường trình phẫu thuật sụp mí" (Mps000393)
            //- "Phiếu tường trình phẫu thuật Glocom" (Mps000394)
            //- "Phiếu tường trình thủ thuật mở bao sau bằng Laser Yag" (Mps000395)
            //- "Phiếu tường trình thủ thuật mở bao sau bằng mống mắt" (Mps000396)
            //- Lưu ý:
            //+ Căn cứ vào loại phẫu thuật mắt (căn cứ vào EYE_SURGRY_DESC_ID trong HIS_SERE_SERV_PTTT để lấy ra HIS_EYE_SURGRY_DESC và kiểm tra dữ liệu LOAI_PT_MAT), để chỉ hiển thị ra các mẫu tương ứng. Cụ thể:
            //LOAI_PT_MAT = 1, chỉ hiển thị: Mps000394
            //LOAI_PT_MAT = 2, chỉ hiển thị: Mps000391
            //LOAI_PT_MAT = 3, chỉ hiển thị: Mps000390
            //LOAI_PT_MAT = 4, chỉ hiển thị: Mps000392
            //LOAI_PT_MAT = 5, chỉ hiển thị: Mps000393
            //LOAI_PT_MAT = 6, chỉ hiển thị: Mps000395
            //LOAI_PT_MAT = 7, chỉ hiển thị: Mps000396
            //+ Input truyền vào các MPS gồm dữ liệu các bảng sau (tương ứng với thông tin PTTT mà người dùng đang xử lý):
            //HIS_TREATMENT, HIS_SERE_SERV_PTTT, HIS_EYE_SURGRY_DESC

            if (this.currentEyeSurgDesc != null && this.currentEyeSurgDesc.LOAI_PT_MAT > 0)
            {
                if (this.currentEyeSurgDesc.LOAI_PT_MAT == IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__PT_TTT)
                {
                    DXMenuItem menuItem10 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.Mps000390", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
                    menuItem10.Tag = PrintTypeSurg.PHIEU_PHAU_THUAT_DUC_TTT;
                    menu.Items.Add(menuItem10);
                }

                else if (this.currentEyeSurgDesc.LOAI_PT_MAT == IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__PT_MONG)
                {
                    DXMenuItem menuItem11 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.Mps000391", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
                    menuItem11.Tag = PrintTypeSurg.PHIEU_PHAU_THUAT_MONG;
                    menu.Items.Add(menuItem11);
                }

                else if (this.currentEyeSurgDesc.LOAI_PT_MAT == IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__PT_TAI_TAO_LE_QUAN)
                {
                    DXMenuItem menuItem12 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.Mps000392", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
                    menuItem12.Tag = PrintTypeSurg.PHIEU_PHAU_THUAT_TAI_TAO_LE_QUAN;
                    menu.Items.Add(menuItem12);
                }

                else if (this.currentEyeSurgDesc.LOAI_PT_MAT == IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__PT_SUP_MI)
                {
                    DXMenuItem menuItem13 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.Mps000393", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
                    menuItem13.Tag = PrintTypeSurg.PHIEU_PHAU_THUAT_SUP_MI;
                    menu.Items.Add(menuItem13);
                }

                else if (this.currentEyeSurgDesc.LOAI_PT_MAT == IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__GLOCOM)
                {
                    DXMenuItem menuItem14 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.Mps000394", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
                    menuItem14.Tag = PrintTypeSurg.PHIEU_PHAU_THUAT_GLOCOM;
                    menu.Items.Add(menuItem14);
                }

                else if (this.currentEyeSurgDesc.LOAI_PT_MAT == IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__TT_LASER_YAG)
                {
                    DXMenuItem menuItem15 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.Mps000395", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
                    menuItem15.Tag = PrintTypeSurg.PHIEU_PHAU_THUAT_LASER_YAG;
                    menu.Items.Add(menuItem15);
                }
                else if (this.currentEyeSurgDesc.LOAI_PT_MAT == IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__TT_MONG_MAT)
                {
                    DXMenuItem menuItem16 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.Print.Mps000396", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickFormType));
                    menuItem16.Tag = PrintTypeSurg.PHIEU_PHAU_THUAT_MONG_MAT;
                    menu.Items.Add(menuItem16);
                }
            }
            DXMenuItem item493 = new DXMenuItem("Phiếu kết quả điều trị can thiệp động mạch vành", new EventHandler(onClickFormType));
            item493.Tag = PrintTypeSurg.PHIEU_KQ_DT_CAN_THIEP_DMV;
            menu.Items.Add(item493);

            btnPrint.DropDownControl = menu;
        }

        private void onClickFormType(object sender, EventArgs e)
        {
            try
            {
                IsActionPrint = true;
                DXMenuItem item = sender as DXMenuItem;
                PrintTypeSurg type = (PrintTypeSurg)(item.Tag);
                LogTheadInSessionInfo(() => PrintProcess(type), string.Format("{0}{1}", "ClickFormType", type));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintProcess(PrintTypeSurg printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeSurg.PHIEU_GAY_ME_HOI_SUC:
                        btnPhieuSoKetTruocMoClick();
                        break;
                    case PrintTypeSurg.IN_PHIEU_YEU_CAU_PHAU_THUAT:
                        ProcessPrintMPS000036();
                        break;
                    case PrintTypeSurg.GIAY_CAM_DOAN_CHAP_NHAN_PHAU_THUAT_THU_THUAT_VA_GAY_ME_HOI_SUC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_GIAY_CAM_DOAN_CHAP_NHAN_pHAU_THUAT_THU_THUAT_VA_GAY_ME_HOI_SUC__MPS000035, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.PHIEU_THU_THUAT_PHAU_THUAT:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT_THU_THUAT__MPS000033, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.CACH_THUC_PHAU_THUAT:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__CACH_THUC_PHAU_THUAT_THU_THUAT__MPS000097, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.BIEU_MAU_KHAC_PHIEU_PTTT:
                        LoadBieuMauKhacPhieuPTTT();
                        break;
                    case PrintTypeSurg.GIAY_CHUNG_NHAN_PT:
                        LoadGiayChungNhanPhauThuatThuThuat();
                        break;
                    case PrintTypeSurg.PHIEU_SU_DUNG_VAT_TU_GIA_TRI_LON:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_SU_DUNG_VAT_TU_GIA_TRI_LON__MPS000311, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.PHIEU_THANH_TOAN_PT_TT:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_THANH_TOAN_PT_TT__MPS000324, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.PHIEU_GAY_ME_TRUOC_MO:
                        LoadBieuMauKhacPhieuGayMeTruocMo();
                        break;
                    case PrintTypeSurg.PHIEU_KE_KHAI_THUOC_VAT_TU:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_KE_KHAI_THUOC_VATU__MPS000338, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.PHIEU_PHAU_THUAT_DUC_TTT:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PT_DUC_TTT__Mps000390, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.PHIEU_PHAU_THUAT_MONG:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PT_MONG__Mps000391, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.PHIEU_PHAU_THUAT_TAI_TAO_LE_QUAN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PT_TAI_TAO_LE_QUAN__Mps000392, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.PHIEU_PHAU_THUAT_SUP_MI:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__SUP_MI__Mps000393, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.PHIEU_PHAU_THUAT_GLOCOM:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GLOCOM__Mps000394, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.PHIEU_PHAU_THUAT_LASER_YAG:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__LASER_YAG__Mps000395, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.PHIEU_PHAU_THUAT_MONG_MAT:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__MONG_MAT__Mps000396, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.BANG_KIEM_AN_TOAN_PHAU_THUAT:
                        LoadBieuMauKhacBangKiemAnToanPTTT();
                        break;
                    case PrintTypeSurg.BANG_GAY_ME_HOI_SUC:
                        LoadBangGayMeHoiSuc();
                        break;
                    case PrintTypeSurg.PHIEU_PHAU_THUAT:
                        LoadPhieuPhauThuat();
                        break;
                    case PrintTypeSurg.PHIEU_THU_THUAT:
                        LoadPhieuThuThuat();
                        break;
                    case PrintTypeSurg.PHIEU_KIEM_KE_DUNG_CU_VTYT:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__DUNG_CU_VTYT__Mps000415, DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.PHIEU_KQ_DT_CAN_THIEP_DMV:
                        richEditorMain.RunPrintTemplate("Mps000493", DelegateRunPrinterSurg);
                        break;
                    case PrintTypeSurg.PHIEU_VAT_TU_TSD:
                        richEditorMain.RunPrintTemplate("Mps000495", DelegateRunPrinterSurg);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrintMPS000036()
        {
            try
            {
                //PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036

                if (serviceReq != null)
                {
                    ThreadChiDinhDichVuADO data = new ThreadChiDinhDichVuADO(this.serviceReq);
                    CreateThreadLoadDataForService(data);

                    HisServiceReqListResultSDO HisServiceReqSDO = new HisServiceReqListResultSDO();
                    HisServiceReqSDO.SereServs = data.listVHisSereServ;
                    HisServiceReqSDO.ServiceReqs = new List<V_HIS_SERVICE_REQ>() { this.serviceReq };
                    HisServiceReqSDO.SereServBills = data.ListSereServBill;
                    HisServiceReqSDO.SereServDeposits = data.ListSereServDeposit;

                    List<V_HIS_BED_LOG> listBedLogs = new List<V_HIS_BED_LOG>();

                    CommonParam param = new CommonParam();
                    HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                    bedLogFilter.TREATMENT_ID = serviceReq.TREATMENT_ID;
                    var resultBedlog = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
                    if (resultBedlog != null)
                    {
                        listBedLogs = resultBedlog;
                    }

                    HisTreatmentWithPatientTypeInfoSDO HisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(HisTreatment, data.hisTreatment);

                    if (data.vHisPatientTypeAlter != null)
                    {
                        HisTreatment.PATIENT_TYPE_CODE = data.vHisPatientTypeAlter.PATIENT_TYPE_CODE;
                        HisTreatment.HEIN_CARD_FROM_TIME = data.vHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                        HisTreatment.HEIN_CARD_NUMBER = data.vHisPatientTypeAlter.HEIN_CARD_NUMBER;
                        HisTreatment.HEIN_CARD_TO_TIME = data.vHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                        HisTreatment.HEIN_MEDI_ORG_CODE = data.vHisPatientTypeAlter.HEIN_MEDI_ORG_CODE;
                        HisTreatment.LEVEL_CODE = data.vHisPatientTypeAlter.LEVEL_CODE;
                        HisTreatment.RIGHT_ROUTE_CODE = data.vHisPatientTypeAlter.RIGHT_ROUTE_CODE;
                        HisTreatment.RIGHT_ROUTE_TYPE_CODE = data.vHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                        HisTreatment.TREATMENT_TYPE_CODE = data.vHisPatientTypeAlter.TREATMENT_TYPE_CODE;
                        HisTreatment.HEIN_CARD_ADDRESS = data.vHisPatientTypeAlter.ADDRESS;
                    }

                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, HisTreatment, listBedLogs, Module != null ? Module.RoomId : 0);
                    PrintServiceReqProcessor.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPhieuSoKetTruocMoClick()
        {
            try
            {
                if (vhisTreatment != null)
                {
                    #region --- Load
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientViewFilter _patientFilter = new MOS.Filter.HisPatientViewFilter();
                    _patientFilter.ID = this.vhisTreatment.PATIENT_ID;
                    var currentPatient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/getView", ApiConsumers.MosConsumer, _patientFilter, param);
                    if (currentPatient == null || currentPatient.Count == 0)
                        throw new NullReferenceException("Khong lay duoc V_HIS_PATIENT bang ID" + this.vhisTreatment.PATIENT_ID);
                    V_HIS_PATIENT _Patient = currentPatient.FirstOrDefault();

                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = this.vhisTreatment.ID;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    V_HIS_PATIENT_TYPE_ALTER _PatientTypeAlter = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>("/api/HisPatientTypeAlter/GetApplied", ApiConsumers.MosConsumer, filter, param);

                    HisDhstFilter _dhstFilter = new HisDhstFilter();
                    _dhstFilter.TREATMENT_ID = this.vhisTreatment.ID;
                    _dhstFilter.ORDER_DIRECTION = "DESC";
                    _dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
                    var currentDhst = new BackendAdapter(param).Get<List<HIS_DHST>>("/api/HisDhst/Get", ApiConsumers.MosConsumer, _dhstFilter, param);
                    HIS_DHST _DHST = new HIS_DHST();
                    if (currentDhst != null && currentDhst.Count > 0)
                    {
                        _DHST = currentDhst.FirstOrDefault();
                    }

                    MOS.Filter.HisBabyViewFilter _babyFIlter = new HisBabyViewFilter();
                    _babyFIlter.TREATMENT_ID = this.vhisTreatment.ID;
                    var currentBaby = new BackendAdapter(param).Get<List<V_HIS_BABY>>("/api/HisBaby/GetView", ApiConsumers.MosConsumer, _babyFIlter, param);
                    V_HIS_BABY _Baby = new V_HIS_BABY();
                    if (currentBaby != null && currentBaby.Count > 0)
                    {
                        _Baby = currentBaby.FirstOrDefault();
                    }

                    MOS.Filter.HisDepartmentTranViewFilter _departmentTranFilter = new HisDepartmentTranViewFilter();
                    _departmentTranFilter.TREATMENT_ID = this.vhisTreatment.ID;
                    _departmentTranFilter.ORDER_DIRECTION = "ASC";
                    _departmentTranFilter.ORDER_FIELD = "CREATE_TIME";
                    var _DepartmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("/api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, _departmentTranFilter, param);
                    if (_DepartmentTrans != null && _DepartmentTrans.Count > 0)
                    {
                        _DepartmentTrans = _DepartmentTrans.Where(p => p.DEPARTMENT_IN_TIME != null).ToList();
                    }

                    MOS.Filter.HisServiceReqViewFilter _reqFilter = new HisServiceReqViewFilter();
                    _reqFilter.TREATMENT_ID = this.vhisTreatment.ID;
                    _reqFilter.ORDER_DIRECTION = "DESC";
                    _reqFilter.ORDER_FIELD = "MODIFY_TIME";
                    var currentServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("/api/HisServiceReq/GetView", ApiConsumers.MosConsumer, _reqFilter, param);
                    V_HIS_SERVICE_REQ _ExamServiceReq = new V_HIS_SERVICE_REQ();
                    if (currentServiceReq != null && currentServiceReq.Count > 0)
                    {
                        _ExamServiceReq = currentServiceReq.FirstOrDefault(p => p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH);
                    }

                    //V_HIS_TREATMENT currentTreatment;
                    //MOS.Filter.HisTreatmentViewFilter treatmentViewFilter = new HisTreatmentViewFilter();
                    //treatmentViewFilter.ID = vhisTreatment.ID;
                    //var treatmentViews = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("/api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatmentViewFilter, param);
                    //if (treatmentViews != null && treatmentViews.Count > 0)
                    //{
                    //    currentTreatment = treatmentViews[0];
                    //}
                    int treatmentCount = 1;
                    MOS.Filter.HisTreatmentFilter treatmentCountFilter = new HisTreatmentFilter();
                    treatmentCountFilter.PATIENT_ID = _Patient.ID;
                    var treatmentPatients = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("/api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentCountFilter, param);
                    if (treatmentPatients != null && treatmentPatients.Count > 0)
                    {
                        treatmentCount = treatmentPatients.Count;
                    }

                    MOS.Filter.HisSereServPtttFilter sereServPtttFilter = new HisSereServPtttFilter();
                    sereServPtttFilter.TDL_TREATMENT_ID = this.vhisTreatment.ID;
                    sereServPtttFilter.ORDER_DIRECTION = "DESC";
                    sereServPtttFilter.ORDER_FIELD = "MODIFY_TIME";
                    var sereServPttts = new BackendAdapter(param).Get<List<HIS_SERE_SERV_PTTT>>("/api/HisSereServPttt/Get", ApiConsumers.MosConsumer, sereServPtttFilter, param);
                    var currentServiceReqIdCls = currentServiceReq != null ? currentServiceReq.Where(o => o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL).Select(o => o.ID).ToList() : new List<long>();
                    MOS.Filter.HisSereServFilter sereServFilter = new HisSereServFilter();
                    sereServFilter.TREATMENT_ID = this.vhisTreatment.ID;
                    sereServFilter.ORDER_DIRECTION = "DESC";
                    sereServFilter.ORDER_FIELD = "MODIFY_TIME";
                    sereServFilter.SERVICE_REQ_IDs = currentServiceReqIdCls;
                    sereServFilter.TDL_SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN;
                    sereServFilter.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                    var sereServCLSs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("/api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);
                    #endregion

                    #region ------- HanhChinhBenhNhan
                    HanhChinhBenhNhan _HanhChinhBenhNhan = new HanhChinhBenhNhan();
                    _HanhChinhBenhNhan.SoYTe = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(p => p.ID == WorkPlace.WorkPlaceSDO.FirstOrDefault().BranchId).PARENT_ORGANIZATION_NAME;
                    _HanhChinhBenhNhan.BenhVien = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(p => p.ID == WorkPlace.WorkPlaceSDO.FirstOrDefault().BranchId).BRANCH_NAME;
                    _HanhChinhBenhNhan.MaYTe = "";
                    _HanhChinhBenhNhan.MaBenhNhan = _Patient.PATIENT_CODE;
                    _HanhChinhBenhNhan.TenBenhNhan = this.vhisTreatment.TDL_PATIENT_NAME;
                    _HanhChinhBenhNhan.NgaySinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.vhisTreatment.TDL_PATIENT_DOB) ?? DateTime.Now;
                    _HanhChinhBenhNhan.Tuoi = HIS.Desktop.Utility.AgeHelper.CalculateAgeNew(this.vhisTreatment.TDL_PATIENT_DOB.ToString());
                    _HanhChinhBenhNhan.GioiTinh = this.vhisTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? GioiTinh.Nam : this.vhisTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE ? GioiTinh.Nu : GioiTinh.ChuaXacDinh;
                    _HanhChinhBenhNhan.NgheNghiep = _Patient.CAREER_NAME;
                    _HanhChinhBenhNhan.MaNgheNghiep = _Patient.CAREER_CODE;
                    _HanhChinhBenhNhan.DanToc = _Patient.NATIONAL_NAME;
                    _HanhChinhBenhNhan.MaDanhToc = _Patient.NATIONAL_CODE;
                    _HanhChinhBenhNhan.NgoaiKieu = "";
                    _HanhChinhBenhNhan.MaNgoaiKieu = "";
                    var mitiRank = _Patient.MILITARY_RANK_ID > 0 ? BackendDataWorker.Get<HIS_MILITARY_RANK>().Where(o => o.ID == _Patient.MILITARY_RANK_ID).FirstOrDefault() : null;
                    _HanhChinhBenhNhan.CapBac = mitiRank != null ? mitiRank.MILITARY_RANK_NAME : "";
                    var branchPatient = _Patient.BRANCH_ID > 0 ? BackendDataWorker.Get<HIS_BRANCH>().Where(o => o.ID == _Patient.BRANCH_ID).FirstOrDefault() : null;
                    _HanhChinhBenhNhan.DonVi = branchPatient != null ? branchPatient.BRANCH_NAME : "";

                    if (String.IsNullOrEmpty(_Patient.COMMUNE_NAME) && String.IsNullOrEmpty(_Patient.DISTRICT_NAME) && String.IsNullOrEmpty(_Patient.PROVINCE_NAME))
                    {
                        _HanhChinhBenhNhan.SoNha = _Patient.VIR_ADDRESS;
                    }
                    else
                    {
                        _HanhChinhBenhNhan.SoNha = _Patient.ADDRESS;
                        _HanhChinhBenhNhan.ThonPho = "";
                        _HanhChinhBenhNhan.XaPhuong = _Patient.COMMUNE_NAME;
                        _HanhChinhBenhNhan.HuyenQuan = _Patient.DISTRICT_NAME;
                        _HanhChinhBenhNhan.MaHuyenQuan = _Patient.DISTRICT_CODE;
                        _HanhChinhBenhNhan.TinhThanhPho = _Patient.PROVINCE_NAME;
                        _HanhChinhBenhNhan.MaTinhThanhPho = _Patient.PROVINCE_CODE;
                    }

                    string patientTypeCode__Bhyt = HisConfigs.Get<string>(HIS.Desktop.Plugins.SurgServiceReqExecute.Config.HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                    string patientTypeCode__VP = HisConfigs.Get<string>(HIS.Desktop.Plugins.SurgServiceReqExecute.Config.HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                    _HanhChinhBenhNhan.NoiLamViec = _Patient.WORK_PLACE;
                    if (_PatientTypeAlter != null && _PatientTypeAlter.ID > 0)
                    {
                        _HanhChinhBenhNhan.DoiTuong = _PatientTypeAlter.PATIENT_TYPE_CODE == patientTypeCode__Bhyt ? DoiTuong.BHYT : _PatientTypeAlter.PATIENT_TYPE_CODE == patientTypeCode__VP ? DoiTuong.ThuPhi : DoiTuong.Khac;
                        if (_PatientTypeAlter.HEIN_CARD_FROM_TIME > 0)
                            _HanhChinhBenhNhan.NgayDangKyBHYT = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_PatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) ?? null;
                        if (_PatientTypeAlter.HEIN_CARD_TO_TIME > 0)
                            _HanhChinhBenhNhan.NgayHetHanBHYT = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_PatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? null;
                        _HanhChinhBenhNhan.SoTheBHYT = _PatientTypeAlter.HEIN_CARD_NUMBER;
                        _HanhChinhBenhNhan.TenNoiDangKyBHYT = _PatientTypeAlter.HEIN_MEDI_ORG_NAME;
                        _HanhChinhBenhNhan.MaNoiDangKyBHYT = _PatientTypeAlter.HEIN_MEDI_ORG_CODE;
                        _HanhChinhBenhNhan.NgayDuocHuong5Nam = _PatientTypeAlter.JOIN_5_YEAR == MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_PatientTypeAlter.JOIN_5_YEAR_TIME ?? 0) : null;
                    }

                    _HanhChinhBenhNhan.HoTenDiaChiNguoiNha = _Patient.RELATIVE_NAME + " - " + _Patient.RELATIVE_ADDRESS;
                    //_HanhChinhBenhNhan.SoDienThoaiNguoiNha = _Patient.re;
                    #endregion

                    #region ------- ThongTinDieuTri
                    ThongTinDieuTri _ThongTinDieuTri = new ThongTinDieuTri();
                    if (_Baby != null && _Baby.ID > 0)
                    {
                        _ThongTinDieuTri.LucVaoDe = "";
                        _ThongTinDieuTri.NgayDe = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_Baby.BORN_TIME ?? 0);
                        _ThongTinDieuTri.NgoiThai = _Baby.BORN_POSITION_NAME;
                        _ThongTinDieuTri.CachThucDe = _Baby.BORN_TYPE_NAME;
                        _ThongTinDieuTri.KiemSoatTuCung = false;
                        _ThongTinDieuTri.KiemSoatTuCung_Text = "";
                        _ThongTinDieuTri.TreSoSinh_LoaiThai = -1;
                        _ThongTinDieuTri.TreSoSinh_GioiTinh = _Baby.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? 1 : 0;
                        _ThongTinDieuTri.TreSoSinh_DiTat = 0;
                        _ThongTinDieuTri.TreSoSinh_DiTat_Text = "";
                        _ThongTinDieuTri.TreSoSinh_CanNang = (int?)_Baby.WEIGHT;
                        _ThongTinDieuTri.TreSoSinh_SongChet = (String.IsNullOrEmpty(_Baby.BORN_RESULT_CODE) || _Baby.BORN_RESULT_CODE == "01") ? 1 : 0;
                    }

                    _ThongTinDieuTri.NguyenNhan_BenhChinh_RaVien = vhisTreatment.ICD_NAME;
                    if (vhisTreatment.END_DEPARTMENT_ID > 0)
                    {
                        var dpEnd = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => p.ID == vhisTreatment.END_DEPARTMENT_ID).FirstOrDefault();
                        _ThongTinDieuTri.KhoaRaVien = dpEnd != null ? dpEnd.DEPARTMENT_NAME : "";
                    }

                    _ThongTinDieuTri.GiuongRaVien = "";
                    _ThongTinDieuTri.SoLuuTru = vhisTreatment.STORE_CODE;
                    _ThongTinDieuTri.MaQuanLy = Inventec.Common.TypeConvert.Parse.ToDecimal(_ExamServiceReq.TREATMENT_CODE); ;//Mỗi lần vào điều trị có cái mã
                    _ThongTinDieuTri.MaBenhNhan = _Patient.PATIENT_CODE;
                    _ThongTinDieuTri.NgayVaoVien = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(vhisTreatment.IN_TIME) ?? null;
                    _ThongTinDieuTri.TrucTiepVao = (_ExamServiceReq != null && _ExamServiceReq.IS_EMERGENCY == 1) ? TrucTiepVao.CapCuu : (_PatientTypeAlter != null && _PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM) ? TrucTiepVao.KKB : TrucTiepVao.KhoaDieuTri;
                    _ThongTinDieuTri.NoiGioiThieu = (_PatientTypeAlter != null && _PatientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT
                        && vhisTreatment.TRANSFER_IN_FORM_ID > 0) ? NoiGioiThieu.CoQuanYTe : NoiGioiThieu.Khac;
                    if (_DepartmentTrans != null && _DepartmentTrans.Count > 0)
                    {
                        _ThongTinDieuTri.TenKhoaVao = _DepartmentTrans[0].DEPARTMENT_NAME;
                        _ThongTinDieuTri.NgayVaoKhoa = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_DepartmentTrans[0].DEPARTMENT_IN_TIME ?? 0) ?? null;
                        if (_DepartmentTrans.Count > 1)
                        {
                            long? songay = null;
                            if (vhisTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                            {
                                songay = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[0].DEPARTMENT_IN_TIME, _DepartmentTrans[1].DEPARTMENT_IN_TIME, vhisTreatment.TREATMENT_END_TYPE_ID, vhisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                            }
                            else
                            {
                                songay = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[0].DEPARTMENT_IN_TIME, _DepartmentTrans[1].DEPARTMENT_IN_TIME, vhisTreatment.TREATMENT_END_TYPE_ID, vhisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                            }

                            _ThongTinDieuTri.SoNgayDieuTriTaiKhoa = Inventec.Common.TypeConvert.Parse.ToInt32(songay.ToString());
                            _ThongTinDieuTri.ChuyenKhoa1 = _DepartmentTrans[1].DEPARTMENT_NAME;
                            _ThongTinDieuTri.NgayChuyenKhoa1 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_DepartmentTrans[1].DEPARTMENT_IN_TIME ?? 0) ?? null;
                            if (_DepartmentTrans.Count > 2)
                            {
                                long? songay1 = null;
                                if (vhisTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                {
                                    songay1 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[1].DEPARTMENT_IN_TIME, _DepartmentTrans[2].DEPARTMENT_IN_TIME, vhisTreatment.TREATMENT_END_TYPE_ID, vhisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                                }
                                else
                                {
                                    songay1 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[1].DEPARTMENT_IN_TIME, _DepartmentTrans[2].DEPARTMENT_IN_TIME, vhisTreatment.TREATMENT_END_TYPE_ID, vhisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                                }

                                _ThongTinDieuTri.SoNgayDieuTriKhoa1 = Inventec.Common.TypeConvert.Parse.ToInt32(songay1.ToString());
                                _ThongTinDieuTri.ChuyenKhoa2 = _DepartmentTrans[2].DEPARTMENT_NAME;
                                _ThongTinDieuTri.NgayChuyenKhoa2 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_DepartmentTrans[2].DEPARTMENT_IN_TIME ?? 0) ?? null;
                                if (_DepartmentTrans.Count > 3)
                                {
                                    long? songay2 = null;
                                    if (vhisTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                    {
                                        songay2 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[2].DEPARTMENT_IN_TIME, _DepartmentTrans[3].DEPARTMENT_IN_TIME, vhisTreatment.TREATMENT_END_TYPE_ID, vhisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                                    }
                                    else
                                    {
                                        songay2 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[2].DEPARTMENT_IN_TIME, _DepartmentTrans[3].DEPARTMENT_IN_TIME, vhisTreatment.TREATMENT_END_TYPE_ID, vhisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                                    }
                                    _ThongTinDieuTri.SoNgayDieuTriKhoa2 = Inventec.Common.TypeConvert.Parse.ToInt32(songay2.ToString());
                                    _ThongTinDieuTri.ChuyenKhoa3 = _DepartmentTrans[3].DEPARTMENT_NAME;
                                    _ThongTinDieuTri.NgayChuyenKhoa3 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_DepartmentTrans[3].DEPARTMENT_IN_TIME ?? 0) ?? null;

                                    if (_DepartmentTrans.Count > 4)
                                    {
                                        long? songay3 = null;
                                        if (vhisTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                        {
                                            songay3 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[3].DEPARTMENT_IN_TIME, _DepartmentTrans[4].DEPARTMENT_IN_TIME, vhisTreatment.TREATMENT_END_TYPE_ID, vhisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                                        }
                                        else
                                        {
                                            songay3 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[3].DEPARTMENT_IN_TIME, _DepartmentTrans[4].DEPARTMENT_IN_TIME, vhisTreatment.TREATMENT_END_TYPE_ID, vhisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                                        }

                                        _ThongTinDieuTri.SoNgayDieuTriKhoa3 = Inventec.Common.TypeConvert.Parse.ToInt32(songay3.ToString());
                                    }
                                }
                            }
                        }
                    }

                    if (vhisTreatment.TRAN_PATI_FORM_ID > 0)
                        _ThongTinDieuTri.ChuyenVien = GetChuyenVienFromTranspatiForm(vhisTreatment.TRAN_PATI_FORM_ID);
                    _ThongTinDieuTri.TenVienChuyenBenhNhanDen = vhisTreatment.TRANSFER_IN_MEDI_ORG_NAME;
                    _ThongTinDieuTri.MaVienChuyenBenhNhanDen = vhisTreatment.TRANSFER_IN_MEDI_ORG_CODE;
                    if (vhisTreatment.OUT_TIME > 0)
                        _ThongTinDieuTri.NgayRaVien = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(vhisTreatment.OUT_TIME ?? 0) ?? null;
                    if (vhisTreatment.TREATMENT_END_TYPE_ID > 0)
                        _ThongTinDieuTri.TinhTrangRaVien = vhisTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN ? TinhTrangRaVien.RaVien : vhisTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__TRON ? TinhTrangRaVien.BoVe : vhisTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN ? TinhTrangRaVien.XinVe : TinhTrangRaVien.DuaVe;
                    long? _snDieuTri = null;
                    if (vhisTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        _snDieuTri = HIS.Common.Treatment.Calculation.DayOfTreatment(vhisTreatment.CLINICAL_IN_TIME, vhisTreatment.OUT_TIME, vhisTreatment.TREATMENT_END_TYPE_ID, vhisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                    }
                    else
                    {
                        _snDieuTri = HIS.Common.Treatment.Calculation.DayOfTreatment(vhisTreatment.CLINICAL_IN_TIME, vhisTreatment.OUT_TIME, vhisTreatment.TREATMENT_END_TYPE_ID, vhisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                    }

                    _ThongTinDieuTri.TongSoNgayDieuTri1 = _snDieuTri.ToString();
                    _ThongTinDieuTri.TongSoNgayDieuTri2 = _snDieuTri.ToString();
                    _ThongTinDieuTri.ChanDoan_NoiChuyenDen = vhisTreatment.IN_ICD_NAME;
                    _ThongTinDieuTri.MaICD_NoiChuyenDen = vhisTreatment.IN_ICD_CODE;
                    _ThongTinDieuTri.ChanDoan_KKB_CapCuu = "";
                    _ThongTinDieuTri.MaICD_KKB_CapCuu = "";
                    _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = vhisTreatment.TRANSFER_IN_ICD_NAME;
                    _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = vhisTreatment.TRANSFER_IN_ICD_CODE;
                    _ThongTinDieuTri.BenhChinh_RaVien = vhisTreatment.ICD_NAME;
                    _ThongTinDieuTri.MaICD_BenhChinh_RaVien = vhisTreatment.ICD_CODE;
                    _ThongTinDieuTri.BenhKemTheo_RaVien = vhisTreatment.ICD_TEXT;
                    _ThongTinDieuTri.MaICD_BenhKemTheo_RaVien = vhisTreatment.ICD_SUB_CODE;
                    _ThongTinDieuTri.VaoVienDoBenhNayLanThu = treatmentCount;
                    if (sereServPttts != null && sereServPttts.Count > 0)
                    {
                        _ThongTinDieuTri.TongSoLanPhauThuat = sereServPttts.Count;//TODO
                        _ThongTinDieuTri.TongSoNgayDieuTriSauPT = null;//TODO
                        _ThongTinDieuTri.LyDoTaiBienBienChung = null;//TODO
                        var ptttMethod = BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(o => o.ID == sereServPttts[0].PTTT_METHOD_ID).FirstOrDefault();
                        _ThongTinDieuTri.PhuongPhapPhauThuat = ptttMethod != null ? ptttMethod.PTTT_METHOD_NAME : "";
                        _ThongTinDieuTri.TinhHinhPhauThuat = false;//TODO
                        _ThongTinDieuTri.MaICD_ChanDoanSauPhauThuat = sereServPttts[0].AFTER_PTTT_ICD_CODE;
                        _ThongTinDieuTri.MaICD_ChanDoanTruocPhauThuat = sereServPttts[0].BEFORE_PTTT_ICD_CODE;
                        _ThongTinDieuTri.MaICD_NguyenNhan_BenhChinh_RV = sereServPttts[0].ICD_CODE;
                        _ThongTinDieuTri.ChanDoanSauPhauThuat = sereServPttts[0].AFTER_PTTT_ICD_NAME;
                        _ThongTinDieuTri.ChanDoanTruocPhauThuat = sereServPttts[0].BEFORE_PTTT_ICD_NAME;
                        _ThongTinDieuTri.ThuThuat = true;
                        _ThongTinDieuTri.PhauThuat = true;
                        _ThongTinDieuTri.TaiBien = sereServPttts.Any(o => o.PTTT_CATASTROPHE_ID > 0);
                        _ThongTinDieuTri.BienChung = false;//khong co truong du lieu trong DB
                    }

                    if (vhisTreatment.TREATMENT_RESULT_ID > 0)
                        _ThongTinDieuTri.KetQuaDieuTri = vhisTreatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__CHET ? KetQuaDieuTri.TuVong : vhisTreatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__DO ? KetQuaDieuTri.GiamDo : vhisTreatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KHOI ? KetQuaDieuTri.Khoi : vhisTreatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__NANG ? KetQuaDieuTri.NangHon : KetQuaDieuTri.KhongThayDoi;
                    _ThongTinDieuTri.GiaiPhauBenh = GiaiPhauBenh.LanhTinh;
                    if (vhisTreatment.DEATH_TIME > 0)
                        _ThongTinDieuTri.NgayTuVong = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(vhisTreatment.DEATH_TIME ?? 0);
                    if (vhisTreatment.DEATH_CAUSE_ID > 0)
                    {
                        var deathCause = BackendDataWorker.Get<HIS_DEATH_CAUSE>().Where(o => o.ID == vhisTreatment.DEATH_CAUSE_ID).FirstOrDefault();
                        if (deathCause != null)
                        {
                            _ThongTinDieuTri.LyDoTuVong = deathCause.DEATH_CAUSE_CODE == "01" ? LyDoTuVong.DoBenh : deathCause.DEATH_CAUSE_CODE == "02" ? LyDoTuVong.DoTaiBienDieuTri : LyDoTuVong.Khac;
                            _ThongTinDieuTri.NguyenNhanChinhTuVong = deathCause.DEATH_CAUSE_NAME;
                            _ThongTinDieuTri.MaICD_NguyenNhanChinhTuVong = vhisTreatment.ICD_CODE;
                        }
                    }

                    if (vhisTreatment.DEATH_WITHIN_ID > 0)
                    {
                        _ThongTinDieuTri.ThoiGianTuVong = vhisTreatment.DEATH_WITHIN_ID == 1 ? ThoiGianTuVong.Trong24hVaoVien : ThoiGianTuVong.Sau24hvaoVien;
                        _ThongTinDieuTri.KhamNghiemTuThi = false;
                        _ThongTinDieuTri.ChanDoanGiaiPhauTuThi = "";
                        _ThongTinDieuTri.MaICD_ChanDoanGiaiPhauTuThi = "";
                    }

                    _ThongTinDieuTri.MaGiamDocBenhVien = "";
                    _ThongTinDieuTri.NgayThangNamTrangBia = DateTime.Now;
                    _ThongTinDieuTri.MaTruongKhoa = "";
                    #endregion

                    #region ------ DHST
                    _ThongTinDieuTri.DauSinhTon = new DauSinhTon();
                    DauSinhTon _DauSinhTon = new DauSinhTon();
                    if (_DHST != null && _DHST.ID > 0)
                    {
                        _DauSinhTon.CanNang = (double)(_DHST.WEIGHT ?? 0);
                        _DauSinhTon.HuyetAp = _DHST.BLOOD_PRESSURE_MAX + "/" + _DHST.BLOOD_PRESSURE_MIN;
                        _DauSinhTon.Mach = (int)(_DHST.PULSE ?? 0);
                        _DauSinhTon.NhietDo = (double)(_DHST.TEMPERATURE ?? 0);
                        _DauSinhTon.NhipTho = (int)(_DHST.BREATH_RATE ?? 0);
                        _DauSinhTon.ChieuCao = (double)(_DHST.HEIGHT ?? 0);
                        _DauSinhTon.BMI = (double)(_DHST.VIR_BMI ?? 0);
                    }
                    _ThongTinDieuTri.DauSinhTon = _DauSinhTon;
                    #endregion

                    #region ------ HoSo
                    _ThongTinDieuTri.HoSo = new HoSo();
                    HoSo _HoSo = new HoSo();
                    _HoSo.CTScanner = 1;
                    _HoSo.Khac = 3;
                    _HoSo.Khac_Text = "Khac_Text";
                    _HoSo.SieuAm = 4;
                    _HoSo.ToanBoHoSo = 5;
                    _HoSo.XetNghiem = 6;
                    _HoSo.XQuang = 7;
                    _ThongTinDieuTri.HoSo = _HoSo;
                    #endregion

                    BenhAnNoiKhoa _BenhAnCommonADO = new BenhAnNoiKhoa();
                    _BenhAnCommonADO.DauSinhTon = new DauSinhTon();
                    _BenhAnCommonADO.HoSo = new HoSo();
                    _BenhAnCommonADO.DacDiemLienQuanBenh = new EMR_MAIN.DacDiemLienQuanBenh()
                    {
                        DiUng = false,
                        DiUng_Text = "",
                        Khac_DacDiemLienQuanBenh = false,
                        Khac_DacDiemLienQuanBenh_Text = "",
                        MaTuy = false,
                        MaTuy_Text = "",
                        RuouBia = false,
                        RuouBia_Text = "",
                        ThuocLa = false,
                        ThuocLao = false,
                        ThuocLao_Text = "",
                        ThuocLa_Text = ""
                    };

                    if (_ExamServiceReq != null)
                    {
                        _BenhAnCommonADO.BacSyDieuTri = _ExamServiceReq.EXECUTE_USERNAME;
                        _BenhAnCommonADO.BacSyLamBenhAn = _ExamServiceReq.REQUEST_USERNAME;
                        _BenhAnCommonADO.BenhChinh = _ExamServiceReq.ICD_NAME;
                        _BenhAnCommonADO.BenhKemTheo = _ExamServiceReq.ICD_TEXT;
                        _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam = sereServCLSs != null ? String.Join(";", sereServCLSs.Select(o => o.TDL_SERVICE_NAME).ToArray()) : "";
                        _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam = _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam.Length > 2048 ? _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam.Substring(0, 2048) : _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam;
                        _BenhAnCommonADO.CoXuongKhop = _ExamServiceReq.PART_EXAM_MUSCLE_BONE;
                        _BenhAnCommonADO.HoHap = _ExamServiceReq.PART_EXAM_RESPIRATORY;
                        _BenhAnCommonADO.HuongDieuTri = _ExamServiceReq.NEXT_TREATMENT_INSTRUCTION;
                        _BenhAnCommonADO.HuongDieuTriVaCacCheDoTiepTheo = _ExamServiceReq.NEXT_TREATMENT_INSTRUCTION;
                        _BenhAnCommonADO.Khac_CacCoQuan = _ExamServiceReq.PART_EXAM;
                        _BenhAnCommonADO.LyDoVaoVien = _ExamServiceReq.HOSPITALIZATION_REASON;
                        _BenhAnCommonADO.MaQuanLy = Inventec.Common.TypeConvert.Parse.ToDecimal(_ExamServiceReq.TREATMENT_CODE);
                        _BenhAnCommonADO.Mat = _ExamServiceReq.PART_EXAM_EYE;
                        _BenhAnCommonADO.NgayKhamBenh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_ExamServiceReq.INTRUCTION_DATE) ?? DateTime.Now;
                        _BenhAnCommonADO.NgayTongKet = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_ExamServiceReq.FINISH_TIME ?? 0) ?? DateTime.Now;
                        _BenhAnCommonADO.NguoiGiaoHoSo = _ExamServiceReq.REQUEST_USERNAME;
                        _BenhAnCommonADO.NguoiNhanHoSo = _ExamServiceReq.EXECUTE_USERNAME;
                        _BenhAnCommonADO.PhanBiet = "";
                        _BenhAnCommonADO.PhuongPhapDieuTri = _ExamServiceReq.TREATMENT_INSTRUCTION;
                        _BenhAnCommonADO.QuaTrinhBenhLy = _ExamServiceReq.PATHOLOGICAL_PROCESS;
                        _BenhAnCommonADO.QuaTrinhBenhLyVaDienBien = _ExamServiceReq.NOTE;
                        _BenhAnCommonADO.RangHamMat = _ExamServiceReq.PART_EXAM_STOMATOLOGY;
                        _BenhAnCommonADO.TaiMuiHong = String.IsNullOrEmpty(_ExamServiceReq.PART_EXAM_ENT) ? _ExamServiceReq.PART_EXAM_EAR + " " + _ExamServiceReq.PART_EXAM_NOSE + " " + _ExamServiceReq.PART_EXAM_THROAT : _ExamServiceReq.PART_EXAM_ENT;
                        _BenhAnCommonADO.TienLuong = "";
                        _BenhAnCommonADO.TienSuBenhBanThan = _ExamServiceReq.PATHOLOGICAL_HISTORY;
                        _BenhAnCommonADO.TienSuBenhGiaDinh = _ExamServiceReq.PATHOLOGICAL_HISTORY_FAMILY;
                        _BenhAnCommonADO.TieuHoa = _ExamServiceReq.PART_EXAM_DIGESTION;
                        _BenhAnCommonADO.TinhTrangNguoiBenhRaVien = _ExamServiceReq.ADVISE;
                        _BenhAnCommonADO.ToanThan = _ExamServiceReq.FULL_EXAM;
                        _BenhAnCommonADO.TomTatBenhAn = _ExamServiceReq.SUBCLINICAL;
                        _BenhAnCommonADO.TomTatKetQuaXetNghiem = "";
                        _BenhAnCommonADO.TuanHoan = _ExamServiceReq.PART_EXAM_CIRCULATION;
                        _BenhAnCommonADO.ThanKinh = _ExamServiceReq.PART_EXAM_NEUROLOGICAL;
                        _BenhAnCommonADO.ThanTietNieuSinhDuc = _ExamServiceReq.PART_EXAM_KIDNEY_UROLOGY;
                        _BenhAnCommonADO.VaoNgayThu = Inventec.Common.TypeConvert.Parse.ToInt32(_ExamServiceReq.SICK_DAY > 0 ? _ExamServiceReq.SICK_DAY.ToString() : "0");
                    }

                    #region Call Show ERM.Dll
                    ERMADO _ERMADO = new ERMADO();
                    _ERMADO._HanhChinhBenhNhan_s = new HanhChinhBenhNhan();
                    _ERMADO._ThongTinDieuTri_s = new ThongTinDieuTri();
                    _ERMADO._LoaiBenhAnEMR_s = new LoaiBenhAnEMR();
                    _ERMADO._LoaiBenhAnEMR_s = LoaiBenhAnEMR.NoiKhoa;//TODO

                    _ERMADO.KyDienTu_ApplicationCode = GlobalVariables.APPLICATION_CODE;
                    _ERMADO.KyDienTu_DiaChiACS = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_ACS;
                    _ERMADO.KyDienTu_DiaChiEMR = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_EMR;
                    _ERMADO.KyDienTu_DiaChiThuVienKy = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS;
                    _ERMADO.KyDienTu_TREATMENT_CODE = vhisTreatment.TREATMENT_CODE;

                    // gán thông tin hành chính
                    _ERMADO._HanhChinhBenhNhan_s = _HanhChinhBenhNhan;
                    _ERMADO._ThongTinDieuTri_s = _ThongTinDieuTri;

                    //Gán dữ liệu vào SDO , tương đương với mỗi loại bệnh án
                    //if (_TYpe == LoaiBenhAnEMR.Bong)
                    //{
                    _ERMADO._BenhAnNoiKhoa_s = new BenhAnNoiKhoa();//TODO
                    _ERMADO._BenhAnNoiKhoa_s = _BenhAnCommonADO;//TODO
                    //}
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ERMADO), _ERMADO));
                    string cmdLn = EncodeData(_ERMADO);
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = Application.StartupPath + @"\Integrate\\EMR\\ConnectToEMR.exe";
                    startInfo.Arguments = cmdLn;
                    Process.Start(startInfo);

                    WaitingManager.Hide();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string EncodeData(object data)
        {
            string result = "";
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(data));
            result = System.Convert.ToBase64String(plainTextBytes);
            return Inventec.Common.String.StringCompressor.CompressString(result);
        }

        private ChuyenVien GetChuyenVienFromTranspatiForm(long? tranpatiFormId)
        {
            ChuyenVien cv = ChuyenVien.Khac;
            try
            {
                if (tranpatiFormId.HasValue && tranpatiFormId > 0)
                {
                    var tpt = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().FirstOrDefault(o => o.ID == tranpatiFormId.Value);
                    if (tpt != null)
                    {
                        cv = (tpt.TRAN_PATI_FORM_CODE == "01" || tpt.TRAN_PATI_FORM_CODE == "02") ? ChuyenVien.TuyenDuoi : tpt.TRAN_PATI_FORM_CODE == "03" ? ChuyenVien.TuyenTren : ChuyenVien.Khac;
                    }
                }
            }
            catch { }
            return cv;
        }

        private bool DelegateRunPrinterSurg(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT_THU_THUAT__MPS000033:
                        LoadBieuMauPhieuThuThuatPhauThuat(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_GIAY_CAM_DOAN_CHAP_NHAN_pHAU_THUAT_THU_THUAT_VA_GAY_ME_HOI_SUC__MPS000035:
                        LoadBieuMauPhieuYCInGiayCamDoan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__CACH_THUC_PHAU_THUAT_THU_THUAT__MPS000097:
                        LoadBieuMauCachThucPhauThuat(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_SU_DUNG_VAT_TU_GIA_TRI_LON__MPS000311:
                        LoadBieuMauPhieuSuDungVatTuGiaTriLon(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_THANH_TOAN_PT_TT__MPS000324:
                        Mps000324(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_KE_KHAI_THUOC_VATU__MPS000338:
                        InPhieuKeKhaiThuocVatTu(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PT_DUC_TTT__Mps000390:
                        InPhieuPTMat(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PT_MONG__Mps000391:
                        InPhieuPTMat(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PT_TAI_TAO_LE_QUAN__Mps000392:
                        InPhieuPTMat(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__SUP_MI__Mps000393:
                        InPhieuPTMat(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GLOCOM__Mps000394:
                        InPhieuPTMat(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__LASER_YAG__Mps000395:
                        InPhieuPTMat(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__MONG_MAT__Mps000396:
                        InPhieuPTMat(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__DUNG_CU_VTYT__Mps000415:
                        InPhieuDungCuVtyt(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000493":
                        InPhieuKQDTDMV(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000495":
                        PrintMps495(printTypeCode, fileName, ref result);
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
        private void PrintMps495(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                HisExpMestMaterialViewFilter filter = new HisExpMestMaterialViewFilter();
                filter.SERE_SERV_PARENT_ID = sereServ.ID;
                filter.IS_REUSABLE = true;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var lstMatePrintMps495 = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, filter, null);
                if (lstMatePrintMps495 == null || lstMatePrintMps495.Count == 0)
                {
                    XtraMessageBox.Show("Không có vật tư tái sử dụng.");
                    return;
                }
                HisServiceReqViewFilter Srfilter = new HisServiceReqViewFilter();
                Srfilter.ID = serviceReq.ID;
                var ViewServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, Srfilter, null).FirstOrDefault();
                V_HIS_SERE_SERV SereServ = new V_HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(SereServ, sereServ);

                MPS.Processor.Mps000495.PDO.Mps000495PDO rdo = new MPS.Processor.Mps000495.PDO.Mps000495PDO(lstMatePrintMps495, SereServ, ViewServiceReq);

                RunPrint(printTypeCode, fileName, rdo, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, Module.RoomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        void RunPrint(string printTypeCode, string fileName, object data, Inventec.Common.FlexCelPrint.DelegateEventLog EventLogPrint, bool result, long? roomId)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName, EventLogPrint));
                }
                else
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(sereServ.TDL_TREATMENT_CODE, printTypeCode, roomId);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName, EventLogPrint) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void EventLogPrint()
        {
            try
            {
                string message = "In tem vật tư tái sử dụng. Mã in : Mps000495" + "  TREATMENT_CODE: " + sereServ.TDL_TREATMENT_CODE + "  Thời gian in: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + "  Người in: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void InPhieuKQDTDMV(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                HisPatientFilter patientFilter = new HisPatientFilter();
                patientFilter.ID = vhisTreatment.PATIENT_ID;
                HIS_PATIENT patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                MOS.Filter.HisSereServPtttViewFilter filter = new MOS.Filter.HisSereServPtttViewFilter();
                filter.SERE_SERV_ID = sereServ.ID;
                var sereServPttts = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                V_HIS_SERE_SERV_PTTT sereServPttt = sereServPttts != null && sereServPttts.Count > 0 ? sereServPttts.First() : null;

                MOS.Filter.HisSereServExtFilter extfilter = new MOS.Filter.HisSereServExtFilter();
                extfilter.SERE_SERV_ID = sereServ.ID;
                var sereServExts = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, extfilter, param);
                HIS_SERE_SERV_EXT sereServExt = sereServExts != null && sereServExts.Count > 0 ? sereServExts.First() : null;

                MOS.Filter.HisStentConcludeFilter conFilter = new MOS.Filter.HisStentConcludeFilter();
                conFilter.SERE_SERV_ID = sereServ.ID;
                var stentConclude = new BackendAdapter(param).Get<List<HIS_STENT_CONCLUDE>>("api/HisStentConclude/Get", ApiConsumers.MosConsumer, filter, param);

                List<V_HIS_EKIP_USER> ekipUsers = new List<V_HIS_EKIP_USER>();

                MOS.Filter.HisSereServFilter _SSfilter = new MOS.Filter.HisSereServFilter();
                _SSfilter.ID = sereServ.ID;
                var sereServByCheck = new BackendAdapter(new CommonParam())
                   .Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, _SSfilter, new CommonParam()).FirstOrDefault();
                if (sereServByCheck != null && sereServByCheck.EKIP_ID != null)
                {
                    HisEkipUserViewFilter hisEkipUserFilter = new HisEkipUserViewFilter();
                    hisEkipUserFilter.EKIP_ID = sereServByCheck.EKIP_ID;
                    ekipUsers = new BackendAdapter(new CommonParam())
            .Get<List<V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, hisEkipUserFilter, null);
                }
                HisSereServFileFilter ssffilter = new MOS.Filter.HisSereServFileFilter();
                ssffilter.SERE_SERV_ID = sereServ.ID;
                var sereServFiles = new BackendAdapter(param).Get<List<HIS_SERE_SERV_FILE>>("api/HisSereServFile/Get", ApiConsumers.MosConsumer, ssffilter, param);
                WaitingManager.Hide();
                MPS.Processor.Mps000493.PDO.Mps000493PDO rdo = new MPS.Processor.Mps000493.PDO.Mps000493PDO(
                    patient,
                    sereServ,
                    sereServPttt,
                    stentConclude,
                    ekipUsers,
                    serviceReq,
                    sereServExt,
                    sereServFiles
                    );

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(vhisTreatment != null ? vhisTreatment.TREATMENT_CODE : "", printTypeCode, Module != null ? Module.RoomId : 0);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void InPhieuDungCuVtyt(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                //Khoa hien tai
                HisTreatmentBedRoomViewFilter bedRoomFilter = new HisTreatmentBedRoomViewFilter();
                bedRoomFilter.TREATMENT_ID = serviceReq.TREATMENT_ID;
                var treatmentBedrooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, bedRoomFilter, param);
                V_HIS_TREATMENT_BED_ROOM treatmentBedroom = treatmentBedrooms != null ? treatmentBedrooms.Where(o => !o.REMOVE_TIME.HasValue).OrderByDescending(o => o.ADD_TIME).FirstOrDefault() : null;

                MOS.Filter.HisSereServPtttViewFilter filter = new MOS.Filter.HisSereServPtttViewFilter();
                filter.SERE_SERV_ID = sereServ.ID;
                var sereServPttts = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                V_HIS_SERE_SERV_PTTT sereServPttt = sereServPttts != null && sereServPttts.Count > 0 ? sereServPttts.First() : null;

                MOS.Filter.HisSereServExtFilter extfilter = new MOS.Filter.HisSereServExtFilter();
                extfilter.SERE_SERV_ID = sereServ.ID;
                var sereServExts = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, extfilter, param);
                HIS_SERE_SERV_EXT sereServExt = sereServExts != null && sereServExts.Count > 0 ? sereServExts.First() : null;

                MOS.Filter.HisSereServFilter filters = new MOS.Filter.HisSereServFilter();
                filters.PARENT_ID = sereServ.ID;
                filters.IS_EXPEND = true;
                var sereServFollows = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, filters, new CommonParam());

                MPS.Processor.Mps000415.PDO.Mps000415ADO ado = new MPS.Processor.Mps000415.PDO.Mps000415ADO();
                if (dtStart.EditValue != null && dtStart.DateTime != DateTime.MinValue)
                {
                    ado.BEGIN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStart.DateTime);
                }

                if (dtFinish.EditValue != null && dtFinish.DateTime != DateTime.MinValue)
                {
                    ado.END_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinish.DateTime);
                }

                WaitingManager.Hide();
                MPS.Processor.Mps000415.PDO.Mps000415PDO rdo = new MPS.Processor.Mps000415.PDO.Mps000415PDO(
                    serviceReq,
                    treatmentBedroom,
                    sereServPttt,
                    sereServExt,
                    sereServFollows,
                    ado,
                    BackendDataWorker.Get<HIS_SERVICE_UNIT>()
                    );

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(vhisTreatment != null ? vhisTreatment.TREATMENT_CODE : "", printTypeCode, Module != null ? Module.RoomId : 0);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadBieuMauKhacPhieuPTTT()
        {
            try
            {
                PrintOtherFormProcessor printOtherFormProcessor = new PrintOtherFormProcessor(this.serviceReq.ID, sereServ.ID, vhisTreatment.ID, vhisTreatment.PATIENT_ID, Library.PrintOtherForm.Base.UpdateType.TYPE.SERVICE_REQ);
                printOtherFormProcessor.Print(PrintType.TYPE.PHIEU_PHAU_THUAT_THU_THUAT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBieuMauKhacPhieuGayMeTruocMo()
        {
            try
            {
                PrintOtherFormProcessor printOtherFormProcessor = new PrintOtherFormProcessor(this.serviceReq.ID, sereServ.ID, vhisTreatment.ID, vhisTreatment.PATIENT_ID, Library.PrintOtherForm.Base.UpdateType.TYPE.SERVICE_REQ);
                printOtherFormProcessor.Print(PrintType.TYPE.MPS000345_PHIEU_KHAM_GAY_ME_TRUOC_MO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadGiayChungNhanPhauThuatThuThuat()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ListSurgMisuByTreatment").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ListSurgMisuByTreatment");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReq.TREATMENT_ID);

                    if (serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT)
                    {
                        listArgs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT.ToString());
                    }

                    if (serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT)
                    {
                        listArgs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT.ToString());
                    }

                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBieuMauCachThucPhauThuat(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                //View Ekip User
                List<V_HIS_EKIP_USER> ekipUsers = new List<V_HIS_EKIP_USER>();

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = vhisTreatment.PATIENT_ID;
                V_HIS_PATIENT patient = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, new CommonParam()).FirstOrDefault();

                MOS.Filter.HisSereServFilter _SSfilter = new MOS.Filter.HisSereServFilter();
                _SSfilter.ID = sereServ.ID;
                var sereServByCheck = new BackendAdapter(new CommonParam())
                   .Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, _SSfilter, new CommonParam()).FirstOrDefault();
                if (sereServByCheck != null && sereServByCheck.EKIP_ID != null)
                {
                    HisEkipUserViewFilter hisEkipUserFilter = new HisEkipUserViewFilter();
                    hisEkipUserFilter.EKIP_ID = sereServByCheck.EKIP_ID;
                    ekipUsers = new BackendAdapter(new CommonParam())
            .Get<List<V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, hisEkipUserFilter, null);
                }

                //List<HisEkipUserADO> ekipUserAdos = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                //AutoMapper.Mapper.CreateMap<HisEkipUserADO, MOS.EFMODEL.DataModels.V_HIS_EKIP_USER>();
                //ekipUsers = AutoMapper.Mapper.Map<List<HisEkipUserADO>, List<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER>>(ekipUserAdos);

                MOS.Filter.HisSereServPtttViewFilter filter = new MOS.Filter.HisSereServPtttViewFilter();
                filter.SERE_SERV_ID = sereServ.ID;

                var sereServPttts = new BackendAdapter(new CommonParam())
                   .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, filter, new CommonParam()).FirstOrDefault();
                WaitingManager.Hide();

                MPS.Processor.Mps000097.PDO.Mps000097PDO rdo = new MPS.Processor.Mps000097.PDO.Mps000097PDO(patient, sereServPttts, ekipUsers, vhisTreatment);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(vhisTreatment != null ? vhisTreatment.TREATMENT_CODE : "", printTypeCode, Module != null ? Module.RoomId : 0);
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoadBieuMauPhieuThuThuatPhauThuat(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                // Lấy thông tin bệnh nhân

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = vhisTreatment.PATIENT_ID;
                V_HIS_PATIENT patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                MPS.Processor.Mps000033.PDO.PatientADO currentPatient = new MPS.Processor.Mps000033.PDO.PatientADO(patient);
                //Lấy thông tin chuyển khoa
                var departmentTran = PrintGlobalStore.getDepartmentTran(vhisTreatment.ID);

                //Thông tin Misu
                //Khoa hien tai
                if (serviceReq != null)
                {
                    HIS_DEPARTMENT department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_DEPARTMENT_ID);
                    if (department != null)
                    {
                        serviceReq.REQUEST_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                    }

                    V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                    if (room != null)
                    {
                        serviceReq.REQUEST_ROOM_NAME = room.ROOM_NAME;
                    }
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

                V_HIS_BED_LOG currentBedLog = new V_HIS_BED_LOG();
                V_HIS_BED_LOG lastBedLog = new V_HIS_BED_LOG();
                MOS.Filter.HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                bedLogFilter.TREATMENT_ID = sereServ.TDL_TREATMENT_ID;
                var bedLogList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogFilter, null);
                if (bedLogList != null && bedLogList.Count > 0 && SereServExt != null)
                {
                    currentBedLog = bedLogList.Where(o => o.START_TIME <= SereServExt.BEGIN_TIME
                        && (!o.FINISH_TIME.HasValue || o.FINISH_TIME.Value > SereServExt.BEGIN_TIME))
                        .OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    lastBedLog = bedLogList.Where(o => o.START_TIME <= SereServExt.BEGIN_TIME)
                        .OrderByDescending(o => o.START_TIME).FirstOrDefault();
                }

                HisSereServFileFilter ssffilter = new MOS.Filter.HisSereServFileFilter();
                ssffilter.SERE_SERV_ID = sereServ.ID;
                var sereServFiles = new BackendAdapter(param).Get<List<HIS_SERE_SERV_FILE>>("api/HisSereServFile/Get", ApiConsumers.MosConsumer, ssffilter, param);

                HisSesePtttMethodViewFilter filterSese = new HisSesePtttMethodViewFilter();
                filterSese.TDL_SERE_SERV_ID = sereServ.ID;
                var sesePtttMethod = new BackendAdapter(param)
                  .Get<List<MOS.EFMODEL.DataModels.V_HIS_SESE_PTTT_METHOD>>("api/HisSesePtttMethod/GetView", ApiConsumers.MosConsumer, filterSese, param);

                List<string> listManner = new List<string>();
                HIS_SKIN_SURGERY_DESC skinDesc = null;
                if (currentHisSurgResultSDO != null && currentHisSurgResultSDO.SurgUpdateSDOs != null && currentHisSurgResultSDO.SurgUpdateSDOs.Count > 0)
                {
                    foreach (var item in currentHisSurgResultSDO.SurgUpdateSDOs)
                    {
                        if (item.SereServPttt != null)
                        {
                            listManner.Add(item.SereServPttt.MANNER);
                        }

                        if (skinDesc == null && item.SkinSurgeryDesc != null)
                        {
                            skinDesc = item.SkinSurgeryDesc;
                        }
                    }
                }

                WaitingManager.Hide();
                MPS.Processor.Mps000033.PDO.Mps000033PDO rdo = new MPS.Processor.Mps000033.PDO.Mps000033PDO(currentPatient, departmentTran, serviceReq, sereServ, SereServExt, sereServPttts, vhisTreatment, vEkipUsers, null, currentBedLog, lastBedLog, listManner, skinDesc, sereServFiles, sesePtttMethod);
                PrintData PrintData;
                if (chkSign.Checked && !IsActionPrint && chkIn.Checked)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, "");
                }
                else if (chkSign.Checked && !IsActionPrint && chkXemIn.Checked)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow, "");
                    //Inventec.Common.SignLibrary.SignLibraryGUIProcessor.SignAndShowPrintPreview(printTypeCode, fileName, rdo);
                }
                else if (chkSign.Checked && !IsActionPrint)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, "");
                }
                else if (chkXemIn.Checked)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                else if (chkIn.Checked)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(vhisTreatment != null ? vhisTreatment.TREATMENT_CODE : "", printTypeCode, Module != null ? Module.RoomId : 0);
                PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                PrintData.EmrInputADO = inputADO;
                result = MPS.MpsPrinter.Run(PrintData);
                IsActionPrint = false;
                IsActionOtherButton = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", this.Module.RoomId, this.Module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoadBieuMauPhieuYCInGiayCamDoan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                // Lấy thông tin bệnh nhân
                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = vhisTreatment.PATIENT_ID;
                V_HIS_PATIENT patient = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, new CommonParam()).FirstOrDefault();

                //Lấy thông tin chuyển khoa
                var departmentTran = PrintGlobalStore.getDepartmentTran(vhisTreatment.ID);
                WaitingManager.Hide();
                MPS.Processor.Mps000035.PDO.Mps000035PDO rdo = new MPS.Processor.Mps000035.PDO.Mps000035PDO(patient, departmentTran, serviceReq, vhisTreatment);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(vhisTreatment != null ? vhisTreatment.TREATMENT_CODE : "", printTypeCode, Module != null ? Module.RoomId : 0);
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadBieuMauPhieuSuDungVatTuGiaTriLon(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                MOS.Filter.HisSereServViewFilter filters = new MOS.Filter.HisSereServViewFilter();
                filters.PARENT_ID = sereServ.ID;
                filters.HAS_EXECUTE = true;
                filters.IS_EXPEND = false;
                var sereServFollows = new BackendAdapter(new CommonParam())
                   .Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filters, new CommonParam());
                if (sereServFollows != null)
                {
                    sereServFollows = sereServFollows.Where(o => (o.TDL_HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT || o.TDL_HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL) && o.PARENT_ID == sereServ.ID).ToList();
                }

                WaitingManager.Hide();
                MPS.Processor.Mps000311.PDO.Mps000311PDO rdo = new MPS.Processor.Mps000311.PDO.Mps000311PDO(sereServFollows ?? new List<V_HIS_SERE_SERV>(), sereServ, vhisTreatment, BackendDataWorker.Get<V_HIS_ROOM>(), BackendDataWorker.Get<HIS_SERVICE_UNIT>(), BackendDataWorker.Get<HIS_PATIENT_TYPE>());

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(vhisTreatment != null ? vhisTreatment.TREATMENT_CODE : "", printTypeCode, Module != null ? Module.RoomId : 0);
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuKeKhaiThuocVatTu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (serviceReq != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();

                    MOS.EFMODEL.DataModels.HIS_SERVICE_REQ ServiceReq = new HIS_SERVICE_REQ();
                    MOS.Filter.HisServiceReqFilter ServiceReqFilter = new HisServiceReqFilter();
                    ServiceReqFilter.ID = serviceReq.ID;
                    var serviceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, ServiceReqFilter, param);
                    if (serviceReqs != null)
                    {
                        ServiceReq = serviceReqs.FirstOrDefault();

                    }

                    // thông tin chung
                    MOS.Filter.HisSereServViewFilter filterComm = new HisSereServViewFilter();
                    filterComm.ID = this.sereServ.ID;

                    V_HIS_SERE_SERV sereServComm = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, filterComm, param).FirstOrDefault();
                    Inventec.Common.Logging.LogSystem.Debug("get sereServ common");

                    // đính kèm
                    MOS.Filter.HisSereServViewFilter filter = new HisSereServViewFilter();
                    filter.PARENT_ID = this.sereServ.ID;
                    //filter.IS_EXPEND = true;
                    var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    // 
                    V_HIS_TREATMENT_FEE treatment = new V_HIS_TREATMENT_FEE();

                    if (this.sereServ.TDL_TREATMENT_ID.HasValue)
                    {
                        MOS.Filter.HisTreatmentFeeViewFilter treatmentFilter = new HisTreatmentFeeViewFilter();
                        treatmentFilter.ID = this.sereServ.TDL_TREATMENT_ID;
                        treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                        Inventec.Common.Logging.LogSystem.Debug("get treatment");
                    }

                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                    if (this.sereServ.TDL_TREATMENT_ID.HasValue)
                    {
                        MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                        patientTypeAlterFilter.TREATMENT_ID = this.sereServ.TDL_TREATMENT_ID.Value;
                        var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                        if (patientTypeAlters != null && patientTypeAlters.Count() > 0)
                        {
                            patientTypeAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                        }
                        Inventec.Common.Logging.LogSystem.Debug("get patientTypeAlter");
                    }

                    MOS.EFMODEL.DataModels.V_HIS_BED_LOG hisBedLog = new V_HIS_BED_LOG();
                    MOS.Filter.HisTreatmentBedRoomFilter treatmentBedroomFilter = new HisTreatmentBedRoomFilter();
                    treatmentBedroomFilter.TREATMENT_ID = serviceReq.TREATMENT_ID;
                    var treatmentBedRooms = new BackendAdapter(param).Get<List<HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/Get", ApiConsumer.ApiConsumers.MosConsumer, treatmentBedroomFilter, null);
                    if (treatmentBedRooms != null && treatmentBedRooms.Count() > 0)
                    {
                        MOS.Filter.HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                        bedLogFilter.TREATMENT_BED_ROOM_IDs = treatmentBedRooms.Select(o => o.ID).Distinct().ToList();
                        var begLogs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogFilter, null);
                        hisBedLog = begLogs.OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    Inventec.Common.Logging.LogSystem.Debug("sereServs count" + sereServs.Count());
                    // get treatmentFee
                    MOS.Filter.HisTreatmentFeeViewFilter treatmentFeeFilter = new HisTreatmentFeeViewFilter();
                    treatmentFeeFilter.ID = ServiceReq.TREATMENT_ID;
                    var treatmentFees = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFeeFilter, null);

                    WaitingManager.Hide();
                    MPS.Processor.Mps000338.PDO.Mps000338PDO pdo = new MPS.Processor.Mps000338.PDO.Mps000338PDO(
                    sereServComm,
                    sereServs,
                    treatment,
                    patientTypeAlter,
                    ServiceReq,
                    hisBedLog
                    );

                    MPS.ProcessorBase.Core.PrintData printData = null;

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((sereServComm != null ? sereServComm.TDL_TREATMENT_CODE : ""), printTypeCode, this.Module != null ? this.Module.RoomId : 0);
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

        public void InPhieuPTMat(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                //+ Input truyền vào các MPS gồm dữ liệu các bảng sau (tương ứng với thông tin PTTT mà người dùng đang xử lý):
                //HIS_TREATMENT, HIS_SERE_SERV_PTTT, HIS_EYE_SURGRY_DESC                              

                List<V_HIS_EKIP_USER> vEkipUsers = new List<V_HIS_EKIP_USER>();
                if (sereServ.EKIP_ID.HasValue)
                {
                    HisEkipUserViewFilter ekipUserFilter = new HisEkipUserViewFilter();
                    ekipUserFilter.EKIP_ID = sereServ.EKIP_ID;
                    vEkipUsers = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, ekipUserFilter, param);
                }

                MOS.Filter.HisSereServPtttViewFilter filter = new MOS.Filter.HisSereServPtttViewFilter();
                filter.SERE_SERV_ID = sereServ.ID;
                var sereServPttts = new BackendAdapter(param)
                   .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                var sereServPttt = sereServPttts != null && sereServPttts.Count > 0 ? sereServPttts.FirstOrDefault() : null;

                HIS_TREATMENT treatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, vhisTreatment);

                WaitingManager.Hide();
                object rdo = null;
                switch (printTypeCode)
                {
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PT_DUC_TTT__Mps000390:
                        rdo = new MPS.Processor.Mps000390.PDO.Mps000390PDO(
                        treatment,
                        sereServPttt,
                        currentEyeSurgDesc,
                        SereServExt,
                        vEkipUsers
                        );
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PT_MONG__Mps000391:
                        rdo = new MPS.Processor.Mps000391.PDO.Mps000391PDO(
                        treatment,
                        sereServPttt,
                        currentEyeSurgDesc,
                        SereServExt,
                        vEkipUsers
                        );
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PT_TAI_TAO_LE_QUAN__Mps000392:
                        rdo = new MPS.Processor.Mps000392.PDO.Mps000392PDO(
                        treatment,
                        sereServPttt,
                        currentEyeSurgDesc,
                        SereServExt,
                        vEkipUsers
                        );
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__SUP_MI__Mps000393:
                        rdo = new MPS.Processor.Mps000393.PDO.Mps000393PDO(
                        treatment,
                        sereServPttt,
                        currentEyeSurgDesc,
                        SereServExt,
                        vEkipUsers
                        );
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GLOCOM__Mps000394:
                        rdo = new MPS.Processor.Mps000394.PDO.Mps000394PDO(
                        treatment,
                        sereServPttt,
                        currentEyeSurgDesc,
                        SereServExt,
                        vEkipUsers
                        );
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__LASER_YAG__Mps000395:
                        rdo = new MPS.Processor.Mps000395.PDO.Mps000395PDO(
                        treatment,
                        sereServPttt,
                        currentEyeSurgDesc,
                        SereServExt,
                        vEkipUsers
                        );
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__MONG_MAT__Mps000396:
                        rdo = new MPS.Processor.Mps000396.PDO.Mps000396PDO(
                        treatment,
                        sereServPttt,
                        currentEyeSurgDesc,
                        SereServExt,
                        vEkipUsers
                        );
                        break;
                    default:
                        break;
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(vhisTreatment != null ? vhisTreatment.TREATMENT_CODE : "", printTypeCode, Module != null ? Module.RoomId : 0);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        public void Mps000324(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                // Lấy thông tin bệnh nhân

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = vhisTreatment.PATIENT_ID;
                V_HIS_PATIENT patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                MPS.Processor.Mps000324.PDO.PatientADO currentPatient = new MPS.Processor.Mps000324.PDO.PatientADO(patient);
                //Lấy thông tin chuyển khoa
                var departmentTran = PrintGlobalStore.getDepartmentTran(vhisTreatment.ID);

                //Thông tin Misu
                //Khoa hien tai
                if (serviceReq != null)
                {
                    HIS_DEPARTMENT department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_DEPARTMENT_ID);
                    if (department != null)
                    {
                        serviceReq.REQUEST_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                    }

                    V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                    if (room != null)
                    {
                        serviceReq.REQUEST_ROOM_NAME = room.ROOM_NAME;
                    }
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

                MOS.Filter.HisSereServFilter filters = new MOS.Filter.HisSereServFilter();
                filters.PARENT_ID = sereServ.ID;
                //filters.HAS_EXECUTE = true;
                var sereServFollows = new BackendAdapter(new CommonParam())
                   .Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, filters, new CommonParam());

                WaitingManager.Hide();
                MPS.Processor.Mps000324.PDO.Mps000324PDO rdo = new MPS.Processor.Mps000324.PDO.Mps000324PDO(
                    currentPatient,
                    departmentTran,
                    serviceReq,
                    sereServ,
                    SereServExt,
                    sereServPttts,
                    vhisTreatment,
                    vEkipUsers,
                    BackendDataWorker.Get<HIS_SERVICE_TYPE>(),
                    BackendDataWorker.Get<HIS_SERVICE_UNIT>(),
                    sereServFollows ?? new List<V_HIS_SERE_SERV>()
                    );

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(vhisTreatment != null ? vhisTreatment.TREATMENT_CODE : "", printTypeCode, Module != null ? Module.RoomId : 0);
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        public string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            string result = "";
            try
            {
                result = ((int)((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100)) + "%";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadBieuMauKhacBangKiemAnToanPTTT()
        {
            try
            {
                if (sereServ != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.ServiceReqId = this.serviceReq.ID;
                    ado.SereServId = sereServ.ID;
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_SERVICE_REQ);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000408_BANG_KIEN_AN_TOAN_PHAU_THUAT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBangGayMeHoiSuc()
        {
            try
            {
                if (sereServ != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.ServiceReqId = this.serviceReq.ID;
                    ado.TreatmentId = this.serviceReq.TREATMENT_ID;
                    ado.SereServId = sereServ.ID;
                    ado.EkipId = this.sereServ.EKIP_ID;
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_SERVICE_REQ);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000411_BANG_GAY_ME_HOI_SUC);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPhieuPhauThuat()
        {
            try
            {
                if (sereServ != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.ServiceReqId = this.serviceReq.ID;
                    ado.TreatmentId = this.serviceReq.TREATMENT_ID;
                    ado.SereServId = sereServ.ID;
                    ado.EkipId = this.sereServ.EKIP_ID;
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_SERVICE_REQ);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000413_PHIEU_PHAU_THUAT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPhieuThuThuat()
        {
            try
            {
                if (sereServ != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.ServiceReqId = this.serviceReq.ID;
                    ado.TreatmentId = this.serviceReq.TREATMENT_ID;
                    ado.SereServId = sereServ.ID;
                    ado.EkipId = this.sereServ.EKIP_ID;
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_SERVICE_REQ);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000414_PHIEU_THU_THUAT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region dịch vụ/ thông tin chung
        private void CreateThreadLoadDataForService(ThreadChiDinhDichVuADO data)
        {
            Thread threadTreatment = new Thread(new ParameterizedThreadStart(LoadDataTreatment));
            Thread threadSereServ = new Thread(new ParameterizedThreadStart(LoadDataSereServ));
            Thread threadSereServBill = new Thread(new ParameterizedThreadStart(LoadDataSereServBill));
            Thread threadSereServDeposit = new Thread(new ParameterizedThreadStart(LoadDataSereServDeposit));

            try
            {
                threadTreatment.Start(data);
                threadSereServ.Start(data);
                threadSereServBill.Start(data);
                threadSereServDeposit.Start(data);

                threadTreatment.Join();
                threadSereServ.Join();
                threadSereServBill.Join();
                threadSereServDeposit.Join();
            }
            catch (Exception ex)
            {
                threadTreatment.Abort();
                threadSereServ.Abort();
                threadSereServBill.Abort();
                threadSereServDeposit.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSereServDeposit(object data)
        {
            try
            {
                LoadThreadDataSereServDeposit((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataSereServDeposit(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    List<HIS_SERE_SERV_DEPOSIT> ssDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                    List<HIS_SESE_DEPO_REPAY> ssRepay = new List<HIS_SESE_DEPO_REPAY>();

                    CommonParam paramCommon = new CommonParam();
                    HisSereServDepositFilter ssDepositFilter = new HisSereServDepositFilter();
                    ssDepositFilter.TDL_TREATMENT_ID = data.vHisServiceReq2Print.TREATMENT_ID;
                    ssDepositFilter.IS_CANCEL = false;
                    var apiDepositResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, ssDepositFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiDepositResult != null && apiDepositResult.Count > 0)
                    {
                        ssDeposit = apiDepositResult;
                    }

                    HisSeseDepoRepayFilter ssRepayFilter = new HisSeseDepoRepayFilter();
                    ssRepayFilter.TDL_TREATMENT_ID = data.vHisServiceReq2Print.TREATMENT_ID;
                    ssRepayFilter.IS_CANCEL = false;
                    var apiRepayResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumers.MosConsumer, ssRepayFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiRepayResult != null && apiRepayResult.Count > 0)
                    {
                        ssRepay = apiRepayResult;
                    }

                    data.ListSereServDeposit = ssDeposit.Where(o => !ssRepay.Exists(e => e.SERE_SERV_DEPOSIT_ID == o.ID)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSereServBill(object data)
        {
            try
            {
                LoadThreadDataSereServBill((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataSereServBill(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    CommonParam paramCommon = new CommonParam();
                    HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                    ssBillFilter.TDL_TREATMENT_ID = data.vHisServiceReq2Print.TREATMENT_ID;
                    ssBillFilter.IS_NOT_CANCEL = true;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        data.ListSereServBill = apiResult;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatment(object data)
        {
            try
            {
                LoadThreadDataTreatment((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataTreatment(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    CommonParam param = new CommonParam();

                    MOS.Filter.HisTreatmentFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentFilter();
                    hisTreatmentFilter.ID = data.vHisServiceReq2Print.TREATMENT_ID;
                    var treatments = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, hisTreatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (treatments != null && treatments.Count > 0)
                    {
                        data.hisTreatment = treatments.FirstOrDefault();
                    }

                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, data.vHisServiceReq2Print.TREATMENT_ID, param);
                    data.vHisPatientTypeAlter = patientTypeAlter;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSereServ(object data)
        {
            try
            {
                LoadThreadDataSereServ((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataSereServ(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    data.listVHisSereServ = GetSereServByServiceReqId(data.vHisServiceReq2Print.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_SERE_SERV> GetSereServByServiceReqId(long serviceReqId)
        {
            Inventec.Common.Logging.LogSystem.Debug("Begin get List<V_HIS_SERE_SERV>");
            List<V_HIS_SERE_SERV> result = new List<V_HIS_SERE_SERV>();
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.SERVICE_REQ_ID = serviceReqId;
                var apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null && apiResult.Count > 0)
                {
                    apiResult = apiResult.Where(o => o.IS_NO_EXECUTE != 1).ToList();
                    foreach (var item in apiResult)
                    {
                        V_HIS_SERE_SERV ss11 = new V_HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(ss11, item);

                        var service = lstService.FirstOrDefault(o => o.ID == ss11.SERVICE_ID);
                        if (service != null)
                        {
                            ss11.TDL_SERVICE_CODE = service.SERVICE_CODE;
                            ss11.TDL_SERVICE_NAME = service.SERVICE_NAME;
                            ss11.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                            ss11.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                            ss11.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                            ss11.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                            ss11.HEIN_SERVICE_TYPE_CODE = service.HEIN_SERVICE_TYPE_CODE;
                            ss11.HEIN_SERVICE_TYPE_NAME = service.HEIN_SERVICE_TYPE_NAME;
                            ss11.HEIN_SERVICE_TYPE_NUM_ORDER = service.HEIN_SERVICE_TYPE_NUM_ORDER;
                        }

                        var executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == item.TDL_EXECUTE_ROOM_ID);
                        if (executeRoom != null)
                        {
                            ss11.EXECUTE_ROOM_CODE = executeRoom.ROOM_CODE;
                            ss11.EXECUTE_ROOM_NAME = executeRoom.ROOM_NAME;
                            ss11.EXECUTE_DEPARTMENT_CODE = executeRoom.DEPARTMENT_CODE;
                            ss11.EXECUTE_DEPARTMENT_NAME = executeRoom.DEPARTMENT_NAME;
                        }

                        var reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == item.TDL_REQUEST_ROOM_ID);
                        if (reqRoom != null)
                        {
                            ss11.REQUEST_DEPARTMENT_CODE = reqRoom.DEPARTMENT_CODE;
                            ss11.REQUEST_DEPARTMENT_NAME = reqRoom.DEPARTMENT_NAME;
                            ss11.REQUEST_ROOM_CODE = reqRoom.ROOM_CODE;
                            ss11.REQUEST_ROOM_NAME = reqRoom.ROOM_NAME;
                        }

                        var patientTpye = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == item.PATIENT_TYPE_ID);
                        if (patientTpye != null)
                        {
                            ss11.PATIENT_TYPE_CODE = patientTpye.PATIENT_TYPE_CODE;
                            ss11.PATIENT_TYPE_NAME = patientTpye.PATIENT_TYPE_NAME;
                        }
                        result.Add(ss11);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_SERE_SERV>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("End get List<V_HIS_SERE_SERV>");
            return result;
        }
        #endregion
    }

    public class ERMADO
    {
        public string KyDienTu_DiaChiACS = "";
        public string KyDienTu_DiaChiEMR = "";
        public string KyDienTu_DiaChiThuVienKy = "";
        public string KyDienTu_ApplicationCode = "HIS";
        public string KyDienTu_TREATMENT_CODE = "";

        public LoaiBenhAnEMR _LoaiBenhAnEMR_s { get; set; }
        public HanhChinhBenhNhan _HanhChinhBenhNhan_s { get; set; }
        public ThongTinDieuTri _ThongTinDieuTri_s { get; set; }
        public BenhAnBong _BenhAnBong_s { get; set; }
        public BenhAnDaLieu _BenhAnDaLieu_s { get; set; }
        public BenhAnHuyetHocTruyenMau _BenhAnHuyetHocTruyenMau_s { get; set; }
        public BenhAnMatBanPhanTruoc _BenhAnMatBanPhanTruoc_s { get; set; }
        public BenhAnMatChanThuong _BenhAnMatChanThuong_s { get; set; }
        public BenhAnMatDayMat _BenhAnMatDayMat_s { get; set; }
        public BenhAnMatGlocom _BenhAnMatGlocom_s { get; set; }
        public BenhAnMatLac _BenhAnMatLac_s { get; set; }
        public BenhAnMatTreEm _BenhAnMatTreEm_s { get; set; }
        public BenhAnDieuTriBanNgay _BenhAnDieuTriBanNgay_s { get; set; }
        public BenhAnNgoaiKhoa _BenhAnNgoaiKhoa_s { get; set; }
        public BenhAnNgoaiTru _BenhAnNgoaiTru_s { get; set; }
        public BenhAnNgoaiTruRangHamMat _BenhAnNgoaiTruRangHamMat_s { get; set; }
        public BenhAnNgoaiTruTaiMuiHong _BenhAnNgoaiTruTaiMuiHong_s { get; set; }
        public BenhAnNgoaiTruYHCT _BenhAnNgoaiTruYHCT_s { get; set; }
        public BenhAnNhiKhoa _BenhAnNhiKhoa_s { get; set; }
        public BenhAnNoiKhoa _BenhAnNoiKhoa_s { get; set; }
        public BenhAnNoiTruYHCT _BenhAnNoiTruYHCT_s { get; set; }
        public BenhAnPhuKhoa _BenhAnPhuKhoa_s { get; set; }
        public BenhAnPhucHoiChucNang _BenhAnPhucHoiChucNang_s { get; set; }
        public BenhAnRangHamMat _BenhAnRangHamMat_s { get; set; }
        public BenhAnSanKhoa _BenhAnSanKhoa_s { get; set; }
        public BenhAnSoSinh _BenhAnSoSinh_s { get; set; }
        public BenhAnTaiMuiHong _BenhAnTaiMuiHong_s { get; set; }
        public BenhAnTamThan _BenhAnTamThan_s { get; set; }
        public BenhAnTruyenNhiem _BenhAnTruyenNhiem_s { get; set; }
        public BenhAnUngBuou _BenhAnUngBuou_s { get; set; }
        public BenhAnXaPhuong _BenhAnXaPhuong_s { get; set; }
        public List<PhauThuatThuThuat_HIS> PhauThuatThuThuat_HIS_s { get; set; }
        public BenhAnTim _BenhAnTim_s { get; set; }
    }
}
