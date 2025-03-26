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
namespace Inventec.UC.UpLoadFile.Design.Template1
{
    partial class Template1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Template1));
            this.gridControlUpLoadFile = new DevExpress.XtraGrid.GridControl();
            this.gridViewUpLoadFile = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemProgressBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
            this.btnChoiseFolder = new DevExpress.XtraEditors.SimpleButton();
            this.btnChoiseFile = new DevExpress.XtraEditors.SimpleButton();
            this.timer1 = new System.Windows.Forms.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlUpLoadFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewUpLoadFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControlUpLoadFile
            // 
            this.gridControlUpLoadFile.Location = new System.Drawing.Point(0, 0);
            this.gridControlUpLoadFile.MainView = this.gridViewUpLoadFile;
            this.gridControlUpLoadFile.Name = "gridControlUpLoadFile";
            this.gridControlUpLoadFile.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemProgressBar1});
            this.gridControlUpLoadFile.Size = new System.Drawing.Size(660, 355);
            this.gridControlUpLoadFile.TabIndex = 0;
            this.gridControlUpLoadFile.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewUpLoadFile});
            // 
            // gridViewUpLoadFile
            // 
            this.gridViewUpLoadFile.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4});
            this.gridViewUpLoadFile.GridControl = this.gridControlUpLoadFile;
            this.gridViewUpLoadFile.Name = "gridViewUpLoadFile";
            this.gridViewUpLoadFile.OptionsFind.AlwaysVisible = true;
            this.gridViewUpLoadFile.OptionsFind.ShowClearButton = false;
            this.gridViewUpLoadFile.OptionsFind.ShowCloseButton = false;
            this.gridViewUpLoadFile.OptionsFind.ShowFindButton = false;
            this.gridViewUpLoadFile.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewUpLoadFile.OptionsView.ShowIndicator = false;
            this.gridViewUpLoadFile.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewUpLoadFile_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "STT";
            this.gridColumn1.FieldName = "STT";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 50;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Tên file";
            this.gridColumn2.FieldName = "FILE_NAME";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 345;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Dung lượng";
            this.gridColumn3.FieldName = "FILE_LENGTH";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 114;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Trạng thái tải";
            this.gridColumn4.ColumnEdit = this.repositoryItemProgressBar1;
            this.gridColumn4.FieldName = "FILE_STATUS";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 149;
            // 
            // repositoryItemProgressBar1
            // 
            this.repositoryItemProgressBar1.Name = "repositoryItemProgressBar1";
            this.repositoryItemProgressBar1.ShowTitle = true;
            // 
            // btnChoiseFolder
            // 
            this.btnChoiseFolder.Image = ((System.Drawing.Image)(resources.GetObject("btnChoiseFolder.Image")));
            this.btnChoiseFolder.Location = new System.Drawing.Point(375, 360);
            this.btnChoiseFolder.Name = "btnChoiseFolder";
            this.btnChoiseFolder.Size = new System.Drawing.Size(140, 40);
            this.btnChoiseFolder.TabIndex = 1;
            this.btnChoiseFolder.Text = "Chọn folder (Ctrl O)";
            this.btnChoiseFolder.Click += new System.EventHandler(this.btnChoiseFolder_Click);
            // 
            // btnChoiseFile
            // 
            this.btnChoiseFile.Image = ((System.Drawing.Image)(resources.GetObject("btnChoiseFile.Image")));
            this.btnChoiseFile.Location = new System.Drawing.Point(520, 360);
            this.btnChoiseFile.Name = "btnChoiseFile";
            this.btnChoiseFile.Size = new System.Drawing.Size(140, 40);
            this.btnChoiseFile.TabIndex = 2;
            this.btnChoiseFile.Text = "Chọn file (Ctrl F)";
            this.btnChoiseFile.Click += new System.EventHandler(this.btnChoiseFile_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Template1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnChoiseFile);
            this.Controls.Add(this.btnChoiseFolder);
            this.Controls.Add(this.gridControlUpLoadFile);
            this.Name = "Template1";
            this.Size = new System.Drawing.Size(660, 400);
            this.Load += new System.EventHandler(this.Template1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlUpLoadFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewUpLoadFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControlUpLoadFile;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewUpLoadFile;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraEditors.SimpleButton btnChoiseFolder;
        private DevExpress.XtraEditors.SimpleButton btnChoiseFile;
        private DevExpress.XtraEditors.Repository.RepositoryItemProgressBar repositoryItemProgressBar1;
        private System.Windows.Forms.Timer timer1;
    }
}
