using HIS.Desktop.Plugins.KskServiceEditList.KskServiceEditList;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using Inventec.Desktop.Common;
using System;

namespace HIS.Desktop.Plugins.KskServiceEditList
{
    /// <summary>
    /// 58013 - Sửa dịch vụ cho nhiều bệnh nhân khám sức khỏe hợp đồng (mở từ Hồ sơ điều trị).
    /// </summary>
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.KskServiceEditList",
       "Sửa dịch vụ khám sức khỏe hợp đồng",
       "Bussiness",
       4,
       "quy-tai-chinh.png",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true)]
    public class KskServiceEditListProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;

        public KskServiceEditListProcessor()
        {
            param = new CommonParam();
        }

        public KskServiceEditListProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IKskServiceEditList behavior = KskServiceEditListFactory.MakeIControl(param, args);
                result = behavior != null ? (object)(behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public override bool IsEnable()
        {
            return true;
        }
    }
}
