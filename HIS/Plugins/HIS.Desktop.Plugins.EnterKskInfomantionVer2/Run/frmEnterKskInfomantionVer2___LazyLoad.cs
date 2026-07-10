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
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Cờ đã fill dữ liệu cho từng tab (index 0..7) — chống fill lại.</summary>
        private readonly bool[] tabFilled = new bool[8];

        /// <summary>
        /// Suy ra tab mặc định cần fill khi mở (mirror thứ tự ưu tiên của SetTabDefault) — dựa
        /// vào bản ghi KSK đã prefetch (pre*), KHÔNG cần chạy FillData toàn bộ trước.
        /// </summary>
        private int ResolveDefaultTab()
        {
            try
            {
                if (preKskGenerals != null && preKskGenerals.Count > 0) return 0;
                if (preKskOverEighteens != null && preKskOverEighteens.Count > 0) return 1;
                if (preKskUnderEighteens != null && preKskUnderEighteens.Count > 0) return 2;
                if (preKskPeriodDrivers != null && preKskPeriodDrivers.Count > 0) return 3;
                if (preKskDriverCars != null && preKskDriverCars.Count > 0) return 4;
                if (preKskOthers != null && preKskOthers.Count > 0) return 5;
                // Occupational: SetTabDefault hiển thị tab 0 (giữ nguyên hành vi cũ).
                if (preKskOccupationals != null && preKskOccupationals.Count > 0) return 0;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return 0;
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
                FillTabByIndex(tab);
                InitIcdConclusionUcForTab(tab);
                LoadIcdConclusionToUc();
                if (tab == 7) ApplyUnderSixConclusionDefaults();
                LoadConclusionTimeExt();
                SetEnableControl();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            finally { try { this.ResumeLayout(false); } catch { } }
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
