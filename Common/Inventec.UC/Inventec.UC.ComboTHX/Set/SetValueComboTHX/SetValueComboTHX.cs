using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboTHX.Set.SetValueComboTHX
{
    class SetValueComboTHX : ISetValueComboTHX
    {
        public void SetValue(System.Windows.Forms.UserControl UC, SDA.EFMODEL.DataModels.V_SDA_COMMUNE Commune)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCComboTHX = (Design.Template1.Template1)UC;
                    UCComboTHX.SetValueComboTHX(Commune);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
