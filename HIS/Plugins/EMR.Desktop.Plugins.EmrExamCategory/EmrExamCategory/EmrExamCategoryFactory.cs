using Inventec.Core;
using System;

namespace EMR.Desktop.Plugins.EmrExamCategory.EmrExamCategory
{
    class EmrExamCategoryFactory
    {
        internal static IEmrExamCategory MakeIControl(CommonParam param, object[] data)
        {
            IEmrExamCategory result = null;
            try
            {
                result = new EmrExamCategoryBehavior(param, data);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error(
                    "Factory failed to create IEmrExamCategory."
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data),
                    ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
