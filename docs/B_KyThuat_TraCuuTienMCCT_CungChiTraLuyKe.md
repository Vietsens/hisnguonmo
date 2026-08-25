# TÀI LIỆU KỸ THUẬT

## Cập nhật tiền cùng chi trả / miễn cùng chi trả qua cổng BHYT

| Thông tin | Nội dung |
|---|---|
| Plugin chính | `HIS.Desktop.Plugins.CallPatientTypeAlter` |
| UC chứa giao diện | `His.UC.UCHein` — `Design/TemplateHeinBHYT1` |
| Library cổng BHXH | `HIS.Desktop.Plugins.Library.CheckHeinGOV` |
| Thư viện gọi API | `His.Bhyt.InsuranceExpertise` (repo `common`) |
| Cấu hình kết nối | `HIS.Desktop.Plugins.Library.RegisterConfig` — `BHXHLoginCFG` |
| Căn cứ pháp lý | Điểm b khoản 2 Điều 18 Nghị định 188/2025/NĐ-CP (01/7/2025) |
| Tài liệu cổng | `Phuluc_MoTaAPI_TraCuuTienMCCT.pdf` — Công văn /CNTT-PM của Trung tâm CNTT BHXH |

Ba trường được cập nhật tự động từ cổng BHYT:

| Trường EFMODEL | Control trên form | Ý nghĩa |
|---|---|---|
| `CO_PAID_ACCUMULATE_AMOUNT` | `txtCoPaidAccumulate` | Cùng chi trả lũy kế trong năm |
| `PAID_6_MONTH` | `chkPaid6Month` | Đã vượt ngưỡng 06 tháng lương cơ sở |
| `FREE_CO_PAID_TIME` | `txtFreeCoPainTime` | TDMC CT — miễn cùng chi trả kể từ ngày nào |

Quy tắc nghiệp vụ tham chiếu bằng mã `QT-xx` — định nghĩa tại Phần 2.

---

# PHẦN 1. DỊCH VỤ CỔNG BHYT

## 1.1 Đặc tả

| Mục | Giá trị |
|---|---|
| URL | `POST {ADDRESS}/api/TraCuuCCT/TraCuuTienMCCT` |
| Môi trường thử | `daotaoegw.baohiemxahoi.gov.vn` |
| Môi trường thật | `egw.baohiemxahoi.gov.vn` |
| Content-Type | `application/json`, charset `utf-8` |
| Xác thực | HTTP Header: `accessToken`, `tokenId`, `passwordHash` |

**Ràng buộc xác thực** — cổng đọc token trong Redis theo khoá `TOKEN-{tokenId}`, tính lại chữ ký HMACSHA256 từ *tài khoản – địa chỉ IP bên gọi – tokenId – dấu thời gian* rồi đối chiếu với `accessToken`:

- Token có hạn — cấu hình `SessionTimeLimit`, mặc định **10 phút**
- **Địa chỉ IP gọi dịch vụ phải trùng IP đã dùng khi lấy token**

## 1.2 Tham số đầu vào

| Vị trí | Trường | Bắt buộc | Mô tả |
|---|---|---|---|
| Header | `accessToken` | x | Token phiên làm việc |
| Header | `tokenId` | x | Mã định danh phiên (`id_token`) |
| Header | `passwordHash` | x | Chuỗi băm mật khẩu |
| Body | `username` | x | Tài khoản thực hiện tra cứu |
| Body | `maThe` | x | Mã thẻ BHYT / mã số BHXH. Sau khi bỏ khoảng trắng phải dài **10, 12 hoặc 15** ký tự |
| Body | `hoTen` | x | Họ tên chủ thẻ. Cổng tự bỏ khoảng trắng thừa trước khi đối chiếu |
| Body | `ngaySinh` | x | Nhận **3 định dạng**: `dd/MM/yyyy`, `MM/yyyy`, hoặc `yyyy` |

## 1.3 Dữ liệu trả về

```json
{
  "MaKetQua": "200",
  "GhiChu": "Nguồn DL lấy từ các CSKCB đề nghị thanh toán KCB BHYT trên HTTTGĐ BHYT tính đến: 05/08/2026 17:30",
  "DataCCT": [
    {
      "Id": 123456,
      "maThe": "DN4010100000001",
      "maCskcb": "01001",
      "ngayVao": "02/04/2026",
      "ngayRa": "05/04/2026",
      "maDoiTuongKCB": "DN",
      "tBNCCTMCCT": 250000,
      "tBNCCTLuyKe": 1250000
    }
  ],
  "ThongTinSoThe": {
    "hoTen": "Nguyễn Văn A",
    "ngaySinh": "01/01/1990",
    "ngayKetThuc": "31/12/2026",
    "maBhxh": "0100000001"
  }
}
```

**Mảng `DataCCT`** — sắp xếp **giảm dần theo ngày ra viện**. Các trường ngày đã định dạng sẵn `dd/MM/yyyy`, ứng dụng **không cần chuyển đổi**; không có giá trị thì trả **chuỗi rỗng**.

| Trường | Kiểu | Ý nghĩa |
|---|---|---|
| `ngayVao` / `ngayRa` | Chuỗi | Ngày vào / ra viện của đợt KCB |
| `maCskcb` | Chuỗi | Cơ sở KCB nơi phát sinh đợt khám |
| **`tBNCCTLuyKe`** | Số thực | **Tiền người bệnh cùng chi trả lũy kế**, dùng xét ngưỡng 06 tháng lương cơ sở |
| **`tBNCCTMCCT`** | Số thực | Tiền cùng chi trả **thuộc diện được miễn** trong đợt KCB đó |
| `duPhong1..5` | Chuỗi | Luôn trả chuỗi rỗng |

> Cổng **không trả về ngày miễn cùng chi trả**. Trường TDMC CT phải suy ra — xem 2.6.

## 1.4 Mã kết quả

**`MaKetQua` là căn cứ duy nhất. Mã HTTP chỉ mang tính tham khảo** — trường hợp không tìm thấy dữ liệu cổng **vẫn trả HTTP 200**.

| MaKetQua | HTTP | Ý nghĩa | `DataCCT` |
|---|---|---|---|
| `200` | 200 | Thành công, có dữ liệu. `GhiChu` ghi mốc thời gian dữ liệu | có |
| `204` | 200 | Không thấy thẻ, hoặc có thẻ nhưng không có chi phí cùng chi trả | `null` |
| `400` | 400 | Thiếu tham số bắt buộc, hoặc `maThe` sai độ dài | `null` |
| `500` | 401 | Token sai / hết hạn / gọi từ IP khác. Phản hồi **không kèm nội dung** | `null` |
| `500` | 500 | Tài khoản thuộc danh sách hạn chế tra cứu, hoặc lỗi hệ thống | `null` |

---

# PHẦN 2. CÔNG THỨC TÍNH

Mỗi công thức trình bày theo 4 lớp: **nghiệp vụ** → **công thức** → **ví dụ** → **trường hợp biên**. Toàn bộ tính **hoàn toàn từ dữ liệu cổng trả về**, không trộn với số liệu HIS tự tính (lý do tại 2.9).

## 2.1 Ba trường và quan hệ giữa chúng

Nghị định 188/2025/NĐ-CP quy định người bệnh được **miễn cùng chi trả** khi thoả **đồng thời hai điều kiện**:

| Điều kiện | Control | Trường EFMODEL |
|---|---|---|
| (1) Tham gia BHYT **5 năm liên tục** | `chkJoin5Year` | `JOIN_5_YEAR` |
| (2) Lũy kế cùng chi trả trong năm **lớn hơn 06 tháng lương cơ sở** | `chkPaid6Month` | `PAID_6_MONTH` |

Khi đủ cả hai, người bệnh được miễn **kể từ một mốc ngày cụ thể** — đó là TDMC CT.

```
tBNCCTLuyKe từ cổng ──> CO_PAID_ACCUMULATE_AMOUNT  (QT-02)
                              │
                              ├──> so với LIMIT (QT-01) ──> PAID_6_MONTH       (QT-03)
                              │
                              └──> tìm đợt vượt ngưỡng ──> FREE_CO_PAID_TIME   (QT-04)
```

Phạm vi tài liệu chỉ tính điều kiện (2) và mốc ngày. Điều kiện (1) — đủ 5 năm — giữ nguyên luồng hiện có.

## 2.2 Ký hiệu

```
R = Response.DataCCT[]           // mảng bản ghi, đã sort GIẢM DẦN theo ngayRa
R[i].tBNCCTLuyKe                 // số thực — LŨY KẾ tính đến hết đợt KCB thứ i
R[i].tBNCCTMCCT                  // số thực — tiền được miễn phát sinh trong đợt i
R[i].ngayRa                      // chuỗi dd/MM/yyyy — ngày ra viện, CÓ THỂ RỖNG
```

**Điểm quan trọng nhất** — mỗi bản ghi `DataCCT` ứng với **một đợt KCB**, và `tBNCCTLuyKe` của bản ghi đó là **tổng lũy kế tính đến hết đợt đó**, đã bao gồm toàn bộ các đợt trước trong năm. Đây **không phải** số tiền cùng chi trả riêng của đợt đó.

Hệ quả: **tuyệt đối không được cộng tổng các bản ghi** — xem cạm bẫy tại 2.4.

## 2.3 QT-01 — Ngưỡng 06 tháng lương cơ sở (`LIMIT`)

### Nghiệp vụ

"06 tháng lương cơ sở" là ngưỡng luật định. Lương cơ sở do Chính phủ ban hành và **thay đổi theo từng đợt** — vì vậy HIS lưu trong bảng `HIS_BHYT_PARAM` có `FROM_TIME` / `TO_TIME` để giữ lịch sử:

| Giai đoạn | BASE_SALARY | LIMIT = ×6 |
|---|---|---|
| 01/7/2019 – 30/6/2023 | 1.490.000 | 8.940.000 |
| 01/7/2023 – 30/6/2024 | 1.800.000 | 10.800.000 |
| Từ 01/7/2024 | 2.340.000 | 14.040.000 |

### Công thức

```
bhytParam = HIS_BHYT_PARAM
              .Where(IS_ACTIVE = 1 AND TO_TIME IS NULL)      // bản ghi đang hiệu lực
              .OrderByDescending(FROM_TIME)
              .FirstOrDefault()

LIMIT = bhytParam.BASE_SALARY x 6
```

Lấy bản ghi **đang hiệu lực tại thời điểm tra cứu**, không phải bản ghi hiệu lực tại thời điểm người bệnh vượt ngưỡng — giữ nguyên hành vi kiểm tra hiện có để hai chỗ không cho kết quả mâu thuẫn.

### Trường hợp biên

| Tình huống | Xử lý |
|---|---|
| `bhytParam == null` hoặc `BASE_SALARY <= 0` | Không suy được QT-03, QT-04. Chỉ điền lũy kế (QT-02), ghi log cảnh báo |

## 2.4 QT-02 — Cùng chi trả lũy kế (`CO_PAID_ACCUMULATE_AMOUNT`)

### Nghiệp vụ

Người bệnh BHYT hưởng 80% / 95% / 100% tuỳ nhóm đối tượng. Phần chênh còn lại người bệnh tự trả — đó là **cùng chi trả**, cộng dồn từ 01/01 đến 31/12 của năm tài chính.

Chỉ tính phần thuộc phạm vi BHYT chi trả. **Không tính**: chi phí ngoài danh mục BHYT, phần vượt trần thanh toán, dịch vụ theo yêu cầu.

Cơ quan BHXH giữ số liệu này dựa trên hồ sơ các cơ sở KCB gửi lên HTTTGĐ BHYT — đây là **nguồn chuẩn**, cũng là số ghi trên giấy chứng nhận không cùng chi trả.

### Công thức

```
if (R rỗng hoặc null)
    -> KHÔNG cập nhật, giữ nguyên giá trị đang có trên form
       (mã 204 nghĩa là "không có dữ liệu", không phải "bằng 0")

CO_PAID_ACCUMULATE_AMOUNT = làm tròn( Max(R[i].tBNCCTLuyKe) )
```

### Cạm bẫy — KHÔNG dùng tổng

Vì `tBNCCTLuyKe` đã là số cộng dồn (xem 2.2), cộng tổng sẽ đếm trùng nhiều lần:

| Bản ghi | ngayRa | tBNCCTLuyKe |
|---|---|---|
| R[0] | 05/08/2026 | 16.500.000 |
| R[1] | 20/06/2026 | 15.200.000 |
| R[2] | 02/04/2026 | 14.500.000 |
| R[3] | 15/02/2026 | 9.800.000 |

```
SAI:  Tổng = 16.500.000 + 15.200.000 + 14.500.000 + 9.800.000 = 56.000.000
ĐÚNG: Max  = 16.500.000
```

Sai lệch gấp **3,4 lần** — đủ để một người bệnh chưa đủ điều kiện bị đánh dấu là đã miễn cùng chi trả.

### Vì sao dùng `Max` chứ không lấy `R[0]`

Lũy kế là hàm **đơn điệu tăng** theo thời gian trong năm tài chính → `Max` luôn bằng bản ghi mới nhất, đồng thời chống thêm hai rủi ro:

| Rủi ro | `R[0]` | `Max` |
|---|---|---|
| Cổng đổi thứ tự sắp xếp | Sai | Đúng |
| `ngayRa` trả rỗng, không xác định được bản ghi mới nhất | Sai | Đúng |

**Kiểm tra chéo bắt buộc** — nếu `R[0].tBNCCTLuyKe` khác `Max` thì ghi log cảnh báo. Đó là dấu hiệu cổng đã đổi hành vi sắp xếp, cần rà lại QT-04 vốn phụ thuộc `ngayRa`.

### Trường hợp biên

| Tình huống | Xử lý |
|---|---|
| `R` rỗng (mã 204) | Giữ nguyên giá trị trên form. **Không gán 0** |
| Chỉ có 1 bản ghi | `Max` = chính bản ghi đó |
| Mọi `tBNCCTLuyKe` = 0 | Điền 0, `chkPaid6Month` bỏ tick |

Cổng trả **số thực**, cột EFMODEL là số nguyên → phải **làm tròn**, không cắt cụt (cắt cụt làm mất tiền lẻ theo hướng bất lợi cho người bệnh).

## 2.5 QT-03 — Đủ 06 tháng lương cơ sở (`PAID_6_MONTH`)

### Nghiệp vụ

Đây là **kết luận** của điều kiện (2) tại 2.1: lũy kế đã vượt ngưỡng luật định hay chưa.

Trên form, khi hai checkbox "Đủ 5 năm" và "Đủ 6 tháng" **cùng được tick**, hệ thống bôi đỏ nhãn TDMC CT và bắt buộc nhập — đó chính là lúc yêu cầu mốc ngày miễn.

### Công thức

```
IsPaid6Month = ( CO_PAID_ACCUMULATE_AMOUNT > LIMIT )

chkPaid6Month = IsPaid6Month
PAID_6_MONTH  = IsPaid6Month ? 'C' : 'K'
```

### Dùng `>` (lớn hơn), không phải `>=`

- Nghị định 188/2025/NĐ-CP: *"lũy kế **lớn hơn** 06 tháng lương cơ sở"*
- Khớp với hàm kiểm tra hiện có trong UC

Nếu dùng `>=` thì lũy kế bằng đúng `14.040.000` sẽ được đánh dấu đủ điều kiện, trong khi hàm kiểm tra lại không chặn lưu — hai chỗ mâu thuẫn.

### Giá trị lưu

Dùng hằng số có sẵn trong `MOS.LibraryHein.Bhyt`, **không hardcode chuỗi**:

| Kết luận | Hằng số | Giá trị |
|---|---|---|
| Đủ ngưỡng | `HeinPaid6Month.HeinPaid6MonthCode.TRUE` | `C` |
| Chưa đủ | `HeinPaid6Month.HeinPaid6MonthCode.FALSE` | `K` |

### Ví dụ

| Lũy kế | LIMIT | So sánh | `chkPaid6Month` | `PAID_6_MONTH` |
|---|---|---|---|---|
| 16.500.000 | 14.040.000 | lớn hơn | tick | `C` |
| 14.040.000 | 14.040.000 | bằng | bỏ tick | `K` |
| 9.800.000 | 14.040.000 | nhỏ hơn | bỏ tick | `K` |

### Trường hợp biên

| Tình huống | Xử lý |
|---|---|
| `LIMIT` không xác định (QT-01 lỗi) | **Không đụng** checkbox, giữ nguyên trạng thái người dùng đang có |
| Người dùng đã tick tay, cổng cho kết quả ngược lại | Đưa vào hộp thoại xác nhận (4.2), không tự ghi đè |

## 2.6 QT-04 — Thời điểm miễn cùng chi trả (`FREE_CO_PAID_TIME`)

### Nghiệp vụ

TDMC CT là **ngày bắt đầu được hưởng 100%**. Theo Nghị định, người bệnh được miễn cùng chi trả **kể từ thời điểm lũy kế vượt 06 tháng lương cơ sở, đến hết năm dương lịch đó**.

Cơ quan BHXH cấp *"Giấy chứng nhận không cùng chi trả trong năm"* ghi đúng ngày này. Backend MOS dùng ngày này cùng `PAID_6_MONTH` để áp mức hưởng 100% cho các dịch vụ có ngày chỉ định từ mốc đó trở đi.

**Cổng không trả về ngày này** — phải suy ra.

### Nguyên lý suy ra

Đợt KCB nào làm lũy kế vượt ngưỡng **lần đầu tiên** thì thời điểm kết thúc đợt đó chính là thời điểm vượt ngưỡng.

**Vì sao lấy `ngayRa` chứ không phải `ngayVao`** — cùng chi trả của một đợt KCB chỉ được chốt khi kết thúc đợt, lúc đó mới biết tổng chi phí và mới cập nhật vào lũy kế. Lấy `ngayVao` sẽ cho người bệnh hưởng miễn sớm hơn thực tế, dẫn tới sai quyết toán BHYT.

### Công thức — 4 bước

```
Bước 1: lọc các đợt đã vượt ngưỡng, bỏ bản ghi không có ngày ra viện
        candidates = R.Where( tBNCCTLuyKe > LIMIT  AND  ngayRa khác rỗng )

Bước 2: sắp xếp TĂNG dần theo ngày ra viện
        (R vốn sort GIẢM dần, nên đây là duyệt ngược từ cuối mảng lên)
        sorted = candidates.OrderBy( ngayRa )

Bước 3: lấy phần tử đầu tiên = đợt vượt ngưỡng LẦN ĐẦU
        crossing = sorted.First()

Bước 4: chuyển dd/MM/yyyy sang số nguyên yyyyMMdd
        FREE_CO_PAID_TIME = crossing != null ? yyyyMMdd(crossing.ngayRa) : null
```

**Định dạng lưu: `yyyyMMdd` — 8 chữ số**, không phải `yyyyMMddHHmmss` như phần lớn trường thời gian khác trong HIS. Trên giao diện hiển thị `dd/MM/yyyy`.

### Ví dụ chi tiết

Dữ liệu cổng trả về, `LIMIT = 14.040.000`:

| idx | ngayVao | ngayRa | tBNCCTLuyKe | Vượt LIMIT? |
|---|---|---|---|---|
| 0 | 02/08/2026 | 05/08/2026 | 16.500.000 | Có |
| 1 | 18/06/2026 | 20/06/2026 | 15.200.000 | Có |
| 2 | 02/04/2026 | 05/04/2026 | 14.500.000 | **Có — lần đầu** |
| 3 | 13/02/2026 | 15/02/2026 | 9.800.000 | Không |

Chạy công thức:

```
Bước 1  candidates = { idx 0, idx 1, idx 2 }        (idx 3 bị loại: 9,8tr < 14,04tr)
Bước 2  sorted     = [ idx 2 (05/04), idx 1 (20/06), idx 0 (05/08) ]
Bước 3  crossing   = idx 2  ->  ngayRa = 05/04/2026
Bước 4  FREE_CO_PAID_TIME = 20260405
```

Kết quả cuối cùng cho cả ba trường:

```
CO_PAID_ACCUMULATE_AMOUNT = 16.500.000       (QT-02: Max)
PAID_6_MONTH              = C                (QT-03: 16.500.000 > 14.040.000)
FREE_CO_PAID_TIME         = 20260405         (QT-04: ngayRa đợt vượt lần đầu)
```

Ý nghĩa nghiệp vụ: người bệnh được miễn cùng chi trả **từ 05/04/2026 đến 31/12/2026**. Đợt điều trị đang tiếp nhận nếu có ngày chỉ định từ 05/04/2026 trở đi sẽ được hưởng 100%.

### Ví dụ đối chiếu — chưa đủ điều kiện

| idx | ngayRa | tBNCCTLuyKe | Vượt LIMIT? |
|---|---|---|---|
| 0 | 05/08/2026 | 11.200.000 | Không |
| 1 | 15/02/2026 | 6.400.000 | Không |

```
CO_PAID_ACCUMULATE_AMOUNT = 11.200.000
PAID_6_MONTH              = K          (11.200.000 không lớn hơn 14.040.000)
FREE_CO_PAID_TIME         = null       (candidates rỗng -> crossing = null)
```

Không cảnh báo — đây là trường hợp bình thường, người bệnh chưa đủ ngưỡng.

### Trường hợp biên

| # | Tình huống | Xử lý |
|---|---|---|
| 1 | `crossing == null` **và** chưa đủ ngưỡng | Bình thường. Để trống, **không cảnh báo** |
| 2 | `crossing == null` **nhưng** đã đủ ngưỡng | **Mâu thuẫn** — lũy kế vượt nhưng mọi `ngayRa` rỗng. Để trống + **cảnh báo ngay lúc điền**, yêu cầu nhập theo giấy chứng nhận |
| 3 | Nhiều đợt cùng `ngayRa` | Lấy đợt đầu tiên sau khi sắp xếp. Cùng ngày nên kết quả không đổi |
| 4 | Cổng chỉ trả 1 bản ghi tổng, đã vượt ngưỡng | `crossing` = chính bản ghi đó. Ngày có thể sớm hơn thực tế — chấp nhận, vì đây là dữ liệu cổng cung cấp |
| 5 | `ngayRa` sai định dạng | Loại bản ghi khỏi `candidates`, ghi log cảnh báo |

## 2.7 Bảng tổng hợp ba công thức

| Trường | Công thức | Nguồn cổng | Kiểu lưu |
|---|---|---|---|
| `LIMIT` (trung gian) | `BASE_SALARY x 6` | — (từ `HIS_BHYT_PARAM`) | số thực |
| `CO_PAID_ACCUMULATE_AMOUNT` | `làm tròn( Max(tBNCCTLuyKe) )` | `DataCCT[].tBNCCTLuyKe` | số nguyên |
| `PAID_6_MONTH` | `(lũy kế > LIMIT) ? C : K` | suy từ trên | chuỗi 1 ký tự |
| `FREE_CO_PAID_TIME` | `ngayRa` của đợt đầu tiên có `tBNCCTLuyKe > LIMIT`, sắp xếp tăng dần theo ngày | `DataCCT[].ngayRa` | số nguyên `yyyyMMdd` |

## 2.8 QT-05 — `tBNCCTMCCT`: chỉ hiển thị, không lưu

`tBNCCTMCCT` là số tiền cùng chi trả **thuộc diện được miễn** phát sinh trong từng đợt KCB — khác `tBNCCTLuyKe`, đây là số **của riêng đợt đó**, không cộng dồn.

HIS **không có cột** tương ứng. Hiển thị tổng `tBNCCTMCCT` trong hộp thoại xác nhận làm thông tin tham khảo cho người tiếp đón, không ghi cơ sở dữ liệu.

## 2.9 QT-06 — Không trộn số cổng với số HIS tự tính

HIS tự tính phần người bệnh đồng chi trả từ dữ liệu dịch vụ đã thực hiện. Số này **khác** lũy kế cổng:

| | Số HIS tự tính | Số cổng BHXH |
|---|---|---|
| Nguồn | Dịch vụ đã thực hiện tại viện | Hồ sơ **đã đề nghị thanh toán** lên HTTTGĐ |
| Phạm vi | Chỉ viện này | **Mọi cơ sở KCB** trong cả nước |
| Độ trễ | Thời gian thực | Trễ — theo mốc ghi trong `GhiChu` |
| Đợt đang mở | Có tính | **Chưa tính** |

Hai con số phục vụ hai mục đích khác nhau và **không thể cộng gộp**:

- Cộng vào sẽ **đếm trùng** phần đợt điều trị tại viện đã được gửi lên cổng
- Không cộng thì thiếu phần đợt đang mở — nhưng đây là **thiếu đúng**, vì lũy kế theo luật chỉ tính hồ sơ đã quyết toán

**Nguyên tắc** — lấy cổng làm nguồn duy nhất cho cả ba trường. Hiển thị `GhiChu` (ví dụ: *"...tính đến: 05/08/2026 17:30"*) để người dùng biết độ trễ dữ liệu và tự quyết khi có tranh chấp.

---

# PHẦN 3. THIẾT KẾ

## 3.1 Thông tin kết nối — tái sử dụng 100%

**Không tạo mới cấu hình kết nối.** Dùng lại `BHXHLoginCFG` đang phục vụ luồng kiểm tra thẻ BHYT:

| Thành phần | Config key |
|---|---|
| `USERNAME` / `PASSWORD` | `HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS` (tách bằng `:`) |
| `ADDRESS` | `HIS.CHECK_HEIN_CARD.BHXH__ADDRESS` |
| Token | lấy qua `api/token/take` — không cần key mới |

### Ánh xạ token hiện có sang header MCCT

| Header MCCT | Lấy từ |
|---|---|
| `accessToken` | `access_token` của token phiên hiện tại |
| `tokenId` | `id_token` của token phiên hiện tại |
| `passwordHash` | Chuỗi băm mật khẩu — hiện dùng MD5, **cần xác minh với cổng** |
| `body.username` | `BHXHLoginCFG.USERNAME` |

Vì dùng chung token với luồng kiểm tra thẻ nên **cùng IP, cùng phiên** — ràng buộc IP tại 1.1 không phát sinh vấn đề mới.

### Ánh xạ body — lấy từ dữ liệu thẻ đã có sẵn

| Body | Nguồn | Ghi chú |
|---|---|---|
| `maThe` | Số thẻ đã chuẩn hoá | Bỏ `-` `_` khoảng trắng → phải còn 10/12/15 ký tự |
| `hoTen` | Họ tên người bệnh, đã xử lý mã hoá ký tự | Dùng chung hàm với luồng kiểm tra thẻ |
| `ngaySinh` | Ngày sinh người bệnh | Luồng hiện có đã sinh `dd/MM/yyyy` hoặc `yyyy` — **cả hai đều hợp lệ** theo 1.2 |

### Config mới duy nhất

```
HIS.CHECK_HEIN_CARD.BHXH__AUTO_CHECK_MCCT    (0 = tắt tự động, 1 = bật)
```

**Đường dẫn API cố định trong code**, không đưa ra cấu hình:

```
api/TraCuuCCT/TraCuuTienMCCT
```

Đây là hằng số `API_MCCT` trong lớp gọi API. Đường dẫn do cổng BHXH quy định, không thay đổi theo từng bệnh viện, nên không cần khai báo cấu hình — bớt một khoản phải thiết lập khi triển khai và loại luôn khả năng gõ sai.

> **Không nối thêm vào key `HIS.CHECK_HEIN_CARD.BHXH__API`** (đang chứa 4 phần ngăn bằng `|`). Hàm tách chuỗi của `BHXHLoginCFG` có lỗi so sánh biên — thêm phần thứ 5 sẽ ném lỗi và ghi log Error mỗi lần nạp cấu hình.

## 3.2 Phân rã theo lớp

Đồ thị tham chiếu giữa các project: `Plugin → UCHein → CheckHeinGOV → RegisterConfig`, và `CheckHeinGOV → InsuranceExpertise`. Thiết kế bám đúng chiều này, **không tạo tham chiếu vòng**.

| # | Project | Việc |
|---|---|---|
| **1** | `His.Bhyt.InsuranceExpertise` | **Thêm** 4 lớp LDO cho request và response<br>**Sửa** lớp gọi API — bổ sung hàm tra cứu MCCT dùng HTTP header và body JSON, đường dẫn cố định bằng hằng số |
| **2** | `Library.RegisterConfig` | **Sửa** `BHXHLoginCFG` — thêm cờ bật/tắt tra cứu tự động (xem 3.1) |
| **3** | `Library.CheckHeinGOV` | **Thêm** lớp ADO chứa dữ liệu thô từ cổng<br>**Sửa** `HeinGOVManager` — thêm hàm tra cứu MCCT: kiểm tra độ dài mã thẻ, gọi API, ánh xạ `MaKetQua` sang thông báo |
| **4** | `His.UC.UCHein` | **Thêm** nhóm Interface + Factory + Behavior theo pattern có sẵn<br>**Sửa** `MainHisHeinBhyt` — mở hàm public nhận kết quả từ cổng<br>**Sửa** `Template__HeinBHYT1` — tính 3 trường theo Phần 2, điền có chặn side-effect<br>**Sửa** Designer — thêm nút tra cứu vào ô lũy kế<br>**Sửa** Resources — bổ sung ngôn ngữ vi / en / my |
| **5** | `Plugins.CallPatientTypeAlter` | **Sửa** luồng kiểm tra thẻ — gọi tra cứu MCCT sau khi kiểm tra thẻ thành công |

**Vì sao tính 3 trường ở lớp 4 (UC) chứ không ở lớp 3 (Library)** — hàm lấy `HIS_BHYT_PARAM` (nguồn của `LIMIT`) đã có sẵn kèm cache trong UC. Đưa phép tính xuống Library sẽ phải nạp lại tham số BHYT, thêm một lượt gọi API thừa mỗi lần tra cứu. Library chỉ gọi cổng và trả dữ liệu thô.

## 3.3 Xử lý theo `MaKetQua`

| MaKetQua | Hành động giao diện | Log |
|---|---|---|
| `200` | Tính 3 trường theo Phần 2 → hộp thoại xác nhận | Info + `GhiChu` |
| `204` | Không có dữ liệu → **giữ nguyên** 3 control, không xoá | Info |
| `400` | Sai tham số (mã thẻ khác 10/12/15) → thông báo người dùng | Warn |
| `500` | Token/IP sai, tài khoản bị hạn chế, lỗi hệ thống → thông báo | Error |
| khác / rỗng | Không rõ → giữ nguyên, thông báo | Error |

> Phải kiểm tra `MaKetQua` **trước**, không dựa vào mã HTTP — mã `204` vẫn trả HTTP 200 kèm `DataCCT` rỗng.

---

# PHẦN 4. LUỒNG NGHIỆP VỤ

## 4.1 Sơ đồ

```
Người dùng quét QR / nhập số thẻ BHYT
  │
  └─> Kiểm tra thẻ trên cổng BHXH
        │
        └─ thẻ hợp lệ
             │
             └─ nếu AUTO_CHECK_MCCT = 1
                  │
                  └─> Tra cứu tiền MCCT
                        ├─ dùng lại token nếu chưa hết hạn 10 phút
                        ├─ POST api/TraCuuCCT/TraCuuTienMCCT   (header + JSON)
                        └─ trả về DataCCT thô, GhiChu, MaKetQua
                             │
                             └─> Tính 3 trường theo Phần 2
                                  │
                                  └─ có chênh lệch với giá trị trên form?
                                       ├─ Có  → hộp thoại xác nhận
                                       │         ├─ Đồng ý  : điền 3 control
                                       │         └─ Từ chối : giữ nguyên
                                       └─ Không → điền im lặng, chỉ ghi log

Song song: nút tra cứu lại trên ô lũy kế → cùng nhánh tra cứu MCCT
```

## 4.2 Hộp thoại xác nhận

```
Cùng chi trả lũy kế trên cổng BHXH:
   16.500.000   (hiện tại: 12.000.000)
Đã cùng chi trả 6 tháng lương cơ sở: Có
TDMC CT trên cổng: 05/04/2026   (hiện tại: trống)

Nguồn DL lấy từ các CSKCB đề nghị thanh toán KCB BHYT
trên HTTTGĐ BHYT tính đến: 05/08/2026 17:30

Bạn có muốn lấy thông tin từ cổng BHXH?          [Có]  [Không]
```

**Chỉ bung hộp thoại khi có ít nhất một trong ba trường lệch.** Trùng khớp hoàn toàn → điền im lặng, chỉ ghi log.

## 4.3 Thứ tự điền — bắt buộc

```
1. Ô cùng chi trả lũy kế
2. Ô TDMC CT              -> phải TRƯỚC bước 3
3. Checkbox đủ 6 tháng
```

Điền TDMC CT trước để hàm kiểm tra ngưỡng không chặn lưu — hàm này chặn khi lũy kế vượt `LIMIT` mà ô TDMC CT còn trống.

Toàn bộ đoạn điền phải **chặn side-effect**: gán giá trị cho ô TDMC CT bằng chương trình sẽ kích hoạt sự kiện đổi text, làm hệ thống tự tick lại hai checkbox 5 năm / 6 tháng và có thể bung hộp thoại *"Bệnh nhân phải có giấy chứng nhận không cùng chi trả trong năm"*. Dùng cờ chặn có sẵn trong UC, và **reset cờ trong khối `finally`**.

---

# PHỤ LỤC A. QUYẾT ĐỊNH THIẾT KẾ ĐÃ CHỐT

| Vấn đề | Phương án đã chọn | Lý do |
|---|---|---|
| Suy ra TDMC CT | Ngày ra viện của đợt vượt ngưỡng lần đầu | Đúng nghĩa "miễn kể từ thời điểm vượt ngưỡng". Lấy ngày ra viện gần nhất sẽ khiến người bệnh mất quyền miễn trong khoảng giữa |
| Thời điểm gọi API | Tự động sau kiểm tra thẻ, kèm nút tra cứu lại | Người tiếp đón không phải thao tác thêm; vẫn tra lại được khi cổng lỗi. Bệnh viện bật/tắt tự động qua config |
| Xung đột giá trị | Hỏi trước khi ghi đè | Tôn trọng số người dùng đã nhập theo giấy tờ; theo đúng pattern đang dùng cho kiểm tra thẻ |
| Nguồn cấu hình kết nối | Tái sử dụng `BHXHLoginCFG` hiện có | Không phát sinh cấu hình mới cho bệnh viện; dùng chung token nên không vướng ràng buộc IP |
| Nơi đặt phép tính 3 trường | Lớp UC, không phải Library | Hàm lấy tham số BHYT đã có cache sẵn ở UC; đưa xuống Library sẽ thêm một lượt gọi API thừa |

---

# PHỤ LỤC B. CHANGELOG

| Ngày | Nội dung | Người thực hiện |
|---|---|---|
| 24/08/2026 | Tạo tài liệu thiết kế — đặc tả API cổng, công thức tính 3 trường, phân rã 5 lớp | khainq |
