using Inventec.UC.Feedback.Init;
using Inventec.UC.Feedback.Set.Delegate.SetDelegateCloseFormFeedback;
using Inventec.UC.Feedback.Set.Delegate.SetDelegateHasExceptionApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Feedback
{
    public partial class MainFeedback
    {
        public enum EnumTemplate
        {
            TEMPLATE1
        }

        #region Init UC FeedBack
        public UserControl Init(EnumTemplate Template, Data.DataInitFeedback Data)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitUC(Template, Data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        #endregion

        #region Set Delegate


        public bool SetDelegateHasException(UserControl UC, HasExceptionApi HasException)
        {
            bool result = false;
            try
            {
                result = SetDelegateHasExceptionApiFactory.MakeISetDelegateHasExceptionApi().SetDelegateHasException(UC, HasException);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateCloseFormFeedback(UserControl UC, CloseForm Close)
        {
            bool result = false;
            try
            {
                result = SetDelegateCloseFormFeedbackFactory.MakeISetDelegateCloseFormFeedback().SetDelegateClose(UC, Close);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        #endregion
    }
}
