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
Chỉ hiện khi config `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport` = 1 và nút Lưu ký đang hiện; trạng thái tick được nhớ giữa các phiên (ControlState). Tooltip: "In hóa đơn điện tử".

**Tick "In" + bấm Lưu ký**:
1. `SaveProcess(true)` — giữ nguyên luồng Lưu ký (tạo bill + phát hành HĐĐT). Phát hành fail (INVOICE_CODE rỗng) → dừng, không in (ghi log Warn).
2. `PrintInvoiceNow()`: lấy link HĐĐT (GET_INVOICE_LINK, retry ≤ 3 lần cách 1s thay Sleep(2000)) → `DocumentViewerManager.Print()` **in thẳng ra máy in**, không mở màn xem PDF (số liên theo `CONFIG_KEY__HIS_DESKTOP__ELECTRONIC_BILL__PRINT_NUM_COPY`).

- **KHÔNG tự duyệt/thực xuất phiếu tại màn này** (chốt 07/08/2026): phần mềm đã tự thực xuất phiếu ngay khi lưu ở màn Xuất bán (`IS_EXPORT_EQUAL_APPROVE` = 1 → phiếu về HOÀN THÀNH, kho đã trừ). Cũng bỏ luôn bước kiểm tra tồn trước khi lưu ký vì sẽ so nhầm với tồn đã trừ và chặn oan.
- KHÔNG tick → luồng Lưu ký cũ nguyên vẹn 100% (mở viewer).
- Thanh toán QR (pay form 8): mở module QR như cũ, không in tại bước này.

### Nút nào được làm việc với hóa đơn điện tử (chốt 08/08/2026)
| Nút | Phát hành HĐĐT | Tải/in HĐĐT |
|-----|----------------|-------------|
| Lưu (Ctrl S) | Không | Không |
| Lưu In (Ctrl I) | Không | **Không** khi config 3082 bật (chỉ in phiếu xuất bán) |
| Lưu ký (Ctrl A) | Có (`SaveProcess(isLuuKy: true)`) | Có — tick "In" thì in thẳng, không tick thì mở viewer |

Phát hành HĐĐT nằm trong `SaveProcess` và chỉ chạy khi `isLuuKy = true`, nên chỉ nút Lưu ký phát hành. Riêng nút Lưu In trước đây vẫn **tải** HĐĐT về xem khi config `HIS.Desktop.Plugins.MedicineSaleBill.PrintNow` = `Mps000339` — bill của nút này chưa có `INVOICE_CODE` nên nhà cung cấp trả lỗi "không tìm thấy hóa đơn tương ứng chuỗi đưa vào". Khi config 3082 bật thì bỏ nhánh đó, chỉ in phiếu xuất bán; config tắt giữ nguyên luồng cũ.

`onClickInHoaDonDienTu` cũng được sửa điều kiện chặn từ `&&` thành `||` (bug cũ): bill chưa phát hành HĐĐT thì thoát sớm + ghi log thay vì gọi API rồi hiện popup lỗi.

### Điều kiện nghiệp vụ
- Nút Lưu ký (và checkbox In) chỉ hiện khi `HIS.Desktop.ElectronicBill.Type` ∈ {1 (VNPT), 2 (HIS)}.
- Hoàn kho khi hủy hóa đơn đã tự thực xuất: xem docs HIS.Desktop.Plugins.ExpMestSaleTransactionList.
- Mẫu số/ký hiệu gửi nhà cung cấp lấy từ **sổ thu chi** (`HIS_ACCOUNT_BOOK.TEMPLATE_CODE` / `SYMBOL_CODE`) — xem mục 5.

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
| Lấy link HĐĐT để in (3082) | Library.ElectronicBill — GET_INVOICE_LINK | — |
| Chi tiết phiếu | api/HisExpMestMedicine/GetView, api/HisExpMestMaterial/GetView | MosConsumer |

### Mẫu số / ký hiệu gửi nhà cung cấp HĐĐT

`frmMedicineSaleBill` gán `dataInput.TemplateCode = accountBook.TEMPLATE_CODE` và `dataInput.SymbolCode = accountBook.SYMBOL_CODE` (sổ thu chi đang chọn). Với VNPT, `ElectronicBillProcessor.GetConfigVnpt` chèn 2 giá trị này vào vị trí 4 và 5 của chuỗi config, `VNPTBehavior` đọc ra thành `pattern` / `serial`. Sai lệch so với dải hóa đơn đã đăng ký bên nhà cung cấp → lỗi `ERR:20 (Pattern và serial không phù hợp…)`, phải sửa cấu hình sổ chứ không sửa code.

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
| 08/08/2026 | nampp | Việc 3082 — thu hẹp tiếp: **chỉ nút Lưu ký được làm việc với HĐĐT**. `btnSavePrint_Click` (Lưu In) không gọi `onClickInHoaDonDienTu` khi config 3082 bật (trước đó nhánh `Config.PrintNowMps == "Mps000339"` vẫn tải HĐĐT → lỗi ERR:6 vì bill chưa phát hành). Sửa điều kiện chặn trong `onClickInHoaDonDienTu` từ `&&` thành `||` (bug cũ, còn gây nguy cơ NullReferenceException khi `transactionBillResult == null`). Giữ (và bổ sung cho bản V1 đang thiếu) phần khóa checkbox "Xuất biên lai/hóa đơn" ở 2 màn Xuất bán: đây là điều kiện để form hóa đơn tìm thấy phiếu, không phải nhiệm vụ của tick "In". Gỡ entry `licenses.licx` rỗng trong csproj ExpMestSaleCreate (chặn lc.exe trên máy build). |
| 07/08/2026 | nampp | Việc 3082 — thu hẹp phạm vi màn thanh toán theo chốt với TUTM: checkbox "In" **chỉ còn nhiệm vụ in HĐĐT**. Bỏ `CheckStockBeforeExport()` và `AutoApproveExportExpMests()` (xóa hẳn 2 hàm) vì phần mềm đã tự thực xuất phiếu khi lưu ở màn Xuất bán; giữ lại `PrintInvoiceNow()`. Phần hủy hóa đơn (hoàn kho + về Yêu cầu) nằm ở các plugin danh sách, không đổi. |
| 05/08/2026 | nampp | Việc 3082 (thay thiết kế 53078 đã hủy/revert): thêm checkbox "In" (chkAutoExportPrint, có ControlState, tooltip "In hóa đơn điện tử") + config `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport` (mặc định tắt). Tick In + Lưu ký → check tồn theo lô (chặn trước khi lưu ký) → SaveProcess(true) → tự Approve (nếu chưa duyệt) + Export → in thẳng (DocumentViewerManager.Print, retry lấy link ≤3). Duyệt/thực xuất fail → dừng in + hiện lý do API. Không tick → luồng cũ 100%. Kèm theo: 2 màn Xuất bán (ExpMestSaleCreate/V2) disable checkbox "Xuất biên lai/hóa đơn" khi config bật để phiếu lưu không tạo bill trước. Gỡ entry licenses.licx trong csproj (chặn msbuild trên máy build). |
| (trước 2026) | team | Tạo plugin thanh toán hóa đơn nhà thuốc. |

## 9. Test Cases

- [ ] Config tắt: KHÔNG có checkbox "In"; Lưu ký như cũ.
- [ ] Config bật, KHÔNG tick: Lưu ký nguyên luồng cũ (mở viewer).
- [ ] Tick In: 1 bấm Lưu ký → bill + HĐĐT + **in thẳng ra máy in**, không mở viewer; phiếu giữ nguyên trạng thái (không đụng tới thực xuất).
- [ ] Tick In, phát hành HĐĐT fail: bill tạo, báo lỗi; KHÔNG in.
- [ ] Tick In, phát hành OK nhưng không lấy được link: báo lỗi + hướng dẫn in lại bằng nút In.
- [ ] Đóng mở lại form: checkbox nhớ trạng thái tick (ControlState).
- [ ] PRINT_NUM_COPY = 2: in đúng 2 liên.
- [ ] Tick In + thanh toán QR (pay form 8): mở module QR, không tự thực xuất/in.
- [ ] Config bật + `PrintNow` = `Mps000339`: bấm **Lưu In** → chỉ in phiếu xuất bán, KHÔNG hiện popup lỗi hóa đơn điện tử.
- [ ] Config bật: bấm **Lưu** (Ctrl S) → không đụng gì tới HĐĐT.
- [ ] Config tắt + `PrintNow` = `Mps000339`: nút Lưu In giữ nguyên luồng cũ (vẫn tải HĐĐT).
- [ ] Config bật, cả 2 màn Xuất bán (V1 + V2): checkbox "Xuất biên lai/hóa đơn" bị bỏ tick + khóa, có tooltip giải thích.
- [ ] Config tắt, cả 2 màn Xuất bán: checkbox "Xuất biên lai/hóa đơn" dùng bình thường như cũ.
