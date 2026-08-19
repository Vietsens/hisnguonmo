/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Lazy-load các tab KSK: chỉ fill tab mặc định (theo bản ghi có sẵn) khi mở form,
 * các tab còn lại fill khi user chuyển sang (SelectedPageChanged). Giảm ~7/8 chi phí
 * FillDataPage* + nhúng UC ICD lúc mở. State chia sẻ (currentKskGeneral...) giữ nguyên
 * ngữ nghĩa: tab nào được fill thì currentKsk* của tab đó mới được set (như code cũ khi
 * FillData chạy), các bước wholesale (SetEnableControl/LoadIcd...) đều guard theo null.
 */
using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Cờ đã fill dữ liệu cho từng tab (index 0..7) — chống fill lại.</summary>
        private readonly bool[] tabFilled = new bool[8];

        /// <summary>Chỉ số 3 tab chọn theo tuổi khi y lệnh chưa có bản ghi KSK nào.</summary>
        private const int TAB_OVER_EIGHTEEN = 1;
        private const int TAB_UNDER_EIGHTEEN = 2;
        private const int TAB_UNDER_SIX = 7;

        /// <summary>
        /// Suy ra tab mặc định cần fill khi mở (mirror thứ tự ưu tiên của SetTabDefault) — dựa
        /// vào bản ghi KSK đã prefetch (pre*), KHÔNG cần chạy FillData toàn bộ trước.
        /// Chưa có bản ghi nào thì chọn theo TUỔI bệnh nhân (xem ResolveDefaultTabByAge).
        /// </summary>
        private int ResolveDefaultTab()
        {
            try
            {
                LogDefaultTabInputs();
                // BẢNG RIÊNG CỦA TẪNG TAB XÉT TRƯỚC, HIS_KSK_GENERAL XÉT SAU CÙNG.
                //
                // HIS_KSK_GENERAL KHÔNG còn là dấu hiệu "hồ sơ Ksk định kỳ": từ khi cụm Kết luận (ICD-10
                // kết luận, ICD tiền sự, KSK_TYPE_ID, ngày kết luận, người kết luận) được lưu tập trung vào
                // bảng này thì LƯU Ở TAB NÀO CŨNG sinh ra một bản ghi GENERAL. Để nguyên luật cũ
                // "có GENERAL -> tab 0" thì lưu ở tab dưới 18 tuổi xong mở lại bị đẩy về tab "Ksk định kỳ"
                // (đã xác nhận bằng log KskTabDefault: general=1, under18=1, over18=0 -> trả về 0).
                int byRecord = ResolveTabBySpecificRecord();
                if (byRecord >= 0) return byRecord;
                // Chỉ có GENERAL mà không có bảng riêng nào -> đúng là hồ sơ Ksk định kỳ.
                if (preKskGenerals != null && preKskGenerals.Count > 0) return 0;
                // Chưa có bản ghi nào -> chọn tab theo tuổi.
                int byAge = ResolveDefaultTabByAge();
                if (byAge >= 0) return byAge;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return 0;
        }

        /// <summary>
        /// Tab suy từ BẢNG RIÊNG của từng loại KSK (không xét HIS_KSK_GENERAL). Trả -1 khi
        /// không bảng riêng nào có bản ghi.
        ///
        /// Nhiều bảng cùng có dự liệu (hồ sơ cũ nhập lẫn, hoặc đã khám nhiều loại) -> ưu tiên tab
        /// KHỚP TUỔI bệnh nhân, vì đó mới là loại hồ sơ đang cần nhập.
        /// </summary>
        private int ResolveTabBySpecificRecord()
        {
            int byAge = ResolveDefaultTabByAge();
            if (byAge >= 0 && HasRecordForTab(byAge)) return byAge;

            if (Cnt(preKskOverEighteens) > 0) return TAB_OVER_EIGHTEEN;
            if (Cnt(preKskUnderEighteens) > 0) return TAB_UNDER_EIGHTEEN;
            if (Cnt(preKskPeriodDrivers) > 0) return 3;
            if (Cnt(preKskDriverCars) > 0) return 4;
            if (Cnt(preKskOthers) > 0) return 5;
            if (Cnt(preKskUnderSixes) > 0) return TAB_UNDER_SIX;
            // Ksk nghề nghiệp hiển thị ở tab 0 — giữ nguyên hành vi cũ.
            if (Cnt(preKskOccupationals) > 0) return 0;
            return -1;
        }

        /// <summary>3 tab chọn theo tuổi đã có bản ghi chưa.</summary>
        private bool HasRecordForTab(int tab)
        {
            if (tab == TAB_OVER_EIGHTEEN) return Cnt(preKskOverEighteens) > 0;
            if (tab == TAB_UNDER_EIGHTEEN) return Cnt(preKskUnderEighteens) > 0;
            if (tab == TAB_UNDER_SIX) return Cnt(preKskUnderSixes) > 0;
            return false;
        }

        /// <summary>
        /// Ghi nhật ký số bản ghi từng loại KSK + tab suy theo tuổi, để đọc được VÌ SAO mở ra
        /// lại đứng ở tab này. Không đổi hành vi — chỉ ghi log.
        /// </summary>
        private void LogDefaultTabInputs()
        {
            try
            {
                LogSystem.Debug(string.Format(
                    "KskTabDefault: general={0}, over18={1}, under18={2}, periodDriver={3},"
                    + " driverCar={4}, other={5}, occupational={6}, underSix={7} | tabByAge={8}"
                    + " | dob={9}, intructionTime={10}",
                    Cnt(preKskGenerals), Cnt(preKskOverEighteens), Cnt(preKskUnderEighteens),
                    Cnt(preKskPeriodDrivers), Cnt(preKskDriverCars), Cnt(preKskOthers),
                    Cnt(preKskOccupationals), Cnt(preKskUnderSixes), ResolveDefaultTabByAge(),
                    (currentServiceReq != null ? currentServiceReq.TDL_PATIENT_DOB : 0),
                    (currentServiceReq != null ? currentServiceReq.INTRUCTION_TIME : 0)));
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static int Cnt(System.Collections.ICollection list)
        {
            return (list != null) ? list.Count : -1;
        }

        /// <summary>
        /// Tab mặc định theo TUỔI bệnh nhân tại thời điểm khám, dùng khi y lệnh chưa có bản ghi KSK:
        /// đủ 18 tuổi -> "Ksk trên 18 tuổi"; từ 6 đến dưới 18 -> "Ksk dưới 18 tuổi";
        /// dưới 6 tuổi -> "Trẻ em dưới 6 tuổi".
        ///
        /// Tuổi tính theo ngày/tháng/năm (AgeInMonthsUnderSix — cùng cách với cảnh báo trẻ dưới 6
        /// tuổi) chứ không lấy hiệu số năm, để mốc tròn tuổi đúng ngày sinh nhật.
        /// Mốc so là giờ chỉ định y lệnh; không có thì lấy giờ hiện tại.
        ///
        /// Trả về -1 khi KHÔNG xác định được tuổi (chưa có y lệnh / không có ngày sinh) — khi đó
        /// giữ nguyên hành vi cũ là tab "Ksk định kỳ".
        /// </summary>
        private int ResolveDefaultTabByAge()
        {
            try
            {
                if (currentServiceReq == null) return -1;
                long dobNum = currentServiceReq.TDL_PATIENT_DOB;
                if (dobNum <= 0) return -1;
                // Bệnh nhân chỉ khai năm sinh: ParseHisDateNumber quy về 01/01 năm đó.
                long examNum = (currentServiceReq.INTRUCTION_TIME > 0)
                    ? currentServiceReq.INTRUCTION_TIME
                    : Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));

                int months = AgeInMonthsUnderSix(dobNum, examNum);
                if (months >= 18 * 12) return TAB_OVER_EIGHTEEN;
                if (months >= 6 * 12) return TAB_UNDER_EIGHTEEN;
                return TAB_UNDER_SIX;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return -1; }
        }

        /// <summary>Gọi đúng FillDataPage* theo tab index.</summary>
        private void FillTabByIndex(int tab)
        {
            switch (tab)
            {
                case 0: FillDataPageGenaral(); break;
                case 1: FillDataPageOverEighteen(); break;
                case 2: FillDataPageUnderEighteen(); break;
                case 3: FillDataPagePeriodDriver(); break;
                case 4: FillDataPageDriverCar(); break;
                case 5: FillDataPageKSKOther(); break;
                case 6: FillDataPageOccupational(); break;
                case 7: FillDataPageUnderSix(); break;
            }
        }

        /// <summary>
        /// Fill 1 tab (idempotent theo tabFilled): dữ liệu + nhúng UC ICD kết luận của tab + đổ ICD +
        /// (tab trẻ &lt;6) default kết luận + ngày kết luận + enable control. Chạy khi mở tab mặc định
        /// và khi user chuyển sang tab lần đầu.
        /// </summary>
        private void EnsureTabLoaded(int tab)
        {
            if (tab < 0 || tab > 7 || tabFilled[tab]) return;
            tabFilled[tab] = true;
            try
            {
                this.SuspendLayout();
                // CHỐNG DÍNH DỮ LIỆU Y LỆNH TRƯỚC: xóa sạch mọi editor trên trang tab trước khi đổ dữ liệu.
                // (ResetControl* của từng tab thiếu sót; nhánh else FillData* chỉ phủ 1 phần control.)
                ClearTabInputEditors(tab);
                // Phải đọc mặc định tiêm chủng TRƯỚC FillTabByIndex: SetDafaultGrid() nằm trong
                // FillTabByIndex, nếu init sau thì lúc dựng lưới chưa biết mặc định là gì.
                if (tab == 2) InitDefaultVaccineToggle();
                FillTabByIndex(tab);
                InitIcdConclusionUcForTab(tab);
                LoadIcdConclusionToUc();
                if (tab == 7) ApplyUnderSixConclusionDefaults();
                // Đối tượng + Nguồn chi trả (bổ sung cho tab dưới 18 / trẻ <6) — init combo + đổ giá trị (STUB DB chờ cột).
                if (tab == 2) { LoadAdminCombosUnderEighteen(); InitUnderEighteenTextLibButtons(); }
                else if (tab == 7) { LoadAdminCombosUnderSix(); LoadAccompanyInfoUnderSix(); }
                // Người khám (kết luận) là GridLookUpEdit thường -> bị ClearTabInputEditors xóa; FillTabByIndex
                // không đổ lại (do LoadConcluderComboExt phụ trách riêng theo HIS_KSK_GENERAL) -> đổ lại tại đây,
                // nếu không khi chuyển sang tab ≥18/dưới 18 người khám sẽ hiển thị trống dù DB có CONCLUDER_LOGINNAME.
                LoadConcluderComboExt();
                LoadConclusionTimeExt();
                SetEnableControl();
                UpdateKskNumberDisplay(); // đổ "Số thứ tự KSK" ngay khi nạp tab (kể cả lúc chỉ MỞ xem, không đổi tab / không lưu)
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            finally { try { this.ResumeLayout(false); } catch { } }
        }

        /// <summary>
        /// Xóa sạch mọi editor nhập liệu trên TRANG TAB (xtraTabControl1.TabPages[tab]) trước khi fill —
        /// để đổi y lệnh không bị dính giá trị của y lệnh trước ở những control mà FillData/ResetControl bỏ sót.
        /// Trừ: UC ICD kết luận/tiền sử (nạp riêng qua LoadIcdConclusionToUc/LoadKskHistoryIcdToUc) và combo
        /// chọn-nhiều (GridCheckMarksSelection — clear riêng qua SetKskObjectValue). Header BN + nút nằm NGOÀI
        /// xtraTabControl1 nên không bị đụng.
        /// </summary>
        private void ClearTabInputEditors(int tab)
        {
            try
            {
                if (this.xtraTabControl1 == null) return;
                if (tab < 0 || tab >= this.xtraTabControl1.TabPages.Count) return;
                ClearInputEditorsRecursive(this.xtraTabControl1.TabPages[tab]);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void ClearInputEditorsRecursive(Control parent)
        {
            if (parent == null) return;
            foreach (Control c in parent.Controls)
            {
                // UC có cơ chế nạp/xóa riêng -> KHÔNG đụng (và không đệ quy vào trong).
                if (c is UcKskConclusionIcd || c is UcKskHistoryIcd) continue;

                // Toggle "Mặc định:" của lưới tiêm chủng là CẤU HÌNH của máy, không phải dự liệu bệnh nhân
                // -> không được xóa (xóa là mất lựa chọn đã lưu vì CheckStateChanged sẽ ghi đè ControlState).
                if (c == chkDefaultVaccine3) continue;

                CheckEdit chk = c as CheckEdit;
                if (chk != null) { chk.Checked = false; continue; }

                BaseEdit be = c as BaseEdit;
                if (be != null)
                {
                    // Combo chọn-nhiều (Đối tượng) -> để SetKskObjectValue xử lý, tránh phá trạng thái tick.
                    GridLookUpEdit gle = be as GridLookUpEdit;
                    if (gle != null && gle.Properties != null
                        && gle.Properties.Tag is HIS.Desktop.Utilities.Extensions.GridCheckMarksSelection)
                        continue;
                    be.EditValue = null;
                    continue;
                }
                if (c.HasChildren) ClearInputEditorsRecursive(c);
            }
        }

        /// <summary>Nhúng UcKskConclusionIcd cho ĐÚNG 1 tab (panel host đặt sẵn trong Designer).</summary>
        private void InitIcdConclusionUcForTab(int tab)
        {
            try
            {
                if (dicIcdConclusionUc.ContainsKey(tab)) return;
                Control host = null;
                switch (tab)
                {
                    case 0: host = this.panel1; break;
                    case 1: host = this.panel2; break;
                    case 2: host = this.panel3; break;
                    case 3: host = this.panel4; break;
                    case 4: host = this.panel5; break;
                    case 5: host = this.panel6; break;
                    case 6: host = this.panel7; break;
                    case 7: host = this.panel8; break;
                }
                if (host == null) return;
                UcKskConclusionIcd uc = new UcKskConclusionIcd();
                uc.Dock = DockStyle.Fill;
                host.Controls.Add(uc);
                uc.InitUc();
                dicIcdConclusionUc[tab] = uc;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
    }
}
