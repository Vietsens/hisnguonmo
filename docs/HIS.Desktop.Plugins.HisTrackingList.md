# HIS.Desktop.Plugins.HisTrackingList — Tài Liệu Module

> Tài liệu này tập trung vào chức năng **In tờ điều trị (Mps000062)** và việc bổ sung **đếm số ngày sử dụng thuốc có cộng số lần sử dụng trước đó**. Các nghiệp vụ khác của plugin (danh sách tờ điều trị, ký số EMR…) chỉ nêu ở mức liên quan.

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisTrackingList |
| Loại | Form (danh sách + in tờ điều trị) |
| Mục đích | Quản lý & in tờ điều trị (theo dõi y lệnh) của bệnh nhân nội trú. Feature này bổ sung việc cộng thêm "Số lần sử dụng thuốc trước đó" khi đếm số ngày dùng thuốc trên bản in `.repx` (Mps000062). |
| Người sửa | phuongnm |
| Ngày | 06/07/2026 |
| Trạng thái | Bảo trì |

**Plugin liên quan (cùng dùng chung mẫu in Mps000062):**
- `HIS.Desktop.Plugins.TrackingCreate` — tạo/sửa tờ điều trị, cũng in Mps000062 → đã cập nhật cùng logic.
- `MPS.Processor.Mps000062` + `MPS.Processor.Mps000062.PDO` — bộ xử lý sinh dữ liệu và render mẫu in.

## 2. Quy Trình Nghiệp Vụ

### Bối cảnh
Khi kê đơn (màn hình `AssignPrescriptionPK`), người dùng có thể nhập **"Số ngày sử dụng thuốc trước đó"** → lưu vào cột `HIS_EXP_MEST_MEDICINE.PREVIOUS_USING_COUNT`.

- **Bản in Excel** (`temp`): đã có key `<#Medicines.USING_COUNT_NUMBER;>` = `PreviousUseDay + count` (xem `Mps000062Processor.cs` dòng ~2705) → đã cộng đúng.
- **Bản in `.repx`**: hiển thị số đếm qua hàm `GetUsedDayCounting()` → **trước đây CHƯA cộng** `PREVIOUS_USING_COUNT`.

### Thay đổi
Bổ sung cấu hình bật/tắt việc cộng `PREVIOUS_USING_COUNT` vào số đếm ngày dùng thuốc trên bản `.repx`, giữ nguyên định dạng hiển thị theo cấu hình sẵn có.

### Luồng in tờ điều trị (rút gọn)
```
frmHisTrackingList / frmTrackingCreate  (nút In)
  → đọc HisConfigs (các option đếm ngày dùng thuốc)
  → build Mps000062SingleKey (gán các option vào _WorkPlaceSDO)
  → MpsPrinter.Run(PrintData("Mps000062", ...))
    → Mps000062Processor.ProcessData()
      → GetUsedDayCounting(medi)  ← điểm sửa (áp dụng cho .repx + các key MEDICINES*_DATA / MERGE)
```

### Điều kiện nghiệp vụ
- `UsedDayCountingAddPreviousUseDay = 1` → số đếm hiển thị = **số đếm hiện tại + PREVIOUS_USING_COUNT**.
- Khác `1` (kể cả để trống) → xử lý **y như cũ** (không đổi hành vi).
- Định dạng hiển thị vẫn tuân theo `UsedDayCountingFormatOption` (đọc số thành chữ cho nhóm Gây nghiện/Hướng thần).

## 3. EFMODEL Sử Dụng (liên quan feature)

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_EXP_MEST_MEDICINE | Table | Nguồn cột `PREVIOUS_USING_COUNT` (số lần/ngày sử dụng trước đó). **Chỉ đọc — không sửa backend.** |
| V_HIS_TRACKING | View | Danh sách tờ theo dõi y lệnh cần in |
| ExpMestMetyReqADO | ADO (kế thừa HIS_EXP_MEST_MEDICINE) | DTO dòng thuốc trong Mps000062; chứa các trường đếm `NUMBER_H_N`, `NUMBER_BY_TYPE`, `NUMBER_USE_AND_ACTIVE`, `NUMBER_ACTIVE_INGR`, `NUMBER_OF_USE_IN_DAY`, `USING_COUNT_NUMBER` |

## 4. Cấu Hình Hệ Thống (HIS_CONFIG)

| Config key | Mô tả | Giá trị | Mặc định | Module link |
|-----------|-------|---------|----------|-------------|
| `HIS.Desktop.Plugins.TrackingPrint.UsedDayCountingOption` | Kiểu đếm số ngày dùng thuốc (1–6) | 1..6 | — | HisTrackingList |
| `HIS.Desktop.Plugins.TrackingPrint.UsedDayCountingFormatOption` | Định dạng hiển thị (1 = đọc số thành chữ cho GN/HT) | 1 / khác | — | HisTrackingList |
| `HIS.Desktop.Plugins.TrackingPrint.UsedDayCountingOutStockOption` | Đếm cả thuốc ngoài kho | 1 / khác | — | HisTrackingList |
| **`HIS.Desktop.Plugins.TrackingPrint.UsedDayCountingAddPreviousUseDay`** (MỚI) | **Cộng thêm Số lần SD thuốc trước đó (PREVIOUS_USING_COUNT) khi đếm số ngày dùng thuốc** | **1 = Có; Khác = Không** | **Trống (⇒ Không)** | **HisTrackingList** |

> **Không có UI cấu hình riêng** cho các key này — quản trị qua màn hình cấu hình hệ thống chung (HIS_CONFIG), giống các key `UsedDayCounting*` sẵn có. Do đó feature này **không tạo/không sửa Form/UC** ⇒ không phát sinh phần thiết kế giao diện DevExpress/LayoutControl.

### SQL bổ sung cấu hình (chạy phía DB — không thuộc code frontend)
```sql
-- Thêm cấu hình mặc định (Trống = Không cộng). Chỉnh giá trị = 1 để bật.
INSERT INTO HIS_CONFIG (ID, CONFIG_CODE, CONFIG_NAME, VALUE, IS_ACTIVE)
VALUES (SEQ_HIS_CONFIG.NEXTVAL,
        'HIS.Desktop.Plugins.TrackingPrint.UsedDayCountingAddPreviousUseDay',
        N'Tùy chọn cộng thêm Số lần sử dụng thuốc trước đó khi đếm số ngày sử dụng thuốc (1: Có, Khác: Không)',
        NULL, 1);
```
*(Tên cột/ sequence theo chuẩn HIS_CONFIG của hệ thống — điều chỉnh nếu schema khác.)*

## 5. Files Thay Đổi

| # | File | Thay đổi |
|---|------|----------|
| 1 | `MPS/MPS.Processor/MPS.Processor.Mps000062.PDO/Mps000062PDO.cs` | Thêm property `long UsedDayCountingAddPreviousUseDay` vào `Mps000062SingleKey` |
| 2 | `MPS/MPS.Processor/MPS.Processor.Mps000062/Mps000062Processor.cs` | Sửa `GetUsedDayCounting()`: tính `previousUseDay` + tách `isVneseFormat`; thêm helper `GetNumberOfUseInDayWithPreviousUseDay()` |
| 3 | `HIS/Plugins/HIS.Desktop.Plugins.HisTrackingList/Config/ConfigKeyss.cs` | Thêm const `DBCODE__..._ADD_PREVIOUS_USE_DAY` |
| 4 | `HIS/Plugins/HIS.Desktop.Plugins.HisTrackingList/Event/frmHisTrackingList__Pluss_Print.cs` | Đọc config + gán `singleKey.UsedDayCountingAddPreviousUseDay` |
| 5 | `HIS/Plugins/HIS.Desktop.Plugins.TrackingCreate/ConfigKeyss.cs` | Thêm const tương ứng |
| 6 | `HIS/Plugins/HIS.Desktop.Plugins.TrackingCreate/frmTrackingCreate__Pluss__Print.cs` | Đọc config + gán `singleKey.UsedDayCountingAddPreviousUseDay` |

### Logic cốt lõi (`GetUsedDayCounting`)
```csharp
long previousUseDay = (rdo._WorkPlaceSDO.UsedDayCountingAddPreviousUseDay == 1)
    ? (medi.PREVIOUS_USING_COUNT ?? 0) : 0;
// Option 1 -> NUMBER_H_N + previousUseDay
// Option 2 -> NUMBER_BY_TYPE + previousUseDay
// Option 3 -> NUMBER_USE_AND_ACTIVE + previousUseDay
// Option 4 -> (NUMBER_BY_TYPE|NUMBER_H_N) + previousUseDay
// Option 5 -> phần số nguyên của NUMBER_OF_USE_IN_DAY ("N" hoặc "N.i") + previousUseDay
// Option 6 -> NUMBER_ACTIVE_INGR + previousUseDay
```
> Khi config ≠ 1 ⇒ `previousUseDay = 0` ⇒ kết quả **giống hệt** trước đây (an toàn tuyệt đối cho các viện chưa bật).

## 6. Dependencies

| Thành phần | Vai trò |
|-----------|---------|
| `MPS.Processor.Mps000062.PDO` (DLL) | Chứa `Mps000062SingleKey` — carrier các option in |
| `MPS.Processor.Mps000062` (DLL) | Render mẫu in, dùng `GetUsedDayCounting` |
| `HIS.Desktop.LocalStorage.HisConfig.HisConfigs` | Đọc cấu hình hệ thống |

## 7. Build & Deploy (QUAN TRỌNG — thứ tự bắt buộc)

Processor tham chiếu PDO dưới dạng **DLL dựng sẵn** (HintPath), KHÔNG phải ProjectReference. `MPS.Processor.Mps000062.PDO.csproj` có `PostBuildEvent` tự copy `.PDO.dll` sang `lib\MPSv2\MPS.PDO` **và** `histest\x64\ReferencedAssemblies`.

Thứ tự build (VS2022 / MSBuild):
1. **Build `MPS.Processor.Mps000062.PDO`** → DLL mới (có property) tự copy sang 2 nơi trên.
2. **Build `MPS.Processor.Mps000062`** → nhìn thấy property mới; PostBuild copy processor DLL sang `lib\MPSv2\MPS.Processor` + `histest\x64\Plugins\MpsProcessor`.
3. **Build `HIS.Desktop.Plugins.HisTrackingList`** và **`HIS.Desktop.Plugins.TrackingCreate`**.
4. Copy các DLL plugin sang `histest\x64\Plugins\Module` (theo quy trình deploy histest).

> Nếu bỏ bước 1 (chỉ build processor/plugin) → lỗi "does not contain a definition for 'UsedDayCountingAddPreviousUseDay'" do vẫn dùng DLL PDO cũ.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 06/07/2026 | phuongnm | Bổ sung cấu hình `HIS.Desktop.Plugins.TrackingPrint.UsedDayCountingAddPreviousUseDay`. Sửa `Mps000062.GetUsedDayCounting` để cộng thêm `PREVIOUS_USING_COUNT` vào số đếm ngày dùng thuốc trên bản in `.repx` (tương đương key Excel `USING_COUNT_NUMBER`). Áp dụng đồng bộ cho HisTrackingList và TrackingCreate. |

## 9. Test Cases

### Chuẩn bị
- 1 bệnh nhân nội trú, thuốc kê có nhập **"Số ngày sử dụng thuốc trước đó"** = ví dụ `2` (⇒ `PREVIOUS_USING_COUNT = 2`).
- Đơn có ≥ 3 lần y lệnh (⇒ số đếm hiện tại ví dụ `3`).

### Khi cấu hình để TRỐNG hoặc ≠ 1 (regression — không đổi hành vi)
- [ ] In tờ điều trị `.repx` → số đếm hiển thị **giữ nguyên như bản cũ** (ví dụ `(3)`).
- [ ] Lần lượt đổi `UsedDayCountingOption` = 1..6 → mọi option hiển thị y như trước khi sửa.

### Khi cấu hình = 1 (feature bật)
- [ ] `UsedDayCountingOption = 1` → hiển thị `(5)` (= 3 + 2).
- [ ] `UsedDayCountingOption = 2/3/4/6` → số đếm tương ứng đều **+2**.
- [ ] `UsedDayCountingOption = 5`, giá trị gốc `"3"` → `"5"`; giá trị gốc `"3.1"` → `"5.1"` (giữ nguyên phần `.i`).
- [ ] `UsedDayCountingFormatOption = 1` + thuốc Gây nghiện/Hướng thần → đọc số thành chữ đúng theo tổng mới (ví dụ `(Năm)`).
- [ ] Thuốc không nhập số ngày trước đó (`PREVIOUS_USING_COUNT` null) → cộng `0` ⇒ hiển thị như cũ.
- [ ] So sánh: giá trị hiển thị trên `.repx` = key Excel `USING_COUNT_NUMBER` (nhất quán 2 bản in).

### In từ cả 2 nguồn
- [ ] In từ **Danh sách tờ điều trị** (HisTrackingList) → áp dụng đúng.
- [ ] In từ **Tạo/sửa tờ điều trị** (TrackingCreate) → áp dụng đúng.
- [ ] Các key gộp `MEDICINES_MERGE*___DATA` cũng cộng đúng (dùng chung `GetUsedDayCounting`).
