using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.HisMediInStockByExpireDate.Export
{
    internal class ExportFactory
    {
        internal static IExport MakeIExport(CommonParam param, UserControl data)
        {
            IExport export = null;
            try
            {
                if (data != null)
                {
                    export = new ExportBehavior(param, data);
                }
                if (export == null)
                {
                    throw new NullReferenceException();
                }
            }
            catch (NullReferenceException ex)
            {
                LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + LogUtil.TraceData(LogUtil.GetMemberName<UserControl>(() => data), data), ex);
                export = null;
            }
            catch (Exception ex2)
            {
                LogSystem.Error(ex2);
                export = null;
            }
            return export;
        }
    }
}
