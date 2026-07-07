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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base; 
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.Config;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.Resources;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel;
using System.Data;
using System.Data;
using System.Drawing;
using System.Drawing;
using System.Linq;
using System.Linq;
using System.Text;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
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
        private bool isLoginAdmin = false;
        private string currentLoginName = null;
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
            NDC_5,
            HER_4,
            MOR_4,
        }
        public ENameOtherItem? NameOtherItem { get; set; }
        public bool IsSignEmr { get; set; }
        #endregion

        Inventec.Desktop.Common.Modules.Module currentModule;
        public frmEnterKskInfomantionVer2(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_SERVICE_REQ hisServiceReq)
            :base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentServiceReq = hisServiceReq;
                this.currentModule = moduleData;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
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
                this.currentLoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.isLoginAdmin = IsLoginAdmin(this.currentLoginName);
                ShowInformationPatient();
                InitAvatarContextMenu();
                FillDataToPages();
                // Nhúng UC "Kết luận theo bệnh (ICD-10)" vào các tab + đổ dữ liệu từ HIS_KSK_GENERAL
                InitIcdConclusionUcForTabs();
                LoadIcdConclusionToUc();
                // Nhúng cụm chọn mã ICD tiền sử (R5) vào panel host đặt sẵn trong Designer (theo tên) — không vỡ layout.
                InitKskHistoryIcdForTabs();
                LoadKskHistoryIcdToUc();
                // Nhúng combo "Người khám" kết luận vào panel host (tab trên/dưới 18 tuổi).
                InitConcluderComboForTabs();
                LoadConcluderComboExt();
                // Tab trẻ <6t: mặc định kết luận sức khỏe = "Bình thường", kết luận ICD-10 = "Chưa phát hiện bất thường"
                // khi chưa có thông tin khám cũ (control kết luận còn trống). Gọi sau LoadIcdConclusionToUc để không bị đè.
                ApplyUnderSixConclusionDefaults();
                SetExamLoginComboDataSourceByPermission();
                SetTabDefault();
                SetEnableControl();
                // Wire event cho checkbox tự động lấy kết quả (KHÔNG lưu/khôi phục trạng thái tích).
                this.chkAutoTestIndex.CheckedChanged += new System.EventHandler(this.chkAutoTestIndex_CheckedChanged);
                // Chỉ enable checkbox ở tab có khám lâm sàng (tab có ô để load kết quả xét nghiệm).
                UpdateAutoTestIndexEnableByTab();
                // In đậm các tiêu đề mục (I/II/III, 1/2/3, 2.1...) ở mọi tab.
                BoldAllSectionHeaders(this);
                // Thêm nút Delete cho mọi GridLookUpEdit để bấm vào là clear được giá trị ô đó.
                InitClearButtonForGridLookUpEdits(this);
                WaitingManager.Hide();
            }
            catch (System.Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // ===== Kết luận theo bệnh (ICD-10) — UC tái sử dụng nhúng vào các tab =====
        // Key = tab index → panel host (đặt sẵn trong Designer ở vùng kết luận mỗi tab).
        private System.Collections.Generic.Dictionary<int, UcKskConclusionIcd> dicIcdConclusionUc = new System.Collections.Generic.Dictionary<int, UcKskConclusionIcd>();

        /// <summary>
        /// Nhúng UcKskConclusionIcd (Kết luận theo bệnh ICD-10) vào panel host đã đặt sẵn trong Designer
        /// ở vùng kết luận của từng tab. Chạy 1 lần lúc Load.
        /// </summary>
        private void InitIcdConclusionUcForTabs()
        {
            try
            {
                var hosts = new System.Collections.Generic.Dictionary<int, System.Windows.Forms.Control>()
                {
                    { 0, this.panel1 }, // Ksk định kỳ (General)
                    { 1, this.panel2 }, // Ksk trên 18 tuổi
                    { 2, this.panel3 }, // Ksk dưới 18 tuổi
                    { 3, this.panel4 }, // Ksk lái xe
                    { 4, this.panel5 }, // Ksk lái xe ô tô
                    { 5, this.panel6 }, // KSK khác
                    { 6, this.panel7 }, // Ksk nghề nghiệp
                    { 7, this.panel8 }, // Trẻ em dưới 6 tuổi (panel mới — thay vùng ICD inline)
                };
                foreach (var kv in hosts)
                {
                    if (kv.Value == null || dicIcdConclusionUc.ContainsKey(kv.Key)) continue;
                    UcKskConclusionIcd uc = new UcKskConclusionIcd();
                    uc.Dock = System.Windows.Forms.DockStyle.Fill;
                    kv.Value.Controls.Add(uc);
                    uc.InitUc();
                    dicIcdConclusionUc[kv.Key] = uc;
                }
            }
            catch (System.Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Đổ ICD-10 từ HIS_KSK_GENERAL của lượt khám vào các UC đã nhúng.</summary>
        private void LoadIcdConclusionToUc()
        {
            try
            {
                if (currentKskGeneral == null) return;
                foreach (var uc in dicIcdConclusionUc.Values)
                    if (uc != null) uc.LoadFromGeneral(currentKskGeneral);
            }
            catch (System.Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void SetEnableControl()
        {
            try
            {
                if (HisConfigCFG.DisablePartExamByExecutor == "1")
                {
                    // Tài khoản đăng nhập là admin thì được phép sửa (enable) thông tin khám của bất kỳ người khám nào.
                    // (this.isLoginAdmin / this.currentLoginName đã được tính sẵn ở frmEnterKskInfomantionVer2_Load)
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
                        LoginNameEnableControl(currentKskGeneral.EXAM_OBSTETRIC_LOGINNAME, txtExamObstetric, cboExamObstetricRank); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám sản phụ khoa.
                        LoginNameEnableControl(currentKskGeneral.EXAM_SUBCLINICAL_LOGINNAME, txtResultSubclinical, txtNoteSubclinical); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.

                        // Disable combo chính (người khám) của từng vùng nếu không phải người khám và không phải admin.
                        LoginNameEnableControl(currentKskGeneral.EXAM_CIRCULATION_LOGINNAME, cboExamCirculationLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_RESPIRATORY_LOGINNAME, cboExamRespiratoryLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_DIGESTION_LOGINNAME, cboExamDigestionLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_KIDNEY_UROLOGY_LOGINNAME, cboExamKidneyUrologyLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_NEUROLOGICAL_LOGINNAME, cboExamNeurologicalLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_MUSCLE_BONE_LOGINNAME, cboExamMuscleBoneLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_ENT_LOGINNAME, cboExamEntLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_STOMATOLOGY_LOGINNAME, cboExamStomatologyLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_EYE_LOGINNAME, cboExamEyeLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_OEND_LOGINNAME, cboExamOendLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_MENTAL_LOGINNAME, cboExamMentalLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_DERMATOLOGY_LOGINNAME, cboExamDermatologyLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_SURGERY_LOGINNAME, cboExamSurgeryLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_OBSTETRIC_LOGINNAME, cboExamObstetricLoginName);
                        LoginNameEnableControl(currentKskGeneral.EXAM_SUBCLINICAL_LOGINNAME, cboExamSubclinicalLoginName);

                        // Khám thể lực (DHST): disable chiều cao/cân nặng/mạch/huyết áp/phân loại theo người khám thể lực (EXECUTE_LOGINNAME).
                        string dhstGeneralExecuteLogin = (dhstGeneral != null) ? dhstGeneral.EXECUTE_LOGINNAME : null;
                        LoginNameEnableControl(dhstGeneralExecuteLogin, spnHeight);
                        LoginNameEnableControl(dhstGeneralExecuteLogin, spnWeight);
                        LoginNameEnableControl(dhstGeneralExecuteLogin, spnPulse);
                        LoginNameEnableControl(dhstGeneralExecuteLogin, spnBloodPressureMax);
                        LoginNameEnableControl(dhstGeneralExecuteLogin, spnBloodPressureMin);
                        LoginNameEnableControl(dhstGeneralExecuteLogin, cboDhstRank);
                        LoginNameEnableControl(dhstGeneralExecuteLogin, cboExecuteLoginName);

                        // Kết luận: disable phân loại SK/bệnh tật/ngày kết luận theo người kết luận (CONCLUDER_LOGINNAME).
                        LoginNameEnableControl(currentKskGeneral.CONCLUDER_LOGINNAME, cboHealthExamRank);
                        LoginNameEnableControl(currentKskGeneral.CONCLUDER_LOGINNAME, txtDiseases);
                        LoginNameEnableControl(currentKskGeneral.CONCLUDER_LOGINNAME, dteConclusionTimeGeneral);
                        LoginNameEnableControl(currentKskGeneral.CONCLUDER_LOGINNAME, cboConcluderLoginName);
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

                        // Disable combo chính (người khám) của từng vùng nếu không phải người khám và không phải admin.
                        LoginNameEnableControl(currentKskOverEight.EXAM_CIRCULATION_LOGINNAME, cboExamCirculationLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_RESPIRATORY_LOGINNAME, cboExamRespiratoryLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_DIGESTION_LOGINNAME, cboExamDigestionLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_KIDNEY_UROLOGY_LOGINNAME, cboExamKidneyUrologyLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_NEUROLOGICAL_LOGINNAME, cboExamNeurologicalLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_MUSCLE_BONE_LOGINNAME, cboExamMuscleBoneLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_ENT_LOGINNAME, cboExamEntLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_STOMATOLOGY_LOGINNAME, cboExamStomatologyLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_EYE_LOGINNAME, cboExamEyeLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_OEND_LOGINNAME, cboExamOendLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_MENTAL_LOGINNAME, cboExamMentalLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_DERMATOLOGY_LOGINNAME, cboExamDermatologyLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_SURGERY_LOGINNAME, cboExamSurgeryLoginName2);
                        LoginNameEnableControl(currentKskOverEight.EXAM_OBSTETRIC_LOGINNAME, cboExamObstetricLoginName2);
                        LoginNameEnableControl(currentKskOverEight.TEST_URINE_LOGINNAME, cboTestUrineLoginName);
                        LoginNameEnableControl(currentKskOverEight.DIIM_LOGINNAME, cboDiimLoginName2);
                        LoginNameEnableControl(currentKskOverEight.TEST_BLOOD, cboTestBloodLoginName);

                        // Khám thể lực (DHST): disable chiều cao/cân nặng/mạch/huyết áp/phân loại theo người khám thể lực.
                        string dhstOverEightExecuteLogin = (dhstOverEighteen != null) ? dhstOverEighteen.EXECUTE_LOGINNAME : null;
                        LoginNameEnableControl(dhstOverEightExecuteLogin, spnHeight2);
                        LoginNameEnableControl(dhstOverEightExecuteLogin, spnWeight2);
                        LoginNameEnableControl(dhstOverEightExecuteLogin, spnPulse2);
                        LoginNameEnableControl(dhstOverEightExecuteLogin, spnBloodPressureMax2);
                        LoginNameEnableControl(dhstOverEightExecuteLogin, spnBloodPressureMin2);
                        LoginNameEnableControl(dhstOverEightExecuteLogin, cboDhstRank2);
                        LoginNameEnableControl(dhstOverEightExecuteLogin, cboExecuteLoginName2);
                    }
                    if (this.currentKskUnderEight != null)
                    {
                        LoginNameEnableControl(currentKskUnderEight.EXAM_CIRCULATION_LOGINNAME, txtExamCirculation3, cboExamCirculationRank3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tuần hoàn.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_RESPIRATORY_LOGINNAME, txtExamRespiratory3, cboExamRespiratoryRank3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám hô hấp.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_DIGESTION_LOGINNAME, txtExamDigestion3, cboExamDigestionRank3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tiêu hóa.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_KIDNEY_UROLOGY_LOGINNAME, txtExamKidneyUrology3, cboExamKidneyUrologyRank3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thận tiết niệu.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_NEURO_MENTAL_LOGINNAME, txtExamNeuroMental3, cboExamNeuroMental3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thần kinh.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_MENTAL_LOGINNAME, txtExamMental3, cboExamMentalRank3); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tâm thần.
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

                        // Disable combo chính (người khám) của từng vùng nếu không phải người khám và không phải admin.
                        LoginNameEnableControl(currentKskUnderEight.EXAM_CIRCULATION_LOGINNAME, cboExamCirculationLoginName3);
                        LoginNameEnableControl(currentKskUnderEight.EXAM_RESPIRATORY_LOGINNAME, cboExamRespiratoryLoginName3);
                        LoginNameEnableControl(currentKskUnderEight.EXAM_DIGESTION_LOGINNAME, cboExamDigestionLoginName3);
                        LoginNameEnableControl(currentKskUnderEight.EXAM_KIDNEY_UROLOGY_LOGINNAME, cboExamKidneyUrologyLoginName3);
                        LoginNameEnableControl(currentKskUnderEight.EXAM_NEURO_MENTAL_LOGINNAME, cboExamNeuroMentalLoginName3);
                        LoginNameEnableControl(currentKskUnderEight.EXAM_MENTAL_LOGINNAME, cboExamMentalLoginName3);
                        LoginNameEnableControl(currentKskUnderEight.EXAM_CLINICAL_OTHER_LOGINNAME, cboExamClinicalOtherLoginName3);
                        LoginNameEnableControl(currentKskUnderEight.EXAM_ENT_LOGINNAME, cboExamEntLoginName3);
                        LoginNameEnableControl(currentKskUnderEight.EXAM_STOMATOLOGY_LOGINNAME, cboExamStomatologyLoginName3);
                        LoginNameEnableControl(currentKskUnderEight.EXAM_EYE_LOGINNAME, cboExamEyeLoginName3);
                        LoginNameEnableControl(currentKskUnderEight.EXAM_SUBCLINICAL_LOGINNAME, cboExamSubclinicalLoginName3);

                        // Khám thể lực (DHST): disable chiều cao/cân nặng/mạch/huyết áp/phân loại theo người khám thể lực.
                        string dhstUnderEightExecuteLogin = (dhstUnderEighteen != null) ? dhstUnderEighteen.EXECUTE_LOGINNAME : null;
                        LoginNameEnableControl(dhstUnderEightExecuteLogin, spnHeight3);
                        LoginNameEnableControl(dhstUnderEightExecuteLogin, spnWeight3);
                        LoginNameEnableControl(dhstUnderEightExecuteLogin, spnPulse3);
                        LoginNameEnableControl(dhstUnderEightExecuteLogin, spnBloodPressureMax3);
                        LoginNameEnableControl(dhstUnderEightExecuteLogin, spnBloodPressureMin3);
                        LoginNameEnableControl(dhstUnderEightExecuteLogin, cboDhstRank3);
                        LoginNameEnableControl(dhstUnderEightExecuteLogin, cboExecuteLoginName3);
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

                        // Disable combo chính (người khám) của từng vùng nếu không phải người khám và không phải admin.
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_CARDIOVASCULAR_LOGINNAME, cboExamCardiovascularLoginName4);
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_RESPIRATORY_LOGINNAME, cboExamRespiratoryLoginName4);
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_NEUROLOGICAL_LOGINNAME, cboExamNeurologicalLoginName4);
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_MUSCLE_BONE_LOGINNAME, cboExamMuscleBoneLoginName4);
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_ENT_LOGINNAME, cboExamEntLoginName4);
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_EYE_LOGINNAME, cboExamEyeLoginName4);
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_MENTAL_LOGINNAME, cboExamMentalLoginName4);
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_OEND_LOGINNAME, cboExamOendLoginName4);
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_MATERNITY_LOGINNAME, cboExamMaternityLoginName4);
                        LoginNameEnableControl(currentKskPeriodDriver.EXAM_SUBCLINICAL_LOGINNAME, cboExamSubclinicalLoginName4);
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

                        // Disable combo chính (người khám) của từng vùng nếu không phải người khám và không phải admin.
                        LoginNameEnableControl(currentKskDriverCar.EXAM_CARDIOVASCULAR_LOGINNAME, cboExamCardiovascularLoginName5);
                        LoginNameEnableControl(currentKskDriverCar.EXAM_ENT_LOGINNAME, cboExamEntLoginName5);
                        LoginNameEnableControl(currentKskDriverCar.EXAM_EYE_LOGINNAME, cboExamEyeLoginName5);
                        LoginNameEnableControl(currentKskDriverCar.EXAM_SUBCLINICAL_LOGINNAME, cboExamSubclinicalLoginName5);
                    }
                    if (this.currentKsKOccupational != null)
                    {
                        LoginNameEnableControl(currentKsKOccupational.EXAM_CIRCULATION_LOGINNAME, txtExamCirculation7, cboExamCirculationRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tuần hoàn.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_RESPIRATORY_LOGINNAME, txtExamRespiratory7, cboExamRespiratoryRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám hô hấp.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_DIGESTION_LOGINNAME, txtExamDigestion7, cboExamDigestionRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tiêu hóa.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_KIDNEY_UROLOGY_LOGINNAME, txtExamKidneyUrology7, cboExamKidneyUrologyRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thận tiết niệu.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_NEUROLOGICAL_LOGINNAME, txtExamNeurological7, cboExamNeurologicalRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám thần kinh.
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
                        LoginNameEnableControl(currentKsKOccupational.EXAM_OEND_LOGINNAME, txtExamOend7, cboExamOendRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám nội tiết.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_MENTAL_LOGINNAME, txtExamMental7, cboExamMentalRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám tâm thần.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_DERMATOLOGY_LOGINNAME, txtExamDernatology7, cboExamDernatologyRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám da liễu.
                        LoginNameEnableControl(currentKsKOccupational.NOTE_CLINICAL, txtNoteSubclinical);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_SURGERY_LOGINNAME, txtExamSurgery7,cboExamSurgeryRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable thông tin ngoại khoa
                        LoginNameEnableControl(currentKsKOccupational.EXAM_OBSTETRIC_LOGINNAME, txtExamObstetric7, cboExamObstetricRank7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám sản phụ khoa.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_SUBCLINICAL_LOGINNAME, txtResultSubclinical7, txtNoteSubclinical7); // có dữ liệu và khác với tài khoản đăng nhập thì disable các trường thông tin khám cận lâm sàng.

                        // Disable combo chính (người khám) của từng vùng nếu không phải người khám và không phải admin.
                        LoginNameEnableControl(currentKsKOccupational.EXAM_CIRCULATION_LOGINNAME, cboExamCirculationLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_RESPIRATORY_LOGINNAME, cboExamRespiratoryLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_DIGESTION_LOGINNAME, cboExamDigestionLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_KIDNEY_UROLOGY_LOGINNAME, cboExamKidneyUrologyLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_OEND_LOGINNAME, cboExamOendLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_MUSLE_BONE_LOGINNAME, cboExamMuscleBoneLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_NEUROLOGICAL_LOGINNAME, cboExamNeurologicalLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_MENTAL_LOGINNAME, cboExamMentalLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_SURGERY_LOGINNAME, cboExamSurgeryLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_DERMATOLOGY_LOGINNAME, cboExamDermatologyLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_OBSTETRIC_LOGINNAME, cboExamObstetricLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_EYE_LOGINNAME, cboExamEyeLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_ENT_LOGINNAME, cboExamEntLoginName7);
                        LoginNameEnableControl(currentKsKOccupational.EXAM_SUBCLINICAL_LOGINNAME, cboExamSubclinicalLoginName7);

                        // Khám thể lực (DHST): disable chiều cao/cân nặng/mạch/huyết áp/phân loại theo người khám thể lực.
                        string dhstOccupationalExecuteLogin = (dhstOccupational != null) ? dhstOccupational.EXECUTE_LOGINNAME : null;
                        LoginNameEnableControl(dhstOccupationalExecuteLogin, spnHeight7);
                        LoginNameEnableControl(dhstOccupationalExecuteLogin, spnWeight7);
                        LoginNameEnableControl(dhstOccupationalExecuteLogin, spnPulse7);
                        LoginNameEnableControl(dhstOccupationalExecuteLogin, spnBloodPressureMax7);
                        LoginNameEnableControl(dhstOccupationalExecuteLogin, spnBloodPressureMin7);
                        LoginNameEnableControl(dhstOccupationalExecuteLogin, cboDhstRank7);
                        LoginNameEnableControl(dhstOccupationalExecuteLogin, cboExecuteLoginName7);

                        // Kết luận: disable phân loại SK/bệnh tật/ngày kết luận theo người kết luận (CONCLUDER_LOGINNAME).
                        LoginNameEnableControl(currentKsKOccupational.CONCLUDER_LOGINNAME, cboHealthExamRank7);
                        LoginNameEnableControl(currentKsKOccupational.CONCLUDER_LOGINNAME, txtDiseases7);
                        LoginNameEnableControl(currentKsKOccupational.CONCLUDER_LOGINNAME, dteConclusionTimeOccupational);
                        LoginNameEnableControl(currentKsKOccupational.CONCLUDER_LOGINNAME, cboConcluderLoginName7);
                    }
                    if (this.currentKskUnderSixEf != null)
                    {
                        // Trẻ em dưới 6 tuổi (tab 8): nếu 1 mục đã có người khám khác (LOGINNAME) và không phải admin
                        // → khóa TOÀN BỘ control trong mục đó (combo BS + các radio/ghi chú), không cho sửa.
                        LoginNameEnableControl(currentKskUnderSixEf.SKIN_LOGINNAME,
                            cboExamDrSkin8, memClinicalObservation8, rdoSkinColor8, rdoPalmEval8, memSkinNote8); // 1. Da (gồm cả Quan sát chung)
                        LoginNameEnableControl(currentKskUnderSixEf.HEADNECK_LOGINNAME,
                            cboExamDrHeadNeck8, rdoFontanel8, rdoHeadShape8, rdoNeckMotion8, rdoHeadAbnormalMass8, memHeadNeckNote8); // 2.1 Đầu - cổ
                        LoginNameEnableControl(currentKskUnderSixEf.EYE_LOGINNAME,
                            cboExamDrEye8, rdoEyePosition8, rdoEyelidConjunctiva8, rdoPupil8, rdoStrabismus8, memEyeNote8); // 2.2 Mắt
                        LoginNameEnableControl(currentKskUnderSixEf.EAR_LOGINNAME,
                            cboExamDrEar8, rdoEarEardrum8, rdoSoundResponse8, rdoEarSwelling8, rdoEarDischarge8, memEarNote8); // 2.3 Tai
                        LoginNameEnableControl(currentKskUnderSixEf.NOSETHROAT_LOGINNAME,
                            cboExamDrNoseThroat8, rdoNoseShape8, rdoRunnyNose8, rdoStuffyNose8, rdoThroat8, memNoseThroatNote8); // 2.4 Mũi - họng
                        LoginNameEnableControl(currentKskUnderSixEf.MOUTHTEETH_LOGINNAME,
                            cboExamDrMouthTeeth8, rdoMouthShape8, rdoNeonatalTeeth8, rdoTongueShape8, rdoTongueTie8, rdoOralThrush8, rdoSmallChin8, rdoToothDecay8, memMouthTeethNote8); // 2.5 Miệng - răng
                        LoginNameEnableControl(currentKskUnderSixEf.RESP_LOGINNAME,
                            cboExamDrResp8, rdoIrregularBreath8, rdoChestRetraction8, rdoAbnormalBreathSound8, rdoRespFailureSign8, rdoLungAuscultation8, memRespNote8); // 3. Hô hấp
                        LoginNameEnableControl(currentKskUnderSixEf.CARDIO_LOGINNAME,
                            cboExamDrCardio8, rdoApexPosition8, rdoPeripheralPulse8, rdoHeartAuscultation8, memCardioNote8); // 4. Tim mạch
                        LoginNameEnableControl(currentKskUnderSixEf.ABDOMEN_LOGINNAME,
                            cboExamDrAbdomen8, rdoAbdomenNavel8, rdoHepatosplenomegaly8, rdoAbdomenMass8, rdoAnus8, rdoGenitalia8, memAbdomenNote8); // 5. Bụng và cơ quan sinh dục
                        LoginNameEnableControl(currentKskUnderSixEf.MUSCULOSKELETAL_LOGINNAME,
                            cboExamDrMusc8, rdoAsymmetricMovement8, rdoSuckingReflex8, rdoGraspReflex8, rdoMoroReflex8, rdoMuscleTone8, rdoHipJoint8, rdoMuscleReflex8, rdoSpineCheck8, rdoLimbsJoints8, rdoGait8, rdoRicketsSignNeuro8, memMusculoskeletalNote8); // 6. Cơ xương và thần kinh

                        // Các panel NGOÀI khám lâm sàng (Hành chính/Sinh tồn/Dinh dưỡng/Phát triển/Tiêm chủng) + phần Kết luận:
                        // khóa theo BÁC SĨ KẾT LUẬN ở tab VII (CONCLUDER_LOGINNAME nằm trên HIS_KSK_GENERAL).
                        string underSixConcluderLogin = (currentKskGeneral != null) ? currentKskGeneral.CONCLUDER_LOGINNAME : null;
                        // I. Hành chính
                        //LoginNameEnableControl(underSixConcluderLogin,
                        //    rdoIsPrematureBirth8, txtEthnic8, txtResidence8, txtAccompanyPersonName8,
                        //    rdoAccompanyRelationship8, txtAccompanyRelationshipOther8, memHistoryPersonal8, memHistoryFamily8, rdoIsTbContact8);
                        //// II. Sinh tồn
                        //LoginNameEnableControl(underSixConcluderLogin,
                        //    spnTemperature8, rdoTemperatureEval8, spnPulse8, rdoPulseEval8, spnRespiratoryRate8, rdoRespiratoryEval8);
                        //// III. Dinh dưỡng
                        //LoginNameEnableControl(underSixConcluderLogin,
                        //    spnBodyLength8, spnBodyLengthAgeSd8, spnWeight8, spnWeightAgeSd8, spnHeadCircumference8, rdoHeadCircEval8, spnArmCircumference8,
                        //    chkIsNutritionalEdema8, chkIsAnemiaSign8, chkIsRicketsSign8, chkIsMalnutrition8, chkIsOverweight8);
                        //// IV. Phát triển tinh thần - vận động
                        //LoginNameEnableControl(underSixConcluderLogin,
                        //    rdoMentalDevNormal8, rdoMotorDevNormal8, rdoAutismRisk8);
                        //// V. Tiêm chủng
                        //LoginNameEnableControl(underSixConcluderLogin,
                        //    rdoVaccineTb8, rdoVaccineHepb18, rdoVaccineFullByAge8);
                        // VII. Kết luận & tư vấn
                        LoginNameEnableControl(underSixConcluderLogin,
                            rdoConclusionHealth8, memConclusionDetail8, memAdviceNextExam8, cboHealthExamRank8, cboConcluder8, rdoIcdConclusion8, btnChooseIcd8);
                        // "Kết luận theo bệnh (ICD-10)" hiển thị bằng UC nhúng (dicIcdConclusionUc[7]) đè lên panel8 →
                        // 2 control inline rdoIcdConclusion8/btnChooseIcd8 bị UC che, phải disable cả UC theo bác sĩ kết luận.
                        DisableIcdConclusionUcByLogin(7, underSixConcluderLogin);
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
                // Chỉ cho phép enable nếu tài khoản đăng nhập là người khám (data == loginName) hoặc là admin.
                // Ngược lại (không phải người khám và không phải admin) -> disable thông tin khám của người đó.
                if (!string.IsNullOrEmpty(data) && data != loginName && !this.isLoginAdmin)
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

        /// <summary>
        /// Disable TOÀN BỘ control của 1 mục khám khi mục đó đã có người khám khác
        /// (data != tài khoản đăng nhập) và không phải admin.
        /// Dùng cho tab trẻ dưới 6 tuổi: chọn BS khám 1 mục (vd Da) → khóa hết control trong mục đó,
        /// không chỉ riêng combo chọn bác sĩ.
        /// </summary>
        private void LoginNameEnableControl(string data, params System.Windows.Forms.Control[] controls)
        {
            try
            {
                if (controls == null || controls.Length == 0) return;
                var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (!string.IsNullOrEmpty(data) && data != loginName && !this.isLoginAdmin)
                {
                    foreach (var c in controls)
                        if (c != null) c.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Disable UC "Kết luận theo bệnh (ICD-10)" (dicIcdConclusionUc[tabIndex]) khi vùng kết luận
        /// đã có người kết luận khác (data != tài khoản đăng nhập) và không phải admin.
        /// UC là UserControl nhúng → set Enabled=false sẽ khóa toàn bộ control con (radio + ô chọn ICD + nút).
        /// </summary>
        private void DisableIcdConclusionUcByLogin(int tabIndex, string data)
        {
            try
            {
                if (!dicIcdConclusionUc.ContainsKey(tabIndex) || dicIcdConclusionUc[tabIndex] == null) return;
                var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (!string.IsNullOrEmpty(data) && data != loginName && !this.isLoginAdmin)
                    dicIcdConclusionUc[tabIndex].Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiểm tra tài khoản đăng nhập có phải admin hay không:
        /// - Tài khoản toàn quyền theo phân quyền ACS (GlobalVariables.AcsAuthorizeSDO.IsFull), hoặc
        /// - Nhân viên có cờ HIS_EMPLOYEE.IS_ADMIN = 1.
        /// Admin thì luôn được hiển thị/sửa đầy đủ (không bị lọc/disable theo người khám).
        /// </summary>
        private bool IsLoginAdmin(string loginName)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(loginName))
                {
                    var employee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>()
                        .FirstOrDefault(p => p.LOGINNAME == loginName.Trim());
                    if (employee != null && employee.IS_ADMIN == (short)1)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
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

                    // Avatar: ưu tiên TDL_PATIENT_AVATAR_URL trên HIS_SERVICE_REQ;
                    // nếu trống thì lấy AVATAR_URL từ HIS_PATIENT; nếu vẫn trống thì dùng ảnh mặc định.
                    string avatarUrl = currentServiceReq.TDL_PATIENT_AVATAR_URL;
                    if (String.IsNullOrEmpty(avatarUrl))
                    {
                        avatarUrl = GetPatientAvatarUrl(currentServiceReq.TDL_PATIENT_ID);
                    }

                    if (!String.IsNullOrEmpty(avatarUrl))
                    {
                        System.IO.MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(avatarUrl);
                        pictureEdit1.Image = Image.FromStream(stream);
                        pictureEdit1.Image.Tag = avatarUrl;
                    }
                    else
                    {
                        string pathLocal = GetPathDefault();
                        pictureEdit1.Image = Image.FromFile(pathLocal);
                    }

                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private string GetPathDefault()
        {
            string imageDefaultPath = string.Empty;
            try
            {
                string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                imageDefaultPath = localPath + "\\Img\\ImageStorage\\notImage.jpg";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return imageDefaultPath;
        }

        /// <summary>
        /// Lấy AVATAR_URL từ HIS_PATIENT theo patientId — dùng khi HIS_SERVICE_REQ.TDL_PATIENT_AVATAR_URL trống.
        /// </summary>
        private string GetPatientAvatarUrl(long patientId)
        {
            try
            {
                if (patientId <= 0) return null;
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientFilter filter = new MOS.Filter.HisPatientFilter();
                filter.ID = patientId;
                var patients = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, null);
                if (patients != null && patients.Count > 0)
                    return patients[0].AVATAR_URL;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
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

                FillDataPageUnderSix();
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
        //private void SetDataCboExamLoginName(DevExpress.XtraEditors.GridLookUpEdit cbo)
        //{
        //    try
        //    {
        //        CommonParam param = new CommonParam();
        //        var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 100, 1));
        //        columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Họ tên", 150, 2));
        //        columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Khoa", 150, 3));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("LOGINNAME", "TDL_USERNAME", columnInfos, true, 400);
        //        ControlEditorLoader.Load(cbo, data, controlEditorADO);
        //        cbo.Properties.ImmediatePopup = true;
        //        cbo.Properties.PopupFormMinSize = new System.Drawing.Size(400, cbo.Properties.PopupFormSize.Height);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        private void SetDataCboExamLoginName(DevExpress.XtraEditors.GridLookUpEdit cbo, bool onlyCurrentLogin = false)
        {
            try
            {
                CommonParam param = new CommonParam();
                var loginName = this.currentLoginName ?? Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                            .Get<V_HIS_EMPLOYEE>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            .ToList();

                // Chỉ hiển thị đúng tài khoản đang đăng nhập (dùng khi ô người khám chưa có dữ liệu và không phải admin).
                if (onlyCurrentLogin)
                {
                    data = data.Where(o => o.LOGINNAME == loginName).ToList();
                }

                // Đưa tài khoản đang đăng nhập lên dòng đầu tiên của danh sách (OrderBy ổn định nên giữ nguyên thứ tự còn lại).
                data = data.OrderByDescending(o => o.LOGINNAME == loginName).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 100, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Họ tên", 150, 2));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Khoa", 150, 3));

                ControlEditorADO controlEditorADO = new ControlEditorADO("LOGINNAME", "TDL_USERNAME", columnInfos, true, 400);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);

                cbo.Properties.ValueMember = "LOGINNAME";
                cbo.Properties.DisplayMember = "TDL_USERNAME";
                cbo.Properties.NullText = "";
                cbo.Properties.ImmediatePopup = true;
                cbo.Properties.PopupFormMinSize = new System.Drawing.Size(400, cbo.Properties.PopupFormSize.Height);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Giới hạn danh sách combobox "Người khám" theo phân quyền (chạy sau khi đã fill dữ liệu).
        /// - Ô người khám đã có dữ liệu (đang bị disable do đã có người khám): giữ nguyên load toàn bộ employee để hiển thị đúng người khám.
        /// - Ô người khám chưa có dữ liệu: chỉ hiển thị đúng tài khoản đang đăng nhập.
        /// - Riêng admin: luôn hiển thị đủ danh sách người khám.
        /// Chỉ áp dụng khi bật cấu hình DisablePartExamByExecutor = "1" (cùng phạm vi với tính năng disable theo người khám).
        /// </summary>
        private void SetExamLoginComboDataSourceByPermission()
        {
            try
            {
                if (HisConfigCFG.DisablePartExamByExecutor != "1")
                    return;
                if (this.isLoginAdmin)
                    return; // admin: giữ đủ danh sách như mặc định

                List<DevExpress.XtraEditors.GridLookUpEdit> examLoginCombos = new List<DevExpress.XtraEditors.GridLookUpEdit>()
                {
                    // Tab Khám tổng quát
                    cboExamCirculationLoginName, cboExamRespiratoryLoginName, cboExamDigestionLoginName,
                    cboExamKidneyUrologyLoginName, cboExamNeurologicalLoginName, cboExamMuscleBoneLoginName,
                    cboExamEntLoginName, cboExamStomatologyLoginName, cboExamEyeLoginName, cboExamOendLoginName,
                    cboExamMentalLoginName, cboExamDermatologyLoginName, cboExamSurgeryLoginName,
                    cboExamObstetricLoginName, cboExamSubclinicalLoginName,
                    // Tab trên 18 tuổi
                    cboExamCirculationLoginName2, cboExamRespiratoryLoginName2, cboExamDigestionLoginName2,
                    cboExamKidneyUrologyLoginName2, cboExamNeurologicalLoginName2, cboExamMuscleBoneLoginName2,
                    cboExamEntLoginName2, cboExamStomatologyLoginName2, cboExamEyeLoginName2, cboExamOendLoginName2,
                    cboExamMentalLoginName2, cboExamDermatologyLoginName2, cboExamSurgeryLoginName2,
                    cboExamObstetricLoginName2, cboTestUrineLoginName, cboDiimLoginName2, cboTestBloodLoginName,
                    // Tab dưới 18 tuổi
                    cboExamCirculationLoginName3, cboExamRespiratoryLoginName3, cboExamDigestionLoginName3,
                    cboExamKidneyUrologyLoginName3, cboExamNeuroMentalLoginName3, cboExamClinicalOtherLoginName3,
                    cboExamEntLoginName3, cboExamStomatologyLoginName3, cboExamEyeLoginName3, cboExamSubclinicalLoginName3,
                    // Tab lái xe định kỳ
                    cboExamCardiovascularLoginName4, cboExamRespiratoryLoginName4, cboExamNeurologicalLoginName4,
                    cboExamMuscleBoneLoginName4, cboExamEntLoginName4, cboExamEyeLoginName4, cboExamMentalLoginName4,
                    cboExamOendLoginName4, cboExamMaternityLoginName4, cboExamSubclinicalLoginName4,
                    // Tab lái xe
                    cboExamCardiovascularLoginName5, cboExamEntLoginName5, cboExamEyeLoginName5, cboExamSubclinicalLoginName5,
                    // Tab khám nghề nghiệp
                    cboExamCirculationLoginName7, cboExamRespiratoryLoginName7, cboExamDigestionLoginName7,
                    cboExamKidneyUrologyLoginName7, cboExamOendLoginName7, cboExamMuscleBoneLoginName7,
                    cboExamNeurologicalLoginName7, cboExamMentalLoginName7, cboExamSurgeryLoginName7,
                    cboExamDermatologyLoginName7, cboExamObstetricLoginName7, cboExamEyeLoginName7,
                    cboExamEntLoginName7, cboExamSubclinicalLoginName7,
                };

                foreach (var cbo in examLoginCombos)
                {
                    if (cbo == null)
                        continue;
                    string value = cbo.EditValue == null ? "" : cbo.EditValue.ToString();
                    // Chưa có dữ liệu người khám -> chỉ cho chọn tài khoản đang đăng nhập.
                    if (string.IsNullOrEmpty(value))
                    {
                        SetDataCboExamLoginName(cbo, true);
                    }
                }
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

        /// <summary>
        /// Kiểm tra bảng DHST có dữ liệu sinh tồn hay không.
        /// Chỉ tính các trường số đo (mạch, nhiệt độ, nhịp thở, huyết áp, chiều cao, cân nặng),
        /// KHÔNG tính người đo (EXECUTE_LOGINNAME/USERNAME) vì đó là metadata.
        /// </summary>
        private bool HasDhstData(HIS_DHST dhst)
        {
            if (dhst == null) return false;
            return dhst.BLOOD_PRESSURE_MAX != null
                || dhst.BLOOD_PRESSURE_MIN != null
                || dhst.HEIGHT != null
                || dhst.WEIGHT != null
                || dhst.PULSE != null
                || dhst.TEMPERATURE != null
                || dhst.BREATH_RATE != null;
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                bool success = false;
                // R8: nhắc chọn mã ICD tiền sử khi đã nhập nội dung mà chưa chọn (vẫn cho lưu)
                ShowKskHistoryIcdWarningIfAny();
                WaitingManager.Show();
                HisServiceReqKskExecuteV2SDO sdo = new HisServiceReqKskExecuteV2SDO();
                sdo.ServiceReqId = currentServiceReq.ID;
                sdo.RequestRoomId = currentModule.RoomId;
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    sdo.KskGeneral = new KskGeneralV2SDO();
                    sdo.KskGeneral.HisKskGeneral = new HIS_KSK_GENERAL();
                    sdo.KskGeneral.HisKskGeneral = GetValueGeneral();
                    HIS_DHST dhstGeneralVal = GetValueDhstGeneral();
                    sdo.KskGeneral.HisDhst = HasDhstData(dhstGeneralVal) ? dhstGeneralVal : null;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1)
                {
                    sdo.KskOverEighteen = new KskOverEighteenV2SDO();
                    sdo.KskOverEighteen.HisKskOverEighteen = new HIS_KSK_OVER_EIGHTEEN();
                    sdo.KskOverEighteen.HisKskOverEighteen = GetValueOverEighteen();
                    HIS_DHST dhstOverEightVal = GetDhstOverighteen();
                    sdo.KskOverEighteen.HisDhst = HasDhstData(dhstOverEightVal) ? dhstOverEightVal : null;
                    sdo.KskOverEighteen.HisPeriodDriverDitys = new System.Collections.Generic.List<HIS_PERIOD_DRIVER_DITY>();
                    sdo.KskOverEighteen.HisPeriodDriverDitys = GetDriverDityOverE();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2)
                {
                    sdo.KskUnderEighteen = new KskUnderEighteenV2SDO();
                    sdo.KskUnderEighteen.HisKskUnderEighteen = new HIS_KSK_UNDER_EIGHTEEN();
                    sdo.KskUnderEighteen.HisKskUnderEighteen = GetValueUnderEighteen();
                    HIS_DHST dhstUnderEightVal = GetDhstUnderEighteen();
                    sdo.KskUnderEighteen.HisDhst = HasDhstData(dhstUnderEightVal) ? dhstUnderEightVal : null;
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
                    HIS_DHST dhstOccupationalVal = GetValueDhstOccupational();
                    sdo.KskOccupationalV2.HisDhst = HasDhstData(dhstOccupationalVal) ? dhstOccupationalVal : null;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 7)
                {
                    // Trẻ em dưới 6 tuổi: dữ liệu khám A–O → HIS_KSK_UNDER_SIX; kết luận (gồm ICD-10) → HIS_KSK_GENERAL
                    sdo.KskUnderSix = new KskUnderSix2SDO();
                    sdo.KskUnderSix.HisKskUnderSix = BuildKskUnderSixEf();
                    HIS_DHST dhstUnderSixVal = GetDhstUnderSix();
                    sdo.KskUnderSix.HisDhst = HasDhstData(dhstUnderSixVal) ? dhstUnderSixVal : null;
                    sdo.KskGeneral = new KskGeneralV2SDO();
                    sdo.KskGeneral.HisKskGeneral = BuildKskGeneralConclusionEf();
                }
                // Kết luận theo bệnh (ICD-10) — UC chung → lưu vào HIS_KSK_GENERAL cho MỌI tab
                int curTabIcd = xtraTabControl1.SelectedTabPageIndex;
                if (dicIcdConclusionUc.ContainsKey(curTabIcd) && dicIcdConclusionUc[curTabIcd] != null)
                {
                    if (sdo.KskGeneral == null) sdo.KskGeneral = new KskGeneralV2SDO();
                    if (sdo.KskGeneral.HisKskGeneral == null)
                    {
                        sdo.KskGeneral.HisKskGeneral = new HIS_KSK_GENERAL();
                        // Gán ID bản ghi HIS_KSK_GENERAL hiện có (nếu đã có) để BE UPDATE đúng dòng — giống GetValueGeneral tab định kỳ.
                        // Thiếu ID → BE không cập nhật được người khám/ICD tiền sử/kết luận ở các tab không phải "định kỳ".
                        if (currentKskGeneral != null) sdo.KskGeneral.HisKskGeneral.ID = currentKskGeneral.ID;
                        if (currentServiceReq != null) sdo.KskGeneral.HisKskGeneral.SERVICE_REQ_ID = currentServiceReq.ID;
                    }
                    dicIcdConclusionUc[curTabIcd].FillToGeneral(sdo.KskGeneral.HisKskGeneral);
                }
                // ICD tiền sử (R5) — lưu tập trung vào HIS_KSK_GENERAL cho mọi tab
                if (sdo.KskGeneral != null && sdo.KskGeneral.HisKskGeneral != null)
                {
                    FillKskHistoryIcdToGeneral(sdo.KskGeneral.HisKskGeneral);
                    // Loại mẫu KSK theo tab đang lưu
                    SetKskTypeIdToGeneral(sdo.KskGeneral.HisKskGeneral);
                    // Người khám kết luận (tab trên/dưới 18 tuổi) → HIS_KSK_GENERAL
                    FillConcluderExtToGeneral(sdo.KskGeneral.HisKskGeneral);
                }
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Debug("INPUT DATA:__api/HisServiceReq/KskExecuteV2 " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                KskExecuteResultV2SDO result = new BackendAdapter(param).Post<KskExecuteResultV2SDO>("api/HisServiceReq/KskExecuteV2", ApiConsumers.MosConsumer, sdo, param);
                //Inventec.Common.Logging.LogSystem.Debug("INPUT DATA:__api/HisServiceReq/KskExecuteV2 " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
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
                    currentKskUnderSixEf = result.KskUnderSix; // để in Mps000516 theo DB sau khi lưu
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
        private void Control_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    // Lấy control tiếp theo trong TabOrder
                    Control nextControl = GetNextControl((Control)sender, true);

                    // Nếu control tiếp theo là ComboBoxEdit thì hiển thị popup
                    if (nextControl is DevExpress.XtraEditors.ComboBoxEdit combo)
                    {
                        combo.Focus();
                        combo.ShowPopup();
                    }
                    else
                    {
                        nextControl?.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                // Chọn sang tab "Trẻ em dưới 6 tuổi": cảnh báo nếu BN đã đủ 6 tuổi (>=72 tháng) tại thời điểm khám.
                if (xtraTabControl1.SelectedTabPageIndex == 7 && !ConfirmUnderSixAgeAtExam())
                {
                    xtraTabControl1.SelectedTabPageIndex = 0; // quay về tab Ksk định kỳ
                    return;
                }
                // Đổi tab: cập nhật trạng thái cho phép tích "Tự động lấy kết quả xét nghiệm".
                UpdateAutoTestIndexEnableByTab();
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
                else if (xtraTabControl1.SelectedTabPageIndex == 7)
                {
                    // Trẻ dưới 6 tuổi: phiếu in dựng từ form (Mps000516) → cho in khi có lượt khám
                    if (currentServiceReq != null)
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
                else if (xtraTabControl1.SelectedTabPageIndex == 7)
                {
                    if (currentServiceReq != null)
                        PrintProcess(PRINT_TYPE.MPS000516);
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
                            case ENameOtherItem.MOR_4:
                                txtMorphine.Text = item.VALUE;
                                break;
                            case ENameOtherItem.HER_4:
                                txtHeroin.Text = item.VALUE;
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

        #region ===== TỰ ĐỘNG LẤY KẾT QUẢ XÉT NGHIỆM THEO NHÓM CHỈ SỐ (chkAutoTestIndex) =====

        // Cờ chặn xử lý sự kiện khi set Checked bằng code (vd lúc đổi tab) — tránh gọi lấy dữ liệu ngoài ý muốn.
        private bool isSuppressAutoTestIndexEvent = false;

        // Giá trị TEST_INDEX_GROUP_CODE theo cấu hình danh mục HIS_TEST_INDEX_GROUP thực tế.
        private static class TestIndexGroupCode
        {
            // Form "KSK trên 18 tuổi" (OverEighteen)
            public const string HONG_CAU = "01";   // Hồng cầu          → txtTestBloodHc2
            public const string BACH_CAU = "02";   // Bạch cầu          → txtTestBloodBc2
            public const string TIEU_CAU = "03";   // Tiểu cầu          → txtTestBloodTc2
            public const string DUONG_MAU = "04";  // Đường máu         → txtTestBloodGluco2
            public const string URE = "05";        // Ure               → txtTestBloodUre2
            public const string CREATININ = "06";  // Creatinin         → txtTestBloodCreatinin2
            public const string GOT = "07";        // GOT (ASAT/AST)    → txtTestBloodAsat2
            public const string GPT = "08";        // GPT (ALAT/ALT)    → txtTestBloodAlat2
            public const string DUONG = "09";      // Đường (niệu)      → txtTestUrineGluco2
            public const string PROTEIN = "10";    // Protein (niệu)    → txtTestUrineProtein2

            // Form "KSK lái xe" (4) + "KSK lái xe ô tô" (5) — chất gây nghiện
            public const string MORPHIN = "11";    // Morphin           → txtMorphine
            public const string HEROIN = "12";     // Heroin            → txtHeroin
            public const string CODEIN = "13";     // Codein            → txtMorphineHeroin4 + txtMorphineHeroin5 (ô gộp "Morphin/Heroin")
            public const string AMPHETAMIN = "14"; // Amphetamin        → txtTestAmphetamin4 + txtAmphetamin5
            public const string MARIJUANA = "15";  // Marijuana         → txtTestMarijuna4 + txtTestMarijuna5
            // Lưu ý: các ô txtTestMethamphetamin4/5, txtTestConcentration4/5
            // KHÔNG có mã nhóm tương ứng trong danh sách → không auto-fill.
        }

        private void chkAutoTestIndex_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isSuppressAutoTestIndexEvent) return;
                if (chkAutoTestIndex.Checked)
                {
                    AutoGetTestIndexByGroup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gọi ngầm sang ContentSubclinical để lấy danh sách chỉ số xét nghiệm theo nhóm,
        /// dùng delegate DelegateSelectTestIndexGroupData. ContentSubclinical chạy headless và tự đóng.
        /// </summary>
        private void AutoGetTestIndexByGroup()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical").FirstOrDefault();
                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ContentSubclinical");
                    return;
                }
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentServiceReq.TREATMENT_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectTestIndexGroupData)DelegateAutoTestIndexGroup);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    // ContentSubclinical chạy headless: đã load + bắn delegate (điền dữ liệu) đồng bộ trong GetPluginInstance.
                    // Không Show form, chỉ giải phóng instance.
                    var contentForm = extenceInstance as System.Windows.Forms.Form;
                    if (contentForm != null) contentForm.Dispose();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegateAutoTestIndexGroup(object data)
        {
            try
            {
                var list = data as List<HIS.Desktop.ADO.ContentSubclinicalTestIndexGroupADO>;
                if (list == null || list.Count == 0) return;
                foreach (var item in list)
                {
                    if (item == null || string.IsNullOrEmpty(item.TEST_INDEX_GROUP_CODE)) continue;
                    FillTestIndexByGroupCode(item.TEST_INDEX_GROUP_CODE, item.VALUE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Điền giá trị chỉ số xét nghiệm vào đúng ô trên 3 form KSK (trên 18 tuổi, lái xe, lái xe ô tô)
        /// dựa theo TEST_INDEX_GROUP_CODE.
        /// </summary>
        private void FillTestIndexByGroupCode(string groupCode, string value)
        {
            // Chỉ điền vào các ô thuộc ĐÚNG tab đang chọn.
            int tab = xtraTabControl1.SelectedTabPageIndex;
            switch (groupCode)
            {
                // ===== Tab "KSK trên 18 tuổi" (index 1) =====
                case TestIndexGroupCode.HONG_CAU: if (tab == 1) FillIfEmptyEnabled(txtTestBloodHc2, value); break;
                case TestIndexGroupCode.BACH_CAU: if (tab == 1) FillIfEmptyEnabled(txtTestBloodBc2, value); break;
                case TestIndexGroupCode.TIEU_CAU: if (tab == 1) FillIfEmptyEnabled(txtTestBloodTc2, value); break;
                case TestIndexGroupCode.DUONG_MAU: if (tab == 1) FillIfEmptyEnabled(txtTestBloodGluco2, value); break;
                case TestIndexGroupCode.URE: if (tab == 1) FillIfEmptyEnabled(txtTestBloodUre2, value); break;
                case TestIndexGroupCode.CREATININ: if (tab == 1) FillIfEmptyEnabled(txtTestBloodCreatinin2, value); break;
                case TestIndexGroupCode.GOT: if (tab == 1) FillIfEmptyEnabled(txtTestBloodAsat2, value); break;
                case TestIndexGroupCode.GPT: if (tab == 1) FillIfEmptyEnabled(txtTestBloodAlat2, value); break;
                case TestIndexGroupCode.DUONG: if (tab == 1) FillIfEmptyEnabled(txtTestUrineGluco2, value); break;
                case TestIndexGroupCode.PROTEIN: if (tab == 1) FillIfEmptyEnabled(txtTestUrineProtein2, value); break;

                // ===== Chất gây nghiện: tab "KSK lái xe định kỳ" (index 3) & "KSK lái xe" (index 4) =====
                case TestIndexGroupCode.MORPHIN: if (tab == 3) FillIfEmptyEnabled(txtMorphine, value); break;
                case TestIndexGroupCode.HEROIN: if (tab == 3) FillIfEmptyEnabled(txtHeroin, value); break;
                case TestIndexGroupCode.CODEIN:
                    // Codein hiển thị ở ô gộp "Morphin/Heroin".
                    if (tab == 3) FillIfEmptyEnabled(txtMorphineHeroin4, value);
                    else if (tab == 4) FillIfEmptyEnabled(txtMorphineHeroin5, value);
                    break;
                case TestIndexGroupCode.AMPHETAMIN:
                    if (tab == 3) FillIfEmptyEnabled(txtTestAmphetamin4, value);
                    else if (tab == 4) FillIfEmptyEnabled(txtAmphetamin5, value);
                    break;
                case TestIndexGroupCode.MARIJUANA:
                    if (tab == 3) FillIfEmptyEnabled(txtTestMarijuna4, value);
                    else if (tab == 4) FillIfEmptyEnabled(txtTestMarijuna5, value);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Chỉ tự động điền giá trị khi ô đang ENABLE và CHƯA có giá trị (tránh ghi đè dữ liệu người dùng đã nhập).
        /// </summary>
        private void FillIfEmptyEnabled(System.Windows.Forms.Control ctrl, string value)
        {
            try
            {
                if (ctrl == null) return;
                if (ctrl.Enabled && string.IsNullOrEmpty(ctrl.Text))
                    ctrl.Text = value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Chỉ cho phép tích "Tự động lấy kết quả xét nghiệm" ở các tab có khám lâm sàng (tab có ô để load kết quả):
        /// KSK trên 18 tuổi (index 1), KSK lái xe định kỳ (index 3), KSK lái xe (index 4).
        /// Mỗi lần đổi tab thì bỏ tích (để mỗi tab độc lập) và bật/tắt theo tab hiện tại.
        /// </summary>
        private void UpdateAutoTestIndexEnableByTab()
        {
            try
            {
                int tab = xtraTabControl1.SelectedTabPageIndex;
                bool allow = (tab == 1 || tab == 3 || tab == 4);

                // Bỏ tích khi đổi tab mà KHÔNG kích hoạt lấy dữ liệu.
                isSuppressAutoTestIndexEvent = true;
                chkAutoTestIndex.Checked = false;
                isSuppressAutoTestIndexEvent = false;

                chkAutoTestIndex.Enabled = allow;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

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

        private void txtMorphine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.MOR_4;
            GetSpecInformation();
        }

        private void txtHeroin_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            NameOtherItem = ENameOtherItem.HER_4;
            GetSpecInformation();
        }

        #region ===== TIỀN SỬ PHỤ KHOA — radio exclusive pair handlers =====
        private void ExclusiveNullablePairGyn(DevExpress.XtraEditors.CheckEdit me, DevExpress.XtraEditors.CheckEdit other)
        {
            if (Equals(me.Tag, "UPDATE")) return;
            try
            {
                me.Tag = "UPDATE";
                other.Tag = "UPDATE";
                if (me.Checked) other.Checked = false;
            }
            finally
            {
                me.Tag = null;
                other.Tag = null;
            }
        }

        private void chkMarriedYes_CheckedChanged_Gyn(object sender, EventArgs e)
        {
            ExclusiveNullablePairGyn((DevExpress.XtraEditors.CheckEdit)sender, chkMarriedNo);
        }

        private void chkMarriedNo_CheckedChanged_Gyn(object sender, EventArgs e)
        {
            ExclusiveNullablePairGyn((DevExpress.XtraEditors.CheckEdit)sender, chkMarriedYes);
        }

        private void chkMenstrualRegular_CheckedChanged_Gyn(object sender, EventArgs e)
        {
            ExclusiveNullablePairGyn((DevExpress.XtraEditors.CheckEdit)sender, chkMenstrualIrregular);
        }

        private void chkMenstrualIrregular_CheckedChanged_Gyn(object sender, EventArgs e)
        {
            ExclusiveNullablePairGyn((DevExpress.XtraEditors.CheckEdit)sender, chkMenstrualRegular);
        }

        private void chkMenstrualYes_CheckedChanged_Gyn(object sender, EventArgs e)
        {
            ExclusiveNullablePairGyn((DevExpress.XtraEditors.CheckEdit)sender, chkMenstrualNo);
        }

        private void chkMenstrualNo_CheckedChanged_Gyn(object sender, EventArgs e)
        {
            ExclusiveNullablePairGyn((DevExpress.XtraEditors.CheckEdit)sender, chkMenstrualYes);
        }

        private void chkContraceptionYes_CheckedChanged_Gyn(object sender, EventArgs e)
        {
            ExclusiveNullablePairGyn((DevExpress.XtraEditors.CheckEdit)sender, chkContraceptionNo);
        }

        private void chkContraceptionNo_CheckedChanged_Gyn(object sender, EventArgs e)
        {
            ExclusiveNullablePairGyn((DevExpress.XtraEditors.CheckEdit)sender, chkContraceptionYes);
        }
        #endregion

        private void zoomTrackBarControl1_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}
