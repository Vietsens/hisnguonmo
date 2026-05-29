# HIS.Desktop.Plugins.TransactionBillTwoInOne — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TransactionBillTwoInOne |
| Loại | Form (FormBase) |
| Mục đích | Thanh toán viện phí 2 sổ (Biên lai viện phí + Hóa đơn dịch vụ) trong cùng một thao tác cho thu ngân |
| Trạng thái | Đang sử dụng |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Thu ngân tìm bệnh nhân theo mã điều trị → load dịch vụ phải thu.
2. Chọn dịch vụ tính vào sổ Biên lai viện phí và sổ Hóa đơn dịch vụ.
3. Chọn hình thức thanh toán, hoàn ứng, kết chuyển.
4. Lưu (Ctrl+S) → backend tạo đồng thời 1-2 giao dịch TT + (tùy chọn) giao dịch HU (hoàn ứng).
5. Sau khi Lưu thành công: in biên lai/hóa đơn theo chế độ cấu hình.

### Mở rộng — Chiết khấu nhiều dòng (key MOS.HIS_TRANSACTION_ENABLE_MULTI_DISCOUNT)
- Mặc định: 3 ô (Chiết khấu đ + Chiết khấu % + Lý do) như cũ.
- Khi key bật ("1"): ẩn 3 ô cũ ở cả "Hóa đơn viện phí" và "Hóa đơn dịch vụ"; thay bằng GridControl 3 cột: Chiết khấu (đ), Chiết khấu (%), Lý do (max 250).
- Nhập cột (đ) → cột (%) tự sinh: `% = (đ ÷ tổng tiền gốc) × 100`, làm tròn về số nguyên (backend DISCOUNT_RATIO kiểu long?).
- Nhập cột (%) → cột (đ) tự sinh: `đ = (% × tổng tiền gốc) ÷ 100`.
- Tick "Không TT" → grid disable + clear data.
- Label "Số tiền" hiển thị = `tổng tiền - tổng cột Chiết khấu (đ)`.
- Khi Lưu — gắn thẳng vào `HIS_TRANSACTION` (pattern nested giống `HIS_BILL_FUND`):
  - `EXEMPTION` = tổng cột Chiết khấu (đ) trong grid tương ứng.
  - `EXEMPTION_REASON` = các Lý do nối bằng dấu `;` (skip lý do rỗng).
  - `HIS_TRANSACTION_DISCOUNT` (`ICollection<HIS_TRANSACTION_DISCOUNT>`): danh sách dòng trong grid:
    - `ID` = 0 nếu mới, giữ ID cũ nếu sửa.
    - `TRANSACTION_ID` = 0 (backend gán sau khi tạo HIS_TRANSACTION).
    - `DISCOUNT` (decimal?), `DISCOUNT_RATIO` (long?), `REASON`, `TREATMENT_ID`.

### Tính tách tiền 2 sổ qua thư viện MOS.LibraryBillTwoBook (Strategy + Factory)
- Mỗi dịch vụ (V_HIS_SERE_SERV_5) được tách thành tiền Biên lai (viện phí) và tiền Hóa đơn (dịch vụ) tùy cấu hình hệ thống `MOS.HIS_TRANSACTION.BILL_TWO_BOOK.OPTION` (`HisConfig.BILL_TWO_BOOK__OPTION`).
- Frontend KHÔNG còn rẽ nhánh theo từng cấu hình. Thay vào đó:
  ```csharp
  IBillTwoBookCalculator calculator = BillCalculatorFactory.Create(HisConfig.BILL_TWO_BOOK__OPTION);
  BillCalcResult result = calculator.Calculate(new BillCalcInput { SereServ5 = item, PatientTypeIdBhyt = ..., PatientTypeIdFee = ..., PatientTypeIdService = ..., LstPatientType = lstPaty });
  // result.RecieptAmount → tiền biên lai; result.InvoiceAmount → tiền hóa đơn
  ```
- Các cách tính cũ (CTO_TW = 1, HCM_115 = 2, QBH_CUBA = 3) nằm trong thư viện. **Bổ sung cách tính mới = thêm calculator trong `MOS.LibraryBillTwoBook`, KHÔNG sửa frontend.**
- Riêng đánh dấu dịch vụ đã xuất biên lai/hóa đơn (`dicSereServBill`) vẫn ở frontend (trạng thái hiển thị, không phải tính tiền); giữ case đặc thù CTO_TW (ĐTTT/PRIMARY là Dịch vụ → đánh dấu đã vào hóa đơn).

### Mở rộng — Hoàn tiền ngân hàng (mới)
- Thu ngân tick checkbox "Hoàn tiền NH" (BẬT) trên màn hình Thanh toán.
- Khi tick "Tự động H/Ư" hoặc "Có kết chuyển" + tick "Hoàn tiền NH" → sau khi Lưu thành công và phát sinh giao dịch hoàn ứng → tự động mở form Hoàn tiền ngân hàng.
- Mặc định checkbox "Hoàn tiền NH" KHÔNG tick. Trạng thái nhớ qua phiên (ControlState).

### Điều kiện nghiệp vụ checkbox "Hoàn tiền NH"
| Điều kiện | Hành vi |
|-----------|---------|
| Tick + có giao dịch HU + có cấu hình + BN có thụ hưởng | Tự động mở form RefundByTransfer |
| Tick + KHÔNG có cấu hình `HIS.Desktop.Plugins.RefundByTransfer.*` | Hiện cảnh báo "Chưa cấu hình hoàn tiền ngân hàng!" rồi dừng |
| Tick + có cấu hình + BN CHƯA có thụ hưởng | Hiện cảnh báo "BN chưa có thông tin thụ hưởng. Vui lòng nhập thông tin thụ hưởng trước." rồi dừng (Lưu vẫn thành công, không rollback) |
| Tick + KHÔNG phát sinh giao dịch HU | Không làm gì thêm |
| Không tick | Hành vi cũ — không thay đổi |
| Lưu thất bại | Không mở form (dù tick) |

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TREATMENT_FEE | View | Thông tin điều trị + tổng tiền tạm ứng/đã thu |
| HIS_TRANSACTION | Table | Giao dịch TT, TU, HU |
| V_HIS_TRANSACTION | View | Hiển thị giao dịch trong session |
| HIS_BILL | Table | Hóa đơn |
| HIS_PATIENT_BANK_ACCOUNT | Table | Thông tin thụ hưởng BN (kiểm tra trước khi mở RefundByTransfer) |
| HIS_TREATMENT | Table | Truyền cho plugin RefundByTransfer |
| HIS_CONFIG | Table | Cấu hình `HIS.Desktop.Plugins.RefundByTransfer.*` |

## 4. UI Layout

### Sơ đồ giao diện (vùng dưới — bottom row y=661)
```
+------------------------------------------------------------------------+
|[☐] Hoàn tiền NH | [☐] Tự động H/Ư | [☐] Kết nối POS | ... [Lưu] [In]  |
+------------------------------------------------------------------------+
   ↑ MỚI THÊM
```

### Control mới
| Control | Kiểu | Vị trí | Kích thước | Mặc định |
|---------|------|--------|-----------|----------|
| `chkRefundByTransfer` | DevExpress.XtraEditors.CheckEdit | (96, 663) | 34×19 | Unchecked |
| `lciRefundByTransfer` | LayoutControlItem | (0, 661) | 132×26 | Caption: "Hoàn tiền NH:" |
| Tooltip | — | — | — | "Tự động mở form Hoàn tiền ngân hàng sau khi thanh toán có phát sinh giao dịch hoàn ứng" |

`emptySpaceItem1` được giảm Size từ (132, 26) → (0, 26) để nhường chỗ cho lciRefundByTransfer.

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Tạo bill 2 sổ | api/HisTransaction/CreateBillTwoBook | MosConsumer | HisTransactionBillTwoBookSDO |
| Kiểm tra bill 2 sổ | api/HisTransaction/CheckBillTwoBook | MosConsumer | HisTransactionBillTwoBookSDO |
| Lấy điều trị | api/HisTreatment/Get | MosConsumer | HisTreatmentFilter |
| Lấy thụ hưởng BN (mới gọi) | api/HisPatientBankAccount/Get | MosConsumer | HisPatientBankAccountFilter |
| Lấy view giao dịch | api/HisTransaction/GetView | MosConsumer | HisTransactionViewFilter |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.ElectronicBill | Tạo hóa đơn điện tử |
| HIS.Desktop.Library.CacheClient | ControlState |

### Thư viện DLL (backend)
| Thư viện | Mục đích |
|----------|----------|
| MOS.LibraryBillTwoBook | Tách tiền 2 sổ. API mới: `BillCalculatorFactory.Create(billOption)` → `IBillTwoBookCalculator.Calculate(BillCalcInput)` → `BillCalcResult { RecieptAmount, InvoiceAmount }`. Strategy theo `BILL_TWO_BOOK.OPTION` |

### Inter-Plugin (mở plugin khác)
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.ConfigPrinter | Click "Cấu hình máy in" | List<MPS_PRINT_TYPE> |
| HIS.Desktop.Plugins.TransactionRepay | Khi cần hoàn ứng tay | TransactionRepayADO |
| HIS.Desktop.Plugins.CreateTransReqQR | Khi tạo QR thanh toán | List<HIS_TRANSACTION>, callback |
| HIS.Desktop.Plugins.RefundByTransfer (MỚI) | Sau Lưu success + tick "Hoàn tiền NH" + có giao dịch HU + BN có thụ hưởng | HIS_TREATMENT, HIS_TRANSACTION (HU), bankCode (string), HIS.Desktop.Common.RefeshReference callback |

## 7. Print

Sử dụng MPS Print Library qua `RichEditorStore`. Các template chính: Mps000147 (Hóa đơn), Mps000148 (Biên lai).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 04/05/2026 | phuongnm@vietsens.vn | Thêm checkbox "Hoàn tiền NH" tại bottom row (bên trái "Tự động H/Ư"). Khi tick + Lưu thành công + có giao dịch HU + BN có thụ hưởng → tự động mở plugin RefundByTransfer. Trạng thái checkbox nhớ qua ControlState. Thêm 2 thông báo "Chưa cấu hình hoàn tiền ngân hàng!" và "BN chưa có thông tin thụ hưởng...". Không sửa logic Lưu hiện có; chỉ hook thêm vào sau khi success. |
| 29/05/2026 | anhnh2@vietsens.vn | Refactor cơ chế gọi `MOS.LibraryBillTwoBook` trong `LoadListSereServ()`. Bỏ `BillTwoBookPriceProcessor` + 3 nhánh `if/else` theo cấu hình (gọi riêng `Hcm115Calculator`/`QbhCubaCalcualator`/`CtoTWCalcualator`). Thay bằng `IBillTwoBookCalculator calculator = BillCalculatorFactory.Create(HisConfig.BILL_TWO_BOOK__OPTION)` rồi `calculator.Calculate(BillCalcInput)` lấy `RecieptAmount`/`InvoiceAmount` đã tách. Bổ sung cách tính mới không phải sửa frontend. Logic đánh dấu đã xuất biên lai/hóa đơn (`dicSereServBill`, gồm case đặc thù CTO_TW) giữ nguyên — bảo toàn hành vi. Thêm `using MOS.LibraryBillTwoBook.Calculator;`. |
| 28/05/2026 | anhnh2@vietsens.vn | Thêm xử lý key `MOS.HIS_TRANSACTION_ENABLE_MULTI_DISCOUNT`. Khi bật: chuyển 3 ô (Chiết khấu đ + % + Lý do) cả 2 màn (Viện phí + Dịch vụ) sang GridControl nhiều dòng. Auto-tính đ ↔ % (cột % làm tròn số nguyên vì backend `DISCOUNT_RATIO` kiểu `long?`). Lý do max 250. Tick "Không TT" disable grid + clear. Label "Số tiền" trừ tổng cột (đ). Khi Lưu: `EXEMPTION` = tổng (đ), `EXEMPTION_REASON` = nối `;`, gán `HIS_TRANSACTION.HIS_TRANSACTION_DISCOUNT` = list (entity `MOS.EFMODEL.DataModels.HIS_TRANSACTION_DISCOUNT`). File mới: `ADO/TransactionDiscountADO.cs`, partial `frmTransactionBillTwoInOne__Plus__GridDiscount.cs` dựng GridControl + LayoutControlItem runtime (KHÔNG đụng Designer.cs). Khi key tắt: giữ nguyên hành vi cũ. |

## 9. Test Cases

### Mặc định (không tick)
- [ ] Mở form lần đầu → checkbox "Hoàn tiền NH" KHÔNG tick.
- [ ] Tick → đóng/mở form lần sau → checkbox vẫn tick (ControlState nhớ).
- [ ] Bỏ tick → Lưu → không mở form RefundByTransfer (hành vi cũ).

### Tick + Tự động H/Ư
- [ ] Tick "Hoàn tiền NH" + "Tự động H/Ư" + Lưu → có giao dịch HU phát sinh → mở form RefundByTransfer với data đúng (treatment, transaction HU, bankCode).
- [ ] Tick "Hoàn tiền NH" nhưng không có HU phát sinh → không mở form, không có thông báo.

### Tick + Kết chuyển
- [ ] Tick "Hoàn tiền NH" + "Có kết chuyển" + Lưu → mở form RefundByTransfer.

### Lỗi
- [ ] Tick "Hoàn tiền NH" + viện không có cấu hình `HIS.Desktop.Plugins.RefundByTransfer.*` → hiện thông báo "Chưa cấu hình hoàn tiền ngân hàng!", không mở form.
- [ ] Tick "Hoàn tiền NH" + BN chưa có HIS_PATIENT_BANK_ACCOUNT → hiện thông báo "BN chưa có thông tin thụ hưởng...", không mở form, Lưu vẫn thành công.
- [ ] Lưu thất bại (validation hoặc API) → không mở form dù tick.

### Đa ngôn ngữ
- [ ] Đổi sang English → caption "Bank Refund:" + tooltip dịch.

### Chiết khấu nhiều dòng (MOS.HIS_TRANSACTION_ENABLE_MULTI_DISCOUNT)
- [ ] Key TẮT (hoặc thiếu) → form hiển thị 3 ô như cũ; thao tác/Lưu y hệt hành vi cũ.
- [ ] Key BẬT → ẩn 3 ô; GridControl hiện ở cả 2 màn (Viện phí + Dịch vụ).
- [ ] Nhập cột (đ) → cột (%) tự cập nhật `% = đ ÷ tổng × 100` (làm tròn integer).
- [ ] Nhập cột (%) → cột (đ) tự cập nhật `đ = % × tổng ÷ 100`.
- [ ] Lý do nhập > 250 ký tự → bị giới hạn ở 250.
- [ ] Tick "Không TT" → grid disable + clear sạch dòng.
- [ ] Label "Số tiền" = tổng - tổng cột (đ); cập nhật realtime khi sửa cell.
- [ ] Lưu thành công với 3 dòng → `EXEMPTION` = tổng đ; `EXEMPTION_REASON` = 3 lý do nối `;`; `HIS_TRANSACTION_DISCOUNT` lưu đủ 3 bản ghi (BE tự cấp ID + TRANSACTION_ID).
