---
description: Trace luồng data end-to-end — UI control → ADO → API/Cache → EFMODEL → Backend table. Dùng khi cần hiểu "data này từ đâu ra"
argument-hint: <tên field, control, hoặc entity VD: TDL_PATIENT_NAME, gridColumn "Tên BN", V_HIS_TREATMENT>
---

# Trace Data Flow

Trace: $ARGUMENTS

## Bước 1: Xác định điểm bắt đầu
- Nếu là UI control → tìm trong Form/UC .cs + .Designer.cs
- Nếu là field name → tìm trong EFMODEL (lib/)
- Nếu là entity → tìm API + plugins dùng

## Bước 2: Trace UI → Data Source
- Control bind tới property nào? (FieldName, DataSource)
- Unbound? → tìm trong CustomUnboundColumnData
- DataSource là kiểu gì? List<ADO>? List<EFMODEL>?
- DataSource gán ở đâu? (FillDataToGrid, GridPaging)

## Bước 3: Trace Data Source → API/Cache
- BackendAdapter.GetRO → URI nào? Filter gì?
- BackendDataWorker.Get<T>() → cache RAM? Auto-filter?
- Mapper: ADO mapping từ EFMODEL nào?

## Bước 4: Trace EFMODEL → Backend
- Entity: V_HIS_* (View) hay HIS_* (Table)?
- Property từ bảng nào? (JOIN, denormalized TDL_*)
- TDL_ prefix = denormalized từ bảng khác

## Bước 5: Output
```
UI: {control} → FieldName: {field}
Data: {List<Type>} gán tại {method}:{line}
API: {URI} via {Consumer} với {Filter}
EFMODEL: {Entity}.{Property}
Backend: {Table}.{Column} (origin: {source table nếu TDL_})
```
