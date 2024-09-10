using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboEthnic.Get.GetValueEthnicName
{
    class GetValueEthnicNameFactory
    {
        internal static IGetValueEthnicName MakeIGetValueEthnicName()
        {
            IGetValueEthnicName result = null;
            try
            {
                result = new GetValueEthnicName();
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
