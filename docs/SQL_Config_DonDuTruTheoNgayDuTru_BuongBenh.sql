-- =====================================================================
-- Cau hinh: Hien thi don thuoc du tru theo ngay du tru (man Buong benh)
-- Plugin  : HIS.Desktop.Plugins.BedRoomPartial
-- Tai lieu: A_NghiepVu_DonDuTruTheoNgayDuTru_BuongBenh.docx  (QT-01..QT-13)
--           B_KyThuat_DonDuTruTheoNgayDuTru_BuongBenh.docx
-- LUU Y   : CONFIG_CODE ben duoi la TAM - phai thay bang ma ke tiep
--           chua duoc su dung trong HIS_CONFIG truoc khi chay that.
-- =====================================================================

INSERT INTO HIS_CONFIG (KEY, DEFAULT_VALUE, DESCRIPTION, CONFIG_CODE, MODULE_LINKS)
SELECT 'HIS.Desktop.Plugins.BedRoomPartial.ShowAnticipatePresByUseDate', '',
'Hiển thị đơn thuốc dự trù theo ngày dự trù trên màn Buồng bệnh.' || CHR(13) || CHR(10) ||
'Mặc định TẮT để không ảnh hưởng các đơn vị đang sử dụng.' || CHR(13) || CHR(10) ||
'1: BẬT. Đơn thuốc dự trù hiển thị ở đúng ngày được dự trù.' || CHR(13) || CHR(10) ||
'Rỗng hoặc giá trị khác: TẮT. Giữ nguyên hành vi hiện tại - đơn dự trù hiển thị ở ngày kê đơn.' || CHR(13) || CHR(10) ||
'CHỈ áp dụng cho đơn thuốc.' || CHR(13) || CHR(10) ||
'KHÔNG áp dụng: đơn máu (SERVICE_REQ_TYPE_ID = 16), chỉ định dịch vụ (CLS, CĐHA, TDCN, PTTT),' || CHR(13) || CHR(10) ||
'y lệnh giường, y lệnh khám và các loại y lệnh khác.' || CHR(13) || CHR(10) ||
'Các y lệnh trên dù có nhập ô Dự trù vẫn nằm ở ngày kê và vẫn hiển thị chữ "Dự trù: dd/MM/yyyy"' || CHR(13) || CHR(10) ||
'ở cột Khoa yêu cầu như hiện tại.' || CHR(13) || CHR(10) ||
CHR(13) || CHR(10) ||
'--- VÍ DỤ ---' || CHR(13) || CHR(10) ||
'Ngày 06 kê đơn dự trù cho ngày 07.' || CHR(13) || CHR(10) ||
'   Giá trị 1  : chọn ngày 07 thấy đơn (cột Ngày kê = 06, Ngày dự trù = 07); chọn ngày 06 không thấy.' || CHR(13) || CHR(10) ||
'   Bỏ trống   : chọn ngày 06 thấy đơn; chọn ngày 07 không thấy.',
'01375',
'HIS.Desktop.Plugins.BedRoomPartial'
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM HIS_CONFIG WHERE KEY = 'HIS.Desktop.Plugins.BedRoomPartial.ShowAnticipatePresByUseDate');

COMMIT;


-- =====================================================================
-- BAT tinh nang cho benh vien (chay khi khach hang yeu cau)
-- =====================================================================
-- UPDATE HIS_CONFIG
--    SET VALUE = '1'
--  WHERE KEY = 'HIS.Desktop.Plugins.BedRoomPartial.ShowAnticipatePresByUseDate';
-- COMMIT;


-- =====================================================================
-- GO CAU HINH (rollback)
-- =====================================================================
-- DELETE FROM HIS_CONFIG
--  WHERE KEY = 'HIS.Desktop.Plugins.BedRoomPartial.ShowAnticipatePresByUseDate';
-- COMMIT;


-- =====================================================================
-- KIEM TRA SAU KHI CHAY
-- =====================================================================
-- SELECT KEY, VALUE, DEFAULT_VALUE, CONFIG_CODE, MODULE_LINKS
--   FROM HIS_CONFIG
--  WHERE KEY = 'HIS.Desktop.Plugins.BedRoomPartial.ShowAnticipatePresByUseDate';
