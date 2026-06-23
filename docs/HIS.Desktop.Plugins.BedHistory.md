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

### Cảnh báo ngày chỉ định cuối tuần (grid chỉ định dịch vụ giường — `gridViewBedServiceType`)
- Cột "Thời gian dự trù" (`UseTime`) nằm cạnh cột "Thời gian chỉ định" (`IntructionTime`), cho phép sửa, mặc định để trống.
- Khi bấm **Chỉ định**: kiểm tra `IntructionTime.DayOfWeek` của từng dòng. Nếu là Thứ 7 hoặc Chủ nhật → hiển thị cảnh báo dạng validate (tam giác vàng `ErrorType.Warning`) trên ô "Thời gian chỉ định" của dòng vi phạm đầu tiên và **chặn lưu** (`ValidateInstructionTimeWeekend`).
- Nội dung cảnh báo: message `CanhBaoNgayChiDinhCuoiTuan` — "Hôm nay là {thứ}, để tránh trường hợp bị xuất toán chi phí giường, hãy sửa thời gian chỉ định về Thứ 6 và nhập thời gian vào ô Thời gian dự trù."
- Cảnh báo được xóa khi người dùng sửa lại cột "Thời gian chỉ định" (`gridViewBedServiceType_CellValueChanged` → `ClearColumnErrors`).
- Khi lưu: nếu "Thời gian dự trù" có giá trị → truyền vào `HisBedServiceSDO.UseTime` (dạng long `yyyyMMddHHmmss`) trong `ProcessBedServiceReqSDO`.

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
| 08/06/2026 | phuongnm | Sửa cảnh báo T7/CN: thay cơ chế `SetColumnError` (chỉ hiện 1 dòng focus, mất khi sửa/đổi dòng) bằng **`IDXDataErrorInfo`** trên `HisBedServiceTypeADO` (`GetPropertyError` cho "IntructionTime" → `ErrorType.Warning`). Grid tự hiển thị tam giác vàng trên mọi dòng T7/CN, tự re-check khi sửa ô, không chặn lưu. Gỡ `ValidateInstructionTimeWeekend`/`ShowInstructionTimeWeekendWarning`/`GetVietnameseDayOfWeek` và các lời gọi liên quan. |
| 08/06/2026 | phuongnm | Theo phản hồi tester (3 ý): (1) Hiện lại checkbox "Tách theo 24 giờ (A)" + **nhớ trạng thái** qua `Properties.Settings.MySplitBy24hState` (load ở Form_Load, lưu ở `chkSplitBy24h_CheckedChanged`). (2) Cảnh báo T7/CN: **bỏ chặn cứng khi lưu** (btnAssigns_Click chỉ hiển thị, không return), **hiện cảnh báo ngay khi load danh sách** qua `ShowInstructionTimeWeekendWarning()` gọi sau mỗi lần bind grid (TotalTime + 3 handler checkbox). (3) Grid yêu cầu DV giường (`gridViewBedServiceReq`): thêm cột **"Thời gian thực hiện"** (`USE_TIME_DISPLAY`, unbound) lấy `HIS_SERVICE_REQ.USE_TIME` tra cứu theo `SERVICE_REQ_ID` từ `ListServiceReqForSereServs`, format `TimeNumberToTimeString`. Sửa double-count số ngày: `ExecuteTotalDateTimeBed` tính `ProcessTotalBedDay` theo **từng giường** thay vì tổng nhóm. |
| 08/06/2026 | phuongnm | Thêm checkbox **"Tách theo 24h"** (`chkSplitBy24h` / `LciSplitBy24h`) đặt giữa "Tính theo ngày" và "Tách theo ngày". Loại trừ lẫn nhau với Tính theo ngày / Tách theo ngày / Tách theo KQ dự kiến. Khi check → đi hàm xử lý mới `ProcessSplitBy24Hours` (sort theo START_TIME; gom nhóm BED_SERVICE_TYPE_ID + BED_ID + SHARE_COUNT + PATIENT_TYPE_ID; merge khoảng liền kề/đè nhau qua `MergeContinuousSegments`, giữ khoảng trống; cắt block 24h neo theo start qua `BuildBlockBedServiceAdo`, mỗi block AMOUNT=1, vòng lặp `blockStart < finish` tránh block rỗng cuối; đoạn 0 giờ vẫn 1 block). Khi bỏ check → quay về dữ liệu gốc. `TotalTime` ưu tiên 24h > (tách ngày/KQ) > thường. |
| 08/06/2026 | phuongnm | Tài liệu 2653 (mục 2.1) — điều chỉnh: Đổi tên cột "Thời gian dự trù" → **"Thời gian thực hiện"** (key `GC_USE_TIME`). Khi load grid (lúc tick chọn giường): `UseTime` mặc định = `IntructionTime` (gán tại `ExecuteTotalDateTimeBed`, dòng `bedServiceType.UseTime = bedServiceType.IntructionTime`). Sau khi load, 2 cột "Thời gian chỉ định" và "Thời gian thực hiện" sửa độc lập, không đồng bộ giá trị. Cập nhật nội dung message `CanhBaoNgayChiDinhCuoiTuan` trỏ tới ô "Thời gian thực hiện". |
| 06/06/2026 | phuongnm | Tài liệu 2653 (mục 2.1): Bổ sung cột "Thời gian dự trù" trong grid `gridViewBedServiceType` (cạnh cột "Thời gian chỉ định", cho phép sửa, mặc định để trống). Khi bấm Chỉ định: nếu ngày chỉ định rơi vào Thứ 7/Chủ nhật → cảnh báo dạng validate (tam giác vàng) trên ô và chặn lưu. Khi lưu: nếu "Thời gian dự trù" có giá trị → truyền vào `UseTime` của `HisBedServiceSDO`. Thêm property `UseTime` trong `HisBedServiceTypeADO`, repository `repositoryItemDtUseTime`, method `ValidateInstructionTimeWeekend()` + `GetVietnameseDayOfWeek()`, message `CanhBaoNgayChiDinhCuoiTuan` (vi/en). |
