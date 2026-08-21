# Việc 53180 — Thiết Kế: Lọc Theo Bác Sĩ Chỉ Định + Tạo Văn Bản Tại Màn Tra Soát Hồ Sơ Bệnh Án

| Thông tin | Giá trị |
|---|---|
| Mã việc | 53180 |
| Plugin | `HIS.Desktop.Plugins.HisTreatmentRecordChecking` — "Tra soát hồ sơ bệnh án" |
| Form | `RecordChecking/FormHisTreatmentRecordChecking.cs` |
| Tài liệu gốc | `PhanTich_Loc_BacSiChiDinh_TraSoat_HoSoBenhAn.docx` |
| Đối tượng đọc | Lập trình viên + Kiểm thử viên |
| Trạng thái | **Đã code xong** — tài liệu này giữ nguyên phần thiết kế ban đầu, các chỗ khác với bản đã code được đánh dấu bằng khối trích dẫn "SAI SO VỚI BẢN ĐÃ CODE" |

> **Tài liệu tham chiếu chính là `53180_B_KyThuat_TraSoatHoSoBenhAn.md`.**
> File này là bản thiết kế viết TRƯỚC khi code. Ba chỗ trong đây mô tả sai thực tế — đã ghi chú tại
> chỗ: cột `HIS_TREATMENT.END_TIME` (mục 2 và 9), filter `DOCUMENT_TYPE_ID` (mục 7 và 9), và yêu cầu
> API MOS trả `SIGNERS`/`UN_SIGNERS` (mục 9). Phân trang cũng chưa làm (mục 7).

> **Đọc nhanh cho tester**: nhảy tới [Mục 3 — Hai chế độ](#3-hai-chế-độ-hoạt-động), [Mục 8 — Quy tắc trạng thái](#8-quy-tắc-trạng-thái-y-lệnh), [Mục 12 — Test case](#12-test-case).
> **Đọc nhanh cho dev**: [Mục 4 — Giao diện](#4-thiết-kế-giao-diện), [Mục 6 — Dữ liệu](#6-thiết-kế-dữ-liệu), [Mục 7 — Xử lý](#7-thiết-kế-xử-lý), [Mục 11 — Danh sách file](#11-danh-sách-file-thay-đổi).

---

## 1. Mục tiêu

Bác sĩ hiện phải mở **từng hồ sơ** mới biết y lệnh nào chưa tạo văn bản / chưa ký xong. Việc này bổ sung:

1. **Bộ lọc theo bác sĩ chỉ định** — xem toàn bộ y lệnh mình đã kê trên nhiều hồ sơ cùng lúc.
2. **Hiển thị trạng thái** từng y lệnh: đã/chưa tạo văn bản, đã ký hết/chưa ký hết.
3. **Nút "Tạo văn bản"** — tạo văn bản EMR ngay trên màn tra soát cho y lệnh chưa có văn bản.

**KHÔNG làm** (đã chốt): tạo văn bản hàng loạt, ký hàng loạt.

---

## 2. Quyết định đã chốt

| # | Nội dung | Quyết định |
|---|---|---|
| 1 | Trường thời gian lọc | Y lệnh (`HIS_SERVICE_REQ`) → `INTRUCTION_TIME`. 6 nguồn còn lại → `CREATE_TIME` |
| 2 | Trạng thái hồ sơ | Theo **thời gian ra viện** `HIS_TREATMENT.OUT_TIME` |
| 3 | Bác sĩ chỉ định | **Không bắt buộc**. Không chọn → giữ nguyên cách làm hiện tại (theo mã hồ sơ). Có chọn → lọc theo bác sĩ, khi đó **thời gian + trạng thái hồ sơ trở thành bắt buộc** |
| 4 | Tạo văn bản | Chỉ tạo cho **1 y lệnh đang chọn**. Không hàng loạt |
| 5 | Y lệnh đã có văn bản | **Không** cho tạo thêm — nút disable |
| 6 | Cơ chế in/tạo VB | Dùng thư viện chuẩn giống các chức năng khác (`EmrGenerateProcessor` + `MpsPrinter`), tái dùng/copy code từ `Library.PrintBordereau` |
| 7 | Cột thời gian | **Thêm cột "Thời gian tạo"** bên cạnh cột thời gian nghiệp vụ hiện có |

> **SAI SO VỚI BẢN ĐÃ CODE — mục 2 ở trên đã sửa.**
> Bản thiết kế gốc ghi `HIS_TREATMENT.END_TIME`. **Cột đó không tồn tại**; bảng chỉ có `IN_TIME`,
> `OUT_TIME`, `IS_PAUSE`, `END_LOGINNAME`, `END_ROOM_ID`, `END_CODE`, `TREATMENT_END_TYPE_ID`.
> Chạy với `END_TIME` sẽ `ORA-00904`. Bản đã code dùng `OUT_TIME`.
> `IS_PAUSE` nghĩa là *tạm dừng*, **không tương đương** — đừng thay lẫn nhau.

---

## 3. Hai chế độ hoạt động

Đây là điểm quan trọng nhất — form có **2 chế độ**, phân biệt bằng ô "Bác sĩ chỉ định".

### Chế độ A — Theo hồ sơ (mặc định, GIỮ NGUYÊN như hiện tại)

```
Điều kiện: ô "Bác sĩ chỉ định" ĐỂ TRỐNG
Hành vi:   Giống hệt hiện tại. Nhập/quét mã điều trị → Tìm → xem y lệnh + văn bản của 1 hồ sơ.
           Các nút Đạt / Không đạt / Duyệt / Hủy duyệt hoạt động bình thường.
Bổ sung:   Cột "Thời gian tạo" mới ở grid giữa. Nút "Tạo văn bản" mới.
```

> **Yêu cầu regression bắt buộc**: mọi hành vi cũ của chế độ A phải y nguyên. Nếu người dùng không đụng vào 3 ô lọc mới thì màn hình phải chạy giống hệt bản cũ.

### Chế độ B — Theo bác sĩ chỉ định (MỚI)

```
Điều kiện: ô "Bác sĩ chỉ định" CÓ giá trị
Bắt buộc:  Từ ngày + Đến ngày + Trạng thái hồ sơ  (thiếu → chặn, báo lỗi tại control)
Hành vi:   Bỏ qua ô mã điều trị. Lấy TẤT CẢ y lệnh do bác sĩ đó tạo/chỉ định,
           trên TẤT CẢ hồ sơ thỏa điều kiện thời gian + trạng thái.
           Grid giữa hiện thêm 3 cột: Mã BN, Tên BN, Mã điều trị.
           Panel thông tin bệnh nhân phía trên: để trống (vì nhiều hồ sơ).
           4 nút Đạt / Không đạt / Duyệt / Hủy duyệt: DISABLE.
           Double-click 1 dòng → nạp mã điều trị của dòng đó vào ô tìm kiếm
           và chuyển về chế độ A cho hồ sơ đó.
```

### Bảng so sánh

| Yếu tố | Chế độ A | Chế độ B |
|---|---|---|
| Ô "Bác sĩ chỉ định" | Trống | Có giá trị |
| Ô mã điều trị | Dùng | Bỏ qua (disable) |
| Từ ngày / Đến ngày | Không bắt buộc (bỏ qua) | **Bắt buộc** |
| Trạng thái hồ sơ | Không bắt buộc (bỏ qua) | **Bắt buộc** |
| Panel thông tin BN | Hiển thị đầy đủ | Để trống |
| Cột Mã BN / Tên BN / Mã điều trị | Ẩn | Hiện |
| 4 nút Đạt/Không đạt/Duyệt/Hủy duyệt | Enable theo trạng thái | Disable |
| Nút "Tạo văn bản" | Enable khi y lệnh chưa có VB | Enable khi y lệnh chưa có VB |
| Phân trang | Không | **Có** (bắt buộc) |

---

## 4. Thiết kế giao diện

### 4.1 Hàng bộ lọc (bổ sung vào hàng đang có `chkUuTien` / `chkToiTao` / `chkIncludeCancelDoc`)

```
┌──────────────────────────────────────────────────────────────────────────────────────────┐
│ [000001636535        ] [ Tìm (Ctrl F) ]                                                  │
├──────────────────────────────────────────────────────────────────────────────────────────┤
│ ▸ Thông tin bệnh nhân   (giữ nguyên)                                                      │
├──────────────────────────────────────────────────────────────────────────────────────────┤
│ ☑ Ưu tiên có dữ liệu  ☐ Tôi tạo  ☐ Bao gồm VB hủy                                        │
│ Bác sĩ chỉ định: [▼ chọn BS      ]  Thời gian: [dd/MM/yyyy]-[dd/MM/yyyy]                 │
│ Trạng thái HS:   [▼ Chưa kết thúc]  ☐ Chưa tạo VB   ☐ Chưa ký hết      [Tạo văn bản]     │
├────────────────┬─────────────────────────────────────┬───────────────────────────────────┤
│ Loại văn bản   │ Grid y lệnh (Gv_InfoRecord)         │ Grid văn bản (Gv_EmrDocument)     │
│ (giữ nguyên)   │                                     │ (giữ nguyên)                      │
└────────────────┴─────────────────────────────────────┴───────────────────────────────────┘
```

### 4.2 Danh sách control mới

| Tên control | Kiểu | Caption | Ghi chú |
|---|---|---|---|
| `cboRequestDoctor` | `GridLookUpEdit` | Bác sĩ chỉ định | Nguồn: `BackendDataWorker.Get<HIS_EMPLOYEE>()` lọc `IS_ACTIVE = 1` và `LOGINNAME` khác rỗng. Display = `TEN`, Value = `LOGINNAME`. Cho phép xóa trắng (`ShowClear = true`) |
| `dtFromDate` | `DateEdit` | Từ ngày | Mặc định: đầu tháng hiện tại |
| `dtToDate` | `DateEdit` | Đến ngày | Mặc định: hôm nay |
| `cboTreatmentStatus` | `LookUpEdit` | Trạng thái hồ sơ | 2 giá trị: `Chưa kết thúc` / `Đã kết thúc`. Mặc định `Chưa kết thúc` |
| `chkNoDocument` | `CheckEdit` | Chưa tạo văn bản | Lọc nhanh |
| `chkNotFullySigned` | `CheckEdit` | Chưa ký hết | Lọc nhanh |
| `btnCreateDocument` | `SimpleButton` | Tạo văn bản | Đặt cạnh nhóm nút dưới, hoặc trên hàng lọc |
| `ucPaging` | `Inventec.UC.Paging` | — | Chỉ hiện ở chế độ B |

**Quy tắc layout** (theo `ui_rules.md`):
- `LayoutControlItem` của `cboRequestDoctor`, `dtFromDate`, `dtToDate`, `cboTreatmentStatus`: `AppearanceItemCaption.TextOptions.HAlignment = Far`, `TextAlignMode = CustomSize`.
- Khi ở **chế độ B**: caption của Từ ngày / Đến ngày / Trạng thái HS đổi màu **Maroon** (trường bắt buộc). Chế độ A: màu mặc định.
- Vùng trống chèn `EmptySpaceItem`.
- Chuẩn thiết kế 1366×768.

### 4.3 Cột mới ở grid giữa `Gv_InfoRecord`

| Thứ tự | FieldName | Caption | Rộng | Hiện ở chế độ |
|---|---|---|---|---|
| (giữ) | `STT` | (icon trạng thái) | 24 | A + B |
| **mới** | `PATIENT_CODE` | Mã BN | 90 | **B** |
| **mới** | `PATIENT_NAME` | Tên BN | 150 | **B** |
| **mới** | `TREATMENT_CODE` | Mã điều trị | 110 | **B** |
| (giữ) | `CODE` | Mã y lệnh | 110 | A + B |
| (giữ) | `TYPE` | Loại | 110 | A + B |
| (giữ) | `CREATE_TIME_STR` | Thời gian chỉ định | 130 | A + B |
| **mới** | `CREATE_TIME_REAL_STR` | **Thời gian tạo** | 130 | A + B |
| (giữ) | `DEPARTMENT_NAME` | Khoa chỉ định | 150 | A + B |
| (giữ) | `CREATOR` | Người chỉ định | 110 | A + B |
| **mới** | `DOC_STATUS_NAME` | Trạng thái VB | 110 | A + B |

> Caption 5 cột giữa vẫn đổi động theo loại văn bản (hàm `ProcessCaptionGridInfoRecord`). Cột "Thời gian tạo" **không đổi caption**.

Ẩn/hiện cột bằng `gridColumn.Visible = true/false` khi chuyển chế độ, không tạo 2 grid.

---

## 5. Ý nghĩa cột "Thời gian chỉ định" vs "Thời gian tạo"

Lý do phải có 2 cột: cột "Thời gian chỉ định" hiện tại lấy **thời gian nghiệp vụ** khác nhau tùy nguồn, trong khi bộ lọc chạy trên `CREATE_TIME`. Nếu chỉ có 1 cột, người dùng lọc ngày 01/08 nhưng thấy dòng ghi 31/07 sẽ tưởng phần mềm sai.

| Loại văn bản | Cột "Thời gian chỉ định" lấy từ | Cột "Thời gian tạo" lấy từ | Lọc theo |
|---|---|---|---|
| Phiếu chỉ định / Phiếu kết quả / Đơn thuốc | `HIS_SERVICE_REQ.INTRUCTION_TIME` | `CREATE_TIME` | `INTRUCTION_TIME` |
| Phiếu chăm sóc | `HIS_CARE.CREATE_TIME` | `CREATE_TIME` | `CREATE_TIME` |
| Biên bản hội chẩn | `HIS_DEBATE.DEBATE_TIME` | `CREATE_TIME` | `CREATE_TIME` |
| Phiếu truyền dịch | `START_TIME – FINISH_TIME` | `CREATE_TIME` | `CREATE_TIME` |
| Phản ứng thuốc | `EXECUTE_TIME` | `CREATE_TIME` | `CREATE_TIME` |
| Tờ điều trị | `TRACKING_TIME` | `CREATE_TIME` | `CREATE_TIME` |
| Truyền máu | `MEASURE_TIME` | `CREATE_TIME` | `CREATE_TIME` |

---

## 6. Thiết kế dữ liệu

### 6.1 Sửa `ADO/InfoRecordADO.cs`

```csharp
class InfoRecordADO
{
    // ===== Giữ nguyên =====
    public long DOCUMENT_TYPE_ID { get; set; }
    public string CODE { get; set; }
    public string TYPE { get; set; }
    public string CREATE_TIME_STR { get; set; }      // thời gian nghiệp vụ (hiển thị)
    public string DEPARTMENT_NAME { get; set; }
    public string SEARCH_CODE { get; set; }
    public long REQ_TYPE_STT_ID { get; set; }
    public string CREATOR { get; set; }

    // ===== Bổ sung 53180 =====
    /// <summary>Thời gian tạo bản ghi (CREATE_TIME) — dùng để lọc và hiển thị cột "Thời gian tạo".</summary>
    public long? CREATE_TIME { get; set; }
    /// <summary>Chuỗi hiển thị của CREATE_TIME.</summary>
    public string CREATE_TIME_REAL_STR { get; set; }

    /// <summary>Mã điều trị — chỉ dùng ở chế độ B (đa hồ sơ).</summary>
    public string TREATMENT_CODE { get; set; }
    /// <summary>ID điều trị — dùng khi double-click mở hồ sơ.</summary>
    public long? TREATMENT_ID { get; set; }
    public string PATIENT_CODE { get; set; }
    public string PATIENT_NAME { get; set; }

    /// <summary>Trạng thái văn bản — tính TRƯỚC khi bind grid (không tính trong CustomUnboundColumnData).</summary>
    public EnumRecordDocumentStatus DOC_STATUS { get; set; }
    /// <summary>Tên trạng thái hiển thị.</summary>
    public string DOC_STATUS_NAME { get; set; }
}
```

### 6.2 File mới `EnumRecordDocumentStatus.cs` (đặt tại gốc plugin)

```csharp
/// <summary>
/// Trạng thái văn bản của 1 y lệnh trên màn tra soát hồ sơ bệnh án (việc 53180).
/// Không có trong IMSys.DbConfig vì đây là trạng thái tính toán phía client
/// từ danh sách V_EMR_DOCUMENT gắn với y lệnh.
/// </summary>
public enum EnumRecordDocumentStatus
{
    /// <summary>Y lệnh chưa sinh văn bản EMR nào.</summary>
    NoDocument = 0,

    /// <summary>Có văn bản nhưng chưa văn bản nào được ký.</summary>
    NotSigned = 1,

    /// <summary>Có văn bản đã ký một phần — vẫn còn người chưa ký.</summary>
    Signing = 2,

    /// <summary>Tất cả văn bản của y lệnh đã ký đủ.</summary>
    FullySigned = 3
}
```

### 6.3 EFMODEL sử dụng

| Entity | Vai trò |
|---|---|
| `HIS_SERVICE_REQ` | Y lệnh — có `REQUEST_LOGINNAME`, `INTRUCTION_TIME` |
| `HIS_CARE`, `HIS_DEBATE`, `HIS_INFUSION`, `HIS_MEDI_REACT`, `HIS_TRACKING`, `HIS_TRANSFUSION` | 6 nguồn còn lại — lọc theo `CREATOR` + `CREATE_TIME` |
| `HIS_INFUSION_SUM`, `HIS_MEDI_REACT_SUM`, `HIS_TRANSFUSION_SUM` | Đường nối 3 bảng trên về hồ sơ — 3 bảng đó **không có** cột `TREATMENT_ID` |
| `HIS_TREATMENT` | `OUT_TIME` (đã ra viện), `TREATMENT_CODE`, `TDL_PATIENT_CODE`, `TDL_PATIENT_NAME`. **Không có cột `END_TIME`** |
| `V_EMR_DOCUMENT` | Văn bản: `SIGNERS`, `UN_SIGNERS`, `HIS_CODE`, `DOCUMENT_TYPE_ID` |
| `EMR_DOCUMENT_TYPE` | Loại văn bản — `DOCUMENT_TYPE_CODE` |
| `SAR_PRINT_TYPE` | Biểu in — **`EMR_DOCUMENT_TYPE_CODE`** là khóa nối sang loại văn bản |
| `HIS_EMPLOYEE` | Nguồn combo bác sĩ (`LOGINNAME`, `TEN`) |

---

## 7. Thiết kế xử lý

### 7.1 Luồng Load form (bổ sung vào `FormHisTreatmentRecordChecking_Load`)

```
WaitingManager.Show()
1.  GetControlAcs()                    ← giữ nguyên
2.  InitConfigAndPermission()          ← giữ nguyên
3.  InitComboRequestDoctor()           ← MỚI: nạp combo bác sĩ (từ cache, không gọi API)
4.  InitComboTreatmentStatus()         ← MỚI: nạp 2 dòng Chưa/Đã kết thúc
5.  FillDataToGridTreatment(...)       ← giữ nguyên (khi mở từ plugin khác)
6.  InitGridEmrDocumentType()          ← giữ nguyên
7.  SetDefaultValueControl()           ← giữ nguyên + reset control mới
8.  SetDefaultFilterValue()            ← MỚI: từ ngày = đầu tháng, đến ngày = hôm nay
9.  ProcessCaptionGridInfoRecord()     ← giữ nguyên
10. InitControlState()                 ← giữ nguyên + đọc trạng thái 2 checkbox mới
11. ApplyModeUI()                      ← MỚI: bật/tắt control theo chế độ
12. FillDataToGrid()                   ← sửa: rẽ nhánh theo chế độ
13. SetDefaultProperties()             ← giữ nguyên
WaitingManager.Hide()
```

### 7.2 Xác định chế độ

```csharp
/// <summary>Trả về true nếu đang ở chế độ B (lọc theo bác sĩ chỉ định).</summary>
private bool IsFilterByDoctorMode()
{
    return cboRequestDoctor.EditValue != null
        && !string.IsNullOrWhiteSpace(cboRequestDoctor.EditValue.ToString());
}
```

`cboRequestDoctor.EditValueChanged` → gọi `ApplyModeUI()`:

| | Chế độ A | Chế độ B |
|---|---|---|
| `TxtTreatmentCode.Enabled` | true | false |
| `dtFromDate` / `dtToDate` / `cboTreatmentStatus` `.Enabled` | false | true |
| Caption 3 item trên | mặc định | **Maroon** |
| `btnDat` / `btnKhongDat` / `btnDuyet` / `btnHuyDuyet` | theo trạng thái hồ sơ | **false** |
| Cột `PATIENT_CODE`/`PATIENT_NAME`/`TREATMENT_CODE` | `Visible = false` | `Visible = true` |
| `ucPaging` | ẩn | hiện |
| Panel thông tin BN | giữ | xóa trắng |

### 7.3 Validate chế độ B (trước khi tìm)

```
Nếu IsFilterByDoctorMode() == true:
    - dtFromDate rỗng            → ErrorProvider tại dtFromDate: "Trường dữ liệu bắt buộc" → CHẶN
    - dtToDate rỗng              → ErrorProvider tại dtToDate                              → CHẶN
    - cboTreatmentStatus rỗng    → ErrorProvider tại cboTreatmentStatus                    → CHẶN
    - dtFromDate > dtToDate      → ErrorProvider: "Từ ngày phải nhỏ hơn hoặc bằng Đến ngày"→ CHẶN
    - Khoảng thời gian > 31 ngày → Cảnh báo hỏi Yes/No, chọn No thì dừng
Xóa hết ErrorProvider khi giá trị đã hợp lệ hoặc khi bấm tìm lại.
```

> Thông báo dùng `MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc)`. Thông báo riêng đặt trong `Resources/Message.Lang.vi|en.resx`.

### 7.4 `FillDataToGrid()` — rẽ nhánh

```
FillDataToGrid()
├── IsFilterByDoctorMode() == false  →  FillDataToGridByTreatment()   [code hiện tại, KHÔNG SỬA LOGIC]
└── IsFilterByDoctorMode() == true   →  FillDataToGridByDoctor()      [MỚI]
```

`FillDataToGridByDoctor()`:

```
1. Validate (7.3) — sai thì return
2. WaitingManager.Show()
3. Dựng filter:
      REQUEST_LOGINNAME     = cboRequestDoctor.EditValue
      FROM_TIME             = dtFromDate  → yyyyMMdd000000
      TO_TIME               = dtToDate    → yyyyMMdd235959
      IS_END_TREATMENT      = (cboTreatmentStatus == "Đã kết thúc")
4. Gọi API mới (mục 9) — GetRO
5. SplitByTreatment() → tách thành từng hồ sơ, chạy chung ProcessDataADO() với chế độ A
6. Lấy văn bản CẢ TRANG: api/EmrDocument/GetView theo TREATMENT_CODEs (1 lần gọi)
   Nạp sẵn luồng ký CẢ TRANG: api/EmrSign/GetView theo DOCUMENT_ID (1 lần gọi)
7. Lọc client theo 2 checkbox nhanh:
      chkNoDocument      → DOC_STATUS == NoDocument
      chkNotFullySigned  → DOC_STATUS != FullySigned
      chkToiTao          → CREATOR == login hiện tại
8. Gc_InfoRecord.BeginUpdate() → DataSource → EndUpdate()
9. WaitingManager.Hide(); SessionManager.ProcessTokenLost(param)
```

> **SAI SO VỚI BẢN THIẾT KẾ GỐC — đã sửa ở khối trên.**
> - **Bỏ `DOCUMENT_TYPE_ID` khỏi filter.** API luôn trả toàn bộ 7 nguồn: lưới trái cần đếm được số
>   lượng của **mọi** loại văn bản, lọc theo loại ở server sẽ làm các loại khác mất dữ liệu.
> - **Không map thẳng sang `InfoRecordADO`.** Server trả dữ liệu thô của 11 bảng; client
>   `SplitByTreatment()` rồi dùng chung `ProcessDataADO()` với chế độ A, nên quy tắc quy đổi chỉ
>   nằm một chỗ.
> - **Phân trang chưa làm** — xem ghi chú ở 7.5.

Click 1 dòng ở chế độ B → lọc trong `ListDocument` đã tải sẵn theo `SEARCH_CODE` của dòng đó → bind
grid phải. Không gọi thêm API.

> **Lọc văn bản phải dùng `TREATMENT_CODEs`, KHÔNG dùng `TREATMENT_IDs`.**
> `V_EMR_DOCUMENT.TREATMENT_ID` là nullable, bộ lọc `TREATMENT_IDs` của EMR yêu cầu
> `o.TREATMENT_ID.HasValue` nên bỏ qua mọi văn bản chưa gán `TREATMENT_ID` — đúng nhóm văn bản mới
> sinh, **chưa ký**. Dùng `TREATMENT_IDs` thì trạng thái *Chưa ký* không bao giờ xuất hiện ở chế độ B.

### 7.5 Hiệu năng — bắt buộc

| Yêu cầu | Cách làm |
|---|---|
| Không gọi `BackendDataWorker.Get<T>()` trong vòng lặp | Dựng `Dictionary` 1 lần trước vòng lặp (khoa, loại y lệnh, nhân viên) |
| Không tính toán trong `CustomUnboundColumnData` | `DOC_STATUS` tính sẵn khi map ADO. Hàm unbound chỉ còn trả icon theo `DOC_STATUS` |
| Grid bind | `BeginUpdate()` / `EndUpdate()` |
| Chế độ B — văn bản | 1 lần gọi `api/EmrDocument/GetView` cho cả trang, không gọi theo từng dòng |
| Chế độ B — luồng ký | 1 lần gọi `api/EmrSign/GetView` cho cả trang; từ điển gộp thêm, không dựng lại, và bỏ qua văn bản đã hỏi |
| Chế độ B — phân trang | **Chưa làm** — xem ghi chú dưới |

> **SAI SO VỚI BẢN ĐÃ CODE — phân trang chưa hoạt động.**
> Backend không đọc `CommonParam.Start`/`.Limit` và không gán `.Count`; client vẫn truyền và vẫn
> đọc. Hệ quả: `ucPaging` hiện tổng = 0, đổi trang trả về cùng một tập dữ liệu. Màn hình vẫn dùng
> được vì client dựng lưới từ tập đầy đủ, nhưng khoảng thời gian rộng sẽ tải nặng — backend có
> ngưỡng cảnh báo 20.000 dòng trong log.
> Khi làm phải chốt **đơn vị đếm là HỒ SƠ, không phải y lệnh**: cắt trang giữa chừng một hồ sơ sẽ
> làm hồ sơ đó thiếu y lệnh.

---

## 8. Quy tắc trạng thái y lệnh

Với 1 y lệnh, gọi `docs` = danh sách `V_EMR_DOCUMENT` gắn với y lệnh đó (khớp qua `HIS_CODE` chứa `SEARCH_CODE`).

| Trạng thái | Điều kiện | Icon | Ý nghĩa |
|---|---|---|---|
| `NoDocument` | `docs` rỗng | Ô đen | Chưa tạo văn bản |
| `NotSigned` | Có `docs`, **tất cả** đều `SIGNERS` rỗng | Ô đen | Có văn bản, chưa ký |
| `Signing` | Có ít nhất 1 doc còn `UN_SIGNERS` khác rỗng | Ô vàng | Đang ký, chưa xong |
| `FullySigned` | **Tất cả** docs đều có `SIGNERS` và `UN_SIGNERS` rỗng | Ô xanh | Hoàn thành |

### Thay đổi so với hiện tại — LƯU Ý CHO TESTER

Code hiện tại dùng `documents.Exists(...)` (chỉ cần **một** văn bản ký xong là báo "đã ký"). Yêu cầu mới là "đã ký **hết**", nên đổi sang kiểm tra **tất cả**.

```
Ví dụ: y lệnh X có 2 văn bản — VB1 ký xong, VB2 chưa ký.
   Bản CŨ : hiển thị "Hoàn thành"      ← SAI so với nghiệp vụ mới
   Bản MỚI: hiển thị "Đang ký"          ← ĐÚNG
```

Đây là **thay đổi hành vi hiển thị hiện có** — tester cần kiểm tra riêng tình huống này.

Thứ tự ưu tiên khi đánh giá: `NoDocument` → `NotSigned` → `Signing` → `FullySigned`.

---

## 9. Backend — ĐÃ CODE

> Nội dung chi tiết và chính xác nằm ở **`53180_B_KyThuat_TraSoatHoSoBenhAn.md`, Phần 4.1**.
> Mục này giữ lại để đối chiếu, các chỗ khác bản đã code được đánh dấu.

### API mới

```
GET  api/HisTreatment/GetServiceReqForRecordChecking
Consumer: MosConsumer
```

**Filter đầu vào** — `HisServiceReqForRecordCheckingFilter : FilterBase`:

| Trường | Kiểu | Bắt buộc | Ý nghĩa |
|---|---|---|---|
| `REQUEST_LOGINNAME` | string | Có | Tài khoản bác sĩ chỉ định |
| `FROM_TIME` | long? | Có | `yyyyMMdd000000` |
| `TO_TIME` | long? | Có | `yyyyMMdd235959` |
| `IS_END_TREATMENT` | bool? | Không | `true` = đã kết thúc, `false` = chưa, `null` = không lọc |

> **SAI SO VỚI BẢN THIẾT KẾ GỐC**: bản gốc có thêm `DOCUMENT_TYPE_ID` và ghi `IS_END_TREATMENT` là
> bắt buộc. Bản đã code **bỏ `DOCUMENT_TYPE_ID`** — API luôn trả toàn bộ 7 nguồn vì lưới trái cần
> đếm số lượng của mọi loại văn bản.

**Quy tắc lọc phía server** — 11 câu `SELECT` rời, mỗi bảng một câu:

```
- Nhóm y lệnh (HIS_SERVICE_REQ):
      REQUEST_LOGINNAME = @loginname
      AND INTRUCTION_TIME >= @from AND INTRUCTION_TIME <= @to
      (không lọc theo SERVICE_REQ_STT_ID — xem ghi chú dưới)
- Nhóm CARE / DEBATE / TRACKING:  CREATOR = @loginname AND CREATE_TIME trong khoảng
- Nhóm INFUSION / MEDI_REACT / TRANSFUSION: như trên, nhưng phải join bảng _SUM
      mới tới được HIS_TREATMENT (3 bảng này không có cột TREATMENT_ID)
- Điều kiện trạng thái hồ sơ:
      @IS_END_TREATMENT = true  → TRE.OUT_TIME IS NOT NULL
      @IS_END_TREATMENT = false → TRE.OUT_TIME IS NULL
      @IS_END_TREATMENT = null  → BỎ HẲN join HIS_TREATMENT
- Treatments + 3 bảng _SUM: nạp theo danh sách ID gom được từ 7 nguồn trên.
```

> **SAI SO VỚI BẢN THIẾT KẾ GỐC**: bản gốc ghi `END_TIME`. **Cột đó không tồn tại** trên
> `HIS_TREATMENT` — chạy sẽ `ORA-00904`. Bản đã code dùng `OUT_TIME`.

**Không lọc theo trạng thái y lệnh.** Đã thử thêm `SERVICE_REQ_STT_ID <> ID__HT` rồi bỏ: lọc bỏ y
lệnh đã hoàn thành sẽ giấu mất chính những y lệnh cần tra soát chữ ký.

**Kết quả trả về** — `HisServiceReqForRecordCheckingSDO`, gồm **11 danh sách entity thô**:

```
Treatments, ServiceReqs, Trackings, Cares, Debates,
Infusions, MediReacts, Transfusions,
InfusionSums, MediReactSums, TransfusionSums
```

Dùng đúng bộ model của `GetInfoForRecordChecking` (chế độ A) để hai chế độ không phải map qua lại.
Client `SplitByTreatment()` tách thành từng hồ sơ rồi chạy chung `ProcessDataADO()`.

> **SAI SO VỚI BẢN THIẾT KẾ GỐC**: bản gốc yêu cầu server trả mỗi dòng đã quy đổi sẵn
> (`DOCUMENT_TYPE_ID`, `TYPE`, `DEPARTMENT_NAME`, `SEARCH_CODE`…) kèm `SIGNERS` / `UN_SIGNERS`.
> - Việc quy đổi **để client làm**, dùng chung `ProcessDataADO()` với chế độ A.
> - `SIGNERS` / `UN_SIGNERS` **không lấy được từ MOS**: `V_EMR_DOCUMENT` nằm ở service và DB khác
>   (`BACKEND/EMR`). Client tự gọi `api/EmrDocument/GetView`.

### API dùng lại (không đổi)

| API | Dùng cho |
|---|---|
| `api/HisTreatment/GetInfoForRecordChecking` | Chế độ A — giữ nguyên |
| `api/EmrDocument/GetView` | Danh sách văn bản của 1 hồ sơ / 1 y lệnh |
| `api/EmrDocumentType/Get` | Danh sách loại văn bản (grid trái) |
| `api/EmrSign/GetView` | Thông tin luồng ký |

---

## 10. Chức năng "Tạo văn bản"

### 10.1 Cơ chế map Loại văn bản → Biểu in

Cấu hình tại màn **Biểu in** (`SAR.Desktop.Plugins.SarPrintType`), trường "Loại văn bản" lưu vào `SAR_PRINT_TYPE.EMR_DOCUMENT_TYPE_CODE`.

Lấy danh sách biểu in — **không cần API**, dữ liệu có sẵn trong cache:

```csharp
var printTypes = BackendDataWorker.Get<SAR_PRINT_TYPE>()
    .Where(o => o.EMR_DOCUMENT_TYPE_CODE == CurrentType.DOCUMENT_TYPE_CODE
             && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
    .OrderBy(o => o.PRINT_TYPE_NAME)
    .ToList();
```

### 10.2 Luồng bấm nút "Tạo văn bản"

```
1. Lấy dòng đang chọn ở Gv_InfoRecord.
      - Không có dòng                    → thoát (nút đã disable)
      - DOC_STATUS != NoDocument         → thoát (nút đã disable)

2. Lấy loại văn bản = CurrentType (loại đang chọn ở grid trái).

3. Lọc SAR_PRINT_TYPE theo EMR_DOCUMENT_TYPE_CODE (mục 10.1):
      0 biểu in  → thông báo "Loại văn bản chưa được cấu hình biểu in." → dừng
      1 biểu in  → dùng luôn, không hỏi
      >1 biểu in → hiện DXPopupMenu để người dùng chọn (Tag = PRINT_TYPE_CODE)

4. Gọi RunPrint(printTypeCode):
      RichEditorStore(SarConsumer, ConfigSystems.URI_API_SAR, ngôn ngữ, PrintStoreLocation.ROOT_PATH)
          .RunPrintTemplate(printTypeCode, DelegateRunPrinter)

5. DelegateRunPrinter(printCode, fileName):
      switch (printCode)
          case <mã đã hỗ trợ> : loadMps = new MpsXXXBehavior(...); break;
          default             : thông báo "Biểu mẫu chưa được hỗ trợ tạo văn bản
                                từ màn tra soát." ; return false;
      return loadMps.Load(printCode, fileName, null);

6. MpsXXXBehavior.Load():
      - build PDO của mẫu đó
      - PrintCustomShow<T>.SignRun(treatmentCode, roomId, documentName)
            → EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(...)
            → MpsPrinter.Run(PrintData { EmrInputADO = inputADO })

7. Sau khi popup ký/preview đóng → gọi lại FillDataToGrid() để cập nhật trạng thái.
```

> **Không cần API tạo văn bản EMR**. Việc tạo văn bản do MPS + SignLibrary xử lý thông qua `EmrInputADO` — giống hệt `Library.PrintBordereau`.

### 10.3 Chế độ xem trước / in

Dùng đúng cơ chế chuẩn của các chức năng khác, **không hardcode**:

| `PrintOption.Value` | `PreviewType` | Khi nào |
|---|---|---|
| `SHOW_DIALOG` | `ShowDialog` | Mặc định |
| `PRINT_NOW` | `PrintNow` | Config `CHE_DO_IN_PHAN_MEM = 2` |
| `EMR_SIGN_NOW` | `EmrSignNow` | Tạo văn bản + ký, không in |
| `PRINT_NOW_AND_EMR_SIGN_NOW` | `EmrSignAndPrintNow` | Tạo + ký + in |
| `EMR_SIGN_AND_PRINT_PREVIEW` | `EmrSignAndPrintPreview` | Tạo + ký + xem trước |

### 10.4 Chiến lược tái sử dụng (theo yêu cầu "tái dùng hoặc copy nhiều nhất có thể")

**Bước 1 — ưu tiên gọi thẳng Print Library có sẵn**, không tự viết Behavior:

| Loại văn bản | Thư viện dùng lại |
|---|---|
| Phiếu chỉ định (`ID__SERVICE_ASSIGN`) | `Library.PrintServiceReq.PrintServiceReqProcessor` |
| Đơn thuốc (`ID__PRESCRIPTION`) | `Library.PrintPrescription.PrintPrescriptionProcessor` |
| Phiếu kết quả (`ID__SERVICE_RESULT`) | `Library.PrintTestTotal` (nếu phù hợp) |

**Bước 2 — loại văn bản chưa có Library**: copy khung từ `Library.PrintBordereau` sang plugin:

```
HIS.Desktop.Plugins.HisTreatmentRecordChecking/
├── Base/
│   ├── ILoad.cs                  ← copy nguyên
│   ├── MpsDataBase.cs            ← copy, rút gọn property không dùng
│   ├── PrintOption.cs            ← copy nguyên
│   ├── GlobalDataStore.cs        ← copy nguyên
│   └── PrintTypeCodeWorker.cs    ← MỚI: hằng số mã biểu in được hỗ trợ
├── PrintCustomShow.cs            ← copy nguyên
└── MpsBehavior/
    └── Mps000XXX/Mps000XXXBehavior.cs   ← mỗi mẫu 1 file
```

### 10.5 Giới hạn cần biết

`switch (printCode)` là **hardcode**. Mỗi mẫu in muốn hỗ trợ cần: 1 Behavior + 1 PDO + 1 reference DLL trong `.csproj` (`..\..\..\..\LIB\MPSv2\MPS.PDO\`). Cấu hình SAR có thể gán bất kỳ biểu in nào cho loại văn bản, nên **bắt buộc có nhánh `default`** báo "Biểu mẫu chưa được hỗ trợ" thay vì lỗi hệ thống.

> **Cần chốt trước khi code**: danh sách mã biểu in `Mps000XXX` hỗ trợ ở đợt 1.

---

## 11. Danh sách file thay đổi

### File mới

| File | Nội dung |
|---|---|
| `EnumRecordDocumentStatus.cs` | Enum trạng thái văn bản (mục 6.2) |
| `Base/ILoad.cs` | Copy từ PrintBordereau |
| `Base/MpsDataBase.cs` | Copy từ PrintBordereau |
| `Base/PrintOption.cs` | Copy từ PrintBordereau |
| `Base/GlobalDataStore.cs` | Copy từ PrintBordereau |
| `Base/PrintTypeCodeWorker.cs` | Hằng số mã biểu in |
| `PrintCustomShow.cs` | Copy từ PrintBordereau |
| `MpsBehavior/Mps000XXX/Mps000XXXBehavior.cs` | Mỗi mẫu 1 file |
| `Resources/Lang.vi.resx`, `Lang.en.resx` | **Chưa có** — phải tạo mới |
| `Resources/Message.Lang.vi.resx`, `Message.Lang.en.resx` | Thông báo riêng plugin |
| `Resources/ResourceLanguageManager.cs`, `ResourceMessage.cs` | Accessor |
| `HisRequestUriStore.cs` | Hằng số URI API |

### File sửa

| File | Nội dung sửa |
|---|---|
| `RecordChecking/FormHisTreatmentRecordChecking.Designer.cs` | Thêm 7 control + 4 cột grid |
| `RecordChecking/FormHisTreatmentRecordChecking.cs` | Rẽ nhánh chế độ, validate, luồng tạo VB |
| `ADO/InfoRecordADO.cs` | Thêm 8 property (mục 6.1) |
| `HIS.Desktop.Plugins.HisTreatmentRecordChecking.csproj` | Reference PDO + Print Library |

### Khuyến nghị tách partial (form đang 2081 dòng — vượt ngưỡng 500 của quy tắc)

```
FormHisTreatmentRecordChecking.cs            ← khai báo, constructor, Load
FormHisTreatmentRecordChecking__Filter.cs    ← 3 bộ lọc mới, ApplyModeUI, validate
FormHisTreatmentRecordChecking__Process.cs   ← FillDataToGrid A/B, map ADO
FormHisTreatmentRecordChecking__Print.cs     ← Tạo văn bản, DelegateRunPrinter
FormHisTreatmentRecordChecking__Grid.cs      ← các event grid, unbound column, style
```

---

## 12. Bổ sung bắt buộc theo quy chuẩn

### 12.1 Đa ngôn ngữ

Plugin **hiện chưa có** thư mục `Resources/` đa ngôn ngữ (`SetCaptionByLanguageKey()` đang bị comment ở dòng 184). Việc này phải bổ sung:

- Tạo đủ 4 file `.resx` + 2 file accessor.
- Khai báo **toàn bộ** caption trong `Lang.vi.resx` + `Lang.en.resx` (số dòng bằng nhau).
- Bỏ comment và hoàn thiện `SetCaptionByLanguageKey()`, gọi trong `Load`.
- Chuyển các chuỗi đang hardcode sang resource: `"Chưa chốt"`, `"Đã duyệt"`, `"Đạt"`, `"Chưa đạt"`, `"Chưa xác định"`, `"Khác"`, `"Không xác định được văn bản ký"`.

### 12.2 ControlState

2 checkbox mới `chkNoDocument`, `chkNotFullySigned` phải nhớ trạng thái giữa các phiên:

- Thêm vào `InitControlState()` (đọc) và mỗi `CheckedChanged` (ghi).
- **Bắt buộc** kiểm tra `if (IsLoadFirstForm) return;` ở đầu handler.
- `KEY = control.Name`, `MODULE_LINK = "HIS.Desktop.Plugins.HisTreatmentRecordChecking"`.

> Tiện thể sửa lỗi có sẵn: `chkUuTien_CheckedChanged` đang **thiếu** kiểm tra `IsLoadFirstForm` nên bị kích hoạt sớm trong lúc Load và ghi thừa xuống SQLite.

### 12.3 Ghi log

- Mọi hàm có `try-catch`.
- Gọi API → `LogSystem.Error(ex)`; sự kiện giao diện → `LogSystem.Warn(ex)`.
- Trước khi gọi API mới: `LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => filter), filter))`.
- Tạo văn bản thành công → `LogUtil.LogActionSuccess("FormHisTreatmentRecordChecking", "CreateDocument", loginName)`.

---

## 13. Test case

### A. Regression — chế độ A phải y nguyên

| # | Bước | Kết quả mong đợi |
|---|---|---|
| A1 | Mở màn hình, không đụng ô bác sĩ. Nhập mã điều trị → Tìm | Hiển thị giống hệt bản cũ |
| A2 | Chọn từng loại văn bản ở grid trái | Grid giữa/phải đổi đúng như cũ |
| A3 | Bấm Đạt / Không đạt / Duyệt / Hủy duyệt | Hoạt động như cũ, trạng thái đổi đúng |
| A4 | Tích "Ưu tiên có dữ liệu" / "Tôi tạo" / "Bao gồm VB hủy" | Như cũ |
| A5 | Mở từ plugin khác truyền `listTreatmentId` | Grid danh sách điều trị hiện như cũ |
| A6 | Đóng, mở lại | Trạng thái checkbox được nhớ đúng |

### B. Bộ lọc theo bác sĩ (KB1, KB2)

| # | Bước | Kết quả mong đợi |
|---|---|---|
| B1 | Chọn bác sĩ, để trống Từ ngày | Báo lỗi ngay tại ô Từ ngày, không gọi API |
| B2 | Chọn bác sĩ, để trống Trạng thái HS | Báo lỗi tại ô Trạng thái, không gọi API |
| B3 | Chọn bác sĩ + đủ thời gian + trạng thái → Tìm | Hiện tất cả y lệnh của bác sĩ trong khoảng đó |
| B4 | Từ ngày > Đến ngày | Báo lỗi "Từ ngày phải nhỏ hơn hoặc bằng Đến ngày" |
| B5 | Khoảng thời gian > 31 ngày | Hiện cảnh báo xác nhận |
| B6 | Ở chế độ B | Ô mã điều trị bị khóa; 4 nút Đạt/Không đạt/Duyệt/Hủy duyệt bị khóa |
| B7 | Ở chế độ B | Grid giữa có cột Mã BN, Tên BN, Mã điều trị |
| B8 | Xóa trắng ô bác sĩ | Quay về chế độ A đầy đủ, ô mã điều trị mở lại |
| B9 | Chọn trạng thái "Đã kết thúc" | Chỉ hiện y lệnh của hồ sơ **có** thời gian kết thúc |
| B10 | Chọn trạng thái "Chưa kết thúc" | Chỉ hiện y lệnh của hồ sơ **chưa** có thời gian kết thúc |
| B11 | Double-click 1 dòng | Nạp mã điều trị dòng đó, chuyển về chế độ A |
| B12 | Kết quả nhiều trang | Phân trang hoạt động, chuyển trang không mất bộ lọc |

### C. Trạng thái y lệnh (KB3, KB5, KB6, KB8)

| # | Tình huống | Kết quả mong đợi |
|---|---|---|
| C1 | Y lệnh không có văn bản nào | Trạng thái "Chưa tạo văn bản", icon ô đen |
| C2 | Có văn bản, chưa ai ký | "Chưa ký" |
| C3 | Có 2 VB: 1 ký xong, 1 chưa ký | **"Đang ký"** (không phải "Hoàn thành") — điểm khác bản cũ |
| C4 | Tất cả VB đã ký đủ | "Hoàn thành", icon xanh |
| C5 | Tích "Chưa tạo văn bản" | Chỉ còn dòng trạng thái C1 |
| C6 | Tích "Chưa ký hết" | Chỉ còn dòng C1 + C2 + C3 |
| C7 | Tích cả 2 checkbox | Chỉ còn dòng C1 |
| C8 | Sau khi đã tạo + ký hết, tích "Chưa tạo VB" | Danh sách trống |

### D. Cột thời gian (mục 7)

| # | Bước | Kết quả mong đợi |
|---|---|---|
| D1 | Chọn loại "Phiếu chỉ định" | Cột "Thời gian chỉ định" = giờ y lệnh; cột "Thời gian tạo" = giờ tạo bản ghi |
| D2 | Chọn loại "Biên bản hội chẩn" | "Thời gian chỉ định" = giờ hội chẩn; "Thời gian tạo" = giờ tạo — có thể khác nhau |
| D3 | Lọc ngày X, xem cột "Thời gian tạo" | Mọi dòng đều nằm trong khoảng đã lọc |

### E. Tạo văn bản (KB4)

| # | Bước | Kết quả mong đợi |
|---|---|---|
| E1 | Chọn y lệnh **đã có** văn bản | Nút "Tạo văn bản" bị khóa |
| E2 | Chọn y lệnh **chưa có** văn bản | Nút "Tạo văn bản" mở |
| E3 | Không chọn dòng nào | Nút bị khóa |
| E4 | Loại VB chưa cấu hình biểu in nào | Báo "Loại văn bản chưa được cấu hình biểu in", không lỗi |
| E5 | Loại VB có đúng 1 biểu in | Vào thẳng, không hỏi chọn |
| E6 | Loại VB có nhiều biểu in | Hiện danh sách để chọn |
| E7 | Chọn biểu in **chưa được hỗ trợ** | Báo "Biểu mẫu chưa được hỗ trợ tạo văn bản từ màn tra soát", không lỗi hệ thống |
| E8 | Tạo văn bản thành công | Đóng popup → grid tự làm mới → trạng thái đổi từ "Chưa tạo văn bản" |
| E9 | Hủy giữa chừng | Trạng thái không đổi, không sinh văn bản rác |
| E10 | Tạo VB ở chế độ B | Hoạt động y như chế độ A |

### F. Hiệu năng

| # | Bước | Kết quả mong đợi |
|---|---|---|
| F1 | Lọc 1 bác sĩ, 1 tháng, hồ sơ chưa kết thúc | Có `WaitingManager`, trả kết quả dưới 5 giây |
| F2 | Cuộn grid nhiều dòng | Không giật, không gọi API khi cuộn |
| F3 | Đổi loại văn bản ở grid trái liên tục | Không treo giao diện |

---

## 14. Điểm còn treo

Danh sách cập nhật đầy đủ ở **`53180_B_KyThuat_TraSoatHoSoBenhAn.md`, Phần 11**.

| # | Nội dung | Ảnh hưởng | Trạng thái |
|---|---|---|---|
| 1 | **Danh sách mã biểu in `Mps000XXX` hỗ trợ tạo văn bản đợt 1** | Mỗi mẫu = 1 Behavior + 1 PDO + 1 reference | Còn treo |
| 2 | **Phân trang chưa làm** — backend không đọc `Start`/`Limit`, không gán `Count` | `ucPaging` hiện tổng = 0, đổi trang trả cùng tập dữ liệu | Còn treo |
| 3 | Chốt cách xác định hồ sơ đã kết thúc: `OUT_TIME` hay `IS_PAUSE` | Đang dùng `OUT_TIME`. Hai cờ khác nghĩa | Còn treo |
| 4 | Chốt nghĩa `APPROVAL_STORE_STT_ID = 3`: "Đạt" (client) hay "Đang xử lý" (backend) | Bảng quy đổi trạng thái ở `FillDataToControl()` | Còn treo |
| 5 | Bác sĩ A có được xem y lệnh của bác sĩ B không? Kiểm soát bằng quyền ACS? | Bảo mật | Còn treo |
| ~~6~~ | ~~Backend xác nhận API mới ở mục 9~~ | | **Đã xong** — API đã code |
| ~~7~~ | ~~Giới hạn khoảng thời gian tối đa — đề xuất 31 ngày~~ | | **Đã xong** — hỏi xác nhận khi vượt 31 ngày |

---

## 15. Thứ tự triển khai

| Bước | Nội dung | Trạng thái |
|---|---|---|
| 1 | Bổ sung `Resources/` đa ngôn ngữ + `SetCaptionByLanguageKey()` | **Xong** |
| 2 | Thêm cột "Thời gian tạo" + `EnumRecordDocumentStatus` + tính trạng thái theo quy tắc mới | **Xong** |
| 3 | Thêm 3 bộ lọc + `ApplyModeUI` + validate | **Xong** |
| 4 | Nối API mới cho chế độ B | **Xong** — riêng phân trang còn treo, xem mục 14 |
| 5 | 2 checkbox lọc nhanh + ControlState | **Xong** |
| 6 | Nút "Tạo văn bản" + khung in copy từ PrintBordereau | **Xong** — trừ danh sách `Mps000XXX`, xem mục 14 |

---

## 16. Ghi chú

Sau khi hoàn thành code, phải tạo/cập nhật tài liệu module theo quy tắc `module_docs.md`:
`docs/HIS.Desktop.Plugins.HisTreatmentRecordChecking.md` (9 mục, kèm Changelog).
