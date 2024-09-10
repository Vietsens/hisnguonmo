using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboNational.Get.GetValueNationalCode
{
    class GetValueNationalCodeFactory
    {
        internal static IGetValueNationalCode MakeIGetValueNationalCode()
        {
            IGetValueNationalCode resutl = null;
            try
            {
                resutl = new GetValueNationalCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                resutl = null;
            }
            return resutl;
        }
    }
}
