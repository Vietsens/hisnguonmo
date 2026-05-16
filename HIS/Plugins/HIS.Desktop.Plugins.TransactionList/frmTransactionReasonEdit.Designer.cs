namespace HIS.Desktop.Plugins.TransactionList
{
    partial class frmTransactionReasonEdit
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
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.cboReason = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewReason = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciReason = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnSave = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnCancel = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpace1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboReason.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpace1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.btnCancel);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.cboReason);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(454, 90);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(364, 36);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 22);
            this.btnCancel.StyleController = this.layoutControl1;
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Hủy (Esc)";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(282, 36);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(78, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Lưu (Ctrl+S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // cboReason
            //
            this.cboReason.Location = new System.Drawing.Point(110, 12);
            this.cboReason.Name = "cboReason";
            this.cboReason.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboReason.Properties.NullText = "";
            this.cboReason.Properties.View = this.gridViewReason;
            this.cboReason.Size = new System.Drawing.Size(332, 20);
            this.cboReason.StyleController = this.layoutControl1;
            this.cboReason.TabIndex = 3;
            //
            // gridViewReason
            //
            this.gridViewReason.Name = "gridViewReason";
            this.gridViewReason.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewReason.OptionsView.ShowGroupPanel = false;
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciReason,
            this.lciBtnSave,
            this.lciBtnCancel,
            this.emptySpace1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(454, 90);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciReason
            //
            this.lciReason.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciReason.Control = this.cboReason;
            this.lciReason.Location = new System.Drawing.Point(0, 0);
            this.lciReason.Name = "lciReason";
            this.lciReason.Size = new System.Drawing.Size(434, 24);
            this.lciReason.Text = "Lý do giao dịch:";
            this.lciReason.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciReason.TextSize = new System.Drawing.Size(95, 20);
            this.lciReason.TextToControlDistance = 5;
            //
            // lciBtnSave
            //
            this.lciBtnSave.Control = this.btnSave;
            this.lciBtnSave.Location = new System.Drawing.Point(270, 24);
            this.lciBtnSave.Name = "lciBtnSave";
            this.lciBtnSave.Size = new System.Drawing.Size(82, 46);
            this.lciBtnSave.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnSave.TextVisible = false;
            //
            // lciBtnCancel
            //
            this.lciBtnCancel.Control = this.btnCancel;
            this.lciBtnCancel.Location = new System.Drawing.Point(352, 24);
            this.lciBtnCancel.Name = "lciBtnCancel";
            this.lciBtnCancel.Size = new System.Drawing.Size(82, 46);
            this.lciBtnCancel.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnCancel.TextVisible = false;
            //
            // emptySpace1
            //
            this.emptySpace1.AllowHotTrack = false;
            this.emptySpace1.Location = new System.Drawing.Point(0, 24);
            this.emptySpace1.Name = "emptySpace1";
            this.emptySpace1.Size = new System.Drawing.Size(270, 46);
            this.emptySpace1.TextSize = new System.Drawing.Size(0, 0);
            //
            // frmTransactionReasonEdit
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 90);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTransactionReasonEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sửa lý do giao dịch";
            this.Load += new System.EventHandler(this.frmTransactionReasonEdit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboReason.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpace1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.GridLookUpEdit cboReason;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewReason;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciReason;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnSave;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnCancel;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpace1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
