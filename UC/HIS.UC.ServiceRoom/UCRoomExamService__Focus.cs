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
using Inventec.Common.Logging;

namespace HIS.UC.ServiceRoom
{
    public partial class UCRoomExamService : UserControl
    {
        #region Outside Focus UserControl

        public void FocusUserControl()
        {
            try
            {
                if (this.isFocusCombo)
                {
                    ProcessShowpopupControlContainerRoom((Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.FocusExecuteRoomOption != "1"));
                }
                else
                {
                    this.txtRoomCode.Focus();
                    this.txtRoomCode.SelectAll();
                }
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
                else
                    this.dlgFocusNextUserControl = SendTABToNextUserControl;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SendTABToNextUserControl()
        {
            try
            {
                SendKeys.Send("{TAB}");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Inside Focus UserControl

        public void FocusTotxtExamServiceCode()
        {
            try
            {
                this.txtExamServiceCode.Focus();
                this.txtExamServiceCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FocusTocboExamService()
        {
            try
            {
                this.cboExamService.Focus();
                this.cboExamService.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

    }
}
