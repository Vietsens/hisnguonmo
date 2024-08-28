using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Get.GetEditValueComboCommune
{
    class GetEditValueComboCommuneFactory
    {
        internal static IGetEditValueComboCommune MakeIGetEditValueComboCommune()
        {
            IGetEditValueComboCommune result = null;
            try
            {
                result = new GetEditValueComboCommune();
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
