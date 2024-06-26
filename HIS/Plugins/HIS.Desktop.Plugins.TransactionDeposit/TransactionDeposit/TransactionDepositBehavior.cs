/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using HIS.Desktop.ADO;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionDeposit.TransactionDeposit
{
    class TransactionDepositBehavior : Tool<IDesktopToolContext>, ITransactionDeposit
    {
        Inventec.Desktop.Common.Modules.Module Module;
        TransactionDepositADO ado;
        internal TransactionDepositBehavior()
            : base()
        {

        }

        internal TransactionDepositBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, TransactionDepositADO data)
            : base()
        {
            Module = module;
            ado = data;
        }

        object ITransactionDeposit.Run()
        {
            object result = null;
            try
            {
                result = new frmTransactionDeposit(Module, ado);
                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ado), ado));
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
