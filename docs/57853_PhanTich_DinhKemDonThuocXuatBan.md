# Việc 57853 — Phân tích: Đính kèm đơn thuốc trên phiếu xuất bán

> TTMB - TK - Thêm chức năng đính kèm ảnh đơn thuốc ở màn xuất bán nhà thuốc và danh sách xuất
> Module chính: `HIS.Desktop.Plugins.ExpMestSaleCreate`
> Chức năng tham khảo: `HIS.Desktop.Plugins.HisImportMestMedicine` — mục "Đính kèm file" (việc 42244)
> Ngày phân tích: 29/09/2026

---

## 1. Yêu cầu tóm tắt

| STT | Điều kiện | Xử lý |
|-----|-----------|-------|
| 1 | Lập phiếu xuất bán cho BN mang đơn ngoại viện | Màn xuất bán có chức năng đính kèm: chọn tệp hoặc chụp ảnh đơn |
| 2 | Đơn nhiều trang | Đính kèm nhiều tệp/ảnh trên 1 phiếu, hiển thị dạng danh sách |
| 3 | Tệp sai định dạng / vượt dung lượng | Cảnh báo, không nhận tệp |
| 4 | Xem lại trên danh sách phiếu xuất | Đánh dấu phiếu có đơn đính kèm; mở xem, phóng to, in |
| 5 | Phiếu chưa hoàn tất | Cho bổ sung, thay thế, xóa; ghi nhận người + thời điểm |
| 6 | Phiếu đã hoàn tất / đã thanh toán / đã khóa sổ | Chỉ xem + bổ sung, **không cho xóa** |

Không bắt buộc với mọi phiếu — bán thuốc không kê đơn vẫn lưu bình thường.

---

## 2. Cơ chế tham khảo (việc 42244 — HisImportMestMedicine)

Tài liệu **lưu sang EMR** (EMR_DOCUMENT / EMR_ATTACHMENT, file trên FSS), không lưu ở MOS.

| Thành phần | Chi tiết |
|---|---|
| Điểm vào | Chuột phải phiếu → "Đính kèm file"; gate `MOS.HIS_IMP_MEST.ALLOW_ATTACH_FILE = 1` + `MOS.HAS_CONNECTION_EMR = 1` |
| `frmImpMestAttachList` | Danh sách tài liệu của phiếu: Xem / Xóa / Đính kèm mới |
| `frmImpMestAttachFile` (1550 dòng) | Chọn file (jpg/png/bmp/gif/pdf, multi-select), chụp ảnh (`HIS.Desktop.Plugins.Camera`), scan, xoay ảnh. **Lưu = gộp tất cả thành 1 PDF = 1 EMR_DOCUMENT** |
| Khóa liên kết | `HisCode = "{MaSite} IMP_MEST_CODE:.. DOCUMENT_NUMBER:.."`, `TreatmentCode = IMP_MEST_CODE`, `IsOutsideTreatment = true`, loại văn bản `IMPAT` |
| API | `api/EmrDocument/CreateByTdo`, `api/EmrDocument/GetView` (HIS_CODE__EXACT + DOCUMENT_TYPE_ID + IS_DELETE=false), `api/EmrDocument/DownloadFile`, `api/EmrDocument/Delete` (xóa mềm) |
| Audit | `CREATOR/CREATE_TIME/MODIFIER/MODIFY_TIME` của EMR_DOCUMENT — không cần làm thêm |

**Khoảng trống so với yêu cầu 57853:**

| Yêu cầu | Bản tham khảo |
|---|---|
| Kiểm tra dung lượng tệp (mục 3) | ❌ Chưa có |
| Chặn xóa theo trạng thái phiếu (mục 6) | ❌ Chưa có — mọi người dùng đều xóa được |
| Đánh dấu phiếu có đính kèm trên lưới (mục 4) | ❌ Chưa có |
| Đính kèm trước khi lưu phiếu (kịch bản 1) | ❌ Phiếu nhập luôn đã tồn tại khi đính kèm |

---

## 3. Phạm vi ảnh hưởng

| Module | Vai trò | Ghi chú |
|---|---|---|
| `HIS.Desktop.Plugins.ExpMestSaleCreate` | Nút "Đính kèm đơn" trên màn xuất bán | Hiện **chưa reference** EMR.* / itextsharp / PdfViewer |
| `HIS.Desktop.Plugins.HisExportMestMedicine` ("Danh sách xuất") | Cột icon đánh dấu + mở danh sách đính kèm | **Màn danh sách duy nhất thực sự mở ExpMestSaleCreate** (sửa phiếu). Đã có reference EMR. Grid `V_HIS_EXP_MEST_2`, có paging |
| `HIS.Desktop.Plugins.HisSaleExpMestList` ("Danh sách xuất bán") | Tùy chọn — chỉ đánh dấu + xem/bổ sung | Không mở màn xuất bán. **Cần chốt** |
| `HIS.Desktop.Plugins.ExpMestSaleCreateV2` | Danh sách xuất mở V2 khi `EXP_MEST_SALE__MODULE_UPDATE_OPTION_SELECT = 1` | `UCHisExportMestMedicine_Event.cs:459` — viện dùng V2 sẽ không có chức năng nếu chỉ làm V1 |
| DB EMR | Thêm `EMR_DOCUMENT_TYPE` (đề xuất `EXPSA` — "Đơn thuốc đính kèm phiếu xuất bán") | Chỉ script, không đổi schema |
| DB HIS config | Key bật/tắt, dung lượng tối đa | Script insert |

---

## 4. Thiết kế đề xuất

### 4.1 Tách plugin dùng chung — không clone form lần 3

Form đính kèm đã có 2 bản copy (`EmrDocument.frmAttackFile` → `frmImpMestAttachFile`). Đề xuất tạo **`HIS.Desktop.Plugins.ExpMestAttachFile`** chứa `frmAttachList` + `frmAttachFile`; màn xuất bán và các màn danh sách gọi qua `CallModule` với ADO:

```csharp
class ExpMestAttachFileADO
{
    public HIS_EXP_MEST ExpMest { get; set; }            // null = chưa lưu phiếu (chế độ buffer)
    public bool AllowDelete { get; set; }                // theo trạng thái phiếu (4.4)
    public List<AttackADO> PendingFiles { get; set; }    // file đang giữ tạm trước khi lưu
    public DelegateSelectData ActionAfterChange { get; set; }
}
```

### 4.2 Khóa liên kết tài liệu ↔ phiếu

| Trường DocumentTDO | Giá trị | Lý do |
|---|---|---|
| `TreatmentCode` | `TDL_TREATMENT_CODE` (vãng lai → `EXP_MEST_CODE`) — *cập nhật 02/10/2026* | `EmrDocumentViewFilter` có **`TREATMENT_CODEs`** → đánh dấu cả trang lưới bằng **1 API** |
| `HisCode` | `"{MaSite} EXP_MEST_CODE:{code} SERVICE_REQ_CODE:{mã đơn}|EXP_MEST_CODE:{code}|EXP_STOCK:{kho}|REQ_DEPT:{khoa}"` | EMR tách khối sau `|` vào cột `EMR_DOCUMENT.EXP_MEST_CODE` → tài liệu gắn đúng phiếu xuất |
| `DocumentTypeId` | `EXPSA` (khóa ReadOnly) | Lọc chọn lọc, tránh quét toàn EMR_DOCUMENT (bài học fix hiệu năng 42244) |
| `IsOutsideTreatment` | `true` | Đơn ngoại viện, không thuộc hồ sơ điều trị |
| `FileType` | `PDF` | Gộp nhiều ảnh/trang thành 1 PDF như bản tham khảo |

> ⚠ Bán theo đơn có mã điều trị HIS thật mà gán `TreatmentCode = EXP_MEST_CODE` → tài liệu **không hiện trong bệnh án EMR** của BN. Phù hợp mục đích "bằng chứng tại nhà thuốc" nhưng cần BA xác nhận.

### 4.3 Luồng trên màn xuất bán

Chưa lưu thì chưa có `EXP_MEST_CODE` → **giữ file tạm trong RAM (buffer)**:

```
[Chưa lưu]  Đính kèm đơn → frmAttachFile (chọn/chụp/xóa từng ảnh trong buffer)
              → trả List<AttackADO> về UC; nút hiển thị "Đính kèm đơn (2)"
[Lưu OK]    với mỗi ExpMestSdos → CreateByTdo(buffer)
              → OK   : clear buffer
              → Lỗi  : cảnh báo "Phiếu đã lưu, đính kèm đơn thất bại" — giữ buffer để thử lại,
                       KHÔNG rollback phiếu; có thể bổ sung sau từ danh sách
[Lưu lỗi]   giữ nguyên buffer
[Mới/Đơn mới] clear buffer (hỏi xác nhận nếu buffer đang có file)
[Đã có phiếu] (sau lưu / mở sửa từ danh sách) → Đính kèm đơn mở thẳng frmAttachList của phiếu
```

**Đơn vị xóa/thay:**
- Trước khi lưu: xóa **từng ảnh** trong buffer.
- Sau khi lưu: mỗi lần đính kèm = 1 PDF gộp → xóa **theo lần đính kèm**. "Thay thế" = Xóa + Đính kèm mới (giống 42244 v1.3).

### 4.4 Quyền xóa theo trạng thái phiếu (mục 5, 6)

| Trạng thái | Trường kiểm tra | Xem / Bổ sung | Xóa |
|---|---|---|---|
| Chưa hoàn tất | Không thỏa các điều kiện dưới | ✔ | ✔ |
| Đã hoàn tất | `EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE` | ✔ | ✖ |
| Đã thanh toán | `BILL_ID.HasValue` (có tính `DEBT_ID`? — cần chốt) | ✔ | ✖ |
| Đã khóa sổ | `MEDI_STOCK_PERIOD_ID.HasValue` (đã chốt kỳ kho) | ✔ | ✖ |

- Không được xóa → ẩn/disable nút Xóa; nếu thao tác → thông báo theo kịch bản 4 (khai báo `Message.Lang.vi/en.resx`).
- ⚠ **Chặn chỉ ở frontend**: `api/EmrDocument/Delete` không biết trạng thái phiếu → vẫn xóa được qua màn "Danh sách văn bản" EMR. Chặn triệt để cần backend.

### 4.5 Kiểm tra định dạng / dung lượng (mục 3, kịch bản 2)

- Kiểm tra **ngay lúc chọn file**, trước khi đọc base64/Image:
  - Phần mở rộng ∈ danh sách cho phép (giữ như tham khảo: jpg, jpeg, png, bmp, gif, pdf).
  - `FileInfo.Length ≤ MaxSizeMB`.
  - Sai → cảnh báo tên tệp + lý do, bỏ tệp đó; các tệp hợp lệ vẫn nhận.
- Ảnh chụp từ Camera cũng kiểm tra dung lượng (thường nhỏ — khớp kịch bản "chụp lại").
- Config đề xuất: `HIS.Desktop.Plugins.ExpMestSaleCreate.AttachFile.MaxSizeMB` (mặc định 5).

### 4.6 Đánh dấu trên danh sách xuất (mục 4, kịch bản 3)

- Thêm cột icon `ATTACH_DISPLAY` (📎) vào nhóm cột icon — **chỉ phiếu bán** (`EXP_MEST_TYPE_ID == ID__BAN`).
- Sau mỗi lần load trang:
  1. Lấy `EXP_MEST_CODE` các phiếu bán trên trang.
  2. Gọi **1 lần** `api/EmrDocument/GetView` với `TREATMENT_CODEs` + `DOCUMENT_TYPE_ID = EXPSA` + `IS_DELETE = false`.
  3. Dựng `HashSet<string>` → `CustomUnboundColumnData` chỉ tra HashSet (O(1), không gọi API/LINQ trong grid).
- Click icon / chuột phải "Đơn đính kèm" → mở `frmAttachList`:
  - **Xem**: `DownloadFile` → `SignLibraryGUIProcessor.ShowPopup` (phóng to, in) như 42244.
  - **Bổ sung**: luôn cho phép.
  - **Xóa**: theo bảng 4.4.
- Lưới đính kèm có 4 cột audit: Thời gian đính kèm · Người đính kèm · Thời gian sửa · Người sửa.
- Ghi `LogAction` cho thao tác đính kèm / xóa (yêu cầu "ghi nhận mọi lần thao tác").

---

## 5. Trường hợp biên

| # | Tình huống | Đề xuất |
|---|---|---|
| 1 | Viện không kết nối EMR (`MOS.HAS_CONNECTION_EMR ≠ 1`) | Ẩn toàn bộ chức năng. **Rủi ro lớn nhất** — nhà thuốc độc lập có thể không có EMR |
| 2 | Bán nhiều BN (`api/HisExpMest/SaleCreateBillList`) | Bản đầu: disable nút đính kèm; hoặc đính kèm theo từng BN |
| 3 | 1 lần lưu sinh nhiều `ExpMestSdos` | Đính kèm cùng buffer cho mỗi phiếu hoặc chỉ phiếu đầu — cần chốt |
| 4 | PDF nhiều trang / ảnh lớn | Gộp PDF chậm → `WaitingManager` (tham khảo đã có) |
| 5 | Đóng tab khi buffer còn file | Hỏi xác nhận trong `ReleaseBeforeClose` |

---

## 6. Script DB

File: `docs/SQL_57853_ExpMestSaleAttachPrescription.sql`

| # | Schema | Nội dung |
|---|---|---|
| 1 | EMR | `EMR_DOCUMENT_TYPE` mã `EXPSA` — "Đơn thuốc đính kèm phiếu xuất bán", `IS_ALLOW_DUPLICATE_HIS_CODE = 1` |
| 2 | HIS | `HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.IsEnable` = 0 (mặc định tắt) |
| 3 | HIS | `HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.MaxFileSizeMB` = 5 |

Chức năng chỉ hoạt động khi đồng thời `MOS.HAS_CONNECTION_EMR = 1`. `CONFIG_CODE` trong script là tạm, DBA thay bằng mã kế tiếp.

---

## 7. Câu hỏi cần chốt (bản code đầu tiên đã áp dụng cột "Đề xuất")

| # | Câu hỏi | Đề xuất |
|---|---|---|
| 1 | Màn danh sách: `HisExportMestMedicine`, `HisSaleExpMestList`, hay cả hai? | Chắc chắn `HisExportMestMedicine`; `HisSaleExpMestList` tùy chọn |
| 2 | `ExpMestSaleCreateV2` có làm cùng không? | Nên làm nếu có viện đang dùng V2 |
| 3 | Dung lượng tối đa (MB), định dạng cho phép? | 5 MB; jpg/jpeg/png/bmp/gif/pdf |
| 4 | "Đã thanh toán" có tính phiếu xác nhận nợ (`DEBT_ID`)? | Có |
| 5 | Bán nhiều BN / 1 lần lưu nhiều phiếu xử lý thế nào? | Disable ở bản đầu / đính kèm cho mọi phiếu |
| 6 | Có yêu cầu backend chặn xóa không? | Chấp nhận FE ở bản đầu, ghi nhận rủi ro |
| 7 | Tách plugin dùng chung `ExpMestAttachFile` hay clone form như 42244? | Tách plugin dùng chung |
