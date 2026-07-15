/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities.Extensions;
namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private HIS_DHST dhstOverEighteen { get; set; }

        private void ResetControlOverEighteen()
        {
            try
            {
                spnHeight2.EditValue = null;
                spnPulse2.EditValue = null;
                spnWeight2.EditValue = null;
                spnBloodPressureMax2.EditValue = null;
                spnBloodPressureMin2.EditValue = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tab ≥18, CHỈ phần KHÁM LÂM SÀNG (14 vùng chuyên khoa): vùng nào ĐÃ nhập Người khám mà THIẾU
        /// kết quả và/hoặc phân loại -> 1 dòng lỗi. KHÔNG kiểm tra phần cận lâm sàng.
        /// </summary>
        private List<string> ValidateExaminerHasResultOverEighteen()
        {
            var errors = new List<string>();
            try
            {
                const string ksk = "Khám sức khỏe trên 18 tuổi";
                // 14 vùng khám lâm sàng: có Người khám + kết quả + phân loại.
                AddExamCheck(errors, ksk, "Tuần hoàn", cboExamCirculationLoginName2, HasText(txtExamCirculation2), cboExamCirculationRank2);
                AddExamCheck(errors, ksk, "Hô hấp", cboExamRespiratoryLoginName2, HasText(txtExamRespiratory2), cboExamRespiratoryRank2);
                AddExamCheck(errors, ksk, "Tiêu hóa", cboExamDigestionLoginName2, HasText(txtExamDigestion2), cboExamDigestionRank2);
                AddExamCheck(errors, ksk, "Thận - tiết niệu", cboExamKidneyUrologyLoginName2, HasText(txtExamKidneyUrology2), cboExamKidneyUrologyRank2);
                AddExamCheck(errors, ksk, "Nội tiết", cboExamOendLoginName2, HasText(txtExamOend2), cboExamOend2);
                AddExamCheck(errors, ksk, "Cơ - xương - khớp", cboExamMuscleBoneLoginName2, HasText(txtExamMuscleBone2), cboExamMuscleBoneRank2);
                AddExamCheck(errors, ksk, "Thần kinh", cboExamNeurologicalLoginName2, HasText(txtExamNeurological2), cboExamNeurologicalRank2);
                AddExamCheck(errors, ksk, "Tâm thần", cboExamMentalLoginName2, HasText(txtExamMental2), cboExamMentalRank2);
                AddExamCheck(errors, ksk, "Ngoại khoa", cboExamSurgeryLoginName2, HasText(txtExamSurgery2), cboExamSurgeryRank2);
                AddExamCheck(errors, ksk, "Sản phụ khoa", cboExamObstetricLoginName2, HasText(txtExamObstetric2), cboExamObstetricRank2);
                AddExamCheck(errors, ksk, "Da liễu", cboExamDermatologyLoginName2, HasText(txtExamDernatology2), cboExamDernatologyRank2);
                AddExamCheck(errors, ksk, "Mắt", cboExamEyeLoginName2,
                    HasAnyText(txtExamEyeSightRight2, txtExamEyeSightLeft2, txtExamEyeSightGlassRight2, txtExamEyeSightGlassLeft2, txtExamEyeDisease2),
                    cboExamEyeRank2);
                AddExamCheck(errors, ksk, "Tai mũi họng", cboExamEntLoginName2,
                    HasAnyText(txtExamEntLeftNormal2, txtExamEntRightNomal2, txtExamEntLeftWhisper2, txtExamEntRightWhisper2, txtExamEntDisease2),
                    cboExamEntDiseaseRank2);
                AddExamCheck(errors, ksk, "Răng hàm mặt", cboExamStomatologyLoginName2,
                    HasAnyText(txtExamStomatologyUpper2, txtExamStomatologyLower2, txtExamStomatologyDisease2),
                    cboExamStomatologyRank2);
                // CHỈ kiểm tra phần KHÁM LÂM SÀNG (14 vùng trên). KHÔNG kiểm tra cận lâm sàng
                // (máu/nước tiểu/CĐHA/CLS khác) theo yêu cầu.
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return errors;
        }

        /// <summary>Thêm 1 dòng lỗi nếu vùng ĐÃ có người khám mà thiếu kết quả/phân loại.</summary>
        private void AddExamCheck(List<string> errors, string ksk, string region,
            DevExpress.XtraEditors.GridLookUpEdit examiner, bool hasResult, DevExpress.XtraEditors.GridLookUpEdit classify)
        {
            try
            {
                if (examiner == null) return;
                bool hasExaminer = examiner.EditValue != null && !string.IsNullOrWhiteSpace(examiner.Text);
                bool hasClassify = (classify == null) || (classify.EditValue != null);

                if (hasExaminer)
                {
                    // ĐÃ có người khám -> BẮT BUỘC nhập đủ CẢ kết quả VÀ phân loại; thiếu cái nào cảnh báo cái đó.
                    if (hasResult && hasClassify) return;   // đủ -> hợp lệ
                    var missing = new List<string>();
                    if (!hasResult) missing.Add("kết quả");
                    if (classify != null && !hasClassify) missing.Add("phân loại");
                    string who = !string.IsNullOrWhiteSpace(examiner.Text) ? examiner.Text : examiner.EditValue.ToString();
                    errors.Add(string.Format("- [{0}] - {1}: đã nhập người khám \"{2}\" nhưng chưa nhập {3}.",
                        ksk, region, who, string.Join(" và ", missing.ToArray())));
                    return;
                }

                // CHƯA có người khám: nếu CHỈ nhập 1 trong 2 (kết quả HOẶC phân loại) -> báo lỗi (phải đủ cả 2).
                // Cả 2 trống (vùng chưa dùng) hoặc cả 2 đều nhập -> KHÔNG báo.
                if (classify == null) return;                 // vùng không có ô phân loại -> bỏ qua
                bool classifyFilled = (classify.EditValue != null);
                if (hasResult == classifyFilled) return;      // cùng trạng thái (đều trống / đều có) -> ok
                string entered = hasResult ? "kết quả" : "phân loại";
                string lack = hasResult ? "phân loại" : "kết quả";
                errors.Add(string.Format("- [{0}] - {1}: đã nhập {2} nhưng chưa nhập {3} (phải nhập đủ cả kết quả và phân loại).",
                    ksk, region, entered, lack));
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private static bool HasText(DevExpress.XtraEditors.BaseEdit edit)
        {
            return edit != null && !string.IsNullOrWhiteSpace(edit.Text);
        }

        private bool HasAnyText(params DevExpress.XtraEditors.BaseEdit[] edits)
        {
            if (edits == null) return false;
            foreach (var e in edits) if (HasText(e)) return true;
            return false;
        }

        private void FillDataPageOverEighteen()
        {
            try
            {
                ResetControlOverEighteen();
                SetDataCboRank(cboDhstRank2);
                SetDataCboRank(cboExamCirculationRank2);
                SetDataCboRank(cboExamRespiratoryRank2);
                SetDataCboRank(cboExamDigestionRank2);
                SetDataCboRank(cboExamKidneyUrologyRank2);
                SetDataCboRank(cboExamNeurologicalRank2);
                SetDataCboRank(cboExamMuscleBoneRank2);
                SetDataCboRank(cboExamMentalRank2);
                SetDataCboRank(cboExamSurgeryRank2);
                SetDataCboRank(cboExamObstetricRank2);
                SetDataCboRank(cboExamEyeRank2);
                SetDataCboRank(cboExamEntDiseaseRank2);
                SetDataCboRank(cboExamStomatologyRank2);
                SetDataCboRank(cboHealthExamRank2);
                SetDataCboRank(cboExamDernatologyRank2);
                SetDataCboRank(cboExamOend2);
                InitTextLibExamTooltips();
                SetDataCboExamLoginName(cboExecuteLoginName2);
                SetDataCboExamLoginName(cboExamEyeLoginName2);
                SetDataCboExamLoginName(cboExamEntLoginName2);
                SetDataCboExamLoginName(cboExamCirculationLoginName2);
                //dangth
                SetDataCboExamLoginName(cboExamRespiratoryLoginName2);
                SetDataCboExamLoginName(cboExamDigestionLoginName2);
                SetDataCboExamLoginName(cboExamKidneyUrologyLoginName2);
                SetDataCboExamLoginName(cboExamOendLoginName2);
                SetDataCboExamLoginName(cboExamMuscleBoneLoginName2);
                SetDataCboExamLoginName(cboExamNeurologicalLoginName2);
                SetDataCboExamLoginName(cboExamMentalLoginName2);
                SetDataCboExamLoginName(cboExamSurgeryLoginName2);
                SetDataCboExamLoginName(cboExamObstetricLoginName2);
                SetDataCboExamLoginName(cboExamStomatologyLoginName2);
                SetDataCboExamLoginName(cboExamDermatologyLoginName2);
                SetDataCboExamLoginName(cboTestBloodLoginName);
                SetDataCboExamLoginName(cboTestUrineLoginName);
                SetDataCboExamLoginName(cboDiimLoginName2);
                // 2 combo "Người khám" mới ở mục "Kết quả khám Cận lâm sàng khác" (giống các combo khác).
                SetDataCboExamLoginName(cboExamSubclinicalLoginName2);
                SetDataCboExamLoginName(cboExamSubclinicalLoginName2_2);
                // Combo Đối tượng (chọn nhiều) + Nguồn chi trả (chọn 1) — danh mục cố định QĐ 1551.
                InitAdminCombos();
                FillDataOverEighteen();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultGridOverE()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDiseaseTypeFilter Disfilter = new HisDiseaseTypeFilter();
                Disfilter.IS_ACTIVE = 1;
                Disfilter.IS_KSK_OVER_EIGHTEEN = 1;
                var dataVacine = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DISEASE_TYPE>>("api/HisDiseaseType/Get", ApiConsumers.MosConsumer, Disfilter, param);
                if (dataVacine != null && dataVacine.Count > 0)
                {
                    dataVacine = dataVacine.OrderBy(o => o.DISEASE_TYPE_CODE).ToList();
                    List<ADO.DiseaseTypeADO> lstAdo = new List<ADO.DiseaseTypeADO>();
                    foreach (var item in dataVacine)
                    {
                        ADO.DiseaseTypeADO ado = new ADO.DiseaseTypeADO();
                        ado.ID = item.ID;
                        ado.DISEASE_TYPE_NAME = item.DISEASE_TYPE_NAME;
                        ado.IS_NO = true;
                        lstAdo.Add(ado);
                    }
                    gridControl3.DataSource = new List<ADO.DiseaseTypeADO>();
                    gridControl3.DataSource = lstAdo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataOverEighteen()
        {
            try
            {
                if (currentServiceReq != null)
                {
                    CommonParam param = new CommonParam();
                    HisKskOverEighteenFilter filter = new HisKskOverEighteenFilter();
                    filter.SERVICE_REQ_ID = currentServiceReq.ID;
                    var data = preKskOverEighteens;
                    if (data != null && data.Count > 0)
                    {
                        currentKskOverEight = data.First();
                        txtPathologicalHistoryFamily.Text = currentKskOverEight.PATHOLOGICAL_HISTORY_FAMILY;
                        txtPathologicalHistory2.Text = currentKskOverEight.PATHOLOGICAL_HISTORY;
                        txtMedicineUsing.Text = currentKskOverEight.MEDICINE_USING;
                        txtMaternityHistory.Text = currentKskOverEight.MATERNITY_HISTORY;
                        // Đối tượng (KSK_PATIENT_TYPES "1;3;13") + Nguồn chi trả (KSK_PAY_SOURCE)
                        SetKskObjectValue(currentKskOverEight.KSK_PATIENT_TYPES);
                        cboPaymentSource.EditValue = currentKskOverEight.KSK_PAY_SOURCE != null
                            ? (int?)currentKskOverEight.KSK_PAY_SOURCE.Value : null;
                        cboDhstRank2.EditValue = currentKskOverEight.DHST_RANK;
                        txtExamCirculation2.Text = currentKskOverEight.EXAM_CIRCULATION;
                        cboExamCirculationRank2.EditValue = currentKskOverEight.EXAM_CIRCULATION_RANK;
                        txtExamRespiratory2.Text = currentKskOverEight.EXAM_RESPIRATORY;
                        cboExamRespiratoryRank2.EditValue = currentKskOverEight.EXAM_RESPIRATORY_RANK;
                        txtExamDigestion2.Text = currentKskOverEight.EXAM_DIGESTION;
                        cboExamDigestionRank2.EditValue = currentKskOverEight.EXAM_DIGESTION_RANK;
                        txtExamKidneyUrology2.Text = currentKskOverEight.EXAM_KIDNEY_UROLOGY;
                        cboExamKidneyUrologyRank2.EditValue = currentKskOverEight.EXAM_KIDNEY_UROLOGY_RANK;
                        txtExamNeurological2.Text = currentKskOverEight.EXAM_NEUROLOGICAL;
                        cboExamNeurologicalRank2.EditValue = currentKskOverEight.EXAM_NEUROLOGICAL_RANK;
                        txtExamMuscleBone2.Text = currentKskOverEight.EXAM_MUSCLE_BONE;
                        cboExamMuscleBoneRank2.EditValue = currentKskOverEight.EXAM_MUSCLE_BONE_RANK;
                        txtExamMental2.Text = currentKskOverEight.EXAM_MENTAL;
                        cboExamMentalRank2.EditValue = currentKskOverEight.EXAM_MENTAL_RANK;
                        txtExamSurgery2.Text = currentKskOverEight.EXAM_SURGERY;
                        cboExamSurgeryRank2.EditValue = currentKskOverEight.EXAM_SURGERY_RANK;
                        txtExamDernatology2.Text = currentKskOverEight.EXAM_DERMATOLOGY;
                        cboExamDernatologyRank2.EditValue = currentKskOverEight.EXAM_DERMATOLOGY_RANK;
                        txtExamObstetric2.Text = currentKskOverEight.EXAM_OBSTETRIC;
                        cboExamObstetricRank2.EditValue = currentKskOverEight.EXAM_OBSTETRIC_RANK;

                        txtExamEyeSightRight2.Text = currentKskOverEight.EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft2.Text = currentKskOverEight.EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight2.Text = currentKskOverEight.EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft2.Text = currentKskOverEight.EXAM_EYESIGHT_GLASS_LEFT;
                        txtExamEyeDisease2.Text = currentKskOverEight.EXAM_EYE_DISEASE;
                        cboExamEyeRank2.EditValue = currentKskOverEight.EXAM_EYE_RANK;
                        txtExamEntLeftNormal2.Text = currentKskOverEight.EXAM_ENT_LEFT_NORMAL;
                        txtExamEntLeftWhisper2.Text = currentKskOverEight.EXAM_ENT_LEFT_WHISPER;
                        txtExamEntRightNomal2.Text = currentKskOverEight.EXAM_ENT_RIGHT_NORMAL;
                        txtExamEntRightWhisper2.Text = currentKskOverEight.EXAM_ENT_RIGHT_WHISPER;
                        txtExamEntDisease2.Text = currentKskOverEight.EXAM_ENT_DISEASE;
                        cboExamEntDiseaseRank2.EditValue = currentKskOverEight.EXAM_ENT_RANK;
                        txtExamStomatologyUpper2.Text = currentKskOverEight.EXAM_STOMATOLOGY_UPPER;
                        txtExamStomatologyLower2.Text = currentKskOverEight.EXAM_STOMATOLOGY_LOWER;
                        txtExamStomatologyDisease2.Text = currentKskOverEight.EXAM_STOMATOLOGY_DISEASE;
                        cboExamStomatologyRank2.EditValue = currentKskOverEight.EXAM_STOMATOLOGY_RANK;

                        txtTestBloodHc2.Text = currentKskOverEight.TEST_BLOOD_HC;
                        txtTestBloodTc2.Text = currentKskOverEight.TEST_BLOOD_TC;
                        txtTestBloodBc2.Text = currentKskOverEight.TEST_BLOOD_BC;
                        // 3 ô HC/BC/TC đã ẩn — memo công thức máu gộp dữ liệu cũ (nếu còn BC/TC) về 1 ô.
                        txtTestBloodFormula2.Text = BuildTestBloodText(currentKskOverEight.TEST_BLOOD_HC, currentKskOverEight.TEST_BLOOD_BC, currentKskOverEight.TEST_BLOOD_TC);
                        txtTestBloodGluco2.Text = currentKskOverEight.TEST_BLOOD_GLUCO;
                        txtTestBloodUre2.Text = currentKskOverEight.TEST_BLOOD_URE;
                        txtTestBloodCreatinin2.Text = currentKskOverEight.TEST_BLOOD_CREATININ;
                        txtTestBloodAsat2.Text = currentKskOverEight.TEST_BLOOD_ASAT;
                        txtTestBloodAlat2.Text = currentKskOverEight.TEST_BLOOD_ALAT;
                        txtTestBloodOther2.Text = currentKskOverEight.TEST_BLOOD_OTHER;
                        txtTestUrineGluco2.Text = currentKskOverEight.TEST_URINE_GLUCO;
                        txtTestUrineProtein2.Text = currentKskOverEight.TEST_URINE_PROTEIN;
                        // 2 ô Đường/Protein niệu đã ẩn — memo XN nước tiểu gộp dữ liệu cũ (nếu còn Protein) về 1 ô.
                        txtTestUrineFormula2.Text = BuildTestUrineText(currentKskOverEight.TEST_URINE_GLUCO, currentKskOverEight.TEST_URINE_PROTEIN);
                        txtTestUrineOther2.Text = currentKskOverEight.TEST_URINE_OTHER;

                        txtResultDiim2.Text = currentKskOverEight.RESULT_DIIM;
                        cboHealthExamRank2.EditValue = currentKskOverEight.HEALTH_EXAM_RANK_ID;
                        txtDiseases2.Text = currentKskOverEight.DISEASES;
                        txtHealthExamRankDescription2.Text = currentKskOverEight.HEALTH_EXAM_RANK_DESCRIPTION;
                        txtExamOend2.Text = currentKskOverEight.EXAM_OEND;
                        cboExamOend2.EditValue = currentKskOverEight.EXAM_OEND_RANK;
                        if (currentKskOverEight.DHST_ID != null && currentKskOverEight.DHST_ID > 0)
                        {
                            HisDhstFilter dhstFilter = new HisDhstFilter();
                            dhstFilter.ID = currentKskOverEight.DHST_ID;
                            var dataDhst = PreGetDhst(dhstFilter.ID);   // cache prefetch (fallback API khi thieu)
                            if (dataDhst != null && dataDhst.Count > 0)
                            {
                                dhstOverEighteen = dataDhst.First();
                                spnHeight2.EditValue = dhstOverEighteen.HEIGHT;
                                spnPulse2.EditValue = dhstOverEighteen.PULSE;
                                spnWeight2.EditValue = dhstOverEighteen.WEIGHT;
                                spnBloodPressureMax2.EditValue = dhstOverEighteen.BLOOD_PRESSURE_MAX;
                                spnBloodPressureMin2.EditValue = dhstOverEighteen.BLOOD_PRESSURE_MIN;
                                //txtVirBmi.Text = currentDhst.VIR_BMI!=null ? currentDhst.VIR_BMI.ToString() : "";
                                FillNoteBMI(spnHeight2, spnWeight2, txtVirBmi2);
                                cboExecuteLoginName2.EditValue = dhstOverEighteen.EXECUTE_LOGINNAME;
                            }
                        }

                        cboExamEyeLoginName2.EditValue = currentKskOverEight.EXAM_EYE_LOGINNAME;
                        cboExamEntLoginName2.EditValue = currentKskOverEight.EXAM_ENT_LOGINNAME;
                        cboExamCirculationLoginName2.EditValue = currentKskOverEight.EXAM_CIRCULATION_LOGINNAME;
                        //dangth
                        cboExamRespiratoryLoginName2.EditValue = currentKskOverEight.EXAM_RESPIRATORY_LOGINNAME;
                        cboExamDigestionLoginName2.EditValue = currentKskOverEight.EXAM_DIGESTION_LOGINNAME;
                        cboExamKidneyUrologyLoginName2.EditValue = currentKskOverEight.EXAM_KIDNEY_UROLOGY_LOGINNAME;
                        cboExamOendLoginName2.EditValue = currentKskOverEight.EXAM_OEND_LOGINNAME;
                        cboExamMuscleBoneLoginName2.EditValue = currentKskOverEight.EXAM_MUSCLE_BONE_LOGINNAME;
                        cboExamNeurologicalLoginName2.EditValue = currentKskOverEight.EXAM_NEUROLOGICAL_LOGINNAME;
                        cboExamMentalLoginName2.EditValue = currentKskOverEight.EXAM_MENTAL_LOGINNAME;
                        cboExamSurgeryLoginName2.EditValue = currentKskOverEight.EXAM_SURGERY_LOGINNAME;
                        cboExamObstetricLoginName2.EditValue = currentKskOverEight.EXAM_OBSTETRIC_LOGINNAME;
                        cboExamDermatologyLoginName2.EditValue = currentKskOverEight.EXAM_DERMATOLOGY_LOGINNAME;
                        cboTestBloodLoginName.EditValue = currentKskOverEight.TEST_BLOOD;
                        cboTestUrineLoginName.EditValue = currentKskOverEight.TEST_URINE_LOGINNAME;
                        cboDiimLoginName2.EditValue = currentKskOverEight.DIIM_LOGINNAME;
                        cboExamStomatologyLoginName2.EditValue = currentKskOverEight.EXAM_STOMATOLOGY_LOGINNAME;

                        HisPeriodDriverDityFilter dityFilter = new HisPeriodDriverDityFilter();
                        dityFilter.KSK_OVER_EIGHTEEN_ID = currentKskOverEight.ID;
                        lstDataDriverDityOverE = preDitysOverE;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstDataDriverDityOverE), lstDataDriverDityOverE));
                        if (lstDataDriverDityOverE != null && lstDataDriverDityOverE.Count > 0)
                        {
                            HisDiseaseTypeFilter Disfilter = new HisDiseaseTypeFilter();
                            Disfilter.IS_ACTIVE = 1;
                            Disfilter.IS_KSK_OVER_EIGHTEEN = 1; // Chỉ lấy những bệnh trên 18 tuổi
                            Disfilter.IDs = lstDataDriverDityOverE.Select(o => o.DISEASE_TYPE_ID).ToList();
                            var dataVacine = preDiseaseTypesOverE;
                            if (dataVacine != null && dataVacine.Count > 0)
                            {
                                dataVacine = dataVacine.OrderBy(o => o.DISEASE_TYPE_CODE).ToList();
                                List<ADO.DiseaseTypeADO> lstAdo = new List<ADO.DiseaseTypeADO>();
                                foreach (var item in dataVacine)
                                {
                                    ADO.DiseaseTypeADO ado = new ADO.DiseaseTypeADO();
                                    ado.ID = item.ID;
                                    ado.DISEASE_TYPE_NAME = item.DISEASE_TYPE_NAME;
                                    var check = lstDataDriverDityOverE.Where(o => o.DISEASE_TYPE_ID == item.ID).FirstOrDefault();
                                    ado.PERIOD_DRIVER_DITY_ID = check.ID;
                                    var stt = check.IS_YES_NO;
                                    if (stt == "1")
                                    {
                                        ado.IS_YES = true;
                                    }
                                    else if (stt == "0")
                                    {
                                        ado.IS_NO = true;
                                    }
                                    lstAdo.Add(ado);
                                }
                                gridControl3.DataSource = new List<ADO.DiseaseTypeADO>();
                                gridControl3.DataSource = lstAdo;
                            }
                        }
                        else
                        {
                            SetDefaultGridOverE();
                        }
                    }
                    else
                    {
                        txtPathologicalHistoryFamily.Text = currentServiceReq.PATHOLOGICAL_HISTORY_FAMILY;
                        txtPathologicalHistory2.Text = currentServiceReq.PATHOLOGICAL_HISTORY;
                        txtExamCirculation2.Text = currentServiceReq.PART_EXAM_CIRCULATION;
                        txtExamRespiratory2.Text = currentServiceReq.PART_EXAM_RESPIRATORY;
                        txtExamDigestion2.Text = currentServiceReq.PART_EXAM_DIGESTION;
                        txtExamKidneyUrology2.Text = currentServiceReq.PART_EXAM_KIDNEY_UROLOGY;
                        txtExamMuscleBone2.Text = currentServiceReq.PART_EXAM_MUSCLE_BONE;
                        txtExamNeurological2.Text = currentServiceReq.PART_EXAM_NEUROLOGICAL;
                        txtExamMental2.Text = currentServiceReq.PART_EXAM_MENTAL;
                        txtExamObstetric2.Text = currentServiceReq.PART_EXAM_OBSTETRIC;

                        txtExamEyeSightRight2.Text = currentServiceReq.PART_EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft2.Text = currentServiceReq.PART_EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight2.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft2.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_LEFT;

                        txtExamEntLeftNormal2.Text = currentServiceReq.PART_EXAM_EAR_LEFT_NORMAL;
                        txtExamEntLeftWhisper2.Text = currentServiceReq.PART_EXAM_EAR_LEFT_WHISPER;
                        txtExamEntRightNomal2.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_NORMAL;
                        txtExamEntRightWhisper2.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_WHISPER;

                        txtExamStomatologyUpper2.Text = currentServiceReq.PART_EXAM_UPPER_JAW;
                        txtExamStomatologyLower2.Text = currentServiceReq.PART_EXAM_LOWER_JAW;
                        txtExamDernatology2.Text = currentServiceReq.PART_EXAM_DERMATOLOGY;
                        txtExamSurgery2.Text = currentServiceReq.SUBCLINICAL;
                        txtHealthExamRankDescription2.Text = null;
                        txtExamOend2.Text = null;
                        cboExamOend2.EditValue = null;
                        // y lệnh chưa có bản ghi ≥18 -> clear Đối tượng + Nguồn chi trả để không dính giá trị BN trước.
                        SetKskObjectValue("");
                        cboPaymentSource.EditValue = null;
                        if (currentServiceReq.DHST_ID != null && currentServiceReq.DHST_ID > 0)
                        {
                            HisDhstFilter dhstFilter = new HisDhstFilter();
                            dhstFilter.ID = currentServiceReq.DHST_ID;
                            var dhstParamSr = new Inventec.Core.CommonParam();
                            var dataDhst = new Inventec.Common.Adapter.BackendAdapter(dhstParamSr).Get<System.Collections.Generic.List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, dhstFilter, dhstParamSr);   // currentServiceReq.DHST_ID: giu nguyen goi API (khong lay tu SDO)
                            if (dataDhst != null && dataDhst.Count > 0)
                            {
                                var currentDhst = dataDhst.First();
                                spnHeight2.EditValue = currentDhst.HEIGHT;
                                spnPulse2.EditValue = currentDhst.PULSE;
                                spnWeight2.EditValue = currentDhst.WEIGHT;
                                spnBloodPressureMax2.EditValue = currentDhst.BLOOD_PRESSURE_MAX;
                                spnBloodPressureMin2.EditValue = currentDhst.BLOOD_PRESSURE_MIN;
                                //txtVirBmi.Text = currentDhst.VIR_BMI!=null ? currentDhst.VIR_BMI.ToString() : "";
                                FillNoteBMI(spnHeight2, spnWeight2, txtVirBmi2);
                            }
                        }
                        SetDefaultGridOverE();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnHeight2_EditValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                FillNoteBMI(spnHeight2, spnWeight2, txtVirBmi2);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnWeight2_EditValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                FillNoteBMI(spnHeight2, spnWeight2, txtVirBmi2);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_KSK_OVER_EIGHTEEN GetValueOverEighteen()
        {
            HIS_KSK_OVER_EIGHTEEN obj = new HIS_KSK_OVER_EIGHTEEN();
            try
            {
                if (currentKskOverEight != null)
                    obj.ID = currentKskOverEight.ID;
                obj.PATHOLOGICAL_HISTORY_FAMILY = txtPathologicalHistoryFamily.Text;
                obj.PATHOLOGICAL_HISTORY = txtPathologicalHistory2.Text;
                obj.MEDICINE_USING = txtMedicineUsing.Text;
                obj.MATERNITY_HISTORY = txtMaternityHistory.Text;
                // Đối tượng: các mã 1-16 phân cách ";" (đặc tả QĐ 1551) + Nguồn chi trả: 1 mã số
                string kskPatientTypes = GetKskObjectValue();
                obj.KSK_PATIENT_TYPES = !string.IsNullOrEmpty(kskPatientTypes) ? kskPatientTypes : null;
                obj.KSK_PAY_SOURCE = cboPaymentSource.EditValue != null
                    ? (short?)Convert.ToInt16(cboPaymentSource.EditValue) : null;
                //DHST
                obj.DHST_RANK = cboDhstRank2.EditValue != null ? (long?)Int64.Parse(cboDhstRank2.EditValue.ToString()) : null;
                obj.EXAM_CIRCULATION = txtExamCirculation2.Text;
                obj.EXAM_CIRCULATION_RANK = cboExamCirculationRank2.EditValue != null ? (long?)Int64.Parse(cboExamCirculationRank2.EditValue.ToString()) : null;
                obj.EXAM_RESPIRATORY = txtExamRespiratory2.Text;
                obj.EXAM_RESPIRATORY_RANK = cboExamRespiratoryRank2.EditValue != null ? (long?)Int64.Parse(cboExamRespiratoryRank2.EditValue.ToString()) : null;
                obj.EXAM_DIGESTION = txtExamDigestion2.Text;
                obj.EXAM_DIGESTION_RANK = cboExamDigestionRank2.EditValue != null ? (long?)Int64.Parse(cboExamDigestionRank2.EditValue.ToString()) : null;
                obj.EXAM_KIDNEY_UROLOGY = txtExamKidneyUrology2.Text;
                obj.EXAM_KIDNEY_UROLOGY_RANK = cboExamKidneyUrologyRank2.EditValue != null ? (long?)Int64.Parse(cboExamKidneyUrologyRank2.EditValue.ToString()) : null;
                obj.EXAM_NEUROLOGICAL = txtExamNeurological2.Text;
                obj.EXAM_NEUROLOGICAL_RANK = cboExamNeurologicalRank2.EditValue != null ? (long?)Int64.Parse(cboExamNeurologicalRank2.EditValue.ToString()) : null;
                obj.EXAM_MUSCLE_BONE = txtExamMuscleBone2.Text;
                obj.EXAM_MUSCLE_BONE_RANK = cboExamMuscleBoneRank2.EditValue != null ? (long?)Int64.Parse(cboExamMuscleBoneRank2.EditValue.ToString()) : null;
                obj.EXAM_MENTAL = txtExamMental2.Text;
                obj.EXAM_MENTAL_RANK = cboExamMentalRank2.EditValue != null ? (long?)Int64.Parse(cboExamMentalRank2.EditValue.ToString()) : null;
                obj.EXAM_SURGERY = txtExamSurgery2.Text;
                obj.EXAM_SURGERY_RANK = cboExamSurgeryRank2.EditValue != null ? (long?)Int64.Parse(cboExamSurgeryRank2.EditValue.ToString()) : null;
                obj.EXAM_DERMATOLOGY = txtExamDernatology2.Text;
                obj.EXAM_DERMATOLOGY_RANK = cboExamDernatologyRank2.EditValue != null ? (long?)Int64.Parse(cboExamDernatologyRank2.EditValue.ToString()) : null;
                obj.EXAM_OBSTETRIC = txtExamObstetric2.Text;
                obj.EXAM_OBSTETRIC_RANK = cboExamObstetricRank2.EditValue != null ? (long?)Int64.Parse(cboExamObstetricRank2.EditValue.ToString()) : null;

                obj.EXAM_EYESIGHT_RIGHT = txtExamEyeSightRight2.Text;
                obj.EXAM_EYESIGHT_LEFT = txtExamEyeSightLeft2.Text;
                obj.EXAM_EYESIGHT_GLASS_RIGHT = txtExamEyeSightGlassRight2.Text;
                obj.EXAM_EYESIGHT_GLASS_LEFT = txtExamEyeSightGlassLeft2.Text;
                obj.EXAM_EYE_DISEASE = txtExamEyeDisease2.Text;
                obj.EXAM_EYE_RANK = cboExamEyeRank2.EditValue != null ? (long?)Int64.Parse(cboExamEyeRank2.EditValue.ToString()) : null;
                obj.EXAM_ENT_LEFT_NORMAL = txtExamEntLeftNormal2.Text;
                obj.EXAM_ENT_LEFT_WHISPER = txtExamEntLeftWhisper2.Text;
                obj.EXAM_ENT_RIGHT_NORMAL = txtExamEntRightNomal2.Text;
                obj.EXAM_ENT_RIGHT_WHISPER = txtExamEntRightWhisper2.Text;
                obj.EXAM_ENT_DISEASE = txtExamEntDisease2.Text;
                obj.EXAM_ENT_RANK = cboExamEntDiseaseRank2.EditValue != null ? (long?)Int64.Parse(cboExamEntDiseaseRank2.EditValue.ToString()) : null;
                obj.EXAM_STOMATOLOGY_UPPER = txtExamStomatologyUpper2.Text;
                obj.EXAM_STOMATOLOGY_LOWER = txtExamStomatologyLower2.Text;
                obj.EXAM_STOMATOLOGY_DISEASE = txtExamStomatologyDisease2.Text;
                obj.EXAM_STOMATOLOGY_RANK = cboExamStomatologyRank2.EditValue != null ? (long?)Int64.Parse(cboExamStomatologyRank2.EditValue.ToString()) : null;
                // Lưu memo công thức máu vào trường Hồng cầu; BC/TC truyền null để dồn dữ liệu về 1 trường.
                obj.TEST_BLOOD_HC = txtTestBloodFormula2.Text;
                obj.TEST_BLOOD_BC = null;
                obj.TEST_BLOOD_TC = null;
                obj.TEST_BLOOD_GLUCO = txtTestBloodGluco2.Text;
                obj.TEST_BLOOD_URE = txtTestBloodUre2.Text;
                obj.TEST_BLOOD_CREATININ = txtTestBloodCreatinin2.Text;
                obj.TEST_BLOOD_ASAT = txtTestBloodAsat2.Text;
                obj.TEST_BLOOD_ALAT = txtTestBloodAlat2.Text;
                obj.TEST_BLOOD_OTHER = txtTestBloodOther2.Text;
                // Lưu memo XN nước tiểu vào trường Đường; Protein truyền null để dồn dữ liệu về 1 trường.
                obj.TEST_URINE_GLUCO = txtTestUrineFormula2.Text;
                obj.TEST_URINE_PROTEIN = null;
                obj.TEST_URINE_OTHER = txtTestUrineOther2.Text;
                obj.RESULT_DIIM = txtResultDiim2.Text;
                obj.HEALTH_EXAM_RANK_ID = cboHealthExamRank2.EditValue != null ? (long?)Int64.Parse(cboHealthExamRank2.EditValue.ToString()) : null;
                obj.DISEASES = txtDiseases2.Text;
                obj.HEALTH_EXAM_RANK_DESCRIPTION = txtHealthExamRankDescription2.Text.Trim();
                obj.EXAM_OEND = txtExamOend2.Text.Trim();
                obj.EXAM_OEND_RANK = cboExamOend2.EditValue != null ? (long?)Int64.Parse(cboExamOend2.EditValue.ToString()) : null;

                obj.EXAM_CIRCULATION_LOGINNAME = cboExamCirculationLoginName2.EditValue != null ? cboExamCirculationLoginName2.EditValue.ToString() : null;
                //dangth
                obj.EXAM_RESPIRATORY_LOGINNAME = cboExamRespiratoryLoginName2.EditValue != null ? cboExamRespiratoryLoginName2.EditValue.ToString() : null;
                obj.EXAM_DIGESTION_LOGINNAME = cboExamDigestionLoginName2.EditValue != null ? cboExamDigestionLoginName2.EditValue.ToString() : null;
                obj.EXAM_KIDNEY_UROLOGY_LOGINNAME = cboExamKidneyUrologyLoginName2.EditValue != null ? cboExamKidneyUrologyLoginName2.EditValue.ToString() : null;
                obj.EXAM_OEND_LOGINNAME = cboExamOendLoginName2.EditValue != null ? cboExamOendLoginName2.EditValue.ToString() : null;
                obj.EXAM_MUSCLE_BONE_LOGINNAME = cboExamMuscleBoneLoginName2.EditValue != null ? cboExamMuscleBoneLoginName2.EditValue.ToString() : null;
                obj.EXAM_NEUROLOGICAL_LOGINNAME = cboExamNeurologicalLoginName2.EditValue != null ? cboExamNeurologicalLoginName2.EditValue.ToString() : null;
                obj.EXAM_MENTAL_LOGINNAME = cboExamMentalLoginName2.EditValue != null ? cboExamMentalLoginName2.EditValue.ToString() : null;
                obj.EXAM_SURGERY_LOGINNAME = cboExamSurgeryLoginName2.EditValue != null ? cboExamSurgeryLoginName2.EditValue.ToString() : null;
                obj.EXAM_OBSTETRIC_LOGINNAME = cboExamObstetricLoginName2.EditValue != null ? cboExamObstetricLoginName2.EditValue.ToString() : null;
                obj.EXAM_EYE_LOGINNAME = cboExamEyeLoginName2.EditValue != null ? cboExamEyeLoginName2.EditValue.ToString() : null;
                obj.EXAM_ENT_LOGINNAME = cboExamEntLoginName2.EditValue != null ? cboExamEntLoginName2.EditValue.ToString() : null;
                obj.EXAM_STOMATOLOGY_LOGINNAME = cboExamStomatologyLoginName2.EditValue != null ? cboExamStomatologyLoginName2.EditValue.ToString() : null;
                obj.EXAM_DERMATOLOGY_LOGINNAME = cboExamDermatologyLoginName2.EditValue != null ? cboExamDermatologyLoginName2.EditValue.ToString() : null;
                obj.TEST_BLOOD = cboTestBloodLoginName.EditValue != null ? cboTestBloodLoginName.EditValue.ToString() : null;
                obj.TEST_URINE_LOGINNAME = cboTestUrineLoginName.EditValue != null ? cboTestUrineLoginName.EditValue.ToString() : null;
                obj.DIIM_LOGINNAME = cboDiimLoginName2.EditValue != null ? cboDiimLoginName2.EditValue.ToString() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }

        private HIS_DHST GetDhstOverighteen()
        {
            HIS_DHST obj = new HIS_DHST();
            try
            {
                if (dhstOverEighteen != null)
                    obj.ID = dhstOverEighteen.ID;
                if (spnBloodPressureMax2.EditValue != null)
                    obj.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spnBloodPressureMax2.Value.ToString());
                if (spnBloodPressureMin2.EditValue != null)
                    obj.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spnBloodPressureMin2.Value.ToString());
                if (spnHeight2.EditValue != null)
                    obj.HEIGHT = Inventec.Common.Number.Get.RoundCurrency(spnHeight2.Value, 2);
                if (spnPulse2.EditValue != null)
                    obj.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spnPulse2.Value.ToString());
                if (spnWeight2.EditValue != null)
                    obj.WEIGHT = Inventec.Common.Number.Get.RoundCurrency(spnWeight2.Value, 2);

                obj.EXECUTE_LOGINNAME = cboExecuteLoginName2.EditValue != null ? cboExecuteLoginName2.EditValue.ToString() : null;
                obj.EXECUTE_USERNAME = obj.EXECUTE_LOGINNAME != null ? BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == obj.EXECUTE_LOGINNAME)?.TDL_USERNAME : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }
        private List<HIS_PERIOD_DRIVER_DITY> GetDriverDityOverE()
        {
            List<HIS_PERIOD_DRIVER_DITY> obj = new List<HIS_PERIOD_DRIVER_DITY>();
            try
            {
                var Alls = gridControl3.DataSource as List<ADO.DiseaseTypeADO>;

                if (Alls != null && Alls.Count > 0)
                {
                    if (currentKskOverEight != null && lstDataDriverDityOverE != null && lstDataDriverDityOverE.Count > 0)
                    {
                        foreach (var item in Alls)
                        {
                            HIS_PERIOD_DRIVER_DITY i = new HIS_PERIOD_DRIVER_DITY();
                            i.ID = item.PERIOD_DRIVER_DITY_ID;
                            i.DISEASE_TYPE_ID = item.ID;
                            i.IS_YES_NO = null;
                            if (item.IS_YES) i.IS_YES_NO = "1";
                            if (item.IS_NO) i.IS_YES_NO = "0";
                            obj.Add(i);
                        }
                    }
                    else
                    {
                        foreach (var item in Alls)
                        {
                            HIS_PERIOD_DRIVER_DITY i = new HIS_PERIOD_DRIVER_DITY();
                            i.DISEASE_TYPE_ID = item.ID;
                            i.IS_YES_NO = null;
                            if (item.IS_YES) i.IS_YES_NO = "1";
                            if (item.IS_NO) i.IS_YES_NO = "0";
                            obj.Add(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }


        private void cboHealthExamRank2_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                if (cboHealthExamRank2.EditValue != null)
                {
                    txtHealthExamRankDescription2.Text = data.FirstOrDefault(o => o.ID == Int64.Parse(cboHealthExamRank2.EditValue.ToString())).DESCRIPTION;
                }
                else
                    txtHealthExamRankDescription2.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repositoryItemCheckEdit6_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var focusRow = (ADO.DiseaseTypeADO)gridView4.GetFocusedRow();
                if (!focusRow.IS_YES)
                {
                    focusRow.IS_YES = true;
                    if (focusRow.IS_NO)
                    {
                        focusRow.IS_NO = false;
                    }
                }
                else
                {
                    focusRow.IS_YES = false;
                }
                ReloadGrid4(focusRow);

            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEdit7_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var focusRow = (ADO.DiseaseTypeADO)gridView4.GetFocusedRow();
                if (!focusRow.IS_NO)
                {
                    focusRow.IS_NO = true;
                    if (focusRow.IS_YES)
                    {
                        focusRow.IS_YES = false;
                    }
                }
                else
                {
                    focusRow.IS_NO = false;
                }
                ReloadGrid4(focusRow);

            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadGrid4(DiseaseTypeADO focusRow)
        {
            try
            {
                var Alls = gridControl3.DataSource as List<ADO.DiseaseTypeADO>;
                int count = 0;
                foreach (var item in Alls)
                {
                    if (item.ID == focusRow.ID)
                    {
                        item.IS_YES = focusRow.IS_YES;
                        item.IS_NO = focusRow.IS_NO;
                        break;
                    }
                    count++;
                }
                gridControl3.DataSource = new List<ADO.DiseaseTypeADO>();
                gridControl3.DataSource = Alls;
                gridView4.FocusedRowHandle = count;
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView4_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (ADO.DiseaseTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodOther2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameSItem = ENameSItem.KHAC_XNM_2;
            GetSpecInformation(ReturnObject = false);
        }

        private void txtTestUrineOther2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameSItem = ENameSItem.KHAC_XNNT_2;
            GetSpecInformation(ReturnObject = false);
        }

        private void txtResultDiim2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameSItem = ENameSItem.CDHA_2;
            GetSpecInformation(ReturnObject = false);
        }
        private void txtTestBloodHc2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.SL_HC_2;
            GetSpecInformation();
        }

        /// <summary>Nút "+" cạnh memo công thức máu — mở chọn kết quả CLS như CDHA (ReturnObject=false), nối bằng ";".</summary>
        private void btnPickTestBlood2_Click(object sender, EventArgs e)
        {
            NameSItem = ENameSItem.CTM_2;
            GetSpecInformation(ReturnObject = false);
        }

        /// <summary>Gộp dữ liệu cũ 3 ô công thức máu về 1 chuỗi: chỉ có Hồng cầu thì trả nguyên, còn BC/TC cũ thì ghép kèm nhãn.</summary>
        private string BuildTestBloodText(string hc, string bc, string tc)
        {
            if (string.IsNullOrEmpty(bc) && string.IsNullOrEmpty(tc))
                return hc;
            List<string> parts = new List<string>();
            if (!string.IsNullOrEmpty(hc)) parts.Add("Hồng cầu: " + hc);
            if (!string.IsNullOrEmpty(bc)) parts.Add("Bạch cầu: " + bc);
            if (!string.IsNullOrEmpty(tc)) parts.Add("Số lượng TC: " + tc);
            return string.Join(Environment.NewLine, parts);
        }

        private void txtTestBloodBc2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.SL_BC_2;
            GetSpecInformation();
        }

        private void txtTestBloodTc2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.SL_TC_2;
            GetSpecInformation();
        }

        private void txtTestBloodGluco2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.DMA_2;
            GetSpecInformation();
        }

        private void txtTestBloodUre2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.URE_2;
            GetSpecInformation();
        }

        private void txtTestBloodCreatinin2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.CRE_2;
            GetSpecInformation();
        }

        private void txtTestBloodAsat2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.ASA_2;
            GetSpecInformation();
        }

        private void txtTestBloodAlat2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.ALA_2;
            GetSpecInformation();

        }

        private void txtTestUrineGluco2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.DUO_2;
            GetSpecInformation();
        }

        private void txtTestUrineProtein2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.PRO_2;
            GetSpecInformation();
        }

        /// <summary>Nút "+" cạnh memo XN nước tiểu — mở chọn kết quả CLS như CDHA (ReturnObject=false), nối bằng ";".</summary>
        private void btnPickTestUrine2_Click(object sender, EventArgs e)
        {
            NameSItem = ENameSItem.NUOC_TIEU_2;
            GetSpecInformation(ReturnObject = false);
        }

        /// <summary>Gộp dữ liệu cũ 2 ô XN nước tiểu (Đường/Protein) về 1 chuỗi: chỉ có Đường thì trả nguyên, còn Protein cũ thì ghép kèm nhãn.</summary>
        private string BuildTestUrineText(string gluco, string protein)
        {
            if (string.IsNullOrEmpty(protein))
                return gluco;
            List<string> parts = new List<string>();
            if (!string.IsNullOrEmpty(gluco)) parts.Add("Đường: " + gluco);
            parts.Add("Protein: " + protein);
            return string.Join(Environment.NewLine, parts);
        }


        #region ---PREVIEWKEYDOWN---
        private void txtPathologicalHistoryFamily_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPathologicalHistory2.Focus();
                    txtPathologicalHistory2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPathologicalHistory2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineUsing.Focus();
                    txtMedicineUsing.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineUsing_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaternityHistory.Focus();
                    txtMaternityHistory.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaternityHistory_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnHeight2.Focus();
                    spnHeight2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnHeight2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnWeight2.Focus();
                    spnWeight2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnWeight2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnPulse2.Focus();
                    spnPulse2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnPulse2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnBloodPressureMax2.Focus();
                    spnBloodPressureMax2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnBloodPressureMax2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnBloodPressureMin2.Focus();
                    spnBloodPressureMin2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnBloodPressureMin2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDhstRank2.Focus();
                    cboDhstRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDhstRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamCirculation2.Focus();
                    txtExamCirculation2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamCirculation2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamCirculationRank2.Focus();
                    cboExamCirculationRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamCirculationRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamRespiratory2.Focus();
                    txtExamRespiratory2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamRespiratory2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamRespiratoryRank2.Focus();
                    cboExamRespiratoryRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamRespiratoryRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamDigestion2.Focus();
                    txtExamDigestion2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamDigestion2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamDigestionRank2.Focus();
                    cboExamDigestionRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamDigestionRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamKidneyUrology2.Focus();
                    txtExamKidneyUrology2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamKidneyUrology2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamKidneyUrologyRank2.Focus();
                    cboExamKidneyUrologyRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamKidneyUrologyRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamMuscleBone2.Focus();
                    txtExamMuscleBone2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMuscleBone2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMuscleBoneRank2.Focus();
                    cboExamMuscleBoneRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMuscleBoneRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamNeurological2.Focus();
                    txtExamNeurological2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamNeurological2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamNeurologicalRank2.Focus();
                    cboExamNeurologicalRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamNeurologicalRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamMental2.Focus();
                    txtExamMental2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMental2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMentalRank2.Focus();
                    cboExamMentalRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMentalRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamSurgery2.Focus();
                    txtExamSurgery2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamSurgery2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamSurgeryRank2.Focus();
                    cboExamSurgeryRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamSurgeryRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamObstetric2.Focus();
                    txtExamObstetric2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamObstetric2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamObstetricRank2.Focus();
                    cboExamObstetricRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamObstetricRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEyeSightRight2.Focus();
                    txtExamEyeSightRight2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightRight2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightLeft2.Focus();
                    txtExamEyeSightLeft2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightLeft2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassRight2.Focus();
                    txtExamEyeSightGlassRight2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassRight2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassLeft2.Focus();
                    txtExamEyeSightGlassLeft2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassLeft2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeDisease2.Focus();
                    txtExamEyeDisease2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeDisease2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEyeRank2.Focus();
                    cboExamEyeRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEyeRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEntLeftNormal2.Focus();
                    txtExamEntLeftNormal2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftNormal2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntLeftWhisper2.Focus();
                    txtExamEntLeftWhisper2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftWhisper2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightNomal2.Focus();
                    txtExamEntRightNomal2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightNomal2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightWhisper2.Focus();
                    txtExamEntRightWhisper2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightWhisper2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntDisease2.Focus();
                    txtExamEntDisease2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntDisease2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEntDiseaseRank2.Focus();
                    cboExamEntDiseaseRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEntDiseaseRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamStomatologyUpper2.Focus();
                    txtExamStomatologyUpper2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyUpper2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamStomatologyLower2.Focus();
                    txtExamStomatologyLower2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyLower2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamStomatologyDisease2.Focus();
                    txtExamStomatologyDisease2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyDisease2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamStomatologyRank2.Focus();
                    cboExamStomatologyRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamStomatologyRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamDernatology2.Focus();
                    txtExamDernatology2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamDernatology2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamDernatologyRank2.Focus();
                    cboExamDernatologyRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamDernatologyRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtTestBloodHc2.Focus();
                    txtTestBloodHc2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodHc2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodBc2.Focus();
                    txtTestBloodBc2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodBc2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodTc2.Focus();
                    txtTestBloodTc2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodTc2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodGluco2.Focus();
                    txtTestBloodGluco2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodGluco2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodUre2.Focus();
                    txtTestBloodUre2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodUre2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodCreatinin2.Focus();
                    txtTestBloodCreatinin2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodCreatinin2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodAsat2.Focus();
                    txtTestBloodAsat2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodAsat2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodAlat2.Focus();
                    txtTestBloodAlat2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodAlat2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodOther2.Focus();
                    txtTestBloodOther2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodOther2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestUrineGluco2.Focus();
                    txtTestUrineGluco2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestUrineGluco2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestUrineProtein2.Focus();
                    txtTestUrineProtein2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestUrineProtein2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestUrineOther2.Focus();
                    txtTestUrineOther2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestUrineOther2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtResultDiim2.Focus();
                    txtResultDiim2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtResultDiim2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboHealthExamRank2.Focus();
                    cboHealthExamRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHealthExamRank2_Closed(object sender, ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtHealthExamRankDescription2.Focus();
                    txtHealthExamRankDescription2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiseases2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        #endregion

        #region Kết quả CLS khác + Đối tượng / Nguồn chi trả (tab "Ksk trên 18 tuổi")

        // Nút "+" chọn kết quả cận lâm sàng cho 2 ô Kết quả mới (giống tab lái xe).
        private void btnPickResultSubclinical2_Click(object sender, EventArgs e)
        {
            NameSItem = ENameSItem.KET_QUA_2;
            GetSpecInformation(ReturnObject = false);
        }

        private void btnPickResultDiim2_Click(object sender, EventArgs e)
        {
            NameSItem = ENameSItem.CDHA_2;
            GetSpecInformation(ReturnObject = false);
        }

        private void btnPickResultSubclinical2_2_Click(object sender, EventArgs e)
        {
            NameSItem = ENameSItem.KET_QUA_2_2;
            GetSpecInformation(ReturnObject = false);
        }

        // Cờ đảm bảo chỉ khởi tạo combo Đối tượng/Nguồn chi trả 1 lần (tránh thêm trùng cột).
        private bool adminCombosInited = false;

        /// <summary>Khởi tạo combo Đối tượng (chọn nhiều, checkbox) + Nguồn chi trả (chọn 1) — y hệt cboEXECUTE_ROOM_NAME.</summary>
        private void InitAdminCombos()
        {
            try
            {
                if (adminCombosInited) return;
                adminCombosInited = true;
                // Thứ tự y hệt HisKskDriverList: Check trước, Combo sau (MultiSelect set lại sau khi gán DataSource).
                InitObjectCheck();
                InitObjectCombo();
                InitPaymentSourceCombo();
                // Nút cạnh Lý do khám: mở Thư viện văn bản (giống btnLyDoKham ExamServiceReqExecute).
                btnLyDoKham.Click -= btnLyDoKham_Click;
                btnLyDoKham.Click += btnLyDoKham_Click;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        // key định tuyến kết quả chọn từ Thư viện văn bản về đúng ô (1 = Lý do khám, 2 = ô Kết quả khám lâm sàng theo textLibTargetEdit).
        private int keyTextLib = 0;

        // Ô Kết quả đích nhận nội dung chọn từ Thư viện văn bản (dùng cho keyTextLib = 2).
        private DevExpress.XtraEditors.BaseEdit textLibTargetEdit = null;
        // Ô Phân loại đích (GridLookUpEdit rank) tương ứng vùng đang mở Thư viện — dùng để tự điền theo "PL:Lx".
        private DevExpress.XtraEditors.GridLookUpEdit textLibTargetClassify = null;

        /// <summary>Nút cạnh Lý do khám — mở Thư viện văn bản, chèn nội dung đã chọn (giống btnLyDoKham_Click).</summary>
        private void btnLyDoKham_Click(object sender, EventArgs e)
        {
            try
            {
                keyTextLib = 1;
                OpenModuleTextLibrary(txtLyDoKham.Text, "LyDoKham");
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Nút cạnh Người khám các mục tab Khám lâm sàng — mở Thư viện văn bản, chèn nội dung vào ô Kết quả tương ứng.</summary>
        private void btnTextLibExamResult_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender == btnTextLibEye2) OpenTextLibEye(); // phần Mắt: 1 mẫu điền nhiều ô
                else if (sender == btnTextLibEnt2) OpenTextLibEnt(); // Tai mũi họng: 1 mẫu điền nhiều ô
                else if (sender == btnTextLibStomatology2) OpenTextLibStomatology(); // Răng hàm mặt: 1 mẫu điền nhiều ô
                else if (sender == btnTextLibCirculation2) OpenTextLibExamResult(txtExamCirculation2, "KhamTuanHoan", cboExamCirculationRank2);
                else if (sender == btnTextLibRespiratory2) OpenTextLibExamResult(txtExamRespiratory2, "KhamHoHap", cboExamRespiratoryRank2);
                else if (sender == btnTextLibDigestion2) OpenTextLibExamResult(txtExamDigestion2, "KhamTieuHoa", cboExamDigestionRank2);
                else if (sender == btnTextLibKidneyUrology2) OpenTextLibExamResult(txtExamKidneyUrology2, "KhamThanTietNieu", cboExamKidneyUrologyRank2);
                else if (sender == btnTextLibOend2) OpenTextLibExamResult(txtExamOend2, "KhamNoiTiet", cboExamOend2);
                else if (sender == btnTextLibMuscleBone2) OpenTextLibExamResult(txtExamMuscleBone2, "KhamCoXuongKhop", cboExamMuscleBoneRank2);
                else if (sender == btnTextLibNeurological2) OpenTextLibExamResult(txtExamNeurological2, "KhamThanKinh", cboExamNeurologicalRank2);
                else if (sender == btnTextLibMental2) OpenTextLibExamResult(txtExamMental2, "KhamTamThan", cboExamMentalRank2);
                else if (sender == btnTextLibSurgery2) OpenTextLibExamResult(txtExamSurgery2, "KhamNgoaiKhoa", cboExamSurgeryRank2);
                else if (sender == btnTextLibObstetric2) OpenTextLibExamResult(txtExamObstetric2, "KhamSanPhuKhoa", cboExamObstetricRank2);
                else if (sender == btnTextLibDermatology2) OpenTextLibExamResult(txtExamDernatology2, "KhamDaLieu", cboExamDernatologyRank2);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void OpenTextLibExamResult(DevExpress.XtraEditors.BaseEdit target, string hashtag, DevExpress.XtraEditors.GridLookUpEdit classify)
        {
            keyTextLib = 2;
            textLibTargetEdit = target;
            textLibTargetClassify = classify;
            OpenModuleTextLibrary(target.Text, hashtag);
        }

        /// <summary>
        /// Tách token phân loại "PL:Lx" (x = 1..5) khỏi nội dung mẫu. Trả về mức x (0 nếu không có) và
        /// GỠ token khỏi content (kèm dấu ; thừa) để không lẫn vào ô văn bản.
        /// </summary>
        private int ExtractPlLevel(ref string content)
        {
            int level = 0;
            try
            {
                if (string.IsNullOrEmpty(content)) return 0;
                var m = System.Text.RegularExpressions.Regex.Match(content, @"PL\s*:\s*L\s*([1-5])",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    level = int.Parse(m.Groups[1].Value);
                    // Gỡ token + dấu ; hoặc xuống dòng bao quanh.
                    content = System.Text.RegularExpressions.Regex.Replace(content,
                        @"\s*;?\s*PL\s*:\s*L\s*[1-5]\s*;?\s*", ";",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    content = content.Trim().Trim(';', ' ', '\r', '\n').Trim();
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return level;
        }

        /// <summary>
        /// Điền ô Phân loại (rank) theo mức L1..L5 = MÃ (HEALTH_EXAM_RANK_CODE) thứ 1..5 của danh mục
        /// phân loại sức khỏe. Ưu tiên tìm rank có CODE == level; nếu không có, lấy rank thứ level theo thứ tự.
        /// </summary>
        private void SetClassifyByLevel(DevExpress.XtraEditors.GridLookUpEdit cbo, int level)
        {
            try
            {
                if (cbo == null || level < 1 || level > 5) return;
                if (cachedRankList == null)
                    cachedRankList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HEALTH_EXAM_RANK>()
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (cachedRankList == null || cachedRankList.Count == 0) return;

                var ordered = cachedRankList
                    .OrderBy(o => { long n; return long.TryParse((o.HEALTH_EXAM_RANK_CODE ?? "").Trim(), out n) ? n : long.MaxValue; })
                    .ThenBy(o => o.HEALTH_EXAM_RANK_CODE)
                    .ToList();

                // Ưu tiên: mã đúng bằng level ("1".."5").
                var target = ordered.FirstOrDefault(o => (o.HEALTH_EXAM_RANK_CODE ?? "").Trim() == level.ToString());
                // Fallback: rank thứ level theo thứ tự.
                if (target == null && ordered.Count >= level) target = ordered[level - 1];
                if (target != null) cbo.EditValue = target.ID;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Gắn tooltip NGẮN (Thư viện mẫu / Cú pháp mẫu / VD) cho 14 nút — set trực tiếp .ToolTip.</summary>
        private void InitTextLibExamTooltips()
        {
            try
            {
                // 11 vùng ô đơn: nội dung khám + token phân loại PL:Lx.
                string tipSingle = "Thư viện mẫu\r\nCú pháp mẫu: nội dung;PL:Lx  (x=1..5)\r\nVD: Bình thường;PL:L1";
                SetBtnToolTip(btnTextLibCirculation2, tipSingle);
                SetBtnToolTip(btnTextLibRespiratory2, tipSingle);
                SetBtnToolTip(btnTextLibDigestion2, tipSingle);
                SetBtnToolTip(btnTextLibKidneyUrology2, tipSingle);
                SetBtnToolTip(btnTextLibOend2, tipSingle);
                SetBtnToolTip(btnTextLibMuscleBone2, tipSingle);
                SetBtnToolTip(btnTextLibNeurological2, tipSingle);
                SetBtnToolTip(btnTextLibMental2, tipSingle);
                SetBtnToolTip(btnTextLibSurgery2, tipSingle);
                SetBtnToolTip(btnTextLibObstetric2, tipSingle);
                SetBtnToolTip(btnTextLibDermatology2, tipSingle);

                SetBtnToolTip(btnTextLibEye2,
                    "Thư viện mẫu\r\nCú pháp mẫu: TLP:..;TLT:..;TLPK:..;TLTK:..;BENH:..;PL:Lx\r\nVD: TLP:10/10;TLT:10/10;BENH:Bình thường;PL:L1");
                SetBtnToolTip(btnTextLibEnt2,
                    "Thư viện mẫu\r\nCú pháp mẫu: TP:..;TT:..;TPT:..;TTT:..;BENH:..;PL:Lx\r\nVD: TP:5/5;TT:5/5;BENH:Bình thường;PL:L1");
                SetBtnToolTip(btnTextLibStomatology2,
                    "Thư viện mẫu\r\nCú pháp mẫu: HT:..;HD:..;BENH:..;PL:Lx\r\nVD: HT:Bình thường;HD:Bình thường;PL:L1");

                // Nút Thư viện mẫu Nội khoa (JSON — điền cả 8 chuyên khoa nội).
                SetBtnToolTip(btnTextLibInternal2,
                    "Thư viện mẫu Nội khoa (JSON)\r\nMỗi chuyên khoa: \"KQ:kết quả;PL:Lx\" (x=1..5)\r\n"
                    + "VD: {\"tuanHoan\":\"KQ:Bình thường;PL:L1\",\"hoHap\":\"KQ:Bình thường;PL:L1\",\r\n"
                    + " \"tieuHoa\":\"KQ:...;PL:L1\",\"thanTietNieu\":\"KQ:...;PL:L1\",\"noiTiet\":\"KQ:...;PL:L1\",\r\n"
                    + " \"coXuongKhop\":\"KQ:...;PL:L1\",\"thanKinh\":\"KQ:...;PL:L1\",\"tamThan\":\"KQ:...;PL:L1\"}");
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void SetBtnToolTip(DevExpress.XtraEditors.SimpleButton b, string tip)
        {
            if (b != null) b.ToolTip = tip;
        }

        /// <summary>Mở plugin Thư viện văn bản (HIS.Desktop.Plugins.TextLibrary) — giống OpenModuleTextLibrary.</summary>
        private void OpenModuleTextLibrary(string content, string hashtag)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws
                    .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TextLibrary").FirstOrDefault();
                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TextLibrary");
                    return;
                }
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS.Desktop.ADO.TextLibraryInfoADO ado = new HIS.Desktop.ADO.TextLibraryInfoADO();
                    ado.Content = content;
                    ado.Hashtag = hashtag;
                    listArgs.Add(ado);
                    listArgs.Add((HIS.Desktop.Common.DelegateDataTextLib)ProcessDataTextLib);

                    // ĐO THỜI GIAN (tạm) mở Thư viện mẫu: tách "tạo form" (GetPluginInstance) và "load+hiển thị"
                    // (tới sự kiện Shown — chạy trong ShowDialog). Đọc log "TextLibOpen.*" để biết phần nào nặng.
                    var swTL = System.Diagnostics.Stopwatch.StartNew();
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(
                        HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    long tConstruct = swTL.ElapsedMilliseconds;
                    Inventec.Common.Logging.LogSystem.Debug("TextLibOpen.construct(GetPluginInstance): " + tConstruct + " ms, hashtag=" + hashtag);
                    var tlForm = extenceInstance as System.Windows.Forms.Form;
                    if (tlForm != null)
                    {
                        tlForm.Shown += (s, ev) => Inventec.Common.Logging.LogSystem.Debug(
                            "TextLibOpen.loadAndShow(Shown): " + (swTL.ElapsedMilliseconds - tConstruct)
                            + " ms | TOTAL(construct+load+show): " + swTL.ElapsedMilliseconds + " ms, hashtag=" + hashtag);
                        tlForm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Callback nhận văn bản đã chọn từ Thư viện văn bản, đổ về ô theo key (giống ProcessDataTextLib).</summary>
        private void ProcessDataTextLib(MOS.EFMODEL.DataModels.HIS_TEXT_LIB textLib)
        {
            try
            {
                if (textLib == null) return;
                switch (keyTextLib)
                {
                    case 1:
                        this.txtLyDoKham.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 2:
                        {
                            string content2 = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                            int lv2 = ExtractPlLevel(ref content2);            // tách "PL:Lx" -> tự điền phân loại
                            if (lv2 > 0) SetClassifyByLevel(textLibTargetClassify, lv2);
                            if (textLibTargetEdit != null) textLibTargetEdit.Text = content2;
                        }
                        break;
                    case 3:
                        {
                            // Phần Mắt: tách "PL:Lx" (tự điền phân loại) rồi cắt "ô:giá trị;..." điền nhiều ô.
                            string content3 = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                            int lv3 = ExtractPlLevel(ref content3);
                            if (lv3 > 0) SetClassifyByLevel(textLibTargetClassify, lv3);
                            FillEyeFieldsFromLibText(content3);
                        }
                        break;
                    case 4:
                        {
                            // Tai mũi họng.
                            string content4 = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                            int lv4 = ExtractPlLevel(ref content4);
                            if (lv4 > 0) SetClassifyByLevel(textLibTargetClassify, lv4);
                            FillEntFieldsFromLibText(content4);
                        }
                        break;
                    case 5:
                        {
                            // Răng hàm mặt.
                            string content5 = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                            int lv5 = ExtractPlLevel(ref content5);
                            if (lv5 > 0) SetClassifyByLevel(textLibTargetClassify, lv5);
                            FillStomatologyFieldsFromLibText(content5);
                        }
                        break;
                    case 6:
                        // Nội khoa (8 chuyên khoa): mẫu là JSON -> parse điền kết quả + phân loại từng vùng.
                        FillInternalFromJson(HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        // Danh sách Đối tượng đã tick (tương tự executeRoomSelecteds).
        private List<KskCodeNameADO> objectSelecteds = new List<KskCodeNameADO>();

        /// <summary>Gắn GridCheckMarksSelection + event (y hệt InitComboExecuteRoomCheck).</summary>
        private void InitObjectCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboObject.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_CheckObject);
                cboObject.Properties.Tag = gridCheck;
                cboObject.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboObject.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboObject.Properties.View);
                }
                cboObject.CustomDisplayText -= cboObject_CustomDisplayText;
                cboObject.CustomDisplayText += cboObject_CustomDisplayText;
                // Tự thêm nút Xóa (generic InitClearButtonForGridLookUpEdits đã bỏ qua combo multi-select
                // để tránh handler EditValueChanged toggle nút -> đóng popup khi tick).
                bool hasDelete = false;
                foreach (EditorButton btn in cboObject.Properties.Buttons)
                    if (btn.Kind == ButtonPredefines.Delete) { hasDelete = true; break; }
                if (!hasDelete)
                {
                    EditorButton del = new EditorButton(ButtonPredefines.Delete);
                    del.ToolTip = "Xóa giá trị đang chọn";
                    cboObject.Properties.Buttons.Add(del);
                }
                cboObject.ButtonClick -= cboObject_ClearMultiButtonClick;
                cboObject.ButtonClick += cboObject_ClearMultiButtonClick;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Gán DataSource + cột Mã/Tên + MultiSelect (y hệt InitComboExecuteRoom).</summary>
        private void InitObjectCombo()
        {
            try
            {
                cboObject.Properties.DataSource = BuildKskObjectList();
                cboObject.Properties.DisplayMember = "NAME";
                cboObject.Properties.ValueMember = "ID";
                cboObject.Properties.NullText = "";

                DevExpress.XtraGrid.Columns.GridColumn colId = cboObject.Properties.View.Columns.AddField("ID");
                colId.VisibleIndex = 1; colId.Width = 45; colId.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn colName = cboObject.Properties.View.Columns.AddField("NAME");
                colName.VisibleIndex = 2; colName.Width = 360; colName.Caption = "Tên";
                cboObject.Properties.PopupFormWidth = 430;
                cboObject.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboObject.Properties.View.OptionsSelection.MultiSelect = true;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Combo Nguồn chi trả: chọn 1, hiển thị cột Mã + Tên.</summary>
        private void InitPaymentSourceCombo()
        {
            try
            {
                cboPaymentSource.Properties.DataSource = BuildKskPaymentSourceList();
                cboPaymentSource.Properties.DisplayMember = "NAME";
                cboPaymentSource.Properties.ValueMember = "ID";
                cboPaymentSource.Properties.NullText = "";

                DevExpress.XtraGrid.Columns.GridColumn colId = cboPaymentSource.Properties.View.Columns.AddField("ID");
                colId.VisibleIndex = 1; colId.Width = 45; colId.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn colName = cboPaymentSource.Properties.View.Columns.AddField("NAME");
                colName.VisibleIndex = 2; colName.Width = 360; colName.Caption = "Tên";
                cboPaymentSource.Properties.PopupFormWidth = 430;
                cboPaymentSource.Properties.View.OptionsView.ShowColumnHeaders = true;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// SelectionChanged: cập nhật danh sách đã tick + làm mới hiển thị NGAY (tick tới đâu hiện tới đó).
        /// KHÔNG gán cboObject.Text (gán .Text khi popup mở sẽ commit -> đóng popup do combo ở trong LayoutControl).
        /// RefreshEditValue() buộc editor gọi lại CustomDisplayText -> cập nhật ô mà KHÔNG đóng popup.
        /// </summary>
        private void Event_CheckObject(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                objectSelecteds = new List<KskCodeNameADO>();
                if (gridCheckMark != null)
                {
                    List<KskCodeNameADO> selectedNews = new List<KskCodeNameADO>();
                    foreach (KskCodeNameADO er in gridCheckMark.Selection)
                    {
                        if (er != null)
                        {
                            if (sb.Length > 0) { sb.Append(", "); }
                            sb.Append(er.NAME);
                            selectedNews.Add(er);
                        }
                    }
                    this.objectSelecteds = new List<KskCodeNameADO>();
                    this.objectSelecteds.AddRange(selectedNews);
                }
                // Tick tới đâu hiển thị tới đó (y hệt cboEXECUTE_ROOM_NAME). An toàn vì combo multi-select đã
                // được loại khỏi generic clear-button (không còn EditValueChanged toggle nút gây đóng popup).
                this.cboObject.Text = sb.ToString();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>CustomDisplayText (y hệt cboEXECUTE_ROOM_NAME_CustomDisplayText): hiển thị từ danh sách đã tick.</summary>
        private void cboObject_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string name = "";
                if (this.objectSelecteds != null && this.objectSelecteds.Count > 0)
                {
                    foreach (var item in this.objectSelecteds)
                    {
                        name += item.NAME + "; ";
                    }
                }
                e.DisplayText = name;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Nút Xóa ở combo Đối tượng (chọn nhiều): bỏ hết tick + xóa nội dung.</summary>
        private void cboObject_ClearMultiButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e == null || e.Button == null || e.Button.Kind != ButtonPredefines.Delete) return;
                GridCheckMarksSelection gridCheck = cboObject.Properties.Tag as GridCheckMarksSelection;
                if (gridCheck != null) gridCheck.ClearSelection(cboObject.Properties.View);
                objectSelecteds = new List<KskCodeNameADO>();
                cboObject.EditValue = null;
                cboObject.Text = string.Empty;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Chuỗi mã Đối tượng đã chọn (join ";") để lưu vào DOI_TUONG.</summary>
        private string GetKskObjectValue()
        {
            return objectSelecteds == null ? "" : string.Join(";", objectSelecteds.Select(o => o.ID.ToString()).ToArray());
        }

        /// <summary>
        /// Đổ chuỗi mã Đối tượng đã lưu ("1;3;13" — KSK_PATIENT_TYPES) vào combo: tick lại checkbox
        /// (GridCheckMarksSelection.Selection theo dòng DataSource) + hiển thị tên qua Event_CheckObject.
        /// </summary>
        private void SetKskObjectValue(string codes)
        {
            try
            {
                GridCheckMarksSelection gridCheck = cboObject.Properties.Tag as GridCheckMarksSelection;
                if (gridCheck == null) return;
                gridCheck.ClearSelection(cboObject.Properties.View);
                objectSelecteds = new List<KskCodeNameADO>();
                if (!string.IsNullOrEmpty(codes))
                {
                    var ds = cboObject.Properties.DataSource as List<KskCodeNameADO>;
                    if (ds != null)
                    {
                        foreach (string c in codes.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var row = ds.FirstOrDefault(o => o.ID.ToString() == c.Trim());
                            if (row != null && !gridCheck.Selection.Contains(row))
                                gridCheck.Selection.Add(row);
                        }
                    }
                }
                gridCheck.OnSelectionChanged();   // đồng bộ objectSelecteds + Text (Event_CheckObject)
                if (string.IsNullOrEmpty(codes))
                {
                    cboObject.EditValue = null;
                    cboObject.Text = string.Empty;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private static List<KskCodeNameADO> BuildKskObjectList()
        {
            return new List<KskCodeNameADO>
            {
                new KskCodeNameADO(1, "Người cao tuổi"),
                new KskCodeNameADO(2, "Người khuyết tật"),
                new KskCodeNameADO(3, "Người thuộc hộ nghèo, cận nghèo"),
                new KskCodeNameADO(4, "Người có công"),
                new KskCodeNameADO(5, "Người mắc bệnh mạn tính"),
                new KskCodeNameADO(6, "Người sống tại vùng đồng bào dân tộc thiểu số và miền núi"),
                new KskCodeNameADO(7, "Người sống tại vùng có điều kiện kinh tế - xã hội khó khăn, đặc biệt khó khăn"),
                new KskCodeNameADO(8, "Người sống tại xã đảo"),
                new KskCodeNameADO(9, "Người sống tại đặc khu"),
                new KskCodeNameADO(10, "Trẻ em trong cơ sở giáo dục mầm non"),
                new KskCodeNameADO(11, "Học sinh trong các cơ sở giáo dục phổ thông"),
                new KskCodeNameADO(12, "Sinh viên"),
                new KskCodeNameADO(13, "Người lao động"),
                new KskCodeNameADO(14, "Người lao động không chính thức"),
                new KskCodeNameADO(15, "Người chưa có Bảo hiểm y tế"),
                new KskCodeNameADO(16, "Các đối tượng khác")
            };
        }

        private static List<KskCodeNameADO> BuildKskPaymentSourceList()
        {
            return new List<KskCodeNameADO>
            {
                new KskCodeNameADO(1, "Ngân sách Trung ương"),
                new KskCodeNameADO(2, "Ngân sách Địa phương"),
                new KskCodeNameADO(3, "Quỹ Bảo hiểm y tế"),
                new KskCodeNameADO(4, "Người sử dụng lao động"),
                new KskCodeNameADO(5, "Xã hội hóa"),
                new KskCodeNameADO(9, "Khác")
            };
        }

        #endregion

        #region Chon ket qua kham lam sang -> Benh tat (btnChooseRs)
        /// <summary>
        /// btnChooseRs: mo form liet ke cac vung kham lam sang (>=18) CO ket qua/benh khac;
        /// tich chon -> dien noi dung vao o "Bệnh tật" (txtDiseases2).
        /// </summary>
        private void btnChooseRs_Click(object sender, EventArgs e)
        {
            try
            {
                var list = new List<KskExamResultADO>();
                AddExamResultRow(list, "Tuần hoàn", txtExamCirculation2);
                AddExamResultRow(list, "Hô hấp", txtExamRespiratory2);
                AddExamResultRow(list, "Tiêu hóa", txtExamDigestion2);
                AddExamResultRow(list, "Thận - Tiết niệu", txtExamKidneyUrology2);
                AddExamResultRow(list, "Nội tiết", txtExamOend2);
                AddExamResultRow(list, "Cơ - Xương - Khớp", txtExamMuscleBone2);
                AddExamResultRow(list, "Thần kinh", txtExamNeurological2);
                AddExamResultRow(list, "Tâm thần", txtExamMental2);
                AddExamResultRow(list, "Ngoại khoa", txtExamSurgery2);
                AddExamResultRow(list, "Da liễu", txtExamDernatology2);
                AddExamResultRow(list, "Sản phụ khoa", txtExamObstetric2);
                AddExamResultRow(list, "Mắt (bệnh khác)", txtExamEyeDisease2);
                AddExamResultRow(list, "Tai mũi họng (bệnh khác)", txtExamEntDisease2);
                AddExamResultRow(list, "Răng hàm mặt (bệnh khác)", txtExamStomatologyDisease2);

                if (list.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Chưa có nội dung kết quả / bệnh khác nào ở tab Khám lâm sàng để chọn.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var frm = new frmChooseExamResult(list))
                {
                    if (frm.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(frm.SelectedText)
                        && txtDiseases2 != null)
                    {
                        txtDiseases2.Text = frm.SelectedText;
                    }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Them 1 dong vao danh sach neu control co noi dung (ket qua / benh khac).</summary>
        private static void AddExamResultRow(List<KskExamResultADO> list, string ten, DevExpress.XtraEditors.BaseEdit ctrl)
        {
            if (ctrl == null) return;
            string kq = (ctrl.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(kq))
                list.Add(new KskExamResultADO { Ten = ten, KetQua = kq, Chon = false });
        }
        #endregion
    }

    /// <summary>Mục danh mục Mã/Tên cho combo Đối tượng, Nguồn chi trả (QĐ 1551).</summary>
    public class KskCodeNameADO
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public KskCodeNameADO() { }
        public KskCodeNameADO(int id, string name) { this.ID = id; this.NAME = name; }
    }
}
