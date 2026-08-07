namespace HIS.Desktop.Plugins.Pharmacology
{
    partial class frmPharmacologyAcin
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
            this.layoutControlAcin = new DevExpress.XtraLayout.LayoutControl();
            this.ucPagingActiveIngredient = new Inventec.UC.Paging.UcPaging();
            this.gridControlActiveIngredient = new DevExpress.XtraGrid.GridControl();
            this.gridViewActiveIngredient = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckSelect = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumnStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnActiveIngredientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnActiveIngredientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnSearchActiveIngredient = new DevExpress.XtraEditors.SimpleButton();
            this.txtSearch = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroupRoot = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItemSearch = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemBtnSearch = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItemSearch = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItemGrid = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemPaging = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlAcin)).BeginInit();
            this.layoutControlAcin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlActiveIngredient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewActiveIngredient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupRoot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBtnSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemPaging)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlAcin
            // 
            this.layoutControlAcin.Controls.Add(this.btnSave);
            this.layoutControlAcin.Controls.Add(this.ucPagingActiveIngredient);
            this.layoutControlAcin.Controls.Add(this.gridControlActiveIngredient);
            this.layoutControlAcin.Controls.Add(this.btnSearchActiveIngredient);
            this.layoutControlAcin.Controls.Add(this.txtSearch);
            this.layoutControlAcin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlAcin.Location = new System.Drawing.Point(0, 0);
            this.layoutControlAcin.Name = "layoutControlAcin";
            this.layoutControlAcin.Root = this.layoutControlGroupRoot;
            this.layoutControlAcin.Size = new System.Drawing.Size(720, 460);
            this.layoutControlAcin.TabIndex = 0;
            this.layoutControlAcin.Text = "layoutControlAcin";
            // 
            // ucPagingActiveIngredient
            // 
            this.ucPagingActiveIngredient.Location = new System.Drawing.Point(2, 428);
            this.ucPagingActiveIngredient.Name = "ucPagingActiveIngredient";
            this.ucPagingActiveIngredient.Size = new System.Drawing.Size(716, 30);
            this.ucPagingActiveIngredient.TabIndex = 3;
            // 
            // gridControlActiveIngredient
            // 
            this.gridControlActiveIngredient.Location = new System.Drawing.Point(2, 28);
            this.gridControlActiveIngredient.MainView = this.gridViewActiveIngredient;
            this.gridControlActiveIngredient.Name = "gridControlActiveIngredient";
            this.gridControlActiveIngredient.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckSelect});
            this.gridControlActiveIngredient.Size = new System.Drawing.Size(716, 396);
            this.gridControlActiveIngredient.TabIndex = 2;
            this.gridControlActiveIngredient.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewActiveIngredient});
            // 
            // gridViewActiveIngredient
            // 
            this.gridViewActiveIngredient.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnCheck,
            this.gridColumnStt,
            this.gridColumnActiveIngredientCode,
            this.gridColumnActiveIngredientName});
            this.gridViewActiveIngredient.GridControl = this.gridControlActiveIngredient;
            this.gridViewActiveIngredient.Name = "gridViewActiveIngredient";
            this.gridViewActiveIngredient.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
            this.gridViewActiveIngredient.OptionsCustomization.AllowFilter = false;
            this.gridViewActiveIngredient.OptionsCustomization.AllowSort = false;
            this.gridViewActiveIngredient.OptionsView.ShowGroupPanel = false;
            this.gridViewActiveIngredient.OptionsView.ShowIndicator = false;
            this.gridViewActiveIngredient.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewActiveIngredient_CellValueChanged);
            this.gridViewActiveIngredient.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewActiveIngredient_CustomUnboundColumnData);
            // 
            // gridColumnCheck
            // 
            this.gridColumnCheck.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnCheck.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnCheck.Caption = "Chọn";
            this.gridColumnCheck.ColumnEdit = this.repositoryItemCheckSelect;
            this.gridColumnCheck.FieldName = "check2";
            this.gridColumnCheck.Name = "gridColumnCheck";
            this.gridColumnCheck.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnCheck.Visible = true;
            this.gridColumnCheck.VisibleIndex = 0;
            this.gridColumnCheck.Width = 45;
            // 
            // repositoryItemCheckSelect
            // 
            this.repositoryItemCheckSelect.AutoHeight = false;
            this.repositoryItemCheckSelect.Name = "repositoryItemCheckSelect";
            this.repositoryItemCheckSelect.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // gridColumnStt
            // 
            this.gridColumnStt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnStt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnStt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnStt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnStt.Caption = "STT";
            this.gridColumnStt.FieldName = "STT";
            this.gridColumnStt.Name = "gridColumnStt";
            this.gridColumnStt.OptionsColumn.AllowEdit = false;
            this.gridColumnStt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnStt.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.gridColumnStt.Visible = true;
            this.gridColumnStt.VisibleIndex = 1;
            this.gridColumnStt.Width = 50;
            // 
            // gridColumnActiveIngredientCode
            // 
            this.gridColumnActiveIngredientCode.Caption = "Mã hoạt chất";
            this.gridColumnActiveIngredientCode.FieldName = "ACTIVE_INGREDIENT_CODE";
            this.gridColumnActiveIngredientCode.Name = "gridColumnActiveIngredientCode";
            this.gridColumnActiveIngredientCode.OptionsColumn.AllowEdit = false;
            this.gridColumnActiveIngredientCode.Visible = true;
            this.gridColumnActiveIngredientCode.VisibleIndex = 2;
            this.gridColumnActiveIngredientCode.Width = 160;
            // 
            // gridColumnActiveIngredientName
            // 
            this.gridColumnActiveIngredientName.Caption = "Tên hoạt chất";
            this.gridColumnActiveIngredientName.FieldName = "ACTIVE_INGREDIENT_NAME";
            this.gridColumnActiveIngredientName.Name = "gridColumnActiveIngredientName";
            this.gridColumnActiveIngredientName.OptionsColumn.AllowEdit = false;
            this.gridColumnActiveIngredientName.Visible = true;
            this.gridColumnActiveIngredientName.VisibleIndex = 3;
            this.gridColumnActiveIngredientName.Width = 500;
            // 
            // btnSearchActiveIngredient
            // 
            this.btnSearchActiveIngredient.Location = new System.Drawing.Point(252, 2);
            this.btnSearchActiveIngredient.Name = "btnSearchActiveIngredient";
            this.btnSearchActiveIngredient.Size = new System.Drawing.Size(101, 22);
            this.btnSearchActiveIngredient.StyleController = this.layoutControlAcin;
            this.btnSearchActiveIngredient.TabIndex = 1;
            this.btnSearchActiveIngredient.Text = "Tìm (Ctrl D)";
            this.btnSearchActiveIngredient.Click += new System.EventHandler(this.btnSearchActiveIngredient_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(2, 2);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearch.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearch.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtSearch.Size = new System.Drawing.Size(246, 20);
            this.txtSearch.StyleController = this.layoutControlAcin;
            this.txtSearch.TabIndex = 0;
            this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
            // 
            // layoutControlGroupRoot
            // 
            this.layoutControlGroupRoot.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroupRoot.GroupBordersVisible = false;
            this.layoutControlGroupRoot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemSearch,
            this.layoutControlItemBtnSearch,
            this.emptySpaceItemSearch,
            this.layoutControlItemGrid,
            this.layoutControlItemPaging,
            this.layoutControlItem1});
            this.layoutControlGroupRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroupRoot.Name = "layoutControlGroupRoot";
            this.layoutControlGroupRoot.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroupRoot.Size = new System.Drawing.Size(720, 460);
            this.layoutControlGroupRoot.TextVisible = false;
            // 
            // layoutControlItemSearch
            // 
            this.layoutControlItemSearch.Control = this.txtSearch;
            this.layoutControlItemSearch.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemSearch.Name = "layoutControlItemSearch";
            this.layoutControlItemSearch.Size = new System.Drawing.Size(250, 26);
            this.layoutControlItemSearch.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemSearch.TextVisible = false;
            // 
            // layoutControlItemBtnSearch
            // 
            this.layoutControlItemBtnSearch.Control = this.btnSearchActiveIngredient;
            this.layoutControlItemBtnSearch.Location = new System.Drawing.Point(250, 0);
            this.layoutControlItemBtnSearch.Name = "layoutControlItemBtnSearch";
            this.layoutControlItemBtnSearch.Size = new System.Drawing.Size(105, 26);
            this.layoutControlItemBtnSearch.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemBtnSearch.TextVisible = false;
            // 
            // emptySpaceItemSearch
            // 
            this.emptySpaceItemSearch.AllowHotTrack = false;
            this.emptySpaceItemSearch.Location = new System.Drawing.Point(460, 0);
            this.emptySpaceItemSearch.Name = "emptySpaceItemSearch";
            this.emptySpaceItemSearch.Size = new System.Drawing.Size(260, 26);
            this.emptySpaceItemSearch.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItemGrid
            // 
            this.layoutControlItemGrid.Control = this.gridControlActiveIngredient;
            this.layoutControlItemGrid.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItemGrid.Name = "layoutControlItemGrid";
            this.layoutControlItemGrid.Size = new System.Drawing.Size(720, 400);
            this.layoutControlItemGrid.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemGrid.TextVisible = false;
            // 
            // layoutControlItemPaging
            // 
            this.layoutControlItemPaging.Control = this.ucPagingActiveIngredient;
            this.layoutControlItemPaging.Location = new System.Drawing.Point(0, 426);
            this.layoutControlItemPaging.Name = "layoutControlItemPaging";
            this.layoutControlItemPaging.Size = new System.Drawing.Size(720, 34);
            this.layoutControlItemPaging.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemPaging.TextVisible = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(357, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(101, 22);
            this.btnSave.StyleController = this.layoutControlAcin;
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Lưu (Ctrl N)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnSave;
            this.layoutControlItem1.Location = new System.Drawing.Point(355, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(105, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // frmPharmacologyAcin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 460);
            this.Controls.Add(this.layoutControlAcin);
            this.Name = "frmPharmacologyAcin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách hoạt chất";
            this.Load += new System.EventHandler(this.frmPharmacologyAcin_Load);
            this.Controls.SetChildIndex(this.layoutControlAcin, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlAcin)).EndInit();
            this.layoutControlAcin.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlActiveIngredient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewActiveIngredient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupRoot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBtnSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemPaging)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControlAcin;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupRoot;
        private DevExpress.XtraEditors.TextEdit txtSearch;
        private DevExpress.XtraEditors.SimpleButton btnSearchActiveIngredient;
        private DevExpress.XtraGrid.GridControl gridControlActiveIngredient;
        internal DevExpress.XtraGrid.Views.Grid.GridView gridViewActiveIngredient;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnCheck;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckSelect;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnStt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnActiveIngredientCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnActiveIngredientName;
        private Inventec.UC.Paging.UcPaging ucPagingActiveIngredient;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemSearch;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemBtnSearch;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItemSearch;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemGrid;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemPaging;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}
