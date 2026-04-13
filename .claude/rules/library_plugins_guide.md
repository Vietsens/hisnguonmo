---
description: 41 Library plugins dùng chung — CheckIcd, CheckHeinGOV, EmrGenerate, BankHub, ElectronicBill, CacheClient. ƯU TIÊN dùng Library có sẵn
paths:
  - "HIS/Plugins/**"
---

# Library Plugins — Thư Viện Dùng Chung Giữa Plugins

41 Library projects trong `HIS/Plugins/HIS.Desktop.Plugins.Library.*` — KHÔNG phải plugin chạy độc lập, mà là thư viện được nhiều plugins gọi.

**ƯU TIÊN dùng Library có sẵn** — KHÔNG tự code lại logic đã có.

---

## 1. Nhóm Validation (4 libraries)

### CheckIcd — Kiểm tra ICD hợp lệ

```csharp
// Kiểm tra chẩn đoán theo giới tính, tuổi, bệnh kèm
CheckIcdManager.ProcessCheckIcd(
    icdCodes,          // string — mã ICD chính (phân cách ;)
    icdSubCodes,       // string — mã ICD phụ
    ref messageError,  // string — trả về thông báo lỗi
    isCheck            // bool — có kiểm tra không
);

// Kiểm tra mở rộng — conflict group trong lịch sử điều trị
CheckIcdManager.ProcessCheckIcd(
    icdCodes, icdSubCodes, ref messageError, isCheck,
    isSave             // bool — đang lưu (kiểm tra nghiêm hơn)
);
```

### CheckHeinGOV — Xác thực thẻ BHYT qua cổng BHXH

```csharp
// Xác thực thẻ BHYT online
HeinGOVManager.Check(
    dataHein,              // HeinCardData — dữ liệu thẻ
    focusNextControl,      // Action — callback focus
    isCheckChange,         // bool — kiểm tra thay đổi
    heinAddress,           // string — địa chỉ KCB
    instructionTime,       // long? — thời gian y lệnh
    isReadQrCode,          // bool — đọc từ QR
    showMessage            // bool — hiện thông báo
);

// Kiểm tra chi tiết hồ sơ
HeinGOVManager.CheckChiTietHS(maHS, viTri);

// Đọc QR CCCD
HeinGOVManager.CheckCccdQrCode(dataHein, focusNextControl, instructionTime);
```

### AlertHospitalFeeNotBHYT — Cảnh báo phí không BHYT

```csharp
AlertHospitalFeeNotBHYTManager.Run(
    treatmentId,     // long
    patientTypeId,   // long
    roomId           // long
);
```

### AlertWarningFee — Cảnh báo vượt trần BHYT

```csharp
AlertWarningFeeManager.Run(
    treatmentId, patientTypeId, treatmentTypeId,
    heinMediorgCode,           // string — mã cơ sở KCB
    patientTypeIdBHYT,         // long — đối tượng BHYT
    totalHeinPrice,            // decimal — tổng chi phí BHYT
    isUsingWarningHeinFee,     // bool — có bật cảnh báo
    amountPlus,                // decimal — số tiền vượt
    ref messageWarning,        // string — thông báo
    isShowMessage              // bool — hiện popup
);
```

---

## 2. Nhóm EMR / Ký Số (2 libraries)

### EmrGenerate — Tạo input ký số EMR

```csharp
var emrProc = new EmrGenerateProcessor();

// Tạo input ký số cho tài liệu
Inventec.Common.SignLibrary.ADO.InputADO inputADO = emrProc.GenerateInputADO(
    treatmentCode,     // string — mã điều trị
    documentCode,      // string — mã tài liệu
    documentName,      // string — tên tài liệu
    roomId             // long — phòng
);

// Tạo input ký số cho in ấn (dùng trong Print flow)
InputADO inputADO = emrProc.GenerateInputADOWithPrintTypeCode(
    treatmentCode,     // string
    printTypeCode,     // string — "Mps000102"
    isSign,            // bool — có ký không
    roomId             // long
);

// Quản lý trạng thái checkbox ký số
bool closeAfterSign = emrProc.GetCheckedStateCloseAfterSign();
emrProc.ActCheckedChangedCloseAfterSign();

// SignPad device
bool useSignPad = emrProc.GetOptionUsingSignPad();
string deviceName = emrProc.GetDeviceSignPadName();
```

### TreatmentEndTypeExt — Mở rộng lý do kết thúc điều trị

```csharp
var proc = new TreatmentEndTypeExtProcessor();
// Xử lý giấy nghỉ ốm, giấy khám thai, phẫu thuật...
```

---

## 3. Nhóm Print (12 libraries)

Đã document chi tiết trong `print_integration.md`:
PrintPrescription, PrintBordereau, PrintServiceReq, PrintServiceReqTreatment,
PrintTreatmentFinish, PrintTreatmentEndTypeExt, PrintOtherForm, PrintTestTotal,
PrintAggrExpMest, PrintPublicMedicines, PrintPatientUpdate, PrintSarFormData.

---

## 4. Nhóm Form / Biểu Mẫu (5 libraries)

### FormOtherSereServ — Biểu mẫu dịch vụ

```csharp
var proc = new FormOtherProcessor();
// Tạo/in biểu mẫu cho dịch vụ (sere_serv)
```

### FormOtherSereServPttt — Biểu mẫu phẫu thuật/thủ thuật

```csharp
var proc = new FormOtherProcessor();
// Biểu mẫu riêng cho PTTT
```

### FormOtherServiceReq — Biểu mẫu yêu cầu dịch vụ

```csharp
var proc = new FormOtherProcessor();
// Biểu mẫu theo service request
```

### FormOtherTreatment — Biểu mẫu điều trị

```csharp
var proc = new FormOtherTreatmentProcessor();
// Biểu mẫu theo treatment (bệnh án, giấy tờ...)
```

### FormMedicalRecord — Menu popup bệnh án

```csharp
var proc = new MediRecordMenuPopupProcessor();
// Tạo menu popup chọn biểu mẫu bệnh án EMR
```

---

## 5. Nhóm Tích Hợp Ngoài (6 libraries)

### BankHub — Thanh toán ngân hàng

```csharp
var bankProc = new BankHubProcess();
bankProc.CheckExpiry();                    // Kiểm tra token hết hạn
string token = bankProc.GetAccessToken(bankCode);  // Lấy OAuth token
```

### ElectronicBill — Hóa đơn điện tử

```csharp
var billProc = new ElectronicBillProcessor();
billProc.Run(billType);                    // Phát hành/hủy hóa đơn
billProc.GetInvoiceInfo(input, ref invoiceSys, ref invoiceCode, ref errorMsg);
```

### NationalPharmacyConnect — Liên thông dược quốc gia

```csharp
var pharmacyProc = new NationalPharmacyConnectProcess(address, loginName, password);
// Kết nối hệ thống dược quốc gia
```

### HisSyncToHid — Đồng bộ HID (Health ID)

```csharp
// Form chọn bệnh nhân để đồng bộ lên hệ thống định danh y tế
var frmSync = new frmPersonSelect();
```

### ConnectBloodPressure — Máy đo huyết áp

```csharp
var bpProc = new ConnectBloodPressureProcessor();
var data = bpProc.GetData();  // Lấy chỉ số huyết áp từ thiết bị
```

### ConnectWhoCnd — Bệnh không lây nhiễm WHO

```csharp
var whoProc = new ConnectWhoCndProcessor();
whoProc.CheckData();  // Kiểm tra dữ liệu bệnh mạn tính
```

---

## 6. Nhóm Nghiệp Vụ (5 libraries)

### DrugInterventionInfo — Tương tác thuốc

```csharp
var drugProc = new DrugInterventionInfoProcessor();
drugProc.CheckPrescription(inputData);  // Kiểm tra tương tác đơn thuốc
```

### MediStockExpend — Cấp phát kho

```csharp
var stockProc = new MediStockExpendProcessor();
// Xử lý cấp phát thuốc/vật tư từ kho
```

### IntegrateAssignPrescription — Tích hợp kê đơn

```csharp
var intProc = new IntegrateAssignPrescriptionProcesser();
// Tích hợp kê đơn với hệ thống ngoài
```

### OtherTreatmentHistory — Lịch sử điều trị ngoài

```csharp
var histProc = new OtherTreatmentHistoryProcessor();
// Quản lý lịch sử điều trị tại cơ sở khác
```

### MedicalExpenseGuarantee — Bảo lãnh chi phí

```csharp
var guaranteeProc = new MedicalExpenseGuaranteeProcessor();
// Kiểm tra điều kiện bảo lãnh viện phí
```

---

## 7. Nhóm Khác (3 libraries)

### RegisterConnectHrm — Kết nối HRM

```csharp
var hrmProc = new RegisterConnectHrmProcessor();
// Đồng bộ đăng ký với hệ thống nhân sự
```

### TwoIDStorageIntegration — Lưu trữ đám mây

```csharp
var storageProc = new TwoIDStorageIntegrationProcessor();
// Lưu tài liệu lên hệ thống lưu trữ đám mây TwoID
```

---

## 8. CacheClient — Lưu Trữ Local

### ControlStateWorker — Lưu trạng thái UI

```csharp
var csWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();

// Đọc trạng thái đã lưu
List<ControlStateRDO> states = csWorker.GetData(moduleLink);

// Lưu trạng thái
csWorker.SetData(new List<ControlStateRDO> {
    new ControlStateRDO {
        MODULE_LINK = "HIS.Desktop.Plugins.MyPlugin",
        KEY = "chkPrint",
        VALUE = "1"  // "1" = checked, "" = unchecked
    }
});

// Xóa trạng thái module
csWorker.ResetData(moduleLink);

// Trạng thái theo session (không lưu disk)
csWorker.GetDataBySession();
csWorker.SetDataBySession(states);
```

### CacheWorker — Cache dữ liệu chung

```csharp
// Lấy dữ liệu cache (SQLite hoặc Redis)
List<T> data = CacheWorker.Get<T>(dataKey);

// Lưu cache
CacheWorker.Set<T>(data, dataKey, isWaitingSync);

// Xóa cache
CacheWorker.Delete(dataKey);
CacheWorker.DeleteWithState(dataKey);  // Xóa cả metadata sync

// Xóa toàn bộ
CacheWorker.TruncateAll();
```

---

## 9. Quy Tắc

| Quy tắc | Chi tiết |
|---------|----------|
| **ƯU TIÊN Library có sẵn** | Kiểm tra 41 libraries TRƯỚC khi tự code |
| Reference qua HintPath | `..\..\..\..\LIB\HIS\HIS.Desktop.Plugins.Library.{Name}.dll` |
| Truyền ĐỦ tham số | Đọc constructor/method của Library để biết tham số BẮT BUỘC |
| KHÔNG tự code ICD validation | Dùng CheckIcdManager |
| KHÔNG tự code BHYT check | Dùng HeinGOVManager |
| KHÔNG tự code EMR sign input | Dùng EmrGenerateProcessor |
| KHÔNG tự code hóa đơn điện tử | Dùng ElectronicBillProcessor |
| Print → xem print_integration.md | 12 Print Libraries document riêng |
| ControlState → ControlStateWorker | KHÔNG tự đọc/ghi SQLite trực tiếp |
