# CheckRequireMediMate — Tài Liệu Module (Library)

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.Library.CheckRequireMediMate (library dùng chung, không lên menu) |
| Loại | Library (static class `CheckRequireMediMateManager`) |
| Mục đích | Việc 3353 (PT-56272 / việc 57755): khi **kết thúc thực hiện** dịch vụ đã bật cờ "Có thuốc, vật tư đi kèm" (`HIS_SERVICE.IS_REQUIRE_MEDI_MATE = 1`) mà chưa có thuốc/vật tư đi kèm còn hiệu lực → **CHẶN**, hiện thông báo *"Không kết thúc được. Dịch vụ chưa có thuốc, vật tư đi kèm … Vui lòng kê thuốc, vật tư đi kèm cho các dịch vụ trên rồi kết thúc lại."* — hộp thông báo **chỉ có nút OK** (không còn Yes/No, không có đường bỏ qua), y lệnh **không được kết thúc**; chỉ riêng lỗi kỹ thuật mới cho qua (fail-open, xem mục 2). |
| Người tạo | dangth2 |
| Ngày tạo | 18/09/2026 |
| Trạng thái | Đã lên test 21/09 ở mức cảnh báo (HISTEST f748c8958); **22/09 đổi sang mức CHẶN** theo chỉ đạo anh Cảnh, đã deploy lại (HISTEST 39fe57142); cùng ngày 22/09 rà lại và **bổ sung màn thứ 5 `TestServiceExecute`** (commit `359ff8013`, DLL HISTEST 052c55c03); **25/09 vá đường lọt ô KT ở màn PTTT + bổ sung màn thứ 6 `SurgServiceReqExecute2`** (DLL HISTEST `a13c6cb21`; ô tick danh mục có lại từ HISTEST `37e858d23`) → đang áp dụng đủ **6 màn**, chờ test lại |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính (mỗi màn kết thúc chỉ chèn đúng một lời gọi, đặt SAU các kiểm tra sẵn có và TRƯỚC lời gọi `api/HisServiceReq/Finish|FinishWithTime`)

```
CheckBeforeFinish(danh sách dịch vụ của y lệnh)
  → lọc bỏ IS_NO_EXECUTE = 1, IS_DELETE = 1
  → api/HisService/Get (FilterBase.IDs = SERVICE_ID) → tập dịch vụ có IS_REQUIRE_MEDI_MATE = 1
      → rỗng → trả true, không gọi gì thêm (đa số trường hợp)
  → api/HisSereServ/Get (PARENT_IDs = ID sere_serv bật cờ, TDL_SERVICE_TYPE_IDs = {THUOC, VT})
      → dịch vụ KHÔNG có dòng con nào (đã loại IS_DELETE bởi API, IS_NO_EXECUTE bởi client) = THIẾU
  → có dịch vụ thiếu → XtraMessageBox CHỈ CÓ NÚT OK, liệt kê "- Mã - Tên" từng dịch vụ
      → ConfirmFinish LUÔN trả false → màn `return`, KHÔNG gọi Finish
      → đường thoát duy nhất: kê bổ sung thuốc/vật tư đi kèm cho các dịch vụ bị liệt kê rồi kết thúc lại
```

### Điều kiện nghiệp vụ
- "Đã có thuốc, vật tư đi kèm" = tồn tại dòng `HIS_SERE_SERV` con có `PARENT_ID` = ID dịch vụ, loại Thuốc (6) / Vật tư (7), chưa hủy, không "không thực hiện". Không xét số lượng / chủng loại / định mức. Dòng hao phí (`IS_EXPEND = 1`) vẫn tính là đã có. Máu (14) không tính.
- Báo **một lần** liệt kê đủ tất cả dịch vụ thiếu trong một lần kết thúc (kể cả nhiều y lệnh ở màn Trả kết quả tổng hợp); chặn cả lần kết thúc đó, không y lệnh nào được hoàn thành.
- Cờ dịch vụ đọc "sống" từ backend (không dùng cache RAM `BackendDataWorker`) để có hiệu lực ngay sau khi sửa danh mục trên mọi máy.
- **Fail-open**: lỗi API / exception (mất mạng, backend cũ chưa có cột) → ghi log Warn và trả `true` (cho qua), để sự cố kỹ thuật không khóa việc kết thúc dịch vụ của viện.
- Cột `IS_REQUIRE_MEDI_MATE` được đọc qua ADO cục bộ `HisServiceRequireADO` nên thư viện **không phụ thuộc DLL `MOS.EFMODEL` mới**; Backend chưa có cột → JSON không có trường → cờ null → không dịch vụ nào bị chặn.
- Phạm vi chặn là **3 nhóm CLS + PTTT + Xét nghiệm** (6 màn ở mục 6). Các màn kết thúc y lệnh ngoài 3 nhóm này **cố ý không gọi thư viện** (xem mục 6).
- **Kết thúc không chỉ đi qua `Finish|FinishWithTime`** (bài học 25/09): hai màn PTTT còn kết thúc qua `api/HisServiceReq/SurgUpdate(List)` với `IsFinished = true` khi ô **KT** được tick (backend đặt y lệnh sang Hoàn thành). Ở đường này màn gọi thư viện ngay trước khi đặt `IsFinished = true`; bị chặn thì gửi `IsFinished = false`: dữ liệu vẫn lưu, y lệnh không hoàn thành.

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

`CheckBeforeFinish` có **4 overload** (3 overload theo kiểu EFMODEL + 1 overload lõi theo ADO).

| Method | Đầu vào | Trả về |
|--------|---------|--------|
| `CheckBeforeFinish(IEnumerable<HIS_SERE_SERV>)` | dịch vụ của y lệnh (ServiceExecute và TestServiceExecute `listServiceADO` — `ServiceADO : HIS_SERE_SERV`; TestServiceReqExcute `lstSereServ`) | true = được kết thúc; **false = BỊ CHẶN** (màn phải `return`) |
| `CheckBeforeFinish(IEnumerable<V_HIS_SERE_SERV_5>)` | PTTT `sereServbyServiceReqs` (rỗng thì lấy `sereServ` đang xử lý) | như trên |
| `CheckBeforeFinish(IEnumerable<V_HIS_SERE_SERV>)` | màn dùng view thường | như trên |
| `CheckBeforeFinish(List<SereServCheckADO>)` | overload **lõi** — 3 overload trên đều quy về đây qua `SereServCheckADO.From(...)` | như trên |
| `CheckBeforeFinishByServiceReqIds(List<long>)` | ID các y lệnh chưa hoàn thành (Trả kết quả tổng hợp) — thư viện tự lấy sere_serv theo `SERVICE_REQ_IDs` | như trên |
| `GetMissingMediMateServices(List<SereServCheckADO>)` | dịch vụ đang kết thúc | danh sách dịch vụ thiếu (không hiện hộp thoại); luôn khác null, lỗi API → list rỗng (fail-open) |
| `ConfirmFinish(List<SereServCheckADO>)` | danh sách thiếu | hiện thông báo CHẶN (`MessageBoxButtons.OK`, icon Warning) → **luôn false** khi còn dịch vụ thiếu; chỉ trả true khi danh sách rỗng/null hoặc chính hộp thoại lỗi (fail-open) |
| `BuildMessage(List<SereServCheckADO>)` | danh sách thiếu | chuỗi thông báo đã ghép "- Mã - Tên" từng dịch vụ vào `{0}` |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Đọc cờ dịch vụ | `HisRequestUriStore.MOSHIS_SERVICE_GET` = api/HisService/Get | MosConsumer | `HisServiceFilter.IDs` (không lọc IS_ACTIVE) |
| Dòng thuốc/VT con | `HisRequestUriStore.MOSHIS_SERE_SERV_GET` = api/HisSereServ/Get | MosConsumer | `HisSereServFilter.PARENT_IDs` + `TDL_SERVICE_TYPE_IDs` |
| Dịch vụ của y lệnh | api/HisSereServ/Get | MosConsumer | `HisSereServFilter.SERVICE_REQ_IDs` |

## 6. Dependencies

### Plugin đang gọi thư viện — **6 màn** thuộc 3 nhóm CLS + PTTT + Xét nghiệm
| Plugin | Màn hiển thị | Vị trí chèn |
|--------|--------------|-------------|
| HIS.Desktop.Plugins.ServiceExecute | Thực hiện dịch vụ ("Xử lý dịch vụ") | `UCServiceExecute.cs:4697` → `btnFinish_Click`, sau khối kiểm tra "chưa xử lý hết dịch vụ", trước POST `FinishWithTime` |
| HIS.Desktop.Plugins.SurgServiceReqExecute | Thực hiện PTTT | (1) `finishClick()`: kiểm tra **TRƯỚC** `btnSaveClick(true)` (từ 25/09; trước đó đặt sau nên ô KT đã kết thúc y lệnh trước khi chặn), bị chặn thì vẫn lưu rồi `return`; (2) `btnSaveClick()`: trước khi đặt `IsFinished = true` ở cả nhánh lưu nhóm và lưu 1 dịch vụ — bao Lưu / Ctrl+S / Chỉ định trong kíp / DV phát sinh |
| HIS.Desktop.Plugins.SurgServiceReqExecute2 | Thực hiện PTTT (bản 2, việc 45072) | `UCSurgServiceReqExecute2_Right.cs` → `btnSave_Click`, sau `ComputeIsFinished_v45072()`: `IsFinished` và thư viện chặn → `IsFinished = false`. Kiểm tra theo `CheckBeforeFinishByServiceReqIds([SERVICE_REQ_ID])`. **Màn thứ 6, bổ sung 25/09/2026** |
| HIS.Desktop.Plugins.ServiceExecuteGroup | Trả kết quả tổng hợp | `Run/frmServiceExecuteGroup.cs:517` → `btnCancel_Click` (nút "Kết thúc (Ctrl E)"), trước `backgroundWorker1.RunWorkerAsync()` |
| HIS.Desktop.Plugins.TestServiceReqExcute | Xử lý xét nghiệm | `UCTestServiceReqExcute.cs:1828` → `btnFinish_Click`, sau `ValidTimeReturn`, trước `WaitingManager.Show()` |
| HIS.Desktop.Plugins.TestServiceExecute | Xử lý dịch vụ (bản có xem ảnh PACS — **tên hiển thị trùng `ServiceExecute`**) | `UCServiceExecute.cs:2894` → `btnFinish_Click`, sau khối kiểm tra `isSave` / "chưa xử lý hết dịch vụ", trước POST `HIS_SERVICE_REQ_FINISH` (`api/HisServiceReq/Finish`). **Màn thứ 5, rà ra 22/09/2026** (commit `359ff8013`) |

Bốn màn Finish đều chỉ chèn đúng một lời gọi (hai màn PTTT xem dòng riêng ở bảng trên) `if (!CheckRequireMediMateManager.CheckBeforeFinish(...)) return;` — riêng màn Trả kết quả tổng hợp gọi overload `CheckBeforeFinishByServiceReqIds(...)`; hai màn PTTT và Trả kết quả tổng hợp có thêm vài dòng dựng danh sách đầu vào trước lời gọi — và tham chiếu qua HintPath `..\..\..\..\lib\HIS\HIS.Desktop.Plugins.Library.CheckRequireMediMate\HIS.Desktop.Plugins.Library.CheckRequireMediMate.dll`. Viện khai plugin "Xử lý dịch vụ" bản nào cũng bị chặn như nhau.

### Màn CỐ Ý không gọi thư viện (ngoài 3 nhóm anh Cảnh chốt 22/09/2026)
Khám bệnh (`ExamServiceReqExecute`, kể cả điểm tự kết thúc y lệnh khám khi kết thúc điều trị — chặn ở đây sẽ hỏng luồng ra viện), Khám sức khỏe (`EnterKskInfomantion*`), Phục hồi chức năng (`RehaServiceReqExecute`), Đo thị lực (`Optometrist`), Lọc máu (`Hemodialysis` — kết thúc bằng đổi trạng thái trên lưới nên muốn chặn phải trả lại giá trị ô, không chỉ `return`). Viện muốn mở rộng thì báo để làm tiếp.

### Thư viện phụ thuộc
Inventec.Common.Adapter (BackendAdapter), HIS.Desktop.ApiConsumer (ProjectReference), MOS.EFMODEL, MOS.Filter, IMSys.DbConfig.HIS_RS, Inventec.Common.Resource + Inventec.Desktop.Common.LanguageManager (đa ngôn ngữ), DevExpress.XtraEditors (XtraMessageBox).

## 7. Resources / Đa ngôn ngữ

- `Resources/Message.Lang.resx` — resource **trung tính** (tiếng Việt) nhúng trong DLL chính; `Resources/Message.Lang.en.resx` — satellite `en\...resources.dll`. Thiếu satellite vẫn hiện tiếng Việt (không bị chuỗi rỗng); ngoài ra mỗi chuỗi còn có giá trị mặc định trong `ResourceMessage.cs`.
- Key: `Library_CheckRequireMediMate__DichVuChuaCoThuocVatTuDiKem` ("Không kết thúc được. Dịch vụ chưa có thuốc, vật tư đi kèm:\n{0}\nVui lòng kê thuốc, vật tư đi kèm cho các dịch vụ trên rồi kết thúc lại."), `Library_CheckRequireMediMate__ThongBao`.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 25/09/2026 | dangth2 | **Tester (chị Hân) báo "chưa được" → rà lại toàn bộ, vá 3 chỗ, đẩy lại test.** (1) Danh mục `HisService` trên test bị dev khác đè từ 22/09 15:06 bằng bản không có ô tick "Có thuốc, vật tư đi kèm" → tester không bật cờ được suốt 22/09-25/09; 25/09 13:56 dev đó đẩy lại bản có ô tick (HISTEST `37e858d23`, IL trùng bản Develop). (2) Màn PTTT `SurgServiceReqExecute`: ô **KT** (mặc định tick) làm Lưu gửi `IsFinished = true` không qua kiểm tra, và nút Kết thúc lưu (đã hoàn thành) rồi mới kiểm tra → chuyển kiểm tra lên trước + chặn `IsFinished`. (3) Bổ sung màn thứ 6 `SurgServiceReqExecute2` (kết thúc qua `SurgUpdate` + ô KT). Thư viện: sửa giá trị mặc định câu thông báo trong `ResourceMessage.cs` (trước còn "Bạn có muốn tiếp tục kết thúc?", chỉ lộ khi đọc resource lỗi) + comment `CheckBeforeFinish`. API công khai và AssemblyVersion 1.0.0.0 giữ nguyên nên các màn cũ không phải build lại vì thư viện. DLL test: HISTEST `a13c6cb21`. |
| 22/09/2026 | dangth2 | **Bổ sung màn thứ 5 — `HIS.Desktop.Plugins.TestServiceExecute`** ("Xử lý dịch vụ", bản có xem ảnh PACS, tên hiển thị trùng `ServiceExecute` nên lần rà đầu bị bỏ sót). Rà lại toàn bộ điểm gọi `api/HisServiceReq/Finish` / `FinishWithTime` theo ý anh Cảnh *"chặn hết ở màn hình xử lí nhé"*: thêm một dòng `CheckBeforeFinish(listServiceADO)` ở `TestServiceExecute/UCServiceExecute.cs:2894` (`ServiceADO : HIS_SERE_SERV` nên dùng đúng overload sẵn có, **không phải sửa thư viện**) + `Reference` HintPath `lib\HIS\...` vào csproj; dọn comment cũ ở 4 điểm chèn trước (vẫn ghi "hỏi Yes/No, không chặn" trong khi hành vi đã là chặn). Từ đây thư viện áp dụng đủ **5 màn** CLS + PTTT + XN; các màn kết thúc ngoài 3 nhóm cố ý không chặn (xem mục 6). Cùng đợt chốt: **phần API cho hệ thống PACS bỏ khỏi phạm vi** (PACS tự làm bên họ). Commit `359ff8013`; DLL test: HISTEST 052c55c03. |
| 22/09/2026 | dangth2 | **Đổi mức xử lý từ CẢNH BÁO sang CHẶN** theo chỉ đạo anh Cảnh (Zalo 22/09 16:28-16:31: *"chặn hết ở màn hình xử lí nhé"*, *"cls,pttt, xn"*, *"điều kiện của mình là dịch vụ phải được tick trước mà"*). `ConfirmFinish` bỏ hộp Yes/No, hiện thông báo chỉ có OK và luôn trả `false` khi còn dịch vụ thiếu; câu thông báo vi/en viết lại theo hướng chặn kèm hướng dẫn kê bổ sung. Bốn màn gọi lúc đó không phải sửa (vẫn `if (!CheckBeforeFinish(...)) return;`) — màn thứ 5 `TestServiceExecute` bổ sung cùng ngày, xem dòng trên. Giữ fail-open khi lỗi kỹ thuật. ⚠ **Khác tài liệu gốc 3353 mục 3.2 và 3.5** ("cảnh báo ở mức nhắc, không chặn" / "KHÔNG chặn thao tác kết thúc dịch vụ") và khác kịch bản 3 của tài liệu (thực tế không dùng thuốc vẫn xác nhận để kết thúc) → PLT cần cập nhật tài liệu. Rủi ro đã báo: màn **Xử lý xét nghiệm** nút "Kê tủ trực" mở `AssignPrescriptionPK` chỉ truyền `ServiceReqId`, thuốc kê ra không gắn `PARENT_ID` vào dịch vụ nên nếu viện tick cờ cho dịch vụ xét nghiệm thì không có đường kê để thoát chặn ngay trên màn đó. DLL test: HISTEST 39fe57142 (22/09 17:12). |
| 18/09/2026 | dangth2 | Tạo mới thư viện (việc 3353 / PT-56272). Static `CheckRequireMediMateManager` với 4 overload `CheckBeforeFinish` + `CheckBeforeFinishByServiceReqIds`; 2 request backend (cờ dịch vụ theo IDs, dòng con theo PARENT_IDs + loại Thuốc/VT); hộp thoại Yes/No mặc định No; fail-open. Chèn vào 4 màn kết thúc: ServiceExecute, SurgServiceReqExecute, ServiceExecuteGroup, TestServiceReqExcute. Chờ Backend: cột `HIS_SERVICE.IS_REQUIRE_MEDI_MATE` + view + EFMODEL (màn danh mục `HisService` thêm mục tick "Có thuốc, vật tư đi kèm" sau khi có DLL mới). Thiết kế: `PTTK\3353 - Thiet ke - ...md`. |

## 9. Test Cases

- [ ] Dịch vụ không bật cờ → kết thúc như cũ, không có request `api/HisSereServ/Get` (xem log).
- [ ] Dịch vụ bật cờ, chưa kê → thông báo liệt kê đúng "- Mã - Tên", hộp thoại **chỉ có nút OK** (không có Yes/No, không có nút mặc định để bỏ qua); bấm OK xong y lệnh **vẫn chưa hoàn thành**, log không có request Finish, thời gian kết thúc không được ghi.
- [ ] Bấm Kết thúc lại nhiều lần khi chưa kê → lần nào cũng bị chặn (kể cả "Tự động kết thúc" sau Lưu và phím tắt ở các màn gọi lại chính hàm kết thúc).
- [ ] Màn PTTT (cả `SurgServiceReqExecute` và `SurgServiceReqExecute2`), ô **KT** đang tick, dịch vụ bật cờ chưa kê → bấm **Lưu** → hộp chặn hiện 1 lần, dữ liệu PTTT được lưu, y lệnh **vẫn Đang xử lý**, tab không tự đóng. Bấm **Kết thúc** → hộp chặn hiện 1 lần, y lệnh vẫn chưa hoàn thành. Kê thuốc/VT đi kèm rồi Lưu hoặc Kết thúc lại → hoàn thành.
- [ ] Màn PTTT, ô KT **không** tick → Lưu không hiện hộp chặn (không kết thúc nên không kiểm tra).
- [ ] Đã kê 1 thuốc hoặc 1 vật tư (kể cả hao phí) gắn dịch vụ → kết thúc bình thường, không bị chặn.
- [ ] Kê rồi hủy đơn → bị chặn lại.
- [ ] Y lệnh 2 dịch vụ thiếu / Trả kết quả tổng hợp 2 y lệnh → 1 thông báo liệt kê đủ; không y lệnh nào được kết thúc.
- [ ] Dịch vụ tick "Không thực hiện" → bỏ qua.
- [ ] Backend cũ chưa có cột → không chặn, không lỗi; mất mạng khi kiểm tra → vẫn cho kết thúc, log Warn (fail-open).
- [ ] Đổi ngôn ngữ en → thông báo tiếng Anh (satellite `en`); thiếu satellite → tiếng Việt.
- [ ] Kê bổ sung đủ cho mọi dịch vụ bị liệt kê rồi kết thúc lại → hoàn thành bình thường (đường thoát duy nhất).
- [ ] Đủ **6 màn** đều chặn (thêm màn thứ 6 **Thực hiện PTTT bản 2** `SurgServiceReqExecute2`, xem dòng ô KT ở trên): Thực hiện dịch vụ (`ServiceExecute`), Thực hiện PTTT, Trả kết quả tổng hợp, Xử lý xét nghiệm, **Xử lý dịch vụ bản `TestServiceExecute`** (màn thứ 5 — khai module `HIS.Desktop.Plugins.TestServiceExecute`, xử lý/lưu hết dịch vụ rồi bấm Kết thúc → bị chặn y như màn Thực hiện dịch vụ; kê bổ sung → kết thúc được).
- [ ] Màn ngoài 3 nhóm (Khám bệnh, KSK, PHCN, Đo thị lực, Lọc máu) → **không** bị chặn, kết thúc như cũ.
- [ ] Xét nghiệm: dịch vụ XN tick cờ → bị chặn; lưu ý nút "Kê tủ trực" trên màn Xử lý xét nghiệm kê theo yêu cầu, không gắn dịch vụ nên không gỡ được chặn (khuyến cáo viện chưa tick cờ cho dịch vụ XN).
