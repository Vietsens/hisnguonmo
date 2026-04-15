---
description: Review UI theo ui_rules — DevExpress 15.2, layout, grid, validation, CRUD pattern, phím tắt
argument-hint: <file hoặc folder path>
---

# Review UI — DevExpress 15.2 Rules

Review: $ARGUMENTS

## 1. Base Class
- Form kế thừa FormBase? UC kế thừa UserControlBase?
- Constructor nhận Module parameter?
- Có SetIcon() trong constructor?

## 2. Layout
- LayoutControl có EnableIndentsWithoutBorders = True?
- LayoutControlItem TextAlignMode = CustomSize, TextSize hợp lý?
- Trường bắt buộc có màu Maroon?
- Vùng trống có EmptySpaceItem?
- Thiết kế cho 1366x768?

## 3. GridControl
- BeginUpdate/EndUpdate khi bind data?
- CustomUnboundColumnData cho STT, icon, datetime?
- RowCellStyle cho tô màu dòng?
- Column format số: #,##0.00?
- Caption tiếng Việt có dấu?
- Grid load 1 bảng/view có ĐỦ 4 cột audit cuối: Thời gian tạo, Người tạo, Thời gian sửa, Người sửa?
- 4 cột audit AllowEdit = false?
- CREATE_TIME_STR, MODIFY_TIME_STR là Unbound + TimeNumberToTimeString?

## 4. Validation
- DXValidationProvider cho required + maxlength?
- DXErrorProvider cho từng field?
- Clear error khi nhập mới?
- Validate TRƯỚC khi save?

## 5. CRUD Pattern
- Validate → WaitingManager.Show → BackendAdapter → Hide → MessageManager → SessionManager?
- Delete có confirm XtraMessageBox YesNo?
- ActionType dùng GlobalVariables.ActionAdd/Edit?

## 6. Phím Tắt
- Form: BarManager + AddBarManager()?
- UC: KeyboardWorker.cs với [KeyboardAction]?

## 7. ControlState (BẮT BUỘC cho checkbox/toggle nhớ trạng thái)
- Có 4 fields: controlStateWorker, currentControlStateRDO, isNotLoadWhileChangeControlStateInFirst, moduleLink?
- moduleLink = đúng Plugin ID (trùng [ExtensionOf])?
- InitControlState() trong Load, SAU SetDefaultValue?
- Flag bật TRUE đầu InitControlState, tắt FALSE cuối (và trong catch)?
- Mỗi checkbox cần nhớ có CheckedChanged handler?
- Handler check flag ĐẦU TIÊN: `if (isNotLoadWhileChangeControlStateInFirst) return;`?
- Gọi controlStateWorker.SetData() mỗi khi user thay đổi?
- KEY = control.Name — KHÔNG hardcode string?

## 8. Load Order
- Config → Combos → Language → Validation → TabIndex → Defaults → ControlState → Grid?

## 9. Tối Ưu Tốc Độ Load
- SuspendLayout/ResumeLayout khi thêm controls bằng code?
- Combo load từ BackendDataWorker (nhanh) trước, Grid API (chậm) sau?
- Tab pages có lazy-load (chỉ load khi click)?
- Data load 1 lần lưu field — KHÔNG load lại mỗi lần cần?

## 10. Tối Ưu Grid
- Có tính toán nặng trong CustomUnboundColumnData? → pre-compute vào ADO
- Có API call trong RowCellStyle? → cache trước
- Có tắt features thừa (ShowGroupPanel, ShowIndicator, AllowFindPanel)?
- Paging server-side qua ucPaging — KHÔNG load all?

## 11. Tối Ưu Layout
- Nested LayoutControl tối đa 2 cấp?
- EmptySpaceItem cho responsive — KHÔNG set size cố định?
- MinSize cho controls quan trọng?

## 12. UX
- WaitingManager cho mọi thao tác > 0.5s?
- Focus đúng control sau save/delete?
- Button disable khi đang xử lý (tránh double-click)?
- Validation error hiện tại control — KHÔNG popup MessageBox từng field?
- Responsive: test 1366x768, 1920x1080?

## 13. Đa Ngôn Ngữ (BẮT BUỘC)
- Có Resources/ folder với Lang.vi.resx + Lang.en.resx?
- Có Message.Lang.vi.resx + Message.Lang.en.resx?
- Có ResourceLanguageManager.cs + ResourceMessage.cs?
- TẤT CẢ LayoutControlItem.Text khai báo trong Lang.resx?
- TẤT CẢ Button.Text khai báo trong Lang.resx?
- TẤT CẢ TabPage.Text khai báo trong Lang.resx?
- Lang.en.resx có ĐẦY ĐỦ số entries bằng Lang.vi.resx?
- Message.Lang.en.resx có ĐẦY ĐỦ số entries bằng Message.Lang.vi.resx?
- SetCaptionByLanguageKey() gọi trong Load event?
- KHÔNG còn hardcode tiếng Việt trong code?
- Thông báo chung dùng MessageUtil.GetMessage(Message.Enum)?
- Thông báo riêng plugin dùng ResourceMessage?

## Output
[CRITICAL] Thiếu FormBase, thiếu BeginUpdate, logic trong Form — file:line — fix
[HIGH] Thiếu SetIcon, thiếu Language files, thiếu 4 cột audit — file:line — fix
[MEDIUM] Thiếu ControlState đầy đủ, thiếu EmptySpace — file:line — fix
[LOW] Caption chưa tiếng Việt, thiếu Maroon, Lang.en thiếu entries — file:line — fix
