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
using HIS.UC.ListMedicineType.ADO;
using HIS.UC.ListMedicineType.GetDataGridView;
using HIS.UC.ListMedicineType.Reload;
using HIS.UC.ListMedicineType.ReloadRow;
using HIS.UC.ListMedicineType.ReloadRow1;
using HIS.UC.ListMedicineType.Run;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.ListMedicineType.LoaCboMediStock;

namespace HIS.UC.ListMedicineType
{
    public class UCListMedicineTypeProcessor : BussinessBase
    {
        object uc;
        public UCListMedicineTypeProcessor()
            : base()
        {
        }

        public UCListMedicineTypeProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(ListMedicineTypeInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIExpMestMedicineGrid(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Reload(UserControl control, List<ListMedicineTypeADO> data)
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

        public void LoaCboMediStock(UserControl control, List<V_HIS_MEST_ROOM> data, bool readonl)
        {
            try
            {
                ILoaCboMediStock behavior = LoaCboMediStockFactory.MakeILoaCboMediStock(param, (control == null ? (UserControl)uc : control), data, readonl);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ReloadRow(UserControl control, HIS_MEDI_STOCK_METY data)
        {
            try
            {
                IReloadRow behavior = ReloadRowFactory.MakeIReloadRow(param, (control == null ? (UserControl)uc : control), data);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void ReloadRow(UserControl control, HIS_ANTIGEN_METY data)
        {
            try
            {
                IReloadRow1 behavior = ReloadRow1Factory.MakeIReloadRow(param, (control == null ? (UserControl)uc : control), data);
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

    }
}
