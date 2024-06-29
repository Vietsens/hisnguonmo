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

using ACS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.Module.ADO
{
    public class ModuleInitADO
    {
        public List<ModuleADO> ListModule { get; set; }
        public List<ACS_MODULE> acsModule { get; set; }
        public List<ModuleColumn> ListModuleColumn { get; set; }

        public bool? IsShowSearchPanel { get; set; }

        public Grid_CustomUnboundColumnData ModuleGrid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click1 btn_Radio_Enable_Click1 { get; set; }
        public gridViewModule_MouseDownModule gridViewModule_MouseDownModule { get; set; }

        public GridView_MouseRightClick gridView_MouseRightClick { get; set; }
    }
}
