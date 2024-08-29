using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Feedback.Sda.SdaFeedback.Create
{
    class SdaFeedbackCreate : Process.BusinessBase
    {
        internal SdaFeedbackCreate()
            : base()
        {

        }

        internal SdaFeedbackCreate(CommonParam paramCreate)
            : base(paramCreate)
        {

        }

        internal ISdaFeedbackCreateBehavior Behavior { get; set; }

        internal SDA.EFMODEL.DataModels.SDA_FEEDBACK Create()
        {
            SDA.EFMODEL.DataModels.SDA_FEEDBACK result = null;
            try
            {
                result = Behavior.Create();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                result = null;
            }
            return result;
        }
    }
}
