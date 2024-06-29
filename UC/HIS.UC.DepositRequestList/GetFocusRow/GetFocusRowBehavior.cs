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
using HIS.UC.DepositRequestList.ADO;
using HIS.UC.DepositRequestList.GetFocusRow;
using HIS.UC.DepositRequestList.Reload;
using HIS.UC.DepositRequestList.Run;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.DepositRequestList.GetFocusRow
{
    class GetFocusRowBehavior : IGetFocusRow
    {
        UserControl entity;

        internal GetFocusRowBehavior(UserControl control)
            : base()
        {
            this.entity = control;
        }

        List<V_HIS_DEPOSIT_REQ> IGetFocusRow.Run()
        {
            List<V_HIS_DEPOSIT_REQ> result = null;
            try
            {
                if (this.entity.GetType() == typeof(UC_DepositRequestList))
                {
                    result = ((UC_DepositRequestList)this.entity).GetFocusRow();
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
