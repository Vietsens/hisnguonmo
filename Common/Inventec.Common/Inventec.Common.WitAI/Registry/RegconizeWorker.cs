using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WitAI
{
    public class RegconizeWorker
    {
        private static RegistryKey appFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.COMPANY_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);

        public static void ChangeRegconizeVoice(bool reg)
        {
            try
            {
                appFolder.SetValue(RegistryConstant.NAME_KEY, reg);
            }
            catch (Exception ex)
            {
                //Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static bool GetRegconizeVoice()
        {
            bool reg = false;
            try
            {
                var f = appFolder.GetValue(RegistryConstant.NAME_KEY, false);
                reg = (bool)(f);
            }
            catch (Exception ex)
            {
                reg = false;
                //Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return reg;
        }
    }
}
