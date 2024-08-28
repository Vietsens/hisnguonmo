using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Feedback.Init
{
    class Init : IInit
    {
        public System.Windows.Forms.UserControl InitUC(MainFeedback.EnumTemplate Template, Data.DataInitFeedback Data)
        {
            UserControl result = null;
            try
            {
                if (Template == MainFeedback.EnumTemplate.TEMPLATE1)
                {
                    result = new Design.Template1.Template1(Data);
                }
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
