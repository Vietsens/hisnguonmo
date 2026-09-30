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
namespace HIS.Desktop.Plugins.GenerateRegisterOrder.Popup
{
    partial class frmChooseIdentity
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.pnlChoose = new DevExpress.XtraEditors.PanelControl();
            this.tlpOptions = new System.Windows.Forms.TableLayoutPanel();
            this.btnCccd = new DevExpress.XtraEditors.SimpleButton();
            this.btnVneId = new DevExpress.XtraEditors.SimpleButton();
            this.btnCccdManual = new DevExpress.XtraEditors.SimpleButton();
            this.btnBhyt = new DevExpress.XtraEditors.SimpleButton();
            this.btnNoPaper = new DevExpress.XtraEditors.SimpleButton();
            this.pnlInput = new DevExpress.XtraEditors.PanelControl();
            this.tlpInput = new System.Windows.Forms.TableLayoutPanel();
            this.lblGuide = new DevExpress.XtraEditors.LabelControl();
            this.txtInput = new DevExpress.XtraEditors.TextEdit();
            this.lblResult = new DevExpress.XtraEditors.LabelControl();
            this.tlpKeypad = new System.Windows.Forms.TableLayoutPanel();
            this.pnlInputAction = new System.Windows.Forms.TableLayoutPanel();
            this.btnBack = new DevExpress.XtraEditors.SimpleButton();
            this.btnConfirm = new DevExpress.XtraEditors.SimpleButton();
            this.tmrIdle = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pnlChoose)).BeginInit();
            this.pnlChoose.SuspendLayout();
            this.tlpOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlInput)).BeginInit();
            this.pnlInput.SuspendLayout();
            this.tlpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtInput.Properties)).BeginInit();
            this.pnlInputAction.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Appearance.Options.UseBackColor = true;
            this.lblTitle.Appearance.Options.UseFont = true;
            this.lblTitle.Appearance.Options.UseForeColor = true;
            this.lblTitle.Appearance.Options.UseTextOptions = true;
            this.lblTitle.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblTitle.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1000, 90);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "CHỌN HÌNH THỨC LẤY SỐ";
            //
            // pnlChoose
            //
            this.pnlChoose.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlChoose.Controls.Add(this.tlpOptions);
            this.pnlChoose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChoose.Location = new System.Drawing.Point(0, 90);
            this.pnlChoose.Name = "pnlChoose";
            this.pnlChoose.Size = new System.Drawing.Size(1000, 610);
            this.pnlChoose.TabIndex = 1;
            //
            // tlpOptions
            //
            this.tlpOptions.ColumnCount = 3;
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tlpOptions.Controls.Add(this.btnCccd, 0, 0);
            this.tlpOptions.Controls.Add(this.btnVneId, 1, 0);
            this.tlpOptions.Controls.Add(this.btnCccdManual, 2, 0);
            this.tlpOptions.Controls.Add(this.btnBhyt, 0, 1);
            this.tlpOptions.Controls.Add(this.btnNoPaper, 1, 1);
            this.tlpOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpOptions.Location = new System.Drawing.Point(0, 0);
            this.tlpOptions.Name = "tlpOptions";
            this.tlpOptions.Padding = new System.Windows.Forms.Padding(20);
            this.tlpOptions.RowCount = 2;
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.Size = new System.Drawing.Size(1000, 610);
            this.tlpOptions.TabIndex = 0;
            //
            // btnCccd
            //
            this.btnCccd.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.btnCccd.Appearance.Options.UseFont = true;
            this.btnCccd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCccd.Location = new System.Drawing.Point(28, 28);
            this.btnCccd.Margin = new System.Windows.Forms.Padding(8);
            this.btnCccd.Name = "btnCccd";
            this.btnCccd.Size = new System.Drawing.Size(304, 269);
            this.btnCccd.TabIndex = 0;
            this.btnCccd.Text = "QUÉT QR\r\nTHẺ CCCD";
            //
            // btnVneId
            //
            this.btnVneId.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.btnVneId.Appearance.Options.UseFont = true;
            this.btnVneId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnVneId.Location = new System.Drawing.Point(348, 28);
            this.btnVneId.Margin = new System.Windows.Forms.Padding(8);
            this.btnVneId.Name = "btnVneId";
            this.btnVneId.Size = new System.Drawing.Size(304, 269);
            this.btnVneId.TabIndex = 1;
            this.btnVneId.Text = "QUÉT QR\r\nVNeID";
            //
            // btnCccdManual
            //
            this.btnCccdManual.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.btnCccdManual.Appearance.Options.UseFont = true;
            this.btnCccdManual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCccdManual.Location = new System.Drawing.Point(668, 28);
            this.btnCccdManual.Margin = new System.Windows.Forms.Padding(8);
            this.btnCccdManual.Name = "btnCccdManual";
            this.btnCccdManual.Size = new System.Drawing.Size(304, 269);
            this.btnCccdManual.TabIndex = 2;
            this.btnCccdManual.Text = "NHẬP TAY\r\nSỐ CCCD";
            //
            // btnBhyt
            //
            this.btnBhyt.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.btnBhyt.Appearance.Options.UseFont = true;
            this.btnBhyt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBhyt.Location = new System.Drawing.Point(28, 313);
            this.btnBhyt.Margin = new System.Windows.Forms.Padding(8);
            this.btnBhyt.Name = "btnBhyt";
            this.btnBhyt.Size = new System.Drawing.Size(304, 269);
            this.btnBhyt.TabIndex = 3;
            this.btnBhyt.Text = "THẺ BHYT";
            //
            // btnNoPaper
            //
            this.btnNoPaper.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.btnNoPaper.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnNoPaper.Appearance.Options.UseFont = true;
            this.btnNoPaper.Appearance.Options.UseForeColor = true;
            this.btnNoPaper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNoPaper.Location = new System.Drawing.Point(348, 313);
            this.btnNoPaper.Margin = new System.Windows.Forms.Padding(8);
            this.btnNoPaper.Name = "btnNoPaper";
            this.btnNoPaper.Size = new System.Drawing.Size(304, 269);
            this.btnNoPaper.TabIndex = 4;
            this.btnNoPaper.Text = "KHÔNG CÓ\r\nGIẤY TỜ";
            //
            // pnlInput
            //
            this.pnlInput.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlInput.Controls.Add(this.tlpInput);
            this.pnlInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInput.Location = new System.Drawing.Point(0, 90);
            this.pnlInput.Name = "pnlInput";
            this.pnlInput.Size = new System.Drawing.Size(1000, 610);
            this.pnlInput.TabIndex = 2;
            this.pnlInput.Visible = false;
            //
            // tlpInput
            //
            this.tlpInput.ColumnCount = 1;
            this.tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInput.Controls.Add(this.lblGuide, 0, 0);
            this.tlpInput.Controls.Add(this.txtInput, 0, 1);
            this.tlpInput.Controls.Add(this.lblResult, 0, 2);
            this.tlpInput.Controls.Add(this.tlpKeypad, 0, 3);
            this.tlpInput.Controls.Add(this.pnlInputAction, 0, 4);
            this.tlpInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpInput.Location = new System.Drawing.Point(0, 0);
            this.tlpInput.Name = "tlpInput";
            this.tlpInput.Padding = new System.Windows.Forms.Padding(20);
            this.tlpInput.RowCount = 5;
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpInput.Size = new System.Drawing.Size(1000, 610);
            this.tlpInput.TabIndex = 0;
            //
            // lblGuide
            //
            this.lblGuide.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblGuide.Appearance.Options.UseFont = true;
            this.lblGuide.Appearance.Options.UseTextOptions = true;
            this.lblGuide.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblGuide.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblGuide.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblGuide.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGuide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGuide.Location = new System.Drawing.Point(23, 23);
            this.lblGuide.Name = "lblGuide";
            this.lblGuide.Size = new System.Drawing.Size(954, 74);
            this.lblGuide.TabIndex = 0;
            this.lblGuide.Text = "";
            //
            // txtInput
            //
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInput.Location = new System.Drawing.Point(23, 103);
            this.txtInput.Name = "txtInput";
            this.txtInput.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold);
            this.txtInput.Properties.Appearance.Options.UseFont = true;
            this.txtInput.Properties.Appearance.Options.UseTextOptions = true;
            this.txtInput.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtInput.Properties.MaxLength = 500;
            this.txtInput.Size = new System.Drawing.Size(954, 44);
            this.txtInput.TabIndex = 1;
            //
            // lblResult
            //
            this.lblResult.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblResult.Appearance.Options.UseFont = true;
            this.lblResult.Appearance.Options.UseTextOptions = true;
            this.lblResult.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblResult.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblResult.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblResult.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblResult.Location = new System.Drawing.Point(23, 153);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(954, 74);
            this.lblResult.TabIndex = 2;
            this.lblResult.Text = "";
            //
            // tlpKeypad
            //
            this.tlpKeypad.ColumnCount = 3;
            this.tlpKeypad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpKeypad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpKeypad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tlpKeypad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpKeypad.Location = new System.Drawing.Point(23, 233);
            this.tlpKeypad.Name = "tlpKeypad";
            this.tlpKeypad.RowCount = 4;
            this.tlpKeypad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpKeypad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpKeypad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpKeypad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpKeypad.Size = new System.Drawing.Size(954, 254);
            this.tlpKeypad.TabIndex = 3;
            //
            // pnlInputAction
            //
            this.pnlInputAction.ColumnCount = 2;
            this.pnlInputAction.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlInputAction.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlInputAction.Controls.Add(this.btnBack, 0, 0);
            this.pnlInputAction.Controls.Add(this.btnConfirm, 1, 0);
            this.pnlInputAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInputAction.Location = new System.Drawing.Point(23, 493);
            this.pnlInputAction.Name = "pnlInputAction";
            this.pnlInputAction.RowCount = 1;
            this.pnlInputAction.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlInputAction.Size = new System.Drawing.Size(954, 84);
            this.pnlInputAction.TabIndex = 4;
            //
            // btnBack
            //
            this.btnBack.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.btnBack.Appearance.Options.UseFont = true;
            this.btnBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBack.Location = new System.Drawing.Point(8, 8);
            this.btnBack.Margin = new System.Windows.Forms.Padding(8);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(461, 68);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "QUAY LẠI";
            //
            // btnConfirm
            //
            this.btnConfirm.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.Appearance.Options.UseFont = true;
            this.btnConfirm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnConfirm.Enabled = false;
            this.btnConfirm.Location = new System.Drawing.Point(485, 8);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(8);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(461, 68);
            this.btnConfirm.TabIndex = 1;
            this.btnConfirm.Text = "XÁC NHẬN";
            //
            // tmrIdle
            //
            this.tmrIdle.Interval = 1000;
            this.tmrIdle.Tick += new System.EventHandler(this.tmrIdle_Tick);
            //
            // Dang ky su kien
            //
            this.btnCccd.Click += new System.EventHandler(this.btnCccd_Click);
            this.btnVneId.Click += new System.EventHandler(this.btnVneId_Click);
            this.btnCccdManual.Click += new System.EventHandler(this.btnCccdManual_Click);
            this.btnBhyt.Click += new System.EventHandler(this.btnBhyt_Click);
            this.btnNoPaper.Click += new System.EventHandler(this.btnNoPaper_Click);
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            //
            // frmChooseIdentity
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.pnlInput);
            this.Controls.Add(this.pnlChoose);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmChooseIdentity";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chọn hình thức lấy số";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmChooseIdentity_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmChooseIdentity_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pnlChoose)).EndInit();
            this.pnlChoose.ResumeLayout(false);
            this.tlpOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlInput)).EndInit();
            this.pnlInput.ResumeLayout(false);
            this.tlpInput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtInput.Properties)).EndInit();
            this.pnlInputAction.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblTitle;
        private DevExpress.XtraEditors.PanelControl pnlChoose;
        private System.Windows.Forms.TableLayoutPanel tlpOptions;
        private DevExpress.XtraEditors.SimpleButton btnCccd;
        private DevExpress.XtraEditors.SimpleButton btnVneId;
        private DevExpress.XtraEditors.SimpleButton btnCccdManual;
        private DevExpress.XtraEditors.SimpleButton btnBhyt;
        private DevExpress.XtraEditors.SimpleButton btnNoPaper;
        private DevExpress.XtraEditors.PanelControl pnlInput;
        private System.Windows.Forms.TableLayoutPanel tlpInput;
        private DevExpress.XtraEditors.LabelControl lblGuide;
        private DevExpress.XtraEditors.TextEdit txtInput;
        private DevExpress.XtraEditors.LabelControl lblResult;
        private System.Windows.Forms.TableLayoutPanel tlpKeypad;
        private System.Windows.Forms.TableLayoutPanel pnlInputAction;
        private DevExpress.XtraEditors.SimpleButton btnBack;
        private DevExpress.XtraEditors.SimpleButton btnConfirm;
        private System.Windows.Forms.Timer tmrIdle;
    }
}
