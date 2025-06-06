using HIS.Desktop.Plugins.HisHolidayPolicies.HisDayPolicies;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisHolidayPolicies
{
    public sealed class HisDayPoliciesBehavior : Tool<IDesktopToolContext>, IHisDayPolicies
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;

        public HisDayPoliciesBehavior()
            :base()
        {
        }
        
        public HisDayPoliciesBehavior(CommonParam param, object[] filter)
            :base()
        {
            this.entity = filter;
        }

        object IHisDayPolicies.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Length > 0)
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
                        result = new frmHisDayPolicies(currentModule);
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
