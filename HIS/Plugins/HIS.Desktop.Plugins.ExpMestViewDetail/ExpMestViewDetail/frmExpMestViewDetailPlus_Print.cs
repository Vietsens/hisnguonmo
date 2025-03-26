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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestViewDetail.ADO;
using HIS.Desktop.Plugins.ExpMestViewDetail.Config;
using HIS.Desktop.Plugins.Library.PrintPrescription;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ADO;
using MPS.Processor.Mps000118.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestViewDetail.ExpMestViewDetail
{
    public partial class frmExpMestViewDetail : HIS.Desktop.Utility.FormBase
    {
        List<V_HIS_EXP_MEST_MEDICINE> lstHuongThan = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> lstGayNghien = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> lstGopGayNgienHuongThan = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> lstThuong = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<HIS_BLOOD_GIVER> lstBloodGiver = new List<HIS_BLOOD_GIVER>();
        HisExpMestResultSDO resultSdo = null;
        string Req_Department_Name = "";
        string Req_Room_Name = "";
        string Exp_Department_Name = "";
        long roomIdByMediStockIdPrint = 0;
        long keyPhieuTra = 0;
        internal enum PrintType
        {
            PHIEU_XUAT_BAN,
            PHIEU_XUAT_CHUYEN_KHO,
            PHIEU_XUAT_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN,
            PHIEU_XUAT_CHUYEN_KHO_THUOC_KHONG_PHAI_GAY_NGHIEN_HUONG_THAN,
            PHIEU_XUAT_SU_DUNG,
            PHIEU_XUAT_SU_DUNG_THEO_DIEU_KIEN,
            PHIEU_XUAT_TRA_NHA_CUNG_CAP,
            PHIEU_XUAT_DON_DUOC_THUOC_VAT_TU,
            PHIEU_YEU_CAU_DON_DUOC_MAU,//MPS108
            PHIEU_XUAT_DON_DUOC_MAU,//MPS107
            IN_HUONG_DAN_SU_DUNG,
            PHIEU_XUAT_HAO_PHI,
            PHIEU_XUAT_THANH_LY,
            PHIEU_XUAT_KHAC,
            PHIEU_XUAT_MAT_MAT,
            PHIEU_XUAT_CHUYEN_KHO_MAU,
            BIEN_BAN_XUAT_MAT_MAT,
            MPS000215_XUAT_BU_CO_SO_TU_TRUC,
            MPS000203_PHIEU_XUAT_KHAC_MAU,
            MPS000216_PHIEU_XUAT_BU_THUOC_LE,
            MPS000244_PHIEU_XUAT_BAO_CHE_THUOC,
            MPS000339_IN_HOA_DON_DO,
            MPS000346,
            MPS000347,
            Mps000421_PHIEU_HIEU_TRUYEN_MAU_VA_CHE_PHAM,
            Mps000422_PHIEU_IN_TEM_DON_MAU,
            Mps000234_DON_TONG_HOP,
            Mps000044_IN_DON_THUOC,
            PHIEU_XUAT_HAO_PHI_KHOA_PHONG_THEO_DIEU_KIEN,
            Mps000118_DON_THUOC_TONG_HOP,
            InVatTuTSD
        }

        string printerName = "";
        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Inventec.Common.SignLibrary.ADO.InputADO();
        private void ProcessPrint(String printTypeCode)
        {
            try
            {
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._CurrentExpMest != null ? this._CurrentExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.moduleData != null ? this.moduleData.RoomId : 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitMenuToButtonPrint()
        {
            try
            {
                WaitingManager.Show();
                DXPopupMenu menu = new DXPopupMenu();
                // xuat ban
                if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT || this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK ||
                    this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT || this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                {
                    if (HisConfigCFG.IS_MUST_BE_FINISHED_BEFORED_PRINTING)
                    {
                        if (this._CurrentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            DXMenuItem itemPhieuXuatBan = new DXMenuItem("Phiếu xuất bán", new EventHandler(OnClickInPhieuXuatKho));
                            itemPhieuXuatBan.Tag = PrintType.PHIEU_XUAT_BAN;
                            menu.Items.Add(itemPhieuXuatBan);
                        }
                    }
                    else
                    {
                        DXMenuItem itemPhieuXuatBan = new DXMenuItem("Phiếu xuất bán", new EventHandler(OnClickInPhieuXuatKho));
                        itemPhieuXuatBan.Tag = PrintType.PHIEU_XUAT_BAN;
                        menu.Items.Add(itemPhieuXuatBan);
                    }

                    DXMenuItem itemInHuongDanSuDung = new DXMenuItem("Hướng dẫn sử dụng", new EventHandler(OnClickInPhieuXuatKho));
                    itemInHuongDanSuDung.Tag = PrintType.IN_HUONG_DAN_SU_DUNG;
                    menu.Items.Add(itemInHuongDanSuDung);

                    DXMenuItem itemInDonThuoc = new DXMenuItem("In đơn thuốc", new EventHandler(OnClickInPhieuXuatKho));
                    itemInDonThuoc.Tag = PrintType.Mps000044_IN_DON_THUOC;
                    menu.Items.Add(itemInDonThuoc);
                }
                else if (this._CurrentExpMest.CHMS_TYPE_ID.HasValue && this._CurrentExpMest.CHMS_TYPE_ID.Value == 1)
                {
                    DXMenuItem itemXuatChuyenKho = new DXMenuItem("Phiếu bổ sung cơ số tủ trực", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatChuyenKho.Tag = PrintType.MPS000347;
                    menu.Items.Add(itemXuatChuyenKho);
                }
                else if (this._CurrentExpMest.CHMS_TYPE_ID.HasValue && this._CurrentExpMest.CHMS_TYPE_ID.Value == 2)
                {
                    DXMenuItem itemXuatChuyenKho = new DXMenuItem("Phiếu hoàn cơ số tủ trực", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatChuyenKho.Tag = PrintType.MPS000346;
                    menu.Items.Add(itemXuatChuyenKho);
                }
                // xuat chuyen kho
                else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                {
                    List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine = this._ExpMestMedicines_Print;
                    List<V_HIS_MEDICINE_TYPE> listMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    if (listExpMestMedicine != null && listMedicineType != null && listExpMestMedicine.Exists(o => listMedicineType.Exists(p => p.ID == o.MEDICINE_TYPE_ID && p.IS_VACCINE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)))
                    {
                        DXMenuItem itemXuatChuyenKho = new DXMenuItem("Biên bản giao nhận Vaccine", new EventHandler(OnClickInPhieuXuatKho));
                        itemXuatChuyenKho.Tag = PrintType.PHIEU_XUAT_CHUYEN_KHO;
                        menu.Items.Add(itemXuatChuyenKho);
                    }
                    else
                    {
                        DXMenuItem itemXuatChuyenKho = new DXMenuItem("Phiếu xuất chuyển kho", new EventHandler(OnClickInPhieuXuatKho));
                        itemXuatChuyenKho.Tag = PrintType.PHIEU_XUAT_CHUYEN_KHO;
                        menu.Items.Add(itemXuatChuyenKho);
                    }
                }
                // xuat tra nha cung cap
                else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC)
                {
                    DXMenuItem itemXuatChuyenKho = new DXMenuItem("Phiếu xuất trả nhà cung cấp", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatChuyenKho.Tag = PrintType.PHIEU_XUAT_TRA_NHA_CUNG_CAP;
                    menu.Items.Add(itemXuatChuyenKho);
                }
                // xuất đơn dược
                else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT || this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK || this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                {

                    DXMenuItem itemXuatDonDuocThuocVatTu = new DXMenuItem("In đơn thuốc", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatDonDuocThuocVatTu.Tag = PrintType.PHIEU_XUAT_DON_DUOC_THUOC_VAT_TU;
                    menu.Items.Add(itemXuatDonDuocThuocVatTu);

                }
                else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                {
                    if (this._CurrentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        DXMenuItem itemXuatDonDuocMau = new DXMenuItem("Phiếu xuất máu", new EventHandler(OnClickInPhieuXuatKho));
                        itemXuatDonDuocMau.Tag = PrintType.PHIEU_XUAT_DON_DUOC_MAU;
                        menu.Items.Add(itemXuatDonDuocMau);

                        DXMenuItem itemTruyenMauVaChePham = new DXMenuItem("Phiếu Truyền máu và chế phẩm", new EventHandler(OnClickInPhieuXuatKho));
                        itemTruyenMauVaChePham.Tag = PrintType.Mps000421_PHIEU_HIEU_TRUYEN_MAU_VA_CHE_PHAM;
                        menu.Items.Add(itemTruyenMauVaChePham);


                    }
                    else
                    {
                        DXMenuItem itemYeuCauDonDuocMau = new DXMenuItem("Phiếu yêu cầu máu", new EventHandler(OnClickInPhieuXuatKho));
                        itemYeuCauDonDuocMau.Tag = PrintType.PHIEU_YEU_CAU_DON_DUOC_MAU;
                        menu.Items.Add(itemYeuCauDonDuocMau);
                    }
                    if (this._CurrentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || this._CurrentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE || (this._CurrentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && this._CurrentExpMest.IS_CONFIRM == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE))
                    {
                        DXMenuItem itemIntemDonMau = new DXMenuItem("Phiếu in tem đơn máu", new EventHandler(OnClickInPhieuXuatKho));
                        itemIntemDonMau.Tag = PrintType.Mps000422_PHIEU_IN_TEM_DON_MAU;
                        menu.Items.Add(itemIntemDonMau);
                    }
                }
                // xuất hao phí khoa phòng
                else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)//
                {
                    DXMenuItem itemXuatSuDung = new DXMenuItem("Phiếu xuất hao phí khoa phòng", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatSuDung.Tag = PrintType.PHIEU_XUAT_SU_DUNG;
                    menu.Items.Add(itemXuatSuDung);

                    DXMenuItem itemDieuKien = new DXMenuItem("In phiếu xuất hao phí khoa phòng theo điều kiện", new EventHandler(OnClickInPhieuXuatKho));
                    itemDieuKien.Tag = PrintType.PHIEU_XUAT_HAO_PHI_KHOA_PHONG_THEO_DIEU_KIEN;
                    menu.Items.Add(itemDieuKien);
                }
                else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__KHAC)
                {
                    DXMenuItem itemXuatKhac = new DXMenuItem("Phiếu xuất khác", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatKhac.Tag = PrintType.PHIEU_XUAT_KHAC;
                    menu.Items.Add(itemXuatKhac);

                    DXMenuItem itemXuatKhacMau = new DXMenuItem("Phiếu xuất khác máu", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatKhacMau.Tag = PrintType.MPS000203_PHIEU_XUAT_KHAC_MAU;
                    menu.Items.Add(itemXuatKhacMau);
                }
                else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                {
                    DXMenuItem itemXuatBuCoSoTuTruc = new DXMenuItem("Phiếu xuất bù cơ số tủ trực", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatBuCoSoTuTruc.Tag = PrintType.MPS000215_XUAT_BU_CO_SO_TU_TRUC;
                    menu.Items.Add(itemXuatBuCoSoTuTruc);
                }
                else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL)
                {
                    DXMenuItem itemXuatBuThuocLe = new DXMenuItem("Phiếu xuất bù thuốc lẻ", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatBuThuocLe.Tag = PrintType.MPS000216_PHIEU_XUAT_BU_THUOC_LE;
                    menu.Items.Add(itemXuatBuThuocLe);
                }
                else if (this._CurrentExpMest.CHMS_TYPE_ID.HasValue && this._CurrentExpMest.CHMS_TYPE_ID == 1)
                {

                }
                //else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                //{
                //    DXMenuItem itemXuatBaoChe = new DXMenuItem("Phiếu xuất bào chế thuốc", new EventHandler(OnClickInPhieuXuatKho));
                //    itemXuatBaoChe.Tag = PrintType.MPS000244_PHIEU_XUAT_BAO_CHE_THUOC;
                //    menu.Items.Add(itemXuatBaoChe);
                //}
                else
                {
                    isPrintXuatHoaChatXN = true;
                    //cboPrint.Enabled = false;
                }

                if (this._CurrentExpMest.BILL_ID.HasValue)
                {
                    DXMenuItem itemHoaDonDo = new DXMenuItem("In hóa đơn đỏ", new EventHandler(OnClickInPhieuXuatKho));
                    itemHoaDonDo.Tag = PrintType.MPS000339_IN_HOA_DON_DO;
                    menu.Items.Add(itemHoaDonDo);
                }

                if ((this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT || this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT || this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK) && this._CurrentExpMest.IS_NOT_TAKEN != 1)
                {
                    DXMenuItem itemDonTongHop = new DXMenuItem("Đơn tổng hợp", new EventHandler(OnClickInPhieuXuatKho));
                    itemDonTongHop.Tag = PrintType.Mps000234_DON_TONG_HOP;
                    menu.Items.Add(itemDonTongHop);
                }
                DXMenuItem itemDonThuocTongHop = new DXMenuItem("In đơn thuốc tổng hợp", new EventHandler(OnClickInPhieuXuatKho));
                itemDonThuocTongHop.Tag = PrintType.Mps000118_DON_THUOC_TONG_HOP;
                menu.Items.Add(itemDonThuocTongHop);


                if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT || this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT || this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK)
                {
                    DXMenuItem itemVTTSD = new DXMenuItem("In tem vật tư tái sử dụng", new EventHandler(OnClickInPhieuXuatKho));
                    itemVTTSD.Tag = PrintType.InVatTuTSD;
                    menu.Items.Add(itemVTTSD);
                }
                cboPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInBienBanXuatMatMat(object sender, EventArgs e)
        { }

        private void OnClickInPhieuXuatKho(object sender, EventArgs e)
        {
            try
            {
                LoadSpecificExpMest();
                var bbtnItem = sender as DXMenuItem;
                PrintType type = (PrintType)(bbtnItem.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {

                    case PrintType.PHIEU_XUAT_CHUYEN_KHO:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKho_MPS000086, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_XUAT_TRA_NHA_CUNG_CAP:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_XUAT_TRA_NHA_CUNG_CAP__MPS000130, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_XUAT_DON_DUOC_THUOC_VAT_TU:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_YEU_CAU_DON_DUOC_MAU:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000108, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_XUAT_DON_DUOC_MAU:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__EXPORT_BLOOD__MPS000107, DelegateRunPrinter);
                        break;
                    case PrintType.IN_HUONG_DAN_SU_DUNG:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_XUAT_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocGayNghienHuongThan_MPS000089, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_XUAT_CHUYEN_KHO_THUOC_KHONG_PHAI_GAY_NGHIEN_HUONG_THAN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan_MPS000090, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_XUAT_HAO_PHI:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_HAO_PHI__MPS000154, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_XUAT_THANH_LY:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_THANH_LY__MPS000155, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_XUAT_KHAC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_KHAC__MPS000165, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_XUAT_MAT_MAT:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_MAT_MAT__MPS000168, DelegateRunPrinter);
                        break;
                    case PrintType.BIEN_BAN_XUAT_MAT_MAT:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__BIEN_BAN_XUAT_MAT_MAT__MPS000205, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_XUAT_CHUYEN_KHO_MAU:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_CHUYEN_KHO_MAU__MPS000198, DelegateRunPrinter);
                        break;
                    case PrintType.MPS000215_XUAT_BU_CO_SO_TU_TRUC:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_BU_CO_SO_TU_TRUC__MPS000215, DelegateRunPrinter);
                        break;
                    case PrintType.MPS000203_PHIEU_XUAT_KHAC_MAU:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_KHAC_MAU__MPS000203, DelegateRunPrinter);
                        break;
                    case PrintType.MPS000216_PHIEU_XUAT_BU_THUOC_LE:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_BU_THUOC_LE__MPS000216, DelegateRunPrinter);
                        break;
                    case PrintType.MPS000339_IN_HOA_DON_DO:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__HOA_DON_DO__MPS000339, DelegateRunPrinter);
                        break;
                    case PrintType.MPS000346:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__MPS000346, DelegateRunPrinter);
                        break;
                    case PrintType.MPS000347:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__MPS000347, DelegateRunPrinter);
                        break;
                    case PrintType.Mps000421_PHIEU_HIEU_TRUYEN_MAU_VA_CHE_PHAM:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__PHIEU_HIEU_TRUYEN_MAU_VA_CHE_PHAM_Mps000421, DelegateRunPrinter);
                        break;
                    //case PrintType.MPS000244_PHIEU_XUAT_BAO_CHE_THUOC:
                    //    richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_BAO_CHE_THUOC__MPS000244, DelegateRunPrinter);
                    //    break;
                    case PrintType.Mps000422_PHIEU_IN_TEM_DON_MAU:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__PHIEU_IN_TEM_DON_MAU_Mps000422, DelegateRunPrinter);
                        break;
                    case PrintType.Mps000234_DON_TONG_HOP:
                        InDonTongHop();
                        //richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__DON_TONG_HOP_Mps000234, DelegateRunPrinter);
                        break;
                    case PrintType.Mps000044_IN_DON_THUOC:
                        //  richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE_IN_DON_THUOC_Mps000044, DelegateRunPrinter);
                        //InDonThuoc(printTypeCode, fileName, ref result);
                        InDonThuoc(RequestUri.PRINT_TYPE_CODE_IN_DON_THUOC_Mps000044);
                        break;
                    case PrintType.PHIEU_XUAT_BAN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092, DelegateRunPrinter);
                        break;

                    case PrintType.PHIEU_XUAT_SU_DUNG:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatSuDung_MPS000135, DelegateRunPrinter);
                        break;

                    case PrintType.PHIEU_XUAT_HAO_PHI_KHOA_PHONG_THEO_DIEU_KIEN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatSuDungTheoDieuKien_MPS000134, DelegateRunPrinter);
                        break;
                    case PrintType.Mps000118_DON_THUOC_TONG_HOP:
                        InDonThuocTongHop(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118);
                        break;
                    case PrintType.InVatTuTSD:
                        Btn_InVatTuTSD();
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


        private void Btn_InVatTuTSD()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate("Mps000494", DelegateRunPrinter);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }


        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {

                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKho_MPS000086:
                        InPhieuXuatChuyenKho(printTypeCode, fileName, ref result);
                        break;
                    case RequestUri.PRINT_TYPE_CODE__MPS000346:
                        Mps000346(ref result, printTypeCode, fileName);
                        break;
                    case RequestUri.PRINT_TYPE_CODE__MPS000347:
                        Mps000347(ref result, printTypeCode, fileName);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_XUAT_TRA_NHA_CUNG_CAP__MPS000130:
                        InPhieuXuatTraNhaCungCap(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__EXPORT_BLOOD__MPS000107:
                        InPhieuXuatDonDuocMau(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000108:
                        InPhieuYeuCauDonDuocMau(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099:
                        InHuongDanSuDung(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocGayNghienHuongThan_MPS000089:
                        InPhieuXuatChuyenKhoThuocGayNghienHuongThan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan_MPS000090:
                        InPhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan(printTypeCode, fileName, ref result);
                        break;

                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_HAO_PHI__MPS000154:
                        InPhieuXuatHaoPhi(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_THANH_LY__MPS000155:
                        InPhieuXuatThanhLy(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118:
                        InPhieuXuatDonTongHopThuocVatTu(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_KHAC__MPS000165:
                        InPhieuXuatKhac(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_MAT_MAT__MPS000168:
                        InPhieuXuatMatMat(printTypeCode, fileName, ref result);
                        break;
                    case RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_KHAC_MAU__MPS000203:
                        InPhieuXuatKhacMau(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_CHUYEN_KHO_MAU__MPS000198:
                        InPhieuXuatChuyenKhoMau(printTypeCode, fileName, ref result);
                        break;
                    case RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_BU_CO_SO_TU_TRUC__MPS000215:
                        InPhieuXuatBuCoSoTuTruc(printTypeCode, fileName, ref result);
                        break;
                    case RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_BU_THUOC_LE__MPS000216:
                        InPhieuXuatBuThuocLe(printTypeCode, fileName, ref result);
                        break;
                    case RequestUri.PRINT_TYPE_CODE__BIEUMAU__HOA_DON_DO__MPS000339:
                        InHoaDonDo(printTypeCode, fileName, ref result);
                        break;
                    case RequestUri.PRINT_TYPE_CODE__PHIEU_HIEU_TRUYEN_MAU_VA_CHE_PHAM_Mps000421:
                        InPhieuHieuTruyenMauVaChePham(printTypeCode, fileName, ref result);
                        break;
                    //case RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_BAO_CHE_THUOC__MPS000244:
                    //    InPhieuXuatBaoCheThuoc(printTypeCode, fileName, ref result);
                    //    break;
                    case RequestUri.PRINT_TYPE_CODE__PHIEU_IN_TEM_DON_MAU_Mps000422:
                        InPhieuTemDonMau(printTypeCode, fileName, ref result);
                        break;
                    //case RequestUri.PRINT_TYPE_CODE__DON_TONG_HOP_Mps000234:
                    //    InDonTongHop(printTypeCode, fileName, ref result);
                    //    break;
                    case RequestUri.PRINT_TYPE_CODE_IN_DON_THUOC_Mps000044:
                        // InDonThuoc(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092:
                        InPhieuXuatBan(printTypeCode, fileName, ref result);
                        break;

                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatSuDung_MPS000135:
                        InPhieuXuatSuDung(printTypeCode, fileName, ref result);
                        break;

                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatSuDungTheoDieuKien_MPS000134:
                        InPhieuXuatTheoDieuKien(printTypeCode, fileName, ref result);
                        //Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                        //store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatSuDungTheoDieuKien_MPS000134, delegateRunTemplate);
                        break;
                    case "Mps000494":
                        PrintMps494(printTypeCode, fileName, ref result);
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
        private void PrintMps494(string printTypeCode, string fileName, ref bool result)
        {
            try
            {

                if (_CurrentExpMest != null && _CurrentExpMest.ID > 0)
                {
                    HisExpMestMaterialFilter filter = new HisExpMestMaterialFilter();
                    filter.EXP_MEST_ID = _CurrentExpMest.ID;

                    var lstMatePrintMps494 = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, filter, null);
                    if (lstMatePrintMps494 == null || lstMatePrintMps494.Count == 0)
                    {
                        XtraMessageBox.Show("Không có vật tư tái sử dụng hoặc vật tư tái sử dụng đã hết số lần tái sử dụng.");
                        return;
                    }
                    var expmestMate = lstMatePrintMps494.Where(o => !string.IsNullOrEmpty(o.SERIAL_NUMBER) && o.REMAIN_REUSE_COUNT != null).ToList();
                    if (expmestMate == null || expmestMate.Count == 0)
                    {
                        XtraMessageBox.Show("Không có vật tư tái sử dụng hoặc vật tư tái sử dụng đã hết số lần tái sử dụng.");
                        return;
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
                            ado.NEXT_REUSABLE_NUMBER = ((te.MAX_REUSE_COUNT ?? 0) - (m.REMAIN_REUSE_COUNT ?? 0) + 2).ToString();
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
                        return;
                    }
                    MPS.Processor.Mps000494.PDO.Mps000494PDO rdo = new MPS.Processor.Mps000494.PDO.Mps000494PDO(lstSend);

                    RunPrint(printTypeCode, fileName, rdo, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, moduleData.RoomId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void EventLogPrint()
        {
            try
            {
                string message = "In tem vật tư tái sử dụng. Mã in : Mps000494" + "  TREATMENT_CODE: " + _CurrentExpMest.TDL_TREATMENT_CODE + "  Thời gian in: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + "  Người in: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, roomId);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName, EventLogPrint) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InDonThuocTongHop(string printTypeCode)
        {
            List<V_HIS_EXP_MEST> listExpMest = new List<V_HIS_EXP_MEST>();
            List<HIS_SERVICE_REQ> serviceReqPrints = new List<HIS_SERVICE_REQ>();
            List<HIS_EXP_MEST> expMestPrints = new List<HIS_EXP_MEST>();
            try
            {
                if (this._CurrentExpMest != null)
                {
                    HIS_EXP_MEST expMest = new HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(expMest, _CurrentExpMest);
                    expMestPrints.Add(expMest);
                }

                //lấy đơn thuốc tư phiếu xuất
                if (_CurrentExpMest != null && _CurrentExpMest.SERVICE_REQ_ID != null)
                {
                    CommonParam parama = new CommonParam();
                    HisServiceReqFilter HisServiceReq = new HisServiceReqFilter();
                    HisServiceReq.ID = _CurrentExpMest.SERVICE_REQ_ID;
                    serviceReqPrints = new BackendAdapter(parama).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, HisServiceReq, parama);
                }

                //số lượng đơn thuốc với số lượng phiếu xuất khác nhau sẽ không in được
                if (serviceReqPrints.Count < expMestPrints.Count)
                {
                    int dem = 0;
                    foreach (var item in expMestPrints)
                    {
                        //không có thông tin đơn thuốc thì tạo thông tin đơn thuốc.
                        if (!item.SERVICE_REQ_ID.HasValue)
                        {
                            dem--;
                            item.SERVICE_REQ_ID = dem;

                            HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                            req.ID = item.SERVICE_REQ_ID ?? 0;
                            req.TREATMENT_ID = item.TDL_TREATMENT_ID ?? 0;
                            req.PARENT_ID = item.TDL_PATIENT_ID;
                            serviceReqPrints.Add(req);
                        }
                        else if (!serviceReqPrints.Exists(o => o.ID == item.SERVICE_REQ_ID))
                        {
                            //nếu có service_req_id mà không get được thông tin thì cũng tạo đơn giả để in
                            HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                            req.ID = item.SERVICE_REQ_ID ?? 0;
                            req.TREATMENT_ID = item.TDL_TREATMENT_ID ?? 0;
                            req.PARENT_ID = item.TDL_PATIENT_ID;
                            serviceReqPrints.Add(req);
                        }
                    }
                }

                MOS.SDO.OutPatientPresResultSDO sdo = new MOS.SDO.OutPatientPresResultSDO();
                sdo.ExpMests = expMestPrints;
                sdo.ServiceReqs = serviceReqPrints;

                Library.PrintPrescription.PrintPrescriptionProcessor processPress = new Library.PrintPrescription.PrintPrescriptionProcessor(new List<MOS.SDO.OutPatientPresResultSDO>() { sdo }, this.currentModuleBase);
                processPress.Print(MPS.Processor.Mps000118.PDO.Mps000118PDO.PrintTypeCode, false);
            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                //result = false;
            }
        }
        private void InPhieuXuatTheoDieuKien(string printTypeCode, string fileName, ref bool result)
        {
            result = false;
            try
            {
                //Review
                // WaitingManager.Show();
                HisExpMestViewFilter depaFilter = new HisExpMestViewFilter();
                depaFilter.ID = _CurrentExpMest.ID;
                var listDepaExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, depaFilter, null);
                if (listDepaExpMest == null || listDepaExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc DepaExpMest bang ID");
                var _ExpMest = listDepaExpMest.First();


                List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                mediFilter.EXP_MEST_ID = _CurrentExpMest.ID;
                _ExpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);

                MOS.Filter.HisExpMestMaterialViewFilter mateFilter = new HisExpMestMaterialViewFilter();
                mateFilter.EXP_MEST_ID = _CurrentExpMest.ID;
                _ExpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, param);

                List<V_HIS_EXP_MEST_MEDICINE> listMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> listMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();

                List<HIS_EXP_MEST_METY_REQ> listMetyReq = new List<HIS_EXP_MEST_METY_REQ>();
                List<HIS_EXP_MEST_MATY_REQ> listMatyReq = new List<HIS_EXP_MEST_MATY_REQ>();


                listMedicine = _ExpMestMedicines;
                listMaterial = _ExpMestMaterials;

                MOS.Filter.HisExpMestMetyReqFilter mediFilter1 = new HisExpMestMetyReqFilter();
                mediFilter1.EXP_MEST_ID = _CurrentExpMest.ID;
                listMetyReq = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/GET", ApiConsumers.MosConsumer, mediFilter1, param);

                MOS.Filter.HisExpMestMatyReqFilter mateFilter1 = new HisExpMestMatyReqFilter();
                mateFilter1.EXP_MEST_ID = _CurrentExpMest.ID;
                listMatyReq = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/GET", ApiConsumers.MosConsumer, mateFilter1, param);



                //Review

                MPS.Processor.Mps000134.PDO.Mps000134PDO mps000134RDO = new MPS.Processor.Mps000134.PDO.Mps000134PDO(
                listMetyReq,
                listMatyReq,
                listMedicine,
                listMaterial,
                _ExpMest,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                null
                );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000134RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000134RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
                //if (result)
                //{
                //    this.Close();
                //}
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            //return result;


        }

        private bool delegateRunTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                //Review
                // WaitingManager.Show();
                HisExpMestViewFilter depaFilter = new HisExpMestViewFilter();
                depaFilter.ID = this.resultSdo.ExpMest.ID;
                var listDepaExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, depaFilter, null);
                if (listDepaExpMest == null || listDepaExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc DepaExpMest bang ID");
                var _ExpMest = listDepaExpMest.First();


                List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                mediFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                _ExpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);

                MOS.Filter.HisExpMestMaterialViewFilter mateFilter = new HisExpMestMaterialViewFilter();
                mateFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                _ExpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, param);

                List<V_HIS_EXP_MEST_MEDICINE> listMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> listMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();

                List<HIS_EXP_MEST_METY_REQ> listMetyReq = new List<HIS_EXP_MEST_METY_REQ>();
                List<HIS_EXP_MEST_MATY_REQ> listMatyReq = new List<HIS_EXP_MEST_MATY_REQ>();


                listMedicine = _ExpMestMedicines;
                listMaterial = _ExpMestMaterials;
                listMetyReq = this.resultSdo.ExpMetyReqs;
                listMatyReq = this.resultSdo.ExpMatyReqs;

                //Review

                MPS.Processor.Mps000134.PDO.Mps000134PDO mps000134RDO = new MPS.Processor.Mps000134.PDO.Mps000134PDO(
                listMetyReq,
                listMatyReq,
                listMedicine,
                listMaterial,
                _ExpMest,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                null
                );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000134RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000134RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
                //if (result)
                //{
                //    this.Close();
                //}
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void InDonTongHop()
        {
            try
            {
                if (this._CurrentExpMest == null)
                {
                    throw new ArgumentNullException("ExpMest is null");
                }
                CommonParam param = new CommonParam();

                //Load expmest
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.ID = this._CurrentExpMest.ID;
                List<HIS_EXP_MEST> expMests = new BackendAdapter(param)
                     .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);

                if (expMests == null || expMests.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("expMests is null");
                    return;
                }

                //Load đơn phòng khám
                var serviceReqIds = expMests.Where(p => p.SERVICE_REQ_ID != null).Select(o => o.SERVICE_REQ_ID ?? 0);
                if (serviceReqIds == null || serviceReqIds.Count() == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("serviceReqIds is null");
                    return;
                }

                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.IDs = serviceReqIds.Distinct().ToList();
                //serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                List<HIS_SERVICE_REQ> serviceReqs = new BackendAdapter(param)
                   .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                if (serviceReqs == null || serviceReqs.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("serviceReqs is null");
                    return;
                }

                //Laays thuoc va tu trong kho

                HisExpMestMedicineFilter expMestMedicineFilter = new HisExpMestMedicineFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                List<HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                List<HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                LoadServiceReqMety();
                LoadServiceReqMaty();

                List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
                if ((expMestMedicines != null && expMestMedicines.Count > 0)
                            || (expMestMaterials != null && expMestMaterials.Count > 0))
                {
                    OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
                    outPatientPresResultSDO.ExpMests = expMests;
                    outPatientPresResultSDO.ServiceReqs = serviceReqs;
                    outPatientPresResultSDO.Medicines = expMestMedicines;
                    outPatientPresResultSDO.Materials = expMestMaterials;
                    outPatientPresResultSDO.ServiceReqMaties = this.ServiceReqMatys;
                    outPatientPresResultSDO.ServiceReqMeties = this.ServiceReqMetys;
                    OutPatientPresResultSDOForPrints.Add(outPatientPresResultSDO);
                }
                HIS_EXP_MEST expMest = new HIS_EXP_MEST();
                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST, HIS_EXP_MEST>();
                expMest = AutoMapper.Mapper.Map<HIS_EXP_MEST>(this._CurrentExpMest);
                PrintPrescriptionProcessor printPrescriptionProcessor = new PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, expMest);

                printPrescriptionProcessor.Print(RequestUri.PRINT_TYPE_CODE__DON_TONG_HOP_Mps000234, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatBaoCheThuoc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                V_HIS_IMP_MEST impMest = new V_HIS_IMP_MEST();
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                if (this._CurrentExpMest != null)
                {
                    MOS.Filter.HisImpMestViewFilter impMestViewFilter = new HisImpMestViewFilter();
                    //impMestViewFilter.PREPARATION_EXP_MEST_ID = this._CurrentExpMest.ID;
                    impMest = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, impMestViewFilter, param).FirstOrDefault();
                }
                if (impMest != null && impMest.ID > 0)
                {
                    MOS.Filter.HisImpMestMedicineViewFilter impMestMedicineViewFilter = new HisImpMestMedicineViewFilter();
                    impMestMedicineViewFilter.IMP_MEST_ID = impMest.ID;
                    impMestMedicines = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, impMestMedicineViewFilter, param);
                }

                if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL_1, V_HIS_EXP_MEST_MATERIAL>();
                    expMestMaterials = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_MATERIAL>>(this._ExpMestMaterials);
                }

                //MPS.Processor.Mps000244.PDO.Mps000244PDO rdo = new MPS.Processor.Mps000244.PDO.Mps000244PDO(
                //      this._CurrentExpMest,
                //      impMest,
                //      impMestMedicines,
                //      expMestMaterials);

                //MPS.ProcessorBase.Core.PrintData PrintData = null;
                //if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                //}
                //else
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                //}
                WaitingManager.Hide();
                //result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatBuThuocLe(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                MOS.Filter.HisExpMestViewFilter e1 = new HisExpMestViewFilter();

                ProcessPrint(printTypeCode);

                MPS.Processor.Mps000216.PDO.Mps000216PDO rdo = new MPS.Processor.Mps000216.PDO.Mps000216PDO(
                    this._CurrentExpMest, this._ExpMestMedicines_Print, this._ExpMestMaterials_Print);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }
        private void InDonThuoc(string printTypeCode)
        {
            List<V_HIS_EXP_MEST> listExpMest = new List<V_HIS_EXP_MEST>();
            List<HIS_SERVICE_REQ> serviceReqPrints = new List<HIS_SERVICE_REQ>();
            List<HIS_EXP_MEST> expMestPrints = new List<HIS_EXP_MEST>();

            try
            {
                bool a = false;
                // ProcessPrint(printTypeCode);
                //if (_CurrentExpMest != null)



                if (this._CurrentExpMest != null)
                {
                    //listExpMest = this._CurrentExpMest.Select(s => s.ExpMest).ToList();

                    //foreach (var item in listExpMest)
                    //{
                    HIS_EXP_MEST expMest = new HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(expMest, _CurrentExpMest);
                    expMestPrints.Add(expMest);
                }

                //lấy đơn thuốc tư phiếu xuất
                if (_CurrentExpMest != null && _CurrentExpMest.TDL_SERVICE_REQ_CODE != null)
                {
                    CommonParam parama = new CommonParam();
                    HisServiceReqFilter HisServiceReq = new HisServiceReqFilter();
                    HisServiceReq.SERVICE_REQ_CODE__EXACT = _CurrentExpMest.TDL_SERVICE_REQ_CODE;
                    serviceReqPrints = new BackendAdapter(parama).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, HisServiceReq, parama);
                }

                //số lượng đơn thuốc với số lượng phiếu xuất khác nhau sẽ không in được
                if (serviceReqPrints.Count < expMestPrints.Count)
                {
                    int dem = 0;
                    foreach (var item in expMestPrints)
                    {
                        //không có thông tin đơn thuốc thì tạo thông tin đơn thuốc.
                        if (!item.SERVICE_REQ_ID.HasValue)
                        {
                            dem--;
                            item.SERVICE_REQ_ID = dem;

                            HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                            req.ID = item.SERVICE_REQ_ID ?? 0;
                            req.TREATMENT_ID = item.TDL_TREATMENT_ID ?? 0;
                            req.PARENT_ID = item.TDL_PATIENT_ID;
                            serviceReqPrints.Add(req);
                        }
                        else if (!serviceReqPrints.Exists(o => o.ID == item.SERVICE_REQ_ID))
                        {
                            //nếu có service_req_id mà không get được thông tin thì cũng tạo đơn giả để in
                            HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                            req.ID = item.SERVICE_REQ_ID ?? 0;
                            req.TREATMENT_ID = item.TDL_TREATMENT_ID ?? 0;
                            req.PARENT_ID = item.TDL_PATIENT_ID;
                            serviceReqPrints.Add(req);
                        }
                    }
                }


                //if (_CurrentExpMest == null)
                //{
                //    serviceReqPrints = dataServiceReqs;
                //}



                MOS.SDO.OutPatientPresResultSDO sdo = new MOS.SDO.OutPatientPresResultSDO();
                sdo.ExpMests = expMestPrints;
                sdo.ServiceReqs = serviceReqPrints;

                Library.PrintPrescription.PrintPrescriptionProcessor processPress = new Library.PrintPrescription.PrintPrescriptionProcessor(new List<MOS.SDO.OutPatientPresResultSDO>() { sdo }, this.currentModuleBase);
                processPress.Print(MPS.Processor.Mps000044.PDO.Mps000044PDO.PrintTypeCode, false);
                //ProcessPrint(printTypeCode);
            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                //result = false;
            }
        }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicinesBCS { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterialsBCS { get; set; }
        List<HIS_TREATMENT> ListTreatment { get; set; }
        HisExpMestBcsMoreInfoSDO MoreInfo { get; set; }

        private void InPhieuXuatBuCoSoTuTruc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("Config.HisConfigCFG.ODER_OPTION", Config.HisConfigCFG.ODER_OPTION));
                #region TT Chung
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                ProcessPrint(printTypeCode);
                _ExpMestMedicinesBCS = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterialsBCS = new List<V_HIS_EXP_MEST_MATERIAL>();
                if (this._ExpMestMedicines_Print != null && this._ExpMestMedicines_Print.Count > 0)
                {
                    _ExpMestMedicinesBCS = this._ExpMestMedicines_Print.Where(o => (o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                || o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)).ToList();
                }
                if (this._ExpMestMaterials_Print != null && this._ExpMestMaterials_Print.Count > 0)
                {
                    _ExpMestMaterialsBCS = this._ExpMestMaterials_Print.Where(o => (o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                || o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)).ToList();
                }

                ListTreatment = new List<HIS_TREATMENT>();
                List<long> treatmentIds = new List<long>();
                treatmentIds.AddRange(_ExpMestMetyReqs_Print.Select(s => s.TREATMENT_ID ?? 0).ToList());
                treatmentIds.AddRange(_ExpMestMatyReqs_Print.Select(s => s.TREATMENT_ID ?? 0).ToList());

                if (treatmentIds != null && treatmentIds.Count > 0)
                {
                    int skip = 0;
                    while (treatmentIds.Count - skip > 0)
                    {
                        var listIds = treatmentIds.Skip(skip).Take(100).ToList();
                        skip += 100;

                        MOS.Filter.HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                        treatFilter.IDs = listIds;
                        var treat = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatFilter, param);
                        if (treat != null && treat.Count > 0)
                        {
                            ListTreatment.AddRange(treat);
                        }
                    }
                }

                if (this._CurrentExpMest != null &&
                    (this._CurrentExpMest.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES
                    || this._CurrentExpMest.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES_DETAIL))
                {
                    HisExpMestBcsMoreInfoFilter moreFilter = new HisExpMestBcsMoreInfoFilter();
                    moreFilter.BCS_EXP_MEST_ID = this._CurrentExpMest.ID;
                    MoreInfo = new BackendAdapter(new CommonParam()).Get<HisExpMestBcsMoreInfoSDO>("api/HisExpMest/GetBcsMoreInfo", ApiConsumers.MosConsumer, moreFilter, null);
                }
                #endregion

                {
                    _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DCHTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DCGNs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DC_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_TCs = new List<HIS_EXP_MEST_METY_REQ>();

                    List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();

                    #region --- Xu Ly Tach GN_HT -----
                    if (_ExpMestMetyReqs_Print != null && _ExpMestMetyReqs_Print.Count > 0)
                    {
                        var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                        var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                        bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                        bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                        var IsSeparatePrintingGN = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN).IS_SEPARATE_PRINTING ?? 0;
                        var IsSeparatePrintingHT = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT).IS_SEPARATE_PRINTING ?? 0;
                        bool dcgn = IsSeparatePrintingGN == 1 && mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN);
                        bool dcht = IsSeparatePrintingHT == 1 && mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT);
                        bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                        bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                        bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                        bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                        bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                        bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);
                        bool tc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC);

                        var mediTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                        foreach (var item in _ExpMestMetyReqs_Print)
                        {
                            var dataMedi = mediTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                            if (dataMedi != null)
                            {
                                if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_GNs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_HTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN && dcgn)
                                {
                                    _ExpMestMetyReq_DC_GN_HTs.Add(item);
                                    _ExpMestMetyReq_DCGNs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT && dcht)
                                {
                                    _ExpMestMetyReq_DC_GN_HTs.Add(item);
                                    _ExpMestMetyReq_DCHTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                                {
                                    _ExpMestMetyReq_TDs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                                {
                                    _ExpMestMetyReq_PXs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                                {
                                    _ExpMestMetyReq_COs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                                {
                                    _ExpMestMetyReq_DTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                                {
                                    _ExpMestMetyReq_KSs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                                {
                                    _ExpMestMetyReq_LAOs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC && tc)
                                {
                                    _ExpMestMetyReq_TCs.Add(item);

                                }
                                else
                                {
                                    _ExpMestMetyReq_Ts.Add(item);
                                }
                            }
                        }
                    }
                    #endregion

                    WaitingManager.Hide();
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    richEditorMain.RunPrintTemplate("Mps000254", DelegateRunMps);

                    #region ----VatTu----
                    if (_ExpMestMatyReqs != null && _ExpMestMatyReqs.Count > 0)
                    {
                        WaitingManager.Show();
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                        (
                         this._CurrentExpMest,
                         null,
                         _ExpMestMaterialsBCS,
                         null,
                         _ExpMestMatyReqs_Print,
                         BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                         MPS.Processor.Mps000215.PDO.keyTitles.vattu,
                         ListTreatment,
                         MoreInfo,
                         Config.HisConfigCFG.ODER_OPTION
                         );

                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ----- Thuong ----
                    if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                    {
                        WaitingManager.Show();
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                        (
                          this._CurrentExpMest,
                         _ExpMestMedicinesBCS,
                         null,
                         _ExpMestMetyReq_Ts,
                         null,
                         BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                         MPS.Processor.Mps000215.PDO.keyTitles.thuong,
                         ListTreatment,
                         MoreInfo,
                         Config.HisConfigCFG.ODER_OPTION
                         );

                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion


                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatChuyenKhoMau(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                ProcessPrint(printTypeCode);

                List<HIS_EXP_MEST_BLTY_REQ> expMestBltyReqs = new List<HIS_EXP_MEST_BLTY_REQ>();
                foreach (var expMestBltyReq1 in this._ExpMestBltyReqs_Print)
                {
                    HIS_EXP_MEST_BLTY_REQ expMestBltyReq = new HIS_EXP_MEST_BLTY_REQ();
                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_BLTY_REQ_1, HIS_EXP_MEST_BLTY_REQ>();
                    expMestBltyReq = AutoMapper.Mapper.Map<HIS_EXP_MEST_BLTY_REQ>(expMestBltyReq1);
                    expMestBltyReqs.Add(expMestBltyReq);
                }
                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                if (expMestBltyReqs != null && expMestBltyReqs.Count > 0)
                {
                    string keyAddictive = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyAddictive = "PHIẾU TRẢ MÁU";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyAddictive = "PHIẾU LĨNH MÁU";
                    }
                    MPS.Processor.Mps000198.PDO.Mps000198PDO mps000198PDO = new MPS.Processor.Mps000198.PDO.Mps000198PDO
                 (
                  this._CurrentExpMest,
                 expMestBltyReqs,
                 null,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                 BackendDataWorker.Get<HIS_BLOOD_ABO>(),
                 BackendDataWorker.Get<HIS_BLOOD_RH>(),
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 keyAddictive,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatMatMat(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                ProcessPrint(printTypeCode);
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestView1Filter expMestViewFilter = new MOS.Filter.HisExpMestView1Filter();
                expMestViewFilter.ID = this._CurrentExpMest.ID;
                var expMest1 = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_1>>("api/HisExpMest/GetView1", ApiConsumer.ApiConsumers.MosConsumer, expMestViewFilter, param).FirstOrDefault();

                WaitingManager.Show();
                MPS.Processor.Mps000168.PDO.Mps000168PDO mps000168PDO = new MPS.Processor.Mps000168.PDO.Mps000168PDO(
                    this._ExpMestMedicines_Print,
                    this._ExpMestMaterials_Print,
                    expMest1
                    );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000168PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000168PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000168PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuXuatKhacMau(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                ProcessPrint(printTypeCode);
                if (this._ExpMestBloods != null && this._ExpMestBloods.Count > 0)
                {
                    WaitingManager.Show();
                    List<V_HIS_EXP_MEST_BLOOD> expMestBloods = new List<V_HIS_EXP_MEST_BLOOD>();
                    foreach (var item in this._ExpMestBloods)
                    {
                        V_HIS_EXP_MEST_BLOOD expMestBlood = new V_HIS_EXP_MEST_BLOOD();
                        AutoMapper.Mapper.CreateMap<ExpMestBloodADODetail, V_HIS_EXP_MEST_BLOOD>();
                        expMestBlood = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_BLOOD>(item);
                        expMestBloods.Add(expMestBlood);
                    }

                    var distinctBloodGiveIds = expMestBloods
                         .Where(s => s.BLOOD_GIVE_ID != null)
                         .Select(s => s.BLOOD_GIVE_ID)
                         .Distinct()
                         .ToList();
                    lstBloodGiver = BackendDataWorker.Get<HIS_BLOOD_GIVER>().Where(s => distinctBloodGiveIds.Contains(s.ID)).ToList();
                    //GetBloodGiver(distinctBloodGiveIds);
                    // TODO
                    //var bloods = dicMediMateAdo.Select(s => s.Value).Where(o => o.IsBlood == true).ToList();

                    //foreach (var item in bloods)
                    //{
                    //    listExpBloods.FirstOrDefault(o => o.BLOOD_ID == item.BLOOD_ID).DESCRIPTION = item.ExpBlood.Description;
                    //}

                    MPS.Processor.Mps000203.PDO.Mps000203ADO mps000203Ado = new MPS.Processor.Mps000203.PDO.Mps000203ADO();
                    if (this._CurrentExpMest.EXP_MEST_REASON_ID != null)
                        mps000203Ado.EXP_REASON_NAME = BackendDataWorker.Get<HIS_EXP_MEST_REASON>().FirstOrDefault(o => o.ID == this._CurrentExpMest.EXP_MEST_REASON_ID).EXP_MEST_REASON_NAME;
                    if (this._CurrentExpMest.IMP_MEDI_STOCK_ID != null)
                    {
                        mps000203Ado.IMP_MEDI_STOCK_CODE = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this._CurrentExpMest.IMP_MEDI_STOCK_ID).MEDI_STOCK_CODE;
                        mps000203Ado.IMP_MEDI_STOCK_NAME = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this._CurrentExpMest.IMP_MEDI_STOCK_ID).MEDI_STOCK_NAME;
                    }

                    MPS.Processor.Mps000203.PDO.Mps000203PDO mps000203PDO = new MPS.Processor.Mps000203.PDO.Mps000203PDO
                    (
                     this._CurrentExpMest,
                     expMestBloods,
                     mps000203Ado,
                     lstBloodGiver
                      );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000203PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000203PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000203PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
                result = false;
            }
        }

        //private void GetBloodGiver (List<long?> bloodGiveIds)
        //{
        //    try
        //    {
        //        CommonParam param = new CommonParam();
        //        HisBloodGiverFilter filter = new HisBloodGiverFilter();
        //        filter.IMP_MEST_IDs
        //        lstBloodGiver = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_GIVER>>("api/HisBloodGiver/Get", ApiConsumers.MosConsumer, bloodGiveIds, param);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        private void InPhieuXuatKhac(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                ProcessPrint(printTypeCode);

                MPS.Processor.Mps000165.PDO.Mps000165PDO rdo = new MPS.Processor.Mps000165.PDO.Mps000165PDO(
                    this._CurrentExpMest, this._ExpMestMedicines_Print, this._ExpMestMaterials_Print);
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }

        private void InPhieuXuatDonTongHopThuocVatTu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                ProcessPrint(printTypeCode);
                //Thong tin thuoc / vat tu
                if (this._CurrentExpMest != null && this._CurrentExpMest.SERVICE_REQ_ID > 0)
                {
                    HIS_EXP_MEST _expMest = new HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(_expMest, this._CurrentExpMest);

                    MOS.Filter.HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                    reqFilter.ID = this._CurrentExpMest.SERVICE_REQ_ID;
                    var _serviceReq = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, reqFilter, null).FirstOrDefault();
                    Library.PrintPrescription.PrintPrescriptionProcessor processPress;

                    if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK)
                    {
                        MOS.SDO.InPatientPresResultSDO sdo = new MOS.SDO.InPatientPresResultSDO();
                        sdo.ExpMests = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>() { _expMest };
                        sdo.ServiceReqs = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>() { _serviceReq };
                        processPress = new Library.PrintPrescription.PrintPrescriptionProcessor(new List<MOS.SDO.InPatientPresResultSDO>() { sdo }, currentModuleBase);
                    }
                    else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT ||
                        this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                    {
                        MOS.SDO.OutPatientPresResultSDO sdo = new MOS.SDO.OutPatientPresResultSDO();
                        sdo.ExpMests = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>() { _expMest };
                        sdo.ServiceReqs = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>() { _serviceReq };
                        processPress = new Library.PrintPrescription.PrintPrescriptionProcessor(new List<MOS.SDO.OutPatientPresResultSDO>() { sdo }, currentModuleBase);
                    }
                    else
                    {
                        processPress = new Library.PrintPrescription.PrintPrescriptionProcessor(new List<MOS.SDO.OutPatientPresResultSDO>(), currentModuleBase);
                    }

                    processPress.Print(MPS.Processor.Mps000118.PDO.Mps000118PDO.PrintTypeCode, false);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuXuatThanhLy(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                ProcessPrint(printTypeCode);
                WaitingManager.Show();

                // get expMest
                MPS.Processor.Mps000155.PDO.Mps000155PDO pdo = new MPS.Processor.Mps000155.PDO.Mps000155PDO(
                    this._CurrentExpMest,
                    this._ExpMestMedicines_Print,
                    this._ExpMestMaterials_Print
                    );

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuXuatHaoPhi(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                ProcessPrint(printTypeCode);
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                MPS.Processor.Mps000154.PDO.Mps000154PDO pdo = new MPS.Processor.Mps000154.PDO.Mps000154PDO(
                    this._CurrentExpMest,
                    this._ExpMestMedicines_Print,
                    this._ExpMestMaterials_Print
                    );

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuXuatSuDung(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
                ProcessPrint(printTypeCode);
                CommonParam param = new CommonParam();
                var _ExpMest = this._CurrentExpMest;
                List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloods = new List<V_HIS_EXP_MEST_BLOOD>();
                List<HIS_EXP_MEST_BLTY_REQ> lstMestBltyReq = new List<HIS_EXP_MEST_BLTY_REQ>();

                MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                mediFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                _ExpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);

                MOS.Filter.HisExpMestMaterialViewFilter mateFilter = new HisExpMestMaterialViewFilter();
                mateFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                _ExpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, param);

                MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                bloodFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                _ExpMestBloods = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestBloods), _ExpMestBloods));

                MOS.Filter.HisExpMestBltyReqFilter bltyReqFilter = new HisExpMestBltyReqFilter();
                bltyReqFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                lstMestBltyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/Get", ApiConsumers.MosConsumer, bltyReqFilter, param);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstMestBltyReq), lstMestBltyReq));

                //Review
                //if (keyPrintType == 1)
                //{
                //    MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                //   this._ExpMestMetyReqs_Print,
                //    this._ExpMestMatyReqs_Print,
                //     this._ExpMestMedicines_Print,
                //    this._ExpMestMaterials_Print,
                //    _ExpMest,
                //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                //    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                //    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                //    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuTongHop
                //                );
                //    MPS.ProcessorBase.Core.PrintData PrintData = null;
                //    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                //    }
                //    else
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                //    }
                //    result = MPS.MpsPrinter.Run(PrintData);
                //}
                //else
                {
                    // tách riêng gây nghiện hướng thần thành một bản in
                    _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DCHTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DCGNs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DC_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                    if (this._ExpMestMetyReqs_Print != null && this._ExpMestMetyReqs_Print.Count > 0)
                    {
                        var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                        var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                        bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                        bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                        var IsSeparatePrintingGN = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN).IS_SEPARATE_PRINTING ?? 0;
                        var IsSeparatePrintingHT = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT).IS_SEPARATE_PRINTING ?? 0;
                        bool dcgn = IsSeparatePrintingGN == 1 && mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN);
                        bool dcht = IsSeparatePrintingHT == 1 && mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT);
                        bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                        bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                        bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                        bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                        bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                        bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);

                        var mediTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                        foreach (var item in this._ExpMestMetyReqs_Print)
                        {
                            var dataMedi = mediTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                            if (dataMedi != null)
                            {
                                if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_HTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_GNs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN && dcgn)
                                {
                                    _ExpMestMetyReq_DC_GN_HTs.Add(item);
                                    _ExpMestMetyReq_DCGNs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT && dcht)
                                {
                                    _ExpMestMetyReq_DC_GN_HTs.Add(item);
                                    _ExpMestMetyReq_DCHTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                                {
                                    _ExpMestMetyReq_TDs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                                {
                                    _ExpMestMetyReq_PXs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                                {
                                    _ExpMestMetyReq_COs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                                {
                                    _ExpMestMetyReq_DTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                                {
                                    _ExpMestMetyReq_KSs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                                {
                                    _ExpMestMetyReq_LAOs.Add(item);
                                }
                                else
                                {
                                    _ExpMestMetyReq_Ts.Add(item);
                                }
                            }
                        }
                    }

                    long keyGOP = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                    if (keyGOP == 1)
                    {
                        #region ---- GN_HT ----
                        if (_ExpMestMetyReq_GN_HTs != null && _ExpMestMetyReq_GN_HTs.Count > 0)
                        {
                            MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                                _ExpMestMetyReq_GN_HTs,
                                null,
                                _ExpMestMedicines,
                                null,
                                _ExpMest,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                                MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuGN_HT
                                );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        #endregion
                    }
                    else
                    {
                        #region ---- GN ----
                        if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                        {
                            MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                                _ExpMestMetyReq_GNs,
                                null,
                                _ExpMestMedicines,
                                null,
                                _ExpMest,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                                MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuGayNghien
                                );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        #endregion

                        #region ---- HT ----
                        if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                        {
                            MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                                _ExpMestMetyReq_HTs,
                                null,
                                _ExpMestMedicines,
                                null,
                                _ExpMest,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                               MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuHuongThan
                                );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        #endregion
                    }

                    #region ---- TD ----
                    if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                                _ExpMestMetyReq_TDs,
                                null,
                                _ExpMestMedicines,
                                null,
                                _ExpMest,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                                MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuThuocDoc
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- PX ----
                    if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                            _ExpMestMetyReq_PXs,
                            null,
                            _ExpMestMedicines,
                            null,
                            _ExpMest,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                            BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                            MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuThuocPhongXa
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- T ----
                    if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                            _ExpMestMetyReq_Ts,
                            null,
                            _ExpMestMedicines,
                            null,
                            _ExpMest,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                            BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                            MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuThuocThuong
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- VT ----
                    if (this._ExpMestMatyReqs_Print != null && this._ExpMestMatyReqs_Print.Count > 0)
                    {
                        var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_VTs = new List<HIS_EXP_MEST_MATY_REQ>();
                        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                        foreach (var item in this._ExpMestMatyReqs_Print)
                        {
                            var dataMaty = _materialTypes.FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                            if (dataMaty != null && dataMaty.IS_CHEMICAL_SUBSTANCE == 1)
                            {
                                _ExpMestMatyReq_HCs.Add(item);
                            }
                            else
                                _ExpMestMatyReq_VTs.Add(item);
                        }

                        #region ---- VT ----
                        if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count > 0)
                        {
                            MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    null,
                    _ExpMestMatyReq_VTs,
                    null,
                    _ExpMestMaterials,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuVatTu
                                );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        #endregion

                        #region ---- HC ----
                        if (_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0)
                        {
                            MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    null,
                    _ExpMestMatyReq_HCs,
                    null,
                    _ExpMestMaterials,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuHoaChat
                                );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        #endregion
                    }
                    #endregion

                    #region Mau

                    if (lstMestBltyReq != null && lstMestBltyReq.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
               null,
               null,
               lstMestBltyReq,
               null,
               null,
               _ExpMestBloods,
               _ExpMest,
               IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
               IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
               BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
               BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
               BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
               MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.Mau
                           );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    #endregion

                    #region ---- CO ----
                    if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                            _ExpMestMetyReq_COs,
                            null,
                            _ExpMestMedicines,
                            null,
                            _ExpMest,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                            BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                            MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.Corticoid
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- DT ----
                    if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                            _ExpMestMetyReq_DTs,
                            null,
                            _ExpMestMedicines,
                            null,
                            _ExpMest,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                            BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                            MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.DichTruyen
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- KS ----
                    if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                            _ExpMestMetyReq_KSs,
                            null,
                            _ExpMestMedicines,
                            null,
                            _ExpMest,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                            BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                            MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.KhangSinh
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- LAO ----
                    if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                            _ExpMestMetyReq_LAOs,
                            null,
                            _ExpMestMedicines,
                            null,
                            _ExpMest,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                            BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                            MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.Lao
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                ProcessPrint(printTypeCode);
                WaitingManager.Show();
                MPS.Processor.Mps000090.PDO.Mps000090PDO pdo = new MPS.Processor.Mps000090.PDO.Mps000090PDO(
                 this._CurrentExpMest,
                 this._ExpMestMedicines_Print,
                 this._ExpMestMaterials_Print
                 );

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuXuatChuyenKhoThuocGayNghienHuongThan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                ProcessPrint(printTypeCode);
                WaitingManager.Show();
                MPS.Processor.Mps000089.PDO.Mps000089PDO pdo = new MPS.Processor.Mps000089.PDO.Mps000089PDO(
                 this._CurrentExpMest,
                 this._ExpMestMedicines_Print
                 );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InHuongDanSuDung(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                ProcessPrint(printTypeCode);
                WaitingManager.Show();
                string genderName = "";
                long? dob = null;
                if (this._CurrentExpMest.TDL_PATIENT_ID != null)
                {
                    HisPatientViewFilter Filter = new HisPatientViewFilter();
                    Filter.ID = this._CurrentExpMest.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, Filter, param);
                    if (patients != null && patients.Count > 0)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_PATIENT patient = patients.FirstOrDefault();
                        genderName = patient.GENDER_NAME;
                        dob = patient.DOB;
                    }
                }
                List<V_HIS_EXP_MEST> listExtMest = new List<V_HIS_EXP_MEST>();
                MOS.Filter.HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this._CurrentExpMest.ID;
                listExtMest = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, new CommonParam());

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Dữ liệu this.listExtMest: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => listExtMest), listExtMest));
                MPS.Processor.Mps000099.PDO.Mps000099PDO pdo = new MPS.Processor.Mps000099.PDO.Mps000099PDO(
                 listExtMest,
                 this._ExpMestMedicines_Print,
                 this._ExpMestMaterials_Print
                 );

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);


            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuYeuCauDonDuocMau(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                if (this._CurrentExpMest != null)
                {
                    ProcessPrint(printTypeCode);
                    MOS.Filter.HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                    filter.ID = this._CurrentExpMest.SERVICE_REQ_ID;
                    var serviceReqs = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                    if (serviceReqs != null && serviceReqs.Count > 0)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ serviceReq = null;
                        serviceReq = serviceReqs.FirstOrDefault();
                        WaitingManager.Show();
                        HIS_EXP_MEST evExpMest = new HIS_EXP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(evExpMest, this._CurrentExpMest);

                        MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                        treatmentFilter.ID = this._CurrentExpMest.TDL_TREATMENT_ID;
                        var treatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();

                        List<V_HIS_EXP_MEST_BLTY_REQ_1> expMestBltyReqs = new List<V_HIS_EXP_MEST_BLTY_REQ_1>();
                        foreach (var item in this._ExpMestBltyReqs_Print)
                        {
                            V_HIS_EXP_MEST_BLTY_REQ_1 expMestBltyReq = new V_HIS_EXP_MEST_BLTY_REQ_1();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_BLTY_REQ_1>(expMestBltyReq, item);
                            expMestBltyReqs.Add(expMestBltyReq);
                        }

                        MPS.Processor.Mps000108.PDO.Mps000108PDO pdo = new MPS.Processor.Mps000108.PDO.Mps000108PDO(
                            evExpMest,
                            expMestBltyReqs,
                            treatment,
                            serviceReq,
                            this._ExpMestBloods_Print
                        );

                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuXuatDonDuocMau(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                WaitingManager.Show();
                if (this._CurrentExpMest.SERVICE_REQ_ID == null)
                    return;
                ProcessPrint(printTypeCode);
                MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ serviceReq = null;
                HisServiceReqViewFilter hisServiceReqViewFilter = new HisServiceReqViewFilter();
                hisServiceReqViewFilter.ID = this._CurrentExpMest.SERVICE_REQ_ID;
                var serviceReqs = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, hisServiceReqViewFilter, param);
                if (serviceReqs != null && serviceReqs.Count > 0)
                {
                    serviceReq = serviceReqs.FirstOrDefault();
                }

                MOS.Filter.HisExpMestBltyReqViewFilter expMestBltyReqFilter = new HisExpMestBltyReqViewFilter();
                expMestBltyReqFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                var expMestBltyReqs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestBltyReqFilter, new CommonParam());
                MPS.Processor.Mps000107.PDO.Mps000107PDO pdo = new MPS.Processor.Mps000107.PDO.Mps000107PDO(
                 serviceReq,
                 expMestBltyReqs,
                 this._ExpMestBloods_Print
                 );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuXuatTraNhaCungCap(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_EXP_MEST expMest = new HIS_EXP_MEST();
                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST, HIS_EXP_MEST>();
                expMest = AutoMapper.Mapper.Map<HIS_EXP_MEST>(this._CurrentExpMest);

                MPS.Processor.Mps000130.PDO.Mps000130PDO pdo = new MPS.Processor.Mps000130.PDO.Mps000130PDO(
                 expMest,
                 this._ExpMestMedicines_Print,
                 this._ExpMestMaterials_Print,
                 this._ExpMestBloods_Print,
                 BackendDataWorker.Get<HIS_SUPPLIER>(),
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>()
                 );

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GN_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DC_GN_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DCGNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DCHTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TDs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_PXs { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_HCs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HCs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TCs = new List<HIS_EXP_MEST_METY_REQ>();

        private void InPhieuXuatChuyenKho(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("Config.HisConfigCFG.ODER_OPTION", Config.HisConfigCFG.ODER_OPTION));

                ProcessPrint(printTypeCode);

                #region TT Chung
                WaitingManager.Show();
                _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DCHTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DCGNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DC_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMetyReq_HCs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                List<HIS_MEDICINE> _Medicines = new List<HIS_MEDICINE>();
                List<HIS_MATERIAL> _Materials = new List<HIS_MATERIAL>();
                List<HIS_BLOOD> _Bloods = new List<HIS_BLOOD>(); 
                List<V_HIS_MEDICINE_PATY> _Medicines_PATY = new List<V_HIS_MEDICINE_PATY >();
                List<V_HIS_MATERIAL_PATY> _Materials_PATY = new List<V_HIS_MATERIAL_PATY >();
                CommonParam param = new CommonParam();

                if (this._ExpMestMedicines_Print != null && this._ExpMestMedicines_Print.Count > 0)
                {
                    List<long> _MedicineIds = this._ExpMestMedicines_Print.Select(p => p.MEDICINE_ID ?? 0).ToList();

                    int skip = 0;
                    while (_MedicineIds.Count - skip > 0)
                    {
                        var listIds = _MedicineIds.Skip(skip).Take(100).ToList();
                        skip += 100;
                        MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = listIds;
                        var dt_Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                        if (dt_Medicines != null && dt_Medicines.Count > 0)
                            _Medicines.AddRange(dt_Medicines);

                    }
                    _Medicines = _Medicines.Distinct().ToList();
                   var his_medipaty = BackendDataWorker.Get<V_HIS_MEDICINE_PATY>().ToList();
                    if(_Medicines != null && _Medicines.Count >0)
                    {
                       foreach(var item in _Medicines)
                       {
                            if(item.IS_SALE_EQUAL_IMP_PRICE != 1)
                            {
                                var data = his_medipaty.Where(o=> o.MEDICINE_ID == item.ID).ToList();
                                if(data != null && data.Count>0)
                                {
                                   _Medicines_PATY.AddRange(data);
                                }
                            }
                       }
                    }
                }

                if (this._ExpMestMaterials_Print != null && this._ExpMestMaterials_Print.Count > 0)
                {
                    List<long> _MaterialIds = this._ExpMestMaterials_Print.Select(p => p.MATERIAL_ID ?? 0).ToList();
                  
                    int skip = 0;
                    while (_MaterialIds.Count - skip > 0)
                    {
                        var listIds = _MaterialIds.Skip(skip).Take(100).ToList();
                        skip += 100;
                        MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = listIds;
                        var dt_Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                        if (dt_Materials != null && dt_Materials.Count > 0)
                            _Materials.AddRange(dt_Materials);

                    }
                    _Materials = _Materials.Distinct().ToList();
                    var his_matepaty = BackendDataWorker.Get<V_HIS_MATERIAL_PATY>().ToList();
                    if(_Materials!=null&& _Materials.Count >0)
                    {
                       foreach(var item in _Materials)
                       {
                            if(item.IS_SALE_EQUAL_IMP_PRICE != 1)
                            {
                                var data = his_matepaty.Where(o=> o.MATERIAL_ID == item.ID).ToList();
                                if(data != null && data.Count>0)
                                {
                                   _Materials_PATY.AddRange(data);
                                }
                            }
                       }
                    }
                }

                if (this._ExpMestBloods_Print != null && this._ExpMestBloods_Print.Count > 0)
                {
                    List<long> _BloodIds = this._ExpMestBloods_Print.Select(p => p.BLOOD_ID).ToList();
                    int skip = 0;
                    while (_BloodIds.Count - skip > 0)
                    {
                        var listIds = _BloodIds.Skip(skip).Take(100).ToList();
                        skip += 100;
                        MOS.Filter.HisBloodFilter bloodNewFilter = new HisBloodFilter();
                        bloodNewFilter.IDs = listIds;
                        var dt_Bloods = new BackendAdapter(param).Get<List<HIS_BLOOD>>("api/HisBlood/Get", ApiConsumers.MosConsumer, bloodNewFilter, param);
                        if (dt_Bloods != null && dt_Bloods.Count > 0)
                            _Bloods.AddRange(dt_Bloods);

                    }
                    _Bloods = _Bloods.Distinct().ToList();
                }

                if (this._CurrentExpMest != null)
                {
                    Req_Department_Name = "";
                    Req_Room_Name = "";
                    Exp_Department_Name = "";
                    var Req_Department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == this._CurrentExpMest.REQ_DEPARTMENT_ID).ToList();
                    if (Req_Department != null && Req_Department.Count > 0)
                    {
                        Req_Department_Name = Req_Department.FirstOrDefault().DEPARTMENT_NAME;
                    }

                    var Req_Room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this._CurrentExpMest.REQ_ROOM_ID).ToList();
                    if (Req_Room != null && Req_Room.Count > 0)
                    {
                        Req_Room_Name = Req_Room.FirstOrDefault().ROOM_NAME;
                    }
                    var Exp_Department = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ID == this._CurrentExpMest.MEDI_STOCK_ID).ToList();
                    if (Exp_Department != null && Exp_Department.Count > 0)
                    {
                        Exp_Department_Name = Exp_Department.FirstOrDefault().DEPARTMENT_NAME;
                    }

                    roomIdByMediStockIdPrint = 0;
                    roomIdByMediStockIdPrint = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this._CurrentExpMest.MEDI_STOCK_ID).ROOM_ID;
                }

                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.HIS_DESKTOP_MPS_AGGR_EXP_MEST_MEDICINE_MERGER_DATA));
                #endregion

                #region In Tong Hop
                //if (keyPrintType == 1)
                //{
                //    string keyName = "";
                //    if (roomIdByMediStockIdPrint > 0)
                //    {
                //        if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                //        {
                //            keyPhieuTra = 1;
                //            keyName = "PHIẾU TRẢ TỔNG HỢP";
                //        }
                //        else
                //        {
                //            keyName = "PHIẾU LĨNH TỔNG HỢP";
                //            keyPhieuTra = 0;
                //        }
                //    }
                //    var expMestBltyReqs = ConvertExpMestBltyViewToTable();
                //    MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                //        (
                //         this._CurrentExpMest,
                //         this._ExpMestMedicines_Print,
                //         this._ExpMestMaterials_Print,
                //         this._ExpMestBloods_Print,
                //         this._ExpMestMetyReqs_Print,
                //         this._ExpMestMatyReqs_Print,
                //         expMestBltyReqs,
                //         Req_Department_Name,
                //         Req_Room_Name,
                //         Exp_Department_Name,
                //         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                //         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                //         BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                //         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                //         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                //         BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                //         BackendDataWorker.Get<HIS_BLOOD_ABO>(),
                //         BackendDataWorker.Get<HIS_BLOOD_RH>(),
                //         keyName,
                //         configKeyMert,
                //         keyPhieuTra,
                //         _Medicines,
                //         _Materials,
                //         _Bloods
                //          );
                //    MPS.ProcessorBase.Core.PrintData PrintData = null;
                //    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                //    }
                //    else
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                //    }

                //    WaitingManager.Hide();
                //    result = MPS.MpsPrinter.Run(PrintData);
                //}
                #endregion
                //else
                {
                    #region --- Xu Ly Tach GN_HT -----
                    if (this._CurrentExpMest != null)
                    {
                        if (this._ExpMestMetyReqs_Print != null && this._ExpMestMetyReqs_Print.Count > 0)
                        {
                            var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                            var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                            bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                            bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                            var IsSeparatePrintingGN = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN).IS_SEPARATE_PRINTING ?? 0;
                            var IsSeparatePrintingHT = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT).IS_SEPARATE_PRINTING ?? 0;
                            bool dcgn = IsSeparatePrintingGN == 1 && mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN);
                            bool dcht = IsSeparatePrintingHT == 1 && mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT);
                            bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                            bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                            bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                            bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                            bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                            bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);

                            var mediTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                            foreach (var item in this._ExpMestMetyReqs_Print)
                            {
                                var dataMedi = mediTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                                if (dataMedi != null)
                                {
                                    if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                                    {
                                        _ExpMestMetyReq_GN_HTs.Add(item);
                                        _ExpMestMetyReq_GNs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                                    {
                                        _ExpMestMetyReq_GN_HTs.Add(item);
                                        _ExpMestMetyReq_HTs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN && dcgn)
                                    {
                                        _ExpMestMetyReq_DC_GN_HTs.Add(item);
                                        _ExpMestMetyReq_DCGNs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT && dcht)
                                    {
                                        _ExpMestMetyReq_DC_GN_HTs.Add(item);
                                        _ExpMestMetyReq_DCHTs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                                    {
                                        _ExpMestMetyReq_TDs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                                    {
                                        _ExpMestMetyReq_PXs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                                    {
                                        _ExpMestMetyReq_COs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                                    {
                                        _ExpMestMetyReq_DTs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                                    {
                                        _ExpMestMetyReq_KSs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                                    {
                                        _ExpMestMetyReq_LAOs.Add(item);
                                    }
                                    else if (dataMedi.IS_CHEMICAL_SUBSTANCE == 1)
                                    {
                                        _ExpMestMetyReq_HCs.Add(item);
                                    }
                                    else
                                    {
                                        _ExpMestMetyReq_Ts.Add(item);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region ----- In GN_HT ------
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                    richEditorMain.RunPrintTemplate("Mps000198", DelegateRunMps);

                    richEditorMain.RunPrintTemplate("Mps000048", DelegateRunMps);
                    #endregion

                    #region -----In Thuoc Thuong -----
                    if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                    {
                        string keyNameAggr = "";// "THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyNameAggr = "PHIẾU TRẢ THUỐC THƯỜNG";
                        }
                        else
                        {
                            keyNameAggr = "PHIẾU LĨNH THUỐC THƯỜNG";
                            keyPhieuTra = 0;
                        }
                        MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                             (
                             this._CurrentExpMest,
                             this._ExpMestMedicines_Print,
                             null,
                             null,
                             _ExpMestMetyReq_Ts,
                             null,
                             null,
                             Req_Department_Name,
                             Req_Room_Name,
                             Exp_Department_Name,
                             IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                             IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                             BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                             BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             null,
                             null,
                             null,
                             null,
                             keyNameAggr,
                             configKeyMert,
                             keyPhieuTra,
                             _Medicines,
                             _Materials,
                             _Bloods,
                             Config.HisConfigCFG.ODER_OPTION,
                             BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>(),
                             _Medicines_PATY,
                             _Materials_PATY,
                             lstConfig
                             );
                        if (lstConfig != null && lstConfig.Count > 0)
                        {
                            Inventec.Common.Logging.LogSystem.Info("lstConfig: " + lstConfig.Count());
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Info("lstConfig NULL ");
                        }
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region -----In Vat Tu -----
                    if (this._ExpMestMatyReqs_Print != null && this._ExpMestMatyReqs_Print.Count > 0)
                    {
                        var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_VTs = new List<HIS_EXP_MEST_MATY_REQ>();
                        _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                        foreach (var item in this._ExpMestMatyReqs_Print)
                        {
                            var dataMaty = _materialTypes.FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                            if (dataMaty != null && dataMaty.IS_CHEMICAL_SUBSTANCE != null)
                            {
                                _ExpMestMatyReq_HCs.Add(item);
                            }
                            else
                                _ExpMestMatyReq_VTs.Add(item);
                        }
                        if (_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0)
                        {
                            string keyNameAggrHc = "";
                            if (roomIdByMediStockIdPrint > 0)
                            {
                                if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                                {
                                    keyPhieuTra = 1;
                                    keyNameAggrHc = "PHIẾU TRẢ HÓA CHẤT";
                                }
                                else
                                {
                                    keyPhieuTra = 0;
                                    keyNameAggrHc = "PHIẾU LĨNH HÓA CHẤT";
                                }
                            }
                            MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                               (
                                 this._CurrentExpMest,
                                 null,
                                 this._ExpMestMaterials_Print,
                                 null,
                                 null,
                                 _ExpMestMatyReq_HCs,
                                 null,
                                 Req_Department_Name,
                                 Req_Room_Name,
                                 Exp_Department_Name,
                                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                                 BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                                  null,
                                  null,
                                 keyNameAggrHc,
                                 configKeyMert,
                                 keyPhieuTra,
                                 _Medicines,
                                 _Materials,
                                 _Bloods,
                                 Config.HisConfigCFG.ODER_OPTION,
                                 null,
                                 _Medicines_PATY,
                                 _Materials_PATY,
                                 lstConfig
                                 );
                            if (lstConfig != null && lstConfig.Count > 0)
                            {
                                Inventec.Common.Logging.LogSystem.Info("lstConfig: " + lstConfig.Count());
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Info("lstConfig NULL ");
                            }
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                            }

                            WaitingManager.Hide();
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count > 0)
                        {
                            string keyNameAggr = "";
                            if (roomIdByMediStockIdPrint > 0)
                            {
                                if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                                {
                                    keyPhieuTra = 1;
                                    keyNameAggr = "PHIẾU TRẢ VẬT TƯ";
                                }
                                else
                                {
                                    keyPhieuTra = 0;
                                    keyNameAggr = "PHIẾU LĨNH VẬT TƯ";
                                }
                            }
                            MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                               (
                                 this._CurrentExpMest,
                                 null,
                                 this._ExpMestMaterials_Print,
                                 null,
                                 null,
                                 _ExpMestMatyReq_VTs,
                                 null,
                                 Req_Department_Name,
                                 Req_Room_Name,
                                 Exp_Department_Name,
                                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                                 BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                                 null,
                                 null,
                                 keyNameAggr,
                                 configKeyMert,
                                 keyPhieuTra,
                                 _Medicines,
                                 _Materials,
                                 _Bloods,
                                 Config.HisConfigCFG.ODER_OPTION,
                                 null,
                                 _Medicines_PATY,
                                 _Materials_PATY,
                                 lstConfig
                                 );
                            if (lstConfig != null && lstConfig.Count > 0)
                            {
                                Inventec.Common.Logging.LogSystem.Info("lstConfig: " + lstConfig.Count());
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Info("lstConfig NULL ");
                            }
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                            }

                            WaitingManager.Hide();
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                    }
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000346(ref bool result, string printTypeCode, string fileName)
        {
            try
            {

                #region TT Chung
                WaitingManager.Show();
                _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                List<HIS_MEDICINE> _Medicines = null;
                List<HIS_MATERIAL> _Materials = null;
                List<HIS_BLOOD> _Bloods = null;
                CommonParam param = new CommonParam();

                if (this._ExpMestMedicines_Print != null && this._ExpMestMedicines_Print.Count > 0)
                {
                    List<long> _MedicineIds = this._ExpMestMedicines_Print.Select(p => p.MEDICINE_ID ?? 0).ToList();
                    MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                    medicineFilter.IDs = _MedicineIds;
                    _Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                }

                if (this._ExpMestMaterials_Print != null && this._ExpMestMaterials_Print.Count > 0)
                {
                    List<long> _MaterialIds = this._ExpMestMaterials_Print.Select(p => p.MATERIAL_ID ?? 0).ToList();
                    MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                    materialFilter.IDs = _MaterialIds;
                    _Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                }

                if (this._ExpMestBloods_Print != null && this._ExpMestBloods_Print.Count > 0)
                {
                    List<long> _BloodIds = this._ExpMestBloods_Print.Select(p => p.BLOOD_ID).ToList();
                    MOS.Filter.HisBloodFilter bloodNewFilter = new HisBloodFilter();
                    bloodNewFilter.IDs = _BloodIds;
                    _Bloods = new BackendAdapter(param).Get<List<HIS_BLOOD>>("api/HisBlood/Get", ApiConsumers.MosConsumer, bloodNewFilter, param);
                }

                if (this._CurrentExpMest != null)
                {
                    Req_Department_Name = "";
                    Req_Room_Name = "";
                    Exp_Department_Name = "";
                    var Req_Department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == this._CurrentExpMest.REQ_DEPARTMENT_ID).ToList();
                    if (Req_Department != null && Req_Department.Count > 0)
                    {
                        Req_Department_Name = Req_Department.FirstOrDefault().DEPARTMENT_NAME;
                    }

                    var Req_Room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this._CurrentExpMest.REQ_ROOM_ID).ToList();
                    if (Req_Room != null && Req_Room.Count > 0)
                    {
                        Req_Room_Name = Req_Room.FirstOrDefault().ROOM_NAME;
                    }
                    var Exp_Department = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ID == this._CurrentExpMest.MEDI_STOCK_ID).ToList();
                    if (Exp_Department != null && Exp_Department.Count > 0)
                    {
                        Exp_Department_Name = Exp_Department.FirstOrDefault().DEPARTMENT_NAME;
                    }

                    roomIdByMediStockIdPrint = 0;
                    roomIdByMediStockIdPrint = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this._CurrentExpMest.MEDI_STOCK_ID).ROOM_ID;
                }

                // tách riêng gây nghiện hướng thần thành một bản in
                _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_HCs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TCs = new List<HIS_EXP_MEST_METY_REQ>();
                if (this._ExpMestMetyReqs_Print != null && this._ExpMestMetyReqs_Print.Count > 0)
                {
                    var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                    var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                    bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                    bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                    bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                    bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                    bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                    bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                    bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                    bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);
                    bool tc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC);


                    var mediTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    foreach (var item in this._ExpMestMetyReqs_Print)
                    {
                        var dataMedi = mediTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                        if (dataMedi != null)
                        {
                            if (dataMedi.IS_CHEMICAL_SUBSTANCE == 1)
                            {
                                _ExpMestMetyReq_HCs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                            {
                                _ExpMestMetyReq_GNs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                            {
                                _ExpMestMetyReq_HTs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                            {
                                _ExpMestMetyReq_TDs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                            {
                                _ExpMestMetyReq_PXs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                            {
                                _ExpMestMetyReq_COs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                            {
                                _ExpMestMetyReq_DTs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                            {
                                _ExpMestMetyReq_KSs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                            {
                                _ExpMestMetyReq_LAOs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC && tc)
                            {
                                _ExpMestMetyReq_TCs.Add(item);
                            }
                            else
                            {
                                _ExpMestMetyReq_Ts.Add(item);
                            }

                        }
                    }
                }

                var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_VTs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                foreach (var item in this._ExpMestMatyReqs_Print)
                {
                    var dataMaty = _materialTypes.FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                    if (dataMaty != null && dataMaty.IS_CHEMICAL_SUBSTANCE == 1)
                    {
                        _ExpMestMatyReq_HCs.Add(item);
                    }
                    else
                        _ExpMestMatyReq_VTs.Add(item);
                }

                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.HIS_DESKTOP_MPS_AGGR_EXP_MEST_MEDICINE_MERGER_DATA));
                WaitingManager.Hide();
                #endregion

                #region ---Thuoc Thuong ----
                if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         _ExpMestMaterials_Print,
         _ExpMestMetyReq_Ts,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "THUỐC THƯỜNG"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---Vat tu Thuong ----
                if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count() > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         _ExpMestMaterials_Print,
         null,
         _ExpMestMatyReq_VTs,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "VẬT TƯ THƯỜNG"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HuongThan ----
                if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO Mps000347PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_HTs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "HƯỚNG THẦN"

          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- GayNghien ----
                if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO Mps000347PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_GNs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "GÂY NGHIỆN"

          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HoaChat ----
                if ((_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0) || (_ExpMestMetyReq_HCs != null && _ExpMestMetyReq_HCs.Count > 0))
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         _ExpMestMaterials_Print,
         _ExpMestMetyReq_HCs,
         _ExpMestMatyReq_HCs,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "HÓA CHẤT"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- DOC ----
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO Mps000347PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_TDs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "ĐỘC"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- PX ----
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO Mps000347PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_PXs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "PHÓNG XẠ"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- CO ----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO Mps000347PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_COs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "CORTICOID"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- DT ----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO Mps000347PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_DTs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "DỊCH TRUYỀN"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- KS ----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO Mps000347PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_KSs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "KHÁNG SINH"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- GayNghien ----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO Mps000347PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_LAOs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "LAO"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- TienChat ----
                if (_ExpMestMetyReq_TCs != null && _ExpMestMetyReq_TCs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO Mps000347PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_TCs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "PHIẾU TRẢ CƠ SỐ THUỐC TIỀN CHẤT"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000347(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                #region TT Chung
                WaitingManager.Show();
                _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                List<HIS_MEDICINE> _Medicines = null;
                List<HIS_MATERIAL> _Materials = null;
                List<HIS_BLOOD> _Bloods = null;
                CommonParam param = new CommonParam();

                if (this._ExpMestMedicines_Print != null && this._ExpMestMedicines_Print.Count > 0)
                {
                    List<long> _MedicineIds = this._ExpMestMedicines_Print.Select(p => p.MEDICINE_ID ?? 0).ToList();
                    MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                    medicineFilter.IDs = _MedicineIds;
                    _Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                }

                if (this._ExpMestMaterials_Print != null && this._ExpMestMaterials_Print.Count > 0)
                {
                    List<long> _MaterialIds = this._ExpMestMaterials_Print.Select(p => p.MATERIAL_ID ?? 0).ToList();
                    MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                    materialFilter.IDs = _MaterialIds;
                    _Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                }

                if (this._ExpMestBloods_Print != null && this._ExpMestBloods_Print.Count > 0)
                {
                    List<long> _BloodIds = this._ExpMestBloods_Print.Select(p => p.BLOOD_ID).ToList();
                    MOS.Filter.HisBloodFilter bloodNewFilter = new HisBloodFilter();
                    bloodNewFilter.IDs = _BloodIds;
                    _Bloods = new BackendAdapter(param).Get<List<HIS_BLOOD>>("api/HisBlood/Get", ApiConsumers.MosConsumer, bloodNewFilter, param);
                }

                if (this._CurrentExpMest != null)
                {
                    Req_Department_Name = "";
                    Req_Room_Name = "";
                    Exp_Department_Name = "";
                    var Req_Department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == this._CurrentExpMest.REQ_DEPARTMENT_ID).ToList();
                    if (Req_Department != null && Req_Department.Count > 0)
                    {
                        Req_Department_Name = Req_Department.FirstOrDefault().DEPARTMENT_NAME;
                    }

                    var Req_Room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this._CurrentExpMest.REQ_ROOM_ID).ToList();
                    if (Req_Room != null && Req_Room.Count > 0)
                    {
                        Req_Room_Name = Req_Room.FirstOrDefault().ROOM_NAME;
                    }
                    var Exp_Department = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ID == this._CurrentExpMest.MEDI_STOCK_ID).ToList();
                    if (Exp_Department != null && Exp_Department.Count > 0)
                    {
                        Exp_Department_Name = Exp_Department.FirstOrDefault().DEPARTMENT_NAME;
                    }

                    roomIdByMediStockIdPrint = 0;
                    roomIdByMediStockIdPrint = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this._CurrentExpMest.MEDI_STOCK_ID).ROOM_ID;
                }

                // tách riêng gây nghiện hướng thần thành một bản in
                _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_HCs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                if (this._ExpMestMetyReqs_Print != null && this._ExpMestMetyReqs_Print.Count > 0)
                {
                    var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                    var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                    bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                    bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                    bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                    bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                    bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                    bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                    bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                    bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);

                    var mediTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    foreach (var item in this._ExpMestMetyReqs_Print)
                    {
                        var dataMedi = mediTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                        if (dataMedi != null)
                        {
                            if (dataMedi.IS_CHEMICAL_SUBSTANCE == 1)
                            {
                                _ExpMestMetyReq_HCs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                            {
                                _ExpMestMetyReq_GNs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                            {
                                _ExpMestMetyReq_HTs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                            {
                                _ExpMestMetyReq_TDs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                            {
                                _ExpMestMetyReq_PXs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                            {
                                _ExpMestMetyReq_COs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                            {
                                _ExpMestMetyReq_DTs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                            {
                                _ExpMestMetyReq_KSs.Add(item);
                            }
                            else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                            {
                                _ExpMestMetyReq_LAOs.Add(item);
                            }
                            else
                            {
                                _ExpMestMetyReq_Ts.Add(item);
                            }
                        }
                    }
                }

                var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_VTs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                foreach (var item in this._ExpMestMatyReqs_Print)
                {
                    var dataMaty = _materialTypes.FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                    if (dataMaty != null && dataMaty.IS_CHEMICAL_SUBSTANCE == 1)
                    {
                        _ExpMestMatyReq_HCs.Add(item);
                    }
                    else
                        _ExpMestMatyReq_VTs.Add(item);
                }

                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.HIS_DESKTOP_MPS_AGGR_EXP_MEST_MEDICINE_MERGER_DATA));
                WaitingManager.Hide();
                #endregion

                #region ---Thuoc Thuong ----

                if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO mps000346PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         _ExpMestMaterials_Print,
         _ExpMestMetyReq_Ts,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "THUỐC THƯỜNG",
           Config.HisConfigCFG.ODER_OPTION

          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---Vat tu Thuong ----
                if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count() > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO mps000346PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         _ExpMestMaterials_Print,
         null,
         _ExpMestMatyReq_VTs,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "VẬT TƯ THƯỜNG",
            Config.HisConfigCFG.ODER_OPTION

          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HuongThan ----
                if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_HTs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "HƯỚNG THẦN",
            Config.HisConfigCFG.ODER_OPTION

          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- GayNghien ----
                if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_GNs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "GÂY NGHIỆN",
  Config.HisConfigCFG.ODER_OPTION

          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HoaChat ----
                if ((_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0) || (_ExpMestMetyReq_HCs != null && _ExpMestMetyReq_HCs.Count > 0))
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO mps000346PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         _ExpMestMaterials_Print,
         _ExpMestMetyReq_HCs,
         _ExpMestMatyReq_HCs,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "HÓA CHẤT",
          Config.HisConfigCFG.ODER_OPTION

          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region thuốc độc
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_TDs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "THUỐC ĐỘC",
        Config.HisConfigCFG.ODER_OPTION

          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region Phóng xạ
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_PXs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "PHÓNG XẠ",
            Config.HisConfigCFG.ODER_OPTION

          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- CO ----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_COs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "CORTICOID",
           Config.HisConfigCFG.ODER_OPTION
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- DT ----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_DTs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "DỊCH TRUYỀN",
            Config.HisConfigCFG.ODER_OPTION
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- KS ----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_KSs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "KHÁNG SINH",
            Config.HisConfigCFG.ODER_OPTION
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- LAO ----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         _CurrentExpMest,
         _ExpMestMedicines_Print,
         null,
         _ExpMestMetyReq_LAOs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "LAO",
           Config.HisConfigCFG.ODER_OPTION
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_CurrentExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunMps(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000048":
                        Mps000048(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000198":
                        Mps000198(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000254":
                        MPS000254(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000349":
                        Mps000349(printTypeCode, fileName);
                        break;
                    case "Mps000350":
                        Mps000350(printTypeCode, fileName);
                        break;
                    case "Mps000351":
                        Mps000351(printTypeCode, fileName);
                        break;
                    case "Mps000352":
                        Mps000352(printTypeCode, fileName);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void MPS000254(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                ProcessPrint(printTypeCode);
                WaitingManager.Show();

                long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                if (keyPrintType == 1)
                {
                    #region ---- GOP GN HT -----
                    if (_ExpMestMetyReq_GN_HTs != null && _ExpMestMetyReq_GN_HTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                           (
                             this._CurrentExpMest,
                            _ExpMestMedicinesBCS,
                            _ExpMestMetyReq_GN_HTs,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000254.PDO.keyTitles.tonghop,
                             ListTreatment,
                         MoreInfo
                             );

                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- GOP DC GN HT -----
                    if (_ExpMestMetyReq_DC_GN_HTs != null && _ExpMestMetyReq_DC_GN_HTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDODC = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                      (
                        this._CurrentExpMest,
                       _ExpMestMedicinesBCS,
                       _ExpMestMetyReq_DC_GN_HTs,
                       IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                       IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                       BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                       BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000254.PDO.keyTitles.tonghopHc,
                        ListTreatment,
                    MoreInfo
                        );

                        MPS.ProcessorBase.Core.PrintData PrintDataDC = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintDataDC = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDODC, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintDataDC = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDODC, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintDataDC);
                    }
                    #endregion
                }
                else
                {
                    #region ---- GN ----
                    if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                           (
                             this._CurrentExpMest,
                            _ExpMestMedicinesBCS,
                            _ExpMestMetyReq_GNs,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000254.PDO.keyTitles.gaynghien,
                             ListTreatment,
                         MoreInfo
                             );

                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- HT -----
                    if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                           (
                             this._CurrentExpMest,
                            _ExpMestMedicinesBCS,
                            _ExpMestMetyReq_HTs,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000254.PDO.keyTitles.huongthan,
                             ListTreatment,
                         MoreInfo
                             );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- DCGN ----
                    if (_ExpMestMetyReq_DCGNs != null && _ExpMestMetyReq_DCGNs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                           (
                             this._CurrentExpMest,
                            _ExpMestMedicinesBCS,
                            _ExpMestMetyReq_DCGNs.OrderBy(o => o.NUM_ORDER).ToList(),
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000254.PDO.keyTitles.DcGayNghien,
                             ListTreatment,
                         MoreInfo
                             );

                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- DCHT -----
                    if (_ExpMestMetyReq_DCHTs != null && _ExpMestMetyReq_DCHTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                           (
                             this._CurrentExpMest,
                            _ExpMestMedicinesBCS,
                            _ExpMestMetyReq_DCHTs.OrderBy(o => o.NUM_ORDER).ToList(),
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000254.PDO.keyTitles.DcHuongThan,
                             ListTreatment,
                         MoreInfo
                             );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }

                #region ----- TD -----
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                       (
                         this._CurrentExpMest,
                        _ExpMestMedicinesBCS,
                        _ExpMestMetyReq_TDs,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         MPS.Processor.Mps000254.PDO.keyTitles.thuocdoc,
                         ListTreatment,
                         MoreInfo
                         );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- PX -----
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                       (
                         this._CurrentExpMest,
                        _ExpMestMedicinesBCS,
                        _ExpMestMetyReq_PXs,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         MPS.Processor.Mps000254.PDO.keyTitles.thuocphongxa,
                         ListTreatment,
                         MoreInfo
                         );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- CO -----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                       (
                         this._CurrentExpMest,
                        _ExpMestMedicinesBCS,
                        _ExpMestMetyReq_COs,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         MPS.Processor.Mps000254.PDO.keyTitles.Corticoid,
                         ListTreatment,
                         MoreInfo
                         );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- DT -----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                       (
                         this._CurrentExpMest,
                        _ExpMestMedicinesBCS,
                        _ExpMestMetyReq_DTs,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         MPS.Processor.Mps000254.PDO.keyTitles.DichTruyen,
                         ListTreatment,
                         MoreInfo
                         );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- KS -----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                       (
                         this._CurrentExpMest,
                        _ExpMestMedicinesBCS,
                        _ExpMestMetyReq_KSs,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         MPS.Processor.Mps000254.PDO.keyTitles.KhangSinh,
                         ListTreatment,
                         MoreInfo
                         );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- LAO -----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                       (
                         this._CurrentExpMest,
                        _ExpMestMedicinesBCS,
                        _ExpMestMetyReq_LAOs,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         MPS.Processor.Mps000254.PDO.keyTitles.Lao,
                         ListTreatment,
                         MoreInfo
                         );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000048(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                ProcessPrint(printTypeCode);
                WaitingManager.Show();
                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));

                long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                if (keyPrintType == 1)
                {
                    #region ---- GN_HT ----
                    if (this._ExpMestMetyReq_GN_HTs != null && this._ExpMestMetyReq_GN_HTs.Count > 0)
                    {
                        string keyAddictive = "";// "THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyAddictive = "PHIẾU TRẢ THUỐC GÂY NGHIỆN, HƯỚNG THẦN";
                        }
                        else
                        {
                            keyPhieuTra = 0;
                            keyAddictive = "PHIẾU LĨNH THUỐC GÂY NGHIỆN, HƯỚNG THẦN";
                        }
                        MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                     (
                      this._CurrentExpMest,
                     this._ExpMestMedicines_Print,
                     null,
                     null,
                     this._ExpMestMetyReq_GN_HTs,
                     null,
                     null,
                     Req_Department_Name,
                     Req_Room_Name,
                     Exp_Department_Name,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                     BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     null,
                     null,
                     keyAddictive,
                     configKeyMert,
                     keyPhieuTra
                       );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }
                else
                {
                    #region ---- GN ----
                    if (this._ExpMestMetyReq_GNs != null && this._ExpMestMetyReq_GNs.Count > 0)
                    {
                        string keyAddictive = "";// "THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyAddictive = "PHIẾU TRẢ THUỐC GÂY NGHIỆN";
                        }
                        else
                        {
                            keyPhieuTra = 0;
                            keyAddictive = "PHIẾU LĨNH THUỐC GÂY NGHIỆN";
                        }
                        MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                     (
                      this._CurrentExpMest,
                     this._ExpMestMedicines_Print,
                     null,
                     null,
                     this._ExpMestMetyReq_GNs,
                     null,
                     null,
                     Req_Department_Name,
                     Req_Room_Name,
                     Exp_Department_Name,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                     BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     null,
                     null,
                     keyAddictive,
                     configKeyMert,
                     keyPhieuTra
                       );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- HT ----
                    if (this._ExpMestMetyReq_HTs != null && this._ExpMestMetyReq_HTs.Count > 0)
                    {
                        string keyNeurological = "";// "THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyNeurological = "PHIẾU TRẢ THUỐC HƯỚNG THẦN";
                        }
                        else
                        {
                            keyPhieuTra = 0;
                            keyNeurological = "PHIẾU LĨNH THUỐC HƯỚNG THẦN";
                        }
                        MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                     (
                      this._CurrentExpMest,
                     this._ExpMestMedicines_Print,
                     null,
                     null,
                     this._ExpMestMetyReq_HTs,
                     null,
                     null,
                     Req_Department_Name,
                     Req_Room_Name,
                     Exp_Department_Name,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                     BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     null,
                     null,
                     keyNeurological,
                     configKeyMert,
                     keyPhieuTra
                       );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }

                #region ---- TD ----
                if (this._ExpMestMetyReq_TDs != null && this._ExpMestMetyReq_TDs.Count > 0)
                {
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC ĐỘC";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC ĐỘC";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  this._CurrentExpMest,
                 this._ExpMestMedicines_Print,
                 null,
                 null,
                 this._ExpMestMetyReq_TDs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );

                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- PX ----
                if (this._ExpMestMetyReq_PXs != null && this._ExpMestMetyReq_PXs.Count > 0)
                {
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC PHÓNG XẠ";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC PHÓNG XẠ";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  this._CurrentExpMest,
                 this._ExpMestMedicines_Print,
                 null,
                 null,
                 this._ExpMestMetyReq_PXs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- CO ----
                if (this._ExpMestMetyReq_COs != null && this._ExpMestMetyReq_COs.Count > 0)
                {
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC CORTICOID";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC CORTICOID";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  this._CurrentExpMest,
                 this._ExpMestMedicines_Print,
                 null,
                 null,
                 this._ExpMestMetyReq_COs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- DT ----
                if (this._ExpMestMetyReq_DTs != null && this._ExpMestMetyReq_DTs.Count > 0)
                {
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC DỊCH TRUYỀN";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC DỊCH TRUYỀN";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  this._CurrentExpMest,
                 this._ExpMestMedicines_Print,
                 null,
                 null,
                 this._ExpMestMetyReq_DTs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- KS ----
                if (this._ExpMestMetyReq_KSs != null && this._ExpMestMetyReq_KSs.Count > 0)
                {
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC KHÁNG SINH";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC KHÁNG SINH";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  this._CurrentExpMest,
                 this._ExpMestMedicines_Print,
                 null,
                 null,
                 this._ExpMestMetyReq_KSs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- LAO ----
                if (this._ExpMestMetyReq_LAOs != null && this._ExpMestMetyReq_LAOs.Count > 0)
                {
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC LAO";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC LAO";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  this._CurrentExpMest,
                 this._ExpMestMedicines_Print,
                 null,
                 null,
                 this._ExpMestMetyReq_LAOs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000198(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                ProcessPrint(printTypeCode);
                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                if (this._ExpMestBltyReqs_Print != null && this._ExpMestBltyReqs_Print.Count > 0)
                {
                    string keyAddictive = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyAddictive = "PHIẾU TRẢ MÁU";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyAddictive = "PHIẾU LĨNH MÁU";
                    }
                    var expMestBltyReqs = ConvertExpMestBltyViewToTable();
                    MPS.Processor.Mps000198.PDO.Mps000198PDO mps000198PDO = new MPS.Processor.Mps000198.PDO.Mps000198PDO
                 (
                  this._CurrentExpMest,
                 expMestBltyReqs,
                 this._ExpMestBloods_Print,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                 BackendDataWorker.Get<HIS_BLOOD_ABO>(),
                 BackendDataWorker.Get<HIS_BLOOD_RH>(),
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 keyAddictive,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_Sale__GNs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_Sale__HTs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_Sale__TPCNs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_Sale__Ts { get; set; }
        List<V_HIS_EXP_MEST> _ExpMest_Sale_Prints { get; set; }
        V_HIS_TRANSACTION _Transaction_Sale_Print { get; set; }
        List<V_HIS_IMP_MEST> impMests { get; set; }
        List<HIS_SERE_SERV_BILL> sereServBills { get; set; }
        List<HIS_SERE_SERV_DEPOSIT> sereServDeposits { get; set; }
        List<V_HIS_TRANSACTION> transactions { get; set; }

        private void InPhieuXuatBan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                WaitingManager.Show();
                _Transaction_Sale_Print = new V_HIS_TRANSACTION();
                _ExpMest_Sale_Prints = new List<V_HIS_EXP_MEST>();
                _ExpMest_Sale_Prints.Add(_CurrentExpMest);
                transactions = new List<V_HIS_TRANSACTION>();

                ProcessPrint(printTypeCode);

                if (_CurrentExpMest.BILL_ID.HasValue)
                {
                    HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                    tranFilter.ID = _CurrentExpMest.BILL_ID;
                    tranFilter.ORDER_DIRECTION = "DESC";
                    tranFilter.ORDER_FIELD = "MODIFY_TIME";
                    _Transaction_Sale_Print = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param).FirstOrDefault();
                    transactions.Add(_Transaction_Sale_Print);

                    HisImpMestFilter impMestFilter = new HisImpMestFilter();
                    impMestFilter.MOBA_EXP_MEST_ID = _CurrentExpMest.ID;
                    impMests = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, impMestFilter, param);

                }
                else
                {
                    if (_CurrentExpMest != null && _CurrentExpMest.SERVICE_REQ_ID != null)
                    {
                        HisSereServBillFilter sereServBillfilter = new HisSereServBillFilter();
                        sereServBillfilter.IS_NOT_CANCEL = true;
                        sereServBillfilter.TDL_SERVICE_REQ_ID = _CurrentExpMest.SERVICE_REQ_ID;
                        sereServBills = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, sereServBillfilter, param);

                        HisSereServDepositFilter sereServDepositfilter = new HisSereServDepositFilter();
                        sereServDepositfilter.IS_CANCEL = false;
                        sereServDepositfilter.TDL_SERVICE_REQ_ID = _CurrentExpMest.SERVICE_REQ_ID;
                        sereServDeposits = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, sereServDepositfilter, param);


                        List<long> billDepositIds = new List<long>();
                        if (sereServBills != null && sereServBills.Count > 0)
                        {
                            billDepositIds.AddRange(sereServBills.Select(s => s.BILL_ID).ToList());
                        }
                        if (sereServDeposits != null && sereServDeposits.Count > 0)
                        {
                            billDepositIds.AddRange(sereServDeposits.Select(s => s.DEPOSIT_ID).ToList());
                        }
                        if (billDepositIds != null && billDepositIds.Count > 0)
                        {
                            HisTransactionViewFilter transFilter = new HisTransactionViewFilter();
                            transFilter.IDs = billDepositIds.Distinct().ToList();
                            transactions = new BackendAdapter(param)
                               .Get<List<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, transFilter, param);
                        }
                    }
                }

                string key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ExpMestSaleCreate.PrintSplitOption");//#24245

                if (key.Trim() == "1")
                {
                    _ExpMestMedi_Sale__GNs = new List<V_HIS_EXP_MEST_MEDICINE>();
                    _ExpMestMedi_Sale__HTs = new List<V_HIS_EXP_MEST_MEDICINE>();
                    _ExpMestMedi_Sale__TPCNs = new List<V_HIS_EXP_MEST_MEDICINE>();
                    _ExpMestMedi_Sale__Ts = new List<V_HIS_EXP_MEST_MEDICINE>();

                    foreach (var item in this._ExpMestMedicines_Print)
                    {
                        if (item.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN)
                        {
                            _ExpMestMedi_Sale__GNs.Add(item);
                        }
                        else if (item.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)
                        {
                            _ExpMestMedi_Sale__HTs.Add(item);
                        }
                        else if (item.IS_FUNCTIONAL_FOOD == 1)
                        {
                            _ExpMestMedi_Sale__TPCNs.Add(item);
                        }
                        else
                        {
                            _ExpMestMedi_Sale__Ts.Add(item);
                        }
                    }

                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    if (_ExpMestMedi_Sale__GNs != null && _ExpMestMedi_Sale__GNs.Count > 0)
                    {
                        store.RunPrintTemplate("Mps000349", DelegateRunMps);
                    }
                    if (_ExpMestMedi_Sale__HTs != null && _ExpMestMedi_Sale__HTs.Count > 0)
                    {
                        store.RunPrintTemplate("Mps000350", DelegateRunMps);
                    }
                    if ((_ExpMestMedi_Sale__TPCNs != null && _ExpMestMedi_Sale__TPCNs.Count > 0)
                        || (this._ExpMestMaterials_Print != null && this._ExpMestMaterials_Print.Count > 0))
                    {
                        store.RunPrintTemplate("Mps000351", DelegateRunMps);
                    }
                    if (_ExpMestMedi_Sale__Ts != null && _ExpMestMedi_Sale__Ts.Count > 0)
                    {
                        store.RunPrintTemplate("Mps000352", DelegateRunMps);
                    }
                }

                else
                {
                    var hisCashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>();
                    var treatment = new V_HIS_TREATMENT();
                    if (_CurrentExpMest.TDL_TREATMENT_ID.HasValue)
                    {
                        HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                        filter.ID = _CurrentExpMest.TDL_TREATMENT_ID;
                        var listTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param);
                        if (listTreatment != null && listTreatment.Count > 0)
                            treatment = listTreatment.FirstOrDefault();
                    }


                    MPS.Processor.Mps000092.PDO.Mps000092PDO pdo = new MPS.Processor.Mps000092.PDO.Mps000092PDO(
                 new List<V_HIS_EXP_MEST>() { _CurrentExpMest },
                 this._ExpMestMedicines_Print,
                 this._ExpMestMaterials_Print,
                 transactions,
                 hisCashierRoom,
                 treatment,
                 impMests
                 );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Mps000349(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                MPS.Processor.Mps000349.PDO.Mps000349PDO rdo = new MPS.Processor.Mps000349.PDO.Mps000349PDO(_ExpMest_Sale_Prints, _ExpMestMedi_Sale__GNs, _Transaction_Sale_Print, impMests);
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000350(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                MPS.Processor.Mps000350.PDO.Mps000350PDO rdo = new MPS.Processor.Mps000350.PDO.Mps000350PDO(_ExpMest_Sale_Prints, _ExpMestMedi_Sale__HTs, _Transaction_Sale_Print, impMests);
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000351(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                MPS.Processor.Mps000351.PDO.Mps000351PDO rdo = new MPS.Processor.Mps000351.PDO.Mps000351PDO(_ExpMest_Sale_Prints, _ExpMestMedi_Sale__TPCNs, _ExpMestMaterials_Print, _Transaction_Sale_Print, impMests);
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000352(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                MPS.Processor.Mps000352.PDO.Mps000352PDO rdo = new MPS.Processor.Mps000352.PDO.Mps000352PDO(_ExpMest_Sale_Prints, _ExpMestMedi_Sale__Ts, _Transaction_Sale_Print, impMests);
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InHoaDonDo(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (!_CurrentExpMest.BILL_ID.HasValue)
                {
                    Inventec.Common.Logging.LogSystem.Debug("BILL_ID is null");
                    return;
                }
                WaitingManager.Show();
                ProcessPrint(printTypeCode);
                V_HIS_TRANSACTION transaction = null;

                HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                tranFilter.ID = _CurrentExpMest.BILL_ID;
                var lstTran = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetVIew", ApiConsumers.MosConsumer, tranFilter, null);
                if (lstTran != null && lstTran.Count > 0)
                {
                    transaction = lstTran.FirstOrDefault();
                }

                MOS.Filter.HisBillGoodsFilter BillGoodFilter = new HisBillGoodsFilter();
                BillGoodFilter.BILL_ID = _CurrentExpMest.BILL_ID;
                var BillGoods = new BackendAdapter(param).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumer.ApiConsumers.MosConsumer, BillGoodFilter, param);

                MOS.Filter.HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this._CurrentExpMest.ID;
                var lstExpMest = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, new CommonParam());

                HisImpMestFilter impMestFilter = new HisImpMestFilter();
                impMestFilter.MOBA_EXP_MEST_ID = this._CurrentExpMest.ID;
                impMests = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, impMestFilter, param);


                WaitingManager.Hide();
                MPS.Processor.Mps000339.PDO.Mps000339PDO pdo = new MPS.Processor.Mps000339.PDO.Mps000339PDO(
                 transaction,
                 BillGoods,
                 this._ExpMestMedicines_Print,
                 this._ExpMestMaterials_Print,
                 lstExpMest,
                 impMests
                 );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InPhieuHieuTruyenMauVaChePham(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                ProcessPrint(printTypeCode);

                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = this._CurrentExpMest.TDL_TREATMENT_ID;
                var treatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();

                HisPatientViewFilter Filter = new HisPatientViewFilter();
                Filter.ID = this._CurrentExpMest.TDL_PATIENT_ID;
                var patients = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, Filter, param).FirstOrDefault();

                List<V_HIS_EXP_MEST_BLOOD> expMestBloods = new List<V_HIS_EXP_MEST_BLOOD>();

                foreach (var item in this._ExpMestBloods)
                {
                    V_HIS_EXP_MEST_BLOOD expMestBlood = new V_HIS_EXP_MEST_BLOOD();
                    AutoMapper.Mapper.CreateMap<ExpMestBloodADODetail, V_HIS_EXP_MEST_BLOOD>();
                    expMestBlood = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_BLOOD>(item);
                    expMestBloods.Add(expMestBlood);
                }

                HisTransfusionSumViewFilter filter = new HisTransfusionSumViewFilter();
                filter.EXP_MEST_BLOOD_IDs = expMestBloods.Select(o => o.ID).ToList();
                List<V_HIS_TRANSFUSION_SUM> v_HIS_TRANSFUSION_SUMs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSFUSION_SUM>>("/api/HisTransfusionSum/GetView", ApiConsumers.MosConsumer, filter, null);

                HisTransfusionFilter filterTrans = new HisTransfusionFilter();
                filterTrans.IDs = v_HIS_TRANSFUSION_SUMs.Select(o => o.ID).ToList();
                List<HIS_TRANSFUSION> hIS_TRANSFUSION = new BackendAdapter(new CommonParam()).Get<List<HIS_TRANSFUSION>>("/api/HisTransfusion/GetView", ApiConsumers.MosConsumer, filterTrans, null);
                
                List<V_HIS_EXP_BLTY_SERVICE> ExpBltyService = new List<V_HIS_EXP_BLTY_SERVICE>();

                HisExpBltyServiceViewFilter BltyServicefilter = new HisExpBltyServiceViewFilter();
                BltyServicefilter.EXP_MEST_ID = this._CurrentExpMest.ID;

                ExpBltyService = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_BLTY_SERVICE>>("api/HisExpBltyService/GetView", ApiConsumer.ApiConsumers.MosConsumer, BltyServicefilter, new CommonParam());


                HisExpMestFilter expFilter = new HisExpMestFilter();
                expFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM;
                expFilter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                expFilter.TDL_TREATMENT_ID = this._CurrentExpMest.TDL_TREATMENT_ID;
                var ExpMest = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expFilter, new CommonParam());

                MOS.Filter.HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this._CurrentExpMest.ID;
                var lstExpMest = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, new CommonParam()).FirstOrDefault();


                WaitingManager.Hide();
                MPS.Processor.Mps000421.PDO.Mps000421PDO pdo = new MPS.Processor.Mps000421.PDO.Mps000421PDO(
                 treatment,
                 patients,
                 lstExpMest,
                 expMestBloods,
                 ExpBltyService,
                 v_HIS_TRANSFUSION_SUMs,
                 ExpMest,
                 hIS_TRANSFUSION
                 );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InPhieuTemDonMau(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                ProcessPrint(printTypeCode);

                MOS.Filter.HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();   
                expMestFilter.ID = this._CurrentExpMest.ID;
                var lstExpMest = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, new CommonParam()).FirstOrDefault();

                HisExpMestBltyReqViewFilter expMestBltyReqfilter = new HisExpMestBltyReqViewFilter();
                expMestBltyReqfilter.EXP_MEST_ID = this._CurrentExpMest.ID;

                var lstExpBltyService = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestBltyReqfilter, new CommonParam());

                WaitingManager.Hide();
                MPS.Processor.Mps000422.PDO.Mps000422PDO pdo = new MPS.Processor.Mps000422.PDO.Mps000422PDO(
                 lstExpMest,
                 lstExpBltyService
                 );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
