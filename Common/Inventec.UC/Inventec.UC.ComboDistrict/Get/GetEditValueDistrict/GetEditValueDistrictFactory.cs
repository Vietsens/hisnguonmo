using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Get.GetEditValueDistrict
{
    class GetEditValueDistrictFactory
    {
        internal static IGetEditValueDistrict MakeIGetEditValueDistrict()
        {
            IGetEditValueDistrict result = null;
            try
            {
                result = new GetEditValueDistrict();
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
