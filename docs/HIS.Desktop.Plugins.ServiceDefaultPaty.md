# Thiết Lập Đối Tượng Thanh Toán Cho Dịch Vụ — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ServiceDefaultPaty |
| Loại | Form |
| Mục đích | Khai báo đối tượng thanh toán (ĐTTT) mặc định của dịch vụ theo **đối tượng bệnh nhân** và **đối tượng phụ thu**. Khi chỉ định dịch vụ, phần mềm ưu tiên lấy ĐTTT theo cấu hình này; không có cấu hình thì xử lý như hiện tại. Cấu hình cũng là điều kiện kích hoạt phân quyền sửa lại ĐTTT (PT-44730 / tài liệu phân tích 2680) |
| Người tạo | tuanln |
| Ngày tạo | 29/07/2026 |
| Trạng thái | Đang phát triển — **chờ backend** (bảng, khung nhìn và bộ API chưa được gencode) |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

1. Quản trị mở màn hình từ menu **Thiết lập** (cạnh "Thiết lập ĐTTT cho dịch vụ đi kèm" — module 8202).
2. Khai một luật: **dịch vụ** (bắt buộc) + **ĐT bệnh nhân** (rỗng = mọi đối tượng) + **ĐT phụ thu** (rỗng = mọi trường hợp, kể cả hồ sơ không có phụ thu) → **ĐTTT mặc định** (bắt buộc).
3. Lưới hiển thị cả dòng đang dùng và dòng đã khóa; khóa/mở khóa và xóa mềm ngay trên lưới.
4. Màn **Chỉ định dịch vụ** tra dòng khớp khi bác sĩ chọn dịch vụ và điền sẵn ĐTTT.
5. Ba màn còn lại (Sửa chỉ định dịch vụ · Bảng kê · Chuyển đối tượng thanh toán) chỉ dùng cấu hình để quyết định **được sửa ô ĐTTT hay không**.

### Điều kiện nghiệp vụ

- Dòng đã khóa hoặc đã xóa mềm không được đưa vào tra cứu lúc chỉ định.
- Nhiều dòng cùng khớp → dòng khai **đủ điều kiện hơn** thắng; bằng nhau thì lấy dòng **tạo sau cùng**.
- Thứ tự ưu tiên nguồn ĐTTT: bảng cấu hình mới → `HIS_SERVICE.DEFAULT_PATIENT_TYPE_ID` → luồng lấy theo đối tượng bệnh nhân hiện hành.
- ĐTTT tra được phải nằm trong danh sách bệnh nhân được hưởng **và** dịch vụ có khai giá; không hợp lệ thì **bỏ qua cấu hình**, không chặn bác sĩ, không báo lỗi.
- Trùng bộ điều kiện (cùng dịch vụ + cùng ĐT bệnh nhân + cùng ĐT phụ thu) → không cho lưu.
- Không hồi tố: thêm/sửa/khóa/xóa chỉ ảnh hưởng chỉ định tạo mới sau đó.
- Cờ **"không dùng BHYT"** (`HIS_SERVICE.DO_NOT_USE_BHYT`) giữ nguyên tác dụng, chạy song song.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| `HIS_SERVICE_DEFAULT_PATY` | Table (**mới — chờ backend**) | Luật tra ĐTTT mặc định: `SERVICE_ID`, `PATIENT_TYPE_ID`, `PRIMARY_PATIENT_TYPE_ID`, `DEFAULT_PATIENT_TYPE_ID` |
| `V_HIS_SERVICE_DEFAULT_PATY` | View (**mới — chờ backend**) | Bảng trên kèm mã/tên dịch vụ và mã/tên 3 đối tượng, phục vụ lưới |
| `V_HIS_SERVICE` | View | Nguồn combo dịch vụ (loại trừ thuốc, vật tư, máu, suất ăn, khác, gói) — lấy từ `BackendDataWorker` |
| `HIS_PATIENT_TYPE` | Table | Nguồn 3 combo đối tượng. Combo **ĐT phụ thu** chỉ lấy `IS_ADDITION = 1` |
| `HIS_CONFIG` | Table | Bản ghi cấu hình phân quyền sửa, khai theo chi nhánh (`BRANCH_ID`) |

> Backend chưa gencode nên frontend dùng DTO/filter tạm trong `HIS.Desktop.Plugins.Library.ServiceDefaultPaty`:
> `ServiceDefaultPatyDTO`, `ServiceDefaultPatyViewDTO`, `ServiceDefaultPatyFilter`. Khi có entity thật thì thay 3 lớp này.

## 4. UI Layout

### Sơ đồ giao diện

```
+---------------------------------------------------+--------------------------------+
| [Ô tìm kiếm]           [Tìm (Ctrl F)]             | Dịch vụ:       [mã] [tên DV] |
+---------------------------------------------------+ ĐT bệnh nhân:  [combo]        |
| Lưới cấu hình                                     | ĐT phụ thu:    [combo]        |
| STT | 🔓 | ❌ | Mã DV | Tên DV | ĐT bệnh nhân |    | ĐTTT mặc định: [combo]        |
|     ĐT phụ thu | ĐTTT mặc định | 4 cột audit      |   [Sửa][Thêm][Làm lại]        |
+---------------------------------------------------+--------------------------------+
| [Phân trang]                                      |                                |
+---------------------------------------------------+--------------------------------+
```

Bố cục theo màn hình tham chiếu 8202. Nhãn bắt buộc (Dịch vụ, ĐTTT mặc định) màu Brown. Combo ĐT bệnh nhân / ĐT phụ thu để trống hiển thị "Tất cả".

### UC sử dụng

| UC | Panel | Mục đích |
|----|-------|----------|
| Inventec.UC.Paging | ucPaging | Phân trang lưới cấu hình |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy danh sách (lưới) | `ServiceDefaultPatyUriStore.MOSHIS_HIS_SERVICE_DEFAULT_PATY_GET_VIEW` | MosConsumer | `ServiceDefaultPatyFilter` |
| Lấy danh sách (tra cứu khi chỉ định) | `ServiceDefaultPatyUriStore.MOSHIS_HIS_SERVICE_DEFAULT_PATY_GET` | MosConsumer | `ServiceDefaultPatyFilter` |
| Thêm | `...CREATE` | MosConsumer | — |
| Sửa | `...UPDATE` | MosConsumer | — |
| Khóa / mở khóa | `...CHANGE_LOCK` (truyền ID) | MosConsumer | — |
| Xóa mềm | `...DELETE` (truyền ID) | MosConsumer | — |

Tiền tố: `api/HisServiceDefaultPaty/`. **Toàn bộ endpoint chưa tồn tại** — API trả null thì worker coi như bảng cấu hình rỗng và mọi màn giữ nguyên hành vi hiện tại.

## 6. Dependencies

### Library Plugins

| Library | Mục đích |
|---------|----------|
| `HIS.Desktop.Plugins.Library.ServiceDefaultPaty` (**mới**) | DTO/filter tạm, `ServiceDefaultPatyUriStore`, `ServiceDefaultPatyCFG` (đọc key phân quyền), `ServiceDefaultPatyWorker` (nạp cấu hình 1 lần, tra dòng khớp, tính quyền sửa). Dùng chung cho 4 màn tiêu thụ |

### Inter-Plugin

Không mở plugin khác. Bốn màn tiêu thụ cấu hình này:

| Plugin | Dùng để | Vị trí |
|--------|---------|--------|
| `HIS.Desktop.Plugins.AssignService` | Điền ĐTTT mặc định + khóa ô | `frmAssignService__Plus__ServiceDefaultPaty.cs`, `frmAssignService__Load.cs` (ChoosePatientTypeDefaultlService), `frmAssignService.cs` (CustomRowCellEdit) |
| `HIS.Desktop.Plugins.AssignServiceEdit` | Khóa ô | `FormAssignServiceEdit__Plus__ServiceDefaultPaty.cs`, `FormAssignServiceEdit.cs` (GridViewService_CustomRowCellEdit) |
| `HIS.Desktop.Plugins.Bordereau` | Khóa ô theo từng dòng | `frmBordereau___ServiceDefaultPaty.cs`, `frmBordereau.cs` (gridViewBordereau_CustomRowCellEdit) |
| `HIS.Desktop.Plugins.CallPatientTypeAlter` | Giữ ĐTTT cũ của chỉ định không đủ quyền | `frmPatientTypeAlter___ServiceDefaultPaty.cs`, `frmPatientTypeAlter.cs` (SwapPatientTypeAlter) |

### Cấu hình hệ thống

| Key | Mặc định | Ý nghĩa |
|-----|----------|---------|
| `HIS.Desktop.Plugins.Assign.ServiceDefaultPatyEditOption` | `1` | `1` = chỉ tài khoản quản trị (`HIS_EMPLOYEE.IS_ADMIN = 1`) · `2` = quản trị **hoặc** người chỉ định (`HIS_SERVICE_REQ.REQUEST_LOGINNAME`) · khác = không phân quyền. Chỉ tác dụng với dịch vụ đã khai trong bảng cấu hình |

## 7. Print

Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 29/07/2026 | tuanln | PT-44730: tạo mới plugin. Màn hình clone bố cục module 8202 (lưới + panel nhập 4 ô, Ctrl F/N/S/R, phân trang, icon khóa/xóa, 4 cột audit). Kèm Library plugin `HIS.Desktop.Plugins.Library.ServiceDefaultPaty` (DTO/filter tạm chờ backend gencode, worker tra cấu hình + tính quyền sửa) và sửa 4 màn tiêu thụ. Validate client: dịch vụ + ĐTTT mặc định bắt buộc, chống trùng bộ điều kiện |

## 9. Test Cases

### Tạo mới
- [ ] Chọn dịch vụ + ĐTTT mặc định → Lưu thành công, lưới có dòng mới
- [ ] Để trống dịch vụ hoặc ĐTTT mặc định → hiện cảnh báo tại control, không gửi API
- [ ] Khai trùng dịch vụ + cặp đối tượng → báo "Cấu hình này đã tồn tại"
- [ ] Combo ĐT phụ thu chỉ liệt kê đối tượng có `IS_ADDITION = 1`

### Sửa
- [ ] Click dòng trên lưới → dữ liệu lên panel, nút Sửa bật, nút Thêm tắt
- [ ] Sửa ĐTTT mặc định → Lưu → lưới cập nhật
- [ ] Sửa về đúng bộ điều kiện của dòng khác → báo trùng

### Khóa / Xóa
- [ ] Khóa dòng → icon đổi trạng thái, giữ nguyên trang đang xem
- [ ] Mở khóa dòng đã khóa → dòng áp dụng lại khi chỉ định
- [ ] Xóa → hỏi xác nhận → xóa mềm, lưới refresh

### Nghiệp vụ tại màn Chỉ định dịch vụ
- [ ] BN BHYT + phụ thu Viện phí, khai dòng CT + BHYT + Viện phí → Viện phí → ĐTTT điền sẵn Viện phí
- [ ] Dòng để trống 2 điều kiện đối tượng → áp cho mọi bệnh nhân
- [ ] Dịch vụ không khai cấu hình → ĐTTT như hiện tại
- [ ] Dịch vụ có cả `DEFAULT_PATIENT_TYPE_ID` và dòng cấu hình khớp → lấy theo dòng cấu hình
- [ ] Cấu hình ra đối tượng bệnh nhân không được hưởng → bỏ qua cấu hình, không báo lỗi

### Phân quyền sửa ô ĐTTT
- [ ] Config `1`, tài khoản thường → ô ĐTTT chỉ đọc ở cả 4 màn
- [ ] Config `1`, tài khoản quản trị → sửa được
- [ ] Config `2`, tài khoản là người chỉ định trên phiếu → sửa được ở màn Sửa chỉ định / Bảng kê
- [ ] Config `2`, tài khoản khác người chỉ định → chỉ đọc
- [ ] Config khác `1`/`2` → mọi tài khoản sửa được
- [ ] Bảng cấu hình rỗng → không màn nào bị siết
- [ ] Màn Chuyển đối tượng thanh toán: chỉ định thuộc dịch vụ bị siết giữ ĐTTT cũ, các chỉ định còn lại vẫn chuyển
