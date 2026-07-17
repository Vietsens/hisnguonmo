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

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 01/07/2026 | huannh | YC3: Bổ sung cột "Số serial" (`SERIAL_NUMBER`) ngay bên phải cột "Số lô" ở tab **Yêu cầu vật tư** (`gridColumnRequestMaterialSerial`) và tab **Duyệt vật tư** (`gridColumnApprovalMaterialSerial`). Dữ liệu lấy từ `V_HIS_EXP_MEST_MATERIAL_1.SERIAL_NUMBER` (API `GetView1`). Bổ sung `SERIAL_NUMBER` vào điều kiện GroupBy trong `GroupExpMestMaterial()` (cả 2 nhánh KHÁC và thường) → mỗi dòng ứng đúng 1 serial, không gộp sai thông tin serial. Vị trí cột ép runtime `VisibleIndex = <Số lô>.VisibleIndex + 1` trong `SetCaptionByLanguageKey`. Caption đa ngôn ngữ vi/en. |

## 9. Test Cases

- [ ] Mở chi tiết 1 phiếu xuất chuyển kho có vật tư gắn serial → tab Yêu cầu vật tư & Duyệt vật tư hiển thị cột "Số serial" ngay sau "Số lô".
- [ ] Vật tư nhiều serial cùng loại → cột serial hiển thị danh sách serial cách nhau dấu phẩy, không trùng.
- [ ] Vật tư không có serial → ô serial trống, không lỗi.
