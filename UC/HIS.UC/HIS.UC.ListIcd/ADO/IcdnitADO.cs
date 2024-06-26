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
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ListIcd.ADO
{
    public class IcdInitADO
    {
        public List<IcdADO> ListIcd { get; set; }
        public List<IcdColumn> ListIcdColumn { get; set; }

        public bool? IsShowSearchPanel { get; set; }

        public Grid_CustomUnboundColumnData IcdGrid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click btn_Radio_Enable_Click { get; set; }
        public gridViewIcd_MouseDownIcd gridViewIcd_MouseDownIcd { get; set; }

        public Delegate_repositoryItemButtonEdit_ContraindicationContent_ButtonClick Delegate_repositoryItemButtonEdit_ContraindicationContent_ButtonClick { get; set; }
        public Delegate_repositoryItemButtonEdit_ContraindicationContent_EditValueChanged Delegate_repositoryItemButtonEdit_ContraindicationContent_EditValueChanged { get; set; }
        public Delegate_repositoryItemButtonEdit_ContraindicationContent_Leave Delegate_repositoryItemButtonEdit_ContraindicationContent_Leave { get; set; }
    }
}
