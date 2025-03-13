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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private HIS_DHST dhstGeneral { get; set; }

        private void ResetControl()
        {
            try
            {
                spnDiseaseOneYear.EditValue = null;
                spnDiseaseTwoYear.EditValue = null;
                spnDiseaseOccuOneYear.EditValue = null;
                spnDiseaseOccuTwoYear.EditValue = null;
                spnRecentWordOneYear.EditValue = null;
                spnRecentWorkOneMonth.EditValue = null;
                spnRecentWorkTwoYear.EditValue = null;
                spnRecentWorkTwoMonth.EditValue = null;
                spnHeight.EditValue = null;
                spnPulse.EditValue = null;
                spnWeight.EditValue = null;
                spnBloodPressureMax.EditValue = null;
                spnBloodPressureMin.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataPageGenaral()
        {
            try
            {
                ResetControl();
                SetDataCboRank(cboDhstRank);
                SetDataCboRank(cboExamCirculationRank);
                SetDataCboRank(cboExamRespiratoryRank);
                SetDataCboRank(cboExamDigestionRank);
                SetDataCboRank(cboExamKidneyUrologyRank);
                SetDataCboRank(cboExamNeurologicalRank);
                SetDataCboRank(cboExamMuscleBoneRank);
                SetDataCboRank(cboExamOendRank);
                SetDataCboRank(cboExamMentalRank);
                SetDataCboRank(cboExamSurgeryRank);
                SetDataCboRank(cboExamObstetricRank);
                SetDataCboRank(cboExamEyeRank);
                SetDataCboRank(cboExamEntDiseaseRank);
                SetDataCboRank(cboExamStomatologyRank);
                SetDataCboRank(cboExamDernatologyRank);
                SetDataCboRank(cboHealthExamRank);
                SetDataCboExamLoginName(cboExecuteLoginName);
                SetDataCboExamLoginName(cboExamCirculationLoginName);
                SetDataCboExamLoginName(cboExamEyeLoginName);
                SetDataCboExamLoginName(cboExamEntLoginName);
                SetDataCboExamLoginName(cboExamStomatologyLoginName);
                SetDataCboExamLoginName(cboExamSubclinicalLoginName);
                SetDataCboExamLoginName(cboConcluderLoginName);
                FillDataGenaral();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataGenaral()
        {
            try
            {
                if (currentServiceReq != null)
                {
                    CommonParam param = new CommonParam();
                    HisKskGeneralFilter filter = new HisKskGeneralFilter();
                    filter.SERVICE_REQ_ID = currentServiceReq.ID;
                    var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_GENERAL>>("api/HisKskGeneral/Get", ApiConsumers.MosConsumer, filter, param);
                    if (data != null && data.Count > 0)
                    {
                        currentKskGeneral = data.First();
                        txtDiseaseOne.Text = currentKskGeneral.HISTORY_DISEASE_ONE;
                        if (!string.IsNullOrEmpty(currentKskGeneral.HISTORY_DISEASE_ONE_YEAR))
                            spnDiseaseOneYear.EditValue = Int64.Parse(currentKskGeneral.HISTORY_DISEASE_ONE_YEAR);
                        
                        txtDiseaseTwo.Text = currentKskGeneral.HISTORY_DISEASE_TWO;
                        if (!string.IsNullOrEmpty(currentKskGeneral.HISTORY_DISEASE_TWO_YEAR))
                            spnDiseaseTwoYear.EditValue = Int64.Parse(currentKskGeneral.HISTORY_DISEASE_TWO_YEAR);
                        
                        txtOccuOne.Text = currentKskGeneral.HISTORY_DISEASE_OCCU_ONE;
                        if (!string.IsNullOrEmpty(currentKskGeneral.HISTORY_DISEASE_OCCU_ONE_YEAR))
                            spnDiseaseOccuOneYear.EditValue = Int64.Parse(currentKskGeneral.HISTORY_DISEASE_OCCU_ONE_YEAR);
                        
                        txtDiseaseOccuTwo.Text = currentKskGeneral.HISTORY_DISEASE_OCCU_TWO;
                        if (!string.IsNullOrEmpty(currentKskGeneral.HISTORY_DISEASE_OCCU_TWO_YEAR))
                            spnDiseaseOccuTwoYear.EditValue = Int64.Parse(currentKskGeneral.HISTORY_DISEASE_OCCU_TWO_YEAR);


                        spnRecentWordOneYear.EditValue = currentKskGeneral.RECENT_WORK_ONE_YEAR ?? null;
                        spnRecentWorkOneMonth.EditValue = currentKskGeneral.RECENT_WORK_ONE_MONTH ?? null;
                        spnRecentWorkTwoYear.EditValue = currentKskGeneral.RECENT_WORK_TWO_YEAR ?? null;
                        spnRecentWorkTwoMonth.EditValue = currentKskGeneral.RECENT_WORK_TWO_MONTH ?? null;

                        if (currentKskGeneral.RECENT_WORK_ONE_FROM != null && currentKskGeneral.RECENT_WORK_ONE_FROM > 0)
                        {
                            dteRecentWorkOneFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentKskGeneral.RECENT_WORK_ONE_FROM ?? 0) ?? DateTime.Now;
                        }
                        if (currentKskGeneral.RECENT_WORK_ONE_TO != null && currentKskGeneral.RECENT_WORK_ONE_TO > 0)
                        {
                            dteRecentWorkOneTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentKskGeneral.RECENT_WORK_ONE_TO ?? 0) ?? DateTime.Now;
                        }
                        if (currentKskGeneral.RECENT_WORK_TWO_FROM != null && currentKskGeneral.RECENT_WORK_TWO_FROM > 0)
                        {
                            dteRecentWorkTwoFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentKskGeneral.RECENT_WORK_TWO_FROM ?? 0) ?? DateTime.Now;
                        }
                        if (currentKskGeneral.RECENT_WORK_TWO_TO != null && currentKskGeneral.RECENT_WORK_TWO_TO > 0)
                        {
                            dteRecentWorkTwoTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentKskGeneral.RECENT_WORK_TWO_TO ?? 0) ?? DateTime.Now;
                        }

                        if (this.currentKskGeneral.NOW_WORK_FROM != null && this.currentKskGeneral.NOW_WORK_FROM > 0)
                        {
                            this.dtpStart.DateTime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentKskGeneral.NOW_WORK_FROM ?? 0) ?? DateTime.Now);
                        }
                        else
                        {
                            dtpStart.EditValue = null;
                        }
                        
                        txtPathologicalHistory.Text = currentKskGeneral.PATHOLOGICAL_HISTORY;
                        cboDhstRank.EditValue = currentKskGeneral.DHST_RANK;
                        txtExamCirculation.Text = currentKskGeneral.EXAM_CIRCULATION;
                        cboExamCirculationRank.EditValue = currentKskGeneral.EXAM_CIRCULATION_RANK;
                        txtExamRespiratory.Text = currentKskGeneral.EXAM_RESPIRATORY;
                        cboExamRespiratoryRank.EditValue = currentKskGeneral.EXAM_RESPIRATORY_RANK;
                        txtExamDigestion.Text = currentKskGeneral.EXAM_DIGESTION;
                        cboExamDigestionRank.EditValue = currentKskGeneral.EXAM_DIGESTION_RANK;
                        txtExamKidneyUrology.Text = currentKskGeneral.EXAM_KIDNEY_UROLOGY;
                        cboExamKidneyUrologyRank.EditValue = currentKskGeneral.EXAM_KIDNEY_UROLOGY_RANK;
                        txtExamNeurological.Text = currentKskGeneral.EXAM_NEUROLOGICAL;
                        cboExamNeurologicalRank.EditValue = currentKskGeneral.EXAM_NEUROLOGICAL_RANK;
                        txtExamMuscleBone.Text = currentKskGeneral.EXAM_MUSCLE_BONE;
                        cboExamMuscleBoneRank.EditValue = currentKskGeneral.EXAM_MUSCLE_BONE_RANK;
                        txtExamOend.Text = currentKskGeneral.EXAM_OEND;
                        cboExamOendRank.EditValue = currentKskGeneral.EXAM_OEND_RANK;
                        txtExamMental.Text = currentKskGeneral.EXAM_MENTAL;
                        cboExamMentalRank.EditValue = currentKskGeneral.EXAM_MENTAL_RANK;
                        txtExamSurgery.Text = currentKskGeneral.EXAM_SURGERY;
                        cboExamSurgeryRank.EditValue = currentKskGeneral.EXAM_SURGERY_RANK;
                        txtExamDernatology.Text = currentKskGeneral.EXAM_DERMATOLOGY;
                        cboExamDernatologyRank.EditValue = currentKskGeneral.EXAM_DERMATOLOGY_RANK;
                        txtExamObstetric.Text = currentKskGeneral.EXAM_OBSTETRIC;
                        cboExamObstetricRank.EditValue = currentKskGeneral.EXAM_OBSTETRIC_RANK;

                        txtExamEyeSightRight.Text = currentKskGeneral.EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft.Text = currentKskGeneral.EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight.Text = currentKskGeneral.EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft.Text = currentKskGeneral.EXAM_EYESIGHT_GLASS_LEFT;
                        txtExamEyeDisease.Text = currentKskGeneral.EXAM_EYE_DISEASE;
                        cboExamEyeRank.EditValue = currentKskGeneral.EXAM_EYE_RANK;
                        txtExamEntLeftNormal.Text = currentKskGeneral.EXAM_ENT_LEFT_NORMAL;
                        txtExamEntLeftWhisper.Text = currentKskGeneral.EXAM_ENT_LEFT_WHISPER;
                        txtExamEntRightNomal.Text = currentKskGeneral.EXAM_ENT_RIGHT_NORMAL;
                        txtExamEntRightWhisper.Text = currentKskGeneral.EXAM_ENT_RIGHT_WHISPER;
                        txtExamEntDisease.Text = currentKskGeneral.EXAM_ENT_DISEASE;
                        cboExamEntDiseaseRank.EditValue = currentKskGeneral.EXAM_ENT_RANK;
                        txtExamStomatologyUpper.Text = currentKskGeneral.EXAM_STOMATOLOGY_UPPER;
                        txtExamStomatologyLower.Text = currentKskGeneral.EXAM_STOMATOLOGY_LOWER;
                        txtExamStomatologyDisease.Text = currentKskGeneral.EXAM_STOMATOLOGY_DISEASE;
                        cboExamStomatologyRank.EditValue = currentKskGeneral.EXAM_STOMATOLOGY_RANK;

                        txtResultSubclinical.Text = currentKskGeneral.RESULT_SUBCLINICAL;
                        txtNoteSubclinical.Text = currentKskGeneral.NOTE_SUBCLINICAL;
                        cboHealthExamRank.EditValue = currentKskGeneral.HEALTH_EXAM_RANK_ID;
                        txtDiseases.Text = currentKskGeneral.DISEASES;
                        if (currentKskGeneral.DHST_ID != null && currentKskGeneral.DHST_ID > 0)
                        {
                            HisDhstFilter dhstFilter = new HisDhstFilter();
                            dhstFilter.ID = currentKskGeneral.DHST_ID;
                            var dataDhst = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                            if (dataDhst != null && dataDhst.Count > 0)
                            {
                                dhstGeneral = dataDhst.First();
                                spnHeight.EditValue = dhstGeneral.HEIGHT;
                                spnPulse.EditValue = dhstGeneral.PULSE;
                                spnWeight.EditValue = dhstGeneral.WEIGHT;
                                spnBloodPressureMax.EditValue = dhstGeneral.BLOOD_PRESSURE_MAX;
                                spnBloodPressureMin.EditValue = dhstGeneral.BLOOD_PRESSURE_MIN;
                                //txtVirBmi.Text = currentDhst.VIR_BMI!=null ? currentDhst.VIR_BMI.ToString() : "";
                                FillNoteBMI(spnHeight, spnWeight, txtVirBmi);
                                cboExecuteLoginName.EditValue = dhstGeneral.EXECUTE_LOGINNAME;
                            }
                        }
                        cboExamCirculationLoginName.EditValue = currentKskGeneral.EXAM_CIRCULATION_LOGINNAME;
                        cboExamEyeLoginName.EditValue = currentKskGeneral.EXAM_EYE_LOGINNAME;
                        cboExamEntLoginName.EditValue = currentKskGeneral.EXAM_ENT_LOGINNAME;
                        cboExamStomatologyLoginName.EditValue = currentKskGeneral.EXAM_STOMATOLOGY_LOGINNAME;
                        cboExamSubclinicalLoginName.EditValue = currentKskGeneral.EXAM_SUBCLINICAL_LOGINNAME;
                        cboConcluderLoginName.EditValue = currentKskGeneral.CONCLUDER_LOGINNAME;
                    }
                    else
                    {
                        txtPathologicalHistory.Text = currentServiceReq.PATHOLOGICAL_HISTORY;
                        txtExamCirculation.Text = currentServiceReq.PART_EXAM_CIRCULATION;
                        txtExamRespiratory.Text = currentServiceReq.PART_EXAM_RESPIRATORY;
                        txtExamDigestion.Text = currentServiceReq.PART_EXAM_DIGESTION;
                        txtExamKidneyUrology.Text = currentServiceReq.PART_EXAM_KIDNEY_UROLOGY;
                        txtExamOend.Text = currentServiceReq.PART_EXAM_OEND;
                        txtExamMuscleBone.Text = currentServiceReq.PART_EXAM_MUSCLE_BONE;
                        txtExamNeurological.Text = currentServiceReq.PART_EXAM_NEUROLOGICAL;
                        txtExamMental.Text = currentServiceReq.PART_EXAM_MENTAL;
                        txtExamObstetric.Text = currentServiceReq.PART_EXAM_OBSTETRIC;

                        txtExamEyeSightRight.Text = currentServiceReq.PART_EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft.Text = currentServiceReq.PART_EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_LEFT;

                        txtExamEntLeftNormal.Text = currentServiceReq.PART_EXAM_EAR_LEFT_NORMAL;
                        txtExamEntLeftWhisper.Text = currentServiceReq.PART_EXAM_EAR_LEFT_WHISPER;
                        txtExamEntRightNomal.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_NORMAL;
                        txtExamEntRightWhisper.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_WHISPER;

                        txtExamStomatologyUpper.Text = currentServiceReq.PART_EXAM_UPPER_JAW;
                        txtExamStomatologyLower.Text = currentServiceReq.PART_EXAM_LOWER_JAW;
                        txtExamDernatology.Text = currentServiceReq.PART_EXAM_DERMATOLOGY;
                        txtExamSurgery.Text = currentServiceReq.SUBCLINICAL;
                        if (currentServiceReq.DHST_ID != null && currentServiceReq.DHST_ID > 0)
                        {
                            HisDhstFilter dhstFilter = new HisDhstFilter();
                            dhstFilter.ID = currentServiceReq.DHST_ID;
                            var dataDhst = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                            if (dataDhst != null && dataDhst.Count > 0)
                            {
                                var currentDhst = dataDhst.First();
                                spnHeight.EditValue = currentDhst.HEIGHT;
                                spnPulse.EditValue = currentDhst.PULSE;
                                spnWeight.EditValue = currentDhst.WEIGHT;
                                spnBloodPressureMax.EditValue = currentDhst.BLOOD_PRESSURE_MAX;
                                spnBloodPressureMin.EditValue = currentDhst.BLOOD_PRESSURE_MIN;
                                //txtVirBmi.Text = currentDhst.VIR_BMI!=null ? currentDhst.VIR_BMI.ToString() : "";
                                FillNoteBMI(spnHeight, spnWeight, txtVirBmi);
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

        private void spnHeight_EditValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                FillNoteBMI(spnHeight, spnWeight, txtVirBmi);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnWeight_EditValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                FillNoteBMI(spnHeight, spnWeight, txtVirBmi);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_KSK_GENERAL GetValueGeneral()
        {
            HIS_KSK_GENERAL obj = new HIS_KSK_GENERAL();
            try
            {
                if (currentKskOverEight != null)
                    obj.ID = currentKskOverEight.ID;
                obj.HISTORY_DISEASE_ONE = txtDiseaseOne.Text;
                obj.HISTORY_DISEASE_ONE_YEAR = spnDiseaseOneYear.EditValue != null ? spnDiseaseOneYear.EditValue.ToString() : null;
                obj.HISTORY_DISEASE_TWO = txtDiseaseTwo.Text;
                obj.HISTORY_DISEASE_TWO_YEAR = spnDiseaseTwoYear.EditValue != null ? spnDiseaseTwoYear.EditValue.ToString() : null;

                obj.HISTORY_DISEASE_OCCU_ONE = txtOccuOne.Text;
                obj.HISTORY_DISEASE_OCCU_ONE_YEAR = spnDiseaseOccuOneYear.EditValue != null ? spnDiseaseOccuOneYear.EditValue.ToString() : null;
                obj.HISTORY_DISEASE_OCCU_TWO = txtDiseaseOccuTwo.Text;
                obj.HISTORY_DISEASE_OCCU_TWO_YEAR = spnDiseaseOccuTwoYear.EditValue != null ? spnDiseaseOccuTwoYear.EditValue.ToString() : null;

                if (spnRecentWordOneYear.EditValue != null) obj.RECENT_WORK_ONE_YEAR = (long?)Int64.Parse(spnRecentWordOneYear.EditValue.ToString());
                if (spnRecentWorkOneMonth.EditValue != null) obj.RECENT_WORK_ONE_MONTH = (long?)Int64.Parse(spnRecentWorkOneMonth.EditValue.ToString());
                if (spnRecentWorkTwoYear.EditValue != null) obj.RECENT_WORK_TWO_YEAR = (long?)Int64.Parse(spnRecentWorkTwoYear.EditValue.ToString());
                if (spnRecentWorkTwoMonth.EditValue != null) obj.RECENT_WORK_TWO_MONTH = (long?)Int64.Parse(spnRecentWorkTwoMonth.EditValue.ToString());

                obj.RECENT_WORK_ONE_FROM = (dteRecentWorkOneFrom.EditValue != null) ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteRecentWorkOneFrom.DateTime) : null;
                obj.RECENT_WORK_ONE_TO = (dteRecentWorkOneTo.EditValue != null) ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteRecentWorkOneTo.DateTime) : null;
                obj.RECENT_WORK_TWO_FROM = (dteRecentWorkTwoFrom.EditValue != null) ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteRecentWorkTwoFrom.DateTime) : null;
                obj.RECENT_WORK_TWO_TO = (dteRecentWorkTwoTo.EditValue != null) ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteRecentWorkTwoTo.DateTime) : null;

                obj.PATHOLOGICAL_HISTORY = txtPathologicalHistory.Text;
                obj.DHST_RANK = cboDhstRank.EditValue != null ? (long?)Int64.Parse(cboDhstRank.EditValue.ToString()) : null;
                obj.EXAM_CIRCULATION = txtExamCirculation.Text;
                obj.EXAM_CIRCULATION_RANK = cboExamCirculationRank.EditValue != null ? (long?)Int64.Parse(cboExamCirculationRank.EditValue.ToString()) : null;

                obj.EXAM_RESPIRATORY = txtExamRespiratory.Text;
                obj.EXAM_RESPIRATORY_RANK = cboExamRespiratoryRank.EditValue != null ? (long?)Int64.Parse(cboExamRespiratoryRank.EditValue.ToString()) : null;
                obj.EXAM_DIGESTION = txtExamDigestion.Text;
                obj.EXAM_DIGESTION_RANK = cboExamDigestionRank.EditValue != null ? (long?)Int64.Parse(cboExamDigestionRank.EditValue.ToString()) : null;

                obj.EXAM_KIDNEY_UROLOGY = txtExamKidneyUrology.Text;
                obj.EXAM_KIDNEY_UROLOGY_RANK = cboExamKidneyUrologyRank.EditValue != null ? (long?)Int64.Parse(cboExamKidneyUrologyRank.EditValue.ToString()) : null;
                obj.EXAM_NEUROLOGICAL = txtExamNeurological.Text;
                obj.EXAM_NEUROLOGICAL_RANK = cboExamNeurologicalRank.EditValue != null ? (long?)Int64.Parse(cboExamNeurologicalRank.EditValue.ToString()) : null;

                obj.EXAM_MUSCLE_BONE = txtExamMuscleBone.Text;
                obj.EXAM_MUSCLE_BONE_RANK = cboExamMuscleBoneRank.EditValue != null ? (long?)Int64.Parse(cboExamMuscleBoneRank.EditValue.ToString()) : null;
                obj.EXAM_MENTAL = txtExamMental.Text;
                obj.EXAM_MENTAL_RANK = cboExamMentalRank.EditValue != null ? (long?)Int64.Parse(cboExamMentalRank.EditValue.ToString()) : null;

                obj.EXAM_SURGERY = txtExamSurgery.Text;
                obj.EXAM_SURGERY_RANK = cboExamSurgeryRank.EditValue != null ? (long?)Int64.Parse(cboExamSurgeryRank.EditValue.ToString()) : null;
                obj.EXAM_DERMATOLOGY = txtExamDernatology.Text;
                obj.EXAM_DERMATOLOGY_RANK = cboExamDernatologyRank.EditValue != null ? (long?)Int64.Parse(cboExamDernatologyRank.EditValue.ToString()) : null;

                obj.EXAM_OBSTETRIC = txtExamObstetric.Text;
                obj.EXAM_OBSTETRIC_RANK = cboExamObstetricRank.EditValue != null ? (long?)Int64.Parse(cboExamObstetricRank.EditValue.ToString()) : null;
                obj.EXAM_OEND = txtExamOend.Text;
                obj.EXAM_OEND_RANK = cboExamOendRank.EditValue != null ? (long?)Int64.Parse(cboExamOendRank.EditValue.ToString()) : null;

                obj.EXAM_DERMATOLOGY = txtExamDernatology.Text;
                obj.EXAM_DERMATOLOGY_RANK = cboExamDernatologyRank.EditValue != null ? (long?)Int64.Parse(cboExamDernatologyRank.EditValue.ToString()) : null;
                obj.EXAM_EYESIGHT_RIGHT = txtExamEyeSightRight.Text;
                obj.EXAM_EYESIGHT_LEFT = txtExamEyeSightLeft.Text;

                obj.EXAM_EYESIGHT_GLASS_RIGHT = txtExamEyeSightGlassRight.Text;
                obj.EXAM_EYESIGHT_GLASS_LEFT = txtExamEyeSightGlassLeft.Text;
                obj.EXAM_EYE_DISEASE = txtExamEyeDisease.Text;
                obj.EXAM_EYE_RANK = cboExamEyeRank.EditValue != null ? (long?)Int64.Parse(cboExamEyeRank.EditValue.ToString()) : null;

                obj.EXAM_ENT_LEFT_NORMAL = txtExamEntLeftNormal.Text;
                obj.EXAM_ENT_LEFT_WHISPER = txtExamEntLeftWhisper.Text;
                obj.EXAM_ENT_RIGHT_NORMAL = txtExamEntRightNomal.Text;
                obj.EXAM_ENT_RIGHT_WHISPER = txtExamEntRightWhisper.Text;

                obj.EXAM_ENT_DISEASE = txtExamEntDisease.Text;
                obj.EXAM_ENT_RANK = cboExamEntDiseaseRank.EditValue != null ? (long?)Int64.Parse(cboExamEntDiseaseRank.EditValue.ToString()) : null;
                obj.EXAM_STOMATOLOGY_UPPER = txtExamStomatologyUpper.Text;
                obj.EXAM_STOMATOLOGY_LOWER = txtExamStomatologyLower.Text;

                obj.EXAM_STOMATOLOGY_DISEASE = txtExamStomatologyDisease.Text;
                obj.EXAM_STOMATOLOGY_RANK = cboExamStomatologyRank.EditValue != null ? (long?)Int64.Parse(cboExamStomatologyRank.EditValue.ToString()) : null;
                obj.RESULT_SUBCLINICAL = txtResultSubclinical.Text;
                obj.NOTE_SUBCLINICAL = txtNoteSubclinical.Text;

                obj.HEALTH_EXAM_RANK_ID = cboHealthExamRank.EditValue != null ? (long?)Int64.Parse(cboHealthExamRank.EditValue.ToString()) : null;
                obj.DISEASES = txtDiseases.Text;
                obj.NOW_WORK_FROM = ((this.dtpStart.EditValue != null) ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(new DateTime?(this.dtpStart.DateTime)) : null);

                obj.EXAM_CIRCULATION_LOGINNAME = cboExamCirculationLoginName.EditValue != null ? cboExamCirculationLoginName.EditValue.ToString() : null;
                obj.EXAM_EYE_LOGINNAME = cboExamEyeLoginName.EditValue != null ? cboExamEyeLoginName.EditValue.ToString() : null;
                obj.EXAM_ENT_LOGINNAME = cboExamEntLoginName.EditValue != null ? cboExamEntLoginName.EditValue.ToString() : null;
                obj.EXAM_STOMATOLOGY_LOGINNAME = cboExamStomatologyLoginName.EditValue != null ? cboExamStomatologyLoginName.EditValue.ToString() : null;
                obj.EXAM_SUBCLINICAL_LOGINNAME = cboExamSubclinicalLoginName.EditValue != null ? cboExamSubclinicalLoginName.EditValue.ToString() : null;
                obj.CONCLUDER_LOGINNAME = cboConcluderLoginName.EditValue != null ? cboConcluderLoginName.EditValue.ToString() : null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }

        private HIS_DHST GetValueDhstGeneral()
        {
            HIS_DHST obj = new HIS_DHST();
            try
            {
                if (dhstGeneral != null)
                    obj.ID = dhstGeneral.ID;
                if (spnBloodPressureMax.EditValue != null)
                    obj.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spnBloodPressureMax.Value.ToString());
                if (spnBloodPressureMin.EditValue != null)
                    obj.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spnBloodPressureMin.Value.ToString());
                if (spnHeight.EditValue != null)
                    obj.HEIGHT = Inventec.Common.Number.Get.RoundCurrency(spnHeight.Value, 2);
                if (spnPulse.EditValue != null)
                    obj.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spnPulse.Value.ToString());
                if (spnWeight.EditValue != null)
                    obj.WEIGHT = Inventec.Common.Number.Get.RoundCurrency(spnWeight.Value, 2);

                obj.EXECUTE_LOGINNAME = cboExecuteLoginName.EditValue != null ? cboExecuteLoginName.EditValue.ToString() : null;
                obj.EXECUTE_USERNAME = obj.EXECUTE_LOGINNAME != null ? BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == obj.EXECUTE_LOGINNAME).TDL_USERNAME : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }
        private void txtResultSubclinical_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameSItem = ENameSItem.KET_LUAN_1;
            GetSpecInformation(ReturnObject = false);
        }

        #region --PREVIEWKEYDOWN--
        private void txtPathologicalHistory_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnHeight.Focus();
                    spnHeight.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnHeight_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnWeight.Focus();
                    spnWeight.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnWeight_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnPulse.Focus();
                    spnPulse.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnPulse_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnBloodPressureMax.Focus();
                    spnBloodPressureMax.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnBloodPressureMax_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnBloodPressureMin.Focus();
                    spnBloodPressureMin.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnBloodPressureMin_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDhstRank.Focus();
                    cboDhstRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDhstRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamCirculation.Focus();
                    txtExamCirculation.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamCirculation_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamCirculationRank.Focus();
                    cboExamCirculationRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamCirculationRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamRespiratory.Focus();
                    txtExamRespiratory.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamRespiratory_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamRespiratoryRank.Focus();
                    cboExamRespiratoryRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamRespiratoryRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamDigestion.Focus();
                    txtExamDigestion.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamDigestion_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamDigestionRank.Focus();
                    cboExamDigestionRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamDigestionRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamKidneyUrology.Focus();
                    txtExamKidneyUrology.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtExamKidneyUrology_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamKidneyUrologyRank.Focus();
                    cboExamKidneyUrologyRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamKidneyUrologyRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamNeurological.Focus();
                    txtExamNeurological.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamNeurological_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamNeurologicalRank.Focus();
                    cboExamNeurologicalRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamNeurologicalRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamMuscleBone.Focus();
                    txtExamMuscleBone.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMuscleBone_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMuscleBoneRank.Focus();
                    cboExamMuscleBoneRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMuscleBoneRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamOend.Focus();
                    txtExamOend.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamOend_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamOendRank.Focus();
                    cboExamOendRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamOendRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamMental.Focus();
                    txtExamMental.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMental_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMentalRank.Focus();
                    cboExamMentalRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMentalRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamSurgery.Focus();
                    txtExamSurgery.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamSurgery_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamSurgeryRank.Focus();
                    cboExamSurgeryRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamSurgeryRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamObstetric.Focus();
                    txtExamObstetric.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamObstetric_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamObstetricRank.Focus();
                    cboExamObstetricRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamObstetricRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEyeSightRight.Focus();
                    txtExamEyeSightRight.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightRight_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightLeft.Focus();
                    txtExamEyeSightLeft.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightLeft_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassRight.Focus();
                    txtExamEyeSightGlassRight.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassRight_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassLeft.Focus();
                    txtExamEyeSightGlassLeft.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassLeft_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeDisease.Focus();
                    txtExamEyeDisease.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeDisease_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEyeRank.Focus();
                    cboExamEyeRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEyeRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEntLeftNormal.Focus();
                    txtExamEntLeftNormal.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftNormal_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntLeftWhisper.Focus();
                    txtExamEyeDisease.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftWhisper_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightNomal.Focus();
                    txtExamEntRightNomal.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightNomal_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightWhisper.Focus();
                    txtExamEntRightWhisper.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightWhisper_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntDisease.Focus();
                    txtExamEntDisease.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntDisease_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEntDiseaseRank.Focus();
                    cboExamEntDiseaseRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEntDiseaseRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamStomatologyUpper.Focus();
                    txtExamStomatologyUpper.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyUpper_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamStomatologyLower.Focus();
                    txtExamStomatologyLower.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyLower_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamStomatologyDisease.Focus();
                    txtExamStomatologyDisease.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyDisease_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamStomatologyRank.Focus();
                    cboExamStomatologyRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamStomatologyRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamDernatology.Focus();
                    txtExamDernatology.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamDernatology_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamDernatologyRank.Focus();
                    cboExamDernatologyRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamDernatologyRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtResultSubclinical.Focus();
                    txtResultSubclinical.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtResultSubclinical_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNoteSubclinical.Focus();
                    txtNoteSubclinical.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNoteSubclinical_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboHealthExamRank.Focus();
                    cboHealthExamRank.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHealthExamRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtDiseases.Focus();
                    txtDiseases.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiseases_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
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

        private void spnRecentWordOneYear_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnRecentWorkOneMonth.Focus();
                    spnRecentWorkOneMonth.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnRecentWorkOneMonth_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dteRecentWorkOneFrom.Focus();
                    dteRecentWorkOneFrom.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dteRecentWorkOneFrom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dteRecentWorkOneTo.Focus();
                    dteRecentWorkOneTo.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dteRecentWorkOneTo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    spnRecentWorkTwoYear.Focus();
                    spnRecentWorkTwoYear.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnRecentWorkTwoYear_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnRecentWorkTwoMonth.Focus();
                    spnRecentWorkTwoMonth.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnRecentWorkTwoMonth_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dteRecentWorkTwoFrom.Focus();
                    dteRecentWorkTwoFrom.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dteRecentWorkTwoFrom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dteRecentWorkTwoTo.Focus();
                    dteRecentWorkTwoTo.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dteRecentWorkTwoTo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtDiseaseOne.Focus();
                    txtDiseaseOne.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiseaseOne_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnDiseaseOneYear.Focus();
                    spnDiseaseOneYear.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnDiseaseOneYear_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtOccuOne.Focus();
                    txtOccuOne.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtOccuOne_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnDiseaseOccuOneYear.Focus();
                    spnDiseaseOccuOneYear.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnDiseaseOccuOneYear_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiseaseTwo.Focus();
                    txtDiseaseTwo.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiseaseTwo_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnDiseaseTwoYear.Focus();
                    spnDiseaseTwoYear.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnDiseaseTwoYear_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiseaseOccuTwo.Focus();
                    txtDiseaseOccuTwo.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiseaseOccuTwo_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnDiseaseOccuTwoYear.Focus();
                    spnDiseaseOccuTwoYear.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnDiseaseOccuTwoYear_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPathologicalHistory.Focus();
                    txtPathologicalHistory.SelectAll();
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
