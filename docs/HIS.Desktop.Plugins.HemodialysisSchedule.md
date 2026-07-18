# HIS.Desktop.Plugins.HemodialysisSchedule — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HemodialysisSchedule |
| Loại | Form (`frmHemodialysisSchedule` kế thừa `FormBase`) |
| Mục đích | Xếp lịch chạy thận: đưa BN vào slot **Phòng + Ngày + Ca**, **KHÔNG sinh y lệnh**. |
| Ngày tạo | 01/07/2026 |
| Trạng thái | Mới phát triển (yêu cầu 4.2.1) |

**Lý do KHÔNG đưa ĐTTT vào Xếp lịch**: ĐTTT phụ thuộc bệnh nhân cụ thể (load qua `HIS_TREATMENT.PATIENT_TYPE_CODE` → `V_HIS_PATIENT_TYPE_ALLOW` → lọc theo chính sách giá ở `V_HIS_SERVICE_PATY`). Lúc xếp slot chưa biết dịch vụ cụ thể → chưa lọc chính xác. ĐTTT là quyết định **thanh toán** của BS trực, chốt khi tạo y lệnh ở màn Chỉ định.

## 2. Quy Trình Nghiệp Vụ

### Vùng trên — Danh sách lịch theo Phòng + Ngày + Ca
- Filter: Phòng chạy (mặc định phòng hiện tại), Ngày (bắt buộc), Ca 1–5 (mặc định Ca 1), từ khóa BN.
- Cột: STT, Ngày, Ca, Tên BN, Mã BN, Mã ĐT, Ngày sinh, Giới tính, Ngày vào, **Đối tượng (read-only từ HIS_TREATMENT)**, Gói vật tư, Ghi chú, 4 cột audit. **KHÔNG có cột Máy.**

### Vùng dưới — Danh sách BN đang điều trị
- Multi-select checkbox + Check-All. Filter: Khoa (mặc định khoa hiện tại), Toàn khoa, Ngày vào từ–đến, từ khóa BN.
- **Server-side paging** qua `Inventec.UC.Paging.UcPaging` (`ucPaging`): dùng `GetRO` để lấy tổng số bản ghi + phân trang; pageSize theo `ConfigApplications.NumPageSize`. Tìm/đổi khoa → `LoadTreatmentGrid()` nạp lại từ trang 0. Lưu ý: multi-select chỉ trong phạm vi TRANG hiện tại.
- **Cột "xóa nhanh"** (nút Delete, cạnh checkbox): CHỈ hiện với BN đã có slot trong lịch hiện tại (Phòng+Ngày+Ca đang chọn, đối chiếu `scheduleADOs`). Bấm → xác nhận → `Delete` slot đó ngay từ lưới dưới, không cần lên lưới trên. BN chưa xếp → nút ẩn (repo rỗng). Hiển thị nút tự cập nhật khi đổi Phòng/Ngày/Ca (RefreshData sau LoadScheduleGrid).

### R5 — Đưa vào lịch
- Tick nhiều BN + bấm "Đưa vào lịch" → INSERT cho mỗi BN tick một bản ghi vào `HIS_HEMODIALYSIS_SCHEDULE` với (TREATMENT_ID, PATIENT_ID, ROOM_ID, SCHEDULE_DATE, KIDNEY_SHIFT) từ filter + (EXP_MEST_TEMPLATE_ID, NOTE) từ bottom form. **KHÔNG ghi `HIS_SERVICE_REQ`** (không sinh y lệnh).

### Inline edit Gói vật tư / Ghi chú
- Double-click cell trên grid trên → LookUp cho Gói, TextEdit cho Ghi chú → khi commit → API Update đúng bản ghi theo `ID` slot.
- Gói vật tư load từ `HIS_EXP_MEST_TEMPLATE` với Filter: `(CREATOR = userĐăngNhập OR IS_PUBLIC = 1) AND IS_KIDNEY = 1 AND IS_ACTIVE = 1`.

### Đổi Ca tại chỗ (inline) — FE-only, KHÔNG cần backend Update hỗ trợ KIDNEY_SHIFT
- Cột **Ca** ở grid trên là LookUp 1–5. Backend `Update` **cố ý chỉ nhận** `EXP_MEST_TEMPLATE_ID`/`NOTE` (khóa nghiệp vụ bất biến) → đổi Ca được xử lý phía client bằng **tạo slot mới ở ca đích + xóa slot cũ**.
- Trình tự an toàn: (1) xác nhận; (2) chặn trùng khi BN đã có ở (cùng phòng+ngày+ca đích) theo unique key; (3) `CreateList` slot mới TRƯỚC; (4) chỉ khi `AddedCount ≥ 1` mới `Delete` slot cũ (lỗi giữa chừng vẫn còn slot cũ, không mất dữ liệu).
- Sau khi đổi, grid trên đang lọc theo Ca cũ nên BN sẽ biến mất khỏi view hiện tại (đã sang ca khác) — đúng nghiệp vụ.

### R6 — Sao chép
- Đọc tất cả bản ghi của (Phòng nguồn, Ngày nguồn) → INSERT vào Ngày đích → **skip BN trùng** theo unique key (TREATMENT_ID, SCHEDULE_DATE, KIDNEY_SHIFT). Trả về count = (thêm mới, skip trùng) để hiển thị popup tóm tắt `frmCopyScheduleConfirm`.

## 3. EFMODEL / Bảng Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| `HIS_HEMODIALYSIS_SCHEDULE` | Table | Slot lịch chạy thận (TREATMENT_ID, PATIENT_ID, ROOM_ID, SCHEDULE_DATE, KIDNEY_SHIFT, EXP_MEST_TEMPLATE_ID, NOTE + audit) |
| `V_HIS_TREATMENT_4` | View | DS BN đang điều trị (vùng dưới) |
| `HIS_EXP_MEST_TEMPLATE` | Table | Gói vật tư cho LookUp inline |
| `V_HIS_EXECUTE_ROOM` | View | Combo Phòng chạy |
| `HIS_DEPARTMENT` | Table | Combo Khoa |

## 4. API Consumer calls

| # | API | Consumer | Mục đích |
|---|-----|----------|----------|
| 1 | `HisHemodialysisSchedule/Get` | MosConsumer | Load slot lịch (Phòng+Ngày+Ca) |
| 2 | `HisHemodialysisSchedule/CreateList` | MosConsumer | R5 — thêm slot cho các BN tick |
| 3 | `HisHemodialysisSchedule/Update` | MosConsumer | Inline edit Gói vật tư / Ghi chú |
| 4 | `HisHemodialysisSchedule/Delete` | MosConsumer | Xóa slot (nút X trên dòng) |
| 5 | `HisHemodialysisSchedule/CopySchedule` | MosConsumer | R6 — sao chép lịch ngày → ngày |
| 6 | `HisTreatment/GetView4` (V_HIS_TREATMENT_4) | MosConsumer | DS BN đang điều trị (vùng dưới) — gọi bằng **GetRO** (server-side paging) |
| 7 | `HisExpMestTemplate/Get` | MosConsumer | Gói vật tư (LookUp) |

> **Lưu ý**: Tên URI ở cột API là quy ước trong `HisRequestUriStore.cs` của plugin. Backend cần hiện thực các endpoint tương ứng (service `HisHemodialysisSchedule` + entity `HIS_HEMODIALYSIS_SCHEDULE`). Xác nhận lại tên action `GetView4` cho V_HIS_TREATMENT_4 với backend.

## 5. Cấu trúc file plugin

```
HIS.Desktop.Plugins.HemodialysisSchedule/
├─ HemodialysisScheduleProcessor.cs        (ExtensionOf DesktopRoot)
├─ HisRequestUriStore.cs
├─ HemodialysisSchedule/
│  ├─ IHemodialysisSchedule.cs / Factory / Behavior
│  ├─ frmHemodialysisSchedule.cs           (logic: filter, load, R5, R6, inline edit, delete)
│  └─ frmHemodialysisSchedule.Designer.cs  (2 grid trong SplitContainer + 2 panel filter)
├─ CopySchedule/frmCopyScheduleConfirm.cs  (popup tóm tắt sao chép R6)
├─ ADO/  HemodialysisScheduleADO, TreatmentInfoADO, ExpMestTemplateADO, ShiftADO, CopyScheduleResultADO
├─ Filter/ HemodialysisScheduleFilter, TreatmentInfoFilter, ExpMestTemplateFilter
├─ SDO/  CopyScheduleSDO
├─ Resources/ ResourceMessageLang + Message.Lang.{vi,en,my}.resx
└─ Properties/AssemblyInfo.cs
```

## 6. Việc cần làm khi tích hợp

- [ ] Backend: tạo bảng `HIS_HEMODIALYSIS_SCHEDULE` + entity EFMODEL + service `HisHemodialysisSchedule` (Get/CreateList/Update/Delete/CopySchedule); unique key (TREATMENT_ID, SCHEDULE_DATE, KIDNEY_SHIFT).
- [ ] Thêm project vào `HIS\HIS.Desktop.sln` và cấu hình copy output sang thư mục Plugins của app.
- [ ] Đăng ký module trong `HIS_MODULE` (menu) trỏ tới `HIS.Desktop.Plugins.HemodialysisSchedule`.
- [ ] Xác nhận tên field của `V_HIS_TREATMENT_4` (TDL_PATIENT_*, TDL_TREATMENT_TYPE_NAME, TDL_PATIENT_HEIN_CARD_NUMBER, ICD_NAME) khớp với `TreatmentInfoADO`.

## 7. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 01/07/2026 | — | Tạo mới plugin (yêu cầu 4.2.1). |
| 07/07/2026 | huannh | Cho phép **đổi Ca tại chỗ** ở grid trên (cột Ca thành LookUp 1–5). Do backend `Update` không nhận `KIDNEY_SHIFT`, xử lý FE-only = tạo slot mới ở ca đích + xóa slot cũ; chặn trùng theo unique key; tạo trước–xóa sau để không mất dữ liệu. Bổ sung message `XacNhanChuyenCaFormat`, `BenhNhanDaCoTrongCaNay`, `ChuyenCaThanhCong`. Backend contract **không đổi**. |
| 07/07/2026 | huannh | Thêm **cột "xóa nhanh"** ở lưới dưới: chỉ hiện nút với BN đã có slot ở Phòng+Ngày+Ca đang chọn (đối chiếu `scheduleADOs`), bấm → `Delete` slot đó ngay. Dùng `CustomRowCellEdit` swap `repoDeleteB`/`repoEmptyB`; `RefreshData` lưới dưới sau `LoadScheduleGrid`. Message `XacNhanRutBenhNhanKhoiLichFormat`, `RutBenhNhanKhoiLichThanhCong`. |
| 07/07/2026 | huannh | **Chẩn đoán filter lưới trên**: filter chuẩn `MOS.Filter.HisHemodialysisScheduleViewFilter`/`HisHemodialysisScheduleFilter` trong lib đang **rỗng** (chỉ FilterBase, thiếu ROOM_ID/SCHEDULE_DATE/KIDNEY_SHIFT) → backend bỏ qua field lọc → tìm ra rỗng. **Chờ backend bổ sung field** rồi mới refactor plugin dùng view `V_HIS_HEMODIALYSIS_SCHEDULE` + filter chuẩn (bỏ enrich `HisTreatment/Get`). Đã thêm Debug trace filter trong `LoadScheduleGrid`. |
| 07/07/2026 | huannh | **Fix tìm kiếm (KEY_WORD)**: cho `HemodialysisScheduleFilter` và `TreatmentInfoFilter` kế thừa `MOS.Filter.FilterBase` (đúng chuẩn MOS: KEY_WORD/CN_WORD/ORDER_* ở base) thay vì POCO trần; set cả `CN_WORD` + `KEY_WORD` khi tìm. Lọc theo ROOM_ID/SCHEDULE_DATE/KIDNEY_SHIFT ở lưới trên **vẫn chờ backend** thêm field vào server filter (xem dòng trên). |
| 08/07/2026 | huannh | Thêm **server-side paging** cho lưới dưới (`Inventec.UC.Paging.UcPaging`): `LoadTreatmentGrid` chuyển từ `Get` (giới hạn ~99 dòng) sang init paging + `LoadTreatmentGridData` dùng `GetRO` (start/limit + tổng bản ghi). pageSize = `ConfigApplications.NumPageSize`. Thêm ref `Inventec.UC.Paging`, control `ucPaging` + `lciPaging` trong Designer (thu nhỏ `lciGridTreatment` chừa chỗ). |
| 08/07/2026 | huannh | Fix **cột Số thẻ BHYT** lưới dưới không hiển thị: field bind sai `TDL_PATIENT_HEIN_CARD_NUMBER` → sửa thành `TDL_HEIN_CARD_NUMBER` (đúng cột `V_HIS_TREATMENT_4`) ở cả `TreatmentInfoADO` và `colHeinCardB.FieldName`. Các field khác lưới dưới đã đối chiếu view: khớp. |
| 08/07/2026 | huannh | Đưa **nút X (xóa slot)** lên cột đầu lưới trên (VisibleIndex 0, đẩy STT/Ngày/Ca lùi 1). Bỏ tích chọn lưới dưới **sau khi Đưa vào lịch** cho đồng nhất với sau khi tìm kiếm (`IsSelected=false` toàn bộ + reset header + RefreshData); việc phân biệt BN đã xếp do cột nút X đỏ đảm nhiệm. |
| 07/07/2026 | huannh | **XÁC NHẬN lỗi thuộc BE** (bằng log): FE gửi đúng `{ROOM_ID:2378, SCHEDULE_DATE:20260707, KIDNEY_SHIFT:1, CN_WORD:"test"}` nhưng `HisHemodialysisSchedule/Get` trả **92 dòng** (≈ toàn bảng) → BE bỏ qua toàn bộ filter nghiệp vụ + tìm kiếm. **Chốt: BE phải thêm `ROOM_ID`, `SCHEDULE_DATE`, `KIDNEY_SHIFT` vào `HisHemodialysisScheduleViewFilter` và wire `CN_WORD` vào WHERE của query Get.** FE giữ nguyên (đã đúng). Debug trace filter + result count để trong `LoadScheduleGrid`/`LoadTreatmentGrid` phục vụ đối chiếu, gỡ sau khi BE xong. |
