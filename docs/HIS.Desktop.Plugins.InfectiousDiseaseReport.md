# Báo Cáo Ca Bệnh Truyền Nhiễm Lên Cổng ECDS — Tài Liệu Thiết Kế

> Plugin đẩy ca bệnh truyền nhiễm từ HIS lên **Cổng Giám sát Bệnh truyền nhiễm Quốc gia (ECDS)**
> `https://daotao-gs.vadp.gov.vn` (môi trường đào tạo) — API tích hợp `/api/fast/v1/*`.
> Thiết kế tham khảo kiến trúc & giao diện của `HIS.Desktop.Plugins.MchTreatmentExamService`.

---

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | `HIS.Desktop.Plugins.InfectiousDiseaseReport` |
| Loại | **2 màn hình**: (a) **Form chi tiết** đẩy từng ca theo ngữ cảnh điều trị; (b) **Form danh sách** đẩy hàng loạt / tự động |
| Mục đích | Lấy thông tin ca bệnh truyền nhiễm từ HIS, ánh xạ (mapping) sang chuẩn ECDS, đẩy lên cổng quốc gia và lưu lại mã đối soát (`maCaBenh`) |
| Hệ thống đích | ECDS — REST API bên ngoài, xác thực **JWT Bearer**, kết nối **trực tiếp từ client** |
| Người tạo | (điền khi tạo) |
| Trạng thái | Thiết kế |

### Quyết định đã chốt

| # | Quyết định | Ảnh hưởng thiết kế |
|---|-----------|--------------------|
| 1 | HIS lưu **mã xã chuẩn Tổng cục Thống kê (GSO)** | Map địa bàn HIS↔ECDS **trực tiếp theo mã GSO** — không cần bảng ánh xạ thủ công cho Tỉnh/Xã/Thôn (xem §8) |
| 2 | Kết nối ECDS **trực tiếp từ client** (không qua proxy) | `EcdsApiWorker` gọi `HttpClient` thẳng tới `BASE_URL`; lưu ý firewall/HTTPS tại từng trạm (xem §6.4) |
| 3 | Vừa đẩy **thủ công từng ca**, vừa có **màn danh sách + đẩy hàng loạt + tự động** | Thêm `frmInfectiousDiseaseReportList` + `cap-nhat-nhieu` + worker nền tự đẩy (xem §4.5) |
| 4 | Mã đơn vị/cơ sở lấy từ **config** | `ECDS.API.MA_DON_VI`, `ECDS.API.MA_CO_SO_DIEU_TRI` (xem §6.3) |
| 5 | Swagger **KHÔNG có enum** cho các trường integer | `EnumEcds.cs` phải dựng từ **tài liệu nghiệp vụ ECDS**, không suy từ swagger (xem §6.5) |

### Điểm khác biệt cốt lõi so với plugin nội bộ

| Khía cạnh | Plugin HIS thông thường | Plugin này |
|-----------|-------------------------|------------|
| Backend | MOS/ACS/SDA nội bộ qua `BackendAdapter` + `HisRequestUriStore` | **Cổng ECDS bên ngoài** — KHÔNG dùng `BackendAdapter`. Dùng `EcdsApiWorker` (HTTP client riêng) |
| Xác thực | Token nội bộ (`TokenCodeStore`) | **JWT** lấy từ `POST /api/fast/v1/auth/login`, cache theo `expiresIn` |
| Response | `{ Success, Data, Param }` | **Bao gói tiếng Việt**: `{ thanhCong, maLoi, thongDiep, duLieu }` |
| Danh mục | `BackendDataWorker.Get<T>()` (mã HIS) | Mã HIS **KHÔNG trùng** mã ECDS → phải mapping qua `/danh-muc/*` |

---

## 2. Kiến Trúc

### 2.1 Luồng khởi tạo (chuẩn Processor → Factory → Behavior → Form)

```
Menu/Treatment context
  → InfectiousDiseaseReportProcessor.Run(args)            [ExtensionOf — MEF]
    → InfectiousDiseaseReportFactory.MakeIControl(param, args)
      → InfectiousDiseaseReportBehavior.Run()             [parse args: Module, HIS_TREATMENT, RefeshReference]
        → CÓ HIS_TREATMENT → frmInfectiousDiseaseReport(...)       [Form chi tiết — §4]
        → KHÔNG có          → frmInfectiousDiseaseReportList(...)   [Form danh sách — §4.5]
```

Giống reference: `Behavior.Run()` duyệt `entity[]`, bắt `Module`, `HIS_TREATMENT`, `HIS.Desktop.Common.RefeshReference`. **Có `HIS_TREATMENT` → mở Form chi tiết; không có → mở Form danh sách.**

### 2.2 Lớp tích hợp ngoài (mới — không có ở plugin nội bộ)

```
frmInfectiousDiseaseReport
  → Worker/EcdsApiWorker.cs        ← HTTP client tới ECDS (login, danh-muc, cap-nhat)
      ↕ Config/EcdsConfigCFG.cs    ← BaseUrl, Username, Password, MaDonVi (từ HisConfigs)
      ↕ Worker/EcdsTokenStore.cs   ← cache accessToken + thời điểm hết hạn (static)
  → Worker/DiseaseCaseMapper.cs    ← map V_HIS_TREATMENT + V_HIS_PATIENT → DiseaseCaseFastDto
  → ADO/*                          ← DTO khớp schema ECDS
```

> **Quy tắc:** UI (Form) **không** gọi HTTP trực tiếp. Mọi call ECDS đi qua `EcdsApiWorker`. Form chỉ gọi Worker và hiển thị kết quả — đúng nguyên tắc "không logic API trong Form".

---

## 3. Quy Trình Nghiệp Vụ — 4 Bước

```
[1] ĐĂNG NHẬP
    EcdsApiWorker.Login()  →  POST /api/fast/v1/auth/login {username, password}
    → nhận accessToken, refreshToken, expiresIn
    → EcdsTokenStore lưu token + thời điểm hết hạn (chỉ login lại khi hết hạn)

[2] LẤY & MAP DANH MỤC
    Lần đầu mở form: nạp danh mục ECDS cần thiết vào cache RAM:
      /danh-muc/benh, /danh-muc/tinh, /danh-muc/xa, /danh-muc/thon,
      /danh-muc/nghe-nghiep, /danh-muc/dan-toc, /danh-muc/quoc-gia,
      /danh-muc/phan-loai-lam-sang (theo maIcd10Benh)
    → Ánh xạ mã HIS ↔ mã ECDS (xem Section 8 — vấn đề mapping)

[3] ĐẨY CA BỆNH
    DiseaseCaseMapper.Map(treatment, patient, formInput) → DiseaseCaseFastDto
    EcdsApiWorker.CapNhatCaBenh(dto)  →  POST /api/fast/v1/ca-benh/cap-nhat
    (đẩy hàng loạt: POST /api/fast/v1/ca-benh/cap-nhat-nhieu)

[4] ĐỐI SOÁT KẾT QUẢ
    Đọc { thanhCong, maLoi, thongDiep, duLieu.maCaBenh }
    → thanhCong=true: lưu maCaBenh vào HIS (bảng mapping nội bộ / ghi chú điều trị),
      hiển thị "Đã đẩy — Mã ca bệnh: XXX"
    → thanhCong=false: hiển thị thongDiep (đỏ), cho phép sửa & đẩy lại
```

### Điều kiện nghiệp vụ

- Chỉ cho đẩy khi ICD của điều trị **thuộc danh mục bệnh truyền nhiễm** (kiểm qua `/danh-muc/benh`).
- Bắt buộc map được: `maIcd10Benh`, `maGioiTinh`, `maDanToc`, `maNgheNghiep`, `maXaHienNay` (các trường `required` của DTO).
- Nếu bệnh là **sốt rét** → hiển thị & bắt buộc nhóm trường sốt rét (soi lam, RDT, G6PD, thuốc...).
- Đẩy lại (update) dùng lại `id`/`maCaBenh` đã lưu để tránh tạo trùng.

---

## 4. Thiết Kế Giao Diện (tương tự MchTreatmentExamService)

Reference dùng **`XtraTabControl` nhiều tab + header thông tin điều trị + BarManager phím tắt**. Áp dụng nguyên mô hình đó.

### 4.1 Sơ đồ tổng thể

```
+---------------------------------------------------------------------------------+
| [HEADER - chỉ đọc] Mã ĐT: .... | Bệnh nhân: .... | Ngày sinh: .. | ICD: ....    |
|                    Trạng thái đẩy: ● Chưa đẩy / ✔ Đã đẩy (Mã CB: 123456)         |
+---------------------------------------------------------------------------------+
| Tab: [Ca bệnh] [Hành chính] [Triệu chứng & XN] [Sốt rét]* [Người báo cáo]       |
| +-----------------------------------------------------------------------------+ |
| |  (nội dung tab đang chọn — LayoutControl)                                   | |
| +-----------------------------------------------------------------------------+ |
+---------------------------------------------------------------------------------+
| [Lấy dữ liệu từ HIS] [Kiểm tra danh mục] [Đẩy lên cổng (Ctrl+S)] [Mới] [Đóng]  |
+---------------------------------------------------------------------------------+
   (*) Tab "Sốt rét" chỉ hiện khi ICD chẩn đoán là sốt rét
```

### 4.2 Chi tiết từng tab (map thẳng field DTO)

**Tab 1 — Ca bệnh** (`maroon` = bắt buộc)
| Control | prefix | Field DTO | UC/Editor | Ghi chú |
|---------|--------|-----------|-----------|---------|
| Bệnh (ICD-10) *maroon* | `cboBenh` | `maIcd10Benh` | GridLookUpEdit (nguồn `/danh-muc/benh`) | lọc bệnh truyền nhiễm |
| Phân loại lâm sàng | `cboPhanLoaiLamSang` | `maPhanLoaiLamSang` | GridLookUpEdit (`/danh-muc/phan-loai-lam-sang?maIcd10Benh`) | phụ thuộc bệnh |
| Loại chẩn đoán *maroon* | `cboLoaiChanDoan` | `loaiChanDoan` | LookUpEdit (enum) | |
| Tình trạng hiện tại | `cboTinhTrang` | `tinhTrangHienTai` | LookUpEdit (enum) | |
| Ngày khởi phát *maroon* | `dteNgayKhoiPhat` | `ngayKhoiPhat` | DateEdit | |
| Ngày nhập viện | `dteNgayNhapVien` | `ngayNhapVien` | DateEdit | từ điều trị |
| Ngày ra viện | `dteNgayRaVien` | `ngayRaVien` | DateEdit | từ điều trị |
| Chẩn đoán ra viện | `txtChanDoanRaVien` | `chanDoanRaVien` | MemoEdit | |
| TT tiêm vắc xin | `cboTiemVacXin` | `thongTinTiemVacXin` | LookUpEdit (enum) | |
| Bệnh kèm theo | `txtBenhKemTheo` | `benhKemTheo` | MemoEdit | |
| Biến chứng | `txtBienChung` | `bienChung` | MemoEdit | |
| Ghi chú | `txtGhiChu` | `ghiChuChung` | MemoEdit | |

**Tab 2 — Hành chính**
| Control | Field DTO | UC/Editor |
|---------|-----------|-----------|
| Họ và tên *maroon* | `hoVaTen` | TextEdit (từ BN) |
| Ngày sinh *maroon* | `ngaySinh` | DateEdit |
| Tuổi *maroon* | `tuoi` | SpinEdit (tính từ ngày sinh) |
| Giới tính *maroon* | `maGioiTinh` | LookUpEdit (mapping HIS→ECDS) |
| Đang mang thai | `dangMangThai` | CheckEdit (ẩn nếu nam) |
| Dân tộc *maroon* | `maDanToc` | GridLookUpEdit (`/danh-muc/dan-toc`) |
| Nghề nghiệp *maroon* | `maNgheNghiep` | GridLookUpEdit (`/danh-muc/nghe-nghiep`) |
| Nơi làm việc | `noiLamViec` | TextEdit |
| Số CCCD/CMND | `soCccdCmnd` | TextEdit |
| Số điện thoại | `soDienThoai` | TextEdit |
| **Địa chỉ hiện nay** | | |
| — Tỉnh/Xã/Thôn *maroon* | `maXaHienNay`, `maThonHienNay` | **3 combo liên kết ECDS** (Tỉnh→Xã→Thôn qua `/danh-muc/*`) |
| — Chi tiết | `diaChiChiTietHienNay` | TextEdit |
| Xã quản lý | `maXaPhuongQuanLy` | combo ECDS |
| Tên người thân | `tenNguoiThan` | TextEdit |

> Ghi chú UC: `HIS.UC.AddressCombo` chỉ dùng cho **mã hành chính HIS**. Cổng ECDS dùng **mã quốc gia riêng** → cần combo Tỉnh/Xã/Thôn nạp từ `/danh-muc/*` của ECDS (KHÔNG tái dùng `AddressCombo` trực tiếp). Có thể tạo UC nội bộ `UCEcdsAddress` (3 GridLookUpEdit liên kết) — tham khảo cấu trúc `UCAddress` của reference.

**Tab 3 — Triệu chứng & Xét nghiệm**
| Nhóm | Field DTO |
|------|-----------|
| Triệu chứng (checkbox) | `trieuChungSot`, `trieuChungRetRun`, `trieuChungVaMoHoi`, `trieuChungKhac` + `moTaTrieuChungKhac` |
| Dịch tễ (checkbox) | `trieuChungTuongTuTrongGiaDinh`, `trieuChungTuongTuNoiLamViec`, `tienSuDichTe` |
| Lấy mẫu XN | `coLayMauXetNghiem`, `tenXetNghiem`, `loaiXetNghiemChung`, `ngayLayMau`+`gioLayMau`+`phutLayMau` |
| Kết quả XN | `ngayTraKetQua`+`gioTraKetQua`+`phutTraKetQua`, `ketQuaXetNghiemChung`, `maDonViXetNghiem` |
| Cơ sở điều trị | `maCoSoDieuTri`, `maHinhThucDieuTri` |

**Tab 4 — Sốt rét** *(chỉ hiện khi ICD = sốt rét)*
| Field DTO | Editor |
|-----------|--------|
| `phuongPhapPhatHienSotRet`, `maDonViXetNghiemSotRet`, `loaiCoSoXetNghiemSotRet` | LookUp/combo |
| `ketQuaSoiLam`, `ketQuaRdt`, `matDoKySinhTrung`, `loaiSotRetChanDoan` | LookUp/text |
| `xetNghiemG6pd`, `ketQuaDinhLuongG6pd`, `phanLoaiG6pd` | LookUp/text |
| `ngayBatDauDieuTri`, `danhSachThuocSotRet[]` (`/danh-muc/thuoc-sot-ret`) | DateEdit + Grid |
| `daTungMacSotRet`, `coGiaoBao`, `maPhanLoaiCaBenhSotRet`, `lichSuDiChuyenDichTe[]` | LookUp/Grid |

**Tab 5 — Người báo cáo** (mặc định điền từ user đăng nhập & cấu hình đơn vị)
| Field DTO | Nguồn |
|-----------|-------|
| `hoTenNguoiBaoCao` | tên nhân viên đăng nhập |
| `soDienThoaiNguoiBaoCao`, `emailNguoiBaoCao` | nhân viên / config |
| `maDonViNguoiBaoCao` | `EcdsConfigCFG.MaDonVi` |

### 4.3 Nút & phím tắt (BarManager — chuẩn FormBase)
| Nút | Hành động | Phím tắt |
|-----|-----------|----------|
| Lấy dữ liệu từ HIS | `DiseaseCaseMapper` fill form từ treatment/patient | Ctrl+L |
| Kiểm tra danh mục | validate mọi mã đã map được sang ECDS chưa | Ctrl+K |
| **Đẩy lên cổng** | bước [3]+[4] | **Ctrl+S** |
| Mới | clear form (giữ thông tin người báo cáo) | Ctrl+N |
| Đóng | đóng form | Esc |

### 4.5 Màn hình danh sách & đẩy hàng loạt / tự động (`frmInfectiousDiseaseReportList`)

Màn hình thứ hai — mở từ menu, quản lý toàn bộ ca bệnh truyền nhiễm cần báo cáo trong 1 khoảng thời gian.

```
+---------------------------------------------------------------------------------+
| [Từ ngày][Đến ngày] [Khoa] [Nhóm bệnh BTN] [Trạng thái đẩy: Tất cả ▾] [Tìm kiếm]|
| [☑ Tự động đẩy]  Chu kỳ: [15] phút                                               |
+---------------------------------------------------------------------------------+
| ☑ | STT | Mã ĐT | Bệnh nhân | ICD | Ngày KP | TT đẩy | Mã CB ECDS | Thông điệp   |
|---|-----|-------|-----------|-----|---------|--------|-----------|--------------|
| ☑ |  1  | ...   | ...       | ... | ...     | ✔ Đã đẩy| 123456   |              |
| ☑ |  2  | ...   | ...       | ... | ...     | ✖ Lỗi  |           | Thiếu mã xã  |
| ☐ |  3  | ...   | ...       | ... | ...     | ○ Chưa |           |              |
+---------------------------------------------------------------------------------+
| [Xem/Sửa chi tiết] [Đẩy các ca đã chọn] [Đối soát với cổng] [Xuất Excel] [Đóng] |
+---------------------------------------------------------------------------------+
```

| Thành phần | Mô tả |
|-----------|-------|
| Nguồn dữ liệu | Danh sách điều trị có ICD thuộc DS bệnh truyền nhiễm (lấy từ HIS qua `BackendAdapter` + paging `Inventec.UC.Paging`) |
| Cột trạng thái đẩy | ○ Chưa đẩy / ✔ Đã đẩy / ✖ Lỗi — lấy từ **bảng đối soát nội bộ** (lưu `treatmentId ↔ maCaBenh ↔ lastPushTime ↔ status`) |
| **Đẩy các ca đã chọn** | Map từng ca → gọi **`/api/fast/v1/ca-benh/cap-nhat-nhieu`** (batch), đọc success/error count, cập nhật cột trạng thái + `maCaBenh` |
| **Xem/Sửa chi tiết** | Mở `frmInfectiousDiseaseReport` (màn §4) cho ca đang chọn để sửa trước khi đẩy |
| **Đối soát với cổng** | Gọi `/api/fast/v1/ca-benh/danh-sach` để so khớp ca đã có trên ECDS, tránh đẩy trùng |
| **Tự động đẩy** | Checkbox + chu kỳ (phút) → `EcdsAutoPushWorker` (Timer) đẩy nền các ca "Chưa đẩy"/"Lỗi" đủ điều kiện. Trạng thái checkbox nhớ qua `ControlStateWorker` |
| Xuất Excel | xuất danh sách + trạng thái đẩy để đối chiếu thủ công |

**Nguyên tắc đẩy hàng loạt/tự động:**
- Chỉ đẩy ca **map đủ** trường bắt buộc; ca thiếu mã → để "Lỗi" kèm `thongDiep`, KHÔNG chặn cả batch.
- Batch chia lô (VD ≤ 50 ca/call) tránh payload quá lớn.
- Auto-push chạy trên `Timer` nền + `WaitingManager` **không** khóa UI; KHÔNG cập nhật grid trong thread — marshal về UI thread (`Invoke`).
- Login 1 lần dùng chung token cho cả batch (`EcdsTokenStore`).
- Ghi `LogAction.Info` mỗi phiên đẩy (audit): số ca thành công/thất bại.

---

## 4b. Ảnh Giao Diện (Mockup)

**Tab Ca bệnh + Màn danh sách:**

![Tab Ca bệnh và màn danh sách](ecds-ui-mockup.png)

**Giao diện tổng (1 màn, các mục thu gọn — accordion):**

![Giao diện tổng accordion](ecds-ui-overview.png)

**Các tab còn lại (Hành chính · Triệu chứng & XN · Sốt rét · Người báo cáo):**

![Các tab còn lại](ecds-ui-tabs.png)

---

## 5. Ánh Xạ HIS → DiseaseCaseFastDto (Mapper)

Nguồn HIS: `V_HIS_TREATMENT` + `V_HIS_PATIENT` (+ `V_HIS_PATIENT_TYPE_ALTER` nếu cần).

| DTO (ECDS) | Nguồn HIS | Cần mapping mã? |
|------------|-----------|-----------------|
| `hoVaTen` | `TDL_PATIENT_NAME` | Không |
| `ngaySinh` | `TDL_PATIENT_DOB` (long→date) | Không |
| `tuoi` | tính từ DOB | Không |
| `maGioiTinh` | `TDL_PATIENT_GENDER_ID` | **Có** (HIS gender → ECDS gender) |
| `soCccdCmnd` | `TDL_PATIENT_CMND_NUMBER` / CCCD | Không |
| `soDienThoai` | patient phone | Không |
| `maIcd10Benh` | `ICD_CODE` | **Có** (đối chiếu `/danh-muc/benh`) |
| `maDanToc` | patient ethnic code | **Có** |
| `maNgheNghiep` | patient career code | **Có** |
| `maXaHienNay` / `maThonHienNay` | patient commune/hamlet code | **Có** (HIS admin → ECDS admin) |
| `diaChiChiTietHienNay` | patient address | Không |
| `ngayNhapVien` | `IN_TIME` | Không |
| `ngayRaVien` | `OUT_TIME` | Không |
| `chanDoanRaVien` | `ICD_NAME` / `ICD_TEXT` | Không |
| `maCoSoDieuTri` | mã cơ sở KCB của viện | **Có** (config) |
| `maDonViNguoiBaoCao` | `EcdsConfigCFG.MaDonVi` | config |

> **DateTime:** HIS lưu `long yyyyMMddHHmmss`; ECDS nhận chuỗi `date` (ISO `yyyy-MM-dd`). Mapper phải convert:
> `Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(x).Value.ToString("yyyy-MM-dd")`.

---

## 6. Lớp API & DTO

### 6.1 Response wrapper (ADO)
```csharp
/// <summary>Bao gói response chuẩn của cổng ECDS.</summary>
public class KetQuaEcdsDto<T>
{
    public bool thanhCong { get; set; }
    public string maLoi { get; set; }
    public string thongDiep { get; set; }
    public T duLieu { get; set; }
}

/// <summary>Dữ liệu login trả về.</summary>
public class DangNhapResultDto
{
    public string accessToken { get; set; }
    public string refreshToken { get; set; }
    public long expiresIn { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public List<string> roles { get; set; }
}
```
`DiseaseCaseFastDto`, `SearchDanhMucFastDto`, `DanhMucItemDto {ma, ten}` — đặt trong `ADO/`, đặt tên field **đúng camelCase như schema ECDS** (JSON serialize khớp).

### 6.2 EcdsApiWorker (khung)
```csharp
internal class EcdsApiWorker
{
    // POST JSON, gắn Bearer token, trả KetQuaEcdsDto<T>
    private KetQuaEcdsDto<T> Post<T>(string path, object body, bool needAuth = true) { ... }

    internal bool Login()                                   // [1] /auth/login
    internal List<DanhMucItemDto> LayDanhMuc(string path, SearchDanhMucFastDto f)  // [2]
    internal KetQuaEcdsDto<DiseaseCaseResultDto> CapNhatCaBenh(DiseaseCaseFastDto dto)  // [3]
}
```
- HTTP: `HttpClient` (hoặc `Inventec.Common.Adapter` REST) — **KHÔNG** `BackendAdapter`.
- Mỗi method try-catch, `LogSystem.Error(ex)` khi lỗi; `WaitingManager.Show/Hide` bao quanh call ở tầng Form.
- Token: `EcdsTokenStore` giữ `accessToken` + `expireAt`; `Login()` chỉ gọi lại khi hết hạn.

### 6.3 Config (`Config/EcdsConfigCFG.cs` — đọc `HisConfigs`)
| Key HisConfig | Ý nghĩa |
|---------------|---------|
| `ECDS.API.BASE_URL` | `https://daotao-gs.vadp.gov.vn` |
| `ECDS.API.USERNAME` | tài khoản tích hợp |
| `ECDS.API.PASSWORD` | mật khẩu |
| `ECDS.API.MA_DON_VI` | mã đơn vị báo cáo |
| `ECDS.API.MA_CO_SO_DIEU_TRI` | mã cơ sở điều trị |

> KHÔNG hardcode URL/tài khoản trong code. KHÔNG log `PASSWORD`, `accessToken` (rule logging).

#### 6.3.1 Chuẩn hoá cấu hình 1 key (theo mẫu KSK 2062) — ĐÃ CHỐT

Thay vì rải nhiều key như bảng trên, **gộp toàn bộ thông tin kết nối vào 1 bản ghi `HIS_CONFIG`** dạng chuỗi phân tách bằng `|` — theo đúng mẫu `MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO`.

**Key:** `MOS.HIS_ECDS_SYNC.ECDS_CONNECTION_INFO`

**Mô tả (điền vào phần mô tả bản ghi `HIS_CONFIG`):**
> Cấu hình hệ thống khai báo thông tin liên thông Báo cáo ca bệnh truyền nhiễm lên Cổng Giám sát Bệnh truyền nhiễm Quốc gia (ECDS).
> Giá trị khai báo có dạng
> `MaDonVi|MaCoSoDieuTri|Username|Password|MaTinh|BaseUrl|LoginPath|PushPath`

**Chú thích từng trường:**

| # | Trường | Bắt buộc | Ý nghĩa | Ví dụ |
|---|--------|:---:|---------|-------|
| 1 | `MaDonVi` | ✅ | Mã đơn vị báo cáo lên ECDS | `30045` |
| 2 | `MaCoSoDieuTri` | ✅ | Mã cơ sở điều trị | `30045001` |
| 3 | `Username` | ✅ | Tài khoản tích hợp ECDS | `bvdakhoa_tichhop` |
| 4 | `Password` | ✅ | Mật khẩu | `••••••` |
| 5 | `MaTinh` | ⚠️ | Mã tỉnh/thành | `01` |
| 6 | `BaseUrl` | ✅ | Địa chỉ cổng | `https://daotao-gs.vadp.gov.vn` |
| 7 | `LoginPath` | ✅ | Đường dẫn đăng nhập lấy token | `/api/fast/v1/auth/login` |
| 8 | `PushPath` | ✅ | Đường dẫn đẩy ca bệnh | `/api/fast/v1/ca-benh/cap-nhat` |

**Ví dụ giá trị (môi trường đào tạo):**
```
30045|30045001|bvdakhoa_tichhop|MatKhau@123|01|https://daotao-gs.vadp.gov.vn|/api/fast/v1/auth/login|/api/fast/v1/ca-benh/cap-nhat
```

**Đọc & parse (`Config/EcdsConfigCFG.cs`):**
```csharp
public class EcdsConnectionInfo
{
    public string MaDonVi { get; set; }
    public string MaCoSoDieuTri { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string MaTinh { get; set; }
    public string BaseUrl { get; set; }
    public string LoginPath { get; set; }
    public string PushPath { get; set; }
}

// KEY = "MOS.HIS_ECDS_SYNC.ECDS_CONNECTION_INFO";  FIELD_COUNT = 8
string[] p = raw.Split('|');
if (p.Length < 8) { LogSystem.Warn("ECDS_CONNECTION_INFO thiếu trường"); return null; }
var cfg = new EcdsConnectionInfo {
    MaDonVi = p[0].Trim(), MaCoSoDieuTri = p[1].Trim(),
    Username = p[2].Trim(), Password = p[3],           // KHÔNG Trim mật khẩu
    MaTinh = p[4].Trim(),  BaseUrl = p[5].Trim().TrimEnd('/'),
    LoginPath = p[6].Trim(), PushPath = p[7].Trim()
};
```

**Lưu ý:**
- Validate `p.Length < 8` → cảnh báo, chặn đẩy, không crash.
- KHÔNG `Trim()` mật khẩu; KHÔNG log `Password`/`accessToken`.
- `BaseUrl` khác nhau giữa đào tạo/production → chỉ đổi 1 chuỗi config.
- Các path phụ (`/ca-benh/cap-nhat-nhieu`, `/ca-benh/danh-sach`, `/danh-muc/*`) suy từ `BaseUrl` + hằng số trong `EcdsApiWorker`, không cần đưa vào config.
- Mật khẩu/URL **không** được chứa ký tự `|`.

> Cấu hình dạng nhiều key ở bảng §6.3 (trên) được thay thế bằng 1 key pipe này.

### 6.4 Kết nối trực tiếp từ client (đã chốt)

- `EcdsApiWorker` dùng `HttpClient` gọi thẳng `BASE_URL` — mỗi trạm làm việc phải mở được HTTPS ra `daotao-gs.vadp.gov.vn` (và domain production tương ứng).
- Bắt buộc **TLS/HTTPS**; nếu môi trường .NET 4.5 mặc định chưa bật TLS 1.2 → set `ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12` khi khởi tạo Worker.
- Xử lý timeout & mất mạng: `HttpClient.Timeout` hợp lý (VD 60s), catch `TaskCanceledException`/`HttpRequestException` → thông báo lỗi kết nối, cho retry (KHÔNG treo UI).
- Vì mỗi client tự đăng nhập → token là **per-máy** trong RAM (`EcdsTokenStore` static). KHÔNG lưu token xuống đĩa.
- Rủi ro cần lưu ý vận hành: nhiều trạm cùng đăng nhập 1 tài khoản tích hợp — xác nhận cổng ECDS cho phép đa phiên; nếu không, cân nhắc tài khoản riêng theo khoa/trạm.

### 6.5 EnumEcds.cs — Giá trị enum (⚠ CHỜ TÀI LIỆU ECDS)

Swagger **KHÔNG khai báo `enum`** cho các trường integer, chỉ có `type: integer` + sample rời rạc (`loaiChanDoan: 1`, `maGioiTinh: "M"`). **Không thể suy giá trị hợp lệ từ swagger** → phải lấy từ tài liệu nghiệp vụ / danh mục ECDS. Trước khi có tài liệu, tạo `EnumEcds.cs` dạng khung, đánh dấu TODO, gán giá trị tường minh + XML comment (đúng coding_rules):

| Trường DTO | Kiểu | Nguồn giá trị | Ghi chú |
|-----------|------|---------------|---------|
| `maGioiTinh` | **string** | mẫu `"M"` | KHÔNG phải int — map từ giới tính HIS sang mã ký tự ECDS |
| `loaiChanDoan` | int | ⚠ cần tài liệu | mẫu `1` |
| `trangThaiCaBenh`, `trangThaiLuu` | int | ⚠ cần tài liệu | required |
| `tinhTrangHienTai` | int | ⚠ cần tài liệu | |
| `thongTinTiemVacXin` | int | ⚠ cần tài liệu | |
| `loaiXetNghiemChung`, `ketQuaXetNghiemChung` | int | ⚠ cần tài liệu | |
| `phuongPhapPhatHienSotRet`, `loaiCoSoXetNghiemSotRet` | int | ⚠ cần tài liệu | nhóm sốt rét |
| `ketQuaSoiLam`, `ketQuaRdt`, `xetNghiemG6pd`, `phanLoaiG6pd`, `loaiSotRetChanDoan`, `daTungMacSotRet` | int | ⚠ cần tài liệu | nhóm sốt rét |

```csharp
/// <summary>
/// Enum các trường phân loại của DiseaseCaseFastDto (cổng ECDS).
/// ⚠ TODO: Giá trị dưới đây là GIẢ ĐỊNH — swagger không khai báo enum.
/// PHẢI đối chiếu tài liệu nghiệp vụ ECDS trước khi dùng thật.
/// </summary>
public enum EcdsLoaiChanDoan
{
    /// <summary>Nghi ngờ (giả định — chờ xác nhận ECDS)</summary>
    NghiNgo = 1,
    /// <summary>Xác định (giả định — chờ xác nhận ECDS)</summary>
    XacDinh = 2
}
// ... các enum còn lại tạo tương tự khi có tài liệu.
```

> Trên UI, các combo enum nạp `display/value` từ chính `EnumEcds` (hoặc danh mục ECDS nếu có endpoint). Tuyệt đối KHÔNG hardcode số trong Mapper — dùng `(int)EcdsLoaiChanDoan.XacDinh`.

---

## 7. Đa Ngôn Ngữ / ControlState / Validation

- **Resources/**: `Lang.vi.resx` + `Lang.en.resx` (mọi caption tab, label, button); `Message.Lang.vi/en.resx` (thông báo riêng: "Bệnh không thuộc danh mục truyền nhiễm", "Chưa map được mã xã sang ECDS"...); `ResourceLanguageManager.cs`, `ResourceMessage.cs`. Gọi `SetCaptionByLanguageKey()` trong Load.
- **ControlState**: checkbox "Tự động đăng nhập lại", chọn đơn vị báo cáo mặc định → nhớ qua `ControlStateWorker` (`moduleLink = "HIS.Desktop.Plugins.InfectiousDiseaseReport"`).
- **Validation**: các trường `required` của DTO → icon warning tại control + `ErrorText`; clear khi hợp lệ / khi nhấn Mới. Nút "Kiểm tra danh mục" chạy pre-check trước khi đẩy.
- **Thread**: mọi call ECDS chạy có `WaitingManager`; KHÔNG cập nhật UI trong thread (đúng ui_rules mục 1).

---

## 8. Mapping Danh Mục

| Danh mục | Chiến lược mapping | Mức rủi ro |
|----------|--------------------|-----------|
| **Tỉnh/Xã/Thôn** | HIS lưu **mã GSO (Tổng cục Thống kê)** → **map trực tiếp** sang mã địa bàn ECDS (ECDS cũng dùng mã GSO). Chỉ cần đối chiếu tồn tại qua `/danh-muc/{tinh,xa,thon}`, KHÔNG cần bảng ánh xạ thủ công | **Thấp** (đã chốt) |
| Bệnh ICD-10 | ICD-10 là chuẩn quốc tế → map trực tiếp; đối chiếu `/danh-muc/benh` để xác nhận **thuộc DS bệnh truyền nhiễm** (nếu không thuộc → chặn đẩy) | Thấp |
| Giới tính | HIS `GENDER_ID` (int) → mã ECDS dạng ký tự (`"M"`/…) — bảng đối chiếu nhỏ, cố định trong Mapper | Thấp |
| Dân tộc, Nghề nghiệp | Đối chiếu theo **mã** nếu HIS dùng mã chuẩn quốc gia; nếu lệch → map theo tên gần đúng + cho chọn lại lần đầu, cache kết quả | Trung bình |

**Cách làm cho địa bàn (đã đơn giản hoá nhờ mã GSO):**
```
patient.commune GSO code (HIS)
  → maXaHienNay = GSO code  (dùng thẳng)
  → maThonHienNay: lấy theo /danh-muc/thon?maXa=GSO
  → verify tồn tại trên ECDS (1 lần, cache) — nếu không thấy → cảnh báo tại control
```

> Combo địa bàn trên form nạp **trực tiếp danh mục ECDS** theo mã GSO; nếu mã GSO của BN không có trong ECDS (địa bàn mới sáp nhập/chưa cập nhật) → cảnh báo tại control, chặn đẩy ca đó (không chặn cả batch).

---

## 9. Cấu Trúc Thư Mục (Medium)

```
HIS.Desktop.Plugins.InfectiousDiseaseReport/
├── InfectiousDiseaseReport/
│   ├── IInfectiousDiseaseReport.cs
│   ├── InfectiousDiseaseReportFactory.cs
│   └── InfectiousDiseaseReportBehavior.cs
├── MainForm/
│   ├── frmInfectiousDiseaseReport.cs
│   ├── frmInfectiousDiseaseReport.Designer.cs
│   ├── frmInfectiousDiseaseReport__Load.cs        ← nạp danh mục, map data
│   ├── frmInfectiousDiseaseReport__Map.cs         ← "Lấy dữ liệu từ HIS"
│   ├── frmInfectiousDiseaseReport__Push.cs        ← bước [3][4] đẩy + đối soát
│   ├── frmInfectiousDiseaseReport__Check.cs       ← validation + kiểm tra danh mục
│   └── frmInfectiousDiseaseReport.resx
├── ListForm/                                       ← MÀN DANH SÁCH (§4.5)
│   ├── frmInfectiousDiseaseReportList.cs
│   ├── frmInfectiousDiseaseReportList.Designer.cs
│   ├── frmInfectiousDiseaseReportList__Load.cs     ← load grid + paging + trạng thái đẩy
│   ├── frmInfectiousDiseaseReportList__Batch.cs    ← đẩy hàng loạt (cap-nhat-nhieu)
│   ├── frmInfectiousDiseaseReportList__Reconcile.cs← đối soát với cổng (ca-benh/danh-sach)
│   └── frmInfectiousDiseaseReportList.resx
├── UCEcdsAddress/                                  ← 3 combo Tỉnh/Xã/Thôn ECDS
├── ADO/
│   ├── DiseaseCaseFastDto.cs
│   ├── KetQuaEcdsDto.cs
│   ├── DangNhapResultDto.cs
│   ├── DanhMucItemDto.cs
│   ├── SearchDiseaseCaseFastDto.cs
│   ├── DiseaseCaseListADO.cs                       ← dòng grid màn danh sách (+ trạng thái đẩy)
│   └── SearchDanhMucFastDto.cs
├── Worker/
│   ├── EcdsApiWorker.cs                            ← login/danh-muc/cap-nhat/cap-nhat-nhieu
│   ├── EcdsTokenStore.cs                           ← cache token static
│   ├── EcdsAutoPushWorker.cs                       ← Timer đẩy nền tự động
│   ├── EcdsPushReconcileStore.cs                   ← bảng đối soát treatmentId↔maCaBenh↔status
│   └── DiseaseCaseMapper.cs
├── Config/
│   └── EcdsConfigCFG.cs
├── Resources/  (Lang + Message + Manager)
├── Properties/AssemblyInfo.cs   ← [assembly: Inventec.Desktop.Core.Plugin]
├── EnumEcds.cs                   ← enum các trường int (loaiChanDoan, ketQua...) có XML comment
├── ModuleLinkString.cs
├── InfectiousDiseaseReportProcessor.cs
└── HIS.Desktop.Plugins.InfectiousDiseaseReport.csproj
```

---

## 10. Cách Mở Plugin (Inter-Plugin)

`Behavior.Run()` chọn màn hình theo args: **có `HIS_TREATMENT` → mở Form chi tiết (§4)**; **không có → mở Form danh sách (§4.5)**.

**Cách 1 — mở Form chi tiết theo 1 điều trị** (nút "Báo cáo BTN quốc gia" tại màn danh sách điều trị / kết thúc khám):
```csharp
List<object> listArgs = new List<object>();
listArgs.Add(currentModule);                         // Module
listArgs.Add(treatment);                             // HIS_TREATMENT (có → mở Form chi tiết)
listArgs.Add(new HIS.Desktop.Common.RefeshReference(OnAfterPush)); // callback refresh
HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(
    ModuleLinkString.InfectiousDiseaseReport,
    currentModule.RoomId, currentModule.RoomTypeId, listArgs);
```

**Cách 2 — mở Form danh sách từ menu** (không truyền `HIS_TREATMENT`):
```csharp
List<object> listArgs = new List<object>();
listArgs.Add(currentModule);                         // Module (không có treatment → mở Form danh sách)
HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(
    ModuleLinkString.InfectiousDiseaseReport,
    currentModule.RoomId, currentModule.RoomTypeId, listArgs);
```

---

## 11. Endpoint ECDS Sử Dụng

| Bước | Method | Path | Body | Trả về |
|------|--------|------|------|--------|
| Đăng nhập | POST | `/api/fast/v1/auth/login` | `{username, password}` | `duLieu.accessToken` |
| Danh mục bệnh | POST | `/api/fast/v1/danh-muc/benh` | `SearchDanhMucFastDto` | `[{ma, ten}]` |
| Tỉnh/Xã/Thôn | POST | `/api/fast/v1/danh-muc/{tinh,xa,thon}` | `{tuKhoa, maTinh, maXa}` | `[{ma, ten}]` |
| Dân tộc/Nghề/QG | POST | `/api/fast/v1/danh-muc/{dan-toc,nghe-nghiep,quoc-gia}` | `SearchDanhMucFastDto` | `[{ma, ten}]` |
| Phân loại LS | POST | `/api/fast/v1/danh-muc/phan-loai-lam-sang` | `{maIcd10Benh}` | `[{ma, ten}]` |
| Thuốc sốt rét | POST | `/api/fast/v1/danh-muc/thuoc-sot-ret` | `SearchDanhMucFastDto` | `[{ma, ten}]` |
| **Đẩy 1 ca** | POST | `/api/fast/v1/ca-benh/cap-nhat` | `DiseaseCaseFastDto` | `duLieu.maCaBenh` |
| Đẩy nhiều ca | POST | `/api/fast/v1/ca-benh/cap-nhat-nhieu` | `[DiseaseCaseFastDto]` | success/error count |
| Tra cứu đã đẩy | POST | `/api/fast/v1/ca-benh/danh-sach` | `SearchDiseaseCaseFastDto` | danh sách phân trang |

---

## 12. Changelog

| Ngày | Người | Mô tả |
|------|-------|-------|
| 24/07/2026 | nampp | Tạo bản thiết kế plugin (Form chi tiết đẩy từng ca) |
| 24/07/2026 | nampp | Bổ sung: mã GSO cho địa bàn, kết nối trực tiếp, màn danh sách + đẩy hàng loạt/tự động, khung EnumEcds (chờ tài liệu ECDS) |
| 26/07/2026 | nampp | Fill tab Hành chính từ `V_HIS_PATIENT` (`api/HisPatient/GetView`): CCCD/CMND, điện thoại, nơi làm việc, địa chỉ hiện nay + thường trú (text điền luôn); combo dân tộc/nghề nghiệp/tỉnh/xã map mã HIS→ID ECDS qua danh mục (xã cascade theo tỉnh), chỉ khi ECDS đã cấu hình. Thêm `HIS_PATIENT_GETVIEW` + ref `MOS.Filter`. Sửa binding Newtonsoft.Json về 6.0.0.0 khớp deploy. |

## 13. Test Cases

- [ ] Mở form từ 1 điều trị có ICD truyền nhiễm → header & các tab tự fill từ HIS.
- [ ] ICD không thuộc DS truyền nhiễm → cảnh báo, chặn đẩy.
- [ ] Login sai tài khoản → hiển thị `thongDiep` lỗi, không crash.
- [ ] Token còn hạn → không login lại (đẩy nhiều ca liên tiếp chỉ login 1 lần).
- [ ] Thiếu trường `required` (giới tính, xã...) → icon warning tại control, chặn đẩy.
- [ ] Đẩy thành công → hiển thị `maCaBenh`, lưu đối soát, gọi callback refresh.
- [ ] Đẩy lại ca đã có `maCaBenh` → update, không tạo trùng.
- [ ] Bệnh sốt rét → hiện tab Sốt rét, bắt buộc nhóm trường sốt rét.
- [ ] Mất mạng khi đẩy → thông báo lỗi kết nối, cho retry.
- [ ] Chuyển ngôn ngữ vi/en → caption đổi đúng.
- [ ] **Màn danh sách**: lọc theo ngày/khoa → hiện đúng ca BTN, đúng cột trạng thái đẩy.
- [ ] **Đẩy hàng loạt**: chọn nhiều ca → `cap-nhat-nhieu` → cập nhật trạng thái & `maCaBenh` từng dòng.
- [ ] Batch có ca lỗi (thiếu mã xã GSO) → ca lỗi để "Lỗi" kèm `thongDiep`, các ca còn lại vẫn đẩy được.
- [ ] **Tự động đẩy**: bật checkbox + chu kỳ → worker nền đẩy ca "Chưa đẩy", không khóa UI, grid cập nhật đúng.
- [ ] **Đối soát**: ca đã có trên ECDS → không tạo trùng.
- [ ] Địa bàn dùng **mã GSO** → map thẳng, đẩy thành công không cần nhập lại xã.

---

## 14. Trạng Thái Câu Hỏi Thiết Kế

| # | Câu hỏi | Trạng thái |
|---|---------|-----------|
| 1 | Map địa bàn HIS↔ECDS | ✅ Chốt: HIS lưu **mã GSO** → map trực tiếp (§8) |
| 2 | Kết nối trực tiếp hay qua proxy | ✅ Chốt: **trực tiếp từ client** (§6.4) |
| 3 | Từng ca hay danh sách/hàng loạt/tự động | ✅ Chốt: **cả hai** — Form chi tiết + Form danh sách + auto-push (§4.5) |
| 4 | Mã đơn vị/cơ sở | ✅ Lấy từ **config** `MA_DON_VI` / `MA_CO_SO_DIEU_TRI` (§6.3) |
| 5 | Giá trị enum các trường integer | ⚠️ **Còn treo** — swagger không có enum; cần **tài liệu nghiệp vụ ECDS** để hoàn thiện `EnumEcds.cs` (§6.5) |

### Còn cần trước khi code
- **Tài liệu enum ECDS** (mục 5) — chặn phần Mapper cho các trường phân loại/kết quả. Tạm dùng khung `EnumEcds.cs` (TODO) để không chặn khung sườn.
- Xác nhận **tài khoản tích hợp** cho phép **đa phiên** (nhiều trạm cùng login) — nếu không, cấp tài khoản theo khoa/trạm (§6.4).
- Danh mục **Dân tộc / Nghề nghiệp**: đã chốt nguồn HIS (xem §17).

---

## 15. Spec Chính Thức ECDS (tầng dữ liệu)

Theo **tài liệu nghiệp vụ ECDS**, ca bệnh gồm 2 khối: **thông tin hành chính** + **TRUONG_HOP_BENH**. Tên trường dạng `UPPER_SNAKE`, danh mục là **ID số nội bộ ECDS** (VD `TINH_ID=709`, `XA_ID=127976`, `BENHCHUANDOAN_ID=40`) — **KHÔNG phải mã GSO**.

> ⚠ Điều chỉnh so với §8: địa bàn (và các danh mục khác) phải **map mã HIS → ID ECDS** qua `/danh-muc/*`, KHÔNG map thẳng. GSO chỉ là khóa đối chiếu để lấy đúng ID ECDS.

### 15.1 Enum ECDS (giá trị chính thức)

| Trường ECDS | Giá trị | ⚠ |
|-------------|---------|----|
| `GIOITINH` | 0=Nữ, 1=Nam | |
| `IS_MANGTHAI` | 0=Không, 1=Có | |
| `SUDUNGVACXIN` | **0=Có**, 1=Không, 2=Không rõ | polarity đảo |
| `PHANLOAICHUANDOAN` | 0=Nghi ngờ, 1=Xác định | |
| `LAYMAUXETNGHIEM` | **0=Có**, 1=Không | polarity đảo |
| `LOAIXETNGHIEM` | 0=Test nhanh, 1=Mac-ELISA, 2=PCR, 3=Khác | |
| `KETQUAXETNGHIEM` | 0=Dương tính, 1=Âm tính, 2=Chưa có KQ | |
| `TINHTRANGHIENNAY` | 0=Ngoại trú, 1=Nội trú, 2=Ra viện, 3=Tử vong, 4=Chuyển viện, 5=Khác | |
| `LOAIPHATHIEN` | 0=Trạm YT, 1=Tại nhà, 2=Y tế cơ quan, 3=Khác | |
| `TINHTRANGRAVIEN` | (chưa có trong tài liệu) | ⚠ TODO |

> `SOLANSUDUNG` = số đếm (không phải enum). Các trường có **0=Có** (SUDUNGVACXIN, LAYMAUXETNGHIEM) BẮT BUỘC dùng qua enum — KHÔNG hardcode 0/1.

### 15.2 EnumEcds.cs

```csharp
namespace HIS.Desktop.Plugins.InfectiousDiseaseReport
{
    /// <summary>Giới tính — GIOITINH.</summary>
    public enum EcdsGioiTinh { Nu = 0, Nam = 1 }

    /// <summary>Tình trạng mang thai — IS_MANGTHAI.</summary>
    public enum EcdsMangThai { Khong = 0, Co = 1 }

    /// <summary>Tiêm/uống vắc xin — SUDUNGVACXIN. ⚠ 0 = Có (polarity đảo).</summary>
    public enum EcdsSuDungVacXin { Co = 0, Khong = 1, KhongRo = 2 }

    /// <summary>Phân loại chẩn đoán — PHANLOAICHUANDOAN.</summary>
    public enum EcdsPhanLoaiChuanDoan { NghiNgo = 0, XacDinh = 1 }

    /// <summary>Có lấy mẫu XN — LAYMAUXETNGHIEM. ⚠ 0 = Có (polarity đảo).</summary>
    public enum EcdsLayMauXetNghiem { Co = 0, Khong = 1 }

    /// <summary>Loại xét nghiệm — LOAIXETNGHIEM.</summary>
    public enum EcdsLoaiXetNghiem { TestNhanh = 0, MacElisa = 1, Pcr = 2, Khac = 3 }

    /// <summary>Kết quả xét nghiệm — KETQUAXETNGHIEM.</summary>
    public enum EcdsKetQuaXetNghiem { DuongTinh = 0, AmTinh = 1, ChuaCoKetQua = 2 }

    /// <summary>Tình trạng hiện nay — TINHTRANGHIENNAY.</summary>
    public enum EcdsTinhTrangHienNay
    { NgoaiTru = 0, NoiTru = 1, RaVien = 2, TuVong = 3, ChuyenVien = 4, Khac = 5 }

    /// <summary>Loại cơ sở phát hiện/điều trị — LOAIPHATHIEN.</summary>
    public enum EcdsLoaiPhatHien { TramYTe = 0, TaiNha = 1, YTeCoQuan = 2, Khac = 3 }
}
```

---

## 16. Nguồn Danh Mục HIS → ECDS (CHỐT)

Danh mục ECDS là **ID số**; giá trị nguồn lấy từ **thông tin bệnh nhân + danh mục HIS hiện có**, rồi Mapper đổi sang **ID ECDS** qua `/danh-muc/*`.

| Trường ECDS | Danh mục ECDS | Nguồn giá trị HIS | Danh mục HIS | Cách map |
|-------------|---------------|-------------------|--------------|----------|
| `NGHENGHIEP_ID` | `nghenghiep` | nghề nghiệp bệnh nhân | **`HIS_CAREER`** | code HIS → ID ECDS |
| `DANTOC_ID` | `dantoc` | dân tộc bệnh nhân | **`SDA_NATIONAL`** | code HIS → ID ECDS |
| `TINH_ID` / `XA_ID` | `tinh` / `xa` | `HT_COMMUNE_CODE` (hiện nay) | `V_SDA_COMMUNE` (GSO) | GSO → ID ECDS |
| `TINH_ID_THUONGTRU` / `XA_ID_THUONGTRU` | `tinh` / `xa` | `COMMUNE_CODE` (thường trú) | `V_SDA_COMMUNE` | GSO → ID ECDS |
| `THON_ID` | `thon` | — (HIS không có cấp thôn) | — | nhập tay / để trống |
| `BENHCHUANDOAN_ID` | `benhchuandoan` | ICD điều trị | HIS ICD | ICD → ID ECDS |
| `DM_CAPDOBENH_ID` | `capdobenh` | phân độ theo bệnh | **danh mục liên thông** (mã liên thông BYT) | mã liên thông → ID ECDS (cascade theo bệnh) |
| `DONVITHUCHIENXN` / `BENHVIENCHUYENTOI_ID` | `coso` | mã cơ sở | `HIS_BRANCH` / danh mục cơ sở | code → ID ECDS |
| `CO_SO_DIEU_TRI` *(string)* | — (tên, không phải ID) | **`HIS_BRANCH.BRANCH_NAME`** | `HIS_BRANCH` | dùng thẳng tên |
| `GIOITINH` | (enum) | `GENDER_ID` | `HIS_GENDER` | map ID→enum (0/1) |

```csharp
// Nạp danh mục HIS (cache RAM)
var careers  = BackendDataWorker.Get<HIS_CAREER>();                 // nghề nghiệp
var nations  = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>(); // dân tộc
var communes = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList();
var branch   = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == branchId);
// CO_SO_DIEU_TRI = branch?.BRANCH_NAME;
```

---

## 17. Bảng Lưu Trữ (chỉ trường ECDS chưa có ở HIS)

Trường đã có ở HIS (họ tên, ngày sinh, giới tính, dân tộc, nghề nghiệp, địa chỉ, ICD, ngày nhập/ra viện) → đọc trực tiếp lúc đẩy, **không nhân bản**. Chỉ lưu trường ECDS đặc thù + đối soát.

### 17.1 `HIS_ECDS_DISEASE_CASE`

**Hệ thống & liên kết:** `ID, CREATE_TIME, MODIFY_TIME, CREATOR, MODIFIER, APP_CREATOR, APP_MODIFIER, IS_ACTIVE, IS_DELETE, GROUP_CODE, TREATMENT_ID`
(Không lưu `PATIENT_ID` — lấy qua view `V_HIS_ECDS_DISEASE_CASE` từ `HIS_TREATMENT.TDL_PATIENT_ID`.)

**Đối soát cổng (cốt lõi):**
| Cột | Kiểu | Ý nghĩa |
|-----|------|---------|
| `ECDS_CASE_ID` | VARCHAR2(50) | ID ca bệnh trên cổng (update tránh trùng) |
| `ECDS_CASE_CODE` | VARCHAR2(50) | Mã ca bệnh cổng trả về |
| `PUSH_STATE` | NUMBER(1) | 0=chưa đẩy, 1=đã đẩy, 2=lỗi |
| `LAST_PUSH_TIME` | NUMBER(14) | Lần đẩy gần nhất |
| `PUSH_MESSAGE` | VARCHAR2(2000) | Thông điệp/lỗi cổng |

**Nghiệp vụ ECDS (theo spec chính thức):**
| Cột | Kiểu | ECDS | Enum/Danh mục |
|-----|------|------|---------------|
| `REPORTED_ICD_CODE` / `REPORTED_DISEASE_ID` | VARCHAR2/NUMBER | `BENHCHUANDOAN_ID` | `benhchuandoan` |
| `DISEASE_SEVERITY_ID` | NUMBER | `DM_CAPDOBENH_ID` | `capdobenh` (liên thông) |
| `DIAGNOSIS_TYPE` | NUMBER(1) | `PHANLOAICHUANDOAN` | `EcdsPhanLoaiChuanDoan` |
| `CURRENT_STATE` | NUMBER | `TINHTRANGHIENNAY` | `EcdsTinhTrangHienNay` |
| `OTHER_STATE_DESC` | VARCHAR2(500) | `TINHTRANGKHAC` | |
| `ONSET_DATE` | NUMBER(14) | `NGAYKHOIPHAT` | |
| `VACCINE_USE` | NUMBER(1) | `SUDUNGVACXIN` | `EcdsSuDungVacXin` (0=Có) |
| `VACCINE_USE_COUNT` | NUMBER | `SOLANSUDUNG` | |
| `IS_SPECIMEN_TAKEN` | NUMBER(1) | `LAYMAUXETNGHIEM` | `EcdsLayMauXetNghiem` (0=Có) |
| `TEST_TYPE` | NUMBER | `LOAIXETNGHIEM` | `EcdsLoaiXetNghiem` |
| `OTHER_TEST_NAME` | VARCHAR2(255) | `LOAIXETNGHIEMKHAC` | |
| `TEST_RESULT` | NUMBER | `KETQUAXETNGHIEM` | `EcdsKetQuaXetNghiem` |
| `TEST_TIME` | NUMBER(14) | `NGAYTHUCHIENXN` | |
| `RESULT_TIME` | NUMBER(14) | `NGAYTRAKETQUAXN` | |
| `TEST_FACILITY_ID` | NUMBER | `DONVITHUCHIENXN` | `coso` |
| `DETECTION_FACILITY_TYPE` | NUMBER | `LOAIPHATHIEN` | `EcdsLoaiPhatHien` |
| `TREATMENT_FACILITY_NAME` | VARCHAR2(500) | `CO_SO_DIEU_TRI` | `HIS_BRANCH.BRANCH_NAME` |
| `TRANSFER_HOSPITAL_NAME` | VARCHAR2(500) | `BENHVIENCHUYENTOI` | |
| `TRANSFER_HOSPITAL_ID` | NUMBER | `BENHVIENCHUYENTOI_ID` | `coso` |
| `DISCHARGE_STATE` | NUMBER | `TINHTRANGRAVIEN` | ⚠ chờ enum |
| `DEATH_DATE` | NUMBER(14) | `NGAYTUVONG` | |
| `SUB_DIAGNOSIS` | VARCHAR2(2000) | `BENHCHUANDOANPHU` | |
| `COMPLICATION` | VARCHAR2(2000) | `CHUANDOANBIENCHUNG` | |
| `DISCHARGE_DIAGNOSIS` | VARCHAR2(2000) | `CHAN_DOAN_RA_VIEN` | |
| `EPIDEMIOLOGY_HISTORY` | VARCHAR2(2000) | `TIEN_SU_DICH_TE` | |
| `GENERAL_NOTE` | VARCHAR2(2000) | `GHICHU` | |
| `WORKPLACE` | VARCHAR2(500) | `NOILAMVIEC` | |
| `IS_PREGNANT` | NUMBER(1) | `IS_MANGTHAI` | `EcdsMangThai` (0=Không,1=Có) — không có nguồn HIS, lưu để đẩy lại |
| `VILLAGE_ID` | NUMBER | `THON_ID` | `thon` (nhập tay) |
| `REPORTER_NAME` | VARCHAR2(255) | `NGUOIBAOCAO` | |
| `REPORTER_EMAIL` | VARCHAR2(100) | `EMAILNGUOIBAOCAO` | |
| `REPORTER_PHONE` | VARCHAR2(20) | `DIENTHOAINGUOIBAOCAO` | |

> Các cột `*_ID` lưu **ID ECDS đã map**; có thể kèm `*_NAME` snapshot để hiển thị ở màn danh sách mà không gọi lại cổng.

### 17.2 Bảng con (mảng)

`HIS_ECDS_MALARIA_MEDICINE` — thuốc sốt rét (⚠ sub-schema chờ tài liệu):
| Cột | Kiểu | Ý nghĩa |
|-----|------|---------|
| `ECDS_DISEASE_CASE_ID` | NUMBER(18) NN | FK → HIS_ECDS_DISEASE_CASE |
| `MEDICINE_CODE` | VARCHAR2(50) | Mã thuốc (`/danh-muc/thuoc-sot-ret`) |
| `MEDICINE_NAME` | VARCHAR2(500) | Tên thuốc (snapshot) |
| `QUANTITY` | NUMBER(12,2) | Số lượng/liều |
| `UNIT_CODE` | VARCHAR2(50) | Đơn vị tính |
| `DAY_COUNT` | NUMBER | Số ngày dùng |
| `NOTE` | VARCHAR2(500) | Ghi chú |

`HIS_ECDS_TRAVEL_HISTORY` — lịch sử di chuyển dịch tễ (⚠ sub-schema chờ tài liệu):
| Cột | Kiểu | Ý nghĩa |
|-----|------|---------|
| `ECDS_DISEASE_CASE_ID` | NUMBER(18) NN | FK → HIS_ECDS_DISEASE_CASE |
| `FROM_DATE` | NUMBER(14) | Từ ngày |
| `TO_DATE` | NUMBER(14) | Đến ngày |
| `LOCATION_COMMUNE_CODE` | VARCHAR2(20) | Mã xã nơi đến (GSO) |
| `LOCATION_NAME` | VARCHAR2(500) | Địa danh (snapshot) |
| `NOTE` | VARCHAR2(500) | Ghi chú |

Cả hai FK `ECDS_DISEASE_CASE_ID` → `HIS_ECDS_DISEASE_CASE`, kèm 11 cột audit chuẩn.

### 17.3 SQL tạo bảng (Oracle)

```sql
-- ============================================================
-- Bảng chính: HIS_ECDS_DISEASE_CASE
-- ============================================================
CREATE TABLE HIS_ECDS_DISEASE_CASE (
    ID                        NUMBER(19)    NOT NULL,
    CREATE_TIME               NUMBER(14),
    MODIFY_TIME               NUMBER(14),
    CREATOR                   VARCHAR2(50),
    MODIFIER                  VARCHAR2(50),
    APP_CREATOR               VARCHAR2(50),
    APP_MODIFIER              VARCHAR2(50),
    IS_ACTIVE                 NUMBER(1)     DEFAULT 1,
    IS_DELETE                 NUMBER(1)     DEFAULT 0,
    GROUP_CODE                VARCHAR2(50),
    -- Liên kết
    TREATMENT_ID              NUMBER(19)    NOT NULL,
    -- Đối soát cổng
    ECDS_CASE_ID              VARCHAR2(50),
    ECDS_CASE_CODE            VARCHAR2(50),
    PUSH_STATE                NUMBER(1)     DEFAULT 0,   -- 0=chưa đẩy,1=đã đẩy,2=lỗi
    LAST_PUSH_TIME            NUMBER(14),
    PUSH_MESSAGE              VARCHAR2(2000),
    -- Nghiệp vụ ECDS
    REPORTED_DISEASE_ID       NUMBER,                    -- BENHCHUANDOAN_ID
    REPORTED_ICD_CODE         VARCHAR2(10),
    DISEASE_SEVERITY_ID       NUMBER,                    -- DM_CAPDOBENH_ID (liên thông)
    DIAGNOSIS_TYPE            NUMBER(1),                 -- PHANLOAICHUANDOAN
    CURRENT_STATE             NUMBER,                    -- TINHTRANGHIENNAY
    OTHER_STATE_DESC          VARCHAR2(500),             -- TINHTRANGKHAC
    ONSET_DATE                NUMBER(14),                -- NGAYKHOIPHAT
    VACCINE_USE               NUMBER(1),                 -- SUDUNGVACXIN (0=Có)
    VACCINE_USE_COUNT         NUMBER,                    -- SOLANSUDUNG
    IS_SPECIMEN_TAKEN         NUMBER(1),                 -- LAYMAUXETNGHIEM (0=Có)
    TEST_TYPE                 NUMBER,                    -- LOAIXETNGHIEM
    OTHER_TEST_NAME           VARCHAR2(255),             -- LOAIXETNGHIEMKHAC
    TEST_RESULT               NUMBER,                    -- KETQUAXETNGHIEM
    TEST_TIME                 NUMBER(14),                -- NGAYTHUCHIENXN
    RESULT_TIME               NUMBER(14),                -- NGAYTRAKETQUAXN
    TEST_FACILITY_ID          NUMBER,                    -- DONVITHUCHIENXN (coso)
    DETECTION_FACILITY_TYPE   NUMBER,                    -- LOAIPHATHIEN
    TREATMENT_FACILITY_NAME   VARCHAR2(500),             -- CO_SO_DIEU_TRI (BRANCH_NAME)
    TRANSFER_HOSPITAL_NAME    VARCHAR2(500),             -- BENHVIENCHUYENTOI
    TRANSFER_HOSPITAL_ID      NUMBER,                    -- BENHVIENCHUYENTOI_ID (coso)
    DISCHARGE_STATE           NUMBER,                    -- TINHTRANGRAVIEN
    DEATH_DATE                NUMBER(14),                -- NGAYTUVONG
    SUB_DIAGNOSIS             VARCHAR2(2000),            -- BENHCHUANDOANPHU
    COMPLICATION              VARCHAR2(2000),            -- CHUANDOANBIENCHUNG
    DISCHARGE_DIAGNOSIS       VARCHAR2(2000),            -- CHAN_DOAN_RA_VIEN
    EPIDEMIOLOGY_HISTORY      VARCHAR2(2000),            -- TIEN_SU_DICH_TE
    GENERAL_NOTE              VARCHAR2(2000),            -- GHICHU
    WORKPLACE                 VARCHAR2(500),             -- NOILAMVIEC
    IS_PREGNANT               NUMBER(1),                 -- IS_MANGTHAI (0=Không,1=Có)
    VILLAGE_ID                NUMBER,                    -- THON_ID (thon)
    REPORTER_NAME             VARCHAR2(255),             -- NGUOIBAOCAO
    REPORTER_EMAIL            VARCHAR2(100),             -- EMAILNGUOIBAOCAO
    REPORTER_PHONE            VARCHAR2(20),              -- DIENTHOAINGUOIBAOCAO
    CONSTRAINT PK_HIS_ECDS_DISEASE_CASE PRIMARY KEY (ID)
);

CREATE SEQUENCE SEQ_HIS_ECDS_DISEASE_CASE START WITH 1 INCREMENT BY 1 NOCACHE;

CREATE INDEX IDX_ECDS_CASE_TREATMENT ON HIS_ECDS_DISEASE_CASE (TREATMENT_ID);
CREATE INDEX IDX_ECDS_CASE_PUSHSTATE ON HIS_ECDS_DISEASE_CASE (PUSH_STATE, IS_ACTIVE);
CREATE INDEX IDX_ECDS_CASE_CODE      ON HIS_ECDS_DISEASE_CASE (ECDS_CASE_CODE);

COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.TREATMENT_ID           IS 'Khóa ngoại điều trị (HIS_TREATMENT.ID)';
-- Đối soát cổng
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.ECDS_CASE_ID           IS 'ID ca bệnh trên cổng ECDS (dùng cập nhật tránh trùng)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.ECDS_CASE_CODE         IS 'Mã ca bệnh cổng ECDS trả về';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.PUSH_STATE             IS 'Trạng thái đẩy: 0=chưa đẩy, 1=đã đẩy, 2=lỗi';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.LAST_PUSH_TIME         IS 'Thời điểm đẩy gần nhất (yyyyMMddHHmmss)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.PUSH_MESSAGE           IS 'Thông điệp/lỗi cổng trả về (thongDiep/maLoi)';
-- Nghiệp vụ ECDS
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.REPORTED_DISEASE_ID    IS 'ID bệnh chẩn đoán chính phía ECDS (BENHCHUANDOAN_ID)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.REPORTED_ICD_CODE      IS 'Mã ICD-10 báo cáo (dùng đối chiếu)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.DISEASE_SEVERITY_ID    IS 'Cấp độ/phân độ bệnh (DM_CAPDOBENH_ID, danh mục liên thông)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.DIAGNOSIS_TYPE         IS 'Phân loại chẩn đoán (PHANLOAICHUANDOAN): 0=Nghi ngờ, 1=Xác định';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.CURRENT_STATE          IS 'Tình trạng hiện nay (TINHTRANGHIENNAY): 0=Ngoại trú,1=Nội trú,2=Ra viện,3=Tử vong,4=Chuyển viện,5=Khác';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.OTHER_STATE_DESC       IS 'Mô tả tình trạng khác (TINHTRANGKHAC)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.ONSET_DATE             IS 'Ngày khởi phát (NGAYKHOIPHAT)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.VACCINE_USE            IS 'Tiêm/uống vắc xin (SUDUNGVACXIN) - LƯU Ý 0=Có, 1=Không, 2=Không rõ';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.VACCINE_USE_COUNT      IS 'Số lần sử dụng vắc xin (SOLANSUDUNG)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.IS_SPECIMEN_TAKEN      IS 'Có lấy mẫu XN (LAYMAUXETNGHIEM) - LƯU Ý 0=Có, 1=Không';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.TEST_TYPE             IS 'Loại xét nghiệm (LOAIXETNGHIEM): 0=Test nhanh,1=Mac-ELISA,2=PCR,3=Khác';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.OTHER_TEST_NAME        IS 'Tên loại XN khác (LOAIXETNGHIEMKHAC)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.TEST_RESULT           IS 'Kết quả XN (KETQUAXETNGHIEM): 0=Dương tính,1=Âm tính,2=Chưa có KQ';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.TEST_TIME             IS 'Ngày thực hiện XN (NGAYTHUCHIENXN)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.RESULT_TIME           IS 'Ngày trả kết quả XN (NGAYTRAKETQUAXN)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.TEST_FACILITY_ID       IS 'Đơn vị thực hiện XN (DONVITHUCHIENXN, danh mục coso ECDS)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.DETECTION_FACILITY_TYPE IS 'Loại cơ sở phát hiện/điều trị (LOAIPHATHIEN): 0=Trạm YT,1=Tại nhà,2=Y tế cơ quan,3=Khác';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.TREATMENT_FACILITY_NAME IS 'Tên cơ sở điều trị (CO_SO_DIEU_TRI = HIS_BRANCH.BRANCH_NAME)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.TRANSFER_HOSPITAL_NAME IS 'Tên bệnh viện chuyển tới (BENHVIENCHUYENTOI)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.TRANSFER_HOSPITAL_ID   IS 'ID bệnh viện chuyển tới (BENHVIENCHUYENTOI_ID, danh mục coso)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.DISCHARGE_STATE        IS 'Tình trạng ra viện (TINHTRANGRAVIEN) - chờ enum ECDS';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.DEATH_DATE            IS 'Ngày tử vong (NGAYTUVONG)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.SUB_DIAGNOSIS         IS 'Bệnh chẩn đoán phụ (BENHCHUANDOANPHU)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.COMPLICATION          IS 'Chẩn đoán biến chứng (CHUANDOANBIENCHUNG)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.DISCHARGE_DIAGNOSIS   IS 'Chẩn đoán ra viện (CHAN_DOAN_RA_VIEN)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.EPIDEMIOLOGY_HISTORY  IS 'Tiền sử dịch tễ (TIEN_SU_DICH_TE)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.GENERAL_NOTE          IS 'Ghi chú bổ sung (GHICHU)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.WORKPLACE            IS 'Nơi làm việc/học tập (NOILAMVIEC)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.IS_PREGNANT          IS 'Tình trạng mang thai (IS_MANGTHAI): 0=Không, 1=Có';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.VILLAGE_ID           IS 'ID thôn ECDS (THON_ID) - đã đổi tên cột từ THON_ID';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.REPORTER_NAME         IS 'Tên người báo cáo (NGUOIBAOCAO)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.REPORTER_EMAIL        IS 'Email người báo cáo (EMAILNGUOIBAOCAO)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.REPORTER_PHONE        IS 'SĐT người báo cáo (DIENTHOAINGUOIBAOCAO)';


-- ============================================================
-- Bảng con: HIS_ECDS_MALARIA_MEDICINE (thuốc sốt rét)
-- ============================================================
CREATE TABLE HIS_ECDS_MALARIA_MEDICINE (
    ID                        NUMBER(19)    NOT NULL,
    CREATE_TIME               NUMBER(14),
    MODIFY_TIME               NUMBER(14),
    CREATOR                   VARCHAR2(50),
    MODIFIER                  VARCHAR2(50),
    APP_CREATOR               VARCHAR2(50),
    APP_MODIFIER              VARCHAR2(50),
    IS_ACTIVE                 NUMBER(1)     DEFAULT 1,
    IS_DELETE                 NUMBER(1)     DEFAULT 0,
    GROUP_CODE                VARCHAR2(50),
    ECDS_DISEASE_CASE_ID      NUMBER(19)    NOT NULL,
    MEDICINE_CODE             VARCHAR2(50),
    MEDICINE_NAME             VARCHAR2(500),
    QUANTITY                  NUMBER(12,2),
    UNIT_CODE                 VARCHAR2(50),
    DAY_COUNT                 NUMBER,
    NOTE                      VARCHAR2(500),
    CONSTRAINT PK_HIS_ECDS_MALARIA_MED PRIMARY KEY (ID),
    CONSTRAINT FK_ECDS_MALARIA_MED_CASE FOREIGN KEY (ECDS_DISEASE_CASE_ID)
        REFERENCES HIS_ECDS_DISEASE_CASE (ID)
);

CREATE SEQUENCE SEQ_HIS_ECDS_MALARIA_MED START WITH 1 INCREMENT BY 1 NOCACHE;
CREATE INDEX IDX_ECDS_MALARIA_MED_CASE ON HIS_ECDS_MALARIA_MEDICINE (ECDS_DISEASE_CASE_ID);


COMMENT ON COLUMN HIS_ECDS_MALARIA_MEDICINE.ECDS_DISEASE_CASE_ID IS 'Khóa ngoại ca bệnh (HIS_ECDS_DISEASE_CASE.ID)';
COMMENT ON COLUMN HIS_ECDS_MALARIA_MEDICINE.MEDICINE_CODE        IS 'Mã thuốc sốt rét (danh mục thuoc-sot-ret ECDS)';
COMMENT ON COLUMN HIS_ECDS_MALARIA_MEDICINE.MEDICINE_NAME        IS 'Tên thuốc (snapshot hiển thị)';
COMMENT ON COLUMN HIS_ECDS_MALARIA_MEDICINE.QUANTITY             IS 'Số lượng/liều dùng';
COMMENT ON COLUMN HIS_ECDS_MALARIA_MEDICINE.UNIT_CODE            IS 'Đơn vị tính (danh mục don-vi-tinh)';
COMMENT ON COLUMN HIS_ECDS_MALARIA_MEDICINE.DAY_COUNT            IS 'Số ngày dùng';
COMMENT ON COLUMN HIS_ECDS_MALARIA_MEDICINE.NOTE                 IS 'Ghi chú';


-- ============================================================
-- Bảng con: HIS_ECDS_TRAVEL_HISTORY (lịch sử di chuyển dịch tễ)
-- ============================================================
CREATE TABLE HIS_ECDS_TRAVEL_HISTORY (
    ID                        NUMBER(19)    NOT NULL,
    CREATE_TIME               NUMBER(14),
    MODIFY_TIME               NUMBER(14),
    CREATOR                   VARCHAR2(50),
    MODIFIER                  VARCHAR2(50),
    APP_CREATOR               VARCHAR2(50),
    APP_MODIFIER              VARCHAR2(50),
    IS_ACTIVE                 NUMBER(1)     DEFAULT 1,
    IS_DELETE                 NUMBER(1)     DEFAULT 0,
    GROUP_CODE                VARCHAR2(50),
    ECDS_DISEASE_CASE_ID      NUMBER(19)    NOT NULL,
    FROM_DATE                 NUMBER(14),
    TO_DATE                   NUMBER(14),
    LOCATION_COMMUNE_CODE     VARCHAR2(20),
    LOCATION_NAME             VARCHAR2(500),
    NOTE                      VARCHAR2(500),
    CONSTRAINT PK_HIS_ECDS_TRAVEL_HIS PRIMARY KEY (ID),
    CONSTRAINT FK_ECDS_TRAVEL_HIS_CASE FOREIGN KEY (ECDS_DISEASE_CASE_ID)
        REFERENCES HIS_ECDS_DISEASE_CASE (ID)
);

CREATE SEQUENCE SEQ_HIS_ECDS_TRAVEL_HIS START WITH 1 INCREMENT BY 1 NOCACHE;
CREATE INDEX IDX_ECDS_TRAVEL_HIS_CASE ON HIS_ECDS_TRAVEL_HISTORY (ECDS_DISEASE_CASE_ID);

COMMENT ON COLUMN HIS_ECDS_TRAVEL_HISTORY.ECDS_DISEASE_CASE_ID  IS 'Khóa ngoại ca bệnh (HIS_ECDS_DISEASE_CASE.ID)';
COMMENT ON COLUMN HIS_ECDS_TRAVEL_HISTORY.FROM_DATE             IS 'Từ ngày (yyyyMMddHHmmss)';
COMMENT ON COLUMN HIS_ECDS_TRAVEL_HISTORY.TO_DATE               IS 'Đến ngày (yyyyMMddHHmmss)';
COMMENT ON COLUMN HIS_ECDS_TRAVEL_HISTORY.LOCATION_COMMUNE_CODE IS 'Mã xã nơi đến (GSO)';
COMMENT ON COLUMN HIS_ECDS_TRAVEL_HISTORY.LOCATION_NAME         IS 'Địa danh (snapshot hiển thị)';
COMMENT ON COLUMN HIS_ECDS_TRAVEL_HISTORY.NOTE                  IS 'Ghi chú';


-- ============================================================
-- View: V_HIS_ECDS_DISEASE_CASE
-- Ca bệnh ECDS kèm thông tin bệnh nhân/điều trị lấy từ HIS_TREATMENT.
-- Dùng cho màn danh sách (hiển thị BN, mã ĐT, ICD... không cần join thủ công).
-- ============================================================
CREATE OR REPLACE VIEW V_HIS_ECDS_DISEASE_CASE AS
SELECT
    e.*,
    t.TREATMENT_CODE          AS TREATMENT_CODE,        -- Mã điều trị
    t.PATIENT_ID              AS PATIENT_ID,            -- Mã BN (HIS_PATIENT.ID)
    t.TDL_PATIENT_CODE        AS PATIENT_CODE,          -- Mã bệnh nhân
    t.TDL_PATIENT_NAME        AS PATIENT_NAME,          -- Họ tên bệnh nhân
    t.TDL_PATIENT_DOB         AS PATIENT_DOB,           -- Ngày sinh (yyyyMMddHHmmss)
    t.TDL_PATIENT_GENDER_ID   AS PATIENT_GENDER_ID,     -- ID giới tính (HIS_GENDER)
    t.TDL_PATIENT_GENDER_NAME AS PATIENT_GENDER_NAME,   -- Tên giới tính
    t.ICD_CODE                AS TREATMENT_ICD_CODE,     -- Mã ICD điều trị
    t.ICD_NAME                AS TREATMENT_ICD_NAME,     -- Tên ICD điều trị
    t.IN_TIME                 AS TREATMENT_IN_TIME,      -- Thời gian vào (yyyyMMddHHmmss)
    t.OUT_TIME                AS TREATMENT_OUT_TIME,     -- Thời gian ra (yyyyMMddHHmmss)
    t.LAST_DEPARTMENT_ID      AS TREATMENT_DEPARTMENT_ID, -- Khoa hiện tại (lọc màn danh sách)
    -- Định danh & nhân khẩu bệnh nhân (V_HIS_PATIENT)
    p.FIRST_NAME              AS PATIENT_FIRST_NAME,     -- Tên
    p.LAST_NAME               AS PATIENT_LAST_NAME,      -- Họ + tên đệm
    p.IS_HAS_NOT_DAY_DOB      AS PATIENT_NO_DAY_DOB,     -- 1 = chỉ có năm sinh
    p.GENDER_CODE             AS PATIENT_GENDER_CODE,    -- Mã giới tính
    -- Giấy tờ tùy thân
    p.CCCD_NUMBER             AS PATIENT_CCCD,           -- -> CCCD
    p.CCCD_DATE               AS PATIENT_CCCD_DATE,      -- Ngày cấp CCCD (yyyyMMddHHmmss)
    p.CCCD_PLACE              AS PATIENT_CCCD_PLACE,     -- Nơi cấp CCCD
    p.PHONE                   AS PATIENT_PHONE,          -- -> DIENTHOAI
    -- Dân tộc / nghề nghiệp
    p.ETHNIC_CODE             AS PATIENT_ETHNIC_CODE,    -- -> DANTOC_ID (danh mục dantoc)
    p.ETHNIC_NAME             AS PATIENT_ETHNIC_NAME,
    p.CAREER_CODE             AS PATIENT_CAREER_CODE,    -- -> NGHENGHIEP_ID (danh mục nghenghiep)
    p.CAREER_NAME             AS PATIENT_CAREER_NAME,
    p.WORK_PLACE              AS PATIENT_WORKPLACE,       -- -> NOILAMVIEC (⚠ xác minh tên cột V_HIS_PATIENT)
    -- Địa chỉ HIỆN NAY (HT_*) -> TINH_ID / XA_ID / DIACHI
    p.HT_PROVINCE_CODE        AS CUR_PROVINCE_CODE,      -- GSO tỉnh hiện nay -> TINH_ID
    p.HT_PROVINCE_NAME        AS CUR_PROVINCE_NAME,
    p.HT_DISTRICT_CODE        AS CUR_DISTRICT_CODE,      -- Huyện (tham khảo)
    p.HT_DISTRICT_NAME        AS CUR_DISTRICT_NAME,
    p.HT_COMMUNE_CODE         AS CUR_COMMUNE_CODE,       -- GSO xã hiện nay -> XA_ID
    p.HT_COMMUNE_NAME         AS CUR_COMMUNE_NAME,
    p.HT_ADDRESS              AS CUR_ADDRESS,            -- -> DIACHI (số nhà/đường)
    p.VIR_HT_ADDRESS          AS CUR_FULL_ADDRESS,       -- Địa chỉ hiện nay đầy đủ (hiển thị)
    -- Địa chỉ THƯỜNG TRÚ -> *_THUONGTRU
    p.PROVINCE_CODE           AS PERM_PROVINCE_CODE,     -- GSO tỉnh thường trú
    p.PROVINCE_NAME           AS PERM_PROVINCE_NAME,
    p.DISTRICT_CODE           AS PERM_DISTRICT_CODE,     -- Huyện (tham khảo)
    p.DISTRICT_NAME           AS PERM_DISTRICT_NAME,
    p.COMMUNE_CODE            AS PERM_COMMUNE_CODE,      -- GSO xã thường trú
    p.COMMUNE_NAME            AS PERM_COMMUNE_NAME,
    p.ADDRESS                 AS PERM_ADDRESS,           -- -> DIACHI_THUONGTRU (số nhà/đường)
    p.VIR_ADDRESS             AS PERM_FULL_ADDRESS       -- Địa chỉ thường trú đầy đủ (hiển thị)
FROM HIS_ECDS_DISEASE_CASE e
JOIN HIS_TREATMENT t ON t.ID = e.TREATMENT_ID
JOIN V_HIS_PATIENT p ON p.ID = t.PATIENT_ID;

COMMENT ON TABLE V_HIS_ECDS_DISEASE_CASE IS 'View ca bệnh ECDS kèm thông tin BN/điều trị (HIS_TREATMENT + V_HIS_PATIENT)';
```

> Ghi chú: HIS dùng Oracle — DateTime lưu `NUMBER(14)` (`yyyyMMddHHmmss`), boolean/enum lưu `NUMBER`. ID cấp từ SEQUENCE tương ứng. FK có thể bỏ nếu quy ước dự án dùng ràng buộc mềm (soft) như các bảng HIS khác.

---

## 19. Phân Tích Luồng Frontend (theo code)

Tầng frontend nằm ở `HIS/Plugins/HIS.Desktop.Plugins.InfectiousDiseaseReport/`.

### 19.1 Bản đồ file → vai trò

| Lớp | File | Vai trò |
|-----|------|---------|
| Entry (MEF) | `InfectiousDiseaseReportProcessor.cs` | `[ExtensionOf]` — nhận `Run(args)` từ HIS |
| Factory | `InfectiousDiseaseReport/InfectiousDiseaseReportFactory.cs` | Tạo Behavior |
| Behavior | `InfectiousDiseaseReport/InfectiousDiseaseReportBehavior.cs` | Parse args → chọn & tạo Form |
| Form (chính) | `MainForm/frmInfectiousDiseaseReport.cs` | Khai báo control, constructor, helper, event |
| Form (UI) | `MainForm/…__BuildUi.cs` | Dựng header + 5 tab + footer bằng code |
| Form (load) | `MainForm/…__Load.cs` | Nạp config, enum combo, danh mục ECDS |
| Form (fill) | `MainForm/…__FillData.cs` | Đổ dữ liệu HIS → header + tab |
| Form (validate) | `MainForm/…__Check.cs` | Kiểm tra trường bắt buộc |
| Form (push) | `MainForm/…__Push.cs` | Build DTO → đẩy → đối soát |
| API | `Worker/EcdsApiWorker.cs` | HTTP tới cổng ECDS |
| Token | `Worker/EcdsTokenStore.cs` | Cache accessToken theo phiên |
| Danh mục | `Worker/EcdsCatalogCache.cs` | Cache danh mục + tra ID theo mã |
| Mapper | `Worker/DiseaseCaseMapper.cs` | Convert date, map mã→ID ECDS |
| Config | `Config/EcdsConfigCFG.cs` | Đọc HisConfigs |
| Enum | `EnumEcds.cs` | Giá trị enum chính thức ECDS |

### 19.2 Luồng khởi tạo (mở Form)

```
HIS gọi mở module
  → InfectiousDiseaseReportProcessor.Run(args)
      → InfectiousDiseaseReportFactory.MakeIControl(param, args)
          → new InfectiousDiseaseReportBehavior(param, args)
      → behavior.Run()
          duyệt args:  Module | HIS_TREATMENT | RefeshReference
          nếu CÓ HIS_TREATMENT → new frmInfectiousDiseaseReport(module, treatment, dlgRefresh)
          nếu KHÔNG            → (TODO) Form danh sách
  → HIS hiển thị Form (ShowDialog/embed)
```

### 19.3 Vòng đời Form (Load order)

```
constructor(module, treatment, dlgRefresh)
  → InitializeComponent()      // shell rỗng (Designer tối giản)
  → BuildUi()                  // dựng toàn bộ control
  → SetIcon()
Form.Load (frmInfectiousDiseaseReport_Load)
  → EcdsConfigCFG.LoadConfig()             // đọc HisConfigs
  → khởi tạo apiWorker / catalogCache / mapper
  → InitEnumCombos()                       // bind combo enum (EnumEcds)
  → InitCatalogCombos()                    // login + nạp danh mục ECDS vào combo
  → FillDataFromHis()                      // đổ dữ liệu điều trị/bệnh nhân
```

| Method | Nhiệm vụ |
|--------|----------|
| `BuildUi` | Header (GroupControl) + `XtraTabControl` 5 tab + footer 5 nút; wire event |
| `InitEnumCombos` | Bind `cboLoaiChanDoan, cboTinhTrang, cboGioiTinh, cboSuDungVacXin, cboLayMau, cboLoaiXN, cboKetQuaXN, cboLoaiPhatHien` từ `EnumEcds` |
| `InitCatalogCombos` | `EnsureLogin` → `GetStatic(benh/dan-toc/nghe-nghiep/tinh/coso)` → `SetupLookup` |
| `FillDataFromHis` | `FillHeader/FillCaBenhTab/FillHanhChinhTab/FillNguoiBaoCaoTab` + `UpdatePushStatusLabel` |

### 19.4 Cấu trúc UI (BuildUi)

```
Form
├─ grpHeader (Dock Top)         Thông tin BN & điều trị (label chỉ đọc) + trạng thái đẩy
├─ pnlBody (Dock Fill)
│   └─ tabMain (XtraTabControl)
│       ├─ tabCaBenh       → lcCaBenh (LayoutControl)
│       ├─ tabHanhChinh    → lcHanhChinh
│       ├─ tabTrieuChung   → lcTrieuChung
│       ├─ tabSotRet       → lcSotRet
│       └─ tabNguoiBaoCao  → lcNguoiBaoCao
└─ pnlFooter (Dock Bottom)  btnGetData · btnCheck · btnPush · btnNew · btnClose
```

- Mỗi tab = 1 `LayoutControl`; mỗi trường thêm bằng `AddRow(group, caption, control)` (label căn phải).
- Nhãn "(*)" đánh dấu trường bắt buộc (validate ở `__Check`).

### 19.5 Luồng đẩy ca bệnh (nút "Đẩy lên cổng")

```
btnPush_Click
  → PushProcess()
      1. EcdsConfigCFG.IsValid()      (chưa cấu hình → cảnh báo, dừng)
      2. ValidateForm(out err)        (thiếu bắt buộc → DXErrorProvider tại control, dừng)
      3. Xác nhận (XtraMessageBox Yes/No)
      4. dto = BuildDtoFromForm()     (đọc control → EcdsDiseaseCaseDto; dto.Id = ecdsCaseId để update)
      5. WaitingManager.Show()
      6. result = apiWorker.DayCaBenh(dto)
             → EnsureLogin() (login nếu token hết hạn)
             → PostRaw("/api/fast/v1/ca-benh/cap-nhat", dto)  [Bearer, TLS1.2, Task.Run]
      7. WaitingManager.Hide()
      8. thanhCong?
           Đúng → lưu ecdsCaseId/ecdsCaseCode → UpdatePushStatusLabel
                  → dlgRefresh() → LogActionSuccess → thông báo "Mã ca bệnh: …"
                  → (TODO) lưu HIS_ECDS_DISEASE_CASE qua BackendAdapter
           Sai  → LogActionFail → thông báo thongDiep lỗi
```

### 19.6 Luồng Danh mục / Token / Cache

```
InitCatalogCombos / FillCaBenhTab
  → EcdsCatalogCache.GetStatic(tenDanhMuc)
       cache HIT → trả ngay
       cache MISS → EcdsApiWorker.LayDanhMuc()
                      → EnsureLogin()
                           token còn hạn (EcdsTokenStore.IsValid) → dùng lại
                           hết hạn → POST /auth/login → EcdsTokenStore.Set()
                      → POST /danh-muc/{ten}
       → lưu cache → trả về
Map mã HIS → ID ECDS:
  EcdsCatalogCache.FindIdByMa(list, maHis)   (VD ICD điều trị → BENHCHUANDOAN_ID)
```

### 19.7 Bảng Event → Handler

| Control | Event | Handler | Hành động |
|---------|-------|---------|-----------|
| `btnGetData` | Click | `btnGetData_Click` | `FillDataFromHis()` |
| `btnCheck` | Click | `btnCheck_Click` | `ValidateForm()` + thông báo hợp lệ |
| `btnPush` | Click | `btnPush_Click` | `PushProcess()` |
| `btnNew` | Click | `btnNew_Click` | reset đối soát + `ClearInputControls()` + fill lại |
| `btnClose` | Click | `btnClose_Click` | `this.Close()` |
| Form | Load | `frmInfectiousDiseaseReport_Load` | khởi tạo + fill |

### 19.8 Xử lý lỗi · Thread · Log

- **Try-catch mọi method**: `LogSystem.Error` (Processor/Factory/Push/API), `LogSystem.Warn` (event UI/init/fill).
- **Thread**: HTTP bọc `Task.Run(...).GetAwaiter().GetResult()` trong `EcdsApiWorker` → tránh deadlock UI; UI có `WaitingManager` khi chờ.
- **Audit**: `LogUtil.LogActionSuccess/Fail("InfectiousDiseaseReport","Push", loginName)`.
- **Bảo mật**: KHÔNG log `PASSWORD`/`accessToken`; token chỉ giữ RAM (`EcdsTokenStore`).
- **Validate**: `DXErrorProvider` hiển thị lỗi ngay tại control; clear khi Mới/valid.

### 19.9 Điểm mở rộng (TODO trong code)

| Vị trí | TODO |
|--------|------|
| `FillHanhChinhTab` | Nạp `V_HIS_PATIENT` (view `V_HIS_ECDS_DISEASE_CASE`) → dân tộc/nghề/CCCD/địa chỉ GSO |
| `PushProcess` | Lưu `HIS_ECDS_DISEASE_CASE` (PUSH_STATE, ECDS_CASE_CODE) qua API backend |
| `BuildTabSotRet` / `BuildDtoFromForm` | Nhóm sốt rét + 2 mảng (thuốc, di chuyển) — chờ enum/sub-schema |
| `InitEnumCombos` | Bind enum nhóm sốt rét khi có tài liệu |
| Behavior | Form danh sách (đẩy hàng loạt/tự động) khi không có `HIS_TREATMENT` |

---

## 20. API Backend MOS — Tạo & Sửa Ca Bệnh (Aggregate Save)

> Tầng backend nằm ở `BACKEND/MOS`. Mục này mô tả **2 API tạo & sửa 1 ca bệnh cùng 2 danh sách con** trong **một lời gọi, một giao dịch nghiệp vụ**. Thiết kế **bám đúng mẫu aggregate có sẵn của MOS** — tham chiếu `MOS.MANAGER.HisAllergyCard` (thẻ dị ứng + danh sách dị nguyên): Manager → `...CreateSDO/...UpdateSDO.Run()` → các `Processor` cha/con → sub‑operation `Create/Update/Truncate` của từng bảng, có `RollbackData()` chuỗi.

### 20.1 Dữ liệu đầu vào & quy tắc tạo/sửa

Một "ca bệnh ECDS" gồm **1 bản ghi cha + 2 danh sách con**:

| Thành phần | Kiểu | Bảng |
|-----------|------|------|
| Ca bệnh (cha) | `HIS_ECDS_DISEASE_CASE` | 1 dòng |
| Thuốc sốt rét | `List<HIS_ECDS_MALARIA_MEDICINE>` | N dòng, FK `ECDS_DISEASE_CASE_ID` |
| Lịch sử di chuyển | `List<HIS_ECDS_TRAVEL_HISTORY>` | N dòng, FK `ECDS_DISEASE_CASE_ID` |

**Quy tắc quyết định — áp dụng cho cả cha lẫn từng dòng con:**

```
ID <= 0 (hoặc null)  → TẠO MỚI (Create)
ID  > 0              → CẬP NHẬT (Update)
Dòng con có trong DB nhưng KHÔNG có trong danh sách gửi lên → XOÁ (Truncate)
```

→ Dùng **2 endpoint** `Create` và `Update` theo đúng mẫu MOS (không gộp). FE quyết định gọi endpoint nào theo `DiseaseCase.ID`; bên trong mỗi endpoint, **danh sách con vẫn tự diff** tạo/sửa/xoá theo ID từng dòng.

### 20.2 SDO đầu vào / kết quả (`MOS.SDO`)

```csharp
/// <summary>Gói dữ liệu ca bệnh ECDS: cha + 2 danh sách con.</summary>
public class HisEcdsDiseaseCaseSDO
{
    public HIS_ECDS_DISEASE_CASE DiseaseCase { get; set; }               // cha
    public List<HIS_ECDS_MALARIA_MEDICINE> MalariaMedicines { get; set; } // con: thuốc sốt rét
    public List<HIS_ECDS_TRAVEL_HISTORY> TravelHistories { get; set; }    // con: lịch sử di chuyển
}

/// <summary>Kết quả trả về sau khi lưu (đọc lại từ DB/view).</summary>
public class HisEcdsDiseaseCaseResultSDO
{
    public V_HIS_ECDS_DISEASE_CASE DiseaseCase { get; set; }
    public List<HIS_ECDS_MALARIA_MEDICINE> MalariaMedicines { get; set; }
    public List<HIS_ECDS_TRAVEL_HISTORY> TravelHistories { get; set; }
}
```

### 20.3 Sơ đồ các lớp (đặt trong `MOS.MANAGER.HisEcdsDiseaseCase`)

```
HisEcdsDiseaseCaseController.Create/Update(ApiParam<HisEcdsDiseaseCaseSDO>)   [MOS.API]
  → HisEcdsDiseaseCaseManager.Create/Update(HisEcdsDiseaseCaseSDO)            [validate TRƯỚC try]
      → SDO/Create/HisEcdsDiseaseCaseCreateSDO.Run(data, ref result)
      → SDO/Update/HisEcdsDiseaseCaseUpdateSDO.Run(data, ref result)
          ├─ SDO/HisEcdsDiseaseCaseSDOCheck.ValidData(data)      ← kiểm tra gói đầu vào
          ├─ DiseaseCaseProcessor      ← cha:  Create / Update(raw)
          ├─ MalariaMedicineProcessor  ← con:  diff Insert/Update/Delete
          └─ TravelHistoryProcessor    ← con:  diff Insert/Update/Delete
```

**Cây thư mục bổ sung** (theo mẫu `HisAllergyCard/SDO`):

```
MOS.MANAGER/HisEcdsDiseaseCase/
├── HisEcdsDiseaseCaseManager.cs        (đã có) → THÊM 2 method Create/Update nhận SDO
└── SDO/
    ├── HisEcdsDiseaseCaseSDOCheck.cs                 ← ValidData(gói đầu vào)
    ├── Create/
    │   ├── HisEcdsDiseaseCaseCreateSDO.cs            ← orchestrator tạo mới
    │   ├── DiseaseCaseProcessor.cs                   ← cha: Create
    │   ├── MalariaMedicineProcessor.cs               ← con: CreateList
    │   └── TravelHistoryProcessor.cs                 ← con: CreateList
    └── Update/
        ├── HisEcdsDiseaseCaseUpdateSDO.cs            ← orchestrator cập nhật
        ├── DiseaseCaseProcessor.cs                   ← cha: Update(raw)
        ├── MalariaMedicineProcessor.cs               ← con: diff Insert/Update/Delete
        └── TravelHistoryProcessor.cs                 ← con: diff Insert/Update/Delete

MOS.SDO/  → HisEcdsDiseaseCaseSDO.cs, HisEcdsDiseaseCaseResultSDO.cs
MOS.API/Controllers/HisEcdsDiseaseCaseController.cs → THÊM action Create/Update (SDO)
```

### 20.4 Controller (`MOS.API`)

```csharp
[HttpPost]
[ActionName("SaveCreate")]              // hoặc đặt Create/Update tùy quy ước route
public ApiResult SaveCreate(ApiParam<HisEcdsDiseaseCaseSDO> param)
{
    try
    {
        ApiResultObject<HisEcdsDiseaseCaseResultSDO> result = new ApiResultObject<HisEcdsDiseaseCaseResultSDO>(null);
        if (param != null)
        {
            HisEcdsDiseaseCaseManager mng = new HisEcdsDiseaseCaseManager(param.CommonParam);
            result = mng.Create(param.ApiData);
        }
        return new ApiResult(result, this.ActionContext);
    }
    catch (Exception ex) { LogSystem.Error(ex); return null; }
}

[HttpPost]
[ActionName("SaveUpdate")]
public ApiResult SaveUpdate(ApiParam<HisEcdsDiseaseCaseSDO> param)
{
    try
    {
        ApiResultObject<HisEcdsDiseaseCaseResultSDO> result = new ApiResultObject<HisEcdsDiseaseCaseResultSDO>(null);
        if (param != null)
        {
            HisEcdsDiseaseCaseManager mng = new HisEcdsDiseaseCaseManager(param.CommonParam);
            result = mng.Update(param.ApiData);
        }
        return new ApiResult(result, this.ActionContext);
    }
    catch (Exception ex) { LogSystem.Error(ex); return null; }
}
```

### 20.5 Manager — VALIDATE TRƯỚC `try` xử lý nghiệp vụ

> Điểm khác biệt theo yêu cầu: **kiểm tra dữ liệu đầu vào (null + `SDOCheck.ValidData`) đặt NGOÀI `try`**; `try` chỉ bao phần gọi orchestrator xử lý nghiệp vụ. Nếu không hợp lệ → trả `PackResult(null, false)` ngay, không đi vào nghiệp vụ.

```csharp
[Logger]
public ApiResultObject<HisEcdsDiseaseCaseResultSDO> Create(HisEcdsDiseaseCaseSDO data)
{
    ApiResultObject<HisEcdsDiseaseCaseResultSDO> result = new ApiResultObject<HisEcdsDiseaseCaseResultSDO>(null);

    // ===== VALIDATE TRƯỚC TRY =====
    if (!IsNotNull(param) || !IsNotNull(data)
        || !new HisEcdsDiseaseCaseSDOCheck(param).ValidData(data))
    {
        return this.PackResult(result.Data, false);
    }

    // ===== TRY XỬ LÝ NGHIỆP VỤ =====
    try
    {
        HisEcdsDiseaseCaseResultSDO resultData = null;
        bool isSuccess = new SDO.Create.HisEcdsDiseaseCaseCreateSDO(param).Run(data, ref resultData);
        result = this.PackResult(resultData, isSuccess);
    }
    catch (Exception ex)
    {
        LogSystem.Error(ex);
        param.HasException = true;
    }
    return result;
}

[Logger]
public ApiResultObject<HisEcdsDiseaseCaseResultSDO> Update(HisEcdsDiseaseCaseSDO data)
{
    ApiResultObject<HisEcdsDiseaseCaseResultSDO> result = new ApiResultObject<HisEcdsDiseaseCaseResultSDO>(null);

    // ===== VALIDATE TRƯỚC TRY =====
    if (!IsNotNull(param) || !IsNotNull(data)
        || !new HisEcdsDiseaseCaseSDOCheck(param).ValidData(data)
        || !IsGreaterThanZero(data.DiseaseCase.ID))   // Update bắt buộc có ID cha
    {
        return this.PackResult(result.Data, false);
    }

    // ===== TRY XỬ LÝ NGHIỆP VỤ =====
    try
    {
        HisEcdsDiseaseCaseResultSDO resultData = null;
        bool isSuccess = new SDO.Update.HisEcdsDiseaseCaseUpdateSDO(param).Run(data, ref resultData);
        result = this.PackResult(resultData, isSuccess);
    }
    catch (Exception ex)
    {
        LogSystem.Error(ex);
        param.HasException = true;
    }
    return result;
}
```

### 20.6 `HisEcdsDiseaseCaseSDOCheck.ValidData` (mẫu `HisAllergyCardSDOCheck`)

```csharp
internal bool ValidData(HisEcdsDiseaseCaseSDO data)
{
    bool valid = true;
    try
    {
        if (data == null) throw new ArgumentNullException("data");
        if (!IsNotNull(data.DiseaseCase)) throw new ArgumentNullException("data.DiseaseCase");
        // 2 danh sách con có thể rỗng (ca không sốt rét / không di chuyển) → chỉ chuẩn hoá null
        if (data.MalariaMedicines == null) data.MalariaMedicines = new List<HIS_ECDS_MALARIA_MEDICINE>();
        if (data.TravelHistories  == null) data.TravelHistories  = new List<HIS_ECDS_TRAVEL_HISTORY>();
    }
    catch (ArgumentNullException ex)
    {
        BugUtil.SetBugCode(param, LibraryBug.Bug.Enum.DuLieuDauVaoKhongHopLe);
        LogSystem.Warn(ex);
        valid = false;
    }
    catch (Exception ex) { LogSystem.Error(ex); param.HasException = true; valid = false; }
    return valid;
}
```

### 20.7 Orchestrator TẠO — `SDO/Create/HisEcdsDiseaseCaseCreateSDO.Run`

```csharp
internal bool Run(HisEcdsDiseaseCaseSDO data, ref HisEcdsDiseaseCaseResultSDO resultData)
{
    bool result = false;
    try
    {
        // 1) CHA: tạo mới → BridgeDAO tự set ID/CREATE_TIME/CREATOR
        if (!this.diseaseCaseProcessor.Run(data.DiseaseCase))
            throw new Exception("diseaseCaseProcessor. Rollback du lieu");

        long caseId = data.DiseaseCase.ID;   // đã có ID sau khi tạo

        // 2) CON: gán FK = caseId rồi CreateList
        if (!this.malariaMedicineProcessor.Run(caseId, data.MalariaMedicines))
            throw new Exception("malariaMedicineProcessor. Rollback du lieu");
        if (!this.travelHistoryProcessor.Run(caseId, data.TravelHistories))
            throw new Exception("travelHistoryProcessor. Rollback du lieu");

        this.PassResult(caseId, ref resultData);
        result = true;
    }
    catch (Exception ex)
    {
        LogSystem.Error(ex);
        param.HasException = true;
        this.Rollback();     // travel → malaria → diseaseCase (ngược thứ tự ghi)
        result = false;
    }
    return result;
}
```

`MalariaMedicineProcessor.Run` (bản TẠO) — gán FK rồi tạo hàng loạt:

```csharp
internal bool Run(long ecdsDiseaseCaseId, List<HIS_ECDS_MALARIA_MEDICINE> items)
{
    bool result = false;
    try
    {
        items = items ?? new List<HIS_ECDS_MALARIA_MEDICINE>();
        items.ForEach(o => o.ECDS_DISEASE_CASE_ID = ecdsDiseaseCaseId);
        if (IsNotNullOrEmpty(items))
        {
            if (!this.hisEcdsMalariaMedicineCreate.CreateList(items))
                throw new Exception("Tao HIS_ECDS_MALARIA_MEDICINE that bai.");
        }
        result = true;   // list rỗng vẫn hợp lệ
    }
    catch (Exception ex) { LogSystem.Error(ex); result = false; }
    return result;
}
```

### 20.8 Orchestrator SỬA — `SDO/Update/HisEcdsDiseaseCaseUpdateSDO.Run`

```csharp
internal bool Run(HisEcdsDiseaseCaseSDO data, ref HisEcdsDiseaseCaseResultSDO resultData)
{
    bool result = false;
    try
    {
        // Xác thực bản ghi cha đang tồn tại + không bị khoá
        HIS_ECDS_DISEASE_CASE raw = null;
        HisEcdsDiseaseCaseCheck checker = new HisEcdsDiseaseCaseCheck(param);
        if (!checker.VerifyId(data.DiseaseCase.ID, ref raw)) throw new Exception("VerifyId cha that bai");
        if (!checker.IsUnLock(raw)) throw new Exception("Ban ghi bi khoa");

        // 1) CHA: cập nhật (giữ before=raw để rollback)
        if (!this.diseaseCaseProcessor.Run(data.DiseaseCase, raw))
            throw new Exception("diseaseCaseProcessor. Rollback du lieu");

        long caseId = data.DiseaseCase.ID;

        // 2) CON: diff Insert/Update/Delete theo ID từng dòng
        if (!this.malariaMedicineProcessor.Run(caseId, data.MalariaMedicines))
            throw new Exception("malariaMedicineProcessor. Rollback du lieu");
        if (!this.travelHistoryProcessor.Run(caseId, data.TravelHistories))
            throw new Exception("travelHistoryProcessor. Rollback du lieu");

        this.PassResult(caseId, ref resultData);
        result = true;
    }
    catch (Exception ex)
    {
        LogSystem.Error(ex);
        param.HasException = true;
        this.Rollback();
        result = false;
    }
    return result;
}
```

`MalariaMedicineProcessor.Run` (bản SỬA) — **diff 3 nhóm** (mẫu `AllergenicProcessor`):

```csharp
internal bool Run(long ecdsDiseaseCaseId, List<HIS_ECDS_MALARIA_MEDICINE> items)
{
    bool result = false;
    try
    {
        items = items ?? new List<HIS_ECDS_MALARIA_MEDICINE>();
        items.ForEach(o => o.ECDS_DISEASE_CASE_ID = ecdsDiseaseCaseId);

        // Con đang có trong DB theo cha  (⚠ cần filter ECDS_DISEASE_CASE_ID — xem §20.10)
        List<HIS_ECDS_MALARIA_MEDICINE> olds = new HisEcdsMalariaMedicineGet(param).Get(
            new HisEcdsMalariaMedicineFilterQuery { ECDS_DISEASE_CASE_ID = ecdsDiseaseCaseId, IS_ACTIVE = 1 })
            ?? new List<HIS_ECDS_MALARIA_MEDICINE>();

        List<HIS_ECDS_MALARIA_MEDICINE> inserts = items.Where(o => o.ID <= 0).ToList();
        List<HIS_ECDS_MALARIA_MEDICINE> updates = items.Where(o => o.ID  > 0).ToList();

        // Kiểm tra ID sửa phải thuộc cha này
        foreach (var u in updates)
            if (!olds.Exists(o => o.ID == u.ID))
                throw new Exception("HIS_ECDS_MALARIA_MEDICINE ID khong thuoc ca benh: " + u.ID);

        // XOÁ = có trong DB nhưng không có trong danh sách sửa
        List<HIS_ECDS_MALARIA_MEDICINE> deletes = olds.Where(o => !updates.Exists(u => u.ID == o.ID)).ToList();

        if (IsNotNullOrEmpty(inserts) && !this.create.CreateList(inserts))
            throw new Exception("Them HIS_ECDS_MALARIA_MEDICINE that bai");
        if (IsNotNullOrEmpty(updates) && !this.update.UpdateList(updates))
            throw new Exception("Sua HIS_ECDS_MALARIA_MEDICINE that bai");
        if (IsNotNullOrEmpty(deletes) && !this.truncate.TruncateList(deletes))
            throw new Exception("Xoa HIS_ECDS_MALARIA_MEDICINE that bai");

        result = true;
    }
    catch (Exception ex) { LogSystem.Error(ex); result = false; }
    return result;
}
```

> `TravelHistoryProcessor` viết y hệt, thay kiểu `HIS_ECDS_TRAVEL_HISTORY` và các sub‑operation tương ứng.

### 20.9 Giao dịch & Rollback (không có transaction DB ambient)

MOS **không** có transaction DB tự động ở tầng Manager → mỗi sub‑operation (`Create/Update`) tự giữ ảnh dữ liệu (`recent…` khi tạo, `beforeUpdate…` khi sửa) và có `RollbackData()`. Orchestrator gom lại:

```csharp
private void Rollback()          // gọi trong catch của Run()
{
    try
    {
        this.travelHistoryProcessor.RollbackData();     // ngược thứ tự đã ghi
        this.malariaMedicineProcessor.RollbackData();
        this.diseaseCaseProcessor.RollbackData();
    }
    catch (Exception ex) { LogSystem.Error(ex); }
}
```

- **Create rollback**: `Truncate` các dòng vừa tạo (cha + con).
- **Update rollback**: `UpdateList(before...)` khôi phục cha + con; các dòng đã `Truncate` (xoá) được khôi phục bởi rollback của sub‑operation Truncate (nếu dùng `Truncate` mềm) — nếu xoá cứng thì cần giữ `deletes` để `CreateList` lại khi rollback.

### 20.10 Việc cần bổ sung trước khi code

| # | Việc | Ghi chú |
|---|------|---------|
| 1 | `HisEcdsDiseaseCaseSDO`, `HisEcdsDiseaseCaseResultSDO` | thêm vào `MOS.SDO` |
| 2 | Filter con theo cha | Thêm biểu thức `ECDS_DISEASE_CASE_ID` vào `HisEcdsMalariaMedicineFilterQuery` & `HisEcdsTravelHistoryFilterQuery` (+ field ở `MOS.Filter`) — hiện auto‑gen **chưa có** → không load được `olds` để diff/xoá |
| 3 | `SDO/HisEcdsDiseaseCaseSDOCheck.cs` | `ValidData` |
| 4 | `SDO/Create/*` + `SDO/Update/*` | orchestrator + 3 processor mỗi bên |
| 5 | 2 method `Create/Update(SDO)` ở `HisEcdsDiseaseCaseManager` | **validate trước try** |
| 6 | 2 action ở `HisEcdsDiseaseCaseController` | nhận `ApiParam<HisEcdsDiseaseCaseSDO>` |
| 7 | (tuỳ chọn) EventLog | mẫu `EventLogGenerator` ở `HisAllergyCard*SDO` nếu cần nhật ký nghiệp vụ |

> Các sub‑operation `Create/Update/Truncate/Get` của 3 bảng **đã có sẵn** (scaffolding), cùng assembly `MOS.MANAGER` (`internal`) nên orchestrator gọi trực tiếp được.

### 20.11 Ví dụ payload

**Tạo mới** (mọi ID = 0/không gửi):
```json
{
  "ApiData": {
    "DiseaseCase": { "TREATMENT_ID": 123456, "REPORTED_ICD_CODE": "A90", "DIAGNOSIS_TYPE": 1, "ONSET_DATE": 20260720000000 },
    "MalariaMedicines": [ { "MEDICINE_CODE": "CQ", "MEDICINE_NAME": "Chloroquin", "QUANTITY": 4, "DAY_COUNT": 3 } ],
    "TravelHistories":  [ { "FROM_DATE": 20260701000000, "TO_DATE": 20260710000000, "LOCATION_COMMUNE_CODE": "27625" } ]
  },
  "CommonParam": {}
}
```

**Cập nhật** (cha có `ID`; con: có `ID` = sửa, không `ID` = thêm; dòng con cũ bị bỏ = xoá):
```json
{
  "ApiData": {
    "DiseaseCase": { "ID": 55, "TREATMENT_ID": 123456, "DIAGNOSIS_TYPE": 1, "CURRENT_STATE": 1 },
    "MalariaMedicines": [
      { "ID": 88, "ECDS_DISEASE_CASE_ID": 55, "MEDICINE_CODE": "CQ", "QUANTITY": 6 },
      { "MEDICINE_CODE": "PQ", "MEDICINE_NAME": "Primaquin", "QUANTITY": 2 }
    ],
    "TravelHistories": []
  },
  "CommonParam": {}
}
```

---

## 21. API Backend MOS — Cập Nhật Danh Sách Kết Quả Liên Thông (Batch Push Result)

> API thứ 3: sau khi plugin đẩy N ca lên cổng ECDS và nhận kết quả từng ca (`maCaBenh`, trạng thái, thông điệp), gọi **1 API cập nhật hàng loạt** các cột đối soát/liên thông trên `HIS_ECDS_DISEASE_CASE` **đã tồn tại**. Đặt luồng tại `MOS.MANAGER.HisEcdsDiseaseCase`.

### 21.1 Phạm vi & đặc thù (khác API Tạo/Sửa ở §20)

Chỉ đụng **5 cột đối soát** (nhóm "Đối soát cổng" §17.1):

| Cột | Ý nghĩa |
|-----|---------|
| `ECDS_CASE_ID` | ID ca trên cổng |
| `ECDS_CASE_CODE` | Mã ca cổng trả về |
| `PUSH_STATE` | 0=chưa đẩy, 1=đã đẩy, 2=lỗi |
| `LAST_PUSH_TIME` | Lần đẩy gần nhất (yyyyMMddHHmmss) |
| `PUSH_MESSAGE` | Thông điệp/lỗi cổng |

- **Chỉ Update, không tạo/xoá** — mọi item bắt buộc `ID > 0`.
- **Cập nhật một phần** — payload KHÔNG mang trường nghiệp vụ (`REPORTED_*`, `ONSET_DATE`…) → **không** được `UpdateList(data)` thẳng (sẽ ghi đè null). Dùng **SQL UPDATE tường minh 5 cột** để không đụng cột nghiệp vụ.
- **SQL 1 lượt** — build câu `UPDATE` trong vòng `for`, chạy `DAOWorker.SqlDAO.Execute(sqls)` **một lần sau vòng lặp** (1 round-trip). Đây là mẫu MOS đã dùng (phần xoá của `HisAllergyCardUpdateSDO`).
- Không liên quan 2 danh sách con.

### 21.2 SDO đầu vào (`MOS.SDO/HisEcdsPushResultSDO.cs`)

```csharp
public class HisEcdsPushResultSDO
{
    public long ID { get; set; }                 // HIS_ECDS_DISEASE_CASE.ID (bắt buộc)
    public string ECDS_CASE_ID { get; set; }
    public string ECDS_CASE_CODE { get; set; }
    public short? PUSH_STATE { get; set; }       // 0/1/2
    public long? LAST_PUSH_TIME { get; set; }
    public string PUSH_MESSAGE { get; set; }
}
```

### 21.3 Controller (`MOS.API`)

```csharp
[HttpPost]
[ActionName("UpdatePushResultList")]
public ApiResult UpdatePushResultList(ApiParam<List<HisEcdsPushResultSDO>> param)
{
    try
    {
        ApiResultObject<bool> result = new ApiResultObject<bool>(false);
        if (param != null)
        {
            HisEcdsDiseaseCaseManager mng = new HisEcdsDiseaseCaseManager(param.CommonParam);
            result = mng.UpdatePushResultList(param.ApiData);
        }
        return new ApiResult(result, this.ActionContext);
    }
    catch (Exception ex) { LogSystem.Error(ex); return null; }
}
```

### 21.4 Manager — VALIDATE TRƯỚC `try`

```csharp
[Logger]
public ApiResultObject<bool> UpdatePushResultList(List<HisEcdsPushResultSDO> data)
{
    ApiResultObject<bool> result = new ApiResultObject<bool>(false);

    // ===== VALIDATE TRƯỚC TRY: list không rỗng + mọi ID > 0 =====
    if (!IsNotNull(param) || !IsNotNullOrEmpty(data) || data.Exists(o => o.ID <= 0))
    {
        return this.PackSingleResult(false);
    }

    // ===== TRY XỬ LÝ NGHIỆP VỤ =====
    try
    {
        bool isSuccess = new HisEcdsDiseaseCasePushResultUpdate(param).Run(data);
        result = this.PackSingleResult(isSuccess);
    }
    catch (Exception ex)
    {
        LogSystem.Error(ex);
        param.HasException = true;
    }
    return result;
}
```

### 21.5 Sub-operation — build SQL trong `for`, `Execute` 1 lượt

```csharp
internal bool Run(List<HisEcdsPushResultSDO> data)
{
    bool result = false;
    try
    {
        // Xác thực mọi ID tồn tại + lấy ảnh hiện tại để rollback
        HisEcdsDiseaseCaseCheck checker = new HisEcdsDiseaseCaseCheck(param);
        List<long> listId = data.Select(o => o.ID).Distinct().ToList();
        List<HIS_ECDS_DISEASE_CASE> listRaw = new List<HIS_ECDS_DISEASE_CASE>();
        if (!checker.VerifyIds(listId, listRaw)) return false;

        long now = Inventec.Common.DateTime.Get.Now() ?? 0;
        string modifier = this.UserName;

        List<string> sqls = new List<string>();
        foreach (var r in data)      // build từng câu UPDATE
        {
            sqls.Add(string.Format(
                "UPDATE HIS_ECDS_DISEASE_CASE SET " +
                "ECDS_CASE_ID = {0}, ECDS_CASE_CODE = {1}, PUSH_STATE = {2}, " +
                "LAST_PUSH_TIME = {3}, PUSH_MESSAGE = {4}, MODIFY_TIME = {5}, MODIFIER = {6} " +
                "WHERE ID = {7}",
                SqlStr(r.ECDS_CASE_ID), SqlStr(r.ECDS_CASE_CODE), SqlNum(r.PUSH_STATE),
                SqlNum(r.LAST_PUSH_TIME), SqlStr(r.PUSH_MESSAGE), now, SqlStr(modifier), r.ID));
        }

        if (IsNotNullOrEmpty(sqls))    // chạy 1 lượt sau vòng for
        {
            if (!DAOWorker.SqlDAO.Execute(sqls))
                throw new Exception("Cap nhat ket qua lien thong that bai.");
            this.beforeUpdateList.AddRange(listRaw);
        }
        result = true;
    }
    catch (Exception ex) { LogSystem.Error(ex); param.HasException = true; result = false; }
    return result;
}

private string SqlStr(string v) => v == null ? "NULL" : "'" + v.Replace("'", "''") + "'";
private string SqlNum(object v) => v == null ? "NULL" : v.ToString();
```

> `RollbackData()` build câu `UPDATE` **ngược** từ `beforeUpdateList` (khôi phục 5 cột) rồi `Execute` — best-effort, ghi `LogSystem.Warn` nếu thất bại.

### 21.6 Lưu ý khi đi đường SQL (so với `UpdateList`)

| Vấn đề | Cách xử lý trong code |
|--------|-----------------------|
| **Escape chuỗi** (SQL injection / lỗi `'`) | `SqlStr()` bọc `'...'` + `Replace("'","''")` cho `PUSH_MESSAGE`, `ECDS_CASE_CODE`… |
| **Audit `MODIFY_TIME`/`MODIFIER`** | set tay trong câu SQL (BridgeDAO KHÔNG tự set khi chạy SQL thô) |
| **Rollback** | giữ `beforeUpdateList` (ảnh trước) → `RollbackData()` build SQL ngược |
| **ID không tồn tại** | `VerifyIds(listId, listRaw)` chặn trước vòng `for` |
| **Batch lớn** | chia lô ≤ 50 ca/lần ở tầng gọi (theo §4.5) |

### 21.7 File thêm/sửa

| File | Loại |
|------|------|
| `MOS.SDO/HisEcdsPushResultSDO.cs` | mới |
| `MOS.MANAGER/HisEcdsDiseaseCase/HisEcdsDiseaseCasePushResultUpdate.cs` | mới (sub-operation) |
| `MOS.MANAGER/HisEcdsDiseaseCase/HisEcdsDiseaseCaseManager.cs` | +method `UpdatePushResultList` (validate trước try) |
| `MOS.API/Controllers/HisEcdsDiseaseCaseController.cs` | +action `UpdatePushResultList` |
| `MOS.SDO.csproj`, `MOS.MANAGER.csproj` | đăng ký file mới |

### 21.8 Ví dụ payload

```json
{
  "ApiData": [
    { "ID": 55, "ECDS_CASE_ID": "9001", "ECDS_CASE_CODE": "CB-2026-000123", "PUSH_STATE": 1, "LAST_PUSH_TIME": 20260725093000, "PUSH_MESSAGE": null },
    { "ID": 56, "ECDS_CASE_ID": null,   "ECDS_CASE_CODE": null,             "PUSH_STATE": 2, "LAST_PUSH_TIME": 20260725093001, "PUSH_MESSAGE": "Thiếu mã xã" }
  ],
  "CommonParam": {}
}
```

---

## 22. Danh Sách & Đồng Bộ — TÁCH THÀNH PLUGIN RIÊNG `InfectiousDiseaseSyncList`

> ⚠ **Cập nhật kiến trúc:** phần **danh sách + đồng bộ hàng loạt** đã được **tách ra plugin riêng `HIS.Desktop.Plugins.InfectiousDiseaseSyncList`** (mô hình `KskSyncList` ↔ `EnterKskInfomantion`). `InfectiousDiseaseReport` **quay về chỉ là form chi tiết** (đẩy từng ca).
> - **InfectiousDiseaseSyncList** (Form): tìm kiếm + grid `V_HIS_TREATMENT` + phân trang + **đồng bộ hàng loạt** (checkbox → BackgroundWorker → `frmEcdsSyncResult`).
> - Bấm **Xem/Sửa** (hoặc double-click) → mở **InfectiousDiseaseReport** qua inter-plugin (`ShowModule` + `HIS_TREATMENT` + `RefeshReference`).
> - Tầng ECDS dùng chung (`EcdsApiWorker`, `EcdsCatalogCache`, `EcdsConfigCFG`, `DiseaseCaseMapper`, DTO, `EnumEcds`) được **nhân bản** sang SyncList để 2 plugin độc lập.
> - Tài liệu riêng: `docs/HIS.Desktop.Plugins.InfectiousDiseaseSyncList.md`.

Phần dưới đây mô tả thiết kế master-detail/đồng bộ (áp dụng cho plugin `InfectiousDiseaseSyncList`).

### 22.1 Cấu trúc UI

```
Form
├─ scMain (SplitContainerControl, Dock Fill)
│   ├─ Panel1 (trái, ~360px)
│   │    ├─ pnlSearch (Top): Mã ĐT · Tên BN · Từ/Đến ngày · [Tìm kiếm]
│   │    ├─ grdList (Fill): Mã ĐT · Mã BN · Bệnh nhân · ICD
│   │    └─ ucPaging (Bottom): phân trang
│   └─ Panel2 (phải)
│        ├─ grpHeader (Top): thông tin BN/điều trị + trạng thái đẩy
│        └─ tabMain (Fill): 5 tab
└─ pnlFooter (Bottom): Lấy dữ liệu · Kiểm tra · Đẩy lên cổng · Mới · Đóng
```

### 22.2 Luồng xử lý (đối chiếu EnterKskInfomantion)

| Bước | EnterKskInfomantion | InfectiousDiseaseReport |
|------|---------------------|-------------------------|
| Nguồn danh sách | `V_HIS_SERVICE_REQ_2` · `api/HisServiceReq/GetView2` | `V_HIS_TREATMENT` · `api/HisTreatment/GetView` |
| Tìm kiếm | `btnSearch` → `FillDataToGridControl` | `btnSearch` → `SearchList` |
| Phân trang | `ucPaging.Init(LoadPaging, param, size, grid)` | `ucPaging.Init(LoadListPaging, param, size, grdList)` |
| Filter | `HisServiceReqView2Filter` (mã ĐT/BN, ngày, khoa, trạng thái) | `HisTreatmentViewFilter` (mã ĐT, tên BN, `IN_TIME_FROM/TO`) |
| Chọn dòng | `GetFocusedRow` → `ChangedDataRow` | `gvList_FocusedRowChanged` → `LoadDetailFromView` |
| Nạp chi tiết | `GetServiceReqData` (API) + `FillDataToEditorControl` | map `V_HIS_TREATMENT`→`HIS_TREATMENT` + `FillDataFromHis` |
| Sau chọn | fill patient info + tabs bên phải | fill header + tabs bên phải |

```
frmInfectiousDiseaseReport_Load
  → InitEnumCombos → InitCatalogCombos
  → SearchList()                      // nạp danh sách trái (mặc định ngày hôm nay)
  → FillDataFromHis()                 // nếu mở theo 1 điều trị (constructor) -> đổ luôn
Chọn dòng grid (gvList.FocusedRowChanged)
  → LoadDetailFromView(V_HIS_TREATMENT)
      → this.treatment = map(view)    // TREATMENT_CODE, TDL_PATIENT_*, ICD_*, IN/OUT_TIME, LAST_DEPARTMENT_ID
      → ClearInputControls()
      → FillDataFromHis()             // đổ header + 5 tab bên phải
```

### 22.3 File bổ sung/sửa

| File | Thay đổi |
|------|----------|
| `MainForm/…__List.cs` | **MỚI** — panel trái: `BuildLeftList`, `SearchList`, `LoadListPaging`, `SetListFilter`, `gvList_FocusedRowChanged`, `LoadDetailFromView` |
| `MainForm/…__BuildUi.cs` | Bọc phải trong `SplitContainerControl`; gắn danh sách trái |
| `MainForm/…__Load.cs` | Gọi `SearchList()` khi mở |
| `MainForm/…__FillData.cs` | Header lấy khoa theo `LAST_DEPARTMENT_ID` |
| `InfectiousDiseaseReport/…Behavior.cs` | **Luôn mở form** (không còn trả null khi thiếu treatment) |
| `.csproj` | Thêm ref: `DevExpress.XtraGrid`, `Inventec.Common.Adapter`, `MOS.Filter`, `Inventec.UC.Paging`, ProjectRef `ApiConsumer`/`Controls.Session`/`ConfigApplication` |

### 22.4 Đồng bộ danh sách (đẩy hàng loạt — mô hình KskSyncList)

Chọn nhiều ca bằng **checkbox** → **Đồng bộ lên cổng (N)** → `BackgroundWorker` đẩy từng ca (login 1 lần) → dialog tổng hợp `frmEcdsSyncResult` → refresh.

| Thành phần | File |
|-----------|------|
| Nút + badge + đa chọn | `…__List.cs` (BuildLeftList: `MultiSelect`, `CheckBoxRowSelect`, `btnSyncList`) |
| Logic đồng bộ | `…__Sync.cs` — `SyncSelected` (BackgroundWorker), `BuildDtoFromTreatment`, `UpdateSyncBadge`, `SetSyncBusy` |
| ADO kết quả | `ADO/EcdsSyncResultADO.cs` |
| Dialog kết quả | `SyncResult/frmEcdsSyncResult.cs` (grid + tóm tắt Tổng/Đã đẩy/Lỗi) |

Đối chiếu KskSyncList: `btnSync_Click → SyncRecords → BackgroundWorker(processor.PushList) → frmKskSyncResult` ≈ `btnSyncList → SyncSelected → BackgroundWorker(loop DayCaBenh) → frmEcdsSyncResult`.

**Ảnh giao diện:**

![Đồng bộ danh sách + dialog kết quả](ecds-ui-synclist.png)

### 22.5 TODO còn lại
- Lọc **chỉ điều trị có ICD bệnh truyền nhiễm** (danh mục BTN / `/danh-muc/benh`).
- Cột **trạng thái đẩy** trên grid (○ chưa / ✔ đã / ✖ lỗi) — cần bảng đối soát `HIS_ECDS_DISEASE_CASE` + API (§20/§21).
- **Tự động đẩy** theo chu kỳ (Timer nền).
- Đẩy dùng batch API `cap-nhat-nhieu` thay vì lặp từng ca (tối ưu).

---

## 23. Bản Đồ API ↔ Chức Năng

Gộp toàn bộ API tích hợp trong tài liệu (ECDS cổng ngoài · MOS backend nội bộ · HIS view · inter-plugin) và gán cho từng chức năng của **2 plugin**.

### 23.1 Theo chức năng

| Chức năng | Plugin | Loại API | Endpoint / Cách gọi |
|-----------|--------|----------|---------------------|
| Đăng nhập cổng (lấy token) | cả 2 | ECDS | `POST /api/fast/v1/auth/login` |
| Nạp danh mục bệnh | Detail | ECDS | `POST /api/fast/v1/danh-muc/benh` |
| Nạp danh mục địa bàn | Detail | ECDS | `POST /api/fast/v1/danh-muc/{tinh,xa,thon}` |
| Nạp dân tộc/nghề/quốc gia | Detail | ECDS | `POST /api/fast/v1/danh-muc/{dan-toc,nghe-nghiep,quoc-gia}` |
| Nạp phân loại/cấp độ bệnh | Detail | ECDS | `POST /api/fast/v1/danh-muc/phan-loai-lam-sang` |
| Nạp thuốc sốt rét | Detail | ECDS | `POST /api/fast/v1/danh-muc/thuoc-sot-ret` |
| Đổ dữ liệu BN/điều trị từ HIS (Fill) | Detail | HIS | `BackendDataWorker` (HIS_GENDER/DEPARTMENT/BRANCH) + `V_HIS_PATIENT`/`V_HIS_ECDS_DISEASE_CASE` |
| **Đẩy 1 ca lên cổng** (nút Đẩy) | Detail | ECDS | `POST /api/fast/v1/ca-benh/cap-nhat` |
| **Lưu bản ghi đối soát HIS — tạo** | Detail | MOS | `POST api/HisEcdsDiseaseCase/SaveCreate` (§20) |
| **Lưu bản ghi đối soát HIS — sửa/đẩy lại** | Detail | MOS | `POST api/HisEcdsDiseaseCase/SaveUpdate` (§20) |
| Danh sách y lệnh (tìm kiếm + phân trang) | SyncList | HIS | `POST api/HisTreatment/GetView` (`HisTreatmentViewFilter`) |
| **Đồng bộ hàng loạt** (chọn nhiều → đẩy) | SyncList | ECDS | `POST /api/fast/v1/ca-benh/cap-nhat-nhieu` (hoặc lặp `.../cap-nhat`) |
| **Lưu kết quả đẩy hàng loạt vào HIS** | SyncList | MOS | `POST api/HisEcdsDiseaseCase/UpdatePushResultList` (§21) |
| **Đối soát với cổng** (tránh trùng) | SyncList | ECDS | `POST /api/fast/v1/ca-benh/danh-sach` (`SearchDiseaseCaseFastDto`) |
| Mở form chi tiết (Xem/Sửa) | SyncList | inter-plugin | `PluginInstanceBehavior.ShowModule("…InfectiousDiseaseReport", HIS_TREATMENT, RefeshReference)` |

### 23.2 Theo endpoint (nguồn)

| Nhóm | Endpoint | Dùng ở chức năng |
|------|----------|------------------|
| **ECDS cổng** | `/api/fast/v1/auth/login` | Mọi thao tác gọi cổng (login 1 lần/phiên) |
| | `/api/fast/v1/danh-muc/*` | Nạp combo danh mục (Detail) + map mã HIS→ID ECDS |
| | `/api/fast/v1/ca-benh/cap-nhat` | Đẩy 1 ca (Detail) |
| | `/api/fast/v1/ca-benh/cap-nhat-nhieu` | Đồng bộ hàng loạt (SyncList) |
| | `/api/fast/v1/ca-benh/danh-sach` | Đối soát (SyncList) |
| **MOS backend** | `api/HisEcdsDiseaseCase/SaveCreate` | Lưu ca bệnh mới vào HIS (Detail, §20) |
| | `api/HisEcdsDiseaseCase/SaveUpdate` | Sửa/đẩy lại (Detail, §20) |
| | `api/HisEcdsDiseaseCase/UpdatePushResultList` | Cập nhật trạng thái đẩy hàng loạt (SyncList, §21) |
| **HIS view** | `api/HisTreatment/GetView` | Danh sách y lệnh (SyncList) |

> **Thứ tự gọi khi đẩy 1 ca (Detail):** `auth/login` → (đã nạp `danh-muc/*`) → `ca-benh/cap-nhat` → nếu `thanhCong`: `HisEcdsDiseaseCase/SaveCreate` (hoặc `SaveUpdate` nếu đã có `ECDS_CASE_ID`).
> **Thứ tự khi đồng bộ hàng loạt (SyncList):** `auth/login` (1 lần) → lặp/`cap-nhat-nhieu` → `HisEcdsDiseaseCase/UpdatePushResultList` (lưu kết quả) → (tuỳ chọn) `ca-benh/danh-sach` đối soát.

---

## 24. Changelog thiết kế

| Ngày | Người | Thay đổi |
|------|-------|----------|
| 24/07/2026 | nampp | Bản thiết kế gốc (Form chi tiết + danh sách + auto-push) |
| 24/07/2026 | nampp | Bổ sung spec chính thức ECDS: EnumEcds.cs, bảng lưu `HIS_ECDS_DISEASE_CASE`, chốt nguồn danh mục (HIS_CAREER, SDA_NATIONAL, danh mục liên thông, HIS_BRANCH.BRANCH_NAME). Ghi chú: danh mục ECDS là ID số → map từ HIS, không map thẳng GSO. |
| 24/07/2026 | nampp | Chuẩn hoá SQL §17.3: ID/FK `NUMBER(19)`, bỏ cột `PATIENT_ID`; thêm view `V_HIS_ECDS_DISEASE_CASE` (kèm BN/điều trị từ HIS_TREATMENT). Tách file `docs/ecds-tables.sql`. |
| 25/07/2026 | nampp | Code plugin (30 file, Form 5 tab dựng bằng code). Bổ sung §19 phân tích luồng Frontend theo code. |
| 25/07/2026 | nampp | Bổ sung §20: thiết kế **API backend MOS tạo & sửa ca bệnh** (aggregate cha + 2 danh sách con) theo mẫu `MOS.MANAGER.HisAllergyCard`; validate trước `try`, đặt luồng tại `MOS.MANAGER.HisEcdsDiseaseCase`, diff con Insert/Update/Delete theo ID, rollback chuỗi. |
| 25/07/2026 | nampp | Sinh code §20 (SDO + SDOCheck + 2 orchestrator + 6 processor + Manager/Controller + filter con `ECDS_DISEASE_CASE_ID`). Bổ sung §21: **API cập nhật danh sách kết quả liên thông** (batch push result) — build SQL trong `for`, `SqlDAO.Execute` 1 lượt sau vòng lặp, validate trước `try`, rollback bằng SQL ngược. |
| 25/07/2026 | nampp | Bổ sung §6.3.1: **chuẩn hoá cấu hình 1 key pipe** `MOS.HIS_ECDS_SYNC.ECDS_CONNECTION_INFO` (`MaDonVi\|MaCoSoDieuTri\|Username\|Password\|MaTinh\|BaseUrl\|LoginPath\|PushPath`) theo mẫu KSK 2062 — thay cho nhiều key ở §6.3. |
| 25/07/2026 | nampp | Tách plugin danh sách/đồng bộ thành `InfectiousDiseaseSyncList`; thêm §23 bản đồ API↔chức năng. **Nối API lưu HIS**: Detail gọi `SaveCreate`/`SaveUpdate` sau khi đẩy + `Get` đối soát khi mở; SyncList gọi `UpdatePushResultList` sau đồng bộ (thêm `HisRequestUriStore` + ADO). |




