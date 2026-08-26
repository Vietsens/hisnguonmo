# Xuất Bán (Kho Nhà Thuốc) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExpMestSaleCreate |
| Loại | UC (UCExpMestSaleCreate — UserControlBase), 8 partial (`___Proccess`, `__Load`, `__InitPrint`, `__Plus_Search`, `__Shortcut`, `__MenuMouseRight`, `__Validate`, `___SaveSignPrintInvoice`) |
| Mục đích | Bán thuốc/vật tư tại kho nhà thuốc: tạo/sửa phiếu xuất bán (theo đơn hoặc vãng lai), in phiếu xuất bán, mở form Xuất hóa đơn (MedicineSaleBill), hủy xuất, xác nhận nợ. Menu "Nhập xuất tồn > Xuất bán". |
| Người tạo | team (việc 36371) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
Chọn kho xuất → nhập BN (mã điều trị/mã đơn/vãng lai) → kê thuốc/vật tư (kiểm tra tồn khả dụng khi thêm dòng) → **Lưu (F5)** hoặc **Lưu in (F9)** → `ProcessSave`: POST `api/HisExpMest/SaleCreateListSdo` (1 BN) / `SaleCreateBillList` (nhiều BN) / `SaleUpdateListSdo` (sửa) → in phiếu xuất bán (Mps) nếu Lưu in → **Xuất hóa đơn (F10)** mở form `HIS.Desktop.Plugins.MedicineSaleBill` để tạo bill/HĐĐT.

### Trạng thái phiếu sau lưu (phụ thuộc BE)
Kho `HIS_MEDI_STOCK_EXTY.IS_AUTO_APPROVE/IS_AUTO_EXECUTE` + `MOS.EXP_MEST.EXPORT_SALE.MUST_BILL` quyết định phiếu về Yêu cầu / Đã duyệt / Hoàn thành ngay khi lưu; `btnCancelExport` (Hủy xuất) chỉ enable khi có phiếu HOÀN THÀNH.

### Checkbox "In" + Lưu in (việc 3082 — v3 25/08/2026)
- Checkbox `chkPrintInvoice` ("In", cạnh "Ký đơn nhà thuốc", trước "Xem trước khi in") **chỉ hiện khi** key `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport` = 1; nhớ trạng thái (ControlState `CHK_PRINT_INVOICE`).
- Tick "In" + **Lưu in**: `savePrintInvoice = true`, `savePrint = false` → lưu phiếu **không in phiếu xuất bán** → sau `MessageManager.Show` gọi `OpenMedicineSaleBillAutoSignPrint()`: mở form Xuất hóa đơn cho từng kết quả lưu (1/nhiều BN) với args `(Module, List<long> expMestIds chưa có bill, DelegateSelectData EnableControlAfterSaveSaleBill, List<string> { "AUTO_SAVE_SIGN_PRINT" })`. Form tự chạy: kiểm tra tồn → Lưu ký (bill + HĐĐT) → tự duyệt/thực xuất phần còn thiếu → in thẳng → tự đóng (xem docs MedicineSaleBill). Nhánh key `ExpMestSaleCreate__Show_MedicineSaleBill` bị bỏ qua trong lượt này để không mở form 2 lần.
- Flag `savePrintInvoice` reset trong `finally` của `btnSavePrint_Click` (validate fail không lọt sang lượt Lưu kế tiếp) và trong catch của `ProcessSave`.
- Key bật đồng thời **bỏ tick + khóa** "Xuất biên lai/hóa đơn" (`chkCreateBill`): nếu tick, phiếu có bill ngay khi lưu → form hóa đơn (lọc HAS_BILL_ID = false) mở ra trống.
- Không tick / key tắt: luồng cũ 100%.

### Điều kiện nghiệp vụ
- Không kê vượt tồn khả dụng (validation khi thêm dòng).
- Hình thức Tiền mặt/CK, Tiền mặt/QT: số tiền CK/QT không vượt tổng phải thanh toán.
- POS (`ChkKetNoiPOS` + `chkCreateBill`): gọi máy POS trước khi lưu.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_MEDI_STOCK | View | Kho xuất |
| HisMedicineTypeInStockSDO / HisMaterialTypeInStockSDO | SDO | Tồn khả dụng theo loại thuốc/vật tư |
| HisExpMestSaleListSDO / HisExpMestSaleListResultSDO | SDO | Dữ liệu lưu phiếu bán + kết quả (ExpMestSdos, Transaction) |
| HIS_EXP_MEST | Table | Phiếu xuất bán (EXP_MEST_STT_ID, BILL_ID) |
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
| Tự động hiển thị tồn ☐ | Ký đơn nhà thuốc ☐ | [In ☐] | Xem trước khi in ☑ |  |
| Hủy xuất (Ctrl H) | Lưu in (F9) | Lưu (F5) | Mới (F8) | Đơn mới (F7) | In ▾ |
| QR | Xuất hóa đơn (F10) | Xác nhận nợ                                        |
+------------------------------------------------------------------------------+
```
`[In ☐]` = `chkPrintInvoice` (việc 3082, ẩn khi key tắt). Phím tắt F5/F7/F8/F9/F10 khi key `ExpMestSaleCreate.IsUsingFunctionKeyInsteadOfCtrlKey` = 1, ngược lại Ctrl S/D/N/I/T.

### UC sử dụng
| UC | Mục đích |
|----|----------|
| HIS.UC.Icd + HIS.UC.SecondaryIcd | Chẩn đoán chính/phụ |
| HIS.UC.MedicineTypeInStock / MaterialTypeInStock | Chọn thuốc/vật tư còn tồn |
| HIS.UC.ExpMestMedicineGrid / ExpMestMaterialGrid | Lưới chi tiết phiếu |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lưu phiếu bán 1 BN | api/HisExpMest/SaleCreateListSdo | MosConsumer |
| Lưu phiếu bán nhiều BN | api/HisExpMest/SaleCreateBillList | MosConsumer |
| Sửa phiếu bán | api/HisExpMest/SaleUpdateListSdo | MosConsumer |
| Đơn thuốc nguồn | api/HisServiceReq/Get | MosConsumer |
| Bệnh nhân | api/HisPatient/Get | MosConsumer |

## 6. Dependencies

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.MedicineSaleBill | Nút Xuất hóa đơn (F10); key `Show_MedicineSaleBill` = 1 sau lưu | Module, List<long> expMestIds, DelegateSelectData |
| HIS.Desktop.Plugins.MedicineSaleBill (chế độ tự động, 3082 v3) | Lưu in + tick "In" (key 3082 bật), sau lưu thành công | Module, List<long> expMestIds (chưa có bill), DelegateSelectData, **List<string> { "AUTO_SAVE_SIGN_PRINT" }** |
| HIS.Desktop.Plugins.CreateTransReqQR | Lưu có tạo bill với hình thức QR | TransReqQRADO |

### Library
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Library.CacheClient.ControlStateWorker | Nhớ checkbox (Xem trước khi in, Xuất biên lai, POS, Ký đơn, **In**) |
| Inventec.Common.RichEditor.RichEditorStore | In phiếu xuất bán / hóa đơn biên lai / HDSD |

## 7. Print

| Loại in | PrintTypeCode | Ghi chú |
|---------|--------------|---------|
| Phiếu xuất bán | theo `InitMenuPrint` (menu In) | `onClickInPhieuXuatBan` — Lưu in không tick "In" |
| Hóa đơn/biên lai xuất bán | Mps000339 | `onClickInHoaDonBienLaiXuatBan` — khi tick Xuất biên lai/hóa đơn |
| Hướng dẫn sử dụng thuốc | Mps000099 | menu In |
| Hóa đơn điện tử (3082) | — | In thẳng tại form MedicineSaleBill (DocumentViewerManager.Print) |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 25/08/2026 | nampp | Việc 3082 **v3** (tài liệu 3082 cập nhật 25/08): thêm checkbox "In" (`chkPrintInvoice` + `lciPrintInvoice`, chèn giữa "Ký đơn nhà thuốc" (thu 185→150) và "Xem trước khi in"), hiện theo key `HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport`, ControlState `CHK_PRINT_INVOICE`. Partial mới `UCExpMestSaleCreate___SaveSignPrintInvoice.cs`: `IsSavePrintInvoiceMode`, `SetPrintInvoiceCheckboxByConfig`, `OpenMedicineSaleBillAutoSignPrint` (mở form hóa đơn với marker `AUTO_SAVE_SIGN_PRINT` cho từng kết quả lưu). `btnSavePrint_Click`: tick In → `savePrintInvoice = true`, không in phiếu xuất bán; `ProcessSave`: sau lưu OK mở form tự động (thay nhánh `Show_MedicineSaleBill`). Tooltip khóa "Xuất biên lai/hóa đơn" cập nhật. Tạo tài liệu module. |
| 08/08/2026 | nampp | Việc 3082: bổ sung khóa checkbox "Xuất biên lai/hóa đơn" khi key bật (bản V1 trước đó thiếu, chỉ có V2); gỡ entry `licenses.licx` rỗng trong csproj. |
| (trước 2026) | team | Việc 36371 tạo chức năng Xuất bán; 38766 sửa xuất bán; các sửa POS/QR/2 sổ… |

## 9. Test Cases

### Lưu / Lưu in
- [ ] Kê thuốc đủ tồn → Lưu (F5): phiếu tạo, trạng thái theo cấu hình kho; nút Hủy xuất enable khi phiếu HOÀN THÀNH.
- [ ] Lưu in (F9) không tick "In": in phiếu xuất bán như cũ.
- [ ] Kê vượt tồn khả dụng: bị chặn khi thêm dòng.

### Việc 3082 v3
- [ ] Key tắt: không thấy checkbox "In"; "Xuất biên lai/hóa đơn" dùng bình thường.
- [ ] Key bật: checkbox "In" hiện cạnh "Ký đơn nhà thuốc"; "Xuất biên lai/hóa đơn" bị bỏ tick + khóa (tooltip).
- [ ] Tick "In" + Lưu in (đủ tồn): không in phiếu xuất bán; form Xuất hóa đơn hiện, tự Lưu ký + thực xuất + in HĐĐT rồi đóng; màn khóa Lưu/Lưu in.
- [ ] Tick "In" + Lưu in (thiếu tồn): popup thiếu, form đóng, phiếu còn ở màn để sửa.
- [ ] Tick "In" nhưng validate màn Xuất bán fail (thiếu BN…): không lưu; bấm Lưu (F5) ngay sau đó KHÔNG mở form hóa đơn (flag đã reset).
- [ ] Nhiều bệnh nhân 1 lượt lưu: form hóa đơn mở lần lượt từng phiếu.
- [ ] Đóng mở lại màn: checkbox "In" nhớ trạng thái.
- [ ] Key `Show_MedicineSaleBill` = 1 + tick In: form chỉ mở 1 lần (chế độ tự động).
