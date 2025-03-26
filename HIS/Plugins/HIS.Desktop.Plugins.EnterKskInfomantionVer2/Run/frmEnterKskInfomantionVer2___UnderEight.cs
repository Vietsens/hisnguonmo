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
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private HIS_DHST dhstUnderEighteen { get; set; }
        private List<HIS_KSK_UNEI_VATY> lstKskUneiVatyUnderEighteen { get; set; }

        private void FillDataPageUnderEighteen()
        {
            try
            {
                ResetControlUnderEight();
                SetDataCboRank(cboDhstRank3);
                SetDataCboRank(cboExamCirculationRank3);
                SetDataCboRank(cboExamRespiratoryRank3);
                SetDataCboRank(cboExamDigestionRank3);
                SetDataCboRank(cboExamNeuroMental3);
                SetDataCboRank(cboExamKidneyUrologyRank3);
                SetDataCboRank(cboExamEyeRank3);
                SetDataCboRank(cboExamEntDiseaseRank3);
                SetDataCboRank(cboExamStomatologyRank3);
                SetDataCboRank(cboExamClinicalOther3);
                SetDataCboExamLoginName(cboExecuteLoginName3);
                SetDataCboExamLoginName(cboExamCirculationLoginName3);
                SetDataCboExamLoginName(cboExamEyeLoginName3);
                SetDataCboExamLoginName(cboExamEntLoginName3);
                SetDataCboExamLoginName(cboExamSubclinicalLoginName3);
                SetDataCboExamLoginName(cboExamStomatologyLoginName3);
                FillDataUnderEighteen();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataUnderEighteen()
        {
            try
            {
                if (currentServiceReq != null)
                {
                    CommonParam param = new CommonParam();
                    HisKskUnderEighteenFilter filter = new HisKskUnderEighteenFilter();
                    filter.SERVICE_REQ_ID = currentServiceReq.ID;
                    var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_UNDER_EIGHTEEN>>("api/HisKskUnderEighteen/Get", ApiConsumers.MosConsumer, filter, param);
                    if (data != null && data.Count > 0)
                    {
                        currentKskUnderEight = data.First();
                        txtPathologicalHistoryFamily3.Text = currentKskUnderEight.PATHOLOGICAL_HISTORY_FAMILY;
                        txtPathologicalHistory3.Text = currentKskUnderEight.PATHOLOGICAL_HISTORY;
                        txtMedicineUsing3.Text = currentKskUnderEight.MEDICINE_USING;
                        txtMarternityHistory3.Text = currentKskUnderEight.MATERNITY_HISTORY;
                        cboDhstRank3.EditValue = currentKskUnderEight.DHST_RANK;
                        txtExamCirculation3.Text = currentKskUnderEight.EXAM_CIRCULATION;
                        cboExamCirculationRank3.EditValue = currentKskUnderEight.EXAM_CIRCULATION_RANK;
                        txtExamRespiratory3.Text = currentKskUnderEight.EXAM_RESPIRATORY;
                        cboExamRespiratoryRank3.EditValue = currentKskUnderEight.EXAM_RESPIRATORY_RANK;
                        txtExamDigestion3.Text = currentKskUnderEight.EXAM_DIGESTION;
                        cboExamDigestionRank3.EditValue = currentKskUnderEight.EXAM_DIGESTION_RANK;
                        txtExamKidneyUrology3.Text = currentKskUnderEight.EXAM_KIDNEY_UROLOGY;
                        cboExamKidneyUrologyRank3.EditValue = currentKskUnderEight.EXAM_KIDNEY_UROLOGY_RANK;
                        txtExamNeuroMental3.Text = currentKskUnderEight.EXAM_NEURO_MENTAL;
                        cboExamNeuroMental3.EditValue = currentKskUnderEight.EXAM_NEURO_MENTAL_RANK;
                        txtExamClinicalOther3.Text = currentKskUnderEight.EXAM_CLINICAL_OTHER;
                        cboExamClinicalOther3.EditValue = currentKskUnderEight.EXAM_CLINICAL_OTHER_RANK;

                        txtExamEyeSightRight3.Text = currentKskUnderEight.EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft3.Text = currentKskUnderEight.EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight3.Text = currentKskUnderEight.EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft3.Text = currentKskUnderEight.EXAM_EYESIGHT_GLASS_LEFT;
                        txtExamEyeDisease3.Text = currentKskUnderEight.EXAM_EYE_DISEASE;
                        cboExamEyeRank3.EditValue = currentKskUnderEight.EXAM_EYE_RANK;
                        txtExamEntLeftNormal3.Text = currentKskUnderEight.EXAM_ENT_LEFT_NORMAL;
                        txtExamEntLeftWhisper3.Text = currentKskUnderEight.EXAM_ENT_LEFT_WHISPER;
                        txtExamEntRightNomal3.Text = currentKskUnderEight.EXAM_ENT_RIGHT_NORMAL;
                        txtExamEntRightWhisper3.Text = currentKskUnderEight.EXAM_ENT_RIGHT_WHISPER;
                        txtExamEntDisease3.Text = currentKskUnderEight.EXAM_ENT_DISEASE;
                        cboExamEntDiseaseRank3.EditValue = currentKskUnderEight.EXAM_ENT_RANK;
                        txtExamStomatologyUpper3.Text = currentKskUnderEight.EXAM_STOMATOLOGY_UPPER;
                        txtExamStomatologyLower3.Text = currentKskUnderEight.EXAM_STOMATOLOGY_LOWER;
                        txtExamStomatologyDisease3.Text = currentKskUnderEight.EXAM_STOMATOLOGY_DISEASE;
                        cboExamStomatologyRank3.EditValue = currentKskUnderEight.EXAM_STOMATOLOGY_RANK;
                        txtResultSubclinical3.Text = currentKskUnderEight.RESULT_SUBCLINICAL;
                        txtNormalHealth3.Text = currentKskUnderEight.NORMAL_HEALTH;
                        txtProblemHealth3.Text = currentKskUnderEight.PROBLEM_HEALTH;
                        HisKskUneiVatyFilter vatyfilter = new HisKskUneiVatyFilter();
                        vatyfilter.KSK_UNDER_EIGHTEEN_ID = currentKskUnderEight.ID;
                        lstDataUneiVaty = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_UNEI_VATY>>("api/HisKskUneiVaty/Get", ApiConsumers.MosConsumer, vatyfilter, param);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstDataUneiVaty), lstDataUneiVaty));
                      
                        if (lstDataUneiVaty != null && lstDataUneiVaty.Count > 0)
                        {
                            HisVaccineTypeFilter vacfilter = new HisVaccineTypeFilter();
                            vacfilter.IS_ACTIVE = 1;
                            vacfilter.IDs = lstDataUneiVaty.Select(o => o.VACCINE_TYPE_ID).ToList();
                            var dataVacine = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_VACCINE_TYPE>>("api/HisVaccineType/Get", ApiConsumers.MosConsumer, vacfilter, param);
                            if (dataVacine != null && dataVacine.Count > 0)
                            {
                                dataVacine = dataVacine.OrderBy(o => o.VACCINE_TYPE_CODE).ToList();
                                List<ADO.VaccineTypeADO> lstAdo = new List<ADO.VaccineTypeADO>();
                                foreach (var item in dataVacine)
                                {
                                    ADO.VaccineTypeADO ado = new ADO.VaccineTypeADO();
                                    ado.ID = item.ID;
                                    ado.VACCINE_TYPE_NAME = item.VACCINE_TYPE_NAME;
                                    var check = lstDataUneiVaty.Where(o => o.VACCINE_TYPE_ID == item.ID).FirstOrDefault();
                                    ado.UNEI_VATY_ID = check.ID;
                                    var stt = check.CONDITION_TYPE;
                                    if (stt == 1)
                                    {
                                        ado.IS_YES = true;
                                    }
                                    else if (stt == 2)
                                    {
                                        ado.IS_NO = true;
                                    }
                                    else if (stt == 3)
                                    {
                                        ado.IS_FORGOT = true;
                                    }
                                    lstAdo.Add(ado);
                                }
                                gridControl1.DataSource = new List<ADO.VaccineTypeADO>();
                                gridControl1.DataSource = lstAdo;
                            }

                        }                       
                        else
                        {
                            SetDafaultGrid();
                        }

                        if (currentKskUnderEight.DHST_ID != null && currentKskUnderEight.DHST_ID > 0)
                        {
                            HisDhstFilter dhstFilter = new HisDhstFilter();
                            dhstFilter.ID = currentKskUnderEight.DHST_ID;
                            var dataDhst = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                            if (dataDhst != null && dataDhst.Count > 0)
                            {
                                dhstUnderEighteen = dataDhst.First();
                                spnHeight3.EditValue = dhstUnderEighteen.HEIGHT;
                                spnPulse3.EditValue = dhstUnderEighteen.PULSE;
                                spnWeight3.EditValue = dhstUnderEighteen.WEIGHT;
                                spnBloodPressureMax3.EditValue = dhstUnderEighteen.BLOOD_PRESSURE_MAX;
                                spnBloodPressureMin3.EditValue = dhstUnderEighteen.BLOOD_PRESSURE_MIN;
                                //txtVirBmi.Text = currentDhst.VIR_BMI!=null ? currentDhst.VIR_BMI.ToString() : "";
                                FillNoteBMI(spnHeight3, spnWeight3, txtVirBmi3);
                                cboExecuteLoginName3.EditValue = dhstUnderEighteen.EXECUTE_LOGINNAME;
                            }
                        }
                        cboExamCirculationLoginName3.EditValue = currentKskGeneral.EXAM_CIRCULATION_LOGINNAME;
                        cboExamEyeLoginName3.EditValue = currentKskGeneral.EXAM_EYE_LOGINNAME;
                        cboExamEntLoginName3.EditValue = currentKskGeneral.EXAM_ENT_LOGINNAME;
                        cboExamStomatologyLoginName3.EditValue = currentKskGeneral.EXAM_STOMATOLOGY_LOGINNAME;
                        cboExamSubclinicalLoginName3.EditValue = currentKskGeneral.EXAM_SUBCLINICAL_LOGINNAME;
                    }
                    else
                    {
                        SetDafaultGrid();
                        txtPathologicalHistoryFamily3.Text = currentServiceReq.PATHOLOGICAL_HISTORY_FAMILY;
                        txtMarternityHistory3.Text = currentServiceReq.PATHOLOGICAL_HISTORY;
                        txtExamCirculation3.Text = currentServiceReq.PART_EXAM_CIRCULATION;
                        txtExamRespiratory3.Text = currentServiceReq.PART_EXAM_RESPIRATORY;
                        txtExamDigestion3.Text = currentServiceReq.PART_EXAM_DIGESTION;
                        txtExamKidneyUrology3.Text = currentServiceReq.PART_EXAM_KIDNEY_UROLOGY;
                        txtExamNeuroMental3.Text = "";
                        if (!string.IsNullOrEmpty(currentServiceReq.PART_EXAM_NEUROLOGICAL))
                            txtExamNeuroMental3.Text = currentServiceReq.PART_EXAM_NEUROLOGICAL;
                        if (!string.IsNullOrEmpty(currentServiceReq.PART_EXAM_MENTAL))
                            txtExamNeuroMental3.Text += " - " + currentServiceReq.PART_EXAM_MENTAL;

                        txtExamEyeSightRight3.Text = currentServiceReq.PART_EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft3.Text = currentServiceReq.PART_EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight3.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft3.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_LEFT;

                        txtExamEntLeftNormal3.Text = currentServiceReq.PART_EXAM_EAR_LEFT_NORMAL;
                        txtExamEntLeftWhisper3.Text = currentServiceReq.PART_EXAM_EAR_LEFT_WHISPER;
                        txtExamEntRightNomal3.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_NORMAL;
                        txtExamEntRightWhisper3.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_WHISPER;

                        txtExamStomatologyUpper3.Text = currentServiceReq.PART_EXAM_UPPER_JAW;
                        txtExamStomatologyLower3.Text = currentServiceReq.PART_EXAM_LOWER_JAW;
                        txtResultSubclinical3.Text = currentServiceReq.SUBCLINICAL;

                        if (currentServiceReq.DHST_ID != null && currentServiceReq.DHST_ID > 0)
                        {
                            HisDhstFilter dhstFilter = new HisDhstFilter();
                            dhstFilter.ID = currentServiceReq.DHST_ID;
                            var dataDhst = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                            if (dataDhst != null && dataDhst.Count > 0)
                            {
                                var currentDhst = dataDhst.First();
                                spnHeight3.EditValue = currentDhst.HEIGHT;
                                spnPulse3.EditValue = currentDhst.PULSE;
                                spnWeight3.EditValue = currentDhst.WEIGHT;
                                spnBloodPressureMax3.EditValue = currentDhst.BLOOD_PRESSURE_MAX;
                                spnBloodPressureMin3.EditValue = currentDhst.BLOOD_PRESSURE_MIN;
                                //txtVirBmi.Text = currentDhst.VIR_BMI!=null ? currentDhst.VIR_BMI.ToString() : "";
                                FillNoteBMI(spnHeight3, spnWeight3, txtVirBmi3);
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


        private void ResetControlUnderEight()
        {
            try
            {
                spnHeight3.EditValue = null;
                spnPulse3.EditValue = null;
                spnWeight3.EditValue = null;
                spnBloodPressureMax3.EditValue = null;
                spnBloodPressureMin3.EditValue = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnHeight3_EditValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                FillNoteBMI(spnHeight3, spnWeight3, txtVirBmi3);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnWeight3_EditValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                FillNoteBMI(spnHeight3, spnWeight3, txtVirBmi3);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (VaccineTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void repositoryItemCheckEdit1_Click(object sender, System.EventArgs e)
        {
            try
            {
                var focusRow = (ADO.VaccineTypeADO)gridView1.GetFocusedRow();
                if (!focusRow.IS_YES)
                {
                    focusRow.IS_YES = true;
                    if (focusRow.IS_NO)
                    {
                        focusRow.IS_NO = false;
                    }
                    if (focusRow.IS_FORGOT)
                    {
                        focusRow.IS_FORGOT = false;
                    }
                }
                else
                {
                    focusRow.IS_YES = false;
                }
                ReloadGrid(focusRow);

            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEdit2_Click(object sender, System.EventArgs e)
        {
            try
            {
                var focusRow = (ADO.VaccineTypeADO)gridView1.GetFocusedRow();
                if (!focusRow.IS_NO)
                {
                    focusRow.IS_NO = true;
                    if (focusRow.IS_YES)
                    {
                        focusRow.IS_YES = false;
                    }
                    if (focusRow.IS_FORGOT)
                    {
                        focusRow.IS_FORGOT = false;
                    }
                }
                else
                {
                    focusRow.IS_NO = false;
                }
                ReloadGrid(focusRow);

            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEdit3_Click(object sender, System.EventArgs e)
        {
            try
            {
                var focusRow = (ADO.VaccineTypeADO)gridView1.GetFocusedRow();
                if (!focusRow.IS_FORGOT)
                {
                    focusRow.IS_FORGOT = true;
                    if (focusRow.IS_NO)
                    {
                        focusRow.IS_NO = false;
                    }
                    if (focusRow.IS_YES)
                    {
                        focusRow.IS_YES = false;
                    }
                }
                else
                {
                    focusRow.IS_FORGOT = false;
                }
                ReloadGrid(focusRow);

            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadGrid(VaccineTypeADO focusRow)
        {
            try
            {
                var Alls = gridControl1.DataSource as List<ADO.VaccineTypeADO>;
                int count = 0;
                foreach (var item in Alls)
                {
                    if (item.ID == focusRow.ID)
                    {
                        item.IS_YES = focusRow.IS_YES;
                        item.IS_NO = focusRow.IS_NO;
                        item.IS_FORGOT = focusRow.IS_FORGOT;
                        break;
                    }
                    count++;
                }
                gridControl1.DataSource = new List<ADO.VaccineTypeADO>();
                gridControl1.DataSource = Alls;
                gridView1.FocusedRowHandle = count;
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDafaultGrid()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisVaccineTypeFilter vacfilter = new HisVaccineTypeFilter();
                vacfilter.IS_ACTIVE = 1;
                var dataVacine = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_VACCINE_TYPE>>("api/HisVaccineType/Get", ApiConsumers.MosConsumer, vacfilter, param);
                if (dataVacine != null && dataVacine.Count > 0)
                {
                    dataVacine = dataVacine.Where(o => o.IS_USE_FOR_KSK == (short?)1).OrderBy(o => o.VACCINE_TYPE_CODE).ToList();
                    List<ADO.VaccineTypeADO> lstAdo = new List<ADO.VaccineTypeADO>();
                    foreach (var item in dataVacine)
                    {
                        ADO.VaccineTypeADO ado = new ADO.VaccineTypeADO();
                        ado.ID = item.ID;
                        ado.VACCINE_TYPE_NAME = item.VACCINE_TYPE_NAME;
                        lstAdo.Add(ado);
                    }
                    gridControl1.DataSource = new List<ADO.VaccineTypeADO>();
                    gridControl1.DataSource = lstAdo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_KSK_UNDER_EIGHTEEN GetValueUnderEighteen()
        {
            HIS_KSK_UNDER_EIGHTEEN obj = new HIS_KSK_UNDER_EIGHTEEN();
            try
            {
                if (currentKskUnderEight != null)
                    obj.ID = currentKskUnderEight.ID;
                obj.PATHOLOGICAL_HISTORY_FAMILY = txtPathologicalHistoryFamily3.Text;
                obj.PATHOLOGICAL_HISTORY = txtPathologicalHistory3.Text;
                obj.MEDICINE_USING = txtMedicineUsing3.Text;
                obj.MATERNITY_HISTORY = txtMarternityHistory3.Text;
                //DHST
                obj.DHST_RANK = cboDhstRank3.EditValue != null ? (long?)Int64.Parse(cboDhstRank3.EditValue.ToString()) : null;
                obj.EXAM_CIRCULATION = txtExamCirculation3.Text;
                obj.EXAM_CIRCULATION_RANK = cboExamCirculationRank3.EditValue != null ? (long?)Int64.Parse(cboExamCirculationRank3.EditValue.ToString()) : null;
                obj.EXAM_RESPIRATORY = txtExamRespiratory3.Text;
                obj.EXAM_RESPIRATORY_RANK = cboExamRespiratoryRank3.EditValue != null ? (long?)Int64.Parse(cboExamRespiratoryRank3.EditValue.ToString()) : null;
                obj.EXAM_DIGESTION = txtExamDigestion3.Text;
                obj.EXAM_DIGESTION_RANK = cboExamDigestionRank3.EditValue != null ? (long?)Int64.Parse(cboExamDigestionRank3.EditValue.ToString()) : null;
                obj.EXAM_KIDNEY_UROLOGY = txtExamKidneyUrology3.Text;
                obj.EXAM_KIDNEY_UROLOGY_RANK = cboExamKidneyUrologyRank3.EditValue != null ? (long?)Int64.Parse(cboExamKidneyUrologyRank3.EditValue.ToString()) : null;
                obj.EXAM_NEURO_MENTAL = txtExamNeuroMental3.Text;
                obj.EXAM_NEURO_MENTAL_RANK = cboExamNeuroMental3.EditValue != null ? (long?)Int64.Parse(cboExamNeuroMental3.EditValue.ToString()) : null;
                obj.EXAM_CLINICAL_OTHER = txtExamClinicalOther3.Text;
                obj.EXAM_CLINICAL_OTHER_RANK = cboExamClinicalOther3.EditValue != null ? (long?)Int64.Parse(cboExamClinicalOther3.EditValue.ToString()) : null;
                obj.EXAM_EYESIGHT_RIGHT = txtExamEyeSightRight3.Text;
                obj.EXAM_EYESIGHT_LEFT = txtExamEyeSightLeft3.Text;
                obj.EXAM_EYESIGHT_GLASS_RIGHT = txtExamEyeSightGlassRight3.Text;
                obj.EXAM_EYESIGHT_GLASS_LEFT = txtExamEyeSightGlassLeft3.Text;
                obj.EXAM_EYE_DISEASE = txtExamEyeDisease3.Text;
                obj.EXAM_EYE_RANK = cboExamEyeRank3.EditValue != null ? (long?)Int64.Parse(cboExamEyeRank3.EditValue.ToString()) : null;
                obj.EXAM_ENT_LEFT_NORMAL = txtExamEntLeftNormal3.Text;
                obj.EXAM_ENT_LEFT_WHISPER = txtExamEntLeftWhisper3.Text;
                obj.EXAM_ENT_RIGHT_NORMAL = txtExamEntRightNomal3.Text;
                obj.EXAM_ENT_RIGHT_WHISPER = txtExamEntRightWhisper3.Text;
                obj.EXAM_ENT_DISEASE = txtExamEntDisease3.Text;
                obj.EXAM_ENT_RANK = cboExamEntDiseaseRank3.EditValue != null ? (long?)Int64.Parse(cboExamEntDiseaseRank3.EditValue.ToString()) : null;
                obj.EXAM_STOMATOLOGY_UPPER = txtExamStomatologyUpper3.Text;
                obj.EXAM_STOMATOLOGY_LOWER = txtExamStomatologyLower3.Text;
                obj.EXAM_STOMATOLOGY_DISEASE = txtExamStomatologyDisease3.Text;
                obj.EXAM_STOMATOLOGY_RANK = cboExamStomatologyRank3.EditValue != null ? (long?)Int64.Parse(cboExamStomatologyRank3.EditValue.ToString()) : null;
                obj.RESULT_SUBCLINICAL = txtResultSubclinical3.Text;
                obj.NORMAL_HEALTH = txtNormalHealth3.Text;
                obj.PROBLEM_HEALTH = txtProblemHealth3.Text;


                obj.EXAM_CIRCULATION_LOGINNAME = cboExamCirculationLoginName3.EditValue != null ? cboExamCirculationLoginName3.EditValue.ToString() : null;
                obj.EXAM_EYE_LOGINNAME = cboExamEyeLoginName3.EditValue != null ? cboExamEyeLoginName3.EditValue.ToString() : null;
                obj.EXAM_ENT_LOGINNAME = cboExamEntLoginName3.EditValue != null ? cboExamEntLoginName3.EditValue.ToString() : null;
                obj.EXAM_STOMATOLOGY_LOGINNAME = cboExamStomatologyLoginName3.EditValue != null ? cboExamStomatologyLoginName3.EditValue.ToString() : null;
                obj.EXAM_SUBCLINICAL_LOGINNAME = cboExamSubclinicalLoginName3.EditValue != null ? cboExamSubclinicalLoginName3.EditValue.ToString() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }

        private HIS_DHST GetDhstUnderEighteen()
        {
            HIS_DHST obj = new HIS_DHST();
            try
            {
                if (dhstUnderEighteen != null)
                    obj.ID = dhstUnderEighteen.ID;
                if (spnBloodPressureMax3.EditValue != null)
                    obj.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spnBloodPressureMax3.Value.ToString());
                if (spnBloodPressureMin3.EditValue != null)
                    obj.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spnBloodPressureMin3.Value.ToString());
                if (spnHeight3.EditValue != null)
                    obj.HEIGHT = Inventec.Common.Number.Get.RoundCurrency(spnHeight3.Value, 2);
                if (spnPulse3.EditValue != null)
                    obj.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spnPulse3.Value.ToString());
                if (spnWeight3.EditValue != null)
                    obj.WEIGHT = Inventec.Common.Number.Get.RoundCurrency(spnWeight3.Value, 2);

                obj.EXECUTE_LOGINNAME = cboExecuteLoginName3.EditValue != null ? cboExecuteLoginName3.EditValue.ToString() : null;
                obj.EXECUTE_USERNAME = obj.EXECUTE_LOGINNAME != null ? BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o=>o.LOGINNAME == obj.EXECUTE_LOGINNAME).TDL_USERNAME : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }

        private List<HIS_KSK_UNEI_VATY> GetUneiVaty()
        {
            List<HIS_KSK_UNEI_VATY> obj = new List<HIS_KSK_UNEI_VATY>();
            try
            {
                var Alls = gridControl1.DataSource as List<ADO.VaccineTypeADO>;
                if (Alls != null && Alls.Count > 0)
                {
                    if (currentKskUnderEight != null && lstDataUneiVaty != null && lstDataUneiVaty.Count > 0)
                    {
                        foreach (var item in Alls)
                        {
                            HIS_KSK_UNEI_VATY i = new HIS_KSK_UNEI_VATY();
                            i.ID = item.UNEI_VATY_ID;
                            i.VACCINE_TYPE_ID = item.ID;
                            i.CONDITION_TYPE = null;
                            if (item.IS_YES) i.CONDITION_TYPE = 1;
                            if (item.IS_NO) i.CONDITION_TYPE = 2;
                            if (item.IS_FORGOT) i.CONDITION_TYPE = 3;
                            obj.Add(i);
                        }
                    }
                    else
                    {
                        foreach (var item in Alls)
                        {
                            HIS_KSK_UNEI_VATY i = new HIS_KSK_UNEI_VATY();
                            i.VACCINE_TYPE_ID = item.ID;
                            i.CONDITION_TYPE = null;
                            if (item.IS_YES) i.CONDITION_TYPE = 1;
                            if (item.IS_NO) i.CONDITION_TYPE = 2;
                            if (item.IS_FORGOT) i.CONDITION_TYPE = 3;
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
        private void txtResultSubclinical3_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameSItem = ENameSItem.KET_QUA_3;
            GetSpecInformation(ReturnObject = false);
        }

        #region ---PREVIEWKEYDOWN----











        private void txtPathologicalHistoryFamily3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMarternityHistory3.Focus();
                    txtMarternityHistory3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMarternityHistory3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPathologicalHistory3.Focus();
                    txtPathologicalHistory3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPathologicalHistory3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineUsing3.Focus();
                    txtMedicineUsing3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineUsing3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnHeight3.Focus();
                    spnHeight3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnHeight3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnWeight3.Focus();
                    spnWeight3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnWeight3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnPulse3.Focus();
                    spnPulse3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnPulse3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnBloodPressureMax3.Focus();
                    spnBloodPressureMax3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnBloodPressureMax3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnBloodPressureMin3.Focus();
                    spnBloodPressureMin3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnBloodPressureMin3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDhstRank3.Focus();
                    cboDhstRank3.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDhstRank3_Closed(object sender, ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamCirculation3.Focus();
                    txtExamCirculation3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamCirculation3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamCirculationRank3.Focus();
                    cboExamCirculationRank3.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamCirculationRank3_Closed(object sender, ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamRespiratory3.Focus();
                    txtExamRespiratory3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamRespiratory3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamRespiratoryRank3.Focus();
                    cboExamRespiratoryRank3.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamRespiratoryRank3_Closed(object sender, ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamDigestion3.Focus();
                    txtExamDigestion3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamDigestion3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamDigestionRank3.Focus();
                    cboExamDigestionRank3.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamDigestionRank3_Closed(object sender, ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamKidneyUrology3.Focus();
                    txtExamKidneyUrology3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamKidneyUrology3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamKidneyUrologyRank3.Focus();
                    cboExamKidneyUrologyRank3.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamKidneyUrologyRank3_Closed(object sender, ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamNeuroMental3.Focus();
                    txtExamNeuroMental3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamNeuroMental3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamNeuroMental3.Focus();
                    cboExamNeuroMental3.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamNeuroMental3_Closed(object sender, ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamClinicalOther3.Focus();
                    txtExamClinicalOther3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamClinicalOther3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamClinicalOther3.Focus();
                    cboExamClinicalOther3.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamClinicalOther3_Closed(object sender, ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEyeSightRight3.Focus();
                    txtExamEyeSightRight3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightRight3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightLeft3.Focus();
                    txtExamEyeSightLeft3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightLeft3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassRight3.Focus();
                    txtExamEyeSightGlassRight3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassRight3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassLeft3.Focus();
                    txtExamEyeSightGlassLeft3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassLeft3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeDisease3.Focus();
                    txtExamEyeDisease3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftNormal3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntLeftWhisper3.Focus();
                    txtExamEntLeftWhisper3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftWhisper3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightNomal3.Focus();
                    txtExamEntRightNomal3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightNomal3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightWhisper3.Focus();
                    txtExamEntRightWhisper3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightWhisper3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntDisease3.Focus();
                    txtExamEntDisease3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeDisease3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEyeRank3.Focus();
                    cboExamEyeRank3.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEyeRank3_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEntLeftNormal3.Focus();
                    txtExamEntLeftNormal3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntDisease3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEntDiseaseRank3.Focus();
                    cboExamEntDiseaseRank3.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEntDiseaseRank3_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamStomatologyUpper3.Focus();
                    txtExamStomatologyUpper3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyUpper3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamStomatologyLower3.Focus();
                    txtExamStomatologyLower3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyLower3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamStomatologyDisease3.Focus();
                    txtExamStomatologyDisease3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyDisease3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamStomatologyRank3.Focus();
                    cboExamStomatologyRank3.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamStomatologyRank3_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtResultSubclinical3.Focus();
                    txtResultSubclinical3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtResultSubclinical3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNormalHealth3.Focus();
                    txtNormalHealth3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNormalHealth3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtProblemHealth3.Focus();
                    txtProblemHealth3.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProblemHealth3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
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
