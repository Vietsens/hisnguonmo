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
using HIS.UC.MetyMatyTypeInStock.GetListCheck;
using HIS.UC.MetyMatyTypeInStock.Reload;
using HIS.UC.MetyMatyTypeInStock.Run;
using HIS.UC.MetyMatyTypeInStock.Search;
using HIS.UC.MetyMatyTypeInStock.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.UC.MetyMatyTypeInStock.Focus;
using HIS.UC.MetyMatyTypeInStock.SearchByKeyword;

namespace HIS.UC.MetyMatyTypeInStock
{
    public class MetyMatyTypeInStockProcessor : BussinessBase
    {
        object uc;
        public MetyMatyTypeInStockProcessor()
            : base()
        {
        }

        public MetyMatyTypeInStockProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(MetyMatyTypeInStockInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIMetyMatyTypeInStock(param, arg);
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

        public void SearchByKeyword(UserControl control, string keyword)
        {
            try
            {
                ISearchByKeyword behavior = SearchByKeywordFactory.MakeISearchByKeyword(param, (control == null ? (UserControl)uc : control), keyword);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(UserControl control, List<MOS.EFMODEL.DataModels.D_HIS_MEDI_STOCK_1> MetyMatyTypeInStocks)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), MetyMatyTypeInStocks);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<MetyMatyTypeInStockADO> GetListCheck(UserControl control)
        {
            List<MetyMatyTypeInStockADO> result = null;
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
