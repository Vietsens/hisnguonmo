# Xuất Bán (Kho Nhà Thuốc) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExpMestSaleCreate |
| Loại | UC (UCExpMestSaleCreate — UserControlBase), 8 partial (`___Proccess`, `__Load`, `__InitPrint`, `__Plus_Search`, `__Shortcut`, `__MenuMouseRight`, `__Validate`, `___SaveSignPrintInvoice`) |
| Mục đích | Bán thuốc/vật tư tại kho nhà thuốc: tạo/sửa phiếu xuất bán (theo đơn hoặc vãng lai), tạo bill khi tick "Xuất biên lai/hóa đơn", in phiếu xuất bán, mở form Xuất hóa đơn (MedicineSaleBill), hủy xuất, xác nhận nợ. Menu "Nhập xuất tồn > Xuất bán". |
| Người tạo | team (việc 36371) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
Chọn kho xuất → nhập BN (mã điều trị/mã đơn/vãng lai) → kê thuốc/vật tư (kiểm tra tồn khả dụng khi thêm dòng) → **Lưu (F5)** hoặc **Lưu in (F9)** → `ProcessSave`: POST `api/HisExpMest/SaleCreateListSdo` (1 BN) / `SaleCreateBillList` (nhiều BN) / `SaleUpdateListSdo` (sửa); tick **"Xuất biên lai/hóa đơn"** (`chkCreateBill`) → `CreateBill = true` → BE tạo luôn bill (`resultSDO.Transaction`) theo sổ/điểm thu/hình thức chọn ở panel phải → Lưu in in phiếu xuất bán (Mps) hoặc hóa đơn/biên lai giấy (Mps000339) → **Xuất hóa đơn (F10)** mở form `HIS.Desktop.Plugins.MedicineSaleBill` (chỉ với phiếu chưa có bill).

### Trạng thái phiếu sau lưu (phụ thuộc BE)
Kho `HIS_MEDI_STOCK_EXTY.IS_AUTO_APPROVE/IS_AUTO_EXECUTE` + `MOS.EXP_MEST.EXPORT_SALE.MUST_BILL` + `MOS.TRANSACTION.EXP_MEST_SALE.IS_AUTO_EXPORT` quyết định phiếu về Yêu cầu / Đã duyệt / Hoàn thành ngay khi lưu hoặc khi tạo bill; `btnCancelExport` (Hủy xuất) chỉ enable khi có phiếu HOÀN THÀNH.

### Nút "Lưu ký in" (việc 3082 — v3.2 29/08/2026)
- Nút mới `btnSaveSignPrint` **"Lưu ký in (Ctrl E / F11)"** ngay trước "Lưu in"; **luôn hiện, không key config**.
- **Enable/disable theo checkbox "Xuất biên lai/hóa đơn"**: `btnSaveSignPrint.Enabled = chkCreateBill.Checked && btnSavePrint.Enabled` (`RefreshSaveSignPrintButton`, đồng bộ qua `chkCreateBill.CheckedChanged`, `btnSavePrint.EnabledChanged`, `EnabledChanged/VisibleChanged` của UC + `BeginInvoke` refresh sau Load — vì `Control.Enabled` là trạng thái hiệu dụng, = false khi WaitingManager khóa form cha lúc load). Checkbox "Xuất biên lai/hóa đơn" giữ nguyên code gốc.
- Bấm: `savePrintInvoice = true`, `savePrint = false` → `btnSave_Click` → `ProcessSave` lưu phiếu **+ BE tạo bill** (vì đang tick), **không in phiếu xuất bán** → sau `MessageManager.Show` gọi `OpenMedicineSaleBillAutoSignPrint()`: mỗi kết quả lưu có `Transaction` → mở form Xuất hóa đơn với args `(Module, List<long> expMestIds, DelegateSelectData EnableControlAfterSaveSaleBill, List<string> { "AUTO_ISSUE_EXISTING_BILL", "TRANSACTION_ID=<id>" })` → form phát hành HĐĐT cho bill đó, tự duyệt/thực xuất, in thẳng, tự đóng (xem docs MedicineSaleBill). Bill hình thức QR → bỏ qua (ProcessSave đã mở module QR). Không có Transaction → `{ "AUTO_SAVE_SIGN_PRINT" }` (form tự tạo bill, chỉ phiếu chưa có bill). Nhánh key `ExpMestSaleCreate__Show_MedicineSaleBill` bị bỏ qua trong lượt này.
- Flag `savePrintInvoice` reset trong `finally` của `btnSaveSignPrint_Click` và catch của `ProcessSave`.
- Nút "Lưu in" giữ nguyên luồng cũ.

### Điều kiện nghiệp vụ
- Không kê vượt tồn khả dụng (validation khi thêm dòng).
- Hình thức Tiền mặt/CK, Tiền mặt/QT: số tiền CK/QT không vượt tổng phải thanh toán.
- POS (`ChkKetNoiPOS` + `chkCreateBill`): gọi máy POS trước khi lưu.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_MEDI_STOCK | View | Kho xuất |
| HisMedicineTypeInStockSDO / HisMaterialTypeInStockSDO | SDO | Tồn khả dụng theo loại thuốc/vật tư |
| HisExpMestSaleListSDO / HisExpMestSaleListResultSDO | SDO | Dữ liệu lưu phiếu bán (CreateBill) + kết quả (ExpMestSdos, Transaction) |
| HIS_EXP_MEST | Table | Phiếu xuất bán (EXP_MEST_STT_ID, BILL_ID) |
| HIS_TRANSACTION | Table | Bill tạo khi tick "Xuất biên lai/hóa đơn" (ID truyền sang form hóa đơn) |
| V_HIS_SERVICE_REQ_11 | View | Đơn thuốc nguồn |
| HIS_PATIENT | Table | Bệnh nhân |
| HIS_PAY_FORM, V_HIS_ACCOUNT_BOOK, V_HIS_CASHIER_ROOM | Table/View | Hình thức thanh toán, sổ, phòng thu |

## 4. UI Layout

```
+------------------------------------------------------------------------------+
| Kho xuất | Đối tượng | Vãng lai | Đơn cũ            Xuất biên lai/hóa đơn ☐  |
| Mã | Mã đơn thuốc | Mã điều trị | DS (F3) | Bệnh nhân | Giới tính | Ngày sinh |
| ... thông tin BN / phiếu / chẩn đoán ...                Tổng tiền / Phải TT   |
| Chọn thuốc/vật tư (Ctrl F) | Số lượng | Số ngày | Giá | VAT | Chiết khấu ...  |
| Lưới thuốc/vật tư đã kê                         | Lưới tồn kho (bên phải)    |
+------------------------------------------------------------------------------+
| Tự động hiển thị tồn ☐ | Ký đơn nhà thuốc ☐ | Xem trước khi in ☑              |
| Hủy xuất (Ctrl H) | [Lưu ký in (Ctrl E)] | Lưu in (F9) | Lưu (F5) | Mới | Đơn mới |
| In ▾ | QR | Xuất hóa đơn (F10) | Xác nhận nợ                                    |
+------------------------------------------------------------------------------+
```
`[Lưu ký in]` = `btnSaveSignPrint` (việc 3082) — chỉ enable khi tick "Xuất biên lai/hóa đơn". Phím tắt F5/F7/F8/F9/F10/F11 khi key `ExpMestSaleCreate.IsUsingFunctionKeyInsteadOfCtrlKey` = 1, ngược lại Ctrl S/D/N/I/T/E.

### UC sử dụng
| UC | Mục đích |
|----|----------|
| HIS.UC.Icd + HIS.UC.SecondaryIcd | Chẩn đoán chính/phụ |
| HIS.UC.MedicineTypeInStock / MaterialTypeInStock | Chọn thuốc/vật tư còn tồn |
| HIS.UC.ExpMestMedicineGrid / ExpMestMaterialGrid | Lưới chi tiết phiếu |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lưu phiếu bán 1 BN (+ bill nếu CreateBill) | api/HisExpMest/SaleCreateListSdo | MosConsumer |
| Lưu phiếu bán nhiều BN | api/HisExpMest/SaleCreateBillList | MosConsumer |
| Sửa phiếu bán | api/HisExpMest/SaleUpdateListSdo | MosConsumer |
| Đơn thuốc nguồn | api/HisServiceReq/Get | MosConsumer |
| Bệnh nhân | api/HisPatient/Get | MosConsumer |

## 6. Dependencies

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.MedicineSaleBill | Nút Xuất hóa đơn (F10); key `Show_MedicineSaleBill` = 1 sau lưu | Module, List<long> expMestIds, DelegateSelectData |
| HIS.Desktop.Plugins.MedicineSaleBill (chế độ tự động, 3082 v3.2) | Nút "Lưu ký in" (tick "Xuất biên lai/hóa đơn"), sau lưu thành công | Module, List<long> expMestIds, DelegateSelectData, **List<string> { "AUTO_ISSUE_EXISTING_BILL", "TRANSACTION_ID=<id>" }** (bill đã tạo) hoặc **{ "AUTO_SAVE_SIGN_PRINT" }** (không có bill) |
| HIS.Desktop.Plugins.CreateTransReqQR | Lưu có tạo bill với hình thức QR | TransReqQRADO |

### Library
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Library.CacheClient.ControlStateWorker | Nhớ checkbox (Xem trước khi in, Xuất biên lai/hóa đơn, POS, Ký đơn) |
| Inventec.Common.RichEditor.RichEditorStore | In phiếu xuất bán / hóa đơn biên lai / HDSD |

## 7. Print

| Loại in | PrintTypeCode | Ghi chú |
|---------|--------------|---------|
| Phiếu xuất bán | theo `InitMenuPrint` (menu In) | `onClickInPhieuXuatBan` — Lưu in không tick "Xuất biên lai/hóa đơn" |
| Hóa đơn/biên lai xuất bán | Mps000339 | `onClickInHoaDonBienLaiXuatBan` — Lưu in khi tick "Xuất biên lai/hóa đơn" |
| Hướng dẫn sử dụng thuốc | Mps000099 | menu In |
| Hóa đơn điện tử (3082) | — | In thẳng tại form MedicineSaleBill (DocumentViewerManager.Print) sau "Lưu ký in" |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 29/08/2026 | nampp | Việc 3082 **v3.2** (chốt DANGTH): **bỏ key** `SaveSignPrintAutoExport` và **bỏ checkbox "Xuất HĐĐT"** riêng (Designer/ControlState); nút `btnSaveSignPrint` luôn hiện, **enable theo `chkCreateBill` ("Xuất biên lai/hóa đơn") && `btnSavePrint.Enabled`**; gỡ khóa `chkCreateBill` (08/08) → code gốc; `OpenMedicineSaleBillAutoSignPrint` truyền marker `AUTO_ISSUE_EXISTING_BILL` + `TRANSACTION_ID=` (bill đã tạo lúc lưu), QR bỏ qua, không bill → `AUTO_SAVE_SIGN_PRINT`. Hàng đáy: emptySpaceItem1 47, Ký đơn 185, Xem trước 130. V2: gỡ khóa `chkExp` → không còn thay đổi. |
| 26/08/2026 | nampp | Việc 3082 v3.1: nút mới "Lưu ký in (Ctrl E / F11)" enable theo checkbox "Xuất HĐĐT" riêng + key; fix `Enabled` hiệu dụng (WaitingManager khóa form cha lúc load) bằng guard `this.Enabled` + `EnabledChanged/VisibleChanged` UC + `BeginInvoke`. |
| 25/08/2026 | nampp | Việc 3082 v3: checkbox "In" tại màn Xuất bán, "Lưu in" mở form hóa đơn tự động; partial `UCExpMestSaleCreate___SaveSignPrintInvoice.cs`; tạo tài liệu module. |
| 08/08/2026 | nampp | Việc 3082: khóa checkbox "Xuất biên lai/hóa đơn" khi key bật (đã gỡ 29/08); gỡ entry `licenses.licx` rỗng trong csproj. |
| (trước 2026) | team | Việc 36371 tạo chức năng Xuất bán; 38766 sửa xuất bán; các sửa POS/QR/2 sổ… |

## 9. Test Cases

### Lưu / Lưu in
- [ ] Kê thuốc đủ tồn → Lưu (F5): phiếu tạo, trạng thái theo cấu hình kho; nút Hủy xuất enable khi phiếu HOÀN THÀNH.
- [ ] Lưu in (F9): in phiếu xuất bán / hóa đơn biên lai giấy như cũ (có hoặc không tick "Xuất biên lai/hóa đơn").
- [ ] Kê vượt tồn khả dụng: bị chặn khi thêm dòng.

### Việc 3082 v3.2 — nút "Lưu ký in"
- [ ] Mở màn, chưa tick "Xuất biên lai/hóa đơn": nút "Lưu ký in" hiện nhưng **disable**.
- [ ] Tick → enable ngay; bỏ tick → disable ngay; sau khi lưu (Lưu in disable) → nút disable; Mới → theo tick. Đóng mở lại màn (checkbox nhớ ControlState) → nút theo.
- [ ] Tick + Lưu ký in (Ctrl E / F11), đủ tồn: phiếu + bill tạo (Mã giao dịch hiện), không in phiếu xuất bán; form Xuất hóa đơn hiện, tự phát hành HĐĐT + thực xuất + in rồi đóng; màn khóa Lưu/Lưu in/Lưu ký in.
- [ ] Tick + Lưu ký in, thiếu tồn: popup thiếu; bill có, HĐĐT không; form đóng.
- [ ] Lưu ký in nhưng validate màn fail (thiếu BN…): không lưu; bấm Lưu (F5) ngay sau đó KHÔNG mở form hóa đơn (flag đã reset).
- [ ] Hình thức QR + Lưu ký in: lưu + module QR như cũ, không mở form tự động.
- [ ] Nhiều bệnh nhân 1 lượt lưu: form mở lần lượt từng bill.
- [ ] Key `Show_MedicineSaleBill` = 1 + Lưu ký in: form chỉ mở 1 lần (chế độ tự động).
