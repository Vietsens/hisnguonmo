# TreatmentEndTypeExt — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.Library.TreatmentEndTypeExt |
| Loại | Library plugin (thư viện dùng chung — mở form từ chức năng ra viện / kết thúc điều trị) |
| Mục đích | Nhập thông tin các loại kết thúc điều trị mở rộng: nghỉ ốm, nghỉ dưỡng thai, nghỉ việc hưởng BHXH, phẫu thuật/thủ thuật |
| Form chính | `SickLeave/frmSickLeave` (Nghỉ ốm / Nghỉ dưỡng thai / Nghỉ việc hưởng BHXH), `Surgery/frmSurgery`, `MaternityLeave/frmMaternityLeave` |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Màn hình Nghỉ việc hưởng BHXH (`frmSickLeave`)
Form dùng chung cho 3 loại (`FormEnum.TYPE`): `NGHI_OM`, `NGHI_DUONG_THAI`, `NGHI_VIEC_HUONG_BHXH`. `InitUIByFormType()` bật/tắt các nhóm control theo loại; tiêu đề form lấy theo `HIS_TREATMENT_END_TYPE_EXT`.

Luồng lưu (`btnSave_Click`):
1. `ValidationControlAge()` — kiểm tra người thân/quan hệ khi bệnh nhân < 7 tuổi.
2. `dxValidationProvider1.Validate()` — kiểm tra nhóm trường bắt buộc (Nơi làm việc → Mã BHXH → Phương pháp điều trị → Số CCCD → Ngày cấp). Thứ tự focus theo `TabIndex`.
3. `Check()` — số ngày nghỉ > 0, ngày nghỉ từ ≤ đến.
4. Với nghỉ ốm ngoại trú: chặn nếu số ngày > 30 (trừ khi `His.LeaveDay.AllowBhxhLeaveOver30days = 1`) và cảnh báo trùng ngày nghỉ với đợt khám trước.
5. Đóng gói `TreatmentEndTypeExtData` và trả về qua delegate `ReloadDataTreatmentEndTypeExt`.

### Trường Số CCCD/HC + Ngày cấp
- `txtCCCDNumber`: tối đa 12 ký tự; tự đổi MaxLength 9 (hộ chiếu, có ký tự không phải số) / 12 (CCCD, toàn số) trong `txtCCCDNumber_EditValueChanged`. Khi lưu: 9 ký tự → PassportNumber/PassportDate, 12 ký tự → CccdNumber/CccdDate, độ dài khác → cảnh báo.
- `cboDateCCCD`: DateEdit định dạng `dd/MM/yyyy`.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_TREATMENT | Table | Thông tin điều trị (load theo `treatmentId`), nguồn dữ liệu mặc định các trường |
| HIS_TREATMENT_END_TYPE_EXT | Table | Loại kết thúc điều trị mở rộng (đặt tiêu đề form) |
| V_HIS_DOCUMENT_BOOK | View | Sổ cấp giấy nghỉ BHXH (`FOR_SICK_BHXH`) |
| HIS_WORK_PLACE | Table | Nơi làm việc |
| ACS_USER | Table | Người cấp |

## 4. UI Layout — `frmSickLeave`

```
+--------------------------------------------------------------+
| Số chứng từ | Mã | Người cấp | Số ...                         |
| Nghỉ từ | Nghỉ đến | Số ngày nghỉ                             |
| Họ tên mẹ | Họ tên bố | Người thân | Quan hệ                  |
| Nơi làm việc (*) | ...                                         |
| Số CCCD/HC | Ngày cấp        ← PTTK_49141 (bắt buộc khi cấu hình bật) |
| Số thẻ BHYT | Mã BHXH (*)                                     |
| Phương pháp điều trị (*) | Ghi chú khác                       |
|                                    [Đồng ý (Ctrl S)]         |
+--------------------------------------------------------------+
(*) = trường bắt buộc, caption màu Maroon
```

- Số CCCD/HC = `layoutControlItem16` / `txtCCCDNumber`.
- Ngày cấp = `layoutControlItem17` / `cboDateCCCD`.

## 5. Cấu Hình Hệ Thống

| Config key | Kiểu | Mặc định | Ý nghĩa |
|-----------|------|----------|---------|
| `HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.SickLeave.RequireCccdNumber` | int | 0 | 1 = bắt buộc nhập Số CCCD/HC và Ngày cấp khi lưu phiếu nghỉ BHXH; 0 = không bắt buộc |
| `His.LeaveDay.AllowBhxhLeaveOver30days` | string | — | 1 = cho phép số ngày nghỉ ốm ngoại trú > 30 |

Đọc qua `HisConfigs.Get<int>(key)` (toàn viện).

## 6. Dependencies

- `His.Bhyt.InsuranceExpertise` — check thẻ qua cổng BHXH (`btnLaySoTheBHYT`).
- Mở plugin `HIS.Desktop.Plugins.HisWorkPlace` (thêm nơi làm việc), `HIS.Desktop.Plugins.InfantInformation` (thông tin con — nghỉ dưỡng thai).

## 7. Print

Không in trực tiếp trong plugin này (in giấy nghỉ do `HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt`).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 08/07/2026 | phuongnm | PTTK_49141 — Bổ sung cấu hình `SickLeave.RequireCccdNumber`. Khi bật (=1): caption "Số CCCD/HC" và "Ngày cấp" hiển thị màu Maroon và bắt buộc nhập (cảnh báo trên ô, chặn lưu). Mặc định (=0): giữ nguyên hành vi cũ. Chỉ sửa frontend (`frmSickLeave.cs`, `frmSickLeave__Validate.cs`). |

## 9. Test Cases

### Config OFF (RequireCccdNumber = 0 hoặc chưa cấu hình)
- [ ] Mở form → caption "Số CCCD/HC", "Ngày cấp" màu bình thường.
- [ ] Lưu khi 2 trường trống → lưu thành công (không chặn).

### Config ON (RequireCccdNumber = 1)
- [ ] Mở form → caption "Số CCCD/HC", "Ngày cấp" màu đỏ đậm (Maroon).
- [ ] Số CCCD trống → nhấn Lưu → cảnh báo trên ô Số CCCD, chặn lưu.
- [ ] Ngày cấp trống → nhấn Lưu → cảnh báo trên ô Ngày cấp, chặn lưu.
- [ ] Cả hai đã nhập (CCCD hợp lệ 9/12 ký tự) → lưu thành công.
- [ ] Thứ tự focus khi nhiều trường trống: Nơi làm việc → Mã BHXH → Phương pháp điều trị → Số CCCD → Ngày cấp.
