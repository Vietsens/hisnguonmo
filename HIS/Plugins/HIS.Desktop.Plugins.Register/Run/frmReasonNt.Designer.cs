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
namespace HIS.Desktop.Plugins.Register.Run
{
    partial class frmReasonNt
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
            this.txtReasonCode = new DevExpress.XtraEditors.TextEdit();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            this.grcReason = new DevExpress.XtraGrid.GridControl();
            this.grvReason = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.HOSPITALIZE_REASON_CODE = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtSearchvalue = new DevExpress.XtraEditors.TextEdit();
            this.txtReasonName = new DevExpress.XtraEditors.ButtonEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.listReason = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtReasonCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchvalue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReasonName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.listReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.txtReasonCode);
            this.layoutControl1.Controls.Add(this.ucPaging);
            this.layoutControl1.Controls.Add(this.grcReason);
            this.layoutControl1.Controls.Add(this.txtSearchvalue);
            this.layoutControl1.Controls.Add(this.txtReasonName);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(544, 332);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(405, 298);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(127, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Chọn (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtReasonCode
            // 
            this.txtReasonCode.Enabled = false;
            this.txtReasonCode.Location = new System.Drawing.Point(12, 298);
            this.txtReasonCode.Name = "txtReasonCode";
            this.txtReasonCode.Size = new System.Drawing.Size(118, 20);
            this.txtReasonCode.StyleController = this.layoutControl1;
            this.txtReasonCode.TabIndex = 7;
            this.txtReasonCode.EditValueChanged += new System.EventHandler(this.txtReasonCode_EditValueChanged);
            // 
            // ucPaging
            // 
            this.ucPaging.Location = new System.Drawing.Point(12, 274);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(520, 20);
            this.ucPaging.TabIndex = 6;
            // 
            // grcReason
            // 
            this.grcReason.Location = new System.Drawing.Point(12, 36);
            this.grcReason.MainView = this.grvReason;
            this.grcReason.Name = "grcReason";
            this.grcReason.Size = new System.Drawing.Size(520, 234);
            this.grcReason.TabIndex = 5;
            this.grcReason.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvReason});
            // 
            // grvReason
            // 
            this.grvReason.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.HOSPITALIZE_REASON_CODE,
            this.gridColumn3});
            this.grvReason.GridControl = this.grcReason;
            this.grvReason.Name = "grvReason";
            this.grvReason.OptionsView.ShowGroupPanel = false;
            this.grvReason.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.grvReason_RowCellClick);
            this.grvReason.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.grvReason_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "STT";
            this.gridColumn1.FieldName = "STT";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 50;
            // 
            // HOSPITALIZE_REASON_CODE
            // 
            this.HOSPITALIZE_REASON_CODE.Caption = "Mã";
            this.HOSPITALIZE_REASON_CODE.FieldName = "HOSPITALIZE_REASON_CODE";
            this.HOSPITALIZE_REASON_CODE.Name = "HOSPITALIZE_REASON_CODE";
            this.HOSPITALIZE_REASON_CODE.OptionsColumn.AllowEdit = false;
            this.HOSPITALIZE_REASON_CODE.Visible = true;
            this.HOSPITALIZE_REASON_CODE.VisibleIndex = 1;
            this.HOSPITALIZE_REASON_CODE.Width = 142;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Tên";
            this.gridColumn3.FieldName = "HOSPITALIZE_REASON_NAME";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 214;
            // 
            // txtSearchvalue
            // 
            this.txtSearchvalue.EditValue = "";
            this.txtSearchvalue.Location = new System.Drawing.Point(12, 12);
            this.txtSearchvalue.Name = "txtSearchvalue";
            this.txtSearchvalue.Properties.NullText = "Từ khóa tìm kiếm";
            this.txtSearchvalue.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearchvalue.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearchvalue.Size = new System.Drawing.Size(520, 20);
            this.txtSearchvalue.StyleController = this.layoutControl1;
            this.txtSearchvalue.TabIndex = 4;
            this.txtSearchvalue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchvalue_KeyDown);
            // 
            // txtReasonName
            // 
            this.txtReasonName.Location = new System.Drawing.Point(134, 298);
            this.txtReasonName.Name = "txtReasonName";
            this.txtReasonName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.txtReasonName.Size = new System.Drawing.Size(267, 20);
            this.txtReasonName.StyleController = this.layoutControl1;
            this.txtReasonName.TabIndex = 8;
            this.txtReasonName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtReasonName_ButtonClick);
            this.txtReasonName.EditValueChanged += new System.EventHandler(this.txtReasonName_EditValueChanged);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.listReason,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(544, 332);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.txtSearchvalue;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(524, 24);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // listReason
            // 
            this.listReason.Control = this.grcReason;
            this.listReason.Location = new System.Drawing.Point(0, 24);
            this.listReason.Name = "listReason";
            this.listReason.Size = new System.Drawing.Size(524, 238);
            this.listReason.TextSize = new System.Drawing.Size(0, 0);
            this.listReason.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.ucPaging;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 262);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(524, 24);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.txtReasonCode;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 286);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(122, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.txtReasonName;
            this.layoutControlItem5.Location = new System.Drawing.Point(122, 286);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(271, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnSave;
            this.layoutControlItem6.Location = new System.Drawing.Point(393, 286);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(131, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // frmReasonNt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 332);
            this.Controls.Add(this.layoutControl1);
            this.KeyPreview = true;
            this.Name = "frmReasonNt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lý do vào nội trú";
            this.Load += new System.EventHandler(this.frmReasonNt_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmReasonNt_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtReasonCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchvalue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReasonName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.listReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.TextEdit txtReasonCode;
        private Inventec.UC.Paging.UcPaging ucPaging;
        private DevExpress.XtraGrid.GridControl grcReason;
        private DevExpress.XtraGrid.Views.Grid.GridView grvReason;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn HOSPITALIZE_REASON_CODE;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraEditors.TextEdit txtSearchvalue;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem listReason;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.ButtonEdit txtReasonName;
    }
}
