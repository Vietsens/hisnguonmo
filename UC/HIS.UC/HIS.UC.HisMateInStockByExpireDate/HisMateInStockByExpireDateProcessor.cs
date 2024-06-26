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
using HIS.UC.HisMateInStockByExpireDate.GetListCheck;
using HIS.UC.HisMateInStockByExpireDate.GetListTreeView;
using HIS.UC.HisMateInStockByExpireDate.Reload;
using HIS.UC.HisMateInStockByExpireDate.Run;
using HIS.UC.HisMateInStockByExpireDate.Search;
using HIS.UC.HisMateInStockByExpireDate.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;

namespace HIS.UC.HisMateInStockByExpireDate
{
    public class HisMateInStockByExpireDateProcessor : BussinessBase
    {
        object uc;
        public HisMateInStockByExpireDateProcessor()
            : base()
        {
        }

        public HisMateInStockByExpireDateProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(HisMateInStockByExpireDateInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIHisMateInStockByExpireDate(param, arg);
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

        public void Reload(UserControl control, List<List<HisMaterialInStockSDO>> HisMateInStockByExpireDates, List<long> MedicineTypeIds)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), HisMateInStockByExpireDates, MedicineTypeIds);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<HisMateInStockByExpireDateADO> GetListCheck(UserControl control)
        {
            List<HisMateInStockByExpireDateADO> result = null;
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

        public List<HisMateInStockByExpireDateADO> GetListTreeView(UserControl control)
        {
            List<HisMateInStockByExpireDateADO> result = null;
            try
            {
                IGetListTreeView behavior = GetListTreeViewFactory.MakeIGetListTreeView(control);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public void ExportExcel(UserControl control)
        {
            try
            {
                Export.IExport behavior = Export.ExportFactory.MakeIExport(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
