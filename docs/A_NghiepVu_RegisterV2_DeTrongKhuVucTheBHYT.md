# TÀI LIỆU NGHIỆP VỤ

## Để trống khu vực khi kiểm tra thẻ BHYT của đợt điều trị cũ

| Thông tin | Nội dung |
|---|---|
| Chức năng | Tiếp đón |
| Người dùng | Nhân viên tiếp đón; quản trị hệ thống (bật/tắt cấu hình) |
| Nội dung thay đổi | Thêm cấu hình hệ thống cho phép **không lấy khu vực (K1/K2/K3) của đợt khám chữa bệnh trước**: khu vực chỉ lấy theo kết quả kiểm tra thẻ trên cổng BHXH |
| Mặc định | **Tắt**: phần mềm hoạt động như hiện nay |
| Ngày lập | 27/10/2026 |
| Cập nhật | 30/10/2026: cổng BHXH lỗi / mất kết nối / báo thẻ không hợp lệ cũng để trống khu vực; áp dụng thêm cho đọc thẻ KCB |

Tài liệu dành cho khách hàng, người phân tích nghiệp vụ và kiểm thử viên.

---

# PHẦN 1. VẤN ĐỀ VÀ MỤC TIÊU

## 1.1 Vấn đề

- Khi người bệnh đã từng khám quay lại, màn hình Tiếp đón tự lấy lại thông tin của đợt khám chữa bệnh trước (hành chính, địa chỉ, liên hệ, thẻ BHYT…), **kể cả khu vực**.
- Nếu thẻ BHYT hiện tại **không có khu vực** (tra cứu trên cổng BHXH không trả về khu vực), ô "Khu vực" vẫn giữ khu vực của đợt trước, ví dụ K1.
- Khu vực sai này được lưu vào hồ sơ, đưa vào dữ liệu đề nghị thanh toán gửi BHXH, và **hồ sơ bị xuất toán**.
- Nhân viên tiếp đón khó phát hiện vì ô "Khu vực" đã được điền sẵn.

## 1.2 Mục tiêu

- Khu vực **chỉ lấy theo kết quả tra cứu thẻ trên cổng BHXH**, không lấy theo đợt khám trước.
- Thẻ không có khu vực thì ô "Khu vực" **để trống**.
- Cổng BHXH lỗi, mất kết nối hoặc báo thẻ không hợp lệ (không có căn cứ khu vực) thì ô "Khu vực" cũng **để trống**.
- Thẻ có khu vực K1/K2/K3 thì điền đúng khu vực đó như hiện nay.
- Khu vực trống vẫn **cho phép lưu** tiếp đón bình thường.
- Các thông tin khác của đợt khám trước vẫn được lấy lại như hiện nay.
- Bệnh viện tự quyết định có áp dụng hay không bằng cấu hình hệ thống.

## 1.3 Phạm vi

| Có làm | Không làm |
|---|---|
| Màn hình Tiếp đón | Các màn hình tiếp đón khác, màn hình sửa thông tin hồ sơ, kiosk |
| Mọi cách kiểm tra thẻ trên màn Tiếp đón: nhập số thẻ, quét QR thẻ BHYT, quét CCCD, tìm người bệnh cũ, đọc thẻ KCB, tự chuyển sang đối tượng BHYT | Sửa lại khu vực của các hồ sơ đã tiếp đón trước đây |
| Đối tượng dùng thẻ BHYT | Đối tượng không dùng thẻ (viện phí, dịch vụ…) |

---

# PHẦN 2. NGHIỆP VỤ HIỆN TẠI

## 2.1 Luồng tiếp đón người bệnh cũ

| Bước | Thao tác | Phần mềm xử lý |
|---|---|---|
| 1 | Nhân viên tìm người bệnh cũ hoặc nhập/quét thẻ BHYT | Lấy lại thông tin đợt khám trước, **điền cả khu vực cũ** (ví dụ K1) |
| 2 | — | Tra cứu thẻ trên cổng BHXH |
| 3 | — | Thẻ hợp lệ: cập nhật các thông tin thẻ theo cổng, nhưng **không cập nhật khu vực**, ô "Khu vực" vẫn là K1 |
| 4 | Nhân viên bấm Lưu | Lưu hồ sơ với khu vực K1 |
| 5 | — | Hồ sơ thanh toán gửi BHXH mang khu vực K1, trong khi thẻ không có khu vực, nên **bị xuất toán** |

## 2.2 Vai trò của khu vực với quyền lợi BHYT

- Khu vực K1/K2/K3 là căn cứ để người bệnh được hưởng quyền lợi như **đúng tuyến**.
- Ở những viện đã bật cấu hình "có khu vực thì tự tính đúng tuyến", khi ô Khu vực có giá trị, phần mềm tự đặt tuyến **đúng tuyến**.
- Vì vậy, khu vực sai không chỉ gây xuất toán mà còn làm người bệnh được tính quyền lợi cao hơn thực tế.

---

# PHẦN 3. NGHIỆP VỤ SAU KHI SỬA

## 3.1 Khi BẬT cấu hình

**Nguyên tắc:** khu vực chỉ lấy theo kết quả kiểm tra thẻ trên cổng BHXH. Cổng xác nhận được thẻ thì lấy đúng khu vực của cổng (cổng không có khu vực thì để trống). Cổng không xác nhận được thẻ (lỗi, mất kết nối, thẻ không hợp lệ) thì không có căn cứ, **để trống**.

| # | Tình huống | Kết quả trên màn Tiếp đón |
|---|---|---|
| 1 | Người bệnh cũ, đợt trước có khu vực K1, thẻ trên cổng **không có** khu vực | Ô "Khu vực" **để trống**; các thông tin khác của đợt trước vẫn được lấy lại |
| 2 | Thẻ trên cổng **có** khu vực (K1/K2/K3) | Điền đúng khu vực theo cổng, kể cả khi khác khu vực đợt trước (ví dụ đợt trước K1, cổng K2 thì điền K2) |
| 3 | Người bệnh mới, thẻ có khu vực | Điền khu vực theo cổng (như hiện nay) |
| 4 | Cổng xác nhận thẻ hết hạn nhưng có thẻ mới, hoặc thẻ còn hạn và có thẻ mới | Khu vực lấy theo thông tin cổng trả về |
| 5 | Cổng BHXH lỗi, mất kết nối | Không có căn cứ từ cổng, ô "Khu vực" **để trống** |
| 6 | Cổng báo thẻ sai thông tin / không hợp lệ / không tồn tại | Ô "Khu vực" **để trống** |
| 6a | Viện không bật kiểm tra thẻ qua cổng, hoặc đầu thẻ thuộc danh mục "không kiểm tra thông tin thẻ trên cổng BHYT", hoặc chưa khai báo tài khoản kết nối cổng | Phần mềm không tra cứu cổng, **giữ nguyên như hiện nay** |
| 7 | Ô "Khu vực" trống khi bấm Lưu | **Lưu thành công**, không bắt buộc nhập khu vực, không hiện thêm cảnh báo |
| 8 | Ô "Khu vực" thay đổi (bị để trống hoặc đổi khu vực) | Phần mềm **tự tính lại tuyến và mức hưởng** như khi nhân viên tự chọn/xóa khu vực |

Nhân viên vẫn có thể **tự chọn lại khu vực bằng tay** sau khi phần mềm để trống, nếu có căn cứ hợp lệ.

## 3.2 Luồng sau khi sửa (cấu hình bật)

| Bước | Thao tác | Phần mềm xử lý |
|---|---|---|
| 1 | Nhân viên tìm người bệnh cũ hoặc nhập/quét thẻ BHYT | Lấy lại thông tin đợt khám trước (tạm thời hiện khu vực cũ) |
| 2 | — | Tra cứu thẻ trên cổng BHXH |
| 3 | — | Cổng xác nhận được thẻ: **thay khu vực bằng khu vực của cổng**; cổng không có khu vực thì để trống ô "Khu vực". Cổng lỗi hoặc báo thẻ không hợp lệ: **để trống** ô "Khu vực" |
| 4 | — | Tính lại tuyến và mức hưởng theo khu vực mới |
| 5 | Nhân viên kiểm tra lại tuyến và bấm Lưu | Lưu hồ sơ với khu vực đúng theo thẻ |

## 3.3 Khi TẮT cấu hình

Phần mềm hoạt động **giống hệt hiện nay**: vẫn lấy lại khu vực của đợt khám trước.

Cấu hình được coi là tắt khi giá trị là 0, để trống, hoặc chưa được khai báo trên hệ thống.

---

# PHẦN 4. CẤU HÌNH HỆ THỐNG

| Thuộc tính | Nội dung |
|---|---|
| Tên cấu hình | Để trống khu vực khi kiểm tra thẻ BHYT của đợt điều trị cũ |
| Phạm vi áp dụng | Toàn viện |
| Giá trị | **1**: bật. **Khác 1 hoặc để trống**: tắt (mặc định) |
| Chức năng áp dụng | Tiếp đón |

**Mô tả hiển thị trong danh mục cấu hình:**

> Để trống khu vực khi kiểm tra thẻ BHYT của đợt điều trị cũ (màn hình Tiếp đón).
> • 1: Khi kiểm tra thẻ BHYT trên cổng BHXH, khu vực chỉ lấy theo kết quả trả về của cổng. Thẻ không có mã khu vực, hoặc cổng lỗi / không kết nối được / báo thẻ không hợp lệ thì để trống ô "Khu vực", không lấy khu vực của đợt khám chữa bệnh trước. Các thông tin khác của đợt khám chữa bệnh trước vẫn được lấy lại như hiện tại.
> • Khác 1 hoặc để trống: Giữ nguyên hành vi hiện tại (lấy lại khu vực của đợt khám chữa bệnh trước).
> Không áp dụng khi phần mềm không tra cứu cổng (tắt kiểm tra thẻ qua cổng, đầu thẻ thuộc danh mục không kiểm tra trên cổng).

Sau khi đổi giá trị cấu hình, máy trạm cần tải lại cấu hình (đăng nhập lại) để có hiệu lực.

---

# PHẦN 5. ẢNH HƯỞNG NGHIỆP VỤ

| # | Ảnh hưởng | Lưu ý cho người dùng |
|---|---|---|
| 1 | Người bệnh trước đây được tính **đúng tuyến nhờ có khu vực**, nay thẻ không có khu vực nên có thể chuyển thành **trái tuyến**, hoặc phải chọn lại tuyến và mức hưởng thay đổi | Đây là kết quả đúng theo thẻ. Nhân viên tiếp đón kiểm tra lại tuyến trước khi lưu |
| 2 | Tra cứu cổng mất khoảng 1–3 giây: trong thời gian này ô "Khu vực" vẫn hiện khu vực cũ, sau đó mới được để trống | Chờ có kết quả kiểm tra thẻ rồi mới bấm Lưu (giống với các thông tin thẻ khác hiện nay) |
| 3 | Cổng BHXH lỗi hoặc mất kết nối thì ô "Khu vực" bị để trống, kể cả khi thẻ thực tế có khu vực | Nếu có căn cứ (thẻ giấy, kiểm tra lại khi cổng thông), nhân viên tự chọn lại khu vực trước khi lưu |
| 4 | Hồ sơ đã tiếp đón trước ngày bật cấu hình | Không bị thay đổi |
| 5 | Các màn hình khác dùng chung vùng thông tin thẻ BHYT | Không bị ảnh hưởng |

---

# PHẦN 6. ĐIỂM CẦN KHÁCH HÀNG CHỐT

| # | Câu hỏi | Phương án đang áp dụng |
|---|---|---|
| 1 | Những kết quả tra cứu cổng nào được coi là "cổng đã xác nhận thẻ"? | Thẻ hợp lệ; thẻ hết hạn nhưng có thẻ mới; thẻ còn hạn và có thẻ mới |
| 2 | Cổng BHXH lỗi hoặc mất kết nối thì xử lý thế nào? | **Đã chốt 30/10/2026:** để trống khu vực (thẻ không hợp lệ cũng để trống) |
| 3 | Thẻ có khu vực khác đợt trước (đợt trước K1, cổng K2) thì có ghi đè không? | Có, ghi đè theo cổng |
| 4 | Khu vực bị để trống làm người bệnh đổi tuyến/mức hưởng có chấp nhận không? | Chấp nhận, phần mềm tự tính lại tuyến |
| 5 | Có áp dụng cho các màn hình tiếp đón khác không? | Chỉ áp dụng màn hình Tiếp đón |

---

# PHẦN 7. KỊCH BẢN KIỂM THỬ

| # | Cấu hình | Dữ liệu | Thao tác | Kết quả mong đợi |
|---|---|---|---|---|
| 1 | Bật | Người bệnh cũ, đợt trước K1; thẻ trên cổng không có khu vực | Nhập số thẻ, Enter | Ô "Khu vực" trống; địa chỉ, liên hệ, nơi đăng ký KCB ban đầu… vẫn lấy lại từ đợt trước |
| 2 | Bật | Như kịch bản 1 | Bấm Lưu | Lưu thành công, hồ sơ không có khu vực, không hiện cảnh báo về khu vực |
| 3 | Bật | Người bệnh cũ, đợt trước K1; cổng trả K2 | Kiểm tra thẻ | Ô "Khu vực" = K2 |
| 4 | Bật | Người bệnh mới; cổng trả K3 | Kiểm tra thẻ | Ô "Khu vực" = K3 |
| 5 | Bật | Người bệnh cũ K1; cổng lỗi / mất kết nối | Kiểm tra thẻ | Ô "Khu vực" trống |
| 5a | Bật | Người bệnh cũ K1; cổng báo thẻ không tồn tại / sai thông tin | Kiểm tra thẻ | Ô "Khu vực" trống |
| 5b | Bật | Người bệnh cũ K1; đầu thẻ thuộc danh mục không kiểm tra trên cổng | Kiểm tra thẻ | Ô "Khu vực" giữ K1 |
| 6 | Bật | Người bệnh cũ K1, trước đây đúng tuyến nhờ khu vực; thẻ không có khu vực | Kiểm tra thẻ | Tuyến được tính lại theo nơi đăng ký KCB ban đầu; mức hưởng thay đổi tương ứng |
| 7 | Bật | Người bệnh cũ K1; thẻ không có khu vực | Quét QR thẻ BHYT / quét CCCD / đọc thẻ KCB / tự chuyển sang đối tượng BHYT | Như kịch bản 1 |
| 8 | Bật | Như kịch bản 1 | Sau khi ô trống, nhân viên tự chọn K1 rồi Lưu | Lưu với K1 theo lựa chọn của nhân viên |
| 9 | Bật | Đối tượng viện phí (không dùng thẻ) | Tìm người bệnh cũ | Không xử lý khu vực |
| 10 | Tắt (= 0) | Người bệnh cũ K1; thẻ không có khu vực | Kiểm tra thẻ, Lưu | Giữ K1 như hiện nay |
| 11 | Chưa khai báo | Như kịch bản 10 | Như kịch bản 10 | Giữ K1 như hiện nay |
| 12 | Bật | Hồ sơ đã tiếp đón trước ngày bật | Mở lại / xem | Dữ liệu cũ không đổi |
