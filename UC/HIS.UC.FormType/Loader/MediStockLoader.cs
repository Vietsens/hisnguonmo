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
    class MediStockLoader
    {
        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboMediStock)
        {
            try
            {
                cboMediStock.Properties.DataSource = Config.HisFormTypeConfig.VHisMediStock;
                cboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMediStock.Properties.ValueMember = "ID";

                cboMediStock.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboMediStock.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboMediStock.Properties.ImmediatePopup = true;
                cboMediStock.ForceInitialize();
                cboMediStock.Properties.View.Columns.Clear();
                cboMediStock.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboMediStock.Properties.View.Columns.AddField("MEDI_STOCK_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;
              
                GridColumn aColumnName = cboMediStock.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
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

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboMediStock, List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK> listData)
        {
            try
            {
                cboMediStock.Properties.DataSource = listData;
                cboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMediStock.Properties.ValueMember = "ID";

                cboMediStock.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboMediStock.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboMediStock.Properties.ImmediatePopup = true;
                cboMediStock.ForceInitialize();
                cboMediStock.Properties.View.Columns.Clear();
                cboMediStock.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboMediStock.Properties.View.Columns.AddField("MEDI_STOCK_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboMediStock.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
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

        internal static void LoadDataToComboRoom(DevExpress.XtraEditors.LookUpEdit cboMediStock)
        {
            try
            {
                cboMediStock.Properties.DataSource = Config.HisFormTypeConfig.VHisMediStock;
                cboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMediStock.Properties.ValueMember = "ID";
                cboMediStock.Properties.ForceInitialize();
                cboMediStock.Properties.Columns.Clear();
                cboMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 100));
                cboMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                cboMediStock.Properties.ShowHeader = false;
                cboMediStock.Properties.ImmediatePopup = true;
                cboMediStock.Properties.DropDownRows = 10;
                cboMediStock.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToComboRoom(DevExpress.XtraEditors.LookUpEdit cboMediStock, List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK> listData)
        {
            try
            {
                cboMediStock.Properties.DataSource = listData;
                cboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMediStock.Properties.ValueMember = "ID";
                cboMediStock.Properties.ForceInitialize();
                cboMediStock.Properties.Columns.Clear();
                cboMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 100));
                cboMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                cboMediStock.Properties.ShowHeader = false;
                cboMediStock.Properties.ImmediatePopup = true;
                cboMediStock.Properties.DropDownRows = 10;
                cboMediStock.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
