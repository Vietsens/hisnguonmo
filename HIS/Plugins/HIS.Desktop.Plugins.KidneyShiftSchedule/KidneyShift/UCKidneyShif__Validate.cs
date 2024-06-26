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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.KidneyShiftSchedule.Resources;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KidneyShiftSchedule.KidneyShift
{
    public partial class UCKidneyShift : UserControlBase
    {
        private void dxValidationProviderControl_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
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

        private void ValidateForm()
        {
            try
            {
                this.ValidationSingleControl(this.dateDateForAdd, this.dxValidationProviderControl);
                this.ValidationSingleControl(this.cboCaForAdd, this.dxValidationProviderControl);
                this.ValidationSingleControl(this.cboMarchineForAdd, this.dxValidationProviderControl);
                this.ValidateGridLookupWithTextEdit(this.cboServiceForAdd, this.txtServiceForAdd, this.dxValidationProviderControl);
                this.ValidationSingleControl(this.cboExpMestTemplateForAdd, this.dxValidationProviderControl);
                this.ValidationSingleControl(this.cboPatientType, this.dxValidationProviderControl);
                this.ValidationSingleControl(this.txtLoginName, this.dxValidationProviderControl, GetMessageForValidUser(), ValidUser);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ResetRequiredField()
        {
            try
            {
                IList<Control> invalidControls = this.dxValidationProviderControl.GetInvalidControls();
                for (int i = invalidControls.Count - 1; i >= 0; i--)
                {
                    this.dxValidationProviderControl.RemoveControlError(invalidControls[i]);
                }
                this.dxErrorProvider1.ClearErrors();
                //this.ClearValidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool ValidUser()
        {
            bool valid = true;
            try
            {
                if ((String.IsNullOrWhiteSpace(txtLoginName.Text) || cboUser.EditValue == null))
                {
                    return false;
                }
                if (cboUser.EditValue != null)
                {
                    ACS_USER user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == 1 && o.LOGINNAME.Equals(cboUser.EditValue.ToString()));
                    if (user == null)
                    {
                        return false;
                    }

                    if (String.IsNullOrWhiteSpace(user.USERNAME))
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

        string GetMessageForValidUser()
        {
            string mess = "";
            try
            {
                if ((String.IsNullOrWhiteSpace(txtLoginName.Text) || cboUser.EditValue == null))
                {
                    return MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                }
                if (cboUser.EditValue != null)
                {
                    ACS_USER user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == 1 && o.LOGINNAME.Equals(cboUser.EditValue.ToString()));
                    if (user == null)
                    {
                        return MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    }

                    if (String.IsNullOrWhiteSpace(user.USERNAME))
                    {
                        return String.Format(ResourceMessage.NguoiChiDinhKhongCoThongTinHoTen, user.LOGINNAME);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return mess;
        }
    }
}
