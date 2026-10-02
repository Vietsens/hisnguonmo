# Tài liệu kỹ thuật — Việc 57944 (NTP)

**Tích hợp 2 nút "Duyệt" và "In ký số" thành 1 nút "Duyệt và ký" khi duyệt khám chuyên khoa / duyệt hội chẩn**

| Thông tin | Giá trị |
|---|---|
| Khách hàng | NTP |
| Module sửa | `HIS.Desktop.Plugins.ApprovalExamSpecialist` — Duyệt khám chuyên khoa |
| Module sửa | `HIS.Desktop.Plugins.ApprovaleDebate` — Duyệt hội chẩn |
| Module tham chiếu (KHÔNG sửa) | `HIS.Desktop.Plugins.TrackingCreate` — nút "Lưu ký" tờ điều trị |
| Thư viện dùng lại | `HIS.Desktop.Plugins.Library.EmrGenerate`, `Inventec.Common.SignLibrary`, `MPS.MpsPrinter` (`PreviewType.EmrSignNow`) |
| Backend | Không đổi — dùng lại `api/HisSpecialistExam/Update` |

**Lý do:** Nhiều bác sĩ chỉ bấm Duyệt mà chưa/quên ký số → văn bản trên EMR thiếu chữ ký. Cần 1 thao tác: **Duyệt xong → tự mở chức năng ký số**.

---

## 1. Hiện trạng code

### 1.1. Mẫu tham chiếu — TrackingCreate "Lưu ký"

[frmTrackingCreateNew.cs:3784](../HIS/Plugins/HIS.Desktop.Plugins.TrackingCreate/frmTrackingCreateNew.cs#L3784) — `btnSaveSign_Click`:

```
checksign = true
→ Validate (ICD, dxValidationProvider, thời gian tờ điều trị ≥ IN_TIME)
→ POST HIS_TRACKING_CREATE / HIS_TRACKING_UPDATE
→ OK → PrintProcess(PrintType.IN_TO_DIEU_TRI)             // Mps000062
        → EmrGenerateProcessor.GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, roomId)
        → PrintData(..., PreviewType.EmrSignNow) { EmrInputADO = inputADO }
        → MPS.MpsPrinter.Run(PrintData)                      // mở màn ký số
→ checksign = false (cả trong catch)
```

[frmTrackingCreate__Pluss__Print.cs:426-467](../HIS/Plugins/HIS.Desktop.Plugins.TrackingCreate/frmTrackingCreate__Pluss__Print.cs#L426-L467): chọn `PreviewType` theo checkbox:

| Điều kiện | PreviewType |
|---|---|
| Ký + "In văn bản đã ký" | `EmrSignAndPrintNow` |
| Ký + "In" | `PrintNow` (bản in) + `EmrSignNow` (bản ký) |
| Chỉ ký | `EmrSignNow` |

### 1.2. ApprovalExamSpecialist — **ĐÃ CÓ** nút "Duyệt và ký" (việc 56271)

| Thành phần | Vị trí | Trạng thái |
|---|---|---|
| Nút `btnSaveAndSign` "Duyệt và ký (Ctrl K)" | [frmApprovalExamSpecialist.Designer.cs:1525](../HIS/Plugins/HIS.Desktop.Plugins.ApprovalExamSpecialist/Run/frmApprovalExamSpecialist.Designer.cs#L1525) | ✅ Có |
| Handler `btnSaveAndSign_Click` | [frmApprovalExamSpecialist.cs:492](../HIS/Plugins/HIS.Desktop.Plugins.ApprovalExamSpecialist/Run/frmApprovalExamSpecialist.cs#L492) | ✅ Có |
| `SaveSpecialistExam()` trả `bool` (tách từ btnSave) | [frmApprovalExamSpecialist.cs:528](../HIS/Plugins/HIS.Desktop.Plugins.ApprovalExamSpecialist/Run/frmApprovalExamSpecialist.cs#L528) | ✅ Có |
| `GetTrackingToSign(EXAM_EXECUTE_TRACKING_ID, TRACKING_ID)` | [frmApprovalExamSpecialist.cs:600](../HIS/Plugins/HIS.Desktop.Plugins.ApprovalExamSpecialist/Run/frmApprovalExamSpecialist.cs#L600) | ✅ Có |
| Partial in tờ điều trị Mps000062 + `EmrSignNow` + xóa văn bản ký cũ | [frmApprovalExamSpecialist_PrintToDieuTri.cs](../HIS/Plugins/HIS.Desktop.Plugins.ApprovalExamSpecialist/Run/frmApprovalExamSpecialist_PrintToDieuTri.cs) | ✅ Có |
| Phím tắt Ctrl+K | [frmApprovalExamSpecialist.cs:439](../HIS/Plugins/HIS.Desktop.Plugins.ApprovalExamSpecialist/Run/frmApprovalExamSpecialist.cs#L439) | ✅ Có |

**Khoảng trống còn lại (cần sửa nhỏ):**

| # | Vấn đề | Vị trí | Severity |
|---|---|---|---|
| G1 | Ctrl+K gọi `btnSaveAndSign_Click` không kiểm tra `btnSaveAndSign.Enabled` (Ctrl+S thì có) | [:439-443](../HIS/Plugins/HIS.Desktop.Plugins.ApprovalExamSpecialist/Run/frmApprovalExamSpecialist.cs#L439-L443) | MEDIUM |
| G2 | Chuỗi hardcode tiếng Việt: "Không xác định được tờ điều trị…", "Vui lòng kiểm tra lại nội dung khám…", "Tờ điều trị đã tồn tại văn bản ký…" | `:513`, `:544`, `_PrintToDieuTri.cs:451` | LOW |
| G3 | Chưa có tùy chọn "In văn bản đã ký" như TrackingCreate (chỉ `EmrSignNow`) | `_PrintToDieuTri.cs:434` | LOW — xem Q2 |
| G4 | Không chống double-click (`btnSaveAndSign.Enabled = false` trong lúc xử lý) | `:492` | LOW |

### 1.3. ApprovaleDebate — **CHƯA CÓ**

| Thành phần | Hiện trạng |
|---|---|
| Nút Duyệt `btnSave` "Duyệt (Ctrl S)" + `bbtnSave` (BarManager) | Logic save viết thẳng trong `btnSave_Click` ([frmApprovaleDebate.cs:774](../HIS/Plugins/HIS.Desktop.Plugins.ApprovaleDebate/ApprovaleDebate/frmApprovaleDebate.cs#L774)), không trả kết quả |
| Nút `btnPrint` "In phiếu duyệt hội chẩn" — Mps000513, `PreviewType.Show/PrintNow` | Có `EmrInputADO` nhưng **không ký ngay** → BS phải tự bấm ký trong preview |
| Checkbox `chkTaoToDieuTri` (ControlState, mặc định theo `HisConfigCFG.GhiToDieuTriKhiDuyetHoiChan`) | Tích → gửi `CONTENT`/`MEDICAL_INSTRUCTION` → backend ghi vào tờ điều trị (`TRACKING_ID`) |
| Reference `EmrGenerate`, `SignLibrary`, `MPS`, `Mps000513.PDO` | ✅ Có |
| Reference `Mps000062.PDO` | ❌ Không có |

---

## 2. Quyết định nghiệp vụ cần chốt

| # | Câu hỏi | Đề xuất |
|---|---|---|
| **Q1** | Duyệt hội chẩn: "Duyệt và ký" ký **văn bản nào**? | **Ký "Phiếu duyệt hội chẩn" Mps000513** — luôn tồn tại sau duyệt, không phụ thuộc `chkTaoToDieuTri`, không phải kéo ~800 dòng dựng PDO Mps000062 sang plugin hội chẩn. Nếu NTP yêu cầu ký **tờ điều trị** (giống khám CK) → xem phương án B mục 4.6. |
| **Q2** | Có thêm checkbox "In văn bản đã ký" (`EmrSignAndPrintNow`) như TrackingCreate không? | Có, cho cả 2 plugin, lưu ControlState. Mặc định bỏ tích. |
| **Q3** | Phiếu đã duyệt (`IS_APPROVAL = 1`) có cho bấm "Duyệt và ký" để **ký lại** không? | Có — bỏ qua bước Duyệt, chỉ mở ký (khám CK đang làm vậy). Xử lý trường hợp ký lỗi phải ký lại. |
| **Q4** | Phiếu ký rồi, bấm ký lại → xóa văn bản ký cũ? | Giữ như khám CK: hỏi xác nhận → `api/EmrDocument/Delete` bản cũ theo `HIS_CODE` → ký mới. |

> Phần 3–4 dưới đây viết theo các đề xuất trên.

---

## 3. Luồng chung "Duyệt và ký"

```
btnApproveAndSign_Click
 ├─ btnApproveAndSign.Enabled = false                          (chống double-click)
 ├─ IS_APPROVAL == 1 ?
 │    ├─ Có  → bỏ qua Duyệt, dùng lại dữ liệu đã duyệt        (ký lại)
 │    └─ Không → bool ok = ApproveProcess()                    (hàm Duyệt dùng chung với btnSave)
 │              └─ !ok → return                                (KHÔNG ký khi duyệt lỗi)
 ├─ Xác định văn bản ký (null → cảnh báo ResourceMessage, return)
 ├─ Đã có văn bản ký trên EMR? → hỏi Yes/No → Delete bản cũ
 ├─ EmrGenerateProcessor.GenerateInputADOWithPrintTypeCode(TREATMENT_CODE, printTypeCode, RoomId)
 ├─ PreviewType = chkPrintSigned ? EmrSignAndPrintNow : EmrSignNow
 ├─ MPS.MpsPrinter.Run(PrintData { EmrInputADO })              (mở màn ký số của SignLibrary)
 └─ finally: btnApproveAndSign.Enabled = (theo trạng thái phiếu)
```

Quy tắc:
- Duyệt thất bại → **không** mở ký.
- Duyệt thành công nhưng ký bị hủy/lỗi → phiếu **vẫn là đã duyệt** (không rollback); BS bấm lại "Duyệt và ký" để ký (Q3).
- Nút "Duyệt" cũ **giữ nguyên** (cho BS không có chứng thư số).

---

## 4. Thiết kế — `HIS.Desktop.Plugins.ApprovaleDebate`

> **Đã triển khai 30/09/2026** theo phương án A (ký Mps000513), Q2 = có checkbox "In văn bản đã ký", Q3 = cho ký lại.
> Khác thiết kế ban đầu:
> - **Q4 (xóa văn bản ký cũ) KHÔNG làm**: Mps000513 không override `ProcessUniqueCodeData()` nên `EMR_DOCUMENT.HIS_CODE` của phiếu rỗng, không lọc được đúng văn bản cũ → bỏ `ConfirmDeleteSignedDocument` (mục 4.4). Muốn làm phải bổ sung `ProcessUniqueCodeData()` = `"HIS_SPECIALIST_EXAM:{ID}"` ở MPS.Processor.Mps000513 (việc riêng).
> - Chế độ ký truyền qua lambda `(code, file) => DeletegatePrintTemplate(code, file, isSign)` thay cho cờ field `isSignAfterApprove` → không phụ thuộc việc `RunPrintTemplate` chạy đồng bộ hay không.
> - Nút "Duyệt và ký" luôn bật (giống khám CK), chỉ tạm khóa trong lúc xử lý.
> - Vị trí: hàng mới `[☐ In văn bản đã ký] [Duyệt và ký (Ctrl K)]` phía trên hàng nút dưới cùng (cột trái chỉ rộng 360px, hàng dưới đã kín).

### 4.1. UI

Thanh nút dưới ([frmApprovaleDebate.Designer.cs:163-332](../HIS/Plugins/HIS.Desktop.Plugins.ApprovaleDebate/ApprovaleDebate/frmApprovaleDebate.Designer.cs#L163)):

```
[Chi tiết bệnh án] [In phiếu duyệt hội chẩn] ...EmptySpace... [☐ In văn bản đã ký] [Duyệt (Ctrl S)] [Duyệt và ký (Ctrl K)]
```

| Control | Loại | Text (vi) | Ghi chú |
|---|---|---|---|
| `btnApproveAndSign` | SimpleButton | Duyệt và ký (Ctrl K) | ToolTip: "Duyệt và ký số phiếu duyệt hội chẩn". Image: DevExpress gallery (sign/approve) |
| `chkPrintSigned` | CheckEdit | In văn bản đã ký | ControlState |
| `bbtnApproveAndSign` | BarButtonItem | Duyệt và ký | Shortcut `Ctrl+K`, gọi `btnApproveAndSign.PerformClick()` |
| `lciBtnApproveAndSign`, `lciChkPrintSigned` | LayoutControlItem | TextVisible = false | Chèn sau `layoutControlItem4` (btnSave), dùng `InsertType.Right` |

Enable: `btnApproveAndSign.Enabled = btnSave.Enabled || IS_APPROVAL == 1` (chưa duyệt → duyệt+ký; đã duyệt → ký lại). Set tại Load ([:151](../HIS/Plugins/HIS.Desktop.Plugins.ApprovaleDebate/ApprovaleDebate/frmApprovaleDebate.cs#L151)) và sau khi duyệt ([:856](../HIS/Plugins/HIS.Desktop.Plugins.ApprovaleDebate/ApprovaleDebate/frmApprovaleDebate.cs#L856)).

### 4.2. Refactor `btnSave_Click` → `ApproveProcess()`

Tách toàn bộ thân `btnSave_Click` hiện tại ra hàm trả `bool` (giống `SaveSpecialistExam` bên khám CK), **không đổi logic**:

```csharp
private void btnSave_Click(object sender, EventArgs e)
{
    try
    {
        ApproveProcess();
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}

/// <summary>
/// Approve the debate. Returns true when api/HisSpecialistExam/Update returns data.
/// </summary>
private bool ApproveProcess()
{
    bool result = false;
    try
    {
        if (!dxValidationProvider1.Validate())
            return false;
        // ... giữ nguyên code mapping + POST + cập nhật currentHisSpecialistExam + refresh tab ...
        result = (rs != null);
        if (result)
        {
            currentHisSpecialistExam.TRACKING_ID = rs.TRACKING_ID;   // bổ sung
            currentHisSpecialistExam.EXAM_EXECUTE_TRACKING_ID = rs.EXAM_EXECUTE_TRACKING_ID; // bổ sung
            this.btnApproveAndSign.Enabled = true;                   // cho phép ký lại
        }
    }
    catch (Exception ex)
    {
        WaitingManager.Hide();
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
    return result;
}
```

> Lưu ý: code hiện tại chỉ gán lại `currentHisSpecialistExam` khi `delegateRefresh != null` ([:811](../HIS/Plugins/HIS.Desktop.Plugins.ApprovaleDebate/ApprovaleDebate/frmApprovaleDebate.cs#L811)) → mở từ nơi không truyền delegate thì `IS_APPROVAL` không cập nhật. Khi tách hàm, **gán dữ liệu khi `rs != null`**, chỉ gọi `delegateRefresh()` khi khác null.

### 4.3. Nút "Duyệt và ký"

```csharp
private void btnApproveAndSign_Click(object sender, EventArgs e)
{
    try
    {
        if (!btnApproveAndSign.Enabled || this.currentHisSpecialistExam == null) return;
        btnApproveAndSign.Enabled = false;

        if (this.currentHisSpecialistExam.IS_APPROVAL != 1)
        {
            // Sign only after a successful approval.
            if (!ApproveProcess()) return;
        }

        this.isSignAfterApprove = true;
        btnPrint_Click(null, null);   // reuse Mps000513 print, sign mode chosen by isSignAfterApprove
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
    finally
    {
        this.isSignAfterApprove = false;
        btnApproveAndSign.Enabled = (this.currentHisSpecialistExam != null);
    }
}
```

> `RichEditorStore.RunPrintTemplate` gọi delegate **đồng bộ** (khám CK đang dựa vào điều này) → reset cờ trong `finally` là an toàn. Kiểm chứng lại khi code; nếu bất đồng bộ thì chuyển reset cờ vào cuối `InPhieuDuyetHoiChan`.

### 4.4. Sửa `InPhieuDuyetHoiChan` — chọn PreviewType

[frmApprovaleDebate.cs:1508-1523](../HIS/Plugins/HIS.Desktop.Plugins.ApprovaleDebate/ApprovaleDebate/frmApprovaleDebate.cs#L1508-L1523):

```csharp
MPS.ProcessorBase.PrintConfig.PreviewType previewType;
if (this.isSignAfterApprove)
{
    if (!ConfirmDeleteSignedDocument(inputADO)) return;   // Q4
    previewType = chkPrintSigned.Checked
        ? MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow
        : MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
}
else
{
    previewType = ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2
        ? MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow
        : MPS.ProcessorBase.PrintConfig.PreviewType.Show;
}
result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
    printTypeCode, fileName, pdo, previewType, printerName) { EmrInputADO = inputADO });

if (this.isSignAfterApprove && result)
{
    Inventec.Common.Logging.LogUtil.LogActionSuccess("frmApprovaleDebate", "ApproveAndSign", loginName);
}
```

`ConfirmDeleteSignedDocument`: lấy `api/EmrDocument/GetView` (EmrConsumer, `TREATMENT_CODE__EXACT`, `IS_DELETE = false`) → lọc văn bản cùng `HIS_CODE` với `inputADO.HisCode` (xác định chính xác chuỗi HIS_CODE mà `EmrGenerate` sinh cho Mps000513 khi code — **không** hardcode `DOCUMENT_TYPE_ID = 7` như bên khám CK, dùng hằng `IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE` nếu cần) → có thì hỏi Yes/No → `api/EmrDocument/Delete`.

### 4.5. ControlState, phím tắt, đa ngôn ngữ

| Hạng mục | Nội dung |
|---|---|
| ControlState | Dùng lại `controlStateWorker`/`currentControlStateRDO` sẵn có. Key mới `CONTROL_STATE_KEY__PRINT_SIGNED = "chkPrintSigned"`. Đọc trong `InitCheckTaoToDieuTri` (hoặc đổi tên `InitControlState`), lưu trong `chkPrintSigned_CheckedChanged` — check `isNotLoadWhileChangeControlStateInFirst` đầu tiên |
| Phím tắt | `bbtnApproveAndSign` Ctrl+K trong BarManager `bar1` (form là FormBase) |
| Lang.vi/en/my.resx | `frmApprovaleDebate.btnApproveAndSign.Text` = "Duyệt và ký (Ctrl K)" / "Approve and sign (Ctrl K)"; `.btnApproveAndSign.ToolTip`; `frmApprovaleDebate.chkPrintSigned.Properties.Caption` = "In văn bản đã ký" / "Print signed document"; `frmApprovaleDebate.bbtnApproveAndSign.Caption` |
| Message.Lang.vi/en/my.resx | `VanBanDaKyBanCoMuonXoaDeKyLai` = "Phiếu duyệt hội chẩn đã có văn bản ký, tiếp tục sẽ xóa văn bản ký hiện tại. Bạn có muốn tiếp tục?" |
| ResourceMessage.cs | Thêm property tương ứng (pattern mục 13e ui_rules) |
| SetCaptionByLanguageKey | Thêm 4 dòng cho controls mới |

### 4.6. Phương án B (nếu Q1 = ký tờ điều trị)

- Chỉ áp dụng khi `chkTaoToDieuTri.Checked` (không tích → không có nội dung trên tờ điều trị → fallback ký Mps000513 hoặc cảnh báo).
- Copy partial `frmApprovalExamSpecialist_PrintToDieuTri.cs` → `frmApprovaleDebate_PrintToDieuTri.cs` (~1000 dòng dựng PDO Mps000062), `trackingToSign = TRACKING_ID` từ response.
- Thêm reference `MPS.Processor.Mps000062.PDO` (HintPath `histest\x64\ReferencedAssemblies` — bản lib cũ thiếu `_SpecialistExams`, xem csproj khám CK dòng 224-227).
- Rủi ro: nhân bản code lớn → nên đề xuất tách thành Library dùng chung ở việc sau.

---

## 5. Thiết kế — `HIS.Desktop.Plugins.ApprovalExamSpecialist` (chỉnh nhỏ)

Nút đã có từ việc 56271, chỉ vá các khoảng trống mục 1.2:

| # | Sửa | Chi tiết |
|---|---|---|
| G1 | Ctrl+K kiểm tra enable | `if (btnSaveAndSign.Enabled) btnSaveAndSign_Click(null, null);` |
| G2 | Đưa 3 chuỗi hardcode vào `Message.Lang.*.resx` + `ResourceMessage` | `KhongXacDinhDuocToDieuTriDeKy`, `KiemTraLaiNoiDungKhamVaYLenh`, `ToDieuTriDaCoVanBanKyBanCoMuonXoa`; đổi `MessageBox.Show` → `XtraMessageBox.Show` |
| G3 | Checkbox `chkPrintSigned` "In văn bản đã ký" + ControlState | `_PrintToDieuTri.cs:434`: `PreviewType = chkPrintSigned.Checked ? EmrSignAndPrintNow : EmrSignNow` |
| G4 | Chống double-click | `btnSaveAndSign.Enabled = false` đầu hàm, khôi phục trong `finally` |
| — | Audit | `LogUtil.LogActionSuccess("frmApprovalExamSpecialist", "ApproveAndSign", loginName)` khi `MpsPrinter.Run` trả true |

---

## 6. Kịch bản kiểm thử

| # | Plugin | Bước | Kỳ vọng |
|---|---|---|---|
| T1 | Hội chẩn | Phiếu chưa duyệt, nhập đủ → "Duyệt và ký" | Duyệt OK (thông báo), mở màn ký số Mps000513; ký xong → văn bản xuất hiện trong "Chi tiết bệnh án" |
| T2 | Hội chẩn | Bỏ trống ý kiến BS → "Duyệt và ký" | Báo lỗi validate tại control, **không** gọi API, **không** mở ký |
| T3 | Hội chẩn | API Update lỗi | Thông báo lỗi, không mở ký, nút "Duyệt và ký" enable lại |
| T4 | Hội chẩn | Duyệt và ký → hủy ở màn ký | Phiếu vẫn đã duyệt; `btnSave` khóa; "Duyệt và ký" còn enable để ký lại |
| T5 | Hội chẩn | Phiếu đã ký, bấm "Duyệt và ký" lần 2 | Hỏi xóa văn bản cũ; No → dừng; Yes → xóa + ký mới, EMR chỉ còn 1 văn bản |
| T6 | Cả 2 | Tích "In văn bản đã ký" → Duyệt và ký | Ký xong in luôn bản đã ký; đóng/mở form checkbox giữ trạng thái |
| T7 | Cả 2 | Ctrl+K khi nút disable | Không làm gì |
| T8 | Cả 2 | Double-click nhanh nút | Chỉ 1 lần gọi API Update |
| T9 | Hội chẩn | Nút "Duyệt" / "In phiếu duyệt hội chẩn" cũ | Hoạt động như trước (Show/PrintNow, không tự ký) |
| T10 | Hội chẩn | Tích/bỏ tích "Tạo tờ điều trị" rồi Duyệt và ký | Luồng tờ điều trị giữ nguyên như nút Duyệt |
| T11 | Khám CK | Hồi quy việc 56271: Duyệt và ký tờ điều trị | Không đổi hành vi (ngoài G1–G4) |
| T12 | Cả 2 | Đổi ngôn ngữ EN | Caption nút/checkbox/thông báo tiếng Anh |

---

## 7. Danh sách file thay đổi

| Plugin | File | Thay đổi |
|---|---|---|
| ApprovaleDebate | `ApprovaleDebate/frmApprovaleDebate.cs` | Tách `ApproveProcess()`, thêm `btnApproveAndSign_Click`, `chkPrintSigned_CheckedChanged`, `ConfirmDeleteSignedDocument`, sửa `InPhieuDuyetHoiChan`, SetCaption, enable nút |
| ApprovaleDebate | `ApprovaleDebate/frmApprovaleDebate.Designer.cs` | `btnApproveAndSign`, `chkPrintSigned`, `bbtnApproveAndSign`, 2 LayoutControlItem |
| ApprovaleDebate | `Resources/Lang.vi/en/my.resx`, `Message.Lang.vi/en/my.resx`, `ResourceMessage.cs` | Keys mục 4.5 |
| ApprovalExamSpecialist | `Run/frmApprovalExamSpecialist.cs`, `Run/frmApprovalExamSpecialist_PrintToDieuTri.cs`, `.Designer.cs` | G1–G4 |
| ApprovalExamSpecialist | `Resources/*.resx`, `Resources/ResourceMessage.cs` | 3 message + caption `chkPrintSigned` |
| docs | `HIS.Desktop.Plugins.ApprovaleDebate.md`, `HIS.Desktop.Plugins.ApprovalExamSpecialist.md` | Changelog 57944, luồng "Duyệt và ký", test checklist |

Không đổi backend, không đổi EFMODEL, không thêm reference mới (phương án A).

---

## 8. Build & deploy

- Build `.NET 4.5 (v4.5)` x64 cả 2 plugin.
- Deploy DLL + satellite `vi/` (resx mới) vào `E:\IVT TEST\histest\x64`.
- ApprovalExamSpecialist tham chiếu `Mps000062.PDO` từ `histest\x64\ReferencedAssemblies` — giữ nguyên.

---

## 9. Changelog

| Ngày | Người | Nội dung |
|---|---|---|
| 30/09/2026 | nampp | Tạo thiết kế việc 57944 |
| 30/09/2026 | nampp | Triển khai ApprovaleDebate (phương án A, bỏ Q4 — xem ghi chú đầu mục 4). ApprovalExamSpecialist (mục 5) chưa làm. |
| 02/10/2026 | nampp | ApprovalExamSpecialist: "Duyệt và ký" ký thêm phiếu kết quả khám CK **Mps000500** sau tờ điều trị Mps000062 (user chốt "ký cả 2 phiếu"). Ký độc lập; ký lại Mps000500 không xóa văn bản cũ (HIS_CODE rỗng). G1–G4 vẫn chưa làm. |
| 02/10/2026 | nampp | ApprovalExamSpecialist **đổi luồng theo yêu cầu**: "Duyệt và ký" chỉ ký Mps000500 (chống bấm đúp — G4); tờ điều trị Mps000062 ký bằng nút mới **"Ký tờ điều trị"** (chỉ bật khi đã duyệt). |
| 02/10/2026 | nampp | **Trả ApprovalExamSpecialist về nguyên bản** theo yêu cầu (source = `ff6691210`, trước 57944): bỏ Mps000500 trong "Duyệt và ký", bỏ nút "Ký tờ điều trị"; xóa `docs/HIS.Desktop.Plugins.ApprovalExamSpecialist.md` (tạo trong 57944). ApprovaleDebate giữ nguyên. Các thiết kế mục 5 tạm dừng. |
