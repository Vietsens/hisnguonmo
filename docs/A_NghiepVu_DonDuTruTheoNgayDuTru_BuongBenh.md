# TÀI LIỆU NGHIỆP VỤ

## Hiển thị đơn thuốc dự trù theo ngày dự trù tại màn Buồng bệnh

| Thông tin | Nội dung |
|---|---|
| Chức năng | Buồng bệnh — theo dõi y lệnh người bệnh nội trú |
| Người dùng | Bác sĩ điều trị, điều dưỡng, quản trị hệ thống |
| Tài liệu đi kèm | `B_KyThuat_DonDuTruTheoNgayDuTru_BuongBenh.docx` |
| Trạng thái | **Đã chốt** |

Dành cho khách hàng, người phân tích nghiệp vụ, kiểm thử viên. Phần lập trình nằm ở tài liệu B.

---

# PHẦN 1. VẤN ĐỀ VÀ MỤC TIÊU

## 1.1 Vấn đề

Màn Buồng bệnh xếp đơn theo **ngày kê đơn**. Đơn thuốc dự trù kê trước một ngày để chuẩn bị thuốc cho hôm sau, nên bị đặt sai ngày.

**Ví dụ**: mùng 6 kê đơn dự trù cho mùng 7. Sang mùng 7, chọn ngày mùng 7 thì **không thấy đơn** — phải quay lại mùng 6.

Hệ quả: bác sĩ và điều dưỡng phải nhớ đơn kê từ ngày nào, chuyển ngày qua lại, dễ bỏ sót đơn khi đi buồng.

## 1.2 Mục tiêu

Đơn thuốc dự trù cho ngày nào thì hiển thị ở đúng ngày đó, vẫn tra được ngày kê, phân biệt được với đơn thường. Có cấu hình bật/tắt theo bệnh viện, **mặc định tắt**.

## 1.3 Phạm vi

| Có làm | Không làm |
|---|---|
| Xếp đơn thuốc dự trù theo ngày dự trù | Thay đổi ngày kê lưu trong dữ liệu |
| Thêm 2 cột "Ngày kê" và "Ngày dự trù" | Thay đổi tổng hợp phiếu lĩnh thuốc |
| Đánh dấu phân biệt đơn dự trù | Thay đổi quy trình duyệt, cấp phát thuốc |
| Cấu hình bật/tắt toàn viện | Áp dụng cho dịch vụ, giường, đơn máu (QT-02) |
| | Thay đổi các màn hình ngoài Buồng bệnh |

---

# PHẦN 2. THAY ĐỔI TRÊN MÀN HÌNH

## 2.1 Danh sách ngày bên trái

| Trường hợp | Trước | Sau (cấu hình bật) |
|---|---|---|
| Mùng 6 kê đơn dự trù cho mùng 7 | Đơn ở ngày **06** | Đơn ở ngày **07** |
| Mùng 7 chưa có y lệnh nào khác | Không có ngày 07 | **Có** ngày 07 |
| Mùng 6 chỉ có đúng đơn dự trù đó | Có ngày 06 | Ngày 06 **biến mất** (QT-06) |

## 2.2 Danh sách y lệnh

| Thay đổi | Nội dung |
|---|---|
| Cột **Ngày kê** | Ngày bác sĩ kê đơn. Hiện với mọi y lệnh |
| Cột **Ngày dự trù** | Ngày đơn được dự trù dùng. Chỉ đơn dự trù, đơn thường để trống |
| Đánh dấu | Dòng đơn dự trù **tô màu xanh da trời**, di chuột hiện chú thích đủ 2 mốc ngày |

Khi cấu hình tắt, 2 cột này **ẩn** — giao diện y hệt hiện tại.

## 2.3 Tab "Tất cả"

Tab này nhóm y lệnh theo tờ điều trị. Đơn dự trù thuộc tờ điều trị của ngày kê, không thuộc ngày đang xem, nên gom vào nhóm riêng đặt trên đầu:

```
[Dự trù — kê ngày 06/08]        ← đơn đến từ ngày khác
[07/08/2026 08:30]              ← tờ điều trị của chính ngày 07
```

---

# PHẦN 3. QUY TẮC NGHIỆP VỤ

Tài liệu kỹ thuật tham chiếu lại các mã `QT-xx` dưới đây.

## QT-01 — Thế nào là đơn thuốc dự trù

Thỏa **đồng thời**: (1) là đơn thuốc theo QT-02; (2) có nhập ngày dự trù, và ngày dự trù **khác** ngày kê.

Nếu để trống ô "Dự trù", hoặc nhập trùng ngày kê → là **đơn thường**, không áp dụng tính năng.

## QT-02 — Phạm vi: chỉ đơn thuốc

| Áp dụng | Không áp dụng |
|---|---|
| Đơn thuốc kho | Đơn máu |
| Đơn thuốc tủ trực | Chỉ định dịch vụ (CLS, CĐHA, TDCN, PTTT) |
| Đơn thuốc điều trị | Y lệnh giường, y lệnh khám, y lệnh khác |

Màn chỉ định dịch vụ và chỉ định giường cũng có ô "Dự trù". Những y lệnh đó **vẫn ở ngày kê và vẫn hiện chữ "Dự trù: …" ở cột Khoa yêu cầu như hiện tại**, kể cả khi cấu hình bật.

## QT-03 — Ngày hiển thị

Đơn thuốc dự trù → hiển thị ở **ngày dự trù**. Mọi y lệnh còn lại → **ngày kê**, như hiện tại.

## QT-04 — Chuyển hẳn, không lặp

Đơn dự trù **chỉ xuất hiện ở ngày dự trù**, không hiển thị lại ở ngày kê — tránh nhìn thấy cùng một đơn ở hai ngày khi đối chiếu thuốc. Ngày kê vẫn tra được ở cột "Ngày kê" trên chính đơn đó.

## QT-05 — Dự trù nhiều ngày

Hệ thống **đã tách sẵn thành nhiều đơn, mỗi ngày một đơn**. Mùng 6 kê dự trù cho mùng 7 và mùng 8 → 2 đơn, mỗi đơn hiện ở đúng ngày của nó, cả hai đều ghi Ngày kê 06/08.

## QT-06 — Ngày kê trở nên rỗng

Sau khi chuyển đơn dự trù đi, nếu một ngày không còn y lệnh nào thì ngày đó **biến mất** khỏi danh sách ngày. Đây là hành vi đúng theo mong muốn.

## QT-07 / QT-08 / QT-09 — Hiển thị

Hiển thị đồng thời **Ngày kê và Ngày dự trù** (QT-07). Đơn dự trù **tô màu xanh da trời** kèm chú thích (QT-08). Ở tab "Tất cả", đơn dự trù đến từ ngày khác gom vào nhóm riêng `Dự trù — kê ngày X` trên đầu; đơn kê trong chính ngày đang xem vẫn nhóm theo tờ điều trị (QT-09).

## QT-10 — Ngày dự trù tương lai

Đơn dự trù cho ngày chưa tới **vẫn hiển thị**. Hôm nay mùng 6 kê dự trù mùng 10 → danh sách ngày có ngày 10.

## QT-11 — Khi cấu hình tắt

Giữ nguyên **hoàn toàn** hành vi hiện tại: đơn dự trù ở ngày kê, 2 cột mới ẩn, cột "Khoa yêu cầu" vẫn hiện "Dự trù: …" như cũ.

## QT-12 — Không đổi dữ liệu và quy trình kho dược

Ngày kê lưu trong dữ liệu, tổng hợp phiếu lĩnh, duyệt thuốc, cấp phát thuốc, các màn hình khác — **không đổi**. Chỉ đổi tiêu chí lọc và hiển thị trên màn Buồng bệnh.

## QT-13 — Bộ lọc "Theo khoa"

Áp dụng với đơn dự trù **giống hệt** đơn thường.

---

# PHẦN 4. TÌNH HUỐNG SỬ DỤNG

| # | Tình huống | Kết quả mong đợi |
|---|---|---|
| 1 | Bật cấu hình. Mùng 6 kê đơn dự trù cho mùng 7. Sang mùng 7 chọn ngày mùng 7 | Thấy đơn ngay, tô xanh da trời, Ngày kê 06/08, Ngày dự trù 07/08 |
| 2 | Cấu hình bật, chọn lại ngày mùng 6 để tìm đơn | **Không** còn đơn đó (QT-04). Nếu mùng 6 hết y lệnh thì ngày 06 biến mất (QT-06). Tra ngày kê ở cột "Ngày kê" của đơn |
| 3 | Cấu hình bật. Mùng 7 kê thêm một đơn thuốc thường | Ngày 07 hiện **cả hai** đơn, phân biệt bằng màu và cột Ngày dự trù |
| 4 | **Tắt** cấu hình. Kê đơn dự trù mùng 6 cho mùng 7 | Đơn vẫn ở mùng 6 như hiện tại, 2 cột mới ẩn (QT-11) |
| 5 | Cấu hình bật. Mùng 6 kê dự trù cho mùng 7 và mùng 8 | Mùng 7 thấy đơn thứ nhất, mùng 8 thấy đơn thứ hai (QT-05) |
| 6 | Cấu hình bật. Mùng 6 chỉ định dịch vụ CLS có dự trù cho mùng 7 | Chỉ định **vẫn ở mùng 6**, không tô màu, cột Khoa yêu cầu vẫn hiện "Dự trù: 07/08" (QT-02) |

---

# PHẦN 5. TEST CASE

## Nhóm A — Cấu hình TẮT, chức năng cũ không đổi (QT-11)

- [ ] A1 — Đơn dự trù mùng 6 cho mùng 7 → vẫn ở ngày 06, chọn mùng 7 không thấy
- [ ] A2 — Cột "Khoa yêu cầu" vẫn hiện "Dự trù: dd/MM/yyyy"; 2 cột mới ẩn; không tô màu
- [ ] A3 — Tốc độ hiển thị khi đổi ngày không chậm hơn trước

## Nhóm B — Xếp đơn theo ngày dự trù (cấu hình BẬT)

- [ ] B1 — Chọn mùng 7 thấy đơn kê mùng 6 (QT-03)
- [ ] B2 — Chọn lại mùng 6 không thấy đơn đó nữa (QT-04)
- [ ] B3 — Mùng 6 chỉ có đúng đơn đó → ngày 06 biến mất khỏi danh sách ngày (QT-06)
- [ ] B4 — Mùng 6 còn y lệnh khác → ngày 06 vẫn còn, chỉ thiếu đơn dự trù
- [ ] B5 — Mùng 7 trước đó chưa có y lệnh → ngày 07 **xuất hiện**
- [ ] B6 — Dự trù cho mùng 7 và mùng 8 → mỗi ngày đúng một đơn (QT-05)
- [ ] B7 — Dự trù cho ngày tương lai → ngày đó xuất hiện, chọn vào thấy đơn (QT-10)
- [ ] B8 — Ngày dự trù **trùng** ngày kê → coi là đơn thường, không nhảy ngày (QT-01)

## Nhóm C — Phạm vi chỉ đơn thuốc (QT-02)

- [ ] C1 — Dịch vụ CLS có dự trù → vẫn ở ngày kê, không tô màu
- [ ] C2 — Chỉ định giường có dự trù → vẫn ở ngày kê
- [ ] C3 — Đơn máu có dự trù → vẫn ở ngày kê
- [ ] C4 — Cột "Khoa yêu cầu" của các y lệnh trên vẫn hiện "Dự trù: …"

## Nhóm D — Hiển thị

- [ ] D1 — Đơn dự trù: đủ 2 cột Ngày kê + Ngày dự trù (QT-07)
- [ ] D2 — Đơn thường: cột Ngày dự trù **để trống**
- [ ] D3 — Đơn dự trù tô xanh da trời, đơn thường không tô; di chuột hiện chú thích (QT-08)
- [ ] D4 — Tab "Tất cả": đơn dự trù nằm trong nhóm `Dự trù — kê ngày X` trên đầu, tên nhóm ghi đúng ngày kê (QT-09)
- [ ] D5 — Bốn tab phân loại y lệnh đúng như trước

## Nhóm E — Kho dược không đổi (QT-12)

- [ ] E1 — Phiếu lĩnh thuốc tổng hợp ra kết quả **giống hệt** trước khi bật cấu hình
- [ ] E2 — Duyệt đơn, cấp phát thuốc hoạt động bình thường
- [ ] E3 — Mở đơn dự trù xem chi tiết → ngày kê trong đơn **không đổi**
- [ ] E4 — In đơn thuốc → ngày trên phiếu in không đổi

## Nhóm F — Thao tác, tốc độ, ngôn ngữ

- [ ] F1 — Sửa/xóa đơn dự trù ở ngày dự trù → đúng đơn đó, danh sách cập nhật đúng ngày
- [ ] F2 — Nút Sửa/Xóa enable/disable đúng phân quyền như trước
- [ ] F3 — Bộ lọc "Theo khoa" áp dụng như đơn thường (QT-13)
- [ ] F4 — Đổi ngày trên danh sách ngày: hiển thị **tức thời**
- [ ] F5 — Người bệnh nằm trên 60 ngày: màn hình vẫn mở được, không treo
- [ ] F6 — Giao diện tiếng Anh: 2 cột mới và chú thích hiển thị tiếng Anh, không để trống

## Nhóm G — Màn hình khác không bị ảnh hưởng

- [ ] G1 — Màn Hội chẩn, Duyệt khám chuyên khoa, Duyệt khám gây mê: cách hiển thị đơn dự trù **không đổi**

---

# PHẦN 6. ĐIỂM ĐÃ CHỐT

| # | Nội dung | Quyết định |
|---|---|---|
| 1 | Phạm vi áp dụng | Chỉ đơn thuốc — 3 loại: đơn kho, tủ trực, điều trị (QT-02) |
| 2 | Dự trù nhiều ngày | Hệ thống đã tách sẵn theo ngày (QT-05) |
| 3 | Tab "Tất cả" | Nhóm riêng `Dự trù — kê ngày X` trên đầu (QT-09) |
| 4 | Ngày dự trù tương lai | Vẫn hiển thị, không chặn (QT-10) |
| 5 | Ngày kê hết y lệnh | Biến mất khỏi danh sách ngày (QT-06) |
| 6 | Mặc định cấu hình | **Tắt** |
