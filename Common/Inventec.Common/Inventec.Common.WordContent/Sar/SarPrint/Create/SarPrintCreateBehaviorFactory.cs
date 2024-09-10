using Inventec.Core;
using System;
using System.Collections.Generic;

namespace Inventec.Common.WordContent.SarPrint.Create
{
    class SarPrintCreateBehaviorFactory
    {
        internal static ISarPrintCreate MakeIAggrExpMestCreate(CommonParam param, object data)
        {
            ISarPrintCreate result = null;
            try
            {
                if (data.GetType() == typeof(SAR.EFMODEL.DataModels.SAR_PRINT))
                {
                    result = new SarPrintCreateBehavior(param, (SAR.EFMODEL.DataModels.SAR_PRINT)data);
                }
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
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
