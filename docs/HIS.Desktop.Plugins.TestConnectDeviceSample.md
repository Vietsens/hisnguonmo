# Kết Nối Thiết Bị Lấy Mẫu Bệnh Phẩm — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TestConnectDeviceSample |
| Loại | UC (`UCTestConnectDeviceSample : UserControlBase`), mở thành tab theo phòng |
| Mục đích | Kết nối nhiều máy quét barcode chạy chế độ cổng COM. Quét thẻ nhân viên (`TK-` + tài khoản) trên một máy quét thì cổng đó "đăng nhập" tài khoản ấy. Quét barcode mẫu (hoặc mã y lệnh 12 số) thì mở popup Thông tin lấy mẫu, điền sẵn người lấy mẫu = tài khoản của cổng và TG lấy mẫu = lúc quét |
| Người tạo | ninhdd (bản terminal cũ, 07/2024); viết lại: dangth (việc 60557, BV Yên Bái) |
| Ngày viết lại | 21/10/2026 |
| Trạng thái | Đang phát triển (GĐ1 việc 60557) |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Chọn cổng COM của máy quét → **Kết nối (Ctrl K)**. Lặp lại cho từng máy quét. Mỗi cổng hiện thành một ô (tile): vàng = chờ quét thẻ, xanh = đã đăng nhập (họ tên, tài khoản, giờ đăng nhập, lượt quét gần nhất), đỏ = mất kết nối.
2. Cán bộ lấy mẫu quét thẻ nhân viên (in ở màn Tài khoản/Nhân viên, nút In barcode) trên máy quét của mình → cổng gắn với tài khoản đó.
3. Quét barcode ống mẫu hoặc phiếu chỉ định → tìm mẫu chờ lấy tại phòng → mở popup `LIS.Desktop.Plugins.SampleInfo` (tiêu đề ghi cổng và người lấy mẫu) → Lưu/Lưu in → popup tự đóng, màn Lấy mẫu bệnh phẩm cùng phòng tự làm mới.
4. Đổi người: Đăng xuất dòng của tài khoản ở màn **DS đăng nhập** (`HIS.Desktop.Plugins.TestDeviceSample`, mở không chặn) hoặc **Ngắt kết nối (Ctrl D)** cổng đang chọn (bấm vào ô để chọn).

### Quy tắc mỗi lượt quét (cổng P, giờ nhận T)
| # | Nếu | Thì |
|---|-----|-----|
| 1 | Cùng chuỗi, cùng cổng, cách lượt đọc trước (kể cả lượt đã bỏ) ≤ 3 giây | Bỏ ngay lúc nhận (`RepeatScanFilter`, cửa sổ trượt) |
| 2 | Thẻ không ra đúng 1 tài khoản đang hoạt động (không có, khóa, lỗi tra cứu) | Báo, không gắn |
| 3 | Thẻ của A, P chưa gắn | Gắn P = A từ T; A đang gắn cổng khác thì cổng đó đăng xuất; ghi nhật ký |
| 4 | Thẻ của A, P đang gắn A | Báo "P đang đăng nhập A" |
| 5 | Thẻ của A, P đang gắn X ≠ A | **Chặn**, báo phải đăng xuất X trước; ghi nhật ký |
| 6 | Barcode mẫu, P chưa gắn | Cảnh báo quét thẻ trước |
| 7 | Không có mẫu chờ lấy (barcode, rồi mã y lệnh 12 số) | Báo "đã lấy lúc … bởi …" (tra cả theo mã y lệnh khi quét phiếu chỉ định) hoặc "không có mẫu chờ lấy …" |
| 8 | Mẫu chờ lấy thuộc ≥ 2 hồ sơ | Báo trùng, không mở popup |
| 9 | Hồ sơ đã kết thúc điều trị, hoặc lỗi tra hồ sơ | Báo, không mở popup; mẫu không gắn hồ sơ HIS thì không kiểm |
| 10 | Chỉ có y lệnh hẹn giờ sau T | Báo "hẹn lúc …" |
| 11 | 1 hoặc nhiều mẫu cùng hồ sơ đã tới giờ | Mở lần lượt popup từng mẫu; từ mẫu thứ 2 đọc lại mẫu, đã lấy ở nơi khác thì báo, không mở |
| 12 | Đang có cửa sổ chặn (popup, hộp thoại) | Báo "đã nhận, n lượt chờ", xử lý khi cửa sổ đóng, TG lấy mẫu vẫn là T |
| 13 | Lưu thành công | Làm mới màn Lấy mẫu bệnh phẩm cùng phòng; người lấy mẫu đã lưu khác người gắn cổng thì ghi nhật ký |
| 14 | Ngắt cổng, đóng tab, mất cổng | Đóng cổng, đăng xuất, báo, ghi nhật ký; lượt quét đang chờ của cổng đó báo "bỏ lượt quét" |

### Điều kiện nghiệp vụ
- Phiên gắn chỉ nằm trong bộ nhớ máy trạm (`TestDeviceSampleDataWorker.TestDeviceSamples`, dùng chung với màn DS đăng nhập); khởi động lại HIS phải kết nối và quét thẻ lại.
- Tìm mẫu theo bộ lọc phòng giống ô Barcode (F4) của màn Lấy mẫu bệnh phẩm, trạng thái Chưa lấy mẫu/Từ chối, thời gian barcode từ 00:00 của (ngày quét − 6 ngày).
- Không đổi các thao tác lấy mẫu bằng tay; popup luôn mở (không lưu thẳng).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| ACS_USER | Table (ACS) | Tra tài khoản của thẻ (`api/AcsUser/Get`), thêm vào cache để popup chọn sẵn |
| V_LIS_SAMPLE | View (LIS) | Tìm mẫu chờ lấy, đọc lại mẫu sau khi lưu |
| HIS_TREATMENT | Table | Kiểm hồ sơ đã kết thúc điều trị (`IS_PAUSE`) |
| V_HIS_ROOM, V_HIS_EXECUTE_ROOM | View (cache) | Phòng đang làm việc, bộ lọc phòng |
| TestDeviceSampleADO | ADO (HIS.Desktop.LocalStorage.BackendData) | Phiên gắn cổng: TeminalCode = tên cổng, Loginname, Username, SessionTimeStart |

## 4. UI Layout

```
+--------------------------------------------------------------------------------------------+
| 1. Chọn cổng COM ... Kết nối   2. Quét thẻ nhân viên ...   3. Quét barcode mẫu ...           |
| Cổng: [COM3 v] [Kết nối (Ctrl K)] [Ngắt kết nối (Ctrl D)] [Làm mới (Ctrl R)]  Đang chờ: n  [DS đăng nhập] |
+--------------------------------------------------------------------------------------------+
| Máy quét đang kết nối                                                                      |
| [COM3  Nguyễn Văn A | a.nv – Đăng nhập lúc 07:30 | Gần nhất ...]  [COM4  Chờ quét thẻ ...]     |
+------------------------------ (splitter) --------------------------------------------------+
| Nhật ký quét: Thời gian | Cổng | Mã quét | Kết quả (lỗi chữ đỏ)                              |
+--------------------------------------------------------------------------------------------+
```
- Thông báo nổi (AlertControl riêng của màn, góc dưới phải, 6 giây, xếp chồng, xanh = thành công, đỏ = cảnh báo) kèm âm báo; thấy được ở mọi tab.
- File: `UCTestConnectDeviceSample.cs` (nút, phím tắt, timer 2 giây kiểm cổng, đóng tab), `___Port.cs` (mở, đóng, mất cổng), `___Scan.cs` (hàng đợi, quy tắc quét, mở popup), `___View.cs` (ô, nhật ký, thông báo, định dạng).

## 5. API Endpoints

| Action | URI | Consumer | Filter/Body |
|--------|-----|----------|-------------|
| Tra tài khoản theo thẻ | api/AcsUser/Get | AcsConsumer | AcsUserFilter.LOGINNAME |
| Tìm mẫu | api/LisSample/GetView | LisConsumer | LisSampleViewFilter (BARCODE__EXACT hoặc SERVICE_REQ_CODE__EXACT, phòng, SAMPLE_STT_IDs, BARCODE_TIME_FROM; ID khi đọc lại) |
| Kiểm hồ sơ | api/HisTreatment/Get | MosConsumer | TREATMENT_CODE__EXACT |
| Nhật ký | /api/SdaEventLog/Create | SdaConsumer | SdaEventLogSDO (đăng nhập, đăng xuất kèm lý do, chặn thẻ, đổi người lấy mẫu trên popup) |

## 6. Dependencies

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| LIS.Desktop.Plugins.SampleInfo | Quét barcode mẫu hợp lệ | `V_LIS_SAMPLE` (bản sao, SAMPLE_LOGINNAME/USERNAME = tài khoản của cổng), `DateTime` giờ quét; đọc kết quả qua `ShowDialog() == DialogResult.OK` |
| HIS.Desktop.Plugins.TestDeviceSample | Nút DS đăng nhập | (Module) — mở `Show(owner)` không chặn, giữ 1 cửa sổ |
| HIS.Desktop.Plugins.SampleCollectionRoom | Sau khi lưu | Gọi phản xạ `FillDataToGridControl()` của UC trong tab `HIS.Desktop.Plugins.SampleCollectionRoom_<RoomId>_<RoomTypeId>` |

### Cấu hình
- `PortConfig.BaudRate/DataBits/Parity/StopBits/Handshake` (appSettings, tùy chọn): mặc định 9600/8/None/One/None; bật DTR/RTS.
- Không đưa module vào `HIS.Desktop.FormClosingOption.ModuleLinkApply` (đóng tab chỉ ẩn thì cổng vẫn mở).
- Tài khoản đăng nhập máy cần quyền module `LIS.Desktop.Plugins.SampleInfo` và `HIS.Desktop.Plugins.TestDeviceSample`.

## 7. Print

Không in trực tiếp. Thẻ nhân viên in ở `HIS.Desktop.Plugins.EmpUser` (MPS000521). In barcode ống mẫu qua "Lưu in" của popup.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 24/07/2024 | ninhdd | Bản đầu: giao thức terminal riêng qua Inventec.Common.Rs232 |
| 21/10/2026 | dangth | **Việc 60557 (BV Yên Bái)** — Viết lại toàn bộ: đọc máy quét chế độ COM (`Worker\ComPortReader`, chống thoát HIS khi rút cáp), nhiều cổng, thẻ `TK-` + tài khoản, quy tắc quét theo thiết kế I.4, mở popup SampleInfo điền sẵn người/giờ lấy mẫu, làm mới màn Lấy mẫu bệnh phẩm, giao diện ô theo cổng + nhật ký quét + thông báo nổi, nút DS đăng nhập, phím tắt Kết nối đổi Ctrl C → Ctrl K, nhật ký SDA. Bỏ giao thức terminal cũ và tham chiếu Rs232. Thiết kế: `PTTK\60557 - Thiet ke - Quet barcode tai khoan nhan dien nguoi lay mau theo cong COM.md` |
| 23/10/2026 | dangth | **Việc 60557** — Sửa lỗi tại viện: mọi lượt quét kẹt mãi ở “sẽ xử lý khi cửa sổ đang mở được đóng”. HIS mở frmMain bằng `ShowDialog` nên `Form.Modal` của frmMain luôn đúng; `IsModalWindowOpen` bỏ duyệt `Application.OpenForms`, chỉ xét cửa sổ chứa màn có bị Windows khóa không (`TopLevelControl` + `IsWindowEnabled`) |

## 9. Test Cases

- [ ] Kết nối 2 cổng (Ctrl K): 2 ô vàng; DS đăng nhập mở không chặn, chưa có dòng
- [ ] Quét mẫu trên cổng chưa gắn: cảnh báo đỏ, mẫu không đổi
- [ ] Quét thẻ khóa / không tồn tại / lỗi tra cứu: báo, không gắn
- [ ] Quét thẻ A, B trên 2 cổng: 2 ô xanh, DS đăng nhập tự hiện 2 dòng; quét thẻ người khác trên cổng đang gắn: bị chặn
- [ ] Quét mẫu: popup tiêu đề COMx – họ tên, người lấy mẫu = tài khoản cổng, TG = lúc quét, barcode khóa; Lưu → popup đóng, màn Lấy mẫu làm mới
- [ ] Quét khi popup đang mở: "đã nhận, n lượt chờ", popup sau mở khi popup trước đóng, TG = lúc quét
- [ ] Không có mẫu / mẫu đã lấy / trùng 2 hồ sơ / y lệnh hẹn giờ / hồ sơ kết thúc / mã y lệnh 12 số
- [ ] Đăng xuất ở DS đăng nhập: dòng biến mất, ô chuyển vàng, quét thẻ mới không cần kết nối lại
- [ ] Rút cáp: HIS không thoát, ô đỏ, đăng xuất, ghi nhật ký; đóng tab: cổng đóng, phiên mất
