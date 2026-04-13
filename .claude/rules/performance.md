---
description: Tối ưu hiệu suất và độ phức tạp thuật toán — O(1) lookup, batch processing, giảm vòng lặp, xử lý dữ liệu lớn. BẮT BUỘC áp dụng khi xử lý list, grid, cache, API
paths:
  - "HIS/Plugins/**"
  - "UC/**"
  - "MPS/**"
---

# Performance — Tối Ưu Độ Phức Tạp Thuật Toán

Mục tiêu: giảm độ phức tạp xuống thấp nhất có thể. Mỗi thao tác trên dữ liệu phải chọn cấu trúc + thuật toán phù hợp.

---

## 1. ĐỘ PHỨC TẠP — Bảng Tham Chiếu Nhanh

| Thao tác | SAI (chậm) | ĐÚNG (nhanh) |
|----------|-----------|--------------|
| Tìm 1 item theo ID | `list.FirstOrDefault(o => o.ID == id)` **O(n)** | `dict[id]` **O(1)** |
| Kiểm tra tồn tại | `list.Contains(id)` **O(n)** | `hashSet.Contains(id)` **O(1)** |
| Tìm nhiều items theo IDs | `list.Where(o => ids.Contains(o.ID))` **O(n×m)** | `list.Where(o => hashSet.Contains(o.ID))` **O(n)** |
| Kiểm tra rỗng | `.Count() > 0` **O(n)** | `.Any()` **O(1)** |
| Ghép chuỗi N lần | `s += item` **O(n²)** | `StringBuilder` **O(n)** |
| Nhóm theo key | Nested loop **O(n²)** | `.GroupBy()` hoặc `.ToLookup()` **O(n)** |
| Giao/Hợp 2 tập | Nested loop **O(n×m)** | `HashSet.IntersectWith/UnionWith` **O(n+m)** |
| Lọc trùng | `list.Distinct()` mỗi lần **O(n)** | `HashSet<T>` xây 1 lần **O(n)** rồi lookup **O(1)** |

---

## 2. LOOKUP — Luôn O(1) Khi Tra Cứu Nhiều Lần

### Dictionary cho tra cứu theo key

```csharp
// TRƯỚC: O(n) mỗi lần tra cứu × m lần = O(n×m)
foreach (var treatment in treatments)  // m items
{
    var dept = departments.FirstOrDefault(o => o.ID == treatment.DEPARTMENT_ID); // O(n)
}

// SAU: O(n + m) — build dict O(n), tra cứu m lần O(1) mỗi lần
var deptDict = departments.ToDictionary(o => o.ID);
foreach (var treatment in treatments)
{
    HIS_DEPARTMENT dept;
    deptDict.TryGetValue(treatment.DEPARTMENT_ID, out dept); // O(1)
}
```

**Quy tắc**: Khi tra cứu >= 2 lần trong list >= 50 items → PHẢI dùng Dictionary.

### HashSet cho kiểm tra tồn tại

```csharp
// TRƯỚC: O(n) mỗi lần Contains × m lần Where = O(n×m)
var deletes = data.Where(o => signers.Select(p => p.SIGNER_ID).Contains(o.ID)).ToList();

// SAU: O(n + m) — build HashSet O(n), check m lần O(1)
var signerIds = new HashSet<long>(signers.Select(p => p.SIGNER_ID));
var deletes = data.Where(o => signerIds.Contains(o.ID)).ToList();
```

**Quy tắc**: Khi gọi `.Contains()` trong `.Where()` → PHẢI chuyển source sang HashSet trước.

### ToLookup / GroupBy cho nhóm dữ liệu

```csharp
// TRƯỚC: O(n²) — tìm nhóm cho mỗi item bằng nested loop
foreach (var service in services)
{
    var group = allServices.Where(o => o.SERVICE_TYPE_ID == service.SERVICE_TYPE_ID).ToList();
}

// SAU: O(n) — nhóm 1 lần, tra cứu O(1)
var lookup = allServices.ToLookup(o => o.SERVICE_TYPE_ID);
foreach (var service in services)
{
    var group = lookup[service.SERVICE_TYPE_ID]; // O(1)
}
```

### Chọn đúng collection

| Nhu cầu | Collection | Độ phức tạp |
|---------|-----------|-------------|
| Tra cứu theo key | `Dictionary<K,V>` | Get/Set: O(1) |
| Kiểm tra tồn tại | `HashSet<T>` | Contains: O(1) |
| Nhóm theo key | `ILookup<K,V>` (ToLookup) | Tra cứu: O(1) |
| Thứ tự + index | `List<T>` | Index: O(1), Search: O(n) |
| Thread-safe cache | `ConcurrentDictionary<K,V>` | Get/Set: O(1) |
| Queue xử lý | `Queue<T>` | Enqueue/Dequeue: O(1) |
| Unique items | `HashSet<T>` | Add/Contains: O(1) |

---

## 3. LINQ — Giảm Duyệt Không Cần Thiết

### Any() thay Count() > 0

```csharp
// SAI: O(n) — duyệt hết để đếm
if (list.Count() > 0) { ... }
if (list.Where(o => o.IS_ACTIVE == 1).Count() > 0) { ... }

// ĐÚNG: O(1) — dừng ngay khi tìm thấy 1
if (list.Any()) { ... }
if (list.Any(o => o.IS_ACTIVE == 1)) { ... }
```

### FirstOrDefault trực tiếp

```csharp
// SAI: O(n) ToList + O(n) FirstOrDefault = 2 lần duyệt
var x = list.Where(o => o.ROLE_ID == id).ToList().FirstOrDefault();

// ĐÚNG: O(n) 1 lần duyệt, dừng khi tìm thấy
var x = list.FirstOrDefault(o => o.ROLE_ID == id);
```

### Gộp điều kiện Where

```csharp
// SAI: 3 lần duyệt (mỗi Where 1 lần)
var result = list.Where(a).Where(b).Where(c).ToList();

// ĐÚNG: 1 lần duyệt
var result = list.Where(o => a(o) && b(o) && c(o)).ToList();
```

### ToList() chỉ khi THỰC SỰ cần

```csharp
// CHỈ ToList khi:
// 1. Bind vào GridControl.DataSource (cần List, không nhận IEnumerable)
// 2. Cần duyệt nhiều lần (IEnumerable chỉ duyệt 1 lần)
// 3. Cần Count/index access

// KHÔNG ToList khi:
// 1. Chỉ FirstOrDefault — dùng trực tiếp
// 2. Chỉ Any — dùng trực tiếp
// 3. Truyền vào method khác nhận IEnumerable<T>
```

---

## 4. VÒNG LẶP — Giảm Lặp Và Độ Phức Tạp

### Tách data preparation ra ngoài loop

```csharp
// TRƯỚC: O(n × m) — mỗi iteration gọi cache + FirstOrDefault
foreach (var item in treatments)  // n items
{
    var dept = BackendDataWorker.Get<HIS_DEPARTMENT>()     // cache lookup
        .FirstOrDefault(o => o.ID == item.DEPARTMENT_ID);  // O(m)
    var room = BackendDataWorker.Get<V_HIS_ROOM>()
        .FirstOrDefault(o => o.ID == item.LAST_ROOM_ID);
}

// SAU: O(n + m) — prepare 1 lần, loop nhẹ
var deptDict = BackendDataWorker.Get<HIS_DEPARTMENT>().ToDictionary(o => o.ID);
var roomDict = BackendDataWorker.Get<V_HIS_ROOM>().ToDictionary(o => o.ID);
foreach (var item in treatments)
{
    deptDict.TryGetValue(item.DEPARTMENT_ID, out var dept);  // O(1)
    roomDict.TryGetValue(item.LAST_ROOM_ID, out var room);   // O(1)
}
```

### Batch xử lý thay vì từng item

```csharp
// TRƯỚC: N API calls — mỗi item 1 request
foreach (var id in selectedIds)
{
    var result = new BackendAdapter(param).Post<bool>(URI_DELETE, consumer, id, param);
}

// SAU: 1 API call — gửi danh sách
var result = new BackendAdapter(param).Post<bool>(URI_DELETE_LIST, consumer, selectedIds, param);
```

### Reuse objects ngoài loop

```csharp
// SAI: Tạo object mới mỗi iteration — GC pressure
for (int i = 0; i < 10000; i++)
{
    var param = new CommonParam();
    var filter = new SomeFilter();
}

// ĐÚNG: Tạo 1 lần, reset nếu cần
var param = new CommonParam();
var filter = new SomeFilter();
for (int i = 0; i < 10000; i++)
{
    // Reset filter properties thay vì new
    filter.ID = ids[i];
}
```

---

## 5. STRING — O(n) Thay Vì O(n²)

```csharp
// SAI: O(n²) — mỗi += tạo string mới, copy tất cả ký tự cũ
string result = "";
foreach (var item in list)  // n items
    result += item.NAME + ", ";  // copy 0→1→2→...→n chars = n²/2

// ĐÚNG: O(n) — StringBuilder append trực tiếp
StringBuilder sb = new StringBuilder();
foreach (var item in list)
{
    if (sb.Length > 0) sb.Append(", ");
    sb.Append(item.NAME);
}

// TỐT NHẤT: O(n) — String.Join (tối ưu nội bộ)
string result = String.Join(", ", list.Select(o => o.NAME));
```

---

## 6. CACHE — Load 1 Lần, Dùng Nhiều

### BackendDataWorker + Dictionary pattern

```csharp
// Load danh mục 1 lần đầu, sau đó tra cứu O(1)
var allDepts = BackendDataWorker.Get<HIS_DEPARTMENT>();
var deptDict = allDepts.ToDictionary(o => o.ID);      // Build O(n), dùng O(1)
var deptByCode = allDepts.ToDictionary(o => o.DEPARTMENT_CODE);

// Lazy-load pattern cho module-level cache
private static Dictionary<long, HIS_DEPARTMENT> _deptDict;
public static Dictionary<long, HIS_DEPARTMENT> DeptDict
{
    get
    {
        if (_deptDict == null)
            _deptDict = BackendDataWorker.Get<HIS_DEPARTMENT>().ToDictionary(o => o.ID);
        return _deptDict;
    }
}
```

### KHÔNG gọi Get<T> trong loop

```csharp
// SAI: N lần gọi cache method
foreach (var item in treatments)
{
    var dept = BackendDataWorker.Get<HIS_DEPARTMENT>()
        .FirstOrDefault(o => o.ID == item.DEPARTMENT_ID);
}

// ĐÚNG: 1 lần get + Dictionary
var deptDict = BackendDataWorker.Get<HIS_DEPARTMENT>().ToDictionary(o => o.ID);
foreach (var item in treatments)
{
    deptDict.TryGetValue(item.DEPARTMENT_ID, out var dept);
}
```

---

## 7. GRID — Batch Rendering

### BeginUpdate/EndUpdate (BẮT BUỘC)

```csharp
gridView.BeginUpdate();
try
{
    gridControl.DataSource = processedData;
}
finally
{
    gridView.EndUpdate();
}
```

### Pre-compute data TRƯỚC khi bind

```csharp
// SAI: Tính toán trong CustomUnboundColumnData — gọi MỖI DÒNG mỗi lần repaint
private void gridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
{
    var data = ...;
    // KHÔNG: Gọi API, BackendDataWorker, hoặc tính toán nặng ở đây
    // KHÔNG: Dictionary.ToDictionary() ở đây
}

// ĐÚNG: Tính toán 1 lần trước khi bind, lưu vào ADO property
// Tạo ADO mở rộng
public class TreatmentADO : V_HIS_TREATMENT
{
    public string DepartmentName { get; set; }    // Pre-computed
    public string StatusDisplay { get; set; }      // Pre-computed
    public string CreateTimeDisplay { get; set; }  // Pre-computed
}

// Tính toán TRƯỚC khi bind
var deptDict = BackendDataWorker.Get<HIS_DEPARTMENT>().ToDictionary(o => o.ID);
var adoList = treatments.Select(o => {
    var ado = new TreatmentADO();
    Mapper.Map(o, ado);
    deptDict.TryGetValue(o.DEPARTMENT_ID, out var dept);
    ado.DepartmentName = dept?.DEPARTMENT_NAME;
    ado.CreateTimeDisplay = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(o.CREATE_TIME ?? 0);
    return ado;
}).ToList();

gridControl.DataSource = adoList;
// CustomUnboundColumnData CHỈ còn làm việc nhẹ: STT, icon
```

### SuspendLayout cho form init

```csharp
this.SuspendLayout();
this.layoutControl1.SuspendLayout();
// ... thêm controls ...
this.layoutControl1.ResumeLayout(false);
this.ResumeLayout(false);
```

---

## 8. API QUERY — Giảm Data Từ Server

### Server-side sort (KHÔNG sort client)

```csharp
filter.ORDER_DIRECTION = "DESC";
filter.ORDER_FIELD = "MODIFY_TIME";
```

### Server-side paging (KHÔNG load all)

```csharp
CommonParam param = new CommonParam(startPage, limit);
var apiResult = new BackendAdapter(param).GetRO<List<V_HIS_TREATMENT>>(
    uri, consumer, filter, param);
// Chỉ nhận trang hiện tại

// KHÔNG:
var all = adapter.Get<List<T>>(uri, consumer, filter, param);
var page = all.Skip(start).Take(size).ToList(); // load 10000 để lấy 50
```

### Filter cụ thể — càng nhiều càng nhanh

```csharp
filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
filter.DEPARTMENT_ID = currentDeptId;
filter.CREATE_TIME_FROM = fromTime;
filter.CREATE_TIME_TO = toTime;
// Server xử lý WHERE clause = nhanh hơn client filter
```

---

## 9. MEMORY — Giảm Áp Lực GC

### Null references khi đóng form

```csharp
currentModuleBase = null;
currentData = null;
listData = null;
// FormBase.OnFormClosing và UCBase.DisposeExt tự động làm
```

### using cho IDisposable

```csharp
using (var stream = new MemoryStream()) { ... }
using (var reader = new StreamReader(path)) { ... }
```

### KHÔNG giữ large objects không cần

```csharp
// SAI: Field 10000 items không bao giờ clear
private List<V_HIS_TREATMENT> _allTreatments;

// ĐÚNG: Clear khi không cần
_allTreatments = null; // hoặc .Clear()
```

---

## 10. BẢNG ĐỘ PHỨC TẠP — Tham Chiếu Khi Code

| Pattern | Độ phức tạp | Ghi chú |
|---------|------------|---------|
| `dict[key]` | O(1) | Nhanh nhất cho lookup |
| `hashSet.Contains(x)` | O(1) | Nhanh nhất cho kiểm tra |
| `list[index]` | O(1) | Truy cập theo vị trí |
| `list.Any()` | O(1) | Dừng ngay khi tìm thấy |
| `list.FirstOrDefault(predicate)` | O(n) | Duyệt tới khi tìm thấy |
| `list.Where(predicate).ToList()` | O(n) | 1 lần duyệt |
| `list.OrderBy(key)` | O(n log n) | Sort |
| `String.Join(sep, list)` | O(n) | Ghép chuỗi tối ưu |
| `StringBuilder.Append()` | O(1) mỗi lần | Tổng O(n) |
| `list.Contains(x)` | O(n) | CHẬM — dùng HashSet |
| `nested loop` | O(n×m) | TRÁNH — dùng Dictionary/HashSet |
| `string += trong loop` | O(n²) | TRÁNH — dùng StringBuilder |

---

## Tổng Hợp Quy Tắc

| Quy tắc | Thực hiện |
|---------|-----------|
| Lookup >= 2 lần | `Dictionary<K,V>` — KHÔNG FirstOrDefault |
| Contains trong Where | `HashSet<T>` — KHÔNG List.Contains |
| Nhóm dữ liệu | `ToLookup()` / `GroupBy()` — KHÔNG nested loop |
| Kiểm tra rỗng | `Any()` — KHÔNG Count() > 0 |
| Ghép chuỗi loop | `StringBuilder` / `String.Join` — KHÔNG += |
| Danh mục | Load 1 lần + Dictionary — KHÔNG Get<T> trong loop |
| Grid data | Pre-compute ADO trước bind — KHÔNG tính trong UnboundColumnData |
| API data | Server sort + paging + filter — KHÔNG load all |
| Batch | 1 API call cho list — KHÔNG N calls trong loop |
| Memory | Null references, using, clear large collections |
