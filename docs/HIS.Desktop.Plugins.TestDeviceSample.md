# Thiết Bị Lấy Thông Tin Mẫu Bệnh Phẩm — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TestDeviceSample |
| Loại | Form (`frmTestDeviceSample : FormBase`); lớp processor nằm ở namespace `Inventec.Desktop.Plugins.TestDeviceSample` |
| Mục đích | Danh sách tài khoản đang đăng nhập máy quét trên máy trạm này, theo cổng COM; đăng xuất từng tài khoản để người khác quét thẻ trên cổng đó |
| Người tạo | ninhdd (07/2024); viết lại: dangth (việc 60557) |
| Trạng thái | Đang phát triển (GĐ1 việc 60557) |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở từ menu hoặc nút **DS đăng nhập** của màn Kết nối thiết bị lấy mẫu (`HIS.Desktop.Plugins.TestConnectDeviceSample`, mở không chặn, giữ 1 cửa sổ).
2. Lưới chỉ đọc: STT, Cổng, Tài khoản, Họ tên, Đăng nhập lúc, nút **Đăng xuất** trên từng dòng. Chỉ hiện cổng đã có tài khoản.
3. Màn tự làm mới 3 giây/lần khi đang mở (chỉ vẽ lại khi danh sách đổi); **Tải lại (Ctrl R)** làm mới ngay.
4. Đăng xuất: hỏi xác nhận → bỏ gắn tài khoản khỏi cổng, dòng biến mất, cổng vẫn kết nối (ô của cổng trên màn Kết nối chuyển vàng "Chờ quét thẻ") → ghi nhật ký.

### Điều kiện nghiệp vụ
- Lưới hiển thị bản sao danh sách phiên dùng chung `TestDeviceSampleDataWorker.TestDeviceSamples`. Khi đăng xuất, chỉ gỡ phiên khớp đủ cổng + tài khoản + giờ đăng nhập; phiên đã đổi (cổng vừa đăng nhập lại) thì báo "đã thay đổi" và tải lại.
- Phiên chỉ nằm trong bộ nhớ máy trạm; màn chỉ thấy và đăng xuất được phiên của chính máy đó.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| TestDeviceSampleADO | ADO (HIS.Desktop.LocalStorage.BackendData) | TeminalCode = cổng, Loginname, Username, SessionTimeStart |

## 4. UI Layout

```
+-----------------------------------------------------------------------+
| Tài khoản đang đăng nhập máy quét trên máy này ... (dòng hướng dẫn)   |
| STT | Cổng | Tài khoản | Họ tên | Đăng nhập lúc | [Đăng xuất]          |
|                                                    [Tải lại (Ctrl R)] |
+-----------------------------------------------------------------------+
```

## 5. API Endpoints

| Action | URI | Consumer | Body |
|--------|-----|----------|------|
| Nhật ký đăng xuất | /api/SdaEventLog/Create | SdaConsumer | SdaEventLogSDO |

## 6. Dependencies

| Plugin | Quan hệ |
|--------|---------|
| HIS.Desktop.Plugins.TestConnectDeviceSample | Tạo/gỡ phiên khi quét thẻ, ngắt cổng; mở màn này bằng nút DS đăng nhập |

## 7. Print

Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 24/07/2024 | ninhdd | Bản đầu: lưới phiên terminal, sửa tài khoản trên dòng |
| 21/10/2026 | dangth | **Việc 60557** — Lưới chỉ đọc chỉ các tài khoản đang đăng nhập, cột Họ tên, nút Đăng xuất trên từng dòng (hỏi xác nhận, so khớp cổng + tài khoản + giờ đăng nhập), tự làm mới 3 giây/lần, nhật ký SDA, chuỗi giao diện/thông báo vi + en. Bỏ sửa tài khoản trên dòng |

## 9. Test Cases

- [ ] Mở từ nút DS đăng nhập: không chặn màn Kết nối, bấm lại chỉ kích hoạt cửa sổ cũ
- [ ] Quét thẻ ở màn Kết nối: dòng mới tự hiện trong vài giây
- [ ] Đăng xuất (Có): dòng biến mất, ô cổng chuyển vàng, có nhật ký; (Không): không đổi
- [ ] Cổng vừa đăng nhập lại người khác rồi bấm Đăng xuất dòng cũ: báo đã thay đổi, phiên mới giữ nguyên
