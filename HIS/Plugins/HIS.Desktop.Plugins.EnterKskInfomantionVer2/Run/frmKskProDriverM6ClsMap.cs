/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>
    /// Danh sách chỉ tiêu cận lâm sàng của Mẫu 06 lấy được tự động từ kết quả xét nghiệm / dịch vụ.
    /// MÃ CHỈ TIÊU dùng chung với bảng nối chỉ số mẫu M3 của màn Đồng bộ KSK (cổng Sở Y tế TP.HCM) —
    /// nhờ vậy viện đã nối cho M3 chỉ cần Xuất file bên đó rồi Nhập file ở đây.
    /// </summary>
    internal static class KskProDriverM6ClsFields
    {
        internal const string MAP_VERSION = "1";
        internal const string FORM_CODE = "MAU06";
        internal const string KIND_TEST_INDEX = "XN";
        internal const string KIND_SERVICE = "DV";
        internal const string STATE_KEY = "KskProDriverClsMap";

        internal class Field
        {
            public string Group;
            public string Code;
            public string Name;
            /// <summary>Cột HIS_KSK_PRO_DRIVER nhận giá trị.</summary>
            public string Column;
            /// <summary>Cột người thực hiện của nhóm.</summary>
            public string PerformerColumn;
        }

        internal static readonly List<Field> ALL = Build();

        private static List<Field> Build()
        {
            List<Field> rs = new List<Field>();
            Action<string, string, string, string, string> add = (g, c, n, col, pf) =>
                rs.Add(new Field { Group = g, Code = c, Name = n, Column = col, PerformerColumn = pf });
            const string B = "Công thức máu", S = "Sinh hóa máu", U = "Nước tiểu", F = "Xét nghiệm riêng cho nữ", D = "Chẩn đoán hình ảnh";
            add(B, "xnm_slhc", "Số lượng hồng cầu", "TEST_BLOOD_HC", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_huyetsacto", "Huyết sắc tố", "TEST_BLOOD_HGB", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_hematocrit", "Hematocrit", "TEST_BLOOD_HCT", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_mcv", "MCV", "TEST_BLOOD_MCV", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_mch", "MCH", "TEST_BLOOD_MCH", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_mchc", "MCHC", "TEST_BLOOD_MCHC", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_rdw", "RDW", "TEST_BLOOD_RDW", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_slbc", "Số lượng bạch cầu", "TEST_BLOOD_BC", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_slbc_trungtinh", "Bạch cầu trung tính", "TEST_BLOOD_NEUT", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_slbc_lympho", "Bạch cầu lympho", "TEST_BLOOD_LYMPH", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_slbc_donnhan", "Bạch cầu đơn nhân", "TEST_BLOOD_MONO", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_slbc_aitoan", "Bạch cầu ái toan", "TEST_BLOOD_EOS", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_slbc_aikiem", "Bạch cầu ái kiềm", "TEST_BLOOD_BASO", "TEST_BLOOD_LOGINNAME");
            add(B, "xnm_sltc", "Số lượng tiểu cầu", "TEST_BLOOD_TC", "TEST_BLOOD_LOGINNAME");
            add(S, "shm_duongmau", "Đường máu", "TEST_BLOOD_GLUCO", "TEST_BLOOD_LOGINNAME");
            add(S, "shm_creatinin", "Creatinin", "TEST_BLOOD_CREATININ", "TEST_BLOOD_LOGINNAME");
            add(S, "shm_asat_got", "ASAT (GOT)", "TEST_BLOOD_ASAT", "TEST_BLOOD_LOGINNAME");
            add(S, "shm_alat_gpt", "ALAT (GPT)", "TEST_BLOOD_ALAT", "TEST_BLOOD_LOGINNAME");
            add(U, "xnnt_titrong", "Tỉ trọng", "TEST_URINE_SG", "TEST_URINE_LOGINNAME");
            add(U, "xnnt_ph", "pH", "TEST_URINE_PH", "TEST_URINE_LOGINNAME");
            add(U, "xnnt_bachcau", "Bạch cầu niệu", "TEST_URINE_LEU", "TEST_URINE_LOGINNAME");
            add(U, "xnnt_hongcau", "Hồng cầu niệu", "TEST_URINE_ERY", "TEST_URINE_LOGINNAME");
            add(U, "xnnt_nitrit", "Nitrit", "TEST_URINE_NITRIT", "TEST_URINE_LOGINNAME");
            add(U, "xnnt_protein", "Protein", "TEST_URINE_PROTEIN", "TEST_URINE_LOGINNAME");
            add(U, "xnnt_glucose", "Glucose", "TEST_URINE_GLUCO", "TEST_URINE_LOGINNAME");
            add(U, "xnnt_cetonic", "Thể cetonic", "TEST_URINE_KETONE", "TEST_URINE_LOGINNAME");
            add(U, "xnnt_bilirubin", "Bilirubin", "TEST_URINE_BILIRUBIN", "TEST_URINE_LOGINNAME");
            add(U, "xnnt_urobilinogen", "Urobilinogen", "TEST_URINE_UBG", "TEST_URINE_LOGINNAME");
            add(U, "xnnt_khac", "Chỉ số khác", "TEST_URINE_OTHER", "TEST_URINE_LOGINNAME");
            add(F, "xet_nghiem_te_bao_co_tu_cung", "Xét nghiệm tế bào cổ tử cung", "TEST_CERVICAL", "TEST_FEMALE_LOGINNAME");
            add(F, "xet_nghiem_hpv", "Xét nghiệm HPV", "TEST_CERVICAL", "TEST_FEMALE_LOGINNAME");
            add(F, "xquang_nhu", "X-quang nhũ", "TEST_BREAST", "TEST_FEMALE_LOGINNAME");
            add(F, "sieu_am_2_tuyen_vu", "Siêu âm 02 tuyến vú", "TEST_BREAST", "TEST_FEMALE_LOGINNAME");
            add(D, "chan_doan_hinh_anh", "Kết quả chẩn đoán hình ảnh (dịch vụ)", "RESULT_DIIM", "DIIM_LOGINNAME");
            return rs;
        }

        /// <summary>Bảng nối đã lưu — cùng hình dạng tệp với bảng nối M3 (Version, FormCode, Items).</summary>
        internal class MapFile
        {
            public string Version { get; set; }
            public string FormCode { get; set; }
            public List<MapItem> Items { get; set; }
        }

        internal class MapItem
        {
            public string FieldCode { get; set; }
            public string TestIndexCode { get; set; }
            /// <summary>"XN" chỉ số xét nghiệm / "DV" dịch vụ. Rỗng = "XN" (tệp cũ).</summary>
            public string SourceKind { get; set; }
            public string Note { get; set; }
        }

        /// <summary>Chuỗi JSON -> (mã chỉ tiêu -> MapItem). Chuỗi hỏng / rỗng -> rỗng, không báo lỗi.</summary>
        internal static Dictionary<string, MapItem> Parse(string json)
        {
            var rs = new Dictionary<string, MapItem>();
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return rs;
                MapFile f = Newtonsoft.Json.JsonConvert.DeserializeObject<MapFile>(json);
                if (f == null || f.Items == null) return rs;
                HashSet<string> known = new HashSet<string>(ALL.Select(o => o.Code));
                foreach (MapItem it in f.Items)
                {
                    if (it == null || string.IsNullOrWhiteSpace(it.FieldCode) || string.IsNullOrWhiteSpace(it.TestIndexCode)) continue;
                    string code = it.FieldCode.Trim();
                    if (!known.Contains(code)) continue; // VD shm_ure của M3 — Mẫu 06 không có ô
                    if (string.IsNullOrWhiteSpace(it.SourceKind)) it.SourceKind = KIND_TEST_INDEX;
                    rs[code] = it;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return rs;
        }
    }

    /// <summary>
    /// Hộp thoại "Nối chỉ số CLS — Mẫu 06": mỗi chỉ tiêu cận lâm sàng của giấy nối 1 chỉ số xét nghiệm hoặc
    /// 1 dịch vụ của viện. Dựng bằng code. Lưu do form gọi (trả JSON qua ResultJson) — giống màn nối chỉ số M3.
    /// </summary>
    internal class frmKskProDriverM6ClsMap : XtraForm
    {
        private class Row
        {
            public string GroupName { get; set; }
            public string FieldCode { get; set; }
            public string FieldName { get; set; }
            /// <summary>"XN|mã" / "DV|mã".</summary>
            public string SourceKey { get; set; }
            public string UnitName { get; set; }
            public string Note { get; set; }
        }

        private class Source
        {
            public string Key { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string Unit { get; set; }
            public string GroupName { get; set; }
        }

        private static readonly string[] SERVICE_TYPE_KEYWORDS =
            new string[] { "siêu âm", "phẫu thuật", "thủ thuật", "chẩn đoán hình ảnh", "x-quang", "nội soi", "thăm dò chức năng" };

        private readonly List<Row> rows = new List<Row>();
        private List<Source> sources = new List<Source>();
        private Dictionary<string, Source> sourceByKey = new Dictionary<string, Source>();
        private GridControl grid;
        private GridView view;

        /// <summary>JSON bảng nối sau khi bấm Lưu (DialogResult = OK).</summary>
        public string ResultJson { get; private set; }

        public frmKskProDriverM6ClsMap(string json)
        {
            try
            {
                this.Text = "Nối chỉ số cận lâm sàng — Giấy KSK định kỳ lái xe ô tô (Mẫu 06)";
                this.StartPosition = FormStartPosition.CenterParent;
                this.Size = new Size(1000, 680);
                this.MinimizeBox = false;
                LoadSources();
                BuildUi();
                ApplyJson(json);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void LoadSources()
        {
            var list = new List<Source>();
            try
            {
                var tis = BackendDataWorker.Get<V_HIS_TEST_INDEX>() ?? new List<V_HIS_TEST_INDEX>();
                list.AddRange(tis.Where(o => o != null && !string.IsNullOrWhiteSpace(o.TEST_INDEX_CODE) && o.IS_ACTIVE == 1
                        && (o.IS_DELETE == null || o.IS_DELETE == 0))
                    .OrderBy(o => o.TEST_INDEX_GROUP_NAME).ThenBy(o => o.NUM_ORDER).ThenBy(o => o.TEST_INDEX_NAME)
                    .Select(o => new Source
                    {
                        Key = KskProDriverM6ClsFields.KIND_TEST_INDEX + "|" + o.TEST_INDEX_CODE,
                        Code = o.TEST_INDEX_CODE,
                        Name = o.TEST_INDEX_NAME,
                        Unit = o.TEST_INDEX_UNIT_NAME,
                        GroupName = o.TEST_INDEX_GROUP_NAME
                    }));
                var svs = BackendDataWorker.Get<V_HIS_SERVICE>() ?? new List<V_HIS_SERVICE>();
                foreach (var sv in svs)
                {
                    if (sv == null || string.IsNullOrWhiteSpace(sv.SERVICE_CODE)) continue;
                    if (sv.IS_ACTIVE != 1 || (sv.IS_DELETE != null && sv.IS_DELETE != 0)) continue;
                    string loai = (sv.SERVICE_TYPE_NAME ?? "").ToLowerInvariant();
                    if (!SERVICE_TYPE_KEYWORDS.Any(k => loai.Contains(k))) continue;
                    list.Add(new Source
                    {
                        Key = KskProDriverM6ClsFields.KIND_SERVICE + "|" + sv.SERVICE_CODE,
                        Code = sv.SERVICE_CODE,
                        Name = sv.SERVICE_NAME,
                        Unit = "",
                        GroupName = "Dịch vụ - " + sv.SERVICE_TYPE_NAME
                    });
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            sources = list;
            sourceByKey = new Dictionary<string, Source>();
            foreach (var s in sources) if (!sourceByKey.ContainsKey(s.Key)) sourceByKey[s.Key] = s;
        }

        private void BuildUi()
        {
            foreach (var f in KskProDriverM6ClsFields.ALL)
                rows.Add(new Row { GroupName = f.Group, FieldCode = f.Code, FieldName = f.Name });

            grid = new GridControl();
            view = new GridView(grid);
            grid.MainView = view;
            grid.Dock = DockStyle.Fill;
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsView.ColumnAutoWidth = false;

            RepositoryItemSearchLookUpEdit repo = new RepositoryItemSearchLookUpEdit();
            repo.DataSource = sources;
            repo.ValueMember = "Key";
            repo.DisplayMember = "Name";
            repo.NullText = "";
            repo.View.OptionsBehavior.AutoPopulateColumns = false;
            repo.View.Columns.AddVisible("Code", "Mã").Width = 90;
            repo.View.Columns.AddVisible("Name", "Tên chỉ số / dịch vụ").Width = 260;
            repo.View.Columns.AddVisible("Unit", "Đơn vị").Width = 70;
            repo.View.Columns.AddVisible("GroupName", "Nhóm").Width = 200;
            repo.PopupFormSize = new Size(660, 380);
            repo.EditValueChanged += (s, e) => { view.PostEditor(); RefreshUnit(); };
            grid.RepositoryItems.Add(repo);

            var cGroup = view.Columns.AddVisible("GroupName", "Nhóm"); cGroup.Width = 160; cGroup.OptionsColumn.AllowEdit = false;
            var cCode = view.Columns.AddVisible("FieldCode", "Mã chỉ tiêu"); cCode.Width = 170; cCode.OptionsColumn.AllowEdit = false;
            var cName = view.Columns.AddVisible("FieldName", "Chỉ tiêu trên Mẫu 06"); cName.Width = 210; cName.OptionsColumn.AllowEdit = false;
            var cSrc = view.Columns.AddVisible("SourceKey", "Chỉ số XN / dịch vụ của viện"); cSrc.Width = 250; cSrc.ColumnEdit = repo;
            var cUnit = view.Columns.AddVisible("UnitName", "Đơn vị"); cUnit.Width = 60; cUnit.OptionsColumn.AllowEdit = false;
            var cNote = view.Columns.AddVisible("Note", "Ghi chú"); cNote.Width = 110;
            grid.DataSource = rows;

            PanelControl bottom = new PanelControl();
            bottom.Dock = DockStyle.Bottom;
            bottom.Height = 40;
            SimpleButton btnSave = Btn(bottom, "Lưu", 870, (s, e) => Save());
            Btn(bottom, "Nhập file…", 760, (s, e) => ImportFile());
            Btn(bottom, "Xuất file…", 650, (s, e) => ExportFile());
            LabelControl hint = new LabelControl();
            hint.Text = "Nhập được tệp xuất từ màn nối chỉ số mẫu M3 (Đồng bộ KSK) — cùng mã chỉ tiêu.";
            hint.Location = new Point(10, 13);
            hint.Appearance.ForeColor = Color.DimGray;
            bottom.Controls.Add(hint);
            this.Controls.Add(grid);
            this.Controls.Add(bottom);
            this.AcceptButton = btnSave;
        }

        private SimpleButton Btn(Control parent, string text, int x, EventHandler click)
        {
            SimpleButton b = new SimpleButton();
            b.Text = text;
            b.Location = new Point(x, 8);
            b.Size = new Size(100, 24);
            b.Click += click;
            parent.Controls.Add(b);
            return b;
        }

        private void RefreshUnit()
        {
            foreach (Row r in rows)
            {
                Source s;
                r.UnitName = (!string.IsNullOrEmpty(r.SourceKey) && sourceByKey.TryGetValue(r.SourceKey, out s)) ? s.Unit : "";
            }
            view.RefreshData();
        }

        private void ApplyJson(string json)
        {
            var map = KskProDriverM6ClsFields.Parse(json);
            foreach (Row r in rows)
            {
                KskProDriverM6ClsFields.MapItem it;
                if (map.TryGetValue(r.FieldCode, out it))
                {
                    r.SourceKey = it.SourceKind.Trim() + "|" + it.TestIndexCode.Trim();
                    r.Note = it.Note;
                }
                else { r.SourceKey = null; r.Note = null; }
            }
            RefreshUnit();
        }

        private string BuildJson()
        {
            view.CloseEditor();
            view.UpdateCurrentRow();
            var f = new KskProDriverM6ClsFields.MapFile
            {
                Version = KskProDriverM6ClsFields.MAP_VERSION,
                FormCode = KskProDriverM6ClsFields.FORM_CODE,
                Items = new List<KskProDriverM6ClsFields.MapItem>()
            };
            foreach (Row r in rows)
            {
                if (string.IsNullOrWhiteSpace(r.SourceKey)) continue;
                int vach = r.SourceKey.IndexOf('|');
                f.Items.Add(new KskProDriverM6ClsFields.MapItem
                {
                    FieldCode = r.FieldCode,
                    SourceKind = vach > 0 ? r.SourceKey.Substring(0, vach) : KskProDriverM6ClsFields.KIND_TEST_INDEX,
                    TestIndexCode = vach > 0 ? r.SourceKey.Substring(vach + 1) : r.SourceKey,
                    // Lưu qua ControlState (câu lệnh ghép chuỗi) -> bỏ dấu nháy đơn.
                    Note = string.IsNullOrWhiteSpace(r.Note) ? null : r.Note.Replace("'", "’")
                });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(f);
        }

        private void Save()
        {
            try
            {
                ResultJson = BuildJson();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void ExportFile()
        {
            try
            {
                using (SaveFileDialog d = new SaveFileDialog())
                {
                    d.Filter = "JSON (*.json)|*.json";
                    d.FileName = "NoiChiSoCLS_Mau06.json";
                    if (d.ShowDialog(this) != DialogResult.OK) return;
                    System.IO.File.WriteAllText(d.FileName, BuildJson(), new System.Text.UTF8Encoding(false));
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); XtraMessageBox.Show(this, "Xuất file thất bại: " + ex.Message); }
        }

        private void ImportFile()
        {
            try
            {
                using (OpenFileDialog d = new OpenFileDialog())
                {
                    d.Filter = "JSON (*.json)|*.json|Tất cả (*.*)|*.*";
                    if (d.ShowDialog(this) != DialogResult.OK) return;
                    string json = System.IO.File.ReadAllText(d.FileName);
                    var map = KskProDriverM6ClsFields.Parse(json);
                    if (map.Count == 0)
                    {
                        XtraMessageBox.Show(this, "Tệp không có cặp nối nào dùng được cho Mẫu 06.");
                        return;
                    }
                    ApplyJson(json);
                    XtraMessageBox.Show(this, "Đã nhập " + map.Count + " cặp nối. Bấm Lưu để ghi lại.");
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); XtraMessageBox.Show(this, "Nhập file thất bại: " + ex.Message); }
        }
    }
}
