/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Nút "Thư viện văn bản" phần Mắt tab "Ksk trên 18 tuổi" (btnTextLibEye2 — khai báo trong Designer,
 * cạnh label "4. Mắt", nhái btnTextLibCirculation2 của phần Tuần hoàn). Bấm -> mở plugin
 * HIS.Desktop.Plugins.TextLibrary (hashtag "KhamMat"). Callback (ProcessDataTextLib case 3) nhận
 * nội dung mẫu, CẮT theo dạng "ô:giá trị;ô:giá trị" rồi điền vào các ô tương ứng của phần Mắt.
 *
 * >>> QUY ƯỚC CHUỖI MẪU (đặt trong Thư viện văn bản, hashtag KhamMat) <<<
 *   TLP:<thị lực phải không kính>;TLT:<trái không kính>;TLPK:<phải có kính>;TLTK:<trái có kính>;BENH:<bệnh về mắt>
 *   VD:  TLP:10/10;TLT:10/10;TLPK:10/10;TLTK:10/10;BENH:Bình thường
 *   - Thiếu khóa nào thì ô đó giữ nguyên (không xóa). Khóa không hợp lệ -> bỏ qua.
 *   - Nếu chuỗi KHÔNG có dấu ':' -> coi là text thường, đổ vào ô "Bệnh về mắt".
 */
using System;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Bấm nút Thư viện phần Mắt -> mở Thư viện văn bản (hashtag KhamMat). keyTextLib=3.</summary>
        private void OpenTextLibEye()
        {
            keyTextLib = 3;
            textLibTargetClassify = cboExamEyeRank2;   // "PL:Lx" trong mẫu -> tự điền ô Phân loại Mắt
            OpenModuleTextLibrary(string.Empty, "KhamMat");
        }

        /// <summary>
        /// Cắt chuỗi mẫu "ô:giá trị;ô:giá trị" và điền vào các ô phần Mắt tab ≥18.
        /// Gọi từ ProcessDataTextLib (case 3).
        /// </summary>
        private void FillEyeFieldsFromLibText(string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content)) return;

                // Không theo định dạng khóa -> đổ nguyên vào ô Bệnh về mắt.
                if (content.IndexOf(':') < 0)
                {
                    if (this.txtExamEyeDisease2 != null) this.txtExamEyeDisease2.Text = content.Trim();
                    return;
                }

                string[] parts = content.Split(';');
                foreach (var raw in parts)
                {
                    if (string.IsNullOrWhiteSpace(raw)) continue;
                    int idx = raw.IndexOf(':');
                    if (idx <= 0) continue;
                    string key = raw.Substring(0, idx).Trim().ToUpperInvariant();
                    string val = raw.Substring(idx + 1).Trim();
                    switch (key)
                    {
                        case "TLP":  // thị lực phải (không kính)
                            if (this.txtExamEyeSightRight2 != null) this.txtExamEyeSightRight2.Text = val;
                            break;
                        case "TLT":  // thị lực trái (không kính)
                            if (this.txtExamEyeSightLeft2 != null) this.txtExamEyeSightLeft2.Text = val;
                            break;
                        case "TLPK": // thị lực phải (có kính)
                            if (this.txtExamEyeSightGlassRight2 != null) this.txtExamEyeSightGlassRight2.Text = val;
                            break;
                        case "TLTK": // thị lực trái (có kính)
                            if (this.txtExamEyeSightGlassLeft2 != null) this.txtExamEyeSightGlassLeft2.Text = val;
                            break;
                        case "BENH": // bệnh về mắt
                        case "MAT":
                            if (this.txtExamEyeDisease2 != null) this.txtExamEyeDisease2.Text = val;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        // ================== TAI MŨI HỌNG (ENT) ==================
        // Khóa: TP=tai phải(nói thường), TT=tai trái(nói thường), TPT=tai phải(nói thầm),
        //       TTT=tai trái(nói thầm), BENH/TMH=bệnh tai mũi họng.
        // VD: TP:5/5;TT:5/5;TPT:0.5/5;TTT:0.5/5;BENH:Bình thường

        /// <summary>Bấm nút Thư viện phần Tai mũi họng -> mở Thư viện văn bản (hashtag KhamTaiMuiHong). keyTextLib=4.</summary>
        private void OpenTextLibEnt()
        {
            keyTextLib = 4;
            textLibTargetClassify = cboExamEntDiseaseRank2;   // "PL:Lx" -> tự điền ô Phân loại Tai mũi họng
            OpenModuleTextLibrary(string.Empty, "KhamTaiMuiHong");
        }

        /// <summary>Cắt chuỗi "ô:giá trị;..." và điền vào các ô phần Tai mũi họng tab ≥18.</summary>
        private void FillEntFieldsFromLibText(string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content)) return;
                if (content.IndexOf(':') < 0)
                {
                    if (this.txtExamEntDisease2 != null) this.txtExamEntDisease2.Text = content.Trim();
                    return;
                }
                foreach (var raw in content.Split(';'))
                {
                    if (string.IsNullOrWhiteSpace(raw)) continue;
                    int idx = raw.IndexOf(':');
                    if (idx <= 0) continue;
                    string key = raw.Substring(0, idx).Trim().ToUpperInvariant();
                    string val = raw.Substring(idx + 1).Trim();
                    switch (key)
                    {
                        case "TP":  // tai phải (nói thường)
                            if (this.txtExamEntRightNomal2 != null) this.txtExamEntRightNomal2.Text = val;
                            break;
                        case "TT":  // tai trái (nói thường)
                            if (this.txtExamEntLeftNormal2 != null) this.txtExamEntLeftNormal2.Text = val;
                            break;
                        case "TPT": // tai phải (nói thầm)
                            if (this.txtExamEntRightWhisper2 != null) this.txtExamEntRightWhisper2.Text = val;
                            break;
                        case "TTT": // tai trái (nói thầm)
                            if (this.txtExamEntLeftWhisper2 != null) this.txtExamEntLeftWhisper2.Text = val;
                            break;
                        case "BENH":
                        case "TMH":
                            if (this.txtExamEntDisease2 != null) this.txtExamEntDisease2.Text = val;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        // ================== RĂNG HÀM MẶT (Stomatology) ==================
        // Khóa: HT=hàm trên, HD=hàm dưới, BENH/RHM=bệnh răng hàm mặt.
        // VD: HT:Bình thường;HD:Bình thường;BENH:Không

        /// <summary>Bấm nút Thư viện phần Răng hàm mặt -> mở Thư viện văn bản (hashtag KhamRangHamMat). keyTextLib=5.</summary>
        private void OpenTextLibStomatology()
        {
            keyTextLib = 5;
            textLibTargetClassify = cboExamStomatologyRank2;   // "PL:Lx" -> tự điền ô Phân loại Răng hàm mặt
            OpenModuleTextLibrary(string.Empty, "KhamRangHamMat");
        }

        /// <summary>Cắt chuỗi "ô:giá trị;..." và điền vào các ô phần Răng hàm mặt tab ≥18.</summary>
        private void FillStomatologyFieldsFromLibText(string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content)) return;
                if (content.IndexOf(':') < 0)
                {
                    if (this.txtExamStomatologyDisease2 != null) this.txtExamStomatologyDisease2.Text = content.Trim();
                    return;
                }
                foreach (var raw in content.Split(';'))
                {
                    if (string.IsNullOrWhiteSpace(raw)) continue;
                    int idx = raw.IndexOf(':');
                    if (idx <= 0) continue;
                    string key = raw.Substring(0, idx).Trim().ToUpperInvariant();
                    string val = raw.Substring(idx + 1).Trim();
                    switch (key)
                    {
                        case "HT":  // hàm trên
                            if (this.txtExamStomatologyUpper2 != null) this.txtExamStomatologyUpper2.Text = val;
                            break;
                        case "HD":  // hàm dưới
                            if (this.txtExamStomatologyLower2 != null) this.txtExamStomatologyLower2.Text = val;
                            break;
                        case "BENH":
                        case "RHM":
                            if (this.txtExamStomatologyDisease2 != null) this.txtExamStomatologyDisease2.Text = val;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
    }
}
