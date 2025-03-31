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
using HIS.Desktop.LocalStorage.LocalData;
using MOS.Filter;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.TreatmentFinish;
using HIS.UC.TreatmentFinish.ADO;
using HIS.UC.TreatmentFinish.Run;
using HIS.UC.TreatmentFinish.Reload;
using MOS.SDO;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ADO;
using HIS.UC.TreatmentFinish.CloseTreatment;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data;
using DevExpress.XtraEditors;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using System.Resources;
using Inventec.Common.Logging;

namespace HIS.UC.TreatmentFinish.Run
{
    public partial class UCTreatmentFinish : UserControl
    {
        #region Declare

        LanguageInputADO languageInputADO;
        TreatmentFinishInitADO treatmentFinishInitADO = new TreatmentFinishInitADO();
        HisTreatmentWithPatientTypeInfoSDO Treatment { get; set; }
        AutoTreatmentFinish__Checked autoTreatmentFinish__Checked;
        Action<bool> DelegateTreatmentFinishCheckChange;
        DelegateGetDateADO getDateADO;
        DataInputADO dataInputADO;
        CheckedTreatmentFinish checkedTreatmentFinish;
        bool IsCheckFinishTime;
        bool isCheckBedService;
        bool mustFinishAllServicesBeforeFinishTreatment;
        List<long> autoFinishServiceIds;
        HisTreatmentFinishSDO treatmentFinishSDO { get; set; }
        bool? isAutoWidth { get; set; }
        bool? useCapSoBABNCT;
        int width { get; set; }
        int height { get; set; }
        bool isInitForm = true;
        bool isFirstLoadData = true;
        long TreatmentEndAppointmentTimeDefault;
        bool TreatmentEndHasAppointmentTimeDefault;
        bool isTreatmentIn;
        bool notAutoInitData;
        int positionHandle = -1;
        bool IsShowButtonIcd;
        TreatmentEndTypeExtProcessor treatmentEndTypeExtProcessor;
        List<HIS_TREATMENT_END_TYPE_EXT> TreatmentEndTypeExts;
        TreatmentEndTypeExtData currentTreatmentEndTypeExt { get; set; }
        long? treatmentId;
        bool isLoadTreatmentInFormTreatmentEndTypeExt; //Load tu ket thuc dieu tri. nghi om, nghi duong thai
        List<V_HIS_PATIENT_PROGRAM> _HisPatientPrograms;
        List<HIS_PROGRAM> _HisPrograms;
        List<long> programIds;
        decimal useDay;
        CreateEMRVBAOnClick dlgCreateEMRVBAOnClick;
        DelegateGetStoreStateValue dlgGetStoreStateValue;
        DelegateSetStoreStateValue dlgSetStoreStateValue;
        bool IsBlockOrder;
        IcdTemp currentIcd = new IcdTemp();
        HIS_TREATMENT HisTreatment = new HIS_TREATMENT();
        private List<AcsUserADO> lstReAcsUserADO;
        /// <summary>
        ///  Cấu hình cho phép mở nhiều hồ sơ điều trị của cùng 1 bệnh nhân hay không.
        ///0: Không cho phép
        ///1: Cho phép.
        ///2: Chỉ cho phép nếu hồ sơ đang mở thuộc chi nhánh khác.
        ///3: Không giới hạn với hồ sơ không phải BHYT.Với hồ sơ BHYT, thì bệnh nhân BHYT chỉ được tạo 1 hồ sơ điều trị ""không phải cấp cứu"", các hồ sơ còn lại bắt buộc phải check ""Là cấp cứu""
        ///4: 
        /// </summary>
        private enum AllowManyTreatmentOpeningOption
        {
            Option1 = 1,
            Option2 = 2,
            Option3 = 3,
            Option4 = 4
        }

        System.Windows.Forms.Timer timerInitForm;

        #endregion

        #region Contructor
        public UCTreatmentFinish()
            : this(null)
        {
        }

        public UCTreatmentFinish(TreatmentFinishInitADO data)
        {
            LogSystem.Debug("UCTreatmentFinish. 1");
            InitializeComponent();
            try
            {
                this.treatmentFinishInitADO = data;
                if (data != null)
                {
                    this.Treatment = data.Treatment;
                    this.isTreatmentIn = data.IsTreatmentIn;
                    this.languageInputADO = data.LanguageInputADO;
                    this.isAutoWidth = data.IsAutoWidth;
                    this.dataInputADO = data.DataInputADO;
                    this.IsCheckFinishTime = data.IsCheckFinishTime;
                    this.isCheckBedService = data.IsCheckBedService;
                    this.useCapSoBABNCT = data.UseCapSoBABNCT;
                    this.mustFinishAllServicesBeforeFinishTreatment = data.MustFinishAllServicesBeforeFinishTreatment;
                    this.autoFinishServiceIds = data.AutoFinishServiceIds;
                    this.getDateADO = data.DelegateGetDateADO;
                    this.TreatmentEndAppointmentTimeDefault = data.TreatmentEndAppointmentTimeDefault;
                    this.TreatmentEndHasAppointmentTimeDefault = data.TreatmentEndHasAppointmentTimeDefault;
                    this.checkedTreatmentFinish = data.DelegateCheckedTreatmentFinish;
                    this.DelegateTreatmentFinishCheckChange = data.DelegateTreatmentFinishCheckChange;
                    this.dlgGetStoreStateValue = data.DelegateGetStoreStateValue;
                    this.dlgSetStoreStateValue = data.DelegateSetStoreStateValue;
                    this.notAutoInitData = (data.NotAutoInitData.HasValue && data.NotAutoInitData.Value);
                    if (this.dataInputADO == null)
                    {
                        this.dataInputADO = new DataInputADO();
                    }
                    this.autoTreatmentFinish__Checked = data.AutoTreatmentFinish__Checked;
                    this.treatmentId = data.treatmentId;
                    this.IsBlockOrder = data.IsBlockOrder;
                    this.IsShowButtonIcd = data.IsShowButtonIcd;
                }
                //SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
                LogSystem.Debug("UCTreatmentFinish. 2");
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
                gBoxTreatmentFinishInfo.Text = this.languageInputADO.gBoxTreatmentFinishInfo__Text;
                lciFordtEndTime.Text = this.languageInputADO.lciFordtEndTime__Text;
                lciTreatmentEndType.Text = this.languageInputADO.lciTreatmentEndType__Text;
                lciForchkAutoPrintGHK.Text = this.languageInputADO.lciForchkAutoPrintGHK__Text;
                lciForchkAutoBK.Text = this.languageInputADO.lciForchkAutoBK__Text;
                licPrintNHBHXH.Text = this.languageInputADO.licPrintNHBHXH_Text;
                licPrintTrichLuc.Text = this.languageInputADO.licPrintTrichLuc_Text;
                lciHisCareer.Text = this.languageInputADO.lciHisCareer__Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCTreatmentFinish
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.TreatmentFinish.Resources.Lang", typeof(UCTreatmentFinish).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gBoxTreatmentFinishInfo.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.gBoxTreatmentFinishInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlEditor.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlEditor.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCareer.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.cboCareer.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkXuatXML.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkXuatXML.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignTL.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkSignTL.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintTL.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkPrintTL.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnShowIcd.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.btnShowIcd.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignBHXH.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkSignBHXH.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintBHXH.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkPrintBHXH.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintBANT.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkPrintBANT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignBK.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkSignBK.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignGHK.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkSignGHK.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboProgram.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.cboProgram.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblNeedSickLeaveCert.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lblNeedSickLeaveCert.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentEndTypeExt.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.cboTreatmentEndTypeExt.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIssueOutPatientStoreCode.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkIssueOutPatientStoreCode.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //toolTipItem2.Text = Inventec.Common.Resource.Get.Value("toolTipItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIssueOutPatientStoreCode.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkIssueOutPatientStoreCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoPrintGHK.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkAutoPrintGHK.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoPrintBK.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkAutoPrintBK.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentEndType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.cboTreatmentEndType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFordtEndTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciFordtEndTime.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFordtEndTime.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciFordtEndTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentEndType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciTreatmentEndType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentEndType.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciTreatmentEndType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForchkAutoPrintGHK.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciForchkAutoPrintGHK.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForchkAutoPrintGHK.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciForchkAutoPrintGHK.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem3.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForcboProgram.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciForcboProgram.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForchkAutoBK.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciForchkAutoBK.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForchkAutoBK.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciForchkAutoBK.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem2.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForlblSoLuuTruBANT.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciForlblSoLuuTruBANT.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForlblSoLuuTruBANT.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciForlblSoLuuTruBANT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.licPrintNHBHXH.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.licPrintNHBHXH.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.licPrintNHBHXH.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.licPrintNHBHXH.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.licPrintTrichLuc.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.licPrintTrichLuc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciXuatXML.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciXuatXML.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHisCareer.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciHisCareer.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoTreatmentFinish.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkAutoTreatmentFinish.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event
        private async void UCTreatmentFinish_Load(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("UCTreatmentFinish_Load. 1");
                ConfigKeyCFG.GetConfig();
                ValidateForm();
                LogSystem.Debug("UCTreatmentFinish_Load. 2");
                if (ConfigKeyCFG.AutoCheckAndDisableExportXmlCollinear)
                {
                    lciXuatXML.Enabled = false;
                    chkXuatXML.Enabled = false;
                    chkXuatXML.CheckState = CheckState.Checked;
                }
                else
                {
                    lciXuatXML.Enabled = true;
                    chkXuatXML.Enabled = true;
                }

                dtEndTime.Enabled = txtTreatmentEndTypeCode.Enabled
                    = cboTreatmentEndType.Enabled
                    = chkAutoPrintGHK.Enabled
                    = chkSignGHK.Enabled
                    = chkAutoPrintBK.Enabled
                    = chkSignBK.Enabled
                    = chkIssueOutPatientStoreCode.Enabled
                    = chkPrintBANT.Enabled
                    = cboTreatmentEndTypeExt.Enabled
                    = chkPrintBHXH.Enabled
                    = chkSignBHXH.Enabled
                    = false;
                LogSystem.Debug("UCTreatmentFinish_Load. 3");

                if (this.notAutoInitData)
                {

                    this.timerInitForm = new System.Windows.Forms.Timer();
                    this.timerInitForm.Tick += new System.EventHandler(this.timerInitForm_Tick);
                    this.timerInitForm.Interval = 500;//Fix 5s
                    this.timerInitForm.Enabled = true;
                    this.timerInitForm.Start();

                    Inventec.Common.Logging.LogSystem.Debug("UcTreatmentFinish.UCTreatmentFinish_Load. Khong tai cac du lieu & khoi tao du lieu default tren giao dien, khi check vao o check chon thi moi tai du lieu____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => notAutoInitData), notAutoInitData));
                    return;
                }
                else
                {
                    await this.InitDataTocboUser();
                }

                if (this.IsShowButtonIcd)
                {
                    this.LayoutShowIcd.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    this.LayoutShowIcd.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                LogSystem.Debug("UCTreatmentFinish_Load. 4");


                this.InitTreatmentEndType();

                this.ValidHeadDepartmentAndDirectorBranch();

                LogSystem.Debug("UCTreatmentFinish_Load. 5");
                this.InitTreatmentEndTypeExt();
                //InitCapSoLuuTruBNCT();
                LogSystem.Debug("UCTreatmentFinish_Load. 6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void timerInitForm_Tick(object sender, EventArgs e)
        {
            try
            {
                timerInitForm.Stop();
                this.InitDataTocboUser();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void InitCapSoLuuTruBNCT()
        {
            try
            {
                if (this.Treatment == null || String.IsNullOrEmpty(this.Treatment.TREATMENT_CODE))
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                    filter.TREATMENT_ID = treatmentId ?? 0;
                    if (this.getDateADO != null)
                    {
                        this.dataInputADO = this.getDateADO();
                        if (this.dataInputADO != null)
                            filter.INTRUCTION_TIME = this.dataInputADO.UseTime;
                    }

                    this.Treatment = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                }

                SetEnableCheckSoLuuTruBANTByConfig();

                if (!chkIssueOutPatientStoreCode.Checked)
                {
                    lciForlblSoLuuTruBANT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciForcboProgram.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                ProcessStoreCodeDisplay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void SetEnableCheckSoLuuTruBANTByConfig()
        {
            try
            {
                string defaultCheckedCheckboxIssueOutPatientStoreCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TreatmentFinish.DefaultCheckedCheckboxCreateOutPatientMediRecord");
                chkIssueOutPatientStoreCode.Checked = defaultCheckedCheckboxIssueOutPatientStoreCode == "1";

                string enableCheckboxIssueOutPatientStoreCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TreatmentFinish.EnableCheckboxCreateOutPatientMediRecord");
                chkIssueOutPatientStoreCode.Enabled = (enableCheckboxIssueOutPatientStoreCode == "1");
                chkIssueOutPatientStoreCode.ReadOnly = !(enableCheckboxIssueOutPatientStoreCode == "1");
                layoutControlItem2.Enabled = (enableCheckboxIssueOutPatientStoreCode == "1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private async Task ProcessProgramm()
        {
            try
            {
                lciForlblSoLuuTruBANT.Visibility = chkIssueOutPatientStoreCode.Checked ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciForcboProgram.Visibility = chkIssueOutPatientStoreCode.Checked ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                HisPatientProgramViewFilter patientProgramFilter = new HisPatientProgramViewFilter();
                patientProgramFilter.PATIENT_ID = Treatment.PATIENT_ID;
                var patientPrograms = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_PROGRAM>>("api/HisPatientProgram/Get", ApiConsumers.MosConsumer, patientProgramFilter, null);
                this._HisPrograms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PROGRAM>().Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue).OrderBy(o => o.PROGRAM_CODE).ToList();
                var dataStores = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_DATA_STORE>();
                var dataStoreIdAllows = (dataStores != null && dataStores.Count > 0) ? dataStores.Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()).Select(o => o.ID) : null;
                this._HisPrograms = this._HisPrograms.Where(o => (o.DATA_STORE_ID == null || (o.DATA_STORE_ID != null && dataStoreIdAllows != null && dataStoreIdAllows.Contains(o.DATA_STORE_ID ?? 0))) && (o.TREATMENT_TYPE_ID == this.Treatment.TDL_TREATMENT_TYPE_ID || o.TREATMENT_TYPE_ID == null)).OrderBy(o => o.PROGRAM_CODE).ToList();

                this._HisPatientPrograms = new List<V_HIS_PATIENT_PROGRAM>();
                var patientProgramsFilter = patientPrograms != null && patientPrograms.Count > 0 ? (
                    from m in patientPrograms
                    from n in _HisPrograms
                    where m.PROGRAM_ID == n.ID
                    select new V_HIS_PATIENT_PROGRAM()
                    {
                        ID = m.ID,
                        PATIENT_ID = m.PATIENT_ID,
                        PATIENT_PROGRAM_CODE = m.PATIENT_PROGRAM_CODE,
                        PROGRAM_ID = m.PROGRAM_ID,
                        PROGRAM_CODE = n.PROGRAM_CODE,
                        PROGRAM_NAME = n.PROGRAM_NAME
                    }).ToList() : null;

                if (patientProgramsFilter != null && patientProgramsFilter.Count > 0)
                    this._HisPatientPrograms.AddRange(patientProgramsFilter);

                this.programIds = patientPrograms != null ? patientPrograms.Select(o => o.PROGRAM_ID).ToList() : null;

                if (!chkIssueOutPatientStoreCode.Checked)
                {
                    cboProgram.EditValue = null;
                    dxValidationProvider1.RemoveControlError(cboProgram);
                    dxValidationProvider1.SetValidationRule(cboProgram, null);
                    lciForcboProgram.AppearanceItemCaption.ForeColor = Color.Black;
                }

                foreach (var pr in this._HisPrograms)
                {
                    //this.programIds == null || !this.programIds.Contains(pr.ID) || pr.TREATMENT_TYPE_ID == null
                    if (this.programIds == null || !this.programIds.Contains(pr.ID))
                    {
                        V_HIS_PATIENT_PROGRAM pgram = new V_HIS_PATIENT_PROGRAM()
                        {
                            PROGRAM_ID = pr.ID,
                            PROGRAM_CODE = pr.PROGRAM_CODE,
                            PROGRAM_NAME = pr.PROGRAM_NAME,
                        };
                        //var dem = this._HisPatientPrograms.Where(o => o.PROGRAM_CODE.Contains(pgram.PROGRAM_CODE) && o.PROGRAM_NAME.Contains(pgram.PROGRAM_NAME)).ToList();
                        //if (dem == null || dem.Count <= 0)
                        //{
                        this._HisPatientPrograms.Add(pgram);
                        //}
                    }
                }
                this._HisPatientPrograms = this._HisPatientPrograms.OrderByDescending(o => o.PROGRAM_CODE).ToList();


                InitComboCommon(cboProgram, this._HisPatientPrograms, "PROGRAM_ID", "PROGRAM_NAME", "PROGRAM_CODE");

                if (chkIssueOutPatientStoreCode.Checked)
                {
                    var programFiltered = this.programIds != null ? this._HisPrograms.Where(o => this.programIds.Contains(o.ID)).ToList() : null;
                    if (programFiltered != null && programFiltered.Count == 1)
                    {
                        cboProgram.EditValue = programFiltered[0].ID;
                    }

                    ValidationSingleControl(cboProgram);
                    lciForcboProgram.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                InitComboCommon(cboProgram, this._HisPatientPrograms, "PROGRAM_ID", "PROGRAM_NAME", "PROGRAM_CODE");

                if (programIds != null && programIds.Count > 0)
                {
                    gridView2.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(gridLookUpEdit1View_RowCellStyle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private async Task ProcessStoreCodeDisplay()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_MEDI_RECORD> mediRecords = null;
                if (Treatment != null && Treatment.MEDI_RECORD_ID > 0)
                {
                    Inventec.Core.CommonParam paramCommon = new Inventec.Core.CommonParam();
                    HisMediRecordFilter mediRecordFilter = new HisMediRecordFilter();
                    mediRecordFilter.ID = Treatment.MEDI_RECORD_ID;
                    mediRecords = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_MEDI_RECORD>>("/api/HisMediRecord/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, mediRecordFilter, paramCommon);

                    lblSoLuuTruBANT.Text = mediRecords != null && mediRecords.Count > 0 ? mediRecords[0].STORE_CODE : "";
                }
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Treatment.MEDI_RECORD_ID), Treatment.MEDI_RECORD_ID) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediRecords), mediRecords));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        void gridLookUpEdit1View_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    long programId = Convert.ToInt64(View.GetRowCellValue(e.RowHandle, "PROGRAM_ID"));
                    if (this.programIds != null && this.programIds.Contains(programId))
                        e.Appearance.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboProgram_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboProgram.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboProgram_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboProgram.Properties.Buttons[1].Visible = (cboProgram.EditValue != null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentEndTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtTreatmentEndTypeCode.Text.Trim()))
                    {
                        string code = txtTreatmentEndTypeCode.Text.Trim();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE>().Where(o => o.TREATMENT_END_TYPE_CODE.Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TREATMENT_END_TYPE_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtTreatmentEndTypeCode.Text = result.First().TREATMENT_END_TYPE_CODE;
                            cboTreatmentEndType.EditValue = result.First().ID;
                            TreatmentEndTypeProcess(result.First());
                            SendKeys.Send("{TAB}");
                        }
                    }
                    if (showCbo)
                    {
                        cboTreatmentEndType.Focus();
                        cboTreatmentEndType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentEndType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboTreatmentEndType.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtTreatmentEndTypeCode.Text = data.TREATMENT_END_TYPE_CODE;
                        TreatmentEndTypeProcess(data);
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentEndType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTreatmentEndType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtTreatmentEndTypeCode.Text = data.TREATMENT_END_TYPE_CODE;
                            TreatmentEndTypeProcess(data);
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down) cboTreatmentEndType.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool IsCheckAutoTreatmentFinish;
        private void chkAutoTreatmentFinish_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                IsCheckAutoTreatmentFinish = chkAutoTreatmentFinish.Checked;
                AutoTreatmentFinishCheckedChanged();
                IsCheckAutoTreatmentFinish = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtEndTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtTreatmentEndTypeCode.Focus();
                    txtTreatmentEndTypeCode.SelectAll();
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtEndTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTreatmentEndTypeCode.Focus();
                    txtTreatmentEndTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtEndTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (cboTreatmentEndType.EditValue != null)
                //{
                //    long treatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndType.EditValue.ToString());
                //    if (treatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                //    {
                //        if (dtEndTime != null && dtEndTime.EditValue != null && dtEndTime.EditorContainsFocus)
                //        {
                //            dtAppointmentTime.DateTime = dtEndTime.DateTime.AddDays((double)(spinApoinmentNumDay.Value));
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckFinishTime()
        {
            try
            {
                if (IsCheckFinishTime)
                {
                    if (dtEndTime.EditValue == null || dtEndTime.DateTime == DateTime.MinValue)
                        return;
                    var finishTime = Convert.ToInt64(dtEndTime.DateTime.ToString("yyyyMMddHHmmss"));
                    List<HIS_SERVICE_REQ> listServiceReq = null;
                    if (listServiceReq == null || listServiceReq.Count <= 0)
                    {
                        HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                        serviceReqFilter.TREATMENT_ID = this.Treatment.ID;
                        serviceReqFilter.HAS_EXECUTE = true;
                        listServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, null);
                    }
                    if (listServiceReq != null || listServiceReq.Count > 0)
                    {
                        var listData = listServiceReq.Where(o => o.INTRUCTION_TIME > finishTime).ToList();
                        if (listData != null && listData.Count > 0)
                        {
                            var listCode = listData.Select(s => s.SERVICE_REQ_CODE).ToList();
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(languageInputADO.ThoiGianKetThucNhoHoiThoiGianYLenhCuaCacMaYeuCauSau, String.Join(",", listCode)), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DevExpress.Utils.DefaultBoolean.True);
                            dtEndTime.Focus();
                            dtEndTime.SelectAll();
                            throw new Exception("Thoi gian ket thuc nho hon thoi gian y lenh");
                        }
                    }
                    CheckCondition();
                    ProcessCheckBedService();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCheckBedService()
        {
            try
            {
                if (isCheckBedService)
                {
                    if (Treatment != null && (Treatment.IN_TREATMENT_TYPE_ID ?? 0) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)// HisTreatmentTypeCFG.HisTreatmentTypeId__TreatIn)
                    {
                        HisSereServView1Filter ssFilter = new HisSereServView1Filter();
                        ssFilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G;// HisServiceTypeCFG.SERVICE_TYPE_ID__BED;
                        ssFilter.TREATMENT_ID = Treatment.ID;
                        ssFilter.HAS_EXECUTE = true;
                        var listSereServBed = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_1>>("api/HisSereServ/GetView1", ApiConsumers.MosConsumer, ssFilter, null);
                        if (listSereServBed == null || listSereServBed.Count <= 0)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(languageInputADO.BenhNhanNoiTruChuaDuocChiDinhDichVuGiuong, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DevExpress.Utils.DefaultBoolean.True);
                            throw new Exception("Benh nhan noi tru chua duoc chi dinh dich vu giuong.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiem tra cac dieu kien khac truoc khi ket thuc ho so dieu tri
        /// </summary>
        /// <param name="treatmentId"></param>
        private void CheckCondition()
        {
            //Neu co cau hinh "bat buoc ket thuc toan bo dich vu truoc khi ket thuc ho so dieu tri"
            //Va viec ket thuc ko phai la "luu tam thoi"
            if (mustFinishAllServicesBeforeFinishTreatment)
            {
                //Lay ra cac dich vu chua hoan thanh va ko phai bi hoan thu
                HisSereServView1Filter filter = new HisSereServView1Filter();
                filter.TREATMENT_ID = Treatment.ID;
                filter.SERVICE_REQ_STT_IDs = new List<long> {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL};
                //HisServiceReqSttCFG.SERVICE_REQ_STT_ID__INPROCESS,
                //HisServiceReqSttCFG.SERVICE_REQ_STT_ID__NEW};

                //ko check voi cac dich vu giuong, mau, thuoc, vat tu
                filter.NOT_IN_SERVICE_TYPE_IDs = new List<long> {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                    //HisServiceTypeCFG.SERVICE_TYPE_ID__BED,
                    //HisServiceTypeCFG.SERVICE_TYPE_ID__BLOOD,
                    //HisServiceTypeCFG.SERVICE_TYPE_ID__MATE,
                    //HisServiceTypeCFG.SERVICE_TYPE_ID__MEDI,
                };
                if (autoFinishServiceIds != null && autoFinishServiceIds.Count > 0)
                {
                    filter.NOT_IN_SERVICE_IDs = autoFinishServiceIds;
                }
                filter.HAS_EXECUTE = true;

                List<V_HIS_SERE_SERV_1> unFinishs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_1>>("api/HisSereServ/GetView1", ApiConsumers.MosConsumer, filter, null);

                //ko check doi voi thuoc/vat tu, va dich vu kham dang xu ly (trong truong hop ket thuc dieu tri o man hinh xu ly kham)
                List<V_HIS_SERE_SERV_1> check = null;
                if (unFinishs != null)
                {
                    check = unFinishs.Where(o => !o.MEDICINE_ID.HasValue && !o.MATERIAL_ID.HasValue).ToList();

                }

                if (check != null && check.Count > 0)
                {
                    string serviceReqCodes = string.Join(",", unFinishs.Select(o => o.TDL_SERVICE_REQ_CODE).ToList());
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(languageInputADO.CacPhieuChiDinhSauChuaKetThucKhongChoPhepKetThuc, serviceReqCodes), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DevExpress.Utils.DefaultBoolean.True);
                    throw new Exception("CLS chua ket thuc dieu tri");
                }
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentEndType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? 0).ToString()));
                if (data != null)
                {
                    if (data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN && ConfigKeyCFG.AllowManyTreatmentOpeningOption == ((int)AllowManyTreatmentOpeningOption.Option4).ToString() && dataInputADO.IsBhyt)
                    {
                        CommonParam param = new CommonParam();
                        HisTreatmentFilter filter = new HisTreatmentFilter();
                        filter.PATIENT_ID = dataInputADO.patientId;
                        filter.IS_PAUSE = false;
                        filter.ID__NOT_EQUAL = this.treatmentId;
                        var Histreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                        if (Histreatment != null && Histreatment.Count > 0)
                        {
                            Histreatment = Histreatment.Where(o => o.TDL_PATIENT_TYPE_ID == ConfigKeyCFG.PatientTypeId__BHYT && (new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY, IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU, IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU }).Exists(p => p == o.TDL_TREATMENT_TYPE_ID)).ToList();
                            if (Histreatment != null && Histreatment.Count > 0)
                            {
                                XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongChoPhepChuyenVien, string.Join(",", Histreatment.Select(o => o.TREATMENT_CODE))), "Thông báo", MessageBoxButtons.OK);
                                cboTreatmentEndType.EditValue = null;
                                txtTreatmentEndTypeCode.Text = null;
                                return;
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

        private void cboTreatmentEndTypeExt_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboTreatmentEndTypeExt.EditValue != null)
                {
                    HIS_TREATMENT_END_TYPE_EXT treatmentEndTypeExt = TreatmentEndTypeExts != null ? TreatmentEndTypeExts.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString())) : null;

                    if (treatmentEndTypeExt == null)
                        return;

                    long treatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString());
                    if (treatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                    {
                        chkPrintBHXH.Enabled = chkSignBHXH.Enabled = true;

                        if (dlgGetStoreStateValue != null)
                        {
                            string vlStatechkPrintBHXH = this.dlgGetStoreStateValue("KeyStorechkPrintBHXH");
                            if (vlStatechkPrintBHXH == "1" && chkPrintBHXH.CheckState != CheckState.Checked)
                            {
                                chkPrintBHXH.CheckState = CheckState.Checked;
                            }
                            string vlStatechkSignBHXH = this.dlgGetStoreStateValue("KeyStorechkSignBHXH");
                            if (vlStatechkSignBHXH == "1" && chkSignBHXH.CheckState != CheckState.Checked)
                            {
                                chkSignBHXH.CheckState = CheckState.Checked;
                            }
                        }

                    }
                    else
                    {
                        chkPrintBHXH.Enabled = chkSignBHXH.Enabled = false;
                        chkPrintBHXH.CheckState = chkSignBHXH.CheckState = CheckState.Unchecked;
                    }

                    HisTreatmentFinishSDO treatmentFinishSDOExt = new HisTreatmentFinishSDO();
                    treatmentFinishSDOExt.TreatmentFinishTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;
                    treatmentFinishSDOExt.DocumentBookId = treatmentFinishSDO != null ? treatmentFinishSDO.DocumentBookId : null;
                    if (String.IsNullOrEmpty(treatmentFinishSDOExt.SickLoginname))
                    {
                        treatmentFinishSDOExt.SickLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        treatmentFinishSDOExt.SickUsername = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    }

                    treatmentEndTypeExtProcessor = new TreatmentEndTypeExtProcessor(treatmentEndTypeExt, ReloadDataTreatmentEndTypeExt);
                    List<object> args = new List<object>();
                    args.Add(this.treatmentId);
                    if (isLoadTreatmentInFormTreatmentEndTypeExt)
                        args.Add(GetTreatmentEndTypeExt());
                    args.Add(treatmentFinishSDOExt);

                    Inventec.Common.Logging.LogSystem.Debug("cboTreatmentEndTypeExt_Closed____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isLoadTreatmentInFormTreatmentEndTypeExt), isLoadTreatmentInFormTreatmentEndTypeExt));

                    treatmentEndTypeExtProcessor.Run(args);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private TreatmentEndTypeExtData GetTreatmentEndTypeExt()
        {
            TreatmentEndTypeExtData data = null;
            try
            {
                if (this.treatmentFinishSDO != null &&
                    (this.treatmentFinishSDO.SickLeaveDay.HasValue
                    || !String.IsNullOrEmpty(this.treatmentFinishSDO.PatientRelativeName)
                    || !String.IsNullOrEmpty(this.treatmentFinishSDO.PatientRelativeType)
                    || !String.IsNullOrEmpty(this.treatmentFinishSDO.PatientWorkPlace)
                    || (this.treatmentFinishSDO.WorkPlaceId.HasValue && this.treatmentFinishSDO.WorkPlaceId.Value > 0)
                    || this.treatmentFinishSDO.SickLeaveFrom.HasValue
                    || this.treatmentFinishSDO.SickLeaveTo.HasValue
                    || (this.treatmentFinishSDO.Babies != null && this.treatmentFinishSDO.Babies.Count > 0)
                    || this.treatmentFinishSDO.DocumentBookId.HasValue
                    || !String.IsNullOrEmpty(this.treatmentFinishSDO.SickLoginname)
                    || this.treatmentFinishSDO.SurgeryAppointmentTime.HasValue
                    || !String.IsNullOrEmpty(this.treatmentFinishSDO.Advise)
                    || !String.IsNullOrEmpty(this.treatmentFinishSDO.AppointmentSurgery)
                    || this.treatmentFinishSDO.IsPregnancyTermination.HasValue
                    || !String.IsNullOrEmpty(this.treatmentFinishSDO.PregnancyTerminationReason)
                    || this.treatmentFinishSDO.GestationalAge.HasValue
                    || !String.IsNullOrEmpty(this.treatmentFinishSDO.TreatmentMethod)
                    ))
                {
                    data = new TreatmentEndTypeExtData();
                    data.SickLeaveDay = this.treatmentFinishSDO.SickLeaveDay;
                    data.PatientRelativeName = this.treatmentFinishSDO.PatientRelativeName;
                    data.PatientRelativeType = this.treatmentFinishSDO.PatientRelativeType;
                    data.PatientWorkPlace = this.treatmentFinishSDO.PatientWorkPlace;
                    data.SickLeaveFrom = this.treatmentFinishSDO.SickLeaveFrom;
                    data.SickLeaveTo = this.treatmentFinishSDO.SickLeaveTo;
                    data.WorkPlaceId = this.treatmentFinishSDO.WorkPlaceId;
                    data.Loginname = this.treatmentFinishSDO.SickLoginname;
                    data.Username = this.treatmentFinishSDO.SickUsername;
                    if (this.treatmentFinishSDO.Babies != null && this.treatmentFinishSDO.Babies.Count > 0)
                    {
                        data.Babes = new List<BabyADO>();
                        foreach (var item in this.treatmentFinishSDO.Babies)
                        {
                            BabyADO maternityLeaveData = new BabyADO();
                            if (item.BornTime.HasValue)
                                maternityLeaveData.BornTimeDt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.BornTime.Value);
                            maternityLeaveData.FatherName = item.FatherName;
                            maternityLeaveData.GenderId = item.GenderId;
                            maternityLeaveData.Weight = item.Weight;
                            data.Babes.Add(maternityLeaveData);
                        }
                    }
                    data.DocumentBookId = this.treatmentFinishSDO.DocumentBookId;
                    data.SurgeryAppointmentTime = this.treatmentFinishSDO.SurgeryAppointmentTime;
                    data.Advise = this.treatmentFinishSDO.Advise;
                    data.AppointmentSurgery = this.treatmentFinishSDO.AppointmentSurgery;
                    data.EndTypeExtNote = this.treatmentFinishSDO.EndTypeExtNote;
                    data.IsPregnancyTermination = this.treatmentFinishSDO.IsPregnancyTermination;
                    data.PregnancyTerminationReason = this.treatmentFinishSDO.PregnancyTerminationReason;
                    data.GestationalAge = this.treatmentFinishSDO.GestationalAge;
                    data.TreatmentMethod = this.treatmentFinishSDO.TreatmentMethod;
                    data.SocialInsuranceNumber = this.treatmentFinishSDO.SocialInsuranceNumber;
                    data.PregnancyTerminationTime = this.treatmentFinishSDO.PregnancyTerminationTime;
                    data.MotherName = this.treatmentFinishSDO.MotherName;
                    data.FatherName = this.treatmentFinishSDO.FatherName;
                }

                Inventec.Common.Logging.LogSystem.Debug("GetTreatmentEndTypeExt____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return data;
        }

        private void ReloadDataTreatmentEndTypeExt(object data)
        {
            try
            {
                if (this.treatmentFinishSDO == null)
                    this.treatmentFinishSDO = new HisTreatmentFinishSDO();

                if (data != null)
                {
                    if (data is TreatmentEndTypeExtData)
                    {
                        this.currentTreatmentEndTypeExt = (TreatmentEndTypeExtData)data;
                        this.treatmentFinishSDO.TreatmentEndTypeExtId = this.currentTreatmentEndTypeExt.TreatmentEndTypeExtId;
                        if (this.treatmentFinishSDO.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO)
                        {
                            this.treatmentFinishSDO.AppointmentSurgery = this.currentTreatmentEndTypeExt.AppointmentSurgery;
                            this.treatmentFinishSDO.Advise = this.currentTreatmentEndTypeExt.Advise;
                            this.treatmentFinishSDO.SurgeryAppointmentTime = this.currentTreatmentEndTypeExt.SurgeryAppointmentTime;
                        }
                        else
                        {
                            this.treatmentFinishSDO.SickLeaveDay = this.currentTreatmentEndTypeExt.SickLeaveDay;
                            this.treatmentFinishSDO.SickLeaveFrom = this.currentTreatmentEndTypeExt.SickLeaveFrom;
                            this.treatmentFinishSDO.SickLeaveTo = this.currentTreatmentEndTypeExt.SickLeaveTo;
                            this.treatmentFinishSDO.PatientRelativeName = this.currentTreatmentEndTypeExt.PatientRelativeName;
                            this.treatmentFinishSDO.PatientRelativeType = this.currentTreatmentEndTypeExt.PatientRelativeType;
                            this.treatmentFinishSDO.PatientWorkPlace = this.currentTreatmentEndTypeExt.PatientWorkPlace;
                            this.treatmentFinishSDO.SickHeinCardNumber = this.currentTreatmentEndTypeExt.SickHeinCardNumber;
                            this.treatmentFinishSDO.SickLoginname = this.currentTreatmentEndTypeExt.Loginname;
                            this.treatmentFinishSDO.SickUsername = this.currentTreatmentEndTypeExt.Username;
                            this.treatmentFinishSDO.WorkPlaceId = this.currentTreatmentEndTypeExt.WorkPlaceId;
                            if (this.currentTreatmentEndTypeExt.Babes != null && this.currentTreatmentEndTypeExt.Babes.Count > 0)
                            {
                                this.treatmentFinishSDO.Babies = new List<HisBabySDO>();
                                foreach (var item in this.currentTreatmentEndTypeExt.Babes)
                                {
                                    HisBabySDO hisBabySDO = new HisBabySDO();
                                    hisBabySDO.FatherName = item.FatherName;
                                    if (item.BornTimeDt.HasValue)
                                    {
                                        hisBabySDO.BornTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(item.BornTimeDt.Value);
                                    }
                                    hisBabySDO.GenderId = item.GenderId;
                                    hisBabySDO.Weight = item.Weight;
                                    this.treatmentFinishSDO.Babies.Add(hisBabySDO);
                                }
                            }
                            this.treatmentFinishSDO.EndTypeExtNote = this.currentTreatmentEndTypeExt.EndTypeExtNote;
                            this.treatmentFinishSDO.DocumentBookId = this.currentTreatmentEndTypeExt.DocumentBookId;
                            this.treatmentFinishSDO.IsPregnancyTermination = this.currentTreatmentEndTypeExt.IsPregnancyTermination;
                            this.treatmentFinishSDO.PregnancyTerminationReason = this.currentTreatmentEndTypeExt.PregnancyTerminationReason;
                            this.treatmentFinishSDO.TreatmentMethod = this.currentTreatmentEndTypeExt.TreatmentMethod;
                            this.treatmentFinishSDO.GestationalAge = this.currentTreatmentEndTypeExt.GestationalAge;
                            this.treatmentFinishSDO.SocialInsuranceNumber = this.currentTreatmentEndTypeExt.SocialInsuranceNumber;
                            this.treatmentFinishSDO.PregnancyTerminationTime = this.currentTreatmentEndTypeExt.PregnancyTerminationTime;
                            this.treatmentFinishSDO.MotherName = this.currentTreatmentEndTypeExt.MotherName;
                            this.treatmentFinishSDO.FatherName = this.currentTreatmentEndTypeExt.FatherName;
                        }
                        this.isLoadTreatmentInFormTreatmentEndTypeExt = true;
                    }
                    else
                    {
                        this.currentTreatmentEndTypeExt = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentEndTypeExt_EditValueChanged(object sender, EventArgs e)
        {
            if (this.treatmentFinishSDO == null) return;

            if (cboTreatmentEndTypeExt.EditValue == null)
            {
                cboTreatmentEndTypeExt.Properties.Buttons[1].Visible = false;
                this.treatmentFinishSDO.SickLeaveDay = null;
                this.treatmentFinishSDO.SickLeaveFrom = null;
                this.treatmentFinishSDO.SickLeaveTo = null;
                this.treatmentFinishSDO.PatientRelativeName = null;
                this.treatmentFinishSDO.PatientRelativeType = null;
                this.treatmentFinishSDO.PatientWorkPlace = null;
                this.treatmentFinishSDO.Babies = null;

                this.treatmentFinishSDO.AppointmentSurgery = null;
                this.treatmentFinishSDO.Advise = null;
                this.treatmentFinishSDO.SurgeryAppointmentTime = null;
                this.treatmentFinishSDO.DocumentBookId = null;
            }
            else
            {
                cboTreatmentEndTypeExt.Properties.Buttons[1].Visible = true;
                //long treatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString());
                //if (treatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                //{
                //    this.treatmentFinishSDO.Babies = null;
                //}
                long treatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString());
                if (treatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                {
                    chkPrintBHXH.Enabled = chkSignBHXH.Enabled = true;

                    if (dlgGetStoreStateValue != null)
                    {
                        string vlStatechkPrintBHXH = this.dlgGetStoreStateValue("KeyStorechkPrintBHXH");
                        if (vlStatechkPrintBHXH == "1" && chkPrintBHXH.CheckState != CheckState.Checked)
                        {
                            chkPrintBHXH.CheckState = CheckState.Checked;
                        }
                        string vlStatechkSignBHXH = this.dlgGetStoreStateValue("KeyStorechkSignBHXH");
                        if (vlStatechkSignBHXH == "1" && chkSignBHXH.CheckState != CheckState.Checked)
                        {
                            chkSignBHXH.CheckState = CheckState.Checked;
                        }
                    }

                }
                else
                {
                    chkPrintBHXH.Enabled = chkSignBHXH.Enabled = false;
                    chkPrintBHXH.CheckState = chkSignBHXH.CheckState = CheckState.Unchecked;
                }
            }
        }

        private void cboTreatmentEndTypeExt_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTreatmentEndTypeExt.Properties.Buttons[1].Visible = false;
                    cboTreatmentEndTypeExt.EditValue = null;
                    cboTreatmentEndTypeExt.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtEndTime_Leave(object sender, EventArgs e)
        {
            try
            {
                if (isInitForm)
                {
                    isInitForm = false;
                    return;
                }

                if (IsCheckFinishTime && dtEndTime.EditValue != null && dtEndTime.DateTime != DateTime.MinValue)
                {
                    WaitingManager.Show();
                    CheckFinishTime();
                    WaitingManager.Hide();
                }

                if (cboTreatmentEndType.EditValue != null)
                {
                    long treatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndType.EditValue.ToString());
                    if (treatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                    {
                        if (dtEndTime != null && dtEndTime.EditValue != null)
                        {
                            //dtAppointmentTime.DateTime = dtEndTime.DateTime.AddDays((double)(spinApoinmentNumDay.Value));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateCreateEMRVBA(CreateEMRVBAOnClick _dlgCreateEMRVBAOnClick)
        {
            try
            {
                this.dlgCreateEMRVBAOnClick = _dlgCreateEMRVBAOnClick;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIssueOutPatientStoreCode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ProcessProgramm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkAutoPrintGHK_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isFirstLoadData) return;

                long treatmentendTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? 0).ToString());
                if (treatmentendTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN && this.dlgSetStoreStateValue != null)
                {
                    this.dlgSetStoreStateValue("KeyStorechkAutoPrintGHK", chkAutoPrintGHK.Checked ? "1" : "0");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSignGHK_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isFirstLoadData) return;

                long treatmentendTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? 0).ToString());
                if (treatmentendTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN && this.dlgSetStoreStateValue != null)
                {
                    this.dlgSetStoreStateValue("KeyStorechkSignGHK", chkSignGHK.Checked ? "1" : "0");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAutoPrintBK_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isFirstLoadData) return;

                this.dlgSetStoreStateValue("KeyStorechkAutoPrintBK", chkAutoPrintBK.Checked ? "1" : "0");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSignBK_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isFirstLoadData) return;

                this.dlgSetStoreStateValue("KeyStorechkSignBK", chkSignBK.Checked ? "1" : "0");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintBANT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (this.isFirstLoadData) return;

                //this.dlgSetStoreStateValue("KeyStorechkPrintBANT", chkPrintBANT.Checked ? "1" : "0");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintBHXH_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isFirstLoadData) return;

                long treatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString());
                if (treatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                {
                    this.dlgSetStoreStateValue("KeyStorechkPrintBHXH", chkPrintBHXH.Checked ? "1" : "0");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSignBHXH_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isFirstLoadData) return;
                long treatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString());
                if (treatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                {
                    this.dlgSetStoreStateValue("KeyStorechkSignBHXH", chkSignBHXH.Checked ? "1" : "0");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintTL_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isFirstLoadData) return;

                this.dlgSetStoreStateValue("KeyStorechkPrintTL", chkPrintTL.Checked ? "1" : "0");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSignTL_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isFirstLoadData) return;

                this.dlgSetStoreStateValue("KeyStorechkSignTL", chkSignTL.Checked ? "1" : "0");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Method

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, List<ColumnInfo> columnInfos, bool showHeader, int popupWidth)
        {
            try
            {
                //if (!String.IsNullOrEmpty(displayMemberCode))
                //{
                //    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                //    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                //}
                //if (!String.IsNullOrEmpty(displayMember))
                //{
                //    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                //    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                //}
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, showHeader, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDataForFirst()
        {
            try
            {
                if (this.notAutoInitData)
                {
                    this.InitTreatmentEndType();

                    this.InitTreatmentEndTypeExt();

                    this.InitCarrer();

                    if (this.useCapSoBABNCT.HasValue && this.useCapSoBABNCT.Value)
                        this.InitCapSoLuuTruBNCT();
                    else
                    {
                        //lciForchkIssueOutPatientStoreCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciForlblSoLuuTruBANT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciForcboProgram.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                }
                else
                {
                    SetEnableCheckSoLuuTruBANTByConfig();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCarrer()
        {
            try
            {
                var HisCareer = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                this.InitComboCommon(cboCareer, HisCareer, "ID", "CAREER_NAME", "CAREER_CODE");

                this.LoadDefautcboCareer(HisCareer);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitTreatmentEndType()
        {
            var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
             <HIS_TREATMENT_END_TYPE>().Where(o => isTreatmentIn ? o.IS_FOR_IN_PATIENT == 1 : o.IS_FOR_OUT_PATIENT == 1).ToList();

            this.InitComboCommon(cboTreatmentEndType, data, "ID", "TREATMENT_END_TYPE_NAME", "TREATMENT_END_TYPE_CODE");
        }

        private void AutoTreatmentFinishCheckedChanged()
        {
            try
            {
                this.InitDataForFirst();
                Inventec.Common.Logging.LogSystem.Debug("AutoTreatmentFinishCheckedChanged");

                dtEndTime.Enabled = txtTreatmentEndTypeCode.Enabled
                    = cboTreatmentEndType.Enabled
                    //= chkAutoPrintGHK.Enabled
                    //= chkSignGHK.Enabled
                    = chkAutoPrintBK.Enabled
                    = chkSignBK.Enabled
                    //= chkIssueOutPatientStoreCode.Enabled
                    = chkPrintBANT.Enabled
                    = cboTreatmentEndTypeExt.Enabled
                    = (chkAutoTreatmentFinish.Checked);

                if (IsAutoCheckBKTheoDoiTuong())
                    chkAutoPrintBK.CheckState = CheckState.Checked;

                if (chkAutoTreatmentFinish.Checked)
                {
                    ValidHeadDepartmentAndDirectorBranch();
                    if (this.Treatment != null)
                    {
                        cboTreatmentEndTypeExt.EditValue = this.Treatment.TREATMENT_END_TYPE_EXT_ID;

                        if (this.treatmentFinishSDO == null)
                            this.treatmentFinishSDO = new HisTreatmentFinishSDO();
                        this.currentTreatmentEndTypeExt = new TreatmentEndTypeExtData();
                        this.currentTreatmentEndTypeExt.AppointmentSurgery = this.treatmentFinishSDO.AppointmentSurgery = this.Treatment.APPOINTMENT_SURGERY;
                        this.currentTreatmentEndTypeExt.Advise = this.treatmentFinishSDO.Advise = this.Treatment.ADVISE;
                        this.currentTreatmentEndTypeExt.SurgeryAppointmentTime = this.treatmentFinishSDO.SurgeryAppointmentTime = this.Treatment.SURGERY_APPOINTMENT_TIME;
                        this.currentTreatmentEndTypeExt.SickLeaveDay = this.treatmentFinishSDO.SickLeaveDay = this.Treatment.SICK_LEAVE_DAY;
                        this.currentTreatmentEndTypeExt.SickLeaveFrom = this.treatmentFinishSDO.SickLeaveFrom = this.Treatment.SICK_LEAVE_FROM;
                        this.currentTreatmentEndTypeExt.SickLeaveTo = this.treatmentFinishSDO.SickLeaveTo = this.Treatment.SICK_LEAVE_TO;
                        this.currentTreatmentEndTypeExt.PatientRelativeName = this.treatmentFinishSDO.PatientRelativeName = this.Treatment.TDL_PATIENT_RELATIVE_NAME;
                        this.currentTreatmentEndTypeExt.PatientRelativeType = this.treatmentFinishSDO.PatientRelativeType = this.Treatment.TDL_PATIENT_RELATIVE_TYPE;
                        this.currentTreatmentEndTypeExt.PatientWorkPlace = this.treatmentFinishSDO.PatientWorkPlace = this.Treatment.TDL_PATIENT_WORK_PLACE;
                        this.currentTreatmentEndTypeExt.SickHeinCardNumber = this.treatmentFinishSDO.SickHeinCardNumber = this.Treatment.SICK_HEIN_CARD_NUMBER;
                        this.currentTreatmentEndTypeExt.Loginname = this.treatmentFinishSDO.SickLoginname = this.Treatment.SICK_LOGINNAME;
                        this.currentTreatmentEndTypeExt.Username = this.treatmentFinishSDO.SickUsername = this.Treatment.SICK_USERNAME;
                        this.currentTreatmentEndTypeExt.WorkPlaceId = this.treatmentFinishSDO.WorkPlaceId = this.Treatment.TDL_PATIENT_WORK_PLACE_ID;
                        this.currentTreatmentEndTypeExt.DocumentBookId = this.treatmentFinishSDO.DocumentBookId = this.Treatment.DOCUMENT_BOOK_ID;
                        this.currentTreatmentEndTypeExt.EndTypeExtNote = treatmentFinishSDO.EndTypeExtNote = this.Treatment.END_TYPE_EXT_NOTE;
                        this.currentTreatmentEndTypeExt.IsPregnancyTermination = this.treatmentFinishSDO.IsPregnancyTermination = this.Treatment.IS_PREGNANCY_TERMINATION == (short?)1;
                        this.currentTreatmentEndTypeExt.PregnancyTerminationReason = this.treatmentFinishSDO.PregnancyTerminationReason = this.Treatment.PREGNANCY_TERMINATION_REASON;
                        this.currentTreatmentEndTypeExt.PregnancyTerminationTime = this.treatmentFinishSDO.PregnancyTerminationTime = this.Treatment.PREGNANCY_TERMINATION_TIME;
                        this.currentTreatmentEndTypeExt.TreatmentMethod = this.treatmentFinishSDO.TreatmentMethod = this.Treatment.TREATMENT_METHOD;
                        this.currentTreatmentEndTypeExt.GestationalAge = treatmentFinishSDO.GestationalAge = this.Treatment.GESTATIONAL_AGE;
                        this.currentTreatmentEndTypeExt.SocialInsuranceNumber = treatmentFinishSDO.SocialInsuranceNumber = this.Treatment.TDL_SOCIAL_INSURANCE_NUMBER;
                        treatmentFinishSDO.TranPatiReasonId = this.Treatment.TRAN_PATI_REASON_ID;
                        treatmentFinishSDO.TranPatiFormId = this.Treatment.TRAN_PATI_FORM_ID;
                        treatmentFinishSDO.TranPatiTechId = this.Treatment.TRAN_PATI_TECH_ID;
                        treatmentFinishSDO.TransferOutMediOrgCode = this.Treatment.MEDI_ORG_CODE;
                        treatmentFinishSDO.TransferOutMediOrgName = this.Treatment.MEDI_ORG_NAME;
                        HisTreatmentExtFilter filter = new HisTreatmentExtFilter();
                        filter.TREATMENT_ID = Treatment.ID;
                        var treatmentExt = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT_EXT>>("api/HisTreatmentExt/Get", ApiConsumers.MosConsumer, filter, null);
                        if (treatmentExt != null)
                        {
                            treatmentFinishSDO.ClinicalNote = treatmentExt[0].CLINICAL_NOTE;
                            treatmentFinishSDO.SubclinicalResult = treatmentExt[0].SUBCLINICAL_RESULT;
                        }
                        treatmentFinishSDO.PatientCondition = this.Treatment.PATIENT_CONDITION;
                        treatmentFinishSDO.TransportVehicle = this.Treatment.TRANSPORT_VEHICLE;
                        treatmentFinishSDO.Transporter = this.Treatment.TRANSPORTER;
                        treatmentFinishSDO.TreatmentDirection = this.Treatment.TREATMENT_DIRECTION;
                        treatmentFinishSDO.MainCause = this.Treatment.MAIN_CAUSE;
                        treatmentFinishSDO.Surgery = this.Treatment.SURGERY;
                        treatmentFinishSDO.DeathTime = this.Treatment.DEATH_TIME;
                        treatmentFinishSDO.IsHasAupopsy = this.Treatment.IS_HAS_AUPOPSY;
                        treatmentFinishSDO.DeathCauseId = this.Treatment.DEATH_CAUSE_ID;
                        treatmentFinishSDO.DeathWithinId = this.Treatment.DEATH_WITHIN_ID;
                        treatmentFinishSDO.HospitalizeReasonName = this.Treatment.HOSPITALIZE_REASON_NAME;
                        treatmentFinishSDO.HospitalizeReasonCode = this.Treatment.HOSPITALIZE_REASON_CODE;
                        if (this.currentTreatmentEndTypeExt.WorkPlaceId == null)
                        {
                            if (!String.IsNullOrWhiteSpace(this.Treatment.TDL_PATIENT_WORK_PLACE_NAME))
                            {
                                var workPlace = BackendDataWorker.Get<HIS_WORK_PLACE>().FirstOrDefault(o => o.WORK_PLACE_NAME == this.Treatment.TDL_PATIENT_WORK_PLACE_NAME);
                                if (workPlace != null)
                                {
                                    this.currentTreatmentEndTypeExt.WorkPlaceId = this.treatmentFinishSDO.WorkPlaceId = workPlace.ID;

                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(this.Treatment.END_DEPT_SUBS_HEAD_LOGINNAME) && (cboEndDeptSubs.Properties.DataSource as List<AcsUserADO>).Exists(o => o.LOGINNAME == this.Treatment.END_DEPT_SUBS_HEAD_LOGINNAME))
                            cboEndDeptSubs.EditValue = this.Treatment.END_DEPT_SUBS_HEAD_LOGINNAME;
                        if (ConfigKeyCFG.EndDepartmentSubsHeadOption == "1" && cboEndDeptSubs.EditValue == null)
                            cboEndDeptSubs.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        if (!string.IsNullOrEmpty(this.Treatment.HOSP_SUBS_DIRECTOR_LOGINNAME) && (cboHospSubs.Properties.DataSource as List<AcsUserADO>).Exists(o => o.LOGINNAME == this.Treatment.HOSP_SUBS_DIRECTOR_LOGINNAME))
                            cboHospSubs.EditValue = this.Treatment.HOSP_SUBS_DIRECTOR_LOGINNAME;
                        if (cboHospSubs.EditValue == null)
                            LoadDefaultEndDept();
                        if (this.Treatment.TREATMENT_END_TYPE_ID != null)
                        {
                            HIS_TREATMENT_END_TYPE treatmentEndType = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().FirstOrDefault(o => o.ID == this.Treatment.TREATMENT_END_TYPE_ID);
                            cboTreatmentEndType.EditValue = this.Treatment.TREATMENT_END_TYPE_ID;
                            txtTreatmentEndTypeCode.Text = (treatmentEndType != null ? treatmentEndType.TREATMENT_END_TYPE_CODE : "");
                        }
                    }
                    //chkAutoPrintGHK.Enabled = chkSignGHK.Enabled = false;
                    //chkAutoPrintGHK.Checked = chkSignGHK.Checked = false;

                    if (layoutControlEditor.Visible == false)
                        layoutControlEditor.Visible = true;

                    if (this.checkedTreatmentFinish != null)
                        checkedTreatmentFinish();

                    if (this.getDateADO != null)
                        this.dataInputADO = this.getDateADO();

                    if (this.dataInputADO != null && this.dataInputADO.UseTime.HasValue)
                    {
                        dtEndTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.dataInputADO.UseTime.Value);
                    }
                    else
                        dtEndTime.EditValue = DateTime.Now;

                    if (this.dataInputADO != null)
                    {
                        if (!string.IsNullOrEmpty(this.dataInputADO.icdCode))
                        {
                            currentIcd.SHOW_ICD_CODE = this.dataInputADO.icdCode;
                        }
                        if (!string.IsNullOrEmpty(this.dataInputADO.icdName))
                        {
                            currentIcd.SHOW_ICD_NAME = this.dataInputADO.icdName;
                        }
                        if (!string.IsNullOrEmpty(this.dataInputADO.icdSubCode))
                        {
                            currentIcd.SHOW_ICD_SUB_CODE = this.dataInputADO.icdSubCode;
                        }
                        if (!string.IsNullOrEmpty(this.dataInputADO.icdText))
                        {
                            currentIcd.SHOW_ICD_TEXT = this.dataInputADO.icdText;
                        }
                    }

                    long treatmentEndTypeSda = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyCFG.TREATMENT_END___TREATMENT_END_TYPE_DEFAULT));
                    if (IsCheckAutoTreatmentFinish)
                    {
                        if (treatmentEndTypeSda == 2)
                        {
                            HIS_TREATMENT_END_TYPE treatmentEndType = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV);
                            cboTreatmentEndType.EditValue = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV;
                            txtTreatmentEndTypeCode.Text = (treatmentEndType != null ? treatmentEndType.TREATMENT_END_TYPE_CODE : "");
                        }
                        else if (treatmentEndTypeSda == 1)
                        {
                            //SetValueToAppoinmentControl();
                            HIS_TREATMENT_END_TYPE treatmentEndType = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN);
                            cboTreatmentEndType.EditValue = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN;
                            txtTreatmentEndTypeCode.Text = (treatmentEndType != null ? treatmentEndType.TREATMENT_END_TYPE_CODE : "");
                            cboTreatmentEndType_Closed(null, null);
                            //chkAutoPrintGHK.Checked = chkSignGHK.Checked = true;//TODO option
                            chkAutoPrintGHK.Enabled = chkSignGHK.Enabled = true;
                            if (dlgGetStoreStateValue != null)
                            {
                                string vlStatechkAutoPrintGHK = this.dlgGetStoreStateValue("KeyStorechkAutoPrintGHK");
                                if (vlStatechkAutoPrintGHK == "1" && chkAutoPrintGHK.CheckState != CheckState.Checked)
                                {
                                    chkAutoPrintGHK.CheckState = CheckState.Checked;
                                }
                                string vlStatechkSignGHK = this.dlgGetStoreStateValue("KeyStorechkSignGHK");
                                if (vlStatechkSignGHK == "1" && chkSignGHK.CheckState != CheckState.Checked)
                                {
                                    chkSignGHK.CheckState = CheckState.Checked;
                                }
                            }
                        }
                        else if (treatmentEndTypeSda == 3 || treatmentEndTypeSda == 4)
                        {
                            if ((treatmentEndTypeSda == 4 && !string.IsNullOrEmpty(Treatment.TRANSFER_IN_MEDI_ORG_CODE) && Treatment.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE) || (!string.IsNullOrEmpty(Treatment.TDL_HEIN_CARD_NUMBER) && !string.IsNullOrEmpty(Treatment.TDL_HEIN_MEDI_ORG_CODE) && (string.IsNullOrEmpty(BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE) || BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE != Treatment.TDL_HEIN_MEDI_ORG_CODE) && (string.IsNullOrEmpty(BranchDataWorker.Branch.ACCEPT_HEIN_MEDI_ORG_CODE) || (!BranchDataWorker.Branch.ACCEPT_HEIN_MEDI_ORG_CODE.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).ToList().Exists(o => o.Equals(Treatment.TDL_HEIN_MEDI_ORG_CODE)))) && (string.IsNullOrEmpty(BranchDataWorker.Branch.SYS_MEDI_ORG_CODE) || (!BranchDataWorker.Branch.SYS_MEDI_ORG_CODE.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).ToList().Exists(o => o.Equals(Treatment.TDL_HEIN_MEDI_ORG_CODE))))))
                            {
                                cboTreatmentEndType.EditValue = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN;
                                cboTreatmentEndType_Closed(null, null);
                            }
                            else
                                cboTreatmentEndType.EditValue = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV;
                            HIS_TREATMENT_END_TYPE treatmentEndType = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().FirstOrDefault(o => o.ID == Int64.Parse(cboTreatmentEndType.EditValue.ToString()));
                            txtTreatmentEndTypeCode.Text = treatmentEndType != null ? treatmentEndType.TREATMENT_END_TYPE_CODE : null;
                        }
                        else if (cboTreatmentEndType.EditValue == null)
                        {
                            txtTreatmentEndTypeCode.Text = "";
                            cboTreatmentEndType.EditValue = null;
                        }
                    }

                    if (this.autoTreatmentFinish__Checked != null)
                        this.autoTreatmentFinish__Checked();

                    if (this.dlgGetStoreStateValue != null)
                    {
                        string vlStatechkSignBK = this.dlgGetStoreStateValue("KeyStorechkSignBK");
                        if (vlStatechkSignBK == "1" && chkSignBK.CheckState != CheckState.Checked)
                        {
                            chkSignBK.CheckState = CheckState.Checked;
                        }
                        string vlStatechkAutoPrintBK = this.dlgGetStoreStateValue("KeyStorechkAutoPrintBK");
                        if (vlStatechkAutoPrintBK == "1" && chkAutoPrintBK.CheckState != CheckState.Checked)
                        {
                            chkAutoPrintBK.CheckState = CheckState.Checked;
                        }
                        string vlStatechkIssueOutPatientStoreCode = this.dlgGetStoreStateValue("KeyStorechkIssueOutPatientStoreCode");
                        if (vlStatechkIssueOutPatientStoreCode == "1" && chkIssueOutPatientStoreCode.CheckState != CheckState.Checked)
                        {
                            chkIssueOutPatientStoreCode.CheckState = CheckState.Checked;
                        }
                        string vlStatechkPrintBANT = this.dlgGetStoreStateValue("KeyStorechkPrintBANT");
                        if (vlStatechkPrintBANT == "1" && chkPrintBANT.CheckState != CheckState.Checked)
                        {
                            chkPrintBANT.CheckState = CheckState.Checked;
                        }

                        string vlStatechkPrintTL = this.dlgGetStoreStateValue("KeyStorechkPrintTL");
                        if (vlStatechkPrintTL == "1" && chkPrintTL.CheckState != CheckState.Checked)
                        {
                            chkPrintTL.CheckState = CheckState.Checked;
                        }
                        string vlStatechkSignTL = this.dlgGetStoreStateValue("KeyStorechkSignTL");
                        if (vlStatechkSignTL == "1" && chkSignTL.CheckState != CheckState.Checked)
                        {
                            chkSignTL.CheckState = CheckState.Checked;
                        }

                        string vlStatechkPrintExam = this.dlgGetStoreStateValue("KeyStorechkPrintExam");
                        if (vlStatechkPrintExam == "1" && chkPrintExam.CheckState != CheckState.Checked)
                        {
                            chkPrintExam.CheckState = CheckState.Checked;
                        }
                        string vlStatechkSignExam = this.dlgGetStoreStateValue("KeyStorechkSignExam");
                        if (vlStatechkSignExam == "1" && chkSignExam.CheckState != CheckState.Checked)
                        {
                            chkSignExam.CheckState = CheckState.Checked;
                        }

                        if (!ConfigKeyCFG.AutoCheckAndDisableExportXmlCollinear)
                        {
                            string vlStatechkXuatXML = this.dlgGetStoreStateValue("KeyStorechkXuatXML");
                            if (vlStatechkXuatXML == "1" && chkXuatXML.CheckState != CheckState.Checked)
                            {
                                chkXuatXML.CheckState = CheckState.Checked;
                            }
                        }

                        //string vlStatechkPrintBHXH = this.dlgGetStoreStateValue("KeyStorechkPrintBHXH");
                        //if (vlStatechkPrintBHXH == "1" && chkPrintBHXH.CheckState != CheckState.Checked)
                        //{
                        //    chkPrintBHXH.CheckState = CheckState.Checked;
                        //}
                        //string vlStatechkSignBHXH = this.dlgGetStoreStateValue("KeyStorechkSignBHXH");
                        //if (vlStatechkSignBHXH == "1" && chkSignBHXH.CheckState != CheckState.Checked)
                        //{
                        //    chkSignBHXH.CheckState = CheckState.Checked;
                        //}
                    }

                    this.txtTreatmentEndTypeCode.Focus();
                    this.txtTreatmentEndTypeCode.SelectAll();
                }
                else
                {
                    //Nothing
                    //layoutControlEditor.Visible = false;
                }
                if (this.DelegateTreatmentFinishCheckChange != null)
                    this.DelegateTreatmentFinishCheckChange(chkAutoTreatmentFinish.Checked);
                isFirstLoadData = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadDefaultEndDept()
        {

            try
            {
                var _vHisExecuteRooms = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(p =>
                     p.IS_ACTIVE == 1 && p.ROOM_ID == treatmentFinishInitADO.WorkingRoomId && !string.IsNullOrEmpty(p.HOSP_SUBS_DIRECTOR_LOGINNAME));
                if (_vHisExecuteRooms != null)
                {
                    cboHospSubs.EditValue = _vHisExecuteRooms.HOSP_SUBS_DIRECTOR_LOGINNAME;
                }
                else
                {
                    var HisDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p =>
                         p.IS_ACTIVE == 1 && p.ID == treatmentFinishInitADO.WorkingDepartmentId && !string.IsNullOrEmpty(p.HOSP_SUBS_DIRECTOR_LOGINNAME));
                    cboHospSubs.EditValue = HisDepartment != null ? HisDepartment.HOSP_SUBS_DIRECTOR_LOGINNAME : null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ValidHeadDepartmentAndDirectorBranch()
        {

            try
            {
                if (!treatmentFinishInitADO.WorkingDepartmentId.HasValue || treatmentFinishInitADO.WorkingDepartmentId.Value == 0)
                    return;
                var Department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == treatmentFinishInitADO.WorkingDepartmentId);

                var empHead = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Department.HEAD_LOGINNAME);
                if (empHead != null && empHead.IS_NEED_SIGN_INSTEAD == 1)
                {
                    ValidCustomGridLookup(txtEndDeptSubs, cboEndDeptSubs);
                    layoutControlItem7.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    dxValidationProvider1.RemoveControlError(txtEndDeptSubs);
                    dxValidationProvider1.SetValidationRule(txtEndDeptSubs, null);
                    layoutControlItem7.AppearanceItemCaption.ForeColor = Color.Black;
                }

                var Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().FirstOrDefault(o => o.ID == Department.BRANCH_ID);
                var empDirec = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Branch.DIRECTOR_LOGINNAME);
                if (empDirec != null && empDirec.IS_NEED_SIGN_INSTEAD == 1)
                {
                    ValidCustomGridLookup(txtHospSubs, cboHospSubs);
                    layoutControlItem10.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    dxValidationProvider1.RemoveControlError(txtHospSubs);
                    dxValidationProvider1.SetValidationRule(txtHospSubs, null);
                    layoutControlItem10.AppearanceItemCaption.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        void TranPatiDataTreatmentFinish(MOS.SDO.HisTreatmentFinishSDO treatmentFinish)
        {
            try
            {
                this.treatmentFinishSDO = treatmentFinish;
                Inventec.Common.Logging.LogSystem.Debug("TranPatiDataTreatmentFinish____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentFinishSDO), treatmentFinishSDO));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TreatmentEndTypeProcess(MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE data)
        {
            try
            {
                if (this.treatmentFinishSDO == null)
                    this.treatmentFinishSDO = new MOS.SDO.HisTreatmentFinishSDO();

                TreatmentEndInputADO treatmentEndInputADO = new TreatmentEndInputADO();
                treatmentEndInputADO.HisTreatmentFinishSDO = this.treatmentFinishSDO;
                treatmentEndInputADO.AppointmentNextRoomIds = (this.treatmentFinishSDO.AppointmentExamRoomIds != null && this.treatmentFinishSDO.AppointmentExamRoomIds.Count > 0) ? this.treatmentFinishSDO.AppointmentExamRoomIds : null;
                treatmentEndInputADO.AppointmentTime = (this.treatmentFinishSDO.AppointmentTime ?? 0);
                treatmentEndInputADO.Treatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatmentEndInputADO.Treatment, this.Treatment);

                if (data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                {
                    chkAutoPrintGHK.Enabled = chkSignGHK.Enabled = false;
                    chkAutoPrintGHK.Checked = chkSignGHK.Checked = false;
                    cboTreatmentEndTypeExt.Enabled = false;
                    cboTreatmentEndTypeExt.EditValue = null;

                    CloseTreatment.FormDeath FormDeath = new CloseTreatment.FormDeath(treatmentEndInputADO);
                    FormDeath.MyGetData = TranPatiDataTreatmentFinish;
                    FormDeath.ShowDialog();
                }
                else if (data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                {
                    cboTreatmentEndTypeExt.Enabled = true;
                    chkAutoPrintGHK.Enabled = chkSignGHK.Enabled = false;
                    chkAutoPrintGHK.Checked = chkSignGHK.Checked = false;

                    CloseTreatment.FormTransfer FormTransfer = new CloseTreatment.FormTransfer(treatmentEndInputADO, treatmentFinishInitADO.WorkingRoomId ?? 0);
                    FormTransfer.MyGetData = TranPatiDataTreatmentFinish;
                    FormTransfer.ShowDialog();
                }
                else if (data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                {
                    chkAutoPrintGHK.Enabled = chkSignGHK.Enabled = true;
                    cboTreatmentEndTypeExt.EditValue = null;
                    cboTreatmentEndTypeExt.Enabled = true;

                    if (dlgGetStoreStateValue != null)
                    {
                        string vlStatechkAutoPrintGHK = this.dlgGetStoreStateValue("KeyStorechkAutoPrintGHK");
                        if (vlStatechkAutoPrintGHK == "1" && chkAutoPrintGHK.CheckState != CheckState.Checked)
                        {
                            chkAutoPrintGHK.CheckState = CheckState.Checked;
                        }
                        string vlStatechkSignGHK = this.dlgGetStoreStateValue("KeyStorechkSignGHK");
                        if (vlStatechkSignGHK == "1" && chkSignGHK.CheckState != CheckState.Checked)
                        {
                            chkSignGHK.CheckState = CheckState.Checked;
                        }
                    }
                    //DelegateGetStoreStateValue dlgGetStoreStateValue;
                    //DelegateSetStoreStateValue dlgSetStoreStateValue;

                    //if (this.treatmentFinishSDO == null || (this.treatmentFinishSDO != null && (this.treatmentFinishSDO.AppointmentTime ?? 0) <= 0))
                    //    SetValueToAppoinmentControl();

                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, treatmentEndInputADO.Treatment);
                    if (treatment.ID == 0) treatment.ID = (treatmentId ?? 0);

                    if (treatment.TDL_PATIENT_TYPE_ID == null && treatment.ID != null)
                    {
                        HisTreatmentFilter filter = new HisTreatmentFilter();
                        filter.ID = treatment.ID;
                        treatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, new CommonParam()).FirstOrDefault();
                    }

                    if (treatmentEndInputADO != null && treatmentEndInputADO.HisTreatmentFinishSDO != null && treatmentEndInputADO.HisTreatmentFinishSDO.AppointmentTime > 0)
                    {
                        //mở lại form để nhập thì vẫn đổ lại thông tin đã nhập.
                        treatment.TREATMENT_END_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN;
                        treatment.ADVISE = treatmentEndInputADO.HisTreatmentFinishSDO.Advise;
                        treatment.APPOINTMENT_TIME = treatmentEndInputADO.HisTreatmentFinishSDO.AppointmentTime;
                        treatment.APPOINTMENT_PERIOD_ID = treatmentEndInputADO.HisTreatmentFinishSDO.AppointmentPeriodId;
                        if (treatmentEndInputADO.HisTreatmentFinishSDO.AppointmentExamRoomIds != null && treatmentEndInputADO.HisTreatmentFinishSDO.AppointmentExamRoomIds.Count > 0)
                        {
                            treatment.APPOINTMENT_EXAM_ROOM_IDS = string.Join(",", treatmentEndInputADO.HisTreatmentFinishSDO.AppointmentExamRoomIds);
                        }
                        else
                        {
                            treatment.APPOINTMENT_EXAM_ROOM_IDS = null;
                        }
                    }
                    else if (treatmentFinishInitADO.WorkingRoomId.HasValue && treatmentFinishInitADO.WorkingRoomId.Value > 0)
                    {
                        treatment.APPOINTMENT_EXAM_ROOM_IDS = treatmentFinishInitADO.WorkingRoomId.Value + "";

                    }
                    treatment.OUT_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtEndTime.DateTime.ToString("yyyyMMddHHmmss"));

                    EndTypeForm.FormAppointment form = new EndTypeForm.FormAppointment(treatment, this.treatmentFinishSDO, this.dataInputADO, treatmentFinishInitADO, TranPatiDataTreatmentFinish, GetUseDayFromAppoimentForm, IsBlockOrder);
                    form.ShowDialog();
                }
                else
                {
                    chkAutoPrintGHK.Enabled = chkSignGHK.Enabled = false;
                    chkAutoPrintGHK.Checked = chkSignGHK.Checked = false;
                    cboTreatmentEndTypeExt.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void GetUseDayFromAppoimentForm(decimal useDay)
        {
            this.useDay = useDay;
        }

        internal decimal GetUseDay()
        {
            return this.useDay;
        }

        private void SetValueToAppoinmentControl()
        {
            try
            {
                if (this.treatmentFinishSDO == null)
                {
                    this.treatmentFinishSDO = new HisTreatmentFinishSDO();
                    this.treatmentFinishSDO.TreatmentId = (treatmentId ?? 0);
                }

                if (this.getDateADO != null)
                    this.dataInputADO = this.getDateADO();

                if (!this.dataInputADO.UseTime.HasValue || !this.dataInputADO.UseTimeTo.HasValue)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong lay dc du lieu this.dataInputADO.UseTime hoac this.dataInputADO.UseTimeTo");
                    return;
                }

                if (!TreatmentEndHasAppointmentTimeDefault && TreatmentEndAppointmentTimeDefault > 0)
                {
                    this.treatmentFinishSDO.AppointmentTime =
                            Inventec.Common.DateTime.Calculation.Add(this.dataInputADO.UseTime ?? 0,
                            TreatmentEndAppointmentTimeDefault,
                            Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY
                            ) ?? Inventec.Common.DateTime.Get.Now().Value;
                }
                else
                {
                    DateTime useTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.dataInputADO.UseTimeTo ?? 0)
                           ?? DateTime.Now;
                    this.treatmentFinishSDO.AppointmentTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(useTimeTo.AddDays((long)1));
                }

                DateTime appointmentTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.treatmentFinishSDO.AppointmentTime ?? 0) ?? DateTime.Now;
                DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.dataInputADO.UseTime ?? 0) ?? DateTime.Now;
                TimeSpan ts = (TimeSpan)(appointmentTime.Date - dtUseTime.Date);
                this.useDay = (decimal)(ts.TotalDays);
                if (this.dataInputADO.AppointmentNextRoomIds != null && this.dataInputADO.AppointmentNextRoomIds.Count > 0)
                {
                    this.treatmentFinishSDO.AppointmentExamRoomIds = this.dataInputADO.AppointmentNextRoomIds;
                }
                else
                {
                    this.treatmentFinishSDO.AppointmentExamRoomIds = null;
                }
                this.treatmentFinishSDO.Advise = this.dataInputADO.Advise;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private DateTime GetUseTimeInSeq(long treatmentId)
        {
            DateTime time = new DateTime();
            try
            {
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.SERVICE_REQ_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "CREATE_TIME";
                var prescriptionList = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (prescriptionList != null && prescriptionList.Count > 0)
                {
                    var hisPrescription = prescriptionList.Where(o => o.USE_TIME_TO.HasValue).OrderByDescending(o => o.USE_TIME_TO).FirstOrDefault();
                    if (hisPrescription != null)
                    {
                        time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisPrescription.USE_TIME_TO.Value) ?? DateTime.MinValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return time;
        }

        public void Reload(DataInputADO dataInputADO)
        {
            try
            {
                this.dataInputADO = dataInputADO;
                this.isFirstLoadData = true;
                WaitingManager.Show();
                AutoTreatmentFinishCheckedChanged();
                //ChangeStateCreateEMRVBA(false);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public HisTreatmentFinishSDO GetData()
        {
            HisTreatmentFinishSDO result = null;
            try
            {
                if (treatmentFinishSDO == null)
                    treatmentFinishSDO = new HisTreatmentFinishSDO();

                treatmentFinishSDO.EndDeptSubsHeadLoginname = cboEndDeptSubs.EditValue != null ? cboEndDeptSubs.EditValue.ToString() : null;
                treatmentFinishSDO.EndDeptSubsHeadUsername = cboEndDeptSubs.EditValue != null ? cboEndDeptSubs.Text.ToString() : null;
                treatmentFinishSDO.HospSubsDirectorLoginname = cboHospSubs.EditValue != null ? cboHospSubs.EditValue.ToString() : null;
                treatmentFinishSDO.HospSubsDirectorUsername = cboHospSubs.EditValue != null ? cboHospSubs.Text.ToString() : null;
                return this.treatmentFinishSDO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public DataOutputADO GetDataOutput()
        {
            DataOutputADO result = new DataOutputADO();
            try
            {
                result.IsSignExam = chkSignExam.Checked;
                result.IsPrintExam = chkPrintExam.Checked;
                result.IsAutoTreatmentFinish = chkAutoTreatmentFinish.Checked;
                result.IsAutoPrintGHK = chkAutoPrintGHK.Checked;
                result.IsSignGHK = chkSignGHK.Checked;
                result.IsAutoBK = chkAutoPrintBK.Checked;
                result.IsSignBK = chkSignBK.Checked;
                result.IsAutoPrintTL = chkPrintTL.Checked;
                result.IsSignTL = chkSignTL.Checked;
                result.IsAutoBANT = chkPrintBANT.Checked;
                result.IsIssueOutPatientStoreCode = chkIssueOutPatientStoreCode.Checked;
                result.TreatmentEndTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? "0").ToString());
                if (cboTreatmentEndTypeExt.EditValue != null)
                    result.TreatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndTypeExt.EditValue ?? "0").ToString());
                result.dtEndTime = dtEndTime.DateTime;
                result.dtAppointmentTime = (treatmentFinishSDO != null && treatmentFinishSDO.AppointmentTime != null && treatmentFinishSDO.AppointmentTime > 0) ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatmentFinishSDO.AppointmentTime ?? 0).Value : DateTime.MinValue;
                //result.Advise = treatmentFinishSDO != null ? treatmentFinishSDO.Advise : "";
                result.AppointmentNextRoomIds = (treatmentFinishSDO != null && treatmentFinishSDO.AppointmentExamRoomIds != null && treatmentFinishSDO.AppointmentExamRoomIds.Count > 0) ? treatmentFinishSDO.AppointmentExamRoomIds : null;
                result.StoreCode = lblSoLuuTruBANT.Text;
                result.ProgramId = cboProgram.EditValue != null ? (long?)cboProgram.EditValue : null;
                result.SickHeinCardNumber = this.currentTreatmentEndTypeExt != null ? this.currentTreatmentEndTypeExt.SickHeinCardNumber : "";
                result.SickLoginname = this.currentTreatmentEndTypeExt != null ? this.currentTreatmentEndTypeExt.Loginname : "";
                result.SickUsername = this.currentTreatmentEndTypeExt != null ? this.currentTreatmentEndTypeExt.Username : "";
                result.SickLeaveDay = this.currentTreatmentEndTypeExt != null ? this.currentTreatmentEndTypeExt.SickLeaveDay : null;
                result.SickLeaveFrom = this.currentTreatmentEndTypeExt != null ? this.currentTreatmentEndTypeExt.SickLeaveFrom : null;
                result.SickLeaveTo = this.currentTreatmentEndTypeExt != null ? this.currentTreatmentEndTypeExt.SickLeaveTo : null;
                result.SickWorkplaceId = this.currentTreatmentEndTypeExt != null ? this.currentTreatmentEndTypeExt.WorkPlaceId : null;
                result.DocumentBookId = (currentTreatmentEndTypeExt != null && currentTreatmentEndTypeExt.DocumentBookId != null && currentTreatmentEndTypeExt.DocumentBookId > 0) ? currentTreatmentEndTypeExt.DocumentBookId : null;
                result.SurgeryAppointmentTime = this.currentTreatmentEndTypeExt != null ? this.currentTreatmentEndTypeExt.SurgeryAppointmentTime : null;
                result.AppointmentSurgery = this.currentTreatmentEndTypeExt != null ? this.currentTreatmentEndTypeExt.AppointmentSurgery : null;
                result.Advise = this.currentTreatmentEndTypeExt != null ? this.currentTreatmentEndTypeExt.Advise : null;
                result.CareeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboCareer.EditValue ?? "0").ToString());

                result.IsPrintBHXH = chkPrintBHXH.Checked;
                result.IsSignBHXH = chkSignBHXH.Checked;
                result.IsExpXml4210Collinear = chkXuatXML.Checked;

                result.NumOrderBlockId = (treatmentFinishSDO != null && treatmentFinishSDO.NumOrderBlockId != null) ? treatmentFinishSDO.NumOrderBlockId : null;
                //result.NumOrderBlockNumOrder = (treatmentFinishSDO != null && treatmentFinishSDO.num != null) ? treatmentFinishSDO.NumOrderBlockId : null;
                result.icdCode = currentIcd != null ? currentIcd.SHOW_ICD_CODE : "";
                result.icdName = currentIcd != null ? currentIcd.SHOW_ICD_NAME : "";
                result.icdSubCode = currentIcd != null ? currentIcd.SHOW_ICD_SUB_CODE : "";
                result.icdText = currentIcd != null ? currentIcd.SHOW_ICD_TEXT : "";

                result.EndDeptSubsHeadLoginname = cboEndDeptSubs.EditValue != null ? cboEndDeptSubs.EditValue.ToString() : null;
                result.EndDeptSubsHeadUsername = cboEndDeptSubs.EditValue != null ? cboEndDeptSubs.Text.ToString() : null;
                result.HospSubsDirectorLoginname = cboHospSubs.EditValue != null ? cboHospSubs.EditValue.ToString() : null;
                result.HospSubsDirectorUsername = cboHospSubs.EditValue != null ? cboHospSubs.Text.ToString() : null;

                result.TranPatiHospitalLoginname = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.TranPatiHospitalLoginname)) ? treatmentFinishSDO.TranPatiHospitalLoginname : null;
                result.TranPatiHospitalUsername = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.TranPatiHospitalUsername)) ? treatmentFinishSDO.TranPatiHospitalUsername : null;
                result.TranPatiReasonId = (treatmentFinishSDO != null && treatmentFinishSDO.TranPatiReasonId != null) ? treatmentFinishSDO.TranPatiReasonId : null;
                result.TranPatiFormId = (treatmentFinishSDO != null && treatmentFinishSDO.TranPatiFormId != null) ? treatmentFinishSDO.TranPatiFormId : null;
                result.TranPatiTechId = (treatmentFinishSDO != null && treatmentFinishSDO.TranPatiTechId != null) ? treatmentFinishSDO.TranPatiTechId : null;
                result.TransferOutMediOrgCode = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.TransferOutMediOrgCode)) ? treatmentFinishSDO.TransferOutMediOrgCode : null;
                result.TransferOutMediOrgName = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.TransferOutMediOrgName)) ? treatmentFinishSDO.TransferOutMediOrgName : null;
                result.ClinicalNote = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.ClinicalNote)) ? treatmentFinishSDO.ClinicalNote : null;
                result.SubclinicalResult = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.SubclinicalResult)) ? treatmentFinishSDO.SubclinicalResult : null;
                result.PatientCondition = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.PatientCondition)) ? treatmentFinishSDO.PatientCondition : null;
                result.TransportVehicle = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.TransportVehicle)) ? treatmentFinishSDO.TransportVehicle : null;
                result.Transporter = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.Transporter)) ? treatmentFinishSDO.Transporter : null;
                result.TreatmentDirection = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.TreatmentDirection)) ? treatmentFinishSDO.TreatmentDirection : null;
                result.MainCause = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.MainCause)) ? treatmentFinishSDO.MainCause : null;
                result.Surgery = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.Surgery)) ? treatmentFinishSDO.Surgery : null;
                result.DeathTime = (treatmentFinishSDO != null && treatmentFinishSDO.DeathTime != null) ? treatmentFinishSDO.DeathTime : null;
                result.IsHasAupopsy = (treatmentFinishSDO != null && treatmentFinishSDO.IsHasAupopsy != null) ? treatmentFinishSDO.IsHasAupopsy : null;
                result.DeathCauseId = (treatmentFinishSDO != null && treatmentFinishSDO.DeathCauseId != null) ? treatmentFinishSDO.DeathCauseId : null;
                result.DeathWithinId = (treatmentFinishSDO != null && treatmentFinishSDO.DeathWithinId != null) ? treatmentFinishSDO.DeathWithinId : null;
                result.EndTypeExtNote = (treatmentFinishSDO != null && treatmentFinishSDO.EndTypeExtNote != null) ? treatmentFinishSDO.EndTypeExtNote : null;
                result.MotherName = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.MotherName)) ? treatmentFinishSDO.MotherName : null;
                result.FatherName = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.FatherName)) ? treatmentFinishSDO.FatherName : null;
                result.TransporterLoginnames = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.TransporterLoginnames)) ? treatmentFinishSDO.TransporterLoginnames : null;
                result.TreatmentMethod = (treatmentFinishSDO != null && treatmentFinishSDO.TreatmentMethod != null) ? treatmentFinishSDO.TreatmentMethod : null;
                result.HospitalizeReasonCode = (treatmentFinishSDO != null && treatmentFinishSDO.HospitalizeReasonCode != null) ? treatmentFinishSDO.HospitalizeReasonCode : null;
                result.HospitalizeReasonName = (treatmentFinishSDO != null && treatmentFinishSDO.HospitalizeReasonName != null) ? treatmentFinishSDO.HospitalizeReasonName : null;
                result.SurgeryName = (treatmentFinishSDO != null && !string.IsNullOrEmpty(treatmentFinishSDO.SurgeryName)) ? treatmentFinishSDO.SurgeryName : null;
                result.SurgeryBeginTime = (treatmentFinishSDO != null && treatmentFinishSDO.SurgeryBeginTime != null) ? treatmentFinishSDO.SurgeryBeginTime : null;
                result.SurgeryEndTime = (treatmentFinishSDO != null && treatmentFinishSDO.SurgeryEndTime != null) ? treatmentFinishSDO.SurgeryEndTime : null;
                result.UsedMedicine = (treatmentFinishSDO != null && treatmentFinishSDO.UsedMedicine != null) ? treatmentFinishSDO.UsedMedicine : null;
                Inventec.Common.Logging.LogSystem.Debug("GetDataOutput____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public string GetSickHeinCardNumber()
        {
            string result = "";
            try
            {
                return (this.currentTreatmentEndTypeExt != null ? this.currentTreatmentEndTypeExt.SickHeinCardNumber : "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public void FocusControl()
        {
            try
            {
                chkAutoTreatmentFinish.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ShowPopupAppointmentControl()
        {
            try
            {
                if (treatmentFinishSDO == null)
                    treatmentFinishSDO = new HisTreatmentFinishSDO();

                TreatmentEndInputADO treatmentEndInputADO = new TreatmentEndInputADO();
                treatmentEndInputADO.HisTreatmentFinishSDO = treatmentFinishSDO;
                treatmentEndInputADO.AppointmentNextRoomIds = (treatmentFinishSDO.AppointmentExamRoomIds != null && treatmentFinishSDO.AppointmentExamRoomIds.Count > 0) ? treatmentFinishSDO.AppointmentExamRoomIds : null;
                treatmentEndInputADO.Treatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatmentEndInputADO.Treatment, this.Treatment);

                HIS_TREATMENT treatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, treatmentEndInputADO.Treatment);
                if (treatment.ID == 0) treatment.ID = (treatmentId ?? 0);
                if (treatmentEndInputADO != null && treatmentEndInputADO.HisTreatmentFinishSDO != null)
                {
                    //mở lại form để nhập thì vẫn đổ lại thông tin đã nhập.
                    treatment.TREATMENT_END_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN;
                    treatment.ADVISE = treatmentEndInputADO.HisTreatmentFinishSDO.Advise;
                    treatment.APPOINTMENT_TIME = treatmentEndInputADO.HisTreatmentFinishSDO.AppointmentTime;
                    treatment.APPOINTMENT_PERIOD_ID = treatmentEndInputADO.HisTreatmentFinishSDO.AppointmentPeriodId;
                    if (treatmentEndInputADO.HisTreatmentFinishSDO.AppointmentExamRoomIds != null && treatmentEndInputADO.HisTreatmentFinishSDO.AppointmentExamRoomIds.Count > 0)
                    {
                        treatment.APPOINTMENT_EXAM_ROOM_IDS = string.Join(",", treatmentEndInputADO.HisTreatmentFinishSDO.AppointmentExamRoomIds);
                    }
                    else
                    {
                        treatment.APPOINTMENT_EXAM_ROOM_IDS = null;
                    }
                }
                treatment.OUT_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtEndTime.DateTime.ToString("yyyyMMddHHmmss"));

                EndTypeForm.FormAppointment form = new EndTypeForm.FormAppointment(treatment, this.treatmentFinishSDO, this.dataInputADO, treatmentFinishInitADO, TranPatiDataTreatmentFinish, GetUseDayFromAppoimentForm, IsBlockOrder);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ShowPopupWhenNotFinishingIncaseOfOutPatient()
        {
            try
            {
                if (chkAutoTreatmentFinish.Checked == false)
                {
                    chkAutoTreatmentFinish.Checked = true;
                    cboTreatmentEndType.Focus();
                    cboTreatmentEndType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void CheckChangeAutoTreatmentFinish(bool check)
        {
            try
            {
                chkAutoTreatmentFinish.Checked = check;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void EnableChangeAutoTreatmentFinish(bool enable)
        {
            try
            {
                chkAutoTreatmentFinish.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitNeedSickLeaveCert(bool isNeedSickLeaveCert)
        {
            try
            {
                lciForlblNeedSickLeaveCert.Visibility = isNeedSickLeaveCert ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void UpdateStoreCode(string storCode)
        {
            try
            {
                this.lblSoLuuTruBANT.Text = storCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void UpdateTreatmentData(HisTreatmentWithPatientTypeInfoSDO treatment)
        {
            try
            {
                this.Treatment = treatment;
                this.treatmentId = treatment.ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnShowIcd_Click(object sender, EventArgs e)
        {
            try
            {
                Icd.frmIcd frm = new Icd.frmIcd(currentIcd, ChooseData);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChooseData(IcdTemp temp)
        {
            try
            {
                currentIcd = temp;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkXuatXML_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isFirstLoadData) return;

                if (!ConfigKeyCFG.AutoCheckAndDisableExportXmlCollinear)
                {
                    this.dlgSetStoreStateValue("KeyStorechkXuatXML", chkXuatXML.Checked ? "1" : "0");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCareer_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtEndTime.Focus();
                    dtEndTime.SelectAll();
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCareer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtEndTime.Focus();
                    dtEndTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitDataTocboUser()
        {
            try
            {
                LogSystem.Debug("InitDataTocboUser 1...");
                this.lstReAcsUserADO = new List<AcsUserADO>();

                // Lấy danh sách ACS_USER có USERNAME và IS_ACTIVE = 1
                var acsUserList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                    .Where(p => !string.IsNullOrEmpty(p.USERNAME) && p.IS_ACTIVE == 1)
                    .OrderBy(o => o.USERNAME)
                    .ToList();

                LogSystem.Debug("InitDataTocboUser 2...");
                List<V_HIS_EMPLOYEE> hisEmployees = await GetHisEmployeesAsync();

                LogSystem.Debug("InitDataTocboUser 3...");
                var vhisEmployeeDict = hisEmployees
                    .Where(e => e.IS_ACTIVE == 1 && acsUserList.Any(u => u.LOGINNAME == e.LOGINNAME))
                    .ToDictionary(e => e.LOGINNAME);

                LogSystem.Debug("InitDataTocboUser 4...");
                this.lstReAcsUserADO = acsUserList.Select(item =>
                {
                    var ado = new AcsUserADO(item);
                    if (vhisEmployeeDict.TryGetValue(item.LOGINNAME, out var vhisEmployee))
                    {
                        ado.DOB = vhisEmployee.DOB;
                        ado.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(vhisEmployee.DOB ?? 0);
                        ado.DIPLOMA = vhisEmployee.DIPLOMA;
                        ado.DEPARTMENT_CODE = vhisEmployee.DEPARTMENT_CODE;
                        ado.DEPARTMENT_ID = vhisEmployee.DEPARTMENT_ID;
                        ado.DEPARTMENT_NAME = vhisEmployee.DEPARTMENT_NAME;
                    }
                    return ado;
                }).ToList();

                LogSystem.Debug("InitDataTocboUser 5...");
                FillDataTocboUserByData();
                LogSystem.Debug("InitDataTocboUser 6...");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task<List<V_HIS_EMPLOYEE>> GetHisEmployeesAsync()
        {
            if (BackendDataWorker.IsExistsKey<V_HIS_EMPLOYEE>())
            {
                return HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>();
            }
            else
            {
                var paramCommon = new CommonParam();
                var filter = new MOS.Filter.HisEmployeeFilter();
                var hisEmployees = await new Inventec.Common.Adapter.BackendAdapter(paramCommon)
                    .GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>>("api/HisEmployee/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                if (hisEmployees != null)
                {
                    BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE), hisEmployees, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                return hisEmployees ?? new List<V_HIS_EMPLOYEE>();
            }
        }


        private void FillDataTocboUserByData()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "Họ tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, true, 400);
                ControlEditorLoader.Load(cboEndDeptSubs, this.lstReAcsUserADO.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
                cboEndDeptSubs.Properties.ImmediatePopup = true;
                ControlEditorLoader.Load(cboHospSubs, this.lstReAcsUserADO.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
                cboHospSubs.Properties.ImmediatePopup = true;

                if (ConfigKeyCFG.EndDepartmentSubsHeadOption == "1" && cboEndDeptSubs.EditValue == null)
                    cboEndDeptSubs.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (cboHospSubs.EditValue == null)
                    LoadDefaultEndDept();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEndDeptSubs_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtEndDeptSubs.Text))
                    {
                        var dt = lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == txtEndDeptSubs.Text.Trim());
                        if (dt != null)
                        {
                            cboEndDeptSubs.EditValue = dt.LOGINNAME;
                            cboEndDeptSubs.Focus();
                        }
                        else
                        {
                            cboEndDeptSubs.EditValue = null;
                        }
                    }
                    else
                    {
                        cboEndDeptSubs.ShowPopup();
                        cboEndDeptSubs.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHospSubs_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtHospSubs.Text))
                    {
                        var dt = lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == txtHospSubs.Text.Trim());
                        if (dt != null)
                        {
                            cboHospSubs.EditValue = dt.LOGINNAME;
                            cboHospSubs.Focus();
                        }
                        else
                        {
                            cboHospSubs.EditValue = null;
                        }
                    }
                    else
                    {
                        cboHospSubs.ShowPopup();
                        cboHospSubs.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndDeptSubs_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboEndDeptSubs.EditValue != null)
                {
                    txtEndDeptSubs.Text = cboEndDeptSubs.EditValue.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHospSubs_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboHospSubs.EditValue != null)
                {
                    txtHospSubs.Text = cboHospSubs.EditValue.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndDeptSubs_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEndDeptSubs.EditValue = null;
                    txtEndDeptSubs.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHospSubs_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHospSubs.EditValue = null;
                    txtHospSubs.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintExam_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isFirstLoadData) return;

                this.dlgSetStoreStateValue("KeyStorechkPrintExam", chkPrintExam.Checked ? "1" : "0");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSignExam_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isFirstLoadData) return;

                this.dlgSetStoreStateValue("KeyStorechkSignExam", chkSignExam.Checked ? "1" : "0");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
