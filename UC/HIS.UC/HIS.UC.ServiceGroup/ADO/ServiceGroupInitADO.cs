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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ServiceGroup.ADO
{
    public class ServiceGroupInitADO
    {
        public List<ServiceGroupADO> ListServiceGroup { get; set; }
        public List<ServiceGroupColumn> ListServiceGroupColumn { get; set; }

        public bool? IsShowSearchPanel { get; set; }

        public Grid_CustomUnboundColumnData ServiceGroupGrid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click1 btn_Radio_Enable_Click1 { get; set; }
        public gridViewServiceGroup_MouseDownMest gridViewServiceGroup_MouseDownMest { get; set; }
        public Spin_EditValueChanged spin_EditValueChanged { get; set; }
        public Check_CheckedChanged check_CheckedChanged { get; set; }
        public ServiceGroupGridView_Click serviceGroupGridView_Click { get; set; }
        public LockItem_Click lockItem_Click { get; set; }
        public UnLockItem_Click unLockItem_Click { get; set; }
        public DeleteItem_Click deleteItem_Click { get; set; }
    }
}
