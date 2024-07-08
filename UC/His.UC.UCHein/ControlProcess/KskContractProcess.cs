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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.ControlProcess
{
    public class KskContractProcess
    {
        public static void LoadDataToComboKskContract(DevExpress.XtraEditors.LookUpEdit cboKskContract, List<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT> data)
        {
            try
            {
                cboKskContract.Properties.DataSource = data;
                cboKskContract.Properties.DisplayMember = "CUSTOMER_NAME";
                cboKskContract.Properties.ValueMember = "ID";
                cboKskContract.Properties.ForceInitialize();
                cboKskContract.Properties.Columns.Clear();
                cboKskContract.Properties.Columns.Add(new LookUpColumnInfo("KSK_CONTRACT_CODE", "Mã", 100));
                cboKskContract.Properties.Columns.Add(new LookUpColumnInfo("CUSTOMER_NAME", "Tên", 200));
                cboKskContract.Properties.Columns.Add(new LookUpColumnInfo("RENDERER_RATIO", "Tỉ lệ đóng chi trả", 150));
                cboKskContract.Properties.Columns.Add(new LookUpColumnInfo("RENDERER_MAX_FEE", "Trần viện phí", 100));
                cboKskContract.Properties.ShowHeader = true;
                cboKskContract.Properties.ImmediatePopup = true;
                cboKskContract.Properties.DropDownRows = 10;
                cboKskContract.Properties.PopupWidth = 550;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
