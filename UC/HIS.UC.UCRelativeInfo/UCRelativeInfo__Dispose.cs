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
using HIS.Desktop.Utility;

namespace HIS.UC.UCRelativeInfo
{
    public partial class UCRelativeInfo : UserControlBase
    {
        public void DisposeControl()
        {
            try
            {
                CurrentModule = null;
                IsChild = false;
                IsObligatory = false;
                positionHandleControl = 0;
                dlgFocusNextUserControl = null;
                this.txtMother.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMother_PreviewKeyDown);
                this.txtFather.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtFather_PreviewKeyDown);
                this.chkCapGiayNghiOm.CheckedChanged -= new System.EventHandler(this.chkCapGiayNghiOm_CheckedChanged);
                this.chkCapGiayNghiOm.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkCapGiayNghiOm_PreviewKeyDown);
                this.txtRelativePhone.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtRelativePhone_PreviewKeyDown);
                this.txtRelativeCMNDNumber.EditValueChanged -= new System.EventHandler(this.txtRelativeCMNDNumber_EditValueChanged);
                this.txtRelativeCMNDNumber.TextChanged -= new System.EventHandler(this.txtRelativeCMNDNumber_TextChanged);
                this.txtRelativeCMNDNumber.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtRelativeCMNDNumber_KeyPress);
                this.txtRelativeCMNDNumber.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtRelativeCMNDNumber_PreviewKeyDown);
                this.txtRelativeCMNDNumber.Validated -= new System.EventHandler(this.txtRelativeCMNDNumber_Validated);
                this.txtRelativeAddress.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtRelativeAddress_PreviewKeyDown);
                this.txtCorrelated.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCorrelated_PreviewKeyDown);
                this.txtHomePerson.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtHomePerson_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.UCRelativeInfo_Load);
                toolTipItem2 = null;
                lciMother = null;
                lciFather = null;
                txtFather = null;
                txtMother = null;
                lciForchkCapGiayNghiOm = null;
                chkCapGiayNghiOm = null;
                lciFortxtRelativePhone = null;
                txtRelativePhone = null;
                dxErrorProviderControl = null;
                dxValidationProviderControl = null;
                txtRelativeCMNDNumber = null;
                lciCMND = null;
                lciAddress = null;
                txtRelativeAddress = null;
                lciRelative = null;
                txtCorrelated = null;
                lciHomPerson = null;
                txtHomePerson = null;
                lcgHomePerson = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
