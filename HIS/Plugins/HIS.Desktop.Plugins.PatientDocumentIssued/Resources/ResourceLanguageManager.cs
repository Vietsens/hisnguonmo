using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;

namespace HIS.Desktop.Plugins.PatientDocumentIssued.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageResources { get; set; }

       
            static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.PatientDocumentIssued.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());



        internal static string ThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_PatientDocumentIssued__ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string VuiLongChonItNhatMotVanBan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_PatientDocumentIssued__VuiLongChonItNhatMotVanBan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }


        internal static string TaiVeThanhCong
            {
                get
                {
                    try
                    {
                        return Inventec.Common.Resource.Get.Value("Plugins_PatientDocumentIssued__TaiVeThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                    return "";
                }
            }

            internal static string ThuMucDaTonTai
            {
                get
                {
                    try
                    {
                        return Inventec.Common.Resource.Get.Value("Plugins_PatientDocumentIssued__ThuMucDaTonTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                    return "";
                }
            }

            internal static string KhongLayDuocFile
            {
                get
                {
                    try
                    {
                        return Inventec.Common.Resource.Get.Value("Plugins_PatientDocumentIssued__KhongLayDuocFile", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                    return "";
                }
            }

            internal static string ThongBaoCoMuonXoaCacDuLieuDaChon
            {
                get
                {
                    try
                    {
                        return Inventec.Common.Resource.Get.Value("Plugins_PatientDocumentIssued__ThongBaoCoMuonXoaCacDuLieuDaChon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
