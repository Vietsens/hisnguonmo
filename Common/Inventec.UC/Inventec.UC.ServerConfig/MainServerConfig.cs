using Inventec.UC.ServerConfig.Init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ServerConfig
{    
    public partial class MainServerConfig
    {       
        public enum EnumTemplate
        {
            TEMPLATE1
        }

        public UserControl Init(EnumTemplate Template, CloseFormConfigSystem CloseForm)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitUC(Template, CloseForm);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);  
                result = null;
            }
            return result;
        }

        public void SetLanguage(string lang)
        {
            try
            {
                Data.DataStore.language = (lang != null) ? lang : "vi";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
