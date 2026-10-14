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

### Điều kiện nghiệp vụ
- Chỉ hiện y lệnh trạng thái Hoàn thành/Đang xử lý; key `HIS.Desktop.Plugins.ContentSubclinical.ShowResultWhenReqComplete` = 1 thì chỉ Hoàn thành, = 2 thì XN phải Hoàn thành.
- Chế độ chỉ xem: `treatmentIdSearch <= 0` không popup "Bắt buộc chọn Hồ sơ điều trị" (chỉ popup ở chế độ chọn khi user chọn hồ sơ khác).
- **Cột Ngày KQ (việc 57966)**:
  - Xét nghiệm, dòng chỉ số: chỉ hiện khi y lệnh `SERVICE_REQ_STT_ID = HIS_SERVICE_REQ_STT.ID__HT` VÀ chỉ số có `VALUE`. Ưu tiên `V_HIS_SERE_SERV_TEIN.RESULT_TIME`, không có thì `HIS_SERVICE_REQ.FINISH_TIME` (giờ LABCON gửi kèm khi hoàn thành = ô "Thời gian kết thúc" màn Sửa thông tin y lệnh). Thiếu cả hai thì trống.
  - Xét nghiệm, dòng dịch vụ: giờ lớn nhất của các dòng chỉ số con đang có giờ; KHÔNG lấy `HIS_SERE_SERV_EXT.END_TIME` (LIS/LABCON không ghi, chỉ có khi lưu Sửa thông tin y lệnh và còn lọt ở y lệnh chưa hoàn thành).
  - Loại khác (thủ thuật, CĐHA, NS, SA, TDCN, GPBL): giữ `HIS_SERE_SERV_EXT.END_TIME` như cũ.
  - XN nhánh kết luận (y lệnh có `EXE_SERVICE_MODULE` khác `TestServiceReqExcute` + có EXT, return sớm) và các node vi sinh/vi khuẩn/kháng sinh (tích "Hiển thị kháng sinh đồ"): vẫn TRỐNG như trước (chưa từng gán END_TIME). Tích "Hiển thị kháng sinh đồ" mà dịch vụ không có kết quả vi sinh thì rơi về nhánh thường và theo quy tắc XN ở trên.
  - Hiện nay MOS chưa ghi `HIS_SERE_SERV_TEIN.RESULT_TIME` (HisTestIndexResultTDO không có trường giờ) nên thực tế luôn là `FINISH_TIME`. Hủy hoàn thành → MOS đặt `FINISH_TIME = null`, STT về ĐXL nhưng giữ `VALUE` → bắt buộc xét STT, không dựa vào có giờ hay có giá trị.
  - `RESULT_TIME` đọc qua reflection (`PropertyInfo` cache, cách làm như 62ddc7801): client có MOS.EFMODEL cũ (trước 23/03/2026, chưa có cột) gọi thẳng property sẽ bị JIT ném MissingMethodException, rơi vào catch của `LoadDataService` và MẤT toàn bộ dòng XN (đã tái hiện: `PTTK\57966\harness_test_FE\efmodel_cu`). Không có cột thì coi như không có giờ riêng → lấy `FINISH_TIME`.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ | Table | Y lệnh của hồ sơ; `SERVICE_REQ_STT_ID` + `FINISH_TIME` dùng cho Ngày KQ xét nghiệm (việc 57966, tra qua Dictionary `serviceReqById` theo ID truyền vào `LoadDataService`) |
| HIS_SERE_SERV | Table | Dịch vụ đã thực hiện (7 loại: CĐHA, NS, SA, TDCN, XN, TT, GPBL) |
| HIS_SERE_SERV_EXT | Table | Ghi chú / nhận xét / kết luận |
| V_HIS_SERE_SERV_TEIN | View | Chỉ số xét nghiệm + khoảng tham chiếu; `RESULT_TIME` (giờ trả theo chỉ số, hiện chưa có dữ liệu) ưu tiên cho Ngày KQ (việc 57966) |
| V_HIS_SERVICE, V_HIS_TEST_INDEX, HIS_TEST_INDEX_GROUP | Cache/API | Danh mục |
| LIS_SAMPLE / LIS_SAMPLE_SERVICE / LIS_RESULT | LIS | Kháng sinh đồ (khi tích chọn) |

## 4. UI Layout

- Cây `treeListServiceReq` (TreeList, `ShowCheckBoxes = true`; chế độ chỉ xem tắt checkbox) — cột: Kết quả, SRI, Ghi chú, Ngày KQ (`END_TIME` chuỗi `dd/MM/yyyy HH:mm:ss`, quy tắc ở mục 2; màu chữ theo màu dòng như các ô khác).
- Bộ lọc trên `layoutControl2`: hồ sơ hiện tại/khác, khoảng ngày chỉ định, Trên/Dưới ngưỡng, Chỉ số quan trọng, Hiển thị DV cha loại XN, Hiển thị kháng sinh đồ.
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

## 6. Dependencies

### Plugin gọi vào (đầu vào)
| Plugin gọi | Chế độ |
|-----------|--------|
| Tờ điều trị (HisTrackingList...) | Chọn/chèn (delegate) |
| KSK (nhóm chỉ số XN) | Headless |
| **HIS.Desktop.Plugins.BedRoomPartial (việc 3170)** | Chỉ xem — args: `long treatmentId` + `List<string>{"VIEW_ONLY"}` + `Module` |

## 7. Print

In kết quả qua `Print/PrintKetQuaProcessor` (RichEditorStore + Mps000014 cho XN, template SAR cho CĐHA/GPBL/NS/SA/TDCN). Hoạt động ở CẢ 2 chế độ (in là thao tác chỉ đọc).

Ngày KQ (việc 57966): phiếu Mps000014 in giờ trả kết quả từ `HIS_SERVICE_REQ.FINISH_TIME` (`FINISH_TIME_FULL_STR`). Màn hình ưu tiên `V_HIS_SERE_SERV_TEIN.RESULT_TIME` rồi mới tới `FINISH_TIME`. Hiện màn và phiếu cùng nguồn `FINISH_TIME` vì MOS chưa ghi `RESULT_TIME`; khi MOS bắt đầu ghi cột này thì phải sửa Mps000014 theo cùng quy tắc để phiếu không lệch màn hình. Phiếu không xét trạng thái y lệnh: y lệnh Đang xử lý còn sót `FINISH_TIME` (dữ liệu cũ, vd DB test hồ sơ 159350 y lệnh 000000867269) thì màn để trống nhưng phiếu vẫn in giờ — muốn khớp tuyệt đối thì sửa Mps000014 (việc riêng).

Lưu ý build máy backup: `RichEditorStore.SetActionCancelChooseTemplate` gọi qua **reflection** (`PrintKetQuaProcessor.cs:157`) vì DLL `Inventec.Common.RichEditor` trên máy backup cũ hơn source — máy có DLL mới vẫn gắn callback như bản gốc, DLL cũ thì bỏ qua.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 13/08/2026 | nampp | Việc 3170 (BV Điện Biên): Thêm chế độ CHỈ XEM. Behavior nhận cờ `List<string>` chứa `ARG__VIEW_ONLY`, nhánh mới không đòi `DelegateSelectData` (đặt SAU các nhánh cũ — không ảnh hưởng luồng cũ). Form: constructor `(Module, long, bool isViewOnly)` + `ApplyViewOnlyMode()` (ẩn checkbox cây, ẩn btnSave + 6 tuỳ chọn chèn qua `LayoutVisibility.Never`, vô hiệu `barButtonItemSave` Ctrl+S, thêm `btnClose` runtime, tiêu đề "Kết quả cận lâm sàng"); guard `if (isViewOnly) return;` đầu `btnSave_Click`; không popup "Bắt buộc chọn Hồ sơ điều trị" khi chỉ xem. Resource 3 ngôn ngữ `frmContentSubclinical.Text.ViewOnly` + `btnClose.Text`. Sửa kèm: `SetActionCancelChooseTemplate` → reflection (mismatch DLL máy backup); xóa entry `licenses.licx` stale khỏi csproj. |
| 13/10/2026 | dangth2 | Việc 57966 (BV Nguyễn Tri Phương): cột Ngày KQ của xét nghiệm trả từ LABCON. Dòng chỉ số: y lệnh Hoàn thành + chỉ số có giá trị → `RESULT_TIME` của chỉ số, không có thì `FINISH_TIME` y lệnh; chưa HT/hủy HT/chưa giá trị → trống. Dòng dịch vụ XN = giờ lớn nhất của chỉ số con (bỏ `EXT.END_TIME` với XN). Loại CLS khác giữ nguyên. Chỉ sửa `frmContentSubclinical__Pluss__Load.cs`: `LoadDataService`/`LoadDataMicrobiologicalResult` nhận `Dictionary<long, HIS_SERVICE_REQ> serviceReqById` (dựng 1 lần trong `LoadDataSS`) thay cho `List<HIS_SERVICE_REQ> rsServiceReq` (tra y lệnh O(1) thay `rsServiceReq.FirstOrDefault`, dùng cho cả nhánh kết luận lẫn Ngày KQ); thêm `GetTestIndexResultTime`, `GetSereServTeinResultTime` (đọc `RESULT_TIME` qua reflection, chịu được MOS.EFMODEL cũ), `ResultTimeToString`. Không thêm API, không đổi resource, không đổi dữ liệu trả về cho màn gọi. Phiếu Mps000014 in `FINISH_TIME` nên hiện cùng nguồn với màn (MOS chưa ghi `RESULT_TIME`); không sửa phiếu (xem mục 7). |

## 9. Test Cases

### Ngày KQ xét nghiệm (việc 57966)
- [ ] XN Hoàn thành, LABCON trả đủ (Điện giải đồ NATRI/KALI/CLO) → mỗi dòng chỉ số + dòng dịch vụ = Thời gian kết thúc y lệnh (`dd/MM/yyyy HH:mm:ss`), khớp màn Sửa thông tin y lệnh
- [ ] XN Đang xử lý (chưa có hoặc đã có giá trị) → trống; chỉ số chưa có giá trị trong y lệnh Hoàn thành → riêng dòng đó trống
- [ ] Hủy hoàn thành → trống; Hoàn thành lại → giờ mới; sửa Thời gian kết thúc ở Sửa thông tin y lệnh → Ngày KQ đổi theo
- [ ] XN cũ đã hoàn thành từ trước (lọc Ngày chỉ định từ/đến) → hiện giờ, không cần nhập bù
- [ ] Thủ thuật/CĐHA giữ nguyên giờ như trước; bộ lọc Trên/Dưới ngưỡng, Chỉ số quan trọng, Hiển thị DV cha vẫn hiện giờ
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
