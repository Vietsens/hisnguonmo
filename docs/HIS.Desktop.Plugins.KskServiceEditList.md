# Sửa dịch vụ khám sức khỏe hợp đồng — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.KskServiceEditList |
| Loại | Form |
| Mục đích | Thêm dịch vụ, xóa dịch vụ, đổi phòng thực hiện theo từng dịch vụ cho nhiều bệnh nhân khám sức khỏe hợp đồng trong một lần lưu (PT-58013, tài liệu 3395) |
| Mở từ | Hồ sơ điều trị (HIS.Desktop.Plugins.TreatmentList) — nút "Sửa dịch vụ" |
| Backend | MOS `api/HisKskContract/ServiceEdit` |
| Người tạo | vuongnd |
| Ngày tạo | 23/09/2026 |
| Trạng thái | Đang phát triển |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Hồ sơ điều trị: lọc theo hợp đồng KSK → Tìm kiếm → nút "Sửa dịch vụ" hiện (chỉ khi grid đang lọc theo hợp đồng).
2. Tick 1 / nhiều / tất cả bệnh nhân → "Sửa dịch vụ". Chưa tick → cảnh báo; có hồ sơ không thuộc hợp đồng đang lọc → cảnh báo.
3. Màn hình "Sửa dịch vụ":
   - Grid **Dịch vụ hiện có**: gộp theo dịch vụ trên toàn bộ hồ sơ đã chọn (Số BN có x/N, Đã thực hiện, Phòng hiện tại). Tick **Xóa** hoặc chọn **Phòng mới** (loại trừ nhau).
   - Vùng **Thêm dịch vụ**: chọn Nhóm DV KSK → Dịch vụ trong nhóm → Phòng (mặc định theo nhóm) → Thêm.
   - Người chỉ định (mặc định tài khoản đăng nhập), Thời gian y lệnh (mặc định hiện tại).
4. Lưu → xác nhận "Áp dụng thay đổi cho N bệnh nhân?" → frontend chia lô 200 hồ sơ/lần gọi API, tuần tự → màn hình **Kết quả** (tổng hợp + chi tiết từng bệnh nhân, xuất Excel).
5. Có ít nhất 1 hồ sơ thành công → làm mới Hồ sơ điều trị, đóng màn hình.

### Điều kiện nghiệp vụ (backend)
- Mỗi hồ sơ xử lý trong 1 giao dịch riêng; hồ sơ có lỗi → rollback toàn bộ thay đổi của hồ sơ đó.
- Thứ tự xử lý trong 1 hồ sơ: Xóa → Đổi phòng → Thêm → tính lại giá.
- Xóa / đổi phòng bị chặn khi: hồ sơ kết thúc / khóa viện phí / tạm khóa / khóa BHYT; y lệnh hoàn thành; y lệnh đang xử lý (theo cấu hình `MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED`); dịch vụ đã thực hiện; XN đã có kết quả; có hóa đơn chưa hủy hoặc tạm ứng chưa hoàn.
- Xóa hết dịch vụ của một y lệnh → báo lỗi, không xóa.
- Đổi phòng theo từng dịch vụ: y lệnh chỉ còn các dịch vụ này → đổi phòng cả y lệnh; ngược lại tách dịch vụ sang y lệnh mới tại phòng mới (giữ đối tượng, số lượng, giá, tỉ lệ chi trả).
- Thêm dịch vụ: dịch vụ phải thuộc nhóm dịch vụ KSK; đối tượng = đối tượng hiện hành của hồ sơ; tỉ lệ chi trả = hợp đồng; giá = giá nhóm KSK (+VAT); hồ sơ đã có dịch vụ → bỏ qua (không chỉ định trùng).
- Hợp đồng hết hiệu lực → chặn thêm dịch vụ và đổi phòng.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TREATMENT_4 | View | Hồ sơ đã tick (truyền từ TreatmentList) |
| V_HIS_KSK_CONTRACT | View | Hợp đồng đang lọc |
| HIS_SERE_SERV | Table | Dịch vụ hiện có của các hồ sơ |
| HIS_KSK / HIS_KSK_SERVICE | Table (cache) | Nhóm dịch vụ KSK, dịch vụ trong nhóm (phòng, số lượng, giá) |
| V_HIS_SERVICE_ROOM | View (cache) | Phòng thực hiện được dịch vụ |
| V_HIS_ROOM, HIS_SERVICE_TYPE, V_HIS_SERVICE | Cache | Tên phòng, loại dịch vụ |
| ACS_USER | Cache | Người chỉ định |

## 4. UI Layout

```
+------------------------------------------------------------------------+
| Hợp đồng: HD001 - Cty ABC            Áp dụng cho: 350 bệnh nhân         |
| Người chỉ định: [cboLogin]           Thời gian y lệnh: [dtIntructionTime]|
| DỊCH VỤ HIỆN CÓ                                                         |
| STT|Mã DV|Tên DV|Loại DV|Số BN có|Đã thực hiện|Phòng hiện tại|Xóa|Phòng mới|
| THÊM DỊCH VỤ                                                            |
| Nhóm DV KSK:[cboKsk] Dịch vụ:[cboKskService] Phòng:[cboAddRoom] [Thêm]   |
| [x]|STT|Mã DV|Tên DV|Nhóm DV KSK|Phòng thực hiện|Số lượng|Đơn giá        |
|                                                         [Lưu (Ctrl+S)]  |
+------------------------------------------------------------------------+
Kết quả: Tổng hợp | grid Mã điều trị, Họ tên, Thao tác, Dịch vụ, Kết quả, Lý do | [Xuất Excel] [Đóng]
```

Grid tổng hợp nhiều bảng → không có 4 cột audit. Không có checkbox nhớ trạng thái → không dùng ControlState.

## 5. API Endpoints

| Action | URI | Consumer | Input |
|--------|-----|----------|-------|
| Lấy dịch vụ của hồ sơ | `api/HisSereServ/Get` | MosConsumer | HisSereServFilter.TREATMENT_IDs (lô 100) |
| Sửa dịch vụ | `api/HisKskContract/ServiceEdit` | MosConsumer | HisKskServiceEditSDO (lô 200 hồ sơ, PostRO) |

`HisKskServiceEditSDO` được khai báo bản sao trong `ADO/KskServiceEditSDO.cs` (không phụ thuộc bản MOS.SDO.dll mới).

## 6. Dependencies

### Inter-Plugin
| Plugin | Chiều | Args |
|--------|-------|------|
| HIS.Desktop.Plugins.TreatmentList | Mở plugin này | `List<V_HIS_TREATMENT_4>`, `V_HIS_KSK_CONTRACT`, `RefeshReference` (BtnSearch) |

### Phân quyền
- ACS_CONTROL `HIS000060` — nút "Sửa dịch vụ" trên Hồ sơ điều trị (chưa khai báo control → không kiểm soát).
- Cần khai báo module `HIS.Desktop.Plugins.KskServiceEditList` trong ACS và quyền gọi API `api/HisKskContract/ServiceEdit`.

## 7. Print

Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 23/09/2026 | vuongnd | Tạo mới plugin (PT-58013) |

## 9. Test Cases

### Mở màn hình
- [ ] Không lọc hợp đồng → nút "Sửa dịch vụ" ẩn
- [ ] Lọc hợp đồng + Tìm kiếm → nút hiện; chưa tick → cảnh báo
- [ ] Tick nhiều BN → màn hình hiện đúng số BN, đúng dịch vụ gộp

### Thêm
- [ ] Thêm dịch vụ cho BN chưa có → Thành công; BN đã có → Bỏ qua
- [ ] Hợp đồng hết hạn → chặn

### Xóa
- [ ] Xóa dịch vụ chưa thực hiện → Thành công
- [ ] Dịch vụ đã thực hiện / có kết quả / đã thanh toán → Lỗi, hồ sơ đó không thay đổi
- [ ] Xóa hết dịch vụ của y lệnh → Lỗi

### Đổi phòng
- [ ] Y lệnh 1 dịch vụ → đổi phòng cả y lệnh
- [ ] Y lệnh nhiều dịch vụ → tách y lệnh mới tại phòng mới
- [ ] Phòng mới không thực hiện được dịch vụ → Lỗi

### Nghiệp vụ đặc biệt
- [ ] Thêm + xóa + đổi phòng trong 1 lần lưu
- [ ] > 200 BN → chia lô, kết quả gộp đúng; xuất Excel kết quả
