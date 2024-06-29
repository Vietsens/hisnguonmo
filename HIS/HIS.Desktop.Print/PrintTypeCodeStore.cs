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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Print
{
    public partial class PrintTypeCodeStore
    {
        public const string PRINT_TYPE_CODE__MPS000102 = "Mps000102";
        public const string PRINT_TYPE_CODE__EXPORT_BLOOD__MPS000107 = "Mps000107";
        public const string PRINT_TYPE_CODE__MPS000108 = "Mps000108";
        public const string PRINT_TYPE_CODE__MPS000109 = "Mps000109";
        public const string PRINT_TYPE_CODE__MPS000110 = "Mps000110";
        public const string PRINT_TYPE_CODE__HoaDonTTTheoYeuCauDichVu_MPS000103 = "Mps000103";
        public const string PRINT_TYPE_CODE__PhieuChiDinhDuaVaoGiaoDichThanhToan_Mps000105 = "Mps000105";
        public const string PRINT_TYPE_CODE__HoaDonThanhToanChiTietDichVu_Mps000106 = "Mps000106";
        public const string PRINT_TYPE_CODE__PhieuThuThanhToan_MPS000111 = "Mps000111";
        public const string PRINT_TYPE_CODE__PhieuThuTamUng_MPS000112 = "Mps000112";
        public const string PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113 = "Mps000113";
        public const string PRINT_TYPE_CODE__BienLaiThuPhiLePhi_MPS000114 = "Mps000114";
        public const string PRINT_TYPE_CODE__InHoaDonDo_MPS000115 = "Mps000115";

        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_PHIE_KET_QUA_NOI_SOI__MPS000021 = "Mps000021";
        public const string PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037 = "Mps000037";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KET_QUA_THEO_DOI_TRUYEN_DICH__MPS000058 = "Mps000058";
        public const string PRINT_TYPE_CODE__BANGKE__NOI_TRU_GOM_NHOM_THEO_KHOA__MPS000059 = "Mps000059";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC_Y_HOC_CO_TRUYEN__MPS000050 = "Mps000050";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC_TRONG_DANH_MUC__MPS000051 = "Mps000051";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC_TONG_HOP__MPS000054 = "Mps000054";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_GIAY_THU_PHAN_UNG_THUOC__MPS000055 = "Mps000055";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_GIAY_THU_PHAN_UNG_THUOC__MPS000056 = "Mps000056";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KET_QUA_DO_DAU_HIEU_SINH_TON__MPS000057 = "Mps000057";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KET_QUA_THEO_DOI_TRUYEN_DICH_TONG_HOP__MPS000058 = "Mps000058";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_TO_ROI__MPS000062 = "Mps000062";
        public const string PRINT_TYPE_CODE__BANG_KE__NOI_TRU_KHONG_GOM_NHOM_THEO_KHOA__MPS000060 = "Mps000060";
        public const string PRINT_TYPE_CODE__BANG_KE__NGOAI_TRU_KHONG_GOM_NHOM_THEO_KHOA__MPS000061 = "Mps000061";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_PHCN_TONG_HOP__MPS000063 = "Mps000063";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHI_DINH_KI_THUAT_PHCN__MPS000064 = "Mps000064";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053 = "Mps000053";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_TRICH_BIEN_BAN_HOI_CHAN__MPS000065 = "Mps000065";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_SO_BIEN_BAN_HOI_CHAN__MPS000066 = "Mps000066";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_HUONG_DAN_SU_DUNG__MPS000067 = "Mps000067";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KET_QUA_CHAM_SOC__MPS000069 = "Mps000069";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC_NGOAI_TRU__MPS000070 = "Mps000070";
        public const string PRINT_TYPE_CODE__YEU_CAU_KHAM_CHUYEN_KHOA__MPS000071 = "Mps000071";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_TO_DIEU_TRI_TONG_HOP__MPS000072 = "Mps000072";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KET_QUA_CHAM_SOC_TONG_HOP__MPS000073 = "Mps000073";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KET_QUA_THEO_DOI_TRUYEN_DICH__MPS000074 = "Mps000074";
        public const string PRINT_TYPE_CODE__BANGKE__BHYT_NGOAI_TRU_TONG_HOP__MPS000075 = "Mps000075";
        public const string PRINT_TYPE_CODE__BANGKE__BHYT_NOI_TRU_TONG_HOP__MPS000076 = "Mps000076";
        
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_BENH_AN_NGOAI_TRU__MPS000012 = "Mps000012";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KHAM_SUC_KHOE_CAN_BO__MPS000013 = "Mps000013";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_XET_NGHIEM__MPS000014 = "Mps000014";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_CHUP_CAT_LOP_VI_TINH__MPS000015 = "Mps000015";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_CHIEU_CHUP_CONG_HUONG_TU__MPS000016 = "Mps000016";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_CHIEU_CHUP_X_QUANG__MPS000017 = "Mps000017";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_CHIEU_CHUP_X_QUANG_CAC_DICH_VU__MPS000018 = "Mps000018";
        public const string PRINT_TYPE_CODE__HOI_CHAN__TRICH_BIEN_BAN_HOI_CHAN__MPS000019 = "Mps000019";
        public const string PRINT_TYPE_CODE__HOI_CHAN__SO_BIEN_BAN_HOI_CHAN__MPS000020 = "Mps000020";
        //public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_PHIE_KET_QUA_NOI_SOI__MPS000021 = "Mps000021";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_DIEN_TIM__MPS000022 =
"Mps000022";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_DIEN_NAO__MPS000023 = "Mps000023";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_SIEU_AM__MPS000024 = "Mps000024";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KHAM__MPS000025 = "Mps000025";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026 = "Mps000026";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_PHIEU_XET_NGHIEM_DOM_SOI_TRUC_TIEP__MPS000027 = "Mps000027";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028 = "Mps000028";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029 = "Mps000029";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030 = "Mps000030";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031 = "Mps000031";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT_THU_THUAT__MPS000033 = "Mps000033";
        public const string PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_KHAM__MPS000034 = "Mps000034";
        //public const string PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037 = "Mps000037";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_GIAY_CAM_DOAN_CHAP_NHAN_pHAU_THUAT_THU_THUAT_VA_GAY_ME_HOI_SUC__MPS000035 = "Mps000035";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036 = "Mps000036";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038 = "Mps000038";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_THAM_DO_CHUC_NANG__MPS000039 = "Mps000039";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040 = "Mps000040";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_DICH_VU_KHAC__MPS000041 = "Mps000041";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042 = "Mps000042";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC__MPS000044 = "Mps000044";
        public const string PRINT_TYPE_CODE__IN__DON_THUOC_VAT_TU__MPS000043 = "Mps000043";

        public const string PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118 = "Mps000118";

        public const string PRINT_TYPE_CODE__SUIM___KET_QUA_DIEN_NAO = "00001";
        public const string PRINT_TYPE_CODE__SUIM___KET_QUA_DIEN_TIM = "00002";
        public const string PRINT_TYPE_CODE__SUIM___KET_QUA_SIEU_AM = "00003";
        public const string PRINT_TYPE_CODE__FUEX___KET_QUA_THAM_DO_CHUC_NANG = "00004";
        public const string PRINT_TYPE_CODE__DIIM___KET_QUA_CHIEU_CHUP_CONG_HUONG_TU = "00005";
        public const string PRINT_TYPE_CODE__DIIM___KET_QUA_CHIEU_CAT_LOP_VI_TINH = "00006";
        public const string PRINT_TYPE_CODE__DIIM___KET_QUA_CHIEU_CHUP_X_QUANG = "00007";
        public const string PRINT_TYPE_CODE__OTHER___KET_QUA_DICH_VU_KHAC = "00008";
        public const string PRINT_TYPE_CODE__ENDO___KET_QUA_NOI_SOI = "00009";
        public const string PRINT_TYPE_CODE__PHIEU_SO_KET_15_NGAY_DIEU_TRI = "00010";
        public const string PRINT_TYPE_CODE__SURG___GIAY_CHUNG_NHAN = "00011";
        public const string PRINT_TYPE_CODE__TEST___PHIEU_XET_NGHIEM_KHAC = "00012";
        public const string PRINT_TYPE_CODE__TEST___PHIEU_XET_NGHIEM_KET_QUA = "00013";
        public const string PRINT_TYPE_CODE__ENDO___PHIEU_NOI_SOI_KHAC = "00014";
        public const string PRINT_TYPE_CODE__ENDO___PHIEU_NOI_SOI_DAI_TRANG = "00016";
        public const string PRINT_TYPE_CODE__SERVICE_REQ_REGISTER = "Mps000001";
        public const string PRINT_TYPE_CODE__BANGKE__NGOAI_TRU_BHYT__MPS000002 = "Mps000002";
        public const string PRINT_TYPE_CODE__BANGKE__NGOAI_TRU_TEMPLATE8_BHYT__MPS000003 = "Mps000003";
        public const string PRINT_TYPE_CODE__BANGKE__NGOAI_TRU_VIENPHI__MPS000004 = "Mps000004";
        public const string PRINT_TYPE_CODE__BANGKE__NOI_TRU_BHYT__MPS000005 = "Mps000005";
        public const string PRINT_TYPE_CODE__BANGKE__NOI_TRU_VIENPHI__MPS000006 = "Mps000006";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KHAM_BENH_VAO_VIEN__MPS000007 = "Mps000007";
        public const string PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008 = "Mps000008";
        public const string PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010 = "Mps000010";
        public const string PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011 = "Mps000011";

        public const string PRINT_TYPE_CODE__PhieuYeuCauNhapThuHoi_MPS000073 = "Mps000073";
        public const string PRINT_TYPE_CODE__BIEUMAU__YEU_CAU_TAM_UNG__MPS000091 = "Mps000091";
        public const string PRINT_TYPE_CODE__BIEUMAU__XAC_NHAN_TAM_UNG__MPS000489 = "Mps000489";
        public const string PRINT_TYPE_CODE__IN_HOA_DON_CHITIET_DICHVU_MPS000152 = "Mps000152";
        public const string PRINT_TYPE_CODE__PhieuChamSocTongHop_MPS000151 = "Mps000151";
        public const string PRINT_TYPE_CODE__InHoaDonDo_MPS000157 = "Mps000157";
        public const string PRINT_TYPE_CODE__BIEUMAU__IN_KET_QUA_XET_NGHIEM__MPS000096 = "Mps000096";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_BAR_CODE__MPS000077 = "Mps000077";
        public const string PRINT_TYPE_CODE__PHIEU_YEU_CAU_VAO_VIEN_MPS000007 = "Mps000007";

        public const string PRINT_TYPE_CODE__BIEU_MAU_PHIEU_XUAT_KHAC__MPS000165 = "Mps000165";

        public const string PRINT_TYPE_CODE__PhieuYeuCauNhapThuHoi_MPS000084 = "Mps000084";
        public const string PRINT_TYPE_CODE__BienBanKiemNhapTuNhaCungCap_MPS000085 = "MPS000085";
        public const string PRINT_TYPE_CODE__PhieuXuatChuyenKho_MPS000086 = "Mps000086";
        public const string PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocGayNghienHuongThan_MPS000089 = "Mps000089";
        public const string PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan_MPS000090 = "Mps000090";
        public const string PRINT_TYPE_CODE__PhieuXuatBan_MPS000092 = "Mps000092";
        public const string PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099 = "Mps000099";
        public const string PRINT_TYPE_CODE__PhieuXuatSuDungTheoDieuKien_MPS000134 = "Mps000134";
        public const string PRINT_TYPE_CODE__PhieuXuatSuDung_MPS000135 = "Mps000135";
        public const string PRINT_TYPE_CODE__PhieuTruyenDich_MPS000146 = "Mps000146";
        public const string PRINT_TYPE_CODE__PhieuHoaDonThanhToan_MPS000147 = "Mps000147";
        public const string PRINT_TYPE_CODE__PhieuBienLaiThanhToan_MPS000148 = "Mps000148";
        public const string PRINT_TYPE_CODE__PhieuNhapMauTuNhaCungCap_MPS000149 = "Mps000149";

        public const string PRINT_TYPE_CODE__PHIEU_NHAP_MAU_TU_NCC_MPS000149 = "Mps000149";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_TU_NCC__MPS000141 = "Mps000141";
        public const string PRINT_TYPE_CODE__BIEUMAU__BARCODE__MPS000142 = "Mps000142";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_THU_HOI__MPS000144 = "Mps000144";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO__MPS000143 = "Mps000143";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN__MPS000142 = "Mps000142";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO_THUOC_KHONG_PHAI_GAY_NGHIEN_HUONG_THAN__MPS000145 = "Mps000145";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_HAO_PHI__MPS000154 = "Mps000154";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_THANH_LY__MPS000155 = "Mps000155";
        public const string PRINT_TYPE_CODE__BIEUMAU__BIEN_BAN_XAC_NHAN_HONG__MPS000166 = "Mps000166";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_MAT_MAT__MPS000168 = "Mps000168";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_KHAC__MPS000165 = "Mps000165";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_CHUYEN_KHO_MAU__MPS000198 = "Mps000198";

        public const string PRINT_TYPE_CODE__PhieuDuTru__MPS000117 = "Mps000117";
        public const string PRINT_TYPE_CODE__ChiTietGoiThau__MPS000119 = "Mps000119";
        public const string PRINT_TYPE_CODE__BIEUMAU__TAO_BIEU_MAU_KHAC__MPS000133 = "Mps000133";

        public const string PRINT_TYPE_CODE__PHIEU_XUAT_TRA_NHA_CUNG_CAP__MPS000130 = "Mps000130";
        public const string PRINT_TYPE_CODE__PHIEU_TONG_HOP_TON_KHO_THUOC_VT_MAU__MPS000131 = "Mps000131";
        public const string PRINT_TYPE_CODE__BIEN_BAN_KIEM_KE_THUOC_VT_MAU__MPS000132 = "Mps000132";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_TO_DIEU_TRI__MPS000062 = "Mps000062";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__TRA_DOI__MPS000047 = "Mps000047";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__TONG_HOP__MPS000046 = "Mps000046";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__GAY_NGHIEN_HUONG_TAM_THAN__MPS000048 = "Mps000048";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__THUOC_VAT_TU__MPS000049 = "Mps000049";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA__TRA_DOI__MPS000093 = "Mps000093";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC__TONG_HOP__MPS000078 = "Mps000078";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOSC_GAY_NGHIEN_HUONG_TAM_THAN__MPS000101 = "Mps000101";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC_CHI_TIET__MPS000100 = "Mps000100";
        public const string PRINT_TYPE_CODE__BIEUMAU__IN_BARCODE_MEDI_RECORD_CODE__MPS000094 = "Mps000094";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_CONG_KHAI_THUOC__MPS000088 = "Mps000088";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_CONG_KHAI_THUOC_THEO_NGAY__MPS000116 = "Mps000116";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__HT_GN__MPS000169 = "Mps000169";
        public const string PRINT_TYPE_CODE__PhieuThuThanhToanKhac_MPS000299 = "Mps000299";
        public const string PRINT_TYPE_CODE__PhieuThuThanhToanKhac_MPS000355 = "Mps000355";
        public const string PRINT_TYPE_CODE__HOI_CHAN__SO_BIEN_BAN_HOI_CHAN_DAU_SAO__MPS000323 = "Mps000323";
        public const string PRINT_TYPE_CODE__PhieuHoanUngThanhToanRaVien_Mps000361 = "Mps000361";
        public const string PRINT_TYPE_CODE__PhieuGiuTheBHYT_Mps000173 = "Mps000173";
        public const string PRINT_TYPE_CODE__PhieuChamSocCapI_MPS000427 = "Mps000427";
        public const string PRINT_TYPE_CODE__PHIEU_YEU_CAU_IN_TO_DIEU_TRI_TRONG_BENH_AN_MPS000429 = "Mps000429";
        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC__MPS000100 = "Mps000100";
        public const string PRINT_TYPE_CODE__TOM_TAT_Y_LENH_PTTT_VA_DON_THUOC__MPS000478 = "Mps000478";
        public const string PRINT_TYPE_CODE__IN_TEM_VTTSD__MPS000494 = "Mps000494";
    }
}
