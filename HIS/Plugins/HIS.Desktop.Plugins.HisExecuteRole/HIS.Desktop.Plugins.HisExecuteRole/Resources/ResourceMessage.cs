using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisExecuteRole
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisExecuteRole.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BanCoMuonKhoaDuLieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisExecuteRole__Look", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonBoKhoaDuLieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisExecuteRole__Unlook", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        //internal static string 
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_InfantInformation__SoTuanPhaiNhoHon40", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
            }
        }
    

