---
description: Review performance theo performance.md — độ phức tạp thuật toán, LINQ, collections, cache, grid, API, memory
argument-hint: <file hoặc folder path>
---

# Review Performance

Review: $ARGUMENTS

## 1. Độ Phức Tạp Thuật Toán
- Có `list.FirstOrDefault()` lặp nhiều lần trong loop? → chuyển sang Dictionary O(1)
- Có `.Contains()` trong `.Where()` trên List? → chuyển sang HashSet O(1)
- Có nested loop (O(n×m))? → chuyển sang Dictionary/HashSet/ToLookup O(n+m)
- Có `.Count() > 0`? → đổi sang `.Any()` O(1)

## 2. LINQ
- Có `.ToList().FirstOrDefault()`? → bỏ ToList, gọi FirstOrDefault trực tiếp
- Có chain `.Where().Where().Where()`? → gộp thành 1 Where
- Có `ToList()` khi chỉ cần Any/First/Count? → bỏ ToList

## 3. Collections
- Tra cứu theo key >= 2 lần trong list >= 50 items? → PHẢI dùng Dictionary
- Kiểm tra tồn tại trong tập lớn? → PHẢI dùng HashSet
- Nhóm dữ liệu? → ToLookup/GroupBy thay nested loop

## 4. String
- Có `string +=` trong loop? → StringBuilder hoặc String.Join
- Có StringBuilder khi chỉ join list? → String.Join đơn giản hơn

## 5. Cache / BackendDataWorker
- Có `BackendDataWorker.Get<T>()` trong vòng lặp? → lấy 1 lần trước loop
- Có gọi Get<T>() rồi FirstOrDefault nhiều lần? → ToDictionary 1 lần
- Có load tất cả rồi filter client-side? → server-side filter

## 6. GridControl
- Có BeginUpdate/EndUpdate khi bind data?
- Có tính toán nặng trong CustomUnboundColumnData? → pre-compute ADO
- Có API call trong RowCellStyle/CustomUnboundColumnData? → cache trước
- Có tắt ShowGroupPanel, ShowIndicator, AllowFindPanel?

## 7. API Query
- Có ORDER_DIRECTION trên server? (không sort client)
- Có server-side paging? (không load all rồi Skip/Take)
- Filter có cụ thể (IS_ACTIVE, DEPARTMENT_ID, TIME_FROM/TO)?

## 8. Memory
- Có null references trong ProcessDisposeModuleDataAfterClose?
- Có giữ large List<T> lâu không cần?
- Có using cho stream/connection?
- Có tạo objects trong loop không cần thiết?

## Output
[CRITICAL] O(n²) hoặc O(n×m) — file:line — fix
[HIGH] Get<T> trong loop, string += trong loop — file:line — fix
[MEDIUM] Count()>0, ToList thừa, thiếu BeginUpdate — file:line — fix
[LOW] Thiếu null references khi close — file:line — fix
