# MPS000160 — Bảng kê ngoại trú viện phí — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Mã in | MPS000160 |
| Project | `MPS.Processor.Mps000160` + `MPS.Processor.Mps000160.PDO` (bản đang dùng — **không** phải bản cũ `MPS\MPS\Old\Core\Mps000160`) |
| Loại | MPS Processor (template Excel FlexCel) |
| Template mẫu | `Mps000160_BangKeNgoaiTru_VienPhi___A4D.xlsx`, `Mps000160_BangKeNgoaiTru_VienPhi__HaoPhi___A4D.xlsx` |
| Mục đích | In bảng kê chi phí khám chữa bệnh ngoại trú cho bệnh nhân **viện phí** (dịch vụ `PATIENT_TYPE__FEE`), nhóm theo loại dịch vụ BHYT, kèm phụ phí (PTTK 2656) |
| Nơi gọi | `HIS.Desktop.Plugins.Library.PrintBordereau` → `MpsBehavior\Mps000160\Mps000160Behavior.cs`; mã in khai báo trong `PrintTypeCodeWorker` của Bordereau, DepositServiceKiosk, TransactionBill, TransactionBillKiosk, TransactionDebt, TransactionDebtCollect |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

`Mps000160Behavior` dựng `Mps000160PDO(DepartmentTrans, TreatmentFees, HeinServiceTypeCFG, PatientTypeCFG, TransactionTypeCFG, SereServs, Transactions, Treatment, HeinServiceTypes, Rooms, Services, MaterialTypes, SingleKeyValue)` → `PrintCustomShow<Mps000160PDO>` → `Mps000160Processor.ProcessData()`:

```
ReadTemplate
→ DataInputProcess()          [Mps000160ProcessorPlus.cs]
   ├─ patientADO = DataRawProcess.PatientRawToADO(Treatment)   (tuổi tính bằng AgeUtil)
   ├─ SereServs → SereServADO(r, HeinServiceTypes, Services, Rooms, MaterialTypes)
   ├─ SereServFeePayment(): lọc AMOUNT > 0, PATIENT_TYPE_ID = FEE, IS_NO_EXECUTE ≠ 1, IS_EXPEND ≠ 1
   │    group (SERVICE_ID, VIR_PRICE, IS_EXPEND) → cộng AMOUNT / VIR_TOTAL_* 
   └─ OrderBy SERVICE_NAME
→ HeinServiceTypeProcess()    group theo HEIN_SERVICE_TYPE_ID (không có → "Khác"), cộng tổng 3 cột tiền
→ ProcessSingleKey()          [Mps000160Processor.cs] key BHYT/đúng tuyến, khoa, phòng khám, tổng tiền, tạm ứng,
                              đã thu/chưa thu/hoàn, phụ phí (TOTAL_SURCHARGE, SURCHARGE_SECTION_*), số phim
→ SetBarcodeKey()             TREATMENT_CODE_BAR
→ singleTag / barCodeTag / objectTag("HeinServiceType" ⊃ "Service" theo TDL_HEIN_SERVICE_TYPE_ID, "Surcharge")
→ user function ReplaceValue (đổi ';' → '/')
```

### Điều kiện nghiệp vụ
- `sereServADOs` rỗng ⇒ `ProcessData` trả `false` (không in).
- **Đây là bảng kê HAO PHÍ** (`PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI__HAO_PHI` / `NOI_TRU_VIENPHI__HAO_PHI`): chỉ lấy dòng `IS_EXPEND = 1` của bệnh nhân **viện phí**; giá/tiền dùng cột `VIR_PRICE_NO_EXPEND` / `VIR_TOTAL_PRICE_NO_EXPEND` (template `__HaoPhi` dùng đúng 2 tag này). Tổng `TOTAL_PRICE`, `HeinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE` cộng theo `VIR_TOTAL_PRICE_NO_EXPEND`; BHYT/BN trả cộng `VIR_TOTAL_HEIN_PRICE` / `VIR_TOTAL_PATIENT_PRICE` (theo Old\Core\Mps000160RDO). Method `SereServBHYTCoPayment()` có sẵn nhưng **không được gọi**.
- Template `Mps000160_BangKeNgoaiTru_VienPhi___A4D.xlsx` (bản không hậu tố HaoPhi) dùng tag `Service.VIR_PRICE` / `VIR_TOTAL_PRICE` — với dòng hao phí các cột này có thể = 0; nếu viện dùng mẫu đó cần đổi tag sang `*_NO_EXPEND`.
- `PRICE_CO_PAYMENT` trong `SereServADO`: không có `HEIN_LIMIT_PRICE` → bằng `VIR_PRICE` nếu là vật tư stent (`HIS_MATERIAL_TYPE.IS_STENT = 1`), ngược lại 0; có trần → `VIR_PRICE - HEIN_LIMIT_PRICE`; nếu vẫn 0 và không có tiền BHYT → `VIR_PRICE`.
- Tổng tiền (`TOTAL_PRICE`, `TOTAL_PRICE_PATIENT`) đã **cộng thêm phụ phí** `SurchargePayforms` (PTTK 2656).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TREATMENT | View | Thông tin điều trị, bệnh nhân (TDL_*), thời gian vào/ra |
| HIS_SERE_SERV | Table | Dòng dịch vụ → `SereServADO` |
| V_HIS_SERVICE, HIS_HEIN_SERVICE_TYPE, V_HIS_ROOM, HIS_MATERIAL_TYPE | View/Table | Enrich tên DV, loại BHYT, phòng thực hiện, stent |
| V_HIS_TREATMENT_FEE | View | Tổng tiền/tạm ứng/kết chuyển |
| HIS_TRANSACTION | Table | Tiền đã thanh toán (TRANSACTION_TYPE__BILL, ≤ OUT_TIME) |
| V_HIS_DEPARTMENT_TRAN | View | Khoa vào/khoa kết thúc |
| HIS_TRANSACTION_PAYFORM | Table | Phụ phí (`SurchargePayforms`) |

## 4. Cấu Trúc File

| File | Nội dung |
|------|----------|
| `Mps000160Processor.cs` | `ProcessData`, `ProcessSingleKey`, `SetBarcodeKey`, `SurchargeProcess`, `ReplaceValueFunction` |
| `Mps000160ProcessorPlus.cs` | `DataInputProcess`, `SereServFeePayment`, `SereServBHYTCoPayment` (không gọi), `HeinServiceTypeProcess` |
| `DataRawProcess.cs` | `PatientRawToADO`, `PatyAlterBHYTRawToADO`, `SetHeinCardNumberDisplayByNumber` |
| `AgeUtil.cs` | `CalculateFullAge(long dob)` → "n Tuổi/Tháng/ngày/Giờ" |
| `Mps000160ExtendSingleKey.cs` | Hằng tên key template |
| `ADO\SereServKey.cs` | `: HIS_SERE_SERV` + cột enrich (SERVICE_*, HEIN_SERVICE_TYPE_*, EXECUTE_ROOM_*, NUMBER_OF_FILM, PRICE_CO_PAYMENT, TOTAL_*) |
| `ADO\SereServADO.cs` | `: SereServKey`, ctor 5 tham số map từ `HIS_SERE_SERV` + danh mục |
| `ADO\PatientADO.cs`, `ADO\PatyAlterBhytADO.cs`, `ADO\HeinServiceTypeADO.cs`, `ADO\SurchargeADO.cs` | ADO object tag / single key |
| PDO: `Mps000160PDO.cs`, `SingleKeyValue.cs`, `Config\*CFG.cs` | Dữ liệu vào từ plugin; `Mps000160PDO_PublicKey.cs` là file **cũ không compile** (không nằm trong csproj) |

Namespace ADO: `MPS.Processor.Mps000160.ADO` (processor). Bản DLL 04/2025 đặt `PatientADO/PatyAlterBhytADO/HeinServiceTypeADO/SereServKey` trong PDO; PDO viết lại 07/2026 đã bỏ, nên từ 26/08/2026 các lớp này nằm trong processor (giống MPS000162) — PDO không cần đổi.

## 5. Build trên máy backup

- `/p:BuildProjectReferences=false` + `ReferencePath=HIS.Desktop\bin\Debug\ReferencedAssemblies`; ProjectReference PDO/ProcessorBase resolve từ `bin\Debug` của từng project (PDO 23/07/2026 14 848 byte, ProcessorBase 06/08/2026 91 648 byte).
- Output: `bin\Debug\MPS.Processor.Mps000160.dll` → deploy `x64\Plugins\MpsProcessor` (PDO giữ nguyên).

## 6. Changelog

| Ngày | Nội dung |
|------|----------|
| 26/08/2026 (v2) | Sửa lỗi in không ra hao phí: `SereServFeePayment` lọc `IS_EXPEND == 1` (DLL 04/2025 lọc `!= 1` — sai với mục đích mẫu), nhóm theo `VIR_PRICE_NO_EXPEND`, cộng thêm `VIR_TOTAL_PRICE_NO_EXPEND`; `HeinServiceTypeProcess` và `thanhtien_tong` (Processor.cs) cộng theo `VIR_TOTAL_PRICE_NO_EXPEND`. Căn cứ: Old\Core\Mps000160RDO + Mps000162 + tag template `__HaoPhi`. |
| 26/08/2026 | Khôi phục 3 file mất (`AgeUtil.cs`, `Mps000160ProcessorPlus.cs`, `ADO\SereServADO.cs`) + tách `SereServKey`, `PatientADO`, `PatyAlterBhytADO`, `HeinServiceTypeADO` sang `ADO\` của processor (trước nằm trong PDO đã bị bỏ). Tái tạo từ IL của DLL gốc 16/04/2025 (ildasm) đối chiếu MPS000122/MPS000162; logic không đổi. csproj thêm 4 Compile; `DataRawProcess.cs` thêm `using MPS.Processor.Mps000160.ADO;`. Build OK. |
| 12/06/2026 | PTTK 2656 — phụ phí: `SurchargeADO`, `SurchargeProcess`, key `TOTAL_SURCHARGE*`, `SURCHARGE_*`; tổng tiền cộng phụ phí. |
