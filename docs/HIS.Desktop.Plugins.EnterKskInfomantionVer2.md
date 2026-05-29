# Sửa Thông Tin Khám Sức Khỏe V2 — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.EnterKskInfomantionVer2 |
| Loại | Form |
| Mục đích | Nhập / cập nhật thông tin khám sức khỏe phiên bản 2 — bao gồm KSK chung, KSK trên 18 tuổi, KSK dưới 8 tuổi, KSK lái xe, KSK định kỳ, KSK nghề nghiệp, KSK khác. Dữ liệu lưu vào HIS_KSK_GENERAL, HIS_KSK_OCCUPATIONAL, HIS_KSK_DRIVER_CAR, HIS_KSK_OVER_EIGHT, HIS_KSK_UNDER_EIGHT, HIS_KSK_OTHER. |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

Form mở từ ServiceReq yêu cầu khám sức khỏe. Gồm nhiều tab tương ứng các loại KSK.
Mỗi tab cho phép nhập: tiền sử bệnh, nghề nghiệp, DHST, khám 11 chuyên khoa + xếp loại, kết quả CLS, kết luận chung.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_KSK_GENERAL | Table | KSK chung |
| HIS_KSK_OCCUPATIONAL | Table | KSK nghề nghiệp |
| HIS_KSK_DRIVER_CAR | Table | KSK lái xe |
| HIS_KSK_OVER_EIGHT | Table | KSK trên 18 tuổi |
| HIS_KSK_UNDER_EIGHT | Table | KSK dưới 8 tuổi |
| HIS_KSK_OTHER | Table | KSK khác |
| HIS_DHST | Table | Dấu hiệu sinh tồn |
| V_HIS_SERVICE_REQ | View | Yêu cầu dịch vụ |

## 4. UI Layout

Form chính `frmEnterKskInfomantionVer2` chứa `xtraTabControl` với nhiều `xtraTabPage`. Mỗi tab bind tới 1 partial class (file `frmEnterKskInfomantionVer2___*.cs`).

Section 10 ("Nghề, công việc trước đây") trong tab General gồm:
- a. Công việc 1 (txtRecentWorkOne) — TextEdit
- a. Thời gian làm việc 1 (spnRecentWordOneYear / spnRecentWorkOneMonth)
- a. Ngày từ — đến 1 (dteRecentWorkOneFrom / dteRecentWorkOneTo)
- b. Công việc 2 (txtRecentWorkTwo) — TextEdit
- b. Thời gian làm việc 2 (spnRecentWorkTwoYear / spnRecentWorkTwoMonth)
- b. Ngày từ — đến 2 (dteRecentWorkTwoFrom / dteRecentWorkTwoTo)

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Get KSK General | api/HisKskGeneral/Get | MosConsumer |
| Get KSK Occupational | api/HisKskOccupational/Get | MosConsumer |
| Get HIS_DHST | api/HisDhst/Get | MosConsumer |
| Save V2 | api/HisServiceReq/KskExecuteV2 | MosConsumer |

## 6. Dependencies

Không có inter-plugin trực tiếp.

## 7. Print

MPS printers (xem `frmEnterKskInfomantionVer2___PrintMPS.cs`).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 28/05/2026 | anhnh2 | Bổ sung 2 ô text "Công việc:" (`txtRecentWorkOne`, `txtRecentWorkTwo`) vào mục 10 "Nghề, công việc trước đây" trong tab General, hiển thị PHÍA TRÊN ô "Thời gian làm việc". Load/save vào cột `RECENT_WORK_ONE` và `RECENT_WORK_TWO` của bảng `HIS_KSK_GENERAL`. Dịch các LayoutControlItem trong `layoutControlGroup4` (Job-2 và Section 12 trở xuống) xuống thêm 48px. Mở rộng `Group4.Size.Height` từ 587 → 635. |
| 29/05/2026 | anhnh2 | Fix bug load tab "KSK dưới 18 tuổi" (`frmEnterKskInfomantionVer2___UnderEight.cs`): (1) 5 combo người khám (Tuần hoàn, Mắt, TMH, RHM, Cận lâm sàng) load sai entity — đọc từ `currentKskGeneral` thay vì `currentKskUnderEight` → save vào `HIS_KSK_UNDER_EIGHTEEN` nhưng load đọc `HIS_KSK_GENERAL` → mất giá trị sau khi mở lại; (2) `cboExamClinicalOtherLoginName3` load sai cột `EXAM_SUBCLINICAL_LOGINNAME` → đúng là `EXAM_CLINICAL_OTHER_LOGINNAME`. Đã sửa 6 dòng trong `FillDataUnderEighteen`. |

## 9. Test Cases

### Mục 10 — Nghề, công việc trước đây
- [ ] Form mở → ô "Công việc 1" và "Công việc 2" hiện ở phía trên ô "Thời gian làm việc" tương ứng
- [ ] Nhập tên công việc 1 + thời gian + ngày từ-đến → Lưu → mở lại → hiển thị đầy đủ
- [ ] Nhập cả công việc 2 → Lưu → mở lại → hiển thị đầy đủ
- [ ] Bỏ trống cả 2 ô công việc → Lưu → load lại → cột RECENT_WORK_ONE/TWO = NULL
- [ ] Layout: section "12. Tiền sử bản thân" và các sections phía sau hiển thị đúng vị trí mới
- [ ] Tab order: focus chạy đúng thứ tự từ tên công việc → thời gian → ngày
