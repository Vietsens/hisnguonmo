using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Set.SetLabelLanguage
{
    class SetLabelLanguage : ISetLabelLanguage
    {
        public void SetLanguageLabel(System.Windows.Forms.UserControl UC, string textLabel)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCComboProvince = (Design.Template1.Template1)UC;
                    UCComboProvince.SetTextLabel(textLabel);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
