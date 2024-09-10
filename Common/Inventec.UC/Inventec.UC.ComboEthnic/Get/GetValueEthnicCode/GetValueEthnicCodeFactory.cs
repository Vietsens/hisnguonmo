using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboEthnic.Get.GetValueEthnicCode
{
    class GetValueEthnicCodeFactory
    {
        internal static IGetValueEthnicCode MakeIGetValueEthnicCode()
        {
            IGetValueEthnicCode result = null;
            try
            {
                result = new GetValueEthnicCode();
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
