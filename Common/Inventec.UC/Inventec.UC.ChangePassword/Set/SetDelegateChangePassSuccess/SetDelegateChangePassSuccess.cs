using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Set.SetDelegateChangePassSuccess
{
    class SetDelegateChangePassSuccess : ISetDelegateChangePassSuccess
    {
        public bool SetDelegateChangeSucess(System.Windows.Forms.UserControl UC, ChangePasswordSuccess ChangeSuccess)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Design.Template2.Template2))
                {
                    Design.Template2.Template2 UCChange = (Design.Template2.Template2)UC;
                    result = UCChange.SetDelegateChangeSuccess(ChangeSuccess);
                    if (result == null)
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
