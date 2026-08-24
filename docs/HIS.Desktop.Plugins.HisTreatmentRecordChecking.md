# Tra Soát Hồ Sơ Bệnh Án — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisTreatmentRecordChecking |
| Loại | Form (`FormBase`) |
| Mục đích | Tra soát mức độ hoàn thiện hồ sơ bệnh án: đối chiếu từng y lệnh với văn bản EMR tương ứng, xác nhận Đạt / Không đạt / Duyệt lưu trữ, và tạo văn bản còn thiếu ngay trên màn hình |
| Category | Common |
| Trạng thái | Hoàn thành (việc 53180) |

### Đầu vào (`HisTreatmentRecordCheckingBehavior.Run`)

| Kiểu | Bắt buộc | Ý nghĩa |
|------|----------|---------|
| `Inventec.Desktop.Common.Modules.Module` | Có | Context module, phòng làm việc |
| `long` | Không | `treatmentId` — mở thẳng một hồ sơ |
| `List<long>` | Không | Danh sách hồ sơ, hiện thêm lưới chọn hồ sơ bên phải |

---

## 2. Quy Trình Nghiệp Vụ

Màn hình có **hai cách tra soát**, quyết định bởi ô *Bác sĩ chỉ định*.

### Cách 1 — Theo hồ sơ (mặc định)

```
Nhập/quét mã hồ sơ → Tìm
  → api/HisTreatment/GetInfoForRecordChecking   (1 hồ sơ)
  → gộp 7 nguồn dữ liệu thành danh sách y lệnh
  → api/EmrDocument/GetView                     (văn bản của hồ sơ)
  → đối chiếu y lệnh ↔ văn bản, hiển thị trạng thái
  → Đạt / Không đạt / Duyệt / Hủy duyệt
```

### Cách 2 — Theo bác sĩ chỉ định (việc 53180)

```
Chọn bác sĩ + khoảng thời gian + trạng thái hồ sơ → Tìm
  → api/HisTreatment/GetServiceReqForRecordChecking  (nhiều hồ sơ, dữ liệu thô 11 bảng)
  → SplitByTreatment()  → tách thành từng hồ sơ, dùng chung ProcessDataADO() với Cách 1
  → api/EmrDocument/GetView theo TREATMENT_CODEs     (1 lần gọi cho cả trang)
  → api/EmrSign/GetView cho toàn bộ văn bản của trang (nạp sẵn luồng ký)
  → đối chiếu, lọc nhanh, tạo văn bản
```

> **Lọc văn bản bằng `TREATMENT_CODEs`, KHÔNG dùng `TREATMENT_IDs`.**
> `V_EMR_DOCUMENT.TREATMENT_ID` là nullable và bộ lọc `TREATMENT_IDs` của EMR yêu cầu
> `o.TREATMENT_ID.HasValue`, nên bỏ qua mọi văn bản chưa gán `TREATMENT_ID` — đúng nhóm văn bản
> mới sinh, **chưa ký**. Dùng `TREATMENT_CODEs` để khớp với Cách 1 (`TREATMENT_CODE__EXACT`).

### Sơ đồ trạng thái hồ sơ (`APPROVAL_STORE_STT_ID`)

```
Chưa chốt (null) ──Đạt──> Đạt (3) ──Duyệt──> Đã duyệt (1)
       │                    ▲                     │
       └──Không đạt──> Chưa đạt (2)               │
                            └────────Hủy duyệt────┘
```

> **Mâu thuẫn chưa chốt**: plugin gọi giá trị `3` là "Đạt", nhưng
> `IMSys.DbConfig.HIS_RS.HIS_TREATMENT` đặt tên là `APPROVAL_STORE_STT_ID__DANG_XU_LY`
> (`__CHOT = 1`, `__TU_CHOI = 2`, `__DANG_XU_LY = 3`). Cần nghiệp vụ xác nhận trước khi thay số
> hardcode bằng hằng số.

### Trạng thái văn bản của một y lệnh (`EnumRecordDocumentStatus`)

| Trạng thái | Điều kiện | Icon |
|---|---|---|
| `NoDocument` | Không có văn bản nào | Đen |
| `NotSigned` | Có văn bản, **tất cả** chưa ký | Đen |
| `Signing` | Đã ký một phần | Vàng |
| `FullySigned` | **Tất cả** văn bản đã ký đủ | Xanh |

> Một y lệnh có thể sinh nhiều văn bản. `FullySigned` dùng `All()` — chỉ đạt khi **mọi** văn bản đã ký xong.

### Điều kiện nghiệp vụ

- Đã chọn bác sĩ → khoảng thời gian và trạng thái hồ sơ trở thành **bắt buộc**.
- Khoảng thời gian tra soát tối đa **31 ngày** (vượt thì hỏi xác nhận).
- Cách 2 khoá 4 nút Đạt / Không đạt / Duyệt / Hủy duyệt — chốt hồ sơ chỉ làm ở Cách 1.
- Chỉ tạo văn bản cho y lệnh **chưa có văn bản**, mỗi lần một y lệnh.

---

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| `HIS_TREATMENT` / `V_HIS_TREATMENT` | Table/View | Hồ sơ điều trị, trạng thái duyệt lưu trữ |
| `HIS_SERVICE_REQ` | Table | Y lệnh — có `REQUEST_LOGINNAME`, `INTRUCTION_TIME` |
| `HIS_CARE`, `HIS_DEBATE`, `HIS_INFUSION`, `HIS_MEDI_REACT`, `HIS_TRACKING`, `HIS_TRANSFUSION` | Table | 6 nguồn còn lại của lưới y lệnh |
| `V_EMR_DOCUMENT` | View | Văn bản EMR — `SIGNERS`, `UN_SIGNERS`, `HIS_CODE` |
| `EMR_DOCUMENT_TYPE` | Table | Loại văn bản (lưới trái) |
| `V_EMR_SIGN` | View | Luồng ký |
| `SAR_PRINT_TYPE` | Table | Biểu in MPS, nối qua `EMR_DOCUMENT_TYPE_CODE` |
| `HIS_EMPLOYEE` | Table | Nguồn combo bác sĩ (`LOGINNAME`, `TDL_USERNAME`) |
| `ACS_CONTROL` | Table | Quyền nút Duyệt / Hủy duyệt |

### Quan hệ chính

- `HIS_TREATMENT → HIS_SERVICE_REQ` (1-n qua `TREATMENT_ID`)
- Y lệnh ↔ văn bản: `V_EMR_DOCUMENT.HIS_CODE` **chứa** `SEARCH_CODE` của y lệnh
  (`SERVICE_REQ_CODE:xxx`, `HIS_CARE:id`, `HIS_DEBATE:id`, …)
- `EMR_DOCUMENT_TYPE.DOCUMENT_TYPE_CODE = SAR_PRINT_TYPE.EMR_DOCUMENT_TYPE_CODE`

### Nguồn "bác sĩ" và "thời gian" theo loại (QT-05)

| Loại văn bản | Coi là của bác sĩ khi | Lọc thời gian theo |
|---|---|---|
| Phiếu chỉ định / Phiếu kết quả / Đơn thuốc | `REQUEST_LOGINNAME` | `INTRUCTION_TIME` |
| 6 loại còn lại | `CREATOR` | `CREATE_TIME` |

---

## 4. UI Layout

```
┌──────────────────────────────────────────────────────────────────────────────┐
│ [Mã hồ sơ__________] [Tìm (Ctrl F)]                                          │
├──────────────────────────────────────────────────────────────────────────────┤
│ ▸ Thông tin bệnh nhân                                                         │
├──────────────────────────────────────────────────────────────────────────────┤
│ ☑ Ưu tiên có dữ liệu ☐ Tôi tạo ☐ Bao gồm VB hủy ☐ Chưa tạo VB ☐ Chưa ký hết │
│ Bác sĩ chỉ định[▼] Từ ngày[__] Đến ngày[__] Trạng thái HS[▼]  [Tạo văn bản] │
├───────────────┬──────────────────────────────┬───────────────────────────────┤
│ Loại văn bản  │ Danh sách y lệnh             │ Danh sách văn bản             │
│ (lưới trái)   │ (lưới giữa)                  │ (lưới phải)                   │
│               ├──────────────────────────────┤                               │
│               │ [◀ 1 2 3 ▶]  phân trang      │                               │
├───────────────┴──────────────────────────────┴───────────────────────────────┤
│                        [Không đạt] [Đạt] [Duyệt] [Hủy duyệt]                 │
└──────────────────────────────────────────────────────────────────────────────┘
```

### Cột lưới y lệnh

| # | FieldName | Caption | Hiện ở |
|---|---|---|---|
| 0 | `STT` | (icon trạng thái) | Cả hai cách |
| 1 | `CODE` | động theo loại | Cả hai |
| 2 | `TYPE` | động theo loại | Cả hai |
| 3 | `CREATE_TIME_STR` | động theo loại | Cả hai |
| 4 | `CREATE_TIME_REAL_STR` | Thời gian tạo | Cả hai |
| 5 | `DEPARTMENT_NAME` | động theo loại | Cả hai |
| 6 | `CREATOR` | động theo loại | Cả hai |
| 7 | `DOC_STATUS_NAME` | Trạng thái VB | Cả hai |
| 8 | `PATIENT_CODE` | Mã BN | **Cách 2** |
| 9 | `PATIENT_NAME` | Tên BN | **Cách 2** |
| 10 | `TREATMENT_CODE` | Mã hồ sơ | **Cách 2** |

> Caption cột 1,2,3,5,6 đổi động theo loại văn bản (`ProcessCaptionGridInfoRecord`).

### UC sử dụng

| UC | Mục đích |
|----|----------|
| `Inventec.UC.Paging.UcPaging` | Phân trang lưới y lệnh ở Cách 2 |

---

## 5. API Endpoints

Khai báo tập trung trong `RecordCheckingUriStore.cs`.

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Tra soát 1 hồ sơ | `api/HisTreatment/GetInfoForRecordChecking` | Mos | `HisTreatmentForRecordCheckingFilter` |
| **Tra soát theo bác sĩ** | `api/HisTreatment/GetServiceReqForRecordChecking` | Mos | `HisServiceReqForRecordCheckingFilter` |
| Danh sách hồ sơ | `api/HisTreatment/GetView` | Mos | `HisTreatmentViewFilter` |
| Loại văn bản | `api/EmrDocumentType/Get` | Emr | `EmrDocumentTypeFilter` |
| Văn bản | `api/EmrDocument/GetView` | Emr | `EmrDocumentViewFilter` |
| Luồng ký | `api/EmrSign/GetView` | Emr | `EmrSignViewFilter` |
| Tải file văn bản | `api/EmrDocument/DownloadFile` | Emr | `EmrDocumentDownloadFileSDO` |
| Xác nhận Đạt / Duyệt | `api/HisTreatment/ApprovalStore` | Mos | `List<long?>` |
| Hủy duyệt | `api/HisTreatment/UnapprovalStore` | Mos | `List<long?>` |
| Xác nhận Chưa đạt | `api/HisTreatment/RejectStore` | Mos | `RejectStoreSDO` |
| Quyền | `AcsRequestUriStore.ACS_TOKEN__AUTHORIZE` | Acs | `AcsTokenLoginSDO` |

### Filter tra soát theo bác sĩ

`HisServiceReqForRecordCheckingFilter : FilterBase`

| Trường | Kiểu | Bắt buộc | Ý nghĩa |
|---|---|---|---|
| `REQUEST_LOGINNAME` | string | Có | Tài khoản bác sĩ |
| `FROM_TIME` / `TO_TIME` | long? | Có | `yyyyMMdd000000` / `yyyyMMdd235959` |
| `IS_END_TREATMENT` | bool? | Không | `true` = đã kết thúc, `false` = chưa, `null` = không lọc |

Gọi bằng `GetRO`. Thiếu một trong ba trường bắt buộc → backend ghi `Warn` và trả `null`.

**Không có `DOCUMENT_TYPE_ID`**: API luôn trả toàn bộ 7 nguồn y lệnh, vì lưới trái cần đếm được số
lượng của mọi loại văn bản. Lọc theo loại chạy phía client, giống Cách 1.

> **Phân trang chưa hoạt động.** Backend không đọc `CommonParam.Start`/`.Limit` và không gán
> `.Count`; client thì vẫn truyền và vẫn đọc. Hệ quả: `ucPaging` hiện tổng = 0, đổi trang nhận về
> cùng một tập dữ liệu. Màn hình vẫn dùng được vì client dựng lưới từ tập đầy đủ, nhưng khoảng thời
> gian rộng sẽ tải nặng. Khi làm phải đếm theo **hồ sơ**, không phải y lệnh — cắt trang giữa chừng
> một hồ sơ sẽ làm hồ sơ đó thiếu y lệnh.

### Kết quả trả về — `HisServiceReqForRecordCheckingSDO`

Dữ liệu **thô của từng bảng**, giữ đúng hình dạng SDO của Cách 1, chỉ khác: `Treatment` → `Treatments`
và có thêm 3 bảng `_SUM` vì `HIS_INFUSION` / `HIS_MEDI_REACT` / `HIS_TRANSFUSION` không có cột `TREATMENT_ID`.

```
Treatments, ServiceReqs, Trackings, Cares, Debates,
Infusions, MediReacts, Transfusions,
InfusionSums, MediReactSums, TransfusionSums

HIS_INFUSION.INFUSION_SUM_ID       → HIS_INFUSION_SUM.ID       → .TREATMENT_ID
HIS_MEDI_REACT.MEDI_REACT_SUM_ID   → HIS_MEDI_REACT_SUM.ID     → .TREATMENT_ID
HIS_TRANSFUSION.TRANSFUSION_SUM_ID → HIS_TRANSFUSION_SUM.ID    → .TREATMENT_ID
```

`SplitByTreatment()` tách dữ liệu phẳng thành từng hồ sơ (`ToLookup` + `Dictionary`, tra cứu O(1)),
rồi gọi `ProcessDataADO(sdo)` — **dùng chung hàm dựng ADO với Cách 1**, không có nhánh xử lý dữ liệu riêng.
Server không quy đổi loại văn bản, không nhân dòng Phiếu chỉ định → Phiếu kết quả; client làm cả hai.

### Quy ước gọi API

- Đọc dữ liệu: overload 5 tham số kèm `SessionManager.ActionLostToken`, biến `paramCommon`, **không** bật `MessageManager`.
- Ghi dữ liệu: overload 4 tham số, có `MessageManager.Show(this, param, success)`.

---

## 6. Dependencies

### Library Plugins

| Library | Mục đích |
|---------|----------|
| `Library.PrintServiceReq` | Tạo văn bản EMR cho Phiếu chỉ định / Phiếu kết quả |
| `Library.EmrGenerate` | (qua PrintServiceReq) sinh `InputADO` ký số |
| `HIS.Desktop.Library.CacheClient` | `ControlStateWorker` — nhớ trạng thái checkbox |
| `Inventec.Common.Controls.EditorLoader` | Nạp combo bác sĩ / trạng thái hồ sơ |

### Inter-Plugin

| Plugin đích | Khi nào mở | Args |
|-------------|-----------|------|
| `HIS.Desktop.Plugins.InfoUser` | Click cột Người tạo / Người chỉ định | `string loginName` |
| `EMR.Desktop.Plugins.EmrSign` | Sau khi ký văn bản qua popup | `long documentId` |

### Phụ thuộc cấu hình

| Màn cấu hình | Cung cấp |
|---|---|
| `HIS.Desktop.Plugins.EmrDocumentType` | Danh sách loại văn bản |
| `SAR.Desktop.Plugins.SarPrintType` | Biểu in MPS của từng loại văn bản (`EMR_DOCUMENT_TYPE_CODE`) |

### Config

| Key | Tác dụng |
|---|---|
| `MOS.HIS_TREATMENT.IS_AUTO_APPROVAL_STORE` | `= 1` → ẩn nút Duyệt (tự động duyệt khi Đạt) |
| `CONFIG_KEY__NUM_PAGESIZE` | Số dòng mỗi trang khi pager chưa khởi tạo |
| `EMR.EMR_DOCUMENT.PATIENT_SIGN.OPTION` | Điều kiện cho phép bệnh nhân ký |

### Quyền ACS

| Mã | Chức năng |
|---|---|
| `HIS000056` | Nút Duyệt |
| `HIS000055` | Nút Hủy duyệt |
| `EMR000002` | Nút In trong popup ký |

---

## 7. Print

| Loại in | Nguồn PrintTypeCode | Thư viện | Chế độ |
|---------|--------------------|----------|--------|
| Văn bản của y lệnh | `SAR_PRINT_TYPE` lọc theo `EMR_DOCUMENT_TYPE_CODE` | `Library.PrintServiceReq` | `PreviewType.EmrSignNow` |

Luồng: nút **Tạo văn bản** → lọc biểu in theo loại văn bản đang chọn → 0 biểu (báo chưa cấu hình) / 1 biểu (chạy luôn) / nhiều biểu (popup chọn) → nạp `V_HIS_SERVICE_REQ` + `V_HIS_SERE_SERV` + `HIS_TREATMENT` + `V_HIS_BED_LOG` → `PrintServiceReqProcessor.Print(printTypeCode)` → làm mới lưới.

### Giới hạn hiện tại — HAI tầng, cần biết cả hai

**Tầng 1 — theo loại văn bản.** Chỉ **Phiếu chỉ định** và **Phiếu kết quả** được nối thư viện.
7 loại còn lại (Đơn thuốc, Tờ điều trị, Phiếu chăm sóc, Hội chẩn, Truyền dịch, Test thuốc, Truyền máu)
báo *"Biểu mẫu này chưa được hỗ trợ tạo văn bản từ màn tra soát"*.

**Tầng 2 — theo mã biểu mẫu.** Ngay cả với Phiếu chỉ định / Phiếu kết quả, `Library.PrintServiceReq`
chỉ dựng được dữ liệu cho **31 mã Mps cố định**. Màn **Biểu in** (`SAR.Desktop.Plugins.SarPrintType`)
lại cho gắn **bất kỳ** mã nào vào một loại văn bản. Mã nằm ngoài danh sách sẽ rơi vào `default: break;`
trong `switch` của thư viện → **không làm gì, không báo lỗi**. Người dùng chọn xong biểu mẫu thì màn hình đứng im.

31 mã `Library.PrintServiceReq` dựng được:

```
Mps000001  Mps000026  Mps000027  Mps000028  Mps000029  Mps000030  Mps000031  Mps000036
Mps000037  Mps000038  Mps000040  Mps000042  Mps000053  Mps000071  Mps000167  Mps000340
Mps000363  Mps000364  Mps000365  Mps000366  Mps000367  Mps000368  Mps000423  Mps000424
Mps000425  Mps000426  Mps000432  Mps000465  Mps000466  Mps000467  Mps000502
```

12 mã `Library.PrintPrescription` dựng được (**chưa nối** vào màn tra soát):

```
Mps000044  Mps000050  Mps000118  Mps000181  Mps000191  Mps000192
Mps000234  Mps000237  Mps000238  Mps000296  Mps000338  Mps000353
```

> Ca đã gặp thực tế: bệnh viện gắn **`Mps000033`** (Phiếu yêu cầu phẫu thuật / thủ thuật) cho *Phiếu chỉ định*.
> Mã này **không** nằm trong 31 mã trên nên chọn xong không có gì xảy ra.
> Mẫu PTTT mà thư viện hỗ trợ là **`Mps000036`**.

Hai lớp `PrintTypeCodeStore` của hai thư viện đều khai báo `internal` nên plugin **không tham chiếu trực tiếp được** —
muốn lọc trước thì phải chép danh sách sang plugin và tự bảo trì khi thư viện bổ sung mã mới.

---

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 07/08/2026 | vuongnd | **Việc 53180 — Bổ sung tra soát theo bác sĩ chỉ định.**<br/>• Tách form 2188 dòng thành 10 file partial theo vai trò.<br/>• Bổ sung `Resources/` đa ngôn ngữ (92 nhãn + 8 thông báo × vi/en), bật `SetCaptionByLanguageKey()`, bỏ toàn bộ chuỗi hardcode.<br/>• Thêm `EnumRecordDocumentStatus` + `CalcDocumentStatus`; **sửa lỗi nghiệp vụ**: trạng thái "Hoàn thành" đổi từ `Exists()` sang `All()` — trước đây chỉ cần 1 văn bản ký xong đã báo hoàn thành.<br/>• Thêm cột *Thời gian tạo* và *Trạng thái VB*; chuyển tính toán ra khỏi `CustomUnboundColumnData`.<br/>• Thêm 3 bộ lọc (bác sĩ chỉ định, khoảng thời gian, trạng thái hồ sơ) + `ApplyModeUI()` + validate; hai cách tra soát dùng chung màn hình.<br/>• Thêm 2 ô lọc nhanh *Chưa tạo văn bản* / *Chưa ký hết* + ControlState.<br/>• Nối API `GetServiceReqForRecordChecking` (`GetRO` + `Inventec.UC.Paging`), lấy văn bản cả trang bằng 1 lần gọi.<br/>• Thêm nút **Tạo văn bản** lấy biểu in từ cấu hình `SAR_PRINT_TYPE`.<br/>• Sửa 7 lỗi có sẵn: lời gọi chết trong `FillDataToGridTreatment`, caption lưới sai lần đầu, thiếu chặn `IsLoadFirstForm` ở `chkUuTien`, truy vấn O(n×m) trong `ProcessDataADO`, `GetSigners` ném lỗi khi khoá không tồn tại, `GetDicEmrSign` rò dữ liệu hồ sơ trước, `controlStateWorker` là `public static`.<br/>• Xóa tham chiếu chết `Properties\licenses.licx` trong csproj (file không tồn tại, chặn build). |
| 08/08/2026 | vuongnd | **Việc 53180 — Khớp dữ liệu API mới với API cũ.**<br/>• Nút *Tạo văn bản* chuyển thành nút theo từng dòng trong lưới; 3 bộ lọc mới đưa lên hàng trên cùng cạnh nút Tìm.<br/>• `ProcessDataADO()` tách 2 overload; thêm `StampTreatmentInfo()` gắn mã hồ sơ / thông tin bệnh nhân cho các dòng vừa sinh.<br/>• Cách 2 bỏ hoàn toàn hàm dựng dữ liệu riêng `MapDoctorOrdersToADO()`, dùng chung `ProcessDataADO()` với Cách 1 — mọi quy tắc `SEARCH_CODE`, `TYPE`, `DEPARTMENT_NAME`, định dạng thời gian và việc nhân đôi dòng Phiếu chỉ định → Phiếu kết quả chỉ còn một chỗ.<br/>• Bám theo SDO backend đã chốt (`HisServiceReqForRecordCheckingSDO`): thêm `SplitByTreatment()` tách dữ liệu phẳng thành từng hồ sơ — 4 bảng nhóm trực tiếp theo `TREATMENT_ID`, 3 bảng `HIS_INFUSION` / `HIS_MEDI_REACT` / `HIS_TRANSFUSION` nối qua bảng `_SUM` (tra cứu O(1)).<br/>• Phân trang đếm theo **hồ sơ**, không phải y lệnh.<br/>• Bỏ `DOCUMENT_TYPE_ID` khỏi filter API (backend đã bỏ) — lọc theo loại văn bản vẫn chạy phía client như Cách 1.<br/>• **Đồng bộ hoàn toàn luồng hiển thị / sắp xếp với Cách 1**: bổ sung guard `CurrentType` + `ProcessCaptionGridInfoRecord()`, reset `CurrentDataInfoRecord`, tách `SyncUndefinedDocumentType()` để Cách 2 cũng thêm/xoá dòng "Chưa xác định", và chuyển `WaitingManager` vào `LoadPagingByDoctor` để mỗi lần đổi trang đều có loading. |
| 15/08/2026 | vuongnd | **Việc 53180 — Sửa lỗi trạng thái "chưa ký" không hiện ở Cách 2; nạp sẵn luồng ký.**<br/>• **Sửa lỗi**: `LoadDocumentsOfTreatments()` đổi từ `filter.TREATMENT_IDs` sang `filter.TREATMENT_CODEs`. `V_EMR_DOCUMENT.TREATMENT_ID` là nullable và bộ lọc `TREATMENT_IDs` của EMR yêu cầu `o.TREATMENT_ID.HasValue`, nên bỏ qua toàn bộ văn bản chưa gán `TREATMENT_ID` — đúng nhóm văn bản mới sinh, **chưa ký**. Cách 1 lọc bằng `TREATMENT_CODE__EXACT` nên không dính; nay hai cách khớp nhau từng dòng.<br/>• Cách 2 **nạp sẵn luồng ký cho cả trang**: gọi `api/EmrSign/GetView` ngay sau khi lấy danh sách văn bản, không phải click từng dòng mới có tên luồng ký / người chưa ký.<br/>• `GetDicEmrSign()` đổi từ *dựng lại* sang *gộp thêm*, bỏ qua văn bản đã hỏi (`loadedEmrSignDocumentIds`) và chia lô 500 tránh vỡ mệnh đề `IN` của Oracle. Gộp an toàn vì khoá là `V_EMR_SIGN.ID`. Thêm `ResetEmrSignCache()` gọi khi bắt đầu lượt tra soát mới ở cả hai cách.<br/>• Backend bỏ điều kiện `SERVICE_REQ_STT_ID <> ID__HT` — lọc bỏ y lệnh đã hoàn thành sẽ giấu mất chính những y lệnh cần tra soát chữ ký. |
| 10/08/2026 | vuongnd | **Sửa lỗi "Ưu tiên" + tối ưu chuẩn bị dữ liệu.**<br/>• Thêm cờ `EmrDocumentTypeADO.IsHasData` (có văn bản **hoặc** có y lệnh) và `OrderListByCheckBox()` đẩy lên theo cờ này — trước đây dùng `IsHasDocument` nên 9 loại trong `ListTypeId` hiển thị y lệnh mà chưa tạo văn bản không bao giờ được đẩy lên đầu. `IsHasDocument` giữ nguyên vì còn dùng tô màu lưới trái.<br/>• **Sửa lỗi mất dữ liệu**: `MediReacts.Select(s => s.MEDICINE_ID.Value)` với `MEDICINE_ID` kiểu `long?` — một bản ghi test thuốc thiếu mã thuốc làm `ProcessDataADO` ném lỗi, mất toàn bộ dòng của hồ sơ đó mà chỉ ghi log.<br/>• Thêm `PrepareLookupData()` + `BuildMedicineLookup()`: dựng sẵn 3 bảng tra cứu (khoa, loại yêu cầu DV, thuốc) **một lần cho cả lần tìm**. Cách 2 với 50 hồ sơ/trang giảm từ tối đa 52 lượt gọi API còn 3, và bỏ 50 lần dựng lại `Dictionary` danh mục.<br/>• Thêm `ProcessDisposeModuleDataAfterClose()` giải phóng các bảng tra cứu khi đóng form. |
| 11/08/2026 | vuongnd | **Sửa lỗi kẹt ở Cách 2 — không quay lại tra soát theo hồ sơ được.**<br/>• Nút xoá trên `cboRequestDoctor` chỉ là glyph của `ButtonPredefines.Delete`, DevExpress **không tự xoá giá trị**. Chưa nối `Properties.ButtonClick` nên bấm không có tác dụng → ô Mã hồ sơ bị khoá vĩnh viễn. Thêm `cboRequestDoctor_ButtonClick` gán `EditValue = null`.<br/>• Thêm `BackToTreatmentMode()` gọi ở nhánh `else` của `ApplyModeUI()`: xoá dữ liệu Cách 2 còn đọng (`ListDataInfoRecord`, `CurrentInfoRecord`, `ListDocument`), làm sạch 3 lưới + panel bệnh nhân, trả con trỏ về ô Mã hồ sơ.<br/>• `LcgPatientInfo.Enabled` bật/tắt theo cách tra soát — vùng thông tin bệnh nhân chỉ có nghĩa khi xem một hồ sơ. |
| 12/08/2026 | vuongnd | **Chuyển nút Tìm ra cuối hàng lọc.**<br/>• Thứ tự hàng trên cùng: Mã hồ sơ → Bác sĩ chỉ định → Từ ngày → Đến ngày → Trạng thái hồ sơ → **[Tìm]**, theo đúng thứ tự thao tác.<br/>• Toạ độ dịch lại toàn hàng, tổng vẫn đúng 1264px (đã kiểm tra không hở, không chồng lấn).<br/>• `BtnSearch.TabIndex` 5 → 44 để Tab chạy hết bộ lọc rồi mới tới nút Tìm. |
| 11/08/2026 | vuongnd | **Ghi nhận giới hạn tạo văn bản (chưa sửa code).**<br/>• Xác định nguyên nhân "chọn biểu mẫu xong không mở được bước tiếp theo": mã Mps được cấu hình ở màn Biểu in nằm ngoài tập mã mà `Library.PrintServiceReq` dựng được → `switch` rơi vào `default: break;`, im lặng.<br/>• Liệt kê 31 mã `PrintServiceReq` và 12 mã `PrintPrescription` hỗ trợ vào mục 7 để đối chiếu khi cấu hình.<br/>• **Không thay đổi mã nguồn** theo yêu cầu — hướng xử lý còn chờ chốt, xem mục 7 và Phần 11 của tài liệu kỹ thuật. |

---

## 9. Test Cases

### Regression — Cách 1 phải y nguyên

- [ ] Nhập mã hồ sơ → Tìm → hiển thị giống hệt bản cũ
- [ ] Chọn từng loại văn bản ở lưới trái → lưới giữa/phải đổi đúng
- [ ] Đạt / Không đạt / Duyệt / Hủy duyệt hoạt động, enable đúng theo quyền và trạng thái
- [ ] 3 ô tích cũ hoạt động như trước
- [ ] Mở từ plugin khác có truyền `List<long>` → lưới hồ sơ hiện như cũ
- [ ] Đóng, mở lại → trạng thái checkbox được nhớ

### Lọc theo bác sĩ

- [ ] Chọn bác sĩ, bỏ trống Từ ngày → báo lỗi tại ô, không gọi API
- [ ] Bỏ trống Trạng thái hồ sơ → báo lỗi tại ô
- [ ] Từ ngày > Đến ngày → báo lỗi
- [ ] Khoảng > 31 ngày → hỏi xác nhận; chọn Không thì dừng
- [ ] Lọc đủ điều kiện → hiện y lệnh của bác sĩ, có cột Mã BN / Tên BN / Mã hồ sơ
- [ ] Ô mã hồ sơ bị khoá; 4 nút chốt hồ sơ bị khoá
- [ ] Trạng thái *Đã kết thúc* → chỉ hồ sơ có thời gian kết thúc
- [ ] Xóa trắng ô bác sĩ → về Cách 1 đầy đủ
- [ ] Nháy đúp một dòng → mở hồ sơ đó ở Cách 1

### Phân trang — CHƯA HOẠT ĐỘNG, bỏ qua khi test

Backend không đọc `Start`/`Limit`, không gán `Param.Count`. Các mục dưới **sẽ trượt**, đây là nợ kỹ
thuật đã biết chứ không phải lỗi mới:

- [ ] ~~Kết quả nhiều trang → thanh phân trang hiện, chuyển trang được~~
- [ ] ~~Chuyển trang không mất điều kiện lọc~~
- [ ] ~~Đổi loại văn bản ở lưới trái → phân trang tính lại từ trang 1~~
- [x] Cách 1 → thanh phân trang bị ẩn

### Trạng thái y lệnh

- [ ] Y lệnh không có văn bản → *Chưa tạo văn bản*, icon đen
- [ ] Có văn bản, chưa ai ký → *Chưa ký*
- [ ] **2 văn bản: 1 ký xong, 1 chưa ký → *Đang ký*** (khác bản cũ)
- [ ] Tất cả đã ký đủ → *Hoàn thành*, icon xanh
- [ ] **Cách 2 phải ra đủ 4 trạng thái giống Cách 1** — nhất là *Chưa ký*. Trước 15/08 nhóm này
      biến mất ở Cách 2 do lọc văn bản bằng `TREATMENT_IDs`
- [ ] Tìm theo bác sĩ xong, **chưa click dòng nào** → cột người ký / người chưa ký đã có tên luồng ký
- [ ] Tích *Chưa tạo văn bản* → chỉ còn y lệnh chưa có văn bản
- [ ] Tích *Chưa ký hết* → còn tất cả trừ *Hoàn thành*
- [ ] Tích cả hai → chỉ còn y lệnh chưa có văn bản

### Cột thời gian

- [ ] Phiếu chỉ định: cột thời gian = giờ y lệnh; Thời gian tạo = giờ tạo bản ghi
- [ ] Biên bản hội chẩn: hai cột có thể khác nhau
- [ ] Lọc theo khoảng ngày → mọi dòng nằm trong khoảng

### Tạo văn bản

- [ ] Y lệnh đã có văn bản → nút mờ
- [ ] Không chọn dòng → nút mờ
- [ ] Loại văn bản chưa cấu hình biểu in → thông báo, không lỗi
- [ ] Loại có đúng 1 biểu in → vào thẳng, không hỏi
- [ ] Loại có nhiều biểu in → hiện danh sách chọn
- [ ] Loại chưa hỗ trợ (Đơn thuốc, Tờ điều trị…) → thông báo, không lỗi
- [ ] Tạo + ký thành công → lưới làm mới, trạng thái đổi
- [ ] Hủy giữa chừng → không sinh văn bản, trạng thái giữ nguyên

### Đa ngôn ngữ

- [ ] Chuyển sang tiếng Anh → toàn bộ nhãn và thông báo hiển thị tiếng Anh
- [ ] Chuyển về tiếng Việt → hiển thị đúng dấu

### Hiệu năng

- [ ] Lọc 1 bác sĩ, 1 tháng → có biểu tượng chờ, trả kết quả dưới 5 giây
- [ ] Cuộn lưới nhiều dòng → không giật, không gọi lại API
- [ ] Đổi loại văn bản liên tục → không treo
