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
namespace HIS.Desktop.Plugins.PatientInfo
{
    partial class frmPatientChoice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPatientChoice));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.grdInformation = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdChoose = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.grdCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdGender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdAddress = new DevExpress.XtraGrid.Columns.GridColumn();
            this.radianChoose = new DevExpress.XtraEditors.Repository.RepositoryItemRadioGroup();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciPatientInformation = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdInformation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radianChoose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientInformation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblDescription);
            this.layoutControl1.Controls.Add(this.btnClose);
            this.layoutControl1.Controls.Add(this.grdInformation);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(456, 128, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1114, 461);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDescription.Location = new System.Drawing.Point(2, 437);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(1035, 13);
            this.lblDescription.StyleController = this.layoutControl1;
            this.lblDescription.TabIndex = 7;
            this.lblDescription.Text = "Chọn bệnh nhân bằng cách bấm enter hoặc nháy đúp chuột vào bệnh nhân. Thêm hồ sơ " +
    "bệnh nhân mới chọn Bỏ qua";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(1041, 437);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(71, 22);
            this.btnClose.StyleController = this.layoutControl1;
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Bỏ qua";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // grdInformation
            // 
            this.grdInformation.Location = new System.Drawing.Point(2, 2);
            this.grdInformation.MainView = this.gridView1;
            this.grdInformation.Name = "grdInformation";
            this.grdInformation.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.radianChoose,
            this.repositoryItemCheckEdit1});
            this.grdInformation.Size = new System.Drawing.Size(1110, 431);
            this.grdInformation.TabIndex = 4;
            this.grdInformation.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.grdInformation.DoubleClick += new System.EventHandler(this.grdInformation_DoubleClick);
            this.grdInformation.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.grdInformation_PreviewKeyDown);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdChoose,
            this.grdCode,
            this.grdName,
            this.grdDate,
            this.grdGender,
            this.grdAddress});
            this.gridView1.GridControl = this.grdInformation;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowDetailButtons = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
            this.gridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridView1_MouseDown);
            // 
            // grdChoose
            // 
            this.grdChoose.AppearanceCell.Options.UseTextOptions = true;
            this.grdChoose.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdChoose.Caption = "Chọn";
            this.grdChoose.ColumnEdit = this.repositoryItemCheckEdit1;
            this.grdChoose.Name = "grdChoose";
            this.grdChoose.OptionsColumn.ShowCaption = false;
            this.grdChoose.Width = 30;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // grdCode
            // 
            this.grdCode.Caption = "Mã bệnh nhân";
            this.grdCode.FieldName = "PATIENT_CODE";
            this.grdCode.Name = "grdCode";
            this.grdCode.OptionsColumn.AllowEdit = false;
            this.grdCode.Visible = true;
            this.grdCode.VisibleIndex = 0;
            this.grdCode.Width = 116;
            // 
            // grdName
            // 
            this.grdName.Caption = "Tên bệnh nhân";
            this.grdName.FieldName = "VIR_PATIENT_NAME";
            this.grdName.Name = "grdName";
            this.grdName.OptionsColumn.AllowEdit = false;
            this.grdName.Visible = true;
            this.grdName.VisibleIndex = 1;
            this.grdName.Width = 150;
            // 
            // grdDate
            // 
            this.grdDate.AppearanceCell.Options.UseTextOptions = true;
            this.grdDate.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdDate.Caption = "Ngày sinh";
            this.grdDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.grdDate.FieldName = "DOB_DISPLAY";
            this.grdDate.Name = "grdDate";
            this.grdDate.OptionsColumn.AllowEdit = false;
            this.grdDate.Tag = new System.DateTime(2016, 10, 1, 11, 58, 9, 631);
            this.grdDate.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdDate.Visible = true;
            this.grdDate.VisibleIndex = 2;
            this.grdDate.Width = 120;
            // 
            // grdGender
            // 
            this.grdGender.Caption = "Giới tính";
            this.grdGender.FieldName = "GENDER_NAME";
            this.grdGender.Name = "grdGender";
            this.grdGender.OptionsColumn.AllowEdit = false;
            this.grdGender.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdGender.Visible = true;
            this.grdGender.VisibleIndex = 3;
            this.grdGender.Width = 97;
            // 
            // grdAddress
            // 
            this.grdAddress.Caption = "Địa chỉ";
            this.grdAddress.FieldName = "VIR_ADDRESS";
            this.grdAddress.Name = "grdAddress";
            this.grdAddress.OptionsColumn.AllowEdit = false;
            this.grdAddress.Visible = true;
            this.grdAddress.VisibleIndex = 4;
            this.grdAddress.Width = 595;
            // 
            // radianChoose
            // 
            this.radianChoose.Name = "radianChoose";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciPatientInformation,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1114, 461);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciPatientInformation
            // 
            this.lciPatientInformation.Control = this.grdInformation;
            this.lciPatientInformation.Location = new System.Drawing.Point(0, 0);
            this.lciPatientInformation.Name = "lciPatientInformation";
            this.lciPatientInformation.Size = new System.Drawing.Size(1114, 435);
            this.lciPatientInformation.TextSize = new System.Drawing.Size(0, 0);
            this.lciPatientInformation.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnClose;
            this.layoutControlItem2.Location = new System.Drawing.Point(1039, 435);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(75, 26);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(70, 26);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(75, 26);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.lblDescription;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 435);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(1039, 26);
            this.layoutControlItem3.Text = resources.GetString("layoutControlItem3.Text");
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextToControlDistance = 0;
            this.layoutControlItem3.TextVisible = false;
            // 
            // frmPatientChoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1114, 461);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmPatientChoice";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chọn thông tin bệnh nhân";
            this.Load += new System.EventHandler(this.PopupPatientInformation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdInformation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radianChoose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientInformation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraGrid.GridControl grdInformation;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn grdChoose;
        private DevExpress.XtraGrid.Columns.GridColumn grdName;
        private DevExpress.XtraGrid.Columns.GridColumn grdDate;
        private DevExpress.XtraGrid.Columns.GridColumn grdGender;
        private DevExpress.XtraGrid.Columns.GridColumn grdAddress;
        private DevExpress.XtraEditors.Repository.RepositoryItemRadioGroup radianChoose;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientInformation;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn grdCode;
        private DevExpress.XtraEditors.LabelControl lblDescription;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
    }
}
