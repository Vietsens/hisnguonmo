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
namespace HIS.Desktop.Plugins.PeriodList.Report
{
    partial class frmCreateReportByDepartment
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lblPeriod = new DevExpress.XtraEditors.LabelControl();
            this.lciPeriod = new DevExpress.XtraLayout.LayoutControlItem();
            this.txtDepartmentCode = new DevExpress.XtraEditors.TextEdit();
            this.lciDepartment = new DevExpress.XtraLayout.LayoutControlItem();
            this.cboDepartment = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.cboDepartment);
            this.layoutControl1.Controls.Add(this.txtDepartmentCode);
            this.layoutControl1.Controls.Add(this.lblPeriod);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(440, 93);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciPeriod,
            this.lciDepartment,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(440, 93);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lblPeriod
            // 
            this.lblPeriod.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPeriod.Location = new System.Drawing.Point(97, 2);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(341, 20);
            this.lblPeriod.StyleController = this.layoutControl1;
            this.lblPeriod.TabIndex = 4;
            // 
            // lciPeriod
            // 
            this.lciPeriod.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPeriod.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPeriod.Control = this.lblPeriod;
            this.lciPeriod.Location = new System.Drawing.Point(0, 0);
            this.lciPeriod.Name = "lciPeriod";
            this.lciPeriod.Size = new System.Drawing.Size(440, 24);
            this.lciPeriod.Text = "Kỳ:";
            this.lciPeriod.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPeriod.TextSize = new System.Drawing.Size(90, 20);
            this.lciPeriod.TextToControlDistance = 5;
            // 
            // txtDepartmentCode
            // 
            this.txtDepartmentCode.Location = new System.Drawing.Point(97, 26);
            this.txtDepartmentCode.Name = "txtDepartmentCode";
            this.txtDepartmentCode.Size = new System.Drawing.Size(63, 20);
            this.txtDepartmentCode.StyleController = this.layoutControl1;
            this.txtDepartmentCode.TabIndex = 5;
            this.txtDepartmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDepartmentCode_PreviewKeyDown);
            // 
            // lciDepartment
            // 
            this.lciDepartment.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciDepartment.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciDepartment.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDepartment.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDepartment.Control = this.txtDepartmentCode;
            this.lciDepartment.Location = new System.Drawing.Point(0, 24);
            this.lciDepartment.Name = "lciDepartment";
            this.lciDepartment.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciDepartment.Size = new System.Drawing.Size(160, 24);
            this.lciDepartment.Text = "Khoa:";
            this.lciDepartment.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDepartment.TextSize = new System.Drawing.Size(90, 20);
            this.lciDepartment.TextToControlDistance = 5;
            // 
            // cboDepartment
            // 
            this.cboDepartment.Location = new System.Drawing.Point(160, 26);
            this.cboDepartment.Name = "cboDepartment";
            this.cboDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDepartment.Properties.NullText = "";
            this.cboDepartment.Size = new System.Drawing.Size(278, 20);
            this.cboDepartment.StyleController = this.layoutControl1;
            this.cboDepartment.TabIndex = 6;
            this.cboDepartment.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDepartment_Closed);
            this.cboDepartment.EditValueChanged += new System.EventHandler(this.cboDepartment_EditValueChanged);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.cboDepartment;
            this.layoutControlItem3.Location = new System.Drawing.Point(160, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem3.Size = new System.Drawing.Size(280, 24);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(332, 50);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Lưu";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnSave;
            this.layoutControlItem4.Location = new System.Drawing.Point(330, 48);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(110, 45);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 48);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(330, 45);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // frmCreateReportByDepartment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 93);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmCreateReportByDepartment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tạo báo cáo theo khoa";
            this.Load += new System.EventHandler(this.frmCreateReportByDepartment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.LookUpEdit cboDepartment;
        private DevExpress.XtraEditors.TextEdit txtDepartmentCode;
        private DevExpress.XtraEditors.LabelControl lblPeriod;
        private DevExpress.XtraLayout.LayoutControlItem lciPeriod;
        private DevExpress.XtraLayout.LayoutControlItem lciDepartment;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
