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
- Bắt buộc map được: `BENHCHUANDOAN_ID`, `GIOITINH`, `DANTOC_ID` (các trường `Có` bắt buộc của 2 object spec).
- Đẩy lại (update) dùng lại `id`/`maCaBenh` đã lưu để tránh tạo trùng.

---

## 4. Thiết Kế Giao Diện (tương tự MchTreatmentExamService)

Giao diện gồm **đúng 2 tab** khớp 2 object trong mục "CHUẨN ĐỊNH DẠNG DỮ LIỆU KẾT NỐI QUA CỔNG API" (QĐ 4039/2025/BYT): **1) Đối tượng mắc bệnh (`DOI_TUONG_MAC_BENH`)** và **2) Trường hợp bệnh (`TRUONG_HOP_BENH`)**. Header thông tin điều trị (chỉ đọc, 2 cột) + footer nút thao tác.

Mỗi tab chia thành **nhóm có tiêu đề (group box)**; trong mỗi nhóm các ô nhập bố cục **2 cột** (nhãn căn phải rộng cố định 150px cho thẳng hàng, ô nhập cân đối; trường bắt buộc `(*)` tô **maroon**). Trường dài (Bệnh ICD-10, các memo chẩn đoán, tiền sử dịch tễ, ghi chú) chiếm trọn chiều ngang nhóm.

- **Tab Đối tượng mắc bệnh**: nhóm *Thông tin cá nhân* · *Địa chỉ hiện nay* · *Địa chỉ thường trú*.
- **Tab Trường hợp bệnh**: nhóm *Chẩn đoán* · *Diễn biến & Ra viện* · *Vắc xin & Xét nghiệm* · *Người báo cáo*.

### 4.1 Sơ đồ tổng thể

```
+---------------------------------------------------------------------------------+
| [HEADER - chỉ đọc] Mã ĐT: .... | Bệnh nhân: .... | Ngày sinh: .. | ICD: ....    |
|                    Trạng thái đẩy: ● Chưa đẩy / ✔ Đã đẩy (Mã CB: 123456)         |
+---------------------------------------------------------------------------------+
| Tab: [Đối tượng mắc bệnh] [Trường hợp bệnh]                                      |
| +-----------------------------------------------------------------------------+ |
| |  (nội dung tab đang chọn — LayoutControl 2 cột)                             | |
| +-----------------------------------------------------------------------------+ |
+---------------------------------------------------------------------------------+
| [Lấy dữ liệu từ HIS] [Kiểm tra danh mục] [Lưu] [Đẩy lên cổng] [Mới] [Đóng]      |
+---------------------------------------------------------------------------------+
```

### 4.2 Chi tiết 2 tab (map thẳng field spec QĐ 4039)

**Tab 1 — Đối tượng mắc bệnh (`DOI_TUONG_MAC_BENH`)** — nguồn `V_HIS_PATIENT`; combo dân tộc/nghề/tỉnh/xã lấy từ SDA (map mã), đẩy cổng đối chiếu mã→ID.
| Control | Field spec | Bắt buộc | Editor/Nguồn |
|---------|-----------|:--:|--------------|
| `txtHoTen` | `HOTEN` | ✔ | TextEdit |
| `dteNgaySinh` | `NGAYSINH` | ✔ | DateEdit |
| `spnTuoi` | *(suy từ NGAYSINH)* | | SpinEdit (không đẩy) |
| `cboGioiTinh` | `GIOITINH` (1=Nam,0=Nữ) | ✔ | LookUpEdit (enum) |
| `txtCccd` | `CCCD` | ✔ | TextEdit |
| `chkMangThai` | `IS_MANGTHAI` (0/1) | | CheckEdit |
| `txtDienThoai` | `DIENTHOAI` | ✔ | TextEdit |
| `cboDanToc` | `DANTOC_ID` | ✔ | LookUpEdit (SDA_ETHNIC→ID cổng) |
| `cboNgheNghiep` | `NGHENGHIEP_ID` | | LookUpEdit (HIS_CAREER→ID cổng) |
| `txtDiaChi` | `DIACHI` (hiện nay) | | TextEdit |
| `cboTinh` | `TINH_ID` (hiện nay) | | LookUpEdit (SDA_PROVINCE→ID) |
| `cboXa` | `XA_ID` (hiện nay) | | LookUpEdit (SDA_COMMUNE→ID) |
| `cboThon` | `THON_ID` (hiện nay) | | LookUpEdit (cascade danh mục cổng `thon` theo xã) |
| `cboTinhTru` | `TINH_ID_THUONGTRU` | | LookUpEdit |
| `cboXaTru` | `XA_ID_THUONGTRU` | | LookUpEdit |
| `txtDiaChiTru` | `DIACHI_THUONGTRU` | | TextEdit |
| `txtNoiLamViec` | `NOILAMVIEC` | | TextEdit |

**Tab 2 — Trường hợp bệnh (`TRUONG_HOP_BENH`)** — chẩn đoán + xét nghiệm + diễn biến + người báo cáo (spec gộp toàn bộ vào object này).
| Control | Field spec | Bắt buộc | Editor/Nguồn |
|---------|-----------|:--:|--------------|
| `cboBenh` | `BENHCHUANDOAN_ID` | ✔ | GridLookUpEdit (`/danh-muc/benh`, tự chọn theo ICD hồ sơ) |
| `cboCapDoBenh` | `DM_CAPDOBENH_ID` | | LookUpEdit (cascade `phan-loai-lam-sang` theo ICD) |
| `cboLoaiChanDoan` | `PHANLOAICHUANDOAN` (0=Nghi,1=Xác định) | ✔ | LookUpEdit (enum) |
| `dteNgayKhoiPhat` | `NGAYKHOIPHAT` | | DateEdit |
| `dteNgayNhapVien` | `NGAYNHAPVIEN` | ✔ | DateEdit (từ `IN_TIME`) |
| `cboTinhTrang` | `TINHTRANGHIENNAY` (0..5) | ✔ | LookUpEdit (enum, suy từ `TREATMENT_END_TYPE_ID`) |
| `txtTinhTrangKhac` | `TINHTRANGKHAC` | | TextEdit |
| `dteNgayRaVien` | `NGAYRAVIEN` | | DateEdit (từ `OUT_DATE`) |
| `cboTinhTrangRaVien` | `TINHTRANGRAVIEN` | | LookUpEdit (HIS_TREATMENT_END_TYPE — enum cổng chờ xác nhận) |
| `dteNgayTuVong` | `NGAYTUVONG` | | DateEdit (từ `DEATH_TIME`) |
| `cboBenhVienChuyenToi` | `BENHVIENCHUYENTOI_ID` (+ tên `BENHVIENCHUYENTOI`) | | LookUpEdit (`/danh-muc/don-vi`) |
| `cboSuDungVacXin` | `SUDUNGVACXIN` (⚠0=Có) | | LookUpEdit (enum) |
| `spnSoLan` | `SOLANSUDUNG` | | SpinEdit |
| `cboLayMau` | `LAYMAUXETNGHIEM` (⚠0=Có) | | LookUpEdit (enum) |
| `cboLoaiXN` | `LOAIXETNGHIEM` (0..3) | | LookUpEdit (enum) |
| `txtLoaiXNKhac` | `LOAIXETNGHIEMKHAC` | | TextEdit |
| `cboKetQuaXN` | `KETQUAXETNGHIEM` (0..2) | | LookUpEdit (enum) |
| `dteNgayThucHienXN` | `NGAYTHUCHIENXN` | | DateEdit |
| `dteNgayTraKQ` | `NGAYTRAKETQUAXN` | | DateEdit |
| `cboDonViXN` | `DONVITHUCHIENXN` | | LookUpEdit (`/danh-muc/don-vi`) |
| `cboLoaiPhatHien` | `LOAIPHATHIEN` (0..3) | ✔ | LookUpEdit (enum) |
| `lblCoSoDieuTriVal` | `CO_SO_DIEU_TRI` | | Label (HIS_BRANCH.BRANCH_NAME) |
| `txtNguoiBaoCao` | `NGUOIBAOCAO` | ✔ | TextEdit (user đăng nhập) |
| `txtDienThoaiBaoCao` | `DIENTHOAINGUOIBAOCAO` | ✔ | TextEdit |
| `txtEmailBaoCao` | `EMAILNGUOIBAOCAO` | ✔ | TextEdit |
| `lblMaDonViVal` | *(config)* | | Label (`EcdsConfigCFG.MaDonVi`) |
| `txtChanDoanRaVien` | `CHAN_DOAN_RA_VIEN` | | MemoEdit (từ `ICD_NAME`) |
| `txtSubDiagnosis` | `BENHCHUANDOANPHU` | | MemoEdit (từ `ICD_TEXT`) |
| `txtComplication` | `CHUANDOANBIENCHUNG` | | MemoEdit |
| `txtTienSuDichTe` | `TIEN_SU_DICH_TE` | | MemoEdit |
| `txtGhiChu` | `GHICHU` | | MemoEdit |

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

**Chi tiết tab (bố cục 2 cột):**

![Chi tiết tab](ecds-ui-tabs.png)

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
| 27/07/2026 | nampp | Thêm **danh sách bên trái** (tham khảo `EnterKskInfomantionQD831`): panel trái lọc mã ĐT/tên BN + khoảng ngày → grid `V_HIS_TREATMENT` (nạp nền), click 1 dòng → `ReloadForTreatment` nạp lại chi tiết theo điều trị (không mở form mới). Bọc nội dung phải trong `pnlRight` + splitter; thêm ref `DevExpress.XtraGrid`; nới rộng form. Partial `__ListPanel.cs`. |
| 27/07/2026 | nampp | Thiết kế **§20b GetFull** (đọc cha + 2 con theo `TREATMENT_CODE`) + **đấu nối**: thay `LoadExistingReconcile` → `LoadEcdsCaseFull` (`api/HisEcdsDiseaseCase/GetFull`) đặt trạng thái đối soát từ cha; thêm **2 grid con** (thuốc sốt rét, lịch sử di chuyển) ở tab Sốt rét, đổ từ GetFull (best-effort). Thêm `HIS_ECDS_GET_FULL` + ADO `HisEcdsDiseaseCaseFullADO` (+ `EcdsMalariaMedicineADO`, `EcdsTravelHistoryADO`, filter theo code). |
| 27/07/2026 | nampp | **Backend MOS đã ship** → thay placeholder bằng **type MOS thật**: `GetFull` dùng `MOS.SDO.HisEcdsDiseaseCaseByCodeFilter` → `MOS.SDO.HisEcdsDiseaseCaseFullSDO` (DiseaseCase=`V_HIS_ECDS_DISEASE_CASE`, 2 con=`HIS_ECDS_MALARIA_MEDICINE`/`HIS_ECDS_TRAVEL_HISTORY`), grid con map field thật (MEDICINE_NAME/QUANTITY/UNIT_CODE/DAY_COUNT/NOTE; LOCATION_NAME/FROM_DATE/TO_DATE/NOTE). **Save §20** dùng `MOS.SDO.HisEcdsDiseaseCaseSDO` + `BuildCaseEntity` map form → `HIS_ECDS_DISEASE_CASE` (đúng kiểu short?/decimal?/long?). Thêm ref `MOS.SDO`; xóa `LoadExistingReconcile`. |
| 27/07/2026 | nampp | `GetFull` giữ verb **GET** (`BackendAdapter.Get`); backend chỉnh `[HttpGet]` cho khớp (trước đó GET → 405 vì controller để POST). Cập nhật §20b theo type thật đã ship + bảng endpoint↔verb. |
| 27/07/2026 | nampp | **Cấu hình ECDS gộp 1 key** (§6.3.1): `EcdsConfigCFG` đọc `MOS.HIS_ECDS_SYNC.ECDS_CONNECTION_INFO` tách 8 phần `MaDonVi\|MaCoSoDieuTri\|Username\|Password\|MaTinh\|BaseUrl\|LoginPath\|PushPath` (thay 6 key `ECDS.API.*` cũ — nguyên nhân lỗi "Không get được config"). `EcdsApiWorker` dùng `LoginPath`/`PushPath` từ config (fallback mặc định). Áp dụng cả 2 plugin. |
| 27/07/2026 | nampp | Fix deserialize danh mục cổng: `duLieu` là object phân trang `{danhSach:[...]}` → thêm `DanhMucPageDto`, `LayDanhMuc` trả `duLieu.danhSach` (cả 2 plugin). |
| 27/07/2026 | nampp | **Tab Ca bệnh đồng bộ từ hồ sơ**: nạp đầy đủ `V_HIS_TREATMENT` (`LoadFullTreatment`); ICD-10 giữ combo cổng tự chọn theo `ICD_CODE`; **Phân độ bệnh** nạp cascade danh mục cổng `phan-loai-lam-sang` theo ICD; **Chẩn đoán ra viện**=`ICD_NAME`, **phụ**=`ICD_TEXT`; **Tình trạng hiện nay** suy từ điều trị (Tử vong nếu `DEATH_TIME`>0 → Ra viện nếu `OUT_TIME`>0 → Nội trú) + điền Ngày tử vong. Chẩn đoán biến chứng để trống (HIS không có trường riêng). |
| 27/07/2026 | nampp | Tab Ca bệnh: **đưa "Bệnh (ICD-10)" lên trên cùng** (full-width); **đổi bệnh → nạp lại phân độ bệnh** theo ICD của bệnh đang chọn (`cboBenh_EditValueChanged` → cascade). Thêm **nút "Lưu"** (`SaveToHisProcess` → `SaveCreate`/`SaveUpdate` theo `hisEcdsCaseId`, giữ nguyên trạng thái đẩy). **Load ưu tiên GetFull**: có ca đã lưu → `MapFromSavedCase` (Ca bệnh + Triệu chứng + Người báo cáo từ `V_HIS_ECDS_DISEASE_CASE`); chưa có → lấy từ hồ sơ HIS. Refactor `BuildCaseEntity()` (chỉ map field ca bệnh) + `NewCaseSdo`; `LoadEcdsCaseFull` trả SDO. |
| 27/07/2026 | nampp | Hành chính: **địa chỉ hiện nay** trống `HT_ADDRESS` → fallback `ADDRESS` (thường trú). Ca bệnh: **Tình trạng hiện nay** map từ `TREATMENT_END_TYPE_ID` (RAVIEN 6→2, CHET 1→3, CHUYEN 2→4, khác→5); không có end-type → theo `TDL_TREATMENT_TYPE_ID` (DTNOITRU 3→1 nội trú, còn lại→0 ngoại trú). **Ngày ra viện** = `OUT_DATE`; **Ngày tử vong** = `DEATH_TIME`. |
| 27/07/2026 | nampp | **Tab Hành chính lấy danh mục từ SDA/HIS** (không phụ thuộc cổng): dân tộc=`SDA_ETHNIC`(ETHNIC_CODE), nghề=`HIS_CAREER`(CAREER_CODE), tỉnh=`SDA_PROVINCE`(PROVINCE_CODE), xã=`SDA_COMMUNE`(COMMUNE_CODE) — `InitSdaAdminCombos`. Map từ `V_HIS_PATIENT` theo mã: CCCD=`CCCD_NUMBER`, dân tộc=`ETHNIC_CODE`; **hiện nay** tỉnh=`HT_PROVINCE_CODE`(fb PROVINCE_CODE)/xã=`HT_COMMUNE_CODE`(fb COMMUNE_CODE)/địa chỉ=`HT_ADDRESS`; **thường trú** tỉnh=`PROVINCE_CODE`/xã=`COMMUNE_CODE`/địa chỉ=`ADDRESS`. Khi đẩy cổng: đối chiếu mã SDA→ID cổng (`ResolveEcdsIdStatic`/`ResolveEcdsIdXa`). Thu gọn ô input (cap 340px) trên bố cục 2 cột. |
| 28/07/2026 | nampp | **Sửa 2 cột thật (item-move)**: header + 2 tab tách cột bằng `LayoutControlItem.Move(left, InsertType.Right)` (cách group-move không render 2 cột); bỏ `MaxSize` để ô lấp đầy nửa hàng. Panel **tìm kiếm danh sách** dựng lại bằng `LayoutControl` (bỏ toạ độ tuyệt đối — nguồn lỗi lệch/chồng ô), theo mẫu `EnterKskInfomantionQD831`. Tăng **chiều cao header** 120→156. Sửa `cboThon` hiển thị `[EditValue is null]` (set `NullText=""`). |
| 28/07/2026 | nampp | **Log API đẩy cổng**: `DayCaBenh`/`DayNhieuCaBenh` log đầy đủ request (URL + JSON) + response (`PostRaw` logRaw: HTTP status + body thô) + tóm tắt `thanhCong/maLoi/thongDiep` — để trace lỗi cổng (VD "bạn phải chọn bệnh"). **Bỏ combo "Phòng khám"** khỏi panel tìm kiếm danh sách (danh sách đã lọc theo ICD truyền nhiễm qua `ICD_CODE_OR_ICD_SUB_CODEs`); gỡ `cboListRoom` + `LoadListRoomCombo` + tham số `roomId`. |
| 28/07/2026 | nampp | **Fix ICD-10 tab Trường hợp bệnh**: (1) `V_HIS_TREATMENT.ICD_CODE` có thể là chuỗi nhiều mã ("A00, A00.0, A00.1, A00.9") còn danh mục **bệnh cổng** cũng để `ma` dạng danh sách → thêm `PrimaryIcdCode` (lấy mã chính, ≤10 ký tự) cho matching + `REPORTED_ICD_CODE` (sửa **ORA-12899** cột tối đa 10); (2) `FindIdByMa` **khớp theo token** (mã "A00" nằm trong danh sách mã của bệnh) → combo Bệnh tự chọn đúng theo ICD hồ sơ. |
| 28/07/2026 | nampp | **Bỏ 3 nút footer** (Lấy dữ liệu từ HIS / Kiểm tra danh mục / Đóng) — còn Lưu · Đẩy lên cổng · Mới (xóa handler `btnGetData_Click/btnCheck_Click/btnClose_Click`). **Ô tìm kiếm danh sách** đổi sang dạng **hint `NullValuePrompt`** (không nhãn) như `EnterKskInfomantionQD831`: "Nội dung tìm kiếm (mã ĐT / mã BN / tên BN)". |
| 28/07/2026 | nampp | **UI không tạo ở runtime**: dời TOÀN BỘ code dựng giao diện + khai báo control vào `frmInfectiousDiseaseReport.Designer.cs` (`InitializeComponent` gọi `BuildHeader/BuildTabs/BuildFooter/BuildListPanel` + helper). Constructor KHÔNG còn gọi `BuildUi()`; **xóa `__BuildUi.cs`**; trim khai báo control khỏi `frmInfectiousDiseaseReport.cs` + `__ListPanel.cs` (chỉ giữ logic/data/event). Giữ nguyên bố cục (2 tab, nhóm 2 cột, panel tìm kiếm). |
| 28/07/2026 | nampp | **Panel tìm kiếm danh sách — bổ sung phòng khám + từ khóa server-side** (mẫu `EnterKskInfomantionQD831`): thêm combo **Phòng khám** (`cboListRoom` — `V_HIS_ROOM` IS_EXAM=1, popup 2 cột Mã/Tên phòng, mặc định tất cả) → lọc `filter.WORKING_ROOM_ID`; từ khóa dùng **`filter.KEY_WORD`** (server lọc, bỏ lọc client). Nới cao `lcSearch` 110→138. |
| 28/07/2026 | nampp | **Hoàn thiện theo LIB backend mới** (model/filter/SDO đã đẩy): (1) `IS_INFECTIOUS` đã có → `__ListPanel` bỏ reflection, lọc trực tiếp `o.IS_INFECTIOUS==1` (§23b.8); (2) map thêm field mới của `HIS_ECDS_DISEASE_CASE` trong `BuildCaseEntity`: `DISCHARGE_STATE`(TINHTRANGRAVIEN), `TRANSFER_HOSPITAL_NAME`(tên BV chuyển tới), `VILLAGE_ID`(THON_ID), `WORKPLACE`(NOILAMVIEC); `MapFromSavedCase` khôi phục `cboTinhTrangRaVien` từ `DISCHARGE_STATE`; (3) backend đã gỡ `HisEcdsDiseaseCaseSDO/ResultSDO` + `FullSDO` chỉ còn `DiseaseCase` → khớp luồng entity CRUD; xóa ADO chết `HisEcdsDiseaseCaseSaveADO`. Build cả Report + SyncList OK. |
| 28/07/2026 | nampp | **Đổi API lưu HIS sang CRUD entity**: `SaveCreate`→**`Create`**, `SaveUpdate`→**`Update`**; body & kết quả từ `HisEcdsDiseaseCaseSDO`/`HisEcdsDiseaseCaseResultSDO` → **`HIS_ECDS_DISEASE_CASE`** trực tiếp. Bỏ `NewCaseSdo` (+ mọi tham chiếu 2 list con trong luồng ghi). `HisRequestUriStore`: `HIS_ECDS_SAVE_CREATE/UPDATE` → `HIS_ECDS_CREATE/UPDATE`. `SaveToHisProcess`/`PersistToHis` dùng `Post<HIS_ECDS_DISEASE_CASE>`, đọc `saved.ID`. Viết lại §20, cập nhật §20b/§23. |
| 28/07/2026 | nampp | **Triển khai lọc danh sách theo ICD truyền nhiễm (PA2)** trong `FetchListRows`: lấy tập mã ICD từ `BackendDataWorker.Get<V_HIS_ICD>()` (IS_INFECTIOUS=1, IS_ACTIVE=1) rồi lọc `data`. `IS_INFECTIOUS` đọc **an toàn qua reflection** (cột backend bổ sung sau) → biên dịch ngay & tự hoạt động khi có cột; cột chưa có → không lọc (panel vẫn dùng được). Xem §23b.8. Thêm `using HIS.Desktop.LocalStorage.BackendData`. |
| 28/07/2026 | nampp | Thêm **§23b — phân tích lọc danh sách ca bệnh theo ICD truyền nhiễm** (`HIS_ICD.IS_INFECTIOUS=1`): 3 phương án (client-filter / tận dụng `ICD_CODE_OR_ICD_SUB_CODEs` / **filter riêng `IS_INFECTIOUS_ICD` — khuyến nghị**), điểm nối `V_HIS_TREATMENT.ICD_CODE ↔ HIS_ICD`, predicate `EXISTS`, đấu nối `FetchListRows`. Phụ thuộc cột `IS_INFECTIOUS` (HisIcd §5.1). |
| 27/07/2026 | nampp | **Chỉnh giao diện**: mỗi tab chia **nhóm có tiêu đề (group box)**, mỗi nhóm bố cục **2 cột** (nhãn căn phải cố định 150px, ô nhập cap 460px cân đối); trường bắt buộc `(*)` tô **maroon**; trường dài/memo chiếm trọn chiều ngang. Header gọn **2 cột** (3 dòng) + dòng trạng thái đẩy full-width; giá trị chính in đậm. Đối tượng: *Thông tin cá nhân / Địa chỉ hiện nay / Địa chỉ thường trú*. Trường hợp bệnh: *Chẩn đoán / Diễn biến & Ra viện / Vắc xin & Xét nghiệm / Người báo cáo*. |
| 27/07/2026 | nampp | **Chuẩn hoá theo QĐ 4039/2025/BYT — còn ĐÚNG 2 tab** = 2 object spec: **Đối tượng mắc bệnh** (`DOI_TUONG_MAC_BENH`) + **Trường hợp bệnh** (`TRUONG_HOP_BENH`). **BỎ**: tab Sốt rét + nhóm chi tiết sốt rét (soi lam/RDT/G6PD/mật độ KST…), nhóm triệu chứng (Sốt/Rét run/Vã mồ hôi/tương tự gia đình…), **2 bảng con** `HIS_ECDS_MALARIA_MEDICINE`/`HIS_ECDS_TRAVEL_HISTORY` (grid + ADO `HisEcdsDiseaseCaseFullADO` + `BindChildGrids`); `NewCaseSdo` gửi list rỗng tới khi backend gỡ SDO. **THÊM**: `cboThon` (`THON_ID` — cascade danh mục cổng `thon` theo xã), `cboTinhTrangRaVien` (`TINHTRANGRAVIEN` — bind `HIS_TREATMENT_END_TYPE`, enum cổng chờ xác nhận), đẩy tên `BENHVIENCHUYENTOI`. Gộp Ca bệnh/Triệu chứng-XN/Người báo cáo vào tab Trường hợp bệnh. Validate lại theo bắt buộc spec (bỏ Tuổi/Nghề/Tỉnh/Xã/Địa chỉ khỏi bắt buộc). Cập nhật §4, §17.2, §20b. |

## 13. Test Cases

- [ ] Mở form từ 1 điều trị có ICD truyền nhiễm → header & các tab tự fill từ HIS.
- [ ] ICD không thuộc DS truyền nhiễm → cảnh báo, chặn đẩy.
- [ ] Login sai tài khoản → hiển thị `thongDiep` lỗi, không crash.
- [ ] Token còn hạn → không login lại (đẩy nhiều ca liên tiếp chỉ login 1 lần).
- [ ] Thiếu trường `required` (giới tính, xã...) → icon warning tại control, chặn đẩy.
- [ ] Đẩy thành công → hiển thị `maCaBenh`, lưu đối soát, gọi callback refresh.
- [ ] Đẩy lại ca đã có `maCaBenh` → update, không tạo trùng.
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

### 17.2 SQL tạo bảng (Oracle)

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
| Form (UI) | `MainForm/…Designer.cs` | Dựng header + 2 tab + footer + panel danh sách (InitializeComponent) |
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
  → InitializeComponent()      // Designer.cs — dựng TOÀN BỘ control (UI không tạo ở runtime)
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
| `InitializeComponent` (Designer.cs) | Dựng Header (GroupControl) + `XtraTabControl` 2 tab + footer 6 nút + panel danh sách trái; wire event. **UI không tạo ở runtime code-behind.** |
| `InitEnumCombos` | Bind `cboLoaiChanDoan, cboTinhTrang, cboGioiTinh, cboSuDungVacXin, cboLayMau, cboLoaiXN, cboKetQuaXN, cboLoaiPhatHien` từ `EnumEcds` |
| `InitCatalogCombos` | `EnsureLogin` → `GetStatic(benh/dan-toc/nghe-nghiep/tinh/coso)` → `SetupLookup` |
| `FillDataFromHis` | `FillHeader/FillCaBenhTab/FillHanhChinhTab/FillNguoiBaoCaoTab` + `UpdatePushStatusLabel` |

### 19.4 Cấu trúc UI (InitializeComponent — Designer.cs)

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
| `InitDischargeStateCombo` | Enum `TINHTRANGRAVIEN` — hiện tạm map `HIS_TREATMENT_END_TYPE`, xác nhận enum cổng |
| Behavior | Form danh sách (đẩy hàng loạt/tự động) khi không có `HIS_TREATMENT` |

---

## 20. API Backend MOS — Tạo & Sửa Ca Bệnh (CRUD entity)

> Lưu ca bệnh là **CRUD entity chuẩn MOS** trên `HIS_ECDS_DISEASE_CASE` — **không bọc SDO, không danh sách con**. Hai endpoint **`Create`** / **`Update`** nhận & trả thẳng entity `HIS_ECDS_DISEASE_CASE`.

### 20.1 Quy tắc tạo/sửa

```
ID <= 0 (hoặc null)  → TẠO MỚI (Create)
ID  > 0              → CẬP NHẬT (Update)
```

FE chọn endpoint theo `hisEcdsCaseId` (ID bản ghi đang mở).

### 20.2 Endpoint

| API | Verb | Body | Trả về |
|-----|------|------|--------|
| `api/HisEcdsDiseaseCase/Create` | **POST** | `HIS_ECDS_DISEASE_CASE` | `HIS_ECDS_DISEASE_CASE` (có ID) |
| `api/HisEcdsDiseaseCase/Update` | **POST** | `HIS_ECDS_DISEASE_CASE` | `HIS_ECDS_DISEASE_CASE` |

### 20.3 Controller (mẫu CRUD chuẩn MOS)

```csharp
[HttpPost]
public ApiResult Create(ApiParam<HIS_ECDS_DISEASE_CASE> param)
{
    try
    {
        ApiResultObject<HIS_ECDS_DISEASE_CASE> result = new ApiResultObject<HIS_ECDS_DISEASE_CASE>(null);
        if (param != null)
            result = new HisEcdsDiseaseCaseManager(param.CommonParam).Create(param.ApiData);
        return new ApiResult(result, this.ActionContext);
    }
    catch (Exception ex) { LogSystem.Error(ex); return null; }
}
// Update: tương tự -> new HisEcdsDiseaseCaseManager(param.CommonParam).Update(param.ApiData)
```

### 20.4 Manager

Dùng CRUD có sẵn của `HisEcdsDiseaseCaseManager` (`Create`/`Update` nhận `HIS_ECDS_DISEASE_CASE`, trả `ApiResultObject<HIS_ECDS_DISEASE_CASE>`):
- **Create** — BridgeDAO tự set `ID/CREATE_TIME/CREATOR`; trả entity vừa tạo (đã có ID).
- **Update** — kiểm tra tồn tại + không khoá, tự set `MODIFY_TIME/MODIFIER`; trả entity sau sửa.
- Không cần SDO/SDOCheck/orchestrator/rollback con (đã bỏ 2 bảng con — xem §17.2).

### 20.5 Frontend đấu nối (Detail)

`BuildCaseEntity()` map form → `HIS_ECDS_DISEASE_CASE`; set `ECDS_CASE_ID/ECDS_CASE_CODE/PUSH_STATE` (và `LAST_PUSH_TIME/PUSH_MESSAGE` khi đẩy) rồi:

```csharp
string uri = hisEcdsCaseId > 0 ? HisRequestUriStore.HIS_ECDS_UPDATE : HisRequestUriStore.HIS_ECDS_CREATE;
var saved = new BackendAdapter(param).Post<HIS_ECDS_DISEASE_CASE>(uri, ApiConsumers.MosConsumer, c, param);
if (saved != null && saved.ID > 0) this.hisEcdsCaseId = saved.ID;   // giữ ID để lần sau Update
```

- **Nút Lưu** (`SaveToHisProcess`): lưu HIS, **giữ nguyên** trạng thái đẩy hiện tại.
- **Sau khi đẩy cổng thành công** (`PersistToHis`): set `PUSH_STATE=Đã đẩy`, `ECDS_CASE_CODE`, `LAST_PUSH_TIME`, `PUSH_MESSAGE`.

### 20.6 Ví dụ payload

**Tạo mới** (ID = 0/không gửi):
```json
{ "ApiData": { "TREATMENT_ID": 123456, "REPORTED_ICD_CODE": "A90", "DIAGNOSIS_TYPE": 1, "ONSET_DATE": 20260720000000 }, "CommonParam": {} }
```
**Cập nhật** (có `ID`):
```json
{ "ApiData": { "ID": 55, "TREATMENT_ID": 123456, "DIAGNOSIS_TYPE": 1, "CURRENT_STATE": 1 }, "CommonParam": {} }
```


## 20b. API Backend MOS — Lấy Ca Bệnh Theo `TREATMENT_CODE` (GetFull)

> Đọc bản ghi ca bệnh theo **mã hồ sơ điều trị** để: nạp lại ca đã lưu (`MapFromSavedCase`) + xác định trạng thái đối soát đẩy. Mẫu tham chiếu: `HisAllergyCard.GetFull`.

### 20b.1 Type MOS (dùng trực tiếp — KHÔNG tự chế ADO)

```csharp
// MOS.EFMODEL.DataModels
HIS_ECDS_DISEASE_CASE          // bảng cha (ghi)
V_HIS_ECDS_DISEASE_CASE        // view (đọc — join sẵn HIS_TREATMENT/HIS_PATIENT: có TREATMENT_CODE, PATIENT_*)

// MOS.Filter (kế thừa FilterBase: ID, IDs, IS_ACTIVE, ORDER_*, KEY_WORD, CREATE_TIME_FROM/TO...)
HisEcdsDiseaseCaseViewFilter   { string TREATMENT_CODE; List<string> TREATMENT_CODES; }

// MOS.SDO
HisEcdsDiseaseCaseFullSDO      { V_HIS_ECDS_DISEASE_CASE DiseaseCase; }   // đọc (GetFull)
HisEcdsPushResultSDO           { long ID; string ECDS_CASE_ID; string ECDS_CASE_CODE; short? PUSH_STATE; long? LAST_PUSH_TIME; string PUSH_MESSAGE; }  // §21 (khóa theo ID bản ghi)
// Ghi: Create/Update nhận & trả THẲNG entity HIS_ECDS_DISEASE_CASE (không dùng SDO — xem §20).
```

> View `V_HIS_ECDS_DISEASE_CASE` join sẵn `HIS_TREATMENT` nên có cột `TREATMENT_CODE` — lọc thẳng theo mã hồ sơ, không cần phân giải khóa tay.

### 20b.2 Endpoint & HTTP verb (ĐÃ TRIỂN KHAI)

| API | Verb | Filter/Body | Trả về |
|-----|------|-------------|--------|
| `api/HisEcdsDiseaseCase/GetFull` | **GET** | `HisEcdsDiseaseCaseViewFilter { TREATMENT_CODE }` | `List<HisEcdsDiseaseCaseFullSDO>` |
| `api/HisEcdsDiseaseCase/GetView` | **GET** | `HisEcdsDiseaseCaseViewFilter { TREATMENT_CODES }` | `List<V_HIS_ECDS_DISEASE_CASE>` |
| `api/HisEcdsDiseaseCase/Create` · `Update` | **POST** | `HIS_ECDS_DISEASE_CASE` | `HIS_ECDS_DISEASE_CASE` |
| `api/HisEcdsDiseaseCase/UpdatePushResultList` | **POST** | `List<HisEcdsPushResultSDO>` | `bool` |

> ⚠ **Verb khớp frontend**: `GetFull`/`GetView` phải để **`[HttpGet]`** (frontend gọi `BackendAdapter.Get` → `?param=base64`); nếu để `[HttpPost]` → GET trả **405 Method Not Allowed**. `Create`/`Update`/`UpdatePushResultList` là **`[HttpPost]`**.

### 20b.3 Frontend đấu nối

- **Detail** `LoadEcdsCaseFull`: `Get<List<HisEcdsDiseaseCaseFullSDO>>("api/HisEcdsDiseaseCase/GetFull", MosConsumer, HisEcdsDiseaseCaseViewFilter { TREATMENT_CODE = treatment.TREATMENT_CODE }, param)` → dùng `DiseaseCase` (V_HIS_ECDS_DISEASE_CASE) đặt trạng thái đối soát + `MapFromSavedCase`.
- **Detail** `SaveToHisProcess`/`PersistToHis`: `Post<HIS_ECDS_DISEASE_CASE>(Create|Update, HIS_ECDS_DISEASE_CASE)` — `BuildCaseEntity` map form → entity; `saved.ID` giữ lại làm `hisEcdsCaseId`.
- **SyncList** `ReconcilePushState`: `Get<List<V_HIS_ECDS_DISEASE_CASE>>("api/HisEcdsDiseaseCase/GetView", HisEcdsDiseaseCaseViewFilter { TREATMENT_CODES })` → `PUSH_STATE` tô cột trạng thái + lưu `caseIdByTreatment`.
- **SyncList** `PersistPushResults`: `Post<bool>(UpdatePushResultList, List<HisEcdsPushResultSDO>)` khóa theo **ID bản ghi** (từ đối soát); ca chưa có bản ghi → bỏ qua (cần tạo qua form chi tiết).

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
│        └─ tabMain (Fill): 2 tab (Đối tượng mắc bệnh · Trường hợp bệnh)
└─ pnlFooter (Bottom): Lấy dữ liệu · Kiểm tra · Lưu · Đẩy lên cổng · Mới · Đóng
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
      → FillDataFromHis()             // đổ header + 2 tab bên phải
```

### 22.3 File bổ sung/sửa

| File | Thay đổi |
|------|----------|
| `MainForm/…__List.cs` | **MỚI** — panel trái: `BuildLeftList`, `SearchList`, `LoadListPaging`, `SetListFilter`, `gvList_FocusedRowChanged`, `LoadDetailFromView` |
| `MainForm/…Designer.cs` | Bọc phải trong panel + splitter; gắn danh sách trái (InitializeComponent) |
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
| Đổ dữ liệu BN/điều trị từ HIS (Fill) | Detail | HIS | `BackendDataWorker` (HIS_GENDER/DEPARTMENT/BRANCH) + `V_HIS_PATIENT`/`V_HIS_ECDS_DISEASE_CASE` |
| **Đẩy 1 ca lên cổng** (nút Đẩy) | Detail | ECDS | `POST /api/fast/v1/ca-benh/cap-nhat` |
| **Lưu bản ghi đối soát HIS — tạo** | Detail | MOS | `POST api/HisEcdsDiseaseCase/Create` (§20) |
| **Lưu bản ghi đối soát HIS — sửa/đẩy lại** | Detail | MOS | `POST api/HisEcdsDiseaseCase/Update` (§20) |
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
| **MOS backend** | `api/HisEcdsDiseaseCase/Create` | Lưu ca bệnh mới vào HIS (Detail, §20) |
| | `api/HisEcdsDiseaseCase/Update` | Sửa/đẩy lại (Detail, §20) |
| | `api/HisEcdsDiseaseCase/UpdatePushResultList` | Cập nhật trạng thái đẩy hàng loạt (SyncList, §21) |
| **HIS view** | `api/HisTreatment/GetView` | Danh sách y lệnh (SyncList) |

> **Thứ tự gọi khi đẩy 1 ca (Detail):** `auth/login` → (đã nạp `danh-muc/*`) → `ca-benh/cap-nhat` → nếu `thanhCong`: `HisEcdsDiseaseCase/Create` (hoặc `Update` nếu đã có bản ghi HIS).
> **Thứ tự khi đồng bộ hàng loạt (SyncList):** `auth/login` (1 lần) → lặp/`cap-nhat-nhieu` → `HisEcdsDiseaseCase/UpdatePushResultList` (lưu kết quả) → (tuỳ chọn) `ca-benh/danh-sach` đối soát.

---

## 23b. Phân Tích — Lọc Danh Sách Ca Bệnh Theo ICD Truyền Nhiễm (`HIS_ICD.IS_INFECTIOUS = 1`)

### 23b.1 Hiện trạng (theo code)

Panel danh sách bên trái (`frmInfectiousDiseaseReport__ListPanel.cs` → `FetchListRows`) đang:
```
HisTreatmentViewFilter { IN_TIME_FROM/TO, ORDER_FIELD=IN_TIME }
  → api/HisTreatment/GetView  → List<V_HIS_TREATMENT>  (MỌI điều trị trong khoảng ngày)
  → client lọc theo từ khoá (mã ĐT / tên BN)
```
→ **Chưa lọc theo bệnh truyền nhiễm** — đang hiển thị mọi điều trị. Mục tiêu: chỉ hiển thị điều trị có **ICD chính (`ICD_CODE`) là bệnh truyền nhiễm**.

### 23b.2 Phụ thuộc bắt buộc (dữ liệu gốc)

`HIS_ICD` **hiện CHƯA có** cột `IS_INFECTIOUS` (mới có `IS_COVID`, cùng mẫu). Phải làm trước (xem `docs/HIS.Desktop.Plugins.HisIcd.md §5.1`):
- `ALTER TABLE HIS_ICD ADD (IS_INFECTIOUS NUMBER(1));` + thêm cột vào `V_HIS_ICD` → regen `MOS.EFMODEL`.
- Checkbox "Bệnh truyền nhiễm" ở `frmHisIcd` (mẫu `IS_COVID`) để nhập dữ liệu.

Không có cột này thì **không có nguồn** để biết ICD nào là truyền nhiễm.

### 23b.3 Điểm nối dữ liệu

`V_HIS_TREATMENT.ICD_CODE` (chẩn đoán chính) ↔ `HIS_ICD.ICD_CODE` (với `IS_INFECTIOUS = 1, IS_ACTIVE = 1`).
Tuỳ nghiệp vụ có thể mở rộng gồm cả `ICD_SUB_CODE` (chẩn đoán kèm, phân tách `;`) — MVP chỉ khớp ICD chính.

### 23b.4 Các phương án

| # | Phương án | Backend cần | Paging | Hiệu năng | Ghi chú |
|---|-----------|-------------|:------:|-----------|---------|
| PA1 | **Client post-filter**: FE build `HashSet<string>` mã ICD truyền nhiễm từ `BackendDataWorker.Get<HIS_ICD>()`, gọi `GetView` (khoảng ngày) rồi **lọc client** theo set | chỉ cần `IS_INFECTIOUS` vào EFMODEL/cache | ❌ SAI (server trả 50 dòng/trang gồm cả không truyền nhiễm → lọc xong trang rỗng/lệch) | Kém (tải thừa) | Chỉ hợp dữ liệu nhỏ / demo |
| PA2 | **Tận dụng filter có sẵn** `ICD_CODE_OR_ICD_SUB_CODEs` (List đã có trong `HisTreatmentViewFilter`): FE build danh sách mã ICD truyền nhiễm rồi truyền vào filter → **server lọc `IN`** | chỉ cần `IS_INFECTIOUS` vào EFMODEL/cache (KHÔNG cần API mới) | ✅ Đúng | Tốt | ⚠ Oracle `IN` giới hạn 1000 phần tử → nếu tập ICD > 1000 phải chunk. Tập ICD truyền nhiễm thường < vài trăm → OK |
| PA3 | **API/filter riêng** (KHUYẾN NGHỊ): server tự JOIN/`EXISTS` `HIS_ICD` | Có (thêm field filter hoặc endpoint) | ✅ Đúng | Tốt nhất (không `IN` list dài) | Server sở hữu định nghĩa "truyền nhiễm", FE gọn |

### 23b.5 Thiết kế PA3 (khuyến nghị) — 2 cách

**Cách A — Thêm 1 field vào `HisTreatmentViewFilter` (NHẸ NHẤT, tái dùng `GetView`):**
```csharp
// MOS.Filter/HisTreatmentViewFilter.cs
public short? IS_INFECTIOUS_ICD { get; set; }   // 1 = chỉ điều trị có ICD chính là bệnh truyền nhiễm
```
Trong DAO GetView (nơi build WHERE), khi `IS_INFECTIOUS_ICD == 1` thêm predicate:
```sql
AND EXISTS (
    SELECT 1 FROM HIS_ICD i
    WHERE i.ICD_CODE = t.ICD_CODE          -- t: V_HIS_TREATMENT
      AND i.IS_INFECTIOUS = 1 AND i.IS_ACTIVE = 1
)
-- (tuỳ chọn mở rộng chẩn đoán kèm: OR EXISTS ... i.ICD_CODE trong tách ';' của t.ICD_SUB_CODE)
```
→ FE chỉ set `filter.IS_INFECTIOUS_ICD = 1;` — không cần endpoint mới.

**Cách B — Endpoint/VIEW chuyên biệt (nếu muốn tách hẳn):**
- Endpoint mới `api/HisEcdsDiseaseCase/GetTreatmentInfectious` (hoặc `api/HisTreatment/GetViewInfectious`) nhận `HisTreatmentViewFilter`, server **luôn** áp predicate truyền nhiễm; hoặc
- View mới `V_HIS_TREATMENT_INFECTIOUS = V_HIS_TREATMENT ⋈ HIS_ICD (IS_INFECTIOUS=1)` rồi `GetView` trên view đó.
- Sạch về mặt tách bạch nhưng thêm artefact (endpoint/view) so với Cách A.

> **Khuyến nghị: PA3 Cách A** — ít việc nhất (1 field + 1 predicate), tái dùng `GetView` + paging sẵn, đúng chuẩn MOS. Nếu chưa kịp đụng backend GetView → dùng **PA2** ngay (0 API mới), chấp nhận chunk khi tập ICD lớn.

### 23b.6 Frontend đấu nối (`FetchListRows`)

**Theo PA3 Cách A:**
```csharp
var filter = new HisTreatmentViewFilter();
if (tfrom > 0) filter.IN_TIME_FROM = tfrom;
if (tto  > 0) filter.IN_TIME_TO   = tto;
filter.IS_INFECTIOUS_ICD = 1;                 // <-- CHỈ ca có ICD truyền nhiễm (server lọc)
filter.ORDER_FIELD = "IN_TIME"; filter.ORDER_DIRECTION = "DESC";
var data = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(
    "api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param);
// (bỏ mọi bước lọc client theo truyền nhiễm)
```

**Theo PA2 (không cần API mới, chỉ cần `IS_INFECTIOUS` trong cache):**
```csharp
var infectiousCodes = BackendDataWorker.Get<HIS_ICD>()
    .Where(o => o.IS_INFECTIOUS == 1 && o.IS_ACTIVE == 1)
    .Select(o => o.ICD_CODE).Distinct().ToList();
if (infectiousCodes.Count == 0) return new List<ListRowADO>();     // chưa gắn cờ ICD nào
filter.ICD_CODE_OR_ICD_SUB_CODEs = infectiousCodes;               // server lọc IN (chunk nếu > 1000)
```

### 23b.7 Lộ trình

1. **Backend (bắt buộc)**: `HIS_ICD.IS_INFECTIOUS` + `V_HIS_ICD` + regen EFMODEL (HisIcd §5.1); checkbox nhập ở `frmHisIcd` (HisIcd §5.2).
2. **Chọn cách lọc**: PA3-A (thêm `IS_INFECTIOUS_ICD` vào `HisTreatmentViewFilter` + predicate GetView) — khuyến nghị; hoặc PA2 (dùng ngay `ICD_CODE_OR_ICD_SUB_CODEs`).
3. **Frontend**: sửa `FetchListRows` theo §23b.6; (cùng cách áp cho `InfectiousDiseaseSyncList` khi liệt kê ca cần đẩy).

### 23b.8 Trạng thái triển khai — PA2 qua cache `V_HIS_ICD` (ĐÃ HOÀN THIỆN 28/07/2026)

`V_HIS_ICD.IS_INFECTIOUS` **đã có** trong EFMODEL → dùng **truy cập trực tiếp** (không còn reflection). Hiện thực ở `frmInfectiousDiseaseReport__ListPanel.cs` (`FetchListRows`):

```csharp
/// <summary>Tập mã ICD bệnh truyền nhiễm (IS_INFECTIOUS=1, IS_ACTIVE=1) từ cache V_HIS_ICD.</summary>
private HashSet<string> GetInfectiousIcdCodes()
{
    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach (var o in BackendDataWorker.Get<V_HIS_ICD>())
    {
        if (o == null || string.IsNullOrEmpty(o.ICD_CODE)) continue;
        if (o.IS_INFECTIOUS == 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
            set.Add(o.ICD_CODE);
    }
    return set;
}

// Trong FetchListRows, sau GetView:
var infectiousCodes = GetInfectiousIcdCodes();
data = data.Where(o => !string.IsNullOrEmpty(o.ICD_CODE) && infectiousCodes.Contains(o.ICD_CODE)).ToList();
```

**Hành vi:** panel danh sách **luôn chỉ hiển thị** điều trị có ICD chính truyền nhiễm. Nếu chưa ICD nào được gắn cờ `IS_INFECTIOUS=1` → danh sách rỗng (đúng nghiệp vụ).

> Lọc theo tập mã (client, từ cache) tương đương PA2; nếu sau này muốn server-side + paging chuẩn thì nâng lên **PA3-A** (`filter.IS_INFECTIOUS_ICD` + `EXISTS`) — §23b.5.

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




