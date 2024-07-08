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
namespace HIS.UC.FormType.Core.SQLInput_F34__
{
    partial class UCSql
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
            this.TxtValue = new DevExpress.XtraEditors.MemoEdit();
            this.TxtName = new DevExpress.XtraEditors.TextEdit();
            this.gridControl1 = new Inventec.Desktop.CustomControl.MyGridControl();
            this.gridView1 = new Inventec.Desktop.CustomControl.MyGridView();
            this.Gc_Name = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.LciUcSql = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciName = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciValue = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TxtValue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciUcSql)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciValue)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.TxtValue);
            this.layoutControl1.Controls.Add(this.TxtName);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 204);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // TxtValue
            // 
            this.TxtValue.Location = new System.Drawing.Point(427, 26);
            this.TxtValue.Name = "TxtValue";
            this.TxtValue.Size = new System.Drawing.Size(231, 176);
            this.TxtValue.StyleController = this.layoutControl1;
            this.TxtValue.TabIndex = 7;
            // 
            // TxtName
            // 
            this.TxtName.Location = new System.Drawing.Point(427, 2);
            this.TxtName.Name = "TxtName";
            this.TxtName.Properties.ReadOnly = true;
            this.TxtName.Size = new System.Drawing.Size(231, 20);
            this.TxtName.StyleController = this.layoutControl1;
            this.TxtName.TabIndex = 5;
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(97, 2);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(231, 200);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gc_Name,
            this.colNAMEUnb});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.CustomRowColumnError += new System.EventHandler<Inventec.Desktop.CustomControl.RowColumnErrorEventArgs>(this.gridView1_CustomRowColumnError);
            this.gridView1.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridView1_RowCellClick);
            // 
            // Gc_Name
            // 
            this.Gc_Name.Caption = "Tên";
            this.Gc_Name.FieldName = "NAME";
            this.Gc_Name.FieldNameSortGroup = "NAMEUnb";
            this.Gc_Name.Name = "Gc_Name";
            this.Gc_Name.OptionsColumn.AllowEdit = false;
            this.Gc_Name.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.Gc_Name.Visible = true;
            this.Gc_Name.VisibleIndex = 0;
            // 
            // colNAMEUnb
            // 
            this.colNAMEUnb.FieldName = "NAMEUnb";
            this.colNAMEUnb.Name = "colNAMEUnb";
            this.colNAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.LciUcSql,
            this.LciName,
            this.LciValue});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 204);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // LciUcSql
            // 
            this.LciUcSql.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciUcSql.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciUcSql.Control = this.gridControl1;
            this.LciUcSql.Location = new System.Drawing.Point(0, 0);
            this.LciUcSql.Name = "LciUcSql";
            this.LciUcSql.Size = new System.Drawing.Size(330, 204);
            this.LciUcSql.Text = " ";
            this.LciUcSql.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciUcSql.TextSize = new System.Drawing.Size(90, 20);
            this.LciUcSql.TextToControlDistance = 5;
            // 
            // LciName
            // 
            this.LciName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciName.Control = this.TxtName;
            this.LciName.Location = new System.Drawing.Point(330, 0);
            this.LciName.Name = "LciName";
            this.LciName.Size = new System.Drawing.Size(330, 24);
            this.LciName.Text = "Tên:";
            this.LciName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciName.TextSize = new System.Drawing.Size(90, 20);
            this.LciName.TextToControlDistance = 5;
            // 
            // LciValue
            // 
            this.LciValue.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciValue.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciValue.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.LciValue.Control = this.TxtValue;
            this.LciValue.Location = new System.Drawing.Point(330, 24);
            this.LciValue.Name = "LciValue";
            this.LciValue.Size = new System.Drawing.Size(330, 180);
            this.LciValue.Text = "Giá trị:";
            this.LciValue.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciValue.TextSize = new System.Drawing.Size(90, 20);
            this.LciValue.TextToControlDistance = 5;
            // 
            // UCSql
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCSql";
            this.Size = new System.Drawing.Size(660, 204);
            this.Load += new System.EventHandler(this.UCSql_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TxtValue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciUcSql)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.MemoEdit TxtValue;
        private DevExpress.XtraEditors.TextEdit TxtName;
        private Inventec.Desktop.CustomControl.MyGridControl gridControl1;
        private Inventec.Desktop.CustomControl.MyGridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem LciUcSql;
        private DevExpress.XtraLayout.LayoutControlItem LciName;
        private DevExpress.XtraLayout.LayoutControlItem LciValue;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Name;
        private DevExpress.XtraGrid.Columns.GridColumn colNAMEUnb;

    }
}
