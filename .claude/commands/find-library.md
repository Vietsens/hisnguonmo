---
description: Tìm Library plugin phù hợp theo chức năng — 41 libraries (Validation, EMR, Print, Form, Tích hợp, Nghiệp vụ)
argument-hint: <chức năng VD: kiểm tra ICD, xác thực BHYT, ký số EMR, in đơn thuốc, hóa đơn điện tử>
---

# Tìm Library Plugin

Tìm library cho: $ARGUMENTS

## Bước 1: Map chức năng sang Library

### Validation
| Chức năng | Library | Method chính |
|-----------|---------|-------------|
| Kiểm tra ICD hợp lệ | CheckIcd | `CheckIcdManager.ProcessCheckIcd(icdCodes, subCodes, ref error, isCheck)` |
| Xác thực thẻ BHYT | CheckHeinGOV | `HeinGOVManager.Check(dataHein, focus, isChange, address, time, isQR, showMsg)` |
| Cảnh báo phí không BHYT | AlertHospitalFeeNotBHYT | `AlertHospitalFeeNotBHYTManager.Run(treatmentId, patientTypeId, roomId)` |
| Cảnh báo vượt trần BHYT | AlertWarningFee | `AlertWarningFeeManager.Run(treatmentId, ..., ref warning, showMsg)` |

### EMR / Ký số
| Chức năng | Library | Method chính |
|-----------|---------|-------------|
| Tạo input ký số | EmrGenerate | `EmrGenerateProcessor.GenerateInputADOWithPrintTypeCode(code, printType, isSign, roomId)` |
| Lý do kết thúc mở rộng | TreatmentEndTypeExt | `TreatmentEndTypeExtProcessor` |

### Print (12 libraries — xem print_integration.md)
| Chức năng | Library |
|-----------|---------|
| In đơn thuốc | PrintPrescription |
| In phiếu thanh toán | PrintBordereau |
| In phiếu yêu cầu DV | PrintServiceReq |
| In phiếu yêu cầu điều trị | PrintServiceReqTreatment |
| In giấy ra viện | PrintTreatmentFinish |
| In biểu mẫu khác | PrintOtherForm |
| In kết quả XN | PrintTestTotal |
| In phiếu xuất tổng hợp | PrintAggrExpMest |

### Form / Biểu mẫu
| Chức năng | Library |
|-----------|---------|
| Biểu mẫu dịch vụ | FormOtherSereServ |
| Biểu mẫu PTTT | FormOtherSereServPttt |
| Biểu mẫu yêu cầu DV | FormOtherServiceReq |
| Biểu mẫu điều trị | FormOtherTreatment |
| Menu bệnh án | FormMedicalRecord |

### Tích hợp ngoài
| Chức năng | Library |
|-----------|---------|
| Thanh toán ngân hàng | BankHub |
| Hóa đơn điện tử | ElectronicBill |
| Liên thông dược quốc gia | NationalPharmacyConnect |
| Đồng bộ HID | HisSyncToHid |
| Máy đo huyết áp | ConnectBloodPressure |
| Bệnh WHO | ConnectWhoCnd |

### Nghiệp vụ
| Chức năng | Library |
|-----------|---------|
| Tương tác thuốc | DrugInterventionInfo |
| Cấp phát kho | MediStockExpend |
| Tích hợp kê đơn | IntegrateAssignPrescription |
| Lịch sử điều trị | OtherTreatmentHistory |
| Bảo lãnh chi phí | MedicalExpenseGuarantee |

## Bước 2: Tra cứu API chi tiết

Đọc library_plugins_guide.md để lấy:
- Constructor parameters (tham số BẮT BUỘC)
- Public methods + return type
- Code mẫu sử dụng

## Bước 3: Reference trong .csproj

```xml
<Reference Include="HIS.Desktop.Plugins.Library.{Name}">
  <HintPath>..\..\..\..\LIB\HIS\HIS.Desktop.Plugins.Library.{Name}\HIS.Desktop.Plugins.Library.{Name}.dll</HintPath>
</Reference>
```

## Bước 4: Tìm plugins đã dùng Library này

Search `using HIS.Desktop.Plugins.Library.{Name}` trong HIS/Plugins/ để xem code mẫu thực tế.

## Lưu ý
- ƯU TIÊN Library có sẵn — KHÔNG tự code lại
- Truyền ĐỦ tham số constructor — đọc IntelliSense
- Print → xem chi tiết print_integration.md
