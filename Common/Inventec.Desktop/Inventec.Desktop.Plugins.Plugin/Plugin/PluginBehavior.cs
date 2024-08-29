using Inventec.Core;
using Inventec.Desktop.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Logging;

namespace Inventec.Desktop.Plugins.Plugin
{
    class PluginBehavior : BusinessBase, IPlugin
    {
        object[] entity;
        internal PluginBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IPlugin.Run()
        {
            try
            {
                return new frmPluginManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
