# Duyệt khám chuyên khoa — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ApprovalExamSpecialist |
| Loại | Form |
| Mục đích | Bác sĩ chuyên khoa duyệt phiếu mời khám chuyên khoa (HIS_SPECIALIST_EXAM): chọn bác sĩ khám, nhập nội dung khám / y lệnh, chẩn đoán; backend ghi nội dung vào tờ điều trị. "Duyệt và ký" ký số phiếu kết quả khám chuyên khoa (Mps000500); "Ký tờ điều trị" ký số tờ điều trị (Mps000062). |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Plugin nhận `V_HIS_SPECIALIST_EXAM` (phiếu mời), `long` (id) và delegate `Common.RefeshReference` từ form cha.
2. Form load: thông tin khám, tab tổng hợp tờ điều trị / XN / CĐHA / TT / TDCN... của ca điều trị.
3. **"Duyệt (Ctrl S)"** → `SaveSpecialistExam()`: validate bác sĩ + nội dung khám + y lệnh → POST `api/HisSpecialistExam/Update` với `IS_APPROVAL = 1`. Backend ghi nội dung vào tờ điều trị (`EXAM_EXECUTE_TRACKING_ID` nếu bật `MOS.HIS_TRACKING.CREATE_FOR_EXAM_DEPARTMENT`, ngược lại `TRACKING_ID`).
4. **"Duyệt và ký (Ctrl K)"** (việc 57944 — chỉ ký Mps000500):
   - Phiếu chưa duyệt → chạy Duyệt như bước 3; lỗi thì dừng, không ký.
   - Phiếu đã duyệt → bỏ qua Duyệt, chỉ ký (ký lại khi lần trước lỗi/hủy).
   - Ký **phiếu kết quả khám chuyên khoa Mps000500** (`EmrSignNow`). KHÔNG ký tờ điều trị.
   - Nút tạm khóa trong lúc xử lý (chống bấm đúp).
5. **"Ký tờ điều trị"** (việc 57944, nút trên cùng cạnh "Tờ điều trị"): chỉ bật khi phiếu đã duyệt. Xác định tờ điều trị (`EXAM_EXECUTE_TRACKING_ID` → `TRACKING_ID`) → ký **Mps000062** (`EmrSignNow`); tờ đã có văn bản ký thì hỏi xóa văn bản cũ trước khi ký lại. Không xác định được tờ điều trị → cảnh báo.
6. **"In (Ctrl P)"** → in/xem trước Mps000500 (`Show` / `PrintNow` theo `CheDoInChoCacChucNangTrongPhanMem`), không tự ký.
7. "Tờ điều trị" mở `HIS.Desktop.Plugins.TrackingCreate`; "Chi tiết bệnh án" mở `HIS.Desktop.Plugins.EmrDocument` theo `TREATMENT_CODE`.

### Sơ đồ trạng thái IS_APPROVAL
```
NULL / 2 (Chờ duyệt / Từ chối) → 1 (Đã duyệt)   ← Duyệt / Duyệt và ký
```

### Điều kiện nghiệp vụ
- "Duyệt" chỉ bật khi `IS_APPROVAL == null || IS_APPROVAL == 2`; "Tờ điều trị" và "Ký tờ điều trị" chỉ bật khi đã duyệt (`ShowHideBtnSave`).
- "Duyệt và ký" luôn bật (cho phép ký lại).
- Ký lại Mps000500 KHÔNG xóa văn bản ký cũ: Mps000500 không override `ProcessUniqueCodeData()` → `EMR_DOCUMENT.HIS_CODE` rỗng, không lọc được đúng văn bản cũ.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SPECIALIST_EXAM | View | Phiếu mời khám chuyên khoa (input, in Mps000500) |
| HIS_SPECIALIST_EXAM | Table | DTO Update khi duyệt |
| HIS_TREATMENT | Table | Ca điều trị (in) |
| HIS_TRACKING / V_HIS_TRACKING | Table/View | Tờ điều trị cần ký (Mps000062) |
| DHisSereServ2, HIS_SERVICE_REQ | DTO/Table | Dữ liệu các tab dịch vụ |
| V_EMR_DOCUMENT | View | Kiểm tra văn bản ký cũ của tờ điều trị |

## 4. UI Layout

```
+------------------------------------------------------------------+
|                              [Ký tờ điều trị] [Tờ điều trị] [Chi tiết bệnh án] |
| Bác sĩ khám ★ | Thời gian | Chẩn đoán chính/phụ                   |
| Nội dung khám ★ / Y lệnh khám ★                                   |
|                | Tab: Tất cả | Khám bệnh | XN | CĐHA | TT | TDCN... |
| [Duyệt (Ctrl S)] [Duyệt và ký (Ctrl K)] [In (Ctrl P)]             |
+------------------------------------------------------------------+
```

| Control | Loại | Mục đích |
|---------|------|----------|
| btnSave | SimpleButton (Ctrl+S) | Duyệt |
| btnSaveAndSign | SimpleButton (Ctrl+K) | Duyệt và ký phiếu kết quả khám chuyên khoa Mps000500 |
| btnSignTracking | SimpleButton | Ký tờ điều trị Mps000062 (chỉ bật khi đã duyệt) |
| btnPrint | SimpleButton (Ctrl+P) | In phiếu kết quả khám chuyên khoa Mps000500 |
| btnTracking | SimpleButton | Mở tờ điều trị |
| btnChiTietBenhAn | SimpleButton | Mở EMR theo TREATMENT_CODE |
| UCTreeListService | UC nội bộ | Hiển thị các tab dịch vụ |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Duyệt | `api/HisSpecialistExam/Update` | MosConsumer |
| View phiếu (in) | `UriApi.HIS_SPEACIALIST_EXAM_GETVIEW` | MosConsumer |
| Điều trị | `HisRequestUriStore.HIS_TREATMENT_GET` | MosConsumer |
| Tờ điều trị | `HisRequestUriStore.HIS_TRACKING_GET` / `HIS_TRACKING_GETVIEW` | MosConsumer |
| Dịch vụ | `api/HisSereServ/GetDHisSereServ2`, `api/HisServiceReq/Get` | MosConsumer |
| Văn bản EMR | `api/EmrDocument/GetView`, `api/EmrDocument/Delete`, `api/EmrDocument/DownloadFile` | EmrConsumer |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.EmrGenerate | `GenerateInputADOWithPrintTypeCode` — input ký số EMR cho Mps000062 và Mps000500 |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args |
|-------------|-----------|------|
| HIS.Desktop.Plugins.TrackingCreate | Nhấn "Tờ điều trị" | `HIS_TRACKING`, Module |
| HIS.Desktop.Plugins.EmrDocument | Nhấn "Chi tiết bệnh án" | `TREATMENT_CODE` qua `PluginInstanceBehavior.ShowModule` |

## 7. Print

| Loại in | PrintTypeCode | PDO | PreviewType |
|---------|--------------|-----|-------------|
| Tờ điều trị (ký) | Mps000062 | `Mps000062PDO` (partial `frmApprovalExamSpecialist_PrintToDieuTri.cs`) | `EmrSignNow` |
| Phiếu kết quả khám chuyên khoa | Mps000500 | `Mps000500PDO(examItem, treatmentItem)` | In: `Show`/`PrintNow`; Duyệt và ký: `EmrSignNow` |

```
btnPrint_Click          → PrintExamResult(isSign: false)
btnSaveAndSign_Click    → [SaveSpecialistExam() nếu chưa duyệt] → PrintExamResult(isSign: true)
btnSignTracking_Click   → GetTrackingToSign(...) → PrintProcess62(IN_TO_DIEU_TRI)
PrintExamResult(isSign) → RunPrintTemplate("Mps000500", (code, file) => DeletegatePrintTemplate(code, file, isSign))
                        → Inphieuketquakhamchuyenkhoa(..., isSign) → MpsPrinter.Run(PrintData{ EmrInputADO })
```

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| (việc 56271) | — | Thêm "Duyệt và ký": duyệt xong in + ký số tờ điều trị Mps000062 (partial `_PrintToDieuTri.cs`). |
| 02/10/2026 | nampp | **Việc 57944 (NTP)**: "Duyệt và ký" ký thêm **phiếu kết quả khám chuyên khoa Mps000500** sau tờ điều trị (`EmrSignNow`, ký độc lập — tờ điều trị lỗi/hủy hoặc không xác định được vẫn ký Mps000500). Tách `btnPrint_Click` → `PrintExamResult(isSign)`; chế độ ký truyền qua lambda vào `DeletegatePrintTemplate`/`Inphieuketquakhamchuyenkhoa`. Audit `LogActionSuccess("frmApprovalExamSpecialist", "ApproveAndSign.Mps000500", loginName)`. Tooltip nút: "Duyệt, ký tờ điều trị và ký phiếu kết quả khám chuyên khoa". Tạo tài liệu module. |
| 02/10/2026 | nampp | **Việc 57944 — đổi luồng**: "Duyệt và ký" **chỉ ký Mps000500** (bỏ ký tờ điều trị), khóa nút trong lúc xử lý. Thêm nút **"Ký tờ điều trị"** (`btnSignTracking`, hàng trên cạnh "Tờ điều trị", lấy 105px từ `emptySpaceItemChiTietBenhAn`) ký Mps000062, chỉ bật khi đã duyệt. Tooltip "Duyệt và ký": "Duyệt và ký số phiếu kết quả khám chuyên khoa". |

## 9. Test Cases

### Duyệt và ký
- [ ] Phiếu chưa duyệt, nhập đủ → "Duyệt và ký" → duyệt thành công → CHỈ mở màn ký phiếu kết quả Mps000500 (không ký tờ điều trị).
- [ ] Thiếu bác sĩ / nội dung khám → không gọi API, không mở ký.
- [ ] Phiếu đã duyệt → "Duyệt và ký" không gọi Update, chỉ ký Mps000500.
- [ ] Bấm đúp nhanh → chỉ 1 lần gọi Update.

### Ký tờ điều trị
- [ ] Phiếu chưa duyệt → nút "Ký tờ điều trị" bị khóa; duyệt xong → nút bật.
- [ ] Phiếu đã duyệt → "Ký tờ điều trị" → mở màn ký Mps000062 của đúng tờ điều trị chứa nội dung duyệt.
- [ ] Tờ điều trị đã có văn bản ký → hỏi xóa văn bản cũ; No → dừng, Yes → xóa + ký lại.
- [ ] Không xác định được tờ điều trị → cảnh báo.

### In
- [ ] "In (Ctrl P)" vẫn preview/in Mps000500 như cũ, không tự ký.
