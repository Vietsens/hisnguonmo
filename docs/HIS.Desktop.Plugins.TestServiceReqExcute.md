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
4. **Kết thúc** (`btnFinish_Click`, dòng 1811): `CheckResultFinish()` → `ValidateSampleTimeVsInstruction()` → `ValidTimeReturn()` → *(việc 3353)* `CheckRequireMediMateManager.CheckBeforeFinish(lstSereServ)` → gán `EXECUTE_LOGINNAME/USERNAME`, `START_TIME` (theo key `NgayThYlOption` / `TestStartTimeOption`), `FINISH_TIME` (từ `dtTimeReturn`) → POST `api/HisServiceReq/FinishWithTime`.
5. In phiếu kết quả xét nghiệm (`PrintProcess(PrintTypeTest.IN_PHIEU_KET_QUA_XET_NGHIEM)`).

### Điều kiện nghiệp vụ
- Thời gian lấy mẫu không nhỏ hơn thời gian y lệnh; thời gian trả kết quả không nhỏ hơn thời gian y lệnh.
- **Việc 3353 (18/09/2026):** dịch vụ XN có `HIS_SERVICE.IS_REQUIRE_MEDI_MATE = 1` mà chưa có thuốc/vật tư đi kèm (dòng `HIS_SERE_SERV` con loại Thuốc/VT, `PARENT_ID` = ID dịch vụ) → hỏi Yes/No (mặc định No) trước khi kết thúc; không chặn.

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
| HIS.Desktop.Plugins.Library.CheckRequireMediMate | Việc 3353 — cảnh báo dịch vụ chưa có thuốc, vật tư đi kèm trước khi kết thúc |

## 7. Print

Phiếu kết quả xét nghiệm — `PrintProcess(PrintTypeTest.IN_PHIEU_KET_QUA_XET_NGHIEM)`.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 18/09/2026 | dangth2 | Tạo tài liệu module. **Việc 3353 (PT-56272):** trong `btnFinish_Click` (`UCTestServiceReqExcute.cs`), sau `ValidTimeReturn()` và trước `WaitingManager.Show()`, thêm lời gọi `HIS.Desktop.Plugins.Library.CheckRequireMediMate.CheckRequireMediMateManager.CheckBeforeFinish(lstSereServ)`; No → `return`. Thêm reference thư viện `HIS.Desktop.Plugins.Library.CheckRequireMediMate` (HintPath lib\HIS). Không key config, không chặn, lỗi API thì bỏ qua. |

## 9. Test Cases

- [ ] Kết thúc y lệnh XN bình thường (dịch vụ không bật cờ) → không có hộp thoại mới.
- [ ] (3353) dịch vụ XN bật cờ, chưa kê vật tư → hộp thoại; No → chưa hoàn thành; Yes → hoàn thành.
- [ ] Thời gian trả kết quả < thời gian y lệnh → chặn như cũ (kiểm tra 3353 chạy sau kiểm tra này).
