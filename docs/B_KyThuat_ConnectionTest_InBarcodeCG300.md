# Kết nối xét nghiệm — In tem barcode qua máy CG300 (Method 2)

| Thông tin | Giá trị |
|-----------|---------|
| Plugin | `HIS.Desktop.Plugins.ConnectionTest` (UC `UC_ConnectionTest`) |
| Thiết bị | Hệ thống lấy máu tự động FOYEN CG300 (tài liệu giao tiếp chung dòng CG-600PLUS) |
| Phương thức kết nối | **Method 2** — HIS/LIS sinh file JSON, phần mềm CG đọc file trong thư mục `ALL`, đọc xong tự xoá |
| Nguồn ContainerCode | Danh mục **Loại mẫu bệnh phẩm** (`HIS_TEST_SAMPLE_TYPE`) gán cho dịch vụ tại `HIS.Desktop.Plugins.HisServiceTestSample` (`HIS_SERVICE_TESA`) |
| Tài liệu gốc | `E:\Tài liệu\Tích hợp\CG300\` — `Software Program Interface Settings FOYEN.pdf`, `LisDG.docx` (bảng ContainerCode ↔ màu ↔ khay) |
| Trạng thái | **Đã code** (13/10/2026) — các câu hỏi mục C áp dụng phương án mặc định, chờ chốt tại viện |

---

## A. PHÂN TÍCH NGHIỆP VỤ

### A.1 Bối cảnh

Tại phòng lấy mẫu, KTV dùng **một máy quét barcode duy nhất**, không thao tác chuột/bàn phím. Máy CG300 đặt cạnh máy trạm HIS; khi HIS ghi file dữ liệu mẫu vào thư mục `ALL`, CG300 tự **dán tem lên ống đúng màu và nhả ống** (với ContainerCode cấu hình là "Tembarcode" thì chỉ in tem rời).

### A.2 Luồng thao tác mong muốn (theo yêu cầu)

| Lần quét | Hệ thống làm gì | Ghi chú |
|----------|-----------------|---------|
| 1 — mã A | Đưa mã vào ô tìm kiếm → tìm → hiển thị mẫu của A | Không cần click ô/nút tìm |
| 2 — **lại mã A** | Sinh file CG300 cho các mẫu đang hiển thị → máy in tem + nhả ống | Giữ nguyên kết quả đang hiển thị |
| 3 — mã B | Con trỏ đã ở ô tìm kiếm, nội dung cũ bôi đen → mã B ghi đè → tìm ngay | |
| 4 — **lại mã B** | Sinh file CG300 → in tem + nhả ống | Lặp lại chu trình |

**Quy tắc phân biệt "tìm" và "in"** (đề xuất): khi chế độ CG300 bật, lần quét có mã **trùng với mã vừa tìm** và lưới đang có kết quả ⇒ **IN**; khác mã ⇒ **TÌM**.
Lý do không dùng kiểu luân phiên lẻ/chẵn: nếu KTV quét nhầm sang BN khác ở bước 2, luân phiên sẽ in tem của BN cũ → sai người bệnh. So khớp mã an toàn hơn.

### A.3 Sơ đồ trạng thái

```
                 quét mã X (X ≠ lastScan)
   ┌────────────┐ ─────────────────────────► ┌──────────────┐
   │   CHỜ QUÉT │                            │ ĐÃ TÌM (X)   │
   └────────────┘ ◄──── không có kết quả ─── └──────────────┘
         ▲                                     │ quét lại X
         │                                     ▼
         │                              ┌──────────────┐
         └──── in xong / lỗi (giữ KQ) ──│ ĐANG IN (X)  │
                                        └──────────────┘
   Sau khi in: lastScan vẫn = X → quét X lần nữa = in lại (RePrint = "Yes")
```

### A.4 Điều kiện nghiệp vụ

1. Checkbox "In tem qua CG300" **chỉ hiển thị khi máy trạm kết nối được CG300** (điều kiện ở T.6). Chỉ khi checkbox hiện **và** được tích thì mới áp dụng luồng quét 2 lần; còn lại ô tìm kiếm giữ nguyên hành vi hiện tại.
2. Mẫu phải **có BARCODE**. Mẫu chưa có barcode ⇒ bỏ qua và cảnh báo (không tự sinh barcode ở bước này — xem câu hỏi Q4).
3. Mẫu đang **Chưa lấy mẫu / Từ chối** ⇒ sau khi ghi file thành công, cập nhật **Đã lấy mẫu** (`api/LisSample/Sample`) — dùng lại đúng logic của nhánh in Bartender, tôn trọng config `LIS.LIS_SAMPLE.ALLOW_AUTO_SAMPLE_AFTER_PRINT_BARCODE`.
4. Một barcode có thể cần **nhiều ống** (VD 1121034 → ống tím 001, xanh 002, đen 006). Mỗi cặp (barcode × ContainerCode) = **1 bản ghi** trong file; các dịch vụ cùng ống gộp chung `TestItemDesc`.
5. Dịch vụ chưa được gán loại mẫu bệnh phẩm ⇒ không sinh bản ghi cho dịch vụ đó, cảnh báo tên dịch vụ để khoa cấu hình bổ sung tại "Dịch vụ – Loại mẫu bệnh phẩm".
6. Không reload lưới sau khi in (chỉ cập nhật trạng thái dòng tại chỗ) ⇒ không mất kết quả đang hiển thị.
7. Sau mọi thao tác (tìm/in/lỗi) con trỏ **luôn trở về ô tìm kiếm và bôi đen** nội dung.

### A.5 Giao thức Method 2 (tóm tắt)

Chi tiết đầy đủ ở **phần T — Thông tin tích hợp CG300** bên dưới.

---

## T. THÔNG TIN TÍCH HỢP CG300

### T.1 Thông tin thiết bị & phương thức

| Hạng mục | Nội dung |
|----------|----------|
| Thiết bị | FOYEN CG300 — hệ thống quản lý lấy máu thông minh (tự chọn ống theo màu, dán tem, nhả ống) |
| Tài liệu hãng | *Software Program Interface Settings (Model CG-600PLUS)* — dùng chung cho dòng CG |
| Phương thức hãng hỗ trợ | **Chỉ 2 phương thức**: Method 1 (CG gọi WebService/WebAPI của HIS/LIS) và Method 2 (HIS/LIS sinh file, CG đọc file) |
| Phương thức áp dụng | **Method 2** — không cần mở API cho CG, quy trình thao tác HIS giữ nguyên, phần mềm CG chạy nền |
| Mô hình triển khai | Phần mềm điều khiển CG và HIS Desktop **cài trên cùng một máy trạm** tại phòng lấy mẫu; CG nối với máy trạm qua cáp của hãng |
| Thư mục trao đổi | Thư mục **`ALL`** (theo tài liệu: nằm dưới thư mục chương trình HIS/LIS). Đường dẫn thực tế cấu hình theo máy trạm (xem B.3) |
| Chiều dữ liệu | **Một chiều HIS → CG**. CG không phản hồi kết quả (khác Method 1 có `UpdateBarcode`) |
| Vòng đời file | HIS ghi file → CG đọc → **CG tự xoá file sau khi đọc** |
| Ống không phải máu | ContainerCode cấu hình là "Tembarcode" ⇒ CG chỉ in tem rời qua máy in ngoài, không nhả ống |

### T.2 Định dạng file

| Thuộc tính | Giá trị |
|------------|---------|
| Nội dung | **Mảng JSON** `[ {...}, {...} ]` — mỗi phần tử là một `FyLabelResponseModel` |
| Đơn vị bản ghi | 1 bản ghi = 1 ống = (1 barcode × 1 ContainerCode). Một barcode nhiều ống ⇒ nhiều bản ghi cùng `LabNum`, khác `ContainerCode` |
| Ràng buộc hãng | Trong một file, mỗi barcode/ống chỉ xuất hiện một lần ⇒ LIS phải gom toàn bộ dịch vụ theo barcode trước khi ghi |
| Kiểu dữ liệu | Tất cả trường là **chuỗi** (kể cả `BloodConfirmState`, `AgeDesc`) |
| Định dạng thời gian | `yyyy-MM-ddTHH:mm:ss` (theo mẫu hãng và `LisDG.docx`) |
| Đuôi / encoding | `.txt`, UTF-8 — **cần xác nhận với hãng** (Q5) |
| Tên file | `{LabNum}_{yyyyMMddHHmmssfff}.txt` (HIS tự đặt, CG đọc mọi file trong `ALL`) |

### T.3 Cấu trúc `FyLabelResponseModel`

| # | Trường | Mô tả (hãng) | Bắt buộc | In trên tem |
|---|--------|--------------|:--------:|:-----------:|
| 1 | `LabNum` | Số mẫu = mã barcode | ✔ | ✔ |
| 2 | `HospNum` | Mã bệnh nhân / số khám | ✔ | ✔ |
| 3 | `PatientType` | Loại bệnh nhân | | |
| 4 | `CnName` | Họ tên | ✔ | ✔ |
| 5 | `Sex` | Giới tính | ✔ | ✔ |
| 6 | `AgeDesc` | Tuổi | ✔ | ✔ |
| 7 | `Location` | Khoa/phòng chỉ định | | |
| 8 | `OrderDate` | Thời gian chỉ định | | |
| 9 | `Priority` | Ưu tiên (`Urgent` / `Normal`) | | |
| 10 | `Source` | Nguồn bệnh phẩm (máu tĩnh mạch, huyết tương…) | ✔ | ✔ |
| 11 | `ContainerCode` | **Mã ống** — CG dùng để chọn khay/màu ống | ✔ | |
| 12 | `Container` | Màu ống | ✔ | |
| 13 | `RePrint` | Đã in hay chưa (`Yes`/`No`) | | |
| 14 | `TestItemDesc` | Tên xét nghiệm | | |
| 15 | `DeptDesc` | Khoa thực hiện/khoa hiển thị trên tem | ✔ | ✔ |
| 16 | `ExamineRequestID` | Mã phiếu yêu cầu HIS | | |
| 17 | `PaymentStatus` | Trạng thái thanh toán | | |
| 18 | `PrintTime` | Thời gian in tem | ✔ | ✔ |
| 19 | `PrintIP` | IP máy in tem | | |
| 20 | `TestType` | Loại bệnh phẩm (máu/nước tiểu/phân) | | |
| 21 | `TestItemType` | Nhóm XN (sinh hoá, huyết học…) | | |
| 22 | `PrintUser` | Tài khoản in | | |
| 23 | `TakeReportDesc` | Thời gian trả kết quả | | |
| 24 | `TakeReportAddress` | Nơi nhận kết quả | | |
| 25 | `BedNum` | Số giường | | |
| 26 | `BloodConfirmState` | Xác nhận lấy máu (`0` chưa / `1` đã) | | |

Cột "In trên tem" theo yêu cầu bệnh viện trong `LisDG.docx`: *LabNum, HospNum, CnName, Sex, AgeDesc, Source, DeptDesc, PrintTime*. Nguồn dữ liệu HIS cho từng trường: xem **B.1**.

### T.4 Bảng ContainerCode ↔ màu ống ↔ khay (cấu hình trên máy CG tại viện)

Mã trong bảng này phải **trùng khớp** với `TEST_SAMPLE_TYPE_CODE` khai báo trong danh mục Loại mẫu bệnh phẩm.

| ContainerCode | Màu (`Container`) | Khay CG |
|---------------|-------------------|---------|
| 001 | Purple | 2 và 3 |
| 004 | Red | 7 |
| 006 | Black | 4 và 5 |
| 007 | White | 1 và 8 |
| 013 | Grey | Tembarcode (chỉ in tem) |
| 020 → 060 | Yellow | Tembarcode (chỉ in tem) |
| 099 | White | 1 và 8 |
| 100 | Purple | 2 và 3 |
| 101 | Green | 6 |
| 102 | Red | 7 |
| 103 | Purple | 2 và 3 |
| 104 | White | 1 và 8 |
| 105 | Purple | 2 và 3 |
| 106 | White | 1 và 8 |

> Ảnh thực tế tại viện có ống xanh lá mã `002` (Green) — **chưa có trong bảng `LisDG.docx`**, cần đối chiếu lại cấu hình máy.

### T.5 File mẫu (HIS sinh ra)

```json
[
  {"LabNum":"1121034","HospNum":"0000123456","PatientType":"BHYT","CnName":"VŨ THỊ MIÊN","Sex":"Nữ","AgeDesc":"62",
   "Location":"Phòng khám 1","OrderDate":"2026-05-13T14:20:10","Priority":"Normal","Source":"Máu toàn phần",
   "ContainerCode":"001","Container":"Purple","RePrint":"No","TestItemDesc":"Tổng phân tích tế bào máu ngoại vi",
   "DeptDesc":"KKB","ExamineRequestID":"000012345678","PaymentStatus":"","PrintTime":"2026-05-13T14:22:53",
   "PrintIP":"10.0.0.21","TestType":"Máu","TestItemType":"","PrintUser":"ktv01","TakeReportDesc":"",
   "TakeReportAddress":"","BedNum":"","BloodConfirmState":"1"},
  {"LabNum":"1121034","HospNum":"0000123456","...":"...","ContainerCode":"006","Container":"Black",
   "TestItemDesc":"Định lượng Glucose; Định lượng Creatinin"}
]
```

### T.6 Xác định "kết nối được CG300"

Method 2 không có kênh hỏi trạng thái thiết bị ⇒ HIS xác định kết nối bằng các điều kiện **trên máy trạm**:

| # | Điều kiện | Cách kiểm tra |
|---|-----------|---------------|
| 1 | Máy trạm được khai báo dùng CG300 | Config máy trạm `...CG300_FOLDER` **có giá trị** (rỗng ⇒ máy không dùng CG300) |
| 2 | Thư mục `ALL` tồn tại và **ghi được** | `Directory.Exists` + ghi/xoá thử file `.chk` (không để CG đọc nhầm: đuôi khác `.txt`) |
| 3 | Phần mềm điều khiển CG **đang chạy** | `Process.GetProcessesByName(<tên tiến trình CG>)` — tên tiến trình khai báo trong config `...CG300_PROCESS_NAME`; để trống ⇒ bỏ qua điều kiện này |

Đủ cả 3 ⇒ **Kết nối CG300 = OK** ⇒ hiện checkbox "In tem qua CG300". Một điều kiện không đạt ⇒ ẩn checkbox, chế độ quét về như cũ.

Thời điểm kiểm tra: khi **load UC**, khi bấm **Tìm (Ctrl F)**, và **ngay trước mỗi lần ghi file** (nếu mất kết nối giữa chừng ⇒ báo lỗi, ẩn checkbox, không cập nhật trạng thái lấy mẫu). Không dùng timer chạy nền để tránh tốn tài nguyên.

### T.7 Checklist triển khai tại viện

- [ ] Cài phần mềm điều khiển CG trên máy trạm phòng lấy mẫu, cấu hình thư mục đọc = thư mục `ALL`.
- [ ] Khai báo bảng ContainerCode/khay trên máy CG (T.4).
- [ ] Danh mục **Loại mẫu bệnh phẩm**: mã = ContainerCode; gán loại mẫu cho từng dịch vụ XN tại "Dịch vụ – Loại mẫu bệnh phẩm".
- [ ] Config máy trạm: đường dẫn `ALL`, tên tiến trình CG.
- [ ] Phân quyền ghi/xoá thư mục `ALL` cho user Windows chạy HIS và phần mềm CG.
- [ ] Chạy thử 1 mẫu nhiều ống, đối chiếu tem in và màu ống.

---

## B. THIẾT KẾ KỸ THUẬT

### B.1 Ánh xạ dữ liệu → `FyLabelResponseModel`

Tem in ra hiển thị: **LabNum, HospNum, CnName, Sex, AgeDesc, Source, DeptDesc, PrintTime** (theo `LisDG.docx`).

| Trường CG | Nguồn HIS/LIS | Định dạng / ví dụ |
|-----------|---------------|-------------------|
| `LabNum` | `V_LIS_SAMPLE.BARCODE` | `1121034` |
| `HospNum` | `V_LIS_SAMPLE.PATIENT_CODE` | Mã BN |
| `PatientType` | `V_LIS_SAMPLE.PATIENT_TYPE_NAME` | "BHYT" / "Viện phí" |
| `CnName` | `V_LIS_SAMPLE.VIR_PATIENT_NAME` (fallback `LAST_NAME + " " + FIRST_NAME`) | `VŨ THỊ MIÊN` |
| `Sex` | `V_LIS_SAMPLE.GENDER_NAME` | `Nam` / `Nữ` |
| `AgeDesc` | Tính từ `DOB` (`MPS.AgeUtil.CalculateFullAge`, đã dùng ở nhánh Bartender) | `62` |
| `Location` | `V_LIS_SAMPLE.REQUEST_ROOM_NAME` | Phòng chỉ định |
| `OrderDate` | `V_LIS_SAMPLE.INTRUCTION_TIME` | `yyyy-MM-ddTHH:mm:ss` |
| `Priority` | `IS_EMERGENCY == 1` ? `Urgent` : `Normal` | |
| `Source` | `HIS_TEST_SAMPLE_TYPE.TEST_SAMPLE_TYPE_NAME` | "Heparin", "Nước tiểu" |
| `ContainerCode` | **`HIS_TEST_SAMPLE_TYPE.TEST_SAMPLE_TYPE_CODE`** | `001`, `004`, `006`… (khớp bảng khay của máy) |
| `Container` | Màu ống — xem câu hỏi **Q2** | `Purple` |
| `RePrint` | `PRINT_COUNT > 0` hoặc mã đã in trong phiên ? `Yes` : `No` | |
| `TestItemDesc` | Tên các dịch vụ của mẫu thuộc ống này, nối `"; "` | |
| `DeptDesc` | `V_LIS_SAMPLE.REQUEST_DEPARTMENT_NAME` (tem thực tế BV in "KKB") | |
| `ExamineRequestID` | `V_LIS_SAMPLE.SERVICE_REQ_CODE` | |
| `PrintTime` | Thời điểm ghi file | `yyyy-MM-ddTHH:mm:ss` |
| `PrintIP` / `PrintUser` | IP máy trạm / loginname hiện tại | |
| `TestType` | `V_LIS_SAMPLE.SAMPLE_TYPE_NAME` (loại bệnh phẩm LIS) | |
| `PaymentStatus`, `TestItemType`, `TakeReportDesc`, `TakeReportAddress`, `BedNum` | Để trống | Không bắt buộc |
| `BloodConfirmState` | `"1"` | Đã xác nhận lấy máu |

### B.2 Xác định ContainerCode cho từng dịch vụ

```
V_LIS_SAMPLE (đang hiển thị)
  └─ api/LisSampleService/GetView {SAMPLE_IDs}      → SERVICE_CODE theo mẫu
       └─ BackendDataWorker V_HIS_SERVICE (dict theo SERVICE_CODE) → SERVICE_ID
            └─ HIS_SERVICE_TESA (dict SERVICE_ID → TEST_SAME_TYPE_ID)   [cấu hình tại HisServiceTestSample]
                 └─ HIS_TEST_SAMPLE_TYPE (dict ID) → TEST_SAMPLE_TYPE_CODE = ContainerCode
Không fallback qua loại mẫu của y lệnh (`HisServiceReqFilter` không lọc được theo danh sách mã → sẽ phải gọi API trong vòng lặp). Dịch vụ chưa gán loại mẫu ⇒ cảnh báo, không sinh bản ghi.
```

- `HIS_SERVICE_TESA` và `HIS_TEST_SAMPLE_TYPE` là danh mục nhỏ, **filter không có SERVICE_IDs** ⇒ nạp toàn bộ bản ghi `IS_ACTIVE = 1` **một lần** (lazy, lần in đầu tiên), giữ trong `Dictionary`, giải phóng khi đóng UC.
- Một dịch vụ gán **nhiều loại mẫu** ⇒ lấy loại mẫu đầu tiên theo `ID` và ghi log Warn (xem Q3).
- Gom nhóm: `ToLookup(barcode + containerCode)` — O(n), không vòng lặp lồng.

### B.3 Cấu hình

| Key | Loại | Giá trị | Ý nghĩa |
|-----|------|---------|---------|
| `CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_FOLDER` | ConfigApplication (per máy trạm) | Đường dẫn thư mục | Thư mục `ALL` mà CG đọc. **Rỗng ⇒ máy trạm không dùng CG300** (checkbox luôn ẩn) |
| `CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_PROCESS_NAME` | ConfigApplication (per máy trạm) | Tên tiến trình (không `.exe`) | Phần mềm điều khiển CG. Rỗng ⇒ không kiểm tra tiến trình |
| `chkPrintBarcodeCG300` ("In tem qua CG300") | Checkbox trên UI + **ControlStateWorker** | 1/0 | **Chỉ hiển thị khi kết nối CG300 OK** (T.6). Khi hiện, khôi phục trạng thái tích lần trước |

Không dùng HisConfig toàn viện vì chỉ một số máy trạm có thiết bị.

### B.4 Ghi file an toàn

1. Serialize mảng bản ghi bằng `Newtonsoft.Json` (đã có trong solution), encoding **UTF-8** (tiếng Việt có dấu — xem Q5).
2. Ghi ra `{LabNum}_{yyyyMMddHHmmssfff}.tmp` trong thư mục `ALL` → `File.Move` sang `.txt`. Tránh việc CG đọc file đang ghi dở.
3. Thư mục chưa tồn tại ⇒ `Directory.CreateDirectory`. Không ghi được (quyền/đĩa) ⇒ thông báo lỗi, **không** cập nhật trạng thái lấy mẫu.
4. Log Debug nội dung JSON (không chứa CCCD/SĐT) + `LogAction` "PrintBarcodeCG300".

### B.5 Thiết kế code

**File mới (partial UC, triple underscore):**

```
HIS.Desktop.Plugins.ConnectionTest/
├── UC_ConnectionTest___CG300.cs        ← xử lý quét 2 lần, gọi worker, cập nhật dòng, focus
├── CG300/
│   ├── Cg300LabelADO.cs                ← POCO FyLabelResponseModel (đúng tên trường của hãng)
│   ├── Cg300LabelBuilder.cs            ← build List<Cg300LabelADO> từ V_LIS_SAMPLE + mapping ống
│   ├── Cg300FileWriter.cs              ← ghi .tmp → .txt vào thư mục ALL
│   └── Cg300ConnectionChecker.cs       ← kiểm tra kết nối (config, thư mục ghi được, tiến trình CG)
├── Config/Cg300CFG.cs                  ← đọc 2 key ConfigApplication (thư mục, tên tiến trình CG)
├── Config/ControlStateConstant.cs      ← + key chkPrintBarcodeCG300
└── Resources/Lang.*.resx, Message.Lang.*.resx, ResourceMessage.cs ← chuỗi mới
```

**Điểm móc vào code hiện có:**

| Vị trí | Thay đổi |
|--------|----------|
| Load UC (sau `InitControlState`) | Gọi `RefreshConnectCG300()` → set `lciPrintBarcodeCG300.Visibility` |
| `UC_ConnectionTest.cs:2587` `txtSearchKey_KeyUp` | Khi `Enter`, checkbox **đang hiển thị** và `chkPrintBarcodeCG300.Checked` ⇒ gọi `ProcessScanCG300(code)` thay cho `FillDataToGridControl()` trực tiếp |
| `UC_ConnectionTest.cs:1567` `FillDataToGridControl` | Đã có `txtSearchKey.Focus(); SelectAll()` ⇒ đáp ứng bước 3, giữ nguyên |
| `UC_ConnectionTest_PlusPrint.cs:406` `PrintBarcodeByBartender` | Tách đoạn cập nhật "Đã lấy mẫu" (dòng 436–466) thành hàm dùng chung `UpdateSampleTakenAfterPrint(LisSampleADO)` để CG300 tái sử dụng, không nhân bản code |
| `UC_ConnectionTest___NhomXN.cs:79` | Đã gọi `api/LisSampleService/GetView` theo `SAMPLE_IDs` ⇒ cân nhắc tái dùng dữ liệu đã nạp nếu còn hợp lệ, tránh gọi API lặp |

**Luồng xử lý `ProcessScanCG300`:**

```csharp
private string lastScanCodeCG300;

private void ProcessScanCG300(string code)
{
    try
    {
        var data = gridControlSample.DataSource as List<LisSampleADO>;
        bool isPrint = !String.IsNullOrWhiteSpace(code)
            && String.Equals(code, lastScanCodeCG300, StringComparison.OrdinalIgnoreCase)
            && data != null && data.Any();

        if (isPrint)
            PrintBarcodeCG300(data);          // bước 2/4 — không reload lưới
        else
        {
            lastScanCodeCG300 = code;
            FillDataToGridControl();          // bước 1/3
        }
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
    finally
    {
        txtSearchKey.Focus();
        txtSearchKey.SelectAll();
    }
}
```

`PrintBarcodeCG300(List<LisSampleADO>)`:
0. `CheckConnectCG300()` — không đạt ⇒ thông báo `CG300MatKetNoi` kèm lý do, ẩn checkbox, dừng.
1. Lọc mẫu có `BARCODE` (mẫu thiếu ⇒ gom vào danh sách cảnh báo).
2. `WaitingManager.Show()` → nạp `LisSampleService` theo `SAMPLE_IDs` (1 lần gọi) → nạp cache TESA/TestSampleType nếu chưa có.
3. `Cg300LabelBuilder.Build(...)` → danh sách bản ghi + danh sách dịch vụ thiếu loại mẫu.
4. `Cg300FileWriter.Write(folder, labels)` → thành công thì `UpdateSampleTakenAfterPrint` cho từng mẫu Chưa LM/Từ chối, cập nhật dòng tại chỗ + `gridViewSample.RefreshData()`.
5. `WaitingManager.Hide()` → nếu có cảnh báo ⇒ hiện 1 thông báo tổng hợp (không chặn việc in các ống hợp lệ).

Trong chế độ CG300, `txtSearchKey_KeyUp` **chỉ tìm theo ô tìm kiếm**; các ô khác (mã điều trị, mã y lệnh…) giữ nguyên hành vi.

### B.6 Giao diện

```
Kết nối OK:     [Ô tìm kiếm ........] [☑ In tem CG300] [Tìm (Ctrl F)]
Không kết nối:  [Ô tìm kiếm ..........................] [Tìm (Ctrl F)]   ← checkbox ẩn (layout item Never)
```

- `lciPrintBarcodeCG300.Visibility` = `Always` khi `CheckConnectCG300()` = true, ngược lại `Never`; khi ẩn thì đồng thời coi như **không tích** (không đổi giá trị đã lưu trong ControlState).
- Checkbox + layout item được **tạo bằng code** (`CreateCG300Control`, không sửa Designer), đặt bên phải ô tìm kiếm, rộng cố định 95px; chỉ tạo khi config `...CG300_FOLDER` có giá trị.
- **Không dùng nút ⟳ riêng** (hàng tìm kiếm không đủ chỗ): kiểm tra lại kết nối khi **load UC**, khi bấm **Tìm (Ctrl F)**, và **trước mỗi lần ghi file**.
- `Cg300ConnectionChecker.Check()` trả `EnumCg300ConnectState`; lý do không đạt ghi log Warn và hiện trong thông báo "Mất kết nối" khi quét in.
- Caption/ToolTip khai báo trong `Lang.vi/en/my.resx`: `UC_ConnectionTest.chkPrintBarcodeCG300.Properties.Caption`, `.ToolTip` ("Quét lại cùng mã để in tem và nhả ống trên máy CG300").
- Khi bật: `txtSearchKey.Properties.NullValuePrompt` = "Quét barcode…" để KTV biết đang ở chế độ CG300.

### B.7 Thông báo mới (Message.Lang.vi/en/my)

| Key | Nội dung (vi) |
|-----|---------------|
| `CG300KhongGhiDuocFile` | Không ghi được dữ liệu sang máy CG300 vào thư mục {0}. Vui lòng kiểm tra cấu hình/quyền thư mục. |
| `CG300MauChuaCoBarcode` | Các mẫu chưa có barcode, không gửi sang CG300: {0} |
| `CG300DichVuChuaGanLoaiMau` | Dịch vụ chưa được gán loại mẫu bệnh phẩm (không xác định được ống): {0} |
| `CG300KhongCoDuLieuIn` | Không có dữ liệu hợp lệ để gửi sang CG300. |
| `CG300MatKetNoi` | Mất kết nối máy CG300: {0}. Vui lòng kiểm tra phần mềm CG và thư mục trao đổi. |
| `CG300LyDoThuMucKhongTonTai` / `CG300LyDoKhongGhiDuoc` / `CG300LyDoPhanMemChuaChay` | Thư mục {0} không tồn tại / Không có quyền ghi thư mục {0} / Phần mềm điều khiển CG300 chưa chạy |

### B.8 API sử dụng

| Mục đích | URI | Consumer | Filter |
|----------|-----|----------|--------|
| Tìm mẫu (có sẵn) | `api/LisSample/GetView` | LisConsumer | `LisSampleViewFilter.KEY_WORD` |
| Dịch vụ của mẫu | `api/LisSampleService/GetView` | LisConsumer | `LisSampleServiceViewFilter { SAMPLE_IDs }` |
| Dịch vụ ↔ loại mẫu | `api/HisServiceTesa/Get` | MosConsumer | `HisServiceTesaFilter { IS_ACTIVE = 1 }` — nạp 1 lần |
| Loại mẫu bệnh phẩm | `api/HisTestSampleType/Get` | MosConsumer | `HisTestSampleTypeFilter { IS_ACTIVE = 1 }` — nạp 1 lần |
| Cập nhật đã lấy mẫu (có sẵn) | `api/LisSample/Sample` | LisConsumer | `LisSampleSampleSDO` |

Không cần thay đổi backend / EFMODEL.

### B.9 Ảnh hưởng & rủi ro

| Hạng mục | Đánh giá |
|----------|----------|
| Máy trạm không bật CG300 | Không ảnh hưởng — nhánh code chỉ chạy khi checkbox bật |
| Nhánh Bartender / MPS000077 | Chỉ tách hàm dùng chung cập nhật lấy mẫu, cần test hồi quy in Bartender |
| Quét liên tục nhanh | Ghi file đồng bộ trên UI thread, dữ liệu nhỏ (<10 KB) — chấp nhận được; chặn quét chồng bằng cờ `isProcessingCG300` |
| CG không chạy / thư mục sai | File dồn trong `ALL`; HIS không biết được (Method 2 không phản hồi) ⇒ ghi rõ trong hướng dẫn vận hành |
| In lại | Quét lại mã lần 3 ⇒ sinh file lần nữa, `RePrint = Yes` — có thể nhả thêm ống (xem Q1) |

---

## C. CÂU HỎI CẦN CHỐT

| # | Câu hỏi | Đề xuất mặc định |
|---|---------|------------------|
| Q1 | Quét cùng mã **lần thứ 3** (sau khi đã in): in lại hay bỏ qua/cảnh báo? | In lại có cảnh báo xác nhận "Mẫu đã in tem, in lại?" — tránh nhả thừa ống |
| Q2 | Trường `Container` (màu ống) lấy từ đâu? `HIS_TEST_SAMPLE_TYPE` chỉ có Mã + Tên | Đặt **tên loại mẫu** theo dạng "Tím - EDTA" và lấy phần trước dấu `-` làm `Container`; hoặc để trống nếu máy chỉ dựa `ContainerCode` (cần hỏi hãng) |
| Q3 | Một dịch vụ gán nhiều loại mẫu bệnh phẩm thì chọn ống nào? | Sinh bản ghi cho **tất cả** loại mẫu được gán (mỗi loại một ống) |
| Q4 | Mẫu chưa có barcode (chưa sinh): tự sinh barcode trước khi gửi CG? | Không tự sinh, cảnh báo — vì barcode do LIS sinh theo cấu hình `IS_AUTO_CREATE_BARCODE` |
| Q5 | Encoding / đuôi file / tên file mà phần mềm CG yêu cầu (UTF-8 có BOM? `.txt`?) | UTF-8 không BOM, `.txt` — cần xác nhận với hãng/kỹ sư lắp đặt |
| Q6 | Mã quét ở bước 1 là **barcode mẫu** hay **mã y lệnh / phiếu chỉ định**? | Hỗ trợ cả hai (KEY_WORD); bước 2 so khớp chuỗi đã quét nên không phụ thuộc loại mã |
| Q7 | `HospNum` in mã BN hay mã điều trị? (tem thực tế BV in "260513.1161") | Mã BN (`PATIENT_CODE`) theo định nghĩa của hãng |
| Q8 | **Tên tiến trình** (file .exe) của phần mềm điều khiển CG300 trên máy trạm? | Cần lấy tại viện (Task Manager) — dùng cho điều kiện kết nối T.6 #3 |
| Q9 | Đường dẫn thư mục `ALL` thực tế mà phần mềm CG đang đọc? | Cấu hình theo máy trạm; tài liệu hãng ghi "dưới thư mục chương trình HIS/LIS" |
| Q10 | Ống mã `002` (Green) trên ảnh thực tế không có trong bảng `LisDG.docx` | Đối chiếu lại bảng khay trên máy CG |

---

## D. TEST CASES

- [ ] Máy trạm không cấu hình thư mục CG300 ⇒ không thấy checkbox.
- [ ] Có cấu hình nhưng thư mục không tồn tại / không ghi được / phần mềm CG chưa chạy ⇒ checkbox ẩn, LogSystem ghi đúng lý do.
- [ ] Bật phần mềm CG rồi bấm Tìm ⇒ checkbox hiện, khôi phục trạng thái tích lần trước.
- [ ] Đang dùng thì tắt phần mềm CG, quét in ⇒ báo "Mất kết nối", checkbox ẩn, mẫu không đổi trạng thái.
- [ ] Bỏ tích chế độ CG300 ⇒ Enter ở ô tìm kiếm hoạt động như cũ, không sinh file.
- [ ] Bật CG300, quét mã A ⇒ lưới hiện mẫu A, con trỏ ở ô tìm kiếm, nội dung bôi đen.
- [ ] Quét lại A ⇒ sinh 1 file trong `ALL`, số bản ghi = số ống khác nhau của các mẫu; lưới không reload; mẫu Chưa LM chuyển Đã LM.
- [ ] Quét mã B ngay sau đó ⇒ ghi đè ô tìm kiếm và tìm B (không in A).
- [ ] Mẫu có 3 dịch vụ thuộc 2 loại mẫu (001, 006) ⇒ 2 bản ghi, `TestItemDesc` gộp đúng dịch vụ theo ống.
- [ ] Dịch vụ chưa gán loại mẫu ⇒ cảnh báo tên dịch vụ, các ống hợp lệ vẫn được gửi.
- [ ] Mẫu chưa có barcode ⇒ cảnh báo, không sinh bản ghi.
- [ ] Thư mục `ALL` không ghi được ⇒ thông báo lỗi, trạng thái mẫu không đổi.
- [ ] File JSON mở bằng Notepad hiển thị đúng tiếng Việt; CG300 đọc được và tự xoá file.
- [ ] Ký tự đặc biệt trong tên BN (dấu nháy, `&`) ⇒ JSON hợp lệ.
- [ ] Hồi quy: In barcode Bartender và MPS000077 vẫn cập nhật trạng thái lấy mẫu như trước.
- [ ] Trạng thái checkbox CG300 được nhớ sau khi đóng/mở lại chức năng.
