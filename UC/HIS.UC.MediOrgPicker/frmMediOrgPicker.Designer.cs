/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.UC.MediOrgPicker
{
    partial class frmMediOrgPicker
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
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.gridControlMediOrg = new DevExpress.XtraGrid.GridControl();
            this.gridViewMediOrg = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMediOrgCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMediOrgName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMediOrg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMediOrg)).BeginInit();
            this.SuspendLayout();
            //
            // txtKeyword
            //
            this.txtKeyword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeyword.Location = new System.Drawing.Point(7, 7);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.EditValueChangedDelay = 400;
            this.txtKeyword.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyword.Size = new System.Drawing.Size(563, 20);
            this.txtKeyword.TabIndex = 0;
            this.txtKeyword.EditValueChanged += new System.EventHandler(this.txtKeyword_EditValueChanged);
            this.txtKeyword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyDown);
            //
            // gridControlMediOrg
            //
            this.gridControlMediOrg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControlMediOrg.Location = new System.Drawing.Point(7, 33);
            this.gridControlMediOrg.MainView = this.gridViewMediOrg;
            this.gridControlMediOrg.Name = "gridControlMediOrg";
            this.gridControlMediOrg.Size = new System.Drawing.Size(563, 396);
            this.gridControlMediOrg.TabIndex = 1;
            this.gridControlMediOrg.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMediOrg});
            //
            // gridViewMediOrg
            //
            this.gridViewMediOrg.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMediOrgCode,
            this.colMediOrgName});
            this.gridViewMediOrg.GridControl = this.gridControlMediOrg;
            this.gridViewMediOrg.Name = "gridViewMediOrg";
            this.gridViewMediOrg.OptionsBehavior.Editable = false;
            this.gridViewMediOrg.OptionsView.ShowGroupPanel = false;
            this.gridViewMediOrg.OptionsView.ShowIndicator = false;
            this.gridViewMediOrg.DoubleClick += new System.EventHandler(this.gridViewMediOrg_DoubleClick);
            this.gridViewMediOrg.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridViewMediOrg_KeyDown);
            //
            // colMediOrgCode
            //
            this.colMediOrgCode.Caption = "Mã CSKCB";
            this.colMediOrgCode.FieldName = "MEDI_ORG_CODE";
            this.colMediOrgCode.Name = "colMediOrgCode";
            this.colMediOrgCode.OptionsColumn.AllowEdit = false;
            this.colMediOrgCode.OptionsColumn.ReadOnly = true;
            this.colMediOrgCode.Visible = true;
            this.colMediOrgCode.VisibleIndex = 0;
            this.colMediOrgCode.Width = 100;
            //
            // colMediOrgName
            //
            this.colMediOrgName.Caption = "Tên CSKCB";
            this.colMediOrgName.FieldName = "MEDI_ORG_NAME";
            this.colMediOrgName.Name = "colMediOrgName";
            this.colMediOrgName.OptionsColumn.AllowEdit = false;
            this.colMediOrgName.OptionsColumn.ReadOnly = true;
            this.colMediOrgName.Visible = true;
            this.colMediOrgName.VisibleIndex = 1;
            this.colMediOrgName.Width = 459;
            //
            // ucPaging1
            //
            this.ucPaging1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ucPaging1.Location = new System.Drawing.Point(7, 435);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(450, 22);
            this.ucPaging1.TabIndex = 2;
            //
            // btnChoose
            //
            this.btnChoose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChoose.Location = new System.Drawing.Point(463, 435);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(107, 23);
            this.btnChoose.TabIndex = 3;
            this.btnChoose.Text = "Chọn (Ctrl S)";
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            //
            // frmMediOrgPicker
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 466);
            this.Controls.Add(this.btnChoose);
            this.Controls.Add(this.ucPaging1);
            this.Controls.Add(this.gridControlMediOrg);
            this.Controls.Add(this.txtKeyword);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 360);
            this.Name = "frmMediOrgPicker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tìm chọn CSKCB";
            this.Load += new System.EventHandler(this.frmMediOrgPicker_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMediOrgPicker_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMediOrg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMediOrg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraGrid.GridControl gridControlMediOrg;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMediOrg;
        private DevExpress.XtraGrid.Columns.GridColumn colMediOrgCode;
        private DevExpress.XtraGrid.Columns.GridColumn colMediOrgName;
        private DevExpress.XtraEditors.SimpleButton btnChoose;
        private Inventec.UC.Paging.UcPaging ucPaging1;
    }
}
