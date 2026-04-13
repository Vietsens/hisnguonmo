---
description: Scan phạm vi ảnh hưởng khi có thay đổi lớn — EFMODEL mới, API đổi, BHXH QĐ mới, IMSys.DbConfig đổi
argument-hint: <mô tả thay đổi VD: "thêm field WEIGHT vào HIS_TREATMENT" hoặc "đổi API HisTreatment/GetView">
---

# Check Migration Impact

Thay đổi: $ARGUMENTS

## Bước 1: Xác định loại thay đổi
- EFMODEL thêm/đổi/xóa field?
- API đổi endpoint hoặc response?
- BHXH ra QĐ mới?
- IMSys.DbConfig thêm constant?
- Lib update (DevExpress, Inventec.*)?

## Bước 2: Scan phạm vi

### EFMODEL thay đổi
```
Grep "{EntityName}" trong HIS/Plugins/ → plugins dùng
Grep "{FieldName}" trong HIS/Plugins/ → nơi dùng field
Grep "{EntityName}" trong MPS/MPS.Processor/ → MPS dùng
Grep "{EntityName}" trong UC/ → UC dùng
```

### API thay đổi
```
Grep "api/{Entity}/{Action}" trong HIS.Desktop.ApiConsumer/
Grep "{URI_CONSTANT}" trong HIS/Plugins/
```

### IMSys.DbConfig thay đổi
```
Grep "IMSys.DbConfig.HIS_RS.{Table}" trong HIS/Plugins/
Grep "== {old_value}" cho hardcode số liên quan
```

## Bước 3: Liệt kê files ảnh hưởng

| File | Loại thay đổi | Severity | Effort |
|------|--------------|----------|--------|
| {path:line} | {mô tả} | CRITICAL/HIGH/MEDIUM | Low/Medium/High |

## Bước 4: Thứ tự update
1. Infrastructure (RequestUriStore, ADO, BackendDataWorker)
2. Shared (HIS.Desktop.Common, Library plugins)
3. Plugins (từng plugin)
4. MPS Processors
5. UC

## Output
```
THAY ĐỔI: {mô tả}
PHẠM VI: {số plugins + số MPS + số UC}
FILES: {danh sách}
THỨ TỰ: Infrastructure → Shared → Plugins → MPS → UC
```
