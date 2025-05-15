using Inventec.Core;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.Plugins.ApprovalExamSpecialist.Run;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.ApprovalExamSpecialist
{
    public sealed class ApprovalExamSpecialistBehavior : Tool<IDesktopToolContext>, IApprovalExamSpecialist
    {
        object[] entity;
        public ApprovalExamSpecialistBehavior()
            : base()
        {
        }
        public ApprovalExamSpecialistBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }
        object IApprovalExamSpecialist.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                long treatmentID = 0;
                V_HIS_SPECIALIST_EXAM obj = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is long)
                        {
                            treatmentID = (long)entity[i];
                        }
                        else if (entity[i] is V_HIS_SPECIALIST_EXAM)
                        {
                            obj = (V_HIS_SPECIALIST_EXAM)entity[i];
                        }
                    }
                
                }
                if (moduleData != null)
                {
                    return new frmApprovalExamSpecialist(moduleData, treatmentID, obj);
                }
                else
                {
                    return null;
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
