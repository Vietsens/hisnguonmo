namespace HIS.Desktop.Plugins.SupplyOrderCreate
{
    partial class frmSupplyOrderCreate
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
            this.panelHeader = new DevExpress.XtraEditors.PanelControl();
            this.lblDraft = new DevExpress.XtraEditors.LabelControl();
            this.lblOrderInfo = new DevExpress.XtraEditors.LabelControl();
            this.cboStock = new DevExpress.XtraEditors.LookUpEdit();
            this.lblStock = new DevExpress.XtraEditors.LabelControl();
            this.panelAdd = new DevExpress.XtraEditors.PanelControl();
            this.lblItemHint = new DevExpress.XtraEditors.LabelControl();
            this.btnLoadRemain = new DevExpress.XtraEditors.SimpleButton();
            this.cboItem = new DevExpress.XtraEditors.GridLookUpEdit();
            this.cboItemView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblItem = new DevExpress.XtraEditors.LabelControl();
            this.gridControlLine = new DevExpress.XtraGrid.GridControl();
            this.gridViewLine = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelBottom = new DevExpress.XtraEditors.PanelControl();
            this.chkPreview = new DevExpress.XtraEditors.CheckEdit();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnConfirm = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnReload = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelHeader)).BeginInit();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboStock.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelAdd)).BeginInit();
            this.panelAdd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboItem.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboItemView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).BeginInit();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkPreview.Properties)).BeginInit();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.Controls.Add(this.lblDraft);
            this.panelHeader.Controls.Add(this.lblOrderInfo);
            this.panelHeader.Controls.Add(this.cboStock);
            this.panelHeader.Controls.Add(this.lblStock);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1264, 60);
            this.panelHeader.TabIndex = 0;
            //
            // lblStock
            //
            this.lblStock.Location = new System.Drawing.Point(8, 11);
            this.lblStock.Name = "lblStock";
            this.lblStock.Size = new System.Drawing.Size(70, 13);
            this.lblStock.TabIndex = 0;
            this.lblStock.Text = "Kho:";
            //
            // cboStock
            //
            this.cboStock.Location = new System.Drawing.Point(84, 8);
            this.cboStock.Name = "cboStock";
            this.cboStock.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboStock.Size = new System.Drawing.Size(300, 20);
            this.cboStock.TabIndex = 1;
            this.cboStock.EditValueChanged += new System.EventHandler(this.cboStock_EditValueChanged);
            //
            // lblOrderInfo
            //
            this.lblOrderInfo.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblOrderInfo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblOrderInfo.Location = new System.Drawing.Point(400, 11);
            this.lblOrderInfo.Name = "lblOrderInfo";
            this.lblOrderInfo.Size = new System.Drawing.Size(850, 13);
            this.lblOrderInfo.TabIndex = 2;
            //
            // lblDraft
            //
            this.lblDraft.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblDraft.AutoEllipsis = true;
            this.lblDraft.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDraft.Location = new System.Drawing.Point(8, 38);
            this.lblDraft.Name = "lblDraft";
            this.lblDraft.Size = new System.Drawing.Size(1242, 13);
            this.lblDraft.TabIndex = 3;
            //
            // panelAdd
            //
            this.panelAdd.Controls.Add(this.lblItemHint);
            this.panelAdd.Controls.Add(this.btnLoadRemain);
            this.panelAdd.Controls.Add(this.cboItem);
            this.panelAdd.Controls.Add(this.lblItem);
            this.panelAdd.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelAdd.Location = new System.Drawing.Point(0, 60);
            this.panelAdd.Name = "panelAdd";
            this.panelAdd.Size = new System.Drawing.Size(1264, 34);
            this.panelAdd.TabIndex = 1;
            //
            // lblItem
            //
            this.lblItem.Location = new System.Drawing.Point(8, 10);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(70, 13);
            this.lblItem.TabIndex = 0;
            this.lblItem.Text = "Mat hang:";
            //
            // cboItem
            //
            this.cboItem.Location = new System.Drawing.Point(84, 7);
            this.cboItem.Name = "cboItem";
            this.cboItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboItem.Properties.View = this.cboItemView;
            this.cboItem.Size = new System.Drawing.Size(500, 20);
            this.cboItem.TabIndex = 1;
            this.cboItem.EditValueChanged += new System.EventHandler(this.cboItem_EditValueChanged);
            //
            // cboItemView
            //
            this.cboItemView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.cboItemView.Name = "cboItemView";
            this.cboItemView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.cboItemView.OptionsView.ShowGroupPanel = false;
            //
            // btnLoadRemain
            //
            this.btnLoadRemain.Location = new System.Drawing.Point(594, 6);
            this.btnLoadRemain.Name = "btnLoadRemain";
            this.btnLoadRemain.Size = new System.Drawing.Size(170, 22);
            this.btnLoadRemain.TabIndex = 2;
            this.btnLoadRemain.Text = "Nap hang con du tru";
            this.btnLoadRemain.Click += new System.EventHandler(this.btnLoadRemain_Click);
            //
            // lblItemHint
            //
            this.lblItemHint.Appearance.ForeColor = System.Drawing.Color.Gray;
            this.lblItemHint.Location = new System.Drawing.Point(774, 10);
            this.lblItemHint.Name = "lblItemHint";
            this.lblItemHint.Size = new System.Drawing.Size(300, 13);
            this.lblItemHint.TabIndex = 3;
            //
            // gridControlLine
            //
            this.gridControlLine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlLine.Location = new System.Drawing.Point(0, 94);
            this.gridControlLine.MainView = this.gridViewLine;
            this.gridControlLine.Name = "gridControlLine";
            this.gridControlLine.Size = new System.Drawing.Size(1264, 547);
            this.gridControlLine.TabIndex = 2;
            this.gridControlLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewLine});
            //
            // gridViewLine
            //
            this.gridViewLine.GridControl = this.gridControlLine;
            this.gridViewLine.Name = "gridViewLine";
            //
            // panelBottom
            //
            this.panelBottom.Controls.Add(this.chkPreview);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Controls.Add(this.btnConfirm);
            this.panelBottom.Controls.Add(this.btnSave);
            this.panelBottom.Controls.Add(this.btnReload);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 641);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1264, 40);
            this.panelBottom.TabIndex = 3;
            this.panelBottom.Resize += new System.EventHandler(this.panelBottom_Resize);
            //
            // chkPreview
            //
            this.chkPreview.Location = new System.Drawing.Point(8, 10);
            this.chkPreview.Name = "chkPreview";
            this.chkPreview.Properties.Caption = "Xem truoc theo nha cung cap";
            this.chkPreview.Size = new System.Drawing.Size(260, 19);
            this.chkPreview.TabIndex = 0;
            this.chkPreview.CheckedChanged += new System.EventHandler(this.chkPreview_CheckedChanged);
            //
            // btnReload
            //
            this.btnReload.Location = new System.Drawing.Point(780, 9);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(110, 22);
            this.btnReload.TabIndex = 1;
            this.btnReload.Text = "Tai lai";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(896, 9);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 22);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Luu nhap (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnConfirm
            //
            this.btnConfirm.Location = new System.Drawing.Point(1022, 9);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(120, 22);
            this.btnConfirm.TabIndex = 3;
            this.btnConfirm.Text = "Xac nhan dat";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(1148, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 22);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Thoat";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // frmSupplyOrderCreate
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.gridControlLine);
            this.Controls.Add(this.panelAdd);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelBottom);
            this.Name = "frmSupplyOrderCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lap don dat hang";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSupplyOrderCreate_FormClosing);
            this.Load += new System.EventHandler(this.frmSupplyOrderCreate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelHeader)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboStock.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelAdd)).EndInit();
            this.panelAdd.ResumeLayout(false);
            this.panelAdd.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboItem.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboItemView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).EndInit();
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkPreview.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelHeader;
        private DevExpress.XtraEditors.LabelControl lblDraft;
        private DevExpress.XtraEditors.LabelControl lblOrderInfo;
        private DevExpress.XtraEditors.LookUpEdit cboStock;
        private DevExpress.XtraEditors.LabelControl lblStock;
        private DevExpress.XtraEditors.PanelControl panelAdd;
        private DevExpress.XtraEditors.LabelControl lblItemHint;
        private DevExpress.XtraEditors.SimpleButton btnLoadRemain;
        private DevExpress.XtraEditors.GridLookUpEdit cboItem;
        private DevExpress.XtraGrid.Views.Grid.GridView cboItemView;
        private DevExpress.XtraEditors.LabelControl lblItem;
        private DevExpress.XtraGrid.GridControl gridControlLine;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewLine;
        private DevExpress.XtraEditors.PanelControl panelBottom;
        private DevExpress.XtraEditors.CheckEdit chkPreview;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnConfirm;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnReload;
    }
}
