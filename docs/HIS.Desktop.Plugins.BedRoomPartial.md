# HIS.Desktop.Plugins.BedRoomPartial — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.BedRoomPartial |
| Loại | UserControl |
| Mục đích | Buồng bệnh — quản lý bệnh nhân theo buồng/giường, theo dõi y lệnh (CLS, dịch vụ, thuốc-vật tư) theo dõi tình trạng, ký số bệnh án, in ấn các phiếu liên quan |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Bác sĩ/điều dưỡng mở chức năng Buồng bệnh tại phòng điều trị
2. Chọn buồng bệnh → load danh sách bệnh nhân + lịch sử giường
3. Chọn bệnh nhân → load TreeList y lệnh (tab Tất cả / CLS / Thuốc-vật tư / Khác)
4. Thao tác y lệnh: sửa, xóa, theo dõi, ký EMR, in phiếu, đăng ký dịch vụ mới
5. Tách dịch vụ, gán giường mới, kết thúc điều trị

### Hiển thị đơn thuốc dự trù theo ngày dự trù (config `ShowAnticipatePresByUseDate`)

Mặc định TẮT — giữ nguyên hành vi cũ (đơn dự trù nằm ở ngày kê).

Khi BẬT, ngày hiển thị của y lệnh (`ngày hiệu lực`) tính như sau:

| Loại y lệnh | Ngày hiệu lực |
|---|---|
| Đơn thuốc dự trù | `date(USE_TIME)` |
| Còn lại | `date(INTRUCTION_TIME)` |

**Đơn thuốc dự trù** = `SERVICE_REQ_TYPE_ID` ∈ {`DONK`, `DONTT`, `DONDT`} **và** `USE_TIME` có giá trị **và** `date(USE_TIME) != date(INTRUCTION_TIME)`.
Vế so sánh ngày bắt buộc vì các plugin `AssignPrescription*` gán `USE_TIME = INTRUCTION_TIME` khi bỏ trống ô "Dự trù".
`DONM` (đơn máu), dịch vụ, giường **không** áp dụng — vẫn ở ngày kê và vẫn hiện chuỗi `"Dự trù: ..."` ở cột Khoa yêu cầu.

Chiến lược nạp: config bật → nạp toàn đợt 1 lần trong `LoadAnticipateCache` (gọi từ `SelectPatient`), đổi ngày lọc RAM (0 API call). Đợt > 90 ngày → fallback gọi API theo từng ngày kê nguồn.

Đơn dự trù nhiều ngày đã được backend **tách sẵn thành nhiều `HIS_SERVICE_REQ`**, mỗi đơn 1 `USE_TIME` — không dùng `USE_TIME_TO`.

### Điều kiện enable nút "Xóa y lệnh" trong TreeList (`ssRootSety.IsEnableDelete`)
Với `SERVICE_REQ_STT_ID == CXL` (chưa xử lý), enable khi thỏa **1 trong 3**:
1. Tài khoản đăng nhập là **người chỉ định** (`REQUEST_LOGINNAME == loginName`)
2. Tài khoản đăng nhập là **admin** (`CheckLoginAdmin.IsAdmin`)
3. Khoa chỉ định **trùng khoa làm việc** AND loại y lệnh là **Khám (KH)**
4. Loại y lệnh là **Giường (G)** AND tài khoản có quyền **HIS000053** (bổ sung theo việc 44693)

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_TREATMENT | Table | Điều trị hiện tại |
| L_HIS_TREATMENT_BED_ROOM | View | Lịch sử giường/buồng |
| V_HIS_BED_ROOM | View | Danh mục buồng |
| V_HIS_SERE_SERV / DHisSereServ2 | View | Dịch vụ đã thực hiện |
| HIS_SERVICE_REQ | Table | Y lệnh |
| HIS_SERE_SERV | Table | Sere-serv (chi tiết DV) |
| V_HIS_DEPARTMENT_TRAN | View | Chuyển khoa |

## 4. UI Layout

UserControl chính: `UCBedRoomPartial` (file ~3000+ dòng, có nhiều partial).
TreeList y lệnh: `UCTreeListService` — render theo `SereServADO` (root + child theo CONCRETE_ID__IN_SETY), 2 cột thao tác `rep_btnEdit_Enable/Disable` và `rep_btnDelete_Enable/Disable` (toggle theo `IsEnableEdit / IsEnableDelete`).

### UC sử dụng
| UC | Mục đích |
|----|----------|
| UCTreeListService | Cây y lệnh chi tiết |
| UCPatientSelect | Chọn bệnh nhân |

### Vùng thông tin hành chính
Hiển thị nhóm máu (`lblBloodType`) từ `TDL_PATIENT_BLOOD_ABO_CODE` + `TDL_PATIENT_BLOOD_RH_CODE` — 4 trường hợp:
- Có cả ABO + RH → hiển thị đủ. VD: `O; RH(-)`
- Chỉ có ABO → hiển thị ABO. VD: `A`
- Chỉ có RH → hiển thị RH. VD: `RH(-)`
- Không có cả 2 → không hiển thị (trống)

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Xóa y lệnh | api/HisServiceReq/Delete | MosConsumer |
| Sửa y lệnh | api/HisServiceReq/Update | MosConsumer |
| Lấy danh sách | api/HisServiceReq/GetView | MosConsumer |

## 6. Dependencies

### ACS — Phân quyền
| Mã control | Tên hiển thị | Mục đích |
|-----------|-------------|----------|
| HIS000053 | Xóa y lệnh giường | Cho phép xóa y lệnh giường của tài khoản khác khi loại y lệnh là Giường (`SERVICE_REQ_TYPE.ID__G`) |

Plugin reference `ACS.EFMODEL.dll`. Load quyền qua `GlobalVariables.AcsAuthorizeSDO.ControlInRoles` lưu vào cờ `hasDeleteBedPermission`. Gọi trong constructor `UCBedRoomPartial`.

### Library
- `HIS.Desktop.IsAdmin` — check admin
- `HIS.Desktop.Library.EmrGenerate` — ký số EMR
- `HIS.Desktop.Library.CacheClient` — ControlState

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.ContentSubclinical | Nút "Kết quả CLS" (luôn hiện, không gate key config) | `long TREATMENT_ID` + `List<string> { "VIEW_ONLY" }` + `Module` — KHÔNG truyền `DelegateSelectData` (chế độ chỉ xem, không chèn kết quả) |
| HIS.Desktop.Plugins.Debate, HisTrackingList, ServiceReqList, AggrHospitalFees, TreatmentHistory, BedHistory, ... | Các nút thanh chức năng còn lại | `TREATMENT_ID` + `Module` (pattern chung `__Pluss__EventBtn.cs`) |

## 7. Print

Plugin tích hợp nhiều mẫu in qua RichEditorStore + MpsPrinter, phụ thuộc loại y lệnh (đơn thuốc, phiếu yêu cầu DV, phiếu xét nghiệm, ...).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 22/05/2026 | dangth2 | Việc 44693 (Tài liệu 2671): Bổ sung điều kiện enable nút "Xóa y lệnh giường" trong `Run/UCBedRoomPartial.cs` (2 vị trí thiết lập `ssRootSety.IsEnableDelete`) — nếu loại y lệnh là Giường (`SERVICE_REQ_TYPE.ID__G`) VÀ tài khoản có quyền HIS000053 thì enable. Các trường hợp khác giữ nguyên. Thêm `Base/ControlCode.cs`, field `hasDeleteBedPermission`, method `LoadDeleteBedPermission()`. Reference `ACS.EFMODEL.dll`. |
| 06/07/2026 | phuongnm | Tài liệu 1223: Sửa hiển thị nhóm máu ở vùng thông tin hành chính (`Run/UCBedRoomPartial.cs`, `lblBloodType`). Trước đây điều kiện `abo && rh` khiến chỉ có 1 trong 2 giá trị thì không hiển thị. Sửa thành 4 trường hợp: có cả ABO+RH (`O; RH(-)`), chỉ ABO (`A`), chỉ RH (`RH(-)`), không có (trống). |
| 17/08/2026 | phuongnm | Hiển thị đơn thuốc dự trù theo ngày dự trù trên màn Buồng bệnh. Thêm config `HIS.Desktop.Plugins.BedRoomPartial.ShowAnticipatePresByUseDate` (mặc định TẮT). Khi bật: đơn thuốc dự trù xếp theo `USE_TIME` thay vì `INTRUCTION_TIME`, chuyển hẳn không lặp ở ngày kê; thêm 2 cột "Ngày kê"/"Ngày dự trù"; tô xanh lá đậm đơn dự trù; tab "Tất cả" gom nhóm riêng `Dự trù — kê ngày X`. Chỉ áp dụng đơn thuốc (`SERVICE_REQ_TYPE_ID` ∈ DONK/DONTT/DONDT), loại trừ đơn máu DONM và dịch vụ/giường. File mới `Run/UCBedRoomPartial__Pluss__Anticipate.cs`. Kèm dọn nợ hiệu năng: 2 khối `dataServiceReq.Where(...)` gọi 4 lần trong vòng lặp → `Dictionary.TryGetValue`. Tài liệu: `A_NghiepVu_/B_KyThuat_DonDuTruTheoNgayDuTru_BuongBenh.docx`. |
| 13/08/2026 | nampp | Việc 3170 (BV Điện Biên): Thêm nút "Kết quả CLS" trên thanh chức năng — mở màn xem kết quả cận lâm sàng (plugin `ContentSubclinical` chế độ CHỈ XEM) không cần vào tờ điều trị. Nút tạo RUNTIME trong `InitSubclinicalResultButton()` (`Run/UCBedRoomPartial.cs`, đặt sau nút "Danh sách y lệnh" qua `LayoutControlItem.Move`) — không sửa Designer vì dãy nút dùng toạ độ pixel cố định. **Luôn hiện, không gate key config** (bản đầu có key `ShowSubclinicalResultButton`, đã bỏ cùng ngày theo yêu cầu). Enable/disable theo bệnh nhân trong `SetEnableButton`. Handler `btnKetQuaCLS_Click` (`__Pluss__EventBtn.cs`), gỡ event trong `__Pluss__Dispose.cs`. Resource 3 ngôn ngữ `UCBedRoomPartial.btnKetQuaCLS.Text/ToolTip`. |

## 9. Test Cases

### Nút "Kết quả CLS" — việc 3170
- [ ] Nút "Kết quả CLS" hiện ngay sau "Danh sách y lệnh"; chưa chọn BN thì disable, chọn BN thì enable
- [ ] Nhấn nút → mở màn kết quả CLS đúng BN đang chọn, KHÔNG có ô tích/nút "Chọn (Ctrl S)"/6 tuỳ chọn chèn, có nút "Đóng"
- [ ] BN chưa có kết quả CLS → màn mở với cây rỗng, không popup lỗi
- [ ] Hồi quy: màn "Chọn kết quả CLS" mở từ tờ điều trị vẫn đủ ô tích + nút Chọn + chèn kết quả như cũ

### Xóa y lệnh giường — phân quyền HIS000053
- [ ] User KHÔNG có quyền HIS000053, KHÔNG là người chỉ định/admin → nút Xóa **disable** trên y lệnh giường
- [ ] User CÓ quyền HIS000053 → nút Xóa **enable** trên y lệnh giường (kể cả y lệnh do người khác tạo)
- [ ] User CÓ quyền HIS000053 nhưng y lệnh KHÔNG phải loại Giường → giữ nguyên logic cũ (không enable thêm)
- [ ] Y lệnh ở trạng thái khác CXL → nút Xóa luôn disable (không bị ảnh hưởng bởi quyền mới)
- [ ] Tài khoản admin / người chỉ định → vẫn enable nút Xóa như trước
