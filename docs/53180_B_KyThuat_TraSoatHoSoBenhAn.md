# Việc 53180 — TÀI LIỆU KỸ THUẬT

## Lọc theo bác sĩ chỉ định và tạo văn bản — Plugin HisTreatmentRecordChecking

| Thông tin | Nội dung |
|---|---|
| Mã việc | 53180 |
| Plugin ID | `HIS.Desktop.Plugins.HisTreatmentRecordChecking` |
| Loại | Form (`FormBase`) |
| Đường dẫn | `HIS/Plugins/HIS.Desktop.Plugins.HisTreatmentRecordChecking/` |
| Form chính | `RecordChecking/FormHisTreatmentRecordChecking.cs` (hiện 2081 dòng) |
| Tài liệu nghiệp vụ | 53180_A_NghiepVu_TraSoatHoSoBenhAn.docx |
| Trạng thái | **Đã code cả frontend và backend** — còn 5 điểm chặn ở Phần 11 |
| Backend | `BACKEND/MOS` — endpoint mới ở Phần 4.1 |

**Tài liệu này dành cho lập trình viên.**
Mọi quy tắc nghiệp vụ được tham chiếu bằng mã `QT-xx` — tra nội dung đầy đủ ở tài liệu A, Phần 4.

---

# PHẦN 1. HIỆN TRẠNG PLUGIN

## 1.1 Cấu trúc hiện tại

```
HIS.Desktop.Plugins.HisTreatmentRecordChecking/
├── HisTreatmentRecordCheckingProcessor.cs        ← MEF entry point
├── HisTreatmentRecordChecking/
│   ├── IHisTreatmentRecordChecking.cs
│   ├── HisTreatmentRecordCheckingFactory.cs
│   └── HisTreatmentRecordCheckingBehavior.cs     ← nhận Module, long, List<long>
├── RecordChecking/
│   ├── FormHisTreatmentRecordChecking.cs         ← 2081 dòng, CHƯA tách partial
│   ├── FormHisTreatmentRecordChecking.Designer.cs
│   └── frmContentFailed.cs                       ← nhập lý do "Chưa đạt"
├── ADO/
│   ├── EmrDocumentTypeADO.cs
│   └── InfoRecordADO.cs
└── Properties/AssemblyInfo.cs

THIẾU: Resources/ (đa ngôn ngữ), HisRequestUriStore.cs, ModuleLinkString.cs
```

## 1.2 Luồng nạp dữ liệu hiện tại

```
FormHisTreatmentRecordChecking_Load()
 ├── GetControlAcs()                    → API ACS_TOKEN__AUTHORIZE
 ├── InitConfigAndPermission()          → config MOS.HIS_TREATMENT.IS_AUTO_APPROVAL_STORE
 ├── FillDataToGridTreatment(listId)    → API HIS_TREATMENT_GETVIEW   (chỉ khi mở từ plugin khác)
 ├── InitGridEmrDocumentType()          → API api/EmrDocumentType/Get
 ├── SetDefaultValueControl()
 ├── ProcessCaptionGridInfoRecord()
 ├── InitControlState()                 → SQLite
 ├── FillDataToGrid()                   ← LÕI
 └── SetDefaultProperties()

FillDataToGrid()
 ├── guard: TxtTreatmentCode rỗng && treatmentId null → return
 ├── CurrentType ??= ListDocumentType.First()
 ├── GetDataTreatment()                 → API api/HisTreatment/GetInfoForRecordChecking
 │    ├── FillDataToControl()           → đổ 14 label + enable 4 nút
 │    └── ProcessDataADO()              → gộp 7 nguồn thành List<InfoRecordADO>
 ├── EmrDocument()                      → API api/EmrDocument/GetView
 ├── OrderListByCheckBox()              → sắp xếp + bind grid trái
 └── ProcessFillDataToGrid()
      ├── loại ∈ ListTypeId  → ProcessDataGridInfoRecord()   (grid giữa)
      └── ngược lại          → ProcessDataGridDocument()     (grid phải + API EmrSign)
```

## 1.3 Bảy nguồn dữ liệu trong `ProcessDataADO()`

| Nguồn (từ `HisTreatmentForRecordCheckingSDO`) | Loại văn bản gán | `CREATOR` gán từ | `CREATE_TIME_STR` gán từ |
|---|---|---|---|
| `ServiceReqs` (loại đơn thuốc) | `ID__PRESCRIPTION` | `REQUEST_LOGINNAME` | `INTRUCTION_TIME` |
| `ServiceReqs` (loại khác) | `ID__SERVICE_ASSIGN` + nhân bản `ID__SERVICE_RESULT` | `REQUEST_LOGINNAME` | `INTRUCTION_TIME` |
| `Cares` | `ID__CARE` | `CREATOR` | `CREATE_TIME` |
| `Debates` | `ID__DEBATE` | `CREATOR` | `DEBATE_TIME` |
| `Infusions` | `ID__INFUSION` | `CREATOR` | `START_TIME – FINISH_TIME` |
| `MediReacts` | `ID__MEDI_REACT` | `CREATOR` | `EXECUTE_TIME` |
| `Trackings` | `ID__TRACKING` | `CREATOR` | `TRACKING_TIME` |
| `Transfusions` | `ID__TRANSFUSION` | `CREATOR` | `MEASURE_TIME` |

→ Đây là cơ sở của **QT-05**: nhóm `ServiceReqs` lọc theo `INTRUCTION_TIME`, sáu nhóm còn lại lọc theo `CREATE_TIME`.

---

# PHẦN 2. THAY ĐỔI GIAO DIỆN (Designer)

## 2.1 Control mới

| Tên control | Kiểu DevExpress | Vị trí | Nguồn dữ liệu | Ghi chú |
|---|---|---|---|---|
| `cboRequestDoctor` | `GridLookUpEdit` | Hàng trên cùng — `Point(285, 0)` W=270 | `BackendDataWorker.Get<HIS_EMPLOYEE>()` lọc `IS_ACTIVE == 1` và `LOGINNAME` khác rỗng | Display `TDL_USERNAME`, Value `LOGINNAME`, nút `Delete` **phải tự nối `ButtonClick`** |
| `dtFromDate` | `DateEdit` | Hàng trên cùng — `Point(555, 0)` W=165 | — | Mặc định ngày đầu tháng |
| `dtToDate` | `DateEdit` | Hàng trên cùng — `Point(720, 0)` W=165 | — | Mặc định hôm nay |
| `cboTreatmentStatus` | `LookUpEdit` | Hàng trên cùng — `Point(885, 0)` W=200 | Danh sách tĩnh 2 dòng | `0 = Chưa kết thúc`, `1 = Đã kết thúc` |
| `chkNoDocument` | `CheckEdit` | Hàng checkbox — `Point(371, 266)` | — | Caption "Chưa tạo văn bản" |
| `chkNotFullySigned` | `CheckEdit` | Hàng checkbox — `Point(521, 266)` | — | Caption "Chưa ký hết" |
| `repositoryItemButtonCreateDoc` | `RepositoryItemButtonEdit` | Cột `Gv_IR_CreateDoc` trong lưới giữa | — | Nút **[Tạo]** trên **từng dòng**, `TextEditStyle = HideTextEditor` |
| `repositoryItemTextEdit1` | `RepositoryItemTextEdit` | — | — | `ReadOnly = true`, dùng để **ẩn nút** ở dòng không đủ điều kiện |
| `ucPaging` | `Inventec.UC.Paging` | Dưới lưới giữa — `Point(220, 581)` | — | Chỉ hiện ở Cách 2 |

> **Không** dùng nút `SimpleButton` riêng ở cuối màn hình. Nút tạo văn bản nằm trên từng dòng lưới:
> `Gv_InfoRecord_CustomRowCellEdit` đổi `e.RepositoryItem` sang `repositoryItemTextEdit1` cho dòng
> không đủ điều kiện (QT-14), nên ô đó trống thay vì hiện nút mờ.

### Bố cục hàng trên cùng (tổng đúng 1264px, không hở không chồng lấn)

| Thứ tự | LayoutItem | Control | X | W |
|---|---|---|---|---|
| 1 | `layoutControlItem1` | `TxtTreatmentCode` | 0 | 285 |
| 2 | `lciRequestDoctor` | `cboRequestDoctor` | 285 | 270 |
| 3 | `lciFromDate` | `dtFromDate` | 555 | 165 |
| 4 | `lciToDate` | `dtToDate` | 720 | 165 |
| 5 | `lciTreatmentStatus` | `cboTreatmentStatus` | 885 | 200 |
| 6 | `layoutControlItem2` | **`BtnSearch`** | 1085 | 143 |
| 7 | `emptySpaceItem1` | — | 1228 | 36 |

Nút Tìm đặt **cuối hàng** theo đúng thứ tự thao tác. Khi sửa bố cục hàng này phải giữ hai điều:
thứ tự trong `layoutControlGroup1.Items.AddRange` khớp thứ tự `Location`, và **tổng `X + W` của mục cuối = 1264**.
`BtnSearch.TabIndex = 44` để Tab chạy qua hết bộ lọc rồi mới tới nút Tìm.

> **Bẫy Designer**: `RepositoryItemButtonEdit.BeginInit()` **xoá sạch** `Buttons`. Phải dùng
> `Buttons.AddRange(new EditorButton[] { ... })`, **không** được viết `Buttons[0].Kind = ...`
> (kiểu VS sinh ra khi kéo thả) — sẽ ném `ArgumentOutOfRangeException` ngay trong `InitializeComponent`
> và form không khởi tạo được.

## 2.2 Layout

Theo `ui_rules.md`:

```csharp
// Với 4 LayoutControlItem của cboRequestDoctor, dtFromDate, dtToDate, cboTreatmentStatus
lci.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
lci.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
lci.TextSize      = new System.Drawing.Size(95, 20);
```

- Trường bắt buộc ở Cách 2 (`dtFromDate`, `dtToDate`, `cboTreatmentStatus`) → caption **Maroon** khi ở Cách 2, màu mặc định khi ở Cách 1.
- Vùng trống chèn `EmptySpaceItem`, không đặt kích thước cứng.
- `layoutControlGroup.EnableIndentsWithoutBorders = true`.
- Chuẩn thiết kế 1366×768.

## 2.3 Cột mới ở `Gv_InfoRecord`

| Thứ tự | Tên cột | FieldName | Caption | Width | Hiện ở |
|---|---|---|---|---|---|
| 1 | (có sẵn) | `STT` | — (icon) | 24 | Cả hai |
| 2 | `Gv_IR_PatientCode` | `PATIENT_CODE` | Mã BN | 90 | Cách 2 |
| 3 | `Gv_IR_PatientName` | `PATIENT_NAME` | Tên BN | 150 | Cách 2 |
| 4 | `Gv_IR_TreatmentCode` | `TREATMENT_CODE` | Mã hồ sơ | 110 | Cách 2 |
| 5 | (có sẵn) | `CODE` | động theo loại | 110 | Cả hai |
| 6 | (có sẵn) | `TYPE` | động theo loại | 110 | Cả hai |
| 7 | (có sẵn) | `CREATE_TIME_STR` | động theo loại | 130 | Cả hai |
| 8 | `Gv_IR_CreateTimeReal` | `CREATE_TIME_REAL_STR` | **Thời gian tạo** | 130 | Cả hai |
| 9 | (có sẵn) | `DEPARTMENT_NAME` | động theo loại | 150 | Cả hai |
| 10 | (có sẵn) | `CREATOR` | động theo loại | 110 | Cả hai |
| 11 | `Gv_IR_DocStatus` | `DOC_STATUS_NAME` | Trạng thái VB | 110 | Cả hai |

**Lưu ý**:
- Cột 8 và 11 là **bound** vào property của ADO, KHÔNG dùng `UnboundType` → không tính toán khi repaint.
- Hàm `ProcessCaptionGridInfoRecord()` chỉ đổi caption 5 cột cũ (CODE, TYPE, CREATE_TIME_STR, DEPARTMENT_NAME, CREATOR). Cột 8 và 11 giữ caption cố định.
- Ẩn/hiện cột dùng `gridColumn.Visible = true/false`, KHÔNG tạo grid thứ hai.

---

# PHẦN 3. THAY ĐỔI TẦNG DỮ LIỆU

## 3.1 Bổ sung `ADO/InfoRecordADO.cs`

```csharp
class InfoRecordADO
{
    // ===== Giữ nguyên =====
    public long DOCUMENT_TYPE_ID { get; set; }
    public string CODE { get; set; }
    public string TYPE { get; set; }
    public string CREATE_TIME_STR { get; set; }   // thời gian nghiệp vụ (hiển thị)
    public string DEPARTMENT_NAME { get; set; }
    public string SEARCH_CODE { get; set; }
    public long REQ_TYPE_STT_ID { get; set; }
    public string CREATOR { get; set; }

    // ===== Bổ sung việc 53180 =====

    /// <summary>Thời gian tạo bản ghi. Dùng để lọc (QT-05) và hiển thị cột "Thời gian tạo".</summary>
    public long? CREATE_TIME { get; set; }

    /// <summary>Chuỗi hiển thị của CREATE_TIME. Bind vào cột Gv_IR_CreateTimeReal.</summary>
    public string CREATE_TIME_REAL_STR { get; set; }

    /// <summary>Mã hồ sơ điều trị. Chỉ dùng ở Cách 2 (đa hồ sơ).</summary>
    public string TREATMENT_CODE { get; set; }

    /// <summary>ID hồ sơ điều trị. Dùng khi nháy đúp để mở hồ sơ.</summary>
    public long? TREATMENT_ID { get; set; }

    public string PATIENT_CODE { get; set; }
    public string PATIENT_NAME { get; set; }

    /// <summary>
    /// Trạng thái văn bản của y lệnh (QT-09).
    /// BẮT BUỘC tính TRƯỚC khi bind grid — KHÔNG tính trong CustomUnboundColumnData.
    /// </summary>
    public EnumRecordDocumentStatus DOC_STATUS { get; set; }

    /// <summary>Tên trạng thái hiển thị, bind vào cột Gv_IR_DocStatus.</summary>
    public string DOC_STATUS_NAME { get; set; }
}
```

## 3.2 File mới `EnumRecordDocumentStatus.cs` (gốc plugin)

Không có trong `IMSys.DbConfig` vì đây là trạng thái tính phía client từ danh sách `V_EMR_DOCUMENT`.

```csharp
/// <summary>
/// Trạng thái văn bản của một y lệnh trên màn Tra soát hồ sơ bệnh án (việc 53180 — QT-09).
/// Tính từ danh sách V_EMR_DOCUMENT gắn với y lệnh qua HIS_CODE.
/// </summary>
public enum EnumRecordDocumentStatus
{
    /// <summary>Y lệnh chưa sinh văn bản nào.</summary>
    NoDocument = 0,

    /// <summary>Có văn bản nhưng chưa văn bản nào được ký.</summary>
    NotSigned = 1,

    /// <summary>Đã ký một phần — vẫn còn người chưa ký.</summary>
    Signing = 2,

    /// <summary>Tất cả văn bản của y lệnh đã ký đủ.</summary>
    FullySigned = 3
}
```

## 3.3 Hàm tính trạng thái (QT-09, QT-10)

```csharp
/// <summary>
/// Tính trạng thái văn bản của một y lệnh theo QT-09, QT-10.
/// QT-10: chỉ FullySigned khi TẤT CẢ văn bản đã ký đủ (dùng All, KHÔNG dùng Exists).
/// </summary>
private EnumRecordDocumentStatus CalcDocumentStatus(List<V_EMR_DOCUMENT> docs)
{
    if (docs == null || docs.Count == 0)
        return EnumRecordDocumentStatus.NoDocument;

    if (docs.All(o => string.IsNullOrEmpty(o.SIGNERS)))
        return EnumRecordDocumentStatus.NotSigned;

    if (docs.All(o => !string.IsNullOrEmpty(o.SIGNERS) && string.IsNullOrEmpty(o.UN_SIGNERS)))
        return EnumRecordDocumentStatus.FullySigned;

    return EnumRecordDocumentStatus.Signing;
}
```

### So sánh với code hiện tại

| | Code hiện tại (`Gv_InfoRecord_CustomUnboundColumnData` dòng ~1435) | Code mới |
|---|---|---|
| Vị trí tính | Trong `CustomUnboundColumnData` — chạy **mỗi cell mỗi lần repaint** | Tính **một lần** khi map ADO |
| Logic "đã ký" | `documents.Exists(...)` — chỉ cần **một** văn bản ký xong | `docs.All(...)` — **tất cả** phải ký xong |
| Gọi `GetDocumentByInfoRecod()` | Mỗi lần repaint | Không gọi lại |

Sau khi sửa, `Gv_InfoRecord_CustomUnboundColumnData` chỉ còn ánh xạ `DOC_STATUS` sang icon:

```csharp
if (e.Column.FieldName == "STT")
{
    switch (data.DOC_STATUS)
    {
        case EnumRecordDocumentStatus.NoDocument:  e.Value = imageList1.Images[3]; break;
        case EnumRecordDocumentStatus.NotSigned:   e.Value = imageList1.Images[5]; break;
        case EnumRecordDocumentStatus.Signing:     e.Value = imageList1.Images[2]; break;
        case EnumRecordDocumentStatus.FullySigned: e.Value = imageList1.Images[4]; break;
    }
}
```

---

# PHẦN 4. API

## 4.1 API mới — ĐÃ CODE

```
api/HisTreatment/GetServiceReqForRecordChecking
Consumer: ApiConsumers.MosConsumer
Gọi bằng: GetRO
```

### File backend

| Tầng | File |
|---|---|
| Filter | `MOS.Filter/HisServiceReqForRecordCheckingFilter.cs` |
| SDO | `MOS.SDO/HisServiceReqForRecordCheckingSDO.cs` |
| Xử lý | `MOS.MANAGER/HisTreatment/Get/HisTreatmentGetServiceReqForRecordChecking.cs` |
| Manager | `MOS.MANAGER/HisTreatment/HisTreatmentManagerSql.cs` |
| Controller | `MOS.API/Controllers/HisTreatmentControllerSql.cs` |

### Filter đầu vào — `HisServiceReqForRecordCheckingFilter : FilterBase`

| Trường | Kiểu | Bắt buộc | Ý nghĩa | Quy tắc |
|---|---|---|---|---|
| `REQUEST_LOGINNAME` | `string` | Có | Tài khoản bác sĩ | QT-01 |
| `FROM_TIME` | `long?` | Có | `yyyyMMdd000000` | QT-02 |
| `TO_TIME` | `long?` | Có | `yyyyMMdd235959` | QT-02 |
| `IS_END_TREATMENT` | `bool?` | Không | `true` = đã kết thúc, `false` = chưa, `null` = không lọc | QT-06 |

Thiếu một trong ba trường bắt buộc → ghi `LogSystem.Warn` và trả `null`.

**Không có `DOCUMENT_TYPE_ID`.** API luôn trả toàn bộ 7 nguồn y lệnh. Grid trái cần đếm được số
lượng của **mọi** loại văn bản; lọc theo loại ở server sẽ làm các loại khác mất dữ liệu. Client tự
tách theo loại đang chọn.

### Quy tắc lọc phía server (QT-05, QT-06)

11 câu `SELECT` rời, mỗi bảng một câu, cùng cách làm với `GetInfoForRecordChecking`:

```sql
-- Nhóm 1: y lệnh — lọc theo REQUEST_LOGINNAME + INTRUCTION_TIME
SELECT SRE.* FROM HIS_SERVICE_REQ SRE
  [JOIN HIS_TREATMENT TRE ON TRE.ID = SRE.TREATMENT_ID]    -- chỉ khi lọc trạng thái hồ sơ
 WHERE (SRE.IS_DELETE IS NULL OR SRE.IS_DELETE <> 1)
   AND (SRE.IS_NO_EXECUTE IS NULL OR SRE.IS_NO_EXECUTE <> 1)
   AND SRE.REQUEST_LOGINNAME = :param1
   AND SRE.INTRUCTION_TIME >= :param2 AND SRE.INTRUCTION_TIME <= :param3
   [AND TRE.OUT_TIME IS NOT NULL | IS NULL]

-- Nhóm 2: CARE / DEBATE / TRACKING — lọc theo CREATOR + CREATE_TIME, join thẳng TREATMENT_ID
-- Nhóm 3: INFUSION / MEDI_REACT / TRANSFUSION — như nhóm 2, nhưng join qua _SUM mới tới HIS_TREATMENT

-- Treatments + 3 bảng _SUM: nạp theo danh sách ID gom được từ 7 nguồn trên,
--   dùng AddInClause() của GetBase (tự tách thành nhiều mệnh đề IN khi vượt 1000 phần tử)
```

Khi `IS_END_TREATMENT = null` thì **bỏ hẳn** `JOIN HIS_TREATMENT` — với 3 nguồn qua `_SUM` là bớt
được 2 join mỗi câu.

> **`HIS_TREATMENT` KHÔNG có cột `END_TIME`.** Bản thiết kế trước ghi `END_TIME` là sai, chạy sẽ
> `ORA-00904`. Bảng chỉ có `IN_TIME`, `OUT_TIME`, `IS_PAUSE`, `END_LOGINNAME`, `END_ROOM_ID`,
> `END_CODE`, `TREATMENT_END_TYPE_ID`. Đang dùng **`OUT_TIME`** (đã ra viện). `IS_PAUSE` mang nghĩa
> *tạm dừng*, **không tương đương** — đừng thay lẫn nhau. Muốn đổi thì sửa duy nhất hàm
> `BuildTreatmentCondition()`.

**Không lọc theo trạng thái y lệnh.** Đã thử thêm `SERVICE_REQ_STT_ID <> ID__HT` rồi bỏ: lọc bỏ y
lệnh đã hoàn thành sẽ giấu mất chính những y lệnh cần tra soát chữ ký.

### Ghi log

Mỗi truy vấn ghi 3 dòng `Debug`: `...sql.<Tên>`, `...sqlParams.<Tên>`, `...rows.<Tên>`. Cuối hàm ghi
dòng tổng số bản ghi từng nguồn; vượt 20.000 dòng thì chuyển sang `Warn`.

### Kết quả trả về — `HisServiceReqForRecordCheckingSDO`

Trả về **dữ liệu thô của từng bảng**, giữ đúng hình dạng của `HisTreatmentForRecordCheckingSDO` (Cách 1).
Server **không** quy đổi loại văn bản — client tự quy đổi bằng `ProcessDataADO()`, dùng chung một hàm với Cách 1.

| Thuộc tính | Kiểu | Ghi chú |
|---|---|---|
| `Treatments` | `List<HIS_TREATMENT>` | Các hồ sơ có y lệnh thoả bộ lọc. Client tra `TREATMENT_CODE` / `TDL_PATIENT_CODE` / `TDL_PATIENT_NAME` từ đây |
| `ServiceReqs` | `List<HIS_SERVICE_REQ>` | Có sẵn `TREATMENT_ID` |
| `Trackings` | `List<HIS_TRACKING>` | Có sẵn `TREATMENT_ID` |
| `Cares` | `List<HIS_CARE>` | Có sẵn `TREATMENT_ID` |
| `Debates` | `List<HIS_DEBATE>` | Có sẵn `TREATMENT_ID` |
| `Infusions` | `List<HIS_INFUSION>` | **Không** có `TREATMENT_ID` |
| `MediReacts` | `List<HIS_MEDI_REACT>` | **Không** có `TREATMENT_ID` |
| `Transfusions` | `List<HIS_TRANSFUSION>` | **Không** có `TREATMENT_ID` |
| `InfusionSums` | `List<HIS_INFUSION_SUM>` | Đường nối `HIS_INFUSION` → hồ sơ |
| `MediReactSums` | `List<HIS_MEDI_REACT_SUM>` | Đường nối `HIS_MEDI_REACT` → hồ sơ |
| `TransfusionSums` | `List<HIS_TRANSFUSION_SUM>` | Đường nối `HIS_TRANSFUSION` → hồ sơ |

### Đường nối 3 bảng không có `TREATMENT_ID`

```
HIS_INFUSION.INFUSION_SUM_ID        → HIS_INFUSION_SUM.ID        → .TREATMENT_ID
HIS_MEDI_REACT.MEDI_REACT_SUM_ID    → HIS_MEDI_REACT_SUM.ID      → .TREATMENT_ID
HIS_TRANSFUSION.TRANSFUSION_SUM_ID  → HIS_TRANSFUSION_SUM.ID     → .TREATMENT_ID
```

Client tách dữ liệu phẳng thành từng hồ sơ bằng `SplitByTreatment()` (`__DoctorMode.cs`):
4 bảng đầu nhóm trực tiếp bằng `ToLookup(TREATMENT_ID)`; 3 bảng còn lại dựng `Dictionary<_SUM.ID, TREATMENT_ID>` rồi nhóm — toàn bộ tra cứu O(1).
Dòng chi tiết không tìm được bản ghi `_SUM` tương ứng sẽ bị bỏ qua (không xác định được thuộc hồ sơ nào).

### Phân trang — CHƯA LÀM (nợ kỹ thuật)

Hiện trạng thực tế của bản đã code:

| | |
|---|---|
| Backend đọc `CommonParam.Start` / `.Limit` | **Không** — trả toàn bộ dữ liệu thoả bộ lọc, không cắt trang |
| Backend gán `CommonParam.Count` | **Không** |
| Client | `LoadPagingByDoctor()` vẫn truyền `new CommonParam(startPage, pageSize)` và đọc `apiResult.Param.Count` |

Hệ quả: `dataTotal` luôn bằng 0 nên `ucPaging` không hiện đúng tổng số; đổi trang gọi lại API
nhưng nhận về cùng một tập dữ liệu. Màn hình vẫn dùng được vì client tự dựng lưới từ tập đầy đủ,
nhưng **khoảng thời gian tra soát rộng sẽ tải nặng** — đây là lý do có ngưỡng cảnh báo 20.000 dòng
trong log.

Khi làm phân trang thật, phải chốt **đơn vị đếm là HỒ SƠ, không phải y lệnh**: một hồ sơ sinh nhiều
dòng y lệnh nên số dòng trên lưới luôn lớn hơn số bản ghi mỗi trang. Cắt trang giữa chừng một hồ sơ
sẽ làm hồ sơ đó thiếu y lệnh.

Xem thêm Phần 11, mục 1.

### Client không cần server làm thêm

| Việc | Ai làm |
|---|---|
| Quy đổi loại văn bản (`DOCUMENT_TYPE_ID`) | Client — `ProcessDataADO()` |
| Sinh `SEARCH_CODE`, `TYPE`, `DEPARTMENT_NAME` | Client |
| Nhân đôi dòng Phiếu chỉ định → Phiếu kết quả | Client — server **không** nhân dòng |
| Gán `TREATMENT_CODE` / `PATIENT_CODE` / `PATIENT_NAME` | Client — `StampTreatmentInfo()` từ `Treatments` |
| Trạng thái văn bản `DOC_STATUS` | Client — gọi riêng `api/EmrDocument/GetView` theo `TREATMENT_CODEs` của cả trang (1 lần gọi) |

### API MOS KHÔNG trả `SIGNERS` / `UN_SIGNERS`

Bản thiết kế trước yêu cầu điều này. Không làm được: `V_EMR_DOCUMENT` nằm ở **service và DB khác**
(`BACKEND/EMR`), MOS không truy vấn trực tiếp được. Client tự gọi `api/EmrDocument/GetView`.

## 4.2 API dùng lại — không đổi

| URI | Consumer | Dùng cho |
|---|---|---|
| `api/HisTreatment/GetInfoForRecordChecking` | Mos | Cách 1 — giữ nguyên |
| `api/EmrDocument/GetView` | Emr | Danh sách văn bản |
| `api/EmrDocumentType/Get` | Emr | Danh sách loại văn bản (grid trái) |
| `api/EmrSign/GetView` | Emr | Thông tin luồng ký |
| `AcsRequestUriStore.ACS_TOKEN__AUTHORIZE` | Acs | Quyền |

### BẮT BUỘC lọc `api/EmrDocument/GetView` bằng `TREATMENT_CODEs`, KHÔNG dùng `TREATMENT_IDs`

Trên `V_EMR_DOCUMENT`, cột `TREATMENT_ID` là `Nullable<long>` còn `TREATMENT_CODE` thì không.
Hai bộ lọc của backend EMR xử lý khác nhau
(`EMR.GetManager/GetDocumentBO/EmrDocument/V/EmrDocumentViewFilterQuery.cs`):

```csharp
// dòng 120-122
if (this.TREATMENT_IDs != null)
    listExpression.Add(o => o.TREATMENT_ID.HasValue && this.TREATMENT_IDs.Contains(o.TREATMENT_ID.Value));

// dòng 439-441
if (this.TREATMENT_CODEs != null)
    listExpression.Add(o => o.TREATMENT_CODE != null && this.TREATMENT_CODEs.Contains(o.TREATMENT_CODE));
```

Điều kiện `o.TREATMENT_ID.HasValue` **loại bỏ mọi văn bản chưa được gán `TREATMENT_ID`** — đúng
nhóm văn bản mới sinh, chưa ký. Cách 1 lọc bằng `TREATMENT_CODE__EXACT` nên vẫn thấy chúng; Cách 2
từng lọc bằng `TREATMENT_IDs` nên **trạng thái "chưa ký" không bao giờ xuất hiện**. Đã sửa
`LoadDocumentsOfTreatments()` sang `TREATMENT_CODEs` để hai cách khớp nhau từng dòng.

Nhánh gộp văn bản (`EmrDocumentGetViewBehaviorMergeFilterQuery`) chỉ chạy khi `IS_MERGE` /
`IS_MERGE_NAME` = true — plugin không set nên đổi sang mã hồ sơ không làm mất văn bản gộp.

## 4.3 Tập trung URI vào `RecordCheckingUriStore.cs` (file mới)

```csharp
internal class RecordCheckingUriStore
{
    internal const string MOSHIS_HIS_TREATMENT_GET_INFO_FOR_RECORD_CHECKING
        = "api/HisTreatment/GetInfoForRecordChecking";
    internal const string MOSHIS_HIS_TREATMENT_GET_SERVICE_REQ_FOR_RECORD_CHECKING
        = "api/HisTreatment/GetServiceReqForRecordChecking";
    internal const string EMR_DOCUMENT_GETVIEW       = "api/EmrDocument/GetView";
    internal const string EMR_DOCUMENT_TYPE_GET      = "api/EmrDocumentType/Get";
    internal const string EMR_SIGN_GETVIEW           = "api/EmrSign/GetView";
    internal const string EMR_DOCUMENT_DOWNLOAD_FILE = "api/EmrDocument/DownloadFile";
}
```

> **Không** đặt tên `HisRequestUriStore`: tên đó đã có trong `HIS.Desktop.ApiConsumer` và plugin này đang dùng, một class cùng tên trong namespace plugin sẽ che mất nó.

---

# PHẦN 5. LUỒNG XỬ LÝ

## 5.1 Load form — bổ sung 4 bước

```
FormHisTreatmentRecordChecking_Load()
 1. GetControlAcs()                     ← giữ nguyên
 2. InitConfigAndPermission()           ← giữ nguyên
 3. InitComboRequestDoctor()            ← MỚI (đọc cache, không gọi API)
 4. InitComboTreatmentStatus()          ← MỚI (danh sách tĩnh 2 dòng)
 5. FillDataToGridTreatment(listId)     ← giữ nguyên
 6. InitGridEmrDocumentType()           ← giữ nguyên
 7. SetDefaultValueControl()            ← bổ sung reset control mới
 8. SetDefaultFilterValue()             ← MỚI (đầu tháng → hôm nay)
 9. SetCaptionByLanguageKey()           ← BỎ COMMENT + hoàn thiện
10. ProcessCaptionGridInfoRecord()      ← giữ nguyên
11. InitControlState()                  ← bổ sung 2 checkbox mới
12. ApplyModeUI()                       ← MỚI
13. FillDataToGrid()                    ← SỬA: rẽ nhánh
14. SetDefaultProperties()              ← giữ nguyên
```

## 5.2 Xác định cách tra soát

```csharp
/// <summary>Trả về true nếu đang tra soát theo bác sĩ chỉ định (Cách 2 — QT-01).</summary>
private bool IsFilterByDoctorMode()
{
    return cboRequestDoctor.EditValue != null
        && !string.IsNullOrWhiteSpace(cboRequestDoctor.EditValue.ToString());
}
```

`cboRequestDoctor.EditValueChanged` → gọi `ApplyModeUI()`.

## 5.3 `ApplyModeUI()` — bật/tắt theo cách tra soát

| Thành phần | Cách 1 | Cách 2 |
|---|---|---|
| `TxtTreatmentCode.Enabled` | `true` | `false` |
| `dtFromDate` / `dtToDate` / `cboTreatmentStatus` `.Enabled` | `false` | `true` |
| Caption 3 item trên | mặc định | **Maroon** |
| `btnDat`, `btnKhongDat`, `btnDuyet`, `btnHuyDuyet` `.Enabled` | theo `FillDataToControl()` | `false` |
| `Gv_IR_PatientCode/PatientName/TreatmentCode` `.Visible` | `false` | `true` |
| `ucPaging.Visible` | `false` | `true` |
| Panel thông tin BN | giữ nguyên | gọi `SetDefaultValueControl()` |

## 5.4 Validate trước khi tìm (QT-02, QT-03, QT-04)

```
Nếu IsFilterByDoctorMode():
   dtFromDate rỗng          → dxErrorProvider.SetError(dtFromDate,
                                MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc),
                                ErrorType.Warning)                     → CHẶN
   dtToDate rỗng            → tương tự tại dtToDate                    → CHẶN
   cboTreatmentStatus rỗng  → tương tự tại cboTreatmentStatus          → CHẶN
   dtFromDate > dtToDate    → ResourceMessage.TuNgayPhaiNhoHonDenNgay  → CHẶN
   (toDate - fromDate) > 31 ngày → XtraMessageBox YesNo, chọn No → dừng
Xóa toàn bộ error khi hợp lệ hoặc khi bấm Tìm lại.
```

## 5.5 `FillDataToGrid()` — rẽ nhánh

```csharp
private void FillDataToGrid()
{
    try
    {
        if (IsFilterByDoctorMode())
            FillDataToGridByDoctor();      // MỚI
        else
            FillDataToGridByTreatment();   // toàn bộ code hiện tại, KHÔNG sửa logic
    }
    catch (Exception ex)
    {
        WaitingManager.Hide();
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

> **Bắt buộc**: `FillDataToGridByTreatment()` là code hiện tại được đưa nguyên vào, không đổi logic. Đây là điều kiện để Cách 1 không bị ảnh hưởng (test nhóm A).

## 5.6 `FillDataToGridByDoctor()` — luồng mới

```
1. Validate (5.4) — sai thì return
2. WaitingManager.Show()
3. Dựng filter (4.1) + CommonParam(startPage, limit) từ ucPaging
4. LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => filter), filter))
5. GetRO<List<...>>(URI, MosConsumer, filter, param)
6. Map kết quả → List<InfoRecordADO>:
      - CREATE_TIME_REAL_STR = TimeNumberToTimeString(CREATE_TIME)
      - DOC_STATUS     = CalcDocumentStatus(docs)          (3.3)
      - DOC_STATUS_NAME = GetDocumentStatusName(DOC_STATUS)
      - tra khoa / loại y lệnh bằng Dictionary dựng SẴN ngoài vòng lặp
7. Lọc phía client:
      chkNoDocument      → DOC_STATUS == NoDocument              (QT-11)
      chkNotFullySigned  → DOC_STATUS != FullySigned             (QT-12)
      chkToiTao          → CREATOR == GetLoginName()
8. Gc_InfoRecord.BeginUpdate() → DataSource = list → EndUpdate()
9. Gc_EmrDocument.DataSource = null      (chờ user click dòng)
10. WaitingManager.Hide()
11. MessageManager.Show(this, param, apiResult != null)
12. SessionManager.ProcessTokenLost(param)
```

## 5.7 Click / nháy đúp dòng ở Cách 2

| Sự kiện | Xử lý |
|---|---|
| `Gv_InfoRecord_RowCellClick` | Gọi `ProcessDataGridDocument()` — lọc `ListDocument` **trong RAM**, **không gọi API**. Dùng chung cho cả hai cách vì `SEARCH_CODE` là duy nhất toàn hệ thống |
| `Gv_InfoRecord_CustomRowCellEdit` | Ẩn/hiện nút **[Tạo]** theo QT-14 (đổi `e.RepositoryItem`) — thay cho việc bật/tắt một nút chung |
| `Gv_InfoRecord_DoubleClick` | `OpenTreatmentOfFocusedOrder()`: `cboRequestDoctor.EditValue = null`; gán `treatmentId` + `TxtTreatmentCode.Text`; `ApplyModeUI()`; `FillDataToGrid()` |

## 5.8 Ràng buộc hiệu năng (bắt buộc)

| Yêu cầu | Cách làm |
|---|---|
| Không gọi `BackendDataWorker.Get<T>()` trong vòng lặp | `PrepareLookupData()` dựng sẵn `dicDepartmentCache` / `dicServiceReqTypeCache` **một lần cho cả lần tìm**, trước khi lặp `ProcessDataADO` |
| **Không gọi API trong vòng lặp** | `BuildMedicineLookup()` gom `MEDICINE_ID` của **cả trang** rồi gọi `api/HisMedicine/GetView` **một lượt** (chia lô 500). Trước đây `GetMedicineById` nằm trong `ProcessDataADO` → Cách 2 với 50 hồ sơ là 50 lượt gọi |
| Không tính toán trong `CustomUnboundColumnData` | `DOC_STATUS` tính sẵn (3.3); hàm unbound chỉ ánh xạ icon |
| Bind grid | `BeginUpdate()` / `EndUpdate()` |
| Cách 2 | Bắt buộc phân trang server-side qua `GetRO` + `CommonParam(start, limit)` |
| `RowCellStyle` | Chỉ so sánh đơn giản, không LINQ, không gọi cache |
| Giải phóng bộ nhớ | `ProcessDisposeModuleDataAfterClose()` (khai báo `public override`, **không** phải `protected`) xoá 3 bảng tra cứu + `ListDataInfoRecord` + `ListDocument` |

### Số lượt gọi API cho một lần bấm Tìm

| | Cách 1 | Cách 2 (50 hồ sơ/trang) |
|---|---|---|
| Y lệnh | 1 | 1 |
| Văn bản EMR | 1 | 1 |
| `V_HIS_MEDICINE` | 0–1 | 0–1 |
| **Tổng** | **2–3** | **2–3** |

### Bẫy dữ liệu đã xử lý

`HIS_MEDI_REACT.MEDICINE_ID` kiểu `long?`. Gọi thẳng `.Value` khi gom danh sách sẽ ném
`NullReferenceException`, và vì `ProcessDataADO` bọc try-catch ở ngoài cùng nên hậu quả là
**mất toàn bộ dòng của hồ sơ đó**, chỉ ghi log, người dùng không thấy thông báo.
Bắt buộc lọc `HasValue` trước khi lấy `.Value`.

---

# PHẦN 6. CHỨC NĂNG TẠO VĂN BẢN

## 6.1 Nguồn cấu hình biểu mẫu (QT-15)

Cấu hình nằm ở màn **Biểu in** — plugin `SAR.Desktop.Plugins.SarPrintType`.
Trường "Loại văn bản" trên màn đó lưu vào **`SAR_PRINT_TYPE.EMR_DOCUMENT_TYPE_CODE`**
(xem `frmSarPrintType.cs` dòng 315 nạp combo, dòng 1032 gán giá trị).

Lấy danh sách biểu mẫu — **không cần API**, dữ liệu đã có trong cache:

```csharp
var printTypes = BackendDataWorker.Get<SAR_PRINT_TYPE>()
    .Where(o => o.EMR_DOCUMENT_TYPE_CODE == CurrentType.DOCUMENT_TYPE_CODE
             && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
    .OrderBy(o => o.PRINT_TYPE_NAME)
    .ToList();
```

## 6.2 Luồng tạo văn bản

```
repositoryItemButtonCreateDoc_ButtonClick        ← nút trên TỪNG DÒNG
 │
 ├─ row = Gv_InfoRecord.GetFocusedRow()
 │    row == null hoặc row.DOC_STATUS != NoDocument → return    (QT-13, QT-14)
 │
 ├─ printTypes = lọc SAR_PRINT_TYPE theo 6.1
 │    Count == 0 → ResourceMessage.LoaiVanBanChuaCauHinhBieuIn → return   (QT-16)
 │    Count == 1 → RunPrint(printTypes[0].PRINT_TYPE_CODE)                (QT-17)
 │    Count >  1 → PopupMenu + BarButtonItem, ShowPopup(Cursor.Position)  (QT-18)
 │                 DevExpress 15.2 KHÔNG có DXPopupMenu.ShowPopup
 │
 └─ RunPrint(printTypeCode)
      │
      ├─ new RichEditorStore(ApiConsumers.SarConsumer,
      │        ConfigSystems.URI_API_SAR,
      │        LanguageManager.GetLanguage(),
      │        PrintStoreLocation.ROOT_PATH)
      │    .RunPrintTemplate(printTypeCode, DelegateRunPrinter)
      │
      └─ DelegateRunPrinter(printCode, fileName)
           switch (printCode)
             case <mã đã hỗ trợ> : loadMps = new MpsXXXBehavior(...); break;
             default             : ResourceMessage.BieuMauChuaDuocHoTro; return false;
           return loadMps.Load(printCode, fileName, null);
                │
                └─ MpsXXXBehavior.Load()
                     ├─ build PDO của mẫu
                     └─ new PrintCustomShow<T>(...).SignRun(treatmentCode, roomId, documentName)
                          ├─ EmrGenerateProcessor()
                          │     .GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, roomId)
                          └─ MPS.MpsPrinter.Run(new PrintData(...) { EmrInputADO = inputADO })
```

Sau khi cửa sổ ký/xem trước đóng → gọi `FillDataToGrid()` (QT-19).
Tiền lệ: `repositoryItemButtonView_ButtonClick` hiện đang gọi `BtnSearch_Click(null, null)` sau `ShowPopup()`.

## 6.3 Điểm mấu chốt — không cần API tạo văn bản

`Library.PrintBordereau` **không** gọi API nào để tạo văn bản EMR. Nó chỉ:

1. Đặt `PreviewType` phù hợp.
2. Gắn `EmrInputADO` (tạo bởi `EmrGenerateProcessor`).
3. Gọi `MPS.MpsPrinter.Run()`.

Phần tạo văn bản và ký do MPS + SignLibrary xử lý. Plugin tra soát làm y hệt.

## 6.4 `PreviewType` (QT — dùng thư viện chuẩn, không hardcode)

`PrintCustomShow` quyết định `PreviewType` từ `GlobalDataStore.CURRENT_PRINT_OPTION`:

| `PrintOption.Value` | `PreviewType` | Khi nào |
|---|---|---|
| `SHOW_DIALOG` (7) | `ShowDialog` | Mặc định |
| `PRINT_NOW` (1) | `PrintNow` | Config `CHE_DO_IN_PHAN_MEM = 2` |
| `EMR_SIGN_NOW` (5) | `EmrSignNow` | Tạo văn bản + ký, không in |
| `PRINT_NOW_AND_EMR_SIGN_NOW` (4) | `EmrSignAndPrintNow` | Tạo + ký + in |
| `EMR_SIGN_AND_PRINT_PREVIEW` (6) | `EmrSignAndPrintPreview` | Tạo + ký + xem trước |

## 6.5 Chiến lược tái sử dụng

### Bước 1 — ưu tiên gọi thẳng Print Library có sẵn

| Loại văn bản | Thư viện dùng lại |
|---|---|
| Phiếu chỉ định (`ID__SERVICE_ASSIGN`) | `Library.PrintServiceReq.PrintServiceReqProcessor` |
| Đơn thuốc (`ID__PRESCRIPTION`) | `Library.PrintPrescription.PrintPrescriptionProcessor` |
| Phiếu kết quả (`ID__SERVICE_RESULT`) | `Library.PrintTestTotal` — cần khảo sát mức phù hợp |

### Bước 2 — loại chưa có Library: copy khung từ `Library.PrintBordereau`

```
HIS.Desktop.Plugins.HisTreatmentRecordChecking/
├── Base/
│   ├── ILoad.cs                  ← copy nguyên
│   ├── MpsDataBase.cs            ← copy, bỏ property không dùng
│   ├── PrintOption.cs            ← copy nguyên
│   ├── GlobalDataStore.cs        ← copy nguyên
│   └── PrintTypeCodeWorker.cs    ← MỚI: hằng số mã biểu mẫu được hỗ trợ
├── PrintCustomShow.cs            ← copy nguyên
└── MpsBehavior/
    └── Mps000XXX/Mps000XXXBehavior.cs
```

### Mẫu tham chiếu

| Cần xem | File |
|---|---|
| `switch (printCode)` | `PrintBordereauProcessor.cs` dòng 285–683 |
| Tạo `DXPopupMenu` | `InitMenuProcessor.cs` dòng 407–423 |
| Gọi `RichEditorStore` | `PrintBordereauProcessor.cs` dòng 272–283 |
| Build PDO + `SignRun` | `MpsBehavior/Mps000120/Mps000120Behavior.cs` dòng 59–139 |
| `PreviewType` + `EmrInputADO` | `PrintCustomShow.cs` dòng 39–103 |

## 6.6 Giới hạn cần lưu ý

`switch (printCode)` là **hardcode**. Mỗi mẫu muốn hỗ trợ cần đủ 3 thứ:

1. Một class `MpsXXXBehavior : MpsDataBase, ILoad`
2. Một PDO tương ứng — constructor và dữ liệu đầu vào khác nhau hoàn toàn giữa các mẫu
3. Một reference DLL trong `.csproj`: `..\..\..\..\LIB\MPSv2\MPS.PDO\MPS.Processor.Mps000XXX.PDO.dll`

Cấu hình SAR có thể gán bất kỳ biểu mẫu nào cho loại văn bản → **bắt buộc** có nhánh `default` báo "chưa được hỗ trợ" (QT-16), không để lỗi hệ thống.

## 6.7 HAI TẦNG GIỚI HẠN — đã gặp trên dữ liệu thật (11/08/2026)

Bản hiện tại chặn ở **tầng 1** nhưng **bỏ sót tầng 2**. Đây là lỗi thiết kế của bản đầu, ghi lại đầy đủ để lần sửa sau không lặp.

### Tầng 1 — theo LOẠI VĂN BẢN (đã chặn đúng)

`CreateDocumentByPrintType()` chỉ cho qua `ID__SERVICE_ASSIGN` và `ID__SERVICE_RESULT`.
7 loại còn lại báo `ResourceMessage.BieuMauChuaDuocHoTro`.

### Tầng 2 — theo MÃ BIỂU MẪU (CHƯA chặn → im lặng)

Màn **Biểu in** cho gắn **bất kỳ** mã Mps nào vào một loại văn bản, nhưng `Library.PrintServiceReq`
chỉ dựng được dữ liệu cho **31 mã cố định**. Mã ngoài danh sách rơi vào `default: break;` →
**không làm gì, không ném exception, không ghi log**. Triệu chứng người dùng thấy:
*"chọn biểu mẫu in xong không mở được bước tiếp theo"*.

| Thư viện | Số mã hỗ trợ | Danh sách |
|---|---|---|
| `PrintServiceReq` | 31 | `Mps000001, 000026, 000027, 000028, 000029, 000030, 000031, 000036, 000037, 000038, 000040, 000042, 000053, 000071, 000167, 000340, 000363, 000364, 000365, 000366, 000367, 000368, 000423, 000424, 000425, 000426, 000432, 000465, 000466, 000467, 000502` |
| `PrintPrescription` | 12 | `Mps000044, 000050, 000118, 000181, 000191, 000192, 000234, 000237, 000238, 000296, 000338, 000353` |

Nguồn đối chiếu:
`Library.PrintServiceReq/PrintTypeCodeStore.cs` · `Library.PrintPrescription/PrintMps*.cs`.
**Cả hai lớp đều `internal`** → plugin không tham chiếu trực tiếp được, muốn lọc phải chép danh sách sang plugin và tự bảo trì.

### Ca thực tế

Bệnh viện gắn **`Mps000033`** (Phiếu yêu cầu phẫu thuật / thủ thuật) cho *Phiếu chỉ định*.
Mã này không nằm trong 31 mã → chọn xong đứng im. Mẫu PTTT mà thư viện hỗ trợ là **`Mps000036`**.

### Ba hướng xử lý — CHƯA CHỐT

| Hướng | Nội dung | Đánh giá |
|---|---|---|
| **A. Sửa cấu hình** | Đổi `Mps000033` → `Mps000036` ở màn Biểu in | Không cần code, dùng được ngay. **Đề xuất** |
| **B. Lọc phía plugin** | Chép danh sách mã sang plugin, chỉ chào biểu mẫu dựng được; hết mẫu thì báo rõ | Chặn được im lặng, nhưng phải bảo trì danh sách khi thư viện đổi |
| **C. Bổ sung vào thư viện** | Thêm `Mps000033` vào `Library.PrintServiceReq` | Đụng thư viện dùng chung nhiều plugin — cần chủ sở hữu thư viện duyệt |

Bổ sung **Đơn thuốc** qua `Library.PrintPrescription` là khả thi, độc lập với ba hướng trên:
`OutPatientPresResultSDO` chỉ cần `ExpMests` + `ServiceReqs`, lấy `HIS_EXP_MEST` theo `SERVICE_REQ_ID`
(mẫu tham chiếu: `UCAllocateExecuteRoom_Print.cs` dòng 84–105).
Riêng **đơn máu** (`SERVICE_REQ_TYPE.ID__DONM`) đi luồng khác, không dùng được thư viện này.

> **Trạng thái 11/08/2026**: đã phân tích, **chưa sửa mã nguồn** theo yêu cầu. Chờ chốt hướng.

---

# PHẦN 7. BỔ SUNG THEO QUY CHUẨN

## 7.1 Đa ngôn ngữ — plugin đang thiếu hoàn toàn

Hiện trạng: **không có** thư mục `Resources/`; `SetCaptionByLanguageKey()` bị comment ở dòng 184.

Cần tạo:

```
Resources/
├── Lang.vi.resx                ← toàn bộ nhãn giao diện
├── Lang.en.resx                ← số dòng PHẢI bằng Lang.vi.resx
├── Message.Lang.vi.resx        ← thông báo riêng plugin
├── Message.Lang.en.resx
├── ResourceLanguageManager.cs
└── ResourceMessage.cs
```

Chuỗi đang hardcode cần chuyển sang resource:

| Chuỗi | Vị trí |
|---|---|
| `"Chưa chốt"`, `"Đã duyệt"`, `"Đạt"`, `"Chưa đạt"` | `FillDataToControl()` |
| `"Chưa xác định"` | `EmrDocument()` |
| `"Khác"` | `ProcessDataGridInfoRecord()` |
| `"Không xác định được văn bản ký"` | `repositoryItemButtonView_ButtonClick()` |
| `"Mã"`, `"Loại"`, `"Thời gian"`, `"Khoa tạo"`, `"Người tạo"`, `"Mã y lệnh"`… | `ProcessCaptionGridInfoRecord()` |

Thông báo mới cần thêm vào `Message.Lang.*.resx`:

| Key | Nội dung tiếng Việt |
|---|---|
| `TuNgayPhaiNhoHonDenNgay` | Từ ngày phải nhỏ hơn hoặc bằng Đến ngày |
| `KhoangThoiGianVuotQua31Ngay` | Khoảng thời gian tra soát vượt quá 31 ngày, có thể mất nhiều thời gian. Bạn có muốn tiếp tục? |
| `LoaiVanBanChuaCauHinhBieuIn` | Loại văn bản này chưa được cấu hình biểu mẫu in. |
| `BieuMauChuaDuocHoTro` | Biểu mẫu này chưa được hỗ trợ tạo văn bản từ màn tra soát. |

## 7.2 ControlState

Hai checkbox mới `chkNoDocument`, `chkNotFullySigned` phải nhớ trạng thái giữa các phiên:

- Đọc trong `InitControlState()`, ghi trong `CheckedChanged`.
- **Bắt buộc** `if (IsLoadFirstForm) return;` ở đầu handler.
- `KEY = control.Name`, `MODULE_LINK = "HIS.Desktop.Plugins.HisTreatmentRecordChecking"`.

## 7.3 Ghi log

| Tình huống | Mức |
|---|---|
| Gọi API thất bại | `LogSystem.Error(ex)` |
| Sự kiện giao diện (`CheckedChanged`, `Click`, `RowCellClick`) | `LogSystem.Warn(ex)` |
| Trước khi gọi API mới | `LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => filter), filter))` |
| Tạo văn bản thành công | `LogUtil.LogActionSuccess("FormHisTreatmentRecordChecking", "CreateDocument", loginName)` |

---

# PHẦN 8. LỖI HIỆN CÓ CẦN SỬA KÈM

Phát hiện khi rà soát code hiện tại. Nên sửa cùng đợt này vì đụng đúng vùng code.

| # | Mức | Vị trí | Mô tả | Cách sửa |
|---|---|---|---|---|
| 1 | Cao | `FillDataToGridTreatment()` dòng 247 + `Load()` dòng 194 | Khi mở với đúng **1** hồ sơ, `FillDataToGrid()` chạy **2 lần** → toàn bộ 3–4 API gọi lặp | Bỏ lời gọi ở dòng 247, để `Load()` gọi một lần |
| 2 | Cao | `ProcessCaptionGridInfoRecord()` dòng 996 | `if (CurrentType == null) return;` — lúc Load `CurrentType` còn null nên hàm không làm gì; sau đó `FillDataToGrid` gán `CurrentType` nhưng không gọi lại → caption grid lần đầu sai | Gọi lại sau khi gán `CurrentType` trong `FillDataToGrid` |
| 3 | Cao | `chkUuTien_CheckedChanged()` dòng 1824 | Thiếu `if (IsLoadFirstForm) return;` → chạy sớm lúc Load, ghi thừa xuống SQLite | Thêm dòng kiểm tra ở đầu handler |
| 4 | Cao | `ProcessDataADO()` dòng 346, 366, 433, 440, 470 | Gọi `BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault()` **trong vòng lặp** → O(n×m) | Dựng `Dictionary` một lần trước vòng lặp |
| 5 | Trung bình | `GetSigners()` dòng 1409 | `dicVEmrSign[long.Parse(item)]` không dùng `TryGetValue` → ném lỗi bị nuốt vào Warn | Dùng `TryGetValue` |
| 6 | Trung bình | `GetDicEmrSign()` dòng 946 | Không xóa dictionary khi API trả rỗng → còn dữ liệu của lần tra soát trước | Gán `dicVEmrSign = new Dictionary<...>()` trước khi nạp |
| 7 | Trung bình | Dòng 102–103 | `controlStateWorker` và `currentControlStateRDO` khai báo `public static` → dùng chung giữa các thể hiện form | Chuyển sang `private` không static |
| 8 | Trung bình | Dòng 124 | `APPROVAL_STORE_STT_ID__DAT = 3` hardcode (đã có TODO trong code) | **KHÔNG chờ backend** — hằng số đã có sẵn, xem ghi chú dưới |

### Ghi chú lỗi #8 — hằng số đã tồn tại

Bản thiết kế trước ghi "chờ backend bổ sung". Không cần: `IMSys.DbConfig.HIS_RS.dll` đã có sẵn

```
IMSys.DbConfig.HIS_RS.HIS_TREATMENT.APPROVAL_STORE_STT_ID__CHOT       = 1
IMSys.DbConfig.HIS_RS.HIS_TREATMENT.APPROVAL_STORE_STT_ID__TU_CHOI    = 2
IMSys.DbConfig.HIS_RS.HIS_TREATMENT.APPROVAL_STORE_STT_ID__DANG_XU_LY = 3
```

Chỉ cần thay số `3` bằng `APPROVAL_STORE_STT_ID__DANG_XU_LY`.

> **Cần nghiệp vụ xác nhận**: client đặt tên giá trị 3 là **`__DAT` ("Đạt")**, backend đặt tên là
> **"ĐANG XỬ LÝ"**. Một trong hai bên đang hiểu sai trạng thái này. Liên quan trực tiếp tới 4 nhãn
> ở 7.1 ("Chưa chốt / Đã duyệt / Đạt / Chưa đạt") — chỉ có 3 giá trị backend cho 4 nhãn client, nên
> bảng quy đổi trạng thái trong `FillDataToControl()` cần rà lại.

---

# PHẦN 9. DANH SÁCH FILE

## 9.1 File tạo mới

| File | Nội dung |
|---|---|
| `EnumRecordDocumentStatus.cs` | Enum trạng thái văn bản (3.2) |
| `RecordCheckingUriStore.cs` | Hằng số URI (4.3) — **không** đặt tên `HisRequestUriStore`, xem ghi chú ở 4.3 |
| `ModuleLinkString.cs` | Hằng số ModuleLink (đang hardcode chuỗi ở nhiều nơi) |
| `Base/ILoad.cs` | Copy từ PrintBordereau |
| `Base/MpsDataBase.cs` | Copy từ PrintBordereau |
| `Base/PrintOption.cs` | Copy từ PrintBordereau |
| `Base/GlobalDataStore.cs` | Copy từ PrintBordereau |
| `Base/PrintTypeCodeWorker.cs` | Hằng số mã biểu mẫu hỗ trợ |
| `PrintCustomShow.cs` | Copy từ PrintBordereau |
| `MpsBehavior/Mps000XXX/Mps000XXXBehavior.cs` | Mỗi mẫu một file |
| `Resources/` (6 file) | Đa ngôn ngữ (7.1) |

## 9.2 File sửa

| File | Nội dung sửa |
|---|---|
| `RecordChecking/FormHisTreatmentRecordChecking.Designer.cs` | 8 control mới + 4 cột grid |
| `RecordChecking/FormHisTreatmentRecordChecking.cs` | Rẽ nhánh cách tra soát, validate, tính trạng thái, tạo văn bản, 8 lỗi ở Phần 8 |
| `ADO/InfoRecordADO.cs` | 8 property mới (3.1) |
| `HIS.Desktop.Plugins.HisTreatmentRecordChecking.csproj` | Reference PDO + Print Library + Compile include file mới |

## 9.3 File backend (repo `BACKEND/MOS`)

| File | Nội dung |
|---|---|
| `MOS.Filter/HisServiceReqForRecordCheckingFilter.cs` | Mới — bộ lọc (4.1) |
| `MOS.SDO/HisServiceReqForRecordCheckingSDO.cs` | Mới — 11 danh sách entity thô (4.1) |
| `MOS.MANAGER/HisTreatment/Get/HisTreatmentGetServiceReqForRecordChecking.cs` | Mới — 11 truy vấn + ghi log SQL |
| `MOS.MANAGER/HisTreatment/HisTreatmentManagerSql.cs` | Sửa — thêm method `GetServiceReqForRecordChecking` |
| `MOS.API/Controllers/HisTreatmentControllerSql.cs` | Sửa — thêm action cùng tên |
| `MOS.Filter.csproj`, `MOS.SDO.csproj`, `MOS.MANAGER.csproj` | Sửa — thêm `<Compile Include>` cho 3 file mới |

> Deploy phải đẩy **cùng lúc** `MOS.SDO.dll` và `MOS.MANAGER.dll`. Lệch hai file này gây
> `MissingMethodException` lúc chạy, không phải lúc biên dịch.

## 9.4 Tách partial class (bắt buộc)

Form đang 2081 dòng, vượt xa ngưỡng 500 của `folder_structure.md`. Sau khi thêm code sẽ vượt 2500 dòng.

```
FormHisTreatmentRecordChecking.cs             ← khai báo, constructor, Load
FormHisTreatmentRecordChecking__Filter.cs     ← bộ lọc mới, ApplyModeUI, validate
FormHisTreatmentRecordChecking__Process.cs    ← FillDataToGrid 2 nhánh, map ADO, tính trạng thái
FormHisTreatmentRecordChecking__Print.cs      ← tạo văn bản, DelegateRunPrinter
FormHisTreatmentRecordChecking__Grid.cs       ← sự kiện grid, unbound column, row style
```

---

# PHẦN 10. THỨ TỰ TRIỂN KHAI

| Bước | Nội dung | Phụ thuộc | Trạng thái |
|---|---|---|---|
| 1 | Tách partial class + sửa 8 lỗi ở Phần 8 | Không | **Xong** — 10 file partial |
| 2 | Bổ sung `Resources/` + hoàn thiện `SetCaptionByLanguageKey()` | Không | **Xong** — 95 nhãn + 8 thông báo × vi/en |
| 3 | `EnumRecordDocumentStatus` + `CalcDocumentStatus` + 2 cột mới (QT-09, QT-10) | Bước 1 | **Xong** |
| 4 | 3 bộ lọc + `ApplyModeUI` + validate | Bước 1 | **Xong** — đặt ở hàng trên cùng cạnh nút Tìm |
| 5 | 2 checkbox lọc nhanh + ControlState (QT-11, QT-12) | Bước 3 | **Xong** |
| 6 | Nối API mới cho Cách 2 | Không | **Xong cả hai phía** — endpoint đã code, xem 4.1. Riêng **phân trang chưa làm**, xem Phần 11 mục 1 |
| 7 | Nút Tạo văn bản + khung in | Không | **Xong** — nút trên từng dòng, biểu mẫu lấy từ `SAR_PRINT_TYPE` |
| 8 | Tối ưu chuẩn bị dữ liệu + sửa lỗi `MEDICINE_ID` null | Bước 6 | **Xong** — xem 5.8 |

---

# PHẦN 11. ĐIỂM CHẶN CÒN LẠI

| # | Nội dung | Ảnh hưởng |
|---|---|---|
| 1 | **Phân trang chưa làm.** Backend không đọc `Start`/`Limit`, không gán `Param.Count`; client vẫn truyền và vẫn đọc | `ucPaging` hiện tổng = 0, đổi trang trả về cùng tập dữ liệu. Khoảng thời gian rộng sẽ tải nặng. Khi làm phải đếm theo **hồ sơ**, không phải y lệnh — xem 4.1 |
| 2 | Chốt cách xác định hồ sơ đã kết thúc: `OUT_TIME` hay cờ `IS_PAUSE` | Đang dùng `OUT_TIME`. Hai cờ **khác nghĩa** (`IS_PAUSE` = tạm dừng). Đổi thì sửa `BuildTreatmentCondition()`. Lưu ý `END_TIME` **không tồn tại** trên `HIS_TREATMENT` |
| 3 | Chốt nghĩa của `APPROVAL_STORE_STT_ID = 3`: "Đạt" (client) hay "Đang xử lý" (backend) | Xem ghi chú lỗi #8, Phần 8. Ảnh hưởng bảng quy đổi trạng thái ở `FillDataToControl()` |
| 4 | **Chốt hướng xử lý mã biểu mẫu ngoài danh sách thư viện — xem 6.7** | Hiện đang **im lặng**: chọn biểu mẫu xong không có gì xảy ra. Ba hướng A/B/C ở mục 6.7 |
| 5 | Chốt có bổ sung **Đơn thuốc** vào màn tra soát không | Hiện Đơn thuốc báo *chưa được hỗ trợ*; nối `PrintPrescription` là khả thi |

### Đã gỡ khỏi danh sách chặn

| Nội dung cũ | Kết quả |
|---|---|
| Backend mở endpoint `GetServiceReqForRecordChecking` | **Đã code** — xem 4.1 |
| Backend trả đủ 3 bảng `_SUM` | **Đã trả** — `InfusionSums` / `MediReactSums` / `TransfusionSums` |
| Chờ backend bổ sung hằng số `APPROVAL_STORE_STT_ID__DAT` | **Đã có sẵn** trong `IMSys.DbConfig.HIS_RS.dll`, chỉ còn việc chốt ngữ nghĩa (mục 3 ở trên) |
| API MOS trả kèm `SIGNERS` / `UN_SIGNERS` | **Bỏ yêu cầu** — khác service/DB, client tự gọi `api/EmrDocument/GetView` |

## Khác biệt đã biết giữa hai cách

Cột khoa của **Truyền máu** để trống ở cả hai cách (code cũ bị comment). Cách 2 có
`HIS_TRANSFUSION_SUM.DEPARTMENT_ID` nên điền được, nhưng Cách 1 không có bảng `_SUM` →
điền vào sẽ làm hai cách hiển thị khác nhau. Giữ nguyên cho tới khi có quyết định.

---

# PHẦN 12. GHI CHÚ

Tài liệu module đã tạo theo `module_docs.md`:
`docs/HIS.Desktop.Plugins.HisTreatmentRecordChecking.md` — đủ 9 mục, kèm Changelog.
