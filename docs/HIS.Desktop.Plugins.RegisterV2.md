# RegisterV2 (Tiếp đón 2) — Tài Liệu Module

> Tài liệu khởi tạo tập trung vào nghiệp vụ kiểm tra cổng BHYT & cập nhật Địa chỉ liên hệ.
> Các phần khác của plugin chưa được khảo sát đầy đủ — bổ sung khi có thay đổi liên quan.

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.RegisterV2 |
| Loại | UC (màn hình Tiếp đón 2) |
| Mục đích | Tiếp nhận bệnh nhân: nhập thông tin BN, kiểm tra thẻ BHYT (cổng BHXH), đăng ký khám, tạm ứng, in phiếu |
| Cập nhật gần nhất | 02/06/2026 — huannh |

## 2. Quy Trình Nghiệp Vụ — Kiểm tra cổng BHYT & cập nhật thông tin

### Luồng cập nhật theo cổng BHYT
```
Nhân viên check thẻ BHYT (cổng BHXH) qua Library CheckHeinGOV
  → Cổng phát hiện thông tin nhập (họ tên / địa chỉ / nơi ĐKBĐ...) khác cổng
    → HeinGOVManager hiển thị dialog xác nhận
      → Nhân viên nhấn "Đồng ý"  (set IsThongTinNguoiDungThayDoiSoVoiCong__Choose = true)
        → UCRegister.CheckTTFull cập nhật thông tin theo cổng:
            • Họ tên, ngày sinh, giới tính, thông tin BHYT (ucHeinInfo) — luôn cập nhật
            • Địa chỉ liên hệ (Tỉnh/Huyện/Xã/Địa chỉ chi tiết) — theo cấu hình bên dưới
```

### Cấu hình: giữ Địa chỉ liên hệ khi cập nhật theo cổng BHYT

| KEY | `MOS.HIS_DESKTOP_PLUGINS_REGISTERV2.KEEP_CONTACT_ADDRESS_ON_BHYT_CHECK` |
|-----|------------------------------------------------------------------------|
| Loại | HisConfigs (HIS_CONFIG — toàn viện), kiểu string |
| Đọc khi | Mở màn hình Tiếp đón 2 (constructor `UCRegister`) → field `isKeepContactAddressOnBhytCheck` |
| = 1 (BẬT) | Giữ nguyên Địa chỉ liên hệ (Tỉnh + Huyện + Xã + Địa chỉ chi tiết) nhân viên đã nhập. Các thông tin khác (họ tên, ngày sinh, giới tính, BHYT) **vẫn cập nhật** theo cổng |
| ≠ 1 (mặc định, kể cả chưa khởi tạo = 0) | Giữ hành vi hiện tại: ghi đè toàn bộ địa chỉ liên hệ theo địa chỉ trên thẻ BHYT cổng trả về |

**Điều kiện nghiệp vụ:**
- Quyết định rẽ nhánh đặt **trước** điều kiện ghi đè địa chỉ đã có (`IsAddress || IsThongTinNguoiDungThayDoiSoVoiCong__Choose` + `CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong == 1`). Điều kiện cũ **không đổi** → backward compatible tuyệt đối khi config = 0.
- Không thay đổi giao diện.

## 3. EFMODEL / Dữ liệu liên quan

| Entity / Type | Mục đích |
|---------------|----------|
| ResultHistoryLDO (His.Bhyt.InsuranceExpertise.LDO) | Dữ liệu thẻ trả về từ cổng BHXH — field `diaChi`, `hoTen`, `maThe`, `maDKBD`, `gioiTinh`, `ngaySinh` |
| HeinCardData (Inventec.Common.QrCodeBHYT) | Dữ liệu thẻ BHYT |
| HIS.UC.AddressCombo.ADO.UCAddressADO | DTO địa chỉ Tỉnh/Huyện/Xã + Địa chỉ chi tiết (control `ucAddressCombo1`) |
| V_SDA_PROVINCE / V_SDA_DISTRICT / V_SDA_COMMUNE | Danh mục địa giới hành chính (AddressProcessor tách địa chỉ) |

## 4. Dependencies

| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.CheckHeinGOV | Kiểm tra thẻ BHYT qua cổng BHXH, dialog xác nhận cập nhật theo cổng |
| HIS.Desktop.Plugins.Library.RegisterConfig | Cấu hình dùng chung của màn hình tiếp đón (HisConfigCFG, AppConfigs) |
| HIS.UC.AddressCombo | Combo Tỉnh/Huyện/Xã |
| HIS.UC.UCPatientRaw | Nhập thông tin bệnh nhân |

## 5. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 02/06/2026 | huannh | Thêm cấu hình `MOS.HIS_DESKTOP_PLUGINS_REGISTERV2.KEEP_CONTACT_ADDRESS_ON_BHYT_CHECK`: khi = 1 thì giữ nguyên Địa chỉ liên hệ nhân viên đã nhập khi đồng ý cập nhật thông tin theo cổng BHYT; mặc định (≠1) giữ hành vi cũ. Sửa `UCRegister__CheckHeinGOV.cs` (thêm nhánh guard trước khối ghi đè địa chỉ) + đọc config khi mở form trong `UCRegister.cs` |
| 02/06/2026 | huannh | Thêm mục menu In ấn "In gộp dịch vụ khám" (gọi **MPS000515**, gộp tất cả phòng khám của BN). Chỉ hiển thị khi config `HIS.REGISTERV2.PRINT_MERGED_EXAM_SERVICE` = 1. Sửa `Config/HisConfigCFG.cs` (method `IsEnablePrintMergedExamService()` đọc config on-demand), `RunV3/UCRegister__Print.cs` (enum `InGopDvKham` + menu item + `DelegateRunPrinterInGopDichVuKham` gọi `Mps000515PDO`), resource key `Plugin_Register_Title_InGopDichVuKham` (vi/en/my) + `ResourceMessage.Title_InGopDichVuKham`, csproj reference `MPS.Processor.Mps000515.PDO` |

## 6. Test Cases

- [ ] Config chưa khởi tạo (không có key) → check cổng + Đồng ý → địa chỉ ghi đè theo cổng (hành vi cũ)
- [ ] Config = 0 → check cổng + Đồng ý → địa chỉ ghi đè theo cổng (hành vi cũ)
- [ ] Config = 1 → nhập địa chỉ liên hệ tay → check cổng + Đồng ý → Tỉnh/Huyện/Xã/Địa chỉ chi tiết **giữ nguyên**; họ tên/ngày sinh/giới tính/BHYT vẫn cập nhật theo cổng
- [ ] Config = 1 → đổi giá trị config khi đang mở form → chỉ có hiệu lực sau khi mở lại màn hình (config đọc lúc mở form)

### In gộp dịch vụ khám
- [ ] Config `HIS.REGISTERV2.PRINT_MERGED_EXAM_SERVICE` ≠ 1 → mở menu In ấn → KHÔNG có mục "In gộp dịch vụ khám"
- [ ] Config = 1 → mở menu In ấn → có mục "In gộp dịch vụ khám" ngay dưới "In dịch vụ khám", trên "In phiếu yêu cầu khám"
- [ ] Config = 1 → chưa đăng ký khám / đang ở chế độ thêm mới → chọn mục → hiện thông báo "không có dữ liệu đăng ký khám" và dừng
- [ ] Config = 1 → đã đăng ký nhiều phòng khám → chọn mục → phiếu MPS000515 gộp đủ tất cả phòng khám của BN
- [ ] Checkbox "Xem trước" bật → hiện preview; tắt + CheDoInChoCacChucNangTrongPhanMem = 2 → in trực tiếp

## 7. Print — In ấn

| Loại in | PrintTypeCode | MPS / PDO | Ghi chú |
|---------|--------------|-----------|---------|
| In dịch vụ khám (phòng được chọn) | Mps000001 | PrintServiceReqProcessor | Không đổi |
| In gộp dịch vụ khám (tất cả phòng) | Mps000515 | Mps000515PDO | Chỉ khi config `HIS.REGISTERV2.PRINT_MERGED_EXAM_SERVICE` = 1 |
| In phiếu yêu cầu khám | Mps000309 | Mps000309PDO | |
| In thẻ bệnh nhân | Mps000178 | Mps000178PDO | |
| Bảng kiểm trước tiêm chủng | Mps000358 | Mps000358PDO | |
| In biên lai/hóa đơn | Mps000420 | Mps000420PDO | |

**Gọi PDO MPS000515 (In gộp dịch vụ khám):** RegisterV2 gọi
`Mps000515PDO(V_HIS_PATIENT currentPatient, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, HIS_TREATMENT treatment, List<V_HIS_SERVICE_REQ> serviceReqs, List<V_HIS_SERE_SERV> sereServs, string gate)`
với `serviceReqs`/`sereServs` = toàn bộ phòng khám BN đã đăng ký lần này (lấy từ `currentHisExamServiceReqResultSDO`, không lọc theo dòng chọn), `gate` = số quầy từ `txtGateNumber`.
