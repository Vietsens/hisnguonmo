using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Set.SetDelegateHasException
{
    class SetDelegateHasException : ISetDelegateHasException
    {
        public bool SetDelegateException(System.Windows.Forms.UserControl UC, HasExceptionApi HasExcep)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Design.Template2.Template2))
                {
                    Design.Template2.Template2 UCChange = (Design.Template2.Template2)UC;
                    result = UCChange.SetDelegateHasException(HasExcep);
                    if (result == false)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                    }
                }   
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
