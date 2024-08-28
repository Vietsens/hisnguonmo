using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboNational.Get.GetValueNationalName
{
    class GetValueNationalNameFactory
    {
        internal static IGetValueNationalName MakeIGetValueNationalName()
        {
            IGetValueNationalName result = null;
            try
            {
                result = new GetValueNationalName();
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
