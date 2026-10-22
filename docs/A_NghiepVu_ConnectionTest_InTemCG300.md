# TÀI LIỆU NGHIỆP VỤ

## In tem barcode và nhả ống tự động qua máy lấy máu CG300

| Thông tin | Nội dung |
|---|---|
| Chức năng | Kết nối xét nghiệm (HIS.Desktop.Plugins.ConnectionTest) |
| Người dùng | Kỹ thuật viên phòng lấy mẫu xét nghiệm; quản trị hệ thống (khai báo cấu hình, danh mục) |
| Thiết bị | Hệ thống lấy máu tự động FOYEN CG300 — tự chọn ống đúng màu, dán tem barcode, nhả ống |
| Danh mục liên quan | Loại mẫu bệnh phẩm; Dịch vụ – Loại mẫu bệnh phẩm (HIS.Desktop.Plugins.HisServiceTestSample) |
| Tài liệu đi kèm | B_KyThuat_ConnectionTest_InBarcodeCG300.md (kỹ thuật); SQL_ConnectionTest_CG300_SdaConfigApp.sql (khai báo cấu hình) |
| Trạng thái | **Đã triển khai**; còn 7 điểm chờ chốt tại viện — xem PHẦN 6 |

Dành cho khách hàng, người phân tích nghiệp vụ, kiểm thử viên. Phần lập trình nằm ở tài liệu B.

---

# PHẦN 1. VẤN ĐỀ VÀ MỤC TIÊU

## 1.1 Vấn đề

Tại phòng lấy mẫu, mỗi người bệnh thường cần nhiều ống máu khác màu (tím, đỏ, đen, xanh…) tùy theo các xét nghiệm được chỉ định. Hiện nay kỹ thuật viên phải tự làm toàn bộ:

| Việc kỹ thuật viên đang làm | Hệ quả |
|---|---|
| Dùng chuột bấm vào ô tìm kiếm, quét mã, bấm nút Tìm | Tay phải rời ống/kim để thao tác máy tính, chậm khi đông người bệnh |
| Tự chọn ống đúng màu cho từng nhóm xét nghiệm | Dễ lấy nhầm loại ống, phải lấy máu lại |
| In tem, bóc tem, tự dán lên từng ống | Tốn thời gian; dán nhầm tem giữa hai người bệnh là sai sót nghiêm trọng |

Bệnh viện đã trang bị máy CG300, có khả năng tự chọn ống theo màu, dán tem và nhả ống. Máy cần phần mềm HIS gửi sang thông tin mẫu và **mã loại ống** của từng xét nghiệm; HIS chưa có khả năng này.

## 1.2 Mục tiêu

Kỹ thuật viên **chỉ dùng máy quét barcode**, không cần chuột hay bàn phím:

| Lần quét | Hệ thống làm gì |
|---|---|
| Lần 1 — quét mã của người bệnh A | Tự tìm và hiển thị các mẫu xét nghiệm của A |
| Lần 2 — quét **lại** mã A | Gửi dữ liệu sang CG300 → máy dán tem lên đúng các ống cần dùng và nhả ống |
| Lần 3 — quét mã người bệnh B | Tự xóa mã cũ, tìm và hiển thị các mẫu của B |
| Lần 4 — quét **lại** mã B | Máy dán tem và nhả ống cho B. Tiếp tục lặp lại với người bệnh kế tiếp |

Chỉ những máy trạm **có kết nối CG300** mới thấy và dùng được tính năng này; các máy trạm khác không có thay đổi gì.

## 1.3 Phạm vi

| Có làm | Không làm |
|---|---|
| Tùy chọn "In tem CG300" trên màn Kết nối xét nghiệm, chỉ hiện khi máy trạm kết nối được CG300 | Thay đổi các chức năng khác của màn Kết nối xét nghiệm (nhập, trả, duyệt kết quả, in phiếu kết quả) |
| Quét mã để tìm, quét lại cùng mã để gửi sang CG300 | Thay đổi cách in tem bằng máy in thường (Bartender, mẫu in barcode hiện có) |
| Xác định ống cần dùng theo danh mục Loại mẫu bệnh phẩm gán cho dịch vụ | Cấu hình bảng khay/màu ống trên máy CG300 (do kỹ sư hãng thực hiện) |
| Tự cập nhật trạng thái **Đã lấy mẫu** sau khi gửi thành công | Nhận phản hồi từ máy CG300 về việc đã nhả ống hay chưa (máy không hỗ trợ ở phương thức đang dùng) |
| Cảnh báo dịch vụ chưa gán loại mẫu, mẫu chưa có barcode | Tự sinh barcode cho mẫu chưa có barcode |
| Hỏi xác nhận khi in lại tem cho mẫu đã in trong ca | Đếm/giới hạn số lần in lại |

## 1.4 Cách HIS kết nối với máy CG300

Hãng cung cấp hai phương thức kết nối; dự án dùng **phương thức 2 — trao đổi qua file**:

- Phần mềm điều khiển CG300 và HIS cài **trên cùng một máy trạm** tại phòng lấy mẫu.
- Khi kỹ thuật viên quét lại mã, HIS ghi một file dữ liệu vào **một thư mục dùng chung** (thư mục `ALL`).
- Phần mềm CG300 đọc file, dán tem, nhả ống, rồi **tự xóa file**.
- Dữ liệu chỉ đi **một chiều** từ HIS sang máy. HIS chỉ biết đã gửi thành công, **không biết** máy đã nhả ống hay chưa. Nếu phần mềm CG300 đang tắt, file sẽ nằm chờ trong thư mục.

Phương thức này không đòi hỏi mở cổng dịch vụ cho thiết bị bên ngoài và không phải sửa phần xử lý trên máy chủ.

---

# PHẦN 2. THAY ĐỔI TRÊN MÀN HÌNH

Chỉ có **một thay đổi** trên màn Kết nối xét nghiệm: ô tích **"In tem CG300"** đặt ngay bên phải ô tìm kiếm.

```
Máy trạm có kết nối CG300:
[ Ô tìm kiếm ............ ] [☑ In tem CG300] [ Tìm (Ctrl F) ]

Máy trạm không có / mất kết nối CG300:
[ Ô tìm kiếm ................................ ] [ Tìm (Ctrl F) ]
```

## 2.1 Ô tích "In tem CG300"

| Trường hợp | Hiển thị |
|---|---|
| Máy trạm chưa được khai báo dùng CG300 | Không có ô tích (giống hiện tại) |
| Đã khai báo nhưng chưa kết nối được (QT-02) | Ô tích bị ẩn |
| Kết nối được | Ô tích hiện. Trạng thái tích/bỏ tích **được nhớ** cho lần mở sau trên máy đó |
| Đang tích | Ô tìm kiếm hiển thị chữ gợi ý **"Quét barcode..."** |

Di chuột lên ô tích hiện hướng dẫn: *"In tem qua máy CG300: quét mã để tìm, quét lại cùng mã để in tem và nhả ống"*.

## 2.2 Ô tìm kiếm khi đang tích "In tem CG300"

- Máy quét gửi mã kèm phím Enter, hệ thống tự xử lý — không cần bấm nút Tìm.
- Sau mỗi lần quét (tìm hay in, kể cả khi có lỗi), con trỏ **luôn quay về ô tìm kiếm** và nội dung cũ được **bôi đen**. Lần quét tiếp theo sẽ ghi đè mã cũ.
- Các ô lọc khác (mã điều trị, mã y lệnh, mã bệnh nhân, khoảng thời gian…) giữ nguyên cách hoạt động.

## 2.3 Thông báo mới

| Khi nào | Nội dung |
|---|---|
| Quét in nhưng máy vừa mất kết nối | Mất kết nối máy CG300: *(lý do)*. Vui lòng kiểm tra phần mềm CG và thư mục trao đổi. |
| Có mẫu chưa có barcode | Các mẫu chưa có barcode, không gửi sang CG300 (mã y lệnh): … |
| Có dịch vụ chưa gán loại mẫu bệnh phẩm | Dịch vụ chưa được gán loại mẫu bệnh phẩm (không xác định được ống): … |
| Không còn gì hợp lệ để gửi | Không có dữ liệu hợp lệ để gửi sang CG300. |
| Không ghi được file | Không ghi được dữ liệu sang máy CG300 vào thư mục …. Vui lòng kiểm tra cấu hình/quyền thư mục. |
| Quét lại mã đã in trong ca | Barcode … đã được in tem qua CG300. Bạn có muốn in lại (máy sẽ nhả thêm ống) không? |

Lý do mất kết nối hiển thị một trong ba dạng: *Thư mục … không tồn tại*; *Không có quyền ghi thư mục …*; *Phần mềm điều khiển CG300 (…) chưa chạy*.

---

# PHẦN 3. QUY TẮC NGHIỆP VỤ

Tài liệu kỹ thuật tham chiếu lại các mã QT-xx dưới đây.

## QT-01 — Khai báo máy trạm dùng CG300

Hai cấu hình phần mềm, đặt **theo tài khoản đăng nhập** tại "Thiết lập cấu hình phần mềm":

| Cấu hình | Ý nghĩa | Để trống |
|---|---|---|
| `CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_FOLDER` | Đường dẫn thư mục `ALL` mà phần mềm CG300 đọc (VD `D:\CG300\ALL`) | Không dùng CG300 — không có ô tích |
| `CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_PROCESS_NAME` | Tên chương trình điều khiển CG300 (không có `.exe`) | Không kiểm tra chương trình có đang chạy hay không |

Vì cấu hình đi theo tài khoản, **nên khai báo cả tên chương trình** để máy trạm khác (không có CG300) không hiện nhầm ô tích khi tài khoản đó đăng nhập ở nơi khác.

## QT-02 — Thế nào là "kết nối được CG300"

Máy CG300 không báo trạng thái về HIS, nên HIS kiểm tra **ngay trên máy trạm**. Phải đạt **đủ 3 điều kiện**:

1. Đã khai báo đường dẫn thư mục (QT-01).
2. Thư mục tồn tại và HIS ghi được file vào đó.
3. Chương trình điều khiển CG300 đang chạy (chỉ kiểm tra khi có khai báo tên chương trình).

Thời điểm kiểm tra: khi **mở màn hình**, khi **bấm nút Tìm (Ctrl F)**, và **ngay trước mỗi lần gửi dữ liệu**. Không kiểm tra ngầm liên tục.
Mất kết nối khi đang làm việc → báo lỗi, ẩn ô tích. Khắc phục xong (bật phần mềm CG300…) thì bấm **Tìm** để ô tích hiện lại.

## QT-03 — Phân biệt quét để tìm và quét để in

Chỉ áp dụng khi ô tích **đang hiện và đang được tích**.

| Mã vừa quét | Điều kiện | Kết quả |
|---|---|---|
| **Khác** mã quét lần trước | — | **Tìm** mẫu theo mã này |
| **Trùng** mã quét lần trước | Lưới đang có mẫu | **Gửi sang CG300** các mẫu đang hiển thị |
| **Trùng** mã quét lần trước | Lưới không có mẫu | Tìm lại |

Không dùng kiểu "lần lẻ tìm, lần chẵn in": nếu kỹ thuật viên lỡ quét nhầm sang người bệnh khác ở lần thứ hai, kiểu lẻ/chẵn sẽ in tem của người bệnh cũ lên ống của người mới. So khớp mã đảm bảo **chỉ in khi đúng người bệnh đang hiển thị**.

Mã quét có thể là barcode mẫu, mã y lệnh, mã bệnh nhân… — bất kỳ mã nào ô tìm kiếm đang hỗ trợ.

## QT-04 — Xác định ống cần dùng (mã ống)

- Mỗi **Loại mẫu bệnh phẩm** trong danh mục tương ứng **một loại ống** trên máy CG300. **Mã loại mẫu phải trùng mã ống** đã cấu hình trên máy (001, 004, 006…).
- Mỗi dịch vụ xét nghiệm được gán loại mẫu tại chức năng **"Dịch vụ – Loại mẫu bệnh phẩm"**.
- Một mẫu (một barcode) có thể cần **nhiều ống**: các dịch vụ dùng chung một loại ống được **gộp vào một ống**; mỗi loại ống khác nhau là một ống riêng.
- Dịch vụ được gán **nhiều loại mẫu** → gửi đủ các ống tương ứng.

Ví dụ: barcode 1121034 có Tổng phân tích tế bào máu (ống tím 001), Glucose và Creatinin (ống đen 006) → máy nhả **2 ống**: ống tím dán tem ghi "Tổng phân tích tế bào máu", ống đen ghi "Glucose; Creatinin".

## QT-05 — Màu ống và nguồn bệnh phẩm

Danh mục Loại mẫu bệnh phẩm chỉ có Mã và Tên. Đặt tên theo dạng **"Màu - Nguồn bệnh phẩm"**, ví dụ `Purple - EDTA`, `Black - Heparin`:

- Phần **trước** dấu ` - ` là **màu ống** gửi sang máy.
- Phần **sau** dấu ` - ` là **nguồn bệnh phẩm**, được **in trên tem**.
- Tên không có dấu ` - ` thì cả hai lấy nguyên tên.

## QT-06 — Thông tin in trên tem

Theo yêu cầu bệnh viện, tem hiển thị:

| Trên tem | Lấy từ |
|---|---|
| Barcode | Barcode của mẫu |
| Mã bệnh nhân | Mã bệnh nhân |
| Họ tên | Họ tên người bệnh |
| Giới tính | Giới tính |
| Tuổi | Tính từ ngày sinh |
| Nguồn bệnh phẩm | Theo QT-05 |
| Khoa | Khoa chỉ định |
| Thời gian in | Thời điểm gửi sang máy |

Ngoài ra HIS còn gửi kèm (máy dùng để xử lý, không in): mã ống, màu ống, tên các xét nghiệm, mã y lệnh, phòng chỉ định, thời gian chỉ định, đối tượng, mức ưu tiên (**Cấp cứu** nếu mẫu được đánh dấu cấp cứu), tài khoản và địa chỉ máy in tem.

## QT-07 — Mẫu chưa có barcode

Mẫu chưa có barcode **không được gửi**, hệ thống cảnh báo kèm mã y lệnh. Các mẫu khác có barcode vẫn gửi bình thường. Hệ thống **không tự sinh barcode** ở bước này.

## QT-08 — Dịch vụ chưa gán loại mẫu bệnh phẩm

Dịch vụ chưa gán loại mẫu thì **không xác định được ống**, nên không gửi phần đó; hệ thống cảnh báo tên dịch vụ để khoa bổ sung danh mục. Các dịch vụ khác của cùng mẫu vẫn được gửi.

## QT-09 — Cập nhật trạng thái lấy mẫu

Sau khi gửi sang CG300 **thành công**, mẫu đang ở trạng thái **Chưa lấy mẫu** hoặc **Từ chối** được chuyển sang **Đã lấy mẫu**, giống như khi in tem bằng máy in Bartender hiện nay.
Ngoại lệ: bệnh viện bật cấu hình `LIS.LIS_SAMPLE.ALLOW_AUTO_SAMPLE_AFTER_PRINT_BARCODE = 1` thì giữ nguyên trạng thái.
Gửi **không thành công** (không ghi được file, mất kết nối) thì **không** đổi trạng thái.

Danh sách mẫu **không tải lại** sau khi in, chỉ cập nhật trạng thái trên dòng — kết quả đang hiển thị không bị mất.

## QT-10 — In lại tem

Quét lại một mã **đã gửi sang CG300 trong lúc đang mở màn hình** → hỏi xác nhận, vì máy sẽ **nhả thêm ống**. Đồng ý thì gửi lại và đánh dấu là in lại.
Đóng và mở lại màn hình thì danh sách "đã gửi" bắt đầu lại từ đầu.

## QT-11 — Không chặn toàn bộ vì một phần lỗi

Một lần quét có thể có cả phần hợp lệ lẫn phần không hợp lệ (QT-07, QT-08). Hệ thống **gửi phần hợp lệ** rồi hiển thị **một thông báo tổng hợp** các phần bị bỏ qua. Chỉ khi không còn gì hợp lệ mới báo "Không có dữ liệu hợp lệ để gửi sang CG300".

## QT-12 — Khi không dùng CG300

Máy trạm chưa khai báo (QT-01), chưa kết nối được (QT-02), hoặc kỹ thuật viên **bỏ tích** "In tem CG300": màn hình hoạt động **y hệt hiện tại** — Enter ở ô tìm kiếm chỉ để tìm, các nút in barcode cũ giữ nguyên.

---

# PHẦN 4. TÌNH HUỐNG SỬ DỤNG

| # | Tình huống | Kết quả mong đợi |
|---|---|---|
| 0 | Máy trạm **không** khai báo CG300, quét mã rồi Enter | Tìm như hiện tại; không có ô tích (QT-12) |
| 1 | Có CG300, đã tích. Quét mã người bệnh A | Hiện các mẫu của A, con trỏ ở ô tìm kiếm, mã bôi đen (QT-03) |
| 2 | Quét lại mã A | Máy dán tem và nhả đúng số ống của A; mẫu chuyển Đã lấy mẫu; danh sách không bị tải lại (QT-04, QT-09) |
| 3 | Quét mã B ngay sau bước 2 | Mã B ghi đè, tìm B — **không** in thêm cho A (QT-03) |
| 4 | Ở bước 1, lỡ quét mã A rồi quét nhầm sang mã C | Lần quét C là **tìm C**, không in tem A (QT-03) |
| 5 | Mẫu của A có 3 xét nghiệm thuộc 2 loại ống | Nhả 2 ống, mỗi tem ghi đúng xét nghiệm của ống đó (QT-04) |
| 6 | Một xét nghiệm của A chưa gán loại mẫu | Vẫn nhả các ống còn lại; thông báo tên xét nghiệm chưa gán (QT-08, QT-11) |
| 7 | Mẫu của A chưa có barcode | Không gửi mẫu đó, thông báo kèm mã y lệnh (QT-07) |
| 8 | Quét mã A lần thứ ba sau khi đã in | Hỏi "đã in tem, có muốn in lại" — Không thì dừng; Có thì nhả thêm ống (QT-10) |
| 9 | Phần mềm CG300 bị tắt giữa ca, quét lại để in | Báo "Mất kết nối… Phần mềm điều khiển CG300 chưa chạy", ô tích ẩn, mẫu giữ trạng thái cũ (QT-02, QT-09) |
| 10 | Bật lại phần mềm CG300, bấm Tìm | Ô tích hiện lại, trạng thái tích như trước (QT-02) |
| 11 | Kỹ thuật viên bỏ tích "In tem CG300" | Quét mã chỉ để tìm như cũ (QT-12) |
| 12 | Tài khoản có khai báo CG300 đăng nhập ở máy trạm khác không có CG300 | Không hiện ô tích, vì thư mục/chương trình không có trên máy đó (QT-01, QT-02) |
| 13 | Mẫu cấp cứu | Bản ghi gửi sang máy có mức ưu tiên Cấp cứu (QT-06) |

---

# PHẦN 5. TEST CASE

## Nhóm A — Không dùng CG300, chức năng cũ không đổi (QT-12)

- [ ] A1 — Tài khoản chưa khai báo thư mục CG300: không có ô tích, Enter ở ô tìm kiếm chỉ tìm
- [ ] A2 — Có ô tích nhưng bỏ tích: quét mã chỉ tìm, không sinh file
- [ ] A3 — In barcode Bartender và mẫu in barcode cũ vẫn cập nhật trạng thái lấy mẫu như trước
- [ ] A4 — Các ô lọc mã điều trị, mã y lệnh, mã bệnh nhân hoạt động như cũ

## Nhóm B — Kết nối (QT-01, QT-02)

- [ ] B1 — Thư mục khai báo không tồn tại: ô tích ẩn
- [ ] B2 — Thư mục không có quyền ghi: ô tích ẩn
- [ ] B3 — Khai báo tên chương trình nhưng chương trình chưa chạy: ô tích ẩn
- [ ] B4 — Đủ 3 điều kiện: ô tích hiện; tích, đóng màn hình, mở lại → vẫn tích
- [ ] B5 — Đang làm việc thì tắt phần mềm CG300, quét in: báo mất kết nối kèm lý do, ô tích ẩn
- [ ] B6 — Bật lại phần mềm CG300, bấm Tìm: ô tích hiện lại

## Nhóm C — Luồng quét (QT-03)

- [ ] C1 — Quét A: hiện mẫu A, con trỏ ở ô tìm kiếm, mã bôi đen
- [ ] C2 — Quét lại A: gửi sang CG300, máy nhả ống
- [ ] C3 — Quét B sau C2: tìm B, không gửi A
- [ ] C4 — Quét A rồi quét C (khác): tìm C, không gửi
- [ ] C5 — Quét lại mã khi lưới không có mẫu: tìm lại, không gửi
- [ ] C6 — Quét bằng mã y lệnh thay cho barcode: luồng tìm/in vẫn đúng
- [ ] C7 — Quét liên tục rất nhanh: không gửi trùng, không treo màn hình

## Nhóm D — Dữ liệu gửi sang máy (QT-04, QT-05, QT-06)

- [ ] D1 — Mẫu 1 xét nghiệm: nhả 1 ống đúng màu
- [ ] D2 — Mẫu nhiều xét nghiệm cùng loại ống: 1 ống, tem ghi đủ tên xét nghiệm
- [ ] D3 — Mẫu nhiều xét nghiệm khác loại ống: đủ số ống, mỗi tem đúng xét nghiệm của ống đó
- [ ] D4 — Tem in đúng: barcode, mã BN, họ tên có dấu, giới tính, tuổi, nguồn bệnh phẩm, khoa, thời gian in
- [ ] D5 — Tên loại mẫu "Purple - EDTA": máy nhận màu Purple, tem in EDTA
- [ ] D6 — Tên người bệnh có dấu nháy, ký tự đặc biệt: máy vẫn đọc được
- [ ] D7 — Mẫu cấp cứu: ưu tiên Cấp cứu

## Nhóm E — Trạng thái và ngoại lệ (QT-07 → QT-11)

- [ ] E1 — Mẫu Chưa lấy mẫu → sau khi gửi thành Đã lấy mẫu, danh sách không tải lại
- [ ] E2 — Cấu hình `ALLOW_AUTO_SAMPLE_AFTER_PRINT_BARCODE = 1`: giữ nguyên trạng thái
- [ ] E3 — Mẫu chưa có barcode: cảnh báo, mẫu khác vẫn gửi
- [ ] E4 — Dịch vụ chưa gán loại mẫu: cảnh báo, các ống khác vẫn gửi
- [ ] E5 — Mọi dịch vụ đều chưa gán: báo "Không có dữ liệu hợp lệ", không đổi trạng thái
- [ ] E6 — Quét lại mã đã in: hỏi xác nhận; Không → không gửi; Có → gửi lại, đánh dấu in lại
- [ ] E7 — Thư mục bị mất quyền ghi đúng lúc gửi: báo lỗi, trạng thái mẫu không đổi

## Nhóm F — Giao diện

- [ ] F1 — Ô tích không làm vỡ bố cục hàng tìm kiếm ở độ phân giải 1366×768
- [ ] F2 — Chữ gợi ý "Quét barcode..." hiện khi tích, trả về chữ cũ khi bỏ tích
- [ ] F3 — Nội dung thông báo hiển thị đúng tiếng Việt có dấu; giao diện tiếng Anh hiển thị đúng bản dịch

---

# PHẦN 6. ĐIỂM CẦN CHỐT

Bảy điểm dưới đây đã được triển khai theo đề xuất mặc định để không chặn tiến độ. Cần bệnh viện và kỹ sư hãng xác nhận trong lúc lắp đặt.

| # | Nội dung | Đề xuất (đang chạy) | Trạng thái |
|---|---|---|---|
| 1 | Quét lại mã đã in trong ca | Hỏi xác nhận rồi in lại, máy nhả thêm ống (QT-10) | **Chờ chốt** |
| 2 | Màu ống lấy từ đâu | Đặt tên loại mẫu dạng "Màu - Nguồn"; hoặc hãng xác nhận máy chỉ cần mã ống (QT-05) | **Chờ chốt** |
| 3 | Dịch vụ gán nhiều loại mẫu | Gửi tất cả các ống được gán (QT-04) | **Chờ chốt** |
| 4 | Mẫu chưa có barcode | Không tự sinh, chỉ cảnh báo (QT-07) | **Chờ chốt** |
| 5 | Định dạng file máy đọc | File chữ `.txt`, mã hóa UTF-8 — cần hãng xác nhận | **Chờ chốt** |
| 6 | Mã in ở dòng "mã bệnh nhân" trên tem | Mã bệnh nhân. Ảnh tem thực tế tại viện đang in dạng `260513.1161` — cần xác nhận đó là mã gì | **Chờ chốt** |
| 7 | Bảng mã ống trên máy | Ảnh thực tế có ống xanh lá mã `002`, chưa có trong bảng mã ống bệnh viện gửi — cần đối chiếu cấu hình máy | **Chờ chốt** |

Ngoài ra cần lấy tại viện: **tên chương trình điều khiển CG300** và **đường dẫn thư mục ALL** để khai báo cấu hình (QT-01).

Vì tính năng chỉ bật trên máy trạm được khai báo và kết nối được CG300, các máy trạm và bệnh viện khác không bị ảnh hưởng.
