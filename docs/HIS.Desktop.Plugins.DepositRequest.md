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
3. Người dùng chọn dòng trên lưới → `LoadDataToForm(data)` đổ Số tiền / Mô tả / Mã yêu cầu; dòng **chưa** thu tiền (`DEPOSIT_ID == null`) hiện đỏ và bật nút Lưu; dòng **đã** thu hiện xanh, chỉ cho In; ô "Số tiền" luôn chỉ đọc.
4. Nhấn `Lưu (Ctrl S)` / `Lưu In (Ctrl I)` → `SaveProcess` → validate (gồm chặn CK/QT vượt trần) → build `HisTransactionDepositSDO` → API `HisTransaction/CreateDeposit`.
5. Thành công → refresh lưới, cập nhật số hóa đơn của sổ, in phiếu MPS000091 nếu chọn Lưu In.
6. Nút `Tạo QR` mở plugin `HIS.Desktop.Plugins.CreateTransReqQR` (ưu tiên cấu hình QR theo phòng `V_HIS_ROOM.QR_CONFIG_JSON`, sau đó tới các key `HIS.Desktop.Plugins.PaymentQrCode*`).

### Điều kiện nghiệp vụ
- Chỉ thu được yêu cầu **chưa** có giao dịch tạm ứng; yêu cầu đã có `DEPOSIT_ID` thì backend chặn (`HisDeposit_TonTaiDuLieu`).
- Backend **bắt buộc** `HIS_TRANSACTION.AMOUNT == HIS_DEPOSIT_REQ.AMOUNT` (`HisTransactionDepositCreate.cs:104`) — không bị đụng: giao dịch vẫn lấy `AMOUNT` từ bản ghi yêu cầu.
- Ô "Số tiền" **luôn chỉ đọc** (chốt cuối 21/08/2026 — yêu cầu cho sửa của bản nháp đã bỏ).
- `TRANSACTION_TYPE_ID` do backend gán cố định = `HIS_TRANSACTION_TYPE.ID__TU`.
- Người dùng phải đang làm việc tại phòng thu ngân (`V_HIS_CASHIER_ROOM` theo `RoomId`).
- Sổ thu chi có `IS_NOT_GEN_TRANSACTION_ORDER = 1` thì cho nhập tay số hóa đơn (`SpNumOrder`).
- Ô "Số tiền CK" **luôn hiện, mặc định disable** — flow bê từ màn Xuất hóa đơn bán thuốc (`frmMedicineSaleBill.cboPayFrom_EditValueChanged`): `PAY_FORM_CODE = "03"` → enable, nhãn "Số tiền CK:" đỏ; `"06"` → enable, nhãn đổi "Số tiền QT:" (tooltip "Số tiền quẹt thẻ"); hình thức khác → disable, nhãn đen. **Không bắt buộc nhập.** Lưu: 03 → `TRANSFER_AMOUNT`, 06 → `SWIPE_AMOUNT`.
- Khi Lưu nếu số nhập > số tiền tạm ứng: message "Số tiền chuyển khoản/quẹt thẻ **lớn hơn số tiền thanh toán của bệnh nhân**" + icon cảnh báo `dxErrorProvider.SetError(..., ErrorType.Warning)` tại ô, không tạo giao dịch; gõ lại số thì icon tự mất.
- Label **"Cần thu"** (`lblCanThu`, cùng hàng ô CK) = max(0, Số tiền − Số tiền CK/QT), cập nhật realtime khi gõ ô CK/QT, sửa ô Số tiền, đổi hình thức, đổi dòng (`UpdateCanThuLabel`).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_DEPOSIT_REQ | View | Dữ liệu lưới yêu cầu tạm ứng + bản ghi đang chọn |
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
| Số tiền / Mã yêu cầu | `txtAmount` (mask `n0`, **chỉ đọc**) + `txtEditReqCode` |
| **Số tiền CK / Cần thu** | `spinTransferAmount` (`lciTransferAmount`) + `lblCanThu` (`lciCanThu`) cùng hàng — ô CK luôn hiện, mặc định disable, enable khi `PAY_FORM_CODE` 03/06 (flow MedicineSaleBill); icon cảnh báo qua `dxErrorProvider` |
| Nút | Tạo QR · Lưu In (Ctrl I) · Lưu (Ctrl S) · In (Ctrl P) |

Bên trái: ô tìm theo mã yêu cầu (F2) / từ khóa, combo trạng thái (Tất cả / Chưa tạm ứng / Đã tạm ứng), lưới `HIS.UC.ListDepositRequest`, phân trang.

## 5. Config (HIS_CONFIG)

| Key | Ý nghĩa |
|-----|---------|
| `HIS.Desktop.Plugins.TransactionBill_Depo_Repa.IsEditTransactionTime` | `1` = cho sửa ô Thời gian giao dịch |
| `HIS.Desktop.Plugins.PaymentQrCode*` | Có ít nhất 1 key có VALUE ⇒ hiện nút Tạo QR |

> Việc 54923 từng thiết kế key `HIS.Desktop.Plugins.DepositRequest.IsEditAmount` và tính năng sửa số tiền — **cả hai đã bỏ khi chốt spec 21/08/2026**. Môi trường nào lỡ INSERT key này thì key nằm im không ảnh hưởng, có thể DELETE.

## 6. API Endpoints

| API | Vai trò |
|-----|---------|
| `GET api/HisDepositReq/GetView` | Nạp lưới yêu cầu tạm ứng (phân trang) |
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
| 18/08/2026 | nampp | Việc 54923 (BV Nguyễn Tri Phương): **(1) Số tiền chuyển khoản** — thêm `spinTransferAmount` + `lciTransferAmount` ("Số tiền CK:", mask `n0`, hàng riêng dưới hàng Số tiền, mặc định ẩn). `CheckPayFormTienMatChuyenKhoan` (file mới `Load\frmDepositRequest_TransferAmount.cs`, bám theo `frmTransactionDeposit`/`DepositService`) hiện ô + đặt rule bắt buộc khi hình thức = `HIS_PAY_FORM.ID__TMCK`, ẩn + bỏ rule với hình thức khác, reset `EditValue = 0` mỗi lần đổi hình thức / đổi dòng; gắn `cboPayForm.EditValueChanged`. `UpdateDataFormTransactionDepositToDTO` gán `Transaction.TRANSFER_AMOUNT` (mặc định `null`); `CheckValidForSave` chặn CK > số tiền tạm ứng. Thêm `Validtion\SpinTranferAmountValidationRule.cs` (copy từ `DepositService`). **(2) Sửa số tiền tạm ứng** — `txtAmount` đổi thành ô nhập tiền (mask `n0`, `UseMaskAsDisplayFormat`), `ReadOnly` set runtime bằng `ApplyEditAmountState`. `SaveProcess` gọi `ProcessUpdateDepositReqAmount()` **trước** khi tạo giao dịch: số tiền thay đổi ⇒ POST `HisDepositReq/Update` (dựng đủ entity qua `NewDepositReqForUpdate`, chỉ đổi `AMOUNT`), lỗi ⇒ dừng không tạo giao dịch; nhờ vậy check `depositReq.AMOUNT != Transaction.AMOUNT` của backend luôn thoả ⇒ **không sửa Backend**. Không sửa: sổ thu chi/số hóa đơn, thời gian giao dịch, tạo QR, in MPS000091. Ghi chú: đã bỏ entry `Properties\licenses.licx` stale khỏi csproj (file không tồn tại → lỗi MSB6003 lc.exe). |
| 21/08/2026 | nampp | Việc 54923 (tiếp): **bỏ key config `HIS.Desktop.Plugins.DepositRequest.IsEditAmount`** theo chốt — ô "Số tiền" mở mặc định cho yêu cầu chưa thu tiền (`DEPOSIT_ID == null`), yêu cầu đã thu vẫn chỉ đọc, giống triết lý màn "Tạm ứng" ở thu ngân (đúng điều kiện là hiện/cho nhập, không cần cấu hình). `ApplyEditAmountState` chỉ còn xét `DEPOSIT_ID`; `ProcessUpdateDepositReqAmount` bỏ guard key; `Config\HisConfigCFG.cs` trả về nguyên bản (chỉ còn `IsEditTransactionTime`). Script SQL thêm key đã xoá khỏi PTTK + bộ giao nộp; môi trường nào lỡ INSERT key thì key nằm im, có thể DELETE. |
| 21/08/2026 | nampp | Việc 54923 (tiếp 2 — theo yêu cầu test): ô "Số tiền CK" đổi từ ẩn/hiện sang **enable/disable giống 100% màn "Tạm ứng"** (`frmTransactionDeposit.CheckPayFormTienMatChuyenKhoan`): ô **luôn hiện** (Designer bỏ `Visibility = Never`); TMCK (03) → enable + bắt buộc, nhãn "Số tiền CK:" đỏ; TMQT → enable + bắt buộc, nhãn đổi "Số tiền quẹt thẻ:"; TMCKQT (09) → enable + bắt buộc (màn chỉ có 1 ô nên hình thức 09 chỉ nhập phần CK); còn lại (Tiền mặt, Quẹt thẻ…) → **disable, nhãn đen**. Lưu mirror theo: TMCK/TMCKQT → `TRANSFER_AMOUNT`, TMQT → `SWIPE_AMOUNT`; message chặn vượt trần đổi thành y màn "Tạm ứng" ("… lớn hơn số tiền tạm ứng của bệnh nhân …", `NumberToStringRoundAuto`). |
| 21/08/2026 | nampp | Việc 54923 (chốt spec — flow bê từ **màn Xuất hóa đơn bán thuốc** `frmMedicineSaleBill.cs:3474-3597`): (1) nhận diện hình thức bằng **`PAY_FORM_CODE == "03"/"06"`** (bỏ hằng ID, bỏ nhánh TMQT-tên-cũ và 09 — 09 rơi về disable); (2) 06 → nhãn **"Số tiền QT:"** (tooltip "Số tiền quẹt thẻ"); (3) **bỏ bắt buộc nhập** (bỏ rule `SpinTranferAmountValidationRule` — không nhập = thu tiền mặt hết, file rule giữ lại không dùng); (4) vượt trần khi Lưu → message spec "Số tiền chuyển khoản/quẹt thẻ **lớn hơn số tiền thanh toán của bệnh nhân**" + **icon cảnh báo** `dxErrorProvider.SetError(spin, msg, ErrorType.Warning)` (thêm component `DXErrorProvider` vào Designer) + `MessageBox.Show`; gõ lại/đổi hình thức/đổi dòng → icon tự xoá; (5) thêm **"Cần thu"** = max(0, Số tiền − CK/QT): `lblCanThu` + `lciCanThu` cùng hàng ô CK (ô CK thu còn 124px như `txtAmount`), `UpdateCanThuLabel()` gọi từ `spinTransferAmount_EditValueChanged` + `txtAmount_EditValueChanged` (event mới) + đổi hình thức + đổi dòng; (6) lưu 03 → `TRANSFER_AMOUNT`, 06 → `SWIPE_AMOUNT` theo `PAY_FORM_CODE`; reset ô = `null` khi đổi dòng. Phần sửa số tiền tạm ứng giữ nguyên. |
| 21/08/2026 | nampp | Việc 54923 (chốt cuối): **gỡ tính năng cho sửa số tiền tạm ứng** — spec chốt chỉ còn ô CK/QT + "Cần thu", ô "Số tiền" trở lại **chỉ đọc như bản gốc** (Designer vốn vẫn giữ `ReadOnly = true`, chỉ gỡ phần mở runtime). Đã xoá: `ApplyEditAmountState` + 3 chỗ gọi, `ProcessUpdateDepositReqAmount`, `NewDepositReqForUpdate`, hằng `HIS_DEPOSIT_REQ_UPDATE`; **giữ** `TryGetEditAmount` (đọc số tiền hiển thị cho "Cần thu") và mask `n0` + `EditValueChanged` của `txtAmount`. Không còn gọi `api/HisDepositReq/Update`; giao dịch lấy `AMOUNT` từ bản ghi yêu cầu như nguyên bản. |

## 9. Test Cases

### Số tiền chuyển khoản / quẹt thẻ + Cần thu
- [ ] Hình thức "Tiền mặt" (mặc định) → ô "Số tiền CK" **hiện nhưng mờ (disable), nhãn đen**; "Cần thu" = Số tiền
- [ ] Chọn hình thức 03 → ô **enable ngay khi đổi combo**, nhãn "Số tiền CK:" đỏ, tooltip "Số tiền chuyển khoản"
- [ ] Chọn hình thức 06 → nhãn đổi **"Số tiền QT:"**, tooltip "Số tiền quẹt thẻ"
- [ ] Gõ số vào ô CK/QT → "Cần thu" nhảy theo từng lần gõ (= Số tiền − số nhập, không âm)
- [ ] 03 + CK > Số tiền → Lưu: popup "Số tiền chuyển khoản lớn hơn số tiền thanh toán của bệnh nhân" + **icon cảnh báo tại ô**; sửa lại số → icon mất
- [ ] 06 + QT > Số tiền → Lưu: popup "Số tiền quẹt thẻ lớn hơn số tiền thanh toán của bệnh nhân" + icon
- [ ] 03 + CK hợp lệ → Lưu: `TRANSFER_AMOUNT` đúng số nhập, `SWIPE_AMOUNT` trống
- [ ] 06 + QT hợp lệ → Lưu: `SWIPE_AMOUNT` đúng số nhập, `TRANSFER_AMOUNT` trống
- [ ] 03/06 **không nhập gì** → Lưu bình thường (không bắt buộc), 2 cột trống
- [ ] Đổi sang hình thức khác → ô mờ đi (vẫn hiện), giá trị + icon xoá, "Cần thu" = Số tiền

### Ô Số tiền (hồi quy)
- [ ] Ô "Số tiền" **luôn chỉ đọc** với mọi dòng (chưa thu lẫn đã thu), hiển thị dấu phân nhóm nghìn
- [ ] Không có request `HisDepositReq/Update` nào phát sinh khi Lưu

### Hồi quy chung
- [ ] Tìm theo mã yêu cầu / từ khóa, đổi bộ lọc trạng thái, phân trang
- [ ] Sổ thu chi có nhập tay số hóa đơn → số hóa đơn vẫn đúng sau khi lưu
- [ ] Nút Tạo QR hoạt động như cũ
- [ ] Màn "Tạm ứng" (`TransactionDeposit`), "Tạm ứng theo dịch vụ" (`DepositService`) và "Yêu cầu tạm ứng" của khoa (`RequestDeposit`) không bị ảnh hưởng
