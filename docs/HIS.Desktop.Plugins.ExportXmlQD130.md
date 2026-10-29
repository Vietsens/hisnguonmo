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

## 2b. Tự động xuất XML hồ sơ đủ 48 giờ (việc 61380, TTYT Phù Ninh)

| Thông tin | Giá trị |
|-----------|---------|
| Gate | `HIS.QD_130_BYT.AUTO_EXPORT_XML_AFTER_48H=1` + đã khai "Thư mục lưu XML hồ sơ đủ 48 giờ" (cửa sổ Thư mục lưu, dòng chỉ hiện khi bật khóa; lưu trong `SavePathADO.pathXml48h`) |
| Chạy | Bộ đếm giờ riêng 10 phút, lượt đầu chạy ngay khi mở màn/lưu thư mục; chạy nền, nạp dữ liệu vào ban giữ riêng (`LoadXmlBatchDataIsolated`) nên không đụng các luồng Xuất Xml / Gửi tự động |
| Hồ sơ | `api/HisTreatment/GetView1`: chi nhánh phòng làm việc, diện ĐT ngoại trú (2)/nội trú (3), `HAS_IN_CODE`, `CLINICAL_IN_TIME` trong [hiện tại − 48 giờ − 7 ngày, hiện tại − 48 giờ], `HAS_XML_48H_SUCCESS=false`; bỏ hồ sơ ra viện trước khi đủ 48 giờ; tối đa 50/lượt |
| Sinh file | `BuildInputAdoForXml` + `CreateXmlProcessor.RunInTreatment` (chế độ hồ sơ đang điều trị), tên file `GetFileName()`, không XML12, không ký, không gửi cổng |
| Kết quả | `api/HisTreatment/UpdateXml48hResult` (HIS_TREATMENT.XML_48H_RESULT 1/2, _TIME, _FILE_PATH, _DESC); lỗi thì lượt sau tự xuất lại; cột "XML 48h" trên lưới, rê chuột xem thời điểm + đường dẫn/lý do |

## 3. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 27/10/2026 | phuongnm | Việc 61380: tự động xuất XML hồ sơ đủ 48 giờ (mục 2b). Tách nơi chứa dữ liệu lô ra `XmlBatchDataADO`: `CreateThreadGetData`/`ThreadGet*` ghi vào ban giữ; các luồng cũ dùng ban giữ trỏ thẳng vào bộ trường của màn nên hành vi không đổi. |
| 14/08/2026 | khainq | Fix review vòng 2 (build 00:52): gửi tay qua menu khi chế độ auto đang bật không còn fire-and-forget (dialog từng đọc 0 kết quả → bấm lại gây đẩy trùng) — dùng cờ `manualSyncKcb4750` (trước đây khai báo nhưng không bao giờ đọc), lượt gửi tay cũng không bị hủy khi tắt auto giữa chừng; `kcb4750ResultLines` capture local trong task (task lượt trước không Add vào list lượt sau); chặn menu khi `backgroundWorker1.IsBusy` (flags `isNotFileSign` dùng chung — chạy chồng làm lô BHYT tự động gửi KHÔNG ký); VLG-only không còn bị chặn khi IS_AUTO_SYNC=1 mà thiếu key 4750; header dialog đếm theo "lượt gửi"; cast `j["data"] as JObject` trong VlgKcbHoanTatWorker. |
| 13/08/2026 | khainq | Thêm đích đẩy KCB thứ 2 "Cổng tiếp nhận KDLYT Vĩnh Long": `Base/VlgKcbHoanTatWorker.cs` mới; checkbox mới trong frmSettingConfigSync (`isSyncKcbVlg` — lưu ngay khi tick, ẩn khi viện không có khóa); menu "Đồng bộ Khám chữa bệnh" hiện thêm khi có khóa VLG; luồng đẩy nền chung Task với 4750 (lỗi 2 đích cô lập nhau); điều kiện dispatch tự động + hủy giữa lô tính cả isSyncKcbVlg. Sửa build máy backup: 3 chỗ `ado.TotalHeinPatientTypeData` (property chưa có trong DLL His.Bhyt.ExportXml.XML130 bản máy này) → gán reflection `SetAdoPropIfExists` để build cả DLL cũ lẫn mới; tạo `Properties/licenses.licx` rỗng. |

## 4. Test Cases
- [ ] Viện không có khóa VLG + không bật 4750: menu Đồng bộ KCB không hiện; form Cài đặt không có checkbox VLG — hành vi y cũ.
- [ ] Viện chỉ VLG: tick checkbox → menu gửi tay đẩy hoan-tat, dialog kết quả kèm tracking; hồ sơ đã đẩy không bị chọn lại ở chu kỳ tự động.
- [ ] Viện cả 2: mỗi hồ sơ 2 dòng kết quả (4750 + VLG); lỗi VLG không làm hỏng kết quả 4750 và ngược lại.
- [ ] ConfigSync cũ (JSON không có isSyncKcbVlg) → mặc định bỏ tick, không lỗi.
- [ ] Khóa 48 giờ tắt: cửa sổ Thư mục lưu 4 dòng như cũ, lưới không có cột XML 48h, không có tiến trình.
- [ ] Khóa bật + khai thư mục: hồ sơ nội trú có số vào viện vào điều trị quá 48 giờ được xuất 1 file, cột XML 48h "Đã xuất"; lượt sau không xuất lại.
- [ ] Thư mục mạng mất kết nối: hồ sơ ghi "Lỗi" kèm lý do, kết nối lại thì lượt sau xuất được.
- [ ] Đang Xuất Xml / Gửi tự động trong lúc tiến trình 48 giờ chạy: kết quả hai luồng không lẫn dữ liệu.
