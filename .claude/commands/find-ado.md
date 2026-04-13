---
description: Tìm ADO class theo entity hoặc function name — HIS.Desktop.ADO, BackendData/ADO, plugin-internal ADO
argument-hint: <entity name VD: prescription, treatment, bed, service, transaction, patient>
---

# Tìm ADO Class

Tìm ADO: $ARGUMENTS

## Bước 1: Search HIS.Desktop.ADO/ (77+ files)
Tìm file names trong `hisnguonmo/HIS/HIS.Desktop.ADO/` match keyword.
Đây là cross-plugin communication ADOs — truyền data giữa plugins qua Processor.Run(args).

Pattern: {FunctionGroup}ADO.cs
VD: AssignPrescriptionADO, ExamTreatmentFinishADO, BedLogADO

## Bước 2: Search BackendData/ADO/ (14 files)
Tìm trong `HIS/HIS.Desktop.LocalStorage.BackendData/ADO/`
Đây là cache composite types — tính toán từ nhiều EFMODEL.

ADO có sẵn: AgeADO, CommuneADO, ServiceComboADO, MedicineMaterialTypeComboADO, TimeSyncADO, DataADO, ServiceADO

## Bước 3: Search plugin-internal ADOs
Tìm `*ADO.cs` hoặc `*Ado.cs` trong `HIS/Plugins/*/`
Một số plugins define ADO riêng.

## Bước 4: Phân tích ADO tìm được

```
ADO: {class name}
Path: {file path}
Namespace: {namespace}

Properties:
  - {name}: {type} — scalar / EFMODEL ref / delegate callback

Delegates (nếu có):
  - {name}: {delegate type} — mục đích

Dùng bởi plugins:
  - {list plugins dùng ADO này}

EFMODEL liên quan:
  - {entity types trong properties}
```

## Lưu ý
- ADO chỉ là data transfer — KHÔNG có business logic
- Delegate properties trong ADO phải nullable
- Dùng View type (V_HIS_*) cho read scenarios
- Kiểm tra ADO có sẵn trước khi tạo mới
