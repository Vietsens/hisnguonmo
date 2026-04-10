using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.HisImportXmlAdjust;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.Common;

namespace Inventec.Desktop.Plugins.HisImportXmlAdjust.Run
{
    public sealed class HisImportXmlAdjustBehavior : Tool<IDesktopToolContext>, IHisImportXmlAdjust
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        RefeshReference delegateRefresh;

        public HisImportXmlAdjustBehavior()
            : base()
        {
        }

        public HisImportXmlAdjustBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisImportXmlAdjust.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is RefeshReference)
                        {
                            delegateRefresh = (RefeshReference)item;
                        }
                    }
                    if (currentModule != null && delegateRefresh != null)
                    {
                        result = new frmImportXmlAdjust(currentModule, delegateRefresh);
                    }
                    else
                    {
                        result = new frmImportXmlAdjust(currentModule);
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
