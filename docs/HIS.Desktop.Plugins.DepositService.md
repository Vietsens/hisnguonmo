# Tạm ứng theo dịch vụ — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.DepositService |
| Loại | Form |
| Mục đích | Tạo giao dịch tạm ứng theo từng dịch vụ (sere_serv) đã chỉ định trong điều trị. Người dùng chọn các dịch vụ cần tạm ứng trên cây dịch vụ, nhập số tiền, hình thức thanh toán, lý do giao dịch rồi lưu để tạo HIS_TRANSACTION (loại TU). |
| Người tạo | huannh |
| Ngày tạo | 21/05/2026 |
| Trạng thái | Đang phát triển |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở form từ menu Tạm ứng dịch vụ → truyền `treatmentId` hoặc `V_HIS_TREATMENT_FEE`, `cashierRoomId`, `branchId`, danh sách `V_HIS_SERE_SERV_5` (tùy biến), delegate refresh.
2. Form load:
   - Load HIS_TREATMENT_FEE → hiển thị info bệnh nhân.
   - Load danh sách dịch vụ chưa thanh toán → đổ lên tree (UC `HIS.UC.SereServTree`).
   - Init combo Hình thức thanh toán, Sổ kế toán, Ngân hàng, Phòng thu, Lý do giao dịch.
   - Mặc định Lý do giao dịch = `Khám` (theo `TDL_TREATMENT_TYPE_ID`).
3. User chọn dịch vụ + nhập số tiền + chọn hình thức/lý do → nhấn `Lưu (Ctrl+S)` / `Lưu In (Ctrl+I)` / `Lưu In và Thanh toán (Ctrl+T)`.
4. Validate (số tiền >= 0, ngân hàng khi cần, số tiền chuyển khoản khớp).
5. Build `HisTransactionDepositSDO` → gọi API `HisTransaction/CreateDeposit` → backend tạo HIS_TRANSACTION + HIS_SERE_SERV_DEPOSIT.
6. Thành công → trả về `V_HIS_TRANSACTION` qua `SendResultToOtherForm` delegate; mở printer nếu chọn `Lưu In`.

### Điều kiện nghiệp vụ
- Chỉ tạm ứng được những dịch vụ chưa có sere_serv_bill chưa hủy.
- Số tiền tạm ứng mỗi dòng <= số tiền bệnh nhân phải trả còn lại.
- BHYT: nếu PATIENT_TYPE_ID == BHYT thì AMOUNT = `VIR_TOTAL_PATIENT_PRICE`.
- TRANSACTION_TYPE_ID cố định = `HIS_TRANSACTION_TYPE.ID__TU` (Tạm ứng).
- TRANSACTION_REASON_ID bắt buộc gửi backend kèm DTO; mặc định FE chọn `Khám`, user có thể đổi sang `Điều trị` hoặc lý do mở rộng do danh mục cung cấp.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TREATMENT_FEE | View | Thông tin điều trị + viện phí (TDL_TREATMENT_TYPE_ID dùng default Lý do GD) |
| V_HIS_TRANSACTION | View | Hiển thị giao dịch (sau khi save) |
| HIS_TRANSACTION | Table | Bản ghi giao dịch tạm ứng được tạo |
| HIS_TRANSACTION_REASON | Table | Danh mục Lý do giao dịch (Khám / Điều trị / mở rộng) — đọc qua API `HisTransactionReason/Get` |
| V_HIS_SERE_SERV_5 | View | Danh sách dịch vụ trên cây |
| HIS_SERE_SERV_BILL | Table | Lọc dịch vụ đã thanh toán |
| HIS_PAY_FORM | Table | Hình thức thanh toán |
| V_HIS_CASHIER_ROOM | View | Phòng thu |
| V_HIS_ACCOUNT_BOOK | View | Sổ kế toán |
| HIS_PATIENT_TYPE | Table | Đối tượng BN (BHYT/Phí) |
| HIS_HOLIDAY_POLICIES | Table | Chính sách ngày lễ — kiểm tra thời gian giao dịch |
| HisTransactionDepositSDO | SDO | DTO request `HisTransaction/CreateDeposit` (chứa Transaction + SereServDeposits) |
| HisTransactionReasonFilter | Filter | Filter cho `HisTransactionReason/Get` |

### Quan hệ chính
- `HIS_TRANSACTION.TRANSACTION_REASON_ID` → `HIS_TRANSACTION_REASON.ID` (phân loại báo cáo).
- `HIS_TRANSACTION` 1-n `HIS_SERE_SERV_DEPOSIT` (chi tiết tạm ứng theo từng dịch vụ).
- `V_HIS_SERE_SERV_5.TDL_TREATMENT_ID` = `V_HIS_TREATMENT_FEE.ID`.

## 4. UI Layout

### Sơ đồ giao diện
```
+-----------------------------------------------------------------------+
| Tree dịch vụ (panelControlTreeSereServ — HIS.UC.SereServTree)         |
|  [ ] Nhóm DV | DV | Số lượng | Đơn giá | BHYT | BN trả | Tạm ứng     |
+-----------------------------------------------------------------------+
| Sổ kế toán | Mã | HTTT | Ngân hàng | TG giao dịch     | Số chứng từ  |
| [Số tiền QT] [4 số cuối thẻ NH] [Số tiền BN đưa] [Số tiền trả BN]    |
| Mô tả      | Mã giao dịch | Lý do giao dịch          | Tiền mặt: ... |
| [Tự đóng][QR][Lưu ký][Lưu (Ctrl+S)][Lưu In(Ctrl+I)][Lưu In + TT][In]  |
+-----------------------------------------------------------------------+
```

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.SereServTree | panelControlTreeSereServ | Cây dịch vụ để chọn tạm ứng |

### Combo / LookUp chính
- `cboPayForm` — LookUpEdit, danh sách HIS_PAY_FORM (TM, TMCK, TMQT, TMCKQT).
- `cboAccountBook` — GridLookUpEdit, V_HIS_ACCOUNT_BOOK theo phòng thu.
- `cboBank` — GridLookUpEdit, HIS_BANK (khi PAY_FORM là CK/QT/CKQT).
- `cboTransactionReason` — GridLookUpEdit, HIS_TRANSACTION_REASON. ValueMember=`ID`, DisplayMember=`TRANSACTION_REASON_NAME`. Mặc định `Khám` khi mở form. Lưu vào `Transaction.TRANSACTION_REASON_ID` khi save.

### Phím tắt (BarManager)
- Ctrl+S: Lưu
- Ctrl+I: Lưu + In
- Ctrl+T: Lưu In + Thanh toán
- Ctrl+N: Mới

## 5. API Endpoints

| Action | URI | Consumer | Filter / Body |
|--------|-----|----------|---------------|
| Tạo tạm ứng dịch vụ | `api/HisTransaction/CreateDeposit` | MosConsumer | HisTransactionDepositSDO (gồm `Transaction.TRANSACTION_REASON_ID`) |
| Lấy lý do giao dịch | `api/HisTransactionReason/Get` | MosConsumer | HisTransactionReasonFilter (IS_ACTIVE=1, ORDER_FIELD=TRANSACTION_REASON_CODE ASC) |
| Lấy điều trị | `api/HisTreatment/GetView` | MosConsumer | HisTreatmentFilter |
| Lấy DV theo điều trị | `api/HisSereServ/GetView5` | MosConsumer | HisSereServFilter |
| Lấy SS đã thanh toán | `api/HisSereServBill/Get` | MosConsumer | HisSereServBillFilter |
| Lấy chính sách ngày lễ | `api/HisHolidayPolicies/Get` | MosConsumer | HisHolidayPoliciesFilter |

URI hardcode trong code (form gọi trực tiếp), không gom RequestUriStore.

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.EmrGenerate | Tạo input ký số khi in (nếu bật) |
| HIS.Desktop.Library.CacheClient | ControlStateWorker — lưu trạng thái checkbox Tự đóng |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| (form gốc — nhận data) | Sau save | `V_HIS_TRANSACTION` qua delegate `SendResultToOtherForm` |
| HIS.Desktop.Plugins.TransactionReason | (không trực tiếp) | Đọc danh mục qua API `HisTransactionReason/Get` |

## 7. Print

| Loại in | PrintTypeCode | Library / MPS | Template |
|---------|--------------|--------------|----------|
| Phiếu thu tạm ứng (Lưu In / Lưu In + TT) | Tùy config `dicPrinter` | MPS.MpsPrinter | Mẫu thu tạm ứng theo Mps tương ứng |

Print logic: `frmDepositServicePlus_Print.cs` + `DepositServicePrintProcess.cs`.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 04/06/2026 | huannh | **Thêm nhiều hình thức thanh toán (MULTI_PAYFORM):** thêm config `MOS.HIS_TRANSACTION.MULTI_PAYFORM` → `HisConfigCFG.IsMultiPayForm`. Khi bật: tích hợp UC `HIS.UC.TransactionPayformGrid` (khai báo trong Designer là `lciPayformGrid`/`panelPayformGrid`, full-width 1 hàng tại y=609 — dưới khu hình thức, trên hàng mô tả/nút; dời các item y≥609 xuống +170), ẩn các control hình thức đơn lẻ (`layoutControlItem5` Hình thức, `layoutControlItem21` Ngân hàng, `lblAmount` Số tiền, `lciTranferAmount`, `lblTransferNew`, `lblSwipeNew`) + gỡ validation rule. Số tiền phải thu của UC = `txtAmount.Tag` (tính từ dịch vụ chọn ở SereServTree), cập nhật trong `ChangeCheckedNodes`. Khi Lưu: `SaveProcess` → guard validate UC + bỏ qua luồng WCF thẻ/POS; `UpdateDataFormMultiPayForm` map `PayformRowADO` sang `HisTransactionDepositSDO.PayformDetails` (giữ `SereServDeposits`, `AMOUNT`=txtAmount.Tag) khi gọi `CreateDeposit`. UC không đụng `panelControlTreeSereServ`. Thêm message `VuiLongNhapHinhThucThanhToan` (vi/en/my) + reference `HIS.UC.TransactionPayformGrid`. |
| 21/05/2026 | huannh | Tạo tài liệu module. Bổ sung combo `Lý do giao dịch` cạnh ô `Mã giao dịch`. Mặc định `Khám` khi mở form (logic: `TDL_TREATMENT_TYPE_ID == HIS_TREATMENT_TYPE.ID__KHAM` hoặc không có treatment → Khám; còn lại → Điều trị). Khi lưu, gửi `TRANSACTION_REASON_ID` vào DTO tạo tạm ứng dịch vụ (`HisTransaction/CreateDeposit`). |

## 9. Test Cases

### Tạo mới
- [ ] Mở form với điều trị diện Khám → cbo Lý do giao dịch mặc định `Khám`.
- [ ] Mở form với điều trị đã vào nội trú → cbo Lý do giao dịch mặc định `Điều trị`.
- [ ] Mở form không có treatment context → cbo mặc định `Khám`.
- [ ] Chọn dịch vụ + nhập số tiền + giữ default lý do → Lưu thành công, backend nhận `TRANSACTION_REASON_ID = Khám`.
- [ ] Đổi sang lý do `Điều trị` → Lưu → backend nhận đúng ID `Điều trị`.

### Combo Lý do giao dịch
- [ ] Mở dropdown → hiển thị danh mục `HIS_TRANSACTION_REASON` IS_ACTIVE=1, sort theo TRANSACTION_REASON_CODE ASC.
- [ ] Danh mục có thêm record mới → mở lại form thấy ngay (không cache local).
- [ ] Gõ keyword → ImmediatePopup hiện danh sách lọc.

### Lưu giao dịch
- [ ] Lưu → backend trả về V_HIS_TRANSACTION có TRANSACTION_REASON_ID đúng.
- [ ] Lưu In → preview/in phiếu thu, không lỗi.
- [ ] Lưu In + Thanh toán → mở luồng thanh toán kế tiếp.

### Đa ngôn ngữ
- [ ] Chuyển sang en → caption `Transaction reason:`.
- [ ] Chuyển sang vi → caption `Lý do giao dịch:`.
- [ ] Chuyển sang my → caption Myanmar.

### Hồi quy
- [ ] Các trường khác (Mô tả, Mã giao dịch, Tiền mặt) hiển thị đúng vị trí, không bị đè layout.
- [ ] Row y=609 căn cột thẳng hàng với row Ngân hàng y=537 (Mã giao dịch | Lý do | Tiền mặt).
