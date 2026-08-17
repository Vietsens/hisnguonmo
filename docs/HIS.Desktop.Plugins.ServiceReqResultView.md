# HIS.Desktop.Plugins.ServiceReqResultView — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ServiceReqResultView |
| Tên hiển thị | Tờ điều trị |
| Loại | Form (`HIS.Desktop.Utility.FormBase`) |
| Mục đích | Xem kết quả của **một dịch vụ** (`HIS_SERE_SERV`): mô tả – kết luận, văn bản EMR đã ký (PDF), hình ảnh PACS; in phiếu kết quả và ký số EMR |
| Người tạo | IVT |
| Ngày cập nhật gần nhất | 12/08/2026 |
| Trạng thái | Bảo trì |

Không có menu độc lập theo nghiệp vụ thường dùng — màn hình được **mở từ plugin khác** với `sereServId`.

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

```
Plugin cha (danh sách y lệnh / cây dịch vụ / phòng thực hiện)
  → ShowModule("HIS.Desktop.Plugins.ServiceReqResultView", { Module, sereServId [, viewLink] })
    → Behavior.Run() → frmServiceReqResultView(Module, sereServId, viewLinkPacs)
      → Load:
         1. LoadDataBySereServId  → V_HIS_SERE_SERV_4 + HIS_SERE_SERV_EXT + HIS_SERVICE_REQ
         2. 3 tác vụ song song: treatment+đối tượng, bệnh nhân, kiểm tra văn bản EMR
         3. Có văn bản EMR → ghép PDF nhiều văn bản → pdfViewer1 (tab PDF)
         4. Không có → hiển thị mô tả/kết luận trên RichEdit (tab Tài liệu)
         5. Chèn ảnh chữ ký theo GEN_SIGNATURE_BY_KEY_CFG
         6. Xác định link xem ảnh PACS → tab PACS (webView1 — EO.WebBrowser)
      → In phiếu (btnPrint / menu PTTT) hoặc Ký số EMR (BtnEmr)
```

### Xác định link xem ảnh PACS (cập nhật 12/08/2026)

Thứ tự ưu tiên trong `SereServClickRow` và `OpenWebFromConfig`:

| # | Điều kiện | Nguồn link |
|---|-----------|-----------|
| 1 | Plugin cha truyền `viewLinkPacs` (link http) | Dùng luôn — không gọi API lấy link, không phụ thuộc `MOS.PACS.ADDRESS` |
| 2 | `MOS.PACS.ADDRESS` của phòng thực hiện có `CloudInfo` | `api/HisSereServExt/GetLinkResult` (HL7/WCF — PACS Sancy), chạy thread riêng |
| 3 | Bản ghi có `Api` | `JSON_FORM_ID` nếu là link http, ngược lại dựng URL từ `Api` + thay `:idChiDinh` / `:idBenhNhan` / `:idDotVaoVien` / `<#PACS_BASE_URI;>` |
| 4 | Không có `Api`, hoặc phòng không có trong `MOS.PACS.ADDRESS` | `HIS_SERE_SERV_EXT.JSON_FORM_ID` nếu là link http (PACS đẩy sang — VD Carestream); vẫn không có mới báo "Chưa cấu hình địa chỉ xem kết quả" |

- Nhánh 2 đặt cờ `isGettingLinkFromPacs` → **không** dùng link cũ đã lưu, tránh hiển thị link hết hạn rồi bị ghi đè.
- Khi mở với mục đích xem ảnh (`isOpenForViewImage`): form chọn sẵn tab PACS và **không** auto-print + Close theo `HIS.Desktop.Plugins.ServiceExecute.PrintOption`.

### Điều kiện nghiệp vụ

- Bắt buộc có `Module` và `sereServId > 0`, thiếu là Behavior trả `null`.
- Văn bản EMR lấy theo `TREATMENT_CODE` + `DOCUMENT_TYPE_ID = SERVICE_RESULT`, lọc theo `HIS_CODE` chứa `SERVICE_REQ_CODE:{...}` và `SER_SERV_ID:{...}` (hoặc `HIS_CODE` = `sereServ.ID`).
- Nhiều văn bản EMR → render từng trang thành ảnh (Aspose.Pdf, 300 DPI) rồi ghép bằng iTextSharp thành 1 PDF liên tục.
- Dịch vụ loại PT/TT → hiện nút in dạng dropdown (Phiếu PTTT / Phiếu kết quả).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SERE_SERV_4 | View | Dịch vụ đang xem (nguồn dữ liệu chính) |
| HIS_SERE_SERV_EXT | Table | Mô tả, kết luận, ghi chú, `JSON_FORM_ID` (link ảnh PACS) |
| HIS_SERVICE_REQ | Table | Y lệnh chứa dịch vụ, `EXECUTE_ROOM_ID` |
| HIS_SERE_SERV_TEMP | Table | Mẫu dịch vụ — `EMR_COLUMN_MAPPING`, `GEN_SIGNATURE_BY_KEY_CFG` |
| SAR_PRINT | Table | Cấu hình phiếu in / mã văn bản EMR |
| V_EMR_DOCUMENT, EMR_VERSION (EMR) | View/Table | Văn bản kết quả đã ký + phiên bản để tải PDF |
| EMR_SIGNER (EMR) | Table | `SIGN_IMAGE` theo `LOGINNAME` để chèn chữ ký |
| HIS_EXECUTE_ROOM, HIS_PATIENT | Table | Phòng thực hiện, thông tin bệnh nhân |

## 4. UI Layout

```
+-------------------------------------------------------------+
| xtraTab                                                     |
|  ├── xtraTabHis                                             |
|  │     ├── xtraTabPage_TabDocument : txtDescription (RichEdit)|
|  │     └── xtraTabPage_TabPdf      : pdfViewer1 (văn bản EMR)|
|  └── xtraTabPacs : txtUrl + webView1 (EO.WebBrowser)         |
+-------------------------------------------------------------+
| txtConclude / txtNote / lblStartTime / lblEndTime            |
| [In] [Ký số] [Mở web] [x] Tự động mở trình duyệt             |
+-------------------------------------------------------------+
```

- Header tab ẩn mặc định; chỉ hiện khi có link ảnh PACS.
- `chkAutoOpenWeb` lưu trạng thái qua `ControlStateWorker` (moduleLink của plugin).

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lấy dịch vụ | `HisRequestUriStore.HIS_SERE_SERV_GETVIEW_4` | MosConsumer |
| Lấy SS_EXT theo sere_serv | `api/HisSereServExt/Get` | MosConsumer |
| Lấy link xem kết quả (PACS Sancy) | `api/HisSereServExt/GetLinkResult` | MosConsumer |
| Văn bản kết quả EMR | `api/EmrDocument/GetView` | EmrConsumer |
| Phiên bản văn bản EMR | `api/EmrVersion/Get` | EmrConsumer |
| Tải file PDF đã ký | `Inventec.Fss.Client.FileDownload.GetFile(URL)` | FSS |

## 6. Dependencies

### Library / thư viện ngoài

| Thành phần | Mục đích |
|-----------|----------|
| HIS.Desktop.Plugins.Library.EmrGenerate | Sinh `InputADO` ký số EMR |
| Inventec.Common.SignLibrary (`SignLibraryGUIProcessor`) | Popup ký số / ký + in |
| EO.WebBrowser | Trình duyệt nhúng xem ảnh PACS |
| Aspose.Pdf + iTextSharp | Render trang PDF thành ảnh và ghép nhiều văn bản |
| MPS.ProcessorBase / MpsPrinter | In phiếu kết quả |

### Được mở từ (inter-plugin)

`ServiceExecute` (nút Tải ảnh — PACS Carestream, có truyền link), `ExecuteRoom`, `ServiceReqList`, `TreatmentHistory`, `BedRoomPartial`, `CoordinationServiceReqCLS`, `PayClinicalResult`, `ApprovalExamSpecialist`, `ApprovalExamAnesthesia`, `ApprovaleDebate`.

**Tham số Behavior nhận** (`ServiceReqResultViewBehavior.Run`):

| Kiểu | Vai trò |
|------|---------|
| `Inventec.Desktop.Common.Modules.Module` | BẮT BUỘC |
| `long` | BẮT BUỘC — `HIS_SERE_SERV.ID` |
| `string` | TÙY CHỌN — link xem ảnh PACS đã có sẵn |

## 7. Print

| Loại in | PrintTypeCode | Cách in |
|---------|--------------|---------|
| Phiếu kết quả CLS | Theo `SAR_PRINT.PRINT_TYPE_CODE` | RichEditorStore + template SAR |
| Phiếu phẫu thuật / thủ thuật | `PrintType.IN_PHIEU_PHAU_THUAT_THU_THUAT` | Menu dropdown khi dịch vụ là PT/TT |
| Ký số EMR | — | `SignLibraryGUIProcessor.ShowPopup` với PDF đã ghép |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 12/08/2026 | anhnh2@vietsens.vn | **Nhận link xem ảnh từ plugin cha + hiển thị được link do PACS đẩy sang.** (1) `ServiceReqResultViewBehavior` duyệt hết tham số trước khi tạo form (trước đây `break` ngay khi đủ Module + sereServId nên tham số đứng sau bị bỏ) và nhận thêm `string` = link xem ảnh. (2) Thêm constructor `frmServiceReqResultView(Module, long, string)`; constructor cũ giữ nguyên nên 10 nơi gọi hiện có không phải sửa. (3) Thêm `TryGetSavedPacsViewLink` — dùng `HIS_SERE_SERV_EXT.JSON_FORM_ID` khi phòng chưa khai `Api` hoặc không có trong `MOS.PACS.ADDRESS`, thay vì chỉ báo "Chưa cấu hình địa chỉ xem kết quả"; áp dụng cho cả `SereServClickRow` và `OpenWebFromConfig`. (4) Cờ `isGettingLinkFromPacs` — nhánh `CloudInfo` (PACS Sancy) vẫn lấy link mới, không dùng link cũ đã lưu. (5) Khi mở để xem ảnh (`isOpenForViewImage`): chọn sẵn tab PACS và **không** auto-print + Close theo config `PrintOption`. Các nhánh cũ (CloudInfo / Api) giữ nguyên hành vi. |
| — | IVT | Tạo plugin (chưa có tài liệu trước 12/08/2026) |

## 9. Test Cases

### Xem ảnh PACS Carestream (mở từ ServiceExecute)
- [ ] Dịch vụ đã có `JSON_FORM_ID` → bấm "Tải ảnh" ở ServiceExecute → form mở đúng tab PACS, ảnh hiển thị trong HIS
- [ ] Phòng thực hiện **không** khai `Api`/`CloudInfo` trong `MOS.PACS.ADDRESS` → vẫn hiển thị được ảnh, KHÔNG hiện "Chưa cấu hình địa chỉ xem kết quả"
- [ ] Viện bật `HIS.Desktop.Plugins.ServiceExecute.PrintOption = 1` → mở để xem ảnh **không** tự in và không tự đóng form
- [ ] Tài khoản chưa được cấp quyền module này → ServiceExecute mở link bằng trình duyệt mặc định (không popup lỗi plugin)
- [ ] `JSON_FORM_ID` rỗng → ServiceExecute báo "Chưa có hình ảnh từ hệ thống PACS cho dịch vụ này.", không mở form

### Không hồi quy các PACS hiện có
- [ ] PACS Sancy (`CloudInfo`) → vẫn gọi `api/HisSereServExt/GetLinkResult`, link mới ghi đè, không dùng link cũ
- [ ] PACS khai `Api` → vẫn ưu tiên `JSON_FORM_ID`, không có thì dựng URL theo `Api` như trước
- [ ] Mở form từ ServiceReqList / ExecuteRoom / TreatmentHistory (không truyền link) → hành vi như cũ, kể cả auto-print khi `PrintOption = 1`
- [ ] Checkbox "Tự động mở trình duyệt" giữ nguyên trạng thái giữa các phiên

### Nghiệp vụ chung
- [ ] Có văn bản EMR đã ký → hiện tab PDF, ghép đúng thứ tự nhiều văn bản
- [ ] Không có văn bản EMR → hiện mô tả/kết luận, ảnh chữ ký render đúng vị trí
- [ ] In phiếu kết quả / phiếu PTTT → preview đúng dữ liệu
- [ ] Ký số EMR → tạo văn bản EMR đúng `HIS_CODE`
