using Inventec.Core;
using Inventec.Desktop.Common;
using Inventec.Desktop.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Plugins.ConfigApplication.ConfigApplication
{
    class ConfigApplicationBehavior : BusinessBase, IConfigApplication
    {
        Inventec.UC.ConfigApplication.Refesh entity;
        internal ConfigApplicationBehavior(CommonParam param, object delgate)
            : base()
        {
            this.entity = (Inventec.UC.ConfigApplication.Refesh)delgate;
        }

        object IConfigApplication.Run()
        {
            try
            {
                return new UCConfigApplication(entity);
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
