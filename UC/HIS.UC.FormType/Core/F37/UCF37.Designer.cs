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
namespace HIS.UC.FormType.Core.F37
{
    partial class UCF37
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCF37));
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gcSelected = new DevExpress.XtraGrid.GridControl();
            this.gvSelected = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcolSelectedCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolCodeSelect = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolNameSelect = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcAvailable = new DevExpress.XtraGrid.GridControl();
            this.gvAvailable = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcolCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnRemoveAll = new DevExpress.XtraEditors.SimpleButton();
            this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
            this.btnAddAll = new DevExpress.XtraEditors.SimpleButton();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lcAvailable = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcSelected = new DevExpress.XtraLayout.LayoutControlItem();
            this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcSelected)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSelected)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcAvailable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAvailable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcAvailable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSelected)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
            this.SuspendLayout();
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed_1);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gcSelected);
            this.layoutControl1.Controls.Add(this.gcAvailable);
            this.layoutControl1.Controls.Add(this.btnRemoveAll);
            this.layoutControl1.Controls.Add(this.btnRemove);
            this.layoutControl1.Controls.Add(this.btnAddAll);
            this.layoutControl1.Controls.Add(this.btnAdd);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(46, 122, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 220);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gcSelected
            // 
            this.gcSelected.Location = new System.Drawing.Point(390, 18);
            this.gcSelected.MainView = this.gvSelected;
            this.gcSelected.Name = "gcSelected";
            this.gcSelected.Size = new System.Drawing.Size(268, 200);
            this.gcSelected.TabIndex = 13;
            this.gcSelected.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvSelected});
            // 
            // gvSelected
            // 
            this.gvSelected.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcolSelectedCheck,
            this.gcolCodeSelect,
            this.gcolNameSelect});
            this.gvSelected.GridControl = this.gcSelected;
            this.gvSelected.Name = "gvSelected";
            this.gvSelected.OptionsView.ShowGroupPanel = false;
            this.gvSelected.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gvSelected_MouseDown);
            // 
            // gcolSelectedCheck
            // 
            this.gcolSelectedCheck.Caption = "gridColumn1";
            this.gcolSelectedCheck.FieldName = "IsChecked";
            this.gcolSelectedCheck.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gcolSelectedCheck.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.gcolSelectedCheck.Name = "gcolSelectedCheck";
            this.gcolSelectedCheck.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcolSelectedCheck.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcolSelectedCheck.Visible = true;
            this.gcolSelectedCheck.VisibleIndex = 0;
            this.gcolSelectedCheck.Width = 30;
            // 
            // gcolCodeSelect
            // 
            this.gcolCodeSelect.Caption = "Mã";
            this.gcolCodeSelect.FieldName = "CODE";
            this.gcolCodeSelect.Name = "gcolCodeSelect";
            this.gcolCodeSelect.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcolCodeSelect.Visible = true;
            this.gcolCodeSelect.VisibleIndex = 1;
            this.gcolCodeSelect.Width = 101;
            // 
            // gcolNameSelect
            // 
            this.gcolNameSelect.Caption = "Tên";
            this.gcolNameSelect.FieldName = "NAME";
            this.gcolNameSelect.Name = "gcolNameSelect";
            this.gcolNameSelect.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcolNameSelect.Visible = true;
            this.gcolNameSelect.VisibleIndex = 2;
            this.gcolNameSelect.Width = 104;
            // 
            // gcAvailable
            // 
            this.gcAvailable.Location = new System.Drawing.Point(92, 18);
            this.gcAvailable.MainView = this.gvAvailable;
            this.gcAvailable.Name = "gcAvailable";
            this.gcAvailable.Size = new System.Drawing.Size(265, 200);
            this.gcAvailable.TabIndex = 12;
            this.gcAvailable.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvAvailable});
            // 
            // gvAvailable
            // 
            this.gvAvailable.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcolCheck,
            this.gcolCode,
            this.gcolName});
            this.gvAvailable.GridControl = this.gcAvailable;
            this.gvAvailable.Name = "gvAvailable";
            this.gvAvailable.OptionsView.ShowGroupPanel = false;
            this.gvAvailable.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gvAvailable_MouseDown);
            // 
            // gcolCheck
            // 
            this.gcolCheck.FieldName = "IsChecked";
            this.gcolCheck.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gcolCheck.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.gcolCheck.Name = "gcolCheck";
            this.gcolCheck.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcolCheck.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcolCheck.Visible = true;
            this.gcolCheck.VisibleIndex = 0;
            this.gcolCheck.Width = 30;
            // 
            // gcolCode
            // 
            this.gcolCode.Caption = "Mã";
            this.gcolCode.FieldName = "CODE";
            this.gcolCode.Name = "gcolCode";
            this.gcolCode.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcolCode.Visible = true;
            this.gcolCode.VisibleIndex = 1;
            this.gcolCode.Width = 79;
            // 
            // gcolName
            // 
            this.gcolName.Caption = "Tên";
            this.gcolName.FieldName = "NAME";
            this.gcolName.Name = "gcolName";
            this.gcolName.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcolName.Visible = true;
            this.gcolName.VisibleIndex = 2;
            this.gcolName.Width = 153;
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(361, 138);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(25, 22);
            this.btnRemoveAll.StyleController = this.layoutControl1;
            this.btnRemoveAll.TabIndex = 11;
            this.btnRemoveAll.Text = "<<";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(361, 112);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(25, 22);
            this.btnRemove.StyleController = this.layoutControl1;
            this.btnRemove.TabIndex = 10;
            this.btnRemove.Text = "<";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(361, 86);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(25, 22);
            this.btnAddAll.StyleController = this.layoutControl1;
            this.btnAddAll.TabIndex = 9;
            this.btnAddAll.Text = ">>";
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(361, 60);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(25, 22);
            this.btnAdd.StyleController = this.layoutControl1;
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = ">";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.emptySpaceItem1,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.emptySpaceItem2,
            this.emptySpaceItem3,
            this.lcAvailable,
            this.lcSelected});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 220);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.emptySpaceItem1.Size = new System.Drawing.Size(90, 220);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnAdd;
            this.layoutControlItem2.Location = new System.Drawing.Point(359, 58);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(29, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnAddAll;
            this.layoutControlItem4.Location = new System.Drawing.Point(359, 84);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(29, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnRemove;
            this.layoutControlItem5.Location = new System.Drawing.Point(359, 110);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(29, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnRemoveAll;
            this.layoutControlItem6.Location = new System.Drawing.Point(359, 136);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(29, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(359, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(29, 58);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(359, 162);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(29, 58);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lcAvailable
            // 
            this.lcAvailable.Control = this.gcAvailable;
            this.lcAvailable.Location = new System.Drawing.Point(90, 0);
            this.lcAvailable.Name = "lcAvailable";
            this.lcAvailable.Size = new System.Drawing.Size(269, 220);
            this.lcAvailable.Text = "Danh sách";
            this.lcAvailable.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcAvailable.TextSize = new System.Drawing.Size(76, 13);
            // 
            // lcSelected
            // 
            this.lcSelected.Control = this.gcSelected;
            this.lcSelected.Location = new System.Drawing.Point(388, 0);
            this.lcSelected.Name = "lcSelected";
            this.lcSelected.Size = new System.Drawing.Size(272, 220);
            this.lcSelected.Text = "Danh sách chọn";
            this.lcSelected.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcSelected.TextSize = new System.Drawing.Size(76, 13);
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            this.imageCollection1.Images.SetKeyName(0, "dau tích-02.jpg");
            this.imageCollection1.Images.SetKeyName(1, "dau tích-01.jpg");
            // 
            // UCF37
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCF37";
            this.Size = new System.Drawing.Size(660, 220);
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcSelected)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSelected)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcAvailable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAvailable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcAvailable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSelected)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.SimpleButton btnRemoveAll;
        private DevExpress.XtraEditors.SimpleButton btnRemove;
        private DevExpress.XtraEditors.SimpleButton btnAddAll;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraGrid.GridControl gcSelected;
        private DevExpress.XtraGrid.Views.Grid.GridView gvSelected;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCodeSelect;
        private DevExpress.XtraGrid.Columns.GridColumn gcolNameSelect;
        private DevExpress.XtraGrid.GridControl gcAvailable;
        private DevExpress.XtraGrid.Views.Grid.GridView gvAvailable;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcolName;
        private DevExpress.XtraLayout.LayoutControlItem lcAvailable;
        private DevExpress.XtraLayout.LayoutControlItem lcSelected;
        private DevExpress.Utils.ImageCollection imageCollection1;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCheck;
        private DevExpress.XtraGrid.Columns.GridColumn gcolSelectedCheck;
    }
}
