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
namespace HIS.UC.ServiceReqDetail
{
    partial class UCServiceReqDetail
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.lblExpMestCode = new DevExpress.XtraEditors.LabelControl();
            this.lblMediStockName = new DevExpress.XtraEditors.LabelControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciExpMestCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMediStockName = new DevExpress.XtraLayout.LayoutControlItem();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repositoryItemCheck__Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheck__Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemRadio_Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemRadio_Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpMestCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediStockName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Controls.Add(this.gridControl);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1000, 422);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.lblExpMestCode);
            this.layoutControl2.Controls.Add(this.lblMediStockName);
            this.layoutControl2.Location = new System.Drawing.Point(2, 2);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(996, 24);
            this.layoutControl2.TabIndex = 15;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // lblExpMestCode
            // 
            this.lblExpMestCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblExpMestCode.Location = new System.Drawing.Point(97, 2);
            this.lblExpMestCode.Name = "lblExpMestCode";
            this.lblExpMestCode.Size = new System.Drawing.Size(92, 20);
            this.lblExpMestCode.StyleController = this.layoutControl2;
            this.lblExpMestCode.TabIndex = 5;
            // 
            // lblMediStockName
            // 
            this.lblMediStockName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMediStockName.Location = new System.Drawing.Point(288, 2);
            this.lblMediStockName.Name = "lblMediStockName";
            this.lblMediStockName.Size = new System.Drawing.Size(706, 20);
            this.lblMediStockName.StyleController = this.layoutControl2;
            this.lblMediStockName.TabIndex = 7;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciExpMestCode,
            this.lciMediStockName});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(996, 24);
            this.Root.TextVisible = false;
            // 
            // lciExpMestCode
            // 
            this.lciExpMestCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciExpMestCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciExpMestCode.Control = this.lblExpMestCode;
            this.lciExpMestCode.Location = new System.Drawing.Point(0, 0);
            this.lciExpMestCode.Name = "lciExpMestCode";
            this.lciExpMestCode.Size = new System.Drawing.Size(191, 24);
            this.lciExpMestCode.Text = "Mã xuất:";
            this.lciExpMestCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciExpMestCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciExpMestCode.TextToControlDistance = 5;
            // 
            // lciMediStockName
            // 
            this.lciMediStockName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMediStockName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMediStockName.Control = this.lblMediStockName;
            this.lciMediStockName.Location = new System.Drawing.Point(191, 0);
            this.lciMediStockName.Name = "lciMediStockName";
            this.lciMediStockName.Size = new System.Drawing.Size(805, 24);
            this.lciMediStockName.Text = "Kho xuất:";
            this.lciMediStockName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMediStockName.TextSize = new System.Drawing.Size(90, 20);
            this.lciMediStockName.TextToControlDistance = 5;
            // 
            // gridControl
            // 
            this.gridControl.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl.Location = new System.Drawing.Point(0, 28);
            this.gridControl.MainView = this.gridView;
            this.gridControl.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl.Name = "gridControl";
            this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheck__Enable,
            this.repositoryItemCheck__Disable,
            this.repositoryItemRadio_Enable,
            this.repositoryItemRadio_Disable});
            this.gridControl.Size = new System.Drawing.Size(1000, 394);
            this.gridControl.TabIndex = 4;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            // 
            // gridView
            // 
            this.gridView.GridControl = this.gridControl;
            this.gridView.Name = "gridView";
            this.gridView.OptionsCustomization.AllowFilter = false;
            this.gridView.OptionsCustomization.AllowSort = false;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.OptionsView.ShowIndicator = false;
            this.gridView.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridView_RowCellStyle);
            this.gridView.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewCashCollect_CustomRowCellEdit);
            this.gridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridViewAccount_PopupMenuShowing);
            this.gridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewAccount_CustomUnboundColumnData);
            this.gridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewAccount_MouseDown);
            // 
            // repositoryItemCheck__Enable
            // 
            this.repositoryItemCheck__Enable.AutoHeight = false;
            this.repositoryItemCheck__Enable.Name = "repositoryItemCheck__Enable";
            this.repositoryItemCheck__Enable.CheckedChanged += new System.EventHandler(this.repositoryItemCheck__Enable_CheckedChanged);
            // 
            // repositoryItemCheck__Disable
            // 
            this.repositoryItemCheck__Disable.AutoHeight = false;
            this.repositoryItemCheck__Disable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style2;
            this.repositoryItemCheck__Disable.Name = "repositoryItemCheck__Disable";
            this.repositoryItemCheck__Disable.ReadOnly = true;
            // 
            // repositoryItemRadio_Enable
            // 
            this.repositoryItemRadio_Enable.AutoHeight = false;
            this.repositoryItemRadio_Enable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.repositoryItemRadio_Enable.Name = "repositoryItemRadio_Enable";
            this.repositoryItemRadio_Enable.Click += new System.EventHandler(this.repositoryItemRadio_Enable_Click);
            // 
            // repositoryItemRadio_Disable
            // 
            this.repositoryItemRadio_Disable.AutoHeight = false;
            this.repositoryItemRadio_Disable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.repositoryItemRadio_Disable.Name = "repositoryItemRadio_Disable";
            this.repositoryItemRadio_Disable.ReadOnly = true;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem12});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 4;
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1000, 422);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControl;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 28);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Size = new System.Drawing.Size(1000, 394);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.layoutControl2;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(1000, 28);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // UCServiceReqDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UCServiceReqDetail";
            this.Size = new System.Drawing.Size(1000, 422);
            this.Load += new System.EventHandler(this.UCAccount_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpMestCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediStockName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck__Enable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck__Disable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemRadio_Enable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemRadio_Disable;
        private DevExpress.XtraEditors.LabelControl lblExpMestCode;
        private DevExpress.XtraEditors.LabelControl lblMediStockName;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lciExpMestCode;
        private DevExpress.XtraLayout.LayoutControlItem lciMediStockName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
    }
}
