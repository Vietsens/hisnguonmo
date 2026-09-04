# Xuất khác (ExpMestOtherExport) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExpMestOtherExport |
| Loại | UC (`UCExpMestOtherExport : UserControlBase`) + form phụ `FromExpMestOtherExport` |
| Mục đích | Tạo / cập nhật / duyệt phiếu **xuất khác** (`HIS_EXP_MEST_TYPE.ID__KHAC`) cho thuốc, vật tư và máu tại kho; in "Phiếu xuất khác" (MPS000165) và "Phiếu xuất khác máu" (MPS000203). |
| Người tạo | Inventec |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

- Chọn kho xuất → chọn hàng (thuốc / vật tư / máu — `dicTypeAdo` theo `TYPE_BLOOD = 3`) → nhập số lượng, lý do xuất → Lưu (`api/HisExpMest/OtherCreate` / `OtherUpdate`) → kết quả `HisExpMestResultSDO resultSdo` (ExpMest, ExpMedicines, ExpMaterials, ExpBloods).
- Nút **In** là dropdown 2 mục (`GenerateMenuPrint`): "In phiếu xuất khác" → MPS000165; "In phiếu xuất khác máu" → MPS000203.
- Phím tắt: `KeyboardWorker.cs`.

### Điều kiện nghiệp vụ
- Chỉ in được sau khi đã lưu (`resultSdo != null`).
- Máu trong phiếu: mỗi bản ghi `V_HIS_EXP_MEST_BLOOD` là 1 đơn vị máu.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_EXP_MEST | View | Phiếu xuất (lấy lại sau lưu để in) |
| V_HIS_EXP_MEST_MEDICINE / V_HIS_EXP_MEST_MATERIAL | View | Dòng thuốc / vật tư để in |
| V_HIS_EXP_MEST_BLOOD | View | Dòng máu để in (MPS000165 + MPS000203) |
| HIS_BLOOD_GIVER, HIS_EXP_MEST_REASON, V_HIS_MEDI_STOCK | Cache `BackendDataWorker` | Người hiến, lý do xuất, kho nhập (MPS000203) |

## 4. UI Layout

- UC chính `UCExpMestOtherExport` (partial: `___Load`, `_Plus_Button`, `_Plus_Control`, `_Plus_TreeGrid`, `_Update`), lưới hàng chọn theo loại, nút In dropdown (`ddBtnPrint`).

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Tạo / sửa phiếu xuất khác | api/HisExpMest/OtherCreate, api/HisExpMest/OtherUpdate | MosConsumer |
| Lấy phiếu | api/HisExpMest/Get, api/HisExpMest/GetView | MosConsumer |
| Dòng thuốc / vật tư / máu (in) | api/HisExpMestMedicine/GetView, api/HisExpMestMaterial/GetView, api/HisExpMestBlood/GetView | MosConsumer |
| Danh mục máu, lý do xuất | api/HisBlood/GetView, api/HisExpMestReason/Get | MosConsumer |

## 6. Dependencies

| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.EmrGenerate | `GenerateInputADOWithPrintTypeCode` cho ký EMR khi in |
| MPS.Processor.Mps000165.PDO, MPS.Processor.Mps000203.PDO | PDO 2 biểu in |

## 7. Print

| Loại in | PrintTypeCode | Hàm | Dữ liệu truyền |
|---------|--------------|-----|----------------|
| Phiếu xuất khác | `PrintTypeCodeStore.PRINT_TYPE_CODE__BIEU_MAU_PHIEU_XUAT_KHAC__MPS000165` | `InPhieuXuatKhac()` | `Mps000165PDO(expMest, medicines, materials, bloods)` — máu lấy qua `GetExpMestBloodsForPrint(param)` (null nếu phiếu không có máu) |
| Phiếu xuất khác máu | `PrintTypeWorker.PRINT_TYPE_CODE__BIEU_MAU_PHIEU_XUAT_KHAC_MAU__MPS000203` | `InPhieuXuatKhacMau()` | `Mps000203PDO(expMest, bloods, Mps000203ADO, listBloodGiver)` — máu cũng qua `GetExpMestBloodsForPrint(param)` |

`GetExpMestBloodsForPrint(CommonParam)`: lọc `IDs = resultSdo.ExpBloods` → `api/HisExpMestBlood/GetView` → gán `DESCRIPTION` từ ghi chú nhập trên lưới (`dicTypeAdo[TYPE_BLOOD]`); có null-check, trả null khi phiếu không có máu.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 26/08/2026 | nampp | Việc 44751 (BV Nguyễn Thị Thập): "Phiếu xuất khác" (MPS000165) tại kho máu in bảng trống. Tách hàm dùng chung `GetExpMestBloodsForPrint()` từ `InPhieuXuatKhacMau()`; `InPhieuXuatKhac()` gọi hàm này và truyền list máu vào ctor mới 4 tham số của `Mps000165PDO`. `InPhieuXuatKhacMau()` dùng lại hàm tách (thêm null-check, hành vi không đổi). **Phải deploy kèm `MPS.Processor.Mps000165.PDO.dll` mới.** Xem `docs/MPS.Processor.Mps000165.md`. |

## 9. Test Cases

- [ ] Kho lẻ máu → Xuất khác 1 đơn vị máu → Lưu → In "Phiếu xuất khác" → bảng có dòng máu, tổng tiền / bằng chữ đúng.
- [ ] Cùng phiếu → In "Phiếu xuất khác máu" (MPS000203) → không đổi so với trước.
- [ ] Kho thuốc → Xuất khác thuốc/vật tư → In "Phiếu xuất khác" → y hệt trước (hồi quy).
- [ ] Phiếu có cả thuốc + máu → 2 nhóm dòng liên tiếp, tổng = tổng 2 nhóm.
