# Danh Sách Xuất (Thuốc/Vật Tư) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisExportMestMedicine |
| Loại | UC (UCHisExportMestMedicine) |
| Mục đích | Màn "Danh sách xuất" của kho: tra cứu phiếu xuất (mọi loại — bán, lĩnh, chuyển kho, đơn phòng khám...) và thao tác trên từng phiếu: xem chi tiết, sửa, duyệt/bỏ duyệt, thực xuất/hủy thực xuất, tạo bill, hủy bill, in, tạo phiếu nhập trả. |
| Người tạo | (kế thừa codebase) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
Lọc phiếu (kho, khoảng ngày, trạng thái, loại xuất, BN…) → grid `V_HIS_EXP_MEST_2` → các nút icon trên dòng: duyệt (`Approve`), bỏ duyệt (`Unapprove`), thực xuất (`Export`), hủy thực xuất (`Unexport`), hoàn thành, tạo bill (mở MedicineSaleBill), **hủy bill** (mở TransactionCancel), xóa phiếu…

### Hoàn kho phiếu xuất bán khi hủy bill (việc 3082)
Khi config `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport` = 1:
- Nút **Hủy bill** (`Btn_CancelBill_Enable`) mở plugin TransactionCancel; callback sau khi hủy thành công gọi `ExpMestRestoreStockWorker.RestoreAfterCancelInvoice(codes, roomId)` với **mã phiếu lấy từ dòng lưới TRƯỚC khi hủy** — không dùng BILL_ID vì BE set `BILL_ID = null` ngay trong luồng hủy giao dịch.
- Worker: lấy phiếu theo mã phiếu, chỉ phiếu **loại XUẤT BÁN**; **HOÀN THÀNH** → `api/HisExpMest/Unexport` (hoàn kho); **ĐÃ DUYỆT** → `api/HisExpMest/Unapprove` → phiếu về **YÊU CẦU (vàng)**. Tự động, không confirm; chỉ báo khi API fail. KHÔNG xóa phiếu — viện tự xóa.
- Config tắt → luồng hủy bill giữ nguyên 100%.

### Đơn đính kèm phiếu xuất bán (việc 57853 — 29/09/2026)
- Gate: `MOS.HAS_CONNECTION_EMR = 1` **và** `HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.IsEnable = 1`. Tắt → không tạo cột, lưới như cũ.
- Cột icon `ATTACH_PRESCRIPTION_DISPLAY` tạo lúc runtime (`InitAttachPrescriptionColumn`, partial `UCHisExportMestMedicine__AttachPrescription.cs`), ngay sau cột "Thanh toán", Fixed Left, rộng 20.
- **Chỉ phiếu bán** (`EXP_MEST_TYPE_ID = ID__BAN`): icon màu = đã có đơn đính kèm, icon xám = chưa có; phiếu loại khác để trống.
- Đánh dấu: mỗi lần nạp trang (`GridPaging`), `LoadAttachPrescriptionMarks` gọi **1 lần** EMR `GetView` cho toàn bộ mã phiếu bán trên trang → `HashSet<string>`; `CustomRowCellEdit` chỉ tra HashSet (không gọi API theo dòng).
- Bấm icon → Danh sách đơn đính kèm của phiếu (Library `ExpMestAttachFile`): Xem/In, Đính kèm mới, Xóa (không cho xóa khi phiếu Hoàn thành / có BILL_ID / có DEBT_ID). Có thay đổi → cập nhật lại icon của dòng đó.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_EXP_MEST_2 | View | Dòng grid phiếu xuất |
| V_HIS_EXP_MEST | View | Phiếu xuất bán gắn bill (3082) |
| HIS_EXP_MEST | Table | Kết quả các API duyệt/xuất/hoàn |
| V_EMR_DOCUMENT | View (EMR) | Đơn đính kèm phiếu bán (việc 57853) |

## 4. UI Layout

Bộ lọc bên trái + grid phiếu xuất với dải nút icon đầu dòng (xem, sửa, duyệt, bỏ duyệt, thực xuất, hủy thực xuất, tạo/hủy bill, in…) + panel thông tin chi tiết bên phải.

Việc 57853: cột icon 📎 "Đơn đính kèm" sau cột "Thanh toán" (chỉ khi bật config, chỉ phiếu bán).

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Duyệt phiếu | api/HisExpMest/Approve | MosConsumer |
| Bỏ duyệt | api/HisExpMest/Unapprove | MosConsumer |
| Thực xuất | api/HisExpMest/Export | MosConsumer |
| Hủy thực xuất/hoàn kho | api/HisExpMest/Unexport | MosConsumer |
| Phiếu theo bill (3082) | api/HisExpMest/GetView (BILL_ID) | MosConsumer |
| Đánh dấu phiếu bán có đơn đính kèm (57853) | api/EmrDocument/GetView (TREATMENT_CODEs = EXP_MEST_CODE trên trang, DOCUMENT_TYPE_ID = EXPSA, IS_DELETE=false) | EmrConsumer |

## 6. Dependencies

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.MedicineSaleBill | Nút tạo bill | expMestId + Module |
| HIS.Desktop.Plugins.TransactionCancel | Nút hủy bill | billId + row + DelegateSelectData |
| HIS.Desktop.Plugins.ExpMestViewDetail | Xem chi tiết phiếu | ExpMestViewDetailADO |

| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.ExpMestAttachFile (mới, 57853) | Đánh dấu + danh sách đơn đính kèm phiếu bán (xem/in, bổ sung, xóa theo trạng thái) |

## 7. Print

In phiếu xuất, hướng dẫn sử dụng thuốc (Mps000099…) qua MPS.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 02/10/2026 | nampp | **Việc 57853 (tên văn bản mặc định)** — Tên mặc định đổi thành **"Đơn thuốc ngoại viện của mã phiếu xuất {EXP_MEST_CODE}"** (key `frmExpMestAttachFile.DefaultDocumentName` có `{0}`). Phiếu đã lưu: điền sẵn theo mã phiếu. Phiếu chưa lưu: ô tên để trống + gợi ý (`...txtDocumentName.NullText.Pending`), lưu phiếu xong tên được dựng theo mã của TỪNG phiếu (`ExpMestAttachFileProcessor.GetDefaultDocumentName`); người dùng tự gõ tên thì giữ nguyên. |
| 02/10/2026 | nampp | **Việc 57853 (gắn đúng mã hồ sơ + mã xuất)** — Tài liệu EMR: `TreatmentCode` = `TDL_TREATMENT_CODE` (bán vãng lai không có hồ sơ → `EXP_MEST_CODE`), vẫn `IsOutsideTreatment = true`; `HIS_CODE` thêm khối chuẩn `|EXP_MEST_CODE:x|EXP_STOCK:y|REQ_DEPT:z` để EMR (`HisCodeStockParser`) ghi cột `EMR_DOCUMENT.EXP_MEST_CODE` / `EXP_MEDI_STOCK_CODE` / `REQ_DEPARTMENT_CODE` (trước đây không có `|` nên các cột này NULL). Danh sách đơn + đánh dấu lọc `TREATMENT_CODEs` = mã hồ sơ + mã phiếu rồi khớp theo `EMR_DOCUMENT.EXP_MEST_CODE` (fallback tách từ `HIS_CODE`) → tài liệu tạo trước đó vẫn nhận ra. |
| 29/09/2026 | khainq | **Việc 57853** — Cột icon "Đơn đính kèm" cho phiếu xuất bán (runtime, gated config `HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.IsEnable` + `MOS.HAS_CONNECTION_EMR`): đánh dấu cả trang bằng 1 API EMR, bấm icon mở danh sách đơn đính kèm. Partial mới `UCHisExportMestMedicine__AttachPrescription.cs`, móc ở `UCHisExportMestMedicine_Load` + `GridPaging`; ProjectReference `HIS.Desktop.Plugins.Library.ExpMestAttachFile`; reference `DevExpress.Images.v15.2`. |
| 07/08/2026 | nampp | Việc 3082: nút **Hủy bill** — sau khi TransactionCancel hủy hóa đơn thành công, tự động hoàn kho (Unexport) + hủy duyệt (Unapprove) đưa phiếu xuất bán về trạng thái Yêu cầu (chỉ khi config `SaveSignPrintAutoExport` = 1). Thêm `ExpMestRestoreStockWorker`. Fix build máy backup: đổi ProjectReference `Library.ElectronicBill` sang Reference resolve qua ReferencePath. |
| (trước 2026) | team | Tạo plugin danh sách xuất thuốc/vật tư. |

## 9. Test Cases

- [ ] Config tắt: hủy bill như cũ, phiếu không đổi trạng thái.
- [ ] Config bật, hủy bill của phiếu xuất bán ĐÃ THỰC XUẤT: tồn kho tăng lại; phiếu về **Yêu cầu**; phiếu vẫn còn trong danh sách.
- [ ] Phiếu không phải loại xuất bán: worker bỏ qua, không đụng trạng thái.
- [ ] Đóng màn Hủy giao dịch mà không hủy: không gọi Unexport/Unapprove.

### Việc 57853 — Đơn đính kèm
- [ ] Config tắt → không có cột 📎, lưới như cũ.
- [ ] Config bật → cột 📎 sau cột Thanh toán; phiếu bán có đơn → icon màu, chưa có → icon xám; phiếu không phải bán → trống.
- [ ] Lọc theo khoảng thời gian (kịch bản 3) → nhận biết đúng phiếu đã có đơn; bấm icon → danh sách đơn, Xem/In được.
- [ ] Bấm icon phiếu chưa có đơn → Đính kèm mới → đóng danh sách → icon dòng đó đổi sang màu; xóa hết đơn (phiếu chưa hoàn tất) → icon về xám.
- [ ] Phiếu Hoàn thành / đã thanh toán → không xóa được (kịch bản 4), vẫn bổ sung được.
- [ ] Chuyển trang → mỗi trang chỉ 1 lần gọi EMR GetView.
