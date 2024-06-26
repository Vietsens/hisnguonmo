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
using His.Bhyt.ExportXml.Base;
using HIS.Desktop.Plugins.ExportXmlQD4210.Base;
using Inventec.Common.LocalStorage.SdaConfig;
using HIS.Desktop.Plugins.ExportXmlQD4210.Config;
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
using HIS.Desktop.Plugins.ExportXmlQD4210.ADO;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.Utils.Menu;
using DevExpress.Utils;
using Inventec.Fss.Client;
using Newtonsoft.Json;
using DevExpress.XtraBars;

namespace HIS.Desktop.Plugins.ExportXmlQD4210
{
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        bool timerTickIsRunning = false;
        bool isInit = true;



        SereServTreeProcessor ssTreeProcessor;
        UserControl ucSereServTree;

        List<V_HIS_TREATMENT_1> listTreatment1 = new List<V_HIS_TREATMENT_1>();
        List<V_HIS_TREATMENT_1> listSelection = new List<V_HIS_TREATMENT_1>();
        List<V_HIS_TREATMENT_1> listSelectionExported__XML4210 = new List<V_HIS_TREATMENT_1>();
        List<V_HIS_TREATMENT_1> listSelectionExported__XML4210Colinear = new List<V_HIS_TREATMENT_1>();

        HIS_BRANCH _Branch = null;

        List<V_HIS_SERE_SERV_TEIN> hisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
        List<V_HIS_SERE_SERV_PTTT> hisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();
        List<HIS_DHST> listDhst = new List<HIS_DHST>();
        List<HIS_TRACKING> hisTrackings = new List<HIS_TRACKING>();
        List<V_HIS_TREATMENT_3> hisTreatments = new List<V_HIS_TREATMENT_3>();
        List<V_HIS_SERE_SERV_2> ListSereServ = new List<V_HIS_SERE_SERV_2>();
        List<HIS_EKIP_USER> ListEkipUser = new List<HIS_EKIP_USER>();
        List<V_HIS_BED_LOG> ListBedlog = new List<V_HIS_BED_LOG>();
        List<V_HIS_HEIN_APPROVAL> listHeinApproval = new List<V_HIS_HEIN_APPROVAL>();
        List<HIS_DEBATE> ListDebates = new List<HIS_DEBATE>();
        List<TreatmentImportADO> listTreatmentImport;
        List<HIS_TREATMENT> listTreatmentDynamic { get; set; }

        internal string filterType__IN = "Trong DS đầu thẻ BHYT sau:";
        internal string filterType__OUT = "Ngoài DS đầu thẻ BHYT sau:";
        internal string filterType__FeeLockTime = "Thời gian khóa viện phí từ:";
        internal string filterType__EndTreatmentTime = "Thời gian kết thúc điều trị từ:";
        internal string filterType__BiginTreatmentTime = "Thời gian vào viện từ:";
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;

        BarManager barManager = null;
        PopupMenuProcessor popupMenuProcessor = null;

        List<HIS_BRANCH> branchSelecteds;
        string SavePath;
        bool isNotLoadWhileChangeControlStateInFirst;
        bool isNotLoadWhilecheckAutoDownFileXmlStateInFirst = true;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorkerAutoDownFileXml;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDOAutoDownFileXml;
        string moduleLink = "HIS.Desktop.Plugins.ExportXmlQD4210";

        string subFolder = "AutoLoadXml4210";
        string registryPathSave = "PATH_SAVE";
        string registryInterval = "INTERVAL_TIME";

        private string[] icdSeparators = new string[] { ";" };

        List<HIS_CONFIG> NewConfig;

        public UCExportXml(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = moduleData;
                this.InitSereServTree();
                HisConfigCFG.LoadConfig();
                Base.ResourceLangManager.InitResourceLanguageManager();
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
                System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ExportXmlQD4210.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());

                ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowSearchPanel = false;
                ado.IsCreateParentNodeWithSereServExpend = false;
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                SereServTreeColumn serviceNameCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("UCExportXml.tenDv", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_NAME", 150, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);

                SereServTreeColumn amountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("UCExportXml.soLuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AMOUNT_PLUS", 40, false);
                amountCol.VisibleIndex = 1;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(amountCol);

                SereServTreeColumn virPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("UCExportXml.donGia", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_PRICE", 80, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.Format.FormatString = "#,##0.0000";
                virPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virPriceCol);

                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("UCExportXml.thanhTien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PRICE", 90, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPriceCol);

                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("UCExportXml.dongChiTra", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_HEIN_PRICE", 90, false);
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.Format.FormatString = "#,##0.0000";
                virTotalHeinPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("UCExportXml.benhNhanTra", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PATIENT_PRICE", 110, false);
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPatientPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);

                SereServTreeColumn virDiscountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("UCExportXml.chietKhau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "DISCOUNT", 90, false);
                virDiscountCol.VisibleIndex = 6;
                virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                virDiscountCol.Format.FormatString = "#,##0.0000";
                virDiscountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virDiscountCol);

                SereServTreeColumn virIsExpendCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("UCExportXml.haoPhi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IsExpend", 60, false);
                virIsExpendCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(virIsExpendCol);

                SereServTreeColumn virVatRatioCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("UCExportXml.vat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VAT", 100, false);
                virVatRatioCol.VisibleIndex = 8;
                virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                virVatRatioCol.Format.FormatString = "#,##0.00";
                virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                SereServTreeColumn serviceCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("UCExportXml.maDichVu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_CODE", 100, false);
                serviceCodeCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("UCExportXml.maYeuCau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_REQ_CODE", 100, false);
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
                this.InitComboStatus();
                this.AddFilterTreatmentTypeItem();
                this.SetDefaultValueControl();
                this.SetDefaultValueAutoControl();
                this.InitControlState();
                this.FillDataToGridTreatment();
                this.LoadDataEmployeestoXML();
                this.InitBranchCheck();
                this.InitComboBranch();
                this.InitControlStateAutoDownFileXml();
                this.SetDefaultValueAutoControl();
                this.isInit = false;
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
                System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ExportXmlQD4210.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControl1.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnExportGroup.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnExportGroup.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPath.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.btnPath.ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExportXml.txtPatientCode.Properties.NullValuePrompt", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.BtnExportExcel.Text = Inventec.Common.Resource.Get.Value("UCExportXml.BtnExportExcel.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.BtnExportExcel.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.BtnExportExcel.ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnExportXMLcollinear.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnExportXMLcollinear.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.cboFilterType.Text = Inventec.Common.Resource.Get.Value("UCExportXml.cboFilterType.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.cboStatusFeeLockOrEndTreatment.Text = Inventec.Common.Resource.Get.Value("UCExportXml.cboStatusFeeLockOrEndTreatment.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("UCExportXml.labelControl1.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExportXml.txtKeyword.Properties.NullValuePrompt", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExportXml.cboStatus.Properties.NullText", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtTreatCodeOrHeinCard.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExportXml.txtTreatCodeOrHeinCard.Properties.NullValuePrompt", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.checkAutoDownFileXml.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.checkAutoDownFileXml.Properties.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnDownload.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnDownload.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnImport.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnImport.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.btnImport.ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.CboBranch.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExportXml.CboBranch.Properties.NullText", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnExportXml.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnExportXml.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridCol_ViewXML.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_ViewXML.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn1.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridCol_Treatment_Stt.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_Stt.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridCol_Treatment_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_TreatmentCode.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_PatientCode.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn_Treatment_PatientCode.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridCol_Treatment_VirPatientName.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_VirPatientName.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridCol_Treatment_Gender.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_Gender.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridCol_Treatment_Dob.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_Dob.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridCol_Treatment_HeinCardNumber.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_HeinCardNumber.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_EndDepartment.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn_Treatment_EndDepartment.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn3.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridCol_Treatment_InTime.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_InTime.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridCol_Clinical_InTime.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Clinical_InTime.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridCol_Treatment_OutTime.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_OutTime.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridCol_Treatment_FeeLockTime.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridCol_Treatment_FeeLockTime.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_TotalPrice.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn_Treatment_TotalPrice.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_TotalHeinPrice.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn_Treatment_TotalHeinPrice.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_TotalPatientPrice.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn_Treatment_TotalPatientPrice.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_XmlDesc.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn_Treatment_XmlDesc.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCExportXml.gridColumn2.Caption", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCExportXml.btnFind.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtHeinCardPrefix.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExportXml.txtHeinCardPrefix.Properties.NullValuePrompt", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboFilterTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExportXml.cboFilterTreatmentType.Properties.NullText", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCExportXml.lciTimeFrom.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTimeTo.Text = Inventec.Common.Resource.Get.Value("UCExportXml.lciTimeTo.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciFolderSave.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.lciFolderSave.OptionsToolTip.ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciFolderSave.Text = Inventec.Common.Resource.Get.Value("UCExportXml.lciFolderSave.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciIntervalTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExportXml.lciIntervalTime.OptionsToolTip.ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciIntervalTime.Text = Inventec.Common.Resource.Get.Value("UCExportXml.lciIntervalTime.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.LciCboBranch.Text = Inventec.Common.Resource.Get.Value("UCExportXml.LciCboBranch.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem2.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem13.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem9.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("UCExportXml.layoutControlItem20.Text", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public void InitComboStatus()
        {
            try
            {
                List<FilterTypeADO> ListStatusAll = new List<FilterTypeADO>();

                FilterTypeADO all = new FilterTypeADO(0, "Tất cả");
                ListStatusAll.Add(all);

                FilterTypeADO duyetBhyt = new FilterTypeADO(1, "Đã duyệt BHYT");
                ListStatusAll.Add(duyetBhyt);

                FilterTypeADO ketthuc = new FilterTypeADO(2, "Đã kết thúc điều trị");
                ListStatusAll.Add(ketthuc);

                FilterTypeADO dacosovaovien = new FilterTypeADO(3, "Đã có số vào viện");
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

        private void SetDefaultValueAutoControl()
        {
            try
            {
                if (HisConfigCFG.IsAutoExportXml)
                {
                    ReadFromRegistry();
                }
                else
                {
                    checkAutoDownFileXml.Visible = false;
                    lciAutoDownFileXml.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    txtFolderSave.Visible = false;
                    lciFolderSave.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    spinIntervalTime.Visible = false;
                    lciIntervalTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtSavePath.Text = this.SavePath;
                dtTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                dtTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;
                cboStatus.EditValue = 1;
                txtHeinCardPrefix.Text = "";
                txtTreatCodeOrHeinCard.Text = "";
                cboFilterTreatmentType.EditValue = 0;
                this._Branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());
                this.BtnExportExcel.Enabled = false;
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
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                this.txtHeinCardPrefix.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML4210__TXT_KEYWORD__NULL_VALUE", Base.ResourceLangManager.LanguageUCExportXmlQD4210, cultureLang);

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


                DXMenuItem itemBiginTreatmentTime = new DXMenuItem(filterType__BiginTreatmentTime, new EventHandler(btnFeeLockOrEndTreatment_Click));
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
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == cboStatusFeeLockOrEndTreatment.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = cboStatusFeeLockOrEndTreatment.Text;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = cboStatusFeeLockOrEndTreatment.Name;
                    csAddOrUpdate.VALUE = cboStatusFeeLockOrEndTreatment.Text;
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

        private void AddFilterTreatmentTypeItem()
        {
            try
            {
                List<HIS_TREATMENT_TYPE> filterTreatmentType = new List<HIS_TREATMENT_TYPE>();
                HIS_TREATMENT_TYPE all = new HIS_TREATMENT_TYPE();
                all.TREATMENT_TYPE_NAME = "Tất cả";
                filterTreatmentType.Add(all);

                var hisTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>();
                filterTreatmentType.AddRange(hisTreatmentType);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboFilterTreatmentType, filterTreatmentType, controlEditorADO);
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
                BtnExportExcel.Enabled = false;
                gridControlTreatment.DataSource = null;
                btnExportXml.Enabled = false;
                btnExportXMLcollinear.Enabled = false;
                btnExportGroup.Enabled = false;
                FillDataToSereServTreeByTreatment(null);

                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HisTreatmentView1Filter filter = new HisTreatmentView1Filter();
                filter.ORDER_DIRECTION = "ACS";
                filter.ORDER_FIELD = "FEE_LOCK_TIME";

                if (this.branchSelecteds != null && this.branchSelecteds.Count > 0)
                {
                    filter.BRANCH_IDs = this.branchSelecteds.Select(o => o.ID).ToList();
                }
                else
                    filter.BRANCH_ID = (this._Branch != null) ? this._Branch.ID : 0;

                filter.HAS_PATY_ALTER_BHYT = true;

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
                    if (!String.IsNullOrWhiteSpace(txtKeyword.Text))
                    {
                        filter.KEY_WORD = txtKeyword.Text.Trim();
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

                    if (cboFilterTreatmentType.EditValue != null && cboFilterTreatmentType.EditValue.ToString() != "0")
                    {
                        filter.TDL_TREATMENT_TYPE_ID = (long)cboFilterTreatmentType.EditValue;
                    }

                    if (cboStatusFeeLockOrEndTreatment.Text == this.filterType__FeeLockTime)
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
                    else if (cboStatusFeeLockOrEndTreatment.Text == this.filterType__EndTreatmentTime)
                    {
                        if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                        {
                            filter.OUT_TIME_FROM = Convert.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                        }
                        if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                        {
                            filter.OUT_TIME_TO = Convert.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");
                        }

                    }
                    else if (cboStatusFeeLockOrEndTreatment.Text == filterType__BiginTreatmentTime)
                    {

                        if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                        {
                            filter.REQUEST_HOSPITALIZE_TIME_FROM = Convert.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                        }
                        if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                        {
                            filter.REQUEST_HOSPITALIZE_TIME_TO = Convert.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");
                        }
                    }

                    if (cboStatus.EditValue != null && (int)cboStatus.EditValue == 0) // tat ca
                    {
                        filter.HAS_HEIN_APPROVAL_OR_IS_PAUSE = true;
                    }
                    else if (cboStatus.EditValue != null && (int)cboStatus.EditValue == 1)// duyet bh
                    {
                        filter.HAS_HEIN_APPROVAL = true;
                    }
                    else if (cboStatus.EditValue != null && (int)cboStatus.EditValue == 2)
                    {
                        filter.IS_PAUSE = true;
                    }
                    else if (cboStatus.EditValue != null && (int)cboStatus.EditValue == 3)// đã có
                    {
                        filter.HAS_IN_CODE = true;
                    }

                    if (cboExportXmlStatus.EditValue != null)
                    {
                        if (cboExportXmlStatus.SelectedIndex == 0) // Tự động xuất file thành công
                        {
                            filter.XML4210_RESULT = IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__CREATE_FILE_SUCCESS;
                        }
                        else if (cboExportXmlStatus.SelectedIndex == 1) //  Tự động xuất file thất bại
                        {
                            filter.XML4210_RESULT = IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__CREATE_FILE_FAIL;
                        }
                        else if (cboExportXmlStatus.SelectedIndex == 2) //  Tải file về mày trạm thành công
                        {
                            filter.XML4210_RESULT = IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__DOWN_FILE_SUCCESS;
                        }
                        else if (cboExportXmlStatus.SelectedIndex == 3) // Tỉa file về máy trạm thất bại
                        {
                            filter.XML4210_RESULT = IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__DOWN_FILE_FAIL;
                        }

                        Inventec.Common.Logging.LogSystem.Info("cboExportXmlStatus.SelectedIndex " + cboExportXmlStatus.SelectedIndex);
                    }
                }
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
                    if (listSereServ != null)
                    {
                        listSereServ = listSereServ.Where(o => o.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList();
                    }
                }

                this.ssTreeProcessor.Reload(ucSereServTree, listSereServ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataEmployeestoXML()
        {
            try
            {
                CommonParam paramGet = new CommonParam();
                MOS.Filter.HisEmployeeFilter filter = new HisEmployeeFilter();
                His.Bhyt.ExportXml.Base.GlobalConfigStore.ListEmployees = new Inventec.Common.Adapter.BackendAdapter(paramGet).Get<List<HIS_EMPLOYEE>>("/api/HisEmployee/Get", ApiConsumers.MosConsumer, filter, paramGet);
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
                    cboStatus.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioFeeLockTime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtTreatCodeOrHeinCard.Focus();
                txtTreatCodeOrHeinCard.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioOutTime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtTreatCodeOrHeinCard.Focus();
                txtTreatCodeOrHeinCard.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioOutTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTreatCodeOrHeinCard.Focus();
                    txtTreatCodeOrHeinCard.SelectAll();
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
                        txtHeinCardPrefix.Focus();
                        txtHeinCardPrefix.SelectAll();
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
                        e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize;
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
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CLINICAL_IN_TIME.Value);
                    }
                    else if (e.Column.FieldName == "OUT_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "FEE_LOCK_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FEE_LOCK_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "XML_DESC")
                    {
                        if (data.XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__CREATE_FILE_SUCCESS)
                        {
                            e.Value = ResourceMessageLang.TuDongXuatFileThanhCong;
                        }
                        else if (data.XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__CREATE_FILE_FAIL)
                        {
                            e.Value = String.Format("{0}({1})", ResourceMessageLang.TuDongXuatFileThatBai, data.XML4210_DESC);
                        }
                        else if (data.XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__DOWN_FILE_SUCCESS)
                        {
                            e.Value = ResourceMessageLang.TaiFileVeMayTramThanhCong;
                        }
                        else if (data.XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__DOWN_FILE_FAIL)
                        {
                            e.Value = ResourceMessageLang.TaiFileVeMayTramThatBai;
                        }
                    }
                    else if (e.Column.FieldName == "XM_COLLINEAR_DESC")
                    {
                        if (data.COLLINEAR_XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.COLLINEAR_XML4210_RESULT__CREATE_FILE_SUCCESS)
                        {
                            e.Value = "Tạo file thành công";
                        }
                        else if (data.COLLINEAR_XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.COLLINEAR_XML4210_RESULT__CREATE_FILE_FAIL)
                        {
                            e.Value = String.Format("{0}({1})", ResourceMessageLang.TuDongXuatFileThatBai, data.COLLINEAR_XML4210_DESC);
                        }
                        else if (data.COLLINEAR_XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.COLLINEAR_XML4210_RESULT__DOWN_FILE_SUCCESS)
                        {
                            e.Value = ResourceMessageLang.TaiFileVeMayTramThanhCong;
                        }
                        else if (data.COLLINEAR_XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.COLLINEAR_XML4210_RESULT__DOWN_FILE_FAIL)
                        {
                            e.Value = ResourceMessageLang.TaiFileVeMayTramThatBai;
                        }
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
                    btnExportXMLcollinear.Enabled = true;
                    BtnExportExcel.Enabled = true;
                    btnExportGroup.Enabled = true;
                }
                else
                {
                    btnExportXml.Enabled = false;
                    btnExportXMLcollinear.Enabled = false;
                    BtnExportExcel.Enabled = false;
                    btnExportGroup.Enabled = false;
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
                bool success = false;

                if (string.IsNullOrEmpty(this.SavePath))
                {
                    btnPath_Click(null, null);
                }
                if (!string.IsNullOrEmpty(this.SavePath))
                {
                    listSelection.ForEach(o => o.XML4210_URL = "");
                    WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click Begin");
                    success = this.GenerateXml(ref param, listSelection, this.SavePath, false);
                    Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click End");
                    WaitingManager.Hide();
                    if (success && param.Messages.Count == 0)
                    {
                        listSelectionExported__XML4210 = new List<V_HIS_TREATMENT_1>();
                        listSelectionExported__XML4210.AddRange(listSelection);
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }

                    this.gridControlTreatment.RefreshDataSource();

                    List<string> treatmentCodes = listSelection.Where(o => String.IsNullOrWhiteSpace(o.XML4210_URL)).Select(s => s.TREATMENT_CODE).ToList();
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool GenerateXml(ref CommonParam paramExport, List<V_HIS_TREATMENT_1> listSelection, string pathSave, bool isCollinear)
        {
            bool result = false;
            try
            {
                if (listSelection.Count > 0)
                {
                    listSelection = listSelection.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();
                    if (String.IsNullOrEmpty(pathSave))
                    {
                        pathSave = ConfigStore.GetFolderSaveXml + "\\ExportXmlPlus\\Xml" + DateTime.Now.ToString("ddMMyyyy");
                        var dicInfo = System.IO.Directory.CreateDirectory(pathSave);
                        if (dicInfo == null)
                        {
                            paramExport.Messages.Add(Base.ResourceMessageLang.KhongTaoDuocFolderLuuXml);
                            return result;
                        }
                    }

                    if (!GlobalConfigStore.IsInit && !this.SetDataToLocalXml(pathSave))
                    {
                        paramExport.Messages.Add(Base.ResourceMessageLang.KhongThieLapDuocCauHinhDuLieuXuatXml);
                        return result;
                    }

                    GlobalConfigStore.PathSaveXml = pathSave;

                    //get cấu hình mới trước khi xuất xml.
                    //get 1 lần trước khi xuất danh sách
                    this.NewConfig = GetNewConfig();

                    int skip = 0;
                    while (listSelection.Count - skip > 0)
                    {
                        var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                        listHeinApproval = new List<V_HIS_HEIN_APPROVAL>();
                        ListSereServ = new List<V_HIS_SERE_SERV_2>();
                        ListEkipUser = new List<HIS_EKIP_USER>();
                        ListBedlog = new List<V_HIS_BED_LOG>();
                        hisTreatments = new List<V_HIS_TREATMENT_3>();
                        listDhst = new List<HIS_DHST>();
                        hisTrackings = new List<HIS_TRACKING>();
                        hisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                        hisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();
                        ListDebates = new List<HIS_DEBATE>();

                        string message = "";
                        CreateThreadGetData(limit);
                        message = ProcessExportXmlDetail(ref result, hisTreatments, listHeinApproval, ListSereServ, listDhst, hisSereServTeins, hisTrackings, hisSereServPttts, ListEkipUser, ListBedlog, isCollinear, ListDebates);
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

        string ProcessExportXmlDetail(ref bool isSuccess, List<V_HIS_TREATMENT_3> hisTreatments, List<V_HIS_HEIN_APPROVAL> hisHeinApprvals,
            List<V_HIS_SERE_SERV_2> ListSereServ, List<HIS_DHST> listDhst, List<V_HIS_SERE_SERV_TEIN> listSereServTein,
            List<HIS_TRACKING> hisTrackings, List<V_HIS_SERE_SERV_PTTT> hisSereServPttts, List<HIS_EKIP_USER> ListEkipUser,
            List<V_HIS_BED_LOG> ListBedlog, bool isCollinear, List<HIS_DEBATE> listDebate)
        {
            string result = "";
            Dictionary<string, List<string>> DicErrorMess = new Dictionary<string, List<string>>();
            try
            {
                Dictionary<long, List<V_HIS_HEIN_APPROVAL>> dicHeinApproval = new Dictionary<long, List<V_HIS_HEIN_APPROVAL>>();
                Dictionary<long, List<V_HIS_SERE_SERV_2>> dicSereServ = new Dictionary<long, List<V_HIS_SERE_SERV_2>>();
                Dictionary<long, List<V_HIS_SERE_SERV_TEIN>> dicSereServTein = new Dictionary<long, List<V_HIS_SERE_SERV_TEIN>>();
                Dictionary<long, HIS_DHST> dicDhst = new Dictionary<long, HIS_DHST>();
                Dictionary<long, List<HIS_TRACKING>> dicTracking = new Dictionary<long, List<HIS_TRACKING>>();
                Dictionary<long, List<V_HIS_SERE_SERV_PTTT>> dicSereServPttt = new Dictionary<long, List<V_HIS_SERE_SERV_PTTT>>();
                Dictionary<long, List<HIS_EKIP_USER>> dicEkipUser = new Dictionary<long, List<HIS_EKIP_USER>>();
                Dictionary<long, List<V_HIS_BED_LOG>> dicBedLog = new Dictionary<long, List<V_HIS_BED_LOG>>();
                Dictionary<long, List<HIS_DEBATE>> dicDebate = new Dictionary<long, List<HIS_DEBATE>>();
                Dictionary<long, List<HIS_DHST>> dicDhstList = new Dictionary<long, List<HIS_DHST>>();

                if (hisHeinApprvals != null && hisHeinApprvals.Count > 0)
                {
                    foreach (var item in hisHeinApprvals)
                    {
                        if (!dicHeinApproval.ContainsKey(item.TREATMENT_ID))
                            dicHeinApproval[item.TREATMENT_ID] = new List<V_HIS_HEIN_APPROVAL>();
                        dicHeinApproval[item.TREATMENT_ID].Add(item);
                    }
                }

                if (ListSereServ != null && ListSereServ.Count > 0)
                {
                    foreach (var sereServ in ListSereServ)
                    {
                        if (sereServ.TDL_HEIN_SERVICE_TYPE_ID.HasValue && sereServ.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT && sereServ.AMOUNT > 0 && sereServ.PRICE > 0 && sereServ.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.TDL_TREATMENT_ID.HasValue)
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

                if (hisTrackings != null && hisTrackings.Count > 0)
                {
                    foreach (var tracking in hisTrackings)
                    {
                        if (!dicTracking.ContainsKey(tracking.TREATMENT_ID))
                            dicTracking[tracking.TREATMENT_ID] = new List<HIS_TRACKING>();

                        dicTracking[tracking.TREATMENT_ID].Add(tracking);
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
                    //sap xep thoi gian tang dan de trong th co nhieu dhst se lay cai co thoi gian thuc hien lon nhat
                    //lay dhst cuoi cung co can nang
                    listDhst = listDhst.OrderBy(o => o.EXECUTE_TIME).ToList();
                    foreach (var item in listDhst)
                    {
                        if (dicDhst.ContainsKey(item.TREATMENT_ID))
                        {
                            if (item.WEIGHT.HasValue) dicDhst[item.TREATMENT_ID] = item;
                            else if (!dicDhst[item.TREATMENT_ID].WEIGHT.HasValue)
                                dicDhst[item.TREATMENT_ID] = item;
                        }
                        else
                            dicDhst[item.TREATMENT_ID] = item;

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

                foreach (var treatment in hisTreatments)
                {
                    InputADO ado = new InputADO();
                    ado.Treatment = treatment;
                    ado.HeinApprovals = new List<V_HIS_HEIN_APPROVAL>();
                    if (!dicHeinApproval.ContainsKey(treatment.ID))
                    {
                        //var errorHein = "Hồ sơ chưa khóa viện phí";
                        CommonParam alterparam = new CommonParam();
                        var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(alterparam).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, treatment.ID, alterparam);
                        V_HIS_HEIN_APPROVAL heinApp = new V_HIS_HEIN_APPROVAL();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_HEIN_APPROVAL>(heinApp, currentPatientTypeAlter);
                        ado.HeinApprovals.Add(heinApp);
                    }
                    else
                    {
                        ado.HeinApprovals = dicHeinApproval.ContainsKey(treatment.ID) ? dicHeinApproval[treatment.ID] : null;
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
                    ado.Branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == treatment.BRANCH_ID);
                    if (dicDhst.ContainsKey(treatment.ID))
                    {
                        ado.Dhst = dicDhst[treatment.ID];
                    }

                    if (dicSereServTein.ContainsKey(treatment.ID))
                    {
                        ado.SereServTeins = dicSereServTein[treatment.ID];
                    }

                    if (dicTracking.ContainsKey(treatment.ID))
                    {
                        ado.Trackings = dicTracking[treatment.ID];
                    }

                    if (dicSereServPttt.ContainsKey(treatment.ID))
                    {
                        ado.SereServPttts = dicSereServPttt[treatment.ID];
                    }

                    if (dicBedLog.ContainsKey(treatment.ID))
                    {
                        ado.BedLogs = dicBedLog[treatment.ID];
                    }

                    if (dicEkipUser.ContainsKey(treatment.ID))
                    {
                        ado.EkipUsers = dicEkipUser[treatment.ID].Distinct().ToList();
                    }

                    if (dicDebate.ContainsKey(treatment.ID))
                    {
                        ado.ListDebate = dicDebate[treatment.ID];
                    }

                    if (dicDhstList.ContainsKey(treatment.ID))
                    {
                        ado.ListDhsts = dicDhstList[treatment.ID];
                    }

                    ado.HeinApproval = ado.HeinApprovals != null ? ado.HeinApprovals.FirstOrDefault() : null;
                    ado.MaterialPackageOption = HisConfigCFG.GetValue(HisConfigCFG.MOS__BHYT__CALC_MATERIAL_PACKAGE_PRICE_OPTION);
                    ado.MaterialPriceOriginalOption = HisConfigCFG.GetValue(HisConfigCFG.XML__4210__MATERIAL_PRICE_OPTION);
                    ado.MaterialStentRatio = HisConfigCFG.GetValue(HisConfigCFG.XML__4210__MATERIAL_STENT_RATIO_OPTION);
                    ado.TenBenhOption = HisConfigCFG.GetValue(HisConfigCFG.TEN_BENH_OPTION);
                    ado.HeinServiceTypeCodeNoTutorial = HisConfigCFG.GetValue(HisConfigCFG.MaThuocOption);
                    ado.XMLNumbers = HisConfigCFG.GetValue(HisConfigCFG.XmlNumbers);
                    ado.MaterialStent2Limit = HisConfigCFG.GetValue(HisConfigCFG.Stent2LimitOption);
                    ado.IsTreatmentDayCount6556 = HisConfigCFG.GetValue(HisConfigCFG.IS_TREATMENT_DAY_COUNT_6556);
                    ado.MaBacSiOption = HisConfigCFG.GetValue(HisConfigCFG.MaBacSiOption);

                    ado.ConfigData = NewConfig;
                    ado.ListHeinMediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>();
                    ado.MaterialTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                    ado.TotalIcdData = BackendDataWorker.Get<HIS_ICD>();
                    ado.TotalSericeData = BackendDataWorker.Get<V_HIS_SERVICE>();
                    ado.Employees = BackendDataWorker.Get<HIS_EMPLOYEE>();
                    ado.ListHeinServiceType = BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();
                    His.Bhyt.ExportXml.CreateXmlMain xmlMain = new His.Bhyt.ExportXml.CreateXmlMain(ado);

                    string errorMess = "";
                    string fullFileName = "";
                    if (isCollinear)
                    {
                        fullFileName = xmlMain.Run4210PathCollinear(ref errorMess);
                    }
                    else
                    {
                        fullFileName = xmlMain.Run4210Path(ref errorMess);
                    }

                    var treatmentsss = listSelection.FirstOrDefault(o => o.ID == treatment.ID);
                    if (String.IsNullOrWhiteSpace(fullFileName))
                    {
                        if (!DicErrorMess.ContainsKey(errorMess))
                        {
                            DicErrorMess[errorMess] = new List<string>();
                        }

                        DicErrorMess[errorMess].Add(treatment.TREATMENT_CODE);
                        treatmentsss.XML4210_URL = "";
                    }
                    else
                    {
                        treatmentsss.XML4210_URL = fullFileName;
                        isSuccess = true;
                    }
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

        //Lay branch theo cashier_room_id chu ko lay tu token (tranh truong hop chay thread bi mat thong tin token)
        private HIS_BRANCH GetByCashierRoomId(long cashierRoomId)
        {
            try
            {
                V_HIS_CASHIER_ROOM cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>() != null ? BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == cashierRoomId) : null;
                if (cashierRoom != null)
                {
                    return (BackendDataWorker.Get<HIS_BRANCH>() != null ? BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == cashierRoom.BRANCH_ID) : null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        bool SetDataToLocalXml(string path)
        {
            bool result = false;
            try
            {
                if (this._Branch == null)
                {
                    return result;
                }

                GlobalConfigStore.Branch = this._Branch;

                GlobalConfigStore.ListIcdCode_Nds = HisConfigCFG.GetListValue(HisConfigCFG.MRS_HIS_REPORT_BHYT_NDS_ICD_CODE__OTHER);
                GlobalConfigStore.ListIcdCode_Nds_Te = HisConfigCFG.GetListValue(HisConfigCFG.MRS_HIS_REPORT_BHYT_NDS_ICD_CODE__TE);

                GlobalConfigStore.IsInit = true;
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
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

        public void FocusTreatmentCode()
        {
            try
            {
                txtTreatCodeOrHeinCard.Focus();
                txtTreatCodeOrHeinCard.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Thread
        private void CreateThreadGetData(List<V_HIS_TREATMENT_1> listSelection)
        {
            System.Threading.Thread HeinApproval = new System.Threading.Thread(ThreadGetHeinApproval);
            System.Threading.Thread SereServ2 = new System.Threading.Thread(ThreadGetSereServ2);
            System.Threading.Thread Treatment3 = new System.Threading.Thread(ThreadGetTreatment3);
            System.Threading.Thread Dhst_Tracking = new System.Threading.Thread(ThreadGetDhst_Tracking);
            System.Threading.Thread SereServTein_PTTT = new System.Threading.Thread(ThreadGetSereServTein_PTTT);
            try
            {
                HeinApproval.Start(listSelection);
                SereServ2.Start(listSelection);
                Treatment3.Start(listSelection);
                Dhst_Tracking.Start(listSelection);
                SereServTein_PTTT.Start(listSelection);

                HeinApproval.Join();
                SereServ2.Join();
                Treatment3.Join();
                Dhst_Tracking.Join();
                SereServTein_PTTT.Join();
            }
            catch (Exception ex)
            {
                HeinApproval.Abort();
                SereServ2.Abort();
                Treatment3.Abort();
                Dhst_Tracking.Abort();
                SereServTein_PTTT.Abort();
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
                        hisSereServTeins.AddRange(resulTein);
                    }

                    HisSereServPtttViewFilter ssPtttFilter = new HisSereServPtttViewFilter();
                    ssPtttFilter.TDL_TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resultPttt = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/GetView", ApiConsumers.MosConsumer, ssPtttFilter, param);
                    if (resultPttt != null && resultPttt.Count > 0)
                    {
                        hisSereServPttts.AddRange(resultPttt);
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
                        listDhst.AddRange(resultDhst);
                    }

                    HisTrackingFilter trackingFilter = new HisTrackingFilter();
                    trackingFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultTracking = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, trackingFilter, param);
                    if (resultTracking != null && resultTracking.Count > 0)
                    {
                        hisTrackings.AddRange(resultTracking);
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

        private void ThreadGetTreatment3(object obj)
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
                    HisTreatmentView3Filter treatmentFilter = new HisTreatmentView3Filter();
                    treatmentFilter.IDs = limit.Select(o => o.ID).ToList();
                    var resultTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_3>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW_3, ApiConsumers.MosConsumer, treatmentFilter, param);
                    if (resultTreatment != null && resultTreatment.Count > 0)
                    {
                        hisTreatments.AddRange(resultTreatment);
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
                    var resultSS = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_2>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW_2, ApiConsumers.MosConsumer, ssFilter, param);
                    if (resultSS != null && resultSS.Count > 0)
                    {
                        ListSereServ.AddRange(resultSS);

                        var ekipIds = resultSS.Select(o => o.EKIP_ID ?? 0).Where(o => o != 0).Distinct().ToList();
                        if (ekipIds != null && ekipIds.Count > 0)//null sẽ có 1 id bằng 0
                        {
                            HisEkipUserFilter ekipFilter = new HisEkipUserFilter();
                            ekipFilter.EKIP_IDs = ekipIds;
                            var resultEkip = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EKIP_USER>>("api/HisEkipUser/Get", ApiConsumers.MosConsumer, ekipFilter, param);
                            if (resultEkip != null && resultEkip.Count > 0)
                            {
                                ListEkipUser.AddRange(resultEkip);
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

        private void ThreadGetHeinApproval(object obj)
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
                    HisHeinApprovalViewFilter approvalFilter = new HisHeinApprovalViewFilter();
                    approvalFilter.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resultHeinApproval = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_HEIN_APPROVAL>>("api/HisHeinApproval/GetView", ApiConsumers.MosConsumer, approvalFilter, param);
                    if (resultHeinApproval != null && resultHeinApproval.Count > 0)
                    {
                        listHeinApproval.AddRange(resultHeinApproval);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridViewTreatment_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_TREATMENT_1)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "ViewXML")
                        {
                            try
                            {
                                if (!String.IsNullOrWhiteSpace(data.XML4210_URL))
                                {
                                    e.RepositoryItem = repositoryItemButtonEdit_XMLViewerEnable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemButtonEdit_XMLViewerDisable;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }

                        if (e.Column.FieldName == "ViewCollinearXML")
                        {
                            try
                            {
                                if (!String.IsNullOrWhiteSpace(data.COLLINEAR_XML4210_URL))
                                {
                                    e.RepositoryItem = repositoryItemButtonEdit_CollinearXMLView__Enable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemButtonEdit_CollinearXMLView__Disable;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_XMLViewerEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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
                        if (hi.Column.FieldName == "ViewXML" && !String.IsNullOrWhiteSpace(treatment1.XML4210_URL))
                        {
                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.XMLViewer").FirstOrDefault();
                            if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.XMLViewer'");
                            if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                            {
                                moduleData.RoomId = this.currentModule.RoomId;
                                moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                                List<object> listArgs = new List<object>();

                                if (File.Exists(treatment1.XML4210_URL))
                                {
                                    listArgs.Add(treatment1.XML4210_URL);
                                }
                                else
                                {
                                    MemoryStream TemplateStream = GetStreamByUrl(treatment1.XML4210_URL);
                                    listArgs.Add(TemplateStream);
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
                                MessageManager.Show("Chức năng chưa hỗ trợ phiên bản hiện tại");
                            }
                        }
                        else if (hi.Column.FieldName == "ViewCollinearXML" && !String.IsNullOrWhiteSpace(treatment1.COLLINEAR_XML4210_URL))
                        {
                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.XMLViewer").FirstOrDefault();
                            if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.XMLViewer'");
                            if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                            {
                                moduleData.RoomId = this.currentModule.RoomId;
                                moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                                List<object> listArgs = new List<object>();

                                if (File.Exists(treatment1.COLLINEAR_XML4210_URL))
                                {
                                    listArgs.Add(treatment1.COLLINEAR_XML4210_URL);
                                }
                                else
                                {
                                    MemoryStream TemplateStream = GetStreamByUrl(treatment1.COLLINEAR_XML4210_URL);
                                    listArgs.Add(TemplateStream);
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
                                MessageManager.Show("Chức năng chưa hỗ trợ phiên bản hiện tại");
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

        private void InitBranchCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(CboBranch.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__CboBranch);
                CboBranch.Properties.Tag = gridCheck;
                CboBranch.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = CboBranch.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(CboBranch.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__CboBranch(object sender, EventArgs e)
        {
            try
            {
                branchSelecteds = new List<HIS_BRANCH>();
                foreach (MOS.EFMODEL.DataModels.HIS_BRANCH rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        branchSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBranch()
        {
            try
            {
                var datas = BackendDataWorker.Get<HIS_BRANCH>();
                if (datas != null)
                {
                    CboBranch.Properties.DataSource = datas;
                    CboBranch.Properties.DisplayMember = "BRANCH_NAME";
                    CboBranch.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = CboBranch.Properties.View.Columns.AddField("BRANCH_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "";
                    CboBranch.Properties.PopupFormWidth = 200;
                    CboBranch.Properties.View.OptionsView.ShowColumnHeaders = false;
                    CboBranch.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = CboBranch.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(CboBranch.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBranch_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_BRANCH rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.BRANCH_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
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
                                DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.LoiKhiLayDuLieuLoc, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
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
                        && string.IsNullOrEmpty(item.TREATMENT_CODE.Trim())
                        && string.IsNullOrEmpty(item.ICD_CODEs.Trim())) continue;

                    HisTreatmentView1ImportFilter.TreatmentImportFilter filter = new HisTreatmentView1ImportFilter.TreatmentImportFilter();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentView1ImportFilter.TreatmentImportFilter>(filter, item);
                    if (!string.IsNullOrEmpty(item.ICD_CODEs.Trim()) && (string.IsNullOrEmpty(item.IN_TIME_STR) || string.IsNullOrEmpty(item.OUT_TIME_STR)))
                    {
                        error += string.Format("Bạn chưa nhập đủ thông tin  “Ngày vào” hoặc “Ngày ra”.");
                    }
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
                    if (!string.IsNullOrEmpty(item.ICD_CODEs))
                    {
                        filter.ICD_CODEs = item.ICD_CODEs.Trim().Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
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
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageLang.TaiFileVeMayTramThanhCong);
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageLang.KhongTimThayFileImport);
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageLang.TaiFileVeMayTramThatBai);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkAutoDownFileXml_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhilecheckAutoDownFileXmlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateAutoDownFileXml = (this.currentControlStateRDOAutoDownFileXml != null && this.currentControlStateRDOAutoDownFileXml.Count > 0) ? this.currentControlStateRDOAutoDownFileXml.Where(o => o.KEY == checkAutoDownFileXml.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateAutoDownFileXml != null)
                {
                    csAddOrUpdateAutoDownFileXml.VALUE = (checkAutoDownFileXml.CheckState.ToString());
                }
                else
                {
                    csAddOrUpdateAutoDownFileXml = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateAutoDownFileXml.KEY = checkAutoDownFileXml.Name;
                    csAddOrUpdateAutoDownFileXml.VALUE = (checkAutoDownFileXml.CheckState.ToString());
                    csAddOrUpdateAutoDownFileXml.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDOAutoDownFileXml == null)
                        this.currentControlStateRDOAutoDownFileXml = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDOAutoDownFileXml.Add(csAddOrUpdateAutoDownFileXml);
                }
                this.controlStateWorkerAutoDownFileXml.SetData(this.currentControlStateRDOAutoDownFileXml);
                WaitingManager.Hide();
                if (checkAutoDownFileXml.Checked)
                {
                    if (String.IsNullOrWhiteSpace(txtFolderSave.Text))
                    {
                        FolderBrowserDialog fbd = new FolderBrowserDialog();
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            if (this.MakeFolderSave(fbd.SelectedPath))
                            {
                                txtFolderSave.Text = fbd.SelectedPath;
                            }
                        }
                    }
                    if (String.IsNullOrWhiteSpace(txtFolderSave.Text))
                    {
                        XtraMessageBox.Show("Bạn chưa chọn thư mục lưu file XML tải tự động vui lòng thiết lập trước khi thực hiện tải!", MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DevExpress.Utils.DefaultBoolean.True);
                        checkAutoDownFileXml.Checked = false;
                        return;
                    }
                    else if (!Directory.Exists(txtFolderSave.Text))
                    {
                        XtraMessageBox.Show("Thư mục tải tự động không hợp lệ vui lòng chọn lại", MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DevExpress.Utils.DefaultBoolean.True);
                        checkAutoDownFileXml.Checked = false;
                        return;
                    }

                    if (spinIntervalTime.Value <= 0)
                    {
                        XtraMessageBox.Show(ResourceMessageLang.ChuaThietLapChuKyThoiGianTai, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DevExpress.Utils.DefaultBoolean.True);
                        checkAutoDownFileXml.Checked = false;
                        return;
                    }

                    txtFolderSave.Enabled = false;
                    spinIntervalTime.Enabled = false;

                    this.StartTimer();
                }
                else
                {
                    this.StopTimer();
                    txtFolderSave.Enabled = true;
                    spinIntervalTime.Enabled = true;
                }



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool MakeFolderSave(string folderPath)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrWhiteSpace(folderPath))
                {
                    if (System.IO.Directory.Exists(folderPath))
                    {
                        return true;
                    }
                    var dicInfo = System.IO.Directory.CreateDirectory(folderPath);
                    if (dicInfo == null)
                    {
                        LogSystem.Warn("Tao folderPath luu tu dong file that bai");
                        return false;
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

        private void StartTimer()
        {
            try
            {
                loadXmlTimer.Interval = (int)(spinIntervalTime.Value * 60000);
                loadXmlTimer.Enabled = true;
                this.loadXmlTimer_Tick(null, null);
                loadXmlTimer.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void StopTimer()
        {
            try
            {
                loadXmlTimer.Stop();
                loadXmlTimer.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void loadXmlTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (timerTickIsRunning)
                {
                    LogSystem.Info("Tien trinh tai file xml dang duoc chay. Khong cho phep khoi tao tien trinh khac");
                    return;
                }
                timerTickIsRunning = true;

                LogSystem.Info("Begin Run Thread Auto Load Xml");



                listTreatmentDynamic = this.GetTreatment();
                if (listTreatmentDynamic != null && listTreatmentDynamic.Count > 0)
                {
                    LogSystem.Info("Thread Auto Load Xml. TreatmentCount: " + listTreatmentDynamic.Count);
                    backgroundWorker1.RunWorkerAsync();
                }
                else
                {
                    LogSystem.Info("Khong co ho so dieu tri nao. Khong thuc hien tai va luu file xml");
                }
                LogSystem.Info("End Run Thread Auto Load Xml");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            timerTickIsRunning = false;
        }

        private void ProcessLoadAndSaveFile(HIS_TREATMENT treat)
        {
            try
            {
                bool success = false;
                string messageError = "";
                if ((!String.IsNullOrWhiteSpace(treat.XML4210_URL) && treat.XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__CREATE_FILE_SUCCESS) || (!String.IsNullOrWhiteSpace(treat.COLLINEAR_XML4210_URL) && treat.COLLINEAR_XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.COLLINEAR_XML4210_RESULT__CREATE_FILE_SUCCESS))
                {
                    string path = "";
                    int countSeperate = 0;
                    MemoryStream stream = null;
                    if (!String.IsNullOrWhiteSpace(treat.COLLINEAR_XML4210_URL) && treat.COLLINEAR_XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.COLLINEAR_XML4210_RESULT__CREATE_FILE_SUCCESS)
                    {
                        stream = this.GetStreamByUrl(treat.COLLINEAR_XML4210_URL);
                        countSeperate = treat.COLLINEAR_XML4210_URL.LastIndexOf("\\");
                        path = treat.COLLINEAR_XML4210_URL.Substring(countSeperate + 1, treat.COLLINEAR_XML4210_URL.Length - countSeperate - 1);
                    }
                    else if (!String.IsNullOrWhiteSpace(treat.XML4210_URL) && treat.XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__CREATE_FILE_SUCCESS)
                    {
                        stream = this.GetStreamByUrl(treat.XML4210_URL);
                        countSeperate = treat.XML4210_URL.LastIndexOf("\\");
                        path = treat.XML4210_URL.Substring(countSeperate + 1, treat.XML4210_URL.Length - countSeperate - 1);
                    }

                    if (stream != null && stream.Length > 0)
                    {
                        stream.Position = 0;
                        var fileName = Path.Combine(txtFolderSave.Text, path);
                        var fileStreeam = new FileStream(@"" + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                        if (fileStreeam != null)
                        {
                            try
                            {
                                stream.CopyTo(fileStreeam);
                                success = true;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                                messageError = ResourceMessageLang.LuuFileVaoThuMucMayTramThatBai + "  " + treat.TREATMENT_CODE;
                            }
                        }
                        else
                        {
                            messageError = ResourceMessageLang.LuuFileVaoThuMucMayTramThatBai + "  " + treat.TREATMENT_CODE;
                        }
                        fileStreeam.Dispose();
                    }
                    else
                    {
                        messageError = ResourceMessageLang.TaiFileVeMayTramThatBai + "  " + treat.TREATMENT_CODE;
                    }
                }
                else
                {
                    messageError = ResourceMessageLang.KhongCoDuongDanTaiFileVeMayTram + "  " + treat.TREATMENT_CODE;
                }

                if (!success)
                {
                    LogSystem.Info("Thread Auto Load Xml. Down and save file xml faild, " + messageError);
                }

                if (treat.COLLINEAR_XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.COLLINEAR_XML4210_RESULT__CREATE_FILE_SUCCESS)
                {
                    HisTreatmentXmlResultSDO sdo = new HisTreatmentXmlResultSDO();
                    sdo.TreatmentId = treat.ID;
                    sdo.XmlResult = success ? IMSys.DbConfig.HIS_RS.HIS_TREATMENT.COLLINEAR_XML4210_RESULT__DOWN_FILE_SUCCESS : IMSys.DbConfig.HIS_RS.HIS_TREATMENT.COLLINEAR_XML4210_RESULT__DOWN_FILE_FAIL;
                    sdo.Description = messageError;
                    CommonParam param = new CommonParam();
                    bool rs = new BackendAdapter(param).Post<bool>("api/HisTreatment/UpdateCollinearXmlResult", ApiConsumers.MosConsumer, sdo, param);
                    if (!rs)
                    {
                        LogSystem.Error("Update UpdateCollinearXmlResult that bai. TreatmentCode: " + treat.TREATMENT_CODE + ":\n" + LogUtil.TraceData("Param", param));
                    }
                }
                else if (treat.XML4210_RESULT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__CREATE_FILE_SUCCESS)
                {
                    HisTreatmentXmlResultSDO sdo = new HisTreatmentXmlResultSDO();
                    sdo.TreatmentId = treat.ID;
                    sdo.XmlResult = success ? IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__DOWN_FILE_SUCCESS : IMSys.DbConfig.HIS_RS.HIS_TREATMENT.XML4210_RESULT__DOWN_FILE_FAIL;
                    sdo.Description = messageError;
                    CommonParam param = new CommonParam();
                    bool rs = new BackendAdapter(param).Post<bool>("api/HisTreatment/UpdateXmlResult", ApiConsumers.MosConsumer, sdo, param);
                    if (!rs)
                    {
                        LogSystem.Error("Update XmlResult that bai. TreatmentCode: " + treat.TREATMENT_CODE + ":\n" + LogUtil.TraceData("Param", param));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private List<HIS_TREATMENT> GetTreatment()
        {
            List<HIS_TREATMENT> result = null;
            try
            {
                if (HisConfigCFG.AutoLoadFileDayNumber < 0)
                {
                    LogSystem.Info("Khong co cau hinh so ngay lay du lieu tai file xml. AutoLoadFileDayNumber: " + HisConfigCFG.AutoLoadFileDayNumber);
                    return null;
                }

                DateTime dt = DateTime.Now.AddDays((-HisConfigCFG.AutoLoadFileDayNumber));

                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.FEE_LOCK_TIME__TO = Convert.ToInt64(DateTime.Now.ToString("yyyyMMdd235959"));
                filter.FEE_LOCK_TIME__FROM = Convert.ToInt64(dt.ToString("yyyyMMdd000000"));

                filter.XML4210_RESULT__OR__COLLINEAR_XML4210_RESULT = 1;

                filter.ColumnParams = new List<string>()
                    {
                        "ID",
                        "TREATMENT_CODE",
                        "IS_ACTIVE",
                        "COLLINEAR_XML4210_RESULT",
                        "COLLINEAR_XML4210_URL",
                        "XML4210_RESULT",
                        "XML4210_URL"
                    };

                LogSystem.Debug("GetDynamic Treatment Filter: " + LogUtil.TraceData("Filter", filter));
                result = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/GetDynamic", ApiConsumers.MosConsumer, filter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void SavePathToRegistry()
        {
            try
            {
                string path = txtFolderSave.Text ?? "";
                Inventec.Common.RegistryUtil.RegistryProcessor.Write(this.registryPathSave, path, this.subFolder);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveIntervalToRegistry()
        {
            try
            {
                int vl = (int)spinIntervalTime.Value;
                Inventec.Common.RegistryUtil.RegistryProcessor.Write(this.registryInterval, vl, this.subFolder);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReadFromRegistry()
        {
            try
            {
                object p = Inventec.Common.RegistryUtil.RegistryProcessor.Read(this.registryPathSave, this.subFolder);
                if (p != null && !String.IsNullOrWhiteSpace(p.ToString()))
                {
                    if (MakeFolderSave(p.ToString()))
                    {
                        txtFolderSave.Text = p.ToString();
                    }
                }

                int interval = 10;
                object t = Inventec.Common.RegistryUtil.RegistryProcessor.Read(this.registryInterval, this.subFolder);
                if (t != null)
                {
                    int o = 0;
                    int.TryParse(t.ToString(), out o);
                    if (o > 0)
                    {
                        interval = o;
                    }
                }
                spinIntervalTime.Value = interval;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFolderSave_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit)
                    return;
                SavePathToRegistry();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFolderSave_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        if (this.MakeFolderSave(fbd.SelectedPath))
                        {
                            txtFolderSave.Text = fbd.SelectedPath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinIntervalTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit)
                    return;
                SaveIntervalToRegistry();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportXMLcollinear_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportXMLcollinear.Enabled || listSelection == null || listSelection.Count == 0) return;
                CommonParam param = new CommonParam();
                bool success = false;
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    success = this.GenerateXml(ref param, listSelection, fbd.SelectedPath, true);
                    WaitingManager.Hide();
                    if (success && param.Messages.Count == 0)
                    {
                        listSelectionExported__XML4210Colinear = new List<V_HIS_TREATMENT_1>();
                        listSelectionExported__XML4210Colinear.AddRange(listSelection);
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }

                    SessionManager.ProcessTokenLost(param);

                    this.gridControlTreatment.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnExportExcel.Enabled || listSelection == null || listSelection.Count == 0) return;

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "DanhSachBenhNhanXml4210.xlsx");

                //chọn đường dẫn
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //getdata
                    WaitingManager.Show();
                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Base.ResourceMessageLang.KhongTimThayFileImport, templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.KhongTimThayFileImport + "(" + templateFile + ")");
                        return;
                    }

                    ProcessData(listSelection, ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.TuDongXuatFileThanhCong);

                                if (MessageBox.Show(Base.ResourceMessageLang.TuDongXuatFileThanhCong,
                                   MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.TuDongXuatFileThatBai);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<TreatmentDetailExportADO> listDetail = new List<TreatmentDetailExportADO>();

        private void ProcessData(List<V_HIS_TREATMENT_1> listTreatment, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    listDetail = new List<TreatmentDetailExportADO>();
                    List<Task> taskAll = new List<Task>();
                    int skip = 0;
                    while (listTreatment.Count - skip > 0)
                    {
                        var limit = listTreatment.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;
                        Task ts = Task.Factory.StartNew((object obj) =>
                        {
                            ProcessDetailData((List<V_HIS_TREATMENT_1>)obj);
                        }, limit);
                        taskAll.Add(ts);
                    }

                    Task.WaitAll(taskAll.ToArray());

                    Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                    Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                    store.SetCommonFunctions();

                    listDetail = listDetail.OrderBy(o => o.TREATMENT_CODE).ThenBy(o => o.INTRUCTION_TIME).ThenBy(o => o.ID).ToList();
                    listTreatment = listTreatment.OrderBy(o => o.TREATMENT_CODE).ToList();
                    objectTag.AddObjectData(store, "ExportTotal", listTreatment);
                    objectTag.AddObjectData(store, "ExportDetail", listDetail);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDetailData(List<V_HIS_TREATMENT_1> listTreatment)
        {
            try
            {
                Dictionary<long, List<HIS_DEPARTMENT_TRAN>> dicDepartmentTran = new Dictionary<long, List<HIS_DEPARTMENT_TRAN>>();
                Dictionary<long, List<V_HIS_SERE_SERV_2>> dicSereServExport = new Dictionary<long, List<V_HIS_SERE_SERV_2>>();

                CommonParam param = new CommonParam();
                HisSereServView2Filter ssFilter = new HisSereServView2Filter();
                ssFilter.TREATMENT_IDs = listTreatment.Select(o => o.ID).ToList();
                var resultSS = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_2>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW_2, ApiConsumers.MosConsumer, ssFilter, param);
                if (resultSS != null && resultSS.Count > 0)
                {
                    foreach (var item in resultSS)
                    {
                        if (item.TDL_HEIN_SERVICE_TYPE_ID.HasValue && item.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT && item.AMOUNT > 0 && item.PRICE > 0 && item.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && item.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && item.TDL_TREATMENT_ID.HasValue)
                        {
                            if (!dicSereServExport.ContainsKey(item.TDL_TREATMENT_ID ?? 0))
                                dicSereServExport[item.TDL_TREATMENT_ID ?? 0] = new List<V_HIS_SERE_SERV_2>();

                            dicSereServExport[item.TDL_TREATMENT_ID ?? 0].Add(item);
                        }
                    }
                }

                HisDepartmentTranViewFilter tranFilter = new HisDepartmentTranViewFilter();
                tranFilter.TREATMENT_IDs = listTreatment.Select(o => o.ID).ToList();
                tranFilter.IS_RECEIVE = true;
                var resultTran = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, tranFilter, param);
                if (resultTran != null && resultTran.Count > 0)
                {
                    foreach (var item in resultTran)
                    {
                        if (!dicDepartmentTran.ContainsKey(item.TREATMENT_ID))
                        {
                            dicDepartmentTran[item.TREATMENT_ID] = new List<HIS_DEPARTMENT_TRAN>();
                        }

                        dicDepartmentTran[item.TREATMENT_ID].Add(item);
                    }
                }

                foreach (var treatment in listTreatment)
                {
                    if (dicSereServExport.ContainsKey(treatment.ID))
                    {
                        List<V_HIS_SERE_SERV_2> sereServExp = dicSereServExport[treatment.ID];
                        List<HIS_DEPARTMENT_TRAN> trans = new List<HIS_DEPARTMENT_TRAN>();
                        List<string> exportHeinServiceCode = GetExportHeinServiceCode(treatment);
                        if (exportHeinServiceCode != null && exportHeinServiceCode.Count > 0)
                        {
                            sereServExp = sereServExp.Where(o => exportHeinServiceCode.Contains(o.TDL_HEIN_SERVICE_BHYT_CODE) || exportHeinServiceCode.Contains(o.ACTIVE_INGR_BHYT_CODE)).ToList();
                        }

                        if (dicDepartmentTran.ContainsKey(treatment.ID))
                        {
                            trans = dicDepartmentTran[treatment.ID];
                        }

                        foreach (var item in sereServExp)
                        {
                            TreatmentDetailExportADO ado = new TreatmentDetailExportADO(item, treatment);

                            var currentTran = trans.OrderByDescending(o => o.DEPARTMENT_IN_TIME ?? 0).FirstOrDefault(o => o.DEPARTMENT_IN_TIME <= item.INTRUCTION_TIME);
                            if (currentTran != null)
                            {
                                ado.IN_DEPARTMENT_ICD_CODE = currentTran.ICD_CODE;
                                ado.IN_DEPARTMENT_ICD_NAME = currentTran.ICD_NAME;
                                ado.IN_DEPARTMENT_ICD_SUB_CODE = currentTran.ICD_SUB_CODE;
                                ado.IN_DEPARTMENT_ICD_TEXT = currentTran.ICD_TEXT;
                                ado.OUT_DEPARTMENT_ICD_CODE = treatment.ICD_CODE;
                                ado.OUT_DEPARTMENT_ICD_NAME = treatment.ICD_NAME;
                                ado.OUT_DEPARTMENT_ICD_SUB_CODE = currentTran.ICD_SUB_CODE;
                                ado.OUT_DEPARTMENT_ICD_TEXT = currentTran.ICD_TEXT;

                                var nextDepa = trans.FirstOrDefault(o => o.PREVIOUS_ID == currentTran.ID);
                                if (nextDepa != null)
                                {
                                    ado.OUT_DEPARTMENT_ICD_CODE = nextDepa.ICD_CODE;
                                    ado.OUT_DEPARTMENT_ICD_NAME = nextDepa.ICD_NAME;
                                    ado.OUT_DEPARTMENT_ICD_SUB_CODE = nextDepa.ICD_SUB_CODE;
                                    ado.OUT_DEPARTMENT_ICD_TEXT = nextDepa.ICD_TEXT;
                                }
                            }

                            listDetail.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<string> GetExportHeinServiceCode(V_HIS_TREATMENT_1 treatment)
        {
            List<string> result = new List<string>();
            try
            {
                if (treatment != null && listTreatmentImport != null && listTreatmentImport.Count > 0)
                {
                    long outTimeFrom = treatment.OUT_TIME.HasValue ? Convert.ToInt64(treatment.OUT_TIME.Value.ToString().Substring(0, 8) + "000000") : 0;
                    long outTimeto = treatment.OUT_TIME.HasValue ? Convert.ToInt64(treatment.OUT_TIME.Value.ToString().Substring(0, 8) + "235959") : 0;
                    var lstTreatment = listTreatmentImport.Where(o => (o.TDL_PATIENT_CODE == treatment.TDL_PATIENT_CODE || o.TREATMENT_CODE == treatment.TREATMENT_CODE) && o.OUT_TIME >= outTimeFrom && o.OUT_TIME <= outTimeto).ToList();
                    if (lstTreatment != null && lstTreatment.Count > 0 && !String.IsNullOrWhiteSpace(lstTreatment.First().EXPORT_HEIN_SERVICE_CODE))
                    {
                        result = lstTreatment.First().EXPORT_HEIN_SERVICE_CODE.Trim().Split('|').ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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

        private void btnPath_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.SavePath = fbd.SelectedPath;
                    txtSavePath.Text = this.SavePath;
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
                        if (item.KEY == txtSavePath.Name)
                        {
                            txtSavePath.Text = item.VALUE;
                            this.SavePath = item.VALUE;
                        }
                        else if (item.KEY == cboStatusFeeLockOrEndTreatment.Name)
                        {
                            cboStatusFeeLockOrEndTreatment.Text = item.VALUE;
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

        private void InitControlStateAutoDownFileXml()
        {
            try
            {
                isNotLoadWhilecheckAutoDownFileXmlStateInFirst = true;
                this.controlStateWorkerAutoDownFileXml = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDOAutoDownFileXml = controlStateWorkerAutoDownFileXml.GetData(moduleLink);
                if (this.currentControlStateRDOAutoDownFileXml != null && this.currentControlStateRDOAutoDownFileXml.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDOAutoDownFileXml)
                    {
                        if (item.KEY == checkAutoDownFileXml.Name)
                        {
                            switch (item.VALUE)
                            {
                                case "Unchecked":
                                    {
                                        checkAutoDownFileXml.CheckState = CheckState.Unchecked;
                                        break;
                                    }
                                case "Checked":
                                    {
                                        checkAutoDownFileXml.CheckState = CheckState.Checked;
                                        txtFolderSave.Enabled = false;
                                        spinIntervalTime.Enabled = false;

                                        this.StartTimer();
                                        break;
                                    }

                                case "Indeterminate":
                                    {
                                        checkAutoDownFileXml.CheckState = CheckState.Indeterminate;
                                        break;
                                    }

                            }
                        }
                    }
                }
                isNotLoadWhilecheckAutoDownFileXmlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSavePath_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == txtSavePath.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = txtSavePath.Text;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = txtSavePath.Name;
                    csAddOrUpdate.VALUE = txtSavePath.Text;
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ExportGroup_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportGroup.Enabled || listSelection == null || listSelection.Count == 0) return;
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Info("ExportGroup_Click Begin");
                bool success = false;

                if (string.IsNullOrEmpty(this.SavePath))
                {
                    btnPath_Click(null, null);
                }
                if (!string.IsNullOrEmpty(this.SavePath))
                {
                    WaitingManager.Show();
                    success = this.GenerateXmlGroup(ref param, listSelection, this.SavePath);
                    WaitingManager.Hide();
                    if (success && param.Messages.Count == 0)
                    {
                        listSelectionExported__XML4210 = new List<V_HIS_TREATMENT_1>();
                        listSelectionExported__XML4210.AddRange(listSelection);
                    }

                    this.gridControlTreatment.RefreshDataSource();
                }

                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                Inventec.Common.Logging.LogSystem.Info("ExportGroup_Click End");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool GenerateXmlGroup(ref CommonParam paramExport, List<V_HIS_TREATMENT_1> listSelection, string pathSave)
        {
            bool result = false;
            try
            {
                if (listSelection.Count > 0)
                {
                    listSelection = listSelection.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();
                    if (String.IsNullOrEmpty(pathSave))
                    {
                        pathSave = ConfigStore.GetFolderSaveXml + "\\ExportXmlPlus\\Xml" + DateTime.Now.ToString("ddMMyyyy");
                        var dicInfo = System.IO.Directory.CreateDirectory(pathSave);
                        if (dicInfo == null)
                        {
                            paramExport.Messages.Add(Base.ResourceMessageLang.KhongTaoDuocFolderLuuXml);
                            return result;
                        }
                    }

                    if (!GlobalConfigStore.IsInit && !this.SetDataToLocalXml(pathSave))
                    {
                        paramExport.Messages.Add(Base.ResourceMessageLang.KhongThieLapDuocCauHinhDuLieuXuatXml);
                        return result;
                    }

                    GlobalConfigStore.PathSaveXml = pathSave;

                    listHeinApproval = new List<V_HIS_HEIN_APPROVAL>();
                    ListSereServ = new List<V_HIS_SERE_SERV_2>();
                    ListEkipUser = new List<HIS_EKIP_USER>();
                    ListBedlog = new List<V_HIS_BED_LOG>();
                    hisTreatments = new List<V_HIS_TREATMENT_3>();
                    listDhst = new List<HIS_DHST>();
                    hisTrackings = new List<HIS_TRACKING>();
                    ListDebates = new List<HIS_DEBATE>();
                    hisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                    hisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();

                    int skip = 0;
                    while (listSelection.Count - skip > 0)
                    {
                        var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                        CreateThreadGetData(limit);
                    }

                    string message = ProcessExportXmlGroup(ref result, hisTreatments, listHeinApproval, ListSereServ, listDhst, hisSereServTeins, hisTrackings, hisSereServPttts, ListEkipUser, ListBedlog, ListDebates);
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

        private string ProcessExportXmlGroup(ref bool isSuccess, List<V_HIS_TREATMENT_3> hisTreatments, List<V_HIS_HEIN_APPROVAL> listHeinApproval,
            List<V_HIS_SERE_SERV_2> listSereServ, List<HIS_DHST> listDhst, List<V_HIS_SERE_SERV_TEIN> hisSereServTeins, List<HIS_TRACKING> hisTrackings,
            List<V_HIS_SERE_SERV_PTTT> hisSereServPttts, List<HIS_EKIP_USER> listEkipUser, List<V_HIS_BED_LOG> ListBedlog, List<HIS_DEBATE> hisDebates)
        {
            string result = "";
            try
            {
                InputGroupADO ado = new InputGroupADO();
                ado.BedLogs = ListBedlog;
                ado.Branch = BranchDataWorker.Branch;
                ado.Dhsts = listDhst;
                ado.EkipUsers = listEkipUser;
                ado.HeinApprovals = listHeinApproval;
                ado.ListSereServ = listSereServ;
                ado.SereServPttts = hisSereServPttts;
                ado.SereServTeins = hisSereServTeins;
                ado.Trackings = hisTrackings;
                ado.Treatments = hisTreatments;
                ado.ListDebate = hisDebates;

                ado.MaterialPackageOption = HisConfigCFG.GetValue(HisConfigCFG.MOS__BHYT__CALC_MATERIAL_PACKAGE_PRICE_OPTION);
                ado.MaterialPriceOriginalOption = HisConfigCFG.GetValue(HisConfigCFG.XML__4210__MATERIAL_PRICE_OPTION);
                ado.MaterialStentRatio = HisConfigCFG.GetValue(HisConfigCFG.XML__4210__MATERIAL_STENT_RATIO_OPTION);
                ado.TenBenhOption = HisConfigCFG.GetValue(HisConfigCFG.TEN_BENH_OPTION);
                ado.HeinServiceTypeCodeNoTutorial = HisConfigCFG.GetValue(HisConfigCFG.MaThuocOption);
                ado.XMLNumbers = HisConfigCFG.GetValue(HisConfigCFG.XmlNumbers);
                ado.MaterialStent2Limit = HisConfigCFG.GetValue(HisConfigCFG.Stent2LimitOption);
                ado.IsTreatmentDayCount6556 = HisConfigCFG.GetValue(HisConfigCFG.IS_TREATMENT_DAY_COUNT_6556);

                ado.MaterialTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                ado.TotalIcdData = BackendDataWorker.Get<HIS_ICD>();
                ado.TotalSericeData = BackendDataWorker.Get<V_HIS_SERVICE>();
                ado.ListHeinMediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>();
                ado.ConfigData = BackendDataWorker.Get<HIS_CONFIG>();

                His.Bhyt.ExportXml.CreateXmlMain xmlMain = new His.Bhyt.ExportXml.CreateXmlMain(ado);

                string fullFileName = xmlMain.Run4210GroupPath(ref result);
                if (!String.IsNullOrWhiteSpace(fullFileName))
                {
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void gridViewTreatment_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                if (listSelection != null && listSelection.Count() > 0)
                {
                    GridHitInfo hi = e.HitInfo;
                    if (hi.InRowCell)
                    {
                        int rowHandleSelected = gridViewTreatment.GetVisibleRowHandle(hi.RowHandle);
                        if (this.barManager == null)
                            this.barManager = new DevExpress.XtraBars.BarManager();
                        this.barManager.Form = this;
                        this.popupMenuProcessor = new PopupMenuProcessor(this.barManager, ExportXML_MouseRightClick);
                        this.popupMenuProcessor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ExportXML_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem))
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.XuatXML:
                            btnExportXml_Click(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.XuatXMLThongTuyen:
                            btnExportXMLcollinear_Click(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.XuatXMLGop:
                            ExportGroup_Click(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.XuatDuLieuBenhNhan:
                            BtnExportExcel_Click(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.XuatLaiFileXML4210Server:
                            ExportXML4210Server();
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ExportXML4210Server()
        {
            try
            {
                if (this.listSelection != null && this.listSelection.Count() > 0)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var rs = new BackendAdapter(param).Post<List<V_HIS_TREATMENT_1>>("api/Histreatment/ExportXML4210", ApiConsumers.MosConsumer, listSelection.Select(o => o.ID).ToList(), param);
                    if (rs != null && rs.Count() > 0)
                    {
                        success = true;
                        foreach (var item in rs)
                        {
                            var treatment = listSelection.FirstOrDefault(o => o.ID == item.ID);
                            treatment.XML4210_URL = item.XML4210_URL;
                        }
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                foreach (HIS_TREATMENT treat in listTreatmentDynamic)
                {
                    this.ProcessLoadAndSaveFile(treat);
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
