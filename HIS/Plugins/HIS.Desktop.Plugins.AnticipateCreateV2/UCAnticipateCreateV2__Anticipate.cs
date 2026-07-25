/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong 52461 - Tạo dự trù v2: chế độ "Dự trù theo" (Loại mặt hàng / Thầu).
 * - Loại mặt hàng: TreeList hiển thị mọi loại (gộp toàn viện, số liệu thầu gộp mọi thầu — GetForAnticipate).
 * - Thầu: hiện 2 combobox Nhà thầu + Gói thầu; chọn thầu → lọc TreeList chỉ còn loại thuộc gói thầu,
 *   và các cột SL thầu / Thầu đã nhập / Thầu còn lại / Giá sau VAT / Nhà cung cấp lấy theo gói thầu đó
 *   (từ V_HIS_BID_MEDICINE_TYPE / V_HIS_BID_MATERIAL_TYPE).
 */
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AnticipateCreateV2
{
    public partial class UCAnticipateCreateV2
    {
        #region ---Anticipate mode fields
        // Radio "Dự trù theo": Loại mặt hàng (mặc định) / Thầu.
        internal DevExpress.XtraEditors.CheckEdit pluginRdoByItemType;
        internal DevExpress.XtraEditors.CheckEdit pluginRdoBySupply;
        // Combobox Nhà thầu + Gói thầu (chỉ hiện ở chế độ Thầu).
        internal DevExpress.XtraEditors.GridLookUpEdit pluginCboSupplier;
        internal DevExpress.XtraEditors.GridLookUpEdit pluginCboBid;

        // Dữ liệu thầu (nạp 1 lần khi lần đầu vào chế độ Thầu).
        internal List<HIS_BID> ListBid;
        internal List<HIS_SUPPLIER> ListSupplier;
        internal List<V_HIS_BID_MEDICINE_TYPE> ListBidMedicineType;
        internal List<V_HIS_BID_MATERIAL_TYPE> ListBidMaterialType;
        private bool bidDataLoaded = false;

        internal long? currentSupplierId = null;
        internal long? currentBidId = null;

        // Loại thuộc gói thầu đang chọn (lọc TreeList). null = không lọc theo thầu.
        internal HashSet<long> allowedTypeIdsByBid = null;
        // Số liệu thầu theo loại (override cột SL thầu/giá/NCC ở chế độ Thầu). Key = typeId.
        internal Dictionary<long, V_HIS_BID_MEDICINE_TYPE> dicBidMediByType = new Dictionary<long, V_HIS_BID_MEDICINE_TYPE>();
        internal Dictionary<long, V_HIS_BID_MATERIAL_TYPE> dicBidMateByType = new Dictionary<long, V_HIS_BID_MATERIAL_TYPE>();

        /// <summary>True khi đang ở chế độ "Dự trù theo Thầu".</summary>
        internal bool IsSupplyMode
        {
            get { return pluginRdoBySupply != null && pluginRdoBySupply.Checked; }
        }

        // Nút thao tác dự trù (thêm runtime cạnh Xuất Excel/In).
        internal DevExpress.XtraEditors.SimpleButton btnBoSungAntc;
        internal DevExpress.XtraEditors.SimpleButton btnSaveAntc;
        internal DevExpress.XtraEditors.SimpleButton btnImportAntc;
        internal DevExpress.XtraEditors.SimpleButton btnNewAntc;
        // Phiếu dự trù vừa lưu (dùng cho In).
        internal HIS_ANTICIPATE anticipatePrint = null;

        // vCong 52461 — bố trí lại thanh lọc: neo hàng "Dự trù theo" + control runtime thay control Designer.
        internal DevExpress.XtraLayout.LayoutControlItem lciModeLabel;
        internal DevExpress.XtraLayout.LayoutControlItem lciModeBid;
        internal DevExpress.XtraEditors.CheckEdit rdoThuocNew, rdoVatTuNew, rdoMauNew;
        internal DevExpress.XtraEditors.CheckEdit chkAlertNew, chkShowZeroNew;
        internal DevExpress.XtraEditors.SimpleButton btnTimNew, btnLamLaiNew;
        #endregion

        /// <summary>
        /// Tạo radio "Dự trù theo" + 2 combobox Nhà thầu/Gói thầu, đặt lên đầu hàng lọc (layoutControl3).
        /// Gọi sau InitImpExpFilter (trong Load).
        /// </summary>
        private void InitAnticipateModeControls()
        {
            try
            {
                if (this.layoutControl3 == null || this.layoutControlItem5 == null) return;

                pluginRdoByItemType = CreateModeRadio("Loại mặt hàng", true);
                pluginRdoByItemType.Name = "pluginRdoByItemType";
                pluginRdoBySupply = CreateModeRadio("Thầu", false);
                pluginRdoBySupply.Name = "pluginRdoBySupply";   // KEY ControlState "Dự trù theo: Thầu"
                pluginRdoByItemType.CheckedChanged += pluginRdoMode_CheckedChanged;
                pluginRdoBySupply.CheckedChanged += pluginRdoMode_CheckedChanged;

                pluginCboSupplier = new DevExpress.XtraEditors.GridLookUpEdit();
                pluginCboSupplier.Name = "pluginCboSupplier";
                pluginCboSupplier.Properties.NullText = "Chọn nhà thầu...";
                pluginCboSupplier.EditValueChanged += pluginCboSupplier_EditValueChanged;

                pluginCboBid = new DevExpress.XtraEditors.GridLookUpEdit();
                pluginCboBid.Name = "pluginCboBid";
                pluginCboBid.Properties.NullText = "Chọn gói thầu...";
                pluginCboBid.EditValueChanged += pluginCboBid_EditValueChanged;

                // Hàng "Dự trù theo" ở TRÊN CÙNG layoutControl3 (anchor Top của layoutControlItem5).
                lciModeLabel = this.layoutControl3.AddItem("Dự trù theo:", pluginRdoByItemType, this.layoutControlItem5, DevExpress.XtraLayout.Utils.InsertType.Top);
                lciModeLabel.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
                lciModeLabel.TextSize = new System.Drawing.Size(70, 20);
                lciModeLabel.MaxSize = new System.Drawing.Size(170, 26);
                lciModeLabel.MinSize = new System.Drawing.Size(170, 26);
                lciModeLabel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;

                var lciSupply = this.layoutControl3.AddItem("", pluginRdoBySupply, lciModeLabel, DevExpress.XtraLayout.Utils.InsertType.Right);
                lciSupply.TextVisible = false;
                lciSupply.MaxSize = new System.Drawing.Size(70, 26);
                lciSupply.MinSize = new System.Drawing.Size(70, 26);
                lciSupply.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;

                var lciSupplier = this.layoutControl3.AddItem("Nhà thầu", pluginCboSupplier, lciSupply, DevExpress.XtraLayout.Utils.InsertType.Right);
                lciSupplier.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
                lciSupplier.TextSize = new System.Drawing.Size(60, 20);
                lciSupplier.MinSize = new System.Drawing.Size(220, 26);
                lciSupplier.MaxSize = new System.Drawing.Size(220, 26);
                lciSupplier.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;

                lciModeBid = this.layoutControl3.AddItem("Gói thầu", pluginCboBid, lciSupplier, DevExpress.XtraLayout.Utils.InsertType.Right);
                lciModeBid.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
                lciModeBid.TextSize = new System.Drawing.Size(55, 20);
                lciModeBid.MinSize = new System.Drawing.Size(220, 26);
                lciModeBid.MaxSize = new System.Drawing.Size(220, 26);
                lciModeBid.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;

                // Mặc định: chế độ Loại mặt hàng → ẩn 2 combobox.
                pluginCboSupplier.Visible = false;
                pluginCboBid.Visible = false;

                // Thêm 3 radio Thuốc/VT/Máu (trái) + nhóm Làm lại/Tìm/Hiển thị hết/Tồn cảnh báo (canh phải)
                BuildFilterRowExtras();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Tạo 1 CheckEdit kiểu Radio cho group "Dự trù theo" (GroupIndex=3 — tránh conflict group khác).</summary>
        private DevExpress.XtraEditors.CheckEdit CreateModeRadio(string caption, bool initialChecked)
        {
            var chk = new DevExpress.XtraEditors.CheckEdit();
            chk.Properties.Caption = caption;
            chk.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            chk.Properties.RadioGroupIndex = 3;
            chk.Checked = initialChecked;
            return chk;
        }

        /// <summary>Ẩn các control lọc gốc trên Designer (đã thay bằng control runtime tương đương).</summary>
        // vCong 52461 — control lọc runtime là NGUỒN SỰ THẬT (control Designer gốc đã gỡ bỏ).
        // Helper đọc trạng thái loại/lọc từ control runtime (null-safe).
        private bool IsMedicineChecked { get { return rdoThuocNew != null && rdoThuocNew.Checked; } }
        private bool IsMaterialChecked { get { return rdoVatTuNew != null && rdoVatTuNew.Checked; } }
        private bool IsBloodChecked { get { return rdoMauNew != null && rdoMauNew.Checked; } }
        private bool IsShowLineZeroChecked { get { return chkShowZeroNew != null && chkShowZeroNew.Checked; } }
        private bool IsAlertMinStockChecked { get { return chkAlertNew != null && chkAlertNew.Checked; } }

        /// <summary>
        /// Thêm 3 radio Thuốc/VT/Máu (trái) + nhóm Làm lại/Tìm/Hiển thị dòng hết/Tồn cảnh báo (canh phải)
        /// vào hàng "Dự trù theo". Gọi cuối InitAnticipateModeControls.
        /// </summary>
        private void BuildFilterRowExtras()
        {
            try
            {
                if (this.layoutControl3 == null || lciModeLabel == null || lciModeBid == null) return;
                var group = lciModeBid.Parent as DevExpress.XtraLayout.LayoutControlGroup;

                // ---- Trái: 3 radio Thuốc / Vật tư / Máu (nhóm radio 5) ----
                rdoThuocNew = CreateLoaiRadio("Thuốc", true);
                rdoVatTuNew = CreateLoaiRadio("Vật tư", false);
                rdoMauNew = CreateLoaiRadio("Máu", false);
                rdoThuocNew.Name = "rdoThuocNew";   // KEY ControlState
                rdoVatTuNew.Name = "rdoVatTuNew";
                rdoMauNew.Name = "rdoMauNew";
                rdoThuocNew.CheckedChanged += rdoLoaiNew_CheckedChanged;
                rdoVatTuNew.CheckedChanged += rdoLoaiNew_CheckedChanged;
                rdoMauNew.CheckedChanged += rdoLoaiNew_CheckedChanged;

                // Chèn bên TRÁI nhãn "Dự trù theo" theo thứ tự Thuốc | Vật tư | Máu | Dự trù theo
                var lciMau = this.layoutControl3.AddItem("", rdoMauNew, lciModeLabel, InsertType.Left);
                var lciVT = this.layoutControl3.AddItem("", rdoVatTuNew, lciMau, InsertType.Left);
                var lciThuoc = this.layoutControl3.AddItem("", rdoThuocNew, lciVT, InsertType.Left);
                FixItem(lciThuoc, 66); FixItem(lciVT, 66); FixItem(lciMau, 56);

                // ---- Khoảng trống co giãn → đẩy nhóm phải về sát phải ----
                var esi = new DevExpress.XtraLayout.EmptySpaceItem();
                if (group != null) group.AddItem(esi, lciModeBid, InsertType.Right);

                // ---- Phải (trái→phải hiển thị): Làm lại · Tìm · Hiển thị dòng hết · Tồn cảnh báo ----
                btnLamLaiNew = new DevExpress.XtraEditors.SimpleButton { Text = "Làm lại (Ctrl R)" };
                btnLamLaiNew.Click += (s, e) => { try { btnRefesh_Click_1(null, null); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); } };
                btnTimNew = new DevExpress.XtraEditors.SimpleButton { Text = "Tìm (Ctrl F)" };
                btnTimNew.Click += (s, e) => { try { btnSearch_Click_1(null, null); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); } };
                chkShowZeroNew = new DevExpress.XtraEditors.CheckEdit();
                chkShowZeroNew.Name = "chkShowZeroNew";   // KEY ControlState
                chkShowZeroNew.Properties.Caption = "Hiển thị dòng hết";
                chkShowZeroNew.CheckedChanged += (s, e) => { try { SaveCheckState(chkShowZeroNew.Name, chkShowZeroNew.Checked); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); } };
                chkAlertNew = new DevExpress.XtraEditors.CheckEdit();
                chkAlertNew.Name = "chkAlertNew";         // KEY ControlState
                chkAlertNew.Properties.Caption = "Tồn ở mức cảnh báo tối thiểu";
                chkAlertNew.CheckedChanged += (s, e) => { try { SaveCheckState(chkAlertNew.Name, chkAlertNew.Checked); if (formLoaded) RebindActivePivot(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); } };

                if (esi != null)
                {
                    var lciLamLai = this.layoutControl3.AddItem("", btnLamLaiNew, esi, InsertType.Right);
                    var lciTim = this.layoutControl3.AddItem("", btnTimNew, lciLamLai, InsertType.Right);
                    var lciShow = this.layoutControl3.AddItem("", chkShowZeroNew, lciTim, InsertType.Right);
                    var lciAlert = this.layoutControl3.AddItem("", chkAlertNew, lciShow, InsertType.Right);
                    FixItem(lciLamLai, 110); FixItem(lciTim, 100); FixItem(lciShow, 130); FixItem(lciAlert, 190);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private DevExpress.XtraEditors.CheckEdit CreateLoaiRadio(string caption, bool initialChecked)
        {
            var chk = new DevExpress.XtraEditors.CheckEdit();
            chk.Properties.Caption = caption;
            chk.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            chk.Properties.RadioGroupIndex = 5;
            chk.Checked = initialChecked;
            return chk;
        }

        /// <summary>Radio loại Thuốc/Vật tư/Máu (nguồn sự thật) → lưu ControlState + tải lại cây theo loại.</summary>
        private void rdoLoaiNew_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var chk = sender as DevExpress.XtraEditors.CheckEdit;
                if (chk == null || !chk.Checked) return;   // chỉ xử lý radio vừa được chọn
                // Radio nhóm 5 loại trừ lẫn nhau → lưu "1" cho radio chọn, "" cho 2 radio còn lại.
                if (rdoThuocNew != null) SaveCheckState(rdoThuocNew.Name, rdoThuocNew.Checked);
                if (rdoVatTuNew != null) SaveCheckState(rdoVatTuNew.Name, rdoVatTuNew.Checked);
                if (rdoMauNew != null) SaveCheckState(rdoMauNew.Name, rdoMauNew.Checked);
                if (formLoaded) ShowUCControl();   // đổi loại → nạp lại cây theo loại
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FixItem(DevExpress.XtraLayout.LayoutControlItem lci, int width)
        {
            if (lci == null) return;
            lci.TextVisible = false;
            lci.MaxSize = new System.Drawing.Size(width, 26);
            lci.MinSize = new System.Drawing.Size(width, 26);
            lci.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        }


        /// <summary>Đổi chế độ Dự trù theo: ẩn/hiện 2 combobox + nạp dữ liệu thầu (lazy) + lọc lại TreeList.</summary>
        private void pluginRdoMode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var senderChk = sender as DevExpress.XtraEditors.CheckEdit;
                if (senderChk == null || !senderChk.Checked) return;   // chỉ xử lý radio vừa được chọn

                bool supplyMode = IsSupplyMode;
                // vCong 52461 — lưu trạng thái "Dự trù theo: Thầu"
                if (pluginRdoBySupply != null) SaveCheckState(pluginRdoBySupply.Name, pluginRdoBySupply.Checked);
                if (pluginCboSupplier != null) pluginCboSupplier.Visible = supplyMode;
                if (pluginCboBid != null) pluginCboBid.Visible = supplyMode;

                if (supplyMode)
                {
                    EnsureBidDataLoaded();
                    LoadCboSupplier(ListSupplier);
                    LoadCboBid(ListBid);
                }
                else
                {
                    // Về chế độ Loại mặt hàng → bỏ lọc theo thầu.
                    currentSupplierId = null;
                    currentBidId = null;
                    allowedTypeIdsByBid = null;
                }

                if (!formLoaded) return;
                RebindActivePivot();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Nạp dữ liệu thầu (HIS_BID, HIS_SUPPLIER, view thầu-thuốc/vật tư) — 1 lần.</summary>
        private void EnsureBidDataLoaded()
        {
            try
            {
                if (bidDataLoaded) return;
                CommonParam param = new CommonParam();

                HisBidFilter bidFilter = new HisBidFilter();
                bidFilter.IS_ACTIVE = 1;
                ListBid = new BackendAdapter(param).Get<List<HIS_BID>>("api/HisBid/Get", ApiConsumers.MosConsumer, bidFilter, param) ?? new List<HIS_BID>();

                HisSupplierFilter supplierFilter = new HisSupplierFilter();
                supplierFilter.IS_ACTIVE = 1;
                ListSupplier = new BackendAdapter(param).Get<List<HIS_SUPPLIER>>("api/HisSupplier/Get", ApiConsumers.MosConsumer, supplierFilter, param) ?? new List<HIS_SUPPLIER>();

                long? dateNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);

                HisBidMedicineTypeViewFilter bidMediFilter = new HisBidMedicineTypeViewFilter();
                bidMediFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                ListBidMedicineType = new BackendAdapter(param).Get<List<V_HIS_BID_MEDICINE_TYPE>>("api/HisBidMedicineType/GetView", ApiConsumers.MosConsumer, bidMediFilter, param) ?? new List<V_HIS_BID_MEDICINE_TYPE>();
                ListBidMedicineType = ListBidMedicineType.Where(o => o.VALID_TO_TIME == null || o.VALID_TO_TIME >= dateNow).ToList();

                HisBidMaterialTypeViewFilter bidMateFilter = new HisBidMaterialTypeViewFilter();
                bidMateFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                ListBidMaterialType = new BackendAdapter(param).Get<List<V_HIS_BID_MATERIAL_TYPE>>("api/HisBidMaterialType/GetView", ApiConsumers.MosConsumer, bidMateFilter, param) ?? new List<V_HIS_BID_MATERIAL_TYPE>();
                ListBidMaterialType = ListBidMaterialType.Where(o => o.VALID_TO_TIME == null || o.VALID_TO_TIME >= dateNow).ToList();

                bidDataLoaded = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboSupplier(List<HIS_SUPPLIER> data)
        {
            try
            {
                List<ColumnInfo> cols = new List<ColumnInfo>();
                cols.Add(new ColumnInfo("SUPPLIER_CODE", "Mã", 80, 1));
                cols.Add(new ColumnInfo("SUPPLIER_NAME", "Tên nhà thầu", 250, 2));
                ControlEditorADO ado = new ControlEditorADO("SUPPLIER_NAME", "ID", cols, false, 350);
                ControlEditorLoader.Load(pluginCboSupplier, data ?? new List<HIS_SUPPLIER>(), ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCboBid(List<HIS_BID> data)
        {
            try
            {
                List<ColumnInfo> cols = new List<ColumnInfo>();
                cols.Add(new ColumnInfo("BID_NUMBER", "QĐ thầu", 120, 1));
                cols.Add(new ColumnInfo("BID_NAME", "Tên gói thầu", 250, 2));
                cols.Add(new ColumnInfo("BID_YEAR", "Năm", 60, 3));
                ControlEditorADO ado = new ControlEditorADO("BID_NAME", "ID", cols, false, 450);
                ControlEditorLoader.Load(pluginCboBid, data ?? new List<HIS_BID>(), ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Chọn Nhà thầu → lọc lại danh sách Gói thầu + lọc TreeList.</summary>
        private void pluginCboSupplier_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!IsSupplyMode) return;
                currentSupplierId = ParseId(pluginCboSupplier.EditValue);

                // Lọc gói thầu theo nhà thầu (qua view thầu-thuốc + thầu-vật tư).
                if (currentSupplierId.HasValue && ListBid != null)
                {
                    var bidIds = new HashSet<long>();
                    if (ListBidMedicineType != null)
                        foreach (var o in ListBidMedicineType.Where(o => o.SUPPLIER_ID == currentSupplierId.Value)) bidIds.Add(o.BID_ID);
                    if (ListBidMaterialType != null)
                        foreach (var o in ListBidMaterialType.Where(o => o.SUPPLIER_ID == currentSupplierId.Value)) bidIds.Add(o.BID_ID);
                    LoadCboBid(ListBid.Where(o => bidIds.Contains(o.ID)).ToList());
                }
                else
                {
                    LoadCboBid(ListBid);
                }
                RebuildBidFilterAndRebind();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Chọn Gói thầu → lọc TreeList chỉ còn loại thuộc gói thầu + dựng dict override cột thầu.</summary>
        private void pluginCboBid_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!IsSupplyMode) return;
                currentBidId = ParseId(pluginCboBid.EditValue);
                RebuildBidFilterAndRebind();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Dựng tập loại được phép + dict số liệu thầu theo loại rồi bind lại TreeList.</summary>
        private void RebuildBidFilterAndRebind()
        {
            try
            {
                dicBidMediByType = new Dictionary<long, V_HIS_BID_MEDICINE_TYPE>();
                dicBidMateByType = new Dictionary<long, V_HIS_BID_MATERIAL_TYPE>();
                allowedTypeIdsByBid = new HashSet<long>();

                if (ListBidMedicineType != null)
                {
                    foreach (var o in ListBidMedicineType)
                    {
                        if (currentBidId.HasValue && o.BID_ID != currentBidId.Value) continue;
                        if (currentSupplierId.HasValue && o.SUPPLIER_ID != currentSupplierId.Value) continue;
                        allowedTypeIdsByBid.Add(o.MEDICINE_TYPE_ID);
                        if (!dicBidMediByType.ContainsKey(o.MEDICINE_TYPE_ID)) dicBidMediByType[o.MEDICINE_TYPE_ID] = o;
                    }
                }
                if (ListBidMaterialType != null)
                {
                    foreach (var o in ListBidMaterialType)
                    {
                        if (!o.MATERIAL_TYPE_ID.HasValue) continue;
                        if (currentBidId.HasValue && o.BID_ID != currentBidId.Value) continue;
                        if (currentSupplierId.HasValue && o.SUPPLIER_ID != currentSupplierId.Value) continue;
                        long mtId = o.MATERIAL_TYPE_ID.Value;
                        allowedTypeIdsByBid.Add(mtId);
                        if (!dicBidMateByType.ContainsKey(mtId)) dicBidMateByType[mtId] = o;
                    }
                }

                // Chưa chọn thầu/nhà thầu nào → không lọc.
                if (!currentBidId.HasValue && !currentSupplierId.HasValue)
                    allowedTypeIdsByBid = null;

                if (formLoaded) RebindActivePivot();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Bind lại cây theo loại đang chọn (không gọi lại API tồn/dự trù).</summary>
        private void RebindActivePivot()
        {
            try
            {
                if (IsMedicineChecked && lstMediInStocks != null) BindPivotTree(true);
                else if (IsMaterialChecked && lstMateInStocks != null) BindPivotTree(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static long? ParseId(object editValue)
        {
            if (editValue == null) return null;
            long v;
            return long.TryParse(editValue.ToString(), out v) ? (long?)v : null;
        }

        private void btnBoSungAntc_Click(object sender, EventArgs e)
        {
            try { BoSungFromTree(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        // ---- Public wrappers cho phím tắt (KeyboardWorker) ----
        public void bbtnBoSung() { try { BoSungFromTree(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); } }
        public void bbtnSaveAntc() { try { SaveAnticipateProcess(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); } }
        public void bbtnNewAntc() { try { NewAnticipate(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); } }
        public void bbtnTinh() { try { ShowUCControl(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); } }
        public void bbtnPrintAntc() { try { PrintAnticipate(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); } }

        private void btnSaveAntc_Click(object sender, EventArgs e)
        {
            try { SaveAnticipateProcess(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void btnNewAntc_Click(object sender, EventArgs e)
        {
            try { NewAnticipate(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void btnImportAntc_Click(object sender, EventArgs e)
        {
            try { ImportAnticipateExcel(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>
        /// Ghi lại ngữ cảnh thầu/giá lúc user nhập SL dự trù cho 1 loại (dùng khi build DTO Lưu).
        /// Chế độ Thầu → SUPPLIER_ID/BID_ID theo gói thầu đang chọn, giá = giá thầu sau VAT.
        /// Chế độ Loại mặt hàng → SUPPLIER_ID/BID_ID = null, giá = Giá sau VAT gộp (EXP_PRICE_VAT).
        /// </summary>
        internal void CaptureAnticipateContext(bool isMedicine, long typeId)
        {
            try
            {
                long? supplierId = IsSupplyMode ? currentSupplierId : null;
                long? bidId = IsSupplyMode ? currentBidId : null;
                decimal price = ResolveAnticipatePrice(isMedicine, typeId);
                if (isMedicine)
                {
                    dicMediAnticipateSupplierId[typeId] = supplierId;
                    dicMediAnticipateBidId[typeId] = bidId;
                    dicMediAnticipatePrice[typeId] = price;
                }
                else
                {
                    dicMateAnticipateSupplierId[typeId] = supplierId;
                    dicMateAnticipateBidId[typeId] = bidId;
                    dicMateAnticipatePrice[typeId] = price;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Giá nhập của dòng dự trù: chế độ Thầu = giá thầu sau VAT; ngược lại = EXP_PRICE_VAT gộp.</summary>
        private decimal ResolveAnticipatePrice(bool isMedicine, long typeId)
        {
            if (IsSupplyMode)
            {
                decimal a, b, c, price; string s;
                if (TryGetBidFigures(isMedicine, typeId, out a, out b, out c, out price, out s)) return price;
            }
            var dicAntc = isMedicine ? dicMediAnticipate : dicMateAnticipate;
            ADO.AnticipateRowADO ar;
            if (dicAntc != null && dicAntc.TryGetValue(typeId, out ar) && ar != null) return ar.EXP_PRICE_VAT;
            return 0;
        }

        /// <summary>
        /// Lưu dự trù: đọc từ GRID DỰ TRÙ (anticipateLines) → HIS_ANTICIPATE + collection METY/MATY/BLTY theo Loại.
        /// Lần lưu đầu tiên gọi Create; các lần "Lưu lại" tiếp theo (đã có anticipatePrint) gọi Update theo ID
        /// (Update ở Manager backend là truncate + recreate toàn bộ detail, nên gửi lại nguyên trạng anticipateLines là đủ).
        /// </summary>
        private void SaveAnticipateProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                if (gridViewAnticipate != null) gridViewAnticipate.CloseEditor();

                var model = new HIS_ANTICIPATE();
                model.HIS_ANTICIPATE_METY = new List<HIS_ANTICIPATE_METY>();
                model.HIS_ANTICIPATE_MATY = new List<HIS_ANTICIPATE_MATY>();
                model.HIS_ANTICIPATE_BLTY = new List<HIS_ANTICIPATE_BLTY>();

                foreach (var line in anticipateLines)
                {
                    if (line == null || !line.Amount.HasValue || line.Amount.Value <= 0) continue;
                    if (line.Type == ADO.AnticipateLineType.THUOC)
                        model.HIS_ANTICIPATE_METY.Add(new HIS_ANTICIPATE_METY
                        { MEDICINE_TYPE_ID = line.TypeId, AMOUNT = line.Amount.Value, IMP_PRICE = line.ImpPrice, SUPPLIER_ID = line.SupplierId, BID_ID = line.BidId, NOTE = line.Note });
                    else if (line.Type == ADO.AnticipateLineType.VATTU)
                        model.HIS_ANTICIPATE_MATY.Add(new HIS_ANTICIPATE_MATY
                        { MATERIAL_TYPE_ID = line.TypeId, AMOUNT = line.Amount.Value, IMP_PRICE = line.ImpPrice, SUPPLIER_ID = line.SupplierId, BID_ID = line.BidId, NOTE = line.Note });
                    else if (line.Type == ADO.AnticipateLineType.MAU)
                        model.HIS_ANTICIPATE_BLTY.Add(new HIS_ANTICIPATE_BLTY
                        { BLOOD_TYPE_ID = line.TypeId, AMOUNT = line.Amount.Value, IMP_PRICE = line.ImpPrice, SUPPLIER_ID = line.SupplierId, BID_ID = line.BidId, NOTE = line.Note });
                }

                if (model.HIS_ANTICIPATE_METY.Count == 0 && model.HIS_ANTICIPATE_MATY.Count == 0 && model.HIS_ANTICIPATE_BLTY.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa có dòng dự trù nào trong danh sách để lưu (bấm Bổ sung để đưa dòng xuống).", "Thông báo");
                    return;
                }

                // Header
                var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.RoomId);
                var stock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.RoomId);
                model.REQUEST_ROOM_ID = this.RoomId;
                model.REQUEST_DEPARTMENT_ID = room != null ? room.DEPARTMENT_ID : (long?)null;
                model.RECEIVE_MEDI_STOCK_ID = stock != null ? stock.ID : (long?)null;
                model.REQUEST_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                model.REQUEST_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();

                bool isUpdate = anticipatePrint != null;
                string uri = RequestUriStore.HIS_ANTICIPATE_CREATE;
                if (isUpdate)
                {
                    model.ID = anticipatePrint.ID;
                    uri = RequestUriStore.HIS_ANTICIPATE_UPDATE;
                }

                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<HIS_ANTICIPATE>(uri, ApiConsumers.MosConsumer, model, param);
                WaitingManager.Hide();

                bool success = result != null;
                MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    anticipatePrint = result;
                    BackendDataWorker.Reset<HIS_ANTICIPATE>();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Import Excel dự trù — đang hoàn thiện (cần chốt định dạng file mẫu). Xem todo 8.</summary>
        private void ImportAnticipateExcel()
        {
            DevExpress.XtraEditors.XtraMessageBox.Show("Chức năng Import đang được hoàn thiện.", "Thông báo");
        }

        /// <summary>Xóa toàn bộ SL dự trù/ghi chú/ngữ cảnh đã nhập trên cây (theo typeId).</summary>
        private void ClearAnticipateInputDicts()
        {
            dicMediAnticipateQty.Clear(); dicMateAnticipateQty.Clear();
            dicMediAnticipateNote.Clear(); dicMateAnticipateNote.Clear();
            dicMediAnticipateSupplierId.Clear(); dicMateAnticipateSupplierId.Clear();
            dicMediAnticipateBidId.Clear(); dicMateAnticipateBidId.Clear();
            dicMediAnticipatePrice.Clear(); dicMateAnticipatePrice.Clear();
        }

        /// <summary>Làm mới phiếu dự trù: xóa grid dự trù + SL dự trù/ghi chú đã nhập trên cây + bind lại cây.</summary>
        private void NewAnticipate()
        {
            try
            {
                if (anticipateLines != null) anticipateLines.Clear();
                if (gridViewAnticipate != null) gridViewAnticipate.RefreshData();
                ClearAnticipateInputDicts();
                anticipatePrint = null;
                if (formLoaded) RebindActivePivot();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lấy số liệu thầu theo loại (chế độ Thầu) để override cột SL thầu/đã nhập/còn lại/giá VAT/NCC.
        /// SL thầu = AMOUNT + ADJUST; Còn lại = AMOUNT + ADJUST + AMOUNT*IMP_MORE_RATIO − IN_AMOUNT; Giá VAT = IMP_PRICE*(1+IMP_VAT_RATIO).
        /// </summary>
        internal bool TryGetBidFigures(bool isMedicine, long typeId,
            out decimal bidAmount, out decimal bidImported, out decimal bidRemain, out decimal priceVat, out string supplierName)
        {
            bidAmount = 0; bidImported = 0; bidRemain = 0; priceVat = 0; supplierName = null;
            try
            {
                if (isMedicine)
                {
                    V_HIS_BID_MEDICINE_TYPE b;
                    if (dicBidMediByType == null || !dicBidMediByType.TryGetValue(typeId, out b) || b == null) return false;
                    decimal amount = b.AMOUNT, adjust = b.ADJUST_AMOUNT ?? 0, moreRatio = b.IMP_MORE_RATIO ?? 0, inAmount = b.IN_AMOUNT ?? 0;
                    bidAmount = amount + adjust;
                    bidImported = inAmount;
                    bidRemain = amount + adjust + amount * moreRatio - inAmount;
                    priceVat = (b.IMP_PRICE ?? 0) * (1 + (b.IMP_VAT_RATIO ?? 0));
                    supplierName = b.SUPPLIER_NAME;
                    return true;
                }
                else
                {
                    V_HIS_BID_MATERIAL_TYPE b;
                    if (dicBidMateByType == null || !dicBidMateByType.TryGetValue(typeId, out b) || b == null) return false;
                    decimal amount = b.AMOUNT, adjust = b.ADJUST_AMOUNT ?? 0, moreRatio = b.IMP_MORE_RATIO ?? 0, inAmount = b.IN_AMOUNT ?? 0;
                    bidAmount = amount + adjust;
                    bidImported = inAmount;
                    bidRemain = amount + adjust + amount * moreRatio - inAmount;
                    priceVat = (b.IMP_PRICE ?? 0) * (1 + (b.IMP_VAT_RATIO ?? 0));
                    supplierName = b.SUPPLIER_NAME;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }
    }
}
