using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.HisMateInStockByExpireDate.GetListTreeView
{
    internal class GetListTreeViewFactory
    {
        internal static IGetListTreeView MakeIGetListTreeView(UserControl data)
        {
            IGetListTreeView getListTreeView = null;
            try
            {
                getListTreeView = new GetListTreeViewBehavior(data);
                if (getListTreeView == null)
                {
                    throw new NullReferenceException();
                }
            }
            catch (NullReferenceException ex)
            {
                LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + LogUtil.TraceData(LogUtil.GetMemberName<UserControl>(() => data), data), ex);
                getListTreeView = null;
            }
            catch (Exception ex2)
            {
                LogSystem.Error(ex2);
                getListTreeView = null;
            }
            return getListTreeView;
        }
    }
}
