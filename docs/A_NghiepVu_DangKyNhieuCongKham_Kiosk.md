# TÀI LIỆU NGHIỆP VỤ

## Đăng ký khám nhiều công khám tại Kiosk tự phục vụ

| Thông tin | Nội dung |
|---|---|
| Chức năng | Kiosk đăng ký khám tự phục vụ (HIS.Desktop.Plugins.RegisterExamKiosk) |
| Người dùng | Người bệnh tự thao tác tại kiosk; nhân viên hướng dẫn, quản trị hệ thống |
| Chức năng tham chiếu | Tiếp đón — Đăng ký khám (RegisterV2, phím F4) đã cho phép chọn nhiều phòng khám |
| Tài liệu đi kèm | B_KyThuat_DangKyNhieuCongKham_Kiosk.docx (lập sau) |
| Trạng thái | **Đã triển khai** phần chức năng; còn 3 điểm chờ chốt — xem PHẦN 6 |

Dành cho khách hàng, người phân tích nghiệp vụ, kiểm thử viên. Phần lập trình nằm ở tài liệu B.

---

# PHẦN 1. VẤN ĐỀ VÀ MỤC TIÊU

## 1.1 Vấn đề

Kiosk hiện chỉ đăng ký được **một công khám cho mỗi lượt**: người bệnh chạm vào một phòng khám, chọn một dịch vụ khám, hệ thống đăng ký ngay và in phiếu.

Người bệnh cần khám từ hai chuyên khoa trở lên trong cùng một lần đến viện (ví dụ: khám Nội tổng hợp và khám Tai Mũi Họng) hiện phải xử lý bằng một trong hai cách, cả hai đều không đạt:

| Cách người bệnh đang làm | Hệ quả |
|---|---|
| Quét thẻ lại và đăng ký lượt thứ hai tại kiosk | Sinh **hồ sơ điều trị thứ hai** cho cùng một lần đến viện — sai dữ liệu, sai quyết toán BHYT, phải nhờ nhân viên hủy |
| Xếp hàng tại quầy tiếp đón để nhân viên đăng ký hộ | Mất tác dụng của kiosk, quầy tiếp đón vẫn ùn tắc |

Trong khi đó, màn Tiếp đón tại quầy (RegisterV2, phím F4) **đã có sẵn** khả năng chọn nhiều phòng khám và nhiều dịch vụ khám trong cùng một hồ sơ, thông qua nút thêm dòng phòng khám. Kiosk chưa có khả năng tương ứng.

## 1.2 Mục tiêu

Người bệnh chọn được **nhiều công khám trong một lượt đăng ký tại kiosk**, tạo ra **một hồ sơ điều trị duy nhất** với nhiều phiếu khám, mỗi phiếu có số thứ tự riêng của phòng đó, in đủ phiếu cho từng phòng.

Tính năng bật/tắt bằng **một key cấu hình toàn viện**. Bệnh viện không khai báo key thì kiosk hoạt động y hệt hiện tại. Khi bật, không giới hạn số công khám mỗi lượt.

## 1.3 Phạm vi

| Có làm | Không làm |
|---|---|
| Chọn nhiều phòng khám trong một lượt tại kiosk | Thay đổi màn Tiếp đón tại quầy (RegisterV2) |
| Xem lại danh sách đã chọn và bỏ chọn từng phòng | Thay đổi cách đọc thẻ CCCD, thẻ BHYT, nhận diện khuôn mặt |
| Đăng ký một lần cho toàn bộ công khám đã chọn | Thay đổi quy trình khám, chuyển khoa, chỉ định cận lâm sàng |
| In phiếu cho từng công khám | Thay đổi nghiệp vụ chỉ định dịch vụ cận lâm sàng theo phiếu hẹn |
| Key cấu hình bật/tắt tính năng theo từng bệnh viện | Đăng ký khám cho nhiều người bệnh trong một lượt |
| Giữ nguyên bố cục màn hình kiosk hiện tại | Thay đổi cách tính giá, mức hưởng BHYT |
| | Hiển thị tổng tiền dự kiến trên màn hình (xem PHẦN 6, điểm 7) |

## 1.4 Ghi chú về chi phí triển khai

Đã kiểm chứng trên mã nguồn: **phần xử lý phía máy chủ không phải sửa** để phục vụ nhiều công khám. Dịch vụ đăng ký khám qua kiosk dùng chung đúng luồng xử lý với màn Tiếp đón tại quầy, vốn đã hỗ trợ nhiều phòng khám: mỗi phòng sinh một phiếu khám riêng, mỗi phiếu được cấp số thứ tự riêng, toàn bộ nằm trong một giao dịch — lỗi ở bất kỳ công khám nào thì hủy toàn bộ, không tạo dữ liệu dở dang.

Khối lượng công việc do đó tập trung ở **màn hình kiosk**. Chi tiết kỹ thuật nằm ở tài liệu B.

---

# PHẦN 2. THAY ĐỔI TRÊN MÀN HÌNH

Nguyên tắc triển khai: **giữ nguyên bố cục màn hình kiosk hiện tại**, không thêm vùng hiển thị mới, không thêm nút mới. Việc chọn nhiều công khám thực hiện qua chính các ô phòng khám đang có và các hộp thoại xác nhận — cùng cách kiosk đang hỏi người bệnh hiện nay.

Toàn bộ mục 2 dưới đây chỉ áp dụng khi **key cấu hình được bật** (QT-02). Không bật key thì không có bất kỳ thay đổi nào trên màn hình.

## 2.1 Màn chọn phòng khám

| Trường hợp | Trước | Sau (khi bật key) |
|---|---|---|
| Chạm vào ô phòng khám | Đăng ký ngay, in phiếu, kết thúc | Ghi nhận công khám vào danh sách rồi hỏi *"Bạn có muốn chọn thêm phòng khám khác không?"* |
| Trả lời **Không** | Không có | Đăng ký toàn bộ công khám đã chọn, in phiếu, kết thúc |
| Trả lời **Có** | Không có | Quay lại màn chọn phòng để chạm phòng tiếp theo |
| Phòng có nhiều dịch vụ khám | Mở cửa sổ chọn dịch vụ rồi đăng ký ngay | Mở cửa sổ chọn dịch vụ, chọn xong quay về màn chọn phòng và hỏi như trên |
| Chạm lại phòng đã chọn | Không có | Hỏi đăng ký ngay hoặc bỏ chọn phòng đó (2.3) |

## 2.2 Hộp thoại xác nhận sau mỗi lần chọn

Mỗi lần người bệnh chọn xong một công khám, kiosk hiển thị danh sách đã chọn kèm câu hỏi:

```
Bạn đã chọn:
1. Phòng khám Nội tổng hợp - Khám nội tổng hợp
2. Phòng khám Tai Mũi Họng - Khám tai mũi họng

Bạn có muốn chọn thêm phòng khám khác không?

                              [ Có ]   [ Không ]
```

Danh sách này thay cho vùng "công khám đã chọn" trên màn hình — người bệnh vẫn đối chiếu được đầy đủ trước khi đăng ký, mà giao diện kiosk không phải thay đổi.

## 2.3 Khi chạm lại phòng đã chọn

Kiosk hiển thị danh sách đã chọn kèm hai lựa chọn:

```
Bạn đã chọn:
1. Phòng khám Nội tổng hợp - Khám nội tổng hợp
2. Phòng khám Tai Mũi Họng - Khám tai mũi họng

Phòng khám này đã được chọn.
Chọn "Có" để đăng ký, chọn "Không" để bỏ chọn phòng này.

                              [ Có ]   [ Không ]
```

Đây cũng là đường thoát khi đăng ký thất bại: người bệnh chạm lại một phòng đã chọn rồi trả lời "Có" để thử đăng ký lại.

## 2.4 Cửa sổ chọn dịch vụ khám của phòng

Giữ nguyên bố cục và cách hiển thị hiện tại. Khác biệt duy nhất: chạm vào một dịch vụ **không** hỏi *"Bạn có chắc chắn muốn đăng ký khám?"* và **không** đăng ký ngay — cửa sổ đóng lại, dịch vụ được ghi vào danh sách, rồi màn chọn phòng hỏi tiếp theo mục 2.2.

## 2.5 Phiếu in

Mỗi công khám in **một phiếu riêng**, theo đúng mẫu phiếu đang dùng, in liên tiếp nhau. Mỗi phiếu chỉ chứa dịch vụ khám của chính phòng đó cùng số thứ tự của phòng đó.

## 2.6 Thông báo cho người bệnh

| Tình huống | Thông báo |
|---|---|
| Chạm lại phòng đã chọn | "Phòng khám này đã được chọn. Chọn "Có" để đăng ký, chọn "Không" để bỏ chọn phòng này." |
| Chọn dịch vụ khám đã có ở phòng khác | "Dịch vụ khám này đã được chọn ở phòng khác." |
| Phòng không có dịch vụ khám | "Phòng không có dịch vụ nào" (giữ nguyên như hiện tại) |
| Dịch vụ khám vượt giới hạn tuổi | Giữ nguyên thông báo giới hạn tuổi hiện tại, kiểm tra ngay khi chọn |
| Một phòng đã hết lượt khám trong ngày | Giữ nguyên thông báo lỗi từ máy chủ; **danh sách đã chọn được giữ lại** để bỏ chọn phòng đó rồi đăng ký lại |

# PHẦN 3. QUY TẮC NGHIỆP VỤ

Tài liệu kỹ thuật tham chiếu lại các mã QT-xx dưới đây.

## QT-01 — Thế nào là một công khám

Một công khám gồm **đúng một phòng khám và đúng một dịch vụ khám** của phòng đó. Nếu phòng chỉ có một dịch vụ khám thì hệ thống tự lấy dịch vụ đó, không hỏi người bệnh.

## QT-02 — Key cấu hình bật/tắt và số công khám trong một lượt

Tính năng điều khiển bằng một key cấu hình toàn viện:

| Key | `HIS.Desktop.Plugins.RegisterExamKiosk.IsAllowRegisterMultiExam` |
|---|---|
| Không khai báo key, hoặc giá trị `0` | **Tắt** — kiosk hoạt động y hệt hiện tại: chạm ô phòng là đăng ký ngay |
| Khai báo key với giá trị khác `0` (ví dụ `1`) | **Bật** — người bệnh chọn được nhiều công khám trong một lượt |

Khi bật: **không giới hạn** số công khám mỗi lượt. Người bệnh chọn bao nhiêu phòng cũng được; mỗi lần chọn xong một phòng, kiosk hỏi có chọn thêm hay không.

Số lượt khám thực tế vẫn bị chặn bởi giới hạn lượt khám trong ngày của từng phòng (QT-08).

## QT-03 — Không chọn trùng phòng khám

Một phòng khám chỉ được chọn một lần trong cùng một lượt. Chạm lại vào phòng đã chọn dẫn tới hộp thoại đăng ký hoặc bỏ chọn (2.3), không phải thêm lần hai.

Nếu bỏ chọn đúng công khám chính thì công khám được chọn kế tiếp trở thành công khám chính (QT-05).

## QT-04 — Không chọn trùng dịch vụ khám

Hai công khám trong cùng một lượt không được dùng cùng một dịch vụ khám, kể cả khi ở hai phòng khác nhau — vì đó là hai lần thu tiền cho cùng một dịch vụ trong một lần đến viện. Trường hợp này chặn ngay tại kiosk kèm thông báo.

## QT-05 — Công khám chính

Công khám **được chọn đầu tiên** là công khám chính. Nó quyết định:

- Khoa tiếp nhận của hồ sơ điều trị.
- Phiếu khám được đánh dấu là phiếu khám chính.
- Phòng khám ghi nhận là phòng khám đầu tiên của lần điều trị.

Các công khám chọn sau là khám thường, không phải khám chính. Thứ tự này hiển thị cho người bệnh trong hộp thoại xác nhận (2.2).

## QT-06 — Số thứ tự khám

Mỗi công khám được cấp **số thứ tự riêng theo phòng khám của nó**, giống hệt như khi đăng ký từng phòng một. Số thứ tự của phòng A không ảnh hưởng đến số thứ tự của phòng B.

Người bệnh tự sắp xếp thứ tự đi khám giữa các phòng; hệ thống không điều phối thứ tự giữa các phòng.

## QT-07 — Đối tượng thanh toán và phụ thu

Đối tượng thanh toán (BHYT, viện phí, dịch vụ…) và khoản phụ thu đã chọn ở bước trước áp dụng **chung cho toàn bộ công khám** trong lượt. Người bệnh không chọn đối tượng riêng cho từng công khám tại kiosk.

## QT-08 — Giới hạn lượt khám trong ngày của phòng

Bệnh viện có thể cấu hình số lượt khám tối đa trong ngày cho từng phòng. Khi đăng ký, nếu **bất kỳ phòng nào** trong danh sách đã đạt giới hạn thì **toàn bộ lượt đăng ký bị từ chối**, kèm thông báo nêu rõ tên phòng đó (2.5). Người bệnh bỏ chọn phòng đó rồi đăng ký lại.

## QT-09 — Tất cả hoặc không

Việc đăng ký nhiều công khám là **một giao dịch duy nhất**: hoặc tất cả công khám được tạo thành công, hoặc không có công khám nào được tạo và cũng không có hồ sơ điều trị nào được sinh ra. Không tồn tại trạng thái đăng ký được một nửa.

## QT-10 — Thanh toán bằng thẻ khám bệnh

Với bệnh viện dùng thẻ khám bệnh trả trước, số tiền trừ vào thẻ là **tổng tiền của tất cả công khám** trong lượt. Nếu số dư thẻ không đủ, xử lý theo đúng cơ chế hiện hành — thông báo và không đăng ký.

Bản triển khai này **không hiển thị tổng tiền dự kiến** trước khi đăng ký, vì việc đó cần thêm vùng hiển thị mới trên màn hình kiosk. Bệnh viện dùng thẻ trả trước và muốn người bệnh thấy tổng tiền trước khi trừ thẻ thì đây là phần bổ sung tách riêng, cần thống nhất thêm.

## QT-11 — Dịch vụ cận lâm sàng theo phiếu hẹn

Nghiệp vụ hiện có: người bệnh có phiếu hẹn kèm chỉ định cận lâm sàng thì kiosk hỏi có thực hiện không, nếu đồng ý thì chỉ định luôn. Nghiệp vụ này **giữ nguyên**, chạy song song và độc lập với việc chọn nhiều công khám.

## QT-12 — In phiếu

In một phiếu cho mỗi công khám, in liên tiếp. Nếu một phiếu in lỗi, các phiếu còn lại vẫn in — dữ liệu đăng ký đã lưu, việc in lại thực hiện tại quầy như hiện nay.

## QT-13 — Khi key cấu hình tắt

Giữ nguyên **hoàn toàn** hành vi hiện tại: chạm vào phòng là đăng ký ngay, không có hộp thoại hỏi chọn thêm, không có thay đổi nào trên giao diện. Đây là trạng thái mặc định của mọi bệnh viện chưa khai báo key.

Khi key bật, người bệnh chỉ khám một chuyên khoa phải trả lời "Không" ở hộp thoại hỏi chọn thêm để hoàn tất — thêm **một lần chạm** so với trước. Bệnh viện cần cân nhắc điểm này trước khi bật key.

## QT-14 — Bộ đếm tự đóng màn hình

Kiosk hiện tự quay về màn chờ khi người bệnh không thao tác trong một khoảng thời gian. Quy tắc này **giữ nguyên** và áp dụng cả khi người bệnh đang chọn công khám. Mỗi lần chạm chọn hoặc bỏ chọn được tính là một thao tác và làm bộ đếm chạy lại từ đầu.

Khi màn hình tự đóng, danh sách công khám đã chọn bị hủy, **không** đăng ký gì cả.

## QT-15 — Thông tin BHYT và chuyển tuyến

Thông tin thẻ BHYT, đúng tuyến hay trái tuyến, giấy chuyển tuyến là thông tin của **cả lần đến viện**, áp dụng chung cho toàn bộ công khám trong lượt. Không khai báo riêng cho từng công khám.

## QT-16 — Chiều cao, cân nặng

Chiều cao và cân nặng người bệnh nhập ở bước trước được ghi nhận **một lần cho lần điều trị**, không nhân lên theo số công khám.

---

# PHẦN 4. TÌNH HUỐNG SỬ DỤNG

| # | Tình huống | Kết quả mong đợi |
|---|---|---|
| 0 | **Không bật key**. Chạm phòng Nội tổng hợp | Đăng ký ngay, in một phiếu — y hệt hiện tại, không có hộp thoại nào (QT-13) |
| 1 | Bật key. Chọn phòng Nội tổng hợp rồi trả lời "Không" | Kết quả giống hệt đăng ký một công khám như trước: một hồ sơ, một phiếu khám, in một phiếu |
| 2 | Chọn phòng Nội tổng hợp, trả lời "Có", chọn phòng Tai Mũi Họng, trả lời "Không" | Một hồ sơ điều trị, hai phiếu khám, hai số thứ tự của hai phòng, in hai phiếu (QT-06, QT-12) |
| 3 | Chọn ba phòng rồi trả lời "Không" | Một hồ sơ, ba phiếu khám, in ba phiếu — không có giới hạn số lượng (QT-02) |
| 4 | Chọn phòng Nội tổng hợp, trả lời "Có", rồi chạm lại chính ô đó, trả lời "Không" | Bỏ chọn phòng đó, danh sách rỗng, chưa đăng ký gì (QT-03) |
| 5 | Chọn hai phòng có cùng một dịch vụ "Khám nội tổng hợp" | Thông báo trùng dịch vụ, không thêm được (QT-04) |
| 6 | Chọn hai phòng, một phòng đã hết lượt khám trong ngày | Từ chối cả lượt, thông báo nêu tên phòng hết lượt; danh sách được giữ lại để bỏ chọn phòng đó (QT-08, 2.6) |
| 7 | Chọn hai phòng, mất kết nối máy chủ khi đăng ký | Không tạo hồ sơ, không tạo phiếu khám nào, thông báo lỗi, danh sách được giữ lại (QT-09) |
| 8 | Chọn hai phòng, trả lời "Có" rồi bỏ đi | Hết thời gian chờ, màn hình về màn chờ, **không** đăng ký gì (QT-14) |
| 9 | Người bệnh dùng thẻ khám bệnh trả trước, chọn hai công khám | Khi đăng ký trừ đúng tổng tiền của cả hai công khám vào thẻ (QT-10) |
| 10 | Người bệnh có phiếu hẹn kèm chỉ định cận lâm sàng, chọn hai công khám | Vẫn hỏi thực hiện cận lâm sàng như hiện tại; đồng ý thì có cả hai phiếu khám và các chỉ định cận lâm sàng (QT-11) |
| 11 | Chọn hai công khám, phòng đầu tiên là Tai Mũi Họng | Khoa tiếp nhận của hồ sơ là khoa của phòng Tai Mũi Họng; phiếu khám của phòng đó là phiếu chính (QT-05) |
| 12 | Sau khi đăng ký xong, người bệnh mới quét thẻ và chọn phòng | Lượt mới bắt đầu với danh sách rỗng, không dính công khám của lượt trước |

---

# PHẦN 5. TEST CASE

## Nhóm A — Key tắt, chức năng cũ không đổi (QT-13)

- [ ] A1 — Không khai báo key: chạm phòng là đăng ký ngay, không hộp thoại hỏi chọn thêm
- [ ] A2 — Khai báo key giá trị `0`: hành vi giống A1
- [ ] A3 — Phòng có nhiều dịch vụ: vẫn hỏi "Bạn có chắc chắn muốn đăng ký khám?" và đăng ký ngay như cũ
- [ ] A4 — Tốc độ hiển thị danh sách phòng không chậm hơn trước

## Nhóm A2 — Key bật, trường hợp một công khám

- [ ] A2.1 — Chạm phòng, trả lời "Không": đăng ký một công khám, in một phiếu, kết quả giống khi tắt key
- [ ] A2.2 — Phiếu in của một công khám giống hệt phiếu cũ (cùng nội dung, cùng số thứ tự)
- [ ] A2.3 — Đăng ký xong quay về màn chờ, danh sách công khám được xóa sạch

## Nhóm B — Chọn và bỏ chọn công khám

- [ ] B1 — Chạm ô phòng: hộp thoại liệt kê công khám vừa chọn và hỏi chọn thêm (QT-01)
- [ ] B2 — Chạm lại ô đã chọn, trả lời "Không": công khám đó bị xóa khỏi danh sách (QT-03)
- [ ] B3 — Bỏ chọn đúng công khám đầu tiên: công khám kế tiếp trở thành công khám chính (QT-05)
- [ ] B4 — Phòng có nhiều dịch vụ: chọn dịch vụ xong quay lại màn chọn phòng, danh sách ghi đúng dịch vụ đã chọn
- [ ] B5 — Phòng có một dịch vụ: thêm thẳng vào danh sách, không hỏi
- [ ] B6 — Chọn từ 4 phòng trở lên: vẫn thêm được, không bị chặn số lượng khi key bật (QT-02)
- [ ] B7 — Chọn hai phòng cùng dịch vụ khám: thông báo trùng dịch vụ (QT-04)
- [ ] B8 — Bỏ chọn hết công khám rồi chọn lại: đăng ký đúng công khám mới chọn
- [ ] B9 — Thứ tự công khám trong hộp thoại đúng thứ tự chọn (QT-05)

## Nhóm C — Đăng ký thành công

- [ ] C1 — Hai công khám: sinh **một** hồ sơ điều trị, **hai** phiếu khám
- [ ] C2 — Mỗi phiếu khám thuộc đúng phòng và đúng dịch vụ đã chọn
- [ ] C3 — Mỗi phiếu có số thứ tự riêng, đúng dải số của phòng đó (QT-06)
- [ ] C4 — Phiếu của công khám chọn đầu tiên là phiếu khám chính (QT-05)
- [ ] C5 — Khoa tiếp nhận của hồ sơ là khoa của phòng chọn đầu tiên (QT-05)
- [ ] C6 — In đủ hai phiếu, nội dung mỗi phiếu đúng phòng của nó (QT-12)
- [ ] C7 — Ba công khám trở lên: kết quả tương tự, đủ số phiếu
- [ ] C8 — Một công khám: kết quả giống hệt luồng hiện tại
- [ ] C9 — Đối tượng thanh toán và phụ thu giống nhau trên tất cả công khám (QT-07)
- [ ] C10 — Chiều cao, cân nặng ghi nhận đúng một lần cho lần điều trị (QT-16)

## Nhóm D — Đăng ký thất bại, tính toàn vẹn (QT-09)

- [ ] D1 — Ngắt mạng khi chạm "Đăng ký": không sinh hồ sơ, không sinh phiếu khám nào
- [ ] D2 — Sau lỗi, danh sách còn nguyên; chạm lại một phòng đã chọn rồi trả lời "Có" thì đăng ký lại được (2.3)
- [ ] D3 — Thử lại thành công: chỉ sinh **một** hồ sơ, không sinh hồ sơ thừa từ lần lỗi trước
- [ ] D4 — Một phòng vượt giới hạn lượt khám trong ngày: từ chối cả lượt, thông báo nêu tên phòng (QT-08)
- [ ] D5 — Sau lỗi ở D4, bỏ chọn phòng đó rồi đăng ký lại: thành công với các phòng còn lại

## Nhóm E — Thanh toán và BHYT

- [ ] E1 — Tổng tiền trừ thẻ bằng tổng tiền của các công khám đã đăng ký (QT-10)
- [ ] E2 — Thẻ khám bệnh trả trước: số tiền trừ đúng bằng tổng đó
- [ ] E3 — Số dư thẻ không đủ: thông báo, không đăng ký, không trừ tiền
- [ ] E4 — Người bệnh BHYT đúng tuyến: mức hưởng trên các phiếu khám đúng như khi đăng ký một công khám
- [ ] E5 — Người bệnh BHYT trái tuyến: thông tin trái tuyến áp dụng cho tất cả phiếu khám (QT-15)
- [ ] E6 — Người bệnh có giấy chuyển tuyến: thông tin chuyển tuyến ghi nhận một lần cho hồ sơ (QT-15)
- [ ] E7 — Đối chiếu dữ liệu giám định BHYT của hồ sơ hai công khám: không có bản ghi thừa, không trùng dịch vụ

## Nhóm F — Cận lâm sàng theo phiếu hẹn (QT-11)

- [ ] F1 — Có phiếu hẹn kèm chỉ định: vẫn hỏi thực hiện như hiện tại
- [ ] F2 — Đồng ý thực hiện: hồ sơ có đủ các phiếu khám và các chỉ định cận lâm sàng
- [ ] F3 — Chỉ định cận lâm sàng và công khám cùng nằm trong một hồ sơ, không lẫn vào nhau (QT-11)
- [ ] F4 — Từ chối thực hiện: chỉ có các phiếu khám

## Nhóm G — Thời gian chờ và thao tác (QT-14)

- [ ] G1 — Đang chọn công khám, để yên quá thời gian chờ: về màn chờ, không đăng ký gì
- [ ] G2 — Mỗi lần chạm ô phòng làm bộ đếm chạy lại từ đầu
- [ ] G3 — Trả lời "Không" hai lần liên tiếp thật nhanh: chỉ đăng ký **một** lần, không sinh hồ sơ thứ hai
- [ ] G4 — Đăng ký xong, kiosk quay về màn chờ và xóa sạch dữ liệu người bệnh trước đó

## Nhóm H — Giao diện

- [ ] H1 — Hộp thoại xác nhận đọc được ở khoảng cách đứng bình thường trước kiosk
- [ ] H2 — Hộp thoại liệt kê đủ các công khám đã chọn (thử với 5 công khám), không bị cắt chữ
- [ ] H3 — Nút Có/Không của hộp thoại đủ lớn để chạm bằng ngón tay
- [ ] H4 — Nội dung hộp thoại và các thông báo mới hiển thị đúng tiếng Việt có dấu

---

# PHẦN 6. ĐIỂM CẦN CHỐT

Sáu điểm dưới đây cần khách hàng và nghiệp vụ quyết định trước khi lập tài liệu kỹ thuật.

| # | Nội dung | Đề xuất | Trạng thái |
|---|---|---|---|
| 1 | Bật/tắt tính năng và số công khám mỗi lượt | Key `HIS.Desktop.Plugins.RegisterExamKiosk.IsAllowRegisterMultiExam` — không khai báo là tắt; bật thì không giới hạn số công khám (QT-02, QT-13) | Đã chốt theo yêu cầu |
| 2 | Chặn trùng dịch vụ khám giữa hai phòng | **Chặn** tại kiosk (QT-04) | Đã làm theo đề xuất |
| 3 | Công khám chính khi có nhiều công khám | Lấy công khám **chọn đầu tiên** (QT-05) | Đã làm theo đề xuất |
| 4 | Một phòng hết lượt khám trong ngày | Từ chối **cả lượt**, người bệnh bỏ chọn phòng đó rồi đăng ký lại (QT-08) | Đã làm theo đề xuất |
| 5 | Phụ thu cho công khám thứ hai trở đi | Áp dụng **cùng phụ thu** với công khám chính (QT-07). Cần xác nhận với bệnh viện chạy chế độ phụ thu tự động theo dịch vụ | **Chờ chốt** |
| 6 | Nhiều phiếu khám BHYT trong một lần đến viện tại kiosk | Chấp nhận — quầy tiếp đón đã làm như vậy từ trước. Cần xác nhận không vướng giám định | **Chờ chốt** |
| 7 | Hiển thị tổng tiền dự kiến trước khi đăng ký | Chưa làm, vì cần thêm vùng hiển thị mới trên màn hình kiosk (QT-10) | **Chờ chốt** |

Bốn điểm đầu đã được triển khai. Ba điểm còn lại chờ khách hàng quyết định — điểm 5 và 6 là xác nhận nghiệp vụ, điểm 7 là phần bổ sung giao diện nếu bệnh viện dùng thẻ khám bệnh trả trước.

Vì tính năng nằm sau key cấu hình, các bệnh viện đang chạy không bị ảnh hưởng cho tới khi khai báo key.
