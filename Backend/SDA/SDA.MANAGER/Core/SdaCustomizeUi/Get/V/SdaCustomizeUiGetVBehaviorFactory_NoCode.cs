using Inventec.Core;
using System;

namespace SDA.MANAGER.Core.SdaCustomizeUi.Get.V
{
    class SdaCustomizeUiGetVBehaviorFactory
    {
        internal static ISdaCustomizeUiGetV MakeISdaCustomizeUiGetV(CommonParam param, object data)
        {
            ISdaCustomizeUiGetV result = null;
            try
            {
                if (data.GetType() == typeof(long))
                {
                    result = new SdaCustomizeUiGetVBehaviorById(param, long.Parse(data.ToString()));
                }
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                MANAGER.Base.BugUtil.SetBugCode(param, LibraryBug.Bug.Enum.Common__FactoryKhoiTaoDoiTuongThatBai);
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
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
