# Duyệt hội chẩn — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ApprovaleDebate |
| Loại | Form |
| Mục đích | Form duyệt/từ chối phiếu mời hội chẩn (HIS_SPECIALIST_EXAM). Bác sĩ chuyên khoa nhập ý kiến hội chẩn, chọn người duyệt, chẩn đoán; có thể mở chi tiết bệnh án EMR và in phiếu duyệt hội chẩn. |
| Trạng thái | Đang bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Plugin nhận `V_HIS_SPECIALIST_EXAM` (phiếu mời hội chẩn) + delegate refresh từ form cha.
2. Form load: hiển thị bác sĩ hội chẩn (multi-select), chẩn đoán chính/phụ, ý kiến BS hội chẩn (nhập nhiều dòng), kèm các tab tổng hợp tờ điều trị / CĐHA / XN / Thuốc-VT-Máu / Siêu âm-Nội soi / PTTT / GPB của ca điều trị.
3. Nhấn "Duyệt (Ctrl+S)": validate người duyệt + ý kiến → POST `api/HisSpecialistExam/Update` với `IS_APPROVAL = 1`, `EXAM_EXECUTE_CONTENT = ý kiến`, `REJECT_APPROVAL_REASON = null`. **Không** tạo tờ điều trị (GP-HC6).
4. Nhấn "Chi tiết bệnh án": mở popup `HIS.Desktop.Plugins.EmrDocument` theo `TREATMENT_CODE` hiện tại.
5. Nhấn "In phiếu duyệt hội chẩn": load lại treatment + view exam → build `Mps000500PDO` → preview/in qua `MPS.MpsPrinter`.

### Sơ đồ trạng thái IS_APPROVAL
```
NULL / 2 (Chờ duyệt) → 1 (Đã duyệt)        ← nhấn Duyệt
                    → (từ chối)             ← qua form từ chối riêng
```

### Điều kiện nghiệp vụ
- Nút "Duyệt" chỉ enable khi `IS_APPROVAL == null || IS_APPROVAL == 2`.
- Ý kiến BS hội chẩn bắt buộc, max 4000 ký tự.
- Bác sĩ hội chẩn bắt buộc chọn ít nhất 1 (multi-select, lọc theo `EXAM_EXECUTE_DEPARMENT_ID`).
- GP-HC6: KHÔNG tạo tờ điều trị khi duyệt — frontend bỏ truyền `CONTENT`/`MEDICAL_INSTRUCTION` trong DTO Update.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SPECIALIST_EXAM | View | Phiếu mời hội chẩn (input) |
| HIS_SPECIALIST_EXAM | Table | DTO Update |
| HIS_TREATMENT | Table | Ca điều trị (dùng khi in) |
| HIS_TRACKING | Table | Tờ điều trị (tab "Tờ điều trị") |
| DHisSereServ2 | DTO | Dữ liệu chỉ định gom theo loại DV (tab CĐHA, XN, Thuốc, PTTT, GPB...) |
| HIS_SERVICE_REQ | Table | Yêu cầu DV trong tab |
| HIS_EMPLOYEE | Table | Combo "Bác sĩ hội chẩn" |
| HIS_ICD | Table | Combo chẩn đoán chính + nhập chẩn đoán phụ |

## 4. UI Layout

```
+--------------------------------------------------------------+
| Bác sĩ hội chẩn  [GridLookUp multi-select đỏ ★]              |
| Cđ chính         [txt][cboICD_YHCT]                           |
| Cđ phụ           [txt][txt tên — F1 chọn]                     |
| Ý kiến BS hội chẩn ★                                          |
| [MemoEdit nhiều dòng, mở rộng dọc đến gần cuối form]          |
|                                                                |
|                                                                |
|              | Tab: Tờ điều trị | CĐHA | XN | Thuốc/VT/Máu... |
|              | [Tree/Grid dữ liệu chỉ định]                    |
|                                                                |
| [Chi tiết bệnh án] [In phiếu duyệt hội chẩn] [Duyệt (Ctrl S)]|
+--------------------------------------------------------------+
```

### Controls chính
| Control | Loại | Mục đích |
|---------|------|----------|
| cboEmployee | GridLookUpEdit multi-select | Bác sĩ hội chẩn |
| txtICD_YHCT + cboICD_YHCT | TextEdit + CustomGridLookUpEdit | Chẩn đoán chính |
| txtICDsub + txtICDsubName | 2 TextEdit | Chẩn đoán phụ (F1 mở popup HIS.Desktop.Plugins.SecondaryIcd) |
| txtYKienBacSi | MemoEdit | Ý kiến bác sĩ hội chẩn (nhiều dòng, 4000 ký tự) |
| btnSave | SimpleButton (Ctrl+S) | Duyệt |
| btnChiTietBenhAn | SimpleButton | Mở popup EMR theo TREATMENT_CODE |
| btnPrint | SimpleButton | In phiếu duyệt hội chẩn (Mps000500) |
| UCTreeListTracking / UCTreeListService | UC nội bộ | Hiển thị các tab nghiệp vụ |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Duyệt / Update phiếu | `api/HisSpecialistExam/Update` | MosConsumer |
| Load view phiếu để in | `api/HisSpecialistExam/GetView` | MosConsumer |
| Load điều trị | `HisRequestUriStore.HIS_TREATMENT_GET` (`api/HisTreatment/Get`) | MosConsumer |
| Load tracking (tab Tờ điều trị) | `HisRequestUriStore.HIS_TRACKING_GET` | MosConsumer |
| Load DHisSereServ2 | `api/HisSereServ/GetDHisSereServ2` | MosConsumer |
| Load HisServiceReq | `api/HisServiceReq/Get` | MosConsumer |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.EmrGenerate | `EmrGenerateProcessor.GenerateInputADOWithPrintTypeCode(...)` — tạo input ký số EMR khi in |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args |
|-------------|-----------|------|
| `HIS.Desktop.Plugins.SecondaryIcd` | F1 trên `txtICDsubName` | `HIS.Desktop.ADO.SecondaryIcdADO(callback, codes, names)` |
| `HIS.Desktop.Plugins.EmrDocument` | Nhấn "Chi tiết bệnh án" | `List<object> { TREATMENT_CODE }` qua `PluginInstanceBehavior.ShowModule` |

### MPS / Print
| Mã | Mục đích | PDO |
|----|----------|-----|
| Mps000500 | In phiếu duyệt hội chẩn / phiếu khám chuyên khoa | `MPS.Processor.Mps000500.PDO.Mps000500PDO(examItem, treatmentItem)` |

## 7. Print

| Loại in | PrintTypeCode | Library/MPS | Template server |
|---------|--------------|-------------|-----------------|
| Phiếu duyệt hội chẩn | Mps000500 | `MPS.MpsPrinter.Run` + `MPS.ProcessorBase.Core.PrintData` | SarConsumer / `ConfigSystems.URI_API_SAR` |

Flow:
```
btnPrint_Click
 → RichEditorStore.RunPrintTemplate("Mps000500", DeletegatePrintTemplate)
   → InPhieuDuyetHoiChan: load HIS_TREATMENT + V_HIS_SPECIALIST_EXAM
   → Mps000500PDO(examItem, treatmentItem)
   → EmrGenerateProcessor.GenerateInputADOWithPrintTypeCode → InputADO ký số
   → MpsPrinter.Run(PrintData{ EmrInputADO = inputADO }, PreviewType = Show | PrintNow)
```

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 21/05/2026 | phuongnm | **B.4.3 Duyệt hội chẩn (Sửa đổi)**: <br>- Xóa trường UI "Diễn biến" (`txtDienBien`) và "PP xử lý" (`txtPPXuLy`) cùng layout items tương ứng.<br>- Mở rộng vùng "Ý kiến bác sĩ hội chẩn" (`txtYKienBacSi`) chiếm vùng đã giải phóng, caption đổi thành "Ý kiến bác sĩ hội chẩn:" (Maroon, bắt buộc).<br>- Thêm nút **"Chi tiết bệnh án"** mở popup `HIS.Desktop.Plugins.EmrDocument` theo `TREATMENT_CODE` (giống pattern ApprovalExamAnesthesia / ApprovalExamSpecialist).<br>- Thêm nút **"In phiếu duyệt hội chẩn"** sử dụng `Mps000500` qua `MPS.MpsPrinter.Run` + `EmrGenerateProcessor` ký số.<br>- **GP-HC6**: Bỏ set `datamapper.CONTENT` và `datamapper.MEDICAL_INSTRUCTION` trong save → không tạo tờ điều trị khi duyệt; bỏ refresh `existingData[0].CONTENT/MEDICAL_INSTRUCTION` ở tab "Tờ điều trị".<br>- Bỏ validate `ValidateNull(txtDienBien)` + `ValidateMaxLength(txtPPXuLy)` trong `ValidContent()`.<br>- Cập nhật Lang.vi / Lang.en / Lang.my: bỏ keys `layoutControlItem9.Text` (Diễn biến) + `layoutControlItem10.Text` (PP xử lý); thêm keys `btnChiTietBenhAn.Text`, `btnPrint.Text`.<br>- Thêm references csproj: `Inventec.Common.RichEditor`, `Inventec.Common.SignLibrary`, `MPS`, `MPS.Processor.Mps000500.PDO`, `MPS.ProcessorBase`, ProjectReference `HIS.Desktop.LocalStorage.ConfigApplication`, `HIS.Desktop.LocalStorage.ConfigSystem`, `HIS.Desktop.Plugins.Library.EmrGenerate`. |

## 9. Test Cases

### Duyệt
- [ ] Mở form từ danh sách phiếu hội chẩn chưa duyệt → btnSave enabled.
- [ ] Chưa chọn bác sĩ → nhấn Duyệt → hiện warning "Trường dữ liệu bắt buộc".
- [ ] Nhập ý kiến > 4000 ký tự → warning maxLength.
- [ ] Nhập đủ → Duyệt → API Update trả về thành công → grid cha refresh, btnSave disable.
- [ ] Sau khi duyệt, mở lại phiếu đã duyệt → btnSave disable.

### Chi tiết bệnh án
- [ ] `TREATMENT_CODE` rỗng → nút không làm gì (return sớm).
- [ ] `TREATMENT_CODE` có → popup `EmrDocument` mở, hiển thị tài liệu của ca điều trị.

### In phiếu duyệt hội chẩn
- [ ] Nhấn In → load `HIS_TREATMENT` + `V_HIS_SPECIALIST_EXAM` → preview Mps000500 hiện đúng thông tin bác sĩ, chẩn đoán, ý kiến.
- [ ] `ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2` → in trực tiếp không preview.
- [ ] Có cấu hình ký số EMR → input ADO ký số được build qua `EmrGenerateProcessor`.

### GP-HC6 — không tạo tờ điều trị
- [ ] Sau khi Duyệt, kiểm tra `HIS_TRACKING` của ca điều trị: KHÔNG có record mới được tạo từ frontend (frontend không truyền `CONTENT`/`MEDICAL_INSTRUCTION` nữa).
- [ ] Tab "Tờ điều trị" của form: nội dung dòng đầu KHÔNG bị overwrite bằng giá trị form (chỉ ICD được cập nhật).
