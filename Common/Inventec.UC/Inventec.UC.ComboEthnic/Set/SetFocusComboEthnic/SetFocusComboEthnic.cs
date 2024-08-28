using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboEthnic.Set.SetFocusComboEthnic
{
    class SetFocusComboEthnic : ISetFocusComboEthnic
    {
        public void SetFocus(System.Windows.Forms.UserControl UC)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCComboEthnic = (Design.Template1.Template1)UC;
                    UCComboEthnic.SetFocus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
