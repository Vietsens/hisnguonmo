using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InterconnectionPrescription.Resources
{
    class ResourceLanguageManager
    {
        internal static System.Resources.ResourceManager LanguageResource = new ResourceManager("HIS.Desktop.Plugins.InterconnectionPrescription.Resources.Lang", typeof(HIS.Desktop.Plugins.InterconnectionPrescription.InterconnectionPrescription.frmInterconnectionPrescription).Assembly);

        internal static string MaLienThongHoacMatKhauKhongDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.InterconnectionPrescription.MaLienThongHoacMatKhauKhongDung", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string ChuaKhaiBaoThongTinMaCoSo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.InterconnectionPrescription.ChuaKhaiBaoThongTinMaCoSo", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string All
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.InterconnectionPrescription.All", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Export
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.InterconnectionPrescription.Export", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NotExport
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.InterconnectionPrescription.NotExport", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NoAddress
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.InterconnectionPrescription.NoAddress", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ErrorErxConfig
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.InterconnectionPrescription.ErrorErxConfig", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NoPrescription
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.InterconnectionPrescription.NoPrescription", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NoPrescription_2
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.InterconnectionPrescription.NoPrescription_2", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CacYLenhDaDuocDongBoBanCoMuonTiepTucKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.InterconnectionPrescription.CacYLenhDaDuocDongBoBanCoMuonTiepTucKhong", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
