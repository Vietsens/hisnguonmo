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
using System.Collections.Generic;
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

        // ================== NỘI KHOA (JSON — điền nhiều vùng cùng lúc) ==================
        // Nút "Thư viện mẫu" cạnh "1. Nội khoa" (tab ≥18, khám lâm sàng). Mẫu là JSON gồm 8 chuyên khoa,
        // mỗi chuyên khoa có "kq" (kết quả) + "pl" (phân loại 1..5 hoặc L1..L5). keyTextLib=6.
        //
        // >>> CÚ PHÁP MẪU (hashtag KhamNoiKhoa) — mỗi chuyên khoa là chuỗi "KQ:xxxx;PL:Lx" (x=1..5) <<<
        // {
        //   "tuanHoan":"KQ:Bình thường;PL:L1", "hoHap":"KQ:Bình thường;PL:L1",
        //   "tieuHoa":"KQ:...;PL:L1", "thanTietNieu":"KQ:...;PL:L1",
        //   "noiTiet":"KQ:...;PL:L1", "coXuongKhop":"KQ:...;PL:L1",
        //   "thanKinh":"KQ:...;PL:L1", "tamThan":"KQ:...;PL:L1"
        // }
        // Thiếu chuyên khoa nào -> bỏ qua chuyên khoa đó (không ghi đè).
        private void OpenTextLibInternal()
        {
            keyTextLib = 6;
            OpenModuleTextLibrary(string.Empty, "KhamNoiKhoa");
        }

        /// <summary>Nút Thư viện mẫu cạnh "1. Nội khoa".</summary>
        private void btnTextLibInternal2_Click(object sender, EventArgs e)
        {
            OpenTextLibInternal();
        }

        /// <summary>Parse JSON mẫu Nội khoa -> điền kết quả + phân loại cho 8 chuyên khoa nội.</summary>
        private void FillInternalFromJson(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return;

                // Chuẩn hóa dấu nháy CONG (copy từ Word/chat: “ ” ‘ ’) -> nháy thẳng để JSON parse được.
                json = json.Replace('“', '"').Replace('”', '"')
                           .Replace('‘', '\'').Replace('’', '\'');

                Newtonsoft.Json.Linq.JObject root;
                try { root = Newtonsoft.Json.Linq.JObject.Parse(json); }
                catch (Exception exParse)
                {
                    LogSystem.Warn(exParse);
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Nội dung mẫu không đúng định dạng JSON. Kiểm tra lại dấu nháy/kết cấu.",
                        "Thông báo", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }

                // Lookup key KHÔNG phân biệt hoa/thường (mẫu có thể ghi "tuanhoan" hoặc "tuanHoan").
                var map = new Dictionary<string, Newtonsoft.Json.Linq.JToken>(StringComparer.OrdinalIgnoreCase);
                foreach (var p in root.Properties()) map[(p.Name ?? "").Trim()] = p.Value;

                ApplyInternalSection(map, "tuanHoan",     txtExamCirculation2,    cboExamCirculationRank2);
                ApplyInternalSection(map, "hoHap",        txtExamRespiratory2,    cboExamRespiratoryRank2);
                ApplyInternalSection(map, "tieuHoa",      txtExamDigestion2,      cboExamDigestionRank2);
                ApplyInternalSection(map, "thanTietNieu", txtExamKidneyUrology2,  cboExamKidneyUrologyRank2);
                ApplyInternalSection(map, "noiTiet",      txtExamOend2,           cboExamOend2);
                ApplyInternalSection(map, "coXuongKhop",  txtExamMuscleBone2,     cboExamMuscleBoneRank2);
                ApplyInternalSection(map, "thanKinh",     txtExamNeurological2,   cboExamNeurologicalRank2);
                ApplyInternalSection(map, "tamThan",      txtExamMental2,         cboExamMentalRank2);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Điền 1 chuyên khoa: giá trị chuyên khoa là chuỗi dạng "KQ:xxxx;PL:Lx" (x=1..5).
        /// -> KQ: phần kết quả (điền vào ô); PL:Lx -> phân loại (SetClassifyByLevel).
        /// </summary>
        private void ApplyInternalSection(Dictionary<string, Newtonsoft.Json.Linq.JToken> map, string key,
            DevExpress.XtraEditors.BaseEdit target, DevExpress.XtraEditors.GridLookUpEdit classify)
        {
            try
            {
                if (map == null) return;
                Newtonsoft.Json.Linq.JToken tok;
                if (!map.TryGetValue(key, out tok) || tok == null || tok.Type == Newtonsoft.Json.Linq.JTokenType.Null) return;

                string s = tok.ToString();
                int lv = ExtractPlLevel(ref s);   // tách "PL:Lx" (còn lại "KQ:xxxx")
                if (lv > 0 && classify != null) SetClassifyByLevel(classify, lv);

                // Bỏ tiền tố "KQ:" -> phần kết quả.
                s = System.Text.RegularExpressions.Regex.Replace(s, @"^\s*KQ\s*:\s*", "",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                if (target != null) target.Text = s;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
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
