# Tài liệu phân tích thiết kế
# HIS.Desktop.Plugins.HisExecuteRoom - Danh mục phòng xử lý

---

## 1. Mục đích

Quản lý danh mục phòng xử lý (HIS_EXECUTE_ROOM) trong hệ thống HIS. Cho phép thêm mới, chỉnh sửa, khóa/mở khóa phòng; cấu hình đầy đủ các thuộc tính nghiệp vụ như thiết lập chi tiết, phòng chạy thận, phân chia ưu tiên, giới hạn chỉ định và tích hợp thanh toán QR.

---

## 2. Cấu trúc project

```
HIS.Desktop.Plugins.HisExecuteRoom/
├── ADO/
│   ├── BankInfo.cs
│   ├── ConfigADO.cs
│   ├── ConfigSettingsADO.cs
│   └── DirectorADO.cs
├── Base/
│   └── GlobalStore.cs
├── HisExecuteRoom/
│   ├── IHisExecuteRoom.cs
│   ├── HisExecuteRoomFactory.cs
│   ├── HisExecuteRoomBehavior.cs
│   ├── frmHisExecuteRoom.cs
│   ├── frmHisExecuteRoom.Designer.cs
│   ├── frmHisExecuteRoom.resx
│   ├── frmHisExecuteRoom___Process.cs
│   ├── frmHisExecuteRoom___Selection_Base.cs
│   └── frmHisExecuteRoom___Selection_RoomConfigOption.cs
├── RoomConfigOption/
│   └── RoomConfigOption.cs
├── Resources/
│   ├── ResourceLanguageManager.cs
│   ├── Lang.en.resx
│   └── Lang.vi.resx
├── ToolTipService/
│   └── ToolTipService.cs
├── Properties/
│   └── AssemblyInfo.cs
├── HisExecuteRoomProcessor.cs
├── HisRequestUriStore.cs
├── ModuleLinkString.cs
├── Validate2.cs
├── ValidMaxlengthtxtExecuteRoomCode.cs
├── ValidMaxLengthtxtPayerAccount.cs
├── ValidationtxtPayerAccount.cs
└── HIS.Desktop.Plugins.HisExecuteRoom.csproj
```

---

## 3. Đăng ký Module

**File:** `HisExecuteRoomProcessor.cs`

```
Module Link  : HIS.Desktop.Plugins.HisExecuteRoom
Tên hiển thị : (từ moduleData.text)
Nhóm         : Danh mục
Category     : Bussiness
Icon         : showproduct_32x32.png
Loại         : MODULE_TYPE_ID__COMBO
```

**Luồng khởi tạo:**
```
HisExecuteRoomProcessor.Run(args)
  → HisExecuteRoomFactory.MakeIControl(param, args)
    → HisExecuteRoomBehavior.Run()
      → new frmHisExecuteRoom(moduleData)
```

---

## 4. Thiết kế chi tiết

### 4.1. ADO Classes

#### 4.1.1. DirectorADO (`ADO/DirectorADO.cs`)

Đại diện bác sĩ ký thay giám đốc, tạo từ `ACS_USER`.

| Property | Type | Mô tả |
|----------|------|-------|
| LOGINNAME | string | Tên đăng nhập |
| USERNAME | string | Tên hiển thị |

#### 4.1.2. ConfigADO (`ADO/ConfigADO.cs`)

Đại diện mục cấu hình QR, dùng cho combobox chọn loại cấu hình thanh toán.

| Property | Type | Mô tả |
|----------|------|-------|
| ID_CONFIG | string | Mã định danh config (lấy từ KEY của HIS_CONFIG) |

#### 4.1.3. ConfigSettingsADO (`ADO/ConfigSettingsADO.cs`)

| Property | Type | Mô tả |
|----------|------|-------|
| NAME | string | Tên cài đặt |
| ID_CONFIG | string | Mã config |
| VALUE_CONFIG | string | Giá trị |
| IS_VALUE | bool | Có phải giá trị hay không |

#### 4.1.4. BankInfo (`ADO/BankInfo.cs`)

| Property | Type | Mô tả |
|----------|------|-------|
| BANK | string | Tên ngân hàng |
| VALUE | string | Giá trị định danh |

---

### 4.2. RoomConfigOption (`RoomConfigOption/RoomConfigOption.cs`)

Định nghĩa danh sách tùy chọn thiết lập chi tiết cho phòng. Hiển thị qua combobox `cboDepartment` (GridLookUpEdit multi-select).

#### 4.2.1. Enum Option

| Enum Value | Mô tả | DB Field | Ghi chú |
|-----------|-------|----------|---------|
| IsEmergency | Phòng cấp cứu | IS_EMERGENCY | |
| IsExam | Là phòng khám | IS_EXAM | |
| IsAutoExpendAddExam | Mặc định hao phí khi khám thêm | IS_AUTO_EXPEND_ADD_EXAM | |
| IsSpeciality | Phòng chuyên khoa | IS_SPECIALITY | |
| IsAllowNoICD | Không nhập ICD | IS_ALLOW_NO_ICD | |
| IsUseKiosk | Là phòng Kiosk | IS_USE_KIOSK | |
| IsPause | Tạm dừng | IS_PAUSE | |
| IsRestrictExecuteRoom | Giới hạn chỉ định phòng thực hiện | IS_RESTRICT_EXECUTE_ROOM | Bắt buộc thiết lập phòng yêu cầu |
| IsRestrictMedicineType | Giới hạn sử dụng thuốc | IS_RESTRICT_MEDICINE_TYPE | Bắt buộc cấu hình thuốc được phép |
| IsRestrictTime | Giới hạn thời gian hoạt động | IS_RESTRICT_TIME | Bắt buộc cấu hình thời gian hoạt |
| IsPauseEnclitic | Tạm dừng chỉ định | IS_PAUSE_ENCLITIC | Không cho chỉ định dịch vụ tới phòng này |
| IsVaccine | Là phòng tiêm chủng | IS_VACCINE | |
| IsVitaminA | Phòng uống Vitamin A | IS_VITAMIN_A | |
| IsRestrictReqService | Giới hạn yêu cầu, thực hiện dịch vụ | IS_RESTRICT_REQ_SERVICE | Bắt buộc thiết lập dịch vụ - phòng |
| IsRestrictPatientType | Giới hạn đối tượng bệnh nhân | IS_RESTRICT_PATIENT_TYPE | Bắt buộc thiết lập Phòng xử lý - đối tượng BN |
| AllowNotChooseService | Không cần chọn dịch vụ | ALLOW_NOT_CHOOSE_SERVICE | |
| IsBlockNumOrder | Cấp số thứ tự theo khung giờ khám | IS_BLOCK_NUM_ORDER | Chỉ enable khi đã chọn IsExam |
| IsSurgery | Là phòng mổ | IS_SURGERY | |
| MustBeApprovedSurgery | Phải duyệt mổ | MUST_BE_APPROVED_SURGERY | Chỉ enable khi đã chọn IsSurgery |

#### 4.2.2. RoomOptionItem

Wrapper object để hiển thị trong GridLookUpEdit.

| Property | Type | Mô tả |
|----------|------|-------|
| Code | string | Tên enum (option.ToString()) |
| Name | string | Mô tả hiển thị (từ DescriptionAttribute) |
| ToolTip | string | Ghi chú cảnh báo (từ ToolTipOptionAttribute) |
| Option | Option | Enum value |

#### 4.2.3. Extension methods

| Method | Mô tả |
|--------|-------|
| `list.Any(option)` | Kiểm tra option có trong danh sách → trả về `(short)1` hoặc `null` |
| `list.Add(option, value)` | Thêm option vào list nếu value = 1 |

---

### 4.3. frmHisExecuteRoom - Form chính

**Files:**
- `frmHisExecuteRoom.cs` - Logic chính
- `frmHisExecuteRoom.Designer.cs` - Layout UI
- `frmHisExecuteRoom___Process.cs` - Xây dựng object lưu
- `frmHisExecuteRoom___Selection_Base.cs` - Khởi tạo cboDepartment và logic base
- `frmHisExecuteRoom___Selection_RoomConfigOption.cs` - Xử lý selection cboDepartment

#### 4.3.1. Bố cục tổng quan

```
┌──────────────────────────────────────────────────────────────────────┐
│ [BarManager] Tìm kiếm | Sửa | Thêm | Reset | Về đầu trang            │
├───────────────────────────────┬──────────────────────────────────────┤
│ [txtKeyword] Tìm kiếm...      │                                      │
│ [btnSearch] [btnImport]       │                                      │
├───────────────────────────────┤    [lcEditorInfo]                    │
│ [gridControlFormList]         │    Panel thiết lập chi tiết          │
│ Danh sách phòng (phân trang)  │    (xem mục 4.3.2)                   │
│                               │                                      │
│ [ucPaging] Phân trang         │  [btnAdd] [btnEdit] [btnCancel]      │
└───────────────────────────────┴──────────────────────────────────────┘
```

#### 4.3.2. Panel lcEditorInfo - Các field nhập liệu

| Control | Loại | Label | DB Field / Ghi chú |
|---------|------|-------|--------------------|
| txtExecuteRoomCode | TextEdit | Mã phòng | EXECUTE_ROOM_CODE (bắt buộc, max 20 ký tự) |
| txtExecuteRoomName | TextEdit | Tên phòng | EXECUTE_ROOM_NAME |
| lkRoomId | GridLookUpEdit | Khoa | DEPARTMENT_ID (HIS_DEPARTMENT) |
| cbbRoomGroup | GridLookUpEdit | Nhóm phòng | ROOM_GROUP_ID (HIS_ROOM_GROUP) |
| txtOrderIssueCode | TextEdit | Order issue code | ORDER_ISSUE_CODE |
| spSTT | SpinEdit | Số thứ tự | NUM_ORDER |
| txtTestTypeCode | TextEdit | Mã loại XN | TEST_TYPE_CODE (SH, VS, HH... tích hợp Labsoft) |
| spMaxRequestByDay | SpinEdit | Số yêu cầu/ngày | MAX_REQUEST_BY_DAY |
| spMaxReqBhytByDay | SpinEdit | Số yêu cầu BHYT/ngày | MAX_REQ_BHYT_BY_DAY |
| spinMaxAppointment | SpinEdit | SL hẹn khám | MAX_APPOINTMENT_BY_DAY |
| spMaxPatientByDay | SpinEdit | Số BN/ngày | MAX_PATIENT_BY_DAY |
| spAVERAGE_ETA | SpinEdit | Thời gian TB (phút) | AVERAGE_ETA |
| spHoldOrder | SpinEdit | Giữ số | HOLD_ORDER |
| cboDepartment | GridLookUpEdit | Thiết lập chi tiết | Multi-select từ RoomConfigOption.Option (xem mục 4.2) |
| chkIsKidney | CheckEdit | Phòng chạy thận | IS_KIDNEY (1=checked, null=unchecked) |
| spinKidneyCount | SpinEdit | Số ca chạy thận/ngày | KIDNEY_SHIFT_COUNT (enable khi chkIsKidney checked) |
| chkIsSplitByPriority | CheckEdit | Tách dãy ưu tiên | IS_SPLIT_BY_PRIORITY (**logic ngược**: checked→null, unchecked→1) |
| cboDefaultDrug | GridLookUpEdit | Nhà thuốc mặc định | DEFAULT_DRUG_STORE_IDS (multi-select HIS_MEDI_STOCK IS_BUSINESS=1) |
| CboResponsible | GridLookUpEdit | Bác sĩ | RESPONSIBLE_LOGINNAME (ACS_USER IS_ACTIVE=1) |
| cboChuyenKhoa | GridLookUpEdit | Chuyên khoa | SPECIALITY_ID (HIS_SPECIALITY IS_ACTIVE=1) |
| txtAddress | TextEdit | Địa chỉ | ADDRESS |
| cboArea | GridLookUpEdit | Khu vực | AREA_ID (HIS_AREA, filter theo DEPARTMENT_ID hoặc null) |
| cboWaitingScreen | GridLookUpEdit | Màn hình chờ | SCREEN_SAVER_MODULE_LINK (ACS_MODULE nhóm MHC) |
| cboCashRoom | GridLookUpEdit | Phòng thu ngân | DEFAULT_CASHIER_ROOM_ID (V_HIS_CASHIER_ROOM IS_ACTIVE=1, filter BRANCH_ID) |
| cboDepositBook | GridLookUpEdit | Sổ tạm ứng | DEPOSIT_ACCOUNT_BOOK_ID (HIS_ACCOUNT_BOOK IS_ACTIVE=1, FOR_DEPOSIT=true) |
| cboAccountBook | GridLookUpEdit | Sổ thanh toán | BILL_ACCOUNT_BOOK_ID (HIS_ACCOUNT_BOOK IS_ACTIVE=1, FOR_BILL=true) |
| cboDefaultService | GridLookUpEdit | DV khám mặc định | DEFAULT_SERVICE_ID (HIS_SERVICE theo phòng) |
| cboDefaultsCLS | GridLookUpEdit | Mặc định chỉ định CLS | DEFAULT_INSTR_PATIENT_TYPE_ID (HIS_PATIENT_TYPE IS_ACTIVE=1) |
| txtHein_card_number | TextEdit | Mã BHYT | BHYT_CODE (HIS_ROOM) |
| cboPayerBank | GridLookUpEdit | Ngân hàng | PAYER_BANK_ID (HIS_BANK IS_ACTIVE=1) |
| txtPayerAccount | TextEdit | Tài khoản | PAYER_ACCOUNT |
| cboAccountQr | GridLookUpEdit | Sổ biên lai QR | QR_ACCOUNT_BOOK_ID (HIS_ACCOUNT_BOOK FOR_DEPOSIT=true, FOR_BILL=true, IS_NOT_GEN_TRANSACTION_ORDER≠1) |
| txtJsonQr | TextEdit | JSON QR | QR_CONFIG_JSON |
| txtDirectorLoginName | TextEdit | Ký thay (login) | HOSP_SUBS_DIRECTOR_LOGINNAME |
| cboDirectorUserName | GridLookUpEdit | Ký thay (tên) | HOSP_SUBS_DIRECTOR_LOGINNAME |

##### Bố cục khối thanh toán QR (Y=528–650)

```
lcEditorInfo (tổng width=439px)
┌──────────────────────────────────────────────────────────────────┐
│ Sổ QR:              │ [cboAccountQr                          ▼]  │ Y=528
├─────────────────────┼──────────────────────────────────────┬─────┤
│ Thiết lập TT QR:    │ [txtJsonQr              (readonly)   ]│[▲] │ Y=552
├─────────────────────┴──────────────────────────────────────┴─────┤
│ Ngân hàng chi trả:  │ [cboPayerBank                          ▼]  │ Y=578
├─────────────────────┼──────────────────────────────────────────┤
│ Tài khoản chi trả:  │ [txtPayerAccount                         ] │ Y=602
├─────────────────────┼────────────────────┬──────────────────────┤
│ Ký thay giám đốc:   │ [txtDirectorLogin  ]│ [cboDirectorUserName]│ Y=626
└─────────────────────┴────────────────────┴──────────────────────┘
  ←── label 130px ───→←──── 115px ─────→←──── 181px ───────────→
```

> Hàng Y=552: `txtJsonQr` (width 272, readonly ButtonEdit) + `btnJsonQr` (icon, 28px bên phải).
> `layoutControlItem67` tại X=226, Y=578 (w=226px): ô rỗng bên phải hàng Ngân hàng (dự phòng hiển thị thêm).
> Hàng Y=626: `layoutControlItem66` (Ký thay giám đốc, w=254px, chứa `txtDirectorLoginName` 115px) + `cboDirector` (w=185px, chứa `cboDirectorUserName` 181px) – hai ô ghép liền nhau.

##### Bố cục khối chạy thận / ưu tiên (Y=650–746)

```
lcEditorInfo (tổng width=439px)
┌──────────────────────────────────────────────────────────────────┐
│ Thiết lập chi tiết: │ [cboDepartment (multi-select)          ▼]  │ Y=650
├──────────────────┬──┼──────────────────────────────────────────┤
│ Chạy thận:  [✓] │  │ Số ca chạy thận/ngày: [spinKidneyCount ] │ Y=674
├──────────────────┴──┴──────────────────────────────────────────┤
│ DV khám mặc định:   │ [cboDefaultService                     ▼] │ Y=698
├─────────────────────┼──────────────────────────────────────────┤
│ Tách dãy ưu tiên:   │ [chkIsSplitByPriority]                    │ Y=722
├─────────────────────┴──────────────────────────────────────────┤
│                  (emptySpaceItem2 – 55px)                        │ Y=746
└──────────────────────────────────────────────────────────────────┘
  ←── lciIsKidney (158px): label 130 + chk ──→←── lciKidneyCount (281px): label 134 + spin ─→
  (hàng Y=674 chia 2 ô ngang liền kề)
```

> `chkIsSplitByPriority` (lciIsSplitByPriority, full width 439px, Y=722): logic lưu **ngược** (xem mục 8.1).
> `spinKidneyCount` bị disable khi `chkIsKidney` không được tích (xem mục 8.2).

#### 4.3.3. Danh sách cột grid (gridControlFormList)

| FieldName | Caption | Ghi chú |
|-----------|---------|---------|
| STT | STT | Tính theo vị trí + trang |
| EXECUTE_ROOM_CODE | Mã phòng | |
| EXECUTE_ROOM_NAME | Tên phòng | |
| grdColRoomId | Khoa | DEPARTMENT_NAME |
| grdCoRoomGroup | Nhóm phòng | |
| IS_ACTIVE_ST | Trạng thái | "Hoạt động" (xanh) / "Tạm khóa" (đỏ) |
| IS_EMERGENCY_STR | Cấp cứu | Icon nếu =1 |
| IS_SPECIALITY_STR | Chuyên khoa | Icon nếu =1 |
| IS_SURGERY_STR | Phòng mổ | Icon nếu =1 |
| IS_EXAM_STR | Phòng khám | Icon nếu =1 |
| IS_USE_KIOSK_STR | Kiosk | Icon nếu =1 |
| IS_VITAMIN_A_Str | Vitamin A | Icon nếu =1 |
| IS_VACCINE_Str | Tiêm chủng | Icon nếu =1 |
| IS_PAUSE_STR | Tạm dừng | Checkbox |
| IS_PAUSE_ENCLITIC_DISPLAY | Tạm dừng chỉ định | Checkbox |
| IS_ALLOW_NO_ICD_STR | Không ICD | Checkbox |
| IS_KIDNEY | Chạy thận | Icon nếu =1 |
| grdColMaxRequestByDay | SL YC/ngày | |
| CREATE_TIME_STR | Ngày tạo | Format từ TimeNumber |
| MODIFY_TIME_STR | Ngày sửa | Format từ TimeNumber |
| grdColCreator | Người tạo | |
| grdColModifier | Người sửa | |
| RestrictTime | TG hoạt động | Button: luôn enable |
| RestrictMedicineType | Thuốc | Button: enable khi IS_RESTRICT_MEDICINE_TYPE=1 |
| RestrictExecuteRoom | Phòng chỉ định | Button: enable khi IS_RESTRICT_EXECUTE_ROOM=1 |
| PatientTypeRoom | Đối tượng BN | Button: enable khi IS_RESTRICT_PATIENT_TYPE=1 |
| ServiceRoom | Dịch vụ - phòng | Button: enable khi IS_RESTRICT_REQ_SERVICE=1 |
| Lock | Khóa/Mở | Button lock/unlock theo IS_ACTIVE |
| DELETE | Xóa/Sửa nhanh | Button |
| SIGN_FOR_DIRECTOR | Ký thay GĐ | LOGINNAME - USERNAME |
| PAYER_ACCOUNT_STR | Tài khoản | Lấy từ V_HIS_ROOM |
| RESPONSIBLE_NAME | Bác sĩ | USERNAME(LOGINNAME) từ V_HIS_ROOM |

---

### 4.4. Luồng xử lý chính

#### 4.4.1. Load form (MeShow)

```
MeShow()
  ├── loadDataArea()               → Load HIS_AREA (dùng cho combo khu vực)
  ├── SetDefaultValue()            → Reset tất cả control về giá trị mặc định
  ├── EnableControlChanged()       → Set enable/disable theo ActionType (Add/Edit)
  ├── FillDataToControlsForm()     → Khởi tạo tất cả combobox lookup
  │     ├── InitComboDepartment()        → Khoa (HIS_DEPARTMENT)
  │     ├── InitComboRoomGroup()         → Nhóm phòng (api/HisRoomGroup/Get)
  │     ├── InitComboSpeciality()        → Chuyên khoa (api/HisSpeciality/Get IS_ACTIVE=1)
  │     ├── InitComboUser()              → Bác sĩ (ACS_USER IS_ACTIVE=1)
  │     ├── InitComboDefaultDrug()       → Nhà thuốc (HIS_MEDI_STOCK IS_BUSINESS=1, multi-select)
  │     ├── InitComboCashRoom()          → Thu ngân (api/HisCashierRoom/GetView)
  │     ├── InitComboArea()              → Khu vực (HIS_AREA)
  │     ├── InitComboWaitingScreen()     → Màn hình chờ (ACS_MODULE nhóm MHC)
  │     ├── InitComboDepositBook()       → Sổ tạm ứng
  │     ├── InitComboAccountBook()       → Sổ thanh toán
  │     ├── InitComboAccountBookQr()     → Sổ biên lai QR (FOR_DEPOSIT + FOR_BILL, loại IS_NOT_GEN_TRANSACTION_ORDER=1)
  │     ├── InitComboDefaultService()    → DV khám mặc định (theo roomId)
  │     ├── InitComboDefaultsCLS()       → Đối tượng CLS (api/HisPatientType/Get IS_ACTIVE=1)
  │     └── LoadResQrInfo()              → Cấu hình QR từ HIS_CONFIG (key bắt đầu PaymentQrCode)
  ├── FillDataToGridControl()      → Load danh sách phòng (phân trang)
  ├── SetCaptionByLanguageKey()    → Áp ngôn ngữ cho các label
  ├── ValidateForm()               → Đăng ký validation rules
  ├── SetDefaultFocus()            → Focus txtKeyword
  ├── LoadComboDirector()          → Combo bác sĩ ký thay
  ├── InitComboPayerBank()         → Ngân hàng (api/HisBank/Get IS_ACTIVE=1)
  └── InitComboDepartment2()       → cboDepartment multi-select (RoomConfigOption) + ToolTipService
```

#### 4.4.2. Tìm kiếm và hiển thị danh sách

```
btnSearch_Click / txtKeyword KeyUp(Enter)
  → FillDataToGridControl()
    → LoadPaging(param)
      → HisExecuteRoomFilter { KEY_WORD, ORDER_FIELD="MODIFY_TIME", ORDER_DIRECTION="DESC" }
      → BackendAdapter.GetRO<List<V_HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/GetView")
      → dnNavigation.DataSource + gridControlFormList.DataSource = data
      → ucPaging.Init() (phân trang)
```

#### 4.4.3. Chọn dòng

```
dnNavigation_PositionChanged / gridControlFormList_DoubleClick / gridviewFormList KeyDown(Enter)
  → ChangedDataRow(data)
    → FillDataToEditorControl(data)
      ├── Load tất cả field về control (xem mục 4.3.2)
      ├── IS_KIDNEY → chkIsKidney + spinKidneyCount
      ├── IS_SPLIT_BY_PRIORITY → chkIsSplitByPriority
      ├── DEFAULT_DRUG_STORE_IDS → cboDefaultDrug multi-select
      ├── Build selectedOptions từ tất cả Option flags
      ├── Load V_HIS_ROOM → cboCashRoom, CboResponsible, cboWaitingScreen, cboDepositBook,
      │                       cboAccountBook, cboDefaultService, cboDefaultsCLS, cboPayerBank,
      │                       cboAccountQr, txtJsonQr, txtHein_card_number, IS_BLOCK_NUM_ORDER
      └── ProcessSelectDepartment(selectedOptions) → tích các option trong cboDepartment
    → ActionType = ActionEdit
    → EnableControlChanged(ActionEdit)
    → btnEdit.Enabled = (IS_ACTIVE=1)
```

#### 4.4.4. Lưu dữ liệu (Thêm / Sửa)

```
btnAdd_Click / btnEdit_Click
  → Validate (dxValidationProvider)
  → SetDataExecuteRoom() → HIS_EXECUTE_ROOM
      ├── EXECUTE_ROOM_CODE, EXECUTE_ROOM_NAME
      ├── IS_EMERGENCY, IS_EXAM, IS_AUTO_EXPEND_ADD_EXAM, IS_SPECIALITY,
      │   IS_ALLOW_NO_ICD, IS_USE_KIOSK, IS_PAUSE, IS_RESTRICT_EXECUTE_ROOM,
      │   IS_RESTRICT_MEDICINE_TYPE, IS_RESTRICT_TIME, IS_PAUSE_ENCLITIC,
      │   IS_VACCINE, IS_VITAMIN_A, IS_RESTRICT_REQ_SERVICE, IS_RESTRICT_PATIENT_TYPE,
      │   ALLOW_NOT_CHOOSE_SERVICE, IS_SURGERY, MUST_BE_APPROVED_SURGERY
      │   (tất cả từ SelectedOptions.Any(option))
      ├── TEST_TYPE_CODE, NUM_ORDER, MAX_REQUEST_BY_DAY, MAX_PATIENT_BY_DAY,
      │   MAX_REQ_BHYT_BY_DAY, MAX_APPOINTMENT_BY_DAY, AVERAGE_ETA
      ├── IS_KIDNEY: chkIsKidney Checked → 1, ngược lại → null
      ├── KIDNEY_SHIFT_COUNT: spinKidneyCount nếu chkIsKidney checked
      └── IS_SPLIT_BY_PRIORITY: **logic ngược** chkIsSplitByPriority Checked → null, Unchecked → 1
  → SetDataRoom() → HIS_ROOM
      ├── ROOM_TYPE_ID = HIS_RS.HIS_ROOM_TYPE.ID__XL
      ├── DEPARTMENT_ID, DEFAULT_SERVICE_ID, AREA_ID, ORDER_ISSUE_CODE, ROOM_GROUP_ID
      ├── DEFAULT_CASHIER_ROOM_ID, DEFAULT_DRUG_STORE_IDS (join IDs)
      ├── IS_PAUSE, IS_USE_KIOSK, IS_RESTRICT_TIME, IS_RESTRICT_EXECUTE_ROOM,
      │   IS_RESTRICT_MEDICINE_TYPE, IS_RESTRICT_PATIENT_TYPE, IS_RESTRICT_REQ_SERVICE,
      │   IS_ALLOW_NO_ICD, IS_BLOCK_NUM_ORDER (từ SelectedOptions)
      ├── RESPONSIBLE_LOGINNAME, RESPONSIBLE_USERNAME
      ├── SCREEN_SAVER_MODULE_LINK, DEPOSIT_ACCOUNT_BOOK_ID, BILL_ACCOUNT_BOOK_ID
      ├── DEFAULT_INSTR_PATIENT_TYPE_ID, PAYER_BANK_ID, PAYER_ACCOUNT
      ├── QR_ACCOUNT_BOOK_ID, QR_CONFIG_JSON
      └── BHYT_CODE
  → API: Create → "api/HisExecuteRoom/Create"
         Update → "api/HisExecuteRoom/Update"
  → Reload danh sách
```

#### 4.4.5. Thiết lập chi tiết (cboDepartment - RoomConfigOption)

**File:** `frmHisExecuteRoom___Selection_RoomConfigOption.cs`, `frmHisExecuteRoom___Selection_Base.cs`

```
InitComboDepartment2()
  → DepartmentsDataSource = List<RoomOptionItem> từ Enum.GetValues(Option)
  → InitCombo(cboDepartment, ..., cboDepartment_MarksSelection, cboDepartment_CustomDisplayText,
              OnViewRowClick, OnViewRowStyle)
    ├── GridCheckMarksSelection (multi-select)
    ├── Cột hiển thị: "Thiết lập chi tiết" (width=475, auto-wrap)
    ├── AutoFilter row với placeholder "Từ khóa tìm kiếm ..."
    └── EnsureToolTipService() → GridBubbleToolTipService hiện tooltip cảnh báo
```

**Ràng buộc phụ thuộc:**

| Khi click | Điều kiện | Hành động |
|-----------|-----------|-----------|
| MustBeApprovedSurgery | IsSurgery chưa chọn | Không cho tick (deselect ngay) |
| IsSurgery bỏ tick | MustBeApprovedSurgery đang được chọn | Tự động bỏ tick MustBeApprovedSurgery |
| IsBlockNumOrder | IsExam chưa chọn | Không cho tick |
| IsExam bỏ tick | IsBlockNumOrder đang được chọn | Tự động bỏ tick IsBlockNumOrder |

**Style disabled:** Các option bị vô hiệu hóa hiển thị màu Gray / nền WhiteSmoke trong `OnViewRowStyle`.

---

### 4.5. Gọi sub-module (CallModule)

Từ các button trên grid, form gọi các module liên quan truyền `roomId`, `roomTypeId` và dữ liệu dòng hiện tại:

| Module Link | Khi nào hiển thị button | Mô tả |
|-------------|------------------------|-------|
| HIS.Desktop.Plugins.HisRoomTime | Cột RestrictTime (luôn enable) | Cấu hình thời gian hoạt động |
| HIS.Desktop.Plugins.MedicineTypeRoom | IS_RESTRICT_MEDICINE_TYPE=1 | Danh mục thuốc được phép |
| HIS.Desktop.Plugins.ExroRoom | IS_RESTRICT_EXECUTE_ROOM=1 | Phòng được chỉ định |
| HIS.Desktop.Plugins.PatientTypeRoom | IS_RESTRICT_PATIENT_TYPE=1 | Đối tượng bệnh nhân được phục vụ |
| HIS.Desktop.Plugins.RoomService | IS_RESTRICT_REQ_SERVICE=1 | Dịch vụ được chỉ định / thực hiện |
| HIS.Desktop.Plugins.HisImportExecuteRoom | Nút Import (toolbar) | Import danh mục phòng |

---

### 4.6. Validation

| Class | Áp dụng cho | Quy tắc |
|-------|-------------|---------|
| `ValidMaxlengthtxtExecuteRoomCode` | txtExecuteRoomCode | Bắt buộc nhập; max 20 ký tự (CountVi) |
| `ValidateSpin2` | SpinEdit | Giá trị phải > 0 (nếu đã nhập) |
| `ValidMaxLengthtxtPayerAccount` | txtPayerAccount | Validation độ dài tài khoản ngân hàng |
| `ValidationtxtPayerAccount` | txtPayerAccount | Validation định dạng tài khoản ngân hàng |

Sử dụng `dxValidationProviderEditorInfo` (DXValidationProvider) + `dxErrorProvider` hiển thị icon lỗi.

---

### 4.7. ToolTipService (`ToolTipService/ToolTipService.cs`)

`GridBubbleToolTipService` hiển thị bubble tooltip trên các dòng của cboDepartment khi hover.

- AutoPopDelay = 15000ms
- InitialDelay = 0ms
- ReshowDelay = 250ms
- Font: SystemFonts.MessageBoxFont, size 12, Regular
- IconType = Warning, AllowHtmlText = true, Title = "Lưu ý"
- Nội dung: lấy từ `RoomOptionItem.ToolTip` (ToolTipOptionAttribute trên enum)

---

### 4.8. GlobalStore (`Base/GlobalStore.cs`)

Utility tĩnh để load dữ liệu vào GridLookUpEdit với 2 cột (code + name).

```csharp
GlobalStore.LoadDataGridLookUpEdit(comboEdit, code, name, value, data)
// PopupFormSize = 300x250, ImmediatePopup = true, PopupFilterMode = Contains
```

---

## 5. Design Pattern

### Factory Pattern

```
IHisExecuteRoom                   ← Interface (Run())
HisExecuteRoomFactory             ← Factory (MakeIControl())
HisExecuteRoomBehavior            ← Behavior (kế thừa BusinessBase)
  └── Run() → return new frmHisExecuteRoom(moduleData)
```

### Partial Class Pattern

Form chính `frmHisExecuteRoom` được tách thành 5 partial class theo chức năng:

```
frmHisExecuteRoom.cs                          ← Logic chính (Load, tìm kiếm, UI events)
frmHisExecuteRoom.Designer.cs                 ← UI layout (auto-generated)
frmHisExecuteRoom___Process.cs                ← SetDataExecuteRoom(), SetDataRoom()
frmHisExecuteRoom___Selection_Base.cs         ← InitCombo, cboClearSelection, ToolTipService
frmHisExecuteRoom___Selection_RoomConfigOption.cs ← cboDepartment events và selection logic
```

### Module Registration Pattern

```
[ExtensionOf(typeof(DesktopRootExtensionPoint), ...)]
HisExecuteRoomProcessor : ModuleBase, IDesktopRoot
  └── Run(args) → Factory.MakeIControl() → behavior.Run()
```

---

## 6. API Endpoints

| Hằng số | URL | Chức năng |
|---------|-----|-----------|
| MOSV_HIS_EXECUTE_ROOM_GET | `api/HisExecuteRoom/GetView` | Lấy danh sách V_HIS_EXECUTE_ROOM (phân trang) |
| MOSV_HIS_EXECUTE_ROOM_CREATE | `api/HisExecuteRoom/Create` | Thêm mới |
| MOSV_HIS_EXECUTE_ROOM_UPDATE | `api/HisExecuteRoom/Update` | Cập nhật |
| MOSV_HIS_EXECUTE_ROOM_DELETE | `api/HisExecuteRoom/Delete` | Xóa |

Filter tìm kiếm: `HisExecuteRoomFilter` với `KEY_WORD`, `ORDER_FIELD="MODIFY_TIME"`, `ORDER_DIRECTION="DESC"`.

---

## 7. Dependency

### Project References

| Project | Mục đích |
|---------|---------|
| HIS.Desktop.ApiConsumer | ApiConsumers.MosConsumer, AcsConsumer |
| HIS.Desktop.Common | BusinessBase, GlobalVariables |
| HIS.Desktop.Controls.Session | SessionManager.ProcessTokenLost |
| HIS.Desktop.LocalStorage.BackendData | BackendDataWorker (HIS_DEPARTMENT, HIS_MEDI_STOCK, ...) |
| HIS.Desktop.LocalStorage.ConfigApplication | ConfigApplicationWorker (PAGE_SIZE) |
| HIS.Desktop.LocalStorage.LocalData | IMSys.DbConfig.HIS_RS |
| HIS.Desktop.Utility | FormBase |
| HIS.Desktop.Utilities.Extensions | Extensions |
| HIS.Desktop.LibraryMessage | MessageUtil |
| HIS.Desktop.ModuleExt | PluginInstanceBehavior.ShowModule (gọi sub-module) |

### DLL References

| DLL | Mục đích |
|-----|---------|
| DevExpress.XtraEditors.v15.2 | CheckEdit, SpinEdit, TextEdit, GridLookUpEdit, DXValidationProvider |
| DevExpress.XtraGrid.v15.2 | GridControl, GridView, GridColumn, GridCheckMarksSelection |
| DevExpress.XtraLayout.v15.2 | LayoutControl, LayoutControlItem |
| DevExpress.XtraBars.v15.2 | BarManager, BarButtonItem |
| DevExpress.Utils.v15.2 | ToolTipController |
| MOS.EFMODEL | V_HIS_EXECUTE_ROOM, HIS_EXECUTE_ROOM, HIS_ROOM, V_HIS_ROOM, HIS_AREA, HIS_DEPARTMENT, HIS_MEDI_STOCK, HIS_CONFIG, ... |
| MOS.Filter | HisExecuteRoomFilter, HisAreaFilter, HisRoomGroupFilter, HisBankFilter, ... |
| ACS.EFMODEL | ACS_USER, ACS_MODULE |
| ACS.Filter | AcsModuleFilter, AcsModuleGroupFilter |
| Inventec.Common.Adapter | BackendAdapter |
| Inventec.Common.Controls.EditorLoader | ControlEditorLoader, ControlEditorADO |
| Inventec.Core | CommonParam |
| Inventec.UC.Paging | PagingGrid, ucPaging |

---

## 8. Ghi chú kỹ thuật quan trọng

### 8.1. Logic ngược IS_SPLIT_BY_PRIORITY

`chkIsSplitByPriority` có logic lưu **ngược** so với convention:

```csharp
// Lưu (frmHisExecuteRoom___Process.cs)
if (chkIsSplitByPriority.CheckState == CheckState.Checked)
    executeRoom.IS_SPLIT_BY_PRIORITY = null;   // Checked → null
else
    executeRoom.IS_SPLIT_BY_PRIORITY = 1;       // Unchecked → 1

// Load (frmHisExecuteRoom.cs)
chkIsSplitByPriority.CheckState =
    (data.IS_SPLIT_BY_PRIORITY.HasValue && data.IS_SPLIT_BY_PRIORITY.Value == 1)
    ? CheckState.Checked : CheckState.Unchecked;  // Standard load
```

### 8.2. IS_KIDNEY kèm spinKidneyCount

Khi `chkIsKidney` thay đổi, `spinKidneyCount` được enable/disable tương ứng. Giá trị KIDNEY_SHIFT_COUNT chỉ được lưu khi chkIsKidney đang checked.

### 8.3. cboDepartment lưu qua SelectedOptions

`SelectedOptions` (List<RoomOptionItem>) là biến trạng thái trung gian. Khi load dữ liệu, các flag từ model được `Add()` vào list; khi lưu, dùng `Any(option)` để lấy giá trị 1/null cho từng field.

### 8.4. V_HIS_EXECUTE_ROOM vs HIS_ROOM

Form thao tác trên 2 entity đồng thời:
- `HIS_EXECUTE_ROOM`: thông tin phòng thực hiện (mã, tên, các flag IS_xxx của execute room)
- `HIS_ROOM`: thông tin phòng vật lý (BHYT_CODE, thu ngân, sổ sách, tài khoản ngân hàng, IS_BLOCK_NUM_ORDER...)

Khi thêm/sửa, form tạo và gửi cả 2 object. Khi hiển thị chi tiết, load thêm từ `BackendDataWorker.Get<V_HIS_ROOM>()` theo ROOM_ID.

---

## 9. So sánh với HIS.Desktop.Plugins.HisImportExecuteRoom

### 9.1. Mục đích

| Tiêu chí | HisExecuteRoom | HisImportExecuteRoom |
|----------|---------------|----------------------|
| Mục đích | Quản lý CRUD đơn lẻ từng phòng xử lý | Nhập khẩu hàng loạt từ file Excel |
| Người dùng | Admin cấu hình từng phòng | Admin import dữ liệu ban đầu / hàng loạt |
| Module type | `MODULE_TYPE_ID__COMBO` | `MODULE_TYPE_ID__FORM` |
| Icon | `showproduct_32x32.png` | `quy-tai-chinh.png` |
| Module link | `HIS.Desktop.Plugins.HisExecuteRoom` | `HIS.Desktop.Plugins.HisImportExecuteRoom` |

### 9.2. Kiến trúc form

| Tiêu chí | HisExecuteRoom | HisImportExecuteRoom |
|----------|---------------|----------------------|
| Form chính | `frmHisExecuteRoom` (COMBO – danh sách + edit) | `frmExecuteRoom` (FORM – chỉ hiển thị lưới kết quả import) |
| Partial files | 5 files (cs, Designer, Process, Selection×2) | 1 file form chính |
| ADO | `V_HIS_EXECUTE_ROOM` (view), `HIS_EXECUTE_ROOM`, `HIS_ROOM` | `ExecuteRoomImportADO extends V_HIS_EXECUTE_ROOM` (thêm cột lỗi + chuỗi hiển thị) |
| Nguồn dữ liệu | Nhập tay từ form, load combo từ DB | Đọc từ file Excel (OpenFileDialog) |

### 9.3. Model dữ liệu

**ExecuteRoomImportADO** (bổ sung so với V_HIS_EXECUTE_ROOM):

| Field | Loại | Mô tả |
|-------|------|-------|
| NUM_ORDER_STR | string | Số thứ tự dạng text (để parse từ Excel) |
| MAX_REQUEST_BY_DAY_STR | string | Số YC/ngày dạng text |
| MAX_REQ_BHYT_BY_DAY_STR | string | Số YC BHYT/ngày dạng text |
| HOLD_ORDER_STR | string | Giữ số dạng text |
| EMERGENCY | string | "1"/"" – cờ cấp cứu |
| EXAM | string | "1"/"" – cờ phòng khám |
| PAUSE | string | "1"/"" – cờ tạm dừng |
| RESTRICT_EXECUTE_ROOM | string | "1"/"" – giới hạn phòng |
| RESTRICT_MEDICINE_TYPE | string | "1"/"" – giới hạn thuốc |
| RESTRICT_TIME | string | "1"/"" – giới hạn thời gian |
| SPECIALITY | string | "1"/"" – phòng chuyên khoa |
| SURGERY | string | "1"/"" – phòng mổ |
| USE_KIOSK | string | "1"/"" – phòng Kiosk |
| ERROR | string | Chuỗi mô tả lỗi validation (hiển thị trong grid) |

### 9.4. Luồng xử lý

| Bước | HisExecuteRoom | HisImportExecuteRoom |
|------|---------------|----------------------|
| 1 | Mở form → load combo từ DB | Mở form → (grid trống) |
| 2 | Nhập / chọn dữ liệu trên form | Bấm Import → chọn file Excel → đọc từng dòng |
| 3 | Validate (dxValidationProvider) | Validate từng dòng: DEPARTMENT_CODE tồn tại, ROOM_GROUP_CODE, SPECIALITY_CODE, EXECUTE_ROOM_CODE không trùng (DB + file) |
| 4 | Gọi API Create/Update | Hiển thị kết quả trong grid, cột ERROR; disable btnSave nếu có lỗi |
| 5 | Reload danh sách | Bấm Lưu → gọi `api/Import/Create` hàng loạt |

### 9.5. API Endpoints

| Module | Endpoint | Mô tả |
|--------|---------|-------|
| HisExecuteRoom | `api/HisExecuteRoom/GetView` | Lấy danh sách view |
| HisExecuteRoom | `api/HisExecuteRoom/Create` | Thêm mới 1 phòng |
| HisExecuteRoom | `api/HisExecuteRoom/Update` | Cập nhật 1 phòng |
| HisExecuteRoom | `api/HisExecuteRoom/Delete` | Xóa |
| HisImportExecuteRoom | `api/Import/Get` | Lấy dữ liệu import |
| HisImportExecuteRoom | `api/Import/Create` | Import hàng loạt |
| HisImportExecuteRoom | `api/Import/Update` | Cập nhật import |
| HisImportExecuteRoom | `api/Import/Delete` | Xóa import |
| HisImportExecuteRoom | `api/Import/ChangeLock` | Khóa/mở khóa |

### 9.6. Validation so sánh

| Quy tắc | HisExecuteRoom | HisImportExecuteRoom |
|---------|---------------|----------------------|
| Mã phòng bắt buộc | Có (ValidMaxlengthtxtExecuteRoomCode, max 20 ký tự) | Có (kiểm tra cột trong Excel) |
| Mã phòng không trùng | API báo lỗi khi save | Kiểm tra trước khi save (DB + trong file) |
| Khoa bắt buộc | Không validate cứng | Bắt buộc, tra cứu theo DEPARTMENT_CODE |
| Nhóm phòng | Tùy chọn | Tùy chọn, tra cứu theo ROOM_GROUP_CODE |
| Chuyên khoa | Tùy chọn | Tùy chọn, tra cứu theo SPECIALITY_CODE |
| Lỗi hiển thị | Icon lỗi tại control (dxErrorProvider) | Cột ERROR trong grid, màu đỏ/vàng |
