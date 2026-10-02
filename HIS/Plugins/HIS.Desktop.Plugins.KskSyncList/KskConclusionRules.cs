/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using His.Ksk.QD2062.Base;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Mục KẾT LUẬN bắt buộc trước khi ĐẨY CỔNG — cùng luật với nút "Kết thúc khám" của màn Nhập thông tin KSK
    /// (EnterKskInfomantionVer2: ValidateConclusionBeforeFinish / GetSavedConclusionState):
    ///  - Hồ sơ KSK từ 18 tuổi:   Phân loại (HIS_KSK_OVER_EIGHTEEN.HEALTH_EXAM_RANK_ID) + Người khám.
    ///  - Hồ sơ KSK 6 - dưới 18:  Phân loại (HIS_KSK_UNDER_EIGHTEEN.HEALTH_EXAM_RANK_ID) + Người khám.
    ///  - Hồ sơ trẻ dưới 6 tuổi:  Xếp loại sức khỏe chung (HIS_KSK_GENERAL.HEALTH_EXAM_RANK_ID) + Bác sĩ khám
    ///                            + Kết luận về sức khỏe (HIS_KSK_GENERAL.HEALTH_CONCLUSION_TYPE).
    ///  - Người khám / Bác sĩ khám của cả 3 loại = HIS_KSK_GENERAL.CONCLUDER_LOGINNAME (cột "Người kết luận").
    ///  - Loại khác (định kỳ, lái xe, nghề nghiệp, KSK khác...): không có quy tắc này — giữ nguyên như cũ.
    ///
    /// Vì sao: từ 25/09/2026 bấm Lưu ở màn nhập KSK không còn bắt mục kết luận (chỉ bắt khi Kết thúc khám), còn
    /// danh sách đồng bộ (V_HIS_KSK_SYNC) lấy mọi hồ sơ đã có thời gian kết luận -> bác sĩ Lưu mà chưa Kết thúc
    /// khám thì hồ sơ thiếu Phân loại vẫn đẩy cổng thành công (BV Nguyễn Đình Chiểu báo 02/10/2026).
    ///
    /// Đọc cột bằng reflection: viện chạy MOS.EFMODEL cũ thiếu cột nào thì bỏ qua đúng điều kiện đó (không chặn
    /// nhầm, không lỗi nạp kiểu làm hỏng cả lô đồng bộ).
    /// </summary>
    internal static class KskConclusionRules
    {
        /// <summary>Loại hồ sơ áp quy tắc mục kết luận (tương ứng 3 tab của màn nhập KSK).</summary>
        internal enum ConclusionRule
        {
            /// <summary>Không áp quy tắc (định kỳ, lái xe, nghề nghiệp, KSK khác).</summary>
            None = 0,
            /// <summary>KSK từ 18 tuổi trở lên — bản ghi HIS_KSK_OVER_EIGHTEEN.</summary>
            OverEighteen = 1,
            /// <summary>KSK 6 - dưới 18 tuổi — bản ghi HIS_KSK_UNDER_EIGHTEEN.</summary>
            UnderEighteen = 2,
            /// <summary>KSK trẻ em dưới 6 tuổi — bản ghi HIS_KSK_UNDER_SIX (mục kết luận lưu ở HIS_KSK_GENERAL).</summary>
            UnderSix = 3
        }

        private const string COL_RANK = "HEALTH_EXAM_RANK_ID";
        private const string COL_HEALTH_CONCLUSION = "HEALTH_CONCLUSION_TYPE";
        private const string COL_CONCLUDER = "CONCLUDER_LOGINNAME";

        /// <summary>
        /// Trả null khi được đẩy (đủ mục kết luận, hoặc loại hồ sơ không có quy tắc này); ngược lại trả lý do
        /// (lưu SYNC_FAILD_REASON + hiện ở hộp kết quả) để KHÔNG đẩy hồ sơ lên cổng nào.
        /// </summary>
        internal static string Validate(V_HIS_KSK_SYNC row, Qd1551KskInput input)
        {
            try
            {
                if (input == null) return null;   // khong dung duoc du lieu -> da co loi rieng
                ConclusionRule rule = ResolveRule(input);
                if (rule == ConclusionRule.None) return null;

                bool underSix = rule == ConclusionRule.UnderSix;
                var missing = new List<string>();

                bool? rank;
                if (rule == ConclusionRule.OverEighteen)
                    rank = HasValue(typeof(HIS_KSK_OVER_EIGHTEEN), input.OverEighteen, COL_RANK);
                else if (rule == ConclusionRule.UnderEighteen)
                    rank = HasValue(typeof(HIS_KSK_UNDER_EIGHTEEN), input.UnderEighteen, COL_RANK);
                else
                    rank = HasValue(typeof(HIS_KSK_GENERAL), input.General, COL_RANK);
                if (rank == false)
                    missing.Add(underSix ? "Xếp loại tình trạng sức khỏe chung" : "Phân loại sức khỏe");

                // Nguoi ket luan: ban ghi HIS_KSK_GENERAL vua tai (giong man nhap KSK), du phong cot cua dong luoi
                // (V_HIS_KSK_SYNC doc cung cot do).
                bool? concluder = HasValue(typeof(HIS_KSK_GENERAL), input.General, COL_CONCLUDER);
                if (concluder == false && HasValue(typeof(V_HIS_KSK_SYNC), row, COL_CONCLUDER) == true)
                    concluder = true;
                if (concluder == false)
                    missing.Add(underSix ? "Bác sĩ khám (người kết luận)" : "Người khám (người kết luận)");

                if (underSix && HasValue(typeof(HIS_KSK_GENERAL), input.General, COL_HEALTH_CONCLUSION) == false)
                    missing.Add("Kết luận về sức khỏe");

                if (missing.Count == 0) return null;
                return "KHÔNG đẩy hồ sơ — chưa đủ mục kết luận: thiếu " + string.Join(", ", missing.ToArray())
                    + ". Nhập đủ mục Kết luận ở màn Nhập thông tin KSK, bấm Lưu rồi đồng bộ lại.";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("KskConclusionRules.Validate: bo qua (van day): " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Loại hồ sơ theo mẫu phiếu của dòng đồng bộ (KSK_TYPE_ID -> FormType) và BẢN GHI có thật — giống
        /// ResolveConclusionRuleTab của màn nhập KSK: mẫu nào có bản ghi riêng thì theo mẫu đó; mẫu không khớp bản
        /// ghi (KSK_TYPE_ID trống / dữ liệu cũ) thì theo bản ghi có thật (từ 18 -> dưới 18 -> dưới 6).
        /// Mẫu lái xe không áp (phiếu lái xe có kết luận riêng, màn nhập KSK cũng không bắt).
        /// </summary>
        internal static ConclusionRule ResolveRule(Qd1551KskInput input)
        {
            if (input == null) return ConclusionRule.None;
            FormType form = input.FormType;
            if (form == FormType.LaiXe) return ConclusionRule.None;

            bool overEighteen = input.OverEighteen != null;
            bool underEighteen = input.UnderEighteen != null;
            bool underSix = input.UnderSix != null;
            if (form == FormType.Tren18 && overEighteen) return ConclusionRule.OverEighteen;
            if (form == FormType.Duoi18 && underEighteen) return ConclusionRule.UnderEighteen;
            if (IsUnderSixForm(form) && underSix) return ConclusionRule.UnderSix;
            if (overEighteen) return ConclusionRule.OverEighteen;
            if (underEighteen) return ConclusionRule.UnderEighteen;
            if (underSix) return ConclusionRule.UnderSix;
            return ConclusionRule.None;
        }

        /// <summary>Mẫu phiếu trẻ dưới 6 tuổi (Phụ lục 01 QĐ 1551: mẫu 6..13 theo mốc tháng tuổi).</summary>
        private static bool IsUnderSixForm(FormType form)
        {
            return form >= FormType.Tre0_2Thang && form <= FormType.Tre2_6Tuoi;
        }

        /// <summary>
        /// Cột của bản ghi đã có giá trị chưa: true = có; false = trống hoặc không có bản ghi; null = MOS.EFMODEL
        /// của viện không có cột này (bỏ qua điều kiện, không chặn nhầm).
        /// </summary>
        private static bool? HasValue(Type entityType, object entity, string column)
        {
            PropertyInfo p = entityType.GetProperty(column);
            if (p == null) return null;
            if (entity == null) return false;
            object v = p.GetValue(entity, null);
            if (v == null) return false;
            string s = v as string;
            return s == null || !string.IsNullOrWhiteSpace(s);
        }
    }
}
