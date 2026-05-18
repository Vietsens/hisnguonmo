# Sửa Phiếu Xuất Chuyển Kho (ExpMestChmsUpdate) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExpMestChmsUpdate |
| Loại | Form |
| Mục đích | Sửa (cập nhật) phiếu xuất chuyển kho đã có — điều chỉnh số lượng xuất chuyển, ghi chú, thêm/xoá thuốc/vật tư/máu trong phiếu |
| Ngày cập nhật | 2026-04-25 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. User mở phiếu xuất chuyển từ danh sách → form `frmExpMestChmsUpdate` load dữ liệu hiện có vào `dicMediMateAdo`.
2. Grid bên trái (tab Thuốc / Vật tư / Máu) hiển thị danh mục tồn kho cho user thêm mới dòng; grid bên phải (`gridControlExpMestChmsDetail`) hiển thị danh sách thuốc/vật tư/máu đã thuộc phiếu.
3. User có 2 cách chỉnh sửa **Số lượng xuất chuyển** và **Ghi chú** trên dòng của grid bên phải:
   - **Edit trực tiếp trên grid** (2 cột `EXP_AMOUNT` và `NOTE`).
   - **Nhập ở vùng `spinExpAmount` + `txtNote` phía dưới grid** — khi click chọn dòng, dữ liệu dòng đó tự động load xuống; khi sửa ở 2 ô này, dòng đang chọn trên grid đồng bộ tương ứng.
4. User bấm Lưu → `api/HisExpMest/ChmsUpdate`.

## 3. EFMODEL / SDO Sử Dụng

| Entity / SDO | Mục đích |
|--------------|---------|
| V_HIS_EXP_MEST | Phiếu xuất chuyển (gán vào `hisExpMest`) |
| V_HIS_EXP_MEST_MEDICINE / _MATERIAL / _BLOOD | Dòng thuốc / vật tư / máu đã xuất |
| HIS_EXP_MEST_METY_REQ | Yêu cầu xuất chuyển theo loại |
| MediMateTypeADO | ADO gom thông tin dòng (có `EXP_AMOUNT`, `NOTE`, `ExpMedicine`, `ExpMaterial`, `ExpBlood`) |
| HisExpMestChmsSDO | Payload cập nhật phiếu |

## 4. UI Layout

```
+------------------------------------------------------------------------+
| TabControl bên trái:                    | gridControlExpMestChmsDetail |
| ┌─ Thuốc (gridControlMedicine)        │   (Danh sách đã thêm vào phiếu)|
| ├─ Vật tư (gridControlMaterial)        │                                 |
| └─ Máu (gridControlBloodType)         │   Cột: Xoá, Sửa, Tên, ĐVT,     |
|                                         │   SL xuất (EXP_AMOUNT — edit   |
| [spinExpAmount][txtNote][chkHienThiLo]  │   inline), Nước sản xuất, hãng,|
|  (đồng bộ 2 chiều với dòng đang chọn)  │   dung tích, ABO, RH, số lô,   |
|                                         │   Ghi chú (NOTE — edit inline) |
+------------------------------------------------------------------------+
| [btnAddd Thêm] [btnCapNhat Cập nhật] [btnSave Lưu] [ddBtnPrint In]     |
+------------------------------------------------------------------------+
```

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Cập nhật phiếu xuất chuyển | api/HisExpMest/ChmsUpdate | MosConsumer |

## 6. Dependencies

### Library Plugins
- `HIS.Desktop.Plugins.Library.EmrGenerate` — ký số EMR khi in.
- `MPS.Processor.Mps000086.PDO` — PDO in phiếu xuất chuyển.

## 7. Print

- `MPS.MpsPrinter.Run` với PDO Mps000086 cho phiếu xuất chuyển kho.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-04-24 | Codex / Claude | PTTK 36619 (BV HAGL): Bổ sung cột **Ghi chú** (`NOTE`) vào `gridViewExpMestChmsDetail`, cho phép chỉnh sửa trực tiếp trên grid. Thêm đồng bộ 2 chiều giữa `gridViewExpMestChmsDetail` và vùng nhập `spinExpAmount` + `txtNote` qua `FocusedRowChanged` + `CellValueChanged` + `EditValueChanged`. Có flag `isSyncingDetailRow` để chặn loop sync. |
| 2026-04-24 | Codex / Claude | PTTK 36619 (BV HAGL) — bổ sung: Tạo 2 ADO mới `HisMedicineInStockADO`, `HisMaterialInStockADO` (kế thừa `HisMedicineInStockSDO`/`HisMaterialInStockSDO` + 2 property `EXP_AMOUNT`, `NOTE`). Đổi type `listMediInStock`, `listMateInStock` từ `List<SDO>` → `List<ADO>`; convert SDO → ADO qua Mapper khi load data từ API (BR04: không mặc định `EXP_AMOUNT`). Thêm 2 cột **SL xuất chuyển** + **Ghi chú** vào `gridViewMedicine` và `gridViewMaterial` (grid bên trái) với `repositoryItemSpinMedicineExpAmount`/`repositoryItemSpinMaterialExpAmount` cho phép edit inline. |
| 2026-04-24 | Codex / Claude | PTTK 36619 (BV HAGL) — BR01/BR02: Thêm method `TryBatchAddFromLeftGrid()` trong `btnAdd_Click`. Khi user bấm Thêm, hệ thống scan `gridControlMedicine` + `gridControlMaterial` — nếu có dòng nào đã nhập `EXP_AMOUNT > 0` thì batch add tất cả sang `dicMediMateAdo` (grid bên phải) trong 1 lần, cảnh báo 1 lần cho các dòng vượt tồn kho + confirm thay thế nếu trùng SERVICE_ID, rồi reset `EXP_AMOUNT`/`NOTE` trên grid trái. Nếu không có dòng nào hợp lệ trên grid trái → fallback flow cũ (add 1 dòng qua `currentMediMate` + vùng nhập `spinExpAmount`/`txtNote`). |
| 2026-04-24 | dangth2 / Claude | PTTK 36619 (BV HAGL) — triển khai hoàn chỉnh: (1) Thêm property `AMOUNT_TRANSFER_MEDI`/`NOTE_TRANSFER_MEDI` vào `HisMedicineInStockADO`, `AMOUNT_TRANSFER_MATE`/`NOTE_TRANSFER_MATE` vào `HisMaterialInStockADO`. (2) Designer: 4 cột mới `gridColumnMedi_SLXuatChuyen`/`gridColumnMedi_NoteXuatChuyen`/`gridColumnMate_SLXuatChuyen`/`gridColumnMate_NoteXuatChuyen` gán `ColumnEdit = repositoryItemSpinTransferAmount` / `repositoryItemTextTransferNote` (AllowEdit=true). Gán repositoryItems vào `gridControlMedicine.RepositoryItems` + `gridControlMaterial.RepositoryItems`. (3) Đổi type `listMediInStock`/`listMateInStock` từ SDO→ADO, wrap data load qua `new HisMedicineInStockADO(sdo)`. (4) Thêm file partial `frmExpMestChmsUpdate__Plus__TransferSync.cs`: sync 2 chiều grid cell ↔ spinExpAmount/txtNote qua `CellValueChanged`, `FocusedRowChanged`, `EditValueChanged` (flag `isSyncingInputFromGrid`/`isSyncingGridFromInput` chặn loop). (5) `btnAdd_Click`: thêm nhánh batch `TryBatchAddFromGrid()` ở đầu — nếu có dòng AMOUNT_TRANSFER > 0 thì batch add, ngược lại fallback sang single-item mode cũ (giữ nguyên BR07). |

## 9. Test Cases (PTTK 36619)

### Sửa trực tiếp trên grid
- [ ] Mở phiếu xuất chuyển → grid bên phải có cột **Số lượng xuất chuyển** và **Ghi chú** ở cuối.
- [ ] Sửa số lượng tại cột EXP_AMOUNT → giá trị `spinExpAmount` phía dưới cập nhật theo.
- [ ] Sửa Ghi chú tại cột NOTE → `txtNote` phía dưới cập nhật theo.

### Sửa ở vùng nhập phía dưới
- [ ] Click chọn một dòng trên grid → `spinExpAmount` và `txtNote` hiển thị giá trị của dòng đó.
- [ ] Sửa `spinExpAmount` → cột `EXP_AMOUNT` của dòng đang chọn cập nhật theo + `ExpMedicine.Amount` / `ExpMaterial.Amount` / `ExpBlood.Amount` cũng cập nhật.
- [ ] Sửa `txtNote` → cột `NOTE` của dòng đang chọn cập nhật theo + Description trong SDO tương ứng.

### Lưu
- [ ] Sau khi sửa, bấm Lưu → payload `api/HisExpMest/ChmsUpdate` mang đúng EXP_AMOUNT và NOTE mới.

### Edge case
- [ ] Khi đổi focus sang dòng khác, thay đổi chưa lưu của dòng trước vẫn giữ trong `dicMediMateAdo`.
- [ ] Không có vòng lặp vô hạn khi sửa (flag `isSyncingDetailRow` hoạt động đúng).
