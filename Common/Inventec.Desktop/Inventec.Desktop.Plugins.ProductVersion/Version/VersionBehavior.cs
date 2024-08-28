using Inventec.Core;
using Inventec.Desktop.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Logging;

namespace Inventec.Desktop.Plugins.ProductVersion
{
    class VersionBehavior : BusinessBase, IVersion
    {
        object[] entity;
        internal VersionBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IVersion.Run()
        {
            try
            {
                return new frmVersion();
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
