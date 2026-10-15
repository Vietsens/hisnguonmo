using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.KskInfomantionOfficials.ADO;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials
{
    /// <summary>
    /// Mau cho cac muc Tien su benh / Kham lam sang — dung Thu vien van ban (HIS.Desktop.Plugins.TextLibrary)
    /// giong EnterKskInfomantionVer2: bam nut → mo Thu vien loc san hashtag cua muc, noi dung muc hien tai
    /// da dien san (JSON) de Luu thanh mau; nhap dup 1 mau → dien vao muc.
    /// Noi dung mau la JSON, ten field viet lien khong dau (giong mau Noi khoa cua EnterKskInfomantionVer2):
    ///   { "ngoaiKhoa": "Binh thuong", "hamTren": "...", "matPhaiKhongKinh": "10/10" }        (o chu)
    ///   { "hutThuoc": { "thuocLa": true, "soNamDaHut": "10" } }                              (muc tick: true/false; muc co o chu: "noi dung")
    /// Doc mau thi khong phan biet dau/hoa thuong/khoang trang ("Thuoc la" = "thuocLa" = "Thuốc lá").
    /// Chi muc/o co trong mau moi bi dien de; khong co trong mau thi giu nguyen.
    /// </summary>
    partial class frmKskInfomantionOfficials
    {
        private const string TEMPLATE_MODULE_LINK = "HIS.Desktop.Plugins.TextLibrary";

        private Dictionary<SimpleButton, TemplateSectionADO> templateSections;
        private HIS_TEXT_LIB chosenTemplate;

        #region Section definitions
        private void EnsureTemplateSections()
        {
            if (this.templateSections != null) return;

            var sections = new Dictionary<SimpleButton, TemplateSectionADO>();

            // --- Tab Tien su benh ---
            sections[btnTemplateHabit] = new TemplateSectionADO("THOI_QUEN", "Thói quen sinh hoạt", "KskCbThoiQuen") { DiseaseParentType = 1 };
            sections[btnTemplateDiseaseHistory] = new TemplateSectionADO("TIEN_SU_BENH", "Tiền sử mắc bệnh", "KskCbTienSuBenh") { DiseaseParentType = 2 };
            sections[btnTemplateFamilyHistory] = new TemplateSectionADO("TIEN_SU_GIA_DINH", "Tiền sử gia đình", "KskCbTienSuGiaDinh") { DiseaseParentType = 3 };

            // --- Tab Kham lam sang: A. Hoi benh ---
            sections[btnTemplateFunctionalSymptom] = new TemplateSectionADO("TRIEU_CHUNG_CO_NANG", "Triệu chứng cơ năng", "KskCbTrieuChungCoNang") { DiseaseParentType = 4 };
            sections[btnTemplatePainSymptom] = new TemplateSectionADO("TRIEU_CHUNG_DAU", "Triệu chứng đau", "KskCbTrieuChungDau") { DiseaseParentType = 5 };

            // --- 2. Toan than --- (field JSON viet lien khong dau, giong mau Noi khoa cua EnterKskInfomantionVer2)
            var body = new TemplateSectionADO("TOAN_THAN", "Toàn thân", "KskCbToanThan");
            body.Fields.Add(new TemplateFieldADO(null, "BODY_SKIN", "daNiemMac", txtDaNiemMac));
            body.Fields.Add(new TemplateFieldADO(null, "BODY_TONGUE", "luoiHoiTho", txtLuoiHoiTho));
            body.Fields.Add(new TemplateFieldADO(null, "BODY_SPIRIT", "tinhThan", txtTinhThan));
            body.Fields.Add(new TemplateFieldADO(null, "BODY_LYMPH", "hachNgoaiVi", txtHachNgoaiVi));
            body.Fields.Add(new TemplateFieldADO(null, "BODY_OTHER", "khac", txtToanThanKhac));
            sections[btnTemplateBody] = body;

            // --- Kham noi khoa (3 → 8). KHONG gom chi so do: tan so tim, huyet ap, nhip tho ---
            var internalMedicine = new TemplateSectionADO("NOI_KHOA", "Khám nội khoa", "KskCbNoiKhoa");
            internalMedicine.Fields.Add(new TemplateFieldADO(null, "HEART_RHYTHM", "nhipTim", txtHeartRhythm));
            internalMedicine.Fields.Add(new TemplateFieldADO(null, "HEART_BEAT", "tiengTim", txtHeartBeat));
            internalMedicine.Fields.Add(new TemplateFieldADO(null, "EXAM_CIRCULATION", "timMachKhac", txtOtherExamCirculation));
            internalMedicine.Fields.Add(new TemplateFieldADO(null, "ARTERY", "dongMach", txtArtery));
            internalMedicine.Fields.Add(new TemplateFieldADO(null, "VEIN", "tinhMach", txtVein));
            internalMedicine.Fields.Add(new TemplateFieldADO(null, "CHEST", "longNguc", txtChest));
            internalMedicine.Fields.Add(new TemplateFieldADO(null, "LUNG", "khamPhoi", txtLung));
            internalMedicine.Fields.Add(new TemplateFieldADO(null, "EXAM_DIGESTION", "tieuHoa", txtExamDigestion));
            internalMedicine.Fields.Add(new TemplateFieldADO(null, "EXAM_KIDNEY_UROLOGY", "thanTietNieu", txtExamKidneyUrology));
            internalMedicine.Fields.Add(new TemplateFieldADO(null, "EXAM_MUSCLE_BONE", "coXuongKhop", txtExamMuscleBone));
            internalMedicine.Fields.Add(new TemplateFieldADO(null, "EXAM_OEND", "noiTiet", txtExamOEND));
            internalMedicine.Fields.Add(new TemplateFieldADO(null, "EXAM_NEUROLOGICAL", "tamThanThanKinh", txtExamNeurological));
            sections[btnTemplateInternal] = internalMedicine;

            // --- Kham ngoai khoa: 1 mau chung cho muc 9 → 14, kem o Nguoi kham tung muc (gia tri = ten dang nhap) ---
            var specialist = new TemplateSectionADO("NGOAI_KHOA", "Khám ngoại khoa", "KskCbNgoaiKhoa");
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_SURGERY", "ngoaiKhoa", txtExamSurgery));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_SURGERY_LOGINNAME", "nguoiKhamNgoaiKhoa", cboExamSurgeryLoginName));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_OBSTETRIC", "sanPhuKhoa", txtExamObstetric));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_OBSTETRIC_LOGINNAME", "nguoiKhamSanPhuKhoa", cboExamObstetricLoginName));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_DERMATOLOGY", "daLieu", txtExamDermatology));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_DERMATOLOGY_LOGINNAME", "nguoiKhamDaLieu", cboExamDermatologyLoginName));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_STOMATOLOGY_UPPER", "hamTren", txtExamStomatologyUpper));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_STOMATOLOGY_LOWER", "hamDuoi", txtExamStomatologyLower));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_STOMATOLOGY", "rangHamMat", txtExamStomatology));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_STOMATOLOGY_LOGINNAME", "nguoiKhamRangHamMat", cboExamStomatologyLoginName));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_ENT_DISEASE", "tinhTrangTai", txtExamENTDisease));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_ENT_LEFT_NORMAL", "taiTraiNoiThuong", txtExamENTLeftNormal));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_ENT_LEFT_WHISPER", "taiTraiNoiTham", txtExamENTLeftWhisper));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_ENT_RIGHT_NORMAL", "taiPhaiNoiThuong", txtExamENTRightNormal));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_ENT_RIGHT_WHISPER", "taiPhaiNoiTham", txtExamENTRightWhisper));
            specialist.Fields.Add(new TemplateFieldADO(null, "TEST_ENT_LEFT", "thinhLucTaiTrai", txtTestENTLeft));
            specialist.Fields.Add(new TemplateFieldADO(null, "TEST_ENT_RIGHT", "thinhLucTaiPhai", txtTestENTRight));
            specialist.Fields.Add(new TemplateFieldADO(null, "ENDOSCOPY_ENT", "noiSoiTaiMuiHong", txtEndoscopyENT));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_ENT", "taiMuiHong", txtExamENT));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_ENT_LOGINNAME", "nguoiKhamTaiMuiHong", cboExamENTLoginName));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_EYESIGHT_RIGHT", "matPhaiKhongKinh", txtExamEyeSightRight));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_EYESIGHT_LEFT", "matTraiKhongKinh", txtExamEyeSightLeft));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_EYESIGHT_GLASS_RIGHT", "matPhaiCoKinh", txtExamEyeSightGlassRight));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_EYESIGHT_GLASS_LEFT", "matTraiCoKinh", txtExamEyeSightGlassLeft));
            specialist.Fields.Add(new TemplateFieldADO(null, "INTRAOCULAR_PRESSURE", "nhanAp", txtIntraocularPressure));
            specialist.Fields.Add(new TemplateFieldADO(null, "LENS", "thuyTinhThe", txtLens));
            specialist.Fields.Add(new TemplateFieldADO(null, "FUNDOSCOPY", "soiDayMat", txtFundoscopy));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_EYE", "mat", txtExamEye));
            specialist.Fields.Add(new TemplateFieldADO(null, "EXAM_EYE_LOGINNAME", "nguoiKhamMat", cboExamEyeLoginName));
            sections[btnTemplateSpecialist] = specialist;

            this.templateSections = sections;
        }
        #endregion

        #region Layout
        /// <summary>
        /// Nut mau dung sat sau chu tieu de: tinh lai do rong o tieu de theo co chu thuc te
        /// (phan mem cho doi co chu), khoang trong phia sau tu co gian.
        /// </summary>
        private void FitTemplateLabelItems()
        {
            try
            {
                FitTemplateLabelItem(layoutControl5, layoutControlItem252, labelControl8);
                FitTemplateLabelItem(layoutControl5, layoutControlItem339, labelControl44);
                FitTemplateLabelItem(layoutControl5, layoutControlItem374, labelControl43);
                FitTemplateLabelItem(layoutControl11, layoutControlItem414, labelControl60);
                FitTemplateLabelItem(layoutControl11, layoutControlItem403, labelControl117);
                FitTemplateLabelItem(layoutControl11, layoutControlItem562, labelControl135);
                FitTemplateLabelItem(layoutControl11, layoutControlItem151, labelControl141);
                FitTemplateLabelItem(layoutControl11, layoutControlItem593, labelControl142);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FitTemplateLabelItem(LayoutControl layoutControl, LayoutControlItem item, LabelControl label)
        {
            int width = label.CalcBestSize().Width + item.Padding.Left + item.Padding.Right + 4;
            int height = item.MinSize.Height;
            if (width == item.MinSize.Width && width == item.MaxSize.Width) return;

            layoutControl.BeginUpdate();
            try
            {
                // Doi thu tu gan de luon giu MinSize <= MaxSize
                if (width > item.MaxSize.Width)
                {
                    item.MaxSize = new Size(width, height);
                    item.MinSize = new Size(width, height);
                }
                else
                {
                    item.MinSize = new Size(width, height);
                    item.MaxSize = new Size(width, height);
                }
            }
            finally
            {
                layoutControl.EndUpdate();
            }
        }
        #endregion

        #region Button
        private void btnTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureTemplateSections();
                SimpleButton button = sender as SimpleButton;
                TemplateSectionADO section;
                if (button == null || !this.templateSections.TryGetValue(button, out section))
                    return;

                EnsureClinicalGridsBuilt();

                // Giong EnterKskInfomantionVer2: loc san hashtag cua muc; noi dung muc hien tai dien san de Luu thanh mau
                HIS.Desktop.ADO.TextLibraryInfoADO textLibraryInfo = new HIS.Desktop.ADO.TextLibraryInfoADO();
                textLibraryInfo.Hashtag = section.Hashtag;
                textLibraryInfo.Content = BuildTemplateJson(section);

                this.chosenTemplate = null;
                OpenTextLibrary(textLibraryInfo, (HIS.Desktop.Common.DelegateDataTextLib)ReceiveChosenTemplate);

                // TextLibrary goi callback truoc khi dong → ap mau sau khi dong de thong bao hien tren form nay
                HIS_TEXT_LIB template = this.chosenTemplate;
                this.chosenTemplate = null;
                if (template != null)
                    ApplyTemplate(section, template);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReceiveChosenTemplate(HIS_TEXT_LIB textLib)
        {
            this.chosenTemplate = textLib;
        }

        private void OpenTextLibrary(HIS.Desktop.ADO.TextLibraryInfoADO textLibraryInfo, HIS.Desktop.Common.DelegateDataTextLib callback)
        {
            Inventec.Desktop.Common.Modules.Module moduleRaw = GlobalVariables.currentModuleRaws
                .FirstOrDefault(o => o.ModuleLink == TEMPLATE_MODULE_LINK);
            if (moduleRaw == null || !moduleRaw.IsPlugin || moduleRaw.ExtensionInfo == null)
            {
                Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + TEMPLATE_MODULE_LINK);
                XtraMessageBox.Show("Tài khoản chưa được cấp chức năng Thư viện văn bản nên không mở được mẫu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Inventec.Desktop.Common.Modules.Module module = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleRaw, this.moduleData.RoomId, this.moduleData.RoomTypeId);
            List<object> listArgs = new List<object>();
            listArgs.Add(module);
            listArgs.Add(textLibraryInfo);
            if (callback != null)
                listArgs.Add(callback);

            Form textLibraryForm = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(module, listArgs) as Form;
            if (textLibraryForm == null) throw new ArgumentNullException("extenceInstance is null");
            textLibraryForm.ShowDialog();
        }
        #endregion

        #region Build JSON (noi dung muc hien tai)
        private string BuildTemplateJson(TemplateSectionADO section)
        {
            JObject root = new JObject();
            if (section.DiseaseParentType.HasValue)
            {
                // Chot o dang go do tren luoi truoc khi doc
                GridView gridView = GetTemplateGridView(section);
                if (gridView != null)
                    gridView.CloseEditor();
                List<TemplateItemADO> items = GetTemplateItems(section);
                // Luoi (tien su gia dinh, trieu chung) hien danh sach phang → JSON phang neu ten muc khong trung
                bool flat = section.DiseaseParentType.Value >= 3
                    && items.GroupBy(o => NormalizeTemplateName(o.Name, true)).All(g => g.Count() == 1);
                foreach (TemplateItemADO item in items)
                {
                    JObject target = root;
                    string key;
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        // Muc chi co o chu, khong co ten (VD "4) Thoi gian trung binh choi the thao") → dung ten nhom
                        key = ToTemplateKey(item.GroupName, false);
                    }
                    else
                    {
                        List<TemplateItemADO> scope = flat ? items : items.Where(o => o.GroupName == item.GroupName).ToList();
                        if (!flat) target = GetOrAddGroup(root, ToTemplateKey(item.GroupName, false));
                        // Bo dau ma trung ten khac (VD "Cổ"/"Cơ" → "co") thi giu dau de phan biet
                        bool clash = scope.Count(o => NormalizeTemplateName(o.Name, false) == NormalizeTemplateName(item.Name, false)) > 1;
                        key = ToTemplateKey(item.Name, clash);
                    }
                    target[UniqueKey(target, key)] = BuildItemValue(item);
                }
            }
            else
            {
                foreach (TemplateFieldADO field in section.Fields)
                {
                    JObject target = string.IsNullOrEmpty(field.Group) ? root : GetOrAddGroup(root, field.Group);
                    target[field.Label] = ReadTemplateField(field);
                }
            }
            return root.ToString(Formatting.Indented);
        }

        private static JObject GetOrAddGroup(JObject root, string groupName)
        {
            string name = string.IsNullOrEmpty(groupName) ? "khac" : groupName;
            JObject group = root[name] as JObject;
            if (group == null)
            {
                group = new JObject();
                root[name] = group;
            }
            return group;
        }

        private static string UniqueKey(JObject target, string name)
        {
            string key = name;
            int i = 2;
            while (target[key] != null)
                key = name + (i++);
            return key;
        }

        /// <summary>
        /// Ten danh muc → field JSON viet lien: "1) Tim mạch" → "timMach", "Số năm đã hút" → "soNamDaHut", "< 5" → "<5".
        /// keepMarks = true: giu dau tieng Viet (chi dung khi bo dau bi trung ten khac, VD "cổ" / "cơ").
        /// </summary>
        private static string ToTemplateKey(string name, bool keepMarks)
        {
            string value = StripTemplateNumbering(name ?? "");
            if (!keepMarks) value = RemoveVietnameseMarks(value);
            string[] words = Regex.Split(value, @"[^\p{L}\p{N}<>≤≥]+").Where(o => o.Length > 0).ToArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (i == 0)
                    sb.Append(word.ToLowerInvariant());
                else if (word.Length > 1 && word.ToUpperInvariant() == word)
                    sb.Append(word);   // giu chu viet tat: "HA", "DM"
                else
                    sb.Append(char.ToUpperInvariant(word[0])).Append(word.Substring(1).ToLowerInvariant());
            }
            return sb.Length > 0 ? sb.ToString() : (name ?? "").Trim();
        }

        private static string StripTemplateNumbering(string text)
        {
            // "1)", "3.", "1.1.", "- " o dau dong
            return Regex.Replace((text ?? "").Trim(), @"^[\-\s]*(\d+(\.\d+)*[\.\)]\s*)?", "");
        }

        private static string RemoveVietnameseMarks(string text)
        {
            string decomposed = (text ?? "").Replace('đ', 'd').Replace('Đ', 'D').Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder(decomposed.Length);
            foreach (char c in decomposed)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private JToken BuildItemValue(TemplateItemADO item)
        {
            bool isChecked;
            string text;
            ReadTemplateItem(item, out isChecked, out text);
            if (!HasItemText(item))
                return new JValue(isChecked);
            if (!HasItemCheck(item))
                return new JValue(text);
            // Muc tick + o chu: co chu → ghi chu (hieu la co), khong chu → true/false
            if (!string.IsNullOrEmpty(text))
                return new JValue(text);
            return new JValue(isChecked);
        }
        #endregion

        #region Apply (nhap dup mau)
        private void ApplyTemplate(TemplateSectionADO section, HIS_TEXT_LIB template)
        {
            string title = template.TITLE ?? "";
            string content = template.CONTENT != null ? HIS.Desktop.Utility.TextLibHelper.BytesToString(template.CONTENT) : "";

            JObject root;
            string parseError;
            if (!TryParseTemplateJson(content, out root, out parseError))
            {
                XtraMessageBox.Show(string.Format("Mẫu \"{0}\" không đọc được, nội dung mẫu cần ở dạng JSON.\r\n{1}\r\n\r\nVí dụ: {{ \"daNiemMac\": \"Hồng\", \"tinhThan\": \"Tỉnh táo\" }}",
                    title, parseError), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> skipped = new List<string>();
            List<TemplateEntryADO> entries = FlattenTemplateJson(root, skipped);
            List<Action> actions = new List<Action>();
            int matched = 0;
            int overwrite = 0;

            if (section.DiseaseParentType.HasValue)
            {
                GridView gridView = GetTemplateGridView(section);
                if (gridView != null)
                    gridView.CloseEditor();

                List<TemplateItemADO> items = GetTemplateItems(section);
                foreach (TemplateEntryADO entry in entries)
                {
                    TemplateItemADO item = MatchTemplateItem(items, entry);
                    bool newChecked;
                    string newText;
                    if (item == null || !TryReadItemValue(item, entry.Value, out newChecked, out newText))
                    {
                        skipped.Add(entry.DisplayName);
                        continue;
                    }
                    matched++;

                    bool curChecked;
                    string curText;
                    ReadTemplateItem(item, out curChecked, out curText);
                    bool changed = (HasItemCheck(item) && curChecked != newChecked) || (HasItemText(item) && curText != newText);
                    if (!changed) continue;
                    if (curChecked || !string.IsNullOrEmpty(curText)) overwrite++;

                    TemplateItemADO target = item;
                    actions.Add(() => WriteTemplateItem(target, newChecked, newText));
                }
                if (actions.Count > 0 && gridView != null)
                    actions.Add(() => gridView.RefreshData());
            }
            else
            {
                foreach (TemplateEntryADO entry in entries)
                {
                    TemplateFieldADO field = MatchTemplateField(section, entry);
                    if (field == null || !(entry.Value is JValue))
                    {
                        skipped.Add(entry.DisplayName);
                        continue;
                    }
                    string newText = JValueToText((JValue)entry.Value);
                    if (field.Editor is GridLookUpEdit)
                    {
                        // O nguoi kham: nhan ten dang nhap hoac ho ten → quy ve ten dang nhap
                        string loginName;
                        if (!TryResolveLoginName(newText, out loginName))
                        {
                            skipped.Add(entry.DisplayName + " = " + newText + " (không tìm thấy tài khoản)");
                            continue;
                        }
                        newText = loginName;
                    }
                    matched++;

                    string curText = ReadTemplateField(field);
                    if (string.Equals(curText, newText, StringComparison.OrdinalIgnoreCase)) continue;
                    if (!string.IsNullOrEmpty(curText)) overwrite++;

                    TemplateFieldADO target = field;
                    actions.Add(() => WriteTemplateField(target, newText));
                }
            }

            if (matched == 0)
            {
                XtraMessageBox.Show(string.Format("Mẫu \"{0}\" không có mục nào khớp với phần \"{1}\" nên không điền.\r\nKiểm tra lại tên field trong mẫu.",
                    title, section.Title), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Mau trung khop noi dung dang co (VD vua Luu mau tu chinh noi dung nay) → khong can lam gi
            if (actions.Count == 0) return;

            if (!CheckCanApplyTemplate()) return;
            if (overwrite > 0
                && XtraMessageBox.Show(string.Format("Mẫu \"{0}\" sẽ ghi đè {1} mục/ô đang có dữ liệu của phần \"{2}\". Tiếp tục?", title, overwrite, section.Title),
                    "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            foreach (Action action in actions)
                action();

            Inventec.Common.Logging.LogSystem.Debug("KskOfficials.Template: ap mau section=" + section.Code + " matched=" + matched + " changed=" + actions.Count + " skipped=" + skipped.Count);
            if (skipped.Count > 0)
            {
                XtraMessageBox.Show(string.Format("Đã điền mẫu \"{0}\". Bỏ qua {1} mục không khớp phần \"{2}\":\r\n- {3}",
                    title, skipped.Count, section.Title, string.Join("\r\n- ", skipped.Take(10))), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool CheckCanApplyTemplate()
        {
            if (this.currentData == null)
            {
                XtraMessageBox.Show("Chưa chọn y lệnh khám. Chọn bệnh nhân ở danh sách bên trái trước khi điền mẫu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.currentServiceReqSTT == ServiceReqStatus.HoanThanh)
            {
                XtraMessageBox.Show("Y lệnh đã hoàn thành nên không sửa được nội dung khám. Hủy kết thúc trước nếu cần điền mẫu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void WriteTemplateItem(TemplateItemADO item, bool isChecked, string text)
        {
            // Tick truoc, ghi chu sau: o chu dang so chi mo khi da tick (va bi xoa khi bo tick)
            if (item.Check != null) item.Check.Checked = isChecked;
            if (item.TextEditor != null) item.TextEditor.Text = text ?? string.Empty;
            if (item.GridRow != null)
            {
                if (item.GridRow.HAS_CHECKBOX) item.GridRow.IS_CHECKED = isChecked;
                if (item.GridRow.HAS_OTHER) UC.DiseaseDetailGridHelper.SetOtherText(item.GridRow, text);
            }
        }
        #endregion

        #region Parse JSON
        private static bool TryParseTemplateJson(string content, out JObject root, out string error)
        {
            root = null;
            error = null;
            string json = (content ?? "").Trim();
            if (json.Length == 0)
            {
                error = "Mẫu đang trống.";
                return false;
            }
            // Go trong Thu vien (RichEdit) co the ra ngoac kep cong
            json = json.Replace('“', '"').Replace('”', '"').Replace('‘', '\'').Replace('’', '\'');
            try
            {
                root = JObject.Parse(json);
                return true;
            }
            catch (JsonReaderException ex)
            {
                error = string.Format("Sai cú pháp ở dòng {0}, vị trí {1}: thường do thiếu dấu phẩy giữa 2 mục, thiếu ngoặc kép hoặc thiếu dấu ngoặc nhọn.", ex.LineNumber, ex.LinePosition);
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static List<TemplateEntryADO> FlattenTemplateJson(JObject root, List<string> skipped)
        {
            List<TemplateEntryADO> entries = new List<TemplateEntryADO>();
            foreach (JProperty prop in root.Properties())
            {
                if (prop.Name.StartsWith("_")) continue;
                JObject group = prop.Value as JObject;
                if (group == null)
                {
                    entries.Add(new TemplateEntryADO { Key = prop.Name, Value = prop.Value });
                    continue;
                }
                foreach (JProperty child in group.Properties())
                {
                    if (child.Name.StartsWith("_")) continue;
                    if (child.Value is JObject || child.Value is JArray)
                    {
                        skipped.Add(prop.Name + " > " + child.Name);
                        continue;
                    }
                    entries.Add(new TemplateEntryADO { Group = prop.Name, Key = child.Name, Value = child.Value });
                }
            }
            return entries;
        }

        /// <summary>Khop field JSON → muc: dung dau truoc, roi khong dau; trong nhom truoc, roi toan muc (neu duy nhat).</summary>
        private TemplateItemADO MatchTemplateItem(List<TemplateItemADO> items, TemplateEntryADO entry)
        {
            foreach (bool keepMarks in new[] { true, false })
            {
                string key = NormalizeTemplateName(entry.Key, keepMarks);
                if (string.IsNullOrEmpty(key)) return null;
                string group = NormalizeTemplateName(entry.Group, keepMarks);

                TemplateItemADO found = null;
                if (!string.IsNullOrEmpty(group))
                    found = SingleOrNull(items.Where(o => NormalizeTemplateName(o.GroupName, keepMarks) == group && NormalizeTemplateName(o.Name, keepMarks) == key));
                if (found == null)
                    found = SingleOrNull(items.Where(o => !string.IsNullOrEmpty(o.Name) && NormalizeTemplateName(o.Name, keepMarks) == key));
                if (found == null) // muc khong ten → field = ten nhom
                    found = SingleOrNull(items.Where(o => string.IsNullOrEmpty(o.Name) && NormalizeTemplateName(o.GroupName, keepMarks) == key));
                if (found != null) return found;
            }
            return null;
        }

        private static T SingleOrNull<T>(IEnumerable<T> source) where T : class
        {
            List<T> list = source.Take(2).ToList();
            return list.Count == 1 ? list[0] : null;
        }

        private TemplateFieldADO MatchTemplateField(TemplateSectionADO section, TemplateEntryADO entry)
        {
            // Chap nhan ca ten cot (EXAM_EYE...)
            TemplateFieldADO byColumn = section.Fields.FirstOrDefault(o => string.Equals(o.Key, (entry.Key ?? "").Trim(), StringComparison.OrdinalIgnoreCase));
            if (byColumn != null) return byColumn;
            foreach (bool keepMarks in new[] { true, false })
            {
                string key = NormalizeTemplateName(entry.Key, keepMarks);
                if (string.IsNullOrEmpty(key)) return null;
                TemplateFieldADO found = SingleOrNull(section.Fields.Where(o => NormalizeTemplateName(o.Label, keepMarks) == key));
                if (found != null) return found;
            }
            return null;
        }

        /// <summary>
        /// Doc gia tri JSON cho 1 muc: true/false → tick; "chu" → o chu (muc vua tick vua ghi chu: co chu = co tick);
        /// muc chi co tick ma ghi chu thi hieu "co/x/true" = co, "khong/false/rong" = khong.
        /// </summary>
        private bool TryReadItemValue(TemplateItemADO item, JToken value, out bool isChecked, out string text)
        {
            isChecked = false;
            text = string.Empty;
            JValue jValue = value as JValue;
            if (jValue == null) return false;

            if (jValue.Type == JTokenType.Boolean)
            {
                isChecked = (bool)jValue.Value;
                return true;
            }
            string s = JValueToText(jValue);
            bool? yesNo = ParseYesNo(s);
            if (!HasItemText(item))
            {
                if (yesNo == null) return false;
                isChecked = yesNo.Value;
                return true;
            }
            if (HasItemCheck(item) && yesNo != null && s.Length > 0 && !Regex.IsMatch(s, @"^\d+$"))
            {
                // "có"/"không" ghi cho muc vua tick vua ghi chu → chi la tick
                isChecked = yesNo.Value;
                return true;
            }
            text = s;
            isChecked = HasItemCheck(item) && s.Length > 0;
            return true;
        }

        private static bool? ParseYesNo(string text)
        {
            string s = NormalizeTemplateName(text, false);
            switch (s)
            {
                case "true": case "x": case "co": case "yes": case "1":
                    return true;
                case "false": case "khong": case "no": case "0": case "":
                    return false;
                default:
                    return null;
            }
        }

        private static string JValueToText(JValue value)
        {
            if (value == null || value.Type == JTokenType.Null || value.Value == null) return string.Empty;
            if (value.Type == JTokenType.Boolean) return string.Empty;
            return Convert.ToString(value.Value, System.Globalization.CultureInfo.InvariantCulture).Trim();
        }

        /// <summary>
        /// So khop ten field: bo so thu tu dau dong ("1)", "3.", "1.1."), hoa/thuong, khoang trang va dau cau
        /// → "Thuốc lá" = "thuốcLá" = "thuoc la" (khi keepMarks = false). Giu ky hieu so sanh (&lt;5, ≤ 8...).
        /// </summary>
        private static string NormalizeTemplateName(string text, bool keepMarks)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            string value = StripTemplateNumbering(text).Normalize(NormalizationForm.FormC);
            if (!keepMarks) value = RemoveVietnameseMarks(value);
            return Regex.Replace(value.ToLowerInvariant(), @"[^\p{L}\p{N}<>≤≥]+", "");
        }
        #endregion

        #region Read section
        private List<TemplateItemADO> GetTemplateItems(TemplateSectionADO section)
        {
            List<TemplateItemADO> items = new List<TemplateItemADO>();
            long parentType = section.DiseaseParentType ?? 0;
            if (parentType == 1 || parentType == 2)
            {
                // Checkbox + o chu gen dong (ucDiseaseDetailGroup) — tra control qua mapping theo DISEASE_DETAIL_ID
                var details = (diseaseDetails ?? new List<V_HIS_DISEASE_DETAIL>())
                    .Where(o => o.PARENT_TYPE == parentType)
                    .OrderBy(o => o.NUM_ORDER_TYPE)
                    .ThenBy(o => o.NUM_ORDER_DETAIL);
                foreach (V_HIS_DISEASE_DETAIL detail in details)
                {
                    CheckEdit check = null;
                    Control text = null;
                    if (diseaseCheckMapping != null) diseaseCheckMapping.TryGetValue(detail.ID, out check);
                    if (diseaseTextMapping != null) diseaseTextMapping.TryGetValue(detail.ID, out text);
                    if (check == null && text == null) continue;
                    items.Add(new TemplateItemADO(detail.ID, detail.DISEASE_TYPE_NAME, detail.NAME) { Check = check, TextEditor = text });
                }
            }
            else
            {
                foreach (ADO.DiseaseDetailGridADO row in GetTemplateGridRows(section))
                {
                    if (!row.HAS_CHECKBOX && !row.HAS_OTHER) continue;
                    items.Add(new TemplateItemADO(row.DISEASE_DETAIL_ID, row.GROUP_NAME, row.DISEASE_NAME) { GridRow = row });
                }
            }
            return items;
        }

        private List<ADO.DiseaseDetailGridADO> GetTemplateGridRows(TemplateSectionADO section)
        {
            switch (section.DiseaseParentType ?? 0)
            {
                case 3: return diseaseGridParent3 ?? new List<ADO.DiseaseDetailGridADO>();
                case 4: return diseaseGridParent4 ?? new List<ADO.DiseaseDetailGridADO>();
                case 5: return diseaseGridParent5 ?? new List<ADO.DiseaseDetailGridADO>();
                default: return new List<ADO.DiseaseDetailGridADO>();
            }
        }

        private GridView GetTemplateGridView(TemplateSectionADO section)
        {
            switch (section.DiseaseParentType ?? 0)
            {
                case 3: return gridView48;
                case 4: return gridView50;
                case 5: return gridView49;
                default: return null;
            }
        }

        private static bool HasItemCheck(TemplateItemADO item)
        {
            return item.Check != null || (item.GridRow != null && item.GridRow.HAS_CHECKBOX);
        }

        private static bool HasItemText(TemplateItemADO item)
        {
            return item.TextEditor != null || (item.GridRow != null && item.GridRow.HAS_OTHER);
        }

        private void ReadTemplateItem(TemplateItemADO item, out bool isChecked, out string text)
        {
            isChecked = false;
            text = string.Empty;
            if (item.Check != null) isChecked = item.Check.Checked;
            if (item.TextEditor != null) text = ReadTemplateEditor(item.TextEditor);
            if (item.GridRow != null)
            {
                isChecked = item.GridRow.HAS_CHECKBOX && item.GridRow.IS_CHECKED;
                text = UC.DiseaseDetailGridHelper.GetOtherText(item.GridRow);
            }
        }

        /// <summary>O chu → Text; o nguoi kham (GridLookUpEdit) → ten dang nhap (EditValue).</summary>
        private static string ReadTemplateField(TemplateFieldADO field)
        {
            GridLookUpEdit lookUp = field.Editor as GridLookUpEdit;
            if (lookUp != null)
                return lookUp.EditValue == null ? string.Empty : lookUp.EditValue.ToString().Trim();
            return ReadTemplateEditor(field.Editor);
        }

        private static void WriteTemplateField(TemplateFieldADO field, string value)
        {
            GridLookUpEdit lookUp = field.Editor as GridLookUpEdit;
            if (lookUp != null)
                lookUp.EditValue = string.IsNullOrEmpty(value) ? null : value;
            else
                field.Editor.Text = value ?? string.Empty;
        }

        /// <summary>Gia tri o nguoi kham trong mau: ten dang nhap hoac ho ten (khong phan biet dau/hoa thuong). Rong = bo trong.</summary>
        private bool TryResolveLoginName(string text, out string loginName)
        {
            loginName = string.Empty;
            string value = (text ?? "").Trim();
            if (value.Length == 0) return true;

            List<ACS.EFMODEL.DataModels.ACS_USER> users = GetAcsUsers().Where(o => o.IS_ACTIVE == 1).ToList();
            ACS.EFMODEL.DataModels.ACS_USER user = users.FirstOrDefault(o => string.Equals(o.LOGINNAME, value, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                foreach (bool keepMarks in new[] { true, false })
                {
                    string name = NormalizeTemplateName(value, keepMarks);
                    user = SingleOrNull(users.Where(o => NormalizeTemplateName(o.USERNAME, keepMarks) == name));
                    if (user != null) break;
                }
            }
            if (user == null) return false;
            loginName = user.LOGINNAME;
            return true;
        }

        private static string ReadTemplateEditor(Control editor)
        {
            return editor == null || editor.Text == null ? string.Empty : editor.Text.Trim();
        }
        #endregion
    }
}
