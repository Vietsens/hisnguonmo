# Chi Tiết Phiếu Xuất (ExpMestViewDetail) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExpMestViewDetail |
| Loại | Form (frmExpMestViewDetail) |
| Mục đích | Xem chi tiết 1 phiếu xuất kho (thuốc/vật tư/máu) theo từng trạng thái: yêu cầu, duyệt, thực xuất. Được mở từ nút "Chi tiết" của các danh sách phiếu xuất (VD: HisExportChmsList — phiếu xuất chuyển kho). |
| Người tạo | Inventec |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

- Form hiển thị thông tin chung của phiếu xuất + các tab chi tiết theo nhóm hàng và trạng thái.
- Tab chính: **Yêu cầu thuốc**, **Yêu cầu vật tư**, **Duyệt thuốc**, **Duyệt vật tư**, **Máu**...
- Dữ liệu vật tư được gom nhóm qua `GroupExpMestMaterial()` theo loại vật tư (+ số lô, hạn dùng nếu là phiếu xuất KHÁC).
- Phân biệt "Yêu cầu" vs "Duyệt" bằng trạng thái phiếu (`EXP_MEST_STT_ID`): REQUEST → tab yêu cầu; DONE/EXECUTE → tab duyệt.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_EXP_MEST_MATERIAL_1 | View | Chi tiết vật tư của phiếu xuất (có `SERIAL_NUMBER`, `PACKAGE_NUMBER`...) |
| V_HIS_EXP_MEST_MEDICINE_1 | View | Chi tiết thuốc |
| HIS_EXP_MEST_MATY_REQ / METY_REQ | Table | Yêu cầu vật tư/thuốc |
| V_HIS_EXP_MEST_BLTY_REQ_1 | View | Yêu cầu máu |

## 4. UI Layout

- `xtraTabControl` với các tab thuốc/vật tư/máu, mỗi tab 1 GridControl.
- Tab **Yêu cầu vật tư**: `gridControlRequestMaterial` / `gridViewRequestMaterial`.
- Tab **Duyệt vật tư**: `gridControlApprovalMaterial` / `gridViewApprovalMaterial`.
- Grid vật tư có cột **Số lô** (`PACKAGE_NUMBER`) và **Số serial** (`SERIAL_NUMBER`, ngay bên phải Số lô).

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lấy vật tư (view) | api/HisExpMestMaterial/GetView1 | MosConsumer |
| Lấy yêu cầu vật tư | api/HisExpMestMatyReq/Get | MosConsumer |

## 6. Dependencies

- Được mở qua inter-plugin từ danh sách phiếu xuất (HisExportChmsList, v.v.) bằng `CallModule.ExpMestViewDetail`.

## 7. Print

- In phiếu xuất qua MPS (Mps000494...) trong `frmExpMestViewDetailPlus_Print.cs` (sử dụng `SERIAL_NUMBER` khi tái sử dụng vật tư).
- Phiếu xuất loại **KHÁC** có 2 mục in: **"Phiếu xuất khác"** (`MPS000165`, `InPhieuXuatKhac()`) và **"Phiếu xuất khác máu"** (`MPS000203`, `InPhieuXuatKhacMau()`).
- `InPhieuXuatKhac()` dựng `Mps000165PDO(_CurrentExpMest, _ExpMestMedicines_Print, _ExpMestMaterials_Print, _ExpMestBloods_Print)` — **có truyền danh sách máu** (từ 26/08/2026) nên phiếu xuất khác tại kho máu in ra dòng máu trên cùng bảng `ListMediMate` với thuốc/vật tư. `_ExpMestBloods_Print` được nạp sẵn ở `LoadExpMestBltyReq()` (thread khi mở form, API `HIS_EXP_MEST_BLOOD_GET_VIEW`) — không phát sinh API mới khi in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 01/07/2026 | huannh | YC3: Bổ sung cột "Số serial" (`SERIAL_NUMBER`) ngay bên phải cột "Số lô" ở tab **Yêu cầu vật tư** (`gridColumnRequestMaterialSerial`) và tab **Duyệt vật tư** (`gridColumnApprovalMaterialSerial`). Dữ liệu lấy từ `V_HIS_EXP_MEST_MATERIAL_1.SERIAL_NUMBER` (API `GetView1`). Bổ sung `SERIAL_NUMBER` vào điều kiện GroupBy trong `GroupExpMestMaterial()` (cả 2 nhánh KHÁC và thường) → mỗi dòng ứng đúng 1 serial, không gộp sai thông tin serial. Vị trí cột ép runtime `VisibleIndex = <Số lô>.VisibleIndex + 1` trong `SetCaptionByLanguageKey`. Caption đa ngôn ngữ vi/en. |
| 26/08/2026 | nampp | Việc 44751 (BV Nguyễn Thị Thập): `InPhieuXuatKhac()` truyền thêm `_ExpMestBloods_Print` vào constructor mới 4 tham số của `Mps000165PDO` → "Phiếu xuất khác" (MPS000165) tại kho máu in được dòng máu (trước đây bảng trống vì PDO chỉ nhận thuốc/vật tư). 1 dòng sửa; **phải deploy kèm `MPS.Processor.Mps000165.PDO.dll` mới**. Xem `docs/MPS.Processor.Mps000165.md`. |

## 9. Test Cases

- [ ] Mở chi tiết 1 phiếu xuất chuyển kho có vật tư gắn serial → tab Yêu cầu vật tư & Duyệt vật tư hiển thị cột "Số serial" ngay sau "Số lô".
- [ ] Vật tư nhiều serial cùng loại → cột serial hiển thị danh sách serial cách nhau dấu phẩy, không trùng.
- [ ] Vật tư không có serial → ô serial trống, không lỗi.
- [ ] Phiếu xuất KHÁC tại kho lẻ máu → In ấn → "Phiếu xuất khác" (MPS000165): bảng có dòng máu (tên loại máu, ĐVT, số lô, hạn, SL = số đơn vị, đơn giá), tổng tiền / bằng chữ đúng; giống phiếu in từ màn Xuất khác.
- [ ] Phiếu xuất KHÁC tại kho thuốc → "Phiếu xuất khác" in y hệt trước (hồi quy).
