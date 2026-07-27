/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * FillDataFromHis: đổ dữ liệu điều trị/bệnh nhân từ HIS vào header + các tab.
 * Các trường hành chính chi tiết (dân tộc, nghề nghiệp, CCCD, địa chỉ mã GSO)
 * sẽ lấy từ V_HIS_PATIENT / view V_HIS_ECDS_DISEASE_CASE — bổ sung khi có API.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.Worker;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.MainForm
{
    public partial class frmInfectiousDiseaseReport
    {
        private void FillDataFromHis()
        {
            try
            {
                if (treatment == null) return;

                FillHeader();
                FillCaBenhTab();
                FillHanhChinhTab();
                FillNguoiBaoCaoTab();
                LoadExistingReconcile();   // đối soát: ca này đã đẩy chưa (§20 Get)
                UpdatePushStatusLabel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillHeader()
        {
            try
            {
                lblTreatmentCodeVal.Text = treatment.TREATMENT_CODE ?? "";
                lblPatientNameVal.Text = treatment.TDL_PATIENT_NAME ?? "";
                lblDobVal.Text = treatment.TDL_PATIENT_DOB > 0
                    ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_PATIENT_DOB)
                    : "";
                lblGenderVal.Text = treatment.TDL_PATIENT_GENDER_NAME ?? "";
                lblIcdVal.Text = (treatment.ICD_CODE ?? "") +
                    (string.IsNullOrEmpty(treatment.ICD_NAME) ? "" : " — " + treatment.ICD_NAME);

                var dept = BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == treatment.LAST_DEPARTMENT_ID);
                lblDepartmentVal.Text = dept != null ? dept.DEPARTMENT_NAME : "";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void FillCaBenhTab()
        {
            try
            {
                // Ánh xạ ICD điều trị -> ID bệnh ECDS (nếu danh mục đã nạp)
                if (cboBenh.Properties.DataSource != null && !string.IsNullOrEmpty(treatment.ICD_CODE))
                {
                    long? benhId = catalogCache.FindIdByMa(
                        catalogCache.GetStatic(Worker.EcdsCatalogCache.DM_BENH), treatment.ICD_CODE);
                    if (benhId.HasValue) cboBenh.EditValue = benhId.Value;
                }

                SetDateLong(dteNgayNhapVien, treatment.IN_TIME);
                SetDateLong(dteNgayRaVien, treatment.OUT_TIME);
                txtChanDoanRaVien.Text = treatment.ICD_NAME ?? "";

                // Mặc định phân loại chẩn đoán = Xác định
                cboLoaiChanDoan.EditValue = (long)EcdsPhanLoaiChuanDoan.XacDinh;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void FillHanhChinhTab()
        {
            try
            {
                txtHoTen.Text = treatment.TDL_PATIENT_NAME ?? "";
                SetDateLong(dteNgaySinh, treatment.TDL_PATIENT_DOB);

                // Tuổi
                if (treatment.TDL_PATIENT_DOB > 0)
                {
                    DateTime? dob = Inventec.Common.DateTime.Convert
                        .TimeNumberToSystemDateTime(treatment.TDL_PATIENT_DOB);
                    if (dob.HasValue)
                        spnTuoi.EditValue = (decimal)Math.Max(0, DateTime.Now.Year - dob.Value.Year);
                }

                // Giới tính: HIS_GENDER.ID -> EcdsGioiTinh
                bool isMale = treatment.TDL_PATIENT_GENDER_ID
                    == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE;
                cboGioiTinh.EditValue = isMale ? (long)EcdsGioiTinh.Nam : (long)EcdsGioiTinh.Nu;

                // Nạp V_HIS_PATIENT để điền CCCD/điện thoại/dân tộc/nghề nghiệp/địa chỉ (hiện nay + thường trú).
                FillPatientAdminFields();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Lấy thông tin hành chính bệnh nhân từ V_HIS_PATIENT (theo PATIENT_ID) và điền vào tab Hành chính.
        /// Text field điền luôn; combo danh mục (dân tộc, nghề, tỉnh/xã) chỉ map khi danh mục ECDS đã nạp.
        /// </summary>
        private void FillPatientAdminFields()
        {
            try
            {
                if (treatment == null || treatment.PATIENT_ID <= 0) return;

                CommonParam param = new CommonParam();
                var filter = new MOS.Filter.HisPatientViewFilter { ID = treatment.PATIENT_ID };
                var list = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(
                    HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                SessionManager.ProcessTokenLost(param);

                var patient = (list != null && list.Count > 0) ? list[0] : null;
                if (patient == null) return;

                // ---- Text field: điền không phụ thuộc cấu hình ECDS ----
                txtCccd.Text = !string.IsNullOrEmpty(patient.CCCD_NUMBER)
                    ? patient.CCCD_NUMBER : (patient.CMND_NUMBER ?? "");
                txtDienThoai.Text = patient.PHONE ?? "";
                txtNoiLamViec.Text = !string.IsNullOrEmpty(patient.WORK_PLACE_NAME)
                    ? patient.WORK_PLACE_NAME : (patient.WORK_PLACE ?? "");
                txtDiaChi.Text = !string.IsNullOrEmpty(patient.ADDRESS)
                    ? patient.ADDRESS : (patient.VIR_ADDRESS ?? "");
                txtDiaChiTru.Text = !string.IsNullOrEmpty(patient.HT_ADDRESS)
                    ? patient.HT_ADDRESS : (patient.VIR_HT_ADDRESS ?? "");

                // ---- Combo danh mục: chỉ map khi danh mục ECDS sẵn sàng (đối chiếu mã HIS -> ID ECDS) ----
                if (!Config.EcdsConfigCFG.IsValid() || catalogCache == null) return;

                SetLookupByHisCode(cboDanToc, EcdsCatalogCache.DM_DANTOC, patient.ETHNIC_CODE);
                SetLookupByHisCode(cboNgheNghiep, EcdsCatalogCache.DM_NGHENGHIEP, patient.CAREER_CODE);
                FillProvinceCommune(cboTinh, cboXa, patient.PROVINCE_CODE, patient.COMMUNE_CODE);
                FillProvinceCommune(cboTinhTru, cboXaTru, patient.HT_PROVINCE_CODE, patient.HT_COMMUNE_CODE);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Đặt giá trị combo theo mã HIS: tra ID ECDS trong danh mục, không thấy thì để trống.</summary>
        private void SetLookupByHisCode(LookUpEdit cbo, string tenDanhMuc, string hisCode)
        {
            try
            {
                if (cbo == null || string.IsNullOrEmpty(hisCode)) return;
                long? id = catalogCache.FindIdByMa(catalogCache.GetStatic(tenDanhMuc), hisCode);
                if (id.HasValue) cbo.EditValue = id.Value;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Map tỉnh + xã: đối chiếu mã tỉnh HIS -> ID tỉnh ECDS, nạp danh mục xã theo tỉnh (cascade) rồi map mã xã HIS.
        /// </summary>
        private void FillProvinceCommune(LookUpEdit cboProvince, LookUpEdit cboCommune,
            string hisProvinceCode, string hisCommuneCode)
        {
            try
            {
                if (string.IsNullOrEmpty(hisProvinceCode)) return;

                var provinces = catalogCache.GetStatic(EcdsCatalogCache.DM_TINH);
                var prov = provinces != null
                    ? provinces.FirstOrDefault(o => string.Equals(o.ma, hisProvinceCode, StringComparison.OrdinalIgnoreCase))
                    : null;
                if (prov == null) return;
                cboProvince.EditValue = prov.id;

                // Nạp xã theo mã tỉnh ECDS (cache theo tỉnh) rồi đối chiếu mã xã HIS.
                var communes = catalogCache.GetCascade(
                    EcdsCatalogCache.DM_XA,
                    new SearchDanhMucFastDto { maTinh = prov.ma },
                    prov.ma);
                SetupLookup(cboCommune, communes, "id", "ten");

                if (!string.IsNullOrEmpty(hisCommuneCode))
                {
                    long? xaId = catalogCache.FindIdByMa(communes, hisCommuneCode);
                    if (xaId.HasValue) cboCommune.EditValue = xaId.Value;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void FillNguoiBaoCaoTab()
        {
            try
            {
                txtNguoiBaoCao.Text = Inventec.UC.Login.Base.ClientTokenManagerStore
                    .ClientTokenManager.GetUserName() ?? "";
                lblMaDonViVal.Text = Config.EcdsConfigCFG.MaDonVi ?? "";

                var branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault();
                lblCoSoDieuTriVal.Text = branch != null ? branch.BRANCH_NAME : "";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void UpdatePushStatusLabel()
        {
            try
            {
                if (!string.IsNullOrEmpty(ecdsCaseCode))
                    lblPushStatus.Text = "✔ Đã đẩy — Mã ca bệnh: " + ecdsCaseCode;
                else
                    lblPushStatus.Text = "● Chưa đẩy lên cổng";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
    }
}
