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
using HIS.Desktop.Plugins.MediStockSummaryWithImpExp.ADO;
using Inventec.Desktop.Common.Message;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockSummaryWithImpExp
{
    public partial class UCMediStockSummaryWithImpExp : HIS.Desktop.Utility.UserControlBase
    {
        #region ---PivotTree fields
        // Cây kết quả Thuốc/Vật tư (dùng chung 1 TreeList, cột build lại theo loại + kho mỗi lần tìm)
        internal DevExpress.XtraTreeList.TreeList treeListPivot;
        // Cây kết quả Máu (cột cố định)
        internal DevExpress.XtraTreeList.TreeList treeListBlood;

        const string STOCK_AMOUNT_FIELD_PREFIX = "STOCK_AMOUNT_";
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
                treeListPivot.OptionsBehavior.Editable = false;
                treeListPivot.OptionsBehavior.AutoPopulateColumns = false;
                treeListPivot.OptionsView.AutoWidth = false;   // nhiều cột (n kho) -> scroll ngang
                treeListPivot.OptionsView.EnableAppearanceEvenRow = true;
                treeListPivot.OptionsMenu.EnableColumnMenu = true;
                treeListPivot.CustomUnboundColumnData += pivotTree_CustomUnboundColumnData;
                treeListPivot.DoubleClick += pivotTree_DoubleClick;

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

                // Mặc định hiển thị cây Thuốc (trống, chờ dữ liệu)
                ShowResultControl(treeListPivot);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Đưa control kết quả (cây thuốc/vật tư hoặc máu) vào panel bên phải.</summary>
        private void ShowResultControl(Control control)
        {
            try
            {
                if (control == null || this.panelControlMediMate == null) return;
                this.panelControlMediMate.Controls.Clear();
                this.panelControlMediMate.Controls.Add(control);
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
        /// Build lại toàn bộ cột của cây Thuốc/Vật tư: nhóm cột chung + n cột kho động theo các kho đang tích.
        /// Gọi mỗi lần tìm (tập kho có thể thay đổi giữa các lần tìm).
        /// </summary>
        private void BuildPivotColumns(bool isMedicine)
        {
            var bound = DevExpress.XtraTreeList.Data.UnboundColumnType.Bound;
            var unbDec = DevExpress.XtraTreeList.Data.UnboundColumnType.Decimal;
            var unbStr = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
            var lang = Base.ResourceLangManager.LanguageUCMediStockSummaryWithImpExp;
            var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

            treeListPivot.BeginUpdate();
            try
            {
                treeListPivot.Columns.Clear();

                // ----- Cột chung (giá trị = tổng trên các kho đã chọn, lấy từ cây tồn gộp) -----
                AddTreeColumn(treeListPivot,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", lang, culture),
                    isMedicine ? "MEDICINE_TYPE_CODE" : "MATERIAL_TYPE_CODE", 90, bound, null);
                AddTreeColumn(treeListPivot,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", lang, culture),
                    isMedicine ? "MEDICINE_TYPE_NAME" : "MATERIAL_TYPE_NAME", 220, bound, null);
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
                AddTreeColumn(treeListPivot,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_AMOUNT", lang, culture),
                    "TotalAmount_Display", 100, unbDec, "#,##0.00");
                AddTreeColumn(treeListPivot,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_AVAILABLE_AMOUNT", lang, culture),
                    "AvailableAmount_Display", 100, unbDec, "#,##0.00");
                AddTreeColumn(treeListPivot, "Tổng nhập", "TOTAL_IMPORT_DISPLAY", 100, unbDec, "#,##0.00");
                AddTreeColumn(treeListPivot, "Tổng xuất", "TOTAL_EXPORT_DISPLAY", 100, unbDec, "#,##0.00");
                AddTreeColumn(treeListPivot, "Tồn cuối kỳ", "ENDING_BALANCE_DISPLAY", 100, unbDec, "#,##0.00");

                // Cơ số / SL cần bù — chỉ có dữ liệu khi chọn đúng 1 kho tủ trực (INCLUDE_BASE_AMOUNT)
                AddTreeColumn(treeListPivot,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_BASE_AMOUNT", lang, culture),
                    "BaseAmount_Display", 80, unbDec, "#,##0.00");
                AddTreeColumn(treeListPivot,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_COMPENSATION_BASE_AMOUNT", lang, culture),
                    "CompensationBaseAmount_Display", 80, unbDec, "#,##0.00");
                AddTreeColumn(treeListPivot,
                    Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_ALERT_MIN_IN_STOCK", lang, culture),
                    "ALERT_MIN_IN_STOCK", 90, bound, "#,##0.##");

                // ----- Cột kho động: mỗi kho đang tích = 1 cột (số lượng tồn của loại tại kho đó) -----
                if (this._MediStocks != null)
                {
                    foreach (var stock in this._MediStocks.Where(p => p.IsCheck == true))
                    {
                        AddTreeColumn(treeListPivot, stock.MEDI_STOCK_NAME,
                            STOCK_AMOUNT_FIELD_PREFIX + stock.ID, 100, unbDec, "#,##0.00");
                    }
                }

                // Ghi chú (DESCRIPTION của loại) — cuối bảng
                string descriptionCaption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_DESCRIPTION", lang, culture);
                if (string.IsNullOrWhiteSpace(descriptionCaption)) descriptionCaption = "Ghi chú";
                AddTreeColumn(treeListPivot, descriptionCaption, "DESCRIPTION_NOTE", 150, unbStr, null);
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
            var lang = Base.ResourceLangManager.LanguageUCMediStockSummaryWithImpExp;
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
                    if (chkAlertMinStock.Checked && chkAlertMinStock.Enabled)
                    {
                        src = FilterAlertMinStock(src.ToList(),
                            o => (o.IS_LEAF ?? 0) == 1 && o.ALERT_MIN_IN_STOCK.HasValue && o.TotalAmount.HasValue
                                 && o.AvailableAmount.HasValue && o.ALERT_MIN_IN_STOCK >= o.AvailableAmount,
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
                    if (chkAlertMinStock.Checked && chkAlertMinStock.Enabled)
                    {
                        src = FilterAlertMinStock(src.ToList(),
                            o => (o.IS_LEAF ?? 0) == 1 && o.ALERT_MIN_IN_STOCK.HasValue && o.TotalAmount.HasValue
                                 && o.AvailableAmount.HasValue && o.ALERT_MIN_IN_STOCK >= o.AvailableAmount,
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
                if (!e.IsGetData) return;

                var medi = e.Row as HisMedicineInStockSDO;
                var mate = medi == null ? e.Row as HisMaterialInStockSDO : null;
                if (medi == null && mate == null) return;

                bool isTypeLeaf = medi != null ? (medi.IS_LEAF ?? 0) == 1 : (mate.IS_LEAF ?? 0) == 1;
                long typeId = medi != null ? medi.MEDICINE_TYPE_ID : mate.MATERIAL_TYPE_ID;
                decimal? totalAmount = medi != null ? medi.TotalAmount : mate.TotalAmount;
                var dicPerStock = medi != null ? dicMediImpExp : dicMateImpExp;
                var dicByType = medi != null ? dicMediImpExpByType : dicMateImpExpByType;

                string fieldName = e.Column.FieldName;
                if (fieldName == "DESCRIPTION_NOTE")
                {
                    string desc = null;
                    var dicDesc = medi != null ? dicMedicineTypeDescription : dicMaterialTypeDescription;
                    if (dicDesc != null) dicDesc.TryGetValue(typeId, out desc);
                    e.Value = desc ?? "";
                    return;
                }

                if (!isTypeLeaf) return;   // dòng nhóm: các cột số để trống

                if (fieldName == "TotalAmount_Display")
                {
                    e.Value = totalAmount;
                }
                else if (fieldName == "AvailableAmount_Display")
                {
                    e.Value = medi != null ? medi.AvailableAmount : mate.AvailableAmount;
                }
                else if (fieldName == "BaseAmount_Display")
                {
                    e.Value = medi != null ? medi.BaseAmount : mate.BaseAmount;
                }
                else if (fieldName == "CompensationBaseAmount_Display")
                {
                    decimal? realBase = medi != null ? medi.RealBaseAmount : mate.RealBaseAmount;
                    e.Value = realBase.HasValue ? (object)(realBase.Value - (totalAmount ?? 0)) : null;
                }
                else if (fieldName == "TOTAL_IMPORT_DISPLAY")
                {
                    MediStockImpExpADO ie;
                    e.Value = (dicByType != null && dicByType.TryGetValue(typeId, out ie) && ie.TOTAL_IMP_QUANTITY.HasValue)
                        ? ie.TOTAL_IMP_QUANTITY.Value : 0m;
                }
                else if (fieldName == "TOTAL_EXPORT_DISPLAY")
                {
                    MediStockImpExpADO ie;
                    e.Value = (dicByType != null && dicByType.TryGetValue(typeId, out ie) && ie.TOTAL_EXP_QUANTITY.HasValue)
                        ? ie.TOTAL_EXP_QUANTITY.Value : 0m;
                }
                else if (fieldName == "ENDING_BALANCE_DISPLAY")
                {
                    MediStockImpExpADO ie;
                    if (dicByType != null && dicByType.TryGetValue(typeId, out ie) && ie.CLOSE_QUANTITY.HasValue)
                        e.Value = ie.CLOSE_QUANTITY.Value;
                    else
                        e.Value = totalAmount ?? 0;   // không có dữ liệu kỳ → tồn hiện tại
                }
                else if (fieldName.StartsWith(STOCK_AMOUNT_FIELD_PREFIX))
                {
                    long stockId;
                    if (long.TryParse(fieldName.Substring(STOCK_AMOUNT_FIELD_PREFIX.Length), out stockId))
                    {
                        MediStockImpExpADO ie;
                        e.Value = (dicPerStock != null && dicPerStock.TryGetValue(ImpExpKey(stockId, typeId), out ie) && ie.AMOUNT.HasValue)
                            ? ie.AMOUNT.Value : 0m;
                    }
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
                bool hasResult = (chkMedicine.Checked && lstMediInStocks != null && lstMediInStocks.Count > 0)
                              || (chkMaterial.Checked && lstMateInStocks != null && lstMateInStocks.Count > 0)
                              || (chkBlood.Checked && lstBloodInStocks != null && lstBloodInStocks.Count > 0);
                if (!hasResult)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa có dữ liệu để xuất", "Thông báo");
                    return;
                }

                var tree = chkBlood.Checked ? treeListBlood : treeListPivot;
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
