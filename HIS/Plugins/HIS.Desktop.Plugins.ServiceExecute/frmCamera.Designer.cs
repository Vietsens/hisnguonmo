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
namespace HIS.Desktop.Plugins.ServiceExecute
{
    partial class frmCamera
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
            DevExpress.XtraGrid.GridLevelNode gridLevelNode2 = new DevExpress.XtraGrid.GridLevelNode();
            DevExpress.Utils.ContextButton contextButton2 = new DevExpress.Utils.ContextButton();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCamera));
            DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement4 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
            DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement5 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
            DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement6 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.tileView1 = new DevExpress.XtraGrid.Views.Tile.TileView();
            this.tileViewColumn1 = new DevExpress.XtraGrid.Columns.TileViewColumn();
            this.repCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.tileViewColumn2 = new DevExpress.XtraGrid.Columns.TileViewColumn();
            this.repPic = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.tileViewColumn3 = new DevExpress.XtraGrid.Columns.TileViewColumn();
            this.tileViewColumn4 = new DevExpress.XtraGrid.Columns.TileViewColumn();
            this.tileViewColumn5 = new DevExpress.XtraGrid.Columns.TileViewColumn();
            this.repSpin = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.cardView1 = new DevExpress.XtraGrid.Views.Card.CardView();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnShowConfig = new DevExpress.XtraEditors.SimpleButton();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnCapture = new DevExpress.XtraEditors.SimpleButton();
            this.panelControlCamera = new DevExpress.XtraEditors.PanelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tileView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repSpin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cardView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlCamera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // gridControl1
            // 
            gridLevelNode2.LevelTemplate = this.gridView1;
            gridLevelNode2.RelationName = "Level1";
            this.gridControl1.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode2});
            this.gridControl1.Location = new System.Drawing.Point(884, 2);
            this.gridControl1.MainView = this.tileView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repCheck,
            this.repPic,
            this.repSpin});
            this.gridControl1.Size = new System.Drawing.Size(285, 682);
            this.gridControl1.TabIndex = 8;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.tileView1,
            this.cardView1,
            this.gridView1});
            // 
            // tileView1
            // 
            this.tileView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.tileViewColumn1,
            this.tileViewColumn2,
            this.tileViewColumn3,
            this.tileViewColumn4,
            this.tileViewColumn5});
            this.tileView1.ColumnSet.CheckedColumn = this.tileViewColumn1;
            contextButton2.Alignment = DevExpress.Utils.ContextItemAlignment.NearCenter;
            contextButton2.Glyph = ((System.Drawing.Image)(resources.GetObject("contextButton2.Glyph")));
            contextButton2.Id = new System.Guid("61296cd7-2b53-49d4-8733-8e6c1c707232");
            contextButton2.Name = "btnDelete";
            contextButton2.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
            this.tileView1.ContextButtons.Add(contextButton2);
            this.tileView1.GridControl = this.gridControl1;
            this.tileView1.Name = "tileView1";
            this.tileView1.OptionsTiles.ColumnCount = 1;
            this.tileView1.OptionsTiles.IndentBetweenGroups = 10;
            this.tileView1.OptionsTiles.ItemSize = new System.Drawing.Size(300, 230);
            this.tileView1.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tileView1.OptionsTiles.Padding = new System.Windows.Forms.Padding(5);
            this.tileView1.OptionsTiles.RowCount = 0;
            this.tileView1.OptionsTiles.ShowGroupText = false;
            tileViewItemElement4.Column = this.tileViewColumn2;
            tileViewItemElement4.Height = 20;
            tileViewItemElement4.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.Manual;
            tileViewItemElement4.ImageLocation = new System.Drawing.Point(-12, -8);
            tileViewItemElement4.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.Stretch;
            tileViewItemElement4.ImageSize = new System.Drawing.Size(300, 210);
            tileViewItemElement4.Text = "tileViewColumn2";
            tileViewItemElement5.Appearance.Normal.BackColor = System.Drawing.Color.Blue;
            tileViewItemElement5.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12F);
            tileViewItemElement5.Appearance.Normal.ForeColor = System.Drawing.Color.White;
            tileViewItemElement5.Appearance.Normal.Options.UseBackColor = true;
            tileViewItemElement5.Appearance.Normal.Options.UseFont = true;
            tileViewItemElement5.Appearance.Normal.Options.UseForeColor = true;
            tileViewItemElement5.Column = this.tileViewColumn3;
            tileViewItemElement5.Height = 25;
            tileViewItemElement5.StretchHorizontal = true;
            tileViewItemElement5.Text = "tileViewColumn3";
            tileViewItemElement5.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomCenter;
            tileViewItemElement5.TextLocation = new System.Drawing.Point(0, 7);
            tileViewItemElement6.Appearance.Normal.BackColor = System.Drawing.Color.Blue;
            tileViewItemElement6.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            tileViewItemElement6.Appearance.Normal.ForeColor = System.Drawing.Color.White;
            tileViewItemElement6.Appearance.Normal.Options.UseBackColor = true;
            tileViewItemElement6.Appearance.Normal.Options.UseFont = true;
            tileViewItemElement6.Appearance.Normal.Options.UseForeColor = true;
            tileViewItemElement6.Column = this.tileViewColumn5;
            tileViewItemElement6.Height = 20;
            tileViewItemElement6.ImageSize = new System.Drawing.Size(20, 30);
            tileViewItemElement6.Text = "tileViewColumn5";
            tileViewItemElement6.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
            tileViewItemElement6.TextLocation = new System.Drawing.Point(-10, -10);
            tileViewItemElement6.Width = 15;
            this.tileView1.TileTemplate.Add(tileViewItemElement4);
            this.tileView1.TileTemplate.Add(tileViewItemElement5);
            this.tileView1.TileTemplate.Add(tileViewItemElement6);
            this.tileView1.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tileView1_ItemClick);
            this.tileView1.ItemDoubleClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tileView1_ItemDoubleClick);
            this.tileView1.ItemRightClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tileView1_ItemRightClick);
            this.tileView1.ContextButtonClick += new DevExpress.Utils.ContextItemClickEventHandler(this.tileView1_ContextButtonClick);
            this.tileView1.ContextButtonCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewContextButtonCustomizeEventHandler(this.tileView1_ContextButtonCustomize);
            this.tileView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.tileView1_CustomUnboundColumnData);
            this.tileView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tileView1_KeyDown);
            // 
            // tileViewColumn1
            // 
            this.tileViewColumn1.Caption = "Chọn";
            this.tileViewColumn1.ColumnEdit = this.repCheck;
            this.tileViewColumn1.FieldName = "IsChecked";
            this.tileViewColumn1.Name = "tileViewColumn1";
            this.tileViewColumn1.OptionsColumn.ShowInCustomizationForm = false;
            this.tileViewColumn1.OptionsColumn.ShowInExpressionEditor = false;
            this.tileViewColumn1.OptionsFilter.AllowAutoFilter = false;
            this.tileViewColumn1.OptionsFilter.AllowFilter = false;
            this.tileViewColumn1.Visible = true;
            this.tileViewColumn1.VisibleIndex = 0;
            // 
            // repCheck
            // 
            this.repCheck.AutoHeight = false;
            this.repCheck.Name = "repCheck";
            // 
            // tileViewColumn2
            // 
            this.tileViewColumn2.Caption = "Ảnh";
            this.tileViewColumn2.ColumnEdit = this.repPic;
            this.tileViewColumn2.FieldName = "IMAGE_DISPLAY";
            this.tileViewColumn2.Name = "tileViewColumn2";
            this.tileViewColumn2.OptionsColumn.AllowEdit = false;
            this.tileViewColumn2.Visible = true;
            this.tileViewColumn2.VisibleIndex = 1;
            // 
            // repPic
            // 
            this.repPic.Name = "repPic";
            // 
            // tileViewColumn3
            // 
            this.tileViewColumn3.Caption = "Tên";
            this.tileViewColumn3.FieldName = "ShowName";
            this.tileViewColumn3.Name = "tileViewColumn3";
            this.tileViewColumn3.OptionsColumn.AllowEdit = false;
            this.tileViewColumn3.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.tileViewColumn3.Visible = true;
            this.tileViewColumn3.VisibleIndex = 2;
            // 
            // tileViewColumn4
            // 
            this.tileViewColumn4.Caption = "tileViewColumn4";
            this.tileViewColumn4.FieldName = "Tile";
            this.tileViewColumn4.Name = "tileViewColumn4";
            this.tileViewColumn4.OptionsColumn.AllowEdit = false;
            this.tileViewColumn4.Visible = true;
            this.tileViewColumn4.VisibleIndex = 3;
            // 
            // tileViewColumn5
            // 
            this.tileViewColumn5.Caption = "tileViewColumn5";
            this.tileViewColumn5.ColumnEdit = this.repSpin;
            this.tileViewColumn5.FieldName = "STTImage";
            this.tileViewColumn5.Name = "tileViewColumn5";
            this.tileViewColumn5.OptionsColumn.ShowInCustomizationForm = false;
            this.tileViewColumn5.OptionsColumn.ShowInExpressionEditor = false;
            this.tileViewColumn5.OptionsFilter.AllowAutoFilter = false;
            this.tileViewColumn5.OptionsFilter.AllowFilter = false;
            this.tileViewColumn5.Visible = true;
            this.tileViewColumn5.VisibleIndex = 4;
            // 
            // repSpin
            // 
            this.repSpin.AutoHeight = false;
            this.repSpin.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repSpin.Name = "repSpin";
            // 
            // cardView1
            // 
            this.cardView1.FocusedCardTopFieldIndex = 0;
            this.cardView1.GridControl = this.gridControl1;
            this.cardView1.Name = "cardView1";
            this.cardView1.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Auto;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Controls.Add(this.btnShowConfig);
            this.layoutControl1.Controls.Add(this.btnDelete);
            this.layoutControl1.Controls.Add(this.btnCapture);
            this.layoutControl1.Controls.Add(this.panelControlCamera);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1171, 712);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnShowConfig
            // 
            this.btnShowConfig.Image = ((System.Drawing.Image)(resources.GetObject("btnShowConfig.Image")));
            this.btnShowConfig.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnShowConfig.Location = new System.Drawing.Point(841, 688);
            this.btnShowConfig.Name = "btnShowConfig";
            this.btnShowConfig.Size = new System.Drawing.Size(328, 22);
            this.btnShowConfig.StyleController = this.layoutControl1;
            this.btnShowConfig.TabIndex = 7;
            this.btnShowConfig.ToolTip = "Tùy chỉnh ẩn/hiện thuộc tính Camera";
            this.btnShowConfig.Click += new System.EventHandler(this.btnShowConfig_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Image = global::HIS.Desktop.Plugins.ServiceExecute.Properties.Resources.trash_can_16x16;
            this.btnDelete.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDelete.Location = new System.Drawing.Point(402, 688);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(435, 22);
            this.btnDelete.StyleController = this.layoutControl1;
            this.btnDelete.TabIndex = 6;
            this.btnDelete.ToolTip = "Xóa ảnh";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCapture
            // 
            this.btnCapture.Image = global::HIS.Desktop.Plugins.ServiceExecute.Properties.Resources.webcam_16x16;
            this.btnCapture.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCapture.Location = new System.Drawing.Point(2, 688);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(396, 22);
            this.btnCapture.StyleController = this.layoutControl1;
            this.btnCapture.TabIndex = 5;
            this.btnCapture.ToolTip = "Chụp hình";
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // panelControlCamera
            // 
            this.panelControlCamera.Location = new System.Drawing.Point(2, 2);
            this.panelControlCamera.Name = "panelControlCamera";
            this.panelControlCamera.Size = new System.Drawing.Size(878, 682);
            this.panelControlCamera.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1171, 712);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.panelControlCamera;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(882, 686);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnCapture;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 686);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(400, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnDelete;
            this.layoutControlItem3.Location = new System.Drawing.Point(400, 686);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(439, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnShowConfig;
            this.layoutControlItem4.Location = new System.Drawing.Point(839, 686);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(332, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.gridControl1;
            this.layoutControlItem5.Location = new System.Drawing.Point(882, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(289, 686);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            this.layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.MaxItemId = 0;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1171, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 741);
            this.barDockControlBottom.Size = new System.Drawing.Size(1171, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 712);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1171, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 712);
            // 
            // frmCamera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1171, 741);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmCamera";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCamera_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCamera_FormClosed);
            this.Load += new System.EventHandler(this.frmCamera_Load);
            this.ResizeBegin += new System.EventHandler(this.frmCamera_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.frmCamera_ResizeEnd);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tileView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repSpin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cardView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControlCamera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnShowConfig;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraEditors.SimpleButton btnCapture;
        private DevExpress.XtraEditors.PanelControl panelControlCamera;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraGrid.Views.Tile.TileView tileView1;
        private DevExpress.XtraGrid.Views.Card.CardView cardView1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private DevExpress.XtraGrid.Columns.TileViewColumn tileViewColumn1;
        private DevExpress.XtraGrid.Columns.TileViewColumn tileViewColumn2;
        private DevExpress.XtraGrid.Columns.TileViewColumn tileViewColumn3;
        private DevExpress.XtraGrid.Columns.TileViewColumn tileViewColumn4;
        private DevExpress.XtraGrid.Columns.TileViewColumn tileViewColumn5;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repCheck;
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repPic;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repSpin;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}
