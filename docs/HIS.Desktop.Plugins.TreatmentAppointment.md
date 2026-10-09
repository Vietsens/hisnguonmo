# HIS.Desktop.Plugins.TreatmentAppointment — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TreatmentAppointment |
| Loại | Form (`frmTreatmentAppointment` kế thừa `FormBase`) |
| Mục đích | Danh sách bệnh nhân hẹn khám — tra cứu lịch hẹn, gọi nhắc thủ công, gửi tin nhắn Zalo nhắc tái khám hàng loạt. |
| Người tạo | Nhóm phát triển HIS |
| Ngày cập nhật | 09/10/2026 |
| Trạng thái | Đang phát triển — PTTK_57005 thêm gateway Zenify ZNS (FE xong, chờ Backend); trước đó PTTK_40213 PA2 tích hợp Zalo OA |

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

- Nút "Gửi tin nhắn nhắc tái khám" CHỈ hiển thị khi config `MOS.SMS.ZALO_ENABLE ∈ {1, 2, 3}`. Khi `= 0`, thiếu config hoặc giá trị lạ (chưa khai trong `EnumZaloEnable`) → nút và cột checkbox ẩn.
- `1` = gateway OneSMS (CONEK), `2` = gateway FNS ZNS (FPT), `3` = gateway Zenify ZNS (msghub.zenify.vn — việc 57005).
- Điều kiện bật/tắt do `EnumZaloEnableHelper.IsSendingMode` quyết định (1 chỗ duy nhất): thêm gateway mới chỉ cần khai thêm member trong `EnumZaloEnable`.
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

- Cột đầu (mới): **Checkbox `IsSelected`** — chỉ hiện khi `ZALO_ENABLE ∈ {1, 2, 3}` (theo `EnumZaloEnableHelper.IsSendingMode`).
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
| `MOS.SMS.ZALO_ENABLE` | `0`/null/giá trị lạ → ẩn nút + cột checkbox. `1` (OneSMS), `2` (FNS) hoặc `3` (Zenify) → hiện; nhãn gateway trong popup đổi theo giá trị. |
| `MOS.SMS.ZALO_TEMPLATE_PARAMS` | FE đọc để tô vàng nội dung xem trước theo **tên param thật** của template viện đăng ký (dạng `logical=ten_param_that` cách nhau `\|`). Không khai vẫn preview được các tên logical mặc định. |
| `CONFIG_KEY__NUM_PAGESIZE` | Page size mặc định |
| `TheVietCFG.DATE_BEFORE_NOTIFY_APPOINTMENT` | Số ngày mặc định cho `spnAppointmentDay` (gián tiếp qua backend) |

## 7. Print

Không có chức năng in trong module này.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 28/05/2026 | huannh | Bổ sung tích hợp Zalo OA (PTTK_40213 PA2 — yêu cầu 4.1.1): thêm cột checkbox multi-select, nút "Gửi tin nhắn nhắc tái khám" cạnh combo Phòng hẹn khám (hiển thị theo `MOS.SMS.ZALO_ENABLE`), popup `frmSelectZaloTemplate` chọn template + preview, gọi 2 API mới `GetZaloTemplates` + `SendAppointmentZalo`. Thêm `TreatmentAppointmentADO`, `ZaloTemplateADO`, `SendAppointmentZaloFilter`, `SendAppointmentZaloResultADO`, `HisRequestUriStore`, `EnumZaloEnable`. Mở rộng Resources (vi/en/my) và `ResourceMessageLang`. |
| 29/05/2026 | huannh | Refactor popup + dialog kết quả theo mockup PTTK_40213 (7 scene): (1) Đổi popup từ grid template sang **ComboBox dropdown** + **badge chất lượng** (`●●● HIGH` / `●●○ MEDIUM` / `●○○ LOW` với màu và tooltip), (2) Thêm header bar hiển thị "Số bệnh nhân: N" + "Gateway: OneSMS/FNS ZNS", (3) Preview header động "Nội dung xem trước (với bệnh nhân: <Tên> · <Mã>)", (4) **Fill placeholder thật** từ BN đầu danh sách (`{{ho_ten}}`, `{{ma_benh_nhan}}`, `{{ngay_tai_kham}}`, `{{khoa_kham}}`) — highlight vàng bằng `RichTextBox.SelectionBackColor`, (5) Note "Các giá trị tô vàng...", (6) Nút "Xác nhận gửi (N)" có hiển thị số BN, (7) Tạo dialog kết quả riêng `frmSendZaloResult` với header màu (xanh/cam/đỏ theo trạng thái) + heading "Đã gửi thành công X/Y..." + mô tả nghiệp vụ + memo chi tiết thất bại. Thêm 14 message key mới. |
| 06/08/2026 | nampp | PTTK_3145 (PT-53437) — Sửa lịch hẹn ngay tại danh sách: thêm menu chuột phải 2 item "Sửa hẹn khám" + "In giấy hẹn khám" (Mps000010 qua PrintTreatmentFinishProcessor). "Sửa hẹn khám" chỉ cho người tạo lịch hẹn (END_LOGINNAME == login hiện tại), gọi GetView4 lấy V_HIS_TREATMENT_4 rồi mở plugin AppointmentInfo kèm RefeshReference reload grid. Thêm 4 cột grid: Phòng hẹn khám (giải mã CSV APPOINTMENT_EXAM_ROOM_IDS — pre-computed vào ADO), Người hẹn khám (END_USERNAME), Thời gian sửa (MODIFY_TIME_STR), Người sửa (MODIFIER). File mới frmTreatmentAppointment__EditAppointment.cs; thêm refs ModuleExt, Inventec.UC.Login, Inventec.Token.ClientSystem, Library.PrintTreatmentFinish; sửa HintPath F:\histest về đường dẫn tương đối. |
| 18/09/2026 | nampp | PTTK_57005 — Tích hợp API Zalo ZNS đối tác Zenify (BVĐK Ninh Thuận): thêm `EnumZaloEnable.ZenifyZns = 3` + `EnumZaloEnableHelper.IsSendingMode` (1 chỗ duy nhất quyết định gateway nào là bật, thay 2 điều kiện liệt kê cứng OneSms/FnsZns ở `frmTreatmentAppointment__SendZalo`); `frmSelectZaloTemplate`: nhãn gateway thêm Zenify, `PLACEHOLDER_PATTERN` nhận cả 3 dạng `<x>` / `{{x}}` / `[x]` (Zalo OA / msghub Zenify / OneSMS), `AddTemplateParamAliases()` đọc `MOS.SMS.ZALO_TEMPLATE_PARAMS` để preview tô vàng theo tên param thật của viện, `ShowNoTemplateReason()` hiện lý do khi API không trả template nào (Zenify không có API liệt kê template nên danh sách lấy từ HIS_CONFIG). Thêm 2 message key `GatewayZenifyZns`, `KhongLayDuocDanhSachTemplateZalo` (vi/en/my). Backend do dev BE làm theo mục II.2 tài liệu PTTK 57005. |
| 06/10/2026 | nampp | PTTK_57005 — Sửa lỗi có sẵn ở màn kết quả gửi Zalo: dòng lỗi in "• mã điều trị (tên BN): lý do" nhưng Backend (`SendAppointmentZaloItemSDO`) chỉ trả `TreatmentId`/`PatientCode` → trên HIS thật ra "•  (): lý do". Thêm `FillMissingTreatmentInfo()` trong `frmTreatmentAppointment__SendZalo.cs`: sau khi gọi API gửi, điền `TreatmentCode`/`PatientName` còn trống từ danh sách đã tích (khớp `TreatmentId`), giá trị Backend đã trả thì giữ nguyên. Đồng thời cho phần xem trước mẫu tin (`BuildSampleDataMap`) dựng dữ liệu **giống hệt Backend** (`ZaloAppointmentReminderService`): 5 giá trị logical (họ tên cắt 30, SĐT ưu tiên di động → điện thoại → người thân cắt 15, mã BN, ngày hẹn từ `APPOINTMENT_DATE` rỗng thì hôm nay, khoa khám = TÊN KHOA từ `V_HIS_ROOM` thay cho tên phòng); có `MOS.SMS.ZALO_TEMPLATE_PARAMS` thì chỉ các tên đã ánh xạ (trùng khóa logical thì cặp sau thắng), so tên phân biệt hoa thường; bỏ 3 tên gán cứng của mẫu 17196 (viện đã khai ánh xạ cho mẫu đó thì xem trước không đổi). Harness form thật + máy chủ MOS giả lập 114/114 (`PTTK\57005\harness_test_FE`). |
| 09/10/2026 | nampp | PTTK_57005 — Theo phạm vi chốt với Zenify (mẫu 600882 `customer_name`/`code`/`date`, `code` = mã điều trị): phần xem trước thêm tên logical `ma_dieu_tri` = `TREATMENT_CODE`, **chỉ dùng khi có trong** `MOS.SMS.ZALO_TEMPLATE_PARAMS` (không ánh xạ thì vẫn đúng 5 tên logical cũ) — đúng quy tắc giao dev BE ở mục 0.3 PTTK 57005. Ánh xạ Ninh Thuận: `ho_ten=customer_name\|ma_dieu_tri=code\|ngay_tai_kham=date`. Harness 120/120 (thêm nhóm D7). |

## 9. Test Cases

### Tải danh sách

- [ ] Mở module → grid load mặc định "Chưa tái khám + Chưa gọi nhắc + Trong 0 ngày" → hiển thị đúng số dòng paging.
- [ ] Thay đổi `spnAppointmentDay` → reload, đồng thời lưu state qua `ControlStateWorker`.

### Nhắc / bỏ nhắc thủ công (giữ behavior cũ)

- [ ] Bấm icon "Nhắc hẹn" trên dòng chưa nhắc → API `AppointmentRemind` thành công → icon đổi sang "Hủy nhắc", grid refresh.
- [ ] Bấm icon "Hủy nhắc" → API `AppointmentUnremind` → quay về trạng thái chưa nhắc.

### Hiển thị/Ẩn nút Zalo theo config

- [ ] `MOS.SMS.ZALO_ENABLE = 0` (hoặc null, hoặc giá trị lạ như `4`) → nút "Gửi tin nhắn nhắc tái khám" và cột checkbox ẩn.
- [ ] `MOS.SMS.ZALO_ENABLE = 1` → nút và cột checkbox hiện. Popup hiển thị nhãn gateway "OneSMS (CONEK)".
- [ ] `MOS.SMS.ZALO_ENABLE = 2` → nút và cột checkbox hiện. Popup hiển thị nhãn gateway "FNS ZNS (FPT)".
- [ ] `MOS.SMS.ZALO_ENABLE = 3` → nút và cột checkbox hiện. Popup hiển thị nhãn gateway "Zenify ZNS".
- [ ] `= 3` mà backend chưa khai danh sách template (`MOS.SMS.ZENIFY_TEMPLATES` / `MOS.SMS.ZALO_TEMPLATE_IDS`) → popup hiện thông báo lý do, nút "Xác nhận gửi" bị khóa (không còn combo rỗng im lặng).

### Gửi tin nhắn Zalo

- [ ] Không tích dòng nào, bấm nút gửi → cảnh báo "Vui lòng chọn ít nhất một bệnh nhân".
- [ ] Tích vài dòng, bấm nút gửi → popup `frmSelectZaloTemplate` mở, label hiển thị "Đã chọn N bệnh nhân...".
- [ ] Popup load danh sách template từ API `GetZaloTemplates` → focus dòng đầu → memo preview hiện nội dung.
- [ ] Click dòng template khác → preview cập nhật theo template focus.
- [ ] Có template nhưng chưa chọn, bấm "Xác nhận gửi" → cảnh báo "Vui lòng chọn một template". (Không có template nào thì nút "Xác nhận gửi" bị khóa, không bấm được.)
- [ ] Bấm "Xác nhận gửi" với template hợp lệ → gọi API `SendAppointmentZalo` → popup tổng kết "Tổng số: N | Thành công: X | Thất bại: Y".
- [ ] Có dòng thất bại → popup hiển thị "Chi tiết các trường hợp gửi thất bại" kèm `TreatmentCode`, `PatientName`, `ErrorMessage`.
- [ ] Sau khi gửi thành công → grid refresh, các dòng đã gửi có cờ `IS_APPOINTMENT_REMINDED = 1` (theo backend).

### Localization

- [ ] Đổi ngôn ngữ vi → en → tất cả caption, button, tooltip, message popup chuyển ngôn ngữ.
- [ ] Lang.vi/en/my.resx và Message.Lang.vi/en/my.resx có đủ key tương ứng.
