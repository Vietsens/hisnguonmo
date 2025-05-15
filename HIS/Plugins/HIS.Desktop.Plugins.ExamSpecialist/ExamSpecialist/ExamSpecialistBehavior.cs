using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExamSpecialist.ExamSpecialist
{
    class ExamSpecialistBehavior : Tool<IDesktopToolContext>, IExamSpecialist
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        
        public ExamSpecialistBehavior()
            : base()
        {
        }

        public ExamSpecialistBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IExamSpecialist.Run()
        {
            long treatmentID = 0;
            object result = null;
            V_HIS_SPECIALIST_EXAM obj = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is long)
                        {
                            treatmentID = (long)item;
                        }
                        else if (item is V_HIS_SPECIALIST_EXAM)
                        {
                            obj = (V_HIS_SPECIALIST_EXAM)item;
                        }
                    }

                    result = new frmExamSpecialist(currentModule, treatmentID, obj);
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
