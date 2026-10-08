namespace HIS.Desktop.Plugins.CallPatientDrugStoreDispense
{
    partial class frmDispenseWaitingScreen
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
            this.panelHeader = new DevExpress.XtraEditors.PanelControl();
            this.panelCalling = new DevExpress.XtraEditors.PanelControl();
            this.panelGrid = new DevExpress.XtraEditors.PanelControl();
            this.gridControlPatient = new DevExpress.XtraGrid.GridControl();
            this.gridViewPatient = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcDobYear = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.timerReload = new System.Windows.Forms.Timer(this.components);
            this.timerScroll = new System.Windows.Forms.Timer(this.components);
            this.timerBlink = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.panelHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelCalling)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelGrid)).BeginInit();
            this.panelGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPatient)).BeginInit();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1280, 100);
            this.panelHeader.TabIndex = 0;
            this.panelHeader.Paint += new System.Windows.Forms.PaintEventHandler(this.panelHeader_Paint);
            this.panelHeader.Resize += new System.EventHandler(this.panel_Resize);
            //
            // panelCalling
            //
            this.panelCalling.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelCalling.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelCalling.Location = new System.Drawing.Point(0, 640);
            this.panelCalling.Name = "panelCalling";
            this.panelCalling.Size = new System.Drawing.Size(1280, 80);
            this.panelCalling.TabIndex = 2;
            this.panelCalling.Visible = false;
            this.panelCalling.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCalling_Paint);
            this.panelCalling.Resize += new System.EventHandler(this.panel_Resize);
            //
            // panelGrid
            //
            this.panelGrid.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(243)))), ((int)(((byte)(236)))));
            this.panelGrid.Appearance.Options.UseBackColor = true;
            this.panelGrid.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelGrid.Controls.Add(this.gridControlPatient);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(0, 100);
            this.panelGrid.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.panelGrid.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Padding = new System.Windows.Forms.Padding(24, 6, 24, 6);
            this.panelGrid.Size = new System.Drawing.Size(1280, 540);
            this.panelGrid.TabIndex = 1;
            //
            // gridControlPatient
            //
            this.gridControlPatient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlPatient.Location = new System.Drawing.Point(24, 6);
            this.gridControlPatient.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.gridControlPatient.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlPatient.MainView = this.gridViewPatient;
            this.gridControlPatient.Name = "gridControlPatient";
            this.gridControlPatient.Size = new System.Drawing.Size(1232, 528);
            this.gridControlPatient.TabIndex = 0;
            this.gridControlPatient.TabStop = false;
            this.gridControlPatient.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPatient});
            //
            // gridViewPatient
            //
            this.gridViewPatient.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(243)))), ((int)(((byte)(236)))));
            this.gridViewPatient.Appearance.Empty.Options.UseBackColor = true;
            this.gridViewPatient.Appearance.Row.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(243)))), ((int)(((byte)(236)))));
            this.gridViewPatient.Appearance.Row.Options.UseBackColor = true;
            this.gridViewPatient.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gridViewPatient.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcStt,
            this.gcPatientName,
            this.gcDobYear,
            this.gcStatus});
            this.gridViewPatient.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gridViewPatient.GridControl = this.gridControlPatient;
            this.gridViewPatient.Name = "gridViewPatient";
            this.gridViewPatient.OptionsBehavior.Editable = false;
            this.gridViewPatient.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewPatient.OptionsCustomization.AllowColumnResizing = false;
            this.gridViewPatient.OptionsCustomization.AllowFilter = false;
            this.gridViewPatient.OptionsCustomization.AllowSort = false;
            this.gridViewPatient.OptionsMenu.EnableColumnMenu = false;
            this.gridViewPatient.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewPatient.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gridViewPatient.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gridViewPatient.OptionsView.ShowGroupPanel = false;
            this.gridViewPatient.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewPatient.OptionsView.ShowIndicator = false;
            this.gridViewPatient.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewPatient.RowHeight = 50;
            this.gridViewPatient.ColumnPanelRowHeight = 40;
            this.gridViewPatient.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridViewPatient.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridViewPatient.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gridViewPatient_CustomDrawCell);
            this.gridViewPatient.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewPatient_CustomDrawColumnHeader);
            //
            // gcStt
            //
            this.gcStt.Caption = "STT";
            this.gcStt.FieldName = "STT_DISPLAY";
            this.gcStt.Name = "gcStt";
            this.gcStt.Visible = true;
            this.gcStt.VisibleIndex = 0;
            this.gcStt.Width = 90;
            //
            // gcPatientName
            //
            this.gcPatientName.Caption = "Họ và tên";
            this.gcPatientName.FieldName = "PATIENT_NAME";
            this.gcPatientName.Name = "gcPatientName";
            this.gcPatientName.Visible = true;
            this.gcPatientName.VisibleIndex = 1;
            this.gcPatientName.Width = 560;
            //
            // gcDobYear
            //
            this.gcDobYear.Caption = "Năm sinh";
            this.gcDobYear.FieldName = "DOB_YEAR";
            this.gcDobYear.Name = "gcDobYear";
            this.gcDobYear.Visible = true;
            this.gcDobYear.VisibleIndex = 2;
            this.gcDobYear.Width = 260;
            //
            // gcStatus
            //
            this.gcStatus.Caption = "Trạng thái";
            this.gcStatus.FieldName = "EXP_MEST_STT_ID";
            this.gcStatus.Name = "gcStatus";
            this.gcStatus.Visible = true;
            this.gcStatus.VisibleIndex = 3;
            this.gcStatus.Width = 260;
            //
            // timerReload
            //
            this.timerReload.Interval = 10000;
            this.timerReload.Tick += new System.EventHandler(this.timerReload_Tick);
            //
            // timerScroll
            //
            this.timerScroll.Interval = 5000;
            this.timerScroll.Tick += new System.EventHandler(this.timerScroll_Tick);
            //
            // timerBlink
            //
            this.timerBlink.Interval = 500;
            this.timerBlink.Tick += new System.EventHandler(this.timerBlink_Tick);
            //
            // frmDispenseWaitingScreen
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(243)))), ((int)(((byte)(236)))));
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelCalling);
            this.Controls.Add(this.panelHeader);
            this.KeyPreview = true;
            this.Name = "frmDispenseWaitingScreen";
            this.Text = "Màn hình chờ phát thuốc nhà thuốc";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDispenseWaitingScreen_FormClosing);
            this.Load += new System.EventHandler(this.frmDispenseWaitingScreen_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmDispenseWaitingScreen_KeyDown);
            this.Controls.SetChildIndex(this.panelHeader, 0);
            this.Controls.SetChildIndex(this.panelCalling, 0);
            this.Controls.SetChildIndex(this.panelGrid, 0);
            ((System.ComponentModel.ISupportInitialize)(this.panelHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelCalling)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelGrid)).EndInit();
            this.panelGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPatient)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelHeader;
        private DevExpress.XtraEditors.PanelControl panelCalling;
        private DevExpress.XtraEditors.PanelControl panelGrid;
        private DevExpress.XtraGrid.GridControl gridControlPatient;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPatient;
        private DevExpress.XtraGrid.Columns.GridColumn gcStt;
        private DevExpress.XtraGrid.Columns.GridColumn gcPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gcDobYear;
        private DevExpress.XtraGrid.Columns.GridColumn gcStatus;
        private System.Windows.Forms.Timer timerReload;
        private System.Windows.Forms.Timer timerScroll;
        private System.Windows.Forms.Timer timerBlink;
    }
}
