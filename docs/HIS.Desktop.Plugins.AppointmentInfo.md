# Thông Tin Hẹn Khám — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.AppointmentInfo |
| Loại | Form (popup `fromAppointmentInfo`) |
| Mục đích | Xem/sửa thông tin hẹn khám của 1 hồ sơ điều trị đã kết thúc: ngày hẹn, phòng khám hẹn (nhiều phòng), khung giờ, lời dặn. Được mở từ Danh sách điều trị (TreatmentList) và Danh sách bệnh nhân hẹn khám (TreatmentAppointment). |
| Người tạo | (kế thừa — plugin có sẵn) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Plugin cha truyền `V_HIS_TREATMENT_4` + `RefeshReference` → form load thông tin hẹn hiện tại (parse `APPOINTMENT_EXAM_ROOM_IDS` CSV → tick phòng).
2. Người dùng sửa ngày hẹn / phòng khám / khung giờ / lời dặn → Lưu → `api/HisTreatment/UpdateAppointmentInfo` → callback `refresh()` cho plugin cha.

### Điều kiện nghiệp vụ (validate khi Lưu)
- Ngày hẹn mới **không được nhỏ hơn ngày hiện tại** (PTTK_3145 — chặn).
- Ngày hẹn không nhỏ hơn ngày kết thúc điều trị (`OUT_TIME`) — chặn.
- Vượt số ngày hẹn tối đa (`MOS.HIS_TREATMENT.MAX_OF_APPOINTMENT_DAYS`): cảnh báo hoặc chặn theo `WARNING_OPTION_...`.
- Ngày hẹn rơi T7/CN/ngày lễ (`HIS_HOLIDAY_POLICIES.IS_WARNING_APPOINTMENT=1`) — cảnh báo Yes/No.
- Phòng vượt định mức hẹn/ngày (`api/HisExecuteRoom/GetCountAppointed`) — chặn.
- Backend chỉ cho update khi `TREATMENT_END_TYPE_ID ∈ {Hẹn, Ra viện, Xin ra viện}`.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TREATMENT_4 | View | Hồ sơ điều trị đầu vào (args) |
| HIS_EXECUTE_ROOM | Table (cache) | Danh sách phòng khám (IS_EXAM=1) |
| HIS_TREATMENT | Table | Kết quả trả về sau update |

Trường hẹn khám trên HIS_TREATMENT: `APPOINTMENT_TIME`, `APPOINTMENT_EXAM_ROOM_IDS` (CSV ROOM_ID), `APPOINTMENT_PERIOD_ID`, `ADVISE`.

## 4. UI Layout

Popup: ngày hẹn (DateEdit) + số ngày hẹn (Spin) + khung giờ (combo từ `api/HisAppointmentPeriod/GetCountByDate`) + grid phòng khám (checkbox) + lời dặn (Memo) + nút Dịch vụ hẹn khám (mở plugin AppointmentService) + Lưu.

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Cập nhật hẹn khám | api/HisTreatment/UpdateAppointmentInfo (`TreatmentAppointmentInfoSDO`) | MosConsumer |
| Khung giờ theo ngày | api/HisAppointmentPeriod/GetCountByDate | MosConsumer |
| Định mức phòng | api/HisExecuteRoom/GetCountAppointed | MosConsumer |

## 6. Dependencies

| Plugin liên quan | Quan hệ |
|------------------|---------|
| HIS.Desktop.Plugins.TreatmentList | Mở form này qua right-click "Thông tin hẹn khám" |
| HIS.Desktop.Plugins.TreatmentAppointment | Mở form này qua right-click "Sửa hẹn khám" (PTTK_3145) |
| HIS.Desktop.Plugins.AppointmentService | Mở từ nút "Dịch vụ hẹn khám" (args: treatmentId) |

Args đầu vào (Behavior): `Module`, `V_HIS_TREATMENT_4` (bắt buộc), `RefeshReference` (callback reload).

## 7. Print

Không in trực tiếp trong module (in Giấy hẹn khám Mps000010 do plugin cha đảm nhiệm).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 06/08/2026 | nampp | PA1 nhật ký tác động: sau khi Lưu thành công, ghi SDA_EVENT_LOG qua api/SdaEventLog/Create (helper Sda\SdaEventLogCreate.cs theo pattern ExamServiceReqExecute) — Description ghi TREATMENT_CODE + ngày hẹn cũ→mới + ROOM_ID cũ→mới, tra cứu tại màn Nhật ký sự kiện (plugin HIS.Desktop.plugins.EventLog). Thêm refs SDA.SDO, Inventec.UC.Login, Inventec.Token.ClientSystem. |
| 06/08/2026 | nampp | Tạo tài liệu module. PTTK_3145 (PT-53437): thêm validate khi Lưu — chặn ngày hẹn nhỏ hơn ngày hiện tại (message `CanhBaoNgayHenNhoHonNgayHienTai`, Resources/Message.vi+en.resx + ResourceMessage.cs). Tạo Properties/licenses.licx (thiếu trong repo, gây lỗi build LC0000). |

## 9. Test Cases

- [ ] Sửa ngày hẹn hợp lệ (>= hôm nay) → Lưu OK, plugin cha reload.
- [ ] Chọn ngày hẹn < ngày hiện tại → Lưu bị chặn, thông báo, focus lại control ngày hẹn.
- [ ] Ngày hẹn < ngày ra viện → chặn (hành vi cũ giữ nguyên).
- [ ] Ngày hẹn T7/CN/lễ → cảnh báo Yes/No (hành vi cũ giữ nguyên).
- [ ] Đổi phòng khám hẹn → Lưu → APPOINTMENT_EXAM_ROOM_IDS cập nhật CSV mới.
