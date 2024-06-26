using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisProgram.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisProgram.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string DuLieuPhongDangChonThuocNhieuChiNhanh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ChooseRoom__DuLieuPhongDangChonThuocNhieuChiNhanh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string InvoiceBook_GiaTriLonHonKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("InvoiceBook_GiaTriLonHonKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TatCa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentType_TatCa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }
        internal static string Kham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentType_Kham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }
        internal static string DieuTriNgoaiTru
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentType_DieuTriNgoaiTru", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }
        internal static string DieuTriNoiTru
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentType_DieuTriNoiTru", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }
        internal static string DieuTriBanNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentType_DieuTriBanNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
