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
using DevExpress.XtraEditors.Controls;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using DevExpress.XtraExport;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.EditorLoader;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors;
using HIS.UC.WorkPlace.Validate;

namespace HIS.UC.WorkPlace
{
    public partial class UCWorkPlaceCombo1 : DevExpress.XtraEditors.XtraUserControl
    {
        List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> worlPlaces;
        DelegateFocusMoveout focusMoveout;
        HIS.UC.WorkPlace.DelegatePlusClick plusClick;
        string searchCode;
        int positionHandleControlPatientInfo = -1;
        public UCWorkPlaceCombo1(DelegateFocusMoveout focusMoveout, HIS.UC.WorkPlace.DelegatePlusClick plusClick, List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> worlPlaces)
        {
            try
            {
                InitializeComponent();
                Language_ucWorkPlaceCombo();
                this.focusMoveout = focusMoveout;
                this.plusClick = plusClick;
                this.worlPlaces = worlPlaces;
                CommonParam param = new CommonParam();
                LoadDataCombo(cboWorkPlace1, this.worlPlaces);
                Language_ucWorkPlaceCombo1();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //public UCWorkPlaceCombo1(HIS.UC.WorkPlace.DelegatePlusClick plusClick, List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> worlPlaces)
        //{
        //    InitializeComponent();
        //    try
        //    {
        //        this.plusClick = plusClick;
        //        this.worlPlaces = worlPlaces;
        //        CommonParam param = new CommonParam();
        //        LoadDataCombo(cboWorkPlace1, this.worlPlaces);
        //        Language_ucWorkPlaceCombo1();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //public void FocusNextUserControl(DelegateFocusMoveout _dlgFocusNextUserControl)
        //{
        //    try
        //    {
        //        this.focusMoveout = _dlgFocusNextUserControl;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        public UCWorkPlaceCombo1(DelegateFocusMoveout focusMoveout, HIS.UC.WorkPlace.DelegatePlusClick plusClick, List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> worlPlaces, bool validate)
        {
            try
            {
                InitializeComponent();
                Language_ucWorkPlaceCombo();
                this.focusMoveout = focusMoveout;
                this.plusClick = plusClick;
                this.worlPlaces = worlPlaces;
                CommonParam param = new CommonParam();
                LoadDataCombo(cboWorkPlace1, this.worlPlaces);
                Language_ucWorkPlaceCombo1();
                if (validate)
                {
                    ValidationCombo(true);
                }
                else
                {
                    ValidationCombo(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Language_ucWorkPlaceCombo()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCWorkPlaceCombo_Load(object sender, EventArgs e)
        {

        }

        public long? GetValue()
        {
            long? result = 0;
            try
            {
                result = (long?)(cboWorkPlace1.EditValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public void FocusControl()
        {
            try
            {
                txtWorkPlaceCode1.Focus();
                txtWorkPlaceCode1.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValue(object data)
        {
            try
            {
                if (data is MOS.EFMODEL.DataModels.HIS_WORK_PLACE)
                {
                    txtWorkPlaceCode1.Text = ((MOS.EFMODEL.DataModels.HIS_WORK_PLACE)data).WORK_PLACE_CODE;
                    cboWorkPlace1.EditValue = ((MOS.EFMODEL.DataModels.HIS_WORK_PLACE)data).ID;
                    Inventec.Common.Logging.LogSystem.Warn("START 1 ####################_______________________" + ((MOS.EFMODEL.DataModels.HIS_WORK_PLACE)data).ID);
                }
                else
                {
                    txtWorkPlaceCode1.Text = "";
                    cboWorkPlace1.EditValue = null;
                    Inventec.Common.Logging.LogSystem.Warn("START 2 ####################_______________________");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateFocusMoveOutControl(DelegateFocusMoveout _dlgMoveOut)
        {
            try
            {
                this.focusMoveout = _dlgMoveOut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadWorkPlaceCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboWorkPlace1.EditValue = null;
                    cboWorkPlace1.Focus();
                    cboWorkPlace1.ShowPopup();
                }
                else
                {
                    var data = this.worlPlaces.Where(o => o.WORK_PLACE_CODE.Contains(searchCode) || o.WORK_PLACE_CODE.Contains(searchCode.ToLower())).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboWorkPlace1.EditValue = data[0].ID;
                            txtWorkPlaceCode1.Text = data[0].WORK_PLACE_CODE;
                            if (focusMoveout != null)
                            {
                                focusMoveout();
                            }
                        }
                        else
                        {
                            var singleData = this.worlPlaces.FirstOrDefault(o => o.WORK_PLACE_CODE.ToLower().Equals(searchCode.ToLower()));
                            if (singleData != null)
                            {
                                cboWorkPlace1.EditValue = singleData.ID;
                                txtWorkPlaceCode1.Text = singleData.WORK_PLACE_CODE;
                                if (focusMoveout != null)
                                {
                                    focusMoveout();
                                }
                            }
                            else
                            {
                                cboWorkPlace1.EditValue = null;
                                cboWorkPlace1.Focus();
                                cboWorkPlace1.ShowPopup();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void ValidationCombo(bool isRequired)
        {
            try
            {
                if (isRequired)
                {
                    lblNoiLamViec.AppearanceItemCaption.ForeColor = Color.Maroon;

                    ValidationRuleControls cbRule = new ValidationRuleControls();
                    cbRule.txtControl = txtWorkPlaceCode1;
                    cbRule.cboControl = cboWorkPlace1;

                    cbRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    cbRule.ErrorType = ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(txtWorkPlaceCode1, cbRule);
                }
                else
                {
                    lblNoiLamViec.AppearanceItemCaption.ForeColor = Color.Black;
                //    txtWorkPlaceCode1.ErrorText = "";
                    dxValidationProvider1.RemoveControlError(txtWorkPlaceCode1);
                    dxValidationProvider1.SetValidationRule(txtWorkPlaceCode1, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal bool ValidationCombo()
        {
            bool result = true;
            try
            {
                result = (dxValidationProvider1.Validate());
            }
            catch (Exception ex)
            {
                  Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        public void ResetValidation()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("RESET UC WORKPLACE ####################_______________________");
                lblNoiLamViec.AppearanceItemCaption.ForeColor = Color.Black;
                dxValidationProvider1.SetValidationRule(txtWorkPlaceCode1, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtWorkPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadWorkPlaceCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWorkPlace_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboWorkPlace1.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_WORK_PLACE ethnic = this.worlPlaces.SingleOrDefault(o => o.ID == (long)(cboWorkPlace1.EditValue ?? 0));
                        if (ethnic != null)
                        {
                            txtWorkPlaceCode1.Text = ethnic.WORK_PLACE_CODE;
                        }
                    }
                    if (focusMoveout != null)
                    {
                        focusMoveout();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWorkPlace_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboWorkPlace1.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_WORK_PLACE data = this.worlPlaces.SingleOrDefault(o => o.ID == (long)(cboWorkPlace1.EditValue ?? 0));
                        if (data != null)
                        {
                            txtWorkPlaceCode1.Text = data.WORK_PLACE_CODE;
                            if (focusMoveout != null)
                            {
                                focusMoveout();
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboWorkPlace1.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Language_ucWorkPlaceCombo1()
        {
            try
            {
                ////Khoi tao doi tuong resource
                His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource__ucWorkPlaceCombo1 = new ResourceManager("HIS.UC.WorkPlace.Resources.Lang", typeof(UCWorkPlaceCombo1).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCWorkPlaceCombo1.layoutControl1.Text", His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource__ucWorkPlaceCombo1, LanguageManager.GetCulture());
                this.cboWorkPlace1.Properties.NullText = Inventec.Common.Resource.Get.Value("UCWorkPlaceCombo1.cboWorkPlace1.Properties.NullText", His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource__ucWorkPlaceCombo1, LanguageManager.GetCulture());
                this.lblNoiLamViec.Text = Inventec.Common.Resource.Get.Value("UCWorkPlaceCombo1.lblNoiLamViec.Text", His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource__ucWorkPlaceCombo1, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void ReloadData(object data)
        {
            try
            {
                LoadDataCombo(cboWorkPlace1, data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDataCombo(DevExpress.XtraEditors.GridLookUpEdit cboWorkPlace, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("WORK_PLACE_CODE", "", 70, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "", 180, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboWorkPlace, (data != null ? data : this.worlPlaces), controlEditorADO);
                Inventec.Common.Logging.LogSystem.Warn("Load Cbo ##############################_____________________________");
  //              SetValue(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtWorkPlaceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadWorkPlaceCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWorkPlace1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    if (this.plusClick != null)
                    {
                        this.plusClick();
                    }
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboWorkPlace1.EditValue = null;
                    txtWorkPlaceCode1.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWorkPlace1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboWorkPlace1.Properties.Buttons[2].Visible = false;
                if (cboWorkPlace1.EditValue != null)
                {
                    cboWorkPlace1.Properties.Buttons[2].Visible = true;
                }
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

                if (positionHandleControlPatientInfo == -1)
                {
                    positionHandleControlPatientInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlPatientInfo > edit.TabIndex)
                {
                    positionHandleControlPatientInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
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
