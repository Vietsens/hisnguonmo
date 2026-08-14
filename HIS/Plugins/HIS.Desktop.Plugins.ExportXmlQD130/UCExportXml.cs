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
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.WCF.DCO;
using His.Bhyt.ExportXml.XML130;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using HIS.Desktop.Plugins.ExportXmlQD130.Base;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using HIS.UC.SereServTree;
using HIS.UC.SettingSignInfo;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.ServiceSign;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Client;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        bool timerTickIsRunning = false;

        SereServTreeProcessor ssTreeProcessor;
        UserControl ucSereServTree;

        List<V_HIS_TREATMENT_1> listTreatment1 = new List<V_HIS_TREATMENT_1>();
        List<V_HIS_TREATMENT_1> listSelection = new List<V_HIS_TREATMENT_1>();

        List<V_HIS_TREATMENT_12> HisTreatments = new List<V_HIS_TREATMENT_12>();
        List<V_HIS_PATIENT_TYPE_ALTER> ListPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
        List<V_HIS_SERE_SERV_2> ListSereServ = new List<V_HIS_SERE_SERV_2>();
        List<V_HIS_BABY> ListBaby = new List<V_HIS_BABY>();
        List<V_HIS_MEDICAL_ASSESSMENT> ListMedicalAssessment = new List<V_HIS_MEDICAL_ASSESSMENT>();
        List<HIS_HIV_TREATMENT> ListHivTreatment = new List<HIS_HIV_TREATMENT>();
        List<HIS_TUBERCULOSIS_TREAT> ListTuberculosisTreat = new List<HIS_TUBERCULOSIS_TREAT>();
        List<V_HIS_SERE_SERV_SUIN> HisSereServSuin = new List<V_HIS_SERE_SERV_SUIN>();
        List<V_HIS_SERE_SERV_TEIN> HisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
        List<V_HIS_SERE_SERV_PTTT> HisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();
        List<HIS_DHST> ListDhst = new List<HIS_DHST>();
        List<HIS_TRACKING> HisTrackings = new List<HIS_TRACKING>();
        List<HIS_EKIP_USER> ListEkipUser = new List<HIS_EKIP_USER>();
        List<V_HIS_BED_LOG> ListBedlog = new List<V_HIS_BED_LOG>();
        List<HIS_DEBATE> ListDebates = new List<HIS_DEBATE>();
        List<HIS_EXP_MEDIMATE_USED> ListExpMedimateUsed = new List<HIS_EXP_MEDIMATE_USED>();
        List<TreatmentImportADO> listTreatmentImport;

        internal string filterType__IN = "Trong DS đầu thẻ BHYT sau:";
        internal string filterType__OUT = "Ngoài DS đầu thẻ BHYT sau:";
        internal string filterType__FeeLockTime = "Thời gian khóa viện phí từ:";
        internal string filterType__EndTreatmentTime = "Thời gian kết thúc điều trị từ:";
        internal string filterType__BeginTreatmentTime = "Thời gian vào viện từ:";
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;

        List<HIS_BRANCH> branchSelecteds;
        List<HIS_PATIENT_TYPE> patientTypeSelecteds;
        List<HIS_PATIENT_TYPE> patientTypeTTSelecteds;
        List<HIS_TREATMENT_TYPE> treatmentTypeSelecteds;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.ExportXmlQD130";
        V_HIS_TREATMENT_1 currentTreatment;
        List<HIS_CONFIG> NewConfig;
        ConfigSyncADO configSync;
        List<V_HIS_TREATMENT_1> listTreatmentSync;
        List<string> listMessageError;
        CommonParam paramUpdateXml130;
        bool callSyncSuccess;
        bool isAutoSync = false;
        //Cờ yêu cầu hủy tiến trình Đồng bộ tự động đang chạy dở (set khi bấm tắt auto). Vòng lặp auto sẽ dừng ở ranh giới hồ sơ.
        volatile bool cancelAutoSyncRequested = false;
        //Dấu hiệu bản build - in ra log để xác nhận máy test có đang chạy đúng DLL mới không. ĐỔI mỗi lần build mới.
        const string KCB4750_BUILD_TAG = "KCB4750-2026-07-29g";
        //Các TreatmentId đang được đẩy 4750 ở nền (fire-and-forget). Dùng để chu kỳ sau KHÔNG chọn lại -> tránh đẩy trùng khi finish chưa kịp lưu.
        readonly System.Collections.Concurrent.ConcurrentDictionary<long, byte> kcb4750InFlight = new System.Collections.Concurrent.ConcurrentDictionary<long, byte>();
        public SavePathADO savePathADO;
        bool isExportXml;
        bool isSendCollinearXml;
        bool isNotFileSign;
        SettingSignADO SettingSignADO;
        bool isXML3176;
        bool btnExportXML3176 = false;
        bool isXML130;
        bool showMessSusscess;
        bool isAutoSignXML3176 = false;
        bool btnAutoSyncClick = false;
        //Cờ đánh dấu đang gửi thủ công riêng dữ liệu KCB lên CSDL 4750 (chỉ tạo XML + đẩy 4750, không gửi cổng BHYT)
        bool manualSyncKcb4750 = false;
        //Danh sách dòng kết quả đồng bộ CSDL 4750 (mã hồ sơ + thành công/thất bại) để thông báo ra màn hình
        List<string> kcb4750ResultLines = new List<string>();
        public SearchFilterADO searchFilter = new SearchFilterADO();
        public UCExportXml(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = moduleData;
                HisConfigCFG.LoadConfig();
                this.InitSereServTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitSereServTree()
        {
            try
            {
                System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ExportXmlQD130.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());

                ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowSearchPanel = false;
                ado.IsCreateParentNodeWithSereServExpend = false;
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                SereServTreeColumn serviceNameCol = new SereServTreeColumn("Tên dịch vụ", "TDL_SERVICE_NAME", 150, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);

                SereServTreeColumn amountCol = new SereServTreeColumn("SL", "AMOUNT_PLUS", 40, false);
                amountCol.VisibleIndex = 1;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(amountCol);

                SereServTreeColumn virPriceCol = new SereServTreeColumn("Đơn giá", "VIR_PRICE", 80, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.Format.FormatString = "#,##0.0000";
                virPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virPriceCol);

                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn("Thành tiền", "VIR_TOTAL_PRICE", 90, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPriceCol);

                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn("Đồng chi trả", "VIR_TOTAL_HEIN_PRICE", 90, false);
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.Format.FormatString = "#,##0.0000";
                virTotalHeinPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn("Bệnh nhân trả", "VIR_TOTAL_PATIENT_PRICE", 110, false);
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPatientPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);

                SereServTreeColumn virDiscountCol = new SereServTreeColumn("Chiết khấu", "DISCOUNT", 90, false);
                virDiscountCol.VisibleIndex = 6;
                virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                virDiscountCol.Format.FormatString = "#,##0.0000";
                virDiscountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virDiscountCol);

                SereServTreeColumn virIsExpendCol = new SereServTreeColumn("Hao phí", "IsExpend", 60, false);
                virIsExpendCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(virIsExpendCol);

                SereServTreeColumn virVatRatioCol = new SereServTreeColumn("VAT", "VAT", 100, false);
                virVatRatioCol.VisibleIndex = 8;
                virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                virVatRatioCol.Format.FormatString = "#,##0.00";
                virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                SereServTreeColumn serviceCodeCol = new SereServTreeColumn("Mã dịch vụ", "TDL_SERVICE_CODE", 100, false);
                serviceCodeCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn("Mã yêu cầu", "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 10;
                ado.SereServTreeColumns.Add(serviceReqCodeCol);

                this.ucSereServTree = (UserControl)ssTreeProcessor.Run(ado);
                if (this.ucSereServTree != null)
                {
                    this.panelControlSereServTree.Controls.Add(this.ucSereServTree);
                    this.ucSereServTree.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExportXml_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();

                this.AddFilterItem();
                this.InItCboFeeLockOrEndTreatment();
                this.GeneratePopupMenu();
                this.InitComboStatus();
                this.InitComboXml130Result();
                this.SetDefaultValueControl();

                this.InitComboTreatmentType();
                this.InitComboBranch();
                this.InitComboPatientType();
                this.InitComboPatientTypeTT();
                this.InitControlState();
                this.SetDefaultSearchFilter();
                this.FillDataToGridTreatment();
                this.InitCheckUSBToken();
                //vCong53286 - Nút Kiểm tra lỗi tiền giám định. Chỉ hiện khi viện đã đấu nối.
                this.InitTienGiamDinhButton();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCheckUSBToken()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                btnXML3176.Visible = !chkXML3176.Checked;
                Inventec.Common.Logging.LogSystem.Info("Load form - checkbox XML3176: " + chkXML3176.Checked);
                if (SettingSignADO != null && !String.IsNullOrWhiteSpace(SettingSignADO.SerialNumber))
                {
                    chkSignFileCertUtil.Checked = !String.IsNullOrWhiteSpace(SettingSignADO.SerialNumber);
                    if (chkSignFileCertUtil.Checked && HisConfigCFG.BHXH__XML_SIGN_OPTION == "1")
                    {
                        chkSignFileCertUtil.Properties.ReadOnly = true;
                        chkSignFileCertUtil.Enabled = false;
                    }
                }
                else
                {
                    chkSignFileCertUtil.Checked = false;
                    chkSignFileCertUtil.Properties.ReadOnly = false;
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExportXmlQD130.Resources.Lang", typeof(UCExportXml).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSavePath.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnSavePath.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboXml130Result.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExportXml.cboXml130Result.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSettingConfigSync.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnSettingConfigSync.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSettingConfigSync.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.btnSettingConfigSync.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAutoSync.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnAutoSync.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAutoSync.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.btnAutoSync.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUnlock.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnUnlock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnLock.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnLock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExportXml.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExportXml.txtPatientCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboFilterType.Text = Inventec.Common.Resource.Get.Value("UCExportXml.cboFilterType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStatusFeeLockOrEndTreatment.Text = Inventec.Common.Resource.Get.Value("UCExportXml.cboStatusFeeLockOrEndTreatment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExportXml.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.txtKeyword.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExportXml.cboStatus.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatCodeOrHeinCard.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExportXml.txtTreatCodeOrHeinCard.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDownload.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnDownload.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.btnImport.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboBranch.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExportXml.CboBranch.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExportXml.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnExportXml.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_ViewXML.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_ViewXML.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_ViewXML.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_ViewXML.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_Treatment_Stt.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_Treatment_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_TreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Treatment_PatientCode.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn_Treatment_PatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_Treatment_VirPatientName.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_VirPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_Treatment_Gender.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_Gender.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_Treatment_Dob.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_Dob.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_Treatment_HeinCardNumber.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_HeinCardNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColCheckCode.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColCheckCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Treatment_EndDepartment.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn_Treatment_EndDepartment.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_Treatment_InTime.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_InTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_Clinical_InTime.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Clinical_InTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_Treatment_OutTime.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_OutTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_Treatment_FeeLockTime.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_FeeLockTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridCol_Treatment_HeinLockTime.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_HeinLockTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Treatment_TotalPrice.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn_Treatment_TotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Treatment_TotalHeinPrice.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn_Treatment_TotalHeinPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Treatment_TotalPatientPrice.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn_Treatment_TotalPatientPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtHeinCardPrefix.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExportXml.txtHeinCardPrefix.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboFilterTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExportXml.cboFilterTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCExportXml.lciTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeTo.Text = Inventec.Common.Resource.Get.Value("UCExportXml.lciTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciCboBranch.Text = Inventec.Common.Resource.Get.Value("UCExportXml.LciCboBranch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem22.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientTypeTT.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExportXml.cboPatientTypeTT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem20.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkXML3176.Text = Inventec.Common.Resource.Get.Value("UCExportXml.chkXML3176.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkXML3176.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.chkXML3176.Properties.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GeneratePopupMenu()
        {
            try
            {
                DevExpress.Utils.Menu.DXPopupMenu menu = new DevExpress.Utils.Menu.DXPopupMenu();

                menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("XML 130", new EventHandler(btnSync_Click)));
                menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("XML 3176", new EventHandler(btnXML3176_Send)));
                menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("XML 130 thông tuyến", new EventHandler((u, v) =>
                {
                    SendXml130Collinear();
                })));

                //Hiển thị menu đồng bộ KCB khi bật liên thông CSDL 4750 (MOS.CSDL_4750.IS_AUTO_SYNC)
                //HOẶC viện có khóa Cổng tiếp nhận KDLYT Vĩnh Long (đẩy hoan-tat).
                if (HisConfigCFG.CSDL_4750__IS_AUTO_SYNC == "1"
                    || !string.IsNullOrWhiteSpace(HisConfigCFG.VLG_2062__CONNECTION_INFO))
                {
                    menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Đồng bộ Khám chữa bệnh (Kết thúc khám/Xuất viện)", new EventHandler(btnSyncKcb4750_Click)));
                }

                btnSend.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Gửi thủ công dữ liệu KCB (Kết thúc khám/Xuất viện) lên CSDL 4750 cho các hồ sơ đang chọn.
        //Chỉ tạo XML rồi đẩy lên CSDL 4750 (mục 3 + 6), KHÔNG gửi lên cổng BHYT.
        private async void btnSyncKcb4750_Click(object sender, EventArgs e)
        {
            try
            {
                //Không chạy chồng lên lượt tự động đang chạy: 2 lượt dùng chung state theo lô
                //(HisTreatments, List*, isNotFileSign...) — chồng nhau sẽ trộn/hỏng dữ liệu của nhau.
                if (backgroundWorker1 != null && backgroundWorker1.IsBusy)
                {
                    XtraMessageBox.Show("Đang chạy Đồng bộ tự động — chờ lượt hiện tại xong rồi bấm lại.",
                        Resources.ResourceMessageLang.ThongBao);
                    return;
                }
                //2 đích đẩy KCB: CSDL 4750 (bật IS_AUTO_SYNC + có key) và Cổng tiếp nhận VLG (có key + tích chọn ở Cài đặt).
                bool has4750 = HisConfigCFG.CSDL_4750__IS_AUTO_SYNC == "1"
                    && !string.IsNullOrWhiteSpace(HisConfigCFG.CSDL_4750__CONNECTION_INFO);
                bool hasVlg = !string.IsNullOrWhiteSpace(HisConfigCFG.VLG_2062__CONNECTION_INFO)
                    && this.configSync != null && this.configSync.isSyncKcbVlg;
                //Bật 4750 nhưng thiếu key: chỉ CHẶN khi không còn đích VLG — có VLG thì vẫn cho đẩy VLG
                //(ProcessSyncTreatment tự bỏ qua worker 4750 không hợp lệ).
                if (HisConfigCFG.CSDL_4750__IS_AUTO_SYNC == "1"
                    && string.IsNullOrWhiteSpace(HisConfigCFG.CSDL_4750__CONNECTION_INFO)
                    && !hasVlg)
                {
                    XtraMessageBox.Show("Chưa cấu hình kết nối CSDL 4750 (HIS.CSDL_4750.CONNECTION_INFO)", Resources.ResourceMessageLang.ThongBao);
                    return;
                }
                if (!has4750 && !hasVlg)
                {
                    XtraMessageBox.Show("Chưa bật đích đồng bộ KCB nào." + Environment.NewLine
                        + "- CSDL 4750: bật MOS.CSDL_4750.IS_AUTO_SYNC + khóa HIS.CSDL_4750.CONNECTION_INFO." + Environment.NewLine
                        + "- Cổng tiếp nhận VLG: có khóa MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO + tích chọn trong nút Cài đặt.",
                        Resources.ResourceMessageLang.ThongBao);
                    return;
                }
                if (listSelection == null || listSelection.Count == 0)
                {
                    XtraMessageBox.Show(Resources.ResourceMessageLang.BanChuaChonHoSoDeDongBo, Resources.ResourceMessageLang.ThongBao);
                    return;
                }

                manualSyncKcb4750 = true;
                isXML130 = true;
                isXML3176 = false;
                isSendCollinearXml = false;
                isNotFileSign = true;
                callSyncSuccess = false;
                try
                {
                    WaitingManager.Show();
                    await ProcessSyncTreatment(listSelection, true);
                    FillDataToGridTreatment();
                    WaitingManager.Hide();

                    //Thông báo rõ từng hồ sơ (mã hồ sơ + thành công/thất bại + message từ API liên thông)
                    int okCount = this.kcb4750ResultLines.Count(o => o.Contains(": Thành công"));
                    int failCount = this.kcb4750ResultLines.Count - okCount;
                    StringBuilder sbMsg = new StringBuilder();
                    //Mỗi hồ sơ có 1 dòng cho MỖI đích (4750 + VLG) -> đếm theo LƯỢT GỬI, không phải theo hồ sơ.
                    sbMsg.AppendLine(string.Format("Kết quả đồng bộ Khám chữa bệnh (CSDL 4750 / Cổng tiếp nhận VLG): {0} lượt gửi thành công, {1} lượt gửi thất bại.", okCount, failCount));
                    sbMsg.AppendLine();
                    foreach (var line in this.kcb4750ResultLines)
                    {
                        sbMsg.AppendLine(line);
                    }
                    XtraMessageBox.Show(sbMsg.ToString().Trim(), Resources.ResourceMessageLang.ThongBao);
                }
                finally
                {
                    manualSyncKcb4750 = false;
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<FilterTypeADO> ListStatusAll = new List<FilterTypeADO>();
        public void InitComboStatus()
        {
            try
            {

                FilterTypeADO tatCa = new FilterTypeADO(0, Resources.ResourceMessageLang.TatCa);
                ListStatusAll.Add(tatCa);

                FilterTypeADO duyetBhyt = new FilterTypeADO(1, Resources.ResourceMessageLang.DaKhoaBHYT);
                ListStatusAll.Add(duyetBhyt);

                FilterTypeADO ketthuc = new FilterTypeADO(2, Resources.ResourceMessageLang.DaKTDieuTri);
                ListStatusAll.Add(ketthuc);

                FilterTypeADO dacosovaovien = new FilterTypeADO(3, Resources.ResourceMessageLang.DaCoSoVaoVien);
                ListStatusAll.Add(dacosovaovien);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "id", columnInfos, false, 250);
                ControlEditorLoader.Load(cboStatus, ListStatusAll, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        List<FilterTypeADO> ListXml130ResultAll = new List<FilterTypeADO>();
        public void InitComboXml130Result()
        {
            try
            {

                FilterTypeADO tatCa = new FilterTypeADO(0, Resources.ResourceMessageLang.TatCa);
                ListXml130ResultAll.Add(tatCa);

                FilterTypeADO daGuiHoSo = new FilterTypeADO(1, Resources.ResourceMessageLang.DaGuiHoSo);
                ListXml130ResultAll.Add(daGuiHoSo);

                FilterTypeADO chuaGuiHoSo = new FilterTypeADO(2, Resources.ResourceMessageLang.ChuaGuiHoSo);
                ListXml130ResultAll.Add(chuaGuiHoSo);

                FilterTypeADO hoSoGuiThatBai = new FilterTypeADO(3, Resources.ResourceMessageLang.HoSoGuiThatBai);
                ListXml130ResultAll.Add(hoSoGuiThatBai);

                FilterTypeADO hoSoGuiThanhCong = new FilterTypeADO(4, Resources.ResourceMessageLang.HoSoGuiThanhCong);
                ListXml130ResultAll.Add(hoSoGuiThanhCong);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "id", columnInfos, false, 250);
                ControlEditorLoader.Load(cboXml130Result, ListXml130ResultAll, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultValueControl()
        {
            try
            {
                cboStatus.EditValue = 0;
                cboXml130Result.EditValue = 0;
                dtTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                dtTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;
                dtHeinLockTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0) ?? DateTime.MinValue;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddFilterItem()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemFilterIn = new DXMenuItem(filterType__IN, new EventHandler(btnFilterType_Click));
                itemFilterIn.Tag = "filterIn";
                menu.Items.Add(itemFilterIn);

                DXMenuItem itemFilterOut = new DXMenuItem(filterType__OUT, new EventHandler(btnFilterType_Click));
                itemFilterOut.Tag = "filterOut";
                menu.Items.Add(itemFilterOut);

                cboFilterType.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InItCboFeeLockOrEndTreatment()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemFeeLockTime = new DXMenuItem(filterType__FeeLockTime, new EventHandler(btnFeeLockOrEndTreatment_Click));
                itemFeeLockTime.Tag = "filterFeeLockTime";
                menu.Items.Add(itemFeeLockTime);

                DXMenuItem itemFilterEndTreatment = new DXMenuItem(filterType__EndTreatmentTime, new EventHandler(btnFeeLockOrEndTreatment_Click));
                itemFilterEndTreatment.Tag = "filterEndTreatment";
                menu.Items.Add(itemFilterEndTreatment);


                DXMenuItem itemBiginTreatmentTime = new DXMenuItem(filterType__BeginTreatmentTime, new EventHandler(btnFeeLockOrEndTreatment_Click));
                itemFilterEndTreatment.Tag = "BiginTreatmentTime";
                menu.Items.Add(itemBiginTreatmentTime);

                cboStatusFeeLockOrEndTreatment.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFilterType_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                cboFilterType.Text = btnMenuCodeFind.Caption;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFeeLockOrEndTreatment_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                cboStatusFeeLockOrEndTreatment.Text = btnMenuCodeFind.Caption;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment()
        {
            try
            {
                //vCong53286 - Tải lại danh sách thì bỏ kết quả kiểm tra đã nhớ trong phiên.
                //Hồ sơ có thể vừa được sửa nên kết quả cũ không còn đúng.
                this.ClearTienGiamDinhSessionResult();

                FillDataToGridTreatment(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTreatment, param, (int)ConfigApplications.NumPageSize, this.gridControlTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment(object param)
        {
            try
            {
                listTreatment1 = new List<V_HIS_TREATMENT_1>();
                listSelection = new List<V_HIS_TREATMENT_1>();
                listTreatmentImport = null;
                gridControlTreatment.DataSource = null;
                btnExportXml.Enabled = false;
                UpdateBtnXML3176Visibility();
                btnXML3176.Enabled = false;
                btnExportGroupXml.Enabled = false;
                btnExportCollinearXml.Enabled = false;
                btnSend.Enabled = false;
                btnLock.Enabled = false;
                btnUnlock.Enabled = false;
                FillDataToSereServTreeByTreatment(null);

                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HisTreatmentView1Filter filter = new HisTreatmentView1Filter();
                filter.ORDER_DIRECTION = "ACS";
                filter.ORDER_FIELD = "FEE_LOCK_TIME";



                if (!String.IsNullOrEmpty(txtTreatCodeOrHeinCard.Text.Trim()))
                {
                    string code = txtTreatCodeOrHeinCard.Text.Trim();
                    try
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    txtTreatCodeOrHeinCard.Text = code;
                    filter.TREATMENT_CODE__EXACT = code;
                }
                else if (!String.IsNullOrEmpty(txtPatientCode.Text.Trim()))
                {
                    string code = txtPatientCode.Text.Trim();
                    try
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }

                    txtPatientCode.Text = code;
                    filter.TDL_PATIENT_CODE__EXACT = code;
                }

                if (String.IsNullOrEmpty(filter.TREATMENT_CODE__EXACT) && String.IsNullOrEmpty(filter.TDL_PATIENT_CODE__EXACT))
                {
                    if (this.branchSelecteds != null && this.branchSelecteds.Count > 0)
                        filter.BRANCH_IDs = this.branchSelecteds.Select(o => o.ID).ToList();

                    if (this.patientTypeSelecteds != null && this.patientTypeSelecteds.Count > 0)
                        filter.TDL_PATIENT_TYPE_IDs = this.patientTypeSelecteds.Select(o => o.ID).ToList();

                    if (this.treatmentTypeSelecteds != null && this.treatmentTypeSelecteds.Count > 0)
                        filter.TDL_TREATMENT_TYPE_IDs = this.treatmentTypeSelecteds.Select(o => o.ID).ToList();

                    if (!String.IsNullOrWhiteSpace(txtKeyword.Text))
                    {
                        filter.KEY_WORD = txtKeyword.Text.Trim();
                    }
                    if (cboStatus.EditValue != null && (int)cboStatus.EditValue == 1)// Đã khóa BHYT
                    {
                        filter.IS_LOCK_HEIN = true;
                    }
                    else if (cboStatus.EditValue != null && (int)cboStatus.EditValue == 2)//Đã kết thúc điều trị
                    {
                        filter.IS_PAUSE = true;
                    }
                    else if (cboStatus.EditValue != null && (int)cboStatus.EditValue == 3)// Đã có số vào viện
                    {
                        filter.HAS_IN_CODE = true;
                    }
                    if (cboXml130Result.EditValue != null && (int)cboXml130Result.EditValue == 1)// Đã gửi hồ sơ
                    {
                        filter.HAS_XML130_RESULT = true;
                    }
                    else if (cboXml130Result.EditValue != null && (int)cboXml130Result.EditValue == 2)//Chưa gửi hồ sơ
                    {
                        filter.HAS_XML130_RESULT = false;
                    }
                    else if (cboXml130Result.EditValue != null && (int)cboXml130Result.EditValue == 3)//Hồ sơ gửi thất bại
                    {
                        filter.XML130_RESULT = 1;
                    }
                    else if (cboXml130Result.EditValue != null && (int)cboXml130Result.EditValue == 4)// Hồ sơ gửi thành công
                    {
                        filter.XML130_RESULT = 2;
                    }
                    if (cboStatusFeeLockOrEndTreatment.Text == this.filterType__FeeLockTime) //Thời gian khóa viện phí
                    {
                        if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                        {
                            filter.FEE_LOCK_TIME_FROM = Convert.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                        }
                        if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                        {
                            filter.FEE_LOCK_TIME_TO = Convert.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");
                        }
                    }
                    else if (cboStatusFeeLockOrEndTreatment.Text == this.filterType__EndTreatmentTime) //Thời gian kết thúc điều trị
                    {
                        if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                        {
                            filter.OUT_TIME_FROM = Convert.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                        }
                        if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                        {
                            filter.OUT_TIME_TO = Convert.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                        }

                    }
                    else if (cboStatusFeeLockOrEndTreatment.Text == filterType__BeginTreatmentTime) //Thời gian vào viện
                    {

                        if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                        {
                            filter.IN_TIME_FROM = Convert.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                        }
                        if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                        {
                            filter.IN_TIME_TO = Convert.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                        }
                    }
                    if (!String.IsNullOrEmpty(txtHeinCardPrefix.Text) && !String.IsNullOrEmpty(txtHeinCardPrefix.Text.Trim()))
                    {
                        string[] heinCardArr = txtHeinCardPrefix.Text.Trim().Split(new char[] { ',' });
                        if (heinCardArr != null && heinCardArr.Length > 0)
                        {
                            foreach (var item in heinCardArr)
                            {
                                if (String.IsNullOrEmpty(item.Trim()))
                                    continue;
                                var card = item.Trim().ToUpper();
                                if (cboFilterType.Text == filterType__IN)
                                {
                                    if (filter.TDL_HEIN_CARD_NUMBER_PREFIXs == null) filter.TDL_HEIN_CARD_NUMBER_PREFIXs = new List<string>();
                                    filter.TDL_HEIN_CARD_NUMBER_PREFIXs.Add(card);
                                }
                                else if (cboFilterType.Text == filterType__OUT)
                                {
                                    if (filter.TDL_HEIN_CARD_NUMBER_PREFIX__NOT_INs == null) filter.TDL_HEIN_CARD_NUMBER_PREFIX__NOT_INs = new List<string>();
                                    filter.TDL_HEIN_CARD_NUMBER_PREFIX__NOT_INs.Add(card);
                                }
                                else
                                {
                                    if (filter.TDL_HEIN_CARD_NUMBER_PREFIXs == null) filter.TDL_HEIN_CARD_NUMBER_PREFIXs = new List<string>();
                                    filter.TDL_HEIN_CARD_NUMBER_PREFIXs.Add(card);
                                }
                            }
                        }
                    }
                }
                //filter.HAS_XML130_RESULT = false;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("filter__:", filter));

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    listTreatment1 = (List<V_HIS_TREATMENT_1>)result.Data;
                    rowCount = (listTreatment1 == null ? 0 : listTreatment1.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }
                gridControlTreatment.BeginUpdate();
                gridControlTreatment.DataSource = listTreatment1;
                gridControlTreatment.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToSereServTreeByTreatment(V_HIS_TREATMENT_1 data)
        {
            try
            {
                var listSereServ = new List<V_HIS_SERE_SERV_5>();
                if (data != null)
                {
                    HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                    ssFilter.TDL_TREATMENT_ID = data.ID;
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                }

                this.ssTreeProcessor.Reload(ucSereServTree, listSereServ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtTimeTo.Focus();
                    dtTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtTreatCodeOrHeinCard.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatCodeOrHeinCard_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (String.IsNullOrEmpty(txtTreatCodeOrHeinCard.Text))
                    {
                        txtPatientCode.Focus();
                        txtPatientCode.SelectAll();
                    }
                    else
                    {
                        this.btnFind_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex < 0 || !e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound)
                    return;
                var data = (V_HIS_TREATMENT_1)gridViewTreatment.GetRow(e.ListSourceRowIndex);
                if (data != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start;
                    }
                    else if (e.Column.FieldName == "DOB_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    }
                    else if (e.Column.FieldName == "IN_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                    }
                    else if (e.Column.FieldName == "CLINICAL_IN_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CLINICAL_IN_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "OUT_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "FEE_LOCK_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FEE_LOCK_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "HEIN_LOCK_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.HEIN_LOCK_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                WaitingManager.Show();
                var row = (V_HIS_TREATMENT_1)gridViewTreatment.GetFocusedRow();
                FillDataToSereServTreeByTreatment(row);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                UpdateBtnXML3176Visibility();
                listSelection = new List<V_HIS_TREATMENT_1>();
                var listIndex = gridViewTreatment.GetSelectedRows();
                foreach (var index in listIndex)
                {
                    var treatment = (V_HIS_TREATMENT_1)gridViewTreatment.GetRow(index);
                    if (treatment != null)
                    {
                        listSelection.Add(treatment);
                    }
                }

                if (listSelection.Count > 0)
                {
                    btnExportXml.Enabled = true;
                    btnExportCollinearXml.Enabled = true;
                    btnSend.Enabled = true;
                    btnExportGroupXml.Enabled = true;
                    //btnXML3176.Enabled = true;
                    if (!chkXML3176.Checked)
                    {
                        btnXML3176.Enabled = true;
                    }
                }
                else
                {
                    btnExportXml.Enabled = false;
                    btnExportCollinearXml.Enabled = false;
                    btnSend.Enabled = false;
                    btnExportGroupXml.Enabled = false;
                    btnXML3176.Enabled = false;
                }

                gridViewTreatment.BeginDataUpdate();
                gridViewTreatment.EndDataUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CustomDrawNodeCell(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (data != null && !e.Node.HasChildren)
                {
                    if (!data.VIR_TOTAL_PATIENT_PRICE.HasValue || data.VIR_TOTAL_PATIENT_PRICE.Value <= 0)
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled)
                    return;
                WaitingManager.Show();
                FillDataToGridTreatment();

                if (listTreatment1 != null && listTreatment1.Count == 1)
                {
                    FillDataToSereServTreeByTreatment(listTreatment1.First());
                }
                gridControlTreatment.Focus();
                SaveSearchFilter();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void btnExportXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportXml.Enabled || listSelection == null || listSelection.Count == 0) return;

                //vCong53286 - Cổng chặn tiền giám định. Hồ sơ có lỗi nghiêm trọng thì dừng cả lượt, không sinh tệp nào.
                if (!await EnsureTienGiamDinhPassedAsync()) return;
                CommonParam param = new CommonParam();
                MemoryStream memoryStream = new MemoryStream();
                bool success = false;
                bool xuatXml12 = true;

                if (this.savePathADO == null || string.IsNullOrEmpty(this.savePathADO.pathXml))
                {
                    btnSavePath_Click(null, null);
                }
                if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXml))
                {
                    if (string.IsNullOrEmpty(this.savePathADO.pathXmlGDYK))
                    {
                        if (XtraMessageBox.Show("Chưa chọn thư mục lưu file chỉ tiêu dữ liệu giám định y khoa. Bạn có muốn chọn đường dẫn không?", Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                            btnSavePath_Click(null, null);
                    }
                    xuatXml12 = !string.IsNullOrEmpty(this.savePathADO.pathXmlGDYK);

                    //if (string.IsNullOrEmpty(SerialNumber))
                    //{
                    //    MessageBox.Show("Không có thông tin Usb Token ký số");
                    //    return;
                    //}
                    //else
                    //{
                    //    WaitingManager.Show();
                    //    Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click Begin");
                    //    success = this.GenerateXml(ref param, ref memoryStream, false, false, xuatXml12, listSelection);
                    //    Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click End");
                    //    WaitingManager.Hide();
                    //}
                    WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click Begin");
                    Inventec.Common.Logging.LogSystem.Info("btnExportXml - checkbox XML3176: " + chkXML3176.Checked);
                    success = this.GenerateXml(ref param, ref memoryStream, false, false, xuatXml12, listSelection, chkXML3176.Checked);
                    Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click End");
                    WaitingManager.Hide();
                    if (success && param.Messages.Count == 0)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else if (param.Messages.Count >= 1)
                    {
                        MessageManager.Show(param, success);
                    }
                    else
                    {
                        param.Messages.Add("Xuất XML thất bại. Vui lòng kiểm tra lại.");
                        MessageManager.Show(this.ParentForm, param, false);
                    }
                    this.gridControlTreatment.RefreshDataSource();
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool GenerateXml(ref CommonParam paramExport, ref MemoryStream memoryStream, bool viewXml, bool xuatXmlTT, bool xuatXml12, List<V_HIS_TREATMENT_1> listSelection, bool isXML3176)
        {
            bool result = false;
            try
            {
                if (listSelection.Count > 0)
                {
                    listSelection = listSelection.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();
                    this.NewConfig = GetNewConfig();
                    int skip = 0;
                    while (listSelection.Count - skip > 0)
                    {
                        var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                        ListPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
                        ListSereServ = new List<V_HIS_SERE_SERV_2>();
                        ListEkipUser = new List<HIS_EKIP_USER>();
                        ListBedlog = new List<V_HIS_BED_LOG>();
                        HisTreatments = new List<V_HIS_TREATMENT_12>();
                        ListDhst = new List<HIS_DHST>();
                        HisTrackings = new List<HIS_TRACKING>();
                        HisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                        HisSereServSuin = new List<V_HIS_SERE_SERV_SUIN>();
                        HisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();
                        ListDebates = new List<HIS_DEBATE>();
                        ListBaby = new List<V_HIS_BABY>();
                        ListMedicalAssessment = new List<V_HIS_MEDICAL_ASSESSMENT>();
                        ListHivTreatment = new List<HIS_HIV_TREATMENT>();
                        ListTuberculosisTreat = new List<HIS_TUBERCULOSIS_TREAT>();
                        ListExpMedimateUsed = new List<HIS_EXP_MEDIMATE_USED>();
                        string message = "";
                        isExportXml = true;
                        //qtcode
                        MemoryStream memoryStreamXml12 = new MemoryStream();
                        //qtcode
                        CreateThreadGetData(limit);
                        isExportXml = false;
                        if (chkSignFileCertUtil.Checked == false)
                        {
                            isNotFileSign = true;
                            //qtcode
                            message = ProcessExportXmlDetail(ref result, ref memoryStream, ref memoryStreamXml12, viewXml, xuatXmlTT, xuatXml12, HisTreatments, ListPatientTypeAlter, ListSereServ, ListDhst, HisSereServTeins, HisTrackings, HisSereServPttts, ListEkipUser, ListBedlog, ListDebates, ListBaby, ListMedicalAssessment, ListHivTreatment, HisSereServSuin, ListTuberculosisTreat, ListExpMedimateUsed, isXML3176);
                        }
                        else
                        {
                            if (SettingSignADO != null && string.IsNullOrEmpty(SettingSignADO.SerialNumber) || SettingSignADO == null)
                            {
                                if (XtraMessageBox.Show("Không có thông tin Serial chứng thư ký số. Bạn có muốn tiếp tục xuất xml?", Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    message = "";
                                }
                                else
                                {
                                    isNotFileSign = true;
                                    message = ProcessExportXmlDetail(ref result, ref memoryStream, ref memoryStreamXml12, viewXml, xuatXmlTT, xuatXml12, HisTreatments, ListPatientTypeAlter, ListSereServ, ListDhst, HisSereServTeins, HisTrackings, HisSereServPttts, ListEkipUser, ListBedlog, ListDebates, ListBaby, ListMedicalAssessment, ListHivTreatment, HisSereServSuin, ListTuberculosisTreat, ListExpMedimateUsed, isXML3176);
                                }
                            }
                            else
                            {
                                isNotFileSign = false;
                                message = ProcessExportXmlDetail(ref result, ref memoryStream, ref memoryStreamXml12, viewXml, xuatXmlTT, xuatXml12, HisTreatments, ListPatientTypeAlter, ListSereServ, ListDhst, HisSereServTeins, HisTrackings, HisSereServPttts, ListEkipUser, ListBedlog, ListDebates, ListBaby, ListMedicalAssessment, ListHivTreatment, HisSereServSuin, ListTuberculosisTreat, ListExpMedimateUsed, isXML3176);
                            }
                        }
                        if (!String.IsNullOrEmpty(message))
                        {
                            paramExport.Messages.Add(message);
                        }
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

        bool GenerateXmlPlus(ref CommonParam paramExport, ref MemoryStream memoryStream, bool xuatXml12, List<V_HIS_TREATMENT_1> listSelection, bool isXML3176)
        {
            bool result = false;
            try
            {
                if (listSelection.Count > 0)
                {
                    listSelection = listSelection.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();
                    this.NewConfig = GetNewConfig();
                    int skip = 0;

                    ListPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
                    ListSereServ = new List<V_HIS_SERE_SERV_2>();
                    ListEkipUser = new List<HIS_EKIP_USER>();
                    ListBedlog = new List<V_HIS_BED_LOG>();
                    HisTreatments = new List<V_HIS_TREATMENT_12>();
                    ListDhst = new List<HIS_DHST>();
                    HisTrackings = new List<HIS_TRACKING>();
                    HisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                    HisSereServSuin = new List<V_HIS_SERE_SERV_SUIN>();
                    HisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();
                    ListDebates = new List<HIS_DEBATE>();
                    ListBaby = new List<V_HIS_BABY>();
                    ListMedicalAssessment = new List<V_HIS_MEDICAL_ASSESSMENT>();
                    ListHivTreatment = new List<HIS_HIV_TREATMENT>();
                    ListTuberculosisTreat = new List<HIS_TUBERCULOSIS_TREAT>();
                    ListExpMedimateUsed = new List<HIS_EXP_MEDIMATE_USED>();
                    string message = "";
                    while (listSelection.Count - skip > 0)
                    {
                        var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                        isExportXml = true;
                        CreateThreadGetData(limit);
                        isExportXml = false;

                    }
                    message = ProcessExportXmlDetailPlus(ref result, ref memoryStream, xuatXml12, isXML3176);
                    if (!String.IsNullOrEmpty(message))
                    {
                        paramExport.Messages.Add(message);
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



        string ProcessExportXmlDetail(ref bool isSuccess, ref MemoryStream memoryStream, ref MemoryStream memoryStreamXml12, bool viewXml, bool XuatXmlTT, bool XuatXml12, List<V_HIS_TREATMENT_12> hisTreatments, List<V_HIS_PATIENT_TYPE_ALTER> hisPatientTypeAlters,
            List<V_HIS_SERE_SERV_2> ListSereServ, List<HIS_DHST> listDhst, List<V_HIS_SERE_SERV_TEIN> listSereServTein,
            List<HIS_TRACKING> hisTrackings, List<V_HIS_SERE_SERV_PTTT> hisSereServPttts, List<HIS_EKIP_USER> ListEkipUser,
            List<V_HIS_BED_LOG> ListBedlog, List<HIS_DEBATE> listDebate, List<V_HIS_BABY> listBaby, List<V_HIS_MEDICAL_ASSESSMENT> listMedicalAssessment, List<HIS_HIV_TREATMENT> listHivTreatment, List<V_HIS_SERE_SERV_SUIN> listSereServSuin, List<HIS_TUBERCULOSIS_TREAT> lstTuberculosisTreat, List<HIS_EXP_MEDIMATE_USED> listExpMedimateUsed, bool isXML3176)
        {
            string result = "";
            Dictionary<string, List<string>> DicErrorMess = new Dictionary<string, List<string>>();
            try
            {
                XuatXml12 = XuatXml12 && TypeXml().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Contains("12");
                Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>> dicPatientTypeAlter = new Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>>();
                Dictionary<long, List<V_HIS_SERE_SERV_2>> dicSereServ = new Dictionary<long, List<V_HIS_SERE_SERV_2>>();
                Dictionary<long, List<V_HIS_SERE_SERV_TEIN>> dicSereServTein = new Dictionary<long, List<V_HIS_SERE_SERV_TEIN>>();
                Dictionary<long, List<V_HIS_SERE_SERV_SUIN>> dicSereServSuin = new Dictionary<long, List<V_HIS_SERE_SERV_SUIN>>();
                Dictionary<long, List<V_HIS_SERE_SERV_PTTT>> dicSereServPttt = new Dictionary<long, List<V_HIS_SERE_SERV_PTTT>>();
                Dictionary<long, List<V_HIS_BED_LOG>> dicBedLog = new Dictionary<long, List<V_HIS_BED_LOG>>();
                Dictionary<long, List<HIS_TRACKING>> dicTracking = new Dictionary<long, List<HIS_TRACKING>>();
                Dictionary<long, List<HIS_EKIP_USER>> dicEkipUser = new Dictionary<long, List<HIS_EKIP_USER>>();
                Dictionary<long, List<V_HIS_BABY>> dicBaby = new Dictionary<long, List<V_HIS_BABY>>();
                Dictionary<long, List<HIS_DEBATE>> dicDebate = new Dictionary<long, List<HIS_DEBATE>>();
                Dictionary<long, List<HIS_DHST>> dicDhstList = new Dictionary<long, List<HIS_DHST>>();
                Dictionary<long, List<V_HIS_MEDICAL_ASSESSMENT>> dicMedicalAssessment = new Dictionary<long, List<V_HIS_MEDICAL_ASSESSMENT>>();
                Dictionary<long, HIS_HIV_TREATMENT> dicHivTreatment = new Dictionary<long, HIS_HIV_TREATMENT>();
                Dictionary<long, HIS_TUBERCULOSIS_TREAT> dicTuberculosisTreat = new Dictionary<long, HIS_TUBERCULOSIS_TREAT>();
                Dictionary<long, List<HIS_EXP_MEDIMATE_USED>> dicExpUsedByExpMestMedicineId = new Dictionary<long, List<HIS_EXP_MEDIMATE_USED>>();
                Dictionary<long, List<HIS_EXP_MEDIMATE_USED>> dicExpUsedByExpMestMaterialId = new Dictionary<long, List<HIS_EXP_MEDIMATE_USED>>();

                if (lstTuberculosisTreat != null && lstTuberculosisTreat.Count > 0)
                {
                    foreach (var item in lstTuberculosisTreat)
                    {
                        if (!dicTuberculosisTreat.ContainsKey(item.TREATMENT_ID))
                            dicTuberculosisTreat[item.TREATMENT_ID] = new HIS_TUBERCULOSIS_TREAT();
                        dicTuberculosisTreat[item.TREATMENT_ID] = item;
                    }
                }

                if (hisPatientTypeAlters != null && hisPatientTypeAlters.Count > 0)
                {
                    foreach (var item in hisPatientTypeAlters)
                    {
                        if (!dicPatientTypeAlter.ContainsKey(item.TREATMENT_ID))
                            dicPatientTypeAlter[item.TREATMENT_ID] = new List<V_HIS_PATIENT_TYPE_ALTER>();
                        dicPatientTypeAlter[item.TREATMENT_ID].Add(item);
                    }
                }

                //huannh
                if (ListSereServ != null && ListSereServ.Count > 0)
                {
                    bool allowZeroPrice = HisConfigCFG.QD_130_BYT__LAY_CA_DVU_0_DONG == "1";
                    Dictionary<long, List<HIS_EKIP_USER>> dicEkipUserByEkipId = null;
                    if (ListEkipUser != null && ListEkipUser.Count > 0)
                    {
                        dicEkipUserByEkipId = new Dictionary<long, List<HIS_EKIP_USER>>();
                        foreach (var eu in ListEkipUser)
                        {
                            if (!dicEkipUserByEkipId.ContainsKey(eu.EKIP_ID))
                                dicEkipUserByEkipId[eu.EKIP_ID] = new List<HIS_EKIP_USER>();
                            dicEkipUserByEkipId[eu.EKIP_ID].Add(eu);
                        }
                    }

                    foreach (var sereServ in ListSereServ)
                    {
                        bool addSereServ;
                        if (allowZeroPrice)
                        {
                            addSereServ = (sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && (sereServ.PRICE > 0 || sereServ.PRICE == 0))
                                || sereServ.IS_NO_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        }
                        else
                        {
                            addSereServ = (sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.PRICE > 0)
                                || sereServ.IS_NO_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        }

                        if (sereServ.AMOUNT > 0 && sereServ.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.TDL_TREATMENT_ID.HasValue && addSereServ)
                        {
                            if (!dicSereServ.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                dicSereServ[sereServ.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_2>();
                            dicSereServ[sereServ.TDL_TREATMENT_ID.Value].Add(sereServ);
                        }

                        if (sereServ.EKIP_ID.HasValue && dicEkipUserByEkipId != null && sereServ.TDL_TREATMENT_ID.HasValue)
                        {
                            if (dicEkipUserByEkipId.TryGetValue(sereServ.EKIP_ID.Value, out var ekips) && ekips.Count > 0)
                            {
                                if (!dicEkipUser.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                    dicEkipUser[sereServ.TDL_TREATMENT_ID.Value] = new List<HIS_EKIP_USER>();
                                dicEkipUser[sereServ.TDL_TREATMENT_ID.Value].AddRange(ekips);
                            }
                        }
                    }
                }

                if (listSereServTein != null && listSereServTein.Count > 0)
                {
                    foreach (var ssTein in listSereServTein)
                    {
                        if (!ssTein.TDL_TREATMENT_ID.HasValue) continue;

                        if (!dicSereServTein.ContainsKey(ssTein.TDL_TREATMENT_ID.Value))
                            dicSereServTein[ssTein.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_TEIN>();

                        dicSereServTein[ssTein.TDL_TREATMENT_ID.Value].Add(ssTein);
                    }
                }

                if (listSereServSuin != null && listSereServSuin.Count > 0)
                {
                    foreach (var ssSuin in listSereServSuin)
                    {

                        if (!dicSereServSuin.ContainsKey(ssSuin.TDL_TREATMENT_ID))
                            dicSereServSuin[ssSuin.TDL_TREATMENT_ID] = new List<V_HIS_SERE_SERV_SUIN>();

                        dicSereServSuin[ssSuin.TDL_TREATMENT_ID].Add(ssSuin);
                    }
                }
                if (hisTrackings != null && hisTrackings.Count > 0)
                {
                    foreach (var tracking in hisTrackings)
                    {
                        if (!dicTracking.ContainsKey(tracking.TREATMENT_ID))
                            dicTracking[tracking.TREATMENT_ID] = new List<HIS_TRACKING>();

                        dicTracking[tracking.TREATMENT_ID].Add(tracking);
                    }
                }
                if (listBaby != null && listBaby.Count > 0)
                {
                    foreach (var baby in listBaby)
                    {
                        if (!dicBaby.ContainsKey(baby.TREATMENT_ID))
                            dicBaby[baby.TREATMENT_ID] = new List<V_HIS_BABY>();

                        dicBaby[baby.TREATMENT_ID].Add(baby);
                    }
                }
                if (listHivTreatment != null && listHivTreatment.Count > 0)
                {
                    listHivTreatment = listHivTreatment.OrderBy(o => o.ID).ToList();
                    foreach (var hivTreatment in listHivTreatment)
                    {
                        dicHivTreatment[hivTreatment.TREATMENT_ID] = hivTreatment;
                    }
                }
                if (hisSereServPttts != null && hisSereServPttts.Count > 0)
                {
                    foreach (var ssPttt in hisSereServPttts)
                    {
                        if (!ssPttt.TDL_TREATMENT_ID.HasValue) continue;

                        if (!dicSereServPttt.ContainsKey(ssPttt.TDL_TREATMENT_ID.Value))
                            dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_PTTT>();

                        dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value].Add(ssPttt);
                    }
                }

                if (listDhst != null && listDhst.Count > 0)
                {
                    foreach (var item in listDhst)
                    {
                        if (!dicDhstList.ContainsKey(item.TREATMENT_ID))
                            dicDhstList[item.TREATMENT_ID] = new List<HIS_DHST>();

                        dicDhstList[item.TREATMENT_ID].Add(item);
                    }
                }

                if (ListBedlog != null && ListBedlog.Count > 0)
                {
                    foreach (var bed in ListBedlog)
                    {
                        if (!dicBedLog.ContainsKey(bed.TREATMENT_ID))
                            dicBedLog[bed.TREATMENT_ID] = new List<V_HIS_BED_LOG>();

                        dicBedLog[bed.TREATMENT_ID].Add(bed);
                    }
                }

                if (listDebate != null && listDebate.Count > 0)
                {
                    foreach (var item in listDebate)
                    {
                        if (!dicDebate.ContainsKey(item.TREATMENT_ID))
                            dicDebate[item.TREATMENT_ID] = new List<HIS_DEBATE>();

                        dicDebate[item.TREATMENT_ID].Add(item);
                    }
                }
                if (listExpMedimateUsed != null && listExpMedimateUsed.Count > 0)
                {
                    foreach (var u in listExpMedimateUsed)
                    {
                        if (u.EXP_MEST_MEDICINE_ID.HasValue)
                        {
                            var k = u.EXP_MEST_MEDICINE_ID.Value;
                            if (!dicExpUsedByExpMestMedicineId.ContainsKey(k))
                                dicExpUsedByExpMestMedicineId[k] = new List<HIS_EXP_MEDIMATE_USED>();
                            dicExpUsedByExpMestMedicineId[k].Add(u);
                        }

                        if (u.EXP_MEST_MATERIAL_ID.HasValue)
                        {
                            var k = u.EXP_MEST_MATERIAL_ID.Value;
                            if (!dicExpUsedByExpMestMaterialId.ContainsKey(k))
                                dicExpUsedByExpMestMaterialId[k] = new List<HIS_EXP_MEDIMATE_USED>();
                            dicExpUsedByExpMestMaterialId[k].Add(u);
                        }
                    }
                }

                if (XuatXml12 && listMedicalAssessment != null && listMedicalAssessment.Count > 0)
                {
                    foreach (var item in listMedicalAssessment)
                    {
                        if (!dicMedicalAssessment.ContainsKey(item.TREATMENT_ID))
                            dicMedicalAssessment[item.TREATMENT_ID] = new List<V_HIS_MEDICAL_ASSESSMENT>();

                        dicMedicalAssessment[item.TREATMENT_ID].Add(item);
                    }
                }
                string connect_infor = HisConfigCFG.QD_130_BYT__CONNECTION_INFO;
                string username = null, password = null, address = null, typeXml = null;
                string xml130Api = null, xmlGdykApi = null;
                List<string> connectInfors = new List<string>();
                int count = 0;
                if (string.IsNullOrEmpty(connect_infor))
                {

                }
                else
                {
                    connectInfors = connect_infor.Split('|').ToList();
                }
                try
                {
                    address = connectInfors[0];
                    username = connectInfors[1];
                    password = connectInfors[2];
                    typeXml = connectInfors[3];
                    xml130Api = connectInfors[4];
                    xmlGdykApi = connectInfors[5];
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error("Key cấu hình hệ thống chỉ thiết lập 3 giá trị");
                }

                var totalMaterialTypeData = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                var totalHeinMediOrgData = BackendDataWorker.Get<HIS_MEDI_ORG>();
                var totalHeinPatientTypeData = BackendDataWorker.Get<HIS_HEIN_PATIENT_TYPE>();
                var totalPatientTypeData = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                var totalIcdData = BackendDataWorker.Get<HIS_ICD>();
                var totalServiceData = BackendDataWorker.Get<V_HIS_SERVICE>();
                var totalEmployeeData = BackendDataWorker.Get<HIS_EMPLOYEE>();
                var totalDepartmentData = HisConfigCFG.QD_130_BVT_XML1_MA_KHOA_OPTION == "1"
                    ? BackendDataWorker.Get<HIS_DEPARTMENT>()
                    : null;
                var serverInfo = new ServerInfo() { Username = username, Password = password, Address = address, TypeXml = typeXml, Xml130Api = xml130Api, XmlGdykApi = xmlGdykApi };

                foreach (var treatment in hisTreatments)
                {
                    InputADO ado = new InputADO();
                    ado.Treatment = treatment;
                    if (dicPatientTypeAlter.ContainsKey(treatment.ID))
                    {
                        ado.ListPatientTypeAlter = dicPatientTypeAlter[treatment.ID];
                    }

                    if (!dicSereServ.ContainsKey(treatment.ID))
                    {
                        var errorSereServ = "Hồ sơ không có dịch vụ";
                        if (!DicErrorMess.ContainsKey(errorSereServ))
                        {
                            DicErrorMess[errorSereServ] = new List<string>();
                        }

                        DicErrorMess[errorSereServ].Add(treatment.TREATMENT_CODE);
                        continue;
                    }

                    ado.ListSereServ = dicSereServ.ContainsKey(treatment.ID) ? dicSereServ[treatment.ID] : null;

                    if (dicDhstList.ContainsKey(treatment.ID))
                    {
                        ado.ListDhst = dicDhstList[treatment.ID];
                    }

                    if (dicSereServTein.ContainsKey(treatment.ID))
                    {
                        ado.ListSereServTein = dicSereServTein[treatment.ID];
                    }
                    if (dicSereServSuin.ContainsKey(treatment.ID))
                    {
                        ado.vSereServSuin = dicSereServSuin[treatment.ID];
                    }
                    if (dicSereServPttt.ContainsKey(treatment.ID))
                    {
                        ado.ListSereServPttt = dicSereServPttt[treatment.ID];
                    }

                    if (dicBedLog.ContainsKey(treatment.ID))
                    {
                        ado.ListBedLog = dicBedLog[treatment.ID];
                    }

                    if (dicTracking.ContainsKey(treatment.ID))
                    {
                        ado.ListTracking = dicTracking[treatment.ID];
                    }

                    if (dicEkipUser.ContainsKey(treatment.ID))
                    {
                        ado.ListEkipUser = dicEkipUser[treatment.ID].Distinct().ToList();
                    }

                    if (dicDebate.ContainsKey(treatment.ID))
                    {
                        ado.ListDebate = dicDebate[treatment.ID];
                    }

                    if (dicBaby.ContainsKey(treatment.ID))
                    {
                        ado.ListBaby = dicBaby[treatment.ID];
                    }
                    if (XuatXml12)
                    {
                        if (dicMedicalAssessment.ContainsKey(treatment.ID))
                        {
                            ado.ListMedicalAssessment = dicMedicalAssessment[treatment.ID];
                        }
                        else
                        {
                            // Thêm dòng này để XML12 không bị null list
                            ado.ListMedicalAssessment = new List<V_HIS_MEDICAL_ASSESSMENT>();
                        }
                    }
                    if (dicHivTreatment.ContainsKey(treatment.ID))
                    {
                        ado.HivTreatment = dicHivTreatment[treatment.ID];
                    }
                    ado.TotalMaterialTypeData = totalMaterialTypeData;
                    ado.TotalHeinMediOrgData = totalHeinMediOrgData;
                    //DLL His.Bhyt.ExportXml.XML130 tren may nay CHUA co property TotalHeinPatientTypeData
                    //(ban moi hon o may dev) -> gan bang reflection de build duoc voi ca DLL cu lan moi.
                    SetAdoPropIfExists(ado, "TotalHeinPatientTypeData", totalHeinPatientTypeData);
                    ado.TotalConfigData = NewConfig;
                    ado.TotalPatientTypeData = totalPatientTypeData;
                    ado.TotalIcdData = totalIcdData;
                    ado.TotalSericeData = totalServiceData;
                    ado.TotalEmployeeData = totalEmployeeData;
                    var usedList = new List<HIS_EXP_MEDIMATE_USED>();

                    if (ado.ListSereServ != null && ado.ListSereServ.Count > 0)
                    {
                        foreach (var ss in ado.ListSereServ)
                        {
                            if (ss.EXP_MEST_MEDICINE_ID.HasValue)
                            {
                                var k = ss.EXP_MEST_MEDICINE_ID.Value;
                                if (dicExpUsedByExpMestMedicineId.TryGetValue(k, out var lst))
                                    usedList.AddRange(lst);
                            }

                            if (ss.EXP_MEST_MATERIAL_ID.HasValue)
                            {
                                var k = ss.EXP_MEST_MATERIAL_ID.Value;
                                if (dicExpUsedByExpMestMaterialId.TryGetValue(k, out var lst))
                                    usedList.AddRange(lst);
                            }
                        }
                    }

                    ado.ListExpMedimateUsed = usedList
                        .GroupBy(x => x.ID)  // hoặc khóa tự nhiên của used record
                        .Select(g => g.First())
                        .ToList();

                    if (totalDepartmentData != null)
                    {
                        ado.ListDepartment = totalDepartmentData;
                    }
                    ado.serverInfo = serverInfo;
                    //if (!isNotFileSign)
                    //    ado.delegateSignXml = DataSignXML;
                    if (dicTuberculosisTreat.ContainsKey(treatment.ID))
                    {
                        ado.TuberculosisTreat = dicTuberculosisTreat[treatment.ID];
                    }
                    // Sử dụng tham số isXML3176 thay vì kiểm tra checkbox trực tiếp
                    ado.IS_3176 = isXML3176;
                    Inventec.Common.Logging.LogSystem.Debug(
                        "ProcessExportXmlDetail - TreatmentCode: " + treatment.TREATMENT_CODE +
                        ", isXML3176 param: " + isXML3176 +
                        " → ado.IS_3176 = " + ado.IS_3176 +
                        " → Xuất XML " + (isXML3176 ? "3176" : "130"));
                    His.Bhyt.ExportXml.XML130.CreateXmlProcessor xmlProcessor = new His.Bhyt.ExportXml.XML130.CreateXmlProcessor(ado);

                    string errorMess = "";
                    string errorMessXml12 = "";
                    string fullFileName = "";
                    string saveFilePath = "";
                    string saveFilePathXml12 = "";
                    string fullFileNameCollinearXml = "";
                    string saveFilePathCollinearXml = "";


                    if (XuatXmlTT)
                    {
                        fullFileNameCollinearXml = xmlProcessor.GetFileNameCollinear();
                        saveFilePathCollinearXml = String.Format("{0}/{1}", this.savePathADO.pathCollinearXml, fullFileNameCollinearXml);

                        var rsXmlTT = xmlProcessor.RunCollinearXml(ref errorMess);
                        if (!String.IsNullOrWhiteSpace(errorMess))
                        {
                            Inventec.Common.Logging.LogSystem.Error("Run130_TT: " + errorMess);
                        }
                        if (rsXmlTT != null)
                        {
                            using (FileStream file = new FileStream(saveFilePathCollinearXml, FileMode.Create, FileAccess.Write))
                            {
                                rsXmlTT.WriteTo(file);
                            }
                            rsXmlTT.Close();
                            isSuccess = true;
                        }
                        else
                        {
                            if (!DicErrorMess.ContainsKey(errorMess))
                            {
                                DicErrorMess[errorMess] = new List<string>();
                            }

                            DicErrorMess[errorMess].Add(treatment.TREATMENT_CODE);
                        }
                    }
                    else
                    {
                        if (!viewXml)
                        {
                            fullFileName = xmlProcessor.GetFileName();
                            saveFilePath = String.Format("{0}/{1}", this.savePathADO.pathXml, fullFileName);
                            saveFilePathXml12 = String.Format("{0}/{1}{2}", this.savePathADO.pathXmlGDYK, "XML12_", fullFileName);
                        }
                        var rs = xmlProcessor.Run(ref errorMess);
                        var rsXml12 = XuatXml12 ? xmlProcessor.RunXml12(ref errorMessXml12) : null;
                        if (!String.IsNullOrWhiteSpace(errorMess))
                        {
                            Inventec.Common.Logging.LogSystem.Error("Run130: " + errorMess);
                        }
                        if (!String.IsNullOrWhiteSpace(errorMessXml12))
                        {
                            Inventec.Common.Logging.LogSystem.Error("Run130_XML12: " + errorMessXml12);
                        }
                        if (rs != null)
                        {
                            if (viewXml)
                            {
                                memoryStream = rs;
                            }
                            else
                            {
                                using (FileStream file = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write))
                                {
                                    rs.WriteTo(file);
                                }
                                rs.Close();
                            }
                            isSuccess = true;
                        }
                        else
                        {
                            if (!DicErrorMess.ContainsKey(errorMess))
                            {
                                DicErrorMess[errorMess] = new List<string>();
                            }

                            DicErrorMess[errorMess].Add(treatment.TREATMENT_CODE);
                        }
                        if (rsXml12 != null)
                        {
                            memoryStreamXml12 = rsXml12;
                            using (FileStream file12 = new FileStream(saveFilePathXml12, FileMode.Create, FileAccess.Write))
                            {
                                rsXml12.WriteTo(file12);
                            }
                            rsXml12.Close();
                        }
                    }

                    if (isNotFileSign == false && SettingSignADO != null)
                    {
                        string currentDirectory = Directory.GetCurrentDirectory();

                        string tempFolderPath = Path.Combine(currentDirectory, "Temp");
                        Directory.CreateDirectory(tempFolderPath);
                        fullFileName = xmlProcessor.GetFileName();
                        string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                        File.Create(tempFilePath).Close();

                        string pathAfterFileSign = "";
                        WcfSignDCO wcfSignDCO = null;
                        if (SettingSignADO.IsHsm)
                        {
                            var xmlBase64 = SourceFileSignApi(ReadFileContent(!string.IsNullOrEmpty(saveFilePathCollinearXml) ? (saveFilePathCollinearXml) : (saveFilePath)));
                            if (!string.IsNullOrEmpty(xmlBase64))
                            {
                                try
                                {
                                    var xmlBytes = Convert.FromBase64String(xmlBase64);
                                    File.WriteAllBytes(tempFilePath, xmlBytes);
                                    pathAfterFileSign = tempFilePath;
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error("Error saving xmlBase64 to file: " + ex);
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (File.Exists(saveFilePathCollinearXml))
                                    {
                                        File.Delete(saveFilePathCollinearXml);
                                    }
                                    if (File.Exists(saveFilePath))
                                    {
                                        File.Delete(saveFilePath);
                                    }
                                }
                                catch (IOException ioEx)
                                {
                                    Inventec.Common.Logging.LogSystem.Error("File đang bị khóa, chưa xóa được: " + ioEx);
                                }
                            }
                        }
                        else
                        {
                            wcfSignDCO = new WcfSignDCO();
                            wcfSignDCO.SerialNumber = SettingSignADO.SerialNumber;
                            wcfSignDCO.OutputFile = tempFilePath;
                            wcfSignDCO.PIN = "";
                            if (!string.IsNullOrEmpty(saveFilePathCollinearXml))
                            {
                                wcfSignDCO.SourceFile = saveFilePathCollinearXml;
                            }
                            else
                            {
                                wcfSignDCO.SourceFile = saveFilePath;
                            }
                            wcfSignDCO.fieldSigned = "CHUKYDONVI";
                            string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                            SignProcessorClient signProcessorClient = new SignProcessorClient();
                            if (VerifyServiceSignProcessorIsRunning())
                            {
                                var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                                if (wcfSignResultDCO != null && wcfSignResultDCO.Success)
                                {
                                    pathAfterFileSign = wcfSignResultDCO.OutputFile;
                                    if (!File.Exists(pathAfterFileSign) || new FileInfo(pathAfterFileSign).Length == 0)
                                    {
                                        XtraMessageBox.Show("Ký số thất bại: file output không tồn tại hoặc rỗng.");
                                        isSuccess = false;
                                        return "";
                                    }
                                    Inventec.Common.Logging.LogSystem.Debug("wcfSignResultDCO.OutputFile: " + Inventec.Common.Logging.LogUtil.TraceData("output file", wcfSignResultDCO.OutputFile));
                                }
                            }
                        }
                        if (this.savePathADO == null || string.IsNullOrEmpty(this.savePathADO.pathCollinearXml))
                        {
                            XtraMessageBox.Show("Vui lòng thiết lập thư mục lưu trữ trước khi xuất dữ liệu.", Resources.ResourceMessageLang.ThongBao);
                            btnSavePath_Click(null, null);
                        }
                        if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXml))
                        {
                            if (!string.IsNullOrEmpty(pathAfterFileSign))
                            {
                                if (wcfSignDCO != null)
                                {
                                    if (wcfSignDCO.SourceFile.Trim() != pathAfterFileSign.Trim())
                                    {
                                        if (File.Exists(wcfSignDCO.SourceFile))
                                        {
                                            File.Delete(wcfSignDCO.SourceFile);
                                        }
                                    }
                                    File.Copy(pathAfterFileSign, wcfSignDCO.SourceFile);
                                }
                                else if (SettingSignADO.IsHsm)
                                {
                                    var sourceFile = !string.IsNullOrEmpty(saveFilePathCollinearXml) ? (saveFilePathCollinearXml) : (saveFilePath);
                                    if (sourceFile.Trim() != pathAfterFileSign.Trim())
                                    {
                                        if (File.Exists(sourceFile))
                                        {
                                            File.Delete(sourceFile);
                                        }
                                    }
                                    File.Copy(pathAfterFileSign, sourceFile);
                                }
                            }
                        }

                        foreach (string ifile in Directory.GetFiles(tempFolderPath))
                        {
                            File.Delete(ifile);
                        }
                    }
                    //count++;
                }
                if (DicErrorMess.Count > 0)
                {
                    foreach (var item in DicErrorMess)
                    {
                        result += String.Format("{0}:{1}. ", item.Key, String.Join(",", item.Value));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private string SourceFileSignApi(string xmlBase64Source)
        {
            string result = null;
            try
            {
                CommonParam param = new CommonParam();
                EMR.SDO.SignXmlBhytSDO signXmlBhytSDO = new EMR.SDO.SignXmlBhytSDO();
                signXmlBhytSDO.XmlBase64 = xmlBase64Source;
                signXmlBhytSDO.TagStoreSignatureValue = "CHUKYDONVI";
                signXmlBhytSDO.ConfigData = new EMR.SDO.XmlConfigDataSDO() { HsmSerialNumber = SettingSignADO.SerialNumber, HsmType = SettingSignADO.Id, HsmUserCode = SettingSignADO.Name, Password = SettingSignADO.Password, SecretKey = SettingSignADO.SercetKey, IdentityNumber = SettingSignADO.CccdNumber };
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<string>("api/EmrSign/SignXmlBhyt", ApiConsumer.ApiConsumers.EmrConsumer, signXmlBhytSDO, SessionManager.ActionLostToken, param);
                if (param != null && param.Messages != null && param.Messages.Count > 0)
                {
                    string message = string.Join(Environment.NewLine, param.Messages);
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Inventec.Common.Logging.LogSystem.Warn(message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string TypeXml()
        {
            string result = "";

            try
            {
                List<string> connectInfors = new List<string>();
                int count = 0;
                if (string.IsNullOrEmpty(HisConfigCFG.QD_130_BYT__CONNECTION_INFO))
                {

                }
                else
                {
                    connectInfors = HisConfigCFG.QD_130_BYT__CONNECTION_INFO.Split('|').ToList();
                }
                try
                {
                    result = connectInfors[3];
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error("Key cấu hình hệ thống chỉ thiết lập 3 giá trị");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        string ProcessExportXmlDetailPlus(ref bool isSuccess, ref MemoryStream memoryStream, bool XuatXml12, bool isXML3176)
        {
            string result = "";
            try
            {
                string connect_infor = HisConfigCFG.QD_130_BYT__CONNECTION_INFO;
                string username = null, password = null, address = null, typeXml = null;
                string xml130Api = null, xmlGdykApi = null;
                List<string> connectInfors = new List<string>();
                if (string.IsNullOrEmpty(connect_infor))
                {

                }
                else
                {
                    connectInfors = connect_infor.Split('|').ToList();
                }
                try
                {
                    address = connectInfors[0];
                    username = connectInfors[1];
                    password = connectInfors[2];
                    typeXml = connectInfors[3];
                    xml130Api = connectInfors[4];
                    xmlGdykApi = connectInfors[5];
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error("Key cấu hình hệ thống chỉ thiết lập 3 giá trị");
                }

                InputADO ado = new InputADO();
                ado.IS_3176 = isXML3176;
                ado.ListTreatment = HisTreatments;
                ado.ListPatientTypeAlter = ListPatientTypeAlter;
                ado.ListSereServ = ListSereServ;
                ado.ListSereServTein = HisSereServTeins;
                ado.ListSereServPttt = HisSereServPttts;
                ado.ListBedLog = ListBedlog;
                ado.ListTracking = HisTrackings;
                ado.ListEkipUser = ListEkipUser;
                ado.ListBaby = ListBaby;
                ado.ListDebate = ListDebates;
                ado.ListDhst = ListDhst;
                ado.ListMedicalAssessment = ListMedicalAssessment ?? new List<V_HIS_MEDICAL_ASSESSMENT>();
                ado.ListHivTreatment = ListHivTreatment;
                ado.vSereServSuin = HisSereServSuin;
                ado.ListTuberculosisTreat = ListTuberculosisTreat;
                ado.TotalMaterialTypeData = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                ado.TotalHeinMediOrgData = BackendDataWorker.Get<HIS_MEDI_ORG>();
                //DLL cu chua co property -> reflection (xem chu thich o SetAdoPropIfExists).
                SetAdoPropIfExists(ado, "TotalHeinPatientTypeData", BackendDataWorker.Get<HIS_HEIN_PATIENT_TYPE>());
                ado.TotalConfigData = NewConfig;
                ado.TotalPatientTypeData = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                ado.TotalIcdData = BackendDataWorker.Get<HIS_ICD>();
                ado.TotalSericeData = BackendDataWorker.Get<V_HIS_SERVICE>();
                ado.TotalEmployeeData = BackendDataWorker.Get<HIS_EMPLOYEE>();
                ado.ListExpMedimateUsed = ListExpMedimateUsed ?? new List<HIS_EXP_MEDIMATE_USED>();
                if (HisConfigCFG.QD_130_BVT_XML1_MA_KHOA_OPTION == "1")
                {
                    ado.ListDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
                }
                ado.serverInfo = new ServerInfo() { Username = username, Password = password, Address = address, TypeXml = typeXml, Xml130Api = xml130Api, XmlGdykApi = xmlGdykApi };

                //ado.delegateSignXml = DataSignXML;
                His.Bhyt.ExportXml.XML130.CreateXmlProcessor xmlProcessor = new His.Bhyt.ExportXml.XML130.CreateXmlProcessor(ado);

                string errorMess = "";
                string errorMessXml12 = "";
                string fullFileName = "";
                string saveFilePath = "";
                string saveFilePathXml12 = "";


                fullFileName = "DATA_XML_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml";
                saveFilePath = String.Format("{0}/{1}", IsProcessingExcel ? this.PathTempXml : this.savePathADO.pathXml, fullFileName);
                saveFilePathXml12 = String.Format("{0}/{1}{2}", IsProcessingExcel ? this.PathTempXml : this.savePathADO.pathXmlGDYK, "XML12_", fullFileName);
                if (IsProcessingExcel)
                {
                    saveFileExcel = saveFilePath;
                    saveFileExcel12 = saveFilePathXml12;
                }
                xmlProcessor = new His.Bhyt.ExportXml.XML130.CreateXmlProcessor(ado);
                var rs = xmlProcessor.RunPlus(saveFilePath, ref errorMess);
                var rsXml12 = XuatXml12 ? xmlProcessor.RunXml12Plus(saveFilePathXml12, ref errorMessXml12) : null;
                if (!String.IsNullOrWhiteSpace(errorMess))
                {
                    Inventec.Common.Logging.LogSystem.Error("Run130: " + errorMess);
                }
                if (!String.IsNullOrWhiteSpace(errorMessXml12))
                {
                    Inventec.Common.Logging.LogSystem.Error("Run130_XML12: " + errorMessXml12);
                }
                if (rs != null)
                {
                    FileStream file = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
                    rs.WriteTo(file);
                    file.Close();
                    rs.Close();
                    isSuccess = true;
                }
                if (rsXml12 != null)
                {
                    FileStream file12 = new FileStream(saveFilePathXml12, FileMode.Create, FileAccess.Write);
                    rsXml12.WriteTo(file12);
                    file12.Close();
                    rsXml12.Close();
                    isSuccess = true;
                }

                if (!IsProcessingExcel && isNotFileSign == false && SettingSignADO != null)
                {
                    string currentDirectory = Directory.GetCurrentDirectory();
                    string tempFolderPath = Path.Combine(currentDirectory, "Temp");
                    Directory.CreateDirectory(tempFolderPath);
                    string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                    File.Create(tempFilePath).Close();
                    WcfSignDCO wcfSignDCO = null;
                    string pathAfterFileSign = null;
                    if (SettingSignADO.IsHsm)
                    {
                        var xmlBase64 = SourceFileSignApi(ReadFileContent(!string.IsNullOrEmpty(saveFilePath) ? saveFilePath : saveFilePathXml12));
                        if (!string.IsNullOrEmpty(xmlBase64))
                        {
                            try
                            {
                                var xmlBytes = Convert.FromBase64String(xmlBase64);
                                File.WriteAllBytes(tempFilePath, xmlBytes);
                                pathAfterFileSign = tempFilePath;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error("Error saving xmlBase64 to file: " + ex);
                            }
                        }
                        else
                        {
                            if (File.Exists(saveFilePath))
                            {
                                File.Delete(saveFilePath);
                            }
                            if (File.Exists(saveFilePathXml12))
                            {
                                File.Delete(saveFilePathXml12);
                            }
                        }
                        if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXml))
                        {
                            if (!string.IsNullOrEmpty(pathAfterFileSign))
                            {
                                // With this updated code to allow overwrite:
                                if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXml))
                                {
                                    if (!string.IsNullOrEmpty(pathAfterFileSign))
                                    {
                                        var destFile = !string.IsNullOrEmpty(saveFilePath) ? saveFilePath : saveFilePathXml12;
                                        if (File.Exists(destFile))
                                        {
                                            File.Delete(destFile);
                                        }
                                        File.Copy(pathAfterFileSign, destFile);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        wcfSignDCO = new WcfSignDCO();
                        wcfSignDCO.SerialNumber = SettingSignADO.SerialNumber;
                        wcfSignDCO.OutputFile = tempFilePath;
                        wcfSignDCO.PIN = "";
                        if (!string.IsNullOrEmpty(saveFilePath))
                        {
                            wcfSignDCO.SourceFile = saveFilePath;
                        }
                        else
                        {
                            wcfSignDCO.SourceFile = saveFilePathXml12;
                        }
                        wcfSignDCO.fieldSigned = "CHUKYDONVI";
                        string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                        SignProcessorClient signProcessorClient = new SignProcessorClient();
                        if (VerifyServiceSignProcessorIsRunning())
                        {
                            var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                            pathAfterFileSign = wcfSignDCO.SourceFile;
                            if (wcfSignResultDCO != null && wcfSignResultDCO.Success)
                            {
                                pathAfterFileSign = wcfSignResultDCO.OutputFile;
                                Inventec.Common.Logging.LogSystem.Debug("wcfSignResultDCO.OutputFile: " + Inventec.Common.Logging.LogUtil.TraceData("output file", wcfSignResultDCO.OutputFile));

                                if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXml))
                                {
                                    File.Copy(wcfSignDCO.OutputFile, pathAfterFileSign);
                                }
                            }
                        }
                    }

                    // Xóa tất cả các file trong thư mục temp
                    foreach (string ifile in Directory.GetFiles(tempFolderPath))
                    {
                        File.Delete(ifile);
                    }
                }
                result = errorMess;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
        private List<HIS_CONFIG> GetNewConfig()
        {
            List<HIS_CONFIG> result = null;
            try
            {
                CommonParam paramGet = new CommonParam();
                MOS.Filter.HisConfigFilter configFilter = new MOS.Filter.HisConfigFilter();
                configFilter.IS_ACTIVE = 1;
                result = new BackendAdapter(paramGet).Get<List<MOS.EFMODEL.DataModels.HIS_CONFIG>>("/api/HisConfig/Get", ApiConsumers.MosConsumer, configFilter, paramGet);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void BtnFind()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnExportXml()
        {
            try
            {
                btnExportXml_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void BtnLock()
        {
            try
            {
                btnLock_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void BtnUnLock()
        {
            try
            {
                btnUnlock_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #region Thread
        private void CreateThreadGetData(List<V_HIS_TREATMENT_1> listSelection)
        {
            try
            {
                System.Threading.Thread PatientTypeAlter = new System.Threading.Thread(ThreadGetPatientTypeAlter);
                System.Threading.Thread Baby = new System.Threading.Thread(ThreadGetBaby);
                System.Threading.Thread MedicalAssessment = new System.Threading.Thread(ThreadGetMedicalAssessment);
                System.Threading.Thread SereServ2 = new System.Threading.Thread(ThreadGetSereServ2);
                System.Threading.Thread Treatment12 = new System.Threading.Thread(ThreadGetTreatment12);
                System.Threading.Thread Dhst_Tracking = new System.Threading.Thread(ThreadGetDhst_Tracking);
                System.Threading.Thread SereServTein_PTTT = new System.Threading.Thread(ThreadGetSereServTein_PTTT);
                System.Threading.Thread SereServSuin = new System.Threading.Thread(ThreadGetSereServSuin);
                System.Threading.Thread TuberculosisTreat = new System.Threading.Thread(ThreadTuberculosisTreat);
                try
                {
                    TuberculosisTreat.Start(listSelection);
                    PatientTypeAlter.Start(listSelection);
                    Baby.Start(listSelection);
                    MedicalAssessment.Start(listSelection);
                    SereServ2.Start(listSelection);
                    Treatment12.Start(listSelection);
                    Dhst_Tracking.Start(listSelection);
                    SereServTein_PTTT.Start(listSelection);
                    SereServSuin.Start(listSelection);
                    TuberculosisTreat.Join();
                    PatientTypeAlter.Join();
                    Baby.Join();
                    MedicalAssessment.Join();
                    SereServ2.Join();
                    Treatment12.Join();
                    Dhst_Tracking.Join();
                    SereServTein_PTTT.Join();
                    SereServSuin.Join();
                }
                catch (Exception ex)
                {
                    TuberculosisTreat.Abort();
                    PatientTypeAlter.Abort();
                    Baby.Abort();
                    MedicalAssessment.Abort();
                    SereServ2.Abort();
                    Treatment12.Abort();
                    Dhst_Tracking.Abort();
                    SereServTein_PTTT.Abort();
                    SereServSuin.Abort();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ThreadTuberculosisTreat(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();

                    HisTuberculosisTreatFilter filter = new HisTuberculosisTreatFilter();
                    filter.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resulTein = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TUBERCULOSIS_TREAT>>("api/HisTuberculosisTreat/Get", ApiConsumers.MosConsumer, filter, param);
                    if (resulTein != null && resulTein.Count > 0)
                    {
                        ListTuberculosisTreat.AddRange(resulTein);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ThreadGetSereServSuin(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();

                    HisSereServSuinViewFilter ssTeinFilter = new HisSereServSuinViewFilter();
                    ssTeinFilter.TDL_TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resulTein = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_SUIN>>("api/HisSereServSuin/GetView", ApiConsumers.MosConsumer, ssTeinFilter, param);
                    if (resulTein != null && resulTein.Count > 0)
                    {
                        HisSereServSuin.AddRange(resulTein);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ThreadGetSereServTein_PTTT(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();

                    HisSereServTeinViewFilter ssTeinFilter = new HisSereServTeinViewFilter();
                    ssTeinFilter.TDL_TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resulTein = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, ssTeinFilter, param);
                    if (resulTein != null && resulTein.Count > 0)
                    {
                        HisSereServTeins.AddRange(resulTein);
                    }

                    HisSereServPtttViewFilter ssPtttFilter = new HisSereServPtttViewFilter();
                    ssPtttFilter.TDL_TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resultPttt = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/GetView", ApiConsumers.MosConsumer, ssPtttFilter, param);
                    if (resultPttt != null && resultPttt.Count > 0)
                    {
                        HisSereServPttts.AddRange(resultPttt);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetDhst_Tracking(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();

                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultDhst = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);
                    if (resultDhst != null && resultDhst.Count > 0)
                    {
                        ListDhst.AddRange(resultDhst);
                    }

                    HisTrackingFilter trackingFilter = new HisTrackingFilter();
                    trackingFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultTracking = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, trackingFilter, param);
                    if (resultTracking != null && resultTracking.Count > 0)
                    {
                        HisTrackings.AddRange(resultTracking);
                    }

                    HisDebateFilter debateFilter = new HisDebateFilter();
                    debateFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultDebate = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_DEBATE>>("api/HisDebate/Get", ApiConsumers.MosConsumer, debateFilter, param);
                    if (resultDebate != null && resultDebate.Count > 0)
                    {
                        ListDebates.AddRange(resultDebate);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetTreatment12(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();
                    HisTreatmentView12Filter treatmentFilter = new HisTreatmentView12Filter();
                    treatmentFilter.IDs = limit.Select(o => o.ID).ToList();
                    var resultTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_12>>("api/HisTreatment/GetView12", ApiConsumers.MosConsumer, treatmentFilter, param);
                    if (resultTreatment != null && resultTreatment.Count > 0)
                    {
                        HisTreatments.AddRange(resultTreatment);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetSereServ2(object obj)
        {
            try
            {
                if (obj == null) return;
                //List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;
                var listSelection = (List<V_HIS_TREATMENT_1>)obj;
                if (listSelection.Count == 0) return;

                // patient type TT ids dùng 1 lần
                List<long> patientTypeTtIds = null;
                if (isExportXml)
                {
                    if (this.patientTypeTTSelecteds != null && this.patientTypeTTSelecteds.Count > 0)
                        patientTypeTtIds = this.patientTypeTTSelecteds.Select(o => o.ID).ToList();
                }
                else
                {
                    if (this.configSync?.patientTypeTTIds != null && this.configSync.patientTypeTTIds.Count > 0)
                        patientTypeTtIds = this.configSync.patientTypeTTIds.ToList();
                }

                // cache tránh gọi used trùng
                var loadedMedIds = new HashSet<long>();
                var loadedMatIds = new HashSet<long>();
                var usedById = new Dictionary<long, HIS_EXP_MEDIMATE_USED>();


                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip += GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    var param = new CommonParam();
                    var ssFilter = new HisSereServView2Filter
                    {
                        TREATMENT_IDs = limit.Select(o => o.ID).ToList(),
                        PATIENT_TYPE_IDs = (patientTypeTtIds != null && patientTypeTtIds.Count > 0) ? patientTypeTtIds : null
                    };

                    var resultSS = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_2>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW_2,ApiConsumers.MosConsumer, ssFilter, param);

                    if (resultSS != null && resultSS.Count > 0)
                    {
                        ListSereServ.AddRange(resultSS);

                        //OverrideTransferMediOrgCodeForBlood(resultSS);

                        try
                        {
                            var usedFilter = new HisExpMedimateUsedFilter
                            {
                                TDL_TREATMENT_ID = this.currentTreatment.ID,
                            };

                            var usedRs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EXP_MEDIMATE_USED>>("api/HisExpMedimateUsed/Get", ApiConsumers.MosConsumer, usedFilter, param);

                            if (usedRs != null && usedRs.Count > 0)
                            {
                                foreach (var u in usedRs)
                                {
                                    if (u == null) continue;
                                    usedById[u.ID] = u;
                                }
                            }
                        }
                        catch (Exception exUsed)
                        {
                            Inventec.Common.Logging.LogSystem.Error("ThreadGetSereServ2 - Load HIS_EXP_MEDIMATE_USED error: " + exUsed);
                        }

                        // ===== 3) ekip user
                        var ekipIds = resultSS.Select(o => o.EKIP_ID ?? 0).Where(o => o != 0).Distinct().ToList();
                        if (ekipIds.Count > 0)
                        {
                            int skipEkip = 0;
                            while (ekipIds.Count - skipEkip > 0)
                            {
                                var limitLong = ekipIds.Skip(skipEkip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                                skipEkip += GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                                var ekipFilter = new HisEkipUserFilter { EKIP_IDs = limitLong };
                                var resultEkip = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EKIP_USER>>(
                                    "api/HisEkipUser/Get",
                                    ApiConsumers.MosConsumer,
                                    ekipFilter,
                                    param
                                );

                                if (resultEkip != null && resultEkip.Count > 0)
                                    ListEkipUser.AddRange(resultEkip);
                            }
                        }
                    }

                    // ===== 4) Bedlog 
                    var bedFilter = new HisBedLogViewFilter { TREATMENT_IDs = limit.Select(o => o.ID).ToList() };
                    var resultBed = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>(
                        "api/HisBedLog/GetView",
                        ApiConsumers.MosConsumer,
                        bedFilter,
                        param
                    );

                    if (resultBed != null && resultBed.Count > 0)
                        ListBedlog.AddRange(resultBed);
                }

                // add used 1 phát cuối, đã dedupe theo used.ID
                if (usedById.Count > 0)
                    ListExpMedimateUsed.AddRange(usedById.Values);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void OverrideTransferMediOrgCodeForBlood(List<V_HIS_SERE_SERV_2> sereServs)
        //{
        //    try
        //    {
        //        if (sereServs == null || sereServs.Count == 0) return;

        //        var bloodIds = sereServs
        //            .Where(o => o.BLOOD_ID.HasValue)
        //            .Select(o => o.BLOOD_ID.Value)
        //            .Distinct()
        //            .ToList();

        //        if (bloodIds.Count == 0) return;

        //        var bloodById = new Dictionary<long, HIS_BLOOD>();
        //        int skipBlood = 0;
        //        while (bloodIds.Count - skipBlood > 0)
        //        {
        //            var batchIds = bloodIds.Skip(skipBlood).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
        //            skipBlood += GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

        //            var paramBlood = new CommonParam();
        //            var bloodFilter = new HisBloodFilter { IDs = batchIds };
        //            var bloods = new BackendAdapter(paramBlood).Get<List<HIS_BLOOD>>(
        //                HisRequestUriStore.HIS_BLOOD_GET,
        //                ApiConsumers.MosConsumer,
        //                bloodFilter,
        //                paramBlood);

        //            if (bloods != null && bloods.Count > 0)
        //            {
        //                foreach (var b in bloods)
        //                {
        //                    if (b != null && !bloodById.ContainsKey(b.ID))
        //                        bloodById[b.ID] = b;
        //                }
        //            }
        //        }

        //        int overrideCount = 0;
        //        var missingIds = new List<long>();
        //        foreach (var ss in sereServs)
        //        {
        //            if (!ss.BLOOD_ID.HasValue) continue;

        //            HIS_BLOOD blood;
        //            if (bloodById.TryGetValue(ss.BLOOD_ID.Value, out blood))
        //            {
        //                ss.TRANSFER_MEDI_ORG_CODE = blood.TRANSFER_MEDI_ORG_CODE;
        //                overrideCount++;
        //            }
        //            else
        //            {
        //                missingIds.Add(ss.BLOOD_ID.Value);
        //            }
        //        }

        //        Inventec.Common.Logging.LogSystem.Debug(
        //            "OverrideTransferMediOrgCodeForBlood - DistinctBloodIds: " + bloodIds.Count
        //            + ", Overridden: " + overrideCount
        //            + (missingIds.Count > 0 ? ", BLOOD_ID không tìm thấy HIS_BLOOD: " + string.Join(",", missingIds) : ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void ThreadGetPatientTypeAlter(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();
                    HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                    filter.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resultPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (resultPatientTypeAlter != null && resultPatientTypeAlter.Count > 0)
                    {
                        ListPatientTypeAlter.AddRange(resultPatientTypeAlter);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ThreadGetBaby(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();
                    HisBabyViewFilter filter = new HisBabyViewFilter();
                    filter.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resultBaby = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (resultBaby != null && resultBaby.Count > 0)
                    {
                        ListBaby.AddRange(resultBaby);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ThreadGetMedicalAssessment(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();
                    HisMedicalAssessmentViewFilter filter = new HisMedicalAssessmentViewFilter();
                    filter.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resultMedicalAssessment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_MEDICAL_ASSESSMENT>>("api/HisMedicalAssessment/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (resultMedicalAssessment != null && resultMedicalAssessment.Count > 0)
                    {
                        ListMedicalAssessment.AddRange(resultMedicalAssessment);
                    }

                    HisHivTreatmentFilter filterHivTreatment = new HisHivTreatmentFilter();
                    filterHivTreatment.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resultHivTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_HIV_TREATMENT>>("api/HisHivTreatment/Get", ApiConsumers.MosConsumer, filterHivTreatment, param);
                    if (resultHivTreatment != null && resultHivTreatment.Count > 0)
                    {
                        ListHivTreatment.AddRange(resultHivTreatment);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        private void gridViewTreatment_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        var treatment1 = (V_HIS_TREATMENT_1)gridViewTreatment.GetRow(hi.RowHandle);
                        if (treatment1 != null)
                        {
                            if (hi.Column.FieldName == "ViewXML")
                            {
                                isNotFileSign = true;
                                CommonParam param = new CommonParam();
                                MemoryStream memoryStream = new MemoryStream();
                                MemoryStream memoryStreamXml12 = new MemoryStream();
                                bool success = false;
                                WaitingManager.Show();
                                List<V_HIS_TREATMENT_1> listTreatments = new List<V_HIS_TREATMENT_1>();
                                listTreatments.Add(treatment1);
                                btnExportXML3176 = chkXML3176.Checked;
                                Inventec.Common.Logging.LogSystem.Info("ViewXML - IS_3176 = " + btnExportXML3176);
                                Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click Begin");
                                //qtcode
                                //success = this.GenerateXml(ref param, ref memoryStream,ref memoryStreamXml12, true, false, true, listTreatments);
                                success = this.GenerateXml(ref param, ref memoryStream, true, false, true, listTreatments, chkXML3176.Checked);
                                isNotFileSign = false;
                                Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click End");
                                WaitingManager.Hide();
                                if (success && param.Messages.Count == 0)
                                {
                                    MessageManager.Show(this.ParentForm, param, success);
                                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.XMLViewer130").FirstOrDefault();
                                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.XMLViewer130'");
                                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                    {
                                        moduleData.RoomId = this.currentModule.RoomId;
                                        moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                                        List<object> listArgs = new List<object>();
                                        if (memoryStream != null)
                                            listArgs.Add(memoryStream);
                                        else
                                        {
                                            DevExpress.XtraEditors.XtraMessageBox.Show("Lỗi tạo xml");
                                            return;
                                        }
                                        listArgs.Add(moduleData);
                                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                                        if (extenceInstance == null)
                                        {
                                            throw new ArgumentNullException("moduleData is null");
                                        }

                                        ((Form)extenceInstance).ShowDialog();
                                    }
                                    else
                                    {
                                        MessageManager.Show(Resources.ResourceMessageLang.ChucNangChuaHoTroPhienBanHienTai);
                                    }
                                }
                                else if (!success && param.Messages.Count > 0)
                                {
                                    MessageManager.Show(param, success);
                                }

                                this.gridControlTreatment.RefreshDataSource();

                                SessionManager.ProcessTokenLost(param);
                            }
                            else if (hi.Column.FieldName == "VIEW_XML_CHECKIN" && !String.IsNullOrEmpty(treatment1.XML_CHECKIN_URL))
                            {
                                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.XMLViewer130").FirstOrDefault();
                                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.XMLViewer130'");
                                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                {
                                    moduleData.RoomId = this.currentModule.RoomId;
                                    moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                                    List<object> listArgs = new List<object>();
                                    MemoryStream TemplateStream = GetStreamByUrl(treatment1.XML_CHECKIN_URL);
                                    if (TemplateStream != null)
                                    {
                                        listArgs.Add(TemplateStream);
                                        listArgs.Add(moduleData);
                                        listArgs.Add((long)2); //truyen vao gia tri 2 de xem xml check-in
                                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                                        if (extenceInstance == null)
                                        {
                                            throw new ArgumentNullException("moduleData is null");
                                        }

                                        ((Form)extenceInstance).ShowDialog();
                                    }
                                    else
                                        MessageManager.Show("Tải file XML thất bại!");
                                }
                                else
                                {
                                    MessageManager.Show(Resources.ResourceMessageLang.ChucNangChuaHoTroPhienBanHienTai);
                                }
                            }
                            else if (hi.Column.FieldName == "ErrorLine" && treatment1.XML130_RESULT == 1 && !string.IsNullOrEmpty(treatment1.XML130_DESC))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(treatment1.XML130_DESC);
                            }
                            else if (hi.Column.FieldName == "XML_CHECKIN_RESULT_STR" && (treatment1.XML_CHECKIN_RESULT == 2 || treatment1.XML_CHECKIN_RESULT == 4) && !string.IsNullOrEmpty(treatment1.XML_CHECKIN_DESC))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(treatment1.XML_CHECKIN_DESC);
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
        private MemoryStream GetStreamByUrl(string url)
        {
            MemoryStream rs = null;
            try
            {
                rs = Inventec.Fss.Client.FileDownload.GetFile(url);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                rs = null;
            }
            return rs;
        }
        /// <summary>
        /// init combo branch
        /// </summary>
        private List<HIS_BRANCH> listBranchDataSource = BackendDataWorker.Get<HIS_BRANCH>().ToList();
        private void InitComboBranch()
        {
            try
            {
                InitCheck(CboBranch, SelectionGrid__cboBranch);
                InitCombo(CboBranch, listBranchDataSource, "BRANCH_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        /// init data doi tuong banh nhan
        /// lay du lieu tu RAM load len danh sach
        /// </summary>
        private List<HIS_PATIENT_TYPE> listPatientTypeDataSource = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
        private void InitComboPatientType()
        {
            InitCheck(cboPatientType, SelectionGrid__cboPatientType);
            InitCombo(cboPatientType, listPatientTypeDataSource, "PATIENT_TYPE_NAME");
        }
        /// <summary>
        /// init data doi tuong thanh toan
        /// </summary>
        private List<HIS_PATIENT_TYPE> listPatientTypeTTDataSource = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
        private void InitComboPatientTypeTT()
        {
            InitCheck(cboPatientTypeTT, SelectionGrid__cboPatientTypeTT);
            InitCombo(cboPatientTypeTT, listPatientTypeTTDataSource, "PATIENT_TYPE_NAME");
        }
        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 2;
                col2.Width = 200;
                col2.Caption = Resources.ResourceMessageLang.TatCa;
                cbo.Properties.PopupFormWidth = 250;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {

                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboPatientType(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_TYPE> sgSelectedNews = new List<HIS_PATIENT_TYPE>();
                    foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.patientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
                    this.patientTypeSelecteds.AddRange(sgSelectedNews);
                }
                this.cboPatientType.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboPatientTypeTT(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_TYPE> sgSelectedNews = new List<HIS_PATIENT_TYPE>();
                    foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.patientTypeTTSelecteds = new List<HIS_PATIENT_TYPE>();
                    this.patientTypeTTSelecteds.AddRange(sgSelectedNews);
                }
                this.cboPatientTypeTT.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboBranch(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {

                    List<HIS_BRANCH> sgSelectedNews = new List<HIS_BRANCH>();
                    foreach (MOS.EFMODEL.DataModels.HIS_BRANCH rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.BRANCH_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }

                    this.branchSelecteds = new List<HIS_BRANCH>();
                    this.branchSelecteds.AddRange(sgSelectedNews);
                }
                this.CboBranch.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void CboBranch_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                this.searchFilter.listBranch = new List<HIS_BRANCH>();
                foreach (MOS.EFMODEL.DataModels.HIS_BRANCH rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    this.searchFilter.listBranch.Add(rv);
                    sb.Append(rv.BRANCH_NAME.ToString());

                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        /// init combo doi tuong dieu tri
        /// </summary>
        private List<HIS_TREATMENT_TYPE> listTreatmentTypeDataSource = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().ToList();
        private void InitComboTreatmentType()
        {
            InitCheck(cboFilterTreatmentType, SelectionGrid__cboFilterTreatmentType);
            cboFilterTreatmentType.Properties.DataSource = listTreatmentTypeDataSource;
            cboFilterTreatmentType.Properties.DisplayMember = "TREATMENT_TYPE_NAME";
            cboFilterTreatmentType.Properties.ValueMember = "ID";
            DevExpress.XtraGrid.Columns.GridColumn col1 = cboFilterTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_CODE");
            col1.VisibleIndex = 1;
            col1.Width = 50;
            col1.Caption = " ";
            DevExpress.XtraGrid.Columns.GridColumn col2 = cboFilterTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_NAME");
            col2.VisibleIndex = 2;
            col2.Width = 200;
            col2.Caption = Resources.ResourceMessageLang.TatCa;
            cboFilterTreatmentType.Properties.PopupFormWidth = 250;
            cboFilterTreatmentType.Properties.View.OptionsView.ShowColumnHeaders = true;
            cboFilterTreatmentType.Properties.View.OptionsSelection.MultiSelect = true;

            GridCheckMarksSelection gridCheckMark = cboFilterTreatmentType.Properties.Tag as GridCheckMarksSelection;
            if (gridCheckMark != null)
            {
                gridCheckMark.ClearSelection(cboFilterTreatmentType.Properties.View);
            }
        }
        private void SelectionGrid__cboFilterTreatmentType(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_TREATMENT_TYPE> sgSelectedNews = new List<HIS_TREATMENT_TYPE>();
                    foreach (MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.TREATMENT_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.treatmentTypeSelecteds = new List<HIS_TREATMENT_TYPE>();
                    this.treatmentTypeSelecteds.AddRange(sgSelectedNews);
                }
                this.cboFilterTreatmentType.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        this.listTreatmentImport = import.GetWithCheck<TreatmentImportADO>(0);
                        if (this.listTreatmentImport != null && this.listTreatmentImport.Count > 0)
                        {
                            string error = "";
                            List<HisTreatmentView1ImportFilter.TreatmentImportFilter> processImport = ProcessDataImport(this.listTreatmentImport, ref error);
                            List<V_HIS_TREATMENT_1> listTreatment = new List<V_HIS_TREATMENT_1>();

                            if (!string.IsNullOrEmpty(error))
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show(error, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                return;
                            }
                            else if (processImport == null)
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessageLang.LoiKhiLayDuLieuLoc, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                return;
                            }
                            else
                            {
                                var skip = 0;
                                while (processImport.Count - skip >= 0)
                                {
                                    var imports = processImport.Skip(skip).Take(20).ToList();
                                    skip += 20;
                                    CommonParam param = new CommonParam();
                                    HisTreatmentView1ImportFilter filter = new HisTreatmentView1ImportFilter();
                                    filter.TreatmentImportFilters = imports;
                                    filter.ORDER_DIRECTION = "DESC";
                                    filter.ORDER_FIELD = "TREATMENT_CODE";

                                    var rsApi = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetByImportView1", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                                    if (rsApi != null)
                                    {
                                        listTreatment.AddRange(rsApi);
                                    }
                                }

                                if (listTreatment != null && listTreatment.Count > 0)//lọc lại danh sách
                                {
                                    listTreatment = listTreatment.GroupBy(o => o.ID).Select(s => s.First()).ToList();
                                }

                                if (listTreatment != null && listTreatment.Count > 0 && ucPaging1 != null && ucPaging1.pagingGrid != null)
                                {
                                    ucPaging1.pagingGrid.CurrentPage = 1;
                                    ucPaging1.pagingGrid.PageCount = 1;
                                    ucPaging1.pagingGrid.MaxRec = listTreatment.Count;
                                    ucPaging1.pagingGrid.DataCount = listTreatment.Count;
                                    ucPaging1.pagingGrid.LoadPage();
                                }

                                gridControlTreatment.BeginUpdate();
                                gridControlTreatment.DataSource = listTreatment;
                                gridControlTreatment.EndUpdate();

                                WaitingManager.Hide();
                            }
                        }
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HisTreatmentView1ImportFilter.TreatmentImportFilter> ProcessDataImport(List<TreatmentImportADO> treatmentImport, ref string error)
        {
            List<HisTreatmentView1ImportFilter.TreatmentImportFilter> result = new List<HisTreatmentView1ImportFilter.TreatmentImportFilter>();
            try
            {
                Inventec.Common.Logging.LogSystem.Info("begin time format");
                string cultureName = "en";
                string timeMax = "";
                if (treatmentImport.Exists(o => !string.IsNullOrEmpty(o.IN_TIME_STR)))
                {
                    var in_time = treatmentImport.Where(o => !string.IsNullOrEmpty(o.IN_TIME_STR)).ToList();
                    if (in_time != null && in_time.Count() > 0)
                    {
                        timeMax = in_time.OrderByDescending(o => o.IN_TIME_STR.Length).ThenByDescending(o => o.IN_TIME_STR).First().IN_TIME_STR;
                    }
                }
                else if (treatmentImport.Exists(o => !string.IsNullOrEmpty(o.OUT_TIME_STR)))
                {
                    var out_time = treatmentImport.Where(o => !string.IsNullOrEmpty(o.OUT_TIME_STR)).ToList();
                    if (out_time != null && out_time.Count() > 0)
                    {
                        timeMax = out_time.OrderByDescending(o => o.IN_TIME_STR.Length).ThenByDescending(o => o.IN_TIME_STR).First().OUT_TIME_STR;
                    }
                }

                if (!String.IsNullOrEmpty(timeMax))
                {
                    try
                    {
                        var dateTime = Convert.ToDateTime(timeMax);
                        if (dateTime != null)
                        {
                            cultureName = "vi";
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        cultureName = "en";
                    }
                }

                CustomProvider provider = new CustomProvider(cultureName);
                Inventec.Common.Logging.LogSystem.Info("end time format");
                foreach (var item in treatmentImport)
                {
                    if (item == null)
                        continue;

                    if (string.IsNullOrEmpty(item.IN_TIME_STR.Trim())
                        && string.IsNullOrEmpty(item.OUT_TIME_STR.Trim())
                        && string.IsNullOrEmpty(item.TDL_HEIN_CARD_NUMBER.Trim())
                        && string.IsNullOrEmpty(item.TDL_PATIENT_CODE.Trim())
                        && string.IsNullOrEmpty(item.TDL_PATIENT_NAME.Trim())
                        && string.IsNullOrEmpty(item.TREATMENT_CODE.Trim())) continue;

                    HisTreatmentView1ImportFilter.TreatmentImportFilter filter = new HisTreatmentView1ImportFilter.TreatmentImportFilter();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentView1ImportFilter.TreatmentImportFilter>(filter, item);

                    if (!string.IsNullOrEmpty(item.IN_TIME_STR))
                    {
                        try
                        {
                            var dateTime = Convert.ToDateTime(item.IN_TIME_STR, provider);
                            filter.IN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateTime);
                            item.IN_TIME = filter.IN_TIME;
                        }
                        catch (Exception)
                        {
                            error += string.Format("Ngày vào {0} không hợp lệ|", item.IN_TIME_STR);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.OUT_TIME_STR))
                    {
                        try
                        {
                            var dateTime = Convert.ToDateTime(item.OUT_TIME_STR, provider);
                            filter.OUT_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateTime);
                            item.OUT_TIME = filter.OUT_TIME;
                        }
                        catch (Exception)
                        {
                            error += string.Format("Ngày ra {0} không hợp lệ|", item.OUT_TIME_STR);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TDL_PATIENT_CODE))
                    {
                        if (item.TDL_PATIENT_CODE.Length < 10 && checkDigit(item.TDL_PATIENT_CODE))
                        {
                            filter.TDL_PATIENT_CODE = string.Format("{0:0000000000}", Convert.ToInt64(item.TDL_PATIENT_CODE));
                            item.TDL_PATIENT_CODE = string.Format("{0:0000000000}", Convert.ToInt64(item.TDL_PATIENT_CODE));
                        }
                        else
                        {
                            filter.TDL_PATIENT_CODE = item.TDL_PATIENT_CODE;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TREATMENT_CODE))
                    {
                        if (item.TREATMENT_CODE.Length < 12 && checkDigit(item.TREATMENT_CODE))
                        {
                            filter.TREATMENT_CODE = string.Format("{0:000000000000}", Convert.ToInt64(item.TREATMENT_CODE));
                            item.TREATMENT_CODE = string.Format("{0:000000000000}", Convert.ToInt64(item.TREATMENT_CODE));
                        }
                        else
                        {
                            filter.TREATMENT_CODE = item.TREATMENT_CODE;
                        }
                    }

                    result.Add(filter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            if (result.Count == 0)
                return null;
            return result;
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath
                + "/Tmp/Imp", "IMPORT_TREATMENT_XML.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_TREATMENT_XML";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName, true);
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessageLang.TaiFileVeMayTramThanhCong);
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessageLang.KhongTimThayFileImport);
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessageLang.TaiFileVeMayTramThatBai);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStatusFeeLockOrEndTreatment_Click(object sender, EventArgs e)
        {
            try
            {
                cboStatusFeeLockOrEndTreatment.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboFilterType_Click(object sender, EventArgs e)
        {
            try
            {
                cboFilterType.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        txtPatientCode.Focus();
                        txtPatientCode.SelectAll();
                    }
                    else
                    {
                        this.btnFind_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == btnSavePath.Name)
                        {
                            this.savePathADO = !String.IsNullOrWhiteSpace(item.VALUE) ? Newtonsoft.Json.JsonConvert.DeserializeObject<SavePathADO>(item.VALUE) : new SavePathADO();
                        }
                        else if (item.KEY == btnSettingConfigSync.Name)
                        {
                            configSync = !String.IsNullOrWhiteSpace(item.VALUE) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigSyncADO>(item.VALUE) : null;
                        }
                        else if (item.KEY == btnFind.Name)
                        {
                            this.searchFilter = !String.IsNullOrWhiteSpace(item.VALUE) ? Newtonsoft.Json.JsonConvert.DeserializeObject<SearchFilterADO>(item.VALUE) : new SearchFilterADO();
                        }
                        else if (item.KEY == chkSignFileCertUtil.Name)
                        {
                            SettingSignADO = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingSignADO>(item.VALUE);
                            chkSignFileCertUtil.Checked = SettingSignADO != null && !string.IsNullOrEmpty(SettingSignADO.SerialNumber);
                        }
                        else if (item.KEY == "chkXML3176")
                        {
                            chkXML3176.Checked = !String.IsNullOrWhiteSpace(item.VALUE) && Boolean.Parse(item.VALUE);

                            // GỌI HÀM MỚI:
                            UpdateBtnXML3176Visibility();
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                chkSignFileCertUtil.Checked = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                this.searchFilter.listPatientType = new List<HIS_PATIENT_TYPE>();
                foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    this.searchFilter.listPatientType.Add(rv);
                    sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboFilterTreatmentType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                this.searchFilter.listPTreattmentType = new List<HIS_TREATMENT_TYPE>();
                foreach (MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    this.searchFilter.listPTreattmentType.Add(rv);
                    sb.Append(rv.TREATMENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnLock.Enabled || this.currentTreatment == null)
                    return;
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                HisTreatmentLockHeinSDO sdo = new HisTreatmentLockHeinSDO();
                sdo.TreatmentId = this.currentTreatment.ID;
                if (dtHeinLockTime.EditValue != null)
                {
                    sdo.HeinLockTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHeinLockTime.DateTime);
                }
                else
                {
                    sdo.HeinLockTime = null;
                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>(HisRequestUriStore.HIS_TREATMENT_LOCK_HEIN, ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                    currentTreatment.IS_LOCK_HEIN = rs.IS_LOCK_HEIN;
                    currentTreatment.HEIN_LOCK_TIME = rs.HEIN_LOCK_TIME;
                    FillDataToGridTreatment();
                }
                WaitingManager.Hide();

                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UnlockHein", ApiConsumers.MosConsumer, currentTreatment.ID, param);

                    WaitingManager.Hide();
                    if (result != null)
                    {
                        success = true;
                        dtHeinLockTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0) ?? DateTime.MinValue;
                        currentTreatment.IS_LOCK_HEIN = null;
                        currentTreatment.HEIN_LOCK_TIME = null;
                        FillDataToGridTreatment();
                    }
                    WaitingManager.Hide();
                    #region Hien thi message thong bao
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                }
                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1)gridViewTreatment.GetFocusedRow();
                if (rowData != null)
                {
                    currentTreatment = rowData;
                    btnLock.Enabled = rowData.IS_LOCK_HEIN != 1 && rowData.IS_ACTIVE == 0;
                    btnUnlock.Enabled = rowData.IS_LOCK_HEIN == 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatment_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    short xml130Result = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewTreatment.GetRowCellValue(e.RowHandle, "XML130_RESULT") ?? "").ToString());
                    string xml130Desc = (gridViewTreatment.GetRowCellValue(e.RowHandle, "XML130_DESC") ?? "").ToString();
                    short xmlCheckinResult = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewTreatment.GetRowCellValue(e.RowHandle, "XML_CHECKIN_RESULT") ?? "").ToString());
                    string xmlCheckinDesc = (gridViewTreatment.GetRowCellValue(e.RowHandle, "XML_CHECKIN_DESC") ?? "").ToString();
                    string xmlCheckinUrl = (gridViewTreatment.GetRowCellValue(e.RowHandle, "XML_CHECKIN_URL") ?? "").ToString();
                    var data = (V_HIS_TREATMENT_1)gridViewTreatment.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "ErrorLine")
                    {
                        if (xml130Result == 1)
                        {
                            if (string.IsNullOrEmpty(xml130Desc))
                                e.RepositoryItem = Btn_Failed;
                            else
                                e.RepositoryItem = Btn_ErrorLine;
                        }
                        else if (xml130Result == 2)
                        {
                            if (data.XML130_CHECK_CODE != null)
                                e.RepositoryItem = Btn_Success;
                            else
                                e.RepositoryItem = Btn_SaveSuccess;
                        }
                    }
                    else if (e.Column.FieldName == "VIEW_XML_CHECKIN")
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(xmlCheckinUrl))
                            {
                                e.RepositoryItem = Btn_ViewXmlCheckinEnable;
                            }
                            else
                            {
                                e.RepositoryItem = Btn_ViewXmlCheckinDisable;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "XML_CHECKIN_RESULT_STR")
                    {
                        if (xmlCheckinResult == 2 || xmlCheckinResult == 4)
                        {
                            if (string.IsNullOrEmpty(xmlCheckinDesc))
                                e.RepositoryItem = Btn_Failed;
                            else
                                e.RepositoryItem = Btn_ErrorLine;
                        }
                        else if (xmlCheckinResult == 3) //thanh cong
                        {
                            e.RepositoryItem = Btn_Success;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAutoSync_Click(object sender, EventArgs e)
        {
            try
            {
                btnAutoSyncClick = true;
                Inventec.Common.Logging.LogSystem.Info("Auto sync - Checkbox XML3176: " + chkXML3176.Checked);
                if (configSync.isXML3176)
                {
                    isXML130 = false;

                    // SỬA: Chặn việc lưu cấu hình khi code tự động check
                    bool oldState = chkXML3176.Checked;

                    isNotLoadWhileChangeControlStateInFirst = true; // Khóa lưu
                    chkXML3176.Checked = true;
                    isNotLoadWhileChangeControlStateInFirst = false; // Mở lại
                }
                else
                {
                    isXML130 = true;
                }
                if (configSync == null)
                {
                    XtraMessageBox.Show(Resources.ResourceMessageLang.VuiLongThietLapDieuKienGuiHoSoTruocKhiThucHien, Resources.ResourceMessageLang.ThongBao);
                    ConfigSyncADO tempConfigSync = new ConfigSyncADO();
                    tempConfigSync.branchIds = this.branchSelecteds.Select(o => o.ID).ToList();
                    tempConfigSync.patientTypeIds = this.patientTypeSelecteds.Select(o => o.ID).ToList();
                    tempConfigSync.patientTypeTTIds = this.patientTypeTTSelecteds.Select(o => o.ID).ToList();
                    tempConfigSync.treatmentTypeIds = this.treatmentTypeSelecteds.Select(o => o.ID).ToList();
                    tempConfigSync.statusId = (int)cboStatus.EditValue;
                    tempConfigSync.period = 10;

                    tempConfigSync.isCheckOutTime = false;
                    tempConfigSync.isCheckCollinearXml = false;
                    tempConfigSync.isXML3176 = false;
                    frmSettingConfigSync frmSettingConfigSync = new frmSettingConfigSync(tempConfigSync, isAutoSync, UpdateConfigSign);
                    frmSettingConfigSync.ShowDialog(this.ParentForm);
                }
                if (chkSignFileCertUtil.Checked == false)
                {
                    if (!isAutoSync && this.configSync != null && this.configSync.period > 0)
                    {
                        isNotFileSign = true;
                        isAutoSync = true;
                        btnAutoSync.Text = Resources.ResourceMessageLang.DangDongBo;
                        btnAutoSync.ToolTip = Resources.ResourceMessageLang.DangChayTienTrinhDongBoDuLieuXml130LenCongBHYT;
                        this.StartTimer();
                    }
                    else
                    {
                        isAutoSync = false;
                        this.cancelAutoSyncRequested = true;   //yêu cầu dừng lô auto đang chạy dở
                        autoSync.Stop();
                        btnAutoSync.Text = Resources.ResourceMessageLang.DongBoTD;
                        btnAutoSync.ToolTip = Resources.ResourceMessageLang.DongBoTuDong;
                    }
                }
                else
                {
                    if (SettingSignADO == null || (SettingSignADO != null && string.IsNullOrEmpty(SettingSignADO.SerialNumber)))
                    {
                        MessageBox.Show("Không có thông tin HSM server/Usb Token ký số");
                        return;
                    }
                    else
                    {
                        //isNotFileSign = false;
                        //isAutoSync = true;
                        //btnAutoSync.Text = Resources.ResourceMessageLang.DangDongBo;
                        //btnAutoSync.ToolTip = Resources.ResourceMessageLang.DangChayTienTrinhDongBoDuLieuXml130LenCongBHYT;
                        //this.StartTimer();
                        if (!isAutoSync && this.configSync != null && this.configSync.period > 0)
                        {
                            isNotFileSign = false;
                            isAutoSync = true;
                            btnAutoSync.Text = Resources.ResourceMessageLang.DangDongBo;
                            btnAutoSync.ToolTip = Resources.ResourceMessageLang.DangChayTienTrinhDongBoDuLieuXml130LenCongBHYT;
                            this.StartTimer();
                        }
                        else
                        {
                            isAutoSync = false;
                            this.cancelAutoSyncRequested = true;   //yêu cầu dừng lô auto đang chạy dở
                            autoSync.Stop();
                            btnAutoSync.Text = Resources.ResourceMessageLang.DongBoTD;
                            btnAutoSync.ToolTip = Resources.ResourceMessageLang.DongBoTuDong;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void StartTimer()
        {
            try
            {
                this.cancelAutoSyncRequested = false;   //bắt đầu lại tiến trình auto -> xoá cờ hủy
                autoSync.Interval = (int)(configSync.period * 60000);
                autoSync.Enabled = true;
                this.autoSync_Tick(null, null);
                autoSync.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UpdateConfigSign(ConfigSyncADO config)
        {
            try
            {
                if (config != null)
                {
                    this.configSync = config;

                    string value = Newtonsoft.Json.JsonConvert.SerializeObject(configSync);
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == btnSettingConfigSync.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = value;
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = btnSettingConfigSync.Name;
                        csAddOrUpdate.VALUE = value;
                        csAddOrUpdate.MODULE_LINK = moduleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnSettingConfigSync_Click(object sender, EventArgs e)
        {
            try
            {
                if (configSync == null)
                {
                    ConfigSyncADO tempConfigSync = new ConfigSyncADO();
                    tempConfigSync.branchIds = this.branchSelecteds.Select(o => o.ID).ToList();
                    tempConfigSync.patientTypeIds = this.patientTypeSelecteds.Select(o => o.ID).ToList();
                    tempConfigSync.patientTypeTTIds = this.patientTypeTTSelecteds.Select(o => o.ID).ToList();
                    tempConfigSync.treatmentTypeIds = this.treatmentTypeSelecteds.Select(o => o.ID).ToList();
                    tempConfigSync.statusId = (int)cboStatus.EditValue;
                    tempConfigSync.period = 10;
                    tempConfigSync.isCheckOutTime = false;
                    tempConfigSync.isCheckCollinearXml = false;
                    tempConfigSync.isXML3176 = false;
                    frmSettingConfigSync frmSettingConfigSync = new frmSettingConfigSync(tempConfigSync, isAutoSync, UpdateConfigSign);
                    frmSettingConfigSync.ShowDialog(this.ParentForm);
                }
                else
                {
                    frmSettingConfigSync frmSettingConfigSync = new frmSettingConfigSync(configSync, isAutoSync, UpdateConfigSign);
                    frmSettingConfigSync.ShowDialog(this.ParentForm);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void autoSync_Tick(object sender, EventArgs e)
        {
            try
            {
                if (timerTickIsRunning)
                {
                    LogSystem.Info("Tien trinh tu dong dong bo dang chay. Khong cho phep khoi tao tien trinh khac");
                    return;
                }
                timerTickIsRunning = true;
                if (this.configSync.isCheckCollinearXml)
                {
                    listTreatmentSync = new List<V_HIS_TREATMENT_1>();
                    //lay ho so khoa bhyt
                    this.configSync.isCheckOutTime = true;
                    var listTreatmentLockBHYT = this.GetTreatment();
                    if (listTreatmentLockBHYT != null)
                        listTreatmentSync.AddRange(listTreatmentLockBHYT);
                    //lay ho so ket thuc dieu tri
                    this.configSync.isCheckOutTime = false;
                    var listTreatmentEnd = this.GetTreatment();
                    if (listTreatmentEnd != null)
                        listTreatmentSync.AddRange(listTreatmentEnd);
                }
                else
                {
                    listTreatmentSync = this.GetTreatment();
                }

                if (listTreatmentSync != null && listTreatmentSync.Count > 0)
                {
                    if (!backgroundWorker1.IsBusy)
                    {
                        LogSystem.Info("Thread Auto Sync. TreatmentCount: " + listTreatmentSync.Count);
                        backgroundWorker1.RunWorkerAsync();
                    }
                    else
                    {
                        LogSystem.Info("BackgroundWorker dang ban, bo qua " + listTreatmentSync.Count + " ho so. Se thu lai lan tick tiep theo.");
                    }
                }
                else
                {
                    LogSystem.Info("Khong co ho so dieu tri nao. Khong thuc hien tu dong dong bo");
                }
                LogSystem.Info("End Run Thread Auto Auto Sync");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            timerTickIsRunning = false;
        }
        private List<V_HIS_TREATMENT_1> GetTreatment()
        {
            List<V_HIS_TREATMENT_1> result = null;
            try
            {
                if (configSync != null)
                {
                    HisTreatmentView1Filter filter = new HisTreatmentView1Filter();
                    if (configSync.branchIds != null && configSync.branchIds.Count > 0)
                        filter.BRANCH_IDs = configSync.branchIds;
                    if (configSync.patientTypeIds != null && configSync.patientTypeIds.Count > 0)
                        filter.TDL_PATIENT_TYPE_IDs = configSync.patientTypeIds;
                    if (configSync.treatmentTypeIds != null && configSync.treatmentTypeIds.Count > 0)
                        filter.TDL_TREATMENT_TYPE_IDs = configSync.treatmentTypeIds;
                    if (configSync.statusId != null)
                    {
                        if (configSync.statusId == 1)
                        {
                            filter.IS_LOCK_HEIN = true;
                        }
                        else if (configSync.statusId == 2)
                        {
                            filter.IS_PAUSE = true;
                        }
                        else if (configSync.statusId == 3)
                        {
                            filter.HAS_IN_CODE = true;
                        }
                    }
                   
                    if (!configSync.isCheckOutTime)
                    {
                        filter.OUT_TIME_FROM = Convert.ToInt64(DateTime.Today.AddDays(-1).ToString("yyyyMMddHHmmss")); 
                        filter.OUT_TIME_TO = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        filter.IS_PAUSE = true;
                    }
                    else
                    {
                        filter.FEE_LOCK_TIME_FROM = Convert.ToInt64(DateTime.Today.AddDays(-1).ToString("yyyyMMddHHmmss"));
                        filter.FEE_LOCK_TIME_TO = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    }
                    if (configSync.isCheckCollinearXml)
                        filter.XML130_RESULT = null;
                    filter.HAS_XML130_RESULT = false;
                    LogSystem.Debug("Treatment Filter: " + LogUtil.TraceData("Filter", filter));
                    result = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumers.MosConsumer, filter, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        //LUỒNG 1: lấy hồ sơ để đẩy CSDL 4750 - lọc RỘNG chỉ theo chi nhánh + loại thời gian
        //(KHÔNG lọc đối tượng BN/TT, loại điều trị, trạng thái, KHÔNG gate theo XML130_RESULT).
        //Sau đó lọc client-side: bỏ hồ sơ đã đẩy 4750 thành công (CSDL4750_FINISH_RESULT == 3) -> tránh đẩy lặp, tự retry hồ sơ lỗi.
        private List<V_HIS_TREATMENT_1> GetTreatmentForKcb4750()
        {
            List<V_HIS_TREATMENT_1> result = null;
            try
            {
                if (configSync != null)
                {
                    HisTreatmentView1Filter filter = new HisTreatmentView1Filter();
                    if (configSync.branchIds != null && configSync.branchIds.Count > 0)
                        filter.BRANCH_IDs = configSync.branchIds;
                    //Lọc đối tượng thanh toán theo config MOS.CSDL_4750.PATIENT_TYPE_CODES (mã cách nhau bởi ','). Rỗng = lấy TẤT CẢ đối tượng.
                    List<long> kcbPatientTypeIds = GetKcb4750PatientTypeIds();
                    if (kcbPatientTypeIds != null && kcbPatientTypeIds.Count > 0)
                        filter.TDL_PATIENT_TYPE_IDs = kcbPatientTypeIds;
                    //Loại thời gian: giống Luồng 2 (ra viện vs khoá viện phí) nhưng KHÔNG áp bộ lọc loại điều trị/trạng thái.
                    if (!configSync.isCheckOutTime)
                    {
                        filter.OUT_TIME_FROM = Convert.ToInt64(DateTime.Today.AddDays(-1).ToString("yyyyMMddHHmmss"));
                        filter.OUT_TIME_TO = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        filter.IS_PAUSE = true;
                    }
                    else
                    {
                        filter.FEE_LOCK_TIME_FROM = Convert.ToInt64(DateTime.Today.AddDays(-1).ToString("yyyyMMddHHmmss"));
                        filter.FEE_LOCK_TIME_TO = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    }
                    LogSystem.Debug("KCB4750 Filter (Luong 1): " + LogUtil.TraceData("Filter", filter));
                    result = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumers.MosConsumer, filter, null);

                    //Chỉ giữ hồ sơ CHƯA đẩy 4750 thành công (finish result khác 3). Đọc cột qua reflection để không phụ thuộc phiên bản EFMODEL.
                    if (result != null && result.Count > 0)
                    {
                        int before = result.Count;
                        //Bỏ hồ sơ đã đẩy 4750 OK (=3) VÀ hồ sơ đang đẩy dở ở nền (in-flight) -> tránh đẩy trùng khi finish chưa kịp lưu.
                        result = result.Where(t => GetCsdl4750FinishResult(t) != 3 && !this.kcb4750InFlight.ContainsKey(t.ID)).ToList();
                        //Giới hạn số hồ sơ đẩy mỗi chu kỳ -> không chiếm luồng nền quá lâu, để Luồng 2 (BHYT) luôn có lượt chạy.
                        //Phần còn lại tự động xử lý ở các chu kỳ sau (cột CSDL4750_FINISH_RESULT loại hồ sơ đã đẩy OK).
                        int maxPerCycle = 200;
                        int cfgMax;
                        if (int.TryParse(HisConfigCFG.CSDL_4750__MAX_PER_CYCLE, out cfgMax) && cfgMax > 0)
                            maxPerCycle = cfgMax;
                        int afterFilter = result.Count;
                        if (result.Count > maxPerCycle)
                            result = result.Take(maxPerCycle).ToList();
                        LogSystem.Info(string.Format("Luong 1 KCB 4750: tong {0} ho so theo chi nhanh+thoi gian, {1} chua day OK, day {2} ho so trong chu ky nay (max {3}).", before, afterFilter, result.Count, maxPerCycle));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        //Chuyển danh sách mã đối tượng thanh toán (MOS.CSDL_4750.PATIENT_TYPE_CODES) sang ID để lọc luồng KCB 4750.
        //Rỗng/null -> trả null (lấy TẤT CẢ). Có mã nhưng không khớp danh mục nào -> null (lấy tất cả) + cảnh báo.
        private List<long> GetKcb4750PatientTypeIds()
        {
            try
            {
                string codes = HisConfigCFG.CSDL_4750__PATIENT_TYPE_CODES;
                if (string.IsNullOrWhiteSpace(codes)) return null;
                var codeSet = codes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(o => o.Trim().ToUpper())
                    .Where(o => !string.IsNullOrEmpty(o))
                    .Distinct()
                    .ToList();
                if (codeSet.Count == 0) return null;
                var allPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                if (allPatientType == null || allPatientType.Count == 0) return null;
                var ids = allPatientType
                    .Where(p => p.PATIENT_TYPE_CODE != null && codeSet.Contains(p.PATIENT_TYPE_CODE.Trim().ToUpper()))
                    .Select(p => p.ID)
                    .Distinct()
                    .ToList();
                if (ids.Count == 0)
                {
                    LogSystem.Warn("Luong 1 KCB 4750 - MOS.CSDL_4750.PATIENT_TYPE_CODES='" + codes + "' khong khop doi tuong nao trong danh muc -> lay tat ca.");
                    return null;
                }
                LogSystem.Info(string.Format("Luong 1 KCB 4750 - loc doi tuong theo MOS.CSDL_4750.PATIENT_TYPE_CODES='{0}' -> {1} ID.", codes, ids.Count));
                return ids;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }

        //Đọc cột CSDL4750_FINISH_RESULT trên V_HIS_TREATMENT_1 qua reflection.
        //An toàn nếu bản EFMODEL deploy chưa có cột -> trả null (coi như chưa đẩy) => degrade sang đẩy toàn bộ.
        private int? GetCsdl4750FinishResult(V_HIS_TREATMENT_1 t)
        {
            try
            {
                if (t == null) return null;
                var p = t.GetType().GetProperty("CSDL4750_FINISH_RESULT");
                if (p == null) return null;
                var v = p.GetValue(t, null);
                if (v == null) return null;
                return Convert.ToInt32(v);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gán property của InputADO bằng reflection: DLL His.Bhyt.ExportXml.XML130 trên máy build này
        /// CHƯA có một số property mới (vd TotalHeinPatientTypeData — bản mới hơn ở máy dev). DLL cũ thì
        /// bỏ qua (giữ hành vi cũ), DLL mới thì gán bình thường — build được với cả hai.
        /// </summary>
        private static void SetAdoPropIfExists(object ado, string propName, object value)
        {
            try
            {
                if (ado == null) return;
                var p = ado.GetType().GetProperty(propName);
                if (p != null && p.CanWrite) p.SetValue(ado, value, null);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private async void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //LUỒNG 2 (ưu tiên, xử lý TRƯỚC): gửi cổng BHYT như cũ - lọc đầy đủ theo form (chi nhánh, đối tượng BN/TT, loại điều trị, trạng thái, thời gian).
                List<Task> lst = new List<Task>();
                if (this.configSync.isXML3176 == true)
                {
                    isAutoSignXML3176 = true;
                    showMessSusscess = false;
                    isXML3176 = true;
                    lst.Add(XML130());
                }
                else
                {
                    lst.Add(ProcessSyncTreatment(listTreatmentSync));
                }
                Task.WaitAll(lst.ToArray());

                //LUỒNG 1: đẩy CSDL 4750. Dựng XML (dùng state chung -> nối tiếp Luồng 2) rồi ĐẨY 4750 fire-and-forget ở nền
                //-> KHÔNG bắt Luồng 2 (BHYT) chờ phần đẩy 4750 chậm. Backpressure: không dồn thêm nếu đang đẩy dở quá nhiều.
                int kcbInFlight = this.kcb4750InFlight.Count;
                LogSystem.Info(string.Format(
                    "[{0}] Luong 1 KCB 4750 - dispatch check: cancel={1}, configSyncNotNull={2}, isSyncKcb={3}, IS_AUTO_SYNC={4}, inFlight={5}",
                    KCB4750_BUILD_TAG,
                    this.cancelAutoSyncRequested,
                    this.configSync != null,
                    this.configSync != null && this.configSync.isSyncKcb,
                    HisConfigCFG.CSDL_4750__IS_AUTO_SYNC,
                    kcbInFlight));
                if (!this.cancelAutoSyncRequested
                    && this.configSync != null
                    && ((this.configSync.isSyncKcb && HisConfigCFG.CSDL_4750__IS_AUTO_SYNC == "1")
                        || (this.configSync.isSyncKcbVlg && !string.IsNullOrWhiteSpace(HisConfigCFG.VLG_2062__CONNECTION_INFO)))
                    && kcbInFlight < 1000)
                {
                    try
                    {
                        List<V_HIS_TREATMENT_1> listKcb = this.GetTreatmentForKcb4750();
                        if (listKcb != null && listKcb.Count > 0)
                        {
                            LogSystem.Info("Thread KCB 4750 (Luong 1). TreatmentCount: " + listKcb.Count);
                            this.ProcessSyncTreatment(listKcb, true).Wait();   //kcb4750Only=true -> chỉ dựng XML + bắn task đẩy 4750 (không chờ push)
                        }
                        else
                        {
                            LogSystem.Info("Luong 1 KCB 4750: khong co ho so can day (theo chi nhanh + loai thoi gian, chua day OK).");
                        }
                    }
                    catch (Exception exKcbFlow)
                    {
                        Inventec.Common.Logging.LogSystem.Error(exKcbFlow);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Xong");
                FillDataToGridTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                isXML130 = true;
                isXML3176 = false;
                await XML130();
                FillDataToGridTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task XML130()
        {
            try
            {
                if ((isAutoSync == true || btnAutoSyncClick == true) && configSync.isXML3176 == true) 
                {
                    LogSystem.Info("Check tự động ");
                    listSelection = this.GetTreatment();
                    LogSystem.Info("Giá trị tự động: " + listSelection.Count());
                }
                if ((listSelection == null || listSelection.Count == 0) && isAutoSignXML3176 == false)
                {
                    XtraMessageBox.Show(Resources.ResourceMessageLang.BanChuaChonHoSoDeDongBo, Resources.ResourceMessageLang.ThongBao);
                    return;
                }
                var listTreatmentSynced = listSelection.Where(o => o.XML130_RESULT == 2).ToList();
                if (listTreatmentSynced != null && listTreatmentSynced.Count > 0 && showMessSusscess == true)
                {
                    if (XtraMessageBox.Show(String.Format(Resources.ResourceMessageLang.CacHoSoDaDongBoThanhCongBanCoMuonDongBoLai, String.Join(", ", listTreatmentSynced.Select(o => o.TREATMENT_CODE).ToList())), Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                }

                var listTreatmentxml3176 = listSelection.Where(o => o.XML130_RESULT == 1).ToList();
                if (listTreatmentxml3176 != null && listTreatmentxml3176.Count > 0 && showMessSusscess == true && isAutoSignXML3176 == false)
                {
                    if (XtraMessageBox.Show(String.Format("Các hồ sơ {0} đã gửi thành công bạn có muốn gửi lại?", String.Join(", ", listTreatmentxml3176.Select(o => o.TREATMENT_CODE).ToList())), Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                }
                
                bool isCheckedSignSafe = false;
                if (chkSignFileCertUtil.InvokeRequired)
                {
                    chkSignFileCertUtil.Invoke(new MethodInvoker(delegate { isCheckedSignSafe = chkSignFileCertUtil.Checked; }));
                }
                else
                {
                    isCheckedSignSafe = chkSignFileCertUtil.Checked;
                }

                if (isCheckedSignSafe)
                {
                    if (SettingSignADO == null || (SettingSignADO != null && string.IsNullOrEmpty(SettingSignADO.SerialNumber)))
                    {
                        if (!isAutoSync)
                            MessageBox.Show("Không có thông tin Usb Token ký số");
                        else
                            LogSystem.Info("Khong co thong tin Usb Token ky so. Bo qua.");
                        return;
                    }
                    else
                    {
                        if (!isAutoSync) WaitingManager.Show();
                        callSyncSuccess = false;
                        isSendCollinearXml = false;
                        await ProcessSyncTreatment(listSelection);
                    }
                }
                else
                {
                    //qtcode
                    isNotFileSign = true;
                    //qtcode
                    if (!isAutoSync) WaitingManager.Show();
                    callSyncSuccess = false;
                    isSendCollinearXml = false;
                    await ProcessSyncTreatment(listSelection);
                }
                if (callSyncSuccess)
                {
                    if (listMessageError != null && listMessageError.Count > 0)
                    {
                        listMessageError = listMessageError.Distinct().ToList();
                        if (paramUpdateXml130.Messages != null && paramUpdateXml130.Messages.Count > 0)
                        {
                            listMessageError.AddRange(paramUpdateXml130.Messages);
                        }
                        LogSystem.Info("b1: " + listMessageError);
                        //XtraMessageBox.Show(Resources.ResourceMessageLang.XuLyThatBai + String.Join("\r\n", listMessageError), Resources.ResourceMessageLang.ThongBao);
                    }
                    else if (paramUpdateXml130.Messages != null && paramUpdateXml130.Messages.Count > 0)
                    {
                        MessageManager.Show(this.ParentForm, paramUpdateXml130, false);
                    }
                    else
                        MessageManager.Show(this.ParentForm, paramUpdateXml130, true);

                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void SendXml130Collinear()
        {
            try
            {
                if (listSelection == null || listSelection.Count == 0)
                {
                    XtraMessageBox.Show(Resources.ResourceMessageLang.BanChuaChonHoSoDeDongBo, Resources.ResourceMessageLang.ThongBao);
                    return;
                }

                isNotFileSign = false;
                //qtcode
                if (chkSignFileCertUtil.Checked == false)
                {
                    isNotFileSign = true;
                }
                //qtcode
                WaitingManager.Show();
                callSyncSuccess = false;
                isSendCollinearXml = true;
                if (chkSignFileCertUtil.Checked == true && (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber)))
                {
                    MessageBox.Show("Không có thông tin HSM server/Usb Token ký số");
                    return;
                }
                await ProcessSyncTreatment(listSelection);

                if (callSyncSuccess)
                {
                    if (listMessageError != null && listMessageError.Count > 0)
                    {
                        listMessageError = listMessageError.Distinct().ToList();
                        if (paramUpdateXml130.Messages != null && paramUpdateXml130.Messages.Count > 0)
                        {
                            listMessageError.AddRange(paramUpdateXml130.Messages);
                        }
                        LogSystem.Info("b2: " + listMessageError);
                        //XtraMessageBox.Show(Resources.ResourceMessageLang.XuLyThatBai + String.Join("\r\n", listMessageError), Resources.ResourceMessageLang.ThongBao);
                    }
                    else if (paramUpdateXml130.Messages != null && paramUpdateXml130.Messages.Count > 0)
                    {
                        MessageManager.Show(this.ParentForm, paramUpdateXml130, false);
                    }
                    else
                        MessageManager.Show(this.ParentForm, paramUpdateXml130, true);

                    FillDataToGridTreatment();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private async Task ProcessSyncTreatment(List<V_HIS_TREATMENT_1> listTreatmentSync, bool kcb4750Only = false)
        {
            try
            {
                listMessageError = new List<string>();
                string connect_infor = HisConfigCFG.QD_130_BYT__CONNECTION_INFO;
                string username = null, password = null, address = null, typeXml = null;
                string xml130Api = null, xmlGdykApi = null;
                List<string> connectInfors = new List<string>();
                //Chế độ gửi riêng KCB 4750 không cần cấu hình cổng BHYT (QD_130_BYT)
                if (!kcb4750Only)
                {
                if (string.IsNullOrEmpty(connect_infor))
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("01 - Lỗi cấu hình hệ thống");
                    return;
                }
                else
                {
                    connectInfors = connect_infor.Split('|').ToList();
                    if (connectInfors.Count < 3 || string.IsNullOrEmpty(connectInfors[0]) || string.IsNullOrEmpty(connectInfors[1]) || string.IsNullOrEmpty(connectInfors[2]))
                    {
                        WaitingManager.Hide();
                        XtraMessageBox.Show("01 - Lỗi cấu hình hệ thống");
                        return;
                    }
                }
                address = connectInfors[0];
                username = connectInfors[1];
                password = connectInfors[2];

                try
                {
                    typeXml = connectInfors[3];
                    xml130Api = connectInfors[4];
                    xmlGdykApi = connectInfors[5];
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error("Key cấu hình hệ thống chỉ thiết lập 3 giá trị");
                }
                }

                // Khởi tạo worker đồng bộ KCB lên CSDL dùng chung ngành Y tế theo QĐ 4750 (mục 3 + mục 6).
                // CHỈ đẩy 4750 ở luồng kcb4750Only (menu "Gửi Đồng bộ KCB" + Luồng 1 tự động).
                // Luồng gửi cổng BHYT (kcb4750Only=false) KHÔNG đẩy 4750 nữa -> tách bạch 2 luồng, tránh đẩy trùng.
                Csdl4750Worker kcb4750Worker = null;
                bool enableKcb4750 = HisConfigCFG.CSDL_4750__IS_AUTO_SYNC == "1" && kcb4750Only;
                //Log chẩn đoán: in ra từng biến quyết định để biết vì sao KCB 4750 bật/tắt (đặc biệt khi gửi tự động không sinh log Csdl4750Worker)
                LogSystem.Info(string.Format(
                    "[{6}] ProcessSyncTreatment - enableKcb4750={0}. IS_AUTO_SYNC={1}, kcb4750Only={2}, configSyncNotNull={3}, isSyncKcb={4}, isXML3176={5}",
                    enableKcb4750,
                    HisConfigCFG.CSDL_4750__IS_AUTO_SYNC,
                    kcb4750Only,
                    this.configSync != null,
                    this.configSync != null && this.configSync.isSyncKcb,
                    this.configSync != null && this.configSync.isXML3176,
                    KCB4750_BUILD_TAG));
                if (enableKcb4750)
                {
                    kcb4750Worker = new Csdl4750Worker(HisConfigCFG.CSDL_4750__CONNECTION_INFO);
                    if (!kcb4750Worker.IsValidConfig)
                    {
                        LogSystem.Warn("ProcessSyncTreatment - Bat dong bo KCB 4750 nhung cau hinh HIS.CSDL_4750.CONNECTION_INFO khong hop le. Bo qua dong bo 4750.");
                        kcb4750Worker = null;
                    }
                }
                //Cổng tiếp nhận KDLYT Vĩnh Long (hoan-tat): cùng luồng kcb4750Only, gate = khóa VLG + tích chọn ở Cài đặt.
                VlgKcbHoanTatWorker vlgKcbWorker = null;
                bool enableKcbVlg = kcb4750Only
                    && this.configSync != null && this.configSync.isSyncKcbVlg
                    && !string.IsNullOrWhiteSpace(HisConfigCFG.VLG_2062__CONNECTION_INFO);
                if (enableKcbVlg)
                {
                    vlgKcbWorker = new VlgKcbHoanTatWorker(HisConfigCFG.VLG_2062__CONNECTION_INFO);
                    if (!vlgKcbWorker.IsValidConfig)
                    {
                        LogSystem.Warn("ProcessSyncTreatment - Bat dong bo KCB VLG nhung khoa MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO khong hop le. Bo qua.");
                        vlgKcbWorker = null;
                    }
                }
                //Danh sách trạng thái finish để gửi lên api/HisTreatment/UpdateCsdl4750FinishInfo (gộp cả lô).
                //Khi CHỈ đẩy VLG (không có 4750): finish lấy theo kết quả VLG để chu kỳ tự động không chọn lại hồ sơ đã đẩy.
                List<HisTreatmentCsdl4750FinishSDO> kcb4750FinishList = (kcb4750Worker != null || vlgKcbWorker != null) ? new List<HisTreatmentCsdl4750FinishSDO>() : null;
                //Danh sách dòng kết quả để thông báo rõ từng hồ sơ ra màn hình (dùng cho gửi thủ công qua menu).
                //BẮT BUỘC dùng biến LOCAL trong các Task nền: field this.kcb4750ResultLines bị gán lại mỗi lượt chạy,
                //task fire-and-forget của lượt TRƯỚC còn chạy dở sẽ Add vào list của lượt SAU dưới lock khác -> hỏng list.
                List<string> kcbResultLines = new List<string>();
                this.kcb4750ResultLines = kcbResultLines;
                //Các Task đẩy chạy nền (song song với gửi cổng BHYT). Chờ hoàn tất trước khi lưu trạng thái finish.
                List<Task> kcb4750Tasks = (kcb4750Worker != null || vlgKcbWorker != null) ? new List<Task>() : null;
                //Khoá đồng bộ khi các Task 4750 ghi vào list dùng chung (finish + result lines).
                object kcb4750Lock = new object();
                //Lượt chạy tự động (cho phép hủy giữa chừng khi tắt Đồng bộ tự động) - áp dụng cả Luồng 1 (KCB) lẫn Luồng 2 (BHYT).
                //Menu/gửi tay (isAutoSync=false) không bị ảnh hưởng.
                //Lượt gửi tay qua menu KHÔNG phải lượt tự động dù chế độ auto đang bật — không bị hủy giữa chừng khi tắt auto.
                bool thisRunIsAuto = this.isAutoSync && !this.manualSyncKcb4750;

                Dictionary<string, List<string>> DicErrorMess = new Dictionary<string, List<string>>();
                if (listTreatmentSync != null && listTreatmentSync.Count > 0)
                {
                    listTreatmentSync = listTreatmentSync.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();

                    this.NewConfig = GetNewConfig();
                    int skip = 0;
                    while (listTreatmentSync.Count - skip > 0)
                    {
                        //Hủy giữa chừng khi: tắt Đồng bộ tự động, HOẶC (lượt KCB tự động) người dùng đã bỏ tích "Đồng bộ KCB".
                        if (thisRunIsAuto && (this.cancelAutoSyncRequested
                            || (kcb4750Only && this.configSync != null && !this.configSync.isSyncKcb && !this.configSync.isSyncKcbVlg)))
                        {
                            LogSystem.Info("ProcessSyncTreatment - Dung xu ly (tat Dong bo tu dong hoac bo tich Dong bo KCB) -> bo cac lo con lai.");
                            break;
                        }
                        var limit = listTreatmentSync.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;
                        #region
                        ListPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
                        ListSereServ = new List<V_HIS_SERE_SERV_2>();
                        ListEkipUser = new List<HIS_EKIP_USER>();
                        ListBedlog = new List<V_HIS_BED_LOG>();
                        HisTreatments = new List<V_HIS_TREATMENT_12>();
                        ListDhst = new List<HIS_DHST>();
                        HisTrackings = new List<HIS_TRACKING>();
                        HisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                        HisSereServSuin = new List<V_HIS_SERE_SERV_SUIN>();
                        HisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();
                        ListDebates = new List<HIS_DEBATE>();
                        ListBaby = new List<V_HIS_BABY>();
                        ListMedicalAssessment = new List<V_HIS_MEDICAL_ASSESSMENT>();
                        ListHivTreatment = new List<HIS_HIV_TREATMENT>();
                        ListTuberculosisTreat = new List<HIS_TUBERCULOSIS_TREAT>();
                        ListExpMedimateUsed = new List<HIS_EXP_MEDIMATE_USED>();
                        CreateThreadGetData(limit);
                        Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>> dicPatientTypeAlter = new Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>>();
                        Dictionary<long, List<V_HIS_SERE_SERV_2>> dicSereServ = new Dictionary<long, List<V_HIS_SERE_SERV_2>>();
                        Dictionary<long, List<V_HIS_SERE_SERV_TEIN>> dicSereServTein = new Dictionary<long, List<V_HIS_SERE_SERV_TEIN>>();
                        Dictionary<long, List<V_HIS_SERE_SERV_SUIN>> dicSereServSuin = new Dictionary<long, List<V_HIS_SERE_SERV_SUIN>>();
                        Dictionary<long, List<V_HIS_SERE_SERV_PTTT>> dicSereServPttt = new Dictionary<long, List<V_HIS_SERE_SERV_PTTT>>();
                        Dictionary<long, List<V_HIS_BED_LOG>> dicBedLog = new Dictionary<long, List<V_HIS_BED_LOG>>();
                        Dictionary<long, List<HIS_TRACKING>> dicTracking = new Dictionary<long, List<HIS_TRACKING>>();
                        Dictionary<long, List<HIS_EKIP_USER>> dicEkipUser = new Dictionary<long, List<HIS_EKIP_USER>>();
                        Dictionary<long, List<V_HIS_BABY>> dicBaby = new Dictionary<long, List<V_HIS_BABY>>();
                        Dictionary<long, List<HIS_DEBATE>> dicDebate = new Dictionary<long, List<HIS_DEBATE>>();
                        Dictionary<long, List<HIS_DHST>> dicDhstList = new Dictionary<long, List<HIS_DHST>>();
                        Dictionary<long, List<V_HIS_MEDICAL_ASSESSMENT>> dicMedicalAssessment = new Dictionary<long, List<V_HIS_MEDICAL_ASSESSMENT>>();
                        Dictionary<long, HIS_HIV_TREATMENT> dicHivTreatment = new Dictionary<long, HIS_HIV_TREATMENT>();
                        Dictionary<long, HIS_TUBERCULOSIS_TREAT> dicTuberculosisTreat = new Dictionary<long, HIS_TUBERCULOSIS_TREAT>();
                        Dictionary<long, List<HIS_EXP_MEDIMATE_USED>> dicExpUsedByExpMestMedicineId = new Dictionary<long, List<HIS_EXP_MEDIMATE_USED>>();
                        Dictionary<long, List<HIS_EXP_MEDIMATE_USED>> dicExpUsedByExpMestMaterialId = new Dictionary<long, List<HIS_EXP_MEDIMATE_USED>>();

                        if (ListExpMedimateUsed != null && ListExpMedimateUsed.Count > 0)
                        {
                            foreach (var u in ListExpMedimateUsed)
                            {
                                if (u == null) continue;

                                if (u.EXP_MEST_MEDICINE_ID.HasValue)
                                {
                                    var k = u.EXP_MEST_MEDICINE_ID.Value;
                                    if (!dicExpUsedByExpMestMedicineId.ContainsKey(k))
                                        dicExpUsedByExpMestMedicineId[k] = new List<HIS_EXP_MEDIMATE_USED>();
                                    dicExpUsedByExpMestMedicineId[k].Add(u);
                                }

                                if (u.EXP_MEST_MATERIAL_ID.HasValue)
                                {
                                    var k = u.EXP_MEST_MATERIAL_ID.Value;
                                    if (!dicExpUsedByExpMestMaterialId.ContainsKey(k))
                                        dicExpUsedByExpMestMaterialId[k] = new List<HIS_EXP_MEDIMATE_USED>();
                                    dicExpUsedByExpMestMaterialId[k].Add(u);
                                }
                            }
                        }

                        if (ListTuberculosisTreat != null && ListTuberculosisTreat.Count > 0)
                        {
                            foreach (var item in ListTuberculosisTreat)
                            {
                                if (!dicTuberculosisTreat.ContainsKey(item.TREATMENT_ID))
                                    dicTuberculosisTreat[item.TREATMENT_ID] = new HIS_TUBERCULOSIS_TREAT();
                                dicTuberculosisTreat[item.TREATMENT_ID] = item;
                            }
                        }
                        if (ListPatientTypeAlter != null && ListPatientTypeAlter.Count > 0)
                        {
                            foreach (var item in ListPatientTypeAlter)
                            {
                                if (!dicPatientTypeAlter.ContainsKey(item.TREATMENT_ID))
                                    dicPatientTypeAlter[item.TREATMENT_ID] = new List<V_HIS_PATIENT_TYPE_ALTER>();
                                dicPatientTypeAlter[item.TREATMENT_ID].Add(item);
                            }
                        }

                        if (ListSereServ != null && ListSereServ.Count > 0)
                        {
                            foreach (var sereServ in ListSereServ)
                            {
                                if (sereServ.AMOUNT > 0 && sereServ.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.TDL_TREATMENT_ID.HasValue && ((sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.PRICE > 0) || sereServ.IS_NO_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE))
                                {
                                    if (!dicSereServ.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                        dicSereServ[sereServ.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_2>();
                                    dicSereServ[sereServ.TDL_TREATMENT_ID.Value].Add(sereServ);
                                }

                                if (sereServ.EKIP_ID.HasValue && ListEkipUser != null && ListEkipUser.Count > 0 && sereServ.TDL_TREATMENT_ID.HasValue)
                                {
                                    var ekips = ListEkipUser.Where(o => o.EKIP_ID == sereServ.EKIP_ID).ToList();
                                    if (ekips != null && ekips.Count > 0)
                                    {
                                        foreach (var item in ekips)
                                        {
                                            if (!dicEkipUser.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                                dicEkipUser[sereServ.TDL_TREATMENT_ID.Value] = new List<HIS_EKIP_USER>();

                                            dicEkipUser[sereServ.TDL_TREATMENT_ID.Value].Add(item);
                                        }
                                    }
                                }
                            }
                        }

                        if (HisSereServTeins != null && HisSereServTeins.Count > 0)
                        {
                            foreach (var ssTein in HisSereServTeins)
                            {
                                if (!ssTein.TDL_TREATMENT_ID.HasValue) continue;

                                if (!dicSereServTein.ContainsKey(ssTein.TDL_TREATMENT_ID.Value))
                                    dicSereServTein[ssTein.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_TEIN>();

                                dicSereServTein[ssTein.TDL_TREATMENT_ID.Value].Add(ssTein);
                            }
                        }
                        if (HisSereServSuin != null && HisSereServSuin.Count > 0)
                        {
                            foreach (var ssSuin in HisSereServSuin)
                            {

                                if (!dicSereServSuin.ContainsKey(ssSuin.TDL_TREATMENT_ID))
                                    dicSereServSuin[ssSuin.TDL_TREATMENT_ID] = new List<V_HIS_SERE_SERV_SUIN>();

                                dicSereServSuin[ssSuin.TDL_TREATMENT_ID].Add(ssSuin);
                            }
                        }
                        if (HisTrackings != null && HisTrackings.Count > 0)
                        {
                            foreach (var tracking in HisTrackings)
                            {
                                if (!dicTracking.ContainsKey(tracking.TREATMENT_ID))
                                    dicTracking[tracking.TREATMENT_ID] = new List<HIS_TRACKING>();

                                dicTracking[tracking.TREATMENT_ID].Add(tracking);
                            }
                        }
                        if (ListBaby != null && ListBaby.Count > 0)
                        {
                            foreach (var baby in ListBaby)
                            {
                                if (!dicBaby.ContainsKey(baby.TREATMENT_ID))
                                    dicBaby[baby.TREATMENT_ID] = new List<V_HIS_BABY>();

                                dicBaby[baby.TREATMENT_ID].Add(baby);
                            }
                        }
                        if (ListHivTreatment != null && ListHivTreatment.Count > 0)
                        {
                            ListHivTreatment = ListHivTreatment.OrderBy(o => o.ID).ToList();
                            foreach (var hivTreatment in ListHivTreatment)
                            {
                                dicHivTreatment[hivTreatment.TREATMENT_ID] = hivTreatment;
                            }
                        }
                        if (HisSereServPttts != null && HisSereServPttts.Count > 0)
                        {
                            foreach (var ssPttt in HisSereServPttts)
                            {
                                if (!ssPttt.TDL_TREATMENT_ID.HasValue) continue;

                                if (!dicSereServPttt.ContainsKey(ssPttt.TDL_TREATMENT_ID.Value))
                                    dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_PTTT>();

                                dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value].Add(ssPttt);
                            }
                        }

                        if (ListDhst != null && ListDhst.Count > 0)
                        {
                            foreach (var item in ListDhst)
                            {
                                if (!dicDhstList.ContainsKey(item.TREATMENT_ID))
                                    dicDhstList[item.TREATMENT_ID] = new List<HIS_DHST>();

                                dicDhstList[item.TREATMENT_ID].Add(item);
                            }
                        }

                        if (ListBedlog != null && ListBedlog.Count > 0)
                        {
                            foreach (var bed in ListBedlog)
                            {
                                if (!dicBedLog.ContainsKey(bed.TREATMENT_ID))
                                    dicBedLog[bed.TREATMENT_ID] = new List<V_HIS_BED_LOG>();

                                dicBedLog[bed.TREATMENT_ID].Add(bed);
                            }
                        }

                        if (ListDebates != null && ListDebates.Count > 0)
                        {
                            foreach (var item in ListDebates)
                            {
                                if (!dicDebate.ContainsKey(item.TREATMENT_ID))
                                    dicDebate[item.TREATMENT_ID] = new List<HIS_DEBATE>();

                                dicDebate[item.TREATMENT_ID].Add(item);
                            }
                        }
                        if (ListMedicalAssessment != null && ListMedicalAssessment.Count > 0)
                        {
                            foreach (var item in ListMedicalAssessment)
                            {
                                if (!dicMedicalAssessment.ContainsKey(item.TREATMENT_ID))
                                    dicMedicalAssessment[item.TREATMENT_ID] = new List<V_HIS_MEDICAL_ASSESSMENT>();

                                dicMedicalAssessment[item.TREATMENT_ID].Add(item);
                            }
                        }
                        #endregion
                        foreach (var treatment in HisTreatments)
                        {
                            //Hủy giữa chừng khi: tắt Đồng bộ tự động, HOẶC (lượt KCB tự động) đã bỏ tích "Đồng bộ KCB" (dừng ở ranh giới hồ sơ).
                            if (thisRunIsAuto && (this.cancelAutoSyncRequested
                                || (kcb4750Only && this.configSync != null && !this.configSync.isSyncKcb && !this.configSync.isSyncKcbVlg)))
                            {
                                LogSystem.Info("ProcessSyncTreatment - Dung xu ly (tat Dong bo tu dong hoac bo tich Dong bo KCB) -> bo cac ho so con lai.");
                                break;
                            }

                            paramUpdateXml130 = new CommonParam();
                            #region
                            bool sendXml12 = true;
                            InputADO ado = new InputADO();
                            ado.Treatment = treatment;
                            if (dicPatientTypeAlter.ContainsKey(treatment.ID))
                            {
                                ado.ListPatientTypeAlter = dicPatientTypeAlter[treatment.ID];
                            }

                            if (!dicSereServ.ContainsKey(treatment.ID))
                            {
                                continue;
                            }

                            ado.ListSereServ = dicSereServ.ContainsKey(treatment.ID) ? dicSereServ[treatment.ID] : null;

                            if (dicDhstList.ContainsKey(treatment.ID))
                            {
                                ado.ListDhst = dicDhstList[treatment.ID];
                            }

                            if (dicSereServTein.ContainsKey(treatment.ID))
                            {
                                ado.ListSereServTein = dicSereServTein[treatment.ID];
                            }
                            if (dicSereServSuin.ContainsKey(treatment.ID))
                            {
                                ado.vSereServSuin = dicSereServSuin[treatment.ID];
                            }
                            if (dicSereServPttt.ContainsKey(treatment.ID))
                            {
                                ado.ListSereServPttt = dicSereServPttt[treatment.ID];
                            }

                            if (dicBedLog.ContainsKey(treatment.ID))
                            {
                                ado.ListBedLog = dicBedLog[treatment.ID];
                            }

                            if (dicTracking.ContainsKey(treatment.ID))
                            {
                                ado.ListTracking = dicTracking[treatment.ID];
                            }

                            if (dicEkipUser.ContainsKey(treatment.ID))
                            {
                                ado.ListEkipUser = dicEkipUser[treatment.ID].Distinct().ToList();
                            }

                            if (dicDebate.ContainsKey(treatment.ID))
                            {
                                ado.ListDebate = dicDebate[treatment.ID];
                            }

                            if (dicBaby.ContainsKey(treatment.ID))
                            {
                                ado.ListBaby = dicBaby[treatment.ID];
                            }
                            if (dicMedicalAssessment.ContainsKey(treatment.ID))
                            {
                                ado.ListMedicalAssessment = dicMedicalAssessment[treatment.ID];
                            }
                            else
                                sendXml12 = false;
                            sendXml12 = !string.IsNullOrEmpty(typeXml) ? typeXml.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Contains("12") && ado.ListMedicalAssessment != null && ado.ListMedicalAssessment.Count > 0 : false;
                            if (dicHivTreatment.ContainsKey(treatment.ID))
                            {
                                ado.HivTreatment = dicHivTreatment[treatment.ID];
                            }
                            ado.TotalMaterialTypeData = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                            ado.TotalHeinMediOrgData = BackendDataWorker.Get<HIS_MEDI_ORG>();
                            //DLL cu chua co property -> reflection (xem chu thich o SetAdoPropIfExists).
                            SetAdoPropIfExists(ado, "TotalHeinPatientTypeData", BackendDataWorker.Get<HIS_HEIN_PATIENT_TYPE>());
                            ado.TotalConfigData = NewConfig;
                            ado.TotalPatientTypeData = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                            ado.TotalIcdData = BackendDataWorker.Get<HIS_ICD>();
                            ado.TotalSericeData = BackendDataWorker.Get<V_HIS_SERVICE>();
                            ado.TotalEmployeeData = BackendDataWorker.Get<HIS_EMPLOYEE>();
                            var usedList = new List<HIS_EXP_MEDIMATE_USED>();

                            if (ado.ListSereServ != null && ado.ListSereServ.Count > 0)
                            {
                                foreach (var ss in ado.ListSereServ)
                                {
                                    if (ss.EXP_MEST_MEDICINE_ID.HasValue)
                                    {
                                        var k = ss.EXP_MEST_MEDICINE_ID.Value;
                                        if (dicExpUsedByExpMestMedicineId.TryGetValue(k, out var lst))
                                            usedList.AddRange(lst);
                                    }

                                    if (ss.EXP_MEST_MATERIAL_ID.HasValue)
                                    {
                                        var k = ss.EXP_MEST_MATERIAL_ID.Value;
                                        if (dicExpUsedByExpMestMaterialId.TryGetValue(k, out var lst))
                                            usedList.AddRange(lst);
                                    }
                                }
                            }

                            ado.ListExpMedimateUsed = usedList
                                .GroupBy(x => x.ID)
                                .Select(g => g.First())
                                .ToList();

                            if (HisConfigCFG.QD_130_BVT_XML1_MA_KHOA_OPTION == "1")
                            {
                                ado.ListDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
                            }
                            ado.serverInfo = new ServerInfo() { Username = username, Password = password, Address = address, TypeXml = typeXml, Xml130Api = xml130Api, XmlGdykApi = xmlGdykApi };
                            //ado.delegateSignXml = DataSignXML;

                            if (dicTuberculosisTreat.ContainsKey(treatment.ID))
                            {
                                ado.TuberculosisTreat = dicTuberculosisTreat[treatment.ID];
                            }
                            // --- SỬA LẠI ĐỂ TRÁNH LỖI CROSS-THREAD KHI CHẠY TỰ ĐỘNG ---
                            bool isCheckedSafe = false;
                            if (chkXML3176.InvokeRequired)
                            {
                                chkXML3176.Invoke(new MethodInvoker(delegate { isCheckedSafe = chkXML3176.Checked; }));
                            }
                            else
                            {
                                isCheckedSafe = chkXML3176.Checked;
                            }

                            if (isCheckedSafe)
                            {
                                ado.IS_3176 = true;
                                Inventec.Common.Logging.LogSystem.Debug("ProcessSyncTreatment - Checkbox tích → IS_3176 = true (XML 3176)");
                            }
                            else
                            {
                                ado.IS_3176 = false;
                                Inventec.Common.Logging.LogSystem.Debug("ProcessSyncTreatment - Checkbox không tích → IS_3176 = false (XML 130)");
                            }
                            // ----------------------------------------------------------
                            #endregion
                            His.Bhyt.ExportXml.XML130.CreateXmlProcessor xmlProcessor = new His.Bhyt.ExportXml.XML130.CreateXmlProcessor(ado);
                            SyncResultADO syncResult = null;
                            SyncResultADO syncResult12 = null;
                            MemoryStream resultSync = null;
                            MemoryStream resultSync12 = null;
                            MemoryStream resultSyncTT = null;
                            string saveFilePathXml12 = "";
                            string saveFilePathXml = "";
                            string saveFilePathXmlTT = "";
                            string errorMess = "";
                            int count = 0;
                            Inventec.Common.Logging.LogSystem.Debug("Dang xu ly gui  : " + treatment.TDL_PATIENT_NAME + " Ma dieu tri: " + treatment.TREATMENT_CODE);

                            if (kcb4750Only)
                            {
                                //Chỉ tạo XML QĐ 4750 để đẩy lên CSDL 4750, không gửi cổng BHYT, không cập nhật trạng thái XML130.
                                resultSync = xmlProcessor.Run(ref errorMess);
                            }
                            else if (configSync != null && !this.configSync.dontSend)
                            {
                                //Thư mục ghi file XML: dùng folderPath nếu có cấu hình; nếu rỗng thì ghi vào Temp
                                //(tránh ghi vào gốc ổ C:\ gây UnauthorizedAccessException và đảm bảo ký số/đẩy 4750 luôn có file nguồn).
                                string xmlSaveDir = !string.IsNullOrEmpty(this.configSync.folderPath)
                                    ? this.configSync.folderPath
                                    : System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Temp");
                                try { Directory.CreateDirectory(xmlSaveDir); }
                                catch (Exception exDir) { Inventec.Common.Logging.LogSystem.Warn("Khong tao duoc thu muc luu XML: " + xmlSaveDir + ". " + exDir.Message); }

                                if (sendXml12)
                                {
                                    string fullFileName = xmlProcessor.GetFileName();

                                    if ((isAutoSync && configSync != null && configSync.isCheckCollinearXml) || (isSendCollinearXml))
                                    {
                                        resultSyncTT = treatment.IS_LOCK_FEE == 1 ? xmlProcessor.RunCollinearXml(ref errorMess) : null;
                                        Task task = null;
                                        List<Task> lstTask = new List<Task>();
                                        if (resultSyncTT != null)
                                        {
                                            saveFilePathXmlTT = String.Format("{0}/{1}{2}", xmlSaveDir, "XMLTT_", fullFileName);
                                            FileStream file12 = new FileStream(saveFilePathXmlTT, FileMode.Create, FileAccess.Write);
                                            resultSyncTT.WriteTo(file12);
                                            file12.Close();
                                            resultSyncTT.Close();
                                            Inventec.Common.Logging.LogSystem.Debug("__Luu XMlTT vao client folder thanh cong. path: " + saveFilePathXmlTT);
                                        }
                                        if (isNotFileSign == false)
                                        {
                                            //Ký file XML THÔNG TUYẾN (saveFilePathXmlTT) - trước đây truyền nhầm saveFilePathXml (rỗng) gây lỗi "File nguồn để ký số không tồn tại".
                                            if (resultSyncTT != null && !string.IsNullOrEmpty(saveFilePathXmlTT) && File.Exists(saveFilePathXmlTT))
                                            {
                                                sendXMLSign(xmlProcessor, saveFilePathXmlTT, ref syncResult);
                                            }
                                            else
                                            {
                                                Inventec.Common.Logging.LogSystem.Warn("Bo qua ky so XML thong tuyen: khong co file de ky (IS_LOCK_FEE != 1 hoac chua sinh XML). Ma dieu tri: " + treatment.TREATMENT_CODE);
                                            }
                                        }
                                        else
                                        {
                                            task = Task.Run(async () => syncResult = await xmlProcessor.SyncDataCollinear());
                                            lstTask.Add(task);
                                        }
                                        Task taskXml12 = Task.Run(async () => syncResult12 = await xmlProcessor.SyncDataXml12());
                                        lstTask.Add(taskXml12);
                                        resultSync12 = xmlProcessor.RunXml12(ref errorMess);
                                        Task.WaitAll(lstTask.ToArray());
                                    }
                                    else
                                    {
                                        resultSync = xmlProcessor.Run(ref errorMess);
                                        Task task = null;
                                        List<Task> lstTask = new List<Task>();
                                        if (resultSync != null)
                                        {
                                            saveFilePathXml = String.Format("{0}/{1}{2}", xmlSaveDir, "XML", fullFileName);
                                            FileStream file12 = new FileStream(saveFilePathXml, FileMode.Create, FileAccess.Write);
                                            resultSync.WriteTo(file12);
                                            file12.Close();
                                            resultSync.Close();
                                            Inventec.Common.Logging.LogSystem.Debug("__Luu XMl vao client folder thanh cong. path: " + saveFilePathXml);
                                        }
                                        if (isNotFileSign == false)
                                        {
                                            if (!string.IsNullOrEmpty(saveFilePathXml) && File.Exists(saveFilePathXml))
                                                sendXMLSign(xmlProcessor, saveFilePathXml, ref syncResult);
                                            else
                                                Inventec.Common.Logging.LogSystem.Warn("Bo qua ky so: khong co file XML de ky. Ma dieu tri: " + treatment.TREATMENT_CODE);
                                        }
                                        else
                                        {
                                            task = Task.Run(async () => syncResult = await xmlProcessor.SyncData());
                                            lstTask.Add(task);
                                        }
                                        Task taskXml12 = Task.Run(async () => syncResult12 = await xmlProcessor.SyncDataXml12());
                                        lstTask.Add(taskXml12);
                                        resultSync12 = xmlProcessor.RunXml12(ref errorMess);
                                        Task.WaitAll(lstTask.ToArray());
                                    }


                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("syncResult__" + Inventec.Common.Logging.LogUtil.GetMemberName(() => syncResult), syncResult));
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("syncResult12__" + Inventec.Common.Logging.LogUtil.GetMemberName(() => syncResult12), syncResult12));



                                    if (syncResult != null && syncResult12 != null)
                                    {
                                        string errorCode = syncResult.ErrorCode;
                                        if ((errorCode == "01" || errorCode == "02" || errorCode == "03") && !isAutoSync)
                                        {
                                            XtraMessageBox.Show(String.Format("{0} - {1}", errorCode, syncResult.Message), Resources.ResourceMessageLang.ThongBao);
                                            return;
                                        }
                                        else
                                        {
                                            callSyncSuccess = true;
                                            if (!syncResult.Success)
                                            {
                                                listMessageError.Add(String.Format("{0}: {1} - {2}", treatment.TREATMENT_CODE, syncResult.ErrorCode, syncResult.Message));
                                            }
                                            if (!syncResult12.Success)
                                            {
                                                listMessageError.Add(String.Format("{0}: {1} - {2}", treatment.TREATMENT_CODE, syncResult12.ErrorCode, syncResult12.Message));
                                            }
                                            if (!((isAutoSync && configSync != null && configSync.isCheckCollinearXml) || isSendCollinearXml))
                                            {

                                                List<string> xmlDescription = new List<string> { syncResult.Message, syncResult12.Message };
                                                List<string> xmlCheckCode = new List<string> { syncResult.CheckCode, syncResult12.CheckCode };
                                                HisTreatmentXmlResultSDO xmlResultSDO = new HisTreatmentXmlResultSDO();
                                                xmlResultSDO.TreatmentId = treatment.ID;
                                                xmlResultSDO.XmlResult = syncResult.Success && syncResult12.Success ? 2 : 1;
                                                xmlResultSDO.Description = String.Join(". ", xmlDescription.Where(o => !String.IsNullOrEmpty(o)).Distinct());
                                                xmlResultSDO.CheckCode = String.Join(";", xmlCheckCode.Where(o => !String.IsNullOrEmpty(o)).Distinct());
                                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => xmlResultSDO), xmlResultSDO));
                                                var rs = new Inventec.Common.Adapter.BackendAdapter(paramUpdateXml130).Post<bool>("api/HisTreatment/UpdateXml130Info", ApiConsumers.MosConsumer, xmlResultSDO, paramUpdateXml130);
                                                //luu file
                                                if (configSync != null && !string.IsNullOrEmpty(configSync.folderPath))
                                                {
                                                    if (resultSync12 != null)
                                                    {
                                                        saveFilePathXml12 = String.Format("{0}/{1}{2}", this.configSync.folderPath, "XML12_", fullFileName);
                                                        FileStream file12 = new FileStream(saveFilePathXml12, FileMode.Create, FileAccess.Write);
                                                        resultSync12.WriteTo(file12);
                                                        file12.Close();
                                                        resultSync12.Close();
                                                        Inventec.Common.Logging.LogSystem.Debug("__Luu XMl12 vao client folder thanh cong. path: " + saveFilePathXml12);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                else
                                {

                                    if ((isAutoSync && configSync != null && configSync.isCheckCollinearXml) || (isSendCollinearXml))
                                    {
                                        resultSync =  xmlProcessor.RunCollinearXml(ref errorMess);
                                    }
                                    else
                                    {
                                        resultSync = (treatment.HEIN_LOCK_TIME != null) ? xmlProcessor.Run(ref errorMess) : null;
                                    }
                                    //luu file
                                    if (resultSync != null)
                                    {
                                        string fullFileName = xmlProcessor.GetFileName();
                                        saveFilePathXml = String.Format("{0}/{1}{2}", xmlSaveDir, "XML", fullFileName);
                                        FileStream file12 = new FileStream(saveFilePathXml, FileMode.Create, FileAccess.Write);
                                        resultSync.WriteTo(file12);
                                        file12.Close();
                                        resultSync.Close();
                                        Inventec.Common.Logging.LogSystem.Debug("__Luu XMl vao client folder thanh cong. path: " + saveFilePathXml);

                                    }
                                    if (isNotFileSign == false)
                                    {
                                        if (!string.IsNullOrEmpty(saveFilePathXml) && File.Exists(saveFilePathXml))
                                            sendXMLSign(xmlProcessor, saveFilePathXml, ref syncResult);
                                        else
                                            Inventec.Common.Logging.LogSystem.Warn("Bo qua ky so: khong co file XML de ky. Ma dieu tri: " + treatment.TREATMENT_CODE);
                                    }
                                    else
                                    {
                                        if ((isAutoSync && configSync != null && configSync.isCheckCollinearXml) || (isSendCollinearXml))
                                        {
                                            syncResult = await xmlProcessor.SyncDataCollinear();
                                        }
                                        else
                                        {
                                            syncResult = (treatment.HEIN_LOCK_TIME != null) ? await xmlProcessor.SyncData() : null;
                                        }
                                    }

                                    //if ((isAutoSync && configSync != null && configSync.isCheckCollinearXml) || (isSendCollinearXml))
                                    //    syncResult = await xmlProcessor.SyncDataCollinear();
                                    //else
                                    //    syncResult = await xmlProcessor.SyncData();
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("syncResult__" + Inventec.Common.Logging.LogUtil.GetMemberName(() => syncResult), syncResult));
                                    if (syncResult != null)
                                    {
                                        if (!syncResult.Success && !isAutoSync)
                                        {
                                            XtraMessageBox.Show("Ký số thất bại: " + syncResult.Message, Resources.ResourceMessageLang.ThongBao);
                                            return;
                                        }
                                        string errorCode = syncResult.ErrorCode;
                                        if ((errorCode == "01" || errorCode == "02" || errorCode == "03") && !isAutoSync)
                                        {
                                            XtraMessageBox.Show(String.Format("{0} - {1}", errorCode, syncResult.Message), Resources.ResourceMessageLang.ThongBao);
                                            return;
                                        }
                                        else
                                        {
                                            if (errorCode == "07" && isAutoSync)
                                            {
                                                LogSystem.Info("Error 07 - bo qua ho so: " + treatment.TREATMENT_CODE);
                                                continue;
                                            }

                                            callSyncSuccess = true;
                                            if (!syncResult.Success)
                                            {
                                                listMessageError.Add(String.Format("{0}: {1} - {2}", treatment.TREATMENT_CODE, syncResult.ErrorCode, syncResult.Message));
                                            }


                                            HisTreatmentXmlResultSDO xmlResultSDO = new HisTreatmentXmlResultSDO();
                                            xmlResultSDO.TreatmentId = treatment.ID;
                                            xmlResultSDO.XmlResult = syncResult.Success ? 2 : 1;
                                            xmlResultSDO.Description = syncResult.Message;
                                            xmlResultSDO.CheckCode = syncResult.CheckCode;
                                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => xmlResultSDO), xmlResultSDO));
                                            var rs = new Inventec.Common.Adapter.BackendAdapter(paramUpdateXml130).Post<bool>("api/HisTreatment/UpdateXml130Info", ApiConsumers.MosConsumer, xmlResultSDO, paramUpdateXml130);
                                            Inventec.Common.Logging.LogSystem.Debug("Update thanh cong  : " + rs + " du lieu: " + treatment.TDL_PATIENT_NAME + " Ma dieu tri: " + treatment.TREATMENT_CODE);


                                        }
                                    }
                                }
                            }
                            else
                            {
                                string errMessage = "";
                                bool success = false;
                                bool signSuccess = true;

                                try
                                {
                                    if (treatment.FEE_LOCK_TIME != null)
                                    {
                                        resultSync = xmlProcessor.Run(ref errMessage);
                                    }
                                    if ((isAutoSync && configSync != null && configSync.isCheckCollinearXml) || (isSendCollinearXml))
                                        resultSync = xmlProcessor.RunCollinearXml(ref errMessage);
                                    if (string.IsNullOrEmpty(errMessage))
                                    {
                                        success = true;
                                    }
                                }
                                catch (Exception error)
                                {
                                    success = false;
                                    errMessage = error.Message;
                                }

                                if (resultSync != null)
                                {
                                    if (this.configSync != null && !string.IsNullOrEmpty(this.configSync.folderPath))
                                    {
                                        string fullFileName = xmlProcessor.GetFileName();
                                        saveFilePathXml = string.Format("{0}/{1}{2}", this.configSync.folderPath, "XML",fullFileName);

                                        using (FileStream fs = new FileStream(saveFilePathXml, FileMode.Create, FileAccess.Write))
                                        {
                                            resultSync.WriteTo(fs);
                                        }
                                        resultSync.Close();

                                        Inventec.Common.Logging.LogSystem.Debug("__Luu XML vao client folder thanh cong. Path: " + saveFilePathXml);

                                        if (!isNotFileSign)
                                        {
                                            signSuccess = sendXMLSign(xmlProcessor, saveFilePathXml, ref syncResult);

                                            if (!signSuccess)
                                            {
                                                if (File.Exists(saveFilePathXml))
                                                {
                                                    try
                                                    {
                                                        File.Delete(saveFilePathXml);
                                                        Inventec.Common.Logging.LogSystem.Warn("Ky so that bai -> da xoa file XML: " + saveFilePathXml);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Inventec.Common.Logging.LogSystem.Error("Khong xoa duoc file XML khi ky so loi: " + ex);
                                                    }
                                                }
                                                return;
                                            }
                                        }
                                    }
                                    HisTreatmentXmlResultSDO xmlResultSDO = new HisTreatmentXmlResultSDO();
                                    xmlResultSDO.TreatmentId = treatment.ID;
                                    xmlResultSDO.XmlResult = success ? 2 : 1;
                                    xmlResultSDO.Description = errMessage;

                                    Inventec.Common.Logging.LogSystem.Debug(
                                        Inventec.Common.Logging.LogUtil.TraceData(
                                            Inventec.Common.Logging.LogUtil.GetMemberName(() => xmlResultSDO),
                                            xmlResultSDO
                                        )
                                    );

                                    var rs = new Inventec.Common.Adapter.BackendAdapter(paramUpdateXml130).Post<bool>("api/HisTreatment/UpdateXml130Info", ApiConsumers.MosConsumer, xmlResultSDO, paramUpdateXml130);
                                }
                            }

                            #region Đồng bộ Khám chữa bệnh lên CSDL 4750 (mục 6) + Cổng tiếp nhận VLG (hoan-tat) - đẩy nền song song với gửi cổng BHYT
                            if (kcb4750Worker != null || vlgKcbWorker != null)
                            {
                                //Lấy bytes XML NGAY (đồng bộ) rồi ĐẨY 4750 ở Task nền -> chạy song song với việc gửi cổng BHYT
                                //của các hồ sơ kế tiếp. Lỗi đẩy 4750 được cô lập trong Task, không ảnh hưởng luồng gửi 130 và ngược lại.
                                byte[] kcbXmlBytes = null;
                                try
                                {
                                    // Ưu tiên lấy đúng file XML đã tạo trên đĩa (nếu có lưu thư mục), nếu không thì lấy từ luồng vừa sinh
                                    if (!string.IsNullOrEmpty(saveFilePathXml) && File.Exists(saveFilePathXml))
                                    {
                                        kcbXmlBytes = File.ReadAllBytes(saveFilePathXml);
                                    }
                                    else
                                    {
                                        MemoryStream kcbXmlStream = resultSyncTT != null ? resultSyncTT : resultSync;
                                        if (kcbXmlStream != null)
                                        {
                                            kcbXmlBytes = kcbXmlStream.ToArray();
                                        }
                                    }
                                }
                                catch (Exception exSnap)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(exSnap);
                                }

                                //Chụp lại thông tin hồ sơ để Task nền dùng an toàn (không phụ thuộc biến vòng lặp)
                                long kcbTreatmentId = treatment.ID;
                                string kcbTreatmentCode = treatment.TREATMENT_CODE;
                                string kcbFileUrl = (!string.IsNullOrEmpty(saveFilePathXml) && File.Exists(saveFilePathXml)) ? saveFilePathXml : null;

                                if (kcbXmlBytes != null && kcbXmlBytes.Length > 0)
                                {
                                    byte[] kcbBytesLocal = kcbXmlBytes;
                                    this.kcb4750InFlight.TryAdd(kcbTreatmentId, 0);   //đánh dấu đang đẩy -> chu kỳ sau không chọn lại
                                    Task kcbTask = Task.Run(async () =>
                                    {
                                        try
                                        {
                                            //1) CSDL 4750 (nếu bật) — trạng thái finish lưu theo kết quả 4750 như cũ.
                                            if (kcb4750Worker != null)
                                            {
                                            Csdl4750ImportResult kcbResult = await kcb4750Worker.ImportXmlAsync(kcbBytesLocal, kcbTreatmentCode);
                                            bool kcbSuccess = kcbResult != null && kcbResult.Success;
                                            string kcbMessage = kcbResult != null ? kcbResult.Message : "Không có phản hồi từ API";
                                            lock (kcb4750Lock)
                                            {
                                                if (kcb4750FinishList != null)
                                                {
                                                    kcb4750FinishList.Add(new HisTreatmentCsdl4750FinishSDO()
                                                    {
                                                        TreatmentId = kcbTreatmentId,
                                                        FinishResult = kcbSuccess ? 3 : 4,   //3=gửi thành công, 4=gửi thất bại
                                                        Description = kcbMessage,
                                                        FinishUrl = kcbFileUrl
                                                    });
                                                }
                                                kcbResultLines.Add(string.Format("{0}: {1}{2}",
                                                    kcbTreatmentCode,
                                                    kcbSuccess ? "Thành công" : "Thất bại",
                                                    string.IsNullOrEmpty(kcbMessage) ? "" : " - " + kcbMessage));
                                            }
                                            }
                                            //2) Cổng tiếp nhận KDLYT Vĩnh Long (nếu bật) — lỗi VLG cô lập riêng, không ảnh hưởng 4750.
                                            //Khi KHÔNG có 4750: finish lưu theo kết quả VLG để chu kỳ tự động không chọn lại hồ sơ.
                                            if (vlgKcbWorker != null)
                                            {
                                                try
                                                {
                                                    Csdl4750ImportResult vlgResult = await vlgKcbWorker.ImportXmlAsync(kcbBytesLocal, kcbTreatmentCode);
                                                    bool vlgSuccess = vlgResult != null && vlgResult.Success;
                                                    string vlgMessage = vlgResult != null ? vlgResult.Message : "VLG: không có phản hồi từ cổng";
                                                    lock (kcb4750Lock)
                                                    {
                                                        if (kcb4750Worker == null && kcb4750FinishList != null)
                                                        {
                                                            kcb4750FinishList.Add(new HisTreatmentCsdl4750FinishSDO()
                                                            {
                                                                TreatmentId = kcbTreatmentId,
                                                                FinishResult = vlgSuccess ? 3 : 4,
                                                                Description = vlgMessage,
                                                                FinishUrl = kcbFileUrl
                                                            });
                                                        }
                                                        kcbResultLines.Add(string.Format("{0}: {1}{2}",
                                                            kcbTreatmentCode,
                                                            vlgSuccess ? "Thành công" : "Thất bại",
                                                            string.IsNullOrEmpty(vlgMessage) ? "" : " - " + vlgMessage));
                                                    }
                                                }
                                                catch (Exception exVlg)
                                                {
                                                    Inventec.Common.Logging.LogSystem.Error(exVlg);
                                                    lock (kcb4750Lock)
                                                    {
                                                        kcbResultLines.Add(kcbTreatmentCode + ": Thất bại - VLG: " + exVlg.Message);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception exKcb)
                                        {
                                            Inventec.Common.Logging.LogSystem.Error(exKcb);
                                            lock (kcb4750Lock)
                                            {
                                                if (kcb4750FinishList != null)
                                                {
                                                    kcb4750FinishList.Add(new HisTreatmentCsdl4750FinishSDO()
                                                    {
                                                        TreatmentId = kcbTreatmentId,
                                                        FinishResult = 4,
                                                        Description = "Lỗi khi gửi: " + exKcb.Message,
                                                        FinishUrl = null
                                                    });
                                                }
                                                kcbResultLines.Add(kcbTreatmentCode + ": Thất bại - " + exKcb.Message);
                                            }
                                        }
                                        finally
                                        {
                                            byte _b; this.kcb4750InFlight.TryRemove(kcbTreatmentId, out _b);   //đẩy xong -> bỏ đánh dấu
                                        }
                                    });
                                    if (kcb4750Tasks != null) kcb4750Tasks.Add(kcbTask);
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Info("Dong bo KCB 4750 - Khong co file XML de gui. Ma dieu tri: " + kcbTreatmentCode);
                                    lock (kcb4750Lock)
                                    {
                                        if (kcb4750FinishList != null)
                                        {
                                            kcb4750FinishList.Add(new HisTreatmentCsdl4750FinishSDO()
                                            {
                                                TreatmentId = kcbTreatmentId,
                                                FinishResult = 2,   //2=lỗi tạo/gửi (không có file XML để gửi)
                                                Description = "Không tạo được file XML để gửi",
                                                FinishUrl = null
                                            });
                                        }
                                        kcbResultLines.Add(kcbTreatmentCode + ": Thất bại - Không tạo được file XML để gửi");
                                    }
                                }
                            }
                            #endregion

                            count++;
                        }
                    }
                }

                //Xử lý các Task đẩy 4750:
                if (kcb4750Tasks != null && kcb4750Tasks.Count > 0)
                {
                    List<Task> kcbTasksLocal = kcb4750Tasks;
                    List<HisTreatmentCsdl4750FinishSDO> kcbFinishLocal = kcb4750FinishList;
                    //isAutoSync là cờ CHẾ ĐỘ (bật suốt phiên) — lượt GỬI TAY qua menu khi auto đang bật
                    //vẫn phải CHỜ (nhánh else) để dialog tổng kết đọc đủ kết quả, không fire-and-forget.
                    if (kcb4750Only && this.isAutoSync && !this.manualSyncKcb4750)
                    {
                        //LUỒNG 1 TỰ ĐỘNG: FIRE-AND-FORGET -> trả luồng NGAY, không chờ đẩy 4750.
                        //Các push 4750 (chậm/504) drain ở nền; lưu finish khi push xong. Nhờ vậy Luồng 2 (BHYT) chạy song song, KHÔNG chờ nhau.
                        Task.Run(async () =>
                        {
                            try
                            {
                                await Task.WhenAll(kcbTasksLocal);
                                PostCsdl4750Finish(kcbFinishLocal);
                            }
                            catch (Exception exBg)
                            {
                                Inventec.Common.Logging.LogSystem.Error(exBg);
                            }
                        });
                    }
                    else
                    {
                        //MENU/gửi tay: chờ hoàn tất để hiển thị tổng kết cho người dùng.
                        try { Task.WhenAll(kcbTasksLocal).Wait(); }
                        catch (Exception exWait) { Inventec.Common.Logging.LogSystem.Error(exWait); }
                        PostCsdl4750Finish(kcbFinishLocal);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Lưu trạng thái đồng bộ liên thông CSDL 4750 (finish) cho cả lô - dùng chung cho gửi tự động & gửi menu.
        private void PostCsdl4750Finish(List<HisTreatmentCsdl4750FinishSDO> finishList)
        {
            try
            {
                if (finishList == null || finishList.Count == 0) return;
                CommonParam paramCsdl4750 = new CommonParam();
                bool rsFinish = new Inventec.Common.Adapter.BackendAdapter(paramCsdl4750)
                    .Post<bool>("api/HisTreatment/UpdateCsdl4750FinishInfo", ApiConsumers.MosConsumer, finishList, paramCsdl4750);
                Inventec.Common.Logging.LogSystem.Info("ProcessSyncTreatment - Luu trang thai CSDL 4750 finish: " + rsFinish + ". So ho so: " + finishList.Count);
            }
            catch (Exception exFinish)
            {
                Inventec.Common.Logging.LogSystem.Error(exFinish);
            }
        }
        #region
        //private void sendXMLSign(His.Bhyt.ExportXml.XML130.CreateXmlProcessor xmlProcessor, string sourceFile, ref SyncResultADO syncResult)
        //{
        //    try
        //    {
        //        if (SettingSignADO == null)
        //        {
        //            Inventec.Common.Logging.LogSystem.Error("Không có thông tin cài đặt ký số sendXMLSign");
        //            return;
        //        }
        //        string currentDirectory = Directory.GetCurrentDirectory();
        //        string tempFolderPath = Path.Combine(currentDirectory, "Temp");
        //        Directory.CreateDirectory(tempFolderPath);
        //        string fullFileName = xmlProcessor.GetFileName();
        //        string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
        //        File.Create(tempFilePath).Close();
        //        string pathAfterFileSign = null;
        //        WcfSignDCO wcfSignDCO = null;
        //        if (SettingSignADO.IsHsm)
        //        {
        //            var xmlBase64 = SourceFileSignApi(ReadFileContent(sourceFile));
        //            if (!string.IsNullOrEmpty(xmlBase64))
        //            {
        //                try
        //                {
        //                    var xmlBytes = Convert.FromBase64String(xmlBase64);
        //                    File.WriteAllBytes(tempFilePath, xmlBytes);
        //                    pathAfterFileSign = tempFilePath;
        //                }
        //                catch (Exception ex)
        //                {
        //                    Inventec.Common.Logging.LogSystem.Error("Error saving xmlBase64 to file: " + ex);
        //                }
        //            }
        //            else
        //            {
        //                if (File.Exists(sourceFile))
        //                {
        //                    File.Delete(sourceFile);
        //                }
        //                if (!isAutoSync)
        //                {
        //                    //XtraMessageBox.Show("Ký số thất bại. Không tạo file XML.", "Thông báo");
        //                    return;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            wcfSignDCO = new WcfSignDCO();
        //            wcfSignDCO.SerialNumber = SettingSignADO.SerialNumber;
        //            wcfSignDCO.OutputFile = tempFilePath;
        //            wcfSignDCO.PIN = "";
        //            wcfSignDCO.SourceFile = sourceFile;
        //            wcfSignDCO.fieldSigned = "CHUKYDONVI";
        //            string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
        //            SignProcessorClient signProcessorClient = new SignProcessorClient();
        //            pathAfterFileSign = sourceFile;
        //            if (VerifyServiceSignProcessorIsRunning())
        //            {
        //                var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
        //                if (wcfSignResultDCO != null && wcfSignResultDCO.Success)
        //                {
        //                    pathAfterFileSign = wcfSignResultDCO.OutputFile;
        //                }
        //                else
        //                {
        //                    //XtraMessageBox.Show("Ký số thất bại. Không tạo file XML.", "Thông báo");
        //                    return;
        //                }
        //            }
        //        }
        //        if (configSync != null && !this.configSync.dontSend)
        //        {
        //            //gọi api đẩy cổng ...
        //            //...
        //            SyncResultADO syncResultADO = new SyncResultADO();
        //            Task task = Task.Run(async () => syncResultADO = await xmlProcessor.SendFileSign(pathAfterFileSign));
        //            task.Wait();
        //            syncResult = syncResultADO;
        //            if (syncResult != null && !syncResult.Success)
        //            {
        //                if (File.Exists(sourceFile))
        //                {
        //                    File.Delete(sourceFile);
        //                }
        //                if (!isAutoSync)
        //                {
        //                    XtraMessageBox.Show("Ký số thất bại: " + syncResult.Message, Resources.ResourceMessageLang.ThongBao);
        //                    return;
        //                }
        //            }
        //        }
        //        if (this.configSync != null && !string.IsNullOrEmpty(this.configSync.folderPath))
        //        {
        //            if (wcfSignDCO != null)
        //            {
        //                if (wcfSignDCO.SourceFile.Trim() != pathAfterFileSign.Trim())
        //                {
        //                    if (File.Exists(wcfSignDCO.SourceFile))
        //                    {
        //                        File.Delete(wcfSignDCO.SourceFile);
        //                    }
        //                }
        //                File.Copy(pathAfterFileSign, wcfSignDCO.SourceFile);
        //            }
        //            else if (SettingSignADO.IsHsm)
        //            {

        //                if (sourceFile != pathAfterFileSign.Trim())
        //                {
        //                    if (File.Exists(sourceFile))
        //                    {
        //                        File.Delete(sourceFile);
        //                    }
        //                }
        //                File.Copy(pathAfterFileSign, sourceFile);
        //            }
        //        }

        //        foreach (string file in Directory.GetFiles(tempFolderPath))
        //        {
        //            File.Delete(file);
        //        }
        //        if (configSync != null && !this.configSync.dontSend && string.IsNullOrEmpty(this.configSync.folderPath))
        //        {
        //            if (wcfSignDCO != null && File.Exists(wcfSignDCO.SourceFile))
        //            {
        //                File.Delete(wcfSignDCO.SourceFile);
        //            }
        //            else if (SettingSignADO.IsHsm)
        //            {
        //                File.Delete(sourceFile);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        #endregion
        private bool sendXMLSign(His.Bhyt.ExportXml.XML130.CreateXmlProcessor xmlProcessor, string sourceFile, ref SyncResultADO syncResult)
        {
            try
            {
                if (SettingSignADO == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(
                        "Không có thông tin cài đặt ký số sendXMLSign"
                    );
                    return false;
                }
                if (string.IsNullOrEmpty(sourceFile) || !File.Exists(sourceFile))
                {
                    Inventec.Common.Logging.LogSystem.Error(
                        "File nguồn để ký số không tồn tại: " + sourceFile
                    );
                    return false;
                }

                string currentDirectory = Directory.GetCurrentDirectory();
                string tempFolderPath = Path.Combine(currentDirectory, "Temp");
                Directory.CreateDirectory(tempFolderPath);

                string fullFileName = xmlProcessor.GetFileName();
                string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                File.Create(tempFilePath).Close();

                string pathAfterFileSign = null;
                WcfSignDCO wcfSignDCO = null;

                // ====== KÝ FILE ======
                if (SettingSignADO.IsHsm)
                {
                    var xmlBase64 = SourceFileSignApi(ReadFileContent(sourceFile));
                    if (string.IsNullOrEmpty(xmlBase64))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Ký HSM thất bại");
                        return false;
                    }

                    var xmlBytes = Convert.FromBase64String(xmlBase64);
                    File.WriteAllBytes(tempFilePath, xmlBytes);
                    pathAfterFileSign = tempFilePath;
                }
                else
                {
                    wcfSignDCO = new WcfSignDCO
                    {
                        SerialNumber = SettingSignADO.SerialNumber,
                        OutputFile = tempFilePath,
                        PIN = "",
                        SourceFile = sourceFile,
                        fieldSigned = "CHUKYDONVI"
                    };

                    string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                    SignProcessorClient signProcessorClient = new SignProcessorClient();

                    if (!VerifyServiceSignProcessorIsRunning())
                    {
                        return false;
                    }

                    var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                    if (wcfSignResultDCO == null || !wcfSignResultDCO.Success)
                    {
                        return false;
                    }

                    pathAfterFileSign = wcfSignResultDCO.OutputFile;
                }

                // ====== GỬI FILE ======
                if (configSync != null && !configSync.dontSend)
                {
                    SyncResultADO syncResultADO = null;
                    Task task = Task.Run(async () =>
                        syncResultADO = await xmlProcessor.SendFileSign(pathAfterFileSign)
                    );
                    task.Wait();

                    syncResult = syncResultADO;

                    if (syncResult == null || !syncResult.Success)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(
                            "Gửi file ký số thất bại: " + syncResult?.Message
                        );
                        return false;
                    }
                }

                // ====== COPY FILE VỀ THƯ MỤC ======
                if (configSync != null && !string.IsNullOrEmpty(configSync.folderPath))
                {
                    if (wcfSignDCO != null)
                    {
                        File.Copy(pathAfterFileSign, wcfSignDCO.SourceFile, true);
                    }
                    else if (SettingSignADO.IsHsm)
                    {
                        File.Copy(pathAfterFileSign, sourceFile, true);
                    }
                }

                // ====== CLEAN TEMP ======
                foreach (string file in Directory.GetFiles(tempFolderPath))
                {
                    File.Delete(file);
                }

                return true; // 🎯 QUAN TRỌNG
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        // Build danh sach HSTH01BH_CHITIET tu danh sach treatments — dung chung cho ca XML va Excel.
        // sttStart: gia tri STT bat dau (cho phep noi tiep cross-chunk khi xuat Excel).
        // dicErrorMess: gom loi cross-call (caller cong them message).
        internal List<HSTH01BH_CHITIET> BuildHsth01bhChiTietList(
            List<V_HIS_TREATMENT_12> hisTreatments,
            List<V_HIS_PATIENT_TYPE_ALTER> hisPatientTypeAlters,
            List<V_HIS_SERE_SERV_2> listSereServ,
            List<V_HIS_SERE_SERV_PTTT> hisSereServPttts,
            int sttStart,
            Dictionary<string, List<string>> dicErrorMess)
        {
            var result = new List<HSTH01BH_CHITIET>();
            if (hisTreatments == null || hisTreatments.Count == 0) return result;
            if (dicErrorMess == null) dicErrorMess = new Dictionary<string, List<string>>();

            Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>> dicPatientTypeAlter = new Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>>();
            Dictionary<long, List<V_HIS_SERE_SERV_2>> dicSereServ = new Dictionary<long, List<V_HIS_SERE_SERV_2>>();
            Dictionary<long, List<V_HIS_SERE_SERV_PTTT>> dicSereServPttt = new Dictionary<long, List<V_HIS_SERE_SERV_PTTT>>();

            if (hisPatientTypeAlters != null)
            {
                foreach (var item in hisPatientTypeAlters)
                {
                    if (!dicPatientTypeAlter.ContainsKey(item.TREATMENT_ID)) dicPatientTypeAlter[item.TREATMENT_ID] = new List<V_HIS_PATIENT_TYPE_ALTER>();
                    dicPatientTypeAlter[item.TREATMENT_ID].Add(item);
                }
            }

            if (listSereServ != null)
            {
                foreach (var sereServ in listSereServ)
                {
                    if (sereServ.TDL_TREATMENT_ID.HasValue)
                    {
                        if (!dicSereServ.ContainsKey(sereServ.TDL_TREATMENT_ID.Value)) dicSereServ[sereServ.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_2>();
                        dicSereServ[sereServ.TDL_TREATMENT_ID.Value].Add(sereServ);
                    }
                }
            }

            if (hisSereServPttts != null)
            {
                foreach (var ssPttt in hisSereServPttts)
                {
                    if (ssPttt.TDL_TREATMENT_ID.HasValue)
                    {
                        if (!dicSereServPttt.ContainsKey(ssPttt.TDL_TREATMENT_ID.Value)) dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_PTTT>();
                        dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value].Add(ssPttt);
                    }
                }
            }

            List<HIS_PATIENT_TYPE> hisPatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
            string thoiGianQtOption = His.Bhyt.ExportXml.XMLTT12.XML01BH.HisConfigKeys.GetConfigData(this.NewConfig, His.Bhyt.ExportXml.XMLTT12.XML01BH.HisConfigKeys.THOI_GIAN_QT_OPTION);

            // Index HIS_EXP_MEDIMATE_USED theo EXP_MEST_MEDICINE_ID / EXP_MEST_MATERIAL_ID để dựng usedList
            // THEO TỪNG HỒ SƠ — giống hệt "Xuất Xml" (Ctrl E = ProcessExportXmlDetail). XML2 dùng usedList để tách
            // số lượng thuốc theo buổi; nếu truyền nguyên ListExpMedimateUsed (toàn bộ hồ sơ) thì IsMatchMedimateUsed
            // sẽ match nhầm chéo hồ sơ (fallback theo MEDICINE_ID/MATERIAL_ID không unique) → số lượng bị cộng dồn,
            // khiến T_TONGCHI_BV/T_TONGCHI_BH bị thổi phồng và lệch so với Ctrl E.
            Dictionary<long, List<HIS_EXP_MEDIMATE_USED>> dicExpUsedByMedicineId = new Dictionary<long, List<HIS_EXP_MEDIMATE_USED>>();
            Dictionary<long, List<HIS_EXP_MEDIMATE_USED>> dicExpUsedByMaterialId = new Dictionary<long, List<HIS_EXP_MEDIMATE_USED>>();
            if (ListExpMedimateUsed != null && ListExpMedimateUsed.Count > 0)
            {
                foreach (var u in ListExpMedimateUsed)
                {
                    if (u == null) continue;
                    if (u.EXP_MEST_MEDICINE_ID.HasValue)
                    {
                        var k = u.EXP_MEST_MEDICINE_ID.Value;
                        if (!dicExpUsedByMedicineId.ContainsKey(k)) dicExpUsedByMedicineId[k] = new List<HIS_EXP_MEDIMATE_USED>();
                        dicExpUsedByMedicineId[k].Add(u);
                    }
                    if (u.EXP_MEST_MATERIAL_ID.HasValue)
                    {
                        var k = u.EXP_MEST_MATERIAL_ID.Value;
                        if (!dicExpUsedByMaterialId.ContainsKey(k)) dicExpUsedByMaterialId[k] = new List<HIS_EXP_MEDIMATE_USED>();
                        dicExpUsedByMaterialId[k].Add(u);
                    }
                }
            }

            int stt = sttStart;
            try
            {

                // Vòng lặp tính toán từng hồ sơ và đẩy vào list chung
                foreach (var treatment in hisTreatments)  // BuildHsth01bhChiTietList core loop

                {
                    try
                    {
                        // Prepare Input
                        His.Bhyt.ExportXml.XMLTT12.XML01BH.InputXML01BHADO inputAdo = new His.Bhyt.ExportXml.XMLTT12.XML01BH.InputXML01BHADO();
                        inputAdo.HisConfig = this.NewConfig;
                        inputAdo.HisPatientTypes = hisPatientTypes;
                        inputAdo.vTreatment12 = new List<V_HIS_TREATMENT_12> { treatment };

                        if (dicPatientTypeAlter.ContainsKey(treatment.ID)) inputAdo.PatientTypeAlter = dicPatientTypeAlter[treatment.ID];
                        else inputAdo.PatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();

                        if (dicSereServ.ContainsKey(treatment.ID)) inputAdo.vSereServ2 = dicSereServ[treatment.ID];
                        else inputAdo.vSereServ2 = new List<V_HIS_SERE_SERV_2>();

                        if (dicSereServPttt.ContainsKey(treatment.ID)) inputAdo.vHisSereServPttt = dicSereServPttt[treatment.ID];
                        else inputAdo.vHisSereServPttt = new List<V_HIS_SERE_SERV_PTTT>();

                        // Processor
                        His.Bhyt.ExportXml.XMLTT12.XML01BH.Xml01BHProcessor xmlProcessor = new His.Bhyt.ExportXml.XMLTT12.XML01BH.Xml01BHProcessor(inputAdo);
                        var ado = xmlProcessor.GenerateXml01BhADOData();

                        if (ado != null)
                        {
                            // Recompute totals from XML2 (drug detail) + XML3 (DVKT detail) with IS_3176=true.19008552 
                            // Aligns C79 summary with detail rows so XML 130 viewer matches HSTH01BH file.
                            // GUARD: only override when detail processors actually returned rows;
                            // otherwise keep Xml01BHProcessor values (covers incomplete treatments).
                            try
                            {
                                // Đồng bộ TUYỆT ĐỐI với "Xuất Xml" (Ctrl E): hồ sơ ở luồng 130 đi qua 2 LỚP LỌC trước
                                // khi vào XML2/XML3:
                                //   Lớp 1 — gate dựng dicSereServ trong ProcessExportXmlDetail (xem ~dòng 1255-1269):
                                //            AMOUNT>0 && IS_EXPEND!=1 && (giữ dòng KHÔNG thực hiện; dòng CÓ thực hiện phải
                                //            PRICE>0, hoặc PRICE>=0 khi bật cấu hình LAY_CA_DVU_0_DONG).
                                //   Lớp 2 — CreateXmlProcessor.Check() (~dòng 370): (HEIN_SERVICE_TYPE có giá trị HOẶC khám
                                //            chính) && AMOUNT>0 && IS_EXPEND!=1.
                                // XML2/XML3 KHÔNG tự lọc IS_EXPEND, và XML3 còn nhận cả dòng có-thực-hiện PRICE==0
                                // (Xml3Processor nhánh PRICE==0). Nếu chỉ tái hiện Lớp 2 thì dòng hao phí (IS_EXPEND=1)
                                // và dòng thực hiện PRICE<=0 sẽ bị cộng thừa → T_BNTT (kéo theo T_TONGCHI_BV/T_TONGCHI_BH) 
                                // lệch so với Ctrl E. Vì vậy phải lọc ĐỦ CẢ 2 LỚP.
                                bool allowZeroPriceTT12 = HisConfigCFG.QD_130_BYT__LAY_CA_DVU_0_DONG == "1";
                                var sereServForDetail = (inputAdo.vSereServ2 ?? new List<V_HIS_SERE_SERV_2>())
                                    .Where(o =>
                                        // Lớp 2 — Check()
                                        (o.TDL_HEIN_SERVICE_TYPE_ID.HasValue
                                         || (!o.TDL_HEIN_SERVICE_TYPE_ID.HasValue
                                             && o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                                             && o.TDL_IS_MAIN_EXAM == 1))
                                        && o.AMOUNT > 0
                                        && o.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                        // Lớp 1 — gate dicSereServ của ProcessExportXmlDetail
                                        && ((o.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                                && (allowZeroPriceTT12 ? (o.PRICE > 0 || o.PRICE == 0) : o.PRICE > 0))
                                            || o.IS_NO_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE))
                                    .ToList();

                                // usedList CHỈ của hồ sơ hiện tại (match theo EXP_MEST_MEDICINE_ID/MATERIAL_ID của
                                // dịch vụ trong hồ sơ), dedupe theo ID — y hệt Ctrl E.
                                var usedListForTreatment = new List<HIS_EXP_MEDIMATE_USED>();
                                foreach (var ss in sereServForDetail)
                                {
                                    if (ss.EXP_MEST_MEDICINE_ID.HasValue
                                        && dicExpUsedByMedicineId.TryGetValue(ss.EXP_MEST_MEDICINE_ID.Value, out var lstMed))
                                        usedListForTreatment.AddRange(lstMed);
                                    if (ss.EXP_MEST_MATERIAL_ID.HasValue
                                        && dicExpUsedByMaterialId.TryGetValue(ss.EXP_MEST_MATERIAL_ID.Value, out var lstMat))
                                        usedListForTreatment.AddRange(lstMat);
                                }
                                usedListForTreatment = usedListForTreatment
                                    .GroupBy(x => x.ID)
                                    .Select(g => g.First())
                                    .ToList();

                                decimal sumTongChiBv = 0;
                                decimal sumTongChiBh = 0;
                                decimal sumBhtt = 0;
                                decimal sumBncct = 0;
                                decimal sumBntt = 0;
                                decimal sumNguonKhac = 0;
                                bool hasXml2Data = false;
                                bool hasXml3Data = false;

                                var input2 = new His.Bhyt.ExportXml.XML130.XML2.Base.InputADO();
                                input2.HisConfig = this.NewConfig;
                                input2.HisEmployee = BackendDataWorker.Get<HIS_EMPLOYEE>();
                                input2.vHisSereServPttt = inputAdo.vHisSereServPttt;
                                input2.vHisService = BackendDataWorker.Get<V_HIS_SERVICE>();
                                input2.vSereServ2 = sereServForDetail;
                                input2.HisExpMedimateUsed = usedListForTreatment;
                                input2.vTreatment12 = treatment;
                                input2.HisPatientTypes = hisPatientTypes;
                                input2.IS_3176 = true;

                                var data2 = new His.Bhyt.ExportXml.XML130.XML2.CreateXmlMain(input2).RunXml2Ado();
                                if (data2 != null && data2.DSACH_CHI_TIET_THUOC != null
                                    && data2.DSACH_CHI_TIET_THUOC.CHI_TIET_THUOC != null
                                    && data2.DSACH_CHI_TIET_THUOC.CHI_TIET_THUOC.Count > 0)
                                {
                                    hasXml2Data = true;
                                    foreach (var item2 in data2.DSACH_CHI_TIET_THUOC.CHI_TIET_THUOC)
                                    {
                                        decimal tmp;
                                        if (decimal.TryParse(item2.THANH_TIEN_BV, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)) sumTongChiBv += tmp;
                                        if (decimal.TryParse(item2.THANH_TIEN_BH, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)) sumTongChiBh += tmp;
                                        if (decimal.TryParse(item2.T_BHTT, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)) sumBhtt += tmp;
                                        if (decimal.TryParse(item2.T_BNCCT, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)) sumBncct += tmp;
                                        if (decimal.TryParse(item2.T_BNTT, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)) sumBntt += tmp;
                                        if (decimal.TryParse(item2.T_NGUONKHAC, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)) sumNguonKhac += tmp;
                                    }
                                }

                                var input3 = new His.Bhyt.ExportXml.XML130.XML3.InputXml3ADO();
                                input3.BedLogs = ListBedlog != null
                                    ? ListBedlog.Where(o => o.TREATMENT_ID == treatment.ID).ToList()
                                    : new List<V_HIS_BED_LOG>();
                                input3.ConfigData = this.NewConfig;
                                input3.EkipUsers = ListEkipUser;
                                input3.Employees = BackendDataWorker.Get<HIS_EMPLOYEE>();
                                input3.Icds = BackendDataWorker.Get<HIS_ICD>();
                                input3.ListSereServ = sereServForDetail;
                                input3.MaterialTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                                input3.PatientTypes = hisPatientTypes;
                                input3.SereServPttts = inputAdo.vHisSereServPttt;
                                input3.Services = BackendDataWorker.Get<V_HIS_SERVICE>();
                                input3.Treatment = treatment;
                                input3.vHisSereServTeins = HisSereServTeins != null
                                    ? HisSereServTeins.Where(o => o.TDL_TREATMENT_ID == treatment.ID).ToList()
                                    : new List<V_HIS_SERE_SERV_TEIN>();
                                input3.IS_3176 = true;

                                var data3 = new His.Bhyt.ExportXml.XML130.XML3.Xml3Processor(input3).GenerateXml3Data();
                                if (data3 != null && data3.DSACH_CHI_TIET_DVKT != null
                                    && data3.DSACH_CHI_TIET_DVKT.CHI_TIET_DVKT != null
                                    && data3.DSACH_CHI_TIET_DVKT.CHI_TIET_DVKT.Count > 0)
                                {
                                    hasXml3Data = true;
                                    foreach (var item3 in data3.DSACH_CHI_TIET_DVKT.CHI_TIET_DVKT)
                                    {
                                        decimal tmp;
                                        if (decimal.TryParse(item3.THANH_TIEN_BV, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)) sumTongChiBv += tmp;
                                        if (decimal.TryParse(item3.THANH_TIEN_BH, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)) sumTongChiBh += tmp;
                                        if (decimal.TryParse(item3.T_BHTT, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)) sumBhtt += tmp;
                                        if (decimal.TryParse(item3.T_BNCCT, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)) sumBncct += tmp;
                                        if (decimal.TryParse(item3.T_BNTT, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)) sumBntt += tmp;
                                        if (decimal.TryParse(item3.T_NGUONKHAC, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)) sumNguonKhac += tmp;
                                    }
                                }

                                if (hasXml2Data || hasXml3Data)
                                {
                                    ado.tTongChiBv = Math.Round(sumTongChiBv, 2, MidpointRounding.AwayFromZero);
                                    ado.tTongChiBh = Math.Round(sumTongChiBh, 2, MidpointRounding.AwayFromZero);
                                    ado.tBhtt = Math.Round(sumBhtt, 2, MidpointRounding.AwayFromZero);
                                    ado.tBncct = Math.Round(sumBncct, 2, MidpointRounding.AwayFromZero);
                                    ado.tBntt = Math.Round(sumBntt, 2, MidpointRounding.AwayFromZero);
                                    ado.tNguonKhac = Math.Round(sumNguonKhac, 2, MidpointRounding.AwayFromZero);
                                }
                            }
                            catch (Exception exSum)
                            {
                                Inventec.Common.Logging.LogSystem.Error(
                                    "Loi tinh lai tong tien C79 tu chi tiet XML2/XML3."
                                    + Inventec.Common.Logging.LogUtil.TraceData(
                                        Inventec.Common.Logging.LogUtil.GetMemberName(() => treatment.TREATMENT_CODE),
                                        treatment.TREATMENT_CODE),
                                    exSum);
                            }

                            HSTH01BH_CHITIET itemC79 = new HSTH01BH_CHITIET();
                            itemC79.STT = stt.ToString();
                            itemC79.HO_TEN = ado.hoTen;
                            itemC79.NGAY_SINH = ado.ngaySinh;
                            itemC79.GIOI_TINH = ado.gioiTinh;
                            itemC79.MA_THE_BHYT = ado.maTheBhyt;
                            itemC79.MA_BENH_CHINH = ado.maBenhChinh;
                            itemC79.NGAY_VAO = ado.ngayVao;
                            itemC79.NGAY_VAO_NOI_TRU = ado.ngayVaoNoiTru;
                            itemC79.NGAY_RA = ado.ngayRa;
                            itemC79.SO_NGAY_DTRI = ado.soNgayDieuTri.ToString();
                            itemC79.MA_LOAI_KCB = ado.maLoaiKcb;
                            itemC79.T_TONGCHI_BV = ado.tTongChiBv.HasValue ? ado.tTongChiBv.Value.ToString("0.##", CultureInfo.InvariantCulture) : "0";
                            itemC79.T_TONGCHI_BH = ado.tTongChiBh.HasValue ? ado.tTongChiBh.Value.ToString("0.##", CultureInfo.InvariantCulture) : "0";
                            itemC79.T_BHTT = ado.tBhtt.HasValue ? ado.tBhtt.Value.ToString("0.##", CultureInfo.InvariantCulture) : "0";
                            itemC79.T_BNCCT = ado.tBncct.HasValue ? ado.tBncct.Value.ToString("0.##", CultureInfo.InvariantCulture) : "0";
                            itemC79.T_BNTT = ado.tBntt.HasValue ? ado.tBntt.Value.ToString("0.##", CultureInfo.InvariantCulture) : "0";
                            itemC79.T_NGUONKHAC = ado.tNguonKhac.HasValue ? ado.tNguonKhac.Value.ToString("0.##", CultureInfo.InvariantCulture) : "0";
                            // MA_CSKCB lấy từ ADO do Xml01BHProcessor sinh ra (đồng bộ logic với XML1: treatment.HEIN_MEDI_ORG_CODE)
                            itemC79.MA_CSKCB = ado.maCsKcb;

                            // --- Tính năm tháng quyết toán theo cấu hình giống XML 3176 ---
                            string outTimeStr = treatment.OUT_TIME.HasValue ? treatment.OUT_TIME.Value.ToString() : "";
                            string heinLockTimeStr = treatment.HEIN_LOCK_TIME.HasValue ? treatment.HEIN_LOCK_TIME.Value.ToString() : "";

                            if (thoiGianQtOption == "1")
                            {
                                if (outTimeStr.Length >= 6)
                                {
                                    itemC79.NAM_QT = outTimeStr.Substring(0, 4);
                                    itemC79.THANG_QT = outTimeStr.Substring(4, 2);
                                }
                                else
                                {
                                    itemC79.NAM_QT = DateTime.Now.Year.ToString();
                                    itemC79.THANG_QT = DateTime.Now.Month.ToString();
                                }
                            }
                            else if (thoiGianQtOption == "2")
                            {
                                itemC79.NAM_QT = DateTime.Now.Year.ToString();
                                itemC79.THANG_QT = DateTime.Now.Month.ToString("00");
                            }
                            else // Các trường hợp khác 1 và 2 (bao gồm cả chưa cấu hình)
                            {
                                string timeToUse = !string.IsNullOrEmpty(heinLockTimeStr) ? heinLockTimeStr : outTimeStr;

                                if (!string.IsNullOrEmpty(timeToUse) && timeToUse.Length >= 6)
                                {
                                    itemC79.NAM_QT = timeToUse.Substring(0, 4);
                                    itemC79.THANG_QT = timeToUse.Substring(4, 2);
                                }
                                else
                                {
                                    itemC79.NAM_QT = DateTime.Now.Year.ToString();
                                    itemC79.THANG_QT = DateTime.Now.Month.ToString("00");
                                }
                            }

                            result.Add(itemC79);
                            stt++;
                        }
                        else
                        {
                            if (!dicErrorMess.ContainsKey("Lỗi sinh dữ liệu tính toán HSTH01BH")) dicErrorMess["Lỗi sinh dữ liệu tính toán HSTH01BH"] = new List<string>();
                            dicErrorMess["Lỗi sinh dữ liệu tính toán HSTH01BH"].Add(treatment.TREATMENT_CODE);
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        if (!dicErrorMess.ContainsKey(ex.Message)) dicErrorMess[ex.Message] = new List<string>();
                        dicErrorMess[ex.Message].Add(treatment.TREATMENT_CODE);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string ProcessExportXmlTT12Detail(ref bool isSuccess, List<V_HIS_TREATMENT_12> hisTreatments, List<V_HIS_PATIENT_TYPE_ALTER> hisPatientTypeAlters, List<V_HIS_SERE_SERV_2> listSereServ, List<V_HIS_SERE_SERV_PTTT> hisSereServPttts)
        {
            string result = "";
            Dictionary<string, List<string>> DicErrorMess = new Dictionary<string, List<string>>();
            try
            {
                // Build list HSTH01BH_CHITIET (logic compute tach ra de reuse cho Excel TT12)
                List<HSTH01BH_CHITIET> chiTietList = BuildHsth01bhChiTietList(
                    hisTreatments, hisPatientTypeAlters, listSereServ, hisSereServPttts, 1, DicErrorMess);

                // Khởi tạo đối tượng gom dữ liệu C79
                HSTH01BH hsth01bh = new HSTH01BH();
                hsth01bh.DS_CHITIET = new DS_CHITIET();
                hsth01bh.DS_CHITIET.DanhSachChiTiet = chiTietList ?? new List<HSTH01BH_CHITIET>();
                hsth01bh.CHUKYDONVI = "";

                // Xuất XML sau khi đã gom đủ dữ liệu vào list
                if (hsth01bh.DS_CHITIET.DanhSachChiTiet.Count > 0)
                {
                    string fullFileName = string.Format("HSTH01BH_{0}.xml", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                    string saveFilePath = Path.Combine(this.savePathADO.pathXmlTT12, fullFileName);

                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(HSTH01BH));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
                        settings.Encoding = new UTF8Encoding(false); // Loại bỏ BOM UTF-8
                        settings.Indent = false;
                        settings.OmitXmlDeclaration = false;

                        using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(ms, settings))
                        {
                            serializer.Serialize(writer, hsth01bh);
                        }

                        File.WriteAllBytes(saveFilePath, ms.ToArray());
                    }

                    // Gọi Ký số cho file CHSTH01BH
                    if (chkSignFileCertUtil.Checked)
                    {
                        if (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber))
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Không có thông tin HSM server/Usb Token ký số");
                        }
                        else
                        {
                            bool signSuccess = SignXmlFileTT12(saveFilePath);
                            if (!signSuccess)
                            {
                                if (!DicErrorMess.ContainsKey("Lỗi ký số XML HSTH01BH")) DicErrorMess["Lỗi ký số XML HSTH01BH"] = new List<string>();
                                DicErrorMess["Lỗi ký số XML HSTH01BH"].Add(fullFileName);

                                if (File.Exists(saveFilePath)) File.Delete(saveFilePath); // Xóa file nếu ký tạch
                            }
                        }
                    }

                    isSuccess = true;
                }

                if (DicErrorMess.Count > 0)
                {
                    foreach (var item in DicErrorMess)
                    {
                        result += String.Format("{0}:{1}. ", item.Key, String.Join(",", item.Value));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result += "Lỗi xử lý dữ liệu xuất XML C79.";
            }
            return result;
        }
        private bool GenerateXmlTT12(ref CommonParam paramExport, List<V_HIS_TREATMENT_1> listSelection)
        {
            bool result = false;
            try
            {
                if (listSelection.Count > 0)
                {
                    listSelection = listSelection.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();
                    this.NewConfig = GetNewConfig();

                    // Khởi tạo list 1 lần, các batch sẽ AddRange dồn vào
                    ListPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
                    ListSereServ = new List<V_HIS_SERE_SERV_2>();
                    HisTreatments = new List<V_HIS_TREATMENT_12>();
                    HisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();

                    int skip = 0;
                    while (listSelection.Count - skip > 0)
                    {
                        var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                        isExportXml = true;
                        CreateThreadGetData(limit); // Gọi Thread lấy Data song song
                        isExportXml = false;
                    }

                    // Pass Data xuống hàm xử lý chi tiết
                    string message = ProcessExportXmlTT12Detail(ref result, HisTreatments, ListPatientTypeAlter, ListSereServ, HisSereServPttts);

                    if (!String.IsNullOrEmpty(message))
                    {
                        paramExport.Messages.Add(message);
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

        private bool SignXmlFileTT12(string sourceFile)
        {
            try
            {
                if (SettingSignADO == null) return false;
                if (string.IsNullOrEmpty(sourceFile) || !File.Exists(sourceFile)) return false;

                string currentDirectory = Directory.GetCurrentDirectory();
                string tempFolderPath = Path.Combine(currentDirectory, "Temp");
                Directory.CreateDirectory(tempFolderPath);

                string fullFileName = Path.GetFileName(sourceFile);
                string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                File.Create(tempFilePath).Close();

                string pathAfterFileSign = null;
                WcfSignDCO wcfSignDCO = null;

                if (SettingSignADO.IsHsm)
                {
                    var xmlBase64 = SourceFileSignApi(ReadFileContent(sourceFile));
                    if (string.IsNullOrEmpty(xmlBase64)) return false;

                    var xmlBytes = Convert.FromBase64String(xmlBase64);
                    File.WriteAllBytes(tempFilePath, xmlBytes);
                    pathAfterFileSign = tempFilePath;
                }
                else
                {
                    wcfSignDCO = new WcfSignDCO
                    {
                        SerialNumber = SettingSignADO.SerialNumber,
                        OutputFile = tempFilePath,
                        PIN = "",
                        SourceFile = sourceFile,
                        fieldSigned = "CHUKYDONVI"
                    };

                    string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                    SignProcessorClient signProcessorClient = new SignProcessorClient();

                    if (!VerifyServiceSignProcessorIsRunning()) return false;

                    var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                    if (wcfSignResultDCO == null || !wcfSignResultDCO.Success) return false;

                    pathAfterFileSign = wcfSignResultDCO.OutputFile;
                }

                // Chép lại file đã ký đè lên thư mục Output
                if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXmlTT12))
                {
                    if (wcfSignDCO != null)
                    {
                        File.Copy(pathAfterFileSign, wcfSignDCO.SourceFile, true);
                    }
                    else if (SettingSignADO.IsHsm)
                    {
                        File.Copy(pathAfterFileSign, sourceFile, true);
                    }
                }

                // Dọn dẹp thư mục Temp
                foreach (string file in Directory.GetFiles(tempFolderPath))
                {
                    File.Delete(file);
                }

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void gridViewTreatment_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {

                if (listSelection != null && listSelection.Count > 0)
                {
                    DXMenuItem menuItemXuatXML = new DXMenuItem("Xuất XML", new EventHandler(this.MenuItemClick_XuatXML130));
                    e.Menu.Items.Add(menuItemXuatXML);

                    DXMenuItem menuItemXuatXMLKhongBaoGomGDYK = new DXMenuItem("Xuất XML (không bao gồm giám định y khoa)", new EventHandler(this.MenuItemClick_XuatXMLKhongBaoGomGDYK));
                    e.Menu.Items.Add(menuItemXuatXMLKhongBaoGomGDYK);

                    DXMenuItem menuItemXuatXmlGiamDinhYKhoa = new DXMenuItem("Xuất XML giám định y khoa", new EventHandler(this.MenuItemClick_XuatXmlGiamDinhYKhoa));
                    e.Menu.Items.Add(menuItemXuatXmlGiamDinhYKhoa);

                    DXMenuItem menuItemXuatXmlCheckIn = new DXMenuItem("Xuất lại file XML check-in server (file được sinh ra khi thiết lập xuất tự động)", new EventHandler(this.MenuItemClick_XuatXmlCheckIn));
                    e.Menu.Items.Add(menuItemXuatXmlCheckIn);

                    DXMenuItem menuItemXuatXmlTT = new DXMenuItem("Xuất XML thông tuyến", new EventHandler(this.btnExportCollinearXml_Click));
                    e.Menu.Items.Add(menuItemXuatXmlTT);

                    DXMenuItem menuItemXuatXml130Excel = new DXMenuItem("Xuất XML 130 dưới dạng excel", new EventHandler(this.btnExportXmlToExcel_Click));
                    e.Menu.Items.Add(menuItemXuatXml130Excel);

                    //DXMenuItem menuItemXuatXmlTT12Excel = new DXMenuItem("Xuất XML TT12 dưới dạng excel", new EventHandler(this.btnExportXmlTT12ToExcel_Click));
                    //e.Menu.Items.Add(menuItemXuatXmlTT12Excel);

                    // PTTK 3142: kiem tra pham vi chuyen mon + validate xuat XML, khong tao file
                    DXMenuItem menuItemKiemTraHoSo = new DXMenuItem("Kiểm tra hồ sơ", new EventHandler(this.MenuItemClick_KiemTraHoSo));
                    e.Menu.Items.Add(menuItemKiemTraHoSo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExportXmlToExcel_Click(object sender, EventArgs e)
        {
            ProcessDataExcel();
        }

        private void btnExportXmlTT12ToExcel_Click(object sender, EventArgs e)
        {
            ProcessDataExcelTT12();
        }

        private void MenuItemClick_XuatXmlCheckIn(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                List<long> listTreatmentIds = listSelection.Select(o => o.ID).ToList();
                int skip = 0;
                while (listTreatmentIds.Count - skip > 0)
                {
                    List<long> limit = listTreatmentIds.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<V_HIS_TREATMENT_1>>("api/Histreatment/ExportXmlCheckIn", ApiConsumers.MosConsumer, limit, param);
                    if (rs != null && rs.Count > 0)
                    {
                        if (rs.Exists(o => o.XML_CHECKIN_RESULT == 1))
                        {
                            success = true;
                        }
                        FillDataToGridTreatment();
                    }
                }
                WaitingManager.Hide();

                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MenuItemClick_XuatXML130(object sender, EventArgs e)
        {
            btnExportXml_Click(null, null);
        }
        private void MenuItemClick_XuatXmlGiamDinhYKhoa(object sender, EventArgs e)
        {
            try
            {
                if (listSelection == null || listSelection.Count == 0) return;
                CommonParam param = new CommonParam();
                MemoryStream memoryStream = new MemoryStream();
                bool success = false;

                if (this.savePathADO == null || string.IsNullOrEmpty(this.savePathADO.pathXmlGDYK))
                {
                    if (XtraMessageBox.Show("Chưa chọn thư mục lưu file chỉ tiêu dữ liệu giám định y khoa. Bạn có muốn chọn đường dẫn không?", Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                    btnSavePath_Click(null, null);
                }
                if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXmlGDYK))
                {
                    WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Info("MenuItemClick_XuatXmlGiamDinhYKhoa Begin");
                    listSelection = listSelection.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();
                    this.NewConfig = GetNewConfig();
                    int skip = 0;
                    while (listSelection.Count - skip > 0)
                    {
                        var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;
                        HisTreatments = new List<V_HIS_TREATMENT_12>();
                        ListMedicalAssessment = new List<V_HIS_MEDICAL_ASSESSMENT>();
                        string message = "";

                        HisTreatmentView12Filter treatmentFilter = new HisTreatmentView12Filter();
                        treatmentFilter.IDs = limit.Select(o => o.ID).ToList();
                        var resultTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_12>>("api/HisTreatment/GetView12", ApiConsumers.MosConsumer, treatmentFilter, param);
                        if (resultTreatment != null && resultTreatment.Count > 0)
                        {
                            HisTreatments.AddRange(resultTreatment);
                        }

                        HisMedicalAssessmentViewFilter filter = new HisMedicalAssessmentViewFilter();
                        filter.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                        var resultMedicalAssessment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_MEDICAL_ASSESSMENT>>("api/HisMedicalAssessment/GetView", ApiConsumers.MosConsumer, filter, param);
                        if (resultMedicalAssessment != null && resultMedicalAssessment.Count > 0)
                        {
                            ListMedicalAssessment.AddRange(resultMedicalAssessment);
                        }
                        Dictionary<string, List<string>> DicErrorMess = new Dictionary<string, List<string>>();
                        Dictionary<long, List<V_HIS_MEDICAL_ASSESSMENT>> dicMedicalAssessment = new Dictionary<long, List<V_HIS_MEDICAL_ASSESSMENT>>();

                        if (ListMedicalAssessment != null && ListMedicalAssessment.Count > 0)
                        {
                            foreach (var item in ListMedicalAssessment)
                            {
                                if (!dicMedicalAssessment.ContainsKey(item.TREATMENT_ID))
                                    dicMedicalAssessment[item.TREATMENT_ID] = new List<V_HIS_MEDICAL_ASSESSMENT>();

                                dicMedicalAssessment[item.TREATMENT_ID].Add(item);
                            }
                        }
                        foreach (var treatment in HisTreatments)
                        {
                            int count = 0;
                            InputADO ado = new InputADO();
                            if (chkXML3176.Checked)
                            {
                                ado.IS_3176 = true;
                            }
                            else
                            {
                                ado.IS_3176 = false;
                            }
                            ado.Treatment = treatment;
                            if (dicMedicalAssessment.ContainsKey(treatment.ID))
                            {
                                ado.ListMedicalAssessment = dicMedicalAssessment[treatment.ID];
                            }
                            if (HisConfigCFG.QD_130_BVT_XML1_MA_KHOA_OPTION == "1")
                            {
                                ado.ListDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
                            }
                            ado.TotalConfigData = NewConfig;
                            His.Bhyt.ExportXml.XML130.CreateXmlProcessor xmlProcessor = new His.Bhyt.ExportXml.XML130.CreateXmlProcessor(ado);
                            SyncResultADO syncResult = null;
                            MemoryStream resultSync = null;
                            string errorMess = "";
                            string fullFileName = "";
                            fullFileName = xmlProcessor.GetFileName();
                            string saveFilePathXml12 = String.Format("{0}/{1}{2}", this.savePathADO.pathXmlGDYK, "XML12_", fullFileName);
                            var rsXml12 = xmlProcessor.RunXml12(ref errorMess);
                            if (!String.IsNullOrWhiteSpace(errorMess))
                            {
                                Inventec.Common.Logging.LogSystem.Error("Run130: " + errorMess);
                            }
                            if (rsXml12 != null)
                            {
                                FileStream file12 = new FileStream(saveFilePathXml12, FileMode.Create, FileAccess.Write);
                                rsXml12.WriteTo(file12);
                                file12.Close();
                                rsXml12.Close();
                                success = true;
                            }
                            else
                            {
                                if (!DicErrorMess.ContainsKey(errorMess))
                                {
                                    DicErrorMess[errorMess] = new List<string>();
                                }

                                DicErrorMess[errorMess].Add(treatment.TREATMENT_CODE);
                            }
                        }
                        if (DicErrorMess.Count > 0)
                        {
                            foreach (var item in DicErrorMess)
                            {
                                message += String.Format("{0}:{1}. ", item.Key, String.Join(",", item.Value));
                            }
                        }
                        if (!String.IsNullOrEmpty(message))
                        {
                            param.Messages.Add(message);
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Info("MenuItemClick_XuatXmlGiamDinhYKhoa End");
                    WaitingManager.Hide();
                    if (success && param.Messages.Count == 0)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }

                    this.gridControlTreatment.RefreshDataSource();
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void MenuItemClick_XuatXMLKhongBaoGomGDYK(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (listSelection == null || listSelection.Count == 0) return;
                    CommonParam param = new CommonParam();
                    MemoryStream memoryStream = new MemoryStream();
                    bool success = false;

                    if (this.savePathADO == null || string.IsNullOrEmpty(this.savePathADO.pathXml))
                    {
                        btnSavePath_Click(null, null);
                    }
                    if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXml))
                    {
                        WaitingManager.Show();
                        Inventec.Common.Logging.LogSystem.Info("MenuItemClick_XuatXMLKhongBaoGomGDYK - Checkbox: " + chkXML3176.Checked);
                        success = this.GenerateXml(ref param, ref memoryStream, false, false, true, listSelection, chkXML3176.Checked);
                        WaitingManager.Hide();
                        if (success && param.Messages.Count == 0)
                        {
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                        else
                        {
                            MessageManager.Show(param, success);
                        }

                        this.gridControlTreatment.RefreshDataSource();
                    }
                    SessionManager.ProcessTokenLost(param);
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSavePath_Click(object sender, EventArgs e)
        {
            try
            {
                frmSettingSavePath frmSettingSavePath = new frmSettingSavePath(savePathADO, UpdateSavePath);
                frmSettingSavePath.ShowDialog(this.ParentForm);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateSavePath(SavePathADO savePathADO)
        {
            try
            {
                if (savePathADO == null)
                    savePathADO = new SavePathADO();
                this.savePathADO = savePathADO;
                string value = Newtonsoft.Json.JsonConvert.SerializeObject(this.savePathADO);
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == btnSavePath.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = btnSavePath.Name;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SaveCheckboxXML3176State()
        {
            try
            {
                string value = chkXML3176.Checked.ToString();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate =
                    (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                    ? this.currentControlStateRDO.Where(o => o.KEY == "chkXML3176" && o.MODULE_LINK == moduleLink).FirstOrDefault()
                    : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = "chkXML3176";
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = moduleLink;

                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientTypeTT_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                this.searchFilter.listDTTT = new List<HIS_PATIENT_TYPE>();
                foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    this.searchFilter.listDTTT.Add(rv);
                    sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async void btnExportCollinearXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportCollinearXml.Enabled || listSelection == null || listSelection.Count == 0) return;

                //vCong53286 - Cổng chặn tiền giám định. Hồ sơ có lỗi nghiêm trọng thì dừng cả lượt, không sinh tệp nào.
                if (!await EnsureTienGiamDinhPassedAsync()) return;
                CommonParam param = new CommonParam();
                MemoryStream memoryStream = new MemoryStream();
                bool success = false;

                if (this.savePathADO == null || string.IsNullOrEmpty(this.savePathADO.pathCollinearXml))
                {
                    XtraMessageBox.Show("Vui lòng thiết lập thư mục lưu trữ trước khi xuất dữ liệu.", Resources.ResourceMessageLang.ThongBao);
                    btnSavePath_Click(null, null);
                }
                if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathCollinearXml))
                {
                    //if (string.IsNullOrEmpty(SerialNumber))
                    //{
                    //    MessageBox.Show("Không có thông tin Usb Token ký số");
                    //    return;
                    //}
                    WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Info("btnExportCollinearXml_Click Begin");
                    Inventec.Common.Logging.LogSystem.Info("btnExportCollinearXml - checkbox XML3176: " + chkXML3176.Checked);
                    success = this.GenerateXml(ref param, ref memoryStream, false, true, true, listSelection, chkXML3176.Checked);
                    Inventec.Common.Logging.LogSystem.Info("btnExportCollinearXml_Click End");
                    WaitingManager.Hide();
                    if (success && param.Messages.Count == 0)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else if (param.Messages.Count > 0)
                    {
                        MessageManager.Show(param, success);
                    }

                    this.gridControlTreatment.RefreshDataSource();
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();

            }
        }
        #region luu tim kiem
        private void cboStatus_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboStatus.EditValue != null)
                {
                    this.searchFilter.prfileType = this.ListStatusAll.Where(s => s.id == Convert.ToInt64(cboStatus.EditValue)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void cboXml130Result_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboXml130Result.EditValue != null)
                {
                    this.searchFilter.statusXml = this.ListXml130ResultAll.Where(s => s.id == Convert.ToInt64(cboXml130Result.EditValue)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SaveSearchFilter()
        {
            try
            {
                string value = Newtonsoft.Json.JsonConvert.SerializeObject(this.searchFilter);
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == btnFind.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = btnFind.Name;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        private void SetDefaultSearchFilter()
        {
            try
            {
                if (this.searchFilter != null)
                {
                    if (this.searchFilter.listBranch != null)
                    {
                        GridCheckMarksSelection gridCheck = CboBranch.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheck != null)
                        {
                            gridCheck.ClearSelection(CboBranch.Properties.View);
                            var rs = listBranchDataSource.Where(s => this.searchFilter.listBranch.Select(o => o.ID).Contains(s.ID)).Distinct().ToList();
                            gridCheck.SelectAll(rs);

                        }
                    }
                    if (this.searchFilter.listPatientType != null)
                    {
                        GridCheckMarksSelection gridCheck = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheck != null)
                        {
                            gridCheck.ClearSelection(cboPatientType.Properties.View);
                            var rs = listPatientTypeDataSource.Where(s => this.searchFilter.listPatientType.Select(o => o.ID).Contains(s.ID)).Distinct().ToList();
                            gridCheck.SelectAll(rs);
                        }
                    }
                    if (this.searchFilter.listPTreattmentType != null)
                    {
                        GridCheckMarksSelection gridCheck = cboFilterTreatmentType.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheck != null)
                        {
                            gridCheck.ClearSelection(cboFilterTreatmentType.Properties.View);
                            var rs = listTreatmentTypeDataSource.Where(s => this.searchFilter.listPTreattmentType.Select(o => o.ID).Contains(s.ID)).Distinct().ToList();
                            gridCheck.SelectAll(rs);
                        }
                    }
                    if (this.searchFilter.listDTTT != null)
                    {
                        GridCheckMarksSelection gridCheck = cboPatientTypeTT.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheck != null)
                        {
                            gridCheck.ClearSelection(cboPatientTypeTT.Properties.View);
                            var rs = listPatientTypeTTDataSource.Where(s => this.searchFilter.listDTTT.Select(o => o.ID).Contains(s.ID)).Distinct().ToList();
                            gridCheck.SelectAll(rs);
                        }
                    }
                    if (this.searchFilter.prfileType != null)
                    {
                        cboStatus.EditValue = this.searchFilter.prfileType.id;
                    }
                    if (this.searchFilter.statusXml != null)
                    {
                        cboXml130Result.EditValue = this.searchFilter.statusXml.id;
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void btnExportGroupXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportGroupXml.Enabled || listSelection == null || listSelection.Count == 0) return;

                //vCong53286 - Cổng chặn tiền giám định. Hồ sơ có lỗi nghiêm trọng thì dừng cả lượt, không sinh tệp nào.
                if (!await EnsureTienGiamDinhPassedAsync()) return;
                CommonParam param = new CommonParam();
                MemoryStream memoryStream = new MemoryStream();
                bool success = false;
                bool xuatXml12 = true;

                if (this.savePathADO == null || string.IsNullOrEmpty(this.savePathADO.pathXml))
                {
                    btnSavePath_Click(null, null);
                }
                if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXml))
                {
                    if (string.IsNullOrEmpty(this.savePathADO.pathXmlGDYK))
                    {
                        if (XtraMessageBox.Show("Chưa chọn thư mục lưu file chỉ tiêu dữ liệu giám định y khoa. Bạn có muốn chọn đường dẫn không?", Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                            btnSavePath_Click(null, null);
                    }
                    IsProcessingExcel = false;
                    if (chkSignFileCertUtil.Checked == false)
                    {
                        WaitingManager.Show();
                        isNotFileSign = true;
                        Inventec.Common.Logging.LogSystem.Info("btnExportGroupXml - Checkbox: " + chkXML3176.Checked);
                        Inventec.Common.Logging.LogSystem.Info("btnExportGroupXml - Calling GenerateXmlPlus with isXML3176: " + chkXML3176.Checked);
                        success = this.GenerateXmlPlus(ref param, ref memoryStream, xuatXml12, listSelection, chkXML3176.Checked);
                        WaitingManager.Hide();
                    }
                    else
                    {
                        if (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber))
                        {
                            if (XtraMessageBox.Show("Không có thông tin HSM server/Usb Token ký số. Bạn có muốn tiếp tục xuất xml?", Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                return;
                            }
                            else
                            {
                                isNotFileSign = true;
                                WaitingManager.Show();
                                success = this.GenerateXmlPlus(ref param, ref memoryStream, xuatXml12, listSelection, chkXML3176.Checked);
                                WaitingManager.Hide();
                            }
                        }
                        else
                        {
                            isNotFileSign = false;
                            WaitingManager.Show();
                            success = this.GenerateXmlPlus(ref param, ref memoryStream, xuatXml12, listSelection, chkXML3176.Checked);
                            WaitingManager.Hide();
                        }
                    }

                    if (success && param.Messages.Count == 0)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }

                    this.gridControlTreatment.RefreshDataSource();
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public string AppFilePathSignService()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "Integrate"), "EMR.SignProcessor"), "EMR.SignProcessor.exe");
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }
        private bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                {
                    return true;
                }
            }

            return false;
        }
        internal bool VerifyServiceSignProcessorIsRunning()
        {
            bool valid = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.1");
                string exeSignPath = AppFilePathSignService();
                if (File.Exists(exeSignPath))
                {
                    if (IsProcessOpen("EMR.SignProcessor"))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.2");
                        valid = true;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.3");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => exeSignPath), exeSignPath));
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exeSignPath;
                        try
                        {

                            Process.Start(startInfo);
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.4");
                            Thread.Sleep(500);
                            valid = true;
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.5");
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        private void chkSignFileCertUtil_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                    return;

                isChkSignFileCertUtil();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void chkXML3176_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                    return;

                SaveCheckboxXML3176State(); // Lưu trạng thái

                UpdateBtnXML3176Visibility();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        bool chooseChungThu = true;
        private void isChkSignFileCertUtil()
        {
            try
            {
                if (chkSignFileCertUtil.Checked == true)
                {
                    frmSetting frm = new frmSetting(SettingSignADO, (result) =>
                    {
                        SettingSignADO = (SettingSignADO)result;
                    });
                    frm.ShowDialog();
                    if (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber))
                        chkSignFileCertUtil.Checked = false;
                }
                else
                {
                    SettingSignADO = null;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignFileCertUtil.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(SettingSignADO);
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignFileCertUtil.Name;
                    csAddOrUpdate.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(SettingSignADO);
                    csAddOrUpdate.MODULE_LINK = this.currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private async void btnXML3176_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnXML3176.Enabled || listSelection == null || listSelection.Count == 0) return;

                //vCong53286 - Cổng chặn tiền giám định. Hồ sơ có lỗi nghiêm trọng thì dừng cả lượt, không sinh tệp nào.
                if (!await EnsureTienGiamDinhPassedAsync()) return;
                CommonParam param = new CommonParam();
                MemoryStream memoryStream = new MemoryStream();
                bool success = false;
                bool xuatXml12 = true;

                if (this.savePathADO == null || string.IsNullOrEmpty(this.savePathADO.pathXml))
                {
                    btnSavePath_Click(null, null);
                }
                if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXml))
                {
                    if (string.IsNullOrEmpty(this.savePathADO.pathXmlGDYK))
                    {
                        if (XtraMessageBox.Show("Chưa chọn thư mục lưu file chỉ tiêu dữ liệu giám định y khoa. Bạn có muốn chọn đường dẫn không?", Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                            btnSavePath_Click(null, null);
                    }
                    xuatXml12 = !string.IsNullOrEmpty(this.savePathADO.pathXmlGDYK);

                    WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Info("btnXML3176_Click Begin - Force xuất XML 3176");

                    // --- SỬA CHÍNH: KHÔNG CẦN ÉP CHECKBOX NỮA ---
                    // Truyền thẳng TRUE vào tham số cuối cùng (isXML3176)
                    success = this.GenerateXml(ref param, ref memoryStream, false, false, xuatXml12, listSelection, true);
                    // ---------------------------------------------

                    Inventec.Common.Logging.LogSystem.Info("btnXML3176_Click End");
                    WaitingManager.Hide();
                    if (success && param.Messages.Count == 0)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else if (param.Messages.Count > 0)
                    {
                        MessageManager.Show(param, success);
                    }

                    this.gridControlTreatment.RefreshDataSource();
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void btnXML3176_Send(object sender, EventArgs e)
        {
            try
            {
                btnAutoSyncClick = false;
                isXML130 = false;
                showMessSusscess = false;
                isXML3176 = true;
                isAutoSignXML3176 = false;
                showMessSusscess = true;
                await XML130();
                FillDataToGridTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string ReadFileContent(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    XmlDocument xmlDocument = new XmlDocument();
                    try
                    {
                        xmlDocument.LoadXml(RemoveByteOrderMark(Encoding.UTF8.GetString(File.ReadAllBytes(filePath))));
                        return Convert.ToBase64String(StringToBytes(RemoveByteOrderMark(Encoding.UTF8.GetString(fileBytes))));
                    }
                    catch (Exception)
                    {
                        xmlDocument.LoadXml(Encoding.UTF8.GetString(File.ReadAllBytes(filePath)));
                        return Convert.ToBase64String(StringToBytes(Encoding.UTF8.GetString(fileBytes)));
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        private string RemoveByteOrderMark(string XML)
        {
            string byteOrderMark = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (XML.StartsWith(byteOrderMark))
            {
                XML = XML.Remove(0, byteOrderMark.Length);
            }
            return XML;
        }
        public byte[] StringToBytes(string input)
        {
            if (input == null) return null;
            return Encoding.UTF8.GetBytes(input);
        }
        private void UpdateBtnXML3176Visibility()
        {
            try
            {
                var targetVisibility = !chkXML3176.Checked ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                // 1. Tìm LayoutItem đang chứa nút btnXML3176
                var item = layoutControl1.GetItemByControl(btnXML3176);

                if (item != null)
                {
                    // Nếu tìm thấy LayoutItem -> Set Visibility của LayoutItem
                    if (item.Visibility != targetVisibility)
                    {
                        item.Visibility = targetVisibility;
                    }
                }
                else
                {
                    // Trường hợp dự phòng (nếu không dùng LayoutControl cho nút này)
                    btnXML3176.Visible = !chkXML3176.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void btnExportXml12_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportXml12.Enabled || listSelection == null || listSelection.Count == 0) return;

                //vCong53286 - Cổng chặn tiền giám định. Hồ sơ có lỗi nghiêm trọng thì dừng cả lượt, không sinh tệp nào.
                if (!await EnsureTienGiamDinhPassedAsync()) return;
                CommonParam param = new CommonParam();
                bool success = false;

                if (this.savePathADO == null || string.IsNullOrEmpty(this.savePathADO.pathXmlTT12))
                {
                    XtraMessageBox.Show("Vui lòng thiết lập thư mục lưu trữ trước khi xuất dữ liệu.", Resources.ResourceMessageLang.ThongBao);
                    btnSavePath_Click(null, null);
                    return;
                }

                if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXmlTT12))
                {
                    WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Info("btnExportXml12_Click Begin");

                    success = this.GenerateXmlTT12(ref param, listSelection);

                    Inventec.Common.Logging.LogSystem.Info("btnExportXml12_Click End");
                    WaitingManager.Hide();

                    if (success && param.Messages.Count == 0)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else if (param.Messages.Count > 0)
                    {
                        MessageManager.Show(param, success);
                    }

                    this.gridControlTreatment.RefreshDataSource();
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
