# Kết nối xét nghiệm (ConnectionTest) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ConnectionTest |
| Loại | UC (MODULE_TYPE_ID__UC) |
| Mục đích | Màn hình kết nối/xử lý xét nghiệm tại phòng XN: lọc mẫu, lấy mẫu, nhập–trả–duyệt kết quả, ký số, in phiếu kết quả. |
| Entry point | `ConnectionTestProcessor` → `ConnectionTestFactory` → `ConnectionTestBehavior` → `UC_ConnectionTest` |
| Trạng thái | Bảo trì |

UI chính `UC_ConnectionTest` được tách nhiều partial:
- `UC_ConnectionTest.cs` (core, load, filter, grid mẫu, nhập/trả/duyệt KQ)
- `UC_ConnectionTest_PlusPrint.cs` (in barcode, in phiếu KQ 1 mẫu Mps000096…)
- `UC_ConnectionTest__CheckAll.cs` (chọn tất cả trên header grid mẫu)
- `UC_ConnectionTest___NhomXN.cs` (**mới** — bộ lọc Nhóm XN)
- `UC_ConnectionTest___InKQTongHop.cs` (**mới** — in KQ tổng hợp nhiều mẫu)

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
Lọc mẫu theo TG yêu cầu / TT mẫu / Khoa / Phòng / **Nhóm XN** → grid mẫu (`gridControlSample`) → chọn 1 dòng để xem/nhập kết quả từng chỉ số → trả KQ → duyệt KQ (đơn lẻ hoặc theo lô) → ký số / in phiếu.

### Bộ lọc Nhóm XN (mục B.4.1 — bổ sung)
- Khi mở/lọc lại danh sách mẫu: nạp dịch vụ XN của các mẫu trong phiên, gom theo **dịch vụ cha** (`V_HIS_SERVICE.PARENT_ID`). Dịch vụ không có `PARENT_ID` gom vào mục **"Khác"** (cuối danh sách).
- Tích ≥1 nhóm → lọc `gridControlSample` theo **logic OR**: chỉ hiển thị mẫu chứa ≥1 dịch vụ thuộc bất kỳ nhóm đã tích.
- Control hiển thị tên các nhóm đã tích, phân cách bằng `;` (VD: "Sinh hóa;Huyết học").
- Bỏ tích tất cả / tích tất cả → bỏ lọc, hiển thị toàn bộ mẫu.
- Reload dữ liệu mẫu → **giữ nguyên** các nhóm đang chọn và áp dụng lại filter.

### In KQ tổng hợp (mục B.4.1 — bổ sung)
Nút **"In KQ tổng hợp"** (`btnInKetQuaTongHop`) xử lý các mẫu đã tích (`LisSampleADO.IsCheck`) theo thứ tự kiểm tra:
1. Chưa chọn mẫu nào → cảnh báo "Chưa chọn mẫu xét nghiệm" → dừng.
2. Mẫu khác bệnh nhân (theo `PATIENT_CODE` — *V_LIS_SAMPLE không có TDL_PATIENT_ID*) → cảnh báo "không cùng một bệnh nhân" → dừng.
3. Có mẫu chưa có kết quả (không có bản ghi `V_LIS_RESULT`) → cảnh báo "có mẫu chưa có kết quả" → dừng.
4. Đạt tất cả → dựng `Mps000517PDO` và gọi biểu in `Mps000517` (preview hoặc in trực tiếp theo `GlobalVariables.CheDoInChoCacChucNangTrongPhanMem`).

### Cột "KQ từ máy" (bổ sung)
Cột `colMachineResult` ("KQ từ máy", tooltip "Kết quả từ máy xét nghiệm") nằm giữa "Máy trả KQ" và "ĐVT" trên treeList chỉ số (`treeListSereServTein`, FieldName `MACHINE_RESULT_VALUE`, read-only).
- Trạng thái **ẩn/hiện + vị trí + độ rộng** cột được lưu/khôi phục qua `InitRestoreLayoutTreeListFromXml(treeListSereServTein)` — chỉ bật khi config `HIS.Desktop.ApplyRestoreLayout.ModuleLinks` chứa `HIS.Desktop.Plugins.ConnectionTest`.
- Config `HIS.Desktop.Plugins.ConnectionTest.IsResultLisMachine`:
  - **= 1**: lấy giá trị "Kết quả" từ màn hình "Trả kết quả xét nghiệm từ máy" (`LIS.Desktop.Plugins.LisMachineResult`) theo **mã y lệnh** (`SERVICE_REQ_CODE`) của mẫu đang chọn, map **chỉ số máy → chỉ số xét nghiệm** qua `V_LIS_TEST_INDEX_MAP` (MACHINE_INDEX_CODE → TEST_INDEX_CODE), rồi điền vào cột theo `TEST_INDEX_CODE`. Phiếu máy mới nhất (theo `CREATE_TIME`) ghi đè khi trùng chỉ số.
  - **≠ 1**: để trống (giữ nguyên hành vi hiện tại — chỉ có giá trị nếu người dùng duyệt KQ).
- Logic: `FillMachineResultValueFromLisMachine(List<TestLisResultADO>)` gọi trong `LoadDataToGridTestResult2()` trước khi bind treeList.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_LIS_SAMPLE | View (LIS) | Dòng mẫu trên grid (ADO `LisSampleADO`); có `ID`, `SERVICE_REQ_CODE`, `PATIENT_CODE`, `BARCODE`, `SAMPLE_STT_ID` |
| V_LIS_SAMPLE_SERVICE | View (LIS) | Dịch vụ của từng mẫu (`SAMPLE_ID`, `SERVICE_CODE`) — dùng cho bộ lọc Nhóm XN |
| V_LIS_RESULT | View (LIS) | Kết quả từng chỉ số của mẫu (`SAMPLE_ID`, `SERVICE_CODE`) — kiểm tra "đã có KQ" + dữ liệu in |
| V_LIS_MACHINE_RESULT | View (LIS) | Phiếu kết quả từ máy (`ID`, `SERVICE_REQ_CODE`, `BARCODE`, `CREATE_TIME`) — cột "KQ từ máy" |
| V_LIS_MACHINE_INDEX_RESULT | View (LIS) | Giá trị chỉ số máy (`MACHINE_RESULT_ID`, `MACHINE_INDEX_CODE`, `VALUE`) — cột "KQ từ máy" |
| V_LIS_TEST_INDEX_MAP | View (LIS) | Ánh xạ chỉ số máy ↔ chỉ số XN (`MACHINE_INDEX_CODE`, `TEST_INDEX_CODE`) — cache RAM |
| V_HIS_SERVICE | View (MOS) | Danh mục dịch vụ; `SERVICE_CODE`, `PARENT_ID`, `SERVICE_NAME` — gom nhóm cha |
| HIS_SERVICE_REQ | Table (MOS) | Yêu cầu dịch vụ (`TREATMENT_ID`, `TDL_PATIENT_ID`, `INTRUCTION_TIME`, `REQUEST_ROOM_ID`) |
| HIS_TREATMENT / HIS_PATIENT / HIS_PATIENT_TYPE_ALTER | Table (MOS) | Thông tin điều trị / bệnh nhân / đối tượng BHYT cho phiếu in |
| V_HIS_TEST_INDEX / V_HIS_TEST_INDEX_RANGE | View (MOS) | Chỉ số XN + dải tham chiếu (cache RAM) |
| HIS_SERE_SERV / V_HIS_TREATMENT_BED_ROOM | Table/View (MOS) | Dịch vụ thực hiện + giường-phòng cho phiếu in |
| LIS_SAMPLE_TYPE / HIS_TEST_SAMPLE_TYPE | Table | Loại mẫu (theo cấu hình tích hợp PACS/LIS) |

## 4. UI Layout

### Bổ sung
| Control | Loại | Vị trí | Ghi chú |
|---------|------|--------|---------|
| `cboServiceGroup` ("Nhóm XN:") | CheckedComboBoxEdit | Khu filter, cạnh "Phòng" (`lciServiceGroup`) | Multi-select; `SeparatorChar=';'`, `SelectAllItemVisible=true` ("(Tất cả)") |
| `btnInKetQuaTongHop` ("In KQ tổng hợp") | SimpleButton | Nhóm nút dưới-trái, cạnh "Duyệt KQ theo lô" (`lciInKetQuaTongHop`) | — |

Cả hai hiển thị mặc định, **không cần config bật/tắt**.

### Đa ngôn ngữ
Khai báo trong `Resources/Lang.vi|en|my.resx`:
`UC_ConnectionTest.btnInKetQuaTongHop.Text/.ToolTip`, `UC_ConnectionTest.lciServiceGroup.Text` — set trong `SetCaptionByLanguageKey()`.
Thông báo riêng trong `Resources/Message.Lang.*` + accessor `ResourceMessage.cs`:
`ChuaChonMauXetNghiem`, `MauKhongCungMotBenhNhan`, `CoMauChuaCoKetQua` ({0}=barcode), `NhomXNKhac`.

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Danh sách mẫu | api/LisSample/GetView | LisConsumer | `LisSampleViewFilter` |
| **Dịch vụ theo mẫu (Nhóm XN)** | api/LisSampleService/GetView | LisConsumer | `LisSampleServiceViewFilter { SAMPLE_IDs }` |
| Kết quả theo mẫu | api/LisResult/GetView | LisConsumer | `LisResultViewFilter { SAMPLE_ID }` |
| **Phiếu KQ từ máy (KQ từ máy)** | api/LisMachineResult/GetView | LisConsumer | `LisMachineResultViewFilter { KEY_WORD }` — lọc chính xác lại theo `SERVICE_REQ_CODE` ở client |
| **Chỉ số KQ từ máy (KQ từ máy)** | api/LisMachineIndexResult/GetView | LisConsumer | `LisMachineIndexResultViewFilter { MACHINE_RESULT_ID }` |
| Yêu cầu DV | api/HisServiceReq/Get | MosConsumer | `HisServiceReqFilter { SERVICE_REQ_CODE__EXACT }` |
| Điều trị / Bệnh nhân / Đối tượng | api/HisTreatment/Get · api/HisPatient/Get · api/HisPatientTypeAlter/GetLastByTreatmentId | MosConsumer | ID / treatmentId |
| Giường-phòng / DV thực hiện | api/HisTreatmentBedRoom/GetView · api/HisSereServ/Get | MosConsumer | TREATMENT_ID / TDL_SERVICE_REQ_CODE_EXACT |

Danh mục dùng cache RAM `BackendDataWorker.Get<V_HIS_SERVICE>()`, `<V_HIS_TEST_INDEX>`, `<V_HIS_TEST_INDEX_RANGE>`, `<V_LIS_TEST_INDEX_MAP>` (ánh xạ chỉ số máy ↔ chỉ số XN).

## 6. Dependencies

- **Inter-plugin**: LIS.Desktop.Plugins.SampleInfo (form thông tin mẫu), EmrGenerate (ký số).
- **Library**: `HIS.Desktop.Plugins.Library.EmrGenerate` (ký số EMR khi in 1 mẫu).

## 7. Print

| Loại in | PrintTypeCode | PDO | Template |
|---------|--------------|-----|----------|
| In barcode | Mps000077 | Mps000077PDO | — |
| In gộp barcode | Mps000496 | Mps000496PDO | — |
| KQ XN 1 mẫu | Mps000096 | Mps000096PDO | — |
| **KQ XN tổng hợp nhiều mẫu** | **Mps000517** | **Mps000517PDO** | `Mps000517_KQXN_TongHopDaMau.xlsx` |

Luồng in tổng hợp: `RichEditorStore.RunPrintTemplate("Mps000517", DelegateRunPrinterTongHop)` → `MpsPrinter.Run(PrintData)` (PreviewType `PrintNow`/`ShowDialog`).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 26/06/2026 | tuanln | **B.4.1 (v42696)**: Thêm bộ lọc multi-select "Nhóm XN" (`cboServiceGroup`) lọc grid mẫu theo nhóm dịch vụ cha (logic OR, giữ chọn khi reload); thêm nút "In KQ tổng hợp" (`btnInKetQuaTongHop`) — validate chọn mẫu/cùng BN/có KQ rồi in `Mps000517`. Bổ sung 2 partial `UC_ConnectionTest___NhomXN.cs`, `UC_ConnectionTest___InKQTongHop.cs`, reference `MPS.Processor.Mps000517.PDO`, resource đa ngôn ngữ. |
| 15/07/2026 | phuongnm | Thêm cột **"KQ từ máy"** (`colMachineResult`, FieldName `MACHINE_RESULT_VALUE`) trên treeList chỉ số, đặt giữa "Máy trả KQ" và "ĐVT". Lưu ẩn/hiện cột qua `InitRestoreLayoutTreeListFromXml` (key `HIS.Desktop.ApplyRestoreLayout.ModuleLinks`). Khi config `HIS.Desktop.Plugins.ConnectionTest.IsResultLisMachine=1`: lấy KQ từ máy (`V_LIS_MACHINE_INDEX_RESULT.VALUE`) map sang chỉ số XN theo `V_LIS_TEST_INDEX_MAP` theo mã y lệnh (`FillMachineResultValueFromLisMachine`); ≠1: để trống. Thêm field ADO `MACHINE_RESULT_VALUE`, config `HisConfigCFG.IsResultLisMachine`, resource caption/tooltip vi/en/my. |

## 9. Test Cases

### Bộ lọc Nhóm XN
- [ ] Mở màn hình → dropdown "Nhóm XN" hiển thị các nhóm cha của mẫu trong phiên + "Khác" (nếu có DV không PARENT_ID).
- [ ] Tích 1 nhóm → chỉ còn mẫu thuộc nhóm đó; text combo = tên nhóm.
- [ ] Tích 2 nhóm → hiển thị mẫu thuộc bất kỳ nhóm nào (OR); text = "A;B".
- [ ] Bỏ tích hết / tích hết → hiển thị toàn bộ mẫu.
- [ ] Đổi filter ngày rồi Tìm lại → các nhóm đang chọn được giữ và áp dụng lại.

### In KQ tổng hợp
- [ ] Không tích mẫu → cảnh báo "Chưa chọn mẫu xét nghiệm".
- [ ] Tích mẫu của 2 BN khác nhau → cảnh báo "không cùng một bệnh nhân".
- [ ] Tích mẫu có mẫu chưa có KQ → cảnh báo "có mẫu chưa có kết quả" (kèm barcode).
- [ ] Tích nhiều mẫu cùng BN, đều có KQ → mở preview/in `Mps000517` đúng dữ liệu.
