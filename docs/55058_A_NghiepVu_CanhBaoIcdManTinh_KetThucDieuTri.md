# Việc 55058 — TÀI LIỆU NGHIỆP VỤ

## Đánh dấu mã bệnh ICD là bệnh mãn tính và cảnh báo khi bác sĩ phòng khám kết thúc điều trị

| Thông tin | Nội dung |
|---|---|
| Mã việc | 55058 |
| Chức năng 1 | Danh mục ICD (`HIS.Desktop.Plugins.HisIcd`) — thêm thuộc tính "Bệnh mãn tính" |
| Chức năng 2 | Xử lý khám (`HIS.Desktop.Plugins.ExamServiceReqExecute`) — cảnh báo khi kết thúc điều trị |
| Người dùng | Quản trị danh mục (phòng KHTH / CNTT), bác sĩ phòng khám |
| Tài liệu đi kèm | `55058_B_KyThuat_CanhBaoIcdManTinh_KetThucDieuTri` (dành cho lập trình viên) |
| Trạng thái | Chờ khách hàng chốt các điểm ở Phần 8 |

**Tài liệu này dành cho**: khách hàng, người phân tích nghiệp vụ, kiểm thử viên. Không chứa nội dung lập trình.

---

# PHẦN 1. BỐI CẢNH VÀ MỤC TIÊU

## 1.1 Vấn đề hiện tại

- Màn hình **Kết thúc điều trị** tại phòng khám đã có ô **"Mãn tính"** (hiện khi bệnh viện bật cấu hình mãn tính). Tích ô này → hồ sơ được ghi nhận là điều trị bệnh mãn tính (sinh diện điều trị ngoại trú tương ứng, bắt buộc nhập *Tóm tắt kết quả cận lâm sàng* và *Phương pháp điều trị*).
- Việc tích ô hoàn toàn **phụ thuộc trí nhớ của bác sĩ**. Danh mục ICD **chưa có** thông tin mã bệnh nào là bệnh mãn tính, nên phần mềm không thể nhắc.
- Hệ quả: bệnh nhân mắc bệnh mãn tính (tăng huyết áp, đái tháo đường, COPD…) bị kết thúc điều trị như bệnh thường → sai diện điều trị, thiếu hồ sơ quản lý bệnh mãn tính, ảnh hưởng thanh quyết toán BHYT.

## 1.2 Mục tiêu

| Mã | Mục tiêu |
|---|---|
| MT-01 | Quản trị danh mục đánh dấu được mã ICD nào là **bệnh mãn tính** |
| MT-02 | Khi bác sĩ phòng khám kết thúc điều trị, nếu **mã bệnh chính** là bệnh mãn tính mà **chưa tích "Mãn tính"** → phần mềm **cảnh báo** |
| MT-03 | Bác sĩ vẫn được quyền quyết định: quay lại tích, hoặc tiếp tục kết thúc |
| MT-04 | Không thay đổi hành vi của mọi màn hình / luồng khác |

## 1.3 Phạm vi

| | Nội dung |
|---|---|
| **Có làm** | Thêm ô "Bệnh mãn tính" vào màn hình Danh mục ICD (thêm / sửa / hiển thị trên danh sách) |
| **Có làm** | Cảnh báo tại **khung Kết thúc điều trị trong màn hình Xử lý khám** (phòng khám) |
| **Có làm** | Chỉ xét **mã bệnh chính** của phần kết thúc điều trị |
| **Không làm** | Không xét mã bệnh phụ |
| **Không làm** | Không áp dụng cho màn hình Kết thúc điều trị độc lập (nội trú, điều trị ngoại trú, buồng bệnh…) |
| **Không làm** | Không tự động tích ô "Mãn tính" thay bác sĩ |
| **Không làm** | Không chặn cứng việc kết thúc điều trị |
| **Không làm** | Không thay đổi cách ô "Mãn tính" đang hoạt động (ẩn/hiện, gọi ghi nhận ngay khi tích) |

---

# PHẦN 2. CHỨC NĂNG 1 — ĐÁNH DẤU BỆNH MÃN TÍNH TRONG DANH MỤC ICD

## 2.1 Mô tả

Màn hình **Danh mục ICD** bổ sung ô tích **"Bệnh mãn tính"**, đặt cùng nhóm với các thuộc tính hiện có (COVID, Bệnh truyền nhiễm, Mã phụ, Lao tiềm ẩn…).

| Hạng mục | Quy tắc |
|---|---|
| Tên hiển thị | **Bệnh mãn tính** |
| Gợi ý (tooltip) | "Mã bệnh là bệnh mãn tính. Khi bác sĩ phòng khám kết thúc điều trị với mã bệnh chính này mà chưa tích Mãn tính, phần mềm sẽ cảnh báo" |
| Mặc định khi thêm mới | **Không tích** |
| Khi chọn 1 dòng trên danh sách | Ô hiển thị đúng trạng thái đã lưu |
| Khi nhấn "Làm mới" | Bỏ tích |
| Lưu | Tích → lưu là bệnh mãn tính; bỏ tích → lưu là không phải |
| Danh sách | Thêm cột **"Bệnh mãn tính"** dạng ô tích (chỉ xem) |
| Bắt buộc | Không |

## 2.2 Hiệu lực

- Thay đổi có hiệu lực tại màn hình Xử lý khám **sau khi máy trạm tải lại danh mục ICD** (mở lại màn hình hoặc làm mới dữ liệu danh mục theo cơ chế hiện hành).
- Mã ICD đang **khóa** không được xét cảnh báo.

---

# PHẦN 3. CHỨC NĂNG 2 — CẢNH BÁO KHI KẾT THÚC ĐIỀU TRỊ Ở PHÒNG KHÁM

## 3.1 Điều kiện hiển thị cảnh báo

Cảnh báo hiển thị khi **đồng thời** thỏa mãn tất cả:

| # | Điều kiện |
|---|---|
| 1 | Bác sĩ thao tác tại màn hình **Xử lý khám** và đã tích **"Kết thúc điều trị"** |
| 2 | Bác sĩ nhấn **"Lưu và kết thúc"** (hoặc phím tắt tương ứng) |
| 3 | Ô **"Mãn tính"** đang **hiển thị và cho phép thao tác** (bệnh viện bật cấu hình mãn tính; hồ sơ chưa kết thúc) |
| 4 | **Mã bệnh chính** của phần kết thúc điều trị được đánh dấu **Bệnh mãn tính** trong danh mục ICD |
| 5 | Ô **"Mãn tính"** **chưa tích** |

## 3.2 Nội dung cảnh báo

Cửa sổ câu hỏi (Có / Không):

> **Cảnh báo**
> Mã bệnh chính **I10 - Bệnh lý tăng huyết áp vô căn (nguyên phát)** là bệnh mãn tính nhưng hồ sơ chưa được tích **"Mãn tính"**. Bạn có muốn tiếp tục kết thúc điều trị không?
> [ Có ]  [ Không ]

| Bác sĩ chọn | Kết quả |
|---|---|
| **Có** | Tiếp tục kết thúc điều trị như bình thường (các kiểm tra khác vẫn chạy) |
| **Không** | Dừng lại, **không lưu**; con trỏ chuyển tới ô "Mãn tính" để bác sĩ tích |

## 3.3 Không cảnh báo trong các trường hợp

| Trường hợp | Lý do |
|---|---|
| Ô "Mãn tính" đã tích | Đã đúng |
| Mã bệnh chính không phải bệnh mãn tính (chỉ bệnh phụ là mãn tính) | Ngoài phạm vi |
| Bệnh viện tắt cấu hình mãn tính → ô "Mãn tính" bị ẩn | Bác sĩ không có chỗ để tích |
| Hồ sơ đã kết thúc → ô "Mãn tính" bị khóa | Không sửa được |
| Chỉ "Lưu" khám, không kết thúc điều trị | Chưa kết thúc |
| Kết thúc điều trị ở màn hình khác (không phải Xử lý khám) | Ngoài phạm vi |
| Mã bệnh chính trống / mã ICD đã khóa | Không đủ dữ liệu |

## 3.4 Thứ tự trong quá trình lưu

Cảnh báo được kiểm tra **sau** các kiểm tra bắt buộc hiện có của phần kết thúc điều trị (thiếu thông tin, sai loại ra viện…) và **trước** khi gửi dữ liệu lưu. Nếu bác sĩ quay lại tích "Mãn tính", phần mềm áp dụng tiếp quy tắc hiện có: bắt buộc nhập *Tóm tắt kết quả cận lâm sàng* và *Phương pháp điều trị*.

---

# PHẦN 4. LUỒNG NGHIỆP VỤ

```
[Quản trị danh mục]
  Danh mục ICD → chọn mã (VD: I10) → tích "Bệnh mãn tính" → Lưu

[Bác sĩ phòng khám]
  Xử lý khám → nhập chẩn đoán → tích "Kết thúc điều trị"
    → chọn loại ra viện, mã bệnh chính = I10, KHÔNG tích "Mãn tính"
    → nhấn "Lưu và kết thúc"
        ├─ Các kiểm tra hiện có (không đổi)
        ├─ [MỚI] I10 là bệnh mãn tính & chưa tích Mãn tính?
        │     ├─ Có  → hỏi "Bạn có muốn tiếp tục kết thúc điều trị không?"
        │     │        ├─ Có    → tiếp tục lưu
        │     │        └─ Không → dừng, focus ô "Mãn tính"
        │     └─ Không → bỏ qua
        └─ Lưu kết thúc điều trị
```

---

# PHẦN 5. KỊCH BẢN KIỂM THỬ (NGHIỆP VỤ)

| # | Chuẩn bị | Thao tác | Kết quả mong đợi |
|---|---|---|---|
| KB-01 | — | Mở Danh mục ICD, thêm mới | Ô "Bệnh mãn tính" hiển thị, không tích |
| KB-02 | Mã I10 | Tích "Bệnh mãn tính" → Lưu → chọn lại dòng | Ô được tích; cột "Bệnh mãn tính" trên danh sách được tích |
| KB-03 | I10 đang là mãn tính | Bỏ tích → Lưu | Lưu thành công, không còn là mãn tính |
| KB-04 | I10 mãn tính, cấu hình mãn tính **bật** | Kết thúc ĐT, bệnh chính I10, **không** tích Mãn tính → Lưu và kết thúc | Hiện cảnh báo |
| KB-05 | Như KB-04 | Chọn **Có** | Kết thúc điều trị thành công, hồ sơ không mãn tính |
| KB-06 | Như KB-04 | Chọn **Không** | Không lưu; con trỏ ở ô "Mãn tính" |
| KB-07 | Như KB-06 | Tích "Mãn tính", nhập đủ CLS + PP điều trị → Lưu và kết thúc | Không cảnh báo, lưu thành công |
| KB-08 | I10 mãn tính | Bệnh chính I10, **đã tích** Mãn tính | Không cảnh báo |
| KB-09 | E11 mãn tính | Bệnh chính J00 (không mãn tính), bệnh phụ E11 | Không cảnh báo |
| KB-10 | Cấu hình mãn tính **tắt** | Bệnh chính I10 | Không cảnh báo (ô Mãn tính ẩn) |
| KB-11 | I10 mãn tính | Chỉ "Lưu" khám, không kết thúc ĐT | Không cảnh báo |
| KB-12 | I10 mãn tính | Kết thúc điều trị ở màn hình Kết thúc điều trị nội trú | Không cảnh báo |
| KB-13 | Hồ sơ đã kết thúc (ô Mãn tính bị khóa) | Mở lại, lưu | Không cảnh báo |
| KB-14 | Ngôn ngữ English | Như KB-04 | Cảnh báo tiếng Anh |

---

# PHẦN 6. DỮ LIỆU

| Dữ liệu | Thay đổi |
|---|---|
| Danh mục ICD | Thêm thuộc tính "Là bệnh mãn tính" (có / không). Mặc định: không |
| Hồ sơ điều trị | **Không đổi** — dùng thông tin "Mãn tính" đã có |
| Cấu hình hệ thống | Không thêm cấu hình mới (xem điểm chốt C-03) |

Khuyến nghị: phòng KHTH chuẩn bị danh sách mã ICD mãn tính (theo danh mục bệnh mãn tính của Bộ Y tế / BHXH đang áp dụng tại bệnh viện) để nhập ngay sau khi triển khai.

---

# PHẦN 7. ẢNH HƯỞNG

| Đối tượng | Ảnh hưởng |
|---|---|
| Quản trị danh mục | Thêm 1 ô tích, cần rà soát nhập danh sách bệnh mãn tính |
| Bác sĩ phòng khám | Có thể thấy thêm 1 cảnh báo khi kết thúc điều trị |
| Các màn hình khác | Không ảnh hưởng |
| Báo cáo / BHYT | Không đổi trực tiếp; giúp tăng tỷ lệ hồ sơ mãn tính được ghi nhận đúng |

---

# PHẦN 8. ĐIỂM CẦN KHÁCH HÀNG CHỐT

| Mã | Câu hỏi | Đề xuất mặc định |
|---|---|---|
| C-01 | Cảnh báo dạng **hỏi Có/Không** (cho tiếp tục) hay **chặn cứng** (bắt buộc tích)? | Hỏi Có/Không |
| C-02 | Khi bệnh viện **tắt** cấu hình mãn tính (ô Mãn tính bị ẩn) có cần cảnh báo không? | Không cảnh báo |
| C-03 | Có cần cấu hình bật/tắt riêng cho cảnh báo này không? | Không — cảnh báo tự có hiệu lực khi có mã ICD được đánh dấu |
| C-04 | Có cần xét cả **mã bệnh phụ** không? | Không (đúng yêu cầu: chỉ bệnh chính) |
| C-05 | Có cần hỗ trợ **nhập Excel** cột "Bệnh mãn tính" cho danh mục ICD không? | Làm sau nếu có yêu cầu |
