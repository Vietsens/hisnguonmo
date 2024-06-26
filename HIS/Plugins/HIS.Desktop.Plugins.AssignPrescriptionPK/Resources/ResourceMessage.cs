using System;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string BanChuaChonKhoXuat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaChonKhoXuat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanDangChonKeDonChoNBenhNhanBanCoChacMuonThucHien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanDangChonKeDonChoNBenhNhanBanCoChacMuonThucHien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CacBenhNhanSaukeDonThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CacBenhNhanSaukeDonThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string SereServOtherpaySourceAlert__DVChuaDuocNhapNguonChiTra
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SereServOtherpaySourceAlert__DVChuaDuocNhapNguonChiTra", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BenhNhanDangNoTienKhogChoPhepChiDinhDV
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanDangNoTienKhogChoPhepChiDinhDV", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string NgayUongTemp3
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__NgayUongTemp3", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayUongTemp4
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__NgayUongTemp4", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayUongTemp5
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__NgayUongTemp5", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChanChongChiDinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChanChongChiDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string CanhBaoChongChiDinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoChongChiDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string SereServConditionAlert__DVChuaDuocNhapDieuKien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SereServConditionAlert__DVChuaDuocNhapDieuKien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string BenhNhanChuongTrinhChuaDuocTaoVoBenhAnBanCoMuonTaoVBDHayKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanChuongTrinhChuaDuocTaoVoBenhAnBanCoMuonTaoVBDHayKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string BenhNhanChuongTrinhChuaDuocTaoVoBenhAn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanChuongTrinhChuaDuocTaoVoBenhAn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HoSoDangNoTienKhongChoPhepKeDon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HoSoDangNoTienKhongChoPhepKeDon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string BanCoMuonNhapThongTinKetThucDieuTriKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonNhapThongTinKetThucDieuTriKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string CanhBaoDiUngThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoDiUngThuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string ThuocXChongChiDinhVoiBenhNhanY
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocXChongChiDinhVoiBenhNhanY", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string SoNgayHenKhamToiDaChoPhepLaXXX__BanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoNgayHenKhamToiDaChoPhepLaXXX__BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoDaKeTrongNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__CanhBaoDaKeTrongNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoNgayHenKhamToiDaChoPhepLaXXX
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoNgayHenKhamToiDaChoPhepLaXXX", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string PhongKhamCoSoLuotKhamVuotDinhMuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("UC_ExamTreatmentFinish_PhongKhamCoSoLuotKhamVuotDinhMuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhungGioVuotQuaSoLuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("UC_ExamTreatmentFinish_KhungGioVuotQuaSoLuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoNgayHenToiDa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("UC_ExamTreatmentFinish_CanhBaoNgayHenToiDa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TruongDuLieuVuotQuaKyTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TruongDuLieuVuotQuaKyTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string PhongKhamCoSoLuongKhamVuotSoLuotChoPhep
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhongKhamCoSoLuongKhamVuotSoLuotChoPhep", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string MabenhPhuDaDuocSuDungChoMaBenhChinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MabenhPhuDaDuocSuDungChoMaBenhChinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
                
        internal static string IcdKhongDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IcdKhongDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string NhapQuaKyTuBenhPhu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NhapQuaKyTuBenhPhu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string WarnKeDonCoSoLuongQuyDoiLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("WarnKeDonCoSoLuongQuyDoiLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string WarnThuocVatTuCoHanSuDungNhoHonThoiGianYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("WarnThuocVatTuCoHanSuDungNhoHonThoiGianYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string DanhSachThuocDaKeDaCoThuocTheoXKhongTheBoSungThemThuocYZ
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DanhSachThuocDaKeDaCoThuocTheoXKhongTheBoSungThemThuocYZ", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string Trong1LanKeDonChiDuocKeTheoLoHoacTheoLoai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Trong1LanKeDonChiDuocKeTheoLoHoacTheoLoai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string _KhongNhapSoLuongKe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("_KhongNhapSoLuongKe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string _KhongDuKhaDungTrongKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("_KhongDuKhaDungTrongKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string CacThuocCoCUngHoatChat_BanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CacThuocCoCUngHoatChat_BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CacThuocTrungDuocTinh_BanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CacThuocTrungDuocTinh_BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CacThuocCoCungMaATC_BanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CacThuocCoCungMaATC_BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CacThuocCoCungMaGroupATC_BanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CacThuocCoCungMaGroupATC_BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SuaDonThuocKhongChoPhepTachDonThuocTrongKhoVaNgoaiKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SuaDonThuocKhongChoPhepTachDonThuocTrongKhoVaNgoaiKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DanhSachThuocVatTuTonTai2KhoXuat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DanhSachThuocVatTuTonTai2KhoXuat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string CapNhatThatBai_KhongDuSoLuongKhaDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CapNhatThatBai_KhongDuSoLuongKhaDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string DonThuocCoCacThuocThuocNhomKhangSinhBanCOMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DonThuocCoCacThuocThuocNhomKhangSinhBanCOMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TrongNgayDaCoThuocKhangSinhBanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TrongNgayDaCoThuocKhangSinhBanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string TongTienDichVuHaoPhiVuotQuaMucGiaTriDuocCauHinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TongTienDichVuHaoPhiVuotQuaMucGiaTriDuocCauHinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string BenhNhanSapHetHanTheBHYT_SoNgayThuocToiDa_BanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanSapHetHanTheBHYT_SoNgayThuocToiDa_BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string DichVuTrongCauHinhThuocCHuaDuocCauHinhChanDoanICDBanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuTrongCauHinhThuocCHuaDuocCauHinhChanDoanICDBanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string DichVuTrongCauHinhThuocCHuaDuocCauHinhChanDoanICD
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuTrongCauHinhThuocCHuaDuocCauHinhChanDoanICD", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string ThuocCoGioiHanChiDinhThanhToanBHYTDeNghiBSXemXetTruocKhiKe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocCoGioiHanChiDinhThanhToanBHYTDeNghiBSXemXetTruocKhiKe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string VatTuXKeQuaSoLuongChoPhepBanCoMuonBoSung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VatTuXKeQuaSoLuongChoPhepBanCoMuonBoSung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string ThuocXKeQuaSoLuongChoPhepBanCoMuonBoSung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocXKeQuaSoLuongChoPhepBanCoMuonBoSung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThuocXKeQuaSoLuongChoPhepTrongNgayBanCoMuonBoSung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocXKeQuaSoLuongChoPhepTrongNgayBanCoMuonBoSung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string VatTuXKeQuaSoLuongChoPhepTrongNgayBanCoMuonBoSung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VatTuXKeQuaSoLuongChoPhepTrongNgayBanCoMuonBoSung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }


        internal static string ThuocXKeQuaSoLuongChoPhepCoChan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocXKeQuaSoLuongChoPhepCoChan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string SoLuongBiLePhanThapPhanToiDaX
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoLuongBiLePhanThapPhanToiDaX", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string BenhNhanNghiNgoDiUngVoiThuocXBanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanNghiNgoDiUngVoiThuocXBanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocVatTuChiSuDungChoGioiTinhX
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocVatTuChiSuDungChoGioiTinhX", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string BenhNhanDiUngVoiThuocXBanCOmuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanDiUngVoiThuocXBanCOmuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string KhongTimThayThuocVatTuDuocCauHinhChanDoanICD
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayThuocVatTuDuocCauHinhChanDoanICD", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string VuiLongKeThuocVatTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VuiLongKeThuocVatTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string KhongTimThayThongTinICD
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayThongTinICD", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string BenhNhanDiKamSomTruocXNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanDiKamSomTruocXNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string BenhNhanDangThieuVienPhi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanDangThieuVienPhi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BenhNhanDangThieuVienPhi_0D
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanDangThieuVienPhi_0D", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VatTuTSDKhongChoPhepSua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VatTuTSDKhongChoPhepSua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string ThuocChayThanKhongCapChoBNMangVeKeNgoaiKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocChayThanKhongCapChoBNMangVeKeNgoaiKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocCoDauCanHoiChan_BanCoMuonHoiChan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocCoDauCanHoiChan_BanCoMuonHoiChan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocCoDauCanHoiChan_ChuaTaoHetVoiHoatChat_BanCoMuonHoiChan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocCoDauCanHoiChan_ChuaTaoHetVoiHoatChat_BanCoMuonHoiChan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongChonPhongLamViec
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongChonPhongLamViec", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string ThuocChiDinhDungChoBenhNhanChuaQuaYTuoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocChiDinhDungChoBenhNhanChuaQuaYTuoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocChiDinhDungChoBenhNhanTuXTuoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocChiDinhDungChoBenhNhanTuXTuoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocChiDinhDungChoBenhNhanTuXDenYTuoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocChiDinhDungChoBenhNhanTuXDenYTuoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string PopupMenu_SuaSoNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__PopupMenu_SuaSoNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string PopupMenu_LoaiHaoPhi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__PopupMenu_LoaiHaoPhi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocVatTuDaBiLamTronSoLuongLenDoSoLuongCuBiKeLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocVatTuDaBiLamTronSoLuongLenDoSoLuongCuBiKeLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TongTienTheoDoiTuongDieuTriChoBHYTDaVuotquaMucGioiHan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__TongTienTheoDoiTuongDieuTriChoBHYTDaVuotquaMucGioiHan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoBenhNhanDaKeThuocTrongNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoBenhNhanDaKeThuocTrongNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoThuocChuaLuuTatForm
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThongBaoTatForm", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VatTuKhongDuKeBanCoMuonKeVatTuMuaNgoai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__VatTuKhongDuKeBanCoMuonKeVatTuMuaNgoai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoSaiThongTinTheBHYT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoSaiThongTinTheBHYT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TakeBeanForUpdateThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TakeBeanForUpdateThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoThuocVatTuNaoConDuSoluongKhaDungTrongKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoThuocVatTuNaoConDuSoluongKhaDungTrongKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocVatTuTachBeanThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocVatTuTachBeanThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocVatTu__SoLuong__Kho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocVatTu__SoLuong__Kho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThuocVatTuKhongDuocPhepKeLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocVatTuKhongDuocPhepKeLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DoiTuongBHYTBatBuocPhaiNhapHDSD
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__DoiTuongBHYTBatBuocPhaiNhapHDSD", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HDSDVuotQuaKyTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__HDSDVuotQuaKyTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayChinhSachGiaCuaDichVu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayChinhSachGiaCuaDichVu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TienBHYTDaKeVuotQuaMucGioiHan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__TienBHYTDaKeVuotQuaMucGioiHan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocDaduocKeVaoDanhSachThuocChobenNhanBanCoMuonChuyenSangBNKhac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThuocDaduocKeVaoDanhSachThuocChobenNhanBanCoMuonChuyenSangBNKhac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoNgayHenLaChuNhat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoNgayHenLaChuNhat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TruongThongTinCoDoDaiVuotQuaGioiHan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TruongThongTinCoDoDaiVuotQuaGioiHan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoNgayHenLaThuBay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoNgayHenLaThuBay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoThoiGianHenKhamSoVoiThoiGianKetThucDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoThoiGianHenKhamSoVoiThoiGianKetThucDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TaiKhoanKhongCoQuyenThucHienChucNang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TaiKhoanKhongCoQuyenThucHienChucNang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChucNangDangPhatTrienVuiLongThuLaiSau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChucNangDangPhatTrienVuiLongThuLaiSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaNhapThongTinTuVong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaNhapThongTinTuVong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaNhapThongTinChuyenVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaNhapThongTinChuyenVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaNhapThoiGianHenKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaNhapThoiGianHenKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucNhoHoiThoiGianYLenhCuaCacMaYeuCauSau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucNhoHoiThoiGianYLenhCuaCacMaYeuCauSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CacPhieuChiDinhSauChuaKetThucKhongChoPhepKetThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CacPhieuChiDinhSauChuaKetThucKhongChoPhepKetThuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaLinhHetThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaLinhHetThuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BenhNhanNoiTruChuaDuocChiDinhDichVuGiuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanNoiTruChuaDuocChiDinhDichVuGiuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TakeBeanThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TakeBeanThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoNgayGiuongNhoHonSoNgayDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoNgayGiuongNhoHonSoNgayDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoNgayGiuongLonHonSoNgayDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoNgayGiuongLonHonSoNgayDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianTuVongKhongDuocLonHonThoiGianHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianTuVongKhongDuocLonHonThoiGianHienTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianTuVongKhongDuocBeHonThoiGianVaoVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianTuVongKhongDuocBeHonThoiGianVaoVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TongTienHaoPhiVuotQuaDinhMucHaoPhiKhongTheChonDichVuNay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__TongTienHaoPhiVuotQuaDinhMucHaoPhiKhongTheChonDichVuNay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocDaChonTrongKhoKhongDuKe_BanComuonKeThuocNgoaiKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThuocDaChonTrongKhoKhongDuKe_BanComuonKeThuocNgoaiKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocKhongCoHoatChat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThuocKhongCoHoatChat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TruongSoLuongNhapSaiDinhDang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__TruongSoLuongNhapSaiDinhDang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BuoiSang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BuoiSang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BuoiTrua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BuoiTrua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BuoiChieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BuoiChieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BuoiToi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BuoiToi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string _NgayXVienBuoiYZ
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__NgayXVienBuoiYZ", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayIcdTuongUngVoiCacMaSau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__KhongTimThayIcdTuongUngVoiCacMaSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SuaDonThuocDuLieuTruyenVaoKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__SuaDonThuocDuLieuTruyenVaoKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaChonNgayChiDinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ChuaChonNgayChiDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string AcinInteractive__OverGrade
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__AcinInteractive__OverGrade", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HeThongTBCuaSoThongBaoBanCoMuonBoSung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HeThongTBCuaSoThongBaoBanCoMuonBoSung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocDaduocKe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThuocDaduocKe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VatTuduocKe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__VatTuDaduocKe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoVoiTocDoXTrongYGio
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoVoiTocDoXTrongYGio", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string NgayUong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__NgayUong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayUongTemp2
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__NgayUongTemp2", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Sang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Sang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Trua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Trua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Chieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Chieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Toi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Toi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string CanhBaoVatTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__VatTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Thuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoTienDaKeChoBHYTDaVuotquaMucGioiHan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__SoTienDaKeChoBHYTDaVuotquaMucGioiHan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VuotQuaSoLuongKhaDungTrongKho__CoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Message__VuotQuaSoLuongKhaDungTrongKho__CoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VuotQuaSoLuongKhaDungTrongKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Message__VuotQuaSoLuongKhaDungTrongKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string BatBuocPhaiNhapDuongDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BatBuocPhaiNhapDuongDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoThuocDaKeTrongNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__CanhBaoThuocDaKeTrongNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoVatTuDaKeTrongNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__CanhBaoVatTuDaKeTrongNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocKhongCoTrongKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThuocKhongCoTrongKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongNhapSoLuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__KhongNhapSoLuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoDoiTuongThanhToan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__KhongCoDoiTuongThanhToan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongXuatLonHonSpoLuongKhadungTrongKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__SoLuongXuatLonHonSpoLuongKhadungTrongKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BenhNhanChuaSuDungHetCacThuocSau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanChuaSuDungHetCacThuocSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BenhNhanChuaThanhToanTienCongKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanChuaThanhToanTienCongKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocVatTuKhongChiemKhaDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocVatTuKhongChiemKhaDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaThucHienXongDichVuCLS
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaThucHienXongDichVuCLS", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianYLenhKhongNamTrongThoiGianBNHienDienTaiKhoa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianYLenhKhongNamTrongThoiGianBNHienDienTaiKhoa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayKhongNamTrongNgaykeDon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgayKhongNamTrongNgaykeDon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayCoNhieuHon1ToDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgayCoNhieuHon1ToDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayChuaCoToDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgayChuaCoToDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonTaoYeuCauKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonTaoYeuCauKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoThuocKeVuotQuaSoNGaySuDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoThuocKeVuotQuaSoNGaySuDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string PopupMenu_PhaTruyenCungNhau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__PopupMenu_PhaTruyenCungNhau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string PopupMenu_HuyPhaTruyen
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__PopupMenu_HuyPhaTruyen", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongThuocNhomThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__KhongThuocNhomThuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayChiDinhTuLonHonNgayChiDinhDen
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__NgayChiDinhTuLonHonNgayChiDinhDen", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonBoSungKhongNeuCoVuiLongNhapLyDo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BanCoMuonBoSungKhongNeuCoVuiLongNhapLyDo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocTuongtacVoiThuocNhungVanKeDo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThuocTuongtacVoiThuocNhungVanKeDo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayDuTruKhongDuocNhoHonThoiGianChiDinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__NgayDuTruKhongDuocNhoHonThoiGianChiDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaNhapLyDoXuat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BanChuaNhapLyDoXuat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuongTrinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ChuongTrinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocVatTuChuaNhapLyDoXuat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThuocVatTuChuaNhapLyDoXuat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BoSungThongTinChanDoanPhuHoacDoiDoiTuongThanhToan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BoSungThongTinChanDoanPhuHoacDoiDoiTuongThanhToan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThuocVatTuChuaNhapLyDoKeQuaSoLuongToiDaTrongDon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocVatTuChuaNhapLyDoKeQuaSoLuongToiDaTrongDon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThuocVatTuChuaNhapLyDoKeQuaSoLuongToiDaTrongNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocVatTuChuaNhapLyDoKeQuaSoLuongToiDaTrongNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DsThuocKeQuaSoLuongChoPhepCoChan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DsThuocKeQuaSoLuongChoPhepCoChan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BanCoMuonBoSungKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonBoSungKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TruongDuLieuVuotQuaKyTuChoPhep
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TruongDuLieuVuotQuaKyTuChoPhep", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TrongTruongHopCoVuiLongNhapLyDo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TrongTruongHopCoVuiLongNhapLyDo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

    }
}
