---
description: Print integration từ plugin — PrintTypeCode, RichEditorStore, MPS.MpsPrinter.Run, PDO. Áp dụng khi thêm chức năng in trong plugin
paths:
  - "HIS/Plugins/**"
  - "HIS/HIS.Desktop.Print/**"
---

# Print Integration — Plugin Gọi In

## 1. Luồng In Tổng Quan

```
Plugin button click
  → RichEditorStore.RunPrintTemplate(printCode, callback)
    → Callback: DeletegatePrintTemplate(printCode, fileName)
      → Build PDO (Print Data Object) từ API data
      → MPS.MpsPrinter.Run(new PrintData(printCode, fileName, pdo, previewType, printer))
        → MPS.ProcessorBase load template
        → Fill data vào template
        → Preview / Print / Export
```

---

## 2. Bước 1: Tạo Button In + RichEditorStore

```csharp
private void btnPrint_Click(object sender, EventArgs e)
{
    try
    {
        // Tạo RichEditorStore với SAR consumer (template server)
        Inventec.Common.RichEditor.RichEditorStore store =
            new Inventec.Common.RichEditor.RichEditorStore(
                ApiConsumers.SarConsumer,                  // SAR API consumer
                ConfigSystems.URI_API_SAR,                 // Template server URI
                Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                GlobalVariables.TemnplatePathFolder        // Local template cache path
            );

        // Gọi RunPrintTemplate với print code + callback
        store.RunPrintTemplate(
            PrintTypeCodeWorker.PRINT_TYPE_CODE__MPS000102,  // PrintTypeCode constant
            DeletegatePrintTemplate                           // Callback method
        );
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

---

## 3. Bước 2: Callback DeletegatePrintTemplate

```csharp
private bool DeletegatePrintTemplate(string printCode, string fileName)
{
    bool result = false;
    try
    {
        switch (printCode)
        {
            case PrintTypeCodeWorker.PRINT_TYPE_CODE__MPS000102:
                PrintMps000102(printCode, fileName, ref result);
                break;
            case PrintTypeCodeWorker.PRINT_TYPE_CODE__MPS000117:
                PrintMps000117(printCode, fileName, ref result);
                break;
            default:
                break;
        }
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
        result = false;
    }
    return result;
}
```

---

## 4. Bước 3: Build PDO + Gọi MpsPrinter

```csharp
private void PrintMps000102(string printTypeCode, string fileName, ref bool result)
{
    try
    {
        WaitingManager.Show();

        // 4a. Load data từ backend
        CommonParam param = new CommonParam();
        var filter = new HisServiceReqViewFilter();
        filter.ID = currentServiceReq.ID;
        var serviceReq = new BackendAdapter(param)
            .Get<List<V_HIS_SERVICE_REQ>>(
                "api/HisServiceReq/GetView",
                ApiConsumers.MosConsumer, filter, param)
            .FirstOrDefault();

        // 4b. Tạo PDO (Print Data Object) — mỗi Mps có PDO riêng
        MPS.Processor.Mps000102.PDO.Mps000102PDO pdo =
            new MPS.Processor.Mps000102.PDO.Mps000102PDO(
                treatment,       // V_HIS_TREATMENT
                serviceReq,      // V_HIS_SERVICE_REQ
                sereServList     // List<V_HIS_SERE_SERV>
            );

        // 4c. Lấy tên máy in từ config
        string printerName = "";
        if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
            printerName = GlobalVariables.dicPrinter[printTypeCode];

        // 4d. (Optional) Tạo EMR signature input
        Inventec.Common.SignLibrary.ADO.InputADO inputADO =
            new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor()
                .GenerateInputADOWithPrintTypeCode(
                    treatment.TREATMENT_CODE ?? "",
                    printTypeCode,
                    currentModuleBase.RoomId);

        WaitingManager.Hide();

        // 4e. Gọi MPS.MpsPrinter.Run
        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
        {
            // In trực tiếp (không preview)
            result = MPS.MpsPrinter.Run(
                new MPS.ProcessorBase.Core.PrintData(
                    printTypeCode, fileName, pdo,
                    MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow,
                    printerName)
                { EmrInputADO = inputADO });
        }
        else
        {
            // Hiện preview trước
            result = MPS.MpsPrinter.Run(
                new MPS.ProcessorBase.Core.PrintData(
                    printTypeCode, fileName, pdo,
                    MPS.ProcessorBase.PrintConfig.PreviewType.Show,
                    printerName)
                { EmrInputADO = inputADO });
        }
    }
    catch (Exception ex)
    {
        WaitingManager.Hide();
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

---

## 5. PrintTypeCode — Constants

### Khai báo trong plugin

```csharp
// File: PrintTypeCodeWorker.cs (trong Base/ hoặc root)
internal class PrintTypeCodeWorker
{
    internal const string PRINT_TYPE_CODE__MPS000102 = "Mps000102";
    internal const string PRINT_TYPE_CODE__MPS000117 = "Mps000117";
    internal const string PRINT_TYPE_CODE__MPS000174 = "Mps000174";
}
```

### Hoặc dùng PrintTypeCodeStore chung

```csharp
// HIS.Desktop.Print/PrintTypeCodeStore.cs — 100+ constants
PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102       // "Mps000102"
PrintTypeCodeStore.PRINT_TYPE_CODE__EXPORT_BLOOD__MPS000107
PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuDuTru__MPS000117
```

**Naming**: `"Mps000XXX"` (chữ M hoa, ps thường, 6 chữ số).

---

## 6. PDO — Print Data Object

Mỗi MPS Processor có PDO riêng trong namespace:
```
MPS.Processor.Mps000102.PDO.Mps000102PDO
MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO
MPS.Processor.Mps000503.PDO.Mps000503PDO
```

PDO constructor nhận các EFMODEL objects:
```csharp
new Mps000102PDO(treatment, serviceReq, sereServList)
new Mps000503PDO(treatment, serviceReq, sereServ)
```

Reference trong .csproj:
```xml
<Reference Include="MPS.Processor.Mps000102.PDO">
  <HintPath>..\..\..\..\LIB\MPSv2\MPS.PDO\MPS.Processor.Mps000102.PDO.dll</HintPath>
</Reference>
```

---

## 7. ƯU TIÊN: Print Libraries (HIS.Desktop.Plugins.Library.Print*)

**BẮT BUỘC kiểm tra 12 Print Libraries trước khi tự build PDO + gọi MpsPrinter.Run.**
Print Libraries đã đóng gói sẵn: load data, build PDO, gọi MpsPrinter, xử lý EMR — chỉ cần truyền đủ tham số đầu vào.

### 12 Print Libraries

| Library | Chức năng | Dùng cho |
|---------|----------|----------|
| `PrintPrescription` | In đơn thuốc (nội trú, ngoại trú, CLS) | Kê đơn, duyệt đơn |
| `PrintBordereau` | In phiếu thanh toán, hóa đơn | Viện phí, thu ngân |
| `PrintServiceReq` | In phiếu yêu cầu dịch vụ | Chỉ định DV, CLS |
| `PrintServiceReqTreatment` | In phiếu yêu cầu điều trị | Đăng ký khám, tiếp nhận |
| `PrintTreatmentFinish` | In giấy ra viện, chuyển viện | Ra viện, kết thúc điều trị |
| `PrintTreatmentEndTypeExt` | In biểu mẫu kết thúc mở rộng | Hồ sơ bệnh án |
| `PrintOtherForm` | In biểu mẫu khác (giấy hẹn, giấy nghỉ) | Các biểu mẫu phụ |
| `PrintTestTotal` | In kết quả xét nghiệm tổng hợp | Xét nghiệm |
| `PrintAggrExpMest` | In phiếu xuất tổng hợp | Kho dược |
| `PrintPublicMedicines` | In danh sách thuốc công | Thuốc công khai |
| `PrintPatientUpdate` | In cập nhật bệnh nhân | Tiếp nhận |
| `PrintSarFormData` | In biểu mẫu SAR | Báo cáo |

### Cách dùng — Truyền ĐỦ tham số khi khởi tạo

#### PrintPrescription

```csharp
// Constructor — truyền đủ data đơn thuốc + module
var printProc = new Library.PrintPrescription.PrintPrescriptionProcessor(
    outPatientPresResults,    // List<OutPatientPresResultSDO> — BẮT BUỘC: data đơn thuốc
    this.currentModule        // Module — BẮT BUỘC: context module
);
// Hoặc nội trú:
var printProc = new Library.PrintPrescription.PrintPrescriptionProcessor(
    inPatientPresResults,     // List<InPatientPresResultSDO> — data nội trú
    this.currentModule
);
// Hoặc với tùy chọn:
var printProc = new Library.PrintPrescription.PrintPrescriptionProcessor(
    outPatientPresResults,    // Data
    isNotShowTaken,           // bool — ẩn đơn đã cấp
    this.currentModule,       // Module
    true                      // callFromPrescription — gọi từ form kê đơn
);

// Set tùy chọn trước khi in
printProc.SetOutHospital(hasMediStockNhaThuoc);  // Có thuốc nhà thuốc?

// In
printProc.Print();                                              // Theo config mặc định
printProc.Print("Mps000118");                                   // PrintTypeCode cụ thể
printProc.Print("Mps000118", true);                             // In trực tiếp
printProc.Print("Mps000118", true, PreviewType.PrintNow);       // Full control
```

#### PrintBordereau

```csharp
// Constructor — truyền đủ context phòng, bệnh nhân, điều trị
var bordereauProc = new Library.PrintBordereau.PrintBordereauProcessor(
    this.currentModule.RoomId,      // long — BẮT BUỘC: phòng hiện tại
    this.currentModule.RoomTypeId,  // long — BẮT BUỘC: loại phòng
    treatmentId,                     // long — BẮT BUỘC: mã điều trị
    patientPrint.ID,                 // long — BẮT BUỘC: mã bệnh nhân
    null,                            // BordereauInitData — dữ liệu khởi tạo (nullable)
    null,                            // ReloadMenuOption — callback reload menu (nullable)
    GetDocmentSigned                 // Action<DocumentSignedUpdateIGSysResultDTO> — callback ký số
);

bordereauProc.IsActionButtonPrintBill = true;  // Hiện nút in hóa đơn
bordereauProc.Print("Mps000446", PrintOption.Value.PRINT_NOW, null);
```

#### PrintTreatmentFinish

```csharp
// Constructor — truyền treatment + room
var printFinish = new Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(
    treatment,                // HIS_TREATMENT — BẮT BUỘC: thông tin điều trị
    currentModule.RoomId      // long? — phòng hiện tại
);
// Hoặc với branch:
var printFinish = new Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(
    treatment,                // HIS_TREATMENT
    branch,                   // HIS_BRANCH — chi nhánh
    currentModule.RoomId
);
// Hoặc với serviceReq:
var printFinish = new Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(
    treatment,                // HIS_TREATMENT
    serviceReq,               // HIS_SERVICE_REQ — yêu cầu DV
    currentModule.RoomId
);

printFinish.Print("Mps000008");     // Giấy ra viện
printFinish.Print("Mps000010");     // Giấy chuyển viện
printFinish.Print("Mps000268");     // Giấy hẹn tái khám
```

#### PrintServiceReq

```csharp
// Constructor — truyền đủ data yêu cầu + điều trị + giường
var printSR = new Library.PrintServiceReq.PrintServiceReqProcessor(
    serviceReqResult,       // HisServiceReqListResultSDO — BẮT BUỘC: kết quả DV
    treatmentInfo,          // HisTreatmentWithPatientTypeInfoSDO — BẮT BUỘC: info điều trị
    bedLogs,                // List<V_HIS_BED_LOG> — lịch sử giường
    currentModule.RoomId    // long? — phòng
);
// Hoặc với PreviewType + callback ký số:
var printSR = new Library.PrintServiceReq.PrintServiceReqProcessor(
    serviceReqResult,
    treatmentInfo,
    bedLogs,
    currentModule.RoomId,
    PreviewType.Show,                             // PreviewType
    (result) => { OnDocumentSigned(result); }     // Action callback ký số
);

printSR.Print();
printSR.Print("Mps000102", true);
printSR.SaveNPrint();                  // Lưu + In
```

### Reference trong .csproj

```xml
<!-- Print Library DLL từ LIB -->
<Reference Include="HIS.Desktop.Plugins.Library.PrintPrescription">
  <HintPath>..\..\..\..\LIB\HIS\HIS.Desktop.Plugins.Library.PrintPrescription\HIS.Desktop.Plugins.Library.PrintPrescription.dll</HintPath>
</Reference>
<Reference Include="HIS.Desktop.Plugins.Library.PrintBordereau">
  <HintPath>..\..\..\..\LIB\HIS\HIS.Desktop.Plugins.Library.PrintBordereau\HIS.Desktop.Plugins.Library.PrintBordereau.dll</HintPath>
</Reference>
```

---

## 8. PreviewType — Chế Độ In

```csharp
MPS.ProcessorBase.PrintConfig.PreviewType.Show                   // Hiện preview
MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow               // In trực tiếp
MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog             // Preview dạng dialog
MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile               // Xuất file
MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow                // Preview + EMR
MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow     // Ký EMR + in
MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview // Ký EMR + preview
MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow             // Chỉ ký EMR
MPS.ProcessorBase.PrintConfig.PreviewType.EmrCreateDocument      // Tạo tài liệu EMR
```

Config từ `ConfigApplications.CheDoInChoCacChucNangTrongPhanMem`:
- `== 2` → PrintNow
- Khác → Show preview

---

## 9. Chọn Cách Nào

| Tình huống | Dùng | Lý do |
|-----------|------|-------|
| In đơn thuốc | `PrintPrescriptionProcessor` | Đã xử lý nội trú/ngoại trú/CLS |
| In phiếu thanh toán | `PrintBordereauProcessor` | Đã tích hợp hóa đơn + BHYT |
| In phiếu yêu cầu DV | `PrintServiceReqProcessor` | Đã build data từ SDO |
| In giấy ra viện | `PrintTreatmentFinishProcessor` | Đã xử lý nhiều mẫu |
| In phiếu xét nghiệm | `PrintTestTotalProcessor` | Đã tổng hợp kết quả |
| In biểu mẫu khác | `PrintOtherFormProcessor` | Các biểu mẫu chung |
| **Chưa có Library phù hợp** | `MPS.MpsPrinter.Run` trực tiếp | Tự build PDO (xem section 2-4) |

**Thứ tự ưu tiên**:
1. Tìm trong 12 Print Libraries → CÓ → dùng Library (đơn giản, đã test)
2. KHÔNG có Library → dùng MpsPrinter.Run trực tiếp (tự build PDO)
3. TUYỆT ĐỐI KHÔNG tự code logic in từ đầu — phải dùng 1 trong 2 cách trên

---

## 10. Quy Tắc Chung

| Quy tắc | Chi tiết |
|---------|----------|
| **ƯU TIÊN Print Library** | Kiểm tra 12 libraries trước — KHÔNG tự build PDO nếu đã có |
| **Truyền ĐỦ tham số** | Constructor của Library cần đủ data — đọc IntelliSense để biết tham số BẮT BUỘC |
| PrintTypeCode là constant | Khai báo trong PrintTypeCodeWorker.cs — KHÔNG hardcode string |
| Print Library ref từ LIB | `..\..\..\..\LIB\HIS\HIS.Desktop.Plugins.Library.Print*.dll` |
| PDO ref từ LIB (khi MpsPrinter) | `..\..\..\..\LIB\MPSv2\MPS.PDO\` |
| WaitingManager | Show trước load data, Hide trước Print() / MpsPrinter.Run |
| Preview vs PrintNow | Theo ConfigApplications — KHÔNG hardcode |
| EMR ký số | Dùng PreviewType.EmrSign* hoặc set EmrInputADO trong PrintData |
