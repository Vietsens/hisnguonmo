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
namespace EMR.Desktop.Plugins.EmrSignDocumentList
{
    partial class frmSignErrrorDetail
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
            this.txtErrorList = new DevExpress.XtraEditors.MemoEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtErrorList.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtErrorList
            // 
            this.txtErrorList.AllowHtmlTextInToolTip = DevExpress.Utils.DefaultBoolean.True;
            this.txtErrorList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtErrorList.Location = new System.Drawing.Point(0, 0);
            this.txtErrorList.Name = "txtErrorList";
            this.txtErrorList.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
            this.txtErrorList.Properties.ReadOnly = true;
            this.txtErrorList.Size = new System.Drawing.Size(776, 253);
            this.txtErrorList.TabIndex = 0;
            // 
            // frmSignErrrorDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 253);
            this.Controls.Add(this.txtErrorList);
            this.Name = "frmSignErrrorDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách lỗi khi ký";
            this.Load += new System.EventHandler(this.frmSignErrrorDetail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtErrorList.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.MemoEdit txtErrorList;

    }
}
