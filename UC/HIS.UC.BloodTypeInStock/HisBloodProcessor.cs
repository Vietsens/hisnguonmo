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
using HIS.UC.HisBloodTypeInStock.GetListCheck;
using HIS.UC.HisBloodTypeInStock.Reload;
using HIS.UC.HisBloodTypeInStock.Run;
using HIS.UC.HisBloodTypeInStock.Search;
using HIS.UC.HisBloodTypeInStock.ADO;
using HIS.UC.HisBloodTypeInStock.New;
using HIS.UC.HisBloodTypeInStock.Focus;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.UC.HisBloodTypeInStock.Export;
using HIS.UC.BloodTypeInStock.Dispose;

namespace HIS.UC.HisBloodTypeInStock
{
    public class HisBloodTypeInStockProcessor : BussinessBase
    {
        object uc;
        public HisBloodTypeInStockProcessor()
            : base()
        {
        }

        public HisBloodTypeInStockProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(HisBloodTypeInStockInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIHisBloodTypeInStock(param, arg);
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

        public void Reload(UserControl control, List<HisBloodTypeInStockSDO> HisBloodTypeInStocks)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), HisBloodTypeInStocks);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(UserControl control, List<HisBloodInStockSDO> HisBloodInStocks)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), HisBloodInStocks);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void Export(UserControl control,string path)
        {
            try
            {
                IExport behavior = ExportFactory.MakeIExport(param, (control == null ? (UserControl)uc : control), path);
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
        public List<HisBloodTypeInStockADO> GetListCheck(UserControl control)
        {
            List<HisBloodTypeInStockADO> result = null;
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
