using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApprovaleDebate.ApprovaleDebate
{
    public sealed class ApprovaleDebateBehavior : Tool<IDesktopToolContext>, IApprovaleDebate
    {
        object[] entity;
        Common.RefeshReference delegateRefresh;
        Inventec.Desktop.Common.Modules.Module module;
        V_HIS_SPECIALIST_EXAM v_his_specialist_exam;
        public ApprovaleDebateBehavior()
            : base()
        {
        }

        public ApprovaleDebateBehavior(CommonParam param, object[] entity)
            : base()
        {
            this.entity = entity;
        }

        object IApprovaleDebate.Run()
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
                            module = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        if (item is V_HIS_SPECIALIST_EXAM SPECIALIST)
                        {
                            v_his_specialist_exam = SPECIALIST;
                        }
                        if (item is Common.RefeshReference deleg)
                        {
                            delegateRefresh = deleg;
                        }
                    }
                    result = new frmApprovaleDebate(module, delegateRefresh, v_his_specialist_exam);
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
