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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.DepositRequestList.ADO
{
     
    public class DepositRequestInitADO
    {
        public List<MOS.EFMODEL.DataModels.V_HIS_DEPOSIT_REQ> listVDepositReq { get; set; }
        public MOS.EFMODEL.DataModels.V_HIS_DEPOSIT_REQ V_DepositReq { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_DEPOSIT_REQ> listDepositReq { get; set; }
        public MOS.EFMODEL.DataModels.HIS_DEPOSIT_REQ DepositReq { get; set; }
        public MOS.EFMODEL.DataModels.V_HIS_TREATMENT V_Treatment { get; set; }
        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public bool? IsAutoWidth { get; set; }
        public long treatmentID { get; set; }
        public bool? IsCreateParentNodeWithSereServExpend { get; set; }

        public string LayoutSereServExpend { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
        public string Keyword_NullValuePrompt { get; set; }

        public gridViewDeposit_CustomUnboundColumnData gridViewDeposit_CustomUnboundColumnData { get; set; }
        public gridviewHandler btnDelete_Click { get; set; }
        public gridviewHandler btnPrint_Click { get; set; }
        public gridViewDeposit_CustomRowCellEdit gridViewDeposit_CustomRowCellEdit { get; set; }
        public gridViewDeposit_CustomDrawCell gridViewDeposit_CustomDrawCell { get; set; }
        public gridviewHandler UpdateSingleRow { get; set; }

    }
}
