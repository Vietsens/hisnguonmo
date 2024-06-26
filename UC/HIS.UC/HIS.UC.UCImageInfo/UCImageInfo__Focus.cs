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
using HIS.UC.UCImageInfo.ADO;
using HIS.Desktop.DelegateRegister;

namespace HIS.UC.UCImageInfo
{
    public partial class UCImageInfo : UserControl
    {
        #region Outside Focus UserControl
        public void FocusUserControl()
        {
            try
            {
                this.btnAnhChanDung.Focus();
                //this.btnAnhChanDung_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusNextUserControl(Action<object> _dlgFocusNextUserControl)
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
        private void FocusToImageHein()
        {
            try
            {
                if (this.btnAnhTheBHYT.Enabled == true && this.pteAnhTheBHYT.Enabled == true && (this.pteAnhTheBHYT.Image == null || String.IsNullOrEmpty(pteAnhTheBHYT.Tag.ToString())))
                {
                    this.btnAnhTheBHYT.Focus();
                    btnAnhTheBHYT_Click(null, null);
                }
                else if (dlgFocusNextUserControl != null)
                {
                    this.dlgFocusNextUserControl(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        public void DisablePictureboxControlHeni(bool _isDisable)
        {
            try
            {
                this.pteAnhTheBHYT.Enabled = _isDisable;
                this.btnAnhTheBHYT.Enabled = _isDisable;
                this.pteCmndAfter.Enabled = _isDisable;
                this.pteCmndBefore.Enabled = _isDisable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
