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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Loader
{
    class KskContractLoader
    {
        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboKskContract)
        {
            try
            {
                cboKskContract.Properties.DataSource = Config.HisFormTypeConfig.HisKskContracts;
                cboKskContract.Properties.DisplayMember = "CUSTOMER_NAME";
                cboKskContract.Properties.ValueMember = "ID";

                cboKskContract.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboKskContract.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboKskContract.Properties.ImmediatePopup = true;
                cboKskContract.ForceInitialize();
                cboKskContract.Properties.View.Columns.Clear();
                cboKskContract.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboKskContract.Properties.View.Columns.AddField("KSK_CONTRACT_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboKskContract.Properties.View.Columns.AddField("CUSTOMER_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboKskContract, List<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT> listData)
        {
            try
            {
                cboKskContract.Properties.DataSource = listData;
                cboKskContract.Properties.DisplayMember = "CUSTOMER_NAME";
                cboKskContract.Properties.ValueMember = "ID";

                cboKskContract.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboKskContract.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboKskContract.Properties.ImmediatePopup = true;
                cboKskContract.ForceInitialize();
                cboKskContract.Properties.View.Columns.Clear();
                cboKskContract.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboKskContract.Properties.View.Columns.AddField("KSK_CONTRACT_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboKskContract.Properties.View.Columns.AddField("CUSTOMER_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToComboKskContract(DevExpress.XtraEditors.LookUpEdit cboKskContract)
        {
            try
            {
                cboKskContract.Properties.DataSource = Config.HisFormTypeConfig.HisKskContracts;
                cboKskContract.Properties.DisplayMember = "CUSTOMER_NAME";
                cboKskContract.Properties.ValueMember = "ID";
                cboKskContract.Properties.ForceInitialize();
                cboKskContract.Properties.Columns.Clear();
                cboKskContract.Properties.Columns.Add(new LookUpColumnInfo("KSK_CONTRACT_CODE", "", 100));
                cboKskContract.Properties.Columns.Add(new LookUpColumnInfo("CUSTOMER_NAME", "", 200));
                cboKskContract.Properties.ShowHeader = false;
                cboKskContract.Properties.ImmediatePopup = true;
                cboKskContract.Properties.DropDownRows = 10;
                cboKskContract.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToComboKskContract(DevExpress.XtraEditors.LookUpEdit cboKskContract, List<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT> listData)
        {
            try
            {
                cboKskContract.Properties.DataSource = listData;
                cboKskContract.Properties.DisplayMember = "CUSTOMER_NAME";
                cboKskContract.Properties.ValueMember = "ID";
                cboKskContract.Properties.ForceInitialize();
                cboKskContract.Properties.Columns.Clear();
                cboKskContract.Properties.Columns.Add(new LookUpColumnInfo("KSK_CONTRACT_CODE", "", 100));
                cboKskContract.Properties.Columns.Add(new LookUpColumnInfo("CUSTOMER_NAME", "", 200));
                cboKskContract.Properties.ShowHeader = false;
                cboKskContract.Properties.ImmediatePopup = true;
                cboKskContract.Properties.DropDownRows = 10;
                cboKskContract.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
