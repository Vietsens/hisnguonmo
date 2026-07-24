/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tồn kho nhập xuất tồn — cây kết quả tự dựng trong plugin (không dùng UC lib HIS.UC.*):
 * - Thuốc/Vật tư (treeListPivot): 1 dòng = 1 loại (gộp tất cả kho đã chọn), các kho hiển thị
 *   theo hàng ngang — mỗi kho đã tích = 1 cột động STOCK_AMOUNT_{mediStockId} (số lượng tồn tại kho đó).
 *   Không còn dòng chi tiết lô; giữ node nhóm loại để nhóm.
 * - Máu (treeListBlood): cây nhóm máu → túi máu, bind thẳng HisBloodInStockSDO.
 */
using HIS.Desktop.Plugins.AnticipateCreateV2.ADO;
using Inventec.Desktop.Common.Message;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AnticipateCreateV2
{
    public partial class UCAnticipateCreateV2 : HIS.Desktop.Utility.UserControlBase
    {
        #region ---PivotTree fields
        // Cây kết quả Thuốc/Vật tư (dùng chung 1 TreeList, cột build lại theo loại + kho mỗi lần tìm)
        internal DevExpress.XtraTreeList.TreeList treeListPivot;
        // Cây kết quả Máu (cột cố định)
        internal DevExpress.XtraTreeList.TreeList treeListBlood;

        const string STOCK_AMOUNT_FIELD_PREFIX = "STOCK_AMOUNT_";

        // vCong 52461 — 2 cột nhập trực tiếp trên cây: SL dự trù + Ghi chú.
        const string ANTICIPATE_QTY_FIELD = "ANTICIPATE_QTY";
        const string ANTICIPATE_NOTE_FIELD = "ANTICIPATE_NOTE";
        // Editor cho 2 cột nhập (tạo 1 lần, gán vào ColumnEdit khi build cột).
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit riAnticipateQty;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit riAnticipateNote;
        #endregion

        /// <summary>
        /// Khởi tạo 2 TreeList kết quả (gọi 1 lần trong constructor).
        /// Cột của treeListPivot build lại mỗi lần tìm (BuildPivotColumns); cột treeListBlood cố định.
        /// </summary>
        private void InitPivotTrees()
        {
            try
            {
                treeListPivot = new DevExpress.XtraTreeList.TreeList();
                treeListPivot.Name = "treeListPivot";
                treeListPivot.KeyFieldName = "NodeId";
                treeListPivot.ParentFieldName = "ParentNodeId";
                // vCong 52461 — cho phép nhập trực tiếp; chỉ 2 cột SL dự trù/Ghi chú AllowEdit=true, còn lại false.
                treeListPivot.OptionsBehavior.Editable = true;
                treeListPivot.OptionsBehavior.AutoPopulateColumns = false;
                treeListPivot.OptionsView.AutoWidth = false;   // nhiều cột (n kho) -> scroll ngang
                treeListPivot.OptionsView.EnableAppearanceEvenRow = true;
                treeListPivot.OptionsMenu.EnableColumnMenu = true;
                treeListPivot.CustomUnboundColumnData += pivotTree_CustomUnboundColumnData;
                treeListPivot.DoubleClick += pivotTree_DoubleClick;

                // Editor cho 2 cột nhập trực tiếp
                riAnticipateQty = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
                riAnticipateQty.MinValue = 0;
                riAnticipateQty.MaxValue = 9999999999;
                riAnticipateQty.IsFloatValue = true;
                riAnticipateQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                riAnticipateQty.DisplayFormat.FormatString = "#,##0.##";
                riAnticipateQty.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                riAnticipateQty.EditFormat.FormatString = "#,##0.##";
                riAnticipateNote = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
                treeListPivot.RepositoryItems.Add(riAnticipateQty);
                treeListPivot.RepositoryItems.Add(riAnticipateNote);

                treeListBlood = new DevExpress.XtraTreeList.TreeList();
                treeListBlood.Name = "treeListBlood";
                treeListBlood.KeyFieldName = "NodeId";
                treeListBlood.ParentFieldName = "ParentNodeId";
                treeListBlood.OptionsBehavior.Editable = false;
                treeListBlood.OptionsBehavior.AutoPopulateColumns = false;
                treeListBlood.OptionsView.AutoWidth = false;
                treeListBlood.OptionsView.EnableAppearanceEvenRow = true;
                treeListBlood.CustomUnboundColumnData += bloodType_CustomUnboundColumnData;
                BuildBloodColumns();

                // Không hiển thị cây ở đây — constructor gọi InitDutruGrid (tạo split) rồi ShowResultControl.
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Đưa control kết quả (cây thuốc/vật tư hoặc máu) vào vùng cây (Panel1 của split nếu có).</summary>
        private void ShowResultControl(Control control)
        {
            try
            {
                var host = GetTreeHostPanel();
                if (control == null || host == null) return;
                host.Controls.Clear();
                host.Controls.Add(control);
                control.Dock = DockStyle.Fill;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Thêm 1 cột vào TreeList theo thứ tự gọi. unboundType != Bound → cột unbound (điền qua CustomUnboundColumnData).</summary>
        private DevExpress.XtraTreeList.Columns.TreeListColumn AddTreeColumn(
            DevExpress.XtraTreeList.TreeList tree, string caption, string fieldName, int width,
            DevExpress.XtraTreeList.Data.UnboundColumnType unboundType, string formatString)
        {
            var col = tree.Columns.Add();
            col.Caption = caption;
            col.FieldName = fieldName;
            col.Width = width;
            col.OptionsColumn.AllowEdit = false;
            col.UnboundType = unboundType;
            if (!string.IsNullOrEmpty(formatString))
            {
                col.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
                col.Format.FormatString = formatString;
            }
            col.VisibleIndex = tree.Columns.Count - 1;
            return col;
        }

        /// <summary>
        /// vCong 52461 — Build cột cây Thuốc/Vật tư cho màn Tạo dự trù:
        /// thông tin loại → số liệu thầu → nhập/xuất/tồn kỳ (Tồn đầu/Nhập mới/Số sử dụng/Tồn cuối) →
        /// Xuất nhiều nhất → Giá sau VAT → SL DỰ TRÙ (nhập) → n cột tồn theo kho (tham khảo) → Ghi chú (nhập).
        /// Gọi mỗi lần tính (tập kho có thể thay đổi).
        /// </summary>
        private void BuildPivotColumns(bool isMedicine)
        {
            var bound = DevExpress.XtraTreeList.Data.UnboundColumnType.Bound;
            var unbDec = DevExpress.XtraTreeList.Data.UnboundColumnType.Decimal;
            var unbInt = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;   // TreeList UnboundColumnType KHÔNG có Integer → dùng Object cho cột tháng (int)
            var unbStr = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
            var lang = Base.ResourceLangManager.LanguageUCAnticipateCreateV2;
            var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

            treeListPivot.BeginUpdate();
            try
            {
                treeListPivot.Columns.Clear();

                // ----- Thông tin loại (bound theo cây tồn gộp) -----
                // vCong 52461 — cột Mã, Tên cố định bên trái khi cuộn ngang (nhiều cột số liệu + n cột kho)
                var colTypeCode = AddTreeColumn(treeListPivot,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", lang, culture),
                    isMedicine ? "MEDICINE_TYPE_CODE" : "MATERIAL_TYPE_CODE", 90, bound, null);
                colTypeCode.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
                var colTypeName = AddTreeColumn(treeListPivot,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", lang, culture),
                    isMedicine ? "MEDICINE_TYPE_NAME" : "MATERIAL_TYPE_NAME", 220, bound, null);
                colTypeName.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
                if (isMedicine)
                {
                    AddTreeColumn(treeListPivot,
                        Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_ACTIVE_INGR_BHYT_NAME", lang, culture),
                        "ACTIVE_INGR_BHYT_NAME", 150, bound, null);
                }
                AddTreeColumn(treeListPivot,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_CONCENTRA", lang, culture),
                    "CONCENTRA", 100, bound, null);
                if (isMedicine)
                {
                    AddTreeColumn(treeListPivot,
                        Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_USE_FORM_NAME", lang, culture),
                        "MEDICINE_USE_FORM_NAME", 100, bound, null);
                }
                AddTreeColumn(treeListPivot,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_SERVICE_UNIT_NAME", lang, culture),
                    "SERVICE_UNIT_NAME", 60, bound, null);
                AddTreeColumn(treeListPivot, "Hãng SX", "MANUFACTURER_DISPLAY", 150, unbStr, null);
                AddTreeColumn(treeListPivot, "Nhà cung cấp", "SUPPLIER_DISPLAY", 150, unbStr, null);

                // ----- Cột danh mục bổ sung (vCong 52461): Nước SX, Số đăng ký, Số QĐ thầu, Ngày QĐ thầu -----
                AddTreeColumn(treeListPivot, "Nước SX", "NATIONAL_DISPLAY", 90, unbStr, null);
                AddTreeColumn(treeListPivot, "Số đăng ký", "REGISTER_DISPLAY", 110, unbStr, null);
                AddTreeColumn(treeListPivot, "Số QĐ", "BID_NUMBER_DISPLAY", 100, unbStr, null);
                AddTreeColumn(treeListPivot, "Ngày QĐ", "BID_DATE_DISPLAY", 90, unbStr, null);
                AddTreeColumn(treeListPivot, "Quy cách", "PACKING_DISPLAY", 110, unbStr, null);

                // ----- Số liệu thầu (gộp mọi thầu còn hiệu lực) -----
                AddTreeColumn(treeListPivot, "SL thầu", "BID_AMOUNT_DISPLAY", 90, unbDec, "#,##0.##");
                AddTreeColumn(treeListPivot, "Thầu đã nhập", "BID_IMPORTED_DISPLAY", 90, unbDec, "#,##0.##");
                AddTreeColumn(treeListPivot, "Thầu còn lại", "BID_REMAIN_DISPLAY", 90, unbDec, "#,##0.##");

                // ----- Nhập/xuất/tồn trong kỳ -----
                AddTreeColumn(treeListPivot, "Tồn đầu kỳ", "OPEN_DISPLAY", 90, unbDec, "#,##0.##");
                AddTreeColumn(treeListPivot, "Nhập mới", "NEW_IMPORT_DISPLAY", 90, unbDec, "#,##0.##");
                AddTreeColumn(treeListPivot, "Số sử dụng", "USED_DISPLAY", 90, unbDec, "#,##0.##");
                AddTreeColumn(treeListPivot, "Tồn cuối kỳ", "CLOSE_DISPLAY", 90, unbDec, "#,##0.##");

                // ----- Xuất nhiều nhất (theo tháng) + Giá sau VAT -----
                AddTreeColumn(treeListPivot, "Xuất nhiều nhất", "MAX_EXPORT_DISPLAY", 100, unbDec, "#,##0.##");
                AddTreeColumn(treeListPivot, "Tháng XC", "MAX_EXPORT_MONTH_DISPLAY", 70, unbInt, null);
                AddTreeColumn(treeListPivot, "Giá sau VAT", "EXP_PRICE_VAT_DISPLAY", 100, unbDec, "#,##0");

                // ----- SL DỰ TRÙ (nhập trực tiếp) -----
                var colQty = AddTreeColumn(treeListPivot, "SL dự trù", ANTICIPATE_QTY_FIELD, 90, unbDec, "#,##0.##");
                colQty.OptionsColumn.AllowEdit = true;
                colQty.ColumnEdit = riAnticipateQty;

                // ----- Ghi chú (nhập trực tiếp) — vCong 52461: CHƯA lưu DB -----
                var colNote = AddTreeColumn(treeListPivot, "Ghi chú", ANTICIPATE_NOTE_FIELD, 150, unbStr, null);
                colNote.OptionsColumn.AllowEdit = true;
                colNote.ColumnEdit = riAnticipateNote;

                // ----- Cột kho động (CUỐI bảng): mỗi kho đang tích = 1 cột (tồn của loại tại kho — tham khảo) -----
                if (this._MediStocks != null)
                {
                    foreach (var stock in this._MediStocks.Where(p => p.IsCheck == true))
                    {
                        AddTreeColumn(treeListPivot, stock.MEDI_STOCK_NAME,
                            STOCK_AMOUNT_FIELD_PREFIX + stock.ID, 90, unbDec, "#,##0.##");
                    }
                }
            }
            finally
            {
                treeListPivot.EndUpdate();
            }
        }

        /// <summary>Cột cây Máu — theo cấu hình cột của màn hình cũ (bỏ Số hóa đơn: SDO không có field).</summary>
        private void BuildBloodColumns()
        {
            var bound = DevExpress.XtraTreeList.Data.UnboundColumnType.Bound;
            var unbStr = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
            var lang = Base.ResourceLangManager.LanguageUCAnticipateCreateV2;
            var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

            treeListBlood.BeginUpdate();
            try
            {
                treeListBlood.Columns.Clear();
                AddTreeColumn(treeListBlood,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", lang, culture),
                    "BloodTypeCode", 90, bound, null);
                AddTreeColumn(treeListBlood,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", lang, culture),
                    "BloodTypeName", 200, bound, null);
                AddTreeColumn(treeListBlood,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_BLOOD_ABO_CODE", lang, culture),
                    "BloodAboCode", 70, bound, null);
                AddTreeColumn(treeListBlood,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_BLOOD_RH_CODE", lang, culture),
                    "BloodRhCode", 40, bound, null);
                AddTreeColumn(treeListBlood, "Mã túi máu", "BloodCode", 170, bound, null);
                AddTreeColumn(treeListBlood, "Số lô", "PackageNumber", 100, bound, null);
                AddTreeColumn(treeListBlood, "Hạn sử dụng", "ExpiredDateStr", 140, unbStr, null);
                AddTreeColumn(treeListBlood, "Thời gian đóng gói", "PackingTimeStr", 140, unbStr, null);
                AddTreeColumn(treeListBlood, "Ngày còn lại", "ExpiredDaysLeft", 100, bound, null);
                AddTreeColumn(treeListBlood, "Nhà cung cấp", "SupplierName", 200, bound, null);
                AddTreeColumn(treeListBlood,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__BLOOD_IN_STOCK__COLUMN_VOLUME", lang, culture),
                    "Volume", 100, bound, null);
                AddTreeColumn(treeListBlood,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_AMOUNT", lang, culture),
                    "Amount", 80, bound, "#,##0.##");
            }
            finally
            {
                treeListBlood.EndUpdate();
            }
        }

        /// <summary>
        /// Bind cây Thuốc/Vật tư: lọc bỏ dòng lô (chỉ giữ node nhóm + node loại),
        /// áp bộ lọc cảnh báo tồn tối thiểu (nếu bật), build lại cột rồi gán DataSource.
        /// </summary>
        private void BindPivotTree(bool isMedicine)
        {
            try
            {
                if (treeListPivot == null) return;

                // "Ẩn dòng nhóm": chỉ giữ dòng loại thật (IS_LEAF=1) và bỏ liên kết cha-con → bảng phẳng
                bool hideGroup = pluginChkHideGroup != null && pluginChkHideGroup.Checked;

                // DataSource phải là List đúng kiểu (không dùng List<object>) để bound column resolve được property
                object dataSource;
                if (isMedicine)
                {
                    IEnumerable<HisMedicineInStockSDO> src = (lstMediInStocks ?? new List<HisMedicineInStockSDO>())
                        .Where(o => o != null && o.isTypeNode);
                    if (IsAlertMinStockChecked && chkAlertNew != null && chkAlertNew.Enabled)
                    {
                        src = FilterAlertMinStock(src.ToList(),
                            o => (o.IS_LEAF ?? 0) == 1 && o.ALERT_MIN_IN_STOCK.HasValue && o.TotalAmount.HasValue
                                 && o.AvailableAmount.HasValue && o.ALERT_MIN_IN_STOCK >= o.AvailableAmount,
                            o => o.NodeId, o => o.ParentNodeId);
                    }
                    // vCong 52461 — chế độ Thầu: chỉ giữ loại thuộc gói thầu đã chọn (+ node nhóm tổ tiên)
                    if (IsSupplyMode && allowedTypeIdsByBid != null)
                    {
                        var allowed = allowedTypeIdsByBid;
                        src = FilterAlertMinStock(src.ToList(),
                            o => (o.IS_LEAF ?? 0) == 1 && allowed.Contains(o.MEDICINE_TYPE_ID),
                            o => o.NodeId, o => o.ParentNodeId);
                    }
                    if (hideGroup)
                    {
                        src = src.Where(o => (o.IS_LEAF ?? 0) == 1);
                    }
                    dataSource = src.ToList();
                }
                else
                {
                    IEnumerable<HisMaterialInStockSDO> src = (lstMateInStocks ?? new List<HisMaterialInStockSDO>())
                        .Where(o => o != null && o.isTypeNode);
                    if (IsAlertMinStockChecked && chkAlertNew != null && chkAlertNew.Enabled)
                    {
                        src = FilterAlertMinStock(src.ToList(),
                            o => (o.IS_LEAF ?? 0) == 1 && o.ALERT_MIN_IN_STOCK.HasValue && o.TotalAmount.HasValue
                                 && o.AvailableAmount.HasValue && o.ALERT_MIN_IN_STOCK >= o.AvailableAmount,
                            o => o.NodeId, o => o.ParentNodeId);
                    }
                    // vCong 52461 — chế độ Thầu: chỉ giữ loại thuộc gói thầu đã chọn (+ node nhóm tổ tiên)
                    if (IsSupplyMode && allowedTypeIdsByBid != null)
                    {
                        var allowed = allowedTypeIdsByBid;
                        src = FilterAlertMinStock(src.ToList(),
                            o => (o.IS_LEAF ?? 0) == 1 && allowed.Contains(o.MATERIAL_TYPE_ID),
                            o => o.NodeId, o => o.ParentNodeId);
                    }
                    if (hideGroup)
                    {
                        src = src.Where(o => (o.IS_LEAF ?? 0) == 1);
                    }
                    dataSource = src.ToList();
                }

                string findText = treeListPivot.FindFilterText;
                treeListPivot.BeginUpdate();
                try
                {
                    treeListPivot.DataSource = null;
                    // Ẩn nhóm → bỏ ParentFieldName để mọi dòng thành root (danh sách phẳng, không đụng dữ liệu gốc)
                    treeListPivot.ParentFieldName = hideGroup ? "" : "ParentNodeId";
                    BuildPivotColumns(isMedicine);
                    treeListPivot.DataSource = dataSource;
                }
                finally
                {
                    treeListPivot.EndUpdate();
                }
                ApplyCollapseMode();
                if (!string.IsNullOrEmpty(findText)) treeListPivot.ApplyFindFilter(findText);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Giữ các node loại thỏa điều kiện cảnh báo + toàn bộ node nhóm tổ tiên của chúng
        /// (đi ngược ParentNodeId nhiều cấp).
        /// </summary>
        private IEnumerable<T> FilterAlertMinStock<T>(List<T> all, Func<T, bool> isAlertLeaf,
            Func<T, string> getNodeId, Func<T, string> getParentNodeId)
        {
            var byNodeId = new Dictionary<string, T>();
            foreach (var item in all)
            {
                string id = getNodeId(item);
                if (!string.IsNullOrEmpty(id) && !byNodeId.ContainsKey(id)) byNodeId[id] = item;
            }
            var keptIds = new HashSet<string>();
            foreach (var leaf in all.Where(isAlertLeaf))
            {
                string id = getNodeId(leaf);
                // Giữ node + walk ngược lên tổ tiên nhóm
                while (!string.IsNullOrEmpty(id) && keptIds.Add(id))
                {
                    T node;
                    if (!byNodeId.TryGetValue(id, out node)) break;
                    id = getParentNodeId(node);
                }
            }
            return all.Where(o => keptIds.Contains(getNodeId(o)));
        }

        /// <summary>Bind cây Máu (giữ cấu trúc nhóm máu → túi máu).</summary>
        private void BindBloodTree()
        {
            try
            {
                if (treeListBlood == null) return;
                treeListBlood.BeginUpdate();
                try
                {
                    treeListBlood.DataSource = lstBloodInStocks ?? new List<HisBloodInStockSDO>();
                }
                finally
                {
                    treeListBlood.EndUpdate();
                }
                treeListBlood.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Áp chế độ hiển thị theo radio: Chi tiết = bung node nhóm, Thu gọn tất cả = đóng node nhóm.</summary>
        private void ApplyCollapseMode()
        {
            try
            {
                if (treeListPivot == null) return;
                if (pluginChkCollapseAll != null && pluginChkCollapseAll.Checked)
                    treeListPivot.CollapseAll();
                else
                    treeListPivot.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Điền giá trị các cột unbound của cây Thuốc/Vật tư.
        /// Chỉ dòng LOẠI thật (IS_LEAF=1) có số liệu; dòng nhóm để trống.
        /// - Cột chung Tổng nhập/Tổng xuất/Tồn cuối kỳ: dict tổng hợp theo LOẠI (cộng các kho đã chọn).
        /// - Cột kho động STOCK_AMOUNT_{id}: dict theo cặp (kho, loại) → AMOUNT (tồn hiện tại tại kho).
        /// </summary>
        private void pivotTree_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                var medi = e.Row as HisMedicineInStockSDO;
                var mate = medi == null ? e.Row as HisMaterialInStockSDO : null;
                if (medi == null && mate == null) return;

                bool isTypeLeaf = medi != null ? (medi.IS_LEAF ?? 0) == 1 : (mate.IS_LEAF ?? 0) == 1;
                long typeId = medi != null ? medi.MEDICINE_TYPE_ID : mate.MATERIAL_TYPE_ID;
                string fieldName = e.Column.FieldName;

                var dicPerStock = medi != null ? dicMediImpExp : dicMateImpExp;
                var dicAntc = medi != null ? dicMediAnticipate : dicMateAnticipate;
                var dicQty = medi != null ? dicMediAnticipateQty : dicMateAnticipateQty;
                var dicNote = medi != null ? dicMediAnticipateNote : dicMateAnticipateNote;

                // ---- Ghi giá trị người dùng nhập (SL dự trù / Ghi chú) — chỉ dòng loại ----
                if (e.IsSetData)
                {
                    if (!isTypeLeaf) return;
                    if (fieldName == ANTICIPATE_QTY_FIELD)
                    {
                        decimal q;
                        dicQty[typeId] = (e.Value != null && decimal.TryParse(e.Value.ToString(), out q)) ? (decimal?)q : null;
                        // Capture ngữ cảnh thầu/giá lúc nhập (dùng khi Lưu) — vCong 52461
                        CaptureAnticipateContext(medi != null, typeId);
                    }
                    else if (fieldName == ANTICIPATE_NOTE_FIELD)
                    {
                        dicNote[typeId] = e.Value as string;
                    }
                    return;
                }

                if (!e.IsGetData) return;
                if (!isTypeLeaf) return;   // dòng nhóm: các cột số để trống

                ADO.AnticipateRowADO ar = null;
                if (dicAntc != null) dicAntc.TryGetValue(typeId, out ar);

                // vCong 52461 — chế độ Thầu: cột SL thầu/đã nhập/còn lại/giá VAT/NCC lấy theo gói thầu đã chọn
                if (IsSupplyMode)
                {
                    decimal bidAmount, bidImported, bidRemain, priceVat;
                    string supplierName;
                    if (TryGetBidFigures(medi != null, typeId, out bidAmount, out bidImported, out bidRemain, out priceVat, out supplierName))
                    {
                        switch (fieldName)
                        {
                            case "BID_AMOUNT_DISPLAY": e.Value = bidAmount; return;
                            case "BID_IMPORTED_DISPLAY": e.Value = bidImported; return;
                            case "BID_REMAIN_DISPLAY": e.Value = bidRemain; return;
                            case "EXP_PRICE_VAT_DISPLAY": e.Value = priceVat; return;
                            case "SUPPLIER_DISPLAY": e.Value = supplierName; return;
                        }
                    }
                }

                switch (fieldName)
                {
                    case "MANUFACTURER_DISPLAY":
                        e.Value = ar != null ? ar.MANUFACTURER_NAME : null;
                        break;
                    case "SUPPLIER_DISPLAY":
                        e.Value = ar != null ? ar.SUPPLIER_NAME : null;
                        break;
                    case "BID_AMOUNT_DISPLAY":
                        e.Value = ar != null ? (object)ar.BID_AMOUNT : 0m;
                        break;
                    case "BID_IMPORTED_DISPLAY":
                        e.Value = ar != null ? (object)ar.BID_IMPORTED_AMOUNT : 0m;
                        break;
                    case "BID_REMAIN_DISPLAY":
                        e.Value = ar != null ? (object)ar.BID_REMAIN_AMOUNT : 0m;
                        break;
                    case "OPEN_DISPLAY":
                        e.Value = ar != null ? (object)ar.OPEN_QUANTITY : 0m;
                        break;
                    case "NEW_IMPORT_DISPLAY":
                        e.Value = ar != null ? (object)ar.NEW_IMPORT_QUANTITY : 0m;
                        break;
                    case "USED_DISPLAY":
                        e.Value = ar != null ? (object)ar.USED_QUANTITY : 0m;
                        break;
                    case "CLOSE_DISPLAY":
                        e.Value = ar != null ? (object)ar.CLOSE_QUANTITY : 0m;
                        break;
                    case "MAX_EXPORT_DISPLAY":
                        e.Value = ar != null ? (object)ar.MAX_EXPORT_QUANTITY : 0m;
                        break;
                    case "MAX_EXPORT_MONTH_DISPLAY":
                        e.Value = (ar != null && ar.MAX_EXPORT_MONTH > 0) ? (object)ar.MAX_EXPORT_MONTH : null;
                        break;
                    case "EXP_PRICE_VAT_DISPLAY":
                        e.Value = ar != null ? (object)ar.EXP_PRICE_VAT : 0m;
                        break;
                    case ANTICIPATE_QTY_FIELD:
                    {
                        decimal? q;
                        e.Value = (dicQty != null && dicQty.TryGetValue(typeId, out q)) ? q : null;
                        break;
                    }
                    case ANTICIPATE_NOTE_FIELD:
                    {
                        string note;
                        e.Value = (dicNote != null && dicNote.TryGetValue(typeId, out note)) ? note : null;
                        break;
                    }
                    case "PACKING_DISPLAY":
                    {
                        var dicPk = medi != null ? dicMedicineTypePacking : dicMaterialTypePacking;
                        string v; e.Value = (dicPk != null && dicPk.TryGetValue(typeId, out v)) ? v : null;
                        break;
                    }
                    case "NATIONAL_DISPLAY":
                    {
                        var dicNat = medi != null ? dicMedicineTypeNational : dicMaterialTypeNational;
                        string v; e.Value = (dicNat != null && dicNat.TryGetValue(typeId, out v)) ? v : null;
                        break;
                    }
                    case "REGISTER_DISPLAY":
                    {
                        var dicReg = medi != null ? dicMedicineTypeRegister : dicMaterialTypeRegister;
                        string v; e.Value = (dicReg != null && dicReg.TryGetValue(typeId, out v)) ? v : null;
                        break;
                    }
                    case "BID_NUMBER_DISPLAY":
                    case "BID_DATE_DISPLAY":
                    {
                        // Số/Ngày QĐ = QĐ trúng thầu (HIS_BID) — chỉ có ở chế độ Thầu (đã chọn gói thầu)
                        var bid = (IsSupplyMode && currentBidId.HasValue && ListBid != null)
                            ? ListBid.FirstOrDefault(o => o.ID == currentBidId.Value) : null;
                        if (bid == null) { e.Value = null; break; }
                        if (fieldName == "BID_NUMBER_DISPLAY")
                        {
                            e.Value = bid.BID_NUMBER;
                        }
                        else
                        {
                            string ds = bid.APPROVAL_TIME.HasValue
                                ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(bid.APPROVAL_TIME.Value) : null;
                            e.Value = (ds != null && ds.Length >= 10) ? ds.Substring(0, 10) : ds;
                        }
                        break;
                    }
                    default:
                        if (fieldName.StartsWith(STOCK_AMOUNT_FIELD_PREFIX))
                        {
                            long stockId;
                            if (long.TryParse(fieldName.Substring(STOCK_AMOUNT_FIELD_PREFIX.Length), out stockId))
                            {
                                MediStockImpExpADO ie;
                                e.Value = (dicPerStock != null && dicPerStock.TryGetValue(ImpExpKey(stockId, typeId), out ie) && ie.AMOUNT.HasValue)
                                    ? ie.AMOUNT.Value : 0m;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Điền cột hạn sử dụng / thời gian đóng gói cho cây Máu.</summary>
        private void bloodType_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                HisBloodInStockSDO data = e.Row as HisBloodInStockSDO;
                if (data == null) return;
                if (e.Column.FieldName == "ExpiredDateStr" && !e.Node.HasChildren)
                {
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)(data.ExpiredDate ?? 0));
                }
                else if (e.Column.FieldName == "PackingTimeStr" && !e.Node.HasChildren)
                {
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)(data.PackingTime ?? 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Double click dòng loại thuốc/vật tư → mở lịch sử xuất nhập (giữ hành vi màn hình cũ).</summary>
        private void pivotTree_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (treeListPivot == null || treeListPivot.FocusedNode == null) return;
                var row = treeListPivot.GetDataRecordByNode(treeListPivot.FocusedNode);
                var medi = row as HisMedicineInStockSDO;
                if (medi != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(medi.MEDICINE_TYPE_ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HistoryMedicine", this.RoomId, this.RoomTypeId, listArgs);
                    return;
                }
                var mate = row as HisMaterialInStockSDO;
                if (mate != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(mate.MATERIAL_TYPE_ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HistoryMaterial", this.RoomId, this.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Xuất Excel trực tiếp từ cây kết quả đang hiển thị (đúng 100% cấu trúc pivot,
        /// bao gồm n cột kho động) — thay cho xuất theo file template cố định.
        /// </summary>
        private void ExportActiveTreeToExcel()
        {
            try
            {
                bool hasResult = (IsMedicineChecked && lstMediInStocks != null && lstMediInStocks.Count > 0)
                              || (IsMaterialChecked && lstMateInStocks != null && lstMateInStocks.Count > 0)
                              || (IsBloodChecked && lstBloodInStocks != null && lstBloodInStocks.Count > 0);
                if (!hasResult)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa có dữ liệu để xuất", "Thông báo");
                    return;
                }

                var tree = IsBloodChecked ? treeListBlood : treeListPivot;
                if (tree == null) return;

                if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

                WaitingManager.Show();
                // Bung toàn bộ node để file Excel đầy đủ dữ liệu (không phụ thuộc trạng thái thu gọn trên màn hình)
                tree.ExpandAll();
                tree.ExportToXlsx(saveFileDialog.FileName);
                ApplyCollapseMode();
                WaitingManager.Hide();

                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
