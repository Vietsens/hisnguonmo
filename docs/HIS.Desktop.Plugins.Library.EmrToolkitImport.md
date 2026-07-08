# HIS.Desktop.Plugins.Library.EmrToolkitImport — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.Library.EmrToolkitImport |
| Loại | Library (.dll dùng chung — KHÔNG phải MEF module) |
| Mục đích | Gửi dữ liệu JSON (mẫu phiếu, hồ sơ) qua API import của **EMRTOOLKIT** và nhận lại dữ liệu đã giải mã. Đóng gói trọn luồng 3 bước để mọi plugin cùng gọi. |
| Người tạo | nampp |
| Ngày tạo | 30/06/2026 |
| Trạng thái | Đang phát triển (test) |

Thư viện được tách riêng theo yêu cầu "không liên quan đến chức năng gọi, để chỗ khác còn gọi vào". Các plugin tiêu dùng:
- `HIS.Desktop.Plugins.TreatmentList` — menu chuột phải "Liên thông EmrToolKit dữ liệu chuyển tuyến" (test JSON mẫu).
- `HIS.Desktop.Plugins.TreatmentFinish` — checkbox "Liên thông EmrToolKit dữ liệu chuyển tuyến" trên form Kết thúc điều trị; sau khi lưu chuyển viện thành công thì đồng bộ dữ liệu thật của hồ sơ.

---

## 2. Quy Trình Nghiệp Vụ

Luồng gửi dữ liệu gồm **3 bước HTTP** tới EMRTOOLKIT:

```
1. CreateToken     POST {BaseUrl}/api/Token/CreateToken
                   body {TenDangNhap, MatKhau}                 → Data.Token
                            │
                            ▼  (header tokencode = Token)
2. MaHoaJson       POST {BaseUrl}/api/EMR/MaHoaJson
                   body = EmrImportModel (JSON đầy đủ)          → Data {DuLieu, KeyGiaiMa}
                            │
                            ▼  (header tokencode = Token)
3. Import          POST {BaseUrl}/api/EMR/v2/Import
                   body {IDMauPhieu, MaCSKCB, DuLieu, KeyGiaiMa} → Data (JSON đã giải mã = dữ liệu đã gửi)
```

- Phản hồi mỗi API theo cấu trúc `Output<T>`: `{ Success, Message, Data }`.
- Bật **TLS 1.2** trước khi gọi (endpoint HTTPS ngoài).
- Bất kỳ bước nào fail → trả `EmrToolkitImportResult.Success = false` kèm `Step` dừng lại.

### Điều kiện
- Cần cấu hình kết nối hợp lệ (xem mục 5). Nếu thiếu config → dùng giá trị mặc định (theo tài liệu API) để test.

---

## 3. Cấu Trúc Thư Mục

```
HIS.Desktop.Plugins.Library.EmrToolkitImport/
├── EmrToolkitImportProcessor.cs      # API CÔNG KHAI: ImportEmr, ImportEmrAndShowResult, ShowResult
├── Service/
│   └── EmrToolkitApiService.cs       # 3 bước HTTP (HttpClient + Newtonsoft.Json)
├── Config/
│   └── EmrToolkitConfigCFG.cs        # Đọc HisConfig + default fallback
├── Models/
│   ├── EmrImportModel.cs             # JSON Giấy Chuyển Viện (~field theo tài liệu MaHoaJson)
│   ├── EmrOutput.cs                  # Output<T> chung
│   ├── CreateTokenRequestADO.cs      # Body CreateToken
│   ├── TokenResultADO.cs             # Data CreateToken
│   ├── MaHoaJsonResultADO.cs         # Data MaHoaJson {DuLieu, KeyGiaiMa}
│   ├── ImportRequestADO.cs           # Body Import
│   └── EmrToolkitImportResult.cs     # Kết quả tổng hợp + enum EmrToolkitImportStep
├── Popup/
│   └── frmEmrToolkitImportResult.cs  # Form kết quả (FormBase) — JSON gửi/nhận + nút Copy
├── Resources/                        # Lang.vi/en + Message.Lang.vi/en + 2 manager
└── Properties/AssemblyInfo.cs
```

---

## 4. API Công Khai (cho plugin khác gọi)

```csharp
var proc = new HIS.Desktop.Plugins.Library.EmrToolkitImport.EmrToolkitImportProcessor();

// 1) Chỉ gọi API, nhận kết quả (không UI) — tự bọc WaitingManager nếu cần
EmrToolkitImportResult result = proc.ImportEmr(model);

// 2) Hiển thị cửa sổ kết quả cho 1 result đã có
proc.ShowResult(result, ownerForm);

// 3) Tiện lợi: gọi API + hiển thị luôn (network đồng bộ, không bọc waiting)
proc.ImportEmrAndShowResult(model, ownerForm);
```

`model` là `EmrImportModel`. Có thể để trống `IDMauPhieu` và `MaCoSoKhamChuaBenh` → thư viện tự điền theo cấu hình.

---

## 5. Cấu Hình (HisConfig)

Dùng **1 key duy nhất** khai báo thông tin cổng EmrToolkit:

| Config key | Định dạng giá trị |
|------------|-------------------|
| `HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo` | `<địa chỉ>|<tài khoản>|<mật khẩu>` (tùy chọn thêm `|<IDMauPhieu>`) |

Ví dụ: `https://emrtoolkit-api.hkpro.vn|admin@2|Admin@123456`

- `BaseUrl`, `Username`, `Password` tách từ chuỗi theo dấu `|`.
- `MaCSKCB` lấy theo token trả về (CreateToken.Data.MaCSKCB).
- `IDMauPhieu` Giấy Chuyển Viện mặc định `524`, có thể ghi đè bằng phần thứ 4 của chuỗi.
- `HasConnectionInfo` = true khi đủ địa chỉ + tài khoản + mật khẩu → dùng làm điều kiện hiển thị menu/checkbox.
- Nếu key trống → `ImportEmr` trả về thất bại với thông báo "Chưa cấu hình kết nối EMRTOOLKIT".

API tĩnh hỗ trợ điều kiện hiển thị ở plugin gọi:
```csharp
bool ok = EmrToolkitImportProcessor.IsConfigured();
```

---

## 6. Dependencies

| Loại | Thư viện |
|------|----------|
| JSON | `Newtonsoft.Json` |
| HTTP | `System.Net.Http` (HttpClient) |
| Logging | `Inventec.Common.Logging` |
| Đa ngôn ngữ | `Inventec.Common.Resource`, `Inventec.Desktop.Common.LanguageManager` |
| Form | `HIS.Desktop.Utility` (FormBase), DevExpress v15.2 |
| Config | `HIS.Desktop.LocalStorage.HisConfig`, `HIS.Desktop.LocalStorage.Location` |

---

## 7. Model EmrImportModel (Giấy Chuyển Viện)

Khớp đúng key tài liệu API `MaHoaJson`, nhóm theo: hành chính bệnh nhân, BHYT, người nhà, điều trị/khoa phòng, tóm tắt bệnh án, hỗ trợ điều trị (truyền dịch/vận mạch), tình trạng lúc chuyển viện, người ký/mẫu phiếu. Kiểu dữ liệu: `int` (mã/loại), `DateTime?` (ngày), `string` (còn lại). Các thuộc tính có `[JsonProperty]` để khớp tên key gửi đi.

---

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 30/06/2026 | nampp | Tạo mới thư viện: 3 bước CreateToken → MaHoaJson → Import; model Giấy Chuyển Viện; form kết quả hiển thị JSON gửi/nhận + nút Copy. |
| 30/06/2026 | nampp | Chuyển cấu hình sang 1 key `HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo` (dạng `địa_chỉ|tài_khoản|mật_khẩu`); MaCSKCB lấy theo token; thêm `IsConfigured()`. Parse response API chịu được cả mảng `[{}]` lẫn object `{}` (fix lỗi bước Import báo thất bại dù có dữ liệu trả về). |

---

## 9. Test Cases

- [ ] Hồ sơ hợp lệ → 3 bước thành công → form kết quả hiện màu xanh + JSON nhận về.
- [ ] Sai username/password → dừng ở bước CreateToken, form hiện màu đỏ + thông báo.
- [ ] Mất mạng / sai URL → dừng đúng bước, ghi log `LogSystem.Error`.
- [ ] Nút "Sao chép JSON" → copy đúng nội dung tab đang xem.
- [ ] Đổi `EMR_TOOLKIT.*` trong HisConfig → giá trị mới được áp dụng.
