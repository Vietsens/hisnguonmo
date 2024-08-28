using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Get.GetEditValueComboProvince
{
    class GetEditValueComboProvinceFactory
    {
        internal static IGetEditValueComboProvince MakeIGetEditValueComboProvince()
        {
            IGetEditValueComboProvince result = null;
            try
            {
                result = new GetEditValueComboProvince();
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
