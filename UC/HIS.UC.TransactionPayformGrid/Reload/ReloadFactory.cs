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
using System.Windows.Forms;

namespace HIS.UC.TransactionPayformGrid.Reload
{
    class ReloadFactory
    {
        internal static IReload MakeIReload(CommonParam param, UserControl control, List<PayformRowADO> data)
        {
            IReload result = null;
            try
            {
                result = new ReloadBehavior(param, control, data);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
