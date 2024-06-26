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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.EnterKskInfomantion.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EnterKskInfomantion
{
    partial class frmEnterKskInfomantion
    {
        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnSave.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.SDO.HisServiceReqKskExecuteSDO updateDTO = new MOS.SDO.HisServiceReqKskExecuteSDO();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                var resultData = new BackendAdapter(param).Post<MOS.SDO.KskExecuteResultSDO>(HisRequestUriStore.MOS_HIS_SERVICE_REQ_KSK_EXECUTE, ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                    var dataSource = (gridViewServiceReq.DataSource as List<ServiceReqADO>);
                    if (dataSource != null && dataSource.Count() > 0)
                    {
                        ChangedDataRow(dataSource.FirstOrDefault(o => o.ID == resultData.HisServiceReq.ID));
                    }
                }

                if (success)
                {
                    EnableControlChanged(this.currentServiceReqSTT);
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(ADO.ServiceReqADO currentData, ref MOS.SDO.HisServiceReqKskExecuteSDO currentDTO)
        {
            try
            {
                currentDTO.ServiceReqId = currentData.ID;
                currentDTO.isFinish = chkIsFinish.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.SDO.HisServiceReqKskExecuteSDO currentDTO)
        {
            try
            {
                currentDTO.RequestRoomId = this.moduleData.RoomId;
                //Tab Ket luan
                currentDTO.Conclusion = txtConclusionTab2.Text.Trim();
                currentDTO.ConclusionClinical = txtConclusionClinicalTab2.Text.Trim();
                currentDTO.ConclusionConsultation = txtConclusionConsultationTab2.Text.Trim();
                currentDTO.ConclusionSubclinical = txtConclusionSubclinicalTab2.Text.Trim();
                currentDTO.ExamConclusion = txtExamConclusionTab2.Text.Trim();
                currentDTO.OccupationalDisease = txtOccupationalDiseaseTab2.Text.Trim();
                currentDTO.ProvisionalDiagnosis = txtProvisionalDiagnosisTab2.Text.Trim();
                //Tab Kham chung
                MOS.SDO.HisKskGeneralSDO kskGeneral = new MOS.SDO.HisKskGeneralSDO();
                HIS_DHST dhstGeneral = new HIS_DHST();
                if (txtHeightTab1.EditValue != null)
                    dhstGeneral.HEIGHT = txtHeightTab1.Value;
                if (txtWeightTab1.EditValue != null)
                    dhstGeneral.WEIGHT = txtWeightTab1.Value;

                var user = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME.ToUpper() == cboBSketluan.EditValue.ToString().ToUpper());
                if (user != null)
                {
                    kskGeneral.ConcluderLoginName = user.LOGINNAME;
                    kskGeneral.ConcluderUserName = user.USERNAME;
                }
                if (cboDayResult.EditValue != null)
                {
                    kskGeneral.ConclusionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(cboDayResult.DateTime);
                }
                else
                {
                    kskGeneral.ConclusionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                }
                try
                {
                    Decimal n;
                    bool isNumeric = Decimal.TryParse(lblBMITab1.Text, out n);
                    if (isNumeric)
                    {
                        dhstGeneral.VIR_BMI = n;
                    }
                    else
                    {
                        dhstGeneral.VIR_BMI = null;
                    }
                }

                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    Inventec.Common.Logging.LogSystem.Warn("ERROR: dhstGeneral.VIR_BMI");
                }
                if (txtPulseTab1.EditValue != null)
                    dhstGeneral.PULSE = (long)(txtPulseTab1.Value);
                if (txtBloodPressureMaxTab1.EditValue != null)
                    dhstGeneral.BLOOD_PRESSURE_MAX = (long)(txtBloodPressureMaxTab1.Value);
                if (txtBloodPressureMinTab1.EditValue != null)
                    dhstGeneral.BLOOD_PRESSURE_MIN = (long)(txtBloodPressureMinTab1.Value);
                if (txtTemperatureTab1.EditValue != null)
                    dhstGeneral.TEMPERATURE = txtTemperatureTab1.Value;
                if (txtBreathRateTab1.EditValue != null)
                    dhstGeneral.BREATH_RATE = txtBreathRateTab1.Value;

                kskGeneral.HisDhst = dhstGeneral;

                kskGeneral.ExamCirculation = txtExamCirculationTab1.Text.Trim();
                if (cboExamCirculationRankTab1.EditValue != null)
                    kskGeneral.ExamCirculationRank = (long)cboExamCirculationRankTab1.EditValue;
                kskGeneral.ExamDermatology = txtExamDermatologyTab1.Text.Trim();
                if (cboExamDermatologyRankTab1.EditValue != null)
                    kskGeneral.ExamDermatologyRank = (long)cboExamDermatologyRankTab1.EditValue;
                kskGeneral.ExamDigestion = txtExamDigestionTab1.Text.Trim();
                if (cboExamDigestionRankTab1.EditValue != null)
                    kskGeneral.ExamDigestionRank = (long)cboExamDigestionRankTab1.EditValue;
                kskGeneral.ExamEnt = txtExamENTTab1.Text.Trim();
                if (cboExamENTRankTab1.EditValue != null)
                    kskGeneral.ExamEntRank = (long)cboExamENTRankTab1.EditValue;
                kskGeneral.ExamEye = txtEyeTab1.Text.Trim();
                if (cboEyeRankTab1.EditValue != null)
                    kskGeneral.ExamEyeRank = (long)cboEyeRankTab1.EditValue;
                kskGeneral.ExamKidneyUrology = txtExamKidneyUrologyTab1.Text.Trim();
                if (cboExamKidneyUrologyRankTab1.EditValue != null)
                    kskGeneral.ExamKidneyUrologyRank = (long)cboExamKidneyUrologyRankTab1.EditValue;
                kskGeneral.ExamMental = txtMentalTab1.Text.Trim();
                if (cboMentalRankTab1.EditValue != null)
                    kskGeneral.ExamMentalRank = (long)cboMentalRankTab1.EditValue;
                kskGeneral.ExamMuscleBone = txtExamMuscleBoneTab1.Text.Trim();
                if (cboExamMuscleBoneRankTab1.EditValue != null)
                    kskGeneral.ExamMuscleBoneRank = (long)cboExamMuscleBoneRankTab1.EditValue;
                kskGeneral.ExamNeurological = txtExamNeurologicalTab1.Text.Trim();
                if (cboExamNeurologicalRankTab1.EditValue != null)
                    kskGeneral.ExamNeurologicalRank = (long)cboExamNeurologicalRankTab1.EditValue;
                kskGeneral.ExamOend = txtExamOENDTab1.Text.Trim();
                if (cboExamOENDRankTab1.EditValue != null)
                    kskGeneral.ExamOendRank = (long)cboExamOENDRankTab1.EditValue;
                kskGeneral.ExamRespiratory = txtExamRepiratoryTab1.Text.Trim();
                if (cboExamRepiratoryRankTab1.EditValue != null)
                    kskGeneral.ExamRespiratoryRank = (long)cboExamRepiratoryRankTab1.EditValue;
                kskGeneral.ExamStomatology = txtExamStomatologyTab1.Text.Trim();
                if (cboExamStomatologyRankTab1.EditValue != null)
                    kskGeneral.ExamStomatologyRank = (long)cboExamStomatologyRankTab1.EditValue;
                kskGeneral.ExamSurgery = txtSurgeryTab1.Text.Trim();
                if (cboSurgeryRankTab1.EditValue != null)
                    kskGeneral.ExamSurgeryRank = (long)cboSurgeryRankTab1.EditValue;

                kskGeneral.NoteDiim = memCDHA.Text.Trim();
                kskGeneral.NoteBlood = memNoteBlood.Text.Trim();
                kskGeneral.NoteTestUrine = memNoteTestUre.Text.Trim();
                kskGeneral.NoteTestOther = memNoteTestOth.Text.Trim();

                if (cboPLSucKhoeTab1.EditValue != null)
                    kskGeneral.HealthExamRankId = (long)cboPLSucKhoeTab1.EditValue;

                kskGeneral.Diseases = txtDiseasesTab1.Text.Trim();
                kskGeneral.TreatmentInstruction = txtTreatmentInstructionTab1.Text.Trim();


                kskGeneral.ExamObstetric = txtExamObstetic.Text.Trim();
                kskGeneral.ExamOccupationalTherapy = txtExamOccupationalTherapy.Text.Trim();
                kskGeneral.ExamTraditional = txtExamTraditional.Text.Trim();
                kskGeneral.ExamNutrion = txtExamNutrion.Text.Trim();

                kskGeneral.ExamObstetricRank = cboExamObsteticRank.EditValue != null ? (long?)cboExamObsteticRank.EditValue : null;
                kskGeneral.ExamOccupationalTherapyRank = cboExamOccupationalTherapyRank.EditValue != null ? (long?)cboExamOccupationalTherapyRank.EditValue : null;
                kskGeneral.ExamTraditionalRank = cboExamTraditionalRank.EditValue != null ? (long?)cboExamTraditionalRank.EditValue : null;
                kskGeneral.ExamNutrionRank = cboExamNutrionRank.EditValue != null ? (long?)cboExamNutrionRank.EditValue : null;
                kskGeneral.HeinMediOrgCode = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId()).HEIN_MEDI_ORG_CODE;
                currentDTO.KskGeneral = kskGeneral;
                //Tab Benh nghe nghiep
                MOS.SDO.HisKskOccupationalSDO kskOccupational = new MOS.SDO.HisKskOccupationalSDO();
                HIS_DHST dhstOccupational = new HIS_DHST();
                if (txtHeightTab3.EditValue != null)
                    dhstOccupational.HEIGHT = txtHeightTab3.Value;
                if (txtWeightTab3.EditValue != null)
                    dhstOccupational.WEIGHT = txtWeightTab3.Value;
                try
                {
                    Decimal n;
                    bool isNumeric = Decimal.TryParse(lblBMITab3.Text, out n);
                    if (isNumeric)
                    {
                        dhstOccupational.VIR_BMI = n;
                    }
                    else
                    {
                        dhstOccupational.VIR_BMI = null;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    Inventec.Common.Logging.LogSystem.Warn("ERROR: dhstOccupational.VIR_BMI");
                }
                if (txtPulseTab3.EditValue != null)
                    dhstOccupational.PULSE = (long)(txtPulseTab3.Value);
                if (txtBloodPressureMaxTab3.EditValue != null)
                    dhstOccupational.BLOOD_PRESSURE_MAX = (long)(txtBloodPressureMaxTab3.Value);
                if (txtBloodPressureMinTab3.EditValue != null)
                    dhstOccupational.BLOOD_PRESSURE_MIN = (long)(txtBloodPressureMinTab3.Value);
                if (txtTemperatureTab3.EditValue != null)
                    dhstOccupational.TEMPERATURE = txtTemperatureTab3.Value;
                if (txtBreathRateTab3.EditValue != null)
                    dhstOccupational.BREATH_RATE = txtBreathRateTab3.Value;

                kskOccupational.HisDhst = dhstOccupational;

                kskOccupational.ExamCirculation = txtExamCirculationTab3.Text.Trim();
                if (cboExamCirculationRankTab3.EditValue != null)
                    kskOccupational.ExamCirculationRank = (long)cboExamCirculationRankTab3.EditValue;
                kskOccupational.ExamDermatology = txtExamDermatologyTab3.Text.Trim();
                if (cboExamDermatologyRankTab3.EditValue != null)
                    kskOccupational.ExamDermatologyRank = (long)cboExamDermatologyRankTab3.EditValue;
                kskOccupational.ExamDigestion = txtExamDigestionTab3.Text.Trim();
                if (cboExamDigestionRankTab3.EditValue != null)
                    kskOccupational.ExamDigestionRank = (long)cboExamDigestionRankTab3.EditValue;
                kskOccupational.ExamEnt = txtExamENTTab3.Text.Trim();
                if (cboExamENTRankTab3.EditValue != null)
                    kskOccupational.ExamEntRank = (long)cboExamENTRankTab3.EditValue;
                kskOccupational.ExamEye = txtEyeTab3.Text.Trim();
                if (cboEyeRankTab3.EditValue != null)
                    kskOccupational.ExamEyeRank = (long)cboEyeRankTab3.EditValue;
                kskOccupational.ExamKidneyUrology = txtExamKidneyUrologyTab3.Text.Trim();
                if (cboExamKidneyUrologyRankTab3.EditValue != null)
                    kskOccupational.ExamKidneyUrologyRank = (long)cboExamKidneyUrologyRankTab3.EditValue;
                kskOccupational.ExamMental = txtMentalTab3.Text.Trim();
                if (cboMentalRankTab3.EditValue != null)
                    kskOccupational.ExamMentalRank = (long)cboMentalRankTab3.EditValue;
                kskOccupational.ExamMuscleBone = txtExamMuscleBoneTab3.Text.Trim();
                if (cboExamMuscleBoneRankTab3.EditValue != null)
                    kskOccupational.ExamMuscleBoneRank = (long)cboExamMuscleBoneRankTab3.EditValue;
                kskOccupational.ExamNeurological = txtExamNeurologicalTab3.Text.Trim();
                if (cboExamNeurologicalRankTab3.EditValue != null)
                    kskOccupational.ExamNeurologicalRank = (long)cboExamNeurologicalRankTab3.EditValue;
                kskOccupational.ExamOend = txtExamOENDTab3.Text.Trim();
                if (cboExamOENDRankTab3.EditValue != null)
                    kskOccupational.ExamOendRank = (long)cboExamOENDRankTab3.EditValue;
                kskOccupational.ExamRespiratory = txtExamRepiratoryTab3.Text.Trim();
                if (cboExamRepiratoryRankTab3.EditValue != null)
                    kskOccupational.ExamRespiratoryRank = (long)cboExamRepiratoryRankTab3.EditValue;
                kskOccupational.ExamStomatology = txtExamStomatologyTab3.Text.Trim();
                if (cboExamStomatologyRankTab3.EditValue != null)
                    kskOccupational.ExamStomatologyRank = (long)cboExamStomatologyRankTab3.EditValue;
                kskOccupational.ExamSurgery = txtSurgeryTab3.Text.Trim();
                if (cboSurgeryRankTab3.EditValue != null)
                    kskOccupational.ExamSurgeryRank = (long)cboSurgeryRankTab3.EditValue;

                kskOccupational.ExamNail = txtExamNailTab3.Text.Trim();
                if (cboExamNailRankTab3.EditValue != null)
                    kskOccupational.ExamNailRank = (long)cboExamNailRankTab3.EditValue;
                kskOccupational.ExamMucosa = txtExamMucosaTab3.Text.Trim();
                if (cboExamMucosaRankTab3.EditValue != null)
                    kskOccupational.ExamMucosaRank = (long)cboExamMucosaRankTab3.EditValue;
                kskOccupational.ExamHematopoietic = txtExamHematopoieticTab3.Text.Trim();
                if (cboExamHematopoieticRankTab3.EditValue != null)
                    kskOccupational.ExamHematopoieticRank = (long)cboExamHematopoieticRankTab3.EditValue;
                kskOccupational.ExamMotion = txtExamMotionTab3.Text.Trim();
                if (cboExamMotionRankTab3.EditValue != null)
                    kskOccupational.ExamMotionRank = (long)cboExamMotionRankTab3.EditValue;
                kskOccupational.ExamCardiovascular = txtExamCardiovascularTab3.Text.Trim();
                if (cboExamCardiovascularRankTab3.EditValue != null)
                    kskOccupational.ExamCardiovascularRank = (long)cboExamCardiovascularRankTab3.EditValue;
                kskOccupational.ExamLymphNodes = txtExamLymphNodesTab3.Text.Trim();
                if (cboExamLymphNodesRankTab3.EditValue != null)
                    kskOccupational.ExamLymphNodesRank = (long)cboExamLymphNodesRankTab3.EditValue;
                kskOccupational.ExamCapillary = txtExamCapillaryTab3.Text.Trim();
                if (cboExamCapillaryRankTab3.EditValue != null)
                    kskOccupational.ExamCapillaryRank = (long)cboExamCapillaryRankTab3.EditValue;

                if (cboPLSucKhoeTab3.EditValue != null)
                    kskOccupational.HealthExamRankId = (long)cboPLSucKhoeTab3.EditValue;
                kskOccupational.Diseases = txtDiseasesTab3.Text.Trim();
                kskOccupational.TreatmentInstruction = txtTreatmentInstructionTab3.Text.Trim();
                kskOccupational.Conclusion = txtConclusionTab3.Text.Trim();

                currentDTO.KskOccupational = kskOccupational;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FinishProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnFinish.Enabled)
                    return;

                positionHandle = -1;

                if (this.currentData == null || this.currentServiceReqSTT == ServiceReqStatus.HoanThanh || this.currentServiceReqSTT == ServiceReqStatus.Default)
                {
                    EnableControlChanged(this.currentServiceReqSTT);
                    return;
                }

                WaitingManager.Show();
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.MOS_HIS_SERVICE_REQ_FINISH, ApiConsumers.MosConsumer, this.currentData.ID, param);
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                }

                if (success)
                {
                    ResetPatientInfoDisplayed();
                    ResetFormData();
                    this.currentData = null;
                    EnableControlChanged(this.currentServiceReqSTT);
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UnfinishProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnUnfinish.Enabled)
                    return;

                positionHandle = -1;

                if (this.currentData == null || this.currentServiceReqSTT != ServiceReqStatus.HoanThanh)
                {
                    EnableControlChanged(this.currentServiceReqSTT);
                    return;
                }

                WaitingManager.Show();
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.MOS_HIS_SERVICE_REQ_UNFINISH, ApiConsumers.MosConsumer, this.currentData.ID, param);
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                }

                if (success)
                {
                    ResetPatientInfoDisplayed();
                    ResetFormData();
                    this.currentData = null;
                    EnableControlChanged(this.currentServiceReqSTT);
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
