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
using HIS.UC.UCRelativeInfo.ADO;
using HIS.UC.UCRelativeInfo.Valid;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Utility;

namespace HIS.UC.UCRelativeInfo
{
    public partial class UCRelativeInfo : UserControlBase
    {
        #region Outside Validate

        bool relativesInforOptionIs1 = (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.RelativesInforOption == "1");
        bool relativesInforOptionIs2 = (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.RelativesInforOption == "2");
        public bool ValidateRequiredField()
        {
            bool valid = true;
            try
            {

                this.positionHandleControl = -1;
                if (this.IsObligatory && (relativesInforOptionIs1 || relativesInforOptionIs2))
                {
                    this.ValidEmptyControl(this.txtFather, true);
                    this.ValidEmptyControl(this.txtMother, true);
                    this.ValidControl(this.txtHomePerson, 100, true);
                    this.ValidControl(this.txtCorrelated, 50, true);
                }
                else
                {
                    this.ValidControl(this.txtHomePerson, 100, this.IsObligatory);
                }
                if (this.dxValidationProviderControl.Validate(txtFather))
                {
                    dxValidationProviderControl.SetValidationRule(txtMother, null);
                    this.ValidControl(this.txtHomePerson, 100, false);
                    this.ValidControl(this.txtCorrelated, 50, false);
                }else if (this.dxValidationProviderControl.Validate(txtMother))
                {
                    dxValidationProviderControl.SetValidationRule(txtFather, null);
                    this.ValidControl(this.txtHomePerson, 100, false);
                    this.ValidControl(this.txtCorrelated, 50, false);
                }else if (this.dxValidationProviderControl.Validate(txtHomePerson) && this.dxValidationProviderControl.Validate(txtCorrelated))
                {
                    dxValidationProviderControl.SetValidationRule(txtMother, null);
                    dxValidationProviderControl.SetValidationRule(txtFather, null);
                }
                if (!this.dxValidationProviderControl.Validate())
                {
                    IList<Control> invalidControls = this.dxValidationProviderControl.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        Inventec.Common.Logging.LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                    valid = false;
                }

            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        public void ResetRequiredField()
        {
            try
            {
                IList<Control> invalidControls = this.dxValidationProviderControl.GetInvalidControls();
                for (int i = invalidControls.Count - 1; i >= 0; i--)
                {
                    this.dxValidationProviderControl.RemoveControlError(invalidControls[i]);
                }
                this.dxErrorProviderControl.ClearErrors();
                this.ResetValidate();
                //this.ValidRelativeCMNDNumber();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValidateControl(bool _isObligatory)
        {
            try
            {

                this.IsObligatory = _isObligatory;
                this.lciFortxtRelativePhone.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                this.lciCMND.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                this.lciAddress.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                if (relativesInforOptionIs1)
                {
                    this.lciAddress.AppearanceItemCaption.ForeColor = (this.IsObligatory && relativesInforOptionIs1) ? System.Drawing.Color.Maroon : System.Drawing.Color.Black;
                    this.lciCMND.AppearanceItemCaption.ForeColor = (this.IsObligatory && relativesInforOptionIs1) ? System.Drawing.Color.Maroon : System.Drawing.Color.Black;
                }
                dxValidationProviderControl.SetValidationRule(txtFather , null);
                dxValidationProviderControl.SetValidationRule(txtMother, null);
                dxValidationProviderControl.SetValidationRule(txtHomePerson, null);
                dxValidationProviderControl.SetValidationRule(txtCorrelated, null);

                this.ValidControl(this.txtHomePerson, 100, false);
                this.ValidControl(this.txtCorrelated, 50, false);
                this.ValidControl(this.txtHomePerson, 100, false);

                this.ValidRelativeCMNDNumber(this.IsObligatory && relativesInforOptionIs1);
                this.ValidControl(this.txtRelativeAddress, 200, this.IsObligatory && relativesInforOptionIs1);
                this.ValidControl(this.txtRelativePhone, 12, false);
                if (this.IsObligatory && (relativesInforOptionIs1 || relativesInforOptionIs2))
                {
                    this.ValidEmptyControl(this.txtFather, true);
                    this.ValidEmptyControl(this.txtMother, true);
                    this.ValidControl(this.txtHomePerson, 100, true);
                    this.ValidControl(this.txtCorrelated, 50, true);
                }
                else
                {
                    this.ValidControl(this.txtHomePerson, 100, this.IsObligatory);
                }
                Inventec.Common.Logging.LogSystem.Debug("bool relativesInforOptionIs1 " + relativesInforOptionIs1);
                Inventec.Common.Logging.LogSystem.Debug("bool relativesInforOptionIs2 " + relativesInforOptionIs2);
                Inventec.Common.Logging.LogSystem.Debug("_isObligatory " + _isObligatory);

                this.lcgHomePerson.AppearanceItemCaption.ForeColor = this.IsObligatory ? System.Drawing.Color.Maroon : System.Drawing.Color.Black;
                this.lciRelative.AppearanceItemCaption.ForeColor = (this.IsObligatory && relativesInforOptionIs1) ? System.Drawing.Color.Maroon : System.Drawing.Color.Black;
                this.lciFather.AppearanceItemCaption.ForeColor = (this.IsObligatory && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.MustHaveNCSInfoForChild) ? System.Drawing.Color.Maroon : System.Drawing.Color.Black;
                this.lciMother.AppearanceItemCaption.ForeColor = (this.IsObligatory && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.MustHaveNCSInfoForChild) ? System.Drawing.Color.Maroon : System.Drawing.Color.Black;
                this.lciForchkCapGiayNghiOm.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                if (!_isObligatory)
                {
                    this.txtRelativeAddress.Text = "";
                    this.ResetRequiredField();
                }
                this.IsChild = false;
                if ((this.IsObligatory && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.MustHaveNCSInfoForChild) || (this.IsObligatory && relativesInforOptionIs1))
                {
                    this.IsChild = _isObligatory;
                }
                Inventec.Common.Logging.LogSystem.Debug("_isObligatory " + IsChild);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool ValidateNeedOne()
        {
            bool result = false;
            try
            {

                if (String.IsNullOrEmpty(txtFather.Text) && String.IsNullOrEmpty(txtMother.Text))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.
                   Show(Resources.ResourceMessage.ThongTinBatBuocMotTrongHai, Resources.ResourceMessage.Thongbao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                    {
                        result = true;
                    }
                    txtFather.Focus();
                }
                return result;
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return result;
            }

        }
        #endregion

        #region Valid

        private void ResetValidate()
        {
            try
            {
                //this.dxValidationProviderControl.SetValidationRule(this.txtCorrelated, null);
                //this.dxValidationProviderControl.SetValidationRule(this.txtHomePerson, null);
                //this.dxValidationProviderControl.SetValidationRule(this.txtRelativeCMNDNumber, null);
                //this.dxValidationProviderControl.SetValidationRule(this.txtRelativeAddress, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void ValidateControl()
        //{
        //    try
        //    {
        //        //ValidRelativeAddress();
        //        //ValidRelativeCMNDNumber();
        //        //ValidCorrelated();
        //        ValidHomePerson();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void ValidRelativeAddress()
        //{
        //    try
        //    {
        //        Valid_RelativeAddress_Control oDobDateRule = new Valid_RelativeAddress_Control();
        //        oDobDateRule.txtRelativeAddress = this.txtRelativeAddress;
        //        oDobDateRule.ErrorText = HIS.UC.UCRelativeInfo.Valid.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
        //        oDobDateRule.ErrorType = ErrorType.Warning;
        //        this.dxValidationProviderControl.SetValidationRule(this.txtRelativeAddress, oDobDateRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void ValidCorrelated()
        //{
        //    try
        //    {
        //        Valid_Correlated_Control oDobDateRule = new Valid_Correlated_Control();
        //        oDobDateRule.txtCorrelated = this.txtCorrelated;
        //        oDobDateRule.ErrorText = HIS.UC.UCRelativeInfo.Valid.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
        //        oDobDateRule.ErrorType = ErrorType.Warning;
        //        this.dxValidationProviderControl.SetValidationRule(this.txtCorrelated, oDobDateRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void ValidRelativeCMNDNumber()
        //{
        //    try
        //    {
        //        Valid_RelativeCMNDNumber_Control oDobDateRule = new Valid_RelativeCMNDNumber_Control();
        //        oDobDateRule.txtRelativeCMNDNumber = this.txtRelativeCMNDNumber;
        //        oDobDateRule.ErrorType = ErrorType.Warning;
        //        this.dxValidationProviderControl.SetValidationRule(this.txtRelativeCMNDNumber, oDobDateRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void ValidRelativeCMNDNumber(bool isValid)
        {
            try
            {
                Validate_CMND_Control oDobDateRule = new Validate_CMND_Control();
                oDobDateRule.txtCMND = this.txtRelativeCMNDNumber;
                oDobDateRule.isValid = isValid;
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(this.txtRelativeCMNDNumber, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidHomePerson()
        {
            try
            {
                Valid_HomePerson_Control oDobDateRule = new Valid_HomePerson_Control();
                oDobDateRule.txtHomePerson = this.txtHomePerson;
                oDobDateRule.ErrorText = HIS.UC.UCRelativeInfo.Valid.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(this.txtHomePerson, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl(DevExpress.XtraEditors.TextEdit txtEdit, int maxlength, bool isVali)
        {
            try
            {
                TextEditMaxLengthValidationRule _Rule = new TextEditMaxLengthValidationRule();
                _Rule.txtEdit = txtEdit;
                _Rule.maxlength = maxlength;
                _Rule.isVali = isVali;
                _Rule.ErrorText = HIS.UC.UCRelativeInfo.Valid.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                _Rule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(txtEdit, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidEmptyControl(DevExpress.XtraEditors.TextEdit txt1, bool isValid)
        {
            try
            {
                ValidCustomMultilFeilName _Rule = new ValidCustomMultilFeilName();
                _Rule.txt1 = txt1;
                _Rule.isValid = isValid;
                _Rule.ErrorText = HIS.UC.UCRelativeInfo.Valid.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                _Rule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(txt1, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
