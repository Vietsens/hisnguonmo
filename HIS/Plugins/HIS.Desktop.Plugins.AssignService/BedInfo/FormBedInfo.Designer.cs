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
namespace HIS.Desktop.Plugins.AssignService.BedInfo
{
    partial class FormBedInfo
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
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.spShareCount = new DevExpress.XtraEditors.SpinEdit();
            this.cboBed = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtBedCode = new DevExpress.XtraEditors.TextEdit();
            this.dtBedFinishTime = new DevExpress.XtraEditors.DateEdit();
            this.dtBedStartTime = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBedStartTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBedFinishTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciBedCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBed = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciShareCount = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spShareCount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBed.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBedCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtBedFinishTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtBedFinishTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtBedStartTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtBedStartTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedStartTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedFinishTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciBedCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciShareCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.spShareCount);
            this.layoutControl1.Controls.Add(this.cboBed);
            this.layoutControl1.Controls.Add(this.txtBedCode);
            this.layoutControl1.Controls.Add(this.dtBedFinishTime);
            this.layoutControl1.Controls.Add(this.dtBedStartTime);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(441, 124);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(332, 98);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(107, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // spShareCount
            // 
            this.spShareCount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spShareCount.Location = new System.Drawing.Point(97, 74);
            this.spShareCount.Name = "spShareCount";
            this.spShareCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spShareCount.Size = new System.Drawing.Size(342, 20);
            this.spShareCount.StyleController = this.layoutControl1;
            this.spShareCount.TabIndex = 8;
            this.spShareCount.KeyUp += new System.Windows.Forms.KeyEventHandler(this.spShareCount_KeyUp);
            // 
            // cboBed
            // 
            this.cboBed.Location = new System.Drawing.Point(160, 50);
            this.cboBed.Name = "cboBed";
            this.cboBed.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.cboBed.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboBed.Properties.NullText = "";
            this.cboBed.Properties.View = this.gridLookUpEdit1View;
            this.cboBed.Size = new System.Drawing.Size(279, 20);
            this.cboBed.StyleController = this.layoutControl1;
            this.cboBed.TabIndex = 7;
            this.cboBed.EditValueChanged += new System.EventHandler(this.cboBed_EditValueChanged);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            this.gridLookUpEdit1View.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridLookUpEdit1View_RowStyle);
            // 
            // txtBedCode
            // 
            this.txtBedCode.Location = new System.Drawing.Point(97, 50);
            this.txtBedCode.Name = "txtBedCode";
            this.txtBedCode.Size = new System.Drawing.Size(63, 20);
            this.txtBedCode.StyleController = this.layoutControl1;
            this.txtBedCode.TabIndex = 6;
            this.txtBedCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBedCode_PreviewKeyDown);
            // 
            // dtBedFinishTime
            // 
            this.dtBedFinishTime.EditValue = null;
            this.dtBedFinishTime.Location = new System.Drawing.Point(97, 26);
            this.dtBedFinishTime.Name = "dtBedFinishTime";
            this.dtBedFinishTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtBedFinishTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtBedFinishTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtBedFinishTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtBedFinishTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtBedFinishTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtBedFinishTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtBedFinishTime.Size = new System.Drawing.Size(342, 20);
            this.dtBedFinishTime.StyleController = this.layoutControl1;
            this.dtBedFinishTime.TabIndex = 5;
            this.dtBedFinishTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtBedFinishTime_PreviewKeyDown);
            // 
            // dtBedStartTime
            // 
            this.dtBedStartTime.EditValue = null;
            this.dtBedStartTime.Location = new System.Drawing.Point(97, 2);
            this.dtBedStartTime.Name = "dtBedStartTime";
            this.dtBedStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtBedStartTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtBedStartTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtBedStartTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtBedStartTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtBedStartTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtBedStartTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtBedStartTime.Size = new System.Drawing.Size(342, 20);
            this.dtBedStartTime.StyleController = this.layoutControl1;
            this.dtBedStartTime.TabIndex = 4;
            this.dtBedStartTime.EditValueChanged += new System.EventHandler(this.dtBedStartTime_EditValueChanged);
            this.dtBedStartTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtBedStartTime_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBedStartTime,
            this.lciBedFinishTime,
            this.LciBedCode,
            this.lciBed,
            this.lciShareCount,
            this.layoutControlItem6,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(441, 124);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciBedStartTime
            // 
            this.lciBedStartTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciBedStartTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciBedStartTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBedStartTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBedStartTime.Control = this.dtBedStartTime;
            this.lciBedStartTime.Location = new System.Drawing.Point(0, 0);
            this.lciBedStartTime.Name = "lciBedStartTime";
            this.lciBedStartTime.OptionsToolTip.ToolTip = "Thời gian bắt đầu";
            this.lciBedStartTime.Size = new System.Drawing.Size(441, 24);
            this.lciBedStartTime.Text = "TG Bắt đầu:";
            this.lciBedStartTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBedStartTime.TextSize = new System.Drawing.Size(90, 13);
            this.lciBedStartTime.TextToControlDistance = 5;
            // 
            // lciBedFinishTime
            // 
            this.lciBedFinishTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciBedFinishTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciBedFinishTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBedFinishTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBedFinishTime.Control = this.dtBedFinishTime;
            this.lciBedFinishTime.Location = new System.Drawing.Point(0, 24);
            this.lciBedFinishTime.Name = "lciBedFinishTime";
            this.lciBedFinishTime.OptionsToolTip.ToolTip = "Thời gian kết thúc";
            this.lciBedFinishTime.Size = new System.Drawing.Size(441, 24);
            this.lciBedFinishTime.Text = "TG kết thúc:";
            this.lciBedFinishTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBedFinishTime.TextSize = new System.Drawing.Size(90, 13);
            this.lciBedFinishTime.TextToControlDistance = 5;
            // 
            // LciBedCode
            // 
            this.LciBedCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.LciBedCode.AppearanceItemCaption.Options.UseForeColor = true;
            this.LciBedCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciBedCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciBedCode.Control = this.txtBedCode;
            this.LciBedCode.Location = new System.Drawing.Point(0, 48);
            this.LciBedCode.Name = "LciBedCode";
            this.LciBedCode.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.LciBedCode.Size = new System.Drawing.Size(160, 24);
            this.LciBedCode.Text = "Giường:";
            this.LciBedCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciBedCode.TextSize = new System.Drawing.Size(90, 13);
            this.LciBedCode.TextToControlDistance = 5;
            // 
            // lciBed
            // 
            this.lciBed.Control = this.cboBed;
            this.lciBed.Location = new System.Drawing.Point(160, 48);
            this.lciBed.Name = "lciBed";
            this.lciBed.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.lciBed.Size = new System.Drawing.Size(281, 24);
            this.lciBed.TextSize = new System.Drawing.Size(0, 0);
            this.lciBed.TextVisible = false;
            // 
            // lciShareCount
            // 
            this.lciShareCount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciShareCount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciShareCount.Control = this.spShareCount;
            this.lciShareCount.Location = new System.Drawing.Point(0, 72);
            this.lciShareCount.Name = "lciShareCount";
            this.lciShareCount.Size = new System.Drawing.Size(441, 24);
            this.lciShareCount.Text = "Nằm ghép:";
            this.lciShareCount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciShareCount.TextSize = new System.Drawing.Size(90, 13);
            this.lciShareCount.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnSave;
            this.layoutControlItem6.Location = new System.Drawing.Point(330, 96);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(111, 28);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 96);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(330, 28);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // FormBedInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 124);
            this.Controls.Add(this.layoutControl1);
            this.Name = "FormBedInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thông tin giường";
            this.Load += new System.EventHandler(this.FormBedInfo_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spShareCount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBed.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBedCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtBedFinishTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtBedFinishTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtBedStartTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtBedStartTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedStartTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedFinishTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciBedCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciShareCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SpinEdit spShareCount;
        private DevExpress.XtraEditors.GridLookUpEdit cboBed;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.TextEdit txtBedCode;
        private DevExpress.XtraEditors.DateEdit dtBedFinishTime;
        private DevExpress.XtraEditors.DateEdit dtBedStartTime;
        private DevExpress.XtraLayout.LayoutControlItem lciBedStartTime;
        private DevExpress.XtraLayout.LayoutControlItem lciBedFinishTime;
        private DevExpress.XtraLayout.LayoutControlItem LciBedCode;
        private DevExpress.XtraLayout.LayoutControlItem lciBed;
        private DevExpress.XtraLayout.LayoutControlItem lciShareCount;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
