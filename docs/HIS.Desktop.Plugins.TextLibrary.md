# Thư Viện Nội Dung — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TextLibrary |
| Loại | Form (`FormTextLibrary`, kế thừa `FormBase`) + popup `ShareAccount\frmShareAccount` |
| Mục đích | Quản lý nội dung mẫu (văn bản RTF hoặc hình ảnh) dùng lại khi nhập hồ sơ: tiêu đề, mã tắt (gõ viết tắt), thẻ (hashtag), chia sẻ toàn viện / theo khoa / theo tài khoản. Mở từ menu (quản lý) hoặc từ nút thư viện trong hồ sơ (chọn mẫu đưa vào ô đang nhập). |
| Người tạo | (kế thừa — plugin có sẵn) |
| Ngày tạo tài liệu | 27/10/2026 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở màn: nạp khoa của tài khoản đăng nhập (`V_HIS_EMPLOYEE.DEPARTMENT_ID`) → gọi `api/HisTextLib/Get` với `CAN_VIEW = true` + `PUBLIC_DEPARTMENT_ID` (MOS lọc theo tài khoản trong token), phân trang `ucPaging`, sắp `MODIFY_TIME DESC`.
2. Bấm 1 dòng: nạp nội dung lên vùng phải (tiêu đề, mã tắt, thẻ, Văn bản/Hình ảnh, 3 ô chia sẻ, danh sách tài khoản được chia sẻ). Chỉ người tạo (`CREATOR`) hoặc quản trị (`HIS_EMPLOYEE.IS_ADMIN`, `CheckLoginAdmin.IsAdmin`) mới được Lưu/Xóa/đổi chia sẻ theo tài khoản.
3. **Mới** → xóa vùng phải, mở lại nút Lưu, ô Chia sẻ theo tài khoản trống, "(0 tài khoản)".
4. **Lưu** → `api/HisTextLib/Create` (mới) hoặc `Update` (đang sửa) → reset cache `HIS_TEXT_LIB`, nạp lại lưới; nếu mở từ hồ sơ thì trả nội dung về qua `DelegateDataTextLib`.
5. **Chọn** (chỉ hiện khi mở từ hồ sơ): người tạo/quản trị → lưu rồi trả nội dung về hồ sơ; người được chia sẻ → trả bản sao nội dung đang hiện, KHÔNG gọi Update lên nội dung của người tạo. Nhấp đúp 1 dòng → trả dòng đó về hồ sơ.

### Chia sẻ theo tài khoản (việc 59771, tài liệu phân tích 3604)
- Hàng dưới ô Thẻ: ô tích **Chia sẻ theo tài khoản** + nút **"+"** + nhãn **"(N tài khoản)"**.
- Nút "+" sáng khi: được sửa (bản ghi mới, hoặc người tạo/quản trị) **và** đang tích Chia sẻ theo tài khoản **và** chưa tích Chia sẻ toàn viện. Nhãn luôn hiện số tài khoản đã chọn (kể cả khi ô bỏ tích hay đang tích toàn viện — danh sách giữ nguyên).
- Popup **Danh sách tài khoản** (`frmShareAccount`):
  - Nguồn: danh mục Tài khoản nhân viên trong bộ nhớ (`V_HIS_EMPLOYEE`): `IS_ACTIVE = 1`, `IS_DELETE != 1`, có `LOGINNAME`, khác người tạo nội dung (bản ghi mới = người đăng nhập, đang sửa = `CREATOR`); sắp theo họ tên. Tài khoản đã chọn được tích sẵn.
  - Ô tìm (mã NV, họ tên, tên đăng nhập — không phân biệt hoa thường, có dấu/không dấu) + combo Khoa (khoa đang hoạt động, nút xóa để bỏ lọc). Lọc chỉ ẩn dòng, trạng thái tích ngoài kết quả lọc được giữ. Lưới tắt lọc tiêu đề cột và ô tìm Ctrl+F riêng (chỉ lọc qua ô tìm + combo Khoa, để "Chọn tất cả" đúng theo dòng đang hiện).
  - "Chọn tất cả" tích/bỏ tích mọi dòng đang hiện; tự cập nhật khi đổi lọc hoặc tích từng dòng.
  - **Đồng ý**: kết quả = tài khoản đang tích + tài khoản đầu vào không có trong popup (bị khóa / không còn trong danh mục — giữ lại), bỏ người tạo, bỏ trùng. Nếu chuỗi ghép dấu phẩy > 3998 byte UTF-8 (cột VARCHAR2(4000) tính theo byte; MOS so khớp `','||PUBLIC_LOGINNAMES||','` nên phải chừa 2 byte, quá 4000 Oracle báo ORA-01489) → báo `DanhSachTaiKhoanChiaSeVuotQuaGioiHan`, không đóng. **Đóng** → không đổi danh sách.
  - Kết quả chỉ ghi xuống CSDL khi bấm Lưu ở màn chính.
- Lưu: `IS_PUBLIC_IN_ACCOUNT = 1/0` theo ô tích; `PUBLIC_LOGINNAMES` = loginname ngăn bằng dấu phẩy, không khoảng trắng; danh sách rỗng → `null`.

### Quy tắc hiển thị (tra cứu ở MOS, gõ viết tắt ở `HIS.Desktop.Utility.ShortcutReplace.GetUsableTextLibs`)
Nội dung được dùng khi thỏa 1 trong 4 điều kiện:
1. Của mình (`CREATOR` = tài khoản đăng nhập).
2. Chia sẻ toàn viện (`IS_PUBLIC = 1`).
3. Chia sẻ theo khoa (`IS_PUBLIC_IN_DEPARTMENT = 1` và `DEPARTMENT_ID` = khoa của tài khoản nhân viên).
4. Chia sẻ theo tài khoản (`IS_PUBLIC_IN_ACCOUNT = 1` và `("," + PUBLIC_LOGINNAMES + ",")` chứa `",<loginname>,"`) — chỉ khi tài khoản nhân viên đang hoạt động (`V_HIS_EMPLOYEE.IS_ACTIVE = 1`).

### Điều kiện nghiệp vụ / giới hạn
- Gõ viết tắt đọc danh mục thư viện nạp 1 lần mỗi phiên → chia sẻ mới chỉ có hiệu lực sau khi người được chia sẻ đăng nhập lại; màn Thư viện nội dung thấy ngay khi mở lại màn.
- Khóa tài khoản nhân viên có hiệu lực ở MOS khi MOS nạp lại danh mục nhân viên (`HisEmployeeCFG`), ở gõ viết tắt khi đăng nhập lại.
- MOS không chặn Sửa/Xóa nội dung của người khác (chỉ FE chặn) — giữ như cũ.
- Cấu hình máy `CONFIG_KEY__HIS_IS_USE_SHORTCUT_REPLACE_KEY = 2` (chỉ dùng mẫu của mình) không đổi.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_TEXT_LIB | Table | Nội dung thư viện (lưới, tạo/sửa/xóa) |
| V_HIS_EMPLOYEE | View (cache) | Khoa của tài khoản đăng nhập; nguồn popup Danh sách tài khoản; trạng thái hoạt động khi gõ viết tắt |
| HIS_EMPLOYEE | Table (API/cache) | Khoa người tạo khi lưu (`api/HisEmployee/Get`); `IS_ADMIN` (quản trị) |
| HIS_DEPARTMENT | Table (cache) | Combo Khoa ở popup |

Cột `HIS_TEXT_LIB` liên quan chia sẻ: `IS_PUBLIC`, `IS_PUBLIC_IN_DEPARTMENT` + `DEPARTMENT_ID`, `IS_PUBLIC_IN_ACCOUNT` NUMBER(2) và `PUBLIC_LOGINNAMES` VARCHAR2(4000) (thêm ở việc 59771, cần `MOS.EFMODEL.dll` mới có 2 thuộc tính này).

## 4. UI Layout

### Sơ đồ giao diện
```
+-------------------------------------------+------------------------------------------------+
| [Thẻ (F2)] [Từ khóa] [Tìm (Ctrl F)]       | Tiêu đề: [..................] [ ] Chia sẻ toàn viện |
|                                           | Mã tắt:  [..................] [ ] Chia sẻ theo khoa |
| Lưới: STT | Xóa | Tiêu đề | Thẻ | Mã tắt | | Thẻ:     [.....................................]  |
|  Nội dung | Chia sẻ toàn viện | theo khoa | | [ ] Chia sẻ theo tài khoản [+] (N tài khoản)     |
|  TG tạo | Người tạo | TG sửa | Người sửa  | (o) Văn bản  ( ) Hình ảnh                        |
|                                           | [UCDocument / UCPicture]                         |
| [Phân trang]                              |               [Chọn (Ctrl T)] [Lưu (Ctrl S)] [Mới (Ctrl N)] |
+-------------------------------------------+------------------------------------------------+

Popup Danh sách tài khoản:
+---------------------------------------------------------------------------+
| [Mã NV, họ tên, tên đăng nhập]   Khoa: [............ v x]  [ ] Chọn tất cả |
| [x] | Mã NV | Họ tên | Tên đăng nhập | Khoa/phòng                           |
|                                                   [Đồng ý]  [Đóng]          |
+---------------------------------------------------------------------------+
```

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| UC\UCDocument (nội bộ plugin) | panelControl1 | Soạn nội dung văn bản (RichEdit) |
| UC\UCPicture (nội bộ plugin) | panelControl1 | Nội dung hình ảnh (base64) |
| Inventec.UC.Paging | ucPaging | Phân trang lưới |
| Inventec.Common.Controls.EditorLoader | cboDepartment (popup) | Nạp combo Khoa |

## 5. API Endpoints

| Action | URI | Consumer | Filter / dữ liệu |
|--------|-----|----------|------------------|
| Lấy danh sách | `GlobalStore.HIS_TEXT_LIB_GET` (api/HisTextLib/Get) | MosConsumer | `HisTextLibFilter` (`CAN_VIEW = true`, `PUBLIC_DEPARTMENT_ID`, `KEY_WORD`, `HASHTAGs`) |
| Tạo mới | `GlobalStore.HIS_TEXT_LIB_CREATE` (api/HisTextLib/Create) | MosConsumer | `HIS_TEXT_LIB` |
| Cập nhật | `GlobalStore.HIS_TEXT_LIB_UPDATE` (api/HisTextLib/Update) | MosConsumer | `HIS_TEXT_LIB` |
| Xóa | `GlobalStore.HIS_TEXT_LIB_DELETE` (api/HisTextLib/Delete) | MosConsumer | `HIS_TEXT_LIB` |
| Khoa người tạo | `GlobalStore.HIS_EMPLOYEE` (api/HisEmployee/Get) | MosConsumer | `HisEmployeeFilter.LOGINNAME__EXACT` |

Việc 59771 không thêm API; popup đọc danh mục nhân viên từ bộ nhớ (`BackendDataWorker`).

## 6. Dependencies

### Library / thư viện dùng chung
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.IsAdmin (`CheckLoginAdmin`) | Xác định quản trị |
| HIS.Desktop.Utility (`TextLibHelper`, `ShortcutReplace`) | Đọc nội dung RTF; gõ viết tắt dùng chung quy tắc hiển thị |
| Inventec.Common.String (`Convert.UnSignVNese`) | Tìm không dấu ở popup |

### Inter-Plugin
| Plugin gọi | Khi nào mở | Args truyền |
|------------|-----------|-------------|
| ExamServiceReqExecute, ServiceExecute, AssignPrescription*, EnterKskInfomantionVer2, KskInfomantionOfficials, ... | Nút thư viện trong hồ sơ | `Module`, `string` (hashtag / "…danhsachmau" / "…luumau"), `DelegateDataTextLib` (nhận nội dung chọn), `HIS.Desktop.ADO.TextLibraryInfoADO` (tùy chọn: IsNotSaveTemplate, IsFindTemplate, IsFillHashtag, IsFillContent, Hashtag, Content) |

## 7. Print

Không có chức năng in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 27/10/2026 | DANGTH | Tạo tài liệu module. Việc 59771 (tài liệu phân tích 3604): thêm **Chia sẻ theo tài khoản** — ô tích + nút "+" + nhãn "(N tài khoản)" (`FormTextLibrary__ShareAccount.cs`), popup `ShareAccount\frmShareAccount` (+ `ADO\ShareAccountADO`), lưu `IS_PUBLIC_IN_ACCOUNT`/`PUBLIC_LOGINNAMES`; khóa ô/nút khi không phải người tạo/quản trị. Sửa lỗi có sẵn: Chọn trên nội dung người khác không còn gọi Update; Mới mở lại nút Lưu. Gõ viết tắt (`HIS.Desktop.Utility.ShortcutReplace.GetUsableTextLibs`) thêm điều kiện chia sẻ theo tài khoản (nhân viên phải đang hoạt động). Resource vi/en/my, thông báo vượt 4000 ký tự. Thêm tham chiếu Inventec.Common.String, Inventec.Common.Controls.EditorLoader. Cần MOS.EFMODEL mới (2 thuộc tính) + MOS đã vá `HisTextLibFilterQuery`. |
| 27/10/2026 | DANGTH | Việc 59771 sửa theo phản biện: popup tắt lọc tiêu đề cột/ô tìm Ctrl+F của lưới (Chọn tất cả chỉ theo dòng đang hiện); "Chọn tất cả" không còn tích khi kết quả lọc rỗng; giới hạn tính theo byte UTF-8 và hạ xuống 3998 (tránh ORA-01489 khi MOS nối dấu phẩy 2 đầu, đã tái hiện trên CSDL test MAX_STRING_SIZE=STANDARD); `ShortcutReplace` dùng hằng `IMSys.DbConfig` thay số 1 và cập nhật chú thích hàm `ProcesseShortcutReplaceWithModuleServiceExecute`. |

## 9. Test Cases

### Tạo mới / sửa / xóa
- [ ] Mới → nhập tiêu đề, mã tắt, nội dung → Lưu thành công, lưới nạp lại.
- [ ] Thiếu tiêu đề → cảnh báo tại ô Tiêu đề.
- [ ] Bấm dòng của mình → sửa → Lưu; Xóa → xác nhận → xóa.

### Chia sẻ theo tài khoản (mục III thiết kế 59771)
- [ ] Mới: "+" mờ; tích Chia sẻ theo tài khoản → "+" sáng; popup không có người tạo, chỉ tài khoản đang hoạt động.
- [ ] Tìm không dấu / theo mã NV / tên đăng nhập; lọc khoa; "Chọn tất cả" chỉ tích dòng đang hiện; bỏ lọc → dòng khoa khác không bị tích; Đóng → nhãn không đổi.
- [ ] Đồng ý → "(N tài khoản)" → Lưu; bấm lại dòng: ô tích + số giữ nguyên, popup tích sẵn.
- [ ] Danh sách > 3998 byte → Đồng ý bị chặn, báo bỏ bớt; đúng 3998 → Đồng ý được.
- [ ] Người được chia sẻ: thấy nội dung; Lưu, Xóa, ô Chia sẻ theo tài khoản, "+" bị khóa; gõ mã tắt + dấu cách → thay nội dung (sau khi đăng nhập lại).
- [ ] Người được chia sẻ bấm Chọn (mở từ hồ sơ) → nội dung vào hồ sơ, `MODIFIER`/`MODIFY_TIME` của nội dung gốc không đổi.
- [ ] Bỏ tích A → Đồng ý → Lưu: A không còn thấy. Bỏ tích ô → Lưu: không ai trong danh sách thấy; tích lại → thấy lại (danh sách còn).
- [ ] Tích Chia sẻ toàn viện → "+" mờ, nhãn giữ số; bỏ tích → "+" sáng lại.
- [ ] Khóa tài khoản nhân viên A → A không thấy; popup không còn A, nhãn vẫn đếm A.
- [ ] Quản trị sửa nội dung người khác → đổi được danh sách (popup không hiện người tạo).
- [ ] Bấm Mới sau khi xem nội dung người khác → Lưu mở, ô trống, "(0 tài khoản)".
- [ ] Dữ liệu cũ: ô trống, "(0 tài khoản)"; theo khoa / toàn viện như cũ.
- [ ] Tiếng Anh: "Share by account", "(N accounts)", popup "Account list".
