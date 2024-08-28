using Inventec.Core;
using Inventec.Desktop.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Logging;

namespace Inventec.Desktop.Plugins.Updater
{
    class VersionUpdateBehavior : BusinessBase, IVersionUpdate
    {
        object[] entity;
        internal VersionUpdateBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IVersionUpdate.Run()
        {
            try
            {
                return new frmVersionUpdate();

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
