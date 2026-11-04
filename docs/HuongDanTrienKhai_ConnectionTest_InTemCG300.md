# HƯỚNG DẪN TRIỂN KHAI

## Bản vá In tem barcode qua máy lấy máu CG300 — HIS 2.418.0

| Thông tin | Nội dung |
|---|---|
| Chức năng | Kết nối xét nghiệm (`HIS.Desktop.Plugins.ConnectionTest`) |
| Phiên bản áp dụng | **HIS 2.418.0** (phát hành 10/10/2026). Không cài lên phiên bản khác |
| Người thực hiện | Cán bộ triển khai / IT bệnh viện, phối hợp kỹ sư hãng FOYEN |
| Cập nhật | 02/11/2026 — ô quét nhận mọi loại mã, quét xong tự tìm / tự in (không cần Enter); cách lấy tên chương trình CG300; **mỗi bệnh nhân 1 bản tin** (1 file / bệnh nhân); 04/11/2026 — không nhả ống lặp khi quét tiếp các phiếu của cùng bệnh nhân |
| Thời gian dự kiến | 30–45 phút cho một máy trạm lấy mẫu (chưa tính khai báo danh mục) |
| Ảnh hưởng | Chỉ máy trạm được khai báo dùng CG300 mới thấy thay đổi; các máy khác hoạt động như cũ |

Tài liệu nghiệp vụ đi kèm: `A_NghiepVu_ConnectionTest_InTemCG300.docx`.

---

# 1. NỘI DUNG BẢN VÁ

| File trong bản vá | Chép vào (thư mục cài HIS) |
|---|---|
| `HIS.Desktop.Plugins.ConnectionTest.dll` | `Plugins\Module\` |
| `vi\HIS.Desktop.Plugins.ConnectionTest.resources.dll` | `vi\` (cạnh `HIS.exe`) |
| `en\HIS.Desktop.Plugins.ConnectionTest.resources.dll` | `en\` (cạnh `HIS.exe`) |
| `my\HIS.Desktop.Plugins.ConnectionTest.resources.dll` | `my\` (cạnh `HIS.exe`) |
| `SQL_ConnectionTest_CG300_SdaConfigApp.sql` | Chạy trên cơ sở dữ liệu, schema **SDA** |

Kiểm tra toàn vẹn file DLL chính (SHA256):
`B48FF2A6D0C07FAA7AE82F7F90D0316B0AB4AE600D015837DD24800159A62C8A`

```
PowerShell:  Get-FileHash .\HIS.Desktop.Plugins.ConnectionTest.dll -Algorithm SHA256
```

Bản vá **không** thay đổi máy chủ (backend), không thêm bảng/cột cơ sở dữ liệu; chỉ thêm 2 dòng cấu hình (bước 3).

---

# 2. CHUẨN BỊ TRƯỚC KHI CÀI

## 2.1 Điều kiện

- [ ] Bệnh viện đang chạy **HIS 2.418.0**.
- [ ] Phần mềm điều khiển CG300 đã được kỹ sư hãng cài **trên chính máy trạm lấy mẫu** (cùng máy với HIS) và cấu hình theo **phương thức 2 — đọc file** (Method 2).
- [ ] Máy CG300 đã khai báo bảng **mã ống ↔ màu ống ↔ khay** (ví dụ 001 tím, 004 đỏ, 006 đen…).
- [ ] Có tài khoản quản trị chạy được câu lệnh trên schema SDA.
- [ ] Có tài khoản HIS được quyền vào: *Thiết lập Dịch vụ - Loại mẫu*, danh mục *Loại mẫu bệnh phẩm*, menu **Hệ thống → Cấu hình tài khoản / Cấu hình tài khoản (admin) / Cấu hình tài khoản (code)**.

## 2.2 Thông tin cần lấy tại viện

| Thông tin | Cách lấy | Ví dụ |
|---|---|---|
| Đường dẫn thư mục `ALL` mà phần mềm CG300 đọc | Hỏi kỹ sư hãng, hoặc xem cấu hình phần mềm CG300 | `D:\CG300\ALL` |
| Tên chương trình điều khiển CG300 | Mở phần mềm CG300 → Task Manager → chuột phải dòng chương trình → **Go to details** → cột **Name**, bỏ đuôi `.exe`. **Không** lấy tên ở tab *Processes* (đó là tên hiển thị, VD `BloodSystemV2.0`) | `Intelligent Workstation` |
| Tài khoản HIS của kỹ thuật viên dùng máy lấy mẫu | Danh sách tài khoản phòng lấy mẫu | `ktv_laymau01` |
| Bảng mã ống trên máy CG300 | Kỹ sư hãng cung cấp | 001 Purple, 004 Red, 006 Black… |

---

# 3. CÁC BƯỚC CÀI ĐẶT

## Bước 1 — Sao lưu

1. Yêu cầu người dùng trên máy trạm **đóng HIS** (kiểm tra Task Manager không còn `HIS.exe`).
2. Sao lưu 4 file sẽ bị ghi đè sang một thư mục riêng, ví dụ `D:\SaoLuu\ConnectionTest_<ngày>\`:
   - `Plugins\Module\HIS.Desktop.Plugins.ConnectionTest.dll`
   - `vi\`, `en\`, `my\` `HIS.Desktop.Plugins.ConnectionTest.resources.dll`

## Bước 2 — Chép file bản vá

1. Giải nén `2.418.0.rar`.
2. Chép **đủ 4 file** vào đúng vị trí ở mục 1, chọn ghi đè.
3. **Bắt buộc** chép cả 3 thư mục `vi`, `en`, `my`. Thiếu các file này thì chữ trên ô tích và các thông báo sẽ hiện **trống**.
4. Nếu bệnh viện phân phối bản cập nhật qua thư mục/máy chủ cập nhật tự động, chép vào đó theo đúng cấu trúc thư mục trên để mọi máy trạm nhận cùng một phiên bản.

> Có thể cài cho mọi máy trạm. Máy không khai báo CG300 (bước 5) vẫn hoạt động y như cũ.

## Bước 3 — Khai báo 2 key cấu hình (chạy một lần cho cả viện)

**Cách 1 — chạy SQL:** chạy file `SQL_ConnectionTest_CG300_SdaConfigApp.sql` trên schema **SDA**. Câu lệnh tạo 2 key, giá trị mặc định **để trống**; chạy lại nhiều lần không bị trùng.

**Cách 2 — trên phần mềm:** menu **Hệ thống → Cấu hình tài khoản (code)** → **Thêm (Ctrl N)**, mỗi key một lần:

| Trường | Giá trị |
|---|---|
| KEY | tên key ở bảng dưới |
| Kiểu giá trị | `string` |
| Giá trị mặc định | **để trống** (điền vào đây thì mọi tài khoản đều nhận giá trị đó) |
| Mã ứng dụng | `HIS` (bắt buộc — thiếu thì phần mềm không nạp key) |
| Mô tả | Kết nối xét nghiệm – In tem CG300: thư mục ALL / tên chương trình CG300 |

| Key | Ý nghĩa |
|---|---|
| `CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_FOLDER` | Thư mục `ALL` mà phần mềm CG300 đọc. Trống = không dùng CG300 |
| `CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_PROCESS_NAME` | Tên chương trình điều khiển CG300. Trống = không kiểm tra chương trình |

Kiểm tra sau khi chạy:

```sql
SELECT KEY, DEFAULT_VALUE, APP_CODE FROM SDA_CONFIG_APP
 WHERE KEY LIKE 'CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300%';
```

Kết quả phải có **2 dòng**, `APP_CODE = HIS`.

## Bước 4 — Khai báo danh mục loại ống

### 4.1 Danh mục Loại mẫu bệnh phẩm

Mỗi loại mẫu bệnh phẩm tương ứng **một loại ống** trên máy CG300:

| Trường | Quy tắc | Ví dụ |
|---|---|---|
| Mã | **Trùng đúng mã ống** đã khai trên máy CG300, giữ nguyên số 0 đầu | `001` |
| Tên | Dạng **`Màu - Nguồn bệnh phẩm`** (có khoảng trắng hai bên dấu gạch) | `Purple - EDTA` |

Phần trước dấu ` - ` là màu ống gửi sang máy; phần sau là nguồn bệnh phẩm **in trên tem**.

### 4.2 Thiết lập Dịch vụ - Loại mẫu

Mở chức năng **Thiết lập Dịch vụ - Loại mẫu**, gán loại mẫu (loại ống) cho **từng dịch vụ xét nghiệm** cần lấy bằng máy CG300. Dịch vụ chưa gán sẽ không được gửi sang máy và bị cảnh báo khi in.

## Bước 5 — Khai báo máy trạm/tài khoản dùng CG300

Chọn một trong hai cách:

- **Kỹ thuật viên tự làm:** đăng nhập bằng tài khoản của mình → **Hệ thống → Cấu hình tài khoản** → tìm `CG300` → nhập cột **Giá trị** → **Lưu (Ctrl S)**.
- **Quản trị làm hộ:** **Hệ thống → Cấu hình tài khoản (admin)** → ở ô **"Chọn theo:"** chọn tài khoản kỹ thuật viên → tìm `CG300` → nhập giá trị → **Lưu (Ctrl S)**.

Giá trị cần nhập:
   - `..._CG300_FOLDER` = đường dẫn thư mục `ALL` (mục 2.2)
   - `..._CG300_PROCESS_NAME` = tên file chương trình CG300 ở tab *Details* của Task Manager, **không** có `.exe` (máy FOYEN CG300: `Intelligent Workstation`, giữ dấu cách)
Lặp lại cho từng tài khoản làm việc tại máy lấy mẫu, rồi **đăng xuất và đăng nhập lại HIS** để nạp cấu hình mới.

Muốn tắt CG300 cho một tài khoản: xóa trắng giá trị `..._CG300_FOLDER` (hoặc bấm **Khôi phục giá trị mặc định**).

> Cấu hình lưu **theo tài khoản**, không theo máy. Luôn khai báo **cả tên chương trình**: khi tài khoản đó đăng nhập ở máy khác không có CG300, ô tích sẽ không hiện nhầm.

Quyền thư mục: tài khoản Windows chạy HIS phải có quyền **ghi và xóa** trong thư mục `ALL`.

## Bước 6 — Bật trên màn hình

1. Bật phần mềm điều khiển CG300.
2. Mở chức năng **Kết nối xét nghiệm**. Bên phải ô tìm kiếm xuất hiện ô tích **"In tem CG300"**.
3. Tích vào ô. Trạng thái tích được nhớ cho các lần mở sau trên máy đó. Ô tìm kiếm đổi gợi ý thành **"Quét mã..."**.
4. Khi làm việc, luôn quét vào ô **"Quét mã..."** (không quét vào ô Mã điều trị / Mã y lệnh / Mã bệnh nhân). Ô này nhận **barcode mẫu, mã y lệnh, mã điều trị hoặc mã bệnh nhân**, tự bù số 0 như các ô mã. Quét xong phần mềm **tự tìm ngay** (không cần máy quét gửi phím Enter); quét lại cùng mã thì **tự in**. Gõ tay bằng bàn phím thì bấm Enter.

---

# 4. KIỂM TRA SAU KHI CÀI

| # | Thao tác | Kết quả đúng |
|---|---|---|
| 1 | Mở Kết nối xét nghiệm bằng tài khoản **chưa** khai báo CG300 | Không có ô tích, màn hình như cũ |
| 2 | Tài khoản đã khai báo, phần mềm CG300 đang chạy | Có ô tích "In tem CG300" |
| 3 | Tắt phần mềm CG300 rồi bấm **Tìm (Ctrl F)** | Ô tích ẩn. Bật lại phần mềm, bấm Tìm → ô tích hiện lại |
| 4 | Tích ô, quét barcode mẫu A | Hiện các mẫu của A, con trỏ ở ô tìm kiếm, mã bôi đen |
| 4b | Quét **mã điều trị** (VD `1165256`) vào ô "Quét mã..." | Hiện các mẫu của lần điều trị `000001165256`; 3 ô mã khác để trống. Quét lại → in tem cho tất cả mẫu đang hiển thị |
| 5 | Quét **lại** barcode A | Máy CG300 dán tem, nhả **đúng số ống, đúng màu**; mẫu chuyển **Đã lấy mẫu** |
| 6 | Quét barcode mẫu B ngay sau | Tìm B, **không** in thêm cho A |
| 7 | Quét lại A lần thứ ba | Hỏi xác nhận in lại |
| 8 | Đối chiếu tem in | Đúng barcode, mã BN, họ tên có dấu, giới tính, tuổi, nguồn bệnh phẩm, khoa, thời gian in |
| 9 | In barcode bằng chức năng cũ (máy in thường/Bartender) | Vẫn hoạt động như trước |
| 10 | Máy quét không gửi phím Enter: quét 1 lần, rồi quét lại | Lần 1 tự tìm, lần 2 tự in — không cần bấm gì |
| 11 | Gõ tay mã vào ô "Quét mã..." | Chưa tìm cho tới khi bấm Enter |
| 12 | Bệnh nhân có nhiều phiếu chỉ định (nhiều phòng khám): quét **1 phiếu** | Lưới hiện đủ mẫu của tất cả phiếu cùng lượt điều trị |
| 13 | Quét lại phiếu đó | Thư mục `ALL` có **đúng 1 file** `<mã BN>_<thời gian>.txt`, máy nhả ống của tất cả phiếu |
| 14 | Quét tiếp phiếu thứ 2 của bệnh nhân đó hai lần | Lưới hiện lại các mẫu; **không** nhả thêm ống, hiện hỏi in lại với nút mặc định **Không** |

---

# 5. XỬ LÝ SỰ CỐ

| Hiện tượng | Nguyên nhân thường gặp | Cách xử lý |
|---|---|---|
| Không có ô tích "In tem CG300" | Chưa chạy SQL bước 3, hoặc chưa đăng nhập lại sau khi khai báo | Kiểm tra lại bước 3, bước 5; đăng xuất/đăng nhập lại |
| Không có ô tích dù đã khai báo | Thư mục `ALL` không tồn tại hoặc không ghi được; phần mềm CG300 chưa chạy; tên chương trình khai sai | Log có dòng `CG300 chua ket noi: <lý do>`. Lý do "chưa chạy" mà phần mềm đang mở → tên khai sai: lấy lại ở tab **Details** (mục 2.2), không lấy tên tab Processes |
| Quét 2 lần thì mã bị nối đôi trong ô, không in | Quét nhầm vào ô **Mã điều trị** (ô này không bôi đen sau Enter) | Bấm chuột vào ô **"Quét mã..."** rồi quét lại |
| Ô tích có, chữ hoặc thông báo trống | Thiếu file trong `vi\ en\ my\` | Chép lại đủ 3 thư mục ngôn ngữ (bước 2) |
| Báo "Mất kết nối máy CG300" khi quét | Phần mềm CG300 bị tắt hoặc thư mục mất quyền trong ca | Bật lại phần mềm/sửa quyền, bấm **Tìm** |
| Báo "Dịch vụ chưa được gán loại mẫu bệnh phẩm" | Dịch vụ chưa gán loại ống | Bổ sung tại **Thiết lập Dịch vụ - Loại mẫu** (bước 4.2) |
| Báo "Các mẫu chưa có barcode" | Mẫu chưa được sinh barcode | Sinh barcode theo quy trình LIS hiện hành rồi quét lại |
| Đã quét lại nhưng máy không nhả ống | Phần mềm CG300 không đọc đúng thư mục; mã ống không có trên máy | Xem trong thư mục `ALL` có file `.txt` mới không. **Có file nằm lại** → phần mềm CG300 chưa đọc (sai thư mục/chưa chạy). **Không có file** → xem thông báo trên HIS |
| Nhả sai màu ống | Mã loại mẫu bệnh phẩm không trùng mã ống trên máy | Đối chiếu bảng mã ống với danh mục (bước 4.1) |
| Tem in sai nguồn bệnh phẩm | Tên loại mẫu không theo dạng `Màu - Nguồn` | Sửa tên loại mẫu (bước 4.1) |

Nhật ký lỗi của HIS: thư mục `Logs\LogSystem.txt` cạnh `HIS.exe` — tìm từ khóa `CG300`. Gửi kèm khi báo lỗi cho bộ phận hỗ trợ.

---

# 6. NÂNG CẤP TỪ BẢN VÁ TRƯỚC

Máy trạm đã cài bản vá CG300 trước ngày 02/11/2026:

1. Đóng HIS, sao lưu 4 file như bước 1.
2. Chép đè DLL chính và 3 thư mục `vi`, `en`, `my`.
3. **Không** cần chạy lại SQL; cấu hình theo tài khoản giữ nguyên.
4. Kiểm tra giá trị `..._CG300_PROCESS_NAME`: nếu đang là `BloodSystemV2.0` (tên hiển thị) thì sửa thành `Intelligent Workstation` (mục 2.2).
5. Đăng nhập lại, chạy kiểm tra số 4b, 10, 11, 12, 13, 14 ở mục 4.

---

# 7. GỠ BẢN VÁ

1. Đóng HIS trên máy trạm.
2. Chép lại 4 file đã sao lưu ở bước 1 vào đúng vị trí cũ.
3. (Tùy chọn) Để trống giá trị 2 key cấu hình của các tài khoản ở bước 5. Không cần xóa key trong cơ sở dữ liệu — bản cũ không đọc 2 key này.

---

# 8. BẢNG KIỂM TRIỂN KHAI

- [ ] Xác nhận phiên bản HIS 2.418.0
- [ ] Đã sao lưu 4 file
- [ ] Đã chép DLL chính + 3 thư mục `vi en my`, kiểm tra SHA256
- [ ] Đã chạy SQL trên schema SDA, kiểm tra có 2 key
- [ ] Mã loại mẫu bệnh phẩm trùng mã ống trên máy; tên theo dạng `Màu - Nguồn`
- [ ] Đã gán loại mẫu cho các dịch vụ xét nghiệm
- [ ] Đã khai báo thư mục + tên chương trình cho từng tài khoản máy lấy mẫu
- [ ] Tài khoản Windows có quyền ghi/xóa thư mục `ALL`
- [ ] Đăng nhập lại, thấy ô tích "In tem CG300"
- [ ] Chạy đủ các bước kiểm tra ở mục 4
