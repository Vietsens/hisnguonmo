using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisKskDriverCreate.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisKskDriverCreate.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string KhongTimThayBenhNhanTheoMa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayBenhNhanTheoMa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayBenhNhanTheoMaDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayBenhNhanTheoMaDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayBenhNhanTheoMaYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayBenhNhanTheoMaYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DuLieuKhongDungDinhDang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DuLieuKhongDungDinhDang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongLayDuocThongTinChungThu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongLayDuocThongTinChungThu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
        internal static string ChuaChonThongTinDonViTinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaChonThongTinDonViTinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuaNhapThongTinNongDo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaNhapThongTinNongDo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongLayDuocThongTinChungThuBanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongLayDuocThongTinChungThuBanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DuDieuKienSucKhoeLaiXe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DuDieuKienSucKhoeLaiXe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongDuDieuKienSucKhoeLaiXe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDuDieuKienSucKhoeLaiXe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DatTieuChuanSucKhoeLaiXeNhungYcKhamLai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DatTieuChuanSucKhoeLaiXeNhungYcKhamLai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DaTonTaiHoSoSucKhoeHang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DaTonTaiHoSoSucKhoeHang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
        internal static string mg100mlMau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("mg100mlMau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string mg1LitKhiTho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("mg1LitKhiTho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
