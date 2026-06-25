# Chỉ Định Giường (AssignBed) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.AssignBed |
| Loại | UserControl (`frmAssignBed`) |
| Mục đích | Chỉ định giường + dịch vụ giường cho bệnh nhân |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ — Chẩn đoán (Việc 2.6)

Plugin **hỗn hợp**: chẩn đoán Tây y dùng control tùy chỉnh (xử lý 2.6 trực tiếp), chẩn đoán YHCT dùng UC (`icdYhctProcessor`/`subIcdYhctProcessor` — UC tự xử lý, KHÔNG đụng).

Tây y (KHÔNG áp dụng YHCT — `IS_TRADITIONAL`):
- **Ẩn chẩn đoán nguyên nhân tử vong** (`IS_DEATH_CAUSE_ONLY = 1`): lọc tại điểm bind combo bệnh chính (`frmAssignBed_Uc.cs` — `DataToComboChuanDoanTD(cboIcds, ...)`) và grid chẩn đoán phụ Tây y (`frmAssignBed.cs` — 2 nơi build `icdSubcodeAdoChecks`). Giữ `currentIcds` đầy đủ để hiển thị/tra cứu giá trị đã lưu.
- **Cảnh báo không khuyến khích bệnh chính** (`IS_NOT_RECOMMEND_MAIN = 1`): cảnh báo khi user chọn/sửa chẩn đoán chính (`ChangecboChanDoanTD`). Chọn Không → xóa, chọn lại. Không cảnh báo khi hiển thị dữ liệu đã lưu.
- **Không có kiểm tra khi lưu (B)**: plugin không có luồng kết thúc điều trị.
- Combo nguyên nhân `cboIcdsCause` (`IS_CAUSE`) và nhánh YHCT KHÔNG bị ảnh hưởng.

## 3. EFMODEL
HIS_ICD (`IS_DEATH_CAUSE_ONLY`, `IS_NOT_RECOMMEND_MAIN`, `IS_TRADITIONAL`, `IS_CAUSE`), V_HIS_ICD, HIS_BED, HIS_SERVICE_REQ.

## 4. Files chính (Việc 2.6)
- `AssignBed/frmAssignBed_Uc.cs` — nạp `currentIcds`; bind combo bệnh chính (lọc death-cause).
- `AssignBed/frmAssignBed.cs` — `ChangecboChanDoanTD` (cảnh báo A1); 2 nơi build grid chẩn đoán phụ Tây y (lọc death-cause A2).
- `Resources/ResourceMessage.cs` + `Message.Lang.vi.resx` — message `BenhKhongKhuyenKhichDungLamBenhChinh`.

## 5. Changelog

| Ngày | Người sửa | Mô tả |
|------|-----------|-------|
| 16/06/2026 | huyvu20 | **Việc 2.6**: Ẩn chẩn đoán nguyên nhân tử vong (`IS_DEATH_CAUSE_ONLY`) khỏi danh sách chọn bệnh chính + phụ Tây y (giữ giá trị đã lưu, trừ YHCT); cảnh báo `IS_NOT_RECOMMEND_MAIN` khi chọn/sửa bệnh chính; thêm message `BenhKhongKhuyenKhichDungLamBenhChinh` (vi). Không có kiểm tra khi lưu. YHCT (qua UC) không đụng. |
| 24/06/2026 | huannh | **TG chỉ định + Dự trù**: thêm ô **TG chỉ định** (`dteInstructionTime`, sửa trực tiếp) hiển thị theo 3 cấu hình giờ (server-time treatment / `ShowServerTimeByDefault` / giờ máy trạm); thêm ô **Dự trù** (`dteProvision`, mặc định trống) → `AssignServiceSDO.UseTimes` (cột `HIS_SERVICE_REQ.USE_TIME`); **tam giác vàng chặn lưu** khi TG chỉ định là Thứ 7/CN (message `Plugin_AssignBed__CanhBaoXuatToanChiPhiGiuong`); fix giờ server lấy đúng giờ:phút; fix cảnh báo "thiếu viện phí" hiện 2 lần khi mở form (bỏ lời gọi trùng `CheckOverTotalPatientPrice`); `CheckTimeInDepartment` chỉ chặn khi có dữ liệu thời gian vào khoa (`times.Count > 0`), bỏ qua khi BN chưa có dữ liệu để đối chiếu. |

## 6. Test Cases
- [ ] Combo bệnh chính + grid chẩn đoán phụ Tây y KHÔNG hiển thị ICD nguyên nhân tử vong; hồ sơ đã lưu vẫn hiển thị đúng.
- [ ] Chọn/sửa bệnh chính có cờ `IS_NOT_RECOMMEND_MAIN` → cảnh báo; chọn Không → xóa.
- [ ] Chẩn đoán YHCT không bị ảnh hưởng.
