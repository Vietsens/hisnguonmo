# Thiết lập khoa - đối tượng thanh toán — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisDepaPatientTypeList |
| Loại | Form |
| Mục đích | Form dùng chung để thiết lập danh sách `HIS_DEPA_PATIENT_TYPE` (Khoa × Đối tượng thanh toán) cho dịch vụ. Được mở từ "Tạo loại thuốc" và "Tạo loại vật tư" — thay thế 2 bản copy `frmDepartmentPatientType` tách rời trước đây. |
| Người tạo | tuanln |
| Ngày tạo | 18/05/2026 |
| Trạng thái | Đang phát triển |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. User mở form từ plugin cha (MedicineTypeCreate / MaterialTypeCreate) bằng nút "Khoa-ĐTTT" — plugin cha gọi `PluginInstance.GetPluginInstance` truyền các tham số đầu vào.
2. Form hiển thị 2 grid: bên trái danh sách Khoa, bên phải danh sách Đối tượng thanh toán.
3. User chọn "Chọn theo" (Khoa / ĐTTT):
   - **Mode Khoa (mặc định)**: Chỉ chọn 1 khoa (radio bên Khoa). Bên ĐTTT: chọn nhiều (checkbox) + thiết lập "Tự động hao phí" / "Không hao phí" cho từng ĐTTT.
   - **Mode ĐTTT**: Chỉ chọn 1 ĐTTT (radio bên ĐTTT). Bên Khoa: chọn nhiều (checkbox) + thiết lập "Tự động hao phí" / "Không hao phí" cho từng Khoa.
4. Khi đổi radio (Khoa hoặc ĐTTT) → form load lại các bản ghi đã chọn của bên kia (kèm IS_AUTO_EXPEND/IS_NOT_EXPEND đã lưu).
5. User nhấn "Chọn (Ctrl+T)" → form build danh sách `HIS_DEPA_PATIENT_TYPE` mới, gọi `DelegateSelectData` callback trả về plugin cha rồi đóng form.

### Quy tắc nghiệp vụ
- **Mutex 2 cột hao phí**: `IS_AUTO_EXPEND` và `IS_NOT_EXPEND` không thể cùng tick. Khi 1 cột đã tick → cột còn lại DISABLE (không click được). Implement qua event `gridView.ShowingEditor` cancel khi cell mutex bị khóa.
- **Switch mode**: Khi user đổi combo "Chọn theo" → reset toàn bộ `selectedDepartments / selectedPatientTypes / unSelectedDepartments / unSelectedPatientTypes` tránh stale data, sau đó fill lại 2 grid theo mode mới.
- **Load lại theo radio**:
  - Mode Khoa: chọn radio 1 khoa → tick lại checkbox các ĐTTT đã save trước + cờ hao phí.
  - Mode ĐTTT: chọn radio 1 ĐTTT → tick lại checkbox các Khoa đã save trước + cờ hao phí.
- **EnsureDepaPatientTypeFromDb**: Khi có `serviceId` và `isCalledApi == false` → load thêm các bản ghi đã lưu DB qua `BackendDataWorker.Get<HIS_DEPA_PATIENT_TYPE>()`, merge vào danh sách hiện tại (de-dup theo DEPARTMENT_ID + PATIENT_TYPE_ID + SERVICE_ID), set `isCalledApi = true`.
- **Save dedupe + update**: khi build danh sách mới, nếu cặp (Dept, PT, Svc) đã tồn tại trong list → UPDATE cờ `IS_AUTO_EXPEND` / `IS_NOT_EXPEND` (không skip). Nếu chưa → ADD mới.
- **Clone input list** trong Constructor để tránh form mutate ảnh hưởng caller khi user Cancel/Close không Save.
- **Shared reference** giữa `selectedDepartments / selectedPatientTypes` và `dataSource[i]` — đảm bảo khi user toggle cờ hao phí trên grid, state save reflect đúng giá trị mới.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_DEPARTMENT | Table | Danh sách khoa (`api/HisDepartment/Get`, filter `IS_ACTIVE = 1`) |
| HIS_PATIENT_TYPE | Table | Danh sách đối tượng thanh toán (`api/HisPatientType/Get`, filter `IS_ACTIVE = 1`) |
| HIS_DEPA_PATIENT_TYPE | Table | Bản ghi gắn Khoa × ĐTTT × Service (lưu thêm `IS_AUTO_EXPEND`, `IS_NOT_EXPEND` — backend đã thêm) |

## 4. UI Layout

```
+----------------------------------------------------------+
| [Từ khóa tìm khoa] [Tìm Ctrl F] [Từ khóa tìm ĐTTT] [Tìm] |
+----------------------------+-----------------------------+
| Grid Khoa                  | Grid Đối tượng thanh toán   |
|  Radio | Chk | Mã | Tên |  |  Radio | Chk | Mã | Tên |  |
|       Tự ĐHP | Khg HP      |        Tự ĐHP | Khg HP      |
+----------------------------+-----------------------------+
| [Paging Khoa]              | [Paging ĐTTT]               |
+----------------------------+-----------------------------+
|                       Chọn theo: [Khoa] [Chọn Ctrl T]    |
+----------------------------------------------------------+
```

### Controls chính
| Control | Mục đích |
|---------|----------|
| `gridControlDepartment` | Grid khoa (radio + checkbox + IS_AUTO_EXPEND + IS_NOT_EXPEND) |
| `gridControlPatientType` | Grid ĐTTT (radio + checkbox + IS_AUTO_EXPEND + IS_NOT_EXPEND) |
| `cboChooseMode` | GridLookUpEdit chọn "Khoa" hoặc "ĐTTT" |
| `btnSave` | Nút "Chọn" — gọi callback trả về plugin cha + đóng form |
| `ucPagingDepartment`, `ucPagingPatientType` | Phân trang server-side |

### Mode behavior

| Mode | Bên Khoa | Bên ĐTTT |
|------|----------|----------|
| **DEPARTMENT (Khoa)** | Radio ENABLE, Checkbox DISABLE, 2 cột hao phí DISABLE | Radio DISABLE, Checkbox ENABLE, 2 cột hao phí ENABLE |
| **PATIENT_TYPE (ĐTTT)** | Radio DISABLE, Checkbox ENABLE, 2 cột hao phí ENABLE | Radio ENABLE, Checkbox DISABLE, 2 cột hao phí DISABLE |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy khoa | `api/HisDepartment/Get` | MosConsumer | HisDepartmentFilter |
| Lấy ĐTTT | `api/HisPatientType/Get` | MosConsumer | HisPatientTypeFilter |

**Lưu ý**: Plugin này KHÔNG tự gọi API Create/Update `HIS_DEPA_PATIENT_TYPE`. Plugin cha (MedicineTypeCreate / MaterialTypeCreate) sẽ nhận danh sách qua callback rồi tự gọi `api/HisDepaPatientType/CreateList` khi save dịch vụ.

## 6. Dependencies

### Inter-Plugin — Được gọi bởi
| Plugin cha | Args truyền vào |
|-----------|-----------------|
| HIS.Desktop.Plugins.MedicineTypeCreate | `Module`, `long? serviceId`, `List<HIS_DEPA_PATIENT_TYPE>`, `bool[2] {isCalledApi, isClickPick}`, `DelegateSelectData` |
| HIS.Desktop.Plugins.MaterialTypeCreate | Tương tự |

### Args đầu vào — Behavior.Run() parse

| Type | Mục đích | Bắt buộc |
|------|----------|----------|
| `Inventec.Desktop.Common.Modules.Module` | Module context | Không |
| `DepaPatientTypeInputADO` | Đóng gói tất cả tham số (cách 1) | Không |
| `long?` / `long` | ServiceId (cách 2 — args nguyên bản) | Không |
| `List<HIS_DEPA_PATIENT_TYPE>` | Danh sách đã chọn trước đó | Không |
| `bool[2]` | `[isCalledApi, isClickPick]` | Không |
| `DelegateSelectData` | Callback trả `DepaPatientTypeResultADO` | Có (để nhận kết quả) |

### Callback Output
Form gọi `delegateSelectData(result)` với `result` là `DepaPatientTypeResultADO`:
- `List<HIS_DEPA_PATIENT_TYPE> DepaPatientTypes` — danh sách mới (đã merge / xóa / update cờ)
- `bool IsCalledApi` — đã load DB hay chưa (để lần mở sau không load lại)
- `bool IsClickPick` — user đã nhấn Chọn

Plugin cha đọc kết quả qua **reflection** để TRÁNH add project reference đến plugin này.

## 7. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 18/05/2026 | tuanln | Tạo mới plugin HIS.Desktop.Plugins.HisDepaPatientTypeList — tách form `frmDepartmentPatientType` chung từ 2 plugin MedicineTypeCreate, MaterialTypeCreate. Thêm 2 cột mới `IS_AUTO_EXPEND` (Tự động hao phí) và `IS_NOT_EXPEND` (Không hao phí) ở cả 2 grid với rule MUTEX DISABLE (tick 1 cái thì cái còn lại disable per-cell qua `ShowingEditor` event). Sửa 2 plugin cha gọi qua inter-plugin (`PluginInstance.GetPluginInstance` + `DelegateSelectData`) thay vì `new frmDepartmentPatientType()` trực tiếp. 2 file `frmDepartmentPatientType.cs/.resx` ở plugin cha vẫn giữ trong codebase (chưa xóa), chỉ không còn được caller gọi đến. |

## 8. Test Cases

### Mode Khoa
- [ ] Mở form: combo "Chọn theo" mặc định = "Khoa". Bên Khoa radio bật, checkbox + 2 cột hao phí disable. Bên ĐTTT checkbox + 2 cột hao phí bật, radio disable.
- [ ] Chọn 1 khoa (radio) → grid ĐTTT tick lại các ĐTTT đã chọn trước (theo `depaPatientTypes` truyền vào + DB).
- [ ] Tick checkbox IS_AUTO_EXPEND ở 1 dòng ĐTTT → IS_NOT_EXPEND của dòng đó tự DISABLE (không click được).
- [ ] Tick checkbox IS_NOT_EXPEND ở 1 dòng ĐTTT → IS_AUTO_EXPEND tự DISABLE.
- [ ] Bỏ tick IS_AUTO_EXPEND → IS_NOT_EXPEND enable trở lại.
- [ ] Nhấn "Chọn (Ctrl+T)" khi chưa chọn khoa → hiện thông báo "Vui lòng chọn một khoa!"
- [ ] Nhấn "Chọn (Ctrl+T)" sau khi chọn → callback trả danh sách `HIS_DEPA_PATIENT_TYPE` với DEPARTMENT_ID = khoa được chọn, PATIENT_TYPE_ID = mỗi ĐTTT đã tick, IS_AUTO_EXPEND / IS_NOT_EXPEND theo trạng thái checkbox. Form tự đóng sau khi save.

### Mode ĐTTT
- [ ] Đổi combo sang "ĐTTT" → bên Khoa: checkbox + 2 cột hao phí bật. Bên ĐTTT: radio bật, checkbox + 2 cột disable.
- [ ] Chọn 1 ĐTTT (radio) → grid Khoa tick lại các khoa đã chọn trước.
- [ ] Tick header checkbox bên Khoa → tất cả khoa tick / bỏ tick.
- [ ] Nhấn "Chọn" khi chưa chọn ĐTTT → hiện thông báo "Vui lòng chọn một đối tượng thanh toán!"
- [ ] Nhấn "Chọn" → callback trả danh sách HIS_DEPA_PATIENT_TYPE với PATIENT_TYPE_ID = ĐTTT chọn, DEPARTMENT_ID = mỗi khoa đã tick.

### Tích hợp
- [ ] Mở từ "Tạo loại thuốc" (MedicineTypeCreate) → save → danh sách `depaPatientTypes` ở plugin cha cập nhật đúng.
- [ ] Mở từ "Tạo loại vật tư" (MaterialTypeCreate) → save → danh sách `depaPatientTypes` ở plugin cha cập nhật đúng.
- [ ] Khi `serviceId != null` lần đầu mở → load thêm dữ liệu DB hiện có (qua `BackendDataWorker.Get<HIS_DEPA_PATIENT_TYPE>()`).
- [ ] User Cancel/Close form không Save → list `depaPatientTypes` ở plugin cha KHÔNG bị mutate (do form đã clone list).
- [ ] User mở form lần 2 sau khi save lần 1 → các tick checkbox + cờ hao phí hiển thị đúng theo dữ liệu lần 1.
- [ ] User đổi cờ AutoExpend/NotExpend của ĐTTT đã tick (từ DB) → Save → cờ MỚI được lưu (không phải cờ cũ).
- [ ] Đa ngôn ngữ vi/en: text label, caption cột, thông báo đều theo culture.
