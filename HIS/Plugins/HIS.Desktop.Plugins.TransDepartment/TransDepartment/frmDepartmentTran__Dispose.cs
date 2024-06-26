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
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransDepartment
{
    public partial class frmDepartmentTran : FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                IsNotShowOutMediAndMate = 0;
                WarningOptionInCaseOfUnassignTrackingServiceReq = 0;
                ucSecondaryIcd = null;
                subIcdProcessor = null;
                currentModule = null;
                ucIcd = null;
                icdProcessor = null;
                isView = false;
                departmentTran = null;
                DelegateReturnSuccess = null;
                RefeshReference = null;
                roomId = 0;
                waitLoad = null;
                positionHandleControl = 0;
                departmentId = 0;
                treatmentId = 0;
                servicereq12 = null;
                sereservs = null;
                listDepartments = null;
                this.chkCCS.CheckedChanged -= new System.EventHandler(this.chkCCS_CheckedChanged);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick_1);
                this.chkAutoLeave.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.chkAutoLeave_KeyDown);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.cboDepartment.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDepartment_Closed);
                this.cboDepartment.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboDepartment_ButtonClick);
                this.cboDepartment.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboDepartment_KeyUp);
                this.txtDepartmentCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDepartmentCode_PreviewKeyDown);
                this.dtLogTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtLogTime_Closed);
                this.dtLogTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtLogTime_KeyDown);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.frmDepartmentTran_Load);
                this.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.frmDepartmentTran_KeyPress);
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboDepartment.Properties.DataSource = null;
                layoutControlItem5 = null;
                chkCCS = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem1 = null;
                bar1 = null;
                barManager1 = null;
                layoutControlItem7 = null;
                panelControl1 = null;
                layoutControlItem6 = null;
                panelControlSubIcd = null;
                emptySpaceItem1 = null;
                emptySpaceItem2 = null;
                layoutControlItem4 = null;
                chkAutoLeave = null;
                layoutControlItem3 = null;
                lbTreatmentType = null;
                layoutControlItem2 = null;
                btnSave = null;
                layoutControlItem1 = null;
                gridLookUpEdit1View = null;
                cboDepartment = null;
                dxValidationProvider1 = null;
                dtLogTime = null;
                txtDepartmentCode = null;
                lciDepartmentTo = null;
                lciDTLogTime = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
