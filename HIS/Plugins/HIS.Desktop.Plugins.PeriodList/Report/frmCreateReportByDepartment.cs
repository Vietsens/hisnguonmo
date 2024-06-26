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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.PeriodList.MrsFilter;
using HIS.Desktop.Plugins.PeriodList.Validation;
using HTC.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PeriodList.Report
{
    public partial class frmCreateReportByDepartment : HIS.Desktop.Utility.FormBase
    {
        HTC_PERIOD period = null;
        string TypeCode = "";
        private int positionHandleControl = -1;

        public frmCreateReportByDepartment(HTC_PERIOD data, string reportTypeCode, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.period = data;
                this.TypeCode = reportTypeCode;
                if (this.TypeCode == "MRS00491")
                {
                    this.Text = this.Text + " thực hiện";
                }
                else
                {
                    this.Text = this.Text + " chỉ định";
                }
                this.SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmCreateReportByDepartment_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.period != null && !String.IsNullOrEmpty(this.TypeCode))
                {
                    this.LoadDataToCboDepartment();
                    this.VaidControl();
                    lblPeriod.Text = period.PERIOD_CODE + " - " + period.PERIOD_NAME;
                    if (this.TypeCode == "MRS00491")
                    {
                        this.lciDepartment.Text = "Khoa thực hiện:";
                    }
                    else if (this.TypeCode == "MRS00492")
                    {
                        this.lciDepartment.Text = "Khoa chỉ định:";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadDataToCboDepartment()
        {
            try
            {
                cboDepartment.Properties.DataSource = BackendDataWorker.Get<HIS_DEPARTMENT>().OrderByDescending(o => o.DEPARTMENT_CODE).ToList();
                cboDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboDepartment.Properties.ValueMember = "ID";
                cboDepartment.Properties.ForceInitialize();
                cboDepartment.Properties.Columns.Clear();
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_CODE", "Mã", 50));
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_NAME", "Tên", 200));
                cboDepartment.Properties.ShowHeader = true;
                cboDepartment.Properties.ImmediatePopup = true;
                cboDepartment.Properties.DropDownRows = 10;
                cboDepartment.Properties.PopupWidth = 250;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void VaidControl()
        {
            try
            {
                ValidControDepartment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidControDepartment()
        {
            try
            {
                DepartmentValidationRule departmentRule = new DepartmentValidationRule();
                departmentRule.txtDepartmentCode = txtDepartmentCode;
                departmentRule.cboDepartment = cboDepartment;
                dxValidationProvider1.SetValidationRule(txtDepartmentCode, departmentRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtDepartmentCode.Text))
                    {
                        string key = txtDepartmentCode.Text.ToLower().Trim();
                        var listData = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.DEPARTMENT_CODE.ToLower().Contains(key) || o.DEPARTMENT_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            cboDepartment.EditValue = listData[0].ID;
                            valid = true;
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{TAB}");
                        }
                    }
                    if (!valid)
                    {
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtDepartmentCode.Text = "";
                if (cboDepartment.EditValue != null)
                {
                    var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepartment.EditValue));
                    if (department != null)
                    {
                        txtDepartmentCode.Text = department.DEPARTMENT_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate()) return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                MRS.SDO.CreateReportSDO sdo = new MRS.SDO.CreateReportSDO();
                sdo.ReportTemplateCode = TypeCode + "01";
                sdo.ReportTypeCode = TypeCode;
                MrsDepartmentFilter filter = new MrsDepartmentFilter();
                filter.PERIOD_ID = period.ID;
                filter.DEPARTMENT_ID = Convert.ToInt64(cboDepartment.EditValue);
                sdo.Filter = filter;
                success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/MrsReport/Create", ApiConsumers.MrsConsumer, sdo, param);
                WaitingManager.Hide();
                MessageManager.Show(param, success);
                if (success)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
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
