# Tài liệu thiết kế — Để trống khu vực khi kiểm tra thẻ BHYT của đợt điều trị cũ

**Màn hình Tiếp đón — kiểm tra thẻ BHYT trên cổng BHXH: không lấy khu vực (K1/K2/K3) từ đợt khám chữa bệnh trước khi thẻ không có mã khu vực**

| Thông tin | Giá trị |
|---|---|
| Module frontend | `HIS.Desktop.Plugins.RegisterV2` — Tiếp đón |
| UC liên quan | `HIS.UC.UCHeniInfo` — vùng thông tin thẻ BHYT (bổ sung 1 method public) |
| Thư viện liên quan | `HIS.Desktop.Plugins.Library.CheckHeinGOV` — KHÔNG sửa |
| Backend | Không sửa code. Chỉ INSERT cấu hình `HIS_CONFIG` |
| API mới | Không |
| EFMODEL / SDO | Không thay đổi |
| Cấu hình mới | `HIS.Desktop.Plugins.RegisterV2.IsClearLiveAreaWhenCheckHeinCardOldTreatment` (mặc định tắt) |
| SQL | `SQL_RegisterV2_DeTrongKhuVucTheBHYT.sql` |
| Ngày lập | 27/10/2026 |
| Cập nhật | 30/10/2026: cổng lỗi / thẻ không hợp lệ → để trống; thêm luồng đọc thẻ KCB; log mọi nhánh |

---

## 1. Nghiệp vụ hiện tại

### 1.1. Mô tả

- Tại màn hình Tiếp đón, khi nhập số thẻ và kiểm tra thẻ BHYT, phần mềm tra cứu trên cổng BHXH và tự điền thông tin vào hồ sơ tiếp đón.
- Với bệnh nhân đã từng khám, phần mềm lấy lại thông tin của đợt khám chữa bệnh trước (hành chính, địa chỉ, liên hệ, thẻ BHYT…), **bao gồm cả khu vực**.
- Khi thẻ **không có mã khu vực** (cổng trả `maKV` rỗng), ô "Khu vực" vẫn giữ khu vực của đợt trước (ví dụ K1).
- Khu vực sai được lưu vào `HIS_PATIENT_TYPE_ALTER.LIVE_AREA_CODE`, đưa vào hồ sơ thanh toán và gửi lên cổng tiếp nhận BHXH → **bị xuất toán**.
- Chưa có cấu hình để bệnh viện chọn bỏ qua khu vực của đợt cũ.

### 1.2. Luồng code hiện tại

```
Tìm BN cũ
  └─ FillDataIntoUCHeinInfo / FillDataIntoUCHeinInfoByPatientTypeAlter
       └─ ô Khu vực = patientSDO.LiveAreaCode (khu vực đợt cũ)             ①
Kiểm tra thẻ (gõ số thẻ / quét QR BHYT / quét CCCD / tự chuyển đối tượng BHYT)
  └─ CheckTTFull(heinCard)            heinCard.LiveAreaCode = khu vực đợt cũ ②
       └─ HeinGOVManager.Check → rsData.HeinCardData = heinCard (đầu vào)  ③
            ├─ maKetQua = 000 : KHÔNG so sánh/ghi đè khu vực → giữ K1 cũ   ③a  ← LỖI
            └─ thẻ mới / 004  : HeinCardData.LiveAreaCode = maKV            ③b
       └─ CheckTTProcessResultData → nạp lại vùng BHYT (một số nhánh)
Lưu
  └─ UCHeinInfo.GetValue → LIVE_AREA_CODE = cboNoiSong (K1 cũ)              ④
```

| # | Thành phần | Vị trí |
|---|---|---|
| ① | Nạp khu vực đợt cũ vào ô Khu vực | `RegisterV2/RunV3/UCRegister__UCHein.cs` L215–L247; `UC/HIS.UC.UCHeniInfo/Design/UCHeinInfo__Process.cs` L120, L530 |
| ② | Dựng dữ liệu thẻ gửi cổng mang khu vực cũ | `UCRegister__CheckHeinGOV.cs` L128; `UCRegister.cs` L892; `UCRegister__AutoPatientTypeBhyt.cs` L159 |
| ③ | Kết quả cổng trả lại chính dữ liệu đầu vào | `Library.CheckHeinGOV/HeinGOVManager.cs` L239 |
| ③a | Nhánh 000 không xử lý khu vực | `HeinGOVManager.cs` L432–L452, `CheckChangeAndUpdateForChangeHeinData` L883 |
| ③b | Nhánh thẻ mới / 004 lấy `maKV` | `HeinGOVManager.cs` L377, L465 |
| ④ | Lấy khu vực khi lưu | `UCHeinInfo__Data.cs` L117 |
| — | Khu vực ảnh hưởng tuyến (K1/K2/K3 → đúng tuyến) | `UCHeinInfo__Process.cs` L362, L381; `UCHeinInfo.cs` L2139, L2171 |
| — | Cảnh báo "Bạn có muốn nhập thông tin khu vực" khi lưu (#3590) — chỉ hiện khi CÓ khu vực | `Register/ServiceRequestRegisterBehaviorBase__Check.cs` L326; `Register/Exam/ServiceRequestRegisterExamBehavior.cs` L63 |
| — | Ô Khu vực `cboNoiSong` là `internal` → plugin không gán trực tiếp được | `UCHeinInfo.Designer.cs` L1303 |

---

## 2. Nghiệp vụ sau khi sửa

### 2.1. Khi BẬT cấu hình (= 1)

| # | Tình huống | Hành vi |
|---|---|---|
| 1 | BN cũ, kiểm tra thẻ, cổng trả thẻ **không có** mã khu vực | Vẫn lấy lại toàn bộ thông tin đợt cũ **trừ khu vực** → ô "Khu vực" **để trống** |
| 2 | Cổng trả thẻ **có** mã khu vực (K1/K2/K3) | Điền đúng khu vực theo kết quả cổng (kể cả khi khác khu vực đợt cũ) |
| 3 | Khu vực trống | Vẫn lưu tiếp đón, đăng ký khám bình thường; không phát sinh thêm thao tác |
| 4 | Thông tin khác của đợt cũ (hành chính, địa chỉ, liên hệ, nơi ĐKBĐ…) | Giữ nguyên như hiện tại |
| 5 | Cổng lỗi / không kết nối (`ResultHistoryLDO = null`), hoặc cổng báo thẻ không hợp lệ (mã khác 000/003/004) | Không có căn cứ từ cổng → **để trống** khu vực (chốt 30/10/2026) |
| 5a | Không gọi cổng: `IsCheckExamHistory` tắt, thiếu dữ liệu thẻ, chưa cấu hình tài khoản cổng (`ResultDataADO = null`), thẻ thuộc whitelist không kiểm tra trên cổng (mã nội bộ `9999`) | **Giữ nguyên như hiện tại** |
| 6 | Hồ sơ đã tiếp đón trước thời điểm áp dụng | Không ảnh hưởng |

> **Nguyên tắc:** khi cấu hình bật và phần mềm đã gọi cổng → khu vực = `maKV` của cổng nếu cổng xác nhận được thẻ (rỗng thì để trống); cổng lỗi hoặc thẻ không hợp lệ → để trống. Khu vực **chỉ** lấy theo kết quả cổng, không lấy theo đợt khám trước.

### 2.2. Khi TẮT cấu hình (= 0 / rỗng / chưa khai báo)

Phần mềm hoạt động **giống hệt hiện tại** — không có dòng code mới nào được thực thi.

### 2.3. Luồng code sau khi sửa

```
CheckTTFull / tìm BN cũ (UCPatientRaw) → HeinGOVManager.Check (không đổi)
  └─ CheckTTProcessResultData(heinCard, focus, ischeckChange)     [UCRegister__CheckHeinGOV.cs]
       └─ [MỚI] ProcessLiveAreaByGovResult(ucPatientRaw1.ResultDataADO)   // NGOÀI khối ResultHistoryLDO != null
Đọc thẻ KCB (ProcessPatientCodeKeydown) → HeinGOVManager.Check
  └─ CheckTTProcessResultData(heinCard)                           [UCRegister__Load.cs]
       └─ [MỚI] ProcessLiveAreaByGovResult(this.ResultDataADO)

ProcessLiveAreaByGovResult(rs):
  IF cấu hình ≠ 1 OR đối tượng không dùng thẻ     → log, bỏ qua
  IF rs == null                                   → log, giữ nguyên   // không gọi cổng
  IF rs.ResultHistoryLDO.maKetQua == "9999"       → log, giữ nguyên   // whitelist không kiểm tra cổng
  liveArea = maKetQua ∈ {000, 003, 004} ? maKV (rỗng → null) : null   // cổng lỗi (LDO null) / thẻ không hợp lệ → null
  ucHeinInfo1.SetLiveAreaCode(liveArea)          // gán ô + tính lại tuyến
  rs.HeinCardData.LiveAreaCode = liveArea
  log Info resultCode, liveAreaCode
```

- Đặt sau các lệnh `FillDataByHeinCardData` trong `CheckTTProcessResultData` để không bị nạp đè lại khu vực cũ.
- Áp dụng cho mọi luồng kiểm tra thẻ của RegisterV2: gõ số thẻ, quét QR BHYT, quét CCCD, tìm BN cũ, tự chuyển đối tượng BHYT (overload 3 tham số) và đọc thẻ KCB (overload 1 tham số).
- Phân biệt cổng lỗi với không gọi cổng: khi gọi cổng bị exception, `HeinGOVManager.Check` vẫn trả `ResultDataADO` nhưng `ResultHistoryLDO = null`; khi không gọi cổng thì trả `null`.
- **Không sửa** thư viện `CheckHeinGOV` (dùng chung nhiều plugin).

---

## 3. Cấu hình (key)

| Thuộc tính | Giá trị |
|---|---|
| KEY | `HIS.Desktop.Plugins.RegisterV2.IsClearLiveAreaWhenCheckHeinCardOldTreatment` |
| DEFAULT_VALUE | `''` (rỗng = tắt) |
| Giá trị hợp lệ | `1` = bật; giá trị khác (`0`, rỗng, null, chưa khai báo) = tắt |
| MODULE_LINKS | `HIS.Desktop.Plugins.RegisterV2` |
| Phạm vi | Toàn viện (`HIS_CONFIG`) |

**Nội dung mô tả (DESCRIPTION):**

```
Để trống khu vực khi kiểm tra thẻ BHYT của đợt điều trị cũ (màn hình Tiếp đón).
- 1: Khi kiểm tra thẻ BHYT trên cổng BHXH, khu vực chỉ lấy theo kết quả trả về của cổng.
     Thẻ không có mã khu vực thì để trống ô "Khu vực", không lấy khu vực của đợt khám chữa bệnh trước.
     Các thông tin khác của đợt khám chữa bệnh trước vẫn được lấy lại như hiện tại.
- Khác 1 hoặc để trống: Giữ nguyên hành vi hiện tại (lấy lại khu vực của đợt khám chữa bệnh trước).

Không áp dụng khi phần mềm không tra cứu cổng (tắt kiểm tra thẻ qua cổng, đầu thẻ thuộc danh mục không kiểm tra trên cổng). Cổng lỗi / không kết nối được / báo thẻ không hợp lệ thì để trống khu vực.
```

**Đọc cấu hình** (`RegisterV2/Config/HisConfigCFG.cs`, theo mẫu `IsAutoSetPatientTypeBhytByHeinCard`):

```csharp
private const string CONFIG_KEY__IsClearLiveAreaWhenCheckHeinCardOldTreatment = "HIS.Desktop.Plugins.RegisterV2.IsClearLiveAreaWhenCheckHeinCardOldTreatment";

/// <summary>
/// Key HIS.Desktop.Plugins.RegisterV2.IsClearLiveAreaWhenCheckHeinCardOldTreatment.
/// = 1: live area is taken only from BHXH gateway result (empty maKV => clear area of previous treatment).
/// Other value or not declared (DEFAULT): keep old behavior.
/// </summary>
internal static bool IsClearLiveAreaWhenCheckHeinCardOldTreatment()
{
    return GetValue(CONFIG_KEY__IsClearLiveAreaWhenCheckHeinCardOldTreatment) == valueString__true;
}
```

| Giá trị key | Kết quả hàm | Luồng |
|---|---|---|
| `1` | true | Mới |
| `0` / rỗng / null / chưa INSERT / giá trị lạ / lỗi đọc | false | Cũ |

---

## 4. Thiết kế sửa code

| # | File | Thay đổi |
|---|---|---|
| 1 | `HIS/Plugins/HIS.Desktop.Plugins.RegisterV2/Config/HisConfigCFG.cs` | Thêm const key + hàm `IsClearLiveAreaWhenCheckHeinCardOldTreatment()` |
| 2 | `HIS/Plugins/HIS.Desktop.Plugins.RegisterV2/RunV3/UCRegister__CheckHeinGOV.cs` | Thêm `ProcessLiveAreaByGovResult()`; gọi cuối khối xử lý `ResultHistoryLDO` trong `CheckTTProcessResultData` |
| 2a | `HIS/Plugins/HIS.Desktop.Plugins.RegisterV2/RunV3/UCRegister__Load.cs` | (30/10) Gọi `ProcessLiveAreaByGovResult(this.ResultDataADO)` cuối overload `CheckTTProcessResultData(HeinCardData)` — luồng đọc thẻ KCB |
| 3 | `UC/HIS.UC.UCHeniInfo/Design/UCHeinInfo__Process.cs` | Thêm method public `SetLiveAreaCode(string liveAreaCode)`: gán `cboNoiSong.EditValue`, gọi lại `ProcessCaseWrongRoute(mã nơi ĐKBĐ đang chọn, liveAreaCode)` |
| 4 | `docs/HIS.Desktop.Plugins.RegisterV2.md` | Changelog + mục 2 (Quy trình) + mục 9 (Test) |
| 5 | `docs/SQL_RegisterV2_DeTrongKhuVucTheBHYT.sql` | INSERT `HIS_CONFIG` |

**Code hiện tại** (`UCRegister__CheckHeinGOV.cs`):

```csharp
/// <summary>
/// Khoa IsClearLiveAreaWhenCheckHeinCardOldTreatment = 1: khu vuc CHI lay theo ket qua cong BHXH,
/// khong giu khu vuc cua dot dieu tri cu.
/// - Cong xac nhan duoc the (000/003/004): khu vuc = maKV (rong => de trong).
/// - Cong loi / mat ket noi (ResultHistoryLDO = null) hoac cong bao the khong hop le: de trong khu vuc.
/// - The thuoc danh muc khong kiem tra tren cong (9999) hoac khong goi cong (resultDataADO = null): giu nguyen.
/// Khoa tat => khong lam gi (giu nguyen luong cu).
/// Tham chieu: docs/B_KyThuat_RegisterV2_DeTrongKhuVucTheBHYT.md
/// </summary>
private void ProcessLiveAreaByGovResult(ResultDataADO resultDataADO)
{
    try
    {
        bool isEnabled = Config.HisConfigCFG.IsClearLiveAreaWhenCheckHeinCardOldTreatment();
        bool isUsingHeinInfo = IsPatientTypeUsingHeinInfo();
        if (!isEnabled || this.ucHeinInfo1 == null || !isUsingHeinInfo)
        {
            Inventec.Common.Logging.LogSystem.Info("ProcessLiveAreaByGovResult: bo qua."
                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isEnabled), isEnabled)
                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isUsingHeinInfo), isUsingHeinInfo));
            return;
        }

        if (resultDataADO == null)
        {
            Inventec.Common.Logging.LogSystem.Info("ProcessLiveAreaByGovResult: khong goi cong BHXH => giu nguyen khu vuc.");
            return;
        }

        string resultCode = (resultDataADO.ResultHistoryLDO != null ? resultDataADO.ResultHistoryLDO.maKetQua : null);
        if (resultCode == GOV_RESULT_CODE__NOT_CHECK_ON_GOV)
        {
            Inventec.Common.Logging.LogSystem.Info("ProcessLiveAreaByGovResult: the khong kiem tra tren cong BHXH => giu nguyen khu vuc.");
            return;
        }

        string liveAreaCode = null;
        if (resultCode == GOV_RESULT_CODE__CARD_VALID
            || resultCode == GOV_RESULT_CODE__CARD_EXPIRED_HAS_NEW_CARD
            || resultCode == GOV_RESULT_CODE__CARD_VALID_HAS_NEW_CARD)
        {
            liveAreaCode = String.IsNullOrWhiteSpace(resultDataADO.ResultHistoryLDO.maKV)
                ? null : resultDataADO.ResultHistoryLDO.maKV.Trim();
        }

        this.ucHeinInfo1.SetLiveAreaCode(liveAreaCode);
        if (resultDataADO.HeinCardData != null)
            resultDataADO.HeinCardData.LiveAreaCode = liveAreaCode;

        Inventec.Common.Logging.LogSystem.Info("ProcessLiveAreaByGovResult: khu vuc lay theo ket qua cong BHXH (cong loi / the khong hop le => de trong)."
            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultCode), resultCode)
            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => liveAreaCode), liveAreaCode));
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

> Mã kết quả cổng (`000`, `003`, `004`) khai báo thành hằng số/enum có XML comment — không hardcode trong điều kiện.

---

## 5. Ảnh hưởng & rủi ro

| # | Mức | Nội dung | Xử lý |
|---|---|---|---|
| 1 | HIGH | Khu vực K1/K2/K3 đang tự đặt "đúng tuyến" (cấu hình `IsAllowedRouteTypeByDefault`, `IsNotRequiredRightTypeInCaseOfHavingAreaCode`). Làm trống khu vực → BN có thể thành trái tuyến / phải chọn tuyến → đổi mức hưởng | `SetLiveAreaCode` bắt buộc tính lại tuyến. Nghiệp vụ xác nhận đây là hành vi đúng |
| 2 | MEDIUM | Kiểm tra thẻ chạy bất đồng bộ: ô Khu vực hiện K1 cũ khoảng 1–3 giây rồi mới được làm trống. Lưu trước khi cổng trả kết quả vẫn lưu K1 | Cùng hạn chế với các trường thẻ khác hiện nay; ghi nhận trong test |
| 3 | LOW | Cảnh báo #3590 chỉ hiện khi có khu vực → khu vực trống không bị chặn lưu | Test xác nhận UC không validate bắt buộc ô Khu vực |
| 4 | LOW | UC `UCHeinInfo` dùng chung nhiều plugin | Chỉ THÊM method mới, chỉ RegisterV2 gọi → plugin khác không bị ảnh hưởng |
| 5 | — | Tắt cấu hình | Không thực thi code mới → giữ nguyên hành vi cũ |

---

## 6. Điểm cần chốt

| # | Câu hỏi | Đề xuất trong tài liệu |
|---|---|---|
| Q1 | Mã kết quả cổng nào được coi là "cổng đã xác nhận thẻ"? | `000`, `003`, `004` |
| Q2 | Cổng lỗi / mất kết nối thì xử lý thế nào? | **Đã chốt 30/10/2026:** để trống khu vực; thẻ không hợp lệ cũng để trống |
| Q3 | Thẻ có khu vực khác đợt cũ (cũ K1, cổng K2) có ghi đè không? | Có — ghi đè theo cổng |
| Q4 | Làm trống khu vực khiến BN đổi tuyến có chấp nhận không? | Chấp nhận, tính lại tuyến tự động |
| Q5 | Phạm vi: chỉ RegisterV2 hay cả `Register` (v1) và các màn tiếp đón khác? | Chỉ RegisterV2 |

---

## 7. Test case

| # | Cấu hình | Dữ liệu | Thao tác | Kết quả mong đợi |
|---|---|---|---|---|
| 1 | 1 | BN cũ có K1, thẻ trên cổng không có khu vực | Nhập số thẻ → Enter | Ô Khu vực trống; địa chỉ, liên hệ, nơi ĐKBĐ… vẫn lấy lại từ đợt cũ |
| 2 | 1 | Như TC1 | Lưu tiếp đón | Lưu thành công, `LIVE_AREA_CODE` rỗng; không hiện cảnh báo khu vực |
| 3 | 1 | BN cũ K1, cổng trả K2 | Kiểm tra thẻ | Ô Khu vực = K2 |
| 4 | 1 | BN mới, cổng trả K3 | Kiểm tra thẻ | Ô Khu vực = K3 |
| 5 | 1 | BN cũ K1, cổng lỗi / không kết nối (VD BN 0026003151 trên môi trường test, log `Dang nhap That Bai`) | Kiểm tra thẻ | Ô Khu vực trống; log `ProcessLiveAreaByGovResult ... resultCode: null` |
| 5a | 1 | BN cũ K1, cổng báo thẻ không tồn tại / sai thông tin | Kiểm tra thẻ | Ô Khu vực trống |
| 5b | 1 | BN cũ K1, đầu thẻ thuộc whitelist không kiểm tra trên cổng | Kiểm tra thẻ | Giữ K1 |
| 5c | 1 | BN cũ K1, thẻ không có khu vực | Đọc thẻ KCB | Ô Khu vực trống |
| 6 | 1 | BN cũ K1 trước đây đúng tuyến nhờ khu vực | Kiểm tra thẻ (không có maKV) | Tuyến được tính lại theo nơi ĐKBĐ; kiểm tra mức hưởng |
| 7 | 1 | BN cũ K1 | Quét QR thẻ BHYT / quét CCCD / tự chuyển đối tượng BHYT | Như TC1 |
| 8 | 1 | Đối tượng Viện phí (không dùng thẻ) | Tìm BN | Không xử lý khu vực |
| 9 | 0 | BN cũ K1, thẻ không có khu vực | Kiểm tra thẻ + Lưu | Giữ K1 như hiện tại |
| 10 | Chưa INSERT key | Như TC9 | Như TC9 | Giữ K1 như hiện tại |
| 11 | 1 | Hồ sơ tiếp đón trước ngày áp dụng | Mở lại / sửa | Dữ liệu cũ không đổi |
