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
using HIS.UC.AddressCombo.ADO;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.Utility;
using MOS.SDO;

namespace HIS.UC.AddressCombo
{
    public partial class UCAddressCombo : UserControlBase
    {
        #region Outside Focus Control

        public void FocusUserControl()
        {
            try
            {
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowLineFirstAddress == 2)
                {
                    this.txtAddress2.Focus();
                    this.txtAddress2.SelectAll();
                }
                else
                {
                    this.txtMaTHX.Focus();
                    this.txtMaTHX.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void GetPatientSdo(HisPatientSDO currentPatientSDO)
        {
            try
            {
                this.currentPatientSDO = currentPatientSDO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusNextUserControl(DelegateFocusNextUserControl _dlgFocusNextUserControl)
        {
            try
            {
                if (_dlgFocusNextUserControl != null)
                {
                    this.dlgFocusNextUserControl = _dlgFocusNextUserControl;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusPhoneNumber()
        {
            try
            {
                   txtPhone.Focus();
                    txtPhone.SelectAll();
            }
            catch (Exception ex)
            {
                 Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Inside Focus Control

        private void FocusToAddress()
        {
            try
            {
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowLineFirstAddress == 2)
                {
                    //if (dlgFocusNextUserControl != null)
                    //{
                    //    this.dlgFocusNextUserControl();
                    //}
                    if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowLineFirstAddress == 2)
                    {
                        if (dlgFocusNextUserControl != null)
                        {
                            this.dlgFocusNextUserControl();
                        }
                    }
                    else if (lciPhone.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never && dlgFocusNextUserControl != null)
                    {
                        this.dlgFocusNextUserControl();
                    }
                    else
                    {
                        txtPhone.Focus();
                        txtPhone.SelectAll();
                    }
                }
                else
                {
                    this.txtAddress.Focus();
                    this.txtAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusToProvince()
        {
            try
            {
                this.txtProvinceCode.Focus();
                this.txtProvinceCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToDistrict()
        {
            try
            {
                this.txtDistrictCode.Focus();
                this.txtDistrictCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToCommune()
        {
            try
            {
                this.txtCommuneCode.Focus();
                this.txtCommuneCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
