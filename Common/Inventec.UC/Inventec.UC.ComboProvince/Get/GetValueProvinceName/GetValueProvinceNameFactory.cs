using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Get.GetValueProvinceName
{
    class GetValueProvinceNameFactory
    {
        internal static IGetValueProvinceName MakeIGetValueProvinceName()
        {
            IGetValueProvinceName result = null;
            try
            {
                result = new GetValueProvinceName();
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
