# Tài liệu phân tích thiết kế
# HIS.Desktop.Plugins.AssignServiceEdit - Sửa chỉ định dịch vụ

**Module:** `HIS.Desktop.Plugins.AssignServiceEdit`
**Tên hiển thị:** Sửa chỉ định dịch vụ
**Namespace:** `Inventec.Desktop.Plugins.AssignServiceEdit`
**Loại module:** Form (MODULE_TYPE_ID__FORM)
**Nhóm:** Common

---

## 1. Tổng quan

Module **AssignServiceEdit** cho phép bác sĩ/nhân viên y tế **chỉnh sửa** danh sách dịch vụ kỹ thuật đã được chỉ định trong một phiếu yêu cầu dịch vụ (`HIS_SERVICE_REQ`) của hồ sơ điều trị. Đây là module bổ trợ cho **AssignService** (tạo mới chỉ định) — thực hiện thao tác sửa/xóa/bổ sung dịch vụ trên phiếu đã có.

Các thao tác chính bao gồm:
- Thêm/bỏ dịch vụ trong phiếu yêu cầu đã tồn tại
- Chỉnh sửa số lượng, đối tượng thanh toán, phòng thực hiện, ghi chú y lệnh
- Kiểm tra tính hợp lệ của ICD - Dịch vụ trước khi lưu
- Xác nhận lưu qua API và tùy chọn in phiếu ngay sau khi lưu

---

## 2. Cấu trúc project

```
HIS.Desktop.Plugins.AssignServiceEdit/
├── ADO/
│   ├── HisSereServADO.cs             (Model dịch vụ thực hiện, mở rộng HIS_SERE_SERV)
│   └── ICDADO.cs                      (Model mã ICD, mở rộng HIS_ICD)
├── AssignServiceEdit/
│   ├── IAssignServiceEdit.cs          (Interface hành vi)
│   ├── AssignServiceEditFactory.cs    (Factory tạo behavior)
│   └── AssignServiceEditBehavior.cs   (Behavior - tạo form chính)
├── Base/
│   ├── AppConfigKeys.cs               (Hằng số các key cấu hình)
│   └── GlobalStore.cs                 (Mapping toàn cục, dữ liệu cache)
├── Config/
│   ├── HisConfigCFG.cs                (Tải và lưu các cấu hình HIS)
│   └── HisPatientTypeCFG.cs           (Cache đối tượng thanh toán)
├── ChooseICD/
│   ├── frmChooseICD.cs                (Dialog chọn mã ICD)
│   └── frmChooseICD.Designer.cs
├── Resources/
│   ├── ResourceLanguageManager.cs     (Quản lý resource đa ngôn ngữ)
│   ├── Lang.en.resx                   (Nhãn tiếng Anh)
│   ├── Lang.vi.resx                   (Nhãn tiếng Việt)
│   ├── Message.Lang.en.resx           (Thông báo tiếng Anh)
│   └── Message.Lang.vi.resx           (Thông báo tiếng Việt)
├── Properties/
│   └── AssemblyInfo.cs
├── AssignServiceEditProcessor.cs      (Entry point - đăng ký plugin)
├── FormAssignServiceEdit.cs           (Form chính ~3387 dòng)
├── FormAssignServiceEdit.Designer.cs
├── FormAssignServiceEdit.resx
├── frmWaringConfigIcdService.cs       (Dialog cảnh báo ICD-Service)
├── frmWaringConfigIcdService.Designer.cs
├── frmWaringConfigIcdService.resx
├── ResourceMessage.cs                 (Hằng số thông báo)
└── HIS.Desktop.Plugins.AssignServiceEdit.csproj
```

---

## 3. Đăng ký Module

**File:** `AssignServiceEditProcessor.cs`

```
Module Link  : HIS.Desktop.Plugins.AssignServiceEdit
Tên hiển thị : Sửa chỉ định dịch vụ
Nhóm         : Common
Loại         : MODULE_TYPE_ID__FORM
Thuộc tính   : ExtensionOf(DesktopRootExtensionPoint)
```

**Luồng khởi tạo:**
```
AssignServiceEditProcessor.Run(object[] args)
  → AssignServiceEditFactory.MakeIControl()
    → AssignServiceEditBehavior.Run()
      → Trích xuất tham số từ ModuleData + AssignServiceEditADO
        → new FormAssignServiceEdit(...)
          → FormAssignServiceEdit.ShowDialog()
```

---

## 4. Thiết kế chi tiết

### 4.1. HisSereServADO (`ADO/HisSereServADO.cs`)

Kế thừa từ `MOS.EFMODEL.DataModels.HIS_SERE_SERV`, bổ sung các thuộc tính phục vụ hiển thị và xử lý UI.

| Property | Type | Mô tả |
|----------|------|-------|
| `IsChecked` | bool | Trạng thái checkbox chọn/bỏ chọn dịch vụ |
| `IsNotUseBhyt` | bool | Không sử dụng BHYT cho dịch vụ này |
| `ExecuteRoomId` | long? | ID phòng thực hiện (hiển thị trên grid) |
| `IsExpend` | bool | Cờ chi phí mở rộng |
| `IsAllowExpend` | bool | Cho phép mở rộng chi phí |
| `IsOutKtcFee` | bool | Ngoài định mức kỹ thuật |
| `Instruction_Note` | string | Ghi chú y lệnh |
| `IsAssignDay` | bool | Chỉ định trong ngày |
| `MIN_DURATION` | decimal? | Thời gian tối thiểu giữa 2 lần chỉ định |
| `PATIENT_TYPE_ID` | long? | Đối tượng thanh toán hiện tại |
| `BILL_PATIENT_TYPE_ID` | long? | Đối tượng thanh toán thanh toán |
| `PRIMARY_PATIENT_TYPE_ID` | long? | Đối tượng thanh toán chính |
| `PACKAGE_NAME` | string | Tên gói dịch vụ (nếu thuộc gói) |
| `IS_NOT_FIXED_SERVICE` | bool | Không phải dịch vụ cố định |
| `AssignNumOrder` | int | Số thứ tự chỉ định |
| `ErrorType` | int | Mã lỗi validation (dùng tô màu dòng lỗi) |

**Constructors:**
- `HisSereServADO()` — Khởi tạo rỗng
- `HisSereServADO(V_HIS_SERE_SERV_12 sereServ)` — Map từ view model

---

### 4.2. ICDADO (`ADO/ICDADO.cs`)

Kế thừa từ `HIS_ICD`, bổ sung thuộc tính hỗ trợ chọn lọc UI.

| Property | Type | Mô tả |
|----------|------|-------|
| `Check` | bool | Trạng thái checkbox (đã chọn hay chưa) |

---

### 4.3. FormAssignServiceEdit (`FormAssignServiceEdit.cs`)

Form chính (~3387 dòng), kế thừa `HIS.Desktop.Utility.FormBase`. Đây là nơi tập trung toàn bộ nghiệp vụ sửa chỉ định dịch vụ.

#### Các phương thức quan trọng

| Phương thức | Dòng (xấp xỉ) | Chức năng |
|-------------|---------------|-----------|
| `FormAssignServiceEdit_Load` | ~133 | Khởi tạo form: tải cấu hình, dữ liệu, thiết lập cột |
| `LoadDataSereServWithTreatment` | ~509 | Tải danh sách dịch vụ của phiếu yêu cầu |
| `PatientTypeWithPatientTypeAlter` | ~544 | Tải thông tin đối tượng thanh toán, lịch sử thay đổi |
| `FillDataToGrid` | ~587 | Đổ dữ liệu dịch vụ vào lưới |
| `SetAssignNumOrder` | ~822 | Tính số thứ tự chỉ định |
| `InitComboExecuteRoom` | ~352 | Khởi tạo combo chọn phòng thực hiện |
| `GridViewService_CustomRowCellEdit` | ~1508 | Thiết lập editor động theo từng cột/dòng |
| `SaveProcess` | ~2717 | Quy trình lưu: validate → xây dựng SDO → gọi API |
| `UpdataDataForProcess` | ~2159 | Tạo đối tượng `HisServiceReqUpdateSDO` để gửi API |
| `CheckData` | ~2349 | Kiểm tra cơ bản: phải chọn ít nhất 1 dịch vụ |
| `CheckIcdService` | ~2294 | Kiểm tra dịch vụ có được cấu hình cho mã ICD |
| `getIcdListFromUcIcd` | ~2252 | Lấy danh sách mã ICD từ form |
| `getSereServWithMinDuration` | ~2104 | Kiểm tra thời gian tối thiểu giữa 2 lần chỉ định |
| `CheckService` | ~2859 | Kiểm tra ràng buộc giới tính, tuổi |
| `CheckOverTotalPatientPrice` | ~2952 | Kiểm tra tổng chi phí không vượt số dư |
| `CreateThreadLoadDataSereServWithTreatment` | ~499 | Tải dữ liệu bất đồng bộ (thread) |

---

### 4.4. frmChooseICD (`ChooseICD/frmChooseICD.cs`)

Dialog cho phép người dùng chọn một mã ICD từ danh sách.

- Hiển thị danh sách ICD dạng lưới DevExpress
- Hỗ trợ tìm kiếm theo mã hoặc tên
- Trả về đối tượng `ICDADO` được chọn

---

### 4.5. frmWaringConfigIcdService (`frmWaringConfigIcdService.cs`)

Dialog cảnh báo khi có dịch vụ chưa được cấu hình trong bảng `HIS_ICD_SERVICE`.

- Hiển thị danh sách dịch vụ chưa được cấu hình ICD-Service
- Chế độ 2 (`HIS_ICD_SERVICE.HAS_CHECK = "2"`): Hiển thị cảnh báo, vẫn cho phép lưu sau khi xác nhận
- Chế độ 1 (`HIS_ICD_SERVICE.HAS_CHECK = "1"`): Chặn lưu, không hiển thị nút xác nhận

---

### 4.6. HisConfigCFG (`Config/HisConfigCFG.cs`)

Class tĩnh lưu trữ các giá trị cấu hình được tải khi khởi tạo module.

| Field | Nguồn | Mô tả |
|-------|-------|-------|
| `IcdServiceHasCheck` | AppConfig | Chế độ kiểm tra ICD-Service (0/1/2) |
| `IcdServiceAllowUpdate` | AppConfig | Cho phép sửa khi có lỗi ICD-Service |
| `IsSetPrimaryPatientType` | AppConfig | Sử dụng đối tượng thanh toán chính |
| `IsSereServMinDurationAlert` | AppConfig | Cảnh báo khi vi phạm MIN_DURATION |
| `IsUsingServerTime` | AppConfig | Dùng giờ máy chủ thay giờ client |
| `IsCheckDepartmentInTime` | AppConfig | Kiểm tra giờ làm việc khi chỉ định |

---

### 4.7. HisPatientTypeCFG (`Config/HisPatientTypeCFG.cs`)

Cache các ID đối tượng thanh toán theo mã code, hỗ trợ truy xuất nhanh theo loại:

- BHYT, ViênPhí (VP/FEE), Dịch vụ (SERVICE), nguồn chi trả khác

---

### 4.8. GlobalStore (`Base/GlobalStore.cs`)

Lưu trữ dữ liệu tĩnh dùng chung trong phiên làm việc:

- `ServiceReqTypeToServiceTypeMapping` — Map loại phiếu yêu cầu → loại dịch vụ
- Danh sách dịch vụ tương đương (`HIS_SERVICE_SAME`) được cache từ API

---

## 5. Giao diện người dùng

### 5.1. Bố cục form chính

Form sử dụng `DevExpress LayoutControl` với các vùng chức năng:

```
┌────────────────────────────────────────────────────────────┐
│  [TOOLBAR] Lưu | Lưu & In                                  │
├────────────────────────────────────────────────────────────┤
│  Thông tin phiếu yêu cầu (mã phiếu, phòng, bác sĩ)       │
├────────────────────────────────────────────────────────────┤
│  Thông tin bệnh nhân / hồ sơ điều trị                     │
├────────────────────────────────────────────────────────────┤
│  LƯỚI DỊCH VỤ (MyGridControl)                             │
│  ┌──┬──────────┬──────────┬────┬──────────┬────────────┐  │
│  │☑ │ Mã DV   │ Tên DV  │ SL │ Đối tượng│ Phòng TH  │  │
│  ├──┼──────────┼──────────┼────┼──────────┼────────────┤  │
│  │☑ │ XN001   │ XN máu  │ 1  │ BHYT     │ P.Xét nghiệm│ │
│  │☑ │ SA001   │ Siêu âm │ 1  │ VP       │ P.Siêu âm  │  │
│  │☐ │ CT001   │ CT bụng │ 1  │ -        │ P.CDHA     │  │
│  └──┴──────────┴──────────┴────┴──────────┴────────────┘  │
├────────────────────────────────────────────────────────────┤
│  [btnSave] Lưu    [btnSaveAndPrint] Lưu & In              │
└────────────────────────────────────────────────────────────┘
```

### 5.2. Các cột lưới dịch vụ

| Cột | Loại editor | Mô tả |
|-----|-------------|-------|
| Chọn (checkbox) | CheckEdit | Đánh dấu dịch vụ cần giữ lại / xóa |
| Mã dịch vụ | TextEdit (readonly) | Mã dịch vụ |
| Tên dịch vụ | TextEdit (readonly) | Tên dịch vụ |
| Số lượng (`AMOUNT`) | SpinEdit | Số lượng dịch vụ (phải > 0) |
| Đối tượng thanh toán | ComboBoxEdit | Chọn từ danh sách đối tượng hợp lệ |
| Đối tượng chính | ComboBoxEdit | Đối tượng thanh toán chính (nếu cấu hình) |
| Phòng thực hiện | ComboBoxEdit | Chọn phòng thực hiện dịch vụ |
| Ghi chú y lệnh | TextEdit | Hướng dẫn thực hiện, ghi chú |
| Mở rộng (`IsExpend`) | CheckEdit | Đánh dấu chi phí mở rộng |
| Không dùng BHYT | CheckEdit | Bỏ qua BHYT cho dịch vụ này |
| Số thứ tự | SpinEdit | Thứ tự chỉ định |

---

## 6. Quy trình nghiệp vụ

### 6.1. Luồng chính (Main Flow)

```
┌──────────────────────────────┐
│   Mở form sửa chỉ định DV   │
│   (truyền ServiceReqId)      │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│   Tải dữ liệu:               │
│   - HIS_SERVICE_REQ          │
│   - HIS_SERE_SERV (đã có)   │
│   - Đối tượng thanh toán     │
│   - Phòng thực hiện          │
│   - Dịch vụ tương đương      │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│   Hiển thị lưới dịch vụ:    │
│   - Tích sẵn dịch vụ đã có  │
│   - Điền thông tin hiện tại  │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│   Người dùng chỉnh sửa:     │
│   - Tích/bỏ tích dịch vụ    │
│   - Sửa số lượng             │
│   - Đổi đối tượng TT        │
│   - Đổi phòng thực hiện     │
│   - Nhập ghi chú             │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│   Nhấn Lưu → SaveProcess()  │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│   VALIDATE:                  │
│   1. CheckData()             │
│   2. CheckPatientType()      │
│   3. CheckAmount()           │
│   4. CheckService()          │
│   5. CheckIcdService()       │
│   6. getSereServMinDuration()│
│   7. CheckOverTotalPrice()   │
└──────────────┬───────────────┘
               │ Hợp lệ
               ▼
┌──────────────────────────────┐
│ UpdataDataForProcess()       │
│ Tạo HisServiceReqUpdateSDO: │
│ - ServiceReq (header)        │
│ - Add: dịch vụ mới tích     │
│ - Update: dịch vụ đã có     │
│ - Delete: dịch vụ bỏ tích   │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│ POST api/HisServiceReq/Update│
└──────────────┬───────────────┘
               │ Thành công
               ▼
┌──────────────────────────────┐
│ - Refresh dữ liệu cha        │
│ - Tùy chọn: In phiếu        │
│ - Đóng form                  │
└──────────────────────────────┘
```

### 6.2. Phân loại dịch vụ khi lưu

Khi xây dựng SDO gửi API, các dịch vụ được phân loại:

| Loại | Điều kiện | Xử lý |
|------|-----------|-------|
| **INSERT** | Dịch vụ mới tích (`SERVICE_ROOM_TYPE = 4`) | Thêm vào `AddServices` |
| **UPDATE** | Dịch vụ đã tồn tại, có thay đổi | Thêm vào `UpdateServices` |
| **DELETE** | Dịch vụ đã có, bị bỏ tích | Thêm vào `DeleteServices` |

---

## 7. Kiểm tra & Validation

### 7.1. Kiểm tra cơ bản

| Quy tắc | Hành động khi vi phạm |
|---------|----------------------|
| Phải chọn ít nhất 1 dịch vụ | Thông báo lỗi, chặn lưu |
| Số lượng (`AMOUNT`) phải > 0 | Tô đỏ dòng vi phạm, chặn lưu |
| Tất cả dịch vụ phải có đối tượng thanh toán | Thông báo lỗi, chặn lưu |

### 7.2. Kiểm tra ICD - Dịch vụ

Dựa trên cấu hình `HIS_ICD_SERVICE.HAS_CHECK`:

| Giá trị | Hành vi |
|---------|---------|
| `"0"` hoặc không khai báo | Không kiểm tra, lưu bình thường |
| `"1"` | Kiểm tra và **chặn lưu** nếu dịch vụ chưa được cấu hình ICD-Service |
| `"2"` | Kiểm tra, **hiển thị cảnh báo** qua `frmWaringConfigIcdService`, vẫn cho lưu sau xác nhận |

Dịch vụ được kiểm tra: Trừ loại **Giá trị (G)** và **Khác (KHAC)**.

Nguồn ICD: Mã ICD chính + Mã ICD phụ (phân tách bằng `;`).

### 7.3. Kiểm tra thời gian tối thiểu (MIN_DURATION)

- Dịch vụ có `MIN_DURATION > 0` sẽ được kiểm tra lịch sử chỉ định gần nhất
- API: `api/HisSereServ/GetExceedMinDuration`
- Vi phạm: Cảnh báo hoặc tự động chuyển đối tượng thanh toán sang Viện phí (VP)

### 7.4. Kiểm tra giới tính / tuổi

| Ràng buộc | Nguồn dữ liệu | Hành động vi phạm |
|-----------|--------------|-------------------|
| `GENDER_ID` của dịch vụ ≠ giới tính bệnh nhân | HIS_SERVICE | Cảnh báo / chặn |
| `AGE_FROM` – `AGE_TO` không bao gồm tuổi bệnh nhân | HIS_SERVICE | Cảnh báo / chặn |

Tuổi được tính theo tháng (từ ngày sinh `BIRTHDAY`).

### 7.5. Kiểm tra tổng chi phí

- Tổng chi phí dịch vụ không được vượt số dư tài khoản bệnh nhân
- API: `api/HisTreatment/GetFeeView`

---

## 8. Các API sử dụng

| Endpoint | Method | Mô tả |
|----------|--------|-------|
| `api/HisServiceSame/GetView` | GET | Lấy danh sách dịch vụ tương đương (cache) |
| `api/HisSereServ/GetView1` | GET | Lấy danh sách dịch vụ của phiếu yêu cầu |
| `api/HisIcdService/Get` | GET | Lấy cấu hình ICD-Service để kiểm tra |
| `api/HisSereServ/GetExceedMinDuration` | GET | Kiểm tra vi phạm thời gian tối thiểu |
| `api/HisTreatment/GetFeeView` | GET | Lấy thông tin tài chính hồ sơ điều trị |
| `api/HisBedLog/GetView` | GET | Lấy thông tin giường (phục vụ in phiếu) |
| `api/HisServiceReq/Update` | POST | **Lưu thay đổi chỉ định dịch vụ** |

### Chi tiết API lưu

**Endpoint:** `POST api/HisServiceReq/Update`

**Request body:** `HisServiceReqUpdateSDO`
```
HisServiceReqUpdateSDO
├── ServiceReq      → Thông tin phiếu yêu cầu (HIS_SERVICE_REQ)
├── AddServices     → Danh sách dịch vụ thêm mới (List<ServiceReqDetailSDO>)
├── UpdateServices  → Danh sách dịch vụ cập nhật (List<ServiceReqDetailSDO>)
└── DeleteServices  → Danh sách dịch vụ xóa (List<ServiceReqDetailSDO>)
```

**Response:** `HisServiceReqUpdateResultSDO`

---

## 9. Cấu hình hệ thống

Các key cấu hình được định nghĩa trong `Base/AppConfigKeys.cs` và tải qua `Config/HisConfigCFG.cs`:

| Config Key | Giá trị | Mô tả |
|-----------|---------|-------|
| `HIS.HIS_ICD_SERVICE.HAS_CHECK` | `"0"` / `"1"` / `"2"` | Chế độ kiểm tra ICD-Dịch vụ |
| `HIS.HIS_ICD_SERVICE.ALLOW_UPDATE` | `"0"` / `"1"` | Cho phép sửa khi có lỗi ICD-Service |
| `MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE` | `"0"` / `"1"` | Hiển thị/dùng cột đối tượng chính |
| `HIS.Desktop.IsSereServMinDurationAlert` | `"0"` / `"1"` | Cảnh báo vi phạm thời gian tối thiểu |
| `MOS.IS_USING_SERVER_TIME` | `"0"` / `"1"` | Dùng giờ máy chủ |
| `HIS.Desktop.Plugins.IsCheckDepartmentInTimeWhenPresOrAssign` | `"0"` / `"1"` | Kiểm tra giờ làm việc khoa phòng |
| `MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT` | string | Mã đối tượng BHYT |
| `MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE` | string | Mã đối tượng Viện phí |

---

## 10. Đa ngôn ngữ

Module hỗ trợ 2 ngôn ngữ: **Tiếng Việt (vi)** và **Tiếng Anh (en)**.

| Resource file | Mô tả |
|--------------|-------|
| `Resources/Lang.vi.resx` | Nhãn giao diện tiếng Việt |
| `Resources/Lang.en.resx` | Nhãn giao diện tiếng Anh |
| `Resources/Message.Lang.vi.resx` | Thông báo tiếng Việt |
| `Resources/Message.Lang.en.resx` | Thông báo tiếng Anh |

Quản lý qua `ResourceLanguageManager.cs` và `ResourceMessage.cs`.

**Các thông báo quan trọng:**

| Key | Nội dung |
|-----|---------|
| `Plugin_AssignServiceEdit__DichVuChuaDuocCauHinhICDDichVu` | Dịch vụ chưa được cấu hình ICD-Dịch vụ |
| `Plugin_AssignServiceEdit__DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep` | Dịch vụ có thời gian chỉ định nằm trong khoảng thời gian không cho phép |
| `ChiDinhDichVu_KhongCoDoiTuongThanhToan` | Không có đối tượng thanh toán |
| `SuaChiDinhDichVu_KhongCoSoLuong` | Số lượng dịch vụ không hợp lệ |

---

## 11. Phụ thuộc

### 11.1. Project references

| Project | Vai trò |
|---------|---------|
| `HIS.Desktop.ADO` | ADO models dùng chung |
| `HIS.Desktop.ApiConsumer` | Gọi API backend |
| `HIS.Desktop.Common` | Utilities, helpers |
| `HIS.Desktop.Controls.Session` | Quản lý phiên làm việc |
| `HIS.Desktop.LibraryMessage` | Thư viện thông báo |
| `HIS.Desktop.LocalStorage.BackendData` | Cache dữ liệu từ backend |
| `HIS.Desktop.LocalStorage.ConfigApplication` | Cấu hình ứng dụng |
| `HIS.Desktop.LocalStorage.HisConfig` | Cấu hình HIS |
| `HIS.Desktop.LocalStorage.LocalData` | Dữ liệu local |
| `HIS.Desktop.LocalStorage.Location` | Thông tin vị trí (phòng/khoa) |
| `HIS.Desktop.ModuleExt` | Module extension base |
| `HIS.Desktop.Utilities` | Tiện ích chung |
| `HIS.Desktop.Plugins.Library.PrintServiceReq` | Thư viện in phiếu yêu cầu |

### 11.2. External DLLs

| DLL | Mô tả |
|-----|-------|
| `DevExpress.XtraEditors` (v15.2) | UI controls |
| `DevExpress.XtraGrid` (v15.2) | Grid control |
| `DevExpress.XtraLayout` (v15.2) | Layout manager |
| `MOS.EFMODEL` | Data entity models (HIS_SERE_SERV, HIS_SERVICE_REQ...) |
| `MOS.Filter` | Filter objects cho API |
| `MOS.SDO` | Service Data Objects (HisServiceReqUpdateSDO...) |
| `Inventec.Common` | Logging, DateTime, Adapter, Mapper |
| `Inventec.Desktop` | Common controls, message dialogs |
| `IMSys.DbConfig.HIS_RS` | Hằng số hệ thống (SERVICE_REQ_TYPE, PATIENT_TYPE_CODE...) |

---

## 12. Các model dữ liệu liên quan

| Model | Bảng/View | Mô tả |
|-------|-----------|-------|
| `HIS_SERVICE_REQ` | his_service_req | Phiếu yêu cầu dịch vụ (header) |
| `HIS_SERE_SERV` | his_sere_serv | Dịch vụ thực hiện (chi tiết) |
| `HIS_PATIENT_TYPE` | his_patient_type | Đối tượng thanh toán |
| `HIS_PATIENT_TYPE_ALTER` | his_patient_type_alter | Lịch sử thay đổi đối tượng TT |
| `HIS_ICD` | his_icd | Mã bệnh ICD |
| `HIS_ICD_SERVICE` | his_icd_service | Cấu hình ICD - Dịch vụ |
| `HIS_SERVICE` | his_service | Danh mục dịch vụ |
| `HIS_SERVICE_SAME` | his_service_same | Dịch vụ tương đương |
| `V_HIS_SERE_SERV_12` | View | View dịch vụ thực hiện mở rộng |
| `V_HIS_ROOM` | View | Thông tin phòng |
| `V_HIS_TREATMENT_FEE` | View | Tổng hợp chi phí điều trị |
