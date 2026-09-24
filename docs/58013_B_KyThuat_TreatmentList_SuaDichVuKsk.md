# Tài liệu kỹ thuật — PT-58013 (Tài liệu 3395)

**Cho phép thêm, xóa dịch vụ, sửa phòng thực hiện các dịch vụ trong hợp đồng khám sức khỏe**

| Thông tin | Giá trị |
|---|---|
| Module chính | `HIS.Desktop.Plugins.TreatmentList` — Hồ sơ điều trị |
| Module tham chiếu | `HIS.Desktop.Plugins.AssignServiceEdit` — Sửa chỉ định dịch vụ (KHÔNG sửa) |
| Module mới (đề xuất) | `HIS.Desktop.Plugins.KskServiceEditList` — Sửa dịch vụ KSK nhiều bệnh nhân |
| Backend | MOS — API mới `api/HisKskContract/ServiceEdit` |

---

## 1. Hiện trạng code

| Thành phần | Hiện trạng | Vị trí |
|---|---|---|
| Lọc hợp đồng KSK | Có sẵn `cboContract` → `filter.TDL_KSK_CONTRACT_ID` | `UCTreatmentList.cs` ~L1068 (`FillDataToGridTreatment`) |
| Load combo hợp đồng | `InitComboContract()` gọi `api/HisKskContract/GetView` | `UCTreatmentList.cs` ~L462 |
| Chọn nhiều dòng | Grid multi-select; `btnPrintfKSK` bật khi có dòng chọn | `UCTreatmentList.cs` ~L1794 (`gridViewtreatmentList_SelectionChanged`) |
| Reset bộ lọc | `SetDefaultControl()` gán `cboContract.EditValue = null` | `UCTreatmentList.cs` ~L595–L640 |
| Tìm theo mã hồ sơ | Khi nhập mã điều trị → gọi `SetDefaultControl()` → bỏ lọc hợp đồng | `UCTreatmentList.cs` ~L946–L958 |
| Phân trang | Server-side `ucPaging1` | `UCTreatmentList.cs` ~L912–L927 |
| Dòng dữ liệu grid | `V_HIS_TREATMENT_4` có `TDL_KSK_CONTRACT_ID`, `IS_PAUSE`, `IS_LOCK_FEE` | EFMODEL |
| AssignServiceEdit | Làm việc trên **1 phiếu** (`serviceReqId`), lưu qua `HisServiceReq/Update` | `AssignServiceEditBehavior.cs` |
| Import KSK (BE) | Gom dịch vụ thành phiếu theo **(phòng, loại y lệnh)**; `REQUEST_ROOM_ID` = phòng người import | `HisKskContract/Import/ServiceReqProcessor.cs` |
| Đổi phòng hàng loạt (BE) | Có sẵn `ChangeListRoom(ServiceReqIds, ExecuteRoomId)` — all-or-nothing | `HisServiceReq/Common/Update/ChangeListRoom` |

---

## 2. Bổ sung nút "Sửa dịch vụ" tại màn hình Hồ sơ điều trị

### 2.1. Quy tắc hiển thị nút (BỔ SUNG)

> Nút **"Sửa dịch vụ"** CHỈ hiển thị khi danh sách trên grid đang được lọc theo **hợp đồng khám sức khỏe**.

| Trạng thái | Nút "Sửa dịch vụ" |
|---|---|
| Chưa chọn hợp đồng ở `cboContract` | **Ẩn** |
| Đã chọn hợp đồng nhưng CHƯA nhấn Tìm kiếm | **Ẩn** (grid vẫn đang hiển thị kết quả cũ, không theo hợp đồng) |
| Đã chọn hợp đồng VÀ đã Tìm kiếm (grid load theo hợp đồng) | **Hiện** |
| Tìm theo mã điều trị / mã khác (hệ thống tự bỏ lọc hợp đồng) | **Ẩn** |
| Nhấn Làm lại / reset bộ lọc (`SetDefaultControl`) | **Ẩn** |
| Xóa giá trị `cboContract` rồi Tìm kiếm | **Ẩn** |
| Người dùng không có quyền (control code) | **Ẩn** trong mọi trường hợp |

**Lý do dựa trên bộ lọc ĐÃ ÁP DỤNG, không dựa trên giá trị combo:** nếu người dùng đổi `cboContract` mà chưa tìm lại, các dòng đang tick vẫn là kết quả của lần tìm trước → có thể không thuộc hợp đồng đang chọn. Vì vậy trạng thái nút phải bám theo hợp đồng đã được dùng để load grid.

### 2.2. Quy tắc bật/tắt và kiểm tra khi nhấn

- Khi nút đang hiện → luôn **Enabled** (KHÔNG disable theo số dòng tick), để khi chưa tick vẫn nhấn được và nhận cảnh báo đúng Kịch bản 7.
- Khi nhấn nút, kiểm tra theo thứ tự:

| # | Điều kiện | Xử lý |
|---|---|---|
| 1 | Chưa tick bệnh nhân nào | Cảnh báo "Chưa chọn bệnh nhân", KHÔNG mở màn hình |
| 2 | Có dòng `TDL_KSK_CONTRACT_ID` null (không phải BN KSK hợp đồng) | Cảnh báo, liệt kê mã điều trị, KHÔNG mở |
| 3 | Có dòng `TDL_KSK_CONTRACT_ID` ≠ hợp đồng đang lọc | Cảnh báo, liệt kê mã điều trị, KHÔNG mở |
| 4 | Hợp lệ | Mở module `KskServiceEditList`, truyền danh sách `TreatmentId` + `KskContractId` + callback refresh |

> Hồ sơ đã kết thúc / đã khóa viện phí: KHÔNG chặn ở bước mở màn hình. Backend trả lý do theo từng bệnh nhân khi lưu (Kịch bản 6).

### 2.3. Thiết kế code

**Field mới** (`UCTreatmentList.cs`):

```csharp
/// <summary>
/// KSK contract id that was applied to the current grid data.
/// Null when the grid is not filtered by a KSK contract.
/// </summary>
private long? appliedKskContractId = null;
```

**Gán khi build filter** — trong `FillDataToGridTreatment`, ngay sau khối gán `filter.TDL_KSK_CONTRACT_ID`:

```csharp
// Remember the contract filter actually applied to the grid
this.appliedKskContractId = filter.TDL_KSK_CONTRACT_ID;
```

> Phải gán từ `filter` (không đọc `cboContract.EditValue`) vì nhánh tìm theo mã điều trị tạo lại `filter` mới và reset combo.

**Cập nhật hiển thị** — gọi sau khi load grid xong (cuối `FillDataToGridTreatment`) và trong `SetDefaultControl()`:

```csharp
private void SetVisibleBtnKskServiceEdit()
{
    try
    {
        bool isVisible = this.appliedKskContractId.HasValue
            && this.appliedKskContractId.Value > 0
            && this.IsAllowKskServiceEdit();   // control code permission

        lciBtnKskServiceEdit.Visibility = isVisible
            ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
            : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}
```

**Reset** — trong `SetDefaultControl()` (cạnh `cboContract.EditValue = null;`):

```csharp
this.appliedKskContractId = null;
SetVisibleBtnKskServiceEdit();
```

**Click** (`UCTreatmentList__Item_Click.cs` hoặc partial mới `UCTreatmentList___KskServiceEdit.cs`):

```csharp
private void btnKskServiceEdit_Click(object sender, EventArgs e)
{
    try
    {
        if (lciBtnKskServiceEdit.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always
            || !this.appliedKskContractId.HasValue)
            return;

        var selectedRows = gridViewtreatmentList.GetSelectedRows();
        if (selectedRows == null || !selectedRows.Any())
        {
            XtraMessageBox.Show(Resources.ResourceMessage.ChuaChonBenhNhan,
                Resources.ResourceMessage.Thongbao);
            return;
        }

        var treatments = selectedRows
            .Select(i => gridViewtreatmentList.GetRow(i) as V_HIS_TREATMENT_4)
            .Where(o => o != null)
            .ToList();

        var invalidCodes = treatments
            .Where(o => o.TDL_KSK_CONTRACT_ID != this.appliedKskContractId)
            .Select(o => o.TREATMENT_CODE)
            .ToList();
        if (invalidCodes.Any())
        {
            XtraMessageBox.Show(String.Format(
                Resources.ResourceMessage.HoSoKhongThuocHopDongKsk,
                String.Join(", ", invalidCodes)),
                Resources.ResourceMessage.Thongbao);
            return;
        }

        Inventec.Common.Logging.LogSystem.Debug(
            Inventec.Common.Logging.LogUtil.TraceData(
                Inventec.Common.Logging.LogUtil.GetMemberName(() => this.appliedKskContractId), this.appliedKskContractId)
            + Inventec.Common.Logging.LogUtil.TraceData("treatmentCount", treatments.Count));

        // Args: List<V_HIS_TREATMENT_4>, V_HIS_KSK_CONTRACT, RefeshReference
        OpenKskServiceEditList(treatments, contract);
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}
```

**Designer** (`UCTreatmentList.Designer.cs`):

| Control | Giá trị |
|---|---|
| `btnKskServiceEdit` | `SimpleButton`, Text "Sửa dịch vụ", ToolTip "Thêm, xóa dịch vụ, sửa phòng thực hiện cho các bệnh nhân KSK hợp đồng đã chọn" |
| `lciBtnKskServiceEdit` | LayoutControlItem, đặt cạnh `layoutControlItem23` (nút "In KSK"), `TextVisible = false`, mặc định `Visibility = Never` |

**Không sửa** `gridViewtreatmentList_SelectionChanged` cho nút mới (nút không phụ thuộc số dòng tick — mục 2.2).

### 2.4. Phân quyền

- Bổ sung control code `HIS000060` trong `Base/ControlCode.cs` — "Sửa dịch vụ KSK hợp đồng". Chưa khai báo ACS_CONTROL này → không kiểm soát (nút hiện theo điều kiện lọc); đã khai báo → người dùng phải có control qua vai trò.
- `IsAllowKskServiceEdit()` kiểm tra theo cùng pattern `controlDelete` hiện có (IS_ANONYMOUS hoặc role được gán).

### 2.5. Đa ngôn ngữ

| Key | vi | en |
|---|---|---|
| `UCTreatmentList.btnKskServiceEdit.Text` | Sửa dịch vụ | Edit services |
| `UCTreatmentList.btnKskServiceEdit.ToolTip` | Thêm, xóa dịch vụ, sửa phòng thực hiện cho các bệnh nhân KSK hợp đồng đã chọn | Add, remove services and change execute room for selected KSK contract patients |
| Message `ChuaChonBenhNhan` | Chưa chọn bệnh nhân. | No patient selected. |
| Message `HoSoKhongThuocHopDongKsk` | Các hồ sơ sau không thuộc hợp đồng khám sức khỏe đang lọc: {0} | The following treatments do not belong to the filtered KSK contract: {0} |

### 2.6. Kịch bản kiểm thử

| # | Bước | Kết quả mong đợi |
|---|---|---|
| T1 | Mở màn hình, không chọn hợp đồng, Tìm kiếm | Nút ẩn |
| T2 | Chọn hợp đồng, CHƯA Tìm kiếm | Nút ẩn |
| T3 | Chọn hợp đồng → Tìm kiếm | Nút hiện |
| T4 | Từ T3, đổi sang hợp đồng khác, chưa Tìm kiếm → nhấn nút | Nút vẫn hiện theo hợp đồng cũ; dữ liệu mở theo hợp đồng cũ (đã áp dụng) |
| T5 | Từ T3, xóa `cboContract` → Tìm kiếm | Nút ẩn |
| T6 | Từ T3, nhập mã điều trị → Enter | Nút ẩn (filter hợp đồng bị bỏ) |
| T7 | Từ T3, nhấn Làm lại | Nút ẩn |
| T8 | Từ T3, không tick dòng nào → nhấn nút | Cảnh báo chưa chọn bệnh nhân, không mở màn hình |
| T9 | Từ T3, tick nhiều dòng → nhấn nút | Mở màn hình "Sửa dịch vụ", hiển thị đúng số bệnh nhân |
| T10 | Tài khoản không có quyền, làm như T3 | Nút ẩn |
| T11 | Chuyển trang (paging) khi đang lọc hợp đồng | Nút vẫn hiện |

---


## 3. Quyết định nghiệp vụ (đã chốt)

Nguyên tắc chung: **đổi phòng theo từng dịch vụ**; các quy tắc còn lại **tham chiếu chức năng `HIS.Desktop.Plugins.HisImportKsk`** (API `api/HisKskContract/Import`) để dịch vụ thêm/sửa sau import có cùng giá, đối tượng, phòng như lúc import.

| # | Câu hỏi | Quyết định | Căn cứ trong HisImportKsk |
|---|---|---|---|
| Q1 | Sửa phòng theo phiếu hay dịch vụ | **Theo từng dịch vụ.** Phiếu chỉ có 1 dịch vụ → đổi phòng cả phiếu; phiếu có nhiều dịch vụ → tách dịch vụ sang phiếu mới tại phòng mới | Import gom phiếu theo (phòng, loại y lệnh) qua `HisServiceReqRoomAssign` |
| Q2 | Người sửa khác người import | **Cho phép**, giống Import: người dùng chọn **Người chỉ định** (`Loginname/Username`), phòng chỉ định = phòng đang làm việc. API mới KHÔNG áp dụng kiểm tra "phiếu do người khác tạo". Kiểm soát bằng quyền module/API (ACS) + control code nút | `frmKsk.btnSave_Click`: `kskContractSdo.Loginname = cboLogin`, `RequestRoomId = currentModule.RoomId` |
| Q3 | Đối tượng, giá dịch vụ thêm mới | Dịch vụ thêm **phải thuộc một Nhóm dịch vụ KSK** (`HIS_KSK` → `HIS_KSK_SERVICE`). Đối tượng = đối tượng hiện hành của hồ sơ (KSK); `HEIN_RATIO` = `KSK_CONTRACT.PAYMENT_RATIO`; `USER_PRICE` = `KSK_SERVICE.PRICE × (1 + VAT_RATIO)` nếu có giá; `AMOUNT` = `KSK_SERVICE.AMOUNT`; phòng mặc định = `KSK_SERVICE.ROOM_ID` | `ServiceReqProcessor.Run`, `MakeServiceReq`; cột "Mã nhóm dịch vụ bổ sung" + "Mã dịch vụ bổ sung" |
| Q4 | "Chọn tất cả" | Chỉ áp dụng cho **các dòng đã tick** (mục 3.5 tài liệu nghiệp vụ). Muốn áp dụng cả hợp đồng: tăng số dòng/trang của `ucPaging` rồi chọn tất cả | Import xử lý đúng danh sách được đưa vào |
| Q5 | "Đã thanh toán" | Dịch vụ có `HIS_SERE_SERV_BILL` chưa hủy **hoặc** `HIS_SERE_SERV_DEPOSIT` chưa hoàn → KHÔNG xóa, KHÔNG đổi phòng. Không phụ thuộc cấu hình `DO_NOT_ALLOW_TO_EDIT_IF_PAID` | AssignServiceEdit đã kiểm tra bill/deposit phía FE (`FormAssignServiceEdit.cs` ~L983–L994) |
| Q6 | Trùng dịch vụ | Cùng `SERVICE_ID`, cùng hồ sơ, `IS_DELETE = 0`, `IS_NO_EXECUTE` null. **KHÔNG** tính dịch vụ tương đương (`HIS_SERVICE_SAME`) | Import không dùng dịch vụ tương đương |
| Q7 | Kết quả xử lý | Xử lý **độc lập từng hồ sơ**, lỗi hồ sơ nào rollback hồ sơ đó; trả `Descriptions` theo từng bệnh nhân + bảng tổng hợp; cho xuất Excel danh sách lỗi | `HisKskContractImport.Run` (mỗi BN 1 `HisKskContractImportDetail`, rollback riêng) + `BuildSummary` (57616) |
| Q8 | Hiệu lực hợp đồng | Hợp đồng hết hạn / thời gian y lệnh ngoài hiệu lực → chặn thao tác **thêm** và **đổi phòng** | `frmKsk` kiểm tra `EXPIRY_DATE`; BE `IsValidContractTime` |
| Q9 | Xóa hết dịch vụ của một y lệnh | **Báo lỗi**, không xóa; hồ sơ đó rollback, lý do ghi trong kết quả (mục 4.4) | — |
| Q10 | Lô lớn (1000+ hồ sơ) | Frontend **chia lô 200 hồ sơ/lần gọi API**, gọi tuần tự, gộp kết quả (mục 5.4) | — |

---

## 4. Thiết kế Backend (MOS)

### 4.1. API mới

Đặt cùng nhóm với Import để dùng lại logic chuẩn bị dữ liệu KSK.

| Mục | Giá trị |
|---|---|
| URI | `api/HisKskContract/ServiceEdit` (POST, trả `ApiResultObject<HisKskServiceEditSDO>`) |
| Controller | `HisKskContractController` (bổ sung action `ServiceEdit`) |
| Manager | `MOS.MANAGER/HisKskContract/ServiceEdit/` — `HisKskContractServiceEdit.cs`, `HisKskContractServiceEditDetail.cs`, `HisKskContractServiceEditCheck.cs` |
| Event log | Thêm DV, Xóa DV: `EventLog` của y lệnh hiện có; Đổi phòng: `EventLog.Enum.HisServiceReq_DoiPhongThucHien` |

### 4.2. SDO

```csharp
public class HisKskServiceEditSDO
{
    // Input
    public long KskContractId { get; set; }
    public long RequestRoomId { get; set; }
    public string Loginname { get; set; }        // Người chỉ định (giống Import)
    public string Username { get; set; }
    public long IntructionTime { get; set; }      // Thời gian y lệnh cho DV thêm / phiếu tách
    public List<long> TreatmentIds { get; set; }
    public List<KskServiceAddSDO> AddServices { get; set; }
    public List<long> DeleteServiceIds { get; set; }
    public List<KskServiceChangeRoomSDO> ChangeRooms { get; set; }

    // Output
    public List<KskServiceEditResultSDO> Results { get; set; }
    public KskServiceEditSummarySDO Summary { get; set; }
}

public class KskServiceAddSDO
{
    public long KskId { get; set; }               // Nhóm dịch vụ KSK
    public long ServiceId { get; set; }
    public long? RoomId { get; set; }             // null -> KSK_SERVICE.ROOM_ID
}

public class KskServiceChangeRoomSDO
{
    public long ServiceId { get; set; }
    public long NewRoomId { get; set; }
}

public class KskServiceEditResultSDO
{
    public long TreatmentId { get; set; }
    public string TreatmentCode { get; set; }
    public string PatientName { get; set; }
    public string Action { get; set; }            // ADD / DELETE / CHANGE_ROOM
    public long ServiceId { get; set; }
    public string ServiceName { get; set; }
    public short ResultType { get; set; }         // 1: thành công, 2: bỏ qua, 3: lỗi
    public List<string> Descriptions { get; set; }
}

public class KskServiceEditSummarySDO
{
    public int TotalTreatment { get; set; }
    public int SuccessTreatment { get; set; }
    public int ErrorTreatment { get; set; }
    public int AddedCount { get; set; }
    public int SkippedDuplicateCount { get; set; }
    public int DeletedCount { get; set; }
    public int NotDeletedCount { get; set; }
    public int ChangedRoomCount { get; set; }
    public int NotChangedRoomCount { get; set; }
}
```

> `ResultType` dùng enum riêng có XML comment + giá trị tường minh (không hardcode số).

### 4.3. Luồng xử lý

```
ServiceEdit.Run(sdo)
  1. HasWorkPlaceInfo(RequestRoomId)
  2. Check chung: hợp đồng tồn tại, chưa khóa; TreatmentIds không rỗng;
     mọi hồ sơ có TDL_KSK_CONTRACT_ID = KskContractId
  3. Prepare 1 lần cho cả lô (KHÔNG query trong vòng lặp):
     - HIS_TREATMENT, SERE_SERV (IS_DELETE=0), SERVICE_REQ theo TreatmentIds
     - SERE_SERV_BILL, SERE_SERV_DEPOSIT, SERE_SERV_TEIN theo SereServIds
     - HIS_KSK_SERVICE theo (KskId, ServiceId) của AddServices
     - PATIENT_TYPE_ALTER hiện hành theo TreatmentIds
  4. foreach treatment -> Detail riêng (CommonParam clone), thứ tự:
       a. Xóa     (DeleteServiceIds)
       b. Đổi phòng (ChangeRooms)
       c. Thêm    (AddServices)
     Detail lỗi -> Rollback riêng hồ sơ đó, ghi Descriptions
  5. BuildSummary -> trả Results + Summary (kể cả khi có lỗi)

  Giới hạn: TreatmentIds tối đa 200 hồ sơ/lần gọi (FE chia lô, mục 5.4);
  vượt quá -> trả lỗi DuLieuDauVaoKhongHopLe
```

### 4.4. Quy tắc Xóa (mỗi hồ sơ, mỗi dịch vụ)

| Điều kiện | Kết quả |
|---|---|
| Hồ sơ không có dịch vụ | Bỏ qua — "Không có dịch vụ" |
| Hồ sơ đã kết thúc (`IS_PAUSE`) hoặc khóa viện phí (`IS_LOCK_FEE`) | Không xóa — lý do |
| Phiếu hoàn thành (`SERVICE_REQ_STT = HT`) | Không xóa — lý do |
| Phiếu đang xử lý (`DXL`) và cấu hình `ALLOW_MODIFYING_OF_STARTED` không cho phép | Không xóa — lý do |
| `SERE_SERV.EXECUTE_TIME` có giá trị | Không xóa — "Dịch vụ đã thực hiện" |
| Có `SERE_SERV_TEIN.VALUE` | Không xóa — "Xét nghiệm đã có kết quả" |
| Có bill chưa hủy / deposit chưa hoàn | Không xóa — "Dịch vụ đã thanh toán" |
| Dịch vụ cha có dịch vụ con / thuộc gói | Không xóa — dùng message hiện có `HisSereServ_TonTaiDichVuCon`, `HisSereServ_CacDichVuTrongGoi` |
| Sau khi xóa, phiếu không còn dịch vụ nào (xóa hết dịch vụ của phiếu) | **Báo lỗi**, KHÔNG xóa — "Không cho phép xóa hết dịch vụ của y lệnh {Mã y lệnh}". Hồ sơ đó rollback toàn bộ thay đổi (Q7) |
| Hợp lệ | Xóa mềm `SERE_SERV` |

### 4.5. Quy tắc Đổi phòng theo từng dịch vụ

| Bước | Xử lý |
|---|---|
| 1 | Tìm `SERE_SERV` của `ServiceId` trong hồ sơ. Không có → Bỏ qua |
| 2 | Phiếu hiện tại đã ở `NewRoomId` → Bỏ qua — "Đã ở phòng mới" |
| 3 | Áp dụng TOÀN BỘ điều kiện chặn của mục 4.4 (dịch vụ đã thực hiện tại phòng cũ giữ nguyên) |
| 4 | Kiểm tra `HIS_SERVICE_ROOM(ServiceId, NewRoomId)` còn hiệu lực (như `ValidateRoom` của Import) |
| 5a | Phiếu chỉ chứa dịch vụ này → cập nhật `EXECUTE_ROOM_ID`, `EXECUTE_DEPARTMENT_ID` của phiếu; cập nhật bộ đếm phòng (`HisRoomCounterCFG`) như `HisServiceReqUpdateChangeRoom` |
| 5b | Phiếu có nhiều dịch vụ → xóa `SERE_SERV` khỏi phiếu cũ, tạo phiếu mới tại `NewRoomId` (cùng loại y lệnh). Giữ nguyên `PATIENT_TYPE_ID`, `AMOUNT`, `USER_PRICE`, `HEIN_RATIO`, `PARENT_ID` của dịch vụ cũ |
| 6 | Nhiều dịch vụ cùng chuyển sang 1 phòng trong cùng hồ sơ → gom chung 1 phiếu mới (theo `HisServiceReqRoomAssign`) |
| 7 | Phiếu xét nghiệm mới → sinh barcode như Import (`GenerateBarcodeTest`) |
| 8 | Ghi `EventLog HisServiceReq_DoiPhongThucHien` (phòng cũ → phòng mới, mã hồ sơ, mã y lệnh) |

### 4.6. Quy tắc Thêm dịch vụ

| Bước | Xử lý |
|---|---|
| 1 | Hồ sơ đã có dịch vụ trùng (Q6) → Bỏ qua — "Đã có dịch vụ" |
| 2 | Hồ sơ đã kết thúc / khóa viện phí → Lỗi |
| 3 | Hợp đồng ngoài hiệu lực tại `IntructionTime` → Lỗi (`IsValidContractTime`) |
| 4 | Lấy `HIS_KSK_SERVICE(KskId, ServiceId)`; phòng = `RoomId` ?? `KSK_SERVICE.ROOM_ID` |
| 5 | Kiểm tra phòng thực hiện được dịch vụ (`ValidateRoom`) và chính sách giá đối tượng KSK (`ValidateServicePaty`) |
| 6 | Tạo `ServiceReqDetailSDO`: `PatientTypeId` = đối tượng hiện hành, `Amount` = `KSK_SERVICE.AMOUNT`, `UserPrice` = `PRICE × (1 + VAT_RATIO)` |
| 7 | Phân phòng + tạo phiếu/`SERE_SERV` bằng đúng code Import (`HisServiceReqRoomAssign`, `MakeServiceReq`, `HisSereServSetPrice`); `HEIN_RATIO` = `PAYMENT_RATIO`; `REQUEST_LOGINNAME` = `Loginname` của SDO |

---

## 5. Thiết kế Frontend — module `HIS.Desktop.Plugins.KskServiceEditList`

### 5.1. Cấu trúc

```
HIS.Desktop.Plugins.KskServiceEditList/
├── KskServiceEditListProcessor.cs      ← MEF, MODULE_TYPE_ID__FORM
├── KskServiceEditList/
│   ├── IKskServiceEditList.cs
│   ├── KskServiceEditListFactory.cs
│   ├── KskServiceEditListBehavior.cs   ← nhận List<V_HIS_TREATMENT_4>, V_HIS_KSK_CONTRACT, RefeshReference
│   ├── frmKskServiceEditList.cs (+ Designer)
│   ├── frmKskServiceEditList__Load.cs  ← dựng dòng dịch vụ, combo
│   ├── frmKskServiceEditList__Grid.cs  ← lọc phòng, công tắc, ô chọn, cột phòng
│   └── frmKskServiceEditList__Save.cs  ← chuyển trạng thái lưới thành SDO, gọi Worker
├── ADO/
│   ├── ServiceRowADO.cs                ← 1 dòng lưới dịch vụ (gộp trên các hồ sơ)
│   ├── RoomADO.cs, KskServiceEditSDO.cs (bản sao SDO backend)
│   └── KskServiceEditBatchResultADO.cs, KskServiceEditResultRowADO.cs
├── Worker/KskServiceEditWorker.cs      ← lấy sere_serv, gọi API theo lô 200 (không để trong Form)
├── Result/frmKskServiceEditResult.cs   ← kết quả + xuất Excel
├── Resources/ Lang(.vi/.en), Message.Lang(.vi/.en), ResourceLanguageManager, ResourceMessage
└── Properties/AssemblyInfo.cs          ← [assembly: Plugin]
```

Không dùng ADO chung trong `HIS.Desktop.ADO` (tránh phải build/deploy lại DLL đó): TreatmentList truyền thẳng `List<V_HIS_TREATMENT_4>` + `V_HIS_KSK_CONTRACT`.

### 5.2. Bố cục màn hình (luồng giống "Sửa chỉ định dịch vụ")

Màn hình giữ đúng cách dùng của **Sửa chỉ định dịch vụ (AssignServiceEdit)**: một lưới dịch vụ có ô chọn,
dịch vụ đang có được tick sẵn; tick / bỏ tick / sửa phòng rồi Lưu. Khác biệt duy nhất là dữ liệu gộp trên
nhiều hồ sơ và Lưu gọi API mới `api/HisKskContract/ServiceEdit`.

```
┌──────────────────────────────────────────────────────────────────────────┐
│ Hợp đồng: HD001 - Cty ABC                     Số bệnh nhân: 350 bệnh nhân │
│ Thời gian chỉ định:[dt]   Phòng thực hiện:[cboRoom]   Người chỉ định:[cbo] │
├──────────────────────────────────────────────────────────────────────────┤
│ ☐ │Mã DV│Tên DV   │Phòng thực hiện ▼│Số BN có│Đã thực hiện│SL│Đơn giá│Loại│
│ ☑ │XN01 │CTM      │P.Xét nghiệm     │350/350 │0           │1 │ ...   │XN  │
│ ▣ │SA01 │SA bụng  │P.Siêu âm        │120/350 │0           │1 │ ...   │CĐHA│
│ ☐ │DT01 │Điện tim │P.TDCN           │0/350   │0           │1 │ ...   │TDCN│
├──────────────────────────────────────────────────────────────────────────┤
│ (Tất cả dịch vụ | Dịch vụ đã chọn)                         [Lưu (Ctrl S)] │
└──────────────────────────────────────────────────────────────────────────┘
```

| Thành phần | Quy tắc |
|---|---|
| Nguồn dòng | Dịch vụ các hồ sơ đang có (`api/HisSereServ/Get` theo `TREATMENT_IDs`, lô 100) ∪ dịch vụ trong nhóm dịch vụ KSK của hợp đồng (`HIS_KSK.KSK_CONTRACT_ID`) và nhóm dùng chung. Dịch vụ đã chọn xếp đầu |
| Ô chọn | Tất cả BN có → ☑; một phần BN có → ▣ (lưng chừng); chưa có → ☐ |
| Bỏ tick dịch vụ đang có | **Xóa** khỏi mọi hồ sơ đã chọn |
| Tick dịch vụ chưa có / tick hẳn dịch vụ lưng chừng | **Thêm** (BN đã có được bỏ qua); phải thuộc nhóm DV KSK, nếu không → cảnh báo |
| Sửa cột "Phòng thực hiện" | Dịch vụ tất cả BN đang có → **đổi phòng**; dịch vụ thêm → phòng thực hiện khi thêm (trống → phòng theo nhóm KSK). Chỉ sửa khi dòng đang tick |
| "Phòng thực hiện" (trên) | Như AssignServiceEdit: lọc lưới theo dịch vụ phòng đó thực hiện được (dòng đang có / đang tick luôn hiện); là phòng mặc định khi tick thêm |
| Công tắc | "Tất cả dịch vụ" / "Dịch vụ đã chọn" (chỉ dòng ☑, ▣) |
| Tìm kiếm | Hàng lọc tự động theo mã, tên, loại dịch vụ |
| Màu dòng | Đỏ = sẽ xóa, xanh = sẽ thêm, cam (cột phòng) = đổi phòng |
| Lưu | Xác nhận "Áp dụng thay đổi cho N bệnh nhân?" → **chia lô 200 hồ sơ/lần gọi API** (mục 5.4) → màn hình kết quả → làm mới Hồ sơ điều trị |

### 5.3. Màn hình kết quả

- Tổng hợp: số hồ sơ thành công / lỗi; số DV đã thêm, bỏ qua do trùng, đã xóa, không xóa được, đã đổi phòng, không đổi được.
- Grid: Mã điều trị, Họ tên, Thao tác, Dịch vụ, Kết quả, Lý do. Lọc theo Kết quả.
- Nút "Xuất Excel" danh sách không thành công (tương tự xuất lỗi của HisImportKsk).

### 5.4. Chia lô khi lưu (đã chốt)

| Quy tắc | Chi tiết |
|---|---|
| Kích thước lô | **200 hồ sơ/lần gọi API** — khai báo hằng số `BATCH_SIZE = 200` trong `Base/Constant.cs`, không hardcode trong code xử lý |
| Nội dung mỗi lô | Cùng `AddServices`, `DeleteServiceIds`, `ChangeRooms`, `Loginname`, `IntructionTime`; chỉ khác `TreatmentIds` |
| Thứ tự | Gọi tuần tự từng lô (không song song) để tránh tranh chấp bộ đếm phòng, số thứ tự y lệnh |
| Tiến độ | `WaitingManager` hiển thị "Đang xử lý lô i/n (x/N bệnh nhân)" |
| Lô lỗi toàn bộ | API trả null / exception / mất kết nối → đánh dấu toàn bộ hồ sơ trong lô là lỗi ("Lỗi kết nối, chưa xử lý"), tiếp tục lô sau |
| Gộp kết quả | Cộng dồn `Results` và các chỉ số `Summary` của mọi lô → hiển thị 1 lần trên `frmKskServiceEditResult` |
| Token | Sau mỗi lô gọi `SessionManager.ProcessTokenLost(param)`; mất token → dừng các lô còn lại, báo số hồ sơ chưa xử lý |
| Log | `LogSystem.Debug` input/output từng lô (số hồ sơ, không log toàn bộ danh sách); `LogAction` 1 bản ghi sau khi hoàn tất |

```csharp
// Run/KskServiceEditProcessor.cs
internal KskServiceEditResultADO Run(
    HisKskServiceEditSDO baseSdo, List<long> treatmentIds)
{
    KskServiceEditResultADO total = new KskServiceEditResultADO();
    try
    {
        int size = Base.Constant.BATCH_SIZE;
        int batchCount = (int)Math.Ceiling((double)treatmentIds.Count / size);
        for (int i = 0; i < batchCount; i++)
        {
            List<long> batchIds = treatmentIds.Skip(i * size).Take(size).ToList();
            HisKskServiceEditSDO sdo = CloneWithTreatmentIds(baseSdo, batchIds);

            CommonParam param = new CommonParam();
            var rs = new BackendAdapter(param).Post<HisKskServiceEditSDO>(
                RequestUriStore.HIS_KSK_CONTRACT__SERVICE_EDIT,
                ApiConsumers.MosConsumer, sdo, param);

            if (param.HasException && IsTokenLost(param))
            {
                SessionManager.ProcessTokenLost(param);
                total.MarkNotProcessed(treatmentIds.Skip(i * size).ToList());
                break;
            }

            if (rs != null) total.Merge(rs);
            else total.MarkBatchError(batchIds, param);
        }
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
    return total;
}
```

### 5.5. Grid, ControlState, đa ngôn ngữ

- Grid tổng hợp nhiều bảng → không bắt buộc 4 cột audit.
- Không có checkbox cần nhớ trạng thái → không cần ControlState.
- Toàn bộ caption/message trong `Lang.*.resx`, `Message.Lang.*.resx`.

---

## 6. Rủi ro còn lại

| Mức | Vấn đề | Hướng xử lý |
|---|---|---|
| HIGH | Bỏ kiểm tra "phiếu do người khác tạo" | Chỉ trong API mới; bắt buộc quyền API + control code; EventLog ghi tài khoản thực hiện |
| MEDIUM | Tách phiếu làm thay đổi mã y lệnh / barcode XN đã in | Kết quả hiển thị mã y lệnh mới; khuyến nghị in lại phiếu chỉ định |
| LOW | Phòng mới hết giờ làm việc / quá tải | Không kiểm tra (giống Import) |

---

## 7. Triển khai (23/09/2026)

| Phần | Vị trí | Ghi chú |
|---|---|---|
| Nút "Sửa dịch vụ" | `HIS.Desktop.Plugins.TreatmentList/UCTreatmentList___KskServiceEdit.cs` | `appliedKskContractId` gán từ filter trong `FillDataToGridTreatment`; reset trong `SetDefaultControl` |
| Module mới | `HIS.Desktop.Plugins.KskServiceEditList` | Args `List<V_HIS_TREATMENT_4>` (không cần gọi lại API lấy hồ sơ), `V_HIS_KSK_CONTRACT`, `RefeshReference`; SDO khai báo bản sao trong plugin |
| API | MOS `api/HisKskContract/ServiceEdit` | `MOS.MANAGER/HisKskContract/ServiceEdit/*`, `MOS.SDO/HisKskServiceEditSDO.cs`; 8 message mới `HisKskContract_*` (MOS.LibraryMessage), 2 EventLog mới `HisKskContract_SuaDichVu_*` |
| Tài liệu module | `docs/HIS.Desktop.Plugins.KskServiceEditList.md` | 9 mục |

**Việc cần làm khi triển khai:**
1. Deploy backend MOS (MOS.API, MOS.MANAGER, MOS.SDO, MOS.LibraryMessage, MOS.LibraryEventLog).
2. Khai báo module `HIS.Desktop.Plugins.KskServiceEditList` trong ACS (ACS_MODULE) và quyền API `api/HisKskContract/ServiceEdit`.
3. (Tùy chọn) Khai báo ACS_CONTROL `HIS000060` để phân quyền nút.

---

## 8. Changelog

| Ngày | Nội dung |
|---|---|
| 2026-09-23 | Khởi tạo tài liệu kỹ thuật; bổ sung quy tắc chỉ hiển thị nút "Sửa dịch vụ" khi grid đang lọc theo hợp đồng KSK |
| 2026-09-23 | Chốt Q1: đổi phòng theo từng dịch vụ (tách phiếu). Chốt Q2–Q8 theo HisImportKsk; bổ sung thiết kế API `HisKskContract/ServiceEdit`, quy tắc xóa / đổi phòng / thêm, thiết kế module `KskServiceEditList` |
| 2026-09-23 | Chốt: xóa hết dịch vụ của y lệnh → báo lỗi, không xóa; frontend chia lô 200 hồ sơ/lần gọi API (mục 5.4) |
| 2026-09-23 | Triển khai code: TreatmentList, module KskServiceEditList, API MOS HisKskContract/ServiceEdit (mục 7) |
| 2026-09-24 | Màn hình Sửa dịch vụ đổi sang luồng hiển thị/xử lý giống "Sửa chỉ định dịch vụ" (mục 5.2); API giữ nguyên |
