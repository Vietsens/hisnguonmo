# Thông tin sức khỏe bà mẹ - trẻ em — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.MchTreatmentExamService |
| Loại | Form (kế thừa `HIS.Desktop.Utility.FormBase`, class `UCMchTreatmentExamService`) |
| Mục đích | Nhập liệu và cập nhật thông tin sức khỏe bà mẹ - trẻ em theo QĐ 3412/QĐ-BYT cho một lượt điều trị. Gồm 6 mục: Khám sàng lọc, Trẻ em dưới 6 tuổi, Khám thai, Sinh đẻ (Mẹ / Con), Tránh thai, Phá thai |
| Người tạo | — |
| Ngày tạo | — |
| Trạng thái | Bảo trì |

Mở màn hình từ `HIS.Desktop.Plugins.MchExamServiceList` (Danh sách thông tin sức khỏe sinh sản) hoặc theo lượt điều trị.

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

```
Mở màn hình (theo HIS_TREATMENT hoặc theo V_MCH_EXAM_SERVICE)
  → Kiểm tra license MCH (CheckMchLicense)
  → Khởi tạo combo, nhóm ô tích chọn, validation, UC (TreeSereServ7, SecondaryIcd, UCAddress)
  → FillDataToForm: tra cứu HIS_TREATMENT + HIS_PATIENT, hiển thị khối Thông tin bệnh nhân,
    nạp cây dịch vụ, nạp lưới danh sách đợt khám của bệnh nhân
  → Người dùng chọn mục nhập liệu theo loại khám → nhập → Lưu
    → CreateBySdo (tạo mới) / UpdateBySdo (cập nhật)
```

### Ánh xạ mục nhập liệu ↔ loại đợt khám

| Tab index | Mục | EXAM_SERVICE_TYPE_ID | Bảng dữ liệu |
|-----------|-----|----------------------|--------------|
| 0 | Khám sàng lọc | 5 | `MCH_SCREENING` |
| 1 | Trẻ em dưới 6 tuổi | 6 | `MCH_CHILD` |
| 2 | Khám thai | 1 | `MCH_ANTENATAL_VISIT` |
| 3 | Sinh đẻ (Mẹ / Con) | 2 | `MCH_BIRTH_INFO` + `MCH_CHILD` |
| 4 | Tránh thai | 3 | `MCH_CONTRACEPTION` |
| 5 | Phá thai | 4 | `MCH_ABORTION` |

### Điều kiện nghiệp vụ

- Bệnh nhân nam trên 72 tháng tuổi: cảnh báo hồ sơ không phù hợp và không cho lưu. Bệnh nhân nam dưới 72 tháng: chỉ mở mục Trẻ em dưới 6 tuổi.
- Chuyển sang mục Trẻ em dưới 6 tuổi với bệnh nhân trên 72 tháng: cảnh báo, người dùng xác nhận mới vào.
- Không cho đổi loại đợt khám khi cập nhật hồ sơ đã lưu, trừ cặp Khám sàng lọc (5) ↔ Trẻ em dưới 6 tuổi (6) cho hồ sơ cũ.
- Ràng buộc trường bắt buộc theo QĐ 3412 áp dụng cố định theo từng mục — xem `frmMchTreatmentExamService_RequiredField3412.cs`.
- Mỗi nhóm ô tích chọn hoạt động như một nhóm chọn một phương án: tích một ô thì bỏ tích các ô còn lại trong cùng nhóm.
- Hai nhóm ở mục Khám sàng lọc là *Mục đích khám phụ khoa* và *Điều trị phụ khoa* không cho bỏ tích hết.
- **Điền mặc định ô tích chọn**: khi tạo mới hoặc sau khi bấm Mới, **47 nhóm** ô tích chọn thuộc phạm vi (gồm 45 nhóm bổ sung theo PTTK_3076 + 2 nhóm *Mục đích khám phụ khoa* và *Điều trị phụ khoa* đã có mặc định từ trước) thuộc 4 mục Khám sàng lọc, Trẻ em dưới 6 tuổi, Khám thai, Sinh đẻ được điền sẵn phương án đầu tiên (Bình thường / Không thực hiện / Không / Sống / Đẻ thường / Chưa cấp / Lần đầu / Không đủ). Nhóm đã có ô được chọn thì giữ nguyên. Mục Phá thai và Tránh thai không áp dụng.
- **Bảo toàn hồ sơ đã lưu**: mở hồ sơ để xem, sửa hoặc sao chép thì xóa toàn bộ trạng thái tích chọn trước khi nạp dữ liệu, đồng thời không tự lấy dữ liệu sang. Ô nào hồ sơ chưa từng ghi nhận vẫn để trống.
- **Phát sinh bản ghi thông tin con**: `MCH_CHILD` ở mục Sinh đẻ chỉ phát sinh khi có ít nhất một trong ba điều kiện: hồ sơ đã có bản ghi con, người dùng đã đổi một nhóm ô tích chọn khỏi phương án mặc định (giá trị khác phương án đầu tiên), hoặc có thông tin con dạng chữ / số / ngày được nhập hay lấy từ giấy chứng sinh. Nhóm ô tích chọn còn ở đúng phương án mặc định thì không tính là có thông tin.
  > Điều kiện "đã đổi khỏi mặc định" là bắt buộc: theo QĐ 3412, khi *Tử vong thai nhi = Có* thì không yêu cầu nhập thêm trường trẻ sơ sinh nào, nên chỉ tích một ô là đã đủ để lưu.

### Tự lấy thông tin sang (chỉ khi tạo mới, chỉ điền vào ô đang trống, lấy một lần khi mở mục)

| Mục | Ô đích | Nguồn | Xử lý |
|-----|--------|-------|-------|
| Khám thai | Cân nặng, Chiều cao | `HIS_DHST.WEIGHT`, `HEIGHT` | Lấy nguyên giá trị (kg, cm) |
| Khám thai | Huyết áp tâm thu / tâm trương | `HIS_DHST.BLOOD_PRESSURE_MAX` / `MIN` | Lấy nguyên giá trị |
| Khám thai | Vòng bụng | `HIS_DHST.BELLY` | Lấy nguyên giá trị |
| Khám thai | Chiều cao tử cung | *Không có nguồn* | Để trống, nhập tay |
| Trẻ em dưới 6 tuổi | Cân nặng | `HIS_DHST.WEIGHT` | **Quy đổi kg → gram (× 1000)** |
| Trẻ em dưới 6 tuổi | Chiều cao | `HIS_DHST.HEIGHT` | Lấy nguyên giá trị |
| Trẻ em dưới 6 tuổi | Vòng đầu | *Không có nguồn* | Để trống, nhập tay |
| Trẻ em dưới 6 tuổi | CCCD | `HIS_PATIENT.CCCD_NUMBER` | Điền nếu hồ sơ có |
| Sinh đẻ - Mẹ | Tỉnh / Huyện / Xã / Địa chỉ | `HIS_PATIENT.PROVINCE_CODE`, `DISTRICT_CODE`, `COMMUNE_CODE`, `ADDRESS` | Điền sẵn để tham khảo |
| Sinh đẻ - Con | Toàn bộ thông tin trẻ sơ sinh | `HIS_BABY` (giấy chứng sinh) | Ánh xạ theo `CopyBabyDataToChildTab` |

Nguồn dấu hiệu sinh tồn là bản ghi gần nhất của lượt điều trị (sắp theo `EXECUTE_TIME` giảm dần). Nguồn không có giá trị thì để trống, không điền số 0.

Người dùng vẫn lấy lại thông tin con bằng cách bấm biểu tượng giấy chứng sinh trên cây danh sách dịch vụ — dùng khi cần lấy lại sau khi đã sửa. Cả hai luồng (tự lấy khi mở mục và bấm biểu tượng) đều **không hiện hộp thoại thông báo**; riêng luồng bấm biểu tượng vẫn chuyển sang mục Sinh đẻ - Con để người dùng thấy kết quả.

### Tra cứu nguồn dữ liệu thất bại

Mỗi nguồn (dấu hiệu sinh tồn, giấy chứng sinh) chỉ được tra cứu một lần cho mỗi lượt điều trị. Nếu lần gọi API thất bại thì mục đó **chưa** bị đánh dấu là đã lấy, nên lần mở mục sau vẫn thử lại — người dùng không phải bấm Mới (thao tác này xóa dữ liệu đang nhập).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| `MCH_PATIENT` | Table | Bệnh nhân phía MCH |
| `MCH_TREATMENT` | Table | Lượt điều trị phía MCH |
| `MCH_EXAM_SERVICE` / `V_MCH_EXAM_SERVICE` | Table / View | Đợt khám — bản ghi gốc của mỗi lượt nhập liệu |
| `MCH_SCREENING` | Table | Mục Khám sàng lọc |
| `MCH_CHILD` | Table | Mục Trẻ em dưới 6 tuổi và mục Sinh đẻ - Con |
| `MCH_ANTENATAL_VISIT` | Table | Mục Khám thai |
| `MCH_BIRTH_INFO` | Table | Mục Sinh đẻ - Mẹ |
| `MCH_CONTRACEPTION` | Table | Mục Tránh thai |
| `MCH_ABORTION` | Table | Mục Phá thai |
| `HIS_TREATMENT` | Table | Lượt điều trị phía HIS — nguồn ánh xạ sang MCH |
| `HIS_PATIENT` | Table | Hồ sơ bệnh nhân — **nguồn đọc** cho địa chỉ và số định danh |
| `HIS_DHST` | Table | Dấu hiệu sinh tồn — **nguồn đọc** cho cân nặng, chiều cao, huyết áp, vòng bụng |
| `HIS_BABY` | Table | Giấy chứng sinh — **nguồn đọc** cho thông tin trẻ sơ sinh |
| `HIS_PATIENT_TYPE_ALTER` | Table | Thông tin thẻ BHYT gần nhất của lượt điều trị |
| `DHisSereServ2` / `V_HIS_SERE_SERV_7` | SDO / View | Cây dịch vụ khám và xét nghiệm của lượt điều trị |
| `HIS_GENDER`, `HIS_BRANCH`, `HIS_MEDI_ORG`, `HIS_ICD`, `SDA_ETHNIC` | Table | Danh mục |

### Quan hệ chính

- `MCH_PATIENT` → `MCH_TREATMENT` → `MCH_EXAM_SERVICE` (1-n)
- `MCH_EXAM_SERVICE` → `MCH_SCREENING` / `MCH_CHILD` / `MCH_ANTENATAL_VISIT` / `MCH_BIRTH_INFO` / `MCH_CONTRACEPTION` / `MCH_ABORTION` (1-1, qua `EXAM_SERVICE_ID`)
- `HIS_TREATMENT` → `HIS_DHST` (1-n, qua `TREATMENT_ID`)
- `HIS_TREATMENT` → `HIS_BABY` (1-n, qua `TREATMENT_ID`)

## 4. UI Layout

```
+---------------------------------------------------------------------------------+
| Mã hồ sơ [......] Mã BN [......] [Tìm kiếm]        [Mới (Ctrl N)] [Lưu (Ctrl S)]|
+---------------------------------------------------------------------------------+
| Thông tin bệnh nhân: Mã BN | Họ tên | Ngày sinh | Giới tính | Mã hồ sơ          |
|                      Số thẻ BHYT | Giá trị thẻ | Nơi KCB | Địa chỉ             |
+------------------------------+--------------------------------------------------+
| Cây dịch vụ khám / xét nghiệm| [Khám sàng lọc][Trẻ em dưới 6 tuổi][Khám thai]  |
| (TreeSereServ7)              | [Sinh đẻ][Tránh thai][Phá thai]                 |
| - biểu tượng giấy chứng sinh |   Ngày khám | Người khám | Trình độ              |
|   → lấy thông tin con        |   ... ô nhập liệu theo từng mục ...              |
|                              |   Riêng Sinh đẻ có 2 mục con: [Mẹ] [Con]        |
+------------------------------+--------------------------------------------------+
| Lưới danh sách đợt khám của bệnh nhân (sửa / sao chép / xóa)                    |
+---------------------------------------------------------------------------------+
```

### UC sử dụng

| UC | Panel | Mục đích |
|----|-------|----------|
| `HIS.UC.TreeSereServ7` | pnSereServ | Cây dịch vụ khám + xét nghiệm, biểu tượng giấy chứng sinh |
| `HIS.UC.SecondaryIcd` | panel3 | Nhóm chẩn đoán ở mục Sinh đẻ - Mẹ |
| `UCAddress` (nội bộ plugin) | panel2 / panel1 | Địa chỉ nơi đẻ phần Mẹ / phần Con |

## 5. API Endpoints

| Action | URI | Consumer | Filter / DTO |
|--------|-----|----------|--------------|
| Tra cứu lượt điều trị | `api/HisTreatment/Get` | MosConsumer | `HisTreatmentFilter` |
| Tra cứu bệnh nhân | `api/HisPatient/Get` | MosConsumer | `HisPatientFilter` |
| Tra cứu thẻ BHYT | `api/HisPatientTypeAlter/Get` | MosConsumer | `HisPatientTypeAlterViewFilter` |
| Tra cứu dịch vụ | `api/HisSereServ/GetDHisSereServ2` | MosConsumer | `DHisSereServ2Filter` |
| Tra cứu dấu hiệu sinh tồn | `api/HisDhst/Get` | MosConsumer | `HisDhstFilter` |
| Tra cứu giấy chứng sinh | `api/HisBaby/Get` | MosConsumer | `HisBabyFilter` |
| Danh sách đợt khám | `api/MchExamService/GetView` | MchConsumer | `MchExamServiceViewFilter` |
| Tạo mới đợt khám | `api/MchExamService/CreateBySdo` | MchConsumer | `MchExamServiceCreateBySDO` |
| Cập nhật đợt khám | `api/MchExamService/UpdateBySdo` | MchConsumer | `MchExamServiceUpdateBySDO` |
| Tra cứu BN / lượt điều trị MCH | `api/MchPatient/Get`, `api/MchTreatment/Get` | MchConsumer | `MchPatientFilter`, `MchTreatmentFilter` |
| Tra cứu chi tiết từng mục | `api/MchScreening/Get`, `api/MchChild/Get`, `api/MchAntenatalVisit/Get`, `api/MchBirthInfo/Get`, `api/MchContraception/Get`, `api/MchAbortion/Get` | MchConsumer | Filter tương ứng |

## 6. Dependencies

### Library Plugins

| Library | Mục đích |
|---------|----------|
| `HIS.Desktop.Library.CacheClient` | Truy cập cache dữ liệu danh mục |

### Inter-Plugin

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| `HIS.Desktop.Plugins.SereServTein` | Bấm biểu tượng trên nút dịch vụ xét nghiệm | `long` sereServId, `Module` |
| `HIS.Desktop.Plugins.ExamServiceReqResult` | Bấm biểu tượng trên nút dịch vụ khám | `long` sereServId |

Màn hình được mở từ `HIS.Desktop.Plugins.MchExamServiceList`, nhận `Module`, `HIS_TREATMENT`, `V_MCH_EXAM_SERVICE`, `RefeshReference`.

## 7. Print

Không có chức năng in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 28/07/2026 | tuanln | Tạo tài liệu module. Hiện thực PTTK_3076: (1) điền sẵn phương án đầu tiên cho 47 nhóm ô tích chọn thuộc 4 mục Khám sàng lọc, Trẻ em dưới 6 tuổi, Khám thai, Sinh đẻ khi tạo mới; (2) xóa trạng thái tích chọn trước khi nạp hồ sơ đã lưu hoặc sao chép hồ sơ; (3) loại các nhóm ô tích chọn khỏi điều kiện phát sinh bản ghi `MCH_CHILD`; (4) tự lấy dấu hiệu sinh tồn sang mục Khám thai và Trẻ em dưới 6 tuổi, quy đổi cân nặng kg → gram cho mục trẻ em; (5) tự lấy thông tin hành chính của người bệnh sang mục Sinh đẻ - Mẹ và số định danh sang mục Trẻ em dưới 6 tuổi; (6) tự lấy thông tin con từ giấy chứng sinh khi mở mục Sinh đẻ, bỏ hộp thoại thông báo |

## 9. Test Cases

### Điền mặc định ô tích chọn

- [ ] Mở giao diện tạo mới → toàn bộ ô tích chọn của 4 mục đã được điền sẵn phương án thông thường
- [ ] Bỏ tích rồi tích lại một số ô → lưu → hệ thống ghi nhận đúng lựa chọn cuối cùng
- [ ] Bấm Mới → dữ liệu bị xóa, giữ Ngày khám / Người khám / Trình độ, ô tích chọn được điền lại mặc định
- [ ] Mục Phá thai (nhóm Kết quả soi mô) và mục Tránh thai → không có ô tích chọn nào được điền sẵn

### Bảo toàn hồ sơ đã lưu

- [ ] Mở hồ sơ đã lưu từ trước khi triển khai tính năng → các ô hồ sơ chưa ghi nhận hiển thị trống, không hiện giá trị mặc định
- [ ] Lưu lại hồ sơ cũ mà không sửa gì → dữ liệu cũ không thay đổi
- [ ] Sao chép hồ sơ đã lưu → hiển thị theo dữ liệu hồ sơ nguồn, không áp giá trị mặc định

### Tự lấy dữ liệu sang

- [ ] Lượt điều trị đã có dấu hiệu sinh tồn → mở mục Khám thai → cân nặng, chiều cao, huyết áp, vòng bụng được điền sẵn; ô Chiều cao tử cung để trống
- [ ] Mở mục Trẻ em dưới 6 tuổi → cân nặng hiển thị theo gram (kg × 1000), chiều cao theo cm; ô Vòng đầu để trống; ô CCCD điền theo hồ sơ bệnh nhân
- [ ] Lượt điều trị chưa ghi nhận dấu hiệu sinh tồn → các ô để trống, không điền số 0
- [ ] Sửa cân nặng ở mục Khám thai → chuyển sang mục khác → quay lại → giá trị đã sửa được giữ nguyên
- [ ] Mở mục Sinh đẻ với bệnh nhân đã có giấy chứng sinh → thông tin con và địa chỉ nơi sinh điền sẵn, không hiện hộp thoại thông báo
- [ ] Mở mục Sinh đẻ với bệnh nhân chưa có giấy chứng sinh → phần Con để trống, phần Mẹ vẫn điền địa chỉ theo hồ sơ bệnh nhân
- [ ] Bấm biểu tượng giấy chứng sinh trên cây dịch vụ sau khi đã sửa → thông tin con được lấy lại

### Phát sinh bản ghi thông tin con

- [ ] Lưu đợt khám Sinh đẻ mà không nhập thông tin con nào và không có giấy chứng sinh → không phát sinh bản ghi `MCH_CHILD`
- [ ] Lưu đợt khám Sinh đẻ có nhập thông tin con → phát sinh bản ghi `MCH_CHILD` đầy đủ
- [ ] **Ca thai tử vong**: chỉ tích *Tử vong thai nhi = Có*, không nhập trường trẻ sơ sinh nào → lưu được và `MCH_CHILD.IS_DEATH = 1` được ghi nhận
- [ ] Chỉ đổi một ô tích chọn phần Con khỏi mặc định (VD *Sàng lọc sơ sinh = Có*) → bản ghi `MCH_CHILD` được phát sinh với đúng lựa chọn đó
- [ ] Sửa hồ sơ đã có bản ghi `MCH_CHILD` nhưng xóa hết các ô dạng chữ / số → bản ghi con vẫn được cập nhật, không bị bỏ qua

### Nghiệp vụ sẵn có (kiểm tra không hồi quy)

- [ ] Bệnh nhân nam trên 6 tuổi → cảnh báo hồ sơ không phù hợp, không cho lưu
- [ ] Ràng buộc trường bắt buộc theo QĐ 3412 vẫn chặn lưu khi thiếu trường
- [ ] Không cho đổi loại đợt khám khi cập nhật hồ sơ đã lưu
