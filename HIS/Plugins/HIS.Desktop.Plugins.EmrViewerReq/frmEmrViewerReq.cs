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
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.EmrViewerReq.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
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

namespace HIS.Desktop.Plugins.EmrViewerReq
{
    public partial class frmEmrViewerReq : FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long _TreatmentId = 0;
        List<HIS_DEPARTMENT> listDepartments = new List<HIS_DEPARTMENT>();

        int positionHandle = -1;

        public frmEmrViewerReq()
        {
            InitializeComponent();
        }

        public frmEmrViewerReq(Inventec.Desktop.Common.Modules.Module module, long _treatmentId)
            : base(module)
        {

            InitializeComponent();
            currentModule = module;
            this._TreatmentId = _treatmentId;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                if (currentModule != null)
                {
                    this.Text = currentModule.text;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmEmrViewerReq_Load(object sender, EventArgs e)
        {
            try
            {
                InitComboDepartment();
                ValidateBedForm();
                ValidateReason(mmReason, 2000);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void InitComboDepartment()
        {
            try
            {
                listDepartments = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "DEPARTMENT_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDepartment, listDepartments, controlEditorADO);
                cboDepartment.Properties.ImmediatePopup = true;
                var department = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == currentModule.RoomId);
                if (department != null)
                {
                    txtDepartmentCode.Text = department.DEPARTMENT_CODE;
                    cboDepartment.EditValue = department.DEPARTMENT_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this._TreatmentId > 0)
                {

                    this.positionHandle = -1;
                    if (!dxValidationProvider1.Validate())
                        return;

                    EMR.Filter.EmrTreatmentFilter treatmentFilter = new EMR.Filter.EmrTreatmentFilter();
                    treatmentFilter.ID = this._TreatmentId;
                    var rsTreatment = new BackendAdapter(new CommonParam()).Get<List<EMR_TREATMENT>>(EMR.URI.EmrTreatment.GET, ApiConsumers.EmrConsumer, treatmentFilter, null);
                    if (rsTreatment != null && rsTreatment.Count > 0)
                    {
                        DateTime reqTime = dtTime.DateTime;
                        DateTime inTimeTreatment = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rsTreatment[0].IN_DATE) ?? DateTime.Now;
                        if (reqTime < inTimeTreatment)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Ngày yêu cầu xem chi tiết bệnh án không được nhỏ hơn ngày nhập viện", "Thông báo");
                            dtTime.Focus();
                            dtTime.SelectAll();
                            return;
                        }
                    }


                    bool success = false;
                    CommonParam param = new CommonParam();

                    EMR.EFMODEL.DataModels.EMR_VIEWER ado = new EMR.EFMODEL.DataModels.EMR_VIEWER();
                    ado.TREATMENT_ID = this._TreatmentId;
                    ado.REQUEST_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    DateTime? finishTime = dtTime.DateTime;
                    if (finishTime != null && finishTime.Value != DateTime.MinValue)
                    {
                        ado.REQUEST_FINISH_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(finishTime.Value.ToString("yyyyMMdd") + "235959");
                    }
                    if (cboDepartment.EditValue != null)
                    {
                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.DEPARTMENT_CODE == cboDepartment.EditValue.ToString());
                        if (room != null)
                        {
                            //thaovtb yeu cau/ Ngay 20/12/2018
                            ado.DEPARTMENT_CODE = room.DEPARTMENT_CODE;
                            ado.DEPARTMENT_NAME = room.DEPARTMENT_NAME;
                        }
                    }
                    else
                        return;
                    ado.REASON = mmReason.Text.Trim();
                    var outPut = new BackendAdapter(param).Post<EMR_VIEWER>(EMR.URI.EmrViewer.CREATE, ApiConsumer.ApiConsumers.EmrConsumer, ado, param);

                    if (outPut != null)
                    {
                        success = true;
                        this.Close();
                    }

                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateBedForm()
        {
            try
            {
                DateFromValidationRule dateValidRule = new DateFromValidationRule();
                dateValidRule.dt = this.dtTime;
                dateValidRule.ErrorText = "Trường dữ liệu bắt buộc";
                dateValidRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.dtTime, dateValidRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidateDepartment()
        {
            try
            {
                ValidationDepartment valid = new ValidationDepartment();
                valid.textEdit = txtDepartmentCode;
                valid.cbo = cboDepartment;
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.txtDepartmentCode, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidateReason(BaseControl control, int maxlength)
        {
            try
            {
                ValidationMaxLength valid = new ValidationMaxLength();
                valid.textEdit = control;
                valid.maxLength = maxlength;
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.mmReason, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadComboDepartment(txtDepartmentCode.Text.ToUpper(), txtDepartmentCode, cboDepartment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadComboDepartment(string searchCode, TextEdit txtDepartmentCode, GridLookUpEdit cbo)
        {
            try
            {
                bool showCbo = true;
                if (String.IsNullOrEmpty(searchCode))
                {
                    txtDepartmentCode.Text = "";
                    cbo.EditValue = null;
                }
                else
                {
                    var listData = this.listDepartments.Where(o => o.DEPARTMENT_CODE == searchCode).ToList();
                    if (listData != null && listData.Count == 1)
                    {
                        showCbo = false;
                        txtDepartmentCode.Text = listData.First().DEPARTMENT_CODE;
                        cbo.EditValue = txtDepartmentCode.Text;
                        mmReason.Focus();
                    }
                    else
                    {
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.DEPARTMENT_CODE == searchCode).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = true;
                            txtDepartmentCode.Text = result.First().DEPARTMENT_CODE;
                            cbo.Focus();
                            cbo.SelectAll();
                        }
                    }
                }

                if (showCbo)
                {
                    cbo.Focus();
                    cbo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        var department = listDepartments.FirstOrDefault(o => o.DEPARTMENT_CODE == cboDepartment.EditValue.ToString());
                        if (department != null)
                        {
                            txtDepartmentCode.Text = department.DEPARTMENT_CODE;
                        }
                    }
                    else
                    {
                        txtDepartmentCode.Focus();
                        txtDepartmentCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboDepartment.ClosePopup();
                    cboDepartment.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboDepartment.ClosePopup();
                    if (cboDepartment.EditValue != null)
                    {
                        var department = listDepartments.FirstOrDefault(o => o.DEPARTMENT_CODE == cboDepartment.EditValue.ToString());
                        if (department != null)
                        {
                            txtDepartmentCode.Text = department.DEPARTMENT_CODE;
                        }
                    }
                    else
                        cboDepartment.ShowPopup();
                }
                else
                    cboDepartment.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
