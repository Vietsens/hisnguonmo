/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tồn kho nhập xuất tồn - phần bổ sung: bộ lọc Từ ngày/Đến ngày,
 * gọi API GetWithImpExp và dựng dictionary cho 3 cột Tổng nhập/Tổng xuất/Tồn cuối kỳ.
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.MediStockSummaryWithImpExp.ADO;
using HIS.UC.HisBloodTypeInStock;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.SDO;
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

        // Tổng nhập/xuất/tồn cuối kỳ tra theo MEDICINE_TYPE_ID / MATERIAL_TYPE_ID.
        internal Dictionary<long, MediStockImpExpADO> dicMediImpExp = new Dictionary<long, MediStockImpExpADO>();
        internal Dictionary<long, MediStockImpExpADO> dicMateImpExp = new Dictionary<long, MediStockImpExpADO>();

        // True sau khi form Load xong: dùng để chặn tự tìm lúc mới mở, nhưng cho tự nạp khi đổi radio loại.
        internal bool formLoaded = false;

        // 3 radio "Chi tiết / Thu gọn theo thuốc / Thu gọn tất cả" — relocate từ UC con lên plugin level
        // (UC con HIS.UC.HisMedicineInStock/HisMaterialInStock có sẵn 3 radio nhưng ở footer, ẩn chúng đi và tạo lại ở hàng filter)
        internal DevExpress.XtraEditors.CheckEdit pluginChkDetails;
        internal DevExpress.XtraEditors.CheckEdit pluginChkCollapseByMedicine;
        internal DevExpress.XtraEditors.CheckEdit pluginChkCollapseAll;

        // Ô search "txtKeyword" trong UC con (Medicine/Material) đặt phía trên tree — TẠO MỚI ở plugin + ẩn ô cũ
        internal DevExpress.XtraEditors.TextEdit pluginTxtTreeSearch;

        #endregion

        /// <summary>
        /// Khởi tạo 2 ô Từ ngày/Đến ngày và đặt giá trị mặc định.
        /// Gọi sau InitializeComponent (trong Load).
        /// Đặt 2 ô lên hàng đầu (Y=0) của layoutControl3 — chèn lên trên panelControlMediMate (= layoutControlItem5).
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
                    // 0) Tạo ô search MỚI cho tree (KHÔNG đụng txtKeyWork bên trái — đó là search kho)
                    //    Ẩn txtKeyword cũ trong UC con (Medicine + Material) — đó là ô search bên trên tree
                    var lciSearch = CreatePluginTreeSearchAndHideUcSearch();

                    // 1) itemFromDate — đặt CẠNH PHẢI ô search (cùng hàng); nếu search không tạo được, fallback tạo hàng mới
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

                    // 3) Move 3 radio "Chi tiết / Thu gọn theo thuốc / Thu gọn tất cả" cạnh phải itemToDate
                    RelocateCollapseRadios(itemToDate);
                }

                ResetImpExpFilterDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ẩn 3 radio (Chi tiết / Thu gọn theo thuốc / Thu gọn tất cả) trong UC con Medicine + Material,
        /// tạo 3 radio mới ở plugin level và đặt cạnh phải ô "Đến ngày" (cùng hàng filter).
        /// Khi user click radio plugin → set Checked của radio UC con tương ứng → UC con tự xử lý logic gốc.
        /// </summary>
        private void RelocateCollapseRadios(DevExpress.XtraLayout.LayoutControlItem anchorItem)
        {
            try
            {
                // 1) Ẩn 3 radio cũ trong UC con + mở rộng tree
                HideUcCollapseRadios(this.ucMedicineInfo);
                ExpandTreeListInUc(this.ucMedicineInfo);
                HideUcCollapseRadios(this.ucMaterialInfo);
                ExpandTreeListInUc(this.ucMaterialInfo);

                // 2) Tạo 3 radio plugin — RadioGroupIndex=2 để KHÔNG conflict với chkMedicine/chkMaterial/chkBlood (group 1)
                pluginChkDetails = CreateCollapseRadio("Chi tiết", true);
                pluginChkCollapseByMedicine = CreateCollapseRadio("Thu gọn theo thuốc", false);
                pluginChkCollapseAll = CreateCollapseRadio("Thu gọn tất cả", false);

                pluginChkDetails.CheckedChanged += pluginCollapseRadio_CheckedChanged;
                pluginChkCollapseByMedicine.CheckedChanged += pluginCollapseRadio_CheckedChanged;
                pluginChkCollapseAll.CheckedChanged += pluginCollapseRadio_CheckedChanged;

                // 3) Thêm cạnh phải anchor (= itemToDate). Thứ tự: anchorItem | Chi tiết | Thu gọn theo thuốc | Thu gọn tất cả
                var lciDetails = this.layoutControl3.AddItem(
                    "", pluginChkDetails, anchorItem, DevExpress.XtraLayout.Utils.InsertType.Right);
                var lciByMedicine = this.layoutControl3.AddItem(
                    "", pluginChkCollapseByMedicine, lciDetails, DevExpress.XtraLayout.Utils.InsertType.Right);
                var lciAll = this.layoutControl3.AddItem(
                    "", pluginChkCollapseAll, lciByMedicine, DevExpress.XtraLayout.Utils.InsertType.Right);

                // FIX WIDTH 140 — 3 radio sát nhau (không stretch)
                foreach (var lci in new[] { lciDetails, lciByMedicine, lciAll })
                {
                    lci.TextVisible = false;
                    lci.MaxSize = new System.Drawing.Size(140, 26);
                    lci.MinSize = new System.Drawing.Size(140, 26);
                    lci.Size = new System.Drawing.Size(140, 26);
                    lci.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                }

                // txtKeyWork đã được move lên ĐẦU trước đó (qua MoveTxtKeyworkToTopRow trong InitImpExpFilter).
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Tạo 1 CheckEdit kiểu Radio cho group "Collapse" (GroupIndex=2).</summary>
        private DevExpress.XtraEditors.CheckEdit CreateCollapseRadio(string caption, bool initialChecked)
        {
            var chk = new DevExpress.XtraEditors.CheckEdit();
            chk.Properties.Caption = caption;
            chk.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            chk.Properties.RadioGroupIndex = 2;
            chk.Checked = initialChecked;
            return chk;
        }

        /// <summary>
        /// Ẩn 3 field chkDetails / chkCollapseByMedicine / chkCollapseAll trong UC con bằng reflection.
        /// Try multiple strategies: LayoutControlItem.Visibility → Control.Visible → Dispose.
        /// </summary>
        private void HideUcCollapseRadios(Control uc)
        {
            if (uc == null) return;
            try
            {
                Inventec.Common.Logging.LogSystem.Info("[ImpExp][Hide] uc type=" + uc.GetType().FullName);

                var bf = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                       | System.Reflection.BindingFlags.Public;
                foreach (var name in new[] { "chkDetails", "chkCollapseByMedicine", "chkCollapseAll" })
                {
                    System.Reflection.FieldInfo field = null;
                    Type t = uc.GetType();
                    while (t != null && field == null)
                    {
                        field = t.GetField(name, bf);
                        t = t.BaseType;
                    }
                    if (field == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("[ImpExp][Hide] FIELD_NOT_FOUND name=" + name);
                        continue;
                    }
                    var ctrl = field.GetValue(uc) as Control;
                    if (ctrl == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("[ImpExp][Hide] CTRL_NULL name=" + name);
                        continue;
                    }

                    Inventec.Common.Logging.LogSystem.Info("[ImpExp][Hide] name=" + name
                        + " parent=" + (ctrl.Parent != null ? ctrl.Parent.GetType().Name : "null"));

                    bool handled = false;

                    // Strategy 1: ẩn LayoutControlItem chứa control + collapse parent group
                    var lc = FindAncestorLayoutControl(ctrl);
                    if (lc != null)
                    {
                        var lci = lc.GetItemByControl(ctrl) as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null)
                        {
                            lci.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                            // Cũng zero out size để chắc chắn không chiếm chỗ
                            lci.MaxSize = new System.Drawing.Size(0, 0);
                            lci.MinSize = new System.Drawing.Size(0, 0);
                            lci.Size = new System.Drawing.Size(0, 0);
                            lci.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;

                            // Collapse parent group nếu group chỉ chứa các LCI bị ẩn
                            CollapseEmptyParentGroup(lci);

                            handled = true;
                            Inventec.Common.Logging.LogSystem.Info("[ImpExp][Hide] STRATEGY1 LCI_Never+ZeroSize name=" + name);
                        }
                    }

                    // Strategy 2: remove khỏi parent
                    if (!handled)
                    {
                        ctrl.Visible = false;
                        if (ctrl.Parent != null) ctrl.Parent.Controls.Remove(ctrl);
                        Inventec.Common.Logging.LogSystem.Info("[ImpExp][Hide] STRATEGY2 RemoveFromParent name=" + name);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        /// <summary>Tìm LayoutControlItem nào trong layoutControl chính chứa target control.</summary>
        private DevExpress.XtraLayout.LayoutControlItem FindLayoutItemForControl(
            DevExpress.XtraLayout.LayoutControl mainLc, Control target)
        {
            if (mainLc == null || target == null) return null;
            try
            {
                return mainLc.GetItemByControl(target) as DevExpress.XtraLayout.LayoutControlItem;
            }
            catch { return null; }
        }

        /// <summary>
        /// Tìm TreeList trong UC con, mở rộng MaxSize của LayoutControlItem chứa nó
        /// để tree stretch xuống lấp chỗ trống do 3 radio bị ẩn.
        /// </summary>
        private void ExpandTreeListInUc(Control uc)
        {
            if (uc == null) return;
            try
            {
                var tl = FindTreeList(uc);
                if (tl == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("[ImpExp][Expand] tree NOT_FOUND in " + uc.GetType().Name);
                    return;
                }
                var lc = FindAncestorLayoutControl(tl);
                if (lc == null) return;
                var lci = lc.GetItemByControl(tl) as DevExpress.XtraLayout.LayoutControlItem;
                if (lci == null) return;

                // Unlock MaxSize Height (0 = unlimited) để tree stretch xuống
                lci.MaxSize = new System.Drawing.Size(0, 0);
                lci.MinSize = new System.Drawing.Size(100, 100);
                lci.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;

                // Force layout refresh
                lc.PerformLayout();
                uc.PerformLayout();

                Inventec.Common.Logging.LogSystem.Info("[ImpExp][Expand] tree expanded in " + uc.GetType().Name
                    + " tree=" + tl.Name);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tạo ô tìm kiếm thuốc/VT MỚI ở plugin (chèn TOP của panel — tạo hàng mới Y=50).
        /// Ẩn ô txtKeyword CŨ trong UC con Medicine + Material để tránh trùng.
        /// </summary>
        private DevExpress.XtraLayout.LayoutControlItem CreatePluginTreeSearchAndHideUcSearch()
        {
            try
            {
                if (this.layoutControl3 == null || this.layoutControlItem5 == null) return null;

                // 1) Ẩn txtKeyword cũ trong UC con
                HideUcTxtKeyword(this.ucMedicineInfo);
                HideUcTxtKeyword(this.ucMaterialInfo);

                // 2) Tạo TextEdit mới ở plugin
                pluginTxtTreeSearch = new DevExpress.XtraEditors.TextEdit();
                pluginTxtTreeSearch.Properties.NullValuePrompt = "Từ khóa tìm kiếm thuốc/vật tư...";
                pluginTxtTreeSearch.EditValueChanged += pluginTxtTreeSearch_EditValueChanged;

                // 3) Thêm vào layoutControl3, chèn TOP của panelControlMediMate
                var newLci = this.layoutControl3.AddItem(
                    "", pluginTxtTreeSearch, this.layoutControlItem5, DevExpress.XtraLayout.Utils.InsertType.Top);
                newLci.TextVisible = false;
                newLci.MaxSize = new System.Drawing.Size(0, 26);
                newLci.MinSize = new System.Drawing.Size(250, 26);
                newLci.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;

                Inventec.Common.Logging.LogSystem.Info("[ImpExp][TreeSearch] created at top, UC txtKeyword hidden");
                return newLci;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Ẩn field "txtKeyword" trong UC con (search bên trên tree) bằng reflection.</summary>
        private void HideUcTxtKeyword(Control uc)
        {
            if (uc == null) return;
            try
            {
                var bf = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                       | System.Reflection.BindingFlags.Public;
                var field = uc.GetType().GetField("txtKeyword", bf);
                if (field == null) return;
                var ctrl = field.GetValue(uc) as Control;
                if (ctrl == null) return;

                var lc = FindAncestorLayoutControl(ctrl);
                if (lc != null)
                {
                    var lci = lc.GetItemByControl(ctrl) as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null)
                    {
                        lci.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lci.MaxSize = new System.Drawing.Size(0, 0);
                        lci.MinSize = new System.Drawing.Size(0, 0);
                        return;
                    }
                }
                ctrl.Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Khi user gõ ô search plugin → apply FindFilter cho cả Medicine + Material tree.</summary>
        private void pluginTxtTreeSearch_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string text = pluginTxtTreeSearch.EditValue as string ?? "";
                SetTreeFindText(this.ucMedicineInfo, text);
                SetTreeFindText(this.ucMaterialInfo, text);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Trèo Parent chain để tìm LayoutControl chứa control.</summary>
        private DevExpress.XtraLayout.LayoutControl FindAncestorLayoutControl(Control ctrl)
        {
            var p = ctrl != null ? ctrl.Parent : null;
            while (p != null)
            {
                var lc = p as DevExpress.XtraLayout.LayoutControl;
                if (lc != null) return lc;
                p = p.Parent;
            }
            return null;
        }

        /// <summary>
        /// Đi lên parent group của LCI. Nếu tất cả con của group đều đã bị Visibility=Never → set group cũng Never.
        /// Đệ quy lên các group cao hơn để collapse triệt để.
        /// </summary>
        private void CollapseEmptyParentGroup(DevExpress.XtraLayout.BaseLayoutItem item)
        {
            try
            {
                if (item == null) return;
                var parent = item.Parent as DevExpress.XtraLayout.LayoutControlGroup;
                if (parent == null) return;
                if (parent.Items == null || parent.Items.Count == 0) return;

                // Đếm visible items trong group
                bool allHidden = true;
                foreach (DevExpress.XtraLayout.BaseLayoutItem child in parent.Items)
                {
                    if (child.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    {
                        // Còn 1 child visible → không collapse group
                        // Trừ EmptySpaceItem (chỉ là khoảng trống, không có control)
                        if (!(child is DevExpress.XtraLayout.EmptySpaceItem))
                        {
                            allHidden = false;
                            break;
                        }
                    }
                }

                if (allHidden)
                {
                    parent.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    Inventec.Common.Logging.LogSystem.Info("[ImpExp][Hide] CollapseGroup name=" + parent.Name);
                    // Đệ quy lên cao hơn
                    CollapseEmptyParentGroup(parent);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khi user click radio plugin → set Checked của radio tương ứng trong UC con (cả Medicine + Material)
        /// để UC con tự kích hoạt logic collapse/expand gốc.
        /// </summary>
        private void pluginCollapseRadio_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var senderChk = sender as DevExpress.XtraEditors.CheckEdit;
                if (senderChk == null || !senderChk.Checked) return;

                string targetFieldName = null;
                if (senderChk == pluginChkDetails) targetFieldName = "chkDetails";
                else if (senderChk == pluginChkCollapseByMedicine) targetFieldName = "chkCollapseByMedicine";
                else if (senderChk == pluginChkCollapseAll) targetFieldName = "chkCollapseAll";
                if (targetFieldName == null) return;

                SyncRadioToUc(this.ucMedicineInfo, targetFieldName);
                SyncRadioToUc(this.ucMaterialInfo, targetFieldName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Set Checked=true cho radio có tên ucFieldName trong UC con để trigger CheckedChanged gốc.</summary>
        private void SyncRadioToUc(Control uc, string ucFieldName)
        {
            if (uc == null) return;
            try
            {
                var bf = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                       | System.Reflection.BindingFlags.Public;
                var field = uc.GetType().GetField(ucFieldName, bf);
                if (field == null) return;
                var ucRadio = field.GetValue(uc) as DevExpress.XtraEditors.CheckEdit;
                if (ucRadio != null && !ucRadio.Checked) ucRadio.Checked = true;
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
        /// Gọi API GetWithImpExp và dựng dictionary tổng nhập/xuất/tồn cuối kỳ.
        /// Không nhập khoảng thời gian =&gt; bỏ qua (Tổng nhập/xuất = 0, Tồn cuối kỳ = tồn hiện tại, xử lý ở callback).
        /// </summary>
        private void BuildImpExpDictionary(bool isMedicine, long? fromTime, long? toTime)
        {
            try
            {
                if (isMedicine) dicMediImpExp = new Dictionary<long, MediStockImpExpADO>();
                else dicMateImpExp = new Dictionary<long, MediStockImpExpADO>();

                if (!fromTime.HasValue && !toTime.HasValue)
                    return;

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

                Inventec.Common.Logging.LogSystem.Info("[ImpExp] CALL uri=" + uri
                    + Inventec.Common.Logging.LogUtil.TraceData("filter", filter));

                var result = new BackendAdapter(param).Post<List<MediStockImpExpADO>>(uri, ApiConsumers.MosConsumer, filter, param);

                Inventec.Common.Logging.LogSystem.Info("[ImpExp] RESULT uri=" + uri
                    + " count=" + (result == null ? "null" : result.Count.ToString())
                    + Inventec.Common.Logging.LogUtil.TraceData("param", param));

                if (result == null) return;

                var dic = isMedicine ? dicMediImpExp : dicMateImpExp;
                foreach (var item in result)
                {
                    if (item == null) continue;
                    long key = isMedicine ? (item.MEDICINE_TYPE_ID ?? 0) : (item.MATERIAL_TYPE_ID ?? 0);
                    if (key == 0) continue;
                    if (dic.ContainsKey(key))
                    {
                        dic[key].TOTAL_IMP_QUANTITY = (dic[key].TOTAL_IMP_QUANTITY ?? 0) + (item.TOTAL_IMP_QUANTITY ?? 0);
                        dic[key].TOTAL_EXP_QUANTITY = (dic[key].TOTAL_EXP_QUANTITY ?? 0) + (item.TOTAL_EXP_QUANTITY ?? 0);
                        dic[key].CLOSE_QUANTITY = (dic[key].CLOSE_QUANTITY ?? 0) + (item.CLOSE_QUANTITY ?? 0);
                    }
                    else
                    {
                        dic[key] = item;
                    }
                }

                // DEBUG: log keys in dictionary để verify lookup
                Inventec.Common.Logging.LogSystem.Info("[ImpExp] DICT keys count=" + dic.Count
                    + " keys=[" + string.Join(",", dic.Keys) + "]");

                // DEBUG: log mọi item có IMP > 0 hoặc EXP > 0 từ API trả về
                foreach (var item in result.Where(o => o != null && ((o.TOTAL_IMP_QUANTITY ?? 0) > 0 || (o.TOTAL_EXP_QUANTITY ?? 0) > 0)))
                {
                    Inventec.Common.Logging.LogSystem.Info("[ImpExp] NONZERO mety=" + item.MEDICINE_TYPE_ID
                        + " maty=" + item.MATERIAL_TYPE_ID
                        + " imp=" + item.TOTAL_IMP_QUANTITY + " exp=" + item.TOTAL_EXP_QUANTITY
                        + " close=" + item.CLOSE_QUANTITY);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region ---Giữ text tìm trên cây kết quả khi bấm Tìm (Reload làm mất FindFilterText)
        /// <summary>Tìm TreeList lồng bên trong UserControl tồn kho (đệ quy).</summary>
        private DevExpress.XtraTreeList.TreeList FindTreeList(Control c)
        {
            if (c == null) return null;
            DevExpress.XtraTreeList.TreeList tl = c as DevExpress.XtraTreeList.TreeList;
            if (tl != null) return tl;
            foreach (Control child in c.Controls)
            {
                var r = FindTreeList(child);
                if (r != null) return r;
            }
            return null;
        }

        private string GetTreeFindText(Control uc)
        {
            try
            {
                var tl = FindTreeList(uc);
                return tl != null ? tl.FindFilterText : null;
            }
            catch { return null; }
        }

        private void SetTreeFindText(Control uc, string text)
        {
            try
            {
                var tl = FindTreeList(uc);
                if (tl != null) tl.ApplyFindFilter(text);
            }
            catch { }
        }

        /// <summary>Lấy text tìm của cây đang hiển thị (theo loại đang chọn).</summary>
        private string GetActiveTreeFindText()
        {
            try
            {
                if (chkMedicine.Checked) return GetTreeFindText(ucMedicineInfo);
                if (chkMaterial.Checked) return GetTreeFindText(ucMaterialInfo);
            }
            catch { }
            return null;
        }

        /// <summary>Gán lại text tìm cho cây đang hiển thị sau khi Reload.</summary>
        private void RestoreActiveTreeFindText(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) return;
                if (chkMedicine.Checked) SetTreeFindText(ucMedicineInfo, text);
                else if (chkMaterial.Checked) SetTreeFindText(ucMaterialInfo, text);
            }
            catch { }
        }
        #endregion

        /// <summary>
        /// Xóa kết quả trên panel kết quả, chuyển panel theo loại đang chọn và để trống (chờ nhấn Tìm).
        /// </summary>
        private void ClearResultPanel()
        {
            try
            {
                if (dicMediImpExp != null) dicMediImpExp.Clear();
                if (dicMateImpExp != null) dicMateImpExp.Clear();

                this.panelControlMediMate.Controls.Clear();
                if (chkMedicine.Checked && this.ucMedicineInfo != null)
                {
                    this.panelControlMediMate.Controls.Add(this.ucMedicineInfo);
                    this.ucMedicineInfo.Dock = DockStyle.Fill;
                    hisMediInStockProcessor.Reload(ucMedicineInfo, null, null);
                }
                else if (chkMaterial.Checked && this.ucMaterialInfo != null)
                {
                    this.panelControlMediMate.Controls.Add(this.ucMaterialInfo);
                    this.ucMaterialInfo.Dock = DockStyle.Fill;
                    hisMateInStockProcessor.Reload(ucMaterialInfo, null, null);
                }
                else if (chkBlood.Checked && this.ucBloodInfo != null)
                {
                    this.panelControlMediMate.Controls.Add(this.ucBloodInfo);
                    this.ucBloodInfo.Dock = DockStyle.Fill;
                    hisBloodProcessor.Reload(ucBloodInfo, new List<HisBloodTypeInStockSDO>());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
