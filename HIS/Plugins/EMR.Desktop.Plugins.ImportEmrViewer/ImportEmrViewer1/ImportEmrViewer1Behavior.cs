using Inventec.Desktop.Core.Tools;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.ImportEmrViewer.ImportEmrViewer1
{
    class ImportEmrViewer1Behavior : Tool<IDesktopToolContext>, ImportEmrViewer1
    {
        object[] entity;

        internal ImportEmrViewer1Behavior()
            : base()
        { }

        internal ImportEmrViewer1Behavior(Inventec.Core.CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object ImportEmrViewer1.Run()
        {
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                    }
                }

                if (moduleData != null)
                {
                    return new frmImportViewer(moduleData);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
