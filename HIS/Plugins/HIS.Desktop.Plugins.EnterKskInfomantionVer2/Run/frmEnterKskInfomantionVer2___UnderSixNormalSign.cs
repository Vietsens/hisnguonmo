/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tab "Trẻ em dưới 6 tuổi" — mục III. ĐÁNH GIÁ DINH DƯỠNG, hàng "Dấu hiệu":
 * ô "Bình thường" (chkIsNormalNutritionSign8) loại trừ lẫn nhau với 5 ô dấu hiệu bất thường.
 *
 * Ô này KHÔNG có cột trong HIS_KSK_UNDER_SIX nên KHÔNG lưu DB — khi nạp lại thì suy ra
 * từ 5 cờ đã lưu (cả 5 đều không tích = bình thường). Xem SyncNormalNutritionSignFromFlags.
 */
using System;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        // Chống đệ quy: khi code tự đổi Checked thì CheckedChanged không được xử lý tiếp.
        private bool isSyncingNutritionSign;

        /// <summary>5 ô dấu hiệu BẤT THƯỜNG của mục III (không gồm ô "Bình thường").</summary>
        private CheckEdit[] AbnormalNutritionSignChecks()
        {
            return new CheckEdit[] {
                this.chkIsNutritionalEdema8, this.chkIsAnemiaSign8, this.chkIsRicketsSign8,
                this.chkIsMalnutrition8, this.chkIsOverweight8 };
        }

        /// <summary>Gắn sự kiện loại trừ lẫn nhau cho ô "Bình thường" và 5 ô bất thường. Gọi ở FillDataPageUnderSix.</summary>
        private void InitNormalNutritionSign()
        {
            try
            {
                if (this.chkIsNormalNutritionSign8 != null)
                {
                    this.chkIsNormalNutritionSign8.CheckedChanged -= chkIsNormalNutritionSign8_CheckedChanged;
                    this.chkIsNormalNutritionSign8.CheckedChanged += chkIsNormalNutritionSign8_CheckedChanged;
                }
                foreach (var chk in AbnormalNutritionSignChecks())
                {
                    if (chk == null) continue;
                    chk.CheckedChanged -= chkAbnormalNutritionSign_CheckedChanged;
                    chk.CheckedChanged += chkAbnormalNutritionSign_CheckedChanged;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Tích "Bình thường" → bỏ tích cả 5 ô dấu hiệu bất thường.</summary>
        private void chkIsNormalNutritionSign8_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isSyncingNutritionSign) return;
                if (this.chkIsNormalNutritionSign8 == null || !this.chkIsNormalNutritionSign8.Checked) return;
                isSyncingNutritionSign = true;
                try
                {
                    foreach (var chk in AbnormalNutritionSignChecks())
                        if (chk != null) chk.Checked = false;
                }
                finally { isSyncingNutritionSign = false; }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Tích bất kỳ ô dấu hiệu bất thường nào → bỏ tích "Bình thường".</summary>
        private void chkAbnormalNutritionSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isSyncingNutritionSign) return;
                CheckEdit src = sender as CheckEdit;
                if (src == null || !src.Checked) return;
                if (this.chkIsNormalNutritionSign8 == null || !this.chkIsNormalNutritionSign8.Checked) return;
                isSyncingNutritionSign = true;
                try { this.chkIsNormalNutritionSign8.Checked = false; }
                finally { isSyncingNutritionSign = false; }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Suy ô "Bình thường" từ 5 cờ đã nạp: cả 5 đều không tích → tích "Bình thường".
        /// Chỉ gọi khi ĐÃ CÓ bản ghi HIS_KSK_UNDER_SIX; bản ghi mới thì để trống hết.
        /// </summary>
        private void SyncNormalNutritionSignFromFlags()
        {
            try
            {
                if (this.chkIsNormalNutritionSign8 == null) return;
                bool hasAbnormal = false;
                foreach (var chk in AbnormalNutritionSignChecks())
                    if (chk != null && chk.Checked) { hasAbnormal = true; break; }

                isSyncingNutritionSign = true;
                try { this.chkIsNormalNutritionSign8.Checked = !hasAbnormal; }
                finally { isSyncingNutritionSign = false; }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Bỏ tích ô "Bình thường" (dùng khi tab chưa có bản ghi — để trắng toàn bộ).</summary>
        private void ClearNormalNutritionSign()
        {
            try
            {
                if (this.chkIsNormalNutritionSign8 == null) return;
                isSyncingNutritionSign = true;
                try { this.chkIsNormalNutritionSign8.Checked = false; }
                finally { isSyncingNutritionSign = false; }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
