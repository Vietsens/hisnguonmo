/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tab "Ksk dưới 18 tuổi" — nút Thư viện văn bản cho từng mục khám lâm sàng và nút "+" chọn
 * kết quả cận lâm sàng, dựng theo đúng khuôn tab "Ksk trên 18 tuổi" (xem ___EyeTemplate.cs
 * và ___OverEighteen.cs).
 *
 * Ô kết quả đơn (7 mục nội khoa) dùng thẳng OpenTextLibExamResult của tab ≥18 (keyTextLib = 2,
 * đổ về textLibTargetEdit) — không phải sửa gì bên đó.
 *
 * 4 mẫu điền NHIỀU ô (Mắt / Tai mũi họng / Răng hàm mặt / Nội khoa JSON) cần callback riêng vì
 * ô đích là bộ control hậu tố "3" -> mở Thư viện văn bản bằng OpenModuleTextLibraryUnderEighteen
 * với delegate ProcessDataTextLibUnderEighteen, KHÔNG đụng vào ProcessDataTextLib của tab ≥18.
 *
 * Ảnh nút khai báo TRONG DESIGNER (btnTextLibXxx3.Image trong frmEnterKskInfomantionVer2.resx),
 * KHÔNG gán lúc chạy. Hàm dưới đây chỉ còn gắn tooltip cú pháp mẫu (giống tab ≥18).
 */
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private bool underEighteenTextLibInited = false;

        #region Tooltip nút

        /// <summary>Gắn tooltip cú pháp mẫu cho các nút Thư viện văn bản của tab dưới 18 tuổi. Idempotent.</summary>
        private void InitUnderEighteenTextLibButtons()
        {
            try
            {
                if (underEighteenTextLibInited) return;
                underEighteenTextLibInited = true;

                // Tab dưới 18 tuổi KHÔNG còn ô Phân loại -> cú pháp mẫu không có token "PL:Lx".
                // (Mẫu cũ lỡ còn "PL:Lx" vẫn dùng được: token bị cắt bỏ, không lọt vào ô nội dung.)
                string tipSingle = "Thư viện mẫu\r\nMẫu là nội dung khám, điền thẳng vào ô Kết quả.\r\nVD: Bình thường";
                SetBtnToolTip(btnTextLibCirculation3, tipSingle);
                SetBtnToolTip(btnTextLibRespiratory3, tipSingle);
                SetBtnToolTip(btnTextLibDigestion3, tipSingle);
                SetBtnToolTip(btnTextLibKidneyUrology3, tipSingle);
                SetBtnToolTip(btnTextLibNeuroMental3, tipSingle);
                SetBtnToolTip(btnTextLibMental3, tipSingle);
                SetBtnToolTip(btnTextLibClinicalOther3, tipSingle);

                SetBtnToolTip(btnTextLibEye3,
                    "Thư viện mẫu\r\nCú pháp mẫu: TLP:..;TLT:..;TLPK:..;TLTK:..;BENH:..\r\nVD: TLP:10/10;TLT:10/10;BENH:Bình thường");
                SetBtnToolTip(btnTextLibEnt3,
                    "Thư viện mẫu\r\nCú pháp mẫu: TP:..;TT:..;TPT:..;TTT:..;BENH:..\r\nVD: TP:5/5;TT:5/5;BENH:Bình thường");
                SetBtnToolTip(btnTextLibStomatology3,
                    "Thư viện mẫu\r\nCú pháp mẫu: HT:..;HD:..;BENH:..\r\nVD: HT:Bình thường;HD:Bình thường");
                SetBtnToolTip(btnTextLibInternal3,
                    "Thư viện mẫu Nội khoa (JSON)\r\nMỗi chuyên khoa: \"KQ:kết quả\"\r\n"
                    + "VD: {\"tuanHoan\":\"KQ:Bình thường\",\"hoHap\":\"KQ:Bình thường\",\r\n"
                    + " \"tieuHoa\":\"KQ:...\",\"thanTietNieu\":\"KQ:...\",\r\n"
                    + " \"thanKinh\":\"KQ:...\",\"tamThan\":\"KQ:...\",\"lamSangKhac\":\"KQ:...\"}");
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region Sự kiện nút

        /// <summary>Nút Thư viện văn bản của các mục khám lâm sàng tab dưới 18 tuổi.</summary>
        private void btnTextLibExamResultUnderEighteen_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender == btnTextLibInternal3) OpenTextLibInternalUnderEighteen();
                else if (sender == btnTextLibEye3) OpenTextLibEyeUnderEighteen();
                else if (sender == btnTextLibEnt3) OpenTextLibEntUnderEighteen();
                else if (sender == btnTextLibStomatology3) OpenTextLibStomatologyUnderEighteen();
                // Tham số Phân loại truyền null: tab dưới 18 tuổi đã bỏ ô Phân loại nên mẫu
                // KHÔNG được tự điền phân loại nữa (token "PL:Lx" nếu có vẫn bị cắt khỏi nội dung).
                else if (sender == btnTextLibCirculation3)
                    OpenTextLibExamResult(txtExamCirculation3, "KhamTuanHoan", null);
                else if (sender == btnTextLibRespiratory3)
                    OpenTextLibExamResult(txtExamRespiratory3, "KhamHoHap", null);
                else if (sender == btnTextLibDigestion3)
                    OpenTextLibExamResult(txtExamDigestion3, "KhamTieuHoa", null);
                else if (sender == btnTextLibKidneyUrology3)
                    OpenTextLibExamResult(txtExamKidneyUrology3, "KhamThanTietNieu", null);
                else if (sender == btnTextLibNeuroMental3)
                    OpenTextLibExamResult(txtExamNeuroMental3, "KhamThanKinh", null);
                else if (sender == btnTextLibMental3)
                    OpenTextLibExamResult(txtExamMental3, "KhamTamThan", null);
                else if (sender == btnTextLibClinicalOther3)
                    OpenTextLibExamResult(txtExamClinicalOther3, "KhamLamSangKhac", null);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Nút "+" chọn kết quả cận lâm sàng (thay nút Plus cũ của ô ButtonEdit).</summary>
        private void btnPickResultSubclinical3_Click(object sender, EventArgs e)
        {
            try
            {
                NameSItem = ENameSItem.KET_QUA_3;
                GetSpecInformation(ReturnObject = false);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region Mở Thư viện văn bản (các mẫu điền nhiều ô)

        /// <summary>Mẫu đang mở thuộc vùng nào (định tuyến nội dung trả về đúng nhóm ô).</summary>
        private enum KskUnderEighteenLibTarget { Eye, Ent, Stomatology, Internal }

        private KskUnderEighteenLibTarget underEighteenLibTarget = KskUnderEighteenLibTarget.Eye;

        // Không gán textLibTargetClassify: tab dưới 18 tuổi đã bỏ ô Phân loại nên mẫu chỉ điền nội dung.

        private void OpenTextLibEyeUnderEighteen()
        {
            underEighteenLibTarget = KskUnderEighteenLibTarget.Eye;
            OpenModuleTextLibraryUnderEighteen("KhamMat");
        }

        private void OpenTextLibEntUnderEighteen()
        {
            underEighteenLibTarget = KskUnderEighteenLibTarget.Ent;
            OpenModuleTextLibraryUnderEighteen("KhamTaiMuiHong");
        }

        private void OpenTextLibStomatologyUnderEighteen()
        {
            underEighteenLibTarget = KskUnderEighteenLibTarget.Stomatology;
            OpenModuleTextLibraryUnderEighteen("KhamRangHamMat");
        }

        private void OpenTextLibInternalUnderEighteen()
        {
            underEighteenLibTarget = KskUnderEighteenLibTarget.Internal;
            OpenModuleTextLibraryUnderEighteen("KhamNoiKhoa");
        }

        /// <summary>
        /// Mở plugin Thư viện văn bản với callback RIÊNG của tab dưới 18 tuổi
        /// (giống OpenModuleTextLibrary nhưng truyền ProcessDataTextLibUnderEighteen).
        /// </summary>
        private void OpenModuleTextLibraryUnderEighteen(string hashtag)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData =
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws
                        .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TextLibrary").FirstOrDefault();
                if (moduleData == null)
                {
                    LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TextLibrary");
                    return;
                }
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) return;

                List<object> listArgs = new List<object>();
                HIS.Desktop.ADO.TextLibraryInfoADO ado = new HIS.Desktop.ADO.TextLibraryInfoADO();
                ado.Content = string.Empty;
                ado.Hashtag = hashtag;
                listArgs.Add(ado);
                listArgs.Add((HIS.Desktop.Common.DelegateDataTextLib)ProcessDataTextLibUnderEighteen);

                var instance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(
                    HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(
                        moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                if (instance == null) throw new ArgumentNullException("moduleData is null");

                var tlForm = instance as System.Windows.Forms.Form;
                if (tlForm != null) tlForm.ShowDialog();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Callback nhận văn bản đã chọn — đổ về nhóm ô của tab dưới 18 tuổi.</summary>
        private void ProcessDataTextLibUnderEighteen(MOS.EFMODEL.DataModels.HIS_TEXT_LIB textLib)
        {
            try
            {
                if (textLib == null) return;
                string content = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                if (underEighteenLibTarget == KskUnderEighteenLibTarget.Internal)
                {
                    FillInternalFromJsonUnderEighteen(content);
                    return;
                }
                // Mẫu cũ có thể còn token "PL:Lx" -> CẮT BỎ (không điền phân loại nữa, cũng không
                // để lẫn vào ô nội dung). Bỏ qua bước này thì phần nội dung đứng trước token
                // sẽ bị vòng lặp "khóa:giá trị" phía dưới loại mất.
                ExtractPlLevel(ref content);
                switch (underEighteenLibTarget)
                {
                    case KskUnderEighteenLibTarget.Eye: FillEyeFieldsFromLibTextUnderEighteen(content); break;
                    case KskUnderEighteenLibTarget.Ent: FillEntFieldsFromLibTextUnderEighteen(content); break;
                    case KskUnderEighteenLibTarget.Stomatology: FillStomatologyFieldsFromLibTextUnderEighteen(content); break;
                    default: break;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region Đổ nội dung mẫu vào ô (bộ control hậu tố "3")

        /// <summary>Mắt: TLP/TLT = thị lực không kính, TLPK/TLTK = có kính, BENH = bệnh về mắt.</summary>
        private void FillEyeFieldsFromLibTextUnderEighteen(string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content)) return;
                if (content.IndexOf(':') < 0)
                {
                    if (txtExamEyeDisease3 != null) txtExamEyeDisease3.Text = content.Trim();
                    return;
                }
                foreach (var raw in content.Split(';'))
                {
                    string key, val;
                    if (!SplitLibToken(raw, out key, out val)) continue;
                    switch (key)
                    {
                        case "TLP": if (txtExamEyeSightRight3 != null) txtExamEyeSightRight3.Text = val; break;
                        case "TLT": if (txtExamEyeSightLeft3 != null) txtExamEyeSightLeft3.Text = val; break;
                        case "TLPK": if (txtExamEyeSightGlassRight3 != null) txtExamEyeSightGlassRight3.Text = val; break;
                        case "TLTK": if (txtExamEyeSightGlassLeft3 != null) txtExamEyeSightGlassLeft3.Text = val; break;
                        case "BENH":
                        case "MAT": if (txtExamEyeDisease3 != null) txtExamEyeDisease3.Text = val; break;
                        default: break;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Tai mũi họng: TP/TT = nói thường, TPT/TTT = nói thầm, BENH = bệnh TMH.</summary>
        private void FillEntFieldsFromLibTextUnderEighteen(string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content)) return;
                if (content.IndexOf(':') < 0)
                {
                    if (txtExamEntDisease3 != null) txtExamEntDisease3.Text = content.Trim();
                    return;
                }
                foreach (var raw in content.Split(';'))
                {
                    string key, val;
                    if (!SplitLibToken(raw, out key, out val)) continue;
                    switch (key)
                    {
                        case "TP": if (txtExamEntRightNomal3 != null) txtExamEntRightNomal3.Text = val; break;
                        case "TT": if (txtExamEntLeftNormal3 != null) txtExamEntLeftNormal3.Text = val; break;
                        case "TPT": if (txtExamEntRightWhisper3 != null) txtExamEntRightWhisper3.Text = val; break;
                        case "TTT": if (txtExamEntLeftWhisper3 != null) txtExamEntLeftWhisper3.Text = val; break;
                        case "BENH":
                        case "TMH": if (txtExamEntDisease3 != null) txtExamEntDisease3.Text = val; break;
                        default: break;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Răng hàm mặt: HT = hàm trên, HD = hàm dưới, BENH = bệnh RHM.</summary>
        private void FillStomatologyFieldsFromLibTextUnderEighteen(string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content)) return;
                if (content.IndexOf(':') < 0)
                {
                    if (txtExamStomatologyDisease3 != null) txtExamStomatologyDisease3.Text = content.Trim();
                    return;
                }
                foreach (var raw in content.Split(';'))
                {
                    string key, val;
                    if (!SplitLibToken(raw, out key, out val)) continue;
                    switch (key)
                    {
                        case "HT": if (txtExamStomatologyUpper3 != null) txtExamStomatologyUpper3.Text = val; break;
                        case "HD": if (txtExamStomatologyLower3 != null) txtExamStomatologyLower3.Text = val; break;
                        case "BENH":
                        case "RHM": if (txtExamStomatologyDisease3 != null) txtExamStomatologyDisease3.Text = val; break;
                        default: break;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Nội khoa (JSON) — điền kết quả + phân loại cho 7 mục nội khoa của tab dưới 18 tuổi.</summary>
        private void FillInternalFromJsonUnderEighteen(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return;
                json = json.Replace('“', '"').Replace('”', '"').Replace('‘', '\'').Replace('’', '\'');

                Newtonsoft.Json.Linq.JObject root;
                try { root = Newtonsoft.Json.Linq.JObject.Parse(json); }
                catch (Exception exParse)
                {
                    LogSystem.Warn(exParse);
                    XtraMessageBox.Show("Nội dung mẫu không đúng định dạng JSON. Kiểm tra lại dấu nháy/kết cấu.",
                        "Thông báo", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }

                var map = new Dictionary<string, Newtonsoft.Json.Linq.JToken>(StringComparer.OrdinalIgnoreCase);
                foreach (var p in root.Properties()) map[(p.Name ?? "").Trim()] = p.Value;

                // Tham số Phân loại truyền null — tab dưới 18 tuổi không còn ô Phân loại.
                // ApplyInternalSection vẫn cắt token "PL:Lx" và tiền tố "KQ:" khỏi nội dung.
                ApplyInternalSection(map, "tuanHoan", txtExamCirculation3, null);
                ApplyInternalSection(map, "hoHap", txtExamRespiratory3, null);
                ApplyInternalSection(map, "tieuHoa", txtExamDigestion3, null);
                ApplyInternalSection(map, "thanTietNieu", txtExamKidneyUrology3, null);
                ApplyInternalSection(map, "thanKinh", txtExamNeuroMental3, null);
                ApplyInternalSection(map, "tamThan", txtExamMental3, null);
                ApplyInternalSection(map, "lamSangKhac", txtExamClinicalOther3, null);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Tách 1 cặp "khóa:giá trị" của chuỗi mẫu. Trả về false nếu đoạn không hợp lệ.</summary>
        private bool SplitLibToken(string raw, out string key, out string value)
        {
            key = null;
            value = null;
            if (string.IsNullOrWhiteSpace(raw)) return false;
            int idx = raw.IndexOf(':');
            if (idx <= 0) return false;
            key = raw.Substring(0, idx).Trim().ToUpperInvariant();
            value = raw.Substring(idx + 1).Trim();
            return true;
        }

        #endregion
    }
}
