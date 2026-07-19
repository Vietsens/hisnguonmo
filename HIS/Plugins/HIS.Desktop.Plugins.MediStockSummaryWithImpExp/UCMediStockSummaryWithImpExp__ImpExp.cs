/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tồn kho nhập xuất tồn - phần bổ sung: bộ lọc Từ ngày/Đến ngày,
 * gọi API GetWithImpExp và dựng dictionary số liệu Nhập/Xuất/Tồn:
 * - dicMediImpExp/dicMateImpExp: theo CẶP (kho, loại) — nguồn cột kho động (AMOUNT theo kho).
 * - dicMediImpExpByType/dicMateImpExpByType: tổng hợp theo LOẠI (cộng các kho đã chọn)
 *   — nguồn 3 cột chung Tổng nhập / Tổng xuất / Tồn cuối kỳ.
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.MediStockSummaryWithImpExp.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockSummaryWithImpExp
{
    public partial class UCMediStockSummaryWithImpExp
    {
        #region ---ImpExp fields
        // Ô lọc Từ ngày / Đến ngày được tạo bằng code (thêm vào layoutControl3).
        internal DevExpress.XtraEditors.DateEdit dtFromDate;
        internal DevExpress.XtraEditors.DateEdit dtToDate;

        // Số liệu Nhập/Xuất/Tồn theo CẶP (MEDI_STOCK_ID, MEDICINE_TYPE_ID/MATERIAL_TYPE_ID). Key = "{mediStockId}_{typeId}".
        internal Dictionary<string, MediStockImpExpADO> dicMediImpExp = new Dictionary<string, MediStockImpExpADO>();
        internal Dictionary<string, MediStockImpExpADO> dicMateImpExp = new Dictionary<string, MediStockImpExpADO>();

        // Số liệu tổng hợp theo LOẠI (SUM trên các kho đã chọn) — dùng cho 3 cột chung khi pivot 1 dòng = 1 loại.
        internal Dictionary<long, MediStockImpExpADO> dicMediImpExpByType = new Dictionary<long, MediStockImpExpADO>();
        internal Dictionary<long, MediStockImpExpADO> dicMateImpExpByType = new Dictionary<long, MediStockImpExpADO>();

        /// <summary>Khóa tra cứu nhập/xuất/tồn theo cặp (kho, loại).</summary>
        internal static string ImpExpKey(long? mediStockId, long? typeId)
        {
            return (mediStockId ?? 0) + "_" + (typeId ?? 0);
        }

        // True sau khi form Load xong: dùng để chặn tự tìm lúc mới mở, nhưng cho tự nạp khi đổi radio loại.
        internal bool formLoaded = false;

        // 2 radio "Chi tiết / Thu gọn tất cả" — bung/đóng node nhóm trên cây kết quả tự dựng.
        // (Bỏ "Thu gọn theo thuốc": không còn dòng lô nên trùng nghĩa với "Chi tiết".)
        internal DevExpress.XtraEditors.CheckEdit pluginChkDetails;
        internal DevExpress.XtraEditors.CheckEdit pluginChkCollapseAll;

        // "Ẩn dòng nhóm": bật → danh sách phẳng, chỉ hiện dòng loại (có số liệu Nhập/Xuất/Tồn), không hiện node nhóm cha.
        internal DevExpress.XtraEditors.CheckEdit pluginChkHideGroup;

        // Ô tìm kiếm thuốc/vật tư trên cây kết quả (ApplyFindFilter).
        internal DevExpress.XtraEditors.TextEdit pluginTxtTreeSearch;
        #endregion

        /// <summary>
        /// Khởi tạo 2 ô Từ ngày/Đến ngày, ô tìm kiếm cây và 2 radio thu gọn; đặt giá trị mặc định.
        /// Gọi sau InitializeComponent (trong Load).
        /// </summary>
        private void InitImpExpFilter()
        {
            try
            {
                dtFromDate = new DevExpress.XtraEditors.DateEdit();
                dtFromDate.Name = "dtFromDate";
                dtFromDate.Properties.Mask.EditMask = "dd/MM/yyyy";
                dtFromDate.Properties.Mask.UseMaskAsDisplayFormat = true;
                dtFromDate.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Vista;
                dtFromDate.Width = 92;

                dtToDate = new DevExpress.XtraEditors.DateEdit();
                dtToDate.Name = "dtToDate";
                dtToDate.Properties.Mask.EditMask = "dd/MM/yyyy";
                dtToDate.Properties.Mask.UseMaskAsDisplayFormat = true;
                dtToDate.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Vista;
                dtToDate.Width = 92;

                if (this.layoutControl3 != null && this.layoutControlItem5 != null)
                {
                    // 0) Ô tìm kiếm thuốc/vật tư — hàng mới phía trên panel kết quả
                    var lciSearch = CreatePluginTreeSearch();

                    // 1) itemFromDate — cạnh phải ô tìm kiếm (cùng hàng); fallback hàng mới nếu search không tạo được
                    DevExpress.XtraLayout.LayoutControlItem itemFromDate;
                    if (lciSearch != null)
                    {
                        itemFromDate = this.layoutControl3.AddItem(
                            "Từ ngày", dtFromDate, lciSearch, DevExpress.XtraLayout.Utils.InsertType.Right);
                    }
                    else
                    {
                        itemFromDate = this.layoutControl3.AddItem(
                            "Từ ngày", dtFromDate, this.layoutControlItem5, DevExpress.XtraLayout.Utils.InsertType.Top);
                    }
                    // itemToDate cạnh PHẢI itemFromDate
                    var itemToDate = this.layoutControl3.AddItem(
                        "Đến ngày", dtToDate, itemFromDate, DevExpress.XtraLayout.Utils.InsertType.Right);

                    // Cấu hình caption + size — fix width=220 để 2 ô Từ ngày/Đến ngày BẰNG NHAU
                    itemFromDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
                    itemFromDate.TextSize = new System.Drawing.Size(60, 20);
                    itemFromDate.MaxSize = new System.Drawing.Size(220, 26);
                    itemFromDate.MinSize = new System.Drawing.Size(220, 26);
                    itemFromDate.Size = new System.Drawing.Size(220, 26);
                    itemFromDate.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;

                    itemToDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
                    itemToDate.TextSize = new System.Drawing.Size(60, 20);
                    itemToDate.MaxSize = new System.Drawing.Size(220, 26);
                    itemToDate.MinSize = new System.Drawing.Size(220, 26);
                    itemToDate.Size = new System.Drawing.Size(220, 26);
                    itemToDate.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;

                    // 2) 2 radio "Chi tiết / Thu gọn tất cả" cạnh phải itemToDate
                    CreateCollapseRadios(itemToDate);
                }

                ResetImpExpFilterDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Tạo 2 radio "Chi tiết / Thu gọn tất cả" ở hàng filter, cạnh phải anchorItem.</summary>
        private void CreateCollapseRadios(DevExpress.XtraLayout.LayoutControlItem anchorItem)
        {
            try
            {
                pluginChkDetails = CreateCollapseRadio("Chi tiết", true);
                pluginChkCollapseAll = CreateCollapseRadio("Thu gọn tất cả", false);

                pluginChkDetails.CheckedChanged += pluginCollapseRadio_CheckedChanged;
                pluginChkCollapseAll.CheckedChanged += pluginCollapseRadio_CheckedChanged;

                // Checkbox "Ẩn dòng nhóm" — bật/tắt hiển thị node nhóm cha trên cây kết quả
                pluginChkHideGroup = new DevExpress.XtraEditors.CheckEdit();
                pluginChkHideGroup.Name = "pluginChkHideGroup";
                pluginChkHideGroup.Properties.Caption = "Ẩn dòng nhóm";
                pluginChkHideGroup.Checked = false;
                pluginChkHideGroup.CheckedChanged += pluginChkHideGroup_CheckedChanged;

                var lciDetails = this.layoutControl3.AddItem(
                    "", pluginChkDetails, anchorItem, DevExpress.XtraLayout.Utils.InsertType.Right);
                var lciAll = this.layoutControl3.AddItem(
                    "", pluginChkCollapseAll, lciDetails, DevExpress.XtraLayout.Utils.InsertType.Right);
                var lciHideGroup = this.layoutControl3.AddItem(
                    "", pluginChkHideGroup, lciAll, DevExpress.XtraLayout.Utils.InsertType.Right);

                // FIX WIDTH 140 — radio/checkbox sát nhau (không stretch)
                foreach (var lci in new[] { lciDetails, lciAll, lciHideGroup })
                {
                    lci.TextVisible = false;
                    lci.MaxSize = new System.Drawing.Size(140, 26);
                    lci.MinSize = new System.Drawing.Size(140, 26);
                    lci.Size = new System.Drawing.Size(140, 26);
                    lci.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Tạo 1 CheckEdit kiểu Radio cho group "Collapse" (GroupIndex=2 — tránh conflict group radio loại).</summary>
        private DevExpress.XtraEditors.CheckEdit CreateCollapseRadio(string caption, bool initialChecked)
        {
            var chk = new DevExpress.XtraEditors.CheckEdit();
            chk.Properties.Caption = caption;
            chk.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            chk.Properties.RadioGroupIndex = 2;
            chk.Checked = initialChecked;
            return chk;
        }

        /// <summary>Tạo ô tìm kiếm thuốc/VT ở plugin (hàng mới phía trên panel kết quả).</summary>
        private DevExpress.XtraLayout.LayoutControlItem CreatePluginTreeSearch()
        {
            try
            {
                if (this.layoutControl3 == null || this.layoutControlItem5 == null) return null;

                pluginTxtTreeSearch = new DevExpress.XtraEditors.TextEdit();
                pluginTxtTreeSearch.Properties.NullValuePrompt = "Từ khóa tìm kiếm thuốc/vật tư...";
                pluginTxtTreeSearch.EditValueChanged += pluginTxtTreeSearch_EditValueChanged;

                var newLci = this.layoutControl3.AddItem(
                    "", pluginTxtTreeSearch, this.layoutControlItem5, DevExpress.XtraLayout.Utils.InsertType.Top);
                newLci.TextVisible = false;
                newLci.MaxSize = new System.Drawing.Size(0, 26);
                newLci.MinSize = new System.Drawing.Size(250, 26);
                newLci.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                return newLci;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Khi user gõ ô tìm kiếm → apply FindFilter cho cây kết quả đang hiển thị.</summary>
        private void pluginTxtTreeSearch_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string text = pluginTxtTreeSearch.EditValue as string ?? "";
                if (treeListPivot != null) treeListPivot.ApplyFindFilter(text);
                if (treeListBlood != null) treeListBlood.ApplyFindFilter(text);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// "Ẩn dòng nhóm": bật → bảng phẳng chỉ còn dòng loại (số liệu Nhập/Xuất/Tồn), không hiện node nhóm cha;
        /// tắt → hiển thị lại cây nhóm → loại như bình thường. Lưu trạng thái vào ControlState.
        /// </summary>
        private void pluginChkHideGroup_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!IsInitForm)
                {
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                        ? this.currentControlStateRDO.Where(o => o.KEY == pluginChkHideGroup.Name && o.MODULE_LINK == this.ModuleLink).FirstOrDefault()
                        : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (pluginChkHideGroup.Checked ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = pluginChkHideGroup.Name;
                        csAddOrUpdate.VALUE = (pluginChkHideGroup.Checked ? "1" : "");
                        csAddOrUpdate.MODULE_LINK = this.ModuleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    if (this.controlStateWorker != null)
                        this.controlStateWorker.SetData(this.currentControlStateRDO);
                }

                // Không còn node nhóm để bung/đóng → 2 radio Chi tiết / Thu gọn tất cả mất tác dụng khi đang ẩn nhóm
                if (pluginChkDetails != null) pluginChkDetails.Enabled = !pluginChkHideGroup.Checked;
                if (pluginChkCollapseAll != null) pluginChkCollapseAll.Enabled = !pluginChkHideGroup.Checked;

                if (!formLoaded) return;
                // Bind lại ngay trên dữ liệu đã tải (không gọi lại API)
                if (chkMedicine.Checked && lstMediInStocks != null)
                {
                    BindPivotTree(true);
                }
                else if (chkMaterial.Checked && lstMateInStocks != null)
                {
                    BindPivotTree(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Radio "Chi tiết" → bung node nhóm; "Thu gọn tất cả" → đóng node nhóm.</summary>
        private void pluginCollapseRadio_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var senderChk = sender as DevExpress.XtraEditors.CheckEdit;
                if (senderChk == null || !senderChk.Checked) return;
                ApplyCollapseMode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Từ ngày = ngày 01 tháng hiện tại; Đến ngày = ngày hiện tại.
        /// </summary>
        private void ResetImpExpFilterDefault()
        {
            try
            {
                var now = DateTime.Now;
                var firstDay = new DateTime(now.Year, now.Month, 1);
                if (dtFromDate != null) dtFromDate.EditValue = firstDay;
                if (dtToDate != null) dtToDate.EditValue = now.Date;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lấy khoảng thời gian (yyyyMMddHHmmss). Trả về false nếu Từ ngày &gt; Đến ngày.
        /// fromTime/toTime = null nếu ô tương ứng để trống.
        /// </summary>
        private bool TryGetImpExpRange(out long? fromTime, out long? toTime)
        {
            fromTime = null;
            toTime = null;
            DateTime? from = null;
            DateTime? to = null;
            if (dtFromDate != null && dtFromDate.EditValue != null) from = dtFromDate.DateTime.Date;
            if (dtToDate != null && dtToDate.EditValue != null) to = dtToDate.DateTime.Date;

            if (from.HasValue && to.HasValue && from.Value > to.Value)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Từ ngày phải nhỏ hơn hoặc bằng đến ngày", "Thông báo");
                return false;
            }

            if (from.HasValue)
                fromTime = Inventec.Common.TypeConvert.Parse.ToInt64(from.Value.ToString("yyyyMMdd") + "000000");
            if (to.HasValue)
                toTime = Inventec.Common.TypeConvert.Parse.ToInt64(to.Value.ToString("yyyyMMdd") + "235959");
            return true;
        }

        /// <summary>
        /// Gọi API GetWithImpExp và dựng 2 dictionary:
        /// - Theo cặp (kho, loại): AMOUNT (tồn hiện tại theo kho — cột kho động) + Nhập/Xuất/Tồn cuối kỳ.
        /// - Tổng hợp theo loại: SUM Nhập/Xuất/Tồn cuối kỳ trên các kho đã chọn (3 cột chung).
        /// LUÔN gọi khi đã chọn kho — kể cả không nhập khoảng thời gian (backend trả Nhập/Xuất = 0,
        /// Tồn cuối kỳ = tồn hiện tại; AMOUNT theo kho vẫn cần cho cột kho động).
        /// </summary>
        private void BuildImpExpDictionary(bool isMedicine, long? fromTime, long? toTime)
        {
            try
            {
                if (isMedicine)
                {
                    dicMediImpExp = new Dictionary<string, MediStockImpExpADO>();
                    dicMediImpExpByType = new Dictionary<long, MediStockImpExpADO>();
                }
                else
                {
                    dicMateImpExp = new Dictionary<string, MediStockImpExpADO>();
                    dicMateImpExpByType = new Dictionary<long, MediStockImpExpADO>();
                }

                if (this.mediStockIds == null || this.mediStockIds.Count == 0)
                    return;

                CommonParam param = new CommonParam();
                MediStockImpExpFilter filter = new MediStockImpExpFilter();
                filter.MEDI_STOCK_IDs = this.mediStockIds;
                filter.FROM_TIME = fromTime;
                filter.TO_TIME = toTime;

                string uri = isMedicine
                    ? "api/HisMediStockMety/GetWithImpExp"
                    : "api/HisMediStockMaty/GetWithImpExp";

                var result = new BackendAdapter(param).Post<List<MediStockImpExpADO>>(uri, ApiConsumers.MosConsumer, filter, param);
                if (result == null) return;

                var dic = isMedicine ? dicMediImpExp : dicMateImpExp;
                var dicByType = isMedicine ? dicMediImpExpByType : dicMateImpExpByType;
                foreach (var item in result)
                {
                    if (item == null) continue;
                    long typeId = isMedicine ? (item.MEDICINE_TYPE_ID ?? 0) : (item.MATERIAL_TYPE_ID ?? 0);
                    if (typeId == 0) continue;

                    // Khóa theo cặp (kho, loại) — số liệu riêng từng kho cho cột kho động
                    string key = ImpExpKey(item.MEDI_STOCK_ID, typeId);
                    if (dic.ContainsKey(key))
                    {
                        dic[key].AMOUNT = (dic[key].AMOUNT ?? 0) + (item.AMOUNT ?? 0);
                        dic[key].TOTAL_IMP_QUANTITY = (dic[key].TOTAL_IMP_QUANTITY ?? 0) + (item.TOTAL_IMP_QUANTITY ?? 0);
                        dic[key].TOTAL_EXP_QUANTITY = (dic[key].TOTAL_EXP_QUANTITY ?? 0) + (item.TOTAL_EXP_QUANTITY ?? 0);
                        dic[key].CLOSE_QUANTITY = (dic[key].CLOSE_QUANTITY ?? 0) + (item.CLOSE_QUANTITY ?? 0);
                    }
                    else
                    {
                        dic[key] = item;
                    }

                    // Tổng hợp theo loại (cộng dồn các kho) — 3 cột chung Tổng nhập/Tổng xuất/Tồn cuối kỳ
                    MediStockImpExpADO aggr;
                    if (!dicByType.TryGetValue(typeId, out aggr))
                    {
                        aggr = new MediStockImpExpADO();
                        if (isMedicine) aggr.MEDICINE_TYPE_ID = typeId; else aggr.MATERIAL_TYPE_ID = typeId;
                        dicByType[typeId] = aggr;
                    }
                    aggr.AMOUNT = (aggr.AMOUNT ?? 0) + (item.AMOUNT ?? 0);
                    aggr.TOTAL_IMP_QUANTITY = (aggr.TOTAL_IMP_QUANTITY ?? 0) + (item.TOTAL_IMP_QUANTITY ?? 0);
                    aggr.TOTAL_EXP_QUANTITY = (aggr.TOTAL_EXP_QUANTITY ?? 0) + (item.TOTAL_EXP_QUANTITY ?? 0);
                    aggr.CLOSE_QUANTITY = (aggr.CLOSE_QUANTITY ?? 0) + (item.CLOSE_QUANTITY ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xóa kết quả trên panel kết quả, chuyển panel theo loại đang chọn và để trống (chờ nhấn Tìm).
        /// </summary>
        private void ClearResultPanel()
        {
            try
            {
                if (dicMediImpExp != null) dicMediImpExp.Clear();
                if (dicMateImpExp != null) dicMateImpExp.Clear();
                if (dicMediImpExpByType != null) dicMediImpExpByType.Clear();
                if (dicMateImpExpByType != null) dicMateImpExpByType.Clear();

                if (chkBlood.Checked)
                {
                    if (treeListBlood != null) treeListBlood.DataSource = null;
                    ShowResultControl(treeListBlood);
                }
                else
                {
                    if (treeListPivot != null) treeListPivot.DataSource = null;
                    ShowResultControl(treeListPivot);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
