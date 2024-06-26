using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisCareTemp.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormCareTemp = new ResourceManager("HIS.Desktop.Plugins.HisCareTemp.Resources.Lang", typeof(HIS.Desktop.Plugins.HisCareTemp.FormCareTemp).Assembly);

        internal static string ChamSoc_DuLieuChiTietKhongTheCungLoaiChamSoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChamSoc_DuLieuChiTietKhongTheCungLoaiChamSoc", LanguageFormCareTemp, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DoDaiVuotQua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DoDaiVuotQua", LanguageFormCareTemp, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThieuTruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThieuTruongDuLieuBatBuoc", LanguageFormCareTemp, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong", LanguageFormCareTemp, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("ThongBao", LanguageFormCareTemp, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanKhongCoQuyenSuaNoiDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanKhongCoQuyenSuaNoiDung", LanguageFormCareTemp, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
