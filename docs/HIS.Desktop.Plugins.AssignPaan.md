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
2. Nạp danh sách tờ điều trị của hồ sơ trong khoa đang làm việc (`LoadDataToTrackingCombo`, `api/HisTracking/GetView`).
3. Chọn tờ mặc định (`SetDefautTrackingCombo`): key `HIS.Desktop.Plugins.AssignPrescription.IsDefaultTracking` = 0 dùng tờ truyền vào; = 1 tờ mới nhất cùng ngày chỉ định trong khoa.
4. Lưu / Lưu in: `api/HisServiceReq/PaanCreate` (`ProcessSave`), gửi `TrackingId` của combo Tờ điều trị.

### Điều kiện nghiệp vụ
- `MOS.HIS_SERVICE_REQ.ASSIGN_SERVICES.IS_TRACKING_REQUIRED` = 1: bắt buộc chọn tờ điều trị với diện nội trú/ngoại trú điều trị.
- Việc 59656 — key `HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption` (1 cảnh báo, 2 chặn, trống = như cũ): chỉ tự chọn tờ do chính người chỉ định tạo (tờ truyền vào của người khác bị bỏ); khi lưu, tờ của người khác hoặc người chỉ định chưa có tờ trong ngày → cảnh báo/chặn (chỉ báo 'chưa có tờ' với diện nội trú/ngoại trú điều trị, bỏ qua khi mở từ màn Tạo tờ điều trị chưa lưu tờ).

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

## 9. Test Cases

### Việc 59656 - Y lệnh chỉ vào tờ điều trị của chính người chỉ định
- [ ] Key trống: chọn tờ của người khác vẫn lưu được, không hỏi (như cũ)
- [ ] Key = 2: chọn tờ điều trị của người khác → chặn, nêu tờ + người tạo
- [ ] Key = 1: như trên nhưng hỏi Có/Không
- [ ] Key bật, người chỉ định (BN nội trú) chưa có tờ trong ngày → cảnh báo/chặn "chưa có tờ điều trị ngày ..."
- [ ] Key bật, mở từ tờ của bác sĩ khác ở màn Tạo tờ điều trị → không chọn sẵn tờ đó
