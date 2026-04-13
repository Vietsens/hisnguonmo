---
name: add-print
description: Thêm chức năng in vào plugin — chọn Print Library hoặc MpsPrinter, sinh code button + callback + PDO + reference csproj
user-invocable: true
argument-hint: <plugin path + loại in VD: "HIS.Desktop.Plugins.MyPlugin in đơn thuốc" hoặc "thêm in phiếu yêu cầu DV">
---

# Add Print — Thêm Chức Năng In Vào Plugin

Target: $ARGUMENTS

## Bước 1: Xác định loại in cần thêm

Từ mô tả, map sang Print Library hoặc MPS code:

| Loại in | Print Library | Khi nào dùng |
|---------|--------------|--------------|
| Đơn thuốc | PrintPrescription | Kê đơn ngoại trú/nội trú/CLS |
| Phiếu thanh toán | PrintBordereau | Viện phí, thu ngân |
| Phiếu yêu cầu DV | PrintServiceReq | Chỉ định DV, CLS |
| Phiếu yêu cầu điều trị | PrintServiceReqTreatment | Đăng ký khám |
| Giấy ra viện | PrintTreatmentFinish | Kết thúc điều trị |
| Biểu mẫu khác | PrintOtherForm | Giấy hẹn, giấy nghỉ |
| Kết quả XN | PrintTestTotal | Xét nghiệm |
| Phiếu xuất tổng hợp | PrintAggrExpMest | Kho dược |
| **Biểu mẫu MỚI** | Không có Library → MpsPrinter.Run trực tiếp | Tự build PDO |

## Bước 2: Thêm reference .csproj

### Nếu dùng Print Library:
```xml
<Reference Include="HIS.Desktop.Plugins.Library.{PrintLibraryName}">
  <HintPath>..\..\..\..\LIB\HIS\HIS.Desktop.Plugins.Library.{PrintLibraryName}\HIS.Desktop.Plugins.Library.{PrintLibraryName}.dll</HintPath>
</Reference>
```

### Nếu dùng MpsPrinter trực tiếp (thêm TẤT CẢ):
```xml
<Reference Include="MPS">
  <HintPath>..\..\..\..\LIB\MPS\MPS.dll</HintPath>
</Reference>
<Reference Include="MPS.ProcessorBase">
  <HintPath>..\..\..\..\LIB\MPSv2\MPS.ProcessorBase\MPS.ProcessorBase.dll</HintPath>
</Reference>
<Reference Include="MPS.Processor.{MpsCode}.PDO">
  <HintPath>..\..\..\..\LIB\MPSv2\MPS.PDO\MPS.Processor.{MpsCode}.PDO.dll</HintPath>
</Reference>
```

### Luôn thêm (cho EMR sign):
```xml
<Reference Include="HIS.Desktop.Plugins.Library.EmrGenerate">
  <HintPath>..\..\..\..\LIB\HIS\HIS.Desktop.Plugins.Library.EmrGenerate\HIS.Desktop.Plugins.Library.EmrGenerate.dll</HintPath>
</Reference>
```

## Bước 3: Tạo PrintTypeCodeWorker.cs (nếu chưa có)

```csharp
// File: PrintTypeCodeWorker.cs (trong Base/ hoặc root plugin)
namespace HIS.Desktop.Plugins.{PluginName}
{
    internal class PrintTypeCodeWorker
    {
        internal const string PRINT_TYPE_CODE__{MPS_CODE} = "{MpsCode}";
        // VD: internal const string PRINT_TYPE_CODE__MPS000102 = "Mps000102";
    }
}
```

## Bước 4: Sinh code — CÁCH 1: Print Library (ƯU TIÊN)

### 4a. Button click handler

```csharp
private void btnPrint_Click(object sender, EventArgs e)
{
    try
    {
        PrintWithLibrary();
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

### 4b. PrintPrescription

```csharp
private void PrintWithLibrary()
{
    try
    {
        // Tạo processor với ĐỦ tham số
        var printProc = new HIS.Desktop.Plugins.Library.PrintPrescription
            .PrintPrescriptionProcessor(
                outPatientPresResults,    // List<OutPatientPresResultSDO> — data
                this.currentModule        // Module — context
            );

        // Set tùy chọn (nếu cần)
        printProc.SetOutHospital(hasNhaThuocMediStock);

        // Gọi in
        printProc.Print(
            PrintTypeCodeWorker.PRINT_TYPE_CODE__MPS000118,
            ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2,  // printNow
            ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2
                ? MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow
                : MPS.ProcessorBase.PrintConfig.PreviewType.Show
        );
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

### 4c. PrintTreatmentFinish

```csharp
private void PrintTreatmentFinish(string printTypeCode)
{
    try
    {
        var printProc = new HIS.Desktop.Plugins.Library.PrintTreatmentFinish
            .PrintTreatmentFinishProcessor(
                this.currentTreatment,    // HIS_TREATMENT — BẮT BUỘC
                this.currentModule.RoomId  // long? — phòng
            );
        printProc.Print(printTypeCode);
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

### 4d. PrintBordereau

```csharp
private void PrintBordereau()
{
    try
    {
        var printProc = new HIS.Desktop.Plugins.Library.PrintBordereau
            .PrintBordereauProcessor(
                this.currentModule.RoomId,       // long — phòng
                this.currentModule.RoomTypeId,   // long — loại phòng
                this.treatmentId,                 // long — mã điều trị
                this.patientId,                   // long — mã BN
                null,                             // BordereauInitData
                null,                             // ReloadMenuOption
                OnDocumentSigned                  // Action callback ký số
            );
        printProc.Print(
            PrintTypeCodeWorker.PRINT_TYPE_CODE__MPS000446,
            PrintOption.Value.PRINT_NOW,
            null
        );
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

## Bước 5: Sinh code — CÁCH 2: MpsPrinter.Run (khi KHÔNG có Library)

### 5a. RichEditorStore + Callback pattern

```csharp
private void btnPrint_Click(object sender, EventArgs e)
{
    try
    {
        var store = new Inventec.Common.RichEditor.RichEditorStore(
            ApiConsumers.SarConsumer,
            ConfigSystems.URI_API_SAR,
            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
            GlobalVariables.TemnplatePathFolder);

        store.RunPrintTemplate(
            PrintTypeCodeWorker.PRINT_TYPE_CODE__{MPS_CODE},
            DelegatePrintTemplate);
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

### 5b. Callback — switch theo printCode

```csharp
private bool DelegatePrintTemplate(string printCode, string fileName)
{
    bool result = false;
    try
    {
        switch (printCode)
        {
            case PrintTypeCodeWorker.PRINT_TYPE_CODE__{MPS_CODE}:
                Print{MpsCode}(printCode, fileName, ref result);
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

### 5c. Build PDO + MpsPrinter.Run

```csharp
private void Print{MpsCode}(string printTypeCode, string fileName, ref bool result)
{
    try
    {
        WaitingManager.Show();

        // Load data từ API
        CommonParam param = new CommonParam();
        var filter = new SomeFilter();
        filter.ID = currentId;
        var data = new BackendAdapter(param)
            .Get<List<SomeType>>("api/SomeEntity/GetView", ApiConsumers.MosConsumer, filter, param)
            .FirstOrDefault();

        // Tạo PDO
        MPS.Processor.{MpsCode}.PDO.{MpsCode}PDO pdo =
            new MPS.Processor.{MpsCode}.PDO.{MpsCode}PDO(
                treatment,      // V_HIS_TREATMENT
                data            // Data vừa load
            );

        // Printer name từ config
        string printerName = "";
        if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
            printerName = GlobalVariables.dicPrinter[printTypeCode];

        // EMR sign input (optional)
        var inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor()
            .GenerateInputADOWithPrintTypeCode(
                currentTreatment.TREATMENT_CODE ?? "",
                printTypeCode,
                true,
                currentModule.RoomId);

        WaitingManager.Hide();

        // Gọi MpsPrinter
        result = MPS.MpsPrinter.Run(
            new MPS.ProcessorBase.Core.PrintData(
                printTypeCode,
                fileName,
                pdo,
                ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2
                    ? MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow
                    : MPS.ProcessorBase.PrintConfig.PreviewType.Show,
                printerName)
            { EmrInputADO = inputADO });
    }
    catch (Exception ex)
    {
        WaitingManager.Hide();
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

## Bước 6: Verify

- [ ] Reference .csproj đúng (Library hoặc MPS PDO)
- [ ] PrintTypeCodeWorker.cs có constant
- [ ] Print Library: constructor truyền ĐỦ tham số BẮT BUỘC
- [ ] MpsPrinter: PDO đúng namespace + constructor
- [ ] WaitingManager Show/Hide đúng cặp
- [ ] Printer name từ GlobalVariables.dicPrinter
- [ ] PreviewType từ ConfigApplications — KHÔNG hardcode
- [ ] EMR sign nếu cần (EmrGenerateProcessor)
- [ ] Try-catch bao quanh tất cả methods
- [ ] Build thành công
