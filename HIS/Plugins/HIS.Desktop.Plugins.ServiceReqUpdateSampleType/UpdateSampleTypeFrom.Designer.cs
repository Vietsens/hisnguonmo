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
namespace HIS.Desktop.Plugins.ServiceReqUpdateSampleType
{
    partial class UpdateSampleTypeFrom
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
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.BtnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.BtnSave = new DevExpress.XtraEditors.SimpleButton();
            this.CboTestSampleType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.LciTestSampleType = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barSave = new DevExpress.XtraBars.BarButtonItem();
            this.barPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            this.TxtTestSampleTypeCode = new DevExpress.XtraEditors.TextEdit();
            this.LciSampleTypeCode = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CboTestSampleType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciTestSampleType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtTestSampleTypeCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciSampleTypeCode)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.TxtTestSampleTypeCode);
            this.layoutControl1.Controls.Add(this.BtnPrint);
            this.layoutControl1.Controls.Add(this.BtnSave);
            this.layoutControl1.Controls.Add(this.CboTestSampleType);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(350, 80);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // BtnPrint
            // 
            this.BtnPrint.Location = new System.Drawing.Point(122, 46);
            this.BtnPrint.Name = "BtnPrint";
            this.BtnPrint.Size = new System.Drawing.Size(105, 22);
            this.BtnPrint.StyleController = this.layoutControl1;
            this.BtnPrint.TabIndex = 6;
            this.BtnPrint.Text = "In (Ctrl P)";
            this.BtnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(231, 46);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(107, 22);
            this.BtnSave.StyleController = this.layoutControl1;
            this.BtnSave.TabIndex = 5;
            this.BtnSave.Text = "Lưu (Ctrl S)";
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // CboTestSampleType
            // 
            this.CboTestSampleType.Location = new System.Drawing.Point(157, 12);
            this.CboTestSampleType.Name = "CboTestSampleType";
            this.CboTestSampleType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.CboTestSampleType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.CboTestSampleType.Properties.NullText = "";
            this.CboTestSampleType.Properties.View = this.gridLookUpEdit1View;
            this.CboTestSampleType.Size = new System.Drawing.Size(181, 20);
            this.CboTestSampleType.StyleController = this.layoutControl1;
            this.CboTestSampleType.TabIndex = 4;
            this.CboTestSampleType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.CboTestSampleType_Closed);
            this.CboTestSampleType.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.CboTestSampleType_ButtonClick);
            this.CboTestSampleType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CboTestSampleType_KeyDown);
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
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.LciTestSampleType,
            this.emptySpaceItem1,
            this.layoutControlItem3,
            this.layoutControlItem2,
            this.emptySpaceItem2,
            this.LciSampleTypeCode});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(350, 80);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // LciTestSampleType
            // 
            this.LciTestSampleType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciTestSampleType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciTestSampleType.Control = this.CboTestSampleType;
            this.LciTestSampleType.Location = new System.Drawing.Point(147, 0);
            this.LciTestSampleType.Name = "LciTestSampleType";
            this.LciTestSampleType.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.LciTestSampleType.Size = new System.Drawing.Size(183, 24);
            this.LciTestSampleType.Text = "Loại bệnh phẩm:";
            this.LciTestSampleType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciTestSampleType.TextSize = new System.Drawing.Size(0, 0);
            this.LciTestSampleType.TextToControlDistance = 0;
            this.LciTestSampleType.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 24);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(330, 10);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.BtnPrint;
            this.layoutControlItem3.Location = new System.Drawing.Point(110, 34);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(109, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.BtnSave;
            this.layoutControlItem2.Location = new System.Drawing.Point(219, 34);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(111, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 34);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(110, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
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
            this.barSave,
            this.barPrint});
            this.barManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.barPrint)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barSave
            // 
            this.barSave.Caption = "save";
            this.barSave.Id = 0;
            this.barSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barSave.Name = "barSave";
            this.barSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barSave_ItemClick);
            // 
            // barPrint
            // 
            this.barPrint.Caption = "print";
            this.barPrint.Id = 1;
            this.barPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.barPrint.Name = "barPrint";
            this.barPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barPrint_ItemClick);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControl1.Location = new System.Drawing.Point(0, 0);
            this.barDockControl1.Size = new System.Drawing.Size(350, 29);
            // 
            // barDockControl2
            // 
            this.barDockControl2.CausesValidation = false;
            this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl2.Location = new System.Drawing.Point(0, 109);
            this.barDockControl2.Size = new System.Drawing.Size(350, 0);
            // 
            // barDockControl3
            // 
            this.barDockControl3.CausesValidation = false;
            this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl3.Location = new System.Drawing.Point(0, 29);
            this.barDockControl3.Size = new System.Drawing.Size(0, 80);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl4.Location = new System.Drawing.Point(350, 29);
            this.barDockControl4.Size = new System.Drawing.Size(0, 80);
            // 
            // TxtTestSampleTypeCode
            // 
            this.TxtTestSampleTypeCode.Location = new System.Drawing.Point(107, 12);
            this.TxtTestSampleTypeCode.MenuManager = this.barManager1;
            this.TxtTestSampleTypeCode.Name = "TxtTestSampleTypeCode";
            this.TxtTestSampleTypeCode.Size = new System.Drawing.Size(50, 20);
            this.TxtTestSampleTypeCode.StyleController = this.layoutControl1;
            this.TxtTestSampleTypeCode.TabIndex = 7;
            this.TxtTestSampleTypeCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtTestSampleTypeCode_KeyDown);
            // 
            // LciSampleTypeCode
            // 
            this.LciSampleTypeCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciSampleTypeCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciSampleTypeCode.Control = this.TxtTestSampleTypeCode;
            this.LciSampleTypeCode.Location = new System.Drawing.Point(0, 0);
            this.LciSampleTypeCode.Name = "LciSampleTypeCode";
            this.LciSampleTypeCode.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.LciSampleTypeCode.Size = new System.Drawing.Size(147, 24);
            this.LciSampleTypeCode.Text = "Loại bệnh phẩm:";
            this.LciSampleTypeCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciSampleTypeCode.TextSize = new System.Drawing.Size(90, 20);
            this.LciSampleTypeCode.TextToControlDistance = 5;
            // 
            // UpdateSampleTypeFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 109);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControl3);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControl2);
            this.Controls.Add(this.barDockControl1);
            this.Name = "UpdateSampleTypeFrom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UpdateSampleTypeFrom";
            this.Load += new System.EventHandler(this.UpdateSampleTypeFrom_Load);
            this.Controls.SetChildIndex(this.barDockControl1, 0);
            this.Controls.SetChildIndex(this.barDockControl2, 0);
            this.Controls.SetChildIndex(this.barDockControl4, 0);
            this.Controls.SetChildIndex(this.barDockControl3, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CboTestSampleType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciTestSampleType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtTestSampleTypeCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciSampleTypeCode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.GridLookUpEdit CboTestSampleType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem LciTestSampleType;
        private DevExpress.XtraEditors.SimpleButton BtnPrint;
        private DevExpress.XtraEditors.SimpleButton BtnSave;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barSave;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraBars.BarButtonItem barPrint;
        private DevExpress.XtraEditors.TextEdit TxtTestSampleTypeCode;
        private DevExpress.XtraLayout.LayoutControlItem LciSampleTypeCode;
    }
}
