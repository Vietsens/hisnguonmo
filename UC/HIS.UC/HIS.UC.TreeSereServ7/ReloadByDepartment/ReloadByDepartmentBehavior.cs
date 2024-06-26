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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TreeSereServ7.ReloadByDepartment
{
    public sealed class ReloadByDepartmentBehavior : IReloadByDepartment
    {
        UserControl control;
        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs;
        List<SereServADO> sereServsAdo;
        long _DepartmentId;
        public ReloadByDepartmentBehavior()
            : base()
        {
        }

        public ReloadByDepartmentBehavior(CommonParam param, UserControl data, long _departmentId, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs)
            : base()
        {
            this.control = data;
            this.sereServs = sereServs;
            this._DepartmentId = _departmentId;
        }

        public ReloadByDepartmentBehavior(CommonParam param, UserControl data, long _departmentId, List<SereServADO> sereServs)
            : base()
        {
            this.control = data;
            this.sereServsAdo = sereServs;
            this._DepartmentId = _departmentId;
        }

        void IReloadByDepartment.Run()
        {
            try
            {
                if (sereServsAdo != null && sereServsAdo.Count > 0)
                    ((HIS.UC.TreeSereServ7.Run.UCTreeSereServ7)this.control).Reload(_DepartmentId, sereServsAdo);
                else
                    ((HIS.UC.TreeSereServ7.Run.UCTreeSereServ7)this.control).Reload(_DepartmentId, sereServs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
