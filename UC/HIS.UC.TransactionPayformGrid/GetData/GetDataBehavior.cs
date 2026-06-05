/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.UC.TransactionPayformGrid.GetData
{
    public sealed class GetDataBehavior : IGetData
    {
        UserControl control;

        public GetDataBehavior()
            : base()
        {
        }

        public GetDataBehavior(CommonParam param, UserControl control)
            : base()
        {
            this.control = control;
        }

        object IGetData.Run()
        {
            try
            {
                if (this.control is UCTransactionPayformGrid)
                {
                    return ((UCTransactionPayformGrid)this.control).GetData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
    }
}
