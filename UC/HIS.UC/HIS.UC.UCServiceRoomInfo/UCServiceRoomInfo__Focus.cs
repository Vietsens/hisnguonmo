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
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;

namespace HIS.UC.UCServiceRoomInfo
{
    public partial class UCServiceRoomInfo : HIS.Desktop.Utility.UserControlBase
    {
        #region Outside Focus UserControl

        public void FocusUserControl()
        {
            try
            {
                if (lciCboPatientType.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    cboPatientType.Focus();
                    cboPatientType.SelectAll();
                }
                else
                {
                    ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).FocusUserControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Focus den uc UCServiceRoomInfo that bai: \n" + ex);
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
                Inventec.Common.Logging.LogSystem.Warn("Focus ra khoi uc UCServiceRoomInfo that bai: \n" + ex);
            }
        }

        #endregion

        #region Inside Focus UserControl

        void FocusInServiceRoomInfo()
        {
            try
            {
                if (this.roomExamServiceProcessor != null)
                {
                    foreach (LayoutControlItem item in this.layoutControl2.Root.Items)
                    {
                        if (item != null && (item.Control is UserControl || item.Control is XtraUserControl))
                        {
                            this.roomExamServiceProcessor.FocusAndShow(item.Control);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void FocusInService()
        {
            try
            {
                if (this.roomExamServiceProcessor != null)
                {
                    foreach (LayoutControlItem item in this.layoutControl2.Root.Items)
                    {
                        if (item != null && (item.Control is UserControl || item.Control is XtraUserControl))
                        {
                            this.roomExamServiceProcessor.FocusService(item.Control);
                            break;
                        }
                    }
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
