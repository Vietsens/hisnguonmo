# Danh Sách Xuất (Thuốc/Vật Tư) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisExportMestMedicine |
| Loại | UC (UCHisExportMestMedicine) |
| Mục đích | Màn "Danh sách xuất" của kho: tra cứu phiếu xuất (mọi loại — bán, lĩnh, chuyển kho, đơn phòng khám...) và thao tác trên từng phiếu: xem chi tiết, sửa, duyệt/bỏ duyệt, thực xuất/hủy thực xuất, tạo bill, hủy bill, in, tạo phiếu nhập trả. |
| Người tạo | (kế thừa codebase) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
Lọc phiếu (kho, khoảng ngày, trạng thái, loại xuất, BN…) → grid `V_HIS_EXP_MEST_2` → các nút icon trên dòng: duyệt (`Approve`), bỏ duyệt (`Unapprove`), thực xuất (`Export`), hủy thực xuất (`Unexport`), hoàn thành, tạo bill (mở MedicineSaleBill), **hủy bill** (mở TransactionCancel), xóa phiếu…

### Hoàn kho phiếu xuất bán khi hủy bill (việc 3082)
Khi config `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport` = 1:
- Nút **Hủy bill** (`Btn_CancelBill_Enable`) mở plugin TransactionCancel; callback sau khi hủy thành công gọi `ExpMestRestoreStockWorker.RestoreAfterCancelInvoice(codes, roomId)` với **mã phiếu lấy từ dòng lưới TRƯỚC khi hủy** — không dùng BILL_ID vì BE set `BILL_ID = null` ngay trong luồng hủy giao dịch.
- Worker: lấy phiếu theo mã phiếu, chỉ phiếu **loại XUẤT BÁN**; **HOÀN THÀNH** → `api/HisExpMest/Unexport` (hoàn kho); **ĐÃ DUYỆT** → `api/HisExpMest/Unapprove` → phiếu về **YÊU CẦU (vàng)**. Tự động, không confirm; chỉ báo khi API fail. KHÔNG xóa phiếu — viện tự xóa.
- Config tắt → luồng hủy bill giữ nguyên 100%.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_EXP_MEST_2 | View | Dòng grid phiếu xuất |
| V_HIS_EXP_MEST | View | Phiếu xuất bán gắn bill (3082) |
| HIS_EXP_MEST | Table | Kết quả các API duyệt/xuất/hoàn |

## 4. UI Layout

Bộ lọc bên trái + grid phiếu xuất với dải nút icon đầu dòng (xem, sửa, duyệt, bỏ duyệt, thực xuất, hủy thực xuất, tạo/hủy bill, in…) + panel thông tin chi tiết bên phải.

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Duyệt phiếu | api/HisExpMest/Approve | MosConsumer |
| Bỏ duyệt | api/HisExpMest/Unapprove | MosConsumer |
| Thực xuất | api/HisExpMest/Export | MosConsumer |
| Hủy thực xuất/hoàn kho | api/HisExpMest/Unexport | MosConsumer |
| Phiếu theo bill (3082) | api/HisExpMest/GetView (BILL_ID) | MosConsumer |

## 6. Dependencies

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.MedicineSaleBill | Nút tạo bill | expMestId + Module |
| HIS.Desktop.Plugins.TransactionCancel | Nút hủy bill | billId + row + DelegateSelectData |
| HIS.Desktop.Plugins.ExpMestViewDetail | Xem chi tiết phiếu | ExpMestViewDetailADO |

## 7. Print

In phiếu xuất, hướng dẫn sử dụng thuốc (Mps000099…) qua MPS.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 07/08/2026 | nampp | Việc 3082: nút **Hủy bill** — sau khi TransactionCancel hủy hóa đơn thành công, tự động hoàn kho (Unexport) + hủy duyệt (Unapprove) đưa phiếu xuất bán về trạng thái Yêu cầu (chỉ khi config `SaveSignPrintAutoExport` = 1). Thêm `ExpMestRestoreStockWorker`. Fix build máy backup: đổi ProjectReference `Library.ElectronicBill` sang Reference resolve qua ReferencePath. |
| (trước 2026) | team | Tạo plugin danh sách xuất thuốc/vật tư. |

## 9. Test Cases

- [ ] Config tắt: hủy bill như cũ, phiếu không đổi trạng thái.
- [ ] Config bật, hủy bill của phiếu xuất bán ĐÃ THỰC XUẤT: tồn kho tăng lại; phiếu về **Yêu cầu**; phiếu vẫn còn trong danh sách.
- [ ] Phiếu không phải loại xuất bán: worker bỏ qua, không đụng trạng thái.
- [ ] Đóng màn Hủy giao dịch mà không hủy: không gọi Unexport/Unapprove.
