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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private HIS_DHST dhstOccupational { get; set; }

        private void ResetControlOccupational()
        {
            try
            {
                spnRecentWorkOneYear7.EditValue = null;
                spnRecentWorkOneMonth7.EditValue = null;
                spnRecentElementOneYear.EditValue = null;
                spnRecentElementOneMonth.EditValue = null;
                spnRecentWorkOneYear8.EditValue = null;
                spnRecentWorkOneMonth8.EditValue = null;
                spnRecentElementOneYear2.EditValue = null;
                spnRecentElementOneMonth2.EditValue = null;
                spnHeight7.EditValue = null;
                spnPulse7.EditValue = null;
                spnWeight7.EditValue = null;
                spnBloodPressureMax7.EditValue = null;
                spnBloodPressureMin7.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataPageOccupational()
        {
            try
            {
                ResetControlOccupational();
                SetDataCboRank(cboDhstRank7);
                SetDataCboRank(cboExamCirculationRank7);
                SetDataCboRank(cboExamRespiratoryRank7);
                SetDataCboRank(cboExamDigestionRank7);
                SetDataCboRank(cboExamKidneyUrologyRank7);
                SetDataCboRank(cboExamOendRank7);
                SetDataCboRank(cboExamMuscleBoneRank7);
                SetDataCboRank(cboExamNeurologicalRank7);
                SetDataCboRank(cboExamMentalRank7);
                SetDataCboRank(cboExamSurgeryRank7);
                SetDataCboRank(cboExamObstetricRank7);
                SetDataCboRank(cboExamEyeRank7);
                SetDataCboRank(cboExamEntDiseaseRank7);
                SetDataCboRank(cboExamStomatologyRank7);
                SetDataCboRank(cboExamDernatologyRank7);
                SetDataCboRank(cboHealthExamRank7);
                SetDataCboExamLoginName(cboExecuteLoginName7);
                SetDataCboExamLoginName(cboExamCirculationLoginName7);
                SetDataCboExamLoginName(cboExamEyeLoginName7);
                SetDataCboExamLoginName(cboExamEntLoginName7);
                SetDataCboExamLoginName(cboExamStomatologyLoginname7);
                SetDataCboExamLoginName(cboExamSubclinicalLoginName7);
                SetDataCboExamLoginName(cboConcluderLoginName7);
                FillDataOccupational();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void FillDataOccupational()
        {
            try
            {
                if (currentServiceReq != null)
                {
                    CommonParam param = new CommonParam();
                    HisKskOccupationalFilter filter = new HisKskOccupationalFilter();
                    filter.SERVICE_REQ_ID = currentServiceReq.ID;
                    var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_OCCUPATIONAL>>("api/HisKskOccupational/Get", ApiConsumers.MosConsumer, filter, param);
                    if (data != null && data.Count > 0)
                    {
                        currentKsKOccupational = data.First();
                        Inventec.Common.Logging.LogSystem.Debug("INPUT DATA:__api/HisServiceReq/KskExecuteV2 " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentKsKOccupational), currentKsKOccupational));
                        txtRecentWork.Text = currentKsKOccupational.RECENT_WORK_ONE;
                        txtRecentWork2.Text = currentKsKOccupational.RECENT_WORK_TWO;
                        txtRecentElement.Text = currentKsKOccupational.RECENT_ELEMENT_ONE;
                        txtRecentElement2.Text = currentKsKOccupational.RECENT_ELEMENT_TWO;
                        txtPathologicalFamily.Text = currentKsKOccupational.PATHOLOGICAL_HISTORY_FAMILY;

                        spnRecentWorkOneYear7.EditValue = currentKsKOccupational.RECENT_WORK_ONE_YEAR ?? null;
                        spnRecentWorkOneMonth7.EditValue = currentKsKOccupational.RECENT_WORK_ONE_MONTH ?? null;
                        spnRecentElementOneYear.EditValue = currentKsKOccupational.RECENT_ELEMENT_ONE_YEAR ?? null;
                        spnRecentElementOneMonth.EditValue = currentKsKOccupational.RECENT_ELEMENT_ONE_MONTH ?? null;

                        spnRecentWorkOneYear8.EditValue = currentKsKOccupational.RECENT_WORK_TWO_YEAR ?? null;
                        spnRecentWorkOneMonth8.EditValue = currentKsKOccupational.RECENT_WORK_TWO_MONTH ?? null;
                        spnRecentElementOneYear2.EditValue = currentKsKOccupational.RECENT_ELEMENT_TWO_YEAR ?? null;
                        spnRecentElementOneMonth2.EditValue = currentKsKOccupational.RECENT_ELEMENT_TWO_MONTH ?? null;

                        if (currentKsKOccupational.RECENT_WORK_ONE_FROM != null && currentKsKOccupational.RECENT_WORK_ONE_FROM > 0)
                        {
                            dteRecentWorkOneFrom7.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentKsKOccupational.RECENT_WORK_ONE_FROM ?? 0) ?? DateTime.Now;
                        }
                        if (currentKsKOccupational.RECENT_WORK_ONE_TO != null && currentKsKOccupational.RECENT_WORK_ONE_TO > 0)
                        {
                            dteRecentWorkOneTo7.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentKsKOccupational.RECENT_WORK_ONE_TO ?? 0) ?? DateTime.Now;
                        }

                        if (currentKsKOccupational.RECENT_WORK_TWO_FROM != null && currentKsKOccupational.RECENT_WORK_TWO_FROM > 0)
                        {
                            dteRecentWorkTwoFrom8.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentKsKOccupational.RECENT_WORK_TWO_FROM ?? 0) ?? DateTime.Now;
                        }
                        if (currentKsKOccupational.RECENT_WORK_TWO_TO != null && currentKsKOccupational.RECENT_WORK_TWO_TO > 0)
                        {
                            dteRecentWorkTwoTo8.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentKsKOccupational.RECENT_WORK_TWO_TO ?? 0) ?? DateTime.Now;
                        }

                        if (this.currentKsKOccupational.NOW_WORK_FROM != null && this.currentKsKOccupational.NOW_WORK_FROM > 0)
                        {
                            this.dtpStart7.DateTime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentKsKOccupational.NOW_WORK_FROM ?? 0) ?? DateTime.Now);
                        }
                        else
                        {
                            dtpStart7.EditValue = null;
                        }

                        txtPathologicalHistory7.Text = currentKsKOccupational.PATHOLOGICAL_HISTORY;
                        cboDhstRank7.EditValue = currentKsKOccupational.DHST_RANK;
                        txtExamCirculation7.Text = currentKsKOccupational.EXAM_CIRCULATION;
                        cboExamCirculationRank7.EditValue = currentKsKOccupational.EXAM_CIRCULATION_RANK;
                        txtExamRespiratory7.Text = currentKsKOccupational.EXAM_RESPIRATORY;
                        cboExamRespiratoryRank7.EditValue = currentKsKOccupational.EXAM_RESPIRATORY_RANK;
                        txtExamDigestion7.Text = currentKsKOccupational.EXAM_DIGESTION;
                        cboExamDigestionRank7.EditValue = currentKsKOccupational.EXAM_DIGESTION_RANK;
                        txtExamKidneyUrology7.Text = currentKsKOccupational.EXAM_KIDNEY_UROLOGY;
                        cboExamKidneyUrologyRank7.EditValue = currentKsKOccupational.EXAM_KIDNEY_UROLOGY_RANK;
                        txtExamOend7.Text = currentKsKOccupational.EXAM_OEND;
                        cboExamOendRank7.EditValue = currentKsKOccupational.EXAM_OEND_RANK;
                        cboExamOendRank7.EditValue = currentKsKOccupational.EXAM_NEUROLOGICAL_RANK;
                        txtExamMuscleBone7.Text = currentKsKOccupational.EXAM_MUSCLE_BONE;
                        cboExamMuscleBoneRank7.EditValue = currentKsKOccupational.EXAM_MUSCLE_BONE_RANK;
                        txtExamNeurological7.Text = currentKsKOccupational.EXAM_NEUROLOGICAL;
                        cboExamNeurologicalRank7.EditValue = currentKsKOccupational.EXAM_NEUROLOGICAL_RANK;
                        txtExamMental7.Text = currentKsKOccupational.EXAM_MENTAL;
                        cboExamMentalRank7.EditValue = currentKsKOccupational.EXAM_MENTAL_RANK;
                        txtExamSurgery7.Text = currentKsKOccupational.EXAM_SURGERY;
                        cboExamSurgeryRank7.EditValue = currentKsKOccupational.EXAM_SURGERY_RANK;
                        txtExamDernatology7.Text = currentKsKOccupational.EXAM_DERMATOLOGY;
                        cboExamDernatologyRank7.EditValue = currentKsKOccupational.EXAM_DERMATOLOGY_RANK;
                        txtExamObstetric7.Text = currentKsKOccupational.EXAM_OBSTETRIC;
                        cboExamObstetricRank7.EditValue = currentKsKOccupational.EXAM_OBSTETRIC_RANK;

                        txtExamEyeSightRight7.Text = currentKsKOccupational.EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft7.Text = currentKsKOccupational.EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight7.Text = currentKsKOccupational.EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft7.Text = currentKsKOccupational.EXAM_EYESIGHT_GLASS_LEFT;
                        txtExamEyeDisease7.Text = currentKsKOccupational.EXAM_EYE_DISEASE;
                        cboExamEyeRank7.EditValue = currentKsKOccupational.EXAM_EYE_RANK;
                        txtExamEntLeftNormal7.Text = currentKsKOccupational.EXAM_ENT_LEFT_NORMAL;
                        txtExamEntLeftWhisper7.Text = currentKsKOccupational.EXAM_ENT_LEFT_WHISPER;
                        txtExamEntRightNomal7.Text = currentKsKOccupational.EXAM_ENT_RIGHT_NORMAL;
                        txtExamEntRightWhisper7.Text = currentKsKOccupational.EXAM_ENT_RIGHT_WHISPER;
                        txtExamEntDisease7.Text = currentKsKOccupational.EXAM_ENT_DISEASE;
                        cboExamEntDiseaseRank7.EditValue = currentKsKOccupational.EXAM_ENT_RANK;
                        txtExamStomatologyUpper7.Text = currentKsKOccupational.EXAM_STOMATOLOGY_UPPER;
                        txtExamStomatologyLower7.Text = currentKsKOccupational.EXAM_STOMATOLOGY_LOWER;
                        txtExamStomatologyDisease7.Text = currentKsKOccupational.EXAM_STOMATOLOGY_DISEASE;
                        cboExamStomatologyRank7.EditValue = currentKsKOccupational.EXAM_STOMATOLOGY_RANK;

                        txtResultSubclinical7.Text = currentKsKOccupational.RESULT_SUBCLINICAL;
                        txtNoteExamDernatology7.Text = currentKsKOccupational.NOTE_CLINICAL;
                        txtNoteSubclinical7.Text = currentKsKOccupational.NOTE_SUBCLINICAL;
                        cboHealthExamRank7.EditValue = currentKsKOccupational.HEALTH_EXAM_RANK_ID;
                        txtDiseases7.Text = currentKsKOccupational.DISEASES;
                        txtPreliminaryDiagnosis.Text = currentKsKOccupational.PROVISIONAL_DIAGNOSIS;
                        txtDefiniteDiagnosis.Text = currentKsKOccupational.DIAGNOSIS;
                        txtResultConsultation.Text = currentKsKOccupational.CONCLUSION;
                        txtSolution.Text = currentKsKOccupational.TREATMENT_INSTRUCTION;


                        if (currentKsKOccupational.DHST_ID != null && currentKsKOccupational.DHST_ID > 0)
                        {
                            HisDhstFilter dhstFilter = new HisDhstFilter();
                            dhstFilter.ID = currentKsKOccupational.DHST_ID;
                            var dataDhst = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                            if (dataDhst != null && dataDhst.Count > 0)
                            {
                                dhstOccupational = dataDhst.First();
                                spnHeight7.EditValue = dhstOccupational.HEIGHT;
                                spnPulse7.EditValue = dhstOccupational.PULSE;
                                spnWeight7.EditValue = dhstOccupational.WEIGHT;
                                spnBloodPressureMax7.EditValue = dhstOccupational.BLOOD_PRESSURE_MAX;
                                spnBloodPressureMin7.EditValue = dhstOccupational.BLOOD_PRESSURE_MIN;
                                //txtVirBmi.Text = currentDhst.VIR_BMI!=null ? currentDhst.VIR_BMI.ToString() : "";
                                FillNoteBMI(spnHeight7, spnWeight7, txtVirBmi);
                                cboExecuteLoginName7.EditValue = dhstOccupational.EXECUTE_LOGINNAME;
                            }
                        }
                        cboExamStomatologyLoginname7.EditValue = currentKsKOccupational.EXAM_STOMATOLOGY_LOGINNAME;
                        cboExamCirculationLoginName7.EditValue = currentKsKOccupational.EXAM_CIRCULATION_LOGINNAME;
                        cboExamEyeLoginName7.EditValue = currentKsKOccupational.EXAM_EYE_LOGINNAME;
                        cboExamEntLoginName7.EditValue = currentKsKOccupational.EXAM_ENT_LOGINNAME;
                        cboExamSubclinicalLoginName7.EditValue = currentKsKOccupational.EXAM_SUBCLINICAL_LOGINNAME;
                        cboConcluderLoginName7.EditValue = currentKsKOccupational.CONCLUDER_LOGINNAME;
                        
                       


                    }
                    else
                    {
                        txtPathologicalHistory7.Text = currentServiceReq.PATHOLOGICAL_HISTORY;
                        txtExamCirculation7.Text = currentServiceReq.PART_EXAM_CIRCULATION;
                        txtExamRespiratory7.Text = currentServiceReq.PART_EXAM_RESPIRATORY;
                        txtExamDigestion7.Text = currentServiceReq.PART_EXAM_DIGESTION;
                        txtExamKidneyUrology7.Text = currentServiceReq.PART_EXAM_KIDNEY_UROLOGY;
                        txtExamNeurological7.Text = currentServiceReq.PART_EXAM_OEND;
                        txtExamMuscleBone7.Text = currentServiceReq.PART_EXAM_MUSCLE_BONE;
                        txtExamOend7.Text = currentServiceReq.PART_EXAM_NEUROLOGICAL;
                        txtExamMental7.Text = currentServiceReq.PART_EXAM_MENTAL;
                        txtExamObstetric7.Text = currentServiceReq.PART_EXAM_OBSTETRIC;

                        txtExamEyeSightRight7.Text = currentServiceReq.PART_EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft7.Text = currentServiceReq.PART_EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight7.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft7.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_LEFT;

                        txtExamEntLeftNormal7.Text = currentServiceReq.PART_EXAM_EAR_LEFT_NORMAL;
                        txtExamEntLeftWhisper7.Text = currentServiceReq.PART_EXAM_EAR_LEFT_WHISPER;
                        txtExamEntRightNomal7.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_NORMAL;
                        txtExamEntRightWhisper7.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_WHISPER;

                        txtExamStomatologyUpper7.Text = currentServiceReq.PART_EXAM_UPPER_JAW;
                        txtExamStomatologyLower7.Text = currentServiceReq.PART_EXAM_LOWER_JAW;
                        txtExamDernatology7.Text = currentServiceReq.PART_EXAM_DERMATOLOGY;
                        txtExamSurgery7.Text = currentServiceReq.SUBCLINICAL;
                        if (currentServiceReq.DHST_ID != null && currentServiceReq.DHST_ID > 0)
                        {
                            HisDhstFilter dhstFilter = new HisDhstFilter();
                            dhstFilter.ID = currentServiceReq.DHST_ID;
                            var dataDhst = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                            if (dataDhst != null && dataDhst.Count > 0)
                            {
                                var currentDhst = dataDhst.First();
                                spnHeight7.EditValue = currentDhst.HEIGHT;
                                spnPulse7.EditValue = currentDhst.PULSE;
                                spnWeight7.EditValue = currentDhst.WEIGHT;
                                spnBloodPressureMax7.EditValue = currentDhst.BLOOD_PRESSURE_MAX;
                                spnBloodPressureMin7.EditValue = currentDhst.BLOOD_PRESSURE_MIN;
                                //txtVirBmi.Text = currentDhst.VIR_BMI!=null ? currentDhst.VIR_BMI.ToString() : "";
                                FillNoteBMI(spnHeight7, spnWeight7, txtVirBmi7);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnHeight7_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FillNoteBMI(spnHeight7, spnWeight7, txtVirBmi7);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void spnWeight7_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FillNoteBMI(spnHeight7, spnWeight7, txtVirBmi7);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_KSK_OCCUPATIONAL GetValueOccupational()
        {
            HIS_KSK_OCCUPATIONAL obj = new HIS_KSK_OCCUPATIONAL();
            try
            {
                if (currentKskOverEight != null)
                    obj.ID = currentKskOverEight.ID;
                obj.RECENT_WORK_ONE = txtRecentWork.Text;
                obj.RECENT_WORK_TWO = txtRecentWork2.Text;

                obj.RECENT_ELEMENT_ONE = txtRecentElement.Text;
                obj.RECENT_ELEMENT_TWO = txtRecentElement2.Text;
                obj.PATHOLOGICAL_HISTORY_FAMILY = txtPathologicalFamily.Text;


                if (spnRecentWorkOneYear7.EditValue != null) obj.RECENT_WORK_ONE_YEAR = (long?)Int64.Parse(spnRecentWorkOneYear7.EditValue.ToString());
                if (spnRecentWorkOneMonth7.EditValue != null) obj.RECENT_WORK_ONE_MONTH = (long?)Int64.Parse(spnRecentWorkOneMonth7.EditValue.ToString());
                if (spnRecentElementOneYear.EditValue != null) obj.RECENT_ELEMENT_ONE_YEAR = (long?)Int64.Parse(spnRecentElementOneYear.EditValue.ToString());
                if (spnRecentElementOneMonth.EditValue != null) obj.RECENT_ELEMENT_ONE_MONTH = (long?)Int64.Parse(spnRecentElementOneMonth.EditValue.ToString());

                if (spnRecentWorkOneYear8.EditValue != null) obj.RECENT_WORK_TWO_YEAR = (long?)Int64.Parse(spnRecentWorkOneYear8.EditValue.ToString());
                if (spnRecentWorkOneMonth8.EditValue != null) obj.RECENT_WORK_TWO_MONTH = (long?)Int64.Parse(spnRecentWorkOneMonth8.EditValue.ToString());
                if (spnRecentElementOneYear2.EditValue != null) obj.RECENT_ELEMENT_TWO_YEAR = (long?)Int64.Parse(spnRecentElementOneYear2.EditValue.ToString());
                if (spnRecentElementOneMonth2.EditValue != null) obj.RECENT_ELEMENT_TWO_MONTH = (long?)Int64.Parse(spnRecentElementOneMonth2.EditValue.ToString());


                obj.RECENT_WORK_ONE_FROM = (dteRecentWorkOneFrom7.EditValue != null) ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteRecentWorkOneFrom7.DateTime) : null;
                obj.RECENT_WORK_ONE_TO = (dteRecentWorkOneTo7.EditValue != null) ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteRecentWorkOneTo7.DateTime) : null;
                obj.RECENT_WORK_TWO_FROM = (dteRecentWorkTwoFrom8.EditValue != null) ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteRecentWorkTwoFrom8.DateTime) : null;
                obj.RECENT_WORK_TWO_TO = (dteRecentWorkTwoTo8.EditValue != null) ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteRecentWorkTwoTo8.DateTime) : null;

                obj.PATHOLOGICAL_HISTORY = txtPathologicalHistory7.Text;
                obj.DHST_RANK = cboDhstRank7.EditValue != null ? (long?)Int64.Parse(cboDhstRank7.EditValue.ToString()) : null;
                obj.EXAM_CIRCULATION = txtExamCirculation7.Text;
                obj.EXAM_CIRCULATION_RANK = cboExamCirculationRank7.EditValue != null ? (long?)Int64.Parse(cboExamCirculationRank7.EditValue.ToString()) : null;

                obj.EXAM_RESPIRATORY = txtExamRespiratory7.Text;
                obj.EXAM_RESPIRATORY_RANK = cboExamRespiratoryRank7.EditValue != null ? (long?)Int64.Parse(cboExamRespiratoryRank7.EditValue.ToString()) : null;
                obj.EXAM_DIGESTION = txtExamDigestion7.Text;
                obj.EXAM_DIGESTION_RANK = cboExamDigestionRank7.EditValue != null ? (long?)Int64.Parse(cboExamDigestionRank7.EditValue.ToString()) : null;

                obj.EXAM_KIDNEY_UROLOGY = txtExamKidneyUrology7.Text;
                obj.EXAM_KIDNEY_UROLOGY_RANK = cboExamKidneyUrologyRank7.EditValue != null ? (long?)Int64.Parse(cboExamKidneyUrologyRank7.EditValue.ToString()) : null;
                obj.EXAM_NEUROLOGICAL = txtExamOend7.Text;
                obj.EXAM_NEUROLOGICAL_RANK = cboExamOendRank7.EditValue != null ? (long?)Int64.Parse(cboExamOendRank7.EditValue.ToString()) : null;

                obj.EXAM_OEND = txtExamOend7.Text;
                obj.EXAM_OEND_RANK = cboExamOendRank7.EditValue != null ? (long?)Int64.Parse(cboExamOendRank7.EditValue.ToString()) : null;

                obj.EXAM_MUSCLE_BONE = txtExamMuscleBone7.Text;
                obj.EXAM_MUSCLE_BONE_RANK = cboExamMuscleBoneRank7.EditValue != null ? (long?)Int64.Parse(cboExamMuscleBoneRank7.EditValue.ToString()) : null;
                obj.EXAM_MENTAL = txtExamMental7.Text;
                obj.EXAM_MENTAL_RANK = cboExamMentalRank7.EditValue != null ? (long?)Int64.Parse(cboExamMentalRank7.EditValue.ToString()) : null;

                obj.EXAM_SURGERY = txtExamSurgery7.Text;
                obj.EXAM_SURGERY_RANK = cboExamSurgeryRank7.EditValue != null ? (long?)Int64.Parse(cboExamSurgeryRank7.EditValue.ToString()) : null;
                obj.EXAM_DERMATOLOGY = txtExamDernatology7.Text;
                obj.EXAM_DERMATOLOGY_RANK = cboExamDernatologyRank7.EditValue != null ? (long?)Int64.Parse(cboExamDernatologyRank7.EditValue.ToString()) : null;

                obj.EXAM_OBSTETRIC = txtExamObstetric7.Text;
                obj.EXAM_OBSTETRIC_RANK = cboExamObstetricRank7.EditValue != null ? (long?)Int64.Parse(cboExamObstetricRank7.EditValue.ToString()) : null;
                obj.EXAM_NEUROLOGICAL = txtExamNeurological7.Text;
                obj.EXAM_NEUROLOGICAL_RANK = cboExamNeurologicalRank7.EditValue != null ? (long?)Int64.Parse(cboExamNeurologicalRank7.EditValue.ToString()) : null;

                obj.EXAM_DERMATOLOGY = txtExamDernatology7.Text;
                obj.EXAM_DERMATOLOGY_RANK = cboExamDernatologyRank7.EditValue != null ? (long?)Int64.Parse(cboExamDernatologyRank7.EditValue.ToString()) : null;
                obj.EXAM_EYESIGHT_RIGHT = txtExamEyeSightRight7.Text;
                obj.EXAM_EYESIGHT_LEFT = txtExamEyeSightLeft7.Text;

                obj.EXAM_EYESIGHT_GLASS_RIGHT = txtExamEyeSightGlassRight7.Text;
                obj.EXAM_EYESIGHT_GLASS_LEFT = txtExamEyeSightGlassLeft7.Text;
                obj.EXAM_EYE_DISEASE = txtExamEyeDisease7.Text;
                obj.EXAM_EYE_RANK = cboExamEyeRank7.EditValue != null ? (long?)Int64.Parse(cboExamEyeRank7.EditValue.ToString()) : null;

                obj.EXAM_ENT_LEFT_NORMAL = txtExamEntLeftNormal7.Text;
                obj.EXAM_ENT_LEFT_WHISPER = txtExamEntLeftWhisper7.Text;
                obj.EXAM_ENT_RIGHT_NORMAL = txtExamEntRightNomal7.Text;
                obj.EXAM_ENT_RIGHT_WHISPER = txtExamEntRightWhisper7.Text;

                obj.EXAM_ENT_DISEASE = txtExamEntDisease7.Text;
                obj.EXAM_ENT_RANK = cboExamEntDiseaseRank7.EditValue != null ? (long?)Int64.Parse(cboExamEntDiseaseRank7.EditValue.ToString()) : null;
                obj.EXAM_STOMATOLOGY_UPPER = txtExamStomatologyUpper7.Text;
                obj.EXAM_STOMATOLOGY_LOWER = txtExamStomatologyLower7.Text;

                obj.EXAM_STOMATOLOGY_DISEASE = txtExamStomatologyDisease7.Text;
                obj.EXAM_STOMATOLOGY_RANK = cboExamStomatologyRank7.EditValue != null ? (long?)Int64.Parse(cboExamStomatologyRank7.EditValue.ToString()) : null;
                obj.RESULT_SUBCLINICAL = txtResultSubclinical7.Text;
                obj.NOTE_SUBCLINICAL = txtNoteSubclinical7.Text;
                obj.NOTE_CLINICAL = txtNoteExamDernatology7.Text;

                obj.HEALTH_EXAM_RANK_ID = cboHealthExamRank7.EditValue != null ? (long?)Int64.Parse(cboHealthExamRank7.EditValue.ToString()) : null;
                obj.DISEASES = txtDiseases7.Text;
                obj.NOW_WORK_FROM = ((this.dtpStart7.EditValue != null) ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(new DateTime?(this.dtpStart7.DateTime)) : null);

                obj.EXAM_CIRCULATION_LOGINNAME = cboExamCirculationLoginName7.EditValue != null ? cboExamCirculationLoginName7.EditValue.ToString() : null;
                obj.EXAM_EYE_LOGINNAME = cboExamEyeLoginName7.EditValue != null ? cboExamEyeLoginName7.EditValue.ToString() : null;
                obj.EXAM_ENT_LOGINNAME = cboExamEntLoginName7.EditValue != null ? cboExamEntLoginName7.EditValue.ToString() : null;
                obj.EXAM_STOMATOLOGY_LOGINNAME = cboExamStomatologyLoginname7.EditValue != null ? cboExamStomatologyLoginname7.EditValue.ToString() : null;
                obj.EXAM_SUBCLINICAL_LOGINNAME = cboExamSubclinicalLoginName7.EditValue != null ? cboExamSubclinicalLoginName7.EditValue.ToString() : null;
                obj.CONCLUDER_LOGINNAME = cboConcluderLoginName7.EditValue != null ? cboConcluderLoginName7.EditValue.ToString() : null;
                obj.CONCLUDER_USERNAME = cboConcluderLoginName7.EditValue != null ? cboConcluderLoginName7.EditValue.ToString() : null;
                obj.PROVISIONAL_DIAGNOSIS = txtPreliminaryDiagnosis.Text;
                obj.DIAGNOSIS = txtDefiniteDiagnosis.Text;
                obj.CONCLUSION = txtResultConsultation.Text;
                obj.TREATMENT_INSTRUCTION = txtSolution.Text;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }

        private HIS_DHST GetValueDhstOccupational()
        {
            HIS_DHST obj = new HIS_DHST();
            try
            {
                if (dhstOccupational != null)
                    obj.ID = dhstOccupational.ID;
                if (spnBloodPressureMax7.EditValue != null)
                    obj.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spnBloodPressureMax7.Value.ToString());
                if (spnBloodPressureMin7.EditValue != null)
                    obj.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spnBloodPressureMin7.Value.ToString());
                if (spnHeight7.EditValue != null)
                    obj.HEIGHT = Inventec.Common.Number.Get.RoundCurrency(spnHeight7.Value, 2);
                if (spnPulse7.EditValue != null)
                    obj.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spnPulse7.Value.ToString());
                if (spnWeight7.EditValue != null)
                    obj.WEIGHT = Inventec.Common.Number.Get.RoundCurrency(spnWeight7.Value, 2);

                obj.EXECUTE_LOGINNAME = cboExecuteLoginName7.EditValue != null ? cboExecuteLoginName7.EditValue.ToString() : null;
                obj.EXECUTE_USERNAME = obj.EXECUTE_LOGINNAME != null ? BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == obj.EXECUTE_LOGINNAME).TDL_USERNAME : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }
        private void txtResultSubclinical7_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameSItem = ENameSItem.KET_QUA_7;
            GetSpecInformation(ReturnObject = false);
        }


        #region --PREVIEWKEYDOWN--
        private void txtPathologicalHistory7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnHeight7.Focus();
                    spnHeight7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnHeight7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnWeight7.Focus();
                    spnWeight7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnWeight7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnPulse7.Focus();
                    spnPulse7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnPulse7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnBloodPressureMax7.Focus();
                    spnBloodPressureMax7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnBloodPressureMax7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnBloodPressureMin7.Focus();
                    spnBloodPressureMin7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnBloodPressureMin7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDhstRank7.Focus();
                    cboDhstRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDhstRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamCirculation7.Focus();
                    txtExamCirculation7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamCirculation7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamCirculationRank7.Focus();
                    cboExamCirculationRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamCirculationRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamRespiratory7.Focus();
                    txtExamRespiratory7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamRespiratory7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamRespiratoryRank7.Focus();
                    cboExamRespiratoryRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboExamRespiratoryRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamDigestion7.Focus();
                    txtExamDigestion7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamDigestion7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamDigestionRank7.Focus();
                    cboExamDigestionRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamDigestionRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamKidneyUrology7.Focus();
                    txtExamKidneyUrology7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtExamKidneyUrology7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamKidneyUrologyRank7.Focus();
                    cboExamKidneyUrologyRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamKidneyUrologyRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamOend7.Focus();
                    txtExamOend7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamNeurological7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamOendRank7.Focus();
                    cboExamOendRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamNeurologicalRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamMuscleBone7.Focus();
                    txtExamMuscleBone7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMuscleBone7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMuscleBoneRank7.Focus();
                    cboExamMuscleBoneRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMuscleBoneRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamNeurological7.Focus();
                    txtExamNeurological7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamOend7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamNeurologicalRank7.Focus();
                    cboExamNeurologicalRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamOendRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamMental7.Focus();
                    txtExamMental7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMental7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMentalRank7.Focus();
                    cboExamMentalRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMentalRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamSurgery7.Focus();
                    txtExamSurgery7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamSurgery7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamSurgeryRank7.Focus();
                    cboExamSurgeryRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamSurgeryRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamObstetric7.Focus();
                    txtExamObstetric7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamObstetric7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamObstetricRank7.Focus();
                    cboExamObstetricRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamObstetricRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEyeSightRight7.Focus();
                    txtExamEyeSightRight7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightRight7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightLeft7.Focus();
                    txtExamEyeSightLeft7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightLeft7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassRight7.Focus();
                    txtExamEyeSightGlassRight7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassRight7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassLeft7.Focus();
                    txtExamEyeSightGlassLeft7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassLeft7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeDisease7.Focus();
                    txtExamEyeDisease7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeDisease7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEyeRank7.Focus();
                    cboExamEyeRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboExamEyeRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEntLeftNormal7.Focus();
                    txtExamEntLeftNormal7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftNormal7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntLeftWhisper7.Focus();
                    txtExamEyeDisease7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftWhisper7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightNomal7.Focus();
                    txtExamEntRightNomal7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightNomal7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightWhisper7.Focus();
                    txtExamEntRightWhisper7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightWhisper7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntDisease7.Focus();
                    txtExamEntDisease7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntDisease7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEntDiseaseRank7.Focus();
                    cboExamEntDiseaseRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEntDiseaseRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamStomatologyUpper7.Focus();
                    txtExamStomatologyUpper7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyUpper7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamStomatologyLower7.Focus();
                    txtExamStomatologyLower7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyLower7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamStomatologyDisease7.Focus();
                    txtExamStomatologyDisease7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyDisease7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamStomatologyRank7.Focus();
                    cboExamStomatologyRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamStomatologyRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamDernatology7.Focus();
                    txtExamDernatology7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            #endregion
        }

        private void txtExamDernatology7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamDernatologyRank7.Focus();
                    cboExamDernatologyRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
       
        private void txtNoteExamDernatology7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamDernatologyRank7.Focus();
                    cboExamDernatologyRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboExamDernatologyRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtResultSubclinical7.Focus();
                    txtResultSubclinical7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
       
        private void txtResultSubclinical7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNoteSubclinical7.Focus();
                    txtNoteSubclinical7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNoteSubclinical7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboHealthExamRank7.Focus();
                    cboHealthExamRank7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHealthExamRank7_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtDiseases7.Focus();
                    txtDiseases7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiseases7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPreliminaryDiagnosis.Focus();
                    txtPreliminaryDiagnosis.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPreliminaryDiagnosis_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtResultConsultation.Focus();
                    txtResultConsultation.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtResultConsultation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtResultConsultation.Focus();
                    txtResultConsultation.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDefiniteDiagnosis_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSolution.Focus();
                    txtSolution.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSolution_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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



        private void txtRecentWork_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnRecentWorkOneYear7.Focus();
                    spnRecentWorkOneYear7.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void spnRecentWorkOneYear7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            
        }

        private void spnRecentWorkOneMonth7_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dteRecentWorkOneFrom7.Focus();
                    dteRecentWorkOneFrom7.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void spnRecentElementOneYear_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnRecentElementOneMonth.Focus();
                    spnRecentElementOneMonth.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void spnRecentElementOneMonth_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRecentWork2.Focus();
                    
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtRecentWork2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnRecentWorkOneYear8.Focus();
                    spnRecentWorkOneYear8.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void spnRecentWorkOneYear8_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnRecentWorkOneMonth8.Focus();
                    spnRecentWorkOneMonth8.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnRecentWorkOneMonth8_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnRecentElementOneYear2.Focus();
                    spnRecentElementOneYear2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnRecentElementOneYear2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnRecentElementOneMonth2.Focus();
                    spnRecentElementOneMonth2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnRecentElementOneMonth2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPathologicalFamily.Focus();
                  
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtPathologicalFamily_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnRecentWorkOneYear8.Focus();
                    spnRecentWorkOneYear8.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
