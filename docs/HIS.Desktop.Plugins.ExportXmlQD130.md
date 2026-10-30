# Xuất XML QĐ130 — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExportXmlQD130 |
| Loại | UC |
| Mục đích | Xuất/gửi XML giám định BHYT theo QĐ 130/4210 (GIAMDINHHS); đồng bộ dữ liệu KCB (Kết thúc khám/Xuất viện) lên các hệ thống ngoài. |
| Trạng thái | Bảo trì |

## 2. Đích đồng bộ KCB (luồng kcb4750Only: menu "Đồng bộ Khám chữa bệnh" + Luồng 1 tự động)

| Đích | Gate | Khóa HIS_CONFIG | Giao thức |
|------|------|-----------------|-----------|
| CSDL 4750 (CSDL dùng chung ngành y tế) | `MOS.CSDL_4750.IS_AUTO_SYNC=1` + checkbox "Đồng bộ KCB" (Cài đặt) | `HIS.CSDL_4750.CONNECTION_INFO` | multipart file XML, token /get-token |
| **Cổng tiếp nhận KDLYT Vĩnh Long (mới 13/08/2026)** | có khóa VLG + checkbox "Đồng bộ KCB lên Cổng tiếp nhận KDLYT Vĩnh Long (hoàn tất)" (Cài đặt — chỉ hiện khi có khóa) | `MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO` (dùng chung với liên thông KSK VLG) | POST `/api/kham-chua-benh/hoan-tat` — XML GIAMDINHHS trực tiếp (application/xml), Bearer token JSON `/api/xac-thuc/token`; ACCEPTED* = vào hàng đợi (bất đồng bộ) |

Ghi chú `VlgKcbHoanTatWorker`: cùng file XML với luồng 4750 (NOIDUNGFILE Base64 sẵn trong batch); giới hạn 10 MiB; retry 401 một lần; fail-fast cả lô khi login lỗi; không log mật khẩu. **Trạng thái finish (UpdateCsdl4750FinishInfo): khi bật CẢ 2 đích lấy theo 4750 (VLG chỉ ghi dòng kết quả + log); khi CHỈ bật VLG lấy theo VLG** để chu kỳ tự động không chọn lại hồ sơ đã đẩy. Kết quả xử lý thật phía cổng tra ở màn "Tra cứu Cổng KDLYT Vĩnh Long" — loại hồ sơ KCB (theo MA_LK = mã điều trị).

## 2b. Xuất XML hồ sơ đủ 48 giờ (việc 61380, TTYT Phù Ninh)

Code ở `UCExportXml___Xml48h.cs`. Hai cách chạy dùng chung bộ hàm xuất (`ExportAndSaveXml48h`).

| Thông tin | Giá trị |
|-----------|---------|
| Gate chung | `HIS.QD_130_BYT.AUTO_EXPORT_XML_AFTER_48H=1` (tắt khóa: không có cột, ô tích, mục chuột phải). Thư mục khai ở cửa sổ **Thư mục lưu**, dòng "Thư mục lưu XML hồ sơ đủ 48 giờ" (chỉ hiện khi bật khóa; lưu trong `SavePathADO.pathXml48h`) |
| Tự động | Giống các ô đồng bộ QĐ 3176: tích ô **"Tự động xuất XML hồ sơ nhập viện đủ 48 giờ"** trong form Thiết lập gửi hồ sơ tự động (`chkAutoXml48h`, cờ `ConfigSyncADO.isAutoXml48h`, lưu ngay khi tích, ẩn khi tắt khóa) rồi bật **Gửi tự động**. Mỗi chu kỳ (ô Chu kỳ phút) `autoSync_Tick` gọi `DispatchXml48hAutoRound`, độc lập với việc chu kỳ đó có hồ sơ gửi cổng BHYT hay không; tắt Gửi tự động là dừng. Bật Gửi tự động mà chưa khai thư mục thì nhắc một lần, không chặn. Không còn bộ đếm 10 phút riêng |
| Xuất tay | Chuột phải hồ sơ đang chọn → **"Xuất XML hồ sơ đủ 48 giờ"** (`MenuItemClick_XuatXml48h`), không cần bật Gửi tự động; chưa khai thư mục thì mở Thư mục lưu. Dùng cho hồ sơ lỗi, hồ sơ quá 7 ngày lượt tự động không quét tới, máy không bật Gửi tự động. Bỏ qua kèm lý do hồ sơ không đủ điều kiện (không phải ngoại trú/nội trú có số vào viện, chưa có thời gian nhập viện, chưa đủ 48 giờ, ra viện trước 48 giờ); hồ sơ đã xuất thành công thì hỏi trước khi xuất lại (ghi đè file). Xong hiện bảng kết quả (tối đa 30 dòng, còn lại xem log). XML 3176 theo ô XML 3176 trên màn |
| Tìm hồ sơ trên màn | Ô **Tình trạng** có thêm lựa chọn **"Nhập viện đủ 48 giờ"** (id 4, chỉ có khi bật khóa; `ApplyXml48hStatusFilter`): diện ĐT ngoại trú/nội trú (giao với ô Diện điều trị nếu đã chọn), `HAS_IN_CODE`, `CLINICAL_IN_TIME_TO` ≤ hiện tại − 48 giờ, không giới hạn 7 ngày, gồm mọi trạng thái XML 48h (xem cột XML 48h). Chọn lựa chọn này thì loại thời gian tự đổi sang **"Thời gian nhập viện từ:"** (lựa chọn mới của nút loại thời gian, lọc `CLINICAL_IN_TIME_FROM/TO` theo phút, dùng được cả khi không chọn 48 giờ). Hồ sơ ra viện trước 48 giờ vẫn hiện (máy chủ không lọc được), xuất tay sẽ bỏ qua kèm lý do. Lựa chọn 4 không mang sang form Thiết lập gửi tự động (đổi về Tất cả) |
| Hồ sơ (tự động) | `api/HisTreatment/GetView1`: chi nhánh theo ô Chi nhánh của form Thiết lập (trống = chi nhánh phòng làm việc), diện ĐT ngoại trú (2)/nội trú (3), `HAS_IN_CODE`, `CLINICAL_IN_TIME` trong [hiện tại − 48 giờ − 7 ngày, hiện tại − 48 giờ], `HAS_XML_48H_SUCCESS=false`; bỏ hồ sơ ra viện trước khi đủ 48 giờ; tối đa 50/lượt, hồ sơ chưa xuất lần nào lấy trước, rồi tới hồ sơ lỗi có `XML_48H_TIME` cũ nhất (để hồ sơ lỗi mãi không chiếm hết 50 chỗ mỗi lượt). XML 3176 theo ô XML 3176 của form Thiết lập. Các bộ lọc khác của form (đối tượng, diện, loại hồ sơ, loại thời gian) không áp |
| Sinh file | Nạp dữ liệu vào ban giữ riêng (`LoadXmlBatchDataIsolated`, lô 50 hồ sơ) nên không đụng các luồng Xuất Xml / Gửi tự động; `BuildInputAdoForXml` + `CreateXmlProcessor.RunInTreatment` (chế độ hồ sơ đang điều trị), tên file `GetFileName()`, không XML12, không ký, không gửi cổng. Cờ `xml48hRunning` chặn chạy chồng lượt tự động và xuất tay |
| Kết quả | `api/HisTreatment/UpdateXml48hResult` (gửi theo lô 200; HIS_TREATMENT.XML_48H_RESULT 1/2, _TIME, _FILE_PATH, _DESC; lỗi không đè thành công, thành công đè thành công); lỗi thì lượt tự động sau xuất lại; cột "XML 48h" trên lưới, rê chuột xem thời điểm + đường dẫn/lý do |

## 3. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 30/10/2026 | phuongnm | Việc 61380: thêm bộ lọc tìm hồ sơ đủ 48 giờ trên màn: lựa chọn Tình trạng "Nhập viện đủ 48 giờ" và loại thời gian "Thời gian nhập viện từ:" (mục 2b). Chỉ sửa FE, API GetView1 đã có sẵn trường lọc. Cùng ngày đổi chữ "vào khoa điều trị" thành "nhập viện" ở mọi nhãn/thông báo 48 giờ (ô tích, lý do bỏ qua, tooltip) cho đúng tên cột `CLINICAL_IN_TIME` = "Thời gian nhập viện" như các màn khác của HIS; khóa resource `Xml48hLyDoChuaVaoKhoa` → `Xml48hLyDoChuaNhapVien`, `Xml48hLocVaoKhoaDu48h` → `Xml48hLocNhapVienDu48h`. |
| 30/10/2026 | phuongnm | Việc 61380: lượt tự động XML 48 giờ lấy hồ sơ chưa xuất trước, rồi hồ sơ lỗi có lần xuất cũ nhất. Trước đây xếp theo giờ nhập viện, khi có từ 50 hồ sơ lỗi mãi (dev: thiếu địa chỉ) thì mỗi lượt lấy lại đúng 50 hồ sơ đó, hồ sơ mới không bao giờ tới lượt. |
| 29/10/2026 | phuongnm | Việc 61380: xuất XML 48 giờ tự động chuyển sang chạy theo Gửi tự động như QĐ 3176 (ô tích mới trong form Thiết lập gửi hồ sơ tự động, bỏ bộ đếm 10 phút riêng); thêm xuất tay bằng chuột phải "Xuất XML hồ sơ đủ 48 giờ" (mục 2b). Form Thiết lập cao thêm 24px. |
| 27/10/2026 | phuongnm | Việc 61380: tự động xuất XML hồ sơ đủ 48 giờ (mục 2b). Tách nơi chứa dữ liệu lô ra `XmlBatchDataADO`: `CreateThreadGetData`/`ThreadGet*` ghi vào ban giữ; các luồng cũ dùng ban giữ trỏ thẳng vào bộ trường của màn nên hành vi không đổi. |
| 14/08/2026 | khainq | Fix review vòng 2 (build 00:52): gửi tay qua menu khi chế độ auto đang bật không còn fire-and-forget (dialog từng đọc 0 kết quả → bấm lại gây đẩy trùng) — dùng cờ `manualSyncKcb4750` (trước đây khai báo nhưng không bao giờ đọc), lượt gửi tay cũng không bị hủy khi tắt auto giữa chừng; `kcb4750ResultLines` capture local trong task (task lượt trước không Add vào list lượt sau); chặn menu khi `backgroundWorker1.IsBusy` (flags `isNotFileSign` dùng chung — chạy chồng làm lô BHYT tự động gửi KHÔNG ký); VLG-only không còn bị chặn khi IS_AUTO_SYNC=1 mà thiếu key 4750; header dialog đếm theo "lượt gửi"; cast `j["data"] as JObject` trong VlgKcbHoanTatWorker. |
| 13/08/2026 | khainq | Thêm đích đẩy KCB thứ 2 "Cổng tiếp nhận KDLYT Vĩnh Long": `Base/VlgKcbHoanTatWorker.cs` mới; checkbox mới trong frmSettingConfigSync (`isSyncKcbVlg` — lưu ngay khi tick, ẩn khi viện không có khóa); menu "Đồng bộ Khám chữa bệnh" hiện thêm khi có khóa VLG; luồng đẩy nền chung Task với 4750 (lỗi 2 đích cô lập nhau); điều kiện dispatch tự động + hủy giữa lô tính cả isSyncKcbVlg. Sửa build máy backup: 3 chỗ `ado.TotalHeinPatientTypeData` (property chưa có trong DLL His.Bhyt.ExportXml.XML130 bản máy này) → gán reflection `SetAdoPropIfExists` để build cả DLL cũ lẫn mới; tạo `Properties/licenses.licx` rỗng. |

## 4. Test Cases
- [ ] Viện không có khóa VLG + không bật 4750: menu Đồng bộ KCB không hiện; form Cài đặt không có checkbox VLG — hành vi y cũ.
- [ ] Viện chỉ VLG: tick checkbox → menu gửi tay đẩy hoan-tat, dialog kết quả kèm tracking; hồ sơ đã đẩy không bị chọn lại ở chu kỳ tự động.
- [ ] Viện cả 2: mỗi hồ sơ 2 dòng kết quả (4750 + VLG); lỗi VLG không làm hỏng kết quả 4750 và ngược lại.
- [ ] ConfigSync cũ (JSON không có isSyncKcbVlg) → mặc định bỏ tick, không lỗi.
- [ ] Khóa 48 giờ tắt: cửa sổ Thư mục lưu 4 dòng như cũ, form Thiết lập gửi tự động không có ô 48 giờ, chuột phải không có mục "Xuất XML hồ sơ đủ 48 giờ", lưới không có cột XML 48h.
- [ ] Khóa bật, chưa tích ô 48 giờ, bật Gửi tự động: không xuất XML 48 giờ (gửi cổng BHYT chạy như cũ).
- [ ] Khóa bật + khai thư mục + tích ô + bật Gửi tự động: hồ sơ nội trú có số vào viện nhập viện quá 48 giờ được xuất 1 file, cột XML 48h "Đã xuất"; chu kỳ sau không xuất lại. Chu kỳ không có hồ sơ gửi BHYT vẫn xuất 48 giờ.
- [ ] Tích ô nhưng chưa khai thư mục, bật Gửi tự động: hiện nhắc một lần, Gửi tự động vẫn chạy.
- [ ] Tắt Gửi tự động: không còn lượt 48 giờ mới.
- [ ] Xuất tay hồ sơ lỗi / hồ sơ quá 7 ngày: xuất được, cột chuyển "Đã xuất". Hồ sơ chưa đủ 48 giờ, ra viện trước 48 giờ, khám ngoại trú: bỏ qua kèm lý do. Hồ sơ đã xuất: hỏi trước, đồng ý thì ghi đè file.
- [ ] Bấm xuất tay khi lượt tự động đang chạy: báo chờ lượt xong.
- [ ] Tình trạng "Nhập viện đủ 48 giờ": loại thời gian tự đổi sang "Thời gian nhập viện từ:"; danh sách chỉ có ngoại trú/nội trú có số vào viện, nhập viện trong khoảng Từ - Đến và trước hiện tại 48 giờ (dev 30/10: khoảng 20/10 - 30/10 ra 110 hồ sơ, khớp đếm DB). Khóa 48 giờ tắt: ô Tình trạng không có lựa chọn này.
- [ ] Tình trạng Tất cả + loại thời gian "Thời gian nhập viện từ:": lọc đúng theo giờ nhập viện (dev: 181 hồ sơ, khớp DB).
- [ ] Thư mục mạng mất kết nối: hồ sơ ghi "Lỗi" kèm lý do, kết nối lại thì lượt sau xuất được.
- [ ] Đang Xuất Xml / Gửi tự động trong lúc lượt 48 giờ chạy: kết quả hai luồng không lẫn dữ liệu.
