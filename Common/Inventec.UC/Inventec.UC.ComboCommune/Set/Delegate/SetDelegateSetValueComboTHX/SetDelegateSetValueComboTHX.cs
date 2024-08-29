using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Set.SetDelegateSetValueComboTHX
{
    class SetDelegateSetValueComboTHX : ISetDelegateSetValueComboTHX
    {
        public bool SetDelegateSetValueTHX(System.Windows.Forms.UserControl UC, SetValueComboTHX Value)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCComboCommune = (Design.Template1.Template1)UC;
                    result = UCComboCommune.SetDelegateSetValueTHX(Value);
                    if (result == null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UCComboCommune), UCComboCommune));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Value), Value));
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
