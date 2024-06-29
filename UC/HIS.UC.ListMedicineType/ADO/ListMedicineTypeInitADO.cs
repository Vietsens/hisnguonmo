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

namespace HIS.UC.ListMedicineType.ADO
{
    public class ListMedicineTypeInitADO
    {
        public List<ListMedicineTypeADO> ListMedicineType { get; set; }
        public List<V_HIS_MEDICINE_TYPE> MedicinType { get; set; }
        public List<ListMedicineTypeColumn> ListMedicineTypeColumn { get; set; }
        public List<V_HIS_MEST_ROOM> LisMestRoom { get; set; }

        public bool? IsShowSearchPanel { get; set; }

        public Grid_CustomUnboundColumnData ListMedicineTypeGrid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click btn_Radio_Enable_Click { get; set; }
        public gridViewMety_MouseDownMety gridViewMety_MouseDownMety { get; set; }

        public Grid_RowCellClick ListDepositReqGrid_RowCellClick { get; set; }
        public ReloadRowChoose ReloadRowChoose_Reload { get; set; }

        public GridView_MouseRightClick GridView_MouseRightClick { get; set; }

        public ProcessUpdateTrustAmount processUpdateTrustAmount { get; set; }
    }
}
