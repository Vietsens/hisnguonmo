<!-- markdownlint-disable-next-line -->
<p align="center">
  <a href="https://nguonmo.benhvienthongminh.vn/ords/f?p=106:1:9302229919244:::::" rel="noopener" target="_blank"><img width="150" height="133" src="https://nguonmo.benhvienthongminh.vn/i/apex_ui/img/favicons/hispro/hispro-180.png" alt="hisnguonmo logo"></a>
</p>

<h1 align="center">His nguồn mở</h1>

**His nguồn mở** Phần mềm desktop chạy trên hệ điều hành Window của hệ thống quản lý bệnh viện HisPro:

- [HIS](https://github.com/Vietsens/hisnguonmo/tree/Develop/HIS) source code HIS main project và các tính năng nghiệp vụ(plugin).

- [MPS](https://github.com/Vietsens/hisnguonmo/tree/Develop/MPS) source code các tính năng in ấn.

- [UC](https://github.com/Vietsens/hisnguonmo/tree/Develop/UC) source code các thành phần giao diện dùng chung, được nhúng trong các plugin.

<div align="center">

[![license](https://img.shields.io/badge/license-GPL3-blue.svg)](https://github.com/Vietsens/hisnguonmo/blob/Develop/LICENSE)
[[DevExpress 15.2.9]](https://www.devexpress.com/)
[[log4net 1.2.10]](https://www.nuget.org/packages/log4net/1.2.10)
[[FlexCell 5.7.6.0]](https://www.tmssoftware.com/site/flexcelnet.asp)
[[Telerik UI for WinForms 2019.3.1022]](https://www.telerik.com/)
[[AForge 2.2.5.0](https://api.nuget.org/v3-flatcontainer/aforge/2.2.5/icon)](https://www.nuget.org/packages/AForge/2.2.5)
[[itextsharp 5.5.3.0]](https://www.nuget.org/packages/iTextSharp/5.5.3)
[[Aspose 11.1]](https://www.nuget.org/packages/Aspose.Words/11.1.0)
[[EO.Pdf]](https://www.nuget.org/packages/EO.Pdf/20.3.34)
[[EO.WebBrowser]](https://www.nuget.org/packages/EO.WebBrowser/20.3.34)
[[BarTender 10.1.0]]
[[STPadLibNet 8.5.2.6]](https://en.signotec.com/portal/seiten/signopad-api-device-api--900000170-10002.html)

</div>



## Documentation

### Yêu cầu môi trường hệ thống
•	Git: https://git-scm.com/downloads

•	.Net framework 4.5:  https://www.microsoft.com/en-us/download/details.aspx?id=42642

•	Microsoft Build Engine(MSBuild): Có thể dùng phiên bản tích hợp sẵn trong .net framework 
	hoặc tải phiên bản tùy chọn ở đây https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild?view=vs-2022

#### Clone

- git clone https://github.com/Vietsens/hisnguonmo.git

- git clone https://github.com/Vietsens/lib.git


## Setup

- Sau khi clone các git cần thiết về tổ chức folder theo cây folder như sau:

	++ HISNGUONMO
	
	++++++++ HIS
	
	++++++++ UC
	
	++++++++ MPS
	
	++ LIB
	
  > Các lệnh cmd
	```shell
	Gõ cd C:\Windows\Microsoft.NET\Framework\v4.0.30319 với win 32 bit
	hoặc Gõ cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319 với win 64 bit
  
	Gõ MSBuild.exe E:\HisNguonMo\Desktop\HIS\HIS.Desktop\HIS.Desktop.csproj /p:Configuration=Release /p:Platform=AnyCPU /p:TargetFrameworkSDKToolsDirectory="C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools" 
	Lưu ý: cần sửa lại tham số cấu hình trong lệnh build cho khớp với môi trường thực tế của máy tính
	Trong đó
	-	E:\HisNguonMo\Desktop\HIS\HIS.Desktop\HIS.Desktop.csproj là đường dẫn đến file cs project của main project his nguồn mở đã tải về
	-	/p:Configuration=Release: chọn cấu hình build: Debug|Release
	-	/p:Platform=AnyCPU: chọn flatform để build: AnyCPU|x86|x64
	-	/p:TargetFrameworkSDKToolsDirectory="C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools": chọn đường dẫn SDKTools


  $ script build
  ```  
## Hướng dẫn build

	Vào đây https://docs.google.com/document/d/1pH4kG4GTyQZkT12sCan25hZuzfyXzIsp/edit để xem chi tiết.

## Changelog

	Vào đây https://gitlab.vietsens.vn/ivt-dev/frontend/hisnguonmo/-/blob/Develop/CHANGELOG.md để xem chi tiết.
