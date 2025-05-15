using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
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
        long treatmentId = 0;
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
            object result = null;
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
                    }

                    result = new frmExamSpecialist(currentModule, treatmentId);
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
