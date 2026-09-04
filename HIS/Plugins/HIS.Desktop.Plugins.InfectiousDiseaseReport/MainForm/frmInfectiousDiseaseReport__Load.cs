/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Load: nạp cấu hình + danh mục ECDS + enum vào combo, rồi fill dữ liệu từ HIS.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.Config;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.Worker;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.MainForm
{
    public partial class frmInfectiousDiseaseReport
    {
        private void frmInfectiousDiseaseReport_Load(object sender, EventArgs e)
        {
            try
            {
                EcdsConfigCFG.LoadConfig();
                apiWorker = new EcdsApiWorker();
                catalogCache = new EcdsCatalogCache(apiWorker);
                mapper = new DiseaseCaseMapper(catalogCache);

                InitEnumCombos();
                InitCatalogCombos();
                FillDataFromHis();
                InitListPanelData();   // panel danh sách bên trái (nạp nền)
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Bind các combo enum theo giá trị chính thức ECDS (EnumEcds).</summary>
        private void InitEnumCombos()
        {
            try
            {
                BindEnumCombo(cboLoaiChanDoan, new List<KeyValueADO>
                {
                    new KeyValueADO((long)EcdsPhanLoaiChuanDoan.NghiNgo, "Nghi ngờ"),
                    new KeyValueADO((long)EcdsPhanLoaiChuanDoan.XacDinh, "Xác định"),
                });

                BindEnumCombo(cboTinhTrang, new List<KeyValueADO>
                {
                    new KeyValueADO((long)EcdsTinhTrangHienNay.NgoaiTru, "Ngoại trú"),
                    new KeyValueADO((long)EcdsTinhTrangHienNay.NoiTru, "Nội trú"),
                    new KeyValueADO((long)EcdsTinhTrangHienNay.RaVien, "Ra viện"),
                    new KeyValueADO((long)EcdsTinhTrangHienNay.TuVong, "Tử vong"),
                    new KeyValueADO((long)EcdsTinhTrangHienNay.ChuyenVien, "Chuyển viện"),
                    new KeyValueADO((long)EcdsTinhTrangHienNay.Khac, "Khác"),
                });

                BindEnumCombo(cboGioiTinh, new List<KeyValueADO>
                {
                    new KeyValueADO((long)EcdsGioiTinh.Nu, "Nữ"),
                    new KeyValueADO((long)EcdsGioiTinh.Nam, "Nam"),
                });

                BindEnumCombo(cboSuDungVacXin, new List<KeyValueADO>
                {
                    new KeyValueADO((long)EcdsSuDungVacXin.Co, "Có"),
                    new KeyValueADO((long)EcdsSuDungVacXin.Khong, "Không"),
                    new KeyValueADO((long)EcdsSuDungVacXin.KhongRo, "Không rõ"),
                });

                BindEnumCombo(cboLayMau, new List<KeyValueADO>
                {
                    new KeyValueADO((long)EcdsLayMauXetNghiem.Co, "Có"),
                    new KeyValueADO((long)EcdsLayMauXetNghiem.Khong, "Không"),
                });

                BindEnumCombo(cboLoaiXN, new List<KeyValueADO>
                {
                    new KeyValueADO((long)EcdsLoaiXetNghiem.TestNhanh, "Test nhanh"),
                    new KeyValueADO((long)EcdsLoaiXetNghiem.MacElisa, "Mac-ELISA"),
                    new KeyValueADO((long)EcdsLoaiXetNghiem.Pcr, "PCR"),
                    new KeyValueADO((long)EcdsLoaiXetNghiem.Khac, "Khác"),
                });

                BindEnumCombo(cboKetQuaXN, new List<KeyValueADO>
                {
                    new KeyValueADO((long)EcdsKetQuaXetNghiem.DuongTinh, "Dương tính"),
                    new KeyValueADO((long)EcdsKetQuaXetNghiem.AmTinh, "Âm tính"),
                    new KeyValueADO((long)EcdsKetQuaXetNghiem.ChuaCoKetQua, "Chưa có kết quả"),
                });

                BindEnumCombo(cboLoaiPhatHien, new List<KeyValueADO>
                {
                    new KeyValueADO((long)EcdsLoaiPhatHien.TramYTe, "Trạm y tế"),
                    new KeyValueADO((long)EcdsLoaiPhatHien.TaiNha, "Tại nhà"),
                    new KeyValueADO((long)EcdsLoaiPhatHien.YTeCoQuan, "Y tế cơ quan"),
                    new KeyValueADO((long)EcdsLoaiPhatHien.Khac, "Khác"),
                });

                // Hình thức điều trị (maHinhThucDieuTri) — cổng BẮT BUỘC, không có danh mục/enum công bố.
                // Dùng mã "1" = Nội trú, "2" = Ngoại trú (ví dụ cổng dùng "2"). Xác nhận thêm khi có tài liệu.
                BindEnumCombo(cboHinhThucDieuTri, new List<KeyValueADO>
                {
                    new KeyValueADO(1, "Nội trú"),
                    new KeyValueADO(2, "Ngoại trú"),
                });

                // Tình trạng ra viện (TINHTRANGRAVIEN): QĐ 4039 KHÔNG liệt kê enum -> tạm dùng
                // danh mục HIS_TREATMENT_END_TYPE (Khỏi/Đỡ/Không đổi/Nặng/Tử vong). Xác nhận mã cổng khi có tài liệu.
                InitDischargeStateCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Nạp danh mục ECDS vào combo (login trước; best-effort nếu chưa cấu hình).</summary>
        private void InitCatalogCombos()
        {
            try
            {
                // --- Danh mục HÀNH CHÍNH lấy từ SDA / HIS (local, luôn có — KHÔNG phụ thuộc cổng) ---
                InitSdaAdminCombos();

                // --- Danh mục nghiệp vụ ECDS (bệnh, cơ sở) lấy từ CỔNG (chỉ khi đã cấu hình) ---
                if (!EcdsConfigCFG.IsValid()) return;
                WaitingManager.Show();
                SetupLookup(cboBenh, catalogCache.GetStatic(EcdsCatalogCache.DM_BENH), "id", "ten");
                SetupLookup(cboDonViXN, catalogCache.GetStatic(EcdsCatalogCache.DM_COSO), "id", "ten");
                SetupLookup(cboBenhVienChuyenToi, catalogCache.GetStatic(EcdsCatalogCache.DM_COSO), "id", "ten");
                // Nghề nghiệp: bind THẲNG danh mục cổng (nghe-nghiep, mã "TT"/"CN"/"HSSV"...) — ValueMember = ma
                // -> chọn là ra đúng mã cổng, KHÔNG cần đối chiếu mã HIS (mã HIS khác hệ mã cổng).
                SetupLookup(cboNgheNghiep, catalogCache.GetStatic(EcdsCatalogCache.DM_NGHENGHIEP), "ma", "ten");
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Danh mục tab Hành chính lấy từ SDA/HIS (ValueMember = mã, khớp mã trong V_HIS_PATIENT):
        /// dân tộc = SDA_ETHNIC(ETHNIC_CODE), nghề = HIS_CAREER(CAREER_CODE),
        /// tỉnh = SDA_PROVINCE(PROVINCE_CODE), xã = SDA_COMMUNE(COMMUNE_CODE).
        /// </summary>
        private void InitSdaAdminCombos()
        {
            try
            {
                // Dân tộc = SDA_ETHNIC (khớp V_HIS_PATIENT.ETHNIC_CODE; V_SDA_NATIONAL là quốc tịch nên KHÔNG dùng).
                var ethnics = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.ETHNIC_NAME).ToList();
                SetupLookup(cboDanToc, ethnics, "ETHNIC_CODE", "ETHNIC_NAME");

                var careers = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.CAREER_NAME).ToList();
                SetupLookup(cboNgheNghiep, careers, "CAREER_CODE", "CAREER_NAME");

                var provinces = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.PROVINCE_NAME).ToList();
                SetupLookup(cboTinh, provinces, "PROVINCE_CODE", "PROVINCE_NAME");
                SetupLookup(cboTinhTru, provinces, "PROVINCE_CODE", "PROVINCE_NAME");

                var communes = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.COMMUNE_NAME).ToList();
                SetupLookup(cboXa, communes, "COMMUNE_CODE", "COMMUNE_NAME");
                SetupLookup(cboXaTru, communes, "COMMUNE_CODE", "COMMUNE_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bind combo "Tình trạng ra viện" (TINHTRANGRAVIEN) từ HIS_TREATMENT_END_TYPE.
        /// ValueMember = ID HIS (đẩy best-effort — QĐ 4039 chưa liệt kê enum cổng cho trường này).
        /// </summary>
        private void InitDischargeStateCombo()
        {
            try
            {
                var endTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.ID).ToList();
                SetupLookup(cboTinhTrangRaVien, endTypes, "ID", "TREATMENT_END_TYPE_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Xóa các control nhập khi tạo mới.</summary>
        private void ClearInputControls()
        {
            try
            {
                foreach (Control c in GetAllInputControls(this))
                {
                    if (c is BaseEdit && !(c is LabelControl))
                        ((BaseEdit)c).EditValue = null;
                }
                if (dxErr != null) dxErr.ClearErrors();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private IEnumerable<Control> GetAllInputControls(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                yield return c;
                foreach (Control child in GetAllInputControls(c))
                    yield return child;
            }
        }
    }
}
