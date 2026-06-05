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
    public sealed class ReloadBehavior : IReload
    {
        UserControl control;
        List<PayformRowADO> data;

        public ReloadBehavior()
            : base()
        {
        }

        public ReloadBehavior(CommonParam param, UserControl control, List<PayformRowADO> data)
            : base()
        {
            this.control = control;
            this.data = data;
        }

        void IReload.Run()
        {
            try
            {
                if (this.control is UCTransactionPayformGrid)
                {
                    ((UCTransactionPayformGrid)this.control).Reload(this.data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
