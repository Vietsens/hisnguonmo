# HIS.Desktop.Plugins.BedHistory — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.BedHistory |
| Loại | Form |
| Mục đích | Quản lý lịch sử giường bệnh: xem, thêm, sửa, xóa log giường, quản lý dịch vụ giường, tách ngày, nằm ghép |
| Trạng thái | Hoàn thành / Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Điều dưỡng mở form Lịch sử giường từ chức năng quản lý bệnh nhân
2. Chọn buồng bệnh từ combobox (lọc theo khoa hoặc toàn viện)
3. Xem lịch sử giường, dịch vụ giường, yêu cầu dịch vụ
4. Thêm/sửa/xóa log giường, gán giường mới
5. Tách dịch vụ theo ngày, theo kết quả dự kiến
6. Lưu thay đổi

### Điều kiện nghiệp vụ
- Buồng bệnh mặc định lọc theo khoa đang điều trị
- Checkbox "Cùng khoa" cho phép lọc buồng chỉ thuộc cùng khoa
- Checkbox "Hiển thị lịch sử giường toàn viện" cho phép xem log giường khoa khác
- Checkbox "Hiển thị giường toàn viện" cho phép chọn giường khoa khác

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 16/04/2026 | phuongnm | Thêm checkbox "Cùng khoa" cạnh combobox Buồng bệnh. Khi tick: chỉ hiển thị buồng thuộc cùng khoa đang điều trị. Lưu trạng thái qua Properties.Settings. Thêm Resources đa ngôn ngữ (vi/en). |
