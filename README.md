<!-- markdownlint-disable-next-line -->
<p align="center">
  <a href="https://nguonmo.benhvienthongminh.vn/ords/f?p=106:1:9302229919244:::::" rel="noopener" target="_blank"><img width="150" height="133" src="https://nguonmo.benhvienthongminh.vn/i/apex_ui/img/favicons/hispro/hispro-180.png" alt="hisnguonmo logo"></a>
</p>

<h1 align="center">His nguồn mở</h1>

**His nguồn mở** Phần mềm chạy trên hệ điều hành Window của hệ thống quản lý bệnh viện HisPro, phát hành theo [[giấy phép GPL v3.0]](https://github.com/Vietsens/hisnguonmo?tab=GPL-3.0-1-ov-file):

- [HIS](https://github.com/Vietsens/hisnguonmo/tree/Develop/HIS) source code HIS main project và các tính năng nghiệp vụ(plugin).  
- [MPS](https://github.com/Vietsens/hisnguonmo/tree/Develop/MPS) source code các tính năng in ấn.  
- [UC](https://github.com/Vietsens/hisnguonmo/tree/Develop/UC) source code các thành phần giao diện dùng chung, được nhúng trong các plugin.  
- [Lib](https://github.com/Vietsens/lib) Cá thư viện mở sử dụng trong dự án.  


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
	  $ script build
  ```  	
	
  > Tạo sẵn folder LIB để lưu bộ thư viện tải về và chạy các lệnh sau để clone các thư viện mở phiên bản tương thích về máy. Hoặc có thể vào trực tiếp các trang cung cấp thư viện nguồn mở nhưng lưu ý tính tương thich của phiên bản sử dụng với mã nguồn
	```shell	
	git clone https://github.com/Vietsens/lib.git
	  $ script build
	```  
  
  > Tải các thư viện mở rộng về lưu trong folder LIB ở trên
	```shell	
		$zipUrl = "http://fsstest.onelink.vn/Upload/HIS/HisNguonMo/lib_extend.zip"
		$zipPath = "G:\HISNGMOGITHUB\lib\lib_extend.zip"
		$extractPath = "G:\HISNGMOGITHUB\lib"

		# Tải file zip
		Invoke-WebRequest -Uri "http://fsstest.onelink.vn/Upload/HIS/HisNguonMo/lib_extend.zip" -OutFile "G:\HISNGMOGITHUB\lib\lib_extend.zip"

		# Giải nén file zip
		Expand-Archive -Path "G:\HISNGMOGITHUB\lib\lib_extend.zip" -DestinationPath "G:\HISNGMOGITHUB\lib"
		
		# Xóa file zip sau khi giải nén
		Remove-Item -Path "G:\HISNGMOGITHUB\lib\lib_extend.zip"
	  $ script build
	```    
  
  
- Sau khi clone các git cần thiết về tổ chức folder theo cây folder như sau:

	++ hisnguonmo  
	++++++++ HIS  
	++++++++ UC  
	++++++++ MPS  
	++ lib  
	
## Build
- Lệnh build:
  > với win 32 bit
	```shell
	cd C:\Windows\Microsoft.NET\Framework\v4.0.30319 với win 32 bit
	  $ script build
	``` 
  > với win 64 bit
	```shell
	cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319 với win 64 bit
  	  $ script build
	```    
  > chạy lệnh build project main
  ```shell
	MSBuild.exe E:\HisNguonMo\hisnguonmo\HIS\HIS.Desktop\HIS.Desktop.csproj /p:Configuration=Release /p:Platform=AnyCPU /p:TargetFrameworkSDKToolsDirectory="C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools"
  	  $ script build
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
   	  $ script build
	```   
	cần sửa lại zipPath và extractPath cho đúng với đường dẫn cần lưu
	có thể chạy từng dòng lệnh một hoặc muốn chạy tất cả các lệnh 1 lần thì lưu script này ra file DownloadAndExtractExtend.ps1, và sau đó chạy file này trong PowerShell:
    ```shell
		.\DownloadAndExtractExtend.ps1
		$ script build
	```  
	Nếu bạn chưa bao giờ chạy script PowerShell trước đây, bạn có thể cần thay đổi chính sách thực thi để cho phép chạy script:
	 ```shell
		Set-ExecutionPolicy RemoteSigned
		$ script build
	```  

## Thông tin phát hành

	Vào đây https://github.com/Vietsens/hisnguonmo/blob/Develop/CHANGELOG.md để xem chi tiết.
	
## Kênh liên lạc

- Mailing lists: hisnguonmo@googlegroups.com
	
	
## Tham khảo

- Build và tổ chức folder sau khi build: https://docs.google.com/document/d/1pH4kG4GTyQZkT12sCan25hZuzfyXzIsp/edit?usp=drive_link&ouid=100318684171831156204&rtpof=true&sd=true
- Tạo project plugin: https://docs.google.com/document/d/1exLo2zBCBG6u2YSK9LK00ggYSsFr-nNN/edit?usp=sharing&ouid=100318684171831156204&rtpof=true&sd=true
