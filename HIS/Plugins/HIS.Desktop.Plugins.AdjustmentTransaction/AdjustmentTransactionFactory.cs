using HIS.Desktop.Common;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AdjustmentTransaction
{
    internal class AdjustmentTransactionFactory
    {
        internal static IAdjustmentTransaction MakeIIAdjustmentTransaction(CommonParam param, object[] data)
        {
            IAdjustmentTransaction result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            DelegateRefreshData delegateRefreshData = null;
            V_HIS_TRANSACTION tran = null;
            try
            {
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            if (data[i] is V_HIS_TRANSACTION)
                            {
                                tran = (V_HIS_TRANSACTION)data[i];
                            }
                            else if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                            else if (data[i] is DelegateRefreshData)
                            {
                                delegateRefreshData = (DelegateRefreshData)data[i];
                            }
                        }
                    }
                }
                if (moduleData != null && tran != null)
                {
                    result = new AdjustmentTransactionBehavior(moduleData, tran, delegateRefreshData);
                }

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
