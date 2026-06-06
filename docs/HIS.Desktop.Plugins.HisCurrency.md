# Danh mục ngoại tệ — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisCurrency |
| Loại | Form |
| Mục đích | Màn hình quản trị danh mục ngoại tệ và tỉ giá quy đổi sang VND. Cho phép thêm mới, sửa, khóa/mở khóa (bật/tắt hoạt động), xóa mềm và cập nhật tỉ giá theo thời điểm. |
| Người tạo | phuongnm |
| Ngày tạo | 03/06/2026 |
| Trạng thái | Đang phát triển |
| Liên quan | PTTK_44193 — Chỉnh sửa hình thức thanh toán (phần B mục 4.1.2) |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
- Màn hình dạng **danh sách (trái) + form thêm/sửa (phải)** theo pattern danh mục chuẩn (giống HIS.Desktop.Plugins.HisBank).
- Người dùng tìm theo mã/tên ngoại tệ → chọn dòng trên lưới → dữ liệu đổ vào form bên phải.
- **Thêm mới**: nhập Mã, Tên, Tỉ giá (VND), Thời điểm cập nhật tỉ giá → bấm **Thêm (Ctrl N)**.
- **Sửa**: chọn dòng → sửa → bấm **Sửa (Ctrl S)** (nút Sửa bị vô hiệu nếu bản ghi đang khóa).
- **Khóa/Mở khóa**: icon ổ khóa trên lưới gọi `ChangeLock` → đảo `IS_ACTIVE` (KHÔNG đụng STATUS_CODE).
- **Xóa**: icon X trên lưới → xóa mềm (`IS_DELETE = 1`).
- **Làm lại (Ctrl R)**: reset form về trạng thái thêm mới.

### Điều kiện nghiệp vụ
- Mã ngoại tệ: bắt buộc, tối đa 10 ký tự (chuẩn ISO 4217, thường 3 ký tự); không trùng (backend kiểm tra).
- Tên ngoại tệ: bắt buộc, tối đa 100 ký tự.
- Tỉ giá (VND): bắt buộc, phải > 0.
- Thời điểm cập nhật tỉ giá: tùy chọn, lưu dạng `long yyyyMMddHHmmss`.

### Sơ đồ trạng thái
```
Hoạt động (IS_ACTIVE=1)  <-- ChangeLock -->  Đã khóa (IS_ACTIVE=0)
        |
       Xóa (IS_DELETE=1)
```

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_CURRENCY | Table | Danh mục ngoại tệ + tỉ giá |

### Cột nghiệp vụ HIS_CURRENCY
| Cột | Kiểu | Ghi chú |
|-----|------|---------|
| CURRENCY_CODE | string | Mã ngoại tệ (USD, EUR...) |
| CURRENCY_NAME | string | Tên ngoại tệ |
| EXCHANGE_RATE | decimal? | Tỉ giá quy đổi sang VND |
| EXCHANGE_RATE_TIME | long? | Thời điểm cập nhật tỉ giá (yyyyMMddHHmmss) |
| IS_ACTIVE / IS_DELETE | short? | Khóa / xóa mềm |
| CREATE_TIME / CREATOR / MODIFY_TIME / MODIFIER | audit | 4 cột audit cuối lưới |

## 4. UI Layout

### Sơ đồ giao diện
```
+--------------------------------------------------------------+------------------------+
| [Nhập mã hoặc tên ngoại tệ để tìm...] [Tìm (Ctrl F)] [Tải lại]| Mã ngoại tệ: *         |
+--------------------------------------------------------------+ [__________]           |
| STT |🔓|❌| Mã | Tên | Tỉ giá (VND) | Thời điểm cập nhật | TT | (Mã chuẩn ISO 4217...)  |
|  1  |  |  |USD | ... |     25.000   | 18/05/2026 08:00   |HĐ  | Tên ngoại tệ: *        |
|  2  |  |  |EUR | ... |     27.300   | 18/05/2026 08:00   |HĐ  | [__________]           |
| ...                                                          | Tỉ giá (VND): *        |
|                                                              | [__________]           |
|                                                              | (Phải lớn hơn 0...)    |
|                                                              | Thời điểm cập nhật:    |
|                                                              | [__/__/____ __:__:__]  |
|                                                              | Thời gian sửa cuối: .. |
| [Trang 1/1 — 1-14/14]  [Mỗi trang: 50]                       | [Sửa][Thêm][Làm lại]   |
+--------------------------------------------------------------+------------------------+
```
Cột lưới: STT | (khóa) | (xóa) | Mã ngoại tệ | Tên ngoại tệ | Tỉ giá (VND) | Thời điểm cập nhật | Trạng thái | Thời gian tạo | Người tạo | Thời gian sửa | Người sửa.
Dòng đã khóa hiển thị màu xám (RowStyle).

### UC sử dụng
| UC | Mục đích |
|----|----------|
| Inventec.UC.Paging.UcPaging | Phân trang server-side |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy danh sách | /api/HisCurrency/Get | MosConsumer | HisCurrencyFilter (KEY_WORD, ID, IS_ACTIVE) |
| Tạo mới | /api/HisCurrency/Create | MosConsumer | — |
| Cập nhật | /api/HisCurrency/Update | MosConsumer | — |
| Xóa (mềm) | /api/HisCurrency/Delete | MosConsumer | — |
| Khóa/Mở khóa | /api/HisCurrency/ChangeLock | MosConsumer | — |

## 6. Dependencies

### Library Plugins
Không.

### Inter-Plugin
Không (màn hình quản trị độc lập). Danh mục này được tiêu thụ bởi UC `HIS.UC.TransactionPayformGrid` (mục 4.1.1) qua API `GET /api/HisCurrency/Get`.

## 7. Print
Không.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 03/06/2026 | phuongnm | Tạo mới plugin Danh mục ngoại tệ (PTTK_44193 mục 4.1.2): danh sách + form thêm/sửa, khóa/mở khóa, xóa mềm, cập nhật tỉ giá. |
| 05/06/2026 | phuongnm | Sửa: (1) chặn trùng mã ngoại tệ khi Thêm/Sửa (IsDuplicateCode gọi Get theo CURRENCY_CODE); (2) tìm kiếm lọc client-side theo mã/tên + Enter để tìm; (3) tô màu cột Trạng thái (Hoạt động=xanh, Đã khóa=đỏ) qua RowCellStyle; (4) ô Thời điểm cập nhật tỉ giá cho chọn cả giờ (CalendarTimeEditing=True). |

## 9. Test Cases

### Tạo mới
- [ ] Nhập đủ Mã/Tên/Tỉ giá → Thêm → lưới cập nhật, form reset.
- [ ] Thiếu Mã / Tên / Tỉ giá → hiện cảnh báo tại control.
- [ ] Tỉ giá ≤ 0 → cảnh báo "Tỉ giá phải lớn hơn 0".
- [ ] Mã trùng → backend trả lỗi "Mã ngoại tệ đã tồn tại".

### Sửa
- [ ] Chọn dòng → dữ liệu đổ vào form, hiện thời gian sửa cuối.
- [ ] Sửa tỉ giá + thời điểm → Sửa → lưới cập nhật.
- [ ] Bản ghi đã khóa → nút Sửa bị vô hiệu.

### Khóa / Mở khóa / Xóa
- [ ] Icon khóa → xác nhận → đảo trạng thái, dòng chuyển màu xám khi khóa.
- [ ] Icon X → xác nhận → xóa mềm, lưới refresh.

### Tìm kiếm / Phân trang
- [ ] Nhập mã/tên → Tìm → lọc đúng.
- [ ] Tải lại → xóa từ khóa, nạp lại toàn bộ.
- [ ] Phân trang đúng số bản ghi.
