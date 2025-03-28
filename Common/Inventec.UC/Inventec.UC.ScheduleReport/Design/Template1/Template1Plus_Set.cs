﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ScheduleReport.Design.Template1
{
    internal partial class Template1
    {
        internal bool SetDelegateHasException(ProcessHasException hasException)
        {
            bool result = false;
            try
            {
                this._HasException = hasException;
                if (_HasException != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hasException), hasException));
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
