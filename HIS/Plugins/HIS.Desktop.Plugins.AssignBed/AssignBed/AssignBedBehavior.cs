using HIS.Desktop.ADO;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignBed.AssignBed
{
    public sealed class AssignBedBehavior : Tool<IDesktopToolContext>, IAssignBed
    {
        AssignServiceADO entity;
        Inventec.Desktop.Common.Modules.Module Module;
        public AssignBedBehavior()
            : base()
        {

        }

        public AssignBedBehavior(CommonParam param, AssignServiceADO data, Inventec.Desktop.Common.Modules.Module module)
            : base()
        {
            this.entity = data;
            this.Module = module;
        }

        object IAssignBed.Run()
        {
            try
            {
                return new frmAssignBed(this.Module, this.entity);
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
