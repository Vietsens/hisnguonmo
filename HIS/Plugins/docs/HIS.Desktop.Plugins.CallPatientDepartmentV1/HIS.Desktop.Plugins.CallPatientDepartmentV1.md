# Tài liệu phân tích thiết kế
# HIS.Desktop.Plugins.CallPatientDepartmentV1 - Màn hình chờ xử lý theo khoa 1

---

## 1. Mục đích

Cung cấp màn hình chờ hiển thị tổng quan tình hình các phòng khám, phòng CLS trong cùng một khoa. Người bệnh ngồi chờ ngoài sảnh có thể xem được các phòng đã xử lý đến số thứ tự bao nhiêu, bao nhiêu ca chưa khám, đang khám, khám xong.

**Plugin tham khảo:** HIS.Desktop.Plugins.CallPatientDepartment (Màn hình chờ tại khoa)

---

## 2. Cấu trúc project

```
HIS.Desktop.Plugins.CallPatientDepartmentV1/
├── ADO/
│   └── RoomADO.cs
├── CallPatientDepartmentV1/
│   ├── ICallPatientDepartmentV1.cs
│   ├── CallPatientDepartmentV1Factory.cs
│   └── CallPatientDepartmentV1Behavior.cs
├── Resources/
│   ├── ResourceLanguageManager.cs
│   ├── Lang.en.resx
│   └── Lang.vi.resx
├── Properties/
│   └── AssemblyInfo.cs
├── CallPatientDepartmentV1Processor.cs
├── FormConfigWaitingScreen.cs
├── FormConfigWaitingScreen.Designer.cs
├── FormConfigWaitingScreen.resx
├── FormWaitingScreenV1.cs
├── FormWaitingScreenV1.Designer.cs
├── FormWaitingScreenV1.resx
├── WaitingScreenCFG.cs
└── HIS.Desktop.Plugins.CallPatientDepartmentV1.csproj
```

---

## 3. Đăng ký Module

**File:** `CallPatientDepartmentV1Processor.cs`

```
Module Link  : HIS.Desktop.Plugins.CallPatientDepartmentV1
Tên hiển thị : Màn hình chờ xử lý theo khoa 1
Icon         : man-hinh.png
Nhóm         : Common
Loại         : MODULE_TYPE_ID__FORM
```

**Luồng khởi tạo:**
```
CallPatientDepartmentV1Processor.Run()
  → CallPatientDepartmentV1Factory.MakeIControl()
    → CallPatientDepartmentV1Behavior.Run()
      → new FormConfigWaitingScreen(moduleData)
```

---

## 4. Thiết kế chi tiết

### 4.1. RoomADO (ADO/RoomADO.cs)

Data object đại diện cho một phòng trong danh sách thiết lập.

| Property | Type | Mô tả |
|----------|------|-------|
| ROOM_ID | long | ID phòng (từ V_HIS_EXECUTE_ROOM.ROOM_ID) |
| EXECUTE_ROOM_CODE | string | Mã phòng |
| EXECUTE_ROOM_NAME | string | Tên phòng |
| IsCheck | bool | Trạng thái checkbox (được chọn hay không) |
| OrderIndex | int | Vị trí sắp xếp, cho phép nhập số nguyên >= 0 |

---

### 4.2. FormConfigWaitingScreen - Màn hình thiết lập

**File:** `FormConfigWaitingScreen.cs`, `FormConfigWaitingScreen.Designer.cs`

#### 4.2.1. Giao diện (DevExpress)

```
┌──────────────────────────────────────────────────┐
│ [LblRoom] PHÒNG KHÁM 1 (KHOA NỘI)              │
├──────────────────────────────────────────────────┤
│ [txtSearch] Nhập từ khóa để tìm kiếm phòng...   │
├──┬────────┬──────────────────────────────┬───────┤
│✓ │ Mã     │ Tên                          │Vị trí │
├──┼────────┼──────────────────────────────┼───────┤
│☑ │ 01     │ phòng khám 1                 │  0    │
│☑ │ PKT    │ Phòng khám thô              │  0    │
│☐ │ P143   │ P.XN Huyết Học - Tầng 1     │  0    │
│☐ │ P144   │ Đơn Vị Hỗ Trợ Sinh Sản     │  0    │
│  │ ...    │ ...                          │ ...   │
├──┴────────┴──────────────────────────────┴───────┤
│ Thời gian tải lại (giây): [10▲▼]    [Bật màn hình│
│                                       mở rộng]   │
└──────────────────────────────────────────────────┘
```

#### 4.2.2. Danh sách control

| Control | Loại DevExpress | Mô tả |
|---------|----------------|-------|
| layoutControl1 | LayoutControl | Layout chính |
| LblRoom | LabelControl | Hiển thị tên phòng/khoa đang mở, font Tahoma 12 |
| txtSearch | TextEdit | Tìm kiếm phòng, placeholder "Nhập từ khóa để tìm kiếm phòng thực hiện" |
| gridControl1 / gridView1 | GridControl / GridView | Danh sách phòng |
| Gc_Check | GridColumn | Checkbox chọn phòng (RepositoryItemCheckEdit) |
| Gc_RoomCode | GridColumn | Mã phòng, FieldName = "EXECUTE_ROOM_CODE", readonly |
| Gc_RoomName | GridColumn | Tên phòng, FieldName = "EXECUTE_ROOM_NAME", readonly |
| Gc_OrderIndex | GridColumn | Vị trí, FieldName = "OrderIndex" (RepositoryItemSpinEdit, IsFloatValue=false, Min=0, Max=999) |
| spinReloadTime | SpinEdit | Thời gian tải lại (giây), IsFloatValue=false, Min=0, Max=9999, mặc định=10. Label màu Maroon (bắt buộc) |
| tgExtendMonitor | ToggleSwitch | Bật/tắt màn hình chờ mở rộng |

#### 4.2.3. Luồng xử lý

**Load form:**
```
FormConfigWaitingScreen_Load
  ├── SetCaptionByLanguageKey()
  ├── LoadDataToGrid()
  │     └── BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>()
  │           Filter: DEPARTMENT_ID = khoa hiện tại, IS_ACTIVE = 1
  │           → Tạo List<RoomADO>, phòng hiện tại được check mặc định
  ├── InitControlState()
  │     └── ControlStateWorker.GetData(ModuleLinkName)
  │           → Restore: phòng được tích, vị trí, thời gian tải lại
  └── Kiểm tra FormWaitingScreenV1 đã mở → set toggle ON
```

**Tìm kiếm phòng:**
```
txtSearch_EditValueChanged
  └── Lọc ListRoom theo keyword (so sánh EXECUTE_ROOM_CODE, EXECUTE_ROOM_NAME)
        → Cập nhật gridControl1.DataSource
```

**Check phòng:**
```
repositoryItemCheckRoom_CheckedChanged
  └── Không giới hạn số phòng được chọn
```

**Bật màn hình chờ (Toggle ON):**
```
tgExtendMonitor_Toggled (IsOn = true)
  ├── ValidateReloadTime()
  │     └── Nếu rỗng hoặc < 1 → DXErrorProvider hiện icon warning
  ├── Kiểm tra có phòng được chọn
  ├── SaveControlState() → Lưu cấu hình vào ControlStateWorker
  ├── Lấy danh sách phòng đã chọn, sắp xếp theo OrderIndex
  ├── new FormWaitingScreenV1(lstRoom, moduleData, reloadTime)
  ├── ShowFormInExtendMonitor(screen)
  │     ├── Nếu có màn hình phụ → fullscreen trên màn hình phụ
  │     └── Nếu không → hiện thông báo, show form bình thường
  └── this.Close()
```

**Tắt màn hình chờ (Toggle OFF):**
```
tgExtendMonitor_Toggled (IsOn = false)
  └── Tìm và đóng FormWaitingScreenV1 trong Application.OpenForms
```

#### 4.2.4. Validation

| Trường | Quy tắc | Xử lý |
|--------|---------|-------|
| spinReloadTime | Bắt buộc nhập, giá trị >= 1. Label màu Maroon. MinValue=0 (cho phép nhập 0 hoặc để trống, validation bắt khi toggle) | DXErrorProvider hiện icon warning bên cạnh ô nhập. Kiểm tra realtime khi thay đổi giá trị và khi bật toggle |
| Danh sách phòng | Phải chọn ít nhất 1 phòng | XtraMessageBox thông báo khi bật toggle |

#### 4.2.5. Lưu trữ cấu hình (ControlStateWorker)

Sử dụng `HIS.Desktop.Library.CacheClient.ControlStateWorker` để lưu/restore cấu hình lần sử dụng trước.

| KEY | VALUE format | Mô tả |
|-----|-------------|-------|
| CHECKED_ROOM_IDS | "123,456,789" | Danh sách ROOM_ID được tích, phân cách dấu phẩy |
| ROOM_ORDERS | "123:1,456:2" | Vị trí phòng, format roomId:orderIndex |
| RELOAD_TIME | "10" | Thời gian tải lại (giây) |

**Thời điểm lưu:** Trước khi mở FormWaitingScreenV1 (trong `tgExtendMonitor_Toggled`)
**Thời điểm restore:** Sau khi load danh sách phòng (trong `FormConfigWaitingScreen_Load`)

---

### 4.3. FormWaitingScreenV1 - Màn hình chờ

**File:** `FormWaitingScreenV1.cs`, `FormWaitingScreenV1.Designer.cs`

#### 4.3.1. Giao diện (DevExpress)

```
┌──────────────────────────────────────────────────────────────────────────────┐
│ Tên phòng  │STT tiếp theo│STT lớn nhất│STT vừa gọi│ Tổng │Chưa khám│Đang khám│Khám xong│
├────────────┼─────────────┼────────────┼───────────┼──────┼─────────┼─────────┼─────────┤
│Phòng khám 1│      2      │     7      │     1     │   7  │    6    │    1    │    0    │
│Phòng khám  │  Không có   │     1      │     1     │   1  │    0    │    1    │    0    │
│thô         │             │            │           │      │         │         │         │
│            │             │            │           │      │         │         │         │
│            │             │            │           │      │         │         │         │
└──────────────────────────────────────────────────────────────────────────────┘
```

#### 4.3.2. Danh sách cột grid

| STT | Header | FieldName | Font | Align | Ghi chú |
|-----|--------|-----------|------|-------|---------|
| 1 | Tên phòng | EXECUTE_ROOM_CODE | Arial 15.75 Regular | Left | |
| 2 | STT tiếp theo | NEXT_CALL_NUMBER | Arial 20.25 Bold | Center | Null → "Không có" |
| 3 | STT lớn nhất | MAX_NUM_ORDER | Arial 20.25 Bold | Center | |
| 4 | STT vừa gọi | CURRENT_CALL_NUMBER | Arial 20.25 Bold | Center | |
| 5 | Tổng | TOTAL_TODAY_SERVICE_REQ | Arial 20.25 Bold | Center | |
| 6 | Chưa khám | TOTAL_NEW_SERVICE_REQ | Arial 20.25 Bold | Center | |
| 7 | Đang khám | TOTAL_PROC_SERVICE_REQ | Arial 20.25 Bold | Center | |
| 8 | Khám xong | TOTAL_END_SERVICE_REQ | Arial 20.25 Bold | Center | |

**Style chung:**
- Header: nền xanh (128,128,255), chữ trắng, font Arial 15.75 Bold, căn giữa
- ColumnPanelRowHeight = 50, RowHeight = 50
- ShowGroupPanel = false, ShowIndicator = false
- BorderStyle = NoBorder, ColumnAutoWidth = true
- Ẩn cả scroll ngang và dọc (ScrollVisibility.Never)

#### 4.3.3. Nguồn dữ liệu

**API:** `api/HisRoom/GetCounterLView3`
**Filter:** `HisRoomCounterLView3Filter` với `ROOM_IDs` = danh sách ROOM_ID từ phòng đã chọn
**Output:** `List<L_HIS_ROOM_COUNTER_3>`

**Các trường của L_HIS_ROOM_COUNTER_3 (bổ sung so với L_HIS_ROOM_COUNTER):**

| Trường | Kiểu | Mô tả | Logic xử lý |
|--------|------|-------|-------------|
| CURRENT_CALL_NUMBER | NUMBER(19,0) | STT đang gọi | NUM_ORDER của HIS_SERVICE_REQ có CALL_COUNT > 0, lấy bản ghi CALL_TIME lớn nhất |
| NEXT_CALL_NUMBER | NUMBER(19,0) | STT tiếp theo | NUM_ORDER của HIS_SERVICE_REQ có (CALL_COUNT < 0 hoặc null) và CALL_TIME null, lấy NUM_ORDER nhỏ nhất |
| TOTAL_PROC_SERVICE_REQ | NUMBER | Tổng đang xử lý | Đếm HIS_SERVICE_REQ có SERVICE_REQ_STT_ID = 2 |

#### 4.3.4. Xử lý đặc biệt

**Cột STT tiếp theo - Null handling:**
```csharp
// gridView1_CustomColumnDisplayText
if (FieldName == "NEXT_CALL_NUMBER" && Value == null)
    DisplayText = "Không có"
```

#### 4.3.5. Timer

| Timer | Interval | Chức năng |
|-------|----------|-----------|
| timerReload | reloadTimeInSeconds * 1000 (từ user nhập) | Tự động gọi lại API load dữ liệu mới |
| timerScroll | 5000ms (5 giây) | Tự động cuộn grid xuống dòng tiếp theo, quay lại đầu khi hết |

**Luồng reload dữ liệu:**
```
timerReload_Tick
  → LoadDataGrid()
    → Task.Factory.StartNew(ExecuteThreadLoadDataGrid)
      → Invoke(FillDataToGridControl)
        → BackendAdapter.Get<List<L_HIS_ROOM_COUNTER_3>>("api/HisRoom/GetCounterLView3")
          → gridControl1.DataSource = result
```

**Luồng auto scroll:**
```
timerScroll_Tick
  → CreateNextRow()
    → Task.Factory.StartNew(ExecuteThreadCreateNextRow)
      → Invoke(NextRowGridView)
        → gridView1.FocusedRowHandle = index++
          (nếu index >= RowCount → reset về 0)
```

#### 4.3.6. Hiển thị màn hình phụ

```
ShowFormInExtendMonitor(Form)
  ├── Screen.AllScreens.Length <= 1
  │     → Thông báo "Không tìm thấy màn hình mở rộng"
  │     → form.Show() bình thường
  └── Screen.AllScreens.Length > 1
        → FormBorderStyle = None
        → Location = màn hình phụ
        → WindowState = Maximized
        → form.Show()
```

---

### 4.4. WaitingScreenCFG - Cấu hình màu sắc/timer

**File:** `WaitingScreenCFG.cs`

Đọc cấu hình từ HisConfig (Oracle) qua `HisConfigs.Get<T>()`.

| Property | Config Key | Mô tả |
|----------|-----------|-------|
| TIMER_FOR_AUTO_LOAD_WAITING_SCREENS | EXE.WAITING_SCREEN.TIMER_FOR_AUTO_LOAD_PATIENTS | Thời gian tải lại mặc định (giây) |
| PARENT_BACK_COLOR_CODES | EXE.WAITING_SCREEN.BACKGROUND_PARENT.COLOR_CODES | Mã màu nền form (R,G,B) |
| GRID_NUM_ORDERS_BACK_COLOR_CODES | EXE.WAITING_SCREEN.BACK_COLOR_GRID_NUM_ORDER.COLOR_CODES | Mã màu nền grid |
| GRID_NUM_ORDERS_HEADER_BACK_COLOR_CODES | EXE.WAITING_SCREEN.BACK_COLOR_GRID_NUM_ORDER_HEADER.COLOR_CODES | Mã màu nền header |
| GRID_NUM_ORDERS_HEADER_FORCE_COLOR_CODES | EXE.WAITING_SCREEN.FORCE_COLOR_GRID_NUM_ORDER_HEADER.COLOR_CODES | Mã màu chữ header |

---

## 5. Design Pattern

### Factory Pattern

```
ICallPatientDepartmentV1          ← Interface (Run())
CallPatientDepartmentV1Factory    ← Factory (MakeIControl())
CallPatientDepartmentV1Behavior   ← Behavior (kế thừa BusinessBase)
  └── Run() → return new FormConfigWaitingScreen(moduleData)
```

### Module Registration Pattern

```
[ExtensionOf(typeof(DesktopRootExtensionPoint), ...)]
CallPatientDepartmentV1Processor : ModuleBase, IDesktopRoot
  └── Run(args) → Factory.MakeIControl() → behavior.Run()
```

---

## 6. Dependency

### Project References

| Project | Mục đích |
|---------|---------|
| HIS.Desktop.ApiConsumer | ApiConsumers.MosConsumer |
| HIS.Desktop.Common | BusinessBase |
| HIS.Desktop.Controls.Session | SessionManager.ProcessTokenLost |
| HIS.Desktop.LocalStorage.BackendData | BackendDataWorker |
| HIS.Desktop.LocalStorage.ConfigApplication | Config ứng dụng |
| HIS.Desktop.LocalStorage.ConfigSystem | Config hệ thống |
| HIS.Desktop.LocalStorage.HisConfig | HisConfigs.Get |
| HIS.Desktop.LocalStorage.LocalData | Dữ liệu local |
| HIS.Desktop.LocalStorage.Location | ApplicationStoreLocation |
| HIS.Desktop.Library.CacheClient | ControlStateWorker (lưu cấu hình) |

### DLL References

| DLL | Mục đích |
|-----|---------|
| DevExpress.Data.v15.2 | Data layer |
| DevExpress.Utils.v15.2 | Utility |
| DevExpress.XtraEditors.v15.2 | SpinEdit, ToggleSwitch, TextEdit, DXErrorProvider |
| DevExpress.XtraGrid.v15.2 | GridControl, GridView, GridColumn |
| DevExpress.XtraLayout.v15.2 | LayoutControl |
| MOS.EFMODEL | V_HIS_ROOM, V_HIS_EXECUTE_ROOM, L_HIS_ROOM_COUNTER_3 |
| MOS.Filter | HisRoomCounterLView3Filter |
| Inventec.Common.Adapter | BackendAdapter |
| Inventec.Core | CommonParam |

---

## 7. Khác biệt so với CallPatientDepartment (plugin gốc)

| Tiêu chí | CallPatientDepartment | CallPatientDepartmentV1 |
|-----------|----------------------|------------------------|
| Nguồn dữ liệu | HIS_SERVICE_REQ trực tiếp | L_HIS_ROOM_COUNTER_3 (view tổng hợp) |
| API | api/HisServiceReq/Get | api/HisRoom/GetCounterLView3 |
| Hiển thị | Chi tiết từng bệnh nhân (Họ, Tên, Giới tính, Tuổi, Buồng, Giường) | Tổng hợp theo phòng (8 cột thống kê) |
| Hỗ trợ phòng giường | Có (BedRoom + ExecuteRoom) | Không (chỉ ExecuteRoom) |
| Tìm kiếm phòng | Không | Có (TextEdit với filter realtime) |
| Cột Vị trí | Không | Có (SpinEdit cho phép nhập số nguyên) |
| Thời gian reload | Từ HisConfig cố định | User nhập trực tiếp trên form (SpinEdit) |
| Lưu cấu hình | Không | Có (ControlStateWorker lưu phòng, vị trí, thời gian) |
| Validation | Không | DXErrorProvider cho thời gian tải lại |

---

## 8. Điều kiện tiên quyết

1. **Database:** View `L_HIS_ROOM_COUNTER_3` phải được tạo trên Oracle với đầy đủ các trường bổ sung (CURRENT_CALL_NUMBER, NEXT_CALL_NUMBER, TOTAL_PROC_SERVICE_REQ)
2. **Backend:** API `api/HisRoom/GetCounterLView3` phải được triển khai với filter `HisRoomCounterLView3Filter`
3. **Model:** `L_HIS_ROOM_COUNTER_3` và `HisRoomCounterLView3Filter` phải được gen vào MOS.EFMODEL và MOS.Filter
4. **Modulelink:** Insert record modulelink trong database cho HIS.Desktop.Plugins.CallPatientDepartmentV1
