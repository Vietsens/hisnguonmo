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
namespace HIS.UC.AccountBook
{
    partial class UCAccountBook
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
            this.gridControlAccountBook = new DevExpress.XtraGrid.GridControl();
            this.gridViewAccountBook = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repositoryItemCheck__Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheck__Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemRadio_Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemRadio_Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemSpinEdit_Amount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repositoryItemSpinEdit_Price = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlAccountBook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewAccountBook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit_Amount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit_Price)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlAccountBook);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(689, 453);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlAccountBook
            // 
            this.gridControlAccountBook.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gridControlAccountBook.Location = new System.Drawing.Point(2, 2);
            this.gridControlAccountBook.MainView = this.gridViewAccountBook;
            this.gridControlAccountBook.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gridControlAccountBook.Name = "gridControlAccountBook";
            this.gridControlAccountBook.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheck__Enable,
            this.repositoryItemCheck__Disable,
            this.repositoryItemRadio_Enable,
            this.repositoryItemRadio_Disable,
            this.repositoryItemSpinEdit_Amount,
            this.repositoryItemSpinEdit_Price});
            this.gridControlAccountBook.Size = new System.Drawing.Size(685, 449);
            this.gridControlAccountBook.TabIndex = 4;
            this.gridControlAccountBook.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewAccountBook});
            // 
            // gridViewAccountBook
            // 
            this.gridViewAccountBook.GridControl = this.gridControlAccountBook;
            this.gridViewAccountBook.Name = "gridViewAccountBook";
            this.gridViewAccountBook.OptionsCustomization.AllowFilter = false;
            this.gridViewAccountBook.OptionsCustomization.AllowSort = false;
            this.gridViewAccountBook.OptionsView.ShowGroupPanel = false;
            this.gridViewAccountBook.OptionsView.ShowIndicator = false;
            this.gridViewAccountBook.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridViewAccountBook_RowCellClick);
            this.gridViewAccountBook.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewAccountBook_CustomRowCellEdit);
            this.gridViewAccountBook.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridView_PopupMenuShowing);
            this.gridViewAccountBook.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewAccountBook_CustomUnboundColumnData);
            this.gridViewAccountBook.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewAccountBook_MouseDown);
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
            // repositoryItemSpinEdit_Amount
            // 
            this.repositoryItemSpinEdit_Amount.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.repositoryItemSpinEdit_Amount.AutoHeight = false;
            this.repositoryItemSpinEdit_Amount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinEdit_Amount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinEdit_Amount.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinEdit_Amount.MaxValue = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.repositoryItemSpinEdit_Amount.Name = "repositoryItemSpinEdit_Amount";
            // 
            // repositoryItemSpinEdit_Price
            // 
            this.repositoryItemSpinEdit_Price.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.repositoryItemSpinEdit_Price.AutoHeight = false;
            this.repositoryItemSpinEdit_Price.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinEdit_Price.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinEdit_Price.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinEdit_Price.MaxValue = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.repositoryItemSpinEdit_Price.Name = "repositoryItemSpinEdit_Price";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 4;
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(689, 453);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlAccountBook;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(689, 453);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // UCAccountBook
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "UCAccountBook";
            this.Size = new System.Drawing.Size(689, 453);
            this.Load += new System.EventHandler(this.UCAccountBook_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlAccountBook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewAccountBook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit_Amount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit_Price)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlAccountBook;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewAccountBook;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck__Enable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck__Disable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemRadio_Enable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemRadio_Disable;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit_Amount;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit_Price;
    }
}
