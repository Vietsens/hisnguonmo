namespace HIS.Desktop.Plugins.WaitingScreenPttt
{
    partial class frmSetupWaitingScreenPttt
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
            this.tgOpenScreen = new DevExpress.XtraEditors.ToggleSwitch();
            this.spinReloadSeconds = new DevExpress.XtraEditors.SpinEdit();
            this.gridControlBedRoom = new DevExpress.XtraGrid.GridControl();
            this.gridViewBedRoom = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnBedRoomCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnBedRoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtDepartment = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciDepartment = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBedRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciReloadSeconds = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOpenScreen = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tgOpenScreen.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinReloadSeconds.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlBedRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewBedRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReloadSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOpenScreen)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.tgOpenScreen);
            this.layoutControl1.Controls.Add(this.spinReloadSeconds);
            this.layoutControl1.Controls.Add(this.gridControlBedRoom);
            this.layoutControl1.Controls.Add(this.txtDepartment);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(504, 461);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // tgOpenScreen
            //
            this.tgOpenScreen.Location = new System.Drawing.Point(152, 427);
            this.tgOpenScreen.Name = "tgOpenScreen";
            this.tgOpenScreen.Properties.OffText = "Tắt";
            this.tgOpenScreen.Properties.OnText = "Bật";
            this.tgOpenScreen.Size = new System.Drawing.Size(340, 24);
            this.tgOpenScreen.StyleController = this.layoutControl1;
            this.tgOpenScreen.TabIndex = 3;
            this.tgOpenScreen.Toggled += new System.EventHandler(this.tgOpenScreen_Toggled);
            //
            // spinReloadSeconds
            //
            this.spinReloadSeconds.EditValue = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.spinReloadSeconds.Location = new System.Drawing.Point(152, 403);
            this.spinReloadSeconds.Name = "spinReloadSeconds";
            this.spinReloadSeconds.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinReloadSeconds.Properties.IsFloatValue = false;
            this.spinReloadSeconds.Properties.Mask.EditMask = "N00";
            this.spinReloadSeconds.Properties.MaxValue = new decimal(new int[] {
            86400,
            0,
            0,
            0});
            this.spinReloadSeconds.Properties.MinValue = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.spinReloadSeconds.Size = new System.Drawing.Size(340, 20);
            this.spinReloadSeconds.StyleController = this.layoutControl1;
            this.spinReloadSeconds.TabIndex = 2;
            //
            // gridControlBedRoom
            //
            this.gridControlBedRoom.Location = new System.Drawing.Point(12, 52);
            this.gridControlBedRoom.MainView = this.gridViewBedRoom;
            this.gridControlBedRoom.Name = "gridControlBedRoom";
            this.gridControlBedRoom.Size = new System.Drawing.Size(480, 347);
            this.gridControlBedRoom.TabIndex = 1;
            this.gridControlBedRoom.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewBedRoom});
            //
            // gridViewBedRoom
            //
            this.gridViewBedRoom.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnBedRoomCode,
            this.gridColumnBedRoomName});
            this.gridViewBedRoom.GridControl = this.gridControlBedRoom;
            this.gridViewBedRoom.Name = "gridViewBedRoom";
            this.gridViewBedRoom.OptionsBehavior.Editable = false;
            this.gridViewBedRoom.OptionsSelection.MultiSelect = true;
            this.gridViewBedRoom.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewBedRoom.OptionsView.ShowGroupPanel = false;
            this.gridViewBedRoom.OptionsView.ShowIndicator = false;
            //
            // gridColumnBedRoomCode
            //
            this.gridColumnBedRoomCode.Caption = "Mã buồng";
            this.gridColumnBedRoomCode.FieldName = "BED_ROOM_CODE";
            this.gridColumnBedRoomCode.Name = "gridColumnBedRoomCode";
            this.gridColumnBedRoomCode.Visible = true;
            this.gridColumnBedRoomCode.VisibleIndex = 1;
            this.gridColumnBedRoomCode.Width = 120;
            //
            // gridColumnBedRoomName
            //
            this.gridColumnBedRoomName.Caption = "Tên buồng";
            this.gridColumnBedRoomName.FieldName = "BED_ROOM_NAME";
            this.gridColumnBedRoomName.Name = "gridColumnBedRoomName";
            this.gridColumnBedRoomName.Visible = true;
            this.gridColumnBedRoomName.VisibleIndex = 2;
            this.gridColumnBedRoomName.Width = 320;
            //
            // txtDepartment
            //
            this.txtDepartment.Location = new System.Drawing.Point(152, 12);
            this.txtDepartment.Name = "txtDepartment";
            this.txtDepartment.Properties.ReadOnly = true;
            this.txtDepartment.Size = new System.Drawing.Size(340, 20);
            this.txtDepartment.StyleController = this.layoutControl1;
            this.txtDepartment.TabIndex = 0;
            this.txtDepartment.TabStop = false;
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciDepartment,
            this.lciBedRoom,
            this.lciReloadSeconds,
            this.lciOpenScreen});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(504, 461);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciDepartment
            //
            this.lciDepartment.Control = this.txtDepartment;
            this.lciDepartment.Location = new System.Drawing.Point(0, 0);
            this.lciDepartment.Name = "lciDepartment";
            this.lciDepartment.Size = new System.Drawing.Size(484, 24);
            this.lciDepartment.Text = "Khoa:";
            this.lciDepartment.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDepartment.TextSize = new System.Drawing.Size(135, 13);
            this.lciDepartment.TextToControlDistance = 5;
            //
            // lciBedRoom
            //
            this.lciBedRoom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciBedRoom.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciBedRoom.Control = this.gridControlBedRoom;
            this.lciBedRoom.Location = new System.Drawing.Point(0, 24);
            this.lciBedRoom.Name = "lciBedRoom";
            this.lciBedRoom.Size = new System.Drawing.Size(484, 367);
            this.lciBedRoom.Text = "Buồng:";
            this.lciBedRoom.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciBedRoom.TextSize = new System.Drawing.Size(135, 13);
            //
            // lciReloadSeconds
            //
            this.lciReloadSeconds.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciReloadSeconds.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciReloadSeconds.Control = this.spinReloadSeconds;
            this.lciReloadSeconds.Location = new System.Drawing.Point(0, 391);
            this.lciReloadSeconds.Name = "lciReloadSeconds";
            this.lciReloadSeconds.Size = new System.Drawing.Size(484, 24);
            this.lciReloadSeconds.Text = "Thời gian tải lại (giây):";
            this.lciReloadSeconds.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciReloadSeconds.TextSize = new System.Drawing.Size(135, 13);
            this.lciReloadSeconds.TextToControlDistance = 5;
            //
            // lciOpenScreen
            //
            this.lciOpenScreen.Control = this.tgOpenScreen;
            this.lciOpenScreen.Location = new System.Drawing.Point(0, 415);
            this.lciOpenScreen.Name = "lciOpenScreen";
            this.lciOpenScreen.Size = new System.Drawing.Size(484, 26);
            this.lciOpenScreen.Text = "Mở màn hình chờ:";
            this.lciOpenScreen.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciOpenScreen.TextSize = new System.Drawing.Size(135, 13);
            this.lciOpenScreen.TextToControlDistance = 5;
            //
            // frmSetupWaitingScreenPttt
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 461);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmSetupWaitingScreenPttt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thiết lập màn hình chờ PT-TT";
            this.Load += new System.EventHandler(this.frmSetupWaitingScreenPttt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tgOpenScreen.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinReloadSeconds.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlBedRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewBedRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReloadSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOpenScreen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtDepartment;
        private DevExpress.XtraLayout.LayoutControlItem lciDepartment;
        private DevExpress.XtraGrid.GridControl gridControlBedRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewBedRoom;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnBedRoomCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnBedRoomName;
        private DevExpress.XtraLayout.LayoutControlItem lciBedRoom;
        private DevExpress.XtraEditors.SpinEdit spinReloadSeconds;
        private DevExpress.XtraLayout.LayoutControlItem lciReloadSeconds;
        private DevExpress.XtraEditors.ToggleSwitch tgOpenScreen;
        private DevExpress.XtraLayout.LayoutControlItem lciOpenScreen;
    }
}
