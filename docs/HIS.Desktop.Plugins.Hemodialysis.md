# Chạy thận (Hemodialysis) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.Hemodialysis |
| Loại | UserControl (UCHemodialysis : UserControlBase) |
| Mục đích | Quản lý danh sách y lệnh chạy thận theo phòng thực hiện: xem trạng thái, bắt đầu/kết thúc xử lý, tạo gói vật tư chạy thận, kê/sửa đơn thuốc chạy thận, xem y lệnh bác sĩ và chi tiết thuốc chạy thận của bệnh nhân. |
| Người tạo | Inventec |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Người dùng lọc theo Từ khóa / Từ ngày / Đến ngày / Trạng thái → **Tìm (Ctrl+F)** → nạp danh sách y lệnh chạy thận của phòng hiện tại (`EXECUTE_ROOM_ID`).
2. Đổi trạng thái xử lý ngay trên lưới (Chưa xử lý → Đang xử lý → Kết thúc) qua các API Start/Unstart/Finish/Unfinish.
3. Tạo **Gói vật tư chạy thận** (mở plugin `AssignPrescriptionPK`) khi y lệnh đã kết thúc.
4. **Sửa đơn thuốc chạy thận** (mở plugin `AssignPrescriptionKidney`) khi y lệnh đang chờ xử lý và đã có `EXECUTE_KIDNEY_SERVICE_REQ_ID`.
5. Chọn 1 dòng bệnh nhân → nạp:
   - Lưới **Y lệnh bác sĩ** (phải-trên): các y lệnh chạy thận của **bệnh nhân** (xuyên đợt điều trị — cross-treatment).
   - Lưới **Chi tiết thuốc chạy thận** (dưới-trái): tất cả thuốc chạy thận đã kê/xuất của bệnh nhân (trừ thuốc đã hiển thị ở cột "Thông tin thuốc").
6. Chọn 1 dòng ở lưới Y lệnh bác sĩ → nạp lưới chi tiết mety/thuốc (phải-dưới), cho phép kê đơn thuốc chạy thận theo từng dòng.

### Điều kiện nghiệp vụ
- Chỉ hiển thị y lệnh có `IS_KIDNEY = true`.
- Nút "Gói vật tư chạy thận" chỉ bật khi trạng thái = Kết thúc (`HT`) và y lệnh đã gắn gói vật tư (`EXP_MEST_TEMPLATE_ID`).
- Nút "Sửa đơn thuốc chạy thận" chỉ bật khi trạng thái = Chưa xử lý (`CXL`) và có `EXECUTE_KIDNEY_SERVICE_REQ_ID`.

### Cột "Máy" (R19)
Hiển thị theo ưu tiên:
1. **Máy ở Xử lý PTTT** (mức dịch vụ) — `MACHINE_NAMES` (gộp từ HIS_SERE_SERV).
2. **Fallback — Máy ở Chỉ định** (mức y lệnh) — `MACHINE_NAME` (HIS_SERVICE_REQ).

Cột `MACHINE_DISPLAY` là cột unbound, tính trong `gridViewPatient_CustomUnboundColumnData`.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SERVICE_REQ_8 | View | Danh sách y lệnh chạy thận (lưới BN). Có `TDL_PATIENT_ID`, `MACHINE_NAME`, `MACHINE_NAMES`, `MEDICINE_INFO`, `KIDNEY_*` |
| HIS_SERVICE_REQ | Table | Lưới "Y lệnh bác sĩ" (load theo `TDL_PATIENT_ID`) |
| V_HIS_SERVICE_REQ_METY | View | Chi tiết mety yêu cầu (lưới phải-dưới) |
| HIS_EXP_MEST_MEDICINE | Table | Chi tiết mety/thuốc đã xuất (lưới phải-dưới) |
| V_HIS_EXP_MEST_MEDICINE | View | Lưới **Chi tiết thuốc chạy thận** (dưới-trái): mã/tên thuốc, hàm lượng, ĐVT, HSD, số lô, số ĐK |
| V_HIS_MEDICINE_TYPE | View | Danh mục thuốc — lấy `IS_KIDNEY = 1` để lọc thuốc chạy thận |
| HIS_SERVICE_REQ_STT | Table | Danh mục trạng thái y lệnh |

### Quan hệ chính
- V_HIS_SERVICE_REQ_8.TDL_PATIENT_ID → gom mọi y lệnh/thuốc chạy thận của 1 bệnh nhân (cross-treatment).
- V_HIS_EXP_MEST_MEDICINE.TDL_MEDICINE_TYPE_ID → V_HIS_MEDICINE_TYPE (lọc IS_KIDNEY).

## 4. UI Layout

```
+---------------------------------------------------------------+------------------------+
| [Từ khóa] [Từ: ..] [Đến: ..] [Trạng thái] [Tìm (Ctrl F)]      | Y lệnh bác sĩ          |
+---------------------------------------------------------------+ (theo bệnh nhân)       |
| Lưới bệnh nhân (V_HIS_SERVICE_REQ_8)                          | TG y lệnh | Người CĐ  |
|  STT | Trạng thái | Ca | Máy | Tên BN | ... | Thông tin thuốc | Số lần chạy          |
+---------------------------------------------------------------+                        |
| [Phân trang bệnh nhân]                                        +------------------------+
+---------------------------------------------------------------+ Chi tiết mety/thuốc    |
| Chi tiết thuốc chạy thận (V_HIS_EXP_MEST_MEDICINE)            | (V_HIS_SERVICE_REQ_METY|
|  STT | Mã thuốc | Tên thuốc | SL | Hàm lượng | ĐVT | HSD |    |  + HIS_EXP_MEST_MEDICINE)|
|  Số lô | Số đăng ký                                          |                        |
| [Phân trang thuốc chạy thận]                                  |                        |
+---------------------------------------------------------------+------------------------+
```

### UC sử dụng
| UC | Mục đích |
|----|----------|
| Inventec.UC.Paging | Phân trang cho lưới bệnh nhân, lưới Y lệnh bác sĩ, và lưới Chi tiết thuốc chạy thận |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Danh sách y lệnh chạy thận | api/HisServiceReq/GetView8 | MosConsumer | HisServiceReqView8Filter |
| Y lệnh bác sĩ (theo BN) | api/HisServiceReq/Get | MosConsumer | HisServiceReqFilter (TDL_PATIENT_ID) |
| Chi tiết mety yêu cầu | api/HisServiceReqMety/GetView | MosConsumer | HisServiceReqMetyFilter |
| Thuốc đã xuất (chi tiết mety phải) | api/HisExpMestMedicine/Get | MosConsumer | HisExpMestMedicineFilter |
| **Chi tiết thuốc chạy thận (dưới-trái)** | api/HisExpMestMedicine/GetView | MosConsumer | HisExpMestMedicineViewFilter (TDL_PATIENT_ID + TDL_MEDICINE_TYPE_IDs) |
| Start/Unstart/Finish/Unfinish | HisRequestUriStore.HIS_SERVICE_REQ_* | MosConsumer | ID |

## 6. Dependencies

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.AssignPrescriptionPK | Nút "Gói vật tư chạy thận" | AssignPrescriptionADO + Module |
| HIS.Desktop.Plugins.AssignPrescriptionKidney | Kê/Sửa đơn thuốc chạy thận | AssignPrescriptionKidneyADO + Module |

## 7. Print
Không có chức năng in trong plugin này.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 02/07/2026 | huannh | Bổ sung lưới "Chi tiết thuốc chạy thận" (dưới-trái) hiển thị tất cả thuốc chạy thận của BN (cross-treatment, nguồn V_HIS_EXP_MEST_MEDICINE), trừ thuốc đã ở cột "Thông tin thuốc", có phân trang riêng. Cột "Máy" (R19) chuyển sang ưu tiên máy Xử lý PTTT (MACHINE_NAMES) → fallback máy Chỉ định (MACHINE_NAME). Đổi nguồn load lưới "Y lệnh bác sĩ" từ TREATMENT_ID → TDL_PATIENT_ID (cross-treatment). |
| 17/07/2026 | huannh | Fix lỗi đổi trạng thái "Chưa xử lý → Đang xử lý" thất bại: backend `/api/HisServiceReq/Start` (vCong42464) nhận object `{ID, SECRETARY_LOGINNAME, SECRETARY_USERNAME}` thay vì id vô hướng → post bare `row.ID` khiến backend nhận Input=null. Thêm ADO `HisServiceReqStartSDO` và gửi object. (Finish/UnStart/UnFinish giữ nguyên post id vì backend chưa đổi.) |
| 02/07/2026 | huannh | Trừ "thuốc đầu" chính xác theo MEDICINE_TYPE_ID và lọc thuốc chạy thận (IS_KIDNEY) ngay trên server qua `TDL_MEDICINE_TYPE_IDs` (phân trang không lệch số dòng); cache danh mục thuốc chạy thận. Bổ sung đa ngôn ngữ: Resources/Lang(.en).resx + Message.Lang(.en).resx + ResourceLanguageManager/ResourceMessage + SetCaptionByLanguageKey() cho toàn bộ caption/label/thông báo (thay hardcode tiếng Việt). |

## 9. Test Cases

### Lưới Chi tiết thuốc chạy thận (mới)
- [ ] Chọn 1 dòng bệnh nhân → lưới dưới-trái hiển thị thuốc chạy thận của BN qua mọi đợt điều trị.
- [ ] Thuốc đang hiển thị ở cột "Thông tin thuốc" KHÔNG xuất hiện trong lưới.
- [ ] Cột HSD hiển thị đúng định dạng ngày giờ; Số lô, Số đăng ký hiển thị đúng.
- [ ] Phân trang hoạt động (chuyển trang, đổi kích thước trang).
- [ ] Bấm "Tìm" nạp lại danh sách BN → lưới thuốc chạy thận được xóa (chưa chọn BN).

### Cột Máy (R19)
- [ ] Y lệnh có máy PTTT → hiển thị máy PTTT (MACHINE_NAMES).
- [ ] Y lệnh chưa có máy PTTT nhưng có máy chỉ định → hiển thị máy chỉ định (MACHINE_NAME).
- [ ] Cả hai rỗng → cột trống.

### Y lệnh bác sĩ (cross-treatment)
- [ ] Chọn BN có nhiều đợt điều trị → lưới Y lệnh bác sĩ hiển thị y lệnh chạy thận của tất cả đợt.
