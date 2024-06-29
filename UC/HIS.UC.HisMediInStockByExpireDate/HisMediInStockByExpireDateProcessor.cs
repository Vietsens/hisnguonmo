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
using HIS.UC.HisMediInStockByExpireDate.GetListCheck;
using HIS.UC.HisMediInStockByExpireDate.GetListTreeView;
using HIS.UC.HisMediInStockByExpireDate.Reload;
using HIS.UC.HisMediInStockByExpireDate.Run;
using HIS.UC.HisMediInStockByExpireDate.Search;
using HIS.UC.HisMediInStockByExpireDate.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;

namespace HIS.UC.HisMediInStockByExpireDate
{
    public class HisMediInStockByExpireDateProcessor : BussinessBase
    {
        object uc;
        public HisMediInStockByExpireDateProcessor()
            : base()
        {
        }

        public HisMediInStockByExpireDateProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(HisMediInStockByExpireDateInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIHisMediInStockByExpireDate(param, arg);
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

        //public void Reload(UserControl control, List<List<HisMedicineInStockSDO>> HisMediInStockByExpireDates)
        //{
        //    try
        //    {
        //        IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), HisMediInStockByExpireDates);
        //        if (behavior != null) behavior.Run();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        public void Reload(UserControl control, List<List<HisMedicineInStockSDO>> HisMediInStockByExpireDates, List<long> MedicineTypeIds)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), HisMediInStockByExpireDates, MedicineTypeIds);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<HisMediInStockByExpireDateADO> GetListCheck(UserControl control)
        {
            List<HisMediInStockByExpireDateADO> result = null;
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

        public List<HisMediInStockByExpireDateADO> GetListTreeView(UserControl control)
        {
            List<HisMediInStockByExpireDateADO> result = null;
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
