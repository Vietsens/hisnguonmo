# Điều phối Cận lâm sàng (CLS) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.CoordinationServiceReqCLS |
| Loại | UserControl (UserControlBase) — nhúng vào workspace như HIS.Desktop.Plugins.BedRoomPartial |
| Mục đích | Theo dõi tập trung y lệnh cận lâm sàng của bệnh nhân theo phòng xử lý; cảnh báo trực quan chỉ số bất thường/vượt ngưỡng bằng màu; ghi nhận người xem + hướng giải quyết cho từng y lệnh |
| Vị trí menu | Phòng Khám, Buồng Bệnh |
| Đối tượng dùng | Bác sĩ, điều dưỡng |
| Người tạo | vuongnd |
| Ngày tạo | 22/07/2026 |
| Trạng thái | Đang phát triển (Frontend — phần III tài liệu) |

> Tham chiếu giao diện & luồng từ chức năng **Danh sách y lệnh** (`HIS.Desktop.Plugins.ServiceReqList`).

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở chức năng → tự động gọi `GetServiceReqCLS` lấy y lệnh CLS trong ngày hiện tại theo phòng xử lý (`REQUEST_ROOM_ID = currentModule.RoomId`).
2. Người dùng lọc theo khoảng ngày y lệnh, mã điều trị, họ tên, mã y tế → **Tìm (Ctrl+F)**; **Xóa trắng** để reset.
3. Chọn 1 bệnh nhân ở lưới trái → lưới phải hiển thị chi tiết các y lệnh của điều trị đó.
4. Bấm **Xem** để xem kết quả theo loại dịch vụ (tái sử dụng module có sẵn).
5. Nhập **Hướng giải quyết**, bấm **Lưu** → `UpdateCoordination` (ghi nhận người xem + hướng giải quyết).
6. Checkbox **Tự động load lại sau [n] giây** → Timer tự làm mới danh sách theo chu kỳ.

### Trạng thái tổng hợp CLS (`SERVICE_REQ_STT_ID`)
```
1 Chưa thực hiện → 2 Đang thực hiện → 3 Đủ kết quả
```

### Quy tắc tô màu (WARNING) — mục 5.5
| Màu | WARNING | Ý nghĩa |
|-----|---------|---------|
| Trắng | null / 1 | Chỉ số bình thường |
| Vàng | 2 | Bất thường, chưa vượt ngưỡng |
| Đỏ | 3 | Vượt ngưỡng cảnh báo nguy hiểm |

## 3. EFMODEL / SDO Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HisServiceReqGetServiceReqCLSSDO | SDO (tạm khai báo cục bộ trong ADO/) | Dòng y lệnh CLS trả về từ API |
| HisServiceReqUpdateCoordinationSDO | SDO (tạm cục bộ) | Body cập nhật hướng giải quyết { Id, SolutionDes } |
| HisServiceReqViewFilterQuery | Filter (tạm cục bộ) | Điều kiện lọc GetServiceReqCLS |
| CoordinationPatientADO | ADO cục bộ | Dòng bệnh nhân gom theo mã điều trị (lưới trái) |
| V_HIS_SERVICE_REQ | View | Lấy `EXE_SERVICE_MODULE_ID`, `IS_ANTIBIOTIC_RESISTANCE` khi Xem kết quả |
| HIS_SERE_SERV | Table | Lấy dịch vụ (id, `IS_SENT_EXT`) khi Xem kết quả |

> **Lưu ý migration**: 3 SDO/Filter đang khai báo cục bộ vì `MOS.SDO`/`MOS.Filter` (bản build 08/07/2026) CHƯA có type CLS. Khi backend bổ sung DLL: xóa 3 file trong `ADO/` (HisServiceReqGetServiceReqCLSSDO, HisServiceReqUpdateCoordinationSDO, HisServiceReqViewFilterQuery) và đổi `using` sang `MOS.SDO` / `MOS.Filter`. Tên class + field giữ nguyên nên code sử dụng không đổi.

## 4. UI Layout

```
+------------------------------------------------------------------+
| TG y lệnh [từ]-[đến]  Mã điều trị[ ] Họ tên[ ] Mã y tế[ ] [Tìm][Xóa]|
| ☑ Tự động load lại sau [ 60 ] giây                                 |
+---------------------------------+--------------------------------+
| LƯỚI TRÁI: DS bệnh nhân          | LƯỚI PHẢI: Chi tiết y lệnh     |
| STT|Mã ĐT|Giường|Tên|NS|Địa chỉ| | STT|Ngày YC|Mã y lệnh|         |
| Hướng GQ|Giới tính|Đối tượng     | Tên DV[Xem]|Người xem|         |
| (tô màu theo WARNING)            | Hướng giải quyết|[Lưu]         |
| [ucPaging1]                      |                                |
+---------------------------------+--------------------------------+
```

Bố cục: `panelFilter` (Dock Top) + `SplitContainerControl` (Panel1 = lưới bệnh nhân + phân trang, Panel2 = lưới chi tiết y lệnh).

**Hiển thị dạng UserControl nhúng** (không popup): Processor đăng ký `Module.MODULE_TYPE_ID__UC`, `IsEnable()=true`; Behavior trả về `UCCoordinationServiceReqCLS`; phím tắt Ctrl+F qua `KeyboardWorker` (thay BarManager của Form).

### Control chính
| Control | Loại | Mục đích |
|---------|------|----------|
| dtIntructionDateFrom/To | DateEdit | Khoảng ngày y lệnh (mặc định hôm nay) |
| txtTreatmentCode/PatientName/PatientCode | TextEdit | Điều kiện tìm |
| btnFind / btnClear | SimpleButton | Tìm (Ctrl+F qua BarManager) / Xóa trắng |
| chkAutoReload + spnAutoReloadSeconds | CheckEdit + SpinEdit | Bật/tắt + chu kỳ auto-refresh |
| gridControlPatient / gridControlServiceReq | GridControl | Lưới trái / phải |
| ucPaging1 | Inventec.UC.Paging | Phân trang lưới trái (client-side) |
| repoBtnView / repoBtnSave | RepositoryItemButtonEdit | Nút Xem / Lưu trong lưới phải |

## 5. API Endpoints

| Action | URI | Consumer | Filter/Body |
|--------|-----|----------|-------------|
| Lấy danh sách CLS | `api/HisServiceReq/GetServiceReqCLS` (GET) | MosConsumer | HisServiceReqViewFilterQuery |
| Cập nhật hướng xử lý | `api/HisServiceReq/UpdateCoordination` (POST) | MosConsumer | HisServiceReqUpdateCoordinationSDO { Id, SolutionDes } |
| Lấy view y lệnh (xem KQ) | `api/HisServiceReq/GetView` | MosConsumer | HisServiceReqViewFilter |
| Lấy dịch vụ y lệnh (xem KQ) | `api/HisSereServ/Get` | MosConsumer | HisSereServFilter |

## 6. Dependencies

### Inter-Plugin — nút "Xem kết quả" (TÁI SỬ DỤNG module có sẵn)
Định tuyến theo `EXE_SERVICE_MODULE_ID` (giữ nguyên logic `ServiceReqList.repositoryItemButtonView_ButtonClick`):

| EXE_SERVICE_MODULE_ID | Điều kiện | Module mở | Args |
|-----------------------|-----------|-----------|------|
| ID__KHAM | | HIS.Desktop.Plugins.ExamServiceReqResult | sereServ.ID (long) |
| ID__XN | IS_ANTIBIOTIC_RESISTANCE = 1 | HIS.Desktop.Plugins.SereServTeinBacterium | HIS_SERE_SERV |
| ID__XN | ngược lại | HIS.Desktop.Plugins.SereServTein | HIS_SERE_SERV |
| ID__XULYXN / PHCN / XULYDV / (PTTT & IS_SENT_EXT=1) | | HIS.Desktop.Plugins.ServiceReqResultView | sereServ.ID (long) |

Mở qua `HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(link, RoomId, RoomTypeId, args)` (wrapper `CallModule`).

### Library Plugins
Không sử dụng (chức năng không in ấn / không ký số).

## 7. Print
Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 22/07/2026 | vuongnd | Tạo mới plugin Frontend Điều phối CLS: form tìm kiếm, lưới bệnh nhân (gom theo mã điều trị) + phân trang, lưới chi tiết y lệnh, tô màu WARNING, nút Xem (tái sử dụng module có sẵn), nút Lưu (UpdateCoordination), auto-refresh bằng Timer, đa ngôn ngữ vi/en. SDO/Filter khai báo cục bộ chờ backend bổ sung MOS.SDO |
| 22/07/2026 | vuongnd | Chuyển hiển thị từ Form popup sang UserControl nhúng workspace (giống BedRoomPartial): Processor MODULE_TYPE_ID__UC + IsEnable=true, Behavior trả UC, phím tắt Ctrl+F qua KeyboardWorker |

## 9. Test Cases

### Tải danh sách
- [ ] Mở chức năng → tự động hiển thị y lệnh CLS trong ngày hiện tại theo phòng.
- [ ] Lọc theo mã điều trị / họ tên / mã y tế / khoảng ngày → **Tìm** trả đúng.
- [ ] Bỏ trống "TG y lệnh từ" → cảnh báo, chặn tìm.
- [ ] "Xóa trắng" → reset điều kiện về ngày hiện tại.

### Lưới bệnh nhân / màu
- [ ] Gom đúng theo mã điều trị, mỗi điều trị 1 dòng.
- [ ] WARNING=3 → dòng đỏ; =2 → vàng; null/1 → trắng.
- [ ] Hướng giải quyết rỗng → hiển thị "Chưa xử lý".
- [ ] Phân trang hoạt động, STT liên tục theo trang.

### Lưới chi tiết + Lưu
- [ ] Chọn bệnh nhân → lưới phải hiển thị đúng các y lệnh.
- [ ] Người xem mặc định = tài khoản đang đăng nhập; Hướng giải quyết mặc định "Đã xem".
- [ ] Sửa hướng giải quyết → **Lưu** → `UpdateCoordination` thành công, lưới cập nhật.

### Xem kết quả (tái sử dụng module)
- [ ] Y lệnh Khám → mở ExamServiceReqResult.
- [ ] Y lệnh XN thường → mở SereServTein; kháng sinh đồ → SereServTeinBacterium.
- [ ] CĐHA/TDCN/PHCN/PTTT gửi ngoài → mở ServiceReqResultView.

### Auto-refresh
- [ ] Tick "Tự động load lại sau [n] giây" → sau n giây danh sách tự làm mới.
- [ ] Bỏ tick → dừng Timer.
- [ ] Nhập số giây không hợp lệ → cảnh báo, bỏ tick.
