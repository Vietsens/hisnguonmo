using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Set.ResetValueControl
{
    class ResetValueControl : IResetValueControl
    {
        public void ResetValue(System.Windows.Forms.UserControl UC)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCComboProvince = (Design.Template1.Template1)UC;
                    UCComboProvince.ResetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
