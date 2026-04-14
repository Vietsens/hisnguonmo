---
name: debug-cache
description: Debug BackendDataWorker cache — stale data, cache miss, sync failure, tìm thiếu Reset
user-invocable: true
argument-hint: <triệu chứng VD: "V_HIS_SERVICE cũ sau update" hoặc "department list rỗng">
---

# Debug BackendData Cache

Triệu chứng: $ARGUMENTS

## Bước 1: Xác định EFMODEL type

Từ triệu chứng → xác định type bị ảnh hưởng:
- "service cũ" → V_HIS_SERVICE hoặc V_HIS_SERVICE_PATY
- "department rỗng" → HIS_DEPARTMENT
- "thuốc không hiện" → V_HIS_MEDICINE_TYPE hoặc HisMedicineTypeInStockSDO

## Bước 2: Trace cache flow

Đọc các files:
1. `HIS.Desktop.LocalStorage.BackendData/Core/BackendDataWorker.cs`
   - Get<T>() flow: dic.ContainsKey → dic.TryGetValue → GetDataByType
2. `HIS.Desktop.LocalStorage.BackendData/Core/Get/GetDataBehaviorFactory__v2.cs`
   - Map typeof(T) → behavior class (MOS/SDA/ACS/LIS)
3. Behavior file cụ thể → API URI + Consumer

## Bước 3: Check auto-filter

SetOtherFilterQuery<T>() tự động inject:
- V_HIS_SERVICE_PATY → BRANCH_ID (có thể sai branch)
- V_HIS_USER_ROOM → IS_ACTIVE + LOGINNAME (có thể sai user)
- HIS_MEDICINE_TYPE_TUT → LOGINNAME

## Bước 4: Tìm callers

Search trong HIS/Plugins/:
```
BackendDataWorker.Get<{TYPE}>()     — ai đọc?
BackendDataWorker.Reset<{TYPE}>()   — ai invalidate?
```

## Bước 5: Chẩn đoán

### Stale data sau update
- Tìm plugin thực hiện Create/Update API cho type này
- Kiểm tra: có gọi Reset<T>() SAU API call không?
- Nếu THIẾU → đó là bug. Thêm Reset<T>() sau BackendAdapter.Post

### List rỗng
- API URI trong behavior có đúng?
- Consumer đã init? (phải sau ConfigSystem.Load.Init)
- Auto-filter có inject điều kiện bất ngờ? (BRANCH_ID, LOGINNAME)
- isSaveToRam có = false?

### Cache không refresh
- Timer intervals trong frmMainPlus__SyncCacheLocalData có > 0?
- CacheMonitorSyncExecute<T>() có được gọi?
- isUseCacheLocal config: 0=RAM only, 1=SQLite, 2=Redis

### RAM vs local DB khác nhau
- timerSyncToRAM interval
- SqliteProcess.Sync() / RedisProcess.Sync() có fail?

## Bước 6: Đề xuất fix

```
Type: {full type name}
Cache Mode: RAM / SQLite+RAM / Redis+RAM
Fetch: {behavior} → {API URI} via {consumer}
Vấn đề: {chẩn đoán cụ thể}
Fix: {code cần thêm — Reset<T>(), sửa filter, fix config}
File cần sửa: {path:line}
```
