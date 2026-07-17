# Sửa Thông Tin Khám Sức Khỏe V2 — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.EnterKskInfomantionVer2 |
| Loại | Form |
| Mục đích | Nhập / cập nhật thông tin khám sức khỏe phiên bản 2 — bao gồm KSK chung, KSK trên 18 tuổi, KSK dưới 8 tuổi, KSK lái xe, KSK định kỳ, KSK nghề nghiệp, KSK khác. Dữ liệu lưu vào HIS_KSK_GENERAL, HIS_KSK_OCCUPATIONAL, HIS_KSK_DRIVER_CAR, HIS_KSK_OVER_EIGHT, HIS_KSK_UNDER_EIGHT, HIS_KSK_OTHER. |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

Form mở từ ServiceReq yêu cầu khám sức khỏe. Gồm nhiều tab tương ứng các loại KSK.
Mỗi tab cho phép nhập: tiền sử bệnh, nghề nghiệp, DHST, khám 11 chuyên khoa + xếp loại, kết quả CLS, kết luận chung.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_KSK_GENERAL | Table | KSK chung |
| HIS_KSK_OCCUPATIONAL | Table | KSK nghề nghiệp |
| HIS_KSK_DRIVER_CAR | Table | KSK lái xe |
| HIS_KSK_OVER_EIGHT | Table | KSK trên 18 tuổi |
| HIS_KSK_UNDER_EIGHT | Table | KSK dưới 8 tuổi |
| HIS_KSK_OTHER | Table | KSK khác |
| HIS_DHST | Table | Dấu hiệu sinh tồn |
| V_HIS_SERVICE_REQ | View | Yêu cầu dịch vụ |

## 4. UI Layout

Form chính `frmEnterKskInfomantionVer2` chứa `xtraTabControl` với nhiều `xtraTabPage`. Mỗi tab bind tới 1 partial class (file `frmEnterKskInfomantionVer2___*.cs`).

Section 10 ("Nghề, công việc trước đây") trong tab General gồm:
- a. Công việc 1 (txtRecentWorkOne) — TextEdit
- a. Thời gian làm việc 1 (spnRecentWordOneYear / spnRecentWorkOneMonth)
- a. Ngày từ — đến 1 (dteRecentWorkOneFrom / dteRecentWorkOneTo)
- b. Công việc 2 (txtRecentWorkTwo) — TextEdit
- b. Thời gian làm việc 2 (spnRecentWorkTwoYear / spnRecentWorkTwoMonth)
- b. Ngày từ — đến 2 (dteRecentWorkTwoFrom / dteRecentWorkTwoTo)

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Get KSK General | api/HisKskGeneral/Get | MosConsumer |
| Get KSK Occupational | api/HisKskOccupational/Get | MosConsumer |
| Get HIS_DHST | api/HisDhst/Get | MosConsumer |
| Save V2 | api/HisServiceReq/KskExecuteV2 | MosConsumer |

## 6. Dependencies

Không có inter-plugin trực tiếp.

## 7. Print

MPS printers (xem `frmEnterKskInfomantionVer2___PrintMPS.cs`).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 28/05/2026 | anhnh2 | Bổ sung 2 ô text "Công việc:" (`txtRecentWorkOne`, `txtRecentWorkTwo`) vào mục 10 "Nghề, công việc trước đây" trong tab General, hiển thị PHÍA TRÊN ô "Thời gian làm việc". Load/save vào cột `RECENT_WORK_ONE` và `RECENT_WORK_TWO` của bảng `HIS_KSK_GENERAL`. Dịch các LayoutControlItem trong `layoutControlGroup4` (Job-2 và Section 12 trở xuống) xuống thêm 48px. Mở rộng `Group4.Size.Height` từ 587 → 635. |
| 29/05/2026 | anhnh2 | Fix bug load tab "KSK dưới 18 tuổi" (`frmEnterKskInfomantionVer2___UnderEight.cs`): (1) 5 combo người khám (Tuần hoàn, Mắt, TMH, RHM, Cận lâm sàng) load sai entity — đọc từ `currentKskGeneral` thay vì `currentKskUnderEight` → save vào `HIS_KSK_UNDER_EIGHTEEN` nhưng load đọc `HIS_KSK_GENERAL` → mất giá trị sau khi mở lại; (2) `cboExamClinicalOtherLoginName3` load sai cột `EXAM_SUBCLINICAL_LOGINNAME` → đúng là `EXAM_CLINICAL_OTHER_LOGINNAME`. Đã sửa 6 dòng trong `FillDataUnderEighteen`. |
| 30/06/2026 | huannh | (R5/R8 — phần FE) Bổ sung cụm chọn mã ICD tiền sử: thêm UC tái sử dụng `UcKskHistoryIcd` (ô mã + ô tên chỉ đọc, ghép `;` + nút `...` mở popup multi-select `HIS.UC.SecondaryIcd.frmSecondaryIcd`); enum `KskHistoryGroup` (5 nhóm); partial `frmEnterKskInfomantionVer2___HistoryIcd.cs` (nhúng cụm bằng code lúc Load, đồng bộ 1 giá trị/nhóm giữa các tab, cảnh báo R8 khi có nội dung mà chưa chọn ICD nhưng vẫn cho lưu). Đã làm BẢN MẪU cho tab "Ksk định kỳ" (General): nhóm Bản thân (cạnh `txtPathologicalHistory`), Nghề nghiệp (cạnh `txtOccuOne`), Sản khoa (cạnh `txtExamObstetric`). Thêm key đa ngôn ngữ `KskHistoryIcd.*` vào `Lang.vi/en.resx`. |
| 30/06/2026 | huannh | (R5/R8 — hoàn thiện) BE đã bổ sung 10 cột vào `HIS_KSK_GENERAL`: `FAMILY_HISTORY_ICD_CODE/NAME`, `PERSONAL_HISTORY_ICD_CODE/NAME`, `OCCUPATIONAL_DISEASE_ICD_CODE/NAME`, `OBSTETRIC_DISEASE_ICD_CODE/NAME`, `TREATING_DISEASE_ICD_CODE/NAME`. Đã nối LƯU (`FillKskHistoryIcdToGeneral`) + ĐỌC (`LoadKskHistoryIcdFromGeneral`) thật cho cả 5 nhóm. NHÂN BẢN cụm chọn ICD theo B.4.1 — CHỈ 5 tab có mục tiền sử (gắn cạnh ô text hiện có), KHÔNG gồm 2 tab lái xe: General (Bản thân/Nghề nghiệp/Sản khoa), OverEighteen + UnderEight (Gia đình/Bản thân/Sản khoa), Occupational (Gia đình/Bản thân/Sản khoa), UnderSix (Gia đình/Bản thân). Nhóm "Đang điều trị" chưa nhúng cụm (không có ô text tự do) — cột `TREATING_DISEASE_ICD_*` vẫn được lưu/đọc, chờ chốt vị trí hiển thị. Khi lưu (sửa ICD) truyền kèm `HIS_KSK_GENERAL.KSK_TYPE_ID` (loại mẫu KSK) — giữ nguyên giá trị đã lưu sẵn từ `currentKskGeneral` (`SetKskTypeIdToGeneral`). |
| 02/07/2026 | huannh | Bổ sung trường "Người khám" ở phần Kết luận cho tab "Ksk trên 18 tuổi" (cạnh `cboHealthExamRank2`) và "Ksk dưới 18 tuổi" (cạnh `panel3`) — nhúng GridLookUpEdit bằng code (`frmEnterKskInfomantionVer2___Concluder.cs`), cấu hình danh sách người khám qua `SetDataCboExamLoginName`. Lưu vào `HIS_KSK_GENERAL.CONCLUDER_LOGINNAME` + `CONCLUDER_USERNAME` (giống tab định kỳ; `CONCLUDER_USERNAME` lấy `V_HIS_EMPLOYEE.TDL_USERNAME` theo loginname). Load từ `currentKskGeneral.CONCLUDER_LOGINNAME`. Thêm key `KskConcluder.Caption` vào Lang.vi/en.resx. |
| 02/07/2026 | huannh | Fix combo "Người khám" mới: (1) gán `GridView` cho GridLookUpEdit tạo runtime (thiếu View nên popup không chọn được ở tab trên 18); (2) đổi anchor tab dưới 18 từ `panel3` (container) sang `txtProblemHealth3` (ô kết luận "Sức khỏe có vấn đề" ở V. KẾT LUẬN) để combo hiển thị đúng chỗ. Thêm nút Combo/Delete + `ClearData_ButtonClick`. |
| 03/07/2026 | huannh | Đổi cách nhúng cụm ICD tiền sử + combo Người khám: KHÔNG chèn runtime vào LayoutControl (làm vỡ layout bố cục cố định) → nhúng vào **PanelControl host đặt sẵn trong Designer**, tìm theo tên bằng `Controls.Find` (`EmbedHistoryIcdIntoPanel`/`EmbedConcluderComboIntoPanel` + `FindHostControl`) rồi `panel.Controls.Add(uc/cbo); Dock=Fill`. Panel chưa đặt → bỏ qua an toàn (compile được, không lỗi). Tên panel: `pnlKskIcd{Family/Personal/Occupational/Obstetric}{tabIndex}` (tab 0/1/2/6/7) + `pnlKskConcluder1`, `pnlKskConcluder2`. CẦN đặt các panel này trong Designer để control hiển thị. |
| 02/07/2026 | huannh | (HIS_KSK_SYNC) Chốt: khi lưu KSK V2, việc tạo bản ghi `HIS_KSK_SYNC` (lấy `KSK_TYPE_ID` = loại mẫu, set `SYNC_RESULT_TYPE = 0` = chưa gửi đồng bộ) do **BE tự sinh từ `HIS_KSK_GENERAL`** — FE KHÔNG phát sinh code mới, chỉ cần đảm bảo `HIS_KSK_GENERAL.KSK_TYPE_ID` được gửi kèm (đã có qua `SetKskTypeIdToGeneral`). |

## 9. Test Cases

### Mục 10 — Nghề, công việc trước đây
- [ ] Form mở → ô "Công việc 1" và "Công việc 2" hiện ở phía trên ô "Thời gian làm việc" tương ứng
- [ ] Nhập tên công việc 1 + thời gian + ngày từ-đến → Lưu → mở lại → hiển thị đầy đủ
- [ ] Nhập cả công việc 2 → Lưu → mở lại → hiển thị đầy đủ
- [ ] Bỏ trống cả 2 ô công việc → Lưu → load lại → cột RECENT_WORK_ONE/TWO = NULL
- [ ] Layout: section "12. Tiền sử bản thân" và các sections phía sau hiển thị đúng vị trí mới
- [ ] Tab order: focus chạy đúng thứ tự từ tên công việc → thời gian → ngày
