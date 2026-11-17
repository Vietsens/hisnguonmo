/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * vCong60263 - Lịch gửi XML 130 theo mốc giờ.
 * Đặt riêng một tệp partial để không chạm vào phần thiết lập đang chạy của các viện khác.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using HIS.Desktop.Plugins.ExportXmlQD130.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    public partial class frmSettingConfigSync
    {
        /// <summary>Một ngày trong tuần của ô chọn "Ngày áp dụng". 1 = Thứ 2 ... 7 = Chủ nhật.</summary>
        private class ScheduleDayADO
        {
            internal int Value { get; set; }
            internal string Name { get; set; }

            public override string ToString()
            {
                return this.Name;
            }
        }

        private const int SEND_MODE_INDEX_PERIOD = 0;
        private const int SEND_MODE_INDEX_SCHEDULE = 1;
        private const int DEFAULT_LOOK_BACK_DAYS = 1;

        /// <summary>Nạp phần lịch gửi từ thiết lập đã lưu lên giao diện.</summary>
        private void LoadScheduleToForm()
        {
            try
            {
                //Ô chọn chế độ gửi
                if (cboSendMode.Properties.Items.Count == 0)
                {
                    cboSendMode.Properties.Items.Add("Theo chu kỳ");
                    cboSendMode.Properties.Items.Add("Theo mốc giờ");
                }
                cboSendMode.SelectedIndex = configSync.IsScheduleMode()
                    ? SEND_MODE_INDEX_SCHEDULE
                    : SEND_MODE_INDEX_PERIOD;

                //Ô chọn ngày áp dụng
                if (cboScheduleDays.Properties.Items.Count == 0)
                {
                    foreach (ScheduleDayADO day in BuildDayItems())
                    {
                        cboScheduleDays.Properties.Items.Add(new CheckedListBoxItem(day, false));
                    }
                }
                List<int> selectedDays = Xml130Schedule.ParseDays(configSync.scheduleDays);
                //Thiết lập cũ chưa có khóa này (null) thì mặc định áp dụng cả bảy ngày theo QT04.
                //Còn chuỗi rỗng là người dùng đã cố ý bỏ chọn hết -> để nguyên không tích.
                bool checkAllAsDefault = (configSync.scheduleDays == null);
                foreach (CheckedListBoxItem item in cboScheduleDays.Properties.Items)
                {
                    ScheduleDayADO day = item.Value as ScheduleDayADO;
                    item.CheckState = (day != null && (checkAllAsDefault || selectedDays.Contains(day.Value)))
                        ? System.Windows.Forms.CheckState.Checked
                        : System.Windows.Forms.CheckState.Unchecked;
                }

                txtScheduleTimes.Text = configSync.scheduleTimes;
                spnLookBackDays.Value = Xml130Schedule.NormalizeLookBackDays(configSync.GetLookBackDays());
                chkResendFailed.Checked = configSync.isResendFailed;
                chkAutoStart.Checked = configSync.isAutoStart;

                this.ApplySendModeState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static List<ScheduleDayADO> BuildDayItems()
        {
            return new List<ScheduleDayADO>
            {
                new ScheduleDayADO { Value = 1, Name = "Thứ 2" },
                new ScheduleDayADO { Value = 2, Name = "Thứ 3" },
                new ScheduleDayADO { Value = 3, Name = "Thứ 4" },
                new ScheduleDayADO { Value = 4, Name = "Thứ 5" },
                new ScheduleDayADO { Value = 5, Name = "Thứ 6" },
                new ScheduleDayADO { Value = 6, Name = "Thứ 7" },
                new ScheduleDayADO { Value = 7, Name = "Chủ nhật" }
            };
        }

        private void cboSendMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (isLoadingConfig)
                {
                    return;
                }
                this.ApplySendModeState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Chọn chế độ nào thì mở các ô của chế độ đó, làm mờ ô của chế độ còn lại.</summary>
        private void ApplySendModeState()
        {
            try
            {
                bool isSchedule = cboSendMode.SelectedIndex == SEND_MODE_INDEX_SCHEDULE;
                //Đang chạy tiến trình thì không cho sửa nhịp chạy (giữ đúng cách làm sẵn có với ô Chu kỳ)
                spnPeriod.Enabled = !isSchedule && !autoSyncIsRunning;
                txtScheduleTimes.Enabled = isSchedule && !autoSyncIsRunning;
                cboScheduleDays.Enabled = isSchedule && !autoSyncIsRunning;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiểm tra phần lịch gửi trước khi lưu. Trả về false kèm thông báo khi có ô chưa hợp lệ.
        /// </summary>
        private bool ValidateSchedule()
        {
            try
            {
                bool isSchedule = cboSendMode.SelectedIndex == SEND_MODE_INDEX_SCHEDULE;
                if (isSchedule)
                {
                    string raw = (txtScheduleTimes.Text ?? String.Empty).Trim();
                    if (String.IsNullOrEmpty(raw))
                    {
                        XtraMessageBox.Show("Chọn chế độ Theo mốc giờ thì phải khai ít nhất một mốc giờ gửi.",
                            Resources.ResourceMessageLang.ThongBao);
                        txtScheduleTimes.Focus();
                        return false;
                    }
                    //Mọi mốc phải đúng định dạng giờ:phút - sai một mốc là không cho lưu, tránh viện tưởng đã khai xong
                    foreach (string part in raw.Split(Xml130Schedule.TIME_SEPARATOR))
                    {
                        if (String.IsNullOrWhiteSpace(part))
                        {
                            continue;
                        }
                        TimeSpan slot;
                        if (!Xml130Schedule.TryParseTime(part, out slot))
                        {
                            XtraMessageBox.Show(
                                String.Format("Mốc giờ \"{0}\" không hợp lệ. Khai theo dạng giờ:phút, nhiều mốc ngăn cách bởi dấu chấm phẩy. Ví dụ 10:00;16:00",
                                    part.Trim()),
                                Resources.ResourceMessageLang.ThongBao);
                            txtScheduleTimes.Focus();
                            return false;
                        }
                    }
                    if (Xml130Schedule.ParseTimes(raw).Count == 0)
                    {
                        XtraMessageBox.Show("Chọn chế độ Theo mốc giờ thì phải khai ít nhất một mốc giờ gửi.",
                            Resources.ResourceMessageLang.ThongBao);
                        txtScheduleTimes.Focus();
                        return false;
                    }
                }
                else if (spnPeriod.Value <= 0)
                {
                    XtraMessageBox.Show(Resources.ResourceMessageLang.ChuKyPhaiLonHon0);
                    spnPeriod.Focus();
                    return false;
                }

                //QT04: bỏ chọn hết ngày thì lịch không bao giờ chạy - phải báo ngay, đừng để lưu im lặng
                if (cboSendMode.SelectedIndex == SEND_MODE_INDEX_SCHEDULE)
                {
                    bool hasDay = false;
                    foreach (CheckedListBoxItem item in cboScheduleDays.Properties.Items)
                    {
                        if (item.CheckState == System.Windows.Forms.CheckState.Checked)
                        {
                            hasDay = true;
                            break;
                        }
                    }
                    if (!hasDay)
                    {
                        XtraMessageBox.Show(
                            "Chưa chọn ngày áp dụng nào nên lịch sẽ không chạy vào ngày nào cả. Chọn ít nhất một ngày trong tuần.",
                            Resources.ResourceMessageLang.ThongBao);
                        cboScheduleDays.Focus();
                        return false;
                    }
                }

                int lookBack = (int)spnLookBackDays.Value;
                if (lookBack < Xml130Schedule.MIN_LOOK_BACK_DAYS || lookBack > Xml130Schedule.MAX_LOOK_BACK_DAYS)
                {
                    XtraMessageBox.Show(
                        String.Format("Số ngày quét lùi chỉ nhận từ {0} đến {1}.",
                            Xml130Schedule.MIN_LOOK_BACK_DAYS, Xml130Schedule.MAX_LOOK_BACK_DAYS),
                        Resources.ResourceMessageLang.ThongBao);
                    spnLookBackDays.Focus();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>Ghi phần lịch gửi từ giao diện vào thiết lập.</summary>
        private void SaveScheduleToConfig()
        {
            try
            {
                this.configSync.sendModeOption = cboSendMode.SelectedIndex == SEND_MODE_INDEX_SCHEDULE
                    ? ConfigSyncADO.SEND_MODE_SCHEDULE
                    : ConfigSyncADO.SEND_MODE_PERIOD;
                //Chuẩn hoá lại chuỗi mốc giờ: bỏ mốc trùng, sắp xếp tăng dần, đưa về dạng HH:mm
                this.configSync.scheduleTimes = Xml130Schedule.FormatTimes(Xml130Schedule.ParseTimes(txtScheduleTimes.Text));

                List<int> days = new List<int>();
                foreach (CheckedListBoxItem item in cboScheduleDays.Properties.Items)
                {
                    ScheduleDayADO day = item.Value as ScheduleDayADO;
                    if (day != null && item.CheckState == System.Windows.Forms.CheckState.Checked)
                    {
                        days.Add(day.Value);
                    }
                }
                //QT04: lưu ĐÚNG những ngày được tích. Tích hết thì lưu đủ bảy ngày (không rút về rỗng),
                //để chuỗi rỗng chỉ còn một nghĩa duy nhất là bỏ chọn toàn bộ - không ngày nào chạy.
                this.configSync.scheduleDays = Xml130Schedule.FormatDays(days);

                this.configSync.lookBackDays = Xml130Schedule.NormalizeLookBackDays((int)spnLookBackDays.Value);
                this.configSync.isResendFailed = chkResendFailed.Checked;
                this.configSync.isAutoStart = chkAutoStart.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
