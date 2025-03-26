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
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.UC.UCPatientRaw
{
    public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {
        public void EnableOrDisableControl(DelegateEnableOrDisableControl _isEnable)
        {
            try
            {
                if (_isEnable != null)
                {
                    this.isEnable = _isEnable;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ValidateRelative(DelegateValidationUserControl _isValidate)
        {
            try
            {
                this.dlgSetValidation = _isValidate;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void VisableOrDisableUCHein(DelegateVisible _isVisible)
        {
            try
            {
                this.isVisible = _isVisible;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void EnableOrDisableBtnPatientNew(DelegateEnableOrDisableBtnPatientNew _enableControl)
        {
            try
            {
                this.enableLciBenhNhanMoi = _enableControl;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DisableControlCareerByKeyConfig(bool _isEnable)
        {
            try
            {
                if (_isEnable == true)
                {
                    this.lciMaNgheNghiep.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lciNgheNghiep.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dxValidationProviderControl.SetValidationRule(txtCareerCode, null);
                }
                else
                {
                    this.lciMaNgheNghiep.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciNgheNghiep.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    if (HisConfigs.Get<string>("HIS.Desktop.Plugins.RegisterV2.IsNotCareerRequired") != "1")
                    {
                        lciMaNgheNghiep.AppearanceItemCaption.ForeColor = Color.Maroon;
                        this.ValidateCareer();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateShowCheckWorkingLetter(Action<bool> _isVisible)
        {
            try
            {
                this.dlgShowCheckWorkingLetter = _isVisible;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateShowOrtherPaySource(Action<long> idOrtherPay)
        {
            try
            {
                this.dlgShowOtherPaySource = idOrtherPay;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
