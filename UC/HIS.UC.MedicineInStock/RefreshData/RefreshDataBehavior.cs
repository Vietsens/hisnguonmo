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
using HIS.UC.HisMedicineInStock.ADO;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.HisMedicineInStock.RefreshData
{
    public sealed class RefreshDataBehavior : IRefreshData
    {
        UserControl control;
        HisMedicineInStockADO HisMedicineInStocks;
        decimal? parentAvailableAmount;
        public RefreshDataBehavior()
            : base()
        {
        }

        public RefreshDataBehavior(CommonParam param, UserControl data, HisMedicineInStockADO HisMedicineInStocks, decimal? parentAvailableAmount)
            : base()
        {
            this.control = data;
            this.HisMedicineInStocks = HisMedicineInStocks;
            this.parentAvailableAmount = parentAvailableAmount;
        }

        void IRefreshData.Run()
        {
            try
            {
                ((HIS.UC.HisMedicineInStock.Run.UCHisMedicineInStock)this.control).RefreshData(HisMedicineInStocks, parentAvailableAmount);                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}