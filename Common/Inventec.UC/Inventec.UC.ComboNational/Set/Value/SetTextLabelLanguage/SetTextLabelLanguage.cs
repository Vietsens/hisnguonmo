using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboNational.Set.Value.SetTextLabelLanguage
{
    class SetTextLabelLanguage : ISetTextLabelLanguage
    {
        public void SetTextLabel(System.Windows.Forms.UserControl UC, string textLabel)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCComboNational = (Design.Template1.Template1)UC;
                    UCComboNational.SetTextLabel(textLabel);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
