using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Set.SetValueComboDistrict
{
    class SetValueComboDistrict : ISetValueComboDistrict
    {
        public void SetValue(System.Windows.Forms.UserControl UC, string DistrictCODE)
        {
            try
            {
                if (UC.GetType() == typeof(Desgin.Template1.Template1))
                {
                    Desgin.Template1.Template1 UCComboDistrict = (Desgin.Template1.Template1)UC;
                    UCComboDistrict.SetValueDistrict(DistrictCODE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
