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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HIS.Desktop.Plugins.TransDepartment.Loader
{
    public class DepartmentLoader
    {
        public static void LoadDataToComboDepartment(DevExpress.XtraEditors.LookUpEdit cboDepartment)
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_CLINICAL == 1).ToList();
                data.OrderByDescending(o => o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId());
                cboDepartment.Properties.DataSource = data;            
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

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
						
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

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboServiceType, object data)
        {
            try
            {
                cboServiceType.Properties.DataSource = data;
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
                aColumnCode.Visible = false;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("DEPARTMENT_NAME");
                aColumnName.Caption = "Tên khoa";
                aColumnName.Visible = false;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDepartmentCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboDepartment, DevExpress.XtraEditors.TextEdit txtDepartment, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDepartment.EditValue = null;
                    cboDepartment.Focus();
                    cboDepartment.ShowPopup();
                }
                else
                {
                    var data =BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.DEPARTMENT_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboDepartment.EditValue = data[0].ID;
                            txtDepartment.Text = data[0].DEPARTMENT_CODE;
                            focusControl.Focus();
                            focusControl.SelectAll();
                            cboDepartment.Properties.Buttons[1].Visible = true;
                        }
                        else if (data.Count > 1)
                        {
                            cboDepartment.EditValue = null;
                            cboDepartment.Focus();
                            cboDepartment.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
