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
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private void FillDataPageDriverCar()
        {
            try
            {
                ResetControlDriverCar();
                SetDataCboLicenseClass();
                SetDataCboRank(cboExamMaternityRank5);
                SetDataCboRank(cboExamRespiratoryRank5);
                SetDataCboRank(cboExamNeurologicalRank5);
                SetDataCboRank(cboExamMuscleBoneRank5);
                SetDataCboRank(cboExamOendRank5);
                SetDataCboRank(cboExamMentalRank5);
                SetDataCboRank(cboExamEyeRank5);
                SetDataCboRank(cboExamEntDiseaseRank5);
                SetDataCboRank(cboExamCardiovascularRank5);
                SetDataCboExamLoginName(cboExamCardiovascularLoginName5);
                SetDataCboExamLoginName(cboExamEyeLoginName5);
                SetDataCboExamLoginName(cboExamEntLoginName5);
                SetDataCboExamLoginName(cboExamSubclinicalLoginName5);
                FillDataDriverCar();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetControlDriverCar()
        {
            try
            {
                spnDiseaseOccuOneYear5.EditValue = null;
                spnDiseaseOccuTwoYear5.EditValue = null;
                spnDiseaseThreeYear5.EditValue = null;
                spnDiseaseFour5.EditValue = null;
                spnExamCardiovascularBloodMax5.EditValue = null;
                spnExamCardiovascularBloodMin5.EditValue = null;
                spnExamCardiovascularPulse5.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataDriverCar()
        {
            try
            {
                if (currentServiceReq != null)
                {
                    CommonParam param = new CommonParam();
                    HisKskDriverCarFilter filter = new HisKskDriverCarFilter();
                    filter.SERVICE_REQ_ID = currentServiceReq.ID;
                    var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_DRIVER_CAR>>("api/HisKskDriverCar/Get", ApiConsumers.MosConsumer, filter, param);
                    if (data != null && data.Count > 0)
                    {
                        currentKskDriverCar = data.First();
                        cboLicenseClass.EditValue = currentKskDriverCar.LICENSE_CLASS;
                        txtDiseaseOne5.Text = currentKskDriverCar.HISTORY_DISEASE_ONE;
                        if (!string.IsNullOrEmpty(currentKskDriverCar.HISTORY_DISEASE_ONE_YEAR)) spnDiseaseOccuOneYear5.EditValue = Int64.Parse(currentKskDriverCar.HISTORY_DISEASE_ONE_YEAR);
                        txtDiseaseTwo5.Text = currentKskDriverCar.HISTORY_DISEASE_TWO;
                        if (!string.IsNullOrEmpty(currentKskDriverCar.HISTORY_DISEASE_TWO_YEAR)) spnDiseaseOccuTwoYear5.EditValue = Int64.Parse(currentKskDriverCar.HISTORY_DISEASE_TWO_YEAR);
                        txtDiseaseThree5.Text = currentKskDriverCar.HISTORY_DISEASE_THREE;
                        if (!string.IsNullOrEmpty(currentKskDriverCar.HISTORY_DISEASE_THREE_YEAR)) spnDiseaseThreeYear5.EditValue = Int64.Parse(currentKskDriverCar.HISTORY_DISEASE_THREE_YEAR);
                        txtDiseaseFour5.Text = currentKskDriverCar.HISTORY_DISEASE_FOUR;
                        if (!string.IsNullOrEmpty(currentKskDriverCar.HISTORY_DISEASE_FOUR_YEAR)) spnDiseaseFour5.EditValue = Int64.Parse(currentKskDriverCar.HISTORY_DISEASE_FOUR_YEAR);
                        txtExamMaternity5.Text = currentKskDriverCar.EXAM_MATERNITY;
                        txtExamMaternityConclude5.Text = currentKskDriverCar.EXAM_MATERNITY_CONCLUDE;
                        cboExamMaternityRank5.EditValue = currentKskDriverCar.EXAM_MATERNITY_RANK;
                        txtExamRespiratory5.Text = currentKskDriverCar.EXAM_RESPIRATORY;
                        txtExamRespiratoryConclude5.Text = currentKskDriverCar.EXAM_RESPIRATORY_CONCLUDE;
                        cboExamRespiratoryRank5.EditValue = currentKskDriverCar.EXAM_RESPIRATORY_RANK;
                        txtExamNeurological5.Text = currentKskDriverCar.EXAM_NEUROLOGICAL;
                        txtExamNeurologicalConclude5.Text = currentKskDriverCar.EXAM_NEUROLOGICAL_CONCLUDE;
                        cboExamNeurologicalRank5.EditValue = currentKskDriverCar.EXAM_NEUROLOGICAL_RANK;
                        txtExamMuscleBone5.Text = currentKskDriverCar.EXAM_MUSCLE_BONE;
                        txtExamMuscleBoneConclude5.Text = currentKskDriverCar.EXAM_MUSCLE_BONE_CONCLUDE;
                        cboExamMuscleBoneRank5.EditValue = currentKskDriverCar.EXAM_MUSCLE_BONE_RANK;
                        txtExamOend5.Text = currentKskDriverCar.EXAM_OEND;
                        txtExamOendConclude5.Text = currentKskDriverCar.EXAM_OEND_CONCLUDE;
                        cboExamOendRank5.EditValue = currentKskDriverCar.EXAM_OEND_RANK;
                        txtExamMental5.Text = currentKskDriverCar.EXAM_MENTAL;
                        txtExamMentalConclude5.Text = currentKskDriverCar.EXAM_MENTAL_CONCLUDE;
                        cboExamMentalRank5.EditValue = currentKskDriverCar.EXAM_MENTAL_RANK;
                        txtExamEyeSightRight5.Text = currentKskDriverCar.EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft5.Text = currentKskDriverCar.EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight5.Text = currentKskDriverCar.EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft5.Text = currentKskDriverCar.EXAM_EYESIGHT_GLASS_LEFT;
                        txtExamEyeDisease5.Text = currentKskDriverCar.EXAM_EYE_DISEASE;
                        cboExamEyeRank5.EditValue = currentKskDriverCar.EXAM_EYE_RANK;
                        txtExamEyeConclude5.Text = currentKskDriverCar.EXAM_EYE_CONCLUDE;
                        txtExamTwoEyesight5.Text = currentKskDriverCar.EXAM_TWO_EYESIGHT;
                        txtExamTwoEyesightGlass5.Text = currentKskDriverCar.EXAM_TWO_EYESIGHT_GLASS;
                        txtExamEyeFieldHoriNormal5.Text = currentKskDriverCar.EXAM_EYEFIELD_HORI_NORMAL;
                        txtExamEyeFieldHoriLimit5.Text = currentKskDriverCar.EXAM_EYEFIELD_HORI_LIMIT;
                        txtExamEyeFieldVertNormal5.Text = currentKskDriverCar.EXAM_EYEFIELD_VERT_NORMAL;
                        txtExamEyeFieldVertLimit5.Text = currentKskDriverCar.EXAM_EYEFIELD_VERT_LIMIT;
                        chkExamEyeFieldIsNormal5.Checked = currentKskDriverCar.EXAM_EYECOLOR_IS_NORMAL == (short?)1 ? true : false;
                        chkExamEyeFieldIsBlind5.Checked = currentKskDriverCar.EXAM_EYECOLOR_IS_BLIND == (short?)1 ? true : false;
                        chkExamEyeFieldIsRed5.Checked = currentKskDriverCar.EXAM_EYECOLOR_IS_BLIND_RED == (short?)1 ? true : false;
                        chkExamEyeFieldIsGreen5.Checked = currentKskDriverCar.EXAM_EYECOLOR_IS_BLIND_GREEN == (short?)1 ? true : false;
                        chkExamEyeFieldIsYellow5.Checked = currentKskDriverCar.EXAM_EYECOLOR_IS_BLIND_YELOW == (short?)1 ? true : false;
                        txtExamEntLeftNormal5.Text = currentKskDriverCar.EXAM_ENT_LEFT_NORMAL;
                        txtExamEntLeftWhisper5.Text = currentKskDriverCar.EXAM_ENT_LEFT_WHISPER;
                        txtExamEntRightNomal5.Text = currentKskDriverCar.EXAM_ENT_RIGHT_NORMAL;
                        txtExamEntRightWhisper5.Text = currentKskDriverCar.EXAM_ENT_RIGHT_WHISPER;
                        txtExamEntDisease5.Text = currentKskDriverCar.EXAM_ENT_DISEASE;
                        txtExamEntDiseaseConclude5.Text = currentKskDriverCar.EXAM_ENT_CONCLUDE;
                        cboExamEntDiseaseRank5.EditValue = currentKskDriverCar.EXAM_ENT_RANK;
                        txtExamCardiovascular5.Text = currentKskDriverCar.EXAM_CARDIOVASCULAR;
                        txtExamCardiovascularConclude5.Text = currentKskDriverCar.EXAM_CARDIOVASCULAR_CONCLUDE;
                        spnExamCardiovascularBloodMax5.EditValue = currentKskDriverCar.EXAM_CARDIOVASCULAR_BLOOD_MAX;
                        spnExamCardiovascularBloodMin5.EditValue = currentKskDriverCar.EXAM_CARDIOVASCULAR_BLOOD_MIN;
                        spnExamCardiovascularPulse5.EditValue = currentKskDriverCar.EXAM_CARDIOVASCULAR_PULSE;
                        cboExamCardiovascularRank5.EditValue = currentKskDriverCar.EXAM_CARDIOVASCULAR_RANK;
                        txtMorphineHeroin5.Text = currentKskDriverCar.TEST_MORPHIN_HEROIN;
                        txtTestMethamphetamin5.Text = currentKskDriverCar.TEST_METHAMPHETAMIN;
                        txtTestMarijuna5.Text = currentKskDriverCar.TEST_MARIJUANA;
                        txtAmphetamin5.Text = currentKskDriverCar.TEST_AMPHETAMIN;
                        txtTestConcentration5.Text = currentKskDriverCar.TEST_CONCENTRATION;
                        txtResultSubclinical5.Text = currentKskDriverCar.RESULT_SUBCLINICAL;
                        txtNoteSubclinical5.Text = currentKskDriverCar.NOTE_SUBCLINICAL;
                        txtConclude5.Text = currentKskDriverCar.CONCLUDE;
                        txtDiseases5.Text = currentKskDriverCar.DISEASES;
                        cboExamEyeLoginName5.EditValue = currentKskDriverCar.EXAM_EYE_LOGINNAME;
                        cboExamEntLoginName5.EditValue = currentKskDriverCar.EXAM_ENT_LOGINNAME;
                        cboExamCardiovascularLoginName5.EditValue = currentKskDriverCar.EXAM_CARDIOVASCULAR_LOGINNAME;
                        cboExamSubclinicalLoginName5.EditValue = currentKskDriverCar.EXAM_SUBCLINICAL_LOGINNAME;
                    }
                    else
                    {
                        txtExamRespiratory5.Text = currentServiceReq.PART_EXAM_RESPIRATORY;
                        txtExamNeurological5.Text = currentServiceReq.PART_EXAM_NEUROLOGICAL;
                        txtExamMental5.Text = currentServiceReq.PART_EXAM_MENTAL;
                        txtExamEyeSightRight4.Text = currentServiceReq.PART_EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft4.Text = currentServiceReq.PART_EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight4.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft4.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_LEFT;
                        txtExamEntLeftNormal4.Text = currentServiceReq.PART_EXAM_EAR_LEFT_NORMAL;
                        txtExamEntLeftWhisper4.Text = currentServiceReq.PART_EXAM_EAR_LEFT_WHISPER;
                        txtExamEntRightNomal4.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_NORMAL;
                        txtExamEntRightWhisper4.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_WHISPER;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataCboLicenseClass()
        {
            try
            {
                List<TypeADO> listData = new List<TypeADO>();
                listData.Add(new TypeADO(1, "B1"));
                listData.Add(new TypeADO(2, "B2"));
                listData.Add(new TypeADO(3, "C"));
                listData.Add(new TypeADO(4, "D"));
                listData.Add(new TypeADO(5, "E"));
                listData.Add(new TypeADO(6, "F"));
                listData.Add(new TypeADO(7, "FB2"));
                listData.Add(new TypeADO(8, "FC"));
                listData.Add(new TypeADO(9, "FD"));
                listData.Add(new TypeADO(10, "FE"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Name", columnInfos, false, 400);
                ControlEditorLoader.Load(cboLicenseClass, listData, controlEditorADO);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private HIS_KSK_DRIVER_CAR GetValueDriverCar()
        {
            HIS_KSK_DRIVER_CAR obj = new HIS_KSK_DRIVER_CAR();
            try
            {
                if (currentKskDriverCar != null)
                    obj.ID = currentKskDriverCar.ID;
                obj.LICENSE_CLASS = cboLicenseClass.EditValue != null ? cboLicenseClass.EditValue.ToString() : null;
                obj.HISTORY_DISEASE_ONE = txtDiseaseOne5.Text;
                obj.HISTORY_DISEASE_ONE_YEAR = spnDiseaseOccuOneYear5.EditValue != null ? spnDiseaseOccuOneYear5.EditValue.ToString() : null;
                obj.HISTORY_DISEASE_TWO = txtDiseaseTwo5.Text;
                obj.HISTORY_DISEASE_TWO_YEAR = spnDiseaseOccuTwoYear5.EditValue != null ? spnDiseaseOccuTwoYear5.EditValue.ToString() : null;
                obj.HISTORY_DISEASE_THREE = txtDiseaseThree5.Text;
                obj.HISTORY_DISEASE_THREE_YEAR = spnDiseaseThreeYear5.EditValue != null ? spnDiseaseThreeYear5.EditValue.ToString() : null;
                obj.HISTORY_DISEASE_FOUR = txtDiseaseFour5.Text;
                obj.HISTORY_DISEASE_FOUR_YEAR = spnDiseaseFour5.EditValue != null ? spnDiseaseFour5.EditValue.ToString() : null;
                obj.EXAM_NEUROLOGICAL = txtExamNeurological5.Text;
                obj.EXAM_NEUROLOGICAL_CONCLUDE = txtExamNeurologicalConclude5.Text;
                obj.EXAM_NEUROLOGICAL_RANK = cboExamNeurologicalRank5.EditValue != null ? (long?)Int64.Parse(cboExamNeurologicalRank5.EditValue.ToString()) : null;
                obj.EXAM_MUSCLE_BONE = txtExamMuscleBone5.Text;
                obj.EXAM_MUSCLE_BONE_CONCLUDE = txtExamMuscleBoneConclude5.Text;
                obj.EXAM_MUSCLE_BONE_RANK = cboExamMuscleBoneRank5.EditValue != null ? (long?)Int64.Parse(cboExamMuscleBoneRank5.EditValue.ToString()) : null;
                obj.EXAM_MENTAL = txtExamMental5.Text;
                obj.EXAM_MENTAL_CONCLUDE = txtExamMentalConclude5.Text;
                obj.EXAM_MENTAL_RANK = cboExamMentalRank5.EditValue != null ? (long?)Int64.Parse(cboExamMentalRank5.EditValue.ToString()) : null;
                obj.EXAM_RESPIRATORY = txtExamRespiratory5.Text;
                obj.EXAM_RESPIRATORY_CONCLUDE = txtExamRespiratoryConclude5.Text;
                obj.EXAM_RESPIRATORY_RANK = cboExamRespiratoryRank5.EditValue != null ? (long?)Int64.Parse(cboExamRespiratoryRank5.EditValue.ToString()) : null;
                obj.EXAM_OEND = txtExamOend5.Text;
                obj.EXAM_OEND_CONCLUDE = txtExamOendConclude5.Text;
                obj.EXAM_OEND_RANK = cboExamOendRank5.EditValue != null ? (long?)Int64.Parse(cboExamOendRank5.EditValue.ToString()) : null;
                obj.EXAM_MATERNITY = txtExamMaternity5.Text;
                obj.EXAM_MATERNITY_CONCLUDE = txtExamMaternityConclude5.Text;
                obj.EXAM_MATERNITY_RANK = cboExamMaternityRank5.EditValue != null ? (long?)Int64.Parse(cboExamMaternityRank5.EditValue.ToString()) : null;
                obj.EXAM_EYE_DISEASE = txtExamEyeDisease5.Text;
                obj.EXAM_EYE_CONCLUDE = txtExamEyeConclude5.Text;
                obj.EXAM_EYE_RANK = cboExamEyeRank5.EditValue != null ? (long?)Int64.Parse(cboExamEyeRank5.EditValue.ToString()) : null;
                obj.EXAM_EYESIGHT_RIGHT = txtExamEyeSightRight5.Text;
                obj.EXAM_EYESIGHT_LEFT = txtExamEyeSightLeft5.Text;
                obj.EXAM_EYESIGHT_GLASS_RIGHT = txtExamEyeSightGlassRight5.Text;
                obj.EXAM_EYESIGHT_GLASS_LEFT = txtExamEyeSightGlassLeft5.Text;
                obj.EXAM_TWO_EYESIGHT = txtExamTwoEyesight5.Text;
                obj.EXAM_TWO_EYESIGHT_GLASS = txtExamTwoEyesightGlass5.Text;
                obj.EXAM_EYEFIELD_HORI_NORMAL = txtExamEyeFieldHoriNormal5.Text;
                obj.EXAM_EYEFIELD_HORI_LIMIT = txtExamEyeFieldHoriLimit5.Text;
                obj.EXAM_EYEFIELD_VERT_NORMAL = txtExamEyeFieldVertNormal5.Text;
                obj.EXAM_EYEFIELD_VERT_LIMIT = txtExamEyeFieldVertLimit5.Text;
                obj.EXAM_EYECOLOR_IS_NORMAL = chkExamEyeFieldIsNormal5.Checked ? (short?)1 : null;
                obj.EXAM_EYECOLOR_IS_BLIND = chkExamEyeFieldIsBlind5.Checked ? (short?)1 : null;
                obj.EXAM_EYECOLOR_IS_BLIND_RED = chkExamEyeFieldIsRed5.Checked ? (short?)1 : null;
                obj.EXAM_EYECOLOR_IS_BLIND_GREEN = chkExamEyeFieldIsGreen5.Checked ? (short?)1 : null;
                obj.EXAM_EYECOLOR_IS_BLIND_YELOW = chkExamEyeFieldIsYellow5.Checked ? (short?)1 : null;
                obj.EXAM_ENT_LEFT_NORMAL = txtExamEntLeftNormal5.Text;
                obj.EXAM_ENT_LEFT_WHISPER = txtExamEntLeftWhisper5.Text;
                obj.EXAM_ENT_RIGHT_NORMAL = txtExamEntRightNomal5.Text;
                obj.EXAM_ENT_RIGHT_WHISPER = txtExamEntRightWhisper5.Text;
                obj.EXAM_ENT_DISEASE = txtExamEntDisease5.Text;
                obj.EXAM_ENT_CONCLUDE = txtExamEntDiseaseConclude5.Text;
                obj.EXAM_ENT_RANK = cboExamEntDiseaseRank5.EditValue != null ? (long?)Int64.Parse(cboExamEntDiseaseRank5.EditValue.ToString()) : null;
                obj.EXAM_CARDIOVASCULAR = txtExamCardiovascular5.Text;
                obj.EXAM_CARDIOVASCULAR_CONCLUDE = txtExamCardiovascularConclude5.Text;
                obj.EXAM_CARDIOVASCULAR_RANK = cboExamCardiovascularRank5.EditValue != null ? (long?)Int64.Parse(cboExamCardiovascularRank5.EditValue.ToString()) : null;
                obj.EXAM_CARDIOVASCULAR_BLOOD_MAX = spnExamCardiovascularBloodMax5.EditValue != null ? (long?)Int64.Parse(spnExamCardiovascularBloodMax5.EditValue.ToString()) : null;
                obj.EXAM_CARDIOVASCULAR_BLOOD_MIN = spnExamCardiovascularBloodMin5.EditValue != null ? (long?)Int64.Parse(spnExamCardiovascularBloodMin5.EditValue.ToString()) : null;
                obj.EXAM_CARDIOVASCULAR_PULSE = spnExamCardiovascularPulse5.EditValue != null ? (long?)Int64.Parse(spnExamCardiovascularPulse5.EditValue.ToString()) : null;
                obj.TEST_MORPHIN_HEROIN = txtMorphineHeroin5.Text;
                obj.TEST_AMPHETAMIN = txtAmphetamin5.Text;
                obj.TEST_METHAMPHETAMIN = txtTestMethamphetamin5.Text;
                obj.TEST_MARIJUANA = txtTestMarijuna5.Text;
                obj.TEST_CONCENTRATION = txtTestConcentration5.Text;
                obj.RESULT_SUBCLINICAL = txtResultSubclinical5.Text;
                obj.NOTE_SUBCLINICAL = txtNoteSubclinical5.Text;
                obj.CONCLUDE = txtConclude5.Text;
                obj.DISEASES = txtDiseases5.Text;

                obj.EXAM_EYE_LOGINNAME = cboExamEyeLoginName5.EditValue != null ? cboExamEyeLoginName5.EditValue.ToString() : null;
                obj.EXAM_ENT_LOGINNAME = cboExamEntLoginName5.EditValue != null ? cboExamEntLoginName5.EditValue.ToString() : null;
                obj.EXAM_CARDIOVASCULAR_LOGINNAME = cboExamCardiovascularLoginName5.EditValue != null ? cboExamCardiovascularLoginName5.EditValue.ToString() : null;
                obj.EXAM_SUBCLINICAL_LOGINNAME = cboExamSubclinicalLoginName5.EditValue != null ? cboExamSubclinicalLoginName5.EditValue.ToString() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }

        private void txtResultSubclinical5_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameSItem = ENameSItem.KET_QUA_5;
            GetSpecInformation(ReturnObject = false);
        }


        private void txtMorphineHeroin5_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.MOR_HER_5;
            GetSpecInformation();
        }

        private void txtAmphetamin5_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.AMP_5;
            GetSpecInformation();
        }

        private void txtTestMethamphetamin5_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.MET_5;
            GetSpecInformation();
        }

        private void txtTestConcentration5_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.MAR_5;
            GetSpecInformation();
        }

        private void txtTestMarijuna5_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.NDC_5;
            GetSpecInformation();
        }
        #region ---PREVIEWKEYDOWN----

        private void cboLicenseClass_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtDiseaseOne5.Focus();
                    txtDiseaseOne5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiseaseOne5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnDiseaseOccuOneYear5.Focus();
                    spnDiseaseOccuOneYear5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnDiseaseOccuOneYear5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiseaseTwo5.Focus();
                    txtDiseaseTwo5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiseaseTwo5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnDiseaseOccuTwoYear5.Focus();
                    spnDiseaseOccuTwoYear5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnDiseaseOccuTwoYear5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiseaseThree5.Focus();
                    txtDiseaseThree5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiseaseThree5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnDiseaseThreeYear5.Focus();
                    spnDiseaseThreeYear5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnDiseaseThreeYear5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiseaseFour5.Focus();
                    txtDiseaseFour5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiseaseFour5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnDiseaseFour5.Focus();
                    spnDiseaseFour5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnDiseaseFour5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamMental5.Focus();
                    txtExamMental5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMental5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamMentalConclude5.Focus();
                    txtExamMentalConclude5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMentalConclude5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMentalRank5.Focus();
                    cboExamMentalRank5.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMentalRank5_CloseUp(object sender, CloseUpEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamNeurological5.Focus();
                    txtExamNeurological5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamNeurological5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamNeurologicalConclude5.Focus();
                    txtExamNeurologicalConclude5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamNeurologicalConclude5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamNeurologicalRank5.Focus();
                    cboExamNeurologicalRank5.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamNeurologicalRank5_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    spnExamCardiovascularPulse5.Focus();
                    spnExamCardiovascularPulse5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnExamCardiovascularPulse5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnExamCardiovascularBloodMax5.Focus();
                    spnExamCardiovascularBloodMax5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnExamCardiovascularBloodMax5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnExamCardiovascularBloodMin5.Focus();
                    spnExamCardiovascularBloodMin5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnExamCardiovascularBloodMin5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamCardiovascular5.Focus();
                    txtExamCardiovascular5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamCardiovascular5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamCardiovascularConclude5.Focus();
                    txtExamCardiovascularConclude5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamCardiovascularConclude5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamCardiovascularRank5.Focus();
                    cboExamCardiovascularRank5.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamCardiovascularRank5_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamRespiratory5.Focus();
                    txtExamRespiratory5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamRespiratory5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamRespiratoryConclude5.Focus();
                    txtExamRespiratoryConclude5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamRespiratoryConclude5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamRespiratoryRank5.Focus();
                    cboExamRespiratoryRank5.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamRespiratoryRank5_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamMuscleBone5.Focus();
                    txtExamMuscleBone5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMuscleBone5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamMuscleBoneConclude5.Focus();
                    txtExamMuscleBoneConclude5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMuscleBoneConclude5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMuscleBoneRank5.Focus();
                    cboExamMuscleBoneRank5.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMuscleBoneRank5_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamOend5.Focus();
                    txtExamOend5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamOend5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamOendConclude5.Focus();
                    txtExamOendConclude5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamOendConclude5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamOendRank5.Focus();
                    cboExamOendRank5.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamOendRank5_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamMaternity5.Focus();
                    txtExamMaternity5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMaternity5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamMaternityConclude5.Focus();
                    txtExamMaternityConclude5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMaternityConclude5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMaternityRank5.Focus();
                    cboExamMaternityRank5.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMaternityRank5_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEyeSightRight5.Focus();
                    txtExamEyeSightRight5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightRight5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightLeft5.Focus();
                    txtExamEyeSightLeft5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightLeft5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassRight5.Focus();
                    txtExamEyeSightGlassRight5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassRight5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassLeft5.Focus();
                    txtExamEyeSightGlassLeft5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassLeft5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamTwoEyesight5.Focus();
                    txtExamTwoEyesight5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamTwoEyesight5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamTwoEyesightGlass5.Focus();
                    txtExamTwoEyesightGlass5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamTwoEyesightGlass5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeFieldHoriNormal5.Focus();
                    txtExamEyeFieldHoriNormal5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeFieldHoriNormal5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeFieldHoriLimit5.Focus();
                    txtExamEyeFieldHoriLimit5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeFieldHoriLimit5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeFieldVertNormal5.Focus();
                    txtExamEyeFieldVertNormal5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeFieldVertNormal5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeFieldVertLimit5.Focus();
                    txtExamEyeFieldVertLimit5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeFieldVertLimit5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkExamEyeFieldIsNormal5.Focus();
                    chkExamEyeFieldIsNormal5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamEyeFieldIsNormal5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkExamEyeFieldIsBlind5.Focus();
                    chkExamEyeFieldIsBlind5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamEyeFieldIsBlind5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkExamEyeFieldIsRed5.Focus();
                    chkExamEyeFieldIsRed5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamEyeFieldIsRed5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkExamEyeFieldIsGreen5.Focus();
                    chkExamEyeFieldIsGreen5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamEyeFieldIsGreen5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkExamEyeFieldIsYellow5.Focus();
                    chkExamEyeFieldIsYellow5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamEyeFieldIsYellow5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeDisease5.Focus();
                    txtExamEyeDisease5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeDisease5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeConclude5.Focus();
                    txtExamEyeConclude5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeConclude5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEyeRank5.Focus();
                    cboExamEyeRank5.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEyeRank5_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEntLeftNormal5.Focus();
                    txtExamEntLeftNormal5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftNormal5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntLeftWhisper5.Focus();
                    txtExamEntLeftWhisper5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftWhisper5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightNomal5.Focus();
                    txtExamEntRightNomal5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightNomal5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightWhisper5.Focus();
                    txtExamEntRightWhisper5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightWhisper5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntDisease5.Focus();
                    txtExamEntDisease5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntDisease5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntDiseaseConclude5.Focus();
                    txtExamEntDiseaseConclude5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntDiseaseConclude5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEntDiseaseRank5.Focus();
                    cboExamEntDiseaseRank5.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEntDiseaseRank5_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtMorphineHeroin5.Focus();
                    txtMorphineHeroin5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMorphineHeroin5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAmphetamin5.Focus();
                    txtAmphetamin5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAmphetamin5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestMethamphetamin5.Focus();
                    txtTestMethamphetamin5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestMethamphetamin5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestMarijuna5.Focus();
                    txtTestMarijuna5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestMarijuna5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestConcentration5.Focus();
                    txtTestConcentration5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestConcentration5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtResultSubclinical5.Focus();
                    txtResultSubclinical5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtResultSubclinical5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNoteSubclinical5.Focus();
                    txtNoteSubclinical5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNoteSubclinical5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtConclude5.Focus();
                    txtConclude5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtConclude5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiseases5.Focus();
                    txtDiseases5.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiseases5_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
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
