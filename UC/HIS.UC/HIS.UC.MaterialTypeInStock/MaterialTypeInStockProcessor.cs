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
using HIS.UC.MaterialTypeInStock.ADO;
using HIS.UC.MaterialTypeInStock.Focus;
using HIS.UC.MaterialTypeInStock.GetListCheck;
using HIS.UC.MaterialTypeInStock.New;
using HIS.UC.MaterialTypeInStock.Reload;
using HIS.UC.MaterialTypeInStock.Run;
using HIS.UC.MaterialTypeInStock.Search;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.MaterialTypeInStock
{
    public class MaterialTypeInStockTreeProcessor : BussinessBase
    {
        object uc;
        public MaterialTypeInStockTreeProcessor()
            : base()
        {
        }

        public MaterialTypeInStockTreeProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(MaterialTypeInStockInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIMaterialTypeInStock(param, arg);
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

        public void Reload(UserControl control, List<HisMaterialTypeInStockSDO> MaterialTypeInStocks)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), MaterialTypeInStocks);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void New(UserControl control)
        {
            try
            {
                INew behavior = NewFactory.MakeINew(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<MaterialTypeInStockADO> GetListCheck(UserControl control)
        {
            List<MaterialTypeInStockADO> result = null;
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

        public void FocusKeyword(UserControl uc)
        {
            try
            {
                IFocus behavior = FocusFactory.MakeIFocus(uc);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
