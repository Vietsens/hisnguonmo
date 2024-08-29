using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Loading.Set.Delegate.SetDelegateBWDoWorker
{
    class SetDelegateBWDoWorker : ISetDelegateBWDoWorker
    {
        public bool SetDelegateDoWorker(System.Windows.Forms.UserControl UC, BWDoWorker DoWorker)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCLoading = (Design.Template1.Template1)UC;
                    result = UCLoading.SetDelegateDoWorker(DoWorker);
                }
                if (result == false) 
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => DoWorker), DoWorker));
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
