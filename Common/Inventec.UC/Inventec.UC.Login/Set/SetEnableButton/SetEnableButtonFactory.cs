using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Set.SetEnableButton
{
    class SetEnableButtonFactory
    {
        internal static ISetEnableButton MakeISetEnableButton()
        {
            ISetEnableButton result = null;
            try
            {
                result = new SetEnableButton();
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
