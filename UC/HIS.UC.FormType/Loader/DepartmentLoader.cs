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
    public class DepartmentLoader
    {
        public static void LoadDataToComboDepartment(DevExpress.XtraEditors.LookUpEdit cboDepartment)
        {
            try
            {
                cboDepartment.Properties.DataSource = Config.HisFormTypeConfig.HisDepartments;
                cboDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboDepartment.Properties.ValueMember = "ID";
                cboDepartment.Properties.ForceInitialize();
                cboDepartment.Properties.Columns.Clear();
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_CODE", "", 100));
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_NAME", "", 200));
                cboDepartment.Properties.ShowHeader = false;
                cboDepartment.Properties.ImmediatePopup = true;
                cboDepartment.Properties.DropDownRows = 10;
                cboDepartment.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToCombo0(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.VHisRooms;
                cboServiceType.Properties.DisplayMember = "ROOM_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("ROOM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("ROOM_NAME");
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

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.HisDepartments;
                cboServiceType.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("DEPARTMENT_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("DEPARTMENT_NAME");
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

        public static void LoadDataToCombo1(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.HisGenders;
                cboServiceType.Properties.DisplayMember = "GENDER_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("GENDER_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("GENDER_NAME");
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

        public static void LoadDataToCombo2(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.VHisIcdGroups;
                cboServiceType.Properties.DisplayMember = "ICD_GROUP_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("ICD_GROUP_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("ICD_GROUP_NAME");
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

        public static void LoadDataToCombo2_1(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.VHisServiceGroups;
                cboServiceType.Properties.DisplayMember = "SERVICE_GROUP_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("SERVICE_GROUP_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("SERVICE_GROUP_NAME");
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

        public static void LoadDataToCombo3(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.HisExpMestTypes;
                cboServiceType.Properties.DisplayMember = "EXP_MEST_TYPE_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("EXP_MEST_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("EXP_MEST_TYPE_NAME");
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

        public static void LoadDataToCombo3_1(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.HisCashierRooms;
                cboServiceType.Properties.DisplayMember = "CASHIER_ROOM_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("CASHIER_ROOM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("CASHIER_ROOM_NAME");
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

        public static void LoadDataToCombo5(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.HisTreatmentTypes;
                cboServiceType.Properties.DisplayMember = "TREATMENT_TYPE_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("TREATMENT_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("TREATMENT_TYPE_NAME");
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

        public static void LoadDataToCombo6(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.HisPatientTypes;
                cboServiceType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("PATIENT_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
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

        public static void LoadDataToCombo4(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.HisExpMestStts;
                cboServiceType.Properties.DisplayMember = "EXP_MEST_STT_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("EXP_MEST_STT_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("EXP_MEST_STT_NAME");
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

        public static void LoadDataToCombo7(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                var listService = Config.HisFormTypeConfig.VHisServices;

                cboServiceType.Properties.DataSource = listService;
                cboServiceType.Properties.DisplayMember = "SERVICE_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("SERVICE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("SERVICE_NAME");
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

        public static void LoadDataToCombo8(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.VHisBids;
                cboServiceType.Properties.DisplayMember = "BID_NUMBER";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("BID_NUMBER");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("BID_NUMBER");
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

        public static void LoadDataToCombo9(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.HisTreatments;
                cboServiceType.Properties.DisplayMember = ".VIR_PATIENT_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("TREATMENT_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("VIR_PATIENT_NAME");
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

        public static void LoadDataToCombo10(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.HisTreatmentEndTypes;
                cboServiceType.Properties.DisplayMember = "TREATMENT_END_TYPE_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("TREATMENT_END_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("TREATMENT_END_TYPE_NAME");
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

        public static void LoadDataToCombo11(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.HisKskContracts;
                cboServiceType.Properties.DisplayMember = "CUSTOMER_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("KSK_CONTRACT_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("CUSTOMER_NAME");
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

        public static void LoadDataToCombo11_1(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.HisBranchs;
                cboServiceType.Properties.DisplayMember = "BRANCH_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("BRANCH_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("BRANCH_NAME");
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

    }
}
