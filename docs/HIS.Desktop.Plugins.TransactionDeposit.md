# Tạm Ứng (Transaction Deposit) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TransactionDeposit |
| Loại | Form |
| Mục đích | Tạo giao dịch tạm ứng cho bệnh nhân (theo điều trị hoặc yêu cầu tạm ứng), hỗ trợ nhiều hình thức thanh toán (tiền mặt, chuyển khoản, quẹt thẻ, QR, thẻ thanh toán). |
| Trạng thái | Đang phát triển — bổ sung Lý do giao dịch (TRANSACTION_REASON_ID) |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở form từ chức năng Thu ngân hoặc từ Tiếp đón (truyền `treatment` hoặc `depositReq`).
2. Hệ thống tự fill thông tin bệnh nhân + số tiền mặc định (`MinimumDepositAmount`).
3. Người dùng chọn **Sổ thu chi**, **Hình thức thanh toán**, **Ngân hàng** (nếu cần), **Lý do giao dịch**, nhập mô tả.
4. Lưu giao dịch → API `HisTransaction/CreateDeposit` → in phiếu (nếu chọn).

### Mặc định Lý do giao dịch
- `TDL_TREATMENT_TYPE_ID == HIS_TREATMENT_TYPE.ID__KHAM` → chọn dòng có name chứa "Khám".
- `TDL_TREATMENT_TYPE_ID` khác (điều trị ngoại trú, nội trú, bán ngày, …) → chọn dòng có name chứa "Điều trị".
- Mở form không có treatment context → mặc định "Khám".
- Không tìm thấy match → chọn dòng đầu tiên trong danh mục.

### Điều kiện nghiệp vụ
- Số tiền tạm ứng phải >= `HisConfigCFG.MinimumDepositAmount`.
- Nếu hình thức là Thẻ → bắt buộc gọi WCF Card POS trước khi gọi MOS.
- Nếu là QR → tạo mã QR sau khi tạo giao dịch thành công.
- Sổ thu chi phải còn (chưa hết hạn ca làm việc).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_TRANSACTION | Table | Giao dịch tạm ứng (có TRANSACTION_REASON_ID) |
| V_HIS_TRANSACTION | View | Hiển thị giao dịch sau khi tạo |
| HIS_TRANSACTION_REASON | Table | **(MỚI)** Danh mục lý do giao dịch |
| V_HIS_TREATMENT_FEE | View | Thông tin điều trị + tài chính BN |
| V_HIS_DEPOSIT_REQ | View | Yêu cầu tạm ứng |
| HIS_DEPOSIT_REASON | Table | Lý do tạm ứng (linkLabel quick pick) |
| V_HIS_ACCOUNT_BOOK | View | Sổ thu chi |
| HIS_PAY_FORM | Table | Hình thức thanh toán |
| HIS_BANK | Table | Ngân hàng |
| V_HIS_CASHIER_ROOM | View | Phòng thu ngân |

### Quan hệ chính
- `HIS_TRANSACTION.TRANSACTION_REASON_ID` → `HIS_TRANSACTION_REASON.ID` (n-1).
- `HIS_TRANSACTION.TREATMENT_ID` → `HIS_TREATMENT.ID`.

## 4. UI Layout

### Sơ đồ giao diện
```
+--------------------------------------------------------------------+
| [Mã treatment] [Mã yêu cầu] [Tìm (Ctrl F)]                         |
+--------------------------------------------------------------------+
| Mã BN: ... | Tên BN: ... | Ngày sinh: ... | Giới tính: ...         |
| Địa chỉ: .........................................................|
+--------------------------------------------------------------------+
| Số tiền:   [ . . . ]   | T/gian giao dịch: [ . . . ] | Mã GD: [   ]|
| Sổ thu chi:[ . . . ]   | Số chứng từ:      [ . . . ] | Lý do: [▼] |  ← MỚI
| Hình thức: [ . . . ]   | Số tiền CK:       [ . . . ] | Ngân hàng: |
+--------------------------------------------------------------------+
| Lý do tạm ứng (memo): ............................................|
| [Quick pick: KHÁM; ĐT; KHÁC...]                                    |
+--------------------------------------------------------------------+
| [POS] [⚙] [☐ Xem trước in] [☐ Tự đóng] | [QR][Lưu ký][Lưu in][Lưu]|
+--------------------------------------------------------------------+
```

### Controls chính
| Control | Loại | Mục đích |
|---------|------|----------|
| cboAccountBook | LookUpEdit | Chọn sổ thu chi |
| cboPayForm | LookUpEdit | Hình thức thanh toán |
| cboBank | GridLookUpEdit | Ngân hàng |
| **cboReason** | **LookUpEdit** | **(MỚI) Lý do giao dịch** |
| txtDescription | MemoEdit | Lý do tạm ứng (free text) |
| linkLabel1 | LinkLabel | Quick pick từ HIS_DEPOSIT_REASON |
| spinTongTuDen | SpinEdit | Số chứng từ |
| dtTransactionTime | DateEdit | Thời gian giao dịch |
| txtTotalAmount | SpinEdit | Số tiền |
| **panelPayformGrid** | **PanelControl + HIS.UC.TransactionPayformGrid** | **(MỚI) Lưới nhập nhiều hình thức thanh toán — chỉ khi MULTI_PAYFORM bật** |

### Cấu hình ảnh hưởng

| KEY | Hành vi khi BẬT (= 1) | Hành vi khi TẮT (≠ 1) |
|-----|------------------------|------------------------|
| `MOS.HIS_TRANSACTION.MULTI_PAYFORM` | UC lưới hình thức thanh toán (`HIS.UC.TransactionPayformGrid`) hiển thị trực tiếp trong form; ẩn các control Hình thức, Số tiền, Số tiền CK, Số tiền QT, Ngân hàng | UC ẩn hoàn toàn; giao diện giữ nguyên 1 ô chọn hình thức như cũ |

**Thay đổi giao diện khi BẬT:**
- `panelPayformGrid` + `lciPayformGrid` (full-width, host `HIS.UC.TransactionPayformGrid`) khai báo sẵn trong Designer, đặt là 1 hàng ngay dưới vùng hình thức và trên ô "Lý do tạm ứng"; mặc định `LayoutVisibility.Never`, runtime bật `Always` khi config = 1.
- Ẩn (`LayoutVisibility.Never`): `layoutControlItem5` (Hình thức), `layoutTotalAmount` (Số tiền), `lciTranferAmount` (Số tiền CK), `lciSwipeAmount` (Số tiền QT), `lciBank` (Ngân hàng).
- Gỡ validation rule trên `txtTotalAmount`, `spinTransferAmount`, `spinSwipeAmount` (đã ẩn) để không chặn lưu.
- Mở rộng chiều cao form (+180px) để hiển thị lưới.

**Thay đổi xử lý khi BẬT:**
- Form load (`timerInitForm_Tick`) → `InitPayformGrid()` khởi tạo UC với số tiền phải thu (`RequiredAmount = txtTotalAmount.Value`).
- Bấm "Mới" (`ResetDefaultValueControl`) → `RefreshPayformGridRequiredAmount()` cập nhật lại số tiền phải thu.
- Bấm Lưu → `SaveDepositMultiPayForm()`: `ValidateData(uc)` → `GetData(uc)` → map `PayformRowADO` sang `HisTransactionDepositSDO.PayformDetails`, `Transaction.AMOUNT = GetTotalAmount(uc)`, **không gọi WCF thẻ/POS** → `CreateDeposit`.

## 5. API Endpoints

| Action | URI | Consumer | Filter / DTO |
|--------|-----|----------|--------------|
| Lưu giao dịch tạm ứng | `UriStores.HIS_TRANSACTION_CREATE_DEPOSIT` (`api/HisTransaction/CreateDeposit`) | MosConsumer | `HisTransactionDepositSDO` (Transaction.TRANSACTION_REASON_ID **MỚI**) |
| Kiểm tra trước (Thẻ/POS) | `UriStores.HIS_TRANSACTION_CHECK_DEPOSIT` | MosConsumer | `HisTransactionDepositSDO` |
| Lấy danh mục lý do giao dịch | `api/HisTransactionReason/Get` **(MỚI)** | MosConsumer | `HisTransactionReasonFilter` |
| Lấy danh mục lý do tạm ứng (quick pick) | `api/HisDepositReason/Get` | MosConsumer | `HisDepositReasonFilter` |
| Lấy sổ thu chi | `api/HisAccountBook/GetView` | MosConsumer | `HisAccountBookViewFilter` |
| Lấy hình thức TT | `api/HisPayForm/Get` | MosConsumer | — |
| Lấy ngân hàng | `api/HisBank/Get` | MosConsumer | — |

## 6. Dependencies

### Inter-Plugin / WCF
| Đích | Khi nào mở | Args |
|------|-----------|------|
| WCF Card (TIG) | Khi PayForm = THE | `WcfDepositDCO` |
| WCF POS (NetTcpBinding_IService1) | Khi PayForm = QUET_THE/TMQT/9 và bật `chkConnectionPOS` | `WcfRequest` |
| `frmDepositReason` (sub-form trong plugin) | Khi click link "Khác..." | currentModule, DelegateSelectData |

### Library
| Library | Mục đích |
|---------|----------|
| `HIS.Desktop.Library.CacheClient.ControlStateWorker` | Lưu trạng thái checkbox (POS, AutoClose...) |

### User Control
| UC | Khi nào dùng | API tích hợp |
|----|--------------|--------------|
| `HIS.UC.TransactionPayformGrid` | Khi `MOS.HIS_TRANSACTION.MULTI_PAYFORM` = 1 | `Run(TransactionPayformGridInitADO)`, `SetRequiredAmount`, `ValidateData`, `GetData` → `List<PayformRowADO>`, `GetTotalAmount` |

## 7. Print

| Loại in | PrintTypeCode | Nơi gọi | Ghi chú |
|---------|--------------|---------|---------|
| Phiếu thu tạm ứng | (theo cấu hình `dicPrinter`) | `btnPrint_Click` / `btnSavePrint_Click` | In ngay hoặc preview theo `ConfigApplications.CheDoInChoCacChucNangTrongPhanMem` |
| QR thanh toán | — | `CreateQR()` sau save thành công | Khi PayForm = QR |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 04/06/2026 | huannh | **Thêm nhiều hình thức thanh toán (MULTI_PAYFORM):** thêm config `MOS.HIS_TRANSACTION.MULTI_PAYFORM`. Khi bật: tích hợp UC `HIS.UC.TransactionPayformGrid` vào form, ẩn các control hình thức đơn lẻ (Hình thức/Số tiền/Số tiền CK/Số tiền QT/Ngân hàng) + gỡ validation rule, mở rộng form. Khi Lưu: đọc danh sách hình thức từ UC → gửi `HisTransactionDepositSDO.PayformDetails` (map từ `PayformRowADO` sang `PayformDetailSDO`) khi gọi `CreateDeposit`, bỏ qua luồng WCF thẻ/POS. Thêm `HisConfigCFG.IsMultiPayForm`, `InitPayformGrid()`, `BuildPayformInitADO()`, `RefreshPayformGridRequiredAmount()`, `SaveDepositMultiPayForm()` và message `VuiLongNhapHinhThucThanhToan` (vi/en/my). |
| 19/05/2026 | huannh | **Thêm Lý do giao dịch (cboReason):** load từ `api/HisTransactionReason/Get`; mặc định "Khám" / "Điều trị" theo `TDL_TREATMENT_TYPE_ID`; gửi `TRANSACTION_REASON_ID` trong `HisTransactionDepositSDO.Transaction` khi gọi `CreateDeposit`. Áp pattern combo chuẩn HIS (LookUpEdit, IS_ACTIVE=1, OrderBy CODE, ShowHeader=false, ImmediatePopup). Bổ sung resource keys `frmTransactionDeposit.lciReason.Text` cho 3 ngôn ngữ vi/en/my. |

## 9. Test Cases

### Lý do giao dịch (mới)
- [ ] Mở form từ điều trị có `TDL_TREATMENT_TYPE_ID = ID__KHAM` → cboReason mặc định = "Khám".
- [ ] Mở form từ điều trị `ID__DTNGOAITRU` / `ID__DTNOITRU` / `ID__DTBANNGAY` → mặc định = "Điều trị".
- [ ] Mở form từ `depositReq` (chưa có treatment) → mặc định = "Khám".
- [ ] Bấm "Mới" → cboReason reset về mặc định theo treatment hiện tại.
- [ ] Đổi sang lý do khác → Lưu → check DB `HIS_TRANSACTION.TRANSACTION_REASON_ID` đúng giá trị đã chọn.
- [ ] Lưu với cboReason để trống → `TRANSACTION_REASON_ID` = null (nếu BE cho phép) hoặc validate báo lỗi.
- [ ] Đổi ngôn ngữ (VI/EN/MY) → caption "Lý do:" hiển thị đúng.

### Nhiều hình thức thanh toán (MULTI_PAYFORM)
- [ ] Config = 1 → mở form: hiển thị lưới UC, ẩn ô Hình thức/Số tiền/Số tiền CK/Số tiền QT/Ngân hàng, form cao hơn.
- [ ] Config ≠ 1 → giao diện giữ nguyên 1 ô chọn hình thức, không có lưới.
- [ ] Config = 1, nhập 2 dòng (tiền mặt + chuyển khoản) tổng = số phải thu → Lưu → `CreateDeposit` nhận đủ `PayformDetails`, `Transaction.AMOUNT` = tổng.
- [ ] Config = 1, lưới trống → Lưu → cảnh báo "Vui lòng nhập ít nhất một hình thức thanh toán".
- [ ] Config = 1, tổng tiền < `MinimumDepositAmount` → cảnh báo, không lưu.
- [ ] Config = 1, bấm "Mới" → lưới reset, số tiền phải thu cập nhật đúng.
- [ ] Config = 1 → không gọi WCF thẻ/POS dù có dòng hình thức thẻ.

### Save chính (regression)
- [ ] Lưu giao dịch tiền mặt → thành công, mở phiếu in (nếu chọn).
- [ ] Lưu qua thẻ (THE) → gọi WCF Card → BE → in.
- [ ] Lưu QR → tạo giao dịch → sinh QR.
- [ ] Số tiền < `MinimumDepositAmount` → cảnh báo, không lưu.
