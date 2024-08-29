using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Get.GetEditValueComboProvince
{
    class GetEditValueComboProvince : IGetEditValueComboProvince
    {
        public object GetEditValue(System.Windows.Forms.UserControl UC)
        {
            object result = null;
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCComboProvince = (Design.Template1.Template1)UC;
                    result = UCComboProvince.GetEditValueCombo();
                    if (result == null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UCComboProvince), UCComboProvince));
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
