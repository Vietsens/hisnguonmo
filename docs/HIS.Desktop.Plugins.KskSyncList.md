# Danh Sách Đồng Bộ Hồ Sơ KSK — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.KskSyncList |
| Loại | UC (UserControl) |
| Mục đích | Danh sách hồ sơ Khám sức khỏe đã kết luận, chờ đồng bộ; dựng bản tin KHAMSUCKHOE theo QĐ 2062/QĐ-BYT, ký số CKS, đẩy đồng thời lên tối đa 5 cổng liên thông (BYT / HSSK / HOC→TTYTQG / HCC / KDLYT Vĩnh Long) và lưu trạng thái đồng bộ |
| Người tạo | (theo PTTK_44350) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Load danh sách hồ sơ KSK đã kết luận (`api/HisKskSync/GetView`), lọc theo loại KSK / thời gian kết luận / trạng thái đồng bộ / mã BN / mã điều trị.
2. User chọn cổng đích qua popup Cài đặt (checkbox từng cổng — chỉ hiện khi key HIS_CONFIG tương ứng có giá trị; lưu lựa chọn qua ControlState).
3. Chọn hồ sơ → Đồng bộ: `KskSyncProcessor.PushList` nạp dữ liệu batch (`api/HisKskSync/GetKskData` + các API phụ song song), build bản tin KHAMSUCKHOE đủ 12 khối (XML1..XML12), ký số `CKS_NGUOI_KET_LUAN` (HSM người kết luận) + `CKS_BENH_VIEN`, đẩy TỪNG hồ sơ lên các cổng đã chọn, lưu kết quả batch (`api/HisKskSync/SaveSyncResult`).
4. Dialog kết quả lô (frmKskSyncResult) + refresh lưới. Ngoài ra có Xem trước bản tin (frmKskSyncPreview) và Xuất XML ra thư mục.

### 5 cổng liên thông + key cấu hình (HIS_CONFIG, BRANCH_ID = NULL)

| Cổng | Key | Giao thức |
|------|-----|-----------|
| KSK BYT | `MOS.HIS_KSK_SYNC.CONNECTION_INFO` | Trục BYT (DLL His.Ksk.QD2062 `PushListMulti`) — login `/api/auth/login`, push `/api/platform/data-sync/push`, envelope `{header, data=base64, signature}` |
| HSSK | `MOS.HIS_KSK_SYNC.HSSK_HN_2062_CONNECTION_INFO` | Trục BYT (qua DLL, chung base64 với BYT) |
| HOC → TTYTQG | `MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO` | OAuth2 (qua DLL, chung base64 với BYT) |
| HCC | `MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO` | Trục BYT, payload riêng json/base64 (`KskHccPusher`) |
| KDLYT Vĩnh Long | `MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO` | **Cổng tiếp nhận Vĩnh Long V1.3**: token `POST /api/xac-thuc/token` → `data.access_token`; push `POST /api/kham-suc-khoe/qd-2062/tiep-nhan` gửi XML KHAMSUCKHOE TRỰC TIẾP (`Content-Type: application/xml`, KHÔNG base64, KHÔNG envelope); response `{success, code, data:{tracking_id, status}}` (`KskVlgPusher`) |

Format value VLG: `MaDonVi|Username|Password|TokenUrl|PushUrl` (tối thiểu 3 trường; 2 URL bỏ trống → cổng chính thức `https://congtiepnhan.kdlyt.vinhlong.vn`). Xem `PTTK/KskSyncList_VLG_INSERT_CONFIG.sql`.

### Điều kiện nghiệp vụ
- Cổng chỉ đẩy được khi ĐÃ TÍCH trong popup VÀ key config có giá trị. Viện không cấu hình key → checkbox không hiện, không ảnh hưởng.
- Ký số bật → bắt buộc đủ người kết luận (HSM); USB token chỉ ký được `CKS_BENH_VIEN`.
- Trạng thái chung 1 dòng/hồ sơ: thành công = TẤT CẢ cổng đã đẩy đều thành công.
- Riêng VLG: HTTP 200 `ACCEPTED`/`ACCEPTED_DUPLICATE` = cổng ĐÃ TIẾP NHẬN (status `QUEUED`, xử lý bất đồng bộ); `tracking_id` lưu vào TRANSACTION_CODE (tiền tố `VLG:` khi >1 cổng), status vào REGISTRATION_NO. Kết quả xử lý thật tra cứu bằng `GET /api/kham-suc-khoe/qd-2062/ho-so/trang-thai?ma_lk`.
- VLG giới hạn body 10 MiB — pusher check trước khi POST; MACSKCB (= MaDonVi config) phải khớp `ma_don_vi` của token (lệch → cảnh báo log + cổng trả 403 MACSKCB_MISMATCH).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_KSK_SYNC | View | Lưới danh sách hồ sơ + trạng thái đồng bộ |
| HIS_KSK_SYNC | Table | Lưu kết quả đồng bộ (upsert theo KSK_TYPE_ID + KSK_RECORD_ID) |
| HIS_KSK_GENERAL / UNDER_SIX / UNDER_EIGHTEEN / OVER_EIGHTEEN | Table | Dữ liệu KSK theo mẫu phiếu |
| HIS_DHST, HIS_TREATMENT, HIS_PATIENT, HIS_SERVICE_REQ | Table | Hành chính + sinh hiệu (XML1/2/3/10) |
| V_HIS_SERE_SERV_2 / TEIN / SUIN, HIS_SERE_SERV_EXT | View/Table | CLS (XML11) |
| EMR_SIGNER | Table (EMR) | Ảnh chữ ký CKDT_ + chứng thư HSM người kết luận |
| HIS_CONFIG, HIS_BRANCH, HIS_HEALTH_EXAM_RANK | Table | Cấu hình cổng, MA_CSKCB, phân loại SK |

## 4. UI Layout

```
+--------------------------------------------------------------------------+
| [Loại KSK] [KL từ ngày] [đến ngày] [Trạng thái ĐB] [Từ khóa] [Tìm] [Mới] |
+--------------------------------------------------------------------------+
| Grid V_HIS_KSK_SYNC: STT|Mã ĐT|Mã BN|Họ tên|NS|Loại KSK|Kết luận|TG KL   |
|                      |Trạng thái ĐB|TG ĐB|Đẩy(↑)|Xem(👁)                  |
+--------------------------------------------------------------------------+
| [Ký số ⚙] [Cài đặt cổng ▾(5 checkbox)] [Xuất XML 📁] [Đồng bộ lên cổng]  |
+--------------------------------------------------------------------------+
```

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Danh sách hồ sơ | api/HisKskSync/GetView | MosConsumer |
| Dữ liệu KSK batch | api/HisKskSync/GetKskData | MosConsumer |
| Lưu kết quả | api/HisKskSync/SaveSyncResult | MosConsumer |
| Ký HSM | api/EmrSign/SignXmlBhyt | EmrConsumer |
| Chữ ký ảnh/chứng thư | api/EmrSigner/Get | EmrConsumer |
| Cổng ngoài (VLG) | POST {TokenUrl}, POST {PushUrl} theo config | HttpWebRequest trực tiếp (TLS 1.2) |

## 6. Dependencies

| Thành phần | Mục đích |
|------------|----------|
| His.Ksk.QD2062.dll (chỉ binary) | Mapper/builder bản tin QĐ 2062 + đẩy trục BYT (PushListMulti) — bản ≥ 06/08/2026 (có HocConfig/PushListMulti) |
| HIS.UC.SettingSignInfo | Form cấu hình ký số |
| BouncyCastle | Chuẩn hóa PEM PKCS#8 (KskPemUtil, cổng HCC) |
| Newtonsoft.Json | Parse response cổng VLG + ControlState |

## 7. Print

Không có chức năng in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 08/08/2026 | nampp + Claude | Thêm cổng thứ 5 **KDLYT Vĩnh Long** (Cổng tiếp nhận, QĐ 2062): file mới `KskVlgPusher.cs` (config parser + token `/api/xac-thuc/token` + push XML trực tiếp `/api/kham-suc-khoe/qd-2062/tiep-nhan` + parse response mới + check 10MiB + TLS1.2); key config mới `MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO`; `SyncVlg` trong KskSyncTargetADO; checkbox thứ 5 + availability trong UCKskSyncList; ctor 5 cổng + nhánh đẩy + gộp kết quả 3 nguồn trong KskSyncProcessor. Viện không cấu hình key → hành vi giữ nguyên. |
| 12/08/2026 | nampp + Claude | 3 tính năng theo kết quả test production (21 hồ sơ INVALID): (1) **Chặn trước khi đẩy VLG** — `ValidateVlgInput`: MA_LOAI_KCB > 2 ký tự (mã "100" của Khám + đối tượng KSK bị cổng chối — hướng dẫn tiếp đón chọn loại điều trị 15/16), thiếu SO_CCCD, thiếu LY_DO_VV → hồ sơ Thất bại kèm hướng dẫn, KHÔNG gửi; (2) **Nút "Cập nhật KQ cổng VLg"** (thay EmptySpaceItem hàng filter, chỉ hiện khi có config VLG) — `KskVlgPusher.GetStatus` + `UpdateVlgStatuses`: tra `GET /ho-so/trang-thai?ma_lk=<mã điều trị>`, VALID → giữ Đã đồng bộ + ghi chú, INVALID → Thất bại + lỗi cổng vào SYNC_FAILD_REASON (lưu DB); đang xử lý/chưa có → chỉ hiển thị; (3) `KskHccPusher.BuildFailMessage` (code thêm từ máy dev) đọc `data.errors` qua reflection để build được với DLL cũ. |
| 09/08/2026 | nampp + Claude | Fix theo review đối kháng (5 findings): (1) fail-fast cả lô khi login VLG lỗi không tự hết (0/401/403/429) — tránh N lần POST token sai làm cổng khóa tài khoản / lô treo N×120s; (2) tích Ký số mà ký thất bại → KHÔNG đẩy bản tin chưa ký lên VLG, đánh dấu thất bại rõ lý do (nhất quán ExportXmlFiles); (3) ACCEPTED_WITH_WARNING/warnings[] hiện lên dialog kết quả (KskVlgPushResult.Warning); (4) ControlState bản cũ (JSON thiếu SyncVlg) → auto-tick cổng VLG theo config một lần khi viện vừa khai key; (5) dialog kết quả ghi rõ "VLG: đã tiếp nhận (QUEUED — cổng xử lý sau, tra cứu bằng mã giao dịch)" (SuccessNote trong KskSyncResultADO). |

## 9. Test Cases

### Viện KHÁC (không cấu hình key VLG) — hồi quy
- [ ] Popup Cài đặt chỉ hiện các cổng cũ, không có checkbox Vĩnh Long
- [ ] Đồng bộ BYT/HSSK/HOC/HCC hoạt động y hệt trước (TRANSACTION_CODE/REGISTRATION_NO không đổi format)

### Viện Vĩnh Long
- [ ] Cấu hình key (SQL mẫu) → checkbox "Liên thông KDLYT Vĩnh Long" xuất hiện, auto-tick lần đầu
- [ ] Đẩy 1 hồ sơ → HTTP 200 ACCEPTED → trạng thái "Đã đồng bộ", TRANSACTION_CODE = tracking_id
- [ ] Đẩy lại hồ sơ cũ → ACCEPTED_DUPLICATE vẫn tính thành công
- [ ] Sai mật khẩu → thất bại "VLG: đăng nhập cổng thất bại — HTTP 401 INVALID_CREDENTIALS..."
- [ ] MaDonVi config ≠ ma_don_vi token → log Warn + cổng trả 403 MACSKCB_MISMATCH ghi vào lý do thất bại
- [ ] Ký số bật → XML gửi đi có CKS_NGUOI_KET_LUAN/CKS_BENH_VIEN trong CHUKYDONVI
- [ ] Đẩy đồng thời VLG + BYT → giá trị ghép "BYT:xxx;VLG:yyy"
- [ ] Hồ sơ > 10 MiB → chặn trước khi POST, lý do rõ ràng
