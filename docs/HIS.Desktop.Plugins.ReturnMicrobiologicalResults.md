# Trả Kết Quả Vi Sinh — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ReturnMicrobiologicalResults |
| Loại | UC (UserControlBase) — mở dạng tab |
| Module | 1961 |
| Mục đích | Màn hình Trả kết quả vi sinh cho Phòng Xét nghiệm: nhập/duyệt/trả kết quả vi khuẩn định danh + kháng sinh đồ (KSĐ), in phiếu kết quả và barcode, ký số EMR |
| Processor | ConnectionTestProcessor (Run → ConnectionTestFactory → ConnectionTestBehavior → UC_ReturnMicrobiologicalResults) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Lọc mẫu theo khoảng thời gian yêu cầu / barcode / mã y lệnh / khoa / phòng → **Danh sách y lệnh** (grid trái).
2. Chọn mẫu → nạp **Dịch vụ** + **Kháng sinh đồ** (grid phải): vi khuẩn định danh, kháng sinh, kỹ thuật, kết quả (R/S/I), SRI.
3. Nhập kết quả → **Lưu (Ctrl S)** (`btnSave`).
4. **Duyệt** mẫu (DuyetE) → có thể **Hủy duyệt** (HuyDuyetE).
5. **Trả kết quả** (TraKetQuaE) → gọi `/api/LisSample/ReturnResult`, chuyển trạng thái mẫu sang `ID__TRA_KQ` (trả kết quả toàn phần). Có thể **Hủy trả KQ** (HuyTraKQE) → về `ID__CO_KQ`.
6. **In (Ctrl P)** (`btnPrint`) → in Phiếu kết quả xét nghiệm (tách theo nhóm dịch vụ).

### Sơ đồ trạng thái mẫu (LIS_SAMPLE_STT)
```
Chưa LM → Có KQ → (Duyệt) → Đã trả KQ
   ↑        ↑                    │
Từ chối   Hủy duyệt          Hủy trả KQ
```

### Điều kiện nghiệp vụ
- Trả kết quả kiểm tra thời gian: lấy mẫu ≤ duyệt mẫu ≤ trả kết quả; thời gian trả ≥ thời gian y lệnh (theo config `StartTimeMustBeGreaterThanInstructionTime`).
- Cảnh báo/chặn khi chỉ số chưa có kết quả (`IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT`).
- Kiểm tra thời gian xử lý tối đa dịch vụ (`PROCESS_TIME_MUST_BE_LESS_THAN_MAX_TOTAL_PROCESS_TIME`).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| LIS_SAMPLE / V_LIS_SAMPLE / V_LIS_SAMPLE_2 | Table/View | Mẫu xét nghiệm (trạng thái, barcode, thời gian) |
| LIS_SAMPLE_SERVICE / V_LIS_SAMPLE_SERVICE | Table/View | Dịch vụ trong mẫu |
| LIS_RESULT / V_LIS_RESULT | Table/View | Kết quả chỉ số |
| LIS_ANTIBIOTIC / LIS_BACTERIUM / LIS_ANTIBIOTIC_RANGE | Table | Kháng sinh, vi khuẩn, dải kháng sinh (KSĐ) |
| LIS_PATIENT_CONDITION | Table | Điều kiện bệnh nhân |
| HIS_SERVICE_REQ / V_HIS_SERVICE_REQ | Table/View | Y lệnh dịch vụ |
| HIS_SERE_SERV / V_HIS_SERVICE | Table/View | Dịch vụ thực hiện / danh mục dịch vụ |
| HIS_TREATMENT / HIS_PATIENT | Table | Điều trị / bệnh nhân (dựng dữ liệu in) |

## 4. UI Layout

```
+-----------------------------------------------------------------------+
| Lọc: [TG YC từ][đến][Barcode mới nhất][Mã y lệnh][Tất cả][Tìm Ctrl F] |
| [Khoa][Phòng]  [] Sx theo hẹn trả                                     |
+------------------------+----------------------------------------------+
| Danh sách y lệnh       | Mã BN / Họ tên / Giới tính / Ngày sinh       |
| STT|Mã YL|Barcode|Tên  | Dịch vụ | Máy | Số hiệu mẫu | Kết quả | ...   |
|                        | Kháng sinh đồ (vi khuẩn → kháng sinh: R/S/I) |
+------------------------+----------------------------------------------+
| Ngày lấy mẫu | Ngày trả KQ | Người trả KQ | ...  TG thực hiện          |
| []Trình ký []Ký                      [Lưu (Ctrl S)] [In (Ctrl P)]      |
+-----------------------------------------------------------------------+
```

### Control thanh thao tác dưới (layoutControl2 / Root)
| Control | Vai trò |
|---------|---------|
| `chkSignProcess` ("Trình ký") | Tự tạo văn bản + trình ký theo luồng "Thiết lập ký" sau khi lưu |
| `chkSign` ("Ký") | Ký số EMR |
| `btnCreateSigner` | Thiết lập luồng ký |
| `btnSave` (Lưu Ctrl S) / `btnPrint` (In Ctrl P) | Lưu / In |

> **PTTK 47031 (v2.0)**: KHÔNG thêm control mới trên màn. Việc tự mở xem trước sau khi trả kết quả toàn phần được bật/tắt bằng **key cấu hình** (xem mục 7), trigger là chính nút "Trả kết quả" sẵn có trên dòng y lệnh.

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Trả kết quả toàn phần | /api/LisSample/ReturnResult | LisConsumer |
| Lấy mẫu | api/LisSample/Get, api/LisSample/GetView | LisConsumer |
| Cập nhật mẫu (mã VB điện tử) | api/LisSample/Update | LisConsumer |
| Barcode mới nhất | api/LisSample/GetByBarcodeLatest | LisConsumer |
| Đặt trạng thái lấy mẫu | api/LisSample/Sample | LisConsumer |
| Dịch vụ trong mẫu | api/LisSampleService/GetView, api/LisSampleService/get | LisConsumer |
| Điều kiện BN | api/LisPatientCondition/Get | LisConsumer |
| Y lệnh / điều trị / bệnh nhân | api/HisServiceReq/Get(View), HIS_TREATMENT_GET, HIS_PATIENT_GET, api/HisSereServ/Get | MosConsumer |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| EmrGenerate | `GenerateInputADOWithPrintTypeCode` — tạo InputADO ký số EMR khi in |
| SignLibrary (SignLibraryGUIProcessor) | Ký số & xem trước bản ký |
| Bartender.PrintClient | In barcode qua Bartender |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args |
|-------------|-----------|------|
| LIS.Desktop.Plugins.SampleInfo | Mẫu chưa làm/từ chối, config `SHOW_FORM_SAMPLE_INFO=1` | rowSample, Module |
| Inventec.Desktop.Plugins.PrintLog | Ghi log in (ShowPrintLog) | PrintLogADO |

## 7. Print

| Loại in | PrintTypeCode | PDO | Ghi chú |
|---------|--------------|-----|---------|
| Phiếu kết quả xét nghiệm | Mps000341 | MPS.Processor.Mps000341.PDO.Mps000341PDO | In tách theo nhóm dịch vụ (`PRINT_OPTION.IN_TACH_THEO_NHOM`) hoặc theo dịch vụ đang chọn (`PRINT_OPTION.IN`). Chế độ: EmrShow / PrintNow / ShowDialog / ký EMR tùy config `IS_USE_SIGN_EMR`, `CheDoInChoCacChucNangTrongPhanMem`, `chkSign`, `chkSignProcess` |
| Barcode | Mps000077 | MPS.Processor.Mps000077.PDO.Mps000077PDO | Hoặc in qua Bartender (`PRINT_BARCODE_BY_BARTENDER=1`) |

### Xem in KQ sau khi trả kết quả (PTTK 47031 — v2.0, config-key)
**Key cấu hình (HIS_CONFIG):** `HIS.Desktop.Plugins.ReturnMicrobiologicalResults__PreviewResultAfterReturn`
- `= "1"` → BẬT: sau khi trả kết quả toàn phần thành công tự mở xem trước phiếu KQ của mẫu vừa trả.
- khác `"1"` / trống → giữ nguyên hành vi hiện tại. **Mặc định TẮT.**
- Nạp trong `HisConfigCFG.LoadConfig()` → field `HisConfigCFG.PREVIEW_RESULT_AFTER_RETURN`.

**Luồng:**
- Trigger = chính nút "Trả kết quả" sẵn có trên dòng y lệnh (`TraKetQuaE_Click`). KHÔNG thêm control mới trên màn.
- Ngay sau khi `/api/LisSample/ReturnResult` thành công và **trước** `FillDataToGridControl()` (giữ nguyên focused row + `lstSampleServiceADOs` của mẫu vừa trả), nếu config = "1" → gọi `PreviewKetQuaXetNghiemAfterReturnResult()`.
- `PreviewKetQuaXetNghiemAfterReturnResult()` (partial `UC_UC_ReturnMicrobiologicalResults_PlusPrint.cs`) đặt cờ `isPreviewAfterReturnResult=true`, `PrintOption=IN_TACH_THEO_NHOM`, gọi Mps000341.
- Trong `LoadBieuMauInKetQuaXetNghiemV2`, khi cờ bật → **luôn** `PreviewType.ShowDialog` (xem trước), bỏ qua PrintNow và luồng tự ký EMR (chkSign/chkSignProcess). Cửa sổ xem trước vẫn có nút ký số EMR để người dùng chủ động ký.
- Chỉ áp dụng trả kết quả toàn phần; trả từng phần không áp dụng.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 20/07/2026 | tuanln | **PTTK 47031 (BV HAGL) — v1**: Thêm ô tích "Xem in KQ" ở thanh thao tác dưới (đã thay bằng v2). |
| 21/07/2026 | tuanln | **PTTK 47031 (BV HAGL) — v2.0 (config-key)**: Bỏ ô tích "Xem in KQ"; thay bằng key cấu hình `HIS.Desktop.Plugins.ReturnMicrobiologicalResults__PreviewResultAfterReturn` (="1" bật / khác 1 giữ nguyên, mặc định TẮT). Khi bật, sau khi trả kết quả toàn phần thành công (nút "Trả kết quả" trên dòng y lệnh) → tự mở xem trước Phiếu KQ (Mps000341) của mẫu vừa trả (luôn ShowDialog, không in thẳng, không tự đẩy vào luồng ký EMR). |

## 9. Test Cases

- [ ] Config `PreviewResultAfterReturn = "1"` + trả kết quả toàn phần **thành công** → tự mở xem trước Phiếu KQ của mẫu vừa trả; nội dung phiếu giống hệt khi bấm nút In.
- [ ] Config = "1" + trả kết quả **thất bại** → không mở xem trước; thông báo lỗi như hiện tại.
- [ ] Config khác "1" / trống (mặc định) → không mở xem trước (giữ hành vi hiện tại).
- [ ] Trả kết quả **từng phần** → không mở xem trước (ngoài phạm vi).
- [ ] Cửa sổ xem trước tự mở là ShowDialog; KHÔNG in thẳng dù `CheDoInChoCacChucNangTrongPhanMem=2`; KHÔNG tự đẩy vào luồng ký EMR dù `IS_USE_SIGN_EMR=1`/đang tích Ký/Trình ký.
- [ ] KHÔNG xuất hiện control mới nào trên màn (không còn ô tích "Xem in KQ").
