# Nhập Quỹ Tiền Mặt (Cash Collect) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.CashCollect |
| Loại | UC (mở dạng tab) |
| Mục đích | Thu ngân gom các giao dịch tiền mặt (thanh toán, tạm ứng, hoàn ứng) chưa nộp thành một **phiếu nộp quỹ** (`HIS_CASHOUT`); xem, sửa ngày nộp, thêm/bớt giao dịch hoặc xóa phiếu đã nộp. |
| UC dùng riêng | `HIS.UC.CashCollect` (lưới giao dịch có cột tích; chỉ plugin này dùng) |
| Trạng thái | Bảo trì — việc 55505 (06/10/2026): lọc Phòng thu ngân, sửa ngày nhập quỹ, tích ALL theo điều kiện lọc |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Lọc giao dịch: Mã giao dịch, Từ khóa, Trạng thái (Tất cả / Đã nộp / Chưa nộp — mặc định Chưa nộp), **Phòng thu ngân** (chọn nhiều), Sổ thu chi (chọn nhiều), Giao dịch viên, Từ số – Đến số, Từ ngày – Đến ngày → **Tìm kiếm (Ctrl F)**.
2. Tích từng giao dịch, hoặc tích **ô trên tiêu đề cột tích** để chọn **mọi giao dịch chưa nộp thỏa bộ lọc (tất cả các trang)**. Danh sách chọn giữ nguyên khi chuyển trang.
3. Lưới bên phải = các giao dịch sẽ nộp; **Tổng số tiền** tính lại mỗi lần chọn/bỏ.
4. Chọn **Ngày nộp** (ngày + giờ, mặc định thời điểm hiện tại) → **Nộp mới (Ctrl N)** → `api/HisCashout/Create`.
5. Sửa phiếu: chọn phiếu ở lưới nộp quỹ → lưới phải nạp giao dịch của phiếu, ô Ngày nộp nạp `CASHOUT_TIME` → đổi ngày, tích thêm giao dịch hoặc bấm X để bỏ giao dịch → **Sửa (Ctrl S)** → `api/HisCashout/Update`. Bỏ hết giao dịch rồi Sửa = xóa phiếu.
6. **Làm lại (Ctrl C)**: xóa bộ lọc (kể cả Phòng thu ngân), xóa danh sách chọn, về chế độ nộp mới.

### Danh sách chọn (việc 55505)
- Plugin giữ giao dịch đã chọn theo ID (`dicSelectedTransaction`), độc lập với trang đang xem; trang nào tải lên cũng đánh dấu tích theo danh sách này.
- Chuyển trang và Tích ALL dùng **bộ lọc của lần Tìm kiếm gần nhất** (`currentTransactionFilter`), không dùng giá trị đang gõ dở trên ô lọc.
- Danh sách chọn bị xóa khi: Tìm kiếm lại, Làm lại, Nộp mới / Sửa thành công, chọn phiếu khác ở lưới nộp quỹ.
- Ô tích tiêu đề tự bỏ tích khi người dùng bỏ 1 dòng (lưới trái) hoặc xóa 1 dòng mới chọn (lưới phải).

### Điều kiện nghiệp vụ
- Giao dịch đã thuộc một phiếu nộp quỹ thì không tích được; Tích ALL chỉ lấy giao dịch chưa nộp (`HAS_CASHOUT = false`).
- Tổng tiền tính theo đúng công thức Backend kiểm tra (`CalcCashoutAmount`): Thanh toán + Tạm ứng − Hoàn ứng − Miễn giảm − Kết chuyển − Quỹ thanh toán. Lệch công thức thì BE trả "Dữ liệu đầu vào không hợp lệ".
- BE chặn giao dịch đã nộp ở phiếu khác ("Các giao dịch sau đã nộp quỹ").
- Lọc Phòng thu ngân cần Backend có `HisTransactionViewFilter.CASHIER_ROOM_IDs` (MOS.Filter 01/10/2026, MOS.MANAGER 02/10/2026). BE cũ bỏ qua điều kiện này; Tích ALL vẫn lọc lại ở máy trạm theo phòng đã chọn.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TRANSACTION | View | Giao dịch (lưới trái, lưới phải) |
| HIS_CASHOUT | Table | Phiếu nộp quỹ: `CASHOUT_TIME` (thời gian nộp người dùng chọn), `AMOUNT`, `LOGINNAME` |
| V_HIS_CASHIER_ROOM | View | Danh mục Phòng thu ngân (chỉ phòng còn hoạt động, cache `BackendDataWorker`) |
| HIS_ACCOUNT_BOOK | Table | Sổ thu chi |
| ACS_USER | Table | Giao dịch viên |

### Quan hệ chính
- `HIS_TRANSACTION.CASHOUT_ID` → `HIS_CASHOUT.ID` (n-1); NULL = chưa nộp quỹ.
- `HIS_TRANSACTION.CASHIER_ROOM_ID` → phòng thu ngân lập giao dịch. Phiếu nộp quỹ không có cột phòng.

## 4. UI Layout

### Sơ đồ giao diện
```
+----------------------------------------------------------------------------------------------+
| [Mã GD][Từ khóa][Trạng thái][Phòng thu ngân ▼][Sổ thu chi ▼] |                                |
| [Giao dịch viên][Từ số][Đến số][Từ ngày][Đến ngày]           | [Tìm kiếm][Tổng số tiền][Người nộp][Ngày nộp dd/MM/yyyy HH:mm] |
+--------------------------------------------------------------+-------------------------------+
| Lưới giao dịch (UC HIS.UC.CashCollect)                       | Lưới phiếu nộp quỹ            |
| [☑ tiêu đề = Tích ALL] STT | Mã GD | Số tiền | ...           | X | Tên đăng nhập | Thời gian nộp quỹ | Số tiền |
|                                                              +-------------------------------+
|                                                              | Lưới giao dịch sẽ nộp (X = bỏ) |
+--------------------------------------------------------------+-------------------------------+
| Phân trang                                                   | [Sửa (Ctrl S)][Nộp mới (Ctrl N)][Làm lại (Ctrl C)] |
+----------------------------------------------------------------------------------------------+
```

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.CashCollect | panelControl1 | Lưới giao dịch có cột tích; `CashCollectInitADO.CheckAll_Click` bật ô tích ALL trên tiêu đề cột `check`, `UCCashCollectProcessor.SetCheckAll` đặt trạng thái ô đó |
| Inventec.UC.Paging | ucPaging1 | Phân trang lưới giao dịch |
| GridCheckMarksSelection1 | cboCashierRoom, cboBookCollection | Combo chọn nhiều |

### Partial class
| File | Nội dung |
|------|----------|
| `UCCashCollect.cs` | Khởi tạo, bộ lọc (`BuildTransactionFilter`), lưới, Nộp mới / Sửa / Làm lại |
| `UCCashCollect___CashierRoom.cs` | Combo Phòng thu ngân |
| `UCCashCollect___Selection.cs` | Danh sách chọn, Tích ALL, lưới phải, `CalcCashoutAmount` |

## 5. API Endpoints

| Action | URI | Consumer | Filter / Dữ liệu |
|--------|-----|----------|------------------|
| Giao dịch (phân trang) | `api/HisTransaction/GetView` | MosConsumer | `HisTransactionViewFilter` (+ `CASHIER_ROOM_IDs`) |
| Tích ALL (không phân trang) | `api/HisTransaction/GetView` | MosConsumer | bộ lọc lần Tìm kiếm gần nhất + `HAS_CASHOUT = false` |
| Giao dịch của phiếu | `api/HisTransaction/GetView` | MosConsumer | `CASHOUT_ID` |
| Phiếu nộp quỹ | `api/HisCashout/Get` | MosConsumer | `HisCashoutFilter` (lọc người đăng nhập ở máy trạm) |
| Nộp mới | `api/HisCashout/Create` | MosConsumer | `HisCashoutSDO { CashoutTime, Amount, TransactionIds }` |
| Sửa | `api/HisCashout/Update` | MosConsumer | `HisCashoutSDO { Id, CashoutTime, Amount, TransactionIds }` |
| Xóa phiếu | `api/HisCashout/Delete` | MosConsumer | ID phiếu |
| Sổ thu chi | `api/HisAccountBook/Get` | MosConsumer | `HisAccountBookFilter` |

## 6. Dependencies

### Library Plugins
Không dùng.

### Inter-Plugin
Không mở plugin khác.

## 7. Print

Không có chức năng in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 06/10/2026 | dangth2 (Claude) | Việc 55505 (NTP): thêm lọc **Phòng thu ngân** (chọn nhiều, `CASHIER_ROOM_IDs`); **Ngày nộp** chọn ngày + giờ, lưu đúng giá trị chọn (bỏ ép 23:59:59), lưới "Thời gian nộp quỹ" hiển thị `CASHOUT_TIME` (trước hiển thị nhầm `CREATE_TIME`); **Tích ALL theo điều kiện lọc** (ô tích trên tiêu đề cột, chọn mọi trang); danh sách chọn giữ qua các trang; một hàm tính tổng tiền theo công thức BE. Sửa kèm luồng Sửa: không còn gửi giao dịch của phiếu sửa trước, không lặp dòng khi xóa rồi tích thêm, ngày nộp không phụ thuộc định dạng vùng của máy; Ctrl N không tạo phiếu khi đang sửa; Sửa xóa phiếu khi không còn giao dịch (thay cho điều kiện tổng tiền = 0). UC `HIS.UC.CashCollect`: `CheckAll_Click`, `SetCheckAll`; khôi phục 2 ảnh `Resources/cancel_16x16*.png` bị hỏng trong git (thiếu byte `0D` ở header PNG, làm build lỗi MSB3103) |

## 9. Test Cases

### Lọc
- [ ] Không chọn Phòng thu ngân → giao dịch mọi phòng
- [ ] Chọn 1 / nhiều phòng → chỉ giao dịch của các phòng đó
- [ ] Làm lại → bỏ chọn phòng

### Tích ALL
- [ ] Kết quả nhiều trang → tích ô tiêu đề → lưới phải đủ số dòng bằng tổng ở thanh phân trang, Tổng số tiền đúng; sang trang khác dòng vẫn tích
- [ ] Bỏ tích 1 dòng → ô tiêu đề bỏ tích, tổng giảm đúng
- [ ] Trạng thái "Đã nộp" → tích ô tiêu đề → thông báo không có giao dịch chưa nộp
- [ ] Bỏ tích ô tiêu đề → bỏ chọn hết

### Nộp mới / Sửa
- [ ] Nộp mới với giao dịch chọn ở nhiều trang → nộp đủ
- [ ] Ngày nộp 19/08/2026 10:30 → lưới nộp quỹ hiện đúng 19/08/2026 10:30
- [ ] Chọn phiếu → đổi ngày → Sửa → lưới hiện ngày mới; DB `CASHOUT_TIME` đúng giá trị chọn
- [ ] Sửa phiếu: tích thêm + bỏ giao dịch cũ → Sửa thành công, tổng tiền khớp
- [ ] Sửa phiếu A rồi phiếu B (chỉ đổi ngày) → không kéo giao dịch của A
- [ ] Bỏ hết giao dịch của phiếu → Sửa → phiếu bị xóa

### Tự động (harness, 06/10/2026)
- 72/72 đạt ở cả culture vi-VN và en-US: UC thật + BackendAdapter thật gọi máy chủ MOS giả lập có luật kiểm tổng tiền / giao dịch đã nộp như BE.
