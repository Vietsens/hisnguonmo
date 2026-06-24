# Danh Sách Biên Bản Hội Chẩn (Debate) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.Debate |
| Loại | Form (Common, group A) |
| Mục đích | Form danh sách + quản lý + in + ký số biên bản hội chẩn. Cho phép xem các biên bản hội chẩn theo bộ lọc (Tôi tạo / Tôi được mời), in 4 loại biểu mẫu (Trích biên bản, Biên bản dấu sao, Sổ biên bản, Hội chẩn PTTT), và xem bản đã ký từ EMR. |
| Trạng thái | Bảo trì |
| Liên hệ | Plugin tạo/sửa biên bản: `HIS.Desktop.Plugins.DebateDiagnostic` |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. User vào module → form hiện grid danh sách biên bản (`gridViewDebateReq`) lọc theo Từ ngày / Đến ngày / Khoa / Tôi tạo / Tôi được mời.
2. Chọn 1 hoặc nhiều biên bản → bật `btnPrintDebate` ("In sổ biên bản hội chẩn") + `btnPrintDebateSigned` ("In biên bản hội chẩn đã ký").
3. Nhấn **`btnPrintDebate`** → popup menu 4 mục:
   - Trích biên bản hội chẩn (MPS000019)
   - Biên bản hội chẩn thuốc dấu sao (MPS000323)
   - Sổ biên bản hội chẩn (MPS000020) — hỗ trợ in nhóm
   - Biên bản hội chẩn trước phẫu thuật (Mps000387)
4. Nhấn **`btnPrintDebateSigned`** → lấy file PDF đã hoàn tất ký từ `api/EmrDocument/GetView` (filter `DOCUMENT_TYPE_ID=DEBATE, HAS_REJECTER=false, HAS_NEXT_SIGNER=false`) → đối chiếu `HIS_CODE` chứa token `HIS_DEBATE:{debateId}` qua helper `IsHisCodeOfDebate` → ghép PDF → viewer.
5. Khi **`chkAutoSign`** tích (visibility theo `HisConfigCFG.IsUseSignEmr`): build `SignerConfigDTO` từ `HIS_DEBATE_USER` (chủ tọa = NumOrder 100, thư ký = 2, user hiện tại = 1, BS khác = 3++) → set `EmrInputADO.SignerConfigs`. MPS engine dùng để chuyển sang luồng ký số EMR.

### B.4.5 — Chuẩn hoá HisCode (2026-05-22)
Tất cả 5 process print method (`InTrichBienBanHoiChanProcess`, `InTrichBienBanHoiChanThuocDauSaoProcess`, `InSoBienBanHoiChanProcess`, `InSoBienBanHoiChanProcessGroup`, `InHoiChanPtttProcess`) đều set:

```csharp
inputADO.HisCode = string.Format("DEBATE_ID:{0} PRINT_TYPE_CODE:{1}", debateID, printTypeCode);
```

→ Backend MOS `EmrIntegrate/DocumentStatusChange` parse `HIS_CODE` để cập nhật trạng thái biên bản khi EMR notify document status thay đổi (qua config `EMR.INTERGRATE.API_ADDRESS.NOTIFY_DOCUMENT_STATUS`).

> **Lưu ý quan trọng — phân biệt 2 loại HisCode:**
> - `inputADO.HisCode` (frontend set ở trên) = `"DEBATE_ID:{id} PRINT_TYPE_CODE:{code}"` — dùng cho luồng `EmrIntegrate/DocumentStatusChange`.
> - `V_EMR_DOCUMENT.HIS_CODE` (HIS_CODE THỰC TẾ lưu của tài liệu) = do MPS processor sinh trong `ProcessUniqueCodeData()`, dạng `"Mps0000XX TREATMENT_CODE:{code} HIS_DEBATE:{debateId}"` (Mps000019 = Trích, Mps000020 = Sổ). **Mọi logic đối chiếu tài liệu ký với biên bản PHẢI dùng token `HIS_DEBATE:{id}`** — KHÔNG dùng `DEBATE_ID:` hay `Mps000020`.

### 45729 — Bộ lọc "Chỉ hiển thị biên bản đã ký" + Xuất Excel

- **`chkOnlySigned`** ("Chỉ hiển thị biên bản đã ký", gần nhóm Trạng thái, mặc định tắt): khi tích → `FilterSignedDebates()` đối chiếu danh sách `V_HIS_DEBATE` với tài liệu ký số EMR, **chỉ giữ biên bản đã ký ĐẦY ĐỦ** (`HAS_NEXT_SIGNER=false` + `HAS_REJECTER=false`); biên bản chưa ký xong/đang dở bị ẩn. Chỉ gọi `api/EmrDocument/GetView` khi bộ lọc bật. Match từng biên bản qua `IsHisCodeOfDebate(HIS_CODE, debate.ID)` (token `HIS_DEBATE:{id}`).
- Đối chiếu theo lô `TREATMENT_CODEs` (100 mã/lần). Kết hợp đồng thời với lọc khoa + thời gian + tiêu chí khác.
- **`btnExportExcel`** ("Xuất Excel"): xuất đúng danh sách đang hiển thị (sau khi áp mọi bộ lọc) ra .xlsx; ẩn các cột thao tác (Xem/Sửa/Xóa/In) khi xuất; danh sách trống → cảnh báo.
- Hiện tại bộ lọc áp **ngay khi tích** (`chkOnlySigned_CheckedChanged` → `LoadGridDebate`); spec mô tả áp "khi bấm Tìm" — chấp nhận chạy ngay cho tiện thao tác.

### Sơ đồ luồng ký số EMR

```
Client (Debate plugin)
   → inputADO.HisCode = "DEBATE_ID:{id} PRINT_TYPE_CODE:{code}"
   → inputADO.SignerConfigs (khi chkAutoSign tích)
   → MPS.MpsPrinter.Run(PrintData { EmrInputADO = inputADO })
        ↓
   Backend MOS — INSERT EMR_DOCUMENT (HIS_CODE, ...)
        ↓
   EMR system — user ký, trạng thái thay đổi
        ↓
   EMR đọc config EMR.INTERGRATE.API_ADDRESS.NOTIFY_DOCUMENT_STATUS
        ↓
   EMR POST → MOS api/EmrIntegrate/DocumentStatusChange
        ↓
   MOS parse HIS_CODE → "DEBATE_ID:123" → UPDATE HIS_DEBATE SET ... WHERE ID=123
```

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_DEBATE | Table | Biên bản hội chẩn |
| V_HIS_DEBATE | View | Danh sách hiển thị grid (có TREATMENT_CODE, ICD_NAME, ...) |
| HIS_DEBATE_USER | Table | Thành phần tham gia (chủ tọa, thư ký, BS) — dùng cho ProcessSigner |
| V_HIS_DEBATE_EKIP_USER | View | Kíp PTTT (cho Mps000387) |
| V_HIS_TREATMENT | View | Thông tin điều trị (`treatmentToPrint`) |
| V_HIS_DEPARTMENT_TRAN | View | Lịch sử chuyển khoa (`departmentTran`) |
| V_HIS_TREATMENT_BED_ROOM | View | Giường bệnh nhân |
| V_HIS_PATIENT_TYPE_ALTER | View | Thẻ BHYT hiện tại |
| V_EMR_DOCUMENT | View | File đã ký từ EMR (cho `btnPrintDebateSigned`) |

## 4. UI Layout

```
+--- frmDebate (Form) -----------------------------------------------+
| Filter row:  [Từ] [Đến] [Khoa]   ☑Tất cả ☑Tôi tạo ☑Tôi được mời  |
|              ☐ Chỉ hiển thị biên bản đã ký  (chkOnlySigned)        |
+-------------------------------------------------------------------+
| Grid (gridViewDebateReq) — V_HIS_DEBATE                          |
|  Xem | Mã ĐT | Tên BN | Mã BN | Địa điểm | Khoa | ICD | ...      |
+-------------------------------------------------------------------+
| [Tự động thiết lập ký theo thành phần tham gia: ☑]                |
| [Xuất Excel] [In biên bản hội chẩn đã ký] [In sổ biên bản ▼]      |
+-------------------------------------------------------------------+
```

## 5. API Endpoints

| Action | URI | Consumer | Filter/DTO |
|--------|-----|----------|------------|
| Lấy danh sách V_HIS_DEBATE | `api/HisDebate/GetView` | MosConsumer | `HisDebateViewFilter` |
| Lấy chi tiết HIS_DEBATE | `api/HisDebate/Get` | MosConsumer | `HisDebateViewFilter` |
| Lấy HIS_DEBATE_USER | `api/HisDebateUser/Get` | MosConsumer | `HisDebateUserFilter` |
| Lấy V_HIS_DEBATE_EKIP_USER | `api/HisDebateEkipUser/GetView` | MosConsumer | `HisDebateEkipUserViewFilter` |
| Lấy file EMR đã ký | `api/EmrDocument/GetView` | EmrConsumer | `EmrDocumentViewFilter` (DOCUMENT_TYPE_ID=DEBATE, HAS_REJECTER=false, HAS_NEXT_SIGNER=false) |
| EMR notify status change | `api/EmrIntegrate/DocumentStatusChange` (MOS gọi từ EMR) | — | Parse `HIS_CODE` để cập nhật |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| `HIS.Desktop.Plugins.Library.EmrGenerate` | Tạo `Inventec.Common.SignLibrary.ADO.InputADO` cho ký số EMR |
| `Inventec.Common.RichEditor.RichEditorStore` | Load template MPS từ SAR server |
| `MPS.MpsPrinter` | Engine in + tích hợp EMR (PreviewType.ShowDialog/PrintNow) |
| `iTextSharp.text.pdf.PdfConcatenate` | Ghép nhiều PDF khi xem bản đã ký |

### Configs
| Config Key | Mục đích |
|------------|----------|
| `HIS.HIS.DESKTOP.IS_USE_SIGN_EMR` | Bật/ẩn `chkAutoSign` UI |
| `EMR.INTERGRATE.API_ADDRESS.NOTIFY_DOCUMENT_STATUS` | (EMR config, không nằm trong plugin) — EMR gọi callback về MOS khi status đổi |

## 7. Print

| Loại | PrintTypeCode | PDO | Note |
|------|--------------|-----|------|
| Trích biên bản hội chẩn | `Mps000019` | `MPS.Processor.Mps000019.PDO.Mps000019PDO` | 7-arg ctor (patient, V_HIS_DEBATE, departmentTran, single, debateUser, treatment, patyAlter) |
| Biên bản hội chẩn dấu sao | `Mps000323` | `MPS.Processor.Mps000323.PDO.Mps000323PDO` | 5-arg ctor |
| Sổ biên bản hội chẩn | `Mps000020` | `MPS.Processor.Mps000020.PDO.Mps000020PDO` | Hỗ trợ in group qua `FlexCelPrintProcessor.SetPartialFile` |
| Hội chẩn trước phẫu thuật | `Mps000387` | `MPS.Processor.Mps000387.PDO.Mps000387PDO` | Có ekip user |

### HisCode chuẩn (B.4.5)
Format `inputADO.HisCode`: `"DEBATE_ID:{currentHisDebate.ID hoặc currentVDebate.ID hoặc debate.ID} PRINT_TYPE_CODE:{printTypeCode}"`

Áp dụng trong cả 5 process method ngay sau `GenerateInputADOWithPrintTypeCode(...)`, TRƯỚC khi gán `SignerConfigs`.

> ⚠️ Đây là `inputADO.HisCode` (cho luồng `EmrIntegrate/DocumentStatusChange`), **KHÁC** với `V_EMR_DOCUMENT.HIS_CODE` thực tế của tài liệu — cái sau do MPS `ProcessUniqueCodeData()` sinh, dạng `"Mps0000XX TREATMENT_CODE:{code} HIS_DEBATE:{id}"`. Khi đối chiếu tài liệu ký với biên bản (bộ lọc đã ký, nút in đã ký) PHẢI dùng token `HIS_DEBATE:{id}`. Xem mục 2.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 22/05/2026 | phuongnm | **B.4.5 — Chuẩn hoá HisCode khi in biên bản hội chẩn**: Set `inputADO.HisCode = "DEBATE_ID:{id} PRINT_TYPE_CODE:{code}"` trong cả 5 print process method (InTrichBienBanHoiChanProcess MPS000019, InTrichBienBanHoiChanThuocDauSaoProcess MPS000323, InSoBienBanHoiChanProcess + InSoBienBanHoiChanProcessGroup MPS000020, InHoiChanPtttProcess Mps000387). Backend MOS `EmrIntegrate/DocumentStatusChange` parse HIS_CODE này để cập nhật trạng thái biên bản khi EMR notify. Đồng thời sửa 3 HintPath sai trong csproj: `Inventec.Common.WebApiClient` (LIB\MPS → LIB\Inventec.Common), `Inventec.Desktop.Common.LocalStorage.Location` (LIB\HIS\ReferencedAssemblies → LIB\Inventec.Desktop), `MPS.Processor.Mps000019.PDO` (LIB\MPSv2\MPS.PDO → histest\x64\ReferencedAssemblies — đồng bộ version 7-arg ctor). |
| 19/06/2026 | phuongnm | **45729 — Sửa lỗi bộ lọc "Chỉ hiển thị biên bản đã ký" không hiện biên bản đã ký đủ**. Root cause: HIS_CODE lưu trong `V_EMR_DOCUMENT` do MPS processor sinh ra (`ProcessUniqueCodeData`) có dạng `"Mps0000XX TREATMENT_CODE:{code} HIS_DEBATE:{debateId}"` (vd `Mps000019 TREATMENT_CODE:000026000892 HIS_DEBATE:4386`) — **KHÔNG phải** `"DEBATE_ID:.. PRINT_TYPE_CODE:Mps000020"` như `inputADO.HisCode` của frontend (giá trị đó dùng cho EmrIntegrate, không phải HIS_CODE của document). `FilterSignedDebates` và `btnPrintDebateSigned_Click` đang match `HIS_CODE.Contains("Mps000020")` → tài liệu ký thực tế là Trích biên bản (Mps000019) nên KHÔNG bao giờ khớp. **Fix**: thêm helper `IsHisCodeOfDebate(hisCode, debateId)` match theo token `HIS_DEBATE:{id}` (có kiểm tra biên giới chữ số, tránh 4386 khớp 43860), bỏ điều kiện `Mps000020`; áp dụng cho cả bộ lọc và nút in. Giữ `HAS_NEXT_SIGNER=false`+`HAS_REJECTER=false` (chỉ hiện biên bản ký ĐỦ). |

## 9. Test Cases

### Print + HisCode
- [ ] Mở module Debate → chọn 1 biên bản → nhấn `btnPrintDebate` → "Trích biên bản hội chẩn" → preview hiện đúng nội dung.
- [ ] Verify `LogSystem.txt` có log `TraceData` cho `inputADO` chứa `"HisCode":"DEBATE_ID:{id} PRINT_TYPE_CODE:Mps000019"`.
- [ ] Verify DB: `SELECT HIS_CODE FROM V_EMR_DOCUMENT WHERE ID = ...` — value trùng format `DEBATE_ID:{id} PRINT_TYPE_CODE:Mps000019`.
- [ ] Lặp lại với 3 loại còn lại: Mps000323, Mps000020, Mps000387. HIS_CODE đúng format tương ứng.
- [ ] Nhấn `btnPrintDebate` với nhiều biên bản chọn → in group MPS000020 → mỗi document có HIS_CODE riêng theo `debate.ID` của row.

### Ký số EMR
- [ ] Config `HIS.HIS.DESKTOP.IS_USE_SIGN_EMR = 1` → `lciChkAutoSign` hiện. Tích `chkAutoSign` → tiến hành in → MPS trigger luồng ký số EMR.
- [ ] Sau khi ký xong → user khác xem `btnPrintDebateSigned` → load file PDF đã ký từ EMR. Verify match qua `IsHisCodeOfDebate` (token `HIS_DEBATE:{id}`).
- [ ] EMR notify document status change → MOS `EmrIntegrate/DocumentStatusChange` parse HIS_CODE → update HIS_DEBATE row tương ứng.

### Bộ lọc "Chỉ hiển thị biên bản đã ký" (45729)
- [ ] Biên bản **ký đủ** mọi thành phần (NEXT_SIGNER null, REJECTER null) → tích `chkOnlySigned` → biên bản **hiện**.
- [ ] Biên bản mới ký 1 phần (còn NEXT_SIGNER) → tích lọc → biên bản **bị ẩn**.
- [ ] Biên bản chưa ký / chưa có tài liệu EMR → tích lọc → **bị ẩn**.
- [ ] Bỏ tích `chkOnlySigned` → hiện lại toàn bộ theo lọc khoa + thời gian.
- [ ] `IsHisCodeOfDebate`: debate ID `4386` KHÔNG khớp tài liệu `HIS_DEBATE:43860` (kiểm tra biên giới chữ số).
- [ ] `btnExportExcel`: xuất .xlsx đúng số dòng đang hiển thị, ẩn cột Xem/Sửa/Xóa/In; danh sách trống → cảnh báo.

### Regression
- [ ] `btnPrintDebateSigned`: chọn biên bản đã ký đủ → in gộp PDF đúng các biên bản (match qua token `HIS_DEBATE:{id}`, không phụ thuộc `Mps000020`).
- [ ] In khi `chkAutoSign` KHÔNG tích → in bình thường, không trigger ký, HIS_CODE vẫn được set đúng (tạo EMR document mặc dù chưa ký).
