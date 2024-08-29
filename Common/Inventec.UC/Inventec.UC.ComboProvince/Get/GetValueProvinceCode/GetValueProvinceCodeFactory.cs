using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Get.GetValueProvinceCode
{
    class GetValueProvinceCodeFactory
    {
        internal static IGetValueProvinceCode MakeIGetValueProvinceCode()
        {
            IGetValueProvinceCode result = null;
            try
            {
                result = new GetValueProvinceCode();
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
