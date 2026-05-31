# Kiểm Tra Truyền Máu — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood |
| Loại | Form |
| Mục đích | Màn hình KTV kiểm tra hòa hợp túi máu trước khi truyền (ABO, Rh, MT muối ống 1/2, Anti-globulin ống 1/2, Pháp ứng chéo, Scangel/Gelcard, Coombs, Tự chứng AC, Thời gian rã đông). Tự động map kết quả XN hòa hợp từ Lab vào 4 ô combobox dựa trên cấu hình bộ mã chỉ số. |
| Người tạo | Inventec |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

1. KTV mở phiếu xuất máu (`HIS_EXP_MEST`) ở trạng thái đã duyệt.
2. Form tự load:
   - Thông tin bệnh nhân, túi máu (`HIS_EXP_MEST_BLOOD`).
   - Dropdown "XN hòa hợp" (`cboXNHH`) — danh sách kết quả XN từ Lab.
3. KTV click chọn 1 túi máu trên `treeListExpBlood` → auto-fill MT muối + Anti-globulin theo vị trí ống.
4. KTV chỉnh thủ công (hoặc chọn lại dropdown XN hòa hợp) khi cần.
5. KTV lưu phiếu kiểm tra (POST `/api/HisExpMest/UpdateTestInfo`).
6. KTV in phiếu truyền máu/phát máu (Mps000421).

### Dropdown "XN hòa hợp" — Quy tắc xây dựng

Mỗi dòng dropdown = 1 túi máu thuộc 1 y lệnh. Sinh dòng theo logic:

```
Lọc V_HIS_SERE_SERV_TEIN theo TDL_TREATMENT_ID
  → Nhóm theo TDL_SERVICE_REQ_ID (y lệnh)
    → Với mỗi y lệnh × mỗi bộ cấu hình:
        - Tìm bản ghi A (mã chỉ số "Mã túi máu") mới nhất → có → sinh 1 dòng
        - Tìm bản ghi B (mã chỉ số "Hòa hợp muối") mới nhất → fill cột MT muối
        - Tìm bản ghi C (mã chỉ số "Hòa hợp anti-globulin") mới nhất → fill cột Anti-globulin
        - Không có A → bỏ qua bộ này
    → Sắp xếp giảm dần theo MODIFY_TIME của A (đan xen các y lệnh)
```

### Auto-fill khi click túi máu

- `TUBE_SLOT` không phải 1 hoặc 2 → không xử lý.
- Slot đã đầy cả 2 ô (MT muối + Anti-globulin) → không ghi đè.
- Match dòng dropdown theo `BLOOD_VALUE = data.BLOOD_CODE` (mã túi thực tế). Nhiều dòng khớp → lấy mới nhất theo `MODIFY_TIME`.
- Chỉ điền ô đang trống trong slot tương ứng.

### Chọn thủ công từ dropdown

- Vị trí ống = 1 → fill `cboSaltEnvi` + `cboAntiGlobulin` (ghi đè).
- Vị trí ống = 2 → fill `cboSaltEnviTwo` + `cboAntiGlobulinTwo` (ghi đè).
- Vị trí ống khác (NULL hoặc ≠ 1, 2) → revert dropdown, hiển thị thông báo "Loại máu chưa khai báo vị trí ống nghiệm.".

### Điều kiện nghiệp vụ

- `TUBE_SLOT` khai báo trong vCong37742 (HIS_BLOOD_TYPE.TUBE_SLOT / HIS_PREPARATIONS_BLOOD).
- Cấu hình hệ thống `BloodHarmonyTestIndex` rỗng → dropdown rỗng, plugin vẫn hoạt động bình thường (early exit).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_EXP_MEST / V_HIS_EXP_MEST | Table / View | Phiếu xuất máu |
| HIS_EXP_MEST_BLOOD / V_HIS_EXP_MEST_BLOOD | Table / View | Chi tiết túi máu trong phiếu |
| HIS_EXP_BLTY_SERVICE / V_HIS_EXP_BLTY_SERVICE | Table / View | Dịch vụ máu kèm theo phiếu |
| HIS_BLOOD_TYPE | Table | Loại máu (chứa TUBE_SLOT, BLOOD_GROUP_ID, PREPARATIONS_BLOOD_ID) |
| HIS_BLOOD_GROUP | Table | Nhóm máu (BLOOD_ERYTHROCYTE, BLOOD_PLASMA) |
| HIS_PREPARATIONS_BLOOD | Table | Chế phẩm máu (PREPARATIONS_BLOOD_CODE — quyết định default cho 4 combo) |
| V_HIS_SERE_SERV_TEIN | View | Chỉ số XN của đợt điều trị — nguồn dữ liệu dropdown XN hòa hợp |
| V_HIS_PATIENT | View | Thông tin bệnh nhân |
| V_HIS_TREATMENT | View | Đợt điều trị (dùng cho in) |
| V_HIS_TRANSFUSION_SUM / HIS_TRANSFUSION | View / Table | Lịch sử truyền máu (dùng cho in) |

### Quan hệ chính

- `HIS_EXP_MEST.TDL_TREATMENT_ID` → `HIS_TREATMENT.ID`
- `V_HIS_SERE_SERV_TEIN.TDL_SERVICE_REQ_ID` → `HIS_SERVICE_REQ.ID` (1 y lệnh có N tests)
- `HIS_EXP_MEST_BLOOD.BLOOD_TYPE_ID` → `HIS_BLOOD_TYPE.ID` → `TUBE_SLOT`

## 4. UI Layout

### Sơ đồ giao diện

```
+--------------------------------------------------------------+
| TreeList: Danh sách túi máu (BLOOD_CODE, BLOOD_TYPE, ...)    |
+--------------------------------------------------------------+
| ABO mới  | RH mới  | Phản ứng chéo  | Scangel/Gelcard        |
| Coombs   | XN hòa hợp (dropdown ▼)                            |
| Test tube ống 1   | MT muối ống 1   | Anti-globulin ống 1    |
| Test tube ống 2   | MT muối ống 2   | Anti-globulin ống 2    |
| Tự chứng AC1 | Tự chứng AC2  | Thời gian rã đông | Ghi chú   |
+--------------------------------------------------------------+
| [Thêm] [In Mps000421]                                        |
+--------------------------------------------------------------+
```

### Control chính

| Control | Vai trò |
|---------|---------|
| `treeListExpBlood` | Danh sách túi máu; click → load dữ liệu vào panel chi tiết |
| `cboXNHH` | Dropdown XN hòa hợp (4 cột: Thời gian / Mã túi máu / MT muối / Anti-globulin) |
| `cboSaltEnvi` / `cboSaltEnviTwo` | MT muối ống 1 / ống 2 |
| `cboAntiGlobulin` / `cboAntiGlobulinTwo` | Anti-globulin ống 1 / ống 2 |
| `txtTestTube` / `txtTestTubeTwo` | Mã ống nghiệm 1 / 2 |
| `cboAC` / `cboAC2` | Tự chứng AC |
| `dtDefrostTime` | Thời gian rã đông |

## 5. API Endpoints

| Action | URI | Consumer | Filter / DTO |
|--------|-----|----------|--------------|
| Lấy chi tiết phiếu | `/api/HisExpMest/GetView` | MosConsumer | `HisExpMestViewFilter` |
| Lấy chỉ số XN | `api/HisSereServTein/GetView` | MosConsumer | `HisSereServTeinViewFilter` (lọc theo `TDL_TREATMENT_ID`) |
| Lấy túi máu | `/api/HisExpMestBlood/GetView` | MosConsumer | `HisExpMestBloodViewFilter` |
| Lấy DV máu | `/api/HisExpBltyService/GetView` | MosConsumer | `HisExpBltyServiceViewFilter` |
| Lưu kết quả | `/api/HisExpMest/UpdateTestInfo` | MosConsumer | DTO update |
| Lấy điều trị (in) | `/api/HisTreatment/GetView` | MosConsumer | `HisTreatmentViewFilter` |
| Lịch sử truyền | `/api/HisTransfusionSum/GetView` | MosConsumer | `HisTransfusionSumViewFilter` |

## 6. Dependencies

### Library Plugins

| Library | Mục đích |
|---------|----------|
| `HIS.Desktop.Plugins.Library.EmrGenerate` | Tạo `Inventec.Common.SignLibrary.ADO.InputADO` cho ký số EMR khi in Mps000421 |

### Inter-Plugin

Không có. Plugin nhận input qua `frmInputExpMestId` (dialog con) hoặc qua constructor `expMestId`.

## 7. Print

| Loại in | PrintTypeCode | PDO | Ghi chú |
|---------|---------------|-----|---------|
| Phiếu truyền máu và phát máu | Mps000421 | `MPS.Processor.Mps000421.PDO.Mps000421PDO` | Có ký số EMR (`EmrInputADO`). Mode: `PrintNow` nếu config `CheDoInChoCacChucNangTrongPhanMem == 2`, ngược lại `ShowDialog` |
| Phiếu truyền máu (khoa lâm sàng) | Mps000271 | `MPS.Processor.Mps000271.PDO.Mps000271PDO` | In **1 phiếu / 1 túi máu** (`expMestBlood` single key). Phần I (XN hòa hợp miễn dịch) lấy theo mẫu Mps000421. KHÔNG đổi PDO/Processor C# — chỉ sửa template Excel |

### Template Mps000271 — Phần I (XN hòa hợp miễn dịch)

File: `HIS/Plugins/Mps000271__PhieuTruyenMau.xlsx`. Phần I được ghép bảng thông tin túi máu + kết quả XN hòa hợp (cột giống mẫu Mps000421), bind theo single key `V_HIS_EXP_MEST_BLOOD` (1 túi máu/phiếu):

| Cột | Nguồn dữ liệu |
|-----|---------------|
| STT | Cố định `1` |
| Mã số túi máu | `<#BLOOD_CODE;>` |
| Tên thành phần | `<#BLOOD_TYPE_NAME;>` |
| Nhóm ABO / Rh | `<#BLOOD_ABO_CODE;>` / `<#BLOOD_RH_CODE;>` |
| Thể tích (ml) | `<#VOLUME;>` |
| Ngày sản xuất | `<#FlFuncTimeNumberToDateString(<#PACKING_TIME;>)>` |
| Hạn sử dụng | `<#FlFuncTimeNumberToDateString(<#EXPIRED_DATE;>)>` |
| 22°C MT nước muối | IF theo `<#SALT_ENVI;>` (ô phụ R11) |
| 37°C MT Anti Globulin | IF theo `<#ANTI_GLOBULIN_ENVI;>` (ô phụ S11) |

Hai cột XN miễn dịch dùng **công thức IF Excel** map giá trị số (combo Id) → text hiển thị (đúng pattern helper-cell + formula sẵn có trong template): `1→1+, 2→2+, 3→3+, 4→4+, 5→Âm tính, 6→0.5+, 7→5+, 8→Khác`. Ô phụ chứa tag FlexCel nằm ở cột R/S (ngoài Print_Area `A1:N`). So sánh dạng `(R11&"")="1"` để an toàn cả khi FlexCel ghi giá trị số hoặc text.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-05-19 | dangth2 | vCong44937 — Sửa logic mapping XN hòa hợp: đổi format config từ "3 nhóm A;B;C" sang "N bộ A\|B\|C". Nhóm dropdown theo y lệnh × bộ cấu hình thay vì theo từng dịch vụ. Match túi máu theo `BLOOD_VALUE` thay vì sere_serv_id. Manual select kiểm tra `TUBE_SLOT` ∈ {1, 2}, hiển thị cảnh báo nếu vị trí ống chưa khai báo. Bỏ logic fallback `AutoFillCombosFromXNHHIfNeeded`, `GetComboEnviItemName`, `MatchHarmonyValue`. |
| 2026-05-20 | dangth2 | vCong44937 — `SetComboEnviValue` đổi sang fuzzy match theo cặp 2 token. VALUE từ Lab map sang ItemName combo nếu chứa ĐỒNG THỜI cả 2 token đã khai báo. Cặp ưu tiên đặc trưng trước: ("âm","tính") → "Âm tính"; ("0.5","+") → "0.5+"; ("5","+") → "5+"; ("4","+") → "4+"; ("3","+") → "3+"; ("2","+") → "2+"; ("1","+") → "1+". Xử lý các chuỗi tự do từ Lab (VD: "Ngưng kết 2 (+)", "1 +", "âm tính (-)"). |
| 2026-05-20 | dangth2 | vCong44937 — Tạo `Resource/Message.Lang.vi.resx` + `Resource/Message.Lang.en.resx` + `Resource/ResourceMessage.cs`. Thông báo "Loại máu chưa khai báo vị trí ống nghiệm." chuyển từ hardcode sang `ResourceMessage.LoaiMauChuaKhaiBaoViTriOngNghiem` (đa ngôn ngữ). Đăng ký file mới vào `.csproj`. |
| 2026-05-20 | dangth2 | vCong44937 — Bổ sung flag `IsToken2Optional` cho `HisComboValuePair`. Với các cặp digit "N+", dấu "+" trở thành tùy chọn: Lab trả VALUE chỉ là "2" (không kèm "+") vẫn map đúng sang "2+". Cặp "Âm tính" giữ bắt buộc cả 2 token. |
| 2026-05-20 | dangth2 | vCong44937 — Default fill 4 combo MT muối / Anti-globulin khi mở "Kiểm tra truyền máu" đổi sang **driven theo `TUBE_SLOT`** (nhất quán, không phụ thuộc chế phẩm máu): `TUBE_SLOT = 1` → chỉ fill ống 1 "Âm tính" (ống 2 trống); `TUBE_SLOT = 2` → chỉ fill ống 2 "Âm tính" (ống 1 trống); khác → để trống cả 4 ô. Bỏ logic cũ `PREPARATIONS_BLOOD_CODE` ("1"/"3"/"4"/"5"/"6") fill khác nhau theo chế phẩm. DB đã lưu → vẫn ưu tiên load DB. |
| 2026-05-20 | dangth2 | vCong44937 — Fix pair A/B/C trong `BuildTestHarmonyList` và auto-fill: (1) Trong `BuildTestHarmonyList`, B/C lookup ƯU TIÊN cùng `SERE_SERV_ID` với A (cùng dịch vụ thực hiện = cùng túi máu) → tránh pair lệch khi nhiều bag cùng `MODIFY_TIME`; fallback `OrderByDescending(MODIFY_TIME).ThenByDescending(SERE_SERV_TEIN_ID)` cho ổn định. (2) Trong `ProcessAutoFillFromTestHarmony`, capture cboXNHH đang chọn trước khi reset — nếu `BLOOD_VALUE` khớp `BLOOD_CODE` túi máu → dùng dòng đó (respect user choice); ngược lại fallback `FindHarmonyByBloodCode`. Giải quyết case nhiều túi máu cùng thời gian khiến fill lấy nhầm "latest". |
| 2026-05-20 | dangth2 | vCong44937 — Click tree túi máu = behave như manual select cboXNHH (R8 ghi đè): bỏ R11/R12 early-return ("slot đầy → không ghi đè") và R14 ("chỉ fill ô trống") trong `ProcessAutoFillFromTestHarmony`. Lý do: DB có thể đã lưu default "Âm tính" từ bước Duyệt phiếu xuất → tránh trường hợp slot có giá trị DB nhưng cboXNHH hiển thị giá trị XN khác, dẫn đến không sync. Giờ click tree luôn fill slot bằng giá trị từ XN hòa hợp đã match. Manual select cboXNHH vẫn giữ R7/R8 (overwrite theo TUBE_SLOT). |
| 2026-05-20 | dangth2 | vCong44937 — Thêm option **"Khác" (Id=8)** cho 6 combo: MT muối / MT muối 2 / Anti globulin / Anti globulin 2 (`LoadDataToComboboxEnvironment`, ComboboxADO Id=8) + Tự chứng AC / AC2 (`LoadComboAC`, ADO decimal 8). `SetComboEnviValue` fallback chọn "Khác" khi VALUE từ LIS không khớp option chuẩn (thay vì để trống). Id=8 là quy ước giá trị tự quyết ở frontend (lưu vào cột `SALT_ENVI`/`ANTI_GLOBULIN`/`AC_SELF_ENVIDENCE`... như các option khác) — không thay đổi schema DB. |
| 2026-05-27 | dangth2 | vCong44937 — Sửa template Excel **Mps000271** (`HIS/Plugins/Mps000271__PhieuTruyenMau.xlsx`): ghép Phần I (bảng thông tin túi máu + XN hòa hợp miễn dịch) theo mẫu Mps000421 vào đầu phiếu. Chèn 3 dòng (2 dòng header + 1 dòng dữ liệu) sau dòng Chẩn đoán, dịch toàn bộ nội dung Phần II/bảng theo dõi/chữ ký xuống +3 dòng (cập nhật `mergeCells`, `dimension A1:BA43`, `Print_Area $A$1:$N$33`, FlexCel band `__ListTransfusion__ $A$20:$N$20`, các công thức tham chiếu START_TIME/FINISH_TIME). 2 cột XN miễn dịch dùng **công thức IF** map `<#SALT_ENVI;>`/`<#ANTI_GLOBULIN_ENVI;>` (số → text) qua ô phụ R11/S11 ngoài Print_Area. Bind single key `V_HIS_EXP_MEST_BLOOD` (1 túi máu/phiếu) — **không** đổi PDO/Processor C#. Xóa `calcChain.xml` + đặt `fullCalcOnLoad="1"` để recompute. Backup: `.xlsx.bak`. |

### Format cấu hình `BloodHarmonyTestIndex`

**Format cũ** (3 nhóm A/B/C, mỗi nhóm nhiều mã):
```
XN001|XN002;XN011|XN012;XN101|XN102
```

**Format mới** (N bộ, mỗi bộ 3 mã = 1 túi máu):
```
A|B|C;A|B|C;...
```
- `;` ngăn cách các bộ (mỗi bộ = 1 túi máu).
- `|` ngăn cách 3 mã trong 1 bộ — thứ tự bắt buộc:
  1. Mã chỉ số "Mã túi máu" (A) — giá trị = mã túi thực tế.
  2. Mã chỉ số "Hòa hợp muối" (B).
  3. Mã chỉ số "Hòa hợp anti-globulin" (C).

Ví dụ 2 túi: `PM_H1MTM|331966|331964;PM_H2MTM|331992|331990`

Bộ thiếu mã A → bỏ qua. B hoặc C rỗng → dòng dropdown vẫn hiển thị, cột tương ứng để trống.

## 9. Test Cases

### Dropdown XN hòa hợp

- [ ] Cấu hình 4 bộ + y lệnh có 4 tests A/B/C/D → dropdown hiển thị 4 dòng đầy đủ.
- [ ] Cấu hình 4 bộ + y lệnh chỉ có 1 tests có chỉ số A → dropdown 1 dòng, 3 bộ khác bị bỏ qua.
- [ ] Cấu hình 4 bộ + y lệnh có A nhưng thiếu B → dòng dropdown hiện, cột MT muối trống.
- [ ] BN có 2 y lệnh × 2-3 tests → dropdown hiển thị tất cả dòng, sắp xếp giảm dần theo MODIFY_TIME (đan xen, không tách block).
- [ ] Cấu hình rỗng → dropdown rỗng, không lỗi.
- [ ] Bộ cấu hình không đủ 3 mã (`A|B` thiếu C) → bỏ qua bộ, log warn.

### Auto-fill khi click túi máu

- [ ] Click túi mã A, slot 1 → MT muối ống 1 và Anti-globulin ống 1 được điền từ bộ khớp mã A.
- [ ] Click túi mã A, slot 2 → MT muối ống 2 và Anti-globulin ống 2 được điền. Slot 1 không đổi.
- [ ] Slot 1 đã có cả MT muối ống 1 và Anti-globulin ống 1 → click túi lại → không ghi đè.
- [ ] Slot 1 có MT muối ống 1 nhưng thiếu Anti-globulin ống 1 → click → chỉ điền Anti-globulin ống 1.
- [ ] Túi máu có `TUBE_SLOT = NULL` → click túi không auto-fill, không thông báo.
- [ ] Mã túi (BLOOD_CODE) không khớp bất kỳ dòng dropdown nào → không fill, dropdown clear.

### Chọn thủ công từ dropdown

- [ ] Túi máu có `TUBE_SLOT = 1`, chọn dòng dropdown → ghi đè MT muối ống 1 và Anti-globulin ống 1.
- [ ] Túi máu có `TUBE_SLOT = 2`, chọn dòng dropdown → ghi đè MT muối ống 2 và Anti-globulin ống 2.
- [ ] Túi máu có `TUBE_SLOT = NULL`, chọn dòng dropdown → hiển thị thông báo "Loại máu chưa khai báo vị trí ống nghiệm.", dropdown revert.
- [ ] Dòng dropdown có MT muối rỗng → combo MT muối được clear.

### In

- [ ] Phiếu trạng thái xuất (`EXP_MEST_STT_ID = 5`) → nút In bật.
- [ ] Click In → preview Mps000421 hiển thị đúng data túi máu, DV máu, lịch sử truyền.
- [ ] Config `CheDoInChoCacChucNangTrongPhanMem = 2` → in trực tiếp không hiện preview.
