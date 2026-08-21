# Việc 53180 — TÀI LIỆU NGHIỆP VỤ

## Bổ sung lọc theo bác sĩ chỉ định và tạo văn bản tại màn Tra soát hồ sơ bệnh án

| Thông tin | Nội dung |
|---|---|
| Mã việc | 53180 |
| Chức năng | Tra soát hồ sơ bệnh án |
| Người dùng | Bác sĩ điều trị, bác sĩ chỉ định, phòng KHTH, phòng CNTT |
| Tài liệu gốc | PhanTich_Loc_BacSiChiDinh_TraSoat_HoSoBenhAn.docx |
| Tài liệu đi kèm | 53180_B_KyThuat_TraSoatHoSoBenhAn.docx (dành cho lập trình viên) |
| Trạng thái | Chờ duyệt |

**Tài liệu này dành cho**: khách hàng, người phân tích nghiệp vụ, kiểm thử viên.
Tài liệu **không** chứa nội dung lập trình. Phần kỹ thuật nằm ở tài liệu B.

---

# PHẦN 1. BỐI CẢNH VÀ MỤC TIÊU

## 1.1 Vấn đề hiện tại

Màn hình Tra soát hồ sơ bệnh án hiện chỉ làm việc được với **một hồ sơ tại một thời điểm**: người dùng phải nhập mã hồ sơ, xem xong rồi mới nhập mã hồ sơ tiếp theo.

Điều này gây ra 4 khó khăn:

1. Bác sĩ không tổng hợp được toàn bộ y lệnh mình đã kê cho các bệnh nhân đang điều trị.
2. Muốn biết y lệnh nào chưa có văn bản, y lệnh nào chưa ký xong thì phải mở lần lượt từng hồ sơ — mất thời gian, dễ bỏ sót.
3. Không khoanh vùng được theo khoảng thời gian, cũng không tách được hồ sơ đã kết thúc và chưa kết thúc.
4. Khi phát hiện thiếu văn bản, phải thoát ra, mở chức năng khác để tạo — không xử lý ngay tại chỗ.

Hệ quả: hồ sơ bệnh án dễ thiếu văn bản, thiếu chữ ký; khó theo dõi tiến độ hoàn thiện bệnh án của từng bác sĩ.

## 1.2 Mục tiêu

Bổ sung ba nhóm chức năng:

| Nhóm | Nội dung |
|---|---|
| **A. Bộ lọc mới** | Lọc y lệnh theo bác sĩ chỉ định, khoảng thời gian, trạng thái hồ sơ |
| **B. Hiển thị trạng thái** | Mỗi y lệnh thể hiện rõ: đã/chưa tạo văn bản, đã ký hết/chưa ký hết. Có ô lọc nhanh |
| **C. Tạo văn bản** | Tạo văn bản cho y lệnh chưa có văn bản, ngay trên màn tra soát |

## 1.3 Phạm vi

| | Nội dung |
|---|---|
| **Có làm** | Thêm bộ lọc; thêm cột trạng thái và cột thời gian tạo; thêm 2 ô lọc nhanh; thêm nút Tạo văn bản |
| **Không làm** | Tạo văn bản hàng loạt cho nhiều y lệnh cùng lúc |
| **Không làm** | Ký hàng loạt cho nhiều văn bản cùng lúc |
| **Không làm** | Thay đổi bất kỳ thao tác tra soát nào đang có |

---

# PHẦN 2. MÔ TẢ MÀN HÌNH

## 2.1 Bố cục sau khi bổ sung

```
┌────────────────────────────────────────────────────────────────────────────────┐
│ [Mã hồ sơ ____] Bác sĩ [▼....] Từ [__/__/__] Đến [__/__/__] TT hồ sơ [▼]  [Tìm]│
├────────────────────────────────────────────────────────────────────────────────┤
│  ▸ Thông tin bệnh nhân        (giữ nguyên như hiện tại)                         │
├────────────────────────────────────────────────────────────────────────────────┤
│  ☑ Ưu tiên có dữ liệu   ☐ Tôi tạo   ☐ Bao gồm văn bản hủy                      │
│  ☐ Chưa tạo văn bản   ☐ Chưa ký hết                                            │  ← MỚI
├──────────────────┬──────────────────────────────┬──────────────────────────────┤
│  Loại văn bản    │  Danh sách y lệnh            │  Danh sách văn bản           │
│  (giữ nguyên)    │  (thêm cột — xem 2.3)        │  (giữ nguyên)                │
│                  │  … mỗi dòng có nút [Tạo]     │                              │  ← MỚI
│                  ├──────────────────────────────┤                              │
│                  │  ◀ ◀ 1/5 ▶ ▶   (phân trang)  │                              │  ← MỚI, chỉ Cách 2
├──────────────────┴──────────────────────────────┴──────────────────────────────┤
│                          [Không đạt] [Đạt] [Duyệt] [Hủy duyệt]                  │
└────────────────────────────────────────────────────────────────────────────────┘
```

Toàn bộ điều kiện tìm nằm trên **một hàng duy nhất**, xếp theo đúng thứ tự thao tác:
mã hồ sơ → bác sĩ → khoảng thời gian → trạng thái hồ sơ → **nút Tìm ở ngoài cùng bên phải**.
Người dùng nhập một mạch từ trái sang phải rồi bấm Tìm một lần ở cuối.
Nút tạo văn bản **không phải nút riêng ở cuối màn hình** mà nằm **trên từng dòng** của bảng y lệnh — xem mục 2.3.

## 2.2 Các ô nhập mới

| Ô nhập | Kiểu | Giá trị mặc định | Diễn giải |
|---|---|---|---|
| **Bác sĩ chỉ định** | Danh sách chọn | Để trống | Chọn bác sĩ cần tra soát. Có nút xóa để quay lại cách tra soát cũ |
| **Thời gian (từ – đến)** | Ngày | Từ đầu tháng đến hôm nay | Khoảng thời gian tra soát |
| **Trạng thái hồ sơ** | Danh sách chọn | Chưa kết thúc | Hai lựa chọn: *Chưa kết thúc* / *Đã kết thúc* |
| **Chưa tạo văn bản** | Ô tích | Bỏ tích | Lọc nhanh: chỉ hiện y lệnh chưa có văn bản |
| **Chưa ký hết** | Ô tích | Bỏ tích | Lọc nhanh: chỉ hiện y lệnh chưa hoàn thành chữ ký |

## 2.3 Cột mới ở bảng danh sách y lệnh

| Cột | Nội dung | Ghi chú |
|---|---|---|
| **Tạo VB** | Nút **[Tạo]** ngay trên dòng | Chỉ hiện ở dòng đủ điều kiện tạo — xem QT-14 |
| Mã bệnh nhân | Mã của bệnh nhân | Chỉ hiện khi tra soát theo bác sĩ |
| Tên bệnh nhân | Họ tên bệnh nhân | Chỉ hiện khi tra soát theo bác sĩ |
| Mã hồ sơ | Mã hồ sơ điều trị | Chỉ hiện khi tra soát theo bác sĩ |
| **Thời gian tạo** | Thời điểm bản ghi được tạo trong hệ thống | Luôn hiện — xem giải thích ở mục 2.4 |
| **Trạng thái văn bản** | Chưa tạo văn bản / Chưa ký / Đang ký / Hoàn thành | Luôn hiện |

Nút đặt trên dòng thay vì đặt riêng ở cuối màn hình: người dùng bấm đúng dòng cần tạo, không phải chọn dòng rồi tìm nút.
Dòng không đủ điều kiện thì **ô để trống**, không hiện nút mờ.

## 2.4 Vì sao cần thêm cột "Thời gian tạo"

Cột thời gian hiện có trên màn hình đang hiển thị **thời gian nghiệp vụ**, và thời gian nghiệp vụ của mỗi loại văn bản là khác nhau:

| Loại văn bản | Cột thời gian hiện có đang hiển thị |
|---|---|
| Phiếu chỉ định, Phiếu kết quả, Đơn thuốc | Thời gian ra y lệnh |
| Biên bản hội chẩn | Thời gian hội chẩn |
| Phiếu truyền dịch | Thời gian bắt đầu – kết thúc truyền |
| Phản ứng thuốc | Thời gian thực hiện |
| Tờ điều trị | Thời gian theo dõi |
| Truyền máu | Thời gian đo |
| Phiếu chăm sóc | Thời gian tạo |

Bộ lọc thời gian mới chạy theo **thời gian tạo bản ghi** (trừ nhóm y lệnh chạy theo thời gian ra y lệnh — xem QT-05). Nếu chỉ có một cột, người dùng lọc ngày 01/08 nhưng thấy dòng ghi 31/07 sẽ tưởng phần mềm sai.

Vì vậy màn hình hiển thị **hai cột thời gian riêng biệt**, người dùng đối chiếu được ngay.

---

# PHẦN 3. HAI CÁCH TRA SOÁT

Màn hình có hai cách làm việc. Cách nào được dùng là do ô **Bác sĩ chỉ định** quyết định.

## 3.1 Cách 1 — Tra soát theo hồ sơ (mặc định, giữ nguyên như hiện tại)

**Khi nào**: ô Bác sĩ chỉ định để trống.

**Cách dùng**: nhập hoặc quét mã hồ sơ → bấm Tìm → xem thông tin bệnh nhân, danh sách y lệnh và văn bản của hồ sơ đó.

**Đặc điểm**:
- Hoạt động **y hệt hiện tại**, không thay đổi gì.
- Ba ô lọc mới (bác sĩ, thời gian, trạng thái hồ sơ) không có tác dụng và bị làm mờ.
- Các nút Đạt / Không đạt / Duyệt / Hủy duyệt hoạt động bình thường.
- Có thêm: cột Thời gian tạo, cột Trạng thái văn bản, nút **[Tạo]** trên từng dòng.

## 3.2 Cách 2 — Tra soát theo bác sĩ chỉ định (mới)

**Khi nào**: ô Bác sĩ chỉ định có chọn người.

**Cách dùng**: chọn bác sĩ → chọn khoảng thời gian → chọn trạng thái hồ sơ → bấm Tìm.

**Đặc điểm**:
- Hiển thị **tất cả y lệnh** của bác sĩ đó, trên **tất cả hồ sơ** thỏa điều kiện.
- Ô mã hồ sơ bị khóa (không dùng đến).
- Ba cột Mã bệnh nhân / Tên bệnh nhân / Mã hồ sơ được hiện thêm.
- Vùng thông tin bệnh nhân phía trên để trống, vì đang xem nhiều hồ sơ.
- Các nút Đạt / Không đạt / Duyệt / Hủy duyệt bị khóa — đây là thao tác chốt từng hồ sơ, chỉ làm ở Cách 1.
- Kết quả được chia trang, **đếm theo hồ sơ** — xem QT-21.
- Nháy đúp vào một dòng → hệ thống nạp mã hồ sơ của dòng đó và chuyển về Cách 1 cho hồ sơ ấy.
- Thứ tự dòng: nhóm theo từng hồ sơ, trong mỗi hồ sơ xếp theo nhóm loại y lệnh. Hồ sơ xếp theo thời gian tạo giảm dần.

## 3.3 Bảng so sánh nhanh

| Nội dung | Cách 1 — Theo hồ sơ | Cách 2 — Theo bác sĩ |
|---|---|---|
| Ô Bác sĩ chỉ định | Trống | Có chọn |
| Ô mã hồ sơ | Dùng | Bị khóa |
| Thời gian từ – đến | Không dùng | **Bắt buộc nhập** |
| Trạng thái hồ sơ | Không dùng | **Bắt buộc chọn** |
| Thông tin bệnh nhân | Hiển thị | Để trống |
| Cột Mã BN / Tên BN / Mã hồ sơ | Ẩn | Hiện |
| Nút Đạt / Không đạt / Duyệt / Hủy duyệt | Dùng được | Bị khóa |
| Nút Tạo văn bản | Dùng được | Dùng được |
| Chia trang | Không | Có |

---

# PHẦN 4. QUY TẮC NGHIỆP VỤ

> Mỗi quy tắc có một mã `QT-xx`. Tài liệu kỹ thuật và các test case đều tham chiếu theo mã này.

## 4.1 Nhóm bộ lọc

| Mã | Quy tắc |
|---|---|
| **QT-01** | Ô Bác sĩ chỉ định **không bắt buộc**. Để trống → tra soát theo hồ sơ như hiện tại |
| **QT-02** | Khi đã chọn bác sĩ, ô **Thời gian từ – đến** và ô **Trạng thái hồ sơ** trở thành **bắt buộc**. Thiếu một trong hai → hệ thống chặn, báo lỗi ngay tại ô thiếu, không tìm kiếm |
| **QT-03** | Ngày bắt đầu không được lớn hơn ngày kết thúc |
| **QT-04** | Khoảng thời gian vượt quá 31 ngày → hệ thống hỏi xác nhận trước khi tìm |
| **QT-05** | Cách xác định y lệnh thuộc về bác sĩ và thuộc khoảng thời gian: xem bảng 4.2 |
| **QT-06** | Trạng thái hồ sơ *Đã kết thúc* = hồ sơ **có** thời gian kết thúc điều trị. *Chưa kết thúc* = hồ sơ **chưa có** thời gian kết thúc |
| **QT-07** | Bộ lọc loại văn bản ở cột trái vẫn hoạt động bình thường ở cả hai cách tra soát |
| **QT-08** | Hai ô lọc nhanh *Chưa tạo văn bản* và *Chưa ký hết* có thể tích cùng lúc — khi đó chỉ còn y lệnh chưa có văn bản |
| **QT-21** | Cách 2 chia trang theo **hồ sơ**, không theo y lệnh. Một hồ sơ sinh nhiều dòng y lệnh nên số dòng hiển thị **nhiều hơn** số bản ghi mỗi trang. Đổi loại văn bản ở cột trái chỉ lọc lại trong trang hiện tại, **không** gọi lại máy chủ và **không** đổi số trang |
| **QT-22** | Ô tích *Ưu tiên có dữ liệu* đẩy lên đầu mọi loại văn bản **có dữ liệu ở bảng giữa** — nghĩa là có văn bản **hoặc** có y lệnh. Loại có y lệnh nhưng chưa tạo văn bản vẫn được đẩy lên, vì đó chính là nhóm cần tra soát nhất |

## 4.2 Cách xác định bác sĩ và thời gian theo từng loại (QT-05)

| Loại văn bản | Coi là "của bác sĩ" khi | Lọc thời gian theo |
|---|---|---|
| Phiếu chỉ định | Bác sĩ là **người chỉ định** y lệnh | Thời gian ra y lệnh |
| Phiếu kết quả | Bác sĩ là **người chỉ định** y lệnh | Thời gian ra y lệnh |
| Đơn thuốc | Bác sĩ là **người chỉ định** y lệnh | Thời gian ra y lệnh |
| Phiếu chăm sóc | Bác sĩ là **người tạo** bản ghi | Thời gian tạo |
| Biên bản hội chẩn | Bác sĩ là **người tạo** bản ghi | Thời gian tạo |
| Phiếu truyền dịch | Bác sĩ là **người tạo** bản ghi | Thời gian tạo |
| Phản ứng thuốc | Bác sĩ là **người tạo** bản ghi | Thời gian tạo |
| Tờ điều trị | Bác sĩ là **người tạo** bản ghi | Thời gian tạo |
| Truyền máu | Bác sĩ là **người tạo** bản ghi | Thời gian tạo |

> Chỉ ba loại đầu có khái niệm "người chỉ định". Sáu loại còn lại lấy theo người tạo bản ghi.

## 4.3 Nhóm trạng thái y lệnh

| Mã | Quy tắc |
|---|---|
| **QT-09** | Mỗi y lệnh có đúng một trong bốn trạng thái: *Chưa tạo văn bản*, *Chưa ký*, *Đang ký*, *Hoàn thành* |
| **QT-10** | Một y lệnh có thể sinh ra **nhiều văn bản**. Trạng thái *Hoàn thành* chỉ đạt được khi **tất cả** văn bản của y lệnh đó đã ký đủ |
| **QT-11** | Ô lọc *Chưa tạo văn bản* → chỉ hiện y lệnh ở trạng thái *Chưa tạo văn bản* |
| **QT-12** | Ô lọc *Chưa ký hết* → hiện y lệnh ở trạng thái *Chưa tạo văn bản*, *Chưa ký*, *Đang ký* (tất cả trừ *Hoàn thành*) |

### Bảng trạng thái chi tiết (QT-09)

| Trạng thái | Điều kiện | Màu ô | Ý nghĩa với người dùng |
|---|---|---|---|
| **Chưa tạo văn bản** | Y lệnh chưa sinh văn bản nào | Đen | Cần tạo văn bản |
| **Chưa ký** | Có văn bản nhưng chưa ai ký | Đen | Cần ký |
| **Đang ký** | Đã ký một phần, còn người chưa ký | Vàng | Chờ người còn lại ký |
| **Hoàn thành** | Tất cả văn bản đã ký đủ | Xanh | Không cần làm gì thêm |

### Lưu ý quan trọng — thay đổi so với hiện tại

Phần mềm hiện tại đánh giá theo kiểu: **chỉ cần một** văn bản ký xong là báo y lệnh đã hoàn thành.
Yêu cầu mới là "đã ký **hết**", nên phải đổi thành: **tất cả** văn bản ký xong mới là hoàn thành.

> **Ví dụ**: Y lệnh X sinh ra 2 văn bản. Văn bản 1 đã ký xong, văn bản 2 chưa ký.
> - Phần mềm **hiện tại** hiển thị: *Hoàn thành* → sai so với nghiệp vụ mới
> - Phần mềm **sau khi sửa** hiển thị: *Đang ký* → đúng

Đây là **thay đổi hành vi hiển thị đang chạy**. Kiểm thử cần kiểm tra riêng tình huống này.

## 4.4 Nhóm tạo văn bản

| Mã | Quy tắc |
|---|---|
| **QT-13** | Chỉ tạo văn bản cho **một y lệnh** — y lệnh của đúng dòng bấm nút. Không tạo hàng loạt |
| **QT-14** | Nút **[Tạo]** chỉ hiện ở dòng có trạng thái **Chưa tạo văn bản**. Dòng đã có văn bản → **ô để trống**, không hiện nút |
| **QT-15** | Biểu mẫu để tạo văn bản lấy từ cấu hình *Loại văn bản* trong danh mục Biểu in. Loại văn bản nào gắn biểu mẫu nào thì chỉ hiện đúng những biểu mẫu đó |
| **QT-16** | Loại văn bản chưa gắn biểu mẫu nào → thông báo cho người dùng, không báo lỗi hệ thống |
| **QT-16b** | **Chưa đạt được (11/08/2026).** Biểu mẫu đã gắn nhưng phần mềm chưa dựng được dữ liệu cho mẫu đó → hiện **không có thông báo nào**, màn hình đứng im sau khi chọn mẫu. Xem mục 9.2 |
| **QT-17** | Loại văn bản gắn đúng **một** biểu mẫu → dùng luôn, không hỏi |
| **QT-18** | Loại văn bản gắn **nhiều** biểu mẫu → hiện danh sách để người dùng chọn |
| **QT-19** | Tạo văn bản xong → danh sách tự làm mới, trạng thái y lệnh cập nhật ngay |
| **QT-20** | Người dùng hủy giữa chừng → không sinh văn bản, trạng thái giữ nguyên |

---

# PHẦN 5. HƯỚNG DẪN SỬ DỤNG THEO TÌNH HUỐNG

## Tình huống 1 — Bác sĩ xem toàn bộ y lệnh của mình trong tháng

1. Mở chức năng Tra soát hồ sơ bệnh án.
2. Ở ô **Bác sĩ chỉ định**, chọn chính mình.
3. Chọn **Thời gian**: từ ngày 01 đến ngày hiện tại.
4. Chọn **Trạng thái hồ sơ**: *Chưa kết thúc*.
5. Bấm **Tìm**.

→ Hệ thống hiển thị toàn bộ y lệnh bác sĩ đã kê trong khoảng đó, kèm cột trạng thái văn bản của từng y lệnh.

## Tình huống 2 — Tìm nhanh những y lệnh còn thiếu văn bản

Sau khi đã lọc như Tình huống 1:

6. Tích ô **Chưa tạo văn bản**.

→ Danh sách chỉ còn các y lệnh chưa có văn bản.

## Tình huống 3 — Tạo văn bản cho một y lệnh

7. Tìm dòng y lệnh cần xử lý — dòng nào tạo được sẽ có nút **[Tạo]** ở cột đầu.
8. Bấm nút **[Tạo]** ngay trên dòng đó.
9. Nếu loại văn bản có nhiều biểu mẫu, chọn biểu mẫu phù hợp.
10. Kiểm tra nội dung, ký và đóng cửa sổ.

→ Danh sách tự làm mới, trạng thái y lệnh chuyển khỏi *Chưa tạo văn bản*.

## Tình huống 4 — Phòng KHTH rà hồ sơ đã kết thúc

1. Chọn **Bác sĩ chỉ định** cần rà.
2. Chọn **Thời gian** cần rà.
3. Chọn **Trạng thái hồ sơ**: *Đã kết thúc*.
4. Tích ô **Chưa ký hết**.

→ Hiển thị các y lệnh còn thiếu văn bản hoặc thiếu chữ ký của những hồ sơ đã kết thúc, để bổ sung hoàn thiện bệnh án.

## Tình huống 5 — Quay lại cách tra soát cũ

Xóa trắng ô **Bác sĩ chỉ định** → màn hình trở về cách tra soát theo mã hồ sơ như trước.

---

# PHẦN 6. THÔNG BÁO HIỂN THỊ CHO NGƯỜI DÙNG

| Tình huống | Nội dung thông báo | Kiểu |
|---|---|---|
| Chưa nhập thời gian khi đã chọn bác sĩ | *Trường dữ liệu bắt buộc* — hiện ngay tại ô | Cảnh báo tại chỗ |
| Chưa chọn trạng thái hồ sơ khi đã chọn bác sĩ | *Trường dữ liệu bắt buộc* — hiện ngay tại ô | Cảnh báo tại chỗ |
| Ngày bắt đầu lớn hơn ngày kết thúc | *Từ ngày phải nhỏ hơn hoặc bằng Đến ngày* | Cảnh báo tại chỗ |
| Khoảng thời gian quá 31 ngày | *Khoảng thời gian tra soát vượt quá 31 ngày, có thể mất nhiều thời gian. Bạn có muốn tiếp tục?* | Hỏi Có / Không |
| Loại văn bản chưa cấu hình biểu mẫu | *Loại văn bản này chưa được cấu hình biểu mẫu in.* | Thông báo |
| Biểu mẫu chưa được hỗ trợ | *Biểu mẫu này chưa được hỗ trợ tạo văn bản từ màn tra soát.* | Thông báo |
| Không có kết quả | Danh sách trống (không cần thông báo) | — |

> Toàn bộ thông báo và nhãn trên màn hình phải có đủ tiếng Việt và tiếng Anh.

---

# PHẦN 7. NHỮNG GÌ KHÔNG ĐƯỢC THAY ĐỔI

Đây là danh sách bắt buộc kiểm thử lại để chắc chắn chức năng cũ không bị ảnh hưởng:

1. Tra soát theo mã hồ sơ — nhập mã, bấm Tìm, xem kết quả.
2. Danh sách loại văn bản ở cột trái và cách tô màu loại văn bản.
3. Ba ô tích cũ: *Ưu tiên có dữ liệu*, *Tôi tạo*, *Bao gồm văn bản hủy*.
4. Bốn nút: *Đạt*, *Không đạt*, *Duyệt*, *Hủy duyệt* — kể cả điều kiện bật/tắt theo quyền và theo trạng thái hồ sơ.
5. Xem và ký văn bản ở bảng bên phải.
6. Mở màn hình từ chức năng khác có truyền danh sách hồ sơ.
7. Ghi nhớ trạng thái các ô tích khi đóng và mở lại phần mềm.

---

# PHẦN 8. TEST CASE

## Nhóm A — Kiểm tra chức năng cũ không bị ảnh hưởng

| Mã | Bước thực hiện | Kết quả mong đợi |
|---|---|---|
| TC-A1 | Mở màn hình, không đụng ô bác sĩ. Nhập mã hồ sơ → Tìm | Hiển thị giống hệt phiên bản cũ |
| TC-A2 | Chọn lần lượt từng loại văn bản ở cột trái | Bảng giữa và bảng phải đổi đúng như cũ |
| TC-A3 | Bấm Đạt / Không đạt / Duyệt / Hủy duyệt | Hoạt động như cũ, trạng thái hồ sơ đổi đúng |
| TC-A4 | Tích lần lượt 3 ô tích cũ | Kết quả như cũ |
| TC-A5 | Mở màn hình từ chức năng khác có truyền danh sách hồ sơ | Bảng danh sách hồ sơ hiện như cũ |
| TC-A6 | Đóng phần mềm, mở lại | Trạng thái các ô tích được nhớ đúng |

## Nhóm B — Bộ lọc theo bác sĩ

| Mã | Quy tắc | Bước thực hiện | Kết quả mong đợi |
|---|---|---|---|
| TC-B1 | QT-02 | Chọn bác sĩ, để trống Từ ngày → Tìm | Báo lỗi ngay tại ô Từ ngày, không tìm kiếm |
| TC-B2 | QT-02 | Chọn bác sĩ, để trống Trạng thái hồ sơ → Tìm | Báo lỗi ngay tại ô Trạng thái, không tìm kiếm |
| TC-B3 | QT-03 | Nhập Từ ngày lớn hơn Đến ngày → Tìm | Báo lỗi *Từ ngày phải nhỏ hơn hoặc bằng Đến ngày* |
| TC-B4 | QT-04 | Chọn khoảng thời gian 60 ngày → Tìm | Hiện câu hỏi xác nhận; chọn Không thì dừng |
| TC-B5 | QT-01 | Chọn bác sĩ + đủ thời gian + trạng thái → Tìm | Hiện tất cả y lệnh của bác sĩ trong khoảng đó |
| TC-B6 | 3.2 | Quan sát màn hình khi đang lọc theo bác sĩ | Ô mã hồ sơ bị khóa; 4 nút Đạt/Không đạt/Duyệt/Hủy duyệt bị khóa |
| TC-B7 | 2.3 | Quan sát bảng y lệnh khi đang lọc theo bác sĩ | Có cột Mã BN, Tên BN, Mã hồ sơ |
| TC-B8 | 3.3 | Xóa trắng ô bác sĩ | Về lại cách tra soát cũ, ô mã hồ sơ mở lại |
| TC-B9 | QT-06 | Chọn Trạng thái hồ sơ = *Đã kết thúc* | Chỉ hiện y lệnh của hồ sơ **có** thời gian kết thúc |
| TC-B10 | QT-06 | Chọn Trạng thái hồ sơ = *Chưa kết thúc* | Chỉ hiện y lệnh của hồ sơ **chưa có** thời gian kết thúc |
| TC-B11 | 3.2 | Nháy đúp vào một dòng | Nạp mã hồ sơ của dòng đó, chuyển về cách tra soát theo hồ sơ |
| TC-B12 | 3.2 | Lọc ra kết quả nhiều trang, chuyển trang | ⚠️ **Chưa kiểm được — phân trang chưa làm.** Xem ghi chú cuối nhóm B |
| TC-B13 | QT-05 | Lọc theo bác sĩ A, kiểm tra cột Người chỉ định | Nhóm Phiếu chỉ định/Kết quả/Đơn thuốc: người chỉ định là A. Các loại khác: người tạo là A |
| TC-B14 | QT-07 | Đang lọc theo bác sĩ, đổi loại văn bản ở cột trái | Danh sách lọc lại đúng theo loại văn bản đã chọn |
| TC-B15 | QT-21 | Lọc ra 1 hồ sơ có 8 y lệnh, cỡ trang 50 | ⚠️ **Chưa kiểm được — phân trang chưa làm.** Khi làm xong: thanh phân trang báo **1 bản ghi**, bảng giữa hiện **8 dòng** — không phải lỗi |
| TC-B16 | QT-21 | Đổi loại văn bản ở cột trái khi đang ở trang 2 | ⚠️ **Chưa kiểm được — phân trang chưa làm.** Khi làm xong: vẫn ở trang 2, số trang không đổi, không có thời gian chờ tải lại |
| TC-B17 | QT-22 | Bác sĩ có y lệnh Phiếu chỉ định **chưa tạo văn bản** nào, tích *Ưu tiên có dữ liệu* | Loại *Phiếu chỉ định* được đẩy lên đầu cột trái |
| TC-B18 | QT-22 | Bỏ tích *Ưu tiên có dữ liệu* | Cột trái xếp lại theo thứ tự cấu hình, không theo dữ liệu |
| TC-B19 | QT-09 | Lọc theo bác sĩ, **chưa nháy vào dòng nào** | Cột người ký / người chưa ký đã có tên luồng ký sẵn, không phải nháy từng dòng |
| TC-B20 | QT-09 | So sánh một hồ sơ ở cả hai cách tra soát | Cùng một hồ sơ phải ra **cùng danh sách y lệnh và cùng trạng thái văn bản** ở cả hai cách, nhất là trạng thái *Chưa ký* |

> **Ghi chú phân trang (TC-B12, TC-B15, TC-B16)**
> Phân trang **chưa được làm**: máy chủ chưa cắt trang và chưa trả tổng số bản ghi. Hiện thanh phân
> trang báo tổng bằng 0 và chuyển trang vẫn ra cùng một danh sách. Màn hình vẫn dùng bình thường vì
> toàn bộ kết quả được tải một lần, nhưng **khoảng thời gian tra soát rộng sẽ tải chậm**.
> Ba trường hợp kiểm thử trên tạm gác lại, không tính là lỗi mới.

## Nhóm C — Trạng thái y lệnh

| Mã | Quy tắc | Tình huống dữ liệu | Kết quả mong đợi |
|---|---|---|---|
| TC-C1 | QT-09 | Y lệnh chưa có văn bản nào | Trạng thái *Chưa tạo văn bản*, ô đen |
| TC-C2 | QT-09 | Có văn bản, chưa ai ký | Trạng thái *Chưa ký* |
| TC-C3 | **QT-10** | Y lệnh có 2 văn bản: 1 đã ký xong, 1 chưa ký | Trạng thái **Đang ký** (không phải Hoàn thành) — **điểm khác bản cũ** |
| TC-C4 | QT-10 | Y lệnh có 2 văn bản, cả 2 đã ký đủ | Trạng thái *Hoàn thành*, ô xanh |
| TC-C5 | QT-11 | Tích ô *Chưa tạo văn bản* | Chỉ còn dòng TC-C1 |
| TC-C6 | QT-12 | Tích ô *Chưa ký hết* | Còn dòng TC-C1, TC-C2, TC-C3 |
| TC-C7 | QT-08 | Tích cả hai ô lọc nhanh | Chỉ còn dòng TC-C1 |
| TC-C8 | QT-12 | Sau khi mọi y lệnh đã tạo và ký đủ, tích *Chưa ký hết* | Danh sách trống |

## Nhóm D — Cột thời gian

| Mã | Quy tắc | Bước thực hiện | Kết quả mong đợi |
|---|---|---|---|
| TC-D1 | 2.4 | Chọn loại *Phiếu chỉ định*, xem 2 cột thời gian | Cột thời gian chỉ định = giờ ra y lệnh; cột Thời gian tạo = giờ tạo bản ghi |
| TC-D2 | 2.4 | Chọn loại *Biên bản hội chẩn*, xem 2 cột thời gian | Cột thời gian = giờ hội chẩn; cột Thời gian tạo = giờ tạo. Hai giá trị có thể khác nhau |
| TC-D3 | QT-05 | Lọc theo khoảng ngày, kiểm tra cột Thời gian tạo | Mọi dòng đều nằm trong khoảng đã lọc |

## Nhóm E — Tạo văn bản

| Mã | Quy tắc | Bước thực hiện | Kết quả mong đợi |
|---|---|---|---|
| TC-E1 | QT-14 | Dòng y lệnh **đã có** văn bản | Ô cột *Tạo VB* để trống, không có nút |
| TC-E2 | QT-14 | Dòng y lệnh **chưa có** văn bản | Có nút **[Tạo]**, bấm được |
| TC-E3 | QT-13 | Bấm **[Tạo]** ở dòng thứ 5 trong khi đang đứng ở dòng thứ 2 | Tạo văn bản cho y lệnh của **dòng thứ 5** |
| TC-E4 | QT-16 | Chọn loại văn bản chưa cấu hình biểu mẫu → bấm **[Tạo]** | Thông báo *chưa được cấu hình biểu mẫu in*, không lỗi |
| TC-E5 | QT-17 | Loại văn bản có đúng 1 biểu mẫu | Vào thẳng cửa sổ tạo văn bản, không hỏi |
| TC-E6 | QT-18 | Loại văn bản có nhiều biểu mẫu | Hiện danh sách biểu mẫu để chọn |
| TC-E7 | QT-16 | Chọn biểu mẫu chưa được hỗ trợ | Thông báo *chưa được hỗ trợ*, không lỗi hệ thống |
| TC-E11 | **QT-16b** | Gắn cho loại văn bản một biểu mẫu phần mềm chưa dựng được (VD `Mps000033`) → bấm **[Tạo]** → chọn mẫu đó | **Hiện tại: FAIL** — màn hình đứng im, không thông báo. Sau khi chốt hướng ở mục 9.2 phải có phản hồi rõ ràng |
| TC-E8 | QT-19 | Tạo văn bản và ký thành công | Danh sách tự làm mới, trạng thái chuyển khỏi *Chưa tạo văn bản* |
| TC-E9 | QT-20 | Bấm Tạo văn bản rồi đóng cửa sổ giữa chừng | Không sinh văn bản, trạng thái giữ nguyên |
| TC-E10 | QT-13 | Tạo văn bản khi đang lọc theo bác sĩ | Hoạt động y như khi tra soát theo hồ sơ |

## Nhóm F — Tốc độ

| Mã | Bước thực hiện | Kết quả mong đợi |
|---|---|---|
| TC-F1 | Lọc 1 bác sĩ, 1 tháng, hồ sơ chưa kết thúc | Có biểu tượng chờ; trả kết quả trong vòng 5 giây |
| TC-F2 | Cuộn danh sách nhiều dòng | Không giật, không tải lại dữ liệu khi cuộn |
| TC-F3 | Đổi loại văn bản ở cột trái liên tục nhiều lần | Màn hình không treo |

## Nhóm G — Ngôn ngữ

| Mã | Bước thực hiện | Kết quả mong đợi |
|---|---|---|
| TC-G1 | Chuyển phần mềm sang tiếng Anh | Toàn bộ nhãn và thông báo trên màn hình hiển thị tiếng Anh, không còn tiếng Việt |
| TC-G2 | Chuyển về tiếng Việt | Hiển thị đúng tiếng Việt có dấu |

---

# PHẦN 9. ĐIỂM CẦN KHÁCH HÀNG CHỐT

| # | Nội dung cần chốt | Nếu chưa chốt thì ảnh hưởng gì |
|---|---|---|
| 1 | **Danh sách biểu mẫu được hỗ trợ tạo văn bản** — hiện chỉ Phiếu chỉ định và Phiếu kết quả; các loại khác báo *chưa được hỗ trợ*. Xem mục 9.2 | Cần chốt để biết còn phải bổ sung biểu mẫu nào |
| 2 | Giới hạn khoảng thời gian tra soát tối đa — đề xuất 31 ngày | Ảnh hưởng tốc độ và tải hệ thống |
| 3 | Bác sĩ A có được xem y lệnh của bác sĩ B không? Nếu có thì kiểm soát bằng quyền nào? | Ảnh hưởng bảo mật và phân quyền |
| 4 | Xác nhận: phần mềm đang báo *Hoàn thành* khi mới có 1 văn bản ký xong là **sai**, cần sửa theo QT-10 | Đây là thay đổi hành vi đang chạy, cần khách hàng đồng ý |
| 5 | **Cách xử lý biểu mẫu đã cấu hình nhưng phần mềm chưa dựng được — xem mục 9.2** | Đang là lỗi im lặng, người dùng không biết vì sao không có gì xảy ra |

## 9.2 Vướng mắc tạo văn bản — ghi nhận 11/08/2026

Khi thử trên dữ liệu thật xuất hiện **hai hiện tượng khác nhau**, gốc là một:

| Hiện tượng | Khi nào | Nguyên nhân |
|---|---|---|
| Báo *"Biểu mẫu này chưa được hỗ trợ tạo văn bản từ màn tra soát"* | Bấm **[Tạo]** ở loại văn bản **không phải** Phiếu chỉ định / Phiếu kết quả | Đúng như thiết kế: 7 loại còn lại chưa nối phần in |
| **Chọn biểu mẫu xong màn hình đứng im**, không thông báo gì | Loại văn bản **đúng**, nhưng biểu mẫu được gắn nằm ngoài tập mẫu phần mềm dựng được | **Lỗi**: chưa kiểm tra trước, nên rơi vào nhánh "không làm gì" của phần in |

**Vì sao xảy ra**: màn **Biểu in** cho phép gắn *bất kỳ* biểu mẫu nào vào một loại văn bản.
Nhưng phần in chỉ biết cách lấy dữ liệu cho **một số biểu mẫu nhất định** — 31 mẫu cho nhóm Phiếu chỉ định,
12 mẫu cho nhóm Đơn thuốc. Gắn mẫu ngoài danh sách thì bấm vào không có phản hồi.

**Ca cụ thể đã gặp**: bệnh viện gắn mẫu **`Mps000033`** (Phiếu yêu cầu phẫu thuật / thủ thuật)
cho loại *Phiếu chỉ định*. Mẫu phẫu thuật mà phần mềm dựng được là **`Mps000036`**.

**Ba hướng xử lý, cần chốt một**:

| Hướng | Việc phải làm | Ưu / nhược |
|---|---|---|
| **A. Sửa cấu hình** | Ở màn Biểu in, đổi sang mẫu phần mềm hỗ trợ (`Mps000036`) | Dùng được ngay, không cần lập trình. **Đề xuất** |
| **B. Ẩn mẫu không dùng được** | Danh sách chọn chỉ hiện mẫu tạo được; hết mẫu thì báo rõ | Hết lỗi im lặng, nhưng phải bảo trì danh sách mỗi khi phần in bổ sung mẫu |
| **C. Bổ sung vào phần in** | Thêm `Mps000033` vào thư viện in dùng chung | Đúng gốc nhất, nhưng ảnh hưởng nhiều phân hệ khác, cần bộ phận quản lý thư viện duyệt |

**Đã phân tích, chưa sửa phần mềm** — chờ chốt hướng.

---

# PHẦN 10. LỘ TRÌNH BÀN GIAO

| Đợt | Nội dung bàn giao | Trạng thái |
|---|---|---|
| **Đợt 1** | Cột Thời gian tạo, cột Trạng thái văn bản, sửa cách tính trạng thái theo QT-10, đa ngôn ngữ | **Đã xong** — chạy được ngay, không phụ thuộc máy chủ |
| **Đợt 2** | Bộ lọc theo bác sĩ + thời gian + trạng thái hồ sơ, hai ô lọc nhanh, chia trang | **Đã xong phần phần mềm.** Chờ máy chủ mở API `GetServiceReqForRecordChecking` mới có dữ liệu |
| **Đợt 3** | Nút **[Tạo]** trên từng dòng | **Dùng được với Phiếu chỉ định / Phiếu kết quả**, và chỉ với biểu mẫu phần mềm dựng được. Còn vướng — xem mục 9.2 |

Đợt 1 dùng được ngay. Đợt 2 chờ máy chủ. Đợt 3 chờ chốt hướng ở mục 9.2.
