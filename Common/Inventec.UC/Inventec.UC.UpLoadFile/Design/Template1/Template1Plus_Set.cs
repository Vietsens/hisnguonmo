using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.UpLoadFile.Design.Template1
{
    internal partial class Template1
    {
        internal bool SetDelegateUpLoad(UpLoadFileToServer upload)
        {
            bool result = false;
            try
            {
                this._UpLoad = upload;
                if (_UpLoad != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => upload), upload));
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
