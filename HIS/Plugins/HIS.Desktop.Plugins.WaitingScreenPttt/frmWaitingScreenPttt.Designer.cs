namespace HIS.Desktop.Plugins.WaitingScreenPttt
{
    partial class frmWaitingScreenPttt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWaitingScreenPttt));
            this.gridControlPatient = new DevExpress.XtraGrid.GridControl();
            this.gridViewPatient = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnDobYear = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnDoctorName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnBedRoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.timerReload = new System.Windows.Forms.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPatient)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControlPatient
            // 
            this.gridControlPatient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlPatient.Location = new System.Drawing.Point(0, 0);
            this.gridControlPatient.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.gridControlPatient.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlPatient.MainView = this.gridViewPatient;
            this.gridControlPatient.Name = "gridControlPatient";
            this.gridControlPatient.Size = new System.Drawing.Size(1024, 600);
            this.gridControlPatient.TabIndex = 0;
            this.gridControlPatient.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPatient});
            // 
            // gridViewPatient
            // 
            this.gridViewPatient.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(84)))), ((int)(((byte)(147)))));
            this.gridViewPatient.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(84)))), ((int)(((byte)(147)))));
            this.gridViewPatient.Appearance.HeaderPanel.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.gridViewPatient.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.White;
            this.gridViewPatient.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gridViewPatient.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewPatient.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gridViewPatient.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridViewPatient.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewPatient.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.gridViewPatient.Appearance.Row.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            this.gridViewPatient.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gridViewPatient.Appearance.Row.Options.UseBackColor = true;
            this.gridViewPatient.Appearance.Row.Options.UseFont = true;
            this.gridViewPatient.Appearance.Row.Options.UseForeColor = true;
            this.gridViewPatient.Appearance.Row.Options.UseTextOptions = true;
            this.gridViewPatient.Appearance.Row.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridViewPatient.ColumnPanelRowHeight = 50;
            this.gridViewPatient.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnPatientName,
            this.gridColumnDobYear,
            this.gridColumnDoctorName,
            this.gridColumnBedRoomName,
            this.gridColumnServiceName});
            this.gridViewPatient.GridControl = this.gridControlPatient;
            this.gridViewPatient.Name = "gridViewPatient";
            this.gridViewPatient.OptionsBehavior.Editable = false;
            this.gridViewPatient.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewPatient.OptionsCustomization.AllowFilter = false;
            this.gridViewPatient.OptionsCustomization.AllowSort = false;
            this.gridViewPatient.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewPatient.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gridViewPatient.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gridViewPatient.OptionsView.RowAutoHeight = true;
            this.gridViewPatient.OptionsView.ShowGroupPanel = false;
            this.gridViewPatient.OptionsView.ShowIndicator = false;
            this.gridViewPatient.RowHeight = 45;
            // 
            // gridColumnPatientName
            // 
            this.gridColumnPatientName.Caption = "HỌ VÀ TÊN";
            this.gridColumnPatientName.FieldName = "PATIENT_NAME";
            this.gridColumnPatientName.Name = "gridColumnPatientName";
            this.gridColumnPatientName.Visible = true;
            this.gridColumnPatientName.VisibleIndex = 0;
            this.gridColumnPatientName.Width = 250;
            // 
            // gridColumnDobYear
            // 
            this.gridColumnDobYear.Caption = "NĂM SINH";
            this.gridColumnDobYear.FieldName = "DOB_YEAR";
            this.gridColumnDobYear.Name = "gridColumnDobYear";
            this.gridColumnDobYear.Visible = true;
            this.gridColumnDobYear.VisibleIndex = 1;
            this.gridColumnDobYear.Width = 110;
            // 
            // gridColumnDoctorName
            // 
            this.gridColumnDoctorName.Caption = "BÁC SĨ THỰC HIỆN";
            this.gridColumnDoctorName.FieldName = "DOCTOR_NAME";
            this.gridColumnDoctorName.Name = "gridColumnDoctorName";
            this.gridColumnDoctorName.Visible = true;
            this.gridColumnDoctorName.VisibleIndex = 2;
            this.gridColumnDoctorName.Width = 250;
            // 
            // gridColumnBedRoomName
            // 
            this.gridColumnBedRoomName.Caption = "BUỒNG";
            this.gridColumnBedRoomName.FieldName = "BED_ROOM_NAME";
            this.gridColumnBedRoomName.Name = "gridColumnBedRoomName";
            this.gridColumnBedRoomName.Visible = true;
            this.gridColumnBedRoomName.VisibleIndex = 3;
            this.gridColumnBedRoomName.Width = 150;
            // 
            // gridColumnServiceName
            // 
            this.gridColumnServiceName.Caption = "TÊN DỊCH VỤ";
            this.gridColumnServiceName.FieldName = "SERVICE_NAME";
            this.gridColumnServiceName.Name = "gridColumnServiceName";
            this.gridColumnServiceName.Visible = true;
            this.gridColumnServiceName.VisibleIndex = 4;
            this.gridColumnServiceName.Width = 300;
            // 
            // timerReload
            // 
            this.timerReload.Tick += new System.EventHandler(this.timerReload_Tick);
            // 
            // frmWaitingScreenPttt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 600);
            this.Controls.Add(this.gridControlPatient);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmWaitingScreenPttt";
            this.Text = "Màn hình chờ PT-TT";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmWaitingScreenPttt_FormClosing);
            this.Load += new System.EventHandler(this.frmWaitingScreenPttt_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmWaitingScreenPttt_KeyDown);
            this.Controls.SetChildIndex(this.gridControlPatient, 0);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPatient)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControlPatient;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPatient;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDobYear;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDoctorName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnBedRoomName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnServiceName;
        private System.Windows.Forms.Timer timerReload;
    }
}
