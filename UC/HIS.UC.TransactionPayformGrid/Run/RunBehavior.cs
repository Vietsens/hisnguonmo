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

namespace HIS.UC.TransactionPayformGrid.Run
{
    public sealed class RunBehavior : IRun
    {
        TransactionPayformGridInitADO entity;

        public RunBehavior()
            : base()
        {
        }

        public RunBehavior(CommonParam param, TransactionPayformGridInitADO data)
            : base()
        {
            this.entity = data;
        }

        object IRun.Run()
        {
            try
            {
                return new UCTransactionPayformGrid(this.entity);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
