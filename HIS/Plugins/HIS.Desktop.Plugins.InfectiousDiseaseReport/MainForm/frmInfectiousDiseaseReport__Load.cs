/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Load: nạp cấu hình + danh mục ECDS + enum vào combo, rồi fill dữ liệu từ HIS.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.Config;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.Worker;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
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

                // TODO: enum nhóm sốt rét (cboPhuongPhapPhatHien, cboKetQuaSoiLam, cboKetQuaRdt,
                // cboXnG6pd, cboPhanLoaiG6pd, cboLoaiCoSoXN, cboLoaiSotRet, cboDaTungMac)
                // — bind khi có tài liệu enum ECDS.
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
                if (!EcdsConfigCFG.IsValid()) return;
                WaitingManager.Show();

                SetupLookup(cboBenh, catalogCache.GetStatic(EcdsCatalogCache.DM_BENH), "id", "ten");
                SetupLookup(cboDanToc, catalogCache.GetStatic(EcdsCatalogCache.DM_DANTOC), "id", "ten");
                SetupLookup(cboNgheNghiep, catalogCache.GetStatic(EcdsCatalogCache.DM_NGHENGHIEP), "id", "ten");
                SetupLookup(cboTinh, catalogCache.GetStatic(EcdsCatalogCache.DM_TINH), "id", "ten");
                SetupLookup(cboTinhTru, catalogCache.GetStatic(EcdsCatalogCache.DM_TINH), "id", "ten");
                SetupLookup(cboDonViXN, catalogCache.GetStatic(EcdsCatalogCache.DM_COSO), "id", "ten");
                SetupLookup(cboDonViXNSotRet, catalogCache.GetStatic(EcdsCatalogCache.DM_COSO), "id", "ten");
                SetupLookup(cboBenhVienChuyenToi, catalogCache.GetStatic(EcdsCatalogCache.DM_COSO), "id", "ten");

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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
