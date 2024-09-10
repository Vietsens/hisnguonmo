using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Loading.Design.Template1
{
    internal partial class Template1
    {
        internal bool SetDelegateRunCompleted(BWRunWorkerCompleted RunCompleted)
        {
            bool result = false;
            try
            {
                this._RunCompleted = RunCompleted;
                if (this._RunCompleted != null )
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateDoWorker(BWDoWorker DoWorker)
        {
            bool result = false;
            try
            {
                this._DoWorker = DoWorker;
                if (_DoWorker != null)
                {
                    result = true;
                }
                else
                {
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
