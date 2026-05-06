# Tài liệu phân tích thiết kế
# HIS.Desktop.Plugins.ApproveExpMestBCS - Duyệt phiếu xuất bù tủ trực

---

## 1. Mục đích

Form duyệt phiếu xuất bù cơ số tủ trực (BCS). Dược sĩ thao tác duyệt số lượng thuốc/vật tư được yêu cầu từ phiếu xuất BCS, có thể thay thế bằng thuốc/vật tư khác (nếu cấu hình cho phép) và thực xuất phiếu.

Plugin bổ sung chế độ **Tách theo bệnh nhân** — cho phép hiển thị và duyệt thuốc/vật tư theo từng điều trị (TREATMENT_ID) thay vì gom chung theo loại như trước, giúp dược sĩ dễ bỏ tick/chỉnh số lượng cho từng bệnh nhân cụ thể.

---

## 2. Cấu trúc project

```
HIS.Desktop.Plugins.ApproveExpMestBCS/
├── ADO/
│   ├── MaterialBeanADO.cs               (Lô vật tư được chọn)
│   ├── MaterialTypeADO.cs               (Row hiển thị vật tư — thêm TREATMENT_ID, PATIENT_NAME, TREATMENT_CODE)
│   ├── MedicineBeanADO.cs               (Lô thuốc được chọn)
│   ├── MedicineTypeADO.cs               (Row hiển thị thuốc — thêm TREATMENT_ID, PATIENT_NAME, TREATMENT_CODE)
│   └── MetyMatyTypeADO.cs               (ADO dùng cho popup thay thế)
├── ApproveExpMestBCS/
│   ├── IApproveExpMestBCS.cs
│   ├── ApproveExpMestBCSBehavior.cs
│   └── ApproveExpMestBCSFactory.cs
├── Config/
│   └── HisConfig.cs                     (Load IS_ALLOW_REPLACE, IS_AUTO_REPLACE)
├── Popup/
│   ├── frmSelectLo.cs                   (Chọn lô thuốc/vật tư)
│   └── frmSelectLo.Designer.cs
├── Properties/
│   ├── AssemblyInfo.cs
│   ├── Resources.resx
│   └── Resources.Designer.cs
├── Resources/
│   ├── Lang.vi.resx                     (Đa ngôn ngữ - tiếng Việt)
│   ├── Lang.en.resx                     (Đa ngôn ngữ - tiếng Anh)
│   └── ResourceLanguageManager.cs       (Holder ResourceManager)
├── Util/
│   └── StringUtil.cs
├── Validation/
│   ├── ComboMediMatyValidationRule.cs
│   └── SpinAmountValidationRule.cs
├── frmApproveExpMestBCS.cs              (Form chính)
├── frmApproveExpMestBCS.Designer.cs
├── frmReplace.cs                        (Popup chọn thuốc/vật tư thay thế)
├── frmReplace.Designer.cs
└── ApproveExpMestBCSProcessor.cs        (Entry point MEF)
```

---

## 3. Luồng hoạt động

1. `ApproveExpMestBCSProcessor` đăng ký module, gọi `ApproveExpMestBCSFactory` → `ApproveExpMestBCSBehavior` → khởi tạo `frmApproveExpMestBCS`.
2. Form nhận `expMestId` (ID phiếu xuất), `DelegateSelectData` (callback trả về kết quả).
3. Load dữ liệu theo thứ tự:
   - `LoadExpMest` — thông tin phiếu + kho phòng hiện tại
   - `VisibleColumnBCS` — bật/tắt cột thay thế theo `IS_ALLOW_REPLACE`
   - `LoadDataInStock` — tồn thuốc/vật tư trong kho
   - `LoadDataMedicine` / `LoadDataMaterial` — build ADO từ `HIS_EXP_MEST_METY_REQ` / `HIS_EXP_MEST_MATY_REQ`, merge với thuốc đã duyệt (nếu có replace)
   - **`LoadTreatmentDict`** (mới) — 1 API call `api/HisTreatment/Get` với danh sách `TREATMENT_ID` duy nhất, lookup Dict
   - **`InitControlState`** (mới) — đọc trạng thái checkbox "Tách theo bệnh nhân" từ cache
   - **Split nếu state = CHECK và có TREATMENT_ID** — tách ADO thành rows per bệnh nhân
   - `LoadDataAutoReplace` — tự động thay thế thuốc nếu `IS_AUTO_REPLACE = 1` (per-patient khi split)
4. User tick/bỏ tick, chỉnh `YCD_AMOUNT`, có thể bấm nút thay thế để chọn thuốc B thay A.
5. User toggle checkbox **Tách theo bệnh nhân**:
   - UNCHECK → CHECK: `SplitMedicineAdosByPatient` / `SplitMaterialAdosByPatient` — tách ADO theo (BN × loại)
   - CHECK → UNCHECK: `GroupMedicineAdosByType` / `GroupMaterialAdosByType` — cộng dồn YCD về 1 row/loại, replace rows gộp theo `REPLACE_*_TYPE_ID`
6. Bấm **Lưu (Ctrl+S)** → `btnSave_Click` → `MakeMedicine`/`MakeMaterial` build `HisExpMestApproveSDO` với per-request SDO (mỗi SDO gắn 1 `ExpMestMetyReqId`/`ExpMestMatyReqId` + `TreatmentId` lấy từ `HIS_EXP_MEST_METY_REQ.TREATMENT_ID`) → POST `api/HisExpMest/Approve`.

---

## 4. API sử dụng

| Endpoint | Consumer | Mục đích |
|---|---|---|
| `api/HisExpMest/Get` | MosConsumer | Lấy phiếu xuất BCS theo ID |
| `api/HisExpMestMetyReq/Get` | MosConsumer | Lấy danh sách yêu cầu thuốc |
| `api/HisExpMestMatyReq/Get` | MosConsumer | Lấy danh sách yêu cầu vật tư |
| `api/HisMedicineType/GetInStock` | MosConsumer | Tồn kho loại thuốc |
| `api/HisMaterialType/GetInStock` | MosConsumer | Tồn kho loại vật tư |
| `api/HisExpMestMedicine/Get` | MosConsumer | Thuốc đã duyệt (khi có replace) |
| `api/HisExpMestMaterial/Get` | MosConsumer | Vật tư đã duyệt (khi có replace) |
| `api/HisMediStock/GetReplaceSDO` | MosConsumer | Danh sách thay thế khả dụng |
| **`api/HisTreatment/Get`** (mới) | MosConsumer | Load thông tin điều trị theo `HisTreatmentFilter.IDs` — phục vụ hiển thị cột Tên BN/Mã ĐT |
| `api/HisExpMest/Approve` | MosConsumer | Duyệt + thực xuất (`HisExpMestApproveSDO`) |

---

## 5. Danh sách cột hiển thị (cả grid Thuốc và Vật tư)

| VisibleIndex | FieldName | Caption | Mô tả |
|---|---|---|---|
| 0 | `IsCheck` | (checkbox) | Chọn để duyệt |
| **1 (khi CHECK)** | **`PATIENT_NAME`** | **Tên bệnh nhân** | **Chỉ hiển thị khi "Tách theo bệnh nhân" = CHECK** |
| **2 (khi CHECK)** | **`TREATMENT_CODE`** | **Mã điều trị** | **Chỉ hiển thị khi "Tách theo bệnh nhân" = CHECK** |
| 1 (UNCHECK) / 3 (CHECK) | `Replace` | (icon) | Nút mở popup thay thế |
| 2 / 4 | `REPLACE_MEDICINE_TYPE_NAME` / `REPLACE_MATERIAL_TYPE_NAME` | Thay thế cho | Chỉ có data khi row là thuốc thay thế |
| 3 / 5 | `MEDICINE_TYPE_NAME` / `MATERIAL_TYPE_NAME` | Tên thuốc / Tên vật tư | |
| 4 / 6 | `MEDICINE_TYPE_CODE` / `MATERIAL_TYPE_CODE` | Mã thuốc / Mã vật tư | |
| 5 / 7 | `INGR_ACTIVE_BHYT_NAME` | Hoạt chất | (chỉ có ở grid thuốc) |
| 6 / 8 | `SERVICE_UNIT_NAME` | Đơn vị | |
| 7 / 9 | `AMOUNT` | Số lượng yêu cầu | `HIS_EXP_MEST_*_REQ.AMOUNT` |
| 8 / 10 | `DD_AMOUNT` | Đã duyệt | `HIS_EXP_MEST_*_REQ.DD_AMOUNT` |
| 9 / 11 | `AVAIL_AMOUNT` | Khả dụng | Tồn kho; khi split → portion còn lại sau cumulative |
| 10 / 12 | `YCD_AMOUNT` | Số lượng duyệt | Input user |
| 11 / 13 | `TT_AMOUNT` | Số lượng thay thế | |

---

## 6. Thay đổi: Bổ sung chế độ "Tách theo bệnh nhân"

### 6.1. Yêu cầu

Khi duyệt phiếu BCS có yêu cầu của nhiều bệnh nhân, thao tác bỏ thuốc/vật tư hoặc chỉnh số lượng duyệt chung cho cả đơn làm user khó kiểm soát. Bổ sung:

- Checkbox **Tách theo bệnh nhân** — lưu trạng thái, dùng lại lần sau.
- CHECK: hiện 2 cột **Tên bệnh nhân**, **Mã điều trị**, tách mỗi loại thuốc/vật tư thành nhiều row theo `TREATMENT_ID`, cho phép bỏ tick/chỉnh YCD từng BN.
- UNCHECK: quay về chế độ gom theo loại, ẩn 2 cột; cộng dồn YCD từng loại; replace gộp theo `REPLACE_*_TYPE_ID`.

### 6.2. Thiết kế ADO

**`MedicineTypeADO.cs`** và **`MaterialTypeADO.cs`** — thêm 3 property:
```csharp
public long? TREATMENT_ID { get; set; }
public string PATIENT_NAME { get; set; }
public string TREATMENT_CODE { get; set; }
```

### 6.3. Thiết kế UI (frmApproveExpMestBCS.Designer.cs)

**Thêm control:**
- `chkSplitByPatient` (CheckEdit) — ô vuông 20×20, không caption riêng.
- `lciSplitByPatient` (LayoutControlItem) — Text = `"Tách theo bệnh nhân:"`, alignment Far (giống `lciDescription`), đặt sát mép phải cùng dòng với `txtDescription`.

**Thêm 4 cột grid** (mặc định `Visible = false`):
- `gridColumn_Medicine_PatientName` — FieldName `PATIENT_NAME`, Caption "Tên bệnh nhân"
- `gridColumn_Medicine_TreatmentCode` — FieldName `TREATMENT_CODE`, Caption "Mã điều trị"
- `gridColumn_Material_PatientName`, `gridColumn_Material_TreatmentCode` — tương tự cho grid vật tư

Lưu ý: **KHÔNG set `VisibleIndex` trong Designer** — DevExpress sẽ tự bật `Visible = true` khi `VisibleIndex >= 0`. Việc gán VisibleIndex chỉ thực hiện trong `SetSplitColumnsVisible(true)` lúc runtime.

### 6.4. Thiết kế frontend (frmApproveExpMestBCS.cs)

**a) Fields mới**
```csharp
#region SplitByPatient
private bool isSplitByPatient = false;
private Dictionary<long, HIS_TREATMENT> treatmentDict = new Dictionary<long, HIS_TREATMENT>();
#endregion

#region ControlState
private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
private bool isNotLoadWhileChangeControlStateInFirst = false;
private const string MODULE_LINK = "HIS.Desktop.Plugins.ApproveExpMestBCS";
private const string KEY_SPLIT_BY_PATIENT = "chkSplitByPatient";
#endregion
```

**b) Load flow mới trong `frmApproveExpMestBCS_Load`**
```
LoadExpMest → VisibleColumnBCS → LoadDataInStock
  → LoadDataMedicine → LoadDataMaterial
  → LoadTreatmentDict          (MỚI: 1 API call)
  → InitControlState            (MỚI: đọc state từ cache)
  → if (isSplit && hasTreatment): SplitMedicine/MaterialAdosByPatient + show columns
  → LoadDataAutoReplace         (replace sau khi đã split nếu TH1)
```

**c) `LoadTreatmentDict`** — 1 API call duy nhất cho cả 2 tab:
```csharp
treatmentDict = new Dictionary<long, HIS_TREATMENT>();
var ids = new HashSet<long>();
foreach (var r in expMestMetyReqs)
    if (r.TREATMENT_ID.HasValue && r.TREATMENT_ID.Value > 0) ids.Add(r.TREATMENT_ID.Value);
foreach (var r in expMestMatyReqs)
    if (r.TREATMENT_ID.HasValue && r.TREATMENT_ID.Value > 0) ids.Add(r.TREATMENT_ID.Value);
if (ids.Count == 0) return;

var filter = new HisTreatmentFilter();
filter.IDs = ids.ToList();
var list = new BackendAdapter(new CommonParam())
    .Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, null);
if (list != null)
{
    foreach (var t in list)
        if (!treatmentDict.ContainsKey(t.ID)) treatmentDict.Add(t.ID, t);
}
```

**d) `SplitMedicineAdosByPatient` / `SplitMaterialAdosByPatient`** — 2 pass:

**Pass 1**: tách row yêu cầu theo `TREATMENT_ID`:
```csharp
var byTreatment = ado.Requests
    .GroupBy(r => r.TREATMENT_ID)
    .OrderBy(g => g.Sum(r => r.AMOUNT - (r.DD_AMOUNT ?? 0))) // BN cần ít duyệt trước
    .ToList();

decimal remainingAvail = ado.AVAIL_AMOUNT;
foreach (var grp in byTreatment)
{
    var splitAdo = CloneMedicineMetadata(ado);
    splitAdo.Requests = grp.ToList();
    splitAdo.AMOUNT = grp.Sum(r => r.AMOUNT);
    splitAdo.DD_AMOUNT = grp.Sum(r => r.DD_AMOUNT ?? 0);
    splitAdo.AVAIL_AMOUNT = Math.Max(remainingAvail, 0);
    ApplyPatientInfo(splitAdo, grp.Key);   // null → 2 cột trống
    decimal needApprove = splitAdo.AMOUNT - splitAdo.DD_AMOUNT;
    splitAdo.YCD_AMOUNT = Math.Min(needApprove, splitAdo.AVAIL_AMOUNT);
    remainingAvail -= splitAdo.YCD_AMOUNT; // CUMULATIVE: BN sau thấy tồn còn lại
    result.Add(splitAdo);
}
```

**Pass 2**: phân bổ row thay thế (IsReplace) cho các BN:
```csharp
var requestLookup = result
    .Where(o => !o.IsReplace && !o.IsApproved && o.TREATMENT_ID.HasValue)
    .GroupBy(o => o.MEDICINE_TYPE_ID)
    .ToDictionary(g => g.Key, g => g.OrderBy(x => x.AMOUNT - x.CURRENT_DD_AMOUNT).ToList());

foreach (var repl in pendingReplaces)
{
    var reqBNs = requestLookup[repl.REPLACE_MEDICINE_TYPE_ID.Value];
    decimal remaining = repl.YCD_AMOUNT;
    foreach (var reqBN in reqBNs) // ASC theo cần duyệt
    {
        if (remaining <= 0) break;
        decimal alloc = Math.Min(remaining, reqBN.AMOUNT - reqBN.CURRENT_DD_AMOUNT);
        var splitRepl = CloneMedicineMetadata(repl);
        splitRepl.YCD_AMOUNT = alloc;
        splitRepl.Requests = reqBN.Requests;
        ApplyPatientInfo(splitRepl, reqBN.TREATMENT_ID);
        result.Add(splitRepl);
        remaining -= alloc;
    }
}
```

**e) `GroupMedicineAdosByType` / `GroupMaterialAdosByType`** — gộp ngược khi UNCHECK:

- Row yêu cầu: `GroupBy(MEDICINE_TYPE_ID)`, YCD = sum các row đã tick
- Row thay thế: `GroupBy(MEDICINE_TYPE_ID + REPLACE_MEDICINE_TYPE_ID + IsApproved)`

**f) `chkSplitByPatient_CheckedChanged`**:
```csharp
if (isNotLoadWhileChangeControlStateInFirst) return;
if (chkSplitByPatient.Checked)
{
    if (!HasAnyTreatmentId()) { SetSplitColumnsVisible(false); SaveControlState(); return; }
    SplitMedicine/MaterialAdosByPatient(); SetSplitColumnsVisible(true);
}
else
{
    if (isSplitByPatient) GroupMedicine/MaterialAdosByType();
    SetSplitColumnsVisible(false);
}
// Rebind grids + SaveControlState
```

**g) Auto Replace (`LoadDataAutoReplace`)** — khi `isSplitByPatient = true`:

- `lisMediReqs` sort `OrderBy(o.AMOUNT - o.CURRENT_DD_AMOUNT) ASC` → BN cần ít trước
- Với mỗi BN: tạo `replaceMedicine`, copy `TREATMENT_ID/PATIENT_NAME/TREATMENT_CODE` từ item
- `inStock.AvailableAmount -= replaceMedicine.YCD_AMOUNT` → tồn kho giảm dần cho BN sau
- `RemoveAll` theo `(REPLACE_MEDICINE_TYPE_ID, TREATMENT_ID)` (thay vì chỉ theo loại)

**h) Manual Replace (`ReplaceMedicineSave` / `ReplaceMaterialSave`)** — khi `isSplitByPatient = true`:
```csharp
replaceMedicine.TREATMENT_ID = focusMedicine.TREATMENT_ID;
replaceMedicine.PATIENT_NAME = focusMedicine.PATIENT_NAME;
replaceMedicine.TREATMENT_CODE = focusMedicine.TREATMENT_CODE;
replaceMedicine.Requests = focusMedicine.Requests; // chỉ Requests của BN đó
medicineAdos.RemoveAll(o => o.REPLACE_MEDICINE_TYPE_ID == focusMedicine.MEDICINE_TYPE_ID
    && o.TREATMENT_ID == focusMedicine.TREATMENT_ID
    && !o.IsApproved);
```

**i) Validation khi edit YCD thay thế** (`CellValueChanged`) — có null check để tránh NRE:
```csharp
var req = isSplitByPatient
    ? medicineAdos.FirstOrDefault(o => !o.IsReplace
        && o.MEDICINE_TYPE_ID == data.REPLACE_MEDICINE_TYPE_ID
        && o.TREATMENT_ID == data.TREATMENT_ID)
    : medicineAdos.FirstOrDefault(o => !o.IsReplace
        && o.MEDICINE_TYPE_ID == data.REPLACE_MEDICINE_TYPE_ID);

if (data.YCD_AMOUNT <= 0) { valid = false; message = "Số lượng duyệt phải lớn hơn 0"; }
else if (req == null)     { valid = false; message = "Không tìm thấy dòng yêu cầu tương ứng"; }
else if (data.YCD_AMOUNT > (req.AMOUNT - req.CURRENT_DD_AMOUNT)) { ... }
// Update req chỉ khi req != null
if (req != null) { req.YCD_AMOUNT = ...; req.CURRENT_YC_AMOUNT = data.YCD_AMOUNT; }
```

**j) Save payload** — **KHÔNG ĐỔI**: `MakeMedicine`/`MakeMaterial` vẫn lặp `ADO.Requests` và tạo 1 SDO cho mỗi `HIS_EXP_MEST_METY_REQ`/`HIS_EXP_MEST_MATY_REQ` với `TreatmentId = req.TREATMENT_ID`. Khi split, `ADO.Requests` chỉ chứa req của 1 BN → SDO tự đúng. Chỉ điều chỉnh match request trong phần replace để lookup theo `TREATMENT_ID` khi split.

### 6.5. Đa ngôn ngữ (Resources/)

Plugin **chưa có** folder Resources trước khi sửa. Tạo mới:

| File | Nội dung |
|---|---|
| `Resources/ResourceLanguageManager.cs` | Class holder `ResourceManager LanguageResource` |
| `Resources/Lang.vi.resx` | Caption các cột + text "Tách theo bệnh nhân:" + tiêu đề form |
| `Resources/Lang.en.resx` | Bản English tương ứng |

Hàm `SetCaptionByLanguageKey()` được gọi ở đầu `Load`, đọc value qua `Inventec.Common.Resource.Get.Value`.

### 6.6. Lưu trạng thái checkbox (ControlState)

Dùng `HIS.Desktop.Library.CacheClient.ControlStateWorker`:

| Field | Giá trị |
|---|---|
| `MODULE_LINK` | `"HIS.Desktop.Plugins.ApproveExpMestBCS"` |
| `KEY` | `"chkSplitByPatient"` |
| `VALUE` | `"1"` (CHECK) hoặc `""` (UNCHECK) |

`InitControlState` đọc khi Load với flag `isNotLoadWhileChangeControlStateInFirst = true` để tránh `CheckedChanged` fire sai. `SaveControlState` ghi lại sau mỗi lần user toggle.

### 6.7. Reference bổ sung trong csproj

```xml
<Reference Include="HIS.Desktop.Library.CacheClient">
  <HintPath>..\..\..\..\histest\x64\ReferencedAssemblies\HIS.Desktop.Library.CacheClient.dll</HintPath>
</Reference>
<Reference Include="Inventec.Common.Resource">
  <HintPath>..\..\..\..\LIB\Inventec.Common\Inventec.Common.Resource\Inventec.Common.Resource.dll</HintPath>
</Reference>
```

Và đăng ký Resources:
```xml
<Compile Include="Resources\ResourceLanguageManager.cs" />
<EmbeddedResource Include="Resources\Lang.vi.resx" />
<EmbeddedResource Include="Resources\Lang.en.resx" />
```

---

## 7. Nghiệp vụ thay thế thuốc/vật tư

### 7.1. TH1 — Auto Replace

**Điều kiện bật**: `MOS.HIS_EXP_MEST.BCS.APPROVE_OTHER_TYPE.IS_ALLOW = 1` **AND** `HIS.HIS_EXP_MEST.BCS.APPROVE.IS_AUTO_REPLACE = 1`.

- Trạng thái checkbox đã lưu là **CHECK** + mở form: tách BN trước → load replace theo từng BN (BN cần ít duyệt được ưu tiên cấp tồn trước, tồn giảm dần).
- Trạng thái **UNCHECK**: giữ logic replace như hiện tại (1 replace row per loại thuốc).

### 7.2. TH2 — Manual Replace

**Điều kiện bật**: `MOS.HIS_EXP_MEST.BCS.APPROVE_OTHER_TYPE.IS_ALLOW = 1`.

- User thay A bằng B khi đang UNCHECK → sau đó CHECK: YCD của row B phân bổ cho các BN yêu cầu A theo thứ tự `(AMOUNT - DD_AMOUNT)` tăng dần, có thể sinh nhiều row B (một row/BN).
- User thay trên row đã tách BN (đang CHECK): row B copy `TREATMENT_ID`, `PATIENT_NAME`, `TREATMENT_CODE` từ row được thay.

---

## 8. Edge cases & ghi chú kỹ thuật

| Tình huống | Xử lý |
|---|---|
| Tất cả req có `TREATMENT_ID = null` | `HasAnyTreatmentId()` trả `false` → không tách, 2 cột vẫn ẩn dù checkbox CHECK (theo spec "hiển thị như hiện tại") |
| Một phần req không có `TREATMENT_ID` | `GroupBy(r => r.TREATMENT_ID)` null thành 1 nhóm riêng → 1 row với 2 cột BN để trống |
| Tồn kho không đủ cho tất cả BN khi split | Phân bổ cumulative từ BN cần ít nhất; BN sau thấy `AVAIL_AMOUNT` = tồn còn lại |
| Toggle CHECK ↔ UNCHECK nhiều lần | YCD được snapshot và gộp về theo loại → user không mất input |
| Row thay thế chưa có request BN tương ứng | Giữ nguyên row replace không split (PATIENT_NAME trống) |
| Save payload | Luôn là per-request SDO, `TreatmentId` lấy từ `req.TREATMENT_ID` — không quan tâm trạng thái checkbox |
| DevExpress column `Visible` vs `VisibleIndex` | Gán `VisibleIndex >= 0` tự bật `Visible = true` → 4 cột split không set `VisibleIndex` trong Designer, chỉ gán runtime |
| `LoadTreatmentDict` khi không có ID | Dict rỗng, không gọi API — tiết kiệm round-trip |

---

## 9. Files thay đổi

| File | Loại | Mô tả |
|---|---|---|
| `ADO/MedicineTypeADO.cs` | Sửa | +3 property `TREATMENT_ID`, `PATIENT_NAME`, `TREATMENT_CODE` |
| `ADO/MaterialTypeADO.cs` | Sửa | +3 property tương tự |
| `frmApproveExpMestBCS.Designer.cs` | Sửa | +`chkSplitByPatient` + `lciSplitByPatient` + 4 grid columns |
| `frmApproveExpMestBCS.cs` | Sửa lớn | +fields, `LoadTreatmentDict`, `InitControlState`/`SaveControlState`, `Split/Group*AdosByPatient/Type`, `SetCaptionByLanguageKey`, chỉnh `LoadDataAutoReplace`, `ReplaceMedicineSave`/`ReplaceMaterialSave`, `CellValueChanged`, `MakeMedicine`/`MakeMaterial` |
| `Resources/Lang.vi.resx` | Tạo mới | Caption UI tiếng Việt |
| `Resources/Lang.en.resx` | Tạo mới | Caption UI tiếng Anh |
| `Resources/ResourceLanguageManager.cs` | Tạo mới | Holder ResourceManager |
| `HIS.Desktop.Plugins.ApproveExpMestBCS.csproj` | Sửa | +Reference `HIS.Desktop.Library.CacheClient`, `Inventec.Common.Resource`; +Compile/EmbeddedResource Resources |
