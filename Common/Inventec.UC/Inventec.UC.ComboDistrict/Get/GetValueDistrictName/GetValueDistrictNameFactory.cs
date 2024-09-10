using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Get.GetValueDistrictName
{
    class GetValueDistrictNameFactory
    {
        internal static IGetValueDistrictName MakeIGetValueDistrictName()
        {
            IGetValueDistrictName result = null;
            try
            {
                result = new GetValueDistrictName();
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
