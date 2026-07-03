/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Popup xác nhận sao chép lịch (R6): tóm tắt số BN sẽ thêm mới và số/ danh sách BN trùng sẽ skip.
 */
using HIS.Desktop.Plugins.HemodialysisSchedule.ADO;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HemodialysisSchedule
{
    public partial class frmCopyScheduleConfirm : DevExpress.XtraEditors.XtraForm
    {
        private readonly int addCount;

        public frmCopyScheduleConfirm(string roomText, long sourceDate, long targetDate,
            List<HemodialysisScheduleADO> willAdd, List<HemodialysisScheduleADO> willSkip)
        {
            try
            {
                InitializeComponent();

                this.addCount = willAdd != null ? willAdd.Count : 0;
                int skipCount = willSkip != null ? willSkip.Count : 0;

                lblRoom.Text = "Phòng chạy:  " + (roomText ?? "");
                lblSourceDate.Text = "Ngày nguồn:  " + FormatDate(sourceDate);
                lblTargetDate.Text = "Ngày đích:  " + FormatDate(targetDate);

                lblAdd.Text = string.Format("✓ Sẽ thêm: {0} BN mới (chưa có trong các ca của ngày đích)", this.addCount);
                lblSkip.Text = string.Format("⚠ Sẽ skip: {0} BN trùng (đã có trong ca ngày đích)", skipCount);

                var sb = new StringBuilder();
                if (willSkip != null)
                {
                    foreach (var s in willSkip)
                    {
                        sb.AppendLine(string.Format("• {0} — đã có trong ca {1} ngày đích",
                            s.TDL_PATIENT_NAME, s.KIDNEY_SHIFT));
                    }
                }
                memoSkip.Text = sb.ToString();
                memoSkip.Visible = skipCount > 0;

                lblFootnote.Text = "Sao chép chỉ thêm slot mới sang ngày đích, KHÔNG xóa slot đã có, KHÔNG sinh y lệnh chạy thận.";
                btnOk.Text = string.Format("Sao chép {0} BN mới", this.addCount);
                btnOk.Enabled = this.addCount > 0;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private string FormatDate(long dateNumber)
        {
            try
            {
                return Inventec.Common.DateTime.Convert.TimeNumberToDateString(dateNumber) ?? dateNumber.ToString();
            }
            catch
            {
                return dateNumber.ToString();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
