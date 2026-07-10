# Nhập máu và chế phẩm (Kho Máu) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ImportBlood |
| Loại | UC (UCImportBloodPlus) + Form host (FrmImportBlood) |
| Mục đích | Nhập máu và chế phẩm máu vào kho máu (từ nhà cung cấp, đăng ký, kiểm kê, khác, hiến máu). Hỗ trợ nhập nhanh túi máu bằng cách quét mã QR theo tiêu chuẩn ISBT 128. |
| Người tạo | Inventec |
| Ngày cập nhật | 06/07/2026 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng nhập túi máu (thủ công)
1. Chọn loại nhập (Nhà cung cấp / Đăng ký / Kiểm kê / Khác / Hiến máu) và kho máu.
2. Chọn loại máu trên cây loại máu (`panelControlBloodType`) → tự set `currentBlood`, enable nút **Thêm**.
3. Nhập nhóm máu, Rh, giá nhập, VAT, người cho, thời gian đóng gói, hạn sử dụng, số lô, mã vạch, nhiễm bệnh.
4. Nhấn **Thêm (Ctrl A)** → thêm túi máu vào lưới (`dicBloodAdo`).
5. Nhấn **Lưu (Ctrl S)** / **Lưu nháp (Ctrl D)** để lưu phiếu nhập.

### Luồng nhập túi máu bằng mã QR (ISBT 128) — khôi phục 06/07/2026
1. Người dùng quét mã QR túi máu vào ô **QR túi máu** (`txtQrBloodBag`) và nhấn Enter.
2. Hệ thống **Base64 decode** chuỗi vừa nhập.
   - Decode thất bại **hoặc** chuỗi sau decode **không chứa** ký tự `|` → hiển thị thông báo *"Dữ liệu QR không hợp lệ vui lòng kiểm tra lại!"* và dừng.
   - Decode thành công và có `|` → tiếp tục xử lý.
3. Tách dữ liệu theo cấu trúc:
   `<Mã vạch>|<Nhóm máu> <Rh>||<Thời gian đóng gói>|<Hạn sử dụng>|<Mã loại máu>|<Tên loại máu>|<Điều kiện bảo quản>`
   - VD (đã decode): `V50052240811700|O +||19/11/2022|30/12/2022|E7426V00|KHỐI HỒNG CẦU TỪ 350ml MÁU TOÀN PHẦN|( 2 - 6°C )`
4. Đổ dữ liệu vào các trường: mã vạch, nhóm máu (ABO), Rh, thời gian đóng gói, hạn sử dụng.
5. Tìm kiếm và chọn **loại máu theo mã loại máu**:
   - **Không có** loại máu tương ứng → **disable** nút Thêm.
   - **Có** loại máu tương ứng → **tự động chọn** loại máu (set `currentBlood`) và **enable** nút Thêm.

### Điều kiện nghiệp vụ
- Kho hiện tại phải là kho máu (`V_HIS_MEDI_STOCK.IS_BLOOD == 1`), nếu không sẽ bị khóa toàn bộ control.
- Với loại nhập "Nhà cung cấp", kho phải cho phép nhập từ NCC (`IS_ALLOW_IMP_SUPPLIER == 1`).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_IMP_MEST / V_HIS_IMP_MEST | Table/View | Phiếu nhập kho máu |
| HIS_IMP_MEST_BLOOD | Table | Chi tiết túi máu trong phiếu nhập |
| HIS_BLOOD / V_HIS_BLOOD | Table/View | Túi máu |
| V_HIS_BLOOD_TYPE | View | Loại máu (tìm theo `BLOOD_TYPE_CODE`, lọc `IS_LEAF=1`, `IS_ACTIVE=1`) |
| HIS_BLOOD_ABO | Table | Nhóm máu ABO (tra theo `BLOOD_ABO_CODE`) |
| HIS_BLOOD_RH | Table | Yếu tố Rh (tra theo `BLOOD_RH_CODE`) |
| HIS_IMP_MEST_TYPE | Table | Loại nhập |
| V_HIS_MEDI_STOCK | View | Kho máu |
| HIS_SUPPLIER | Table | Nhà cung cấp |

## 4. UI Layout

Vùng trái (chi tiết túi máu) — thứ tự từ trên xuống:
```
+--------------------------------------------------------------+
| [Cây loại máu — panelControlBloodType]                       |
+--------------------------------------------------------------+
| QR túi máu:  [____________ txtQrBloodBag ________________]   |  ← khôi phục
+--------------------------------------------------------------+
| Nhóm máu: [cbo] Rh:[cbo]  Giá nhập:[spin]                    |
| VAT(%):[spin]  Mã người cho:[txt]  Tên người cho:[txt]       |
| TG đóng gói:[dt] Số lô:[txt]  CSKCB chuyển:[txt]             |
| Hạn sử dụng:[dt] Mã vạch:[txt]  [ ] Nhiễm bệnh              |
|                       [Thêm] [Sửa] [Hủy]                     |
+--------------------------------------------------------------+
```
- Ô **QR túi máu** (`txtQrBloodBag`) đặt trong `layoutQrBloodBag` (full-width, ngay dưới cây loại máu). Chiều cao cây loại máu giảm 28px để chèn hàng QR mà không xô lệch các control bên dưới.
- Tooltip: *"Nhãn (mã QR) chế phẩm máu theo tiêu chuẩn ISBT 128"*.

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.BloodType | panelControlBloodType | Cây chọn loại máu |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lấy phiếu nhập | api/HisImpMest/Get, api/HisImpMest/GetView | MosConsumer |
| Chi tiết túi máu | api/HisImpMestBlood/Get | MosConsumer |
| Lấy túi máu | api/HisBlood/Get | MosConsumer |
| Loại máu theo gói thầu | api/HisBidBloodType/Get | MosConsumer |

> Việc chọn loại máu / nhóm máu / Rh cho chức năng QR lấy từ cache `BackendDataWorker` (không gọi API).

## 6. Dependencies

- HIS.UC.BloodType (cây loại máu).
- BackendDataWorker cache: V_HIS_BLOOD_TYPE, HIS_BLOOD_ABO, HIS_BLOOD_RH, V_HIS_MEDI_STOCK, HIS_SUPPLIER...

## 7. Print

Plugin có nhiều mẫu in phiếu nhập (biên bản kiểm nhập, phiếu nhập từ NCC, phiếu nhập chuyển kho...) qua `RichEditorStore` + MPS. Chức năng QR không thay đổi phần in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 09/07/2026 | phuongnm | (a) Sau khi **Thêm** túi máu và khi bấm **Làm mới** → tự động xóa trắng ô QR túi máu. (b) Loại nhập **Hiến máu**: khi quét QR mà chưa chọn/sửa hồ sơ hiến máu → hiện thông báo *"Vui lòng chọn hồ sơ hiến máu trước khi quét QR túi máu!"* (message `VuiLongChonHoSoHienMauTruocKhiQuetQr`); nếu đang sửa hồ sơ thì quét đổ dữ liệu + enable Thêm bình thường. |
| 09/07/2026 | phuongnm | Thêm thông báo khi mã loại máu (field [5]) trong QR không có trong danh mục kho: *"Không tìm thấy loại máu có mã \"{0}\" trong danh mục. Vui lòng kiểm tra lại mã loại máu trong QR!"* (message `KhongTimThayLoaiMauCoMaTrongDanhMuc`). Trước đây chỉ disable nút Thêm âm thầm, giờ báo rõ mã nào không khớp để người quét/encode QR biết. |
| 06/07/2026 | phuongnm | Khôi phục chức năng **Nhập dữ liệu từ QR túi máu** (đã mất code). Thêm ô `txtQrBloodBag` + `layoutQrBloodBag` trong Designer (thu nhỏ cây loại máu 28px để chèn hàng QR). Thêm partial `UCImportBloodPlus__Plus__Qr.cs` xử lý: Base64 decode, validate ký tự `|`, tách cấu trúc ISBT 128, đổ dữ liệu (mã vạch, nhóm máu, Rh, TG đóng gói, HSD), tìm & tự chọn loại máu theo mã loại máu (enable/disable nút Thêm). Bổ sung resource `LAYOUT_QR_BLOOD_BAG`, `TOOLTIP_QR_BLOOD_BAG` (Lang.vi/en) và message `DuLieuQrKhongHopLe` (Message.Lang.vi/en) + accessor `ResourceMessageLang.DuLieuQrKhongHopLe`. |

## 9. Test Cases

### QR túi máu
- [ ] Quét chuỗi Base64 hợp lệ (có `|` sau decode) → đổ đúng mã vạch, nhóm máu, Rh, TG đóng gói, HSD.
- [ ] Mã loại máu tồn tại (leaf, active) → tự chọn loại máu, nút Thêm **enable**.
- [ ] Mã loại máu không tồn tại → nút Thêm **disable**, các trường khác vẫn đổ dữ liệu.
- [ ] Chuỗi không phải Base64 hợp lệ → thông báo *"Dữ liệu QR không hợp lệ vui lòng kiểm tra lại!"*.
- [ ] Chuỗi Base64 hợp lệ nhưng sau decode không có `|` → thông báo lỗi như trên.
- [ ] Nhóm máu dạng "O +" / "AB -" → tách đúng ABO và Rh.
- [ ] HSD theo QR không bị ghi đè bởi HSD tự tính từ TG đóng gói.
- [ ] Sau khi quét thành công → nhấn Enter tại ô Mã vạch thực hiện Thêm túi máu vào lưới.
