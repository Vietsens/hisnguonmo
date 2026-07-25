/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong 52461 - Tạo dự trù v2: GRID DỰ TRÙ (bên dưới cây) + Bổ sung (Ctrl A).
 * - Panel kết quả tách đôi (SplitContainer dọc): trên = cây kết quả, dưới = grid dự trù.
 * - Nhập SL dự trù trên cây → Bổ sung → đưa dòng loại (có SL dự trù > 0) xuống grid.
 * - Grid cho sửa SL dự trù / Giá nhập / Ghi chú, nút X đỏ xóa dòng.
 * - Lưu đọc từ danh sách dòng grid này.
 */
using DevExpress.XtraGrid.Views.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AnticipateCreateV2
{
    public partial class UCAnticipateCreateV2
    {
        #region ---Dutru grid fields
        // Tách panel kết quả: Panel1 = cây, Panel2 = grid dự trù.
        internal DevExpress.XtraEditors.SplitContainerControl splitResult;
        internal DevExpress.XtraGrid.GridControl gridControlAnticipate;
        internal DevExpress.XtraGrid.Views.Grid.GridView gridViewAnticipate;

        // Danh sách dòng dự trù đã Bổ sung — nguồn dữ liệu grid + nguồn Lưu.
        internal BindingList<ADO.AnticipateLineADO> anticipateLines = new BindingList<ADO.AnticipateLineADO>();

        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit riLineAmount;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit riLinePrice;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit riLineNote;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit riLineDelete;

        // vCong 52461 — thanh nút thao tác ở đáy grid dự trù (góc dưới phải) + nút In runtime.
        internal DevExpress.XtraEditors.SimpleButton btnPrintAntc;
        internal FlowLayoutPanel pnlDutruAction;
        #endregion

        /// <summary>
        /// Tách panel kết quả thành cây (trên) + grid dự trù (dưới); dựng grid dự trù + cột.
        /// Gọi trong constructor SAU InitPivotTrees, TRƯỚC khi hiển thị cây lần đầu.
        /// </summary>
        private void InitDutruGrid()
        {
            try
            {
                if (this.panelControlMediMate == null) return;

                splitResult = new DevExpress.XtraEditors.SplitContainerControl();
                splitResult.Horizontal = false;               // Panel1 trên, Panel2 dưới
                splitResult.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
                splitResult.Dock = DockStyle.Fill;
                splitResult.Panel1.Text = "";
                splitResult.Panel2.Text = "";

                gridControlAnticipate = new DevExpress.XtraGrid.GridControl();
                gridViewAnticipate = new DevExpress.XtraGrid.Views.Grid.GridView();
                gridControlAnticipate.ViewCollection.Add(gridViewAnticipate);
                gridControlAnticipate.MainView = gridViewAnticipate;
                gridControlAnticipate.Dock = DockStyle.Fill;

                gridViewAnticipate.OptionsView.ShowGroupPanel = false;
                gridViewAnticipate.OptionsView.ColumnAutoWidth = false;   // nhiều cột → scroll ngang (bắt buộc để Fixed cột chạy)
                gridViewAnticipate.OptionsBehavior.Editable = true;
                gridViewAnticipate.OptionsSelection.MultiSelect = false;
                gridViewAnticipate.OptionsView.EnableAppearanceEvenRow = true;
                // vCong 52461 — tham khảo gridViewProcess (plugin AnticipateCreate cũ): thêm hàng lọc + nút luôn hiện + bỏ indicator
                gridViewAnticipate.OptionsView.ShowAutoFilterRow = true;
                gridViewAnticipate.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
                gridViewAnticipate.OptionsView.ShowIndicator = false;
                gridViewAnticipate.OptionsView.ShowDetailButtons = false;
                gridViewAnticipate.CustomUnboundColumnData += gridViewAnticipate_CustomUnboundColumnData;

                BuildDutruColumns();

                gridControlAnticipate.DataSource = anticipateLines;

                // Grid (Dock=Fill) thêm TRƯỚC, thanh nút (Dock=Bottom) thêm SAU → nút nằm đáy, grid fill phần còn lại
                splitResult.Panel2.Controls.Add(gridControlAnticipate);
                BuildBottomActionBar();
                if (pnlDutruAction != null) splitResult.Panel2.Controls.Add(pnlDutruAction);

                this.panelControlMediMate.Controls.Clear();
                this.panelControlMediMate.Controls.Add(splitResult);

                // Panel2 (grid dự trù) chiếm ~40% chiều cao
                try { splitResult.SplitterPosition = 240; } catch { }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Nơi đặt cây kết quả: Panel1 của split nếu đã tách, ngược lại panel gốc.</summary>
        internal Control GetTreeHostPanel()
        {
            if (splitResult != null && splitResult.Panel1 != null) return splitResult.Panel1;
            return this.panelControlMediMate;
        }

        private DevExpress.XtraGrid.Columns.GridColumn AddGridCol(string caption, string field, int width,
            bool bound, bool allowEdit, string format)
        {
            var col = gridViewAnticipate.Columns.AddVisible(field, caption);
            col.Width = width;
            col.OptionsColumn.AllowEdit = allowEdit;
            if (!bound) col.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            if (!string.IsNullOrEmpty(format))
            {
                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                col.DisplayFormat.FormatString = format;
            }
            return col;
        }

        private void BuildDutruColumns()
        {
            riLineAmount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            riLineAmount.MinValue = 0; riLineAmount.MaxValue = 9999999999; riLineAmount.IsFloatValue = true;
            riLineAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric; riLineAmount.DisplayFormat.FormatString = "#,##0.##";
            riLinePrice = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            riLinePrice.MinValue = 0; riLinePrice.MaxValue = 999999999999; riLinePrice.IsFloatValue = true;
            riLinePrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric; riLinePrice.DisplayFormat.FormatString = "#,##0";
            riLineNote = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            riLineDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            riLineDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            riLineDelete.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Delete;
            riLineDelete.ButtonClick += riLineDelete_ButtonClick;

            gridControlAnticipate.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[]
            { riLineAmount, riLinePrice, riLineNote, riLineDelete });

            gridViewAnticipate.Columns.Clear();

            var colDel = AddGridCol("Xóa", "COL_DELETE", 40, false, true, null);
            colDel.ColumnEdit = riLineDelete;
            colDel.OptionsColumn.ShowInCustomizationForm = false;
            colDel.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;   // vCong 52461 — nút Xóa cố định trái

            var colStt = AddGridCol("STT", "COL_STT", 40, false, false, null);
            colStt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;

            var colCode = AddGridCol("Mã", "Code", 100, true, false, null);
            colCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;   // Mã cố định trái
            var colName = AddGridCol("Tên", "Name", 220, true, false, null);
            colName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;   // Tên cố định trái
            AddGridCol("Hoạt chất", "ActiveIngrName", 140, true, false, null);
            AddGridCol("Nồng độ", "Concentra", 80, true, false, null);
            AddGridCol("ĐVT", "UnitName", 55, true, false, null);
            AddGridCol("Loại", "Type", 60, true, false, null);
            AddGridCol("Hãng SX", "ManufacturerName", 130, true, false, null);
            AddGridCol("Nhà cung cấp", "SupplierName", 140, true, false, null);
            AddGridCol("Tồn đầu", "OpenQuantity", 85, true, false, "#,##0.##");
            AddGridCol("Nhập mới", "NewImport", 80, true, false, "#,##0.##");
            AddGridCol("Số sử dụng", "Used", 85, true, false, "#,##0.##");
            AddGridCol("Tồn cuối", "CloseQuantity", 85, true, false, "#,##0.##");
            AddGridCol("Xuất nhiều nhất", "MaxExportDisplay", 150, true, false, null);
            AddGridCol("SL thầu", "BidAmount", 85, true, false, "#,##0.##");
            AddGridCol("Thầu đã nhập", "BidImported", 90, true, false, "#,##0.##");
            AddGridCol("Thầu còn lại", "BidRemain", 90, true, false, "#,##0.##");

            var colAmount = AddGridCol("SL dự trù", "Amount", 90, true, true, "#,##0.##");
            colAmount.ColumnEdit = riLineAmount;
            var colNote = AddGridCol("Ghi chú", "Note", 150, true, true, null);
            colNote.ColumnEdit = riLineNote;
            var colPrice = AddGridCol("Giá nhập", "ImpPrice", 100, true, true, "#,##0");
            colPrice.ColumnEdit = riLinePrice;

            AddGridCol("Nhà thầu", "SupplierName2", 140, false, false, null);
            AddGridCol("Gói thầu", "BidName", 150, true, false, null);
        }

        private void gridViewAnticipate_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                if (e.Column.FieldName == "COL_STT")
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
                else if (e.Column.FieldName == "SupplierName2")
                {
                    var row = e.Row as ADO.AnticipateLineADO;
                    if (row != null) e.Value = row.SupplierName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void riLineDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind != DevExpress.XtraEditors.Controls.ButtonPredefines.Delete) return;
                var row = gridViewAnticipate.GetFocusedRow() as ADO.AnticipateLineADO;
                if (row == null) return;
                gridViewAnticipate.CloseEditor();
                anticipateLines.Remove(row);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bổ sung (Ctrl A): đưa các dòng loại đang hiển thị trên cây có SL dự trù > 0 xuống grid dự trù.
        /// Gộp theo (TypeId, BidId, SupplierId) — Bổ sung lại sẽ cập nhật SL.
        /// </summary>
        internal void BoSungFromTree()
        {
            try
            {
                bool isMedicine;
                if (IsMedicineChecked) isMedicine = true;
                else if (IsMaterialChecked) isMedicine = false;
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chế độ Máu chưa hỗ trợ Bổ sung ở phiên bản này.", "Thông báo");
                    return;
                }

                string lineType = isMedicine ? ADO.AnticipateLineType.THUOC : ADO.AnticipateLineType.VATTU;
                var dicQty = isMedicine ? dicMediAnticipateQty : dicMateAnticipateQty;
                var dicNote = isMedicine ? dicMediAnticipateNote : dicMateAnticipateNote;
                var dicSup = isMedicine ? dicMediAnticipateSupplierId : dicMateAnticipateSupplierId;
                var dicBid = isMedicine ? dicMediAnticipateBidId : dicMateAnticipateBidId;
                var dicPrice = isMedicine ? dicMediAnticipatePrice : dicMateAnticipatePrice;
                var dicAntc = isMedicine ? dicMediAnticipate : dicMateAnticipate;

                // Tập typeId của các dòng loại (leaf) đang hiển thị trên cây, có SL dự trù > 0
                var leafTypeIds = new List<long>();
                if (isMedicine)
                {
                    if (lstMediInStocks != null)
                        leafTypeIds = lstMediInStocks.Where(o => o != null && o.isTypeNode && (o.IS_LEAF ?? 0) == 1)
                            .Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList();
                }
                else
                {
                    if (lstMateInStocks != null)
                        leafTypeIds = lstMateInStocks.Where(o => o != null && o.isTypeNode && (o.IS_LEAF ?? 0) == 1)
                            .Select(o => o.MATERIAL_TYPE_ID).Distinct().ToList();
                }

                int added = 0;
                foreach (long typeId in leafTypeIds)
                {
                    decimal? qty;
                    if (!dicQty.TryGetValue(typeId, out qty) || !qty.HasValue || qty.Value <= 0) continue;

                    long? supId; dicSup.TryGetValue(typeId, out supId);
                    long? bidId; dicBid.TryGetValue(typeId, out bidId);
                    decimal price; dicPrice.TryGetValue(typeId, out price);
                    string note; dicNote.TryGetValue(typeId, out note);
                    ADO.AnticipateRowADO ar = null; if (dicAntc != null) dicAntc.TryGetValue(typeId, out ar);

                    var existing = anticipateLines.FirstOrDefault(o => o.TypeId == typeId && o.Type == lineType
                        && o.BidId == bidId && o.SupplierId == supId);
                    if (existing != null)
                    {
                        existing.Amount = qty;
                        existing.Note = note;
                        existing.ImpPrice = price;
                    }
                    else
                    {
                        anticipateLines.Add(new ADO.AnticipateLineADO
                        {
                            TypeId = typeId,
                            Type = lineType,
                            Code = ar != null ? (isMedicine ? ar.MEDICINE_TYPE_CODE : ar.MATERIAL_TYPE_CODE) : null,
                            Name = ar != null ? (isMedicine ? ar.MEDICINE_TYPE_NAME : ar.MATERIAL_TYPE_NAME) : null,
                            ActiveIngrName = ar != null ? ar.ACTIVE_INGR_BHYT_NAME : null,
                            Concentra = ar != null ? ar.CONCENTRA : null,
                            UnitName = ar != null ? ar.UNIT_NAME : null,
                            ManufacturerName = ar != null ? ar.MANUFACTURER_NAME : null,
                            SupplierId = supId,
                            SupplierName = ar != null ? ar.SUPPLIER_NAME : null,
                            BidId = bidId,
                            BidName = GetBidName(bidId),
                            BidAmount = ar != null ? (decimal?)ar.BID_AMOUNT : null,
                            BidImported = ar != null ? (decimal?)ar.BID_IMPORTED_AMOUNT : null,
                            BidRemain = ar != null ? (decimal?)ar.BID_REMAIN_AMOUNT : null,
                            OpenQuantity = ar != null ? (decimal?)ar.OPEN_QUANTITY : null,
                            NewImport = ar != null ? (decimal?)ar.NEW_IMPORT_QUANTITY : null,
                            Used = ar != null ? (decimal?)ar.USED_QUANTITY : null,
                            CloseQuantity = ar != null ? (decimal?)ar.CLOSE_QUANTITY : null,
                            MaxExport = ar != null ? (decimal?)ar.MAX_EXPORT_QUANTITY : null,
                            MaxExportMonth = ar != null ? ar.MAX_EXPORT_MONTH : 0,
                            ImpPrice = price,
                            Amount = qty,
                            Note = note
                        });
                        added++;
                    }
                }

                if (added == 0 && !anticipateLines.Any())
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa có dòng nào có SL dự trù > 0 để bổ sung.", "Thông báo");
                }
                gridViewAnticipate.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Tên gói thầu theo BID_ID (từ ListBid nếu đã nạp).</summary>
        private string GetBidName(long? bidId)
        {
            try
            {
                if (!bidId.HasValue || ListBid == null) return null;
                var b = ListBid.FirstOrDefault(o => o.ID == bidId.Value);
                return b != null ? b.BID_NAME : null;
            }
            catch { return null; }
        }

        /// <summary>Thanh nút thao tác ở ĐÁY grid dự trù (góc dưới phải): Bổ sung · Import · Lưu · In · Mới.</summary>
        private void BuildBottomActionBar()
        {
            try
            {
                pnlDutruAction = new FlowLayoutPanel();
                pnlDutruAction.Dock = DockStyle.Bottom;
                pnlDutruAction.Height = 38;
                pnlDutruAction.FlowDirection = FlowDirection.RightToLeft;   // add trước → nằm phải nhất
                pnlDutruAction.WrapContents = false;
                pnlDutruAction.Padding = new Padding(6, 5, 6, 5);

                btnNewAntc = CreateActionButton("Mới (Ctrl N)", btnNewAntc_Click);
                btnPrintAntc = CreateActionButton("In (Ctrl P)", btnPrintAntc_Click);
                btnSaveAntc = CreateActionButton("Lưu (Ctrl S)", btnSaveAntc_Click);
                btnImportAntc = CreateActionButton("Import", btnImportAntc_Click);
                btnBoSungAntc = CreateActionButton("Bổ sung (Ctrl A)", btnBoSungAntc_Click);

                // RightToLeft: phần tử thêm trước ở bên phải → Mới ngoài cùng phải.
                // Hiển thị trái→phải: Bổ sung · Import · Lưu · In · Mới
                pnlDutruAction.Controls.Add(btnNewAntc);
                pnlDutruAction.Controls.Add(btnPrintAntc);
                pnlDutruAction.Controls.Add(btnSaveAntc);
                pnlDutruAction.Controls.Add(btnImportAntc);
                pnlDutruAction.Controls.Add(btnBoSungAntc);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private DevExpress.XtraEditors.SimpleButton CreateActionButton(string text, EventHandler onClick)
        {
            var b = new DevExpress.XtraEditors.SimpleButton();
            b.Text = text;
            b.Height = 26;
            b.Width = 110;
            b.Margin = new Padding(3, 1, 3, 1);
            b.Click += onClick;
            return b;
        }

        private void btnPrintAntc_Click(object sender, EventArgs e)
        {
            try { PrintAnticipate(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }
    }
}
