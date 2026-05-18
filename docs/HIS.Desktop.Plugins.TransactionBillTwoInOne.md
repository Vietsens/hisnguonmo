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
