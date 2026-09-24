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
| KDLYT Vĩnh Long | `MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO` | **Cổng tiếp nhận Vĩnh Long API V1.5** (từ 24/09/2026; API V1.3 `/tiep-nhan` Kho đã khóa, trả 410 `KSK_LEGACY_API_DISABLED` từ 18/09): token `POST /api/xac-thuc/token`; push `POST /api/platform/data-sync/push` với envelope trục BYT `{header{version 1.0.6, sender_id <GTIN 13 số>, receiver_id TTYQG, txn_type sync_checkup, msg_id, msg_type 101, data_type xml/base64, send_datetime}, data=base64 XML, signature ""}` + header `service-type: 100`; Kho chuyển tiếp sang Cổng Bộ Y tế. Phản hồi envelope Bộ (`header.res_code`), mã theo dõi ở header `X-HOC-Tracking-Id` (`KskVlgPusher`) |

Format value VLG: `MaDonVi|Username|Password|TokenUrl|PushUrl|SenderGtin` (tối thiểu 3 trường). TokenUrl trống → cổng chính thức `https://congtiepnhan.kdlyt.vinhlong.vn`. PushUrl trống hoặc còn đường dẫn cũ `/tiep-nhan` → tự đổi thành `{host của TokenUrl}/api/platform/data-sync/push`. SenderGtin = mã định danh CSKCB 13 số (BV Nguyễn Đình Chiểu `8934285071993`); trống thì lấy SenderId 13 số của cổng BYT/HSSK/HCC, không bao giờ dùng mã 5 số. SQL cập nhật: `Downloads/VLG_V15_Release_24092026/01_CapNhat_Cau_Hinh_VLG_V15.sql`.

### Điều kiện nghiệp vụ
- Cổng chỉ đẩy được khi ĐÃ TÍCH trong popup VÀ key config có giá trị. Viện không cấu hình key → checkbox không hiện, không ảnh hưởng.
- Ký số bật → bắt buộc đủ người kết luận (HSM); USB token chỉ ký được `CKS_BENH_VIEN`.
- Trạng thái chung 1 dòng/hồ sơ: thành công = TẤT CẢ cổng đã đẩy đều thành công.
- Riêng VLG (V1.5): "Đã đồng bộ" = **Kho dữ liệu đã giữ bản tin**. Mã thành công của Bộ (`CM_SUCCESS`…) và mã chờ (`HOC_BYT_FORWARD_PAUSED`, `HOC_BYT_TIMEOUT`, `PS_DS_SAVE_FAIL`, `CM_AUTH_ACCOUNT_FAIL`, `HOC_BYT_*`, 5xx kèm `X-HOC-Tracking-Id`, mã 2xx lạ) đều tính thành công kèm ghi chú "KHÔNG đẩy lại" (đẩy lại = msg_id mới = phiên bản mới gửi Bộ). Bộ từ chối (`CM_INVALID_REQUEST`, `PS_DS_*_SIGNATURE_*`, `PS_DS_VERIFY_FAIL`) và lỗi 400/401/403/410/413/429/503 `HOC_SIGNING_UNAVAILABLE` là thất bại. `TRANSACTION_CODE` = `X-HOC-Tracking-Id` (không có thì `MSG:<msg_id>`), `REGISTRATION_NO` = `res_code`; tiền tố `VLG:` khi >1 cổng.
- VLG chống gửi trùng: mất phản hồi sau khi đã gửi, hoặc 5xx không kèm mã/tracking của Kho → Thất bại với `REGISTRATION_NO = VLG_CHUA_RO`, `TRANSACTION_CODE = MSG:<msg_id>`. Lần đồng bộ sau đối soát `GET /api/kham-suc-khoe/doi-soat-byt/trang-thai?sender_id&msg_id` (không được thì tra theo mã liên kết): Kho đã có và chờ/Bộ nhận → không gửi lại; Kho chưa có hoặc bản đó bị từ chối → gửi bản hiện tại; không đối soát được → giữ dấu, chưa gửi. Mất phản hồi 2 hồ sơ liên tiếp → dừng lô. Body tối đa 10 MiB (sau base64), timeout 180 giây, 401/503 gửi lại đúng mảng byte cũ.

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
| Cổng ngoài (VLG) — đẩy | POST {TokenUrl}, POST {PushUrl} (`/api/platform/data-sync/push`) | HttpWebRequest trực tiếp (TLS 1.2) |
| Cổng ngoài (VLG) — tra hồ sơ | GET {host}/api/kham-suc-khoe/qd-2062/ho-so/trang-thai?ma_lk | HttpWebRequest (nút "Cập nhật KQ cổng VLg") |
| Cổng ngoài (VLG) — đối soát bản tin | GET {host}/api/kham-suc-khoe/doi-soat-byt/trang-thai?sender_id&msg_id | HttpWebRequest (chống gửi trùng khi mất phản hồi) |

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
| 24/09/2026 | nampp + Claude | **Chuyển cổng VLG sang API V1.5** (API V1.3 bị Kho khóa, 410 từ 18/09 — mọi hồ sơ đồng bộ từ 18/09 thất bại): `KskVlgPusher` đẩy `/api/platform/data-sync/push` bằng envelope trục BYT (sender_id = GTIN 13 số, msg_id = sender_id + yyMMdd + UUID), serialize 1 lần, đọc `X-HOC-Tracking-Id`, phân loại `res_code` 2 lớp Kho/Bộ; khóa cấu hình thêm trường 6 SenderGtin, PushUrl cũ tự nâng; XML dựng y trục BYT (MACSKCB 13 số, ngày 12 số — bỏ `NormalizeVlgDates`); "Cập nhật KQ cổng" xét Kho + Bộ (`byt_status`/`byt_res_code`); sửa câu `if` treo trong `BuildResultAdo`. **Sau review đối kháng (6 lỗi xác nhận):** chống gửi trùng dùng API đối soát theo sender_id + msg_id và xét đúng lần gửi đó; kết quả VLG biết trước (thiếu GTIN / lần trước đã vào Kho) chỉ bỏ lệnh đẩy VLG, các cổng khác vẫn đẩy; "Cập nhật KQ cổng" không nâng hồ sơ mất phản hồi khi bản tin chưa vào Kho và chỉ thay đoạn `VLG:` trong mã ghép; `BuildSyncEntity` cắt TRANSACTION_CODE/REGISTRATION_NO vừa 100 ký tự (rút `MSG:` còn UUID); 504/5xx không kèm dấu hiệu Kho → chưa rõ; 2 lần mất phản hồi liên tiếp → dừng lô; phản hồi Brotli bỏ body; `received_at` so theo thời gian; TokenUrl lạ giữ host (không rơi về prod); `BuildFailedResult` giữ mã cũ. Kiểm trên cổng dev + máy chủ giả: 32/32 kiểm thử và 5/5 kịch bản "Cập nhật KQ cổng" đạt. |
| 21/08/2026 | nampp + Claude | Bổ sung `KskVlgLengthRules.ValidateRequired`: chặn cứng (không đẩy cổng nào) khi hồ sơ VLG THIẾU trường bắt buộc **Đối tượng (DOI_TUONG)** / **Nguồn chi trả (NGUON_CHI_TRA)** — QĐ 2062 đánh "x" nhưng KSK lái xe không nhập vẫn đẩy được; nay bắt buộc, kiểm trên XML đã dựng, thẻ vắng/rỗng đều tính thiếu. |
| 21/08/2026 | nampp + Claude | Thêm KskVlgLengthRules.cs: (21/08 bản 2 — chặn cứng) Khi hồ sơ sai dữ liệu bắt buộc hoặc vượt độ dài QĐ 2062 mà viện có chọn cổng VLG -> KHÔNG đồng bộ lên BẤT KỲ cổng nào (pre-gate trước cả BYT/HSSK/HOC/HCC), đánh Thất bại + lý do, giữ mã đối soát cũ. chặn đẩy VLG khi thẻ vượt Kích thước tối đa theo Phụ lục QĐ 2062 (180 thẻ, gộp 3 mẫu lấy max; thẻ "n" bỏ qua; giải mã NOIDUNGFILE base64). Kiểm trên XML đã dựng sau ký; báo tối đa 8 thẻ "TÊN dài X (tối đa Y)". Chỉ nhánh VLG. |
| 08/08/2026 | nampp + Claude | Thêm cổng thứ 5 **KDLYT Vĩnh Long** (Cổng tiếp nhận, QĐ 2062): file mới `KskVlgPusher.cs` (config parser + token `/api/xac-thuc/token` + push XML trực tiếp `/api/kham-suc-khoe/qd-2062/tiep-nhan` + parse response mới + check 10MiB + TLS1.2); key config mới `MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO`; `SyncVlg` trong KskSyncTargetADO; checkbox thứ 5 + availability trong UCKskSyncList; ctor 5 cổng + nhánh đẩy + gộp kết quả 3 nguồn trong KskSyncProcessor. Viện không cấu hình key → hành vi giữ nguyên. |
| 12/08/2026 | nampp + Claude | 3 tính năng theo kết quả test production (21 hồ sơ INVALID): (1) **Chặn trước khi đẩy VLG** — `ValidateVlgInput`: MA_LOAI_KCB > 2 ký tự (mã "100" của Khám + đối tượng KSK bị cổng chối — hướng dẫn tiếp đón chọn loại điều trị 15/16), thiếu SO_CCCD, thiếu LY_DO_VV → hồ sơ Thất bại kèm hướng dẫn, KHÔNG gửi; (2) **Nút "Cập nhật KQ cổng VLg"** (thay EmptySpaceItem hàng filter, chỉ hiện khi có config VLG) — `KskVlgPusher.GetStatus` + `UpdateVlgStatuses`: tra `GET /ho-so/trang-thai?ma_lk=<mã điều trị>`, VALID → giữ Đã đồng bộ + ghi chú, INVALID → Thất bại + lỗi cổng vào SYNC_FAILD_REASON (lưu DB); đang xử lý/chưa có → chỉ hiển thị; (3) `KskHccPusher.BuildFailMessage` (code thêm từ máy dev) đọc `data.errors` qua reflection để build được với DLL cũ. |
| 09/08/2026 | nampp + Claude | Fix theo review đối kháng (5 findings): (1) fail-fast cả lô khi login VLG lỗi không tự hết (0/401/403/429) — tránh N lần POST token sai làm cổng khóa tài khoản / lô treo N×120s; (2) tích Ký số mà ký thất bại → KHÔNG đẩy bản tin chưa ký lên VLG, đánh dấu thất bại rõ lý do (nhất quán ExportXmlFiles); (3) ACCEPTED_WITH_WARNING/warnings[] hiện lên dialog kết quả (KskVlgPushResult.Warning); (4) ControlState bản cũ (JSON thiếu SyncVlg) → auto-tick cổng VLG theo config một lần khi viện vừa khai key; (5) dialog kết quả ghi rõ "VLG: đã tiếp nhận (QUEUED — cổng xử lý sau, tra cứu bằng mã giao dịch)" (SuccessNote trong KskSyncResultADO). |

## 9. Test Cases

### Viện KHÁC (không cấu hình key VLG) — hồi quy
- [ ] Popup Cài đặt chỉ hiện các cổng cũ, không có checkbox Vĩnh Long
- [ ] Đồng bộ BYT/HSSK/HOC/HCC hoạt động y hệt trước (TRANSACTION_CODE/REGISTRATION_NO không đổi format)

### Viện Vĩnh Long
- [ ] Chạy SQL V1.5 (khóa 6 trường) → restart → đẩy 1 hồ sơ → "Đã đồng bộ", ghi chú "Cổng Bộ Y tế đã tiếp nhận" hoặc "Kho dữ liệu đã tiếp nhận … KHÔNG đẩy lại"; TRANSACTION_CODE = `KSKBYT-…`
- [ ] Khóa cũ còn PushUrl `/tiep-nhan` → vẫn đẩy được (tự nâng lên data-sync/push)
- [ ] Thiếu mã 13 số (khóa 3 trường, không có cổng BYT/HSSK/HCC) → Thất bại "chưa khai mã định danh cơ sở KCB 13 số"
- [ ] "Cập nhật KQ cổng VLg" sau khi đẩy → Đã đồng bộ + "đang chờ Cổng Bộ Y tế (PENDING)" hoặc "Cổng Bộ Y tế đã tiếp nhận"
- [ ] Hồ sơ INVALID trên Kho → Thất bại + danh sách lỗi; Bộ từ chối → Thất bại + mã Bộ
- [ ] Rút mạng giữa lúc đẩy (sau khi gửi) → Thất bại `VLG_CHUA_RO`; cắm lại, đồng bộ lại → không gửi trùng nếu Kho đã có
- [ ] Hồ sơ lỗi 410 từ 18/09 → đồng bộ lại qua API mới thành công
- [ ] Sai mật khẩu → "đăng nhập cổng thất bại — HTTP 401 …", cả lô dừng ngay
- [ ] Thiếu Đối tượng / Nguồn chi trả, vượt độ dài QĐ 2062 → chặn mọi cổng (như 21/08)
- [ ] Tích thêm BYT/HOC cùng VLG → log cảnh báo Bộ có thể nhận 2 bản tin; mã ghép "BYT:…;VLG:…" không vượt 100 ký tự
