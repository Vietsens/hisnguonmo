# TÀI LIỆU KỸ THUẬT

## Hiển thị đơn thuốc dự trù theo ngày dự trù — Plugin BedRoomPartial

| Thông tin | Nội dung |
|---|---|
| Plugin ID | `HIS.Desktop.Plugins.BedRoomPartial` |
| Loại | UserControl (`UserControlBase`) |
| UC chính | `Run/UCBedRoomPartial.cs` — 3691 dòng, đã tách 5 partial `__Pluss__` |
| Tài liệu nghiệp vụ | `A_NghiepVu_DonDuTruTheoNgayDuTru_BuongBenh.docx` |
| Backend | **Không cần sửa** — filter đã có sẵn |
| Ước lượng | ~450 dòng, 12 file, không thêm reference DLL |

Quy tắc nghiệp vụ tham chiếu bằng mã `QT-xx` — tra ở tài liệu A, Phần 3.

---

# PHẦN 1. HIỆN TRẠNG

## 1.1 Luồng hiện tại

```
SelectPatient(rowBedRoom, treatmentId)                     UCBedRoomPartial.cs:1020
  └─> LoadDataDateByTreatmentToTreeList(treatmentId)                        :1532
        ├─ GET HIS_SERVICE_REQ_GET_GROUP_BY_DATE -> HisServiceReqGroupByDateSDO
        ├─ GET api/HisTracking/Get               -> node con = tờ điều trị
        └─> LoadDataSereServByTreatmentId(rowClickByDate)                   :1854
              ├─ GET api/HisSereServ/GetDHisSereServ2
              │       { TREATMENT_ID, INTRUCTION_DATE = <ngày chọn> }
              ├─ GET api/HisServiceReq/Get { IDs }
              └─ dựng List<SereServADO> -> 4 tab UCTreeListService
                   ucAll <- GroupDataByTracking(...)                        :2072

treeListDateTime_FocusedNodeChanged -> LoadDataSereServByTreatmentId        :2503
```

## 1.2 Hai nút thắt

| # | Vị trí | Vấn đề |
|---|---|---|
| 1 | `UCBedRoomPartial.cs:1538` | Cây ngày lấy từ API group-by-date → chỉ có ngày **có y lệnh được kê**. Mùng 7 không có y lệnh nào thì không có node. |
| 2 | `UCBedRoomPartial.cs:1867` | `_sereServ2Filter.INTRUCTION_DATE = <ngày chọn>` → đơn kê mùng 6 bị loại **ở tầng SQL**, client không lấy lại được. |

## 1.3 Phần đã có sẵn

Code đã nhận biết đơn dự trù nhưng nhét tạm chữ vào cột "Khoa yêu cầu" — `:1918-1921` (tab CLS/Thuốc/Khác) và `:2129-2133` (tab Tất cả), đánh dấu `//qtcode`:

```csharp
if (rootSety.First().USE_TIME.HasValue)
    ssRootSety.REQUEST_DEPARTMENT_NAME = string.Format("Dự trù: {0}", ...);
```

Đoạn này **giữ lại** cho nhánh không áp dụng tính năng (QT-02, QT-11) — xem 3.4.

## 1.4 Plugin khác có cùng đoạn code — KHÔNG SỬA

`ApprovaleDebate/frmApprovaleDebate.cs:293`, `ApprovalExamSpecialist/Run/frmApprovalExamSpecialist.cs:623`, `ApprovalExamAnesthesia/Run/frmApprovalExamAnesthesia.cs:745`.

---

# PHẦN 2. MÔ HÌNH DỮ LIỆU

Đã xác minh bằng reflection trên `LIB/MOS/MOS.EFMODEL.dll`, `MOS.SDO.dll`, `MOS.Filter.dll`.

## 2.1 Trường liên quan

| Entity | Trường | Ghi chú |
|---|---|---|
| `HIS_SERVICE_REQ` | `INTRUCTION_TIME` | Ngày kê |
| | `USE_TIME` | Ngày dự trù |
| | `USE_TIME_TO` | **KHÔNG dùng** — xem 2.3 |
| | `SERVICE_REQ_TYPE_ID` | Xác định "đơn thuốc" (QT-02) |
| | `IS_TEMPORARY_PRES` | Đơn **tạm** — khác đơn dự trù, không dùng |
| `DHisSereServ2` | `USE_TIME`, `SERVICE_REQ_ID`, `TRACKING_ID/TIME` | Không có `SERVICE_REQ_TYPE_ID` → phải join `HIS_SERVICE_REQ` |

## 2.2 Khả năng lọc backend

| Filter | Kết luận |
|---|---|
| `DHisSereServ2Filter` | Chỉ có `TREATMENT_ID`, `INTRUCTION_DATE`, `IS_NO_EXECUTE`, `TDL_MEDICINE_CONCENTRA`. Không lọc được theo ngày dự trù, **nhưng `INTRUCTION_DATE` là tùy chọn** — bỏ trống thì trả toàn đợt. |
| `HisServiceReqFilter` | Đã có `TREATMENT_ID`, `IDs`, `USE_TIME`, `USE_DATE_FROM/TO`, `USE_TIME_OR_INTRUCTION_TIME_FROM/TO` |

Bằng chứng `INTRUCTION_DATE` tùy chọn: `ApprovalExamSpecialist/Run/frmApprovalExamSpecialist.cs:580-582` và `ApprovalExamAnesthesia/Run/frmApprovalExamAnesthesia.cs:702-704` đang gọi chỉ với `TREATMENT_ID`.

→ **Không cần bổ sung API backend.**

## 2.3 Công thức xác định (QT-01, QT-02, QT-03)

```csharp
// Hằng "đơn thuốc" — QT-02. DONM (16) là đơn MÁU, loại trừ có chủ đích.
HIS_SERVICE_REQ_TYPE.ID__DONK  (6)   // đơn thuốc kho
HIS_SERVICE_REQ_TYPE.ID__DONTT (14)  // đơn thuốc tủ trực
HIS_SERVICE_REQ_TYPE.ID__DONDT (15)  // đơn thuốc điều trị
```

Bộ 3 này là tiêu chí "đơn thuốc" dùng nhất quán toàn codebase: `AssignPrescriptionPK/__Load.cs:247`, `__Check.cs:380`, `AllocateExecuteRoom/UCAllocateExecuteRoom_Print.cs:84-86`. `DONM` bị loại vì `AssignServiceEdit/Base/GlobalStore.cs:42` map `DONM → HIS_SERVICE_TYPE.ID__MAU`.

**Đơn thuốc dự trù**:

```
SERVICE_REQ_TYPE_ID ∈ { DONK, DONTT, DONDT }                (QT-02)
&& USE_TIME.HasValue
&& date(USE_TIME) != date(INTRUCTION_TIME)                   (QT-01)
```

Vế so sánh ngày **bắt buộc**: một số luồng kê đơn gán `USE_TIME = INTRUCTION_TIME` khi bỏ trống ô "Dự trù" (`frmAssignPrescription__Edit.cs:57-58` các plugin `AssignPrescription*`). Chỉ kiểm tra `HasValue` sẽ hiểu nhầm đơn thường thành đơn dự trù.

**Ngày hiệu lực** — mỗi y lệnh đúng **1** ngày, dạng `yyyyMMdd000000`:

| Loại | Ngày hiệu lực |
|---|---|
| Đơn thuốc dự trù | `date(USE_TIME)` |
| Còn lại | `date(INTRUCTION_TIME)` |

**Không dùng `USE_TIME_TO`** (QT-05): backend đã tách đơn dự trù nhiều ngày thành nhiều `HIS_SERVICE_REQ`, mỗi đơn một `USE_TIME`. Ánh xạ y lệnh → ngày là **1:1**.

## 2.4 Đặt điều kiện ở tầng `HIS_SERVICE_REQ` — vì sao

Cho phép dựng cây ngày **chỉ từ cache y lệnh**, không phải chờ dữ liệu `sere_serv`.

---

# PHẦN 3. GIẢI PHÁP

## 3.1 Chiến lược nạp dữ liệu

Config bật → đổi từ *nạp-theo-ngày* sang *nạp-theo-đợt + lọc client*:

| | Config TẮT | Config BẬT |
|---|---|---|
| `DHisSereServ2Filter` | `{ TREATMENT_ID, INTRUCTION_DATE }` | `{ TREATMENT_ID }` |
| `HisServiceReqFilter` | `{ IDs }` mỗi lần đổi ngày | `{ TREATMENT_ID }` — 1 lần |
| Đổi ngày | 2 API call | **0 API call** |

Lý do không chọn hướng gọi bổ sung theo từng ngày kê nguồn: đơn dự trù có thể kê từ nhiều ngày khác nhau → số call không xác định trước. Nạp cả đợt là 1 call cố định, kèm lợi ích chuyển ngày tức thời.

**Ngưỡng an toàn**: đợt điều trị `> 90` ngày → fallback nạp-theo-ngày, ghi `LogSystem.Debug`.

## 3.2 File mới `Run/UCBedRoomPartial__Pluss__Anticipate.cs`

```csharp
#region Anticipate — QT-01..QT-10

/// <summary>Loại y lệnh là "đơn thuốc" — QT-02. DONM (đơn máu) loại trừ có chủ đích.</summary>
private static readonly HashSet<long> MEDICINE_SERVICE_REQ_TYPE_IDS = new HashSet<long>
{
    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
};

private bool isShowAnticipateByUseDate;                              // config, đọc 1 lần
private Dictionary<long, HIS_SERVICE_REQ> dictServiceReqOfTreatment; // reqId -> y lệnh
private Dictionary<long, long> dictEffectiveDateByReqId;             // reqId -> ngày (QT-03)
private HashSet<long> anticipateReqIds;                              // reqId đơn dự trù (QT-08)
private List<DHisSereServ2> allSereServOfTreatment;                  // cache toàn đợt
private long cachedTreatmentId;
private const int ANTICIPATE_MAX_DAYS = 90;
#endregion
```

### Năm hàm mới

| Hàm | Nhiệm vụ |
|---|---|
| `LoadAnticipateCache(long treatmentId)` | Nạp toàn bộ `HIS_SERVICE_REQ` + `DHisSereServ2` của đợt, dựng 3 cấu trúc trên. Gọi 1 lần trong `SelectPatient` |
| `IsAnticipateMedicinePres(HIS_SERVICE_REQ)` | QT-01 + QT-02 |
| `GetEffectiveDate(HIS_SERVICE_REQ)` | QT-03 |
| `BuildTreeDates(List<HisServiceReqGroupByDateSDO>)` | Danh sách ngày dựng cây, giảm dần |
| `FilterSereServByEffectiveDate(long dateNumber)` | Lọc dòng dịch vụ của 1 ngày từ cache |

```csharp
private bool IsAnticipateMedicinePres(HIS_SERVICE_REQ req)
{
    try
    {
        if (req == null || !req.USE_TIME.HasValue) return false;
        if (!MEDICINE_SERVICE_REQ_TYPE_IDS.Contains(req.SERVICE_REQ_TYPE_ID)) return false;  // QT-02
        return (req.USE_TIME.Value / 1000000) != (req.INTRUCTION_TIME / 1000000);            // QT-01
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
        return false;
    }
}
```

```csharp
// FilterSereServByEffectiveDate — O(n), lookup O(1)
long effDate;
return allSereServOfTreatment
    .Where(o => dictEffectiveDateByReqId.TryGetValue(o.SERVICE_REQ_ID ?? 0, out effDate)
                && effDate == dateNumber)
    .ToList();
```

## 3.3 Điểm sửa trong code hiện có

### (a) `SelectPatient` — `:1020`

Chèn `LoadAnticipateCache(treatmentId)` **trước** `LoadDataDateByTreatmentToTreeList(treatmentId)`.

### (b) `LoadDataDateByTreatmentToTreeList` — `:1532`

```
if (!isShowAnticipateByUseDate)  -> giữ nguyên 100% logic cũ            (QT-11)
else:
    listDates = distinct( dictEffectiveDateByReqId.Values ) giảm dần
      // Đơn dự trù mang ngày dự trù, đơn thường mang ngày kê -> danh sách ngày
      // TỰ ĐỘNG đúng: ngày kê chỉ còn toàn đơn dự trù sẽ biến mất  (QT-04, QT-06)
      // Ngày dự trù tương lai vẫn nằm trong danh sách              (QT-10)
    node con (tracking) giữ nguyên cách ghép theo TRACKING_TIME
```

**Vẫn giữ** lời gọi API group-by-date làm nguồn đối chiếu và đường lui khi cache rỗng/lỗi.

`ADO/ServiceReqGroupByDateADO.cs` cần **constructor mới** `(long instructionDate, long treatmentId, List<long> listTreeListIDs)` — ngày dự trù không có bản ghi `HisServiceReqGroupByDateSDO` để map.

### (c) `LoadDataSereServByTreatmentId` — `:1854`

Chỉ đổi khối lấy dữ liệu đầu hàm; phần dựng cây phía sau **giữ nguyên**:

```csharp
if (isShowAnticipateByUseDate && allSereServOfTreatment != null)
{
    dataNew        = FilterSereServByEffectiveDate(dateNumber);   // lọc RAM
    dataServiceReq = dictServiceReqOfTreatment.Values.ToList();   // đã cache
}
else
{
    // ... nguyên khối gọi API cũ (2 call) ...
}
```

### (d) Khối gán hiển thị — `:1916-1934` và `:2121-2148`

```csharp
HIS_SERVICE_REQ req = null;
bool isAnticipate = isShowAnticipateByUseDate
    && dictServiceReqOfTreatment != null
    && dictServiceReqOfTreatment.TryGetValue(rootSety.First().SERVICE_REQ_ID ?? 0, out req)
    && anticipateReqIds.Contains(req.ID);

if (isAnticipate)
{
    // QT-07, QT-08 — 2 cột riêng, KHÔNG đè REQUEST_DEPARTMENT_NAME
    ssRootSety.IS_ANTICIPATE = true;
    ssRootSety.INSTRUCTION_DATE_STR = ToDateString(req.INTRUCTION_TIME);
    ssRootSety.USE_DATE_STR         = ToDateString(req.USE_TIME ?? 0);
}
else if (rootSety.First().USE_TIME.HasValue)
{
    // QT-02 + QT-11 — config tắt, HOẶC y lệnh dự trù KHÔNG phải đơn thuốc
    // (dịch vụ, giường, đơn máu) -> giữ nguyên đoạn //qtcode cũ
    ssRootSety.REQUEST_DEPARTMENT_NAME = string.Format("Dự trù: {0}", ...);
}
```

**Nhánh `else if` là điểm mấu chốt của QT-02** — dịch vụ/giường có dự trù vẫn ở ngày kê và vẫn hiện chữ "Dự trù: …" như cũ, kể cả khi config bật.

### (e) `GroupDataByTracking` (tab "Tất cả") — `:2072`

Tab này nhóm cấp 1 theo `TRACKING_TIME`, mà đơn dự trù mang `TRACKING_ID` của ngày kê. Khi config bật, tách đơn dự trù "đến từ ngày khác" thành nhóm cấp 1 riêng đặt lên đầu (QT-09):

```
[Dự trù — kê ngày 06/08]     <- SereServADO gốc, SERVICE_CODE = tên nhóm
[07/08/2026 08:30]           <- tờ điều trị của chính ngày 07, logic cũ
```

Đơn kê trong chính ngày đang xem vẫn nhóm theo tracking như cũ.

## 3.4 Cấu hình

| Hạng mục | Giá trị |
|---|---|
| Key | `HIS.Desktop.Plugins.BedRoomPartial.ShowAnticipatePresByUseDate` |
| Nguồn | `HIS_CONFIG` toàn viện, `"1"` = bật |
| Mặc định | Tắt |

Thêm hằng vào `Key/HisConfigKeys.cs`, property `ShowAnticipatePresByUseDate` vào `Key/HisConfigCFG.cs` theo pattern `AssignBedOption` đang có (try-catch, `return false` khi lỗi).

**Bắt buộc**: đọc 1 lần vào field trong `UCBedRoomPartial_Load` — không gọi `HisConfigs.Get` trong vòng lặp dựng cây.

---

# PHẦN 4. GIAO DIỆN

**`ADO/SereServADO.cs`** — 3 property: `INSTRUCTION_DATE_STR` (string, cột Ngày kê), `USE_DATE_STR` (string, cột Ngày dự trù), `IS_ANTICIPATE` (bool, đánh dấu màu). Đều cần XML comment kèm mã `QT`.

**`UCTreeListService.Designer.cs`** — 2 cột đặt sau `tc_RequestDepartmentName` (`VisibleIndex = 11`):

| Cột | Caption | FieldName | Width |
|---|---|---|---|
| `tc_InstructionDate` | Ngày kê | `INSTRUCTION_DATE_STR` | 80 |
| `tc_UseDate` | Ngày dự trù | `USE_DATE_STR` | 90 |

Cả hai `AllowEdit = false`, `Visible = false` mặc định; `UCTreeListService.ReLoad` bật `Visible` theo cờ config (QT-11).

**Đánh dấu (QT-08)** — `treeSereServ.NodeCellStyle`, chỉ so sánh `data.IS_ANTICIPATE` rồi set `ForeColor = Color.Green`. Không LINQ, không gọi cache trong event này (`ui_rules` mục 8). Tooltip lấy từ `ResourceMessage`, truyền 2 tham số ngày.

**Đa ngôn ngữ** — 3 key caption vào `Lang.vi/en/my.resx` theo pattern `IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE__COL__INSTRUCTION_DATE` / `__COL__USE_DATE` / `__GROUP__ANTICIPATE` (`Dự trù — kê ngày {0}`). Câu tooltip vào `Message.Lang.*.resx` + property trong `ResourceMessage.cs`.

---

# PHẦN 5. HIỆU NĂNG

| Kịch bản | Hiện tại | Sau khi bật config |
|---|---|---|
| Chọn bệnh nhân | 4 call | 4 call, nhưng lấy **toàn đợt** |
| Đổi ngày trên cây | **2 call/lần** | **0 call** |
| BN nằm > 90 ngày | — | Fallback nạp-theo-ngày |

## Nợ hiệu năng cần dọn kèm

`:1898-1903` gọi `dataServiceReq.Where(...)` **4 lần liên tiếp trên cùng một điều kiện**, bên trong vòng lặp:

```csharp
// SAU — 1 lookup O(1), dùng lại dictServiceReqOfTreatment đã có
HIS_SERVICE_REQ sr;
if (dictServiceReqOfTreatment.TryGetValue(types.First().SERVICE_REQ_ID ?? 0, out sr))
{
    idSerReqType        = sr.SERVICE_REQ_TYPE_ID;
    idDepartment        = sr.REQUEST_DEPARTMENT_ID;
    idExecuteDepartment = sr.EXECUTE_DEPARTMENT_ID;
    IsTemporaryPres     = sr.IS_TEMPORARY_PRES;
}
```

Đoạn tương tự lặp ở `:2103-2111` trong `GroupDataByTracking` — sửa cả hai.

## Ràng buộc bắt buộc

Lookup dùng `Dictionary`/`HashSet`, không `FirstOrDefault` trong loop. `NodeCellStyle` chỉ so sánh bool. `BestFitColumns()` giữ nguyên vị trí. Không log trong vòng lặp dựng cây.

`__Pluss__Dispose.cs` — null 4 field cache trong `ProcessDisposeModuleDataAfterClose`.

---

# PHẦN 6. DANH SÁCH FILE

## Tạo mới

`Run/UCBedRoomPartial__Pluss__Anticipate.cs`

## Sửa

| File | Nội dung |
|---|---|
| `Key/HisConfigKeys.cs` | Hằng key config |
| `Key/HisConfigCFG.cs` | Property `ShowAnticipatePresByUseDate` |
| `Run/UCBedRoomPartial.cs` | `:1020`, `:1532`, `:1854`, `:2072`; 2 khối `//qtcode` (`:1918`, `:2129`); 2 khối lookup O(n) (`:1898`, `:2103`) |
| `Run/UCBedRoomPartial__Pluss__Dispose.cs` | Null 4 field cache |
| `ADO/SereServADO.cs` | 3 property |
| `ADO/ServiceReqGroupByDateADO.cs` | Constructor dựng node từ ngày dự trù |
| `UCTreeListService.Designer.cs` / `.cs` | 2 cột, bật/ẩn theo cờ, `NodeCellStyle`, tooltip |
| `Resources/Lang.*.resx`, `Message.Lang.*.resx`, `ResourceMessage.cs` | 3 key caption + tooltip |
| `docs/HIS.Desktop.Plugins.BedRoomPartial.md` | Changelog + section 2, 4, 5 |

## Không đụng tới

Backend `MOS` (filter đã đủ), `.csproj`, và 3 plugin ở mục 1.4.

---

# PHẦN 7. THỨ TỰ TRIỂN KHAI

Config (3.4) → `IsAnticipateMedicinePres` + `GetEffectiveDate` (3.2) → `LoadAnticipateCache` + `SelectPatient` → `BuildTreeDates` + sửa `:1532` → `FilterSereServByEffectiveDate` + sửa `:1854` → dọn nợ hiệu năng (Phần 5) → ADO + 2 cột + `NodeCellStyle` → `GroupDataByTracking` → đa ngôn ngữ + Dispose + log.

**Hai mốc kiểm chứng bắt buộc**: sau bước `:1854` phải chọn mùng 7 thấy được đơn kê mùng 6 (QT-03); trước bàn giao, Nhóm A của tài liệu A (config tắt) phải **pass 100%**.

---

# PHẦN 8. TEST KỸ THUẬT

Ngoài test nghiệp vụ ở tài liệu A Phần 5:

- [ ] T1 — Config bật, đổi ngày 10 lần → **không** phát sinh API call mới
- [ ] T2 — Config tắt → số API call khi đổi ngày **bằng đúng** bản hiện tại (2 call)
- [ ] T3 — Đợt > 90 ngày → tự fallback, có log Debug, hiển thị vẫn đúng
- [ ] T4 — Cache rỗng hoặc API lỗi → cây vẫn dựng từ API group-by-date, không văng exception
- [ ] T5 — Sửa/xóa y lệnh trên đơn dự trù → cache nạp lại, cây refresh đúng ngày
- [ ] T6 — Đổi bệnh nhân → `cachedTreatmentId` đổi, không dùng nhầm dữ liệu BN cũ
- [ ] T7 — Đóng màn → 4 field cache được null
- [ ] T8 — Y lệnh có `USE_TIME` nhưng `SERVICE_REQ_TYPE_ID = DONM` → không vào `anticipateReqIds` (QT-02)
- [ ] T9 — Y lệnh có `USE_TIME` trùng `INTRUCTION_TIME` → không vào `anticipateReqIds` (QT-01)
- [ ] T10 — Build solution, không cảnh báo reference thiếu
