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
namespace HIS.UC.DateEditor
{
    partial class UCDate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDate));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.pnlIntructionTime = new System.Windows.Forms.Panel();
            this.dtInstructionTime = new DevExpress.XtraEditors.DateEdit();
            this.txtInstructionTime = new DevExpress.XtraEditors.ButtonEdit();
            this.timeIntruction = new DevExpress.XtraEditors.TimeSpanEdit();
            this.chkMultiIntructionTime = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcichkMultiDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDateEditor = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            this.pnlIntructionTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInstructionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeIntruction.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMultiIntructionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcichkMultiDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDateEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.pnlIntructionTime);
            this.layoutControl1.Controls.Add(this.timeIntruction);
            this.layoutControl1.Controls.Add(this.chkMultiIntructionTime);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(350, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // pnlIntructionTime
            // 
            this.pnlIntructionTime.Controls.Add(this.dtInstructionTime);
            this.pnlIntructionTime.Controls.Add(this.txtInstructionTime);
            this.pnlIntructionTime.Location = new System.Drawing.Point(97, 2);
            this.pnlIntructionTime.Name = "pnlIntructionTime";
            this.pnlIntructionTime.Size = new System.Drawing.Size(100, 20);
            this.pnlIntructionTime.TabIndex = 0;
            // 
            // dtInstructionTime
            // 
            this.dtInstructionTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtInstructionTime.EditValue = null;
            this.dtInstructionTime.Location = new System.Drawing.Point(0, 0);
            this.dtInstructionTime.Name = "dtInstructionTime";
            this.dtInstructionTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInstructionTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInstructionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtInstructionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInstructionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtInstructionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInstructionTime.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtInstructionTime.Size = new System.Drawing.Size(100, 20);
            this.dtInstructionTime.TabIndex = 1;
            this.dtInstructionTime.EditValueChanged += new System.EventHandler(this.dtInstructionTime_EditValueChanged);
            this.dtInstructionTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtInstructionTime_TabMedicine_PreviewKeyDown);
            // 
            // txtInstructionTime
            // 
            this.txtInstructionTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInstructionTime.Location = new System.Drawing.Point(0, 0);
            this.txtInstructionTime.Name = "txtInstructionTime";
            this.txtInstructionTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("txtInstructionTime.Properties.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Giữ Ctrl để chọn nhiều ngày", null, null, true)});
            this.txtInstructionTime.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.txtInstructionTime.Size = new System.Drawing.Size(100, 22);
            this.txtInstructionTime.TabIndex = 1;
            this.txtInstructionTime.Visible = false;
            this.txtInstructionTime.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtInstructionTime_ButtonClick);
            this.txtInstructionTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtInstructionTime_PreviewKeyDown);
            // 
            // timeIntruction
            // 
            this.timeIntruction.EditValue = System.TimeSpan.Parse("00:00:00");
            this.timeIntruction.EnterMoveNextControl = true;
            this.timeIntruction.Location = new System.Drawing.Point(201, 2);
            this.timeIntruction.Name = "timeIntruction";
            this.timeIntruction.Properties.AllowEditDays = false;
            this.timeIntruction.Properties.AllowEditSeconds = false;
            this.timeIntruction.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.timeIntruction.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeIntruction.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeIntruction.Properties.Mask.EditMask = "HH:mm";
            this.timeIntruction.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.timeIntruction.Size = new System.Drawing.Size(67, 20);
            this.timeIntruction.StyleController = this.layoutControl1;
            this.timeIntruction.TabIndex = 1;
            this.timeIntruction.Leave += new System.EventHandler(this.timeIntruction_Leave);
            // 
            // chkMultiIntructionTime
            // 
            this.chkMultiIntructionTime.Location = new System.Drawing.Point(272, 2);
            this.chkMultiIntructionTime.Name = "chkMultiIntructionTime";
            this.chkMultiIntructionTime.Properties.Caption = "Nhiều ngày";
            this.chkMultiIntructionTime.Properties.FullFocusRect = true;
            this.chkMultiIntructionTime.Size = new System.Drawing.Size(76, 19);
            this.chkMultiIntructionTime.StyleController = this.layoutControl1;
            this.chkMultiIntructionTime.TabIndex = 2;
            this.chkMultiIntructionTime.CheckedChanged += new System.EventHandler(this.chkMultiIntructionTime_CheckedChanged);
            this.chkMultiIntructionTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkMultiIntructionTime_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lcichkMultiDate,
            this.lciDateEditor});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(350, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.timeIntruction;
            this.layoutControlItem1.Location = new System.Drawing.Point(199, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(71, 24);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lcichkMultiDate
            // 
            this.lcichkMultiDate.Control = this.chkMultiIntructionTime;
            this.lcichkMultiDate.Location = new System.Drawing.Point(270, 0);
            this.lcichkMultiDate.Name = "lcichkMultiDate";
            this.lcichkMultiDate.Size = new System.Drawing.Size(80, 24);
            this.lcichkMultiDate.TextSize = new System.Drawing.Size(0, 0);
            this.lcichkMultiDate.TextVisible = false;
            // 
            // lciDateEditor
            // 
            this.lciDateEditor.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciDateEditor.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciDateEditor.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDateEditor.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDateEditor.Control = this.pnlIntructionTime;
            this.lciDateEditor.Location = new System.Drawing.Point(0, 0);
            this.lciDateEditor.Name = "lciDateEditor";
            this.lciDateEditor.OptionsToolTip.ToolTip = "Thời gian chỉ định";
            this.lciDateEditor.Size = new System.Drawing.Size(199, 24);
            this.lciDateEditor.Text = "TG chỉ định:";
            this.lciDateEditor.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDateEditor.TextSize = new System.Drawing.Size(90, 20);
            this.lciDateEditor.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // UCDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UCDate";
            this.Size = new System.Drawing.Size(350, 24);
            this.Load += new System.EventHandler(this.UCIcd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            this.pnlIntructionTime.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInstructionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeIntruction.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMultiIntructionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcichkMultiDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDateEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        internal DevExpress.XtraEditors.TimeSpanEdit timeIntruction;
        internal DevExpress.XtraEditors.CheckEdit chkMultiIntructionTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem lcichkMultiDate;
        private System.Windows.Forms.Panel pnlIntructionTime;
        internal DevExpress.XtraEditors.DateEdit dtInstructionTime;
        private DevExpress.XtraEditors.ButtonEdit txtInstructionTime;
        private DevExpress.XtraLayout.LayoutControlItem lciDateEditor;
    }
}
