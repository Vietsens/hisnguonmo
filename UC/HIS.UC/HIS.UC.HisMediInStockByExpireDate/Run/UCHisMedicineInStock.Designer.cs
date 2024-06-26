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
namespace HIS.UC.HisMediInStockByExpireDate.Run
{
    partial class UCHisMediInStockByExpireDate
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
            this.repositoryItemchkIsExpend__Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemchkIsExpend__Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheckEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheckEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKeyword = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHisMediInStockByExpireDateAdd = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.trvService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHisMediInStockByExpireDateAdd)).BeginInit();
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
            this.trvService.Cursor = System.Windows.Forms.Cursors.Default;
            this.trvService.KeyFieldName = "CONCRETE_ID__IN_DATE";
            this.trvService.Location = new System.Drawing.Point(2, 28);
            this.trvService.Name = "trvService";
            this.trvService.OptionsBehavior.AllowIndeterminateCheckState = true;
            this.trvService.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.trvService.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.trvService.OptionsBehavior.EnableFiltering = true;
            this.trvService.OptionsBehavior.PopulateServiceColumns = true;
            this.trvService.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Smart;
            this.trvService.OptionsFind.FindDelay = 100;
            this.trvService.OptionsFind.FindNullPrompt = "Nhập chuỗi tìm kiếm ...";
            this.trvService.OptionsFind.ShowClearButton = false;
            this.trvService.OptionsFind.ShowFindButton = false;
            this.trvService.OptionsSelection.InvertSelection = true;
            this.trvService.OptionsView.AutoWidth = false;
            this.trvService.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            this.trvService.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFullFocus;
            this.trvService.OptionsView.ShowCheckBoxes = true;
            this.trvService.OptionsView.ShowHorzLines = false;
            this.trvService.OptionsView.ShowIndicator = false;
            this.trvService.OptionsView.ShowVertLines = false;
            this.trvService.ParentFieldName = "PARENT_ID__IN_DATE";
            this.trvService.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemchkIsExpend__Enable,
            this.repositoryItemchkIsExpend__Disable,
            this.repositoryItemCheckEdit1,
            this.repositoryItemCheckEdit2,
            this.repositoryItemCheckEdit3,
            this.repositoryItemCheckEdit4});
            this.trvService.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
            this.trvService.Size = new System.Drawing.Size(1312, 486);
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
            this.trvService.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.trvService_CustomDrawNodeCell);
            this.trvService.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.trvService_PopupMenuShowing);
            this.trvService.Click += new System.EventHandler(this.trvService_Click);
            this.trvService.DoubleClick += new System.EventHandler(this.trvService_DoubleClick);
            this.trvService.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trvService_KeyDown);
            // 
            // repositoryItemchkIsExpend__Enable
            // 
            this.repositoryItemchkIsExpend__Enable.AutoHeight = false;
            this.repositoryItemchkIsExpend__Enable.Name = "repositoryItemchkIsExpend__Enable";
            this.repositoryItemchkIsExpend__Enable.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemchkIsExpend__Enable.CheckedChanged += new System.EventHandler(this.repositoryItemchkIsExpend__Enable_CheckedChanged);
            // 
            // repositoryItemchkIsExpend__Disable
            // 
            this.repositoryItemchkIsExpend__Disable.AutoHeight = false;
            this.repositoryItemchkIsExpend__Disable.Name = "repositoryItemchkIsExpend__Disable";
            this.repositoryItemchkIsExpend__Disable.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemchkIsExpend__Disable.ReadOnly = true;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // repositoryItemCheckEdit2
            // 
            this.repositoryItemCheckEdit2.AutoHeight = false;
            this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            // 
            // repositoryItemCheckEdit3
            // 
            this.repositoryItemCheckEdit3.AutoHeight = false;
            this.repositoryItemCheckEdit3.Name = "repositoryItemCheckEdit3";
            // 
            // repositoryItemCheckEdit4
            // 
            this.repositoryItemCheckEdit4.AutoHeight = false;
            this.repositoryItemCheckEdit4.Name = "repositoryItemCheckEdit4";
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnNew);
            this.layoutControl1.Controls.Add(this.trvService);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1316, 516);
            this.layoutControl1.TabIndex = 31;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(1203, 2);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(111, 22);
            this.btnNew.StyleController = this.layoutControl1;
            this.btnNew.TabIndex = 8;
            this.btnNew.Text = "Mới (Ctrl N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(1197, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 1;
            this.txtKeyword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyUp);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKeyword,
            this.layoutControlItem4,
            this.lciHisMediInStockByExpireDateAdd});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1316, 516);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciKeyword
            // 
            this.lciKeyword.Control = this.txtKeyword;
            this.lciKeyword.Location = new System.Drawing.Point(0, 0);
            this.lciKeyword.Name = "lciKeyword";
            this.lciKeyword.Size = new System.Drawing.Size(1201, 26);
            this.lciKeyword.TextSize = new System.Drawing.Size(0, 0);
            this.lciKeyword.TextVisible = false;
            this.lciKeyword.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.trvService;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(1316, 490);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // lciHisMediInStockByExpireDateAdd
            // 
            this.lciHisMediInStockByExpireDateAdd.Control = this.btnNew;
            this.lciHisMediInStockByExpireDateAdd.Location = new System.Drawing.Point(1201, 0);
            this.lciHisMediInStockByExpireDateAdd.Name = "lciHisMediInStockByExpireDateAdd";
            this.lciHisMediInStockByExpireDateAdd.Size = new System.Drawing.Size(115, 26);
            this.lciHisMediInStockByExpireDateAdd.TextSize = new System.Drawing.Size(0, 0);
            this.lciHisMediInStockByExpireDateAdd.TextVisible = false;
            this.lciHisMediInStockByExpireDateAdd.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // UCHisMediInStockByExpireDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCHisMediInStockByExpireDate";
            this.Size = new System.Drawing.Size(1316, 516);
            this.Load += new System.EventHandler(this.UCServiceTree_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trvService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHisMediInStockByExpireDateAdd)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal DevExpress.XtraTreeList.TreeList trvService;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemchkIsExpend__Enable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemchkIsExpend__Disable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit3;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit4;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraLayout.LayoutControlItem lciHisMediInStockByExpireDateAdd;
    }
}
