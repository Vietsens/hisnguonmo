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
using HIS.Desktop.Utility;
using HIS.UC.UCAddressCombo.Valid;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.Library.RegisterConfig;

namespace HIS.UC.AddressCombo
{
    public partial class UCAddressCombo : UserControlBase
    {
        #region Outside Validate

        public bool ValidateRequiredField(bool _isValidate)
        {
            bool valid = true;
            try
            {
                if (_isValidate == true)
                    this.SetValidate();
                else if (_isValidate == false)
                {
                    this.ResetRequiredField();
                    valid = true;
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Check validte with controls address when patient is children and create hein by Birth certificate.
        /// </summary>
        /// <param name="_isValidate"></param>
        public void IsValidateAddressCombo(string _isValidate)
        {
            try
            {
                if (_isValidate == "1")
                {
                    lciProvince.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciDistrict.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciCommune.AppearanceItemCaption.ForeColor = Color.Maroon;
                    SetValidate();
                }else if(_isValidate == "2")
                {
                    lciProvince.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciDistrict.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateProvince();
                    ValidateDistrict();
                }    
                else
                {
                    lciProvince.AppearanceItemCaption.ForeColor = Color.Black;
                    lciDistrict.AppearanceItemCaption.ForeColor = Color.Black;
                    lciCommune.AppearanceItemCaption.ForeColor = Color.Black;
                    ResetRequiredField();
                }
                //bool _isValidateDHST = HisConfigCFG.IsRequiredDHST;            
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Set Validate

        private void ClearValidate()
        {
            try
            {
                this.dxValidationProviderControl.SetValidationRule(txtProvinceCode, null);
                this.dxValidationProviderControl.SetValidationRule(txtDistrictCode, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValidate()
        {
            try
            {
                ValidateProvince();
                ValidateDistrict();
                ValidateCommune();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateProvince()
        {
            Valid_Province_Control validateProvince = new Valid_Province_Control();
            validateProvince.cboProvince = this.cboProvince;
            validateProvince.txtProvince = this.txtProvinceCode;
            validateProvince.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            validateProvince.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtProvinceCode, validateProvince);
        }

        private void ValidateDistrict()
        {
            Valid_District_Control validateProvince = new Valid_District_Control();
            validateProvince.cboDistrict = this.cboDistrict;
            validateProvince.txtDistrict = this.txtDistrictCode;
            validateProvince.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            validateProvince.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtDistrictCode, validateProvince);
        }

        private void ValidateCommune()
        {
            Valid_Commune_Control validateProvince = new Valid_Commune_Control();
            validateProvince.cboCommune = this.cboCommune;
            validateProvince.txtCommune = this.txtCommuneCode;
            validateProvince.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            validateProvince.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtCommuneCode, validateProvince);
        }

        private void ValidateMaxlengthAddress(DevExpress.XtraEditors.TextEdit txtEdit, int maxlength, bool isVali = false)
        {
            TextEditMaxLengthValidationRule _rule = new TextEditMaxLengthValidationRule();
            _rule.txtEdit = txtEdit;
            _rule.maxlength = maxlength;
            _rule.isVali = isVali;
            _rule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            _rule.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtEdit, _rule);
        }

        #endregion
    }
}
