/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisPatientPackage
{
    /// <summary>
    /// Popup nhập lý do khóa gói. Dựng UI bằng code (không Designer) — dialog phụ trợ gọn nhẹ.
    /// Dùng: var frm = new frmLockReason(caption, prompt); if (frm.ShowDialog()==OK) { var r = frm.Reason; }
    /// </summary>
    public class frmLockReason : XtraForm
    {
        private MemoEdit memoReason;
        private SimpleButton btnOk;
        private SimpleButton btnCancel;

        /// <summary>Lý do người dùng nhập (đã Trim).</summary>
        public string Reason { get; private set; }

        public frmLockReason(string caption, string prompt)
        {
            try
            {
                this.Text = caption;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.ShowInTaskbar = false;
                this.StartPosition = FormStartPosition.CenterParent;
                this.ClientSize = new Size(420, 200);

                LabelControl lbl = new LabelControl();
                lbl.Text = prompt;
                lbl.Location = new Point(12, 12);
                lbl.AutoSizeMode = LabelAutoSizeMode.None;
                lbl.Size = new Size(396, 16);

                memoReason = new MemoEdit();
                memoReason.Location = new Point(12, 34);
                memoReason.Size = new Size(396, 110);
                memoReason.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

                btnOk = new SimpleButton();
                btnOk.Text = HIS.Desktop.Plugins.HisPatientPackage.Resources.ResourceMessage.DongY;
                btnOk.Size = new Size(90, 28);
                btnOk.Location = new Point(228, 158);
                btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                btnOk.DialogResult = DialogResult.OK;
                btnOk.Click += btnOk_Click;

                btnCancel = new SimpleButton();
                btnCancel.Text = HIS.Desktop.Plugins.HisPatientPackage.Resources.ResourceMessage.Huy;
                btnCancel.Size = new Size(90, 28);
                btnCancel.Location = new Point(318, 158);
                btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                btnCancel.DialogResult = DialogResult.Cancel;

                this.Controls.Add(lbl);
                this.Controls.Add(memoReason);
                this.Controls.Add(btnOk);
                this.Controls.Add(btnCancel);
                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                this.Reason = (memoReason.Text ?? string.Empty).Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
