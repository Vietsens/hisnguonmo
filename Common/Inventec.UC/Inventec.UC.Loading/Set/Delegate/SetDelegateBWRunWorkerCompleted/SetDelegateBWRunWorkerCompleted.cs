using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Loading.Set.Delegate.SetDelegateBWRunWorkerCompleted
{
    class SetDelegateBWRunWorkerCompleted : ISetDelegateBWRunWorkerCompleted
    {
        public bool SetDelegateRunCompleted(System.Windows.Forms.UserControl UC, BWRunWorkerCompleted RunCompleted)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCLoading = (Design.Template1.Template1)UC;
                    result = UCLoading.SetDelegateRunCompleted(RunCompleted);
                }
                if (result == false)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => RunCompleted), RunCompleted));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
