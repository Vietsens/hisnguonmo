using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.SupplyOrderList.SupplyOrderList
{
    /// <summary>
    /// Args: Module
    /// </summary>
    class SupplyOrderListBehavior : Tool<IDesktopToolContext>, ISupplyOrderList
    {
        object[] entity;
        internal SupplyOrderListBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ISupplyOrderList.Run()
        {
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                    }
                }

                if (moduleData != null)
                {
                    return new UCSupplyOrderList(moduleData);
                }
                Inventec.Common.Logging.LogSystem.Warn("SupplyOrderList: thieu Module");
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
