# Việc 52540 — TÀI LIỆU NGHIỆP VỤ

## Kiểm tra tương tác thuốc giữa các đơn khác nhau trong cùng hồ sơ bằng MIMS

| Thông tin | Nội dung |
|---|---|
| Mã việc | 52540 |
| Chức năng | Kê đơn thuốc / chỉ định thuốc — Phòng khám, CLS, Thận nhân tạo, Y học cổ truyền |
| Người dùng | Bác sĩ kê đơn, bác sĩ điều trị, dược sĩ lâm sàng, phòng Dược, phòng CNTT |
| Hệ thống ngoài | MIMS (cơ sở dữ liệu tra cứu tương tác thuốc) |
| Tài liệu đi kèm | 52540_B_KyThuat_KiemTraTuongTacThuocGiuaCacDon_MIMS.docx (dành cho lập trình viên) |
| Trạng thái | Chờ khách hàng chốt 8 điểm ở Phần 12 |

**Tài liệu này dành cho**: khách hàng, người phân tích nghiệp vụ, dược sĩ lâm sàng, kiểm thử viên.
Tài liệu **không** chứa nội dung lập trình.

---

# PHẦN 1. BỐI CẢNH VÀ MỤC TIÊU

## 1.1 Vấn đề hiện tại

Phần mềm đã kết nối MIMS để kiểm tra tương tác thuốc khi bác sĩ lưu đơn. Nhưng phạm vi kiểm tra **chỉ nằm trong một đơn** — đúng những thuốc đang hiện trên màn hình kê đơn tại thời điểm đó.

Trong khi đó, một hồ sơ điều trị thực tế gần như luôn có **nhiều đơn thuốc**:

| Tình huống thực tế | Vì sao sinh ra nhiều đơn |
|---|---|
| Nội trú | Mỗi ngày một y lệnh; kê nhiều ngày; thuốc tủ trực; thuốc dự trù |
| Chuyển khoa | Bác sĩ khoa mới kê đơn mới, đơn khoa cũ vẫn còn hiệu lực |
| Hội chẩn / bác sĩ chuyên khoa | Nhiều bác sĩ cùng kê cho một bệnh nhân |
| Ngoại trú tái khám | Đơn lần khám trước còn thuốc dùng 10–30 ngày |
| Đơn BHYT + đơn ngoài danh mục | Cùng lần khám nhưng tách thành hai đơn |
| Thận nhân tạo, Y học cổ truyền, CLS | Kê ở màn hình riêng, sinh đơn riêng |

Hệ quả: hai thuốc tương tác nghiêm trọng với nhau, nếu **nằm ở hai đơn khác nhau** thì phần mềm **không cảnh báo**, dù bệnh nhân đang dùng đồng thời cả hai.

Đây là điểm mù đúng nghĩa: càng bệnh nhân nặng, càng nhiều bác sĩ tham gia, càng nhiều đơn — thì nguy cơ tương tác càng cao mà khả năng phát hiện của phần mềm lại càng thấp.

## 1.2 Mục tiêu

| Mã | Mục tiêu |
|---|---|
| MT-01 | Khi bác sĩ lưu đơn, MIMS kiểm tra tương tác giữa thuốc **đang kê** và thuốc **các đơn khác còn hiệu lực** trong cùng hồ sơ |
| MT-02 | Cảnh báo chỉ hiện khi có liên quan tới ít nhất một thuốc bác sĩ **đang kê** — không làm bác sĩ phải xử lý cảnh báo giữa hai thuốc mà mình không kê |
| MT-03 | Cửa sổ cảnh báo chỉ rõ thuốc nào thuộc đơn khác, của đơn nào, dùng đến ngày nào — để bác sĩ biết phải xử lý ở đâu |
| MT-04 | Bệnh viện bật/tắt và chọn phạm vi kiểm tra được; mặc định giữ nguyên như hiện tại |
| MT-05 | Ghi nhận đầy đủ vào sổ theo dõi cảnh báo tương tác, phục vụ bình đơn thuốc và báo cáo dược lâm sàng |

## 1.3 Phạm vi

| | Nội dung |
|---|---|
| **Có làm** | Kiểm tra tương tác giữa thuốc đang kê và thuốc các đơn khác còn hiệu lực trong hồ sơ |
| **Có làm** | Hiển thị rõ nguồn gốc thuốc đơn khác trong cửa sổ cảnh báo |
| **Có làm** | Cấu hình phạm vi: chỉ đơn hiện tại / cùng hồ sơ điều trị / toàn lịch sử bệnh nhân |
| **Có làm** | Áp dụng cho cả chức năng chuột phải "Đánh giá thông tin thuốc" |
| **Có làm** | Áp dụng cho 4 màn hình kê đơn: Phòng khám, CLS, Thận nhân tạo, Y học cổ truyền |
| **Không làm** | Kiểm tra thuốc bệnh nhân tự mua ngoài, không có trong hệ thống |
| **Không làm** | Kiểm tra tương tác tại màn hình dược sĩ duyệt đơn (xem gợi ý mở rộng ở Phần 13.3) |
| **Không làm** | Kiểm tra tương tác thuốc với thực phẩm, với xét nghiệm |
| **Không làm** | Thay đổi cách kiểm tra tương tác trong phạm vi một đơn đang có |
| **Không làm** | Thay đổi giao diện màn hình kê đơn (chỉ thay đổi nội dung cửa sổ cảnh báo) |
| **Không làm** | Chuẩn hoá lại danh mục thuốc — ánh xạ thuốc sang MIMS (xem điều kiện tiên quyết 1.5) |

## 1.4 Người dùng và mức ảnh hưởng

| Người dùng | Ảnh hưởng |
|---|---|
| Bác sĩ kê đơn | Thấy nhiều cảnh báo hơn trước; cần biết cách đọc phần "thuốc từ đơn khác" |
| Bác sĩ điều trị nội trú | Ảnh hưởng nhiều nhất — hồ sơ nội trú có nhiều đơn nhất |
| Dược sĩ lâm sàng | Có thêm dữ liệu để bình đơn thuốc và theo dõi tương tác toàn hồ sơ |
| Phòng Dược | Phải rà soát ánh xạ danh mục thuốc sang MIMS (điều kiện tiên quyết) |
| Phòng CNTT | Bật cấu hình theo giai đoạn, theo dõi số lượng cảnh báo |

## 1.5 Điều kiện tiên quyết — ánh xạ danh mục thuốc sang MIMS

Phần mềm chỉ kiểm tra được tương tác của thuốc đã được **ánh xạ sang MIMS**, thông qua mã ATC hoặc hoạt chất của thuốc trong danh mục.

| Trạng thái thuốc trong danh mục | Kết quả kiểm tra |
|---|---|
| Có mã ATC đã ánh xạ MIMS | Kiểm tra được |
| Có hoạt chất đã ánh xạ MIMS | Kiểm tra được |
| Không có cả hai | **Bị bỏ qua âm thầm** — không kiểm tra, không thông báo |

Điều này đã đúng với chức năng hiện tại, nhưng việc 52540 làm nó **quan trọng hơn nhiều**: nếu thuốc của đơn cũ chưa được ánh xạ, hệ thống vẫn báo "không có tương tác" trong khi thực tế chưa kiểm tra được. Đó là cảnh báo an toàn giả.

**Đề nghị nghiệp vụ:** trước khi bật tính năng, phòng Dược rà soát và báo cáo **tỷ lệ thuốc trong danh mục đang dùng đã ánh xạ MIMS**. Khuyến nghị đạt tối thiểu 90% số thuốc phát sinh sử dụng thực tế trong 3 tháng gần nhất.

---

# PHẦN 2. HIỆN TRẠNG NGHIỆP VỤ

## 2.1 Ba thời điểm phần mềm hỏi MIMS

| Thời điểm | Cách kích hoạt | Phạm vi thuốc gửi đi |
|---|---|---|
| Khi lưu đơn | Tự động, bác sĩ bấm Lưu | Toàn bộ thuốc trong đơn đang kê |
| Chuột phải, chọn **1** thuốc → "Thông tin thuốc" | Bác sĩ chủ động | 1 thuốc đang chọn |
| Chuột phải, chọn **nhiều** thuốc → "Đánh giá thông tin thuốc" | Bác sĩ chủ động | Các thuốc đang chọn |

Ngoài ba thời điểm này, phần mềm **không** hỏi MIMS. Cụ thể: khi bác sĩ bổ sung từng thuốc vào đơn, chưa có kiểm tra tương tác — chỉ khi bấm Lưu.

## 2.2 MIMS đang kiểm tra những nhóm cảnh báo nào

| Nhóm cảnh báo | Ý nghĩa | Các mức MIMS trả về |
|---|---|---|
| Tương tác thuốc – thuốc | Hai thuốc dùng cùng nhau gây hại | Nghiêm trọng, Trung bình, Nhẹ, Lưu ý |
| Tương tác thuốc – bệnh | Thuốc chống chỉ định với bệnh của bệnh nhân (theo mã ICD) | Chống chỉ định, Rất cần thận trọng |
| Trùng hoạt chất | Hai thuốc chứa cùng hoạt chất → nguy cơ quá liều | Cùng hoạt chất, cùng nhóm phân tử mẹ, cùng nhóm phân tử gốc |
| Trùng nhóm điều trị | Hai thuốc cùng nhóm tác dụng → điều trị trùng lặp | Trùng toàn mã ATC, trùng đến cấp 4, trùng đến cấp 3 |
| Thai kỳ | Thuốc ảnh hưởng thai nhi | X, D, +, C, B, A |
| Cho con bú | Thuốc bài tiết qua sữa | Chống chỉ định, Nên tránh, Thận trọng |
| Chống chỉ định theo Việt Nam | Cặp chống chỉ định theo quy định trong nước | Có / Không |

Hai nhóm Thai kỳ và Cho con bú chỉ hoạt động khi bệnh nhân nữ được đánh dấu mang thai / cho con bú và bệnh viện đã bật cấu hình tương ứng (việc đã làm trước đó).

## 2.3 Nhóm nào bị ảnh hưởng bởi việc chỉ kiểm tra trong một đơn

| Nhóm cảnh báo | Cách hoạt động | Có bị bỏ sót khi thuốc ở đơn khác? |
|---|---|---|
| Tương tác thuốc – thuốc | So từng **cặp** thuốc | **CÓ — bỏ sót hoàn toàn** |
| Trùng hoạt chất | So từng **cặp** thuốc | **CÓ — bỏ sót hoàn toàn** |
| Trùng nhóm điều trị | So từng **cặp** thuốc | **CÓ — bỏ sót hoàn toàn** |
| Chống chỉ định theo Việt Nam | So từng **cặp** thuốc | **CÓ — bỏ sót hoàn toàn** |
| Tương tác thuốc – bệnh | Xét **từng thuốc** với bệnh | Không — thuốc đang kê vẫn được xét đủ |
| Thai kỳ / Cho con bú | Xét **từng thuốc** | Không — thuốc đang kê vẫn được xét đủ |

Bốn nhóm đầu là **nhóm so cặp** — đây chính là toàn bộ nội dung của việc 52540.

## 2.4 Cảnh báo về đơn cũ hiện đã có — và vì sao chưa đủ

Phần mềm hiện đã có một cảnh báo liên quan đơn cũ, khi bệnh viện bật cấu hình tương ứng:

> *"Bệnh nhân đã có đơn thuốc cũ còn sử dụng tới ngày dd/MM/yyyy. Bạn có muốn tiếp tục?"*

Cảnh báo này chỉ so **trùng tên thuốc**: bác sĩ kê lại đúng thuốc bệnh nhân đang còn dùng.

| | Cảnh báo đơn cũ hiện có | Việc 52540 |
|---|---|---|
| So sánh theo | Tên thuốc giống nhau | Tương tác dược lý theo dữ liệu MIMS |
| Phát hiện được | Kê lặp cùng một thuốc | Hai thuốc **khác tên, khác nhóm** nhưng tương tác với nhau |
| Phát hiện được | — | Trùng hoạt chất dù khác tên thương mại |
| Phát hiện được | — | Trùng nhóm điều trị |
| Thời điểm | Khi bổ sung từng thuốc | Khi lưu đơn |
| Phạm vi | Toàn lịch sử bệnh nhân | Theo cấu hình: hồ sơ điều trị hoặc toàn lịch sử |

Hai cảnh báo **bổ trợ nhau**, không thay thế nhau. Việc 52540 không bỏ cảnh báo cũ.

## 2.5 Mức độ phần mềm "biết" về đơn cũ — trước và sau

| Mức | Nội dung | Hiện tại | Sau 52540 |
|---|---|---|---|
| 1 | Biết bệnh nhân còn đơn thuốc cũ chưa dùng hết | Có | Có |
| 2 | Biết bác sĩ đang kê lại đúng thuốc cũ đó | Có | Có |
| 3 | Biết thuốc đang kê tương tác với thuốc đơn cũ | **Không** | **Có** |
| 4 | Biết thuốc đang kê trùng hoạt chất / trùng nhóm với thuốc đơn cũ | **Không** | **Có** |

---

# PHẦN 3. CÁC TÌNH HUỐNG ĐANG BỊ BỎ SÓT

## 3.1 Tình huống 1 — Nội trú, hai khoa cùng điều trị

```
Hồ sơ điều trị BN Nguyễn Văn A — nội trú khoa Tim mạch, hội chẩn khoa Cơ xương khớp

Đơn 1  05/09  Khoa Tim mạch      Warfarin           dùng đến 20/09
Đơn 2  07/09  Khoa Cơ xương khớp Diclofenac         ← bác sĩ đang kê hôm nay
```

Warfarin + Diclofenac là cặp tương tác **nghiêm trọng** (tăng nguy cơ xuất huyết).

- **Hiện tại:** bác sĩ khoa Cơ xương khớp bấm Lưu → không có cảnh báo nào, vì đơn của mình chỉ có một thuốc.
- **Sau 52540:** hiện cảnh báo nghiêm trọng, kèm dòng *"Warfarin — đơn ngày 05/09/2026, dùng đến 20/09/2026 (khoa Tim mạch)"*.

## 3.2 Tình huống 2 — Ngoại trú tái khám, đơn cũ còn hiệu lực

```
Hồ sơ ngoại trú BN Trần Thị B

Đơn 1  01/09  Phòng khám Nội      Clarithromycin 7 ngày   dùng đến 08/09
Đơn 2  04/09  Phòng khám Tim mạch Simvastatin             ← đang kê hôm nay
```

Cặp này làm tăng nguy cơ tiêu cơ vân. Bệnh nhân vẫn đang trong 7 ngày kháng sinh.

- **Hiện tại:** không cảnh báo.
- **Sau 52540:** cảnh báo, và bác sĩ có đủ thông tin để hoãn statin 4 ngày.

## 3.3 Tình huống 3 — Trùng hoạt chất, khác tên thương mại

```
Đơn 1  06/09  Panadol Extra (paracetamol + caffeine)   dùng đến 10/09
Đơn 2  07/09  Efferalgan (paracetamol)                 ← đang kê hôm nay
```

- **Hiện tại:** cảnh báo trùng tên thuốc **không** bắt được, vì hai tên khác nhau. MIMS cũng không bắt được vì hai thuốc ở hai đơn.
- **Sau 52540:** MIMS báo trùng hoạt chất mức cao nhất → phòng ngừa quá liều paracetamol, là một trong các nguyên nhân ngộ độc gan phổ biến nhất.

## 3.4 Tình huống 4 — Thuốc tủ trực và thuốc dự trù

Nội trú, y lệnh thuốc tủ trực buổi tối do điều dưỡng thực hiện, đơn dự trù cho ngày hôm sau — đều là các đơn riêng trong cùng hồ sơ, đều đang có hiệu lực.

- **Hiện tại:** mỗi đơn kiểm tra độc lập.
- **Sau 52540:** thuốc còn hiệu lực của các đơn này được đưa vào phép kiểm tra.

## 3.5 Tình huống 5 — Thang thuốc Y học cổ truyền và thuốc tân dược

Bệnh nhân dùng đồng thời thang thuốc YHCT (kê ở màn hình riêng) và thuốc tân dược. Nếu thành phần thang thuốc có ánh xạ MIMS thì tương tác sẽ được xét; nếu chưa ánh xạ thì không.

Đây là ví dụ trực tiếp cho điều kiện tiên quyết ở mục 1.5.

---

# PHẦN 4. YÊU CẦU NGHIỆP VỤ

| Mã | Yêu cầu | Mức |
|---|---|---|
| YC-01 | Khi lưu đơn, thuốc các đơn khác **còn hiệu lực** trong hồ sơ phải được đưa vào phép kiểm tra tương tác | Bắt buộc |
| YC-02 | Chỉ hiện cảnh báo có liên quan tới ít nhất một thuốc **đang kê**. Không hiện cảnh báo giữa hai thuốc đều thuộc đơn khác | Bắt buộc |
| YC-03 | Cửa sổ cảnh báo liệt kê rõ: tên thuốc từ đơn khác, ngày kê đơn đó, ngày dùng đến | Bắt buộc |
| YC-04 | Bệnh viện chọn được phạm vi: chỉ đơn hiện tại / cùng hồ sơ điều trị / toàn lịch sử bệnh nhân | Bắt buộc |
| YC-05 | Mặc định khi cập nhật phần mềm: giữ nguyên hành vi hiện tại, không tự bật | Bắt buộc |
| YC-06 | Không được làm chậm thao tác mở màn hình và lưu đơn một cách đáng kể | Bắt buộc |
| YC-07 | Nếu không lấy được dữ liệu đơn khác (mạng, máy chủ) thì **không được chặn bác sĩ lưu đơn**; vẫn kiểm tra phạm vi đơn hiện tại như cũ | Bắt buộc |
| YC-08 | Ghi nhận vào sổ theo dõi: phạm vi đã kiểm tra, số thuốc từ đơn khác, người xác nhận, thời điểm xác nhận | Bắt buộc |
| YC-09 | Chức năng chuột phải "Đánh giá thông tin thuốc" dùng cùng phạm vi | Nên có |
| YC-10 | Cảnh báo mức nặng phát sinh từ đơn khác nên yêu cầu bác sĩ nhập lý do khi vẫn tiếp tục lưu | Chờ chốt — Phần 12 |

---

# PHẦN 5. QUY TẮC NGHIỆP VỤ

> Mỗi quy tắc có mã `QT-xx`. Tài liệu kỹ thuật và test case đều tham chiếu theo mã này.

## 5.1 Nhóm bật / tắt

| Mã | Quy tắc |
|---|---|
| **QT-01** | Tính năng chỉ chạy khi bệnh viện đang dùng MIMS để kiểm tra tương tác. Bệnh viện dùng hệ thống kiểm tra tương tác khác không bị ảnh hưởng |
| **QT-02** | Có ba mức phạm vi: **(1)** chỉ đơn đang kê — mặc định, đúng như hiện tại; **(2)** thêm các đơn khác trong **cùng hồ sơ điều trị**; **(3)** thêm các đơn trong **toàn lịch sử bệnh nhân** |
| **QT-03** | Đổi cấu hình có hiệu lực từ lần mở màn hình kê đơn tiếp theo, không cần khởi động lại phần mềm |

## 5.2 Nhóm phạm vi hồ sơ

| Mã | Quy tắc |
|---|---|
| **QT-04** | Mức phạm vi (2): lấy thuốc của mọi đơn thuộc **cùng một lần điều trị** — cùng hồ sơ, bất kể khoa nào, bác sĩ nào, kê ở màn hình nào |
| **QT-05** | Mức phạm vi (3): lấy thuốc của mọi đơn thuộc **cùng bệnh nhân**, kể cả các lần điều trị trước, trong khoảng số ngày do bệnh viện cấu hình |
| **QT-06** | Đơn của bệnh nhân khác không bao giờ được lấy, kể cả khi bác sĩ đang kê cho nhiều bệnh nhân cùng lúc |

## 5.3 Nhóm "thuốc nào được coi là đang dùng"

| Mã | Quy tắc |
|---|---|
| **QT-07** | Thuốc được coi là **đang dùng** khi **ngày dùng đến** của nó **bằng hoặc sau** ngày–giờ chỉ định của đơn đang kê. Quy tắc này theo đúng khuyến nghị của MIMS: thuốc hết ngày dùng thì coi là đã dừng |
| **QT-08** | Thuốc **không có ngày dùng đến** (bệnh viện không nhập số ngày dùng): lấy nếu **ngày kê đơn** nằm trong khoảng số ngày do bệnh viện cấu hình, tính lùi từ ngày chỉ định hiện tại. Mặc định 30 ngày |
| **QT-09** | Bác sĩ đổi ngày hoặc giờ chỉ định trên màn hình → danh sách thuốc đang dùng được xác định lại theo mốc thời gian mới |
| **QT-10** | Thuốc trên **đơn đã hủy** hoặc bản ghi đã xóa không được lấy |

## 5.4 Nhóm loại trừ

| Mã | Quy tắc |
|---|---|
| **QT-11** | Nếu bác sĩ đang **sửa** một đơn đã lưu, thuốc của chính đơn đó không bị coi là "đơn khác" — tránh cảnh báo thuốc tương tác với chính nó |
| **QT-12** | Thuốc của đơn khác **trùng đúng thuốc** bác sĩ đang kê thì không đưa thêm vào phép kiểm tra — trường hợp này đã có cảnh báo trùng thuốc riêng (mục 2.4) |
| **QT-13** | Mỗi thuốc chỉ tính một lần, dù xuất hiện ở nhiều đơn khác nhau. Lấy bản ghi có ngày dùng đến **xa nhất** để hiển thị |
| **QT-14** | Vật tư, hóa chất không được đưa vào kiểm tra tương tác — chỉ thuốc |
| **QT-15** | Số thuốc lấy từ đơn khác bị giới hạn tối đa (mặc định 30 thuốc), ưu tiên thuốc còn dùng lâu nhất. Nếu bị cắt, hệ thống ghi nhận vào sổ theo dõi để phòng CNTT biết cần nâng giới hạn |

## 5.5 Nhóm hiển thị cảnh báo

| Mã | Quy tắc |
|---|---|
| **QT-16** | Cảnh báo chỉ hiện khi liên quan tới ít nhất một thuốc bác sĩ **đang kê**. Cặp tương tác giữa hai thuốc đều thuộc đơn khác **không hiện** |
| **QT-17** | Cửa sổ cảnh báo có một khối riêng ở **đầu**, tiêu đề *"Thuốc đang dùng từ đơn khác trong hồ sơ (đã đưa vào kiểm tra tương tác)"*, liệt kê từng thuốc kèm ngày kê và ngày dùng đến |
| **QT-18** | Phần nội dung cảnh báo của MIMS bên dưới giữ nguyên định dạng, màu sắc theo mức nghiêm trọng như hiện tại |
| **QT-19** | Không có thuốc đơn khác nào thoả điều kiện → cửa sổ cảnh báo giữ **nguyên như hiện tại**, không có khối bổ sung |
| **QT-20** | Hai nút hiện có giữ nguyên ý nghĩa: **Xác nhận** = bác sĩ đã đọc, tiếp tục lưu đơn; **Bỏ qua** hoặc đóng cửa sổ = **không lưu** đơn |

## 5.6 Nhóm xử lý khi có lỗi

| Mã | Quy tắc |
|---|---|
| **QT-21** | Không lấy được thuốc đơn khác (máy chủ chậm, mất mạng nội bộ) → vẫn kiểm tra MIMS với thuốc đơn hiện tại, **không chặn** bác sĩ lưu đơn, ghi nhật ký lỗi |
| **QT-22** | MIMS không phản hồi hoặc báo lỗi → giữ nguyên hành vi hiện tại: hiện thông báo *"Kiểm tra kết nối MIMS"*, bác sĩ tự quyết định |
| **QT-23** | Thuốc chưa được ánh xạ MIMS bị bỏ qua **âm thầm** như hiện tại; số lượng thuốc bị bỏ qua được ghi vào sổ theo dõi để phòng Dược rà soát |

---

# PHẦN 6. CỬA SỔ CẢNH BÁO VÀ THÔNG BÁO

## 6.1 Bố cục cửa sổ cảnh báo sau khi bổ sung

```
┌──────────────────────────────────────────────────────────────────────────┐
│ Kiểm tra tương tác thuốc, bệnh liên quan                          [ X ] │
├──────────────────────────────────────────────────────────────────────────┤
│ ┌──────────────────────────────────────────────────────────────────────┐ │
│ │ Thuốc đang dùng từ đơn khác trong hồ sơ                    ← MỚI     │ │
│ │ (đã đưa vào kiểm tra tương tác):                                     │ │
│ │  • Warfarin 5mg   — đơn ngày 05/09/2026, dùng đến 20/09/2026         │ │
│ │  • Panadol Extra  — đơn ngày 06/09/2026, dùng đến 10/09/2026         │ │
│ └──────────────────────────────────────────────────────────────────────┘ │
│                                                                          │
│  ── Nội dung cảnh báo do MIMS trả về (giữ nguyên như hiện tại) ──        │
│                                                                          │
│  ● NGHIÊM TRỌNG   Warfarin  ↔  Diclofenac                               │
│    Tăng nguy cơ xuất huyết tiêu hóa...                                   │
│                                                                          │
│  ● TRÙNG HOẠT CHẤT   Panadol Extra  ↔  Efferalgan                       │
│    Cùng hoạt chất paracetamol — nguy cơ quá liều...                      │
│                                                                          │
├──────────────────────────────────────────────────────────────────────────┤
│                                          [ Xác nhận ]     [ Bỏ qua ]     │
└──────────────────────────────────────────────────────────────────────────┘
```

Thay đổi duy nhất trên giao diện là **khối chữ ở đầu cửa sổ**. Không thêm nút, không thêm ô nhập, không đổi màn hình kê đơn.

## 6.2 Nội dung khối "Thuốc đang dùng từ đơn khác"

| Thông tin hiển thị | Vì sao cần |
|---|---|
| Tên thuốc | Để bác sĩ biết thuốc nào gây cảnh báo |
| Ngày kê của đơn đó | Để biết đơn nào, tra được ai kê |
| Ngày dùng đến | Để quyết định: hoãn thuốc mới, hay đổi thuốc, hay dừng thuốc cũ |

Ba thông tin bổ sung sau đây **có thể hiển thị nếu khách hàng yêu cầu** — cần chốt ở Phần 12 vì làm khối chữ dài hơn:

- Khoa kê đơn
- Bác sĩ kê đơn
- Mã đơn thuốc

## 6.3 Bảng thông báo cho người dùng

| Tình huống | Nội dung | Kiểu |
|---|---|---|
| Có tương tác, có thuốc từ đơn khác | Cửa sổ cảnh báo như 6.1 | Xác nhận / Bỏ qua |
| Có tương tác, không có thuốc từ đơn khác | Cửa sổ cảnh báo như hiện tại | Xác nhận / Bỏ qua |
| Không có tương tác | Không hiện gì, lưu bình thường | — |
| MIMS không phản hồi | *Kiểm tra kết nối MIMS* | Thông báo |
| Không lấy được thuốc đơn khác | Không thông báo cho bác sĩ; ghi nhật ký kỹ thuật | — |
| Cảnh báo mức nặng từ đơn khác (nếu chốt YC-10) | *Cảnh báo mức nghiêm trọng liên quan thuốc đơn khác. Nhập lý do tiếp tục kê:* | Bắt buộc nhập lý do |

> Toàn bộ nhãn và thông báo phải có đủ tiếng Việt và tiếng Anh.

---

# PHẦN 7. QUY TRÌNH SỬ DỤNG THEO TÌNH HUỐNG

## Tình huống 1 — Bác sĩ kê đơn bình thường, không có tương tác

1. Bác sĩ chọn thuốc, nhập liều, bấm **Lưu**.
2. Phần mềm âm thầm gửi MIMS thuốc đang kê + thuốc đơn khác còn hiệu lực.
3. Không có cảnh báo → đơn được lưu ngay.

Bác sĩ **không cảm nhận được thay đổi nào**. Đây phải là tình huống chiếm đa số.

## Tình huống 2 — Có tương tác với thuốc đơn khác

1. Bác sĩ bấm **Lưu** → cửa sổ cảnh báo hiện ra.
2. Bác sĩ đọc khối đầu: thuốc nào từ đơn khác, dùng đến ngày nào.
3. Bác sĩ chọn một trong ba hướng xử lý:

| Hướng xử lý | Thao tác |
|---|---|
| Đổi thuốc mình đang kê | Bấm **Bỏ qua** → về màn kê đơn, đổi thuốc, lưu lại |
| Hoãn thuốc mới đến khi hết thuốc cũ | Bấm **Bỏ qua** → đổi ngày chỉ định hoặc hẹn tái khám |
| Vẫn kê, có theo dõi | Bấm **Xác nhận** → đơn được lưu, hệ thống ghi nhận bác sĩ đã đọc cảnh báo |

## Tình huống 3 — Thuốc đơn khác do bác sĩ khác kê, cần trao đổi

1. Bác sĩ đọc khối cảnh báo, biết đơn ngày nào.
2. Tra hồ sơ điều trị để biết bác sĩ và khoa kê đơn đó.
3. Trao đổi, thống nhất phương án; nếu phải dừng thuốc cũ thì bác sĩ kê đơn cũ thực hiện.
4. Quay lại lưu đơn.

> Đây là điểm quan trọng cần thống nhất trong quy trình bệnh viện: **bác sĩ đang kê không sửa được đơn của bác sĩ khác**. Cảnh báo giúp phát hiện, nhưng việc xử lý cần quy trình phối hợp — xem Phần 10.2.

## Tình huống 4 — Bác sĩ chủ động kiểm tra trước khi lưu

1. Chọn nhiều thuốc trong đơn → chuột phải → **Đánh giá thông tin thuốc**.
2. Kết quả trả về đã tính cả thuốc các đơn khác.
3. Bác sĩ điều chỉnh đơn trước khi bấm Lưu, tránh phải quay lại.

## Tình huống 5 — Dược sĩ lâm sàng rà soát sau

1. Truy vấn sổ theo dõi cảnh báo tương tác theo khoảng thời gian, khoa, bác sĩ.
2. Xem các trường hợp cảnh báo mức nặng mà bác sĩ vẫn tiếp tục kê.
3. Đưa vào nội dung bình đơn thuốc / họp Hội đồng thuốc và điều trị.

---

# PHẦN 8. CẤU HÌNH CHO BỆNH VIỆN

## 8.1 Ba tham số cấu hình

| Tham số | Ý nghĩa nghiệp vụ | Giá trị | Mặc định |
|---|---|---|---|
| Phạm vi kiểm tra tương tác | Quyết định lấy thuốc từ đâu | **1** = chỉ đơn đang kê · **2** = cùng hồ sơ điều trị · **3** = toàn lịch sử bệnh nhân | **1** (giữ nguyên hiện tại) |
| Số ngày lùi | Áp dụng cho thuốc **không có ngày dùng đến** — coi là còn dùng nếu kê trong bao nhiêu ngày gần đây | Số nguyên (ngày) | **30** |
| Cách gửi dữ liệu tới MIMS | Tham số kỹ thuật, chỉ đổi khi kết quả thử nghiệm MIMS yêu cầu | 1 hoặc 2 | **1** |

## 8.2 Khuyến nghị chọn phạm vi theo loại bệnh viện

| Loại hình | Khuyến nghị | Lý do |
|---|---|---|
| Bệnh viện có nội trú | **2** (cùng hồ sơ điều trị) | Đúng bài toán: nhiều đơn trong một lần điều trị |
| Phòng khám, bệnh viện chủ yếu ngoại trú | **3** (toàn lịch sử) với số ngày lùi 30 | Mỗi lần khám là một hồ sơ riêng; mức 2 sẽ không bắt được đơn lần khám trước |
| Bệnh viện chuyên khoa dùng ít thuốc | **2** | Đủ dùng, ít cảnh báo |
| Giai đoạn thử nghiệm | **1** ở toàn viện, **2** ở một khoa | Đo lường trước khi mở rộng |

> Lưu ý quan trọng cho bệnh viện ngoại trú: ở mức **2**, nếu mỗi lần khám tạo một hồ sơ điều trị mới thì đơn của lần khám trước **không** được xét. Muốn bắt tình huống 3.2 thì phải chọn mức **3**.

## 8.3 Lộ trình bật khuyến nghị

| Giai đoạn | Thời gian | Việc làm |
|---|---|---|
| 0 | Trước khi bật | Phòng Dược rà soát tỷ lệ ánh xạ MIMS của danh mục thuốc (mục 1.5) |
| 1 | Tuần 1 | Bật mức **2** cho **một khoa nội trú**. Đo số cảnh báo/ngày, số lần bác sĩ bấm Bỏ qua |
| 2 | Tuần 2–3 | Dược sĩ lâm sàng rà 100% cảnh báo phát sinh, đánh giá tỷ lệ cảnh báo có giá trị lâm sàng |
| 3 | Tuần 4 | Nếu tỷ lệ cảnh báo nhiễu cao → thu hẹp bằng cách chỉ hiển thị mức nghiêm trọng (xem Phần 12, điểm 6) |
| 4 | Sau đó | Mở rộng toàn viện |

Tắt tính năng chỉ cần đưa tham số phạm vi về **1** — không cần cập nhật lại phần mềm.

---

# PHẦN 9. NHỮNG GÌ KHÔNG ĐƯỢC THAY ĐỔI

Danh sách bắt buộc kiểm thử lại để chắc chắn nghiệp vụ hiện tại không bị ảnh hưởng:

1. Kê đơn và lưu đơn khi bệnh viện **không** dùng MIMS.
2. Kiểm tra tương tác trong phạm vi một đơn khi phạm vi để mức **1** — phải giống hoàn toàn hiện tại.
3. Cảnh báo trùng tên thuốc với đơn cũ (mục 2.4).
4. Cảnh báo thai kỳ / cho con bú.
5. Cảnh báo chống chỉ định theo quy định Việt Nam.
6. Cảnh báo tương tác nội bộ theo hoạt chất của phần mềm (không qua MIMS).
7. Chức năng chuột phải "Thông tin thuốc" khi chọn một thuốc.
8. Kê đơn cho nhiều bệnh nhân cùng lúc; kê nhiều ngày; đơn dự trù; đơn tủ trực.
9. Ý nghĩa hai nút **Xác nhận** / **Bỏ qua** của cửa sổ cảnh báo.
10. Tốc độ mở màn hình kê đơn và tốc độ lưu đơn.

---

# PHẦN 10. ẢNH HƯỞNG NGHIỆP VỤ VÀ RỦI RO VẬN HÀNH

## 10.1 Số lượng cảnh báo sẽ tăng

Đây là hệ quả **không tránh được** và cũng chính là mục đích của việc 52540: cảnh báo tăng vì trước đây bị bỏ sót.

| Nhóm bệnh nhân | Mức tăng dự kiến |
|---|---|
| Ngoại trú, một đơn/lần khám | Rất ít |
| Nội trú ngắn ngày | Trung bình |
| Nội trú dài ngày, nhiều khoa, bệnh nhân cao tuổi nhiều bệnh nền | **Cao** |

Rủi ro đi kèm: **bác sĩ bấm Xác nhận theo phản xạ** khi cảnh báo quá nhiều, làm mất tác dụng của cả hệ thống cảnh báo. Đây là rủi ro nghiệp vụ lớn nhất của việc này.

Biện pháp giảm thiểu:

1. Chỉ hiển thị cảnh báo liên quan thuốc đang kê (QT-16) — đã thiết kế sẵn.
2. Bật theo giai đoạn, đo lường trước khi mở rộng (mục 8.3).
3. Nếu vẫn nhiều: giới hạn mức nghiêm trọng được hiển thị (Phần 12, điểm 6).
4. Theo dõi tỷ lệ **Xác nhận / tổng cảnh báo** hàng tháng — tỷ lệ tiến gần 100% là dấu hiệu cảnh báo đang bị bỏ qua theo phản xạ.

## 10.2 Trách nhiệm xử lý cảnh báo chéo đơn

Vấn đề nghiệp vụ mới phát sinh: cảnh báo chỉ ra tương tác với **đơn của bác sĩ khác**, nhưng bác sĩ đang kê không có quyền sửa đơn đó.

Ba phương án quy trình, cần bệnh viện chọn:

| Phương án | Nội dung | Ưu | Nhược |
|---|---|---|---|
| **A** | Bác sĩ đang kê tự quyết định trong phạm vi đơn của mình (đổi thuốc / hoãn / vẫn kê) | Đơn giản, không cần quy trình mới | Không xử lý được gốc nếu thuốc cũ mới là thuốc nên dừng |
| **B** | Cảnh báo mức nặng → bác sĩ **bắt buộc nhập lý do**, dược sĩ lâm sàng rà lại trong ngày | Có kiểm soát, có dấu vết | Thêm một bước nhập liệu |
| **C** | Cảnh báo mức nặng → chặn lưu, phải hội chẩn dược | An toàn nhất | Chặn luồng điều trị, rủi ro chậm trễ cấp cứu |

Khuyến nghị: **phương án B**, và **không** chọn C với đơn cấp cứu.

## 10.3 Giá trị nghiệp vụ thu được

| Giá trị | Diễn giải |
|---|---|
| An toàn người bệnh | Phát hiện tương tác nghiêm trọng vốn không thể phát hiện bằng mắt khi thuốc nằm rải ở nhiều đơn |
| Phòng ngừa quá liều | Trùng hoạt chất giữa các đơn — đặc biệt paracetamol, NSAID, corticoid |
| Dữ liệu dược lâm sàng | Sổ theo dõi cảnh báo tương tác trở thành nguồn dữ liệu thật cho bình đơn thuốc và Hội đồng thuốc |
| Hồ sơ pháp lý | Ghi nhận rõ: đã cảnh báo, ai đọc, lúc nào, quyết định gì — bảo vệ cả bệnh nhân và bác sĩ |
| Tiêu chí chất lượng | Phục vụ tiêu chí về hoạt động dược lâm sàng và an toàn sử dụng thuốc trong đánh giá chất lượng bệnh viện |

## 10.4 Giới hạn cần nói rõ với người dùng

| Giới hạn | Diễn giải |
|---|---|
| Không kiểm tra thuốc ngoài hệ thống | Bệnh nhân tự mua thuốc, dùng thực phẩm chức năng → phần mềm không biết. Vẫn phải hỏi bệnh nhân |
| Phụ thuộc ánh xạ danh mục | Thuốc chưa ánh xạ MIMS → không kiểm tra được, không thông báo |
| Phụ thuộc chất lượng nhập liệu ngày dùng | Đơn không nhập số ngày dùng → phải suy luận theo số ngày cấu hình, có thể lấy thừa hoặc thiếu |
| Không thay thế đánh giá của bác sĩ | Dữ liệu MIMS là hỗ trợ quyết định, không phải chỉ định điều trị |
| Giới hạn 30 thuốc từ đơn khác | Hồ sơ nội trú rất dài có thể vượt giới hạn; phần bị cắt được ghi nhận để phòng CNTT xử lý |

---

# PHẦN 11. TEST CASE NGHIỆP VỤ

## Nhóm A — Chức năng hiện tại không bị ảnh hưởng

| Mã | Điều kiện | Kỳ vọng |
|---|---|---|
| TC-A1 | Bệnh viện không dùng MIMS | Lưu đơn bình thường, không có cảnh báo tương tác |
| TC-A2 | Dùng MIMS, phạm vi để mức **1** | Cảnh báo giống hoàn toàn trước khi cập nhật |
| TC-A3 | Dùng MIMS, phạm vi mức **2**, hồ sơ chỉ có một đơn | Không có khối "thuốc từ đơn khác"; cảnh báo như cũ |
| TC-A4 | Cảnh báo trùng tên thuốc với đơn cũ | Vẫn hiện như trước |
| TC-A5 | Bệnh nhân nữ mang thai, phạm vi mức **2** | Cảnh báo thai kỳ vẫn hiện đầy đủ |

## Nhóm B — Phát hiện đúng tương tác chéo đơn

| Mã | Điều kiện | Kỳ vọng |
|---|---|---|
| TC-B1 | Tình huống 3.1 (Warfarin + Diclofenac, hai khoa) | Hiện cảnh báo nghiêm trọng, khối đầu ghi đúng Warfarin và ngày dùng đến |
| TC-B2 | Tình huống 3.3 (trùng hoạt chất paracetamol) | Hiện cảnh báo trùng hoạt chất |
| TC-B3 | Hai thuốc cùng nhóm điều trị ở hai đơn | Hiện cảnh báo trùng nhóm điều trị |
| TC-B4 | Cặp chống chỉ định theo quy định Việt Nam ở hai đơn | Hiện cảnh báo tương ứng |
| TC-B5 | Bác sĩ bấm **Xác nhận** | Đơn được lưu; sổ theo dõi ghi nhận người và thời điểm xác nhận |
| TC-B6 | Bác sĩ bấm **Bỏ qua** | Đơn **không** được lưu, quay về màn kê đơn giữ nguyên dữ liệu đang nhập |

## Nhóm C — Không gây nhiễu

| Mã | Điều kiện | Kỳ vọng |
|---|---|---|
| TC-C1 | Hai thuốc **đều thuộc đơn khác** tương tác với nhau; thuốc đang kê không liên quan | **Không** hiện cảnh báo |
| TC-C2 | Thuốc đơn khác đã quá ngày dùng đến | Không đưa vào kiểm tra, không hiện trong khối đầu |
| TC-C3 | Thuốc đơn khác trùng đúng thuốc đang kê | Không đưa vào kiểm tra lần hai |
| TC-C4 | Bác sĩ **sửa** đơn đã lưu | Thuốc của chính đơn đang sửa không bị coi là đơn khác |
| TC-C5 | Đơn khác đã bị hủy | Không lấy thuốc của đơn đó |
| TC-C6 | Vật tư ở đơn khác | Không đưa vào kiểm tra |

## Nhóm D — Hiển thị

| Mã | Điều kiện | Kỳ vọng |
|---|---|---|
| TC-D1 | Có 3 thuốc từ đơn khác | Khối đầu liệt kê đủ 3, đúng tên, đúng ngày kê, đúng ngày dùng đến |
| TC-D2 | Tên thuốc có ký tự đặc biệt | Hiển thị đúng, không lỗi hiển thị |
| TC-D3 | Đổi ngôn ngữ sang tiếng Anh | Tiêu đề khối và nhãn hiển thị tiếng Anh |
| TC-D4 | Đổi ngày chỉ định trên màn kê đơn rồi lưu | Danh sách thuốc đơn khác được tính lại theo ngày mới |

## Nhóm E — Tình huống lỗi

| Mã | Điều kiện | Kỳ vọng |
|---|---|---|
| TC-E1 | Máy chủ không trả được dữ liệu đơn khác | Vẫn kiểm tra phạm vi đơn hiện tại; **không chặn** lưu đơn; không hiện lỗi cho bác sĩ |
| TC-E2 | MIMS không phản hồi | Hiện *Kiểm tra kết nối MIMS* như hiện tại |
| TC-E3 | Thuốc đơn khác chưa ánh xạ MIMS | Bị bỏ qua; số lượng bỏ qua được ghi vào sổ theo dõi |
| TC-E4 | Hồ sơ nội trú có hơn 30 thuốc đơn khác còn hiệu lực | Chỉ lấy 30 thuốc còn dùng lâu nhất; có ghi nhận việc bị cắt |

## Nhóm F — Cấu hình

| Mã | Điều kiện | Kỳ vọng |
|---|---|---|
| TC-F1 | Đổi phạm vi từ **1** sang **2** | Có hiệu lực từ lần mở màn hình kê đơn tiếp theo |
| TC-F2 | Phạm vi mức **3** | Lấy được thuốc từ hồ sơ điều trị khác của cùng bệnh nhân |
| TC-F3 | Phạm vi mức **3**, số ngày lùi = 7 | Đơn kê cách đây 20 ngày không có ngày dùng đến → không lấy |
| TC-F4 | Đưa phạm vi về **1** | Tính năng tắt hoàn toàn, không cần cập nhật phần mềm |

## Nhóm G — Tốc độ

| Mã | Điều kiện | Kỳ vọng |
|---|---|---|
| TC-G1 | Mở màn hình kê đơn, hồ sơ nội trú 200 dòng thuốc, phạm vi mức **2** | Thời gian mở không tăng đáng kể so với hiện tại |
| TC-G2 | Bấm Lưu → cửa sổ cảnh báo | Không chậm hơn hiện tại quá 1 giây |
| TC-G3 | Kê đơn cho nhiều bệnh nhân cùng lúc | Không treo, không chậm bất thường |

## Nhóm H — Sổ theo dõi cảnh báo

| Mã | Điều kiện | Kỳ vọng |
|---|---|---|
| TC-H1 | Có cảnh báo, bác sĩ xác nhận | Sổ theo dõi ghi: phạm vi kiểm tra, số thuốc từ đơn khác, mức nghiêm trọng cao nhất, người xác nhận, thời điểm |
| TC-H2 | Truy vấn sổ theo dõi theo khoa và khoảng thời gian | Lấy được danh sách phục vụ bình đơn thuốc |

---

# PHẦN 12. ĐIỂM CẦN KHÁCH HÀNG CHỐT

| # | Câu hỏi | Phương án gợi ý | Hệ quả nếu chọn khác |
|---|---|---|---|
| 1 | "Hồ sơ" hiểu là **một lần điều trị** hay **toàn bộ lịch sử bệnh nhân**? | Mức **2** cho bệnh viện có nội trú; mức **3** cho cơ sở ngoại trú | Chọn sai → bỏ sót tình huống 3.2 (ngoại trú) hoặc cảnh báo quá nhiều (mức 3 ở nội trú) |
| 2 | Cảnh báo tương tác chéo đơn **chỉ cảnh báo** hay **chặn lưu**? | Chỉ cảnh báo; mức nặng bắt buộc nhập lý do (phương án B, mục 10.2) | Chặn lưu → rủi ro chậm trễ điều trị, nhất là cấp cứu |
| 3 | Khi kê đơn cho **nhiều bệnh nhân cùng lúc**, kiểm tra theo hồ sơ đang mở hay từng bệnh nhân? | Theo hồ sơ đang mở (giữ như hiện tại) | Kiểm tra từng bệnh nhân → mỗi bệnh nhân một lần gọi MIMS, thao tác chậm rõ rệt |
| 4 | Có làm cho **cả 4** màn hình kê đơn (Phòng khám, CLS, Thận nhân tạo, YHCT) hay chỉ Phòng khám? | Cả 4 | Chỉ một màn → bác sĩ ở màn khác vẫn bị bỏ sót, gây hiểu nhầm là phần mềm đã kiểm tra hết |
| 5 | Khối cảnh báo có cần thêm **khoa** và **bác sĩ kê đơn** của đơn khác? | Có — giúp bác sĩ biết liên hệ ai (tình huống 3.3) | Không thêm → bác sĩ phải tự tra hồ sơ |
| 6 | Muốn hiển thị **mức nghiêm trọng nào**? | Giai đoạn đầu: đủ các mức để đánh giá. Sau đó có thể giới hạn còn: tương tác Nghiêm trọng, thuốc–bệnh Chống chỉ định, trùng hoạt chất mức 1, trùng nhóm điều trị mức 1 | Để đủ mức lâu dài → nguy cơ bác sĩ bỏ qua theo phản xạ (mục 10.1) |
| 7 | Thuốc của **đơn cũ** có cần xét chống chỉ định với **bệnh mới chẩn đoán hôm nay** không? | Không, ở giai đoạn đầu — vì bác sĩ đang kê không sửa được đơn cũ | Có → phát hiện thêm nhưng tăng cảnh báo mà bác sĩ đang kê không xử lý được |
| 8 | Có cần kiểm tra tương tác **ngay khi bổ sung từng thuốc** vào đơn, thay vì chỉ khi lưu? | Không, ở giai đoạn đầu | Có → phát hiện sớm hơn, nhưng mỗi lần thêm thuốc là một lần gọi MIMS, thao tác chậm |

---

# PHẦN 13. LỘ TRÌNH VÀ MỞ RỘNG

## 13.1 Lộ trình bàn giao

| Bước | Nội dung | Đầu ra |
|---|---|---|
| 1 | Khách hàng chốt 8 điểm Phần 12 | Biên bản chốt yêu cầu |
| 2 | Phòng Dược rà soát tỷ lệ ánh xạ MIMS của danh mục thuốc | Báo cáo tỷ lệ ánh xạ |
| 3 | Kỹ thuật thử nghiệm trên môi trường MIMS thử | Kết quả xác nhận MIMS trả cảnh báo đúng như thiết kế |
| 4 | Lập trình và kiểm thử nội bộ theo Phần 11 | Bản cài đặt thử nghiệm |
| 5 | Bật thí điểm một khoa nội trú, mức phạm vi **2** | Số liệu cảnh báo tuần đầu |
| 6 | Dược sĩ lâm sàng đánh giá giá trị lâm sàng của cảnh báo | Kết luận: mở rộng hoặc thu hẹp mức hiển thị |
| 7 | Mở rộng toàn viện | Biên bản nghiệm thu |

## 13.2 Chỉ số theo dõi sau khi bật

| Chỉ số | Cách lấy | Ngưỡng cần chú ý |
|---|---|---|
| Số cảnh báo chéo đơn / ngày / khoa | Sổ theo dõi cảnh báo | Tăng đột biến → xem lại mức hiển thị |
| Tỷ lệ bác sĩ bấm **Xác nhận** | Sổ theo dõi | Tiến gần 100% → dấu hiệu bỏ qua theo phản xạ |
| Tỷ lệ cảnh báo có giá trị lâm sàng | Dược sĩ đánh giá thủ công tuần đầu | Dưới 30% → cần thu hẹp mức hiển thị |
| Số thuốc bị bỏ qua vì chưa ánh xạ MIMS | Sổ theo dõi | Còn cao → tiếp tục rà danh mục |

## 13.3 Gợi ý mở rộng ngoài phạm vi việc 52540

| Gợi ý | Giá trị |
|---|---|
| Kiểm tra tương tác toàn hồ sơ tại màn hình **dược sĩ duyệt đơn** | Dược sĩ nhìn được toàn hồ sơ, là vị trí phù hợp nhất để chặn tương tác |
| Báo cáo tương tác thuốc theo khoa / bác sĩ / tháng | Phục vụ Hội đồng thuốc và điều trị |
| Cảnh báo tương tác khi **bổ sung từng thuốc**, không chờ đến lúc lưu | Bác sĩ phát hiện sớm, ít phải làm lại đơn |
| Hiển thị danh sách thuốc bệnh nhân đang dùng ngay trên màn hình kê đơn | Bác sĩ chủ động thấy bối cảnh trước khi kê, giảm cảnh báo phát sinh |
