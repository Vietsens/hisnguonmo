using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RehaServiceReqExecute.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ExamServiceReqExecute.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        
        internal static string KhongTimThayIcdTuongUngVoiCacMaSau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__KhongTimThayIcdTuongUngVoiCacMaSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__TaiKhoanKhongCoQuyenThucHienChucNang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DaCoDonThuocBanCoMuonTiepTucKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__DaCoDonThuocBanCoMuonTiepTucKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DonThuocLanKhamTruoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__DonThuocLanKhamTruoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
