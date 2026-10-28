# Báo cáo (ReportAll) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ReportAll |
| Loại | UC |
| Module ID | 259 |
| Mục đích | Màn hình Báo cáo tổng: chọn nhóm biểu báo cáo, chọn biểu báo cáo, tạo yêu cầu kết xuất, xem danh sách báo cáo đã kết xuất và thao tác trên từng báo cáo (công khai, sửa, xóa, **gửi**, tải về, in) |
| Trạng thái | Bảo trì |

Plugin là vỏ chứa: toàn bộ giao diện nghiệp vụ nằm ở các thư viện dùng chung, plugin chỉ khởi tạo và truyền tham số vào.

| Thư viện dùng chung | Vai trò |
|---------------------|---------|
| `Inventec.UC.ListReports` (TEMPLATE3) | Lưới danh sách báo cáo + nhóm nút thao tác + cửa sổ Gửi báo cáo |
| `Inventec.UC.ListReportType` | Danh sách biểu báo cáo |
| `Inventec.UC.ListReportTypeGroup` | Danh sách nhóm biểu báo cáo |
| `His.UC.CreateReport` | Tạo yêu cầu kết xuất báo cáo |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

1. Người dùng mở màn Báo cáo → chọn nhóm biểu báo cáo → chọn biểu báo cáo.
2. Khai báo tham số lọc → tạo yêu cầu kết xuất. Báo cáo được đưa vào hàng đợi xử lý của dịch vụ báo cáo.
3. Danh sách báo cáo hiển thị trạng thái: Chờ xử lý → Đang xử lý → Hoàn thành (hoặc Lỗi / Hủy).
4. Báo cáo Hoàn thành thì tải về hoặc in được; tệp lấy từ kho file dùng chung.
5. Người tạo báo cáo (hoặc tài khoản quản trị) có thể **gửi** báo cáo cho tài khoản khác, hoặc đặt báo cáo ở chế độ công khai.

### Điều kiện nghiệp vụ

- Nút Gửi, Sửa, Xóa, Công khai chỉ hoạt động với **người tạo báo cáo** hoặc **tài khoản quản trị**.
- Nút Tải về và In chỉ hoạt động khi báo cáo ở trạng thái **Hoàn thành**.
- Một báo cáo hiển thị với người dùng khi: người đó là người tạo, **hoặc** báo cáo công khai, **hoặc** tên đăng nhập của người đó nằm trong danh sách người nhận.
- Lưu danh sách người nhận là **ghi đè**, không cộng dồn với lần gửi trước.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| `SAR_REPORT` | Table | Bản ghi báo cáo đã tạo yêu cầu kết xuất; chứa danh sách người nhận, đường dẫn tệp, trạng thái |
| `V_SAR_REPORT` | View | Dữ liệu hiển thị trên lưới danh sách báo cáo |
| `SAR_REPORT_TYPE` | Table | Biểu báo cáo |
| `ACS_USER` | Table | Danh sách tài khoản để chọn người nhận khi gửi báo cáo (tên đăng nhập, họ tên, email, điện thoại) |

## 4. UI Layout

```
+--------------------------------------------------------------+
| [Nhóm biểu báo cáo] | [Biểu báo cáo] | [Tạo yêu cầu kết xuất] |
+--------------------------------------------------------------+
| Danh sách báo cáo đã kết xuất                                 |
| Mã | Tên báo cáo | Trạng thái | ... | [Công khai][Sửa][Xóa]   |
|                                      [Gửi][Tải về][In]        |
+--------------------------------------------------------------+
```

Cửa sổ **Gửi báo cáo** (thuộc thư viện danh sách báo cáo):

```
+------------------------------------------------------------------+
| Mã báo cáo: ...   Tên báo cáo: ...   Trạng thái: ...              |
| [Ô tìm kiếm]                                                      |
| Lưới tài khoản                    | Lưới tài khoản đã chọn        |
| [x] Tên đăng nhập|Họ tên|Email|ĐT |  Tên đăng nhập|Họ tên|Email|ĐT|
| (ghi chú khi bật gửi email)                            [Lưu]      |
+------------------------------------------------------------------+
```

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lấy danh sách báo cáo | `/api/SarReport/GetView` | SarConsumer |
| Gửi báo cáo cho tài khoản khác | `/api/SarReport/Send` | SarConsumer |
| Công khai / bỏ công khai | `/api/SarReport/Public` | SarConsumer |
| Sửa tên, mô tả báo cáo | `/api/SarReport/UpdateNameDescription` | SarConsumer |
| Khóa, xóa báo cáo | `/api/SarReport/Lock`, `/api/SarReport/Delete` | SarConsumer |
| Lấy trạng thái báo cáo, khổ giấy | `/api/SarReportStt/Get`, `/api/SarPaperSize/Get` | SarConsumer |
| Lấy tệp báo cáo | `/api/MrsReport/GetFile` | MrsConsumer |
| Lấy cấu hình hệ thống | `/api/SdaConfig/Get` | SdaConsumer |
| Lấy danh sách tài khoản (chọn người nhận) | `/api/AcsUser/Get` | AcsConsumer |

Tệp báo cáo tải về từ kho file dùng chung theo đường dẫn lưu trên bản ghi báo cáo.

## 6. Dependencies

### Thư viện dùng chung

| Thư viện | Mục đích |
|----------|---------|
| `Inventec.UC.ListReports` | Danh sách báo cáo, cửa sổ Gửi báo cáo |
| `Inventec.UC.ListReportType` / `Inventec.UC.ListReportTypeGroup` | Biểu báo cáo, nhóm biểu báo cáo |
| `His.UC.CreateReport` | Tạo yêu cầu kết xuất |

### Cấu hình ảnh hưởng

| KEY | Bảng | Tác dụng |
|-----|------|---------|
| `SAR.SendMail` | `web.config` của dịch vụ báo cáo | Bật/tắt gửi báo cáo qua email và khai tham số máy chủ thư. **Máy trạm không đọc khóa này** — chỉ hiển thị thông báo do dịch vụ trả về sau khi lưu |
| `HIS.Desktop.Plugins.ReportCreate.CustomFormType` | `HIS_CONFIG` | Tùy biến loại form tạo báo cáo |

## 7. Print

In báo cáo thực hiện trong thư viện danh sách báo cáo (nút In trên từng dòng), không qua MPS Processor của plugin.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 30/09/2026 | HUANNH | Tạo tài liệu module. Bổ sung đọc cấu hình `HIS.REPORT_SEND_MAIL.ENABLE` và truyền vào thư viện danh sách báo cáo để bật phần hiển thị liên quan tới gửi báo cáo qua email (vCong56270 — PTTK_56270) |
| 02/10/2026 | HUANNH | Gỡ bỏ phần đọc cấu hình nói trên theo chốt của viện — công tắc tính năng chuyển hẳn sang `web.config` của dịch vụ báo cáo. Cửa sổ Gửi báo cáo nay hiển thị đúng thông báo do dịch vụ trả về (trước đây luôn hiện "Xử lý thành công"), và giữ nguyên các dòng đã tick khi bấm sang cột khác (sửa lỗi dùng chung) |

## 9. Test Cases

### Cấu hình tắt (mặc định)

- [ ] `web.config` của dịch vụ báo cáo không khai `SAR.SendMail` hoặc để trường đầu bằng `0` → cửa sổ Gửi báo cáo hiển thị y như trước
- [ ] Gửi báo cáo → vẫn lưu người nhận, thông báo như cũ, không có thông báo nào về email

### Cấu hình bật

- [ ] Chọn tài khoản có Email → Lưu → thư tới hòm thư người nhận, kèm tệp báo cáo
- [ ] Chọn tài khoản chưa có Email → Lưu → thông báo nêu rõ tài khoản chưa nhận được email, báo cáo vẫn gửi trên phần mềm
- [ ] Tài khoản đang đăng nhập chưa khai Email → thông báo "Tài khoản của bạn chưa khai báo Email…", báo cáo vẫn gửi trên phần mềm
- [ ] Dịch vụ gửi email lỗi → báo cáo vẫn gửi trên phần mềm, có thông báo lỗi gửi email

### Giữ tick khi chọn người nhận

- [ ] Tick nhiều tài khoản rồi bấm sang cột Họ tên / Email / Điện thoại của dòng khác → các dòng đã tick còn nguyên, lưới bên phải vẫn đủ người nhận
- [ ] Bỏ tick vẫn bỏ được bình thường

### Không ảnh hưởng chức năng cũ

- [ ] Tạo yêu cầu kết xuất, tải về, in, công khai, sửa, xóa báo cáo hoạt động như trước
- [ ] Người không phải người tạo báo cáo vẫn không bấm được nút Gửi
