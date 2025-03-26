using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MonitorMediStockDataError.MonitorMediStockDataError
{
    class MonitorMediStockDataErrorBehavior : Tool<IDesktopToolContext>, IMonitorMediStockDataError
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        public MonitorMediStockDataErrorBehavior()
            : base()
        {
        }

        public MonitorMediStockDataErrorBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IMonitorMediStockDataError.Run()
        {
            //object result = null;
            //try
            //{
            //    Inventec.Desktop.Common.Modules.Module moduleData = null;
            //    if (entity != null && entity.Count() > 0)
            //    {
            //        for (int i = 0; i < entity.Count(); i++)
            //        {
            //            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
            //            {
            //                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
            //            }
            //        }
            //    }
            //    if (moduleData != null)
            //    {
            //        return new UCMonitorMediStockDataError(moduleData);
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //    result = null;
            //}
            //return result;
            object result = null;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Gọi MonitorMediStockDataErrorBehavior");
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }

                    }
                    if (currentModule != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Gọi module thành công");
                        result = new UCMonitorMediStockDataError(currentModule);
                    }
                }
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
