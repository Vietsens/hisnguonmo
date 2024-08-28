using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Get.GetEditValueDistrict
{
    class GetEditValueDistrict : IGetEditValueDistrict
    {
        public object GetEditValue(System.Windows.Forms.UserControl UC)
        {
            object result = null;
            try
            {
                if (UC.GetType() == typeof(Desgin.Template1.Template1))
                {
                    Desgin.Template1.Template1 UCComboDistrict = (Desgin.Template1.Template1)UC;
                    result = UCComboDistrict.GetEditValueCombo();
                    if (result == null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UCComboDistrict), UCComboDistrict));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
