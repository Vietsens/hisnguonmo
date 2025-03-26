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
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.Utility;

namespace HIS.UC.UCRelativeInfo
{
    public partial class UCRelativeInfo : UserControlBase
    {
        #region Outside Focus UserControl

        public void FocusUserControl()
        {
            try
            {
                this.txtFather.Focus();
                this.txtFather.SelectAll();
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

        #endregion

        #region Inside Focus UserControl

        private void FocusToCorrelated()
        {
            try
            {
                if (lciRelative.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {
                    SendKeys.Send("{TAB}");
                }
                else
                {
                    this.txtCorrelated.Focus();
                    this.txtCorrelated.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToRelativeCMNDNumber()
        {
            try
            {
                if (lciCMND.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {
                    SendKeys.Send("{TAB}");
                }
                else
                {
                    this.txtRelativeCMNDNumber.Focus();
                    this.txtRelativeCMNDNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToRelativeAddress()
        {
            try
            {
                if (lciAddress.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {
                    SendKeys.Send("{TAB}");
                }
                else
                {
                    this.txtRelativeAddress.Focus();
                    this.txtRelativeAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToRelativePhone()
        {
            try
            {
                if (lciFortxtRelativePhone.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {
                    SendKeys.Send("{TAB}");
                }
                else
                {
                    this.txtRelativePhone.Focus();
                    this.txtRelativePhone.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToMother()
        {
            try
            {
                if (lciFortxtRelativePhone.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {
                    SendKeys.Send("{TAB}");
                }
                else
                {
                    this.txtMother.Focus();
                    this.txtMother.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToHomePerson()
        {
            try
            {
                if (lciFortxtRelativePhone.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {
                    SendKeys.Send("{TAB}");
                }
                else
                {
                    this.txtHomePerson.Focus();
                    this.txtHomePerson.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
