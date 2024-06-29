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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.HisBloodTypeInStock.Reload
{
    public sealed class ReloadBehavior : IReload
    {
        UserControl control;
        List<HisBloodTypeInStockSDO> HisBloodTypeInStocks;
        List<HisBloodInStockSDO> HisBloodInStocks;
        public ReloadBehavior()
            : base()
        {
        }

        public ReloadBehavior(CommonParam param, UserControl data, List<HisBloodTypeInStockSDO> HisBloodTypeInStocks)
            : base()
        {
            this.control = data;
            this.HisBloodTypeInStocks = HisBloodTypeInStocks;
        }
        public ReloadBehavior(CommonParam param, UserControl data, List<HisBloodInStockSDO> HisBloodInStocks)
            : base()
        {
            this.control = data;
            this.HisBloodInStocks = HisBloodInStocks;
        }

        void IReload.Run()
        {
            try
            {
                if (HisBloodTypeInStocks != null)
                    ((HIS.UC.HisBloodTypeInStock.Run.UCHisBloodTypeInStock)this.control).Reload(HisBloodTypeInStocks);
                else if (HisBloodInStocks != null)
                    ((HIS.UC.HisBloodTypeInStock.Run.UCHisBloodTypeInStock)this.control).Reload(HisBloodInStocks);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
