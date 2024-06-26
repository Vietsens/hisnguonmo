using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMedicalContractCreate.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisMedicalContractCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.HisMedicalContractCreate.Run.FormHisMedicalContractCreate).Assembly);

        internal static string ThieuTruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThieuTruongDuLieuBatBuoc", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HanSuDungKhongDuocNhoHonNgayHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HanSuDungKhongDuocNhoHonNgayHienTai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("ThongBao", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongKhongDuocAm
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoLuongKhongDuocAm", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("CanhBaoThuoc", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("CanhBaoVatTu", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BiTrung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BiTrung", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongHopDongVuotQuaSoLuongThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoLuongHopDongVuotQuaSoLuongThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaNhapHopDongVuotQuaThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiaNhapHopDongVuotQuaThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VatNhapHopDongVuotQuaThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VatNhapHopDongVuotQuaThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DaTonTaiThuocVatTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DaTonTaiThuocVatTu", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HieuLucDenLonHonHieuLucTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HieuLucDenLonHonHieuLucTu", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NhapQuaMaxlength
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NhapQuaMaxlength", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaHopDongVuotQuaThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiaHopDongVuotQuaThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChiHienThiThuocVatTuKD
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiHienThiThuocVatTuKD", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChiHienThiThuocVatTuKinhDoanh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiHienThiThuocVatTuKinhDoanh", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChiHienThiThuocVatTuKhongKD
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiHienThiThuocVatTuKhongKD", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChiHienThiThuocVatTuKhongKinhDoanh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiHienThiThuocVatTuKhongKinhDoanh", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
