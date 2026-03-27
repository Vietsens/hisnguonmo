# Tài liệu phân tích thiết kế
# HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood - Kiểm tra truyền máu

---

## 1. Mục đích

Chức năng kiểm tra trước khi truyền máu cho phép nhân viên y tế:

- Xem danh sách túi máu / chế phẩm máu / dịch vụ trong một phiếu lĩnh máu.
- Nhập kết quả kiểm tra hòa hợp trước truyền: ống nghiệm, môi trường muối, anti globulin, phản ứng chéo, tự chứng AC, Scangel/Gelcard, KQNP Coombs.
- **Tự động điền** kết quả MT muối và Anti globulin dựa trên kết quả **XN hòa hợp** (xét nghiệm từ hồ sơ điều trị) khi chọn túi máu, theo vị trí ống `TUBE_SLOT`.
- Lưu thông tin nhóm máu bệnh nhân và kết quả kiểm tra vào cơ sở dữ liệu.
- In phiếu kiểm tra truyền máu.

---

## 2. Cấu trúc project

```
HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood/
├── frmHisCheckBeforeTransfusionBlood.cs          (Form chính)
├── frmHisCheckBeforeTransfusionBlood.Designer.cs (Thiết kế UI)
├── frmHisCheckBeforeTransfusionBlood.resx        (Resources)
├── HisCheckBeforeTransfusionBloodProcesser.cs    (Entry point / Module registration)
├── HisRequestUriStore.cs                         (Hằng số API endpoints)
├── ADO.cs                                        (ADO dùng cho cboAC, cboAC2)
├── ADOs/
│   ├── TestHarmonyADO.cs                         (Dòng dữ liệu cho combobox XN hòa hợp)
│   └── TestIndexResultADO.cs                     (Kết quả chỉ số xét nghiệm)
├── Base/
│   ├── ComboboxADO.cs                            (Item cho LookUpEdit MT muối / Anti globulin)
│   ├── ExpBloodADO.cs                            (Dữ liệu túi máu / dịch vụ trên TreeList)
│   └── TestIndexProcessor.cs                     (Xử lý chỉ số xét nghiệm hòa hợp)
├── Config/
│   └── ConfigKey.cs                              (Đọc cấu hình HisConfig)
├── InputExpMestId/
│   ├── frmInputExpMestId.cs                      (Dialog chọn phiếu lĩnh máu)
│   ├── frmInputExpMestId.Designer.cs
│   └── frmInputExpMestId.resx
├── Resource/
│   ├── ResourceLanguageManager.cs
│   ├── Lang.en.resx
│   └── Lang.vi.resx
├── Properties/
│   └── AssemblyInfo.cs
└── HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.csproj
```

---

## 3. Đăng ký Module

**File:** `HisCheckBeforeTransfusionBloodProcesser.cs`

```
Module Link  : HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood
Tên hiển thị : Danh mục
Nhóm         : Bussiness
Icon         : showproduct_32x32.png
Loại         : MODULE_TYPE_ID__FORM
```

**Luồng khởi tạo:**

```
HisCheckBeforeTransfusionBloodProcessor.Run(args)
  ├── args không có expMestId, không có delegateSelect
  │     → new frmHisCheckBeforeTransfusionBlood(moduleData)
  │           → frmInputExpMestId.ShowDialog()  ← Chọn phiếu lĩnh máu
  ├── args có expMestId
  │     → new frmHisCheckBeforeTransfusionBlood(moduleData, expMestId)
  ├── args có delegateSelect
  │     → new frmHisCheckBeforeTransfusionBlood(moduleData, delegateSelect)
  │           → frmInputExpMestId.ShowDialog()
  └── args có cả delegateSelect và expMestId
        → new frmHisCheckBeforeTransfusionBlood(moduleData, delegateSelect, expMestId)
```

---

## 4. Thiết kế chi tiết

### 4.1. Data Models (ADO)

#### 4.1.1. ExpBloodADO (`Base/ExpBloodADO.cs`)

Đại diện cho một dòng trên TreeList (túi máu hoặc dịch vụ xét nghiệm).

| Property | Type | Mô tả |
|----------|------|-------|
| ExpMestBloodId | long? | ID bản ghi V_HIS_EXP_MEST_BLOOD |
| BloodTypeId | long | ID loại máu |
| BLOOD_CODE | string | Mã túi máu |
| SERVICE_BLOOD_CODE | string | Mã loại máu / dịch vụ |
| SERVICE_BLOOD_NAME | string | Tên loại máu / dịch vụ |
| VOLUME | decimal? | Dung tích (ml) |
| AMOUNT | long | Số lượng |
| BLOOD_CODE | string | Mã vạch túi máu |
| BLOOD_ABO_CODE | string | Nhóm máu ABO của túi |
| BLOOD_HR_CODE | string | Yếu tố Rh của túi |
| BLOOD_ABO_HR_CODE | string | Ghép ABO + Rh hiển thị |
| EXPIRED_DATE_STR | string | Hạn dùng dạng chuỗi |
| GIVE_NAME | string | Người cho |
| PATIENT_BLOOD_ABO_CODE | string | Nhóm máu ABO bệnh nhân |
| PATIENT_BLOOD_RH_CODE | string | Rh bệnh nhân |
| PUC | string | Kết quả PUC |
| SCANGEL_GELCARD | string | Kết quả Scangel/Gelcard |
| COOMBS | string | Kết quả KQNP Coombs |
| TEST_TUBE | string | Ống nghiệm 1 |
| SALT_ENVI | long? | MT muối ống 1 |
| ANTI_GLOBULIN | long? | Anti globulin ống 1 |
| TEST_TUBE_TWO | string | Ống nghiệm 2 |
| SALT_ENVI_TWO | long? | MT muối ống 2 |
| ANTI_GLOBULIN_TWO | long? | Anti globulin ống 2 |
| AC_SELF_ENVIDENCE | decimal? | Tự chứng AC |
| AC_SELF_ENVIDENCE_SECOND | decimal? | Tự chứng AC2 |
| SERVICE_RESULT | string | Kết quả dịch vụ |
| TUBE_SLOT | long? | Vị trí ống (1 hoặc 2), load từ `V_HIS_EXP_MEST_BLOOD.TUBE_SLOT` trong `BuidTreeList()` |
| is_Sevrvice_Blood | bool | true = dịch vụ; false = túi máu |
| Key | string | Khóa node TreeList |
| ParentKey | string | Khóa node cha TreeList |

#### 4.1.2. TestHarmonyADO (`ADOs/TestHarmonyADO.cs`)

Dòng dữ liệu cho combobox **XN hòa hợp** (`cboXNHH`).

| Property | Type | Mô tả |
|----------|------|-------|
| SERE_SERV_ID | long | ID dịch vụ xét nghiệm (khóa chính combobox) |
| RESULT_TIME | long? | Thời gian trả kết quả (số) |
| RESULT_TIME_STR | string (computed) | Thời gian trả kết quả dạng chuỗi |
| BLOOD_VALUE | string | Giá trị chỉ số túi máu (danh sách A) |
| SALT_VALUE | string | Giá trị chỉ số MT muối (danh sách B) |
| ANTI_GLOBULIN_VALUE | string | Giá trị chỉ số anti globulin (danh sách C) |

#### 4.1.3. TestIndexResultADO (`ADOs/TestIndexResultADO.cs`)

Kết quả một chỉ số xét nghiệm từ `V_HIS_SERE_SERV_TEIN`.

| Property | Type | Mô tả |
|----------|------|-------|
| SERE_SERV_TEIN_ID | long | ID bản ghi |
| TEST_INDEX_CODE | string | Mã chỉ số xét nghiệm |
| TEST_INDEX_NAME | string | Tên chỉ số xét nghiệm |
| VALUE | string | Kết quả |
| SERE_SERV_ID | long | ID dịch vụ xét nghiệm |
| TREATMENT_ID | long | ID hồ sơ điều trị |
| RESULT_TIME | long? | Thời gian trả kết quả (số) |
| RESULT_TIME_STR | string (computed) | Thời gian trả kết quả dạng chuỗi |

#### 4.1.4. ComboboxADO (`Base/ComboboxADO.cs`)

Item cho các LookUpEdit MT muối / Anti globulin.

| Property | Type | Giá trị mẫu |
|----------|------|-------------|
| Id | long | 5 |
| ItemName | string | "Âm tính" |

**Danh sách giá trị chuẩn:**

| Id | Tên hiển thị |
|----|-------------|
| 5 | Âm tính |
| 1 | 1+ |
| 2 | 2+ |
| 3 | 3+ |
| 4 | 4+ |
| 6 | 0.5+ |
| 7 | 5+ |

---

### 4.2. ConfigKey (`Config/ConfigKey.cs`)

| Hằng số | Giá trị |
|---------|---------|
| `Code_BloodHarmonyTestIndexConfig` | `HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.BloodHarmonyTestIndex` |
| `Code_IsNotAllowEditBloodInformation` | `HIS.Desktop.Plugins.BrowseExportTicket.IsNotAllowEditBloodInformation` |

**Định dạng cấu hình `BloodHarmonyTestIndex`:**

```
<mã_A1>|<mã_A2>;<mã_B1>|<mã_B2>;<mã_C1>|<mã_C2>
```

- Nhóm 1 (trước `;` đầu tiên): mã chỉ số **túi máu** → Danh sách A
- Nhóm 2 (giữa hai `;`): mã chỉ số **môi trường muối** → Danh sách B
- Nhóm 3 (sau `;` thứ hai): mã chỉ số **anti globulin** → Danh sách C

**Ví dụ:**
```
XN001|XN002;XN011|XN012;XN101|XN102
```

Đọc qua `HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code)`.

---

### 4.3. TestIndexProcessor (`Base/TestIndexProcessor.cs`)

Lớp xử lý trung tâm cho tính năng XN hòa hợp.

**Properties:**

| Property | Type | Mô tả |
|----------|------|-------|
| BloodTestIndexList | `List<TestIndexResultADO>` | **Danh sách A** – chỉ số xét nghiệm túi máu |
| SaltEnviTestIndexList | `List<TestIndexResultADO>` | **Danh sách B** – chỉ số MT muối |
| AntiGlobulinTestIndexList | `List<TestIndexResultADO>` | **Danh sách C** – chỉ số anti globulin |
| TestHarmonyList | `List<TestHarmonyADO>` | Danh sách tổng hợp cho `cboXNHH` |

**Methods:**

| Method | Mô tả |
|--------|-------|
| `LoadTestIndexData(long treatmentId)` | Gọi API `api/HisSereServTein/GetView` với filter `TDL_TREATMENT_ID`, sau đó phân loại vào 3 danh sách A/B/C theo config, cuối cùng gọi `BuildTestHarmonyList()` |
| `ParseConfig()` *(private)* | Tách chuỗi config theo dấu `;` và `\|` ra 3 tập mã chỉ số |
| `BuildTestHarmonyList()` *(private)* | Union SERE_SERV_ID từ 3 danh sách, nhóm thành `TestHarmonyADO`, sắp xếp giảm dần theo `RESULT_TIME` |
| `GetBloodTestIndexBySereServId(long)` | Tìm bản ghi đầu tiên trong danh sách A theo `SERE_SERV_ID` |
| `GetSaltEnviTestIndexBySereServId(long)` | Tìm bản ghi đầu tiên trong danh sách B theo `SERE_SERV_ID` |
| `GetAntiGlobulinTestIndexBySereServId(long)` | Tìm bản ghi đầu tiên trong danh sách C theo `SERE_SERV_ID` |
| `FindBloodTestIndexByBloodCode(string bloodCode)` | Tìm A1: bản ghi trong danh sách A có `VALUE == bloodCode` |

**Luồng `LoadTestIndexData`:**

```
LoadTestIndexData(treatmentId)
  ├── ParseConfig()
  │     └── Tách BloodHarmonyTestIndexConfig → bloodTestIndexCodes, saltEnviTestIndexCodes, antiGlobulinTestIndexCodes
  ├── BackendAdapter.Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView")
  │     Filter: TDL_TREATMENT_ID = treatmentId
  ├── Lọc theo codes → BloodTestIndexList (A), SaltEnviTestIndexList (B), AntiGlobulinTestIndexList (C)
  └── BuildTestHarmonyList()
        ├── Union SERE_SERV_ID từ A + B + C
        ├── Với mỗi SERE_SERV_ID: tạo TestHarmonyADO
        │     RESULT_TIME  ← bloodIndex.RESULT_TIME
        │     BLOOD_VALUE  ← bloodIndex.VALUE
        │     SALT_VALUE   ← saltIndex.VALUE
        │     ANTI_GLOBULIN_VALUE ← antiGlobulinIndex.VALUE
        └── Sắp xếp giảm dần theo RESULT_TIME
```

---

### 4.4. frmHisCheckBeforeTransfusionBlood (Form chính)

**File:** `frmHisCheckBeforeTransfusionBlood.cs`, `frmHisCheckBeforeTransfusionBlood.Designer.cs`

#### 4.4.1. Giao diện

```
┌─────────────────────────────── Kiểm tra truyền máu ─────────────────────────────────────────┐
│ ┌──────────────────────────────────────────────────────────────────────────────────────────┐ │
│ │ Máu - chế phẩm/dịch vụ        │ Dung tích │ Số lượng │ Mã vạch   │ Hạn dùng  │ Người cho │ Nhóm máu │ Kết quả │ │
│ ├────────────────────────────────┼──────────┼──────────┼───────────┼───────────┼──────────┼──────────┼─────────┤ │
│ │ ▼ máu 02                       │   250,0  │    1     │           │           │          │          │         │ │
│ │    └─ máu 02                   │   250,0  │    1     │ 565568898 │ 19/03/2026│          │    B     │         │ │
│ │    ...                         │          │          │           │           │          │          │         │ │
│ └──────────────────────────────────────────────────────────────────────────────────────────┘ │
│                                                                                              │
│  Nhóm máu BN: [cboOldAbo ▼][x] [cboOldRH ▼][x]   Theo Hồ sơ: [cboAboHoSo ▼][x] [▼][x]    │
│                                                                           ↓                  │
│  Phản ứng chéo: [____________________________________]   XN hòa hợp: [cboXNHH ▼][x]        │
│                                                                                              │
│  Ống nghiệm 1: [___]   MT muối:  [cboSaltEnvi    ▼][x]   Anti globulin:  [cboAntiGlobulin    ▼][x]   Tự chứng AC:  [cboAC  ▼][x] │
│  Ống nghiệm 2: [___]   MT muối 2:[cboSaltEnviTwo ▼][x]   Anti globulin 2:[cboAntiGlobulinTwo ▼][x]   Tự chứng AC2: [cboAC2 ▼][x] │
│                                                                                              │
│  Scangel/Gelcard: [____________________]   KQNP Coombs: [____________________]             │
│  Thời gian rã đông: [__________________]   Ghi chú:     [____________________]             │
│                                                                                              │
│                                                    [Lưu (Ctrl S)]  [In (Ctrl P)]  [▼]      │
└──────────────────────────────────────────────────────────────────────────────────────────────┘
```

#### 4.4.2. Danh sách control

| Control | Loại DevExpress | Mô tả |
|---------|----------------|-------|
| `treeListExpBlood` | TreeList | Danh sách máu/chế phẩm/dịch vụ, phân cấp theo BLOOD_TYPE_ID |
| `cboOldAbo` | GridLookUpEdit | Nhóm máu ABO hiện tại của BN (readonly, từ V_HIS_PATIENT) |
| `cboOldRH` | GridLookUpEdit | Yếu tố Rh hiện tại của BN (readonly) |
| `cboNewAbo` | GridLookUpEdit | Nhóm máu ABO mới cập nhật (from HIS_BLOOD_ABO) |
| `cboNewRH` | GridLookUpEdit | Yếu tố Rh mới cập nhật (from HIS_BLOOD_RH) |
| `cboXNHH` | GridLookUpEdit | **XN hòa hợp (MỚI)** – chọn kết quả xét nghiệm hòa hợp |
| `txtTestTube` | TextEdit | Ống nghiệm 1 |
| `cboSaltEnvi` | LookUpEdit | MT muối ống 1 (từ ComboboxADO) |
| `cboAntiGlobulin` | LookUpEdit | Anti globulin ống 1 |
| `txtTestTubeTwo` | TextEdit | Ống nghiệm 2 |
| `cboSaltEnviTwo` | LookUpEdit | MT muối ống 2 |
| `cboAntiGlobulinTwo` | LookUpEdit | Anti globulin ống 2 |
| `cboAC` | GridLookUpEdit | Tự chứng AC (từ ADO: 0=Âm tính, 0.5, 1+…5+) |
| `cboAC2` | GridLookUpEdit | Tự chứng AC2 |
| `txtPuc` | TextEdit | Phản ứng chéo (PUC) |
| `txtScangelGelcard` | TextEdit | Scangel/Gelcard |
| `txtCoombs` | TextEdit | KQNP Coombs |
| `btnAdd` | SimpleButton | Lưu (Ctrl S) |
| `btnPrint` | SimpleButton | In (Ctrl P), chỉ enable khi EXP_MEST_STT_ID = 5 |

**Cột TreeList `treeListExpBlood`:**

| Cột | FieldName | Mô tả |
|-----|-----------|-------|
| Máu - chế phẩm/dịch vụ | SERVICE_BLOOD_NAME | Tên loại máu hoặc dịch vụ |
| Dung tích | VOLUME | ml |
| Số lượng | AMOUNT | |
| Mã vạch | BLOOD_CODE | Mã túi máu |
| Hạn dùng | EXPIRED_DATE_STR | |
| Người cho | GIVE_NAME | |
| Nhóm máu | BLOOD_ABO_HR_CODE | ABO + Rh túi máu |
| Kết quả | SERVICE_RESULT | Kết quả dịch vụ xét nghiệm |

**Combobox `cboXNHH` – 4 cột:**

| FieldName | Header | Width |
|-----------|--------|-------|
| RESULT_TIME_STR | Trả | 80 |
| BLOOD_VALUE | Túi máu | 100 |
| SALT_VALUE | MT muối | 100 |
| ANTI_GLOBULIN_VALUE | Anti globulin | 100 |

DisplayMember = `RESULT_TIME_STR`, ValueMember = `SERE_SERV_ID`, ImmediatePopup = true, popup width = 400.

#### 4.4.3. Luồng khởi tạo Form

```
frmHisCheckBeforeTransfusionBlood_Load
  ├── ConfigKey.GetConfigKey()
  ├── ValidateForm()
  ├── LoadCurrentPatient()
  │     └── api/HisExpMest/GetView (filter: ID = ExpMestId)
  │     └── api/HisPatient/GetView (filter: ID = TDL_PATIENT_ID)
  │     → cboOldAbo.EditValue = BLOOD_ABO_CODE, cboOldRH.EditValue = BLOOD_RH_CODE
  ├── new TestIndexProcessor()
  ├── testIndexProcessor.LoadTestIndexData(TDL_TREATMENT_ID)
  ├── LoadComboAC(cboAC), LoadComboAC(cboAC2)
  ├── LoadDataToCombo()
  │     └── api/HisBloodAbo/Get → InitComboAbo(cboNewAbo, cboOldAbo)
  │     └── api/HisBloodRh/Get → InitComboRH(cboNewRH, cboOldRH)
  ├── LoadDataToComboboxEnvironment()
  │     → InitComboEnvi(cboSaltEnvi, cboSaltEnviTwo, cboAntiGlobulin, cboAntiGlobulinTwo)
  │     → EditValue = 0 (mặc định)
  ├── LoadComboTestHarmony()
  │     → ControlEditorLoader.Load(cboXNHH, TestHarmonyList, ...)
  └── BuidTreeList()
        └── api/HisExpMestBlood/GetView (filter: EXP_MEST_ID)
        └── api/HisExpBltyService/GetView (filter: EXP_MEST_ID)
        → Nhóm theo BLOOD_TYPE_ID → Cây ExpBloodADO
        → treeListExpBlood.DataSource = records
```

#### 4.4.4. Luồng chọn túi máu trên TreeList → FillDataToEditorControl

```
treeListExpBlood_FocusedNodeChanged / Click
  └── FillDataToEditorControl(ExpBloodADO data)
        ├── Hiển thị: cboNewAbo, cboNewRH, txtPuc, txtScangelGelcard, txtCoombs
        ├── Điền ống nghiệm 1/2: txtTestTube, cboSaltEnvi, cboAntiGlobulin, txtTestTubeTwo, cboSaltEnviTwo, cboAntiGlobulinTwo
        │     Nếu chưa có giá trị → mặc định Id=5 (Âm tính)
        ├── Điều chỉnh enable theo PREPARATIONS_BLOOD_CODE (loại chế phẩm)
        │     code 3/4/5: null salt1+anti1+salt2+anti2+AC+AC2, salt2=Âm tính
        │     code 6:     null salt1+anti1+salt2+anti2+AC+AC2, salt1=salt2=anti1=Âm tính
        │     code 1:     null salt1+anti1+salt2+anti2+AC+AC2, salt1=anti1=Âm tính
        ├── Điều chỉnh enable txtTestTube/txtTestTubeTwo theo BLOOD_GROUP (BLOOD_ERYTHROCYTE, BLOOD_PLASMA)
        └── ProcessAutoFillFromTestHarmony(data)
```

#### 4.4.5. Luồng tự điền XN hòa hợp (ProcessAutoFillFromTestHarmony)

Khi nhấn chọn một túi máu (`is_Sevrvice_Blood = false`) có `BLOOD_CODE`, hệ thống tự tìm và điền kết quả xét nghiệm hòa hợp tương ứng:

```
ProcessAutoFillFromTestHarmony(ExpBloodADO data)
  ├── Điều kiện: data != null, testIndexProcessor != null, BLOOD_CODE không rỗng
  │
  ├── tubeSlot = data.TUBE_SLOT  ← đã load sẵn từ BuidTreeList(), không gọi API thêm
  │
  ├── TUBE_SLOT khác 1 và 2 → kết thúc
  │
  ├── TUBE_SLOT = 1: shouldFill = (SALT_ENVI null HOẶC ANTI_GLOBULIN null)
  │   TUBE_SLOT = 2: shouldFill = (SALT_ENVI_TWO null HOẶC ANTI_GLOBULIN_TWO null)
  │   shouldFill = false → kết thúc
  │
  ├── Tìm A1 = testIndexProcessor.FindBloodTestIndexByBloodCode(BLOOD_CODE)
  │     = BloodTestIndexList.FirstOrDefault(o => o.VALUE == BLOOD_CODE)
  │   A1 == null → kết thúc
  │
  ├── Hiển thị thời gian trả kết quả lên cboXNHH
  │     → TestHarmonyList.FirstOrDefault(o => o.SERE_SERV_ID == A1.SERE_SERV_ID)
  │     → cboXNHH.EditValue = harmonyItem.SERE_SERV_ID
  │
  ├── saltIndex = GetSaltEnviTestIndexBySereServId(A1.SERE_SERV_ID)
  │   antiGlobulinIndex = GetAntiGlobulinTestIndexBySereServId(A1.SERE_SERV_ID)
  │
  ├── TUBE_SLOT = 1:
  │     saltIndex.VALUE → SetComboEnviValue(cboSaltEnvi, value)
  │     antiGlobulinIndex.VALUE → SetComboEnviValue(cboAntiGlobulin, value)
  │
  └── TUBE_SLOT = 2:
        saltIndex.VALUE → SetComboEnviValue(cboSaltEnviTwo, value)
        antiGlobulinIndex.VALUE → SetComboEnviValue(cboAntiGlobulinTwo, value)
```

**Điều kiện kích hoạt tự điền:**

| TUBE_SLOT | Điều kiện | Kết quả điền vào |
|-----------|-----------|-----------------|
| 1 | SALT_ENVI null **hoặc** ANTI_GLOBULIN null | `cboSaltEnvi`, `cboAntiGlobulin` |
| 2 | SALT_ENVI_TWO null **hoặc** ANTI_GLOBULIN_TWO null | `cboSaltEnviTwo`, `cboAntiGlobulinTwo` |
| khác 1, 2 | — | Không xử lý |

#### 4.4.6. Luồng chọn thủ công trên cboXNHH

Khi người dùng tự chọn một kết quả xét nghiệm từ `cboXNHH`:

```
cboXNHH_EditValueChanged
  ├── Lấy sereServId từ cboXNHH.EditValue
  ├── tubeSlot = curentSelect.TUBE_SLOT  ← lấy từ ADO, không gọi API
  │
  ├── TUBE_SLOT = 1:
  │     cboSaltEnvi.EditValue ← SALT_VALUE
  │     cboAntiGlobulin.EditValue ← ANTI_GLOBULIN_VALUE
  │
  └── TUBE_SLOT = 2:
        cboSaltEnviTwo.EditValue ← SALT_VALUE
        cboAntiGlobulinTwo.EditValue ← ANTI_GLOBULIN_VALUE
```

#### 4.4.7. Lưu dữ liệu (UpdateExpMestBlood)

```
btnAdd_Click
  └── UpdateExpMestBlood()
        → API: api/HisExpMest/UpdateTestInfo
        → Payload: ExpMestBloodDTO với danh sách ExpBloodADO đã chỉnh sửa
```

---

### 4.5. frmInputExpMestId (Dialog chọn phiếu)

**File:** `InputExpMestId/frmInputExpMestId.cs`

- Hiển thị khi mở plugin mà không truyền `expMestId` từ ngoài.
- Cho phép người dùng tìm kiếm và chọn phiếu lĩnh máu (`V_HIS_EXP_MEST`).
- Kết quả được trả về qua property `ExpMest`.

---

## 5. API Endpoints

| Hằng số / URL | Mục đích |
|--------------|---------|
| `api/HisExpMest/UpdateTestInfo` | Lưu kết quả kiểm tra (SALT_ENVI, ANTI_GLOBULIN, PUC, v.v.) |
| `api/HisExpMest/GetView` | Lấy thông tin phiếu lĩnh máu |
| `api/HisExpMestBlood/GetView` | Lấy danh sách túi máu (có TUBE_SLOT, BLOOD_CODE) |
| `api/HisExpMestBlood/Get` | Lấy chi tiết một túi máu |
| `api/HisExpBltyService/GetView` | Lấy dịch vụ xét nghiệm đính kèm phiếu lĩnh |
| `api/HisSereServTein/GetView` | Lấy chỉ số xét nghiệm theo TDL_TREATMENT_ID |
| `api/HisBloodAbo/Get` | Lấy danh sách nhóm máu ABO |
| `api/HisBloodRh/Get` | Lấy danh sách yếu tố Rh |
| `api/HisPatient/GetView` | Lấy thông tin bệnh nhân |
| `api/HisPatient/Update` | Cập nhật nhóm máu bệnh nhân |

---

## 6. Dependency

### Project References

| Project | Mục đích |
|---------|---------|
| HIS.Desktop.ApiConsumer | `ApiConsumers.MosConsumer` |
| HIS.Desktop.Common | `FormBase`, `BusinessBase` |
| HIS.Desktop.Controls.Session | `SessionManager.ProcessTokenLost` |
| HIS.Desktop.LocalStorage.BackendData | `BackendDataWorker.Get<T>` |
| HIS.Desktop.LocalStorage.ConfigApplication | Cấu hình ứng dụng |
| HIS.Desktop.LocalStorage.ConfigSystem | Cấu hình hệ thống |
| HIS.Desktop.LocalStorage.HisConfig | `HisConfigs.Get<string>` |
| HIS.Desktop.LocalStorage.LocalData | Dữ liệu local |
| HIS.Desktop.LocalStorage.Location | `ApplicationStoreLocation` |
| HIS.Desktop.Plugins.Library.EmrGenerate | In phiếu |

### DLL References

| DLL | Control / Type dùng |
|-----|-------------------|
| DevExpress.Data.v15.2 | Data layer |
| DevExpress.Utils.v15.2 | Utilities |
| DevExpress.XtraEditors.v15.2 | LookUpEdit, TextEdit, SimpleButton |
| DevExpress.XtraGrid.v15.2 | GridLookUpEdit (cboXNHH, cboAC) |
| DevExpress.XtraTreeList.v15.2 | TreeList (`treeListExpBlood`) |
| DevExpress.XtraLayout.v15.2 | LayoutControl |
| DevExpress.XtraBars.v15.2 | BarManager, BarButtonItem (Ctrl S, Ctrl P) |
| MOS.EFMODEL | `V_HIS_EXP_MEST_BLOOD`, `V_HIS_SERE_SERV_TEIN`, `V_HIS_EXP_MEST`, `V_HIS_EXP_BLTY_SERVICE`, `HIS_BLOOD_ABO`, `HIS_BLOOD_RH`, `HIS_PREPARATIONS_BLOOD`, `HIS_BLOOD_TYPE`, `HIS_BLOOD_GROUP` |
| MOS.Filter | `HisSereServTeinViewFilter`, `HisExpMestBloodViewFilter`, `HisExpBltyServiceFilter`, `HisBloodAboFilter`, `HisBloodRhFilter` |
| Inventec.Common.Adapter | `BackendAdapter` |
| Inventec.Common.Controls.EditorLoader | `ControlEditorLoader`, `ColumnInfo`, `ControlEditorADO` |
| Inventec.Core | `CommonParam` |

---

## 7. Điều kiện tiên quyết

1. **Cấu hình `BloodHarmonyTestIndex`** phải được khai báo trong HisConfig với key `HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.BloodHarmonyTestIndex` theo định dạng:
   ```
   <mã_túi_máu_1>|<mã_túi_máu_2>;...;<mã_MT_muối_1>|...;<mã_anti_globulin_1>|...
   ```
   Nếu không có cấu hình, tính năng XN hòa hợp sẽ không tự điền (log cảnh báo).

2. **Database:** View `V_HIS_SERE_SERV_TEIN` phải có các cột: `ID`, `TEST_INDEX_CODE`, `TEST_INDEX_NAME`, `VALUE`, `SERE_SERV_ID`, `TDL_TREATMENT_ID`, `RESULT_TIME`.

3. **Database:** `V_HIS_EXP_MEST_BLOOD` phải có cột `TUBE_SLOT` (giá trị 1 hoặc 2) để xác định ống nghiệm.

4. **Modulelink:** Insert record modulelink trong database cho `HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood`.

5. **Cấu hình tuỳ chọn:** `HIS.Desktop.Plugins.BrowseExportTicket.IsNotAllowEditBloodInformation = "1"` để khoá chỉnh sửa nhóm máu BN khi đã có dữ liệu.
