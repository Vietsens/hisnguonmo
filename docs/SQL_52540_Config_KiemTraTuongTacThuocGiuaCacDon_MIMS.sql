-- =====================================================================
-- Viec 52540: Kiem tra tuong tac giua cac thuoc trong cac don khac nhau
--             cua ho so bang MIMS
-- Plugin  : HIS.Desktop.Plugins.AssignPrescriptionPK
--           HIS.Desktop.Plugins.AssignPrescriptionCLS
--           HIS.Desktop.Plugins.AssignPrescriptionKidney
--           HIS.Desktop.Plugins.AssignPrescriptionYHCT
-- Tai lieu: 52540_A_NghiepVu_KiemTraTuongTacThuocGiuaCacDon_MIMS.docx (QT-01..QT-23)
--           52540_B_KyThuat_KiemTraTuongTacThuocGiuaCacDon_MIMS.docx
-- LUU Y   : CONFIG_CODE ben duoi la TAM - phai thay bang ma ke tiep
--           chua duoc su dung trong HIS_CONFIG truoc khi chay that.
--           Ca 3 config mac dinh de TRONG => he thong giu nguyen hanh vi
--           hien tai (chi kiem tra tuong tac trong pham vi don dang ke).
-- =====================================================================

-- ---------------------------------------------------------------------
-- 1. Pham vi kiem tra tuong tac thuoc bang MIMS
-- ---------------------------------------------------------------------
INSERT INTO HIS_CONFIG (KEY, DEFAULT_VALUE, DESCRIPTION, CONFIG_CODE, MODULE_LINKS)
SELECT 'HIS.Desktop.Mims.InteractionScopeOption', '',
'Phạm vi kiểm tra tương tác thuốc bằng MIMS khi lưu đơn thuốc.' || CHR(13) || CHR(10) ||
'Chỉ có hiệu lực khi HIS.Desktop.Plugins.AssignPrescription.ConnectDrugInterventionInfo = 2 (dùng MIMS).' || CHR(13) || CHR(10) ||
'- Rỗng hoặc 1: Chỉ kiểm tra các thuốc trong đơn đang kê (giữ nguyên hành vi hiện tại).' || CHR(13) || CHR(10) ||
'- 2: Kiểm tra thêm các thuốc còn hiệu lực của các đơn KHÁC trong CÙNG hồ sơ điều trị.' || CHR(13) || CHR(10) ||
'- 3: Kiểm tra thêm các thuốc còn hiệu lực của mọi đơn thuộc cùng bệnh nhân (kể cả các lần điều trị trước).' || CHR(13) || CHR(10) ||
CHR(13) || CHR(10) ||
'Thuốc được coi là "còn hiệu lực" khi Ngày dùng đến >= thời điểm chỉ định của đơn đang kê.' || CHR(13) || CHR(10) ||
'Thuốc không có Ngày dùng đến thì lấy theo cấu hình HIS.Desktop.Mims.PreviousPrescriptionDayRange.' || CHR(13) || CHR(10) ||
'Không lấy: thuốc của đơn đã hủy, thuốc của chính đơn đang sửa, thuốc trùng đúng thuốc đang kê, vật tư.' || CHR(13) || CHR(10) ||
'Chỉ hiển thị cảnh báo có liên quan tới ít nhất một thuốc bác sĩ ĐANG kê.' || CHR(13) || CHR(10) ||
'Tối đa 30 thuốc từ các đơn khác được đưa vào một lần kiểm tra (ưu tiên thuốc còn dùng lâu nhất).' || CHR(13) || CHR(10) ||
CHR(13) || CHR(10) ||
'--- KHUYẾN NGHỊ ---' || CHR(13) || CHR(10) ||
'   Bệnh viện có nội trú          : dùng 2.' || CHR(13) || CHR(10) ||
'   Phòng khám / chủ yếu ngoại trú: dùng 3 (mỗi lần khám là một hồ sơ riêng nên giá trị 2 sẽ' || CHR(13) || CHR(10) ||
'                                   không bắt được đơn của lần khám trước).' || CHR(13) || CHR(10) ||
'   Giai đoạn thử nghiệm          : bật 2 cho một khoa nội trú, theo dõi số lượng cảnh báo trước khi mở rộng.',
'01000', 'HIS.Desktop.Plugins.AssignPrescriptionPK,HIS.Desktop.Plugins.AssignPrescriptionCLS,HIS.Desktop.Plugins.AssignPrescriptionKidney,HIS.Desktop.Plugins.AssignPrescriptionYHCT'
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM HIS_CONFIG WHERE KEY = 'HIS.Desktop.Mims.InteractionScopeOption');

-- ---------------------------------------------------------------------
-- 2. So ngay lui khi thuoc don khac khong co Ngay dung den
-- ---------------------------------------------------------------------
INSERT INTO HIS_CONFIG (KEY, DEFAULT_VALUE, DESCRIPTION, CONFIG_CODE, MODULE_LINKS)
SELECT 'HIS.Desktop.Mims.PreviousPrescriptionDayRange', '30',
'Số ngày lùi để lấy thuốc của các đơn khác khi kiểm tra tương tác bằng MIMS.' || CHR(13) || CHR(10) ||
'Chỉ có hiệu lực khi HIS.Desktop.Mims.InteractionScopeOption = 2 hoặc 3.' || CHR(13) || CHR(10) ||
'- Là số nguyên dương, tính lùi từ thời điểm chỉ định của đơn đang kê.' || CHR(13) || CHR(10) ||
'- Rỗng hoặc <= 0: hệ thống dùng mặc định 30 ngày.' || CHR(13) || CHR(10) ||
CHR(13) || CHR(10) ||
'Dùng cho 2 việc:' || CHR(13) || CHR(10) ||
'   1. Giới hạn khoảng ngày khi lấy dữ liệu đơn cũ (giảm tải máy chủ).' || CHR(13) || CHR(10) ||
'   2. Xác định thuốc còn dùng với các đơn KHÔNG nhập số ngày dùng (không có Ngày dùng đến):' || CHR(13) || CHR(10) ||
'      thuốc được coi là còn dùng nếu ngày kê đơn nằm trong khoảng số ngày này.' || CHR(13) || CHR(10) ||
CHR(13) || CHR(10) ||
'--- VÍ DỤ ---' || CHR(13) || CHR(10) ||
'Giá trị 30, đơn đang kê ngày 09/09/2026:' || CHR(13) || CHR(10) ||
'   Đơn cũ ngày 20/08/2026 không nhập số ngày dùng  -> ĐƯỢC lấy.' || CHR(13) || CHR(10) ||
'   Đơn cũ ngày 01/07/2026 không nhập số ngày dùng  -> KHÔNG lấy.' || CHR(13) || CHR(10) ||
'   Đơn cũ ngày 01/07/2026 dùng đến 15/09/2026      -> ĐƯỢC lấy (còn Ngày dùng đến).',
'01001', 'HIS.Desktop.Plugins.AssignPrescriptionPK,HIS.Desktop.Plugins.AssignPrescriptionCLS,HIS.Desktop.Plugins.AssignPrescriptionKidney,HIS.Desktop.Plugins.AssignPrescriptionYHCT'
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM HIS_CONFIG WHERE KEY = 'HIS.Desktop.Mims.PreviousPrescriptionDayRange');

-- ---------------------------------------------------------------------
-- 3. Cach gui thuoc don khac toi MIMS (tham so ky thuat)
-- ---------------------------------------------------------------------
INSERT INTO HIS_CONFIG (KEY, DEFAULT_VALUE, DESCRIPTION, CONFIG_CODE, MODULE_LINKS)
SELECT 'HIS.Desktop.Mims.CrossPrescriptionRequestMode', '1',
'Cách gửi thuốc của các đơn khác tới MIMS. Tham số KỸ THUẬT, chỉ đổi khi MIMS yêu cầu.' || CHR(13) || CHR(10) ||
'Chỉ có hiệu lực khi HIS.Desktop.Mims.InteractionScopeOption = 2 hoặc 3.' || CHR(13) || CHR(10) ||
'- Rỗng hoặc 1: Gộp thuốc đơn khác vào khối Prescribing của request.' || CHR(13) || CHR(10) ||
'              Đúng theo MIMS API Guide: khối Prescribing là "Current and Past Medications".' || CHR(13) || CHR(10) ||
'- 2: Đưa thuốc đơn khác vào khối Prescribed riêng (hợp lệ theo Request55.xsd, dùng đối chứng).' || CHR(13) || CHR(10) ||
CHR(13) || CHR(10) ||
'Giữ giá trị 1 trừ khi kết quả thử nghiệm trên môi trường MIMS cho thấy cách 2 trả cảnh báo đúng hơn.',
'01002', 'HIS.Desktop.Plugins.AssignPrescriptionPK,HIS.Desktop.Plugins.AssignPrescriptionCLS,HIS.Desktop.Plugins.AssignPrescriptionKidney,HIS.Desktop.Plugins.AssignPrescriptionYHCT'
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM HIS_CONFIG WHERE KEY = 'HIS.Desktop.Mims.CrossPrescriptionRequestMode');

COMMIT;

-- =====================================================================
-- KIEM TRA SAU KHI CHAY
-- =====================================================================
-- SELECT KEY, VALUE, DEFAULT_VALUE, CONFIG_CODE, MODULE_LINKS
-- FROM HIS_CONFIG
-- WHERE KEY IN ('HIS.Desktop.Mims.InteractionScopeOption',
--               'HIS.Desktop.Mims.PreviousPrescriptionDayRange',
--               'HIS.Desktop.Mims.CrossPrescriptionRequestMode');

-- =====================================================================
-- BAT TINH NANG (chay rieng khi khach hang da chot pham vi)
-- =====================================================================
-- UPDATE HIS_CONFIG SET VALUE = '2'
--  WHERE KEY = 'HIS.Desktop.Mims.InteractionScopeOption';
-- COMMIT;

-- =====================================================================
-- ROLLBACK
-- =====================================================================
-- DELETE FROM HIS_CONFIG
--  WHERE KEY IN ('HIS.Desktop.Mims.InteractionScopeOption',
--                'HIS.Desktop.Mims.PreviousPrescriptionDayRange',
--                'HIS.Desktop.Mims.CrossPrescriptionRequestMode');
-- COMMIT;
