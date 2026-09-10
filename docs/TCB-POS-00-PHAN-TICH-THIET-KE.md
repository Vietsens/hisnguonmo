# TCB-POS — Phân tích & Thiết kế: Thanh toán qua máy POS Techcombank

| Mục | Giá trị |
|-----|---------|
| Ngày lập | 2026-09-09 |
| Module nền | `common/HISUTIL/POS.WCFService` (WCF.exe + WCF.Client.dll) |
| Thiết bị | Techcombank SmartPOS X990 (10 máy đã bàn giao) |
| Tài liệu nguồn | `E:\Tài liệu\Tích hợp\Techcombank\POS\TECHSPEC POS OMG.pdf` (SmartPOS — Tích hợp PMBH theo phương thức TCP/IP v1.0) |
| Portal đối soát | https://merchant.techcombank.com |
| Phạm vi | Bổ sung ngân hàng **POS TECHCOMBANK** vào module POS.WCFService hiện có |

---

## 1. Hiện trạng module POS.WCFService

### 1.1 Kiến trúc tổng thể

```
HIS.Desktop (plugin viện phí / nhà thuốc)
   │  new WcfClient()                     [WCF.Client.dll]
   │  Sale(Base64(JSON(WcfRequest)))
   ▼
net.tcp://localhost:8022/WCF/Service1/    [netTcpBinding, SessionMode.Required]
   ▼
WCF.exe  (process riêng, WinForms ApplicationContext)
   ├── Service1        — host ServiceHost + logic từng hãng POS
   ├── ConnectService  — implement IService1, ủy quyền sang Service1
   ├── ConnectConfig   — đọc/ghi appSettings trong WCF.exe.config
   └── FormConnect     — UI cấu hình (chọn ngân hàng, IP, Port, Link)
   ▼
Thiết bị POS  (TCP socket / HTTP)
```

**Vị trí triển khai:** `HIS.Desktop\bin\Debug\Integrate\POS.WCFService\WCF.exe`
Plugin tự khởi động process này bằng `OpenAppPOS()` nếu chưa chạy.

### 1.2 Hợp đồng dịch vụ (`IService1`)

```csharp
[ServiceContract(SessionMode = SessionMode.Required)]
public interface IService1
{
    [OperationContract] string Sale(string data);   // data = Base64(UTF8(JSON(WcfRequest)))
    [OperationContract] string Void(string data);
    [OperationContract] void   cauhinh();           // mở FormConnect cấu hình
}
```

Chiều đi và chiều về đều là `Base64(UTF8(JSON(WcfRequest)))`.

### 1.3 Các hãng POS đang hỗ trợ

| Ngân hàng | `namebank` | Transport | Định dạng | Sale | Void |
|-----------|-----------|-----------|-----------|------|------|
| MB (PAX) | `POS MB` | TCP Socket → `Ipname_Pax:portname_Pax` | Khung STX/LEN/DATA/ETX/LRC, mã hóa **3DES-CBC** khóa hardcode `X7WVTM4TOEALACIUOOZACRZA`, payload `APP:;TXN_TYPE:1;AMOUNT:…;CURRENCY_CODE:704;PRINT_MSG:…;SEND:OK` | ✔ `TXN_TYPE:1` | ✔ `TXN_TYPE:3` |
| SHB | `POS SHB` | HTTP POST → `linkketnoi` (middleware) | JSON | ✔ | ✘ chưa cài đặt |
| BIDV | `POS BIDV` | HTTP GET → `http://{ipBIDV}/ecr?rqt={billId}\|{creator}&cmd=sale\|{AMOUNT}` | JSON `WcfRequestBIDV` | ✔ | ✔ `cmd=void\|{INVOICE}` |

### 1.4 Cấu hình (`WCF.exe.config` → appSettings)

| Key | Ý nghĩa |
|-----|---------|
| `namebank` | Hãng POS đang chọn |
| `Ipname_Pax`, `portname_Pax` | IP/Port máy MB (PAX) |
| `linkketnoi` | URL middleware SHB |
| `ipshb` | IP máy POS SHB |
| `ipBIDV` | IP máy POS BIDV |
| `PortConfig.TimeOutSecond` | Timeout (mặc định 30s) |

### 1.5 DTO trao đổi (`WcfRequest`)

Trường HIS **gửi đi**: `AMOUNT` (long, VNĐ), `billId` (GUID 20 ký tự), `creator` (login name), `INVOICE` (chỉ dùng khi Void), `PRINT_MSG`.

Trường HIS **đọc về**:

| Trường | Dùng để |
|--------|---------|
| `RESPONSE_CODE` | `"00"` ⇒ thành công. Điều kiện DUY NHẤT plugin dùng để quyết định lưu giao dịch |
| `ERROR` | Mã lỗi hiển thị cho thu ngân |
| `PAN` | → `HIS_TRANSACTION.POS_PAN` |
| `NAME` | → `HIS_TRANSACTION.POS_CARD_HOLDER` |
| `INVOICE` | → `HIS_TRANSACTION.POS_INVOICE` (dùng để Void sau này) |
| toàn bộ object | → `HIS_TRANSACTION.POS_RESULT_JSON` |

---

## 2. Các chức năng đang sử dụng POS.WCFService

Tìm theo tham chiếu `WCF.Client.dll` / `WCF.dll` trong `.csproj` và lời gọi `WcfClient` trong mã nguồn. **6 plugin**:

| # | Plugin | Nghiệp vụ | Vị trí gọi | Thao tác | Điều kiện kích hoạt | Lưu kết quả |
|---|--------|-----------|-----------|----------|--------------------|-------------|
| 1 | `HIS.Desktop.Plugins.TransactionBill` | Thanh toán viện phí | [frmTransactionBill__Plus__Button.cs:1471](HIS/Plugins/HIS.Desktop.Plugins.TransactionBill/frmTransactionBill__Plus__Button.cs#L1471) | `Sale` | `chkConnectPOS.Checked` && PAY_FORM ∈ {`QUET_THE`, `TMQT`} && số tiền > 0 | `Transaction.POS_PAN/POS_CARD_HOLDER/POS_INVOICE/POS_RESULT_JSON` |
| 2 | `HIS.Desktop.Plugins.TransactionBillTwoInOne` | Thanh toán 2 trong 1 (hóa đơn + biên lai) | [frmTransactionBillTwoInOne__Plus__Button.cs:449](HIS/Plugins/HIS.Desktop.Plugins.TransactionBillTwoInOne/frmTransactionBillTwoInOne__Plus__Button.cs#L449) | `Sale` | tương tự #1 | `InvoiceTransaction` + `RecieptTransaction`.`POS_RESULT_JSON` |
| 3 | `HIS.Desktop.Plugins.TransactionDeposit` | Tạm ứng viện phí | [frmTransactionDeposit.cs:2180](HIS/Plugins/HIS.Desktop.Plugins.TransactionDeposit/frmTransactionDeposit.cs#L2180) | `Sale` | `chkConnectionPOS.Checked` | `Transaction.POS_RESULT_JSON` |
| 4 | `HIS.Desktop.Plugins.DepositService` | Tạm ứng theo dịch vụ | [frmDepositService.cs:2635](HIS/Plugins/HIS.Desktop.Plugins.DepositService/DepositService/frmDepositService.cs#L2635) | `Sale` | `chkConnectionPOS.Checked` | `hisDepositSDO.Transaction.POS_*` |
| 5 | `HIS.Desktop.Plugins.TransactionCancel` | **Hủy** giao dịch viện phí | [frmTransactionCancel.cs:584](HIS/Plugins/HIS.Desktop.Plugins.TransactionCancel/frmTransactionCancel.cs#L584) | `Void` | `chkConnectionPOS.Checked` && `POS_INVOICE` ≠ rỗng && HisConfig `HIS.Desktop.TransactionBillSelect == "1"` | cập nhật lại `transaction.POS_*` |
| 6 | `HIS.Desktop.Plugins.ExpMestSaleCreate` | Bán thuốc nhà thuốc | [UCExpMestSaleCreate___Proccess.cs:241](HIS/Plugins/HIS.Desktop.Plugins.ExpMestSaleCreate/UCExpMestSaleCreate___Proccess.cs#L241) | `Sale` | `ChkKetNoiPOS.Checked` && `chkCreateBill.Checked` | `saleSDO.PosPan/PosCardHoder/PosInvoice/PosResultJson` |

### 2.1 Mẫu tích hợp chung của 6 plugin

```csharp
WcfClient cll;                                  // field của Form/UC

OpenAppPOS();                                   // Process.Start(...\Integrate\POS.WCFService\WCF.exe) nếu chưa chạy
WcfRequest wc = new WcfRequest();
wc.AMOUNT  = (long)soTien;
wc.billId  = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
wc.creator = loginName;
var json   = JsonConvert.Serialize<WcfRequest>(wc);
if (cll == null) cll = new WcfClient();         // lỗi ⇒ "Kiểm tra lại cấu hình NetTcpBinding_IService1"
var result = cll.Sale(Convert.ToBase64String(Encoding.UTF8.GetBytes(json)));

if (result != null && result.RESPONSE_CODE == "00") { /* lưu POS_* rồi mới gọi API tạo phiếu */ }
else { /* "Giao dịch không thành công (Mã lỗi: " + result.ERROR + ")" → hủy toàn bộ */ }
```

Nút **cấu hình POS** (`btnPosConfig_Click`) → `OpenAppPOS()` → `cll.cauhinh()` → mở `FormConnect` trong WCF.exe.
Checkbox kết nối POS được nhớ trạng thái qua `ControlStateWorker` (KEY = tên checkbox, MODULE_LINK = plugin).

### 2.2 Tích hợp POS khác — KHÔNG dùng module này (để phân biệt)

| Module | Thư viện | Mục đích |
|--------|----------|----------|
| `HIS.Desktop.Common.BankQrCode` → `PosProcessor` | `IPOS.WCFService.Client` (`Integrate\IPOS\IPOS.exe`) | Đẩy ảnh QR sang màn hình phụ / thiết bị IPOS |
| `HIS.Desktop.Plugins.CreateTransReqQR` | `QrCodeProcessor` + `PosStatic.SendData` | Sinh & hiển thị QR chuyển khoản |
| Nhiều plugin viện phí | `CARD.WCF.Client.TransactionClient` | Thẻ khám bệnh nội bộ (không phải POS ngân hàng) |

### 2.3 Backend lưu thông tin máy POS như thế nào

Kiểm chứng trên source backend `E:\svn\IMSys\BACKEND\MOS`.

| Plugin | API | Lưu `POS_*`? | Căn cứ |
|--------|-----|-------------|--------|
| TransactionBill, TransactionBillTwoInOne | `HisTransaction/CreateBill` | ✔ | `HisTransactionBillCreate.cs:303` → `hisTransactionCreate.Create(data.Transaction, …)` |
| TransactionDeposit, DepositService | `HisTransaction/CreateDeposit` | ✔ | `HisTransactionDepositCreate.cs:139` |
| Điều chỉnh hóa đơn | `HisTransaction/AdjustmentBill` | ✔ | `HisTransactionAdjustmentBill.cs:114` |
| ExpMestSaleCreate | `HisExpMest/SaleCreateList` | ✔ | `HisExpMestSaleCreateListSdo.cs:270-273` — gán tường minh 4 cột |
| TransactionCancel | `HisTransaction/Cancel` | ✘ | SDO không có trường POS — xem §5.6a |

**Cơ chế:** frontend gán thẳng lên `data.Transaction` (kiểu `HIS_TRANSACTION`), backend `HisTransactionCreate.Create()` gọi `DAOWorker.HisTransactionDAO.Create(data)` — **insert nguyên entity**, nên mọi cột đã map đều xuống DB mà backend không phải liệt kê từng cột.

#### TID / MID / Serial máy POS

Không có cột riêng, nhưng **đã được lưu** trong `POS_RESULT_JSON`:

| Trường TCB trả về | `WcfRequest` | Ví dụ |
|---|---|---|
| `transInfo.tid` | `TERMINAL_ID` | `60002668` |
| `transInfo.mid` | `MERCHANT_CODE` | `010012345678901` |
| `transInfo.posSerial` | `SERIAL_NUMBER` | `V9E0857895` |

Đo thực tế chuỗi JSON của một giao dịch TCB: **588 bytes / giới hạn cột 4000** — dư 85%, không có rủi ro tràn.

Cả 4 hãng dùng chung cơ chế này, nên báo cáo đối soát theo TID viết một lần dùng được cho tất cả:

| Hãng | TID | MID | Serial |
|------|-----|-----|--------|
| MB (PAX) | key `TERMINAL_ID` trong chuỗi `KEY:VALUE` | `MERCHANT_CODE` | `SERIAL_NUMBER` |
| SHB | `transInfo.tid` | `transInfo.mid` | `transInfo.posSerial` |
| BIDV | `transInfo.tid` | `transInfo.mid` | `transInfo.posSerial` |
| TCB | `transInfo.tid` | `transInfo.mid` | `transInfo.posSerial` |

#### Giới hạn các cột POS (`DataModelTable.edmx`)

| Cột | Kiểu | Giá trị TCB thực tế | Đủ chỗ? |
|-----|------|--------------------|---------|
| `POS_INVOICE` | `varchar2(20)` | `receiptNo` (số) | ✔ |
| `POS_PAN` | `varchar2(20)` | `457362******1234` (16) | ✔ |
| `POS_CARD_HOLDER` | `varchar2(100)` | tên chủ thẻ | ✔ |
| `POS_RESULT_JSON` | `varchar2(4000)` | 588 bytes | ✔ |
| `CANCEL_REASON` | `varchar2(2000)` | lý do + đoạn nối ~55 ký tự | ✔ |

**Đã chốt: KHÔNG thêm cột `POS_TID` / `POS_MID` riêng.** TCB xử lý y hệt MB / SHB / BIDV — TID, MID, Serial nằm trong `POS_RESULT_JSON`. Lý do:

- Giữ nhất quán cả 4 hãng, không tạo ngoại lệ riêng cho TCB.
- Báo cáo đối soát theo TID viết một lần dùng chung cho mọi hãng.
- Không phát sinh thay đổi DB / EDMX / backend.

Đánh đổi chấp nhận: truy vấn theo TID phải lọc trong JSON (`POS_RESULT_JSON LIKE '%"TERMINAL_ID":"60002668"%'`), nên luôn giới hạn khoảng thời gian trước. Thực tế mỗi quầy cố định một máy POS nên `CASHIER_ROOM_ID` cũng suy ra được TID qua bảng ánh xạ quầy ↔ máy.

Nếu về sau đối soát theo TID thành nhu cầu thường xuyên: thêm cột vào DB + cập nhật `DataModelTable.edmx` / `DataModelView.edmx` rồi sinh lại — **không phải sửa Manager hay DAO** vì entity được insert trọn vẹn.

---

## 3. Đối chiếu TCB SmartPOS X990 với hiện trạng

### 3.1 Giao thức TCB

| Hạng mục | TCB SmartPOS | BIDV (đang có) | Chênh lệch |
|----------|--------------|----------------|-----------|
| Protocol | **HTTPS** + SSL self-signed cert | HTTP | Phải bypass/pin cert |
| URL Sale | `https://{ip}/ecr?rqt={requestId}\|{clientId}&cmd=sale\|{amount}` | `http://{ip}/ecr?rqt=…&cmd=sale\|…` | Cùng họ ECR |
| Header | `x-api-key`, `x-api-id` (tuỳ chọn) | không có | Không dùng — máy BV không bật xác thực |
| Body | `{"merchInfo":{ … }}` (thông tin đối soát) | không có | **Bổ sung** |
| Hủy thao tác | `cmd=ocan`, **PORT KHÁC** (TCB cấp sau) | `cmd=void\|{invoice}` | **Khác bản chất** — xem §3.3 |
| Thanh toán QR | `cmd=qr_p` / `cmd=qr_d` | không có | Không triển khai — hợp đồng chỉ quẹt thẻ |
| Truy vấn | COMING SOON | không có | Chưa có |
| Response | `{requestId, clientId, command, status, transInfo, qrTransInfo}` | `{requestId, clientId, command, status, transInfo}` | Thêm `qrTransInfo` |
| Thành công | `status = DONE` **và** `transInfo.returnCode = "00"` **và** `transStatus = APPROVE` | cùng logic | Giữ nguyên |
| Thất bại | `status = CANCELED` (user bấm Cancel / timeout / mất kết nối) | — | Cần phân biệt rõ |

### 3.2 Ánh xạ `transInfo` (TCB) → `WcfRequest` (HIS)

| `WcfRequest` | Nguồn TCB | Ghi chú |
|--------------|-----------|---------|
| `RESPONSE_CODE` | `"00"` khi `status=DONE && returnCode="00" && transStatus="APPROVE"` | Cờ quyết định của mọi plugin |
| `ERROR` | `transInfo.returnCode`; `"111"` khi `status != DONE` | Giữ quy ước BIDV |
| `PAN` | `transInfo.cardNo` | Đã masked 6 đầu + 4 cuối |
| `NAME` | `transInfo.cardHolder` | |
| `INVOICE` | `transInfo.receiptNo` | Số hóa đơn — dùng để hủy |
| `AMOUNT` | `transInfo.amount / 100` | ⚠ TCB trả **thêm 2 số 00 thập phân** |
| `REF_NO` | `transInfo.refNo` | 12 số |
| `APPV_CODE` | `transInfo.approvalCode` | 6 số |
| `TERMINAL_ID` | `transInfo.tid` | 8 số |
| `MERCHANT_CODE` | `transInfo.mid` | 15 số |
| `SERIAL_NUMBER` | `transInfo.posSerial` | |
| `CARD_TYPE` | `transInfo.cardType` | VISA/Master/local |
| `CURRENCY_CODE` | `transInfo.currencyCode` | mặc định 704 |
| `DATE` / `TIME` | `transInfo.time` (`YYYYMMDDhhmmss`) | Tách 8 + 6 ký tự |

### 3.3 ⚠ Khoảng trống nghiệp vụ CRITICAL — hủy giao dịch

Tài liệu TCB chỉ có `cmd=ocan` = **"Hủy thao tác thanh toán"** — theo ghi chú của TCB: *"API này chỉ có tác dụng ở màn hình tap/chèn/quẹt thẻ"*. Đây là hủy **thao tác đang chờ**, KHÔNG phải void/refund một giao dịch đã APPROVE.

Hệ quả: **`HIS.Desktop.Plugins.TransactionCancel` (chức năng #5) không thể vận hành với POS TCB** theo tài liệu hiện tại.

Cần chốt với TCB một trong các phương án:

| Phương án | Mô tả | Ảnh hưởng |
|-----------|-------|-----------|
| A | TCB bổ sung API `void`/`refund` theo `receiptNo` | Giữ nguyên luồng HIS, chỉ cần cài đặt thêm |
| B | Hủy thủ công trên máy POS + Merchant Portal, HIS chỉ ghi nhận | Đơn giản nhưng HIS ghi đã hủy trong khi ngân hàng vẫn ghi đã thu ⇒ **vẫn lệch đối soát** |
| **B+** | **Như B nhưng ràng buộc hai bên vào cùng một thao tác**: HIS chỉ cho hoàn tất hủy sau khi thu ngân nhập mã hủy lấy từ máy POS | **Đã chọn và hiện thực** — xem §5.6a |
| C | Chờ API Truy vấn (COMING SOON) rồi mới hủy | Lùi tiến độ |

**Đã chốt: B+ ngay, A khi TCB cấp API.** Hai phương án chạy song song được, không loại trừ nhau:
`PosDeviceTcb.Void()` hiện trả mã `998` (chưa hỗ trợ) nên plugin rơi vào nhánh xác nhận thủ công. Khi TCB cấp API void thật, chỉ cần hiện thực `Void()` trả `00` là plugin tự chuyển sang luồng tự động — **giao diện và nghiệp vụ không phải sửa lại**.

Lý do không chọn B thuần: khóa ô tích POS chỉ ngăn HIS gọi sang máy POS, nhưng nút hủy phía HIS vẫn chạy bình thường ⇒ HIS ghi đã hủy, ngân hàng vẫn ghi đã thu, không ai phát hiện cho tới kỳ đối soát.

---

## 4. Phát hiện lỗi/rủi ro trên module hiện tại

| Mức | Vấn đề | Vị trí | Tác động | Đề xuất |
|-----|--------|--------|----------|---------|
| CRITICAL | Timeout HTTP = `request.Timeout = 99999999` (≈27 giờ). Vòng `while(Check(count))` vô nghĩa với luồng HTTP vì `Socket == null` ⇒ trả `false` ngay | `Service1.SHB_Sale`, `Service1.BIDV_SaleVoid` | Máy POS treo ⇒ HIS đơ vô hạn, thu ngân phải kill app, giao dịch mồ côi | Đặt `Timeout` từ `PortConfig.TimeOutSecond`, bỏ vòng lặp `Check` |
| CRITICAL | Không có giao dịch truy vấn/đối soát. Nếu POS đã `DONE` mà HIS mất response ⇒ tiền đã trừ, phiếu không tạo | toàn module | Thất thoát/lệch quỹ | Ghi nhật ký `billId` trước khi gửi; đối soát Merchant Portal cuối ca |
| HIGH | `request.GetRequestStream()` được gọi trên request `Method = "GET"` | `BIDV_SaleVoid` nhánh timeout | `ProtocolViolationException` | Bỏ, chuyển sang POST khi có body |
| HIGH | Khóa 3DES hardcode trong mã nguồn | `Service1.EncryptMessage/DecryptMessage` | Rò rỉ khóa nếu decompile | Đưa vào config mã hóa hoặc DPAPI |
| HIGH | `wc` là field instance, không reset giữa các giao dịch; nhánh SHB/BIDV chỉ gán một phần trường | `Service1` | Dữ liệu giao dịch trước lẫn sang giao dịch sau | Khởi tạo `WcfRequest` cục bộ trong từng lời gọi |
| HIGH | `MessageBox.Show` trong tiến trình dịch vụ (kể cả trong `catch`) | `Service1`, `ConnectConfig` | Treo luồng WCF, hộp thoại không có owner | Thay bằng `LogSystem.Error` + trả mã lỗi |
| MEDIUM | `catch (Exception ex) { }` rỗng | `Service1.Void` (MB), `TCP_Disconnect`, `ReceiveResponse`, `ParseTranxResultMsg`, `StartService` | Nuốt lỗi, không debug được — vi phạm `logging_guidelines.md` | Bổ sung `LogSystem.Error(ex)` |
| MEDIUM | `ConnectService` khởi tạo `new Service1()` (mỗi lần lại `new ServiceHost` → `host.Open()` ném lỗi và bị nuốt) | `ConnectService.cs` | Rò rỉ đối tượng, khó suy luận trạng thái | Tách host khỏi lớp logic; `Service1` thành singleton |
| MEDIUM | `WcfRequest` là DTO hợp nhất của 4 hãng (40+ trường, tên lẫn PascalCase/camelCase) | `WCF.Data.Pax.cs` | Khó bảo trì, dễ gán nhầm | Giữ nguyên cho tương thích, thêm DTO riêng cho từng hãng ở tầng device |
| MEDIUM | Danh sách ngân hàng hardcode `{ "POS MB", "POS SHB", "POS BIDV" }` | `FormConnect.cs:27` | Thêm hãng phải sửa nhiều nhánh `if` | Chuyển sang enum + Strategy (§5) |
| LOW | `IsProcessOpen("WCF")` đếm `Count >= 2` và so khớp `Contains` | các plugin | Nhận nhầm tiến trình khác có tên chứa "WCF" | So khớp chính xác `ProcessName == "WCF"` |
| LOW | Mã nguồn còn biến `json` mẫu chứa IP/số tiền test | `SHB_Sale`, `BIDV_SaleVoid` | Rác | Xóa |
| MEDIUM | Luồng MB đọc key `"TERMINAL _ID"` **có dấu cách**, mọi key khác đều không có (`Service1.cs:769` bản gốc) | `ParseTranxResultMsg` | Nếu máy PAX gửi `TERMINAL_ID` thì TID của máy MB xưa nay luôn rỗng trong `POS_RESULT_JSON` — thiếu dữ liệu đối soát mà không báo lỗi | Đọc cả hai biến thể (đã sửa) |

### Trạng thái xử lý

Đã sửa cùng đợt refactor: toàn bộ CRITICAL (trừ đối soát), HIGH và MEDIUM, cùng 2 mục LOW.

| Còn lại | Ghi chú |
|---------|---------|
| CRITICAL — không có giao dịch truy vấn/đối soát | Phụ thuộc TCB (API đang COMING SOON). Trước mắt: `billId` được ghi log trước khi gửi, đối soát thủ công trên Merchant Portal |
| LOW — `IsProcessOpen("WCF")` so khớp `Contains` và đếm `>= 2` | Nằm trong 6 plugin HIS, không thuộc module POS. Sửa sẽ phải build lại cả 6 plugin nên tách sang đợt sau |

---

## 5. Thiết kế bổ sung POS TECHCOMBANK

### 5.1 Nguyên tắc

1. **KHÔNG đổi `IService1`** ở giai đoạn 1 ⇒ 6 plugin không phải sửa, không phải build lại, không phải sinh lại proxy `PaxSv`.
2. Tách logic từng hãng POS theo **Strategy**, thay cho chuỗi `if (ConnectConfig.NameBank == …)` trong `Service1`.
3. Giữ nguyên hợp đồng dữ liệu `WcfRequest` làm "mẫu số chung"; mọi hãng tự quy đổi về `RESPONSE_CODE`/`ERROR`/`PAN`/`NAME`/`INVOICE`.

### 5.2 Cấu trúc thư mục đề xuất (`May PS/WCF/WCF/`)

```
WCF/
├── IService1.cs                     (GIỮ NGUYÊN — không đổi hợp đồng)
├── ConnectService.cs                → chỉ chuyển tiếp sang PosGateway
├── Service1.cs                      → chỉ còn: mở/đóng ServiceHost
├── ConnectConfig.cs                 → gộp Get/Set chung + thêm 8 khóa TCB
├── FormConnect.cs / .Designer.cs    → thêm "POS TECHCOMBANK" + 5 ô nhập + nút kiểm tra
├── WCF.Data.Pax.cs                  (giữ nguyên — hợp đồng với HIS)
├── WCF.Data.BIDV.cs                 (giữ nguyên)
├── WCF.Data.TCB.cs                  ← MỚI: WcfResponseTCB / transInfoTCB / qrTransInfoTCB
├── Common/
│   ├── PosBankName.cs               ← MỚI: hằng tên hãng, bỏ hardcode
│   ├── PosResponseCode.cs           ← MỚI: 00 / 111 / 997 / 998 / 999
│   └── TcbReturnCode.cs             ← MỚI: bảng mã lỗi ISO-8583 (tiếng Việt)
└── Devices/
    ├── IPosDevice.cs                interface: Sale / Void / TestConnection
    ├── PosDeviceBase.cs             ← MỚI: HTTP dùng chung, pin chứng thư, log
    ├── PosDeviceFactory.cs          chọn thiết bị theo ConnectConfig.NameBank
    ├── PosGateway.cs                ← MỚI: giải mã Base64/JSON, điều phối, bắt lỗi
    ├── PosDeviceMb.cs               tách từ Service1 (socket + 3DES)
    ├── PosDeviceShb.cs              tách từ Service1 (HTTP middleware)
    ├── PosDeviceBidv.cs             tách từ Service1 (ECR HTTP)
    └── PosDeviceTcb.cs              ← MỚI (ECR HTTPS + header + merchInfo)
```

**Lệch so với thiết kế ban đầu:** `WCF.Data.TCB.cs` đặt ở thư mục gốc cùng `WCF.Data.Pax.cs` / `WCF.Data.BIDV.cs` thay vì tạo thư mục `Data/` — giữ nguyên cách sắp xếp sẵn có, tránh churn không cần thiết. Bổ sung thêm `PosDeviceBase.cs`, `PosGateway.cs`, `PosResponseCode.cs` so với bản thiết kế.

### 5.3 Interface thiết bị

```csharp
namespace WCF.Devices
{
    /// <summary>Hợp đồng chung cho mọi hãng máy POS.</summary>
    internal interface IPosDevice
    {
        /// <summary>Giao dịch thanh toán. Trả về WcfRequest đã quy đổi về mẫu số chung.</summary>
        WcfRequest Sale(WcfRequest request);

        /// <summary>Giao dịch hủy. Ném NotSupportedException nếu hãng không hỗ trợ.</summary>
        WcfRequest Void(WcfRequest request);

        /// <summary>Kiểm tra kết nối tới thiết bị, dùng cho nút Test trên FormConnect.</summary>
        bool TestConnection(out string message);
    }
}
```

`Service1.Sale` rút gọn còn:

```csharp
public string Sale(string data)
{
    try
    {
        if (string.IsNullOrWhiteSpace(ConnectConfig.NameBank)) { ShowConfigForm(); return ""; }

        WcfRequest request = DecodeRequest(data);
        Inventec.Common.Logging.LogSystem.Debug("POS.Sale.Request"
            + Inventec.Common.Logging.LogUtil.TraceData(
                Inventec.Common.Logging.LogUtil.GetMemberName(() => request), request));

        IPosDevice device = PosDeviceFactory.Create(ConnectConfig.NameBank);
        WcfRequest response = device.Sale(request);

        Inventec.Common.Logging.LogSystem.Debug("POS.Sale.Response"
            + Inventec.Common.Logging.LogUtil.TraceData(
                Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response));
        return EncodeResponse(response);
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
        return EncodeResponse(new WcfRequest { RESPONSE_CODE = "", ERROR = "999" });
    }
}
```

### 5.4 `PosDeviceTcb` — luồng Sale

```
1. Dựng URL:
   https://{IpTcb}/ecr?rqt={requestId}|{clientId}&cmd=sale|{amount}
     requestId = request.creator     (tài khoản thu ngân, ≤ 12 ký tự)
     clientId  = request.billId      (GUID 20 ký tự — DUY NHẤT, dùng định danh & truy vấn)
     amount    = request.AMOUNT      (VNĐ, KHÔNG thêm 00)

2. Header:
   Content-Type: application/json; charset=UTF-8
   (KHÔNG gửi x-api-key / x-api-id — 10 máy của bệnh viện không bật xác thực)

3. Body:
   {"merchInfo":{"bill_number":"{billId}","cashier":"{creator}","hospital_code":"{MerchInfoHospitalCode}"}}

4. Cert: self-signed ⇒ ServerCertificateValidationCallback chỉ chấp nhận
   thumbprint đã cấu hình (KHÔNG return true vô điều kiện).

5. Timeout = ConnectConfig.TimeOut * 1000.

6. Parse TcbEcrResponse → quy đổi theo bảng §3.2.
```

Sơ đồ tuần tự:

```
Thu ngân        Plugin viện phí        WCF.exe/PosDeviceTcb        SmartPOS X990        TCB Host
   │  bấm Lưu         │                        │                        │                  │
   │─────────────────>│                        │                        │                  │
   │                  │ CheckBill (API MOS)    │                        │                  │
   │                  │ Sale(base64)           │                        │                  │
   │                  │───────────────────────>│                        │                  │
   │                  │                        │ GET/POST /ecr cmd=sale │                  │
   │                  │                        │───────────────────────>│                  │
   │  quẹt/chạm thẻ   │                        │                        │─────────────────>│
   │  hoặc quét QR    │                        │                        │<─────────────────│
   │                  │                        │<───────────────────────│  status=DONE     │
   │                  │<───────────────────────│  RESPONSE_CODE="00"    │  returnCode=00   │
   │                  │ CreateBill (API MOS) + lưu POS_*                │                  │
   │<─────────────────│ In hóa đơn + biên lai POS                       │                  │
```

### 5.5 Cấu hình mới

| Key (appSettings) | Kiểu | Ý nghĩa | Bắt buộc |
|-------------------|------|---------|----------|
| `namebank` | string | thêm giá trị `POS TECHCOMBANK` | ✔ |
| `ipTCB` | string(15) | IP máy POS TCB | ✔ |
| `portTCB` | string | Port ECR (mặc định 443) | |
| `portOcanTCB` | string | Port riêng cho lệnh `ocan` (TCB cấp sau) | |
| `certThumbprintTCB` | string | Vân tay chứng thư self-signed để pin. Bỏ trống ⇒ chấp nhận mọi chứng thư + ghi `Warn` (chỉ dùng khi tích hợp thử) | PROD |
| `merchInfoTCB` | json | Mẫu `merchInfo` bổ sung (mã bệnh viện, mã quầy) | |
| `httpMethodTCB` | string | Phương thức HTTP của lệnh sale. Mặc định `POST`; đổi sang `GET` được mà không phải build lại — phục vụ điểm cần chốt §7.5 | |
| `PortConfig.TimeOutSecond` | int | dùng chung, khuyến nghị 90 (chờ khách thao tác) | |

`FormConnect` bổ sung nhánh `POS TECHCOMBANK`: bật `txtIP`, `txtPort`, thêm `txtApiKey`, `txtApiId`, `txtCertThumbprint`, `memoMerchInfo`, nút **Kiểm tra kết nối** gọi `IPosDevice.TestConnection`.

### 5.6 Ảnh hưởng tới 6 plugin

| Plugin | Sửa mã? | Ghi chú |
|--------|---------|---------|
| TransactionBill | Không | Chạy được ngay sau khi cập nhật `WCF.exe` |
| TransactionBillTwoInOne | Không | |
| TransactionDeposit | Không | |
| DepositService | Không | |
| ExpMestSaleCreate | Không | |
| **TransactionCancel** | **Có** | Đã hiện thực phương án B — xem §5.6a |

Chỉ cần build lại `WCF.exe` + `WCF.dll` và triển khai vào `Integrate\POS.WCFService\`. `WCF.Client.dll` **không đổi** (hợp đồng `IService1` giữ nguyên).

### 5.6a TransactionCancel — hiện thực phương án B+

Hợp đồng `IService1` chỉ có `Sale`/`Void`/`cauhinh`, không hỏi được tên hãng POS qua WCF. Nếu thêm method mới thì phải sinh lại proxy và build lại cả 6 plugin. Vì vậy plugin đọc thẳng file cấu hình của tiến trình POS:

```csharp
// Base/PosDeviceConfigReader.cs
// Đọc appSettings["namebank"] trong {StartupPath}\Integrate\POS.WCFService\WCF.exe.config
internal static bool IsVoidSupported()   // false khi namebank == "POS TECHCOMBANK"
```

#### Luồng hủy khi máy POS không tự hủy được

```
Thu ngân bấm Hủy
  → HIS đọc namebank từ WCF.exe.config
  → Không hủy tự động được?
      ├── Không → gọi cll.Void() qua WCF như cũ (MB / BIDV)
      └── Có   → 1. Hộp thoại cảnh báo kèm POS_INVOICE + POS_PAN + POS_CARD_HOLDER
                    "Hủy trên máy POS / Merchant Portal TRƯỚC"
                 2. Thu ngân hủy trên máy POS, lấy mã hủy trên biên lai
                 3. Hộp thoại nhập mã hủy — BẮT BUỘC, để trống thì dừng
                 4. Ghi nhận rồi mới gọi api/HisTransaction/Cancel
```

Không hủy được trên máy POS ⇒ không có mã để nhập ⇒ HIS không hủy. Hai bên không lệch âm thầm.

Các thành phần trong [frmTransactionCancel.cs](../HIS/Plugins/HIS.Desktop.Plugins.TransactionCancel/frmTransactionCancel.cs):

| Thành phần | Vai trò |
|-----------|---------|
| `ApplyPosVoidRestriction()` — gọi trong Load | Gắn tooltip báo trước cho thu ngân. **Không khóa ô tích** — việc hủy vẫn phải làm được |
| `ConfirmManualPosVoid(out posVoidRef)` | Cảnh báo + bắt nhập mã hủy, trả `false` nếu thu ngân bỏ qua hoặc để trống |
| `ShowPosVoidRefInputDialog(...)` | Hộp thoại nhập, dựng tại chỗ vì DevExpress 15.2 **không có** `XtraInputBox` |
| `ApplyManualPosVoid(sdo, posVoidRef)` | Ghi nhận mã hủy + ghi `LogAction` audit |

#### Nơi lưu vết — đã kiểm chứng trên source backend

`HisTransactionCancelSDO` chỉ có `TransactionId`, `RequestRoomId`, `CancelReason`, `CancelReasonId`, `CancelTime`, `IsInternal` — **không có trường nào mang được `POS_RESULT_JSON` lên backend** (quét toàn bộ `MOS.SDO`/`MOS.MANAGER`/`MOS.DAO`: `POS_RESULT_JSON` chỉ xuất hiện ở luồng bán thuốc `HisExpMestSaleCreateListSdo.cs:273`).

**Đã chốt: lưu vào `CANCEL_REASON`.** Kiểm chứng trên `E:\svn\IMSys\BACKEND\MOS`:

| Kiểm chứng | Kết quả |
|-----------|---------|
| `HisTransactionCancel.cs:191` — `raw.CANCEL_REASON = data.CancelReason;` | Gán **vô điều kiện**, không phụ thuộc `CancelReasonId` |
| `HisTransactionCancel.cs:197` — `raw.CANCEL_REASON_ID = data.CancelReasonId;` | Hai trường độc lập, không ghi đè nhau |
| `HisTransactionCancel.cs:216` — `DAOWorker.HisTransactionDAO.Update(raw)` | Update **nguyên entity** ⇒ mọi cột đã gán đều xuống DB |
| `HIS_TRANSACTION.CANCEL_REASON` trong `DataModelTable.edmx` | `varchar2(2000)` — đoạn nối ~55 ký tự, dư sức chứa |
| `CANCEL_REASON` có trong `V_HIS_TRANSACTION` | ✔ tra cứu / báo cáo đọc được |
| `raw.CANCEL_LOGINNAME` (`:194`) | Backend tự lấy từ token ⇒ frontend không cần gửi tài khoản |

Nội dung ghi xuống:

| Nơi lưu | Nội dung |
|---------|----------|
| `sdo.CancelReason` → `HIS_TRANSACTION.CANCEL_REASON` | `{lý do hủy} [Hủy trên máy POS TECHCOMBANK - mã hủy: {mã}]` |
| `LogAction.txt` | `TransactionCancel.ManualPosVoid____TransactionId=…____Bank=…____PosInvoice=…____VoidRef=…` |
| `this.transaction.POS_RESULT_JSON` | Chỉ gán tại chỗ cho hiển thị/in — backend không nhận khi hủy |

Không cần sửa backend. Nếu về sau muốn tách riêng, chỉ tốn 2 dòng: thêm `PosResultJson` vào `HisTransactionCancelSDO` và một dòng gán `raw.POS_RESULT_JSON` cạnh `HisTransactionCancel.cs:191`.

**Khi TCB cấp API void thật** → hiện thực `PosDeviceTcb.Void()` trả `RESPONSE_CODE = "00"` và bỏ chặn trong `PosDeviceConfigReader.IsVoidSupported()`; luồng tự chuyển sang tự động, giao diện không phải sửa.

### 5.7 Thanh toán QR — KHÔNG triển khai

**Hợp đồng chốt bệnh viện chỉ dùng quẹt thẻ.** Máy TCB có sẵn chức năng QR động nhưng không đưa vào phạm vi; `PosDeviceTcb` chỉ phát lệnh `sale`. Phần dưới giữ lại làm tham chiếu nếu sau này mở rộng.


Máy TCB sinh QR động (`cmd=qr_p`) hoặc nhận QR từ HIS (`cmd=qr_d`). Để dùng cần **mở rộng `IService1`**:

```csharp
[OperationContract] string SaleQr(string data);   // data thêm QrMode: qr_p | qr_d, QrData (base64)
```

⇒ phải sinh lại proxy `PaxSv`, build lại `WCF.Client.dll` và **build lại các plugin muốn dùng**. Đề xuất tách thành đợt riêng, sau khi luồng thẻ đã nghiệm thu. Response bổ sung `qrTransInfo` (`virtualAccount`, `accountingReference`, `bankCode`, `transactionDate`…) → nên lưu nguyên vào `POS_RESULT_JSON`.

DTO `qrTransInfoTCB` đã khai báo sẵn trong `WCF.Data.TCB.cs` nên giai đoạn 2 không phải dựng lại.

### 5.8 Trạng thái hiện thực

Đã viết mã, biên dịch sạch (chưa chạy với máy POS thật).

| Hạng mục | Trạng thái |
|----------|-----------|
| Tách Strategy `IPosDevice` + 4 thiết bị | Xong |
| `PosDeviceTcb` — HTTPS, `merchInfo`, pin chứng thư, ánh xạ `transInfo` | Xong |
| `PosDeviceTcb.Void` trả mã 998 (không hỗ trợ) | Xong — theo §3.3 phương án B+ |
| Gửi `ocan` tự động khi Sale hết thời gian chờ (giải phóng máy POS) | Xong |
| `ConnectConfig` — 6 khóa TCB + gộp Get/Set | Xong |
| `FormConnect` — thêm TCB, 3 ô nhập, nút "Kiểm tra kết nối", hiện/ẩn theo hãng | Xong |
| `TransactionCancel` — bắt xác nhận + nhập mã hủy lấy từ máy POS (B+) | Xong |
| Lưu mã hủy vào `CANCEL_REASON` + `LogAction` | Xong — đã kiểm chứng backend ghi xuống DB, không cần sửa backend (§5.6a) |
| Lưu TID/MID trong `POS_RESULT_JSON`, không thêm cột riêng | Đã chốt — thống nhất với MB/SHB/BIDV (§2.3) |
| Bỏ `x-api-key` / `x-api-id` khỏi cấu hình và giao diện | Xong |
| Chỉ phát lệnh `sale`, không dùng QR | Xong — theo hợp đồng (§5.7) |
| Build `WCF.exe` bản .NET Framework 4.5.0 | Xong — 289 KB, `bin\Debug\WCF.exe` |
| Chạy thử với máy POS Techcombank | **Chưa** — chỉ còn chờ IP máy POS |

**Điều chỉnh hành vi so với bản cũ** — cần lưu ý khi hồi quy MB/SHB/BIDV:

| Thay đổi | Lý do |
|----------|-------|
| Mỗi giao dịch tạo một thiết bị mới, trạng thái cục bộ | Sửa lỗi HIGH: `wc` là field dùng chung, dữ liệu giao dịch trước lẫn sang sau |
| Timeout HTTP lấy theo `PortConfig.TimeOutSecond` | Sửa lỗi CRITICAL: trước là `99999999` (≈27 giờ) |
| Bỏ `MessageBox` trong tiến trình dịch vụ, thay bằng mã lỗi trả về HIS + ghi log | Sửa lỗi HIGH: hộp thoại không có owner làm treo luồng WCF |
| SHB/BIDV khi lỗi trả về `ERROR` có mã (111/997/999) thay vì chuỗi rỗng | Thu ngân thấy được nguyên nhân |
| Mã lỗi thiết bị được diễn giải tiếng Việt vào `PRINT_MSG` | Trước dùng `m_ErrorCode` rỗng nên luôn ném `KeyNotFoundException` |

---

## 6. Kế hoạch kiểm thử

| # | Ca kiểm thử | Kỳ vọng |
|---|-------------|---------|
| 1 | Sale thành công (thẻ nội địa / VISA) | `status=DONE`, `returnCode=00`, `transStatus=APPROVE` ⇒ HIS tạo phiếu, `POS_*` đầy đủ |
| 2 | Khách bấm Cancel trên POS | `status=CANCELED` ⇒ HIS KHÔNG tạo phiếu, hiện mã lỗi |
| 3 | Thẻ không đủ số dư (`returnCode=51`) | HIS KHÔNG tạo phiếu, hiển thị đúng diễn giải từ bảng mã lỗi |
| 4 | Rút dây mạng POS giữa giao dịch | Timeout đúng `PortConfig.TimeOutSecond`, HIS không treo |
| 5 | Máy POS yêu cầu xác thực (nếu TCB bật sau này) | Trả lỗi rõ ràng, không tạo phiếu — khi đó phải bổ sung lại header |
| 6 | Chứng thư không khớp thumbprint | Từ chối kết nối, ghi log |
| 7 | Đối chiếu `AMOUNT` trả về với số tiền gửi | Đúng sau khi chia 100 |
| 8 | 2 quầy thu ngân giao dịch đồng thời trên 2 máy POS | Không lẫn dữ liệu (`WcfRequest` cục bộ) |
| 9 | Đối soát cuối ca với Merchant Portal theo `refNo`/`receiptNo` | Khớp 100% |
| 10 | Hủy giao dịch: bỏ trống mã hủy | HIS KHÔNG hủy, hiện cảnh báo |
| 11 | Hủy giao dịch: nhập mã hủy hợp lệ | HIS hủy thành công, `CANCEL_REASON` có `[Hủy trên máy POS TECHCOMBANK - mã hủy: …]` |
| 12 | Hủy giao dịch khi hãng POS là MB/BIDV | Vẫn gọi `cll.Void()` tự động như cũ, không hiện hộp thoại nhập mã |
| 13 | Hồi quy MB / SHB / BIDV sau refactor | Không thay đổi hành vi |

---

## 7. Việc cần chốt với Techcombank

1. **API void/refund** theo `receiptNo` — hiện đang phải hủy thủ công trên POS/Portal rồi nhập mã hủy vào HIS (§3.3, §5.6a).
2. Port riêng cho lệnh `ocan`.
3. File chứng thư self-signed + thumbprint. Không chặn quẹt thử: `certThumbprintTCB` để trống thì chấp nhận mọi chứng thư và ghi `Warn`. **Trước khi lên PROD bắt buộc phải điền** để tránh bị giả mạo thiết bị.
4. ~~`x-api-key` / `x-api-id`~~ — **đã bỏ khỏi phạm vi**: 10 máy của bệnh viện không bật xác thực, tài liệu TCB cho phép bỏ qua tham số này.
5. Phương thức HTTP chính xác của lệnh Sale (tài liệu ghi URL dạng query nhưng có BODY ⇒ POST hay GET-with-body?).
6. `amount` trong response có luôn kèm 2 số 00 hay không (ảnh hưởng phép chia 100).
7. Độ dài tối đa thực tế của `requestId` (12) — tài khoản thu ngân HIS có thể dài hơn ⇒ quy tắc cắt.
8. Thời điểm có API Truy vấn (đang COMING SOON) — cần cho đối soát tự động.

---

## 8. Ước lượng

| Hạng mục | Ước lượng |
|----------|-----------|
| Refactor Strategy + tách 3 device hiện có | 2 ngày |
| `PosDeviceTcb` (HTTPS, header, merchInfo, cert pinning, mapping) | 2 ngày |
| `FormConnect` + `ConnectConfig` cho TCB | 1 ngày |
| Xử lý các lỗi CRITICAL/HIGH ở §4 | 1,5 ngày |
| Sửa `TransactionCancel` theo phương án chốt | 0,5 ngày |
| Kiểm thử nội bộ + hồi quy MB/SHB/BIDV | 2 ngày |
| UAT tại bệnh viện với máy thật | 2 ngày |
| **Tổng** | **≈ 11 ngày công** (chưa gồm giai đoạn 2 — QR) |

---

## Changelog

| Ngày | Nội dung | Người |
|------|----------|-------|
| 2026-09-09 | Khởi tạo — phân tích hiện trạng POS.WCFService, liệt kê 6 chức năng sử dụng, thiết kế bổ sung POS Techcombank | khainq |
| 2026-09-10 | Hiện thực: tách Strategy `IPosDevice` (MB/SHB/BIDV/TCB), thêm `PosDeviceTcb` + `PosGateway` + `PosDeviceBase`, 8 khóa cấu hình TCB, `FormConnect` hỗ trợ TCB. Sửa kèm các lỗi CRITICAL/HIGH/MEDIUM ở mục 4 | khainq |
| 2026-09-10 | Đổi hướng xử lý hủy giao dịch từ khóa cứng sang **B+**: bắt thu ngân hủy trên máy POS rồi nhập mã hủy thì HIS mới cho hủy, tránh lệch đối soát. Ghi mã hủy vào `CANCEL_REASON` + `LogAction` | khainq |
| 2026-09-10 | Bỏ hẳn `x-api-key`/`x-api-id` khỏi cấu hình, giao diện và mã nguồn; chốt chỉ quẹt thẻ không dùng QR; build `WCF.exe` bản .NET 4.5.0 | khainq |
| 2026-09-10 | Hạ `x-api-key`/`x-api-id`/chứng thư từ điều kiện chặn xuống tuỳ chọn — tài liệu TCB cho phép bỏ qua khi POS không yêu cầu xác thực; chỉ cần IP máy POS là quẹt thử được | khainq |
| 2026-09-10 | Chốt lưu TID/MID trong `POS_RESULT_JSON` như các hãng MB/SHB/BIDV, không thêm cột `POS_TID`/`POS_MID` | khainq |
| 2026-09-10 | `PosDeviceMb` đọc cả `TERMINAL_ID` lẫn `TERMINAL _ID` — bản gốc chỉ đọc biến thể có dấu cách, nghi TID máy MB xưa nay bị rỗng | khainq |
| 2026-09-10 | Kiểm chứng trên source backend MOS: xác nhận `POS_*` xuống DB ở luồng thanh toán, `CANCEL_REASON` xuống DB ở luồng hủy, TID/MID nằm sẵn trong `POS_RESULT_JSON`. Bổ sung §2.3 | khainq |
| 2026-09-10 | Build + deploy `HIS.Desktop.Plugins.TransactionCancel`: nhánh hủy theo máy POS Techcombank, `Base/PosDeviceConfigReader.cs` đọc `Integrate\POS.WCFService\WCF.exe.config` khóa `namebank`, 3 thông báo mới. DLL build ở **.NET 4.5.0** (x64) cho khớp `WCF.exe` — bản deploy 4.5.2 trước đó đã được ghi đè | khainq |
