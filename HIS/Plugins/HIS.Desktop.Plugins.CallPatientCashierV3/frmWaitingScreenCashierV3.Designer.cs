namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    partial class frmWaitingScreenCashierV3
    {
        private System.ComponentModel.IContainer components = null;

        private DevExpress.XtraEditors.PanelControl panelHeader;
        private DevExpress.XtraEditors.PictureEdit picLogo;
        private DevExpress.XtraEditors.LabelControl lblBranch;
        private DevExpress.XtraEditors.LabelControl lblDepartment;
        private DevExpress.XtraEditors.LabelControl lblClock;
        private DevExpress.XtraEditors.LabelControl lblTime;
        private DevExpress.XtraEditors.PanelControl panelCard;
        private DevExpress.XtraEditors.PanelControl panelTable;
        private DevExpress.XtraEditors.LabelControl lblTitle;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn colYearOfBirth;
        private DevExpress.XtraGrid.Columns.GridColumn colServiceGroups;
        private DevExpress.XtraEditors.PanelControl panelBottom;
        private DevExpress.XtraEditors.LabelControl lblCallingTitle;
        private DevExpress.XtraEditors.LabelControl lblCalling;
        private DevExpress.XtraEditors.LabelControl lblPage;
        private System.Windows.Forms.Timer timerReload;
        private System.Windows.Forms.Timer timerPage;
        private System.Windows.Forms.Timer timerClock;
        private System.Windows.Forms.Timer timerBlink;

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
            this.panelHeader = new DevExpress.XtraEditors.PanelControl();
            this.picLogo = new DevExpress.XtraEditors.PictureEdit();
            this.lblBranch = new DevExpress.XtraEditors.LabelControl();
            this.lblDepartment = new DevExpress.XtraEditors.LabelControl();
            this.lblClock = new DevExpress.XtraEditors.LabelControl();
            this.lblTime = new DevExpress.XtraEditors.LabelControl();
            this.panelCard = new DevExpress.XtraEditors.PanelControl();
            this.panelTable = new DevExpress.XtraEditors.PanelControl();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colYearOfBirth = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colServiceGroups = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelBottom = new DevExpress.XtraEditors.PanelControl();
            this.lblCallingTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblCalling = new DevExpress.XtraEditors.LabelControl();
            this.lblPage = new DevExpress.XtraEditors.LabelControl();
            this.timerReload = new System.Windows.Forms.Timer(this.components);
            this.timerPage = new System.Windows.Forms.Timer(this.components);
            this.timerClock = new System.Windows.Forms.Timer(this.components);
            this.timerBlink = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.panelHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelCard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.panelHeader.SuspendLayout();
            this.panelCard.SuspendLayout();
            this.panelTable.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // panelHeader (dải trên nền trắng: tên viện đỏ, khoa navy, ngày giờ bên phải)
            //
            InitFlatPanel(this.panelHeader, "panelHeader");
            this.panelHeader.Appearance.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.picLogo);
            this.panelHeader.Controls.Add(this.lblBranch);
            this.panelHeader.Controls.Add(this.lblDepartment);
            this.panelHeader.Controls.Add(this.lblClock);
            this.panelHeader.Controls.Add(this.lblTime);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height = 110;
            this.panelHeader.Resize += new System.EventHandler(this.panelHeader_Resize);
            this.picLogo.Name = "picLogo";
            this.picLogo.Properties.AllowFocused = false;
            this.picLogo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.picLogo.Properties.ShowMenu = false;
            this.picLogo.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            this.picLogo.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.picLogo.Properties.Appearance.Options.UseBackColor = true;
            this.picLogo.TabStop = false;
            InitTransparentLabel(this.lblBranch, "lblBranch", "BỆNH VIỆN");
            InitTransparentLabel(this.lblDepartment, "lblDepartment", "");
            InitTransparentLabel(this.lblClock, "lblClock", "");
            InitTransparentLabel(this.lblTime, "lblTime", "");
            //
            // panelCard (nền navy, lề quanh thẻ trắng)
            //
            InitFlatPanel(this.panelCard, "panelCard");
            this.panelCard.Controls.Add(this.panelTable);
            this.panelCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCard.Padding = new System.Windows.Forms.Padding(30, 12, 30, 16);
            //
            // panelTable (thẻ trắng chứa thanh tiêu đề + bảng)
            //
            InitFlatPanel(this.panelTable, "panelTable");
            this.panelTable.Appearance.BackColor = System.Drawing.Color.White;
            this.panelTable.Controls.Add(this.gridControl);
            this.panelTable.Controls.Add(this.lblTitle);
            this.panelTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTable.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            //
            // lblTitle (thanh "DANH SÁCH CHỜ" nền navy chữ trắng)
            //
            this.lblTitle.Appearance.Options.UseBackColor = true;
            this.lblTitle.Appearance.Options.UseFont = true;
            this.lblTitle.Appearance.Options.UseForeColor = true;
            this.lblTitle.Appearance.Options.UseTextOptions = true;
            this.lblTitle.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblTitle.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Height = 70;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "DANH SÁCH CHỜ";
            //
            // gridControl (tắt skin để Appearance của tiêu đề cột / dòng áp dụng đúng)
            //
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.gridControl.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControl.LookAndFeel.UseWindowsXPTheme = false;
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = "gridControl";
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { this.gridView });
            //
            // gridView
            //
            this.gridView.Appearance.EvenRow.Options.UseBackColor = true;
            this.gridView.Appearance.EvenRow.Options.UseForeColor = true;
            this.gridView.Appearance.OddRow.Options.UseBackColor = true;
            this.gridView.Appearance.OddRow.Options.UseForeColor = true;
            this.gridView.Appearance.Row.Options.UseFont = true;
            this.gridView.Appearance.Row.Options.UseForeColor = true;
            this.gridView.Appearance.Row.Options.UseBackColor = true;
            this.gridView.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gridView.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gridView.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gridView.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridView.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridView.Appearance.Empty.Options.UseBackColor = true;
            this.gridView.Appearance.VertLine.Options.UseBackColor = true;
            this.gridView.Appearance.HorzLine.Options.UseBackColor = true;
            this.gridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colPatientName,
            this.colYearOfBirth,
            this.colServiceGroups});
            this.gridView.GridControl = this.gridControl;
            this.gridView.Name = "gridView";
            this.gridView.OptionsBehavior.Editable = false;
            this.gridView.OptionsBehavior.ReadOnly = true;
            this.gridView.OptionsCustomization.AllowColumnMoving = false;
            this.gridView.OptionsCustomization.AllowColumnResizing = false;
            this.gridView.OptionsCustomization.AllowFilter = false;
            this.gridView.OptionsCustomization.AllowSort = false;
            this.gridView.OptionsDetail.EnableMasterViewMode = false;
            this.gridView.OptionsDetail.ShowDetailTabs = false;
            this.gridView.OptionsMenu.EnableColumnMenu = false;
            this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gridView.OptionsView.ColumnAutoWidth = true;
            this.gridView.OptionsView.EnableAppearanceEvenRow = true;
            this.gridView.OptionsView.EnableAppearanceOddRow = true;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.OptionsView.ShowIndicator = false;
            this.gridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gridView.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridView_RowStyle);
            this.gridView.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridView_CustomDrawColumnHeader);
            //
            // columns
            //
            InitColumn(this.colPatientName, "colPatientName", "HỌ TÊN BỆNH NHÂN", "PATIENT_NAME", 0, 440, DevExpress.Utils.HorzAlignment.Near);
            InitColumn(this.colYearOfBirth, "colYearOfBirth", "NĂM SINH", "YEAR_OF_BIRTH", 1, 200, DevExpress.Utils.HorzAlignment.Center);
            InitColumn(this.colServiceGroups, "colServiceGroups", "NHÓM DỊCH VỤ", "SERVICE_GROUPS", 2, 460, DevExpress.Utils.HorzAlignment.Center);
            this.colServiceGroups.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            //
            // panelBottom (thanh ĐANG GỌI, gradient teal qua Appearance)
            //
            InitFlatPanel(this.panelBottom, "panelBottom");
            this.panelBottom.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.panelBottom.Controls.Add(this.lblCallingTitle);
            this.panelBottom.Controls.Add(this.lblCalling);
            this.panelBottom.Controls.Add(this.lblPage);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Height = 140;
            this.panelBottom.Resize += new System.EventHandler(this.panelBottom_Resize);
            InitTransparentLabel(this.lblCallingTitle, "lblCallingTitle", "» ĐANG GỌI:");
            this.lblCallingTitle.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.RightCenter;
            this.lblTime.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.RightCenter;
            InitTransparentLabel(this.lblCalling, "lblCalling", "");
            this.lblCalling.AllowHtmlString = true;
            InitTransparentLabel(this.lblPage, "lblPage", "");
            //
            // timers
            //
            this.timerReload.Tick += new System.EventHandler(this.timerReload_Tick);
            this.timerPage.Tick += new System.EventHandler(this.timerPage_Tick);
            this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
            this.timerBlink.Tick += new System.EventHandler(this.timerBlink_Tick);
            //
            // frmWaitingScreenCashierV3
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.panelCard);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelBottom);
            this.Name = "frmWaitingScreenCashierV3";
            this.ShowInTaskbar = true;
            this.Text = "Màn hình chờ thu ngân 3";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmWaitingScreenCashierV3_FormClosing);
            this.Load += new System.EventHandler(this.frmWaitingScreenCashierV3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelCard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelCard.ResumeLayout(false);
            this.panelTable.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        /// <summary>PanelControl không viền, không skin để Appearance.BackColor / BackColor2 áp dụng đúng.</summary>
        private static void InitFlatPanel(DevExpress.XtraEditors.PanelControl panel, string name)
        {
            panel.Name = name;
            panel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            panel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            panel.LookAndFeel.UseDefaultLookAndFeel = false;
            panel.LookAndFeel.UseWindowsXPTheme = false;
            panel.Appearance.Options.UseBackColor = true;
        }

        private static void InitTransparentLabel(DevExpress.XtraEditors.LabelControl label, string name, string text)
        {
            label.Name = name;
            label.Text = text;
            label.Appearance.BackColor = System.Drawing.Color.Transparent;
            label.Appearance.Options.UseBackColor = true;
            label.Appearance.Options.UseFont = true;
            label.Appearance.Options.UseForeColor = true;
        }

        private static void InitColumn(DevExpress.XtraGrid.Columns.GridColumn col, string name, string caption, string fieldName, int visibleIndex, int width, DevExpress.Utils.HorzAlignment cellAlign)
        {
            col.AppearanceCell.Options.UseTextOptions = true;
            col.AppearanceCell.TextOptions.HAlignment = cellAlign;
            col.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            col.AppearanceHeader.Options.UseTextOptions = true;
            col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            col.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            col.Caption = caption;
            col.FieldName = fieldName;
            col.ImageAlignment = System.Drawing.StringAlignment.Near;
            col.Name = name;
            col.OptionsColumn.AllowEdit = false;
            col.Visible = true;
            col.VisibleIndex = visibleIndex;
            col.Width = width;
        }

        #endregion
    }
}
