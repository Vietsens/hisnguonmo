# HIS.Desktop.Plugins.HisGoodsType — Loại dịch vụ ngoài khám chữa bệnh

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisGoodsType |
| Loại | Form (FormBase) |
| Mục đích | Quản lý danh mục Loại dịch vụ ngoài khám chữa bệnh (CRUD + khoá/mở khoá). Áp dụng cho Thanh toán khác — phân loại hàng hoá / dịch vụ ngoài KCB như: vận chuyển, giường, sổ khám bệnh, dịch vụ người nhà, khác. |
| Người tạo | anhnh2 |
| Ngày tạo | 28/04/2026 |
| Trạng thái | Đang phát triển — phụ thuộc backend gencode HIS_GOODS_TYPE |
| Liên quan | Nghiệp vụ #42922 — Thanh Toán Khác — Thêm Phân Loại Hàng Hoá |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

```
Mở module → Load danh sách HIS_GOODS_TYPE
  → Người dùng nhập Mã / Tên / Số sắp xếp
    → Validate (Mã, Tên bắt buộc; Số sắp xếp >= 0)
      → Lưu → Backend kiểm tra trùng Mã
        → Hiển thị thông báo + refresh grid
```

### Các thao tác hỗ trợ

| Thao tác | Mô tả | Phím tắt |
|----------|-------|----------|
| Tìm kiếm | Tìm theo Mã / Tên (KEY_WORD) | Ctrl + F |
| Thêm mới | Nhập dữ liệu → Lưu | Ctrl + N |
| Sửa | Chọn dòng grid → đổi dữ liệu → Lưu | Ctrl + S |
| Khoá / Mở khoá | Nút trong grid (cột isLock) — toggle IS_ACTIVE | — |
| Xoá mềm | Nút Xoá trong grid — gán IS_DELETE = 1 | — |
| Làm lại | Reset form về trạng thái thêm mới | Ctrl + R |
| Focus tìm kiếm | Đưa focus về ô tìm kiếm | F2 |

### Điều kiện nghiệp vụ

- Mã loại dịch vụ phải UNIQUE (validate ở backend khi Create / Update).
- Không cho xoá nếu loại đang được dùng trong HIS_NONE_MEDI_SERVICE.GOODS_TYPE_ID (validate backend, plugin chỉ gọi API và hiển thị message).
- Khi loại đã bị khoá (IS_ACTIVE = 0): không cho phép Sửa.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_GOODS_TYPE | Table (bảng mới — gencode) | Danh mục loại dịch vụ ngoài KCB |

### Cột bảng HIS_GOODS_TYPE

| Cột | Kiểu | Nullable | Mô tả |
|-----|------|----------|-------|
| ID | NUMBER(19) | No | Khoá chính |
| GOODS_TYPE_CODE | VARCHAR2(20) | No | Mã loại — UNIQUE |
| GOODS_TYPE_NAME | VARCHAR2(200) | Yes | Tên hiển thị |
| NUM_ORDER | NUMBER(19) | Yes | Thứ tự sắp xếp |
| (các cột audit chuẩn) | — | — | IS_ACTIVE, IS_DELETE, GROUP_CODE, CREATE_TIME, MODIFY_TIME, CREATOR, MODIFIER, APP_CREATOR, APP_MODIFIER |

### Dữ liệu mặc định khi khởi tạo

| GOODS_TYPE_CODE | GOODS_TYPE_NAME | NUM_ORDER |
|-----------------|-----------------|-----------|
| VAN_CHUYEN | Vận chuyển | 1 |
| GIUONG | Giường | 2 |
| SO_KHAM_BENH | Sổ khám bệnh | 3 |
| DV_NGUOI_NHA | Dịch vụ người nhà | 4 |
| KHAC | Khác | 99 |

## 4. UI Layout

### Sơ đồ giao diện

```
+-----------------------------------------------------------------+
| [Tìm (Ctrl F)] [ Từ khoá tìm kiếm.................... ]         |
+-----------------------------------------------+-----------------+
| Grid danh sách                                 | Mã loại DV: __|
| STT|Lock|Edit|Mã|Tên|Số sx|Trạng thái         | Tên loại DV: _|
|    |      |    |  |   |     |Tg tạo|Người tạo  | Số sắp xếp:  _|
|    |      |    |  |   |     |Tg sửa|Người sửa  |               |
|                                                | [Sửa] [Thêm] [Lại]
| [Phân trang]                                   |               |
+-----------------------------------------------+-----------------+
```

### Đặc tả control

| Control | Type | Mô tả |
|---------|------|-------|
| txtKeyword | TextEdit | Ô tìm kiếm theo từ khoá |
| btnSearch | SimpleButton | Nút tìm |
| gridControlFormList | GridControl | Lưới danh sách |
| ucPaging | Inventec.UC.Paging.UcPaging | Phân trang server-side |
| txtGoodsTypeCode | TextEdit | Mã (Maroon — bắt buộc, max 20) |
| txtGoodsTypeName | TextEdit | Tên (Maroon — bắt buộc, max 200) |
| spNumOrder | SpinEdit | Số sắp xếp (>= 0) |
| btnEdit / btnAdd / btnRefresh | SimpleButton | CRUD buttons |
| dxValidationProviderEditorInfo | DXValidationProvider | Validate chuẩn DevExpress |
| barManager1 | BarManager | Phím tắt Ctrl+F/S/N/R + F2 |

### Cột grid

| # | FieldName | Caption | Visible |
|---|-----------|---------|---------|
| 1 | STT | STT | Yes |
| 2 | isLock | (icon) | Yes |
| 3 | Delete | (icon) | Yes |
| 4 | GOODS_TYPE_CODE | Mã loại dịch vụ | Yes |
| 5 | GOODS_TYPE_NAME | Tên loại dịch vụ | Yes |
| 6 | NUM_ORDER | Số sắp xếp | Yes |
| 7 | IS_ACTIVE_STR | Trạng thái | Yes |
| 8 | CREATE_TIME_STR | Thời gian tạo | Yes |
| 9 | CREATOR | Người tạo | Yes |
| 10 | MODIFY_TIME_STR | Thời gian sửa | Yes |
| 11 | MODIFIER | Người sửa | Yes |

## 5. API Endpoints

| Action | URI | Consumer | Filter / DTO |
|--------|-----|----------|--------------|
| Lấy danh sách | `api/HisGoodsType/Get` | MosConsumer | HisGoodsTypeFilter |
| Tạo mới | `api/HisGoodsType/Create` | MosConsumer | HIS_GOODS_TYPE |
| Cập nhật | `api/HisGoodsType/Update` | MosConsumer | HIS_GOODS_TYPE |
| Xoá mềm | `api/HisGoodsType/Delete` | MosConsumer | long (ID) |
| Khoá / Mở khoá | `api/HisGoodsType/ChangeLock` | MosConsumer | long (ID) |

URI constants ở `HisRequestUriStore.cs`.

## 6. Dependencies

### Library Plugins
Không sử dụng library plugin chuyên biệt. Chỉ dùng UC chuẩn:
- Inventec.UC.Paging — phân trang.
- DevExpress controls (LookUpEdit, GridControl, TextEdit, SpinEdit).

### Inter-Plugin
Plugin được consumer khác gọi gián tiếp:
- HIS.Desktop.Plugins.HisNoneMediService — combobox "Loại dịch vụ" load từ `api/HisGoodsType/Get?IS_ACTIVE=1`.

## 7. Print
Không in ấn.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 28/04/2026 | anhnh2 | Tạo mới plugin theo nghiệp vụ #42922 |

## 9. Test Cases

### Tạo mới
- [ ] Nhập đầy đủ Mã + Tên + Số sắp xếp → Lưu thành công, Grid refresh.
- [ ] Bỏ trống Mã → Hiện lỗi "Trường dữ liệu bắt buộc" tại Mã.
- [ ] Bỏ trống Tên → Hiện lỗi tại Tên.
- [ ] Nhập Mã trùng → Backend trả lỗi → Hiện thông báo từ MessageManager.
- [ ] Số sắp xếp âm → Validate fail "Trường dữ liệu không nhận giá trị âm".

### Sửa
- [ ] Click dòng grid → Form load đầy đủ Mã / Tên / Số sắp xếp.
- [ ] Đổi Tên + Lưu → Grid cập nhật, không đổi Mã.
- [ ] Khi dòng đã khoá (IS_ACTIVE = 0) → btnEdit disable.

### Khoá / Mở khoá
- [ ] Nhấn nút khoá → confirm Yes → IS_ACTIVE đổi → icon đổi.
- [ ] Mở khoá tương tự.

### Xoá
- [ ] Nhấn nút Xoá → confirm Yes → IS_DELETE = 1 → biến mất khỏi grid.
- [ ] Loại đang dùng trong HIS_NONE_MEDI_SERVICE → Backend trả lỗi → message "Loại dịch vụ đang được sử dụng. Không thể xoá."

### Đa ngôn ngữ
- [ ] Đổi sang en → tất cả label / caption đổi.

### Phân trang
- [ ] Số bản ghi > pageSize → ucPaging hiện đúng số trang, chuyển trang OK.
