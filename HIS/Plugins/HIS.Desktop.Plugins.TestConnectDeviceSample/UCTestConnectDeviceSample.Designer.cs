/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
namespace HIS.Desktop.Plugins.TestConnectDeviceSample
{
    partial class UCTestConnectDeviceSample
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlScanLog = new DevExpress.XtraGrid.GridControl();
            this.gridViewScanLog = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcScanTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPortName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcResult = new DevExpress.XtraGrid.Columns.GridColumn();
            this.tileControlPorts = new DevExpress.XtraEditors.TileControl();
            this.tileGroupPorts = new DevExpress.XtraEditors.TileGroup();
            this.lblPending = new DevExpress.XtraEditors.LabelControl();
            this.btnDeviceList = new DevExpress.XtraEditors.SimpleButton();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnDisconnect = new DevExpress.XtraEditors.SimpleButton();
            this.btnConnect = new DevExpress.XtraEditors.SimpleButton();
            this.cboPortCom = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblGuide = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciGuide = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPortCom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciConnect = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDisconnect = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRefresh = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPending = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciDeviceList = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTilePorts = new DevExpress.XtraLayout.LayoutControlItem();
            this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
            this.lciScanLog = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.timerPortCheck = new System.Windows.Forms.Timer(this.components);
            this.alertControlScan = new DevExpress.XtraBars.Alerter.AlertControl(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlScanLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewScanLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPortCom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGuide)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPortCom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConnect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDisconnect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPending)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDeviceList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTilePorts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciScanLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.gridControlScanLog);
            this.layoutControl1.Controls.Add(this.tileControlPorts);
            this.layoutControl1.Controls.Add(this.lblPending);
            this.layoutControl1.Controls.Add(this.btnDeviceList);
            this.layoutControl1.Controls.Add(this.btnRefresh);
            this.layoutControl1.Controls.Add(this.btnDisconnect);
            this.layoutControl1.Controls.Add(this.btnConnect);
            this.layoutControl1.Controls.Add(this.cboPortCom);
            this.layoutControl1.Controls.Add(this.lblGuide);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1320, 600);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // gridControlScanLog
            //
            this.gridControlScanLog.Location = new System.Drawing.Point(2, 387);
            this.gridControlScanLog.MainView = this.gridViewScanLog;
            this.gridControlScanLog.Name = "gridControlScanLog";
            this.gridControlScanLog.Size = new System.Drawing.Size(1316, 211);
            this.gridControlScanLog.TabIndex = 8;
            this.gridControlScanLog.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewScanLog});
            //
            // gridViewScanLog
            //
            this.gridViewScanLog.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcScanTime,
            this.gcPortName,
            this.gcCode,
            this.gcResult});
            this.gridViewScanLog.GridControl = this.gridControlScanLog;
            this.gridViewScanLog.Name = "gridViewScanLog";
            this.gridViewScanLog.OptionsBehavior.Editable = false;
            this.gridViewScanLog.OptionsCustomization.AllowSort = false;
            this.gridViewScanLog.OptionsView.ShowGroupPanel = false;
            this.gridViewScanLog.OptionsView.ShowIndicator = false;
            this.gridViewScanLog.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewScanLog_RowCellStyle);
            //
            // gcScanTime
            //
            this.gcScanTime.Caption = "Thời gian";
            this.gcScanTime.FieldName = "SCAN_TIME_STR";
            this.gcScanTime.Name = "gcScanTime";
            this.gcScanTime.OptionsColumn.AllowEdit = false;
            this.gcScanTime.Visible = true;
            this.gcScanTime.VisibleIndex = 0;
            this.gcScanTime.Width = 70;
            //
            // gcPortName
            //
            this.gcPortName.Caption = "Cổng";
            this.gcPortName.FieldName = "PORT_NAME";
            this.gcPortName.Name = "gcPortName";
            this.gcPortName.OptionsColumn.AllowEdit = false;
            this.gcPortName.Visible = true;
            this.gcPortName.VisibleIndex = 1;
            this.gcPortName.Width = 70;
            //
            // gcCode
            //
            this.gcCode.Caption = "Mã quét";
            this.gcCode.FieldName = "CODE";
            this.gcCode.Name = "gcCode";
            this.gcCode.OptionsColumn.AllowEdit = false;
            this.gcCode.Visible = true;
            this.gcCode.VisibleIndex = 2;
            this.gcCode.Width = 150;
            //
            // gcResult
            //
            this.gcResult.Caption = "Kết quả";
            this.gcResult.FieldName = "RESULT";
            this.gcResult.Name = "gcResult";
            this.gcResult.OptionsColumn.AllowEdit = false;
            this.gcResult.Visible = true;
            this.gcResult.VisibleIndex = 3;
            this.gcResult.Width = 900;
            //
            // tileControlPorts
            //
            this.tileControlPorts.AllowDrag = false;
            this.tileControlPorts.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.tileControlPorts.AppearanceItem.Normal.Options.UseFont = true;
            this.tileControlPorts.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tileControlPorts.Groups.Add(this.tileGroupPorts);
            this.tileControlPorts.IndentBetweenItems = 12;
            this.tileControlPorts.ItemCheckMode = DevExpress.XtraEditors.TileItemCheckMode.Single;
            this.tileControlPorts.ItemSize = 140;
            this.tileControlPorts.Location = new System.Drawing.Point(2, 92);
            this.tileControlPorts.MaxId = 1;
            this.tileControlPorts.Name = "tileControlPorts";
            this.tileControlPorts.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tileControlPorts.Padding = new System.Windows.Forms.Padding(12);
            this.tileControlPorts.ShowText = false;
            this.tileControlPorts.Size = new System.Drawing.Size(1316, 266);
            this.tileControlPorts.TabIndex = 7;
            this.tileControlPorts.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileControlPorts_ItemClick);
            //
            // tileGroupPorts
            //
            this.tileGroupPorts.Name = "tileGroupPorts";
            //
            // lblPending
            //
            this.lblPending.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblPending.Appearance.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblPending.Location = new System.Drawing.Point(632, 48);
            this.lblPending.Name = "lblPending";
            this.lblPending.Size = new System.Drawing.Size(256, 16);
            this.lblPending.StyleController = this.layoutControl1;
            this.lblPending.TabIndex = 6;
            //
            // btnDeviceList
            //
            this.btnDeviceList.Location = new System.Drawing.Point(1192, 46);
            this.btnDeviceList.Name = "btnDeviceList";
            this.btnDeviceList.Size = new System.Drawing.Size(126, 22);
            this.btnDeviceList.StyleController = this.layoutControl1;
            this.btnDeviceList.TabIndex = 5;
            this.btnDeviceList.Text = "DS đăng nhập";
            this.btnDeviceList.Click += new System.EventHandler(this.btnDeviceList_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(512, 46);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(116, 22);
            this.btnRefresh.StyleController = this.layoutControl1;
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Làm mới (Ctrl R)";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnDisconnect
            //
            this.btnDisconnect.Location = new System.Drawing.Point(372, 46);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(136, 22);
            this.btnDisconnect.StyleController = this.layoutControl1;
            this.btnDisconnect.TabIndex = 3;
            this.btnDisconnect.Text = "Ngắt kết nối (Ctrl D)";
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            //
            // btnConnect
            //
            this.btnConnect.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnConnect.Appearance.Options.UseFont = true;
            this.btnConnect.Location = new System.Drawing.Point(252, 46);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(116, 22);
            this.btnConnect.StyleController = this.layoutControl1;
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Kết nối (Ctrl K)";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            //
            // cboPortCom
            //
            this.cboPortCom.Location = new System.Drawing.Point(57, 46);
            this.cboPortCom.Name = "cboPortCom";
            this.cboPortCom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPortCom.Properties.NullText = "Chọn cổng COM";
            this.cboPortCom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboPortCom.Size = new System.Drawing.Size(191, 20);
            this.cboPortCom.StyleController = this.layoutControl1;
            this.cboPortCom.TabIndex = 1;
            this.cboPortCom.SelectedIndexChanged += new System.EventHandler(this.cboPortCom_SelectedIndexChanged);
            //
            // lblGuide
            //
            this.lblGuide.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblGuide.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(84)))), ((int)(((byte)(147)))));
            this.lblGuide.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblGuide.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblGuide.Location = new System.Drawing.Point(4, 4);
            this.lblGuide.Name = "lblGuide";
            this.lblGuide.Size = new System.Drawing.Size(1312, 36);
            this.lblGuide.StyleController = this.layoutControl1;
            this.lblGuide.TabIndex = 0;
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciGuide,
            this.lciPortCom,
            this.lciConnect,
            this.lciDisconnect,
            this.lciRefresh,
            this.lciPending,
            this.emptySpaceItem1,
            this.lciDeviceList,
            this.lciTilePorts,
            this.splitterItem1,
            this.lciScanLog});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1320, 600);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciGuide
            //
            this.lciGuide.Control = this.lblGuide;
            this.lciGuide.Location = new System.Drawing.Point(0, 0);
            this.lciGuide.MaxSize = new System.Drawing.Size(0, 44);
            this.lciGuide.MinSize = new System.Drawing.Size(100, 44);
            this.lciGuide.Name = "lciGuide";
            this.lciGuide.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 4, 4, 4);
            this.lciGuide.Size = new System.Drawing.Size(1320, 44);
            this.lciGuide.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciGuide.TextSize = new System.Drawing.Size(0, 0);
            this.lciGuide.TextVisible = false;
            //
            // lciPortCom
            //
            this.lciPortCom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciPortCom.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciPortCom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPortCom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPortCom.Control = this.cboPortCom;
            this.lciPortCom.Location = new System.Drawing.Point(0, 44);
            this.lciPortCom.MaxSize = new System.Drawing.Size(250, 26);
            this.lciPortCom.MinSize = new System.Drawing.Size(250, 26);
            this.lciPortCom.Name = "lciPortCom";
            this.lciPortCom.Size = new System.Drawing.Size(250, 26);
            this.lciPortCom.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPortCom.Text = "Cổng:";
            this.lciPortCom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPortCom.TextSize = new System.Drawing.Size(50, 20);
            this.lciPortCom.TextToControlDistance = 5;
            //
            // lciConnect
            //
            this.lciConnect.Control = this.btnConnect;
            this.lciConnect.Location = new System.Drawing.Point(250, 44);
            this.lciConnect.MaxSize = new System.Drawing.Size(120, 26);
            this.lciConnect.MinSize = new System.Drawing.Size(120, 26);
            this.lciConnect.Name = "lciConnect";
            this.lciConnect.Size = new System.Drawing.Size(120, 26);
            this.lciConnect.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciConnect.TextSize = new System.Drawing.Size(0, 0);
            this.lciConnect.TextVisible = false;
            //
            // lciDisconnect
            //
            this.lciDisconnect.Control = this.btnDisconnect;
            this.lciDisconnect.Location = new System.Drawing.Point(370, 44);
            this.lciDisconnect.MaxSize = new System.Drawing.Size(140, 26);
            this.lciDisconnect.MinSize = new System.Drawing.Size(140, 26);
            this.lciDisconnect.Name = "lciDisconnect";
            this.lciDisconnect.Size = new System.Drawing.Size(140, 26);
            this.lciDisconnect.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDisconnect.TextSize = new System.Drawing.Size(0, 0);
            this.lciDisconnect.TextVisible = false;
            //
            // lciRefresh
            //
            this.lciRefresh.Control = this.btnRefresh;
            this.lciRefresh.Location = new System.Drawing.Point(510, 44);
            this.lciRefresh.MaxSize = new System.Drawing.Size(120, 26);
            this.lciRefresh.MinSize = new System.Drawing.Size(120, 26);
            this.lciRefresh.Name = "lciRefresh";
            this.lciRefresh.Size = new System.Drawing.Size(120, 26);
            this.lciRefresh.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciRefresh.TextSize = new System.Drawing.Size(0, 0);
            this.lciRefresh.TextVisible = false;
            //
            // lciPending
            //
            this.lciPending.Control = this.lblPending;
            this.lciPending.Location = new System.Drawing.Point(630, 44);
            this.lciPending.MinSize = new System.Drawing.Size(1, 26);
            this.lciPending.Name = "lciPending";
            this.lciPending.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 4, 2);
            this.lciPending.Size = new System.Drawing.Size(260, 26);
            this.lciPending.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPending.TextSize = new System.Drawing.Size(0, 0);
            this.lciPending.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(890, 44);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(300, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // lciDeviceList
            //
            this.lciDeviceList.Control = this.btnDeviceList;
            this.lciDeviceList.Location = new System.Drawing.Point(1190, 44);
            this.lciDeviceList.MaxSize = new System.Drawing.Size(130, 26);
            this.lciDeviceList.MinSize = new System.Drawing.Size(130, 26);
            this.lciDeviceList.Name = "lciDeviceList";
            this.lciDeviceList.Size = new System.Drawing.Size(130, 26);
            this.lciDeviceList.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDeviceList.TextSize = new System.Drawing.Size(0, 0);
            this.lciDeviceList.TextVisible = false;
            //
            // lciTilePorts
            //
            this.lciTilePorts.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.lciTilePorts.AppearanceItemCaption.Options.UseFont = true;
            this.lciTilePorts.Control = this.tileControlPorts;
            this.lciTilePorts.Location = new System.Drawing.Point(0, 70);
            this.lciTilePorts.MaxSize = new System.Drawing.Size(0, 340);
            this.lciTilePorts.MinSize = new System.Drawing.Size(200, 180);
            this.lciTilePorts.Name = "lciTilePorts";
            this.lciTilePorts.Size = new System.Drawing.Size(1320, 290);
            this.lciTilePorts.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTilePorts.Text = "Máy quét đang kết nối";
            this.lciTilePorts.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTilePorts.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciTilePorts.TextSize = new System.Drawing.Size(300, 16);
            //
            // splitterItem1
            //
            this.splitterItem1.AllowHotTrack = true;
            this.splitterItem1.Location = new System.Drawing.Point(0, 360);
            this.splitterItem1.Name = "splitterItem1";
            this.splitterItem1.Size = new System.Drawing.Size(1320, 5);
            //
            // lciScanLog
            //
            this.lciScanLog.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.lciScanLog.AppearanceItemCaption.Options.UseFont = true;
            this.lciScanLog.Control = this.gridControlScanLog;
            this.lciScanLog.Location = new System.Drawing.Point(0, 365);
            this.lciScanLog.Name = "lciScanLog";
            this.lciScanLog.Size = new System.Drawing.Size(1320, 235);
            this.lciScanLog.Text = "Nhật ký quét";
            this.lciScanLog.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciScanLog.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciScanLog.TextSize = new System.Drawing.Size(300, 16);
            //
            // dxValidationProvider1
            //
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            //
            // timerPortCheck
            //
            this.timerPortCheck.Interval = 2000;
            this.timerPortCheck.Tick += new System.EventHandler(this.timerPortCheck_Tick);
            //
            // alertControlScan
            //
            this.alertControlScan.AllowHtmlText = true;
            this.alertControlScan.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);
            this.alertControlScan.AppearanceCaption.Options.UseFont = true;
            this.alertControlScan.AppearanceText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);
            this.alertControlScan.AppearanceText.Options.UseFont = true;
            this.alertControlScan.AppearanceText.Options.UseTextOptions = true;
            this.alertControlScan.AppearanceText.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.alertControlScan.AutoFormDelay = 6000;
            this.alertControlScan.AutoHeight = true;
            this.alertControlScan.FormLocation = DevExpress.XtraBars.Alerter.AlertFormLocation.BottomRight;
            this.alertControlScan.GetDesiredAlertFormWidth += new DevExpress.XtraBars.Alerter.GetDesiredFormWidthEventHandler(this.alertControlScan_GetDesiredAlertFormWidth);
            //
            // UCTestConnectDeviceSample
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCTestConnectDeviceSample";
            this.Size = new System.Drawing.Size(1320, 600);
            this.Load += new System.EventHandler(this.UCTestConnectDeviceSample_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlScanLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewScanLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPortCom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGuide)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPortCom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConnect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDisconnect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPending)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDeviceList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTilePorts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciScanLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblGuide;
        private DevExpress.XtraEditors.ComboBoxEdit cboPortCom;
        private DevExpress.XtraEditors.SimpleButton btnConnect;
        private DevExpress.XtraEditors.SimpleButton btnDisconnect;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnDeviceList;
        private DevExpress.XtraEditors.LabelControl lblPending;
        private DevExpress.XtraEditors.TileControl tileControlPorts;
        private DevExpress.XtraEditors.TileGroup tileGroupPorts;
        private DevExpress.XtraGrid.GridControl gridControlScanLog;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewScanLog;
        private DevExpress.XtraGrid.Columns.GridColumn gcScanTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcPortName;
        private DevExpress.XtraGrid.Columns.GridColumn gcCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcResult;
        private DevExpress.XtraLayout.LayoutControlItem lciGuide;
        private DevExpress.XtraLayout.LayoutControlItem lciPortCom;
        private DevExpress.XtraLayout.LayoutControlItem lciConnect;
        private DevExpress.XtraLayout.LayoutControlItem lciDisconnect;
        private DevExpress.XtraLayout.LayoutControlItem lciRefresh;
        private DevExpress.XtraLayout.LayoutControlItem lciPending;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciDeviceList;
        private DevExpress.XtraLayout.LayoutControlItem lciTilePorts;
        private DevExpress.XtraLayout.SplitterItem splitterItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciScanLog;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private System.Windows.Forms.Timer timerPortCheck;
        private DevExpress.XtraBars.Alerter.AlertControl alertControlScan;
    }
}
