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
3. Chọn bệnh nhân → load TreeList y lệnh (tab Tất cả / CLS / Thuốc-vật tư / Khác / Khám bệnh; key `ClsTabOption` bật thì tab CLS tách thành các tab nhóm — xem mục dưới)
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

### Tách tab CLS theo nhóm dịch vụ (việc 57971, key `HIS.Desktop.Plugins.BedRoomPartial.ClsTabOption`)

Mặc định TẮT (trống / khác 1, 2) — 1 tab "CLS" như cũ, sau mỗi lần nạp cây luôn quay về tab Tất cả.

| Giá trị | Hành vi |
|---|---|
| 1 | Ẩn tab "CLS", chèn các tab nhóm ngay sau "Tất cả"; **luôn hiện** Xét nghiệm, Siêu âm, X-quang, TDCN, Nội soi; CT / MRI, Giải phẫu bệnh, CLS khác, tab loại CĐHA khác chỉ hiện khi có y lệnh |
| 2 | Như 1 nhưng chỉ hiện tab nhóm có y lệnh |

- **Bảng nhóm** `Base/ClsTabGroupStore.cs` (bản sao y hệt ở plugin ContentSubclinical — sửa phải sửa cả 2): Xét nghiệm (XN), Siêu âm (SA), X-quang (CĐHA có `DIIM_TYPE_ID` = `HIS_DIIM_TYPE.ID__XQ`), CT / MRI (`ID__CT`/`ID__MRI`), tab tự sinh `DIIM<id>` cho loại CĐHA khác (tên = `DIIM_TYPE_NAME`), TDCN, Nội soi, Giải phẫu bệnh (GPBL), CLS khác (CĐHA chưa gán loại). Thủ thuật vẫn ở tab Khác. Loại CĐHA tra `V_HIS_SERVICE` theo `DHisSereServ2.SERVICE_ID` (MOS bản 53745), không có thì theo mã dịch vụ.
- **Dữ liệu tab** (`Base/ClsTabDataBuilder.cs`): phân nhóm ở dòng dịch vụ; dòng loại DV và dòng phiếu được đưa vào mọi tab có dịch vụ của nó (giữ đúng bố cục tab CLS cũ, dòng dùng chung không nhân bản). Tên tab = tên + `(số dịch vụ)` trong ngày đang chọn ở cột Ngày.
- **Giữ tab đang chọn** khi đổi bệnh nhân / ngày / nạp lại (bỏ vòng `TabPages[3..0]`; cây ở tab chưa mở được tạo handle sẵn bằng `UCTreeListService.EnsureTreeHandle()` nên nạp lại vẫn đúng). Tab nhớ bị ẩn → về Tất cả; BN sau có lại nhóm đó → quay lại tab đã nhớ. Bấm vào tiêu đề tab đang hiện (vd Tất cả sau khi bị đưa về) cũng tính là chọn (`xtraTabControl1_MouseDown` + `CalcHitInfo`). Sau mỗi lần nạp, tab đang chọn được cuộn tới (`MakePageVisible`) vì tab nhóm hiện thêm bên trái có thể đẩy nó ra sau nút cuộn. Ẩn/hiện tab Khám bệnh (nạp bất đồng bộ) đi qua `SetExamTabVisible` để không bị tính là người dùng chọn.
- **Tab Khám bệnh** (sửa kèm, mọi chế độ): `LoadMainExamByTreatmentId` xóa nội dung khám + sinh hiệu của BN trước ngay khi bắt đầu tải (trước đây tab vẫn hiện thông tin BN trước trong lúc chờ, và giữ nguyên sinh hiệu cũ khi BN mới không có DHST); kết quả trả về muộn của BN trước bị bỏ (`examLoadingTreatmentId`).
- Tạo tab nhóm lỗi (thiếu tab) → bỏ các tab đã tạo, giữ tab CLS cũ (key coi như tắt). Hỏi xác nhận sửa/xóa gặp lỗi → dừng thao tác (không bỏ qua câu hỏi).
- **Sửa/xóa phiếu từ tab nhóm**: phiếu còn dịch vụ thuộc nhóm khác (thực tế: phiếu CĐHA lẫn X-quang với CT/MRI) → hỏi "Phiếu … còn có dịch vụ thuộc nhóm … Thao tác sẽ áp dụng cho toàn bộ phiếu. Bạn có muốn tiếp tục?" (tên nhóm đầy đủ `ClsTabPage.FullName`, vd TDCN → "Thăm dò chức năng") trước khi gọi `Edit_Click` / `Delete_Click`.
- Tab nhóm trống vẽ dòng "Không có y lệnh trong khoảng ngày đã chọn" (`UCTreeListService.SetEmptyText`). Nút Thu gọn, cột dự trù (`SetAnticipateColumnsVisible`), cột HP (`SetExpendCheckClick`) áp dụng cho cả tab nhóm.
- Nút "Kết quả CLS" truyền thêm cờ `CLS_TAB` (+ `CLS_TAB_INIT:<mã nhóm>` khi đang đứng ở tab nhóm) để màn kết quả hiện theo tab và mở sẵn đúng nhóm.
- Code: `Run/UCBedRoomPartial__Pluss__ClsTab.cs` (tạo/nạp/ẩn hiện/chọn tab), `Run/UCBedRoomPartial__Pluss__ClsTabAction.cs` (xác nhận, cờ Kết quả CLS, Thu gọn, dọn dẹp), `UCTreeListService___ClsTab.cs`, `EnumClsTabOption.cs`.
- **Phụ thuộc DLL**: cần `MOS.SDO.dll` bản 14/10/2026 (`DHisSereServ2.SERVICE_ID`, `IS_EXPEND` — việc 53745); plugin đã gồm code 53745 nên `MOS.SDO.dll` cũ hơn làm cây y lệnh trống (MissingMethodException khi nạp).

### Điều kiện enable nút "Xóa y lệnh" trong TreeList (`ssRootSety.IsEnableDelete`)
Với `SERVICE_REQ_STT_ID == CXL` (chưa xử lý), enable khi thỏa **1 trong 5**:
1. Tài khoản đăng nhập là **người chỉ định** (`REQUEST_LOGINNAME == loginName`)
2. Tài khoản đăng nhập là **admin** (`CheckLoginAdmin.IsAdmin`)
3. Khoa chỉ định **trùng khoa làm việc** AND loại y lệnh là **Khám (KH)**
4. Loại y lệnh là **Giường (G)** AND tài khoản có quyền **HIS000053** (bổ sung theo việc 44693)
5. Key `MOS.HIS_SERVICE_REQ.ALLOW_DELETE_BY_SAME_REQUEST_DEPARTMENT = 1` AND khoa chỉ định **trùng khoa làm việc** — mọi loại y lệnh (bổ sung theo việc 55703, mặc định tắt)

### Xóa theo quy tắc cùng khoa — kiểm tra văn bản ký EMR (việc 55703)
Khi bấm Xóa (`Delete_Click`), y lệnh **chỉ** xóa được nhờ điều kiện 5 (tài khoản không phải người tạo y lệnh `SERVICE_REQ_CREATOR` / người chỉ định / admin / giường + HIS000053 / khám cùng khoa) phải qua `CheckEmrDocumentBeforeSameDepartmentDelete` (`Run/UCBedRoomPartial__Pluss__DeleteSameDepartment.cs`) trước luồng xóa cũ:
- `MOS.HAS_CONNECTION_EMR = 1` thì gọi `api/EmrDocument/Get` theo mã hồ sơ, lấy văn bản chưa xóa có `HIS_CODE` chứa `SERVICE_REQ_CODE:<mã y lệnh>` (mọi loại văn bản, không chỉ phiếu chỉ định).
- Văn bản đã có người ký (`SIGNERS`) → chặn "đã được ký số"; văn bản chưa ký do tài khoản khác tạo → chặn (EMR chỉ cho người tạo xóa văn bản); không lấy được văn bản → chặn "không kiểm tra được".
- Y lệnh có quyền xóa sẵn có giữ nguyên luồng cũ. Logic dùng chung ở `Base/SameDepartmentDeleteChecker.cs` (bản sao cùng nội dung với plugin ServiceReqList).

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
| V_HIS_SERVICE | View (cache) | Loại chẩn đoán hình ảnh (`DIIM_TYPE_ID`, `DIIM_TYPE_NAME`) để tách tab X-quang / CT / MRI (việc 57971) |

## 4. UI Layout

UserControl chính: `UCBedRoomPartial` (file ~3000+ dòng, có nhiều partial).
TreeList y lệnh: `UCTreeListService` — render theo `SereServADO` (root + child theo CONCRETE_ID__IN_SETY), 2 cột thao tác `rep_btnEdit_Enable/Disable` và `rep_btnDelete_Enable/Disable` (toggle theo `IsEnableEdit / IsEnableDelete`).

### UC sử dụng
| UC | Mục đích |
|----|----------|
| UCTreeListService | Cây y lệnh chi tiết |
| UCPatientSelect | Chọn bệnh nhân |

### Lưới Danh sách BN (`gridViewTreatmentBedRoom`)
Nguồn `TreatmentBedRoomADO` (kế thừa view `L_HIS_TREATMENT_BED_ROOM`, API `GetLView`). Cột theo thứ tự mặc định: STT, biểu tượng, cấp cứu, Tên BN, Mã BN, Mã điều trị, Tuổi, Giới tính, Số thẻ, Hạn thẻ, Thời gian vào, **Mã giường** (`BED_CODE`, việc 59466), Giường (`BED_NAME`), Số vào viện, Chẩn đoán chính/phụ, Phòng điều trị, Số ngày điều trị, Khoa, Phân loại BN, Phân loại chăm sóc, Ngày nhập viện, SĐT.

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
| Kiểm tra văn bản ký trước khi xóa cùng khoa (việc 55703) | api/EmrDocument/Get (`UriApi.EMR_DOCUMENT_GET`) | EmrConsumer |
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
| HIS.Desktop.Plugins.ContentSubclinical | Nút "Kết quả CLS" (luôn hiện, không gate key config) | `long TREATMENT_ID` + `List<string> { "VIEW_ONLY" }` + `Module` — KHÔNG truyền `DelegateSelectData` (chế độ chỉ xem, không chèn kết quả). Key `ClsTabOption` bật: list thêm `"CLS_TAB"` và `"CLS_TAB_INIT:<mã nhóm>"` khi đang ở tab nhóm (việc 57971) |
| HIS.Desktop.Plugins.Debate, HisTrackingList, ServiceReqList, AggrHospitalFees, TreatmentHistory, BedHistory, ... | Các nút thanh chức năng còn lại | `TREATMENT_ID` + `Module` (pattern chung `__Pluss__EventBtn.cs`) |

## 7. Print

Plugin tích hợp nhiều mẫu in qua RichEditorStore + MpsPrinter, phụ thuộc loại y lệnh (đơn thuốc, phiếu yêu cầu DV, phiếu xét nghiệm, ...).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 24/10/2026 | dangth2 | Việc 59466 (BV HAGL, tài liệu 3509): thêm cột "Mã giường" (`grdColBedCode`, FieldName `BED_CODE` của view `L_HIS_TREATMENT_BED_ROOM`, cột bound, không sửa được) vào lưới Danh sách BN, đặt ngay trước cột "Giường"; dời VisibleIndex các cột sau +1. Lưới đặt `OptionsLayout.LayoutVersion = "1"` + handler `gridViewTreatmentBedRoom_LayoutUpgrade`: viện bật `HIS.Desktop.ApplyRestoreLayout.ModuleLinks` và máy đã lưu bố cục lưới từ trước (file không có version) thì khi khôi phục, cột Mã giường được đưa về ngay trái cột Giường (nếu không xử lý, DevExpress xếp cột mới sau Giường); bố cục lưu sau thay đổi giữ nguyên lựa chọn người dùng. Caption theo khóa `UCBedRoomPartial.grdColBedCode.Caption` (vi "Mã giường", en "Bed code", my). Sửa `Run/UCBedRoomPartial.Designer.cs`, `Run/UCBedRoomPartial.cs` (SetCaptionByLanguageKey), `Run/UCBedRoomPartial__Pluss__Dispose.cs`, `Resources/Lang.vi/en/my.resx`. Không sửa BE, không thêm key. |
| 14/10/2026 | dangth2 | Việc 57971 (BV Nguyễn Tri Phương): tách tab "CLS" của cây y lệnh thành tab theo nhóm dịch vụ (Xét nghiệm, Siêu âm, X-quang, CT / MRI, TDCN, Nội soi, Giải phẫu bệnh, CLS khác + tab tự sinh theo loại CĐHA khác), bật bằng key `HIS.Desktop.Plugins.BedRoomPartial.ClsTabOption` = 1 (luôn hiện tab chính) / 2 (chỉ tab có y lệnh), mặc định tắt. Tên tab kèm số dịch vụ, giữ tab đang chọn khi đổi bệnh nhân/ngày (thay vòng chọn lại `TabPages[3..0]` bằng `EnsureTreeHandle`), hỏi xác nhận khi sửa/xóa phiếu có dịch vụ nhóm khác, dòng trống cho tab không có y lệnh, nút "Kết quả CLS" gửi cờ `CLS_TAB`/`CLS_TAB_INIT:`. File mới `Base/ClsTabGroupStore.cs`, `Base/ClsTabDataBuilder.cs`, `Base/ClsTabPage.cs`, `EnumClsTabOption.cs`, `Run/UCBedRoomPartial__Pluss__ClsTab.cs`, `Run/UCBedRoomPartial__Pluss__ClsTabAction.cs`, `UCTreeListService___ClsTab.cs`; sửa `Run/UCBedRoomPartial.cs` (Load, `LoadDataSereServByTreatmentId`, `LoadMainExamByTreatmentId` → `SetExamTabVisible`, Thu gọn), `__Anticipate.cs`, `__Dispose.cs`, `__EventBtn.cs`, `Key/*`, resource vi/en/my (tên tab, dòng trống, câu xác nhận `PhieuConDichVuThuocNhomKhac`). Key tắt: hành vi như cũ, trừ sửa kèm tab Khám bệnh: xóa nội dung BN trước khi tải BN mới, sinh hiệu trống khi BN không có DHST, bỏ kết quả trả về muộn của BN trước. Cần `MOS.SDO.dll` bản 53745. |
| 06/10/2026 | dangth2 | Việc 55703 (BV Nguyễn Tri Phương): cho phép người cùng khoa chỉ định xóa y lệnh do người khác chỉ định, bật bằng key `MOS.HIS_SERVICE_REQ.ALLOW_DELETE_BY_SAME_REQUEST_DEPARTMENT = 1` (mặc định tắt). Thêm điều kiện 5 vào 2 chỗ đặt `ssRootSety.IsEnableDelete` (`LoadDataSereServByTreatmentId`, `GroupDataByTracking`), đọc key 1 lần trước vòng lặp; nút y lệnh mang thêm `SERVICE_REQ_CREATOR` (ADO). `Delete_Click` gọi `CheckEmrDocumentBeforeSameDepartmentDelete` trước luồng cũ (file mới `Run/UCBedRoomPartial__Pluss__DeleteSameDepartment.cs`, `Base/SameDepartmentDeleteChecker.cs`). Key mới trong `Key/HisConfigKeys.cs` + `Key/HisConfigCFG.cs` (kèm `MOS.HAS_CONNECTION_EMR`), `UriApi.EMR_DOCUMENT_GET`, 4 thông báo `XoaCungKhoa*` (vi/en/my). Cần Backend MOS bản có cùng key. |
| 21/09/2026 | sinhnt | Việc 3352 (PT-56263): nhãn `Dự trù: ...` ở cột "Khoa yêu cầu" (2 chỗ trong `Run/UCBedRoomPartial.cs`: `LoadDataSereServByTreatmentId` và `GroupDataByTracking`) và cột "Ngày dự trù" (`USE_DATE_STR`) hiển thị thêm **giờ phút** khi `USE_TIME` có giờ — helper `FormatUseTimeDisplay` đặt cạnh `ToDateNumber` trong `Run/UCBedRoomPartial__Pluss__Anticipate.cs`. Nới `tc_UseDate.Width` 90 → 115 cho đủ chuỗi `dd/MM/yyyy HH:mm`. **Không đụng** logic gom nhóm theo ngày (`ToDateNumber`, `IsAnticipateMedicinePres`, `GetEffectiveDate`) nên cây ngày bên trái không đổi. |
| 22/05/2026 | dangth2 | Việc 44693 (Tài liệu 2671): Bổ sung điều kiện enable nút "Xóa y lệnh giường" trong `Run/UCBedRoomPartial.cs` (2 vị trí thiết lập `ssRootSety.IsEnableDelete`) — nếu loại y lệnh là Giường (`SERVICE_REQ_TYPE.ID__G`) VÀ tài khoản có quyền HIS000053 thì enable. Các trường hợp khác giữ nguyên. Thêm `Base/ControlCode.cs`, field `hasDeleteBedPermission`, method `LoadDeleteBedPermission()`. Reference `ACS.EFMODEL.dll`. |
| 06/07/2026 | phuongnm | Tài liệu 1223: Sửa hiển thị nhóm máu ở vùng thông tin hành chính (`Run/UCBedRoomPartial.cs`, `lblBloodType`). Trước đây điều kiện `abo && rh` khiến chỉ có 1 trong 2 giá trị thì không hiển thị. Sửa thành 4 trường hợp: có cả ABO+RH (`O; RH(-)`), chỉ ABO (`A`), chỉ RH (`RH(-)`), không có (trống). |
| 17/08/2026 | phuongnm | Hiển thị đơn thuốc dự trù theo ngày dự trù trên màn Buồng bệnh. Thêm config `HIS.Desktop.Plugins.BedRoomPartial.ShowAnticipatePresByUseDate` (mặc định TẮT). Khi bật: đơn thuốc dự trù xếp theo `USE_TIME` thay vì `INTRUCTION_TIME`, chuyển hẳn không lặp ở ngày kê; thêm 2 cột "Ngày kê"/"Ngày dự trù"; tô xanh da trời đơn dự trù; tab "Tất cả" gom nhóm riêng `Dự trù — kê ngày X`. Chỉ áp dụng đơn thuốc (`SERVICE_REQ_TYPE_ID` ∈ DONK/DONTT/DONDT), loại trừ đơn máu DONM và dịch vụ/giường. File mới `Run/UCBedRoomPartial__Pluss__Anticipate.cs`. Kèm dọn nợ hiệu năng: 2 khối `dataServiceReq.Where(...)` gọi 4 lần trong vòng lặp → `Dictionary.TryGetValue`. Tài liệu: `A_NghiepVu_/B_KyThuat_DonDuTruTheoNgayDuTru_BuongBenh.docx`. |
| 13/08/2026 | nampp | Việc 3170 (BV Điện Biên): Thêm nút "Kết quả CLS" trên thanh chức năng — mở màn xem kết quả cận lâm sàng (plugin `ContentSubclinical` chế độ CHỈ XEM) không cần vào tờ điều trị. Nút tạo RUNTIME trong `InitSubclinicalResultButton()` (`Run/UCBedRoomPartial.cs`, đặt sau nút "Danh sách y lệnh" qua `LayoutControlItem.Move`) — không sửa Designer vì dãy nút dùng toạ độ pixel cố định. **Luôn hiện, không gate key config** (bản đầu có key `ShowSubclinicalResultButton`, đã bỏ cùng ngày theo yêu cầu). Enable/disable theo bệnh nhân trong `SetEnableButton`. Handler `btnKetQuaCLS_Click` (`__Pluss__EventBtn.cs`), gỡ event trong `__Pluss__Dispose.cs`. Resource 3 ngôn ngữ `UCBedRoomPartial.btnKetQuaCLS.Text/ToolTip`. |
| 21/08/2026 | nampp | Việc 3222 (v3): **Bỏ tự điền tài khoản bác sĩ đăng nhập** khi vào chế độ "chưa kê đơn" (gỡ `SetDoctorFilterByLoginUser` + field liên quan) — ô bác sĩ trống = lọc TẤT CẢ bác sĩ, ai muốn xem của mình thì tự chọn. Đổi NullText thành "Bác sĩ điều trị (tất cả)" (3 ngôn ngữ). |
| 21/08/2026 | nampp | Việc 3222 (v2): Tài liệu viện cập nhật — chế độ "BN chưa kê đơn trong khoảng" hiển thị cả bệnh nhân đã kết thúc điều trị/đã ra viện (truy vấn nền không giới hạn `HAS_OUT_TIME`, trạng thái mặc định "Tất cả"). Sửa `SetTreatmentBedRoomFilter` (`Run/UCBedRoomPartial.cs`): khi chế độ = 2 và trạng thái ≠ "Đã kết thúc điều trị" thì ép `HAS_OUT_TIME = false`; khi trạng thái = "Tất cả" ép thêm `IS_PAUSE = false`. Người dùng chọn trạng thái "Đã kết thúc điều trị" thì tôn trọng (không ép). KHÔNG tick hộ checkbox "Ẩn kết thúc tạm" vì trạng thái checkbox lưu ControlState theo người dùng. |
| 19/08/2026 | nampp | Việc 3222 (BV Điện Biên): Lọc bệnh nhân chưa kê đơn trong ngày theo tài khoản bác sĩ điều trị. Cơ chế lọc đã có sẵn (`cboPatientFilter` = 2 → `NO_PRESCRIPTION_FROM/TO`; `cboEmployee` → `DOCTOR_LOGINNAME`) nhưng ô lọc bác sĩ không có nhãn nên người dùng không nhận ra. Sửa trong `Run/UCBedRoomPartial.cs`: `InitComboEmployee` đặt `NullText` = "Bác sĩ điều trị" + tooltip cho `layoutControlItem50`, lưu `lstDoctorFilter`; thêm `SetDoctorFilterByLoginUser()` tự điền tài khoản bác sĩ đang đăng nhập khi chọn chế độ "BN chưa kê đơn trong khoảng" (chỉ khi ô trống, rời chế độ chỉ hoàn tác giá trị hệ thống tự điền); gọi từ `cboPatientFilter_EditValueChanged`. Resource 3 ngôn ngữ `BacSiDieuTri`, `LocTheoBacSiDieuTri`. **Không sửa Designer.** Kèm sửa Backend `MOS.MANAGER/HisTreatmentBedRoom/HisTreatmentBedRoomGet.cs` — `GetLView` trước đây lọc "chưa kê đơn" SAU khi DAO đã phân trang nên mỗi trang khuyết bệnh nhân và tổng số bản ghi sai; nay lấy đủ dữ liệu, lọc, gán lại `param.Count` rồi mới `Skip/Take`. |

## 9. Test Cases

### Cột Mã giường — việc 59466
- [ ] Mở Buồng bệnh: lưới Danh sách BN có cột "Mã giường" ngay trái cột "Giường"; BN đang nằm giường hiện mã giường (vd H001), cùng giường với cột "Giường"
- [ ] BN chưa xếp giường: ô Mã giường trống, các cột khác như cũ
- [ ] Chuyển giường cho BN rồi tải lại danh sách: Mã giường đổi theo giường mới
- [ ] Ngôn ngữ tiếng Anh: tiêu đề "Bed code"
- [ ] Ẩn/hiện cột qua Column Chooser (chuột phải tiêu đề cột) vẫn dùng được với cột mới
- [ ] Viện bật khôi phục bố cục (`HIS.Desktop.ApplyRestoreLayout.ModuleLinks` có BedRoomPartial), máy đã lưu bố cục lưới bằng bản cũ: mở bản mới → Mã giường vẫn ngay trái Giường; tự kéo Mã giường sang chỗ khác → mở lại giữ đúng chỗ đã kéo

### Tách tab CLS theo nhóm dịch vụ — việc 57971
- [ ] Key trống: 5 tab như cũ (Tất cả, CLS, Thuốc vật tư máu, Khác, Khám bệnh), nạp BN/ngày luôn về Tất cả
- [ ] Key = 1: không còn tab CLS; Tất cả | Xét nghiệm (n) | Siêu âm | X-quang (n) | TDCN (n) | Nội soi | Thuốc… | Khác | Khám bệnh; tab chính hiện cả khi rỗng (dòng "Không có y lệnh trong khoảng ngày đã chọn")
- [ ] BN có CT/MRI, GPBL, CĐHA chưa gán loại → hiện thêm CT / MRI, Giải phẫu bệnh, CLS khác; BN sau không có → ẩn
- [ ] Phiếu CĐHA X-quang + CT: tab X-quang chỉ dòng X-quang, tab CT / MRI chỉ dòng CT; Xóa/Sửa phiếu từ 1 tab → hỏi xác nhận nêu nhóm còn lại; Không → không làm gì
- [ ] Đang ở tab Xét nghiệm, đổi BN / bấm ngày khác / tích HP → vẫn tab Xét nghiệm, số trên tab đổi theo dữ liệu
- [ ] Đang ở tab Siêu âm bấm "Kết quả CLS" → màn kết quả mở sẵn tab Siêu âm
- [ ] Key = 2: chỉ hiện tab nhóm có y lệnh
- [ ] MOS cũ (chưa trả SERVICE_ID) vẫn tách X-quang / CT theo mã dịch vụ
- [ ] Đang ở Siêu âm, BN sau không có siêu âm → Tất cả; bấm vào Tất cả rồi chọn BN có siêu âm → vẫn ở Tất cả (không bấm thì quay lại Siêu âm)
- [ ] Màn 1366, BN nhiều nhóm, đang ở "Thuốc, vật tư, máu" / "Khám bệnh" → tab đó vẫn chọn và nhìn thấy trên thanh tab
- [ ] Đang ở tab Khám bệnh chọn BN khác → trong lúc tải các ô để trống (không hiện BN trước); BN không có DHST → ô sinh hiệu trống (cả key tắt)

### Lọc BN chưa kê đơn theo bác sĩ điều trị — việc 3222
- [ ] Chế độ "chưa kê đơn": BN đã ra viện / đã kết thúc điều trị KHÔNG xuất hiện; BN đã duyệt đủ ĐK ra viện nhưng chưa kết thúc VẪN xuất hiện (v2)
- [ ] Trong chế độ "chưa kê đơn" chọn trạng thái "Đã kết thúc điều trị" → vẫn liệt kê được hồ sơ đã kết thúc (ngoại lệ được tôn trọng) (v2)
- [ ] Checkbox "Ẩn kết thúc tạm" giữ nguyên trạng thái người dùng đã đặt, không bị tick/bỏ tick hộ (v2)
- [ ] Ô lọc hàng 4 cột phải hiện chữ mờ "Bác sĩ điều trị" + tooltip khi rê chuột
- [ ] Tài khoản bác sĩ chọn "BN chưa kê đơn trong khoảng" → ô bác sĩ tự điền tài khoản đang đăng nhập, ngày từ/đến = hôm nay
- [ ] Tổng số bản ghi trên thanh phân trang = số BN chưa kê đơn sau lọc; lật hết trang, tổng dòng khớp (cần DLL Backend mới)
- [ ] Kê đơn điều trị cho 1 BN rồi bấm Tìm → BN đó biến mất khỏi danh sách
- [ ] Tài khoản điều dưỡng → ô bác sĩ để trống, chọn tay từng bác sĩ vẫn lọc đúng
- [ ] Chọn bác sĩ bằng tay rồi đổi chế độ lọc khác → lựa chọn được giữ nguyên
- [ ] Bấm nút x trên ô bác sĩ → bỏ điều kiện lọc, danh sách trở lại toàn buồng
- [ ] Hồi quy: các chế độ lọc cũ (BN đang ở trong buồng / BN vào trong khoảng) đúng số liệu và tốc độ như trước

### Nút "Kết quả CLS" — việc 3170
- [ ] Nút "Kết quả CLS" hiện ngay sau "Danh sách y lệnh"; chưa chọn BN thì disable, chọn BN thì enable
- [ ] Nhấn nút → mở màn kết quả CLS đúng BN đang chọn, KHÔNG có ô tích/nút "Chọn (Ctrl S)"/6 tuỳ chọn chèn, có nút "Đóng"
- [ ] BN chưa có kết quả CLS → màn mở với cây rỗng, không popup lỗi
- [ ] Hồi quy: màn "Chọn kết quả CLS" mở từ tờ điều trị vẫn đủ ô tích + nút Chọn + chèn kết quả như cũ

### Xóa y lệnh cùng khoa — việc 55703
- [ ] Key tắt: nút Xóa trên cây y lệnh giữ nguyên như cũ
- [ ] Key = 1: y lệnh người khác chỉ định, cùng khoa làm việc, chưa xử lý → nút Xóa bật (cả 4 tab)
- [ ] Key = 1: y lệnh khoa khác / đang xử lý / hoàn thành → nút Xóa tắt
- [ ] Y lệnh cùng khoa chưa có văn bản EMR → xóa thành công (Backend bản mới)
- [ ] Y lệnh đã ký số → chặn, thông báo mã y lệnh + tên văn bản + người ký, không gọi API xóa
- [ ] Văn bản chưa ký do người khác tạo → chặn; do chính mình tạo → xóa được cả văn bản
- [ ] EMR lỗi / mất kết nối → chặn "Không kiểm tra được văn bản ký"
- [ ] Hồi quy: người chỉ định / người tạo y lệnh / admin xóa như cũ

### Xóa y lệnh giường — phân quyền HIS000053
- [ ] User KHÔNG có quyền HIS000053, KHÔNG là người chỉ định/admin → nút Xóa **disable** trên y lệnh giường
- [ ] User CÓ quyền HIS000053 → nút Xóa **enable** trên y lệnh giường (kể cả y lệnh do người khác tạo)
- [ ] User CÓ quyền HIS000053 nhưng y lệnh KHÔNG phải loại Giường → giữ nguyên logic cũ (không enable thêm)
- [ ] Y lệnh ở trạng thái khác CXL → nút Xóa luôn disable (không bị ảnh hưởng bởi quyền mới)
- [ ] Tài khoản admin / người chỉ định → vẫn enable nút Xóa như trước
