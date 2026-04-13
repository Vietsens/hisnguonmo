---
description: Tìm delegate phù hợp cho giao tiếp giữa plugins — trả về delegate type, cách dùng, code mẫu
argument-hint: <mục đích VD: trả data về parent, refresh grid, đóng form, focus UC tiếp theo>
---

# Tìm Delegate

Mục đích: $ARGUMENTS

## Bước 1: Map mục đích sang delegate

### Delegate Nghiệp Vụ (HIS.Desktop.Common/Delegate.cs)

| Mục đích | Delegate |
|----------|----------|
| Trả data về parent | DelegateSelectData(object data) |
| Trả 2 objects | DelegateSelectDatas(object data1, object data2) |
| Trả nhiều objects | DelegateReturnMutilObject(object[] args) |
| Thông báo refresh | DelegateRefreshData() |
| Trả kết quả bool | DelegateReturnSuccess(bool success) |
| Đóng form trả data | DelegateCloseForm_Uc(object data) |
| Refresh references | RefeshReference() |
| Refresh ICD | DelegateRefeshDataIcd(HIS_ICD icd) |
| Refresh ICD phụ | DelegateRefeshIcdChandoanphu(string codes, string names) |
| Refresh treatment | DelegateRefeshTreatmentPartialData(long treatmentId) |
| Generic callback | Action<Type> hoặc Action<object> |

### Delegate UI (DelegateRegister.cs)

| Mục đích | Delegate |
|----------|----------|
| Focus UC tiếp theo | DelegateFocusNextUserControl() |
| Validate UC | DelegateValidationUserControl(bool) |
| Ẩn/hiện control | DelegateVisible(bool) |
| Bật/tắt control | DelegateEnableOrDisableControl(bool?, bool?) |
| Bật/tắt nút Save | DelegateEnableButtonSave(bool) |
| Tìm bệnh nhân | DelegateSearchPatient(string, string) |
| Gửi data BN | DelegateSendPatientSDO(HisPatientSDO) |
| Gửi data thẻ | DelegateSendCardSDO(HisCardSDO) |
| Reload data | DelegateReloadData(bool) |
| Lấy giờ y lệnh | DelegateGetIntructionTime() → DateTime |

## Bước 2: Code mẫu sử dụng

```csharp
// 1. Truyền delegate qua Processor.Run()
var args = new object[] { moduleData, new DelegateSelectData(OnDataSelected) };
Processor.Run(args);

// 2. Parse trong Behavior.Run()
for (int i = 0; i < entity.Count(); i++) {
    if (entity[i] is DelegateSelectData)
        delegateSelect = (DelegateSelectData)entity[i];
}

// 3. Lưu trong Form field
this.delegateSelect = delegateSelect;

// 4. Invoke sau save (null check BẮT BUỘC)
if (this.delegateSelect != null)
    this.delegateSelect(savedData);
```
