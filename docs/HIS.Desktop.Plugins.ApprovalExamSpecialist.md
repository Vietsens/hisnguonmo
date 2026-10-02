# Duyệt khám chuyên khoa — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ApprovalExamSpecialist |
| Loại | Form |
| Mục đích | Bác sĩ chuyên khoa duyệt phiếu mời khám chuyên khoa (HIS_SPECIALIST_EXAM): chọn bác sĩ khám, nhập nội dung khám / y lệnh, chẩn đoán; backend ghi nội dung vào tờ điều trị. "Duyệt và ký" ký số tờ điều trị (Mps000062) và/hoặc phiếu kết quả khám chuyên khoa (Mps000500) theo cấu hình. |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Plugin nhận `V_HIS_SPECIALIST_EXAM` (phiếu mời), `long` (id) và delegate `Common.RefeshReference` từ form cha.
2. **"Duyệt (Ctrl S)"** → `SaveSpecialistExam()`: validate bác sĩ + nội dung khám + y lệnh → POST `api/HisSpecialistExam/Update` với `IS_APPROVAL = 1`. Backend ghi nội dung vào tờ điều trị (`EXAM_EXECUTE_TRACKING_ID` nếu bật `MOS.HIS_TRACKING.CREATE_FOR_EXAM_DEPARTMENT`, ngược lại `TRACKING_ID`).
3. **"Duyệt và ký (Ctrl K)"** (việc 56271; cấu hình phiếu ký từ việc 57944):
   - Phiếu chưa duyệt → chạy Duyệt như bước 2; lỗi thì dừng, không ký. Phiếu đã duyệt → chỉ ký (ký lại khi lần trước lỗi/hủy).
   - Phiếu được ký theo cấu hình `HIS.Desktop.Plugins.ApprovalExamSpecialist.ApproveAndSignOption`:

     | Giá trị | Ký |
     |---|---|
     | Rỗng / 1 (mặc định) | Chỉ tờ điều trị Mps000062 — hành vi việc 56271 |
     | 2 | Chỉ phiếu kết quả khám chuyên khoa Mps000500 |
     | 3 | Tờ điều trị Mps000062, rồi phiếu kết quả Mps000500 |

   - Ký tờ điều trị: tờ đã có văn bản ký thì hỏi xóa văn bản cũ trước khi ký lại; không xác định được tờ điều trị → cảnh báo (giá trị 1: dừng; giá trị 3: vẫn ký Mps000500).
   - Tooltip nút đổi theo cấu hình.
4. **"In (Ctrl P)"** → in/xem trước Mps000500 (`Show` / `PrintNow` theo `CheDoInChoCacChucNangTrongPhanMem`), không ký.
5. "Tờ điều trị" mở `HIS.Desktop.Plugins.TrackingCreate`; "Chi tiết bệnh án" mở `HIS.Desktop.Plugins.EmrDocument` theo `TREATMENT_CODE`.

### Sơ đồ trạng thái IS_APPROVAL
```
NULL / 2 (Chờ duyệt / Từ chối) → 1 (Đã duyệt)   ← Duyệt / Duyệt và ký
```

### Điều kiện nghiệp vụ
- "Duyệt" chỉ bật khi `IS_APPROVAL == null || IS_APPROVAL == 2`; "Tờ điều trị" chỉ bật khi đã duyệt; "Duyệt và ký" luôn bật.
- Ký lại Mps000500 KHÔNG xóa văn bản ký cũ: Mps000500 không override `ProcessUniqueCodeData()` → `EMR_DOCUMENT.HIS_CODE` rỗng.

### Cấu hình
| Key | Loại | Giá trị | Script |
|---|---|---|---|
| `HIS.Desktop.Plugins.ApprovalExamSpecialist.ApproveAndSignOption` | HIS_CONFIG | rỗng/1/2/3 (enum `EnumApproveAndSignOption`) | `docs/SQL_57944_ApprovalExamSpecialist_ApproveAndSignOption.sql` |
| `MOS.HIS_TRACKING.CREATE_FOR_EXAM_DEPARTMENT` | HIS_CONFIG (backend) | Ghi nội dung duyệt vào tờ điều trị riêng của khoa khám | — |

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
|                                   [Tờ điều trị] [Chi tiết bệnh án] |
| Ngày khám | Bác sĩ khám ★ | Chẩn đoán chính/phụ                    |
| Nội dung khám ★ / Y lệnh khám ★                                    |
|                 | Tab: Tất cả | Khám bệnh | XN | CĐHA | TT | TDCN... |
| [Duyệt (Ctrl S)] [Duyệt và ký (Ctrl K)] [In (Ctrl P)]              |
+------------------------------------------------------------------+
```

| Control | Loại | Mục đích |
|---------|------|----------|
| btnSave | SimpleButton (Ctrl+S) | Duyệt |
| btnSaveAndSign | SimpleButton (Ctrl+K) | Duyệt và ký phiếu theo cấu hình `ApproveAndSignOption` |
| btnPrint | SimpleButton (Ctrl+P) | In phiếu kết quả khám chuyên khoa Mps000500 |
| btnTracking | SimpleButton | Mở tờ điều trị |
| btnChiTietBenhAn | SimpleButton | Mở EMR theo TREATMENT_CODE |

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
| Phiếu kết quả khám chuyên khoa | Mps000500 | `Mps000500PDO(examItem, treatmentItem)` | In: `Show`/`PrintNow`; Duyệt và ký (cấu hình 2/3): `EmrSignNow` |

```
btnPrint_Click          → PrintExamResult(isSign: false)
btnSaveAndSign_Click    → [SaveSpecialistExam() nếu chưa duyệt]
                        → option 1/3: PrintProcess62(IN_TO_DIEU_TRI)
                        → option 2/3: PrintExamResult(isSign: true)
PrintExamResult(isSign) → RunPrintTemplate("Mps000500", (code, file) => DeletegatePrintTemplate(code, file, isSign))
```

Mỗi mã in có nhiều file mẫu trên SAR (`SAR.Desktop.Plugins.SarPrintType`) thì màn hình chọn biểu mẫu của RichEditor hiện riêng cho mã đó.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| (việc 56271) | — | Thêm "Duyệt và ký": duyệt xong in + ký số tờ điều trị Mps000062. |
| 02/10/2026 | nampp | **Việc 57944**: thêm cấu hình HIS_CONFIG `HIS.Desktop.Plugins.ApprovalExamSpecialist.ApproveAndSignOption` (enum `EnumApproveAndSignOption`, đọc qua `Key.HisConfigCFG.ApproveAndSignOption`) chọn phiếu ký khi "Duyệt và ký": 1 tờ điều trị (mặc định), 2 phiếu kết quả Mps000500, 3 cả hai. `btnPrint_Click` → `PrintExamResult(isSign)`; tooltip nút theo cấu hình; audit `LogActionSuccess("frmApprovalExamSpecialist", "ApproveAndSign.Mps000500", ...)`. Script `docs/SQL_57944_ApprovalExamSpecialist_ApproveAndSignOption.sql`. |

## 9. Test Cases

### Duyệt và ký theo cấu hình
- [ ] Cấu hình rỗng/1 → "Duyệt và ký" chỉ ký tờ điều trị (như cũ); không xác định được tờ điều trị → cảnh báo, dừng.
- [ ] Cấu hình 2 → chỉ mở màn ký Mps000500, không ký tờ điều trị.
- [ ] Cấu hình 3 → ký tờ điều trị rồi ký Mps000500; hủy ký tờ điều trị / không có tờ điều trị → vẫn ký Mps000500.
- [ ] Giá trị lạ (vd 9, chữ) → xử lý như 1.
- [ ] Thiếu bác sĩ / nội dung khám → không gọi API, không mở ký (mọi cấu hình).
- [ ] Phiếu đã duyệt → không gọi Update, chỉ ký theo cấu hình.
- [ ] Tooltip "Duyệt và ký" đúng theo cấu hình.

### In
- [ ] "In (Ctrl P)" vẫn preview/in Mps000500, không ký.
