using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisHivTreatment.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisHivTreatment.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string KhongCoThongTin
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoThongTin", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguoiNhiemHIV
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiNhiemHIV", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TrePhoiNhiemVoiHiv
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TrePhoiNhiemVoiHiv", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DieuTriDuPhongTruocPhoiNhiem
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DieuTriDuPhongTruocPhoiNhiem", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DieuTriDuPhongSauPhoiNhiem
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DieuTriDuPhongSauPhoiNhiem", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Khac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Khac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TreDuoi18ThangSinhRaTuMeNhiemHiv
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TreDuoi18ThangSinhRaTuMeNhiemHiv", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string PhoiNhiem
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhoiNhiem", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DangDieuTriLao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DangDieuTriLao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CoBau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CoBau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuyenDa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuyenDa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SauSinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SauSinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ViemGan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ViemGan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NghienChichMaTuy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NghienChichMaTuy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BenhNhanHivMoiDangKyLanDau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanHivMoiDangKyLanDau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BenhNhanHivChuaDieuTriArvChuyenToi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanHivChuaDieuTriArvChuyenToi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BenhNhanHivDaDieuTriArvChuyenToi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanHivDaDieuTriArvChuyenToi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BenhNhanHivDaDieuTriArvNayDieuTriLai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanHivDaDieuTriArvNayDieuTriLai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BenhNhanHivChuaDieuTriArvDangKyLai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanHivChuaDieuTriArvDangKyLai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string PhacDoBac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhacDoBac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string PhacDo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhacDo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string PhacDoKhacHIV
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhacDoKhacHIV", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongDieuTriLao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDieuTriLao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DieuTriLaoTiemAn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DieuTriLaoTiemAn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DieuTriLao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DieuTriLao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DieuTriLaoKhangThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DieuTriLaoKhangThuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThuongQuy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuongQuy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NghiNgoThatBaiDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NghiNgoThatBaiDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Lan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Lan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string AmTinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("AmTinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DuongTinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DuongTinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DieuTriArv
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DieuTriArv", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DuPhongLao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DuPhongLao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DieuTriViemGanB
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DieuTriViemGanB", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Lan3
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Lan3", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string PhacDoKhacLao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhacDoKhacLao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongPhatHien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongPhatHien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Duoi50BanSao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Duoi50BanSao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Tu50DenDuoi200BanSao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Tu50DenDuoi200BanSao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Tu200DenDuoi1000BanSao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Tu200DenDuoi1000BanSao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Tren1000BanSao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Tren1000BanSao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SoNgayCapThuocKhongDuocLonHon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoNgayCapThuocKhongDuocLonHon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongDuocLonHon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDuocLonHon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaiDoan1
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiaiDoan1", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string GiaiDoan2
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiaiDoan2", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaiDoan3
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiaiDoan3", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaiDoan4
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiaiDoan4", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string HoanThanh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HoanThanh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongSangLoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongSangLoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NgungDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgungDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NguoiBanDam
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiBanDam", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NguoiChuyenDoiGioiTinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiChuyenDoiGioiTinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NguoiCoQuanHeTinhDucDongGioi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiCoQuanHeTinhDucDongGioi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguoiCoQuanHeTinhDucVoiNguoiNhiemHIV
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiCoQuanHeTinhDucVoiNguoiNhiemHIV", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguoiCoTrieuChungLamSangNghiNgoNhiemHIV
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiCoTrieuChungLamSangNghiNgoNhiemHIV", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguoiDiBienDong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiDiBienDong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguoiMacBenhLao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiMacBenhLao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguoiMacCacBenhLayTruyenQuaDuongTinhDuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiMacCacBenhLayTruyenQuaDuongTinhDuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguoiSuDungMaTuy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiSuDungMaTuy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string PhacDoMoiTheoHuongDanQuocGia
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhacDoMoiTheoHuongDanQuocGia", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string PhamNhanNguoiBiTamGiam
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhamNhanNguoiBiTamGiam", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SangLocTrieuChung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SangLocTrieuChung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TacDungPhu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TacDungPhu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThatBaiDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThatBaiDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThieuThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThieuThuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ToiUuHoaPhacDo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ToiUuHoaPhacDo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TuVong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TuVong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string XetNghiemProteinPhanUngC
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("XetNghiemProteinPhanUngC", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DangDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DangDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CungSongChungVoiNguoiNhiemHIV
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CungSongChungVoiNguoiNhiemHIV", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CacDoiTuongKhac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CacDoiTuongKhac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChupXQuangPhoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChupXQuangPhoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BoDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BoDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DieuTriViemGanC
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DieuTriViemGanC", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongDanhGia
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDanhGia", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
