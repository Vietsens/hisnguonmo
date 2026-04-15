# Tài liệu phân tích thiết kế
# HIS.Desktop.Plugins.EnterKskInfomantionVer2 - Nhập thông tin Khám sức khỏe V2

---

## 1. Mục đích

Quản lý nhập thông tin khám sức khỏe (KSK) cho nhiều loại hình KSK khác nhau: KSK định kỳ, KSK trên/dưới 18 tuổi, KSK lái xe, KSK lái xe ô tô, KSK khác, KSK nghề nghiệp. Mỗi loại có bảng dữ liệu và quy trình nhập liệu riêng. Hỗ trợ nhập thông tin lâm sàng, kết luận, xếp loại sức khỏe, chọn người khám cho từng mục, quản lý tiền sử bệnh, xét nghiệm cận lâm sàng, và in phiếu KSK.

---

## 2. Cấu trúc project

```
HIS.Desktop.Plugins.EnterKskInfomantionVer2/
├── Run/
│   ├── frmEnterKskInfomantionVer2.cs              -- Form chính (khởi tạo, shared methods)
│   ├── frmEnterKskInfomantionVer2.Designer.cs     -- Thiết kế UI (DevExpress controls)
│   ├── frmEnterKskInfomantionVer2.resx
│   ├── frmEnterKskInfomantionVer2___Genaral.cs    -- Tab 1: KSK định kỳ (HIS_KSK_GENERAL)
│   ├── frmEnterKskInfomantionVer2___OverEighteen.cs -- Tab 2: KSK trên 18 tuổi
│   ├── frmEnterKskInfomantionVer2___UnderEight.cs -- Tab 3: KSK dưới 18 tuổi
│   ├── frmEnterKskInfomantionVer2___PeriodDriver.cs -- Tab 4: KSK lái xe (HIS_KSK_PERIOD_DRIVER)
│   ├── frmEnterKskInfomantionVer2___DriverCar.cs  -- Tab 5: KSK lái xe ô tô (HIS_KSK_DRIVER_CAR)
│   ├── frmEnterKskInfomantionVer2___KSKOther.cs   -- Tab 6: KSK khác
│   ├── frmEnterKsKInfomantionVer2___Occupational.cs -- Tab 7: KSK nghề nghiệp
│   ├── frmEnterKskInfomantionVer2___PrintMPS.cs   -- Xử lý in
│   └── frmEnterKskInfomantionVer2___Resource.cs   -- Xử lý resource
├── ADO/
│   ├── DiseaseTypeADO.cs       -- ADO cho bệnh lý (checkbox Có/Không)
│   ├── TypeADO.cs
│   └── VaccineTypeADO.cs
├── Config/
│   └── HisConfigCFG.cs         -- Cấu hình DisablePartExamByExecutor
├── EnterKskInfomantionVer2/
│   ├── EnterKskInfomantionVer2Behavior.cs
│   ├── EnterKskInfomantionVer2Factory.cs
│   └── IEnterKskInfomantionVer2.cs
├── Resources/
│   ├── ResourceLanguageManager.cs
│   ├── Lang.en.resx
│   └── Lang.vi.resx
├── Properties/
│   └── AssemblyInfo.cs
├── CallModule.cs
└── EnterKskInfomantionVer2Processor.cs
```

---

## 3. Đăng ký Module

**File:** `EnterKskInfomantionVer2Processor.cs`

```
Module Link  : HIS.Desktop.Plugins.EnterKskInfomantionVer2
Tên hiển thị : Thông tin khám sức khỏe Ver2
Nhóm         : Common
Category     : Common
Icon         : kham-suc-khoe.png
Loại         : MODULE_TYPE_ID__FORM
Priority     : 14
```

**Luồng khởi tạo:**
```
EnterKskInfomantionVer2Processor.Run(args)
  → EnterKskInfomantionVer2Factory.MakeIControl(param, args)
    → EnterKskInfomantionVer2Behavior.Run()
      → new frmEnterKskInfomantionVer2(moduleData, ...)
```

---

## 4. Thiết kế chi tiết

### 4.1. Bố cục form chính

Form chính sử dụng `DevExpress XtraTabControl` với 7 tab:

| Tab | Tên hiển thị | Model/Bảng DB | File xử lý | LayoutControl |
|-----|-------------|---------------|-------------|---------------|
| 1 | KSK định kỳ | HIS_KSK_GENERAL | `___Genaral.cs` | layoutControl8 |
| 2 | KSK trên 18 tuổi | — | `___OverEighteen.cs` | layoutControl4 |
| 3 | KSK dưới 18 tuổi | — | `___UnderEight.cs` | layoutControl6 |
| 4 | KSK lái xe | HIS_KSK_PERIOD_DRIVER | `___PeriodDriver.cs` | layoutControl10 |
| 5 | KSK lái xe ô tô | HIS_KSK_DRIVER_CAR | `___DriverCar.cs` | layoutControl12 |
| 6 | KSK khác | — | `___KSKOther.cs` | — |
| 7 | KSK nghề nghiệp | — | `___Occupational.cs` | layoutControl15 |

### 4.2. Cấu trúc chung mỗi tab

| Nhóm | Các control | Mô tả |
|------|-------------|-------|
| Thông tin chung | TextEdit, GridLookUpEdit | Tiền sử bệnh, thuốc đang dùng, tiền sử thai sản |
| Mục khám lâm sàng | TextEdit (kết quả) + TextEdit (kết luận) + GridLookUpEdit (xếp loại) + GridLookUpEdit (người khám) | Các chuyên khoa: mắt, TMH, tim mạch, hô hấp, v.v. |
| Xét nghiệm | TextEdit, ButtonEdit | Ma túy, nồng độ cồn, cận lâm sàng |
| Kết luận | TextEdit, DateEdit | Kết luận KSK, thời gian kết luận |
| Bệnh lý | GridControl (checkbox Có/Không) | Danh sách bệnh lý theo loại KSK |

### 4.3. Combobox Người khám (GridLookUpEdit)

**Method khởi tạo:** `SetDataCboExamLoginName()` trong `frmEnterKskInfomantionVer2.cs`

**Nguồn dữ liệu:** `V_HIS_EMPLOYEE` (chỉ nhân viên đang hoạt động `IS_ACTIVE == COMMON.IS_ACTIVE__TRUE`)

| Cột hiển thị | Field | Width | Mô tả |
|-------------|-------|-------|-------|
| Tên đăng nhập | LOGINNAME | 100 | Tên đăng nhập hệ thống |
| Họ tên | TDL_USERNAME | 150 | Họ và tên nhân viên |
| Khoa | DEPARTMENT_NAME | 150 | Khoa/phòng ban |

| Thuộc tính | Giá trị |
|-----------|---------|
| ValueMember | LOGINNAME |
| DisplayMember | TDL_USERNAME |
| NullText | "" |
| ImmediatePopup | true |
| PopupFormMinSize | 400px width |
| Buttons | Combo (mở dropdown) + Delete (xóa giá trị) |
| ButtonClick | `ClearData_ButtonClick` — xóa EditValue khi nhấn Delete |

**Giá trị lưu DB:** `LOGINNAME` (string)

### 4.4. Quy tắc đặt tên control

#### Hậu tố tab (suffix)

| Tab | Hậu tố | Ví dụ |
|-----|--------|-------|
| 1 - KSK định kỳ | (không có) | cboExamRespiratoryLoginName |
| 2 - KSK trên 18 tuổi | 2 | cboExamRespiratoryLoginName2 |
| 3 - KSK dưới 18 tuổi | 3 | cboExamRespiratoryLoginName3 |
| 4 - KSK lái xe | 4 | cboExamRespiratoryLoginName4 |
| 5 - KSK lái xe ô tô | 5 | cboExamRespiratoryLoginName5 |
| 7 - KSK nghề nghiệp | 7 | cboExamRespiratoryLoginName7 |

#### Tiền tố control

| Tiền tố | Loại control | Ví dụ |
|---------|-------------|-------|
| txt | TextEdit / ButtonEdit | txtExamMental4 |
| cbo | GridLookUpEdit | cboExamMentalRank4, cboExamMentalLoginName4 |
| spn | SpinEdit | spnExamCardiovascularBloodMax4 |
| chk | CheckEdit | chkExamEyeFieldIsNormal4 |
| dte | DateEdit | dteConclusionTimePeriodDriver |

---

## 5. Tab 4 — KSK lái xe (HIS_KSK_PERIOD_DRIVER)

### 5.1. Bố cục tab KSK lái xe

```
┌────────────────────────┬──────────────────────────────────────────────────────────────────────┐
│ layoutControl11        │ II. KHÁM LÂM SÀNG (labelControl36)                                  │
│ (Panel trái X=0-483)   ├─────────────────────────────┬──────────┬───────┬───────────────────────┤
│                        │ 1. Tâm thần: [text]         │Kết luận: │[rank▼]│Người khám: [cbo ▼ x] │
│ - Hạng GPLX            │ 2. Thần kinh: [text]        │Kết luận: │[rank▼]│Người khám: [cbo ▼ x] │
│ - Bệnh lý (GridControl)├─────────────────────────────┴──────────┴───────┴───────────────────────┤
│ - Tiền sử bệnh         │ 3. Mắt: (nhiều dòng - thị lực, sắc giác...)     Người khám: [cbo ▼ x]│
│ - Thuốc đang dùng      │ 4. TMH: (nhiều dòng - tai trái, tai phải...)     Người khám: [cbo ▼ x]│
│ - Thai sản              │ 5. Tim mạch: Mạch/Huyết áp + Kết luận           Người khám: [cbo ▼ x]│
│                        ├─────────────────────────────┬──────────┬───────┬───────────────────────┤
│                        │ 6. Hô hấp: [text]          │Kết luận: │[rank▼]│Người khám: [cbo ▼ x] │
│                        │ 7. Cơ xương khớp: [text]   │Kết luận: │[rank▼]│Người khám: [cbo ▼ x] │
│                        │ 8. Nội tiết: [text]         │Kết luận: │[rank▼]│Người khám: [cbo ▼ x] │
│                        │ 9. Thai sản: [text]         │Kết luận: │[rank▼]│Người khám: [cbo ▼ x] │
│                        ├────────────────────────────────────────────────────────────────────────┤
│                        │ III. KHÁM CẬN LÂM SÀNG                                                │
│                        │ XN ma túy: Morphin/Heroin, Amphetamin, Methamphetamin, Marijuana       │
│                        │ Nồng độ cồn, Kết quả CLS, Ghi chú CLS   Người khám: [cbo ▼ x]       │
│                        ├────────────────────────────────────────────────────────────────────────┤
│                        │ IV. KẾT LUẬN: [text]                      Ngày kết luận: [dte]        │
└────────────────────────┴────────────────────────────────────────────────────────────────────────┘
```

### 5.2. Các mục khám lâm sàng — Control mapping

| STT | Mục | Control nhập | Control kết luận | Control xếp loại | Control người khám |
|-----|-----|-------------|-----------------|-------------------|-------------------|
| 1 | Tâm thần | txtExamMental4 | txtExamMentalConclude4 | cboExamMentalRank4 | cboExamMentalLoginName4 |
| 2 | Thần kinh | txtExamNeurological4 | txtNeurologicalConclude4 | cboNeurologicalRank4 | cboExamNeurologicalLoginName4 |
| 3 | Mắt | txtExamEyeSightRight4, txtExamEyeSightLeft4, txtExamEyeDisease4... | txtExamEyeConclude4 | cboExamEyeRank4 | cboExamEyeLoginName4 |
| 4 | Tai mũi họng | txtExamEntLeftNormal4, txtExamEntRightNomal4, txtExamEntDisease4... | txtExamEntConclude4 | cboExamEntDiseaseRank4 | cboExamEntLoginName4 |
| 5 | Tim mạch | txtExamCardiovascular4, spnExamCardiovascularPulse4, spnExamCardiovascularBloodMax4, spnExamCardiovascularBloodMin4 | txtExamCardiovascularConclude4 | cboExamCardiovascularRank4 | cboExamCardiovascularLoginName4 |
| 6 | Hô hấp | txtExamRespiratory4 | txtExamRespiratoryConclude4 | cboExamRespiratoryRank4 | cboExamRespiratoryLoginName4 |
| 7 | Cơ xương khớp | txtExamMuscleBone4 | txtExamMuscleBoneConclude4 | cboExamMuscleBoneRank4 | cboExamMuscleBoneLoginName4 |
| 8 | Nội tiết | txtExamOend4 | txtExamOendConclude4 | cboExamOendRank4 | cboExamOendLoginName4 |
| 9 | Thai sản | txtExamMaternity4 | txtExamMaternityConclude4 | cboExamMaternityRank4 | cboExamMaternityLoginName4 |

### 5.3. Mapping Người khám — DB Field (HIS_KSK_PERIOD_DRIVER)

| Mục | Control | DB Field | Ghi chú |
|-----|---------|----------|---------|
| Mắt | cboExamEyeLoginName4 | EXAM_EYE_LOGINNAME | Có sẵn |
| Tai mũi họng | cboExamEntLoginName4 | EXAM_ENT_LOGINNAME | Có sẵn |
| Tim mạch | cboExamCardiovascularLoginName4 | EXAM_CARDIOVASCULAR_LOGINNAME | Có sẵn |
| Cận lâm sàng | cboExamSubclinicalLoginName4 | EXAM_SUBCLINICAL_LOGINNAME | Có sẵn |
| Hô hấp | cboExamRespiratoryLoginName4 | EXAM_RESPIRATORY_LOGINNAME | **Bổ sung mới** |
| Thần kinh | cboExamNeurologicalLoginName4 | EXAM_NEUROLOGICAL_LOGINNAME | **Bổ sung mới** |
| Cơ xương khớp | cboExamMuscleBoneLoginName4 | EXAM_MUSCLE_BONE_LOGINNAME | **Bổ sung mới** |
| Tâm thần | cboExamMentalLoginName4 | EXAM_MENTAL_LOGINNAME | **Bổ sung mới** |
| Nội tiết | cboExamOendLoginName4 | EXAM_OEND_LOGINNAME | **Bổ sung mới** |
| Thai sản | cboExamMaternityLoginName4 | EXAM_MATERNITY_LOGINNAME | **Bổ sung mới** |

### 5.4. Layout dòng mục 1, 2, 6–9 (dạng 1 dòng)

```
layoutControlGroup9 (Root, width=1745)
├─ layoutControl11 (panel trái, X=0, width=483)
└─ Vùng lâm sàng (X=483, width=1262)
   Mỗi dòng (ví dụ Tâm thần Y=22):
   ┌──────────────────────┬────────────────┬───────────┬───────────────────┐
   │ Text (638px)         │ Kết luận (292px)│ Rank (111)│ Người khám (221px)│
   │ X=483                │ X=1121          │ X=1413    │ X=1524            │
   └──────────────────────┴────────────────┴───────────┴───────────────────┘
```

### 5.5. Các mục khác

| Mục | Control | DB Field |
|-----|---------|----------|
| Hạng GPLX | cboLicenseClass4 | LICENSE_CLASS_ID, LICENSE_CLASS_NAME |
| Tiền sử bệnh gia đình | txtPathologicalHistoryFamily4 | PATHOLOGICAL_HISTORY_FAMILY |
| Tiền sử bệnh bản thân | txtPathologicalHistory4 | PATHOLOGICAL_HISTORY |
| Thuốc đang dùng | txtMedicineUsing4 | MEDICINE_USING |
| Tiền sử thai sản | txtMaternityHistory4 | MATERNITY_HISTORY |
| Morphin/Heroin | txtMorphineHeroin4, txtMorphine, txtHeroin | TEST_MORPHIN_HEROIN, TEST_MORPHIN, TEST_HEROIN |
| Methamphetamin | txtTestMethamphetamin4 | TEST_METHAMPHETAMIN |
| Amphetamin | txtTestAmphetamin4 | TEST_AMPHETAMIN |
| Marijuana | txtTestMarijuna4 | TEST_MARIJUANA |
| Nồng độ cồn | txtTestConcentration4 | TEST_CONCENTRATION |
| Kết quả CLS | txtResultSubclinical4 | RESULT_SUBCLINICAL |
| Ghi chú CLS | txtNoteSubclinical4 | NOTE_SUBCLINICAL |
| Kết luận | txtConclude4 | CONCLUDE |
| Thời gian kết luận | dteConclusionTimePeriodDriver | CONCLUSION_TIME |
| Mã CSKCB | (tự động lấy từ Branch) | HEIN_MEDI_ORG_CODE |

---

## 6. Luồng xử lý — Tab 4 KSK lái xe

### 6.1. Khởi tạo tab (FillDataPagePeriodDriver)

```
FillDataPagePeriodDriver()
  ├── ResetControlPeriodDriver()          → Reset spnBloodMax, BloodMin, Pulse về null
  ├── InitComboLicenseClass4()            → Nạp HIS_LICENSE_CLASS (IS_ACTIVE=TRUE)
  ├── SetDataCboRank() × 9               → Nạp combo xếp loại cho 9 mục
  │     (cboExamRespiratoryRank4, cboNeurologicalRank4, cboExamMuscleBoneRank4,
  │      cboExamOendRank4, cboExamMentalRank4, cboExamEyeRank4,
  │      cboExamEntDiseaseRank4, cboExamCardiovascularRank4, cboExamMaternityRank4)
  ├── SetDataCboExamLoginName() × 10     → Nạp combo người khám cho 10 mục
  │     (Eye, ENT, Cardiovascular, Subclinical,
  │      Respiratory, Neurological, MuscleBone, Mental, Oend, Maternity)
  └── FillDataUnderPeriodDriver()         → Tải dữ liệu đã lưu từ API
```

### 6.2. Tải dữ liệu đã lưu (FillDataUnderPeriodDriver)

```
FillDataUnderPeriodDriver()
  → API: BackendAdapter.Get<List<HIS_KSK_PERIOD_DRIVER>>("api/HisKskPeriodDriver/Get")
    Filter: SERVICE_REQ_ID = currentServiceReq.ID
  → Nếu có data:
    ├── Gán tất cả control từ currentKskPeriodDriver (text, conclude, rank, loginname)
    ├── Load CONCLUSION_TIME → dteConclusionTimePeriodDriver
    └── Load bệnh lý: API "api/HisPeriodDriverDity/Get" → gridControl2
  → Nếu không có data:
    ├── Lấy giá trị mặc định từ currentServiceReq (tiền sử, thị lực, thính lực)
    └── SetDefaultGrid() → Load danh sách bệnh lý mặc định (HIS_DISEASE_TYPE.IS_KSK_PERIOD_DRIVER=1)
```

### 6.3. Lưu dữ liệu (GetValuePeriodDriver)

```
GetValuePeriodDriver() → HIS_KSK_PERIOD_DRIVER
  ├── ID (nếu đang sửa)
  ├── LICENSE_CLASS_ID, LICENSE_CLASS_NAME
  ├── Tiền sử: PATHOLOGICAL_HISTORY_FAMILY, PATHOLOGICAL_HISTORY, MEDICINE_USING, MATERNITY_HISTORY
  ├── Mục khám 1–9: EXAM_XXX, EXAM_XXX_CONCLUDE, EXAM_XXX_RANK (từ text, conclude, rank controls)
  ├── Mắt chi tiết: EXAM_EYESIGHT_RIGHT/LEFT, EXAM_EYESIGHT_GLASS_RIGHT/LEFT, EXAM_TWO_EYESIGHT,
  │                  EXAM_EYEFIELD_HORI/VERT, EXAM_EYECOLOR_IS_NORMAL/BLIND/RED/GREEN/YELOW
  ├── TMH: EXAM_ENT_LEFT/RIGHT_NORMAL/WHISPER, EXAM_ENT_DISEASE
  ├── Tim mạch: EXAM_CARDIOVASCULAR_BLOOD_MAX/MIN, EXAM_CARDIOVASCULAR_PULSE
  ├── XN: TEST_MORPHIN_HEROIN, TEST_AMPHETAMIN, TEST_METHAMPHETAMIN, TEST_MARIJUANA,
  │       TEST_CONCENTRATION, TEST_MORPHIN, TEST_HEROIN
  ├── CLS: RESULT_SUBCLINICAL, NOTE_SUBCLINICAL
  ├── Kết luận: CONCLUDE, CONCLUSION_TIME
  ├── Người khám (10 mục):
  │     EXAM_EYE_LOGINNAME, EXAM_ENT_LOGINNAME, EXAM_CARDIOVASCULAR_LOGINNAME,
  │     EXAM_SUBCLINICAL_LOGINNAME, EXAM_RESPIRATORY_LOGINNAME, EXAM_NEUROLOGICAL_LOGINNAME,
  │     EXAM_MUSCLE_BONE_LOGINNAME, EXAM_MENTAL_LOGINNAME, EXAM_OEND_LOGINNAME,
  │     EXAM_MATERNITY_LOGINNAME
  └── HEIN_MEDI_ORG_CODE (tự lấy từ HIS_BRANCH)

GetDriverDity() → List<HIS_PERIOD_DRIVER_DITY>
  → Thu thập danh sách bệnh lý từ gridControl2 (DISEASE_TYPE_ID, IS_YES_NO)
```

### 6.4. Pattern Load/Save người khám

```csharp
// Load: gán giá trị từ DB vào combo
cboExamXxxLoginName4.EditValue = currentKskPeriodDriver.EXAM_XXX_LOGINNAME;

// Save: lấy giá trị từ combo ra object
obj.EXAM_XXX_LOGINNAME = cboExamXxxLoginName4.EditValue != null
    ? cboExamXxxLoginName4.EditValue.ToString() : null;
```

---

## 7. Cấu hình

| Key | Mô tả |
|-----|-------|
| `HIS.Desktop.Plugins.EnterKskInfomantionVer2.DisablePartExamByExecutor` | Nếu bật, các trường khám sẽ bị disable khi đã có người khám khác nhập liệu (ngăn ghi đè dữ liệu). Kiểm tra qua method `LoginNameEnableControl()` |

---

## 8. Phụ thuộc chính

| Thư viện | Mục đích |
|---------|---------|
| DevExpress 15.2 | UI Controls (LayoutControl, GridLookUpEdit, TextEdit, GridControl...) |
| MOS.EFMODEL | Data Models (HIS_KSK_PERIOD_DRIVER, HIS_KSK_DRIVER_CAR, V_HIS_EMPLOYEE...) |
| MOS.SDO | Service Data Objects, Filter classes |
| HIS.Desktop.LocalStorage.BackendData | Cache dữ liệu danh mục — `BackendDataWorker.Get<V_HIS_EMPLOYEE>()` |
| HIS.Desktop.ApiConsumer | Gọi API backend — `ApiConsumers.MosConsumer` |
| Inventec.Common.Controls.EditorLoader | Khởi tạo GridLookUpEdit — `ControlEditorLoader.Load()` |
| IMSys.DbConfig.HIS_RS | Constants — `COMMON.IS_ACTIVE__TRUE` |

---

## 9. API Endpoints

| Action | URI | Consumer | Filter | Ghi chú |
|--------|-----|----------|--------|---------|
| Lấy KSK lái xe | api/HisKskPeriodDriver/Get | MosConsumer | HisKskPeriodDriverFilter { SERVICE_REQ_ID } | Tab 4 |
| Lấy bệnh lý | api/HisPeriodDriverDity/Get | MosConsumer | HisPeriodDriverDityFilter { KSK_PERIOD_DRIVER_ID } | Tab 4 |
| Lấy loại bệnh | api/HisDiseaseType/Get | MosConsumer | HisDiseaseTypeFilter { IS_ACTIVE=1, IS_KSK_PERIOD_DRIVER=1 } | Tab 4 |

---

## 10. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 14/04/2026 | tuanln | Bổ sung 6 combobox Người khám cho Tab 4 KSK lái xe: Tâm thần (cboExamMentalLoginName4), Thần kinh (cboExamNeurologicalLoginName4), Hô hấp (cboExamRespiratoryLoginName4), Cơ xương khớp (cboExamMuscleBoneLoginName4), Nội tiết (cboExamOendLoginName4), Thai sản (cboExamMaternityLoginName4). Thu nhỏ Kết luận 513→292px và dịch Xếp loại từ X=1634→X=1413 cho 6 mục để chứa Người khám cùng dòng. DB fields: EXAM_RESPIRATORY_LOGINNAME, EXAM_NEUROLOGICAL_LOGINNAME, EXAM_MUSCLE_BONE_LOGINNAME, EXAM_MENTAL_LOGINNAME, EXAM_OEND_LOGINNAME, EXAM_MATERNITY_LOGINNAME |
