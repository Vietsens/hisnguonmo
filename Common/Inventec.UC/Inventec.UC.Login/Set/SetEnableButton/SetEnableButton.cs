using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Set.SetEnableButton
{
    class SetEnableButton : ISetEnableButton
    {
        public void SetButtonEnable(System.Windows.Forms.UserControl UC, bool valid)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCLogin = (Design.Template1.Template1)UC;
                    UCLogin.SetEnableButton(valid);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
