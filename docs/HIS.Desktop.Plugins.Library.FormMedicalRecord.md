# HIS.Desktop.Plugins.Library.FormMedicalRecord — Tài liệu module

## 1. Tổng quan

Thư viện tạo menu chọn vỏ bệnh án, mẫu phiếu và chuẩn bị dữ liệu HIS để mở EMR qua `ConnectToEMR.exe`.

## 2. Quy trình nghiệp vụ

`MediRecordProcessor.LoadDataEmr` lấy dữ liệu qua `api/HisTreatment/GetForEmr`, ánh xạ thông tin hành chính vào `HanhChinhBenhNhan`, gán vào `EMR_ADO._HanhChinhBenhNhan_s`, rồi mã hóa dữ liệu để truyền cho EMR.

Thông tin người nhà lấy từ bệnh nhân hiện tại (`HisTreatmentForEmrSDO.Patient`):

| Nguồn HIS | Đích EMR |
|---|---|
| `RELATIVE_NAME` + `RELATIVE_ADDRESS` | `HoTenDiaChiNguoiNha` |
| `RELATIVE_PHONE` | `SoDienThoaiNguoiNha` |
| `RELATIVE_TYPE` | `QuanHeVoiNguoiNha` |

`RELATIVE_TYPE` là nội dung ô “Quan hệ” trên màn hình sửa bệnh nhân. Truyền nguyên giá trị, bao gồm `null` hoặc chuỗi rỗng khi chưa khai báo, để EMR nhận cùng dữ liệu với HIS.

## 3. EFMODEL sử dụng

`V_HIS_PATIENT` cung cấp thông tin hành chính và người nhà; `V_HIS_TREATMENT` cung cấp thông tin đợt điều trị.

## 4. Giao diện

Menu chọn bệnh án/phiếu được tạo bởi `MediRecordMenuPopupProcessor`; danh sách phiếu có thể mở qua `frmPhieu`. Ô “Là … của người bệnh” của giấy cung cấp thông tin và cam kết chung về nhập viện nội trú thuộc giao diện EMR.

## 5. API liên quan

`api/HisTreatment/GetForEmr` qua `MosConsumer`, đầu vào `HisTreatmentForEmrSDO`. Bổ sung quan hệ người nhà sử dụng dữ liệu bệnh nhân đã có trong phản hồi.

## 6. Dependencies

`MOS.SDO`, `MOS.EFMODEL`, `EMR_MAIN.dll` và `EMR_MAIN.Library.dll`. Project nhận đường dẫn release qua `HisEmrReleaseRoot`, mặc định là `lib/EMRLinksoft` ở cùng cấp với checkout. Hai DLL EMR dùng `SpecificVersion=False`; Newtonsoft.Json tham chiếu đúng phiên bản 11.

Release: [EMRRelease](https://14.224.177.62:4433/ChuongTrinh/EMRRelease). Xem [hướng dẫn build và triển khai runtime](HIS.EMR.Runtime.md) để đặt `EMR_MAIN.Library`, MongoDB.Bson và Json11 đúng vị trí, đồng thời giữ Json6 cho host HIS.

Bộ EMR version `26.10.22.1509` tách các model dùng chung (`HanhChinhBenhNhan`, `ThongTinDieuTri`, `LoaiBenhAnEMR`, ...) sang `EMR_MAIN.Library.dll`. `HanhChinhBenhNhan.QuanHeVoiNguoiNha` là `string` có setter. `ConnectToEMR.exe` cũng cần sử dụng bộ EMR hỗ trợ trường này khi chạy.

Các điểm tương thích với bộ DLL mới:

- Dùng assembly alias `EmrLibrary` để chọn `BenhAnNgoaiTru_HoTroSinhSan` từ `EMR_MAIN.Library`, vì lớp này xuất hiện trong cả hai DLL.
- `BenhAnMatChanThuong.XuatHuyet_MP` và `XuatHuyet_MT` đã là `int`; serialize trực tiếp model để giữ dữ liệu số trong JSON.
- `EmrMainADO` kế thừa `MaICD_BenhKemTheo` và `MaICD_ChanDoanVaoVien` từ `ThongTinDieuTri`, không khai báo lại các thuộc tính cùng tên.

## 7. In

Hiển thị và in mẫu phiếu do EMR xử lý từ dữ liệu nhận được.

## 8. Changelog

| Ngày | Người sửa | Thay đổi |
|---|---|---|
| 22/10/2026 | Codex | Truyền `V_HIS_PATIENT.RELATIVE_TYPE` sang `HanhChinhBenhNhan.QuanHeVoiNguoiNha` trong luồng mở bệnh án/phiếu EMR. |
| 22/10/2026 | Codex | Sửa tham chiếu và kiểu dữ liệu để build với EMR `26.10.22.1509`; bổ sung `EMR_MAIN.Library`, xử lý model trùng và kế thừa trực tiếp các trường ICD. |
| 24/10/2026 | Codex | Cấu hình Json6/Json11 song song; bổ sung hướng dẫn và script triển khai thư viện model, MongoDB.Bson và connector từ release. |

## 9. Kiểm tra

Đã Rebuild riêng thư viện bằng MSBuild với bộ DLL EMR `26.10.22.1509` ở cả Debug và Release, AnyCPU: 0 lỗi. Các cảnh báo mã sẵn có không chặn build.

Kiểm tra cuối dùng CLR native với cấu hình HIS thật, nạp Json6 trước, không đăng ký `AssemblyResolve`. JIT `LoadDataEmr`, mapper thật, serializer Json11 và `EncodeData(EMR_ADO)` đều đạt. Kiểm tra dữ liệu giả có danh sách lồng đạt 43/43 kiểm tra trên 13 model; bao gồm nhánh cần MongoDB.Bson. Kết quả này thay cho phép thử Json6 ban đầu có resolver thay thế.

Người dùng đã xác nhận tính năng hoạt động trên giao diện ngày 24/10/2026. Khi nghiệm thu ở môi trường khác, kiểm tra quan hệ người nhà có giá trị, đổi giá trị và để trống; đồng thời mở bệnh án Nội khoa có dữ liệu chuyển khoa để kiểm tra đủ dependency. Chi tiết tại [hướng dẫn runtime](HIS.EMR.Runtime.md).
