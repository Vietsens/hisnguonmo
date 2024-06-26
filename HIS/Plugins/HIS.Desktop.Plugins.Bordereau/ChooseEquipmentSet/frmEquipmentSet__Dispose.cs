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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Controls.ValidationRule;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Bordereau.ChooseEquipmentSet
{
    public partial class frmEquipmentSet : Form
    {
        internal void DisposeControl()
        {
            try
            {
                //success = false;
                //numOrder = null;
                //equipmentId = null;
                controlType = null;
                positionHandle = 0;
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.spinNumOrder.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.spinNumOrder_KeyDown);
                this.spinNumOrder.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinNumOrder_PreviewKeyDown);
                this.cboEquipmentSet.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboEquipmentSet_Closed);
                this.cboEquipmentSet.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboEquipmentSet_ButtonClick);
                this.cboEquipmentSet.EditValueChanged -= new System.EventHandler(this.cboEquipmentSet_EditValueChanged);
                this.cboEquipmentSet.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboEquipmentSet_PreviewKeyDown);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.barButtonItemCtrlS.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemCtrlS_ItemClick);
                this.Load -= new System.EventHandler(this.frmEquipmentSet_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboEquipmentSet.Properties.DataSource = null;
                layoutControlItem2 = null;
                lblGiaTran = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItemCtrlS = null;
                bar1 = null;
                barManager1 = null;
                dxValidationProvider1 = null;
                emptySpaceItem1 = null;
                layoutControlItem3 = null;
                lblNumOrder = null;
                layoutControlItem1 = null;
                gridLookUpEdit1View = null;
                cboEquipmentSet = null;
                spinNumOrder = null;
                btnSave = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
