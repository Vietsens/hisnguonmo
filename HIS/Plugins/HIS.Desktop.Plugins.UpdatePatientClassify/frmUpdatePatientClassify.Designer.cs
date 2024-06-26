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
namespace HIS.Desktop.Plugins.UpdatePatientClassify
{
    partial class frmUpdatePatientClassify
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.btnSave = new DevExpress.XtraEditors.SimpleButton();
			this.cboPatientClassify = new DevExpress.XtraEditors.GridLookUpEdit();
			this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.barManager1 = new DevExpress.XtraBars.BarManager();
			this.bar1 = new DevExpress.XtraBars.Bar();
			this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
			this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
			this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
			this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
			this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cboPatientClassify.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// layoutControl1
			// 
			this.layoutControl1.Controls.Add(this.btnSave);
			this.layoutControl1.Controls.Add(this.cboPatientClassify);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 29);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(485, 44);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "layoutControl1";
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(366, 36);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(95, 22);
			this.btnSave.StyleController = this.layoutControl1;
			this.btnSave.TabIndex = 5;
			this.btnSave.Text = "Lưu (Ctrl S)";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// cboPatientClassify
			// 
			this.cboPatientClassify.Location = new System.Drawing.Point(111, 7);
			this.cboPatientClassify.Name = "cboPatientClassify";
			this.cboPatientClassify.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
			this.cboPatientClassify.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
			this.cboPatientClassify.Properties.NullText = "";
			this.cboPatientClassify.Properties.PopupSizeable = false;
			this.cboPatientClassify.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.cboPatientClassify.Properties.View = this.gridLookUpEdit1View;
			this.cboPatientClassify.Size = new System.Drawing.Size(350, 20);
			this.cboPatientClassify.StyleController = this.layoutControl1;
			this.cboPatientClassify.TabIndex = 4;
			this.cboPatientClassify.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPatientClassify_ButtonClick);
			// 
			// gridLookUpEdit1View
			// 
			this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
			this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
			this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
			// 
			// layoutControlGroup1
			// 
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Size = new System.Drawing.Size(468, 60);
			this.layoutControlGroup1.TextVisible = false;
			// 
			// layoutControlItem1
			// 
			this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.layoutControlItem1.Control = this.cboPatientClassify;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(468, 34);
			this.layoutControlItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
			this.layoutControlItem1.Text = "Phân loại bệnh nhân:";
			this.layoutControlItem1.TextSize = new System.Drawing.Size(101, 13);
			// 
			// layoutControlItem2
			// 
			this.layoutControlItem2.Control = this.btnSave;
			this.layoutControlItem2.Location = new System.Drawing.Point(364, 34);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(104, 26);
			this.layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 5, 0, 0);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			// 
			// emptySpaceItem1
			// 
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 34);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(364, 26);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			// 
			// barManager1
			// 
			this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
			this.barManager1.DockControls.Add(this.barDockControl1);
			this.barManager1.DockControls.Add(this.barDockControl2);
			this.barManager1.DockControls.Add(this.barDockControl3);
			this.barManager1.DockControls.Add(this.barDockControl4);
			this.barManager1.Form = this;
			this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem1});
			this.barManager1.MaxItemId = 1;
			// 
			// bar1
			// 
			this.bar1.BarName = "Tools";
			this.bar1.DockCol = 0;
			this.bar1.DockRow = 0;
			this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1)});
			this.bar1.Text = "Tools";
			this.bar1.Visible = false;
			// 
			// barButtonItem1
			// 
			this.barButtonItem1.Caption = "btnSave";
			this.barButtonItem1.Id = 0;
			this.barButtonItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
			this.barButtonItem1.Name = "barButtonItem1";
			this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
			// 
			// barDockControl1
			// 
			this.barDockControl1.CausesValidation = false;
			this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.barDockControl1.Location = new System.Drawing.Point(0, 0);
			this.barDockControl1.Size = new System.Drawing.Size(485, 29);
			// 
			// barDockControl2
			// 
			this.barDockControl2.CausesValidation = false;
			this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barDockControl2.Location = new System.Drawing.Point(0, 73);
			this.barDockControl2.Size = new System.Drawing.Size(485, 0);
			// 
			// barDockControl3
			// 
			this.barDockControl3.CausesValidation = false;
			this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
			this.barDockControl3.Location = new System.Drawing.Point(0, 29);
			this.barDockControl3.Size = new System.Drawing.Size(0, 44);
			// 
			// barDockControl4
			// 
			this.barDockControl4.CausesValidation = false;
			this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
			this.barDockControl4.Location = new System.Drawing.Point(485, 29);
			this.barDockControl4.Size = new System.Drawing.Size(0, 44);
			// 
			// dxValidationProvider1
			// 
			this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
			// 
			// frmUpdatePatientClassify
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(485, 73);
			this.Controls.Add(this.layoutControl1);
			this.Controls.Add(this.barDockControl3);
			this.Controls.Add(this.barDockControl4);
			this.Controls.Add(this.barDockControl2);
			this.Controls.Add(this.barDockControl1);
			this.Name = "frmUpdatePatientClassify";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Cập nhật thông tin phân loại bệnh nhân";
			this.Load += new System.EventHandler(this.frmUpdatePatientClassify_Load);
			this.Controls.SetChildIndex(this.barDockControl1, 0);
			this.Controls.SetChildIndex(this.barDockControl2, 0);
			this.Controls.SetChildIndex(this.barDockControl4, 0);
			this.Controls.SetChildIndex(this.barDockControl3, 0);
			this.Controls.SetChildIndex(this.layoutControl1, 0);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cboPatientClassify.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.GridLookUpEdit cboPatientClassify;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
		private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
	}
}
