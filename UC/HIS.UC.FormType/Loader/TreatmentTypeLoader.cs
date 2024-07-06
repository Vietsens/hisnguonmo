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
    class TreatmentTypeLoader
    {
        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboTreatmentType)
        {
            try
            {
                cboTreatmentType.Properties.DataSource = Config.HisFormTypeConfig.HisTreatmentTypes;
                cboTreatmentType.Properties.DisplayMember = "TREATMENT_TYPE_NAME";
                cboTreatmentType.Properties.ValueMember = "ID";

                cboTreatmentType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboTreatmentType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboTreatmentType.Properties.ImmediatePopup = true;
                cboTreatmentType.ForceInitialize();
                cboTreatmentType.Properties.View.Columns.Clear();
                cboTreatmentType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_NAME");
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

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboTreatmentType, List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> listData)
        {
            try
            {
                cboTreatmentType.Properties.DataSource = listData;
                cboTreatmentType.Properties.DisplayMember = "TREATMENT_TYPE_NAME";
                cboTreatmentType.Properties.ValueMember = "ID";

                cboTreatmentType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboTreatmentType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboTreatmentType.Properties.ImmediatePopup = true;
                cboTreatmentType.ForceInitialize();
                cboTreatmentType.Properties.View.Columns.Clear();
                cboTreatmentType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_NAME");
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

        internal static void LoadDataToComboTreatmentType(DevExpress.XtraEditors.LookUpEdit cboTreatmentType)
        {
            try
            {
                cboTreatmentType.Properties.DataSource = Config.HisFormTypeConfig.HisTreatmentTypes;
                cboTreatmentType.Properties.DisplayMember = "TreatmentType_NAME";
                cboTreatmentType.Properties.ValueMember = "ID";
                cboTreatmentType.Properties.ForceInitialize();
                cboTreatmentType.Properties.Columns.Clear();
                cboTreatmentType.Properties.Columns.Add(new LookUpColumnInfo("TREATMENT_TYPE_CODE", "", 100));
                cboTreatmentType.Properties.Columns.Add(new LookUpColumnInfo("TREATMENT_TYPE_NAME", "", 200));
                cboTreatmentType.Properties.ShowHeader = false;
                cboTreatmentType.Properties.ImmediatePopup = true;
                cboTreatmentType.Properties.DropDownRows = 10;
                cboTreatmentType.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToComboTreatmentType(DevExpress.XtraEditors.LookUpEdit cboTreatmentType, List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> listData)
        {
            try
            {
                cboTreatmentType.Properties.DataSource = listData;
                cboTreatmentType.Properties.DisplayMember = "TREATMENT_TYPE_NAME";
                cboTreatmentType.Properties.ValueMember = "ID";
                cboTreatmentType.Properties.ForceInitialize();
                cboTreatmentType.Properties.Columns.Clear();
                cboTreatmentType.Properties.Columns.Add(new LookUpColumnInfo("TREATMENT_TYPE_CODE", "", 100));
                cboTreatmentType.Properties.Columns.Add(new LookUpColumnInfo("TREATMENT_TYPE_NAME", "", 200));
                cboTreatmentType.Properties.ShowHeader = false;
                cboTreatmentType.Properties.ImmediatePopup = true;
                cboTreatmentType.Properties.DropDownRows = 10;
                cboTreatmentType.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
