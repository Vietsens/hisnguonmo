# Sửa Thông Tin Bệnh Nhân — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.PatientUpdate |
| Loại | Form (`frmPatientUpdate`, title "Sửa thông tin bệnh nhân") |
| Mục đích | Sửa thông tin hành chính / người thân / thông tin bệnh của bệnh nhân; từ 29/07/2026 kèm checklist "PN mang thai / PN cho con bú" phục vụ cảnh báo thuốc MIMS |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở form (từ Hồ sơ bệnh nhân / danh sách điều trị) với `V_HIS_PATIENT` hoặc cặp `patientId + treatmentId`.
2. Load dữ liệu bệnh nhân lên 3 group: Thông tin hành chính / Thông tin người thân / Thông tin bệnh.
3. Sửa thông tin → nút "Lưu (Ctrl S)" → validate → `api/HisPatient/UpdateSdo` → in tem/phiếu (nếu tick) → đóng form.

### Checklist Phân loại phụ nữ (MIMS — 29/07/2026)
- Config `HIS.Desktop.Mims.IsCheckPregnancyLactation` = 1 → nhóm control tạo RUNTIME trong group "Thông tin bệnh" (hàng checkbox bệnh, vị trí `emptySpaceItem5`): CheckEdit "PN mang thai" + SpinEdit số tháng (1-9) / CheckEdit "PN cho con bú" + SpinEdit số tháng.
- Chỉ enable khi giới tính = Nữ; đổi giới tính sang Nam → tự bỏ tick + disable (hook `cboGender1.EditValueChanged`).
- Tick "PN mang thai" thì BẮT BUỘC nhập số tháng 1-9 (`ValidMimsWomanClassify()` chặn lưu).
- Load async từ `HIS_MIMS_PATIENT_PROFILE` theo PATIENT_ID; lưu SAU khi `UpdateSdo` thành công (`SaveMimsWomanClassify()` — dirty-check, không đổi không gọi API; 1 bản ghi active/BN, update tại chỗ).
- Dữ liệu dùng CHUNG với tab "Phân loại phụ nữ" màn Xử trí khám (ExamServiceReqExecute) — đánh dấu nơi này nơi kia nhận biết được; các form kê đơn (PK/CLS/YHCT/Kidney) dùng bản ghi này để gửi `<PatientProfile>` vào MIMS.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_PATIENT | View | Bệnh nhân đang sửa (`currentVHisPatientDTO`) |
| HIS_PATIENT | Table | DTO lưu (`UpdatePatientDTOFromDataForm`) |
| HIS_GENDER | Table | Combo giới tính (`cboGender1`, EditValue = ID) |
| HIS_MIMS_PATIENT_PROFILE | Table (BE đang làm) | Trạng thái PN mang thai/cho con bú — qua DTO `MimsPatientProfileRecord` của thư viện MIMS |

## 4. UI Layout

3 GroupBox trong LayoutControl: "Thông tin hành chính" (`groupBox1`/`layoutControl2`), "Thông tin người thân" (`groupBox2`/`layoutControl3`), "Thông tin bệnh" (`groupBox3`/`layoutControl6` — hàng 0: BN mãn tính / CAPD / lao / HIV + panel Phân loại phụ nữ runtime). Hàng cuối: các checkbox in + nút Lưu.

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Load BN theo treatment | HisRequestUriStore.HIS_PATIENT_GETVIEW | MosConsumer |
| Lưu | api/HisPatient/UpdateSdo | MosConsumer |
| Cập nhật thẻ (quét thẻ) | api/HisPatient/UpdateCard | MosConsumer |
| Profile MIMS (load/save) | api/HisMimsPatientProfile/Get, Create, Update (**BE đang làm**) | MosConsumer |

## 6. Dependencies

| Thư viện | Mục đích |
|----------|----------|
| HIS.Desktop.MIMS.Integration | `MimsPatientProfileWorker` (Get/Save profile), `MimsPatientProfileRecord` |
| HIS.UC.WorkPlace | Nơi làm việc |
| MOS.LibraryHein | BHYT |

## 7. Print

In tem barcode / phiếu YC khám qua `ProcessPrint()` + `LoadConfigHisAcc()` (SDA_CONFIG_APP).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 29/07/2026 | nampp | **MIMS — Checklist PN mang thai / cho con bú** — Thêm nhóm control runtime vào group "Thông tin bệnh" (config `HIS.Desktop.Mims.IsCheckPregnancyLactation`, mặc định TẮT). Nữ mới enable; đổi giới tính → clear + disable; tick mang thai bắt buộc nhập số tháng 1-9 (chặn lưu); load async / lưu sau UpdateSdo thành công vào `HIS_MIMS_PATIENT_PROFILE` (dùng chung với màn Xử trí khám + 4 form kê đơn). Files: `frmPatientUpdate__Mims.cs` (MỚI), `frmPatientUpdate.cs` (gọi `InitMimsWomanClassify` sau `FillDataPatientToControl`), `frmPatientUpdate__Event.cs` (validate + save trong `btnSave_Click`), `Config.cs` (`IsCheckMimsPregnancyLactation`), csproj (+ ref `HIS.Desktop.MIMS.Integration`). |

## 9. Test Cases

### Checklist Phân loại phụ nữ
- [ ] Config tắt → không có nhóm control mới, form như cũ.
- [ ] Config bật + BN nữ → hiện 2 checkbox + 2 ô tháng ở group "Thông tin bệnh"; BN có bản ghi cũ → tick + số tháng hiển thị đúng.
- [ ] Config bật + BN nam → nhóm control disable.
- [ ] Đang tick, đổi giới tính sang Nam → bỏ tick + disable.
- [ ] Tick "PN mang thai" không nhập tháng → bấm Lưu bị chặn, báo lỗi tại ô số tháng.
- [ ] Tick + nhập tháng → Lưu → bảng HIS_MIMS_PATIENT_PROFILE có bản ghi; mở màn Xử trí khám thấy trạng thái tương ứng.
- [ ] Không thay đổi checklist → Lưu → KHÔNG gọi api/HisMimsPatientProfile (kiểm tra log).
- [ ] BE chưa deploy API → Lưu bệnh nhân vẫn thành công, chỉ log Warn.
