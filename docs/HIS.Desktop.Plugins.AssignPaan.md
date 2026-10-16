# Chỉ định giải phẫu bệnh (AssignPaan) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.AssignPaan |
| Loại | Form (`frmAssignPaan` kế thừa FormBase) |
| Mục đích | Chỉ định dịch vụ giải phẫu bệnh (loại GPB, vị trí, dung dịch, phòng thực hiện, đối tượng) cho hồ sơ điều trị, gắn tờ điều trị và in phiếu yêu cầu |
| Người tạo | (không rõ — tài liệu lập 09/10/2026 khi sửa việc 59656) |
| Ngày tạo | 09/10/2026 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở từ Buồng bệnh, màn xử lý dịch vụ, màn Tạo tờ điều trị (truyền tờ đang mở) với hồ sơ điều trị hoặc dịch vụ cha.
2. Nạp danh sách tờ điều trị của hồ sơ trong khoa đang làm việc (`LoadDataToTrackingCombo`, `api/HisTracking/GetView`). Bật key `HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption` = 1: combo chỉ hiện tờ do người chỉ định tạo (`GetTrackingDataSourceByOwner`), danh sách đầy đủ vẫn giữ ở `trackingAdos`.
3. Chọn tờ mặc định (`SetDefautTrackingCombo`): key `HIS.Desktop.Plugins.AssignPrescription.IsDefaultTracking` = 0 dùng tờ truyền vào; = 1 tờ mới nhất cùng ngày chỉ định trong khoa. Bật key `AssignToOwnTrackingOption` = 1: `SetDefaultOwnTrackingCombo` (tờ truyền vào nếu của người chỉ định, nếu không tờ mới nhất của người chỉ định cùng ngày chỉ định trong khoa, không có thì để trống).
4. Đổi Người chỉ định (`cboUsername_EditValueChanged`) khi key = 1: `ReloadTrackingComboByOwner` lọc lại `trackingAdos` theo người chỉ định mới (không gọi lại API) và chọn lại tờ mới nhất của người đó trong ngày chỉ định (không có thì để trống); người chỉ định không đổi thì giữ nguyên lựa chọn.
5. Lưu / Lưu in: `api/HisServiceReq/PaanCreate` (`ProcessSave`), gửi `TrackingId` của combo Tờ điều trị.

### Điều kiện nghiệp vụ
- `MOS.HIS_SERVICE_REQ.ASSIGN_SERVICES.IS_TRACKING_REQUIRED` = 1: bắt buộc chọn tờ điều trị với diện nội trú/ngoại trú điều trị.
- Việc 59656 (sửa 14/10/2026) — key `HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption` = 1: combo Tờ điều trị chỉ hiện tờ do chính người chỉ định tạo (`HIS_TRACKING.CREATOR` = người chỉ định) và chọn sẵn tờ mới nhất của người đó trong ngày chỉ định (tờ truyền vào của người khác không hiện, không chọn); đổi người chỉ định thì lọc/chọn lại. Key trống/giá trị khác: như trước việc 59656 (hiện mọi tờ). Không còn cảnh báo/chặn khi lưu.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TRACKING | View | Danh sách tờ điều trị (combo) |
| V_HIS_TREATMENT, V_HIS_PATIENT_TYPE_ALTER | View | Hồ sơ, diện điều trị |
| ACS_USER | Table | Người chỉ định (`cboUsername`) |

## 4. UI Layout

Combo Tờ điều trị (`cboTracking`, cột thời gian), Thời gian chỉ định (`dtInstructionTime`), Người chỉ định (`cboUsername` hiện khi `HIS.Desktop.Plugins.AssignConfig.ShowRequestUser` = 1), các combo dịch vụ GPB; nút Lưu, Lưu in, In.

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Tờ điều trị | api/HisTracking/GetView | MosConsumer |
| Tạo chỉ định | api/HisServiceReq/PaanCreate | MosConsumer |

## 6. Dependencies

| Plugin gọi | Args truyền |
|------------|-------------|
| HIS.Desktop.Plugins.TrackingCreate | treatmentId, `HIS_TRACKING` đang mở, `RefeshReference` |

## 7. Print

Phiếu yêu cầu xét nghiệm giải phẫu bệnh phẩm (`InPhieuYeuCauXetNghiemGiaiPhauBenhPham`).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 09/10/2026 | dangth2 | Việc 59656: y lệnh GPB chỉ vào tờ điều trị do chính người chỉ định tạo. Bật key `HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption`: `SetDefautTrackingCombo` → `SetDefaultOwnTrackingCombo` (tờ truyền vào nếu của người chỉ định, nếu không tờ mới nhất của người chỉ định cùng ngày trong khoa, không có thì để trống); Lưu / Lưu in gọi `CheckTrackingOwnerBeforeSave` (partial `frmAssignPaan__Plus__TrackingOwner.cs`). Lớp logic `Base/TrackingOwnerChecker.cs` + `EnumTrackingOwnerOption.cs` (bản sao giống hệt ở 7 plugin), 4 message `TrackingOwner__*` (vi/en). Key trống: không đổi hành vi. |
| 14/10/2026 | dangth | Việc 59656 (sửa theo test chị Hân 14/10): key `AssignToOwnTrackingOption` = 1 chỉ hiện và chọn sẵn tờ điều trị do người chỉ định tạo (`LoadDataToTrackingCombo` nạp combo qua `GetTrackingDataSourceByOwner`), đổi người chỉ định thì lọc/chọn lại (`cboUsername_EditValueChanged` → `ReloadTrackingComboByOwner`, lọc lại danh sách đã nạp `trackingAdos`, không gọi lại API); bỏ cảnh báo/chặn khi lưu (xóa `CheckTrackingOwnerBeforeSave` và các hàm phụ, 4 message `TrackingOwner__*` vi/en). Enum chỉ còn `None` = 0, `OwnOnly` = 1. |

## 9. Test Cases

### Việc 59656 - Combo tờ điều trị chỉ hiện tờ của chính người chỉ định (sửa 14/10/2026)
- [ ] Key trống/giá trị khác: combo hiện mọi tờ của khoa, chọn mặc định như cũ, lưu không hỏi
- [ ] Key = 1: combo chỉ hiện tờ do người chỉ định tạo, chọn sẵn tờ mới nhất của người đó trong ngày chỉ định; không có tờ trong ngày → để trống
- [ ] Key = 1, đăng nhập nhunghtt, đổi Người chỉ định sang hant (hant có tờ hôm nay) → combo chỉ còn tờ của hant, chọn sẵn tờ hôm nay của hant
- [ ] Key = 1, đổi Người chỉ định sang người chưa có tờ trong ngày → combo chỉ hiện tờ của người đó (nếu có ngày khác), để trống
- [ ] Key = 1, chọn lại đúng người chỉ định đang chọn → giữ nguyên tờ đã chọn
- [ ] Key = 1, mở từ tờ của bác sĩ khác ở màn Tạo tờ điều trị → tờ đó không hiện, không được chọn
- [ ] Key = 1, đổi Thời gian chỉ định → nạp lại, vẫn chỉ hiện tờ của người chỉ định
- [ ] Key = 1, Lưu / Lưu in → không còn cảnh báo/chặn tờ của người khác hay "chưa có tờ"
