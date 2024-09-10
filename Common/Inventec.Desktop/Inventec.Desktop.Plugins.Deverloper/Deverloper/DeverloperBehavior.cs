using Inventec.Core;
using Inventec.Desktop.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Logging;

namespace Inventec.Desktop.Plugins.Deverloper
{
    class DeverloperBehavior : BusinessBase, IDeverloper
    {
        object[] entity;
        internal DeverloperBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IDeverloper.Run()
        {
            try
            {
                return new frmDeverloper();

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
