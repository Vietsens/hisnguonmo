# Chọn kết quả cận lâm sàng — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ContentSubclinical |
| Loại | Form (`frmContentSubclinical`) |
| Mục đích | Hiển thị cây kết quả cận lâm sàng của 1 hồ sơ điều trị (ngày → nhóm DV → dịch vụ → chỉ số XN) để bác sĩ tích chọn và chèn vào tờ điều trị; kèm bộ lọc, in kết quả. Từ việc 3170 có thêm chế độ CHỈ XEM để tra cứu từ ngoài tờ điều trị |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính (chế độ chọn — từ tờ điều trị)
1. Tờ điều trị (HisTrackingList/DocumentEdit...) mở plugin, truyền `Module + treatmentId + DelegateSelectData`.
2. Form load toàn bộ y lệnh có kết quả của hồ sơ, dựng cây theo ngày → nhóm DV → dịch vụ → chỉ số (giảm dần theo ngày).
3. Bác sĩ lọc/tích chọn → nhấn "Chọn (Ctrl S)" → build chuỗi kết quả theo 6 tuỳ chọn định dạng → bắn `DelegateSelectData` về tờ điều trị.

### 4 chế độ của Behavior (`ContentSubclinicalBehavior.Run`)
| Chế độ | Điều kiện nhận biết trong args | Hành vi |
|--------|------------------------------|---------|
| Headless theo nhóm chỉ số | có `DelegateSelectTestIndexGroupData` | Tạo form không Show, load đồng bộ, bắn delegate |
| Headless theo serviceIds | có `DelegateSelectData` + tham số `string` | Lấy kết quả DV con theo cấu hình, trả chuỗi |
| Form chọn (mặc định) | có `DelegateSelectData` | ShowDialog để tích chọn/chèn |
| **CHỈ XEM (việc 3170)** | có `List<string>` chứa `ARG__VIEW_ONLY` ("VIEW_ONLY"), KHÔNG cần delegate | ShowDialog chỉ để tra cứu: ẩn checkbox cây, ẩn nút Chọn + 6 tuỳ chọn chèn, vô hiệu Ctrl+S, thêm nút "Đóng", đổi tiêu đề "Kết quả cận lâm sàng" (`ApplyViewOnlyMode()`) |

**Bẫy khi truyền args**: các kiểu đơn giản đã bị chiếm hết — `long` = treatmentId, `string` = serviceIds (kích headless), `bool` = returnObject. Cờ mới bắt buộc đi qua `List<string>`.

### Kết quả theo tab nhóm dịch vụ (việc 57971 — chế độ CHỈ XEM + cờ `CLS_TAB`)
Buồng bệnh bật key `HIS.Desktop.Plugins.BedRoomPartial.ClsTabOption` thì nút "Kết quả CLS" gửi thêm `"CLS_TAB"` (+ `"CLS_TAB_INIT:<mã nhóm>"`) trong `List<string>`. Behavior chỉ áp dụng ở nhánh CHỈ XEM; chế độ chọn và headless bỏ qua cờ (giữ nguyên cây).
- Thanh tab ngang (runtime, `InitClsTabBar`, chèn ngay trên cây trong `layoutControl2`): **Tất cả** + 1 tab / nhóm **có kết quả** theo bảng nhóm dùng chung với Buồng bệnh (`Base/ClsTabGroupStore.cs`, bản sao y hệt): Xét nghiệm, Siêu âm, X-quang, CT / MRI, loại CĐHA khác (tên theo danh mục), TDCN, Nội soi, Giải phẫu bệnh, **Thủ thuật**, CLS khác. Tên tab kèm số dịch vụ.
- Tab Tất cả = cây cũ (Ngày → loại DV → DV → chỉ số), luôn đủ mọi kết quả của khoảng ngày (không bị bộ lọc XN). Tab nhóm: Ngày (in đậm, mới nhất trên) → DV → chỉ số. Mọi tab mở sẵn, không có nút mở/thu gọn (`ShowButtons = false`, `AllowExpandOnDblClick = false`, chặn `BeforeCollapse`). Phân nhóm theo khóa đường dẫn `CONCRETE_ID__IN_SETY` (đoạn 2 = loại DV) + loại CĐHA của dịch vụ (`V_HIS_SERVICE.DIIM_TYPE_ID`); cây dùng `ParentFieldName = PARENT_ID__IN_TAB` (gán lại mỗi lần đổi tab, `PARENT_ID__IN_SETY` giữ nguyên).
- Bộ lọc XN (Trên/Dưới ngưỡng, Chỉ số quan trọng, Hiển thị DV cha loại XN, Hiển thị kháng sinh đồ) chỉ bật ở tab Xét nghiệm và chỉ lọc tab này (`LoadDataSS` giữ bản trước bộ lọc cho Tất cả và các tab khác; số trên tab tính từ bản này). Ở chế độ tab, "Chỉ số quan trọng" không còn bớt dòng khi dựng (kháng sinh đồ, ghi chú LIS — `IsImportantOnlyBuild`), chỉ lọc tab Xét nghiệm. Dòng kháng sinh đồ (MIC) mang `ID` = sere_serv ở chế độ tab để đếm 1 dịch vụ 1 lần và hiện ảnh nuôi cấy trong tab. Lọc xong, chỉ số vẫn nằm dưới dòng ngày + dòng xét nghiệm của nó (dựng lại dòng cha bị bộ lọc bỏ); lọc hết thì dòng "Không có kết quả phù hợp điều kiện lọc". Cột SRI hiện ở Tất cả và Xét nghiệm, ẩn ở tab khác.
- Khung ảnh chỉ ảnh của dịch vụ trong tab đang chọn (`ImageADO.SERE_SERV_ID`; ảnh tải 1 lần cho mọi tab).
- Mở sẵn tab theo `CLS_TAB_INIT` nếu nhóm có kết quả, không thì Tất cả. Tìm lại (đổi ngày / hồ sơ khác / bộ lọc): tab tính lại, tab đang chọn hết dữ liệu thì về Tất cả. Không có kết quả: chỉ còn Tất cả, cây/ảnh trống, dòng "Không có kết quả cận lâm sàng trong khoảng ngày đã chọn".
- In kết quả: in đúng các dịch vụ của tab đang xem (mỗi lần dựng tab, mọi dòng được tích ngầm); phiếu CĐHA lẫn X-quang + CT → tab X-quang ra phiếu X-quang, tab CT / MRI ra phiếu CT (mục 7).
- Bố cục cây đã lưu (`HIS.Desktop.ApplyRestoreLayout.ModuleLinks`, FormBase đặt tên file theo đường dẫn control): chế độ tab đổi tên cây thành `treeListServiceReqClsTab` nên có file riêng — `ParentFieldName = PARENT_ID__IN_TAB` và cột SRI ẩn theo tab không lọt sang file của chế độ chọn (trước đó màn chọn kết quả từ tờ điều trị mở ra cây thu gọn, mất tích, "Chọn" trả rỗng). Chế độ tab không áp bố cục đã lưu (`BeforeLoadLayout` → `Allow = false`), luôn dùng cây thiết kế.
- Code: `frmContentSubclinical__Pluss__ClsTab.cs` (thanh tab, gắn dữ liệu), `…__ClsTabRows.cs` (phân nhóm, đếm, dòng từng tab), `…__ClsTabEvent.cs` (sự kiện cây/form).

### Điều kiện nghiệp vụ
- Chỉ hiện y lệnh trạng thái Hoàn thành/Đang xử lý; key `HIS.Desktop.Plugins.ContentSubclinical.ShowResultWhenReqComplete` = 1 thì chỉ Hoàn thành, = 2 thì XN phải Hoàn thành.
- Chế độ chỉ xem: `treatmentIdSearch <= 0` không popup "Bắt buộc chọn Hồ sơ điều trị" (chỉ popup ở chế độ chọn khi user chọn hồ sơ khác).
- **Cột Ngày KQ (việc 57966)**:
  - Xét nghiệm, dòng dịch vụ (`HIS_SERE_SERV`): `HIS_SERVICE_REQ.FINISH_TIME` của y lệnh chứa dịch vụ (tra theo `HIS_SERE_SERV.SERVICE_REQ_ID`; giờ LABCON gửi kèm khi hoàn thành = ô "Thời gian kết thúc" màn Sửa thông tin chung y lệnh). Chỉ hiện khi y lệnh `SERVICE_REQ_STT_ID = HIS_SERVICE_REQ_STT.ID__HT`, có `FINISH_TIME` VÀ có ít nhất 1 chỉ số (`V_HIS_SERE_SERV_TEIN.VALUE`) có giá trị; thiếu thì trống. KHÔNG lấy `HIS_SERE_SERV_EXT.END_TIME` (LIS/LABCON không ghi, chỉ có khi lưu Sửa thông tin y lệnh và còn lọt ở y lệnh chưa hoàn thành).
  - Xét nghiệm, dòng chỉ số (`HIS_SERE_SERV_TEIN`): luôn TRỐNG (chốt với chị Hân 16/10/2026: Ngày KQ là của dịch vụ, không phải của từng chỉ số). Không đọc `HIS_SERE_SERV_TEIN.RESULT_TIME` (cột có trong DB nhưng không phần mềm nào ghi). Chế độ Trên/Dưới ngưỡng cây chỉ còn danh sách chỉ số (không có dòng dịch vụ) nên cột Ngày KQ trống.
  - Loại khác (thủ thuật, CĐHA, NS, SA, TDCN, GPBL): giữ `HIS_SERE_SERV_EXT.END_TIME` như cũ.
  - XN nhánh kết luận (y lệnh có `EXE_SERVICE_MODULE` khác `TestServiceReqExcute` + có EXT, return sớm) và các node vi sinh/vi khuẩn/kháng sinh (tích "Hiển thị kháng sinh đồ"): vẫn TRỐNG như trước (chưa từng gán END_TIME). Tích "Hiển thị kháng sinh đồ" mà dịch vụ không có kết quả vi sinh thì rơi về nhánh thường và theo quy tắc XN ở trên.
  - Hủy hoàn thành → MOS đặt `FINISH_TIME = null`, STT về ĐXL nhưng giữ `VALUE` → bắt buộc xét STT, không dựa vào có giờ hay có giá trị.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ | Table | Y lệnh của hồ sơ; `SERVICE_REQ_STT_ID` + `FINISH_TIME` dùng cho Ngày KQ xét nghiệm (việc 57966, tra qua Dictionary `serviceReqById` theo ID truyền vào `LoadDataService`) |
| HIS_SERE_SERV | Table | Dịch vụ đã thực hiện (7 loại: CĐHA, NS, SA, TDCN, XN, TT, GPBL) |
| HIS_SERE_SERV_EXT | Table | Ghi chú / nhận xét / kết luận |
| V_HIS_SERE_SERV_TEIN | View | Chỉ số xét nghiệm + khoảng tham chiếu; `VALUE` dùng làm điều kiện hiện Ngày KQ ở dòng dịch vụ (việc 57966; không đọc `RESULT_TIME`) |
| V_HIS_SERVICE, V_HIS_TEST_INDEX, HIS_TEST_INDEX_GROUP | Cache/API | Danh mục; `V_HIS_SERVICE.DIIM_TYPE_ID/DIIM_TYPE_NAME` tách tab X-quang / CT / MRI (việc 57971) |
| HIS_SERE_SERV_FILE | API | Ảnh kết quả; `SERE_SERV_ID` gắn vào ảnh để lọc theo tab (việc 57971) |
| LIS_SAMPLE / LIS_SAMPLE_SERVICE / LIS_RESULT | LIS | Kháng sinh đồ (khi tích chọn) |

## 4. UI Layout

- Cây `treeListServiceReq` (TreeList, `ShowCheckBoxes = true`; chế độ chỉ xem tắt checkbox) — cột: Kết quả, SRI, Ghi chú, Ngày KQ (`END_TIME` chuỗi `dd/MM/yyyy HH:mm:ss`, quy tắc ở mục 2; màu chữ theo màu dòng như các ô khác).
- Bộ lọc trên `layoutControl2`: hồ sơ hiện tại/khác, khoảng ngày chỉ định, Trên/Dưới ngưỡng, Chỉ số quan trọng, Hiển thị DV cha loại XN, Hiển thị kháng sinh đồ.
- Chế độ chỉ xem + cờ `CLS_TAB` (việc 57971): `XtraTabControl` chỉ có header (`tabCls`, `lciTabCls` cao 25px) chèn ngay trên cây (`Move(layoutControlItem25, InsertType.Top)`), không sửa Designer.
- Hàng nút dưới: 6 tuỳ chọn định dạng chèn (`chkAssign`, `chkServiceType`, `chkLineBreak`, `chkGetInfo`, `chkJustSelectIndexImportant`, `chkNotSelectSurg`) + `btnSave` "Chọn (Ctrl S)" + `btnPrintKetQua` "In kết quả". Chế độ chỉ xem: ẩn cả 7 layout item này, thêm `btnClose` runtime cạnh nút In (`LayoutControlItem.Move` sau `layoutControlItem3`).

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Y lệnh | api/HisServiceReq/Get | MosConsumer |
| Dịch vụ | api/HisSereServ/Get | MosConsumer |
| Ghi chú/kết luận | /api/HisSereServExt/Get | MosConsumer |
| Chỉ số XN | api/HisSereServTein/Get (View) | MosConsumer |
| Nhóm chỉ số | api/HisTestIndexGroup/Get | MosConsumer |
| Kháng sinh đồ | api/LisSample/Get, /api/LisSampleService/Get, api/LisResult/Get | LisConsumer |
| Ảnh kết quả | /api/HisSereServFile/Get (+ tải file qua FSS) | MosConsumer |

## 6. Dependencies

### Plugin gọi vào (đầu vào)
| Plugin gọi | Chế độ |
|-----------|--------|
| Tờ điều trị (HisTrackingList...) | Chọn/chèn (delegate) |
| KSK (nhóm chỉ số XN) | Headless |
| **HIS.Desktop.Plugins.BedRoomPartial (việc 3170)** | Chỉ xem — args: `long treatmentId` + `List<string>{"VIEW_ONLY"}` + `Module`; key Buồng bệnh `ClsTabOption` bật thì list thêm `"CLS_TAB"`, `"CLS_TAB_INIT:<mã nhóm>"` (việc 57971) |

## 7. Print

In kết quả qua `Print/PrintKetQuaProcessor` (RichEditorStore + Mps000014 cho XN, template SAR cho CĐHA/GPBL/NS/SA/TDCN). Hoạt động ở CẢ 2 chế độ (in là thao tác chỉ đọc).

Ngày KQ (việc 57966): phiếu Mps000014 in giờ trả kết quả từ `HIS_SERVICE_REQ.FINISH_TIME` (`FINISH_TIME_FULL_STR`), cùng nguồn với dòng dịch vụ trên màn. Phiếu không xét trạng thái y lệnh: y lệnh Đang xử lý còn sót `FINISH_TIME` (dữ liệu cũ, vd DB test hồ sơ 159350 y lệnh 000000867269) thì màn để trống nhưng phiếu vẫn in giờ — muốn khớp tuyệt đối thì sửa Mps000014 (việc riêng).

In gộp: `Print()` chờ đến khi số y lệnh đã xử lý (`CountServiceReqPrinted`, tăng qua `SetDataGroup`) bằng số y lệnh cần in, tối đa `TIME_OUT_PRINT_MERGE` × 50 ms = 40 giây. Việc 57971 sửa lỗi có sẵn (mọi chế độ):
- `PrintDocumentKetQua` gán `saveMemoryStream` cho `PrintMergeAdo` null → NullReferenceException, phiếu CĐHA/SA/NS/TDCN/GPBL không bao giờ vào bản in gộp, màn chờ đủ 40 giây rồi báo "Không tìm thấy dữ liệu để in!". Nay tạo `PrintMergeAdo` mới (PDF chép sang `MemoryStream` riêng, `numCopy = 1`, `EmrInputADO` của phiếu).
- Y lệnh CĐHA/SA/NS/TDCN/GPBL: in 1 phiếu cho **mỗi dịch vụ đang tích** của y lệnh, theo `HIS_SERE_SERV_EXT` của chính dịch vụ đó (`PrintDocumentsOfServiceReq`, `GetCheckedSereServExts`; trước đây lấy kết quả đầu tiên của cả y lệnh → tab CT / MRI có thể in phiếu X-quang). Thông tin điều trị và y lệnh (`FINISH_TIME`, `EXECUTE_USERNAME`) nạp 1 lần / y lệnh (`GetSheetServiceReq`); gọi API y lệnh lỗi thì phiếu vẫn in, chỉ trống người thực hiện (trước đây phiếu bị bỏ không báo). Mỗi y lệnh tính đã xử lý đúng 1 lần, kể cả khi không ra phiếu (chưa có kết quả, không có mẫu SAR, lỗi). Loại y lệnh không có mẫu kết quả (thủ thuật), y lệnh không thực hiện cũng tính đã xử lý. XN: `RunPrintTemplate` chạy đồng bộ, nếu xong mà y lệnh chưa được tính (lỗi dựng phiếu Mps000014, không chọn mẫu) thì tính luôn.
- Xem in (`ShowMergedPreview`): phiếu XN (Excel) xếp trước vì bản xem in lấy máy in / số bản / EMR từ phiếu đầu; chỉ 1 phiếu PDF thì mở `frmSetupPrintPreviewMerge` (như nhánh nhiều phiếu của `FlexCelPrintProcessor.PrintPreviewShow`) — nhánh 1 phiếu của `FlexCelPrintProcessor` mở PDF như file Excel → bản xem trống.

Lưu ý build máy backup: `RichEditorStore.SetActionCancelChooseTemplate` gọi qua **reflection** (`PrintKetQuaProcessor.cs:157`) vì DLL `Inventec.Common.RichEditor` trên máy backup cũ hơn source — máy có DLL mới vẫn gắn callback như bản gốc, DLL cũ thì bỏ qua.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 13/08/2026 | nampp | Việc 3170 (BV Điện Biên): Thêm chế độ CHỈ XEM. Behavior nhận cờ `List<string>` chứa `ARG__VIEW_ONLY`, nhánh mới không đòi `DelegateSelectData` (đặt SAU các nhánh cũ — không ảnh hưởng luồng cũ). Form: constructor `(Module, long, bool isViewOnly)` + `ApplyViewOnlyMode()` (ẩn checkbox cây, ẩn btnSave + 6 tuỳ chọn chèn qua `LayoutVisibility.Never`, vô hiệu `barButtonItemSave` Ctrl+S, thêm `btnClose` runtime, tiêu đề "Kết quả cận lâm sàng"); guard `if (isViewOnly) return;` đầu `btnSave_Click`; không popup "Bắt buộc chọn Hồ sơ điều trị" khi chỉ xem. Resource 3 ngôn ngữ `frmContentSubclinical.Text.ViewOnly` + `btnClose.Text`. Sửa kèm: `SetActionCancelChooseTemplate` → reflection (mismatch DLL máy backup); xóa entry `licenses.licx` stale khỏi csproj. |
| 13/10/2026 | dangth2 | Việc 57966 (BV Nguyễn Tri Phương): cột Ngày KQ của xét nghiệm trả từ LABCON. Dòng chỉ số: y lệnh Hoàn thành + chỉ số có giá trị → `RESULT_TIME` của chỉ số, không có thì `FINISH_TIME` y lệnh; chưa HT/hủy HT/chưa giá trị → trống. Dòng dịch vụ XN = giờ lớn nhất của chỉ số con (bỏ `EXT.END_TIME` với XN). Loại CLS khác giữ nguyên. Chỉ sửa `frmContentSubclinical__Pluss__Load.cs`: `LoadDataService`/`LoadDataMicrobiologicalResult` nhận `Dictionary<long, HIS_SERVICE_REQ> serviceReqById` (dựng 1 lần trong `LoadDataSS`) thay cho `List<HIS_SERVICE_REQ> rsServiceReq` (tra y lệnh O(1) thay `rsServiceReq.FirstOrDefault`, dùng cho cả nhánh kết luận lẫn Ngày KQ); thêm `GetTestIndexResultTime`, `GetSereServTeinResultTime` (đọc `RESULT_TIME` qua reflection, chịu được MOS.EFMODEL cũ), `ResultTimeToString`. Không thêm API, không đổi resource, không đổi dữ liệu trả về cho màn gọi. Phiếu Mps000014 in `FINISH_TIME` nên hiện cùng nguồn với màn (MOS chưa ghi `RESULT_TIME`); không sửa phiếu (xem mục 7). |
| 14/10/2026 | dangth2 | Việc 57971 (BV Nguyễn Tri Phương): chế độ CHỈ XEM mở từ Buồng bệnh có cờ `CLS_TAB` → thay cây theo ngày bằng thanh tab ngang Tất cả + tab nhóm có kết quả (bảng nhóm dùng chung `Base/ClsTabGroupStore.cs`, có tab Thủ thuật), mở sẵn tab theo `CLS_TAB_INIT:<mã nhóm>`; tab nhóm Ngày → DV → chỉ số; mọi tab mở sẵn, không thu gọn; bộ lọc XN chỉ ở tab Xét nghiệm (Tất cả + tab khác luôn đủ kết quả), cột SRI ở Tất cả/Xét nghiệm; ảnh lọc theo tab; in đúng dịch vụ của tab; dọn cây/ảnh/tab khi không có kết quả. File mới `frmContentSubclinical__Pluss__ClsTab.cs`, `Base/ClsTabGroupStore.cs`; sửa Behavior (đọc cờ), `frmContentSubclinical.cs` (Load gọi `InitClsTabBar`), `__Pluss__Load.cs` (bản trước bộ lọc XN, `BindClsTabs`, `ClearClsTabs`, ảnh có `SERE_SERV_ID`), ADO `PARENT_ID__IN_TAB`, `ImageADO.SERE_SERV_ID`, resource vi/en/my. Không có cờ: mọi chế độ như cũ. Sửa kèm lỗi in có sẵn ở `Print/PrintKetQuaProcessor.cs` (mọi chế độ): phiếu CĐHA/SA/NS/TDCN/GPBL không vào bản in gộp và màn chờ 40 giây rồi báo "Không tìm thấy dữ liệu để in!"; nay mỗi dịch vụ đang tích 1 phiếu theo kết quả của chính dịch vụ, mỗi y lệnh tính xong 1 lần (cả XN lỗi), 1 phiếu PDF mở bằng form xem in gộp (mục 7). Chế độ tab: "Chỉ số quan trọng" chỉ lọc tab Xét nghiệm; cây mang tên riêng `treeListServiceReqClsTab` (file bố cục đã lưu riêng, không áp bố cục đã lưu). Sửa kèm: LIS không trả lời (`LisSample/Get` null) không còn làm mất cả dòng xét nghiệm. File tách: `…__ClsTab.cs`, `…__ClsTabRows.cs`, `…__ClsTabEvent.cs`. |

| 16/10/2026 | dangth2 | Việc 57966 sửa theo test chị Hân: cột Ngày KQ của xét nghiệm chỉ hiện ở dòng dịch vụ (`HIS_SERE_SERV`) = `HIS_SERVICE_REQ.FINISH_TIME` khi y lệnh Hoàn thành và có chỉ số có giá trị; dòng chỉ số (`HIS_SERE_SERV_TEIN`) để trống. Bỏ `GetTestIndexResultTime`, `GetSereServTeinResultTime` (không còn đọc `RESULT_TIME` qua reflection), thêm `GetTestResultTime(serviceReq, hasTestValue)`. Chỉ sửa `frmContentSubclinical__Pluss__Load.cs`. |

## 9. Test Cases

### Kết quả theo tab (việc 57971 — key Buồng bệnh `ClsTabOption` = 1/2)
- [ ] Buồng bệnh bấm "Kết quả CLS" khi đang ở tab Tất cả → thanh tab Tất cả + nhóm có kết quả (kèm số), mở tab Tất cả, cây như cũ
- [ ] Đang ở tab Siêu âm của Buồng bệnh → mở sẵn tab Siêu âm: Ngày in đậm → DV, không nút thu gọn, không cột SRI, bộ lọc XN mờ, ảnh chỉ của siêu âm
- [ ] Tab Tất cả: cây như cũ, mở sẵn, không thu gọn được, ô lọc XN mờ, có cột SRI
- [ ] Tab Xét nghiệm: Ngày → DV → chỉ số, màu bất thường + Ngày KQ như cũ, ô lọc dùng được; tick Trên ngưỡng → Tìm → chỉ tab Xét nghiệm bị lọc (chỉ số vẫn dưới dòng ngày + xét nghiệm), Tất cả và tab khác vẫn đủ; lọc hết → dòng "Không có kết quả phù hợp điều kiện lọc"
- [ ] Phiếu CĐHA X-quang + CT → tab X-quang và CT / MRI tách đúng; CĐHA chưa gán loại → CLS khác; thủ thuật → tab Thủ thuật
- [ ] Đổi khoảng ngày / Hồ sơ khác → Tìm → tab tính lại; tab đang chọn hết dữ liệu → Tất cả; khoảng ngày không có kết quả → chỉ Tất cả, dòng trống, không còn ảnh cũ
- [ ] In kết quả ở tab nào in dịch vụ của tab đó; phiếu siêu âm / CĐHA đã có kết quả hiện bản xem in ngay (trước đây chờ ~40 giây rồi báo "Không tìm thấy dữ liệu để in!"), phiếu chưa có kết quả báo ngay
- [ ] Phiếu CĐHA có X-quang + CT: tab X-quang in phiếu X-quang, tab CT / MRI in phiếu CT, tab Tất cả in cả 2; chỉ 1 phiếu PDF vẫn hiện bản xem in (có trang, số bản = 1)
- [ ] Tab Xét nghiệm tick "Chỉ số quan trọng" → Tìm → tab Tất cả vẫn đủ kháng sinh đồ + ghi chú LIS
- [ ] Tờ điều trị → "Chọn kết quả CLS" → tích 1 dịch vụ CT của phiếu X-quang + CT → In kết quả chỉ ra phiếu CT
- [ ] Key Buồng bệnh trống → màn như cũ (cây); tờ điều trị "Chọn kết quả CLS" như cũ

### Ngày KQ xét nghiệm (việc 57966)
- [ ] XN Hoàn thành, LABCON trả đủ (Điện giải đồ NATRI/KALI/CLO) → dòng dịch vụ = Thời gian kết thúc y lệnh (`dd/MM/yyyy HH:mm:ss`), khớp màn Sửa thông tin chung y lệnh; các dòng chỉ số NATRI/KALI/CLO trống
- [ ] XN Đang xử lý (chưa có hoặc đã có giá trị) → trống; y lệnh Hoàn thành mà không chỉ số nào có giá trị → trống
- [ ] Hủy hoàn thành → trống; Hoàn thành lại → giờ mới; sửa Thời gian kết thúc ở Sửa thông tin y lệnh → Ngày KQ đổi theo
- [ ] XN cũ đã hoàn thành từ trước (lọc Ngày chỉ định từ/đến) → hiện giờ, không cần nhập bù
- [ ] Thủ thuật/CĐHA giữ nguyên giờ như trước; Chỉ số quan trọng, Hiển thị DV cha vẫn hiện giờ ở dòng dịch vụ; Trên/Dưới ngưỡng chỉ còn danh sách chỉ số → trống
- [ ] Chọn kết quả CLS từ tờ điều trị/KSK/hội chẩn → nội dung chèn không đổi so với bản cũ
- [ ] XN nhánh kết luận (thực hiện ở màn Xử lý dịch vụ) → Ngày KQ trống như cũ, kết luận vẫn hiện
- [ ] Tích "Hiển thị kháng sinh đồ": dịch vụ không có kết quả vi sinh vẫn có giờ, dịch vụ Đang xử lý vẫn trống
- [ ] Key `ShowResultWhenReqComplete` = 1 hoặc 2 → chỉ còn y lệnh/XN Hoàn thành, giờ vẫn đúng

### Chế độ chỉ xem (việc 3170)
- [ ] Mở từ nút "Kết quả CLS" màn Buồng bệnh → cây kết quả + bộ lọc + màu chỉ số bất thường giống hệt màn trong tờ điều trị
- [ ] Không có ô tích trên cây, không có nút "Chọn (Ctrl S)", không có 6 tuỳ chọn chèn; Ctrl+S không làm gì
- [ ] Có nút "Đóng" cạnh "In kết quả"; In kết quả hoạt động bình thường
- [ ] BN chưa có kết quả → cây rỗng, không popup lỗi
- [ ] Đổi ngôn ngữ vi/en → tiêu đề form + nút Đóng đúng ngôn ngữ

### Hồi quy chế độ cũ
- [ ] Mở từ tờ điều trị → đủ ô tích + nút Chọn + 6 tuỳ chọn; chèn kết quả vào tờ điều trị như cũ
- [ ] 2 chế độ headless (nhóm chỉ số KSK, serviceIds) chạy đúng
