using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ConnectionTest.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ConnectionTest.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
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
        internal static string YLenhChuaCoVanBanQues
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("YLenhChuaCoVanBanQues", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string YLenhChuaCoVanBan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("YLenhChuaCoVanBan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string YLenhDaTonTaiVanBanKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("YLenhDaTonTaiVanBanKy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
