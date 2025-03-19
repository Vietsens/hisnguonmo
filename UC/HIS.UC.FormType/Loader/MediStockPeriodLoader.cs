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
    class MediStockPeriodLoader
    {
        public static void LoadDataToComboMediStockPeriod(DevExpress.XtraEditors.LookUpEdit cboMediStockPeriod)
        {
            try
            {
                cboMediStockPeriod.Properties.DataSource = Config.HisFormTypeConfig.VHisMediStockPeriod;
                cboMediStockPeriod.Properties.DisplayMember = "MEDI_STOCK_PERIOD_NAME";
                cboMediStockPeriod.Properties.ValueMember = "ID";
                cboMediStockPeriod.Properties.ForceInitialize();
                cboMediStockPeriod.Properties.Columns.Clear();
                cboMediStockPeriod.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_PERIOD_CODE", "", 100));
                cboMediStockPeriod.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_PERIOD_NAME", "", 200));
                cboMediStockPeriod.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                cboMediStockPeriod.Properties.ShowHeader = false;
                cboMediStockPeriod.Properties.ImmediatePopup = true;
                cboMediStockPeriod.Properties.DropDownRows = 10;
                cboMediStockPeriod.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                var data = Config.HisFormTypeConfig.VHisMediStockPeriod.Where(o => Config.HisFormTypeConfig.VHisMediStock.Select(s => s.ID).Contains(o.MEDI_STOCK_ID)).ToList();
                cboServiceType.Properties.DataSource = data;
                cboServiceType.Properties.DisplayMember = "MEDI_STOCK_PERIOD_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("MEDI_STOCK_PERIOD_CODE");
                aColumnCode.Caption = "Mã kỳ ";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;
                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("MEDI_STOCK_PERIOD_NAME");
                aColumnName.Caption = "Tên kỳ";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;

                GridColumn aColumnName1 = cboServiceType.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
                aColumnName1.Caption = "Tên kho";
                aColumnName1.Visible = true;
                aColumnName1.VisibleIndex = 3;
                aColumnName1.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
       
    }
}
