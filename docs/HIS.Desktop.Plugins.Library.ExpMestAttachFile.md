# Đính Kèm Đơn Thuốc Phiếu Xuất Bán (Library) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.Library.ExpMestAttachFile (Library — không đăng ký module ACS) |
| Loại | Library (2 Form popup kế thừa `FormBase`) |
| Mục đích | Đính kèm tệp/ảnh chụp đơn thuốc người bệnh mang từ ngoài vào phiếu xuất bán; lưu sang EMR (loại văn bản `EXPSA`), xem/in lại, bổ sung, xóa theo trạng thái phiếu. Dùng chung cho màn Xuất bán và Danh sách xuất. |
| Người tạo | khainq |
| Ngày tạo | 29/09/2026 |
| Việc | 57853 — TTMB - TK - Thêm chức năng đính kèm ảnh đơn thuốc ở màn xuất bán nhà thuốc và danh sách xuất |
| Trạng thái | Đang phát triển |

Tham khảo: chức năng "Đính kèm file" của `HIS.Desktop.Plugins.HisImportMestMedicine` (việc 42244). Tách thành Library thay vì copy form lần 3 (EmrDocument → ImpMest → ExpMest), không cần khai báo module ACS mới.

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
```
Màn Xuất bán — phiếu CHƯA lưu
  [Đính kèm đơn] → ChooseFiles(pending) → frmExpMestAttachFile (chế độ tạm)
      Chọn tệp / Chụp ảnh (HIS.Desktop.Plugins.Camera) → kiểm tra định dạng + dung lượng → xem trước, xoay, xóa từng tệp
      [Đồng ý] → trả PendingAttachADO (tệp giữ trong RAM)
  Lưu phiếu OK → AttachPendingFiles(phiếu vừa lưu, pending) → gộp 1 PDF → EmrDocument/CreateByTdo cho từng phiếu

Phiếu ĐÃ có (màn Xuất bán chế độ sửa / icon trên Danh sách xuất)
  ShowAttachList(phiếu) → frmExpMestAttachList
      Xem / In (DownloadFile → SignLibraryGUIProcessor.ShowPopup)
      Đính kèm mới → frmExpMestAttachFile (chế độ lưu thẳng EMR)
      Xóa (xóa mềm EMR) — chỉ khi phiếu chưa hoàn tất
```

### Liên kết tài liệu ↔ phiếu
| Trường DocumentTDO | Giá trị | Lý do |
|---|---|---|
| TreatmentCode | `TDL_TREATMENT_CODE` (mã hồ sơ); bán vãng lai không có hồ sơ → `EXP_MEST_CODE` | Đúng mã hồ sơ của phiếu; backend bắt buộc không rỗng |
| HisCode | `{MaSite} EXP_MEST_CODE:{mã} SERVICE_REQ_CODE:{mã đơn}` + `|EXP_MEST_CODE:{mã}|EXP_STOCK:{mã kho}|REQ_DEPT:{mã khoa}` | Khối sau dấu `|` được EMR (`HisCodeStockParser`) tách vào cột **`EMR_DOCUMENT.EXP_MEST_CODE`**, `EXP_MEDI_STOCK_CODE`, `REQ_DEPARTMENT_CODE` — cùng định dạng `MPS AbstractProcessor.BuildEmrStockData` |
| DocumentTypeId | ID của `EXPSA` | Lọc chọn lọc, tránh quét toàn EMR_DOCUMENT |
| IsOutsideTreatment | true | Không vào bộ bệnh án; backend không kiểm tra khóa/lưu trữ hồ sơ → đính kèm được cả khi hồ sơ đã khóa |
| (Truy vấn) | `TREATMENT_CODEs` = mã hồ sơ + mã phiếu, rồi khớp theo `EMR_DOCUMENT.EXP_MEST_CODE` (fallback tách `EXP_MEST_CODE:` từ `HIS_CODE`) | Bộ lọc EMR không có `EXP_MEST_CODEs`; 1 hồ sơ nhiều phiếu không lẫn đơn của nhau; tài liệu tạo trước 02/10/2026 vẫn nhận ra |
| FileType | PDF | Mọi tệp của 1 lần đính kèm gộp thành 1 PDF (ảnh: 1 trang A4, tự xoay ngang theo ảnh; PDF: giữ nguyên trang) |

### Điều kiện nghiệp vụ
- Bật khi `MOS.HAS_CONNECTION_EMR = 1` **và** `HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.IsEnable = 1`.
- Định dạng: jpg, jpeg, png, bmp, gif, pdf. Dung lượng tối đa/tệp: `HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.MaxFileSizeMB` (mặc định 5). Kiểm tra ngay khi chọn tệp (trước khi đọc vào RAM); ảnh chụp đo theo JPEG.
- **Không cho xóa** khi phiếu `EXP_MEST_STT_ID = ID__DONE` hoặc có `BILL_ID` hoặc có `DEBT_ID` (`ExpMestAttachFileProcessor.IsAllowDelete`). Phiếu đã chốt kỳ kho luôn Hoàn thành → cùng điều kiện. `HIS_EXP_MEST` không có cột `MEDI_STOCK_PERIOD_ID`.
- Người/thời điểm đính kèm/xóa: `CREATOR/CREATE_TIME/MODIFIER/MODIFY_TIME` của EMR_DOCUMENT + `LogAction` (`AttachPrescription`, `DeletePrescriptionAttach`).
- ⚠ Chặn xóa **chỉ ở frontend** — API `EmrDocument/Delete` không biết trạng thái phiếu, vẫn xóa được qua màn "Danh sách văn bản" EMR.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_EXP_MEST / V_HIS_EXP_MEST / V_HIS_EXP_MEST_2 | Table/View | Nguồn dựng `ExpMestAttachInfoADO` (ID, EXP_MEST_CODE, TDL_SERVICE_REQ_CODE, EXP_MEST_STT_ID, BILL_ID, DEBT_ID) |
| V_EMR_DOCUMENT | View (EMR) | Tài liệu đã đính kèm |
| EMR_DOCUMENT_TYPE | Table (EMR) | Loại văn bản `EXPSA` (cache ID khi tìm thấy) |

### ADO public
| Class | Mô tả |
|---|---|
| `ExpMestAttachInfoADO` | Thông tin tối thiểu của phiếu (ctor từ HIS_EXP_MEST / V_HIS_EXP_MEST / V_HIS_EXP_MEST_2) |
| `PendingAttachADO` | Tệp chọn khi phiếu chưa lưu: `DocumentName`, `Files`, `Count` |
| `AttachFileADO` | 1 tệp: `FileName`, `PdfPath` (PDF) hoặc `Image` (ảnh), `FileSize`, `IsCapture` |

## 4. UI Layout

### frmExpMestAttachFile
```
+------------------------------------------------------------------+
| Tên văn bản: [Đơn thuốc ngoại viện                             ] |
| [Chọn tệp] [Chụp ảnh] [↶ Xoay trái] [Xoay phải ↷]                |
| STT | 🗑 | Tên tệp      |  Xem trước (ảnh: PictureEdit Zoom /     |
| 1   | 🗑 | Ảnh chụp 1   |             PDF: PdfViewer)             |
| Định dạng cho phép ... tối đa 5 MB/tệp   [Đồng ý/Lưu (Ctrl S)][Đóng] |
+------------------------------------------------------------------+
```
### frmExpMestAttachList
```
+------------------------------------------------------------------+
| (Maroon) Phiếu đã hoàn thành/đã thanh toán: chỉ xem và bổ sung... |
| STT | 👁 | 🗑 | Tên văn bản | Thời gian đính kèm | Người đính kèm | Thời gian sửa | Người sửa |
| n tài liệu            [Đính kèm mới (Ctrl N)] [Làm mới (F5)] [Đóng] |
+------------------------------------------------------------------+
```
Phím tắt: Ctrl S (lưu/đồng ý), Ctrl N (đính kèm mới), F5 (làm mới), Esc (đóng).

## 5. API Endpoints

| Action | URI | Consumer | Filter / Body |
|--------|-----|----------|---------------|
| Loại văn bản EXPSA | api/EmrDocumentType/Get | EmrConsumer | EmrDocumentTypeFilter (DOCUMENT_TYPE_CODE__EXACT, IS_ACTIVE) |
| Danh sách / đánh dấu | api/EmrDocument/GetView | EmrConsumer | EmrDocumentViewFilter (TREATMENT_CODEs = mã hồ sơ + mã phiếu, DOCUMENT_TYPE_ID, IS_ACTIVE, IS_DELETE=false) → khớp `EXP_MEST_CODE` phía client |
| Tạo tài liệu | EMR.URI.EmrDocument.CREATE_BY_TDO | EmrConsumer | DocumentTDO (base64 PDF) |
| Tải nội dung | api/EmrDocument/DownloadFile | EmrConsumer | EmrDocumentDownloadFileSDO (ID, IsMerge) |
| Xóa mềm | EMR.URI.EmrDocument.DELETE | EmrConsumer | documentId |

## 6. Dependencies

### Public API — `ExpMestAttachFileProcessor`
| Method | Mô tả |
|---|---|
| `IsEnable()` | Config bật + có kết nối EMR |
| `IsAllowDelete(ExpMestAttachInfoADO)` | Quy tắc xóa theo trạng thái phiếu |
| `ChooseFiles(PendingAttachADO)` | Form chọn tệp chế độ tạm; null = hủy |
| `AttachPendingFiles(List<ExpMestAttachInfoADO>, PendingAttachADO)` | Gộp PDF + tạo tài liệu cho từng phiếu; cảnh báo phiếu lỗi |
| `ShowAttachList(ExpMestAttachInfoADO, roomId, Action)` | Danh sách đơn của phiếu; Action gọi khi có thay đổi |
| `GetExpMestCodesHasAttach(List<ExpMestAttachInfoADO>)` | HashSet mã phiếu đã có đơn (1 API) — cần EXP_MEST_CODE + TDL_TREATMENT_CODE |
| `ConfirmDiscard`, `Release`, `ShowMultiPatientWarning` | Hỗ trợ màn Xuất bán |
| `GetButtonCaption/GetButtonToolTip/GetGridColumnCaption/GetGridToolTip` | Chuỗi đa ngôn ngữ cho màn gọi |

### Library / Inter-Plugin
| Thành phần | Mục đích |
|---|---|
| HIS.Desktop.Plugins.Library.EmrGenerate | `GenerateInputADO` cho viewer |
| Inventec.Common.SignLibrary | `SignLibraryGUIProcessor.ShowPopup` xem/phóng to/in |
| itextsharp 5.5.3 | Gộp ảnh/PDF |
| HIS.Desktop.Plugins.Camera | Chụp ảnh — args `DelegateSelectData` (mỗi ảnh 1 lần callback) |

### Được dùng bởi
| Plugin | Vị trí |
|---|---|
| HIS.Desktop.Plugins.ExpMestSaleCreate | `UCExpMestSaleCreate___AttachPrescription.cs` |
| HIS.Desktop.Plugins.HisExportMestMedicine | `UCHisExportMestMedicine__AttachPrescription.cs` |

### Deploy
- DLL: `x64\ReferencedAssemblies\HIS.Desktop.Plugins.Library.ExpMestAttachFile.dll` + satellite `x64\vi\`, `x64\en\`.
- Có `Resources\Lang.resx` + `Message.Lang.resx` **neutral** (tiếng Việt) nhúng DLL chính → deploy thiếu satellite vẫn có chữ.
- Script DB: `docs/SQL_57853_ExpMestSaleAttachPrescription.sql` (EMR_DOCUMENT_TYPE `EXPSA` + 2 HIS_CONFIG).

## 7. Print

Không có mẫu MPS. In đơn đính kèm qua viewer `SignLibraryGUIProcessor.ShowPopup` (nút Xem/In).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 02/10/2026 | nampp | **Việc 57853 (gắn đúng mã hồ sơ + mã xuất)** — Tài liệu EMR: `TreatmentCode` = `TDL_TREATMENT_CODE` (bán vãng lai không có hồ sơ → `EXP_MEST_CODE`), vẫn `IsOutsideTreatment = true`; `HIS_CODE` thêm khối chuẩn `|EXP_MEST_CODE:x|EXP_STOCK:y|REQ_DEPT:z` để EMR (`HisCodeStockParser`) ghi cột `EMR_DOCUMENT.EXP_MEST_CODE` / `EXP_MEDI_STOCK_CODE` / `REQ_DEPARTMENT_CODE` (trước đây không có `|` nên các cột này NULL). Danh sách đơn + đánh dấu lọc `TREATMENT_CODEs` = mã hồ sơ + mã phiếu rồi khớp theo `EMR_DOCUMENT.EXP_MEST_CODE` (fallback tách từ `HIS_CODE`) → tài liệu tạo trước đó vẫn nhận ra. |
| 29/09/2026 | khainq | Việc 57853 — Tạo thư viện: form đính kèm (chế độ tạm/lưu thẳng, chọn tệp, chụp ảnh, xoay, kiểm tra định dạng/dung lượng), form danh sách đơn (xem/in, bổ sung, xóa theo trạng thái), worker EMR, gộp PDF, audit LogAction. |

## 9. Test Cases

### Chọn tệp / chụp ảnh
- [ ] Chọn nhiều tệp cùng lúc (jpg + pdf) → hiện đủ trong lưới, bấm từng dòng xem trước đúng (ảnh/PDF).
- [ ] Tệp > MaxFileSizeMB → cảnh báo "vượt quá dung lượng cho phép", không thêm; tệp .docx → cảnh báo sai định dạng.
- [ ] Config MaxFileSizeMB rỗng / chữ / ≤ 0 → dùng 5 MB.
- [ ] Chụp ảnh nhiều lần → "Ảnh chụp 1.jpg", "Ảnh chụp 2.jpg"…; xoay trái/phải → ảnh lưu đúng chiều trong PDF.
- [ ] Không có quyền module Camera → thông báo, không lỗi.
- [ ] Chế độ tạm: Đóng (không Đồng ý) → danh sách tệp giữ như trước khi mở.

### Lưu / danh sách
- [ ] Lưu thẳng không có tệp → "Vui lòng chọn tệp hoặc chụp ảnh...".
- [ ] Chưa có loại văn bản EXPSA trong EMR → thông báo chưa khai báo loại văn bản, không lưu.
- [ ] Lưu → 1 dòng mới trong danh sách, Người đính kèm = tài khoản đăng nhập, thời gian đúng.
- [ ] Xem → viewer toàn màn hình, phóng to + in được; tệp tạm trong `temp\` bị xóa sau khi đóng.
- [ ] Phiếu chưa hoàn tất → Xóa: xác nhận → xóa mềm → dòng biến mất; LogAction ghi `DeletePrescriptionAttach`.
- [ ] Phiếu Hoàn thành / BILL_ID / DEBT_ID → nhãn cảnh báo Maroon, nút Xóa xám + thông báo không được xóa.
- [ ] Ngôn ngữ EN → caption/thông báo tiếng Anh.
