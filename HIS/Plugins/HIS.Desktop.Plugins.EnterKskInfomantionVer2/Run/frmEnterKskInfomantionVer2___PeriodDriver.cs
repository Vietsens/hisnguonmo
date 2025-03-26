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
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using Inventec.Common.Controls.EditorLoader;
namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private void FillDataPagePeriodDriver()
        {
            try
            {
                ResetControlPeriodDriver();
                InitComboLicenseClass4();
                SetDataCboRank(cboExamRespiratoryRank4);
                SetDataCboRank(cboNeurologicalRank4);
                SetDataCboRank(cboExamMuscleBoneRank4);
                SetDataCboRank(cboExamOendRank4);
                SetDataCboRank(cboExamMentalRank4);
                SetDataCboRank(cboExamEyeRank4);
                SetDataCboRank(cboExamEntDiseaseRank4);
                SetDataCboRank(cboExamCardiovascularRank4);
                SetDataCboRank(cboExamMaternityRank4);
                SetDataCboExamLoginName(cboExamEyeLoginName4);
                SetDataCboExamLoginName(cboExamEntLoginName4);
                SetDataCboExamLoginName(cboExamCardiovascularLoginName4);
                SetDataCboExamLoginName(cboExamSubclinicalLoginName4);
                FillDataUnderPeriodDriver();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboLicenseClass4()
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_LICENSE_CLASS>(false,true).Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LICENSE_CLASS_NAME", "", 250, 0));
                ControlEditorADO controlEditorADO = new ControlEditorADO("LICENSE_CLASS_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboLicenseClass4, data, controlEditorADO);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetControlPeriodDriver()
        {
            try
            {
                spnExamCardiovascularBloodMax4.EditValue = null;
                spnExamCardiovascularBloodMin4.EditValue = null;
                spnExamCardiovascularPulse4.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataUnderPeriodDriver()
        {
            try
            {
                if (currentServiceReq != null)
                {
                    CommonParam param = new CommonParam();
                    HisKskPeriodDriverFilter filter = new HisKskPeriodDriverFilter();
                    filter.SERVICE_REQ_ID = currentServiceReq.ID;
                    var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_PERIOD_DRIVER>>("api/HisKskPeriodDriver/Get", ApiConsumers.MosConsumer, filter, param);
                    if (data != null && data.Count > 0)
                    {
                        currentKskPeriodDriver = data.First();

                        cboLicenseClass4.EditValue = currentKskPeriodDriver.LICENSE_CLASS_ID;
                        txtPathologicalHistoryFamily4.Text = currentKskPeriodDriver.PATHOLOGICAL_HISTORY_FAMILY;
                        txtPathologicalHistory4.Text = currentKskPeriodDriver.PATHOLOGICAL_HISTORY;
                        txtMedicineUsing4.Text = currentKskPeriodDriver.MEDICINE_USING;
                        txtMaternityHistory4.Text = currentKskPeriodDriver.MATERNITY_HISTORY;
                        txtExamRespiratory4.Text = currentKskPeriodDriver.EXAM_RESPIRATORY;
                        txtExamRespiratoryConclude4.Text = currentKskPeriodDriver.EXAM_RESPIRATORY_CONCLUDE;
                        cboExamRespiratoryRank4.EditValue = currentKskPeriodDriver.EXAM_RESPIRATORY_RANK;
                        txtExamNeurological4.Text = currentKskPeriodDriver.EXAM_NEUROLOGICAL;
                        txtNeurologicalConclude4.Text = currentKskPeriodDriver.EXAM_NEUROLOGICAL_CONCLUDE;
                        cboNeurologicalRank4.EditValue = currentKskPeriodDriver.EXAM_NEUROLOGICAL_RANK;
                        txtExamMuscleBone4.Text = currentKskPeriodDriver.EXAM_MUSCLE_BONE;
                        txtExamMuscleBoneConclude4.Text = currentKskPeriodDriver.EXAM_MUSCLE_BONE_CONCLUDE;
                        cboExamMuscleBoneRank4.EditValue = currentKskPeriodDriver.EXAM_MUSCLE_BONE_RANK;
                        txtExamOend4.Text = currentKskPeriodDriver.EXAM_OEND;
                        txtExamOendConclude4.Text = currentKskPeriodDriver.EXAM_OEND_CONCLUDE;
                        cboExamOendRank4.EditValue = currentKskPeriodDriver.EXAM_OEND_RANK;
                        txtExamMental4.Text = currentKskPeriodDriver.EXAM_MENTAL;
                        txtExamMentalConclude4.Text = currentKskPeriodDriver.EXAM_MENTAL_CONCLUDE;
                        cboExamMentalRank4.EditValue = currentKskPeriodDriver.EXAM_MENTAL_RANK;
                        txtExamMaternity4.Text = currentKskPeriodDriver.EXAM_MATERNITY;
                        txtExamMaternityConclude4.Text = currentKskPeriodDriver.EXAM_MATERNITY_CONCLUDE;
                        cboExamMaternityRank4.EditValue = currentKskPeriodDriver.EXAM_MATERNITY_RANK;
                        txtExamEyeSightRight4.Text = currentKskPeriodDriver.EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft4.Text = currentKskPeriodDriver.EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight4.Text = currentKskPeriodDriver.EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft4.Text = currentKskPeriodDriver.EXAM_EYESIGHT_GLASS_LEFT;
                        txtExamEyeDisease4.Text = currentKskPeriodDriver.EXAM_EYE_DISEASE;
                        cboExamEyeRank4.EditValue = currentKskPeriodDriver.EXAM_EYE_RANK;
                        txtExamEyeConclude4.Text = currentKskPeriodDriver.EXAM_EYE_CONCLUDE;
                        txtExamTwoEyesight4.Text = currentKskPeriodDriver.EXAM_TWO_EYESIGHT;
                        txtExamTwoEyesightGlass4.Text = currentKskPeriodDriver.EXAM_TWO_EYESIGHT_GLASS;
                        txtExamEyeFieldHoriNormal4.Text = currentKskPeriodDriver.EXAM_EYEFIELD_HORI_NORMAL;
                        txtExamEyeFieldHoriLimit4.Text = currentKskPeriodDriver.EXAM_EYEFIELD_HORI_LIMIT;
                        txtExamEyeFieldVertNormal4.Text = currentKskPeriodDriver.EXAM_EYEFIELD_VERT_NORMAL;
                        txtExamEyeFieldVertLimit4.Text = currentKskPeriodDriver.EXAM_EYEFIELD_VERT_LIMIT;
                        chkExamEyeFieldIsNormal4.Checked = currentKskPeriodDriver.EXAM_EYECOLOR_IS_NORMAL == (short?)1 ? true : false;
                        chkExamEyeFieldIsBlind4.Checked = currentKskPeriodDriver.EXAM_EYECOLOR_IS_BLIND == (short?)1 ? true : false;
                        chkExamEyeFieldIsRed4.Checked = currentKskPeriodDriver.EXAM_EYECOLOR_IS_BLIND_RED == (short?)1 ? true : false;
                        chkExamEyeFieldIsGreen4.Checked = currentKskPeriodDriver.EXAM_EYECOLOR_IS_BLIND_GREEN == (short?)1 ? true : false;
                        chkExamEyeFieldIsYellow4.Checked = currentKskPeriodDriver.EXAM_EYECOLOR_IS_BLIND_YELOW == (short?)1 ? true : false;
                        txtExamEntLeftNormal4.Text = currentKskPeriodDriver.EXAM_ENT_LEFT_NORMAL;
                        txtExamEntLeftWhisper4.Text = currentKskPeriodDriver.EXAM_ENT_LEFT_WHISPER;
                        txtExamEntRightNomal4.Text = currentKskPeriodDriver.EXAM_ENT_RIGHT_NORMAL;
                        txtExamEntRightWhisper4.Text = currentKskPeriodDriver.EXAM_ENT_RIGHT_WHISPER;
                        txtExamEntDisease4.Text = currentKskPeriodDriver.EXAM_ENT_DISEASE;
                        txtExamEntConclude4.Text = currentKskPeriodDriver.EXAM_ENT_CONCLUDE;
                        cboExamEntDiseaseRank4.EditValue = currentKskPeriodDriver.EXAM_ENT_RANK;
                        txtExamCardiovascular4.Text = currentKskPeriodDriver.EXAM_CARDIOVASCULAR;
                        txtExamCardiovascularConclude4.Text = currentKskPeriodDriver.EXAM_CARDIOVASCULAR_CONCLUDE;
                        spnExamCardiovascularBloodMax4.EditValue = currentKskPeriodDriver.EXAM_CARDIOVASCULAR_BLOOD_MAX;
                        spnExamCardiovascularBloodMin4.EditValue = currentKskPeriodDriver.EXAM_CARDIOVASCULAR_BLOOD_MIN;
                        spnExamCardiovascularPulse4.EditValue = currentKskPeriodDriver.EXAM_CARDIOVASCULAR_PULSE;
                        cboExamCardiovascularRank4.EditValue = currentKskPeriodDriver.EXAM_CARDIOVASCULAR_RANK;
                        txtMorphineHeroin4.Text = currentKskPeriodDriver.TEST_MORPHIN_HEROIN;
                        txtTestMethamphetamin4.Text = currentKskPeriodDriver.TEST_METHAMPHETAMIN;
                        txtTestMarijuna4.Text = currentKskPeriodDriver.TEST_MARIJUANA;
                        txtTestAmphetamin4.Text = currentKskPeriodDriver.TEST_AMPHETAMIN;
                        txtTestConcentration4.Text = currentKskPeriodDriver.TEST_CONCENTRATION;
                        txtResultSubclinical4.Text = currentKskPeriodDriver.RESULT_SUBCLINICAL;
                        txtNoteSubclinical4.Text = currentKskPeriodDriver.NOTE_SUBCLINICAL;
                        txtConclude4.Text = currentKskPeriodDriver.CONCLUDE;


                        cboExamEyeLoginName4.EditValue = currentKskPeriodDriver.EXAM_EYE_LOGINNAME;
                        cboExamEntLoginName4.EditValue = currentKskPeriodDriver.EXAM_ENT_LOGINNAME;
                        cboExamCardiovascularLoginName4.EditValue = currentKskPeriodDriver.EXAM_CARDIOVASCULAR_LOGINNAME;
                        cboExamSubclinicalLoginName4.EditValue = currentKskPeriodDriver.EXAM_SUBCLINICAL_LOGINNAME;

                        HisPeriodDriverDityFilter dityFilter = new HisPeriodDriverDityFilter();
                        dityFilter.KSK_PERIOD_DRIVER_ID = currentKskPeriodDriver.ID;
                        lstDataDriverDity = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PERIOD_DRIVER_DITY>>("api/HisPeriodDriverDity/Get", ApiConsumers.MosConsumer, dityFilter, param);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstDataDriverDity), lstDataDriverDity));
                        if (lstDataDriverDity != null && lstDataDriverDity.Count > 0)
                        {
                            HisDiseaseTypeFilter Disfilter = new HisDiseaseTypeFilter();
                            Disfilter.IS_ACTIVE = 1;
                            Disfilter.IDs = lstDataDriverDity.Select(o => o.DISEASE_TYPE_ID).ToList();
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
                                    var check = lstDataDriverDity.Where(o => o.DISEASE_TYPE_ID == item.ID).FirstOrDefault();
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
                                gridControl2.DataSource = new List<ADO.DiseaseTypeADO>();
                                gridControl2.DataSource = lstAdo;
                            }
                        }
                        else
                        {
                            SetDefaultGrid();
                        }
                    }
                    else
                    {
                        txtPathologicalHistoryFamily4.Text = currentServiceReq.PATHOLOGICAL_HISTORY_FAMILY;
                        txtPathologicalHistory4.Text = currentServiceReq.PATHOLOGICAL_HISTORY;
                        txtExamRespiratory4.Text = currentServiceReq.PART_EXAM_RESPIRATORY;
                        txtExamNeurological4.Text = currentServiceReq.PART_EXAM_NEUROLOGICAL;
                        txtExamMental4.Text = currentServiceReq.PART_EXAM_MENTAL;
                        txtExamEyeSightRight4.Text = currentServiceReq.PART_EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft4.Text = currentServiceReq.PART_EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight4.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft4.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_LEFT;
                        txtExamEntLeftNormal4.Text = currentServiceReq.PART_EXAM_EAR_LEFT_NORMAL;
                        txtExamEntLeftWhisper4.Text = currentServiceReq.PART_EXAM_EAR_LEFT_WHISPER;
                        txtExamEntRightNomal4.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_NORMAL;
                        txtExamEntRightWhisper4.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_WHISPER;
                        SetDefaultGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLicenseClass4_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    if (this.currentModule == null)
                    {
                        CallModule.Run(CallModule.LicenseClass, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule.Run(CallModule.LicenseClass, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    }
                    InitComboLicenseClass4();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEdit4_Click(object sender, System.EventArgs e)
        {
            try
            {
                var focusRow = (ADO.DiseaseTypeADO)gridView2.GetFocusedRow();
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
                ReloadGrid2(focusRow);

            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEdit5_Click(object sender, System.EventArgs e)
        {
            try
            {
                var focusRow = (ADO.DiseaseTypeADO)gridView2.GetFocusedRow();
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
                ReloadGrid2(focusRow);

            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadGrid2(DiseaseTypeADO focusRow)
        {
            try
            {
                var Alls = gridControl2.DataSource as List<ADO.DiseaseTypeADO>;
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
                gridControl2.DataSource = new List<ADO.DiseaseTypeADO>();
                gridControl2.DataSource = Alls;
                gridView2.FocusedRowHandle = count;
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultGrid()
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
                    gridControl2.DataSource = new List<ADO.DiseaseTypeADO>();
                    gridControl2.DataSource = lstAdo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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
        private HIS_KSK_PERIOD_DRIVER GetValuePeriodDriver()
        {
            HIS_KSK_PERIOD_DRIVER obj = new HIS_KSK_PERIOD_DRIVER();
            try
            {
                if (currentKskPeriodDriver != null)
                    obj.ID = currentKskPeriodDriver.ID;
                if (cboLicenseClass4.EditValue != null)
                {
                    obj.LICENSE_CLASS_ID = (long?)Int64.Parse(cboLicenseClass4.EditValue.ToString());
                    obj.LICENSE_CLASS_NAME = cboLicenseClass4.Text;
                }
                obj.PATHOLOGICAL_HISTORY_FAMILY = txtPathologicalHistoryFamily4.Text;
                obj.PATHOLOGICAL_HISTORY = txtPathologicalHistory4.Text;
                obj.MEDICINE_USING = txtMedicineUsing4.Text;
                obj.MATERNITY_HISTORY = txtMaternityHistory4.Text;
                obj.EXAM_NEUROLOGICAL = txtExamNeurological4.Text;
                obj.EXAM_NEUROLOGICAL_CONCLUDE = txtNeurologicalConclude4.Text;
                obj.EXAM_NEUROLOGICAL_RANK = cboNeurologicalRank4.EditValue != null ? (long?)Int64.Parse(cboNeurologicalRank4.EditValue.ToString()) : null;
                obj.EXAM_MUSCLE_BONE = txtExamMuscleBone4.Text;
                obj.EXAM_MUSCLE_BONE_CONCLUDE = txtExamMuscleBoneConclude4.Text;
                obj.EXAM_MUSCLE_BONE_RANK = cboExamMuscleBoneRank4.EditValue != null ? (long?)Int64.Parse(cboExamMuscleBoneRank4.EditValue.ToString()) : null;
                obj.EXAM_MENTAL = txtExamMental4.Text;
                obj.EXAM_MENTAL_CONCLUDE = txtExamMentalConclude4.Text;
                obj.EXAM_MENTAL_RANK = cboExamMentalRank4.EditValue != null ? (long?)Int64.Parse(cboExamMentalRank4.EditValue.ToString()) : null;
                obj.EXAM_RESPIRATORY = txtExamRespiratory4.Text;
                obj.EXAM_RESPIRATORY_CONCLUDE = txtExamRespiratoryConclude4.Text;
                obj.EXAM_RESPIRATORY_RANK = cboExamRespiratoryRank4.EditValue != null ? (long?)Int64.Parse(cboExamRespiratoryRank4.EditValue.ToString()) : null;
                obj.EXAM_OEND = txtExamOend4.Text;
                obj.EXAM_OEND_CONCLUDE = txtExamOendConclude4.Text;
                obj.EXAM_OEND_RANK = cboExamOendRank4.EditValue != null ? (long?)Int64.Parse(cboExamOendRank4.EditValue.ToString()) : null;
                obj.EXAM_MATERNITY = txtExamMaternity4.Text;
                obj.EXAM_MATERNITY_CONCLUDE = txtExamMaternityConclude4.Text;
                obj.EXAM_MATERNITY_RANK = cboExamMaternityRank4.EditValue != null ? (long?)Int64.Parse(cboExamMaternityRank4.EditValue.ToString()) : null;
                obj.EXAM_EYE_DISEASE = txtExamEyeDisease4.Text;
                obj.EXAM_EYE_CONCLUDE = txtExamEyeConclude4.Text;
                obj.EXAM_EYE_RANK = cboExamEyeRank4.EditValue != null ? (long?)Int64.Parse(cboExamEyeRank4.EditValue.ToString()) : null;
                obj.EXAM_EYESIGHT_RIGHT = txtExamEyeSightRight4.Text;
                obj.EXAM_EYESIGHT_LEFT = txtExamEyeSightLeft4.Text;
                obj.EXAM_EYESIGHT_GLASS_RIGHT = txtExamEyeSightGlassRight4.Text;
                obj.EXAM_EYESIGHT_GLASS_LEFT = txtExamEyeSightGlassLeft4.Text;
                obj.EXAM_TWO_EYESIGHT = txtExamTwoEyesight4.Text;
                obj.EXAM_TWO_EYESIGHT_GLASS = txtExamTwoEyesightGlass4.Text;
                obj.EXAM_EYEFIELD_HORI_NORMAL = txtExamEyeFieldHoriNormal4.Text;
                obj.EXAM_EYEFIELD_HORI_LIMIT = txtExamEyeFieldHoriLimit4.Text;
                obj.EXAM_EYEFIELD_VERT_NORMAL = txtExamEyeFieldVertNormal4.Text;
                obj.EXAM_EYEFIELD_VERT_LIMIT = txtExamEyeFieldVertLimit4.Text;
                obj.EXAM_EYECOLOR_IS_NORMAL = chkExamEyeFieldIsNormal4.Checked ? (short?)1 : null;
                obj.EXAM_EYECOLOR_IS_BLIND = chkExamEyeFieldIsBlind4.Checked ? (short?)1 : null;
                obj.EXAM_EYECOLOR_IS_BLIND_RED = chkExamEyeFieldIsRed4.Checked ? (short?)1 : null;
                obj.EXAM_EYECOLOR_IS_BLIND_GREEN = chkExamEyeFieldIsGreen4.Checked ? (short?)1 : null;
                obj.EXAM_EYECOLOR_IS_BLIND_YELOW = chkExamEyeFieldIsYellow4.Checked ? (short?)1 : null;

                obj.EXAM_ENT_LEFT_NORMAL = txtExamEntLeftNormal4.Text;
                obj.EXAM_ENT_LEFT_WHISPER = txtExamEntLeftWhisper4.Text;
                obj.EXAM_ENT_RIGHT_NORMAL = txtExamEntRightNomal4.Text;
                obj.EXAM_ENT_RIGHT_WHISPER = txtExamEntRightWhisper4.Text;
                obj.EXAM_ENT_DISEASE = txtExamEntDisease4.Text;
                obj.EXAM_ENT_CONCLUDE = txtExamEntConclude4.Text;
                obj.EXAM_ENT_RANK = cboExamEntDiseaseRank4.EditValue != null ? (long?)Int64.Parse(cboExamEntDiseaseRank4.EditValue.ToString()) : null;
                obj.EXAM_CARDIOVASCULAR = txtExamCardiovascular4.Text;
                obj.EXAM_CARDIOVASCULAR_CONCLUDE = txtExamCardiovascularConclude4.Text;
                obj.EXAM_CARDIOVASCULAR_RANK = cboExamCardiovascularRank4.EditValue != null ? (long?)Int64.Parse(cboExamCardiovascularRank4.EditValue.ToString()) : null;
                obj.EXAM_CARDIOVASCULAR_BLOOD_MAX = spnExamCardiovascularBloodMax4.EditValue != null ? (long?)Int64.Parse(spnExamCardiovascularBloodMax4.EditValue.ToString()) : null;
                obj.EXAM_CARDIOVASCULAR_BLOOD_MIN = spnExamCardiovascularBloodMin4.EditValue != null ? (long?)Int64.Parse(spnExamCardiovascularBloodMin4.EditValue.ToString()) : null;
                obj.EXAM_CARDIOVASCULAR_PULSE = spnExamCardiovascularPulse4.EditValue != null ? (long?)Int64.Parse(spnExamCardiovascularPulse4.EditValue.ToString()) : null;
                obj.TEST_MORPHIN_HEROIN = txtMorphineHeroin4.Text;
                obj.TEST_AMPHETAMIN = txtTestAmphetamin4.Text;
                obj.TEST_METHAMPHETAMIN = txtTestMethamphetamin4.Text;
                obj.TEST_MARIJUANA = txtTestMarijuna4.Text;
                obj.TEST_CONCENTRATION = txtTestConcentration4.Text;
                obj.RESULT_SUBCLINICAL = txtResultSubclinical4.Text;
                obj.NOTE_SUBCLINICAL = txtNoteSubclinical4.Text;
                obj.CONCLUDE = txtConclude4.Text;

                obj.EXAM_EYE_LOGINNAME = cboExamEyeLoginName4.EditValue != null ? cboExamEyeLoginName4.EditValue.ToString() : null;
                obj.EXAM_ENT_LOGINNAME = cboExamEntLoginName4.EditValue != null ? cboExamEntLoginName4.EditValue.ToString() : null;
                obj.EXAM_CARDIOVASCULAR_LOGINNAME = cboExamCardiovascularLoginName4.EditValue != null ? cboExamCardiovascularLoginName4.EditValue.ToString() : null;
                obj.EXAM_SUBCLINICAL_LOGINNAME = cboExamSubclinicalLoginName4.EditValue != null ? cboExamSubclinicalLoginName4.EditValue.ToString() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }
        private List<HIS_PERIOD_DRIVER_DITY> GetDriverDity()
        {
            List<HIS_PERIOD_DRIVER_DITY> obj = new List<HIS_PERIOD_DRIVER_DITY>();
            try
            {
                var Alls = gridControl2.DataSource as List<ADO.DiseaseTypeADO>;

                if (Alls != null && Alls.Count > 0)
                {
                    if (currentKskUnderEight != null && lstDataDriverDity != null && lstDataDriverDity.Count > 0)
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
        private void txtResultSubclinical4_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameSItem = ENameSItem.KET_QUA_4;
            GetSpecInformation(ReturnObject = false);
        }

        private void txtMorphineHeroin4_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.MOR_HER_4;
            GetSpecInformation();
        }

        private void txtTestAmphetamin4_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.AMP_4;
            GetSpecInformation();
        }

        private void txtTestMethamphetamin4_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.MET_4;
            GetSpecInformation();
        }

        private void txtTestMarijuna4_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.MAR_4;
            GetSpecInformation();
        }

        private void txtTestConcentration4_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.NDC_4;
            GetSpecInformation();
        }

        #region ---PREVIEWKEYDOWN---


        private void txtPathologicalHistoryFamily4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPathologicalHistory4.Focus();
                    txtPathologicalHistory4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPathologicalHistory4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineUsing4.Focus();
                    txtMedicineUsing4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineUsing4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaternityHistory4.Focus();
                    txtMaternityHistory4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaternityHistory4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamMental4.Focus();
                    txtExamMental4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMental4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamMentalConclude4.Focus();
                    txtExamMentalConclude4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMentalConclude4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMentalRank4.Focus();
                    cboExamMentalRank4.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMentalRank4_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamNeurological4.Focus();
                    txtExamNeurological4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamNeurological4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNeurologicalConclude4.Focus();
                    txtNeurologicalConclude4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNeurologicalConclude4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboNeurologicalRank4.Focus();
                    cboNeurologicalRank4.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNeurologicalRank4_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEyeSightRight4.Focus();
                    txtExamEyeSightRight4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightRight4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightLeft4.Focus();
                    txtExamEyeSightLeft4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightLeft4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassRight4.Focus();
                    txtExamEyeSightGlassRight4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassRight4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassLeft4.Focus();
                    txtExamEyeSightGlassLeft4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassLeft4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamTwoEyesight4.Focus();
                    txtExamTwoEyesight4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamTwoEyesight4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamTwoEyesightGlass4.Focus();
                    txtExamTwoEyesightGlass4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamTwoEyesightGlass4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeFieldHoriNormal4.Focus();
                    txtExamEyeFieldHoriNormal4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeFieldHoriNormal4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeFieldHoriLimit4.Focus();
                    txtExamEyeFieldHoriLimit4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeFieldHoriLimit4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeFieldVertNormal4.Focus();
                    txtExamEyeFieldVertNormal4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeFieldVertNormal4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeFieldVertLimit4.Focus();
                    txtExamEyeFieldVertLimit4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeFieldVertLimit4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkExamEyeFieldIsNormal4.Focus();
                    chkExamEyeFieldIsNormal4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamEyeFieldIsNormal4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkExamEyeFieldIsBlind4.Focus();
                    chkExamEyeFieldIsBlind4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamEyeFieldIsBlind4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkExamEyeFieldIsRed4.Focus();
                    chkExamEyeFieldIsRed4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamEyeFieldIsRed4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkExamEyeFieldIsGreen4.Focus();
                    chkExamEyeFieldIsGreen4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamEyeFieldIsGreen4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkExamEyeFieldIsYellow4.Focus();
                    chkExamEyeFieldIsYellow4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamEyeFieldIsYellow4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeDisease4.Focus();
                    txtExamEyeDisease4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeDisease4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeConclude4.Focus();
                    txtExamEyeConclude4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeConclude4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEyeRank4.Focus();
                    cboExamEyeRank4.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEyeRank4_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEntLeftNormal4.Focus();
                    txtExamEntLeftNormal4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftNormal4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntLeftWhisper4.Focus();
                    txtExamEntLeftWhisper4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftWhisper4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightNomal4.Focus();
                    txtExamEntRightNomal4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightNomal4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightWhisper4.Focus();
                    txtExamEntRightWhisper4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightWhisper4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntDisease4.Focus();
                    txtExamEntDisease4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntDisease4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntConclude4.Focus();
                    txtExamEntConclude4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntConclude4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEntDiseaseRank4.Focus();
                    cboExamEntDiseaseRank4.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEntDiseaseRank4_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    spnExamCardiovascularPulse4.Focus();
                    spnExamCardiovascularPulse4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnExamCardiovascularPulse4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnExamCardiovascularBloodMax4.Focus();
                    spnExamCardiovascularBloodMax4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnExamCardiovascularBloodMax4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnExamCardiovascularBloodMin4.Focus();
                    spnExamCardiovascularBloodMin4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnExamCardiovascularBloodMin4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamCardiovascular4.Focus();
                    txtExamCardiovascular4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamCardiovascular4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamCardiovascularConclude4.Focus();
                    txtExamCardiovascularConclude4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamCardiovascularConclude4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamCardiovascularRank4.Focus();
                    cboExamCardiovascularRank4.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamCardiovascularRank4_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamRespiratory4.Focus();
                    txtExamRespiratory4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamRespiratory4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamRespiratoryConclude4.Focus();
                    txtExamRespiratoryConclude4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamRespiratoryConclude4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamRespiratoryRank4.Focus();
                    cboExamRespiratoryRank4.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamRespiratoryRank4_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamMuscleBone4.Focus();
                    txtExamMuscleBone4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMuscleBone4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamMuscleBoneConclude4.Focus();
                    txtExamMuscleBoneConclude4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMuscleBoneConclude4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMuscleBoneRank4.Focus();
                    cboExamMuscleBoneRank4.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMuscleBoneRank4_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamOend4.Focus();
                    txtExamOend4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamOend4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamOendConclude4.Focus();
                    txtExamOendConclude4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamOendConclude4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamOendRank4.Focus();
                    cboExamOendRank4.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamOendRank4_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamMaternity4.Focus();
                    txtExamMaternity4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMaternity4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamMaternityConclude4.Focus();
                    txtExamMaternityConclude4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMaternityConclude4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMaternityRank4.Focus();
                    cboExamMaternityRank4.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMaternityRank4_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtMorphineHeroin4.Focus();
                    txtMorphineHeroin4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMorphineHeroin4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestAmphetamin4.Focus();
                    txtTestAmphetamin4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestAmphetamin4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestMethamphetamin4.Focus();
                    txtTestMethamphetamin4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestMethamphetamin4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestMarijuna4.Focus();
                    txtTestMarijuna4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestMarijuna4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestConcentration4.Focus();
                    txtTestConcentration4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestConcentration4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtResultSubclinical4.Focus();
                    txtResultSubclinical4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtResultSubclinical4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNoteSubclinical4.Focus();
                    txtNoteSubclinical4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNoteSubclinical4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtConclude4.Focus();
                    txtConclude4.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtConclude4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
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
