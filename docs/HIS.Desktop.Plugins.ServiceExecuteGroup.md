# Trả kết quả tổng hợp (ServiceExecuteGroup) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ServiceExecuteGroup |
| Loại | Form (`Run/frmServiceExecuteGroup` kế thừa `FormBase`) |
| Mục đích | Trả kết quả / kết thúc **nhiều y lệnh cùng lúc** từ màn Xử lý y lệnh (ExecuteRoom): chọn nhiều dòng → chuột phải "Trả kết quả tổng hợp" → nhập mô tả/kết luận chung → Lưu (tạo `HIS_SERE_SERV_EXT` cho từng dịch vụ) và/hoặc Kết thúc (gọi `api/HisServiceReq/Finish` từng y lệnh). |
| Người tạo | IVT |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. ExecuteRoom gom các dòng đang chọn (`UCExecuteRoom.cs:1921-1932`) → mở plugin với `List<L_HIS_SERVICE_REQ>` (`UCExecuteRoom___Popup_Menu_Showing.cs:1070`).
2. **Lưu (Ctrl S)** — `BackgroundWorker` duyệt từng y lệnh, từng dịch vụ: POST `api/HisSereServExt/CreateSdo` (`HisSereServExtWithFileSDO`) với nội dung chung; lỗi từng dòng gom vào lưới `lstResultError` (`ResultADO`: mã y lệnh, tên dịch vụ, mô tả lỗi).
3. **Kết thúc (Ctrl E)** — nút `btnCancel` (tên control lịch sử, caption "Kết thúc (Ctrl E)") → `currentChoose = Choose.CANCEL` → `backgroundWorker1` chạy `SetDataToCancel()`: bỏ qua y lệnh đã hoàn thành (`SERVICE_REQ_STT_ID = ID__HT`), POST `api/HisServiceReq/Finish` cho từng y lệnh còn lại, cập nhật `SERVICE_REQ_STT_ID` và gọi `delegateRefresh` về ExecuteRoom.
4. Thanh tiến trình `pbProcess` + `lblProcess` "Đã xử lý: n/N"; in phiếu tại `frmServiceExecuteGroup_Print.cs`.

### Điều kiện nghiệp vụ
- Kiểm tra hạn dùng qua `Validation/ExpiredDateValidationRule`.
- **Việc 3353 (18/09/2026):** trước khi chạy worker Kết thúc, gọi `CheckRequireMediMateManager.CheckBeforeFinishByServiceReqIds(ID các y lệnh chưa hoàn thành)`: dịch vụ có `HIS_SERVICE.IS_REQUIRE_MEDI_MATE = 1` mà chưa có thuốc/vật tư đi kèm → **một** hộp thoại Yes/No liệt kê đủ dịch vụ của tất cả y lệnh; No → không kết thúc y lệnh nào. Hỏi ở UI thread, trước `RunWorkerAsync()`.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| L_HIS_SERVICE_REQ | View | Danh sách y lệnh đầu vào (`lstServiceReqSend`) |
| V_HIS_SERVICE_REQ | View | Kết quả trả về sau Finish |
| HIS_SERE_SERV / HIS_SERE_SERV_EXT | Table | Dịch vụ của y lệnh + kết quả (`HisSereServExtWithFileSDO`) |

## 4. UI Layout

```
+----------------------------------------------------------+
| Mô tả / Kết luận / Ghi chú dùng chung cho các y lệnh     |
+----------------------------------------------------------+
| Lưới lỗi: Mã y lệnh | Tên dịch vụ | Mô tả lỗi             |
+----------------------------------------------------------+
| [Lưu (Ctrl S)] [Kết thúc (Ctrl E)] [In]   Đã xử lý: n/N  |
+----------------------------------------------------------+
```

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lưu kết quả từng dịch vụ | api/HisSereServExt/CreateSdo | MosConsumer |
| Kết thúc y lệnh | `HisRequestUriStore.HIS_SERVICE_REQ_FINISH` (api/HisServiceReq/Finish) | MosConsumer |
| (thư viện 3353) cờ dịch vụ / dòng thuốc-VT con | api/HisService/Get, api/HisSereServ/Get | MosConsumer |

## 6. Dependencies

| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.CheckRequireMediMate | Việc 3353 — cảnh báo dịch vụ chưa có thuốc, vật tư đi kèm trước khi kết thúc |

Inter-plugin: được mở từ `HIS.Desktop.Plugins.ExecuteRoom` (menu chuột phải khi chọn nhiều y lệnh), trả `RefeshReference` để ExecuteRoom tải lại lưới.

## 7. Print

Xem `Run/frmServiceExecuteGroup_Print.cs`.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 18/09/2026 | dangth2 | Tạo tài liệu module. **Việc 3353 (PT-56272):** trong `btnCancel_Click` (nút "Kết thúc (Ctrl E)", `Run/frmServiceExecuteGroup.cs`) thêm lời gọi `CheckRequireMediMateManager.CheckBeforeFinishByServiceReqIds(...)` cho các y lệnh chưa hoàn thành, đặt trước `backgroundWorker1.RunWorkerAsync()`; No → `return`, không chạy worker. Thêm reference `HIS.Desktop.Plugins.Library.CheckRequireMediMate`. Ghi chú build trên máy backup: csproj có `Properties\licenses.licx` (file gitignore, không tồn tại) → lc.exe quá dài (MSB6003); build qua bản csproj tạm bỏ dòng này, không sửa csproj gốc. |

## 9. Test Cases

- [ ] Chọn 2 y lệnh, Kết thúc → cả 2 hoàn thành, ExecuteRoom tải lại.
- [ ] Y lệnh đã hoàn thành trong danh sách → bỏ qua, không lỗi.
- [ ] (3353) 2 y lệnh có dịch vụ bật cờ chưa kê thuốc/VT → 1 hộp thoại liệt kê đủ; No → không y lệnh nào bị kết thúc; Yes → kết thúc hết.
- [ ] Lỗi Finish 1 y lệnh → hiện trong lưới lỗi, các y lệnh khác vẫn xử lý.
