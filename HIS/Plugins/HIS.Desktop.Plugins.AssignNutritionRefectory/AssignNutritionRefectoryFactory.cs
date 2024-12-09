using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNutritionRefectory
{
    public class AssignNutritionRefectoryFactory
    {
        internal static IAssignNutritionRefectory MakeIAssignNutritionRefectory(CommonParam param, object[] data)
        {
            IAssignNutritionRefectory result = null;
            
            try
            {
                
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("DATA_INPUT", data));

                result = new AssignNutritionRefectoryBehavior(param,data);
                
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException e)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong tao duoc " + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), e);
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
