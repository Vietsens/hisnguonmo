using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.SupplyOrderCreate.SupplyOrderCreate
{
    /// <summary>
    /// Args: Module, MEDI_STOCK_ID (long, optional), V_HIS_SUPPLY_ORDER (optional - order to open), DelegateRefreshData (optional)
    /// </summary>
    class SupplyOrderCreateBehavior : Tool<IDesktopToolContext>, ISupplyOrderCreate
    {
        object[] entity;
        internal SupplyOrderCreateBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ISupplyOrderCreate.Run()
        {
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            long? mediStockId = null;
            V_HIS_SUPPLY_ORDER order = null;
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
                            mediStockId = (long)item;
                        }
                        else if (item is V_HIS_SUPPLY_ORDER)
                        {
                            order = (V_HIS_SUPPLY_ORDER)item;
                        }
                        else if (item is HIS.Desktop.Common.DelegateRefreshData)
                        {
                            refreshData = (HIS.Desktop.Common.DelegateRefreshData)item;
                        }
                    }
                }

                if (moduleData != null)
                {
                    return new frmSupplyOrderCreate(moduleData, mediStockId, order, refreshData);
                }
                Inventec.Common.Logging.LogSystem.Warn("SupplyOrderCreate: thieu Module");
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
