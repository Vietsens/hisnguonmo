using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Set.SetDelegateValueCboDistrictAndCommune
{
    class SetDelegateValueCboDistrictAndCommune : ISetDelegateValueCboDistrictAndCommune
    {
        public bool SetDelegateDistrictAndCommune(System.Windows.Forms.UserControl UC, SetValueCboDistrictAndCboCommune SetValue)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCComboProvince = (Design.Template1.Template1)UC;
                    result = UCComboProvince.SetDelegateSetValue(SetValue);
                    if (result == false)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UCComboProvince), UCComboProvince));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SetValue), SetValue));
                    }
                }
            }
            catch (Exception ex)
            {

                result = false;
            }
            return result;
        }
    }
}
