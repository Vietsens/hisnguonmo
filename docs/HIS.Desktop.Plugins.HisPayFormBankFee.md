# Cấu Hình Phụ Phí Ngân Hàng — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisPayFormBankFee |
| Loại | Form (danh mục chuẩn — lưới trái + panel sửa phải) |
| Mục đích | Quản trị cấu hình tỉ lệ phụ phí theo từng cặp **hình thức thanh toán + ngân hàng**. Cấu hình này được TransactionDeposit/cashier dùng để tính phụ phí khi quẹt thẻ / chuyển khoản. |
| Người tạo | huannh |
| Ngày tạo | 05/06/2026 |
| Trạng thái | Hoàn thành |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở form → tải danh sách cấu hình phụ phí qua API `Get` (server-side paging).
2. **Thêm**: chọn Hình thức thanh toán + Ngân hàng (hoặc "Tất cả ngân hàng") + nhập Tỉ lệ phụ phí (%) + Tên phụ phí → **Thêm (Ctrl N)**.
3. **Sửa**: double-click / Enter dòng trên lưới → đổ dữ liệu xuống panel → sửa → **Sửa (Ctrl S)**.
4. **Khóa/Mở khóa**: nút khóa trên mỗi dòng → bật/tắt `IS_ACTIVE` (xóa mềm, không xóa vật lý).

### Quy tắc ưu tiên cấu hình
- `BANK_ID = null` → cấu hình áp dụng cho **mọi ngân hàng** của hình thức thanh toán đó (wildcard).
- Cấu hình cụ thể (`payform + bank cụ thể`) được ưu tiên hơn cấu hình wildcard (`payform + tất cả NH`).

### Điều kiện nghiệp vụ (validate)
- Hình thức thanh toán: **bắt buộc** (chỉ liệt kê HT có `IS_REQUIRED_BANK = 1`).
- Tỉ lệ phụ phí: **bắt buộc > 0** (đơn vị %, VD `1.5` = 1,5%).
- Tên phụ phí: **bắt buộc**, tối đa **200 ký tự** (in trên biên lai như dòng riêng).
- **Không trùng cặp** `PAY_FORM_ID + BANK_ID` (kiểm tra qua API `Get` lọc theo `PAY_FORM_ID` trước khi lưu).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_PAY_FORM_BANK_FEE | Table | Bản ghi cấu hình phụ phí (PAY_FORM_ID, BANK_ID nullable, FEE_RATE, FEE_NAME, IS_ACTIVE) |
| HIS_PAY_FORM | Table (cache) | Combo Hình thức thanh toán — lọc `IS_ACTIVE=1 AND IS_REQUIRED_BANK=1` |
| HIS_BANK | Table (cache) | Combo Ngân hàng — lọc `IS_ACTIVE=1`, thêm dòng "(Tất cả ngân hàng)" = `ID 0` ↔ `BANK_ID null` |

### Trường chính HIS_PAY_FORM_BANK_FEE
`ID, PAY_FORM_ID (long), BANK_ID (long?), FEE_RATE (decimal?), FEE_NAME (string), IS_ACTIVE, IS_DELETE, CREATE_TIME, CREATOR, MODIFY_TIME, MODIFIER`.

## 4. UI Layout

```
+--------------------------------------------------+  +---------------------------+
| [Từ khóa tìm.........]  [Tìm (Ctrl F)]          |  | Thông tin cấu hình phụ phí|
+--------------------------------------------------+  | Hình thức thanh toán: * [v]|
| STT | 🔒 | Hình thức TT | Ngân hàng | Tỉ lệ(%) |  | Ngân hàng:            [v]  |
|     |    | Tên phụ phí  | Trạng thái| 4 cột   |  | Tỉ lệ phụ phí (%): *  [__] |
|     |    |              | audit     |          |  | Tên phụ phí: *       [____]|
+--------------------------------------------------+  | [Sửa][Thêm][Làm lại]      |
| Trang 1/1 — 1-5/5 bản ghi    Mỗi trang [50]     |  +---------------------------+
+--------------------------------------------------+
```

### Lưới (load 1 bảng → bắt buộc 4 cột audit ở cuối)
STT | 🔒(khóa/mở) | Hình thức thanh toán | Ngân hàng | Tỉ lệ phụ phí (%) | Tên phụ phí | Trạng thái | **Thời gian tạo | Người tạo | Thời gian sửa | Người sửa**.

### UC sử dụng
| UC | Mục đích |
|----|----------|
| Inventec.UC.Paging | Phân trang server-side |

> Combo dùng `LookUpEdit` (cấu hình columns runtime), không dùng UC riêng vì nguồn là danh mục cache đơn giản.

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy danh sách | `HIS_PAY_FORM_BANK_FEE_GET` = `/api/HisPayFormBankFee/Get` | MosConsumer | HisPayFormBankFeeFilter (KEY_WORD, PAY_FORM_ID, ID, ORDER_FIELD/DIRECTION) |
| Thêm mới | `HIS_PAY_FORM_BANK_FEE_CREATE` = `/api/HisPayFormBankFee/Create` | MosConsumer | — (ID để backend tự sinh) |
| Cập nhật / Khóa-mở | `HIS_PAY_FORM_BANK_FEE_UPDATE` = `/api/HisPayFormBankFee/Update` | MosConsumer | — (bắt buộc ID>0; khóa/mở = set IS_ACTIVE) |

> ⚠️ **`/Update` KHÔNG phải upsert.** Backend `Update` gọi `VerifyId` bắt buộc `ID > 0`; gọi với `ID=0` báo lỗi `MOS005 (Id invalid)`. Vì vậy **Thêm mới phải gọi `/Create`**, Sửa mới gọi `/Update`. (Tài liệu gốc ghi "Update = thêm/sửa" không đúng thực tế backend.)

## 6. Dependencies

### Library Plugins
Không dùng (danh mục thuần).

### Inter-Plugin
Không mở plugin khác. Dữ liệu cấu hình được các plugin viện phí (TransactionDeposit, DepositService...) đọc qua `BackendDataWorker.Get<HIS_PAY_FORM_BANK_FEE>()`.

## 7. Print
Không có chức năng in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 05/06/2026 | huannh | Tạo mới plugin: lưới + panel CRUD, Get/Update, khóa-mở qua IS_ACTIVE, validate tỉ lệ>0 / trùng cặp / tên ≤200, đa ngôn ngữ vi+en, 4 cột audit, paging. |
| 05/06/2026 | huannh | Fix: backend `/Update` bắt buộc ID>0 (MOS005) → tách `/Create` cho thêm mới. Bỏ MaxLength để rule validate tên ≤200 hoạt động. Set Padding=0 các LayoutControlGroup cho layout sát viền. |

## 9. Test Cases

### Tạo mới
- [ ] Chọn HT + NH + tỉ lệ + tên → Thêm → thành công, lưới refresh, panel reset.
- [ ] Bỏ trống HT / tỉ lệ ≤ 0 / tên trống → hiện validation tại control.
- [ ] Tên > 200 ký tự → cảnh báo "Tên phụ phí vượt quá 200 ký tự".
- [ ] Trùng cặp HT+NH đã tồn tại → cảnh báo "Cấu hình ... đã tồn tại".
- [ ] Chọn "(Tất cả ngân hàng)" → lưu `BANK_ID = null`, lưới hiển thị "(Tất cả ngân hàng)".

### Sửa
- [ ] Double-click dòng → đổ dữ liệu xuống panel, chuyển sang chế độ Sửa.
- [ ] Dòng đang khóa (`IS_ACTIVE=0`) → nút Sửa bị disable.
- [ ] Sửa tỉ lệ/tên → Sửa (Ctrl S) → lưới cập nhật.

### Khóa / Mở khóa
- [ ] Dòng hoạt động → nút khóa → xác nhận → `IS_ACTIVE=0`, cột Trạng thái = "Đã khóa".
- [ ] Dòng đã khóa → nút mở → xác nhận → `IS_ACTIVE=1`, cột Trạng thái = "Hoạt động".

### Khác
- [ ] Tìm theo từ khóa (tên HT / tên phụ phí) → lưới lọc đúng.
- [ ] Đổi ngôn ngữ vi/en → caption + thông báo đổi theo.
