using HIS.UC.HisMateInStockByExpireDate.ADO;
using HIS.UC.HisMateInStockByExpireDate.Run;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.HisMateInStockByExpireDate.GetListTreeView
{
    internal class GetListTreeViewBehavior : IGetListTreeView
    {
        internal GetListTreeViewBehavior(UserControl control)
        {
            this.entity = control;
        }
        List<HisMateInStockByExpireDateADO> IGetListTreeView.Run()
        {
            List<HisMateInStockByExpireDateADO> result = null;
            try
            {
                if (this.entity.GetType() == typeof(UCHisMateInStockByExpireDate))
                {
                    result = ((UCHisMateInStockByExpireDate)this.entity).GetListTreeView();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        // Token: 0x04000027 RID: 39
        private UserControl entity;
    }
}
