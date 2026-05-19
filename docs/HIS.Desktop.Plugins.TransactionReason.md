# Danh mục Lý do giao dịch — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TransactionReason |
| Loại | Form |
| Mục đích | Quản lý danh mục `HIS_TRANSACTION_REASON` — lý do giao dịch dùng phân loại báo cáo (Khám / Điều trị / mở rộng). Cung cấp cho các màn hình Tạm ứng, Hoàn ứng, Thanh toán, Tạm ứng dịch vụ, Hoàn ứng dịch vụ, Danh sách giao dịch, Sửa thông tin giao dịch. |
| Người tạo | phuongnm |
| Ngày tạo | 16/05/2026 |
| Trạng thái | Đang phát triển |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Người dùng (admin danh mục) mở màn hình `Lý do giao dịch`.
2. Lưới hiển thị danh sách bản ghi `HIS_TRANSACTION_REASON` với filter `KEY_WORD` (mã/tên/ghi chú).
3. Người dùng chọn dòng để Sửa hoặc nhập mới ở panel phải để Thêm.
4. Bấm `Sửa (Ctrl+S)` / `Thêm (Ctrl+N)` để lưu — gọi API.
5. Bấm icon `Xóa` / `Khóa` trên lưới để xóa mềm hoặc đổi trạng thái hoạt động.
6. Sau khi lưu thành công, `BackendDataWorker.Reset<HIS_TRANSACTION_REASON>()` để các plugin tiêu dùng lấy dữ liệu mới.

### Điều kiện nghiệp vụ
- `TRANSACTION_REASON_CODE` không trống, max 20 ký tự, không trùng (backend kiểm).
- `TRANSACTION_REASON_NAME` không trống, max 200 ký tự.
- `DESCRIPTION` tối đa 500 ký tự.
- Bản ghi `IS_ACTIVE = 0` chỉ cho phép mở khóa, không cho sửa.
- 2 bản ghi chuẩn (`KHAM`, `DIEU_TRI`) là dữ liệu seed — frontend các plugin tiêu dùng chỉ chọn theo logic FE-COMMON-01/02/03.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_TRANSACTION_REASON | Table | Bảng danh mục chính. Field: `ID`, `TRANSACTION_REASON_CODE`, `TRANSACTION_REASON_NAME`, `DESCRIPTION`, `IS_ACTIVE`, `IS_DELETE`, audit fields. |
| HisTransactionReasonFilter | Filter | Filter `KEY_WORD`, `ID`, `IS_ACTIVE`, `ORDER_FIELD`, `ORDER_DIRECTION` cho API `Get`. |

### Quan hệ chính
- `HIS_TRANSACTION` có khóa ngoại `TRANSACTION_REASON_ID` tham chiếu tới bảng này (phục vụ phân loại báo cáo).

## 4. UI Layout

### Sơ đồ giao diện (theo screenshot 4.1.9 tài liệu thiết kế)
```
+--------------------------------------------------------------+--------------------+
| [txtKeyword]   [Tìm (Ctrl F)]                                | Mã: [txt]          |
+--------------------------------------------------------------+ Tên: [txt]         |
| Grid: STT | lock | delete | Mã lý do | Tên | Ghi chú |       | Ghi chú: [memo]    |
|        Trạng thái | TG tạo | Người tạo | TG sửa | Người sửa  | [Sửa][Thêm][Reset] |
+--------------------------------------------------------------+--------------------+
| [<<][<][1][>][>>][refresh][pageSize][...]      1 - n / total |                    |
+--------------------------------------------------------------+--------------------+
```

### Cột grid (theo thứ tự VisibleIndex)
0. STT — unbound, width 40, Fixed.Left
1. lock icon (gridColumn1) — Fixed.Left
2. delete/edit icon (gridColumnEdit) — Fixed.Left
3. Mã lý do (grdColReasonCode, FieldName=`TRANSACTION_REASON_CODE`)
4. Tên Lý do giao dịch (grdColReasonName, FieldName=`TRANSACTION_REASON_NAME`)
5. Ghi chú (grdColDescription, FieldName=`DESCRIPTION`)
6. Trạng thái (gridColumn2, FieldName=`IS_ACTIVE_STR`, unbound)
7. Thời gian tạo (grdColCreateTime, unbound từ `CREATE_TIME`)
8. Người tạo (grdColCreator, FieldName=`CREATOR`)
9. Thời gian sửa (grdColModifyTime, unbound từ `MODIFY_TIME`)
10. Người sửa (grdColModifier, FieldName=`MODIFIER`)

### Panel phải (editor)
- `txtTransactionReasonCode` (TextEdit, MaxLength=20)
- `txtTransactionReasonName` (TextEdit, MaxLength=200)
- `memoDescription` (MemoEdit, MaxLength=500)
- 3 nút: `btnEdit` Sửa (Ctrl+S), `btnAdd` Thêm (Ctrl+N), `btnRefresh` Làm lại (Ctrl+R)

### Phím tắt (BarManager)
- Ctrl+F: tìm kiếm
- Ctrl+S: sửa
- Ctrl+N: thêm
- Ctrl+R: làm lại form
- F2: focus về ô tìm kiếm

## 5. API Endpoints

| Action | URI | Consumer | Filter / Body |
|--------|-----|----------|---------------|
| Lấy danh sách | `api/HisTransactionReason/Get` | MosConsumer | HisTransactionReasonFilter |
| Tạo mới | `api/HisTransactionReason/Create` | MosConsumer | HIS_TRANSACTION_REASON DTO |
| Cập nhật | `api/HisTransactionReason/Update` | MosConsumer | HIS_TRANSACTION_REASON DTO |
| Xóa mềm | `api/HisTransactionReason/Delete` | MosConsumer | ID (long) |
| Khóa/Mở khóa | `api/HisTransactionReason/ChangeLock` | MosConsumer | ID (long) |

URI constants: [HisRequestUriStore.cs](../HIS/Plugins/HIS.Desktop.Plugins.TransactionReason/HisRequestUriStore.cs)

## 6. Dependencies

### Library Plugins
Không sử dụng Library Plugin nào — đây là danh mục đơn giản.

### Inter-Plugin
- Các plugin tiêu dùng dữ liệu danh mục này: TransactionDeposit, TransactionRepay, TransactionBill, TransactionBillTwoInOne, TransactionList, TransactionInfoEdit, DepositService, RepayService. Chúng đọc qua `BackendDataWorker.Get<HIS_TRANSACTION_REASON>()` (cache) và lọc `IS_ACTIVE == 1`.

## 7. Print
Không có chức năng in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 16/05/2026 | phuongnm | Tạo plugin danh mục `Lý do giao dịch` theo thiết kế 42668 mục 4.1.9. Pattern theo HisDepositReason, đổi entity sang HIS_TRANSACTION_REASON, thêm trường `DESCRIPTION` (memo), loại bỏ các trường `ABBREVIATION` và `IS_COMMON` không có trong thiết kế. |

## 9. Test Cases

### Tạo mới
- [ ] Nhập đủ Mã + Tên → Thêm thành công, lưới reload.
- [ ] Mã trống → hiện validation "Trường dữ liệu bắt buộc".
- [ ] Tên trống → hiện validation "Trường dữ liệu bắt buộc".
- [ ] Mã trùng với bản ghi đã có → backend trả lỗi, hiển thị message.
- [ ] Ghi chú > 500 ký tự → hiện validation "Nhập quá 500 kí tự."

### Sửa
- [ ] Double-click dòng → Form bên phải nhận giá trị Mã, Tên, Ghi chú.
- [ ] Sửa Tên → Lưu → Grid cập nhật.
- [ ] Dòng đã bị Tạm khóa (IS_ACTIVE=0) → nút Sửa bị disable.

### Khóa / Mở khóa / Xóa
- [ ] Click icon khóa → Confirm → Trạng thái đổi từ Hoạt động sang Tạm khóa (đỏ).
- [ ] Click icon mở khóa → Confirm → Trạng thái về Hoạt động (xanh).
- [ ] Click icon xóa (dòng Hoạt động) → Confirm → Xóa mềm thành công.

### Tìm kiếm
- [ ] Gõ KHAM vào txtKeyword + Enter → lưới hiển thị duy nhất bản ghi `KHAM`.
- [ ] Gõ chuỗi không khớp → lưới trống, không lỗi.

### Phân trang
- [ ] Dữ liệu > pageSize → UcPaging hiển thị nhiều trang, chuyển trang load đúng.

### Đa ngôn ngữ
- [ ] Chuyển sang en → tất cả label/button/cột grid đổi sang English.
- [ ] Chuyển sang vi → quay lại tiếng Việt có dấu.
