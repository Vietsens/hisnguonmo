/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Popup "Thông tin quan hệ gia đình": GridControl nhập nhiều thành viên gia đình.
 * Lưu vào HIS_KSK_RELATION (nav collection của HIS_KSK_PROFILE):
 *   RELATION_CODE (MA_QUANHE_831), RELATION_NAME (TEN_QUAN_HE), IS_ADOPTED (phân biệt mã 18),
 *   RELATED_PERSON_NAME (họ tên), IDENTITY_CODE (mã định danh), PHONE, MOBILE, IS_GUARDIAN (giám hộ).
 * Cột: Loại quan hệ | Mã định danh | Họ tên | Điện thoại | Di động | Giám hộ | Xóa. AutoWidth = true; nút Lưu góc dưới phải.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionQD831.Run
{
    public partial class frmFamilyRelation : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>1 loại quan hệ cố định (MA_QUANHE_831). Ten là khóa hiển thị (duy nhất, phân biệt mã 18).</summary>
        internal class RelationItem
        {
            public string Ma { get; set; }
            public string Ten { get; set; }
            public bool Adopted { get; set; }
            public RelationItem(string ma, string ten, bool adopted) { Ma = ma; Ten = ten; Adopted = adopted; }
        }

        /// <summary>1 dòng thành viên gia đình (bind grid).</summary>
        public class FamilyRelationRow
        {
            public long Id { get; set; }                  // giữ ID khi sửa (0 = thêm mới)
            public string RelationName { get; set; }       // TEN_QUAN_HE — khóa lookup
            public string IdentityCode { get; set; }        // mã định danh
            public string RelatedPersonName { get; set; }   // họ tên người nhà
            public string Phone { get; set; }
            public string Mobile { get; set; }
            public bool IsGuardian { get; set; }            // giám hộ
        }

        private readonly BindingList<FamilyRelationRow> rows = new BindingList<FamilyRelationRow>();

        /// <summary>Danh sách HIS_KSK_RELATION sau khi Lưu (form cha gắn vào SDO). null nếu Hủy.</summary>
        public List<HIS_KSK_RELATION> Result { get; private set; }

        public frmFamilyRelation()
        {
            InitializeComponent();
            try
            {
                try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location); }
                catch (Exception exIcon) { LogSystem.Warn(exIcon); }
                InitGrid();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Mở lại với danh sách đã có (đã lưu / đã nhập).</summary>
        public frmFamilyRelation(List<HIS_KSK_RELATION> initial) : this()
        {
            try
            {
                rows.Clear(); // bỏ dòng trống seed từ ctor mặc định
                if (initial != null)
                    foreach (var r in initial)
                        if (r != null)
                            rows.Add(new FamilyRelationRow
                            {
                                Id = r.ID,
                                RelationName = ResolveName(r),
                                IdentityCode = r.IDENTITY_CODE,
                                RelatedPersonName = r.RELATED_PERSON_NAME,
                                Phone = r.PHONE,
                                Mobile = r.MOBILE,
                                IsGuardian = r.IS_GUARDIAN.HasValue && r.IS_GUARDIAN.Value == 1
                            });
                EnsureTrailingEmptyRow();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void InitGrid()
        {
            var gv = this.gridViewFamily;
            gv.OptionsView.ShowGroupPanel = false;
            gv.OptionsView.ColumnAutoWidth = true;            // AutoWidth = true
            gv.OptionsView.NewItemRowPosition = NewItemRowPosition.None; // tự thêm dòng khi chọn loại quan hệ (không dùng new-item-row)
            gv.OptionsBehavior.Editable = true;
            gv.CellValueChanged -= gridViewFamily_CellValueChanged;
            gv.CellValueChanged += gridViewFamily_CellValueChanged;
            gv.Columns.Clear();
            this.gridControlFamily.RepositoryItems.Clear();

            // Loại quan hệ — GridLookUpEdit cố định (khóa theo TEN_QUAN_HE duy nhất)
            var repoRel = new RepositoryItemGridLookUpEdit();
            repoRel.DataSource = RelationList();
            repoRel.DisplayMember = "Ten";
            repoRel.ValueMember = "Ten";
            repoRel.NullText = "";
            repoRel.View.OptionsView.ShowColumnHeaders = false;
            repoRel.PopulateViewColumns();
            this.gridControlFamily.RepositoryItems.Add(repoRel);

            // Giám hộ — CheckEdit
            var repoChk = new RepositoryItemCheckEdit();
            this.gridControlFamily.RepositoryItems.Add(repoChk);

            // Nút xóa dòng
            var repoDel = new RepositoryItemButtonEdit();
            repoDel.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            repoDel.Buttons.Clear();
            repoDel.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(
                DevExpress.XtraEditors.Controls.ButtonPredefines.Delete));
            repoDel.ButtonClick += repoDel_ButtonClick;
            this.gridControlFamily.RepositoryItems.Add(repoDel);

            AddCol(gv, "RelationName", "Loại quan hệ", repoRel, 170);
            AddCol(gv, "IdentityCode", "Mã định danh", null, 130);
            AddCol(gv, "RelatedPersonName", "Họ tên", null, 180);
            AddCol(gv, "Phone", "Điện thoại", null, 100);
            AddCol(gv, "Mobile", "Di động", null, 100);
            var cGuard = AddCol(gv, "IsGuardian", "Giám hộ", repoChk, 65);
            cGuard.OptionsColumn.FixedWidth = true;

            // Cột Xóa (unbound)
            var cDel = gv.Columns.AddVisible("colDelete");
            cDel.Caption = "Xóa";
            cDel.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            cDel.OptionsColumn.AllowEdit = true;
            cDel.OptionsColumn.ReadOnly = false;
            cDel.OptionsColumn.FixedWidth = true;
            cDel.Width = 50;
            cDel.ColumnEdit = repoDel;
            cDel.VisibleIndex = gv.Columns.Count - 1;

            this.gridControlFamily.DataSource = rows;
            EnsureTrailingEmptyRow();
        }

        /// <summary>Luôn có 1 dòng trống ở cuối để nhập tiếp (khi mở, sau khi nạp, sau khi tự thêm).</summary>
        private void EnsureTrailingEmptyRow()
        {
            try
            {
                if (rows.Count == 0) { rows.Add(new FamilyRelationRow()); return; }
                var last = rows[rows.Count - 1];
                if (!string.IsNullOrWhiteSpace(last.RelationName) || !string.IsNullOrWhiteSpace(last.RelatedPersonName))
                    rows.Add(new FamilyRelationRow());
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Chọn "Loại quan hệ" ở dòng cuối -> tự thêm 1 dòng trống mới bên dưới.</summary>
        private void gridViewFamily_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column == null || e.Column.FieldName != "RelationName") return;
                if (string.IsNullOrWhiteSpace(e.Value as string)) return;
                var row = this.gridViewFamily.GetRow(e.RowHandle) as FamilyRelationRow;
                if (row != null && rows.IndexOf(row) == rows.Count - 1)
                    rows.Add(new FamilyRelationRow());
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private GridColumn AddCol(GridView gv, string field, string caption, RepositoryItem edit, int width)
        {
            var c = gv.Columns.AddVisible(field);
            c.Caption = caption;
            c.Width = width;
            if (edit != null) c.ColumnEdit = edit;
            c.VisibleIndex = gv.Columns.Count - 1;
            return c;
        }

        private void repoDel_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var gv = this.gridViewFamily;
                int handle = gv.FocusedRowHandle;
                if (handle < 0) return;
                gv.CloseEditor();
                var row = gv.GetRow(handle) as FamilyRelationRow;
                if (row != null) rows.Remove(row);
                EnsureTrailingEmptyRow(); // luôn còn 1 dòng trống để nhập tiếp
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        // ==================== Danh mục quan hệ cố định (MA_QUANHE_831) ====================
        private static List<RelationItem> RelationList()
        {
            return new List<RelationItem>
            {
                new RelationItem("01", "Chủ hộ", false),
                new RelationItem("17", "Vợ/Chồng chủ hộ", false),
                new RelationItem("08", "Con đẻ", false),
                new RelationItem("18", "Con dâu/Con rể", false),
                new RelationItem("19", "Bố/Mẹ", false),
                new RelationItem("20", "Ông/Bà", false),
                new RelationItem("12", "Cháu", false),
                new RelationItem("13", "Quan hệ khác", false),
                new RelationItem("21", "Anh/Chị", false),
                new RelationItem("18", "Con nuôi", true),
            };
        }

        private static RelationItem FindByName(string ten)
        {
            if (string.IsNullOrWhiteSpace(ten)) return null;
            foreach (var it in RelationList()) if (it.Ten == ten) return it;
            return null;
        }

        /// <summary>Suy tên hiển thị từ 1 HIS_KSK_RELATION đã lưu (ưu tiên RELATION_NAME; fallback theo mã + IS_ADOPTED).</summary>
        private static string ResolveName(HIS_KSK_RELATION r)
        {
            if (r == null) return null;
            if (!string.IsNullOrWhiteSpace(r.RELATION_NAME)) return r.RELATION_NAME;
            bool adopted = r.IS_ADOPTED.HasValue && r.IS_ADOPTED.Value == 1;
            foreach (var it in RelationList())
                if (it.Ma == r.RELATION_CODE && it.Adopted == adopted) return it.Ten;
            return null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.gridViewFamily.CloseEditor();
                this.gridViewFamily.UpdateCurrentRow();

                // Bỏ dòng trống hoàn toàn.
                var data = new List<FamilyRelationRow>();
                foreach (var r in rows)
                {
                    if (r == null) continue;
                    bool empty = string.IsNullOrWhiteSpace(r.RelationName)
                        && string.IsNullOrWhiteSpace(r.IdentityCode)
                        && string.IsNullOrWhiteSpace(r.RelatedPersonName)
                        && string.IsNullOrWhiteSpace(r.Phone)
                        && string.IsNullOrWhiteSpace(r.Mobile)
                        && !r.IsGuardian;
                    if (!empty) data.Add(r);
                }

                // Bắt buộc: Loại quan hệ + Họ tên với mọi dòng có dữ liệu.
                for (int i = 0; i < data.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(data[i].RelationName) || string.IsNullOrWhiteSpace(data[i].RelatedPersonName))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(
                            string.Format("Dòng {0}: bắt buộc nhập Loại quan hệ và Họ tên.", i + 1),
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Map -> HIS_KSK_RELATION (KSK_PROFILE_ID để BE gán theo hồ sơ).
                var list = new List<HIS_KSK_RELATION>();
                foreach (var r in data)
                {
                    var it = FindByName(r.RelationName);
                    list.Add(new HIS_KSK_RELATION
                    {
                        ID = r.Id,
                        RELATION_CODE = it != null ? it.Ma : null,
                        RELATION_NAME = r.RelationName,
                        IS_ADOPTED = (short)(it != null && it.Adopted ? 1 : 0),
                        RELATED_PERSON_NAME = r.RelatedPersonName,
                        IDENTITY_CODE = r.IdentityCode,
                        PHONE = r.Phone,
                        MOBILE = r.Mobile,
                        IS_GUARDIAN = (short)(r.IsGuardian ? 1 : 0),
                        IS_ACTIVE = 1,
                        IS_DELETE = 0
                    });
                }

                this.Result = list;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }
    }
}
