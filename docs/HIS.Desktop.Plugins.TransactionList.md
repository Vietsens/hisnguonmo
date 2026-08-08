# Danh Sách Giao Dịch — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TransactionList |
| Loại | Form (frmTransactionList) |
| Mục đích | Danh sách giao dịch thu/chi (thanh toán, tạm ứng, hoàn ứng...) theo hồ sơ điều trị hoặc theo khoảng thời gian: xem, in biên lai/hóa đơn, hủy giao dịch, khôi phục, đổi hình thức thanh toán. |
| Người tạo | (kế thừa codebase) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
Lọc theo mã điều trị/khoảng ngày/sổ/loại giao dịch → grid giao dịch (`V_HIS_TRANSACTION`) → thao tác trên từng dòng: in biên lai, in hóa đơn điện tử, hủy giao dịch, khôi phục, sửa thông tin.

### Hủy giao dịch
`repositoryItemBtnCancelTran_ButtonClick` → mở plugin **TransactionCancel** (hủy bill + HĐĐT) → reload grid.

### Hoàn kho phiếu xuất bán khi hủy hóa đơn (việc 3082)
Khi config `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport` = 1 (nhà thuốc bật tự động thực xuất khi Lưu ký):
- **TRƯỚC khi mở màn Hủy giao dịch**: `GetSaleExpMestCodesByBillId(data)` lấy mã các phiếu xuất bán theo `BILL_ID`. Bắt buộc lấy trước vì BE (`HisTransactionCancel.ProcessSaleExpMest`) **set `BILL_ID = null`** ngay trong luồng hủy → sau khi hủy không tìm lại được phiếu theo bill.
- Sau khi màn Hủy giao dịch đóng, `RestoreExpMestAfterCancelInvoice(data, codes)` kiểm tra lại trên server `IS_CANCEL = 1` rồi gọi `ExpMestRestoreStockWorker.RestoreAfterCancelInvoice(codes, reqRoomId)`.
- Worker: lấy phiếu theo **mã phiếu**, chỉ phiếu **loại XUẤT BÁN**; phiếu **HOÀN THÀNH** → `api/HisExpMest/Unexport` (hoàn số lượng vào kho); phiếu **ĐÃ DUYỆT** → `api/HisExpMest/Unapprove` → phiếu về trạng thái **YÊU CẦU (vàng)**. Tự động, không confirm; chỉ hiện message khi API fail. KHÔNG xóa phiếu.
- Lưu ý nghiệp vụ: nếu config BE `EXPORT_SALE_MUST_BILL` = 1 thì BE **chặn hủy giao dịch** khi phiếu còn HOÀN THÀNH (*"Phiếu xuất bán chưa hủy thực xuất"*), trong khi `Unexport` lại bị chặn khi phiếu còn bill → FE không xử lý được, cần BE hỗ trợ (đề xuất BE tự Unexport/Unapprove trong `HisTransactionCancel`).
- ReqRoomId lấy theo phòng của `CASHIER_ROOM_ID` giao dịch, fallback phòng làm việc hiện tại.
- Config tắt → không làm gì, luồng hủy giữ nguyên 100%.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TRANSACTION | View | Dòng grid giao dịch |
| V_HIS_EXP_MEST | View | Phiếu xuất bán gắn với bill (3082) |
| V_HIS_CASHIER_ROOM | View | Suy ra ROOM_ID cho ReqRoomId (3082) |

## 4. UI Layout

Bộ lọc (mã điều trị, khoảng ngày, sổ, loại giao dịch, phòng thu) + grid giao dịch với các nút icon trên dòng: in, hủy giao dịch, khôi phục, sửa thông tin, đổi hình thức thanh toán.

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Danh sách giao dịch | api/HisTransaction/GetView | MosConsumer |
| Phiếu xuất theo bill (3082) | api/HisExpMest/GetView (BILL_ID) | MosConsumer |
| Hủy thực xuất/hoàn kho (3082) | api/HisExpMest/Unexport | MosConsumer |
| Hủy duyệt → về Yêu cầu (3082) | api/HisExpMest/Unapprove | MosConsumer |

## 6. Dependencies

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.TransactionCancel | Hủy giao dịch | V_HIS_TRANSACTION + Module |

## 7. Print

In biên lai/hóa đơn qua MPS + Library.ElectronicBill.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 07/08/2026 | nampp | Việc 3082: thêm `ExpMestRestoreStockWorker` + `RestoreExpMestAfterCancelInvoice` — sau khi hủy giao dịch (hóa đơn nhà thuốc) thành công, tự động hoàn kho (Unexport) và hủy duyệt (Unapprove) để phiếu xuất bán về trạng thái Yêu cầu. Chỉ chạy khi config `SaveSignPrintAutoExport` = 1 và giao dịch đã thực sự bị hủy (`IS_CANCEL = 1`). |
| (trước 2026) | team | Tạo plugin danh sách giao dịch. |

## 9. Test Cases

- [ ] Config tắt: hủy giao dịch như cũ, phiếu xuất không đổi trạng thái.
- [ ] Config bật, hủy hóa đơn của phiếu xuất bán ĐÃ THỰC XUẤT: tồn kho tăng lại đúng SL; phiếu về **Yêu cầu**; phiếu vẫn còn.
- [ ] Hủy giao dịch KHÔNG phải hóa đơn nhà thuốc (viện phí thường): không đụng gì tới phiếu xuất.
- [ ] Đóng màn Hủy giao dịch mà không hủy: không gọi Unexport/Unapprove.
