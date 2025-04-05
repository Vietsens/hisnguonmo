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
using HIS.Desktop.DelegateRegister;
using HIS.UC.UCHeniInfo.ADO;
using Inventec.Common.Logging;
using HIS.UC.UCHeniInfo.HisPatient;
using His.UC.UCHeniInfo.Design;
using Inventec.Common.QrCodeBHYT;
using MOS.SDO;
using HIS.UC.UCHeniInfo.Data;
using HIS.UC.UCHeniInfo.Utils;
using HIS.UC.UCHeniInfo.ControlProcess;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using MOS.LibraryHein.Bhyt.HeinRightRouteType;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ADO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.UCHeniInfo.CustomValidateRule;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;

namespace HIS.UC.UCHeniInfo
{
    public partial class UCHeinInfo : UserControlBase
    {
        #region Declare

        DelegateFocusNextUserControl dlgFocusNextUserControl;
        DelegateValidationUserControl dlgValidationControl;
        ProcessFillDataCareerUnder6AgeByHeinCardNumber dlgProcessFillDataCareerUnder6AgeByHeinCardNumber;
        FillDataPatientSDOToRegisterForm dlgfillDataPatientSDOToRegisterForm;
        CheckExamHistoryByHeinCardNumber dlgcheckExamHistory;
        DelegateAutoCheckCC dlgAutoCheckCC;
        DelegateCheckTT dlgCheckTT;
        GetIsChild dlgGetIsChild;
        DelegateVisible dlgIsEnableEmergency;
        DelegateVisible dlgShowThongTinChuyenTuyen;
        DelegateVisible dlgDisableBtnTTCT;
        DelegateSetCareerByHeinCardNumber dlgSetCareerByHeinCardNumber;
        UpdateTranPatiDataByPatientOld updateTranPatiDataByPatientOld;
        DelegateSend3WBhytCode dlgSend3WBhytCode;
        DelegateCheckSS dlgCheckSS;
        Action dlgProcessChangePatientDob;
        public ResultDataADO ResultDataADO { get; set; }
        UCHeinADO dataHein = new UCHeinADO();
        bool IsDefaultRightRouteType { get; set; }
        HisPatientSDO currentPatientSdo;
        bool isTempQN;
        bool isEdit;
        string isShowCheckKhongKTHSD { get; set; }
        bool isCheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong;
        bool isPatientOld = false;

        HeinCardData heinCardData;

        bool IsDungTuyenCapCuuByTime = false;
        bool ChangeDataByCard = false;
        string CodeProvince = "";

        HIS.UC.UCPatientRaw.UCPatientRaw currentPatientRaw;
        long treatmentId;
        Inventec.Desktop.Common.Modules.Module module;
        object dataSourceCboHeinRightRouteTemp;

        bool IsCheckAutoDT = false;
        bool IsShowMessage = false;
        enum RightRouterFactory
        {
            RIGHT_ROUTER,
            WRONG_ROUTER,
            WRONG_ROUTER__CHOICE_RIGHT,
            WRONG_ROUTER__CHOICE_RIGHT_FOR_MEDI_ORG_ROUTE,
            WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTCC,
            WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTGT,
            WRONG_ROUTER__CHOICE_RIGHT__DELETE_CHOICE_TYPE,
            WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_HASAPPOINTMENT,
        }
        bool isInitFormData = false;
        #endregion

        #region Constructor - Load

        public UCHeinInfo()
            : base("HIS.Desktop.Plugins.RegisterV2", "UCHeinInfo")
        {
            Inventec.Common.Logging.LogSystem.Debug("UCHeinInfo .1");
            InitializeComponent();
            Inventec.Common.Logging.LogSystem.Debug("UCHeinInfo .2");
        }

        private void SetSourceHeinRightRoute()
        {
            try
            {
                DataStore.HeinRightRouteTypes = new List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData>();
                DataStore.HeinRightRouteTypes.Add(new MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData(MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE, MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteStore.GetByCode(MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE).HeinRightRouteName));
                DataStore.HeinRightRouteTypes.Add(new MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData(MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE, MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteStore.GetByCode(MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE).HeinRightRouteName));
                DataStore.HeinRightRouteTypes.AddRange(MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeStore.Get());
                if (HisConfigCFG.IsNotDisplayedRouteTypeOver)
                {
                    DataStore.HeinRightRouteTypes = DataStore.HeinRightRouteTypes.Where(o => o.HeinRightRouteTypeCode != MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCHeinInfo_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCHeinInfo_Load .1");
                //InitFieldFromAsync();                
                Inventec.Common.Logging.LogSystem.Debug("UCHeinInfo_Load .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCHeinInfo/UCHeinInfo_Load:\n" + ex);
            }
        }



        #endregion

        #region public Method

        public async Task InitFieldFromAsync()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCHeinInfo.InitFieldFromAsync .1");
                if (!isInitFormData)
                {
                    this.SetDataSource();
                    //DataStore.LoadConfig();
                    this.heinCardData = new HeinCardData();
                    SetCaptionByLanguageKey();
                    this.TuDongCheckCapCuuTheoGiaTriTuyen();
                    this.lciIsBhytHolded.TextVisible = true;
                    this.lciIsBhytHolded.TextSize = lciHasAbsentLetter.TextSize;
                    this.chkSs.Enabled = false;
                    this.chkSs.Checked = false;
                    isInitFormData = true;
                }

                Inventec.Common.Logging.LogSystem.Debug("UCHeinInfo.InitFieldFromAsync .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool IsInitFormData
        {
            get { return isInitFormData; }
        }

        public void SetValueIsInitFormData(bool _isInitFormData)
        {
            this.isInitFormData = _isInitFormData;
        }

        public void SetDataSource()
        {
            try
            {
                EditorLoaderProcessor.InitComboCommon(this.cboDKKCBBD, MediOrgDataWorker.MediOrgADOs, "MEDI_ORG_CODE", "MEDI_ORG_NAME", 250, "MEDI_ORG_CODE", 70, "MEDI_ORG_NAME_UNSIGNED", 5, 0);
                EditorLoaderProcessor.InitComboCommon(this.cboNoiSong, MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaStore.Get(), "HeinLiveCode", "HeinLiveName", "HeinLiveCode");
                SetSourceHeinRightRoute();
                EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, DataStore.HeinRightRouteTypes, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                dataSourceCboHeinRightRouteTemp = DataStore.HeinRightRouteTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitInputData(DataInitUCHeniInfo dataInitUCHeniInfo)
        {
            try
            {
                this.dlgFocusNextUserControl = dataInitUCHeniInfo.dlgFocusNextUserControl;
                this.dlgValidationControl = dataInitUCHeniInfo.dlgValidationControl;
                this.dlgCheckSS = dataInitUCHeniInfo.dlgCheckSS;//check SS
                this.dlgProcessFillDataCareerUnder6AgeByHeinCardNumber = dataInitUCHeniInfo.dlgProcessFillDataCareerUnder6AgeByHeinCardNumber;
                this.dlgfillDataPatientSDOToRegisterForm = dataInitUCHeniInfo.dlgFillDataPatientSDOToRegisterForm;
                this.dlgcheckExamHistory = dataInitUCHeniInfo.dlgCheckExamHistory;
                this.dlgGetIsChild = dataInitUCHeniInfo.dlgGetIsChild;
                this.dlgAutoCheckCC = dataInitUCHeniInfo.dlgAutoCheckCC;
                this.IsDefaultRightRouteType = dataInitUCHeniInfo.IsDefaultRightRouteType;
                this.isEdit = dataInitUCHeniInfo.IsEdit;
                HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT = dataInitUCHeniInfo.PATIENT_TYPE_ID__BHYT;
                this.isTempQN = dataInitUCHeniInfo.IsTempQN;
                this.isShowCheckKhongKTHSD = dataInitUCHeniInfo.IsShowCheckKhongKTHSD;
                this.isTempQN = dataInitUCHeniInfo.IsTempQN;
                this.isCheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong = dataInitUCHeniInfo.IsCheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong;
                this.updateTranPatiDataByPatientOld = dataInitUCHeniInfo.UpdateTranPatiDataByPatientOld;
                this.CheckHSDAndTECard();
                this.LoadDefautlRightRouteTypeByKeyAndTime();
                this.InitValidateRule(HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.PatientTypeID_Default);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void RefreshUserControl()
        {
            try
            {
                if (this.chkHasCardTemp.Checked)
                {
                    EditorLoaderProcessor.InitComboCommon(this.cboDKKCBBD, MediOrgDataWorker.MediOrgADOs, "MEDI_ORG_CODE", "MEDI_ORG_NAME", 250, "MEDI_ORG_CODE", 70, "MEDI_ORG_NAME_UNSIGNED", 5, 0);
                }

                this.chkHasCardTemp.Checked = false;
                this.chkHasCardTemp.Enabled = false;
                this.txtSoThe.EditValue = "";
                this.cboSoThe.EditValue = null;
                this.cboSoThe.Properties.DataSource = null;
                this.txtAddress.Text = "";
                this.txtHeinCardFromTime.Text = "";
                this.dtHeinCardFromTime.Text = "";
                this.txtHeinCardToTime.Text = "";
                this.dtHeinCardToTime.Text = "";
                this.txtMaDKKCBBD.Text = "";
                this.cboDKKCBBD.EditValue = null;
                this.txtDu5Nam.Text = "";
                this.dtDu5Nam.Text = "";
                this.IsDungTuyenCapCuuByTime = false;
                this.LoadDefautlRightRouteTypeByKeyAndTime();
                this.cboHeinRightRoute.EditValue = null;
                this.chkJoin5Year.Checked = this.chkPaid6Month.Checked = this.chkKhongKTHSD.Checked = false;
                this.lciKhongKTHSD.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.cboNoiSong.EditValue = null;
                this.cboNoiSong.Properties.Buttons[1].Visible = false;
                this.txtFreeCoPainTime.Text = "";
                this.txtMucHuong.Text = "";
                this.heinCardData = new HeinCardData();
                this.oldHeinCardNumber = null;
                this.dataHein = new UCHeinADO();
                this.currentPatientSdo = null;
                this.isPatientOld = false;
                this.ResetRequiredField();
                this.chkHasAbsentLetter.Checked = false;
                this.chkHasWorkingLetter.Checked = false;
                this.chkIsTt46.Checked = false;
                this.chkIsBhytHolded.Checked = true;
                this.lciHeinRightRoute.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                this.IsShowMessage = false;
                this.SetTreatmentId(0);
                this.chkSs.Checked = false;
                this.chkSs.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateChangePatientDob(Action _dlgProcessChangePatientDob)
        {
            this.dlgProcessChangePatientDob = _dlgProcessChangePatientDob;
        }

        public void ChangeRoomNotEmergency()
        {
            this.IsDungTuyenCapCuuByTime = false;
            MediOrgSelectRowChange(false, (cboNoiSong.EditValue ?? "").ToString());
        }
        #endregion


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCHeinInfo
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.UCHeniInfo.Resources.Lang", typeof(UCHeinInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCheckInfoBHYT.ToolTip = Inventec.Common.Resource.Get.Value("UCHeinInfo.btnCheckInfoBHYT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtNote.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHeinInfo.txtNote.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsTt46.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHeinInfo.chkIsTt46.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsTt46.ToolTip = Inventec.Common.Resource.Get.Value("UCHeinInfo.chkIsTt46.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHasAbsentLetter.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHeinInfo.chkHasAbsentLetter.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHasAbsentLetter.ToolTip = Inventec.Common.Resource.Get.Value("UCHeinInfo.chkHasAbsentLetter.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsBhytHolded.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHeinInfo.chkIsBhytHolded.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHasWorkingLetter.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHeinInfo.chkHasWorkingLetter.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHasWorkingLetter.ToolTip = Inventec.Common.Resource.Get.Value("UCHeinInfo.chkHasWorkingLetter.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtDu5Nam.ToolTip = Inventec.Common.Resource.Get.Value("UCHeinInfo.txtDu5Nam.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkKhongKTHSD.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHeinInfo.chkKhongKTHSD.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboNoiSong.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHeinInfo.cboNoiSong.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboNoiSong.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHeinInfo.cboNoiSong.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtFreeCoPainTime.ToolTip = Inventec.Common.Resource.Get.Value("UCHeinInfo.txtFreeCoPainTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPaid6Month.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHeinInfo.chkPaid6Month.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkJoin5Year.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHeinInfo.chkJoin5Year.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                toolTipItem2.Text = Inventec.Common.Resource.Get.Value("toolTipItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHeinRightRoute.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHeinInfo.cboHeinRightRoute.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHeinRightRoute.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHeinInfo.cboHeinRightRoute.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDKKCBBD.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHeinInfo.cboDKKCBBD.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtHeinCardToTime.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHeinInfo.dtHeinCardToTime.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                toolTipItem3.Text = Inventec.Common.Resource.Get.Value("toolTipItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSoThe.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHeinInfo.cboSoThe.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHasCardTemp.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHeinInfo.chkHasCardTemp.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                toolTipItem4.Text = Inventec.Common.Resource.Get.Value("toolTipItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gboxHeinCardInformation.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.gboxHeinCardInformation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHasDobCertificate.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciHasDobCertificate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinCardNumber.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciHeinCardNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinCardToTime.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciHeinCardToTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinCardAddress.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciHeinCardAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKCBBĐ.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciKCBBĐ.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKCBBĐ.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciKCBBĐ.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinRightRoute.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciHeinRightRoute.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinCardFromTime.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciHeinCardFromTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKhongKTHSD.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciKhongKTHSD.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lci5Y.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHeinInfo.layoutControlItem2.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lci5Y.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciJoin5Year.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciJoin5Year.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciJoin5Year.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciJoin5Year.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKV.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciKV.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPaid6Month.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciPaid6Month.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPaid6Month.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciPaid6Month.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFreeCoPainTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciFreeCoPainTime.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFreeCoPainTime.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciFreeCoPainTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciHasWorkingLetter.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciHasWorkingLetter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsBhytHolded.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciIsBhytHolded.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciHasAbsentLetter.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciHasAbsentLetter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciIsTt46.Text = Inventec.Common.Resource.Get.Value("UCHeinInfo.lciIsTt46.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #region Event SoThe

        private void txtSoThe_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        private void txtSoThe_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string heinCardNumber = this.txtSoThe.Text;
                    heinCardNumber = HeinUtils.TrimHeinCardNumber(heinCardNumber.Replace(" ", "").Replace("  ", "").ToUpper().Trim());
                    Send3WCode();
                    this.FocusTodtHeinCardFromTime();

                    string cardFormat = this.txtSoThe.Text;
                    cardFormat = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(cardFormat, System.Drawing.FontStyle.Bold);
                    cardFormat = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(cardFormat, Color.Green);
                    LogSystem.Debug("txtSoThe_KeyDown => So the ban dau = " + this.txtSoThe.Text + ", so the sau khi da xu ly chuoi = " + heinCardNumber);
                    bool valid = true;
                    valid = valid && new MOS.LibraryHein.Bhyt.BhytHeinProcessor().IsValidHeinCardNumber(heinCardNumber);
                    if (valid && !String.IsNullOrEmpty(heinCardNumber))
                    {
                        this.dxErrorProviderControl.ClearErrors();
                        var listResult = HisPatientGet.GetSDO(heinCardNumber);
                        if (listResult != null && listResult.Count > 0)
                        {
                            LogSystem.Info("txtSoThe_KeyDown => Tim thay " + listResult.Count + " BN co So the = " + heinCardNumber + ".");
                            if (listResult.Count > 1)
                            {
                                frmPatientChoice frm = new frmPatientChoice(listResult, this.FillDataAfterSelectOnePatient, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>());
                                frm.ShowDialog();
                            }
                            else
                            {
                                this.currentPatientSdo = listResult[0];
                                //Fill dữ liệu bệnh nhân (gọi sang module tiếp đón) & dữ liệu đối tượng điều trị vào form
                                this.FillDataAfterSelectOnePatient(listResult[0]);
                                oldHeinCardNumber = null;
                                CheckExamHistoryFromBHXHApi(null);
                            }

                            if (this.currentPatientSdo != null && DateTime.Now.Year - this.currentPatientSdo.VIR_DOB_YEAR > 5)
                            {
                                chkSs.Enabled = false;
                                chkSs.Checked = false;
                            }


                        }
                        else if (!String.IsNullOrEmpty(this.txtSoThe.Text) && !(this.txtSoThe.EditValue.Equals(this.txtSoThe.OldEditValue == null ? "" : this.txtSoThe.OldEditValue)))
                        {
                            oldHeinCardNumber = null;
                            CheckExamHistoryFromBHXHApi(null);
                            LogSystem.Info("Ket thuc check thong tuyen tu event txtSoThe_Keydown");
                        }
                        if (this.dlgSetCareerByHeinCardNumber != null && this.isPatientOld == false)
                            this.dlgSetCareerByHeinCardNumber(heinCardNumber);
                        LogSystem.Info("txtSoThe_Keydown");
                        if (this.dlgProcessChangePatientDob != null)
                            this.dlgProcessChangePatientDob();
                        txtHeinCardFromTime.Focus();
                        txtHeinCardFromTime.SelectAll();
                    }
                    else
                    {
                        if (chkHasCardTemp.Checked)
                        {
                            txtHeinCardFromTime.Focus();
                            txtHeinCardFromTime.SelectAll();
                        }
                        else
                        {
                            this.dxErrorProviderControl.SetError(this.txtSoThe, HIS.UC.UCHeniInfo.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapSoTheBHYTKhongHopLe));
                            FocusTotxtSoThe();
                        }
                    }

                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Down || e.KeyCode == Keys.PageDown)
                {
                    if (!String.IsNullOrEmpty(this.txtSoThe.Text) && !(this.txtSoThe.EditValue.ToString().Replace("-", "").Equals(this.txtSoThe.OldEditValue == null ? "" : this.cboSoThe.OldEditValue)))
                    {
                        oldHeinCardNumber = null;
                        this.CheckExamHistoryFromBHXHApi(null);
                        LogSystem.Info("Ket thuc check thong tuyen tu event txtSoThe_Keydown");
                    }
                    this.cboSoThe.ShowPopup();
                    this.cboSoThe.SelectAll();
                    PopupProcess.SelectFirstRowPopup(this.cboSoThe);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSoThe_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = HIS.UC.UCHeniInfo.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapSoTheBHYTKhongHopLe);
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void Send3WCode()
        {
            try
            {
                string sendData = "";
                if (!string.IsNullOrEmpty(this.txtSoThe.Text))
                {
                    string heinCardNumber = this.txtSoThe.Text;
                    heinCardNumber = HeinUtils.TrimHeinCardNumber(heinCardNumber.Replace(" ", "").Replace("  ", "").ToUpper().Trim());
                    sendData = heinCardNumber.Substring(0, 3);

                }
                if (this.dlgSend3WBhytCode != null)
                {
                    dlgSend3WBhytCode(sendData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSoThe_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                oldHeinCardNumber = txtSoThe.Text.Trim();


                this.CheckHSDAndTECard();
                if (this.currentPatientRaw != null && this.currentPatientRaw.GetValue().DOB > 0)
                {
                    DateTime dateofbirth = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentPatientRaw.GetValue().DOB) ?? DateTime.MinValue;
                    if (dateofbirth != DateTime.MinValue)
                    {
                        if (DateTime.Now.Year - dateofbirth.Year < 6)
                        {
                            this.chkSs.Enabled = true;

                        }
                        else
                        {
                            this.chkSs.Enabled = false;
                            this.chkSs.Checked = false;

                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSoThe_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                this.HeinCardSelectRowHandler((MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER)this.cboSoThe.Properties.GetDataSourceRowByKeyValue(cboSoThe.EditValue));
                if (this.cboSoThe.EditValue != null && !(this.cboSoThe.EditValue.ToString().Replace("-", "").Equals(this.cboSoThe.OldEditValue == null ? "" : this.cboSoThe.OldEditValue)))
                {
                    oldHeinCardNumber = null;
                    this.CheckExamHistoryFromBHXHApi(FocusOutOfUc);
                    if (this.dlgProcessChangePatientDob != null)
                        this.dlgProcessChangePatientDob();
                    LogSystem.Info("Ket thuc check thong tuyen tu event cboSoThe_Closed");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSoThe_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.HeinCardSelectRowHandler((MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER)this.cboSoThe.Properties.GetDataSourceRowByKeyValue(cboSoThe.EditValue));
                    if (this.cboSoThe.EditValue != null && !(this.cboSoThe.EditValue.ToString().Replace("-", "").Equals(this.cboSoThe.OldEditValue == null ? "" : this.cboSoThe.OldEditValue)))
                    {
                        oldHeinCardNumber = null;
                        this.CheckExamHistoryFromBHXHApi(null);
                        if (this.dlgProcessChangePatientDob != null)
                            this.dlgProcessChangePatientDob();
                    }
                    this.cboSoThe.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSoThe_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {

        }

        #endregion

        #region Event HeinCardFromTime

        private void dtHeinCardFromTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    DateTime? dt = HIS.UC.UCHeniInfo.Utils.HeinUtils.ConvertDateStringToSystemDate(this.dtHeinCardFromTime.Text);
                    this.dtHeinCardFromTime.Visible = false;
                    this.dtHeinCardFromTime.Update();
                    this.txtHeinCardFromTime.Text = this.dtHeinCardFromTime.DateTime.ToString("dd/MM/yyyy");
                    this.txtHeinCardToTime.Focus();
                    this.txtHeinCardToTime.SelectAll();
                    if (!String.IsNullOrEmpty(this.txtHeinCardFromTime.Text) && !(this.txtHeinCardFromTime.EditValue.Equals(this.txtHeinCardFromTime.OldEditValue == null ? "" : this.txtHeinCardFromTime.OldEditValue))
                         )
                        this.CheckExamHistoryFromBHXHApi(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinCardFromTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.dtHeinCardFromTime.EditValue != null && this.dtHeinCardFromTime.DateTime != DateTime.MinValue)
                {
                    if (this.dtHeinCardFromTime.DateTime.Date > DateTime.Now.Date)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.His_UCHein__TheBHYTChuaDenHanSuDung), MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), DefaultBoolean.True);
                        this.txtHeinCardFromTime.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtHeinCardFromTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    this.dtHeinCardFromTime.Visible = false;
                    this.dtHeinCardFromTime.Update();
                    this.txtHeinCardFromTime.Text = this.dtHeinCardFromTime.DateTime.ToString("dd/MM/yyyy");

                    if (!String.IsNullOrEmpty(this.txtHeinCardFromTime.Text) && !(this.txtHeinCardFromTime.EditValue.Equals(this.txtHeinCardFromTime.OldEditValue == null ? "" : this.txtHeinCardFromTime.OldEditValue))
                      )
                        this.CheckExamHistoryFromBHXHApi(null);

                    FocusTotxtHeinCardToTime();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCardFromTime_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = HIS.UC.UCHeniInfo.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void txtHeinCardFromTime_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = HIS.UC.UCHeniInfo.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardFromTime.EditValue = dt;
                        this.dtHeinCardFromTime.Update();
                    }

                    this.dtHeinCardFromTime.Visible = true;
                    this.dtHeinCardFromTime.ShowPopup();
                    this.dtHeinCardFromTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCardFromTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DateTime? dt = HIS.UC.UCHeniInfo.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardFromTime.EditValue = dt;
                        this.dtHeinCardFromTime.Update();

                        if (!String.IsNullOrEmpty(this.txtHeinCardFromTime.Text) && !(this.txtHeinCardFromTime.EditValue.Equals(this.txtHeinCardFromTime.OldEditValue == null ? "" : this.txtHeinCardFromTime.OldEditValue)))
                            this.CheckExamHistoryFromBHXHApi(null);
                    }

                    this.txtHeinCardToTime.Focus();
                    this.txtHeinCardToTime.SelectAll();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = HIS.UC.UCHeniInfo.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardFromTime.EditValue = dt;
                        this.dtHeinCardFromTime.Update();
                    }

                    this.dtHeinCardFromTime.Visible = true;
                    this.dtHeinCardFromTime.ShowPopup();
                    this.dtHeinCardFromTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCardFromTime_Validated(object sender, EventArgs e)
        {
            try
            {
                //if (!String.IsNullOrEmpty(this.txtHeinCardFromTime.Text) && !(this.txtHeinCardFromTime.EditValue.Equals(this.txtHeinCardFromTime.OldEditValue == null ? "" : this.txtHeinCardFromTime.OldEditValue)))
                //    this.CheckExamHistoryFromBHXHApi(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region HeinCardToTime

        private void dtHeinCardToTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtHeinCardToTime.Visible = false;
                    this.dtHeinCardToTime.Update();
                    this.txtHeinCardToTime.Text = this.dtHeinCardToTime.DateTime.ToString("dd/MM/yyyy");
                    if (!String.IsNullOrEmpty(this.txtHeinCardToTime.Text) && !(this.txtHeinCardToTime.EditValue.Equals(this.txtHeinCardToTime.OldEditValue == null ? "" : this.txtHeinCardToTime.OldEditValue)))
                        this.CheckExamHistoryFromBHXHApi(null);
                    this.txtAddress.Focus();
                    this.txtAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinCardToTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtHeinCardToTime.Visible = false;
                    this.dtHeinCardToTime.Update();
                    this.txtHeinCardToTime.Text = this.dtHeinCardToTime.DateTime.ToString("dd/MM/yyyy");
                    //SendKeys.Send("{TAB}");
                    this.txtAddress.Focus();
                    this.txtAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinCardToTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.dtHeinCardToTime.EditValue != null && this.dtHeinCardToTime.DateTime != DateTime.MinValue)
                {
                    if (this.isShowCheckKhongKTHSD == "1")
                        this.CheckHSDAndTECard();
                    else if (!ChangeDataByCard && this.dtHeinCardToTime.DateTime.Date < DateTime.Now.Date)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.His_UCHein__TheBHYTDaHetHanSuDung), MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), DefaultBoolean.True);
                        this.txtHeinCardToTime.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHeinCardToTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = HIS.UC.UCHeniInfo.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardToTime.EditValue = dt;
                        this.dtHeinCardToTime.Update();
                    }

                    this.dtHeinCardToTime.Visible = true;
                    this.dtHeinCardToTime.Focus();
                    this.dtHeinCardToTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCardToTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DateTime? dt = HIS.UC.UCHeniInfo.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardToTime.EditValue = dt;
                        this.dtHeinCardToTime.Update();
                    }
                    this.CheckExamHistoryFromBHXHApi(FocusOutOfUc);//xuandv
                                                                   //SendKeys.Send("{TAB}");
                    this.txtAddress.Focus();
                    this.txtAddress.SelectAll();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = HIS.UC.UCHeniInfo.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardToTime.EditValue = dt;
                        this.dtHeinCardToTime.Update();
                    }

                    this.dtHeinCardToTime.Visible = true;
                    this.dtHeinCardToTime.ShowPopup();
                    this.dtHeinCardToTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region TDDu5Nam

        private void dtDu5nam_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtDu5Nam.Visible = false;
                    this.dtDu5Nam.Update();
                    if (this.dtDu5Nam.EditValue != null && this.dtDu5Nam.DateTime != DateTime.MinValue)
                        this.txtDu5Nam.Text = this.dtDu5Nam.DateTime.ToString("dd/MM/yyyy");
                    else
                        this.txtDu5Nam.Text = null;
                    if (this.txtFreeCoPainTime.Enabled)
                    {
                        this.txtFreeCoPainTime.Focus();
                        this.txtFreeCoPainTime.SelectAll();
                    }
                    else
                    {
                        this.FocusNextControlFreeCoPainTime();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDu5Nam_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtDu5Nam.Visible = false;
                    this.dtDu5Nam.Update();
                    if (this.dtDu5Nam.EditValue != null && this.dtDu5Nam.DateTime != DateTime.MinValue)
                        this.txtDu5Nam.Text = this.dtDu5Nam.DateTime.ToString("dd/MM/yyyy");
                    else
                        this.txtDu5Nam.Text = null;
                    // SendKeys.Send("{TAB}");
                    if (this.txtFreeCoPainTime.Enabled)
                    {
                        this.txtFreeCoPainTime.Focus();
                        this.txtFreeCoPainTime.SelectAll();
                    }
                    else
                    {
                        this.FocusNextControlFreeCoPainTime();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDu5Nam_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = HIS.UC.UCHeniInfo.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtDu5Nam.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtDu5Nam.EditValue = dt;
                        this.dtDu5Nam.Update();
                    }

                    this.dtDu5Nam.Visible = true;
                    this.dtDu5Nam.Focus();
                    this.dtDu5Nam.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDu5Nam_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DateTime? dt = HIS.UC.UCHeniInfo.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtDu5Nam.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtDu5Nam.EditValue = dt;
                        this.dtDu5Nam.Update();
                    }
                    // SendKeys.Send("{TAB}");
                    if (this.txtFreeCoPainTime.Enabled)
                    {
                        this.txtFreeCoPainTime.Focus();
                        this.txtFreeCoPainTime.SelectAll();
                    }
                    else
                    {
                        this.FocusNextControlFreeCoPainTime();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = HIS.UC.UCHeniInfo.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtDu5Nam.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtDu5Nam.EditValue = dt;
                        this.dtDu5Nam.Update();
                    }

                    this.dtDu5Nam.Visible = true;
                    this.dtDu5Nam.ShowPopup();
                    this.dtDu5Nam.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region DKKCBBD

        private void txtMaDKKCBBD_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadNoiDKKCBBDCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDKKCBBD_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDKKCBBD.EditValue != null)
                    {
                        this.MediOrgSelectRowChange(true, (cboNoiSong.EditValue ?? "").ToString());
                        if (this.cboDKKCBBD.EditValue.Equals(this.cboDKKCBBD.OldEditValue == null ? "" : this.cboDKKCBBD.OldEditValue) == false)
                            this.CheckExamHistoryFromBHXHApi(FocusOutOfUc);
                    }

                    if (this.cboHeinRightRoute.EditValue == null)
                    {
                        this.cboHeinRightRoute.Focus();
                        this.cboHeinRightRoute.ShowPopup();
                    }
                    else if (dlgFocusNextUserControl != null) dlgFocusNextUserControl();
                    //else
                    //{
                    //    this.cboHeinRightRoute.Focus();
                    //    this.cboHeinRightRoute.ShowPopup();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDKKCBBD_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.A)
                {
                    this.cboDKKCBBD.Focus();
                    this.cboDKKCBBD.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (cboDKKCBBD.EditValue != null)
                        this.MediOrgSelectRowChange(true, (cboNoiSong.EditValue ?? "").ToString());

                    if (this.cboHeinRightRoute.EditValue == null)
                    {
                        this.cboHeinRightRoute.Focus();
                        this.cboHeinRightRoute.ShowPopup();
                    }
                    else if (dlgFocusNextUserControl != null) dlgFocusNextUserControl();
                    e.Handled = true;
                }
                else
                {
                    this.cboDKKCBBD.ShowPopup();
                    PopupProcess.SelectFirstRowPopup(this.cboDKKCBBD);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion


        #region FreeCoPainTime

        private void txtFreeCoPainTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.FocusNextControlFreeCoPainTime();
                }
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.PageDown)
                {
                    this.dtFreeCoPainTime.ShowPopup();
                    this.dtFreeCoPainTime.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtFreeCoPainTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtFreeCoPainTime.Visible = false;
                    this.dtFreeCoPainTime.Update();
                    if (this.dtFreeCoPainTime.EditValue != null && this.dtFreeCoPainTime.DateTime != DateTime.MinValue)
                        this.txtFreeCoPainTime.Text = this.dtFreeCoPainTime.DateTime.ToString("dd/MM/yyyy");
                    else
                        this.txtFreeCoPainTime.Text = null;
                    this.FocusNextControlFreeCoPainTime();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtFreeCoPainTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtFreeCoPainTime.Visible = false;
                    this.dtFreeCoPainTime.Update();
                    if (this.dtFreeCoPainTime.EditValue != null && this.dtFreeCoPainTime.DateTime != DateTime.MinValue)
                        this.txtFreeCoPainTime.Text = this.dtFreeCoPainTime.DateTime.ToString("dd/MM/yyyy");
                    else
                        this.txtFreeCoPainTime.Text = null;
                    this.FocusNextControlFreeCoPainTime();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFreeCoPainTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Down)
                {
                    DateTime? dt = HeinUtils.ConvertDateStringToSystemDate(this.txtFreeCoPainTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtFreeCoPainTime.EditValue = dt;
                        this.dtFreeCoPainTime.Update();
                    }

                    this.dtFreeCoPainTime.Visible = true;
                    this.dtFreeCoPainTime.ShowPopup();
                    this.dtFreeCoPainTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFreeCoPainTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFreeCoPainTime_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtFreeCoPainTime.Text)) return;

                string date = "";
                if (this.txtFreeCoPainTime.Text.Contains("/"))
                    date = Utils.HeinUtils.DateToDateRaw(this.txtFreeCoPainTime.Text);

                if (!String.IsNullOrEmpty(date))
                {
                    this.txtFreeCoPainTime.Text = date;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFreeCoPainTime_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string inputDate = this.txtFreeCoPainTime.Text.Trim();
                if (!String.IsNullOrEmpty(inputDate))
                {
                    if (inputDate.Length == 8)
                    {
                        inputDate = inputDate.Substring(0, 2) + "/" + inputDate.Substring(2, 2) + "/" + inputDate.Substring(4, 4);
                    }

                    var dateFreeCoPainTime = Utils.HeinUtils.ConvertDateStringToSystemDate(inputDate);
                    if (dateFreeCoPainTime != null && dateFreeCoPainTime.Value != DateTime.MinValue)
                    {
                        this.txtFreeCoPainTime.Text = inputDate;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFreeCoPainTime_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                AutoValidate = AutoValidate.EnableAllowFocusChange;
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFreeCoPainTime_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (HisConfigCFG.IsNotAutoCheck5Y6M)
                    return;
                this.chkJoin5Year.Checked = this.IsShowMessage = (!String.IsNullOrEmpty(this.txtFreeCoPainTime.Text.Trim()));
                this.chkPaid6Month.Checked = false;
                if (!String.IsNullOrEmpty(this.txtFreeCoPainTime.Text.Trim()))
                {
                    this.IsShowMessage = this.chkJoin5Year.Checked;
                    DateTime? dt = HeinUtils.ConvertDateStringToSystemDate(this.txtFreeCoPainTime.Text);
                    this.chkPaid6Month.Checked = dt != null && dt.Value != DateTime.MinValue && Int64.Parse(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dt).ToString().Substring(0, 8)) < Int64.Parse(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now).ToString().Substring(0, 8));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region 5Year 6Month

        private void chkPaid6Month_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.ValidateCheckBox6M(chkPaid6Month.Checked);
                this.ChangeDefaultHeinRatio();
                this.Join5YearAndPaid6MonthCheckedChanged();
                this.ShowMessageNotAutoCheck5Y6M(this.chkPaid6Month);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPaid6Month_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtDu5Nam.Focus();
                    this.txtDu5Nam.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // -------------------- 5 year ------------------------//

        private void Join5YearAndPaid6MonthCheckedChanged()
        {
            try
            {
                if (chkJoin5Year.Checked && chkPaid6Month.Checked)
                {
                    lciFreeCoPainTime.AppearanceItemCaption.ForeColor = Color.Maroon;
                    this.ValidFreeCoPainTime(true);
                }
                else if (!chkPaid6Month.Checked)
                {
                    lciFreeCoPainTime.AppearanceItemCaption.ForeColor = Color.Black;
                    this.ValidFreeCoPainTime(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkJoin5Year_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.chkPaid6Month.Enabled)
                        this.chkPaid6Month.Focus();
                    else
                    {
                        if (this.txtFreeCoPainTime.Enabled)
                        {
                            this.txtFreeCoPainTime.Focus();
                            this.txtFreeCoPainTime.SelectAll();
                        }
                        else
                        {
                            this.FocusNextControlFreeCoPainTime();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkJoin5Year_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.ValidateCheckBox5Y(chkJoin5Year.Checked);
                this.ChangeDefaultHeinRatio();
                this.Join5YearAndPaid6MonthCheckedChanged();
                this.ShowMessageNotAutoCheck5Y6M(this.chkJoin5Year);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateCheckBox6M(bool IsVlid)
        {
            try
            {
                if (IsVlid)
                {
                    lciFreeCoPainTime.AppearanceItemCaption.ForeColor = Color.Maroon;
                    TemplateHeinBHYT1__CheckBox__ValidationRule vld = new TemplateHeinBHYT1__CheckBox__ValidationRule();
                    vld.chk = chkPaid6Month;
                    vld.txt = txtFreeCoPainTime;
                    vld.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    this.dxValidationProviderControl.SetValidationRule(this.txtFreeCoPainTime, vld);
                }
                else
                {
                    lciFreeCoPainTime.AppearanceItemCaption.ForeColor = Color.Black;
                    this.dxValidationProviderControl.SetValidationRule(this.txtFreeCoPainTime, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateCheckBox5Y(bool IsVlid)
        {
            try
            {
                if (IsVlid)
                {
                    lci5Y.AppearanceItemCaption.ForeColor = Color.Maroon;
                    TemplateHeinBHYT1__CheckBox__ValidationRule vld = new TemplateHeinBHYT1__CheckBox__ValidationRule();
                    vld.chk = chkJoin5Year;
                    vld.txt = txtDu5Nam;
                    vld.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    this.dxValidationProviderControl.SetValidationRule(this.txtDu5Nam, vld);
                }
                else
                {
                    lci5Y.AppearanceItemCaption.ForeColor = Color.Black;
                    this.dxValidationProviderControl.SetValidationRule(this.txtDu5Nam, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowMessageNotAutoCheck5Y6M(CheckEdit checkEdit)
        {
            try
            {
                IsShowMessage = IsShowMessage && (chkJoin5Year.Checked || chkPaid6Month.Checked);
                if (!HisConfigCFG.IsNotAutoCheck5Y6M || IsShowMessage || (!chkJoin5Year.Checked && !chkPaid6Month.Checked)) return;
                IsShowMessage = true;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.PhaiCoGiayChungNhanKhongCungChiTra, MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    checkEdit.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region HeinRightRoute

        private void cboHeinRightRoute_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    // this.FocusNextControlFreeCoPainTime();
                    //if (this.cboHeinRightRoute.EditValue.ToString() == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)
                    //    this.dlgIsEnableEmergency(true);
                    //else
                    //    this.dlgIsEnableEmergency(false);

                    //string _heinRightRoute = this.cboHeinRightRoute.EditValue.ToString().Trim();

                    //if (_heinRightRoute == HeinRightRouteTypeCode.PRESENT
                    //    && HisConfigCFG.KeyValueObligatoryTranferMediOrg == 1)
                    //{
                    //    this.dlgShowThongTinChuyenTuyen(true);
                    //    //this.dlgDisableBtnTTCT(true);
                    //}
                    //else if (_heinRightRoute == HeinRightRouteTypeCode.PRESENT
                    //&& HisConfigCFG.KeyValueObligatoryTranferMediOrg == 2)
                    //{
                    //    this.dlgShowThongTinChuyenTuyen(true);
                    //    //this.dlgDisableBtnTTCT(true);
                    //}
                    //else if (_heinRightRoute == HeinRightRouteTypeCode.APPOINTMENT
                    //    && HisConfigCFG.KeyValueObligatoryTranferMediOrg == 2)
                    //{
                    //    this.dlgShowThongTinChuyenTuyen(true);
                    //    //this.dlgDisableBtnTTCT(true);
                    //}
                    //else if (_heinRightRoute == HeinRightRouteTypeCode.PRESENT)
                    //    this.dlgDisableBtnTTCT(true);
                    //else
                    //    this.dlgDisableBtnTTCT(false);
                    //this.cboNoiSong.Focus();
                    //this.cboNoiSong.ShowPopup();
                    this.FocusNextControlFreeCoPainTime();//#18970
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHeinRightRoute_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("cboHeinRightRoute_EditValueChanged() => cboHeinRightRoute.EditValue", cboHeinRightRoute.EditValue != null ? cboHeinRightRoute.EditValue.ToString() : "null"));
                this.DisableBTNThongTinChuyenTuyen();
                this.TuDongCheckCapCuuTheoGiaTriTuyen();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Tiep don: UCHeinInfo/cboHeinRightRoute_EditValueChanged:\n" + ex);
            }
        }

        private void TuDongChonLoaiThongTuyen(HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO MediOrgADO)
        {
            try
            {
                string heinRightRoute = this.cboHeinRightRoute.EditValue != null ? this.cboHeinRightRoute.EditValue.ToString() : "";
                if (!IsDungTuyenCapCuuByTime && MediOrgADO != null && !String.IsNullOrEmpty(MediOrgADO.MEDI_ORG_CODE)
                    && !IsDefaultRightRouteType && HisConfigCFG.IsAllowedRouteTypeByDefault && !HisConfigCFG.IsAutoShowTransferFormInCaseOfAppointment)
                {
                    var result = MacDinhLoaiThongTuyen(MediOrgADO);

                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        this.cboHeinRightRoute.EditValue = result;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string MacDinhLoaiThongTuyen(Desktop.LocalStorage.BackendData.ADO.MediOrgADO MediOrgADO)
        {
            string result = null;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MacDinhLoaiThongTuyen() => MediOrgADO", MediOrgADO));
                var branch = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MacDinhLoaiThongTuyen() => branch", branch));
                if (MediOrgADO != null && !String.IsNullOrEmpty(MediOrgADO.MEDI_ORG_CODE) && branch != null)
                {
                    string branchHeinMediOrgCode = (branch.HEIN_MEDI_ORG_CODE ?? "").Trim();
                    if (MediOrgADO.MEDI_ORG_CODE.Trim() == branchHeinMediOrgCode
                        || ValidAcceptHeinMediOrgCode(MediOrgADO.MEDI_ORG_CODE, branch.ACCEPT_HEIN_MEDI_ORG_CODE)
                        || ValidSysMediOrgCode(MediOrgADO.MEDI_ORG_CODE, branch.SYS_MEDI_ORG_CODE))
                    {
                        result = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                    }
                    else
                    {
                        string ProvinceCode = MediOrgADO.MEDI_ORG_CODE.Substring(0, 2);
                        string branchProvinceCode = branchHeinMediOrgCode.Length > 2 ? branchHeinMediOrgCode.Substring(0, 2) : null;
                        if ((HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT
                                || HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE)
                            && ProvinceCode == branchProvinceCode
                            && (MediOrgADO.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT
                                || MediOrgADO.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE)
                            )
                        {
                            result = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER;
                        }
                        else if (ProvinceCode != branchProvinceCode
                            || MediOrgADO.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE
                            || MediOrgADO.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL
                            || HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE
                            || HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL)
                        {
                            result = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MacDinhLoaiThongTuyen() => result", result));
            return result;
        }

        private bool ValidAcceptHeinMediOrgCode(string mediOrgCode, string AcceptHeinMediOrgCode)
        {
            bool valid = false;
            try
            {
                if (String.IsNullOrWhiteSpace(mediOrgCode) || String.IsNullOrWhiteSpace(AcceptHeinMediOrgCode))
                    return false;
                string[] listCode = AcceptHeinMediOrgCode.Split(',', ';');
                foreach (var code in listCode)
                {
                    if (code != null && code.Trim() == mediOrgCode.Trim())
                        return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ValidSysMediOrgCode(string mediOrgCode, string SysMediOrgCode)
        {
            bool valid = false;
            try
            {
                if (String.IsNullOrWhiteSpace(mediOrgCode) || String.IsNullOrWhiteSpace(SysMediOrgCode))
                    return false;
                string[] listCode = SysMediOrgCode.Split(',', ';');
                foreach (var code in listCode)
                {
                    if (code != null && code.Trim() == mediOrgCode.Trim())
                        return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private void TuDongCheckCapCuuTheoGiaTriTuyen()
        {
            try
            {
                if (this.cboHeinRightRoute.EditValue != null && this.cboHeinRightRoute.EditValue.ToString() == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)
                {
                    if (this.dlgIsEnableEmergency != null)
                        this.dlgIsEnableEmergency(true);
                }
                else if (this.dlgIsEnableEmergency != null)
                    this.dlgIsEnableEmergency(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Tiep don: UCHeinInfo/TuDongCheckCapCuuTheoGiaTriTuyen:\n" + ex);
            }
        }

        private void DisableBTNThongTinChuyenTuyen()
        {
            try
            {
                //if (this.cboHeinRightRoute.EditValue != null
                //    && (this.cboHeinRightRoute.EditValue.ToString() == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT
                //    || this.cboHeinRightRoute.EditValue.ToString() == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT))
                //{
                //    this.dlgDisableBtnTTCT(true);
                //}
                //else
                //    this.dlgDisableBtnTTCT(false);
                if (this.cboHeinRightRoute.EditValue != null)
                {
                    string _heinRightRoute = this.cboHeinRightRoute.EditValue.ToString().Trim();

                    if (_heinRightRoute == HeinRightRouteTypeCode.PRESENT
                        && (HisConfigCFG.KeyValueObligatoryTranferMediOrg == 1 || HisConfigCFG.KeyValueObligatoryTranferMediOrg == 2 || HisConfigCFG.KeyValueObligatoryTranferMediOrg == 3))
                    {
                        this.dlgShowThongTinChuyenTuyen(true);
                        //this.dlgDisableBtnTTCT(true);
                    }
                    else if (_heinRightRoute == HeinRightRouteTypeCode.APPOINTMENT
                        && HisConfigCFG.KeyValueObligatoryTranferMediOrg == 2)
                    {
                        this.dlgShowThongTinChuyenTuyen(true);
                        //this.dlgDisableBtnTTCT(true);
                    }
                    else if (_heinRightRoute == HeinRightRouteTypeCode.APPOINTMENT
                        && HisConfigCFG.IsAutoShowTransferFormInCaseOfAppointment
                        && CheckMediOrgCode())
                    {
                        this.dlgShowThongTinChuyenTuyen(false);
                    }
                    else if (_heinRightRoute == HeinRightRouteTypeCode.PRESENT)
                        this.dlgDisableBtnTTCT(true);
                    else
                        this.dlgDisableBtnTTCT(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Tiep don: UCHeinInfo/DisableBTNThongTinChuyenTuyen:\n" + ex);
            }
        }

        //mã nơi KCB ban đầu của thẻ BHYT không trùng với mã của chi nhánh (của người dùng đang làm việc) 
        private bool CheckMediOrgCode()
        {
            bool result = false;
            try
            {
                var branch = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch;
                bool valid = true;
                valid = valid && branch != null;
                valid = valid && !String.IsNullOrWhiteSpace(txtMaDKKCBBD.Text);
                valid = valid && !String.IsNullOrWhiteSpace(branch.HEIN_MEDI_ORG_CODE);
                valid = valid && txtMaDKKCBBD.Text.Trim().ToLower() == branch.HEIN_MEDI_ORG_CODE.ToLower();
                result = !valid;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        #region NoiSong

        private void cboNoiSong_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboNoiSong.EditValue = null;
                    this.cboNoiSong.Properties.Buttons[1].Visible = false;
                    HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO mediorg = MediOrgDataWorker.MediOrgADOs.SingleOrDefault(o => o.MEDI_ORG_CODE == (this.cboDKKCBBD.EditValue ?? "").ToString());
                    if (mediorg != null && HisConfigCFG.IsNotRequiredRightTypeInCaseOfHavingAreaCode)
                        this.ProcessCaseWrongRoute(mediorg.MEDI_ORG_CODE);

                    this.ChangeDefaultHeinRatio();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiSong_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    string liveArea = (this.cboNoiSong.EditValue ?? "").ToString();
                    this.chkJoin5Year.Focus();

                    if (HisConfigCFG.IsNotRequiredRightTypeInCaseOfHavingAreaCode)
                    {
                        HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO mediorg = MediOrgDataWorker.MediOrgADOs.SingleOrDefault(o => o.MEDI_ORG_CODE == (this.cboDKKCBBD.EditValue ?? "").ToString());
                        if (mediorg != null)
                            this.ProcessCaseWrongRoute(mediorg.MEDI_ORG_CODE, liveArea);

                        if (!HisConfigCFG.IsAllowedRouteTypeByDefault)
                        {
                            this.cboHeinRightRoute.ShowPopup();
                        }
                        if (HisConfigCFG.IsAllowedRouteTypeByDefault && (!String.IsNullOrEmpty(liveArea) && (liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K1 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K2 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K3)))
                        {
                            cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                            this.dxValidationProviderControl.RemoveControlError(cboHeinRightRoute);
                        }
                    }

                    if (this.cboNoiSong.EditValue != null)
                    {
                        this.ChangeDefaultHeinRatio();
                        this.cboNoiSong.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiSong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string liveArea = (this.cboNoiSong.EditValue ?? "").ToString();
                    HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO mediorg = HisConfigCFG.IsNotRequiredRightTypeInCaseOfHavingAreaCode ? MediOrgDataWorker.MediOrgADOs.SingleOrDefault(o => o.MEDI_ORG_CODE == (this.cboDKKCBBD.EditValue ?? "").ToString()) : null;
                    if (mediorg != null)
                        this.ProcessCaseWrongRoute(mediorg.MEDI_ORG_CODE, liveArea);
                    if (this.cboNoiSong.EditValue != null)
                    {
                        if ((HisConfigCFG.IsNotRequiredRightTypeInCaseOfHavingAreaCode && !String.IsNullOrEmpty(liveArea) && (liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K1 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K2 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K3)))
                        {
                            cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                            this.dxValidationProviderControl.RemoveControlError(cboHeinRightRoute);
                        }

                        this.cboNoiSong.Properties.Buttons[1].Visible = true;
                        this.ChangeDefaultHeinRatio();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region The tam - Khong Check Hạn The

        private void chkHasDobCertificate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.chkHasCardTemp.Checked)
                    {
                        if (this.dlgFocusNextUserControl != null) this.dlgFocusNextUserControl();
                    }
                    else
                    {
                        this.txtSoThe.Focus();
                        this.txtSoThe.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHasCardTemp_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool isEnable = (!this.chkHasCardTemp.Checked);
                if (this.isTempQN && isEnable == false)
                {
                    this.lciHeinCardFromTime.Enabled = isEnable;
                    this.lciHeinCardNumber.Enabled = isEnable;
                    this.lciHeinCardToTime.Enabled = isEnable;

                    this.lcicboKCBBĐ.Enabled = !isEnable;
                    this.lciFreeCoPainTime.Enabled = !isEnable;
                    this.lciHeinCardAddress.Enabled = !isEnable;
                    this.lciHeinRightRoute.Enabled = !isEnable;
                    this.lciJoin5Year.Enabled = !isEnable;
                    this.lciKCBBĐ.Enabled = !isEnable;
                    this.lciKV.Enabled = !isEnable;
                    this.lciPaid6Month.Enabled = !isEnable;
                    this.lci5Y.Enabled = !isEnable;
                    if (this.isPatientOld == false)
                    {
                        this.txtSoThe.EditValue = "";
                        this.cboSoThe.EditValue = null;
                        this.cboSoThe.Properties.DataSource = null;
                        this.txtHeinCardFromTime.Text = "";
                        this.dtHeinCardFromTime.Text = "";
                        this.txtHeinCardToTime.Text = "";
                        this.dtHeinCardToTime.Text = "";
                        this.ResetRequiredFieldWhenCardTempIsQN();
                    }
                }
                else if (this.isTempQN && isEnable == true)
                {
                    this.lciHeinCardFromTime.Enabled = !isEnable;
                    this.lciHeinCardNumber.Enabled = !isEnable;
                    this.lciHeinCardToTime.Enabled = !isEnable;

                    this.lcicboKCBBĐ.Enabled = !isEnable;
                    this.lciFreeCoPainTime.Enabled = !isEnable;
                    this.lciHeinCardAddress.Enabled = !isEnable;
                    this.lciHeinRightRoute.Enabled = !isEnable;
                    this.lciJoin5Year.Enabled = !isEnable;
                    this.lciKCBBĐ.Enabled = !isEnable;
                    this.lciKV.Enabled = !isEnable;
                    this.lciPaid6Month.Enabled = !isEnable;
                    this.lci5Y.Enabled = !isEnable;
                    if (this.isPatientOld == false)
                    {
                        this.txtSoThe.EditValue = "";
                        this.cboSoThe.EditValue = null;
                        this.cboSoThe.Properties.DataSource = null;
                        this.txtHeinCardFromTime.Text = "";
                        this.dtHeinCardFromTime.Text = "";
                        this.txtHeinCardToTime.Text = "";
                        this.dtHeinCardToTime.Text = "";
                        this.txtDu5Nam.Text = "";
                        this.dtDu5Nam.Text = "";
                        this.txtAddress.Text = "";
                        this.txtFreeCoPainTime.Text = "";
                        this.dtFreeCoPainTime.Text = "";
                        this.cboDKKCBBD.EditValue = null;
                        this.cboHeinRightRoute.EditValue = null;
                        this.cboNoiSong.EditValue = null;
                        this.txtMaDKKCBBD.Text = "";
                        this.txtMucHuong.Text = "";
                        this.chkJoin5Year.Checked = false;
                        this.chkPaid6Month.Checked = false;
                        this.ResetValidate();
                    }
                }
                else if (this.isTempQN == false && isEnable == false) // không phải quân nhân và được check vào thẻ tạm --> trẻ em --> kiểm tra bệnh nhân cũ
                {
                    //this.lcicboKCBBĐ.Enabled = isEnable;
                    //this.lciFreeCoPainTime.Enabled = isEnable;
                    //this.lciHeinCardAddress.Enabled = isEnable;
                    //this.lciHeinCardFromTime.Enabled = isEnable;
                    //this.lciHeinCardNumber.Enabled = isEnable;
                    //this.lciHeinCardToTime.Enabled = isEnable;
                    //this.lciHeinRightRoute.Enabled = isEnable;
                    //this.lciJoin5Year.Enabled = isEnable;
                    //this.lciKCBBĐ.Enabled = isEnable;
                    if (this.lciKhongKTHSD.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    {
                        this.lciKhongKTHSD.Enabled = isEnable;
                    }

                    this.lciKV.Enabled = isEnable;
                    this.lciPaid6Month.Enabled = isEnable;
                    this.lci5Y.Enabled = isEnable;
                    if (this.isPatientOld == false)
                    {
                        this.txtSoThe.EditValue = "";
                        this.cboSoThe.EditValue = null;
                        this.cboSoThe.Properties.DataSource = null;
                        this.txtHeinCardFromTime.Text = "";
                        this.dtHeinCardFromTime.Text = "";
                        this.txtHeinCardToTime.Text = "";
                        this.dtHeinCardToTime.Text = "";
                        if (IsDungTuyenCapCuuByTime && (this.cboHeinRightRoute.EditValue == null || this.cboHeinRightRoute.EditValue == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT) && HisConfigCFG.IsAllowedRouteTypeByDefault)
                            this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                        else if (IsDungTuyenCapCuuByTime == false && (this.cboHeinRightRoute.EditValue == null || this.cboHeinRightRoute.EditValue == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT))
                            this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                        this.ResetRequiredField();
                    }
                    this.lciHeinCardNumber.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.lciHeinCardFromTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.lciHeinCardToTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.lciHeinCardAddress.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.lciKCBBĐ.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.lciFreeCoPainTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;


                    this.ResetValidate();
                }
                else if (isEnable == true)
                {
                    this.lcicboKCBBĐ.Enabled = isEnable;
                    this.lciFreeCoPainTime.Enabled = isEnable;
                    this.lciHeinCardAddress.Enabled = isEnable;
                    this.lciHeinCardFromTime.Enabled = isEnable;
                    this.lciHeinCardNumber.Enabled = isEnable;
                    this.lciHeinCardToTime.Enabled = isEnable;
                    this.lciHeinRightRoute.Enabled = isEnable;
                    this.lciJoin5Year.Enabled = isEnable;
                    this.lciKCBBĐ.Enabled = isEnable;
                    if (this.lciKhongKTHSD.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    {
                        this.lciKhongKTHSD.Enabled = isEnable;
                    }

                    this.lciKV.Enabled = isEnable;
                    this.lciPaid6Month.Enabled = isEnable;
                    this.lci5Y.Enabled = isEnable;
                    this.InitValidateRule(HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT);
                    this.lciHeinCardNumber.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciHeinCardFromTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciHeinCardToTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciHeinCardAddress.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciKCBBĐ.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkKhongKTHSD_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkKhongKTHSD.Checked == true)
                    this.ClearValidHeinCardToTime();
                else
                    this.ValidHeinCardToTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Dia chi the

        private void txtAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.CheckExamHistoryFromBHXHApi(FocusOutOfUc);//xuandv
                    this.txtMaDKKCBBD.Focus();
                    this.txtMaDKKCBBD.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region private Method
        private void txtHeinCardFromTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DateTime? dt = HIS.UC.UCHeniInfo.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardFromTime.EditValue = dt;
                        this.dtHeinCardFromTime.Update();
                    }
                    this.CheckExamHistoryFromBHXHApi(FocusOutOfUc);//xuandv

                    this.txtHeinCardToTime.Focus();
                    this.txtHeinCardToTime.SelectAll();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = HIS.UC.UCHeniInfo.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardFromTime.EditValue = dt;
                        this.dtHeinCardFromTime.Update();
                    }

                    this.dtHeinCardFromTime.Visible = true;
                    this.dtHeinCardFromTime.ShowPopup();
                    this.dtHeinCardFromTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusNextControlFreeCoPainTime()
        {
            try
            {
                if (this.dlgFocusNextUserControl != null)
                    this.dlgFocusNextUserControl();
                else
                    SendKeys.Send("{TAB}");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableUCHein()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void chkHasWorkingLetter_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkHasWorkingLetter.Checked)
                {
                    EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, DataStore.HeinRightRouteTypes, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                    this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                }
                else
                {
                    if (this.chkHasAbsentLetter.Checked)
                        return;
                    EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, dataSourceCboHeinRightRouteTemp, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                    this.cboHeinRightRoute.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHasAbsentLetter_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkHasAbsentLetter.Checked)
                {
                    EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, DataStore.HeinRightRouteTypes, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                    this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                }
                else
                {
                    if (this.chkHasWorkingLetter.Checked)
                        return;
                    EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, dataSourceCboHeinRightRouteTemp, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                    this.cboHeinRightRoute.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsTt46_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkIsTt46.Checked)
                {
                    EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, DataStore.HeinRightRouteTypes, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                    this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                    this.txtNote.Enabled = true;
                }
                else
                {
                    EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, dataSourceCboHeinRightRouteTemp, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                    this.txtNote.Enabled = false;
                    this.txtNote.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHasWorkingLetter_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (lciHasWorkingLetter.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    lciIsBhytHolded.TextVisible = false;
                }
                else if (lciHasWorkingLetter.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {
                    lciIsBhytHolded.TextVisible = true;
                    lciIsBhytHolded.TextSize = lciHasAbsentLetter.TextSize;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSoThe_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkHasCardTemp.Checked)
                {
                    if (!string.IsNullOrEmpty(txtSoThe.Text))
                    {
                        this.lciHeinCardNumber.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                        this.lciHeinCardFromTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                        this.lciHeinCardAddress.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                        this.lciKCBBĐ.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;

                        this.InitValidateRule(HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.PatientTypeID_Default);
                        this.dxValidationProviderControl.SetValidationRule(this.txtHeinCardToTime, null);
                        this.dtHeinCardFromTime.DateTime = DateTime.Now;
                        this.txtHeinCardFromTime.Text = this.dtHeinCardFromTime.DateTime.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        this.lciHeinCardNumber.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                        this.lciHeinCardFromTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                        this.lciHeinCardToTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                        this.lciHeinCardAddress.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                        this.lciKCBBĐ.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                        this.lciFreeCoPainTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;


                        this.ResetValidate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCheckInfoBHYT_Click(object sender, EventArgs e)
        {
            try
            {
                bool Valid = true;
                if (!currentPatientRaw.ValidateRequiredFieldCheckInfoBhyt())
                    Valid = false;

                if (!dxValidationProviderControl.Validate(txtSoThe))
                    Valid = false;

                if (treatmentId <= 0 && !Valid)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CheckInfoBHYT").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.CheckInfoBHYT");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    if (treatmentId > 0)
                    {
                        listArgs.Add(treatmentId);
                    }
                    else
                    {
                        HIS.Desktop.ADO.CheckInfoBhytADO ado = new CheckInfoBhytADO();
                        ado.TDL_PATIENT_NAME = currentPatientRaw.GetValueForCheckInfoBhyt().PATIENT_NAME;
                        ado.TDL_DOB = currentPatientRaw.GetValueForCheckInfoBhyt().DOB;
                        ado.TDL_GENDER_NAME = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().First(o => o.ID == this.currentPatientRaw.GetValueForCheckInfoBhyt().GENDER_ID).GENDER_NAME;
                        ado.TDL_HEIN_CARD_NUMBER = HeinUtils.TrimHeinCardNumber(this.txtSoThe.Text);
                        listArgs.Add(ado);
                    }
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.module.RoomId, this.module.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.module.RoomId, this.module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSoThe_Leave(object sender, EventArgs e)
        {
            try
            {
                Send3WCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSs_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkSs.Checked)
                {
                    EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, DataStore.HeinRightRouteTypes, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                    this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                }
                else
                {
                    EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, dataSourceCboHeinRightRouteTemp, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                    this.cboHeinRightRoute.EditValue = null;
                }
                this.dlgCheckSS(chkSs.Checked);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
