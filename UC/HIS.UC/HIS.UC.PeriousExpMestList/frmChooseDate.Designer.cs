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
namespace HIS.UC.PeriousExpMestList
{
    partial class frmChooseDate
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
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.dateTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.lciFordateTimeFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.dateTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.lciFordateTimeTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFordateTimeFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFordateTimeTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnChoose);
            this.layoutControl1.Controls.Add(this.dateTimeTo);
            this.layoutControl1.Controls.Add(this.dateTimeFrom);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(320, 99);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciFordateTimeFrom,
            this.lciFordateTimeTo,
            this.layoutControlItem3,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(320, 99);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // dateTimeFrom
            // 
            this.dateTimeFrom.EditValue = null;
            this.dateTimeFrom.Location = new System.Drawing.Point(107, 12);
            this.dateTimeFrom.Name = "dateTimeFrom";
            this.dateTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateTimeFrom.Size = new System.Drawing.Size(201, 20);
            this.dateTimeFrom.StyleController = this.layoutControl1;
            this.dateTimeFrom.TabIndex = 4;
            // 
            // lciFordateTimeFrom
            // 
            this.lciFordateTimeFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciFordateTimeFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciFordateTimeFrom.Control = this.dateTimeFrom;
            this.lciFordateTimeFrom.Location = new System.Drawing.Point(0, 0);
            this.lciFordateTimeFrom.Name = "lciFordateTimeFrom";
            this.lciFordateTimeFrom.Size = new System.Drawing.Size(300, 24);
            this.lciFordateTimeFrom.Text = "Từ:";
            this.lciFordateTimeFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciFordateTimeFrom.TextSize = new System.Drawing.Size(90, 20);
            this.lciFordateTimeFrom.TextToControlDistance = 5;
            // 
            // dateTimeTo
            // 
            this.dateTimeTo.EditValue = null;
            this.dateTimeTo.Location = new System.Drawing.Point(107, 36);
            this.dateTimeTo.Name = "dateTimeTo";
            this.dateTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateTimeTo.Size = new System.Drawing.Size(201, 20);
            this.dateTimeTo.StyleController = this.layoutControl1;
            this.dateTimeTo.TabIndex = 5;
            // 
            // lciFordateTimeTo
            // 
            this.lciFordateTimeTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciFordateTimeTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciFordateTimeTo.Control = this.dateTimeTo;
            this.lciFordateTimeTo.Location = new System.Drawing.Point(0, 24);
            this.lciFordateTimeTo.Name = "lciFordateTimeTo";
            this.lciFordateTimeTo.Size = new System.Drawing.Size(300, 24);
            this.lciFordateTimeTo.Text = "Đến:";
            this.lciFordateTimeTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciFordateTimeTo.TextSize = new System.Drawing.Size(90, 20);
            this.lciFordateTimeTo.TextToControlDistance = 5;
            // 
            // btnChoose
            // 
            this.btnChoose.Location = new System.Drawing.Point(211, 60);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(97, 22);
            this.btnChoose.StyleController = this.layoutControl1;
            this.btnChoose.TabIndex = 6;
            this.btnChoose.Text = "Chọn";
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnChoose;
            this.layoutControlItem3.Location = new System.Drawing.Point(199, 48);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(101, 31);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 48);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(199, 31);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmChooseDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 99);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmChooseDate";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tùy chọn thời gian";
            this.Load += new System.EventHandler(this.frmChooseDate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFordateTimeFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFordateTimeTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnChoose;
        private DevExpress.XtraEditors.DateEdit dateTimeTo;
        private DevExpress.XtraEditors.DateEdit dateTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciFordateTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciFordateTimeTo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
