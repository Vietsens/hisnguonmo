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
using HIS.UC.HisMaterialInStock.ADO;
using HIS.UC.HisMaterialInStock.Reload;
using HIS.UC.HisMaterialInStock.Run;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.HisMaterialInStock.GetData;
using HIS.UC.HisMaterialInStock.Focus;

namespace HIS.UC.HisMaterialInStock
{
    public class MaterialInStockProcessor : BusinessBase
    {
        object uc;
        public MaterialInStockProcessor()
            : base()
        {
        }

        public MaterialInStockProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(HisMaterialInStockInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIRun(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Reload(UserControl control, List<MOS.SDO.HisMaterialInStockSDO> data)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), data);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public object GetDataGridView(UserControl control)
        {
            object result = null;
            try
            {
                IGetDataGridView behavior = GetDataGridViewFactory.MakeIGetDataGridView(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
