using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;

namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    public sealed class CallPatientCashierV3Behavior : Tool<IDesktopToolContext>, ICallPatientCashierV3
    {
        object[] entity;

        public CallPatientCashierV3Behavior()
            : base()
        {
        }

        public CallPatientCashierV3Behavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ICallPatientCashierV3.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module module = null;
                if (this.entity != null)
                {
                    foreach (var item in this.entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            module = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                    }
                }
                if (module != null)
                {
                    return new frmConfigCashierV3(module);
                }
                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
