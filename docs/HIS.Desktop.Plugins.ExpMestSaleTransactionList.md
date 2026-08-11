# Danh Sách Giao Dịch Xuất Bán — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExpMestSaleTransactionList |
| Loại | Form (frmExpMestSaleTransactionList) |
| Mục đích | Danh sách giao dịch/phiếu xuất bán nhà thuốc: xem, in biên lai/hóa đơn, hủy giao dịch (qua plugin TransactionCancel), hủy phiếu xuất (PharmacyCashierExpCancel). |
| Người tạo | (kế thừa codebase) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng hủy
- Dòng có TRANSACTION_CODE → `HuyBienLaiHoaDon`: mở plugin TransactionCancel (hủy bill + HĐĐT), callback reload grid.
- Dòng chỉ có EXP_MEST_CODE → `DeleteExpMest`: 1 phiếu gọi thẳng `api/HisExpMest/PharmacyCashierExpCancel`; nhiều phiếu mở dialog frmExpMest hủy từng phiếu.

### Hủy hóa đơn → hoàn kho + trả phiếu về Yêu cầu (việc 3082)
Khi config `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport` = 1 (checkbox "In" bên MedicineSaleBill tự thực xuất phiếu khi Lưu ký):
- `ExpMestRestoreStockWorker.RestoreAfterCancelInvoice(owner, expMestCodes, reqRoomId)`: lấy phiếu theo code (`api/HisExpMest/GetView`); phiếu **HOÀN THÀNH** (`ID__DONE`) → `api/HisExpMest/Unexport` (hoàn số lượng vào kho); phiếu **ĐÃ DUYỆT** (`ID__EXECUTE`, gồm cả phiếu vừa Unexport xong) → `api/HisExpMest/Unapprove` → phiếu về trạng thái **YÊU CẦU (vàng)**. Chạy **tự động, không confirm**; chỉ hiện message khi API fail. KHÔNG xóa phiếu — viện tự xóa bằng tay sau.
- Điểm gọi: **duy nhất** trong callback sau khi TransactionCancel hủy giao dịch/HĐĐT thành công (`RestoreStockAfterCancel`).
- Nhánh **Hủy phiếu xuất** (`DeleteExpMest`, dialog `frmExpMest`) giữ nguyên code cũ: BE `PharmacyCashierExpCancel` đã tự chạy chuỗi Unexport → Unapprove → Truncate (xóa phiếu).
- Ràng buộc BE: `Unexport` bị chặn nếu phiếu xuất bán còn bill chưa hủy (message *"Phiếu xuất bán đã thanh toán, hủy giao dịch trước"*) → bắt buộc hủy giao dịch trước, hoàn kho sau (FE gọi đúng thứ tự này).
- Config tắt → không làm gì, luồng hủy giữ nguyên 100%.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| DHisTransExpSDO | SDO | Dòng grid giao dịch + phiếu xuất |
| V_HIS_TRANSACTION | View | Giao dịch để hủy/in |
| V_HIS_EXP_MEST | View | Trạng thái phiếu xuất (check ĐÃ THỰC XUẤT — 3082) |
| V_HIS_CASHIER_ROOM | View | Suy ra ROOM_ID làm ReqRoomId/WorkingRoomId |

## 4. UI Layout

Bộ lọc (khoảng ngày, mã điều trị, mã phiếu, sổ, loại giao dịch, phòng thu) + grid giao dịch (nút Xóa/Hủy từng dòng, popup menu chuột phải: in biên lai, hủy biên lai, hủy phiếu xuất).

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Danh sách giao dịch+phiếu | api/HisTransaction/GetTransExp | MosConsumer |
| Hủy phiếu xuất bán | api/HisExpMest/PharmacyCashierExpCancel | MosConsumer |
| Phiếu theo code (3082) | api/HisExpMest/GetView (EXP_MEST_CODEs) | MosConsumer |
| Hủy thực xuất/hoàn kho (3082) | api/HisExpMest/Unexport | MosConsumer |
| Hủy duyệt → về Yêu cầu (3082) | api/HisExpMest/Unapprove | MosConsumer |

## 6. Dependencies

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.TransactionCancel | Hủy giao dịch có TRANSACTION_CODE | V_HIS_TRANSACTION + DelegateSelectData |

## 7. Print

In biên lai/hóa đơn qua MPS (Mps000111, Mps000339, ...) và in HĐĐT qua Library.ElectronicBill.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 07/08/2026 | nampp | Việc 3082 (theo tài liệu phân tích cập nhật 07/08): đổi `UnexportIfExported` → `RestoreAfterCancelInvoice` — sau khi hủy hóa đơn, TỰ ĐỘNG (bỏ confirm) gọi Unexport (hoàn kho) rồi Unapprove để phiếu về trạng thái **Yêu cầu (vàng)**, không xóa phiếu. Gỡ hook khỏi 2 nhánh hủy phiếu xuất (`DeleteExpMest`, `frmExpMest`) vì BE `PharmacyCashierExpCancel` đã tự chạy Unexport → Unapprove → Truncate. |
| 05/08/2026 | nampp | Việc 3082: thêm `ExpMestRestoreStockWorker` — hoàn kho (api/HisExpMest/Unexport) cho phiếu ĐÃ THỰC XUẤT khi hủy giao dịch/phiếu, có confirm, chỉ chạy khi config `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport` = 1. Móc 3 luồng: callback TransactionCancel, DeleteExpMest, frmExpMest. Fix build máy backup: gỡ licenses.licx stale + đổi 3 ProjectReference không tồn tại (MPS, MPS.ProcessorBase, HIS.UC.Icd) sang Reference resolve qua ReferencePath; thay hmenu-lock.png hỏng (blob git lỗi CRLF); InBienLai: map HIS_PATIENT → V_HIS_PATIENT khớp Mps000111PDO bản mới. |
| (trước 2026) | team | Tạo plugin danh sách giao dịch xuất bán. |

## 9. Test Cases

- [ ] Config tắt: hủy giao dịch/phiếu như cũ, không đụng trạng thái phiếu.
- [ ] Config bật, hủy hóa đơn của phiếu ĐÃ THỰC XUẤT: tồn kho tăng lại đúng số lượng; phiếu về trạng thái **Yêu cầu**; phiếu vẫn còn trong danh sách.
- [ ] Sau đó "Hủy phiếu xuất": phiếu bị xóa; kho không đổi thêm.
- [ ] Phiếu CHƯA thực xuất (chỉ ở Đã duyệt): hủy hóa đơn → chỉ Unapprove về Yêu cầu, kho không đổi.
- [ ] Hủy phiếu xuất trực tiếp (không qua hủy hóa đơn) với phiếu đã thực xuất: BE tự hoàn kho + xóa phiếu.
