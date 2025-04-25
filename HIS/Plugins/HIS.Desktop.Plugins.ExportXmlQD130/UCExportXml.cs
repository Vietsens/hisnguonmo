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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.SereServTree;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Controls.Session;
using His.Bhyt.ExportXml.XML130;
using HIS.Desktop.Plugins.ExportXmlQD130.Base;
using HIS.Desktop.Utility;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.IO;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using HIS.Desktop.LibraryMessage;
using MOS.SDO;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.Utils.Menu;
using DevExpress.Utils;
using Inventec.Fss.Client;
using Newtonsoft.Json;
using DevExpress.XtraBars;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using EMR.WCF.DCO;
using Inventec.Common.SignLibrary.ServiceSign;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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
        public SavePathADO savePathADO;
        bool isExportXml;
        bool isSendCollinearXml;
        bool isNotFileSign;
        string SerialNumber;
        bool isXML3176;
        bool btnExportXML3176 = false;
        bool isXML130;
        bool showMessSusscess;
        bool isAutoSignXML3176 = false;
        bool btnAutoSyncClick = false;
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
                if (HisConfigCFG.BHXH__XML_SIGN_OPTION == "1")
                {
                    chkSignFileCertUtil.Checked = true;
                    chkSignFileCertUtil.Properties.ReadOnly = true;
                    chkSignFileCertUtil.Enabled = false;
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(SerialNumber))
                    {
                        chkSignFileCertUtil.Checked = !String.IsNullOrWhiteSpace(SerialNumber);
                    }
                    else
                    {
                        chkSignFileCertUtil.Checked = false;
                        chkSignFileCertUtil.Properties.ReadOnly = false;
                    }
                }
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

                btnSend.DropDownControl = menu;
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
                    btnXML3176.Enabled = true;
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

        private void btnExportXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportXml.Enabled || listSelection == null || listSelection.Count == 0) return;
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
                    success = this.GenerateXml(ref param, ref memoryStream, false, false, xuatXml12, listSelection);
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
        bool GenerateXml(ref CommonParam paramExport, ref MemoryStream memoryStream, bool viewXml, bool xuatXmlTT, bool xuatXml12, List<V_HIS_TREATMENT_1> listSelection)
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
                        string message = "";
                        isExportXml = true;
                        CreateThreadGetData(limit);
                        isExportXml = false;
                        if (chkSignFileCertUtil.Checked == false)
                        {
                            isNotFileSign = true;
                            message = ProcessExportXmlDetail(ref result, ref memoryStream, viewXml, xuatXmlTT, xuatXml12, HisTreatments, ListPatientTypeAlter, ListSereServ, ListDhst, HisSereServTeins, HisTrackings, HisSereServPttts, ListEkipUser, ListBedlog, ListDebates, ListBaby, ListMedicalAssessment, ListHivTreatment, HisSereServSuin, ListTuberculosisTreat);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(SerialNumber))
                            {
                                if (XtraMessageBox.Show("Không có thông tin Usb Token ký số. Bạn có muốn tiếp tục xuất xml?", Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    message = "";
                                }
                                else
                                {
                                    isNotFileSign = true;
                                    message = ProcessExportXmlDetail(ref result, ref memoryStream, viewXml, xuatXmlTT, xuatXml12, HisTreatments, ListPatientTypeAlter, ListSereServ, ListDhst, HisSereServTeins, HisTrackings, HisSereServPttts, ListEkipUser, ListBedlog, ListDebates, ListBaby, ListMedicalAssessment, ListHivTreatment, HisSereServSuin, ListTuberculosisTreat);
                                }
                            }
                            else
                            {
                                isNotFileSign = false;
                                message = ProcessExportXmlDetail(ref result, ref memoryStream, viewXml, xuatXmlTT, xuatXml12, HisTreatments, ListPatientTypeAlter, ListSereServ, ListDhst, HisSereServTeins, HisTrackings, HisSereServPttts, ListEkipUser, ListBedlog, ListDebates, ListBaby, ListMedicalAssessment, ListHivTreatment, HisSereServSuin, ListTuberculosisTreat);
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

        bool GenerateXmlPlus(ref CommonParam paramExport, ref MemoryStream memoryStream, bool xuatXml12, List<V_HIS_TREATMENT_1> listSelection)
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
                    string message = "";
                    while (listSelection.Count - skip > 0)
                    {
                        var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                        isExportXml = true;
                        CreateThreadGetData(limit);
                        isExportXml = false;

                    }
                    message = ProcessExportXmlDetailPlus(ref result, ref memoryStream, xuatXml12);
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
            List<V_HIS_BED_LOG> ListBedlog, List<HIS_DEBATE> listDebate, List<V_HIS_BABY> listBaby, List<V_HIS_MEDICAL_ASSESSMENT> listMedicalAssessment, List<HIS_HIV_TREATMENT> listHivTreatment, List<V_HIS_SERE_SERV_SUIN> listSereServSuin, List<HIS_TUBERCULOSIS_TREAT> lstTuberculosisTreat)
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

                if (ListSereServ != null && ListSereServ.Count > 0)
                {
                    foreach (var sereServ in ListSereServ)
                    {
                        if (sereServ.TDL_HEIN_SERVICE_TYPE_ID.HasValue && sereServ.AMOUNT > 0 && sereServ.PRICE > 0 && sereServ.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.TDL_TREATMENT_ID.HasValue)
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
                    if (XuatXml12 && dicMedicalAssessment.ContainsKey(treatment.ID))
                    {
                        ado.ListMedicalAssessment = dicMedicalAssessment[treatment.ID];
                    }
                    if (dicHivTreatment.ContainsKey(treatment.ID))
                    {
                        ado.HivTreatment = dicHivTreatment[treatment.ID];
                    }
                    ado.TotalMaterialTypeData = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                    ado.TotalHeinMediOrgData = BackendDataWorker.Get<HIS_MEDI_ORG>();
                    ado.TotalConfigData = NewConfig;
                    ado.TotalPatientTypeData = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                    ado.TotalIcdData = BackendDataWorker.Get<HIS_ICD>();
                    ado.TotalSericeData = BackendDataWorker.Get<V_HIS_SERVICE>();
                    ado.TotalEmployeeData = BackendDataWorker.Get<HIS_EMPLOYEE>();
                    ado.serverInfo = new ServerInfo() { Username = username, Password = password, Address = address, TypeXml = typeXml, Xml130Api = xml130Api, XmlGdykApi = xmlGdykApi };
                    if (!isNotFileSign)
                        ado.delegateSignXml = DataSignXML;
                    if (dicTuberculosisTreat.ContainsKey(treatment.ID))
                    {
                        ado.TuberculosisTreat = dicTuberculosisTreat[treatment.ID];
                    }
                    if (btnExportXML3176 == true)
                    {
                        ado.IS_3176 = true;
                        Inventec.Common.Logging.LogSystem.Debug("IS_3176 = true");
                        //btnExportXML3176 = false;
                    }
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
                            FileStream file = new FileStream(saveFilePathCollinearXml, FileMode.Create, FileAccess.Write);
                            rsXmlTT.WriteTo(file);
                            file.Close();
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
                                FileStream file = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
                                rs.WriteTo(file);
                                file.Close();
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
                            FileStream file12 = new FileStream(saveFilePathXml12, FileMode.Create, FileAccess.Write);
                            rsXml12.WriteTo(file12);
                            file12.Close();
                            rsXml12.Close();
                        }
                    }

                    if (isNotFileSign == false)
                    {
                        // Lấy đường dẫn đến thư mục hiện tại của chương trình
                        string currentDirectory = Directory.GetCurrentDirectory();

                        // Tạo đường dẫn đến thư mục tạm trong thư mục hiện tại
                        string tempFolderPath = Path.Combine(currentDirectory, "Temp");

                        // Tạo thư mục tạm nếu chưa tồn tại
                        Directory.CreateDirectory(tempFolderPath);
                        fullFileName = xmlProcessor.GetFileName();
                        // Tạo đường dẫn đến file tạm 
                        string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                        File.Create(tempFilePath).Close();
                        //FileStream file = new FileStream(saveFilePathCollinearXml, FileMode.Create, FileAccess.Write);

                        WcfSignDCO wcfSignDCO = new WcfSignDCO();
                        wcfSignDCO.SerialNumber = SerialNumber;
                        wcfSignDCO.OutputFile = tempFilePath;
                        wcfSignDCO.PIN = "";
                        if (!string.IsNullOrEmpty(saveFilePathCollinearXml))
                        {
                            wcfSignDCO.SourceFile = saveFilePathCollinearXml;//tempFolderPath;
                        }
                        else
                        {
                            wcfSignDCO.SourceFile = saveFilePath;//tempFolderPath;
                        }
                        wcfSignDCO.fieldSigned = "CHUKYDONVI";
                        string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                        SignProcessorClient signProcessorClient = new SignProcessorClient();
                        var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                        string pathAfterFileSign = "";
                        if (wcfSignResultDCO != null && wcfSignResultDCO.Success)
                        {
                            pathAfterFileSign = wcfSignResultDCO.OutputFile;
                            Inventec.Common.Logging.LogSystem.Debug("wcfSignResultDCO.OutputFile: " + Inventec.Common.Logging.LogUtil.TraceData("output file", wcfSignResultDCO.OutputFile));
                        }

                        if (this.savePathADO == null || string.IsNullOrEmpty(this.savePathADO.pathCollinearXml))
                        {
                            XtraMessageBox.Show("Vui lòng thiết lập thư mục lưu trữ trước khi xuất dữ liệu.", Resources.ResourceMessageLang.ThongBao);
                            btnSavePath_Click(null, null);
                        }
                        if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXml))
                        {
                            string destinationFile = Path.Combine(savePathADO.pathXml, fullFileName);
                            Inventec.Common.Logging.LogSystem.Debug("destinationFile" + destinationFile);
                            //File.Create(destinationFile).Close();

                            if (!string.IsNullOrEmpty(pathAfterFileSign))
                            {
                                //if (File.Exists(destinationFile))
                                //{
                                //    File.Delete(destinationFile);
                                //}
                                if (wcfSignDCO.SourceFile.Trim() != pathAfterFileSign.Trim())
                                {
                                    if (File.Exists(wcfSignDCO.SourceFile))
                                    {
                                        File.Delete(wcfSignDCO.SourceFile);
                                    }
                                }
                                File.Copy(pathAfterFileSign, wcfSignDCO.SourceFile);
                                //File.Copy(pathAfterFileSign, destinationFile);
                            }
                        }

                        //string xmlFilePath = Path.Combine(tempFolderPath, fullFileName);
                        //if (File.Exists(wcfSignDCO.SourceFile))
                        //{
                        //    File.Delete(wcfSignDCO.SourceFile);
                        //}
                        // Xóa tất cả các file trong thư mục temp
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

        private string DataSignXML(string SourceFile, string element)
        {
            string result = null;

            // Lấy đường dẫn đến thư mục hiện tại của chương trình
            string currentDirectory = Directory.GetCurrentDirectory();

            // Tạo đường dẫn đến thư mục tạm trong thư mục hiện tại
            string tempFolderPath = Path.Combine(currentDirectory, "Temp");
            try
            {
                if (VerifyServiceSignProcessorIsRunning() && !string.IsNullOrEmpty(SourceFile))
                {

                    // Tạo thư mục tạm nếu chưa tồn tại
                    Directory.CreateDirectory(tempFolderPath);

                    string fullFileName = Guid.NewGuid().ToString() + ".xml";
                    // Tạo đường dẫn đến file tạm 
                    string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                    File.Create(tempFilePath).Close();

                    var sourceXml = Guid.NewGuid().ToString() + ".xml";
                    // Write the string array to a new file named "xml".
                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(tempFolderPath, sourceXml)))
                    {
                        outputFile.WriteLine(SourceFile);
                    }


                    WcfSignDCO wcfSignDCO = new WcfSignDCO();
                    wcfSignDCO.SerialNumber = SerialNumber;
                    wcfSignDCO.OutputFile = tempFilePath;
                    wcfSignDCO.PIN = "";

                    wcfSignDCO.SourceFile = Path.Combine(tempFolderPath, sourceXml);

                    wcfSignDCO.fieldSigned = element;
                    string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                    SignProcessorClient signProcessorClient = new SignProcessorClient();
                    string pathAfterFileSign = SourceFile;

                    var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                    if (wcfSignResultDCO != null && wcfSignResultDCO.Success)
                    {
                        pathAfterFileSign = wcfSignResultDCO.OutputFile;
                    }
                    result = Encoding.UTF8.GetString(File.ReadAllBytes(pathAfterFileSign));

                    if (configSync != null && !this.configSync.dontSend && string.IsNullOrEmpty(this.configSync.folderPath))
                    {
                        if (File.Exists(wcfSignDCO.SourceFile))
                        {
                            File.Delete(wcfSignDCO.SourceFile);
                        }
                    }
                }
                else
                    return SourceFile;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                foreach (string file in Directory.GetFiles(tempFolderPath))
                {
                    File.Delete(file);
                }
            }
            return result;
        }

        string ProcessExportXmlDetailPlus(ref bool isSuccess, ref MemoryStream memoryStream, bool XuatXml12)
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
                ado.ListMedicalAssessment = ListMedicalAssessment;
                ado.ListHivTreatment = ListHivTreatment;
                ado.vSereServSuin = HisSereServSuin;
                ado.ListTuberculosisTreat = ListTuberculosisTreat;
                ado.TotalMaterialTypeData = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                ado.TotalHeinMediOrgData = BackendDataWorker.Get<HIS_MEDI_ORG>();
                ado.TotalConfigData = NewConfig;
                ado.TotalPatientTypeData = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                ado.TotalIcdData = BackendDataWorker.Get<HIS_ICD>();
                ado.TotalSericeData = BackendDataWorker.Get<V_HIS_SERVICE>();
                ado.TotalEmployeeData = BackendDataWorker.Get<HIS_EMPLOYEE>();
                ado.serverInfo = new ServerInfo() { Username = username, Password = password, Address = address, TypeXml = typeXml, Xml130Api = xml130Api, XmlGdykApi = xmlGdykApi };

                ado.delegateSignXml = DataSignXML;
                His.Bhyt.ExportXml.XML130.CreateXmlProcessor xmlProcessor = new His.Bhyt.ExportXml.XML130.CreateXmlProcessor(ado);

                string errorMess = "";
                string errorMessXml12 = "";
                string fullFileName = "";
                string saveFilePath = "";
                string saveFilePathXml12 = "";


                fullFileName = "DATA_XML_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml";
                saveFilePath = String.Format("{0}/{1}", this.savePathADO.pathXml, fullFileName);
                saveFilePathXml12 = String.Format("{0}/{1}{2}", this.savePathADO.pathXmlGDYK, "XML12_", fullFileName);

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

                if (isNotFileSign == false)
                {

                    // Lấy đường dẫn đến thư mục hiện tại của chương trình
                    string currentDirectory = Directory.GetCurrentDirectory();

                    // Tạo đường dẫn đến thư mục tạm trong thư mục hiện tại
                    string tempFolderPath = Path.Combine(currentDirectory, "Temp");

                    // Tạo thư mục tạm nếu chưa tồn tại
                    Directory.CreateDirectory(tempFolderPath);
                    // Tạo đường dẫn đến file tạm 
                    string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                    File.Create(tempFilePath).Close();
                    //FileStream file = new FileStream(saveFilePathCollinearXml, FileMode.Create, FileAccess.Write);

                    WcfSignDCO wcfSignDCO = new WcfSignDCO();
                    wcfSignDCO.SerialNumber = SerialNumber;
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
                    var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                    string pathAfterFileSign = wcfSignDCO.SourceFile;
                    if (wcfSignResultDCO != null && wcfSignResultDCO.Success)
                    {
                        pathAfterFileSign = wcfSignResultDCO.OutputFile;
                        Inventec.Common.Logging.LogSystem.Debug("wcfSignResultDCO.OutputFile: " + Inventec.Common.Logging.LogUtil.TraceData("output file", wcfSignResultDCO.OutputFile));

                        if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXml))
                        {
                            File.Copy(wcfSignDCO.OutputFile, pathAfterFileSign);
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
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();
                    HisSereServView2Filter ssFilter = new HisSereServView2Filter();
                    ssFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    if (isExportXml)
                    {
                        if (this.patientTypeTTSelecteds != null && this.patientTypeTTSelecteds.Count > 0)
                        {
                            ssFilter.PATIENT_TYPE_IDs = this.patientTypeTTSelecteds.Select(o => o.ID).ToList();
                        }
                    }
                    else
                    {
                        if (this.configSync != null && this.configSync.patientTypeTTIds != null && this.configSync.patientTypeTTIds.Count > 0)
                        {
                            ssFilter.PATIENT_TYPE_IDs = this.configSync.patientTypeTTIds.ToList();
                        }
                    }

                    var resultSS = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_2>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW_2, ApiConsumers.MosConsumer, ssFilter, param);
                    if (resultSS != null && resultSS.Count > 0)
                    {
                        ListSereServ.AddRange(resultSS);

                        var ekipIds = resultSS.Select(o => o.EKIP_ID ?? 0).Where(o => o != 0).Distinct().ToList();
                        if (ekipIds != null && ekipIds.Count > 0)//null sẽ có 1 id bằng 0
                        {
                            int skipEkip = 0;
                            while (ekipIds.Count - skipEkip > 0)
                            {
                                var limitLong = ekipIds.Skip(skipEkip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                                skipEkip = skipEkip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;
                                HisEkipUserFilter ekipFilter = new HisEkipUserFilter();
                                ekipFilter.EKIP_IDs = limitLong;
                                var resultEkip = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EKIP_USER>>("api/HisEkipUser/Get", ApiConsumers.MosConsumer, ekipFilter, param);
                                if (resultEkip != null && resultEkip.Count > 0)
                                {
                                    ListEkipUser.AddRange(resultEkip);
                                }
                            }
                        }

                    }

                    HisBedLogViewFilter bedFilter = new HisBedLogViewFilter();
                    bedFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultBed = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedFilter, param);
                    if (resultBed != null && resultBed.Count > 0)
                    {
                        ListBedlog.AddRange(resultBed);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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
                                Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click Begin");
                                success = this.GenerateXml(ref param, ref memoryStream,ref memoryStreamXml12, true, false, true, listTreatments);
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
                            SerialNumber = item.VALUE;
                            //chkSignFileCertUtil.Checked = !String.IsNullOrWhiteSpace(SerialNumber);
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
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
                if (configSync.isXML3176)
                {
                    isXML130 = false;
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
                        autoSync.Stop();
                        btnAutoSync.Text = Resources.ResourceMessageLang.DongBoTD;
                        btnAutoSync.ToolTip = Resources.ResourceMessageLang.DongBoTuDong;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(SerialNumber))
                    {
                        MessageBox.Show("Không có thông tin Usb Token ký số");
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

                LogSystem.Info("Begin Run Thread Auto Sync");

                if (this.configSync.isCheckCollinearXml)
                {
                    listTreatmentSync = new List<V_HIS_TREATMENT_1>();
                    //lay ho so khoa bhyt
                    this.configSync.isCheckOutTime = true;
                    var listTreatmentLockBHYT = this.GetTreatment();
                    listTreatmentSync.AddRange(listTreatmentLockBHYT);
                    //lay ho so ket thuc dieu tri
                    this.configSync.isCheckOutTime = false;
                    var listTreatmentEnd = this.GetTreatment();
                    listTreatmentSync.AddRange(listTreatmentEnd);
                }
                else
                {
                    listTreatmentSync = this.GetTreatment();
                }

                if (this.configSync.isXML3176)
                {
                    LogSystem.Info("Thread Auto Sync. isXML3176 ");
                    backgroundWorker1.RunWorkerAsync();
                }

                if (listTreatmentSync != null && listTreatmentSync.Count > 0)
                {
                    LogSystem.Info("Thread Auto Sync. TreatmentCount: " + listTreatmentSync.Count);
                    backgroundWorker1.RunWorkerAsync();
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
                    if (configSync.isCheckOutTime)
                    {
                        filter.OUT_TIME_FROM = Inventec.Common.DateTime.Get.StartDay();
                        filter.OUT_TIME_TO = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        filter.IS_PAUSE = true;
                    }
                    else
                    {
                        filter.FEE_LOCK_TIME_FROM = Inventec.Common.DateTime.Get.StartDay();
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

        private async void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                List<Task> lst = new List<Task>();
                lst.Add(ProcessSyncTreatment(listTreatmentSync));
                if (this.configSync.isXML3176 == true)
                {
                    isAutoSignXML3176 = true;
                    showMessSusscess = false;
                    isXML3176 = true;
                    lst.Add(XML130());
                }
                Task.WaitAll(lst.ToArray());
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
                if (btnAutoSyncClick == true && configSync.isXML3176 == true)
                {
                    listSelection = this.GetTreatment();
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
                //isAutoSignXML3176 = false;
                //showMessSusscess = true;
                if (chkSignFileCertUtil.Checked == true)
                {
                    if (string.IsNullOrEmpty(SerialNumber))
                    {
                        MessageBox.Show("Không có thông tin Usb Token ký số");
                        return;
                    }
                    else
                    {
                        WaitingManager.Show();
                        callSyncSuccess = false;
                        isSendCollinearXml = false;
                        await ProcessSyncTreatment(listSelection);
                    }
                }
                else
                {
                    WaitingManager.Show();
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
                        XtraMessageBox.Show(Resources.ResourceMessageLang.XuLyThatBai + String.Join("\r\n", listMessageError), Resources.ResourceMessageLang.ThongBao);
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
                WaitingManager.Show();
                callSyncSuccess = false;
                isSendCollinearXml = true;
                if (chkSignFileCertUtil.Checked == true && string.IsNullOrEmpty(SerialNumber))
                {
                    MessageBox.Show("Không có thông tin Usb Token ký số");
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
                        XtraMessageBox.Show(Resources.ResourceMessageLang.XuLyThatBai + String.Join("\r\n", listMessageError), Resources.ResourceMessageLang.ThongBao);
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
        private async Task ProcessSyncTreatment(List<V_HIS_TREATMENT_1> listTreatmentSync)
        {
            try
            {
                listMessageError = new List<string>();
                string connect_infor = HisConfigCFG.QD_130_BYT__CONNECTION_INFO;
                string username = null, password = null, address = null, typeXml = null;
                string xml130Api = null, xmlGdykApi = null;
                List<string> connectInfors = new List<string>();
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


                Dictionary<string, List<string>> DicErrorMess = new Dictionary<string, List<string>>();
                if (listTreatmentSync != null && listTreatmentSync.Count > 0)
                {
                    listTreatmentSync = listTreatmentSync.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();

                    this.NewConfig = GetNewConfig();
                    int skip = 0;
                    while (listTreatmentSync.Count - skip > 0)
                    {
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
                                if (sereServ.TDL_HEIN_SERVICE_TYPE_ID.HasValue && sereServ.AMOUNT > 0 && sereServ.PRICE > 0 && sereServ.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.TDL_TREATMENT_ID.HasValue)
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
                            ado.TotalConfigData = NewConfig;
                            ado.TotalPatientTypeData = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                            ado.TotalIcdData = BackendDataWorker.Get<HIS_ICD>();
                            ado.TotalSericeData = BackendDataWorker.Get<V_HIS_SERVICE>();
                            ado.TotalEmployeeData = BackendDataWorker.Get<HIS_EMPLOYEE>();
                            ado.serverInfo = new ServerInfo() { Username = username, Password = password, Address = address, TypeXml = typeXml, Xml130Api = xml130Api, XmlGdykApi = xmlGdykApi };
                            ado.delegateSignXml = DataSignXML;

                            if (dicTuberculosisTreat.ContainsKey(treatment.ID))
                            {
                                ado.TuberculosisTreat = dicTuberculosisTreat[treatment.ID];
                            }
                            if (isXML130 == false)
                            {
                                ado.IS_3176 = true;
                                Inventec.Common.Logging.LogSystem.Debug("IS_3176 = true");
                            }
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

                            if (configSync != null && !this.configSync.dontSend)
                            {

                                if (sendXml12)
                                {


                                    string fullFileName = xmlProcessor.GetFileName();

                                    if ((isAutoSync && configSync != null && configSync.isCheckCollinearXml) || (isSendCollinearXml))
                                    {
                                        resultSyncTT = xmlProcessor.RunCollinearXml(ref errorMess);
                                        Task task = null;
                                        List<Task> lstTask = new List<Task>();
                                        if (resultSyncTT != null)
                                        {
                                            saveFilePathXmlTT = String.Format("{0}/{1}{2}", this.configSync.folderPath, "XMLTT_", fullFileName);
                                            FileStream file12 = new FileStream(saveFilePathXmlTT, FileMode.Create, FileAccess.Write);
                                            resultSyncTT.WriteTo(file12);
                                            file12.Close();
                                            resultSyncTT.Close();
                                            Inventec.Common.Logging.LogSystem.Debug("__Luu XMlTT vao client folder thanh cong. path: " + saveFilePathXmlTT);
                                        }
                                        if (isNotFileSign == false)
                                        {
                                            sendXMLSign(xmlProcessor, saveFilePathXml, ref syncResult);
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
                                            saveFilePathXml = String.Format("{0}/{1}{2}", this.configSync.folderPath, "XML", fullFileName);
                                            FileStream file12 = new FileStream(saveFilePathXml, FileMode.Create, FileAccess.Write);
                                            resultSync.WriteTo(file12);
                                            file12.Close();
                                            resultSync.Close();
                                            Inventec.Common.Logging.LogSystem.Debug("__Luu XMl vao client folder thanh cong. path: " + saveFilePathXml);
                                        }
                                        if (isNotFileSign == false)
                                        {
                                            sendXMLSign(xmlProcessor, saveFilePathXml, ref syncResult);
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
                                        if (errorCode == "01" || errorCode == "02" || errorCode == "03")
                                        {
                                            XtraMessageBox.Show(String.Format("{0} - {1}", errorCode, syncResult.Message), Resources.ResourceMessageLang.ThongBao);
                                            autoSync.Stop();
                                            isAutoSync = false;
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
                                    resultSync = treatment.HEIN_LOCK_TIME != null ? xmlProcessor.Run(ref errorMess) : xmlProcessor.RunCollinearXml(ref errorMess);

                                    //luu file
                                    if (resultSync != null)
                                    {
                                        string fullFileName = xmlProcessor.GetFileName();
                                        saveFilePathXml = String.Format("{0}/{1}{2}", this.configSync.folderPath, "XML", fullFileName);
                                        FileStream file12 = new FileStream(saveFilePathXml, FileMode.Create, FileAccess.Write);
                                        resultSync.WriteTo(file12);
                                        file12.Close();
                                        resultSync.Close();
                                        Inventec.Common.Logging.LogSystem.Debug("__Luu XMl vao client folder thanh cong. path: " + saveFilePathXml);

                                    }
                                    if (isNotFileSign == false)
                                    {
                                        sendXMLSign(xmlProcessor, saveFilePathXml, ref syncResult);
                                    }
                                    else
                                    {
                                        syncResult = treatment.HEIN_LOCK_TIME != null ? await xmlProcessor.SyncData() : await xmlProcessor.SyncDataCollinear();
                                    }

                                    //if ((isAutoSync && configSync != null && configSync.isCheckCollinearXml) || (isSendCollinearXml))
                                    //    syncResult = await xmlProcessor.SyncDataCollinear();
                                    //else
                                    //    syncResult = await xmlProcessor.SyncData();
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("syncResult__" + Inventec.Common.Logging.LogUtil.GetMemberName(() => syncResult), syncResult));
                                    if (syncResult != null)
                                    {


                                        string errorCode = syncResult.ErrorCode;
                                        if (errorCode == "01" || errorCode == "02" || errorCode == "03")
                                        {
                                            XtraMessageBox.Show(String.Format("{0} - {1}", errorCode, syncResult.Message), Resources.ResourceMessageLang.ThongBao);
                                            autoSync.Stop();
                                            isAutoSync = false;
                                            return;
                                        }
                                        else
                                        {
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
                                try
                                {
                                    resultSync = xmlProcessor.RunCollinearXml(ref errorMess);
                                    if (string.IsNullOrEmpty(errMessage)) success = true;
                                }
                                catch (Exception error)
                                {
                                    success = false;
                                    errorMess = error.Message;
                                }
                                if (resultSync != null)
                                {
                                    HisTreatmentXmlResultSDO xmlResultSDO = new HisTreatmentXmlResultSDO();
                                    xmlResultSDO.TreatmentId = treatment.ID;
                                    xmlResultSDO.XmlResult = success ? 2 : 1;
                                    xmlResultSDO.Description = errMessage;
                                    //xmlResultSDO.CheckCode = syncResult.CheckCode;
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => xmlResultSDO), xmlResultSDO));
                                    var rs = new Inventec.Common.Adapter.BackendAdapter(paramUpdateXml130).Post<bool>("api/HisTreatment/UpdateXml130Info", ApiConsumers.MosConsumer, xmlResultSDO, paramUpdateXml130);
                                    //luu file
                                    if (this.configSync != null && !string.IsNullOrEmpty(this.configSync.folderPath))
                                    {
                                        string fullFileName = xmlProcessor.GetFileName();
                                        saveFilePathXml = String.Format("{0}/{1}{2}", this.configSync.folderPath, "XML", fullFileName);
                                        FileStream file12 = new FileStream(saveFilePathXml, FileMode.Create, FileAccess.Write);
                                        resultSync.WriteTo(file12);
                                        file12.Close();
                                        resultSync.Close();
                                        success = true;
                                        Inventec.Common.Logging.LogSystem.Debug("__Luu XMl vao client folder thanh cong. path: " + saveFilePathXml);
                                        if (isNotFileSign == false)
                                        {
                                            sendXMLSign(xmlProcessor, saveFilePathXml, ref syncResult);
                                        }
                                    }
                                }

                            }
                            count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void sendXMLSign(His.Bhyt.ExportXml.XML130.CreateXmlProcessor xmlProcessor, string sourceFile, ref SyncResultADO syncResult)
        {
            try
            {

                // Lấy đường dẫn đến thư mục hiện tại của chương trình
                string currentDirectory = Directory.GetCurrentDirectory();

                // Tạo đường dẫn đến thư mục tạm trong thư mục hiện tại
                string tempFolderPath = Path.Combine(currentDirectory, "Temp");

                // Tạo thư mục tạm nếu chưa tồn tại
                Directory.CreateDirectory(tempFolderPath);

                string fullFileName = xmlProcessor.GetFileName();
                // Tạo đường dẫn đến file tạm 
                string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                File.Create(tempFilePath).Close();

                WcfSignDCO wcfSignDCO = new WcfSignDCO();
                wcfSignDCO.SerialNumber = SerialNumber;
                wcfSignDCO.OutputFile = tempFilePath;
                wcfSignDCO.PIN = "";

                wcfSignDCO.SourceFile = sourceFile;

                wcfSignDCO.fieldSigned = "CHUKYDONVI";
                string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                SignProcessorClient signProcessorClient = new SignProcessorClient();
                string pathAfterFileSign = sourceFile;
                if (VerifyServiceSignProcessorIsRunning())
                {
                    var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                    if (wcfSignResultDCO != null && wcfSignResultDCO.Success)
                    {
                        pathAfterFileSign = wcfSignResultDCO.OutputFile;
                    }
                }

                if (configSync != null && !this.configSync.dontSend)
                {
                    //gọi api đẩy cổng ...
                    //...
                    SyncResultADO syncResultADO = new SyncResultADO();
                    Task task = Task.Run(async () => syncResultADO = await xmlProcessor.SendFileSign(pathAfterFileSign));
                    task.Wait();
                    syncResult = syncResultADO;
                }
                if (this.configSync != null && !string.IsNullOrEmpty(this.configSync.folderPath))
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

                foreach (string file in Directory.GetFiles(tempFolderPath))
                {
                    File.Delete(file);
                }
                if (configSync != null && !this.configSync.dontSend && string.IsNullOrEmpty(this.configSync.folderPath))
                {
                    if (File.Exists(wcfSignDCO.SourceFile))
                    {
                        File.Delete(wcfSignDCO.SourceFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                            ado.Treatment = treatment;
                            if (dicMedicalAssessment.ContainsKey(treatment.ID))
                            {
                                ado.ListMedicalAssessment = dicMedicalAssessment[treatment.ID];
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
                        success = this.GenerateXml(ref param, ref memoryStream, false, false, true, listSelection);
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

        private void btnExportCollinearXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportCollinearXml.Enabled || listSelection == null || listSelection.Count == 0) return;
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
                    success = this.GenerateXml(ref param, ref memoryStream, false, true, true, listSelection);
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

        private void btnExportGroupXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportGroupXml.Enabled || listSelection == null || listSelection.Count == 0) return;
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

                    if (chkSignFileCertUtil.Checked == false)
                    {
                        WaitingManager.Show();
                        isNotFileSign = true;
                        success = this.GenerateXmlPlus(ref param, ref memoryStream, xuatXml12, listSelection);
                        WaitingManager.Hide();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(SerialNumber))
                        {
                            if (XtraMessageBox.Show("Không có thông tin Usb Token ký số. Bạn có muốn tiếp tục xuất xml?", Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                return;
                            }
                            else
                            {
                                isNotFileSign = true;
                                WaitingManager.Show();
                                success = this.GenerateXmlPlus(ref param, ref memoryStream, xuatXml12, listSelection);
                                WaitingManager.Hide();
                            }
                        }
                        else
                        {
                            isNotFileSign = false;
                            WaitingManager.Show();
                            success = this.GenerateXmlPlus(ref param, ref memoryStream, xuatXml12, listSelection);
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
                CreateThread();
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
                    if (VerifyServiceSignProcessorIsRunning())
                    {
                        if (chooseChungThu && string.IsNullOrEmpty(SerialNumber))
                        {
                            WcfSignDCO wcfSignDCO = new WcfSignDCO();
                            wcfSignDCO.HwndParent = this.ParentForm.Handle;
                            string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                            SignProcessorClient signProcessorClient = new SignProcessorClient();
                            var wcfSignResultDCO = signProcessorClient.GetSerialNumber(jsonData);  //EDIT
                            if (wcfSignResultDCO != null)
                            {
                                SerialNumber = wcfSignResultDCO.OutputFile;
                            }
                            chooseChungThu = false;
                        }
                    }

                    if (!string.IsNullOrEmpty(SerialNumber))
                    {
                        HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignFileCertUtil.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault() : null;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                        if (csAddOrUpdate != null)
                        {
                            csAddOrUpdate.VALUE = SerialNumber;
                        }
                        else
                        {
                            csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                            csAddOrUpdate.KEY = chkSignFileCertUtil.Name;
                            csAddOrUpdate.VALUE = SerialNumber;
                            csAddOrUpdate.MODULE_LINK = this.currentModule.ModuleLink;
                            if (this.currentControlStateRDO == null)
                                this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                            this.currentControlStateRDO.Add(csAddOrUpdate);
                        }
                        this.controlStateWorker.SetData(this.currentControlStateRDO);
                    }
                    //else
                    //{
                    //    chkSignFileCertUtil.Checked = false;
                    //    XtraMessageBox.Show("Không lấy được chứng thư hoặc chứng thư không hợp lệ", Resources.ResourceMessageLang.ThongBao);
                    //}
                }
                else
                {
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignFileCertUtil.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault() : null;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = "";
                    }
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                    chooseChungThu = true;
                    SerialNumber = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        System.Windows.Forms.Timer timerCert = new System.Windows.Forms.Timer();
        private void CreateThread()
        {

            try
            {
                timerCert.Stop();
                timerCert.Interval = 100;
                timerCert.Tick += timerCert_Tick;
                timerCert.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void timerCert_Tick(object sender, EventArgs e)
        {
            timerCert.Stop();
            isChkSignFileCertUtil();
        }

        private async void btnXML3176_Click(object sender, EventArgs e)
        {

            try
            {
                btnExportXML3176 = true;
                if (!btnXML3176.Enabled || listSelection == null || listSelection.Count == 0) return;
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
                    Inventec.Common.Logging.LogSystem.Info("btnXML3176_Click Begin");
                    success = this.GenerateXml(ref param, ref memoryStream, false, false, xuatXml12, listSelection);
                    btnExportXML3176 = false;
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
    }
}
