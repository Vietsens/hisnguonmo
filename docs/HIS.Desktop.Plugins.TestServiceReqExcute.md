# Xử lý xét nghiệm (TestServiceReqExcute) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TestServiceReqExcute |
| Loại | UserControl (`UCTestServiceReqExcute`) |
| Mục đích | Xử lý y lệnh xét nghiệm tại phòng xét nghiệm: nhập/nhận kết quả chỉ số (`HIS_SERE_SERV_TEIN`), thời gian lấy mẫu / thực hiện / trả kết quả, người thực hiện; Lưu, Kết thúc, In phiếu kết quả. |
| Người tạo | IVT |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở từ Xử lý y lệnh (ExecuteRoom) với y lệnh XN (`currentServiceReq`); nạp dịch vụ của y lệnh `lstSereServ` (`api/HisSereServ/Get` theo `SERVICE_REQ_ID`, dòng 340-346) và chỉ số `lstSereServTein`.
2. Nhập kết quả trên lưới chỉ số (`gridControlSereServTein`, `HisSereServTeinSDO`), chọn người thực hiện (`cboUserAssign`), thời gian (`dtTime`, `dtTimeReturn`).
3. **Lưu** → cập nhật kết quả chỉ số.
4. **Kết thúc** (`btnFinish_Click`, dòng 1810): `CheckResultFinish()` → `ValidateSampleTimeVsInstruction()` → `ValidTimeReturn()` → *(việc 3353)* `CheckRequireMediMateManager.CheckBeforeFinish(lstSereServ)` — trả `false` thì **`return` ngay, không gọi Finish** → gán `EXECUTE_LOGINNAME/USERNAME`, `START_TIME` (theo key `NgayThYlOption` / `TestStartTimeOption`), `FINISH_TIME` (từ `dtTimeReturn`) → POST `api/HisServiceReq/FinishWithTime`.
5. In phiếu kết quả xét nghiệm (`PrintProcess(PrintTypeTest.IN_PHIEU_KET_QUA_XET_NGHIEM)`).

### Điều kiện nghiệp vụ
- Thời gian lấy mẫu không nhỏ hơn thời gian y lệnh; thời gian trả kết quả không nhỏ hơn thời gian y lệnh.
- **Việc 3353 — mức CHẶN (chốt 22/09/2026):** dịch vụ XN có `HIS_SERVICE.IS_REQUIRE_MEDI_MATE = 1` mà chưa có thuốc/vật tư đi kèm (dòng `HIS_SERE_SERV` con loại Thuốc/VT, `PARENT_ID` = ID dịch vụ, chưa hủy và `IS_NO_EXECUTE != 1`) → hiện hộp thông báo **chỉ có nút OK** liệt kê đủ các dịch vụ thiếu, **KHÔNG cho kết thúc**. Câu thông báo: *"Không kết thúc được. Dịch vụ chưa có thuốc, vật tư đi kèm:\n{0}\nVui lòng kê thuốc, vật tư đi kèm cho các dịch vụ trên rồi kết thúc lại."* Đường thoát duy nhất là kê bổ sung thuốc/vật tư gắn vào dịch vụ rồi bấm Kết thúc lại.
- **Fail-open:** lỗi gọi API / mất mạng / Backend cũ chưa có cột `IS_REQUIRE_MEDI_MATE` → **không chặn**, cho kết thúc bình thường, chỉ ghi `LogSystem.Warn`. Viện không tick cờ cho dịch vụ nào thì màn này hoạt động y như cũ.
- ⚠ **Lưu ý riêng màn Xử lý xét nghiệm:** trên chính màn này **chưa có chỗ kê thuốc/vật tư gắn `PARENT_ID` vào dịch vụ** — nút "Kê đơn dược" / "Kê tủ trực" (`UCTestServiceReqExcute.cs:1977`) tạo `AssignPrescriptionADO(TREATMENT_ID, 0, 0)`, thuốc kê ra không gắn dịch vụ nên **không gỡ được chặn ngay tại màn đó**. Muốn gỡ phải ra màn **Xử lý y lệnh** (ExecuteRoom) → chuột phải dòng dịch vụ → **"Kê đơn cận lâm sàng"** (truyền `sereServInput` nên sinh `PARENT_ID`). Vì vậy **khuyến cáo viện chưa tick cờ `IS_REQUIRE_MEDI_MATE` cho dịch vụ xét nghiệm** cho tới khi có chỗ kê gắn dịch vụ ngay trên màn này.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ | Table | Y lệnh đang xử lý (`currentServiceReq`), gửi lên `FinishWithTime` |
| HIS_SERE_SERV | Table | Dịch vụ của y lệnh (`lstSereServ`) |
| V_HIS_SERE_SERV_TEIN | View | Chỉ số xét nghiệm |
| ACS_USER | Table | Người thực hiện (`lstAcsUser`) |

## 4. UI Layout

```
+-----------------------------------------------------------+
| Thông tin y lệnh / bệnh nhân | Người thực hiện | TG lấy mẫu |
+-----------------------------------------------------------+
| Lưới chỉ số xét nghiệm (kết quả, đơn vị, khoảng tham chiếu) |
+-----------------------------------------------------------+
| [Lưu] [Kết thúc] [In]                    TG trả kết quả    |
+-----------------------------------------------------------+
```

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Dịch vụ của y lệnh | `HisRequestUriStore.HIS_SERE_SERV_GET` (api/HisSereServ/Get) | MosConsumer |
| Kết thúc | /api/HisServiceReq/FinishWithTime | MosConsumer |
| (thư viện 3353) cờ dịch vụ / dòng thuốc-VT con | api/HisService/Get, api/HisSereServ/Get | MosConsumer |

## 6. Dependencies

| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.EmrGenerate | Ký số EMR khi in |
| HIS.Desktop.Plugins.Library.CheckRequireMediMate | Việc 3353 — **chặn** kết thúc khi dịch vụ chưa có thuốc, vật tư đi kèm (1 trong 6 màn gọi thư viện) |

## 7. Print

Phiếu kết quả xét nghiệm — `PrintProcess(PrintTypeTest.IN_PHIEU_KET_QUA_XET_NGHIEM)`.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 18/09/2026 | dangth2 | Tạo tài liệu module. **Việc 3353 (PT-56272):** trong `btnFinish_Click` (`UCTestServiceReqExcute.cs`), sau `ValidTimeReturn()` và trước `WaitingManager.Show()`, thêm lời gọi `HIS.Desktop.Plugins.Library.CheckRequireMediMate.CheckRequireMediMateManager.CheckBeforeFinish(lstSereServ)`; No → `return`. Thêm reference thư viện `HIS.Desktop.Plugins.Library.CheckRequireMediMate` (HintPath lib\HIS). Không key config, không chặn, lỗi API thì bỏ qua. |
| 22/09/2026 | dangth2 | **Việc 3353 — đổi từ CẢNH BÁO sang CHẶN** (anh Cảnh chốt): hộp thông báo **chỉ còn nút OK**, bỏ Yes/No nên không còn khái niệm "nút mặc định"; `CheckRequireMediMateManager.ConfirmFinish` **luôn trả `false`** khi còn dịch vụ thiếu → `btnFinish_Click` `return`, **không gọi** `api/HisServiceReq/FinishWithTime`. Đổi câu thông báo thành "Không kết thúc được. Dịch vụ chưa có thuốc, vật tư đi kèm: … Vui lòng kê thuốc, vật tư đi kèm cho các dịch vụ trên rồi kết thúc lại." Áp dụng cho cả 3 nhóm CLS + PTTT + **Xét nghiệm**, tổng cộng **5 màn** (thêm `TestServiceExecute` rà ra ngày 22/09). Lời gọi tại màn này **giữ nguyên**, không phải sửa code plugin — chỉ thư viện đổi hành vi. Giữ nguyên fail-open: lỗi API / Backend cũ chưa có cột → không chặn, ghi `LogSystem.Warn`. Bổ sung khuyến cáo chưa tick cờ cho dịch vụ XN (màn này chưa có chỗ kê gắn `PARENT_ID`). |

## 9. Test Cases

- [ ] Kết thúc y lệnh XN bình thường (dịch vụ không bật cờ) → không có hộp thoại mới, kết thúc như cũ.
- [ ] (3353) dịch vụ XN bật cờ, chưa kê thuốc/vật tư → hộp thông báo **chỉ có nút OK**, nội dung "Không kết thúc được…" liệt kê đủ dịch vụ thiếu; bấm OK → **y lệnh vẫn chưa kết thúc** (không gọi `FinishWithTime`), màn giữ nguyên dữ liệu đang nhập.
- [ ] (3353) nhiều dịch vụ XN cùng thiếu → chỉ hiện **một** hộp thông báo liệt kê đủ các dịch vụ, không báo lặp từng dịch vụ.
- [ ] (3353) kê bổ sung thuốc/vật tư gắn dịch vụ qua màn **Xử lý y lệnh** → chuột phải → "Kê đơn cận lâm sàng", quay lại Kết thúc → kết thúc được bình thường.
- [ ] (3353) kê bằng nút "Kê đơn dược" / "Kê tủ trực" ngay trên màn này → **vẫn bị chặn** (thuốc không gắn `PARENT_ID`) — đúng như khuyến cáo chưa tick cờ cho dịch vụ XN.
- [ ] (3353) fail-open: ngắt mạng / Backend chưa có cột `IS_REQUIRE_MEDI_MATE` → **không chặn**, kết thúc được, log ghi `Warn`.
- [ ] Thời gian trả kết quả < thời gian y lệnh → chặn như cũ (kiểm tra 3353 chạy sau kiểm tra này).
