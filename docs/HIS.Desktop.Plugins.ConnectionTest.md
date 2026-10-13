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
- `UC_ConnectionTest___CG300.cs` (**mới** — quét barcode 2 lần để in tem + nhả ống qua máy lấy máu FOYEN CG300) + thư mục `CG300/`

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

### In tem qua máy CG300 (Method 2 — trao đổi file)
Chi tiết phân tích/thiết kế: `docs/B_KyThuat_ConnectionTest_InBarcodeCG300.md`.
- Chỉ áp dụng cho máy trạm khai báo config `CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_FOLDER` (thư mục `ALL` mà phần mềm CG đọc). Checkbox **"In tem CG300"** (tạo bằng code, cạnh ô tìm kiếm) **chỉ hiện khi kết nối được**: thư mục tồn tại + ghi được + tiến trình CG đang chạy (nếu khai báo `..._CG300_PROCESS_NAME`). Kiểm tra lúc load, khi bấm Tìm và trước mỗi lần ghi file.
- Khi checkbox hiện **và** được tích: Enter ở ô tìm kiếm (máy quét gửi Enter) → mã **khác** lần quét trước ⇒ tìm; mã **trùng** lần quét trước và lưới có dữ liệu ⇒ in. Con trỏ luôn về ô tìm kiếm, nội dung bôi đen.
- In: lấy dịch vụ của các mẫu đang hiển thị → loại mẫu bệnh phẩm gán cho dịch vụ (`HIS_SERVICE_TESA`, cấu hình ở HisServiceTestSample) → `ContainerCode = TEST_SAMPLE_TYPE_CODE`. Mỗi (barcode × ống) = 1 bản ghi JSON `FyLabelResponseModel`; ghi `{barcode}_{time}.tmp` rồi đổi sang `.txt` trong thư mục `ALL`. Tên loại mẫu dạng `"Màu - Nguồn"` (VD `Purple - EDTA`) được tách thành `Container`/`Source`.
- Sau khi ghi file: mẫu Chưa LM/Từ chối → gọi `api/LisSample/Sample` (hàm dùng chung `UpdateSampleTakenAfterPrint`, tôn trọng `LIS.LIS_SAMPLE.ALLOW_AUTO_SAMPLE_AFTER_PRINT_BARCODE`), cập nhật dòng tại chỗ, **không reload lưới**.
- Cảnh báo (không chặn các ống hợp lệ): mẫu chưa có barcode, dịch vụ chưa gán loại mẫu. Barcode đã gửi trong phiên → hỏi xác nhận in lại.

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

| HIS_TEST_SAMPLE_TYPE / HIS_SERVICE_TESA | Table (MOS) | **CG300**: loại mẫu bệnh phẩm (mã = ContainerCode) và gán dịch vụ ↔ loại mẫu — nạp 1 lần khi in |

## 4. UI Layout

### Bổ sung
| Control | Loại | Vị trí | Ghi chú |
|---------|------|--------|---------|
| `cboServiceGroup` ("Nhóm XN:") | CheckedComboBoxEdit | Khu filter, cạnh "Phòng" (`lciServiceGroup`) | Multi-select; `SeparatorChar=';'`, `SelectAllItemVisible=true` ("(Tất cả)") |
| `btnInKetQuaTongHop` ("In KQ tổng hợp") | SimpleButton | Nhóm nút dưới-trái, cạnh "Duyệt KQ theo lô" (`lciInKetQuaTongHop`) | — |
| `chkPrintBarcodeCG300` ("In tem CG300") | CheckEdit (tạo runtime) | Bên phải ô tìm kiếm (`lciPrintBarcodeCG300`) | Chỉ hiện khi kết nối CG300; nhớ trạng thái qua ControlState `chkPrintBarcodeCG300` |

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

| **Loại mẫu bệnh phẩm (CG300)** | api/HisTestSampleType/Get | MosConsumer | `HisTestSampleTypeFilter { IS_ACTIVE = 1 }` — nạp 1 lần |
| **Dịch vụ ↔ loại mẫu (CG300)** | api/HisServiceTesa/Get | MosConsumer | `HisServiceTesaFilter { IS_ACTIVE = 1 }` — nạp 1 lần |
| **Cập nhật đã lấy mẫu sau in tem (CG300/Bartender)** | api/LisSample/Sample | LisConsumer | `LisSampleSampleSDO` |

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

| 13/10/2026 | vuongnd | **In tem barcode qua máy CG300 (Method 2)**: checkbox "In tem CG300" chỉ hiện khi máy trạm kết nối được CG300 (config `..._CG300_FOLDER` / `..._CG300_PROCESS_NAME`); quét mã để tìm, quét lại cùng mã để ghi file JSON `FyLabelResponseModel` vào thư mục `ALL` (ContainerCode = mã loại mẫu bệnh phẩm gán cho dịch vụ). Thêm partial `UC_ConnectionTest___CG300.cs`, thư mục `CG300/` (ADO, builder, file writer, connection checker), `Config/Cg300CFG.cs`; tách hàm dùng chung `UpdateSampleTakenAfterPrint` từ nhánh in Bartender; resource vi/en/my. |

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

### In tem qua CG300
- [ ] Máy trạm không cấu hình thư mục CG300 → không thấy checkbox.
- [ ] Thư mục không tồn tại / không ghi được / phần mềm CG chưa chạy → checkbox ẩn; bật CG rồi bấm Tìm → checkbox hiện, giữ trạng thái tích.
- [ ] Quét mã A → tìm; quét lại A → sinh 1 file `.txt` trong `ALL`, số bản ghi = số ống; lưới không reload; mẫu Chưa LM → Đã LM.
- [ ] Quét mã B ngay sau → tìm B (không in A).
- [ ] Dịch vụ chưa gán loại mẫu / mẫu chưa có barcode → cảnh báo, các ống hợp lệ vẫn gửi.
- [ ] Quét lại A lần 3 → hỏi xác nhận in lại, bản ghi `RePrint = Yes`.
- [ ] Hồi quy: In barcode Bartender vẫn cập nhật trạng thái lấy mẫu như trước.
