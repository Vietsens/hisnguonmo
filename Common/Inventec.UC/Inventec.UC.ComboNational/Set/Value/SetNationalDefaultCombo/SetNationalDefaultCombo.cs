using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboNational.Set.Value.SetNationalDefaultCombo
{
    class SetNationalDefaultCombo : ISetNationalDefaultCombo
    {
        public void SetDefaultNational(System.Windows.Forms.UserControl UC, SDA.EFMODEL.DataModels.SDA_NATIONAL National)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCComboNational = (Design.Template1.Template1)UC;
                    UCComboNational.SetDefaultNational(National);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
