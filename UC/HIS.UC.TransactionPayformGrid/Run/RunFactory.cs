/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using HIS.UC.TransactionPayformGrid.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.UC.TransactionPayformGrid.Run
{
    class RunFactory
    {
        internal static IRun MakeIRun(CommonParam param, object data)
        {
            IRun result = null;
            try
            {
                if (data is TransactionPayformGridInitADO)
                {
                    result = new RunBehavior(param, (TransactionPayformGridInitADO)data);
                }
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + (data != null ? data.GetType().ToString() : "null") + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
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
