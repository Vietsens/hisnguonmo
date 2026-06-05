namespace HIS.UC.TransactionPayformGrid
{
    partial class UCTransactionPayformGrid
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.gridControlPayform = new DevExpress.XtraGrid.GridControl();
            this.gridViewPayform = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colPayForm = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBank = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBankFee = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCurrency = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colExchangeRate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTotalAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoLookUpPayForm = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.repoLookUpBank = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.repoLookUpCurrency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.repoSpinBankFee = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repoSpinAmount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repoSpinExchangeRate = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repoSpinTotalReadOnly = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repoBtnDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repoTextDash = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPayform)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPayform)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoLookUpPayForm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoLookUpBank)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoLookUpCurrency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoSpinBankFee)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoSpinAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoSpinExchangeRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoSpinTotalReadOnly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoTextDash)).BeginInit();
            this.SuspendLayout();
            //
            // gridControlPayform
            //
            this.gridControlPayform.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlPayform.Location = new System.Drawing.Point(0, 0);
            this.gridControlPayform.MainView = this.gridViewPayform;
            this.gridControlPayform.Name = "gridControlPayform";
            this.gridControlPayform.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
                this.repoLookUpPayForm,
                this.repoLookUpBank,
                this.repoLookUpCurrency,
                this.repoSpinBankFee,
                this.repoSpinAmount,
                this.repoSpinExchangeRate,
                this.repoSpinTotalReadOnly,
                this.repoBtnDelete,
                this.repoTextDash});
            this.gridControlPayform.Size = new System.Drawing.Size(900, 90);
            this.gridControlPayform.TabIndex = 0;
            this.gridControlPayform.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
                this.gridViewPayform});
            //
            // gridViewPayform
            //
            this.gridViewPayform.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
                this.colPayForm,
                this.colBank,
                this.colBankFee,
                this.colAmount,
                this.colCurrency,
                this.colExchangeRate,
                this.colTotalAmount,
                this.colDelete});
            this.gridViewPayform.GridControl = this.gridControlPayform;
            this.gridViewPayform.Name = "gridViewPayform";
            this.gridViewPayform.OptionsView.ColumnAutoWidth = true;
            this.gridViewPayform.OptionsView.ShowGroupPanel = false;
            this.gridViewPayform.OptionsView.ShowIndicator = false;
            this.gridViewPayform.OptionsView.ShowFooter = true;
            // Nut dropdown / nut X luon hien o moi cell -> click 1 lan la trung nut (giong grid Quy ho tro)
            this.gridViewPayform.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewPayform.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.gridViewPayform.OptionsBehavior.Editable = true;
            // Chon/sua trong 1 click (mac dinh Default phai click 2 lan: lan 1 focus, lan 2 mo editor)
            this.gridViewPayform.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
            // Dong bo hanh vi voi grid Chiet khau / Quy ho tro: them dong, xuong dong, chon gia tri
            this.gridViewPayform.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewPayform.OptionsNavigation.AutoFocusNewRow = true;
            this.gridViewPayform.OptionsNavigation.EnterMoveNextColumn = true;
            this.gridViewPayform.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewPayform.OptionsCustomization.AllowColumnResizing = false;
            this.gridViewPayform.OptionsMenu.EnableColumnMenu = false;
            this.gridViewPayform.OptionsCustomization.AllowGroup = false;
            this.gridViewPayform.OptionsCustomization.AllowSort = false;
            this.gridViewPayform.OptionsView.AnimationType = DevExpress.XtraGrid.Views.Base.GridAnimationType.NeverAnimate;
            this.gridViewPayform.NewItemRowText = "Nhấn vào đây để thêm hình thức thanh toán mới";
            this.gridViewPayform.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewPayform_CustomRowCellEdit);
            this.gridViewPayform.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewPayform_CustomColumnDisplayText);
            this.gridViewPayform.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewPayform_CellValueChanged);
            this.gridViewPayform.InvalidRowException += new DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventHandler(this.gridViewPayform_InvalidRowException);
            this.gridViewPayform.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gridViewPayform_InitNewRow);
            this.gridViewPayform.ShownEditor += new System.EventHandler(this.gridViewPayform_ShownEditor);
            //
            // colPayForm
            //
            this.colPayForm.Caption = "Hình thức TT";
            this.colPayForm.ColumnEdit = this.repoLookUpPayForm;
            this.colPayForm.FieldName = "PAY_FORM_ID";
            this.colPayForm.Name = "colPayForm";
            this.colPayForm.SummaryItem.FieldName = "PAY_FORM_ID";
            this.colPayForm.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
            this.colPayForm.SummaryItem.DisplayFormat = "Tổng thành tiền:";
            this.colPayForm.Visible = true;
            this.colPayForm.VisibleIndex = 0;
            this.colPayForm.Width = 150;
            //
            // colBank
            //
            this.colBank.Caption = "Ngân hàng";
            this.colBank.ColumnEdit = this.repoLookUpBank;
            this.colBank.FieldName = "BANK_ID";
            this.colBank.Name = "colBank";
            this.colBank.Visible = true;
            this.colBank.VisibleIndex = 1;
            this.colBank.Width = 110;
            //
            // colBankFee
            //
            this.colBankFee.Caption = "Phụ phí";
            this.colBankFee.ColumnEdit = this.repoSpinBankFee;
            this.colBankFee.DisplayFormat.FormatString = "#,##0";
            this.colBankFee.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colBankFee.FieldName = "BANK_FEE_AMOUNT";
            this.colBankFee.Name = "colBankFee";
            this.colBankFee.Visible = true;
            this.colBankFee.VisibleIndex = 2;
            this.colBankFee.Width = 100;
            //
            // colAmount
            //
            this.colAmount.Caption = "Số tiền";
            this.colAmount.ColumnEdit = this.repoSpinAmount;
            this.colAmount.DisplayFormat.FormatString = "#,##0";
            this.colAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colAmount.FieldName = "AMOUNT";
            this.colAmount.Name = "colAmount";
            this.colAmount.Visible = true;
            this.colAmount.VisibleIndex = 3;
            this.colAmount.Width = 90;
            //
            // colCurrency
            //
            this.colCurrency.Caption = "Loại tiền";
            this.colCurrency.ColumnEdit = this.repoLookUpCurrency;
            this.colCurrency.FieldName = "CURRENCY_CODE";
            this.colCurrency.Name = "colCurrency";
            this.colCurrency.Visible = true;
            this.colCurrency.VisibleIndex = 4;
            this.colCurrency.Width = 80;
            //
            // colExchangeRate
            //
            this.colExchangeRate.Caption = "Tỉ giá";
            this.colExchangeRate.ColumnEdit = this.repoSpinExchangeRate;
            this.colExchangeRate.DisplayFormat.FormatString = "#,##0.######";
            this.colExchangeRate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colExchangeRate.FieldName = "EXCHANGE_RATE";
            this.colExchangeRate.Name = "colExchangeRate";
            this.colExchangeRate.Visible = true;
            this.colExchangeRate.VisibleIndex = 5;
            this.colExchangeRate.Width = 80;
            //
            // colTotalAmount
            //
            this.colTotalAmount.Caption = "Thành tiền (VND)";
            this.colTotalAmount.ColumnEdit = this.repoSpinTotalReadOnly;
            this.colTotalAmount.DisplayFormat.FormatString = "#,##0";
            this.colTotalAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colTotalAmount.FieldName = "TOTAL_AMOUNT_VND";
            this.colTotalAmount.Name = "colTotalAmount";
            this.colTotalAmount.OptionsColumn.AllowEdit = false;
            this.colTotalAmount.SummaryItem.FieldName = "TOTAL_AMOUNT_VND";
            this.colTotalAmount.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            this.colTotalAmount.SummaryItem.DisplayFormat = "{0:#,##0}";
            this.colTotalAmount.Visible = true;
            this.colTotalAmount.VisibleIndex = 6;
            this.colTotalAmount.Width = 120;
            //
            // colDelete
            //
            this.colDelete.Caption = " ";
            this.colDelete.ColumnEdit = this.repoBtnDelete;
            this.colDelete.FieldName = "Delete";
            this.colDelete.Name = "colDelete";
            // AllowEdit = true de cell vao edit-mode -> ButtonClick cua nut X moi fire (false se khong xoa duoc)
            this.colDelete.OptionsColumn.AllowEdit = true;
            this.colDelete.OptionsColumn.ShowInCustomizationForm = false;
            this.colDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colDelete.Visible = true;
            this.colDelete.VisibleIndex = 7;
            this.colDelete.Width = 30;
            //
            // repoLookUpPayForm
            //
            this.repoLookUpPayForm.AutoHeight = false;
            this.repoLookUpPayForm.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoLookUpPayForm.Name = "repoLookUpPayForm";
            this.repoLookUpPayForm.NullText = "";
            this.repoLookUpPayForm.EditValueChanged += new System.EventHandler(this.repoLookUpPayForm_EditValueChanged);
            //
            // repoLookUpBank
            //
            this.repoLookUpBank.AutoHeight = false;
            this.repoLookUpBank.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoLookUpBank.Name = "repoLookUpBank";
            this.repoLookUpBank.NullText = "";
            this.repoLookUpBank.EditValueChanged += new System.EventHandler(this.repoLookUpBank_EditValueChanged);
            //
            // repoLookUpCurrency
            //
            this.repoLookUpCurrency.AutoHeight = false;
            this.repoLookUpCurrency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoLookUpCurrency.Name = "repoLookUpCurrency";
            this.repoLookUpCurrency.NullText = "";
            this.repoLookUpCurrency.EditValueChanged += new System.EventHandler(this.repoLookUpCurrency_EditValueChanged);
            //
            // repoSpinBankFee
            //
            this.repoSpinBankFee.AutoHeight = false;
            this.repoSpinBankFee.DisplayFormat.FormatString = "#,##0";
            this.repoSpinBankFee.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repoSpinBankFee.EditFormat.FormatString = "#,##0";
            this.repoSpinBankFee.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repoSpinBankFee.MaxValue = new decimal(new int[] { 1410065408, 2, 0, 0 });
            this.repoSpinBankFee.MinValue = new decimal(new int[] { 0, 0, 0, 0 });
            this.repoSpinBankFee.Name = "repoSpinBankFee";
            this.repoSpinBankFee.SpinStyle = DevExpress.XtraEditors.Controls.SpinStyles.Vertical;
            //
            // repoSpinAmount
            //
            this.repoSpinAmount.AutoHeight = false;
            this.repoSpinAmount.DisplayFormat.FormatString = "#,##0";
            this.repoSpinAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repoSpinAmount.EditFormat.FormatString = "#,##0";
            this.repoSpinAmount.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repoSpinAmount.MaxValue = new decimal(new int[] { 1410065408, 2, 0, 0 });
            this.repoSpinAmount.MinValue = new decimal(new int[] { 0, 0, 0, 0 });
            this.repoSpinAmount.Name = "repoSpinAmount";
            this.repoSpinAmount.SpinStyle = DevExpress.XtraEditors.Controls.SpinStyles.Vertical;
            //
            // repoSpinExchangeRate
            //
            this.repoSpinExchangeRate.AutoHeight = false;
            this.repoSpinExchangeRate.DisplayFormat.FormatString = "#,##0.######";
            this.repoSpinExchangeRate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repoSpinExchangeRate.EditFormat.FormatString = "#,##0.######";
            this.repoSpinExchangeRate.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repoSpinExchangeRate.MaxValue = new decimal(new int[] { 1410065408, 2, 0, 0 });
            this.repoSpinExchangeRate.MinValue = new decimal(new int[] { 0, 0, 0, 0 });
            this.repoSpinExchangeRate.Name = "repoSpinExchangeRate";
            this.repoSpinExchangeRate.SpinStyle = DevExpress.XtraEditors.Controls.SpinStyles.Vertical;
            //
            // repoSpinTotalReadOnly
            //
            this.repoSpinTotalReadOnly.AutoHeight = false;
            this.repoSpinTotalReadOnly.DisplayFormat.FormatString = "#,##0";
            this.repoSpinTotalReadOnly.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repoSpinTotalReadOnly.Name = "repoSpinTotalReadOnly";
            this.repoSpinTotalReadOnly.ReadOnly = true;
            //
            // repoBtnDelete
            //
            this.repoBtnDelete.AutoHeight = false;
            this.repoBtnDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.repoBtnDelete.Name = "repoBtnDelete";
            this.repoBtnDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repoBtnDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoBtnDelete_ButtonClick);
            //
            // repoTextDash
            //
            this.repoTextDash.AutoHeight = false;
            this.repoTextDash.Name = "repoTextDash";
            this.repoTextDash.ReadOnly = true;
            this.repoTextDash.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //
            // UCTransactionPayformGrid
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridControlPayform);
            this.Name = "UCTransactionPayformGrid";
            this.Size = new System.Drawing.Size(900, 90);
            this.Load += new System.EventHandler(this.UCTransactionPayformGrid_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPayform)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPayform)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoLookUpPayForm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoLookUpBank)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoLookUpCurrency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoSpinBankFee)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoSpinAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoSpinExchangeRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoSpinTotalReadOnly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoTextDash)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControlPayform;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPayform;
        private DevExpress.XtraGrid.Columns.GridColumn colPayForm;
        private DevExpress.XtraGrid.Columns.GridColumn colBank;
        private DevExpress.XtraGrid.Columns.GridColumn colBankFee;
        private DevExpress.XtraGrid.Columns.GridColumn colAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colCurrency;
        private DevExpress.XtraGrid.Columns.GridColumn colExchangeRate;
        private DevExpress.XtraGrid.Columns.GridColumn colTotalAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repoLookUpPayForm;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repoLookUpBank;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repoLookUpCurrency;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repoSpinBankFee;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repoSpinAmount;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repoSpinExchangeRate;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repoSpinTotalReadOnly;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoBtnDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoTextDash;
    }
}
