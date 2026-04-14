# Naming Conventions - C# Desktop (Inventec Standard)

## 1. Mục tiêu

Áp dụng quy chuẩn coding nhằm:
- Đồng bộ code trong team
- Giảm lỗi khi phát triển và bảo trì
- Tăng khả năng đọc hiểu và mở rộng

## 2. Naming Convention

### Quy tắc viết hoa

PascalCase:
- Class, Method, Property, Namespace, Enum

CamelCase:
- Variable, Parameter, Private Field

UpperCase:
- Constant

## 3. Quy tắc đặt tên chi tiết

Class:
- Danh từ, PascalCase
- Không prefix (C, Class...)
- Không dùng "_"

Ví dụ:
PatientService
FileStream

Interface:
- Prefix "I"
- PascalCase

Ví dụ:
IPatientService

Method:
- Động từ, PascalCase

Ví dụ:
GetPatients
RemoveAll

Property:
- Danh từ, PascalCase

Ví dụ:
PatientName

Parameter:
- CamelCase
- Rõ nghĩa

Ví dụ:
string patientName

Enum:
- PascalCase
- Không dùng hậu tố Enum

Event:
- Dùng động từ
- Dạng: Closing, Closed
- EventArgs có hậu tố EventArgs

## 4. UI Control Naming (WinForms)

Bắt buộc dùng prefix theo loại control:

Label: lbl  
LinkLabel: llbl  
Button: btn  
TextBox: txt  
MainMenu: mnu  
CheckBox: chk  
RadioButton: rdo  
GroupBox: grp  
PictureBox: pic  
DataGridView: dgv  
Grid: grd  
ListBox: lst  
ComboBox: cbo  
ListView: lstv  
TreeView: tre  
TabControl: tab  
DateTimePicker: dtm  
MonthCalendar: mon  
ScrollBar: sbr  
Timer: tmr  
Splitter: spl  
DomainUpDown: dud  
NumericUpDown: nud  
TrackBar: trk  
ProgressBar: prg  
RichTextBox: rtxt  
ImageList: img  
HelpProvider: hlp  
ToolTip: tip  
ContextMenu: cmnu  
ToolBar: tbr  
Form: frm  
StatusBar: sbrs  
NotifyIcon: nico  
OpenFileDialog: ofd  
SaveFileDialog: sfd  
FontDialog: fdlg  
ColorDialog: cdlg  
PrintDialog: pdlg  
PrintPreviewDialog: ppdlg  
PrintPreviewControl: ppc  
ErrorProvider: err  
PrintDocument: pdoc  
PageSetupDialog: psd  
CrystalReport: crv  
FileSystemWatcher: fsw  
EventLog: log  
DirectoryEntry: dire  
DirectorySearcher: dirs  
MessageQueue: msq  
PerformanceCounter: pco  
Process: proc  
ServiceController: ser  
ReportDocument: rpt  
DataSet: ds  
OleDbDataAdapter: olea  
OleDbConnection: olec  
OleDbCommand: oled  
SqlDataAdapter: sqla  
SqlConnection: sqlc  
SqlCommand: sqld  
DataView: dvw  

Nguyên tắc:
- Prefix + Tên nghiệp vụ (PascalCase)
- Không đặt tên chung chung

Ví dụ:
txtPatientName
btnSearch
dgvPatientList
cboCustomerCode

## 5. Menu Naming

Format:
mnu + Parent + Child

Ví dụ:
mnuFile
mnuFileNew
mnuEditCopy
mnuInsertIndexAndTables

## 6. Naming theo Data Binding

Field: CustomerCode

TextBox:
txtCustomerCode

ComboBox:
cboCustomerCode

Biến module:
mCustomerCode

## 7. Quy tắc đặt tên - Lưu ý

- Không dùng tên chỉ khác nhau chữ hoa/thường
- Không viết tắt khó hiểu
- Không trùng keyword .NET
- Không dùng "_" trong class/interface
- Có thể dùng viết tắt phổ biến: XML, UI, IO

## 8. Tổ chức file C#

- Mỗi class 1 file
- Tên file = tên class
- Không vượt quá 2000 dòng

Thứ tự file:
1. using
2. namespace
3. class/interface

## 9. Thứ tự trong class

1. Fields (private → protected → internal → public)
2. Properties
3. Constructors
4. Methods (group theo chức năng)

## 10. Namespace

Format:
CompanyName.Technology.Feature

Ví dụ:
Inventec.HIS.Patient
Inventec.BHYT.XML3200

## 11. Quy tắc method

- < 50 dòng
- 1 nhiệm vụ
- Không nested sâu

## 12. Exception Handling

- Không bỏ qua exception
- Luôn log
- Không catch rỗng
- Thông báo rõ cho user

## 13. Kiến trúc bắt buộc

3 layer:

UI:
- Form/UserControl
- Không business logic

Service:
- Xử lý nghiệp vụ

Repository/API:
- Truy xuất dữ liệu

Không được:
- Gọi API trực tiếp trong UI
- Xử lý logic trong Form

## 14. Database Naming

- PascalCase
- Giống Property

Ví dụ:
CustomerCode
PatientName

## 15. Clean Code

- Không duplicate
- Tên rõ nghĩa
- Ưu tiên dễ đọc
- Refactor thường xuyên

## 16. Quy tắc bắt buộc cho Claude

- Luôn tuân thủ toàn bộ rules này khi sinh code
- Không tự ý thay đổi naming convention
- Luôn dùng đúng prefix control
- Không viết logic trong UI
- Nếu thiếu thông tin phải hỏi lại, không suy đoán
