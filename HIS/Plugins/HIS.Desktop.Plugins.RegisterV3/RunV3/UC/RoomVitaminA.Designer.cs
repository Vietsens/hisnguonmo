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
namespace HIS.Desktop.Plugins.RegisterV3.RunV3.UC
{
    partial class RoomVitaminA
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
            this.dtTime = new DevExpress.XtraEditors.DateEdit();
            this.chkKhac = new DevExpress.XtraEditors.CheckEdit();
            this.chkPhuNuSauSinh = new DevExpress.XtraEditors.CheckEdit();
            this.chkTreEm = new DevExpress.XtraEditors.CheckEdit();
            this.chkSick = new DevExpress.XtraEditors.CheckEdit();
            this.chkOneMonthBorn = new DevExpress.XtraEditors.CheckEdit();
            this.txtRoom = new DevExpress.XtraEditors.TextEdit();
            this.cboRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.chkSuyDinhDuong = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkKhac.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPhuNuSauSinh.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTreEm.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSick.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkOneMonthBorn.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSuyDinhDuong.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.chkSuyDinhDuong);
            this.layoutControl1.Controls.Add(this.dtTime);
            this.layoutControl1.Controls.Add(this.chkKhac);
            this.layoutControl1.Controls.Add(this.chkPhuNuSauSinh);
            this.layoutControl1.Controls.Add(this.chkTreEm);
            this.layoutControl1.Controls.Add(this.chkSick);
            this.layoutControl1.Controls.Add(this.chkOneMonthBorn);
            this.layoutControl1.Controls.Add(this.txtRoom);
            this.layoutControl1.Controls.Add(this.cboRoom);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(370, 205);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // dtTime
            // 
            this.dtTime.EditValue = null;
            this.dtTime.Location = new System.Drawing.Point(77, 2);
            this.dtTime.Name = "dtTime";
            this.dtTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy  HH:mm:ss";
            this.dtTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTime.Properties.EditFormat.FormatString = "dd/MM/yyyy  HH:mm:ss";
            this.dtTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTime.Properties.Mask.EditMask = "dd/MM/yyyy  HH:mm:ss";
            this.dtTime.Size = new System.Drawing.Size(291, 20);
            this.dtTime.StyleController = this.layoutControl1;
            this.dtTime.TabIndex = 15;
            this.dtTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtTime_PreviewKeyDown);
            // 
            // chkKhac
            // 
            this.chkKhac.Location = new System.Drawing.Point(272, 50);
            this.chkKhac.Name = "chkKhac";
            this.chkKhac.Properties.Caption = "Khác";
            this.chkKhac.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkKhac.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkKhac.Size = new System.Drawing.Size(96, 19);
            this.chkKhac.StyleController = this.layoutControl1;
            this.chkKhac.TabIndex = 14;
            this.chkKhac.CheckedChanged += new System.EventHandler(this.chkKhac_CheckedChanged);
            this.chkKhac.Click += new System.EventHandler(this.chkKhac_Click);
            this.chkKhac.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkKhac_PreviewKeyDown_1);
            // 
            // chkPhuNuSauSinh
            // 
            this.chkPhuNuSauSinh.Location = new System.Drawing.Point(140, 50);
            this.chkPhuNuSauSinh.Name = "chkPhuNuSauSinh";
            this.chkPhuNuSauSinh.Properties.Caption = "Phụ nữ sau sinh";
            this.chkPhuNuSauSinh.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkPhuNuSauSinh.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkPhuNuSauSinh.Size = new System.Drawing.Size(128, 19);
            this.chkPhuNuSauSinh.StyleController = this.layoutControl1;
            this.chkPhuNuSauSinh.TabIndex = 13;
            this.chkPhuNuSauSinh.CheckedChanged += new System.EventHandler(this.chkPhuNuSauSinh_CheckedChanged);
            this.chkPhuNuSauSinh.Click += new System.EventHandler(this.chkPhuNuSauSinh_Click);
            this.chkPhuNuSauSinh.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkPhuNuSauSinh_PreviewKeyDown);
            // 
            // chkTreEm
            // 
            this.chkTreEm.Location = new System.Drawing.Point(9, 50);
            this.chkTreEm.Name = "chkTreEm";
            this.chkTreEm.Properties.Caption = "Trẻ em";
            this.chkTreEm.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkTreEm.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkTreEm.Size = new System.Drawing.Size(127, 19);
            this.chkTreEm.StyleController = this.layoutControl1;
            this.chkTreEm.TabIndex = 12;
            this.chkTreEm.CheckedChanged += new System.EventHandler(this.chkTreEm_CheckedChanged);
            this.chkTreEm.Click += new System.EventHandler(this.chkTreEm_Click);
            this.chkTreEm.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkTreEm_PreviewKeyDown);
            // 
            // chkSick
            // 
            this.chkSick.Location = new System.Drawing.Point(2, 73);
            this.chkSick.Name = "chkSick";
            this.chkSick.Properties.Caption = "Bị bệnh thiếu Vitamin A";
            this.chkSick.Properties.FullFocusRect = true;
            this.chkSick.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkSick.Size = new System.Drawing.Size(366, 19);
            this.chkSick.StyleController = this.layoutControl1;
            this.chkSick.TabIndex = 11;
            this.chkSick.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkSick_PreviewKeyDown);
            // 
            // chkOneMonthBorn
            // 
            this.chkOneMonthBorn.Location = new System.Drawing.Point(2, 119);
            this.chkOneMonthBorn.Name = "chkOneMonthBorn";
            this.chkOneMonthBorn.Properties.Caption = "1 tháng sau sinh";
            this.chkOneMonthBorn.Properties.FullFocusRect = true;
            this.chkOneMonthBorn.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkOneMonthBorn.Size = new System.Drawing.Size(366, 19);
            this.chkOneMonthBorn.StyleController = this.layoutControl1;
            this.chkOneMonthBorn.TabIndex = 10;
            this.chkOneMonthBorn.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkOneMonthBorn_PreviewKeyDown);
            // 
            // txtRoom
            // 
            this.txtRoom.Location = new System.Drawing.Point(77, 26);
            this.txtRoom.Name = "txtRoom";
            this.txtRoom.Size = new System.Drawing.Size(61, 20);
            this.txtRoom.StyleController = this.layoutControl1;
            this.txtRoom.TabIndex = 8;
            this.txtRoom.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtRoom_PreviewKeyDown);
            // 
            // cboRoom
            // 
            this.cboRoom.Location = new System.Drawing.Point(138, 26);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboRoom.Properties.AutoComplete = false;
            this.cboRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboRoom.Properties.NullText = "";
            this.cboRoom.Properties.View = this.gridLookUpEdit1View;
            this.cboRoom.Size = new System.Drawing.Size(230, 20);
            this.cboRoom.StyleController = this.layoutControl1;
            this.cboRoom.TabIndex = 4;
            this.cboRoom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboRoom_Closed);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem5,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.layoutControlItem6,
            this.layoutControlItem9});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(370, 205);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.cboRoom;
            this.layoutControlItem1.Location = new System.Drawing.Point(138, 24);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem1.Size = new System.Drawing.Size(232, 24);
            this.layoutControlItem1.Text = "Phòng:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem5.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem5.Control = this.txtRoom;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutControlItem5.Size = new System.Drawing.Size(138, 24);
            this.layoutControlItem5.Text = "Phòng:";
            this.layoutControlItem5.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem5.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.chkOneMonthBorn;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 117);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(370, 88);
            this.layoutControlItem2.Text = " ";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.chkSick;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 71);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(370, 23);
            this.layoutControlItem3.Text = " ";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextToControlDistance = 0;
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.chkTreEm;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(138, 23);
            this.layoutControlItem4.Text = " ";
            this.layoutControlItem4.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(2, 0);
            this.layoutControlItem4.TextToControlDistance = 5;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.chkPhuNuSauSinh;
            this.layoutControlItem7.Location = new System.Drawing.Point(138, 48);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(132, 23);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.chkKhac;
            this.layoutControlItem8.Location = new System.Drawing.Point(270, 48);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(100, 23);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem6.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem6.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem6.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem6.Control = this.dtTime;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.OptionsToolTip.ToolTip = "Thời gian uống";
            this.layoutControlItem6.Size = new System.Drawing.Size(370, 24);
            this.layoutControlItem6.Text = "Thời gian:";
            this.layoutControlItem6.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem6.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // chkSuyDinhDuong
            // 
            this.chkSuyDinhDuong.Location = new System.Drawing.Point(2, 96);
            this.chkSuyDinhDuong.Name = "chkSuyDinhDuong";
            this.chkSuyDinhDuong.Properties.Caption = "Còi xương suy dinh dưỡng";
            this.chkSuyDinhDuong.Properties.FullFocusRect = true;
            this.chkSuyDinhDuong.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkSuyDinhDuong.Size = new System.Drawing.Size(366, 19);
            this.chkSuyDinhDuong.StyleController = this.layoutControl1;
            this.chkSuyDinhDuong.TabIndex = 16;
            this.chkSuyDinhDuong.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkSuyDinhDuong_PreviewKeyDown);
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.chkSuyDinhDuong;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 94);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(370, 23);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // RoomVitaminA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "RoomVitaminA";
            this.Size = new System.Drawing.Size(370, 205);
            this.Load += new System.EventHandler(this.RoomVitaminA_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkKhac.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPhuNuSauSinh.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTreEm.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSick.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkOneMonthBorn.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSuyDinhDuong.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        internal DevExpress.XtraEditors.GridLookUpEdit cboRoom;
        internal DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        internal DevExpress.XtraEditors.TextEdit txtRoom;
        internal DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        internal DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        internal DevExpress.XtraEditors.CheckEdit chkSick;
        internal DevExpress.XtraEditors.CheckEdit chkOneMonthBorn;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        internal DevExpress.XtraEditors.CheckEdit chkKhac;
        internal DevExpress.XtraEditors.CheckEdit chkPhuNuSauSinh;
        internal DevExpress.XtraEditors.CheckEdit chkTreEm;
        internal DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        internal DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        internal DevExpress.XtraEditors.DateEdit dtTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        internal DevExpress.XtraEditors.CheckEdit chkSuyDinhDuong;
        internal DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
    }
}
