# Tạm ứng theo yêu cầu — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.DepositRequest |
| Loại | UserControl (mở trong tab phòng thu ngân) |
| Mục đích | Phòng thu ngân thu tiền cho các **yêu cầu tạm ứng** do khoa lập sẵn (`HIS_DEPOSIT_REQ`). Chọn yêu cầu trên lưới → chọn sổ thu chi + hình thức thanh toán → Lưu để tạo `HIS_TRANSACTION` loại TU và gắn `DEPOSIT_ID` vào yêu cầu. |
| Người tạo | — |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở màn từ menu Tạm ứng theo yêu cầu → `UCDepositRequest(module, List<V_HIS_DEPOSIT_REQ>)`.
2. `UCDepositRequest_Load`: mặc định bộ lọc trạng thái = *Chưa tạm ứng* → `FillDataToGrid()` (API `HisDepositReq/GetView`, phân trang, lọc theo `BRANCH_ID`) → `ValidControls()` → `LoadCombo()` (sổ thu chi, hình thức thanh toán) → `HisConfigCFG.LoadConfig()` → set thời gian giao dịch → `SetDefaultCreateQR()` → đổ dòng đang chọn lên form.
3. Người dùng chọn dòng trên lưới → `LoadDataToForm(data)` đổ Số tiền / Mô tả / Mã yêu cầu; dòng **chưa** thu tiền (`DEPOSIT_ID == null`) hiện đỏ và bật nút Lưu, dòng **đã** thu hiện xanh và chỉ cho In.
4. Nhấn `Lưu (Ctrl S)` / `Lưu In (Ctrl I)` → `SaveProcess` → validate → build `HisTransactionDepositSDO` → API `HisTransaction/CreateDeposit`.
5. Thành công → refresh lưới, cập nhật số hóa đơn của sổ, in phiếu MPS000091 nếu chọn Lưu In.
6. Nút `Tạo QR` mở plugin `HIS.Desktop.Plugins.CreateTransReqQR` (ưu tiên cấu hình QR theo phòng `V_HIS_ROOM.QR_CONFIG_JSON`, sau đó tới các key `HIS.Desktop.Plugins.PaymentQrCode*`).

### Điều kiện nghiệp vụ
- Chỉ thu được yêu cầu **chưa** có giao dịch tạm ứng; yêu cầu đã có `DEPOSIT_ID` thì backend chặn (`HisDeposit_TonTaiDuLieu`).
- Backend **bắt buộc** `HIS_TRANSACTION.AMOUNT == HIS_DEPOSIT_REQ.AMOUNT` (`HisTransactionDepositCreate.cs:104`) ⇒ muốn thu số khác thì phải cập nhật lại yêu cầu trước (xem mục 8, việc 54923).
- `TRANSACTION_TYPE_ID` do backend gán cố định = `HIS_TRANSACTION_TYPE.ID__TU`.
- Người dùng phải đang làm việc tại phòng thu ngân (`V_HIS_CASHIER_ROOM` theo `RoomId`).
- Sổ thu chi có `IS_NOT_GEN_TRANSACTION_ORDER = 1` thì cho nhập tay số hóa đơn (`SpNumOrder`).
- Ô "Số tiền CK" chỉ hiện với hình thức `HIS_PAY_FORM.ID__TMCK` (mã 03 — Tiền mặt/Chuyển khoản), bắt buộc nhập > 0 và ≤ số tiền tạm ứng.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_DEPOSIT_REQ | View | Dữ liệu lưới yêu cầu tạm ứng + bản ghi đang chọn |
| HIS_DEPOSIT_REQ | Table | Bản ghi gửi lên `HisDepositReq/Update` khi sửa số tiền |
| HIS_TRANSACTION | Table | Giao dịch tạm ứng được tạo (`AMOUNT`, `PAY_FORM_ID`, `TRANSFER_AMOUNT`, `NUM_ORDER`) |
| V_HIS_TRANSACTION | View | Kết quả trả về sau khi tạo giao dịch |
| HIS_PAY_FORM | Table | Hình thức thanh toán (combo) |
| V_HIS_ACCOUNT_BOOK | View | Sổ thu chi (combo, kiểm tra số hóa đơn) |
| V_HIS_CASHIER_ROOM | View | Phòng thu của người dùng |
| V_HIS_ROOM | View | `QR_CONFIG_JSON` — cấu hình QR theo phòng |
| HIS_CONFIG | Table | Key config của màn (xem mục 5) |
| HisTransactionDepositSDO | SDO | DTO của `HisTransaction/CreateDeposit` |

## 4. UI Layout

Khối thông tin bên phải (`layoutControl7` / `layoutControlGroup6`), theo hàng từ trên xuống:

| Hàng | Nội dung |
|------|----------|
| Thời gian | `dtTransactionTime` — chỉ cho sửa khi bật key `...IsEditTransactionTime` |
| Sổ thu chi | `txtAccountBookCode` + `cboAccountBook` |
| Hình thức | `txtPayFormCode` + `cboPayForm` + Tổng/Từ/Đến (`txtTotalFromNumberOder`) |
| Số hóa đơn | `SpNumOrder` |
| Mô tả | `txtDescription` |
| Số tiền / Mã yêu cầu | `txtAmount` (mask `n0`) + `txtEditReqCode` |
| **Số tiền CK** | `spinTransferAmount` — `lciTransferAmount`, mặc định `Visibility = Never` |
| Nút | Tạo QR · Lưu In (Ctrl I) · Lưu (Ctrl S) · In (Ctrl P) |

Bên trái: ô tìm theo mã yêu cầu (F2) / từ khóa, combo trạng thái (Tất cả / Chưa tạm ứng / Đã tạm ứng), lưới `HIS.UC.ListDepositRequest`, phân trang.

## 5. Config (HIS_CONFIG)

| Key | Ý nghĩa |
|-----|---------|
| `HIS.Desktop.Plugins.TransactionBill_Depo_Repa.IsEditTransactionTime` | `1` = cho sửa ô Thời gian giao dịch |
| `HIS.Desktop.Plugins.DepositRequest.IsEditAmount` | `1` = cho sửa ô "Số tiền" (chỉ với yêu cầu chưa thu tiền). Khác `1` / không khai báo = ô chỉ đọc như cũ |
| `HIS.Desktop.Plugins.PaymentQrCode*` | Có ít nhất 1 key có VALUE ⇒ hiện nút Tạo QR |

## 6. API Endpoints

| API | Vai trò |
|-----|---------|
| `GET api/HisDepositReq/GetView` | Nạp lưới yêu cầu tạm ứng (phân trang) |
| `POST api/HisDepositReq/Update` | Cập nhật `AMOUNT` khi thu ngân sửa số tiền (việc 54923) |
| `POST api/HisTransaction/CreateDeposit` | Tạo giao dịch tạm ứng |
| `GET api/HisAccountBook/GetView` | Nạp lại sổ thu chi để lấy số hóa đơn hiện tại |
| `GET api/HisTreatmentBedRoom/GetView` | Lấy giường hiện tại để in lên phiếu |

## 7. Print

| Mã in | Biểu |
|-------|------|
| MPS000091 | Phiếu yêu cầu tạm ứng (`PRINT_TYPE_CODE__BIEUMAU__YEU_CAU_TAM_UNG__MPS000091`) |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 18/08/2026 | nampp | Việc 54923 (BV Nguyễn Tri Phương): **(1) Số tiền chuyển khoản** — thêm `spinTransferAmount` + `lciTransferAmount` ("Số tiền CK:", mask `n0`, hàng riêng dưới hàng Số tiền, mặc định ẩn). `CheckPayFormTienMatChuyenKhoan` (file mới `Load\frmDepositRequest_TransferAmount.cs`, bám theo `DepositService`) hiện ô + đặt rule bắt buộc khi hình thức = `HIS_PAY_FORM.ID__TMCK`, ẩn + bỏ rule với hình thức khác, reset `EditValue = 0` mỗi lần đổi hình thức / đổi dòng; gắn `cboPayForm.EditValueChanged`. `UpdateDataFormTransactionDepositToDTO` gán `Transaction.TRANSFER_AMOUNT` (mặc định `null`); `CheckValidForSave` chặn CK > số tiền tạm ứng. Thêm `Validtion\SpinTranferAmountValidationRule.cs` (copy từ `DepositService`). **(2) Sửa số tiền tạm ứng** — `txtAmount` đổi thành ô nhập tiền (mask `n0`, `UseMaskAsDisplayFormat`), `ReadOnly` set runtime bằng `ApplyEditAmountState`: chỉ mở khi key `HIS.Desktop.Plugins.DepositRequest.IsEditAmount = 1` **và** `DEPOSIT_ID == null`. `SaveProcess` gọi `ProcessUpdateDepositReqAmount()` **trước** khi tạo giao dịch: số tiền thay đổi ⇒ POST `HisDepositReq/Update` (dựng đủ entity qua `NewDepositReqForUpdate`, chỉ đổi `AMOUNT`), lỗi ⇒ dừng không tạo giao dịch; nhờ vậy check `depositReq.AMOUNT != Transaction.AMOUNT` của backend luôn thoả ⇒ **không sửa Backend**. Không sửa: sổ thu chi/số hóa đơn, thời gian giao dịch, tạo QR, in MPS000091. Ghi chú: đã bỏ entry `Properties\licenses.licx` stale khỏi csproj (file không tồn tại → lỗi MSB6003 lc.exe). |

## 9. Test Cases

### Số tiền chuyển khoản
- [ ] Chọn hình thức "Tiền mặt/Chuyển khoản" → hiện ô "Số tiền CK" nhãn đỏ, mặc định 0
- [ ] Để trống / để 0 → Lưu: cảnh báo bắt buộc, con trỏ nhảy vào ô CK, không tạo giao dịch
- [ ] CK > số tiền tạm ứng → Lưu: cảnh báo "Số tiền chuyển khoản ... lớn hơn số tiền tạm ứng ..."
- [ ] CK = số tiền tạm ứng → Lưu: `HIS_TRANSACTION.TRANSFER_AMOUNT` đúng số vừa nhập
- [ ] CK < số tiền tạm ứng → Lưu: `TRANSFER_AMOUNT` = phần chuyển khoản, phần còn lại là tiền mặt
- [ ] Đổi sang hình thức khác → ô CK ẩn, bỏ ràng buộc, `TRANSFER_AMOUNT = null`

### Sửa số tiền tạm ứng
- [ ] Chưa khai báo key → ô "Số tiền" chỉ đọc, thu tiền bình thường (**hồi quy**)
- [ ] Bật key + yêu cầu chưa thu → sửa 3.000.000 → 2.000.000 → Lưu: `HIS_DEPOSIT_REQ.AMOUNT` = giao dịch = 2.000.000, lưới cập nhật
- [ ] Bật key + để 0 / để trống → Lưu: cảnh báo "Số tiền tạm ứng phải lớn hơn 0", không gọi API
- [ ] Bật key + không sửa gì → Lưu: **không** gọi `HisDepositReq/Update`
- [ ] Bật key + yêu cầu đã thu (dòng xanh) → ô "Số tiền" chỉ đọc, nút Lưu mờ
- [ ] Sửa số tiền rồi bấm sang dòng khác → ô nạp lại theo dòng mới, không lưu ngầm
- [ ] Lưu In sau khi sửa → phiếu MPS000091 in đúng số tiền mới
- [ ] Tắt key (VALUE = 0) + restart → ô "Số tiền" trở lại chỉ đọc, ô CK vẫn chạy

### Hồi quy chung
- [ ] Tìm theo mã yêu cầu / từ khóa, đổi bộ lọc trạng thái, phân trang
- [ ] Sổ thu chi có nhập tay số hóa đơn → số hóa đơn vẫn đúng sau khi lưu
- [ ] Nút Tạo QR hoạt động như cũ
- [ ] Màn "Tạm ứng" (`DepositService`) và "Yêu cầu tạm ứng" của khoa (`RequestDeposit`) không bị ảnh hưởng
