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
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.ViewInfo;
using MOS.Filter;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using EMR.Filter;
using EMR.EFMODEL.DataModels;
using System.IO;
using System.Reflection;
using System.Net;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Common.SignLibrary;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.EmrDocument.Base;
using AutoMapper;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using HIS.Desktop.Plugins.EmrDocument.Resources;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Drawing.Printing;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Common.SignFile;
using EMR.SDO;
using static Aspose.Pdf.Operator;

namespace HIS.Desktop.Plugins.EmrDocument
{
    public partial class EmrDocumentForm : FormBase
    {
        #region Declare
        public HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        public List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        V_HIS_ROOM room = null;
        long _TreatmentId = 0;
        string treatmentCode = "";
        List<long> treatmentIds = new List<long>();
        List<string> treatmentCodes = new List<string>();
        List<EmrDocumentADO> listData;
        List<EmrDocumentADO> listDataTrue;
        static List<EmrDocumentADO> listDataTrueStatic;
        EmrDocumentADO curentEmrDocument;
        string PatientCode = "";
        List<EMR_TREATMENT> lstTreatment = new List<EMR_TREATMENT>();
        List<long> lstTreatmentID = new List<long>();
        List<long> lstTreatmentID_Treatment = new List<long>();
        /// <summary>
        /// lấy các dữ liệu có ID > 0
        /// </summary>
        List<EmrDocumentADO> lstDataPage;

        int currentPage = 0;

        string loginName = null;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;

        bool isStore = false;
        bool isInit = true;
        bool IsSigning = false;
        bool checkreason = false;
        bool checkColumn = false;
        List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_GROUP> currentDocumentGroups;
        string outPdfFile = "";
        DocumentUpdateStateForIntegrateSystem documentUpdateStateForIntegrateSystem;
        Dictionary<long, string> DicoutPdfFile;
        string emrTreatmentStoreCode;
        RefeshReference refreshData = null;
        List<EMR_DOCUMENT_TYPE> lstEmrType { get; set; }
        List<PatientSignType> lstPatientSignType = new List<PatientSignType>();
        bool isFirstLoad = false;
        #endregion

        public EmrDocumentForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData, RefeshReference fr)
            : base(module)
        {
            InitializeComponent();
            currentModule = module;
            this.refreshData = fr;
            this.delegateSelect = delegateData;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public EmrDocumentForm(Inventec.Desktop.Common.Modules.Module module, long _treatmentId, RefeshReference fr)
            : base(module)
        {
            InitializeComponent();
            this.refreshData = fr;
            currentModule = module;
            this._TreatmentId = _treatmentId;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public EmrDocumentForm(Inventec.Desktop.Common.Modules.Module module, string _treatmentCode, bool isSigning, RefeshReference fr)
            : base(module)
        {
            InitializeComponent();
            currentModule = module;
            this.refreshData = fr;
            this.treatmentCode = _treatmentCode;
            this.IsSigning = isSigning;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public EmrDocumentForm(Inventec.Desktop.Common.Modules.Module module, List<string> _treatmentCodes, bool _isStore, RefeshReference fr)
            : base(module)
        {
            InitializeComponent();
            currentModule = module;
            this.refreshData = fr;
            this.treatmentCodes = _treatmentCodes;
            this.isStore = _isStore;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public EmrDocumentForm(Inventec.Desktop.Common.Modules.Module module, RefeshReference fr)
            : base(module)
        {
            try
            {
                InitializeComponent();
                this.refreshData = fr;
                currentModule = module;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Loadform
        private void EmrDocumentForm_Load(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.LocalStorage.EmrConfig.ConfigLoader.Refresh();
                Config.ConfigKey.GetConfigKey();
                this.controlAcs = new List<ACS.EFMODEL.DataModels.ACS_CONTROL>();
                GetControlAcs();
                this.documentUpdateStateForIntegrateSystem = new DocumentUpdateStateForIntegrateSystem();
                loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (IsSigning)
                {
                    cboStatus.Focus();
                    cboStatus.EditValue = "Đang ký";
                }
                else
                {
                    cboStatus.EditValue = "Tất cả";
                }
                if (Config.ConfigKey.deleteFileOption == "1")
                {
                    lciIncludeDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lciIncludeDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                this.InitControlState();
                this.ProcessGroupRow();
                MeShow();
                isInit = false;
                InitEmployee();
                if (refreshData != null)
                    this.refreshData();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        void GetControlAcs()
        {
            try
            {
                CommonParam param = new CommonParam();
                ACS.SDO.AcsTokenLoginSDO tokenLoginSDOForAuthorize = new ACS.SDO.AcsTokenLoginSDO();
                tokenLoginSDOForAuthorize.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                tokenLoginSDOForAuthorize.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;

                var acsAuthorize = new BackendAdapter(param).Get<ACS.SDO.AcsAuthorizeSDO>(HIS.Desktop.ApiConsumer.AcsRequestUriStore.ACS_TOKEN__AUTHORIZE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, tokenLoginSDOForAuthorize, param);

                if (acsAuthorize != null)
                {
                    controlAcs = acsAuthorize.ControlInRoles.ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EmrDocumentForm_Leave(object sender, EventArgs e)
        {
            try
            {
                this.refreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitEmployee()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEmployeeFilter filter = new HisEmployeeFilter();
                filter.LOGINNAME__EXACT = loginName;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("filter1", filter));
                var hisEmployeedatas = new BackendAdapter(param).Get<List<HIS_EMPLOYEE>>("/api/HisEmployee/Get", ApiConsumers.MosConsumer, filter, param);
                var hisEmployeedata = (hisEmployeedatas != null && hisEmployeedatas.Count > 0) ? hisEmployeedatas.FirstOrDefault() : null;
                if (hisEmployeedata != null && hisEmployeedata.IS_ADMIN == 1)
                {
                    checkIncludeDelete.Enabled = true;
                }
                else
                {
                    checkIncludeDelete.Enabled = false;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void MeShow()
        {
            if (!Config.ConfigKey.IsHasConnectionEmr)
                return;
            this.lstTreatment = new List<EMR_TREATMENT>();

            if (!String.IsNullOrWhiteSpace(this.treatmentCode))
            {
                EMR.Filter.EmrTreatmentFilter filter = new EMR.Filter.EmrTreatmentFilter();
                filter.TREATMENT_CODE__EXACT = this.treatmentCode;
                var treatments = new BackendAdapter(new CommonParam()).Get<List<EMR_TREATMENT>>("api/EmrTreatment/Get", ApiConsumer.ApiConsumers.EmrConsumer, filter, null);
                if (treatments != null && treatments.Count > 0)
                {
                    this._TreatmentId = treatments.FirstOrDefault().ID;
                }
            }
            else if (treatmentCodes != null && treatmentCodes.Count > 0)
            {
                EMR.Filter.EmrTreatmentFilter filter = new EMR.Filter.EmrTreatmentFilter();
                filter.TREATMENT_CODEs = this.treatmentCodes;
                var treatments = new BackendAdapter(new CommonParam()).Get<List<EMR_TREATMENT>>("api/EmrTreatment/Get", ApiConsumer.ApiConsumers.EmrConsumer, filter, null);
                if (treatments != null && treatments.Count > 0)
                {
                    this.treatmentIds = treatments.Select(s => s.ID).ToList();
                }
            }

            Inventec.Common.Logging.LogSystem.Info("treatmentCode1: " + this.treatmentCode + " _TreatmentId: " + this._TreatmentId);
            if (this._TreatmentId != 0 && this._TreatmentId != null)
            {
                EMR.Filter.EmrTreatmentFilter filter = new EMR.Filter.EmrTreatmentFilter();
                filter.ID = this._TreatmentId;
                var Treatmentlst = new BackendAdapter(new CommonParam()).Get<List<EMR_TREATMENT>>("api/EmrTreatment/Get", ApiConsumer.ApiConsumers.EmrConsumer, filter, null);
                if (Treatmentlst != null && Treatmentlst.Count > 0)
                {
                    this.PatientCode = Treatmentlst.FirstOrDefault().PATIENT_CODE;
                    this.treatmentCode = Treatmentlst.FirstOrDefault().TREATMENT_CODE;
                    this.emrTreatmentStoreCode = Treatmentlst.FirstOrDefault().STORE_CODE;
                }
                Inventec.Common.Logging.LogSystem.Info("PatientCode: " + this.PatientCode);
            }

            if (!string.IsNullOrEmpty(this.PatientCode))
            {
                EMR.Filter.EmrTreatmentFilter filter = new EMR.Filter.EmrTreatmentFilter();
                filter.PATIENT_CODE__EXACT = this.PatientCode;
                this.lstTreatment = new BackendAdapter(new CommonParam()).Get<List<EMR_TREATMENT>>("api/EmrTreatment/Get", ApiConsumer.ApiConsumers.EmrConsumer, filter, null);
                this.lstTreatmentID = this.lstTreatment.Select(o => o.ID).ToList();
                Inventec.Common.Logging.LogSystem.Info("lstTreatment: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstTreatment), lstTreatment));
            }

            this.room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);

            isFirstLoad = true;
            SetDefaultValue();

            LoadEmrDocumentType();

            LoadPatientSignType();

            FillDatagctFormList();

            SetCaptionByLanguageKey();

            this.btnAttack.Enabled = !this.isStore;
            isFirstLoad = false;
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_GROUP_TYPE)
                        {
                            checkGroupType.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.CHECK_MERGE_DOC)
                        {
                            chkMergeDoc.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.CHECK_MERGE)
                        {
                            chkMerge.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkDowloadGroup.Name)
                        {
                            chkDowloadGroup.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkNotFillZero.Name)
                        {
                            chkNotFillZero.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkAddPatientSign.Name)
                        {
                            chkAddPatientSign.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadEmrDocumentType()
        {
            try
            {
                EMR.Filter.EmrDocumentTypeFilter filter = new EmrDocumentTypeFilter();
                lstEmrType = new BackendAdapter(new CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE>>(HIS.Desktop.Plugins.EmrDocument.EmrRequestUriStore.EMR_DOCUMENT_TYPE_GET, ApiConsumers.EmrConsumer, filter, null);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_TYPE_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(this.cboType, lstEmrType, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatientSignType()
        {
            try
            {
                lstPatientSignType.Add(new PatientSignType() { Name = "Chưa ký", Type = PSType.Unsigned });
                lstPatientSignType.Add(new PatientSignType() { Name = "Đã ký", Type = PSType.Signers });
                lstPatientSignType.Add(new PatientSignType() { Name = "Phải ký", IsPatientMustSign = true, Type = PSType.PatientMustSign });
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Type", columnInfos, false, 200);
                ControlEditorLoader.Load(this.cboStatusPatientSign, lstPatientSignType, controlEditorADO);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.EmrDocument.Resources.Lang", typeof(EmrDocumentForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar1.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkDowloadGroup.Properties.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.chkDowloadGroup.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar3.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.bar3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnsearch.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.bbtnsearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDelete.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.btnDelete.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSTORE_CODE.Properties.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.chkSTORE_CODE.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("EmrDocumentForm.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDownload.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.btnDownload.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnDOCUMENT_TYPE_NAME.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumnDOCUMENT_TYPE_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkGroupType.Properties.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.checkGroupType.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkIncludeDelete.Properties.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.checkIncludeDelete.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMergeDoc.Properties.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.chkMergeDoc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclStt.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.grclStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclUser.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.grclUser.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclTitle.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.grclTitle.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclDepartment.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.grclDepartment.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gtclSignTime.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.gtclSignTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclRejectTime.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.grclRejectTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclRejectTime_LD.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.grclRejectTime_LD.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentForm.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboType.Properties.NullText = Inventec.Common.Resource.Get.Value("EmrDocumentForm.cboType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIncludeDelete.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("EmrDocumentForm.lciIncludeDelete.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIncludeDelete.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.lciIncludeDelete.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCheckGroupType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("EmrDocumentForm.lciCheckGroupType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCheckGroupType.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.lciCheckGroupType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("EmrDocumentForm.layoutControlItem19.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("EmrDocumentForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDatagctFormList()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FillDatagctFormList.1");
                WaitingManager.Show();

                if (checkGroupType.Checked)
                {
                    LoadPaging(new CommonParam());
                }
                else
                {
                    int numPageSize = 0;
                    if (ucPagingDocument.pagingGrid != null)
                    {
                        numPageSize = ucPagingDocument.pagingGrid.PageSize;
                    }
                    else
                    {

                        numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                    }

                    LoadPaging(new CommonParam(0, numPageSize));

                    CommonParam param = new CommonParam();
                    param.Limit = rowCount;
                    param.Count = dataTotal;
                    ucPagingDocument.Init(LoadPaging, param, numPageSize, this.treeListDocument);
                }
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug("FillDatagctFormList.2");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        bool IsMergeDocument = false;
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = null;
                Inventec.Core.ApiResultObject<List<EmrDocumentADO>> apiResult = null;
                listData = new List<EmrDocumentADO>();
                lstDataPage = new List<EmrDocumentADO>();
                IsMergeDocument = false;
                BindingList<EmrDocumentADO> records = new BindingList<EmrDocumentADO>();

                long TreatmentId = 0;

                Inventec.Common.Logging.LogSystem.Debug("LoadPaging.1");
                EMR.Filter.EmrTreatmentFilter Emrfilter = new EMR.Filter.EmrTreatmentFilter();
                if (chkSTORE_CODE.Checked)
                {
                    Emrfilter.STORE_CODE__EXACT = emrTreatmentStoreCode;
                }

                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length == 7 || chkNotFillZero.Checked)
                    {
                        txtTreatmentCode.Text = code;
                    }
                    else if ((code.Length < 12 && code.Length != 7) || !chkNotFillZero.Checked)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }

                    if (!chkSTORE_CODE.Checked)
                    {
                        Emrfilter.TREATMENT_CODE__EXACT = code;
                    }
                }

                if (this.treatmentCodes != null && this.treatmentCodes.Count > 0)
                {
                    Emrfilter.TREATMENT_CODEs = this.treatmentCodes;
                }

                Inventec.Common.Logging.LogSystem.Debug("LoadPaging.1.1");

                var treatments = new BackendAdapter(new CommonParam()).Get<List<EMR_TREATMENT>>("api/EmrTreatment/Get", ApiConsumer.ApiConsumers.EmrConsumer, Emrfilter, null);

                Inventec.Common.Logging.LogSystem.Debug("LoadPaging.1.2");

                if (treatments != null && treatments.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadPaging.1.3");
                    if (!chkSTORE_CODE.Checked)
                    {
                        TreatmentId = treatments.FirstOrDefault().ID;
                    }
                    else
                    {
                        lstTreatmentID_Treatment = treatments.Select(o => o.ID).ToList();
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("LoadPaging.1.4");
                Inventec.Common.Logging.LogSystem.Debug("LoadPaging.2");
                if (Config.ConfigKey.IsStoredMustReqToView && treatments != null && treatments.Count == 1 && treatments[0].STORE_TIME != null)
                {
                    TreatmentCheckViewerSDO data = new TreatmentCheckViewerSDO();
                    data.TreatmentId = treatments[0].ID;
                    data.IsRoomLT = room != null ? room.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__LT : false;
                    data.RoomCode = room != null ? room.ROOM_CODE : null;
                    data.DepartmentCode = room != null ? room.DEPARTMENT_CODE : null;
                    var resultData = new BackendAdapter(paramCommon).Post<bool>("api/EmrTreatment/CheckViewer", ApiConsumers.EmrConsumer, data, paramCommon);
                    if (!resultData)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Bệnh án {0} đã được lưu trữ bạn sẽ không thể xem được chi tiết bệnh án.", treatments[0].TREATMENT_CODE), Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK);
                        if (isFirstLoad)
                            this.Close();
                        return;
                    }
                }
                paramCommon = (checkGroupType.Checked) ? new CommonParam() : new CommonParam(startPage, limit);
                #region
                if (chkMergeDoc.Checked || chkMerge.Checked)
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadPaging.3.1");
                    EmrDocumentMergeViewFilter filter = new EmrDocumentMergeViewFilter();

                    SetFilterNavBar(TreatmentId, ref filter);
                    if (chkSTORE_CODE.Checked)
                    {
                        filter.TREATMENT_ID = null;
                        filter.TREATMENT_IDs = lstTreatmentID_Treatment;
                    }
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "NUM_ORDER";
                    filter.ORDER_DIRECTION1 = "ASC";
                    filter.ORDER_FIELD1 = "DOCUMENT_GROUP_NUM_ORDER";
                    filter.ORDER_DIRECTION2 = "ASC";
                    filter.ORDER_FIELD2 = "HIS_ORDER";
                    filter.ORDER_DIRECTION3 = "ASC";
                    filter.ORDER_FIELD3 = "DOCUMENT_TIME";
                    filter.ORDER_DIRECTION4 = "ASC";
                    filter.ORDER_FIELD4 = "CREATE_TIME";

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                    apiResult = new ApiResultObject<List<EmrDocumentADO>>();
                    apiResult.Data = new BackendAdapter(paramCommon).Get<List<EmrDocumentADO>>(HIS.Desktop.Plugins.EmrDocument.EmrRequestUriStore.EMR_DOCUMENT_GET_MERGE_VIEW, ApiConsumers.EmrConsumer, filter, paramCommon);

                    Inventec.Common.Logging.LogSystem.Debug("LoadPaging.4.1");

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                    IsMergeDocument = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadPaging.3.2");
                    EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
                    SetFilterNavBar(TreatmentId, ref filter);
                    if (chkSTORE_CODE.Checked)
                    {
                        filter.TREATMENT_ID = null;
                        filter.TREATMENT_IDs = lstTreatmentID_Treatment;
                    }
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "NUM_ORDER";
                    filter.ORDER_DIRECTION1 = "ASC";
                    filter.ORDER_FIELD1 = "DOCUMENT_GROUP_NUM_ORDER";
                    filter.ORDER_DIRECTION2 = "ASC";
                    filter.ORDER_FIELD2 = "HIS_ORDER";
                    filter.ORDER_DIRECTION3 = "ASC";
                    filter.ORDER_FIELD3 = "DOCUMENT_TIME";
                    filter.ORDER_DIRECTION4 = "ASC";
                    filter.ORDER_FIELD4 = "CREATE_TIME";

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                    apiResult = new BackendAdapter(paramCommon).GetRO<List<EmrDocumentADO>>(HIS.Desktop.Plugins.EmrDocument.EmrRequestUriStore.EMR_DOCUMENT_GET_VIEW, ApiConsumers.EmrConsumer, filter, paramCommon);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));

                    Inventec.Common.Logging.LogSystem.Debug("LoadPaging.4.2");
                }
                Inventec.Common.Logging.LogSystem.Debug("LoadPaging.5");
                #endregion
                if (apiResult != null)
                {
                    #region API RESULT
                    Inventec.Common.Logging.LogSystem.Debug("LoadPaging.6");
                    string documentTreatmentIdKXD = "_____KXD_____";
                    string documentTypeCodeKXD = "____KXD____";
                    string documentGroupCodeKXD = "__KXD__";

                    listData = apiResult.Data;

                    long order = 1;

                    List<EmrDocumentADO> LstDataNew = new List<EmrDocumentADO>();

                    if (string.IsNullOrEmpty(txtTreatmentCode.Text) && this.lstTreatment != null && this.lstTreatment.Count > 1)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("LoadPaging.7");
                        if (checkGroupType.Checked && listData != null && listData.Count > 0)
                        {
                            List<EmrDocumentADO> listTypes = new List<EmrDocumentADO>();
                            List<EmrDocumentADO> listGroups = new List<EmrDocumentADO>();

                            foreach (var item in this.lstTreatment)
                            {
                                var GroupByTreatmentId = listData.Where(o => o.TREATMENT_ID == item.ID).GroupBy(g => g.TREATMENT_ID).ToList();

                                string strtreatmentKey = String.Format("{0}", !string.IsNullOrEmpty(item.TREATMENT_CODE) ? item.TREATMENT_CODE : documentTreatmentIdKXD);

                                var lstCheck = listData.Where(o => o.TREATMENT_ID == item.ID).ToList();
                                if (lstCheck != null && lstCheck.Count > 0)
                                {
                                    listData.Add(new EmrDocumentADO()
                                    {
                                        ID = -1,
                                        DOCUMENT_DISPLAY = String.Format("{0} - {1}", item.TREATMENT_CODE, Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.IN_TIME)),
                                        CHILD_KEY = strtreatmentKey,
                                        PARENT_KEY = "",
                                        CUSTOM_NUM_ORDER = (order),
                                        SIGNERS = "a"
                                    });
                                }

                                foreach (var i in GroupByTreatmentId)
                                {
                                    var GroupByTypes = i.GroupBy(g => g.DOCUMENT_TYPE_ID).ToList();
                                    foreach (var g in GroupByTypes)
                                    {
                                        int count = g.Count();

                                        string strTypeKey = String.Format("{0}__{1}", !String.IsNullOrEmpty(g.ToList().First().DOCUMENT_TYPE_CODE) ? g.ToList().First().DOCUMENT_TYPE_CODE : documentTypeCodeKXD, order);
                                        if (!listData.Exists(e => e.CHILD_KEY == strTypeKey))
                                        {
                                            listData.Add(new EmrDocumentADO()
                                            {
                                                DOCUMENT_DISPLAY = String.Format("Loại: {0}({1})", !String.IsNullOrEmpty(g.ToList().First().DOCUMENT_TYPE_NAME) ? g.ToList().First().DOCUMENT_TYPE_NAME : "Không xác định", count),
                                                CHILD_KEY = strTypeKey,
                                                PARENT_KEY = !string.IsNullOrEmpty(txtTreatmentCode.Text) ? "" : strtreatmentKey,
                                                CUSTOM_NUM_ORDER = (order)
                                            });
                                        }

                                        var listByGroups = g.ToList();
                                        foreach (var itemBG in listByGroups)
                                        {
                                            var docGroupC = (this.currentDocumentGroups != null && currentDocumentGroups.Count > 0 && itemBG.DOCUMENT_GROUP_ID.HasValue && itemBG.DOCUMENT_GROUP_ID.Value > 0) ? this.currentDocumentGroups.Where(t => t.ID == itemBG.DOCUMENT_GROUP_ID.Value).FirstOrDefault() : null;
                                            if (docGroupC != null && !String.IsNullOrEmpty(docGroupC.VIR_PATH))
                                            {
                                                var arrSplit = docGroupC.VIR_PATH.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                                                if (arrSplit != null && arrSplit.Count() < 10)
                                                {
                                                    for (int iii = 0; iii < 10 - arrSplit.Count(); iii++)
                                                    {
                                                        itemBG.CUSTOM_BY_GROUP_NUM_ORDER += "0000000000000000";
                                                    }
                                                }
                                                var docGroupCCs = (this.currentDocumentGroups != null && currentDocumentGroups.Count > 0) ? this.currentDocumentGroups.Where(t => (docGroupC.VIR_PATH + "/").Contains("/" + t.ID + "/")).OrderBy(t => t.NUM_ORDER).ToList() : null;
                                                itemBG.CUSTOM_BY_GROUP_NUM_ORDER += ((docGroupCCs != null && docGroupCCs.Count > 0) ? String.Join("0000000000000000", docGroupCCs.Select(t => (t.NUM_ORDER.HasValue ? (String.Format("{0:0000000000000000}", t.NUM_ORDER) + "") : "9999999999999999"))) : "");
                                            }
                                        }

                                        listByGroups = listByGroups.OrderBy(o => o.CUSTOM_BY_GROUP_NUM_ORDER).ThenBy(o => o.DOCUMENT_TIME).ThenBy(o => o.CREATE_TIME).ToList();

                                        listByGroups.ForEach(o =>
                                        {
                                            var docGroup = (this.currentDocumentGroups != null && currentDocumentGroups.Count > 0 && o.DOCUMENT_GROUP_ID.HasValue && o.DOCUMENT_GROUP_ID.Value > 0) ? this.currentDocumentGroups.Where(t => t.ID == o.DOCUMENT_GROUP_ID.Value).FirstOrDefault() : null;
                                            string strChildKeyGroup = String.Format("{0}____{1}", o.DOCUMENT_TYPE_CODE, docGroup != null ? docGroup.VIR_PATH + "/" : documentGroupCodeKXD);
                                            if (docGroup != null && this.currentDocumentGroups != null && !listData.Exists(p => p.CHILD_KEY == strChildKeyGroup))
                                            {
                                                //Kiểm tra nếu có nhóm văn bản thì cần kiểm tra tiếp nhóm văn bản đó có văn bản cha nào không
                                                //Nếu có thì cần tái hiện lại cây nhóm văn bản và gắn trong loại, văn bản sẽ được gắn vào nhóm văn bản lá
                                                var groupCheckParents = this.currentDocumentGroups != null ? this.currentDocumentGroups.Where(t => t.VIR_PATH != null && ("/" + docGroup.VIR_PATH + "/").Contains("/" + t.ID + "/")).OrderBy(t => t.NUM_ORDER).ToList() : null;
                                                if (groupCheckParents != null && groupCheckParents.Count > 0)
                                                {
                                                    int count11 = listData.Where(p => p.DOCUMENT_GROUP_ID == o.DOCUMENT_GROUP_ID && p.DOCUMENT_TYPE_NAME == o.DOCUMENT_TYPE_NAME).Count();
                                                    foreach (var itemgroupParent in groupCheckParents)
                                                    {
                                                        string strChildKeyGroupChild = String.Format("{0}____{1}", o.DOCUMENT_TYPE_CODE, itemgroupParent.VIR_PATH + "/");
                                                        string strChildKeyGroupParent = (itemgroupParent.PARENT_ID == null || itemgroupParent.PARENT_ID == 0) ? strTypeKey : String.Format("{0}____{1}", o.DOCUMENT_TYPE_CODE, itemgroupParent.PARENT_PATH + "/");

                                                        string cus1 = "";
                                                        var arrSplit1 = itemgroupParent.VIR_PATH.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                                                        if (arrSplit1 != null && arrSplit1.Count() < 10)
                                                        {
                                                            for (int iii = 0; iii < 10 - arrSplit1.Count(); iii++)
                                                            {
                                                                cus1 += "0000000000000000";
                                                            }
                                                        }

                                                        var docGroupCCs = (this.currentDocumentGroups != null && currentDocumentGroups.Count > 0) ? this.currentDocumentGroups.Where(t => (itemgroupParent.VIR_PATH + "/").Contains("/" + t.ID + "/")).OrderBy(t => t.NUM_ORDER).ToList() : null;
                                                        cus1 += ((docGroupCCs != null && docGroupCCs.Count > 0) ? String.Join("0000000000000000", docGroupCCs.Select(t => (t.NUM_ORDER.HasValue ? (String.Format("{0:0000000000000000}", t.NUM_ORDER) + "") : "9999999999999999"))) : "");

                                                        if (!listData.Exists(e => e.CHILD_KEY == strChildKeyGroupChild))
                                                        {
                                                            listData.Add(new EmrDocumentADO()
                                                            {
                                                                DOCUMENT_DISPLAY = (itemgroupParent.IS_LEAF.HasValue && itemgroupParent.IS_LEAF == 1) ? String.Format("{0}({1})", o.DOCUMENT_GROUP_NAME, count11) : itemgroupParent.DOCUMENT_GROUP_NAME,
                                                                CHILD_KEY = strChildKeyGroupChild,
                                                                PARENT_KEY = strChildKeyGroupParent,
                                                                CUSTOM_NUM_ORDER = (order),
                                                                DOCUMENT_GROUP_NUM_ORDER = itemgroupParent.NUM_ORDER,
                                                                CUSTOM_BY_GROUP_NUM_ORDER = cus1
                                                            });
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    int count1 = listData.Where(p => p.DOCUMENT_GROUP_ID == o.DOCUMENT_GROUP_ID && p.DOCUMENT_TYPE_NAME == o.DOCUMENT_TYPE_NAME).Count();

                                                    string cus1 = "";
                                                    var arrSplit1 = docGroup.VIR_PATH.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                                                    if (arrSplit1 != null && arrSplit1.Count() < 10)
                                                    {
                                                        for (int iii = 0; iii < 10 - arrSplit1.Count(); iii++)
                                                        {
                                                            cus1 += "0000000000000000";
                                                        }
                                                    }

                                                    var docGroupCCs = (this.currentDocumentGroups != null && currentDocumentGroups.Count > 0) ? this.currentDocumentGroups.Where(t => (docGroup.VIR_PATH + "/").Contains("/" + t.ID + "/")).OrderBy(t => t.NUM_ORDER).ToList() : null;
                                                    cus1 += ((docGroupCCs != null && docGroupCCs.Count > 0) ? String.Join("0000000000000000", docGroupCCs.Select(t => (t.NUM_ORDER.HasValue ? (String.Format("{0:0000000000000000}", t.NUM_ORDER) + "") : ""))) : "");
                                                    if (!listData.Exists(e => e.CHILD_KEY == strChildKeyGroup))
                                                    {
                                                        listData.Add(new EmrDocumentADO()
                                                        {
                                                            DOCUMENT_DISPLAY = String.Format("{0}({1})", o.DOCUMENT_GROUP_NAME, count1),
                                                            CHILD_KEY = strChildKeyGroup,
                                                            PARENT_KEY = strTypeKey,
                                                            CUSTOM_NUM_ORDER = (order),
                                                            DOCUMENT_GROUP_NUM_ORDER = docGroup.NUM_ORDER,
                                                            CUSTOM_BY_GROUP_NUM_ORDER = cus1
                                                        });
                                                    }
                                                }
                                            }

                                            o.DOCUMENT_DISPLAY = o.DOCUMENT_NAME;
                                            o.CUSTOM_NUM_ORDER = order;
                                            o.PARENT_KEY = (docGroup != null) ? strChildKeyGroup : strTypeKey;
                                            o.CHILD_KEY = String.Format("{0}____{1}____{2}____{3}", o.TREATMENT_CODE, o.DOCUMENT_TYPE_CODE, o.DOCUMENT_GROUP_CODE, o.DOCUMENT_CODE);

                                            order++;
                                        });
                                    }
                                }
                                listData = listData.OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.CUSTOM_BY_GROUP_NUM_ORDER).ThenBy(o => o.DOCUMENT_TIME).ThenBy(o => o.CREATE_TIME).ToList();
                            }
                        }
                        else if (listData != null && listData.Count > 0)
                        {
                            order = 1;

                            foreach (var item in this.lstTreatment)
                            {
                                var lstCheck = listData.Where(o => o.TREATMENT_ID == item.ID).ToList();

                                if (lstCheck != null && lstCheck.Count > 0)
                                {
                                    string strtreatmentKey = String.Format("{0}", !string.IsNullOrEmpty(item.TREATMENT_CODE) ? item.TREATMENT_CODE : documentTreatmentIdKXD);
                                    LstDataNew.Add(new EmrDocumentADO()
                                    {
                                        DOCUMENT_DISPLAY = String.Format("{0} - {1}", item.TREATMENT_CODE, Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.IN_TIME)),
                                        CHILD_KEY = strtreatmentKey,
                                        PARENT_KEY = "",
                                        CUSTOM_NUM_ORDER = (order)
                                    });
                                    LstDataNew.AddRange(lstCheck);

                                    LstDataNew.ForEach(o =>
                                        {
                                            if (o.ID > 0 && o.TREATMENT_CODE == item.TREATMENT_CODE)
                                            {
                                                o.DOCUMENT_DISPLAY = o.DOCUMENT_NAME;
                                                o.CUSTOM_NUM_ORDER = order;
                                                o.PARENT_KEY = strtreatmentKey;
                                                o.CHILD_KEY = String.Format("{0}____{1}____{2}____{3}", o.TREATMENT_CODE, o.DOCUMENT_TYPE_CODE, o.DOCUMENT_GROUP_CODE, o.DOCUMENT_CODE);
                                                order++;
                                            }
                                        });
                                }
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug("LoadPaging.8");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("LoadPaging.9");
                        if (checkGroupType.Checked && listData != null && listData.Count > 0)
                        {
                            var listDataSnapshot = listData.ToList(); // snapshot để GroupBy không bị ảnh hưởng

                            var groupByTypes = listDataSnapshot.GroupBy(g => g.DOCUMENT_TYPE_ID).ToList();
                            foreach (var group in groupByTypes)
                            {
                                var firstItem = group.FirstOrDefault();
                                if (firstItem == null) continue;

                                int count = group.Count();
                                string typeCode = !string.IsNullOrEmpty(firstItem.DOCUMENT_TYPE_CODE) ? firstItem.DOCUMENT_TYPE_CODE : documentTypeCodeKXD;
                                string typeName = !string.IsNullOrEmpty(firstItem.DOCUMENT_TYPE_NAME) ? firstItem.DOCUMENT_TYPE_NAME : "Không xác định";
                                string typeKey = typeCode;

                                // Thêm node loại
                                listData.Add(new EmrDocumentADO
                                {
                                    DOCUMENT_DISPLAY = string.Format("Loại: {0}({1})", typeName, count),
                                    CHILD_KEY = typeKey,
                                    PARENT_KEY = "",
                                    CUSTOM_NUM_ORDER = order
                                });

                                // Duyệt các văn bản theo loại
                                var documents = group.ToList();

                                foreach (var doc in documents)
                                {
                                    string customGroupOrder = BuildGroupOrder(doc.DOCUMENT_GROUP_ID);
                                    doc.CUSTOM_BY_GROUP_NUM_ORDER = customGroupOrder;
                                }

                                documents = documents
                                    .OrderBy(d => d.CUSTOM_BY_GROUP_NUM_ORDER)
                                    .ThenBy(d => d.DOCUMENT_TIME)
                                    .ThenBy(d => d.CREATE_TIME)
                                    .ToList();

                                foreach (var doc in documents)
                                {
                                    var docGroup = currentDocumentGroups.FirstOrDefault(g => g.ID == doc.DOCUMENT_GROUP_ID);
                                    string groupKey = string.Format("{0}____{1}/", doc.DOCUMENT_TYPE_CODE, (docGroup != null ? docGroup.VIR_PATH : documentGroupCodeKXD));

                                    // Nếu chưa tồn tại node nhóm văn bản
                                    if (docGroup != null && !listData.Exists(p => p.CHILD_KEY == groupKey))
                                    {
                                        var parentGroups = GetParentGroups(docGroup);

                                        foreach (var parent in parentGroups)
                                        {
                                            string parentKey = parent.PARENT_ID == null || parent.PARENT_ID == 0
                                                ? typeKey
                                                : string.Format("{0}____{1}/", doc.DOCUMENT_TYPE_CODE, parent.PARENT_PATH);

                                            string childKey = string.Format("{0}____{1}/", doc.DOCUMENT_TYPE_CODE, parent.VIR_PATH);
                                            string groupDisplay = parent.IS_LEAF == 1 ? string.Format("{0}({1})", doc.DOCUMENT_GROUP_NAME, documents.Count(d => d.DOCUMENT_GROUP_ID == doc.DOCUMENT_GROUP_ID)) : parent.DOCUMENT_GROUP_NAME;
                                            string customOrder = BuildGroupOrder(parent.ID);

                                            if (!listData.Exists(p => p.CHILD_KEY == childKey))
                                            {
                                                listData.Add(new EmrDocumentADO
                                                {
                                                    DOCUMENT_DISPLAY = groupDisplay,
                                                    CHILD_KEY = childKey,
                                                    PARENT_KEY = parentKey,
                                                    CUSTOM_NUM_ORDER = order,
                                                    DOCUMENT_GROUP_NUM_ORDER = parent.NUM_ORDER,
                                                    CUSTOM_BY_GROUP_NUM_ORDER = customOrder
                                                });
                                            }
                                        }
                                    }

                                    // Cập nhật văn bản
                                    doc.DOCUMENT_DISPLAY = doc.DOCUMENT_NAME;
                                    doc.CUSTOM_NUM_ORDER = order;
                                    doc.PARENT_KEY = docGroup != null ? groupKey : typeKey;
                                    doc.CHILD_KEY = string.Format("{0}____{1}____{2}", doc.DOCUMENT_TYPE_CODE, doc.DOCUMENT_GROUP_CODE, doc.DOCUMENT_CODE);
                                    order++;
                                }
                            }

                            // Sắp xếp lại toàn bộ
                            listData = listData
                                .OrderByDescending(d => d.NUM_ORDER)
                                .ThenBy(d => d.CUSTOM_BY_GROUP_NUM_ORDER)
                                .ThenBy(d => d.DOCUMENT_TIME)
                                .ThenBy(d => d.CREATE_TIME)
                                .ToList();
                        }
                        else if (listData != null && listData.Count > 0)
                        {
                            order = 1;
                            listData.ForEach(o =>
                            {
                                o.DOCUMENT_DISPLAY = o.DOCUMENT_NAME;
                                o.CUSTOM_NUM_ORDER = order;
                                o.PARENT_KEY = "";
                                o.CHILD_KEY = String.Format("{0}____{1}____{2}", o.DOCUMENT_TYPE_CODE, o.DOCUMENT_GROUP_CODE, o.DOCUMENT_CODE);
                                order++;
                            });
                        }
                        Inventec.Common.Logging.LogSystem.Debug("LoadPaging.10");
                    }
                    #endregion
                    Inventec.Common.Logging.LogSystem.Debug("LoadPaging.11");
                    if (listData != null && listData.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("LoadPaging.12");
                        lstDataPage = listData.Where(o => o.ID > 0).OrderBy(p => p.CUSTOM_NUM_ORDER).ToList();
                    }
                    treeListDocument.ParentFieldName = "PARENT_KEY";
                    treeListDocument.KeyFieldName = "CHILD_KEY";

                    if (LstDataNew != null && LstDataNew.Count > 0)
                    {
                        records = new BindingList<EmrDocumentADO>(LstDataNew);
                    }
                    else if (listData != null && listData.Count > 0)
                    {
                        records = new BindingList<EmrDocumentADO>(listData);
                    }
                    treeListDocument.DataSource = records;

                    treeListDocument.Focus();
                    treeListDocument.CollapseAll();

                    rowCount = (listData == null ? 0 : listData.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);

                    pictureEdit1.Image = imageCheck.Images[0];
                    Inventec.Common.Logging.LogSystem.Debug("LoadPaging.13");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private string BuildGroupOrder(long? groupId)
        {
            try
            {
                if (groupId == null || groupId == 0 || currentDocumentGroups == null) return "";

                var group = currentDocumentGroups.FirstOrDefault(g => g.ID == groupId.Value);
                if (group == null || string.IsNullOrEmpty(group.VIR_PATH)) return "";

                var segments = group.VIR_PATH.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var pad = new string('0', 16);
                string result = new string('0', (10 - segments.Length) * 16);

                var hierarchy = currentDocumentGroups
                    .Where(g => (string.Format("{0}/", group.VIR_PATH)).Contains(string.Format("/{0}/", g.ID)))
                    .OrderBy(g => g.NUM_ORDER)
                    .Select(g => g.NUM_ORDER.HasValue
                        ? g.NUM_ORDER.Value.ToString("0000000000000000")
                        : "9999999999999999");

                result += string.Join("", hierarchy);
                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return "";
        }

        private List<EMR_DOCUMENT_GROUP> GetParentGroups(EMR_DOCUMENT_GROUP group)
        {
            if (group == null || string.IsNullOrEmpty(group.VIR_PATH) || currentDocumentGroups == null)
                return new List<EMR_DOCUMENT_GROUP>();

            return currentDocumentGroups
                .Where(g => string.Format("/{0}/", group.VIR_PATH).Contains(string.Format("/{0}/", g.ID)))
                .OrderBy(g => g.NUM_ORDER)
                .ToList();
        }

        private void SetFilterNavBar(long treatmentId, ref EmrDocumentViewFilter filter)
        {
            try
            {
                //filter.KEY_WORD = txtSearch.Text.Trim();
                if (cboStatus.EditValue == "Tất cả")
                {
                    filter.HAS_NEXT_SIGNER_OR_NOT_SIGNERS = null;
                    filter.HAS_REJECTER = null;
                }
                else if (cboStatus.EditValue == "Đã hoàn thành")
                {
                    filter.HAS_NEXT_SIGNER_OR_NOT_SIGNERS = false;
                    filter.HAS_REJECTER = false;
                    filter.IS_DELETE = false;
                }
                else if (cboStatus.EditValue == "Đang ký")
                {
                    filter.HAS_NEXT_SIGNER_OR_NOT_SIGNERS = true;
                    filter.HAS_REJECTER = false;
                    filter.IS_DELETE = false;
                }
                else if (cboStatus.EditValue == "Từ chối ký")
                {
                    filter.HAS_REJECTER = true;
                    filter.IS_DELETE = false;
                }

                if (!checkIncludeDelete.Checked)
                {
                    filter.IS_DELETE = false;
                }

                if (this.treatmentIds != null && this.treatmentIds.Count > 0)
                {
                    filter.TREATMENT_IDs = this.treatmentIds;
                }

                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    filter.TREATMENT_ID = treatmentId;
                }
                else
                {
                    if (this.lstTreatmentID != null && this.lstTreatmentID.Count > 0)
                    {
                        filter.TREATMENT_IDs = this.lstTreatmentID;
                    }
                }

                if (cboType.EditValue != null)
                {
                    filter.DOCUMENT_TYPE_ID = (long)cboType.EditValue;
                }

                if (cboStatusPatientSign.EditValue != null && !string.IsNullOrEmpty(PatientCode))
                {
                    var sps = (PSType)cboStatusPatientSign.EditValue;
                    switch (sps)
                    {
                        case PSType.Unsigned:
                            filter.UNSIGNED = string.Format(lstPatientSignType.First().Description, PatientCode);
                            break;
                        case PSType.Signers:
                            filter.SIGNERS = string.Format(lstPatientSignType.First().Description, PatientCode);
                            break;
                        case PSType.PatientMustSign:
                            filter.UNSIGNED = string.Format(lstPatientSignType.First().Description, PatientCode);
                            var patientMustSignList = lstEmrType.Where(o => o.PATIENT_MUST_SIGN == 1).ToList();
                            filter.DOCUMENT_TYPE_IDs = patientMustSignList.Count > 0 ? patientMustSignList.Select(o => o.ID).ToList() : null;
                            break;
                        default:
                            break;
                    }
                }

                if (chkMergeDoc.Checked)
                {
                    filter.IS_DELETE = false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(long treatmentId, ref EmrDocumentMergeViewFilter filter)
        {
            try
            {
                //filter.KEY_WORD = txtSearch.Text.Trim();
                if (cboStatus.EditValue == "Tất cả")
                {
                    filter.HAS_NEXT_SIGNER = null;
                    filter.HAS_REJECTER = null;
                }
                else if (cboStatus.EditValue == "Đã hoàn thành")
                {
                    filter.HAS_NEXT_SIGNER_OR_NOT_SIGNERS = false;
                    filter.HAS_REJECTER = false;
                }
                else if (cboStatus.EditValue == "Đang ký")
                {
                    filter.HAS_NEXT_SIGNER_OR_NOT_SIGNERS = true;
                    filter.HAS_REJECTER = false;
                }
                else if (cboStatus.EditValue == "Từ chối ký")
                {
                    filter.HAS_REJECTER = true;
                }

                if (!checkIncludeDelete.Checked)
                {
                    filter.IS_DELETE = false;
                }

                if (this.treatmentIds != null && this.treatmentIds.Count > 0)
                {
                    filter.TREATMENT_IDs = this.treatmentIds;
                }
                if (!chkSTORE_CODE.Checked)
                {
                    if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        filter.TREATMENT_ID = treatmentId;
                    }
                    else
                    {
                        if (this.lstTreatmentID != null && this.lstTreatmentID.Count > 0)
                        {
                            filter.TREATMENT_IDs = this.lstTreatmentID;
                        }
                    }
                }

                if (cboType.EditValue != null)
                {
                    filter.DOCUMENT_TYPE_ID = (long)cboType.EditValue;
                }

                filter.IS_MERGE = chkMergeDoc.Checked;

                filter.IS_MERGE_NAME = chkMerge.Checked;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.cboType.EditValue = null;
                this.btnFirst.Enabled = false;
                this.btnPrevious.Enabled = false;
                this.btnNext.Enabled = false;
                this.btnLast.Enabled = false;
                pictureEdit1.Image = imageCheck.Images[0];
                this.btnDelete.Enabled = false;
                this.btnPrint.Enabled = false;
                this.btnDownload.Enabled = false;
                this.btnPatientSign.Enabled = false;
                this.btnHomeRelativeSign.Enabled = false;
                this.txtTreatmentCode.Text = this.treatmentCode;
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;
                EMR.Filter.EmrDocumentGroupFilter filter = new EmrDocumentGroupFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.currentDocumentGroups = new BackendAdapter(new CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_GROUP>>(HIS.Desktop.Plugins.EmrDocument.EmrRequestUriStore.EMR_DOCUMENT_GROUP_GET, ApiConsumers.EmrConsumer, filter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("btnSearch_Click.1");
                FillDatagctFormList();
                Inventec.Common.Logging.LogSystem.Debug("btnSearch_Click.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPdf(V_EMR_DOCUMENT data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.MERGE_CODE), data.MERGE_CODE) + "chkMergeDoc.Checked=" + chkMergeDoc.Checked);
                if ((chkMergeDoc.Checked || chkMerge.Checked) && !String.IsNullOrEmpty(data.MERGE_CODE))
                {
                    LoadPdfMergeViewer(data);
                }
                else
                {
                    LoadPdfViewer(data);
                }
            }
            catch (Exception ex)
            {
                this.panel1.Controls.Clear();
                this.panel1 = new Panel();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPdfMergeViewer(V_EMR_DOCUMENT data)
        {
            try
            {
                outPdfFile = "";
                string strDTI = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", ConfigSystems.URI_API_ACS, ConfigSystems.URI_API_EMR, ConfigSystems.URI_API_FSS, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().TokenCode, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());
                DocumentManager documentManager = new DocumentManager(strDTI);
                var uc = documentManager.GetUcDocumentMerge(data, ref outPdfFile, chkMerge.Checked);
                if (uc != null)
                {
                    uc.Dock = DockStyle.Fill;
                    this.panel1.Controls.Clear();
                    this.panel1.Controls.Add(uc);

                    string message = "Xem văn bản gộp. Mã văn bản: " + data.DOCUMENT_CODE + ", MERGE_CODE:" + data.MERGE_CODE + ", TREATMENT_CODE: " + data.TREATMENT_CODE + ". Thời gian xem: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + ". Người xem: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => message), message));
                }
                else
                {
                    this.panel1.Controls.Clear();
                }
            }
            catch (Exception ex)
            {
                this.panel1.Controls.Clear();
                this.panel1 = new Panel();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPdfViewer(V_EMR_DOCUMENT data)
        {
            try
            {
                SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADO(data.TREATMENT_CODE, data.DOCUMENT_CODE, data.DOCUMENT_NAME, currentModule.RoomId);
                this.IsSigning = data.REJECTER == null;
                if ((this.IsSigning
                    && data.NEXT_SIGNER == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                    || (((data.NEXT_SIGNER == null && (data.SIGNERS == null || !data.SIGNERS.Contains("#@!@#" + data.PATIENT_CODE))) || (data.NEXT_SIGNER != null && data.NEXT_SIGNER.Contains("#@!@#" + data.PATIENT_CODE)))
                            && Config.ConfigKey.patientSignOption == "3"))
                {
                    inputADO.IsSign = true;
                }
                else
                    inputADO.IsSign = false;

                inputADO.IsSave = false;
                inputADO.IsExport = false;
                inputADO.IsPrint = true;
                inputADO.IsEnableButtonPrint = controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "EMR000002") != null;
                inputADO.IsShowPatientSign = true;

                if (data.WIDTH != null && data.HEIGHT != null && data.RAW_KIND != null)
                {
                    inputADO.PaperSizeDefault = new System.Drawing.Printing.PaperSize(data.PAPER_NAME, (int)data.WIDTH, (int)data.HEIGHT);
                    if (data.RAW_KIND != null)
                    {
                        inputADO.PaperSizeDefault.RawKind = (int)data.RAW_KIND;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("inputADO______", inputADO));

                Inventec.Common.Logging.LogSystem.Info("data.LAST_VERSION_URL: " + data.LAST_VERSION_URL);
                if (!String.IsNullOrEmpty(data.LAST_VERSION_URL))
                {
                    var documentFileSDOs = GetEmrDocumentFile(data, null, null, null, null);
                    var uc = libraryProcessor.GetUC(documentFileSDOs != null && documentFileSDOs.Count > 0 ? documentFileSDOs[0].Base64Data : null, FileType.Pdf, inputADO);
                    if (uc != null)
                    {
                        uc.Dock = DockStyle.Fill;
                        this.panel1.Controls.Clear();
                        this.panel1.Controls.Add(uc);

                        string message = "Xem văn bản. Mã văn bản: " + data.DOCUMENT_CODE + ", TREATMENT_CODE: " + data.TREATMENT_CODE + ". Thời gian xem: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + ". Người xem: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
                    }
                    else
                    {
                        this.panel1.Controls.Clear();
                    }
                }
                else
                {
                    this.panel1.Controls.Clear();
                }
            }
            catch (Exception ex)
            {
                this.panel1.Controls.Clear();
                this.panel1 = new Panel();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<EmrDocumentFileSDO> GetEmrDocumentFile(V_EMR_DOCUMENT document, List<long> docIds, bool? IsMerge, bool? IsShowPatientSign, bool? IsShowWatermark)
        {
            CommonParam paramCommon = new CommonParam();
            EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
            var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
            if (document != null)
                emrFilter.ID = document.ID;
            else
                emrFilter.IDs = docIds;
            sdo.EmrDocumentViewFilter = emrFilter;
            sdo.IsMerge = IsMerge;
            sdo.IsShowPatientSign = IsShowPatientSign;
            sdo.IsShowWatermark = IsShowWatermark;
            sdo.RoomCode = room != null ? room.ROOM_CODE : null;
            sdo.DepartmentCode = room != null ? room.DEPARTMENT_CODE : null;
            sdo.IsRoomLT = room != null ? room.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__LT : false;
            if (docIds != null && docIds.Count > 0)
            {
                List<NumOrderDocumentSDO> numOrderList = new List<NumOrderDocumentSDO>();
                int stt = 1;
                foreach (var id in docIds)
                {
                    if (id > 0)
                    {
                        var item = new NumOrderDocumentSDO
                        {
                            IdDocument = id,
                            STT = stt++
                        };
                        Inventec.Common.Logging.LogSystem.Debug($"[NumOrderDocumentSDO] IdDocument = {item.IdDocument}, STT = {item.STT}");
                        numOrderList.Add(item);

                    }
                }

                sdo.NumOrderDocuments = numOrderList;
            }
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
            return new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);
        }

        private void repositoryItem__Delete_E_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = treeListDocument.GetDataRecordByNode(treeListDocument.FocusedNode) as EmrDocumentADO;

                    CommonParam param = new CommonParam();
                    EMR_DOCUMENT data = new EMR_DOCUMENT();
                    data.ID = rowData.ID;
                    var resultData = new BackendAdapter(param).Post<bool>(EMR.URI.EmrDocument.DELETE, ApiConsumers.EmrConsumer, data.ID, param);
                    if (resultData)
                    {
                        this.documentUpdateStateForIntegrateSystem.UpdateStateIGSys(rowData, SignStateCode.DOCUMENT_DELETE);
                        FillDatagctFormList();
                    }
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, resultData);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItem__SetingSign_E_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = treeListDocument.GetDataRecordByNode(treeListDocument.FocusedNode) as EmrDocumentADO;
                List<object> listArgs = new List<object>();
                listArgs.Add(rowData.ID);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("EMR.Desktop.Plugins.EmrSign", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnAttack_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = treeListDocument.GetDataRecordByNode(treeListDocument.FocusedNode) as EmrDocumentADO;
                frmAttackFile frmAttackFile = new frmAttackFile(rowData, this._TreatmentId, this.loginName, RefeshAfterAttackFile);
                frmAttackFile.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadViewSign(long? documentID)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                List<V_EMR_SIGN> listSign = null;
                EmrSignViewFilter filter = new EmrSignViewFilter();
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "NUM_ORDER";
                filter.DOCUMENT_ID = documentID;
                listSign = new BackendAdapter(paramCommon).Get<List<V_EMR_SIGN>>(EMR.URI.EmrSign.GET_VIEW, ApiConsumers.EmrConsumer, filter, paramCommon);

                gridViewSign.GridControl.DataSource = listSign;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSign_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                V_EMR_SIGN pData = (V_EMR_SIGN)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "SIGN_TIME_str")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.SIGN_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "User")
                    {
                        if (pData.FLOW_ID.HasValue)
                        {
                            if (!String.IsNullOrWhiteSpace(pData.LOGINNAME))
                            {
                                e.Value = String.Format("{0}({1}-{2})", pData.FLOW_NAME, pData.LOGINNAME ?? "", pData.USERNAME ?? "");
                            }
                            else
                            {
                                e.Value = pData.FLOW_NAME;
                            }
                        }
                        else if (!String.IsNullOrWhiteSpace(pData.LOGINNAME))
                        {
                            e.Value = pData.LOGINNAME + "-" + pData.USERNAME;
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(pData.RELATION_NAME))
                            {
                                e.Value = !string.IsNullOrEmpty(pData.CARD_CODE) ? string.Format("{0}({1})", pData.RELATION_PEOPLE_NAME, pData.CARD_CODE) : pData.RELATION_PEOPLE_NAME;
                            }
                            else
                            {
                                e.Value = pData.VIR_PATIENT_NAME;
                            }
                        }
                    }
                    else if (e.Column.FieldName == "REJECT_TIME_str")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.REJECT_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "TITLE_STR")
                    {
                        if (!String.IsNullOrWhiteSpace(pData.PATIENT_CODE) && !String.IsNullOrWhiteSpace(pData.RELATION_NAME))
                        {
                            e.Value = pData.RELATION_NAME;
                        }
                        else
                        {
                            e.Value = pData.TITLE;
                        }
                    }
                    if (e.Column.FieldName == "RejectTime_LD")
                    {
                        if (!String.IsNullOrWhiteSpace(pData.REJECT_REASON))
                        {
                            e.Value = pData.REJECT_REASON;
                        }
                    }
                    if (e.Column.FieldName == "CANCEL_TIME_str")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CANCEL_TIME ?? 0);
                    }

                    gridControlSign.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboType_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboType.Properties.Buttons[1].Visible = true;
                    cboType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnsearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void btnAttack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAttack.Enabled) return;
                frmAttackFile frmAttackFile = new frmAttackFile(this._TreatmentId, this.loginName, RefeshAfterAttackFile);
                frmAttackFile.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshAfterAttackFile()
        {
            try
            {
                FillDatagctFormList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkIncludeDelete_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkGroupType_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit)
                {
                    return;
                }
                WaitingManager.Show();
                this.SaveStateControlGroupType();
                this.ProcessGroupRow();
                this.FillDatagctFormList();
                this.pictureEdit1.Image = imageCheck.Images[0];
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGroupRow()
        {
            try
            {
                if (checkGroupType.Checked)
                {
                    layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    treeListColumnDOCUMENT_TYPE_NAME.Visible = false;
                    treeListColumnDOCUMENT_TYPE_NAME.VisibleIndex = -1;
                    treeListDocument.CollapseAll();
                }
                else
                {
                    layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    if (!treeListColumnDOCUMENT_TYPE_NAME.Visible)
                    {
                        treeListColumnDOCUMENT_TYPE_NAME.Visible = true;
                        treeListColumnDOCUMENT_TYPE_NAME.VisibleIndex = 4;
                        treeListColumn1.Width = 30;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveStateControlGroupType()
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_GROUP_TYPE && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (checkGroupType.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_GROUP_TYPE;
                    csAddOrUpdate.VALUE = (checkGroupType.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
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

        private void treeListDocument_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Row != null)
                {
                    EmrDocumentADO pData = e.Row as EmrDocumentADO;
                    if (pData != null && pData.ID > 0)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = pData.CUSTOM_NUM_ORDER > 0 ? (long?)pData.CUSTOM_NUM_ORDER : null;
                        }
                        else if (e.Column.FieldName == "Status_Str")
                        {
                            if (pData.IS_DELETE == 1)//xóa
                            {
                                e.Value = "Xóa";
                            }
                            else if (pData.REJECTER != null)//từ chối
                            {
                                e.Value = "Từ chối";
                            }
                            else if (pData.REJECTER == null && (pData.NEXT_SIGNER != null || pData.SIGNERS == null))//Đang ký
                            {
                                e.Value = "Đang ký";
                            }
                            else if (pData.REJECTER == null && pData.NEXT_SIGNER == null && pData.SIGNERS != null)
                            {
                                e.Value = "Đã hoàn thành";
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_str")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATOR")
                        {
                            e.Value = pData.CREATOR;
                        }
                        else if (e.Column.FieldName == "DOCUMENT_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.DOCUMENT_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListDocument_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeListDocument.GetDataRecordByNode(e.Node) as EmrDocumentADO;
                if (data != null && data.ID > 0)
                {
                    string creator = data.CREATOR;
                    long storeTime = (data.STORE_TIME ?? 0);
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (this.isStore)
                        {
                            e.RepositoryItem = repositoryItem__Delete_D;
                        }
                        else if (creator == this.loginName && storeTime <= 0 && data.IS_DELETE != 1)
                        {
                            e.RepositoryItem = repositoryItem__Delete_E;
                        }
                        else if (data.IS_CAPTURE == 1)
                        {
                            e.RepositoryItem = repositoryItem__Delete_E;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItem__Delete_D;
                        }
                    }
                    else if (e.Column.FieldName == "ATTACK")
                    {
                        e.RepositoryItem = repositoryItembtnAttack;
                    }
                    else if (e.Column.FieldName == "SETING_SIGN")
                    {
                        if (this.isStore)
                        {
                            e.RepositoryItem = repositoryItem__SetingSign_D;
                        }
                        else if (storeTime <= 0 && data.IS_DELETE != 1 && (data.COUNT_RESIGN_FAILED == null || data.COUNT_RESIGN_FAILED <= 0))
                        {
                            e.RepositoryItem = repositoryItem__SetingSign_E;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItem__SetingSign_D;
                        }
                    }
                    else if (e.Column.FieldName == "Edit")
                    {
                        if (!this.isStore && creator == this.loginName)
                        {
                            e.RepositoryItem = repEdit_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repEdit_Disable;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListDocument_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = treeListDocument.GetDataRecordByNode(e.Node) as EmrDocumentADO;
                if (data != null)
                {
                    if (data.ID == 0)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                    }
                    else
                    {
                        if (data.IS_DELETE == 1)//Đã xóa
                        {
                            e.Appearance.ForeColor = Color.Red;
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);//gạch
                        }
                        else if (data.COUNT_RESIGN_FAILED != null && data.COUNT_RESIGN_FAILED > 0)
                        {
                            e.Appearance.ForeColor = Color.Maroon;
                        }
                        else if (!string.IsNullOrEmpty(data.REJECTER))//Từ chối
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else if (data.COUNT_RESIGN_WAIT != null && data.COUNT_RESIGN_WAIT > 0)
                        {
                            e.Appearance.ForeColor = Color.Green;
                        }
                        else if (string.IsNullOrEmpty(data.REJECTER) && (!string.IsNullOrEmpty(data.NEXT_SIGNER) || data.SIGNERS == null))//Đang ký
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        else//Đã ký
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListDocument_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListDocument_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                EmrDocumentADO rowEmrDocumentData = (EmrDocumentADO)treeListDocument.GetDataRecordByNode(treeListDocument.FocusedNode);
                if (checkGroupType.Checked)
                {
                    if (e.KeyCode == Keys.Right)
                    {
                        treeListDocument.FocusedNode.Expanded = true;
                    }
                    else if (e.KeyCode == Keys.Left)
                    {
                        treeListDocument.FocusedNode.Expanded = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListDocument_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            try
            {
                if (isInit && checkGroupType.Checked)
                {
                    return;
                }

                EmrDocumentADO rowEmrDocumentData = (EmrDocumentADO)treeListDocument.GetDataRecordByNode(treeListDocument.FocusedNode);
                Inventec.Common.Logging.LogSystem.Debug("treeListDocument_FocusedNodeChanged:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowEmrDocumentData), rowEmrDocumentData));

                if (!treeListDocument.FocusedNode.HasChildren && rowEmrDocumentData != null && rowEmrDocumentData.ID > 0)
                {
                    currentPage = lstDataPage.IndexOf(rowEmrDocumentData);
                    Inventec.Common.Logging.LogSystem.Info("currentPage: " + currentPage);
                    WaitingManager.Show();
                    LoadPdf(rowEmrDocumentData);
                    curentEmrDocument = rowEmrDocumentData;
                    Inventec.Common.Logging.LogSystem.Warn("CURRENT EMR ______________________");
                    loadViewSign(rowEmrDocumentData.ID);
                    WaitingManager.Hide();
                }
                if (currentPage > 0)
                {
                    this.btnFirst.Enabled = true;
                    this.btnPrevious.Enabled = true;
                }
                else if (currentPage < lstDataPage.Count - 1)
                {
                    this.btnNext.Enabled = true;
                    this.btnLast.Enabled = true;
                }
                else if (currentPage == 0)
                {
                    this.btnFirst.Enabled = false;
                    this.btnPrevious.Enabled = false;
                }
                else if (currentPage == lstDataPage.Count - 1)
                {
                    this.btnNext.Enabled = false;
                    this.btnLast.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListDocument_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    TreeList view = sender as TreeList;
                    TreeListHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.HitInfoType == HitInfoType.NodeCheckBox)
                    {
                        if (hi.Node.Nodes.Count == 0)
                        {
                            EmrDocumentADO rowData = (EmrDocumentADO)view.GetDataRecordByNode(hi.Node);
                            if (hi.Node.Checked)
                            {
                                rowData.IsChecked = false;
                                int s = listData.Where(o => o.IsChecked == false && o.ID > 0).Count();
                                if (s == listData.Where(o => o.ID > 0).Count())
                                {
                                    pictureEdit1.Image = imageCheck.Images[0];
                                }
                                else
                                {
                                    pictureEdit1.Image = imageCheck.Images[1];
                                }
                            }
                            else
                            {
                                rowData.IsChecked = true;
                                int s = listData.Where(o => o.IsChecked == true && o.ID > 0).Count();

                                if (s == listData.Where(o => o.ID > 0).Count())
                                {
                                    pictureEdit1.Image = imageCheck.Images[2];
                                }
                                else
                                {
                                    pictureEdit1.Image = imageCheck.Images[1];
                                }

                                Inventec.Common.Logging.LogSystem.Info("dữ liệu node: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowData), rowData) + " __s: " + s + " __listData.Where(o => o.ID > 0).Count: " + listData.Where(o => o.ID > 0).Count());
                            }
                        }
                        else
                        {
                            if (hi.Node.Checked)
                            {
                                NodeIsChecked(view, hi.Node.Nodes, false);
                                int s = listData.Where(o => o.IsChecked == false && o.ID > 0).Count();
                                if (s == listData.Where(o => o.ID > 0).Count())
                                {
                                    pictureEdit1.Image = imageCheck.Images[0];
                                }
                                else
                                {
                                    pictureEdit1.Image = imageCheck.Images[1];
                                }
                            }
                            else
                            {
                                NodeIsChecked(view, hi.Node.Nodes, true);
                                int s = listData.Where(o => o.IsChecked == true && o.ID > 0).Count();

                                if (s == listData.Where(o => o.ID > 0).Count())
                                {
                                    pictureEdit1.Image = imageCheck.Images[2];
                                }
                                else
                                {
                                    pictureEdit1.Image = imageCheck.Images[1];
                                }
                                Inventec.Common.Logging.LogSystem.Info("s: " + s + " listData.Where(o => o.ID > 0).Count(): " +
                                    (listData.Where(o => o.ID > 0).Count()));
                            }
                        }
                    }

                    listDataTrue = new List<EmrDocumentADO>();
                    if (this.listData != null && this.listData.Count > 0)
                    {
                        listDataTrue = this.listData.Where(o => o.IsChecked == true && o.ID > 0).OrderBy(p => p.CUSTOM_NUM_ORDER).ToList();
                    }
                    if (listDataTrue != null && listDataTrue.Count > 0)
                    {
                        this.btnPrint.Enabled = controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "EMR000002") != null;

                        this.btnDownload.Enabled = controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "EMR000003") != null;
                        this.btnHomeRelativeSign.Enabled = true;
                        this.btnPatientSign.Enabled = true;
                    }
                    else
                    {
                        this.btnHomeRelativeSign.Enabled = false;
                        this.btnPatientSign.Enabled = false;
                        this.btnPrint.Enabled = false;
                        this.btnDownload.Enabled = false;
                    }
                    SetStateButtonDelete();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetStateButtonDelete()
        {
            try
            {
                if (this.listDataTrue != null && this.listDataTrue.Count > 0)
                {
                    this.btnDelete.Enabled = false;
                    bool canDelete = true;
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    foreach (var item in this.listDataTrue)
                    {
                        if (item.CREATOR != loginName)
                            canDelete = false;
                    }
                    if (canDelete)
                        this.btnDelete.Enabled = true;
                }
                else
                {
                    this.btnDelete.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NodeIsChecked(TreeList view, DevExpress.XtraTreeList.Nodes.TreeListNodes Nodes, bool check)
        {
            try
            {
                foreach (var item in Nodes)
                {
                    EmrDocumentADO rowData = (EmrDocumentADO)view.GetDataRecordByNode((DevExpress.XtraTreeList.Nodes.TreeListNode)item);
                    if (rowData.ID > 0)
                    {
                        rowData.IsChecked = check;
                        Inventec.Common.Logging.LogSystem.Info("dữ liệu node: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowData), rowData));
                    }
                    else
                    {
                        DevExpress.XtraTreeList.Nodes.TreeListNode listNode = (DevExpress.XtraTreeList.Nodes.TreeListNode)item;
                        NodeIsChecked(view, listNode.Nodes, check);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        internal static void InsertPage1(Stream sourceStream, string sourceFile, Dictionary<long, string> fileListJoin, string desFileJoined, List<EMR_SIGN> signAlls, long documentId, bool IsGroup = false)
        {
            List<string> joinStreams = new List<string>();
            if (fileListJoin != null && fileListJoin.Count > 0)
            {
                iTextSharp.text.pdf.PdfReader reader1 = null;
                Stream sourceStreamCopy = new MemoryStream();
                if (!String.IsNullOrEmpty(sourceFile) && File.Exists(sourceFile))
                {
                    reader1 = new iTextSharp.text.pdf.PdfReader(sourceFile);
                }
                else if (sourceStream != null)
                {
                    sourceStream.Position = 0;
                    sourceStreamCopy = new MemoryStream();
                    sourceStream.CopyTo(sourceStreamCopy);
                    sourceStream.Position = 0;
                    reader1 = new iTextSharp.text.pdf.PdfReader(sourceStream);
                }

                string output = Utils.GenerateTempFileWithin();
                if (signAlls != null && signAlls.Count > 0)
                {
                    ProcessInsertPatientSign(reader1, output, documentId, signAlls);
                }
                else
                {
                    sourceStreamCopy.Position = 0;
                    Utils.ByteToFile(Utils.StreamToByte(sourceStreamCopy), output);
                }
                joinStreams.Add(output);
                int pageCount = reader1.NumberOfPages;
                iTextSharp.text.Rectangle pageSize = reader1.GetPageSizeWithRotation(reader1.NumberOfPages);
                iTextSharp.text.Rectangle pageSize1 = new iTextSharp.text.Rectangle(pageSize.Left, pageSize.Bottom, pageSize.Right, (pageSize.Bottom + pageSize.Height), pageSize.Rotation);

                foreach (var item in fileListJoin)
                {
                    int lIndex1 = item.Value.LastIndexOf(".");
                    string EXTENSION = item.Value.Substring(lIndex1 > 0 ? lIndex1 + 1 : lIndex1);
                    if (EXTENSION != "pdf")
                    {
                        MemoryStream stream = null;
                        if (File.Exists(item.Value))
                        {
                            using (var streamSource = new FileStream(item.Value, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                streamSource.Position = 0;
                                stream = new MemoryStream();
                                streamSource.CopyTo(stream);
                            }
                        }
                        else
                            stream = Inventec.Fss.Client.FileDownload.GetFile(item.Value);
                        stream.Position = 0;
                        string convertTpPdf = Utils.GenerateTempFileWithin();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu convertTpPdf: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => convertTpPdf), convertTpPdf));
                        Stream streamConvert = new FileStream(convertTpPdf, FileMode.Create, FileAccess.Write);
                        iTextSharp.text.Document iTextdocument = new iTextSharp.text.Document(pageSize1, 0, 0, 0, 0);
                        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(iTextdocument, streamConvert);
                        iTextdocument.Open();
                        writer.Open();

                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(stream);
                        if (img.Height > img.Width)
                        {
                            float percentage = 0.0f;
                            percentage = pageSize.Height / img.Height;
                            img.ScalePercent(percentage * 100);
                        }
                        else
                        {
                            float percentage = 0.0f;
                            percentage = pageSize.Width / img.Width;
                            img.ScalePercent(percentage * 100);
                        }
                        iTextdocument.Add(img);
                        iTextdocument.Close();
                        writer.Close();
                        joinStreams.Add(convertTpPdf);
                    }
                    else
                    {
                        MemoryStream stream = null;
                        if (File.Exists(item.Value))
                        {
                            using (var streamSource = new FileStream(item.Value, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                streamSource.Position = 0;
                                stream = new MemoryStream();
                                streamSource.CopyTo(stream);
                            }
                        }
                        else
                            stream = Inventec.Fss.Client.FileDownload.GetFile(item.Value);

                        if (stream != null && stream.Length > 0)
                        {
                            stream.Position = 0;
                            string pdfAddFile = Utils.GenerateTempFileWithin();
                            Utils.ByteToFile(Utils.StreamToByte(stream), pdfAddFile);
                            joinStreams.Add(pdfAddFile);
                            if (signAlls != null && signAlls.Count > 0 && signAlls.Where(o => o.IS_SIGN_ELECTRONIC == 1 && o.COOR_X_RECTANGLE > 0 && o.COOR_Y_RECTANGLE > 0 && o.DOCUMENT_ID == item.Key).ToList().Count > 0)
                            {
                                stream.Position = 0;
                                var reader2g = new PdfReader(stream);
                                ProcessInsertPatientSign(reader2g, pdfAddFile, item.Key, signAlls);
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Loi convert va luu tam file pdf tu server fss ve may tram____item=" + item);
                        }
                    }
                }
                Stream currentStream = File.Open(desFileJoined, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                var pages = new List<int>();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu joinStreams: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => joinStreams), joinStreams));

                foreach (var file in joinStreams)
                {
                    iTextSharp.text.pdf.PdfReader pdfReader = null;
                    pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                    pages = new List<int>();
                    for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                    {
                        pages.Add(i);
                    }
                    pdfReader.SelectPages(pages);
                    pdfConcat.AddPages(pdfReader);
                    pdfReader.Close();
                }

                try
                {
                    reader1.Close();
                }
                catch { }

                try
                {
                    if (sourceStream != null)
                        sourceStream.Close();
                }
                catch { }

                try
                {
                    pdfConcat.Close();
                }
                catch { }

                foreach (var file in joinStreams)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;
                if (IsMergeDocument)
                {
                    loadDictionary();

                    Dictionary<long, string> lstURL = new Dictionary<long, string>();
                    long key = 0;
                    foreach (var item in this.listDataTrue)
                    {
                        if (!string.IsNullOrEmpty(DicoutPdfFile[item.ID]))
                        {
                            lstURL.Add(item.ID, DicoutPdfFile[item.ID]);
                        }
                        if (lstURL.ContainsKey(item.ID) == false)
                        {
                            lstURL.Add(item.ID, item.LAST_VERSION_URL);
                        }
                    }
                    CommonParam param1 = new CommonParam();
                    List<EMR_SIGN> apiResultEmrSign = null;
                    List<Task> list = new List<Task>();
                    list.Add(Task.Run(delegate
                    {
                        EmrAttachmentFilter filterAttachment = new EmrAttachmentFilter();
                        filterAttachment.DOCUMENT_IDs = this.listDataTrue.Select(o => o.ID).ToList();
                        filterAttachment.ORDER_DIRECTION = "DESC";
                        filterAttachment.ORDER_FIELD = "ID";
                        List<EMR_ATTACHMENT> apiResultAttachment = new BackendAdapter(param1).Get<List<EMR_ATTACHMENT>>("api/EmrAttachment/Get", ApiConsumers.EmrConsumer, filterAttachment, param1);
                        if (apiResultAttachment != null && apiResultAttachment.Count > 0)
                        {
                            Dictionary<long, string> lstURLTmp = new Dictionary<long, string>();
                            foreach (var item in lstURL)
                            {
                                lstURLTmp.Add(item.Key, item.Value);
                                foreach (var itemAttachment in apiResultAttachment)
                                {
                                    if (itemAttachment.DOCUMENT_ID == item.Key)
                                    {
                                        long a = itemAttachment.ID + 999999999999999;
                                        if (lstURLTmp.ContainsKey(a) == false)
                                        {
                                            lstURLTmp.Add(a, itemAttachment.URL);
                                        }
                                        if (DicoutPdfFile.ContainsKey(a) == false)
                                        {
                                            DicoutPdfFile.Add(a, "");
                                        }
                                    }
                                }
                            }
                            lstURL = lstURLTmp;
                        }
                    }));
                    list.Add(Task.Run(delegate
                    {
                        EmrSignViewFilter val2 = new EmrSignViewFilter();
                        val2.PATIENT_CODE__EXACT = this.listDataTrue.FirstOrDefault().PATIENT_CODE;
                        apiResultEmrSign = ((AdapterBase)new BackendAdapter(param1)).Get<List<EMR_SIGN>>("api/EmrSign/Get", ApiConsumers.EmrConsumer, val2, param1);
                    }));
                    Task.WaitAll(list.ToArray());
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstURL), lstURL));


                    string output = Utils.GenerateTempFileWithin();
                    if (lstURL != null && lstURL.Count > 0)
                    {
                        key = lstURL.Keys.FirstOrDefault();
                        MemoryStream streamSource = null;
                        string streamSourceStr = null;
                        if (!string.IsNullOrEmpty(DicoutPdfFile[key]))
                        {
                            Inventec.Common.Logging.LogSystem.Info("nhận string");
                            streamSourceStr = DicoutPdfFile[key];
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Info("nhận MemoryStream");
                            string filePath = lstURL.Values.FirstOrDefault();
                            if (File.Exists(filePath))
                            {
                                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    stream.Position = 0;
                                    streamSource = new MemoryStream();
                                    stream.CopyTo(streamSource);
                                }
                            }
                            else
                            {
                                streamSource = Inventec.Fss.Client.FileDownload.GetFile(filePath);
                                streamSource.Position = 0;
                            }
                        }

                        Dictionary<long, string> lst = new Dictionary<long, string>();
                        int dem = 0;
                        foreach (var item in lstURL)
                        {
                            if (dem != 0)
                            {
                                if (lst.ContainsKey(item.Key) == false)
                                {
                                    lst.Add(item.Key, item.Value);
                                }
                            }
                            dem++;
                        }
                        listDataTrueStatic = listDataTrue;
                        if (lst != null && lst.Count > 0)
                        {
                            InsertPage1(streamSource, streamSourceStr, lst, output, apiResultEmrSign, lstURL.Keys.FirstOrDefault());
                        }
                        else
                        {
                            InsertPageOne(streamSource, streamSourceStr, output, apiResultEmrSign, key);
                        }

                        Inventec.Common.Logging.LogSystem.Warn("output: " + output);

                        Inventec.Common.Logging.LogSystem.Info("url: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstURL), lstURL));

                        Inventec.Common.DocumentViewer.Template.frmPdfViewer DocumentView = new Inventec.Common.DocumentViewer.Template.frmPdfViewer(output);

                        DocumentView.Text = "In";

                        DocumentView.ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show(ResourceMessage.KhongLayDuocFile);
                    }

                    try
                    {
                        if (File.Exists(output))
                        {
                            File.Delete(output);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    var documents = GetEmrDocumentFile(null, listDataTrue.Select(o => o.ID).ToList(), chkDowloadGroup.Checked, chkAddPatientSign.Checked, false);

                    string output = Utils.GenerateTempFileWithin();
                    if (documents != null && documents.Count > 0)
                    {
                        documents = documents.OrderBy(a => listDataTrue.OrderByDescending(o => o.NUM_ORDER).OrderBy(o => o.CUSTOM_NUM_ORDER).ToList().FindIndex(o => o.ID == a.DocumentId)).ToList();
                        List<string> joinStreams = new List<string>();

                        List<MemoryStream> documentData = new List<MemoryStream>();
                        foreach (var item in documents)
                        {
                            if (item.Extension.ToLower().Equals("pdf"))
                            {
                                string pdfAddFile = Utils.GenerateTempFileWithin();
                                Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(item.Base64Data))), pdfAddFile);
                                joinStreams.Add(pdfAddFile);
                            }
                        }

                        Stream currentStream = File.Open(output, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                        var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                        var pages = new List<int>();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu joinStreams: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => joinStreams), joinStreams));

                        foreach (var file in joinStreams)
                        {
                            iTextSharp.text.pdf.PdfReader pdfReader = null;
                            pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                            pages = new List<int>();
                            for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                            {
                                pages.Add(i);
                            }
                            pdfReader.SelectPages(pages);
                            pdfConcat.AddPages(pdfReader);
                            pdfReader.Close();
                        }
                        try
                        {
                            pdfConcat.Close();
                        }
                        catch { }

                        foreach (var file in joinStreams)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch { }
                        }

                        Inventec.Common.DocumentViewer.Template.frmPdfViewer DocumentView = new Inventec.Common.DocumentViewer.Template.frmPdfViewer(output);

                        DocumentView.Text = "In";

                        DocumentView.ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show(ResourceMessage.KhongLayDuocFile);
                    }

                    try
                    {
                        if (File.Exists(output))
                        {
                            File.Delete(output);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                this.panel1.Controls.Clear();
                this.panel1 = new Panel();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            try
            {
                var rowEmrDocumentData = this.lstDataPage.FirstOrDefault();
                if (rowEmrDocumentData != null && rowEmrDocumentData.ID > 0)
                {
                    WaitingManager.Show();
                    LoadPdf(rowEmrDocumentData);
                    WaitingManager.Hide();
                    currentPage = 0;

                    this.btnNext.Enabled = true;
                    this.btnLast.Enabled = true;
                    this.btnFirst.Enabled = false;
                    this.btnPrevious.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            try
            {
                var rowEmrDocumentData = this.lstDataPage.LastOrDefault();
                if (rowEmrDocumentData != null && rowEmrDocumentData.ID > 0)
                {
                    WaitingManager.Show();
                    LoadPdf(rowEmrDocumentData);
                    WaitingManager.Hide();
                    currentPage = this.lstDataPage.IndexOf(rowEmrDocumentData);
                    this.btnFirst.Enabled = true;
                    this.btnPrevious.Enabled = true;
                    this.btnNext.Enabled = false;
                    this.btnLast.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.lstDataPage != null && this.lstDataPage.Count > 0)
                {

                    if (this.lstDataPage[currentPage] != null && this.lstDataPage[currentPage].ID > 0)
                    {
                        currentPage++;
                        WaitingManager.Show();
                        LoadPdf(this.lstDataPage[currentPage]);
                        WaitingManager.Hide();

                        if (currentPage > 0)
                        {
                            this.btnFirst.Enabled = true;
                            this.btnPrevious.Enabled = true;
                        }
                        else if (currentPage < lstDataPage.Count - 1)
                        {
                            this.btnNext.Enabled = true;
                            this.btnLast.Enabled = true;
                        }
                        else if (currentPage == 0)
                        {
                            this.btnFirst.Enabled = false;
                            this.btnPrevious.Enabled = false;
                        }
                        else if (currentPage == lstDataPage.Count - 1)
                        {
                            this.btnNext.Enabled = false;
                            this.btnLast.Enabled = false;
                        }
                        Inventec.Common.Logging.LogSystem.Info("lstDataPage.Count - 1: " + (lstDataPage.Count - 1) + "currentPage: " + currentPage);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.lstDataPage != null && this.lstDataPage.Count > 0)
                {
                    currentPage--;
                    if (this.lstDataPage[currentPage] != null && this.lstDataPage[currentPage].ID > 0)
                    {
                        WaitingManager.Show();
                        LoadPdf(this.lstDataPage[currentPage]);
                        WaitingManager.Hide();

                        if (currentPage > 0)
                        {
                            this.btnFirst.Enabled = true;
                            this.btnPrevious.Enabled = true;
                        }
                        else if (currentPage < lstDataPage.Count - 1)
                        {
                            this.btnNext.Enabled = true;
                            this.btnLast.Enabled = true;
                        }
                        else if (currentPage == 0)
                        {
                            this.btnFirst.Enabled = false;
                            this.btnPrevious.Enabled = false;
                        }
                        else if (currentPage == lstDataPage.Count - 1)
                        {
                            this.btnNext.Enabled = false;
                            this.btnLast.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void pictureEdit1_Click(object sender, EventArgs e)
        {
            try
            {
                if (pictureEdit1.Image == imageCheck.Images[0] || pictureEdit1.Image == imageCheck.Images[1])
                {
                    pictureEdit1.Image = imageCheck.Images[2];
                }
                else if (pictureEdit1.Image == imageCheck.Images[2])
                {
                    pictureEdit1.Image = imageCheck.Images[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void pictureEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("pictureEdit1_EditValueChanged");
                if (pictureEdit1.Image == imageCheck.Images[0])
                {
                    checkColumn = false;
                    if (this.listData != null && this.listData.Where(o => o.IsChecked == false && o.ID > 0).Count() != this.listData.Where(o => o.ID > 0).Count())
                    {
                        treeListDocument.UncheckAll();
                    }
                }
                else if (pictureEdit1.Image == imageCheck.Images[2])
                {
                    checkColumn = true;
                    if (this.listData != null && this.listData.Where(o => o.IsChecked == true && o.ID > 0).Count() != this.listData.Where(o => o.ID > 0).Count())
                    {
                        treeListDocument.CheckAll();
                    }
                }
                if (this.listData != null && (pictureEdit1.Image == imageCheck.Images[0] || pictureEdit1.Image == imageCheck.Images[2]))
                {
                    foreach (var item in this.listData)
                    {
                        item.IsChecked = checkColumn;
                    }
                }
                listDataTrue = new List<EmrDocumentADO>();
                if (this.listData != null && this.listData.Count > 0)
                {
                    listDataTrue = this.listData.Where(o => o.IsChecked == true && o.ID > 0).OrderBy(p => p.CUSTOM_NUM_ORDER).ToList();
                }
                if (listDataTrue != null && listDataTrue.Count > 0)
                {
                    this.btnPrint.Enabled = controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "EMR000002") != null;
                    this.btnDownload.Enabled = controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "EMR000003") != null;
                    this.btnHomeRelativeSign.Enabled = true;
                    this.btnPatientSign.Enabled = true;
                }
                else
                {
                    this.btnHomeRelativeSign.Enabled = false;
                    this.btnPatientSign.Enabled = false;
                    this.btnPrint.Enabled = false;
                    this.btnDownload.Enabled = false;
                }
                SetStateButtonDelete();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// load dữ liệu DicoutPdfFile
        /// </summary>
        private void loadDictionary()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => outPdfFile), outPdfFile));
                DicoutPdfFile = new Dictionary<long, string>();
                foreach (var item in lstDataPage)
                {
                    outPdfFile = "";
                    if (chkMergeDoc.Checked && !String.IsNullOrEmpty(item.MERGE_CODE))
                    {
                        if (String.IsNullOrEmpty(outPdfFile))
                        {
                            string strDTI = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", ConfigSystems.URI_API_ACS, ConfigSystems.URI_API_EMR, ConfigSystems.URI_API_FSS, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().TokenCode, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());
                            DocumentManager documentManager = new DocumentManager(strDTI);
                            var uc = documentManager.GetUcDocumentMerge((item.ORIGINAL_HIGH ?? 0), item.TREATMENT_CODE, item.MERGE_CODE, ref outPdfFile);
                        }

                        if (DicoutPdfFile.ContainsKey(item.ID) == false)
                        {
                            DicoutPdfFile.Add(item.ID, outPdfFile);
                        }
                    }
                    else
                    {
                        if (DicoutPdfFile.ContainsKey(item.ID) == false)
                        {
                            DicoutPdfFile.Add(item.ID, outPdfFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;
                //Send Message Reason
                FrmReason frm = new FrmReason((HIS.Desktop.Common.DelegateReturnSuccess)Reload, this.listDataTrue);
                frm.ShowDialog();

                if (checkreason == false)
                {
                    return;
                }

                OpenFileDialog openFolder = new OpenFileDialog();
                openFolder.ValidateNames = false;
                openFolder.CheckFileExists = false;
                openFolder.CheckPathExists = true;
                openFolder.FileName = "Folder Selection.";

                if (openFolder.ShowDialog() == DialogResult.OK)
                {
                    if (IsMergeDocument)
                    {
                        loadDictionary();
                        string directoryPath = Path.GetDirectoryName(openFolder.FileName) + @"\" + listDataTrue.FirstOrDefault().TREATMENT_CODE + "_" + listDataTrue.FirstOrDefault().VIR_PATIENT_NAME;
                        if (!System.IO.Directory.Exists(directoryPath))
                        {
                            System.IO.Directory.CreateDirectory(directoryPath);
                        }

                        int count = 0;
                        int totalList = listDataTrue.Count;
                        string filePath = "";
                        Dictionary<long, string> lstURL = new Dictionary<long, string>();
                        Dictionary<long, string> lstURLJSON = new Dictionary<long, string>();
                        Dictionary<long, string> lstURLXML = new Dictionary<long, string>();

                        long key = 0;
                        foreach (var item in this.listDataTrue)
                        {
                            if (!string.IsNullOrEmpty(DicoutPdfFile[item.ID])) lstURL.Add(item.ID, DicoutPdfFile[item.ID]);

                            if (!lstURL.ContainsKey(item.ID)) lstURL.Add(item.ID, item.LAST_VERSION_URL);

                            if (!lstURLJSON.ContainsKey(item.ID)) lstURLJSON.Add(item.ID, item.LAST_XML_VERSION_URL);

                            if (!lstURLXML.ContainsKey(item.ID)) lstURLXML.Add(item.ID, item.LAST_JSON_VERSION_URL);
                        }

                        CommonParam param1 = new CommonParam();
                        List<EMR_SIGN> apiResultEmrSign = null;
                        List<EMR_ATTACHMENT> apiResultAttachment = null;
                        List<Task> list = new List<Task>();
                        list.Add(Task.Run(delegate
                        {
                            EmrAttachmentFilter filterAttachment = new EmrAttachmentFilter();
                            filterAttachment.DOCUMENT_IDs = this.listDataTrue.Select(o => o.ID).ToList();
                            filterAttachment.ORDER_DIRECTION = "DESC";
                            filterAttachment.ORDER_FIELD = "ID";
                            apiResultAttachment = new BackendAdapter(param1).Get<List<EMR_ATTACHMENT>>("api/EmrAttachment/Get", ApiConsumers.EmrConsumer, filterAttachment, param1);
                            if (apiResultAttachment != null && apiResultAttachment.Count > 0)
                            {
                                Dictionary<long, string> lstURLTmp = new Dictionary<long, string>();
                                foreach (var item in lstURL)
                                {
                                    lstURLTmp.Add(item.Key, item.Value);
                                    foreach (var itemAttachment in apiResultAttachment)
                                    {
                                        if (itemAttachment.DOCUMENT_ID == item.Key)
                                        {
                                            long a = itemAttachment.ID + 999999999999999;
                                            if (lstURLTmp.ContainsKey(a) == false)
                                            {
                                                lstURLTmp.Add(a, itemAttachment.URL);
                                            }
                                            if (DicoutPdfFile.ContainsKey(a) == false)
                                            {
                                                DicoutPdfFile.Add(a, "");
                                            }
                                        }
                                    }
                                }
                                lstURL = lstURLTmp;
                            }
                        }));
                        if (chkAddPatientSign.Checked)
                        {
                            list.Add(Task.Run(delegate
                            {
                                EmrSignViewFilter val2 = new EmrSignViewFilter();
                                val2.PATIENT_CODE__EXACT = this.listDataTrue.FirstOrDefault().PATIENT_CODE;
                                apiResultEmrSign = ((AdapterBase)new BackendAdapter(param1)).Get<List<EMR_SIGN>>("api/EmrSign/Get", ApiConsumers.EmrConsumer, val2, param1);
                            }));
                        }
                        Task.WaitAll(list.ToArray());
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstURL), lstURL));

                        if (chkDowloadGroup.Checked)
                        {
                            string output = Utils.GenerateTempFileWithin();
                            if (lstURL != null && lstURL.Count > 0)
                            {
                                filePath = directoryPath + @"\" + listDataTrue.FirstOrDefault().TREATMENT_CODE + "_" + listDataTrue.FirstOrDefault().VIR_PATIENT_NAME + "_" + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) + ".pdf";
                                key = lstURL.Keys.FirstOrDefault();
                                MemoryStream streamSource = null;
                                string streamSourceStr = null;

                                if (!string.IsNullOrEmpty(DicoutPdfFile[key]))
                                {
                                    Inventec.Common.Logging.LogSystem.Info("nhận string");
                                    streamSourceStr = DicoutPdfFile[key];
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Info("nhận MemoryStream");
                                    if (File.Exists(lstURL.Values.FirstOrDefault()))
                                    {
                                        using (var stream = new FileStream(lstURL.Values.FirstOrDefault(), FileMode.Open, FileAccess.Read, FileShare.Read))
                                        {
                                            stream.Position = 0;
                                            streamSource = new MemoryStream();
                                            stream.CopyTo(streamSource);
                                        }
                                    }
                                    else
                                    {
                                        streamSource = Inventec.Fss.Client.FileDownload.GetFile(lstURL.Values.FirstOrDefault());
                                        streamSource.Position = 0;
                                    }
                                }

                                Dictionary<long, string> lst = new Dictionary<long, string>();
                                int dem = 0;
                                foreach (var item in lstURL)
                                {
                                    if (dem != 0)
                                    {
                                        if (lst.ContainsKey(item.Key) == false)
                                        {
                                            lst.Add(item.Key, item.Value);
                                        }
                                    }
                                    dem++;
                                }
                                listDataTrueStatic = listDataTrue;

                                if (lst != null && lst.Count > 0)
                                {
                                    InsertPage1(streamSource, streamSourceStr, lst, filePath, apiResultEmrSign, lstURL.Keys.FirstOrDefault());
                                }
                                else
                                {
                                    InsertPageOne(streamSource, streamSourceStr, filePath, apiResultEmrSign, key);
                                }

                                Inventec.Common.Logging.LogSystem.Warn("filePath: " + filePath);
                            }
                            if (lstURLJSON != null && lstURLJSON.Count > 0)
                            {
                                foreach (var item in lstURLJSON)
                                {
                                    if (string.IsNullOrEmpty(item.Value))
                                        continue;
                                    filePath = directoryPath + @"\" + listDataTrue.FirstOrDefault().TREATMENT_CODE + "_" + listDataTrue.FirstOrDefault().VIR_PATIENT_NAME + "_" + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) + ".json";
                                    using (MemoryStream streamSource = Inventec.Fss.Client.FileDownload.GetFile(item.Value))
                                    {
                                        streamSource.Position = 0;
                                        using (Stream saveFile = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                                        {
                                            streamSource.CopyTo(saveFile);
                                        }
                                    }
                                }
                            }
                            if (lstURLXML != null && lstURLXML.Count > 0)
                            {
                                foreach (var item in lstURLXML)
                                {
                                    if (string.IsNullOrEmpty(item.Value))
                                        continue;
                                    filePath = directoryPath + @"\" + listDataTrue.FirstOrDefault().TREATMENT_CODE + "_" + listDataTrue.FirstOrDefault().VIR_PATIENT_NAME + "_" + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) + ".xml";
                                    using (MemoryStream streamSource = Inventec.Fss.Client.FileDownload.GetFile(item.Value))
                                    {
                                        streamSource.Position = 0;
                                        using (Stream saveFile = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                                        {
                                            streamSource.CopyTo(saveFile);
                                        }
                                    }
                                }

                            }
                            if (lstURL == null || lstURL.Count() == 0)
                            {
                                MessageManager.Show(ResourceMessage.KhongLayDuocFile);
                            }


                        }
                        else
                        {
                            listDataTrueStatic = listDataTrue;
                            foreach (var item in listDataTrue)
                            {
                                if (!lstURL.ContainsKey(item.ID))
                                    return;
                                string documentName = null;
                                if (!string.IsNullOrEmpty(item.DOCUMENT_NAME))
                                {
                                    //loại bỏ các ký tự đặc biệt: \/:*?"<>|
                                    var charsToRemove = Path.GetInvalidPathChars();
                                    documentName = string.Join("", item.DOCUMENT_NAME.Split(Path.GetInvalidFileNameChars()));
                                }
                                else
                                {
                                    documentName = item.DOCUMENT_NAME;
                                }

                                var versionURL = lstURL[item.ID];
                                var versionJSON = lstURLJSON[item.ID];
                                var versionXML = lstURLXML[item.ID];
                                List<string> attachmentURLs = null;
                                var attachment = apiResultAttachment.Where(o => o.DOCUMENT_ID == item.ID);
                                if (attachment != null)
                                {
                                    attachmentURLs = new List<string>();
                                    foreach (var att in attachment)
                                    {
                                        if (lstURL.ContainsKey(att.ID + 999999999999999))
                                            attachmentURLs.Add(att.URL);
                                    }
                                }
                                if (!string.IsNullOrEmpty(versionURL))
                                {
                                    filePath = directoryPath + @"\" + item.CUSTOM_NUM_ORDER + "_" + documentName + ".pdf";
                                    MemoryStream streamSource = null;
                                    Inventec.Common.Logging.LogSystem.Info("nhận MemoryStream");
                                    streamSource = Inventec.Fss.Client.FileDownload.GetFile(versionURL);
                                    streamSource.Position = 0;
                                    InsertPageOne(streamSource, null, filePath, apiResultEmrSign, item.ID);
                                    if (attachmentURLs != null && attachmentURLs.Count > 0)
                                    {
                                        count = 1;
                                        foreach (var aURL in attachmentURLs)
                                        {
                                            filePath = directoryPath + @"\" + item.CUSTOM_NUM_ORDER + "_" + documentName + "_Đính kèm_" + count + aURL.Substring(aURL.LastIndexOf("."));
                                            streamSource = Inventec.Fss.Client.FileDownload.GetFile(aURL);
                                            streamSource.Position = 0;
                                            InsertPageOne(streamSource, null, filePath, null, 0);
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(versionJSON))
                                {
                                    filePath = directoryPath + @"\" + item.CUSTOM_NUM_ORDER + "_" + documentName + ".json";
                                    using (MemoryStream streamSource = Inventec.Fss.Client.FileDownload.GetFile(versionJSON))
                                    {
                                        streamSource.Position = 0;
                                        using (Stream saveFile = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                                        {
                                            streamSource.CopyTo(saveFile);
                                        }
                                    }

                                }
                                if (!string.IsNullOrEmpty(versionXML))
                                {
                                    filePath = directoryPath + @"\" + item.CUSTOM_NUM_ORDER + "_" + documentName + ".xml";
                                    using (MemoryStream streamSource = Inventec.Fss.Client.FileDownload.GetFile(versionXML))
                                    {
                                        streamSource.Position = 0;
                                        using (Stream saveFile = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                                        {
                                            streamSource.CopyTo(saveFile);
                                        }
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        var documents = GetEmrDocumentFile(null, listDataTrue.Select(o => o.ID).ToList(), chkDowloadGroup.Checked, chkAddPatientSign.Checked, false);
                        if (documents == null || documents.Count() == 0)
                        {
                            MessageManager.Show(ResourceMessage.KhongLayDuocFile);
                            return;
                        }
                        string directoryPath = Path.GetDirectoryName(openFolder.FileName) + @"\" + listDataTrue.FirstOrDefault().TREATMENT_CODE + "_" + listDataTrue.FirstOrDefault().VIR_PATIENT_NAME;
                        if (!System.IO.Directory.Exists(directoryPath))
                        {
                            System.IO.Directory.CreateDirectory(directoryPath);
                        }

                        if (chkDowloadGroup.Checked)
                        {
                            string pdfAddFile = directoryPath + @"\" + documents[0].DocumentName + "." + documents[0].Extension;
                            Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(documents[0].Base64Data))), pdfAddFile);
                        }
                        else
                        {
                            int count = 1;
                            foreach (var item in documents)
                            {
                                string file = directoryPath + @"\" + count + "_" + item.DocumentName + "." + item.Extension;
                                Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(item.Base64Data))), file);
                                count++;
                            }
                        }
                    }
                }
                #region Hien thi message thong bao
                MessageManager.ShowAlert(this, ResourceMessage.ThongBao, ResourceMessage.TaiVeThanhCong);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static BaseFont GetBaseFont()
        {
            string TAHOMA_TFF = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "tahoma.ttf");

            //Create a base font object making sure to specify IDENTITY-H
            return BaseFont.CreateFont(TAHOMA_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

        }

        private static List<int> GetConfigImage()
        {
            List<int> rs = new List<int>();
            try
            {
                var configs = GlobalStore.EmrConfigs;
                var cfgAppearanceOptions = configs.Where(o => o.KEY == "EMR.EMR_SIGN.SIGNATURE_APPEARANCE_OPTION");
                var cfgAppearanceOption = cfgAppearanceOptions != null ? cfgAppearanceOptions.FirstOrDefault() : null;
                if (cfgAppearanceOption != null)
                {
                    try
                    {
                        string vlAppearanceOption = !String.IsNullOrEmpty(cfgAppearanceOption.VALUE) ? cfgAppearanceOption.VALUE : cfgAppearanceOption.DEFAULT_VALUE;
                        //vd: p:0|f:11|w:320|h:140
                        var arrOT = vlAppearanceOption.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrOT != null && arrOT.Count() > 0)
                        {
                            foreach (string vop in arrOT)
                            {
                                if (!String.IsNullOrEmpty(vop))
                                {
                                    var arrOTDetail = vop.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (arrOTDetail != null && arrOTDetail.Count() > 1)
                                    {
                                        if (!String.IsNullOrEmpty(arrOTDetail[1]))
                                        {
                                            string k = arrOTDetail[0].ToLower();
                                            switch (k)
                                            {
                                                case "w":
                                                    int w = Inventec.Common.Integrate.TypeConvertParse.ToInt32(arrOTDetail[1]);
                                                    rs.Add(w);
                                                    break;
                                                case "h":
                                                    int h = Inventec.Common.Integrate.TypeConvertParse.ToInt32(arrOTDetail[1]);
                                                    rs.Add(h);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private static void ProcessInsertPatientSign(PdfReader readerWorking, string outPathFile, long documentId, List<EMR_SIGN> signAlls)
        {
            try
            {
                using (FileStream fs_ = File.Open(outPathFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (PdfStamper stam = new PdfStamper(readerWorking, fs_))
                    {
                        var signElectronics = (signAlls != null && signAlls.Count > 0) ? signAlls.Where(o => o.IS_SIGN_ELECTRONIC == 1 && o.COOR_X_RECTANGLE > 0 && o.COOR_Y_RECTANGLE > 0 && o.DOCUMENT_ID == documentId).ToList() : null;
                        if (signElectronics != null && signElectronics.Count > 0)
                        {
                            foreach (var itemsignElectronic in signElectronics)
                            {
                                int pageNum = (int)(itemsignElectronic.PAGE_NUMBER ?? 1);
                                PdfContentByte cbo = stam.GetOverContent(pageNum);
                                cbo.SetColorFill(iTextSharp.text.BaseColor.BLACK);
                                cbo.SetFontAndSize(GetBaseFont(), 12);
                                cbo.BeginText();
                                if (itemsignElectronic.SIGN_IMAGE != null)
                                {
                                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(itemsignElectronic.SIGN_IMAGE);
                                    image.SetAbsolutePosition((float)itemsignElectronic.COOR_X_RECTANGLE, (float)itemsignElectronic.COOR_Y_RECTANGLE);
                                    var configImage = GetConfigImage();
                                    if (configImage != null && configImage.Count() == 2)
                                    {
                                        float SignaltureImageWidth = 0;
                                        image.WidthPercentage = SharedUtils.CalculateWidthPercent(configImage[0], configImage[1], image, SignaltureImageWidth, 100, SignPdfAsynchronous.ProcessHeightPlus(100, configImage[0]));
                                    }
                                    var signStr = !string.IsNullOrEmpty(itemsignElectronic.RELATION_PEOPLE_NAME) ? string.Format("{0}({1})", itemsignElectronic.RELATION_PEOPLE_NAME, itemsignElectronic.RELATION_NAME) : itemsignElectronic.VIR_PATIENT_NAME;
                                    var document = listDataTrueStatic.FirstOrDefault(o => o.ID == documentId);
                                    if (document != null)
                                    {
                                        switch (document.PATIENT_SIGNATURE_DISPLAY_TYPE)
                                        {
                                            case 0:
                                                break;
                                            case 1:
                                                cbo.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0} đã ký", signStr, ""), (float)itemsignElectronic.COOR_X_RECTANGLE + (image.Width / 4), (float)itemsignElectronic.COOR_Y_RECTANGLE - (image.Height / 2), 0f);
                                                break;
                                            case 2:
                                                cbo.AddImage(image);
                                                break;
                                            default:
                                                cbo.AddImage(image);
                                                cbo.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0} đã ký", signStr, ""), (float)itemsignElectronic.COOR_X_RECTANGLE + (image.Width / 4), (float)itemsignElectronic.COOR_Y_RECTANGLE - (image.Height / 2), 0f);
                                                break;
                                        }
                                    }
                                }
                                else if (itemsignElectronic != null && itemsignElectronic.COOR_X_RECTANGLE.HasValue && itemsignElectronic.COOR_Y_RECTANGLE.HasValue)
                                {
                                    var signStr = !string.IsNullOrEmpty(itemsignElectronic.RELATION_PEOPLE_NAME) ? string.Format("{0}({1})", itemsignElectronic.RELATION_PEOPLE_NAME, itemsignElectronic.RELATION_NAME) : itemsignElectronic.VIR_PATIENT_NAME;
                                    cbo.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0} đã ký", signStr, ""), (float)itemsignElectronic.COOR_X_RECTANGLE, (float)itemsignElectronic.COOR_Y_RECTANGLE, 0f);
                                }

                                cbo.EndText();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal static void InsertPageOne(Stream sourceFile, string streamSourceStr, string desFileJoined, List<EMR_SIGN> signAlls, long documentId)
        {
            iTextSharp.text.pdf.PdfReader reader1 = null;
            if (signAlls != null && signAlls.Count > 0)
            {
                if (sourceFile != null)
                {
                    reader1 = new PdfReader(sourceFile);
                }
                if (!string.IsNullOrEmpty(streamSourceStr))
                {
                    reader1 = new PdfReader(streamSourceStr);
                }
                ProcessInsertPatientSign(reader1, desFileJoined, documentId, signAlls);
            }
            else if (sourceFile != null)
            {
                using (Stream saveFile = File.Open(desFileJoined, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    sourceFile.CopyTo(saveFile);
                }
            }
            else if (!string.IsNullOrEmpty(streamSourceStr))
            {
                reader1 = new PdfReader(streamSourceStr);
                ProcessInsertPatientSign(reader1, desFileJoined, documentId, signAlls);
            }
            try
            {
                if (reader1 != null)
                    reader1.Close();
            }
            catch { }
        }

        private void treeListDocument_LeftCoordChanged(object sender, EventArgs e)
        {
            try
            {
                TreeList view = sender as TreeList;
                if (view.LeftCoord >= 33)
                {
                    pictureEdit1.Visible = false;
                }
                else
                {
                    pictureEdit1.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        string code = txtTreatmentCode.Text.Trim();
                        if (code.Length < 12)
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            txtTreatmentCode.Text = code;
                        }
                    }
                    FillDatagctFormList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMergeDoc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_MERGE_DOC && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkMergeDoc.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_MERGE_DOC;
                    csAddOrUpdate.VALUE = (chkMergeDoc.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
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

        private void gridViewSign_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_EMR_SIGN pData = (V_EMR_SIGN)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (curentEmrDocument != null)
                    {
                        if (curentEmrDocument.SIGNERS != null)
                        {
                            if (e.Column.FieldName != "STT")
                            {
                                if (pData.PCA_SERIAL == null)
                                    e.Appearance.ForeColor = Color.Red;
                                else
                                    e.Appearance.ForeColor = Color.Blue;
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                SetStateButtonDelete();
                if (btnDelete.Enabled)
                {
                    string message = ResourceMessage.ThongBaoCoMuonXoaCacDuLieuDaChon;
                    if (MessageBox.Show(message, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ProcessDelete();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDelete()
        {
            try
            {
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<long> listEmrDocumentId = new List<long>();
                if (this.listDataTrue != null)
                {
                    listEmrDocumentId = this.listDataTrue.Select(o => o.ID).ToList();
                }

                bool apiResult = new BackendAdapter(param).Post<bool>("api/EmrDocument/DeleteList", ApiConsumers.EmrConsumer, listEmrDocumentId, param);
                if (apiResult)
                {
                    FillDatagctFormList();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, apiResult);
                #endregion

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

        private void Reload(bool obj)
        {
            try
            {
                checkreason = obj;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDowloadGroup_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkDowloadGroup.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkDowloadGroup.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkDowloadGroup.Name;
                    csAddOrUpdate.VALUE = (chkDowloadGroup.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
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

        private void gridViewSign_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Icon_SignCancel")
                    {
                        var signTime = (gridViewSign.GetRowCellValue(e.RowHandle, "SIGN_TIME") ?? "").ToString();
                        var loginName = (gridViewSign.GetRowCellValue(e.RowHandle, "LOGINNAME") ?? "").ToString();
                        if (!string.IsNullOrEmpty(signTime) && (string.IsNullOrEmpty(loginName) || loginName.Equals(loginName)))
                            e.RepositoryItem = repCancelSign;
                        else
                            e.RepositoryItem = repNull;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repCancelSign_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (V_EMR_SIGN)gridViewSign.GetFocusedRow();

                if (!string.IsNullOrEmpty(rowData.LOGINNAME))
                    GetReason(null);
                else
                {
                    frmSignCancel frm = new frmSignCancel(GetReason);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetReason(string reason)
        {
            try
            {
                var rowData = (V_EMR_SIGN)gridViewSign.GetFocusedRow();
                CommonParam param = new CommonParam();
                EMR.SDO.SignCancelSDO data = new EMR.SDO.SignCancelSDO();
                data.SignId = rowData.ID;
                data.CancelReason = reason;
                var resultData = new BackendAdapter(param).Post<bool>("api/EmrSign/Cancel", ApiConsumers.EmrConsumer, data, param);
                if (resultData)
                {
                    EmrDocumentADO rowEmrDocumentData = (EmrDocumentADO)treeListDocument.GetDataRecordByNode(treeListDocument.FocusedNode);
                    loadViewSign(rowEmrDocumentData.ID);
                }
                #region Hien thi message thong bao
                MessageManager.Show(this, param, resultData);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMerge_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_MERGE && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkMerge.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_MERGE;
                    csAddOrUpdate.VALUE = (chkMerge.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
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

        private void chkNotFillZero_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkNotFillZero.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkNotFillZero.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkNotFillZero.Name;
                    csAddOrUpdate.VALUE = (chkNotFillZero.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
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

        private void chkAddPatientSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAddPatientSign.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAddPatientSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAddPatientSign.Name;
                    csAddOrUpdate.VALUE = (chkAddPatientSign.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
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

        private void cboStatusPatientSign_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboStatusPatientSign.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repEdit_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "EMR.Desktop.Plugins.DocumentEdit").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = EMR.Desktop.Plugins.DocumentEdit");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    var rowData = treeListDocument.GetDataRecordByNode(treeListDocument.FocusedNode) as EmrDocumentADO;
                    List<object> listArgs = new List<object>();
                    listArgs.Add((RefeshReference)FillDatagctFormList);
                    listArgs.Add(rowData.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PatientAndHomeRelatetiveSign(bool IsPatientSign, bool IsHomeRelativeSign)
        {
            try
            {
                var lstWarn = listDataTrue.Where(data => !string.IsNullOrEmpty(data.SIGNERS) && data.SIGNERS.Contains("#@!@#" + data.PATIENT_CODE)).ToList();
                if (lstWarn != null && lstWarn.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Bệnh nhân/Người nhà đã ký văn bản {0}.", string.Join(", ", lstWarn.Select(o => string.Format("{0}({1})", o.DOCUMENT_NAME, o.DOCUMENT_CODE)))), Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK);
                    return;
                }
                EMR.Filter.EmrRelationFilter filter = new EMR.Filter.EmrRelationFilter();
                var relationDatas = new BackendAdapter(new CommonParam()).Get<List<EMR_RELATION>>("api/EmrRelation/Get", ApiConsumer.ApiConsumers.EmrConsumer, filter, null);
                string RelationPeopleName = null;
                EMR_RELATION Relation = null;
                int index = 0;
                bool IsReloadSuccess = false;
                foreach (var item in listDataTrue)
                {
                    index++;
                    bool IsBreak = false;
                    if (IsHomeRelativeSign && string.IsNullOrEmpty(RelationPeopleName) && Relation == null)
                    {
                        frmHomeRelativeSign frm = new frmHomeRelativeSign(relationDatas, name => RelationPeopleName = name, relative => Relation = relative, closed => IsBreak = !closed);
                        frm.ShowDialog();
                    }
                    if (IsBreak)
                        break;

                    var file = GetEmrDocumentFile(item, null, null, null, null);
                    if (file == null || file.Count == 0)
                        continue;

                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADO(item.TREATMENT_CODE, item.DOCUMENT_CODE, item.DOCUMENT_NAME, currentModule.RoomId);
                    if (item.WIDTH != null && item.HEIGHT != null && item.RAW_KIND != null)
                    {
                        inputADO.PaperSizeDefault = new System.Drawing.Printing.PaperSize(item.PAPER_NAME, (int)item.WIDTH, (int)item.HEIGHT);
                        if (item.RAW_KIND != null)
                        {
                            inputADO.PaperSizeDefault.RawKind = (int)item.RAW_KIND;
                        }
                    }

                    inputADO.DTI = ConfigSystems.URI_API_ACS + "|" + ConfigSystems.URI_API_EMR + "|" + ConfigSystems.URI_API_FSS;
                    inputADO.DTI = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", ConfigSystems.URI_API_ACS, ConfigSystems.URI_API_EMR, ConfigSystems.URI_API_FSS, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().TokenCode, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());
                    inputADO.IsSave = false;
                    inputADO.IsSign = true;//set true nếu cần gọi ký
                    inputADO.IsPatientSign = IsPatientSign;
                    inputADO.IsHomeRelativeSign = IsHomeRelativeSign;
                    inputADO.IsReject = false;
                    inputADO.IsPrint = false;
                    inputADO.IsExport = false;
                    inputADO.Relation = Relation;
                    inputADO.RelationPeopleName = RelationPeopleName;
                    inputADO.Treatment = new Inventec.Common.SignLibrary.DTO.TreatmentDTO();
                    inputADO.Treatment.TREATMENT_CODE = item.TREATMENT_CODE;//mã hồ sơ điều trị
                    inputADO.DocumentCode = item.DOCUMENT_CODE;
                    inputADO.DocumentName = item.DOCUMENT_NAME;
                    inputADO.IsRemoveSignPadBefore = index == listDataTrue.Count;
                    var dto = libraryProcessor.SignNow(file[0].Base64Data, FileType.Pdf, inputADO);
                    if (dto != null && dto.Success)
                    {
                        IsReloadSuccess = true;
                        MessageManager.Show(this, new CommonParam(), dto != null && dto.Success);
                    }
                }
                if (IsReloadSuccess)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPatientSign_Click(object sender, EventArgs e)
        {
            PatientAndHomeRelatetiveSign(true, false);
        }

        private void btnHomeRelativeSign_Click(object sender, EventArgs e)
        {
            PatientAndHomeRelatetiveSign(false, true);
        }

        private void treeListDocument_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IsChecked")
                {
                    UpdateListDataTrue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UpdateListDataTrue()
        {
            try
            {
                listDataTrue = new List<EmrDocumentADO>();

                if (this.listData != null && this.listData.Count > 0)
                {
                    listDataTrue = this.listData
                        .Where(o => o.IsChecked == true && o.ID > 0)
                        .OrderBy(p => p.CUSTOM_NUM_ORDER)
                        .ToList();
                }

                bool hasValidData = listDataTrue != null && listDataTrue.Count > 0;

                this.btnPrint.Enabled = hasValidData && controlAcs?.FirstOrDefault(o => o.CONTROL_CODE == "EMR000002") != null;
                this.btnDownload.Enabled = hasValidData && controlAcs?.FirstOrDefault(o => o.CONTROL_CODE == "EMR000003") != null;
                this.btnHomeRelativeSign.Enabled = hasValidData;
                this.btnPatientSign.Enabled = hasValidData;

                SetStateButtonDelete();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
