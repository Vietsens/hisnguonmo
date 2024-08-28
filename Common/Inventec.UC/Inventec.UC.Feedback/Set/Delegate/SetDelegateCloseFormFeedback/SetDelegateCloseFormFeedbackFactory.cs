using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Feedback.Set.Delegate.SetDelegateCloseFormFeedback
{
    class SetDelegateCloseFormFeedbackFactory
    {
        internal static ISetDelegateCloseFormFeedback MakeISetDelegateCloseFormFeedback()
        {
            ISetDelegateCloseFormFeedback result = null;
            try
            {
                result = new SetDelegateCloseFormFeedback();
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
