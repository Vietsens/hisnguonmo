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
using HIS.UC.HisMaterialInStock.GetListCheck;
using HIS.UC.HisMaterialInStock.Reload;
using HIS.UC.HisMaterialInStock.Run;
using HIS.UC.HisMaterialInStock.Search;
using HIS.UC.HisMaterialInStock.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.UC.HisMaterialInStock.FocusRow;
using HIS.UC.MaterialInStock.Dispose;
using HIS.UC.HisMedicineInStock.RefreshData;
using HIS.UC.HisMaterialInStock.RefreshData;
using HIS.UC.MaterialInStock.GetListAll;

namespace HIS.UC.HisMaterialInStock
{
    public class HisMaterialInStockProcessor : BussinessBase
    {
        object uc;
        public HisMaterialInStockProcessor()
            : base()
        {
        }

        public HisMaterialInStockProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(HisMaterialInStockInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIHisMaterialInStock(param, arg);
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

        public void Reload(UserControl control, List<HisMaterialInStockSDO> HisMaterialInStocks,List<long>MediStockId)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), HisMaterialInStocks, MediStockId);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void RefreshData(UserControl control, HisMaterialInStockADO HisMaterialInStocks, decimal? parentAvailableAmount)
        {
            try
            {
                IRefreshData behavior = RefreshDataFactory.MakeIRefreshData(param, (control == null ? (UserControl)uc : control), HisMaterialInStocks, parentAvailableAmount);
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

        public List<HisMaterialInStockADO> GetListCheck(UserControl control)
        {
            List<HisMaterialInStockADO> result = null;
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
        public List<HisMaterialInStockSDO> GetListAll(UserControl control)
        {
            List<HisMaterialInStockSDO> result = null;
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
