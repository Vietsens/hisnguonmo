---
name: integrate-library
description: Tích hợp Library plugin vào code — tìm Library phù hợp, đọc API, sinh code khởi tạo + gọi + reference csproj
user-invocable: true
argument-hint: <chức năng VD: "kiểm tra ICD" hoặc "in đơn thuốc" hoặc "xác thực BHYT" hoặc "hóa đơn điện tử">
---

# Integrate Library — Tích Hợp Thư Viện Vào Plugin

Chức năng cần: $ARGUMENTS

## Bước 1: Tìm Library phù hợp

### Nhóm Validation
| Chức năng | Library | Class chính |
|-----------|---------|-------------|
| Kiểm tra ICD | CheckIcd | CheckIcdManager |
| Xác thực BHYT | CheckHeinGOV | HeinGOVManager |
| Cảnh báo phí BHYT | AlertHospitalFeeNotBHYT | AlertHospitalFeeNotBHYTManager |
| Cảnh báo vượt trần | AlertWarningFee | AlertWarningFeeManager |

### Nhóm EMR / Ký số
| Chức năng | Library | Class chính |
|-----------|---------|-------------|
| Tạo input ký số | EmrGenerate | EmrGenerateProcessor |
| Kết thúc điều trị mở rộng | TreatmentEndTypeExt | TreatmentEndTypeExtProcessor |

### Nhóm Print (12 libraries)
| Chức năng | Library | Class chính |
|-----------|---------|-------------|
| In đơn thuốc | PrintPrescription | PrintPrescriptionProcessor |
| In phiếu thanh toán | PrintBordereau | PrintBordereauProcessor |
| In phiếu yêu cầu DV | PrintServiceReq | PrintServiceReqProcessor |
| In phiếu yêu cầu điều trị | PrintServiceReqTreatment | PrintServiceReqTreatmentProcessor |
| In giấy ra viện | PrintTreatmentFinish | PrintTreatmentFinishProcessor |
| In biểu mẫu khác | PrintOtherForm | PrintOtherFormProcessor |
| In kết quả XN | PrintTestTotal | PrintTestTotalProcessor |
| In phiếu xuất tổng hợp | PrintAggrExpMest | PrintAggrExpMestProcessor |

### Nhóm Form
| Chức năng | Library | Class chính |
|-----------|---------|-------------|
| Biểu mẫu dịch vụ | FormOtherSereServ | FormOtherProcessor |
| Biểu mẫu PTTT | FormOtherSereServPttt | FormOtherProcessor |
| Biểu mẫu yêu cầu DV | FormOtherServiceReq | FormOtherProcessor |
| Biểu mẫu điều trị | FormOtherTreatment | FormOtherTreatmentProcessor |
| Menu bệnh án | FormMedicalRecord | MediRecordMenuPopupProcessor |

### Nhóm Tích hợp
| Chức năng | Library | Class chính |
|-----------|---------|-------------|
| Thanh toán ngân hàng | BankHub | BankHubProcess |
| Hóa đơn điện tử | ElectronicBill | ElectronicBillProcessor |
| Liên thông dược | NationalPharmacyConnect | NationalPharmacyConnectProcess |
| Đồng bộ HID | HisSyncToHid | frmPersonSelect |
| Tương tác thuốc | DrugInterventionInfo | DrugInterventionInfoProcessor |

Nếu KHÔNG tìm thấy Library → báo user, KHÔNG tự code — có thể đã có Library chưa liệt kê.

## Bước 2: Đọc API của Library được chọn

Đọc file Processor/Manager chính của Library:
```
common/ hoặc LIB/ → HIS.Desktop.Plugins.Library.{Name}/
Tìm: {Name}Processor.cs hoặc {Name}Manager.cs
```

Liệt kê:
- **Constructors** — tham số nào BẮT BUỘC, kiểu gì
- **Public methods** — Run(), Print(), Check(), GetValue()...
- **Properties** — set trước khi gọi method

## Bước 3: Thêm reference .csproj

Tìm file `.csproj` của plugin hiện tại, thêm:

```xml
<Reference Include="HIS.Desktop.Plugins.Library.{LibraryName}">
  <HintPath>..\..\..\..\LIB\HIS\HIS.Desktop.Plugins.Library.{LibraryName}\HIS.Desktop.Plugins.Library.{LibraryName}.dll</HintPath>
</Reference>
```

Nếu Library cần thêm dependencies (VD: MPS.ProcessorBase cho Print):
```xml
<Reference Include="MPS.ProcessorBase">
  <HintPath>..\..\..\..\LIB\MPSv2\MPS.ProcessorBase\MPS.ProcessorBase.dll</HintPath>
</Reference>
```

## Bước 4: Sinh code tích hợp

### 4a. Using statement

```csharp
using HIS.Desktop.Plugins.Library.{LibraryName};
```

### 4b. Code theo từng loại Library

#### Validation (CheckIcd):
```csharp
private bool ValidateIcd()
{
    try
    {
        string messageError = "";
        bool isValid = CheckIcdManager.ProcessCheckIcd(
            icdCode,                    // string — mã ICD chính (;-separated)
            icdSubCode,                 // string — mã ICD phụ
            ref messageError,           // ref string — trả về lỗi
            true                        // bool — bật kiểm tra
        );

        if (!isValid && !string.IsNullOrEmpty(messageError))
        {
            DevExpress.XtraEditors.XtraMessageBox.Show(
                messageError,
                MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        return isValid;
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
        return true; // Lỗi validation → cho qua, KHÔNG chặn user
    }
}
```

#### Validation (CheckHeinGOV):
```csharp
private void CheckBHYT()
{
    try
    {
        HeinGOVManager.Check(
            heinCardData,               // HeinCardData — dữ liệu thẻ BHYT
            () => { nextControl.Focus(); },  // Action — focus sau check
            true,                        // bool — kiểm tra thay đổi
            heinAddress,                 // string — địa chỉ KCB
            instructionTime,             // long? — thời gian y lệnh
            false,                       // bool — đọc từ QR
            true                         // bool — hiện thông báo
        );
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

#### Print (PrintPrescription):
```csharp
private void PrintPrescription()
{
    try
    {
        var printProc = new Library.PrintPrescription.PrintPrescriptionProcessor(
            outPatientPresResults,      // List<OutPatientPresResultSDO> — BẮT BUỘC
            this.currentModule          // Module — BẮT BUỘC
        );

        // Set tùy chọn
        printProc.SetOutHospital(hasNhaThuoc);

        // In
        printProc.Print(
            PrintTypeCodeWorker.PRINT_TYPE_CODE__MPS000118,  // PrintTypeCode constant
            false,                                             // printNow
            MPS.ProcessorBase.PrintConfig.PreviewType.Show     // Preview
        );
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

#### EMR (EmrGenerate):
```csharp
private Inventec.Common.SignLibrary.ADO.InputADO GetEmrSignInput(string printTypeCode)
{
    try
    {
        var emrProc = new Library.EmrGenerate.EmrGenerateProcessor();
        return emrProc.GenerateInputADOWithPrintTypeCode(
            this.currentTreatment.TREATMENT_CODE ?? "",  // string
            printTypeCode,                                 // string
            true,                                          // bool — có ký
            this.currentModule.RoomId                      // long
        );
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
        return null;
    }
}
```

#### Tích hợp (ElectronicBill):
```csharp
private void CreateElectronicBill()
{
    try
    {
        WaitingManager.Show();
        var billProc = new Library.ElectronicBill.ElectronicBillProcessor();
        billProc.Run(billType);  // Enum loại hóa đơn

        string invoiceSys = "", invoiceCode = "", errorMsg = "";
        billProc.GetInvoiceInfo(input, ref invoiceSys, ref invoiceCode, ref errorMsg);

        if (!string.IsNullOrEmpty(errorMsg))
            Inventec.Common.Logging.LogSystem.Warn("ElectronicBill error: " + errorMsg);

        WaitingManager.Hide();
    }
    catch (Exception ex)
    {
        WaitingManager.Hide();
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

## Bước 5: Tìm plugin mẫu đã dùng Library này

Search trong `HIS/Plugins/`:
```
using HIS.Desktop.Plugins.Library.{LibraryName}
```
Đọc 1-2 files tìm được để hiểu cách dùng thực tế.

## Bước 6: Verify

- [ ] Reference .csproj đúng HintPath
- [ ] Using statement đúng namespace
- [ ] Constructor truyền ĐỦ tham số BẮT BUỘC
- [ ] Method gọi đúng (Print, Check, Run, Generate...)
- [ ] Try-catch bao quanh
- [ ] WaitingManager nếu có API call
- [ ] KHÔNG tự code lại logic Library đã có
- [ ] Test chức năng hoạt động đúng
