using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Get.GetValueCommuneName
{
    class GetValueCommuneNameFactory
    {
        internal static IGetValueCommuneName MakeIGetValueCommuneName()
        {
            IGetValueCommuneName result = null;
            try
            {
                result = new GetValueCommuneName();
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
