---
description: Trace BackendDataWorker cache flow cho EFMODEL type — RAM, SQLite/Redis, API, auto-filter
argument-hint: <EFMODEL type VD: V_HIS_SERVICE, HIS_DEPARTMENT, V_HIS_MEDICINE_TYPE>
---

# Trace Cache Flow

Type: $ARGUMENTS

## Bước 1: Tìm type trong lib/ EFMODEL
- MOS.EFMODEL.DataModels.*
- SDA.EFMODEL.DataModels.*
- ACS.EFMODEL.DataModels.*
- LIS.EFMODEL.DataModels.*

## Bước 2: Check GetDataBehaviorFactory
Đọc `HIS.Desktop.LocalStorage.BackendData/Core/Get/GetDataBehaviorFactory__v2.cs`
Tìm behavior nào fetch type này:
- MosGetListBehavior → MOS API
- SdaGetListBehavior → SDA API
- AcsGetListBehavior → ACS API
- RdCacheGetListBehavior → Redis cache
- Special: CommuneADO, AgeADO, ServiceComboADO

## Bước 3: Tìm API URI
Từ behavior → RequestUriStore tương ứng

## Bước 4: Check auto-filter
SetOtherFilterQuery<T>() inject filter cho:
- V_HIS_SERVICE_PATY → BRANCH_ID, CustomColumns
- V_HIS_USER_ROOM, HIS_USER_ROOM → IS_ACTIVE, LOGINNAME
- HIS_MEDICINE_TYPE_TUT → LOGINNAME

## Bước 5: Trace callers
Search trong HIS/Plugins/:
- BackendDataWorker.Get<{TYPE}>() — ai đọc?
- BackendDataWorker.Reset<{TYPE}>() — ai invalidate?

## Bước 6: Output

```
Type: {full namespace}
Cache: RAM (ConcurrentDictionary) → SQLite/Redis (nếu config) → API
Backend: {MOS/SDA/ACS} qua {BehaviorClass}
API URI: {constant từ RequestUriStore}
Auto-filter: {nếu có}
Callers Get: [{files}]
Callers Reset: [{files}]
Quy tắc: Gọi Reset<T>() SAU KHI modify data
```
