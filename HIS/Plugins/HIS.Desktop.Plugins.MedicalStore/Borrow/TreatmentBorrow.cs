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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MedicalStore.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicalStore.Borrow
{
    public partial class TreatmentBorrow : HIS.Desktop.Utility.FormBase
    {
        int positionHandle = -1;
        TreatmentADO currentAdo;
        RefeshReference refeshData;

        public TreatmentBorrow(TreatmentADO ado, RefeshReference refeshData)
        {
            InitializeComponent();

            try
            {
                this.currentAdo = ado;
                this.refeshData = refeshData;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                if (btnSave.Enabled)
                    btnSave_Click(null, null);
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
                positionHandle = -1;
                bool success = false;
                CommonParam param = new CommonParam();

                if (!dxValidationProvider1.Validate())
                    return;

                HIS_TREATMENT_BORROW treatmentBorrow = new HIS_TREATMENT_BORROW();
                if (cboDepartment.EditValue != null)
                {
                    treatmentBorrow.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString());
                }

                if (cboTaiKhoan.EditValue != null && !string.IsNullOrEmpty(cboTaiKhoan.EditValue.ToString()))
                {
                    treatmentBorrow.BORROW_LOGINNAME = cboTaiKhoan.EditValue.ToString();
                    treatmentBorrow.BORROW_USERNAME = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == treatmentBorrow.BORROW_LOGINNAME).USERNAME;
                }

                treatmentBorrow.TREATMENT_ID = this.currentAdo.ID;

                if (dtThoiGianHenTra.EditValue != null)
                {
                    treatmentBorrow.APPOINTMENT_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtThoiGianHenTra.DateTime);
                }
                else
                    treatmentBorrow.APPOINTMENT_TIME = null;

                WaitingManager.Show();
                var rsApi = new BackendAdapter(param).Post<HIS_TREATMENT_BORROW>("api/HisTreatmentBorrow/Create", ApiConsumer.ApiConsumers.MosConsumer, treatmentBorrow, param);
                WaitingManager.Hide();

                if (rsApi != null)
                {
                    success = true;
                    if (this.refeshData != null)
                    {
                        this.refeshData();
                    }
                    this.Close();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void TreatmentBorrow_Load(object sender, EventArgs e)
        {

            try
            {
                ValidateForm();
                InitComboDepartment();
                InitComboTaiKhoan();

                dtGiveTime.DateTime = DateTime.Now;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidateForm()
        {
            try
            {
                ValidateGridLookupWithTextEdit(cboDepartment, txtDepartmentCode);
                ValidateThoiGianHenTra();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateThoiGianHenTra()
        {
            try
            {
                ValidateThoiGianHenTra validRule = new ValidateThoiGianHenTra();
                validRule.dtTime = dtThoiGianHenTra;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtThoiGianHenTra, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDepartment()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTaiKhoan()
        {

            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 350);
                ControlEditorLoader.Load(cboTaiKhoan, BackendDataWorker.Get<ACS_USER>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDepartmentCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtDepartmentCode.Text))
                    {
                        cboDepartment.EditValue = null;
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                    else
                    {
                        List<HIS_DEPARTMENT> searchs = null;
                        var listData1 = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.DEPARTMENT_CODE.ToUpper().Contains(txtDepartmentCode.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.DEPARTMENT_CODE.ToUpper() == txtDepartmentCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtDepartmentCode.Text = searchs[0].DEPARTMENT_CODE;
                            cboDepartment.EditValue = searchs[0].ID;
                            txtTaiKhoan.Focus();
                            txtTaiKhoan.SelectAll();
                        }
                        else
                        {
                            cboDepartment.Focus();
                            cboDepartment.ShowPopup();
                        }
                    }
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
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_DEPARTMENT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                            txtDepartmentCode.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTaiKhoan_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtTaiKhoan.Text))
                    {
                        cboTaiKhoan.EditValue = null;
                        cboTaiKhoan.Focus();
                        cboTaiKhoan.ShowPopup();
                    }
                    else
                    {
                        List<ACS_USER> searchs = null;
                        var listData1 = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME.ToUpper().Contains(txtTaiKhoan.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.LOGINNAME.ToUpper() == txtTaiKhoan.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtTaiKhoan.Text = searchs[0].LOGINNAME;
                            cboTaiKhoan.EditValue = searchs[0].LOGINNAME;
                            dtThoiGianHenTra.Focus();
                            dtThoiGianHenTra.ShowPopup();
                        }
                        else
                        {
                            cboTaiKhoan.Focus();
                            cboTaiKhoan.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTaiKhoan_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTaiKhoan.EditValue != null && !string.IsNullOrEmpty(cboTaiKhoan.EditValue.ToString()))
                    {
                        var data = BackendDataWorker.Get<ACS_USER>().SingleOrDefault(o => o.LOGINNAME == (cboTaiKhoan.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            txtTaiKhoan.Text = data.LOGINNAME;
                            txtTaiKhoan.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtThoiGianHenTra_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtGiveTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDepartmentCode.Focus();
                    txtDepartmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtGiveTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtGiveTime.EditValue != null)
                {
                    var giveTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtGiveTime.DateTime);
                    double overTime = HisConfigs.Get<double>("HIS.Desktop.Plugins.MedicalStore.TreatmentBorrow.OverAppointment");

                    var addDay = Inventec.Common.DateTime.Calculation.Add(giveTime ?? 0, overTime, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY);

                    dtThoiGianHenTra.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(addDay ?? 0) ?? DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
