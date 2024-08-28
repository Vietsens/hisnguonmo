using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Get.GetValueCommuneCode
{
    class GetValueCommuneCodeFactory
    {
        internal static IGetValueCommuneCode MakeIGetValueCommuneCode()
        {
            IGetValueCommuneCode result = null;
            try
            {
                result = new GetValueCommuneCode();
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
