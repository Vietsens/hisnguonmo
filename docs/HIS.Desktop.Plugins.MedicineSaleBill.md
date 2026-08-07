# Thanh Toán / Hóa Đơn Nhà Thuốc — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.MedicineSaleBill |
| Loại | Form (frmMedicineSaleBill) |
| Mục đích | Form thanh toán/hóa đơn cho phiếu xuất bán kho nhà thuốc: tạo bill (CreateBillWithBillGood), phát hành hóa đơn điện tử (VNPT qua Library.ElectronicBill), in hóa đơn. Mở từ màn Xuất bán (ExpMestSaleCreate/V2 — nút "Xuất hóa đơn (F10)"). |
| Người tạo | (kế thừa codebase) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
Chọn phiếu xuất bán → nhập thông tin người mua/hình thức thanh toán/sổ → Lưu (Ctrl S) / Lưu In (Ctrl I) / Lưu ký (Ctrl A). Lưu ký = tạo bill + phát hành HĐĐT rồi mở màn XEM PDF hóa đơn.

### Checkbox "In" (việc 3082)
Chỉ hiện khi config `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport` = 1 và nút Lưu ký đang hiện; trạng thái tick được nhớ giữa các phiên (ControlState). **Tick "In" + bấm Lưu ký** → luồng tuần tự, fail bước nào dừng bước đó:
1. `CheckStockBeforeExport()`: so số lượng xuất theo LÔ (V_HIS_EXP_MEST_MEDICINE/MATERIAL nhóm theo MEDICINE_ID/MATERIAL_ID) với tồn `V_HIS_MEDICINE/V_HIS_MATERIAL.AMOUNT` — thiếu → báo danh sách mặt hàng thiếu, DỪNG NGAY (không lưu ký, không phát hành — kịch bản 2 tài liệu 3082).
2. `SaveProcess(true)` — giữ nguyên luồng Lưu ký. Phát hành fail (INVOICE_CODE rỗng) → dừng, không thực xuất, không in.
3. `AutoApproveExportExpMests()`: từng phiếu chọn — chưa duyệt (DRAFT/REQUEST) thì `api/HisExpMest/Approve` trước, rồi `api/HisExpMest/Export` (IsFinish=true); bỏ qua phiếu đã DONE. Fail → popup lý do từ `param.Messages` + `param.BugCodes` + hướng dẫn "vào màn Thực xuất thuốc xử lý thủ công rồi in lại bằng nút In"; KHÔNG in.
4. `PrintInvoiceNow()`: lấy link HĐĐT (GET_INVOICE_LINK, retry ≤ 3 lần cách 1s thay Sleep(2000)) → `DocumentViewerManager.Print()` in thẳng (số liên theo `CONFIG_KEY__HIS_DESKTOP__ELECTRONIC_BILL__PRINT_NUM_COPY`).
- KHÔNG tick → luồng Lưu ký cũ nguyên vẹn 100% (mở viewer).
- Thanh toán QR (pay form 8): mở module QR như cũ, không tự thực xuất/in.

### Điều kiện nghiệp vụ
- Nút Lưu ký (và checkbox In) chỉ hiện khi `HIS.Desktop.ElectronicBill.Type` ∈ {1 (VNPT), 2 (HIS)}.
- Hoàn kho khi hủy hóa đơn đã tự thực xuất: xem docs HIS.Desktop.Plugins.ExpMestSaleTransactionList.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_EXP_MEST | View | Phiếu xuất bán (trạng thái để quyết định Approve/Export) |
| V_HIS_EXP_MEST_MEDICINE / V_HIS_EXP_MEST_MATERIAL | View | Chi tiết thuốc/vật tư theo lô |
| V_HIS_MEDICINE / V_HIS_MATERIAL | View | Tồn kho theo lô (check tồn trước thực xuất) |
| V_HIS_TRANSACTION | View | Bill kết quả |
| V_HIS_TREATMENT_FEE | View | Thông tin điều trị/viện phí cho HĐĐT |
| V_HIS_ACCOUNT_BOOK, V_HIS_CASHIER_ROOM, HIS_PAY_FORM | View/Table | Sổ, phòng thu ngân, hình thức thanh toán |

## 4. UI Layout

Hàng nút đáy form: Ngoài giờ | Không hiển thị HĐ ĐT | **In (chkAutoExportPrint — ẩn khi config tắt, nhớ trạng thái)** | QR | Lưu ký (Ctrl A) | Lưu (Ctrl S) | Lưu in (Ctrl I) | In (Ctrl P) | Mới (Ctrl N).

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Tạo bill | api/HisTransaction/CreateBillWithBillGood | MosConsumer |
| Cập nhật info HĐĐT | api/HisTransaction/UpdateInvoiceInfo | MosConsumer |
| Duyệt phiếu (3082) | api/HisExpMest/Approve (HisExpMestApproveSDO) | MosConsumer |
| Thực xuất phiếu (3082) | HisRequestUriStore.HIS_EXP_MEST_EXPORT = api/HisExpMest/Export | MosConsumer |
| Tồn kho theo lô (3082) | api/HisMedicine/GetView, api/HisMaterial/GetView | MosConsumer |
| Chi tiết phiếu | api/HisExpMestMedicine/GetView, api/HisExpMestMaterial/GetView | MosConsumer |

## 6. Dependencies

| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.ElectronicBill | Phát hành/lấy link HĐĐT (CREATE_INVOICE, GET_INVOICE_LINK) |
| Inventec.Common.DocumentViewer | Run (mở viewer) / Print (in thẳng — 3082) |
| HIS.Desktop.Library.CacheClient.ControlStateWorker | Nhớ trạng thái checkbox In (3082) |
| HIS.Desktop.Plugins.CreateTransReqQR | Thanh toán QR |

## 7. Print

In HĐĐT: link PDF từ nhà cung cấp → DocumentViewerManager (viewer khi Lưu ký thường; Print trực tiếp khi tick "In").

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 05/08/2026 | nampp | Việc 3082 (thay thiết kế 53078 đã hủy/revert): thêm checkbox "In" (chkAutoExportPrint, có ControlState, tooltip "In hóa đơn điện tử") + config `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport` (mặc định tắt). Tick In + Lưu ký → check tồn theo lô (chặn trước khi lưu ký) → SaveProcess(true) → tự Approve (nếu chưa duyệt) + Export → in thẳng (DocumentViewerManager.Print, retry lấy link ≤3). Duyệt/thực xuất fail → dừng in + hiện lý do API. Không tick → luồng cũ 100%. Kèm theo: 2 màn Xuất bán (ExpMestSaleCreate/V2) disable checkbox "Xuất biên lai/hóa đơn" khi config bật để phiếu lưu không tạo bill trước. Gỡ entry licenses.licx trong csproj (chặn msbuild trên máy build). |
| (trước 2026) | team | Tạo plugin thanh toán hóa đơn nhà thuốc. |

## 9. Test Cases

- [ ] Config tắt: KHÔNG có checkbox "In"; Lưu ký như cũ.
- [ ] Config bật, KHÔNG tick: Lưu ký nguyên luồng cũ (mở viewer, không tự thực xuất).
- [ ] Tick In, đủ tồn: 1 bấm Lưu ký → bill + HĐĐT + phiếu tự duyệt/THỰC XUẤT (tồn giảm đúng) + in thẳng.
- [ ] Tick In, thiếu tồn theo lô: chặn NGAY, báo danh sách thiếu; không tạo bill, kho không đổi.
- [ ] Tick In, phát hành HĐĐT fail: bill tạo, báo lỗi; KHÔNG thực xuất, KHÔNG in.
- [ ] Duyệt/thực xuất fail sau phát hành: KHÔNG in; popup lý do + hướng dẫn thủ công; HĐĐT giữ nguyên.
- [ ] Đóng mở lại form: checkbox nhớ trạng thái tick (ControlState).
- [ ] PRINT_NUM_COPY = 2: in đúng 2 liên.
- [ ] Tick In + thanh toán QR (pay form 8): mở module QR, không tự thực xuất/in.
