---
name: optimize-code
description: Tìm và fix performance anti-patterns — O(n²) sang O(n), string+= sang StringBuilder, Get trong loop sang Dictionary
user-invocable: true
argument-hint: <file hoặc folder path>
---

# Optimize Code — Fix Performance Anti-Patterns

Target: $ARGUMENTS

## Bước 1: Scan tìm anti-patterns

Đọc file/folder và tìm các patterns sau:

### P1. O(n²) — Nested lookup [CRITICAL]
```
Pattern: list.Where(o => otherList.Select(p => p.ID).Contains(o.ID))
Pattern: foreach { list.FirstOrDefault(o => o.ID == item.ID) }
Fix: Chuyển sang HashSet/Dictionary trước loop
```

### P2. Get<T>() trong loop [CRITICAL]
```
Pattern: foreach { BackendDataWorker.Get<T>().FirstOrDefault(...) }
Fix: Lấy 1 lần trước loop, ToDictionary, TryGetValue trong loop
```

### P3. string += trong loop [HIGH]
```
Pattern: foreach { result += item.Name + ", "; }
Fix: StringBuilder hoặc String.Join
```

### P4. Count() > 0 [MEDIUM]
```
Pattern: if (list.Count() > 0)
Fix: if (list.Any())
```

### P5. ToList().FirstOrDefault() [MEDIUM]
```
Pattern: list.Where(x).ToList().FirstOrDefault()
Fix: list.FirstOrDefault(x)
```

### P6. API/tính toán trong CustomUnboundColumnData [HIGH]
```
Pattern: gridView_CustomUnboundColumnData { BackendDataWorker.Get<T>() }
Fix: Pre-compute vào ADO trước khi bind
```

### P7. Client-side paging [HIGH]
```
Pattern: allData.Skip(start).Take(pageSize)
Fix: Server-side paging qua CommonParam(start, limit)
```

### P8. Tạo object trong loop [MEDIUM]
```
Pattern: for (10000) { new CommonParam(); new Filter(); }
Fix: Tạo ngoài loop, reuse
```

## Bước 2: Phân loại theo severity

CRITICAL: O(n²), Get trong loop
HIGH: string +=, API trong UnboundColumnData, client paging
MEDIUM: Count()>0, ToList thừa, new trong loop

## Bước 3: Tự động fix

Với mỗi anti-pattern tìm được:
1. Hiển thị code cũ (SAI)
2. Sinh code mới (ĐÚNG) với độ phức tạp giảm
3. Giữ nguyên logic nghiệp vụ — chỉ tối ưu cấu trúc

### Fix O(n²) → O(n):
```csharp
// Tự động chuyển:
var ids = new HashSet<long>(sourceList.Select(o => o.ID));
var result = targetList.Where(o => ids.Contains(o.ID)).ToList();
```

### Fix Get trong loop → Dictionary:
```csharp
// Tự động chuyển:
var dict = BackendDataWorker.Get<T>().ToDictionary(o => o.ID);
foreach (var item in list)
{
    dict.TryGetValue(item.FOREIGN_ID, out var related);
}
```

### Fix string += → StringBuilder/Join:
```csharp
// Tự động chuyển:
string result = String.Join(", ", list.Select(o => o.NAME));
```

## Bước 4: Output

```
FILE: {path}
FIXES: {số lượng}
  [CRITICAL] Line {n}: O(n×m) → O(n+m) — {mô tả}
  [HIGH] Line {n}: Get trong loop → Dictionary — {mô tả}
  [MEDIUM] Line {n}: Count()>0 → Any() — {mô tả}

TRƯỚC: {tổng độ phức tạp cũ}
SAU: {tổng độ phức tạp mới}
```
