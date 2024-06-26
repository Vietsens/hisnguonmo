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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.ContactDeclaration.ADO;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using Inventec.Core;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;

namespace HIS.Desktop.Plugins.ContactDeclaration.UcObject
{
    public partial class UcStaff : UserControl
    {
        public UpdateVContactPoint updateVContactPoint;
        List<HisEmployeeADO> lstEmployeeADO { set; get; }
        HisEmployeeADO EmployeeADO = new HisEmployeeADO();
        V_HIS_CONTACT_POINT CurrentContactPoint;
        int positionHandle = -1;

        public UcStaff() { }

        public UcStaff(List<HisEmployeeADO> EmployeeADOs, UpdateVContactPoint _updateVContactPoint)
        {
            this.lstEmployeeADO = EmployeeADOs;
            this.updateVContactPoint = _updateVContactPoint;
            InitializeComponent();
        }

        private void UcStaff_Load(object sender, EventArgs e)
        {
            LoadComboboxStaff(this.lstEmployeeADO);

            ValidateUcStaff();
        }

        public bool ValidateForm()
        {
            positionHandle = -1;
            return this.dxValidationProvider1.Validate();
        }

        public void ValidateUcStaff()
        {
            try
            {
                ValidateGridLookupWithTextEdit(dxValidationProvider1, cboStaff, txtStaff);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(DXValidationProvider validate, GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                validate.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadComboboxStaff(List<HisEmployeeADO> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboStaff, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtStaff_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtStaff.Text) && lstEmployeeADO != null && lstEmployeeADO.Count > 0)
                    {
                        bool showCbo = true;
                        if (!String.IsNullOrEmpty(txtStaff.Text.Trim()))
                        {
                            string code = txtStaff.Text.Trim();
                            var listData = lstEmployeeADO.Where(o => o.LOGINNAME.Contains(code)).ToList();
                            var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.LOGINNAME.ToUpper() == code.ToUpper()).ToList() : listData) : null;
                            if (result != null && result.Count > 0)
                            {
                                showCbo = false;
                                EmployeeADO = result.First();
                                cboStaff.EditValue = result.First().LOGINNAME;
                                lblDepartmentStaff.Text = result.First().DEPARTMENT_NAME;
                            }
                        }

                        if (showCbo)
                        {
                            cboStaff.Focus();
                            cboStaff.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStaff_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboStaff.EditValue != null)
                {
                    var Search = lstEmployeeADO.FirstOrDefault(o => o.LOGINNAME == cboStaff.EditValue.ToString());
                    if (Search != null)
                    {
                        WaitingManager.Show();
                        EmployeeADO = Search;
                        txtStaff.Text = Search.LOGINNAME;
                        lblDepartmentStaff.Text = Search.DEPARTMENT_NAME;
                        cboStaff.Properties.Buttons[1].Visible = true;

                        CommonParam paramCommon = new CommonParam();
                        HisContactPointFilter filter = new HisContactPointFilter();

                        filter.EMPLOYEE_ID = Search.ID;

                        var ContactPoint = new BackendAdapter(paramCommon).Get<List<HIS_CONTACT_POINT>>("/api/HisContactPoint/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                        //CurrentContactPoint = ContactPoint.FirstOrDefault();
                        SetCurrentContactPoint(ref this.CurrentContactPoint, ContactPoint.FirstOrDefault());

                        this.updateVContactPoint(CurrentContactPoint);

                        WaitingManager.Hide();
                    }
                }
                else
                {
                    EmployeeADO = new HisEmployeeADO();
                    txtStaff.Text = "";
                    lblDepartmentStaff.Text = "";
                    cboStaff.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCurrentContactPoint(ref V_HIS_CONTACT_POINT VContactPoint, HIS_CONTACT_POINT ConTactPoint)
        {
            try
            {
                VContactPoint = new V_HIS_CONTACT_POINT();
                if (ConTactPoint != null)
                {
                    VContactPoint.CONTACT_LEVEL = ConTactPoint.CONTACT_LEVEL;
                    VContactPoint.CONTACT_POINT_OTHER_TYPE_NAME = ConTactPoint.CONTACT_POINT_OTHER_TYPE_NAME;
                    VContactPoint.CONTACT_TYPE = ConTactPoint.CONTACT_TYPE;
                    VContactPoint.CREATE_TIME = ConTactPoint.CREATE_TIME;
                    VContactPoint.CREATOR = ConTactPoint.CREATOR;
                    VContactPoint.DOB = ConTactPoint.DOB;
                    VContactPoint.EMPLOYEE_ID = ConTactPoint.EMPLOYEE_ID;
                    VContactPoint.FIRST_NAME = ConTactPoint.FIRST_NAME;
                    VContactPoint.FULL_NAME = ConTactPoint.VIR_FULL_NAME;
                    VContactPoint.GENDER_ID = ConTactPoint.GENDER_ID;
                    VContactPoint.ID = ConTactPoint.ID;
                    VContactPoint.LAST_NAME = ConTactPoint.LAST_NAME;
                    VContactPoint.MODIFIER = ConTactPoint.MODIFIER;
                    VContactPoint.MODIFY_TIME = ConTactPoint.MODIFY_TIME;
                    VContactPoint.NOTE = ConTactPoint.NOTE;
                    VContactPoint.PATIENT_ID = ConTactPoint.PATIENT_ID;
                    VContactPoint.PHONE = ConTactPoint.PHONE;
                    VContactPoint.TEST_RESULT_1 = ConTactPoint.TEST_RESULT_1;
                    VContactPoint.TEST_RESULT_2 = ConTactPoint.TEST_RESULT_2;
                    VContactPoint.TEST_RESULT_3 = ConTactPoint.TEST_RESULT_3;
                    VContactPoint.VIR_ADDRESS = ConTactPoint.VIR_ADDRESS;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtStaff.Text = "";
                    cboStaff.EditValue = null;
                    lblDepartmentStaff.Text = "";
                    cboStaff.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocustxtStaff()
        {
            try
            {
                txtStaff.Focus();
                txtStaff.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public HisEmployeeADO GetvalueHisEmployeeADO()
        {
            try
            {
                return EmployeeADO;
            }
            catch (Exception ex)
            {
                return new HisEmployeeADO();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public V_HIS_CONTACT_POINT GetCurrentContactPoint()
        {
            try
            {
                if (this.CurrentContactPoint != null && this.CurrentContactPoint.ID > 0)
                {
                    return this.CurrentContactPoint;
                }
                else
                {
                    return null;
                }
                //return this.CurrentContactPoint;
            }
            catch (Exception ex)
            {
                return new V_HIS_CONTACT_POINT();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetValuecboStaff(long? employeeId)
        {
            try
            {
                HisEmployeeADO check = null;
                if (employeeId.HasValue)
                {
                    check = new HisEmployeeADO();
                    check = (this.lstEmployeeADO != null && this.lstEmployeeADO.Count > 0) ? this.lstEmployeeADO.FirstOrDefault(o => o.ID == employeeId.Value) : null;
                }
                txtStaff.EditValue = "";
                cboStaff.EditValue = check != null ? check.LOGINNAME : null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        public void reset()
        {
            try
            {
                this.txtStaff.Text = "";
                this.cboStaff.EditValue = null;
                this.lblDepartmentStaff.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void cboStaff_KeyUp(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        try
            //        {
            //            if (!String.IsNullOrEmpty(cboStaff.Text) && cboStaff.EditValue != null)
            //            {
            //                //this.cboStaff.ShowPopup();
            //                var Search1 = lstEmployeeADO.Where(o => o.LOGINNAME.ToUpper().Contains(cboStaff.EditValue.ToString().ToUpper())).ToList();
            //                if (Search1 != null)
            //                {
            //                    //LoadComboboxStaff(Search1);
            //                    HisEmployeeADO Search = new HisEmployeeADO();
            //                    if (Search1.Count == 1)
            //                    {
            //                        Search = Search1.FirstOrDefault();
            //                        EmployeeADO = Search;
            //                        txtStaff.Text = Search.LOGINNAME;
            //                        lblDepartmentStaff.Text = Search.DEPARTMENT_NAME;
            //                    }
            //                    else
            //                    {
            //                        if (txtStaff.Text != cboStaff.EditValue)
            //                        {
            //                            this.cboStaff.ShowPopup();
            //                        }
            //                    }
            //                    WaitingManager.Show();

            //                    cboStaff.Properties.Buttons[1].Visible = true;

            //                    CommonParam paramCommon = new CommonParam();
            //                    HisContactPointFilter filter = new HisContactPointFilter();

            //                    filter.EMPLOYEE_ID = Search.ID;

            //                    var ContactPoint = new BackendAdapter(paramCommon).Get<List<HIS_CONTACT_POINT>>("/api/HisContactPoint/Get", ApiConsumers.MosConsumer, filter, paramCommon);

            //                    //CurrentContactPoint = ContactPoint.FirstOrDefault();
            //                    SetCurrentContactPoint(ref this.CurrentContactPoint, ContactPoint.FirstOrDefault());

            //                    this.updateVContactPoint(CurrentContactPoint);

            //                    WaitingManager.Hide();
            //                }
            //                else
            //                {
            //                    EmployeeADO = new HisEmployeeADO();
            //                    txtStaff.Text = "";
            //                    lblDepartmentStaff.Text = "";
            //                    cboStaff.Properties.Buttons[1].Visible = false;
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            WaitingManager.Hide();
            //            Inventec.Common.Logging.LogSystem.Error(ex);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);                
            //}
        }

        private void cboStaff_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("TAB");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
