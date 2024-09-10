using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Feedback.Sda.SdaFeedback.Create
{
    internal class SdaFeedbackCreateFactory : Process.GetBase
    {
        internal SdaFeedbackCreateFactory() : base() { }
         internal SdaFeedbackCreateFactory(CommonParam paramFactory) : base(paramFactory) { }
        public enum FeedbacktType
        {
            ExamReport,
            DEFAULT,
        }
        public SdaFeedbackCreate CreateFactory(FeedbacktType feedbackType, SDA.EFMODEL.DataModels.SDA_FEEDBACK dataForm)
        {
            SdaFeedbackCreate createConcrete = new SdaFeedbackCreate(param);
            switch (feedbackType)
            {
                case FeedbacktType.DEFAULT:
                    createConcrete.Behavior = new SdaFeedbackCreateBehaviorDefault(param, dataForm);
                    break;
                default:
                    createConcrete.Behavior = new SdaFeedbackCreateBehaviorDefault(param, dataForm);
                    break;
            }
            return createConcrete;
        }
    }
}
