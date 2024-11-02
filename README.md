<!-- markdownlint-disable-next-line -->
<p align="center">
  <a href="https://nguonmo.benhvienthongminh.vn/ords/f?p=106:1:9302229919244:::::" rel="noopener" target="_blank"><img width="150" height="133" src="https://nguonmo.benhvienthongminh.vn/i/apex_ui/img/favicons/hispro/hispro-180.png" alt="hisnguonmo logo"></a>
</p>

<h1 align="center">His nguồn mở</h1>

**His nguồn mở** Phần mềm chạy trên hệ điều hành Window của hệ thống quản lý bệnh viện HisPro, phát hành theo [[giấy phép GPL v3.0]](https://github.com/Vietsens/hisnguonmo?tab=GPL-3.0-1-ov-file):

- [HIS](https://github.com/Vietsens/hisnguonmo/tree/Develop/HIS) source code HIS main project và các tính năng nghiệp vụ(plugin).  
- [MPS](https://github.com/Vietsens/hisnguonmo/tree/Develop/MPS) source code các tính năng in ấn.  
- [UC](https://github.com/Vietsens/hisnguonmo/tree/Develop/UC) source code các thành phần giao diện dùng chung, được nhúng trong các plugin.  
- [Common](https://github.com/Vietsens/hisnguonmo/tree/Develop/Common) source code các thư viện common.  


## Thư viện

### Nguồn đóng
- [DevExpress 15.2.9](https://www.devexpress.com/ "DevExpress 15.2.9")	
- [FlexCell 5.7.6.0](https://www.tmssoftware.com/site/flexcelnet.asp "FlexCell 5.7.6.0"): Xử lý in ấn, báo cáo
- [TELERIK](https://www.telerik.com/ "Telerik UI for WinForms 2019.3.1022")	
- [Aspose 11.1](https://www.nuget.org/packages/Aspose.Words/11.1.0 "Aspose 11.1"): Xử lý văn bản điện tử dạng word
- [EO.Pdf](https://www.nuget.org/packages/EO.Pdf/20.3.34 "EO.Pdf"): Chuyển đổi nội dung từ các site tích hợp sang PDF
- [EO.WebBrowser](https://www.nuget.org/packages/EO.WebBrowser/20.3.34 "EO.WebBrowser"): Hiển thị nội dung từ các site tích hợp
- [BarTender 10.1.0](https://sonamin.com/blogs/download-phan-mem-driver/tai-bartender "EO.WebBrowser"): Xử lý in barcode
- [STPadLibNet](https://en.signotec.com/portal/seiten/signopad-api-device-api--900000170-10002.html "STPadLibNet"): Kết nối bảng ký

### Nguồn mở cộng đồng
- log4net: Apache License https://www.apache.org/licenses/
- DirectX.Capture:	https://www.codeproject.com/Articles/4740/Capture-Sample-with-DirectX-and-NET
- AForge: LGPL v3,GPL v3 https://www.aforgenet.com/framework/license/
- itextsharp: https://www.gnu.org/licenses/agpl-3.0.html
- Newtonsoft.Json: https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md
- EntityFramework: https://vi.wikipedia.org/wiki/Entity_Framework
- Osinfo: https://learn.microsoft.com/en-us/powershell/dsc/reference/microsoft/osinfo/cli/osinfo?view=dsc-3.0&tabs=windows
- NAudio: https://github.com/naudio/NAudio?tab=MIT-1-ov-file
- BouncyCastle: https://www.bouncycastle.org/about/license/#License
- MessagePack: MIT License https://github.com/MessagePack-CSharp/MessagePack-CSharp?tab=License-1-ov-file
- Microsoft.IdentityModel.JsonWebTokens, System.IdentityModel.Tokens.Jwt: MIT license https://www.nuget.org/packages/Microsoft.IdentityModel.JsonWebTokens/



## Yêu cầu môi trường
	Máy tính cần cài đặt sẵn các phần mềm sau:

•	Git: https://git-scm.com/downloads

•	.Net framework 4.5:  https://www.microsoft.com/en-us/download/details.aspx?id=42642

•	Microsoft Build Engine(MSBuild): Có thể dùng phiên bản tích hợp sẵn trong .net framework 
	hoặc tải phiên bản tùy chọn ở đây https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild?view=vs-2022




## Clone source code

- Clone source code	từ git: mở windows powershell và thực hiện chạy các lệnh sau
  > Tạo sẵn folder HISNGUONMO để lưu source code tải về và chạy lệnh bên dưới để clone về máy
  ```shell	
	git clone https://github.com/Vietsens/hisnguonmo.git
  ```  	
	
  
  > Tải các thư viện bản build sẵn với phiên bản tương thích về lưu trong folder LIB ở trên máy
	```shell	
		$zipUrl = "http://fsstest.onelink.vn/Upload/HIS/HisNguonMo/lib_extend.zip"
		$zipPath = "E:\HisNguonMo\hisnguonmo\lib\lib_extend.zip"
		$extractPath = "E:\HisNguonMo\hisnguonmo\lib"

		# Tải file zip
		Invoke-WebRequest -Uri $zipUrl -OutFile $zipPath

		# Giải nén file zip
		Expand-Archive -Path $zipPath -DestinationPath $extractPath
		
		# Xóa file zip sau khi giải nén
		Remove-Item -Path $zipPath
	```    
  
  
- Sau khi clone các git cần thiết về tổ chức folder theo cây folder như sau:

	++ hisnguonmo  
	++++++++ HIS  
	++++++++ UC  
	++++++++ MPS  
	++++++++ Common  
	++ lib  
	
## Build
- Lệnh build:
  > với win 32 bit
	```shell
	cd C:\Windows\Microsoft.NET\Framework\v4.0.30319 với win 32 bit
	``` 
  > với win 64 bit
	```shell
	cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319 với win 64 bit
	```    
  > chạy lệnh build project main
  ```shell
	MSBuild.exe E:\HisNguonMo\hisnguonmo\HIS\HIS.Desktop\HIS.Desktop.csproj /p:Configuration=Release /p:Platform=AnyCPU /p:TargetFrameworkSDKToolsDirectory="C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools"
  ```  	
	Lưu ý: cần sửa lại tham số cấu hình trong lệnh build cho khớp với môi trường thực tế của máy tính
	Trong đó
	-	E:\HisNguonMo\hisnguonmo\HIS\HIS.Desktop\HIS.Desktop.csproj là đường dẫn đến file cs project của main project his nguồn mở đã tải về
	-	/p:Configuration=Release: chọn cấu hình build: Debug|Release
	-	/p:Platform=AnyCPU: chọn flatform để build: AnyCPU|x86|x64
	-	/p:TargetFrameworkSDKToolsDirectory="C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools": chọn đường dẫn SDKTools

- Chạy thử với các thành phần build sẵn
  Bạn có thể tạo một script PowerShell để thực hiện việc tải và lưu các thành phần build sẵn vào folder chứa phiên bản his sau khi build ở trên:
    ```shell
		$zipUrl = "http://fsstest.onelink.vn/Upload/HIS/HisNguonMo/extend.zip"
		$zipPath = "E:\HisNguonMo\hisnguonmo\Build\hisnguonmo_extend.zip"
		$extractPath = "E:\HisNguonMo\hisnguonmo\Build"

		# Tải file zip
		Invoke-WebRequest -Uri $zipUrl -OutFile $zipPath

		# Giải nén file zip
		Expand-Archive -Path $zipPath -DestinationPath $extractPath
		
		# Xóa file zip sau khi giải nén
		Remove-Item -Path $zipPath
	```   
	cần sửa lại zipPath và extractPath cho đúng với đường dẫn cần lưu
	có thể chạy từng dòng lệnh một hoặc muốn chạy tất cả các lệnh 1 lần thì lưu script này ra file DownloadAndExtractExtend.ps1, và sau đó chạy file này trong PowerShell:
    ```shell
		.\DownloadAndExtractExtend.ps1
	```  
	Nếu bạn chưa bao giờ chạy script PowerShell trước đây, bạn có thể cần thay đổi chính sách thực thi để cho phép chạy script:
	 ```shell
		Set-ExecutionPolicy RemoteSigned
	```  

## Thông tin phát hành

	Vào đây https://github.com/Vietsens/hisnguonmo/blob/Develop/CHANGELOG.md để xem chi tiết.
	
## Kênh liên lạc

- Mailing lists: hisnguonmo@googlegroups.com
- Cách tham gia kênh https://support.google.com/groups/answer/1067205

## Issues

- Các vấn đề liên quan đến sản phẩm vui lòng phản hồi tại [đây](https://github.com/Vietsens/hisnguonmo/issues)
	
## Tham khảo

[Tài liệu hướng dẫn](https://github.com/Vietsens/hisnguonmo/wiki)
