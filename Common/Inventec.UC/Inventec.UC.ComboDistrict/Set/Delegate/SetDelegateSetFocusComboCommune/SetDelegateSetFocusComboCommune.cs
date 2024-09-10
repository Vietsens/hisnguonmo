using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Set.SetDelegateSetFocusComboCommune
{
    class SetDelegateSetFocusComboCommune : ISetDelegateSetFocusComboCommune
    {
        public bool SetDelegateFocusCommune(System.Windows.Forms.UserControl UC, SetFocusComboCommune Focus)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Desgin.Template1.Template1))
                {
                    Desgin.Template1.Template1 UCComboDistrict = (Desgin.Template1.Template1)UC;
                    result = UCComboDistrict.SetDelegateFocusCommune(Focus);
                    if (result == false)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UCComboDistrict), UCComboDistrict));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Focus), Focus));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
