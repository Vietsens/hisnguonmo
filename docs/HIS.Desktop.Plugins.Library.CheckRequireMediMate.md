# CheckRequireMediMate — Tài Liệu Module (Library)

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.Library.CheckRequireMediMate (library dùng chung, không lên menu) |
| Loại | Library (static class `CheckRequireMediMateManager`) |
| Mục đích | Việc 3353 (PT-56272 / việc 57755): khi **kết thúc thực hiện** dịch vụ đã bật cờ "Có thuốc, vật tư đi kèm" (`HIS_SERVICE.IS_REQUIRE_MEDI_MATE = 1`) mà chưa có thuốc/vật tư đi kèm còn hiệu lực → hỏi *"Dịch vụ chưa có thuốc, vật tư đi kèm … Bạn có muốn tiếp tục kết thúc?"* (Yes/No, mức nhắc, **không chặn**). |
| Người tạo | dangth2 |
| Ngày tạo | 18/09/2026 |
| Trạng thái | Đang phát triển (chờ Backend thêm cột + test) |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính (mỗi màn kết thúc chỉ gọi 1 dòng, đặt SAU các kiểm tra sẵn có và TRƯỚC lời gọi `api/HisServiceReq/Finish|FinishWithTime`)

```
CheckBeforeFinish(danh sách dịch vụ của y lệnh)
  → lọc bỏ IS_NO_EXECUTE = 1, IS_DELETE = 1
  → api/HisService/Get (FilterBase.IDs = SERVICE_ID) → tập dịch vụ có IS_REQUIRE_MEDI_MATE = 1
      → rỗng → trả true, không gọi gì thêm (đa số trường hợp)
  → api/HisSereServ/Get (PARENT_IDs = ID sere_serv bật cờ, TDL_SERVICE_TYPE_IDs = {THUOC, VT})
      → dịch vụ KHÔNG có dòng con nào (đã loại IS_DELETE bởi API, IS_NO_EXECUTE bởi client) = THIẾU
  → có dịch vụ thiếu → XtraMessageBox Yes/No (mặc định No) liệt kê "- Mã - Tên" từng dịch vụ
      → Yes → true (màn gọi Finish như cũ) | No → false (màn return, người dùng kê bổ sung)
```

### Điều kiện nghiệp vụ
- "Đã có thuốc, vật tư đi kèm" = tồn tại dòng `HIS_SERE_SERV` con có `PARENT_ID` = ID dịch vụ, loại Thuốc (6) / Vật tư (7), chưa hủy, không "không thực hiện". Không xét số lượng / chủng loại / định mức. Dòng hao phí (`IS_EXPEND = 1`) vẫn tính là đã có. Máu (14) không tính.
- Hỏi **một lần** cho tất cả dịch vụ thiếu trong một lần kết thúc (kể cả nhiều y lệnh ở màn Trả kết quả tổng hợp).
- Cờ dịch vụ đọc "sống" từ backend (không dùng cache RAM `BackendDataWorker`) để có hiệu lực ngay sau khi sửa danh mục trên mọi máy.
- **Fail-open**: lỗi API / exception → ghi log Warn và trả `true` (không chặn kết thúc).
- Cột `IS_REQUIRE_MEDI_MATE` được đọc qua ADO cục bộ `HisServiceRequireADO` nên thư viện **không phụ thuộc DLL `MOS.EFMODEL` mới**; Backend chưa có cột → JSON không có trường → cờ null → không cảnh báo.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERE_SERV | Table | Dịch vụ đang kết thúc + dòng thuốc/vật tư con (`PARENT_ID`) |
| V_HIS_SERE_SERV, V_HIS_SERE_SERV_5 | View | Overload đầu vào cho các màn dùng view (PTTT dùng view 5) |
| HIS_SERVICE (qua `ADO/HisServiceRequireADO`) | Table | Cờ `IS_REQUIRE_MEDI_MATE` (cột mới, việc 3353) |

### ADO
- `ADO/SereServCheckADO` — kiểu chung: ID, SERVICE_ID, SERVICE_REQ_ID, TDL_SERVICE_CODE, TDL_SERVICE_NAME, IS_NO_EXECUTE, IS_DELETE; `From(HIS_SERE_SERV | V_HIS_SERE_SERV | V_HIS_SERE_SERV_5)`.
- `ADO/HisServiceRequireADO` — ID, SERVICE_CODE, SERVICE_NAME, IS_REQUIRE_MEDI_MATE (tên property trùng tuyệt đối tên cột backend).

## 4. API công khai

| Method | Đầu vào | Trả về |
|--------|---------|--------|
| `CheckBeforeFinish(IEnumerable<HIS_SERE_SERV>)` | dịch vụ của y lệnh (ServiceExecute `listServiceADO`, TestServiceReqExcute `lstSereServ`) | true = được kết thúc |
| `CheckBeforeFinish(IEnumerable<V_HIS_SERE_SERV_5>)` | PTTT `sereServbyServiceReqs` | true = được kết thúc |
| `CheckBeforeFinish(IEnumerable<V_HIS_SERE_SERV>)` | màn dùng view thường | true = được kết thúc |
| `CheckBeforeFinishByServiceReqIds(List<long>)` | ID các y lệnh chưa hoàn thành (Trả kết quả tổng hợp) — thư viện tự lấy sere_serv theo `SERVICE_REQ_IDs` | true = được kết thúc |
| `GetMissingMediMateServices(List<SereServCheckADO>)` | — | danh sách dịch vụ thiếu (không hiện hộp thoại) |
| `ConfirmFinish(List<SereServCheckADO>)` | danh sách thiếu | hiện Yes/No, true = Yes |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Đọc cờ dịch vụ | `HisRequestUriStore.MOSHIS_SERVICE_GET` = api/HisService/Get | MosConsumer | `HisServiceFilter.IDs` (không lọc IS_ACTIVE) |
| Dòng thuốc/VT con | `HisRequestUriStore.MOSHIS_SERE_SERV_GET` = api/HisSereServ/Get | MosConsumer | `HisSereServFilter.PARENT_IDs` + `TDL_SERVICE_TYPE_IDs` |
| Dịch vụ của y lệnh | api/HisSereServ/Get | MosConsumer | `HisSereServFilter.SERVICE_REQ_IDs` |

## 6. Dependencies

### Plugin đang gọi thư viện (giai đoạn 1)
| Plugin | Vị trí chèn |
|--------|-------------|
| HIS.Desktop.Plugins.ServiceExecute | `UCServiceExecute.cs` → `btnFinish_Click`, sau khối kiểm tra "chưa xử lý hết dịch vụ", trước POST `FinishWithTime` |
| HIS.Desktop.Plugins.SurgServiceReqExecute | `SurgServiceReqExecuteControl.cs` → `finishClick()`, sau `CheckLessTime`, trước `if (valid)` (đã `btnSaveClick(true)` trước đó) |
| HIS.Desktop.Plugins.ServiceExecuteGroup | `Run/frmServiceExecuteGroup.cs` → `btnCancel_Click` (nút "Kết thúc (Ctrl E)"), trước `backgroundWorker1.RunWorkerAsync()` |
| HIS.Desktop.Plugins.TestServiceReqExcute | `UCTestServiceReqExcute.cs` → `btnFinish_Click`, sau `ValidTimeReturn`, trước `WaitingManager.Show()` |

Tham chiếu qua HintPath `..\..\..\..\lib\HIS\HIS.Desktop.Plugins.Library.CheckRequireMediMate\HIS.Desktop.Plugins.Library.CheckRequireMediMate.dll`.

### Thư viện phụ thuộc
Inventec.Common.Adapter (BackendAdapter), HIS.Desktop.ApiConsumer (ProjectReference), MOS.EFMODEL, MOS.Filter, IMSys.DbConfig.HIS_RS, Inventec.Common.Resource + Inventec.Desktop.Common.LanguageManager (đa ngôn ngữ), DevExpress.XtraEditors (XtraMessageBox).

## 7. Resources / Đa ngôn ngữ

- `Resources/Message.Lang.resx` — resource **trung tính** (tiếng Việt) nhúng trong DLL chính; `Resources/Message.Lang.en.resx` — satellite `en\...resources.dll`. Thiếu satellite vẫn hiện tiếng Việt (không bị chuỗi rỗng); ngoài ra mỗi chuỗi còn có giá trị mặc định trong `ResourceMessage.cs`.
- Key: `Library_CheckRequireMediMate__DichVuChuaCoThuocVatTuDiKem` ("Dịch vụ chưa có thuốc, vật tư đi kèm:\n{0}\nBạn có muốn tiếp tục kết thúc?"), `Library_CheckRequireMediMate__ThongBao`.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 18/09/2026 | dangth2 | Tạo mới thư viện (việc 3353 / PT-56272). Static `CheckRequireMediMateManager` với 4 overload `CheckBeforeFinish` + `CheckBeforeFinishByServiceReqIds`; 2 request backend (cờ dịch vụ theo IDs, dòng con theo PARENT_IDs + loại Thuốc/VT); hộp thoại Yes/No mặc định No; fail-open. Chèn vào 4 màn kết thúc: ServiceExecute, SurgServiceReqExecute, ServiceExecuteGroup, TestServiceReqExcute. Chờ Backend: cột `HIS_SERVICE.IS_REQUIRE_MEDI_MATE` + view + EFMODEL (màn danh mục `HisService` thêm mục tick "Có thuốc, vật tư đi kèm" sau khi có DLL mới). Thiết kế: `PTTK\3353 - Thiet ke - ...md`. |

## 9. Test Cases

- [ ] Dịch vụ không bật cờ → kết thúc như cũ, không có request `api/HisSereServ/Get` (xem log).
- [ ] Dịch vụ bật cờ, chưa kê → hộp thoại liệt kê đúng "- Mã - Tên", nút mặc định No; No → y lệnh chưa hoàn thành; Yes → hoàn thành.
- [ ] Đã kê 1 thuốc hoặc 1 vật tư (kể cả hao phí) gắn dịch vụ → không cảnh báo.
- [ ] Kê rồi hủy đơn → cảnh báo lại.
- [ ] Y lệnh 2 dịch vụ thiếu / Trả kết quả tổng hợp 2 y lệnh → 1 hộp thoại liệt kê đủ; No → không y lệnh nào bị kết thúc.
- [ ] Dịch vụ tick "Không thực hiện" → bỏ qua.
- [ ] Backend cũ chưa có cột → không cảnh báo, không lỗi; mất mạng khi kiểm tra → vẫn cho kết thúc, log Warn.
- [ ] Đổi ngôn ngữ en → thông báo tiếng Anh (satellite `en`); thiếu satellite → tiếng Việt.
