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

### Hoàn kho khi hủy (việc 3082)
Khi config `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport` = 1 (checkbox "In" bên MedicineSaleBill tự thực xuất phiếu khi Lưu ký):
- `ExpMestRestoreStockWorker.UnexportIfExported(owner, expMestCodes, reqRoomId)`: lấy phiếu theo code (`api/HisExpMest/GetView`), phiếu nào trạng thái ĐÃ THỰC XUẤT (`HIS_EXP_MEST_STT.ID__DONE`) thì confirm rồi gọi `api/HisExpMest/Unexport` hoàn số lượng vào kho.
- Điểm gọi: (1) callback sau khi TransactionCancel hủy giao dịch thành công (`RestoreStockAfterCancel`); (2) `DeleteExpMest` trước khi PharmacyCashierExpCancel; (3) dialog frmExpMest trước khi hủy từng phiếu.
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
| Hoàn xuất/hoàn kho (3082) | api/HisExpMest/Unexport | MosConsumer |

## 6. Dependencies

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.TransactionCancel | Hủy giao dịch có TRANSACTION_CODE | V_HIS_TRANSACTION + DelegateSelectData |

## 7. Print

In biên lai/hóa đơn qua MPS (Mps000111, Mps000339, ...) và in HĐĐT qua Library.ElectronicBill.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 05/08/2026 | nampp | Việc 3082: thêm `ExpMestRestoreStockWorker` — hoàn kho (api/HisExpMest/Unexport) cho phiếu ĐÃ THỰC XUẤT khi hủy giao dịch/phiếu, có confirm, chỉ chạy khi config `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport` = 1. Móc 3 luồng: callback TransactionCancel, DeleteExpMest, frmExpMest. Fix build máy backup: gỡ licenses.licx stale + đổi 3 ProjectReference không tồn tại (MPS, MPS.ProcessorBase, HIS.UC.Icd) sang Reference resolve qua ReferencePath; thay hmenu-lock.png hỏng (blob git lỗi CRLF); InBienLai: map HIS_PATIENT → V_HIS_PATIENT khớp Mps000111PDO bản mới. |
| (trước 2026) | team | Tạo plugin danh sách giao dịch xuất bán. |

## 9. Test Cases

- [ ] Config tắt: hủy giao dịch/phiếu như cũ, không có confirm hoàn kho.
- [ ] Config bật, hủy giao dịch có phiếu ĐÃ THỰC XUẤT: sau khi TransactionCancel thành công → confirm hoàn kho → tồn kho tăng lại đúng số lượng.
- [ ] Config bật, hủy phiếu xuất (chưa có bill) ĐÃ THỰC XUẤT: confirm hoàn kho trước, sau đó hủy phiếu như cũ.
- [ ] Phiếu CHƯA thực xuất: không hỏi hoàn kho, luồng hủy như cũ.
- [ ] Từ chối confirm hoàn kho: không gọi Unexport, luồng hủy tiếp tục (BE quyết định).
