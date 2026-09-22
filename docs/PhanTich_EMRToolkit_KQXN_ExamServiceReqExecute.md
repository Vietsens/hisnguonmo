# Phân tích: Liên thông kết quả xét nghiệm qua cổng EMRToolkit (bản 3 — đã chốt)

**Frontend:** `HIS.Desktop.Plugins.ExamServiceReqExecute` (Xử lý khám), `HIS.Desktop.Plugins.SereServTein` (Kết quả xét nghiệm), `HIS.Desktop.Plugins.Library.EmrToolkitImport` (thư viện kết nối)
**Backend:** `MOS` — source thật tại **`E:\svn\IMSys\BACKEND\MOS`** (`MOS.QuartzScheduler` + `MOS.MANAGER` + `MOS.API/Web.config`)
**Tài liệu cổng:** `E:\Tài liệu\Tích hợp\KQXN\HUONG_DAN_GOI_API_LIEN_THONG_KQXN.md`
**Trạng thái:** Phân tích hoàn thiện — sẵn sàng code

---

## 0. Các điểm đã chốt

| # | Nội dung | Kết luận |
|---|---|---|
| 1 | Nơi lưu kết quả đẩy | **Cột trên bảng có sẵn, KHÔNG tạo bảng mới**: `HIS_SERVICE_REQ` giữ trạng thái phiếu, `HIS_SERE_SERV` giữ `RECORD_ID` từng dịch vụ (mục 5) |
| 2 | Cách kích hoạt đẩy | **Chỉ tiến trình quét theo chu kỳ** — không cắm vào luồng trả kết quả xét nghiệm |
| 3 | Đẩy tay thủ công | **Bỏ** — không làm API, không làm màn hình đẩy lại |
| 4 | Chu kỳ tiến trình | **600000 ms (10 phút)**, khai báo ở `MOS.API/Web.config` |
| 5 | Phạm vi phiếu đẩy | **Mọi phiếu cận lâm sàng đã hoàn thành** thuộc loại được bật ở cấu hình, thuộc **hồ sơ đã kết thúc** (`HIS_TREATMENT.OUT_TIME` có giá trị), nằm trong **khoảng ngày quét** khai ở cấu hình |
| 6 | Cấu hình | **Một khóa `HIS_CONFIG` duy nhất** — `HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo`, các tham số là các vị trí phân tách `\|` (mục 6.1) |
| 7 | Luồng xem trên Xử lý khám | **Giữ** — nút `EMRToolkit` đặt ngay sau "In ấn" |
| 8 | Nhánh xem | Dùng **PDF** (`DownloadPdf`), không làm nhánh JSON (`VerifyOtp`) |
| 9 | Mã dùng chung | `MaDichVu` dùng `HIS_SERVICE.HEIN_SERVICE_BHYT_CODE` **có sẵn** (mã kỹ thuật dùng chung như khai báo với cổng BHYT). `MaDungChung` là **cột mới** `HIS_TEST_INDEX.COMMON_CODE`, khai báo ở `HIS.Desktop.Plugins.HisTestIndex` (mục 4.3) |
| 10 | `GioiTinh` | Giữ đúng quy ước HIS (`HIS_GENDER.ID`) |
| 11 | Loại CLS triển khai lần này | **Chỉ xét nghiệm**. Mọi tên gọi (khóa cấu hình, tên class, tên cột, tên job) đặt ở mức **cận lâm sàng — `Subclinical`**, để thêm CĐHA / PTTT / TDCN về sau chỉ là thêm builder + bật thêm loại trong cấu hình, không đổi kiến trúc hay DDL (mục 3.6) |

---

## 1. Hiện trạng code

Đã viết và **build thành công** trong `HIS.Desktop.Plugins.Library.EmrToolkitImport` (frontend):

| Thành phần | File | Theo phạm vi đã chốt |
|---|---|---|
| Cấu hình | `Config/EmrLabResultConfigCFG.cs` | Giữ, **đổi tên** `EmrToolkitSubclinicalConfigCFG.cs` + parse chuỗi cấu hình mới (mục 6.1) |
| Models | `Models/Subclinical/*.cs` (đổi tên từ `Models/LabResult/`) | Giữ phần đọc; **xóa** `LabResultSubmitRequestADO`, `LabResultSubmitResultADO`, `LabResultSyncItemADO`, `LabResultSyncResultADO`, `KetQuaXetNghiemPayloadADO`, `ThamSoXetNghiemADO`, `QcInfoADO` (chuyển sang backend) |
| Gọi API cổng | `Service/EmrToolkitSubclinicalApiService.cs` (đổi tên từ `EmrLabResultApiService.cs`) | Giữ `CheckValidity` / `RequestView` / `DownloadPdf`; **xóa** `SubmitLabResults`, `MaHoaJson` |
| Entry point | `EmrToolkitSubclinicalProcessor.cs` (đổi tên từ `EmrLabResultProcessor.cs`) | Giữ phần đọc; **xóa** `Submit*`, `ShowSyncResult` |
| Popup OTP | `Popup/frmEmrToolkitOtp.*` | **Giữ** |
| Popup kết quả đồng bộ | `Popup/frmEmrToolkitSyncResult.*` | **Xóa** (bỏ đẩy tay) |
| Resource vi/en + neutral | `Resources/*.resx` | Giữ; bỏ các key chỉ dùng cho đồng bộ |

`frmSereServTein.Designer.cs` đã có tab **"KQ liên thông EMRToolkit"** (lưới bản ghi cổng + `PdfViewer` + 2 nút) → **bỏ `btnSyncEmrToolkit`**, giữ `btnViewEmrToolkit`.

---

## 2. FRONTEND — Luồng xem kết quả liên thông

### 2.1. Nút trên màn Xử lý khám

| Thuộc tính | Giá trị |
|---|---|
| Tên control | `btnEmrToolkit` |
| Caption | `EMRToolkit` |
| Key ngôn ngữ | `ExamServiceReqExecuteControl.btnEmrToolkit.Text` (vi + en + my) |
| Vị trí | Hàng nút dưới cùng (y=646), **ngay sau** `btnPrint_ExamService` ("In ấn", x=1060); `btnVoBenhAn` (1150), `btnServiceConsult` (1240), `btnFastTrackingCreate` (1330) dịch phải |
| Hiển thị | `Visible = EmrToolkitSubclinicalProcessor.IsViewEnable(EmrToolkitSubclinicalType.TEST)` |
| Sự kiện | `btnEmrToolkit_Click` — partial mới `ExamServiceReqExecuteControl__EmrToolkit.cs` |

Khi click:

```
Lấy HIS_SERVICE_REQ loại XN của lần khám hiện tại (theo TREATMENT_ID)
  ├─ 0 phiếu  → thông báo "Lần khám này chưa có chỉ định xét nghiệm". DỪNG.
  ├─ 1 phiếu  → mở frmSereServTein với HIS_SERE_SERV đầu tiên của phiếu
  └─ >1 phiếu → popup chọn phiếu rồi mở
```

Mở plugin theo mẫu `HIS.Desktop.Plugins.ExecuteRoom/TreeSereServ___Process.cs:150`. `SereServTeinBehavior` đã nhận `long` + `Module` → **không sửa Behavior/Factory/Processor**.

### 2.2. Tab xem kết quả trong frmSereServTein

```
xtraTabPageEmrToolkit — "KQ liên thông EMRToolkit"
├── [Số định danh: 0790xxxxxxx]                      [Xem KQ EMRToolkit]
├── grdEmrValidity   — bản ghi còn hiệu lực trên cổng (chỉ đọc)
│     Mã phiếu XN · Mã dịch vụ · Loại XN · Mã CSKCB · Hiệu lực đến · QC · Ngày tạo
└── pdfViewerEmr     — phiếu kết quả PDF tải từ cổng
```

```
1. Mở tab (lazy-load — chỉ gọi API lần đầu click vào tab)
   ├─ chưa cấu hình → khóa tab + thông báo. DỪNG.
   └─ số định danh: HIS_PATIENT.CCCD_NUMBER → CMND_NUMBER (không có → thông báo, DỪNG)

2. GET api/LabResult/CheckValidity?soDinhDanhBenhNhan=...
   ├─ Data rỗng (HTTP 200) → "Không còn kết quả hiệu lực" (không phải lỗi, không log Error)
   └─ có dữ liệu → đổ lưới (BeginUpdate/EndUpdate)

3. Chọn 1 dòng → "Xem KQ EMRToolkit" (hoặc double-click)
   ├─ POST api/LabResult/RequestView { SoDinhDanhBenhNhan, MaPhieuXN } → TransactionId, KenhGuiOTP, ThoiHanOTP
   ├─ popup frmEmrToolkitOtp (đếm ngược theo ThoiHanOTP)
   └─ POST api/LabResult/DownloadPdf { TransactionId, OTP }
        → pdf (1 mẫu phiếu) hoặc zip (nhiều mẫu) → giải nén → pdfViewerEmr.LoadDocument
```

Ràng buộc từ cổng phải tôn trọng trong UI:

- OTP **chỉ dùng được một lần** → muốn xem lại phải xin OTP mới; thông báo rõ để user không tưởng là lỗi.
- `RequestView` giới hạn **5 giây/lần** cho cùng số định danh (HTTP 429) → disable nút 5 giây.
- Sai OTP quá 5 lần → HTTP 423 → đóng popup, bắt xin OTP mới.

Bảng lỗi → xử lý (đã cài trong `EmrToolkitSubclinicalApiService`): 401 lấy token mới thử lại 1 lần; 404/410/423/429 thông báo tương ứng; 403 khóa CSKCB nguồn bị vô hiệu; 5xx log Error.

### 2.3. Ràng buộc kỹ thuật FE

- `WaitingManager.Show/Hide` bao mọi lượt gọi mạng; `Hide()` trong `finally` và mọi `catch`.
- Gọi mạng ở luồng nền, cập nhật UI qua `Invoke` — không treo màn khám.
- Dữ liệu cổng **chỉ đọc** (`AllowEdit = false`); không áp dụng 4 cột audit (không phải bảng/view HIS).
- Không log OTP, token, `KeyGiaiMa`, số định danh bệnh nhân.
- `LogAction.Info` mỗi lần xem — truy vết ai đã xem dữ liệu bệnh nhân từ viện khác.
- `pdfViewerEmr.CloseDocument()` + gán null khi đóng form.

---

## 3. BACKEND MOS — Tiến trình đẩy KQXN

### 3.1. Mẫu bám theo

Ba tiến trình liên thông cổng ngoài trong MOS, đều cùng một bộ 4 file. Mẫu **mới nhất và sạch nhất là `Hoc3176`** (dùng `SqlBind.AddP` để bind tham số, chịu được trường hợp CSDL chưa có cột):

| Tiến trình | Thư mục MANAGER | Thư mục Scheduler |
|---|---|---|
| CSDL 4750 | `MOS.MANAGER/HisTreatment/Csdl4750/` | `MOS.QuartzScheduler/Csdl4750/` |
| VLG KDLYT | `MOS.MANAGER/HisTreatment/VlgKdlyt/` | `MOS.QuartzScheduler/VlgKdlyt/` |
| HOC 3176 | `MOS.MANAGER/HisTreatment/Hoc3176/` | `MOS.QuartzScheduler/Hoc3176/` |

Bộ 4 file mỗi tiến trình: `{Name}ADO.cs` (enum trạng thái + ADO kết nối + ADO kết quả), `{Name}ApiClient.cs` (HttpClient + token cache + TLS 1.2 + cắt message), `{Name}SyncProcessor.cs` (build → gửi → ghi trạng thái), `{Name}RetryScan.cs` (quét định kỳ, `IS_SENDING` chống chạy trùng, `MAX_*_PER_RUN`).

Hai điểm quan trọng học từ `Hoc3176`:

1. **Ghi trạng thái bằng SQL trực tiếp** (`DAOWorker.SqlDAO.Execute`) thay vì qua entity → **không phải cập nhật `MOS.EFMODEL`/EDMX** cho cột mới.
2. Bọc riêng try-catch quanh phần đọc/ghi cột mới, log cảnh báo nhắc kiểm tra DDL (`ORA-00904` khi CSDL chưa bổ sung cột) — tiến trình không làm sập luồng khác.

### 3.2. Cấu trúc file sẽ tạo

Tên đặt ở mức **cận lâm sàng (`Subclinical`)**, không đặt theo `LabResult`, để thêm CĐHA/PTTT/TDCN về sau không phải đổi tên file, tên job hay tên cột.

```
MOS.MANAGER/HisServiceReq/EmrToolkit/
├── EmrToolkitADO.cs                        ← enum EmrToolkitResultOption { SUCCESS=1, FAIL=2 }
│                                              enum EmrToolkitSubclinicalType { TEST, DIIM, TDCN, SURG, PROC }
│                                              EmrToolkitConnectionADO.Parse(chuỗi cấu hình — mục 6.1)
│                                              EmrToolkitSubmitResultADO (RecordId, ValidUntil), SyncResultADO
├── EmrToolkitApiClient.cs                  ← CreateToken (cache + hạn dùng) / MaHoaJson / Submit theo loại
├── Payload/
│   ├── IEmrToolkitPayloadBuilder.cs        ← Build(HIS_SERE_SERV, ngữ cảnh) → object payload
│   ├── EmrToolkitPayloadBuilderFactory.cs  ← theo SERVICE_TYPE_ID → builder tương ứng; loại chưa hỗ trợ → null
│   ├── EmrToolkitTestPayloadBuilder.cs     ← XÉT NGHIỆM (làm lần này): payload KetQuaXetNghiem + DanhSachThamSo
│   └── (sau này) EmrToolkitDiimPayloadBuilder.cs, EmrToolkitSurgPayloadBuilder.cs ...
├── EmrToolkitSubclinicalSyncProcessor.cs   ← đẩy 1 phiếu: validate → factory build → gửi → ghi cột
└── EmrToolkitSubclinicalRetryScan.cs       ← tiến trình quét định kỳ theo các loại được bật

MOS.MANAGER/Config/CFG/HisEmrToolkitCFG.cs   ← đọc HIS_CONFIG (đọc trực tiếp, không cache static
                                                 → sửa cấu hình không cần restart, giống HisHoc3176CFG)

MOS.QuartzScheduler/EmrToolkitSubclinical/
├── SyncEmrToolkitSubclinicalJob.cs         ← IJob.Execute → EmrToolkitSubclinicalRetryScan.Run()
└── SyncEmrToolkitSubclinicalTrigger.cs     ← đọc Web.config key interval; <=0 hoặc trống → không chạy job
```

Sửa thêm — **chỉ 2 file**:

| File | Thay đổi |
|---|---|
| `MOS.QuartzScheduler/JobProcessor.cs` | thêm `EmrToolkitSubclinical.SyncEmrToolkitSubclinicalTrigger.AddJob();` |
| `MOS.API/Web.config` | thêm `<add key="MOS.API.Scheduler.SyncEmrToolkitSubclinical" value="600000" />` |

Không sửa `MOS.EFMODEL`, `MOS.DAO`, `MOS.SDO`, `MOS.API/Controllers`, không sửa luồng trả kết quả xét nghiệm.

### 3.3. Tiến trình quét (EmrToolkitSubclinicalRetryScan)

```
SyncEmrToolkitSubclinicalJob (Quartz, 10 phút/lượt)
  └── EmrToolkitSubclinicalRetryScan.Run()
        ├─ IS_SENDING = true → bỏ lượt này (chống chạy trùng)
        ├─ HisEmrToolkitCFG.Connection == null → log Warn, thoát (chưa cấu hình)
        ├─ HisEmrToolkitCFG.SyncServiceReqTypeIds rỗng → thoát (không bật loại nào)
        ├─ số ngày quét = HisEmrToolkitCFG.ScanDayNumber (mục 3.3.1)
        └─ quét SQL, tối đa MAX_SERVICE_REQ_PER_RUN = 300 phiếu/lượt
```

SQL quét (tham số bind qua `SqlBind.AddP`, hằng số nằm trong mã nguồn):

```sql
SELECT REQ.* FROM HIS_SERVICE_REQ REQ
  JOIN HIS_TREATMENT TREA ON TREA.ID = REQ.TREATMENT_ID
 WHERE REQ.SERVICE_REQ_TYPE_ID IN ({0})          -- các loại CLS được bật ở cấu hình
   AND REQ.SERVICE_REQ_STT_ID  = :stt            -- HIS_SERVICE_REQ_STT.ID__HT (phiếu hoàn thành)
   AND (REQ.IS_DELETE IS NULL OR REQ.IS_DELETE <> 1)
   AND NVL(REQ.EMR_TOOLKIT_RESULT, 0) IN (0, 2)  -- 0 chưa gửi, 2 gửi lỗi
   AND TREA.OUT_TIME IS NOT NULL                 -- hồ sơ ĐÃ KẾT THÚC
   AND TREA.OUT_TIME >= :outTimeFrom             -- BẮT BUỘC: chặn khoảng ngày quét
   AND TREA.OUT_TIME <= :outTimeTo
   AND (TREA.IS_DELETE IS NULL OR TREA.IS_DELETE <> 1)
 ORDER BY REQ.ID FETCH FIRST 300 ROWS ONLY
```

`{0}` là danh sách id loại yêu cầu dịch vụ dựng từ cấu hình (`IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TEST`, sau này thêm `ID__DIIM`, `ID__SURG`...) — **không hardcode số**, và chỉ nhận các loại đã có builder tương ứng. Lần này danh sách chỉ có xét nghiệm.

Hai điều kiện then chốt:

- `TREA.OUT_TIME IS NOT NULL` — chỉ đẩy khi **hồ sơ đã kết thúc**, tránh gửi kết quả của hồ sơ còn đang điều trị (kết quả còn có thể bị sửa).
- `TREA.OUT_TIME` nằm trong khoảng ngày — **bắt buộc có, không được bỏ**. Đây là điều kiện dẫn hướng truy vấn (`HIS_TREATMENT.OUT_TIME` đã có index), giống cách `HisExpMestUploadErx` quét đơn thuốc điện tử. Thiếu nó thì mỗi lượt quét sẽ duyệt toàn bộ `HIS_SERVICE_REQ` của cả cơ sở dữ liệu → quá tải.

`NVL(...) IN (0, 2)` thay vì `IS NULL OR = 2` để dùng được index hàm tùy chọn ở mục 5.3.

Vượt ngưỡng 300 phiếu → log Warn, phần còn lại xử lý ở chu kỳ sau (đúng mẫu `Hoc3176RetryScan`).

### 3.3.1. Cấu hình số ngày quét — chống quét toàn bộ CSDL

| Nguồn | `HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo`, **vị trí 6** của chuỗi `|` (mục 6.1) |
|---|---|
| Ý nghĩa | Số ngày lùi lại tính từ hiện tại, áp lên `HIS_TREATMENT.OUT_TIME` |
| Mặc định | **3** ngày — dùng khi trường trống, không phải số, hoặc `<= 0` |
| Chặn trên | **90** ngày (`MAX_SCAN_DAY_NUMBER` trong mã nguồn) — khai to hơn thì lấy 90 |
| Cách đọc | `HisEmrToolkitCFG.SCAN_DAY_NUMBER` đọc trực tiếp `HIS_CONFIG` mỗi lượt quét |

```csharp
// Không bao giờ để truy vấn chạy mà thiếu mốc thời gian
int dayNumber = HisEmrToolkitCFG.SCAN_DAY_NUMBER;          // đã kẹp trong [1, 90]
long outTimeFrom = Convert.ToInt64(DateTime.Now.AddDays(-dayNumber).ToString("yyyyMMdd") + "000000");
long outTimeTo = Inventec.Common.DateTime.Get.Now().Value;
```

Cân nhắc khi chọn giá trị: kết quả xét nghiệm trên cổng chỉ còn hiệu lực theo `ValidUntil` (mặc định 24 giờ kể từ giờ trả kết quả nếu danh mục dùng chung chưa khai). Quét quá xa vừa tốn tải vừa đẩy lên những kết quả đã hết hiệu lực — 3 ngày là đủ để bù các lượt lỗi do mạng/cổng bảo trì.

### 3.4. Xử lý một phiếu (EmrToolkitSubclinicalSyncProcessor.Sync)

```
1. VALIDATE — không đạt → ghi EMR_TOOLKIT_RESULT = 2 + DESC rồi thoát (không gọi cổng)
   ├─ hồ sơ đã kết thúc (HIS_TREATMENT.OUT_TIME có giá trị) — kiểm lại vì có thể đã đổi từ lúc quét
   ├─ có FINISH_TIME (cổng cần để tính ValidUntil)
   ├─ bệnh nhân có số định danh: HIS_PATIENT.CCCD_NUMBER → CMND_NUMBER
   └─ có ít nhất 1 chỉ số HIS_SERE_SERV_TEIN với VALUE khác rỗng

2. LẤY DỮ LIỆU 1 LƯỢT (không query trong loop)
   ├─ HIS_SERE_SERV của phiếu: SERVICE_TYPE_ID = ID__XN, bỏ IS_NO_EXECUTE
   ├─ HIS_SERE_SERV_TEIN theo SERE_SERV_IDs (IS_ACTIVE = 1) → nhóm theo SERE_SERV_ID (ToLookup)
   ├─ HIS_TEST_INDEX / HIS_SERVICE / HIS_TEST_INDEX_UNIT từ Config (cache) → Dictionary
   └─ HIS_TREATMENT + HIS_PATIENT + V_HIS_SERVICE_REQ (REQUEST_USERNAME, EXECUTE_USERNAME)

3. TOKEN — CreateToken 1 lần cho cả lượt quét, cache theo chi nhánh kèm thời hạn

4. VỚI MỖI DỊCH VỤ XN (1 HIS_SERE_SERV = 1 payload)
   ├─ bỏ qua nếu đã có EMR_TOOLKIT_RECORD_ID và EMR_TOOLKIT_VALID_UNTIL còn hiệu lực
   ├─ build KetQuaXetNghiemPayload (mục 4); không còn chỉ số nào có kết quả → bỏ qua, ghi lý do
   ├─ POST api/EMR/MaHoaJson → DuLieu, KeyGiaiMa
   ├─ POST api/LabResult/Submit { MaCSKCB, SoDinhDanhBenhNhan, MaPhieuXN, DuLieu, KeyGiaiMa }
   │     → RecordId, ValidUntil
   └─ UPDATE HIS_SERE_SERV SET EMR_TOOLKIT_RECORD_ID, EMR_TOOLKIT_VALID_UNTIL, EMR_TOOLKIT_TIME

5. TỔNG KẾT PHIẾU — UPDATE HIS_SERVICE_REQ
   ├─ mọi dịch vụ thành công → EMR_TOOLKIT_RESULT = 1
   ├─ có dịch vụ lỗi        → EMR_TOOLKIT_RESULT = 2, EMR_TOOLKIT_DESC = lý do (cắt ≤ 900 ký tự)
   └─ EMR_TOOLKIT_TIME = thời điểm đẩy (yyyyMMddHHmmss)
```

Phiếu `RESULT = 2` sẽ được chu kỳ sau quét lại; dịch vụ nào đã có `RECORD_ID` còn hiệu lực thì không gửi lại → chỉ phần lỗi được thử lại, không sinh bản ghi trùng trên cổng.

### 3.5. Hiệu năng, log, an toàn

| Yêu cầu | Cách làm |
|---|---|
| Không dồn tải lên cổng | 300 phiếu/lượt, 10 phút/lượt, `IS_SENDING` chống chạy trùng |
| Không query trong loop | Danh mục đổ `Dictionary`/`ToLookup` trước vòng lặp |
| Không log rác | 1 dòng Info đầu/cuối lượt + số phiếu; lỗi từng phiếu log Error kèm `SERVICE_REQ_ID` |
| Không lộ dữ liệu | Không log payload (có định danh bệnh nhân), không log token / mật khẩu / `KeyGiaiMa` |
| Chịu được CSDL chưa có cột | Bọc try-catch riêng phần đọc/ghi cột mới, log Warn nhắc kiểm tra DDL |
| Không ảnh hưởng nghiệp vụ | Tiến trình độc lập, không sửa luồng trả kết quả xét nghiệm |

### 3.6. Mở rộng sang CĐHA / PTTT / TDCN sau này

Thiết kế để việc thêm một loại cận lâm sàng **không sửa** những phần dưới đây:

| Phần | Vì sao không phải sửa |
|---|---|
| DDL (mục 5) | Cột `EMR_TOOLKIT_*` đặt tên trung tính, không chứa chữ "LAB"/"TEST" → dùng chung mọi loại CLS |
| Tiến trình quét | Lọc theo **danh sách** `SERVICE_REQ_TYPE_ID` dựng từ cấu hình, không cố định một loại |
| Job / Trigger / Web.config | Tên `SyncEmrToolkitSubclinical*` ở mức cận lâm sàng |
| `EmrToolkitApiClient` | `CreateToken` + `MaHoaJson` dùng chung; chỉ endpoint Submit là khác theo loại |
| Xử lý một phiếu | `EmrToolkitSubclinicalSyncProcessor` gọi builder qua factory, không biết loại cụ thể |

Việc phải làm khi cổng mở API cho loại mới — đúng 3 bước:

1. Thêm `EmrToolkit<Loai>PayloadBuilder.cs` (build payload theo tài liệu cổng cho loại đó).
2. Khai loại đó trong `EmrToolkitPayloadBuilderFactory` + endpoint Submit tương ứng trong `EmrToolkitApiClient`.
3. Bật nhãn loại trong cấu hình, ví dụ `...|XN,CDHA|XN,CDHA|3||120` — không phải build lại gì nếu chỉ bật/tắt loại đã có builder.

Lưu ý nghiệp vụ khi mở rộng: CĐHA/PTTT trả kết quả dạng mô tả + kết luận + ảnh (`HIS_SERE_SERV_EXT`, `HIS_SERE_SERV_FILE`), không có `DanhSachThamSo` như xét nghiệm → builder của loại đó phải map sang đúng cấu trúc cổng yêu cầu, và phần tệp ảnh có thể cần luồng upload riêng. Đó là lý do tách builder theo loại ngay từ bây giờ.

---

## 4. Mapping HIS → payload cổng (backend build)

### 4.1. Hành chính — 1 payload = 1 dịch vụ XN

| Field payload | Nguồn | Ghi chú |
|---|---|---|
| `HoVaTenBenhNhan` | `HIS_SERVICE_REQ.TDL_PATIENT_NAME` | |
| `SoCCCD` | `HIS_PATIENT.CCCD_NUMBER` → `CMND_NUMBER` | Trùng `SoDinhDanhBenhNhan` khi Submit |
| `Tuoi` | tính từ `TDL_PATIENT_DOB` | |
| `GioiTinh` | `TDL_PATIENT_GENDER_ID` | Giữ nguyên quy ước HIS |
| `MaCoSoKhamChuaBenh` | cấu hình `MA_CSKCB`, trống → `MaCSKCB` của token | |
| `MaDichVu` | `HIS_SERVICE.HEIN_SERVICE_BHYT_CODE`, trống → `SERVICE_CODE` | Mã kỹ thuật dùng chung — chính mã đã khai với cổng BHYT, không thêm cột mới |
| `TenDichVu` | `HIS_SERVICE.SERVICE_NAME` | |
| `BacSiChiDinh` | `V_HIS_SERVICE_REQ.REQUEST_USERNAME` | Tên người, không phải loginname |
| `KyThuatVien` | `HIS_SERE_SERV_EXT.SUBCLINICAL_RESULT_LOGINNAME` → `EXECUTE_USERNAME` | |
| `ThoiGianLayMau` | `HIS_SERVICE_REQ.START_TIME` → `INTRUCTION_TIME` | `long yyyyMMddHHmmss` → ISO `yyyy-MM-ddTHH:mm:ss` |
| `ThoiGianTraKetQua` | `HIS_SERVICE_REQ.FINISH_TIME` | Quyết định `ValidUntil` |
| `QC.DatChuan` | `true` | HIS chưa quản lý QC nội kiểm ở mức này |
| `MaPhieuXN` (request Submit) | `HIS_SERVICE_REQ.SERVICE_REQ_CODE` | Cho phép tải PDF theo lô |

### 4.2. `DanhSachThamSo` — từng chỉ số

| Field payload | Nguồn | Ghi chú |
|---|---|---|
| `MaDungChung` | **`HIS_TEST_INDEX.COMMON_CODE`** (cột mới — mục 4.3), trống → `TEST_INDEX_CODE` | Mã chỉ số theo danh mục dùng chung |
| `TenXetNghiem` | `TEST_INDEX_NAME` | |
| `KetQua` | `HIS_SERE_SERV_TEIN.VALUE` | |
| `DonVi` | `TEST_INDEX_UNIT_NAME` | |
| `KhoangThamChieu` | `DESCRIPTION` (chỉ số bình thường) | |

Chỉ gửi chỉ số có `VALUE` khác rỗng.

### 4.3. Mã định danh gửi lên cổng

Cổng định danh dữ liệu bằng **mã dùng chung**, không phải mã nội bộ của viện. Hai field cần mã này lấy từ hai nguồn khác nhau:

| Field payload | Nguồn | Có sẵn? | Nơi khai báo |
|---|---|---|---|
| `MaDichVu` | `HIS_SERVICE.HEIN_SERVICE_BHYT_CODE` | **Có sẵn** — mã kỹ thuật dùng chung, chính mã đã khai với cổng BHYT | `HIS.Desktop.Plugins.HisService` (ô "Mã BHYT" hiện có) |
| `MaDungChung` | `HIS_TEST_INDEX.COMMON_CODE` | **Cột mới** — chỉ số xét nghiệm chưa có mã dùng chung | `HIS.Desktop.Plugins.HisTestIndex` (ô mới) |

`HEIN_SERVICE_BHYT_CODE` đang được dùng đúng vai này ở luồng xuất XML BHYT (`MA_DICH_VU` lấy từ cột đó — `UC_HisService.cs:8315`), nên **không thêm cột mới cho `HIS_SERVICE`**.

**Quy tắc lấy giá trị (fallback, không chặn đẩy):**

```
MaDichVu    = HIS_SERVICE.HEIN_SERVICE_BHYT_CODE
              trống -> SERVICE_CODE            (kèm log Warn: dịch vụ chưa khai mã BHYT)

MaDungChung = HIS_TEST_INDEX.COMMON_CODE
              trống -> TEST_INDEX_CODE         (kèm log Warn: chỉ số chưa khai mã dùng chung)
```

Chọn fallback thay vì chặn vì viện có hàng nghìn dịch vụ/chỉ số; bắt khai đủ mới cho đẩy sẽ khóa toàn bộ chức năng. Cổng từ chối mã sai thì lý do ghi vào `EMR_TOOLKIT_DESC`, quản trị danh mục biết mà khai bù.

#### Việc phải làm ở plugin `HIS.Desktop.Plugins.HisTestIndex`

| # | Việc | Chi tiết |
|---|---|---|
| 1 | Ô nhập | `txtCommonCode` (TextEdit, MaxLength 50) + `lciCommonCode` caption "Mã dùng chung", đặt cạnh ô "Mã BHYT" (`txtBHYTCode`) hiện có |
| 2 | Cột lưới | `COMMON_CODE`, caption "Mã dùng chung", đặt sau cột "Mã BHYT" (`grclBHYTCode`) |
| 3 | Nạp dữ liệu | `txtCommonCode.Text = data.COMMON_CODE` (cạnh chỗ gán `txtBHYTCode.Text`, `frmHisTestIndex.cs:782`) |
| 4 | Lưu | `currentDTO.COMMON_CODE = txtCommonCode.Text.Trim()` trong `UpdateDTOFromDataForm` (`frmHisTestIndex.cs:1168`) |
| 5 | Validate | Rule độ dài 50 — dùng lại `ValidMaxlength` / `ValidTextEditMaxLenght` sẵn có của plugin |
| 6 | Đa ngôn ngữ | Key caption ô nhập + cột lưới vào `Lang.vi.resx` và `Lang.en.resx` |

**Phụ thuộc kỹ thuật — khác với nhóm cột `EMR_TOOLKIT_*`:**

Plugin danh mục gán trực tiếp vào entity `MOS.EFMODEL.DataModels.HIS_TEST_INDEX` rồi POST qua API, nên cột mới **BẮT BUỘC có trong EFMODEL**, không thể chỉ đọc/ghi bằng SQL raw như phần trạng thái đẩy. Chuỗi việc:

```
1. DBA chạy DDL  : ALTER TABLE HIS_TEST_INDEX ADD COMMON_CODE   (mục 5.4)
                   cập nhật lại view V_HIS_TEST_INDEX (thêm cột vào SELECT)
2. Backend MOS   : thêm property vào DataModelTable.edmx (SSDL + CSDL + MSL),
                   DataModelView.edmx, HIS_TEST_INDEX.cs, V_HIS_TEST_INDEX.cs
                   + rule độ dài trong HisTestIndexCheck
3. Build MOS.EFMODEL -> MOS.EFMODEL.dll
4. Copy DLL sang E:\IVT DEV\FRONTEND\lib (theo quy trình release đang dùng)
5. Frontend      : sửa plugin HisTestIndex (6 việc ở bảng trên) rồi build
```

Trước khi hoàn tất bước 2–4, tiến trình đẩy vẫn chạy được nhờ fallback ở trên — nên có thể code và kiểm thử luồng đẩy song song, không phải chờ.

---

## 5. Nơi lưu — DDL (chốt: cột trên bảng có sẵn)

Script: `docs/SQL_EmrToolkit_LabResult.sql`.

Tên cột **không chứa chữ "LAB"/"TEST"** — cùng bộ cột này dùng cho mọi loại cận lâm sàng sau này (CĐHA, PTTT, TDCN), không phải thêm cột khi mở rộng. Loại của phiếu đã nằm ở `HIS_SERVICE_REQ.SERVICE_REQ_TYPE_ID`, không cần lưu lại.

### 5.1. `HIS_SERVICE_REQ` — trạng thái phiếu, phục vụ tiến trình quét

| Cột | Kiểu | Ý nghĩa |
|---|---|---|
| `EMR_TOOLKIT_RESULT` | NUMBER(1,0) | NULL/0 = chưa gửi, 1 = thành công, 2 = thất bại |
| `EMR_TOOLKIT_DESC` | VARCHAR2(1000 CHAR) | Lý do thất bại gần nhất (cắt ≤ 900 ký tự trước khi ghi) |
| `EMR_TOOLKIT_TIME` | NUMBER(14,0) | Thời điểm đẩy gần nhất (`yyyyMMddHHmmss`) |

### 5.2. `HIS_SERE_SERV` — `RECORD_ID` theo từng dịch vụ

| Cột | Kiểu | Ý nghĩa |
|---|---|---|
| `EMR_TOOLKIT_RECORD_ID` | VARCHAR2(50 CHAR) | **Mã bản ghi cổng trả về** cho dịch vụ này |
| `EMR_TOOLKIT_VALID_UNTIL` | NUMBER(14,0) | Hiệu lực đến (`yyyyMMddHHmmss`) — dùng để bỏ qua khi đẩy lại |
| `EMR_TOOLKIT_TIME` | NUMBER(14,0) | Thời điểm đẩy dịch vụ này |

Vì sao đặt ở đây: cổng trả **một `RecordId` cho mỗi dịch vụ kỹ thuật**, nên đây đúng hạt dữ liệu. Đọc luôn theo `SERVICE_REQ_ID`/`ID` — các index này đã có, **không thêm index mới trên `HIS_SERE_SERV`** (bảng rất lớn).

Chấp nhận có ý thức: mỗi lần đẩy lại sẽ ghi đè `RECORD_ID` cũ → chỉ giữ bản ghi cổng hiện hành, không giữ lịch sử nhiều lần đẩy. Nếu sau này cần lịch sử/đối soát thì mới tách bảng riêng.

### 5.3. Index — không bắt buộc

Truy vấn quét dẫn hướng bằng `HIS_TREATMENT.OUT_TIME` (đã có index) rồi join sang `HIS_SERVICE_REQ` theo `TREATMENT_ID` (đã có index), nên **không cần index mới** cho tiến trình chạy đúng.

Chỉ khi DBA đo thấy chậm ở bước lọc cờ thì mới thêm index hàm dưới đây (khớp điều kiện `NVL(EMR_TOOLKIT_RESULT,0) IN (0,2)`; index thường trên cột NULL-phần-lớn sẽ không hiệu quả):

```sql
-- TÙY CHỌN, chỉ thêm khi cần
CREATE INDEX IDX_HIS_SERVICE_REQ_EMR_TK
  ON HIS_SERVICE_REQ (NVL(EMR_TOOLKIT_RESULT, 0), TREATMENT_ID);
```

### 5.4. Danh mục chỉ số xét nghiệm — cột mã dùng chung (mục 4.3)

| Bảng | Cột | Kiểu | Ý nghĩa |
|---|---|---|---|
| `HIS_TEST_INDEX` | `COMMON_CODE` | VARCHAR2(50 CHAR) | Mã chỉ số theo danh mục dùng chung — gửi ở `MaDungChung` |

Phải cập nhật lại **view** `V_HIS_TEST_INDEX` (thêm cột vào câu SELECT) vì frontend nạp danh mục qua `BackendDataWorker` từ view và lưới danh mục bind theo view.

`HIS_SERVICE` **không thêm cột** — `MaDichVu` dùng `HEIN_SERVICE_BHYT_CODE` có sẵn.

Không cần index: cột chỉ để đọc theo khóa chính của danh mục (đã có index) và để hiển thị.

### 5.5. Việc kèm DDL

1. DBA chạy DDL (`ALTER TABLE ... ADD` cột NULL — Oracle 11g+ là thao tác metadata, không rewrite bảng) và cập nhật view `V_HIS_TEST_INDEX` ở mục 5.4.
2. Cột `EMR_TOOLKIT_*` (mục 5.1, 5.2): **không cần** cập nhật `MOS.EFMODEL`/EDMX — tiến trình đọc bằng `SqlDAO.GetSql<HIS_SERVICE_REQ>("SELECT * ...")` với điều kiện cột mới nằm trong `WHERE`, và ghi bằng `SqlDAO.Execute("UPDATE ...")`, đúng cách `Hoc3176SyncProcessor` đang làm.
3. Cột `COMMON_CODE` (mục 5.4): **BẮT BUỘC** cập nhật `MOS.EFMODEL` (EDMX + entity + view entity) rồi build lại `MOS.EFMODEL.dll` và đưa sang `E:\IVT DEV\FRONTEND\lib`, vì plugin danh mục gán trực tiếp vào entity — xem chuỗi việc ở mục 4.3.
4. Nếu sau này muốn frontend hiển thị trạng thái đẩy → khi đó mới bổ sung cột `EMR_TOOLKIT_*` vào view `V_HIS_SERVICE_REQ` + filter.

---

## 6. Cấu hình

### 6.1. HIS_CONFIG — MỘT khóa duy nhất, các tham số là vị trí phân tách `|`

Toàn bộ cấu hình nằm trong khóa **đã tồn tại** (dùng cho luồng import EMR), bổ sung các vị trí mới từ chỉ số 4 trở đi. Cùng quy chuẩn với `MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO` (`MaCsyt|Username|Password|ClientId|MaTinh|GrantType|TokenUrl|PushUrl|PrivateKey`).

```
Khóa   : HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo
Giá trị: <BaseUrl>|<TaiKhoan>|<MatKhau>|<IDMauPhieu>|<SyncTypes>|<ViewTypes>|<ScanDayNumber>|<MaCskcb>|<TimeoutSecond>
Ví dụ  : http://14.224.177.62:18090|79013|Abc@123456|524|XN|XN|3||120
Tối giản: http://14.224.177.62:18090|79013|Abc@123456|524|XN|XN
```

| Vị trí | Trường | Trống/sai thì | Ai dùng | Ý nghĩa |
|---|---|---|---|---|
| 0 | `BaseUrl` | chưa cấu hình | BE + FE | Địa chỉ cổng, bỏ dấu `/` cuối |
| 1 | `TaiKhoan` | chưa cấu hình | BE + FE | Một tài khoản dùng chung cho gửi và tra cứu |
| 2 | `MatKhau` | chưa cấu hình | BE + FE | Mật khẩu chỉ nằm ở một chỗ duy nhất |
| 3 | `IDMauPhieu` | `524` | FE | Của luồng import EMR sẵn có — liên thông CLS không dùng |
| 4 | `SyncTypes` | rỗng → **không đẩy** | BE | Loại CLS được **đẩy lên cổng**, phân tách `,`. Lần này: `XN` |
| 5 | `ViewTypes` | rỗng → **ẩn nút/tab** | FE | Loại CLS được **tra cứu/xem**, phân tách `,`. Lần này: `XN` |
| 6 | `ScanDayNumber` | `3`, kẹp `[1, 90]` | BE | Khoảng ngày quét theo `HIS_TREATMENT.OUT_TIME` — mục 3.3.1 |
| 7 | `MaCskcb` | lấy `MaCSKCB` của token | BE + FE | Ghi đè mã CSKCB (viện nhiều cơ sở) |
| 8 | `TimeoutSecond` | `120` | BE + FE | Timeout HTTP; tải PDF lâu hơn gọi JSON |

**Nhãn loại CLS** dùng ở vị trí 4 và 5 — chữ viết tắt nghiệp vụ, không dùng số, mỗi nhãn map sang một hằng `IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__*` trong mã nguồn:

| Nhãn | Loại | Trạng thái |
|---|---|---|
| `XN` | Xét nghiệm | **Làm lần này** |
| `CDHA` | Chẩn đoán hình ảnh | Cổng chưa có API — để dành |
| `TDCN` | Thăm dò chức năng | Để dành |
| `PTTT` | Phẫu thuật / thủ thuật | Để dành |

Bật thêm loại về sau chỉ là sửa cấu hình, ví dụ `...|XN,CDHA|XN,CDHA|3||120`. Nhãn nào **chưa có builder** trong mã nguồn thì bị bỏ qua kèm log Warn — cấu hình sai không làm sập tiến trình.

Quy tắc parse (BE và FE dùng cùng logic, mẫu `Hoc3176ConnectionADO.Parse`):

- Thiếu các vị trí phía sau → dùng mặc định, **không tính là lỗi cấu hình**.
- Giá trị không parse được → dùng mặc định + log Warn nêu rõ vị trí sai.
- Thiếu vị trí 0–2 → coi như chưa cấu hình: BE thoát tiến trình (log Warn), FE ẩn nút/tab.
- Tương thích ngược: luồng import EMR hiện chỉ đọc vị trí 0–3, thêm vị trí 4–8 không ảnh hưởng.

`HisEmrToolkitCFG` (BE) và `EmrToolkitSubclinicalConfigCFG` (FE) đọc trực tiếp `HIS_CONFIG` mỗi lần gọi, **không cache static**, để đổi cấu hình không phải restart tiến trình hay thoát ứng dụng — đúng cách `HisHoc3176CFG` làm.

### 6.2. MOS.API/Web.config

```xml
<add key="MOS.API.Scheduler.SyncEmrToolkitSubclinical" value="600000" />
```

Đặt cạnh các key cùng nhóm (`SyncCsdl4750` = 120000, `SyncVlgKdlyt`, `SyncHoc3176`). Giá trị `0` hoặc rỗng → **trigger không khởi động job**, dùng để tắt ở các viện không liên thông.

---

## 7. Danh sách file tạo/sửa

### Backend MOS (`E:\svn\IMSys\BACKEND\MOS`)

| File | Loại |
|---|---|
| `MOS.MANAGER/HisServiceReq/EmrToolkit/EmrToolkitADO.cs` | Mới |
| `MOS.MANAGER/HisServiceReq/EmrToolkit/EmrToolkitApiClient.cs` | Mới |
| `MOS.MANAGER/HisServiceReq/EmrToolkit/Payload/IEmrToolkitPayloadBuilder.cs` | Mới |
| `MOS.MANAGER/HisServiceReq/EmrToolkit/Payload/EmrToolkitPayloadBuilderFactory.cs` | Mới |
| `MOS.MANAGER/HisServiceReq/EmrToolkit/Payload/EmrToolkitTestPayloadBuilder.cs` | Mới |
| `MOS.MANAGER/HisServiceReq/EmrToolkit/EmrToolkitSubclinicalSyncProcessor.cs` | Mới |
| `MOS.MANAGER/HisServiceReq/EmrToolkit/EmrToolkitSubclinicalRetryScan.cs` | Mới |
| `MOS.MANAGER/Config/CFG/HisEmrToolkitCFG.cs` | Mới |
| `MOS.QuartzScheduler/EmrToolkitSubclinical/SyncEmrToolkitSubclinicalJob.cs` | Mới |
| `MOS.QuartzScheduler/EmrToolkitSubclinical/SyncEmrToolkitSubclinicalTrigger.cs` | Mới |
| `MOS.MANAGER/MOS.MANAGER.csproj`, `MOS.QuartzScheduler/MOS.QuartzScheduler.csproj` | Sửa — thêm Compile |
| `MOS.QuartzScheduler/JobProcessor.cs` | Sửa — 1 dòng `AddJob()` |
| `MOS.API/Web.config` | Sửa — 1 key interval |
| `MOS.EFMODEL/DataModels/DataModelTable.edmx` (SSDL + CSDL + MSL) | Sửa — thêm `COMMON_CODE` cho `HIS_TEST_INDEX` |
| `MOS.EFMODEL/DataModels/DataModelView.edmx` | Sửa — thêm `COMMON_CODE` cho `V_HIS_TEST_INDEX` |
| `MOS.EFMODEL/DataModels/HIS_TEST_INDEX.cs`, `V_HIS_TEST_INDEX.cs` | Sửa — thêm property `COMMON_CODE` |
| `MOS.MANAGER/HisTestIndex/HisTestIndexCheck.cs` | Sửa — rule độ dài 50 cho `COMMON_CODE` |

### Frontend

| File | Loại | Nội dung |
|---|---|---|
| `Library.EmrToolkitImport/**` | Sửa | Giữ luồng đọc, **xóa** phần Submit + popup kết quả đồng bộ (mục 1) |
| `SereServTein/frmSereServTein.Designer.cs` | Sửa | **Bỏ `btnSyncEmrToolkit`**, giữ tab + lưới + PdfViewer + `btnViewEmrToolkit` |
| `SereServTein/frmSereServTein__EmrToolkit.cs` | Mới | Logic tab xem: CheckValidity → OTP → PDF |
| `SereServTein/frmSereServTein.cs` | Sửa | Init tab trong Load, caption, ẩn/hiện theo cấu hình |
| `SereServTein/Resources/Lang.vi.resx`, `Lang.en.resx` | Sửa | Caption tab/nút/cột |
| `SereServTein/HIS.Desktop.Plugins.SereServTein.csproj` | Sửa | `DevExpress.XtraPdfViewer.v15.2` + ProjectReference library |
| `ExamServiceReqExecute/ExamServiceReqExecuteControl.designer.cs` | Sửa | `btnEmrToolkit` sau "In ấn", dịch các item từ x=1150 |
| `ExamServiceReqExecute/ExamServiceReqExecuteControl__EmrToolkit.cs` | Mới | Click → chọn phiếu XN → mở SereServTein |
| `ExamServiceReqExecute/__InitLanguage.cs`, `__Load.cs`, `Config/HisConfigCFG.cs`, `Resources/Lang.*.resx`, `*.csproj` | Sửa | Caption, ẩn/hiện, reference |
| `HisTestIndex/HisTestIndex/frmHisTestIndex.Designer.cs` | Sửa | Ô nhập `txtCommonCode` + cột lưới `COMMON_CODE` |
| `HisTestIndex/HisTestIndex/frmHisTestIndex.cs` | Sửa | Nạp và lưu `COMMON_CODE`, validate độ dài 50 |
| `HisTestIndex/Resources/Lang.vi.resx`, `Lang.en.resx` | Sửa | Caption ô nhập và cột lưới |


### Tài liệu

- `docs/SQL_EmrToolkit_LabResult.sql` — DDL (đã có, cập nhật theo bản này)
- `docs/HIS.Desktop.Plugins.ExamServiceReqExecute.md`, `docs/HIS.Desktop.Plugins.SereServTein.md` — cập nhật Changelog + mục tích hợp sau khi code xong

---

## 8. Rủi ro còn lại

| Rủi ro | Mức | Giảm thiểu |
|---|---|---|
| Chỉ số chưa khai `COMMON_CODE` (hoặc dịch vụ chưa khai mã BHYT) nên phải dùng fallback, cổng có thể từ chối mã | **Cao** | Fallback để chức năng chạy được ngay; lý do từ chối ghi vào `EMR_TOOLKIT_DESC` để quản trị danh mục khai bù; rà bằng dữ liệu thật trên môi trường test trước khi bật ở viện |
| Phải sửa `MOS.EFMODEL` và phát hành lại `MOS.EFMODEL.dll` cho frontend | **Cao** | Tách rõ hai nhóm cột: `EMR_TOOLKIT_*` không cần EFMODEL, chỉ `COMMON_CODE` cần. Luồng đẩy code và kiểm thử được trước nhờ fallback nên không phải chờ DLL |
| `ALTER TABLE HIS_SERE_SERV` trên bảng rất lớn | Trung bình | Chỉ thêm cột NULL (metadata-only), **không thêm index** trên bảng này; chạy ngoài giờ cao điểm |
| Ghi đè `RECORD_ID` khi đẩy lại | Trung bình | Chấp nhận có ý thức; bỏ qua dịch vụ đã có `RECORD_ID` còn hiệu lực nên thực tế ít ghi đè |
| Kết quả sửa sau khi đã đẩy → dữ liệu cổng lệch | Trung bình | Cần chốt nghiệp vụ: có đẩy lại khi sửa kết quả không (mục 9) |
| Dồn phiếu khi cổng lỗi dài | Thấp | 300 phiếu/lượt + khoảng ngày `SCAN_DAY_NUMBER` + cảnh báo khi vượt ngưỡng |
| Quét quá rộng gây quá tải CSDL | Thấp | Điều kiện `OUT_TIME` trong khoảng ngày là **bắt buộc trong SQL**; số ngày kẹp trong `[1, 90]`, mặc định 3 (mục 3.3.1) |
| OTP một lần dùng làm UX xem PDF khó chịu | Thấp | Thông báo rõ, nút xin OTP mới, chặn 5 giây |
| Cổng đổi response `CheckValidity` | Thấp | Parse phòng thủ, thiếu field không văng exception |

---

## 9. Điểm nghiệp vụ nên chốt thêm (không chặn việc code)

1. **Kết quả bị sửa sau khi đã đẩy thành công** — có đẩy lại để cổng cập nhật không? Nếu có, cần xoá `EMR_TOOLKIT_RECORD_ID` của dịch vụ đó khi kết quả thay đổi (sửa thêm ở luồng cập nhật kết quả), hoặc để tiến trình so `MODIFY_TIME` của `HIS_SERE_SERV_TEIN` với `EMR_TOOLKIT_TIME`. Phương án thứ hai không phải sửa luồng nghiệp vụ nên hợp với quyết định "chỉ dùng job".
2. **Phiếu bị hủy kết quả / hủy phiếu sau khi đã đẩy** — cổng chưa có API thu hồi; tạm thời để kết quả tự hết hiệu lực theo `ValidUntil`.
3. **`SCAN_DAY_NUMBER`** mặc định 3 ngày — đủ chưa? Tham chiếu: `MOS.CSDL_4750.RETRY_DAY_NUMBER` đang dùng 30 ngày, nhưng kết quả xét nghiệm trên cổng hết hiệu lực nhanh hơn nhiều nên 3 ngày là đủ.
4. **Hồ sơ chưa kết thúc nhưng đã có kết quả XN** (nội trú dài ngày) — hiện không đẩy. Nếu nghiệp vụ muốn đẩy sớm để viện khác tra cứu được trong lúc đang điều trị thì phải đổi điều kiện `OUT_TIME IS NOT NULL` thành mốc khác (ví dụ phiếu hoàn thành quá N giờ).
