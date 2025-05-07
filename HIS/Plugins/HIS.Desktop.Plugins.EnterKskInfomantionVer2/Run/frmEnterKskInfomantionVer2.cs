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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.Resources;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using MOS.SDO;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.ADO;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.Config;
namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2 : HIS.Desktop.Utility.FormBase
    {
        #region ----ObjCurrent-----
        private V_HIS_SERVICE_REQ currentServiceReq { get; set; }
        private HIS_KSK_GENERAL currentKskGeneral { get; set; }
        private HIS_KSK_OVER_EIGHTEEN currentKskOverEight { get; set; }
        private HIS_KSK_UNDER_EIGHTEEN currentKskUnderEight { get; set; }
        private HIS_KSK_PERIOD_DRIVER currentKskPeriodDriver { get; set; }
        private HIS_KSK_DRIVER_CAR currentKskDriverCar { get; set; }
        private HIS_KSK_OTHER currentKskOther { get; set; }
        private HIS_KSK_OCCUPATIONAL currentKsKOccupational { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_PERIOD_DRIVER_DITY> lstDataDriverDity { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_PERIOD_DRIVER_DITY> lstDataDriverDityOverE { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_KSK_UNEI_VATY> lstDataUneiVaty { get; set; }
        private bool ReturnObject = false;
        public enum ENameSItem
        {
            KET_LUAN_1,
            KHAC_XNM_2,
            KHAC_XNNT_2,
            CDHA_2,
            KET_QUA_3,
            KET_QUA_4,
            KET_QUA_5,
            KET_QUA_7
        }
        public ENameSItem? NameSItem { get; set; }

        public enum ENameOtherItem
        {
            SL_HC_2,
            SL_BC_2,
            SL_TC_2,
            DMA_2,
            URE_2,
            CRE_2,
            ASA_2,
            ALA_2,
            DUO_2,
            PRO_2,
            MOR_HER_4,
            AMP_4,
            MET_4,
            MAR_4,
            NDC_4,
            MOR_HER_5,
            AMP_5,
            MET_5,
            MAR_5,
            NDC_5
        }
        public ENameOtherItem? NameOtherItem { get; set; }
        public bool IsSignEmr { get; set; }
        #endregion

        Inventec.Desktop.Common.Modules.Module currentModule;
        public frmEnterKskInfomantionVer2(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_SERVICE_REQ hisServiceReq)
        {
            InitializeComponent();
            try
            {
                this.currentServiceReq = hisServiceReq;
                this.currentModule = moduleData;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmEnterKskInfomantionVer2_Load(object sender, System.EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                ShowInformationPatient();
                FillDataToPages();
                SetTabDefault();
                SetEnableControl();
                WaitingManager.Hide();
            }
            catch (System.Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableControl()
        {
            try
            {
                if (HisConfigCFG.DisablePartExamByExecutor == "1")
                {
                    if (this.currentKskGeneral != null)
                    {
                        LoginNameEnableControl(currentKskGeneral.EXAM_CIRCULATION_LOGINNAME, txtExamCirculation, cboExamCirculationRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tuần hoàn.
                        LoginNameEnableControl(currentKskGeneral.EXAM_RESPIRATORY_LOGINNAME, txtExamRespiratory, cboExamRespiratoryRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám hô hấp.
                        LoginNameEnableControl(currentKskGeneral.EXAM_DIGESTION_LOGINNAME, txtExamDigestion, cboExamDigestionRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tiêu hóa.
                        LoginNameEnableControl(currentKskGeneral.EXAM_KIDNEY_UROLOGY_LOGINNAME, txtExamKidneyUrology, cboExamKidneyUrologyRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thận tiết niệu.
                        LoginNameEnableControl(currentKskGeneral.EXAM_NEUROLOGICAL_LOGINNAME, txtExamNeurological, cboExamNeurologicalRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thần kinh.
                        LoginNameEnableControl(currentKskGeneral.EXAM_MUSCLE_BONE_LOGINNAME, txtExamMuscleBone, cboExamMuscleBoneRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cơ xương khớp.
                        LoginNameEnableControl(currentKskGeneral.EXAM_ENT_LOGINNAME, txtExamEntLeftNormal); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskGeneral.EXAM_ENT_LOGINNAME, txtExamEntRightNomal); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskGeneral.EXAM_ENT_LOGINNAME, txtExamEntLeftWhisper); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskGeneral.EXAM_ENT_LOGINNAME, txtExamEntRightWhisper); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskGeneral.EXAM_ENT_LOGINNAME, txtExamEntDisease, cboExamEntDiseaseRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskGeneral.EXAM_STOMATOLOGY_LOGINNAME, txtExamStomatologyUpper); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám răng hàm mặt.
                        LoginNameEnableControl(currentKskGeneral.EXAM_STOMATOLOGY_LOGINNAME, txtExamStomatologyLower); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám răng hàm mặt.
                        LoginNameEnableControl(currentKskGeneral.EXAM_STOMATOLOGY_LOGINNAME, txtExamStomatologyDisease, cboExamStomatologyRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám răng hàm mặt.
                        LoginNameEnableControl(currentKskGeneral.EXAM_EYE_LOGINNAME, txtExamEyeSightRight); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskGeneral.EXAM_EYE_LOGINNAME, txtExamEyeSightLeft); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskGeneral.EXAM_EYE_LOGINNAME, txtExamEyeSightGlassRight); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskGeneral.EXAM_EYE_LOGINNAME, txtExamEyeSightGlassLeft); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskGeneral.EXAM_EYE_LOGINNAME, txtExamEyeDisease, cboExamEyeRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskGeneral.EXAM_OEND_LOGINNAME, txtExamOend, cboExamOendRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám nội tiết.
                        LoginNameEnableControl(currentKskGeneral.EXAM_MENTAL_LOGINNAME, txtExamMental, cboExamMentalRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tâm thần.
                        LoginNameEnableControl(currentKskGeneral.EXAM_DERMATOLOGY_LOGINNAME, txtExamDernatology, cboExamDernatologyRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám da liễu.
                        LoginNameEnableControl(currentKskGeneral.EXAM_SURGERY_LOGINNAME, txtExamSurgery, cboExamSurgeryRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable thông tin ngoại khoa
                        LoginNameEnableControl(currentKskGeneral.EXAM_OBSTETRIC_LOGINNAME, txtExamObstetric, cboExamSurgeryRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám sản phụ khoa.
                        LoginNameEnableControl(currentKskGeneral.EXAM_SUBCLINICAL_LOGINNAME, txtResultSubclinical, txtNoteSubclinical); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                    }
                    if (this.currentKskOverEight != null)
                    {
                        LoginNameEnableControl(currentKskOverEight.EXAM_CIRCULATION_LOGINNAME, txtExamCirculation2, cboExamCirculationRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tuần hoàn.
                        LoginNameEnableControl(currentKskOverEight.EXAM_RESPIRATORY_LOGINNAME, txtExamRespiratory2, cboExamRespiratoryRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám hô hấp.
                        LoginNameEnableControl(currentKskOverEight.EXAM_DIGESTION_LOGINNAME, txtExamDigestion2, cboExamDigestionRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tiêu hóa.
                        LoginNameEnableControl(currentKskOverEight.EXAM_KIDNEY_UROLOGY_LOGINNAME, txtExamKidneyUrology2, cboExamKidneyUrologyRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thận tiết niệu.
                        LoginNameEnableControl(currentKskOverEight.EXAM_NEUROLOGICAL_LOGINNAME, txtExamNeurological2, cboExamNeurologicalRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thần kinh.
                        LoginNameEnableControl(currentKskOverEight.EXAM_MUSCLE_BONE_LOGINNAME, txtExamMuscleBone2, cboExamMuscleBoneRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cơ xương khớp.
                        LoginNameEnableControl(currentKskOverEight.EXAM_ENT_LOGINNAME, txtExamEntLeftNormal2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskOverEight.EXAM_ENT_LOGINNAME, txtExamEntRightNomal2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskOverEight.EXAM_ENT_LOGINNAME, txtExamEntLeftWhisper2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskOverEight.EXAM_ENT_LOGINNAME, txtExamEntRightWhisper2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskOverEight.EXAM_ENT_LOGINNAME, txtExamEntDisease2, cboExamEntDiseaseRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskOverEight.EXAM_STOMATOLOGY_LOGINNAME, txtExamStomatologyUpper2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám răng hàm mặt.
                        LoginNameEnableControl(currentKskOverEight.EXAM_STOMATOLOGY_LOGINNAME, txtExamStomatologyLower2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám răng hàm mặt.
                        LoginNameEnableControl(currentKskOverEight.EXAM_STOMATOLOGY_LOGINNAME, txtExamStomatologyDisease2, cboExamStomatologyRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám răng hàm mặt.
                        LoginNameEnableControl(currentKskOverEight.EXAM_EYE_LOGINNAME, txtExamEyeSightRight2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskOverEight.EXAM_EYE_LOGINNAME, txtExamEyeSightLeft2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskOverEight.EXAM_EYE_LOGINNAME, txtExamEyeSightGlassRight2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskOverEight.EXAM_EYE_LOGINNAME, txtExamEyeSightGlassLeft2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskOverEight.EXAM_EYE_LOGINNAME, txtExamEyeDisease2, cboExamEyeRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskOverEight.EXAM_OEND_LOGINNAME, txtExamOend2, cboExamOend2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám nội tiết.
                        LoginNameEnableControl(currentKskOverEight.EXAM_MENTAL_LOGINNAME, txtExamMental2, cboExamMentalRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tâm thần.
                        LoginNameEnableControl(currentKskOverEight.EXAM_DERMATOLOGY_LOGINNAME, txtExamDernatology2, cboExamDernatologyRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám da liễu.

                        LoginNameEnableControl(currentKskOverEight.EXAM_SURGERY_LOGINNAME, txtExamSurgery2, cboExamSurgeryRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable thông tin ngoại khoa
                        LoginNameEnableControl(currentKskOverEight.EXAM_OBSTETRIC_LOGINNAME, txtExamObstetric2, cboExamObstetricRank2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám sản phụ khoa.
                        LoginNameEnableControl(currentKskOverEight.TEST_URINE_LOGINNAME, txtTestUrineGluco2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin xét nghiệm nước tiểu
                        LoginNameEnableControl(currentKskOverEight.TEST_URINE_LOGINNAME, txtTestUrineProtein2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin xét nghiệm nước tiểu
                        LoginNameEnableControl(currentKskOverEight.TEST_URINE_LOGINNAME, txtTestUrineOther2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin xét nghiệm nước tiểu
                        LoginNameEnableControl(currentKskOverEight.DIIM_LOGINNAME, txtResultDiim2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin kết quả chẩn đoán hình ảnh
                        LoginNameEnableControl(currentKskOverEight.TEST_BLOOD, txtTestBloodHc2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin kết quả xét nghiệm máu
                        LoginNameEnableControl(currentKskOverEight.TEST_BLOOD, txtTestBloodBc2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin kết quả xét nghiệm máu
                        LoginNameEnableControl(currentKskOverEight.TEST_BLOOD, txtTestBloodTc2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin kết quả xét nghiệm máu
                        LoginNameEnableControl(currentKskOverEight.TEST_BLOOD, txtTestBloodGluco2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin kết quả xét nghiệm máu
                        LoginNameEnableControl(currentKskOverEight.TEST_BLOOD, txtTestBloodUre2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin kết quả xét nghiệm máu
                        LoginNameEnableControl(currentKskOverEight.TEST_BLOOD, txtTestBloodCreatinin2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin kết quả xét nghiệm máu
                        LoginNameEnableControl(currentKskOverEight.TEST_BLOOD, txtTestBloodAsat2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin kết quả xét nghiệm máu
                        LoginNameEnableControl(currentKskOverEight.TEST_BLOOD, txtTestBloodAlat2); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin kết quả xét nghiệm máu
                    }
                    if (this.currentKskUnderEight != null)
                    {
                        LoginNameEnableControl(currentKskUnderEight.EXAM_CIRCULATION_LOGINNAME, txtExamCirculation3, cboExamCirculationRank3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tuần hoàn.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_RESPIRATORY_LOGINNAME, txtExamRespiratory3, cboExamRespiratoryRank3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám hô hấp.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_DIGESTION_LOGINNAME, txtExamDigestion3, cboExamDigestionRank3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tiêu hóa.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_KIDNEY_UROLOGY_LOGINNAME, txtExamKidneyUrology3, cboExamKidneyUrologyRank3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thận tiết niệu.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_NEURO_MENTAL_LOGINNAME, txtExamNeuroMental3, cboExamNeuroMental3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thần kinh.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_CLINICAL_OTHER_LOGINNAME, txtExamClinicalOther3, cboExamClinicalOther3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cơ xương khớp.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_ENT_LOGINNAME, txtExamEntLeftNormal3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_ENT_LOGINNAME, txtExamEntRightNomal3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_ENT_LOGINNAME, txtExamEntLeftWhisper3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_ENT_LOGINNAME, txtExamEntRightWhisper3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_ENT_LOGINNAME, txtExamEntDisease3, cboExamEntDiseaseRank3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_STOMATOLOGY_LOGINNAME, txtExamStomatologyUpper3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám răng hàm mặt.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_STOMATOLOGY_LOGINNAME, txtExamStomatologyLower3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám răng hàm mặt.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_STOMATOLOGY_LOGINNAME, txtExamStomatologyDisease3, cboExamStomatologyRank3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám răng hàm mặt.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_EYE_LOGINNAME, txtExamEyeSightRight3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_EYE_LOGINNAME, txtExamEyeSightLeft3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_EYE_LOGINNAME, txtExamEyeSightGlassRight3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_EYE_LOGINNAME, txtExamEyeSightGlassLeft3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_EYE_LOGINNAME, txtExamEyeDisease3, cboExamEyeRank3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_SUBCLINICAL_LOGINNAME, txtResultSubclinical3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                    }
                    if (this.currentKskPeriodDriver != null)
                    {
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_CARDIOVASCULAR_LOGINNAME, txtExamCardiovascular4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tim mạch.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_CARDIOVASCULAR_LOGINNAME, spnExamCardiovascularPulse4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tim mạch.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_CARDIOVASCULAR_LOGINNAME, spnExamCardiovascularBloodMax4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tim mạch.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_CARDIOVASCULAR_LOGINNAME, spnExamCardiovascularBloodMin4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tim mạch.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_CARDIOVASCULAR_LOGINNAME, txtExamCardiovascularConclude4, cboExamCardiovascularRank4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tim mạch.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_RESPIRATORY_LOGINNAME, txtExamRespiratory4, cboExamRespiratoryRank4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám hô hấp.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_RESPIRATORY_LOGINNAME, txtExamRespiratoryConclude4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám hô hấp.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_NEUROLOGICAL_LOGINNAME, txtExamNeurological4, cboNeurologicalRank4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thần kinh.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_NEUROLOGICAL_LOGINNAME, txtNeurologicalConclude4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thần kinh.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_MUSCLE_BONE_LOGINNAME, txtExamMuscleBone4, cboExamMuscleBoneRank4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cơ xương khớp.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_MUSCLE_BONE_LOGINNAME, txtExamMuscleBoneConclude4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cơ xương khớp.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_ENT_LOGINNAME, txtExamEntLeftNormal4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_ENT_LOGINNAME, txtExamEntRightNomal4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_ENT_LOGINNAME, txtExamEntLeftWhisper4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_ENT_LOGINNAME, txtExamEntRightWhisper4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_ENT_LOGINNAME, txtExamEntDisease4, cboExamEntDiseaseRank4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_ENT_LOGINNAME, txtExamEntConclude4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, txtExamEyeSightRight4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, txtExamEyeSightLeft4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, txtExamEyeSightGlassRight4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, txtExamEyeSightGlassLeft4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, txtExamEyeDisease4, cboExamEyeRank4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, txtExamEyeConclude4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, txtExamTwoEyesight4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, txtExamTwoEyesightGlass4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, txtExamEyeFieldHoriNormal4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, txtExamEyeFieldHoriLimit4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, txtExamEyeFieldVertNormal4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, txtExamEyeFieldVertLimit4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, chkExamEyeFieldIsBlind4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, chkExamEyeFieldIsGreen4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, chkExamEyeFieldIsNormal4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, chkExamEyeFieldIsRed4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, chkExamEyeFieldIsYellow4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_MENTAL_LOGINNAME, txtExamMental4, cboExamMentalRank4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tâm thần.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_MENTAL_LOGINNAME, txtExamMentalConclude4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tâm thần.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_OEND_LOGINNAME, txtExamOend4, cboExamOendRank4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám nội tiết.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_OEND_LOGINNAME, txtExamOendConclude4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám nội tiết.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_MATERNITY_LOGINNAME, txtExamMaternity4, cboExamMaternityRank4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thai sản
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_MATERNITY_LOGINNAME, txtExamMaternityConclude4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thai sản
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_SUBCLINICAL_LOGINNAME, txtMorphineHeroin4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_SUBCLINICAL_LOGINNAME, txtTestAmphetamin4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_SUBCLINICAL_LOGINNAME, txtTestMethamphetamin4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_SUBCLINICAL_LOGINNAME, txtTestMarijuna4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_SUBCLINICAL_LOGINNAME, txtTestConcentration4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_SUBCLINICAL_LOGINNAME, txtResultSubclinical4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_SUBCLINICAL_LOGINNAME, txtNoteSubclinical4); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                    }
                    if (this.currentKskDriverCar != null)
                    {
                        LoginNameEnableControl(currentKskDriverCar.EXAM_CARDIOVASCULAR_LOGINNAME, txtExamCardiovascular5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tim mạch.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_CARDIOVASCULAR_LOGINNAME, spnExamCardiovascularPulse5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tim mạch.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_CARDIOVASCULAR_LOGINNAME, spnExamCardiovascularBloodMax5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tim mạch.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_CARDIOVASCULAR_LOGINNAME, spnExamCardiovascularBloodMin5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tim mạch.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_CARDIOVASCULAR_LOGINNAME, txtExamCardiovascularConclude5, cboExamCardiovascularRank5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tim mạch.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_RESPIRATORY_LOGINNAME, txtExamRespiratory5, cboExamRespiratoryRank5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám hô hấp.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_RESPIRATORY_LOGINNAME, txtExamRespiratoryConclude5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám hô hấp.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_NEUROLOGICAL_LOGINNAME, txtExamNeurological5, cboExamNeurologicalRank5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thần kinh.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_NEUROLOGICAL_LOGINNAME, txtExamNeurologicalConclude5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thần kinh.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_MUSCLE_BONE_LOGINNAME, txtExamMuscleBone5, cboExamMuscleBoneRank5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cơ xương khớp.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_MUSCLE_BONE_LOGINNAME, txtExamMuscleBoneConclude5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cơ xương khớp.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_ENT_LOGINNAME, txtExamEntLeftNormal5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_ENT_LOGINNAME, txtExamEntRightNomal5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_ENT_LOGINNAME, txtExamEntLeftWhisper5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_ENT_LOGINNAME, txtExamEntRightWhisper5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_ENT_LOGINNAME, txtExamEntDisease5, cboExamEntDiseaseRank5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_ENT_LOGINNAME, txtExamEntDiseaseConclude5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, txtExamEyeSightRight5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, txtExamEyeSightLeft5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, txtExamEyeSightGlassRight5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, txtExamEyeSightGlassLeft5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, txtExamEyeDisease5, cboExamEyeRank5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, txtExamEyeConclude5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, txtExamTwoEyesight5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, txtExamTwoEyesightGlass5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, txtExamEyeFieldHoriNormal5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, txtExamEyeFieldHoriLimit5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, txtExamEyeFieldVertNormal5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, txtExamEyeFieldVertLimit5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, chkExamEyeFieldIsBlind5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, chkExamEyeFieldIsGreen5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, chkExamEyeFieldIsNormal5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, chkExamEyeFieldIsRed5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, chkExamEyeFieldIsYellow5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_MENTAL_LOGINNAME, txtExamMental5, cboExamMentalRank5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tâm thần.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_MENTAL_LOGINNAME, txtExamMentalConclude5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tâm thần.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_OEND_LOGINNAME, txtExamOend5, cboExamOendRank5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám nội tiết.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_OEND_LOGINNAME, txtExamOendConclude5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám nội tiết.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_MATERNITY_LOGINNAME, txtExamMaternity5, cboExamMaternityRank5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thai sản
                        LoginNameEnableControl(currentKskDriverCar.EXAM_MATERNITY_LOGINNAME, txtExamMaternityConclude5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thai sản
                        LoginNameEnableControl(currentKskDriverCar.EXAM_SUBCLINICAL_LOGINNAME, txtMorphineHeroin5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_SUBCLINICAL_LOGINNAME, txtAmphetamin5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_SUBCLINICAL_LOGINNAME, txtTestMethamphetamin5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_SUBCLINICAL_LOGINNAME, txtTestMarijuna5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_SUBCLINICAL_LOGINNAME, txtTestConcentration5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_SUBCLINICAL_LOGINNAME, txtResultSubclinical5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_SUBCLINICAL_LOGINNAME, txtNoteSubclinical5); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.
                    }
                    if (this.currentKsKOccupational != null) 
                    {
                        LoginNameEnableControl(currentKsKOccupational.EXAM_CIRCULATION_LOGINNAME, txtExamCirculation7, cboExamCirculationRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tuần hoàn.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_RESPIRATORY_LOGINNAME, txtExamRespiratory7, cboExamRespiratoryRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám hô hấp.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_DIGESTION_LOGINNAME, txtExamDigestion7, cboExamDigestionRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tiêu hóa.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_KIDNEY_UROLOGY_LOGINNAME, txtExamKidneyUrology7, cboExamKidneyUrologyRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thận tiết niệu.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_NEUROLOGICAL_LOGINNAME, txtExamOend7, cboExamOendRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thần kinh.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_MUSLE_BONE_LOGINNAME, txtExamMuscleBone7, cboExamMuscleBoneRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thần kinh.
                       
                        LoginNameEnableControl(currentKsKOccupational.EXAM_ENT_LOGINNAME, txtExamEntLeftNormal7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_ENT_LOGINNAME, txtExamEntRightNomal7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_ENT_LOGINNAME, txtExamEntLeftWhisper7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_ENT_LOGINNAME, txtExamEntRightWhisper7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_ENT_LOGINNAME, txtExamEntDisease7, cboExamEntDiseaseRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tai mũi họng.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_STOMATOLOGY_LOGINNAME, txtExamStomatologyUpper7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám răng hàm mặt.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_STOMATOLOGY_LOGINNAME, txtExamStomatologyLower7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám răng hàm mặt.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_STOMATOLOGY_LOGINNAME, txtExamStomatologyDisease7, cboExamStomatologyRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám răng hàm mặt.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_EYE_LOGINNAME, txtExamEyeSightRight7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_EYE_LOGINNAME, txtExamEyeSightLeft7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_EYE_LOGINNAME, txtExamEyeSightGlassRight7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_EYE_LOGINNAME, txtExamEyeSightGlassLeft7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_EYE_LOGINNAME, txtExamEyeDisease7, cboExamEyeRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám mắt.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_OEND_LOGINNAME, txtExamNeurological7, cboExamNeurologicalRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám nội tiết.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_MENTAL_LOGINNAME, txtExamMental7, cboExamMentalRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tâm thần.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_DERMATOLOGY_LOGINNAME, txtExamDernatology7, cboExamDernatologyRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám da liễu.
                        LoginNameEnableControl(currentKsKOccupational.NOTE_CLINICAL, txtNoteSubclinical);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_SURGERY_LOGINNAME, txtExamSurgery7,cboExamSurgeryRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable thông tin ngoại khoa
                        LoginNameEnableControl(currentKsKOccupational.EXAM_OBSTETRIC_LOGINNAME, txtExamObstetric7, cboExamObstetricRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám sản phụ khoa.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_SUBCLINICAL_LOGINNAME, txtResultSubclinical7, txtNoteSubclinical7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.

                       
                    }
                
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoginNameEnableControl(string data, BaseEdit txt, BaseEdit cbo = null)
        {

            try
            {
                var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (!string.IsNullOrEmpty(data) && data != loginName)
                {
                    txt.Enabled = false;
                    if (cbo != null)
                        cbo.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void SetTabDefault()
        {
            try
            {
                bool isActive = false;
                if (currentKskGeneral != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 0;
                    isActive = true;
                }
                else if (currentKskOverEight != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 1;
                    isActive = true;
                }
                else if (currentKskUnderEight != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 2;
                    isActive = true;
                }
                else if (currentKskPeriodDriver != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 3;
                    isActive = true;
                }
                else if (currentKskDriverCar != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 4;
                    isActive = true;
                }
                else if (currentKskOther != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 5;
                    isActive = true;
                }
                if (currentKsKOccupational != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 0;
                    isActive = true;
                }
                btnPrint.Enabled = isActive;
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowInformationPatient()
        {
            try
            {
                if (currentServiceReq != null)
                {
                    txtServiceCode.Text = currentServiceReq.SERVICE_REQ_CODE;
                    txtTreatmentCode.Text = currentServiceReq.TDL_TREATMENT_CODE;
                    txtPatientCode.Text = currentServiceReq.TDL_PATIENT_CODE;
                    txtPatientName.Text = currentServiceReq.TDL_PATIENT_NAME;
                    txtGender.Text = currentServiceReq.TDL_PATIENT_GENDER_NAME;
                    txtPatientDob.Text = currentServiceReq.TDL_PATIENT_IS_HAS_NOT_DAY_DOB != (short?)1 ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentServiceReq.TDL_PATIENT_DOB) : currentServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    txtInstructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentServiceReq.INTRUCTION_TIME);
                    if (currentServiceReq.TDL_KSK_CONTRACT_ID != null && currentServiceReq.TDL_KSK_CONTRACT_ID > 0)
                    {
                        CommonParam param = new CommonParam();
                        HisKskContractFilter filter = new HisKskContractFilter();
                        filter.ID = currentServiceReq.TDL_KSK_CONTRACT_ID;
                        var dataKskContract = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>>("api/HisKskContract/Get", ApiConsumers.MosConsumer, filter, param).SingleOrDefault();
                        txtKskContract.Text = dataKskContract.KSK_CONTRACT_CODE;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToPages()
        {
            try
            {
                FillDataPageGenaral();

                FillDataPageOccupational();

                FillDataPageOverEighteen();

                FillDataPageUnderEighteen();

                FillDataPageDriverCar();

                FillDataPagePeriodDriver();

                FillDataPageKSKOther();
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataCboRank(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEALTH_EXAM_RANK_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("HEALTH_EXAM_RANK_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEALTH_EXAM_RANK_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDataCboExamLoginName(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 100, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Họ tên", 150, 2));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Khoa", 150, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("LOGINNAME", "LOGINNAME", columnInfos, true, 400);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
                cbo.Properties.ImmediatePopup = true;
                cbo.Properties.PopupFormMinSize = new System.Drawing.Size(400, cbo.Properties.PopupFormSize.Height);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillNoteBMI(DevExpress.XtraEditors.SpinEdit spinHeight, DevExpress.XtraEditors.SpinEdit spinWeight, System.Windows.Forms.Label txtBMI)
        {
            try
            {
                decimal bmi = 0;
                if (spinHeight.Value != null && spinHeight.Value != 0)
                {
                    bmi = (spinWeight.Value) / ((spinHeight.Value / 100) * (spinHeight.Value / 100));
                }
                string displayBMI = Math.Round(bmi, 2) + "";
                if (bmi < 16)
                {
                    displayBMI += " (Gầy độ III)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.SKINNY.III", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (16 <= bmi && bmi < 17)
                {
                    displayBMI += " (Gầy độ II)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.SKINNY.II", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (17 <= bmi && bmi < (decimal)18.5)
                {
                    displayBMI += " (Gầy độ I)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.UCDHST.SKINNY.I", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if ((decimal)18.5 <= bmi && bmi < 25)
                {
                    displayBMI += " (Bình thường)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.NORMAL", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (25 <= bmi && bmi < 30)
                {
                    displayBMI += " (Thừa cân)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OVERWEIGHT", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (30 <= bmi && bmi < 35)
                {
                    displayBMI += " (Béo phì độ I)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OBESITY.I", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (35 <= bmi && bmi < 40)
                {
                    displayBMI += " (Béo phì độ II)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OBESITY.II", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (40 < bmi)
                {
                    displayBMI += " (Béo phì độ III)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OBESITY.III", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                txtBMI.Text = displayBMI;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                HisServiceReqKskExecuteV2SDO sdo = new HisServiceReqKskExecuteV2SDO();
                sdo.ServiceReqId = currentServiceReq.ID;
                sdo.RequestRoomId = currentModule.RoomId;
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    sdo.KskGeneral = new KskGeneralV2SDO();
                    sdo.KskGeneral.HisKskGeneral = new HIS_KSK_GENERAL();
                    sdo.KskGeneral.HisKskGeneral = GetValueGeneral();
                    sdo.KskGeneral.HisDhst = new HIS_DHST();
                    sdo.KskGeneral.HisDhst = GetValueDhstGeneral();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1)
                {
                    sdo.KskOverEighteen = new KskOverEighteenV2SDO();
                    sdo.KskOverEighteen.HisKskOverEighteen = new HIS_KSK_OVER_EIGHTEEN();
                    sdo.KskOverEighteen.HisKskOverEighteen = GetValueOverEighteen();
                    sdo.KskOverEighteen.HisDhst = new HIS_DHST();
                    sdo.KskOverEighteen.HisDhst = GetDhstOverighteen();
                    sdo.KskOverEighteen.HisPeriodDriverDitys = new System.Collections.Generic.List<HIS_PERIOD_DRIVER_DITY>();
                    sdo.KskOverEighteen.HisPeriodDriverDitys = GetDriverDityOverE();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2)
                {
                    sdo.KskUnderEighteen = new KskUnderEighteenV2SDO();
                    sdo.KskUnderEighteen.HisKskUnderEighteen = new HIS_KSK_UNDER_EIGHTEEN();
                    sdo.KskUnderEighteen.HisKskUnderEighteen = GetValueUnderEighteen();
                    sdo.KskUnderEighteen.HisDhst = new HIS_DHST();
                    sdo.KskUnderEighteen.HisDhst = GetDhstUnderEighteen();
                    sdo.KskUnderEighteen.HisKskUneiVatys = new System.Collections.Generic.List<HIS_KSK_UNEI_VATY>();
                    sdo.KskUnderEighteen.HisKskUneiVatys = GetUneiVaty();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 3)
                {
                    sdo.KskPeriodDriver = new KskPeriodDriverV2SDO();
                    sdo.KskPeriodDriver.HisKskPeriodDriver = new HIS_KSK_PERIOD_DRIVER();
                    sdo.KskPeriodDriver.HisKskPeriodDriver = GetValuePeriodDriver();
                    sdo.KskPeriodDriver.HisPeriodDriverDitys = new System.Collections.Generic.List<HIS_PERIOD_DRIVER_DITY>();
                    sdo.KskPeriodDriver.HisPeriodDriverDitys = GetDriverDity();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 4)
                {
                    sdo.HisKskDriverCar = new HIS_KSK_DRIVER_CAR();
                    sdo.HisKskDriverCar = GetValueDriverCar();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 5)
                {
                    sdo.HisKskOther = GetValueKSKOther();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 6)
                {
                    sdo.KskOccupationalV2 = new HisKskOccupationalV2SDO();
                    sdo.KskOccupationalV2.HisKskOccupational = new HIS_KSK_OCCUPATIONAL();
                    sdo.KskOccupationalV2.HisKskOccupational = GetValueOccupational();
                    sdo.KskOccupationalV2.HisDhst = new HIS_DHST();
                    sdo.KskOccupationalV2.HisDhst = GetValueDhstOccupational();
                }
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Debug("INPUT DATA:__api/HisServiceReq/KskExecuteV2 " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                KskExecuteResultV2SDO result = new BackendAdapter(param).Post<KskExecuteResultV2SDO>("api/HisServiceReq/KskExecuteV2", ApiConsumers.MosConsumer, sdo, param);
                Inventec.Common.Logging.LogSystem.Debug("INPUT DATA:__api/HisServiceReq/KskExecuteV2 " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                if (result != null)
                {       
                    success = true;
                    currentKskDriverCar = result.HisKskDriverCar;
                    currentKskGeneral = result.HisKskGeneral;
                    currentKskOverEight = result.HisKskOverEighteen;
                    currentKskPeriodDriver = result.HisKskPeriodDriver;
                    currentKskUnderEight = result.HisKskUnderEighteen;
                    currentKskOther = result.HisKskOther;
                    currentKsKOccupational = result.KskOccupational;
                    currentServiceReq = result.HisServiceReq;
                    btnPrint.Enabled = true;
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (System.Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                bool IsEnable = false;
                btnSave.Enabled = true;
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    if (currentKskGeneral != null)
                        IsEnable = true;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1)
                {
                    if (currentKskOverEight != null)
                        IsEnable = true;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2)
                {
                    if (currentKskUnderEight != null)
                        IsEnable = true;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 3)
                {
                    if (currentKskPeriodDriver != null)
                        IsEnable = true;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 4)
                {
                    if (currentKskDriverCar != null)
                        IsEnable = true;
                }
               
                else if (xtraTabControl1.SelectedTabPageIndex == 5)
                {
                    if (currentKskOther != null)
                    {
                        IsEnable = true;
                    }
                    if (chkKSKType1.Checked || chkKSKType2.Checked)
                    {
                        btnSave.Enabled = true;
                    }
                    else
                    {
                        btnSave.Enabled = false;
                    }
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 6)
                {
                    if (currentKsKOccupational != null)
                        IsEnable = true;
                }
                btnPrint.Enabled = IsEnable;
            }
            catch (System.Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled)
                    return;
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    if (currentKskGeneral != null)
                        PrintProcess(PRINT_TYPE.MPS000315);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1)
                {
                    if (currentKskOverEight != null)
                        PrintProcess(PRINT_TYPE.MPS000452);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2)
                {
                    if (currentKskUnderEight != null)
                        PrintProcess(PRINT_TYPE.MPS000453);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 3)
                {
                    if (currentKskPeriodDriver != null)
                        PrintProcess(PRINT_TYPE.MPS000454);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 4)
                {
                    if (currentKskDriverCar != null)
                        PrintProcess(PRINT_TYPE.MPS000455);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 5)
                {
                    if (currentKskOther != null)
                        PrintProcess(PRINT_TYPE.MPS000464);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 6)
                {
                    if (currentKsKOccupational != null)
                        PrintProcess(PRINT_TYPE.MPS000499);
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkKSKType1_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (chkKSKType1.Checked || chkKSKType2.Checked)
                {
                    btnSave.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkKSKType2_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (chkKSKType1.Checked || chkKSKType2.Checked)
                {
                    btnSave.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetSpecInformation(bool ReturnObject = true)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ContentSubclinical");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentServiceReq.TREATMENT_ID);
                    listArgs.Add(ReturnObject);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)DelegateSelectDataContentSubclinical);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegateSelectDataContentSubclinical(object data)
        {
            try
            {
                if (data != null && data is String)
                {
                    switch (NameSItem)
                    {
                        case ENameSItem.KET_LUAN_1:
                            txtResultSubclinical.Text = data.ToString();
                            break;
                        case ENameSItem.KHAC_XNM_2:
                            txtTestBloodOther2.Text = data.ToString();
                            break;
                        case ENameSItem.KHAC_XNNT_2:
                            txtTestUrineOther2.Text = data.ToString();
                            break;
                        case ENameSItem.CDHA_2:
                            txtResultDiim2.Text = data.ToString();
                            break;
                        case ENameSItem.KET_QUA_3:
                            txtResultSubclinical3.Text = data.ToString();
                            break;
                        case ENameSItem.KET_QUA_4:
                            txtResultSubclinical4.Text = data.ToString();
                            break;
                        case ENameSItem.KET_QUA_5:
                            txtResultSubclinical5.Text = data.ToString();
                            break;
                        case ENameSItem.KET_QUA_7:
                            txtResultSubclinical7.Text = data.ToString();
                            break;
                        default:
                            break;
                    }
                    NameSItem = null;
                    ReturnObject = true;
                }
                else if (data != null && data is List<ContentSubclinicalADO>)
                {
                    var item = (data as List<ContentSubclinicalADO>).LastOrDefault();
                    if (item != null)
                    {
                        switch (NameOtherItem)
                        {
                            case ENameOtherItem.SL_HC_2:
                                txtTestBloodHc2.Text = item.VALUE;
                                break;
                            case ENameOtherItem.SL_BC_2:
                                txtTestBloodBc2.Text = item.VALUE;
                                break;
                            case ENameOtherItem.SL_TC_2:
                                txtTestBloodTc2.Text = item.VALUE;
                                break;
                            case ENameOtherItem.DMA_2:
                                txtTestBloodGluco2.Text = item.VALUE;
                                break;
                            case ENameOtherItem.URE_2:
                                txtTestBloodUre2.Text = item.VALUE;
                                break;
                            case ENameOtherItem.CRE_2:
                                txtTestBloodCreatinin2.Text = item.VALUE;
                                break;
                            case ENameOtherItem.ASA_2:
                                txtTestBloodAsat2.Text = item.VALUE;
                                break;
                            case ENameOtherItem.ALA_2:
                                txtTestBloodAlat2.Text = item.VALUE;
                                break;
                            case ENameOtherItem.DUO_2:
                                txtTestUrineGluco2.Text = item.VALUE;
                                break;
                            case ENameOtherItem.PRO_2:
                                txtTestUrineProtein2.Text = item.VALUE;
                                break;
                            case ENameOtherItem.MOR_HER_4:
                                txtMorphineHeroin4.Text = item.VALUE;
                                break;
                            case ENameOtherItem.AMP_4:
                                txtTestAmphetamin4.Text = item.VALUE;
                                break;
                            case ENameOtherItem.MET_4:
                                txtTestMethamphetamin4.Text = item.VALUE;
                                break;
                            case ENameOtherItem.MAR_4:
                                txtTestMarijuna4.Text = item.VALUE;
                                break;
                            case ENameOtherItem.NDC_4:
                                txtTestConcentration4.Text = item.VALUE;
                                break;
                            case ENameOtherItem.MOR_HER_5:
                                txtMorphineHeroin5.Text = item.VALUE;
                                break;
                            case ENameOtherItem.AMP_5:
                                txtAmphetamin5.Text = item.VALUE;
                                break;
                            case ENameOtherItem.MET_5:
                                txtTestMethamphetamin5.Text = item.VALUE;
                                break;
                            case ENameOtherItem.MAR_5:
                                txtTestMarijuna5.Text = item.VALUE;
                                break;
                            case ENameOtherItem.NDC_5:
                                txtTestConcentration5.Text = item.VALUE;
                                break;
                            default:
                                break;
                        }
                    }
                    NameOtherItem = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ClearData_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if(e.Button.Kind == ButtonPredefines.Delete) {
                    var cbo = sender as GridLookUpEdit;
                    if (cbo != null)
                        cbo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnSaveAndSign_Click(object sender, EventArgs e)
        {
            try
            {
                IsSignEmr = true;
                btnSave_Click(null, null);
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboExamEntLoginName7_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                cboExamEntLoginName7.Text= null;
                cboExamEntLoginName7.EditValue = null;
            }
        }
    }
}
