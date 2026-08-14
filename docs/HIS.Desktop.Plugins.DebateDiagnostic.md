# Biên bản hội chẩn — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.DebateDiagnostic |
| Loại | Form |
| Mục đích | Tạo, sửa, in biên bản hội chẩn của bệnh nhân. Quản lý danh sách thành phần tham gia (chủ tọa, thư ký, BS hội chẩn), danh sách mời và phản hồi tham gia của bác sĩ/khoa. |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

1. Khoa chủ trì lập biên bản hội chẩn → chọn loại hội chẩn, thành phần tham gia, danh sách mời (BS các khoa).
2. BS được mời nhận thông báo (SDA_NOTIFY) → vào module phản hồi (IS_PARTICIPATION = 1 = tham gia, 0 = từ chối).
3. Khoa chủ trì hoàn thiện kết luận, hướng điều trị, phương pháp → nhấn **Lưu biên bản**.
4. **B.4.4 — Trước khi lưu**: query danh sách khoa được mời (resolve qua LOGINNAME → V_HIS_EMPLOYEE.DEPARTMENT_ID). Nếu có khoa có user `IS_PARTICIPATION = null` → cảnh báo, người dùng chọn tiếp tục hoặc hủy.
5. Lưu biên bản qua `api/HisDebate/CreateAutoTracking` (Add) hoặc `api/HisDebate/Update` (Edit).
6. **B.4.4 — Sau khi lưu thành công**: tổng hợp chuỗi *Diễn biến tờ B* và gọi `api/HisDebate/UpdateWithTracking` với `IsAutoCreateTracking = 1` để backend tạo HIS_TRACKING tự động.

### Format chuỗi Diễn biến tờ B (frontend tự tổng hợp)

```
Hội chẩn {tên các khoa đã duyệt} đã duyệt phiếu mời hội chẩn.
Kết luận: {CONCLUSION}.
Hướng điều trị: {TREATMENT_METHOD}.
BS duyệt: {tên các BS có IS_PARTICIPATION = 1}
```

- **Khoa đã duyệt** = DISTINCT department của user có `IS_PARTICIPATION = 1` (resolve LOGINNAME → V_HIS_EMPLOYEE.DEPARTMENT_ID → HIS_DEPARTMENT.DEPARTMENT_NAME)
- **BS duyệt** = USERNAME của user có `IS_PARTICIPATION = 1`
- **Kết luận** ↔ `HIS_DEBATE.CONCLUSION` (cột comment DB: "Ket luan")
- **Hướng điều trị** ↔ `HIS_DEBATE.TREATMENT_METHOD` (cột comment DB: "Phuong phap dieu tri" — synonym của "Hướng điều trị" trong y khoa VN)
- Lưu ý: KHÔNG dùng `TREATMENT_TRACKING` (cột này = "Tóm tắt diễn biến điều trị")
- BS không có `HIS_EMPLOYEE.DEPARTMENT_ID` (chỉ có ACS_USER, không phải nhân viên) → bị filter ra, KHÔNG đưa vào cảnh báo (by design)

### Điều kiện nghiệp vụ

- Nếu `chkAutoSign.Checked` thì bắt buộc có Chủ tọa và Thư ký.
- LOGINNAME không được trùng trong danh sách invite_user và debate_user.
- COMMENT_DOCTOR không quá 1000 ký tự.
- Thời gian hội chẩn ≥ thời gian vào viện.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_DEBATE | Table | Biên bản hội chẩn (CONCLUSION, TREATMENT_TRACKING, TREATMENT_METHOD, DEBATE_TIME, DEPARTMENT_ID...) |
| HIS_DEBATE_USER | Table | Thành phần tham gia (chủ tọa, thư ký, BS) |
| HIS_DEBATE_INVITE_USER | Table | Danh sách mời (LOGINNAME, USERNAME, IS_PARTICIPATION, COMMENT_DOCTOR) |
| HIS_DEBATE_TEMP | Table | Mẫu hội chẩn (lưu sẵn) |
| HIS_TRACKING | Table | Diễn biến điều trị (tờ B) — auto-create từ UpdateWithTracking |
| V_HIS_EMPLOYEE | View | Resolve LOGINNAME → DEPARTMENT_ID |
| HIS_DEPARTMENT | Table | Resolve DEPARTMENT_ID → DEPARTMENT_NAME |
| ACS_USER | Table | Thông tin user (LOGINNAME, USERNAME) |

## 5. API Endpoints

| Action | URI | Consumer | DTO/SDO |
|--------|-----|----------|---------|
| Tạo + auto tracking | `api/HisDebate/CreateAutoTracking` | MosConsumer | `HisDebateCreateAutoTrackingSDO` |
| Cập nhật | `/api/HisDebate/Update` | MosConsumer | `HIS_DEBATE` |
| Cập nhật + tracking (B.4.4) | `api/HisDebate/UpdateWithTracking` | MosConsumer | `MOS.SDO.HisDebateUpdateWithTrackingSDO` |
| Xóa | `/api/HisDebate/Delete` | MosConsumer | id |
| Lấy danh sách mời | `api/HisDebateInviteUser/Get` | MosConsumer | `HisDebateInviteUserFilter` |
| Cập nhật mời | `api/HisDebateInviteUser/Update` | MosConsumer | `HIS_DEBATE_INVITE_USER` |
| Gửi thông báo | `api/SdaNotify/Create` | SdaConsumer | `SDA_NOTIFY` |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| Inventec.Common.RichEditor | Trích biên bản, sổ biên bản (template MPS000019, MPS000020) |
| HIS.Desktop.Library.EmrGenerate | Tạo input ký số EMR khi `chkAutoCreateEmr.Checked` |

## 7. Print

| Template | PrintTypeCode | Mục đích |
|----------|--------------|----------|
| Trích biên bản hội chẩn | MPS000019 | Trích phần kết luận |
| Sổ biên bản hội chẩn | MPS000020 | Sổ tổng hợp |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 11/08/2026 | nampp | **Fix lỗi với bệnh nhân ngoại trú/cấp cứu** (đi kèm việc Mời hội chẩn tại phòng khám cấp cứu). Combo "Khoa điều trị" chỉ được gán khi `vHisTreatment.LAST_DEPARTMENT_ID > 0` (`FormDebateDiagnostic.cs:919`); bệnh nhân cấp cứu chưa nhập khoa nên combo để trống, mà 2 chỗ tạo thông báo `SDA_NOTIFY` lại parse thẳng `Int64.Parse((cboDepartment.EditValue ?? "").ToString())` → `FormatException`, kéo theo mất toàn bộ thông báo mời tham gia hội chẩn. Thêm hàm dùng chung `GetSelectedDepartmentName()` (null-safe, `Int64.TryParse`, trả chuỗi rỗng + ghi log Warn khi không có khoa) và thay 2 vị trí: `FormDebateDiagnostic_Event.cs:1760`, `FormDebateDiagnostic.cs:3091`. |
| 22/05/2026 | phuongnm | **B.4.4 — Sửa đổi luồng Lưu biên bản hội chẩn**: (1) Trước khi lưu, kiểm tra danh sách khoa được mời, cảnh báo nếu có khoa `IS_PARTICIPATION = null`. (2) Sau khi lưu thành công (cả Add và Edit), tổng hợp chuỗi *Diễn biến tờ B* và gọi `api/HisDebate/UpdateWithTracking` với `IsAutoCreateTracking = 1`. Thêm URI constant `HIS_DEBATE_UPDATE_WITH_TRACKING`, dùng SDO `MOS.SDO.HisDebateUpdateWithTrackingSDO` (đã có trong MOS.SDO.dll), message resource `KhoaChuaDuyetPhieuMoi` (vi+en), 3 helper method (`ConfirmDepartmentsNotParticipated`, `BuildTrackingContentDevelopmentB`, `CallUpdateWithTracking`). |
| 23/05/2026 | phuongnm | **B.4.4 — Fix 2 bug sau khi test thực tế**: (1) "Hướng điều trị" trong chuỗi *Diễn biến tờ B* map sai field — đổi từ `TREATMENT_TRACKING` (Tóm tắt diễn biến điều trị) sang `TREATMENT_METHOD` (Phương pháp điều trị — đúng theo nghiệp vụ y khoa VN, verify qua DB column comment). (2) `CallUpdateWithTracking` nhận data từ `hisDebateResult` (API return — có thể partial) — đổi sang `hisDebateSave` (local form data đầy đủ) sau khi gán `hisDebateSave.ID = hisDebateResult.ID` để backend tìm đúng record. Verify: HIS_TRACKING.CONTENT giờ chứa đúng nội dung tab "Phương pháp điều trị" mà user nhập. |
| 23/05/2026 | phuongnm | **B.4.4 — Bổ sung quy tắc deploy**: Plugin có `.vi.resx`/`.en.resx` phải deploy CẢ satellite assemblies (`vi\<plugin>.resources.dll`, `en\<plugin>.resources.dll`), không chỉ main DLL. Lần deploy đầu thiếu satellite → dialog cảnh báo hiện rỗng. |
| 25/05/2026 | phuongnm | **B.4.4 — UX fix dialog chồng waiting indicator**: Trong `ConfirmDepartmentsNotParticipated`, gọi `WaitingManager.Hide()` ngay trước `XtraMessageBox.Show` (vì `btnSave_Click` đã gọi `WaitingManager.Show()` ở đầu). Sau khi user chọn "Có" → gọi `WaitingManager.Show()` lại để cover phần `SaveHisDebate` phía sau. Trải nghiệm: dialog cảnh báo không còn hiển thị chung với 5-dot waiting indicator. |

## 9. Test Cases

### B.4.4 — Lưu biên bản
- [ ] Mời nhiều BS thuộc nhiều khoa khác nhau → tất cả đều IS_PARTICIPATION=null → Nhấn Lưu → hiện cảnh báo liệt kê đúng tên các khoa.
- [ ] User chọn **No** trong cảnh báo → không gọi API save, không gọi UpdateWithTracking.
- [ ] User chọn **Yes** trong cảnh báo → save thành công → UpdateWithTracking được gọi với TrackingContent đúng format.
- [ ] Tất cả BS đã IS_PARTICIPATION=1 → không hiện cảnh báo → save → UpdateWithTracking gọi với chuỗi "Hội chẩn {khoa A, khoa B} đã duyệt phiếu mời hội chẩn. Kết luận: ... Hướng điều trị: ... BS duyệt: ...".
- [ ] Một số BS IS_PARTICIPATION=1, một số =null cùng khoa → khoa đó coi là "chưa duyệt" → hiện cảnh báo.
- [ ] Action = Edit (sửa biên bản đã lưu) → áp dụng cùng luồng (check khoa + UpdateWithTracking).
- [ ] Không có ai trong HIS_DEBATE_INVITE_USER → không cảnh báo → UpdateWithTracking vẫn được gọi với approvedDeptNames và approvedDoctorNames rỗng.
- [ ] Backend UpdateWithTracking trả null → log Warn, không ảnh hưởng đến message "Lưu thành công" của save chính.
