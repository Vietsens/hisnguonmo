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

### Điều kiện enable nút "Xóa dịch vụ giường" (yêu cầu dịch vụ giường, trạng thái CXL)
Enable khi thỏa **1 trong 4** điều kiện sau (kết hợp AND với `SERVICE_REQ_STT_ID == CXL`):
1. Tài khoản đăng nhập là **người tạo** yêu cầu (`CREATOR == loginName`)
2. Tài khoản đăng nhập là **người chỉ định** yêu cầu (`REQUEST_LOGINNAME == loginName`)
3. Tài khoản đăng nhập là **admin**
4. Tài khoản đăng nhập được phân quyền nút **HIS000053** (Xóa y lệnh giường)

Điều kiện disable bổ sung: `IsDisable == true` hoặc buồng giường không nằm trong `ListVBedRoom`.

## 6. Dependencies

### ACS — Phân quyền
| Mã control | Tên hiển thị | Mục đích |
|-----------|-------------|----------|
| HIS000053 | Xóa y lệnh giường | Cho phép xóa y lệnh giường của tài khoản khác (VD: bác sỹ trực xóa y lệnh BS khác chỉ định) |

Plugin reference `ACS.EFMODEL.dll`. Load quyền qua `GlobalVariables.AcsAuthorizeSDO.ControlInRoles` và lưu cờ `hasDeleteBedPermission`.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 16/04/2026 | phuongnm | Thêm checkbox "Cùng khoa" cạnh combobox Buồng bệnh. Khi tick: chỉ hiển thị buồng thuộc cùng khoa đang điều trị. Lưu trạng thái qua Properties.Settings. Thêm Resources đa ngôn ngữ (vi/en). |
| 22/05/2026 | dangth2 | Việc 44693 (Tài liệu 2671): Thêm phân quyền nút HIS000053 — Xóa y lệnh giường. Bổ sung điều kiện enable nút "Xóa dịch vụ giường" trong `FormBedHistory.cs:gridViewBedServiceReq_CustomRowCellEdit`: enable thêm khi tài khoản có quyền HIS000053. Thêm file `ControlCode.cs`, field `hasDeleteBedPermission`, method `LoadDeleteBedPermission()`. Reference `ACS.EFMODEL.dll`. |
