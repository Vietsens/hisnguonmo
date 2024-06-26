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
namespace HIS.Desktop.Plugins.RepartitionRatioCreate
{
    partial class frmRepartitionRatioCreate
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
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnSelectPreviousPeriod = new DevExpress.XtraEditors.SimpleButton();
            this.lblYearMonth = new DevExpress.XtraEditors.LabelControl();
            this.treeListRepartitionRatio = new DevExpress.XtraTreeList.TreeList();
            this.treeListCol_Name = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListCol_Ratio = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListCol_CreateTime = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListCol_Creator = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListCol_ModifyTime = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColModifier = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemSpinRatioEnable = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repositoryItemSpinRatioDisable = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.lblPeriod = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciPeriod = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciYearMonth = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListRepartitionRatio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinRatioEnable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinRatioDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYearMonth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnRefresh);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.btnSelectPreviousPeriod);
            this.layoutControl1.Controls.Add(this.lblYearMonth);
            this.layoutControl1.Controls.Add(this.treeListRepartitionRatio);
            this.layoutControl1.Controls.Add(this.lblPeriod);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(880, 523);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(772, 2);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(106, 22);
            this.btnRefresh.StyleController = this.layoutControl1;
            this.btnRefresh.TabIndex = 10;
            this.btnRefresh.Text = "Làm lại";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(772, 499);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSelectPreviousPeriod
            // 
            this.btnSelectPreviousPeriod.Location = new System.Drawing.Point(662, 2);
            this.btnSelectPreviousPeriod.Name = "btnSelectPreviousPeriod";
            this.btnSelectPreviousPeriod.Size = new System.Drawing.Size(106, 22);
            this.btnSelectPreviousPeriod.StyleController = this.layoutControl1;
            this.btnSelectPreviousPeriod.TabIndex = 8;
            this.btnSelectPreviousPeriod.Text = "Lấy TL kỳ trước";
            this.btnSelectPreviousPeriod.Click += new System.EventHandler(this.btnSelectPreviousPeriod_Click);
            // 
            // lblYearMonth
            // 
            this.lblYearMonth.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblYearMonth.Location = new System.Drawing.Point(537, 2);
            this.lblYearMonth.Name = "lblYearMonth";
            this.lblYearMonth.Size = new System.Drawing.Size(121, 20);
            this.lblYearMonth.StyleController = this.layoutControl1;
            this.lblYearMonth.TabIndex = 7;
            // 
            // treeListRepartitionRatio
            // 
            this.treeListRepartitionRatio.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListCol_Name,
            this.treeListCol_Ratio,
            this.treeListCol_CreateTime,
            this.treeListCol_Creator,
            this.treeListCol_ModifyTime,
            this.treeListColModifier});
            this.treeListRepartitionRatio.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListRepartitionRatio.KeyFieldName = "AdoId";
            this.treeListRepartitionRatio.Location = new System.Drawing.Point(0, 26);
            this.treeListRepartitionRatio.Name = "treeListRepartitionRatio";
            this.treeListRepartitionRatio.OptionsBehavior.AllowIndeterminateCheckState = true;
            this.treeListRepartitionRatio.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.treeListRepartitionRatio.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.treeListRepartitionRatio.OptionsBehavior.AutoPopulateColumns = false;
            this.treeListRepartitionRatio.OptionsBehavior.EnableFiltering = true;
            this.treeListRepartitionRatio.OptionsBehavior.PopulateServiceColumns = true;
            this.treeListRepartitionRatio.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Smart;
            this.treeListRepartitionRatio.OptionsView.AutoWidth = false;
            this.treeListRepartitionRatio.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFullFocus;
            this.treeListRepartitionRatio.OptionsView.ShowHorzLines = false;
            this.treeListRepartitionRatio.OptionsView.ShowIndicator = false;
            this.treeListRepartitionRatio.OptionsView.ShowVertLines = false;
            this.treeListRepartitionRatio.ParentFieldName = "AdoParentId";
            this.treeListRepartitionRatio.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSpinRatioEnable,
            this.repositoryItemSpinRatioDisable});
            this.treeListRepartitionRatio.Size = new System.Drawing.Size(880, 471);
            this.treeListRepartitionRatio.TabIndex = 6;
            this.treeListRepartitionRatio.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.treeListRepartitionRatio_CustomNodeCellEdit);
            this.treeListRepartitionRatio.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeListRepartitionRatio_NodeCellStyle);
            this.treeListRepartitionRatio.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.treeListRepartitionRatio_CellValueChanged);
            // 
            // treeListCol_Name
            // 
            this.treeListCol_Name.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListCol_Name.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListCol_Name.Caption = "Tên";
            this.treeListCol_Name.FieldName = "TypeName";
            this.treeListCol_Name.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
            this.treeListCol_Name.Name = "treeListCol_Name";
            this.treeListCol_Name.OptionsColumn.AllowEdit = false;
            this.treeListCol_Name.OptionsColumn.AllowFocus = false;
            this.treeListCol_Name.OptionsColumn.AllowSort = false;
            this.treeListCol_Name.Visible = true;
            this.treeListCol_Name.VisibleIndex = 0;
            this.treeListCol_Name.Width = 500;
            // 
            // treeListCol_Ratio
            // 
            this.treeListCol_Ratio.AppearanceCell.Options.UseTextOptions = true;
            this.treeListCol_Ratio.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListCol_Ratio.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListCol_Ratio.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListCol_Ratio.Caption = "Tỷ lệ (%)";
            this.treeListCol_Ratio.FieldName = "Ratio";
            this.treeListCol_Ratio.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
            this.treeListCol_Ratio.Format.FormatString = "#,##0.00";
            this.treeListCol_Ratio.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeListCol_Ratio.Name = "treeListCol_Ratio";
            this.treeListCol_Ratio.Visible = true;
            this.treeListCol_Ratio.VisibleIndex = 1;
            this.treeListCol_Ratio.Width = 120;
            // 
            // treeListCol_CreateTime
            // 
            this.treeListCol_CreateTime.AppearanceCell.Options.UseTextOptions = true;
            this.treeListCol_CreateTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListCol_CreateTime.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListCol_CreateTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListCol_CreateTime.Caption = "Thời gian tạo";
            this.treeListCol_CreateTime.FieldName = "CreateTime";
            this.treeListCol_CreateTime.Name = "treeListCol_CreateTime";
            this.treeListCol_CreateTime.OptionsColumn.AllowEdit = false;
            this.treeListCol_CreateTime.OptionsColumn.AllowFocus = false;
            this.treeListCol_CreateTime.OptionsColumn.AllowSort = false;
            this.treeListCol_CreateTime.Visible = true;
            this.treeListCol_CreateTime.VisibleIndex = 2;
            this.treeListCol_CreateTime.Width = 120;
            // 
            // treeListCol_Creator
            // 
            this.treeListCol_Creator.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListCol_Creator.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListCol_Creator.Caption = "Người tạo";
            this.treeListCol_Creator.FieldName = "Creator";
            this.treeListCol_Creator.Name = "treeListCol_Creator";
            this.treeListCol_Creator.OptionsColumn.AllowEdit = false;
            this.treeListCol_Creator.OptionsColumn.AllowFocus = false;
            this.treeListCol_Creator.OptionsColumn.AllowSort = false;
            this.treeListCol_Creator.Visible = true;
            this.treeListCol_Creator.VisibleIndex = 3;
            // 
            // treeListCol_ModifyTime
            // 
            this.treeListCol_ModifyTime.AppearanceCell.Options.UseTextOptions = true;
            this.treeListCol_ModifyTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListCol_ModifyTime.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListCol_ModifyTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListCol_ModifyTime.Caption = "Thời gian sửa";
            this.treeListCol_ModifyTime.FieldName = "ModifyTime";
            this.treeListCol_ModifyTime.Format.FormatString = "#,##0.00";
            this.treeListCol_ModifyTime.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeListCol_ModifyTime.Name = "treeListCol_ModifyTime";
            this.treeListCol_ModifyTime.OptionsColumn.AllowEdit = false;
            this.treeListCol_ModifyTime.OptionsColumn.AllowFocus = false;
            this.treeListCol_ModifyTime.OptionsColumn.AllowSort = false;
            this.treeListCol_ModifyTime.Visible = true;
            this.treeListCol_ModifyTime.VisibleIndex = 4;
            this.treeListCol_ModifyTime.Width = 120;
            // 
            // treeListColModifier
            // 
            this.treeListColModifier.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColModifier.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColModifier.Caption = "Người sửa";
            this.treeListColModifier.FieldName = "Modifier";
            this.treeListColModifier.Name = "treeListColModifier";
            this.treeListColModifier.OptionsColumn.AllowEdit = false;
            this.treeListColModifier.OptionsColumn.AllowFocus = false;
            this.treeListColModifier.OptionsColumn.AllowSort = false;
            this.treeListColModifier.Visible = true;
            this.treeListColModifier.VisibleIndex = 5;
            // 
            // repositoryItemSpinRatioEnable
            // 
            this.repositoryItemSpinRatioEnable.AutoHeight = false;
            this.repositoryItemSpinRatioEnable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinRatioEnable.DisplayFormat.FormatString = "#,##0.00";
            this.repositoryItemSpinRatioEnable.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinRatioEnable.EditFormat.FormatString = "#,##0.00";
            this.repositoryItemSpinRatioEnable.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinRatioEnable.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.repositoryItemSpinRatioEnable.Name = "repositoryItemSpinRatioEnable";
            // 
            // repositoryItemSpinRatioDisable
            // 
            this.repositoryItemSpinRatioDisable.AutoHeight = false;
            this.repositoryItemSpinRatioDisable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinRatioDisable.DisplayFormat.FormatString = "#,##0.00";
            this.repositoryItemSpinRatioDisable.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinRatioDisable.EditFormat.FormatString = "#,##0.00";
            this.repositoryItemSpinRatioDisable.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinRatioDisable.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.repositoryItemSpinRatioDisable.Name = "repositoryItemSpinRatioDisable";
            this.repositoryItemSpinRatioDisable.ReadOnly = true;
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
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciPeriod,
            this.layoutControlItem3,
            this.lciYearMonth,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(880, 523);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciPeriod
            // 
            this.lciPeriod.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPeriod.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPeriod.Control = this.lblPeriod;
            this.lciPeriod.Location = new System.Drawing.Point(0, 0);
            this.lciPeriod.Name = "lciPeriod";
            this.lciPeriod.Size = new System.Drawing.Size(440, 26);
            this.lciPeriod.Text = "Kỳ:";
            this.lciPeriod.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPeriod.TextSize = new System.Drawing.Size(90, 20);
            this.lciPeriod.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.treeListRepartitionRatio;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem3.Size = new System.Drawing.Size(880, 471);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // lciYearMonth
            // 
            this.lciYearMonth.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciYearMonth.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciYearMonth.Control = this.lblYearMonth;
            this.lciYearMonth.Location = new System.Drawing.Point(440, 0);
            this.lciYearMonth.Name = "lciYearMonth";
            this.lciYearMonth.Size = new System.Drawing.Size(220, 26);
            this.lciYearMonth.Text = "Tháng:";
            this.lciYearMonth.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciYearMonth.TextSize = new System.Drawing.Size(90, 20);
            this.lciYearMonth.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnSelectPreviousPeriod;
            this.layoutControlItem1.Location = new System.Drawing.Point(660, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnSave;
            this.layoutControlItem2.Location = new System.Drawing.Point(770, 497);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnRefresh;
            this.layoutControlItem4.Location = new System.Drawing.Point(770, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 497);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(770, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmRepartitionRatioCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 523);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmRepartitionRatioCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tạo tỷ lệ phân phối của kỳ";
            this.Load += new System.EventHandler(this.frmRepartitionRatioCreate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListRepartitionRatio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinRatioEnable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinRatioDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYearMonth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblPeriod;
        private DevExpress.XtraLayout.LayoutControlItem lciPeriod;
        private DevExpress.XtraTreeList.TreeList treeListRepartitionRatio;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.LabelControl lblYearMonth;
        private DevExpress.XtraLayout.LayoutControlItem lciYearMonth;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnSelectPreviousPeriod;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListCol_Name;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListCol_Ratio;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListCol_CreateTime;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListCol_Creator;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListCol_ModifyTime;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColModifier;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinRatioEnable;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinRatioDisable;
    }
}
