# Gói dịch vụ bệnh nhân — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisPatientPackage |
| Loại | UC (màn 6.2 Danh sách gói) + Form (màn 6.1 Đăng ký/Sửa — sẽ bổ sung) |
| Mục đích | Quản lý gói dịch vụ "mua trước – dùng sau" của bệnh nhân. Màn 6.2: liệt kê toàn bộ gói (V_HIS_PATIENT_PACKAGE), xóa, khóa/mở khóa, và mở các chức năng Sửa/Thanh toán/Hoàn tiền/In. |
| Người tạo | phuongnm |
| Ngày tạo | 27/05/2026 |
| Trạng thái | Đang phát triển (đã xong 6.2 Danh sách; 6.1 Đăng ký/Sửa là việc tiếp theo) |

## 2. Quy Trình Nghiệp Vụ

### 4 trạng thái hiển thị (màn 6.2) + màu mũi tên
Cột "mũi tên trạng thái" (ngay sau STT) vẽ tam giác đổi màu theo trạng thái (`gridView_CustomDrawCell`):

| Mã hiển thị | Nhãn | Màu mũi tên | Nền dòng |
|---|---|---|---|
| WAITING_PAYMENT | Chờ thanh toán | Cam (240,163,10) | trắng |
| PAID | Đã thanh toán | Xanh lá (46,158,79) | xanh lá nhạt |
| REFUNDED | Đã hoàn tiền | Xanh dương (0,120,215) | xanh dương nhạt |
| CANCELED | Đã hủy | Xám (128,128,128) | xám nhạt |

### Logic ẩn/hiện nút theo trạng thái (`IsActionAllowed`)
| Trạng thái | Sửa | Xóa | In | Thanh toán | Hoàn tiền | Khóa/Mở |
|---|:--:|:--:|:--:|:--:|:--:|:--:|
| Chờ thanh toán | ✅ | ✅ | ✅ | ✅ | — | — |
| Đã thanh toán | — | — | ✅ | — | ✅ | Khóa ✅ |
| Đã hoàn tiền | — | — | ✅ | — | — | — |
| Đã hủy | — | ✅ | — | — | — | Mở khóa ✅ |

> Lưu ý CSDL: bảng nền `HIS_PATIENT_PACKAGE.STATUS_CODE` (§3.1) chỉ có 3 mã gốc (REGISTERED/IN_USE/LOCKED). 4 trạng thái hiển thị ở trên (theo bảng màn 6.2) được suy ra thêm từ tiền đã hoàn/đã hủy — cần backend chốt mapping mã gốc → mã hiển thị (`PatientPackageStatusCode` + `GetStatusName`/`IsActionAllowed`, gom một chỗ trong UcHisPatientPackage___Grid.cs).

### Điều kiện
- Nút Xóa: hỏi xác nhận trước; backend `HisPatientPackage/Delete` chặn nếu gói đã thanh toán mà chưa hoàn hết. Xóa kèm chi tiết.
- Nút Khóa: bắt buộc nhập lý do (XtraInputBox) trước khi gọi API.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_PATIENT_PACKAGE | View | Nguồn dữ liệu grid danh sách (BN + đối tượng + gói + trạng thái) |
| HIS_PATIENT_PACKAGE | Table | DTO truyền sang Thanh toán/Hoàn/Khóa (map từ view) |
| HIS_GENDER | Table (cache) | Resolve tên giới tính theo PATIENT_GENDER_ID |

Cột STATUS_CODE: `REGISTERED` | `IN_USE` | `LOCKED` (xem `PatientPackageStatusCode.cs`).

## 4. UI Layout (màn 6.2 — UserControl)

```
+---------------------------+------------------------------------------------------------+
| [Mã bệnh nhân          ]  |  STT | (Sửa)(Xóa)(Khóa)(TT)(Hoàn)(In) | Mã BN | Tên BN |   |
| [Từ khóa tìm kiếm      ]  |      ...                                                   |
| Thời gian tạo             |  ... Gói | Trạng thái | Địa chỉ | TG tạo | Người tạo | ... |
| [Trong ngày v][  date  ]  |                                                            |
| [  ◄  ] [  ►  ]           |                                                            |
|                           |                                                            |
| [Tìm (Ctrl F)][Làm lại]   |  [ UcPaging .................................. ]           |
+---------------------------+------------------------------------------------------------+
```

- Panel trái: `txtPatientCode`, `txtKeyword`, `cboTimeType` (Trong ngày/tuần/tháng/Tùy chọn) + `dteDate` + `btnPrevDate`/`btnNextDate`, `btnSearch` (Ctrl+F), `btnRefresh` (Ctrl+R).
- Grid: cột STT + 6 cột nút icon (Sửa/Xóa/Khóa/Thanh toán/Hoàn tiền/In) ẩn/hiện theo trạng thái qua `gridView_CustomRowCellEdit`; cột dữ liệu; 4 cột audit cuối (Thời gian tạo/Người tạo/Thời gian sửa/Người sửa). Tô màu dòng theo trạng thái.

### Icon các nút
| Hành động | Icon | Nguồn |
|-----------|------|-------|
| Mũi tên trạng thái | tam giác vẽ động (`gridView_CustomDrawCell`), đổi màu theo trạng thái | tự vẽ |
| Sửa | `images/edit/edit_16x16.png` | DevExpress gallery |
| Xóa | `delete_16x16.png` | embed (AllergyCard) |
| Khóa | `lock_16x16.png` | embed (ServiceReqList hmenu-lock) |
| Mở khóa | `unlock_16x16.gif` | embed (ServiceReqList hmenu-unlock) |
| Thanh toán | `images/miscellaneous/currency_16x16.png` | DevExpress gallery |
| Hoàn tiền | `images/actions/refresh2_16x16.png` | DevExpress gallery |
| In | `images/print/printer_16x16.png` | DevExpress gallery |

(Các icon `print_16x16.png`/`refund_16x16.png` đã nhúng nhưng không còn dùng — có thể bỏ sau.)

### UC sử dụng
| UC | Mục đích |
|----|----------|
| Inventec.UC.Paging | Phân trang server-side |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Danh sách | api/HisPatientPackage/GetView | MosConsumer | HisPatientPackageViewFilter (KEY_WORD, CREATE_TIME_FROM/TO, ORDER_*) |
| Xóa | api/HisPatientPackage/Delete | MosConsumer | ID (long) |
| Khóa | api/HisPatientPackage/Lock | MosConsumer | HIS_PATIENT_PACKAGE (ID + LOCKED_REASON) — *backend bổ sung* |
| Mở khóa | api/HisPatientPackage/Unlock | MosConsumer | ID (long) — *backend bổ sung* |

## 6. Dependencies (Inter-Plugin)

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.PatientPackageRegister (màn 6.1) | Nút Sửa | `(int)GlobalVariables.ActionEdit`, `V_HIS_PATIENT_PACKAGE` |
| HIS.Desktop.Plugins.TransactionBillOther | Nút Thanh toán | `HIS_PATIENT_PACKAGE`, `V_HIS_PATIENT_PACKAGE` |
| HIS.Desktop.Plugins.TransactionRepay | Nút Hoàn tiền | `HIS_PATIENT_PACKAGE`, `V_HIS_PATIENT_PACKAGE` |

Mở qua `HIS.Desktop.Utility.PluginInstance.GetPluginInstance(...)` (xem `OpenModuleByLink` trong UcHisPatientPackage___Process.cs). Các module đích là Form → ShowDialog → refresh danh sách. Theo mục 6.5 đặc tả, TransactionBillOther/TransactionRepay được bổ sung nhận thông tin gói.

### Hợp đồng args của PatientPackageRegister (verify từ DLL `PatientPackageRegister.Run()`)

`PatientPackageRegisterBehavior.Run()` quét mảng `entity[]` và chỉ nhận **3 kiểu** (qua `isinst`):

| Kiểu | Vai trò | Bắt buộc |
|------|---------|----------|
| `Inventec.Desktop.Common.Modules.Module` | Module context — PluginInstance **tự inject** qua `GetModuleWithWorkingRoom`; nếu null ⇒ Run() throw | ✅ (auto) |
| `MOS.EFMODEL.DataModels.HIS_PATIENT` | Bệnh nhân của gói (chế độ tạo mới truyền cái này) | — |
| `MOS.EFMODEL.DataModels.HIS_PATIENT_PACKAGE` | Gói cần sửa; **không truyền ⇒ chế độ tạo mới** | — |

> ⚠️ Run() **KHÔNG** parse `int action`/`long`. Form quyết định Tạo/Sửa theo việc có `HIS_PATIENT_PACKAGE` hay không (nút Sửa của UcHisPatientPackage có add `(int)2` nhưng giá trị này bị Run() bỏ qua). Tạo gói = truyền `HIS_PATIENT`.

### Entry points tạo gói (mục 6.9 — Bổ sung menu gói)

| Plugin gọi | Vị trí menu | Args truyền |
|------------|-------------|-------------|
| HIS.Desktop.Plugins.ExamServiceReqExecute | Menu nút "Khác" → "Đăng ký gói" | `HIS_PATIENT` (CurrentPatient) |
| HIS.Desktop.Plugins.TreatmentList | Chuột phải → "Bệnh nhân" → "Đăng ký gói" | `HIS_PATIENT` (load từ `currentTreatment.PATIENT_ID` qua `GetPatientByID`) |

## 7. Print

| Loại in | Trạng thái |
|---------|-----------|
| Phiếu thông tin gói | TODO — chưa có PrintTypeCode mẫu. `PrintProcess()` hiện báo "đang phát triển". |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 27/05/2026 | phuongnm | Tạo mới plugin; dựng màn 6.2 Danh sách gói (UC): panel lọc, grid + nút icon theo trạng thái, paging, Xóa/Khóa/Mở khóa + mở Sửa/Thanh toán/Hoàn tiền. |
| 27/05/2026 | phuongnm | Chuyển sang 4 trạng thái hiển thị (Chờ thanh toán/Đã thanh toán/Đã hoàn tiền/Đã hủy) + cột mũi tên đổi màu (CustomDrawCell). Đổi icon Thanh toán/Hoàn tiền/In sang DevExpress gallery (currency/refresh2/printer). |
| 27/05/2026 | phuongnm | Mở màn 6.2 dạng **cửa sổ Form** `frmHisPatientPackage : FormBase` (bọc UC, Dock Fill). FormBase tự set icon HIS + tiêu đề từ Module.text. Behavior trả Form thay vì UC. |
| 28/05/2026 | phuongnm | Bổ sung **cột checkbox** trước STT (MultiSelect + CheckBoxRowSelect); nhóm **"Thời gian tạo"** thêm nút thu/ẩn (▲/▼), chữ xanh đậm; nút **◄ ►** chỉnh kích thước + căn giữa + chữ xanh. |
| 28/05/2026 | phuongnm | Spec đổi: **màn 6.1 Đăng ký/Sửa gói là plugin RIÊNG** `HIS.Desktop.Plugins.PatientPackageRegister` (không còn dùng chung module link). Bỏ routing theo `int action` trong Behavior; nút Sửa giờ mở `ModuleLinkString.PatientPackageRegister`. |
| 29/05/2026 | tuanln | **6.9 Bổ sung menu gói** — thêm 2 entry point tạo gói: `ExamServiceReqExecute` (menu nút "Khác" → "Đăng ký gói") và `TreatmentList` (chuột phải → "Bệnh nhân" → "Đăng ký gói"), đều mở `PatientPackageRegister` truyền `HIS_PATIENT` (tạo mới). Ghi rõ hợp đồng args verify từ DLL: Run() chỉ nhận `Module` + `HIS_PATIENT` + `HIS_PATIENT_PACKAGE`. |
| 05/06/2026 | phuongnm | **Fix Sửa gói: trạng thái không load** — `BuildPackageArgs` map `STATUS_CODE` đã bị chuyển sang mã HIỂN THỊ; bổ sung `PatientPackageStatusCode.ToRaw()` trước khi truyền sang `PatientPackageRegister` để `cboTrangThai` load đúng (giống LockProcess/UnlockProcess). **Fix lọc**: tìm theo Từ khóa giờ VẪN áp filter thời gian (bỏ `return` sớm trong `SetFilter`); Mã BN vẫn tìm xuyên ngày. |
| 05/06/2026 | phuongnm | **Fix tìm theo Mã BN (tester)** — `HisPatientPackageViewFilter` chỉ có `KEY_WORD` (không có field PATIENT_ID/PATIENT_CODE), nên Mã BN resolve qua `LoadPatientByCode` (HisPatient, `PATIENT_CODE__EXACT`) lấy mã chuẩn rồi đưa MÃ ĐÚNG vào `KEY_WORD`. (Lưu ý: tìm theo PATIENT_ID đúng nghĩa cần backend thêm field vào ViewFilter + DAO GetView.) |
| 05/06/2026 | phuongnm | **Fix Mã BN khớp nhầm theo tên** — nhập chữ/tên (vd "DGFT") vào ô Mã BN không còn ra kết quả: `SetFilter` đổi sang trả `bool`, nếu `LoadPatientByCode` không thấy BN với mã chính xác -> trả `false` -> `LoadGridData` ép grid rỗng, KHÔNG gọi API (bỏ fallback KEY_WORD theo raw input để tránh backend khớp theo PATIENT_NAME/PACKAGE_NAME). |
| 06/06/2026 | phuongnm | **Mã BN dùng field PATIENT_CODE** — backend cập nhật `HisPatientPackageViewFilter` thêm field riêng `PATIENT_CODE` + `PATIENT_NAME`. Ô Mã BN giờ set thẳng `filter.PATIENT_CODE = code` (lọc đúng cột mã BN), bỏ workaround `LoadPatientByCode` + KEY_WORD và bỏ `bool SetFilter` (backend trả rỗng tự nhiên khi nhập tên không khớp mã). Gỡ `LoadPatientByCode` (dead code). Ô Từ khóa vẫn dùng KEY_WORD + lọc thời gian. |

## 9. Test Cases

### Danh sách / Lọc
- [ ] Mở từ menu → hiển thị danh sách gói, phân trang đúng.
- [ ] Nhập Mã BN + Enter → lọc đúng (tự chèn 0 đủ 10 số).
- [ ] Đổi "Thời gian tạo" (ngày/tuần/tháng) + ◄/► → đổi khoảng thời gian.
- [ ] Ctrl+F = Tìm, Ctrl+R = Làm lại.

### Nút + mũi tên theo trạng thái
- [ ] Chờ thanh toán (mũi tên cam): hiện Sửa/Xóa/In/Thanh toán.
- [ ] Đã thanh toán (mũi tên xanh lá): hiện In/Hoàn tiền/Khóa; ẩn Sửa/Xóa.
- [ ] Đã hoàn tiền (mũi tên xanh dương): chỉ hiện In.
- [ ] Đã hủy (mũi tên xám): hiện Xóa/Mở khóa.

### Hành động
- [ ] Xóa → confirm → API → refresh.
- [ ] Khóa → nhập lý do (bắt buộc) → API → refresh; bỏ trống lý do → cảnh báo.
- [ ] Mở khóa → confirm → API → refresh.
- [ ] Thanh toán/Hoàn tiền → mở đúng module, truyền gói + BN, đóng lại refresh.
- [ ] Sửa → mở màn 6.1 (khi đã triển khai).
