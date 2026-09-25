# Xử lý dịch vụ — bản TestServiceExecute — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TestServiceExecute |
| Loại | UserControl (`UCServiceExecute`, kế thừa `HIS.Desktop.Utility.UserControlBase`) |
| Mục đích | Màn "Xử lý dịch vụ" tại phòng thực hiện: chọn mẫu mô tả (`HIS_SERE_SERV_TEMP`), nhập mô tả / kết luận / ghi chú / số phim, chọn máy và thời gian bắt đầu - kết thúc, lấy kết quả chỉ số từ LIS, xem ảnh từ PACS; Lưu (`HIS_SERE_SERV_EXT`), Kết thúc y lệnh, In phiếu trả kết quả, Ký số EMR. |
| Người tạo | IVT |
| Ngày tạo tài liệu | 23/09/2026 |
| Trạng thái | Bảo trì |

### ⚠ Bẫy: TRÙNG TÊN HIỂN THỊ với `HIS.Desktop.Plugins.ServiceExecute`

Đây là **plugin thứ hai** cũng tên "Xử lý dịch vụ". Hai plugin giống nhau gần như toàn bộ phần khai báo module nên rất dễ nhầm khi nhận việc / sửa lỗi:

| Thuộc tính `[ExtensionOf]` | HIS.Desktop.Plugins.ServiceExecute | HIS.Desktop.Plugins.**TestServiceExecute** |
|---------------------------|------------------------------------|--------------------------------------------|
| Tên hiển thị | "Xử lý dịch vụ" | "Xử lý dịch vụ" (**trùng**) |
| Nhóm / Order | Common / 16 | Common / 16 (**trùng**) |
| Icon | `weightedpies_32x32.png` | `weightedpies_32x32.png` (**trùng**) |
| Phím gợi nhớ | "E" | "E" (**trùng**) |
| Loại module | `Module.MODULE_TYPE_ID__UC` | `Module.MODULE_TYPE_ID__UC` (**trùng**) |
| Tên class Processor / UC | `ServiceExecuteProcessor` / `UCServiceExecute` | `ServiceExecuteProcessor` / `UCServiceExecute` (**trùng tên class**, chỉ khác namespace + assembly) |
| API kết thúc y lệnh | POST `api/HisServiceReq/FinishWithTime` | POST **`api/HisServiceReq/Finish`** (không truyền thời gian) |
| Phạm vi mẫu mô tả | theo loại dịch vụ của y lệnh | lọc mẫu theo `SERVICE_TYPE_ID` = **XN** (`UCServiceExecute.cs:107, 1674`) |
| Đặc thù | Customize Layout, `chkSendExt`, kiểm tra máy CLS khi lưu… | Lấy kết quả chỉ số từ **LIS** (`api/LisSample/GetView`, `api/LisResult/GetView`), xem ảnh **PACS**, tô màu giá trị bất thường |

Hệ quả khi làm việc:

- Trên menu / phân quyền (ACS) hai module hiện **cùng một tên**, chỉ phân biệt được bằng `ModuleLink`. Khi viện báo lỗi ở màn "Xử lý dịch vụ" **phải hỏi/kiểm tra viện đang khai module nào** trước khi sửa.
- Mọi rà soát toàn hệ thống theo kiểu "tìm nút Kết thúc gọi `FinishWithTime`" sẽ **bỏ sót plugin này** (nó gọi `Finish`). Đây chính là lý do việc 3353 lần rà đầu (18/09/2026) chỉ chèn 4 màn, đến 22/09/2026 rà lại mới ra màn thứ 5 (xem mục 8).
- Khi deploy DLL phải phân biệt rõ hai file cùng chức năng: `HIS.Desktop.Plugins.ServiceExecute.dll` và `HIS.Desktop.Plugins.TestServiceExecute.dll`.

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

```
1. Phòng thực hiện (ExecuteRoom) → chọn bệnh nhân / y lệnh → "Xử lý"
2. Processor.Run(args) → ServiceExecuteFactory → ServiceExecuteBehavior
   → lấy Module + V_HIS_SERVICE_REQ (hoặc HIS.Desktop.ADO.ServiceExecuteADO có kèm DelegateRefresh)
   → tạo UCServiceExecute(moduleData, serviceReq, RefreshData)
3. UCServiceExecute_Load: LoadKeysFromlanguage → SetDefaultValueControl
   → CreateThreadLoadDataDefault (4 thread song song: LoadCurrentServiceReq,
     ProcessLoadListTemplate, ProcessDataForTemplate, LoadTreatmentWithPaty)
   → FillDataCombo → FillDataToGrid → SetDisable → ValidBeginTime/ValidEndTime
4. FillDataToGrid: api/HisSereServ/Get theo SERVICE_REQ_ID → List<ADO.ServiceADO>
   (ServiceADO : HIS_SERE_SERV) → bind lưới dịch vụ → chọn dòng đầu tiên
5. Chọn dòng dịch vụ (SereServClickRow) → nạp HIS_SERE_SERV_EXT, mẫu mô tả,
   nội dung SAR_PRINT, ảnh PACS, tiền (bill / tạm ứng / hoàn ứng) để biết đã thanh toán chưa
6. Chọn mẫu (cboSereServTemp / txtSereServTempCode) → đổ nội dung vào txtDescription
   (RichEditControl) + thay các key dữ liệu trong dicParam / dicImage
   (kết quả chỉ số lấy từ LIS: api/LisSample/GetView, api/LisResult/GetView)
7. Lưu (SaveProcessor) → api/HisSereServExt/CreateSdo | UpdateSdo
   → bật nút In, nút Ký số EMR; "Lưu và đóng" thì hỏi in rồi tự Kết thúc
8. Kết thúc (btnFinish_Click) → kiểm tra đã xử lý hết dịch vụ
   → *(việc 3353)* CheckRequireMediMateManager.CheckBeforeFinish(listServiceADO)
   → POST api/HisServiceReq/Finish
9. In phiếu trả kết quả (RichEditor) / Ký số EMR (EMR.Desktop.Plugins.EmrSign)
```

### Các lối vào hàm Kết thúc

| Lối vào | Vị trí | Ghi chú |
|---------|--------|---------|
| Nút "Kết thúc" | `btnFinish_Click` (`UCServiceExecute.cs:2858`) | Lối chính |
| Phím tắt Ctrl+E | `KeyboardWorker.cs` (`[KeyboardAction("End", …, XKeys.Control \| XKeys.E)]`) → `UCServiceExecute.End()` (`:4506`) | `End()` chỉ gọi lại `btnFinish_Click` khi nút đang bật (`:4511`) |
| Tự kết thúc sau "Lưu và đóng" | `SaveProcessor` (`:3129`), `SaveAllProcess` (`:3266`) | Hỏi in xong thì gọi `btnFinish_Click` |

→ Chỉ cần chèn kiểm tra **một điểm** trong `btnFinish_Click` là phủ hết mọi lối vào.

### Điều kiện nghiệp vụ

- Y lệnh đã hoàn thành (`SERVICE_REQ_STT_ID = HT`) → `SetDisable()` khóa toàn bộ nút Lưu / Kết thúc / Kê đơn / chọn mẫu.
- Dịch vụ chưa thanh toán (`isNoPay`) → không cho Lưu, không cho In.
- Phải xử lý (lưu) **hết** các dịch vụ trong y lệnh mới được Kết thúc; còn dịch vụ chưa lưu → thông báo `ChuaXuLyHetDichVu` và dừng (`:2878-2890`).
- Thời gian bắt đầu / kết thúc thực hiện được validate bằng `BeginTimeValidationRule` / `EndTimeValidationRule`; số phim validate bằng `FilmValidationRule` (chỉ nhận số).
- Khi khóa viện phí: key `MOS.HIS_SERVICE_REQ.IS_ALLOWING_PROCESSING_SUBCLINICAL_AFTER_LOCKING_TREATMENT = 1` và hồ sơ `IS_LOCK_FEE = 1` → khóa nút "Chỉ định dịch vụ" và "Kê tủ trực".
- **Việc 3353 (PT-56272 / 57799) — mức CHẶN, áp dụng từ 22/09/2026:** dịch vụ đã tick "Có thuốc, vật tư đi kèm" (`HIS_SERVICE.IS_REQUIRE_MEDI_MATE = 1`) mà **chưa có** dòng `HIS_SERE_SERV` con loại Thuốc / Vật tư (`PARENT_ID` = ID dịch vụ, chưa hủy, không "không thực hiện") → hiện thông báo **chỉ có nút OK**: *"Không kết thúc được. Dịch vụ chưa có thuốc, vật tư đi kèm:\n{danh sách}\nVui lòng kê thuốc, vật tư đi kèm cho các dịch vụ trên rồi kết thúc lại."* Thư viện luôn trả `false` → `btnFinish_Click` **`return` ngay, KHÔNG gọi `api/HisServiceReq/Finish`**. Không có nút bỏ qua, không có key config. Đường thoát duy nhất là **kê bổ sung thuốc/vật tư cho dịch vụ rồi Kết thúc lại**.
- **Fail-open (việc 3353):** lỗi API / mất mạng / Backend cũ chưa có cột `IS_REQUIRE_MEDI_MATE` → thư viện ghi `LogSystem.Warn` và trả `true` → **không chặn**, y lệnh kết thúc bình thường (sự cố kỹ thuật không được khóa phòng thực hiện).

### Đường kê bổ sung để gỡ chặn (việc 3353)

| Nút trên màn này | Plugin mở | Có gắn dịch vụ (`PARENT_ID`)? | Gỡ được chặn? |
|------------------|-----------|-------------------------------|---------------|
| "Kê tủ trực" (`btnTuTruc_Click`, `:2963`) | HIS.Desktop.Plugins.AssignPrescriptionCLS | **Có** — truyền `V_HIS_SERE_SERV sereServInput` vào `AssignPrescriptionADO(TREATMENT_ID, 0, SERVICE_REQ_ID, sereServInput)` (`:2972`) | **Có** |
| "Kê đơn" (`btnAssignPrescription_Click`, `:2681`) | HIS.Desktop.Plugins.AssignPrescriptionPK | **Không** — `AssignPrescriptionADO(TREATMENT_ID, 0, 0)` (`:2688`), thuốc kê ra không gắn dịch vụ | **Không** |

→ Khi bị chặn, phải hướng dẫn người dùng kê bằng nút **"Kê tủ trực"** (hoặc chuột phải "Kê đơn cận lâm sàng" ở màn Xử lý y lệnh), không phải nút "Kê đơn".

### Key cấu hình (HIS_CONFIG)

| Key | Tác dụng |
|-----|----------|
| `HIS.Desktop.Plugins.TestServiceExecute.ThoiGianKetThuc` | Chuỗi key thời gian kết thúc trong template in — khi in sẽ thay bằng `FINISH_TIME` thật của y lệnh |
| `HIS.Desktop.Plugins.TestServiceExecute.HideTimePrint` | Ẩn thời gian trên bản in |
| `HIS.Desktop.Plugins.TestServiceExecute.ConnectPacsByFss` | `= 1`: tải ảnh PACS qua FSS; khác: đọc ảnh theo đường dẫn chia sẻ của server PACS |
| `HIS.Desktop.Plugins.TestServiceExecute.OptionImage` | `= 1`: bật lấy ảnh từ PACS theo `TDL_PACS_TYPE_CODE` |
| `MOS.HIS_SERVICE_REQ.IS_ALLOWING_PROCESSING_SUBCLINICAL_AFTER_LOCKING_TREATMENT` | Cho xử lý CLS sau khi khóa viện phí (khóa 2 nút chỉ định / kê tủ trực) |

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ / V_HIS_SERVICE_REQ | Table / View | Y lệnh đang xử lý (`currentServiceReq`, `ServiceReqConstruct`); ID gửi lên `api/HisServiceReq/Finish` |
| HIS_SERE_SERV | Table | Dịch vụ của y lệnh — bọc trong `ADO.ServiceADO : HIS_SERE_SERV` (`listServiceADO`) |
| HIS_SERE_SERV_EXT | Table | Dữ liệu xử lý: mô tả, kết luận, ghi chú, số phim, máy, thời gian bắt đầu/kết thúc |
| HIS_SERE_SERV_TEMP | Table | Mẫu mô tả (lọc `IS_ACTIVE = 1`, theo giới tính, `SERVICE_TYPE_ID` = XN hoặc không khai) |
| HIS_MACHINE / HIS_SERVICE_MACHINE | Table | Máy thực hiện và ánh xạ dịch vụ - máy |
| V_HIS_TEST_INDEX / V_HIS_TEST_INDEX_RANGE | View | Chỉ số xét nghiệm và khoảng tham chiếu (tô cảnh báo cao/thấp) |
| V_LIS_SAMPLE / V_LIS_RESULT | View (LIS) | Mẫu và kết quả chỉ số lấy sang từ hệ thống LIS |
| SAR_PRINT | Table (SAR) | Nội dung mô tả đã lưu dạng RTF (`DESCRIPTION_SAR_PRINT_ID`) |
| EMR_DOCUMENT | Table (EMR) | Tài liệu bệnh án điện tử phục vụ ký số |
| HIS_SERE_SERV_BILL / HIS_SERE_SERV_DEPOSIT / HIS_SESE_DEPO_REPAY | Table | Xác định dịch vụ đã thanh toán / tạm ứng (cờ `isNoPay`) |
| HIS_TREATMENT + HisTreatmentWithPatientTypeInfoSDO | Table / SDO | Hồ sơ điều trị, đối tượng BHYT, cờ `IS_LOCK_FEE` |
| HIS_SERVICE (qua thư viện 3353) | Table | Cờ `IS_REQUIRE_MEDI_MATE` — thư viện `CheckRequireMediMate` tự đọc, plugin không khai báo |

### Quan hệ chính

- HIS_SERVICE_REQ → HIS_SERE_SERV (1-n, qua `SERVICE_REQ_ID`)
- HIS_SERE_SERV → HIS_SERE_SERV_EXT (1-1, qua `SERE_SERV_ID`)
- HIS_SERE_SERV → HIS_SERE_SERV con loại Thuốc/Vật tư (1-n, qua `PARENT_ID`) — cơ sở kiểm tra của việc 3353

## 4. UI Layout

### Sơ đồ giao diện

```
+---------------------------------------------------------------------------+
| Thông tin bệnh nhân / y lệnh (mã, tên, tuổi, giới, chẩn đoán)             |
+---------------------------------------------------------------------------+
| Lưới dịch vụ (gridControlSereServ / gridViewSereServ)                     |
|  Số phiếu | Mã DV | Tên DV | Máy | Người TH | Vai trò | [VT] | [Gửi SA]   |
+-------------------------------+-------------------------------------------+
| Mã mẫu (txtSereServTempCode)  | Ảnh PACS / ảnh đính kèm                   |
| Mẫu (cboSereServTemp) [...]   | (cardControl + tileView1)                 |
|-------------------------------|                                           |
| Mô tả (txtDescription —       | [Chọn ảnh] [Đổi ảnh] [Xóa ảnh]            |
|  RichEditControl)             |                                           |
|-------------------------------+-------------------------------------------+
| Kết luận (txtConclude) | Ghi chú (txtNote) | Số phim (txtNumberOfFilm)    |
| TG bắt đầu (dtBeginTime)      | TG kết thúc (dtEndTime)                   |
+---------------------------------------------------------------------------+
| [Chỉ định DV F9] [Kê đơn F8] [Kê tủ trực] [DS mẫu] [Ký số EMR]            |
| [Lưu Ctrl+S] [Lưu & đóng] [Kết thúc Ctrl+E] [In Ctrl+P] [Phẫu thuật]      |
+---------------------------------------------------------------------------+
```

### Control chính

| Control | Kiểu | Mục đích |
|---------|------|----------|
| `gridControlSereServ` / `gridViewSereServ` | GridControl | Danh sách dịch vụ của y lệnh (nguồn `listServiceADO`) |
| `txtSereServTempCode` / `cboSereServTemp` | TextEdit / GridLookUpEdit | Tìm và chọn mẫu mô tả |
| `txtDescription` | `DevExpress.XtraRichEdit.RichEditControl` | Nội dung mô tả (RTF, lưu qua SAR_PRINT) |
| `txtConclude`, `txtNote`, `txtNumberOfFilm` | MemoEdit / TextEdit | Kết luận, ghi chú, số phim |
| `dtBeginTime`, `dtEndTime` | DateEdit | Thời gian bắt đầu / kết thúc thực hiện |
| `cardControl` / `tileView1` | GridControl / TileView | Ảnh PACS, ảnh đính kèm |
| `btnSave`, `btnSaveNClose`, `btnFinish`, `btnPrint`, `BtnEmr` | SimpleButton | Lưu / Lưu & đóng / **Kết thúc** / In / Ký số |
| `btnAssignService`, `btnAssignPrescription`, `btnTuTruc`, `btnSereServTempList`, `btnAssignPaan` | SimpleButton | Mở các plugin liên quan (xem mục 6) |
| `barManager1` | BarManager | Popup menu "In" cho dịch vụ siêu âm (qua `FormOtherSereServ`) |

### UC sử dụng

Không dùng `HIS.UC.*` — toàn bộ là control DevExpress 15.2 + `RichEditControl`. Đa ngôn ngữ nạp trong `LoadKeysFromlanguage()` theo key `IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__*` (`Resources/Lang.vi.resx`, `Lang.en.resx`); thông báo riêng nằm ở `Message.Lang.vi/en.resx` + `ResourceMessage.cs`.

### Phím tắt (`KeyboardWorker.cs`)

| Phím | Method |
|------|--------|
| Ctrl+S | `Save()` |
| **Ctrl+E** | `End()` → `btnFinish_Click` (đi qua kiểm tra 3353) |
| Ctrl+P | `Print()` |
| F9 | `AssignService()` |
| F8 | `AssignPre()` |

## 5. API Endpoints

| Action | URI | Consumer | Filter / Input |
|--------|-----|----------|----------------|
| Lấy dịch vụ của y lệnh | `ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET` (api/HisSereServ/Get) | MosConsumer | `HisSereServFilter.SERVICE_REQ_ID` + `IS_ACTIVE` |
| Lấy y lệnh | `RequestUriStore.HIS_SERVICE_REQ_GET` (api/HisServiceReq/Get) | MosConsumer | `HisServiceReqFilter.ID` |
| Lấy hồ sơ + đối tượng | `RequestUriStore.HIS_TREATMENT_GET_TREATMENT_WITH_PATIENT_TYPE_INFO_SDO` | MosConsumer | `HisTreatmentWithPatientTypeInfoFilter` |
| Lấy dữ liệu xử lý | `RequestUriStore.HIS_SERE_SERV_EXT_GET` (api/HisSereServExt/Get) | MosConsumer | `HisSereServExtFilter` |
| **Lưu dữ liệu xử lý** | `RequestUriStore.HIS_SERE_SERV_EXT_CREATE_SDO` / `..._UPDATE_SDO` | MosConsumer | `HisSereServExtSDO` |
| **Kết thúc y lệnh** | `RequestUriStore.HIS_SERVICE_REQ_FINISH` = **api/HisServiceReq/Finish** | MosConsumer | `currentServiceReq.ID` (POST, **không** truyền thời gian) |
| Tiền / tạm ứng / hoàn ứng | api/HisSereServBill/Get, api/HisSereServDeposit/Get, api/HisSeseDepoRepay/Get | MosConsumer | theo `SERE_SERV_ID` |
| Nội dung mô tả đã lưu | `ApiConsumer.SarRequestUriStore.SAR_PRINT_GET` | SarConsumer | `SarPrintFilter.IDs` |
| Kết quả xét nghiệm từ LIS | api/LisSample/GetView, api/LisResult/GetView | **LisConsumer** | `LisSampleFilter`, `LisResultViewFilter` |
| Ảnh PACS | `RequestUriStore.PACS_SERIVCE__LAY_THONG_TIN_ANH` (api/His/LayThongTinHinhAnh) | `PacsApiConsumer` (raw) | số phiếu (`SoPhieu`) |
| Tài liệu EMR để ký | `EMR.URI.EmrDocument.GET` | EmrConsumer | `EmrDocumentFilter` |
| *(thư viện 3353)* cờ dịch vụ / dòng thuốc-VT con | api/HisService/Get, api/HisSereServ/Get | MosConsumer | `HisServiceFilter.IDs`; `HisSereServFilter.PARENT_IDs` + `TDL_SERVICE_TYPE_IDs` |

## 6. Dependencies

### Library Plugins

| Library | Mục đích |
|---------|----------|
| **HIS.Desktop.Plugins.Library.CheckRequireMediMate** | **Việc 3353 — CHẶN kết thúc khi dịch vụ đã tick "Có thuốc, vật tư đi kèm" mà chưa kê thuốc/vật tư** (tham chiếu HintPath `..\..\..\..\lib\HIS\HIS.Desktop.Plugins.Library.CheckRequireMediMate\...dll`, csproj dòng 87-89) |
| HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT | Cảnh báo viện phí với đối tượng không BHYT trước khi chỉ định dịch vụ (`:2649`) |
| HIS.Desktop.Plugins.Library.FormOtherSereServ | Menu "In" mở rộng cho dịch vụ siêu âm (`FormOtherProcessor`, `:2758`, `:4328`) |
| Inventec.Common.SignLibrary / Inventec.Common.RichEditor | Ký số, xử lý nội dung RichText |
| Inventec.Fss.Client | Tải ảnh PACS qua FSS |

### Inter-Plugin

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.AssignService | Nút "Chỉ định dịch vụ" (F9) | `AssignServiceADO` + Module (`:2656`, `:2669`) |
| HIS.Desktop.Plugins.AssignPrescriptionPK | Nút "Kê đơn" (F8) | `AssignPrescriptionADO(TREATMENT_ID, 0, 0)` — **không gắn dịch vụ** (`:2708`) |
| HIS.Desktop.Plugins.AssignPrescriptionCLS | Nút "Kê tủ trực" | `AssignPrescriptionADO(..., sereServInput)`, `IsCabinet = true` — **có gắn dịch vụ** (`:2981`) |
| HIS.Desktop.Plugins.SereServTemplate | Nút "Danh sách mẫu" | danh sách `SERVICE_TYPE_IDs` (`:2734`) |
| HIS.Desktop.Plugins.HisServiceReqMaty | Nút vật tư trên lưới dịch vụ | y lệnh + dịch vụ (`:2998`) |
| HIS.Desktop.Plugins.AssignPaan | Nút phẫu thuật/thủ thuật (`btnAssignPaan_Click`) | `TREATMENT_ID` + Module, mở dạng `ShowDialog` |
| EMR.Desktop.Plugins.EmrSign | Nút Ký số EMR | `EMR_DOCUMENT` + `DocumentTDO` (`:3406`) |

## 7. Print

| Loại in | Cơ chế | Template |
|---------|--------|----------|
| Phiếu trả kết quả dịch vụ | `PrintResult(bool printNow)` — dựng `DevExpress.XtraRichEdit.RichEditControl`, thay các key dữ liệu từ `dicParam` / `dicImage`, thay key `ThoiGianKetThuc` bằng `FINISH_TIME` thật của y lệnh | Mẫu `HIS_SERE_SERV_TEMP` người dùng chọn (nội dung lưu ở `SAR_PRINT`) |
| In / Xem trước | `printNow = true` hoặc key `CheDoInChoCacChucNangTrongPhanMem = 2` → `printDocument.Print()`; còn lại → `ShowPrintPreview()` | — |
| In mở rộng cho siêu âm | `sereServ.TDL_SERVICE_TYPE_ID = SA` → popup menu từ `FormOtherSereServ.FormOtherProcessor.GetBarButtonItem(barManager1)` | Theo cấu hình của thư viện |
| Ký số EMR | `BtnEmr_Click` → theo key `HIS.Desktop.Plugins.Library.EmrGenerate.SignType` (1 / 2) → mở `EMR.Desktop.Plugins.EmrSign` | Tài liệu EMR tương ứng |

Không dùng MPS processor — toàn bộ in ở màn này đi qua RichEditor + mẫu dịch vụ.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 23/09/2026 | dangth2 | Tạo tài liệu module (9 mục) cho plugin `HIS.Desktop.Plugins.TestServiceExecute` — trước đó plugin chưa có docs. Ghi rõ bẫy **trùng tên hiển thị "Xử lý dịch vụ"** với `HIS.Desktop.Plugins.ServiceExecute` (trùng cả nhóm Common, order 16, icon `weightedpies_32x32.png`, phím "E", tên class `ServiceExecuteProcessor` / `UCServiceExecute`), khác nhau ở chỗ màn này POST `api/HisServiceReq/Finish` còn `ServiceExecute` POST `FinishWithTime`. |
| 22/09/2026 | dangth2 | **Việc 3353 (PT-56272 / 57799) — chèn kiểm tra CHẶN "Dịch vụ chưa có thuốc, vật tư đi kèm" khi kết thúc.** Rà lại theo chỉ đạo anh Cảnh *"chặn hết ở màn hình xử lí nhé"* (CLS + PTTT + XN) thì phát hiện màn này bị sót ở lần chèn 18/09 vì nút Kết thúc gọi thẳng `api/HisServiceReq/Finish`, không qua `FinishWithTime`. Thêm 1 dòng vào `btnFinish_Click` (`UCServiceExecute.cs:2894`), đặt **sau** khối kiểm tra `isSave` / `ChuaXuLyHetDichVu` (kết thúc dòng 2890) và **trước** POST `RequestUriStore.HIS_SERVICE_REQ_FINISH` (dòng 2899): `if (!HIS.Desktop.Plugins.Library.CheckRequireMediMate.CheckRequireMediMateManager.CheckBeforeFinish(listServiceADO)) return;`. `listServiceADO` là `List<ADO.ServiceADO>` mà `ServiceADO : HIS_SERE_SERV` nên dùng đúng overload sẵn có, **không phải sửa thư viện**. Một điểm chèn phủ luôn Ctrl+E và các lối tự kết thúc sau Lưu (`:3129`, `:3266`, `:4511`). Thông báo **chỉ có nút OK**, thư viện luôn trả `false` → màn `return`, không gọi Finish; giữ **fail-open** khi lỗi API/Backend cũ. Thêm `<Reference Include="HIS.Desktop.Plugins.Library.CheckRequireMediMate">` (HintPath `lib\HIS`) vào csproj (dòng 87-89); không thêm message vào resx riêng (chuỗi nằm trong thư viện), không key config. Commit `359ff8013`; DLL lên test HISTEST `052c55c03`. Thiết kế: `PTTK\3353 - Thiet ke - Canh bao dich vu chua co thuoc vat tu di kem khi ket thuc thuc hien.md`. |

## 9. Test Cases

### Kết thúc y lệnh (hồi quy)

- [ ] Y lệnh còn dịch vụ chưa lưu → thông báo "Chưa xử lý hết dịch vụ", không kết thúc (như cũ).
- [ ] Lưu hết dịch vụ (dịch vụ **không** tick cờ) → Kết thúc thành công, nút Lưu/Kết thúc tự khóa, lưới danh sách ngoài phòng thực hiện refresh.
- [ ] Ctrl+E và "Lưu và đóng" (tự kết thúc sau khi hỏi in) → hành vi giống nút Kết thúc.

### Việc 3353 — chặn khi thiếu thuốc, vật tư đi kèm

- [ ] Dịch vụ tick "Có thuốc, vật tư đi kèm", chưa kê → bấm Kết thúc → thông báo liệt kê đúng "- Mã - Tên" dịch vụ, **chỉ có nút OK**; bấm OK xong y lệnh **vẫn chưa hoàn thành** (log không có request `api/HisServiceReq/Finish`).
- [ ] Bị chặn → dùng nút **"Kê tủ trực"** kê 1 thuốc hoặc 1 vật tư cho dịch vụ → Kết thúc lại → hoàn thành bình thường.
- [ ] Bị chặn → kê bằng nút **"Kê đơn"** (AssignPrescriptionPK, không gắn dịch vụ) → **vẫn bị chặn** (đúng thiết kế, cần hướng dẫn người dùng).
- [ ] Y lệnh có 2 dịch vụ cùng thiếu → **một** thông báo liệt kê đủ cả 2; không dịch vụ nào được kết thúc.
- [ ] Dịch vụ tick cờ nhưng đã có dòng thuốc/vật tư con (kể cả hao phí `IS_EXPEND = 1`) → không chặn.
- [ ] Đã kê rồi hủy đơn → bị chặn lại.
- [ ] Dịch vụ tick "Không thực hiện" (`IS_NO_EXECUTE = 1`) → bỏ qua, không chặn.
- [ ] Ctrl+E và luồng "Lưu và đóng" khi còn dịch vụ thiếu → **cũng bị chặn** (một điểm chèn phủ hết).
- [ ] Backend cũ chưa có cột `IS_REQUIRE_MEDI_MATE` / rút mạng khi kiểm tra → **không chặn**, kết thúc bình thường, log `Warn` (fail-open).
- [ ] Đổi ngôn ngữ sang `en` → thông báo tiếng Anh ("Cannot finish. The following services have no accompanying medicine/material…"); thiếu satellite `en` → hiện tiếng Việt.

### Chức năng khác (hồi quy sau khi thêm reference thư viện)

- [ ] Chọn mẫu mô tả, đổ dữ liệu chỉ số từ LIS, tô màu giá trị vượt ngưỡng → như cũ.
- [ ] Lưu (`api/HisSereServExt/CreateSdo` / `UpdateSdo`) → bật nút In và nút Ký số EMR.
- [ ] In phiếu trả kết quả: thời gian kết thúc trên bản in lấy đúng `FINISH_TIME`; dịch vụ siêu âm hiện popup menu in mở rộng.
- [ ] Xem ảnh PACS với `OptionImage = 1` (cả 2 chế độ `ConnectPacsByFss = 1` và đọc thư mục chia sẻ).
- [ ] Y lệnh đã hoàn thành mở lại → toàn bộ nút bị khóa, không phát sinh request kiểm tra 3353.
