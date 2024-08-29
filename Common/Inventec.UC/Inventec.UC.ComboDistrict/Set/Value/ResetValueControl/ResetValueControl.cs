using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Set.ResetValueControl
{
    class ResetValueControl : IResetValueControl
    {
        public void ResetValue(System.Windows.Forms.UserControl UC)
        {
            try
            {
                if (UC.GetType() == typeof(Desgin.Template1.Template1))
                {
                    Desgin.Template1.Template1 UCComboDistrict = (Desgin.Template1.Template1)UC;
                    UCComboDistrict.ResetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
