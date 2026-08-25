namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom
{
    partial class frmTreatmentBedRoom
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.toggleSwitch1 = new DevExpress.XtraEditors.ToggleSwitch();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.spinReloadTime = new DevExpress.XtraEditors.SpinEdit();
            this.gridControlRoom = new DevExpress.XtraGrid.GridControl();
            this.gridViewRoom = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gcRoomCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcRoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtDepartment = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciDepartment = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.spinColumnCount = new DevExpress.XtraEditors.SpinEdit();
            this.lciColumnCount = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toggleSwitch1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinReloadTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinColumnCount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciColumnCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.toggleSwitch1);
            this.layoutControl1.Controls.Add(this.labelControl1);
            this.layoutControl1.Controls.Add(this.spinColumnCount);
            this.layoutControl1.Controls.Add(this.spinReloadTime);
            this.layoutControl1.Controls.Add(this.gridControlRoom);
            this.layoutControl1.Controls.Add(this.txtDepartment);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(431, 321);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // toggleSwitch1
            // 
            this.toggleSwitch1.Location = new System.Drawing.Point(166, 295);
            this.toggleSwitch1.Name = "toggleSwitch1";
            this.toggleSwitch1.Properties.OffText = "Bật màn hình mở rộng";
            this.toggleSwitch1.Properties.OnText = "On";
            this.toggleSwitch1.Size = new System.Drawing.Size(263, 24);
            this.toggleSwitch1.StyleController = this.layoutControl1;
            this.toggleSwitch1.TabIndex = 6;
            this.toggleSwitch1.Toggled += new System.EventHandler(this.toggleSwitch1_Toggled);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(303, 271);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(20, 13);
            this.labelControl1.StyleController = this.layoutControl1;
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "giây";
            // 
            // spinReloadTime
            // 
            this.spinReloadTime.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinReloadTime.Location = new System.Drawing.Point(127, 271);
            this.spinReloadTime.Name = "spinReloadTime";
            this.spinReloadTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinReloadTime.Size = new System.Drawing.Size(172, 20);
            this.spinReloadTime.StyleController = this.layoutControl1;
            this.spinReloadTime.TabIndex = 4;
            // 
            // gridControlRoom
            // 
            this.gridControlRoom.Location = new System.Drawing.Point(2, 26);
            this.gridControlRoom.MainView = this.gridViewRoom;
            this.gridControlRoom.Name = "gridControlRoom";
            this.gridControlRoom.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gridControlRoom.Size = new System.Drawing.Size(427, 241);
            this.gridControlRoom.TabIndex = 1;
            this.gridControlRoom.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewRoom});
            this.gridControlRoom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridControlRoom_MouseDown);
            // 
            // gridViewRoom
            // 
            this.gridViewRoom.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcCheck,
            this.gcRoomCode,
            this.gcRoomName});
            this.gridViewRoom.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            this.gridViewRoom.GridControl = this.gridControlRoom;
            this.gridViewRoom.Name = "gridViewRoom";
            this.gridViewRoom.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
            this.gridViewRoom.OptionsFind.AllowFindPanel = false;
            this.gridViewRoom.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewRoom.OptionsView.ShowGroupPanel = false;
            this.gridViewRoom.OptionsView.ShowIndicator = false;
            this.gridViewRoom.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewRoom_CustomDrawColumnHeader);
            this.gridViewRoom.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewRoom_CellValueChanged);
            // 
            // gcCheck
            // 
            this.gcCheck.AppearanceHeader.Options.UseTextOptions = true;
            this.gcCheck.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcCheck.Caption = "Chọn";
            this.gcCheck.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gcCheck.FieldName = "IsCheck";
            this.gcCheck.MaxWidth = 45;
            this.gcCheck.MinWidth = 45;
            this.gcCheck.Name = "gcCheck";
            this.gcCheck.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcCheck.OptionsColumn.FixedWidth = true;
            this.gcCheck.ToolTip = "Tích chọn phòng";
            this.gcCheck.Visible = true;
            this.gcCheck.VisibleIndex = 0;
            this.gcCheck.Width = 45;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // gcRoomCode
            // 
            this.gcRoomCode.AppearanceCell.Options.UseTextOptions = true;
            this.gcRoomCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gcRoomCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gcRoomCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcRoomCode.Caption = "Mã phòng";
            this.gcRoomCode.FieldName = "ROOM_CODE";
            this.gcRoomCode.Name = "gcRoomCode";
            this.gcRoomCode.OptionsColumn.AllowEdit = false;
            this.gcRoomCode.Visible = true;
            this.gcRoomCode.VisibleIndex = 1;
            this.gcRoomCode.Width = 130;
            // 
            // gcRoomName
            // 
            this.gcRoomName.AppearanceCell.Options.UseTextOptions = true;
            this.gcRoomName.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gcRoomName.AppearanceHeader.Options.UseTextOptions = true;
            this.gcRoomName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcRoomName.Caption = "Tên phòng";
            this.gcRoomName.FieldName = "ROOM_NAME";
            this.gcRoomName.Name = "gcRoomName";
            this.gcRoomName.OptionsColumn.AllowEdit = false;
            this.gcRoomName.Visible = true;
            this.gcRoomName.VisibleIndex = 2;
            this.gcRoomName.Width = 380;
            // 
            // txtDepartment
            // 
            this.txtDepartment.Location = new System.Drawing.Point(56, 2);
            this.txtDepartment.Name = "txtDepartment";
            this.txtDepartment.Properties.NullValuePrompt = "Khoa của phòng làm việc";
            this.txtDepartment.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtDepartment.Properties.ReadOnly = true;
            this.txtDepartment.Size = new System.Drawing.Size(373, 20);
            this.txtDepartment.StyleController = this.layoutControl1;
            this.txtDepartment.TabIndex = 0;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciDepartment,
            this.lciRoom,
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem2,
            this.lciColumnCount,
            this.emptySpaceItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(431, 321);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciDepartment
            // 
            this.lciDepartment.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDepartment.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDepartment.Control = this.txtDepartment;
            this.lciDepartment.Location = new System.Drawing.Point(0, 0);
            this.lciDepartment.Name = "lciDepartment";
            this.lciDepartment.Size = new System.Drawing.Size(431, 24);
            this.lciDepartment.Text = "Khoa";
            this.lciDepartment.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDepartment.TextSize = new System.Drawing.Size(50, 20);
            this.lciDepartment.TextToControlDistance = 4;
            // 
            // lciRoom
            // 
            this.lciRoom.Control = this.gridControlRoom;
            this.lciRoom.Location = new System.Drawing.Point(0, 24);
            this.lciRoom.Name = "lciRoom";
            this.lciRoom.Size = new System.Drawing.Size(431, 221);
            this.lciRoom.TextSize = new System.Drawing.Size(0, 0);
            this.lciRoom.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.spinReloadTime;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 269);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(301, 24);
            this.layoutControlItem1.Text = "Thời gian tải lại:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(120, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(325, 269);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(106, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.labelControl1;
            this.layoutControlItem2.Location = new System.Drawing.Point(301, 269);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(24, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.toggleSwitch1;
            this.layoutControlItem3.Location = new System.Drawing.Point(164, 293);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(267, 28);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            //
            // spinColumnCount
            //
            this.spinColumnCount.EditValue = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.spinColumnCount.Location = new System.Drawing.Point(127, 247);
            this.spinColumnCount.Name = "spinColumnCount";
            this.spinColumnCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinColumnCount.Properties.IsFloatValue = false;
            this.spinColumnCount.Properties.Mask.EditMask = "N00";
            this.spinColumnCount.Properties.MaxValue = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.spinColumnCount.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinColumnCount.Size = new System.Drawing.Size(172, 20);
            this.spinColumnCount.StyleController = this.layoutControl1;
            this.spinColumnCount.TabIndex = 7;
            //
            // lciColumnCount
            //
            this.lciColumnCount.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciColumnCount.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciColumnCount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciColumnCount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciColumnCount.Control = this.spinColumnCount;
            this.lciColumnCount.Location = new System.Drawing.Point(0, 245);
            this.lciColumnCount.Name = "lciColumnCount";
            this.lciColumnCount.Size = new System.Drawing.Size(301, 24);
            this.lciColumnCount.Text = "Số cột hiển thị:";
            this.lciColumnCount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciColumnCount.TextSize = new System.Drawing.Size(120, 20);
            this.lciColumnCount.TextToControlDistance = 5;
            //
            // emptySpaceItem3
            //
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(301, 245);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(130, 24);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            //
            // emptySpaceItem2
            //
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 293);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(164, 28);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmTreatmentBedRoom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 321);
            this.Controls.Add(this.layoutControl1);
            this.MinimumSize = new System.Drawing.Size(420, 300);
            this.Name = "frmTreatmentBedRoom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách phòng trong khoa";
            this.Load += new System.EventHandler(this.frmTreatmentBedRoom_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.toggleSwitch1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinReloadTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinColumnCount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciColumnCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtDepartment;
        private DevExpress.XtraLayout.LayoutControlItem lciDepartment;
        private DevExpress.XtraGrid.GridControl gridControlRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewRoom;
        private DevExpress.XtraGrid.Columns.GridColumn gcCheck;
        private DevExpress.XtraGrid.Columns.GridColumn gcRoomCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcRoomName;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraLayout.LayoutControlItem lciRoom;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SpinEdit spinReloadTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.ToggleSwitch toggleSwitch1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.SpinEdit spinColumnCount;
        private DevExpress.XtraLayout.LayoutControlItem lciColumnCount;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
    }
}
