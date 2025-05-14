using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApprovaleDebate.ApprovaleDebate
{
    public sealed class ApprovaleDebateBehavior : Tool<IDesktopToolContext>, IApprovaleDebate
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        public ApprovaleDebateBehavior()
            : base()
        {
        }

        public ApprovaleDebateBehavior(CommonParam param, Inventec.Desktop.Common.Modules.Module moduleData)
            : base()
        {
            this.moduleData = moduleData;
        }

        object IApprovaleDebate.Run()
        {
            try
            {
                return new frmApprovaleDebate(this.moduleData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true;
                return null;
            }
        }
    }
}
