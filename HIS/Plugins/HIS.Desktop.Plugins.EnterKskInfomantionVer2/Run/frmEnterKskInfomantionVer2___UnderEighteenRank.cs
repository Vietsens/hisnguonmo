/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Ô "Phân loại" ở mục V. KẾT LUẬN của tab Khám sức khỏe dưới 18 tuổi.
 *
 * Lưu vào HIS_KSK_UNDER_EIGHTEEN.HEALTH_EXAM_RANK_ID — cột RIÊNG của tab này, không dùng chung
 * với ô Phân loại của tab Thông tin chung (cột đó thuộc HIS_KSK_GENERAL). Nên việc lưu và nạp làm
 * trực tiếp như mọi ô khác của tab, xem GetValueUnderEighteen / FillDataUnderEighteen.
 *
 * GIAO DIỆN nằm trong Designer (cboHealthExamRank3 + lciHealthExamRank3 thuộc sub-tab "Kết luận"),
 * KHÔNG dựng lúc chạy. File này chỉ còn phần dữ liệu: đổ danh mục, nạp và lấy giá trị.
 */
using System;
using System.Linq;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private bool rank3Inited;

        /// <summary>
        /// Đổ danh mục cho ô Phân loại của mục V. KẾT LUẬN. Gọi được nhiều lần, chỉ chạy một lượt.
        ///
        /// Dùng ĐÚNG helper SetDataCboRank như ~78 ô Phân loại còn lại (tab Thông tin chung dòng 88,
        /// tab trên 18 tuổi dòng 236…): tự dựng cột HEALTH_EXAM_RANK_CODE + HEALTH_EXAM_RANK_NAME,
        /// DisplayMember = HEALTH_EXAM_RANK_NAME, ValueMember = ID.
        ///
        /// TRƯỚC ĐÂY dùng Properties.Assign(cboHealthExamRank.Properties) để clone cấu hình từ ô
        /// Phân loại của tab Thông tin chung — SAI: ô đó chỉ được SetDataCboRank khi
        /// FillDataPageGenaral() chạy (tức khi tab 0 được fill), mà ResolveDefaultTab ưu tiên tab có
        /// bảng riêng nên tab 0 gần như không bao giờ fill → clone về một RepositoryItem CHƯA cấu
        /// hình (không cột, không ValueMember) → ô Phân loại tab dưới 18 hỏng.
        /// </summary>
        private void BuildKskRank3()
        {
            try
            {
                if (rank3Inited) return;
                if (this.cboHealthExamRank3 == null) return;
                rank3Inited = true;

                SetDataCboRank(this.cboHealthExamRank3);

                // Hồ sơ có thể đã nạp TRƯỚC khi danh mục được đổ -> đổ lại giá trị.
                FillKskRank3();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổ giá trị đã lưu vào ô Phân loại. Gọi sau khi nạp hồ sơ của tab.</summary>
        private void FillKskRank3()
        {
            try
            {
                if (cboHealthExamRank3 == null) return;
                cboHealthExamRank3.EditValue = (currentKskUnderEight != null)
                    ? currentKskUnderEight.HEALTH_EXAM_RANK_ID : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Mã phân loại đang chọn — dùng khi lưu. Chưa đổ danh mục thì giữ nguyên giá trị cũ.</summary>
        private long? GetKskRank3Value()
        {
            try
            {
                if (cboHealthExamRank3 == null || !rank3Inited)
                {
                    // Người dùng chưa mở tab nên ô chưa được đổ danh mục: KHÔNG trả null, vì lượt lưu
                    // sẽ ghi đè mất giá trị đang có trong cơ sở dữ liệu.
                    return (currentKskUnderEight != null) ? currentKskUnderEight.HEALTH_EXAM_RANK_ID : null;
                }
                if (cboHealthExamRank3.EditValue == null) return null;
                long v;
                return long.TryParse(cboHealthExamRank3.EditValue.ToString(), out v) ? (long?)v : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }
    }
}
