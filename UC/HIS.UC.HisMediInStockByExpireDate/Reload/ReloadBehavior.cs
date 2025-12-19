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
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.HisMediInStockByExpireDate.Reload
{
    public sealed class ReloadBehavior : IReload
    {
        UserControl control;
        List<List<HisMedicineInStockSDO>> HisMediInStockByExpireDates;
        List<long> MedicineTypeIds;
        List<V_HIS_MEDI_STOCK> lstMediStock;
        public ReloadBehavior()
            : base()
        {
        }

        //public ReloadBehavior(CommonParam param, UserControl data, List<List<HisMedicineInStockSDO>> HisMediInStockByExpireDates)
        //    : base()
        //{
        //    this.control = data;
        //    this.HisMediInStockByExpireDates = HisMediInStockByExpireDates;
        //}

        public ReloadBehavior(CommonParam param, UserControl data, List<List<HisMedicineInStockSDO>> HisMediInStockByExpireDates, List<long> MedicineTypeIds, List<V_HIS_MEDI_STOCK> lstMediStock)
            : base()
        {
            this.control = data;
            this.HisMediInStockByExpireDates = HisMediInStockByExpireDates;
            this.MedicineTypeIds = MedicineTypeIds;
            this.lstMediStock = lstMediStock;
        }

        void IReload.Run()
        {
            try
            {
                ((HIS.UC.HisMediInStockByExpireDate.Run.UCHisMediInStockByExpireDate)this.control).Reload(HisMediInStockByExpireDates, MedicineTypeIds, lstMediStock);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
