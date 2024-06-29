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
using HIS.UC.HisMedicineInStock.GetListCheck;
using HIS.UC.HisMedicineInStock.Reload;
using HIS.UC.HisMedicineInStock.Run;
using HIS.UC.HisMedicineInStock.Search;
using HIS.UC.HisMedicineInStock.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.UC.MedicineInStock.FocusRow;
using HIS.UC.MedicineInStock.Dispose;
using HIS.UC.MedicineInStock.GetListAll;
using HIS.UC.HisMedicineInStock.RefreshData;

namespace HIS.UC.HisMedicineInStock
{
    public class HisMedicineInStockProcessor : BussinessBase
    {
        object uc;
        public HisMedicineInStockProcessor()
            : base()
        {
        }

        public HisMedicineInStockProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(HisMedicineInStockInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIHisMedicineInStock(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Search(UserControl control)
        {
            try
            {
                ISearch behavior = SearchFactory.MakeISearch(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(UserControl control, List<HisMedicineInStockSDO> HisMedicineInStocks,List<long> MediStockID)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), HisMedicineInStocks, MediStockID);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void RefreshData(UserControl control, HisMedicineInStockADO HisMedicineInStocks, decimal? parentAvailableAmount)
        {
            try
            {
                IRefreshData behavior = RefreshDataFactory.MakeIRefreshData(param, (control == null ? (UserControl)uc : control), HisMedicineInStocks, parentAvailableAmount);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusRowTree(UserControl control)
        {
            try
            {
                IFocusRow behavior = FocusRowFactory.MakeIFocusRow(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void DisposeControl(UserControl control)
        {
            try
            {
                IDispose behavior = DisposeFactory.MakeIDispose((control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<HisMedicineInStockADO> GetListCheck(UserControl control)
        {
            List<HisMedicineInStockADO> result = null;
            try
            {
                IGetListCheck behavior = GetListCheckFactory.MakeIGetListCheck(control);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        public List<HisMedicineInStockSDO> GetListAll(UserControl control)
        {
            List<HisMedicineInStockSDO> result = null;
            try
            {
                IGetListAll behavior = GetListAllFactory.MakeIGetListAll(control);
                result = (behavior != null) ? behavior.Run() : null;
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
