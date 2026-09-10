# MPS000158 — Bảng kê ngoại trú BHYT (hao phí) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Mã in | MPS000158 |
| Project | `MPS.Processor.Mps000158` + `MPS.Processor.Mps000158.PDO` (bản đang dùng — **không** phải bản cũ `MPS\MPS\Old\Core\Mps000158`) |
| Loại | MPS Processor (template Excel FlexCel) |
| Template mẫu | `Mps000158_BangKeNgoaiTru_BHYT.xlsx`, `Mps000158_BangKeNgoaiTru_BHYT__HaoPhi___A4D.xlsx` |
| Mục đích | In bảng kê **hao phí** (`IS_EXPEND = 1`) của bệnh nhân **BHYT** theo loại dịch vụ BHYT: giá BHYT, BHYT trả, BN cùng trả, BN tự trả; kèm phụ phí (PTTK 2656) |
| Nơi gọi | `HIS.Desktop.Plugins.Library.PrintBordereau` → `MpsBehavior\Mps000158\Mps000158Behavior.cs`, mã in `PRINT_TYPE_CODE___NGOAI_TRU_BHYT__HAO_PHI` / `NOI_TRU_BHYT__HAO_PHI` (`PrintBordereauProcessor.cs`) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

`Mps000158Behavior` dựng `Mps000158PDO(CurrentPatyAlter, PatyAlters, DepartmentTrans, TreatmentFees, HeinServiceTypeCFG, PatientTypeCFG, SereServs, SereServExts, Treatment, HeinServiceTypes, Rooms, Services, TreatmentType, Branch, materialTypes, SingleKeyValue)` → `PrintCustomShow<Mps000158PDO>` → `Mps000158Processor.ProcessData()`:

```
ReadTemplate
→ DataInputProcess()          [Mps000158ProcessorPlus.cs]
   ├─ patientADO      = DataRawProcess.PatientRawToADO(Treatment)          (tuổi: AgeUtil)
   ├─ patyAlterBHYTADOs = DataRawProcess.PatyAlterBHYTRawToADOs(PatyAlters, Branch, TreatmentType, CurrentPatyAlter, Treatment)  (RATIO_STR = mức hưởng)
   ├─ SereServs → SereServADO(r, SereServExts, HeinServiceTypes, Services, Rooms, materialTypes, PatyAlters)
   │     PRICE_BHYT = PriceBHYTProcess (trần BHYT / giá gốc / stent), TOTAL_PRICE_BHYT, TOTAL_PRICE_PATIENT_SELF,
   │     TOTAL_HEIN_PRICE_ONE_AMOUNT, RADIO_SERIVCE, PRICE_CO_PAYMENT, NUMBER_OF_FILM (ưu tiên HIS_SERE_SERV_EXT mới nhất)
   ├─ lọc AMOUNT > 0, PATIENT_TYPE_ID = BHYT, IS_NO_EXECUTE ≠ 1, IS_EXPEND = 1
   │     group (SERVICE_ID, TOTAL_HEIN_PRICE_ONE_AMOUNT, IS_EXPEND, NUMBER_OF_FILM) → cộng AMOUNT / VIR_TOTAL_HEIN_PRICE / VIR_TOTAL_PATIENT_PRICE_BHYT / TOTAL_PRICE_BHYT / TOTAL_PRICE_PATIENT_SELF
   └─ OrderBy SERVICE_NAME
→ HeinServiceTypeProcess()    group theo HEIN_SERVICE_TYPE_ID (không có → "Khác"), 4 cột tổng (TOTAL_PRICE_BHYT, HEIN, PATIENT_BHYT, PATIENT_SELF)
→ ProcessSingleKey()          [Mps000158Processor.cs] key thẻ BHYT (nhiều thẻ nối "; "), đúng tuyến, khoa/phòng, tổng tiền
                              (TOTAL_PRICE = Σ TOTAL_PRICE_BHYT + phụ phí, TOTAL_PRICE_PATIENT_SELF = Σ TOTAL_PRICE_PATIENT_SELF + phụ phí), số phim, TreatmentFee
→ SetBarcodeKey()             TREATMENT_CODE_BAR
→ singleTag / barCodeTag / objectTag("HeinServiceType" ⊃ "Service" theo TDL_HEIN_SERVICE_TYPE_ID, "PatyAlterBHYT", "Surcharge")
→ user function ReplaceValue (đổi ';' → '/')
```

### Điều kiện nghiệp vụ
- `sereServADOs` rỗng ⇒ `ProcessData` trả `false` (không in).
- Chỉ dòng **hao phí** (`IS_EXPEND = 1`) của BN **BHYT**; không lọc `PRICE_BHYT > 0` (khác MPS000120 là bảng kê BHYT thường lọc `IS_EXPEND ≠ 1` và `PRICE_BHYT > 0`).
- `PRICE_BHYT` = 0 khi dòng không có tiền BHYT (`VIR_TOTAL_HEIN_PRICE` = 0) hoặc số thẻ không khớp `PatyAlters`.
- Template dùng cột `Service.PRICE_BHYT / TOTAL_PRICE_BHYT / VIR_TOTAL_HEIN_PRICE / VIR_TOTAL_PATIENT_PRICE_BHYT / TOTAL_PRICE_PATIENT_SELF`, `PatyAlterBHYT.HEIN_CARD_NUMBER / RATIO_STR / STR_HEIN_CARD_FROM_TIME / STR_HEIN_CARD_TO_TIME`.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TREATMENT | View | Thông tin điều trị, bệnh nhân (TDL_*) |
| HIS_SERE_SERV, HIS_SERE_SERV_EXT | Table | Dòng dịch vụ → `SereServADO`; EXT lấy số phim |
| HIS_PATIENT_TYPE_ALTER, V_HIS_PATIENT_TYPE_ALTER | Table/View | Danh sách thẻ BHYT → `PatyAlterBhytADO`; thẻ hiện tại để tính mức hưởng |
| HIS_BRANCH, HIS_TREATMENT_TYPE | Table | Tuyến CSKCB, loại điều trị → `GetDefaultHeinRatioForView` |
| V_HIS_SERVICE, HIS_HEIN_SERVICE_TYPE, V_HIS_ROOM, HIS_MATERIAL_TYPE | View/Table | Enrich tên DV, loại BHYT, phòng thực hiện, stent |
| V_HIS_TREATMENT_FEE, V_HIS_DEPARTMENT_TRAN, HIS_TRANSACTION_PAYFORM | View/Table | Tổng tiền/tạm ứng, khoa, phụ phí |

## 4. Cấu Trúc File

| File | Nội dung |
|------|----------|
| `Mps000158Processor.cs` | `ProcessData`, `ProcessSingleKey`, `SetBarcodeKey`, `SurchargeProcess`, `ReplaceValueFunction` |
| `Mps000158ProcessorPlus.cs` | `DataInputProcess`, `HeinServiceTypeProcess` |
| `DataRawProcess.cs` | `PatientRawToADO`, `PatyAlterBHYTRawToADOs`, `GetDefaultHeinRatioForView`, `SetHeinCardNumberDisplayByNumber` |
| `AgeUtil.cs` | `CalculateFullAge(long dob)` |
| `Mps000158ExtendSingleKey.cs` | Hằng tên key template |
| `ADO\SereServKey.cs` | `: HIS_SERE_SERV` + cột enrich (SERVICE_*, HEIN_SERVICE_TYPE_*, EXECUTE_ROOM_*, NUMBER_OF_FILM, PRICE_BHYT, TOTAL_PRICE_BHYT, TOTAL_PRICE_PATIENT_SELF, RADIO_SERIVCE, TOTAL_HEIN_PRICE_ONE_AMOUNT, PRICE_CO_PAYMENT, TOTAL_*) |
| `ADO\SereServADO.cs` | `: SereServKey`, ctor 7 tham số + `PriceBHYTProcess` |
| `ADO\PatientADO.cs`, `ADO\PatyAlterBhytADO.cs` (có `RATIO_STR`), `ADO\HeinServiceTypeADO.cs` (có `TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE`), `ADO\SurchargeADO.cs` | ADO object tag / single key |
| PDO: `Mps000158PDO.cs`, `SingleKeyValue.cs`, `Config\*CFG.cs` | Dữ liệu vào; `Mps000158PDO_PublicKey.cs` là file **cũ không compile** (không nằm trong csproj) |

Namespace ADO: `MPS.Processor.Mps000158.ADO` (processor). Bản DLL 04/2025 đặt `PatientADO/PatyAlterBhytADO/HeinServiceTypeADO/SereServKey` trong PDO; PDO viết lại 07/2026 đã bỏ, nên từ 27/08/2026 các lớp này nằm trong processor (giống MPS000160/162) — PDO không cần đổi.

## 5. Build trên máy backup

- `/p:BuildProjectReferences=false` + `ReferencePath=HIS.Desktop\bin\Debug\ReferencedAssemblies`; HintPath `MPS.ProcessorBase` trong csproj trỏ `..\Product\HIS\His.Desktop\1.48\...` không tồn tại → resolve qua ReferencePath. ProjectReference PDO lấy từ `MPS.Processor.Mps000158.PDO\bin\Debug` (23/07/2026, 14 848 byte).
- Output: `bin\Debug\MPS.Processor.Mps000158.dll` → deploy `x64\Plugins\MpsProcessor` (PDO giữ nguyên).

## 6. Changelog

| Ngày | Nội dung |
|------|----------|
| 27/08/2026 | Khôi phục 3 file mất (`AgeUtil.cs`, `Mps000158ProcessorPlus.cs`, `ADO\SereServADO.cs`) + tách `SereServKey`, `PatientADO`, `PatyAlterBhytADO`, `HeinServiceTypeADO` sang `ADO\` của processor (trước nằm trong PDO đã bị bỏ). Tái tạo từ IL của DLL gốc 16/04/2025 (ildasm) đối chiếu MPS000120; **logic không đổi** (IL gốc đã lọc `IS_EXPEND == 1`). csproj thêm 4 Compile; `DataRawProcess.cs` thêm `using MPS.Processor.Mps000158.ADO;`. |
| 12/06/2026 | PTTK 2656 — phụ phí: `SurchargeADO`, `SurchargeProcess`, key `TOTAL_SURCHARGE*`, `SURCHARGE_*`. |
