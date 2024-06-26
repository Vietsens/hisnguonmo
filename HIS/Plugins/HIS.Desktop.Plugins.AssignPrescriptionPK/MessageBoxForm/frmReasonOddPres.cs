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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraLayout.Utils;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm
{
    public partial class frmReasonOddPres : DevExpress.XtraEditors.XtraForm
    {
        LyDoKeThuocQuaSoLuong LyDoKeLe;
        string PrescriptionMessage = "";
        string _lyDoKeLe = "";
        internal int positionHandleControl = -1;
        bool IsClickYN;

        public frmReasonOddPres(string _PrescriptionMessage, LyDoKeThuocQuaSoLuong _LyDoKeLe, string _lyDoKeLe)
        {
            InitializeComponent();
            this.LyDoKeLe = _LyDoKeLe;
            this.PrescriptionMessage = _PrescriptionMessage;
            this._lyDoKeLe = _lyDoKeLe;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmReasonOddPres_Load(object sender, EventArgs e)
        {

            try
            {
                Resources.ResourceLanguageManager.LanguagefrmReasonMaxPrescription = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm.frmReasonMaxPrescription).Assembly);
                this.txtPrescriptionMessage.Text = this.PrescriptionMessage;
                this.txtReason.Focus();
                this.txtReason.SelectAll();
                this.txtReason.Text = this._lyDoKeLe;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, int maxLength)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = true;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
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

        private void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidationSingleControl(this.txtReason, this.dxValidationProvider1, 2000);

                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                IsClickYN = true;
                WaitingManager.Show();
                this.LyDoKeLe(this.txtReason.Text);
                this.Close();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            try
            {
                this.dxValidationProvider1.SetValidationRule(txtReason, null);
                this.LyDoKeLe("");
                IsClickYN = false;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmReasonOddPres_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_lyDoKeLe) && !IsClickYN)
                    this.LyDoKeLe("");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
