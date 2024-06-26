using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.LisSampleUpdate.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.LisSampleUpdate.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string DichVuChuaChonMayTraKetQuaBanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuChuaChonMayTraKetQuaBanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgaySinhKhongDuocNhoHon7
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgaySinhKhongDuocNhoHon7", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string YeuCauNhapDayDuNgayThangNamSinhVoiBNDuoi6Tuoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("YeuCauNhapDayDuNgayThangNamSinhVoiBNDuoi6Tuoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuChuaChonMayTraKetQua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuChuaChonMayTraKetQua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaChonChiSoLuuKetQua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaChonChiSoLuuKetQua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChiSoChuaNhapGiaTriBanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiSoChuaNhapGiaTriBanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChiSoChuaNhapGiaTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiSoChuaNhapGiaTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaChonChiSoIn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaChonChiSoIn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string NhapNgaySinhKhongDungDinhDang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NhapNgaySinhKhongDungDinhDang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string ThongTinNgaySinhPhaiNhoHonNgayHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThongTinNgaySinhPhaiNhoHonNgayHienTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }
    }
}
