using Inventec.Core;
using System;

namespace ACS.MANAGER.Core.AcsRoleBase.Get.Ev
{
    class AcsRoleBaseGetEvBehaviorFactory
    {
        internal static IAcsRoleBaseGetEv MakeIAcsRoleBaseGetEv(CommonParam param, object data)
        {
            IAcsRoleBaseGetEv result = null;
            try
            {
                if (data.GetType() == typeof(string))
                {
                    result = new AcsRoleBaseGetEvBehaviorByCode(param, data.ToString());
                }
                else if (data.GetType() == typeof(long))
                {
                    result = new AcsRoleBaseGetEvBehaviorById(param, long.Parse(data.ToString()));
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
