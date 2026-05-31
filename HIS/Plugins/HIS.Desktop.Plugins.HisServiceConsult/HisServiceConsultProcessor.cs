/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;

namespace HIS.Desktop.Plugins.HisServiceConsult
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.HisServiceConsult",
        "Kết quả tư vấn dịch vụ",
        "Bussiness",
        4,
        "showproduct_32x32.png",
        "A",
        Module.MODULE_TYPE_ID__FORM,
        true,
        true)]
    public class HisServiceConsultProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;

        public HisServiceConsultProcessor()
        {
            param = new CommonParam();
        }

        public HisServiceConsultProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IHisServiceConsult behavior = HisServiceConsultFactory.MakeIControl(param, args);
                result = behavior != null ? behavior.Run() : null;
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
