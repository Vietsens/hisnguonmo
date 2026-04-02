# Tài liệu phân tích thiết kế
# HIS.Desktop.Plugins.HisBidForm - Danh mục hình thức thầu

**Module:** `HIS.Desktop.Plugins.HisBidForm`
**Tên hiển thị:** Danh mục hình thức thầu
**Namespace:** `HIS.Desktop.Plugins.HisBidForm`
**Loại module:** Form (MODULE_TYPE_ID__FORM)
**Nhóm:** Bussiness

---

## 1. Tổng quan

Module **HisBidForm** cho phép quản lý danh mục **Hình thức thầu** (Bid Form) trong hệ thống HIS. Đây là module danh mục CRUD cơ bản, cho phép:

- **Thêm mới** hình thức thầu (mã, tên)
- **Sửa** thông tin hình thức thầu đã có
- **Xóa** hình thức thầu
- **Khóa/Mở khóa** hình thức thầu
- **Tìm kiếm** theo từ khóa
- Hỗ trợ **phân trang** danh sách

### Lý do tạo module

Trước đây, danh sách hình thức thầu được hardcode cứng 6 giá trị trong BidCreate và BidUpdate:
1. Đấu thầu rộng rãi
2. Đấu thầu hạn chế
3. Chỉ định thầu
4. Chào hàng cạnh tranh
5. Mua sắm trực tiếp
6. Khác

Module này cho phép quản lý danh mục động qua database, thay thế hardcode.

**Tham khảo:** Thiết kế và xử lý tương tự module **HIS.Desktop.Plugins.HisBidType** (Loại thầu).

---

## 2. Cấu trúc project

```
HIS.Desktop.Plugins.HisBidForm/
├── HisBidForm/
│   ├── IHisBidForm.cs                (Interface hành vi)
│   ├── HisBidFormFactory.cs          (Factory tạo behavior)
│   ├── HisBidFormBehavior.cs         (Behavior - tạo form chính)
│   ├── frmHisBidForm.cs              (Form chính - logic CRUD)
│   ├── frmHisBidForm.Designer.cs     (Giao diện form - DevExpress)
│   └── frmHisBidForm.resx            (Resources: icon, hình ảnh)
├── Properties/
│   └── AssemblyInfo.cs
├── HisBidFormProcessor.cs            (Entry point - đăng ký plugin)
├── HisRequestUriStore.cs             (Hằng số API endpoint)
├── ValidateMaxLength.cs              (Validation rule tùy chỉnh)
└── HIS.Desktop.Plugins.HisBidForm.csproj
```

---

## 3. Đăng ký Module

**File:** `HisBidFormProcessor.cs`

```
Module Link  : HIS.Desktop.Plugins.HisBidForm
Tên hiển thị : Danh mục
Nhóm         : Bussiness
Loại         : MODULE_TYPE_ID__FORM
Thuộc tính   : ExtensionOf(DesktopRootExtensionPoint)
```

**Luồng khởi tạo:**
```
HisBidFormProcessor.Run(object[] args)
  → HisBidFormFactory.MakeIControl()
    → HisBidFormBehavior.Run()
      → Trích xuất Module từ args
        → new frmHisBidForm(moduleData)
```

---

## 4. Thiết kế chi tiết

### 4.1. frmHisBidForm (`HisBidForm/frmHisBidForm.cs`)

Form chính kế thừa `HIS.Desktop.Utility.FormBase`, thực hiện toàn bộ nghiệp vụ CRUD cho danh mục hình thức thầu.

#### Biến trạng thái

| Biến | Type | Mô tả |
|------|------|-------|
| `ActionType` | int | Trạng thái hiện tại: `ActionAdd` (thêm) hoặc `ActionEdit` (sửa) |
| `currentData` | HIS_BID_FORM | Bản ghi đang được chọn trên grid |
| `positionHandle` | int | Vị trí tab control đang focus (dùng cho validation) |
| `startPage` | int | Vị trí bắt đầu trang hiện tại |
| `rowCount` | int | Số bản ghi trang hiện tại |
| `dataTotal` | int | Tổng số bản ghi |

#### Các phương thức quan trọng

| Phương thức | Chức năng |
|-------------|-----------|
| `MeShow()` | Khởi tạo form: set default, load data, set caption, validate |
| `FillDataToGridControl()` | Tải dữ liệu từ API và hiển thị lên grid với phân trang |
| `LoadPaging(object param)` | Gọi API lấy dữ liệu phân trang (GET HisBidForm) |
| `SetFilterNavBar(ref filter)` | Gán từ khóa tìm kiếm vào filter |
| `SaveProcess()` | Xử lý lưu: validate → tạo DTO → gọi API Create/Update |
| `UpdateDTOFromDataForm(ref dto)` | Map dữ liệu từ form (txtCode, txtName) vào DTO |
| `LoadCurrent(id, ref dto)` | Tải lại bản ghi hiện tại từ API (trước khi update) |
| `ChangedDataRow(data)` | Xử lý khi chọn dòng trên grid: fill data, đổi trạng thái sang Edit |
| `FillDataToEditorControl(data)` | Đổ dữ liệu bản ghi vào txtCode, txtName |
| `ResetFormData()` | Xóa trắng form nhập liệu |
| `EnableControlChanged(action)` | Enable/Disable nút Sửa/Thêm theo trạng thái |
| `ValidateForm()` | Thiết lập validation rule cho txtCode (max 2 ký tự) và txtName (max 100 ký tự) |
| `btnGDelete_ButtonClick` | Xử lý xóa bản ghi (gọi API Delete) |
| `btnGLock_ButtonClick` | Mở khóa bản ghi (gọi API ChangeLock) |
| `btnGunLock_ButtonClick` | Khóa bản ghi (gọi API ChangeLock) |
| `gridviewFormList_CustomRowCellEdit` | Hiển thị icon Khóa/Mở khóa/Xóa theo trạng thái IS_ACTIVE |
| `gridviewFormList_RowCellStyle` | Tô màu cột trạng thái: Xanh (hoạt động), Đỏ (tạm khóa) |
| `gridviewFormList_CustomUnboundColumnData` | Tính giá trị các cột unbound: STT, trạng thái, thời gian |

### 4.2. HisRequestUriStore (`HisRequestUriStore.cs`)

Hằng số các API endpoint:

| Hằng số | Giá trị | Mô tả |
|---------|---------|-------|
| `HisBidForm_CREATE` | `api/HisBidForm/Create` | Tạo mới hình thức thầu |
| `HisBidForm_DELETE` | `api/HisBidForm/Delete` | Xóa hình thức thầu |
| `HisBidForm_UPDATE` | `api/HisBidForm/Update` | Cập nhật hình thức thầu |
| `HisBidForm_GET` | `api/HisBidForm/Get` | Lấy danh sách hình thức thầu |
| `HisBidForm_CHANGE_LOCK` | `api/HisBidForm/ChangeLock` | Khóa/mở khóa hình thức thầu |

### 4.3. ValidateMaxLength (`ValidateMaxLength.cs`)

Validation rule tùy chỉnh kế thừa `DevExpress.XtraEditors.DXErrorProvider.ValidationRule`:

| Kiểm tra | Thông báo lỗi |
|----------|---------------|
| Trường bắt buộc (rỗng) | "Trường dữ liệu bắt buộc" |
| Vượt quá độ dài tối đa | "Chỉ được nhập tối đa {maxLength} kí tự" |

Sử dụng `Inventec.Common.String.CountVi.Count()` để đếm ký tự tiếng Việt chính xác.

---

## 5. Giao diện người dùng

### 5.1. Bố cục form chính

Form sử dụng `DevExpress LayoutControl` chia 2 phần: Grid bên trái, Form nhập bên phải.

```
┌──────────────────────────────────────────┬──────────────────────┐
│  [TOOLBAR ẩn] Ctrl+F | Ctrl+S | Ctrl+N  │                      │
│              | Ctrl+R | F2               │                      │
├──────────────────────────────────────────┤  DataNavigator       │
│  [Từ khóa tìm kiếm     ] [Tìm (Ctrl F)]│  (ẩn)                │
├──────────────────────────────────────────┤──────────────────────│
│  DANH SÁCH HÌNH THỨC THẦU               │  Mã HT thầu: [    ] │
│  ┌───┬──┬──┬────────┬─────────┬───────┐  │  Tên HT thầu:[    ] │
│  │STT│🔒│🗑│Mã HT   │Tên HT   │TT     │  │                      │
│  ├───┼──┼──┼────────┼─────────┼───────┤  │  [Sửa] [Thêm] [Làm  │
│  │ 1 │🔓│❌│01      │Đấu thầu │Hoạt   │  │   lại]               │
│  │   │  │  │        │rộng rãi │động   │  │                      │
│  │ 2 │🔓│❌│02      │Đấu thầu │Hoạt   │  │                      │
│  │   │  │  │        │hạn chế  │động   │  │                      │
│  │ 3 │🔒│  │03      │Chỉ định │Tạm    │  │                      │
│  │   │  │  │        │thầu     │khóa   │  │                      │
│  └───┴──┴──┴────────┴─────────┴───────┘  │                      │
│  [Phân trang: << < 1/5 > >>]             │                      │
└──────────────────────────────────────────┴──────────────────────┘
```

### 5.2. Các cột grid

| Cột | FieldName | Loại | Mô tả |
|-----|-----------|------|-------|
| STT | `STT` | Unbound | Số thứ tự tự động tính |
| Khóa/Mở khóa | `isLock` | Unbound + ButtonEdit | Icon khóa/mở khóa, click để đổi trạng thái |
| Xóa | `Delete` | Unbound + ButtonEdit | Icon xóa, chỉ hiện khi bản ghi đang hoạt động |
| Mã hình thức thầu | `BID_FORM_CODE` | Bound (readonly) | Mã hình thức thầu |
| Tên hình thức thầu | `BID_FORM_NAME` | Bound (readonly) | Tên hình thức thầu |
| Trạng thái | `IS_ACTIVE_STR` | Unbound | "Hoạt động" (xanh) / "Tạm khóa" (đỏ) |
| Thời gian tạo | `CREATE_TIME_STR` | Unbound | Format từ số sang chuỗi ngày giờ |
| Người tạo | `CREATOR` | Bound (readonly) | Username người tạo |
| Thời gian sửa | `MODIFY_TIME_STR` | Unbound | Format từ số sang chuỗi ngày giờ |
| Người sửa | `MODIFIER` | Bound (readonly) | Username người sửa |

### 5.3. Form nhập liệu (bên phải)

| Control | Tên | Validation | Mô tả |
|---------|-----|-----------|-------|
| `txtCode` | Mã HT thầu | Bắt buộc, tối đa 2 ký tự | Mã hình thức thầu |
| `txtName` | Tên HT thầu | Bắt buộc, tối đa 100 ký tự | Tên hình thức thầu |
| `btnEdit` | Sửa (Ctrl S) | - | Enable khi ActionType = Edit |
| `btnAdd` | Thêm (Ctrl N) | - | Enable khi ActionType = Add |
| `btnRefresh` | Làm lại (Ctrl R) | - | Reset form về trạng thái Thêm mới |

---

## 6. Quy trình nghiệp vụ

### 6.1. Luồng thêm mới

```
┌──────────────────────────┐
│   Form khởi tạo          │
│   ActionType = Add       │
│   btnAdd = Enabled       │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│   Nhập Mã + Tên          │
│   vào txtCode, txtName   │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│   Nhấn Thêm (Ctrl N)    │
│   → SaveProcess()        │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│   Validate:              │
│   - txtCode: bắt buộc,  │
│     max 2 ký tự          │
│   - txtName: bắt buộc,  │
│     max 100 ký tự        │
└────────────┬─────────────┘
             │ Hợp lệ
             ▼
┌──────────────────────────┐
│   Tạo HIS_BID_FORM DTO  │
│   IS_ACTIVE = TRUE       │
│   POST api/HisBidForm/   │
│        Create            │
└────────────┬─────────────┘
             │ Thành công
             ▼
┌──────────────────────────┐
│   Refresh grid           │
│   Reset form             │
│   Hiển thị thông báo     │
└──────────────────────────┘
```

### 6.2. Luồng sửa

```
┌──────────────────────────┐
│   Click/DoubleClick dòng │
│   trên grid              │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│   ChangedDataRow():      │
│   - Fill txtCode, txtName│
│   - ActionType = Edit    │
│   - btnEdit = Enabled    │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│   Sửa thông tin          │
│   Nhấn Sửa (Ctrl S)     │
│   → SaveProcess()        │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│   LoadCurrent() → lấy   │
│   bản ghi mới nhất từ DB│
│   UpdateDTOFromDataForm()│
│   POST api/HisBidForm/   │
│        Update            │
└────────────┬─────────────┘
             │ Thành công
             ▼
┌──────────────────────────┐
│   Refresh grid           │
│   Hiển thị thông báo     │
└──────────────────────────┘
```

### 6.3. Luồng xóa

```
Click icon Xóa trên grid
  → btnGDelete_ButtonClick()
    → POST api/HisBidForm/Delete (truyền ID)
      → Thành công: Refresh grid
      → Hiển thị thông báo kết quả
```

### 6.4. Luồng khóa/mở khóa

```
Click icon Khóa/Mở khóa trên grid
  → Hiển thị hộp thoại xác nhận (Yes/No)
    → Yes: POST api/HisBidForm/ChangeLock (truyền ID)
      → Thành công: Refresh grid
```

---

## 7. Kiểm tra & Validation

| Quy tắc | Control | Thông báo lỗi |
|---------|---------|---------------|
| Mã hình thức thầu bắt buộc | `txtCode` | "Trường dữ liệu bắt buộc" |
| Mã hình thức thầu tối đa 2 ký tự | `txtCode` | "Chỉ được nhập tối đa 2 kí tự" |
| Tên hình thức thầu bắt buộc | `txtName` | "Trường dữ liệu bắt buộc" |
| Tên hình thức thầu tối đa 100 ký tự | `txtName` | "Chỉ được nhập tối đa 100 kí tự" |

Validation sử dụng `DXValidationProvider` với rule `ValidateMaxLength` tùy chỉnh. Khi validation thất bại, focus tự động chuyển đến control lỗi đầu tiên (theo TabIndex).

---

## 8. Các API sử dụng

| Endpoint | Method | Mô tả |
|----------|--------|-------|
| `api/HisBidForm/Get` | GET | Lấy danh sách hình thức thầu (hỗ trợ phân trang, tìm kiếm) |
| `api/HisBidForm/Create` | POST | Tạo mới hình thức thầu |
| `api/HisBidForm/Update` | POST | Cập nhật hình thức thầu |
| `api/HisBidForm/Delete` | POST | Xóa hình thức thầu (truyền ID) |
| `api/HisBidForm/ChangeLock` | POST | Khóa/mở khóa hình thức thầu (truyền ID) |

### Chi tiết API Get

**Request:** `HisBidFormFilter`
```
HisBidFormFilter (kế thừa FilterBase)
├── KEY_WORD        → Từ khóa tìm kiếm
├── ID              → Lọc theo ID cụ thể
├── ORDER_FIELD     → Sắp xếp theo trường (mặc định: MODIFY_TIME)
├── ORDER_DIRECTION → Chiều sắp xếp (mặc định: DESC)
├── Start           → Vị trí bắt đầu (phân trang)
└── Limit           → Số bản ghi mỗi trang
```

**Response:** `ApiResultObject<List<HIS_BID_FORM>>`

---

## 9. Phím tắt

| Phím tắt | Chức năng |
|----------|-----------|
| `Ctrl + F` | Tìm kiếm |
| `Ctrl + S` | Sửa (khi đang ở chế độ Edit) |
| `Ctrl + N` | Thêm mới (khi đang ở chế độ Add) |
| `Ctrl + R` | Làm lại (reset form) |
| `F2` | Focus vào ô tìm kiếm |

---

## 10. Phụ thuộc

### 10.1. Project references

| Project | Vai trò |
|---------|---------|
| `HIS.Desktop.ApiConsumer` | Gọi API backend (BackendAdapter) |
| `HIS.Desktop.Common` | Utilities, GlobalVariables, BusinessBase |
| `HIS.Desktop.Controls.Session` | Quản lý phiên (SessionManager.ProcessTokenLost) |
| `HIS.Desktop.LibraryMessage` | Thư viện thông báo (MessageUtil) |
| `HIS.Desktop.LocalStorage.BackendData` | ApiConsumers.MosConsumer |
| `HIS.Desktop.LocalStorage.ConfigApplication` | ConfigApplicationWorker (page size) |
| `HIS.Desktop.LocalStorage.LocalData` | GlobalVariables (ActionAdd, ActionEdit) |
| `HIS.Desktop.LocalStorage.Location` | ApplicationStoreLocation (icon path) |
| `HIS.Desktop.ModuleExt` | Module extension base |
| `HIS.Desktop.Utilities` | FormBase, WaitingManager |

### 10.2. External DLLs

| DLL | Mô tả |
|-----|-------|
| `DevExpress.XtraEditors` (v15.2) | TextEdit, SimpleButton, DataNavigator, PanelControl |
| `DevExpress.XtraGrid` (v15.2) | GridControl, GridView, GridColumn |
| `DevExpress.XtraLayout` (v15.2) | LayoutControl, LayoutControlItem |
| `DevExpress.XtraBars` (v15.2) | BarManager, BarButtonItem (phím tắt) |
| `MOS.EFMODEL` | Data model: HIS_BID_FORM |
| `MOS.Filter` | HisBidFormFilter |
| `IMSys.DbConfig.HIS_RS` | Hằng số: IS_ACTIVE__TRUE, IS_ACTIVE__FALSE |
| `Inventec.Common.Adapter` | BackendAdapter |
| `Inventec.Common.Logging` | LogSystem |
| `Inventec.Common.DateTime` | Convert.TimeNumberToTimeString |
| `Inventec.Common.String` | CountVi.Count (đếm ký tự tiếng Việt) |
| `Inventec.Common.Mapper` | DataObjectMapper.Map |
| `Inventec.Common.TypeConvert` | Parse.ToInt16 |
| `Inventec.Desktop.Common` | Message (MessageManager), Controls (ValidationProvider) |
| `Inventec.Desktop.Core` | Plugin attribute, DesktopRootExtensionPoint |
| `Inventec.UC.Paging` | UcPaging (phân trang) |

---

## 11. Model dữ liệu

### 11.1. HIS_BID_FORM (Bảng mới)

| Field | Type | Mô tả |
|-------|------|-------|
| `ID` | long | Khóa chính |
| `BID_FORM_CODE` | string | Mã hình thức thầu |
| `BID_FORM_NAME` | string | Tên hình thức thầu |
| `IS_ACTIVE` | short? | Trạng thái: 1 = Hoạt động, 0 = Tạm khóa |
| `IS_DELETE` | short? | Cờ xóa mềm |
| `CREATE_TIME` | long? | Thời gian tạo (dạng số yyyyMMddHHmmss) |
| `MODIFY_TIME` | long? | Thời gian sửa cuối |
| `CREATOR` | string | Username người tạo |
| `MODIFIER` | string | Username người sửa cuối |
| `APP_CREATOR` | string | Ứng dụng tạo |
| `APP_MODIFIER` | string | Ứng dụng sửa |
| `GROUP_CODE` | string | Mã nhóm |

### 11.2. Liên kết với HIS_BID

Bảng `HIS_BID` có field `BID_FORM_ID` (nullable long) tham chiếu đến `HIS_BID_FORM.ID`. Sau khi module này hoạt động, BidCreate và BidUpdate sẽ gọi API lấy danh mục từ HIS_BID_FORM thay vì hardcode.

---

## 12. Lưu ý triển khai

| Phụ thuộc | Trạng thái | Ghi chú |
|-----------|------------|---------|
| Bảng `HIS_BID_FORM` trong DB | Cần tạo | Người làm phần DB tạo |
| Class `HIS_BID_FORM` trong `MOS.EFMODEL` | Cần tạo | Người làm phần BE tạo |
| Class `HisBidFormFilter` trong `MOS.Filter` | Cần tạo | Người làm phần BE tạo |
| API CRUD cho HisBidForm | Cần tạo | Người làm phần BE tạo |
| Sửa BidCreate/BidUpdate gọi API | Tách riêng | Người khác làm (FE phần 2) |
| Đăng ký module trong ACS | Cần cấu hình | Thêm ModuleLink trên hệ thống |
