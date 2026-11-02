-- =====================================================================
-- Viec 61482 - TTMB - Chan khong cho sua thoi gian bat dau thuc hien CLS
-- Plugin  : HIS.Desktop.Plugins.ServiceExecute (Xu ly CLS)
--           HIS.Desktop.Plugins.SurgServiceReqExecute (Xu ly PTTT)
--           HIS.Desktop.Plugins.ServiceReqUpdateInstruction (Sua thong tin chung)
-- Key = 1 : ap dung. Bo trong / khac 1: khong ap dung (nhu cu). Mac dinh bo trong.
-- =====================================================================

INSERT INTO HIS_CONFIG (KEY, DEFAULT_VALUE, DESCRIPTION, CONFIG_CODE, MODULE_LINKS)
SELECT 'HIS.Desktop.Plugins.IsLockSubclinicalStartTime', NULL,
'Khóa thời gian bắt đầu thực hiện CLS, phẫu thuật thủ thuật' || CHR(13) || CHR(10) ||
'- 1: Thời gian bắt đầu chỉ lấy bằng nút "Lấy thời gian hiện tại", không cho sửa tay; đã lưu thì không cho sửa ở các màn Xử lý CLS, Xử lý PTTT, Sửa thông tin chung' || CHR(13) || CHR(10) ||
'- Khác 1: Không áp dụng', '01495',
'HIS.Desktop.Plugins.ServiceExecute,HIS.Desktop.Plugins.SurgServiceReqExecute,HIS.Desktop.Plugins.ServiceReqUpdateInstruction'
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM HIS_CONFIG WHERE KEY = 'HIS.Desktop.Plugins.IsLockSubclinicalStartTime');

-- Bat de test:
-- UPDATE HIS_CONFIG SET VALUE = '1' WHERE KEY = 'HIS.Desktop.Plugins.IsLockSubclinicalStartTime';
