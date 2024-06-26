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
namespace HIS.Desktop.Plugins.ExpMestSaleCreate.FormChoosePatient
{
    partial class FormPatients
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
            this.lblInfo = new DevExpress.XtraEditors.LabelControl();
            this.btnIgnore = new DevExpress.XtraEditors.SimpleButton();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gc_PatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_PatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_Dob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_Gender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_Address = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciInfo = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblInfo);
            this.layoutControl1.Controls.Add(this.btnIgnore);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(770, 361);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblInfo.Location = new System.Drawing.Point(2, 337);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(656, 13);
            this.lblInfo.StyleController = this.layoutControl1;
            this.lblInfo.TabIndex = 6;
            this.lblInfo.Text = "Chọn bệnh nhân bằng cách bấm enter hoặc nháy đúp chuột vào bệnh nhân. Thêm hồ sơ " +
    "bệnh nhân mới chọn Bỏ qua";
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(662, 337);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(106, 22);
            this.btnIgnore.StyleController = this.layoutControl1;
            this.btnIgnore.TabIndex = 5;
            this.btnIgnore.Text = "Bỏ qua";
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(2, 2);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(766, 331);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gc_PatientCode,
            this.gc_PatientName,
            this.gc_Dob,
            this.gc_Gender,
            this.gc_Address});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
            this.gridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridView1_KeyDown);
            this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // gc_PatientCode
            // 
            this.gc_PatientCode.Caption = "Mã bệnh nhân";
            this.gc_PatientCode.FieldName = "PATIENT_CODE";
            this.gc_PatientCode.Name = "gc_PatientCode";
            this.gc_PatientCode.OptionsColumn.AllowEdit = false;
            this.gc_PatientCode.Visible = true;
            this.gc_PatientCode.VisibleIndex = 0;
            this.gc_PatientCode.Width = 80;
            // 
            // gc_PatientName
            // 
            this.gc_PatientName.Caption = "Tên bệnh nhân";
            this.gc_PatientName.FieldName = "VIR_PATIENT_NAME";
            this.gc_PatientName.Name = "gc_PatientName";
            this.gc_PatientName.OptionsColumn.AllowEdit = false;
            this.gc_PatientName.Visible = true;
            this.gc_PatientName.VisibleIndex = 1;
            this.gc_PatientName.Width = 150;
            // 
            // gc_Dob
            // 
            this.gc_Dob.Caption = "Ngày sinh";
            this.gc_Dob.FieldName = "DOB_STR";
            this.gc_Dob.Name = "gc_Dob";
            this.gc_Dob.OptionsColumn.AllowEdit = false;
            this.gc_Dob.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gc_Dob.Visible = true;
            this.gc_Dob.VisibleIndex = 2;
            this.gc_Dob.Width = 120;
            // 
            // gc_Gender
            // 
            this.gc_Gender.Caption = "Giới tính";
            this.gc_Gender.FieldName = "GENDER_NAME";
            this.gc_Gender.Name = "gc_Gender";
            this.gc_Gender.OptionsColumn.AllowEdit = false;
            this.gc_Gender.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gc_Gender.Visible = true;
            this.gc_Gender.VisibleIndex = 3;
            this.gc_Gender.Width = 70;
            // 
            // gc_Address
            // 
            this.gc_Address.Caption = "Địa chỉ";
            this.gc_Address.FieldName = "VIR_ADDRESS";
            this.gc_Address.Name = "gc_Address";
            this.gc_Address.OptionsColumn.AllowEdit = false;
            this.gc_Address.Visible = true;
            this.gc_Address.VisibleIndex = 4;
            this.gc_Address.Width = 500;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.lciInfo});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(770, 361);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(770, 335);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnIgnore;
            this.layoutControlItem2.Location = new System.Drawing.Point(660, 335);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // lciInfo
            // 
            this.lciInfo.Control = this.lblInfo;
            this.lciInfo.Location = new System.Drawing.Point(0, 335);
            this.lciInfo.Name = "lciInfo";
            this.lciInfo.Size = new System.Drawing.Size(660, 26);
            this.lciInfo.TextSize = new System.Drawing.Size(0, 0);
            this.lciInfo.TextVisible = false;
            // 
            // FormPatients
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 361);
            this.Controls.Add(this.layoutControl1);
            this.Name = "FormPatients";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chọn thông tin bệnh nhân";
            this.Load += new System.EventHandler(this.FormPatients_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblInfo;
        private DevExpress.XtraEditors.SimpleButton btnIgnore;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem lciInfo;
        private DevExpress.XtraGrid.Columns.GridColumn gc_PatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn gc_PatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gc_Dob;
        private DevExpress.XtraGrid.Columns.GridColumn gc_Gender;
        private DevExpress.XtraGrid.Columns.GridColumn gc_Address;
    }
}
