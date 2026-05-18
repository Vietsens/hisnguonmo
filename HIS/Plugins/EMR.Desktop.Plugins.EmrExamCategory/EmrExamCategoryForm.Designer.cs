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
namespace EMR.Desktop.Plugins.EmrExamCategory
{
    partial class EmrExamCategoryForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject13 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject14 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject15 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject16 = new DevExpress.Utils.SerializableAppearanceObject();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmrExamCategoryForm));
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.bbtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.pnlBottom = new DevExpress.XtraEditors.PanelControl();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.splitMain = new DevExpress.XtraEditors.SplitContainerControl();
            // Category side
            this.pnlCatHeader = new DevExpress.XtraEditors.PanelControl();
            this.lblCatTitle = new DevExpress.XtraEditors.LabelControl();
            this.btnCatAdd = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlCat = new DevExpress.XtraGrid.GridControl();
            this.gridViewCat = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcolCatNumOrder = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolCatDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolCatCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolCatName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolCatCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolCatCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolCatModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolCatModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnCatDeleteEnable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnCatDeleteDisable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.riCatNumOrder = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            // Rule side
            this.pnlRuleHeader = new DevExpress.XtraEditors.PanelControl();
            this.lblRuleTitle = new DevExpress.XtraEditors.LabelControl();
            this.btnRuleAdd = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlRule = new DevExpress.XtraGrid.GridControl();
            this.gridViewRule = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcolRuleNumOrder = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolRuleDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolRulePattern = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolRuleMatchType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolRuleKeyExtractor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolRuleExamCategory = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolRuleCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolRuleCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolRuleModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolRuleModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnRuleDeleteEnable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnRuleDeleteDisable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.riRuleNumOrder = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.riRuleMatchType = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.riRuleExamCategoryId = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            // BeginInit
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).BeginInit();
            this.pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlCatHeader)).BeginInit();
            this.pnlCatHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlCat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCatDeleteEnable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCatDeleteDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riCatNumOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlRuleHeader)).BeginInit();
            this.pnlRuleHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlRule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRuleDeleteEnable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRuleDeleteDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riRuleNumOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riRuleMatchType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riRuleExamCategoryId)).BeginInit();
            this.SuspendLayout();
            //
            // barManager1
            //
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnSave,
            this.bbtnRefresh});
            this.barManager1.MaxItemId = 2;
            //
            // bar2
            //
            this.bar2.BarName = "Tools";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRefresh)});
            this.bar2.Text = "Tools";
            this.bar2.Visible = false;
            //
            // bbtnSave
            //
            this.bbtnSave.Caption = "Lưu (Ctrl S)";
            this.bbtnSave.Id = 0;
            this.bbtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnSave.Name = "bbtnSave";
            this.bbtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSave_ItemClick);
            //
            // bbtnRefresh
            //
            this.bbtnRefresh.Caption = "Làm mới (Ctrl R)";
            this.bbtnRefresh.Id = 1;
            this.bbtnRefresh.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
            this.bbtnRefresh.Name = "bbtnRefresh";
            this.bbtnRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRefresh_ItemClick);
            //
            // barDockControlTop
            //
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1200, 0);
            //
            // barDockControlBottom
            //
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 650);
            this.barDockControlBottom.Size = new System.Drawing.Size(1200, 0);
            //
            // barDockControlLeft
            //
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 650);
            //
            // barDockControlRight
            //
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1200, 0);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 650);
            //
            // pnlBottom
            //
            this.pnlBottom.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlBottom.Controls.Add(this.btnSave);
            this.pnlBottom.Controls.Add(this.btnRefresh);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 756);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1900, 44);
            this.pnlBottom.TabIndex = 10;
            //
            // btnRefresh
            //
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(1693, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(95, 28);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(1797, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(95, 28);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Lưu";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // =============================================
            // SPLIT MAIN
            // =============================================
            //
            // splitMain
            //
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.Controls.Add(this.gridControlCat);
            this.splitMain.Panel1.Controls.Add(this.pnlCatHeader);
            this.splitMain.Panel1.Text = "Panel1";
            this.splitMain.Panel2.Controls.Add(this.gridControlRule);
            this.splitMain.Panel2.Controls.Add(this.pnlRuleHeader);
            this.splitMain.Panel2.Text = "Panel2";
            this.splitMain.Size = new System.Drawing.Size(1900, 756);
            this.splitMain.SplitterPosition = 946;
            this.splitMain.IsSplitterFixed = true;
            this.splitMain.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
            this.splitMain.TabIndex = 11;
            this.splitMain.Text = "splitMain";
            // =============================================
            // LEFT: CATEGORY
            // =============================================
            //
            // pnlCatHeader
            //
            this.pnlCatHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlCatHeader.Controls.Add(this.lblCatTitle);
            this.pnlCatHeader.Controls.Add(this.btnCatAdd);
            this.pnlCatHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCatHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlCatHeader.Name = "pnlCatHeader";
            this.pnlCatHeader.Size = new System.Drawing.Size(946, 32);
            this.pnlCatHeader.TabIndex = 1;
            //
            // lblCatTitle
            //
            this.lblCatTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblCatTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCatTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCatTitle.Location = new System.Drawing.Point(0, 0);
            this.lblCatTitle.Name = "lblCatTitle";
            this.lblCatTitle.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblCatTitle.Size = new System.Drawing.Size(914, 32);
            this.lblCatTitle.TabIndex = 0;
            this.lblCatTitle.Text = "Loại xét nghiệm";
            //
            // btnCatAdd
            //
            this.btnCatAdd.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCatAdd.Location = new System.Drawing.Point(914, 0);
            this.btnCatAdd.Name = "btnCatAdd";
            this.btnCatAdd.Size = new System.Drawing.Size(32, 32);
            this.btnCatAdd.TabIndex = 1;
            this.btnCatAdd.ToolTip = "Thêm loại xét nghiệm";
            this.btnCatAdd.Click += new System.EventHandler(this.btnCatAdd_Click);
            //
            // gridControlCat
            //
            this.gridControlCat.AllowDrop = true;
            this.gridControlCat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlCat.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            this.gridControlCat.Location = new System.Drawing.Point(0, 32);
            this.gridControlCat.MainView = this.gridViewCat;
            this.gridControlCat.MenuManager = this.barManager1;
            this.gridControlCat.Name = "gridControlCat";
            this.gridControlCat.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btnCatDeleteEnable,
            this.btnCatDeleteDisable,
            this.riCatNumOrder});
            this.gridControlCat.Size = new System.Drawing.Size(946, 724);
            this.gridControlCat.TabIndex = 0;
            this.gridControlCat.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewCat});
            this.gridControlCat.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridControlCat_MouseDown);
            this.gridControlCat.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridControlCat_MouseMove);
            this.gridControlCat.DragOver += new System.Windows.Forms.DragEventHandler(this.gridControlCat_DragOver);
            this.gridControlCat.DragDrop += new System.Windows.Forms.DragEventHandler(this.gridControlCat_DragDrop);
            //
            // gridViewCat
            //
            this.gridViewCat.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcolCatNumOrder,
            this.gcolCatDelete,
            this.gcolCatCode,
            this.gcolCatName,
            this.gcolCatCreateTime,
            this.gcolCatCreator,
            this.gcolCatModifyTime,
            this.gcolCatModifier});
            this.gridViewCat.GridControl = this.gridControlCat;
            this.gridViewCat.Name = "gridViewCat";
            this.gridViewCat.OptionsCustomization.AllowFilter = false;
            this.gridViewCat.OptionsView.ColumnAutoWidth = false;
            this.gridViewCat.OptionsView.ShowGroupPanel = false;
            this.gridViewCat.OptionsView.ShowIndicator = false;
            this.gridViewCat.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewCat_CustomRowCellEdit);
            this.gridViewCat.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewCat_CustomUnboundColumnData);
            this.gridViewCat.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewCat_CellValueChanged);
            //
            // gcolCatNumOrder
            //
            this.gcolCatNumOrder.Caption = "Thứ tự";
            this.gcolCatNumOrder.ColumnEdit = this.riCatNumOrder;
            this.gcolCatNumOrder.FieldName = "NUM_ORDER";
            this.gcolCatNumOrder.Name = "gcolCatNumOrder";
            this.gcolCatNumOrder.OptionsFilter.AllowFilter = false;
            this.gcolCatNumOrder.Visible = true;
            this.gcolCatNumOrder.VisibleIndex = 0;
            this.gcolCatNumOrder.Width = 60;
            //
            // gcolCatDelete
            //
            this.gcolCatDelete.AppearanceCell.Options.UseTextOptions = true;
            this.gcolCatDelete.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcolCatDelete.FieldName = "CatDelete";
            this.gcolCatDelete.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.gcolCatDelete.Name = "gcolCatDelete";
            this.gcolCatDelete.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcolCatDelete.OptionsColumn.ShowCaption = false;
            this.gcolCatDelete.OptionsFilter.AllowFilter = false;
            this.gcolCatDelete.Visible = true;
            this.gcolCatDelete.VisibleIndex = 1;
            this.gcolCatDelete.Width = 30;
            //
            // gcolCatCode
            //
            this.gcolCatCode.Caption = "Mã";
            this.gcolCatCode.FieldName = "CATEGORY_CODE";
            this.gcolCatCode.Name = "gcolCatCode";
            this.gcolCatCode.OptionsFilter.AllowFilter = false;
            this.gcolCatCode.Visible = true;
            this.gcolCatCode.VisibleIndex = 2;
            this.gcolCatCode.Width = 120;
            //
            // gcolCatName
            //
            this.gcolCatName.Caption = "Tên loại xét nghiệm";
            this.gcolCatName.FieldName = "CATEGORY_NAME";
            this.gcolCatName.Name = "gcolCatName";
            this.gcolCatName.OptionsFilter.AllowFilter = false;
            this.gcolCatName.Visible = true;
            this.gcolCatName.VisibleIndex = 3;
            this.gcolCatName.Width = 200;
            //
            // gcolCatCreateTime
            //
            this.gcolCatCreateTime.Caption = "Thời gian tạo";
            this.gcolCatCreateTime.FieldName = "CAT_CREATE_TIME_STR";
            this.gcolCatCreateTime.Name = "gcolCatCreateTime";
            this.gcolCatCreateTime.OptionsColumn.AllowEdit = false;
            this.gcolCatCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcolCatCreateTime.Visible = true;
            this.gcolCatCreateTime.VisibleIndex = 4;
            this.gcolCatCreateTime.Width = 120;
            //
            // gcolCatCreator
            //
            this.gcolCatCreator.Caption = "Người tạo";
            this.gcolCatCreator.FieldName = "CREATOR";
            this.gcolCatCreator.Name = "gcolCatCreator";
            this.gcolCatCreator.OptionsColumn.AllowEdit = false;
            this.gcolCatCreator.OptionsFilter.AllowFilter = false;
            this.gcolCatCreator.Visible = true;
            this.gcolCatCreator.VisibleIndex = 5;
            this.gcolCatCreator.Width = 90;
            //
            // gcolCatModifyTime
            //
            this.gcolCatModifyTime.Caption = "Thời gian sửa";
            this.gcolCatModifyTime.FieldName = "CAT_MODIFY_TIME_STR";
            this.gcolCatModifyTime.Name = "gcolCatModifyTime";
            this.gcolCatModifyTime.OptionsColumn.AllowEdit = false;
            this.gcolCatModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcolCatModifyTime.Visible = true;
            this.gcolCatModifyTime.VisibleIndex = 6;
            this.gcolCatModifyTime.Width = 120;
            //
            // gcolCatModifier
            //
            this.gcolCatModifier.Caption = "Người sửa";
            this.gcolCatModifier.FieldName = "MODIFIER";
            this.gcolCatModifier.Name = "gcolCatModifier";
            this.gcolCatModifier.OptionsColumn.AllowEdit = false;
            this.gcolCatModifier.OptionsFilter.AllowFilter = false;
            this.gcolCatModifier.Visible = true;
            this.gcolCatModifier.VisibleIndex = 7;
            this.gcolCatModifier.Width = 90;
            //
            // btnCatDeleteEnable
            //
            this.btnCatDeleteEnable.AutoHeight = false;
            this.btnCatDeleteEnable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnCatDeleteEnable.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Xóa", null, null, true)});
            this.btnCatDeleteEnable.Name = "btnCatDeleteEnable";
            this.btnCatDeleteEnable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnCatDeleteEnable.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnCatDeleteEnable_Click);
            //
            // btnCatDeleteDisable
            //
            this.btnCatDeleteDisable.AutoHeight = false;
            this.btnCatDeleteDisable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, false, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnCatDeleteEnable.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, true)});
            this.btnCatDeleteDisable.Name = "btnCatDeleteDisable";
            this.btnCatDeleteDisable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            //
            // riCatNumOrder
            //
            this.riCatNumOrder.AutoHeight = false;
            this.riCatNumOrder.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.riCatNumOrder.IsFloatValue = false;
            this.riCatNumOrder.MaxValue = new decimal(new int[] {9999, 0, 0, 0});
            this.riCatNumOrder.MinValue = new decimal(new int[] {1, 0, 0, 0});
            this.riCatNumOrder.Name = "riCatNumOrder";
            // =============================================
            // RIGHT: RULE
            // =============================================
            //
            // pnlRuleHeader
            //
            this.pnlRuleHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlRuleHeader.Controls.Add(this.lblRuleTitle);
            this.pnlRuleHeader.Controls.Add(this.btnRuleAdd);
            this.pnlRuleHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRuleHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlRuleHeader.Name = "pnlRuleHeader";
            this.pnlRuleHeader.Size = new System.Drawing.Size(946, 32);
            this.pnlRuleHeader.TabIndex = 1;
            //
            // lblRuleTitle
            //
            this.lblRuleTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblRuleTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblRuleTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRuleTitle.Location = new System.Drawing.Point(0, 0);
            this.lblRuleTitle.Name = "lblRuleTitle";
            this.lblRuleTitle.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblRuleTitle.Size = new System.Drawing.Size(914, 32);
            this.lblRuleTitle.TabIndex = 0;
            this.lblRuleTitle.Text = "Cấu hình ghép cặp văn bản";
            //
            // btnRuleAdd
            //
            this.btnRuleAdd.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRuleAdd.Location = new System.Drawing.Point(914, 0);
            this.btnRuleAdd.Name = "btnRuleAdd";
            this.btnRuleAdd.Size = new System.Drawing.Size(32, 32);
            this.btnRuleAdd.TabIndex = 1;
            this.btnRuleAdd.ToolTip = "Thêm rule ghép cặp";
            this.btnRuleAdd.Click += new System.EventHandler(this.btnRuleAdd_Click);
            //
            // gridControlRule
            //
            this.gridControlRule.AllowDrop = true;
            this.gridControlRule.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlRule.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            this.gridControlRule.Location = new System.Drawing.Point(0, 32);
            this.gridControlRule.MainView = this.gridViewRule;
            this.gridControlRule.MenuManager = this.barManager1;
            this.gridControlRule.Name = "gridControlRule";
            this.gridControlRule.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btnRuleDeleteEnable,
            this.btnRuleDeleteDisable,
            this.riRuleNumOrder,
            this.riRuleMatchType,
            this.riRuleExamCategoryId});
            this.gridControlRule.Size = new System.Drawing.Size(946, 724);
            this.gridControlRule.TabIndex = 0;
            this.gridControlRule.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewRule});
            this.gridControlRule.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridControlRule_MouseDown);
            this.gridControlRule.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridControlRule_MouseMove);
            this.gridControlRule.DragOver += new System.Windows.Forms.DragEventHandler(this.gridControlRule_DragOver);
            this.gridControlRule.DragDrop += new System.Windows.Forms.DragEventHandler(this.gridControlRule_DragDrop);
            //
            // gridViewRule
            //
            this.gridViewRule.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcolRuleNumOrder,
            this.gcolRuleDelete,
            this.gcolRulePattern,
            this.gcolRuleMatchType,
            this.gcolRuleKeyExtractor,
            this.gcolRuleExamCategory,
            this.gcolRuleCreateTime,
            this.gcolRuleCreator,
            this.gcolRuleModifyTime,
            this.gcolRuleModifier});
            this.gridViewRule.GridControl = this.gridControlRule;
            this.gridViewRule.Name = "gridViewRule";
            this.gridViewRule.OptionsCustomization.AllowFilter = false;
            this.gridViewRule.OptionsView.ColumnAutoWidth = false;
            this.gridViewRule.OptionsView.ShowGroupPanel = false;
            this.gridViewRule.OptionsView.ShowIndicator = false;
            this.gridViewRule.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewRule_CustomRowCellEdit);
            this.gridViewRule.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewRule_CustomUnboundColumnData);
            this.gridViewRule.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewRule_CellValueChanged);
            //
            // gcolRuleNumOrder
            //
            this.gcolRuleNumOrder.Caption = "Ưu tiên";
            this.gcolRuleNumOrder.ColumnEdit = this.riRuleNumOrder;
            this.gcolRuleNumOrder.FieldName = "NUM_ORDER";
            this.gcolRuleNumOrder.Name = "gcolRuleNumOrder";
            this.gcolRuleNumOrder.OptionsColumn.AllowEdit = true;
            this.gcolRuleNumOrder.OptionsColumn.AllowFocus = true;
            this.gcolRuleNumOrder.OptionsColumn.ReadOnly = false;
            this.gcolRuleNumOrder.OptionsFilter.AllowFilter = false;
            this.gcolRuleNumOrder.Visible = true;
            this.gcolRuleNumOrder.VisibleIndex = 0;
            this.gcolRuleNumOrder.Width = 60;
            //
            // gcolRuleDelete
            //
            this.gcolRuleDelete.AppearanceCell.Options.UseTextOptions = true;
            this.gcolRuleDelete.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcolRuleDelete.FieldName = "RuleDelete";
            this.gcolRuleDelete.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.gcolRuleDelete.Name = "gcolRuleDelete";
            this.gcolRuleDelete.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcolRuleDelete.OptionsColumn.ShowCaption = false;
            this.gcolRuleDelete.OptionsFilter.AllowFilter = false;
            this.gcolRuleDelete.Visible = true;
            this.gcolRuleDelete.VisibleIndex = 1;
            this.gcolRuleDelete.Width = 30;
            //
            // gcolRulePattern
            //
            this.gcolRulePattern.Caption = "Pattern";
            this.gcolRulePattern.FieldName = "PATTERN";
            this.gcolRulePattern.Name = "gcolRulePattern";
            this.gcolRulePattern.OptionsFilter.AllowFilter = false;
            this.gcolRulePattern.Visible = true;
            this.gcolRulePattern.VisibleIndex = 2;
            this.gcolRulePattern.Width = 130;
            //
            // gcolRuleMatchType
            //
            this.gcolRuleMatchType.Caption = "Kiểu match";
            this.gcolRuleMatchType.ColumnEdit = this.riRuleMatchType;
            this.gcolRuleMatchType.FieldName = "MATCH_TYPE";
            this.gcolRuleMatchType.Name = "gcolRuleMatchType";
            this.gcolRuleMatchType.OptionsFilter.AllowFilter = false;
            this.gcolRuleMatchType.Visible = true;
            this.gcolRuleMatchType.VisibleIndex = 3;
            this.gcolRuleMatchType.Width = 90;
            //
            // gcolRuleKeyExtractor
            //
            this.gcolRuleKeyExtractor.Caption = "Extract key";
            this.gcolRuleKeyExtractor.FieldName = "KEY_EXTRACTOR";
            this.gcolRuleKeyExtractor.Name = "gcolRuleKeyExtractor";
            this.gcolRuleKeyExtractor.OptionsFilter.AllowFilter = false;
            this.gcolRuleKeyExtractor.Visible = true;
            this.gcolRuleKeyExtractor.VisibleIndex = 4;
            this.gcolRuleKeyExtractor.Width = 120;
            //
            // gcolRuleExamCategory
            //
            this.gcolRuleExamCategory.Caption = "Loại xét nghiệm";
            this.gcolRuleExamCategory.ColumnEdit = this.riRuleExamCategoryId;
            this.gcolRuleExamCategory.FieldName = "EXAM_CATEGORY_ID";
            this.gcolRuleExamCategory.Name = "gcolRuleExamCategory";
            this.gcolRuleExamCategory.OptionsFilter.AllowFilter = false;
            this.gcolRuleExamCategory.Visible = true;
            this.gcolRuleExamCategory.VisibleIndex = 5;
            this.gcolRuleExamCategory.Width = 140;
            //
            // gcolRuleCreateTime
            //
            this.gcolRuleCreateTime.Caption = "Thời gian tạo";
            this.gcolRuleCreateTime.FieldName = "RULE_CREATE_TIME_STR";
            this.gcolRuleCreateTime.Name = "gcolRuleCreateTime";
            this.gcolRuleCreateTime.OptionsColumn.AllowEdit = false;
            this.gcolRuleCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcolRuleCreateTime.Visible = true;
            this.gcolRuleCreateTime.VisibleIndex = 6;
            this.gcolRuleCreateTime.Width = 120;
            //
            // gcolRuleCreator
            //
            this.gcolRuleCreator.Caption = "Người tạo";
            this.gcolRuleCreator.FieldName = "CREATOR";
            this.gcolRuleCreator.Name = "gcolRuleCreator";
            this.gcolRuleCreator.OptionsColumn.AllowEdit = false;
            this.gcolRuleCreator.OptionsFilter.AllowFilter = false;
            this.gcolRuleCreator.Visible = true;
            this.gcolRuleCreator.VisibleIndex = 7;
            this.gcolRuleCreator.Width = 90;
            //
            // gcolRuleModifyTime
            //
            this.gcolRuleModifyTime.Caption = "Thời gian sửa";
            this.gcolRuleModifyTime.FieldName = "RULE_MODIFY_TIME_STR";
            this.gcolRuleModifyTime.Name = "gcolRuleModifyTime";
            this.gcolRuleModifyTime.OptionsColumn.AllowEdit = false;
            this.gcolRuleModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcolRuleModifyTime.Visible = true;
            this.gcolRuleModifyTime.VisibleIndex = 8;
            this.gcolRuleModifyTime.Width = 120;
            //
            // gcolRuleModifier
            //
            this.gcolRuleModifier.Caption = "Người sửa";
            this.gcolRuleModifier.FieldName = "MODIFIER";
            this.gcolRuleModifier.Name = "gcolRuleModifier";
            this.gcolRuleModifier.OptionsColumn.AllowEdit = false;
            this.gcolRuleModifier.OptionsFilter.AllowFilter = false;
            this.gcolRuleModifier.Visible = true;
            this.gcolRuleModifier.VisibleIndex = 9;
            this.gcolRuleModifier.Width = 90;
            //
            // btnRuleDeleteEnable
            //
            this.btnRuleDeleteEnable.AutoHeight = false;
            this.btnRuleDeleteEnable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnRuleDeleteEnable.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "Xóa", null, null, true)});
            this.btnRuleDeleteEnable.Name = "btnRuleDeleteEnable";
            this.btnRuleDeleteEnable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnRuleDeleteEnable.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnRuleDeleteEnable_Click);
            //
            // btnRuleDeleteDisable
            //
            this.btnRuleDeleteDisable.AutoHeight = false;
            this.btnRuleDeleteDisable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, false, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnRuleDeleteEnable.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject13, serializableAppearanceObject14, serializableAppearanceObject15, serializableAppearanceObject16, "", null, null, true)});
            this.btnRuleDeleteDisable.Name = "btnRuleDeleteDisable";
            this.btnRuleDeleteDisable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            //
            // riRuleNumOrder
            //
            this.riRuleNumOrder.AutoHeight = false;
            this.riRuleNumOrder.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.riRuleNumOrder.IsFloatValue = false;
            this.riRuleNumOrder.MaxValue = new decimal(new int[] {9999, 0, 0, 0});
            this.riRuleNumOrder.MinValue = new decimal(new int[] {1, 0, 0, 0});
            this.riRuleNumOrder.Name = "riRuleNumOrder";
            //
            // riRuleMatchType
            //
            this.riRuleMatchType.AutoHeight = false;
            this.riRuleMatchType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.riRuleMatchType.Items.AddRange(new object[] {
            "PREFIX",
            "CONTAINS",
            "REGEX"});
            this.riRuleMatchType.Name = "riRuleMatchType";
            this.riRuleMatchType.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //
            // riRuleExamCategoryId
            //
            this.riRuleExamCategoryId.AutoHeight = false;
            this.riRuleExamCategoryId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.riRuleExamCategoryId.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CATEGORY_CODE", "Mã", 80),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CATEGORY_NAME", "Tên loại XN", 180)});
            this.riRuleExamCategoryId.DisplayMember = "CATEGORY_NAME";
            this.riRuleExamCategoryId.Name = "riRuleExamCategoryId";
            this.riRuleExamCategoryId.NullText = "";
            this.riRuleExamCategoryId.ValueMember = "ID";
            // =============================================
            // FORM
            // =============================================
            //
            // EmrExamCategoryForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1900, 800);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "EmrExamCategoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý Phân loại Xét nghiệm & Cấu hình ghép cặp";
            this.Load += new System.EventHandler(this.EmrExamCategoryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlCatHeader)).EndInit();
            this.pnlCatHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlCat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCatDeleteEnable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCatDeleteDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riCatNumOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlRuleHeader)).EndInit();
            this.pnlRuleHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlRule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRuleDeleteEnable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRuleDeleteDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riRuleNumOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riRuleMatchType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riRuleExamCategoryId)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // BarManager
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem bbtnSave;
        private DevExpress.XtraBars.BarButtonItem bbtnRefresh;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        // Bottom panel
        private DevExpress.XtraEditors.PanelControl pnlBottom;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        // Split container
        private DevExpress.XtraEditors.SplitContainerControl splitMain;
        // Category (left)
        private DevExpress.XtraEditors.PanelControl pnlCatHeader;
        private DevExpress.XtraEditors.LabelControl lblCatTitle;
        private DevExpress.XtraEditors.SimpleButton btnCatAdd;
        private DevExpress.XtraGrid.GridControl gridControlCat;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewCat;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCatNumOrder;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCatDelete;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCatCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCatName;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCatCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCatCreator;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCatModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCatModifier;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnCatDeleteEnable;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnCatDeleteDisable;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit riCatNumOrder;
        // Rule (right)
        private DevExpress.XtraEditors.PanelControl pnlRuleHeader;
        private DevExpress.XtraEditors.LabelControl lblRuleTitle;
        private DevExpress.XtraEditors.SimpleButton btnRuleAdd;
        private DevExpress.XtraGrid.GridControl gridControlRule;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewRule;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRuleNumOrder;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRuleDelete;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRulePattern;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRuleMatchType;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRuleKeyExtractor;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRuleExamCategory;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRuleCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRuleCreator;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRuleModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRuleModifier;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnRuleDeleteEnable;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnRuleDeleteDisable;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit riRuleNumOrder;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox riRuleMatchType;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit riRuleExamCategoryId;
    }
}
