using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Set.SetValueComboCommune
{
    class SetValueComboCommune : ISetValueComboCommune
    {
        public void SetValue(System.Windows.Forms.UserControl UC, string CommuneCODE)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCComboCommune = (Design.Template1.Template1)UC;
                    UCComboCommune.SetValueCombo(CommuneCODE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
