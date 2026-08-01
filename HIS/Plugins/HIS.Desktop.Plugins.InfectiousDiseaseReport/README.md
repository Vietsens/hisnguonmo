# HIS.Desktop.Plugins.InfectiousDiseaseReport

Plugin đẩy **ca bệnh truyền nhiễm** từ HIS lên **Cổng giám sát quốc gia (ECDS)** — `daotao-gs.vadp.gov.vn`.
Tài liệu thiết kế đầy đủ: `hisnguonmo/docs/HIS.Desktop.Plugins.InfectiousDiseaseReport.md` (+ `.docx`).

## Đã tạo (khung code)

| Tầng | File |
|------|------|
| Entry point | `InfectiousDiseaseReportProcessor.cs`, `ModuleLinkString.cs` |
| Architecture | `InfectiousDiseaseReport/` (Interface, Factory, Behavior) |
| Enum | `EnumEcds.cs` — 9 enum chính thức + `EcdsPushState` (⚠ 2 enum polarity đảo: 0=Có) |
| ADO/DTO | `ADO/` — `EcdsDiseaseCaseDto` (tên trường UPPER_SNAKE), `KetQuaEcdsDto<T>`, `DangNhapResultDto`, `DanhMucItemDto`, `SearchDanhMucFastDto` |
| Config | `Config/EcdsConfigCFG.cs` — đọc HisConfigs |
| Worker | `Worker/` — `EcdsApiWorker` (login/danh mục/đẩy), `EcdsTokenStore`, `EcdsCatalogCache`, `DiseaseCaseMapper` |
| Resources | `Resources/` — ResourceMessage + Message.Lang.vi/en.resx |
| Form (đầy đủ) | `MainForm/frmInfectiousDiseaseReport.*` — UI trong `Designer.cs` (InitializeComponent, KHÔNG dựng runtime), **2 tab theo QĐ 4039** (Đối tượng mắc bệnh / Trường hợp bệnh), luồng `Load → InitCombo → FillDataFromHis → ValidateForm → SaveToHisProcess/PushProcess` |
| csproj | `HIS.Desktop.Plugins.InfectiousDiseaseReport.csproj` — reference DevExpress 15.2 + Inventec/HIS + Newtonsoft (⚠ kiểm tra path Newtonsoft.Json) |
| Metadata | `Properties/AssemblyInfo.cs` |

### Luồng chức năng đã hiện thực
1. Mở Form từ 1 điều trị (Behavior nhận `HIS_TREATMENT`) → header + các tab tự điền từ HIS.
2. Nạp danh mục ECDS (login → cache) vào combo; enum bind theo `EnumEcds`.
3. Nút **Đẩy lên cổng**: validate trường bắt buộc → xác nhận → `EcdsApiWorker.DayCaBenh` → đối soát `maCaBenh` → gọi `dlgRefresh`.
4. Kết nối trực tiếp HTTPS (TLS 1.2), token cache theo phiên, log đầy đủ.

## CẦN LÀM TIẾP (trong Visual Studio)

1. **`.csproj`** — tạo project, thêm reference theo HintPath chuẩn:
   - `MOS.EFMODEL`, `SDA.EFMODEL`, `IMSys.DbConfig.*`, `Inventec.*`, `HIS.Desktop.Utility`, `HIS.Desktop.LocalStorage`, `HIS.Desktop.Common`, `Newtonsoft.Json`, DevExpress 15.2.
2. **Thiết kế UI 2 tab** (Đối tượng mắc bệnh / Trường hợp bệnh) — khớp 2 object `DOI_TUONG_MAC_BENH` / `TRUONG_HOP_BENH` của QĐ 4039. Áp `LayoutControl` 2 cột, nhãn maroon cho trường bắt buộc, `SetCaptionByLanguageKey`, `ControlState`.
3. **Form danh sách** `ListForm/frmInfectiousDiseaseReportList` — grid + đẩy hàng loạt (`cap-nhat-nhieu`) + auto-push (Timer) + đối soát.
4. **Bảng lưu** `HIS_ECDS_DISEASE_CASE` ở backend MOS + API CRUD; hoàn thiện lưu trạng thái đẩy/đối soát. (2 bảng con thuốc sốt rét / lịch sử di chuyển đã BỎ theo QĐ 4039 — xem docs §17.2.)
5. **DiseaseCaseMapper** — hoàn thiện map mã HIS → **ID ECDS** qua `EcdsCatalogCache`:
   - Nghề nghiệp: `HIS_CAREER` → `nghenghiep`
   - Dân tộc: `SDA_NATIONAL` → `dantoc`
   - Cấp độ bệnh: danh mục **liên thông** → `capdobenh`
   - Cơ sở điều trị (tên): `HIS_BRANCH.BRANCH_NAME`
   - Địa bàn: `HT_COMMUNE_CODE`/`COMMUNE_CODE` (GSO) → `xa/tinh` ID ECDS

## Còn treo (chờ tài liệu ECDS)
- Enum `TINHTRANGRAVIEN`.
- Sub-schema 2 mảng: `danhSachThuocSotRet`, `lichSuDiChuyenDichTe`.
- Xác nhận endpoint: `fast/v1` (camelCase) hay template import (UPPER_SNAKE) → chỉnh `[JsonProperty]` cho khớp.

## Cấu hình (HisConfigs)
`ECDS.API.BASE_URL`, `ECDS.API.USERNAME`, `ECDS.API.PASSWORD`, `ECDS.API.MA_DON_VI`, `ECDS.API.MA_CO_SO_DIEU_TRI`, `ECDS.API.TIMEOUT_SECOND`.
