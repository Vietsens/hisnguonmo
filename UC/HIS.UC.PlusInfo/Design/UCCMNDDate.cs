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
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.Utility;
using SDA.EFMODEL.DataModels;
using HIS.UC.PlusInfo.ADO;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.PlusInfo.Validate;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.UC.PlusInfo.ShareMethod;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCCMNDDate : UserControlBase
    {
        #region Declare

        DelegateNextControl dlgFocusNextUserControl;
        int positionHandle = -1;
        #endregion

        #region Contructor - Load

        public UCCMNDDate()
             : base("UCPlusInfo", "UCCMNDDate")
        {
            try
            {
                InitializeComponent();
                this.ValidRelativeCMNDDate();
                this.txtCMNDDate.TabIndex = this.TabIndex;
                //this.SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCCMNDDate_Load(object sender, EventArgs e)
        {
            try
            {
                //this.txtCMNDDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidRelativeCMNDDate()
        {
            try
            {
                Validate_CMNDDate_Control oDobDateRule = new Validate_CMNDDate_Control();
                oDobDateRule.txtCMNDDATE = this.txtCMNDDate;
                oDobDateRule.dt = this.dtCMNDDate;
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtCMNDDate, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.lciCMNDDate.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCCMNDDate", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo,LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCCMNDDate
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCCMNDDate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCCMNDDate.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtCMNDDate.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCCMNDDate.dtCMNDDate.Properties.NullValuePrompt", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCMNDDate.Text = Inventec.Common.Resource.Get.Value("UCCMNDDate.lciCMNDDate.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Data

        internal long? GetValue()
        {
            long? CMND_DATE = null;
            try
            {
                if (!String.IsNullOrEmpty(this.txtCMNDDate.Text))
                {
                    DateTime? dt = DateTimeHelper.ConvertDateTimeStringToSystemTime(this.txtCMNDDate.Text+" 00:00");
                    CMND_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dt);
                }
                else
                    CMND_DATE = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return CMND_DATE;
        }

        internal void SetValue(long? CMNDDate)
        {
            try
            {
                if ((CMNDDate ?? 0) > 0)
                {
                    dtCMNDDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(CMNDDate ?? 0).Value;
                    txtCMNDDate.Text = dtCMNDDate.DateTime.ToString("dd/MM/yyyy");
                    Inventec.Common.Logging.LogSystem.Info("UCCMNDDate: Set date with data : "+CMNDDate);
                }
                else
                {
                    this.txtCMNDDate.Text = "";
                    dtCMNDDate.Text = "";
                    Inventec.Common.Logging.LogSystem.Info("UCCMNDDate: Set date with data null");
                }
                this.dtCMNDDate.Visible = false;
                //this.txtCMNDDate.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Focus

        internal void FocusNextControl(DelegateNextControl _dlgFocusNextControl)
        {
            try
            {
                if (_dlgFocusNextControl != null)
                    this.dlgFocusNextUserControl = _dlgFocusNextControl;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
        internal bool ValidateRequiredField()
        {
            bool result = true;
            try
            {
                positionHandle = -1;
                result = dxValidationProvider1.Validate();
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        internal void ResetRequiredField()
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #region Event Control

        private void dtCMNDDate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    
                    this.dtCMNDDate.Update();
                    this.dtCMNDDate.Visible = false;
                    this.txtCMNDDate.Text = this.dtCMNDDate.DateTime.ToString("dd/MM/yyyy HH:mm");
                    this.positionHandle = -1;
                    if (!dxValidationProvider1.Validate())
                    {
                        return;
                    }
                    if (this.dlgFocusNextUserControl != null)
                        this.dlgFocusNextUserControl(this.txtCMNDDate, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtCMNDDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.positionHandle = -1;
                    if (!dxValidationProvider1.Validate())
                    {
                        return;
                    }
                    this.dtCMNDDate.Visible = true;
                    this.dtCMNDDate.Update();
                    this.dtCMNDDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    System.Threading.Thread.Sleep(100);
                    if (this.dlgFocusNextUserControl != null)
                        this.dlgFocusNextUserControl(this.TabIndex, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtCMNDDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCMNDDate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateTimeStringToSystemTime(this.txtCMNDDate.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtCMNDDate.EditValue = dt;
                        this.dtCMNDDate.Update();
                    }
                    this.dtCMNDDate.Visible = true;
                    this.dtCMNDDate.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCMNDDate_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
               
                if (string.IsNullOrEmpty(this.txtCMNDDate.Text))
                {
                    return;
                }
                else
                {
                    
                    try
                    {                       
                        DateTime.ParseExact(this.txtCMNDDate.Text, "dd/MM/yyyy", null);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCMNDDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    //this.dtCMNDDate.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCMNDDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                   
                    string date = "";
                    if (this.txtCMNDDate.Text.Contains("/"))
                        date = PatientDobUtil.PatientDobToDobRaw(this.txtCMNDDate.Text);

                    if (!String.IsNullOrEmpty(date))
                    {
                        this.txtCMNDDate.Text = date;
                    }
                    Inventec.Common.Logging.LogSystem.Warn("__________1");
                    this.positionHandle = -1;
                     if (!dxValidationProvider1.Validate())
                        {
                            return;
                        }
                     Inventec.Common.Logging.LogSystem.Warn("__________2");
                    if (this.dlgFocusNextUserControl != null)
                        this.dlgFocusNextUserControl(this.TabIndex,null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

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

        private void txtCMNDDate_Validated(object sender, EventArgs e)
        {
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void DisposeControl()
        {
            try
            {
                positionHandle = 0;
                dlgFocusNextUserControl = null;
                this.txtCMNDDate.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCMNDDate_ButtonClick);
                this.txtCMNDDate.EditValueChanged -= new System.EventHandler(this.txtCMNDDate_EditValueChanged);
                this.txtCMNDDate.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtCMNDDate_KeyDown);
                this.txtCMNDDate.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCMNDDate_PreviewKeyDown);
                this.txtCMNDDate.Validated -= new System.EventHandler(this.txtCMNDDate_Validated);
                this.dtCMNDDate.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtCMNDDate_Closed);
                this.dtCMNDDate.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtCMNDDate_KeyDown);
                this.dtCMNDDate.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.dtCMNDDate_KeyPress);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.UCCMNDDate_Load);
                dxErrorProvider1 = null;
                dxValidationProvider1 = null;
                txtCMNDDate = null;
                lciCMNDDate = null;
                dtCMNDDate = null;
                panel1 = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
