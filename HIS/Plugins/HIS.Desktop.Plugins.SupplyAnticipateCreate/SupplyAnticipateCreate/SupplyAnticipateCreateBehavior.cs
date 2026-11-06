using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.SupplyAnticipateCreate.SupplyAnticipateCreate
{
    /// <summary>
    /// Args: Module, ANTICIPATE_ID (long, optional - none = new), DelegateRefreshData (optional)
    /// </summary>
    class SupplyAnticipateCreateBehavior : Tool<IDesktopToolContext>, ISupplyAnticipateCreate
    {
        object[] entity;
        internal SupplyAnticipateCreateBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ISupplyAnticipateCreate.Run()
        {
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            long? anticipateId = null;
            HIS.Desktop.Common.DelegateRefreshData refreshData = null;
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
                        else if (item is long)
                        {
                            anticipateId = (long)item;
                        }
                        else if (item is HIS.Desktop.Common.DelegateRefreshData)
                        {
                            refreshData = (HIS.Desktop.Common.DelegateRefreshData)item;
                        }
                    }
                }

                if (moduleData != null)
                {
                    return new frmSupplyAnticipateCreate(moduleData, anticipateId, refreshData);
                }
                Inventec.Common.Logging.LogSystem.Warn("SupplyAnticipateCreate: thieu Module");
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
