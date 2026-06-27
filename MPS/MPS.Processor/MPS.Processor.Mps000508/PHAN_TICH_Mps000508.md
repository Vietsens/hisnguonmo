# Phân tích MPS.Processor.Mps000508 (Bảng kê chi phí KCB BHYT theo QĐ 697/QĐ-BYT)

> Tài liệu đọc-hiểu để sửa nhanh. Đọc file này thay vì đọc lại toàn bộ code.
> Phạm vi: processor `Mps000508` dùng cho **cả 2 mã in**: `MPS000508` (ngoại trú) và `MPS000509` (nội trú).
> Cập nhật lần cuối theo code nhánh `Khainq` (PTTK 2689 — QĐ 697/QĐ-BYT).

---

## 0. TL;DR (đọc nhanh nhất)

- **MPS000508 = Bảng kê BHYT NGOẠI TRÚ** theo QĐ 697. **MPS000509 = Bảng kê BHYT NỘI TRÚ** theo QĐ 697.
- **Cả 2 mã chạy CHUNG 1 DLL**: `Mps000508Behavior` + `Mps000508Processor` + PDO `Mps000508PDO`. 509 "reuse" 508 (giống 280 reuse 279, 282 reuse 281).
- **Khác nhau giữa 508/509 KHÔNG nằm ở processor** mà ở **file template** (mỗi mã in trỏ template riêng) + **điều kiện trên template**.
- Processor **đọc thẳng từ hồ sơ thật** (1 đợt điều trị theo `Treatment.ID`) → **giá trị key luôn đúng**, bất kể in bằng 508 hay 509.
- Chỉ lấy **phần BHYT** (`PATIENT_TYPE = BHYT`, `PRICE_BHYT > 0`). Không lấy viện phí (viện phí là MPS000510/511).
- "Nối thẻ BHYT": có gom nhiều kỳ thẻ/đối tượng BHYT **trong cùng 1 đợt** qua `KEY_PATY_ALTER`. KHÔNG nối nhiều đợt.

---

## 1. Định danh & vị trí

| Thành phần | Đường dẫn |
|---|---|
| Processor (MPS-side, build data ra key) | `MPS/MPS.Processor/MPS.Processor.Mps000508/` |
| PDO (Print Data Object — chứa data đầu vào) | `MPS/MPS.Processor/MPS.Processor.Mps000508.PDO/` |
| Behavior (HIS-side, gom data + gọi print) | `HIS/Plugins/HIS.Desktop.Plugins.Library.PrintBordereau/MpsBehavior/Mps000508/Mps000508Behavior.cs` |
| Bộ điều phối in (chọn mã → behavior) | `HIS/Plugins/HIS.Desktop.Plugins.Library.PrintBordereau/PrintBordereauProcessor.cs` |
| Khai báo mã in | `.../PrintBordereau/Base/PrintTypeCodeWorker.cs` |
| Form gọi in (UI) | `HIS/Plugins/HIS.Desktop.Plugins.Bordereau/` (form `frmBordereau`) |

Mã in (PrintTypeCodeWorker.cs:94-98 — **so sánh phân biệt HOA/thường**, phải đúng "MPS000..." viết hoa):
```
NGOAI_TRU_BHYT__697 = "MPS000508"   → NGOẠI TRÚ
NOI_TRU_BHYT__697   = "MPS000509"   → NỘI TRÚ
NGOAI_TRU_VIEN_PHI__697 = "MPS000510"
NOI_TRU_VIEN_PHI__697   = "MPS000511"
BANG_KE_697_TONG_HOP    = "MPS000512"
```

---

## 2. Luồng tổng thể (end-to-end)

```
frmBordereau (UI)
  → PrintBordereauProcessor.InitMenuPrint()        // dựng menu các bảng kê
  → người dùng chọn 1 mã in (508 / 509)
  → RunPrint(mpsCode)
      → RichEditorStore.RunPrintTemplate(mpsCode, DelegateRunPrinter)
           // framework TẢI FILE TEMPLATE đăng ký cho mpsCode (508 và 509 có template riêng)
           // rồi callback:
      → DelegateRunPrinter(printCode, fileName)     // PrintBordereauProcessor.cs:285
           → switch(printCode)
               case "MPS000508": loadMps = new Mps000508Behavior(...)             // :640
               case "MPS000509": printCode = "MPS000508";                         // :642-645
                                 loadMps = new Mps000508Behavior(..., pt509.NAME) // reuse!
           → loadMps.Load(printCode, fileName, returnEventPrint)
               → Mps000508Behavior.Load()           // gom data: API + BackendData
                   → new Mps000508PDO(...)           // đóng gói data
                   → PrintCustomShow<Mps000508PDO>.SignRun(...)
                       → Mps000508Processor.ProcessData()   // BƠM KEY vào template
                           → store.ReadTemplate(fileName)   // fileName = template của mã đã chọn
                           → DataInputProcess() / GroupDisplayProcess() / ProcessSingleKey() / ...
                           → singleTag/objectTag.ProcessData(store, ...)
```

**Điểm mấu chốt về 508 vs 509:**
- `fileName` (template) được resolve theo **mpsCode gốc** TRƯỚC khi vào `DelegateRunPrinter`. → 509 dùng template của 509.
- Trong switch, `printCode` của 509 bị **gán lại = "MPS000508"** (PrintBordereauProcessor.cs:643). Việc gán này **chỉ ảnh hưởng**: tra cứu tên máy in (`GlobalVariables.dicPrinter[printCode]`) và logic replace mã — **KHÔNG đụng template**.
- `documentName`: 509 truyền `pt509.PRINT_TYPE_NAME` (tên bản in nội trú); 508 truyền `""`.

→ **Kết luận:** chung code build data, **khác file template**. Muốn 509 ra đúng mẫu nội trú thì phải có template riêng cho MPS000509.

---

## 3. Cấu trúc file processor

### 3.1 Mps000508 (processor)
| File | Vai trò |
|---|---|
| `Mps000508Processor.cs` | Lõi: `ProcessData()`, `ProcessSingleKey()` (sinh single key), QR/barcode/ảnh, unique code, print log |
| `Mps000508ProcessorPlus.cs` | `DataInputProcess()` (lọc + gom SereServ), `GroupDisplayProcess()`, gom HeinServiceType / Bed / MedicineLine / PatyAlter |
| `Mps000508Processor.ExeRoom.cs` | Gom theo **khoa/phòng xử lý** (port từ Mps000512/304). Template không dùng thì vô hại |
| `DataRawProcess.cs` | Map raw → ADO: `PatientRawToADO`, `PatyAlterBHYTRawToADO`, tách số thẻ, tính KBCB time, tỷ lệ hưởng |
| `PatientTypeAlterProcessor.cs` | Sinh `KEY_PATY_ALTER` (khóa "nối thẻ") từ thuộc tính thẻ BHYT |
| `Mps000508ExtendSingleKey.cs` | Khai báo tên các single key (kế thừa `CommonKey`) |
| `AgeUtil.cs` | Tính tuổi |
| `ADO/SereServADO.cs` | **Quan trọng nhất**: 1 dòng dịch vụ + toàn bộ công thức tính tiền |
| `ADO/HeinServiceTypeExt.cs` | Hằng id nhóm "ảo": Thuốc-dịch truyền=123, VTYT=124, Giường=125, Gói VTYT=126 |
| `ADO/GroupDepartmentADO.cs`, `OtherSourceADO.cs`, `SurchargeADO.cs`, `HeinServiceTypeExt.cs` | ADO phụ cho gom khoa/phòng, nguồn khác, phụ phí |

### 3.2 Mps000508.PDO
| File | Vai trò |
|---|---|
| `Mps000508PDO.cs` | Khai báo toàn bộ data đầu vào + 4 constructor overload |
| `Config/HeinServiceTypeCFG.cs`, `PatientTypeCFG.cs`, `ServiceTypeCFG.cs` | Cấu hình id cố định truyền từ behavior |
| `HisConfigValue.cs` | 4 cờ HIS_CONFIG (xem mục 9) |
| `SingleKeyValue.cs` | Các giá trị đơn behavior gắn sẵn (tên khoa/phòng, tổng ngày, user trả KQ...) |

---

## 4. Dữ liệu đầu vào (Mps000508Behavior.Load)

Phạm vi: **1 đợt điều trị** = `this.Treatment.ID`. Behavior lấy data từ 2 nguồn:

**Gọi API (BackendAdapter):**
- `api/HisSereServExt/Get` — ext của các SereServ (số phim...)
- `api/HisPatientTypeAlter/Get` (theo `TREATMENT_ID`, order `LOG_TIME ASC`) — **toàn bộ kỳ thẻ BHYT của đợt** (data nối thẻ)
- `api/HisDiimType/Get` — loại CĐHA (đếm phim)
- `api/HisServiceReq/Get` — chỉ khi bật medicine line / group theo lần dùng

**Lấy BackendData (cache):**
- `HIS_BRANCH` (theo branch hiện tại), `HIS_TREATMENT_TYPE`, `HIS_MATERIAL_TYPE`, `HIS_DEPARTMENT`, `HIS_MEDI_ORG`, `HIS_OTHER_PAY_SOURCE`, `HIS_SERVICE_UNIT`, (tùy cờ) `HIS_MEDICINE_TYPE`, `HIS_MEDICINE_LINE`.

**Cấu hình id cố định gắn trong behavior:**
- `HeinServiceTypeCFG.HEIN_SERVICE_TYPE__EXAM_ID = ID__KH` (khám), `__HIGHTECH_ID = ID__DVKTC`.
- `PatientTypeCFG.PATIENT_TYPE__BHYT = PATIENT_TYPE_ID__BHYT`, `__FEE = ...IS_FEE`.

`SereServs`, `DepartmentTrans`, `TreatmentFees`, `Treatment`, `Patient`, `Rooms`, `Services`, `HeinServiceTypes`, `transReq`, `lstConfig`... được truyền sẵn từ `PrintBordereauProcessor`.

---

## 5. Luồng xử lý chính — ProcessData() (Mps000508Processor.cs:116)

Thứ tự gọi:
1. `store.ReadTemplate(fileName)` — nạp template.
2. **`DataInputProcess()`** (ProcessorPlus.cs:22) — lọc + gom dịch vụ:
   - Lọc SereServ: `AMOUNT > 0 && PATIENT_TYPE = BHYT && PRICE_BHYT > 0 && IS_NO_EXECUTE != 1 && IS_EXPEND != 1`.
   - Gom (`GroupBy`) theo: `SERVICE_ID, PRIMARY_PRICE, PRICE_BHYT, SERVICE_PAY_RATE, BHYT_PAY_RATE, IS_EXPEND, NUMBER_OF_FILM, KEY_PATY_ALTER, HEIN_SERVICE_TYPE_ID, STENT_ORDER` → cộng dồn `AMOUNT` và các cột tiền.
   - **STENT_ORDER > 1**: tính lại `TOTAL_PRICE_BHYT = quỹ BHTT + (BN cùng chi trả hoặc nguồn khác)` (đặc thù stent can thiệp tim mạch).
   - `ProcessOtherSource()` — gom nguồn chi trả khác.
   - CĐHA: đếm số phim theo `DIIM_TYPE` → `CDHACountList`.
   - `PatyAlterProcess()` — sinh danh sách đối tượng BHYT (xem 6.4).
3. **`GroupDisplayProcess()`** (ProcessorPlus.cs:149):
   - `HeinServiceTypeProcess()` — gom theo loại hình dịch vụ BHYT (xem 6.1).
   - Dồn các nhóm Giường (GI_NGT/GI_NT/GI_BN/GI_L) về `BED__ID` (125).
   - `MedicineLineProcesss()` — gom theo đường dùng thuốc.
   - `HeinServiceTypeBedProcess()` — nhóm con của giường/gói VTYT.
4. `ExeRoomProcess()` (file ExeRoom.cs) — gom theo khoa/phòng (template 697 thường KHÔNG dùng; an toàn nếu bỏ qua).
5. **`ProcessSingleKey()`** (Processor.cs:479) — sinh toàn bộ single key (xem mục 7).
6. `SetQrCode()` / `SetBarcodeKey()` / `SetTreatmentQrCodeBase()` / `SetImageKey()` — QR thanh toán, barcode mã ĐT, ảnh thẻ/avatar.
7. `ProcessPrintLogData()` — log "Mã điều trị: ...".
8. `SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()))` — số thứ tự bản in.
9. Nếu `sereServADOs` rỗng → **return false** (không in).
10. Đăng ký các bảng object + quan hệ vào template (mục 8).

---

## 6. Các bộ gom (object tables)

### 6.1 HeinServiceType (loại hình dịch vụ BHYT) — ProcessorPlus.cs:178
Gom `sereServADOs` theo `{HEIN_SERVICE_TYPE_ID, KEY_PATY_ALTER}`, cộng các cột tiền theo nhóm. Có xử lý đặc biệt **Gói VTYT** (gộp con theo `GOI_VT_Y_TE__ID`) và **Giường** (gộp về `BED__ID`).

### 6.2 HeinServiceTypeBed — ProcessorPlus.cs:314
Nhóm con dưới Giường (125) / Gói VTYT (126), theo `{HEIN_SERVICE_TYPE_ID, KEY_PATY_ALTER, MEDICINE_LINE_ID, HEIN_SERVICE_TYPE_PARENT_1_ID}`.

### 6.3 MedicineLine (đường dùng thuốc) — ProcessorPlus.cs:361
Chỉ có data khi bật cờ `IS_SHOW_MEDICINE_LINE`. Gom theo `{MEDICINE_LINE_ID, HEIN_SERVICE_TYPE_ID, KEY_PATY_ALTER}`, tính `REMEDY_COUNT` (số đợt cấp).

### 6.4 PatyAlterBHYT (đối tượng/kỳ thẻ BHYT) — ProcessorPlus.cs:403  ← "NỐI THẺ"
Gom `sereServADOs` theo `KEY_PATY_ALTER` → mỗi nhóm = **1 dòng đối tượng BHYT** + tổng tiền của đối tượng đó. Sắp theo `LOG_TIME`.
- `KEY_PATY_ALTER` do `PatientTypeAlterProcessor.ToString()` sinh ra = ghép: số thẻ | nơi ĐKBĐ | tuyến | đúng/trái tuyến | loại trái tuyến | 5 năm | đủ 6 tháng | khu vực sống | mã HN | hạn thẻ từ | hạn thẻ đến. → **đổi bất kỳ thuộc tính nào ⇒ tách dòng đối tượng mới** = cơ chế "nối/tách kỳ thẻ trong 1 đợt".
- `KBCB_TIME_FROM_STR/TO_STR` (DataRawProcess.cs:87-142): tính mốc thời gian KCB cho từng kỳ thẻ (dò kỳ thẻ kế tiếp `patientTypeAlterNext`).

### 6.5 ExeRoom (khoa/phòng) — ExeRoom.cs
Bộ gom `ServiceExeRoom / ServiceGroupByDepa / ServiceGroupByRoom / HeinServiceTypeExeRoom`. Dùng cho mẫu cần tách theo khoa/phòng. Mẫu 697 cơ bản không dùng.

### 6.6 Surcharge (phụ phí) — Processor.cs:456 (PTTK 2656)
Từ `rdo.SurchargePayforms` (lọc `SURCHARGE_AMOUNT > 0`). Cộng vào `thanhtien_tong` và `tongtienbenhnhantutra`.

---

## 7. Công thức tính tiền (ADO/SereServADO.cs) — CỐT LÕI

Mỗi `SereServADO` (constructor :56) tính:
- **Tỷ lệ** (`t`, :283-292): `t = 100 * Math.Round(giáBHYT_giới_hạn / (ORIGINAL_PRICE*(1+VAT)), 2)`. Làm tròn TỶ LỆ 2 số rồi ×100 (giống Mps000304), tránh lệch tiền tự trả DV vượt trần.
  - Thuốc/VTYT: `SERVICE_PAY_RATE = 100`, `BHYT_PAY_RATE = t`.
  - DV khác: `SERVICE_PAY_RATE = t`, `BHYT_PAY_RATE = 100`.
- `PRICE_BHYT` (:306, hàm `PriceBHYTProcess`): nếu có thẻ + `VIR_TOTAL_HEIN_PRICE > 0` → `ORIGINAL_PRICE*(1+VAT)` làm tròn; ngược lại 0.
- `TOTAL_PRICE_BHYT = PRICE_BHYT * AMOUNT * (BHYT_PAY_RATE/100) * (SERVICE_PAY_RATE/100)`.
- `PRIMARY_PRICE` (:309-325): giá theo BHYT (có VAT); nếu không bật chênh lệch & DV khám vượt → kẹp về `PRICE_BHYT`.
- `VIR_TOTAL_PRICE_NO_EXPEND = PRIMARY_PRICE * AMOUNT` (tổng theo giá BHYT).
- `TOTAL_PRICE_PATIENT_SELF` (:330) = phần tự trả trong phạm vi BHYT (kẹp ≥ 0). Có nhánh `IsSurgPriceOption_1` cho PT/TT.
- `TOTAL_PRICE_VP = VIR_PRICE * AMOUNT` (tổng theo giá viện phí).
- `TOTAL_PATIENT_PRICE_LEFT` (:344-351) = chênh lệch ngoài BHYT + tự trả trong BHYT (KHÔNG nhân tỷ lệ).
- Quy đổi đơn vị tính (:353-368) nếu service unit có `CONVERT_RATIO`.

Các cột tổng dùng cho key (Processor.cs:782-814): `thanhtienBH_tong`, `thanhtien_tong`, `bhytthanhtoan_tong`, `bnthanhtoan_tong`, `nguonkhac_tong`, `tongtienbenhnhantutra`, `tongTienBenhNhan`, `thanhtien_tong_new` (theo VP), `tongtienbenhnhantutra_new`.

---

## 8. Bộ KEY xuất ra template

### 8.1 Object tables + quan hệ (ProcessData, Processor.cs:171-229)
| Tag template | Nguồn |
|---|---|
| `HeinServiceType` | loại hình DV BHYT |
| `Service` | dòng dịch vụ chi tiết (`sereServADOs`) |
| `PatyAlterBHYT`, `PatyAlterBHYTAll` | đối tượng/kỳ thẻ BHYT |
| `MedicineLine` | đường dùng thuốc |
| `HeinServiceTypeBed` | nhóm con giường/gói VTYT |
| `CDHACountList` | đếm phim CĐHA |
| `Surcharge` | phụ phí |
| `OtherPaySource` | nguồn chi trả khác |
| `ServiceExeRoom`, `ServiceGroupByDepa`, `ServiceGroupByRoom`, `HeinServiceTypeExeRoom` | gom theo khoa/phòng |

Quan hệ chính (nối bằng `KEY`/`KEY_PATY_ALTER` và `ID`/`HEIN_SERVICE_TYPE_ID`):
`PatyAlterBHYT(KEY)` → Service / HeinServiceType / Bed / MedicineLine `(KEY_PATY_ALTER)`;
`HeinServiceType(ID)` → Service `(HEIN_SERVICE_TYPE_ID)`, Bed `(PARENT_ID)`, MedicineLine `(HEIN_SERVICE_TYPE_ID)`.

### 8.2 Single key tiêu biểu (ProcessSingleKey, Processor.cs:479)
- BN/đợt: object key của `PatientADO`, `V_HIS_TREATMENT`, `V_HIS_TREATMENT_FEE`, `SingleKeyValue`; `TREATMENT_CODE`, giới tính (`GENDER_INDEX/NAME`), khoa/phòng (`DEPARTMENT_NAME`, `END_DEPARTMENT_*`), giờ (`CLINICAL_IN_TIME_STR`, `OPEN/CLOSE_TIME_SEPARATE_STR`).
- BHYT: `IS_HEIN/IS_NOT_HEIN`, `HEIN_MEDI_ORG_CODE/NAME`, `HEIN_CARD_ADDRESS`, đúng tuyến (`RIGHT_ROUTE_TYPE_NAME[_CC/_TT]`, `NOT_RIGHT_ROUTE_TYPE_NAME`, `THONG_TUYEN`), `RATIO_STR`, hạn thẻ, 5 năm, `LIVE_AREA_CODE`, `CO_PAID_ACCUMULATE_AMOUNT`.
- Tổng tiền: `TOTAL_PRICE[_TEXT]`, `TOTAL_PRICE_BHYT`, `TOTAL_PRICE_HEIN[_TEXT]`, `TOTAL_PRICE_PATIENT[_TEXT]`, `TOTAL_PRICE_OTHER[_TEXT]`, `TOTAL_PRICE_PATIENT_SELF`, `TOTAL_PRICE_PATIENT_NO_PAY_RATE[_TEXT]`, `TOTAL_PRICE_PATIENT_ALL_697[_TEXT]`, phụ phí `TOTAL_SURCHARGE[_TEXT]`, tạm ứng/thanh toán (`TREATMENT_FEE_*`, `TOTAL_DEPOSIT_AMOUNT`).
- Phim: `TOTAL_NUMBER_FILM[_STR]`.

---

## 9. HIS_CONFIG ảnh hưởng (đọc trong Behavior — SdaConfigKey)

| Key cấu hình | Cờ trong code | Tác động |
|---|---|---|
| `...IS_PRICE_WITH_DIFFERENCE` | `IsPriceWithDifference` | Có tính chênh lệch giá DV khám hay kẹp về giá BHYT |
| `MOS.BHYT.CALC_MATERIAL_PACKAGE_PRICE_OPTION` | `IsNotSameDepartment` | Cách gộp VTYT trong gói theo phòng |
| `MOS.BHYT.CALC_ARISING_SURG_PRICE_OPTION` | `IsSurgPriceOption_1` | Cách tính tự trả cho PT/TT |
| `...Mps.IsGroupHeinServiceByUseTime` | `IsGroupHeinServiceByUseTime` | Gắn thẻ BHYT theo thời điểm dùng (USE_TIME) |
| `...Bordereau.IsShowMedicineLine` | (isShowMedicineLine) | Có hiển thị đường dùng thuốc / lấy ServiceReq |

---

## 10. Khác biệt 508 (NGOẠI TRÚ) vs 509 (NỘI TRÚ) — CHÍNH XÁC

> Một đợt chỉ thuộc **một** loại điều trị. Processor đọc từ hồ sơ thật nên **giá trị luôn đúng theo đợt**, không phụ thuộc mã in. Các key dưới đây đổi giá trị theo **loại điều trị thật** của hồ sơ, KHÔNG phải theo việc bấm 508 hay 509.

**Key PHÂN LOẠI (3 key, không phải 2):**
| Key | Ngoại trú | Nội trú | Nguồn |
|---|---|---|---|
| `TYPE_INDEX` | 2 | 3 | Processor.cs:511-553 (nếu tìm thấy trong `TreatmentTypes` thì = chính `TREATMENT_TYPE.ID`) |
| `TYPE_NAME` | "ĐIỀU TRỊ NGOẠI TRÚ" | "ĐIỀU TRỊ NỘI TRÚ" | nt |
| **`TYPE_INDEX_697`** | **"02"** | **"03"** | Processor.cs:402-432 (`01`=khám,`02`=ngoại trú,`03`=nội trú,`04`=ban ngày) ← **mẫu 697 dùng key này** |

**Key GIÁ TRỊ phụ thuộc loại điều trị:**
- `TREATMENT_DAY_COUNT_6556` — số ngày điều trị; chỉ set khi **không phải khám / nội trú** (Processor.cs:728-760). Ngoại trú trong ngày → trống.
- `KBCB_TIME_FROM_STR` / `KBCB_TIME_TO_STR` (mỗi `PatyAlterBHYT`) — có nhánh riêng cho nội trú (DataRawProcess.cs:91-142, dùng min/max instruction time với thẻ tỉnh + trái tuyến trước 2021).
- `RATIO_STR` (dòng đối tượng) — override riêng nội trú tỉnh/trái tuyến (ProcessorPlus.cs:426-432).
- Unique code / số TT bản in — `ProcessUniqueCodeData()` đổi `"Mps000508"→"Mps000509"` khi nội trú (Processor.cs:955-958).
- `KEY_PATY_ALTER` — thêm hậu tố `|true/false` cho nội trú tỉnh trái tuyến (PatientTypeAlterProcessor.cs:54-66) → ảnh hưởng cách tách dòng đối tượng.

**Phần DÙNG CHUNG, luôn đúng cho cả 2:** mọi tổng tiền (BHYT/BN/nguồn khác/tự trả), danh sách dịch vụ, gom loại hình DV, thuốc, giường, phụ phí, QR/barcode/ảnh.

**Làm bản nội trú đúng mẫu:**
1. Tạo template riêng cho **MPS000509**, đăng ký lên server in.
2. Trong template, đặt điều kiện theo **`TYPE_INDEX_697 = "03"`** (hoặc `TYPE_INDEX = 3`) cho các vùng đặc thù nội trú (số ngày, giường, khoa, lũy kế).
3. Nếu cần các trường mục Section11 / lũy kế bản `_697`: phải **bật lại** 2 dòng đang comment (xem mục 11).

---

## 11. ⚠️ Gotchas khi sửa

1. **2 hàm `_697` đang BỊ COMMENT** (Processor.cs:639-640):
   ```csharp
   //SetSingleKey697_Section11();        // → HEIN_PATIENT_TYPE_CODE_697, MEDI_ORG_NAME_697,
                                          //   TRANSFER_IN_MEDI_ORG_NAME_697/CODE_697 KHÔNG được set
   //SetSingleKey697_CoPaidAccumulate(); // → CO_PAID_ACCUMULATE_AMOUNT_697 / _STR_697 KHÔNG được set
   ```
   (Bản alias không hậu tố `CO_PAID_ACCUMULATE_AMOUNT[_STR]` vẫn có do set riêng tại Processor.cs:641-644.)
   → Template cần các key `_697` này thì phải bỏ comment.

2. **Mã in so sánh phân biệt HOA/thường**: phải đúng `"MPS000508"`/`"MPS000509"` viết hoa, khớp record `SAR_PRINT_TYPE` trong DB.

3. **509 reuse 508**: sửa `Mps000508Processor`/`Behavior` là **ảnh hưởng CẢ nội trú lẫn ngoại trú**. Test cả 2 mã.

4. **`GetDefaultHeinRatioForView` ghi log bằng `LogSystem.Error`** (DataRawProcess.cs:198) dù không phải lỗi — đừng nhầm khi đọc log.

5. **Rỗng = không in**: `ProcessData` return false khi `sereServADOs` rỗng (Processor.cs:137-138) → bảng kê không có DV BHYT hợp lệ sẽ không ra.

6. **ExeRoom** (file `.ExeRoom.cs`) là tính năng port thêm; nếu template không có tag tương ứng thì vô hại, nhưng vẫn chạy tốn CPU.

7. **Lọc cứng BHYT**: chỉ DV `PATIENT_TYPE = BHYT && PRICE_BHYT > 0`. DV viện phí/tự trả 100% không lên bảng kê này (đó là việc của MPS000510/511).

---

## 12. Kết luận

- `Mps000508Processor` là **bộ build dữ liệu chung** cho bảng kê BHYT theo QĐ 697, phục vụ **cả ngoại trú (508) và nội trú (509)**.
- Dữ liệu lấy theo **một đợt điều trị**, chỉ phần **BHYT**, có **nối nhiều kỳ thẻ trong đợt** qua `KEY_PATY_ALTER`.
- **Giá trị key luôn đúng theo hồ sơ thật**; phân biệt ngoại/nội trú trên giấy là việc của **template + điều kiện `TYPE_INDEX_697`**, không phải của processor.
- Khi sửa: nhớ 509 reuse 508 (test cả hai), và 2 hàm `_697` đang bị comment.
