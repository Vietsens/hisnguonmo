# Triển khai EMR khi mở vỏ bệnh án

## Bộ release

Tải bộ EMR tại [thư mục EMRRelease](https://14.224.177.62:4433/ChuongTrinh/EMRRelease), giữ nguyên cấu trúc thư mục sau khi tải. Bộ đã kiểm tra có `EMR_MAIN.dll` và `EMR_MAIN.Library.dll` phiên bản `26.10.22.1509`, Newtonsoft.Json `11.0.0.0` và MongoDB.Bson `2.5.0.0`.

DLL release được quản lý riêng, không đưa vào Git. Cấu hình kết nối EMR cần phù hợp với môi trường triển khai.

## Build module

Project `HIS.Desktop.Plugins.Library.FormMedicalRecord` nhận đường dẫn release qua property `HisEmrReleaseRoot`. Khi không truyền hoặc truyền giá trị rỗng, project dùng thư mục `lib/EMRLinksoft` ở cùng cấp với checkout `hisnguonmo`, theo cách tổ chức thư viện hiện có của HIS.

Ví dụ trong Developer PowerShell của Visual Studio, chạy từ gốc repo:

```powershell
$module = 'HIS\Plugins\HIS.Desktop.Plugins.Library.FormMedicalRecord\HIS.Desktop.Plugins.Library.FormMedicalRecord.csproj'
MSBuild.exe $module /t:Rebuild /p:Configuration=Release /p:Platform=AnyCPU /p:HisEmrReleaseRoot="D:\Project\EMRRelease"
```

Các dependency HIS và DevExpress vẫn cần bộ thư viện build hiện có của dự án. Project kiểm tra ba DLL EMR dùng để biên dịch và báo đường dẫn release nếu thiếu. `EMR_MAIN.Library` giữ alias `EmrLibrary`; các DLL EMR và Json11 dùng `Private=False` để triển khai đúng vị trí bằng bước dưới.

## Triển khai vào thư mục chạy HIS

Đóng HIS và `ConnectToEMR.exe` trước khi cập nhật. Chạy script từ gốc repo, truyền thư mục HIS đang chạy và DLL module vừa build:

```powershell
$deploy = @{
    ReleaseRoot = 'D:\Project\EMRRelease'
    HisRuntimeRoot = 'D:\Project\His\hisnguonmo\HIS\HIS.Desktop\bin\Debug'
    FormMedicalRecordPath = 'HIS\Plugins\HIS.Desktop.Plugins.Library.FormMedicalRecord\bin\Release\HIS.Desktop.Plugins.Library.FormMedicalRecord.dll'
}
.\scripts\Deploy-EmrRelease.ps1 @deploy -WhatIf
.\scripts\Deploy-EmrRelease.ps1 @deploy
```

Đường dẫn trên là ví dụ; thay bằng thư mục thực tế của máy triển khai. Script được đưa vào repo và không phụ thuộc các build target riêng của máy phát triển.

| Nguồn release | Đích trong thư mục HIS |
|---|---|
| `EMR_MAIN.Library.dll`, `MongoDB.Bson.dll` | `ReferencedAssemblies/` |
| `Newtonsoft.Json.dll` phiên bản 11 | `ReferencedAssemblies/EmrJson11/` |
| DLL/EXE ở gốc release | `Integrate/EMR/` |
| DLL trong `Plugin`, gồm thư mục con | `Integrate/EMR/Plugin/` |
| DLL FormMedicalRecord truyền vào script | `ReferencedAssemblies/` |

Script kiểm tra nguồn trước khi copy, sao lưu các file bị thay thế và bổ sung `codeBase` cho Json11 trong `HIS.exe.config`. Các file kết nối `.config` của connector, dữ liệu và template được giữ nguyên. Script không xóa file đích. Đường dẫn sao lưu được in sau khi triển khai.

## Vì sao cần các thư viện này

Model dùng chung của EMR mới nằm trong `EMR_MAIN.Library.dll`. HIS cần nạp model để chuẩn bị dữ liệu trước khi gọi connector; chỉ có DLL trong `Integrate/EMR` chưa đủ. Thiếu thư viện có thể gây lỗi JIT trước khi handler mở bệnh án vào `try/catch`.

HIS dùng Newtonsoft.Json 6, còn model EMR mới có dependency và attribute của Json11. `App.config` khai báo chính xác phiên bản 11 tại đường dẫn riêng:

```xml
<dependentAssembly>
  <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral"/>
  <codeBase version="11.0.0.0" href="ReferencedAssemblies/EmrJson11/Newtonsoft.Json.dll"/>
</dependentAssembly>
```

Giữ Json6 và `EMR_MAIN.dll` cũ của host HIS cho các module hiện hữu. Không redirect toàn cục Json6 sang Json11. Connector dùng bộ DLL release riêng trong `Integrate/EMR`.

`MongoDB.Bson.dll` phiên bản 2.5 cần có trong host vì serializer đọc attribute `BsonIgnore` trên `LichSuChuyenKhoa.DateNgayYeuCauChuyen` khi danh sách chuyển khoa có phần tử. Thiếu DLL này vẫn gây lỗi dù bệnh án trống có thể serialize thành công.

Khởi động lại HIS sau khi cập nhật để CLR nạp lại module và cấu hình assembly. Nếu connector báo lỗi kết nối dữ liệu, kiểm tra cấu hình kết nối của môi trường tại `Integrate/EMR`; script không thay thông tin kết nối.

## Kiểm chứng

- Rebuild module Debug và Release với bộ EMR `26.10.22.1509`.
- Kiểm tra CLR native trong tiến trình riêng, dùng `HIS.exe.config`, nạp Json6 trước và không đăng ký `AssemblyResolve`: JIT `LoadDataEmr`, ánh xạ `BenhAnNoiKhoa`, serialize và gọi `EncodeData(EMR_ADO)` thật đạt.
- Kiểm tra dữ liệu giả có phần tử trong các danh sách lồng: 13 model, 1.542 lần gán, 43 kiểm tra đạt; gồm danh sách chuyển khoa cần MongoDB.Bson. Các kiểm tra này không gọi API hoặc ghi database.
- Đối chiếu SHA256 781 DLL/EXE của release với connector đã triển khai: khớp toàn bộ.
- Ngày 24/10/2026, người dùng xác nhận tính năng yêu cầu đã hoạt động trên giao diện thực tế.
