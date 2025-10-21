using HIS.Desktop.Common;
using HIS.Desktop.Plugins.AdjustmentTransaction.AdjustmentTransaction;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AdjustmentTransaction
{
    class AdjustmentTransactionBehavior : Tool<IDesktopToolContext>, IAdjustmentTransaction
    {
        Inventec.Desktop.Common.Modules.Module Module;
        V_HIS_TRANSACTION tran = null;
        DelegateRefreshData delegateRefreshData = null;
        internal AdjustmentTransactionBehavior(Inventec.Desktop.Common.Modules.Module module, V_HIS_TRANSACTION tran, DelegateRefreshData delegateRefreshData)
            : base()
        {
            this.Module = module;
            this.tran = tran;
            this.delegateRefreshData = delegateRefreshData;
        }

        object IAdjustmentTransaction.Run()
        {
            object result = null;

            try
            {
                result = new frmAdjustmentTransaction(Module, tran, this.delegateRefreshData);
                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tran), tran));
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
