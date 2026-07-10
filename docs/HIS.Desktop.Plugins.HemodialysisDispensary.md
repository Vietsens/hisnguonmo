# Dự trù thuốc chạy thận — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HemodialysisDispensary |
| Loại | UserControl (MODULE_TYPE_ID__UC) |
| Mục đích | Điều dưỡng dự trù thuốc chạy thận hôm trước ngày BN đến: chọn BN theo Phòng+Ngày+Ca → load đơn chạy thận BS (theo bệnh nhân, cross-treatment) → (+) từng thuốc để kê y lệnh nội trú + tạo phiếu xuất đi lĩnh. |
| Vị trí menu | Phòng khám — Kế hoạch tổng hợp (đăng ký ACS_MODULE, xem mục 6) |
| Người tạo | phuongnm |
| Ngày tạo | 01/07/2026 |
| Trạng thái | Hoàn thành (frontend) |

**Phạm vi:** CHỈ frontend. Plugin dùng **100% API/EFMODEL/Filter đã có sẵn** — KHÔNG thêm bảng/view/API backend mới (tuân thủ yêu cầu chỉ làm frontend, không đụng backend của nhóm khác). Xem mục 8 về khác biệt so với thiết kế 4.2.2 gốc.

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. ĐD mở màn hình → mặc định Phòng = phòng đang làm việc, Ngày = hôm nay, Ca = 1.
2. Lọc **Phòng + Ngày + Ca** (+ từ khóa) → lưới trái hiển thị **DS bệnh nhân chạy thận** trong phòng/ngày/ca đó.
3. Chọn 1 bệnh nhân → lưới phải trên **"Y lệnh bác sĩ"** load các đơn chạy thận BS đã kê **theo bệnh nhân (cross-treatment)**, ưu tiên đơn còn số lượng.
4. Chọn 1 y lệnh BS → lưới phải dưới hiển thị **chi tiết thuốc + cột "Còn lại"** (KIDNEY_AMOUNT_LEFT).
5. Bấm **(+)** trên dòng thuốc (chỉ bật khi *thuốc chạy thận* VÀ *Còn lại > 0* — R11) → mở màn **Kê đơn chạy thận** (AssignPrescriptionKidney) đã tự thêm thuốc với SL = SL chi tiết / Số lần chạy (R12, do AssignPrescriptionKidney tự tính theo KIDNEY_TIMES).
6. Lưu trên form kê đơn = tạo y lệnh nội trú + phiếu xuất ngay. Đóng lại → lưới tự nạp lại (trừ lũy kế "Còn lại", cập nhật trạng thái BN).

### Điều kiện nghiệp vụ
- (+) chỉ enable khi: đã chọn BN + đã chọn y lệnh BS + dòng là **thuốc chạy thận (IS_KIDNEY)** + **Còn lại > 0** (R11).
- "Còn lại" lấy từ công thức view sẵn có `V_HIS_SERVICE_REQ_METY.KIDNEY_AMOUNT_LEFT` — hết toa → (+) tự disable (R13).
- Số lượng dự trù mỗi lần = SL chi tiết / Số lần chạy (KIDNEY_TIMES) — do AssignPrescriptionKidney tính (R12).

## 3. EFMODEL Sử Dụng (ĐÃ CÓ SẴN)

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SERVICE_REQ_8 | View | Lưới trái — BN chạy thận theo Phòng+Ngày+Ca (IS_KIDNEY, EXECUTE_ROOM_ID, KIDNEY_SHIFT, MEDICINE_INFO, TDL_PATIENT_*, TDL_TREATMENT_CODE, DESCRIPTION…) |
| V_HIS_SERVICE_REQ_7 | View | Lưới phải trên — đơn chạy thận BS theo bệnh nhân (TDL_PATIENT_ID, KIDNEY_TIMES, INTRUCTION_TIME, REQUEST_LOGINNAME/USERNAME) |
| V_HIS_SERVICE_REQ_METY | View | Lưới phải dưới — chi tiết thuốc + KIDNEY_AMOUNT_LEFT (Còn lại) |
| HIS_SERVICE_REQ | Table | Lấy y lệnh BS gốc (đủ field, đặc biệt KIDNEY_TIMES) khi mở AssignPrescriptionKidney |
| HIS_SERVICE_REQ_METY | Table | Dựng ServiceReqMety truyền sang AssignPrescriptionKidney |
| V_HIS_ROOM | View | Combo Phòng chạy |
| V_HIS_MEDICINE_TYPE | View | Tên/ĐVT/IS_KIDNEY của thuốc |

## 4. UI Layout

```
+-------------------------------------------------------------+---------------------------+
| Phòng chạy: [P197][CK Răng Hàm Mặt v]  Ngày:[__][<][>]      |  Y lệnh bác sĩ            |
|  Ca:[1 v][<][>]   [Từ khóa tìm kiếm]        [Tìm (Ctrl F)]  |  TG y lệnh|Người CĐ|Số lần |
+-------------------------------------------------------------+---------------------------+
| STT|Ngày|Ca|(o)|(doc)|Tên BN|Năm sinh|Giới|Thông tin thuốc  |  [UcPaging]                |
|    |    |  |   |     |      |        |    |Mã BN|Mã ĐT|Ghi chú+---------------------------+
|                                                             |  STT|(+)|Tên|ĐVT|SL|Còn lại|
|  [UcPaging]                                                 |                           |
+-------------------------------------------------------------+---------------------------+
```

- Lưới trái (V_HIS_SERVICE_REQ_8): STT, Ngày, Ca, icon trạng thái, icon "đã dự trù", Tên BN, Năm sinh, Giới tính, Thông tin thuốc, Mã BN, Mã ĐT, Ghi chú.
- Lưới phải trên (V_HIS_SERVICE_REQ_7): Thời gian y lệnh, Người chỉ định, Số lần chạy + UcPaging.
- Lưới phải dưới (V_HIS_SERVICE_REQ_METY → MetyMatyADO): STT, (+), Tên, Đơn vị tính, Số lượng, Còn lại.

### UC sử dụng
| UC | Mục đích |
|----|----------|
| Inventec.UC.Paging | Phân trang lưới trái + lưới y lệnh BS |

## 5. API Endpoints (ĐÃ CÓ SẴN)

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lưới trái | api/HisServiceReq/GetView8 | MosConsumer | HisServiceReqView8Filter (EXECUTE_ROOM_ID, IS_KIDNEY, INTRUCTION_DATE_FROM/TO, KEY_WORD, SERVICE_REQ_TYPE_IDs) + lọc Ca (KIDNEY_SHIFT) phía client |
| Y lệnh BS | api/HisServiceReq/GetView7 | MosConsumer | HisServiceReqView7Filter (TDL_PATIENT_ID, SERVICE_REQ_TYPE_IDs = {DONDT, DONK}) |
| Y lệnh gốc | api/HisServiceReq/Get | MosConsumer | HisServiceReqFilter (ID) |
| Chi tiết thuốc | api/HisServiceReqMety/GetView | MosConsumer | HisServiceReqMetyFilter (SERVICE_REQ_ID) |

## 6. Dependencies

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.AssignPrescriptionKidney | Bấm (+) trên dòng thuốc | `AssignPrescriptionKidneyADO { ServiceReq = HIS_SERVICE_REQ (y lệnh BS gốc), ServiceReqMety = HIS_SERVICE_REQ_METY (dòng chọn), ServiceReqParentId = ID y lệnh chạy thận của BN }` + `Module` (working room). AssignPrescriptionKidney tự tính SL = AMOUNT / KIDNEY_TIMES. |

### Đăng ký menu (ACS_MODULE — DB, do người triển khai chạy script)
| Cột | Giá trị |
|-----|---------|
| MODULE_LINK | HIS.Desktop.Plugins.HemodialysisDispensary |
| MODULE_NAME | Dự trù thuốc chạy thận |
| Nhóm | Phòng khám — Kế hoạch tổng hợp |

## 7. Print

Không có chức năng in trong plugin này (in phiếu lĩnh thực hiện ở màn Tổng hợp phiếu lĩnh — ExpMestAggregate, mục 4.1.7).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 01/07/2026 | phuongnm | Tạo mới plugin frontend 4.2.2 "Dự trù thuốc chạy thận" (kế thừa màn Chạy thận). |

### Khác biệt có chủ đích so với thiết kế 4.2.2 gốc
Thiết kế gốc dựng lưới trái từ **view mới `V_HIS_HEMODIALYSIS_SCHEDULE`** + API `api/HisHemodialysisSchedule` (thuộc plugin 4.2.1 Xếp lịch MỚI). Các đối tượng backend này **CHƯA tồn tại** và thuộc phạm vi backend nhóm khác. Do yêu cầu **chỉ làm frontend, không thêm backend**, lưới trái được dựng từ **`V_HIS_SERVICE_REQ_8` (đã có sẵn)** — chính là cách màn "Chạy thận" đang dùng: lọc BN chạy thận theo `EXECUTE_ROOM_ID + KIDNEY_SHIFT + INTRUCTION_DATE`.

**Nếu sau này backend bổ sung `V_HIS_HEMODIALYSIS_SCHEDULE` + `HisHemodialysisScheduleFilter` + `api/HisHemodialysisSchedule/Get`:** chỉ cần đổi nguồn `LoadPagingPatient` sang view/filter mới và đổi kiểu `currentServiceReq/currentListData` — UI, lưới Y lệnh BS, lưới chi tiết, và luồng (+) giữ nguyên.

## 9. Test Cases

### Lọc + hiển thị
- [ ] Mở màn → mặc định Phòng hiện tại, Ngày hôm nay, Ca 1 → lưới trái hiển thị đúng BN chạy thận.
- [ ] Đổi Ngày/Ca bằng nút ◀▶ → lưới tự nạp lại đúng.
- [ ] Đổi Phòng ở combo → lưới nạp lại theo phòng mới, ô mã phòng cập nhật.
- [ ] Gõ từ khóa + Tìm (Ctrl F) → lọc đúng.

### Chọn BN → y lệnh BS → chi tiết
- [ ] Chọn BN → lưới "Y lệnh bác sĩ" hiển thị đơn chạy thận theo bệnh nhân (cross-treatment).
- [ ] Chọn y lệnh BS → lưới chi tiết hiển thị thuốc + cột "Còn lại".

### Dấu (+)
- [ ] (+) chỉ bật với dòng thuốc chạy thận và Còn lại > 0.
- [ ] Bấm (+) → mở AssignPrescriptionKidney đã thêm sẵn thuốc, SL = SL/Số lần chạy.
- [ ] Lưu bên AssignPrescriptionKidney → đóng → lưới chi tiết + trạng thái BN nạp lại; "Còn lại" trừ đúng.
