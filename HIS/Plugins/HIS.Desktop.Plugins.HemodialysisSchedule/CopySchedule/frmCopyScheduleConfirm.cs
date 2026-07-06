/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Popup xác nhận sao chép lịch (R6): tóm tắt số ca / số BN ngày nguồn & ngày đích,
 * số BN sẽ thêm mới và danh sách BN trùng (đã có trong ca ngày đích) sẽ bỏ qua.
 */
using HIS.Desktop.Plugins.HemodialysisSchedule.ADO;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HemodialysisSchedule
{
    public partial class frmCopyScheduleConfirm : DevExpress.XtraEditors.XtraForm
    {
        private readonly int addCount;

        public frmCopyScheduleConfirm(string roomText, long sourceDate, long targetDate,
            List<HemodialysisScheduleADO> sourceList, List<HemodialysisScheduleADO> targetList)
        {
            try
            {
                InitializeComponent();

                sourceList = sourceList ?? new List<HemodialysisScheduleADO>();
                targetList = targetList ?? new List<HemodialysisScheduleADO>();

                // Khóa trùng theo cặp (bệnh nhân + ca) đã có ở ngày đích
                var targetKeys = new HashSet<string>(targetList.Select(o => o.TREATMENT_ID + "|" + o.KIDNEY_SHIFT));
                var willAdd = new List<HemodialysisScheduleADO>();
                var willSkip = new List<HemodialysisScheduleADO>();
                foreach (var s in sourceList)
                {
                    if (targetKeys.Contains(s.TREATMENT_ID + "|" + s.KIDNEY_SHIFT))
                        willSkip.Add(s);
                    else
                        willAdd.Add(s);
                }
                this.addCount = willAdd.Count;
                int skipCount = willSkip.Count;

                int sourceCa = sourceList.Select(o => o.KIDNEY_SHIFT).Distinct().Count();
                int targetCa = targetList.Select(o => o.KIDNEY_SHIFT).Distinct().Count();

                // In đậm 3 dòng thông tin đầu
                lblRoom.Font = new Font(lblRoom.Font, FontStyle.Bold);
                lblSourceDate.Font = new Font(lblSourceDate.Font, FontStyle.Bold);
                lblTargetDate.Font = new Font(lblTargetDate.Font, FontStyle.Bold);

                lblRoom.Text = "Phòng chạy:  " + (roomText ?? "");
                lblSourceDate.Text = string.Format("Ngày nguồn:  {0}   ({1} ca - {2} BN)",
                    FormatDate(sourceDate), sourceCa, sourceList.Count);
                lblTargetDate.Text = string.Format("Ngày đích:  {0}   ({1} ca - {2} BN đã có)",
                    FormatDate(targetDate), targetCa, targetList.Count);

                lblAdd.Text = string.Format("✓ Sẽ thêm: {0} BN mới (chưa có trong các ca của ngày đích)", this.addCount);
                lblSkip.Text = string.Format("⚠ Sẽ bỏ qua: {0} BN trùng (đã có trong ca ngày đích)", skipCount);

                var sb = new StringBuilder();
                foreach (var s in willSkip)
                {
                    string name = string.IsNullOrEmpty(s.TDL_PATIENT_NAME) ? s.TREATMENT_CODE : s.TDL_PATIENT_NAME;
                    sb.AppendLine(string.Format("• {0} — đã có trong ca {1} ngày đích", name, s.KIDNEY_SHIFT));
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
