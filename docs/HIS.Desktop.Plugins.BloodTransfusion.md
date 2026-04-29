# Truyền Máu — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.BloodTransfusion |
| Loại | Form |
| Mục đích | Quản lý quá trình truyền máu cho bệnh nhân: chọn phiếu xuất máu, ghi nhận lần truyền (lần thứ, thời gian, người truyền, dung tích, chẩn đoán) và nhập các lần theo dõi chỉ số (tốc độ truyền, mạch, huyết áp, nhịp thở, thân nhiệt, ghi chú) trong suốt quá trình truyền |
| Người tạo | INVENTEC |
| Ngày tạo | 16/04/2025 |
| Trạng thái | Đang sử dụng |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

1. Người dùng tìm bệnh nhân/điều trị bằng mã điều trị, mã BN hoặc từ khóa.
2. Form load grid `Phiếu xuất máu` (V_HIS_EXP_MEST_BLOOD) đã được duyệt cho điều trị.
3. Người dùng chọn 1 phiếu xuất máu → bấm nút Truyền máu (cột "TransfusionBlood") để ghi nhận lần truyền mới, hoặc click vào dòng `Lần truyền máu` (V_HIS_TRANSFUSION_SUM) đã có để xem/sửa.
4. Form điền dữ liệu mặc định: người truyền = login hiện tại, thời gian truyền từ/đến = hiện tại, dung tích truyền = dung tích phiếu, chẩn đoán = chẩn đoán điều trị.
5. Lưu (Ctrl+S) → gọi API `HisTransfusionSum/CreateOrUpdateSdo` → cập nhật grid và bật nút In.
6. Trong panel `Các lần theo dõi truyền máu`:
   - Click cột header `+` (Action) → tạo dòng theo dõi mới với MEASURE_TIME = hiện tại.
   - Dữ liệu các chỉ số được sao chép từ lần theo dõi mới nhất trong phiếu hiện tại; nếu phiếu trống thì sao chép từ lần theo dõi mới nhất trong hồ sơ điều trị; nếu cả hai đều trống → tạo dòng rỗng.
   - Click nút `Sao chép` ở cuối dòng N → tạo dòng mới với MEASURE_TIME = hiện tại và sao chép toàn bộ chỉ số y hệt dòng N.
   - Sửa giá trị bất kỳ ô → API `HisTransfusion/Update` được gọi tự động (CellValueChanged).
   - Click X → xóa dòng theo dõi.
7. In (Ctrl+P) → gọi mẫu Mps000271 (Phiếu truyền máu).

### Điều kiện nghiệp vụ

- Form chỉ cho phép Lưu/Thêm lần theo dõi/Sao chép khi điều trị **đang mở** (`IS_PAUSE != 1`) và phòng làm việc thuộc khoa điều trị (`LAST_DEPARTMENT_ID == currentRoom.DEPARTMENT_ID`) hoặc thuộc khoa cùng điều trị (`CO_TREAT_DEPARTMENT_IDS`).
- Mở từ danh sách điều trị (read-only mode) → không thêm/sửa/xóa được.
- Phiếu đã có lần truyền (TRANSFUSION_SUM) thì cột TransfusionBlood disable, ép người dùng click vào lần truyền có sẵn để chỉnh sửa.
- Validation thời gian đo: phải nằm trong khoảng `START_TIME` – `FINISH_TIME` của lần truyền (gridViewTransfusion_ValidatingEditor).
- Chỉ user tạo (CREATOR) hoặc admin được phép xóa lần truyền (gridColumn `Delete` của TransfusionSum).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TREATMENT | View | Thông tin điều trị (mã, BN, khoa, đoán, IS_PAUSE) |
| V_HIS_EXP_MEST_BLOOD | View | Phiếu xuất máu đã duyệt |
| V_HIS_TRANSFUSION_SUM | View | Lần truyền máu (tổng quan) |
| HIS_TRANSFUSION | Table | Lần theo dõi truyền máu (chi tiết chỉ số) |
| HIS_TRANSFUSION_SUM | Table | Bảng gốc lần truyền (tạo/cập nhật/xóa) |
| HIS_ICD | Table | Danh mục ICD cho UC chẩn đoán |
| ACS_USER | Table | Combo người truyền |
| V_HIS_ROOM | View | Phòng hiện tại (xác định khoa) |

### Quan hệ

- `HIS_TREATMENT` 1-n `HIS_EXP_MEST_BLOOD` (qua `TDL_TREATMENT_ID`).
- `HIS_EXP_MEST_BLOOD` 1-n `HIS_TRANSFUSION_SUM` (qua `EXP_MEST_BLOOD_ID`).
- `HIS_TRANSFUSION_SUM` 1-n `HIS_TRANSFUSION` (qua `TRANSFUSION_SUM_ID`).

## 4. UI Layout

### Sơ đồ giao diện

```
+---------------------------------------------------------------------+
| Mã điều trị | Mã BN | Từ khóa | Tìm (Ctrl+F)                        |
+---------------------------------------------------------------------+
| Grid Phiếu xuất máu (V_HIS_EXP_MEST_BLOOD)                          |
|  STT | Status | Mã ĐT | Mã máu | ... | TG xuất | Truyền máu        |
+---------------------------------------------------------------------+
| Phân trang                                                          |
+--------------------------------+------------------------------------+
| Grid Lần truyền máu            | Form chi tiết lần truyền          |
|  STT | Mã máu | TG từ-đến |    | Người truyền, TG, Lần thứ, Dung   |
|  Người truyền | TT | Xóa       | tích, CĐ chính, CĐ phụ, Phản ứng  |
|                                | chéo, Ghi chú                     |
+--------------------------------+ + Lưu (Ctrl+S) / In (Ctrl+P)       |
| Grid Các lần theo dõi (Transfusion)                                 |
|  X | TG đo | Tốc độ | Da | Nhịp thở | Mạch | HA(Max) | HA(Min) |    |
|    Thân nhiệt | Diễn biến | [Sao chép]                              |
+---------------------------------------------------------------------+
```

### Buttons / Cột đặc biệt

| Cột | FieldName | Hành động |
|-----|-----------|-----------|
| Action (header `+`) | Action | Click header → thêm lần theo dõi mới (sao chép từ lần mới nhất) |
| Action (cells `X`) | Action | Click → xóa lần theo dõi đó |
| Copy (cells `Sao chép`) | Copy | Click → sao chép toàn bộ chỉ số dòng N thành lần theo dõi mới với TG đo = hiện tại |

### UC sử dụng

| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.Icd | panelControlIcdMain | Chẩn đoán chính |
| HIS.UC.SecondaryIcd | panelControlIcdSub | Chẩn đoán phụ |
| Inventec.UC.Paging | ucPaging | Phân trang grid phiếu xuất máu |

## 5. API Endpoints

| Action | URI | Consumer | Filter/Body |
|--------|-----|----------|-------------|
| Lấy danh sách phiếu xuất máu | `api/HisExpMestBlood/GetView` | MosConsumer | HisExpMestBloodViewFilter |
| Lấy điều trị theo mã | `api/HisTreatment/GetView` | MosConsumer | HisTreatmentViewFilter |
| Lấy danh sách lần truyền | `api/HisTransfusionSum/GetView` | MosConsumer | HisTransfusionSumViewFilter |
| Lấy danh sách lần theo dõi | `api/HisTransfusion/Get` | MosConsumer | HisTransfusionFilter (lọc theo `TRANSFUSION_SUM_ID`) |
| Tạo lần theo dõi | `api/HisTransfusion/Create` | MosConsumer | HIS_TRANSFUSION |
| Cập nhật lần theo dõi | `api/HisTransfusion/Update` | MosConsumer | HIS_TRANSFUSION |
| Xóa lần theo dõi | `api/HisTransfusion/Delete` | MosConsumer | id (long) |
| Tạo/Cập nhật lần truyền | `api/HisTransfusionSum/CreateOrUpdateSdo` | MosConsumer | HisTransfusionSumSDO |
| Xóa lần truyền | `api/HisTransfusionSum/Delete` | MosConsumer | id (long) |

## 6. Dependencies

### Library Plugins

Không sử dụng Library plugins.

### Inter-Plugin

Plugin chỉ nhận tham số `Module` từ `BloodTransfusionProcessor.Run(args)`. Không mở plugin khác.

## 7. Print

| Loại in | PrintTypeCode | Cách gọi | Template |
|---------|---------------|----------|----------|
| Phiếu truyền máu | Mps000271 | RichEditorStore.RunPrintTemplate("Mps000271", delegateRunPrint) | SAR template "Mps000271" |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 16/04/2025 | INVENTEC | Tạo plugin BloodTransfusion |
| 28/04/2026 | dangth2 (Việc 42612) | Đổi hành vi nút "+" của grid `Các lần theo dõi truyền máu`: tự sao chép chỉ số từ lần theo dõi mới nhất (ưu tiên 1: phiếu hiện tại; ưu tiên 2: phiếu khác trong cùng điều trị; ưu tiên 3: tạo dòng rỗng); MEASURE_TIME = thời điểm hiện tại. Thêm cột nút `Sao chép` ở cuối grid: click trên dòng N → tạo dòng mới với MEASURE_TIME hiện tại, copy y hệt chỉ số dòng N. Áp dụng cho BV HAGL và toàn bộ viện (không gắn config). |

## 9. Test Cases

### Sao chép tự động khi nhấn nút "+"

- [ ] Phiếu hiện tại có ≥ 1 lần theo dõi → click `+` ở header → dòng mới có TG đo = hiện tại + chỉ số copy từ lần theo dõi có MEASURE_TIME lớn nhất trong phiếu.
- [ ] Phiếu hiện tại trống nhưng điều trị có lần theo dõi ở phiếu khác → click `+` → dòng mới có TG đo = hiện tại + chỉ số copy từ lần theo dõi mới nhất trong điều trị.
- [ ] Phiếu hiện tại và điều trị đều chưa có lần theo dõi → click `+` → dòng mới rỗng (chỉ có TG đo = hiện tại).
- [ ] Sau khi tạo, refresh grid → dòng mới hiển thị ở cuối, các ô đều có thể chỉnh sửa.

### Nút "Sao chép" trên từng dòng

- [ ] Click `Sao chép` trên dòng N → dòng mới có TG đo = hiện tại + chỉ số copy y hệt dòng N (Tốc độ, Da, Nhịp thở, Mạch, HA Max/Min, Thân nhiệt, Ghi chú).
- [ ] Sau khi tạo, refresh grid → dòng mới hiển thị; các ô đều có thể chỉnh sửa và lưu thay đổi qua API Update.
- [ ] Click `Sao chép` khi điều trị `IS_PAUSE = 1` hoặc đang mở từ danh sách điều trị → không tạo dòng mới (tooltip vẫn hiển thị).

### Hành vi nguyên gốc

- [ ] Lưu lần truyền: nhập đủ thông tin → Ctrl+S → API `HisTransfusionSum/CreateOrUpdateSdo` thành công, grid lần truyền refresh, nút In bật.
- [ ] Xóa lần theo dõi: click X → API `HisTransfusion/Delete` thành công, dòng biến mất.
- [ ] Xóa lần truyền: chỉ user tạo hoặc admin được xóa, click → API `HisTransfusionSum/Delete`, refresh 3 grid.
- [ ] In phiếu: chọn lần truyền → Ctrl+P → preview Mps000271.
