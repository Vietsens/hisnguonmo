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

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ | Table | Y lệnh của hồ sơ |
| HIS_SERE_SERV | Table | Dịch vụ đã thực hiện (7 loại: CĐHA, NS, SA, TDCN, XN, TT, GPBL) |
| HIS_SERE_SERV_EXT | Table | Ghi chú / nhận xét / kết luận |
| V_HIS_SERE_SERV_TEIN | View | Chỉ số xét nghiệm + khoảng tham chiếu |
| V_HIS_SERVICE, V_HIS_TEST_INDEX, HIS_TEST_INDEX_GROUP | Cache/API | Danh mục |
| LIS_SAMPLE / LIS_SAMPLE_SERVICE / LIS_RESULT | LIS | Kháng sinh đồ (khi tích chọn) |

## 4. UI Layout

- Cây `treeListServiceReq` (TreeList, `ShowCheckBoxes = true`; chế độ chỉ xem tắt checkbox) — cột: Kết quả, SRI, Ghi chú, Ngày KQ.
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

Lưu ý build máy backup: `RichEditorStore.SetActionCancelChooseTemplate` gọi qua **reflection** (`PrintKetQuaProcessor.cs:157`) vì DLL `Inventec.Common.RichEditor` trên máy backup cũ hơn source — máy có DLL mới vẫn gắn callback như bản gốc, DLL cũ thì bỏ qua.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 13/08/2026 | nampp | Việc 3170 (BV Điện Biên): Thêm chế độ CHỈ XEM. Behavior nhận cờ `List<string>` chứa `ARG__VIEW_ONLY`, nhánh mới không đòi `DelegateSelectData` (đặt SAU các nhánh cũ — không ảnh hưởng luồng cũ). Form: constructor `(Module, long, bool isViewOnly)` + `ApplyViewOnlyMode()` (ẩn checkbox cây, ẩn btnSave + 6 tuỳ chọn chèn qua `LayoutVisibility.Never`, vô hiệu `barButtonItemSave` Ctrl+S, thêm `btnClose` runtime, tiêu đề "Kết quả cận lâm sàng"); guard `if (isViewOnly) return;` đầu `btnSave_Click`; không popup "Bắt buộc chọn Hồ sơ điều trị" khi chỉ xem. Resource 3 ngôn ngữ `frmContentSubclinical.Text.ViewOnly` + `btnClose.Text`. Sửa kèm: `SetActionCancelChooseTemplate` → reflection (mismatch DLL máy backup); xóa entry `licenses.licx` stale khỏi csproj. |

## 9. Test Cases

### Chế độ chỉ xem (việc 3170)
- [ ] Mở từ nút "Kết quả CLS" màn Buồng bệnh → cây kết quả + bộ lọc + màu chỉ số bất thường giống hệt màn trong tờ điều trị
- [ ] Không có ô tích trên cây, không có nút "Chọn (Ctrl S)", không có 6 tuỳ chọn chèn; Ctrl+S không làm gì
- [ ] Có nút "Đóng" cạnh "In kết quả"; In kết quả hoạt động bình thường
- [ ] BN chưa có kết quả → cây rỗng, không popup lỗi
- [ ] Đổi ngôn ngữ vi/en → tiêu đề form + nút Đóng đúng ngôn ngữ

### Hồi quy chế độ cũ
- [ ] Mở từ tờ điều trị → đủ ô tích + nút Chọn + 6 tuỳ chọn; chèn kết quả vào tờ điều trị như cũ
- [ ] 2 chế độ headless (nhóm chỉ số KSK, serviceIds) chạy đúng
