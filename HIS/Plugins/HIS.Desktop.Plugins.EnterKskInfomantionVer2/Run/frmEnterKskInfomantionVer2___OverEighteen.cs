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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using HIS.Desktop.LocalStorage.BackendData;
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
                SetDataCboExamLoginName(cboExecuteLoginName2);
                SetDataCboExamLoginName(cboExamEyeLoginName2);
                SetDataCboExamLoginName(cboExamEntLoginName2);
                SetDataCboExamLoginName(cboExamCirculationLoginName2);
                SetDataCboExamLoginName(cboExamStomatologyLoginName2);
                SetDataCboExamLoginName(cboDiimLoginName2);
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
                var dataVacine = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DISEASE_TYPE>>("api/HisDiseaseType/Get", ApiConsumers.MosConsumer, Disfilter, param);
                if (dataVacine != null && dataVacine.Count > 0)
                {
                    List<ADO.DiseaseTypeADO> lstAdo = new List<ADO.DiseaseTypeADO>();
                    foreach (var item in dataVacine)
                    {
                        ADO.DiseaseTypeADO ado = new ADO.DiseaseTypeADO();
                        ado.ID = item.ID;
                        ado.DISEASE_TYPE_NAME = item.DISEASE_TYPE_NAME;
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
                    var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_OVER_EIGHTEEN>>("api/HisKskOverEighteen/Get", ApiConsumers.MosConsumer, filter, param);
                    if (data != null && data.Count > 0)
                    {
                        currentKskOverEight = data.First();
                        txtPathologicalHistoryFamily.Text = currentKskOverEight.PATHOLOGICAL_HISTORY_FAMILY;
                        txtPathologicalHistory2.Text = currentKskOverEight.PATHOLOGICAL_HISTORY;
                        txtMedicineUsing.Text = currentKskOverEight.MEDICINE_USING;
                        txtMaternityHistory.Text = currentKskOverEight.MATERNITY_HISTORY;
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
                        txtTestBloodGluco2.Text = currentKskOverEight.TEST_BLOOD_GLUCO;
                        txtTestBloodUre2.Text = currentKskOverEight.TEST_BLOOD_URE;
                        txtTestBloodCreatinin2.Text = currentKskOverEight.TEST_BLOOD_CREATININ;
                        txtTestBloodAsat2.Text = currentKskOverEight.TEST_BLOOD_ASAT;
                        txtTestBloodAlat2.Text = currentKskOverEight.TEST_BLOOD_ALAT;
                        txtTestBloodOther2.Text = currentKskOverEight.TEST_BLOOD_OTHER;
                        txtTestUrineGluco2.Text = currentKskOverEight.TEST_URINE_GLUCO;
                        txtTestUrineProtein2.Text = currentKskOverEight.TEST_URINE_PROTEIN;
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
                            var dataDhst = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param);
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
                        cboDiimLoginName2.EditValue = currentKskOverEight.DIIM_LOGINNAME;
                        cboExamStomatologyLoginName2.EditValue = currentKskOverEight.EXAM_STOMATOLOGY_LOGINNAME;

                        HisPeriodDriverDityFilter dityFilter = new HisPeriodDriverDityFilter();
                        dityFilter.KSK_OVER_EIGHTEEN_ID = currentKskOverEight.ID;
                        lstDataDriverDityOverE = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PERIOD_DRIVER_DITY>>("api/HisPeriodDriverDity/Get", ApiConsumers.MosConsumer, dityFilter, param);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstDataDriverDityOverE), lstDataDriverDityOverE));
                        if (lstDataDriverDityOverE != null && lstDataDriverDityOverE.Count > 0)
                        {
                            HisDiseaseTypeFilter Disfilter = new HisDiseaseTypeFilter();
                            Disfilter.IS_ACTIVE = 1;
                            Disfilter.IDs = lstDataDriverDityOverE.Select(o => o.DISEASE_TYPE_ID).ToList();
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
                        if (currentServiceReq.DHST_ID != null && currentServiceReq.DHST_ID > 0)
                        {
                            HisDhstFilter dhstFilter = new HisDhstFilter();
                            dhstFilter.ID = currentServiceReq.DHST_ID;
                            var dataDhst = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param);
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
                obj.TEST_BLOOD_HC = txtTestBloodHc2.Text;
                obj.TEST_BLOOD_BC = txtTestBloodTc2.Text;
                obj.TEST_BLOOD_TC = txtTestBloodBc2.Text;
                obj.TEST_BLOOD_GLUCO = txtTestBloodGluco2.Text;
                obj.TEST_BLOOD_URE = txtTestBloodUre2.Text;
                obj.TEST_BLOOD_CREATININ = txtTestBloodCreatinin2.Text;
                obj.TEST_BLOOD_ASAT = txtTestBloodAsat2.Text;
                obj.TEST_BLOOD_ALAT = txtTestBloodAlat2.Text;
                obj.TEST_BLOOD_OTHER = txtTestBloodOther2.Text;
                obj.TEST_URINE_GLUCO = txtTestUrineGluco2.Text;
                obj.TEST_URINE_PROTEIN = txtTestUrineProtein2.Text;
                obj.TEST_URINE_OTHER = txtTestUrineOther2.Text;
                obj.RESULT_DIIM = txtResultDiim2.Text;
                obj.HEALTH_EXAM_RANK_ID = cboHealthExamRank2.EditValue != null ? (long?)Int64.Parse(cboHealthExamRank2.EditValue.ToString()) : null;
                obj.DISEASES = txtDiseases2.Text;
                obj.HEALTH_EXAM_RANK_DESCRIPTION = txtHealthExamRankDescription2.Text.Trim();
                obj.EXAM_OEND = txtExamOend2.Text.Trim();
                obj.EXAM_OEND_RANK = cboExamOend2.EditValue != null ? (long?)Int64.Parse(cboExamOend2.EditValue.ToString()) : null;

                obj.EXAM_CIRCULATION_LOGINNAME = cboExamCirculationLoginName2.EditValue != null ? cboExamCirculationLoginName2.EditValue.ToString() : null;
                obj.EXAM_EYE_LOGINNAME = cboExamEyeLoginName2.EditValue != null ? cboExamEyeLoginName2.EditValue.ToString() : null;
                obj.EXAM_ENT_LOGINNAME = cboExamEntLoginName2.EditValue != null ? cboExamEntLoginName2.EditValue.ToString() : null;
                obj.EXAM_STOMATOLOGY_LOGINNAME = cboExamStomatologyLoginName2.EditValue != null ? cboExamStomatologyLoginName2.EditValue.ToString() : null;
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
                obj.EXECUTE_USERNAME = obj.EXECUTE_LOGINNAME != null ? BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == obj.EXECUTE_LOGINNAME).TDL_USERNAME : null;
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
    }
}
