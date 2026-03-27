namespace HIS.Desktop.Plugins.CallPatientDepartmentV1
{
    partial class FormWaitingScreenV1
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
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcRoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcNextCallNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcMaxNumOrder = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcCurrentCallNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcTotal = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcNewCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcProcCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcEndCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.timerReload = new System.Windows.Forms.Timer(this.components);
            this.timerScroll = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.Shown += new System.EventHandler(this.FormWaitingScreenV1_Shown);
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1241, 572);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // gridControl1
            //
            this.gridControl1.EmbeddedNavigator.Appearance.ForeColor = System.Drawing.Color.White;
            this.gridControl1.EmbeddedNavigator.Appearance.Options.UseForeColor = true;
            this.gridControl1.Location = new System.Drawing.Point(2, 2);
            this.gridControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
            this.gridControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1237, 568);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            //
            // gridView1
            //
            this.gridView1.Appearance.Empty.BackColor = System.Drawing.Color.LavenderBlush;
            this.gridView1.Appearance.Empty.BackColor2 = System.Drawing.Color.Transparent;
            this.gridView1.Appearance.Empty.BorderColor = System.Drawing.Color.White;
            this.gridView1.Appearance.Empty.Options.UseBackColor = true;
            this.gridView1.Appearance.Empty.Options.UseBorderColor = true;
            this.gridView1.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.gridView1.Appearance.FocusedCell.BackColor2 = System.Drawing.Color.Transparent;
            this.gridView1.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.gridView1.Appearance.FocusedRow.BackColor2 = System.Drawing.Color.Transparent;
            this.gridView1.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridView1.Appearance.HorzLine.BackColor = System.Drawing.Color.Transparent;
            this.gridView1.Appearance.HorzLine.Options.UseBackColor = true;
            this.gridView1.Appearance.OddRow.BackColor = System.Drawing.Color.Transparent;
            this.gridView1.Appearance.OddRow.Options.UseBackColor = true;
            this.gridView1.Appearance.Row.BackColor = System.Drawing.Color.Transparent;
            this.gridView1.Appearance.Row.Options.UseBackColor = true;
            this.gridView1.Appearance.SelectedRow.BackColor = System.Drawing.Color.Transparent;
            this.gridView1.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gridView1.Appearance.VertLine.BackColor = System.Drawing.Color.Black;
            this.gridView1.Appearance.VertLine.Options.UseBackColor = true;
            this.gridView1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gridView1.ColumnPanelRowHeight = 50;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcRoomName,
            this.gcNextCallNumber,
            this.gcMaxNumOrder,
            this.gcCurrentCallNumber,
            this.gcTotal,
            this.gcNewCount,
            this.gcProcCount,
            this.gcEndCount});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsFind.AllowFindPanel = false;
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gridView1.OptionsView.ColumnAutoWidth = true;
            this.gridView1.RowHeight = 50;
            this.gridView1.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridView1.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridView1_CustomColumnDisplayText);
            //
            // gcRoomName
            //
            this.gcRoomName.AppearanceCell.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcRoomName.AppearanceCell.Options.UseFont = true;
            this.gcRoomName.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gcRoomName.AppearanceHeader.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcRoomName.AppearanceHeader.ForeColor = System.Drawing.Color.White;
            this.gcRoomName.AppearanceHeader.Options.UseBackColor = true;
            this.gcRoomName.AppearanceHeader.Options.UseFont = true;
            this.gcRoomName.AppearanceHeader.Options.UseForeColor = true;
            this.gcRoomName.AppearanceHeader.Options.UseTextOptions = true;
            this.gcRoomName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcRoomName.Caption = "Tên phòng";
            this.gcRoomName.FieldName = "EXECUTE_ROOM_NAME";
            this.gcRoomName.Name = "gcRoomName";
            this.gcRoomName.OptionsColumn.AllowEdit = false;
            this.gcRoomName.OptionsColumn.AllowFocus = false;
            this.gcRoomName.OptionsColumn.AllowMove = false;
            this.gcRoomName.OptionsColumn.AllowShowHide = false;
            this.gcRoomName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcRoomName.Visible = true;
            this.gcRoomName.VisibleIndex = 0;
            this.gcRoomName.Width = 200;
            //
            // gcNextCallNumber
            //
            this.gcNextCallNumber.AppearanceCell.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcNextCallNumber.AppearanceCell.Options.UseFont = true;
            this.gcNextCallNumber.AppearanceCell.Options.UseTextOptions = true;
            this.gcNextCallNumber.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcNextCallNumber.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gcNextCallNumber.AppearanceHeader.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcNextCallNumber.AppearanceHeader.ForeColor = System.Drawing.Color.White;
            this.gcNextCallNumber.AppearanceHeader.Options.UseBackColor = true;
            this.gcNextCallNumber.AppearanceHeader.Options.UseFont = true;
            this.gcNextCallNumber.AppearanceHeader.Options.UseForeColor = true;
            this.gcNextCallNumber.AppearanceHeader.Options.UseTextOptions = true;
            this.gcNextCallNumber.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcNextCallNumber.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gcNextCallNumber.DisplayFormat.FormatString = "{0:N0}";
            this.gcNextCallNumber.Caption = "STT tiếp theo";
            this.gcNextCallNumber.FieldName = "NEXT_CALL_NUMBER";
            this.gcNextCallNumber.Name = "gcNextCallNumber";
            this.gcNextCallNumber.OptionsColumn.AllowEdit = false;
            this.gcNextCallNumber.OptionsColumn.AllowFocus = false;
            this.gcNextCallNumber.OptionsColumn.AllowMove = false;
            this.gcNextCallNumber.OptionsColumn.AllowShowHide = false;
            this.gcNextCallNumber.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcNextCallNumber.Visible = true;
            this.gcNextCallNumber.VisibleIndex = 1;
            this.gcNextCallNumber.Width = 140;
            //
            // gcMaxNumOrder
            //
            this.gcMaxNumOrder.AppearanceCell.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcMaxNumOrder.AppearanceCell.Options.UseFont = true;
            this.gcMaxNumOrder.AppearanceCell.Options.UseTextOptions = true;
            this.gcMaxNumOrder.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcMaxNumOrder.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gcMaxNumOrder.AppearanceHeader.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcMaxNumOrder.AppearanceHeader.ForeColor = System.Drawing.Color.White;
            this.gcMaxNumOrder.AppearanceHeader.Options.UseBackColor = true;
            this.gcMaxNumOrder.AppearanceHeader.Options.UseFont = true;
            this.gcMaxNumOrder.AppearanceHeader.Options.UseForeColor = true;
            this.gcMaxNumOrder.AppearanceHeader.Options.UseTextOptions = true;
            this.gcMaxNumOrder.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcMaxNumOrder.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gcMaxNumOrder.DisplayFormat.FormatString = "{0:N0}";
            this.gcMaxNumOrder.Caption = "STT lớn nhất";
            this.gcMaxNumOrder.FieldName = "MAX_NUM_ORDER";
            this.gcMaxNumOrder.Name = "gcMaxNumOrder";
            this.gcMaxNumOrder.OptionsColumn.AllowEdit = false;
            this.gcMaxNumOrder.OptionsColumn.AllowFocus = false;
            this.gcMaxNumOrder.OptionsColumn.AllowMove = false;
            this.gcMaxNumOrder.OptionsColumn.AllowShowHide = false;
            this.gcMaxNumOrder.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcMaxNumOrder.Visible = true;
            this.gcMaxNumOrder.VisibleIndex = 2;
            this.gcMaxNumOrder.Width = 130;
            //
            // gcCurrentCallNumber
            //
            this.gcCurrentCallNumber.AppearanceCell.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcCurrentCallNumber.AppearanceCell.Options.UseFont = true;
            this.gcCurrentCallNumber.AppearanceCell.Options.UseTextOptions = true;
            this.gcCurrentCallNumber.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcCurrentCallNumber.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gcCurrentCallNumber.AppearanceHeader.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcCurrentCallNumber.AppearanceHeader.ForeColor = System.Drawing.Color.White;
            this.gcCurrentCallNumber.AppearanceHeader.Options.UseBackColor = true;
            this.gcCurrentCallNumber.AppearanceHeader.Options.UseFont = true;
            this.gcCurrentCallNumber.AppearanceHeader.Options.UseForeColor = true;
            this.gcCurrentCallNumber.AppearanceHeader.Options.UseTextOptions = true;
            this.gcCurrentCallNumber.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcCurrentCallNumber.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gcCurrentCallNumber.DisplayFormat.FormatString = "{0:N0}";
            this.gcCurrentCallNumber.Caption = "STT vừa gọi";
            this.gcCurrentCallNumber.FieldName = "CURRENT_CALL_NUMBER";
            this.gcCurrentCallNumber.Name = "gcCurrentCallNumber";
            this.gcCurrentCallNumber.OptionsColumn.AllowEdit = false;
            this.gcCurrentCallNumber.OptionsColumn.AllowFocus = false;
            this.gcCurrentCallNumber.OptionsColumn.AllowMove = false;
            this.gcCurrentCallNumber.OptionsColumn.AllowShowHide = false;
            this.gcCurrentCallNumber.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcCurrentCallNumber.Visible = true;
            this.gcCurrentCallNumber.VisibleIndex = 3;
            this.gcCurrentCallNumber.Width = 130;
            //
            // gcTotal
            //
            this.gcTotal.AppearanceCell.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcTotal.AppearanceCell.Options.UseFont = true;
            this.gcTotal.AppearanceCell.Options.UseTextOptions = true;
            this.gcTotal.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcTotal.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gcTotal.AppearanceHeader.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcTotal.AppearanceHeader.ForeColor = System.Drawing.Color.White;
            this.gcTotal.AppearanceHeader.Options.UseBackColor = true;
            this.gcTotal.AppearanceHeader.Options.UseFont = true;
            this.gcTotal.AppearanceHeader.Options.UseForeColor = true;
            this.gcTotal.AppearanceHeader.Options.UseTextOptions = true;
            this.gcTotal.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcTotal.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gcTotal.DisplayFormat.FormatString = "{0:N0}";
            this.gcTotal.Caption = "Tổng";
            this.gcTotal.FieldName = "TOTAL_TODAY_SERVICE_REQ";
            this.gcTotal.Name = "gcTotal";
            this.gcTotal.OptionsColumn.AllowEdit = false;
            this.gcTotal.OptionsColumn.AllowFocus = false;
            this.gcTotal.OptionsColumn.AllowMove = false;
            this.gcTotal.OptionsColumn.AllowShowHide = false;
            this.gcTotal.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcTotal.Visible = true;
            this.gcTotal.VisibleIndex = 4;
            this.gcTotal.Width = 100;
            //
            // gcNewCount
            //
            this.gcNewCount.AppearanceCell.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcNewCount.AppearanceCell.Options.UseFont = true;
            this.gcNewCount.AppearanceCell.Options.UseTextOptions = true;
            this.gcNewCount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcNewCount.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gcNewCount.AppearanceHeader.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcNewCount.AppearanceHeader.ForeColor = System.Drawing.Color.White;
            this.gcNewCount.AppearanceHeader.Options.UseBackColor = true;
            this.gcNewCount.AppearanceHeader.Options.UseFont = true;
            this.gcNewCount.AppearanceHeader.Options.UseForeColor = true;
            this.gcNewCount.AppearanceHeader.Options.UseTextOptions = true;
            this.gcNewCount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcNewCount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gcNewCount.DisplayFormat.FormatString = "{0:N0}";
            this.gcNewCount.Caption = "Chưa khám";
            this.gcNewCount.FieldName = "TOTAL_NEW_SERVICE_REQ";
            this.gcNewCount.Name = "gcNewCount";
            this.gcNewCount.OptionsColumn.AllowEdit = false;
            this.gcNewCount.OptionsColumn.AllowFocus = false;
            this.gcNewCount.OptionsColumn.AllowMove = false;
            this.gcNewCount.OptionsColumn.AllowShowHide = false;
            this.gcNewCount.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcNewCount.Visible = true;
            this.gcNewCount.VisibleIndex = 5;
            this.gcNewCount.Width = 120;
            //
            // gcProcCount
            //
            this.gcProcCount.AppearanceCell.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcProcCount.AppearanceCell.Options.UseFont = true;
            this.gcProcCount.AppearanceCell.Options.UseTextOptions = true;
            this.gcProcCount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcProcCount.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gcProcCount.AppearanceHeader.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcProcCount.AppearanceHeader.ForeColor = System.Drawing.Color.White;
            this.gcProcCount.AppearanceHeader.Options.UseBackColor = true;
            this.gcProcCount.AppearanceHeader.Options.UseFont = true;
            this.gcProcCount.AppearanceHeader.Options.UseForeColor = true;
            this.gcProcCount.AppearanceHeader.Options.UseTextOptions = true;
            this.gcProcCount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcProcCount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gcProcCount.DisplayFormat.FormatString = "{0:N0}";
            this.gcProcCount.Caption = "Đang khám";
            this.gcProcCount.FieldName = "TOTAL_PROC_SERVICE_REQ";
            this.gcProcCount.Name = "gcProcCount";
            this.gcProcCount.OptionsColumn.AllowEdit = false;
            this.gcProcCount.OptionsColumn.AllowFocus = false;
            this.gcProcCount.OptionsColumn.AllowMove = false;
            this.gcProcCount.OptionsColumn.AllowShowHide = false;
            this.gcProcCount.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcProcCount.Visible = true;
            this.gcProcCount.VisibleIndex = 6;
            this.gcProcCount.Width = 120;
            //
            // gcEndCount
            //
            this.gcEndCount.AppearanceCell.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcEndCount.AppearanceCell.Options.UseFont = true;
            this.gcEndCount.AppearanceCell.Options.UseTextOptions = true;
            this.gcEndCount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcEndCount.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gcEndCount.AppearanceHeader.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcEndCount.AppearanceHeader.ForeColor = System.Drawing.Color.White;
            this.gcEndCount.AppearanceHeader.Options.UseBackColor = true;
            this.gcEndCount.AppearanceHeader.Options.UseFont = true;
            this.gcEndCount.AppearanceHeader.Options.UseForeColor = true;
            this.gcEndCount.AppearanceHeader.Options.UseTextOptions = true;
            this.gcEndCount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcEndCount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gcEndCount.DisplayFormat.FormatString = "{0:N0}";
            this.gcEndCount.Caption = "Khám xong";
            this.gcEndCount.FieldName = "TOTAL_END_SERVICE_REQ";
            this.gcEndCount.Name = "gcEndCount";
            this.gcEndCount.OptionsColumn.AllowEdit = false;
            this.gcEndCount.OptionsColumn.AllowFocus = false;
            this.gcEndCount.OptionsColumn.AllowMove = false;
            this.gcEndCount.OptionsColumn.AllowShowHide = false;
            this.gcEndCount.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcEndCount.Visible = true;
            this.gcEndCount.VisibleIndex = 7;
            this.gcEndCount.Width = 120;
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1241, 572);
            this.layoutControlGroup1.TextVisible = false;
            //
            // layoutControlItem1
            //
            this.layoutControlItem1.Control = this.gridControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1241, 572);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            //
            // timerReload
            //
            this.timerReload.Tick += new System.EventHandler(this.timerReload_Tick);
            //
            // timerScroll
            //
            this.timerScroll.Tick += new System.EventHandler(this.timerScroll_Tick);
            //
            // FormWaitingScreenV1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1241, 572);
            this.Controls.Add(this.layoutControl1);
            this.Name = "FormWaitingScreenV1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Màn hình chờ theo khoa 1";
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWaitingScreenV1_FormClosing);
            this.Load += new System.EventHandler(this.FormWaitingScreenV1_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private System.Windows.Forms.Timer timerReload;
        private System.Windows.Forms.Timer timerScroll;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gcRoomName;
        private DevExpress.XtraGrid.Columns.GridColumn gcNextCallNumber;
        private DevExpress.XtraGrid.Columns.GridColumn gcMaxNumOrder;
        private DevExpress.XtraGrid.Columns.GridColumn gcCurrentCallNumber;
        private DevExpress.XtraGrid.Columns.GridColumn gcTotal;
        private DevExpress.XtraGrid.Columns.GridColumn gcNewCount;
        private DevExpress.XtraGrid.Columns.GridColumn gcProcCount;
        private DevExpress.XtraGrid.Columns.GridColumn gcEndCount;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}