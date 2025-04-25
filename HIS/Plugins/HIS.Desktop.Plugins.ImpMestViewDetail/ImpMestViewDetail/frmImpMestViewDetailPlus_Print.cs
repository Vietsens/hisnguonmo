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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ImpMestViewDetail.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ImpMestViewDetail.ImpMestViewDetail
{
    public partial class frmImpMestViewDetail : HIS.Desktop.Utility.FormBase
    {

        internal enum PrintType
        {
            BIEN_BAN_KIEM_NHAP_TU_NCC,
            PHIEU_NHAP_THUOC_VAT_TU_TU_NCC,
            PHIEU_NHAP_MAU_TU_NCC,
            BAR_CODE,
            PHIEU_NHAP_THU_HOI,
            PHIEU_NHAP_CHUYEN_KHO,
            PHIEU_NHAP_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN,
            PHIEU_NHAP_CHUYEN_KHO_KHONG_PHAI_THUOC_GAY_NGHIEN_HUONG_THAN,
            PHIEU_NHAP_KIEM_KE_DAU_KY_KHAC,
            Mps000214_Nhap_Hao_Phi_Tra_Lai,
            Mps000213_Don_Mau_Tra_Lai,
            Mps000084_Don_Noi_Tru_Tra_Lai,
            Mps000230_Bu_Thuoc_Le,
            Mps000221_Bu_Co_So_Tu_Truc,
            Mps000244_Phieu_San_Xuat_thuoc,
            Mps000092_Phieu_Xuat_Ban,
            IN_TEM_THEO_SO_SERI
        }

        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

        public void InitMenuToButtonPrint()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                CommonParam param = new CommonParam();
                vimpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                MOS.Filter.HisImpMestMaterialViewFilter impMestMaterialViewFilter = new HisImpMestMaterialViewFilter();
                impMestMaterialViewFilter.IMP_MEST_ID = this.ImpMestId;
                vimpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, impMestMaterialViewFilter, param);
                if (vimpMestMaterials != null && vimpMestMaterials.Count > 0)
                {
                    vimpMestMaterials = vimpMestMaterials.Where(o => !String.IsNullOrEmpty(o.SERIAL_NUMBER)).ToList();
                    if (vimpMestMaterials != null && vimpMestMaterials.Count > 0)
                    {
                        DXMenuItem itemIntemTheoSoSeri = new DXMenuItem("In tem theo số Seri", new EventHandler(OnClickInPhieuNhap));
                        itemIntemTheoSoSeri.Tag = PrintType.IN_TEM_THEO_SO_SERI;
                        menu.Items.Add(itemIntemTheoSoSeri);
                    }
                }
                // nhap tu nha cung cap
                if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    btnHoiDongKiemNhap.Enabled = true;
                    if ((this.impMestMedicines != null && this.impMestMedicines.Count > 0) || (this.impMestMaterials != null && this.impMestMaterials.Count > 0))
                    {
                        DXMenuItem itemBienBankiemNhapNCC = new DXMenuItem("Biên bản kiểm nhập từ nhà cung cấp", new EventHandler(OnClickInPhieuNhap));
                        itemBienBankiemNhapNCC.Tag = PrintType.BIEN_BAN_KIEM_NHAP_TU_NCC;
                        menu.Items.Add(itemBienBankiemNhapNCC);
                        if (this.ImpMestSttId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT)
                        {
                            DXMenuItem itemPhieuNhapTuNCC = new DXMenuItem("Phiếu nhập thuốc, vật tư từ nhà cung cấp", new EventHandler(OnClickInPhieuNhap));
                            itemPhieuNhapTuNCC.Tag = PrintType.PHIEU_NHAP_THUOC_VAT_TU_TU_NCC;
                            menu.Items.Add(itemPhieuNhapTuNCC);
                        }
                    }
                    else if (this.impMestBloods != null && this.impMestBloods.Count > 0)
                    {
                        DXMenuItem itemPhieuNhapMauTuNCC = new DXMenuItem("Phiếu nhập máu từ nhà cung cấp", new EventHandler(OnClickInPhieuNhap));
                        itemPhieuNhapMauTuNCC.Tag = PrintType.PHIEU_NHAP_MAU_TU_NCC;
                        menu.Items.Add(itemPhieuNhapMauTuNCC);
                        DXMenuItem itemBienBankiemNhapNCC = new DXMenuItem("Biên bản kiểm nhập từ nhà cung cấp", new EventHandler(OnClickInPhieuNhap));
                        itemBienBankiemNhapNCC.Tag = PrintType.BIEN_BAN_KIEM_NHAP_TU_NCC;
                        menu.Items.Add(itemBienBankiemNhapNCC);
                    }
                }
                else if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL || this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL || this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DONKTL)
                {
                    DXMenuItem itemPhieuNhapThuHoi = new DXMenuItem("Đơn trả lại", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapThuHoi.Tag = PrintType.PHIEU_NHAP_THU_HOI;
                    menu.Items.Add(itemPhieuNhapThuHoi);
                }
                else if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK)
                {
                    DXMenuItem itemPhieuNhapChuyenKho = new DXMenuItem("Phiếu nhập chuyển kho", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapChuyenKho.Tag = PrintType.PHIEU_NHAP_CHUYEN_KHO;
                    menu.Items.Add(itemPhieuNhapChuyenKho);

                    DXMenuItem itemPhieuNhapChuyenKhoThuocGayNghienHuongThan = new DXMenuItem("Phiếu nhập chuyển kho thuốc gây nghiện, hướng thần", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapChuyenKhoThuocGayNghienHuongThan.Tag = PrintType.PHIEU_NHAP_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN;
                    menu.Items.Add(itemPhieuNhapChuyenKhoThuocGayNghienHuongThan);

                    DXMenuItem itemPhieuNhapChuyenKhoKhongPhaiThuocGayNghienHuongThan = new DXMenuItem("Phiếu nhập chuyển kho không phải thuốc gây nghiện, hướng thần", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapChuyenKhoKhongPhaiThuocGayNghienHuongThan.Tag = PrintType.PHIEU_NHAP_CHUYEN_KHO_KHONG_PHAI_THUOC_GAY_NGHIEN_HUONG_THAN;
                    menu.Items.Add(itemPhieuNhapChuyenKhoKhongPhaiThuocGayNghienHuongThan);
                }
                else if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC || this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK || this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    btnHoiDongKiemNhap.Enabled = true;

                    DXMenuItem itemPhieuNhapKiemKeDauKyKhac = new DXMenuItem("Phiếu nhập kiểm kê, đầu kỳ, khác", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapKiemKeDauKyKhac.Tag = PrintType.PHIEU_NHAP_KIEM_KE_DAU_KY_KHAC;
                    menu.Items.Add(itemPhieuNhapKiemKeDauKyKhac);
                }
                else if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL || this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BTL)
                {
                    DXMenuItem itemPhieuNhapHaoPhiTraLai = new DXMenuItem("Phiếu nhập hao phí trả lại/Xuất bán trả lại", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapHaoPhiTraLai.Tag = PrintType.Mps000214_Nhap_Hao_Phi_Tra_Lai;
                    menu.Items.Add(itemPhieuNhapHaoPhiTraLai);

                    if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BTL)
                    {
                        DXMenuItem itemHoaDonXuatBan = new DXMenuItem("Phiếu xuất bán", new EventHandler(OnClickInPhieuNhap));
                        itemHoaDonXuatBan.Tag = PrintType.Mps000092_Phieu_Xuat_Ban;
                        menu.Items.Add(itemHoaDonXuatBan);
                    }
                }
                else if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL)
                {
                    DXMenuItem itemPhieuNhapDonMauTraLai = new DXMenuItem("Phiếu nhập đơn máu trả lại", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapDonMauTraLai.Tag = PrintType.Mps000213_Don_Mau_Tra_Lai;
                    menu.Items.Add(itemPhieuNhapDonMauTraLai);
                }
                else if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BL)
                {
                    DXMenuItem itemPhieuNhapBuThuocLe = new DXMenuItem("Phiếu nhập bù lẻ", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapBuThuocLe.Tag = PrintType.Mps000230_Bu_Thuoc_Le;
                    menu.Items.Add(itemPhieuNhapBuThuocLe);
                }
                else if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCS)
                {
                    DXMenuItem itemPhieuNhapBuCoSoTuTruc = new DXMenuItem("Phiếu bù cơ số tủ trực", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapBuCoSoTuTruc.Tag = PrintType.Mps000221_Bu_Co_So_Tu_Truc;
                    menu.Items.Add(itemPhieuNhapBuCoSoTuTruc);
                }
                else
                {
                    cboPrint.Enabled = false;
                }

                cboPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuNhap(object sender, EventArgs e)
        {
            try
            {
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
                    case PrintType.BIEN_BAN_KIEM_NHAP_TU_NCC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BienBanKiemNhapTuNhaCungCap_MPS000085, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_THUOC_VAT_TU_TU_NCC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_TU_NCC__MPS000141, DelegateRunPrinter);
                        break;
                    case PrintType.BAR_CODE:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__BARCODE__MPS000142, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_THU_HOI:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuYeuCauNhapThuHoi_MPS000084, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_CHUYEN_KHO:
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO__MPS000143, DelegateRunPrinter);
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO_MAU__MPS000226, DelegateRunPrinter);
                        }
                        break;
                    case PrintType.PHIEU_NHAP_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN__MPS000142, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_CHUYEN_KHO_KHONG_PHAI_THUOC_GAY_NGHIEN_HUONG_THAN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO_THUOC_KHONG_PHAI_GAY_NGHIEN_HUONG_THAN__MPS000145, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_MAU_TU_NCC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_NHAP_MAU_TU_NCC_MPS000149, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_KIEM_KE_DAU_KY_KHAC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_KIEM_KE_DAU_KY_KHAC__MPS000199, DelegateRunPrinter);
                        break;
                    case PrintType.Mps000214_Nhap_Hao_Phi_Tra_Lai:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_HAO_PHI_TRA_LAI__MPS000214, DelegateRunPrinter);
                        break;
                    case PrintType.Mps000213_Don_Mau_Tra_Lai:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_DON_MAU_TRA_LAI__MPS000213, DelegateRunPrinter);
                        break;
                    case PrintType.Mps000230_Bu_Thuoc_Le:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_BU_THUOC_LE__MPS000230, DelegateRunPrinter);
                        break;
                    case PrintType.Mps000221_Bu_Co_So_Tu_Truc:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_BU_CO_SO_TU_TRUC__MPS000221, DelegateRunPrinter);
                        break;
                    case PrintType.Mps000244_Phieu_San_Xuat_thuoc:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_SAN_XUAT_THUOC__MPS000244, DelegateRunPrinter);
                        break;
                    case PrintType.Mps000092_Phieu_Xuat_Ban:
                        richEditorMain.RunPrintTemplate("Mps000092", DelegateRunPrinter);
                        break;
                    case PrintType.IN_TEM_THEO_SO_SERI:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_TEM_THEO_SO_SERI__MPS000494, DelegateRunPrinter);
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

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BienBanKiemNhapTuNhaCungCap_MPS000085:
                        InBienBanKiemNhapTuNhaCungCap(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_TU_NCC__MPS000141:
                        InPhieuNhapTuNhaCungCap(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO__MPS000143:
                        InPhieuNhapChuyenKho(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO_MAU__MPS000226:
                        InPhieuNhapChuyenKhoMau(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN__MPS000142:
                        InPhieuNhapChuyenKhoThuocGayNghienHuongThan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO_THUOC_KHONG_PHAI_GAY_NGHIEN_HUONG_THAN__MPS000145:
                        InPhieuNhapChuyenKhoThuocKhongPhaiGayNghienHuongThan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuYeuCauNhapThuHoi_MPS000084:
                        InPhieuNhapThuHoi(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_NHAP_MAU_TU_NCC_MPS000149:
                        InPhieuNhapMauTuNCC(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_KIEM_KE_DAU_KY_KHAC__MPS000199:
                        InPhieuNhapKiemKeDauKyKhac(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_HAO_PHI_TRA_LAI__MPS000214:
                        InPhieuNhapHaoPhiTraLai(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_DON_MAU_TRA_LAI__MPS000213:
                        InPhieuNhapDonMauTraLai(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_BU_THUOC_LE__MPS000230:
                        InPhieuNhapBuThuocLe(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_BU_CO_SO_TU_TRUC__MPS000221:
                        InPhieuNhapBuCoSoTuTruc(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000092":
                        InHoaDonXuatBan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_TEM_THEO_SO_SERI__MPS000494:
                        InTemTheoSoSeri(printTypeCode, fileName, ref result);
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

        private void InHoaDonXuatBan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.impMest == null)
                    return;

                WaitingManager.Show();
                CommonParam param = new CommonParam();

                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this.impMest.MOBA_EXP_MEST_ID;
                List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);
                if (expMests == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy phiếu xuất tương ứng");
                    return;
                }

                V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();
                if (expMests.FirstOrDefault().TDL_TREATMENT_ID.HasValue)
                {
                    HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                    filter.ID = expMests.FirstOrDefault().TDL_TREATMENT_ID.Value;
                    var listTreatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, null);
                    if (listTreatment != null && listTreatment.Count > 0)
                        treatment = listTreatment.FirstOrDefault();
                }

                if ((treatment == null || treatment.ID == 0) && expMests.FirstOrDefault().TDL_PATIENT_ID.HasValue)
                {
                    CommonParam paramCommon = new CommonParam();
                    HisTreatmentViewFilter filterView = new HisTreatmentViewFilter();
                    filterView.PATIENT_ID = expMests.FirstOrDefault().TDL_PATIENT_ID.Value;
                    var listTreatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filterView, null);
                    if (listTreatment != null && listTreatment.Count > 0)
                    {
                        treatment = listTreatment.OrderByDescending(o => o.TREATMENT_CODE).FirstOrDefault();
                    }
                }

                HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                tranFilter.IDs = expMests.Select(s => s.BILL_ID ?? 0).ToList();
                List<V_HIS_TRANSACTION> transactionList = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param);

                V_HIS_TRANSACTION transaction = new V_HIS_TRANSACTION();
                if (transactionList == null || transactionList.Count <= 0)
                {
                    transaction = transactionList.FirstOrDefault();
                    //DevExpress.XtraEditors.XtraMessageBox.Show("Phiếu xuất chưa có hóa đơn thanh toán");
                    //return;
                }

                HisBillGoodsFilter goodsFilter = new HisBillGoodsFilter();
                goodsFilter.BILL_IDs = expMests.Select(s => s.BILL_ID ?? 0).ToList();
                List<HIS_BILL_GOODS> billGoods = new BackendAdapter(param).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, goodsFilter, param);

                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetVIew", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetVIew", ApiConsumers.MosConsumer, expMestMaterialFilter, param);
                var hisCashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>();

                HisImpMestFilter impMestFilter = new HisImpMestFilter();
                impMestFilter.MOBA_EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_IMP_MEST> impMests = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, impMestFilter, param);


                MPS.Processor.Mps000092.PDO.Mps000092PDO rdo = new MPS.Processor.Mps000092.PDO.Mps000092PDO(expMests, expMestMedicines, expMestMaterials, transaction, hisCashierRoom, treatment, impMests);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                MPS.ProcessorBase.Core.PrintData printdata = null;
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(printdata);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InTemTheoSoSeri(string printTypeCode, string fileName, ref bool result)
        {

            try
            {
                if (this.vimpMestMaterials != null && this.vimpMestMaterials.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    List<MPS.Processor.Mps000494.PDO.SerialADO> listSerial = new List<MPS.Processor.Mps000494.PDO.SerialADO>();

                    List<HIS_MATERIAL> materials = new List<HIS_MATERIAL>();
                    MOS.Filter.HisMaterialFilter hisMaterialFilter = new HisMaterialFilter();
                    hisMaterialFilter.IDs = this.vimpMestMaterials.Select(o => o.MATERIAL_ID).Distinct().ToList();
                    materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, hisMaterialFilter, param);
                    foreach (var item in vimpMestMaterials)
                    {
                        HIS_MATERIAL material = materials.FirstOrDefault(o => o.ID == item.MATERIAL_ID);
                        MPS.Processor.Mps000494.PDO.SerialADO serial = new MPS.Processor.Mps000494.PDO.SerialADO();
                        serial.SERIAL_NUMBER = item.SERIAL_NUMBER;
                        serial.NEXT_REUSABLE_NUMBER = null;
                        serial.SIZE = material != null ? material.MATERIAL_SIZE : null;
                        listSerial.Add(serial);
                    }

                    MPS.Processor.Mps000494.PDO.Mps000494PDO mps000494PDO = new MPS.Processor.Mps000494.PDO.Mps000494PDO(listSerial);
                    WaitingManager.Hide();

                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000494PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000494PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public List<V_HIS_IMP_MEST_BLOOD> MapImpMestBloodFromSDO(List<ImpMestBloodSDODetail> input)
        {
            List<V_HIS_IMP_MEST_BLOOD> result = new List<V_HIS_IMP_MEST_BLOOD>();
            try
            {
                foreach (var item in input)
                {
                    V_HIS_IMP_MEST_BLOOD blood = new V_HIS_IMP_MEST_BLOOD();
                    AutoMapper.Mapper.CreateMap<ImpMestBloodSDODetail, V_HIS_IMP_MEST_BLOOD>();
                    blood = AutoMapper.Mapper.Map<V_HIS_IMP_MEST_BLOOD>(item);
                    result.Add(blood);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InPhieuNhapChuyenKhoMau(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<V_HIS_EXP_MEST> datas = new List<V_HIS_EXP_MEST>();
                WaitingManager.Show();
                if (this.impMest != null && this.impMest.CHMS_EXP_MEST_ID > 0)
                {
                    MOS.Filter.HisExpMestViewFilter filter = new HisExpMestViewFilter();
                    filter.ID = this.impMest.CHMS_EXP_MEST_ID;
                    datas = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, filter, null);
                }

                MPS.Processor.Mps000226.PDO.Mps000226PDO.Mps000226Key _Mps000226Key = new MPS.Processor.Mps000226.PDO.Mps000226PDO.Mps000226Key();
                if (datas != null && datas.Count > 0)
                {
                    _Mps000226Key.EXP_MEDI_STOCK_CODE = datas[0].MEDI_STOCK_CODE;
                    _Mps000226Key.EXP_MEDI_STOCK_NAME = datas[0].MEDI_STOCK_NAME;
                }

                var impMestBloodPrint = MapImpMestBloodFromSDO(this.impMestBloods);
                if (impMestBloodPrint == null || impMestBloodPrint.Count <= 0)
                    return;

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                MPS.Processor.Mps000226.PDO.Mps000226PDO pdo = new MPS.Processor.Mps000226.PDO.Mps000226PDO(
                 this.impMest,
                 impMestBloodPrint,
                 _Mps000226Key,
                 HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator
                );

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
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

        private void InPhieuNhapBuThuocLe(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                // get expMest from impMest
                CommonParam param = new CommonParam();
                V_HIS_EXP_MEST expMest = new V_HIS_EXP_MEST();
                MOS.Filter.HisExpMestViewFilter expMestViewFilter = new HisExpMestViewFilter();
                expMestViewFilter.ID = this.impMest.CHMS_EXP_MEST_ID;
                var expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestViewFilter, param);
                if (expMests != null && expMests.Count > 0)
                {
                    expMest = expMests.FirstOrDefault();
                }

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                var impMestMedicinePrints = MapImpMestMedicineFromSDO(this.impMestMedicines);

                var ImpMestMaterialPrints = MapImpMestMaterialFromSDO(this.impMestMaterials);

                MPS.Processor.Mps000230.PDO.Mps000230PDO pdo = new MPS.Processor.Mps000230.PDO.Mps000230PDO(
                    this.impMest,
                    expMest,
                    impMestMedicinePrints,
                    ImpMestMaterialPrints
                );

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuNhapDonMauTraLai(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var impMestBloodPrint = MapImpMestBloodFromSDO(this.impMestBloods);
                MPS.Processor.Mps000213.PDO.Mps000213PDO pdo = new MPS.Processor.Mps000213.PDO.Mps000213PDO(
                this.impMest,
                impMestBloodPrint
                );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuNhapBuCoSoTuTruc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                // get expMest from impMest
                CommonParam param = new CommonParam();
                V_HIS_EXP_MEST expMest = new V_HIS_EXP_MEST();
                MOS.Filter.HisExpMestViewFilter expMestViewFilter = new HisExpMestViewFilter();
                expMestViewFilter.ID = this.impMest.CHMS_EXP_MEST_ID;
                var expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestViewFilter, param);
                if (expMests != null && expMests.Count > 0)
                {
                    expMest = expMests.FirstOrDefault();
                }

                var impMestMedicinePrints = MapImpMestMedicineFromSDO(this.impMestMedicines);

                var ImpMestMaterialPrints = MapImpMestMaterialFromSDO(this.impMestMaterials);

                MPS.Processor.Mps000221.PDO.Mps000221PDO pdo = new MPS.Processor.Mps000221.PDO.Mps000221PDO(
                    this.impMest,
                    expMest,
                    impMestMedicinePrints,
                    ImpMestMaterialPrints
                );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuNhapHaoPhiTraLai(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                MOS.Filter.HisImpMestFilter hisImpMestFilter = new MOS.Filter.HisImpMestFilter();
                hisImpMestFilter.ID = this.ImpMestId;
                var ImpMest = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GET, ApiConsumers.MosConsumer, hisImpMestFilter, param).FirstOrDefault();
                var impMestMedicinePrints = new List<V_HIS_IMP_MEST_MEDICINE>();
                MOS.Filter.HisImpMestMedicineFilter impMestMedicineViewFilter = new HisImpMestMedicineFilter();
                impMestMedicineViewFilter.IMP_MEST_ID = this.ImpMestId;
                impMestMedicinePrints = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMestMedicineViewFilter, param);
                var impMestMaterialPrints = new List<V_HIS_IMP_MEST_MATERIAL>();
                MOS.Filter.HisImpMestMaterialViewFilter impMestMaterialViewFilter = new HisImpMestMaterialViewFilter();
                impMestMaterialViewFilter.IMP_MEST_ID = this.ImpMestId;
                impMestMaterialPrints = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, impMestMaterialViewFilter, param);

                MOS.Filter.HisExpMestMedicineViewFilter hisExpMestMedicineViewFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                hisExpMestMedicineViewFilter.EXP_MEST_ID = ImpMest.MOBA_EXP_MEST_ID;
                var expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, hisExpMestMedicineViewFilter, param);

                MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialViewFilter = new MOS.Filter.HisExpMestMaterialViewFilter();
                expMestMaterialViewFilter.EXP_MEST_ID = ImpMest.MOBA_EXP_MEST_ID;
                var expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialViewFilter, param);

                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.ID = ImpMest.MOBA_EXP_MEST_ID;
                var expMest = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, null).FirstOrDefault();

                MPS.Processor.Mps000214.PDO.Mps000214PDO pdo = new MPS.Processor.Mps000214.PDO.Mps000214PDO(
                this.impMest,
                impMestMedicinePrints,
                impMestMaterialPrints,
                expMestMedicines,
                expMestMaterials,
                expMest
                );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
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

        private void InPhieuNhapKiemKeDauKyKhac(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                // new 
                WaitingManager.Show();

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                CommonParam param = new CommonParam();
                MOS.Filter.HisImpMestViewFilter impMestFilter = new MOS.Filter.HisImpMestViewFilter();
                impMestFilter.ID = this.ImpMestId;
                this.impMest = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("/api/HisImpMest/GetView", ApiConsumers.MosConsumer, impMestFilter, param).FirstOrDefault();
                if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                    userFilter.IMP_MEST_ID = this.ImpMestId;
                    var _ImpMestUser = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                    _ImpMestUser = _ImpMestUser.OrderBy(p => p.ID).ToList();

                    HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                    mediFilter.IMP_MEST_ID = this.ImpMestId;
                    var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                    HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                    mateFilter.IMP_MEST_ID = this.ImpMestId;
                    var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);
                    // MPS.Processor.Mps000170.PDO.Mps000170PDO Mps000170RDO = new MPS.Processor.Mps000170.PDO.Mps000170PDO(initImpMest, hisImpMestMedicines, hisImpMestMaterials);

                    var impMestBloodPrint = MapImpMestBloodFromSDO(this.impMestBloods);

                    List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                    if (hisImpMestMedicines != null && hisImpMestMedicines.Count > 0)
                    {
                        HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = hisImpMestMedicines.Select(o => o.MEDICINE_ID).Distinct().ToList();
                        medicines = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
                    }
                    List<HIS_MATERIAL> materials = new List<HIS_MATERIAL>();
                    if (hisImpMestMaterials != null && hisImpMestMaterials.Count > 0)
                    {
                        HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = hisImpMestMaterials.Select(o => o.MATERIAL_ID).Distinct().ToList();
                        materials = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, new CommonParam());
                    }

                    List<V_HIS_MEDICAL_CONTRACT> MedicalContract = new List<V_HIS_MEDICAL_CONTRACT>();
                    List<long> MedicalContractIds = new List<long>();

                    if (medicines != null && medicines.Count > 0)
                    {
                        List<long> medicineContract = medicines.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();

                        if (medicineContract != null && medicineContract.Count > 0)
                        {
                            MedicalContractIds.AddRange(medicineContract);
                        }
                    }

                    if (materials != null && materials.Count > 0)
                    {
                        List<long> materialContract = materials.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();

                        if (materialContract != null && materialContract.Count > 0)
                        {
                            MedicalContractIds.AddRange(materialContract);
                        }
                    }

                    if (MedicalContractIds != null && MedicalContractIds.Count > 0)
                    {
                        HisMedicalContractViewFilter MedicalContractFilter = new HisMedicalContractViewFilter();
                        MedicalContractFilter.IDs = MedicalContractIds;
                        MedicalContract = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/GetView", ApiConsumers.MosConsumer, MedicalContractFilter, new CommonParam());
                    }

                    List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO> MedicalContractADO = new List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>();
                    if (MedicalContract != null && MedicalContract.Count > 0)
                    {
                        foreach (var item in medicines)
                        {
                            MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO ado = new MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO();

                            var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>(ado, Contract);
                            ado.MEDICINE_ID = item.ID;
                            MedicalContractADO.Add(ado);
                        }

                        foreach (var item in materials)
                        {
                            MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO ado = new MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO();

                            var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>(ado, Contract);
                            ado.MATERIAL_ID = item.ID;
                            MedicalContractADO.Add(ado);
                        }
                    }

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                    MPS.Processor.Mps000199.PDO.Mps000199PDO Mps000199RDO = new MPS.Processor.Mps000199.PDO.Mps000199PDO(
                       this.impMest,
                       hisImpMestMedicines,
                       hisImpMestMaterials,
                       impMestBloodPrint,
                       _ImpMestUser,
                        MedicalContractADO
                       );

                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                }
                else if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
                {
                    MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                    userFilter.IMP_MEST_ID = this.ImpMestId;
                    var _ImpMestUser = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                    _ImpMestUser = _ImpMestUser.OrderBy(p => p.ID).ToList();

                    HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                    mediFilter.IMP_MEST_ID = this.ImpMestId;
                    var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                    HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                    mateFilter.IMP_MEST_ID = this.ImpMestId;
                    var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);

                    var impMestBloodPrint = MapImpMestBloodFromSDO(this.impMestBloods);

                    List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                    if (hisImpMestMedicines != null && hisImpMestMedicines.Count > 0)
                    {
                        HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = hisImpMestMedicines.Select(o => o.MEDICINE_ID).Distinct().ToList();
                        medicines = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
                    }
                    List<HIS_MATERIAL> materials = new List<HIS_MATERIAL>();
                    if (hisImpMestMaterials != null && hisImpMestMaterials.Count > 0)
                    {
                        HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = hisImpMestMaterials.Select(o => o.MATERIAL_ID).Distinct().ToList();
                        materials = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, new CommonParam());
                    }

                    List<V_HIS_MEDICAL_CONTRACT> MedicalContract = new List<V_HIS_MEDICAL_CONTRACT>();
                    List<long> MedicalContractIds = new List<long>();

                    if (medicines != null && medicines.Count > 0)
                    {
                        List<long> medicineContract = medicines.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();

                        if (medicineContract != null && medicineContract.Count > 0)
                        {
                            MedicalContractIds.AddRange(medicineContract);
                        }
                    }

                    if (materials != null && materials.Count > 0)
                    {
                        List<long> materialContract = materials.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();

                        if (materialContract != null && materialContract.Count > 0)
                        {
                            MedicalContractIds.AddRange(materialContract);
                        }
                    }

                    if (MedicalContractIds != null && MedicalContractIds.Count > 0)
                    {
                        HisMedicalContractViewFilter MedicalContractFilter = new HisMedicalContractViewFilter();
                        MedicalContractFilter.IDs = MedicalContractIds;
                        MedicalContract = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/GetView", ApiConsumers.MosConsumer, MedicalContractFilter, new CommonParam());
                    }

                    List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO> MedicalContractADO = new List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>();
                    if (MedicalContract != null && MedicalContract.Count > 0)
                    {
                        foreach (var item in medicines)
                        {
                            MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO ado = new MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO();

                            var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>(ado, Contract);
                            ado.MEDICINE_ID = item.ID;
                            MedicalContractADO.Add(ado);
                        }

                        foreach (var item in materials)
                        {
                            MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO ado = new MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO();

                            var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>(ado, Contract);
                            ado.MATERIAL_ID = item.ID;
                            MedicalContractADO.Add(ado);
                        }
                    }

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                    MPS.Processor.Mps000199.PDO.Mps000199PDO Mps000170RDO = new MPS.Processor.Mps000199.PDO.Mps000199PDO(
                        this.impMest,
                        hisImpMestMedicines,
                        hisImpMestMaterials,
                        impMestBloodPrint,
                        _ImpMestUser,
                        MedicalContractADO
                    );

                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000170RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000170RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000170RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                }
                else if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
                {
                    MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                    userFilter.IMP_MEST_ID = this.ImpMestId;
                    var _ImpMestUser = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                    _ImpMestUser = _ImpMestUser.OrderBy(p => p.ID).ToList();
                    HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                    mediFilter.IMP_MEST_ID = this.ImpMestId;
                    var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                    HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                    mateFilter.IMP_MEST_ID = this.ImpMestId;
                    var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);

                    var impMestBloodPrint = MapImpMestBloodFromSDO(this.impMestBloods);

                    List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                    if (hisImpMestMedicines != null && hisImpMestMedicines.Count > 0)
                    {
                        HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = hisImpMestMedicines.Select(o => o.MEDICINE_ID).Distinct().ToList();
                        medicines = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
                    }
                    List<HIS_MATERIAL> materials = new List<HIS_MATERIAL>();
                    if (hisImpMestMaterials != null && hisImpMestMaterials.Count > 0)
                    {
                        HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = hisImpMestMaterials.Select(o => o.MATERIAL_ID).Distinct().ToList();
                        materials = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, new CommonParam());
                    }

                    List<V_HIS_MEDICAL_CONTRACT> MedicalContract = new List<V_HIS_MEDICAL_CONTRACT>();
                    List<long> MedicalContractIds = new List<long>();

                    if (medicines != null && medicines.Count > 0)
                    {
                        List<long> medicineContract = medicines.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();

                        if (medicineContract != null && medicineContract.Count > 0)
                        {
                            MedicalContractIds.AddRange(medicineContract);
                        }
                    }

                    if (materials != null && materials.Count > 0)
                    {
                        List<long> materialContract = materials.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();

                        if (materialContract != null && materialContract.Count > 0)
                        {
                            MedicalContractIds.AddRange(materialContract);
                        }
                    }

                    if (MedicalContractIds != null && MedicalContractIds.Count > 0)
                    {
                        HisMedicalContractViewFilter MedicalContractFilter = new HisMedicalContractViewFilter();
                        MedicalContractFilter.IDs = MedicalContractIds;
                        MedicalContract = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/GetView", ApiConsumers.MosConsumer, MedicalContractFilter, new CommonParam());
                    }

                    List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO> MedicalContractADO = new List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>();
                    if (MedicalContract != null && MedicalContract.Count > 0)
                    {
                        foreach (var item in medicines)
                        {
                            MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO ado = new MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO();

                            var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>(ado, Contract);
                            ado.MEDICINE_ID = item.ID;
                            MedicalContractADO.Add(ado);
                        }

                        foreach (var item in materials)
                        {
                            MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO ado = new MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO();

                            var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>(ado, Contract);
                            ado.MATERIAL_ID = item.ID;
                            MedicalContractADO.Add(ado);
                        }
                    }

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                    MPS.Processor.Mps000199.PDO.Mps000199PDO Mps000199RDO = new MPS.Processor.Mps000199.PDO.Mps000199PDO(
                        this.impMest,
                        hisImpMestMedicines,
                        hisImpMestMaterials,
                        impMestBloodPrint,
                        _ImpMestUser,
                        MedicalContractADO
                       );
                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                }
                WaitingManager.Hide();
                if (PrintData != null)
                {
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                else
                    throw new Exception("lỗi in mps000199");

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InBienBanKiemNhapTuNhaCungCap(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                List<HIS_MATERIAL> materials = new List<HIS_MATERIAL>();
                MOS.Filter.HisImpMestViewFilter impMestFilter = new MOS.Filter.HisImpMestViewFilter();
                impMestFilter.ID = this.ImpMestId;
                this.impMest = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("/api/HisImpMest/GetView", ApiConsumers.MosConsumer, impMestFilter, param).FirstOrDefault();
                MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                userFilter.IMP_MEST_ID = this.ImpMestId;
                var rs = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                rs = rs.OrderBy(p => p.ID).ToList();
                MOS.Filter.HisImpMestBloodFilter bloodFilter = new MOS.Filter.HisImpMestBloodFilter();
                bloodFilter.IMP_MEST_ID = this.ImpMestId;
                var bloodData = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_BLOOD>>("/api/HisImpMestBlood/GetView", ApiConsumers.MosConsumer, bloodFilter, param);
                bloodData = bloodData.OrderBy(o => o.ID).ToList();
                MOS.EFMODEL.DataModels.HIS_SUPPLIER supplier = new HIS_SUPPLIER();

                if (this.impMest != null && this.impMest.SUPPLIER_ID != null)
                {
                    supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == this.impMest.SUPPLIER_ID);
                }

                if (this.impMestMedicines != null && this.impMestMedicines.Count > 0)
                {
                    MOS.Filter.HisMedicineFilter hisMedicineFilter = new HisMedicineFilter();
                    hisMedicineFilter.IDs = this.impMestMedicines.Select(o => o.MEDICINE_ID).Distinct().ToList();
                    medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, hisMedicineFilter, param);
                }

                MOS.EFMODEL.DataModels.V_HIS_BID bid = new V_HIS_BID();

                if (this.impMestMaterials != null && this.impMestMaterials.Count > 0)
                {
                    MOS.Filter.HisMaterialFilter hisMaterialFilter = new HisMaterialFilter();
                    hisMaterialFilter.IDs = this.impMestMaterials.Select(o => o.MATERIAL_ID).Distinct().ToList();
                    materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, hisMaterialFilter, param);
                }

                List<V_HIS_IMP_MEST_MEDICINE> impMestMedicinePrints = new List<V_HIS_IMP_MEST_MEDICINE>();
                foreach (var item in this.impMestMedicines)
                {
                    V_HIS_IMP_MEST_MEDICINE impMestMedicine = new V_HIS_IMP_MEST_MEDICINE();
                    AutoMapper.Mapper.CreateMap<ImpMestMedicineSDODetail, V_HIS_IMP_MEST_MEDICINE>();
                    impMestMedicine = AutoMapper.Mapper.Map<V_HIS_IMP_MEST_MEDICINE>(item);
                    impMestMedicinePrints.Add(impMestMedicine);
                }

                List<V_HIS_MEDICAL_CONTRACT> MedicalContract = new List<V_HIS_MEDICAL_CONTRACT>();
                List<long> MedicalContractIds = new List<long>();

                if (medicines != null && medicines.Count > 0)
                {
                    List<long> medicineContract = medicines.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();
                    
                    if (medicineContract != null && medicineContract.Count > 0)
                    {
                        MedicalContractIds.AddRange(medicineContract);
                    }
                }

                if (materials != null && materials.Count > 0)
                {
                    List<long> materialContract = materials.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();
                    
                    if (materialContract != null && materialContract.Count > 0)
                    {
                        MedicalContractIds.AddRange(materialContract);
                    }
                }

                if (MedicalContractIds != null && MedicalContractIds.Count > 0)
                {
                    HisMedicalContractViewFilter MedicalContractFilter = new HisMedicalContractViewFilter();
                    MedicalContractFilter.IDs = MedicalContractIds;
                    MedicalContract = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/GetView", ApiConsumers.MosConsumer, MedicalContractFilter, new CommonParam());
                }
                
                List<MPS.Processor.Mps000085.PDO.MedicalContractADO> MedicalContractADO = new List<MPS.Processor.Mps000085.PDO.MedicalContractADO>();

                if (MedicalContract != null && MedicalContract.Count > 0)
                {
                    foreach (var item in medicines)
                    {
                        MPS.Processor.Mps000085.PDO.MedicalContractADO ado = new MPS.Processor.Mps000085.PDO.MedicalContractADO();

                        var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000085.PDO.MedicalContractADO>(ado, Contract);
                        ado.MEDICINE_ID = item.ID;
                        MedicalContractADO.Add(ado);
                    }

                    foreach (var item in materials)
                    {
                        MPS.Processor.Mps000085.PDO.MedicalContractADO ado = new MPS.Processor.Mps000085.PDO.MedicalContractADO();

                        var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000085.PDO.MedicalContractADO>(ado, Contract);
                        ado.MATERIAL_ID = item.ID;
                        MedicalContractADO.Add(ado);
                    }
                }

                WaitingManager.Show();

                var ImpMestMaterialPrints = MapImpMestMaterialFromSDO(this.impMestMaterials);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                MPS.Processor.Mps000085.PDO.Mps000085PDO mps0000085RDO = new MPS.Processor.Mps000085.PDO.Mps000085PDO(
                 this.impMest,
                 impMestMedicinePrints,
                 ImpMestMaterialPrints,
                 rs,
                 medicines,
                 materials,
                 supplier,
                 MedicalContractADO,
                 bloodData
                  );
                WaitingManager.Hide();

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps0000085RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps0000085RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps0000085RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps0000085RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuNhapTuNhaCungCap(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                WaitingManager.Show();


                List<V_HIS_IMP_MEST_MEDICINE> impMestMedicinePrints = new List<V_HIS_IMP_MEST_MEDICINE>();
                foreach (var item in this.impMestMedicines)
                {
                    V_HIS_IMP_MEST_MEDICINE impMestMedicine = new V_HIS_IMP_MEST_MEDICINE();
                    AutoMapper.Mapper.CreateMap<ImpMestMedicineSDODetail, V_HIS_IMP_MEST_MEDICINE>();
                    impMestMedicine = AutoMapper.Mapper.Map<V_HIS_IMP_MEST_MEDICINE>(item);
                    impMestMedicinePrints.Add(impMestMedicine);
                }

                var ImpMestMaterialPrints = MapImpMestMaterialFromSDO(this.impMestMaterials);

                List<HIS_MEDICINE> _Medicines = new List<HIS_MEDICINE>();
                List<HIS_MATERIAL> _Materials = new List<HIS_MATERIAL>();
                if (impMestMedicinePrints != null && impMestMedicinePrints.Count > 0)
                {
                    List<long> _MedicineIds = impMestMedicinePrints.Select(p => p.MEDICINE_ID).ToList();
                    MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                    medicineFilter.IDs = _MedicineIds;
                    _Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);

                }
                if (ImpMestMaterialPrints != null && ImpMestMaterialPrints.Count > 0)
                {
                    List<long> _MaterialIds = ImpMestMaterialPrints.Select(p => p.MATERIAL_ID).ToList();
                    MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                    materialFilter.IDs = _MaterialIds;
                    _Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                }

                List<V_HIS_MEDICAL_CONTRACT> MedicalContract = new List<V_HIS_MEDICAL_CONTRACT>();
                List<long> MedicalContractIds = new List<long>();

                if (_Medicines != null && _Medicines.Count > 0)
                {
                    List<long> medicineContract = _Medicines.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();
                    
                    if (medicineContract != null && medicineContract.Count > 0)
                    {
                        MedicalContractIds.AddRange(medicineContract);
                    }
                }

                if (_Materials != null && _Materials.Count > 0)
                {
                    List<long> materialContract = _Materials.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();
                    
                    if (materialContract != null && materialContract.Count > 0)
                    {
                        MedicalContractIds.AddRange(materialContract);
                    }
                }

                if (MedicalContractIds != null && MedicalContractIds.Count > 0)
                {
                    HisMedicalContractViewFilter MedicalContractFilter = new HisMedicalContractViewFilter();
                    MedicalContractFilter.IDs = MedicalContractIds;
                    MedicalContract = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/GetView", ApiConsumers.MosConsumer, MedicalContractFilter, new CommonParam());
                }

                List<MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO> MedicalContractADO = new List<MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO>();

                if (MedicalContract != null && MedicalContract.Count > 0)
                {
                    foreach (var item in _Medicines)
                    {
                        MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO ado = new MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO();

                        var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO>(ado, Contract);
                        ado.MEDICINE_ID = item.ID;
                        MedicalContractADO.Add(ado);
                    }

                    foreach (var item in _Materials)
                    {
                        MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO ado = new MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO();

                        var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO>(ado, Contract);
                        ado.MATERIAL_ID = item.ID;
                        MedicalContractADO.Add(ado);
                    }
                }

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                MPS.Processor.Mps000141.PDO.Mps000141PDO pdo = new MPS.Processor.Mps000141.PDO.Mps000141PDO(
                 this.impMest,
                 impMestMedicinePrints,
                 ImpMestMaterialPrints,
                 _Medicines,
                 _Materials,
                 BackendDataWorker.Get<HIS_IMP_SOURCE>(),
                 MedicalContractADO
                );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
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

        private void InPhieuNhapThuHoi(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                MOS.Filter.HisImpMestFilter hisImpMestFilter = new MOS.Filter.HisImpMestFilter();
                hisImpMestFilter.ID = this.ImpMestId;
                var ImpMest = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GET, ApiConsumers.MosConsumer, hisImpMestFilter, param).FirstOrDefault();
                var impMestMedicinePrints = new List<V_HIS_IMP_MEST_MEDICINE>();
                MOS.Filter.HisImpMestMedicineFilter impMestMedicineViewFilter = new HisImpMestMedicineFilter();
                impMestMedicineViewFilter.IMP_MEST_ID = this.ImpMestId;
                impMestMedicinePrints = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMestMedicineViewFilter, param);
                var impMestMaterialPrints = new List<V_HIS_IMP_MEST_MATERIAL>();
                MOS.Filter.HisImpMestMaterialViewFilter impMestMaterialViewFilter = new HisImpMestMaterialViewFilter();
                impMestMaterialViewFilter.IMP_MEST_ID = this.ImpMestId;
                impMestMaterialPrints = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, impMestMaterialViewFilter, param);

                MOS.Filter.HisExpMestMedicineViewFilter hisExpMestMedicineViewFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                hisExpMestMedicineViewFilter.EXP_MEST_ID = ImpMest.MOBA_EXP_MEST_ID;
                var expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, hisExpMestMedicineViewFilter, param);

                MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialViewFilter = new MOS.Filter.HisExpMestMaterialViewFilter();
                expMestMaterialViewFilter.EXP_MEST_ID = ImpMest.MOBA_EXP_MEST_ID;
                var expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialViewFilter, param);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);
                MPS.Processor.Mps000084.PDO.Mps000084PDO pdo = new MPS.Processor.Mps000084.PDO.Mps000084PDO(
                this.impMest,
                impMestMedicinePrints,
                impMestMaterialPrints,
                expMestMedicines,
                expMestMaterials
                );

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
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

        private void InPhieuNhapMauTuNCC(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                WaitingManager.Show();

                var impMestBloodPrint = MapImpMestBloodFromSDO(this.impMestBloods);

                MPS.Processor.Mps000149.PDO.Mps000149PDO pdo = new MPS.Processor.Mps000149.PDO.Mps000149PDO(
                 this.impMest,
                 impMestBloodPrint
                );
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
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

        private void RunPrint226()
        {
            try
            {
                store.RunPrintTemplate("Mps000226", delegateRunPrint226);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool delegateRunPrint226(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.impMestBloods != null && this.impMestBloods.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();

                    MOS.Filter.HisExpMestViewFilter hisExpMestViewFilter = new MOS.Filter.HisExpMestViewFilter();
                    hisExpMestViewFilter.ID = this.impMest.CHMS_EXP_MEST_ID;
                    var expMestView = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, hisExpMestViewFilter, null);

                    MPS.Processor.Mps000226.PDO.Mps000226PDO.Mps000226Key mps000226Key = new MPS.Processor.Mps000226.PDO.Mps000226PDO.Mps000226Key();

                    if (expMestView != null && expMestView.Count > 0)
                    {
                        mps000226Key.EXP_MEDI_STOCK_CODE = expMestView.FirstOrDefault().MEDI_STOCK_CODE;
                        mps000226Key.EXP_MEDI_STOCK_NAME = expMestView.FirstOrDefault().MEDI_STOCK_NAME;
                    }

                    MPS.Processor.Mps000226.PDO.Mps000226PDO rdo = null;

                    var impMestBloodPrint = MapImpMestBloodFromSDO(this.impMestBloods);

                    if (impMestBloodPrint == null || impMestBloodPrint.Count <= 0)
                        return false;

                    rdo = new MPS.Processor.Mps000226.PDO.Mps000226PDO(this.impMest, impMestBloodPrint, mps000226Key, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    if (result)
                    {
                        //this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuNhapChuyenKho(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if ((this.impMestMedicines != null && this.impMestMedicines.Count > 0) || (this.impMestMaterials != null && this.impMestMaterials.Count > 0))
                {
                    CommonParam param = new CommonParam();
                    List<V_HIS_EXP_MEST> datas = new List<V_HIS_EXP_MEST>();
                    WaitingManager.Show();

                    if (this.impMest != null && this.impMest.CHMS_EXP_MEST_ID > 0)
                    {
                        MOS.Filter.HisExpMestViewFilter filter = new HisExpMestViewFilter();
                        filter.ID = this.impMest.CHMS_EXP_MEST_ID;
                        datas = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, filter, null);
                    }

                    MPS.Processor.Mps000143.PDO.Mps000143PDO.Mps000143Key _Mps000143Key = new MPS.Processor.Mps000143.PDO.Mps000143PDO.Mps000143Key();
                    if (datas != null && datas.Count > 0)
                    {
                        _Mps000143Key.EXP_MEDI_STOCK_CODE = datas[0].MEDI_STOCK_CODE;
                        _Mps000143Key.EXP_MEDI_STOCK_NAME = datas[0].MEDI_STOCK_NAME;
                    }

                    List<V_HIS_IMP_MEST_MEDICINE> impMestMedicinePrints = new List<V_HIS_IMP_MEST_MEDICINE>();
                    foreach (var item in this.impMestMedicines)
                    {
                        V_HIS_IMP_MEST_MEDICINE impMestMedicine = new V_HIS_IMP_MEST_MEDICINE();
                        AutoMapper.Mapper.CreateMap<ImpMestMedicineSDODetail, V_HIS_IMP_MEST_MEDICINE>();
                        impMestMedicine = AutoMapper.Mapper.Map<V_HIS_IMP_MEST_MEDICINE>(item);
                        impMestMedicinePrints.Add(impMestMedicine);
                    }

                    var ImpMestMaterialPrints = MapImpMestMaterialFromSDO(this.impMestMaterials);

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0);

                    if ((ImpMestMaterialPrints != null && ImpMestMaterialPrints.Count > 0) || (impMestMedicinePrints != null && impMestMedicinePrints.Count > 0))
                    {

                        long keyPrintType = ConfigApplicationWorker.Get<long>(Base.AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__CHE_DO_IN_GOP_PHIEU_TRA);

                        if (keyPrintType == 1)
                        {
                            _Mps000143Key.KEY_NAME_TITLES = "";
                            MPS.Processor.Mps000143.PDO.Mps000143PDO rdo = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, impMestMedicinePrints, ImpMestMaterialPrints, _Mps000143Key, ConfigApplications.NumberSeperator);
                            PrintData(printTypeCode, fileName, rdo, false, inputADO, ref result);
                        }
                        else
                        {
                            var _ImpMestMedi_Ts = new List<V_HIS_IMP_MEST_MEDICINE>();
                            var _ImpMestMedi_GNs = new List<V_HIS_IMP_MEST_MEDICINE>();
                            var _ImpMestMedi_HTs = new List<V_HIS_IMP_MEST_MEDICINE>();
                            var _ImpMestMedi_TDs = new List<V_HIS_IMP_MEST_MEDICINE>();
                            var _ImpMestMedi_PXs = new List<V_HIS_IMP_MEST_MEDICINE>();
                            var _ImpMestMedi_Others = new List<V_HIS_IMP_MEST_MEDICINE>();

                            if (impMestMedicinePrints != null && impMestMedicinePrints.Count > 0)
                            {
                                var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                                var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                                bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                                bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                                bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                                bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);

                                _ImpMestMedi_Ts = impMestMedicinePrints.Where(p => !mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0)).ToList();
                                _ImpMestMedi_GNs = impMestMedicinePrints.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn).ToList();
                                _ImpMestMedi_HTs = impMestMedicinePrints.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht).ToList();
                                _ImpMestMedi_TDs = impMestMedicinePrints.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc).ToList();
                                _ImpMestMedi_PXs = impMestMedicinePrints.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px).ToList();

                                _ImpMestMedi_Others = impMestMedicinePrints.Where(p => mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0) &&
                                    p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                                && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                                && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                                && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX).ToList();
                            }

                            #region thuoc thuong
                            if (_ImpMestMedi_Ts != null && _ImpMestMedi_Ts.Count > 0)
                            {
                                _Mps000143Key.KEY_NAME_TITLES = "THUỐC THƯỜNG";
                                MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_Ts = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, _ImpMestMedi_Ts, null, _Mps000143Key, ConfigApplications.NumberSeperator);
                                PrintData(printTypeCode, fileName, rdo_Ts, false, inputADO, ref result);
                            }
                            #endregion

                            #region Gay nghien, huong than
                            if ((_ImpMestMedi_GNs != null && _ImpMestMedi_GNs.Count > 0) || (_ImpMestMedi_HTs != null && _ImpMestMedi_HTs.Count > 0))
                            {
                                long keyPrintTypeHTGN = ConfigApplicationWorker.Get<long>(Base.AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                                if (keyPrintTypeHTGN == 1)
                                {
                                    List<V_HIS_IMP_MEST_MEDICINE> DataGroups = new List<V_HIS_IMP_MEST_MEDICINE>();

                                    if (_ImpMestMedi_GNs != null && _ImpMestMedi_GNs.Count > 0)
                                    {
                                        DataGroups.AddRange(_ImpMestMedi_GNs);
                                    }

                                    if (_ImpMestMedi_HTs != null && _ImpMestMedi_HTs.Count > 0)
                                    {
                                        DataGroups.AddRange(_ImpMestMedi_HTs);
                                    }

                                    _Mps000143Key.KEY_NAME_TITLES = "GÂY NGHIỆN, HƯỚNG THẦN";
                                    MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_GNHTs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, DataGroups, null, _Mps000143Key, ConfigApplications.NumberSeperator);
                                    PrintData(printTypeCode, fileName, rdo_GNHTs, false, inputADO, ref result);
                                }
                                else
                                {
                                    if (_ImpMestMedi_GNs != null && _ImpMestMedi_GNs.Count > 0)
                                    {
                                        _Mps000143Key.KEY_NAME_TITLES = "GÂY NGHIỆN";
                                        MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_GNs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, _ImpMestMedi_GNs, null, _Mps000143Key, ConfigApplications.NumberSeperator);
                                        PrintData(printTypeCode, fileName, rdo_GNs, false, inputADO, ref result);
                                    }

                                    if (_ImpMestMedi_HTs != null && _ImpMestMedi_HTs.Count > 0)
                                    {
                                        _Mps000143Key.KEY_NAME_TITLES = "HƯỚNG THẦN";
                                        MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_HTs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, _ImpMestMedi_HTs, null, _Mps000143Key, ConfigApplications.NumberSeperator);
                                        PrintData(printTypeCode, fileName, rdo_HTs, false, inputADO, ref result);
                                    }
                                }
                            }
                            #endregion

                            #region thuoc doc
                            if (_ImpMestMedi_TDs != null && _ImpMestMedi_TDs.Count > 0)
                            {
                                _Mps000143Key.KEY_NAME_TITLES = "ĐỘC";
                                MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_TDs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, _ImpMestMedi_TDs, null, _Mps000143Key, ConfigApplications.NumberSeperator);
                                PrintData(printTypeCode, fileName, rdo_TDs, false, inputADO, ref result);
                            }
                            #endregion

                            #region thuoc phong xa
                            if (_ImpMestMedi_PXs != null && _ImpMestMedi_PXs.Count > 0)
                            {
                                _Mps000143Key.KEY_NAME_TITLES = "PHÓNG XẠ";
                                MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_PXs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, _ImpMestMedi_PXs, null, _Mps000143Key, ConfigApplications.NumberSeperator);
                                PrintData(printTypeCode, fileName, rdo_PXs, false, inputADO, ref result);
                            }
                            #endregion

                            #region thuoc khac
                            if (_ImpMestMedi_Others != null && _ImpMestMedi_Others.Count > 0)
                            {
                                _Mps000143Key.KEY_NAME_TITLES = "KHÁC";
                                MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_Ks = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, _ImpMestMedi_Others, null, _Mps000143Key, ConfigApplications.NumberSeperator);
                                PrintData(printTypeCode, fileName, rdo_Ks, false, inputADO, ref result);
                            }
                            #endregion

                            #region vat tu
                            if (ImpMestMaterialPrints != null && ImpMestMaterialPrints.Count > 0)
                            {
                                _Mps000143Key.KEY_NAME_TITLES = "VẬT TƯ";
                                MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_VTs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, null, ImpMestMaterialPrints, _Mps000143Key, ConfigApplications.NumberSeperator);
                                PrintData(printTypeCode, fileName, rdo_VTs, false, inputADO, ref result);
                            }
                            #endregion
                        }
                    }
                    WaitingManager.Hide();
                }

                //RunPrint226();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_IMP_MEST_MEDICINE> MapImpMestMedicineFromSDO(List<ImpMestMedicineSDODetail> input)
        {
            List<V_HIS_IMP_MEST_MEDICINE> result = new List<V_HIS_IMP_MEST_MEDICINE>();
            try
            {
                foreach (var item in input)
                {
                    V_HIS_IMP_MEST_MEDICINE impMestMedicine = new V_HIS_IMP_MEST_MEDICINE();
                    AutoMapper.Mapper.CreateMap<ImpMestMedicineSDODetail, V_HIS_IMP_MEST_MEDICINE>();
                    impMestMedicine = AutoMapper.Mapper.Map<V_HIS_IMP_MEST_MEDICINE>(item);
                    result.Add(impMestMedicine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<V_HIS_IMP_MEST_MATERIAL> MapImpMestMaterialFromSDO(List<ImpMestMaterialSDODetail> input)
        {
            List<V_HIS_IMP_MEST_MATERIAL> result = new List<V_HIS_IMP_MEST_MATERIAL>();
            try
            {
                foreach (var item in input)
                {
                    V_HIS_IMP_MEST_MATERIAL impMestMedicine = new V_HIS_IMP_MEST_MATERIAL();
                    AutoMapper.Mapper.CreateMap<ImpMestMedicineSDODetail, V_HIS_IMP_MEST_MATERIAL>();
                    impMestMedicine = AutoMapper.Mapper.Map<V_HIS_IMP_MEST_MATERIAL>(item);
                    result.Add(impMestMedicine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InPhieuNhapChuyenKhoThuocGayNghienHuongThan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                WaitingManager.Show();

                var impMestMedicinePrints = MapImpMestMedicineFromSDO(this.impMestMedicines);

                MPS.Processor.Mps000142.PDO.Mps000142PDO pdo = new MPS.Processor.Mps000142.PDO.Mps000142PDO(
                 this.impMest,
                 impMestMedicinePrints
                );
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
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

        private void InPhieuNhapChuyenKhoThuocKhongPhaiGayNghienHuongThan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                WaitingManager.Show();

                var impMestMedicinePrints = MapImpMestMedicineFromSDO(this.impMestMedicines);

                var ImpMestMaterialPrints = MapImpMestMaterialFromSDO(this.impMestMaterials);

                MPS.Processor.Mps000145.PDO.Mps000145PDO pdo = new MPS.Processor.Mps000145.PDO.Mps000145PDO(
                 this.impMest,
                 impMestMedicinePrints,
                 ImpMestMaterialPrints
                );
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
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

        private void PrintData(string printTypeCode, string fileName, object data, bool printNow, Inventec.Common.SignLibrary.ADO.InputADO inputADO, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (printNow)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
