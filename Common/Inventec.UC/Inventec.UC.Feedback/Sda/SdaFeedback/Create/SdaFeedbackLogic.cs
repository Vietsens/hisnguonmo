using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Feedback.Sda.SdaFeedback.Create
{
    internal class SdaFeedbackLogic : Inventec.UC.Feedback.Process.BusinessBase
    {
        public SdaFeedbackLogic()
            : base()
        {

        }

        public SdaFeedbackLogic(CommonParam paramGet)
            : base(paramGet)
        {

        }
        public SDA.EFMODEL.DataModels.SDA_FEEDBACK Create(SDA.EFMODEL.DataModels.SDA_FEEDBACK data)
        {
            bool valid = true;
            valid = valid && IsNotNull(param);
            valid = valid && IsNotNull(data);
            SDA.EFMODEL.DataModels.SDA_FEEDBACK result = null;
            #region Logging Input Data
            try
            {
                Input = Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param);
            }
            catch { }
            #endregion

            try
            {
                TokenCheck();
                result = new SdaFeedbackCreateFactory(param).CreateFactory(SdaFeedbackCreateFactory.FeedbacktType.DEFAULT, data).Create();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            TroubleCheck(result); return result;
        }
    }
}
