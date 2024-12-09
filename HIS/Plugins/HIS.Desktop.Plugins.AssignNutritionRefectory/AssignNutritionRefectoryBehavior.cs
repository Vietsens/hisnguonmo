using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNutritionRefectory
{
    class AssignNutritionRefectoryBehavior : IAssignNutritionRefectory
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        object[] data;

        internal AssignNutritionRefectoryBehavior(CommonParam param, object[] filter)
                : base()
        {
            this.data = filter;
        }
        public object Run()
        {
            object result = null;
            try
            {
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                        }
                    }
                }
                result = new frmAssign(moduleData);
                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData), moduleData));
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
