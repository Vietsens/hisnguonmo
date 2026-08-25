# Đối Tượng Điều Trị (CallPatientTypeAlter) — Tài Liệu Module

> Tài liệu được khởi tạo phục vụ thay đổi TT 06/2026 (mục 2.4). Một số phần chỉ ghi nhận phạm vi liên quan đến thay đổi, chưa audit toàn bộ plugin.

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.CallPatientTypeAlter |
| Loại | Form (đổi đối tượng điều trị/BHYT của hồ sơ) |
| Mục đích | Thay đổi loại đối tượng bệnh nhân (BHYT, thu phí, QN...) và cập nhật thông tin BHYT, thông tin giới thiệu chuyển tuyến |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ (phần liên quan TT 06/2026)

Plugin dùng chung **UC hiển thị thông tin BHYT** `His.UC.UCHein.MainHisHeinBhyt` (template `TEMPLATE__BHYT1`), khởi tạo với cờ `IsInitFromCallPatientTypeAlter = true`. UC chứa ô **chẩn đoán giới thiệu chuyển tuyến từ tuyến dưới** (`cboChanDoanTD` / `txtMaChanDoanTD`).

**Quy tắc ICD theo TT 06/2026 — trường hợp mở từ đối tượng điều trị:**
- **Cảnh báo bệnh chính (IS_NOT_RECOMMEND_MAIN = 1)**: CHỈ cảnh báo khi SỬA chẩn đoán chính (không phải lúc hiển thị thông tin). Nếu chẩn đoán đánh dấu "Không khuyến khích dùng là bệnh chính" → hiện cảnh báo *"Bệnh XXX không khuyến khích dùng làm bệnh chính. Bạn có chắc chắn sử dụng không?"*. Chọn Có → giữ nguyên; chọn Không → xóa thông tin và chọn lại.
- **Nguyên nhân tử vong (IS_DEATH_CAUSE_ONLY = 1)**: Không hiển thị các chẩn đoán đánh dấu nguyên nhân tử vong trong danh sách chọn.
- Không liên quan chẩn đoán YHCT (IS_TRADITIONAL).

Cách truyền tham số khi khởi tạo UC ([frmPatientTypeAlter_PatientTypeAlter.cs](../HIS/Plugins/HIS.Desktop.Plugins.CallPatientTypeAlter/frmPatientTypeAlter_PatientTypeAlter.cs)):
```csharp
dataHein.IsInitFromCallPatientTypeAlter = true;
dataHein.IsHideIcdDeathCauseOnly = true;              // ẩn chẩn đoán nguyên nhân tử vong
dataHein.IsWarningIcdNotRecommendMainWhenEdit = true; // cảnh báo khi sửa "không khuyến khích dùng là bệnh chính"
```

## 3. EFMODEL Sử Dụng (liên quan thay đổi)

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_ICD / V_HIS_ICD | Table/View | Danh mục ICD. Bổ sung `IS_NOT_RECOMMEND_MAIN`, `IS_DEATH_CAUSE_ONLY` (short?) |
| HIS_TREATMENT | Table | Lưu `TRANSFER_IN_ICD_CODE`, `TRANSFER_IN_ICD_NAME` (chẩn đoán chuyển tuyến) |
| HIS_PATIENT_TYPE_ALTER | Table | `CO_PAID_ACCUMULATE_AMOUNT` (long?), `PAID_6_MONTH` (C/K), `FREE_CO_PAID_TIME` (long? `yyyyMMdd`) — cập nhật từ cổng BHYT |
| HIS_BHYT_PARAM | Table | `BASE_SALARY` — nguồn tính ngưỡng 06 tháng lương cơ sở |

## 4. Tra Cứu Tiền Cùng Chi Trả / Miễn Cùng Chi Trả (MCCT)

Gọi dịch vụ `api/TraCuuCCT/TraCuuTienMCCT` trên cổng BHYT để cập nhật 3 trường cùng chi trả.
Thiết kế chi tiết: `docs/B_KyThuat_TraCuuTienMCCT_CungChiTraLuyKe.md`.

**Điểm gọi**

| Luồng | Vị trí |
|-------|--------|
| Tự động | `frmPatientTypeAlter_CheckGOV.cs` — cuối `CheckThongTuyen()`, chỉ chạy khi `BHXHLoginCFG.IsAutoCheckMcct` |
| Thủ công | Nút Search trên `txtCoPaidAccumulate` → delegate `DelegateCheckTienMCCT` → `CheckTienMCCTManual()` |

**Công thức** (`R` = `DataCCT[]`, `LIMIT` = `BASE_SALARY × 6`)

| Trường | Công thức |
|--------|-----------|
| `CO_PAID_ACCUMULATE_AMOUNT` | `làm tròn( Max(R[i].tBNCCTLuyKe) )` — **KHÔNG dùng tổng**, `tBNCCTLuyKe` đã là số cộng dồn |
| `PAID_6_MONTH` | `(lũy kế > LIMIT) ? 'C' : 'K'` — dùng `>`, không phải `>=` |
| `FREE_CO_PAID_TIME` | `ngayRa` của đợt **đầu tiên** có `tBNCCTLuyKe > LIMIT` (sắp tăng dần theo ngày) → `yyyyMMdd` |

**Cấu hình**

| Key | Ý nghĩa |
|-----|---------|
| `HIS.CHECK_HEIN_CARD.BHXH__AUTO_CHECK_MCCT` | `1` = tự động sau check thẻ · `0` = chỉ thủ công |

Đường dẫn `api/TraCuuCCT/TraCuuTienMCCT` **cố định trong code** (hằng số `API_MCCT` của `ApiInsuranceExpertise`) — không có cấu hình.

Tài khoản / địa chỉ cổng tái sử dụng `HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS` và `HIS.CHECK_HEIN_CARD.BHXH__ADDRESS` — dùng chung token với luồng check thẻ nên cùng IP, không vướng ràng buộc IP của cổng.

**Lưu ý**

- Căn cứ `MaKetQua`, **không** dựa vào mã HTTP — mã `204` vẫn trả HTTP 200 kèm `DataCCT` rỗng, khi đó **giữ nguyên** giá trị trên form (không gán 0).
- Chỉ hỏi ghi đè khi có ít nhất 1 trong 3 trường lệch so với form.
- Thứ tự điền bắt buộc: lũy kế → TDMC CT → checkbox 6 tháng (nếu ngược, `ValidateCoPaidAccumulate` chặn lưu).
- Đoạn điền bọc trong cờ `isFillingHeinDataFromDb` + `IsAutoCheck`, reset trong `finally` — tránh `txtDTMCChiTra_TextChanged` tự tick lại checkbox và bung hộp thoại giấy chứng nhận.

## 6. Dependencies

| UC dùng chung | Mục đích |
|---------------|----------|
| His.UC.UCHein (MainHisHeinBhyt) | Hiển thị thông tin BHYT + chẩn đoán giới thiệu chuyển tuyến. Logic lọc/cảnh báo ICD nằm trong UC này. `SetCoPaidAccumulateFromGov()` nhận kết quả tra cứu MCCT và tính 3 trường cùng chi trả |
| HIS.Desktop.Plugins.Library.CheckHeinGOV | `HeinGOVManager.CheckTienMCCT()` — gọi cổng BHYT, trả dữ liệu thô |
| HIS.Desktop.Plugins.Library.RegisterConfig | `BHXHLoginCFG` — tài khoản, địa chỉ cổng, `IsAutoCheckMcct` |
| His.Bhyt.InsuranceExpertise (repo `common`) | `ApiInsuranceExpertise.TraCuuTienMCCT()` — HTTP header + body JSON |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 16/06/2026 | sinhnt | TT 06/2026 (mục 2.4): truyền `IsHideIcdDeathCauseOnly=true`, `IsWarningIcdNotRecommendMainWhenEdit=true` cho UC BHYT — ẩn chẩn đoán nguyên nhân tử vong, cảnh báo khi sửa "không khuyến khích dùng là bệnh chính" |
| 17/06/2026 | sinhnt | Fix bug: sửa mã đối tượng đúng tuyến (RIGHT_ROUTE_TYPE_CODE, vd 3.1→3.6) lưu thành công nhưng mở lại vẫn giá trị cũ. Nguyên nhân: nhánh ActionEdit sau Update không đồng bộ kết quả về object cache `currentTreatmentLogSDO`. Fix: map đầy đủ `resultPatientTypeAlter.PatientTypeAlter` về cache (giống ActionAdd) |
| 29/07/2026 | tuanln | PT-44730: khi chuyển đối tượng thanh toán của hồ sơ, chỉ định thuộc dịch vụ đã khai trong bảng cấu hình `HIS_SERVICE_DEFAULT_PATY` (chờ backend) mà tài khoản không đủ quyền sửa ĐTTT thì **giữ nguyên ĐTTT cũ** của chỉ định đó, các chỉ định còn lại vẫn chuyển bình thường. Thêm partial `frmPatientTypeAlter___ServiceDefaultPaty.cs` (worker nạp cấu hình 1 lần/form + `IsAllowEditPatientTypeByServiceConfig`, người chỉ định lấy theo `TDL_REQUEST_LOGINNAME` của chỉ định). Trong `SwapPatientTypeAlter`, chèn bước hoàn lại `oldPatientTypeId` ngay trước khối xử lý `SERVICE_CONDITION_ID` / `DO_NOT_USE_BHYT`. Quyền theo key `HIS.Desktop.Plugins.Assign.ServiceDefaultPatyEditOption` (`1` = quản trị · `2` = quản trị hoặc người chỉ định · khác = không siết) |

| 24/08/2026 | khainq | Cập nhật tiền cùng chi trả / miễn cùng chi trả qua cổng BHYT (`api/TraCuuCCT/TraCuuTienMCCT`). Tự động gọi sau khi check thẻ thành công (theo cấu hình `BHXH__AUTO_CHECK_MCCT`), kèm nút tra cứu thủ công trên ô lũy kế. Suy ra 3 trường `CO_PAID_ACCUMULATE_AMOUNT` / `PAID_6_MONTH` / `FREE_CO_PAID_TIME` — xem mục 4. Thêm 4 LDO + `TraCuuTienMCCT()` (`His.Bhyt.InsuranceExpertise`), `ResultMCCTADO` + `CheckTienMCCT()` (`CheckHeinGOV`), `Core/SetCoPaidAccumulateFromGov/` + `Template__HeinBHYT1__CoPaidMCCT.cs` (`His.UC.UCHein`). Thiết kế: `docs/B_KyThuat_TraCuuTienMCCT_CungChiTraLuyKe.md` |

## 9. Test Cases

**Tra cứu MCCT**

- [ ] `MaKetQua=200`, lũy kế > LIMIT, có đợt vượt ngưỡng → điền đủ 3 trường, TDMC CT = ngày ra viện đợt vượt **lần đầu**
- [ ] `MaKetQua=200`, lũy kế ≤ LIMIT → điền lũy kế, bỏ tick 6 tháng, TDMC CT trống, không cảnh báo
- [ ] `MaKetQua=200`, mọi `ngayRa` rỗng nhưng lũy kế vượt → điền lũy kế + tick 6 tháng, TDMC CT trống **kèm cảnh báo**
- [ ] `MaKetQua=204` → **giữ nguyên** 3 control, không xóa giá trị đang có
- [ ] Mã thẻ 11 ký tự → chặn tại client, không gọi cổng
- [ ] Token hết hạn → tự lấy token mới rồi gọi lại 1 lần
- [ ] Cổng timeout / không phản hồi → không treo form, giữ nguyên 3 control
- [ ] Số cổng khác số trên form → hộp thoại Có/Không; chọn Không thì giữ nguyên toàn bộ
- [ ] Số cổng trùng số trên form → không bung hộp thoại
- [ ] `HIS_BHYT_PARAM` không có bản ghi hiệu lực → chỉ điền lũy kế, không đụng checkbox và TDMC CT
- [ ] Sau khi điền tự động, bấm Lưu → không bị `ValidateCoPaidAccumulate` chặn
- [ ] `BHXH__AUTO_CHECK_MCCT=0` → không gọi tự động, nút tra cứu thủ công vẫn chạy
- [ ] Lưu xong, mở lại form Sửa → 3 trường hiển thị đúng giá trị đã lưu
- [ ] In 3 mẫu Mps000508 / Mps000510 / Mps000512 → trường lũy kế hiển thị đúng

**TT 06/2026**

- [ ] Danh sách chẩn đoán giới thiệu KHÔNG hiển thị ICD có IS_DEATH_CAUSE_ONLY=1
- [ ] Sửa chẩn đoán có IS_NOT_RECOMMEND_MAIN=1 → hiện cảnh báo; chọn Không → xóa & chọn lại; chọn Có → giữ nguyên
- [ ] Hiển thị (load) hồ sơ có sẵn → KHÔNG hiện cảnh báo
- [ ] Chẩn đoán YHCT vẫn hiển thị bình thường
