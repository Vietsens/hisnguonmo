using HIS.Desktop.Plugins.HisDepaPatientTypeList.HisDepaPatientTypeList;
using Inventec.Core;
using Inventec.Desktop.Core;
using System;

namespace HIS.Desktop.Plugins.HisDepaPatientTypeList
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.HisDepaPatientTypeList",
        "Thiết lập khoa - đối tượng thanh toán",
        "Common",
        99,
        "khoa-dttt.png",
        "A",
        Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM,
        true,
        true)]
    public class HisDepaPatientTypeListProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;

        public HisDepaPatientTypeListProcessor()
        {
            param = new CommonParam();
        }

        public HisDepaPatientTypeListProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        object IDesktopRoot.Run(object[] args)
        {
            object result = null;
            try
            {
                IHisDepaPatientTypeList behavior = HisDepaPatientTypeListFactory.MakeIHisDepaPatientTypeList(param, args);
                result = behavior != null ? behavior.Run() : null;
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
            return false;
        }
    }
}
