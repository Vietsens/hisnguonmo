﻿using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisSurgQuota.Config
{
    class HisSurgQuotaProcessor
    {
        [ExtensionOf(typeof(DesktopRootExtensionPoint), "HIS.Desktop.Plugins.HisSurgQuota", "Khác", "Bussiness", 8, "pttt.png", "", Module.MODULE_TYPE_ID__UC, true, true)]
        public class HisOpensourceHisStoreProcessor : ModuleBase, IDesktopRoot
        {
            CommonParam param;
            public HisOpensourceHisStoreProcessor()
            {
                param = new CommonParam();
            }
            public HisOpensourceHisStoreProcessor(CommonParam paramBussiness)
            {
                param = paramBussiness == null ? new CommonParam() : paramBussiness;
            }
            public object Run(object[] args)
            {
                object result = null;
                try
                {
                    IHisSurgQuota behavior = HisSurgQuotaFactory.MakeIControl(param, args);
                    result = behavior != null ? (object)(behavior.Run()) : null;
                    return result;
                }
                catch (Exception e)
                {
                    Inventec.Common.Logging.LogSystem.Warn(e);
                    return result;
                }
            }
            public override bool IsEnable()
            {
                bool result = false;
                try
                {
                    result = true;
                }
                catch (Exception e)
                {

                    return result;
                }
                return result;
            }
        }
    }
}
