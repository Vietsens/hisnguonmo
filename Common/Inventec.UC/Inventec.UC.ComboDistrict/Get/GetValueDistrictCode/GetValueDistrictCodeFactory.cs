using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Get.GetValueDistrictCode
{
    class GetValueDistrictCodeFactory
    {
        internal static IGetValueDistrictCode MakeIGetValueDistrictCode()
        {
            IGetValueDistrictCode result = null;
            try
            {
                result = new GetValueDistrictCode();
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
