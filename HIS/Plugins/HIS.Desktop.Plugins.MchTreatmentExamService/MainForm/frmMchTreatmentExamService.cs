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
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.MchTreatmentExamService.UCAdress;
using HIS.UC.SecondaryIcd;
using HIS.UC.TreeSereServ7;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MCH.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Declare

        private Inventec.Desktop.Common.Modules.Module moduleData;
        private HIS_TREATMENT Treatment;
        private UCAddress addressMother;
        private UCAddress addressBaby;
        UserControl ucSereServ;
        TreeSereServ7Processor treeSereServ7Processor;
        V_MCH_EXAM_SERVICE ExamService;
        V_MCH_EXAM_SERVICE ExamServiceEdit;
        private Dictionary<string, List<CheckEdit>> radioGroups;

        private MCH_PATIENT _patient;
        private MCH_TREATMENT _treatment;
        private MCH_EXAM_SERVICE _examService;
        private MCH_SCREENING _screening;
        private MCH_CHILD _child;
        private MCH_BIRTH_INFO _birthInfo;
        private MCH_ANTENATAL_VISIT _antenatalVisit;
        private MCH_CONTRACEPTION _contraception;
        private MCH_ABORTION _abortion;
        private SecondaryIcdProcessor subIcdProcessor;
        private UserControl ucSecondaryIcd;
        private HIS.Desktop.Common.RefeshReference dlgRefresh;
        #endregion

        #region Constructor

        public UCMchTreatmentExamService()
        {
            InitializeComponent();
        }

        public UCMchTreatmentExamService(Inventec.Desktop.Common.Modules.Module moduleData, HIS_TREATMENT Treatment, V_MCH_EXAM_SERVICE ExamService, HIS.Desktop.Common.RefeshReference dlgRefresh)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.dlgRefresh = dlgRefresh;
                this.Treatment = Treatment;
                this.moduleData = moduleData;
                this.ExamService = ExamService;
                // Khởi tạo Dictionary
                radioGroups = new Dictionary<string, List<CheckEdit>>();
                SetIconFrm();
                // Cấu hình định dạng DateTime cho DateEdit controls
                ConfigDateTimeFormat();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Load
        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UCMchTreatmentExamService_Load(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                // Kiểm tra license MCH trước
                if (!CheckMchLicense(ref param))
                {
                    XtraMessageBox.Show(param.GetMessage(),
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DisableAllControls();
                    return;
                }

                // Khởi tạo giá trị mặc định null cho SpinEdit
                InitAllSpinEditDefaultValue();
                
                // Khởi tạo combo trình độ
                InitAllComboDiploma();
                
                // Khởi tạo combo người khám
                InitAllComboUser();
                
                // Khởi tạo combo chẩn đoán và điều trị UT cổ tử cung
                InitAllComboCervicalCancer();
                
                // Khởi tạo button Delete cho tất cả GridLookUpEdit
                InitAllGridLookUpEditDeleteButton();
                
                // Khởi tạo nhiều nhóm radio
                InitializeMultipleRadioGroups();
                
                // Khởi tạo validation cho tất cả các tab
                InitValidation();
                
                InitUcdSecondIcd();
                CheckEnableByTreatment();
                SafeInitUc();
                InitUcAddress();
                FillDataToForm();
                FillEditData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillEditData()
        {
            if (ExamServiceEdit == null || ExamServiceEdit.ID <= 0)
                return;
            bool loadSuccess = LoadExamServiceDetailData(ExamServiceEdit);

            WaitingManager.Hide();

            if (!loadSuccess)
            {
                XtraMessageBox.Show("Không thể tải dữ liệu. Vui lòng thử lại!", "Lỗi",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            SwitchToTabAndFillData(ExamServiceEdit.EXAM_SERVICE_TYPE_ID);
            SetDefaultExamDateAndUser(ExamServiceEdit);
        }

        #endregion

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear ExamServiceEdit để chuyển sang chế độ tạo mới
                ExamServiceEdit = null;
                btnSave.Enabled = true;
                // Clear toàn bộ dữ liệu các tab NGOẠI TRỪ: Ngày khám, Người khám, Trình độ
                ClearAllTabsDataExceptExamInfo();
                
                // Reset các data models
                ResetDataModels();
                
                Inventec.Common.Logging.LogSystem.Debug("btnNew_Click: Cleared all tabs data except Exam Date, User, Diploma. ExamServiceEdit set to null.");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Có lỗi xảy ra khi tạo mới: " + ex.Message,
                    "Lỗi",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedicalHistoryInternal2_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (this.MedicalHistoryInternal2Selected != null && this.MedicalHistoryInternal2Selected.Count > 0)
                {
                    foreach (var item in this.MedicalHistoryInternal2Selected)
                    {
                        roomName += item.NAME + ", ";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }
    }
}
