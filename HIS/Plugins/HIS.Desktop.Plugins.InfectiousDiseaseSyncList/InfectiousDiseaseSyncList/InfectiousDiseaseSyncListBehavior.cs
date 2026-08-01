/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Parse args (Module) -> mở Form danh sách đồng bộ.
 */
using HIS.Desktop.Plugins.InfectiousDiseaseSyncList.MainForm;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.InfectiousDiseaseSyncList
{
    class InfectiousDiseaseSyncListBehavior : Tool<IDesktopToolContext>, IInfectiousDiseaseSyncList
    {
        object[] entity;

        internal InfectiousDiseaseSyncListBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IInfectiousDiseaseSyncList.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                    }
                }
                return new UCInfectiousDiseaseSyncList(moduleData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
