-- =====================================================================
-- Viec 57853: TTMB - TK - Dinh kem anh/tep don thuoc o man xuat ban nha thuoc
--             va danh sach xuat
-- Plugin  : HIS.Desktop.Plugins.ExpMestSaleCreate    (nut "Dinh kem don")
--           HIS.Desktop.Plugins.HisExportMestMedicine (cot icon "Don dinh kem")
--           HIS.Desktop.Plugins.Library.ExpMestAttachFile (thu vien dung chung - MOI)
-- Tai lieu: docs/57853_PhanTich_DinhKemDonThuocXuatBan.md/.docx
-- LUU Y   : - Muc 1 chay tren schema EMR, muc 2-3 chay tren schema HIS.
--           - CONFIG_CODE ben duoi la TAM - thay bang ma ke tiep chua dung
--             trong HIS_CONFIG truoc khi chay that.
--           - Mac dinh TAT (IsEnable = 0) => giao dien xuat ban / danh sach
--             xuat giu nguyen nhu cu cho toi khi vien bat.
--           - Chuc nang chi hoat dong khi MOS.HAS_CONNECTION_EMR = 1.
-- =====================================================================

-- ---------------------------------------------------------------------
-- 1. [EMR] Loai van ban "Don thuoc dinh kem phieu xuat ban"
--    DOCUMENT_TYPE_CODE PHAI = 'EXPSA' (frontend loc/luu dung ma nay).
--    IS_ALLOW_DUPLICATE_HIS_CODE = 1: 1 phieu co nhieu lan dinh kem.
--    DBA dieu chinh ten sequence/cot audit theo chuan site.
-- ---------------------------------------------------------------------
INSERT INTO EMR_DOCUMENT_TYPE
    (ID, DOCUMENT_TYPE_CODE, DOCUMENT_TYPE_NAME, IS_ACTIVE, IS_DELETE,
     CREATE_TIME, CREATOR, IS_ALLOW_DUPLICATE_HIS_CODE)
SELECT SEQ_EMR_DOCUMENT_TYPE.NEXTVAL, 'EXPSA',
       'Đơn thuốc đính kèm phiếu xuất bán', 1, 0,
       TO_NUMBER(TO_CHAR(SYSDATE, 'YYYYMMDDHH24MISS')), 'admin', 1
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM EMR_DOCUMENT_TYPE WHERE DOCUMENT_TYPE_CODE = 'EXPSA');
COMMIT;

-- ---------------------------------------------------------------------
-- 2. [HIS] Bat/tat chuc nang dinh kem don thuoc tren phieu xuat ban
-- ---------------------------------------------------------------------
INSERT INTO HIS_CONFIG (KEY, DEFAULT_VALUE, DESCRIPTION, CONFIG_CODE, MODULE_LINKS)
SELECT 'HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.IsEnable', '0',
'Bật chức năng đính kèm đơn thuốc (chọn tệp hoặc chụp ảnh) trên phiếu xuất bán.' || CHR(13) || CHR(10) ||
'- 1: Bật. Màn Xuất bán có nút "Đính kèm đơn"; màn Danh sách xuất có cột icon đánh dấu phiếu bán đã có đơn đính kèm.' || CHR(13) || CHR(10) ||
'- 0 hoặc rỗng: Tắt (giữ nguyên giao diện như cũ).' || CHR(13) || CHR(10) ||
'Chỉ có hiệu lực khi MOS.HAS_CONNECTION_EMR = 1 (tệp lưu sang EMR, loại văn bản EXPSA).' || CHR(13) || CHR(10) ||
'Phiếu đã hoàn thành / đã thanh toán / đã xác nhận nợ: chỉ được xem và bổ sung đơn, không được xóa.',
'01001', 'HIS.Desktop.Plugins.ExpMestSaleCreate,HIS.Desktop.Plugins.HisExportMestMedicine'
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM HIS_CONFIG WHERE KEY = 'HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.IsEnable');

-- ---------------------------------------------------------------------
-- 3. [HIS] Dung luong toi da cua 1 tep dinh kem (MB)
-- ---------------------------------------------------------------------
INSERT INTO HIS_CONFIG (KEY, DEFAULT_VALUE, DESCRIPTION, CONFIG_CODE, MODULE_LINKS)
SELECT 'HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.MaxFileSizeMB', '5',
'Dung lượng tối đa (MB) của 1 tệp/ảnh đơn thuốc khi đính kèm vào phiếu xuất bán.' || CHR(13) || CHR(10) ||
'- Số dương, cho phép số thập phân (VD: 2.5). Rỗng hoặc <= 0: mặc định 5 MB.' || CHR(13) || CHR(10) ||
'- Tệp vượt dung lượng hoặc sai định dạng (chỉ nhận jpg, jpeg, png, bmp, gif, pdf) bị cảnh báo và không nhận.',
'01002', 'HIS.Desktop.Plugins.ExpMestSaleCreate,HIS.Desktop.Plugins.HisExportMestMedicine'
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM HIS_CONFIG WHERE KEY = 'HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.MaxFileSizeMB');

COMMIT;
