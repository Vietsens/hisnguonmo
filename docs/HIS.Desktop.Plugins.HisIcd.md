# Danh Mục ICD — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | `HIS.Desktop.Plugins.HisIcd` |
| Loại | Form (`HIS.Desktop.Utility.FormBase`) |
| Mục đích | Quản lý danh mục mã bệnh ICD-10 (`HIS_ICD`): thêm/sửa/khóa, gắn thuộc tính (COVID, lao tiềm ẩn, mã phụ, YHCT, nguyên nhân tử vong…) |
| EFMODEL | `HIS_ICD` (bảng), `V_HIS_ICD` (view danh sách) |
| Form chính | `HisIcd/frmHisIcd.cs` (2323 dòng) |

## 2. Kiến Trúc & Luồng

```
HisIcdProcessor.Run → HisIcdFactory → HisIcdBehavior.Run → frmHisIcd
frmHisIcd:
  Load → MeShow → FillDataToControlsForm (combo nhóm/tuổi/giới/ICD YHCT) → FillDataToGridControl (LoadPaging: GetView V_HIS_ICD)
  Chọn dòng → ChangedDataRow → FillDataToEditorControl (đổ HIS_ICD -> control)
  Lưu → SaveProcess → UpdateDTOFromDataForm (control -> HIS_ICD updateDTO) → Post Create/Update
```

**Các checkbox thuộc tính (HIS_ICD.IS_*)** — mỗi checkbox theo cùng 1 mẫu 5 điểm:
`chkIsCovid`(IS_COVID), `chkIsInfectious`(IS_INFECTIOUS), `chkIsSword`(IS_SWORD), `chkIsSubcode`(IS_SUBCODE), `chkValid1Year`(VALID_1_YEAR),
`chkIsNotRecommendMain`(IS_NOT_RECOMMEND_MAIN), `chkIsDeathCauseOnly`(IS_DEATH_CAUSE_ONLY),
`chkIsCause`(IS_CAUSE), `chkIsRequireCause`(IS_REQUIRE_CAUSE), `chkIsHeinNds`(IS_HEIN_NDS),
`chkNotUseHein`(DO_NOT_USE_HEIN), `chkIsLatentTuberculosis`(IS_LATENT_TUBERCULOSIS), `chkChanDieuTri`(UNABLE_FOR_TREATMENT), `chkIsChronic`(IS_CHRONIC — 55058, xử lý trong partial `frmHisIcd__Chronic.cs`).

## 3. EFMODEL

`HIS_ICD` cột trạng thái (short? 0/1): `IS_ACTIVE, IS_COVID, IS_INFECTIOUS, IS_SWORD, IS_SUBCODE, IS_CAUSE, IS_REQUIRE_CAUSE, IS_HEIN_NDS, IS_LATENT_TUBERCULOSIS, IS_NOT_RECOMMEND_MAIN, IS_DEATH_CAUSE_ONLY, IS_TRADITIONAL, DO_NOT_USE_HEIN, UNABLE_FOR_TREATMENT, VALID_1_YEAR`.
✅ `IS_INFECTIOUS` (bệnh truyền nhiễm) — có trên cả `HIS_ICD` và `V_HIS_ICD` (dùng cho liên thông ECDS).
🆕 `IS_CHRONIC` (bệnh mãn tính, 55058) — cột mới trên `HIS_ICD` + `V_HIS_ICD` (`docs/SQL_55058_HisIcd_IsChronic.sql`); dùng cảnh báo khi kết thúc điều trị ở `ExamServiceReqExecute`.

## 4. API Endpoints (`HisRequestUriStore`)

| Action | URI |
|--------|-----|
| GetView | `api/HisIcd/GetView` (`HisIcdFilter`, `V_HIS_ICD`) |
| Get | `api/HisIcd/Get` |
| Create | `api/HisIcd/Create` |
| Update | `api/HisIcd/Update` |
| Delete | `api/HisIcd/Delete` |
| ChangeLock | `api/HisIcd/ChangeLock` |

Create/Update nhận `HIS_ICD` đầy đủ (backend map trọn entity) → **thêm cột mới sẽ tự lưu**, không cần sửa API.

---

## 5. Checkbox "Bệnh truyền nhiễm" — ✅ ĐÃ TRIỂN KHAI (28/07/2026)

> Checkbox **"Bệnh truyền nhiễm"** (mặc định KHÔNG tích). Mã ICD được tích dùng để **nhận diện ca bệnh truyền nhiễm** trong chức năng **"Thông tin bệnh truyền nhiễm"** (`HIS.Desktop.Plugins.InfectiousDiseaseReport` / `...SyncList`). Làm theo đúng mẫu `chkIsCovid` / `IS_COVID`.

### 5.1 Backend — ✅ đã có

Cột `HIS_ICD.IS_INFECTIOUS` **đã ship** (EFMODEL: `HIS_ICD.IS_INFECTIOUS` + `V_HIS_ICD.IS_INFECTIOUS`, kiểu `short?`). SQL gốc:
```sql
ALTER TABLE HIS_ICD ADD (IS_INFECTIOUS NUMBER(1));
COMMENT ON COLUMN HIS_ICD.IS_INFECTIOUS IS 'Là bệnh truyền nhiễm (1=có, null/0=không) - nhận diện ca bệnh truyền nhiễm cho liên thông ECDS';
-- + thêm cột IS_INFECTIOUS vào view V_HIS_ICD
```
Create/Update API **không cần sửa** (map trọn entity).

### 5.2 Frontend `frmHisIcd` — ✅ đã triển khai (đúng mẫu `IS_COVID`)

| # | Vị trí | Đã làm |
|---|--------|--------|
| 1 | **Designer** (`frmHisIcd.Designer.cs`) | Thêm `CheckEdit chkIsInfectious` + `LayoutControlItem layoutControlItem29` (đặt **ngay dưới `chkIsCovid`** trong `layoutControlGroup4`, tại `Location (0,527)` size `(360,24)`); **dịch 2 hàng nút bên dưới +24px** (item6/7/9 → 551, item10/11 → 577, emptySpaceItem1 → 603) và **nới `lcEditorInfo`/`layoutControlGroup4` 601→625**. Thêm cột grid `gridColumn9` FieldName `IS_INFECTIOUS_CHK`, Caption "Bệnh truyền nhiễm", `ColumnEdit=check`, `VisibleIndex=23`. |
| 2 | `SetCaptionByLanguageKey()` | `chkIsInfectious.Properties.Caption` + `.ToolTip` từ resource |
| 3 | `FillDataToEditorControl(data)` | `chkIsInfectious.Checked = (data.IS_INFECTIOUS == 1);` |
| 4 | `ResetFormData()` | `chkIsInfectious.Checked = false;` — **mặc định không tích** |
| 5 | `UpdateDTOFromDataForm(ref currentDTO)` | `currentDTO.IS_INFECTIOUS = chkIsInfectious.Checked ? (short?)1 : null;` |
| 6 | `gridviewFormList_CustomUnboundColumnData` | `else if (e.Column.FieldName == "IS_INFECTIOUS_CHK") e.Value = pData != null && pData.IS_INFECTIOUS == 1;` |
| 7 | `chkIsInfectious_KeyUp` | Space bật/tắt, Enter → `chkValid1Year`; nối tab: `chkIsCovid` Enter → `chkIsInfectious` → `chkValid1Year` |
| 8 | Resources `Lang.vi/en.resx` | `frmHisIcd.chkIsInfectious.Properties.Caption` = "Bệnh truyền nhiễm" / "Infectious disease"; `...ToolTip` |
| 9 | `.csproj` | Gỡ tham chiếu `EmbeddedResource Properties\licenses.licx` (file thiếu sẵn trong repo → chặn build LC0000) |

> **Lưu ý layout:** Designer là bố cục DevExpress theo **vị trí tuyệt đối** (flat group, item side-by-side qua `Location`). Chèn ô mới = đặt tại vị trí trống sau `chkIsCovid` + dịch cụm nút xuống; control tự định vị lại theo `LayoutControlItem` khi load. Cần kiểm tra thực tế không đè lên hàng nút Sửa/Thêm/Làm mới.

### 5.3 Bên tiêu thụ (ECDS)

- **`InfectiousDiseaseReport`** — ✅ đã dùng: `FetchListRows` lọc danh sách chỉ ca có ICD truyền nhiễm qua `BackendDataWorker.Get<V_HIS_ICD>().Where(o => o.IS_INFECTIOUS == 1)` → `HashSet<string>` mã ICD (xem `docs/HIS.Desktop.Plugins.InfectiousDiseaseReport.md §23b.8`).
- **`InfectiousDiseaseSyncList`** — TODO: áp cùng cách lọc danh sách (`SetListFilter`/`LoadListPaging`) khi liệt kê ca cần đẩy.

## 5B. Checkbox "Bệnh mãn tính" — 55058 (28/09/2026)

> Checkbox **"Bệnh mãn tính"** (mặc định KHÔNG tích). Mã ICD được tích → khi BS phòng khám kết thúc điều trị (`HIS.Desktop.Plugins.ExamServiceReqExecute`, nút "Lưu và kết thúc") với ICD chính này mà chưa tích "Mãn tính" thì phần mềm hỏi Yes/No (xem `docs/HIS.Desktop.Plugins.ExamServiceReqExecute.md §7C`).

### 5B.1 Backend
- DB: `ALTER TABLE HIS_ICD ADD (IS_CHRONIC NUMBER(2,0))` + thêm cột vào `V_HIS_ICD` — `docs/SQL_55058_HisIcd_IsChronic.sql`.
- MOS.EFMODEL: `HIS_ICD.IS_CHRONIC`, `V_HIS_ICD.IS_CHRONIC` (`short?`) + EDMX (`DataModelTable.edmx`, `DataModelView.edmx` — SSDL/CSDL/MSL, ngay sau `IS_INFECTIOUS`).
- API Create/Update **không sửa** (`HisIcdUpdate` map trọn entity).

### 5B.2 Frontend `frmHisIcd`
| # | Vị trí | Đã làm |
|---|--------|--------|
| 1 | Designer | `CheckEdit chkIsChronic` + `layoutControlItem30` tại `(0,551)` (dưới `chkIsInfectious`), TabIndex 43; dịch hàng nút +24px (item6/7/9 → 575, item10/11 → 601, emptySpaceItem1 → 627), nới `lcEditorInfo`/`layoutControlGroup4` 625→649. Cột grid `grdColIsChronic` FieldName `IS_CHRONIC_CHK`, `ColumnEdit=check`, VisibleIndex 24 |
| 2 | `frmHisIcd__Chronic.cs` (MỚI) | `IsChronicFieldSupported` (reflection 1 lần), `InitChronicControl` (ẩn ô + cột nếu EFMODEL runtime chưa có cột), `SetCaptionChronic`, `IsChronicIcd`, `FillChronicToEditor`, `UpdateChronicToDTO`, accessor `NoInlining` `GetIsChronic`/`SetIsChronic`, `chkIsChronic_KeyUp` |
| 3 | `frmHisIcd.cs` | Gọi `SetCaptionChronic()` trong `SetCaptionByLanguageKey`, `InitChronicControl()` trong `MeShow`, nhánh `IS_CHRONIC_CHK` trong `CustomUnboundColumnData`, `FillChronicToEditor` / `chkIsChronic.Checked=false` / `UpdateChronicToDTO`; Enter từ `chkIsDeathCauseOnly` → `chkIsChronic` |
| 4 | Resources `Lang.vi/en.resx` | `frmHisIcd.chkIsChronic.Properties.Caption`/`.ToolTip`, `frmHisIcd.grdColIsChronic.Caption`/`.ToolTip` |

## 6. Changelog

| Ngày | Người | Thay đổi |
|------|-------|----------|
| 27/07/2026 | nampp | Tạo tài liệu module + **phân tích thêm checkbox "Bệnh truyền nhiễm"**: cần cột mới `HIS_ICD.IS_INFECTIOUS` (DB + view + EFMODEL) rồi sửa `frmHisIcd` theo mẫu `IS_COVID` (7 điểm §5.2). Dùng cho nhận diện ca bệnh truyền nhiễm ở plugin ECDS. |
| 28/07/2026 | nampp | **ĐÃ TRIỂN KHAI checkbox "Bệnh truyền nhiễm"** (`IS_INFECTIOUS` đã có trong EFMODEL): Designer thêm `chkIsInfectious` + `layoutControlItem29` (dưới COVID, dịch 2 hàng nút +24, nới `lcEditorInfo`/group 601→625) + cột grid `gridColumn9`(`IS_INFECTIOUS_CHK`). `frmHisIcd.cs`: `SetCaptionByLanguageKey`, `CustomUnboundColumnData`(`IS_INFECTIOUS_CHK`), `FillDataToEditorControl` (`data.IS_INFECTIOUS==1`), `ResetFormData` (bỏ tích), `UpdateDTOFromDataForm` (`IS_INFECTIOUS = 1/null`), `chkIsInfectious_KeyUp` + nối tab COVID→Infectious→Valid1Year. Resources `Lang.vi/en` ("Bệnh truyền nhiễm"/"Infectious disease" + tooltip). Gỡ tham chiếu `Properties\licenses.licx` (thiếu file) khỏi csproj. |
| 28/09/2026 | khainq | **55058 — Checkbox "Bệnh mãn tính"** (`IS_CHRONIC`): Designer `chkIsChronic` + `layoutControlItem30` + cột `grdColIsChronic`; partial mới `frmHisIcd__Chronic.cs` (tự ẩn khi EFMODEL runtime chưa có cột); Resources vi/en. Backend: cột `HIS_ICD.IS_CHRONIC` + view + EFMODEL/EDMX. Xem §5B. |

## 7. Test Cases

- [ ] Thêm cột `IS_INFECTIOUS` → EFMODEL có `HIS_ICD.IS_INFECTIOUS` + `V_HIS_ICD.IS_INFECTIOUS`.
- [ ] Mở form ICD → checkbox "Bệnh truyền nhiễm" hiển thị, **mặc định không tích**.
- [ ] Tích + Lưu → `IS_INFECTIOUS=1` xuống DB; mở lại đúng trạng thái.
- [ ] Bỏ tích + Lưu → `IS_INFECTIOUS=null/0`.
- [ ] Plugin ECDS lọc đúng ICD có `IS_INFECTIOUS=1`.
- [ ] **55058** Form ICD → "Bệnh mãn tính" hiển thị, mặc định không tích; tích + Lưu → `IS_CHRONIC=1`, chọn lại dòng đúng trạng thái, cột grid tích; bỏ tích → `null`.
- [ ] **55058** Enter: `chkIsDeathCauseOnly` → `chkIsChronic` → nút Thêm/Sửa; Space bật/tắt.
- [ ] **55058** Runtime MOS.EFMODEL cũ → ô + cột "Bệnh mãn tính" bị ẩn, form hoạt động bình thường.
