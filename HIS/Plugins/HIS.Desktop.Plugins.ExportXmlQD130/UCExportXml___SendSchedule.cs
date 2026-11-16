/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * vCong60263 - Gửi tự động XML 130 theo mốc giờ + Nhật ký gửi.
 * Tách riêng một tệp partial để không trộn vào phần đang chạy của các viện khác.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using HIS.Desktop.Plugins.ExportXmlQD130.Base;
using Inventec.Common.Logging;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    public partial class UCExportXml
    {
        //Hình thức gửi ghi vào nhật ký
        private const short XML130_SEND_MODE_AUTO = 1;
        private const short XML130_SEND_MODE_MANUAL = 2;
        //Nhịp kiểm tra khi chạy theo mốc giờ: 30 giây một lần là đủ mịn cho mốc tính theo phút
        private const int XML130_SCHEDULE_TICK_MS = 30000;
        //Quy ước sẵn có của XML130_RESULT trên hồ sơ: 2 = gửi thành công, 1 = gửi thất bại
        private const short XML130_RESULT_SUCCESS = 2;
        private const short XML130_RESULT_FAILED = 1;

        //Các mốc đã chạy, để mỗi mốc chỉ chạy một lần trong ngày
        private readonly HashSet<string> xml130RanSlotKeys = new HashSet<string>();
        //Mã lượt gửi tự động hiện tại - các hồ sơ cùng một lượt mang cùng mã
        private string xml130RoundCode;
        //Mã lượt của đợt gửi tay - tách khỏi mã lượt tự động để hai bên không dùng chung (QT13)
        private string xml130ManualRoundCode;
        //Đang ở trong một lượt gửi tự động hay không. Đặt ở đầu/cuối backgroundWorker1_DoWork.
        //KHÔNG dùng isAutoSync để phân biệt: cờ đó chỉ nói tiến trình tự động đang bật,
        //cán bộ bấm gửi tay lúc đó vẫn phải được ghi là gửi tay.
        private volatile bool xml130IsAutoRound;
        private int xml130RoundSuccess;
        private int xml130RoundFailed;
        private DateTime? xml130LastRoundTime;
        private string xml130LastSkipReason;
        //Dọn nhật ký quá hạn: một lần mỗi ngày
        private DateTime xml130LastCleanupDate = DateTime.MinValue;

        #region Cấu hình

        /// <summary>Viện có bật ghi nhật ký gửi hay không.</summary>
        private bool Xml130IsSaveSendLog()
        {
            return HisConfigCFG.XML130_SAVE_SEND_LOG == "1";
        }

        /// <summary>Đang chạy theo mốc giờ hay theo chu kỳ.</summary>
        private bool Xml130IsScheduleMode()
        {
            return this.configSync != null && this.configSync.IsScheduleMode();
        }

        #endregion

        #region Mở màn Nhật ký

        /// <summary>
        /// Mục "Nhật ký gửi" chỉ hiện khi viện bật cấu hình ghi nhật ký.
        /// Đặt ở menu chuột phải của lưới, KHÔNG thêm nút vào hàng công cụ:
        /// hàng công cụ đã kín chỗ (mọi hàng vừa khít 1088px) và x=1088 là chỗ
        /// bắt đầu của panel Tên dịch vụ — chèn ô bố cục vào đó sẽ lấy mất bề
        /// rộng của panel đó.
        /// </summary>
        private bool Xml130IsShowSendLogMenu()
        {
            return this.Xml130IsSaveSendLog();
        }

        private void Xml130OpenSendLog(object sender, EventArgs e)
        {
            try
            {
                frmXml130SendLog frm = new frmXml130SendLog();
                frm.ShowDialog(this.ParentForm);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                XtraMessageBox.Show("Không mở được màn hình Nhật ký gửi. Xem nhật ký kỹ thuật của phần mềm để biết chi tiết.",
                    Resources.ResourceMessageLang.ThongBao);
            }
        }

        #endregion

        #region Lịch gửi

        /// <summary>Nhịp của bộ đếm: chế độ mốc giờ kiểm tra mỗi 30 giây, chế độ chu kỳ giữ nguyên như trước.</summary>
        private int Xml130GetTimerInterval()
        {
            if (this.Xml130IsScheduleMode())
            {
                return XML130_SCHEDULE_TICK_MS;
            }
            return (int)(this.configSync.period * 60000);
        }

        /// <summary>
        /// Quyết định lượt này có chạy hay không.
        /// Chế độ chu kỳ: luôn chạy (y như trước).
        /// Chế độ mốc giờ: chỉ chạy khi có mốc đã tới mà hôm nay chưa chạy.
        /// </summary>
        private bool Xml130TryBeginAutoRound()
        {
            try
            {
                if (!this.Xml130IsScheduleMode())
                {
                    this.Xml130BeginRound(null);
                    return true;
                }

                DateTime now = DateTime.Now;
                this.Xml130DropSlotKeysOfOtherDays(now);

                TimeSpan? due = Xml130Schedule.FindDueSlot(now,
                    this.configSync.scheduleTimes, this.configSync.scheduleDays, this.xml130RanSlotKeys);
                if (!due.HasValue)
                {
                    this.Xml130UpdateStatusCaption();
                    return false;
                }

                //QT07: mở muộn thì chạy bù ĐÚNG MỘT lượt -> đánh dấu mọi mốc đã qua trong ngày,
                //không chỉ riêng mốc muộn nhất, nếu không các mốc còn lại sẽ chạy dồn ở những nhịp sau.
                Xml130Schedule.MarkPassedSlotsAsRun(now,
                    this.configSync.scheduleTimes, this.configSync.scheduleDays, this.xml130RanSlotKeys);

                //Lượt trước chưa xong thì bỏ qua mốc này, KHÔNG xếp hàng chờ (tránh chạy chồng)
                if (backgroundWorker1.IsBusy)
                {
                    this.xml130LastSkipReason = String.Format("bỏ qua mốc {0} do lượt trước chưa xong",
                        Xml130Schedule.FormatTime(due.Value));
                    LogSystem.Info("XML130 lich gui: " + this.xml130LastSkipReason);
                    this.Xml130UpdateStatusCaption();
                    return false;
                }

                this.Xml130BeginRound(Xml130Schedule.FormatTime(due.Value));
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                //Có lỗi khi xét lịch thì không gửi, để lượt sau xét lại - an toàn hơn là gửi nhầm
                return false;
            }
        }

        /// <summary>Sang ngày mới thì bỏ các mốc đã chạy của ngày cũ.</summary>
        private void Xml130DropSlotKeysOfOtherDays(DateTime now)
        {
            try
            {
                string prefix = now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "_";
                List<string> stale = new List<string>();
                foreach (string key in this.xml130RanSlotKeys)
                {
                    if (!key.StartsWith(prefix, StringComparison.Ordinal))
                    {
                        stale.Add(key);
                    }
                }
                foreach (string key in stale)
                {
                    this.xml130RanSlotKeys.Remove(key);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void Xml130BeginRound(string slotText)
        {
            this.xml130RoundSuccess = 0;
            this.xml130RoundFailed = 0;
            this.xml130LastSkipReason = null;
            this.xml130LastRoundTime = DateTime.Now;
            this.xml130RoundCode = this.Xml130BuildRoundCode();
            LogSystem.Info(String.Format("XML130 bat dau luot gui. Ma luot: {0}{1}",
                this.xml130RoundCode,
                String.IsNullOrEmpty(slotText) ? String.Empty : (", moc gio: " + slotText)));
            this.Xml130UpdateStatusCaption();
            this.Xml130RunDailyLogCleanup();
        }

        /// <summary>Mã lượt: thời điểm bắt đầu + tên máy, đủ phân biệt giữa các máy và các lượt.</summary>
        private string Xml130BuildRoundCode()
        {
            string machine = this.Xml130GetMachineName();
            string stamp = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            string code = stamp + "_" + machine;
            return code.Length > 50 ? code.Substring(0, 50) : code;
        }

        private string Xml130GetMachineName()
        {
            try
            {
                string name = Environment.MachineName ?? String.Empty;
                return name.Length > 100 ? name.Substring(0, 100) : name;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return String.Empty;
            }
        }

        #endregion

        #region Phạm vi hồ sơ của mỗi lượt

        /// <summary>
        /// Mốc đầu khoảng quét: đầu ngày của N ngày trước.
        /// Không khai (0) thì giữ nguyên hành vi cũ là đầu ngày hiện tại.
        /// </summary>
        private DateTime Xml130GetScanFromDate()
        {
            int lookBack = this.configSync != null
                ? Xml130Schedule.NormalizeLookBackDays(this.configSync.GetLookBackDays())
                : Xml130Schedule.DEFAULT_LOOK_BACK_DAYS;
            return DateTime.Today.AddDays(-lookBack);
        }

        /// <summary>
        /// Khử trùng hồ sơ theo mã, giữ nguyên thứ tự xuất hiện đầu tiên.
        /// Chế độ XML thông tuyến chạy hai lượt lọc (khóa viện phí / kết thúc điều trị) rồi ghép lại,
        /// một hồ sơ thỏa cả hai mốc sẽ nằm hai lần trong danh sách và bị gửi hai lần trong cùng một lượt.
        /// Khoảng quét lùi càng rộng thì khả năng trùng càng cao.
        /// </summary>
        private List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1> Xml130DistinctById(
            List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1> source)
        {
            try
            {
                if (source == null || source.Count < 2)
                {
                    return source;
                }
                HashSet<long> seen = new HashSet<long>();
                List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1> result =
                    new List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1>(source.Count);
                foreach (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1 item in source)
                {
                    if (item != null && seen.Add(item.ID))
                    {
                        result.Add(item);
                    }
                }
                if (result.Count != source.Count)
                {
                    LogSystem.Info("XML130: bo " + (source.Count - result.Count)
                        + " ho so trung trong cung mot luot gui.");
                }
                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return source;
            }
        }

        /// <summary>
        /// Lấy thêm hồ sơ đã gửi thất bại trong cùng khoảng quét và ghép vào danh sách của lượt gửi.
        /// Dùng lại đúng bộ điều kiện lọc của lượt, chỉ đổi điều kiện trạng thái gửi.
        /// </summary>
        private List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1> Xml130AppendFailedTreatments(
            MOS.Filter.HisTreatmentView1Filter filter,
            List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1> current)
        {
            List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1> result = current
                ?? new List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1>();
            try
            {
                //XML130_RESULT: 2 = gửi thành công, 1 = gửi thất bại
                filter.HAS_XML130_RESULT = null;
                filter.XML130_RESULT = XML130_RESULT_FAILED;
                List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1> failed =
                    new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam())
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1>>(
                            "api/HisTreatment/GetView1",
                            HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (failed == null || failed.Count == 0)
                {
                    return result;
                }

                HashSet<long> existed = new HashSet<long>();
                foreach (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1 item in result)
                {
                    existed.Add(item.ID);
                }
                int added = 0;
                foreach (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1 item in failed)
                {
                    if (existed.Add(item.ID))
                    {
                        result.Add(item);
                        added++;
                    }
                }
                LogSystem.Info("XML130: lay them " + added + " ho so gui loi de gui lai o luot nay.");
            }
            catch (Exception ex)
            {
                //Lấy thêm hồ sơ lỗi mà hỏng thì vẫn gửi nhóm hồ sơ chưa gửi, không làm hỏng cả lượt
                LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        #region Thông tin kèm theo mỗi lần gửi

        /// <summary>
        /// Gắn thời điểm gửi, hình thức gửi, máy và mã lượt vào dữ liệu ghi nhận kết quả gửi.
        /// Máy chủ vẫn tự quyết định có ghi nhật ký hay không theo cấu hình.
        /// </summary>
        private void Xml130ApplySendInfo(HisTreatmentXmlResultSDO sdo)
        {
            try
            {
                if (sdo == null)
                {
                    return;
                }
                //QT13: phân biệt theo LƯỢT đang chạy, không theo việc tiến trình tự động có đang bật hay không.
                bool isAutoRound = this.xml130IsAutoRound;

                sdo.SendTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture));
                sdo.SendMode = isAutoRound ? XML130_SEND_MODE_AUTO : XML130_SEND_MODE_MANUAL;
                sdo.MachineName = this.Xml130GetMachineName();

                if (isAutoRound)
                {
                    if (String.IsNullOrEmpty(this.xml130RoundCode))
                    {
                        this.xml130RoundCode = this.Xml130BuildRoundCode();
                    }
                    sdo.SendRoundCode = this.xml130RoundCode;
                }
                else
                {
                    //Gửi tay: cả đợt dùng chung một mã lượt riêng, không mượn mã của lượt tự động
                    if (String.IsNullOrEmpty(this.xml130ManualRoundCode))
                    {
                        this.xml130ManualRoundCode = this.Xml130BuildRoundCode();
                    }
                    sdo.SendRoundCode = this.xml130ManualRoundCode;
                }

                sdo.IsWriteLog = this.Xml130IsSaveSendLog();
                //Hàm này chỉ được gọi khi đã thực sự gửi lên cổng - nhánh chỉ xuất file XML không gọi vào đây.
                sdo.IsSent = true;

                //Số đếm này dùng cho cảnh báo cuối LƯỢT TỰ ĐỘNG - gửi tay không được cộng vào,
                //nếu không cảnh báo cuối lượt sẽ báo sai số. XML130_RESULT: 2 = thành công, khác = thất bại.
                if (isAutoRound)
                {
                    if (sdo.XmlResult == XML130_RESULT_SUCCESS)
                    {
                        this.xml130RoundSuccess++;
                    }
                    else
                    {
                        this.xml130RoundFailed++;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Gửi tay: mở một đợt mới để cả đợt mang cùng một mã lượt.
        /// Vẫn mở bình thường khi tiến trình tự động đang bật - đợt gửi tay có mã riêng,
        /// không dùng chung mã với lượt tự động và không đụng vào số đếm của lượt đó (QT13).
        /// </summary>
        private void Xml130BeginManualRound()
        {
            try
            {
                this.xml130ManualRoundCode = this.Xml130BuildRoundCode();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        #region Kết thúc lượt

        /// <summary>
        /// Kết thúc một lượt gửi tự động: cập nhật dòng trạng thái và cảnh báo nếu có hồ sơ lỗi.
        /// Cảnh báo KHÔNG chặn thao tác và KHÔNG làm dừng các lượt sau.
        /// </summary>
        private void Xml130EndAutoRound()
        {
            try
            {
                this.Xml130UpdateStatusCaption();
                if (!this.isAutoSync || this.xml130RoundFailed <= 0)
                {
                    return;
                }
                string message = String.Format(
                    "Lượt gửi XML 130 lúc {0} có {1} hồ sơ gửi lỗi.{2}",
                    this.xml130LastRoundTime.HasValue
                        ? this.xml130LastRoundTime.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                        : String.Empty,
                    this.xml130RoundFailed,
                    this.Xml130IsSaveSendLog() ? " Mở nút Nhật ký để xem chi tiết." : String.Empty);
                LogSystem.Warn("XML130: " + message);
                //Hiện cảnh báo không chặn, người dùng đóng lúc nào cũng được
                XtraMessageBox.Show(message, Resources.ResourceMessageLang.ThongBao);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        #region Dòng trạng thái

        /// <summary>
        /// Hiện trạng thái ngay cạnh nút Gửi tự động: đang bật/tắt, mốc kế tiếp, lượt gần nhất.
        /// Dùng phần nhãn của chính ô bố cục nên không thêm điều khiển mới vào hàng công cụ;
        /// viện không bật gửi tự động thì nhãn ẩn, giao diện y như trước.
        /// </summary>
        private void Xml130UpdateStatusCaption()
        {
            try
            {
                if (!this.isAutoSync)
                {
                    btnAutoSync.ToolTip = Resources.ResourceMessageLang.DongBoTuDong;
                    return;
                }

                //Nhãn NGẮN cạnh nút (hàng công cụ chật), chi tiết đầy đủ để ở phần chú giải khi rê chuột
                string shortText;
                List<string> full = new List<string>();
                full.Add("Tiến trình gửi tự động đang bật");

                if (this.Xml130IsScheduleMode())
                {
                    DateTime? next = Xml130Schedule.FindNextSlot(DateTime.Now,
                        this.configSync.scheduleTimes, this.configSync.scheduleDays, this.xml130RanSlotKeys);
                    if (next.HasValue)
                    {
                        string nextText = next.Value.ToString(
                            next.Value.Date == DateTime.Today ? "HH:mm" : "dd/MM HH:mm", CultureInfo.InvariantCulture);
                        shortText = "Tiếp: " + nextText;
                        full.Add("Mốc gửi kế tiếp: " + nextText);
                    }
                    else if (!Xml130Schedule.IsApplyDay(DateTime.Now, this.configSync.scheduleDays))
                    {
                        shortText = "Nghỉ hôm nay";
                        full.Add("Hôm nay không nằm trong những ngày áp dụng lịch");
                    }
                    else
                    {
                        shortText = "Hết mốc";
                        full.Add("Đã chạy hết các mốc giờ của hôm nay");
                    }
                    full.Add("Các mốc đã khai: " + (String.IsNullOrEmpty(this.configSync.scheduleTimes)
                        ? "(chưa khai)" : this.configSync.scheduleTimes));
                }
                else
                {
                    shortText = String.Format(CultureInfo.InvariantCulture, "Chu kỳ {0}'", this.configSync.period);
                    full.Add(String.Format(CultureInfo.InvariantCulture, "Chạy theo chu kỳ {0} phút", this.configSync.period));
                }

                if (this.xml130LastRoundTime.HasValue)
                {
                    full.Add(String.Format(CultureInfo.InvariantCulture, "Lượt gần nhất {0}: {1} hồ sơ gửi, {2} hồ sơ lỗi",
                        this.xml130LastRoundTime.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                        this.xml130RoundSuccess + this.xml130RoundFailed,
                        this.xml130RoundFailed));
                    if (this.xml130RoundFailed > 0)
                    {
                        shortText += String.Format(CultureInfo.InvariantCulture, " · {0} lỗi", this.xml130RoundFailed);
                    }
                }
                if (!String.IsNullOrEmpty(this.xml130LastSkipReason))
                {
                    full.Add("Ghi nhận: " + this.xml130LastSkipReason);
                }

                //KHÔNG gắn nhãn vào ô bố cục: hàng công cụ đã kín chỗ, thêm nhãn sẽ
                //lấy mất bề rộng của các nút khác. Trạng thái để ở chú giải của nút.
                full.Insert(0, shortText);
                btnAutoSync.ToolTip = String.Join(Environment.NewLine, full.ToArray());
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        #region Cột Thời gian gửi

        private DevExpress.XtraGrid.Columns.GridColumn gridColXml130SendTime;

        /// <summary>
        /// Thêm cột "Thời gian gửi" vào cuối lưới hồ sơ.
        /// Cột này không gắn cấu hình: thời điểm gửi được ghi cho mọi viện, đưa XML 130 về ngang
        /// các luồng liên thông khác vốn đã lưu thời điểm.
        /// </summary>
        private void Xml130InitSendTimeColumn()
        {
            try
            {
                if (this.gridColXml130SendTime != null)
                {
                    return;
                }
                this.gridColXml130SendTime = this.gridViewTreatment.Columns.AddField("XML130_SEND_TIME");
                this.gridColXml130SendTime.Caption = "Thời gian gửi";
                this.gridColXml130SendTime.Name = "gridColXml130SendTime";
                this.gridColXml130SendTime.OptionsColumn.AllowEdit = false;
                this.gridColXml130SendTime.Width = 130;
                //Thiếu dòng này thì cột có thể co về 0 khi lưới chia lại bề rộng
                this.gridColXml130SendTime.OptionsColumn.FixedWidth = true;
                this.gridColXml130SendTime.Visible = true;
                this.gridColXml130SendTime.VisibleIndex = this.gridViewTreatment.Columns.Count;

                //Đăng ký thêm - không gỡ bỏ trình xử lý nào sẵn có của màn hình
                this.gridViewTreatment.CustomColumnDisplayText += this.GridViewTreatment_Xml130SendTimeDisplayText;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>Đổi số yyyyMMddHHmmss thành dạng ngày giờ đọc được.</summary>
        private void GridViewTreatment_Xml130SendTimeDisplayText(object sender,
            DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column == null || e.Column.FieldName != "XML130_SEND_TIME")
                {
                    return;
                }
                if (e.Value == null)
                {
                    e.DisplayText = String.Empty;
                    return;
                }
                long value;
                if (!Int64.TryParse(e.Value.ToString(), out value) || value <= 0)
                {
                    e.DisplayText = String.Empty;
                    return;
                }
                DateTime? time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(value);
                e.DisplayText = time.HasValue
                    ? time.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                    : String.Empty;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Ghi nhớ bật gửi tự động

        /// <summary>
        /// Mở màn hình mà thiết lập có bật "Ghi nhớ bật gửi tự động" thì chạy tiếp tiến trình theo lịch đã khai.
        /// Viện không bật thì không làm gì - đúng như trước nay.
        /// </summary>
        private void Xml130AutoStartIfRemembered()
        {
            try
            {
                if (this.configSync == null || !this.configSync.isAutoStart || this.isAutoSync)
                {
                    return;
                }
                //Chế độ chu kỳ vẫn phải có chu kỳ hợp lệ; chế độ mốc giờ phải có ít nhất một mốc
                if (this.Xml130IsScheduleMode())
                {
                    if (Xml130Schedule.ParseTimes(this.configSync.scheduleTimes).Count == 0)
                    {
                        LogSystem.Warn("XML130: bat ghi nho gui tu dong nhung chua khai moc gio nao, khong tu chay.");
                        return;
                    }
                }
                else if (this.configSync.period <= 0)
                {
                    LogSystem.Warn("XML130: bat ghi nho gui tu dong nhung chu ky khong hop le, khong tu chay.");
                    return;
                }

                //Ký số bật mà chưa khai thiết bị thì không tự chạy, tránh cả lượt gửi hỏng
                if (chkSignFileCertUtil.Checked
                    && (this.SettingSignADO == null || String.IsNullOrEmpty(this.SettingSignADO.SerialNumber)))
                {
                    LogSystem.Warn("XML130: bat ghi nho gui tu dong nhung chua co thong tin ky so, khong tu chay.");
                    return;
                }

                this.isNotFileSign = !chkSignFileCertUtil.Checked;
                this.isAutoSync = true;
                btnAutoSync.Text = Resources.ResourceMessageLang.DangDongBo;
                btnAutoSync.ToolTip = Resources.ResourceMessageLang.DangChayTienTrinhDongBoDuLieuXml130LenCongBHYT;
                LogSystem.Info("XML130: tu chay tiep tien trinh gui tu dong theo thiet lap da ghi nho.");
                this.StartTimer();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Có đủ điều kiện để bật tiến trình gửi tự động hay không.
        /// Chế độ chu kỳ: phải có chu kỳ > 0 (y như trước). Chế độ mốc giờ: phải khai ít nhất một mốc.
        /// </summary>
        private bool Xml130CanStartAutoSync()
        {
            try
            {
                if (this.configSync == null)
                {
                    return false;
                }
                if (this.Xml130IsScheduleMode())
                {
                    if (Xml130Schedule.ParseTimes(this.configSync.scheduleTimes).Count > 0)
                    {
                        return true;
                    }
                    XtraMessageBox.Show("Chế độ gửi đang đặt Theo mốc giờ nhưng chưa khai mốc giờ nào. Mở Thiết lập gửi hồ sơ tự động để khai mốc giờ.",
                        Resources.ResourceMessageLang.ThongBao);
                    return false;
                }
                return this.configSync.period > 0;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>Tắt tiến trình: xoá ghi nhớ bật và các mốc đã chạy, trả dòng trạng thái về rỗng.</summary>
        private void Xml130ForgetAutoStart()
        {
            try
            {
                this.xml130RanSlotKeys.Clear();
                this.xml130LastSkipReason = null;
                this.Xml130UpdateStatusCaption();
                if (this.configSync == null || !this.configSync.isAutoStart)
                {
                    return;
                }
                this.configSync.isAutoStart = false;
                //Ghi ngay vào thiết lập của máy để lần mở sau không tự chạy
                this.UpdateConfigSign(this.configSync);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        #region Dọn nhật ký quá hạn

        /// <summary>
        /// Gọi máy chủ dọn nhật ký quá hạn một lần mỗi ngày, ở lượt gửi tự động đầu tiên trong ngày.
        /// Chạy ngầm, lỗi thì bỏ qua và không ảnh hưởng việc gửi.
        /// </summary>
        private void Xml130RunDailyLogCleanup()
        {
            try
            {
                if (!this.Xml130IsSaveSendLog())
                {
                    return;
                }
                if (this.xml130LastCleanupDate.Date == DateTime.Today)
                {
                    return;
                }
                this.xml130LastCleanupDate = DateTime.Today;

                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Inventec.Core.CommonParam paramCleanup = new Inventec.Core.CommonParam();
                        long deleted = new Inventec.Common.Adapter.BackendAdapter(paramCleanup)
                            .Post<long>("api/HisTreatmentXmlLog/DeleteExpired",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, new object(), paramCleanup);
                        LogSystem.Info("XML130_SEND_LOG: don nhat ky qua han xong, so dong: " + deleted);
                    }
                    catch (Exception exCleanup)
                    {
                        LogSystem.Warn("XML130_SEND_LOG: don nhat ky qua han that bai (bo qua).", exCleanup);
                    }
                });
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
