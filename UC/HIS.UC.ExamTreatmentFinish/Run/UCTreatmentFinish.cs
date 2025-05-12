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
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using HIS.UC.Death;
using HIS.UC.TranPati;
using HIS.UC.TranPati.ADO;
using HIS.UC.Death.ADO;
using HIS.UC.ExamTreatmentFinish.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.UC.Sick;
using HIS.UC.Sick.ADO;
using DevExpress.XtraEditors.Controls;
using HIS.UC.SurgeryAppointment;
using HIS.UC.SurgeryAppointment.ADO;
using HIS.UC.ExamTreatmentFinish.Config;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.SecondaryIcd;
using Inventec.Desktop.Common.Message;
using MOS.SDO;
using System.Threading;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.UCCauseOfDeath.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using HIS.UC.SecondaryIcd.ADO;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using IcdADO = HIS.UC.ExamTreatmentFinish.ADO.IcdADO;
using Inventec.Common.Adapter;
using DevExpress.XtraLayout;
using HIS.UC.ExamTreatmentFinish.Resources;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Inventec.Common.Logging;
using DevExpress.XtraExport;

namespace HIS.UC.ExamTreatmentFinish.Run
{
    public partial class UCExamTreatmentFinish : UserControl
    {
        private Desktop.Plugins.Library.CheckIcd.CheckIcdManager checkIcdManager { get; set; }
        string icdSubCodeScreeen { get; set; }
        frmICDInformation frmICDInformation = null;
        ShowICDInformationADO currentShowICDInformation = null;
        private DateTime prescriptionTime = new DateTime();
        List<HIS_TREATMENT_END_TYPE> treatmentEndTypes { get; set; }
        List<HIS_TREATMENT_END_TYPE_EXT> treatmentEndTypeExts { get; set; }
        List<HIS_TREATMENT_RESULT> treatmentResults { get; set; }
        List<ProgramADO> ProgramADOList { get; set; }

        private int positionHandle = -1;
        //TranPatiProcessor tranPatiProcessor;
        //UserControl ucTranPati;
        private MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO { get; set; }
        Inventec.Desktop.Common.Modules.Module moduleData;
        DeathProcessor deathProcessor;
        UserControl ucDeath;

        SickProcessor sickProcessor;
        UserControl ucSick;

        SurgeryAppointmentProcessor surgeryAppointmentProcessor;
        UserControl ucSurgeryAppointment;
        long RoomId = 0;

        TreatmentFinishInitADO ExamTreatmentFinishInitADO;
        HIS_TREATMENT_EXT _treatmentext;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.UC.ExamTreatmentFinish";
        bool IsVisibleHosTransfer = false;
        bool sttTempHosTransfer = false;

        HisTreatmentFinishSDO sickSdoResult { get; set; }
        SurgAppointmentADO surSdoResult { get; set; }

        HisTreatmentFinishSDO deathSdoResult { get; set; }
        CauseOfDeathADO causeResult { get; set; }
        SickInitADO sickInitADO { get; set; }
        SurgeryAppointmentInitADO surgeryInitADO { get; set; }
        string nameModuleLink { get; set; }
        HIS.Desktop.Utility.UserControlBase userControl { get; set; }
        List<AcsUserADO> lstReAcsUserADO;
        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;
        List<HIS_DEPARTMENT> listDepartment;
        List<HIS_BRANCH> listBranch;
        public UCExamTreatmentFinish(TreatmentFinishInitADO _ExamTreatmentFinishInitADO,HIS_TREATMENT_EXT _treatmentExt)
        {
            InitializeComponent();
            try
            {
                this.ExamTreatmentFinishInitADO = _ExamTreatmentFinishInitADO;
                this.treatmentEndTypeExts = _ExamTreatmentFinishInitADO.TreatmentEndTypeExts;
                this.moduleData = _ExamTreatmentFinishInitADO.moduleData;
                this.icdSubCodeScreeen = _ExamTreatmentFinishInitADO.dlgGetIcdSubCode();
                this._treatmentext = _treatmentExt;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public UCExamTreatmentFinish(TreatmentFinishInitADO _ExamTreatmentFinishInitADO)
        {
            InitializeComponent();
            try
            {
                this.ExamTreatmentFinishInitADO = _ExamTreatmentFinishInitADO;
                this.treatmentEndTypeExts = _ExamTreatmentFinishInitADO.TreatmentEndTypeExts;
                this.moduleData = _ExamTreatmentFinishInitADO.moduleData;
                this.icdSubCodeScreeen = _ExamTreatmentFinishInitADO.dlgGetIcdSubCode();
                //this._treatmentext = _treatmentExt;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void UCExamTreatmentFinish_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfig.GetConfig();
                this.autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                if (this.RoomId > 0)
                {
                    this.isAllowNoIcd = (BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.RoomId).IS_ALLOW_NO_ICD == 1);
                }
                //                LoadTreatmentNew(this.ExamTreatmentFinishInitADO.Treatment);
                this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                this.currentTraditionalIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && p.IS_TRADITIONAL == 1).OrderBy(o => o.ICD_CODE).ToList();
                this.isAutoCheckIcd = (this.autoCheckIcd == 1);
                InitUcSecondaryIcd();
                UCIcdInit();
                SetDefaultConfig();
                ValidateForm();
                
                LoadDataToComboCareer();
                LoadComboTreatmentEndType();
                LoadComboTreatmentResult();
                LoadComboProgram(this.ExamTreatmentFinishInitADO.PatientPrograms, ExamTreatmentFinishInitADO.DataStores);
                SetDafaultComboProram();
                EnableControlByCheckStoreData();
                LoadComboTreatmentEndTypeExt();
                EnableControlAppoinment(false);
                LoadShowICDInformationData();
                LoadDataToControl();
                LoadTreatmentEndTypeDefault();

                LoadIcdToControl(this.ExamTreatmentFinishInitADO.IcdCode, this.ExamTreatmentFinishInitADO.IcdName);
                Inventec.Common.Logging.LogSystem.Debug("this.ExamTreatmentFinishInitADO.Treatment________" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.ExamTreatmentFinishInitADO.Treatment), this.ExamTreatmentFinishInitADO.Treatment));


                LoadTraditionalIcdToControl(this.ExamTreatmentFinishInitADO.TraditionalIcdCode, this.ExamTreatmentFinishInitADO.TraditionalIcdName);
                LoadTraditionalSubIcdToControl(this.ExamTreatmentFinishInitADO.TraditionalIcdSubCode, this.ExamTreatmentFinishInitADO.TraditionalIcdText);
                //LoadDataToIcdSub(this.ExamTreatmentFinishInitADO.IcdSubCode, this.ExamTreatmentFinishInitADO.IcdText);
                if (this.ExamTreatmentFinishInitADO != null)
                {
                    cboCareer.EditValue = this.ExamTreatmentFinishInitADO.CareerId;
                }

                InitControlState();
                VisibleIcdFromKeyConfig(this.ExamTreatmentFinishInitADO);
                nameModuleLink = "UCExamTreatmentFinish_Load";
                if (moduleData != null)
                {
                    nameModuleLink = moduleData.ModuleLink;
                }
                userControl = new UserControlBase();
                userControl.RegisterTimer(nameModuleLink, this.Name + ".timer1", timer1.Interval, timer_Tick);
                userControl.StartTimer(nameModuleLink, this.Name + ".timer1");
                checkIcdManager = new Desktop.Plugins.Library.CheckIcd.CheckIcdManager(DlgIcdSubCode, this.ExamTreatmentFinishInitADO.Treatment);
                SetCaptionByLanguageKey();
                this.listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().ToList();
                this.listBranch = BackendDataWorker.Get<HIS_BRANCH>().ToList();
                LoadDataTocboUser();
                if (this.layoutControlItem31.AppearanceItemCaption.ForeColor == Color.Brown) ValidateSignDirect();
                if (this.layoutControlItem30.AppearanceItemCaption.ForeColor == Color.Brown) ValidateSignHead();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void DlgIcdSubCode(string icdCodes, string icdNames)
        {
            try
            {
                ProcessIcdSub(icdCodes, icdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessIcdSub(string icdCodes, string icdNames)
        {
            try
            {
                string icdCodeUc = null;
                string icdTextUc = null;
                if (ucSecondaryIcd != null && subIcdProcessor != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        icdCodeUc = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        icdTextUc = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }
                var lstIcdCode = icdCodes.Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var lstIcdName = icdNames.Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var lstIcdCodeScreen = icdCodeUc.Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                lstIcdCodeScreen.AddRange(lstIcdCode);
                lstIcdCodeScreen = lstIcdCodeScreen.Distinct().ToList();
                string icdCode = string.Join(";", lstIcdCodeScreen);

                var lstIcdNameScreen = icdTextUc.Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                lstIcdNameScreen.AddRange(lstIcdName);
                lstIcdNameScreen = lstIcdNameScreen.Distinct().ToList();
                string icdName = string.Join(";", lstIcdNameScreen);
                LoaducSecondaryIcd(icdCode, icdName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void VisibleLayoutSubIcd(bool IsVisble)
        {
            try
            {
                layoutControlItem33.Visibility = layoutControlItem32.Visibility = IsVisble ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                VisibleLayoutSubIcd(HisConfig.OptionSubIcdWhenFinish == "3");
                if (HisConfig.OptionSubIcdWhenFinish != "3")
                    return;
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), currentIcds);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.hisTreatment = this.ExamTreatmentFinishInitADO.Treatment;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextSize = 100;
                ado.TextLblIcd = "CĐ phụ ra viện:";
                ado.TootiplciIcdSubCode = "Chẩn đoán phụ ra viện";
                ado.TextNullValue = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__TXT_ICD_TEXT__NULL_VALUE");
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelControlSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                    LoaducSecondaryIcd(this.ExamTreatmentFinishInitADO.Treatment.ICD_SUB_CODE, this.ExamTreatmentFinishInitADO.Treatment.ICD_TEXT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoaducSecondaryIcd(string icdCode, string icdName)
        {
            try
            {
                SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                subIcd.ICD_SUB_CODE = icdCode;
                subIcd.ICD_TEXT = icdName;
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusOut()
        {
            try
            {
                txtTraditionalIcdCode.Focus();
                txtTraditionalIcdCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetStringFromKey(string key)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(key))
                {
                    result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCTreatmentFinish
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.ExamTreatmentFinish.Resources.Lang", typeof(UCExamTreatmentFinish).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCareer.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.cboCareer.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsExpXml4210Collinear.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkIsExpXml4210Collinear.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintHosTransfer.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkPrintHosTransfer.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintPrescription.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkPrintPrescription.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnICDInformation.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.btnICDInformation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnICDInformation.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.btnICDInformation.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkKyPhieuTrichLuc.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkKyPhieuTrichLuc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkInPhieuTrichLuc.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkInPhieuTrichLuc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignBHXH.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkSignBHXH.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintBHXH.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkPrintBHXH.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignBordereau.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkSignBordereau.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignAppoinment.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkSignAppoinment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.txtIcdText.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTraditionalIcds.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.cboTraditionalIcds.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkTraditionalIcd.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkTraditionalIcd.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentResult.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.cboTreatmentResult.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkEditIcd.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkEditIcd.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcds.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.cboIcds.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboProgram.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.cboProgram.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCapSoLuuTruBA.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkCapSoLuuTruBA.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkBANT.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkBANT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChkCapSoLuuTruBA.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciChkCapSoLuuTruBA.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChkCapSoLuuTruBA.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciChkCapSoLuuTruBA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSoLuuTruBA.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciSoLuuTruBA.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSoLuuTruBA.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciSoLuuTruBA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientProgram.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciPatientProgram.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBANT.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciBANT.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBANT.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciBANT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentEndTypeExt.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.cboTreatmentEndTypeExt.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintBordereau.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkPrintBordereau.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintAppoinment.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.chkPrintAppoinment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentEndType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.cboTreatmentEndType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem10.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdText.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciIcdText.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdText.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciIcdText.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem3.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem8.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem14.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem5.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem23.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsExpXml4210Collinear.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciIsExpXml4210Collinear.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCareer.Text = Inventec.Common.Resource.Get.Value("UCTreatmentFinish.lciCareer.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void timer_Tick()
        {
            try
            {
                Inventec.Common.Logging.LogAction.Info(nameModuleLink + "_____START TIMER");
                FillDataToCbo(this.ExamTreatmentFinishInitADO.Treatment);
                Inventec.Common.Logging.LogAction.Info(nameModuleLink + "_____BEGIN TIMER");
                userControl.StopTimer(nameModuleLink, this.Name + ".timer1");
                Inventec.Common.Logging.LogAction.Info(nameModuleLink + "_____AFTER TIMER");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                FillDataToCbo(this.ExamTreatmentFinishInitADO.Treatment);
                timer1.Stop();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisibleIcdFromKeyConfig(TreatmentFinishInitADO ExamTreatmentFinishInitADO)
        {
            try
            {
                if (ExamTreatmentFinishInitADO.IsAutoSetIcdWhenFinishInOtherExam)
                {
                    lciIcdText.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    txtIcdCode.Text = null;
                    layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    cboIcds.EditValue = null;
                    txtIcdMainText.Text = null;
                    layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    chkEditIcd.Checked = false;
                    layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dxValidationProvider1.SetValidationRule(txtIcdCode, null);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadShowICDInformationData()
        {
            try
            {
                if (this.ExamTreatmentFinishInitADO != null)
                {
                    this.currentShowICDInformation = new ShowICDInformationADO(this.ExamTreatmentFinishInitADO.Treatment);
                }
                else
                {
                    this.currentShowICDInformation = new ShowICDInformationADO();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadTreatmentNew(HIS_TREATMENT treatment)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                treatmentFilter.ID = this.treatment.ID;
                var treatments = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param);
                this.ExamTreatmentFinishInitADO.Treatment = treatments.FirstOrDefault();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToCbo(HIS_TREATMENT treatment)
        {
            try
            {
                if (treatment != null)
                {
                    if (cboTreatmentEndType.EditValue == null && treatment.TREATMENT_END_TYPE_ID != null && treatment.TREATMENT_END_TYPE_ID > 0)
                    {
                        cboTreatmentEndType.EditValue = treatment.TREATMENT_END_TYPE_ID;
                    }
                    if (treatment.TREATMENT_END_TYPE_EXT_ID != null && treatment.TREATMENT_END_TYPE_EXT_ID > 0)
                    {
                        cboTreatmentEndTypeExt.EditValue = treatment.TREATMENT_END_TYPE_EXT_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                bool IsEnableControlXml = false;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPrintAppoinment.Name)
                        {
                            chkPrintAppoinment.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkSignAppoinment.Name)
                        {
                            chkSignAppoinment.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkSignBordereau.Name)
                        {
                            chkSignBordereau.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkPrintBHXH.Name)
                        {
                            chkPrintBHXH.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkSignBHXH.Name)
                        {
                            chkSignBHXH.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkInPhieuTrichLuc.Name)
                        {
                            chkInPhieuTrichLuc.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkKyPhieuTrichLuc.Name)
                        {
                            chkKyPhieuTrichLuc.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkPrintPrescription.Name)
                        {
                            chkPrintPrescription.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkPrintHosTransfer.Name)
                        {
                            sttTempHosTransfer = item.VALUE == "1";
                        }
                        if (item.KEY == chkIsExpXml4210Collinear.Name)
                        {
                            IsEnableControlXml = item.VALUE == "1";
                        }
                        if (item.KEY == chkPrintBordereau.Name && !chkPrintBordereau.Checked)
                        {
                            chkPrintBordereau.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkPrintExam.Name)
                        {
                            chkPrintExam.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkSignExam.Name)
                        {
                            chkSignExam.Checked = item.VALUE == "1";
                        }
                    }
                }
                if (HisConfig.IsExportXmlCollinear)
                {
                    lciIsExpXml4210Collinear.Enabled = false;
                    chkIsExpXml4210Collinear.Checked = true;
                }
                else
                {
                    chkIsExpXml4210Collinear.Checked = IsEnableControlXml;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void EnableControlByCheckStoreData()
        {
            try
            {
                if (chkCapSoLuuTruBA.CheckState != CheckState.Checked)
                {
                    lciSoLuuTruBA.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciPatientProgram.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lciSoLuuTruBA.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciPatientProgram.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultConfig()
        {
            try
            {
                chkCapSoLuuTruBA.CheckState = HisConfig.IsCheckedCheckboxIssueOutPatientStoreCode ? CheckState.Checked : CheckState.Unchecked;
                chkCapSoLuuTruBA.Enabled = HisConfig.IsEnableCheckboxIssueOutPatientStoreCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentEndType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboTreatmentEndTypeExt.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void dtEndTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTreatmentEndType.Focus();
                }
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
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboTreatmentEndType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentEndType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void dtEndTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTreatmentEndType.EditValue != null)
                {
                    //long treatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndType.EditValue.ToString());
                    //if (treatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                    //{
                    //    SetAppoinmentTime();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChiDinhChoLanKhamSau_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AppointmentService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AppointmentService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(treatment.ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentEndTypeExt_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboTreatmentEndTypeExt.Properties.Buttons[1].Visible = cboTreatmentEndTypeExt.EditValue != null;
                layoutControlIPanelUCExtend.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (cboTreatmentEndTypeExt.EditValue != null)
                {
                    //layoutControlIPanelUCExtend.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    if (Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM
                        || Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                    {
                        sickInitADO = new SickInitADO();
                        sickInitADO.currentModule = this.ExamTreatmentFinishInitADO.moduleData;
                        sickInitADO.CurrentHisTreatment = ExamTreatmentFinishInitADO.Treatment;
                        if (Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                        {
                            sickInitADO.IsDuongThai = true;
                        }
                        else
                        {
                            sickInitADO.ListDocumentBook = LoadDocumentBook();
                        }

                        Inventec.Common.Logging.LogSystem.Debug("cboTreatmentEndTypeExt_EditValueChanged____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sickInitADO), sickInitADO));
                        sickSdoResult = null;
                        frmPopUpSick frm = new frmPopUpSick(sickInitADO, ActionGetSdoSickResult);
                        frm.ShowDialog();
                        //sickProcessor = new SickProcessor();
                        //this.ucSick = (UserControl)sickProcessor.Run(sickInitADO);
                        //this.ucSick.Dock = DockStyle.Fill;
                        //panelExamTreatmentFinish.Controls.Clear();
                        //panelExamTreatmentFinish.Controls.Add(this.ucSick);
                        ////EnableControlAppoinment(false);
                    }
                    else if (Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO)
                    {
                        surgeryInitADO = new SurgeryAppointmentInitADO();
                        surgeryInitADO.CurrentHisTreatment = ExamTreatmentFinishInitADO.Treatment;
                        surSdoResult = null;
                        frmPopUpSick frm = new frmPopUpSick(surgeryInitADO, ActionGetSdoSurResult);
                        frm.ShowDialog();

                        //surgeryAppointmentProcessor = new SurgeryAppointmentProcessor();
                        //this.ucSurgeryAppointment = (UserControl)surgeryAppointmentProcessor.Run(surgeryInitADO);
                        //this.ucSurgeryAppointment.Dock = DockStyle.Fill;
                        //panelExamTreatmentFinish.Controls.Clear();
                        //panelExamTreatmentFinish.Controls.Add(this.ucSurgeryAppointment);
                        ////EnableControlAppoinment(false);
                    }
                    else
                    {
                        panelExamTreatmentFinish.Controls.Clear();
                        //EnableControlAppoinment(false);
                    }

                    if (Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                    {
                        chkPrintBHXH.Enabled = chkSignBHXH.Enabled = true;
                    }
                    else
                    {
                        chkPrintBHXH.Enabled = chkSignBHXH.Enabled = false;
                    }
                }
                else
                {
                    panelExamTreatmentFinish.Controls.Clear();
                    //EnableControlAppoinment(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ActionGetSdoSurResult(SurgAppointmentADO obj)
        {
            try
            {
                surSdoResult = obj;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ActionGetSdoSickResult(HisTreatmentFinishSDO obj)
        {
            try
            {
                sickSdoResult = obj;
                sickInitADO.CurrentHisTreatment = new HIS_TREATMENT();
                sickInitADO.CurrentHisTreatment.SICK_LEAVE_DAY = sickSdoResult.SickLeaveDay;
                sickInitADO.CurrentHisTreatment.TDL_PATIENT_RELATIVE_NAME = sickSdoResult.PatientRelativeName;
                sickInitADO.CurrentHisTreatment.TDL_PATIENT_RELATIVE_TYPE = sickSdoResult.PatientRelativeType;

                sickInitADO.CurrentHisTreatment.SICK_LOGINNAME = sickSdoResult.SickLoginname;
                sickInitADO.CurrentHisTreatment.TDL_PATIENT_WORK_PLACE = sickSdoResult.PatientWorkPlace;
                sickInitADO.CurrentHisTreatment.SICK_LEAVE_FROM = sickSdoResult.SickLeaveFrom;

                sickInitADO.CurrentHisTreatment.SICK_LEAVE_TO = sickSdoResult.SickLeaveTo;

                sickInitADO.WorkPlaceId = sickSdoResult.WorkPlaceId;
                sickInitADO.CurrentHisTreatment.TDL_PATIENT_TYPE_ID = ExamTreatmentFinishInitADO.Treatment.TDL_PATIENT_TYPE_ID;
                sickInitADO.CurrentHisTreatment.TDL_HEIN_CARD_NUMBER = sickSdoResult.SickHeinCardNumber;
                sickInitADO.CurrentHisTreatment.ID = sickSdoResult.TreatmentId;
                sickInitADO.CurrentHisTreatment.DOCUMENT_BOOK_ID = sickSdoResult.DocumentBookId;
                sickInitADO.CurrentHisTreatment.END_TYPE_EXT_NOTE = sickSdoResult.EndTypeExtNote;
                sickInitADO.CurrentHisTreatment.TREATMENT_METHOD = sickSdoResult.TreatmentMethod;
                sickInitADO.CurrentHisTreatment.PREGNANCY_TERMINATION_REASON = sickSdoResult.PregnancyTerminationReason;
                sickInitADO.CurrentHisTreatment.IS_PREGNANCY_TERMINATION = sickSdoResult.IsPregnancyTermination == true ? (short?)1 : null;
                sickInitADO.CurrentHisTreatment.GESTATIONAL_AGE = sickSdoResult.GestationalAge;
                sickInitADO.CurrentHisTreatment.PREGNANCY_TERMINATION_TIME = sickSdoResult.PregnancyTerminationTime;
                sickInitADO.CurrentHisTreatment.TDL_PATIENT_MOTHER_NAME = sickSdoResult.MotherName;
                sickInitADO.CurrentHisTreatment.TDL_PATIENT_FATHER_NAME = sickSdoResult.FatherName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTreatmentEndTypeExt.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void dtTimeAppointments_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DateEdit editor = sender as DateEdit;
        //        if (editor != null)
        //            this.CalculateDayNum();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void spinSickLeaveDay_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        SpinEdit editor = sender as SpinEdit;
        //        if (editor != null && editor.EditorContainsFocus)
        //            this.CalculateDateTo();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void CalculateDayNum()
        //{
        //    try
        //    {
        //        if (dtTimeAppointment.EditValue != null)
        //        {
        //            TimeSpan ts = (TimeSpan)(dtTimeAppointment.DateTime.Date - DateTime.Now.Date);
        //            spDay.Value = ts.Days;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void CalculateDateTo()
        //{
        //    try
        //    {
        //        if (dtTimeAppointment.EditValue != null)
        //        {
        //            dtTimeAppointment.DateTime = DateTime.Now.AddDays((double)(spDay.Value));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        public event EventHandler CapSoLuuTruBAChanged;
        public void UpdateLabelColor(LayoutControlItem LayoutControlItem1, LayoutControlItem LayoutControlItem2)
        {
            if (chkCapSoLuuTruBA.Checked)
            {
                LayoutControlItem1.AppearanceItemCaption.ForeColor = Color.Maroon;
                LayoutControlItem2.AppearanceItemCaption.ForeColor = Color.Maroon;
                
            }
            else
            {
                LayoutControlItem1.AppearanceItemCaption.ForeColor = Color.Black;
                LayoutControlItem2.AppearanceItemCaption.ForeColor = Color.Black;
            }
        }

        public void UpdateValide(DevExpress.XtraEditors.TextEdit textEdit1, DevExpress.XtraEditors.TextEdit textEdit2, DevExpress.XtraEditors.TextEdit textEdit3)
        {
            if (treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
            {
                List<string> errors = new List<string>();

                if (string.IsNullOrEmpty(textEdit1.Text.Trim()))
                {
                    errors.Add("Tóm tắt kết quả cận lâm sàng");
                }
                if (string.IsNullOrEmpty(textEdit2.Text.Trim()))
                {
                    errors.Add("Phương pháp điều trị");
                }
                if (string.IsNullOrEmpty(textEdit3.Text.Trim()))
                {
                    errors.Add("Chuẩn đoán sơ bộ");
                }

                if (errors.Count > 0)
                {
                    string errorMessage = "Bạn chưa nhập: " + string.Join(" hoặc ", errors) + ".";
                    DevExpress.XtraEditors.XtraMessageBox.Show(errorMessage, ResourceMessage.ThongBao);
                    return;
                }

            }
            else if(cboProgram.EditValue != null && cboProgram.Properties.View.FocusedRowHandle == 0)
            {
                List<string> errors = new List<string>();

                if (string.IsNullOrEmpty(textEdit1.Text.Trim()))
                {
                    errors.Add("Tóm tắt kết quả cận lâm sàng");
                }
                if (string.IsNullOrEmpty(textEdit2.Text.Trim()))
                {
                    errors.Add("Phương pháp điều trị");
                }
                if (string.IsNullOrEmpty(textEdit3.Text.Trim()))
                {
                    errors.Add("Chuẩn đoán sơ bộ");
                }

                if (errors.Count > 0)
                {
                    string errorMessage = "Bạn chưa nhập: " + string.Join(" hoặc ", errors) + ".";
                    DevExpress.XtraEditors.XtraMessageBox.Show(errorMessage, ResourceMessage.ThongBao);
                    return;
                }
            }
        }

        private void SetDafaultComboProram()
        {
            try
            {
                if (chkCapSoLuuTruBA.CheckState == CheckState.Checked && this.ExamTreatmentFinishInitADO != null && this.ExamTreatmentFinishInitADO.PatientPrograms != null && this.ExamTreatmentFinishInitADO.PatientPrograms.Count == 1)
                {
                    var program = this.ProgramADOList.FirstOrDefault(o => o.ID == this.ExamTreatmentFinishInitADO.PatientPrograms[0].PROGRAM_ID);
                    if (program != null)
                        cboProgram.EditValue = program.ID;
                    else
                        cboProgram.EditValue = null;
                }
                else
                {
                    cboProgram.EditValue = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCapSoLuuTruBA_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                
                EnableControlByCheckStoreData();
                LoadComboProgram(this.ExamTreatmentFinishInitADO.PatientPrograms, this.ExamTreatmentFinishInitADO.DataStores);
                SetDafaultComboProram();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewProgram_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                Boolean SelectPatient = (Boolean)gridViewProgram.GetRowCellValue(e.RowHandle, "SelectPatient");
                if (SelectPatient)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProgram_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboProgram.EditValue = null;
                    e.Button.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProgram_EditValueChanged(object sender, EventArgs e)
        {

            CapSoLuuTruBAChanged?.Invoke(this, EventArgs.Empty);
            if (cboProgram.EditValue != null)
            {
                dxValidationProvider1.RemoveControlError(cboProgram);
                cboProgram.Properties.Buttons[1].Visible = true;
            }
        }

        private void txtIcdCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "ICD không đúng";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCombo(txtIcdCode.Text.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcds.Text))
                {
                    cboIcds.EditValue = null;
                    txtIcdMainText.Text = "";
                    chkEditIcd.Checked = false;
                }
                else
                {
                    this._TextIcdName = cboIcds.Text;
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
                layoutControlIPanelUCExtend.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                IsVisibleHosTransfer = false;
                chkPrintHosTransfer.Checked = false;
                chkPrintHosTransfer.Enabled = false;
                if (cboTreatmentEndType.EditValue != null)
                {
                    HIS_TREATMENT_END_TYPE data = treatmentEndTypes.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        if (data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                        {
                            var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"));
                            if (HisConfig.AllowManyOpeningOption == "4" && ExamTreatmentFinishInitADO.Treatment.TDL_PATIENT_TYPE_ID == patientType.ID)
                            {
                                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                                treatmentFilter.PATIENT_ID = ExamTreatmentFinishInitADO.Treatment.PATIENT_ID;
                                treatmentFilter.TDL_PATIENT_TYPE_IDs = new List<long>() { patientType.ID };
                                treatmentFilter.IS_PAUSE = false;
                                treatmentFilter.TDL_TREATMENT_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY, IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU, IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU };
                                treatmentFilter.ID__NOT_EQUAL = this.ExamTreatmentFinishInitADO.Treatment.ID;

                                var treatmentOlds = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, null);

                                if (treatmentOlds != null && treatmentOlds.Count() > 0)
                                {
                                    XtraMessageBox.Show(string.Format("Bệnh nhân có đợt điều trị ngoại trú/nội trú cũ chưa kết thúc không cho phép chuyển viện. (Hồ sơ đã tạo: “{0}”)", String.Join(", ", treatmentOlds.Select(o => o.TREATMENT_CODE))), "Thông báo", MessageBoxButtons.OK);
                                    cboTreatmentEndType.EditValue = null;
                                    return;
                                }
                            }

                            //TranPatiInitADO tranPatiADO = new TranPatiInitADO();
                            //tranPatiADO.IsTextHolder = true;
                            //tranPatiADO.TranPatiData = new TranPatiDataSourcesADO();
                            //tranPatiADO.TranPatiData.HisTranPatiForms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_FORM>();
                            //tranPatiADO.TranPatiData.HisTranPatiReasons = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_REASON>();
                            //tranPatiADO.TranPatiData.HisMediOrgs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDI_ORG>();
                            //tranPatiADO.TranPatiData.HisTranPatiTechs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_TECH>();
                            //tranPatiADO.TranPatiData.CurrentHisTreatment = ExamTreatmentFinishInitADO.Treatment;

                            //tranPatiProcessor = new TranPatiProcessor();
                            //this.ucTranPati = (UserControl)tranPatiProcessor.Run(tranPatiADO);
                            //this.ucTranPati.Dock = DockStyle.Fill;
                            panelExamTreatmentFinish.Controls.Clear();
                            //panelExamTreatmentFinish.Controls.Add(this.ucTranPati);

                            cboTreatmentEndTypeExt.EditValue = null;
                            IsVisibleHosTransfer = true;
                            chkPrintHosTransfer.Enabled = true;
                            chkPrintHosTransfer.Checked = sttTempHosTransfer;

                            EnableControlAppoinment(false);
                            HIS_TREATMENT treatment = new HIS_TREATMENT();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, ExamTreatmentFinishInitADO.Treatment);
                            if (currentTreatmentFinishSDO != null)
                            {
                                //mở lại form để nhập thì vẫn đổ lại thông tin đã nhập.
                                treatment.MEDI_ORG_CODE = currentTreatmentFinishSDO.TransferOutMediOrgCode;
                                treatment.TRAN_PATI_FORM_ID = currentTreatmentFinishSDO.TranPatiFormId;
                                treatment.TRAN_PATI_REASON_ID = currentTreatmentFinishSDO.TranPatiReasonId;
                                treatment.TRAN_PATI_TECH_ID = currentTreatmentFinishSDO.TranPatiTechId;
                                treatment.PATIENT_CONDITION = currentTreatmentFinishSDO.PatientCondition;
                                treatment.TRANSPORT_VEHICLE = currentTreatmentFinishSDO.TransportVehicle;
                                treatment.TRANSPORTER = currentTreatmentFinishSDO.Transporter;
                                treatment.TREATMENT_METHOD = currentTreatmentFinishSDO.TreatmentMethod;
                                treatment.TREATMENT_DIRECTION = currentTreatmentFinishSDO.TreatmentDirection;
                                treatment.USED_MEDICINE = currentTreatmentFinishSDO.UsedMedicine;
                                treatment.CLINICAL_SIGNS = currentTreatmentFinishSDO.ClinicalSigns;
                                this._treatmentext.SUBCLINICAL_RESULT = currentTreatmentFinishSDO.SubclinicalResult;
                            }

                            EndTypeForm.FormTransfer form = new EndTypeForm.FormTransfer(this.moduleData, treatment, UpdateExamTreatmentFinish,this._treatmentext);
                            form.ShowDialog();
                        }
                        else if (data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                        {
                            //layoutControlIPanelUCExtend.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            DeathInitADO deathInitADO = new DeathInitADO();
                            deathInitADO.IsTextHolder = true;
                            deathInitADO.DeathDataSource = new DeathDataSourcesADO();
                            deathInitADO.DeathDataSource.CurrentHisTreatment = ExamTreatmentFinishInitADO.Treatment;
                            deathInitADO.DeathDataSource.HisDeathCauses = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>();
                            deathInitADO.DeathDataSource.HisDeathWithins = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>();

                            BackendDataWorker.Reset<V_HIS_DEATH_CERT_BOOK>();
                            var deathCertBooks = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_DEATH_CERT_BOOK>().Where(o => o.IS_ACTIVE == 1 && o.TOTAL > 0 && (o.FROM_NUM_ORDER + o.TOTAL - 1 > (o.CURRENT_DEATH_CERT_NUM  ?? 0)) && (o.BRANCH_ID == null || o.BRANCH_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId()));

                            if (deathCertBooks != null && deathCertBooks.Count() > 0)
                                deathInitADO.DeathDataSource.HisDeathCertBooks = deathCertBooks.ToList();
                            deathInitADO.BranchName = this.ExamTreatmentFinishInitADO.BranchName;
                            deathInitADO.CmndDate = this.ExamTreatmentFinishInitADO.CmndDate;
                            deathInitADO.CmndNumber = this.ExamTreatmentFinishInitADO.CmndNumber;
                            deathInitADO.CmndPlace = this.ExamTreatmentFinishInitADO.CmndPlace;
                            deathInitADO.DocumentType = this.ExamTreatmentFinishInitADO.DocumentType;

                            //deathProcessor = new DeathProcessor();
                            //this.ucDeath = (UserControl)deathProcessor.Run(deathInitADO);
                            //this.ucDeath.Dock = DockStyle.Fill;
                            cboTreatmentEndTypeExt.EditValue = null;
                            cboTreatmentEndTypeExt.Enabled = false;
                            //panelExamTreatmentFinish.Controls.Clear();
                            //panelExamTreatmentFinish.Controls.Add(this.ucDeath);
                            CauseOfDeathADO causeOfDeathADO = new CauseOfDeathADO();
                            causeOfDeathADO.Treatment = ExamTreatmentFinishInitADO.Treatment;
                            causeOfDeathADO.SevereIllNessInfo = ExamTreatmentFinishInitADO.SevereIllNessInfo;
                            causeOfDeathADO.ListEventsCausesDeath = ExamTreatmentFinishInitADO.ListEventsCausesDeath;
                            EnableControlAppoinment(false);
                            deathSdoResult = null;
                            causeResult = null;
                            frmPopUpSick frm = new frmPopUpSick(deathInitADO, causeOfDeathADO, ActionGetSdoDeathResult, ActionGetCauseResult);
                            frm.ShowDialog();
                        }
                        else if (data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                        {
                            EnableControlAppoinment(true);
                            //SetAppoinmentTime();
                            //dtTimeAppointment.Focus();
                            //dtTimeAppointment.SelectAll();
                            cboTreatmentEndTypeExt.EditValue = null;
                            cboTreatmentEndTypeExt.Enabled = true;

                            //if (ExamTreatmentFinishInitADO != null)
                            //    this._ucAdvise = new UcAdvise(ExamTreatmentFinishInitADO.currentRoomId);
                            //else
                            //    this._ucAdvise = new UcAdvise();
                            //this._ucAdvise.Dock = DockStyle.Fill;
                            panelExamTreatmentFinish.Controls.Clear();
                            //panelExamTreatmentFinish.Controls.Add(this._ucAdvise);

                            HIS_TREATMENT treatment = new HIS_TREATMENT();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, ExamTreatmentFinishInitADO.Treatment);
                            if (currentTreatmentFinishSDO != null)
                            {
                                //mở lại form để nhập thì vẫn đổ lại thông tin đã nhập.
                                treatment.TREATMENT_END_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN;
                                treatment.ADVISE = currentTreatmentFinishSDO.Advise;
                                treatment.APPOINTMENT_TIME = currentTreatmentFinishSDO.AppointmentTime;
                                treatment.APPOINTMENT_PERIOD_ID = currentTreatmentFinishSDO.AppointmentPeriodId;
                                if (currentTreatmentFinishSDO.AppointmentExamRoomIds != null && currentTreatmentFinishSDO.AppointmentExamRoomIds.Count > 0)
                                {
                                    treatment.APPOINTMENT_EXAM_ROOM_IDS = string.Join(",", currentTreatmentFinishSDO.AppointmentExamRoomIds);
                                }
                                else
                                {
                                    treatment.APPOINTMENT_EXAM_ROOM_IDS = null;
                                }
                            }

                            //check mặc định phòng khám lần sau là phòng hiện tại
                            if (String.IsNullOrWhiteSpace(treatment.APPOINTMENT_EXAM_ROOM_IDS))
                            {
                                treatment.APPOINTMENT_EXAM_ROOM_IDS = this.ExamTreatmentFinishInitADO.moduleData.RoomId.ToString();
                            }

                            treatment.OUT_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtEndTime.DateTime.ToString("yyyyMMddHHmmss"));

                            EndTypeForm.FormAppointment form = new EndTypeForm.FormAppointment(treatment, UpdateExamTreatmentFinish, ExamTreatmentFinishInitADO.IsBlockNumOrder);
                            form.ShowDialog();
                        }
                        else
                        {
                            panelExamTreatmentFinish.Controls.Clear();
                            EnableControlAppoinment(false);
                            cboTreatmentEndTypeExt.Enabled = true;
                            lciChiDinhDichVuhenKham.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ActionGetCauseResult(CauseOfDeathADO obj)
        {
            try
            {
                causeResult = obj;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => causeResult), causeResult));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ActionGetSdoDeathResult(HisTreatmentFinishSDO obj)
        {
            try
            {
                deathSdoResult = obj;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => deathSdoResult), deathSdoResult));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void UpdateExamTreatmentFinish(MOS.SDO.HisTreatmentFinishSDO obj)
        {
            try
            {
                currentTreatmentFinishSDO = obj;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentEndType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTreatmentEndTypeExt.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void chkSignAppoinment_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                {
                    //if (isNotLoadWhileChangeControlStateInFirst)
                    //{
                    //    return;
                    //}
                    WaitingManager.Show();
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignAppoinment.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (chkSignAppoinment.Checked ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = chkSignAppoinment.Name;
                        csAddOrUpdate.VALUE = (chkSignAppoinment.Checked ? "1" : "");
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

        private void chkSignBordereau_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignBordereau.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSignBordereau.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignBordereau.Name;
                    csAddOrUpdate.VALUE = (chkSignBordereau.Checked ? "1" : "");
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

        private void chkPrintBHXH_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintBHXH.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintBHXH.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintBHXH.Name;
                    csAddOrUpdate.VALUE = (chkPrintBHXH.Checked ? "1" : "");
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

        private void chkSignBHXH_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignBHXH.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSignBHXH.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignBHXH.Name;
                    csAddOrUpdate.VALUE = (chkSignBHXH.Checked ? "1" : "");
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

        private void chkInPhieuTrichLuc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkInPhieuTrichLuc.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkInPhieuTrichLuc.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkInPhieuTrichLuc.Name;
                    csAddOrUpdate.VALUE = (chkInPhieuTrichLuc.Checked ? "1" : "");
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

        private void chkKyPhieuTrichLuc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkKyPhieuTrichLuc.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkKyPhieuTrichLuc.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkKyPhieuTrichLuc.Name;
                    csAddOrUpdate.VALUE = (chkKyPhieuTrichLuc.Checked ? "1" : "");
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

        internal void FocusControl()
        {
            try
            {
                frmPopUpSick frm = new frmPopUpSick(sickInitADO, ActionGetSdoSickResult, true);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnICDInformation_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ExamTreatmentFinishInitADO != null)
                {
                    this.frmICDInformation = new frmICDInformation(this.moduleData, this.ExamTreatmentFinishInitADO.Treatment, SaveICDInfoClick);
                    this.frmICDInformation.ShowDialog();
                }
                else
                {
                    this.frmICDInformation = new frmICDInformation(this.moduleData, new HIS_TREATMENT(), SaveICDInfoClick);
                    this.frmICDInformation.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SaveICDInfoClick(HIS_TREATMENT treatmentForSaveICDInfo)
        {
            try
            {
                if (this.currentShowICDInformation != null)
                {
                    this.currentShowICDInformation.SHOW_ICD_CODE = treatmentForSaveICDInfo.SHOW_ICD_CODE;
                    this.currentShowICDInformation.SHOW_ICD_NAME = treatmentForSaveICDInfo.SHOW_ICD_NAME;
                    this.currentShowICDInformation.SHOW_ICD_SUB_CODE = treatmentForSaveICDInfo.SHOW_ICD_SUB_CODE;
                    this.currentShowICDInformation.SHOW_ICD_TEXT = treatmentForSaveICDInfo.SHOW_ICD_TEXT;
                }
                if (this.treatment != null && treatmentForSaveICDInfo != null)
                {
                    this.treatment.SHOW_ICD_CODE = treatmentForSaveICDInfo.SHOW_ICD_CODE;
                    this.treatment.SHOW_ICD_NAME = treatmentForSaveICDInfo.SHOW_ICD_NAME;
                    this.treatment.SHOW_ICD_SUB_CODE = treatmentForSaveICDInfo.SHOW_ICD_SUB_CODE;
                    this.treatment.SHOW_ICD_TEXT = treatmentForSaveICDInfo.SHOW_ICD_TEXT;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("___this.currentShowICDInformation: ", this.currentShowICDInformation));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintPrescription_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintPrescription.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintPrescription.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintPrescription.Name;
                    csAddOrUpdate.VALUE = (chkPrintPrescription.Checked ? "1" : "");
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

        private void chkPrintHosTransfer_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintHosTransfer.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintHosTransfer.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintHosTransfer.Name;
                    csAddOrUpdate.VALUE = (chkPrintHosTransfer.Checked ? "1" : "");
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

        private void chkIsExpXml4210Collinear_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkIsExpXml4210Collinear.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkIsExpXml4210Collinear.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkIsExpXml4210Collinear.Name;
                    csAddOrUpdate.VALUE = (chkIsExpXml4210Collinear.Checked ? "1" : "");
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
        private void LoadDataTocboUser()
        {
            try
            {
                this.lstReAcsUserADO = new List<AcsUserADO>();
                var acsUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
                       <ACS.EFMODEL.DataModels.ACS_USER>().Where(p => !string.IsNullOrEmpty(p.USERNAME) && p.IS_ACTIVE == 1).OrderBy(o => o.USERNAME).ToList();
                foreach (var item in acsUser)
                {
                    AcsUserADO ado = new AcsUserADO(item);

                    var VhisEmployee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME && o.IS_ACTIVE == 1);
                    if (VhisEmployee != null)
                    {
                        ado.DOB = VhisEmployee.DOB;
                        ado.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(VhisEmployee.DOB ?? 0);
                        ado.DIPLOMA = VhisEmployee.DIPLOMA;
                        ado.DEPARTMENT_CODE = VhisEmployee.DEPARTMENT_CODE;
                        ado.DEPARTMENT_ID = VhisEmployee.DEPARTMENT_ID;
                        ado.DEPARTMENT_NAME = VhisEmployee.DEPARTMENT_NAME;
                        this.lstReAcsUserADO.Add(ado);
                    }

                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "Họ tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, true, 400);
                ControlEditorLoader.Load(cboEndDeptSubs, this.lstReAcsUserADO.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
                cboEndDeptSubs.Properties.ImmediatePopup = true;
                ControlEditorLoader.Load(cboHospSubs, this.lstReAcsUserADO.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
                cboHospSubs.Properties.ImmediatePopup = true;
                if (this.ExamTreatmentFinishInitADO.Treatment != null)
                {

                    // code thêm từ đây
                    if (this.ExamTreatmentFinishInitADO.Treatment.HOSP_SUBS_DIRECTOR_LOGINNAME != null)
                    {
                        cboHospSubs.EditValue = this.ExamTreatmentFinishInitADO.Treatment.HOSP_SUBS_DIRECTOR_LOGINNAME;
                        LogSystem.Info($"ExamTreatmentFinishInitADO.Treatment.HOSP_SUBS_DIRECTOR_LOGINNAME != null {this.ExamTreatmentFinishInitADO.Treatment.HOSP_SUBS_DIRECTOR_LOGINNAME}");
                    }
                    else
                    {
                        var _vHisExecuteRooms = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(p => p.ROOM_ID == moduleData.RoomId);

                        if (_vHisExecuteRooms != null && _vHisExecuteRooms.HOSP_SUBS_DIRECTOR_LOGINNAME != null)    
                        {
                            cboHospSubs.EditValue = _vHisExecuteRooms.HOSP_SUBS_DIRECTOR_LOGINNAME;
                            LogSystem.Info($"_vHisExecuteRooms.HOSP_SUBS_DIRECTOR_LOGINNAME != null {_vHisExecuteRooms.HOSP_SUBS_DIRECTOR_LOGINNAME}");
                        }
                        else
                        {
                            var _VHisRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                            
                            var depar = listDepartment.FirstOrDefault(s => s.ID == _VHisRoom.DEPARTMENT_ID);
                            if (depar != null && depar.HOSP_SUBS_DIRECTOR_LOGINNAME != null)
                            {
                                cboHospSubs.EditValue = depar.HOSP_SUBS_DIRECTOR_LOGINNAME;
                                LogSystem.Info($"depar.HOSP_SUBS_DIRECTOR_LOGINNAME != null {depar.HOSP_SUBS_DIRECTOR_LOGINNAME}");
                            }
                        }
                    }    
                    if (this.ExamTreatmentFinishInitADO.Treatment.END_DEPT_SUBS_HEAD_LOGINNAME != null)
                    {
                        cboEndDeptSubs.EditValue = this.ExamTreatmentFinishInitADO.Treatment.END_DEPT_SUBS_HEAD_LOGINNAME;
                    }
                    else
                    {
                        if (Config.HisConfig.ENDDEPARTMENTSUBSHEADOPTION == "1")
                        {
                            var USER = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            if (USER != null)  
                            {
                                cboEndDeptSubs.EditValue = USER;
                            }
                        }
                    }
                    /// đến đây
                    
                    var department = listDepartment.FirstOrDefault(s => s.ID == (this.ExamTreatmentFinishInitADO.Treatment.LAST_DEPARTMENT_ID ?? 0));
                    if(department != null)
                    {
                        if (CheckNeedSignInstead(department.HEAD_LOGINNAME))
                        {
                            this.layoutControlItem30.AppearanceItemCaption.ForeColor = Color.Brown;
                        }
                        var branch = this.listBranch.FirstOrDefault(o => o.ID == (department.BRANCH_ID));
                        if(branch != null)
                        {
                            if (CheckNeedSignInstead(branch.DIRECTOR_LOGINNAME))
                            {
                                this.layoutControlItem31.AppearanceItemCaption.ForeColor = Color.Brown;
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
        private bool CheckNeedSignInstead(string login_name)
        {
            bool isSign = false;
            try
            {
                //var rs = HisEmployees.Where(s => s.LOGINNAME.Equals(login_name)).FirstOrDefault();
                HisEmployeeFilter emFilter = new HisEmployeeFilter();
                emFilter.LOGINNAME__EXACT = login_name;
                var rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EMPLOYEE>>("/api/HisEmployee/GetView", ApiConsumers.MosConsumer, emFilter, new CommonParam());
                if (rs != null)
                {
                    if (rs.First().IS_NEED_SIGN_INSTEAD ==  1) isSign = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return isSign;
        }
        private void cboEndDeptSubs_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEndDeptSubs.EditValue = null;
                    cboEndDeptSubs.Text = "";
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
                    cboHospSubs.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCheckIcd_Click(object sender, EventArgs e)
        {
            try
            {
                #region Lấy toàn bộ các CĐ của hsđt
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.TREATMENT_ID = ExamTreatmentFinishInitADO.Treatment.ID;
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                List<HIS_ICD> lstHisIcd = new List<HIS_ICD>();
                foreach (var item in result)
                {
                    if (!string.IsNullOrEmpty(item.ICD_CODE))
                    {
                        HIS_ICD icd = new HIS_ICD();
                        icd.ICD_CODE = item.ICD_CODE;
                        icd.ICD_NAME = item.ICD_NAME;
                        lstHisIcd.Add(icd);
                    }

                    if (!string.IsNullOrEmpty(item.ICD_SUB_CODE) || !string.IsNullOrEmpty(item.ICD_TEXT))
                    {
                        string[] arrIcd = !string.IsNullOrEmpty(item.ICD_SUB_CODE) ? item.ICD_SUB_CODE.Split(';') : null;
                        string[] arrIcdName = !string.IsNullOrEmpty(item.ICD_TEXT) ? item.ICD_TEXT.Split(';') : null;
                        if (arrIcd != null && arrIcd.Count() > 0)
                            {
                                for (int i = 0; i < arrIcd.Length; i++)
                                {
                                    HIS_ICD icd = new HIS_ICD();
                                    if (!string.IsNullOrEmpty(arrIcd[i]))
                                    {
                                        icd.ICD_CODE = arrIcd[i].Trim();
                                    }
                                    try
                                    {
                                        icd.ICD_NAME = arrIcdName != null ? arrIcdName[i].Trim() : null;
                                        lstHisIcd.Add(icd);
                                    }
                                    catch (Exception ex)
                                    {
                                        lstHisIcd.Add(icd);
                                    }
                                }
                            }
                        
                        if (arrIcdName != null && arrIcdName.Count() > 0 && arrIcd != null && arrIcd.Count() > 0 && arrIcd.Length < arrIcdName.Length)
                            {
                                for (int i = arrIcd.Length; i < arrIcdName.Length; i++)
                                {
                                    HIS_ICD icd = new HIS_ICD();
                                    icd.ICD_NAME = arrIcdName[i].Trim();
                                    lstHisIcd.Add(icd);

                                }
                            }else if (arrIcdName != null && arrIcdName.Count() > 0)
                        {
                            for (int i = 0; i < arrIcdName.Length; i++)
                            {
                                HIS_ICD icd = new HIS_ICD();
                                icd.ICD_NAME = arrIcdName[i].Trim();
                                lstHisIcd.Add(icd);

                            }
                        }
                    }
                }
                #endregion
                #region Phân loại các ICD
                foreach (var item in lstHisIcd)
                {
                    if (!string.IsNullOrEmpty(item.ICD_CODE) && !string.IsNullOrEmpty(item.ICD_NAME))
                    {
                        var icd = currentIcds.FirstOrDefault(o => o.ICD_CODE == item.ICD_CODE);
                        if (icd != null && icd.ICD_NAME == item.ICD_NAME)
                        {
                            item.IS_ACTIVE = 1;
                        }
                        else
                        {
                            item.IS_ACTIVE = 2;
                        }
                    }
                    if (string.IsNullOrEmpty(item.ICD_NAME))
                    {
                        item.IS_ACTIVE = 3;
                    }
                    if (string.IsNullOrEmpty(item.ICD_CODE))
                    {
                        item.IS_ACTIVE = 4;
                    }
                }
                List<IcdADO> lstIcdNew = new List<IcdADO>();
                foreach (var item in lstHisIcd)
                {
                    if (!string.IsNullOrEmpty(item.ICD_CODE) || !string.IsNullOrEmpty(item.ICD_NAME))

                        lstIcdNew.Add(new IcdADO() { ICD_CODE = item.ICD_CODE, ICD_NAME = item.ICD_NAME, IS_ACTIVE = item.IS_ACTIVE });
                }

                lstIcdNew = lstIcdNew.OrderBy(o => o.IS_ACTIVE).ThenBy(o => o.ICD_CODE).Distinct(new Compare()).ToList();

                //lstIcdNew = lstIcdNew.OrderBy(o => o.ICD_CODE).Distinct(new Compare()).ToList();
                #endregion
                SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                string icdSubCodeQ = null;
                string icdSubNameQ = null;
                string icdCode = null;
                string icdName = null;
                icdCode = txtIcdCode.Text.Trim();
                icdName = txtIcdMainText.Text.Trim();
                //if (!string.IsNullOrEmpty(icdCode))
                //{
                //List<IcdADO> lstIcd = lstIcdNew.Where(o => !string.IsNullOrEmpty(o.ICD_CODE) && o.ICD_CODE != icdCode).OrderBy(o => o.IS_ACTIVE).ToList();
                List<IcdADO> lstIcd = lstIcdNew.Where(o => o.ICD_CODE != icdCode && o.ICD_NAME != icdName).ToList();
                icdSubCodeQ = String.Join(";", lstIcd.Select(o => o.ICD_CODE).Distinct()) + ";";
                icdSubNameQ = String.Join(";", lstIcd.Select(o => o.ICD_NAME).Distinct()) + ";";
                //icdSubNameQ = String.Join(";", lstIcd.Where(o => o.IS_ACTIVE == 1).Select(o => o.ICD_NAME).Distinct()) + ";";
                //}
                //else
                //{
                //    var lstIcdActiveNotIcdCode = lstIcdNew.Where(o => o.IS_ACTIVE == 1 && o.ICD_CODE != icdCode).ToList();
                //    foreach (var item in lstIcdActiveNotIcdCode)
                //    {
                //        if (item.ICD_NAME == icdName)
                //            continue;
                //        icdSubCodeQ += item.ICD_CODE + ";";
                //        icdSubNameQ += item.ICD_NAME + ";";
                //    }
                //}
                //var lstHisIcdTmp = lstIcdNew.Where(o => o.IS_ACTIVE != 1 && o.ICD_CODE != icdCode).ToList();
                ////List<IcdADO> lstIcdOther = new List<IcdADO>();
                ////lstIcdOther.AddRange(lstIcdNew.Where(o => o.IS_ACTIVE != 1 && o.ICD_CODE == icdCode).ToList());
                //foreach (var item in lstHisIcdTmp.GroupBy(o => o.ICD_CODE).ToList())
                //{
                //    //lstIcdOther.AddRange(item.Where(o => o.ICD_NAME != item.First().ICD_NAME).ToList());
                //    if (item.First().ICD_NAME == icdName)
                //        continue;
                //    if (string.IsNullOrEmpty(icdCode))
                //        icdSubCodeQ += item.First().ICD_CODE + ";";
                //    foreach (var childName in item)
                //    {
                //        icdSubNameQ += childName.ICD_NAME + ";";
                //    }
                //}
                //var lstIcdActive = lstIcdNew.Where(o => o.IS_ACTIVE == 1 && o.ICD_CODE == icdCode).ToList();
                var lstIcdActive = lstIcdNew.Where(o => o.ICD_CODE == icdCode).ToList();
                foreach (var item in lstIcdActive)
                {
                    if (item.ICD_NAME == icdName)
                        continue;
                    icdSubNameQ += item.ICD_NAME + ";";
                }
                ////foreach (var item in lstIcdOther)
                ////{
                ////    if (item.ICD_NAME == icdName)
                ////        continue;
                ////    icdSubNameQ += item.ICD_NAME + ";";
                ////}
                #region Xử lý dấu ; 
                List<IcdADO> lstIcdErr = new List<IcdADO>();
                List<string> lstIcdCodeErrr = new List<string>();
                if (!string.IsNullOrEmpty(icdSubCodeQ) && (icdSubCodeQ.StartsWith(";") || icdSubCodeQ.EndsWith(";")))
                {
                    List<string> lstTmp = new List<string>();
                    var arr = icdSubCodeQ.Split(';');
                    foreach (var item in arr)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            string messErr = null;
                            if (!checkIcdManager.ProcessCheckIcd(null, item, ref messErr,false,false))
                            {
                                XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                                lstIcdCodeErrr.Add(item);
                                continue;
                            }
                            lstTmp.Add(item);
                        }
                    }
                    icdSubCodeQ = string.Join(";", lstTmp);
                }
                if (!string.IsNullOrEmpty(icdSubNameQ) && (icdSubNameQ.StartsWith(";") || icdSubNameQ.EndsWith(";")))
                {
                    List<string> lstTmp = new List<string>();
                    lstIcdErr = lstIcdNew.Where(o=> lstIcdCodeErrr.Exists(p=>p.Equals(o.ICD_CODE))).ToList();
                    var arr = icdSubNameQ.Split(';');
                    foreach (var item in arr)
                    {
                        if (!string.IsNullOrEmpty(item) && !lstIcdErr.Exists(p=>p.ICD_CODE.Equals(item)))
                            lstTmp.Add(item);
                    }
                    icdSubNameQ = string.Join(";", lstTmp);
                }
                #endregion
                LoaducSecondaryIcd(icdSubCodeQ, icdSubNameQ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintBordereau_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }               
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? (this.currentControlStateRDO.Where(o => o.KEY == chkPrintBordereau.Name && o.MODULE_LINK == moduleLink) != null && this.currentControlStateRDO.Where(o => o.KEY == chkPrintBordereau.Name && o.MODULE_LINK == moduleLink).ToList().Count > 0 ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintBordereau.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null) : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintBordereau.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintBordereau.Name;
                    csAddOrUpdate.VALUE = (chkPrintBordereau.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
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
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? (this.currentControlStateRDO.Where(o => o.KEY == chkPrintExam.Name && o.MODULE_LINK == moduleLink) != null && this.currentControlStateRDO.Where(o => o.KEY == chkPrintExam.Name && o.MODULE_LINK == moduleLink).ToList().Count > 0 ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintExam.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null) : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintExam.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintExam.Name;
                    csAddOrUpdate.VALUE = (chkPrintExam.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
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
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? (this.currentControlStateRDO.Where(o => o.KEY == chkSignExam.Name && o.MODULE_LINK == moduleLink) != null && this.currentControlStateRDO.Where(o => o.KEY == chkSignExam.Name && o.MODULE_LINK == moduleLink).ToList().Count > 0 ? this.currentControlStateRDO.Where(o => o.KEY == chkSignExam.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null) : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSignExam.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignExam.Name;
                    csAddOrUpdate.VALUE = (chkSignExam.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintAppoinment_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                {                  
                    WaitingManager.Show();
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintAppoinment.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (chkPrintAppoinment.Checked ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = chkPrintAppoinment.Name;
                        csAddOrUpdate.VALUE = (chkPrintAppoinment.Checked ? "1" : "");
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

        private void cboEndDeptSubs_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(cboEndDeptSubs.Text))
                {
                    cboEndDeptSubs.EditValue = null;
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
                if (string.IsNullOrEmpty(cboHospSubs.Text))
                {
                    cboHospSubs.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
