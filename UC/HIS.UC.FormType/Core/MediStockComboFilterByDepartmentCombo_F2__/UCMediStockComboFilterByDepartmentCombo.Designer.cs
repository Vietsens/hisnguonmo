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
namespace HIS.UC.FormType.MediStockComboFilterByDepartmentCombo
{
    public partial class UCMediStockComboFilterByDepartmentCombo
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lblTitleName = new DevExpress.XtraEditors.LabelControl();
            this.txtRoomCode = new DevExpress.XtraEditors.TextEdit();
            this.txtDepartmentCode = new DevExpress.XtraEditors.TextEdit();
            this.cboDepartment = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciDepartment = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMediStock = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTitleName = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRoomCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTitleName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblTitleName);
            this.layoutControl1.Controls.Add(this.txtRoomCode);
            this.layoutControl1.Controls.Add(this.txtDepartmentCode);
            this.layoutControl1.Controls.Add(this.cboDepartment);
            this.layoutControl1.Controls.Add(this.cboRoom);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 48);
            this.layoutControl1.TabIndex = 1;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblTitleName
            // 
            this.lblTitleName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTitleName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTitleName.Location = new System.Drawing.Point(2, 2);
            this.lblTitleName.Name = "lblTitleName";
            this.lblTitleName.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.lblTitleName.Size = new System.Drawing.Size(86, 44);
            this.lblTitleName.StyleController = this.layoutControl1;
            this.lblTitleName.TabIndex = 25;
            this.lblTitleName.Text = " ";
            // 
            // txtRoomCode
            // 
            this.txtRoomCode.Location = new System.Drawing.Point(187, 26);
            this.txtRoomCode.Name = "txtRoomCode";
            this.txtRoomCode.Size = new System.Drawing.Size(105, 20);
            this.txtRoomCode.StyleController = this.layoutControl1;
            this.txtRoomCode.TabIndex = 3;
            this.txtRoomCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtRoomCode_PreviewKeyDown);
            // 
            // txtDepartmentCode
            // 
            this.txtDepartmentCode.Location = new System.Drawing.Point(187, 2);
            this.txtDepartmentCode.Name = "txtDepartmentCode";
            this.txtDepartmentCode.Size = new System.Drawing.Size(105, 20);
            this.txtDepartmentCode.StyleController = this.layoutControl1;
            this.txtDepartmentCode.TabIndex = 1;
            this.txtDepartmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDepartmentCode_PreviewKeyDown);
            // 
            // cboDepartment
            // 
            this.cboDepartment.Location = new System.Drawing.Point(292, 2);
            this.cboDepartment.Name = "cboDepartment";
            this.cboDepartment.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDepartment.Properties.NullText = "";
            this.cboDepartment.Properties.View = this.gridLookUpEdit1View;
            this.cboDepartment.Size = new System.Drawing.Size(366, 20);
            this.cboDepartment.StyleController = this.layoutControl1;
            this.cboDepartment.TabIndex = 2;
            this.cboDepartment.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDepartment_Closed);
            this.cboDepartment.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboDepartment_KeyUp);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // cboRoom
            // 
            this.cboRoom.Location = new System.Drawing.Point(292, 26);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboRoom.Properties.NullText = "";
            this.cboRoom.Properties.View = this.gridView1;
            this.cboRoom.Size = new System.Drawing.Size(366, 20);
            this.cboRoom.StyleController = this.layoutControl1;
            this.cboRoom.TabIndex = 4;
            this.cboRoom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboRoom_Closed);
            this.cboRoom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboRoom_KeyUp);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciDepartment,
            this.layoutControlItem10,
            this.layoutControlItem1,
            this.lciMediStock,
            this.lciTitleName});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 48);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciDepartment
            // 
            this.lciDepartment.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDepartment.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDepartment.Control = this.txtDepartmentCode;
            this.lciDepartment.Location = new System.Drawing.Point(90, 0);
            this.lciDepartment.Name = "layoutDepartment";
            this.lciDepartment.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciDepartment.Size = new System.Drawing.Size(202, 24);
            this.lciDepartment.Text = "Khoa:";
            this.lciDepartment.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDepartment.TextSize = new System.Drawing.Size(90, 13);
            this.lciDepartment.TextToControlDistance = 5;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.cboDepartment;
            this.layoutControlItem10.Location = new System.Drawing.Point(292, 0);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem10.Size = new System.Drawing.Size(368, 24);
            this.layoutControlItem10.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextToControlDistance = 0;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.cboRoom;
            this.layoutControlItem1.Location = new System.Drawing.Point(292, 24);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem1.Size = new System.Drawing.Size(368, 24);
            this.layoutControlItem1.Text = "Phòng:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciMediStock
            // 
            this.lciMediStock.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMediStock.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMediStock.Control = this.txtRoomCode;
            this.lciMediStock.Location = new System.Drawing.Point(90, 24);
            this.lciMediStock.Name = "layMediStock";
            this.lciMediStock.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciMediStock.Size = new System.Drawing.Size(202, 24);
            this.lciMediStock.Text = "Kho:";
            this.lciMediStock.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMediStock.TextSize = new System.Drawing.Size(90, 13);
            this.lciMediStock.TextToControlDistance = 5;
            // 
            // lciTitleName
            // 
            this.lciTitleName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTitleName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTitleName.Control = this.lblTitleName;
            this.lciTitleName.Location = new System.Drawing.Point(0, 0);
            this.lciTitleName.MaxSize = new System.Drawing.Size(500, 100);
            this.lciTitleName.MinSize = new System.Drawing.Size(10, 10);
            this.lciTitleName.Name = "lciTitleName";
            this.lciTitleName.Size = new System.Drawing.Size(90, 48);
            this.lciTitleName.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTitleName.Text = " ";
            this.lciTitleName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTitleName.TextSize = new System.Drawing.Size(0, 0);
            this.lciTitleName.TextToControlDistance = 0;
            this.lciTitleName.TextVisible = false;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // UCMediStockComboFilterByDepartmentCombo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UCMediStockComboFilterByDepartmentCombo";
            this.Size = new System.Drawing.Size(660, 48);
            this.Load += new System.EventHandler(this.UCMediStockComboFilterByDepartmentCombo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtRoomCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTitleName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtDepartmentCode;
        private DevExpress.XtraLayout.LayoutControlItem lciDepartment;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.GridLookUpEdit cboDepartment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.GridLookUpEdit cboRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.TextEdit txtRoomCode;
        private DevExpress.XtraLayout.LayoutControlItem lciMediStock;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.LabelControl lblTitleName;
        private DevExpress.XtraLayout.LayoutControlItem lciTitleName;
    }
}
