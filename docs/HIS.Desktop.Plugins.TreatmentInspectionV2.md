# Giám định bảo hiểm y tế v2 — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TreatmentInspectionV2 |
| Loại | UC |
| Mục đích | Tiếp nhận danh sách hồ sơ mà bên giám định BHYT yêu cầu giám định (nhập khẩu từ Excel theo mã điều trị), rồi giám định trên đúng danh sách đó. Kế thừa toàn bộ nghiệp vụ của màn Giám định hồ sơ bệnh án và bổ sung nhập khẩu + lọc theo ngày nhập khẩu. |
| Người tạo | huannh |
| Ngày tạo | 11/08/2026 |
| Trạng thái | Đang phát triển — chờ phần Backend |
| Tài liệu thiết kế | `_Software-Specs/02_Analysis_Design/PTTK_XXXXX_Giam_Dinh_BHYT_V2_Import_Ho_So_Giam_Dinh.md` (v1.1) |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

1. Bên giám định BHYT gửi danh sách mã điều trị cần giám định.
2. Cán bộ mở chức năng, bấm **Tải file mẫu**, điền danh sách mã điều trị vào file Excel.
3. Bấm **Nhập khẩu danh sách** → chọn file → hệ thống kiểm tra từng dòng, tra thông tin hồ sơ để đối chiếu, đánh dấu dòng lỗi.
4. Hết dòng lỗi → **Lưu** → mỗi dòng thành một bản ghi tiếp nhận giám định kèm thời điểm nhập khẩu.
5. Danh sách trên màn hình làm mới, đợt vừa nhập nằm trên cùng.
6. Cán bộ giám định từng hồ sơ: xem chi tiết bệnh án, Duyệt giám định / Từ chối duyệt kèm lý do, Hủy lưu tủ bệnh án.
7. Cần xem lại một đợt cũ → điền khoảng **Ngày nhập khẩu** → Tìm.

### Điều kiện nghiệp vụ

- Danh sách **luôn** giới hạn trong các hồ sơ đã nhập khẩu. Hồ sơ chưa từng nhập khẩu không xuất hiện dù thỏa mọi điều kiện lọc khác.
- Mở màn hình không điền gì → ra toàn bộ hồ sơ đã nhập khẩu (có phân trang). Không kế thừa cảnh báo chặn tìm kiếm khi bỏ trống điều kiện thời gian của màn V1, và không kế thừa hai mặc định Ngày ra viện = hôm nay / Trạng thái = Đã lưu bệnh án chưa duyệt.
- Cho phép cùng một mã điều trị nhập khẩu lại ở đợt sau — mỗi đợt một dòng. Dòng đó hiện **cảnh báo** kèm ngày đã tiếp nhận trước đó nhưng **vẫn lưu được** (nút Lưu không bị khóa), vì bên giám định BHYT gửi lại hồ sơ để giám định lại là việc bình thường. Chỉ chặn trùng trong cùng một file.
- Chỉ lưu khi danh sách có dữ liệu và không còn dòng lỗi. Người dùng sửa lỗi trên file Excel rồi nạp lại, không sửa trên giao diện.
- Nhập khẩu chỉ ghi nhận danh sách tiếp nhận — không đổi trạng thái giám định, không đổi dữ liệu hồ sơ điều trị.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TREATMENT_11 | View | Danh sách hồ sơ giám định và tra cứu hồ sơ khi kiểm tra file nhập khẩu |
| HIS_TREATMENT | Table | Nhận kết quả của các hành động duyệt / từ chối / hủy lưu tủ bệnh án |
| HIS_DEPARTMENT | Table | Danh mục khoa cho bộ lọc và cột Khoa kết thúc |
| HIS_RECORD_INSPECTION_IMP | Table | **Bảng mới** — danh sách hồ sơ tiếp nhận giám định kèm thời điểm nhập khẩu |

### ADO cầu nối tạm thời

`MOS.Filter` và `MOS.EFMODEL` là assembly biên dịch sẵn trong `lib/`, nên hai lớp dưới đây khai báo phía plugin và **phải xóa** khi bản Backend phát hành:

| Lớp | Vai trò | Thay bằng |
|-----|---------|-----------|
| `ADO/HisTreatmentView11FilterV2.cs` | Bổ sung `HAS_RECORD_INSPECTION_IMP`, `IMPORT_TIME_FROM`, `IMPORT_TIME_TO`, `TREATMENT_CODEs` | `MOS.Filter.HisTreatmentView11Filter` sau khi thêm 4 tham số |
| `ADO/RecordInspectionImpADO.cs` | Entity + filter của bảng mới | `MOS.EFMODEL.DataModels.HIS_RECORD_INSPECTION_IMP` và filter tương ứng |

Các tham số này được serialize cùng thành viên kế thừa, nên Backend bind được ngay khi khai báo cùng tên; trước đó Backend bỏ qua và danh sách hoạt động như màn V1.

### ADO nghiệp vụ

| Lớp | Vai trò |
|-----|---------|
| `ADO/RecordInspectionImportADO.cs` | Một dòng đọc từ file Excel: mã điều trị + thông tin hồ sơ tra được + danh sách lỗi + trạng thái |
| `ADO/TrangThaiADO.cs` | 5 mức trạng thái giám định cho combo lọc (kế thừa V1) |

## 4. UI Layout

```
+-- BỘ LỌC ------------+----------------------------------------------------------+
| Mã điều trị          | [Tải file mẫu][Nhập khẩu danh sách] [Hủy lưu tủ bệnh án] |
| Mã lưu trữ           |                            [Duyệt giám định (Ctrl A)]    |
| Mã bệnh nhân         +----------------------------------------------------------+
| Từ khóa tìm kiếm     | STT | Ngày nhập khẩu | Mã lưu trữ | Mã điều trị | ...     |
| Ngày lưu trữ:  __ __ |                                                          |
| Ngày vào viện: __ __ |                                                          |
| Ngày ra viện:  __ __ |                                                          |
| Ngày nhập khẩu:__ __ |  <-- MỚI                                                 |
| Khoa điều trị:  ___  |                                                          |
| Khoa kết thúc:  ___  |                                                          |
| Trạng thái:     ___  |                                                          |
|      [Tìm (Ctrl F)]  |                                                          |
+----------------------+----------------------------------------------------------+
                       | ucPaging1                                                |
                       +----------------------------------------------------------+
```

### Thay đổi giao diện so với V1

| Thành phần | Thay đổi |
|------------|----------|
| `dtImportTimeFrom` / `dtImportTimeTo` + `layoutControlItem21` / `22` | Thêm — ô lọc Ngày nhập khẩu, đặt dưới Ngày ra viện |
| `btnDownloadTemplate` / `btnImportList` + `layoutControlItem23` / `24` | Thêm — 2 nút trên thanh thao tác phía trên danh sách |
| `gcImportTime` | Thêm — cột Ngày nhập khẩu, unbound, `VisibleIndex = 2` |
| Vị trí `btnUnSave` / `btnDuyetGiamDinh` | Dịch sang phải nhường chỗ cho 2 nút mới |

### Cửa sổ nhập khẩu — `Import/frmImportRecordInspection.cs`

```
+----------------------------------------------------------------------+
| [Tải file mẫu][Chọn file Excel][Chỉ dòng lỗi][Lưu (Ctrl S)]          |
+----------------------------------------------------------------------+
| STT | ! | X | Mã điều trị | Mã BN | Tên BN | TG vào | TG ra | Khoa... |
+----------------------------------------------------------------------+
| Tổng số dòng: n  |  Hợp lệ: n  |  Lỗi: n  |  File: <tên file>        |
+----------------------------------------------------------------------+
```

- Dòng lỗi hiển thị chữ đỏ và có **icon lỗi** ở cột thứ 2. **Trỏ chuột vào icon → tooltip liệt kê đủ nguyên nhân**; bấm vào icon thì mở popup cùng nội dung, tiện khi một dòng có nhiều lỗi.

**Cột icon lỗi — theo đúng pattern `HIS.Desktop.Plugins.HisImportBid`:**

| Thành phần | Cấu hình |
|---|---|
| Editor | `RepositoryItemButtonEdit` + 1 `EditorButton` kiểu `Glyph`, `TextEditStyle = HideTextEditor`, `ImageLocation = MiddleCenter` |
| Gán editor | Theo **từng dòng** qua `CustomRowCellEdit` — chỉ gán khi dòng có lỗi, nên dòng hợp lệ để trống hẳn |
| Grid | `OptionsView.ShowButtonMode = ShowAlways` để nút hiện ở mọi dòng, và **không** set `OptionsBehavior.Editable = false` (set sẽ làm nút trong ô không bấm được) |
| Ảnh lỗi | `Resources/row_error.png` — trích từ `FormImportBid.resx` (key `RepositoryItemButtonError.Buttons`, 16x16 ARGB) để dùng đúng icon của các màn khác. Nhúng dạng `EmbeddedResource`, nạp lúc `Load` bằng `GetManifestResourceStream` |
| Ảnh cảnh báo | `btnGWarning` — icon `images/support/info_16x16.png` từ gallery DevExpress. DevExpress 15.2 không có glyph `warning`; icon info khác hẳn icon lỗi nên phân biệt được ngay dòng chặn lưu và dòng chỉ nhắc |
| Ưu tiên | Dòng vừa lỗi vừa cảnh báo → hiện icon **lỗi**; lỗi là thứ chặn lưu nên quan trọng hơn |
| Màu chữ dòng | Lỗi → đỏ · Cảnh báo → cam · Hợp lệ → mặc định |
| Tooltip nút | Lấy từ `Lang.*.resx` key `frmImportRecordInspection.btnGError.ToolTip` |
| Tooltip nội dung lỗi | `ToolTipController.GetActiveObjectInfo` |

**Chống nhấp nháy tooltip**: controller nổ event ở **mỗi lần chuột di chuyển**; tạo `ToolTipControlInfo` mới mỗi lần làm tooltip đóng-mở liên tục, người dùng thấy nhấp nháy. Form cache `lastRowHandle` / `lastColumn` / `lastInfo` và chỉ dựng lại khi con trỏ sang ô khác — cùng cách màn Giám định (V1) xử lý cột trạng thái.
- Nút `X` bỏ dòng khỏi danh sách; sau khi bỏ, cả lô được kiểm tra lại để gỡ cờ trùng trên dòng còn giữ mã.
- Nút **Lưu** chỉ mở khi có dữ liệu và không còn dòng lỗi.

### File Excel mẫu — `Tmp/Imp/IMPORT_RECORD_INSPECTION.xlsx`

| Hạng mục | Giá trị |
|----------|---------|
| Nguồn trong repo | `HIS.Desktop.Plugins.TreatmentInspectionV2/Tmp/Imp/IMPORT_RECORD_INSPECTION.xlsx` (csproj `Content` + `CopyToOutputDirectory`) |
| Vị trí runtime | `{ApplicationStartupPath}\Tmp\Imp\IMPORT_RECORD_INSPECTION.xlsx` |
| Tên sheet | `DanhSachGiamDinh` (sheet index 0) |

Cấu trúc — theo đúng convention của `Inventec.Common.ExcelImport`:

| Dòng | A | B |
|------|---|---|
| 1 | `Mã điều trị` (caption, in đậm) | `Ghi chú` (caption, in đậm) |
| 2 | `{%IMPORT%}.{TREATMENT_CODE}` | `{%IMPORT%}.{NOTE}` |
| 3+ | dữ liệu người dùng điền | *(tùy chọn)* |

**Vì sao có cột Ghi chú dù nghiệp vụ chỉ cần mã điều trị**: `Import.CheckIndex()` trả `false` khi file có **≤ 1 cột tag**, khiến `GetWithCheck` trả danh sách rỗng. Thư viện bắt buộc tối thiểu 2 cột tag cùng một dòng. Cột `NOTE` vì vậy vừa thỏa ràng buộc kỹ thuật vừa có ích khi đối chiếu — nhưng **chỉ hiển thị, không lưu** (bảng `HIS_RECORD_INSPECTION_IMP` không có cột ghi chú). Nếu viện muốn lưu, Backend bổ sung cột `NOTE` và sửa `RecordInspectionImpADO`.

**Ghi chú kỹ thuật**: cột A định dạng text (`numFmtId="49"`) để mã điều trị có số 0 ở đầu không bị Excel cắt. Người dùng vẫn có thể điền mã ngắn — plugin tự pad về 12 ký tự.

**Đã kiểm chứng** bằng chính đường dẫn code của plugin (`SpreadsheetControl.LoadDocument` → `GetCellValue`, DevExpress 15.2): `Cells[0,0].RowHeight = 62.5 > 0`, quét được đúng 2 tag ở cùng dòng index 1 với key `TREATMENT_CODE` / `NOTE`, `CheckIndex()` = true, dữ liệu đọc từ dòng index 2 trở đi đúng giá trị (kể cả mã giữ nguyên số 0 đầu).

### Chuẩn hóa chiều cao dòng trước khi đọc — `NormalizeRowHeight()`

`Inventec.Common.ExcelImport.Import.GetWithCheck` có chốt `RowHeightUnit > 0` với `RowHeightUnit = workSheet.Cells[0,0].RowHeight` ([Import.cs:195](../Common/Inventec.Common/Inventec.Common.ExcelImport/Import.cs#L195)). **File do WPS Office lưu không ghi chiều cao dòng → DevExpress trả `RowHeight = 0` → thư viện bỏ qua toàn bộ sheet, chưa kịp quét tag lần nào**, và trả danh sách rỗng mà không ném exception.

Đo trên file thật của tester (9.585 bytes, WPS lưu, có 2 dòng dữ liệu):

| Chỉ số | File WPS gốc | Sau `NormalizeRowHeight()` |
|---|---|---|
| `ReadFileExcel` | True | True |
| `Cells[0,0].RowHeight` | **0** ← trượt | 20 |
| `Rows.LastUsedIndex` | 3 | 3 |
| `tag count` | **0** | 2 |
| `rows` | **0** | **2** — đúng dữ liệu |

Vì vậy `btnChooseFile_Click` không đọc file người dùng chọn trực tiếp: nó gọi `NormalizeRowHeight()` để nạp file bằng `SpreadsheetControl`, gán chiều cao cho mọi dòng đã dùng của mọi sheet, lưu ra **file tạm trong `%TEMP%`**, đọc file tạm rồi xóa. File người dùng chọn **không bị sửa**. Nếu chuẩn hóa thất bại thì trả về đường dẫn gốc — không bao giờ chặn một lần nhập khẩu vốn đã chạy được.

Nhờ vậy plugin nhập được file lưu từ WPS Office, Google Sheets, LibreOffice — không chỉ Microsoft Excel.

### UC sử dụng

| UC | Panel | Mục đích |
|----|-------|----------|
| Inventec.UC.Paging | `ucPaging1` | Phân trang danh sách hồ sơ (kế thừa V1) |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy danh sách hồ sơ giám định | `HisRequestUriStore.HIS_TREATMENT_GETVIEW11` | MosConsumer | `HisTreatmentView11FilterV2` |
| Tra cứu hồ sơ theo lô mã điều trị | `HisRequestUriStore.HIS_TREATMENT_GETVIEW11` | MosConsumer | `HisTreatmentView11FilterV2.TREATMENT_CODEs` |
| Lấy thời điểm nhập khẩu của trang hiện tại | `HisRequestUriStore.HIS_RECORD_INSPECTION_IMP_GET` | MosConsumer | `RecordInspectionImpFilter` |
| Ghi nhận danh sách tiếp nhận | `HisRequestUriStore.HIS_RECORD_INSPECTION_IMP_CREATE_LIST` | MosConsumer | — |
| Hủy lưu tủ bệnh án | `HisRequestUriStore.HIS_TREATMENT_OUT_OF_MEDI_RECORD_LIST` | MosConsumer | — |
| Duyệt giám định | `HisRequestUriStore.HIS_TREATMENT_RECORD_INSPECTION_APPROVE` | MosConsumer | — |
| Hủy duyệt giám định | `HisRequestUriStore.HIS_TREATMENT_RECORD_INSPECTION_UN_APPROVE` | MosConsumer | — |
| Hủy từ chối duyệt | `HisRequestUriStore.HIS_TREATMENT_RECORD_INSPECTION_UN_REJECT` | MosConsumer | — |
| Đếm số lần xem bệnh án | `api/HisTreatment/DocumentViewCount` | MosConsumer | — |

> Hai endpoint `HisRequestUriStore.HIS_RECORD_INSPECTION_IMP_*` **chưa tồn tại** ở Backend tại thời điểm tạo module.

## 6. Dependencies

### Thư viện

| Thư viện | Mục đích |
|----------|----------|
| `Inventec.Common.ExcelImport` | Đọc file Excel nhập khẩu (`ReadFileExcel` + `GetWithCheck<T>`) |

### Inter-Plugin

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| `HIS.Desktop.Plugins.EmrDocument` | Bấm xem chi tiết bệnh án trên một dòng | `string` mã điều trị, `bool` false, `RefeshReference` callback làm mới danh sách |

## 7. Print

Không có chức năng in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 11/08/2026 | huannh | Tạo mới plugin từ bản clone `HIS.Desktop.Plugins.TreatmentInspection`. Bổ sung: bộ lọc Ngày nhập khẩu, cột Ngày nhập khẩu, 2 nút Tải file mẫu / Nhập khẩu danh sách, cửa sổ nhập khẩu Excel với 4 quy tắc kiểm tra, 2 lớp ADO cầu nối chờ Backend. Bỏ 2 giá trị mặc định của bộ lọc và cảnh báo chặn tìm kiếm khi bỏ trống điều kiện thời gian. |

## 9. Test Cases

### Nhập khẩu

- [ ] Tải file mẫu → file tải về đúng tên `IMPORT_RECORD_INSPECTION.xlsx`, hỏi mở ngay
- [ ] Máy trạm chưa có file mẫu → thông báo rõ, không crash
- [ ] File đúng mẫu, mọi dòng hợp lệ → nút Lưu mở, lưu thành công, danh sách làm mới
- [ ] File không đọc được → thông báo, không ghi nhận gì
- [ ] File rỗng → thông báo, danh sách trống
- [ ] File vượt 5.000 dòng → thông báo chia nhỏ file, không nạp
- [ ] Dòng thiếu mã điều trị → báo lỗi, nút Lưu khóa
- [ ] Mã điều trị không tồn tại → báo lỗi kèm mã
- [ ] Mã điều trị thiếu số 0 ở đầu → tự chuẩn hóa 12 ký tự, không báo lỗi
- [ ] Hai dòng cùng mã trong một file → dòng sau báo trùng kèm số dòng trước
- [ ] Bấm nút `...` trên dòng lỗi → popup liệt kê đủ lỗi
- [ ] Bỏ một dòng bằng nút `X` → cờ trùng trên dòng còn lại được gỡ, STT đánh lại
- [ ] Toggle Chỉ dòng lỗi / Tất cả các dòng → danh sách đổi đúng
- [ ] Ctrl+S khi còn dòng lỗi → không lưu

### Danh sách

- [ ] Mở màn hình không điền gì → ra hồ sơ đã nhập khẩu, KHÔNG hiện cảnh báo đòi điền điều kiện thời gian
- [ ] Chưa nhập khẩu đợt nào → danh sách rỗng
- [ ] Hồ sơ chưa từng nhập khẩu → không xuất hiện dù thỏa điều kiện lọc khác
- [ ] Điền Ngày nhập khẩu từ–đến → chỉ còn hồ sơ thuộc đợt trong khoảng
- [ ] Hồ sơ thuộc nhiều đợt trong khoảng lọc → chỉ 1 dòng, cột Ngày nhập khẩu lấy thời điểm gần nhất
- [ ] Bộ lọc Ngày ra viện và Trạng thái để trống khi mở màn hình

### Nghiệp vụ kế thừa

- [ ] Duyệt giám định hàng loạt các dòng đang chọn
- [ ] Hủy duyệt, Từ chối duyệt kèm lý do, Hủy từ chối
- [ ] Hủy lưu tủ bệnh án — chặn khi hồ sơ chưa lưu tủ / đã duyệt giám định
- [ ] Xem chi tiết bệnh án → mở bệnh án điện tử, số lần xem tăng 1

### Phân quyền

- [ ] Tài khoản chưa được gán module → không thấy chức năng trên menu
- [ ] Màn Giám định hồ sơ bệnh án (4121) hoạt động không thay đổi
