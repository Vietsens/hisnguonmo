using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Set.SetLoadComboDistrict
{
    class SetLoadComboDistrict : ISetLoadComboDistrict
    {
        public void SetLoadDistrict(System.Windows.Forms.UserControl UC, string ProvinceCODE)
        {
            try
            {
                if (UC.GetType() == typeof(Desgin.Template1.Template1))
                {
                    Desgin.Template1.Template1 UCComboDistrict = (Desgin.Template1.Template1)UC;
                    UCComboDistrict.SetLoadComboDistrict(ProvinceCODE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
