# Thiết kế: Đẩy file đính kèm hồ sơ điều trị sang EMR + Scan trực tiếp từ máy scan

| | |
|---|---|
| Ticket | 52909 - IVT - [CODE] Sửa chức năng HIS.Desktop.Plugins.HisTreatmentFile |
| Plugin | `HIS.Desktop.Plugins.HisTreatmentFile` (form `frmTreatmentFile`) |
| Commit gốc | `a1ca2582f` (khainq, 2026-07-28) + phần bổ sung `HIS_CODE` / xóa văn bản EMR |
| Tài liệu này | Viết ngược từ code đang có. Dùng chung cho DEV (biết code đang làm gì, sửa ở đâu) và TEST (biết phải kiểm cái gì, ở đâu ra kết quả) |

---

## 1. Tóm tắt

Trước đây màn "File đính kèm hồ sơ điều trị" chỉ làm 1 việc: gom các file người dùng chọn (hoặc ảnh chụp từ camera), upload lên FSS rồi lưu 1 dòng `HIS_TREATMENT_FILE` với cột `FILE_URLS` là danh sách url nối bằng `|`.

Thay đổi này thêm 2 việc:

1. **Thêm nguồn ảnh thứ 3: máy scan** (WIA), quét 1 mặt hoặc quét nhiều trang 2 mặt qua khay nạp tự động.
2. **Song song với `HIS_TREATMENT_FILE`, đẩy thêm 1 bản sang EMR dưới dạng "văn bản"**: gộp toàn bộ file đính kèm của lần lưu đó thành 1 file PDF rồi gọi API EMR tạo văn bản. Khi xóa dòng `HIS_TREATMENT_FILE` thì xóa luôn văn bản EMR tương ứng.

Nguyên tắc xuyên suốt: **EMR là phần phụ, không được chặn HIS.** `HIS_TREATMENT_FILE` lưu/xóa xong rồi mới xử lý EMR; EMR lỗi thì chỉ log + báo, không rollback bên HIS.

---

## 2. Mục tiêu / Ngoài phạm vi

**Trong phạm vi**

- Quét ảnh từ máy scan vào cùng danh sách đính kèm với ảnh chọn từ máy và ảnh chụp camera.
- Nhập thêm 3 thông tin văn bản EMR trên form: Loại văn bản, Tên văn bản, Nhóm văn bản.
- Tạo văn bản EMR khi bấm **Lưu** (`btnAdd`).
- Xóa văn bản EMR khi xóa dòng ở danh sách đã lưu (`btnGDelete`).
- Chức năng chỉ hoạt động khi hệ thống có kết nối EMR.

**Ngoài phạm vi (đang KHÔNG làm — xem mục 11)**

- Bấm **Sửa** (`btnEdit`) không cập nhật lại văn bản EMR.
- Xóa lẻ 1 ảnh trong khung xem trước (nút X trên tile) không cập nhật văn bản EMR.
- Không ký số, không gửi trạng thái sang hệ thống tích hợp.

---

## 3. Bản đồ thành phần

| File | Vai trò |
|---|---|
| `frmTreatmentFile.cs` | Toàn bộ logic. Phần mới nằm ở 2 nhóm hàm: nhóm EMR (`CreateEmrDocument`, `BuildEmrHisCode`, `GetEmrTreatmentCode`, `GetEmrDocumentsOfTreatmentFile`, `DeleteEmrDocument`, `CombineMultiplePDFs`, `AppendPdfToWriter`, `ConvertImageToTempPdf`, `DownloadFssToTempFile`) và nhóm Scan (`ShowScan`, `Scan`, `ScanDuplex`, `AdjustScannerSettings`, `SetWIAProperty`, `FillImageScanToCardControl`) |
| `frmTreatmentFile.Designer.cs` | 5 control mới: `cboDocumentType`, `txtDocumentName`, `CboDocumentGroup` (khối nhập thông tin), `btnScan` + `chkPrintDupicate` (khối thanh công cụ ảnh) |
| `Config/ConfigKey.cs` | **Mới.** Đọc 1 khóa cấu hình HIS: `MOS.HAS_CONNECTION_EMR` → `ConfigKey.IsHasConnectionEmr` |
| `Base/StreamToPdfADO.cs` | **Mới.** DTO trung gian trả về từ máy scan: `Url` = đường dẫn file tạm ảnh vừa quét |
| `Base/AttackADO.cs` | Không đổi, nhưng ý nghĩa các cột được dùng thêm: `IsFss` phân biệt file đã ở FSS hay còn ở máy trạm, `image` là ảnh để hiển thị và để gộp PDF |
| `HIS.Desktop.Plugins.HisTreatmentFile.csproj` | Thêm reference `EMR.URI`, project reference `HIS.Desktop.LocalStorage.EmrConfig`, **COMReference `WIA` (EmbedInteropTypes)** |

Nguồn port: phần scan và phần tạo văn bản được port từ `HIS.Desktop.Plugins.EmrDocument\frmAttackFile.cs` (nút "Đính kèm" bên màn văn bản EMR). Nhiều tên biến/hằng giữ nguyên để đối chiếu 2 bên: `formatJpeg`, `AdjustScannerSettings(...,150,0,0,1250,1754,0,0,1)`, `chkPrintDupicate`.

---

## 4. Điều kiện bật chức năng

Toàn bộ phần EMR bị bao bởi 1 cổng duy nhất:

```
ConfigKey.GetConfigKey()   // gọi 1 lần ở frmTreatmentFile_Load
IsHasConnectionEmr = (HisConfigs.Get<string>("MOS.HAS_CONNECTION_EMR") == "1")
```

Bốn hàm kiểm tra cổng này và **return sớm**: `InitComboDocumentType`, `LoadCboTextGroup`, `CreateEmrDocument`, `DeleteEmrDocument`.

Hệ quả cho TEST:

- `MOS.HAS_CONNECTION_EMR = 0` (hoặc không có key): 3 combo/textbox văn bản vẫn hiện trên form nhưng combo **rỗng không có dữ liệu**; lưu/xóa chỉ tác động `HIS_TREATMENT_FILE`; log ghi `He thong khong ket noi EMR, bo qua ...`.
- `= 1`: chạy đủ luồng EMR.
- Nút **Scan** KHÔNG phụ thuộc cấu hình này — scan là chức năng của HIS, luôn dùng được.

---

## 5. Luồng nghiệp vụ

### 5.1 Mở form — `frmTreatmentFile_Load`

```
ConfigKey.GetConfigKey()      → đọc MOS.HAS_CONNECTION_EMR
InitControlForm()             → nạp combo Loại file (HIS_FILE_TYPE)      [cũ]
InitComboDocumentType()       → nạp combo Loại văn bản (EMR_DOCUMENT_TYPE)   [mới]
LoadCboTextGroup()            → nạp combo Nhóm văn bản (EMR_DOCUMENT_GROUP)  [mới]
LoadTreatment()               → lấy HIS_TREATMENT theo _TreatmentId → currentTreatment
FillDataFormList()            → nạp lưới danh sách file đã lưu (phân trang)
ValidateForm()                → chỉ đặt rule bắt buộc cho cboFileType
```

Dữ liệu 2 combo mới:

| Combo | API | Filter | Hiển thị |
|---|---|---|---|
| `cboDocumentType` | `api/EmrDocumentType/Get` (EmrConsumer) | `IS_ACTIVE=1`, sắp xếp `DOCUMENT_TYPE_CODE ASC` | code (80) + name (150), popup 230 |
| `CboDocumentGroup` | `api/EmrDocumentGroup/Get` (EmrConsumer) | `IS_ACTIVE=1`, **`IS_LEAF=true`** (chỉ nhóm lá) | code (100) + name (250), popup 350 |

Điều hướng bàn phím (thiết kế cho nhập nhanh, không dùng chuột):

```
cboDocumentType: gõ ký tự → tự mở popup; Enter → đóng popup, nhảy sang txtDocumentName (SelectAll)
txtDocumentName: Enter → nhảy sang CboDocumentGroup và mở popup
cả 2 combo: nút xóa (Buttons[1]) chỉ hiện khi đã có giá trị
```

### 5.2 Ba nguồn nạp file đính kèm

Cả 3 nguồn đều đổ vào **một** danh sách `ListfileNameAttack` và **upload FSS ngay tại thời điểm nạp** (không đợi bấm Lưu). Điểm khác biệt phải nắm khi test:

| | Chọn từ máy (`btnAttach`) | Chụp camera (`btnOpenCamera`) | **Scan (`btnScan`) — mới** |
|---|---|---|---|
| Hàm | `ProcessImage` | `FillImageFromModuleCamereToUC` | `ShowScan` → `FillImageScanToCardControl` |
| Định dạng nhận | jpg/png/jpeg/bmp/gif/**pdf** | jpg | máy scan trả bmp hoặc jpeg → **luôn chuyển về jpg** |
| Tên file | `{now}_{tên gốc}{ext}` | `{now}_{dem}.jpg` | `{now}_Scan_{dem}.jpg` |
| `IsFss` | `false` | `true` | `true` |
| `image` | ảnh thật; nếu là PDF thì gán ảnh placeholder `Img\ImageStorage\notImage.jpg` | ảnh chụp | ảnh vừa quét |
| Thư mục FSS | `{TREATMENT_CODE}\ATTACHMENT_FILE` | như trên | như trên |
| Xem trước | ảnh/pdf theo item click | ảnh vừa chụp | **trang đầu tiên của lần quét** |

Ghi chú thiết kế: `Dem` là số thứ tự tăng dần trong phiên, lấy `max(Dem)` hiện có rồi +1 cho từng trang → quét 2 lần liên tiếp không đè tên nhau.

### 5.3 Scan — chi tiết

```
btnScan_Click
└─ ShowScan()
   ├─ WIA.DeviceManager: không có thiết bị nào     → "Vui lòng kết nối đến máy Scan với máy tính"  → dừng
   ├─ không có thiết bị nào Type = ScannerDeviceType → "Không tìm thấy máy Scan được kết nối..."   → dừng
   ├─ device = firstScannerAvailable.Connect()
   ├─ chkPrintDupicate.Checked ? ScanDuplex(device) : Scan(device)
   └─ streams rỗng → dừng lặng; ngược lại → FillImageScanToCardControl(streams)
```

**`Scan` (1 mặt, quét trên mặt kính)**: `AdjustScannerSettings(item, 150dpi, 0,0, 1250x1754, 0,0, colorMode=1)` (khổ A4 ở 150 DPI) rồi `dlg.ShowTransfer(item, formatJpeg, false)`, lưu ra 1 file tạm. Trả về list 0 hoặc 1 phần tử.

**`ScanDuplex` (nhiều trang, 2 mặt, khay nạp tự động)**:

```
device.Properties["3088"] = 5     // WIA_DPS_DOCUMENT_HANDLING_SELECT = FEEDER(1) | DUPLEX(4)
while (true) { ShowTransfer(items) → lưu file tạm → thêm vào list; lỗi → break }
```

Vòng lặp **kết thúc bằng exception**: hết giấy trong khay thì WIA ném lỗi, `catch { break }` coi như kết thúc phiên quét. Đây là hành vi cố ý, không phải bug — nên log ở mức này không ghi Error.

Máy không hỗ trợ 2 mặt → `HRESULT: 0x80210067` → hiện "Máy scan có thể không hỗ trợ in 2 mặt, vui lòng kiểm tra lại." và trả `null`.

`AdjustScannerSettings` có **2 tầng**: đặt thông số mong muốn; nếu máy từ chối (exception) thì log lại giá trị máy đang có rồi đặt lại theo DPI thật của máy, kích thước quy đổi `+50 px`. Test với máy scan lạ nên đọc log dòng `Gắn lại giá trị theo máy scan:` để biết đã rơi vào tầng 2.

**`FillImageScanToCardControl`** (đưa ảnh quét vào form + FSS):

```
currentTreatment == null  → log Error, dừng (không xác định được thư mục lưu)
với từng file tạm:
   đọc ảnh → Bitmap → save MemoryStream dạng Jpeg → byte[]      (chuẩn hóa về jpg)
   xóa file tạm
   tạo AttackADO { FILE_NAME=FullName={now}_Scan_{dem}.jpg, Extension=jpg,
                   image=..., IsFss=true, IsChecked=true, Url={TREATMENT_CODE}\ATTACHMENT_FILE\{tên} }
   thêm FileHolder(byte[], tên) vào danh sách upload
FileUpload.UploadFile(APPLICATION_CODE, url, files, true)
   thành công → khớp OriginalName để thay Url tạm bằng Url thật của FSS
   thất bại   → "Upload file thất bại, vui lòng liên hệ quản trị hệ thống để được hỗ trợ."
ListfileNameAttack.AddRange(...) → xem trước trang đầu → FilldataToTittleView(...)
```

Lưu ý: danh sách vẫn được `AddRange` **kể cả khi upload thất bại** (giữ nguyên hành vi của luồng camera cũ). Test cần biết: upload lỗi mà vẫn bấm Lưu thì `FILE_URLS` sẽ chứa đường dẫn tương đối chưa hợp lệ.

### 5.4 Lưu — `btnAdd_Click`

```
lstData = cardControl.DataSource as List<AttackADO>
rỗng → "Vui lòng chọn file ảnh từ máy tính hoặc chụp ảnh từ camera."
dxValidationProvider.Validate() thất bại (thiếu Loại file) → dừng
POST api/HisTreatmentFile/Create  { TREATMENT_ID, FILE_TYPE_ID, DESCRIPTION, FILE_URLS = url join '|' }
   → resultData != null:
        CreateEmrDocument(lstData, resultData)     ← phần mới
        FillDataFormList()
        SetDefaultValue()                          ← đã bổ sung clear 3 control văn bản
```

**`CreateEmrDocument(lstData, treatmentFile)`** — thứ tự cố ý:

```
1. chưa bật EMR / lstData rỗng                     → return
2. dựng DocumentTDO từ form (tên, loại, nhóm), IsCapture = true
3. TreatmentCode = GetEmrTreatmentCode()
   rỗng → log Warn, return  (không tạo văn bản mồ côi không thuộc hồ sơ nào)
4. HisCode = BuildEmrHisCode(TreatmentCode, treatmentFile.ID)
5. WaitingManager.Show()
6. output = CombineMultiplePDFs(lstData)   → gộp tất cả đính kèm thành 1 pdf tạm
   file không tồn tại / 0 byte → "Không tạo được file để đẩy sang EMR. Dữ liệu đính kèm đã được lưu." → return
7. POST EMR.URI.EmrDocument.CREATE_WITH_FILE (EmrConsumer) kèm FileHolder{FileName=output, Content=stream}
8. resultData == null → MessageManager.Show(this, param, false)   (báo lỗi của EMR, KHÔNG rollback HIS)
finally: xóa file pdf tạm
```

**`GetEmrTreatmentCode`** — `EMR_TREATMENT` dùng chung ID với `HIS_TREATMENT`, nên lọc `api/EmrTreatment/Get` theo `ID = _TreatmentId`. Không tìm thấy → log Warn và **fallback về `currentTreatment.TREATMENT_CODE`** của HIS.

**`CombineMultiplePDFs(lstData)`** — điểm quan trọng nhất về mặt kỹ thuật:

```
outFile = Path.GetTempFileName();  PdfCopy(document, FileStream(outFile))
với từng item trong lstData:
   .pdf →  IsFss ? DownloadFssToTempFile(item.Url, ".pdf") : dùng item.FullName
           file không tồn tại → log Warn, bỏ qua
           AppendPdfToWriter(file, copyAcroForm: true)
   khác →  item.image == null → log Warn, bỏ qua
           ConvertImageToTempPdf(item.image) → AppendPdfToWriter(file, copyAcroForm: false)
finally: đóng writer/document/stream + xóa toàn bộ file tạm trung gian
```

So với bản cũ, 4 điểm đã sửa và **phải giữ**:

1. Duyệt `lstData` (đúng danh sách đang lưu) thay vì biến thành viên `ListfileNameAttack`.
2. File đã nằm trên FSS thì **tải về file tạm** trước khi đọc — trước đây `PdfReader(item.FullName)` với `FullName` là url FSS nên luôn nổ.
3. Nhánh ảnh trước đây tạo pdf tạm nhưng **không đóng stream và không nối trang vào file đích** → mất ảnh + rò file tạm. Nay tách hàm `ConvertImageToTempPdf` (dùng `using`) và luôn `AppendPdfToWriter`.
4. Tỷ lệ ảnh tính theo `docc.PageSize` (trang của chính file ảnh) thay vì `document.PageSize`.

`AppendPdfToWriter` bọc try/catch riêng: **1 file lỗi chỉ mất file đó**, các file còn lại vẫn được gộp.

### 5.5 Xóa — `btnGDelete_ButtonClick`

```
xác nhận Yes → POST api/HisTreatmentFile/Delete (MosConsumer, rowData.ID)
   success → DeleteEmrDocument(rowData)      ← phần mới
             FillDataFormList(); ListfileNameAttack = new List<AttackADO>()
   MessageManager.Show(this, param, success)
```

**`DeleteEmrDocument(treatmentFile)`**

```
chưa bật EMR / treatmentFile null → return
GetEmrDocumentsOfTreatmentFile(...) → GET EMR.URI.EmrDocument.GET_VIEW với filter:
        TREATMENT_CODE__EXACT = mã hồ sơ điều trị
        HIS_CODE__EXACT       = BuildEmrHisCode(mã hồ sơ, treatmentFile.ID)
        IS_ACTIVE = 1, IS_DELETE = false
không có văn bản nào → return lặng
với từng văn bản: POST EMR.URI.EmrDocument.DELETE (document.ID)
        thất bại → MessageManager.Show(..., false) rồi tiếp tục văn bản kế tiếp (không dừng cả vòng)
```

---

## 6. Hợp đồng dữ liệu

### 6.1 `DocumentTDO` gửi sang EMR

| Field | Giá trị | Nguồn |
|---|---|---|
| `DocumentName` | tên văn bản | `txtDocumentName.Text` (không bắt buộc) |
| `DocumentTypeId` | id loại văn bản | `cboDocumentType.EditValue` |
| `DocumentGroupId` | id nhóm văn bản | `CboDocumentGroup.EditValue` |
| `TreatmentCode` | mã hồ sơ điều trị | `EMR_TREATMENT.TREATMENT_CODE`, fallback `HIS_TREATMENT.TREATMENT_CODE` |
| `HisCode` | khóa đối chiếu ngược về HIS | `BuildEmrHisCode` — xem 6.2 |
| `IsCapture` | luôn `true` | văn bản dạng ảnh chụp/quét |
| file kèm | 1 file pdf đã gộp | `CombineMultiplePDFs` |

### 6.2 Quy ước `HIS_CODE` — bất biến quan trọng nhất

```
HIS_CODE = "{TREATMENT_CODE} HIS_TREATMENT_FILE_ID:{HIS_TREATMENT_FILE.ID}"
```

Lý do có nhãn `HIS_TREATMENT_FILE_ID:` (giống quy ước chung `HIS_TRACKING:`, `SERVICE_REQ_CODE:` của các màn khác): trên **cùng một hồ sơ điều trị** có nhiều chức năng cùng đẩy văn bản sang EMR. Nếu `HIS_CODE` chỉ là mã hồ sơ thì lúc xóa sẽ khớp và **xóa lây văn bản của chức năng khác**.

`BuildEmrHisCode` được **dùng chung cho cả lúc tạo và lúc xóa** — sửa quy tắc phải sửa 1 chỗ, 2 bên không thể lệch nhau. Đây là điểm dev tuyệt đối không được inline lại chuỗi này ở nơi khác.

---

## 7. Bất biến thiết kế (dev giữ — test kiểm)

1. **EMR không bao giờ chặn HIS.** Mọi hàm EMR bọc try/catch, lỗi chỉ log/báo. `HIS_TREATMENT_FILE` đã lưu/xóa thì không rollback.
2. **`CreateEmrDocument` gọi SAU khi HIS tạo thành công**, và nhận `resultData` (dòng vừa tạo) — vì cần `ID` để dựng `HIS_CODE`.
3. **Không tạo văn bản khi không có `TreatmentCode`** — thà thiếu văn bản còn hơn có văn bản mồ côi.
4. **1 lần bấm Lưu = 1 dòng `HIS_TREATMENT_FILE` = 1 văn bản EMR = 1 file PDF gộp** (n ảnh/pdf → 1 văn bản, không phải n văn bản).
5. **Xóa lọc theo cặp `TREATMENT_CODE__EXACT` + `HIS_CODE__EXACT`**, không lọc theo mỗi mã hồ sơ.
6. **Mọi file tạm phải được xóa** trong `finally`: pdf gộp (`CreateEmrDocument`), pdf trung gian và file tải từ FSS (`CombineMultiplePDFs`), ảnh quét (`FillImageScanToCardControl`).
7. **Ảnh quét luôn được chuẩn hóa về JPEG** trước khi upload, bất kể máy scan trả bmp hay jpeg.
8. **1 file đính kèm lỗi không được làm hỏng cả file gộp** (`AppendPdfToWriter` catch riêng).
9. **Nút Scan không phụ thuộc `MOS.HAS_CONNECTION_EMR`.**

---

## 8. Thông báo hiển thị cho người dùng

| Tình huống | Nội dung |
|---|---|
| Không có thiết bị WIA nào | Vui lòng kết nối đến máy Scan với máy tính |
| Có thiết bị nhưng không có máy scan | Không tìm thấy máy Scan được kết nối với máy tính |
| Máy scan không hỗ trợ 2 mặt (0x80210067) | Máy scan có thể không hỗ trợ in 2 mặt, vui lòng kiểm tra lại. |
| Upload FSS thất bại | Upload file thất bại, vui lòng liên hệ quản trị hệ thống để được hỗ trợ. |
| Gộp PDF thất bại / file 0 byte | Không tạo được file để đẩy sang EMR. Dữ liệu đính kèm đã được lưu. |
| API EMR tạo/xóa trả null/false | Thông báo lỗi chuẩn qua `MessageManager.Show(this, param, false)` |
| Chưa chọn file nào mà bấm Lưu | Vui lòng chọn file ảnh từ máy tính hoặc chụp ảnh từ camera. |

---

## 9. Log để test đối chiếu

Tìm theo các mốc sau trong log (`Inventec.Common.Logging`):

| Mốc | Mức | Ý nghĩa |
|---|---|---|
| `He thong khong ket noi EMR, bo qua tao van ban EMR` | Info | cấu hình tắt — đúng như mong đợi, không phải lỗi |
| `He thong khong ket noi EMR, bo qua xoa van ban EMR` | Info | như trên, nhánh xóa |
| `Khong xac dinh duoc ma ho so dieu tri, bo qua tao van ban EMR____TREATMENT_ID=` | Warn | không dựng được `TreatmentCode` |
| `Khong tim thay EMR_TREATMENT, dung ma cua HIS_TREATMENT____TREATMENT_ID=` | Warn | đã fallback sang mã HIS |
| `Gop file pdf de tao van ban EMR that bai, bo qua____` | Warn | file gộp rỗng/không tồn tại |
| `Khong tim thay file pdf de gop____` | Warn | 1 đính kèm bị bỏ qua |
| `File dinh kem khong co du lieu anh, bo qua____` | Warn | item không có `image` |
| `Tai file tu FSS that bai____` | Warn | `DownloadFssToTempFile` không lấy được stream |
| `Goi api tao van ban EMR thanh cong/that bai____Du lieu dau vao:...` | Debug | có full `DocumentTDO` (đọc được `HisCode`, `TreatmentCode`) |
| `Lay van ban EMR theo dong HIS_TREATMENT_FILE____TREATMENT_CODE=...____HIS_CODE=...____So van ban tim thay=` | Debug | kiểm tra đúng quy ước `HIS_CODE` và số bản khớp |
| `Goi api xoa van ban EMR thanh cong/that bai____EMR_DOCUMENT_ID=...____HIS_CODE=` | Debug | từng văn bản bị xóa |
| `du lieu anh scan: ...` | Debug | danh sách `AttackADO` sau khi quét |
| `Gắn lại giá trị theo máy scan:` | Error | đã rơi vào tầng fallback thông số WIA |

---

## 10. Kịch bản kiểm thử

### 10.1 Cấu hình

| # | Tiền đề | Thao tác | Kết quả mong đợi |
|---|---|---|---|
| C1 | `MOS.HAS_CONNECTION_EMR = 1` | mở form | 2 combo Loại/Nhóm văn bản có dữ liệu; Nhóm chỉ hiện nhóm lá |
| C2 | `MOS.HAS_CONNECTION_EMR = 0` | mở form, lưu, xóa | combo rỗng; `HIS_TREATMENT_FILE` vẫn lưu/xóa bình thường; không có bản ghi EMR nào; log Info "bo qua" |
| C3 | key không tồn tại | mở form | như C2, không văn vẹo lỗi |
| C4 | EMR service tắt, config = 1 | mở form | combo rỗng (log Error trong `GetDocumentType`), form vẫn dùng được |

### 10.2 Scan

| # | Tiền đề | Thao tác | Kết quả mong đợi |
|---|---|---|---|
| S1 | không cắm máy scan | bấm Scan | thông báo "Vui lòng kết nối đến máy Scan..." |
| S2 | chỉ có webcam/máy ảnh WIA | bấm Scan | thông báo "Không tìm thấy máy Scan..." |
| S3 | máy scan phẳng, bỏ trống ô "2 mặt" | Scan, quét 1 trang | 1 tile ảnh mới, xem trước đúng ảnh, đuôi file `.jpg`, upload FSS thành công |
| S4 | máy scan ADF, tick "2 mặt", 3 tờ | Scan | 6 tile (3 tờ × 2 mặt) theo đúng thứ tự, tên `..._Scan_1..6.jpg`, xem trước trang đầu |
| S5 | máy scan không hỗ trợ duplex, tick "2 mặt" | Scan | thông báo không hỗ trợ 2 mặt, không thêm tile nào |
| S6 | ADF, hủy hộp thoại quét giữa phiên | Scan | các trang đã quét trước đó vẫn được giữ, không crash |
| S7 | đã có 2 ảnh camera trong danh sách | Scan thêm 2 trang | tổng 4 tile, `Dem` không trùng, không mất ảnh cũ |
| S8 | máy scan chỉ hỗ trợ 300 DPI | Scan | vẫn ra ảnh (tầng fallback `AdjustScannerSettings`), log `Gắn lại giá trị theo máy scan:` |
| S9 | FSS lỗi | Scan | thông báo upload thất bại; ảnh vẫn hiện trên form |

### 10.3 Lưu + tạo văn bản EMR

| # | Tiền đề | Thao tác | Kết quả mong đợi |
|---|---|---|---|
| L1 | 1 ảnh camera + chọn Loại file, nhập đủ 3 thông tin văn bản | Lưu | 1 dòng `HIS_TREATMENT_FILE`; 1 `EMR_DOCUMENT` với `HIS_CODE = "{mã HS} HIS_TREATMENT_FILE_ID:{id vừa tạo}"`, file pdf 1 trang, `IsCapture=true` |
| L2 | 3 ảnh (1 chọn từ máy, 1 camera, 1 scan) | Lưu | **1** văn bản EMR duy nhất, pdf **3 trang** đúng thứ tự danh sách |
| L3 | 1 file pdf 5 trang chọn từ máy | Lưu | pdf gộp 5 trang, không mất trang |
| L4 | chọn 1 dòng đã lưu ở lưới (các file `IsFss=true`) rồi tạo mới có ảnh | Lưu | file lấy từ FSS được tải về và gộp được (không lỗi `PdfReader`) |
| L5 | không nhập Loại file | Lưu | chặn tại validate, không gọi API nào |
| L6 | không nhập Tên/Loại/Nhóm văn bản | Lưu | `HIS_TREATMENT_FILE` lưu OK; văn bản EMR tạo với các field rỗng/null hoặc EMR trả lỗi → hiện thông báo lỗi EMR, **HIS không bị rollback** (xem mục 11.4) |
| L7 | EMR service tắt (config vẫn = 1) | Lưu | `HIS_TREATMENT_FILE` lưu OK; hiện lỗi của EMR; không treo `WaitingManager` |
| L8 | 1 item pdf có url FSS sai | Lưu | log Warn bỏ qua file đó; các file còn lại vẫn vào pdf gộp |
| L9 | sau khi lưu thành công | quan sát form | 3 control văn bản, Loại file, mô tả, danh sách ảnh đều được clear (`SetDefaultValue`) |
| L10 | sau khi lưu | kiểm `%TEMP%` | không còn file tạm nào của lần lưu (pdf gộp + pdf trung gian + file tải FSS) |

### 10.4 Xóa

| # | Tiền đề | Thao tác | Kết quả mong đợi |
|---|---|---|---|
| X1 | 1 dòng đã lưu, đã có văn bản EMR | xóa dòng ở lưới | `HIS_TREATMENT_FILE` bị xóa; văn bản EMR tương ứng bị xóa; log `So van ban tim thay=1` |
| X2 | **2 dòng** đã lưu trên cùng hồ sơ | xóa dòng thứ 1 | **chỉ** văn bản của dòng 1 bị xóa, văn bản của dòng 2 còn nguyên (kiểm bất biến `HIS_CODE`) |
| X3 | cùng hồ sơ có văn bản EMR do chức năng khác tạo | xóa dòng ở màn này | văn bản của chức năng khác **không** bị xóa |
| X4 | văn bản EMR đã bị xóa tay trước đó | xóa dòng | `So van ban tim thay=0`, không lỗi, không thông báo lạ |
| X5 | EMR trả false khi xóa | xóa dòng | dòng HIS đã xóa; hiện lỗi EMR; không rollback |
| X6 | config = 0 | xóa dòng | chỉ xóa HIS, log Info bỏ qua EMR |

---

## 11. Giới hạn đã biết / nợ kỹ thuật

Ghi rõ ở đây để test không báo là bug mới, và để dev biết việc còn lại:

1. **Sửa (`btnEdit`) không đồng bộ EMR.** `UpdateDTOFromdataForm` chỉ cập nhật `HIS_TREATMENT_FILE`. Đổi/bớt file rồi bấm Sửa → văn bản EMR vẫn là bản gộp lúc tạo. Muốn làm: gộp lại pdf + gọi API cập nhật (hoặc xóa-tạo lại) theo đúng `HIS_CODE`.
2. **Xóa lẻ 1 ảnh trên tile (`tileView1_ContextButtonClick`) không đồng bộ EMR**, cũng không dùng `HIS_CODE`.
3. **Chọn 1 dòng ở lưới (`grvFormList_Click`) không nạp lại 3 thông tin văn bản** (chỉ nạp Loại file + Mô tả) — vì `HIS_TREATMENT_FILE` không lưu các field này. Người dùng thấy 3 ô trống dù văn bản EMR đã có tên/loại/nhóm.
4. **3 thông tin văn bản không được validate bắt buộc** (`ValidateForm` chỉ đặt rule cho `cboFileType`). Nếu EMR bắt buộc `DocumentName`/`DocumentTypeId` thì lỗi chỉ xuất hiện sau khi HIS đã lưu.
5. **`chkPrintDupicate` không được lưu `ControlState`** — mở lại form là mất lựa chọn "2 mặt". Bản gốc bên `EmrDocument\frmAttackFile.cs` có lưu.
6. **URI không nhất quán**: phần mới dùng hằng `EMR.URI.EmrDocument.*` nhưng vẫn hardcode `"api/EmrTreatment/Get"`, `"api/EmrDocumentType/Get"`, `"api/EmrDocumentGroup/Get"` (đã có `EMR.URI.EmrTreatment.GET`).
7. **`file.FileName = output`** là đường dẫn tuyệt đối của file tạm (giữ nguyên theo bản gốc). Tên file lưu bên EMR phụ thuộc cách backend xử lý.
8. **`Config/EmrConfigCFG.cs` và `DocumentUpdateStateForIntegrateSystem.cs` là code chết**: đã tạo file nhưng **chưa khai báo trong `.csproj`** và **không nơi nào gọi**. Đây là phần chuẩn bị cho việc thông báo trạng thái văn bản sang hệ thống tích hợp ngoài (port từ `HIS.Desktop.Plugins.EmrDocument`) nhưng chưa nối vào luồng. Trước khi dùng phải: thêm `<Compile Include=...>`, gọi `UpdateStateIGSys` sau khi tạo/xóa văn bản thành công.
9. **`csproj` đang trỏ `HIS.Desktop.Controls.Session.dll` vào `bin\Debug`** (trước là `bin\Release`) — cần trả lại `Release` trước khi phát hành.
10. **`CombineMultiplePDFs` trả về `outFile` kể cả khi có exception ở giữa** — bên gọi bắt buộc phải kiểm tra `FileInfo.Exists && Length > 0` như `CreateEmrDocument` đang làm. Đừng bỏ bước kiểm này ở nơi gọi mới.
11. **WIA yêu cầu build x86/AnyCPU đúng với driver máy scan**; `EmbedInteropTypes=True` nên không cần deploy `Interop.WIA.dll`, nhưng máy trạm phải có Windows Image Acquisition service đang chạy.

---

## 12. Phụ lục

### 12.1 Thuộc tính WIA đang dùng

| Mã | Tên | Giá trị đặt |
|---|---|---|
| 3088 | `WIA_DPS_DOCUMENT_HANDLING_SELECT` | 5 = FEEDER(1) \| DUPLEX(4) — chỉ khi tick "2 mặt" |
| 6146 | `WIA_IPA_...SCAN_COLOR_MODE` | 1 |
| 6147 / 6148 | H/V `SCAN_RESOLUTION_DPI` | 150 |
| 6149 / 6150 | H/V `SCAN_START_PIXEL` | 0 / 0 |
| 6151 / 6152 | H/V `SCAN_SIZE_PIXELS` | 1250 / 1754 (≈ A4 @150 DPI) |
| 6154 / 6155 | `BRIGHTNESS` / `CONTRAST` percents | 0 / 0 |

`formatJpeg = {B96B3CAE-0728-11D3-9D7B-0000F81EF32E}` — FormatID ảnh JPEG, chỉ dùng ở `Scan` (1 mặt). Nhánh duplex dùng format mặc định của máy nên có thể ra BMP → đã chuẩn hóa lại ở `FillImageScanToCardControl`.

### 12.2 API sử dụng

| API | Consumer | Dùng ở |
|---|---|---|
| `api/HisFileType/Get` | Mos | `InitCboFileType` |
| `api/HisTreatment/Get` | Mos | `LoadTreatment` |
| `api/HisTreatmentFile/Get` | Mos | `LoadPaging` |
| `api/HisTreatmentFile/Create` \| `/Update` \| `/Delete` | Mos | `btnAdd` / `btnEdit` / `btnGDelete` |
| `api/EmrDocumentType/Get` | Emr | `GetDocumentType` |
| `api/EmrDocumentGroup/Get` | Emr | `LoadCboTextGroup` |
| `api/EmrTreatment/Get` | Emr | `GetEmrTreatmentCode` |
| `EMR.URI.EmrDocument.CREATE_WITH_FILE` | Emr | `CreateEmrDocument` |
| `EMR.URI.EmrDocument.GET_VIEW` | Emr | `GetEmrDocumentsOfTreatmentFile` |
| `EMR.URI.EmrDocument.DELETE` | Emr | `DeleteEmrDocument` |
| `Inventec.Fss.Client.FileUpload.UploadFile` / `FileDownload.GetFile` | FSS | nạp/tải file đính kèm |
