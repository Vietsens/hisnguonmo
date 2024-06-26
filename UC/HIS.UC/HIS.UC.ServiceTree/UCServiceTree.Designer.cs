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
namespace HIS.UC.ServiceTree
{
    partial class UCServiceTree
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
            this.trvService = new DevExpress.XtraTreeList.TreeList();
            this.treeColumnServiceCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnServiceName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnServiceUnitName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnServiceTypeCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnServiceTypeName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnHeinServiceTypeCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnHeinServiceTypeName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKeyword = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.trvService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // trvService
            // 
            this.trvService.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.trvService.Appearance.FocusedCell.Options.UseBackColor = true;
            this.trvService.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.trvService.Appearance.FocusedRow.Options.UseBackColor = true;
            this.trvService.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.trvService.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.trvService.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeColumnServiceCode,
            this.treeColumnServiceName,
            this.treeColumnServiceUnitName,
            this.treeColumnServiceTypeCode,
            this.treeColumnServiceTypeName,
            this.treeColumnHeinServiceTypeCode,
            this.treeColumnHeinServiceTypeName});
            this.trvService.Cursor = System.Windows.Forms.Cursors.Default;
            this.trvService.KeyFieldName = "CONCRETE_ID";
            this.trvService.Location = new System.Drawing.Point(2, 26);
            this.trvService.Name = "trvService";
            this.trvService.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.trvService.OptionsBehavior.EnableFiltering = true;
            this.trvService.OptionsBehavior.PopulateServiceColumns = true;
            this.trvService.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Smart;
            this.trvService.OptionsFind.FindDelay = 100;
            this.trvService.OptionsFind.FindNullPrompt = "Nhập chuỗi tìm kiếm ...";
            this.trvService.OptionsFind.ShowClearButton = false;
            this.trvService.OptionsFind.ShowFindButton = false;
            this.trvService.OptionsView.AutoWidth = false;
            this.trvService.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFullFocus;
            this.trvService.OptionsView.ShowCheckBoxes = true;
            this.trvService.OptionsView.ShowHorzLines = false;
            this.trvService.OptionsView.ShowIndicator = false;
            this.trvService.OptionsView.ShowVertLines = false;
            this.trvService.ParentFieldName = "PARENT_ID";
            this.trvService.Size = new System.Drawing.Size(444, 394);
            this.trvService.TabIndex = 3;
            this.trvService.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.trvService_GetStateImage);
            this.trvService.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(this.trvService_GetSelectImage);
            this.trvService.StateImageClick += new DevExpress.XtraTreeList.NodeClickEventHandler(this.trvService_StateImageClick);
            this.trvService.SelectImageClick += new DevExpress.XtraTreeList.NodeClickEventHandler(this.trvService_SelectImageClick);
            this.trvService.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.trvService_CustomNodeCellEdit);
            this.trvService.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.trvService_NodeCellStyle);
            this.trvService.CustomUnboundColumnData += new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.trvService_CustomUnboundColumnData);
            this.trvService.BeforeCheckNode += new DevExpress.XtraTreeList.CheckNodeEventHandler(this.trvService_BeforeCheckNode);
            this.trvService.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.trvService_AfterCheckNode);
            this.trvService.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.trvService_PopupMenuShowing);
            this.trvService.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trvService_KeyDown);
            // 
            // treeColumnServiceCode
            // 
            this.treeColumnServiceCode.Caption = "Mã dịch vụ";
            this.treeColumnServiceCode.FieldName = "SERVICE_CODE";
            this.treeColumnServiceCode.MinWidth = 34;
            this.treeColumnServiceCode.Name = "treeColumnServiceCode";
            this.treeColumnServiceCode.OptionsColumn.AllowEdit = false;
            this.treeColumnServiceCode.Visible = true;
            this.treeColumnServiceCode.VisibleIndex = 0;
            this.treeColumnServiceCode.Width = 105;
            // 
            // treeColumnServiceName
            // 
            this.treeColumnServiceName.Caption = "Tên dịch vụ";
            this.treeColumnServiceName.FieldName = "SERVICE_NAME";
            this.treeColumnServiceName.Name = "treeColumnServiceName";
            this.treeColumnServiceName.OptionsColumn.AllowEdit = false;
            this.treeColumnServiceName.Visible = true;
            this.treeColumnServiceName.VisibleIndex = 1;
            this.treeColumnServiceName.Width = 335;
            // 
            // treeColumnServiceUnitName
            // 
            this.treeColumnServiceUnitName.Caption = "Đơn vị tính";
            this.treeColumnServiceUnitName.FieldName = "SERVICE_UNIT_NAME";
            this.treeColumnServiceUnitName.Name = "treeColumnServiceUnitName";
            this.treeColumnServiceUnitName.Width = 100;
            // 
            // treeColumnServiceTypeCode
            // 
            this.treeColumnServiceTypeCode.Caption = "Mã loại DV";
            this.treeColumnServiceTypeCode.FieldName = "SERVICE_TYPE_CODE";
            this.treeColumnServiceTypeCode.Name = "treeColumnServiceTypeCode";
            this.treeColumnServiceTypeCode.OptionsColumn.AllowEdit = false;
            this.treeColumnServiceTypeCode.Width = 100;
            // 
            // treeColumnServiceTypeName
            // 
            this.treeColumnServiceTypeName.Caption = "Tên loại DV";
            this.treeColumnServiceTypeName.FieldName = "SERVICE_TYPE_NAME";
            this.treeColumnServiceTypeName.Name = "treeColumnServiceTypeName";
            this.treeColumnServiceTypeName.Width = 150;
            // 
            // treeColumnHeinServiceTypeCode
            // 
            this.treeColumnHeinServiceTypeCode.Caption = "Mã bảo hiểm";
            this.treeColumnHeinServiceTypeCode.FieldName = "HEIN_SERVICE_TYPE_CODE";
            this.treeColumnHeinServiceTypeCode.Name = "treeColumnHeinServiceTypeCode";
            this.treeColumnHeinServiceTypeCode.Width = 100;
            // 
            // treeColumnHeinServiceTypeName
            // 
            this.treeColumnHeinServiceTypeName.Caption = "Tên bảo hiểm";
            this.treeColumnHeinServiceTypeName.FieldName = "HEIN_SERVICE_TYPE_NAME";
            this.treeColumnHeinServiceTypeName.Name = "treeColumnHeinServiceTypeName";
            this.treeColumnHeinServiceTypeName.Width = 150;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.trvService);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(448, 422);
            this.layoutControl1.TabIndex = 31;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(444, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 1;
            this.txtKeyword.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKeyword,
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(448, 422);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciKeyword
            // 
            this.lciKeyword.Control = this.txtKeyword;
            this.lciKeyword.Location = new System.Drawing.Point(0, 0);
            this.lciKeyword.Name = "lciKeyword";
            this.lciKeyword.Size = new System.Drawing.Size(448, 24);
            this.lciKeyword.TextSize = new System.Drawing.Size(0, 0);
            this.lciKeyword.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.trvService;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(448, 398);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // UCServiceTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCServiceTree";
            this.Size = new System.Drawing.Size(448, 422);
            this.Load += new System.EventHandler(this.UCServiceTree_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trvService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal DevExpress.XtraTreeList.TreeList trvService;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceTypeCode;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceTypeName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnHeinServiceTypeCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnHeinServiceTypeName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceUnitName;
    }
}
