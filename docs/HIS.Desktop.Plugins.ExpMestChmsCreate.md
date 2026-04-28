# Xuất Chuyển Kho (ExpMestChmsCreate) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExpMestChmsCreate |
| Loại | UserControl |
| Mục đích | Tạo phiếu xuất chuyển kho thuốc/vật tư/máu giữa các kho dược (chuyển cho mượn / hoàn trả) |
| Ngày cập nhật | 2026-04-25 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. User chọn kho xuất (`cboExpMediStock`), kho nhập (`cboImpMediStock`) và hướng chuyển (radioImport / radioExport).
2. UI hiển thị tab Thuốc / Vật tư / Máu — mỗi tab có grid danh sách tồn kho với cột nhập **Số lượng xuất chuyển** (`EXP_AMOUNT`).
3. User nhập số lượng cho nhiều dòng trên lưới → bấm **Thêm** 1 lần → gom toàn bộ dòng có `EXP_AMOUNT > 0` chuyển sang grid bên phải (`gridControlExpMestChmsDetail`).
4. User kiểm tra lại → bấm **Lưu** → hệ thống tạo phiếu xuất (`api/HisExpMest/ChmsCreate`) hoặc cập nhật (`api/HisExpMest/ChmsUpdate`).

### Hai mode
- `chkPlanningExport.Checked = TRUE` — Kế hoạch xuất: batch add nhiều dòng 1 lần (phạm vi PTTK 36619).
- `chkPlanningExport.Checked = FALSE` — Add từng dòng qua `currentMediMate` + `spinExpAmount` + `txtNote`.

## 3. EFMODEL / SDO Sử Dụng

| Entity / SDO | Mục đích |
|--------------|---------|
| V_HIS_EXP_MEST | Phiếu xuất chuyển |
| HIS_EXP_MEST_METY_REQ | Yêu cầu xuất chuyển theo loại |
| V_HIS_EXP_MEST_MEDICINE / _MATERIAL / _BLOOD | Dòng thuốc / vật tư / máu đã xuất |
| HisExpMestChmsSDO | Payload tạo phiếu (mode kế hoạch) |
| HisExpMestChmsListSDO | Payload tạo danh sách phiếu (mode thường) |
| MediMateTypeADO (ADO/) | Gom thông tin 1 dòng (thuốc / vật tư / máu) để bind grid phải |
| HisMedicineInStockADO / HisMaterialInStockADO | Dữ liệu tồn kho, có field `EXP_AMOUNT` cho user nhập |

## 4. UI Layout

```
+-------------------------------------------------------------------+
| [chkPlanningExport] [radioImport|radioExport] [cboExpMediStock]  |
| [cboImpMediStock] [cboReasonRequired] [txtDescription]           |
+-------------------------------------------------------------------+
| TabControlMain:                         | Grid bên phải:           |
| ┌─ Thuốc (gridControlMedicine)        │ gridControlExpMestChmsDetail|
| │  cột EXP_AMOUNT cho phép edit inline│ (danh sách đã thêm — nhóm  |
| ├─ Vật tư (gridControlMaterial)        │  theo kho, có cột Xóa,    |
| │  cột EXP_AMOUNT cho phép edit inline│  Tên, SL xuất, đơn giá...) |
| └─ Máu (gridControlBloodType__BloodPage)                           |
|                                         |                            |
| [spinExpAmount][txtNote]  ← dùng cho mode chkPlanningExport=FALSE   |
+-------------------------------------------------------------------+
| [btnAdd Thêm (Ctrl+A)] [btnSave Lưu (Ctrl+S)] [ddBtnPrint In]      |
+-------------------------------------------------------------------+
```

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Tạo phiếu xuất chuyển (kế hoạch) | api/HisExpMest/ChmsCreate | MosConsumer |
| Cập nhật phiếu xuất chuyển | api/HisExpMest/ChmsUpdate | MosConsumer |
| Tạo danh sách phiếu (mode thường) | api/HisExpMest/ChmsCreateList | MosConsumer |

## 6. Dependencies

### Library Plugins
- `HIS.Desktop.Plugins.Library.EmrGenerate` — tạo input ký số EMR khi in phiếu.

### Inter-Plugin
- Không trực tiếp mở plugin khác; có thể được mở từ plugin quản lý phiếu xuất chuyển khác qua `PluginInstance.GetPluginInstance`.

## 7. Print

- Sử dụng `MPS.MpsPrinter.Run` với PDO tương ứng (Mps000086 cho phiếu xuất chuyển kho).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-04-24 | Codex / Claude | PTTK 36619 (BV HAGL): (1) Trong mode `chkPlanningExport = TRUE`, đổi filter `btnAdd_Click` từ `Where(IsCheck)` sang `Where(EXP_AMOUNT > 0)` — user nhập số lượng cho nhiều dòng trên lưới rồi bấm Thêm 1 lần là đủ; bỏ `return` khi 1 dòng có `EXP_AMOUNT <= 0` để không chặn cả batch. (2) Thêm cột **Ghi chú** (FieldName `NOTE`) vào grid bên trái `gridViewMedicine` và `gridViewMaterial`, cho phép edit inline; bổ sung property `NOTE` vào `HisMedicineInStockADO` và `HisMaterialInStockADO`; khi bấm Thêm, `NOTE` được copy sang `MediMateTypeADO.NOTE` và `ExpMedicine/ExpMaterial.Description` để lưu phiếu. (3) BR04: Xóa dòng `this.EXP_AMOUNT = Math.Min(AvailableAmount, ExportedTotalAmount)` trong constructor của `HisMedicineInStockADO` và `HisMaterialInStockADO` để không tự động sinh giá trị mặc định cho `EXP_AMOUNT` — user phải tự nhập. |
| 2026-04-25 | dangth2 / Claude | PTTK 36619 (BV HAGL) — bổ sung cột mới riêng biệt cho luồng nhập trực tiếp: (1) Thêm property `AMOUNT_TRANSFER_MEDICINE`/`NOTE_TRANSFER_MEDICINE` vào `HisMedicineInStockADO`, `AMOUNT_TRANSFER_MATERIAL`/`NOTE_TRANSFER_MATERIAL` vào `HisMaterialInStockADO` (BR04: không auto-fill). (2) Designer.cs: 4 cột `gridColumnMedicine_SLXuatchuyen`/`gridColumnMedicine_NOTEXuatChuyen`/`gridColumnMaterial_SLXuatChuyen`/`gridColumnMaterial_NOTEXuatChuyen` gán `ColumnEdit = repositoryItemSpinTransferAmountCreate`/`repositoryItemTextTransferNoteCreate`, AllowEdit=true; gán repositoryItems vào `gridControlMedicine`+`gridControlMaterial`. (3) Tạo file partial `UCExpMestChmsCreate___Plus___TransferBatch.cs` với method `TryBatchAddFromTransferColumns()`. (4) `btnAdd_Click`: thêm nhánh batch ở đầu — nếu có dòng AMOUNT_TRANSFER_MEDICINE/MATERIAL > 0 thì batch add tất cả vào `dicMediMateAdo` và return; ngược lại fallback xuống 2 nhánh cũ (chkPlanningExport / single-item). Sau add, reset cell trên grid để không add trùng. Tất cả code cũ giữ nguyên. |

## 9. Test Cases (PTTK 36619)

### Nhập hàng loạt số lượng và bấm Thêm một lần
- [ ] Mở form, bật `chkPlanningExport`, chọn kho xuất + nhập.
- [ ] Tại tab Thuốc / Vật tư, nhập `EXP_AMOUNT > 0` cho nhiều dòng.
- [ ] Bấm Thêm → toàn bộ dòng có `EXP_AMOUNT > 0` chuyển sang grid bên phải trong 1 lần.
- [ ] Dòng có `EXP_AMOUNT` rỗng / 0 / âm KHÔNG được chuyển, không chặn các dòng khác.
- [ ] Nếu có dòng `EXP_AMOUNT > AvailableAmount` → cảnh báo vượt tồn kho, cho phép tiếp tục.

### Lưu phiếu
- [ ] Sau khi grid phải có dữ liệu, bấm Lưu → tạo phiếu thành công (`api/HisExpMest/ChmsCreate`).
