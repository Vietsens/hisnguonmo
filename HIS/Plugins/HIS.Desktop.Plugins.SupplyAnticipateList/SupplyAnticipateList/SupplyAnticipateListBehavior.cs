using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.SupplyAnticipateList.SupplyAnticipateList
{
    /// <summary>
    /// Args: Module
    /// </summary>
    class SupplyAnticipateListBehavior : Tool<IDesktopToolContext>, ISupplyAnticipateList
    {
        object[] entity;
        internal SupplyAnticipateListBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ISupplyAnticipateList.Run()
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
                    return new UCSupplyAnticipateList(moduleData);
                }
                Inventec.Common.Logging.LogSystem.Warn("SupplyAnticipateList: thieu Module");
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
