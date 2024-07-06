using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.OpensourceHisStoreReportDetail.OpensourceHisStoreReportDetail
{
    class OpensourceHisStoreReportDetailBehavior : Tool<IDesktopToolContext>, IOpensourceHisStoreReportDetail
    {
        object[] entity;
        internal OpensourceHisStoreReportDetailBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IOpensourceHisStoreReportDetail.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                HisStoreServiceReportSDO listData = null;
                //string serviceCode = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        if (entity[i] is HisStoreServiceReportSDO)
                        {
                            listData = (HisStoreServiceReportSDO)entity[i];
                        }
                    }
                }
                return new frmDetailMarketReport(moduleData, listData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
