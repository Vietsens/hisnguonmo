# TCB-POS — Hướng dẫn triển khai: Thanh toán qua máy POS Techcombank

| Mục | Giá trị |
|-----|---------|
| Ngày lập | 2026-09-10 |
| Đơn vị | Bệnh viện Nguyễn Tri Phương — 468 Nguyễn Trãi, P. An Đông, TP.HCM |
| Phạm vi | Bổ sung thanh toán **quẹt thẻ** qua 10 máy Techcombank SmartPOS X990 |
| Repo deploy | `E:\IVT TEST\histest` — nhánh `Test` |
| Nền tảng | .NET Framework 4.5.0 |
| Tài liệu thiết kế | `TCB-POS-00-PHAN-TICH-THIET-KE.md` |

> **Không dùng QR.** Máy TCB có sẵn chức năng QR động nhưng hợp đồng chốt chỉ dùng quẹt thẻ. HIS chỉ phát lệnh `sale`.

---

## 1. Bản này có gì

| Hạng mục | Nội dung |
|----------|----------|
| Thêm hãng POS | `POS TECHCOMBANK` — kết nối ECR qua HTTPS |
| Hủy giao dịch | Bắt thu ngân hủy trên máy POS trước, nhập mã hủy vào HIS mới cho hủy |
| Tái cấu trúc | Tách logic 4 hãng POS (MB / SHB / BIDV / TCB) theo Strategy |
| Sửa lỗi | Timeout vô hạn khi máy POS treo; dữ liệu lẫn giữa các giao dịch; thiếu TID máy MB; hộp thoại lỗi làm treo dịch vụ |

---

## 2. Danh sách file triển khai

Tất cả nằm trong repo `E:\IVT TEST\histest` nhánh `Test`.

| # | File | Đích | Bắt buộc |
|---|------|------|----------|
| 1 | `WCF.exe` | `x64\Integrate\POS.WCFService\` | ✔ |
| 2 | `WCF.pdb` | `x64\Integrate\POS.WCFService\` | tùy chọn (debug) |
| 3 | `HIS.Desktop.Plugins.TransactionCancel.dll` | `x64\Plugins\Module\` | ✔ |
| 4 | `HIS.Desktop.Plugins.TransactionCancel.resources.dll` | `x64\vi\` | ✔ |
| 5 | `HIS.Desktop.Plugins.TransactionCancel.resources.dll` | `x64\en\` | ✔ |

### ⚠️ TUYỆT ĐỐI KHÔNG copy `WCF.exe.config`

File `WCF.exe.config` chứa cấu hình POS **riêng của từng máy trạm** (hãng POS, IP máy POS, timeout). Đè lên sẽ xóa sạch cấu hình quầy đó và thu ngân phải khai báo lại từ đầu.

### Vì sao phải có file 4 và 5

Các câu thông báo của màn hình hủy giao dịch nằm trong **satellite assembly** (`vi\` và `en\`), không nằm trong DLL chính. Deploy thiếu 2 file này thì thông báo hiện ra **rỗng** — thu ngân không biết vì sao không hủy được.

### Không cần deploy lại

`WCF.dll` và `WCF.Client.dll` **không đổi**, 5 plugin thanh toán còn lại (`TransactionBill`, `TransactionBillTwoInOne`, `TransactionDeposit`, `DepositService`, `ExpMestSaleCreate`) **không phải build lại** — hợp đồng dịch vụ `IService1` giữ nguyên.

---

## 3. Chuẩn bị trước khi triển khai

### 3.1 Hạ tầng mạng — phía bệnh viện

- [ ] 10 máy POS đã nối vào mạng LAN/WiFi của bệnh viện
- [ ] Mỗi máy POS được đặt **IP tĩnh** (hoặc DHCP reservation theo MAC). Máy POS đổi IP là quầy đó mất kết nối
- [ ] Máy trạm thu ngân **ping được** tới IP máy POS
- [ ] Lập bảng ánh xạ: **Quầy thu ngân ↔ Serial máy POS ↔ TID ↔ IP**

### 3.2 Danh sách 10 máy POS đã bàn giao

| # | Serial | TID | IP (điền khi lắp) | Quầy (điền khi lắp) |
|---|--------|-----|-------------------|---------------------|
| 1 | V9E0857895 | 60002668 | | |
| 2 | V9E0857896 | 60002669 | | |
| 3 | V9E0857897 | 60002670 | | |
| 4 | V9E0857898 | 60002671 | | |
| 5 | V9E0857899 | 60002672 | | |
| 6 | V9E0857900 | 60002673 | | |
| 7 | V9E0857901 | 60002674 | | |
| 8 | V9E0857902 | 60002675 | | |
| 9 | V9E0857903 | 60002676 | | |
| 10 | V9E0857904 | 60002677 | | |

Giữ bảng này để đối soát: TID trong giao dịch HIS phải khớp TID của máy đặt tại quầy đó.

### 3.3 Cấu hình hệ thống HIS

- [ ] Config `HIS.Desktop.TransactionBillSelect` = `1` (bật luồng POS trong màn hình hủy giao dịch)

---

## 4. Các bước triển khai

Làm trên **từng máy trạm thu ngân**.

1. **Tắt HIS** và tắt tiến trình POS đang chạy nền:
   ```
   taskkill /IM HIS.exe /F
   taskkill /IM WCF.exe /F
   ```
   `WCF.exe` chạy ngầm, không tắt thì không ghi đè được file.

2. **Sao lưu** (để quay lui nhanh):
   ```
   copy "...\Integrate\POS.WCFService\WCF.exe"        "%TEMP%\WCF.exe.bak"
   copy "...\Plugins\Module\HIS.Desktop.Plugins.TransactionCancel.dll" "%TEMP%\TC.dll.bak"
   ```

3. **Copy 5 file** theo bảng mục 2. Nhắc lại: **không copy `WCF.exe.config`**.

4. **Mở HIS**, vào màn hình thanh toán viện phí, bấm nút **cấu hình POS**.

5. Trên màn hình *Cấu hình kết nối máy POS*:

   | Trường | Giá trị |
   |--------|---------|
   | Chọn POS | `POS TECHCOMBANK` |
   | IP | IP máy POS đặt tại quầy này |
   | Port | để trống (mặc định 443) — chỉ điền nếu TCB yêu cầu port khác |
   | Port hủy thao tác | để trống — điền khi TCB cấp port cho lệnh `ocan` |
   | Vân tay chứng thư | để trống khi chạy thử; **bắt buộc điền trước khi lên PROD** |
   | merchInfo bổ sung | tùy chọn, JSON object. VD: `{"hospital_code":"BVNTP","counter":"QUAY01"}` |

6. Bấm **Kiểm tra kết nối** → phải báo thành công. Sau đó bấm **Lưu cấu hình**.

> Không có ô `x-api-key` / `x-api-id`: 10 máy của bệnh viện không bật xác thực nên đã bỏ khỏi phạm vi.

---

## 5. Kiểm tra sau triển khai

| # | Việc kiểm tra | Kỳ vọng |
|---|---------------|---------|
| 1 | Quẹt thẻ một giao dịch nhỏ (VD 10.000đ) | Máy POS in biên lai, HIS tạo phiếu thu |
| 2 | Đối chiếu `TERMINAL_ID` trong giao dịch với TID máy tại quầy | Khớp |
| 3 | Khách bấm Cancel trên máy POS | HIS **không** tạo phiếu, hiện thông báo lỗi |
| 4 | Rút dây mạng máy POS rồi quẹt | HIS báo lỗi sau đúng thời gian chờ (mặc định 30 giây), **không treo** |
| 5 | Hủy giao dịch vừa tạo | HIS hiện cảnh báo kèm số hóa đơn POS, bắt nhập mã hủy |
| 6 | Bỏ trống mã hủy rồi bấm đồng ý | HIS **không** hủy |
| 7 | Hủy trên máy POS, lấy mã hủy, nhập vào HIS | HIS hủy thành công |
| 8 | Kiểm tra lý do hủy trong DB | Có đoạn `[Hủy trên máy POS TECHCOMBANK - mã hủy: …]` |
| 9 | Hồi quy quầy đang dùng máy MB / BIDV | Hoạt động như cũ, không đổi thao tác |

### Câu SQL kiểm tra

```sql
-- Thong tin may POS trong giao dich vua tao
SELECT ID, TRANSACTION_CODE, POS_INVOICE, POS_PAN, POS_CARD_HOLDER, POS_RESULT_JSON
FROM   HIS_TRANSACTION
WHERE  ID = :transactionId;

-- Ma huy da luu vao ly do huy chua
SELECT ID, IS_CANCEL, CANCEL_REASON, CANCEL_LOGINNAME, CANCEL_TIME
FROM   HIS_TRANSACTION
WHERE  ID = :transactionId;
```

`POS_RESULT_JSON` chứa `TERMINAL_ID` (TID), `MERCHANT_CODE` (MID), `SERIAL_NUMBER` (serial máy), `REF_NO`, `APPV_CODE`.

---

## 6. Quay lui (rollback)

Chép lại file đã sao lưu ở bước 2, hoặc lấy bản trước từ repo deploy:

```
cd E:\IVT TEST\histest
git checkout HEAD~1 -- x64/Integrate/POS.WCFService/WCF.exe
git checkout HEAD~1 -- x64/Plugins/Module/HIS.Desktop.Plugins.TransactionCancel.dll
git checkout HEAD~1 -- x64/vi/HIS.Desktop.Plugins.TransactionCancel.resources.dll
git checkout HEAD~1 -- x64/en/HIS.Desktop.Plugins.TransactionCancel.resources.dll
```

Nhớ tắt `HIS.exe` và `WCF.exe` trước khi ghi đè. Cấu hình trong `WCF.exe.config` không bị ảnh hưởng vì bản mới không đụng tới file này.

---

## 7. Sự cố thường gặp

| Hiện tượng | Nguyên nhân | Xử lý |
|-----------|-------------|-------|
| "Kiểm tra lại cấu hình NetTcpBinding_IService1" | `WCF.exe` chưa chạy hoặc bị chặn | Kiểm tra tiến trình `WCF.exe`; HIS tự khởi động nó khi thanh toán |
| Không copy được `WCF.exe` | Tiến trình đang chạy | `taskkill /IM WCF.exe /F` rồi copy lại |
| Thông báo khi hủy hiện **rỗng** | Thiếu satellite `vi\` hoặc `en\` | Copy đủ file 4 và 5 ở mục 2 |
| Quẹt xong HIS báo lỗi nhưng máy POS đã trừ tiền | Mất phản hồi giữa chừng | Tra `Logs\LogSystem.txt` theo `billId`; đối soát Merchant Portal; hủy trên máy POS rồi hủy trong HIS theo mục 4 |
| Thu ngân chọn nhầm hãng POS | Cấu hình sai `namebank` | Mở lại màn hình cấu hình, chọn `POS TECHCOMBANK`, lưu |
| Máy POS treo ở màn hình chờ quẹt | HIS đã hết thời gian chờ | HIS tự gửi lệnh `ocan` giải phóng máy nếu đã cấu hình port hủy; chưa có thì bấm Cancel trên máy |
| Giao dịch không hủy được qua HIS | Đúng thiết kế — TCB không có API void | Hủy trên máy POS / Merchant Portal trước, rồi nhập mã hủy vào HIS |

---

## 8. Lưu ý vận hành

1. **Chỉ thao tác thanh toán từ màn hình HIS.** Máy POS vẫn còn nút QR trên màn hình của nó; nếu thu ngân bấm QR trực tiếp trên máy thì tiền vào tài khoản bệnh viện nhưng **HIS không hề biết** — không có phiếu thu, không có dữ liệu đối soát. Nên đề nghị TCB khóa chức năng QR trên 10 máy này.

2. **Hủy giao dịch phải làm trên máy POS trước.** HIS cố tình chặn để hai bên không lệch: không hủy được trên máy POS thì không có mã hủy để nhập, HIS cũng không cho hủy.

3. **Vân tay chứng thư** để trống thì phần mềm chấp nhận mọi chứng thư và ghi cảnh báo vào `LogSystem.txt`. Chấp nhận được khi chạy thử, nhưng **trước khi lên PROD phải xin TCB thumbprint và điền vào** để tránh bị giả mạo thiết bị.

4. **Thời gian chờ** mặc định 30 giây (`PortConfig.TimeOutSecond` trong `WCF.exe.config`). Khách thao tác chậm thì nâng lên 60–90 giây.

---

## 9. Việc còn chờ Techcombank

| Việc | Ảnh hưởng |
|------|-----------|
| Port riêng cho lệnh `ocan` | Chưa có thì không tự giải phóng được máy POS khi hết thời gian chờ |
| File chứng thư + thumbprint | Bắt buộc trước khi lên PROD |
| API void/refund theo `receiptNo` | Có thì HIS hủy tự động được, bỏ bước nhập mã hủy tay |
| API truy vấn giao dịch | Có thì đối soát tự động được, hiện phải tra Merchant Portal |

---

## Changelog

| Ngày | Nội dung | Người |
|------|----------|-------|
| 2026-09-10 | Khởi tạo — hướng dẫn triển khai bản POS Techcombank .NET 4.5.0 | khainq |
