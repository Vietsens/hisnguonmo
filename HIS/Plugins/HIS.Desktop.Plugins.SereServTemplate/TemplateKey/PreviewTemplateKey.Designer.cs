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
namespace HIS.Desktop.Plugins.SereServTemplate.TemplateKey
{
    partial class PreviewTemplateKey
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
            this.GridControlKey = new DevExpress.XtraGrid.GridControl();
            this.GridViewKey = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gc_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Key = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Value = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtSearch = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridControlKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.GridControlKey);
            this.layoutControl1.Controls.Add(this.txtSearch);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1173, 567);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // GridControlKey
            // 
            this.GridControlKey.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GridControlKey.Location = new System.Drawing.Point(3, 31);
            this.GridControlKey.MainView = this.GridViewKey;
            this.GridControlKey.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GridControlKey.Name = "GridControlKey";
            this.GridControlKey.Size = new System.Drawing.Size(1167, 533);
            this.GridControlKey.TabIndex = 5;
            this.GridControlKey.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.GridViewKey});
            // 
            // GridViewKey
            // 
            this.GridViewKey.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gc_Stt,
            this.Gc_Key,
            this.Gc_Value});
            this.GridViewKey.GridControl = this.GridControlKey;
            this.GridViewKey.Name = "GridViewKey";
            this.GridViewKey.OptionsView.ShowGroupPanel = false;
            this.GridViewKey.OptionsView.ShowIndicator = false;
            this.GridViewKey.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.GridViewKey_CustomUnboundColumnData);
            // 
            // Gc_Stt
            // 
            this.Gc_Stt.Caption = "STT";
            this.Gc_Stt.FieldName = "STT";
            this.Gc_Stt.Name = "Gc_Stt";
            this.Gc_Stt.OptionsColumn.AllowEdit = false;
            this.Gc_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Stt.Visible = true;
            this.Gc_Stt.VisibleIndex = 0;
            // 
            // Gc_Key
            // 
            this.Gc_Key.Caption = "Key";
            this.Gc_Key.FieldName = "KEY";
            this.Gc_Key.Name = "Gc_Key";
            this.Gc_Key.Visible = true;
            this.Gc_Key.VisibleIndex = 1;
            this.Gc_Key.Width = 460;
            // 
            // Gc_Value
            // 
            this.Gc_Value.Caption = "Giá trị";
            this.Gc_Value.FieldName = "VALUE";
            this.Gc_Value.Name = "Gc_Value";
            this.Gc_Value.OptionsColumn.AllowEdit = false;
            this.Gc_Value.Visible = true;
            this.Gc_Value.VisibleIndex = 2;
            this.Gc_Value.Width = 559;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(3, 3);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearch.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearch.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtSearch.Size = new System.Drawing.Size(1167, 22);
            this.txtSearch.StyleController = this.layoutControl1;
            this.txtSearch.TabIndex = 4;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1173, 567);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.txtSearch;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1173, 28);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.GridControlKey;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 28);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(1173, 539);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // PreviewTemplateKey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1173, 567);
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "PreviewTemplateKey";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PreviewTemplateKey";
            this.Load += new System.EventHandler(this.PreviewTemplateKey_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridControlKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtSearch;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.GridControl GridControlKey;
        private DevExpress.XtraGrid.Views.Grid.GridView GridViewKey;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Key;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Value;
    }
}
