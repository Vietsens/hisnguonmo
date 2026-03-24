using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RefundByTransfer.RefundByTransfer
{
    class RefundByTransferFactory
    {
        internal static IRefundByTransfer MakeIControl(CommonParam param, object[] data)
        {
            IRefundByTransfer result = null;

            try
            {
                result = new RefundByTransferBehavior(param, data);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." +
                    data.GetType().ToString() +
                    Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }

            return result;
        }
    }
}
