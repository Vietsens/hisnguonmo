/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tab "Ksk dưới 18 tuổi" — mục Kết luận, hai nút cạnh ô "Sức khỏe" (nút dưới nằm dưới nút trên):
 *   btnChooseRs3      — chọn kết quả khám lâm sàng đã nhập ở tab con "Khám lâm sàng";
 *   btnTextLibHealth3 — Thư viện mẫu (plugin HIS.Desktop.Plugins.TextLibrary).
 * Cả hai đều điền nội dung vào txtNormalHealth3.
 *
 * Dựng theo đúng khuôn nút btnChooseRs của tab "Ksk trên 18 tuổi" (xem ___OverEighteen.cs,
 * vùng "Chon ket qua kham lam sang -> Benh tat"): gom các vùng khám ĐÃ CÓ nội dung ở tab con
 * "Khám lâm sàng", mở frmChooseExamResult để tích chọn, rồi đổ nội dung đã chọn vào ô đích.
 *
 * KHÁC tab ≥18 đúng hai chỗ:
 *   - ô nguồn là bộ control hậu tố "3" (tab dưới 18 tuổi);
 *   - ô đích là "Sức khỏe" (txtNormalHealth3), không phải "Bệnh tật".
 *
 * Dùng lại frmChooseExamResult và AddExamResultRow của tab ≥18 — không nhân bản form/hàm.
 * Ảnh nút khai báo trong Designer + resx (btnChooseRs3.Image), KHÔNG gán lúc chạy.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>
        /// Hashtag của mẫu văn bản cho ô "Sức khỏe" mục Kết luận (tab dưới 18 tuổi).
        /// Phải khai ĐÚNG chuỗi này ở danh mục Thư viện văn bản, không thì danh sách mẫu sẽ trống.
        /// </summary>
        private const string HASHTAG__KET_LUAN_SUC_KHOE = "KetLuanSucKhoe";

        /// <summary>
        /// Mở danh sách các vùng khám lâm sàng của tab dưới 18 tuổi có nội dung; tích chọn dòng nào
        /// thì nội dung Kết quả của dòng đó được đổ vào ô "Sức khỏe" của mục Kết luận.
        /// </summary>
        private void btnChooseRs3_Click(object sender, EventArgs e)
        {
            try
            {
                // Thứ tự liệt kê theo đúng thứ tự các mục trên tab con "Khám lâm sàng".
                var list = new List<KskExamResultADO>();
                AddExamResultRow(list, "Tuần hoàn", txtExamCirculation3);
                AddExamResultRow(list, "Hô hấp", txtExamRespiratory3);
                AddExamResultRow(list, "Tiêu hóa", txtExamDigestion3);
                AddExamResultRow(list, "Thận - Tiết niệu", txtExamKidneyUrology3);
                AddExamResultRow(list, "Thần kinh", txtExamNeuroMental3);
                AddExamResultRow(list, "Tâm thần", txtExamMental3);
                AddExamResultRow(list, "Lâm sàng khác", txtExamClinicalOther3);
                AddExamResultRow(list, "Mắt (bệnh về mắt)", txtExamEyeDisease3);
                AddExamResultRow(list, "Tai mũi họng (bệnh)", txtExamEntDisease3);
                AddExamResultRow(list, "Răng hàm mặt (bệnh)", txtExamStomatologyDisease3);

                if (list.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Chưa có nội dung kết quả khám nào ở tab Khám lâm sàng để chọn.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var frm = new frmChooseExamResult(list))
                {
                    if (frm.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(frm.SelectedText)
                        && txtNormalHealth3 != null)
                    {
                        txtNormalHealth3.Text = frm.SelectedText;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>
        /// Nút Thư viện mẫu của ô "Sức khỏe" — mở plugin Thư viện văn bản theo hashtag riêng của
        /// mục Kết luận, mẫu chọn xong được điền thẳng vào ô.
        ///
        /// Dùng lại OpenTextLibExamResult của tab ≥18 (keyTextLib = 2, đổ về textLibTargetEdit) —
        /// tham số Phân loại truyền null vì ô Phân loại của mục Kết luận không đi theo mẫu này
        /// (token "PL:Lx" nếu mẫu có vẫn bị cắt, không lọt vào ô nội dung).
        /// </summary>
        private void btnTextLibHealth3_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNormalHealth3 == null) return;
                OpenTextLibExamResult(txtNormalHealth3, HASHTAG__KET_LUAN_SUC_KHOE, null);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }
    }
}
