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
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {

                dlgGetIntructionTime = null;
                timer = null;
                _RoomID = 0;
                changeRoomNotEmergency = null;
                registerPatientWithRightRouteBHYT = null;
                dlgGetPatientTypeId = null;
                dlgFocusNextUserControl = null;
                isFocusCombo = false;
                roomExamServiceNumber = 0;
                roomExamServiceProcessor.DisposeControl(ucRoomExamService);
                ucRoomExamService = null;
                roomExamServiceProcessor = null;
                this.CboPatientTypePrimary.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.CboPatientTypePrimary_Closed);
                this.CboPatientTypePrimary.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.CboPatientTypePrimary_ButtonClick);
                this.CboPatientTypePrimary.EditValueChanged -= new System.EventHandler(this.CboPatientTypePrimary_EditValueChanged);
                this.CboPatientTypePrimary.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.CboPatientTypePrimary_KeyUp);
                this.cboPatientType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPatientType_Closed);
                this.cboPatientType.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPatientType_ButtonClick);
                this.cboPatientType.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboPatientType_KeyUp);
                this.btnAddRow.Click -= new System.EventHandler(this.btnAddRow_Click);
                this.Load -= new System.EventHandler(this.UCServiceRoomInfo_Load);
                gridView1.GridControl.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                lciCboPatientTypePhuThu = null;
                gridView1 = null;
                CboPatientTypePrimary = null;
                layoutControlItem5 = null;
                labelControl1 = null;
                lciCboPatientType = null;
                gridLookUpEdit1View = null;
                cboPatientType = null;
                layoutControlItem2 = null;
                Root = null;
                layoutControl2 = null;
                layoutControlItem1 = null;
                btnAddRow = null;
                layoutControlGroup1 = null;
                lcUCServiceRoomInfo = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
