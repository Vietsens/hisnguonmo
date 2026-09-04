namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup.SelectedExam
{
    partial class frmSelectedExamList
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblNote = new System.Windows.Forms.Label();
            this.grdSelectedExam = new DevExpress.XtraGrid.GridControl();
            this.gridViewSelectedExam = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcNumOrder = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcRoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonEditDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnAddMore = new System.Windows.Forms.Button();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.timerCloseForm = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grdSelectedExam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSelectedExam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditDelete)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.BackColor = System.Drawing.Color.Teal;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(900, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "CÁC PHÒNG KHÁM BẠN ĐÃ CHỌN";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // grdSelectedExam
            //
            this.grdSelectedExam.Location = new System.Drawing.Point(20, 70);
            this.grdSelectedExam.MainView = this.gridViewSelectedExam;
            this.grdSelectedExam.Name = "grdSelectedExam";
            this.grdSelectedExam.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonEditDelete});
            this.grdSelectedExam.Size = new System.Drawing.Size(860, 340);
            this.grdSelectedExam.TabIndex = 1;
            this.grdSelectedExam.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSelectedExam});
            //
            // gridViewSelectedExam
            //
            this.gridViewSelectedExam.Appearance.HeaderPanel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.gridViewSelectedExam.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewSelectedExam.Appearance.Row.Font = new System.Drawing.Font("Arial", 15F);
            this.gridViewSelectedExam.Appearance.Row.Options.UseFont = true;
            this.gridViewSelectedExam.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcNumOrder,
            this.gcRoomName,
            this.gcServiceName,
            this.gcDelete});
            this.gridViewSelectedExam.GridControl = this.grdSelectedExam;
            this.gridViewSelectedExam.Name = "gridViewSelectedExam";
            this.gridViewSelectedExam.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewSelectedExam.OptionsCustomization.AllowFilter = false;
            this.gridViewSelectedExam.OptionsCustomization.AllowGroup = false;
            this.gridViewSelectedExam.OptionsCustomization.AllowSort = false;
            this.gridViewSelectedExam.OptionsFind.AllowFindPanel = false;
            this.gridViewSelectedExam.OptionsMenu.EnableColumnMenu = false;
            this.gridViewSelectedExam.OptionsMenu.EnableFooterMenu = false;
            this.gridViewSelectedExam.OptionsNavigation.AutoFocusNewRow = true;
            this.gridViewSelectedExam.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gridViewSelectedExam.OptionsView.ColumnAutoWidth = true;
            this.gridViewSelectedExam.OptionsView.ShowGroupPanel = false;
            this.gridViewSelectedExam.OptionsView.ShowIndicator = false;
            this.gridViewSelectedExam.RowHeight = 56;
            this.gridViewSelectedExam.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewSelectedExam_CustomUnboundColumnData);
            //
            // gcNumOrder
            //
            this.gcNumOrder.Caption = "TT";
            this.gcNumOrder.FieldName = "NUM_ORDER_STR";
            this.gcNumOrder.MaxWidth = 70;
            this.gcNumOrder.MinWidth = 70;
            this.gcNumOrder.Name = "gcNumOrder";
            this.gcNumOrder.OptionsColumn.AllowEdit = false;
            this.gcNumOrder.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcNumOrder.Visible = true;
            this.gcNumOrder.VisibleIndex = 0;
            this.gcNumOrder.Width = 70;
            //
            // gcRoomName
            //
            this.gcRoomName.Caption = "Phòng khám";
            this.gcRoomName.FieldName = "RoomName";
            this.gcRoomName.Name = "gcRoomName";
            this.gcRoomName.OptionsColumn.AllowEdit = false;
            this.gcRoomName.Visible = true;
            this.gcRoomName.VisibleIndex = 1;
            this.gcRoomName.Width = 330;
            //
            // gcServiceName
            //
            this.gcServiceName.Caption = "Dịch vụ khám";
            this.gcServiceName.FieldName = "ServiceName";
            this.gcServiceName.Name = "gcServiceName";
            this.gcServiceName.OptionsColumn.AllowEdit = false;
            this.gcServiceName.Visible = true;
            this.gcServiceName.VisibleIndex = 2;
            this.gcServiceName.Width = 330;
            //
            // gcDelete
            //
            this.gcDelete.Caption = " ";
            this.gcDelete.ColumnEdit = this.repositoryItemButtonEditDelete;
            this.gcDelete.FieldName = "DELETE_BUTTON";
            this.gcDelete.MaxWidth = 130;
            this.gcDelete.MinWidth = 130;
            this.gcDelete.Name = "gcDelete";
            this.gcDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcDelete.Visible = true;
            this.gcDelete.VisibleIndex = 3;
            this.gcDelete.Width = 130;
            //
            // repositoryItemButtonEditDelete
            //
            this.repositoryItemButtonEditDelete.Appearance.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold);
            this.repositoryItemButtonEditDelete.Appearance.Options.UseFont = true;
            this.repositoryItemButtonEditDelete.AutoHeight = false;
            DevExpress.XtraEditors.Controls.EditorButton editorButtonDelete = new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph);
            editorButtonDelete.Caption = "XÓA";
            editorButtonDelete.Width = 120;
            this.repositoryItemButtonEditDelete.Buttons.Clear();
            this.repositoryItemButtonEditDelete.Buttons.Add(editorButtonDelete);
            this.repositoryItemButtonEditDelete.Name = "repositoryItemButtonEditDelete";
            this.repositoryItemButtonEditDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButtonEditDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEditDelete_ButtonClick);
            //
            // lblNote
            //
            this.lblNote.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Italic);
            this.lblNote.ForeColor = System.Drawing.Color.DimGray;
            this.lblNote.Location = new System.Drawing.Point(20, 416);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(860, 30);
            this.lblNote.TabIndex = 2;
            this.lblNote.Text = "Phòng khám đầu tiên trong danh sách là phòng khám chính";
            this.lblNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnAddMore
            //
            this.btnAddMore.BackColor = System.Drawing.Color.Teal;
            this.btnAddMore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddMore.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnAddMore.ForeColor = System.Drawing.Color.White;
            this.btnAddMore.Location = new System.Drawing.Point(20, 452);
            this.btnAddMore.Name = "btnAddMore";
            this.btnAddMore.Size = new System.Drawing.Size(276, 88);
            this.btnAddMore.TabIndex = 3;
            this.btnAddMore.Text = "+ CHỌN THÊM PHÒNG";
            this.btnAddMore.UseVisualStyleBackColor = false;
            this.btnAddMore.Click += new System.EventHandler(this.btnAddMore_Click);
            //
            // btnRegister
            //
            this.btnRegister.BackColor = System.Drawing.Color.SeaGreen;
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegister.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnRegister.ForeColor = System.Drawing.Color.White;
            this.btnRegister.Location = new System.Drawing.Point(312, 452);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(276, 88);
            this.btnRegister.TabIndex = 4;
            this.btnRegister.Text = "ĐĂNG KÝ";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            //
            // btnClose
            //
            this.btnClose.BackColor = System.Drawing.Color.Gray;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(604, 452);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(276, 88);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "ĐÓNG";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // timerCloseForm
            //
            this.timerCloseForm.Interval = 180000;
            //
            // frmSelectedExamList
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 560);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.btnAddMore);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.grdSelectedExam);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmSelectedExamList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách phòng khám đã chọn";
            this.Load += new System.EventHandler(this.frmSelectedExamList_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmSelectedExamList_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSelectedExam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdSelectedExam)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblNote;
        private DevExpress.XtraGrid.GridControl grdSelectedExam;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSelectedExam;
        private DevExpress.XtraGrid.Columns.GridColumn gcNumOrder;
        private DevExpress.XtraGrid.Columns.GridColumn gcRoomName;
        private DevExpress.XtraGrid.Columns.GridColumn gcServiceName;
        private DevExpress.XtraGrid.Columns.GridColumn gcDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEditDelete;
        private System.Windows.Forms.Button btnAddMore;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Timer timerCloseForm;
    }
}
