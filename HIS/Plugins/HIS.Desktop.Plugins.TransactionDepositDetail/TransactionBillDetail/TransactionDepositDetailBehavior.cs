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
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionDepositDetail.TransactionDepositDetail
{
    class TransactionDepositDetailBehavior : Tool<IDesktopToolContext>, ITransactionDepositDetail
    {
        Inventec.Desktop.Common.Modules.Module Module;
        long billId = 0;
        public TransactionDepositDetailBehavior()
            : base()
        {
        }

        public TransactionDepositDetailBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, long data)
            : base()
        {
            this.billId = data;
            this.Module = module;
        }

        object ITransactionDepositDetail.Run()
        {
            try
            {
                return new frmTransactionDepositDetail(this.Module, this.billId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true;
                return null;
            }
        }
    }
}
