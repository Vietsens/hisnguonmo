# Kê Đơn Cận Lâm Sàng (AssignPrescriptionCLS) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.AssignPrescriptionCLS |
| Loại | Form (`frmAssignPrescription` kế thừa `FormBase`) |
| Mục đích | Kê đơn thuốc/vật tư phục vụ chỉ định cận lâm sàng |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ — Chẩn đoán (Việc 2.6)

Chẩn đoán **chính** và **phụ** dùng control tùy chỉnh (không qua UC), nên xử lý 2.6 trực tiếp trong plugin (KHÔNG áp dụng cho YHCT — `IS_TRADITIONAL`):

- **Cảnh báo không khuyến khích bệnh chính** (`IS_NOT_RECOMMEND_MAIN = 1`): chỉ cảnh báo khi user **chọn/sửa** chẩn đoán chính (`ChangecboChanDoanTD`, `LoadIcdCombo`). Hiển thị "Bệnh {0} không khuyến khích dùng làm bệnh chính. Bạn có chắc chắn sử dụng không?". Chọn Không → xóa, chọn lại. Không cảnh báo khi hiển thị dữ liệu đã lưu.
- **Ẩn chẩn đoán nguyên nhân tử vong** (`IS_DEATH_CAUSE_ONLY = 1`): mặc định KHÔNG hiển thị trong danh sách chọn của bệnh chính (`cboIcds`) và bệnh phụ (popup `frmSecondaryIcd`). Giữ `currentIcds` đầy đủ để hiển thị giá trị đã lưu.
- **Không có kiểm tra khi lưu (B)**: plugin này KHÔNG có luồng kết thúc điều trị (đã comment), nên không áp dụng kiểm tra death-cause khi lưu.

## 3. EFMODEL
HIS_ICD (`IS_DEATH_CAUSE_ONLY`, `IS_NOT_RECOMMEND_MAIN`, `IS_TRADITIONAL`), HIS_EXP_MEST, HIS_SERVICE_REQ.

## 4. Files chính (Việc 2.6)
- `AssignPrescription/frmAssignPrescription__InitUC.cs` — nạp `currentIcds`.
- `AssignPrescription/frmAssignPrescription__InitUCIcd.cs` — bind combo bệnh chính (lọc death-cause).
- `AssignPrescription/frmAssignPrescription.cs` — `ChangecboChanDoanTD`/`LoadIcdCombo` (cảnh báo A1), `frmSecondaryIcd` (lọc death-cause A2).
- `Resources/ResourceMessage.cs` + `Message.Lang.vi/en.resx` — message `BenhKhongKhuyenKhichDungLamBenhChinh`.

## 5. Changelog

| Ngày | Người sửa | Mô tả |
|------|-----------|-------|
| 16/06/2026 | huyvu20 | **Việc 2.6**: Ẩn chẩn đoán nguyên nhân tử vong (`IS_DEATH_CAUSE_ONLY`) khỏi danh sách chọn bệnh chính + phụ (giữ giá trị đã lưu, trừ YHCT); cảnh báo `IS_NOT_RECOMMEND_MAIN` khi chọn/sửa bệnh chính; thêm message `BenhKhongKhuyenKhichDungLamBenhChinh` (vi/en). Không có kiểm tra khi lưu (plugin không có kết thúc điều trị). |

## 6. Test Cases
- [ ] Combo bệnh chính/phụ KHÔNG hiển thị ICD nguyên nhân tử vong; hồ sơ đã lưu vẫn hiển thị đúng.
- [ ] Chọn/gõ ICD chính có cờ `IS_NOT_RECOMMEND_MAIN` → cảnh báo; chọn Không → xóa. Mở hồ sơ đã lưu → KHÔNG cảnh báo.
- [ ] YHCT không bị ảnh hưởng.
