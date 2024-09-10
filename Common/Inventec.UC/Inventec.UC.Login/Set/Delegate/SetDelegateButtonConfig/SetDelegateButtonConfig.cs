using Inventec.UC.Login.Design.Template1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Set.SetDelegateButtonConfig
{
    class SetDelefateButtonConfig : ISetDelefateButtonConfig
    {
        public bool SetDelegateConfig(System.Windows.Forms.UserControl UC, EventButtonConfig btnConfig)
        {
            bool valid = false;
            try
            {
                if (UC.GetType() == typeof(Template1))
                {
                    Template1 UCLogin = (Template1)UC;
                    valid = UCLogin.SetDelegateButtonConfig(btnConfig);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}
