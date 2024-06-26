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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.CallPatientTypeAlter;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter.Loader
{
    public class PatientTypeLoader
    {
        public static void LoadDataToCombo(DevExpress.XtraEditors.LookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> listData)
        {
            try
            {
                cboPatientType.Properties.BeginUpdate();
                cboPatientType.Properties.DataSource = listData;
                cboPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboPatientType.Properties.ValueMember = "ID";
                cboPatientType.Properties.ForceInitialize();
                cboPatientType.Properties.Columns.Clear();
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_CODE", "", 100));
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_NAME", "", 200));
                cboPatientType.Properties.ShowHeader = false;
                cboPatientType.Properties.ImmediatePopup = true;
                cboPatientType.Properties.DropDownRows = 10;
                cboPatientType.Properties.PopupWidth = 300;
                cboPatientType.Properties.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                cboPatientType.Properties.DataSource = data;
                cboPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboPatientType.Properties.ValueMember = "ID";

                cboPatientType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboPatientType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboPatientType.Properties.ImmediatePopup = true;
                cboPatientType.ForceInitialize();
                cboPatientType.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboPatientType.Properties.View.Columns.AddField("PATIENT_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboPatientType.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
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

        public static void LoadDataToCombo(DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> listData)
        {
            try
            {
                cboPatientType.Properties.BeginUpdate();
                cboPatientType.Properties.DataSource = listData;
                cboPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboPatientType.Properties.ValueMember = "ID";
                cboPatientType.Properties.ForceInitialize();
                cboPatientType.Properties.Columns.Clear();
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_CODE", "", 100));
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_NAME", "", 200));
                cboPatientType.Properties.ShowHeader = false;
                cboPatientType.Properties.ImmediatePopup = true;
                cboPatientType.Properties.DropDownRows = 10;
                cboPatientType.Properties.PopupWidth = 300;
                cboPatientType.Properties.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboForNameValue(DevExpress.XtraEditors.LookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> listData)
        {
            try
            {
                cboPatientType.Properties.BeginUpdate();
                cboPatientType.Properties.DataSource = listData;
                cboPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboPatientType.Properties.ValueMember = "PATIENT_TYPE_NAME";
                cboPatientType.Properties.ForceInitialize();
                cboPatientType.Properties.Columns.Clear();
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_CODE", "", 100));
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_NAME", "", 200));
                cboPatientType.Properties.ShowHeader = false;
                cboPatientType.Properties.ImmediatePopup = true;
                cboPatientType.Properties.DropDownRows = 10;
                cboPatientType.Properties.PopupWidth = 300;
                cboPatientType.Properties.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboForNameValue(DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> listData)
        {
            try
            {
                cboPatientType.Properties.BeginUpdate();
                cboPatientType.Properties.DataSource = listData;
                cboPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboPatientType.Properties.ValueMember = "PATIENT_TYPE_NAME";
                cboPatientType.Properties.ForceInitialize();
                cboPatientType.Properties.Columns.Clear();
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_CODE", "", 100));
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_NAME", "", 200));
                cboPatientType.Properties.ShowHeader = false;
                cboPatientType.Properties.ImmediatePopup = true;
                cboPatientType.Properties.DropDownRows = 10;
                cboPatientType.Properties.PopupWidth = 300;
                cboPatientType.Properties.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToPatientTypeRepositoryItemCombo(DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repositoryItemcboPatientType, object data)
        {
            try
            {
                repositoryItemcboPatientType.DataSource = data;
                repositoryItemcboPatientType.DisplayMember = "PATIENT_TYPE_NAME";
                repositoryItemcboPatientType.ValueMember = "PATIENT_TYPE_CODE";
                repositoryItemcboPatientType.ForceInitialize();
                repositoryItemcboPatientType.Columns.Clear();
                repositoryItemcboPatientType.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_CODE", "", 100));
                repositoryItemcboPatientType.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_NAME", "", 200));
                repositoryItemcboPatientType.ShowHeader = false;
                repositoryItemcboPatientType.ImmediatePopup = true;
                repositoryItemcboPatientType.DropDownRows = 10;
                repositoryItemcboPatientType.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
    }
}
