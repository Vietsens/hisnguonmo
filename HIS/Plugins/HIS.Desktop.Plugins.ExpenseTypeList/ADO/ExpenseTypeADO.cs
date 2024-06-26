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
using HTC.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpenseTypeList.ADO
{
    public class ExpenseTypeADO : HTC_EXPENSE_TYPE
    {
        public bool AllowCreateExpense { get; set; }
        public bool IsPlus { get; set; }
        public string CreateTimeStr { get; set; }
        public string ModifyTimeStr { get; set; }

        public ExpenseTypeADO() { }

        public ExpenseTypeADO(HTC_EXPENSE_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HTC_EXPENSE_TYPE>(this, data);
                    this.AllowCreateExpense = (this.IS_ALLOW_EXPENSE == IMSys.DbConfig.HTC_RS.HTC_EXPENSE_TYPE.IS_ALLOW_EXPENSE__TRUE);
                    this.IsPlus = (this.IS_PLUS == IMSys.DbConfig.HTC_RS.HTC_EXPENSE_TYPE.IS_PLUS__TRUE);
                    this.CreateTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.CREATE_TIME ?? 0);
                    this.ModifyTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.MODIFY_TIME ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
