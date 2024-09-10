using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Feedback.Sda.SdaFeedback.Create
{
    interface ISdaFeedbackCreateBehavior
    {
        SDA.EFMODEL.DataModels.SDA_FEEDBACK Create();
    }
}
