# Tài liệu kỹ thuật — Việc 55058

**Đánh dấu ICD là bệnh mãn tính + cảnh báo khi BS phòng khám kết thúc điều trị với ICD chính mãn tính mà chưa tích "Mãn tính"**

| Thông tin | Giá trị |
|---|---|
| Module 1 | `HIS.Desktop.Plugins.HisIcd` — Danh mục ICD |
| Module 2 | `HIS.Desktop.Plugins.ExamServiceReqExecute` — Xử lý khám |
| UC liên quan | `HIS.UC.ExamTreatmentFinish` — khung Kết thúc điều trị trong Xử lý khám (thêm 2 public member) |
| Backend | MOS — thêm cột `HIS_ICD.IS_CHRONIC` (DB + view + EFMODEL). **Không** thêm API |
| Tài liệu nghiệp vụ | `55058_A_NghiepVu_CanhBaoIcdManTinh_KetThucDieuTri` |
| Tiền lệ áp dụng | `IS_INFECTIOUS` (28/07/2026 — `docs/HIS.Desktop.Plugins.HisIcd.md §5`), `IS_DEATH_CAUSE_ONLY` (`GetDeathCauseIcdInSelection`) |

---

## 1. Hiện trạng code

| Thành phần | Hiện trạng | Vị trí |
|---|---|---|
| `HIS_ICD` / `V_HIS_ICD` | **Chưa có** cờ mãn tính. Cờ cuối là `IS_INFECTIOUS` | `MOS.EFMODEL/DataModels/HIS_ICD.cs:68`; `lib/MOS/MOS.EFMODEL.dll` |
| `HIS_TREATMENT.IS_CHRONIC` | Đã có (`short?`) | `MOS.EFMODEL/DataModels/HIS_TREATMENT.cs:144` |
| `HisTreatmentFinishSDO.IsChronic` | Đã có (`bool?`) | `MOS.SDO/HisTreatmentFinishSDO.cs:84` |
| API HisIcd Create/Update | Nhận `HIS_ICD` trọn entity → cột mới tự lưu | `HisRequestUriStore.cs` (plugin HisIcd) |
| Form ICD | 14 checkbox cờ, mẫu 5 điểm (Fill/Reset/UpdateDTO/Grid/Lang) | `HisIcd/frmHisIcd.cs` (2358 dòng) |
| Checkbox "Mãn tính" (UC) | `chkChronic` / `lciChronic`, `internal` | `UC/HIS.UC.ExamTreatmentFinish/Run/UCTreatmentFinish.Designer.cs:457-466, 1719-1732, 2192` |
| Ẩn/hiện `chkChronic` | Hiện khi `MOS.HIS_TREATMENT.FINISH.CHRONIC_CHANGE_TREATMENT_TYPE_OPTION = 1`; `ReadOnly` khi hồ sơ đã kết thúc (`IS_PAUSE = 1`) | `Run/UCTreatmentFinish.cs:1327` (`ProcessChronicVisibility`) |
| Tích `chkChronic` | Gọi **ngay** `api/HisTreatment/SetChronic` (sinh/xóa dòng diện điều trị) | `Run/UCTreatmentFinish.cs:1377, 1448` |
| Public API UC | `IsChronicChecked`, event `ChronicRequiredChanged` | `Run/UCTreatmentFinish.cs:1387, 1396` |
| Nút "Lưu và kết thúc" | `btnSaveFinish_Click_Action` → `ProcessExamServiceReqExecute` → `ProcessTreatmentFinish` | `ExamServiceReqExecuteControl.cs:3399, 3491`; `__Process.cs:1733, 2181` |
| ICD chính khi kết thúc | `TreatmentFinishSDO.IcdCode` = `icdADOInTreatment.ICD_CODE` (UC) hoặc fallback `UcIcdGetValue()` | `__Process.cs:2453-2466` |
| Tiền lệ check ICD theo cờ | `GetDeathCauseIcdInSelection()` — HashSet từ `currentIcds` + `ResourceMessage` | `__Process.cs:2237-2247, 2650-2685` |
| Tiền lệ YesNo | `ValidIcdLen` (config "2" → `YesNo, Question`) | `ExamServiceReqExecuteControl.cs:4312-4319` |
| Cache ICD | `this.currentIcds = BackendDataWorker.Get<HIS_ICD>()` (IS_ACTIVE = 1, loại YHCT) | `ExamServiceReqExecuteControl.cs:356` |

**Kết luận:** chỉ `btnSaveFinish` đi qua `ProcessTreatmentFinish` (caller duy nhất `ProcessExamServiceReqExecute` tại L3491) → đặt check trong `ProcessTreatmentFinish` là đúng phạm vi "form kết thúc điều trị ở phòng khám", không ảnh hưởng plugin `TreatmentFinish` độc lập.

---

## 2. Backend (MOS) — thêm cột

### 2.1 SQL

```sql
ALTER TABLE HIS_ICD ADD (IS_CHRONIC NUMBER(1));
COMMENT ON COLUMN HIS_ICD.IS_CHRONIC IS
  'La benh man tinh (1 = co, null/0 = khong). Dung canh bao khi ket thuc dieu tri o phong kham ma chua tich Man tinh';

-- V_HIS_ICD: bo sung cot IS_CHRONIC (CREATE OR REPLACE VIEW ... ICD.IS_CHRONIC ...)
```

> Theo mẫu `IS_INFECTIOUS`: không default, `null` = không mãn tính.

### 2.2 EFMODEL + build

| # | Việc | Ghi chú |
|---|---|---|
| 1 | `HIS_ICD.cs`, `V_HIS_ICD.cs`: thêm `public Nullable<short> IS_CHRONIC { get; set; }` | `E:\svn\IMSys\BACKEND\MOS\MOS.EFMODEL\DataModels\` |
| 2 | Cập nhật mapping EF (nếu có) cho HIS_ICD / V_HIS_ICD | Kiểm tra cơ chế sinh model của MOS |
| 3 | Build `MOS.EFMODEL.dll` → copy vào `FRONTEND/lib/MOS/` | HisIcd tham chiếu `..\..\..\..\..\LIB\MOS\MOS.EFMODEL.dll` |
| 4 | `HisIcdManager` Create/Update | **Không sửa** (map trọn entity) — cần xác nhận `HisIcdUpdate` không copy từng field |
| 5 | Cache `BackendDataWorker` HIS_ICD | Không sửa — client nhận cột mới sau khi reload cache |

---

## 3. Frontend 1 — `HIS.Desktop.Plugins.HisIcd` (theo mẫu `chkIsInfectious`)

Path: `HIS/Plugins/HIS.Desktop.Plugins.HisIcd/HIS.Desktop.Plugins.HisIcd/HisIcd/`

### 3.1 Designer (`frmHisIcd.Designer.cs`)

Bố cục hiện tại (`layoutControlGroup4`, vị trí tuyệt đối):

| Item | Control | Location hiện tại | Sau sửa |
|---|---|---|---|
| layoutControlItem28 | chkIsDeathCauseOnly | (0,503) | giữ |
| layoutControlItem29 | chkIsInfectious | (0,527) | giữ |
| **layoutControlItem30 (mới)** | **chkIsChronic** | — | **(0,551)** size (360,24), Text " ", TextSize (95,20), CustomSize |
| layoutControlItem6/7/9 | btnEdit/btnAdd/btnRefresh | y = 551 | **y = 575** |
| layoutControlItem10/11 | simpleButton1/2 | y = 577 | **y = 601** |
| emptySpaceItem1 | — | (193,603) | **(193,627)** |
| lcEditorInfo / layoutControlGroup4 | — | (360,625) | **(360,649)** |

- `chkIsChronic`: `CheckEdit`, `TabIndex` kế sau `chkIsInfectious`/`chkIsDeathCauseOnly` (43), `KeyUp += chkIsChronic_KeyUp`.
- Cột grid mới `grdColIsChronic`: FieldName `IS_CHRONIC_CHK`, `UnboundType = Object`, `ColumnEdit = this.check`, `AllowEdit = false`, width 110, **VisibleIndex ngay sau `gridColumn9` (IS_INFECTIOUS_CHK)**, trước 4 cột audit.

### 3.2 Code (`frmHisIcd.cs`)

| # | Method | Thêm |
|---|---|---|
| 1 | `SetCaptionByLanguageKey()` (L112–245) | Caption + ToolTip cho `chkIsChronic`, Caption + ToolTip cho `grdColIsChronic` |
| 2 | `FillDataToEditorControl(V_HIS_ICD data)` (L848) | `chkIsChronic.Checked = (data.IS_CHRONIC == 1);` |
| 3 | `ResetFormData()` (L915) | `chkIsChronic.Checked = false;` |
| 4 | `UpdateDTOFromDataForm(ref HIS_ICD currentDTO)` (L1234) | `currentDTO.IS_CHRONIC = chkIsChronic.Checked ? (short?)1 : null;` |
| 5 | `gridviewFormList_CustomUnboundColumnData` (L535) | nhánh `IS_CHRONIC_CHK` |
| 6 | `chkIsChronic_KeyUp` (mới) + nối tab | `chkIsDeathCauseOnly` Enter → `chkIsChronic`; `chkIsChronic` Enter → `btnAdd`/`btnEdit` theo `ActionType` (chuyển logic đang ở `chkIsDeathCauseOnly_KeyUp` L2083) |

```csharp
// CustomUnboundColumnData
else if (e.Column.FieldName == "IS_CHRONIC_CHK")
{
    e.Value = pData != null && pData.IS_CHRONIC == 1;
}
```

```csharp
private void chkIsChronic_KeyUp(object sender, KeyEventArgs e)
{
    try
    {
        if (e.KeyCode == Keys.Space)
        {
            chkIsChronic.Checked = !chkIsChronic.Checked;
        }
        else if (e.KeyCode == Keys.Enter)
        {
            // Same target as the old last checkbox (chkIsDeathCauseOnly)
            if (this.ActionType == GlobalVariables.ActionAdd)
                btnAdd.Focus();
            else
                btnEdit.Focus();
        }
        e.Handled = true;
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}
```

> Đúng mẫu `chkIsDeathCauseOnly_KeyUp` (L2083). Sau đó sửa nhánh Enter của `chkIsDeathCauseOnly_KeyUp` thành `chkIsChronic.Focus();`.

### 3.3 Resources

| Key | vi | en |
|---|---|---|
| `frmHisIcd.chkIsChronic.Properties.Caption` | Bệnh mãn tính | Chronic disease |
| `frmHisIcd.chkIsChronic.ToolTip` | Mã bệnh là bệnh mãn tính. Khi bác sĩ phòng khám kết thúc điều trị với mã bệnh chính này mà chưa tích "Mãn tính", phần mềm sẽ cảnh báo | Chronic disease code. The exam doctor is warned when finishing treatment with this main ICD without ticking "Chronic" |
| `frmHisIcd.grdColIsChronic.Caption` | Bệnh mãn tính | Chronic disease |
| `frmHisIcd.grdColIsChronic.ToolTip` | Là bệnh mãn tính | Is chronic disease |

### 3.4 Import (ngoài phạm vi — chốt C-05)

`btnImport_Click` (L1759) mở plugin riêng `HIS.Desktop.Plugins.HisIcdImport`. Nếu cần import cột "Bệnh mãn tính" → sửa template + map trong plugin đó (việc riêng).

---

## 4. Frontend 2 — `HIS.UC.ExamTreatmentFinish` (thêm 2 public member)

`chkChronic` là `internal` và có 2 trạng thái không nhìn thấy từ plugin (ẩn theo config, ReadOnly khi đã kết thúc). Plugin cần biết **"ô có đang cho tích không"** và cần **focus** vào ô khi BS chọn "Không".

File: `UC/HIS.UC.ExamTreatmentFinish/Run/UCTreatmentFinish.cs` (cạnh `IsChronicChecked` L1387)

```csharp
/// <summary>
/// "Chronic" checkbox is visible and editable (config on, treatment not finished).
/// Parent plugin uses it to decide whether to warn about a chronic main ICD.
/// </summary>
public bool IsChronicEditable
{
    get
    {
        return this.chkChronic != null
            && this.lciChronic.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
            && !this.chkChronic.ReadOnly
            && this.chkChronic.Enabled;
    }
}

/// <summary>
/// Move focus to the "Chronic" checkbox.
/// </summary>
public void FocusChronic()
{
    try
    {
        if (this.chkChronic != null && this.chkChronic.CanFocus)
        {
            this.chkChronic.Focus();
        }
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}
```

**Build/Deploy:** plugin tham chiếu `..\..\..\..\histest\x64\ReferencedAssemblies\HIS.UC.ExamTreatmentFinish.dll` (csproj L184-186) → build UC (net45, x64) → copy DLL vào `ReferencedAssemblies` **trước** khi build plugin.

> **Phương án không sửa UC** (nếu không muốn deploy lại UC): plugin tự đọc `HisConfigs.Get<string>("MOS.HIS_TREATMENT.FINISH.CHRONIC_CHANGE_TREATMENT_TYPE_OPTION") == "1"` và `treatment.IS_PAUSE != 1`. Nhược điểm: lặp logic của UC, không focus được ô Mãn tính. **Khuyến nghị: sửa UC.**

---

## 5. Frontend 3 — `HIS.Desktop.Plugins.ExamServiceReqExecute`

### 5.1 Điểm chèn

Trong `ProcessTreatmentFinish` (`ExamServiceReqExecuteControl__Process.cs:2181`), **cuối khối** `if (treatmentFinish != null && treatmentFinish.TreatmentFinishSDO != null)` — sau khi đã gán `TreatmentFinishSDO.IcdCode` (L2453-2466) và các check YHCT, ngay trước dấu `}` đóng khối (sau đoạn gán `ClinicalSigns/SubclinicalResult/TreatmentMethod` ~L2551):

```csharp
// 55058: main ICD is flagged chronic in HIS_ICD but "Chronic" is not ticked → ask
if (!CheckChronicMainIcd(serviceReqUpdateSDO.TreatmentFinishSDO.IcdCode))
{
    return false;
}
```

Lý do chọn vị trí:
- Dùng đúng **ICD chính cuối cùng gửi backend** (ưu tiên ICD của UC, fallback ICD khám).
- Đứng sau các check chặn cứng hiện có trong `ProcessTreatmentFinish` (nghỉ ốm, nguyên nhân tử vong, bệnh nặng xin về, YHCT) → không hỏi BS rồi lại bị chặn vì lỗi khác.
- `return false` → `ProcessExamServiceReqExecute` trả false → `btnSaveFinish_Click_Action` `return` (L3491), không gọi API.

### 5.2 Method mới (đặt cạnh `GetDeathCauseIcdInSelection`, `__Process.cs` ~L2650)

```csharp
/// <summary>
/// 55058: warn when the main ICD of the exam treatment-finish panel is flagged chronic
/// (HIS_ICD.IS_CHRONIC = 1) but the "Chronic" checkbox is not ticked.
/// Only applies when the checkbox is visible and editable.
/// Returns false when the doctor chooses to go back and tick it.
/// </summary>
private bool CheckChronicMainIcd(string mainIcdCode)
{
    bool valid = true;
    try
    {
        var uc = this.ucTreatmentFinish as UCExamTreatmentFinish;
        if (uc == null || !uc.IsChronicEditable || uc.IsChronicChecked)
            return true;

        if (String.IsNullOrWhiteSpace(mainIcdCode))
            return true;

        string code = mainIcdCode.Trim();
        // currentIcds: active, non-traditional ICDs loaded on form load
        HIS_ICD chronicIcd = this.currentIcds != null
            ? this.currentIcds.FirstOrDefault(o => o.IS_CHRONIC == 1 && o.ICD_CODE == code)
            : null;
        if (chronicIcd == null)
            return true;

        Inventec.Common.Logging.LogSystem.Debug(
            "CheckChronicMainIcd: main ICD is chronic but not ticked"
            + Inventec.Common.Logging.LogUtil.TraceData(
                Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));

        if (XtraMessageBox.Show(
                String.Format(ResourceMessage.IcdChinhLaBenhManTinhChuaTichManTinh,
                    chronicIcd.ICD_CODE + " - " + chronicIcd.ICD_NAME),
                HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                    LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            valid = false;
            this.BeginInvoke(new Action(() => uc.FocusChronic()));
        }
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
    return valid;
}
```

Ghi chú thiết kế:
- **Không O(n²)**: 1 lần `FirstOrDefault` trên `currentIcds` (đã cache khi Load), chỉ chạy khi ô Mãn tính editable & chưa tích.
- Mã ICD khóa (`IS_ACTIVE = 0`) tự bị loại vì `currentIcds` chỉ chứa ICD active.
- Không tự tích `chkChronic` thay BS: tích sẽ gọi API `SetChronic` ngay (sinh dòng diện điều trị) — phải là hành động chủ động của BS.
- Không cần config mới (chốt C-03). Nếu KH muốn bật/tắt / chặn cứng → thêm key `HIS.Desktop.Plugins.ExamServiceReqExecute.WarningChronicIcdOption` ("1" chặn, "2" hỏi) theo mẫu `ValidIcdLen` (L4312).

### 5.3 Resources

`Resources/Message.Lang.vi.resx` / `Message.Lang.en.resx` (+ `Message.Lang.my.resx` nếu duy trì) + property trong `Resources/ResourceMessage.cs` (mẫu `TichManTinhPhaiNhapTruongBatBuoc` L82):

| Key | vi | en |
|---|---|---|
| `IcdChinhLaBenhManTinhChuaTichManTinh` | Mã bệnh chính {0} là bệnh mãn tính nhưng hồ sơ chưa được tích "Mãn tính". Bạn có muốn tiếp tục kết thúc điều trị không? | The main diagnosis {0} is a chronic disease but "Chronic" is not ticked. Do you want to continue finishing the treatment? |

> Lưu ý satellite: deploy DLL lẻ phải kèm `vi\HIS.Desktop.Plugins.ExamServiceReqExecute.resources.dll`, nếu không chuỗi rỗng.

---

## 6. Luồng sau sửa

```
btnSaveFinish_Click_Action (L3399)
  ├─ ...các check cũ (AllowManyTreatmentOpening, GetValue, VerifyTreatmentFinish, ValidIcd...)
  └─ ProcessExamServiceReqExecute (L3491)
       └─ ProcessTreatmentFinish (Process.cs:2181)
            ├─ Nghỉ ốm / dưỡng thai / nguyên nhân tử vong / bệnh nặng xin về (cũ)
            ├─ Gán IcdCode, IcdName, YHCT (cũ)
            ├─ Gán ClinicalSigns / SubclinicalResult / TreatmentMethod (cũ)
            └─ [MỚI] CheckChronicMainIcd(IcdCode)
                 ├─ UC null / ô ẩn-khóa / đã tích / ICD rỗng / ICD không mãn tính → true
                 └─ ICD mãn tính & chưa tích → YesNo
                      ├─ Yes → true (tiếp tục lưu)
                      └─ No  → false + FocusChronic()  → dừng, không gọi API
```

---

## 7. Danh sách file thay đổi

| Repo / module | File | Loại |
|---|---|---|
| MOS (DB) | Script `ALTER TABLE HIS_ICD` + `V_HIS_ICD` | Mới |
| MOS.EFMODEL | `HIS_ICD.cs`, `V_HIS_ICD.cs` | Sửa |
| lib | `lib/MOS/MOS.EFMODEL.dll` | Cập nhật |
| HisIcd | `frmHisIcd.Designer.cs`, `frmHisIcd.cs`, `Resources/Lang.vi.resx`, `Lang.en.resx` | Sửa |
| UC ExamTreatmentFinish | `Run/UCTreatmentFinish.cs` | Sửa (+2 member) |
| ExamServiceReqExecute | `ExamServiceReqExecuteControl__Process.cs`, `Resources/ResourceMessage.cs`, `Message.Lang.vi/en(/my).resx` | Sửa |
| docs | `HIS.Desktop.Plugins.HisIcd.md`, `HIS.Desktop.Plugins.ExamServiceReqExecute.md` (Changelog + section) | Sửa |

**Thứ tự build:** MOS.EFMODEL → copy `lib` → HisIcd; UC ExamTreatmentFinish → copy `histest/x64/ReferencedAssemblies` → ExamServiceReqExecute. Tất cả .NET 4.5 (v4.5) + x64.

---

## 8. Rủi ro / side effects

| # | Rủi ro | Xử lý |
|---|---|---|
| R-01 | Client dùng EFMODEL cũ → không có `IS_CHRONIC` → build lỗi | Deploy EFMODEL trước; cần deploy đồng bộ các plugin dùng `HIS_ICD` nếu bản EFMODEL mới đổi layout (thường chỉ thêm property → tương thích) |
| R-02 | Cache ICD trên máy trạm chưa có cột mới → cảnh báo không xuất hiện | Reload cache HIS_ICD sau khi deploy / sau khi đánh dấu danh mục |
| R-03 | BS chọn "Không" rồi tích Mãn tính → API `SetChronic` chạy ngay, chuyển diện điều trị | Đúng thiết kế hiện hành của ô Mãn tính |
| R-04 | Sau khi tích, `btnSaveFinish` phải bắt buộc nhập CLS + PP điều trị | Hiện `CheckChronicRequiredFields()` chỉ gọi ở `btnSave_Click_Action` (L4071); nhánh `btnSaveFinish` dựa vào validation rule do event `ChronicRequiredChanged` gắn → **kiểm tra lại khi test (TC-10)** |
| R-05 | `HisIcdUpdate` backend copy từng field thay vì trọn entity | Xác minh trong `MOS.MANAGER/HisIcd/Update`; nếu copy từng field → bổ sung `IS_CHRONIC` |
| R-06 | Designer dịch hàng nút +24px có thể đè control | Kiểm tra trên 1366×768 |

---

## 9. Test cases (kỹ thuật)

| # | Kịch bản | Kỳ vọng |
|---|---|---|
| TC-01 | Sau migrate DB | `HIS_ICD.IS_CHRONIC`, `V_HIS_ICD.IS_CHRONIC` tồn tại; GetView trả field |
| TC-02 | HisIcd: thêm mới | `chkIsChronic` không tích; Lưu → `IS_CHRONIC = null` |
| TC-03 | HisIcd: tích + Lưu (sửa) → chọn lại dòng | DB `IS_CHRONIC = 1`; checkbox + cột grid tích |
| TC-04 | HisIcd: bỏ tích + Lưu | `IS_CHRONIC = null` |
| TC-05 | HisIcd: bàn phím Enter từ `chkIsDeathCauseOnly` → `chkIsChronic` → nút Thêm/Sửa; Space bật/tắt | Đúng thứ tự |
| TC-06 | Config mãn tính = 1, ICD chính I10 (`IS_CHRONIC=1`), không tích → Lưu và kết thúc | YesNo cảnh báo |
| TC-07 | TC-06 → Yes | Lưu thành công, `TreatmentFinishSDO.IsChronic = null` |
| TC-08 | TC-06 → No | Không gọi API lưu; focus `chkChronic` |
| TC-09 | Đã tích Mãn tính | Không cảnh báo |
| TC-10 | TC-08 → tích Mãn tính, để trống CLS/PP điều trị → Lưu và kết thúc | Bị chặn yêu cầu nhập (R-04) |
| TC-11 | Chỉ ICD phụ mãn tính | Không cảnh báo |
| TC-12 | Config mãn tính = 0 (ô ẩn) | Không cảnh báo |
| TC-13 | Hồ sơ đã kết thúc (`IS_PAUSE=1`, ô ReadOnly) | Không cảnh báo |
| TC-14 | ICD chính mãn tính nhưng đã khóa (`IS_ACTIVE=0`) | Không cảnh báo |
| TC-15 | Nút "Lưu" (không kết thúc) | Không cảnh báo |
| TC-16 | ICD chính lấy từ UC ≠ ICD khám (UC mãn tính) | Cảnh báo theo ICD của UC |
| TC-17 | Plugin TreatmentFinish độc lập | Không đổi hành vi |
| TC-18 | Ngôn ngữ en | Chuỗi tiếng Anh; deploy thiếu satellite → kiểm tra chuỗi không rỗng |

---

## 10. Cập nhật tài liệu module (khi code)

- `docs/HIS.Desktop.Plugins.HisIcd.md`: thêm `chkIsChronic (IS_CHRONIC)` vào §2, §3; section mới "Checkbox Bệnh mãn tính"; Changelog; Test cases.
- `docs/HIS.Desktop.Plugins.ExamServiceReqExecute.md`: section "Cảnh báo ICD chính mãn tính (55058)", Changelog, Test cases.

---

## 11. Trạng thái triển khai (28/09/2026)

| Hạng mục | Kết quả | Khác thiết kế |
|---|---|---|
| SQL | `docs/SQL_55058_HisIcd_IsChronic.sql` — cột `NUMBER(2,0)` (khớp EDMX `Precision=2` của `IS_INFECTIOUS`) | Chưa chạy DB — cần DBA |
| MOS.EFMODEL (SVN local) | `HIS_ICD.cs`, `V_HIS_ICD.cs` + `DataModelTable.edmx`, `DataModelView.edmx` (SSDL/CSDL/MSL) — build OK | Chưa commit SVN (máy không có svn CLI) |
| R-05 `HisIcdUpdate` | Đã xác minh: không copy từng field → không sửa backend | — |
| HisIcd | Logic đặt trong partial mới `HisIcd/frmHisIcd__Chronic.cs` (file chính đã > 2000 dòng) | `IS_CHRONIC` đọc/ghi qua `PropertyInfo` cache; tự ẩn ô + cột khi EFMODEL runtime chưa có cột |
| UC ExamTreatmentFinish | `IsChronicEditable` (kiểm cả `ReadOnly` và `Properties.ReadOnly`), `FocusChronic()` | — |
| ExamServiceReqExecute | Partial mới `ExamServiceReqExecuteControl__CheckChronicIcd.cs`; gọi cuối khối `chkTreatmentFinish.Checked` trong `ProcessTreatmentFinish` | `FindChronicIcd` tìm theo mã rồi đọc `IS_CHRONIC` qua `PropertyInfo` → EFMODEL cũ thì bỏ qua cảnh báo |
| Build | 3 project build v4.5 AnyCPU, 0 lỗi **với `lib/MOS/MOS.EFMODEL.dll` hiện tại** (không cần EFMODEL mới để biên dịch) | Không tham chiếu trực tiếp `IS_CHRONIC` để không làm gãy build của người khác từ Develop |

**Vì sao cần cơ chế tự tắt:** bản `MOS.EFMODEL.dll` đang chạy (`histest/x64/`) và bản `lib/MOS` lệch nhau (mỗi bản có cột của team khác mà bản kia không có — ví dụ `NURSE_EXECUTE_*`, `PACS_END_TIME` so với `PACS_BEGIN_TIME`), còn source SVN local thiếu cả hai phần đó. Không dựng được một EFMODEL gộp đúng từ máy này, nên **không deploy EFMODEL**. Plugin mới vẫn chạy an toàn trên EFMODEL cũ (tính năng ẩn/tắt) và tự bật khi backend deploy EFMODEL có `IS_CHRONIC` + DB có cột.

**Việc còn lại để tính năng hoạt động thật:**
1. DBA chạy `SQL_55058_HisIcd_IsChronic.sql` (cột + view).
2. Team backend merge thay đổi EDMX/model vào SVN MOS, build + deploy MOS server và phát hành `MOS.EFMODEL.dll` mới (gộp đủ cột các team) cho `lib/MOS` + `histest/x64` + `histest/x64/ReferencedAssemblies`.
3. Reload cache HIS_ICD trên máy trạm.
