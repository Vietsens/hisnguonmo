# HIS.Desktop.Plugins.TreatmentAppointment — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TreatmentAppointment |
| Loại | Form (`frmTreatmentAppointment` kế thừa `FormBase`) |
| Mục đích | Danh sách bệnh nhân hẹn khám — tra cứu lịch hẹn, gọi nhắc thủ công, gửi tin nhắn Zalo nhắc tái khám hàng loạt. |
| Người tạo | Nhóm phát triển HIS |
| Ngày cập nhật | 28/05/2026 |
| Trạng thái | Đang phát triển — bổ sung tích hợp Zalo OA (PTTK_40213 PA2) |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

1. User mở module **Danh sách bệnh nhân hẹn khám**.
2. Form load với bộ lọc mặc định: "Chưa tái khám" + "Chưa gọi nhắc" + "Đến ngày hẹn khám trong N ngày".
3. Grid hiển thị danh sách điều trị có lịch hẹn tái khám (`APPOINTMENT_TIME != null`).
4. User có thể:
   - Bấm icon "Nhắc hẹn" trên từng dòng → gọi `AppointmentRemind` đánh dấu đã nhắc.
   - Bấm icon "Hủy nhắc" → gọi `AppointmentUnremind`.
   - **(Mới)** Tích chọn nhiều dòng + bấm "Gửi tin nhắn nhắc tái khám" → mở popup chọn template Zalo → xác nhận gửi.

### Điều kiện nghiệp vụ

- Nút "Gửi tin nhắn nhắc tái khám" CHỈ hiển thị khi config `MOS.SMS.ZALO_ENABLE ∈ {1, 2}`. Khi `= 0` hoặc thiếu config → nút và cột checkbox ẩn.
- `1` = gateway OneSMS (CONEK), `2` = gateway FNS ZNS (FPT).
- Phải tích chọn ít nhất 1 bệnh nhân mới gửi được. Backend chỉ gửi cho điều trị có số điện thoại di động hợp lệ.
- Sau khi gửi thành công, backend cập nhật `HIS_TREATMENT.IS_APPOINTMENT_REMINDED = 1` và `APPOINTMENT_REMIND_TIME`.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| `HIS_TREATMENT` | Table | Điều trị — nguồn dữ liệu chính của grid (lọc theo APPOINTMENT_TIME) |
| `HIS_DEPARTMENT` | Table | Combo "Khoa kết thúc" |
| `V_HIS_EXECUTE_ROOM` | View | Combo "Phòng hẹn khám" |
| `TreatmentAppointmentADO` | ADO (mở rộng HIS_TREATMENT) | Thêm `IsSelected` để bind cột checkbox multi-select |

### Quan hệ chính

- `HIS_TREATMENT.LAST_APPOINTMENT_EXAM_ROOM_ID` → `V_HIS_EXECUTE_ROOM.ROOM_ID` (phòng hẹn khám).
- `HIS_TREATMENT.END_DEPARTMENT_ID` → `HIS_DEPARTMENT.ID` (khoa kết thúc).

## 4. UI Layout

### Sơ đồ giao diện

```
+----------------------------------------------------------------------------------+
| [Đã tái khám] [Chưa tái khám] [Chưa gọi nhắc] [Đã gọi nhắc] [Khoa] [Option] [N]ngày |
| [Mã hẹn] [Mã BN] [Từ khóa]                  [Tìm kiếm]  [Phòng hẹn khám] [Gửi Zalo] |
+----------------------------------------------------------------------------------+
| ☐ | STT | Nhắc | Trạng thái | Mã BN | Mã ĐT | Tên BN | ... | Ngày hẹn | ICD     |
+----------------------------------------------------------------------------------+
| [Phân trang ucPaging]                                                            |
+----------------------------------------------------------------------------------+
```

### Bộ lọc

- Radio: Đã tái khám / Chưa tái khám / Đã gọi nhắc / Chưa gọi nhắc.
- ComboBox đa chọn: Khoa kết thúc, Phòng hẹn khám.
- Combo option: Đến ngày hẹn khám trong N ngày / Đã quá / Trong khoảng.
- Text: Mã hẹn khám, Mã BN, Từ khóa.

### Grid

- Cột đầu (mới): **Checkbox `IsSelected`** — chỉ hiện khi `ZALO_ENABLE ∈ {1, 2}`.
- STT (unbound), Nhắc hẹn (RepositoryItemButtonEdit), Trạng thái (icon), Trạng thái text.
- Mã BN, Mã ĐT, Tên BN, Giới tính, Ngày sinh, Địa chỉ, SĐT, Ngày hẹn khám, Thời gian vào, Chẩn đoán chính.

### UC sử dụng

| UC | Mục đích |
|----|----------|
| `Inventec.UC.Paging.UcPaging` | Phân trang server-side |
| `DevExpress.XtraEditors.GridLookUpEdit` + `GridCheckMarksSelection` | Combo đa chọn Khoa / Phòng |

## 5. API Endpoints

Định nghĩa tập trung trong `HisRequestUriStore.cs`:

| Action | URI | Consumer | Filter / Body | Mục đích |
|--------|-----|----------|---------------|----------|
| Lấy danh sách | `api/HisTreatment/Get` | MosConsumer | `HisTreatmentFilter` | Load grid (paging) |
| Đánh dấu nhắc | `api/HisTreatment/AppointmentRemind` | MosConsumer | `long treatmentId` | Bật cờ đã gọi nhắc |
| Bỏ đánh dấu | `api/HisTreatment/AppointmentUnremind` | MosConsumer | `long treatmentId` | Tắt cờ đã gọi nhắc |
| **(Mới)** Lấy template Zalo | `api/HisTreatment/GetZaloTemplates` | MosConsumer | — | Trả danh sách `ZaloTemplateADO` |
| **(Mới)** Gửi tin Zalo | `api/HisTreatment/SendAppointmentZalo` | MosConsumer | `SendAppointmentZaloFilter` | Gửi tin cho danh sách `TreatmentIds` + `TemplateId` → `SendAppointmentZaloResultADO` |

## 6. Dependencies

### Library Plugins

| Library | Mục đích |
|---------|----------|
| `HIS.Desktop.LocalStorage.HisConfig` | Đọc `MOS.SMS.ZALO_ENABLE` từ cache HIS_CONFIG để ẩn/hiện nút Zalo |
| `HIS.Desktop.Library.CacheClient` | `ControlStateWorker` lưu/đọc trạng thái `spnAppointmentDay` |
| `HIS.Desktop.LocalStorage.BackendData` | Cache RAM cho combo Khoa / Phòng |
| `Inventec.Common.Mapper` | Map `HIS_TREATMENT` → `TreatmentAppointmentADO` (giữ field gốc, thêm `IsSelected`) |

### Inter-Plugin

Hiện tại không mở plugin khác. Tích hợp Zalo thực hiện qua API backend.

### Config

| Key | Tác động |
|-----|----------|
| `MOS.SMS.ZALO_ENABLE` | `0`/null → ẩn nút + cột checkbox. `1` (OneSMS) hoặc `2` (FNS) → hiện. |
| `CONFIG_KEY__NUM_PAGESIZE` | Page size mặc định |
| `TheVietCFG.DATE_BEFORE_NOTIFY_APPOINTMENT` | Số ngày mặc định cho `spnAppointmentDay` (gián tiếp qua backend) |

## 7. Print

Không có chức năng in trong module này.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 28/05/2026 | huannh | Bổ sung tích hợp Zalo OA (PTTK_40213 PA2 — yêu cầu 4.1.1): thêm cột checkbox multi-select, nút "Gửi tin nhắn nhắc tái khám" cạnh combo Phòng hẹn khám (hiển thị theo `MOS.SMS.ZALO_ENABLE`), popup `frmSelectZaloTemplate` chọn template + preview, gọi 2 API mới `GetZaloTemplates` + `SendAppointmentZalo`. Thêm `TreatmentAppointmentADO`, `ZaloTemplateADO`, `SendAppointmentZaloFilter`, `SendAppointmentZaloResultADO`, `HisRequestUriStore`, `EnumZaloEnable`. Mở rộng Resources (vi/en/my) và `ResourceMessageLang`. |
| 29/05/2026 | huannh | Refactor popup + dialog kết quả theo mockup PTTK_40213 (7 scene): (1) Đổi popup từ grid template sang **ComboBox dropdown** + **badge chất lượng** (`●●● HIGH` / `●●○ MEDIUM` / `●○○ LOW` với màu và tooltip), (2) Thêm header bar hiển thị "Số bệnh nhân: N" + "Gateway: OneSMS/FNS ZNS", (3) Preview header động "Nội dung xem trước (với bệnh nhân: <Tên> · <Mã>)", (4) **Fill placeholder thật** từ BN đầu danh sách (`{{ho_ten}}`, `{{ma_benh_nhan}}`, `{{ngay_tai_kham}}`, `{{khoa_kham}}`) — highlight vàng bằng `RichTextBox.SelectionBackColor`, (5) Note "Các giá trị tô vàng...", (6) Nút "Xác nhận gửi (N)" có hiển thị số BN, (7) Tạo dialog kết quả riêng `frmSendZaloResult` với header màu (xanh/cam/đỏ theo trạng thái) + heading "Đã gửi thành công X/Y..." + mô tả nghiệp vụ + memo chi tiết thất bại. Thêm 14 message key mới. |

## 9. Test Cases

### Tải danh sách

- [ ] Mở module → grid load mặc định "Chưa tái khám + Chưa gọi nhắc + Trong 0 ngày" → hiển thị đúng số dòng paging.
- [ ] Thay đổi `spnAppointmentDay` → reload, đồng thời lưu state qua `ControlStateWorker`.

### Nhắc / bỏ nhắc thủ công (giữ behavior cũ)

- [ ] Bấm icon "Nhắc hẹn" trên dòng chưa nhắc → API `AppointmentRemind` thành công → icon đổi sang "Hủy nhắc", grid refresh.
- [ ] Bấm icon "Hủy nhắc" → API `AppointmentUnremind` → quay về trạng thái chưa nhắc.

### Hiển thị/Ẩn nút Zalo theo config

- [ ] `MOS.SMS.ZALO_ENABLE = 0` (hoặc null) → nút "Gửi tin nhắn nhắc tái khám" và cột checkbox ẩn.
- [ ] `MOS.SMS.ZALO_ENABLE = 1` → nút và cột checkbox hiện. Tooltip "Gateway OneSMS".
- [ ] `MOS.SMS.ZALO_ENABLE = 2` → nút và cột checkbox hiện. Tooltip "Gateway FNS ZNS".

### Gửi tin nhắn Zalo

- [ ] Không tích dòng nào, bấm nút gửi → cảnh báo "Vui lòng chọn ít nhất một bệnh nhân".
- [ ] Tích vài dòng, bấm nút gửi → popup `frmSelectZaloTemplate` mở, label hiển thị "Đã chọn N bệnh nhân...".
- [ ] Popup load danh sách template từ API `GetZaloTemplates` → focus dòng đầu → memo preview hiện nội dung.
- [ ] Click dòng template khác → preview cập nhật theo template focus.
- [ ] Bấm "Xác nhận gửi" mà không chọn template (trường hợp grid rỗng) → cảnh báo "Vui lòng chọn một template".
- [ ] Bấm "Xác nhận gửi" với template hợp lệ → gọi API `SendAppointmentZalo` → popup tổng kết "Tổng số: N | Thành công: X | Thất bại: Y".
- [ ] Có dòng thất bại → popup hiển thị "Chi tiết các trường hợp gửi thất bại" kèm `TreatmentCode`, `PatientName`, `ErrorMessage`.
- [ ] Sau khi gửi thành công → grid refresh, các dòng đã gửi có cờ `IS_APPOINTMENT_REMINDED = 1` (theo backend).

### Localization

- [ ] Đổi ngôn ngữ vi → en → tất cả caption, button, tooltip, message popup chuyển ngôn ngữ.
- [ ] Lang.vi/en/my.resx và Message.Lang.vi/en/my.resx có đủ key tương ứng.
