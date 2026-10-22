# Thông Tin Lấy Mẫu — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | LIS.Desktop.Plugins.SampleInfo |
| Loại | Form popup (`frmSampleInfo : FormBase`) |
| Mục đích | Ghi nhận lấy mẫu cho 1 mẫu (hoặc danh sách mẫu): người lấy mẫu, thời gian lấy mẫu, loại mẫu, barcode, tình trạng, giờ hẹn trả; Lưu / Lưu in barcode |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Tham số mở (SampleInfoFactory phân loại theo kiểu)
| Kiểu | Ý nghĩa |
|------|---------|
| `V_LIS_SAMPLE` | Mở cho 1 mẫu (người lấy mẫu chọn sẵn theo `SAMPLE_LOGINNAME` nếu có, không thì tài khoản đăng nhập) |
| `List<V_LIS_SAMPLE>` | Chế độ nhiều mẫu |
| `string` | Mã y lệnh (SERVICE_REQ_CODE) |
| `DateTime` | **Việc 60557** — giờ quét barcode, chỉ màn Kết nối thiết bị lấy mẫu truyền |
| `Module` | Phòng đang làm việc |

### Khi có giờ quét (mở từ máy quét, việc 60557)
- TG lấy mẫu mặc định = giờ quét (giờ hẹn trả tính theo giờ này).
- Ô Barcode khóa nếu mẫu đã có barcode.
- Lưu hoặc Lưu in thành công → form đóng với `DialogResult.OK` (màn Kết nối dựa vào đó để làm mới màn Lấy mẫu bệnh phẩm và ghi nhật ký).

### Mẫu đã lấy (Đã lấy mẫu) mở không có giờ quét
- TG lấy mẫu mặc định giữ giờ đã ghi (`SAMPLE_TIME`) thay vì giờ mở popup, để sửa người lấy mẫu không làm đổi giờ lấy mẫu.

### Các nơi gọi khác
Không đổi: không truyền giờ quét thì TG lấy mẫu = giờ mở popup như cũ, Lưu không tự đóng (trừ ô "Đóng sau khi in" của Lưu in).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_LIS_SAMPLE | View (LIS) | Mẫu đang xử lý |
| LIS_SAMPLE_TYPE, LIS_PATIENT_CONDITION | Table (LIS) | Loại mẫu, tình trạng |
| ACS_USER | Cache | Danh sách người lấy mẫu |
| HIS_SERVICE_REQ, HIS_SERVICE | Table/Cache | Y lệnh, giờ hẹn trả dự kiến |

## 4. UI Layout

Giữ nguyên. Khi mở từ máy quét, tiêu đề form do màn Kết nối đặt: "Thông tin lấy mẫu – COMx – Người lấy mẫu: …".

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lưu 1 mẫu | api/LisSample/Sample | LisConsumer |
| Lưu nhiều mẫu | api/LisSample/SampleList | LisConsumer |

## 6. Dependencies

| Plugin gọi | Ghi chú |
|------------|---------|
| HIS.Desktop.Plugins.SampleCollectionRoom | Nút lấy mẫu (không truyền giờ quét) |
| HIS.Desktop.Plugins.TestConnectDeviceSample | Việc 60557: truyền bản sao mẫu (SAMPLE_LOGINNAME = tài khoản của cổng) + giờ quét |

## 7. Print

Lưu in: in barcode ống mẫu (Mps000077 hoặc BarTender theo cấu hình).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 21/10/2026 | dangth | **Việc 60557** — Tham số tùy chọn `DateTime` giờ quét (Factory/Behavior/frmSampleInfo): TG lấy mẫu = giờ quét, khóa ô Barcode khi mẫu đã có barcode, Lưu/Lưu in thành công đóng form với OK. Mẫu đã lấy mở lại giữ TG lấy mẫu đã ghi (`GetDefaultSampleTime`). "Lưu in" lưu thất bại (lỗi API hoặc kiểm giờ hẹn trả) thì bấm lại được (trước đây cờ `isSaving` kẹt); lưu thành công vẫn chặn lưu lần 2 |

## 9. Test Cases

- [ ] Mở từ máy quét: người lấy mẫu = tài khoản của cổng, TG = giờ quét, barcode khóa; Lưu → đóng OK, body gửi đúng người và giờ
- [ ] Lưu lỗi: form không đóng
- [ ] Mở từ màn Lấy mẫu bệnh phẩm (không giờ quét): như cũ (TG = giờ mở, không tự đóng sau Lưu)
- [ ] Mẫu đã lấy mở lại qua Danh sách y lệnh: TG lấy mẫu giữ giờ cũ, giờ hẹn trả tính theo giờ đó
