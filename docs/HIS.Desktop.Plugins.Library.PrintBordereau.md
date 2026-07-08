# PrintBordereau (Thư viện in bảng kê) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.Library.PrintBordereau |
| Loại | Library (thư viện in dùng chung) |
| Mục đích | In bảng kê chi phí KCB cho bệnh nhân (~39 mẫu Mps), gọi MPS Processor + template FlexCel |
| Trạng thái | Bảo trì |

Thư viện được nhúng vào các plugin tiêu thụ (Bordereau, AssignPrescription, AssignService, TransactionDebtCollect…). Form cha tạo `PrintBordereauProcessor` rồi gọi `Print()` / `InitMenuPrint()`.

## 2. Quy Trình Nghiệp Vụ

### Luồng in
```
Form cha → new PrintBordereauProcessor(roomId, roomTypeId, treatmentId, patientId, initData, reloadMenu)
  → Print(printTypeCode) / Print()
    → InitData() + LoadData()  (nạp SereServ, TreatmentFee, Transaction, Bill, Deposit, ...)
      → RunPrint(mpsCode) → DelegateRunPrinter(printCode, fileName)
        → switch(printCode) → tạo MpsXXXXXBehavior (kế thừa MpsDataBase, ILoad)
          → Behavior.Load() → build PDO (project MPS.Processor.MpsXXXXX.PDO)
            → PrintCustomShow → MPS Processor.ProcessData() → đổ vào template FlexCel
```

### Phụ phí hình thức thanh toán (PTTK 2656 — mục 4.2.8)
- Config `MOS.HIS_TRANSACTION.MULTI_PAYFORM` = 1 → sau danh sách dịch vụ, in thêm dòng phụ phí cho từng `HIS_TRANSACTION_PAYFORM` có `SURCHARGE_AMOUNT > 0`: **Tên = SURCHARGE_NAME | Số lượng = 1 | Thành tiền = SURCHARGE_AMOUNT**.
- Config ≠ 1 → **không đọc** `HIS_TRANSACTION_PAYFORM`; bảng kê giữ nguyên 100% như cũ (không gọi API, không thay đổi data/layout).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERE_SERV | Table | Dịch vụ đã thực hiện (dòng bảng kê) |
| V_HIS_TREATMENT / V_HIS_TREATMENT_FEE | View | Điều trị + tổng phí |
| HIS_TRANSACTION | Table | Giao dịch của điều trị (lấy ID để truy phụ phí) |
| **HIS_TRANSACTION_PAYFORM** | Table | **Hình thức thanh toán/phụ phí từng giao dịch** (mục 4.2.8) |

Quan hệ phụ phí: `HIS_TREATMENT → HIS_TRANSACTION (TREATMENT_ID) → HIS_TRANSACTION_PAYFORM (TRANSACTION_ID)`.

## 4. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Giao dịch của điều trị | api/HisTransaction/Get | MosConsumer | HisTransactionFilter (TREATMENT_ID, IS_CANCEL=false) |
| **Phụ phí theo giao dịch** | **api/HisTransactionPayform/Get** | MosConsumer | **HisTransactionPayformFilter (TRANSACTION_ID)** |

## 5. Cấu hình ảnh hưởng

| KEY | BẬT (= 1) | TẮT (≠ 1) |
|-----|-----------|-----------|
| MOS.HIS_TRANSACTION.MULTI_PAYFORM | Nạp + in dòng phụ phí (region "Surcharge") | Không đọc payform; bảng kê như cũ |

## 6. Triển khai mục 4.2.8

### Phần data (plugin — áp dụng cho TẤT CẢ mẫu, OFF-safe)
| File | Thay đổi |
|------|----------|
| `AppConfigKey.cs` | Thêm const `MULTI_PAYFORM` |
| `Base/MpsDataBase.cs` | Thêm property `List<HIS_TRANSACTION_PAYFORM> SurchargePayforms` |
| `PrintBordereauProcessorPlus.cs` | `IsMultiPayformEnabled()` + `LoadTransactionPayform()` (gọi cuối `LoadData()`, chỉ khi config bật) |
| `PrintBordereauProcessor.cs` | Trong `DelegateRunPrinter`: gắn `SurchargePayforms` vào mọi `loadMps` (1 điểm, áp cho cả 39 mẫu) |

### Phần render — mục "N. Phụ phí" (đã làm cho 2 mẫu tham chiếu 120/122)
Phụ phí hiển thị thành **1 nhóm như "1. Khám bệnh"/"2. Máu"**: 1 dòng header đậm + các dòng chi tiết, đặt **NGAY TRƯỚC "Tổng cộng"**; và **"Tổng cộng" gộp luôn phụ phí**.

**PDO** (mỗi mẫu): + property `public List<HIS_TRANSACTION_PAYFORM> SurchargePayforms`.
**Behavior** (plugin): + `rdo.SurchargePayforms = this.SurchargePayforms;` trước khi in.
**Processor** (mỗi mẫu):
- `SurchargeProcess()` build `List<SurchargeADO>` (STT, SURCHARGE_NAME, AMOUNT=1, SURCHARGE_AMOUNT) + `objectTag.AddObjectData(store,"Surcharge",...)`.
- **Gộp tổng**: `thanhtien_tong += totalSurcharge;` (cột Thành tiền) + cột "Người bệnh trả/tự trả" `+= totalSurcharge;` TRƯỚC khi `SetSingleKey(TOTAL_PRICE...)`.
- Single keys: `TOTAL_SURCHARGE`, `TOTAL_SURCHARGE_TEXT`, `SURCHARGE_COUNT`, `SURCHARGE_SECTION_NO`, `SURCHARGE_SECTION_LABEL` (= "N. Phụ phí" khi có, "" khi không).

### Template (2 dòng trước "Tổng cộng" + named range)
**Dòng HEADER** (đậm, clone từ dòng header nhóm `__HeinServiceType__`), ẩn khi không có phụ phí:
- Cột Nội dung: `<#if(<#SURCHARGE_SECTION_LABEL;>="";<#delete row>;<#SURCHARGE_SECTION_LABEL;>)>`
- Cột Thành tiền + Người bệnh trả: `<#TOTAL_SURCHARGE;>`

**Dòng CHI TIẾT** (clone từ dòng `<#Service.*>`):
| Cột | Tag |
|-----|-----|
| STT / Tên | `<#Surcharge.STT;>` , `<#Surcharge.SURCHARGE_NAME;>` |
| Số lượng | `<#Surcharge.AMOUNT;>` |
| Thành tiền | `<#Surcharge.SURCHARGE_AMOUNT;>` |
| Người bệnh trả/tự trả | `<#Surcharge.SURCHARGE_AMOUNT;>` |

#### ⚠️ BẮT BUỘC: Named Range `__Surcharge__` cho dòng CHI TIẾT
FlexCel định nghĩa band bằng named range `__TênDataset__`. Chỉ đặt tag là CHƯA ĐỦ — phải có:
```xml
<definedName name="__Surcharge__">Sheet1!$A$&lt;dòng chi tiết&gt;:$BN$&lt;dòng chi tiết&gt;</definedName>
```
- Dòng band đặt **NGOÀI** range `__HeinServiceType__` → band cấp gốc, render 1 lần/bản ghi (nếu nằm trong range master mà không có quan hệ → lọc 0 dòng → trống).
- **Mps000120**: header R31, chi tiết R32 → `__Surcharge__` = `Sheet1!$A$32:$BN$32`, "Tổng cộng" dời xuống R33.
- **Mps000122**: header R24, chi tiết R25 → `__Surcharge__` = `Sheet1!$A$25:$BN$25`, "Tổng cộng" dời xuống R26.
- Khi chèn dòng: nhớ renumber rows/cells, dời mergeCells, dời definedNames (Print_Area...) + dimension theo.

## 7. Lưu ý build (multi-repo)

Plugin tham chiếu `MPS.Processor.Mps000120.PDO.dll` / `Mps000122.PDO.dll` từ `LIB\MPSv2\MPS.PDO\`.
→ Thứ tự build: **(1)** build lại 2 project PDO + Processor MPS → **(2)** cập nhật DLL vào `LIB\MPSv2\MPS.PDO\` (hoặc trỏ HintPath sang `MPS\...\bin\Debug`) → **(3)** build plugin PrintBordereau.

## 8. PTTK 2724 — Render bảng kê PDF đính kèm HĐĐT VNPT (mục 3.3)

Sau khi tạo hóa đơn điện tử VNPT thành công, plugin thanh toán (TransactionBill / TransactionList)
cần **bảng kê chi tiết dạng PDF** để đính kèm lên cổng HĐĐT. Thư viện bổ sung 1 method render PDF
(base64) tái dùng mẫu MPS sẵn có. **KHÔNG đụng** method/menu in bảng kê 6556 giấy hiện tại — chỉ thêm mới.

> Phân tầng: thư viện CHỈ render PDF. SOAP đính kèm do `Library.ElectronicBill` + framework lo (mục 3.5).
> Config `MOS.HIS_TRANSACTION.AUTO_ATTACH_BORDEREAU_HDDT__VNPT` (giá trị = `printTypeCode`, VD Nam Định = `Mps000321`)
> do **plugin tiêu thụ** đọc rồi truyền `printTypeCode` xuống thư viện — thư viện không đọc config này.

### API mới
```csharp
public string RenderHddtBordereauToPdf(string printTypeCode, BordereauInitData initData = null)
// → trả PDF dạng base64 (null nếu thất bại). initData.HddtInfo đã set số HĐ + ngày xuất.
```

### Luồng render
```
TransactionBill → new PrintBordereauProcessor(roomId, roomTypeId, treatmentId, patientId, initData{HddtInfo}, null)
  → RenderHddtBordereauToPdf("Mps000321", initData)
    → InitData() + LoadData() + LoadTransactionView()
      → RichEditorStore.RunPrintTemplate(printTypeCode, DelegateRenderHddtToPdf)  (đồng bộ)
        → Mps000321Behavior.BuildPdo() → Mps000321PDO
        → MpsPrinter.Run(PreviewType.SaveFile, saveMemoryStream) → stream Excel (.xlsx)
        → Inventec.Common.FileConvert.Convert.ExcelToPdfUsingFlex(xlsx → pdf)
        → System.Convert.ToBase64String(pdf) → trả plugin
```

### File thay đổi (chỉ trong thư viện)
| File | Thay đổi |
|------|----------|
| `ADO/HddtInfoADO.cs` | **Mới** — `InvoiceNumOrder` (string), `InvoiceTime` (long?) |
| `ADO/BordereauInitData.cs` | + property nullable `HddtInfo` (HddtInfoADO) |
| `Base/MpsDataBase.cs` | + property `HddtInfo` (carry cho Processor + mọi behavior) |
| `PrintBordereauProcessorPlus.cs` | `InitData()` copy `data.HddtInfo` |
| `MpsBehavior/Mps000321/Mps000321Behavior.cs` | Tách `BuildPdo()` (Load tái dùng); để TODO forward `rdo.HddtInfo` (chờ mục 3.4) |
| `PrintBordereauProcessor.cs` | case Mps000321: forward `HddtInfo` vào behavior |
| `PrintBordereauProcessorHddt.cs` | **Mới** — `RenderHddtBordereauToPdf()` + callback render PDF |
| `*.csproj` | + ref `Inventec.Common.FileConvert` (convert Excel→PDF), + 2 Compile include |

### Lưu ý kỹ thuật
- `PreviewType.SaveFile` với template **Excel** xuất ra **.xlsx**, KHÔNG phải PDF → thư viện tự convert
  xlsx→PDF bằng `Inventec.Common.FileConvert.Convert.ExcelToPdfUsingFlex` (cùng engine FlexCel MPS dùng nội bộ).
- `RenderHddtBordereauToPdf` chạy đồng bộ (RunPrintTemplate tải template rồi gọi callback inline).

### ⚠️ Phụ thuộc chéo mục 3.4 (MPS — dev khác)
- `Mps000321PDO` **chưa có** property `HddtInfo`. Behavior đã forward sẵn dữ liệu nhưng để dòng
  `rdo.HddtInfo = this.HddtInfo;` dạng **TODO comment** trong `Mps000321Behavior.BuildPdo()`.
  Khi mục 3.4 thêm property vào PDO → bỏ comment 1 dòng → giá trị HDDT (số HĐ, ngày) mới hiển thị trên template.
- **Circular dependency**: `HddtInfoADO` đặt trong Library, nhưng `MPS.Processor.Mps000321.PDO.dll` (Library ref vào)
  KHÔNG thể ref ngược Library. Mục 3.4 nên đặt kiểu HddtInfo **trong assembly PDO** (hoặc dùng 2 property scalar
  `string`/`long?`) để tránh vòng lặp — KHÔNG dùng trực tiếp `HddtInfoADO` của Library trên PDO.

## 9. Lưu ý build (multi-repo) — bổ sung 2724

Như mục 7. Riêng 2724: thư viện thêm ref `Inventec.Common.FileConvert.dll` (đã có trong `LIB\Inventec.Common\`).
Phần render HDDT hiển thị đầy đủ chỉ khi mục 3.4 (Mps000321PDO + template HDDT) hoàn tất + rebuild DLL PDO.

## 10. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 05/06/2026 | dangth2 | PTTK 2656 mục 4.2.8: in dòng phụ phí từ HIS_TRANSACTION_PAYFORM khi config MOS.HIS_TRANSACTION.MULTI_PAYFORM bật. Phần data áp dụng mọi mẫu (OFF-safe); render hoàn chỉnh cho Mps000120 + Mps000122 (kèm nội trú reuse). |
| 08/06/2026 | dangth2 | PTTK 2724 mục 3.3: thêm `RenderHddtBordereauToPdf` render bảng kê ra PDF base64 (SaveFile→xlsx→FileConvert) để đính kèm HĐĐT VNPT; `HddtInfoADO` + `BordereauInitData.HddtInfo`; tách `Mps000321Behavior.BuildPdo()`. Forward xuống PDO để TODO (chờ mục 3.4). Không sửa luồng in 6556 giấy. |
| 12/06/2026 | dangth2 | PTTK 2656 mục 4.2.8: hoàn tất render dòng phụ phí cho **37/39 mẫu** bảng kê. 35 mẫu có code riêng (120,122,124,125,127,128,158,160,162,193,194,196,260,261,265,279,281,285,295,302,304,306,312,313,314,321,348,356,359,441,463,504,508,510,512) + 2 mẫu reuse PDO (249→120, 251→122). Mỗi mẫu: PDO `SurchargePayforms` + Behavior gán + Processor `SurchargeProcess()` + bind band `Surcharge` + cộng phụ phí vào tổng cộng + section key (`SURCHARGE_SECTION_LABEL` = "N. Phụ phí", `TOTAL_SURCHARGE`...). LOẠI TRỪ 224 (giấy phụ thu) + 446 (yêu cầu thanh toán) — biểu mẫu đặc thù, theo quyết định nghiệp vụ. OFF-safe tuyệt đối. |
| 04/07/2026 | dangth2 | **PTTK 2883 — mục 2: Mps000504 bổ sung keys gom nhóm theo khoa/phòng như MPS000304 (temp 6556)**: `Mps000504Behavior` thêm `LoadExeRoomInput(rdo)` — nạp input pipeline ExeRoom vào PDO (SereServs đã lọc `[fromDateReq, toDateReq]` trên `TDL_INTRUCTION_TIME`, SereServExts, PatientTypeAlterAlls, HeinServiceTypes, Services, Rooms, Departments, medicineTypes/MedicineLines/ServiceReqs theo config `IS_SHOW_MEDICINE_LINE`, Branch, TreatmentTypes, PatientTypeCFG, HisConfigValue, HisServiceUnit, ListOtherPaySource) — mirror `Mps000304Behavior`. MPS side: port pipeline ExeRoom từ Mps000304 sang `MPS.Processor.Mps000504` (ADO SereServADO/HeinServiceTypeADO/HeinServiceTypeExt/MedicineLineADO/GroupDepartmentADO/PatyAlterBhytADO/PatientADO + DataRawProcess + PatientTypeAlterProcessor + AgeUtil + `Mps000504ProcessorPlus.ExeRoom.cs`); PDO thêm partial `Mps000504PDO__Plus.cs` (`TreatmentView` + input lists + `HisConfigValue`/`PatientTypeCFG`/`HeinServiceTypeCFG`). Temp dùng được các key: `<#ReqExeDepaRoom.>`, `<#ReqExeRoom.>`, `<#HeinServiceTypeExeRoom.>`, `<#MedicineLineExeRoom.>`, `<#HeinServiceTypeBedExeRoom.>`, `<#ServiceExeRoom.>`, `<#PatyAlterBHYTExeRoom.>` + relationships như 304. OFF-safe: input ExeRoom null (behavior cũ) → xuất danh sách rỗng, biểu in phẳng như cũ. <br/> **Lưu ý build (multi-repo)**: rebuild `MPS.Processor.Mps000504.PDO` + `MPS.Processor.Mps000504`, copy DLL PDO mới vào `ReferencedAssemblies` (theo HintPath của PrintBordereau) / `LIB\MPSv2\MPS.PDO\` trước khi build HIS. |

## 11. Test Cases

### 2656 — Config TẮT (≠ 1) — regression
- [ ] In Mps000120/122 (và các mẫu khác): bảng kê **giống hệt** trước thay đổi; KHÔNG gọi api/HisTransactionPayform/Get.

### 2656 — Config BẬT (= 1)
- [ ] Giao dịch có payform `SURCHARGE_AMOUNT > 0` → in dòng phụ phí (Tên=SURCHARGE_NAME, SL=1, Thành tiền=SURCHARGE_AMOUNT) dưới danh sách DV.
- [ ] Nhiều phụ phí nhiều giao dịch → in đủ, sắp theo SORT_ORDER.
- [ ] Không có phụ phí > 0 → không in dòng nào (như cũ).
- [ ] Mẫu chưa thêm region "Surcharge" → in bình thường, không lỗi.

### 2883 — Mps000504 keys ExeRoom (temp 6556 theo khoa)
- [ ] Temp Mps000504 CŨ (chỉ dùng `<#SereServs.>`) → in như trước, không lỗi (regression).
- [ ] Temp mới dùng `<#ReqExeDepaRoom.>`/`<#ReqExeRoom.>`/`<#ServiceExeRoom.>`... → in ra chi phí gom nhóm theo khoa → phòng xử lý, giống bố cục 6556 (Mps000304).
- [ ] Lọc thời gian ở bảng kê rồi in Mps000504 → các band ExeRoom CHỈ chứa DV có `TDL_INTRUCTION_TIME` trong khoảng lọc; tổng nhóm khớp với vùng "CP theo ĐK lọc" phần BHYT.
- [ ] BN không có DV BHYT trong khoảng lọc → band ExeRoom rỗng, biểu vẫn in phần danh sách phẳng.
- [ ] Config `IS_SHOW_MEDICINE_LINE = 1` → band `MedicineLineExeRoom` tách Tân dược/Chế phẩm như 304.

### 2724 — Render PDF đính kèm HĐĐT
- [ ] `RenderHddtBordereauToPdf("Mps000321", initData)` trả về chuỗi base64 PDF hợp lệ (decode mở được).
- [ ] `printTypeCode` rỗng/null → trả null, ghi log Warn, KHÔNG crash.
- [ ] Luồng in bảng kê 6556 giấy (Print/InitMenuPrint) **không đổi** sau khi thêm method mới.
- [ ] Sau khi mục 3.4 xong (bỏ comment `rdo.HddtInfo`): PDF hiển thị "Kèm theo số hóa đơn: {N}" + "Ngày DD tháng MM năm YYYY".
