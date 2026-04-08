# Tai lieu phan tich thiet ke
# HIS.Desktop.Plugins.PublicMedicineGeneral - Tao phieu cong khai thuoc

---

## 1. Muc dich

Plugin phuc vu viec tao va in phieu cong khai thuoc/vat tu/mau cho benh nhan noi tru. Ho tro 2 che do:
- **Cong khai theo ngay (chkDate):** Tim kiem theo phieu xuat kho (HisExpMest), load chi tiet thuoc/vat tu/mau, in mau Mps000177.
- **Cong khai theo y lenh (chkServiceReq):** Tim kiem theo y lenh (HisServiceReq), load chi tiet dich vu, in mau Mps000486.

---

## 2. Cau truc project

```
HIS.Desktop.Plugins.PublicMedicineGeneral/
+-- ADO/
|   +-- ExpMestMediAndMateADO.cs
+-- Base/
|   +-- GlobaStore.cs
+-- PublicMedicineGeneral/
|   +-- IPublicMedicineGeneral.cs
|   +-- PublicMedicineGeneralFactory.cs
|   +-- PublicMedicineGeneralBehavior.cs
+-- Resources/
|   +-- ResourceLanguageManager.cs
|   +-- ResourceMessage.cs
+-- Validation/
|   +-- DateValidationRule.cs
|   +-- Validation.cs
+-- Properties/
|   +-- AssemblyInfo.cs
+-- FormPublicMedicineGeneral.cs
+-- FormPublicMedicineGeneral.Designer.cs
+-- PublicMedicineGeneralProcessor.cs
+-- HIS.Desktop.Plugins.PublicMedicineGeneral.csproj
```

---

## 3. Dang ky Module

**File:** `PublicMedicineGeneralProcessor.cs`

```
Module Link  : HIS.Desktop.Plugins.PublicMedicineGeneral
Ten hien thi : Tao cong khai thuoc
Icon         : thuoc.png
Nhom         : Common
Thu tu        : 23
Loai         : MODULE_TYPE_ID__FORM
```

**Luong khoi tao:**
```
PublicMedicineGeneralProcessor.Run(args)
  -> PublicMedicineGeneralFactory.MakeIPublicMedicineGeneral(param, args)
    -> PublicMedicineGeneralBehavior.Run()
      -> new FormPublicMedicineGeneral(currentModule)
```

---

## 4. Thiet ke chi tiet

### 4.1. ExpMestMediAndMateADO (ADO/ExpMestMediAndMateADO.cs)

Ke thua `V_HIS_EXP_MEST_MEDICINE`, bo sung cac property dung chung cho thuoc/vat tu/mau.

| Property | Type | Mo ta |
|----------|------|-------|
| Service_Type_Id | long | Loai dich vu: THUOC / VT / MAU |
| IS_CHEMICAL_SUBSTANCE | short? | Hoa chat (vat tu) |
| INTRUCTION_TIME | long | Thoi gian chi dinh (format yyyyMMddHHmmss) |
| INTRUCTION_DATE | long | Ngay chi dinh (format yyyyMMddHHmmss) |
| TREATMENT_ID | long | ID ho so dieu tri |
| type | int | Loai (dung trong in) |

---

### 4.2. GlobaStore (Base/GlobaStore.cs)

Chua cac hang so API endpoint:

| Hang so | Gia tri |
|---------|---------|
| HisPatientGetview | api/HisPatient/GetView |
| HisPrescriptionGetview1 | api/HisPrescription/GetView1 |
| HisTreatmentGetView | api/HisTreatment/GetView |
| HisTreatmentBedRoomGetview | api/HisTreatmentBedRoom/GetView |
| HisAggrExpMestGetview | api/HisAggrExpMest/GetView |
| HisExpMestGet | api/HisExpMest/Get |
| HisBedRoomGetView | api/HisBedRoom/GetView |
| HisSereServGetView | api/HisSereServ/GetView |
| HisMedicineGet | api/HisMedicine/Get |
| HisMaterialGet | api/HisMaterial/Get |
| MAX_REQUEST_LENGTH_PARAM | 100 (so luong ID toi da moi request) |

---

### 4.3. FormPublicMedicineGeneral - Man hinh chinh

**File:** `FormPublicMedicineGeneral.cs`, `FormPublicMedicineGeneral.Designer.cs`

#### 4.3.1. Giao dien (DevExpress LayoutControl)

```
+-------------------------------------------------------------------+
| [chkDate] Cong khai theo ngay    [chkServiceReq] Cong khai theo   |
|                                               y lenh              |
+-------------------------------------------------------------------+
| Thuoc [v]    Vat tu [v]    Mau [v]                                |
+-------------------------------------------------------------------+
| [cboTimeType]  Tu ngay: [dtFromTime]  Den ngay: [dtToTime]       |
|   [txtPatientCode]  [txtExpMestCode]        [btnFind Tim kiem]    |
+-------------------------------------------------------------------+
| +-- gridControlTreatmentBedRoom --+                               |
| | STT | Ten BN | Ma BN | Ma DT | TG vao | Giuong |              |
| | (checkbox multiselect)          |                               |
| +-------------------------------------------------------------------+
|                                                    [btnPrint In]  |
+-------------------------------------------------------------------+
```

#### 4.3.2. Cac control tren form

| Control | Loai | Mo ta |
|---------|------|-------|
| chkDate | CheckEdit (Radio) | Che do "Cong khai theo ngay" - RadioGroupIndex=1 |
| chkServiceReq | CheckEdit (Radio) | Che do "Cong khai theo y lenh" - RadioGroupIndex=1 |
| chkMedicine | CheckEdit | Loc thuoc |
| chkMedical | CheckEdit | Loc vat tu |
| chkBlood | CheckEdit | Loc mau |
| cboTimeType | ComboBoxEdit | Chon loai thoi gian loc: "Thoi gian chi dinh" (mac dinh, index=0) / "Thoi gian du tru" (index=1). Dat truoc dtFromTime trong layout. |
| dtFromTime | DateEdit | Tu ngay (dd/MM/yyyy) - required |
| dtToTime | DateEdit | Den ngay (dd/MM/yyyy) - required |
| txtPatientCode | TextEdit | Ma benh nhan (chi cho nhap so) |
| txtExpMestCode | TextEdit | Ma phieu tong hop (chi cho nhap so, an khi chkServiceReq checked) |
| btnFind | SimpleButton | Tim kiem (Ctrl+F) |
| btnPrint | SimpleButton | In (Ctrl+P) |
| gridControlTreatmentBedRoom | GridControl | Danh sach benh nhan - multiselect checkbox |
| barManager1 | BarManager | Phim tat: Ctrl+T, Ctrl+F, Ctrl+P |

#### 4.3.3. Cac cot trong gridViewTreatmentBedRoom

| Cot | FieldName | Mo ta |
|-----|-----------|-------|
| STT | STT (Unbound) | So thu tu tu dong |
| Ten benh nhan | TDL_PATIENT_NAME | Ten benh nhan |
| Ma benh nhan | TDL_PATIENT_CODE | Ma benh nhan |
| Ma dieu tri | TREATMENT_CODE | Ma dieu tri |
| Thoi gian vao | TIME_str (Unbound) | Format tu IN_TIME |
| Giuong | BED_NAME | Ten giuong |

---

### 4.4. Luong xu ly chinh

#### 4.4.1. Form Load
```
FormPublicMedicineGeneral_Load()
  -> LoadDataPatient()         // Goi API HisTreatmentBedRoom/GetView lay DS benh nhan trong buong
  -> InitControlState()        // Phuc hoi trang thai chkDate/chkServiceReq tu cache
  -> SetIcon()
  -> LoadKeysFromlanguage()    // Load nhan da ngon ngu
  -> SetPrintTypeToMps()       // Load PrintTypes tu SAR
  -> SetDefaultValueControl()  // Set cboTimeType=0, ngay hien tai, check thuoc/VT/mau
  -> FillDataToCbo()           // Lay thong tin phong/khoa hien tai
  -> Validation()              // Dat validation cho dtFromTime, dtToTime
```

#### 4.4.2. Tim kiem (btnFind_Click)
```
btnFind_Click()
  Neu co txtPatientCode -> loc ListTreatmentBedRooms theo TDL_PATIENT_CODE
  Neu co txtExpMestCode -> goi GetDataByFilter(false) lay ExpMest,
                           loc ListTreatmentBedRooms theo TREATMENT_ID
  Con lai -> hien thi toan bo ListTreatmentBedRooms
  -> Cap nhat gridControlTreatmentBedRoom, SelectAll

  * GetDataByFilter(false) su dung cboTimeType de chon filter:
    - Index 0: TDL_INTRUCTION_DATE_FROM/TO
    - Index 1: TDL_USE_TIME_FROM/TO (+ them query INTRUCTION_DATE, chi giu USE_TIME=null)
```

#### 4.4.3. In (btnPrint_Click)
```
btnPrint_Click()
  Validate form
  Kiem tra da chon benh nhan
  Neu chkDate.Checked:
    -> GetDataByFilter(IsPrint=true)
      -> Clear dicExpMest, bedRoomName
      -> Tao HisExpMestFilter theo cboTimeType:
         + Index 0 (Thoi gian chi dinh):
           filter.TDL_INTRUCTION_DATE_FROM/TO
         + Index 1 (Thoi gian du tru):
           Query 1: filter.TDL_USE_TIME_FROM/TO -> lay du tru ngay do
           Query 2: filter.TDL_INTRUCTION_DATE_FROM/TO -> lay chi dinh ngay do
                    Chi giu records co TDL_USE_TIME = null (ke cung ngay)
           Gop 2 ket qua, loai trung theo ID (Dictionary)
           Override TDL_INTRUCTION_TIME = TDL_USE_TIME cho records co USE_TIME
         + TDL_PATIENT_CODE__EXACT / EXP_MEST_CODE__EXACT
         + REQ_DEPARTMENT_ID (khoa hien tai)
      -> CallApiExpMest() -> api/HisExpMest/Get
      -> Sort _ExpMestMediAndMateADOs: Thuoc -> Vat tu -> Mau, roi theo ten
      -> CreateThreadLoadData_New() (5 threads song song, co lock):
         Thread 1: LoadDataMedicine    -> api/HisExpMestMedicine/GetView
         Thread 2: LoadDataMaterial    -> api/HisExpMestMaterial/GetView
         Thread 3: LoadDataBlood       -> HIS_EXP_MEST_BLOOD_GETVIEW
         Thread 4: LoadDataPatient     -> api/HisTreatment/GetView + api/HisPatient/GetView
         Thread 5: LoadDataTreatmentBedRoom -> api/HisTreatmentBedRoom/GetView
      -> PrintProcess() -> Mps000177 (Phieu cong khai thuoc theo ngay)

  Neu chkServiceReq.Checked:
    -> Print486()
      -> Tao HisServiceReqFilter theo cboTimeType:
         + Index 0: INTRUCTION_DATE_FROM/TO
         + Index 1: USE_TIME_FROM/TO
      -> 1 query duy nhat api/HisServiceReq/Get
      -> api/HisSereServ/GetView2
      -> api/HisServiceReqMety/Get
      -> api/HisServiceReqMaty/Get
      -> Mps000486 (Phieu cong khai thuoc theo y lenh)
```

---

### 4.5. API duoc goi

| API Endpoint | Filter chinh | Muc dich |
|--------------|-------------|----------|
| api/HisTreatmentBedRoom/GetView | BED_ROOM_ID, IS_IN_ROOM=true | Load DS benh nhan dang nam |
| api/HisExpMest/Get | TDL_PATIENT_CODE__EXACT, EXP_MEST_CODE__EXACT, TDL_INTRUCTION_DATE_FROM/TO hoac TDL_USE_TIME_FROM/TO, REQ_DEPARTMENT_ID, EXP_MEST_TYPE_ID, AGGR_EXP_MEST_IDs, TDL_TREATMENT_IDs | Lay phieu xuat kho |
| api/HisExpMestMedicine/GetView | EXP_MEST_IDs | Chi tiet thuoc |
| api/HisExpMestMaterial/GetView | EXP_MEST_IDs | Chi tiet vat tu |
| HIS_EXP_MEST_BLOOD_GETVIEW | EXP_MEST_IDs | Chi tiet mau |
| api/HisTreatment/GetView | IDs | Thong tin dieu tri |
| api/HisPatient/GetView | IDs (patient) | Thong tin benh nhan |
| api/HisServiceReq/Get | TREATMENT_IDs, SERVICE_REQ_TYPE_IDs, INTRUCTION_DATE_FROM/TO hoac USE_TIME_FROM/TO | Y lenh (mode ServiceReq) |
| api/HisSereServ/GetView2 | SERVICE_REQ_IDs | Chi tiet dich vu |
| api/HisServiceReqMety/Get | SERVICE_REQ_IDs | Loai thuoc theo y lenh |
| api/HisServiceReqMaty/Get | SERVICE_REQ_IDs | Loai vat tu theo y lenh |

---

### 4.6. Filter thoi gian theo cboTimeType

#### 4.6.1. Thoi gian chi dinh (cboTimeType.SelectedIndex = 0)

1 query duy nhat:

| Che do | API | Filter |
|--------|-----|--------|
| Cong khai theo ngay | HisExpMest/Get | TDL_INTRUCTION_DATE_FROM/TO |
| Cong khai theo y lenh | HisServiceReq/Get | INTRUCTION_DATE_FROM/TO |

#### 4.6.2. Thoi gian du tru (cboTimeType.SelectedIndex = 1)

**Cong khai theo ngay (HisExpMest/Get):** 2 query gop lai (OR logic):

| Query | Filter | Loc |
|-------|--------|-----|
| Query 1 (du tru) | TDL_USE_TIME_FROM/TO | Giu tat ca |
| Query 2 (chi dinh cung ngay) | TDL_INTRUCTION_DATE_FROM/TO | Chi giu TDL_USE_TIME = null |

**Cong khai theo y lenh (HisServiceReq/Get):** 1 query duy nhat:

| Filter |
|--------|
| USE_TIME_FROM/TO |

**Giai thich:**
- Query 1: Lay tat ca phieu co du tru cho ngay do (VD: M3 ke cho M4 -> USE_TIME=M4)
- Query 2: Lay chi dinh ngay do nhung chi giu nhung don USE_TIME = null (ke cung ngay, VD: M4 ke cho M4 nhung khong ghi USE_TIME)
- Gop 2 ket qua vao Dictionary theo ID -> dam bao khong trung, ket qua on dinh
- Override TDL_INTRUCTION_TIME = TDL_USE_TIME cho records co USE_TIME -> phieu in hien thi dung ngay du tru

**Vi du chon ngay M4:**
- M3 ke cho M4 (USE_TIME=M4) -> lay tu query 1
- M4 ke cho M4 (USE_TIME=null) -> lay tu query 2
- M4 ke cho M5 (USE_TIME=M5) -> query 2 loai bo vi USE_TIME != null

---

### 4.7. Validation

**DateValidationRule:** Kiem tra DateEdit khong rong va khac DateTime.MinValue (cho dtFromTime, dtToTime).

**Validation:** Kiem tra it nhat mot trong hai truong txtPatientCode hoac txtExpMestCode phai co gia tri (hien khong su dung truc tiep trong form).

---

### 4.8. Mau in

| Ma mau | Mo ta | Su dung khi |
|--------|-------|-------------|
| Mps000177 | Phieu cong khai thuoc theo ngay | chkDate.Checked |
| Mps000486 | Phieu cong khai thuoc theo y lenh | chkServiceReq.Checked |

**Thu tu sap xep khi in (Mps000177):** Thuoc -> Vat tu -> Mau, roi theo ten (MEDICINE_TYPE_NAME) A-Z trong moi nhom.

---

### 4.9. Tinh nang loc theo loai thoi gian (cboTimeType)

**Yeu cau:** Cho phep nguoi dung chon loc theo thoi gian chi dinh hoac thoi gian du tru.

**Control:** `cboTimeType` (ComboBoxEdit, TextEditStyle=DisableTextEditor)
- Index 0: "Thoi gian chi dinh" — mac dinh khi mo form (`SetDefaultValueControl`)
- Index 1: "Thoi gian du tru"

**Ap dung cho:** ca In (Ctrl+P) va Tim kiem (Ctrl+F)

**Param BE su dung:**
- `HisExpMest/Get`: `TDL_USE_TIME_FROM`, `TDL_USE_TIME_TO` (thoi gian du tru)
- `HisServiceReq/Get`: `USE_TIME_FROM`, `USE_TIME_TO` (thoi gian du tru)

**Logic khi chon "Thoi gian du tru":**
1. Query 1: `TDL_USE_TIME_FROM/TO` -> lay phieu co du tru ngay do
2. Query 2: `TDL_INTRUCTION_DATE_FROM/TO` -> lay chi dinh ngay do, chi giu records co `TDL_USE_TIME = null`
3. Gop vao Dictionary theo ID (uu tien query 1) -> ket qua on dinh, khong trung
4. Override `TDL_INTRUCTION_TIME = TDL_USE_TIME` cho records co USE_TIME -> hien thi dung ngay du tru tren phieu in

---

### 4.10. Thread-safety

3 thread (LoadDataMedicine, LoadDataMaterial, LoadDataBlood) chay song song va cung ghi vao `_ExpMestMediAndMateADOs` (List<>).
Su dung `lock (_lockExpMestADO)` khi `.Add()` de tranh race condition lam mat/trung data.

Truoc moi lan in, clear `dicExpMest` va `bedRoomName` de tranh tich luy data cu tu lan in truoc.

---

## 5. Design Patterns

- **Factory Pattern:** `PublicMedicineGeneralFactory` tao `PublicMedicineGeneralBehavior` qua interface `IPublicMedicineGeneral`.
- **Module Registration:** `PublicMedicineGeneralProcessor` ke thua `ModuleBase`, dang ky qua `ExtensionOf` attribute.
- **Multi-threading + Lock:** 5 threads chay song song de load data; su dung `lock` cho List<> chung de dam bao thread-safety.
- **Control State Cache:** Luu trang thai chkDate/chkServiceReq qua `ControlStateWorker` de phuc hoi khi mo lai form.
- **2-query OR merge:** Khi chon "Thoi gian du tru", goi 2 API rieng (USE_TIME + INTRUCTION_DATE) roi gop ket qua bang Dictionary de mo phong OR logic ma API khong ho tro.

---

## 6. Dependency

### Project references
- HIS.Desktop.ApiConsumer
- HIS.Desktop.LocalStorage.BackendData
- HIS.Desktop.LocalStorage.ConfigApplication
- HIS.Desktop.LocalStorage.ConfigSystem
- HIS.Desktop.LocalStorage.Location
- HIS.Desktop.Library.CacheClient
- HIS.Desktop.Utility
- Inventec.Core
- Inventec.Desktop.Core
- Inventec.Desktop.Common
- Inventec.Common.Adapter
- MOS.EFMODEL
- MOS.Filter
- MPS.Processor.Mps000177
- MPS.Processor.Mps000486
- AutoMapper
- DevExpress (XtraEditors, XtraGrid, XtraLayout, XtraBars)
