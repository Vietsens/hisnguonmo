using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Set.SetDelegateGetValueComboDistrict
{
    class SetDelegateGetValueComboDistrict : ISetDelegateGetValueComboDistrict
    {
        public bool SetDelegateValueDistrict(System.Windows.Forms.UserControl UC, GetValueComboDistrict GetValue)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCComboCommune = (Design.Template1.Template1)UC;
                    result = UCComboCommune.SetDelegateGetValueDistrict(GetValue);
                    if (result == false)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UCComboCommune), UCComboCommune));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GetValue), GetValue));
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
