# Chỉ Định Chạy Thận (KidneyShiftSchedule) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.KidneyShiftSchedule |
| Loại | UserControl (UCKidneyShift) |
| Mục đích | Chỉ định chạy thận: xem lịch chạy thận theo tuần, chọn BN (đột xuất toàn viện / theo lịch) và tạo y lệnh chạy thận (Ca, Máy, Gói vật tư, Dịch vụ, ĐTTT). Trước 2891 tên menu là "Xếp lịch chạy thận". |
| Trạng thái | Bảo trì (cập nhật theo việc 2891) |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Vùng trên: lưới lịch chạy thận theo tuần (V_HIS_SERVICE_REQ_9, IS_KIDNEY), điều hướng tuần trước/sau, lọc theo phòng thực hiện/ca/máy/ngày.
2. Vùng giữa trái: lưới BN đang điều trị toàn viện (V_HIS_TREATMENT_4) — chọn BN đột xuất/cấp cứu.
3. Vùng giữa phải (MỚI - 2891): lưới BN trong lịch chạy thận lấy từ Xếp lịch MỚI (V_HIS_HEMODIALYSIS_SCHEDULE) — **phụ thuộc backend, xem Changelog**.
4. Vùng dưới: form nhập Ngày + Ca + Máy + Gói vật tư + Dịch vụ + ĐTTT + Người chỉ định + Ghi chú + nút "Đưa vào lịch" → tạo y lệnh chạy thận.

### Điều kiện nghiệp vụ (sau 2891)
- Người chỉ định luôn = BS trực (tài khoản đăng nhập), khóa không cho sửa (R8).
- Điều dưỡng (HIS_EMPLOYEE.IS_NURSE) đăng nhập → chỉ được xem, không thao tác chỉ định/hủy (R9).
- Máy + Gói vật tư KHÔNG bắt buộc khi tạo y lệnh (R7 — Máy chỉ nhập ở Xử lý PTTT).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SERVICE_REQ_9 | View | Lưới lịch chạy thận theo tuần (IS_KIDNEY) |
| V_HIS_TREATMENT_4 | View | Lưới BN điều trị toàn viện (vùng trái) |
| V_HIS_HEMODIALYSIS_SCHEDULE | View | (2891) BN theo lịch chạy thận (vùng phải) — do backend bổ sung |
| HIS_MACHINE | Table | Combo máy chạy thận |
| HIS_EXP_MEST_TEMPLATE | Table | Combo gói vật tư |
| ACS_USER | Table | Combo người chỉ định |
| HIS_EMPLOYEE | Table | (2891) Xác định chức danh BS/ĐD để phân quyền thao tác |
| HIS_PATIENT_TYPE / V_HIS_PATIENT_TYPE_ALLOW / V_HIS_SERVICE_PATY | Table/View | Lọc ĐTTT hợp lệ theo BN + dịch vụ |

## 4. UI Layout

```
+----------------------------------------------------------------+
| Lịch chạy thận theo tuần (lưới trên) [<tuần] [tuần>] [In]      |
+-------------------------------+--------------------------------+
| BN điều trị toàn viện (trái)  | BN theo lịch (phải - MỚI 2891) |
+-------------------------------+--------------------------------+
| Ngày | Ca | Máy | Gói VT | DV | ĐTTT | Người chỉ định | Ghi chú |
| [Đưa vào lịch]                                                 |
+----------------------------------------------------------------+
```

## 5. API Endpoints

| Action | URI (RequestUriStore) | Consumer |
|--------|------------------------|----------|
| Lấy lịch chạy thận theo tuần | HIS_SERVICE_REQ_GETVIEW_9 | MosConsumer |
| Lấy BN điều trị toàn viện | HIS_TREATMENT_GETVIEW_4 | MosConsumer |
| Tạo y lệnh chạy thận | HIS_SERVICE_REQ__KIDNEYS_CHEDULE | MosConsumer |
| Hủy y lệnh | HIS_SERVICE_REQ_DELETE | MosConsumer |
| Lấy BN theo lịch (2891) | api/HisHemodialysisSchedule/Get (đề xuất) | MosConsumer |

## 6. Dependencies

Inter-plugin: mở từ menu Phòng khám - Kế hoạch tổng hợp. Liên quan plugin mới HemodialysisSchedule (Xếp lịch chạy thận MỚI) cung cấp dữ liệu vùng phải.

## 7. Print

In lịch chạy thận (nút In trên lưới tuần) — theo printTypeCode cấu hình trong plugin.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 04/07/2026 | dangth2 | **Việc 2891 - mục 4.1.3 (Chỉ định chạy thận)**: (R7) Bỏ bắt buộc Máy + Gói vật tư khi tạo y lệnh — gỡ đăng ký required `cboMarchineForAdd`/`cboExpMestTemplateForAdd` trong `ValidateForm`, null-safe `MachineId` trong `ProcessAddClick`. (R8) Người chỉ định luôn = BS trực = tài khoản đăng nhập: `InitComboUser` auto-fill theo login + khóa `cboUser`/`txtLoginName`. (R9) Điều dưỡng đăng nhập (HIS_EMPLOYEE.IS_NURSE) không được thao tác: thêm `ApplyPermissionByEmployeeTitle()` khóa vùng nhập + nút "Đưa vào lịch", chặn `ProcessAddClick` và nút Hủy trên lưới. **Vùng grid "BN theo lịch"** (vùng giữa phải): thêm `gridControlHemoSchedule` trong Designer + partial `UCKidneyShift___HemoSchedule.cs` (`FillDataToGridHemoSchedule`, `RowHemoScheduleClick` pre-fill Ca/Gói/Máy/Ngày theo slot) đọc `V_HIS_HEMODIALYSIS_SCHEDULE` qua `api/HisHemodialysisSchedule/Get`. **PHỤ THUỘC**: cần lib có `V_HIS_HEMODIALYSIS_SCHEDULE` (backend 2891 mục 2.4) — chưa có trong lib checkout hiện tại nên plugin chỉ build được sau khi cập nhật lib; cần rà lại tên field cột + layout trong VS Designer. |

## 9. Test Cases

### R7 - Bỏ bắt buộc Máy + Gói vật tư
- [ ] Không chọn Máy, không chọn Gói vật tư → "Đưa vào lịch" vẫn tạo y lệnh thành công (không báo bắt buộc).
- [ ] Có chọn Máy → MachineId gửi đúng lên backend.

### R8 - Người chỉ định = BS trực
- [ ] Mở form → cboUser/txtLoginName = tài khoản đăng nhập, không sửa được (disabled).

### R9 - Chặn điều dưỡng
- [ ] Đăng nhập tài khoản điều dưỡng (IS_NURSE=1, IS_DOCTOR≠1) → vùng nhập + nút "Đưa vào lịch" bị khóa; nút Hủy trên lưới bị vô hiệu; bấm tạo báo "không có quyền".
- [ ] Đăng nhập bác sĩ (IS_DOCTOR=1) → thao tác bình thường.

### Vùng "BN theo lịch" (2891 - chờ backend)
- [ ] Chọn BN vùng phải → nạp thông tin BN + Ca/Gói theo slot lịch → tạo y lệnh theo lịch.
