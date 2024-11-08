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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using His.UC.UCHein.Base;
using His.UC.UCHein.Config;
using His.UC.UCHein.Data;
using His.UC.UCHein.Utils;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace His.UC.UCHein.Design.TemplateHeinBHYT1
{
    public partial class Template__HeinBHYT1 : UserControl
    {
        #region Reclare
        SetFocusMoveOut dlgsetFocusMoveOut;
        SetShortcutKeyDown dlgsetShortcutKeyDown;
        ProcessFillDataCareerUnder6AgeByHeinCardNumber dlgProcessFillDataCareerUnder6AgeByHeinCardNumber;
        DelegateAutoCheckCC dlgautoCheckCC;
        CheckExamHistoryByHeinCardNumber dlgcheckExamHistory;
        FillDataPatientSDOToRegisterForm dlgfillDataPatientSDOToRegisterForm;
        DelegateSetRelativeAddress _DelegateSetRelativeAddress;
        Action actChangePatientDob;
        int positionHandleControl = -1;
        long PatientTypeId;
        string TreatmentTypeCode { get; set; }
        CultureInfo CultureInfo { get; set; }
        DataInitHeinBhyt entity;
        ResultDataADO ResultDataADO { get; set; }
        string SysMediOrgCode { get; set; }
        string HeinLevelCodeCurrent { get; set; }
        string MediOrgCodeCurrent { get; set; }
        List<string> MediOrgCodesAccepts { get; set; }
        long TreatmentTypeIdExam { get; set; }
        internal static long PatientTypeIdBHYT { get; set; }
        long isVisibleControl { get; set; }
        string isShowCheckKhongKTHSD { get; set; }
        string autoCheckIcd { get; set; }
        bool IsDefaultRightRouteType { get; set; }
        bool IsNotRequiredRightTypeInCaseOfHavingAreaCode { get; set; }
        bool IsEdit { get; set; }
        bool IsTempQN { get; set; }
        bool IsObligatoryTranferMediOrg { get; set; }
        string ObligatoryTranferMediOrg { get; set; }
        HisPatientSDO currentPatientSdo;
        bool isDefaultInit = false;
        long ExceedDayAllow;

        public long PatientId { get; private set; }

        bool isCallByRegistor = false;
        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER patientTypeAlterOld { get; set; }

        string _TextIcdName = "";

        bool IsDungTuyenCapCuuByTime = false;
        bool IsShowMessage = false;
        bool IsAutoCheck = false;
        private long logTime;

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
        #endregion

        public Template__HeinBHYT1()
            : this(null)
        {
        }

        public Template__HeinBHYT1(DataInitHeinBhyt data)
        {
            try
            {
                InitializeComponent();

                if (data != null)
                {
                    this.entity = data;
                    this.isDefaultInit = true;
                    DataStore.MediOrgs = (from m in data.MediOrgs select new MediOrgADO(m)).ToList();
                    DataStore.IcdADOs = (from m in data.Icds select new IcdADO(m)).ToList();
                    DataStore.LiveAreas = data.LiveAreas;
                    DataStore.Icds = data.Icds.Where(p => p.IS_ACTIVE == 1).ToList();
                    DataStore.TranPatiForms = data.TranPatiForms;
                    DataStore.TranPatiReasons = data.TranPatiReasons;
                    DataStore.HeinRightRouteTypes = data.HeinRightRouteTypes;
                    DataStore.TreatmentTypes = data.TreatmentTypes;
                    DataStore.Genders = data.Genders;
                    DataStore.PatientTypes = data.PatientTypes;
                    this.MediOrgCodeCurrent = data.MEDI_ORG_CODE__CURRENT;
                    this.MediOrgCodesAccepts = data.MEDI_ORG_CODES__ACCEPTs;
                    this.HeinLevelCodeCurrent = data.HEIN_LEVEL_CODE__CURRENT;
                    this.TreatmentTypeIdExam = data.TREATMENT_TYPE_ID__EXAM;
                    this.SysMediOrgCode = data.SYS_MEDI_ORG_CODE;
                    PatientTypeIdBHYT = data.PATIENT_TYPE_ID__BHYT;
                    this.PatientTypeId = data.PatientTypeId;
                    this.lblEditIcd.Enabled = false;
                    this.isVisibleControl = data.isVisibleControl;
                    this.isShowCheckKhongKTHSD = data.IsShowCheckKhongKTHSD;
                    this.IsNotRequiredRightTypeInCaseOfHavingAreaCode = data.IsNotRequiredRightTypeInCaseOfHavingAreaCode;
                    this.autoCheckIcd = data.AutoCheckIcd;
                    this.IsDefaultRightRouteType = data.IsDefaultRightRouteType;
                    this.IsEdit = data.IsEdit;
                    this.IsTempQN = data.IsTempQN;
                    this.IsObligatoryTranferMediOrg = data.IsObligatoryTranferMediOrg;
                    this.ObligatoryTranferMediOrg = data.ObligatoryTranferMediOrg;
                    this.IsDungTuyenCapCuuByTime = data.IsDungTuyenCapCuuByTime;

                    this.dlgProcessFillDataCareerUnder6AgeByHeinCardNumber = data.ProcessFillDataCareerUnder6AgeByHeinCardNumber;
                    this.dlgfillDataPatientSDOToRegisterForm = data.FillDataPatientSDOToRegisterForm;
                    this.dlgautoCheckCC = data.AutoCheckCC;
                    this.dlgcheckExamHistory = data.CheckExamHistory;
                    this.dlgsetFocusMoveOut = data.SetFocusMoveOut;
                    this.dlgsetShortcutKeyDown = data.SetShortcutKeyDown;
                    this._DelegateSetRelativeAddress = data.SetRelativeAddress;
                    this.actChangePatientDob = data.ActChangePatientDob;
                    this.ExceedDayAllow = data.ExceedDayAllow;
                    this.PatientId = data.PatientId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Template__HeinBHYT1_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Template__HeinBHYT1_Load()");
                this.SetCaptionByLanguageKeyNew();
                Config.HisConfigCFG.LoadConfig();
                if (this.isDefaultInit)
                    this.InitData(this.entity);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitData(DataInitHeinBhyt data)
        {
            try
            {
                this.InitDataToControl();
                this.VisibleControl(this.isVisibleControl);
                this.DisableControlWhenPatientTypeQN(this.IsTempQN, false);
                this.CheckTempQN();
                this.CheckHSDAndTECard();
                this.ValidControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AutoSelectEmergency(DataInitHeinBhyt data)
        {
            try
            {
                if (data.IsAutoSelectEmergency)
                {
                    cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                    txtHeinRightRouteCode.Text = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckTempQN()
        {
            try
            {
                if (!this.IsTempQN)
                    this.lblHeincardNumber.Size = new Size(this.lciTempQN.Size.Width + this.lblHeincardNumber.Size.Width - this.layoutControlItem1.Size.Width, this.lblHeincardNumber.Size.Height);
                this.lciTempQN.Visibility = this.IsTempQN ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetLanguageKey()
        {
            try
            {
                this.lciFreeCoPainTime.Text = Inventec.Common.Resource.Get.Value("lciFreeCoPainTime.Text", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.txtFreeCoPainTime.ToolTip = Inventec.Common.Resource.Get.Value("txtFreeCoPainTime.Tooltip", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lblCaptionHasDobCertificate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LBL_CAPTION_HAS_DOB_CERTIFICATE", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lblHeincardNumber.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LBL_HEINCARD_NUMBER", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lblHeincardFromDate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LBL_HEINCARD_FROM_DATE", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lblHeincardToDate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LBL_HEINCARD_TO_DATE", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lblCaptionAddress.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LBL_CAPTION_ADDRESS", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lblHeincardMediOrg.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LBL_HEINCARD_MEDIORG", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lciInCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT__LCI_IN_CODE", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lciHNCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT__LCI_HNCODE", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lblRightRouteType.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LBL_RIGHT_ROUTE_TYPE", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lblMediRecordMediOrgForm.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LBL_MEDI_RECORD_MEDI_ORG_FORM", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lciMediRecordRouteTransfer.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LAYOUT_CONTROL_ITEM12", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lciMediRecordNoRouteTransfer.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LAYOUT_CONTROL_ITEM13", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lciTransPatiFormCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LBL_MEDI_RECORD_TRANS_PATI_FORM", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lciTransPatiReasonCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LBL_MEDI_RECORD_TRANS_PATI_REASON", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lblMediRecordLiveArea.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LBL_MEDI_RECORD_LIVE_AREA", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lcichkJoin5Year.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LCI_JOIN_5_YEAR", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lcichkPaid6Month.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LCI_PAID_6_MONTH", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lblMediRecordBenefitSymbol.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_BHYT_LBL_MEDI_RECORD_BENEFIT_SYMBOL", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lciIcdMain.Text = Inventec.Common.Resource.Get.Value("lciIcdMain.Text", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lcirdoWrongRoute.Text = Inventec.Common.Resource.Get.Value("lcirdoWrongRoute.Text", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lcirdoRightRoute.Text = Inventec.Common.Resource.Get.Value("lcirdoRightRoute.Text", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                this.lblEditIcd.Text = Inventec.Common.Resource.Get.Value("lblEditIcd.Text", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện Template__HeinBHYT1
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("His.UC.UCHein.Resources.Lang", typeof(Template__HeinBHYT1).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.dtHeinCardToTime.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.dtHeinCardToTime.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.dtHeinCardFromTime.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.dtHeinCardFromTime.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnCheckInfoBHYT.ToolTip = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.btnCheckInfoBHYT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.rdoWrongRoute.Properties.Caption = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.rdoWrongRoute.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboNoiSong.Properties.NullText = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.cboNoiSong.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkTempQN.Properties.Caption = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.chkTempQN.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtFreeCoPainTime.ToolTip = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.txtFreeCoPainTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.checkKhongKTHSD.Properties.Caption = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.checkKhongKTHSD.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.rdoRightRoute.Properties.Caption = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.rdoRightRoute.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkPaid6Month.Properties.Caption = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.chkPaid6Month.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkPaid6Month.ToolTip = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboChanDoanTD.Properties.NullText = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.cboChanDoanTD.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkHasDialogText.Properties.Caption = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.chkHasDialogText.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboLyDoChuyen.Properties.NullText = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.cboLyDoChuyen.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboHeinRightRoute.Properties.NullText = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.cboHeinRightRoute.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboDKKCBBD.Properties.NullText = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.cboDKKCBBD.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboNoiChuyenDen.Properties.NullText = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.cboNoiChuyenDen.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboHinhThucChuyen.Properties.NullText = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.cboHinhThucChuyen.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkJoin5Year.Properties.Caption = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.chkJoin5Year.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkJoin5Year.ToolTip = Inventec.Common.Resource.Get.Value("toolTipItem2.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkMediRecordNoRouteTransfer.Properties.Caption = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.chkMediRecordNoRouteTransfer.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkMediRecordNoRouteTransfer.ToolTip = Inventec.Common.Resource.Get.Value("toolTipItem3.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkMediRecordRouteTransfer.Properties.Caption = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.chkMediRecordRouteTransfer.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkMediRecordRouteTransfer.ToolTip = Inventec.Common.Resource.Get.Value("toolTipItem4.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboSoThe.ToolTip = Inventec.Common.Resource.Get.Value("toolTipItem5.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboSoThe.Properties.NullText = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.cboSoThe.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkHasDobCertificate.Properties.Caption = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.chkHasDobCertificate.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkHasDobCertificate.ToolTip = Inventec.Common.Resource.Get.Value("toolTipItem6.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblCaptionHasDobCertificate.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lblCaptionHasDobCertificate.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblHeincardNumber.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lblHeincardNumber.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblHeincardToDate.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lblHeincardToDate.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblHeincardFromDate.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lblHeincardFromDate.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblCaptionAddress.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lblCaptionAddress.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblMediRecordMediOrgForm.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lblMediRecordMediOrgForm.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblHeincardMediOrg.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lblHeincardMediOrg.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblRightRouteType.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lblRightRouteType.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTransPatiReasonCode.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciTransPatiReasonCode.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblMediRecordBenefitSymbol.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lblMediRecordBenefitSymbol.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciIcdMain.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciIcdMain.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblEditIcd.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lblEditIcd.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciMediRecordRouteTransfer.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciMediRecordRouteTransfer.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciMediRecordNoRouteTransfer.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciMediRecordNoRouteTransfer.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTransPatiFormCode.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciTransPatiFormCode.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lcirdoWrongRoute.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lcirdoWrongRoute.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lcirdoRightRoute.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lcirdoRightRoute.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciKhongKTHSD.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciKhongKTHSD.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciFreeCoPainTime.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciFreeCoPainTime.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciInCode.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciInCode.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciHNCode.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciHNCode.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lcichkJoin5Year.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lcichkJoin5Year.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lcichkPaid6Month.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lcichkPaid6Month.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTempQN.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciTempQN.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblMediRecordLiveArea.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lblMediRecordLiveArea.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciDu5Nam.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciDu5Nam.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciDu5Nam.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciDu5Nam.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciFordtTransferInTimeFrom.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciFordtTransferInTimeFrom.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciFordtTransferInTimeFrom.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciFordtTransferInTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciFordtTransferInTimeTo.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciFordtTransferInTimeTo.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciFordtTransferInTimeTo.Text = Inventec.Common.Resource.Get.Value("Template__HeinBHYT1.lciFordtTransferInTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValidate(string heinMediOrgCode)
        {
            try
            {
                if ((MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL == this.HeinLevelCodeCurrent
                        || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE == this.HeinLevelCodeCurrent)
                    && (!String.IsNullOrEmpty(heinMediOrgCode) && MediOrgCodeCurrent != heinMediOrgCode && (String.IsNullOrWhiteSpace(this.SysMediOrgCode) || !this.SysMediOrgCode.Contains(heinMediOrgCode))))
                {
                    if (this.IsTempQN && chkTempQN.Checked)
                    {
                        this.lblRightRouteType.AppearanceItemCaption.ForeColor = new Color();
                    }
                    else
                    {
                        this.lblRightRouteType.AppearanceItemCaption.ForeColor = Color.Maroon;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckHSDAndTECard()
        {
            try
            {
                this.lciKhongKTHSD.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.checkKhongKTHSD.Checked = false;
                if (this.isShowCheckKhongKTHSD == "1" && this.dtHeinCardToTime.DateTime != DateTime.MinValue)
                {
                    string heinCardNumber = txtSoThe.Text;
                    heinCardNumber = heinCardNumber.Replace(" ", "").ToUpper().Trim();
                    heinCardNumber = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber);
                    if (!String.IsNullOrEmpty(heinCardNumber) && heinCardNumber.StartsWith("TE") && this.dtHeinCardToTime.DateTime.Date < DateTime.Now.Date)
                    {
                        this.lciKhongKTHSD.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibleControl(long isVisibleControl)
        {
            try
            {
                if (isVisibleControl == 1)
                {
                    this.lciMediRecordRouteTransfer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;//Mã Chuyen dung tuyen
                    this.lciMediRecordNoRouteTransfer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;//Chuyen vuot tuyen
                    this.lciTransPatiFormCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;//Mã Hinh thuc chuyen
                    this.lciTransPatiFormCbo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;//Hinh thuc chuyen
                    this.lciTransPatiReasonCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;//Mã Ly do chuyen
                    this.lciTransPatiReasoncbo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;//Ly do chuyen

                    this.Height = 73;
                }
                else
                {
                    this.Height = 97;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DisableControlWhenPatientTypeQN(bool isTempQN, bool checkedCardTemp) // tientv #7479
        {
            try
            {
                this.lblHeincardNumber.Enabled = !isTempQN;
                this.lblHeincardFromDate.Enabled = !isTempQN;
                this.lblHeincardToDate.Enabled = !isTempQN;
                if (isTempQN == true)
                {
                    dxValidationProvider1.SetValidationRule(txtSoThe, null);
                    dxValidationProvider1.SetValidationRule(txtHeinCardToTime, null);
                    dxValidationProvider1.SetValidationRule(txtHeinCardFromTime, null);
                }
                if (checkedCardTemp == true)
                {
                    this.lblCaptionAddress.Enabled = checkedCardTemp;
                    this.lblHeincardMediOrg.Enabled = checkedCardTemp;
                    this.lciDKKCBBDName.Enabled = checkedCardTemp;
                    this.lciRightRouteTypeName.Enabled = checkedCardTemp;
                    this.rdoRightRoute.Enabled = checkedCardTemp;
                    this.rdoWrongRoute.Enabled = checkedCardTemp;
                    this.lblRightRouteType.Enabled = checkedCardTemp;
                    this.lcichkJoin5Year.Enabled = checkedCardTemp;
                    this.lcichkPaid6Month.Enabled = checkedCardTemp;
                    this.lciFreeCoPainTime.Enabled = checkedCardTemp;
                    this.lblMediRecordBenefitSymbol.Enabled = checkedCardTemp;
                    this.lblMediRecordLiveArea.Enabled = checkedCardTemp;
                    this.lblMediRecordMediOrgForm.Enabled = checkedCardTemp;
                    this.lblRightRouteType.Enabled = checkedCardTemp;
                    this.lciNoiChuyenDenName.Enabled = checkedCardTemp;
                    this.lciIcdMain.Enabled = checkedCardTemp;
                    this.panelICD.Enabled = checkedCardTemp;
                    this.lblEditIcd.Enabled = checkedCardTemp;
                    this.lciInCode.Enabled = checkedCardTemp;
                    this.lciHNCode.Enabled = checkedCardTemp;
                    this.lciMediRecordRouteTransfer.Enabled = checkedCardTemp;
                    this.lciMediRecordNoRouteTransfer.Enabled = checkedCardTemp;
                    this.lciTransPatiFormCode.Enabled = checkedCardTemp;
                    this.lciTransPatiFormCbo.Enabled = checkedCardTemp;
                    this.lciTransPatiReasonCode.Enabled = checkedCardTemp;
                    this.lciTransPatiReasoncbo.Enabled = checkedCardTemp;
                }
                else
                {
                    this.lblCaptionAddress.Enabled = !isTempQN;
                    this.lblHeincardMediOrg.Enabled = !isTempQN;
                    this.lciDKKCBBDName.Enabled = !isTempQN;
                    this.lciRightRouteTypeName.Enabled = !isTempQN;
                    this.rdoRightRoute.Enabled = !isTempQN;
                    this.rdoWrongRoute.Enabled = !isTempQN;
                    this.lblRightRouteType.Enabled = !isTempQN;
                    this.lcichkJoin5Year.Enabled = !isTempQN;
                    this.lcichkPaid6Month.Enabled = !isTempQN;
                    this.lciFreeCoPainTime.Enabled = !isTempQN;
                    this.lblMediRecordBenefitSymbol.Enabled = !isTempQN;
                    this.lblMediRecordLiveArea.Enabled = !isTempQN;
                    this.lblMediRecordMediOrgForm.Enabled = !isTempQN;
                    this.lblRightRouteType.Enabled = !isTempQN;
                    this.lciNoiChuyenDenName.Enabled = !isTempQN;
                    this.lciIcdMain.Enabled = !isTempQN;
                    this.panelICD.Enabled = !isTempQN;
                    this.lblEditIcd.Enabled = !isTempQN;
                    this.lciInCode.Enabled = !isTempQN;
                    this.lciHNCode.Enabled = !isTempQN;
                    this.lciMediRecordRouteTransfer.Enabled = !isTempQN;
                    this.lciMediRecordNoRouteTransfer.Enabled = !isTempQN;
                    this.lciTransPatiFormCode.Enabled = !isTempQN;
                    this.lciTransPatiFormCbo.Enabled = !isTempQN;
                    this.lciTransPatiReasonCode.Enabled = !isTempQN;
                    this.lciTransPatiReasoncbo.Enabled = !isTempQN;
                }
                if (IsTempQN == true && checkedCardTemp == false)
                {
                    lblHeincardNumber.AppearanceItemCaption.ForeColor = new Color();
                    lblHeincardToDate.AppearanceItemCaption.ForeColor = new Color();
                    lblHeincardFromDate.AppearanceItemCaption.ForeColor = new Color();
                    lblRightRouteType.AppearanceItemCaption.ForeColor = new Color();

                    IList<Control> invalidControls = this.dxValidationProvider1.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        this.dxValidationProvider1.RemoveControlError(invalidControls[i]);
                    }
                    this.dxErrorProvider1.ClearErrors();

                    dxValidationProvider1.SetValidationRule(txtSoThe, null);
                    dxValidationProvider1.SetValidationRule(txtFreeCoPainTime, null);
                    dxValidationProvider1.SetValidationRule(txtHeinCardToTime, null);
                    dxValidationProvider1.SetValidationRule(txtHeinCardFromTime, null);
                    dxValidationProvider1.SetValidationRule(txtHeinRightRouteCode, null);
                    dxValidationProvider1.SetValidationRule(txtHNCode, null);
                    dxValidationProvider1.SetValidationRule(txtMaNoiChuyenDen, null);
                    dxValidationProvider1.SetValidationRule(txtMaChanDoanTD, null);
                    dxValidationProvider1.SetValidationRule(txtMaDKKCBBD, null);
                }
                else if (IsTempQN == true && checkedCardTemp == true)
                {
                    lblHeincardNumber.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lblHeincardToDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lblHeincardFromDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lblRightRouteType.AppearanceItemCaption.ForeColor = Color.Maroon;
                    txtHeinCardToTime.EditValue = null;
                    dtHeinCardFromTime.EditValue = null;
                    txtHeinCardFromTime.EditValue = null;
                    dtHeinCardToTime.EditValue = null;
                    txtSoThe.EditValue = null;
                    cboSoThe.EditValue = null;
                    txtAddress.Text = "";

                    IList<Control> invalidControls = this.dxValidationProvider1.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        this.dxValidationProvider1.RemoveControlError(invalidControls[i]);
                    }
                    this.dxErrorProvider1.ClearErrors();

                    this.ValidFreeCoPainTime(true);
                    this.ValidRightRouteType();
                    this.ValidHNCode();
                    //this.ValidNoiChuyenDen();
                    this.ValidIcd();
                    this.ValidNoiDKKCBBD();
                }
                else
                {
                    lblRightRouteType.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lblHeincardNumber.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lblHeincardToDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lblHeincardFromDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                    txtHeinCardToTime.EditValue = null;
                    dtHeinCardFromTime.EditValue = null;
                    txtHeinCardFromTime.EditValue = null;
                    dtHeinCardToTime.EditValue = null;
                    txtSoThe.EditValue = null;
                    cboSoThe.EditValue = null;
                    this.ValidTxtSoThe();
                    this.ValidFreeCoPainTime(true);
                    this.ValidHeinCardToTime();
                    this.ValidHeinCardFromTime();
                    this.ValidRightRouteType();
                    this.ValidHNCode();
                    this.ValidIcd();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShortcutKeyDown(System.Windows.Forms.Keys key)
        {
            try
            {
                if (this.dlgsetShortcutKeyDown != null)
                    this.dlgsetShortcutKeyDown(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM GetTranPatiFormById(long id)
        {
            MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM result = null;
            try
            {
                result = DataStore.TranPatiForms.SingleOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (result ?? new MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM());
        }

        private MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON GetTranPatiReasonById(long id)
        {
            MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON result = null;
            try
            {
                result = DataStore.TranPatiReasons.SingleOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (result ?? new MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON());
        }

        internal void ProcessFillDataTranPatiInForm(long treatmentId)
        {
            try
            {
                if (this.currentPatientSdo != null
                    && !String.IsNullOrEmpty(this.currentPatientSdo.AppointmentCode))
                //&& !this.MediOrgCodeCurrent.Equals(patientTypeAlter.HEIN_MEDI_ORG_CODE))
                {
                    return;//xuandv  Neu La Hen Kham Thy Khong Hien Thi thong Tin Chuyen Den
                }
                var treatment = His.UC.UCHein.HisTreatment.HisTreatmentGet.GetById(treatmentId);
                if (treatment != null)
                {
                    if (treatment.TRANSFER_IN_TIME_FROM != null)
                    {
                        dtTransferInTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatment.TRANSFER_IN_TIME_FROM ?? 0) ?? DateTime.Now;
                    }
                    else
                    {
                        dtTransferInTimeFrom.EditValue = null;
                    }
                    if (treatment.TRANSFER_IN_TIME_TO != null)
                    {
                        dtTransferInTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatment.TRANSFER_IN_TIME_TO ?? 0) ?? DateTime.Now;
                    }
                    else
                    {
                        dtTransferInTimeTo.EditValue = null;
                    }
                    this.txtMaHinhThucChuyen.Text = (treatment.TRANSFER_IN_FORM_ID.HasValue ? (this.GetTranPatiFormById(treatment.TRANSFER_IN_FORM_ID.Value) ?? new MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM()).TRAN_PATI_FORM_CODE : "");
                    this.cboHinhThucChuyen.EditValue = treatment.TRANSFER_IN_FORM_ID;

                    this.txtMaLyDoChuyen.Text = (treatment.TRANSFER_IN_REASON_ID.HasValue ? (this.GetTranPatiReasonById(treatment.TRANSFER_IN_REASON_ID.Value) ?? new MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON()).TRAN_PATI_REASON_CODE : "");
                    this.cboLyDoChuyen.EditValue = treatment.TRANSFER_IN_REASON_ID;

                    this.txtMaNoiChuyenDen.Text = treatment.TRANSFER_IN_MEDI_ORG_CODE;
                    this.cboNoiChuyenDen.EditValue = treatment.TRANSFER_IN_MEDI_ORG_CODE;

                    this.chkMediRecordRouteTransfer.Checked = (treatment.TRANSFER_IN_CMKT == 1);
                    this.chkMediRecordNoRouteTransfer.Checked = (treatment.TRANSFER_IN_CMKT == 0);

                    this.txtInCode.Text = treatment.TRANSFER_IN_CODE;
                    this.lblEditIcd.Enabled = (!string.IsNullOrEmpty(treatment.TRANSFER_IN_CODE));
                    this.txtMaChanDoanTD.Text = treatment.TRANSFER_IN_ICD_CODE;
                    //this.cboChanDoanTD.EditValue = treatment.TRANSFER_IN_ICD_ID;
                    if (!string.IsNullOrEmpty(treatment.TRANSFER_IN_ICD_CODE))
                    {
                        var icd = DataStore.Icds.FirstOrDefault(o => o.ICD_CODE == treatment.TRANSFER_IN_ICD_CODE) ?? new MOS.EFMODEL.DataModels.HIS_ICD();
                        this.cboChanDoanTD.EditValue = icd.ID;

                        if (autoCheckIcd == "1" || (!String.IsNullOrEmpty(treatment.TRANSFER_IN_ICD_NAME) && (treatment.TRANSFER_IN_ICD_NAME ?? "").Trim().ToLower() != (icd.ICD_NAME ?? "").Trim().ToLower()))
                        {
                            chkHasDialogText.Checked = true;
                            txtDialogText.Text = treatment.TRANSFER_IN_ICD_NAME;
                        }
                        else
                        {
                            chkHasDialogText.Checked = false;
                            txtDialogText.Text = icd.ICD_NAME;
                        }
                    }
                    else
                    {
                        if (this.IsObligatoryTranferMediOrg)
                        {
                            this.lblEditIcd.Enabled = true;
                            chkHasDialogText.Checked = true;
                        }
                        else
                            chkHasDialogText.Checked = false;
                        txtDialogText.Text = treatment.TRANSFER_IN_ICD_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FocusMoveOut()
        {
            try
            {
                if (this.dlgsetFocusMoveOut != null)
                    this.dlgsetFocusMoveOut();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra hạn của thẻ bhyt, nếu hạn thẻ nhỏ hơn cấu hình giới hạn số ngày cảnh báo hạn thẻ => show cảnh báo cho người dùng
        /// số ngày còn lại của hạn thẻ bhyt nếu sắp hết hạn, nếu có giấy chứng sinh và các trường hợp ngược lại trả về -1
        /// </summary>
        /// <param name="alertExpriedTimeHeinCardBhyt"></param>
        /// <param name="resultDayAlert"></param>
        /// <returns>số ngày còn lại của hạn thẻ bhyt nếu sắp hết hạn, nếu có giấy chứng sinh và các trường hợp ngược lại trả về -1</returns>
        internal long GetExpriedTimeHeinCardBhyt(long alertExpriedTimeHeinCardBhyt, ref long resultDayAlert)
        {
            long result = -1;
            try
            {
                if (this.chkHasDobCertificate.Checked)
                {
                    result = -1;//Truong hop chung sinh tra ve hop le vi ngay server se tu sinh
                }
                else
                {
                    DateTime? dtHeinCardFromTime = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                    DateTime? dtHeinCardToTime = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                    if (dtHeinCardFromTime != null && dtHeinCardFromTime.Value != DateTime.MinValue
                        && dtHeinCardToTime != null && dtHeinCardToTime.Value != DateTime.MinValue)
                    {
                        DateTime dtToTime = dtHeinCardToTime.Value;
                        result = (long)((dtToTime.Date - DateTime.Now.Date).TotalDays);
                        if (result > alertExpriedTimeHeinCardBhyt)
                        {
                            //Chua het han, set lai gia tri tra ve la 0
                            //Cho su dung se kiem tra neu tra ve khong la ngay hop le
                            result = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal void DefaultFocusUserControl()
        {
            try
            {
                if (this.chkHasDobCertificate.Enabled)
                    this.chkHasDobCertificate.Focus();
                else
                {
                    this.txtSoThe.Focus();
                    this.txtSoThe.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetFocusUserByLiveAreaCode()
        {
            try
            {
                this.cboNoiSong.Focus();
                this.cboNoiSong.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void SetLogTime(long logTime)
        {
            try
            {
                this.logTime = logTime;
                this.ShowPatientFromHeinCardNumber();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void SetTreatmentType(long TreatmentTypeId)
        {
            try
            {
                this.TreatmentTypeId = TreatmentTypeId;
                this.ShowPatientFromHeinCardNumber();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FocusHeinCardFromTime()
        {
            try
            {
                this.txtHeinCardFromTime.Focus();
                this.txtHeinCardFromTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void HeinCardNumberKeyDownByRegisterForm(HeinCardData heinCardData, bool isSearchHeinCardNumber)
        {
            try
            {
                if (this.dlgProcessFillDataCareerUnder6AgeByHeinCardNumber != null)
                {
                    this.dlgProcessFillDataCareerUnder6AgeByHeinCardNumber(heinCardData, isSearchHeinCardNumber);
                }

                this.txtHeinCardFromTime.Focus();
                this.txtHeinCardFromTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void UpdateHasDobCertificateEnable(bool hasDobCretificate)
        {
            try
            {
                this.chkHasDobCertificate.Enabled = hasDobCretificate;
                this.chkHasDobCertificate.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataAfterFindQrCode(Inventec.Common.QrCodeBHYT.HeinCardData dataHein)
        {
            try
            {
                IsAutoCheck = true;
                this.txtSoThe.Text = dataHein.HeinCardNumber;
                MOS.EFMODEL.DataModels.HIS_MEDI_ORG dataMediOrg = DataStore.MediOrgs.FirstOrDefault(o => o.MEDI_ORG_CODE == dataHein.MediOrgCode);
                if (dataMediOrg != null)
                {
                    this.txtMaDKKCBBD.Text = dataMediOrg.MEDI_ORG_CODE;
                    this.cboDKKCBBD.EditValue = dataMediOrg.MEDI_ORG_CODE;
                    this.MediOrgSelectRowChange(false, dataHein.LiveAreaCode);
                }
                if (Config.HisConfigCFG.IsAllowedRouteTypeByDefault == "1" && dataMediOrg != null)
                {
                    TuDongChonLoaiThongTuyen(dataMediOrg, dataHein);
                }
                else
                {
                    ChonLoai_DungTuyen_HenKham(dataHein);
                }

                if (!String.IsNullOrEmpty(dataHein.FromDate))
                {
                    this.txtHeinCardFromTime.Text = dataHein.FromDate;
                    this.dtHeinCardFromTime.EditValue = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                }
                else
                {
                    this.txtHeinCardFromTime.Text = "";
                    this.dtHeinCardFromTime.EditValue = null;
                }

                if (!String.IsNullOrEmpty(dataHein.ToDate))
                {
                    this.txtHeinCardToTime.Text = dataHein.ToDate;
                    this.dtHeinCardToTime.EditValue = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                }
                else
                {
                    this.txtHeinCardToTime.Text = "";
                    this.dtHeinCardToTime.EditValue = null;
                }
                if (!string.IsNullOrEmpty(dataHein.FineYearMonthDate))
                {
                    DateTime? _dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(dataHein.FineYearMonthDate);
                    this.txtDu5Nam.Text = dataHein.FineYearMonthDate;
                    this.dtDu5Nam.EditValue = _dt;
                    string ngayHeThong = DateTime.Now.ToString("dd/MM/yyyy");
                    DateTime? _dtNow = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(ngayHeThong);
                    if (entity.IsInitFromCallPatientTypeAlter)
                    {
                        if (_dt < _dtNow && !HisConfigCFG.IsNotAutoCheck5Y6M)
                        {
                            this.chkJoin5Year.Checked = true;
                        }
                        else
                        {
                            this.chkJoin5Year.Checked = false;
                        }
                    }
                    else
                    {
                        if (_dt < _dtNow)
                        {
                            this.chkJoin5Year.Checked = true;
                        }
                        else
                        {
                            this.chkJoin5Year.Checked = false;
                        }
                    }
                }
                else
                {
                    this.txtDu5Nam.Text = "";
                    this.dtDu5Nam.EditValue = null;
                }

                MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData liveArea = DataStore.LiveAreas.SingleOrDefault(o => o.HeinLiveCode == dataHein.LiveAreaCode);
                this.cboNoiSong.EditValue = ((liveArea != null) ? liveArea.HeinLiveCode : null);
                this.chkJoin5Year.Checked = this.chkPaid6Month.Checked = false;
                if (!String.IsNullOrEmpty(dataHein.MediOrgCode)
                    && !String.IsNullOrEmpty(dataHein.PatientName)
                    && !String.IsNullOrEmpty(dataHein.Dob)
                    && !String.IsNullOrEmpty(dataHein.Gender))
                {
                    //xuandv
                    string _address = Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.Address);
                    if (string.IsNullOrEmpty(_address))
                    {
                        this.txtAddress.Text = dataHein.Address;
                    }
                    else
                        this.txtAddress.Text = _address;
                }
                else
                    this.txtAddress.Text = dataHein.Address;

                this.txtHNCode.Text = "";
                if ((MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL == this.HeinLevelCodeCurrent
                        || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE == this.HeinLevelCodeCurrent)
                        && !this.MediOrgCodeCurrent.Equals(this.txtMaDKKCBBD.Text)
                        && this.rdoRightRoute.Checked)
                {
                    if (this.IsDefaultRightRouteType)
                    {
                        this.InitDefaultRightRouteType();
                        Inventec.Common.Logging.LogSystem.Debug("Quet the bhyt load thong tin the. this.IsDefaultRightRouteType = true");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Quet the bhyt load thong tin the. show thong bao phai chon truong hop");
                        if (cboHeinRightRoute.EditValue == null)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.His_UCHein__MaDKKCBBDKhacVoiCuaVienNguoiDungPhaiChonTruongHop), Base.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), DefaultBoolean.True);
                            this.txtHeinRightRouteCode.Focus();
                            this.txtHeinRightRouteCode.SelectAll();
                        }
                        // DevExpress.XtraEditors.XtraMessageBox.Show(Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.His_UCHein__MaDKKCBBDKhacVoiCuaVienNguoiDungPhaiChonTruongHop), Base.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), DefaultBoolean.True);
                        //this.txtHeinRightRouteCode.Focus();
                        //this.txtHeinRightRouteCode.SelectAll();
                    }
                }
                IsAutoCheck = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataAfterCheckBHYT(Inventec.Common.QrCodeBHYT.HeinCardData dataHein)
        {
            try
            {
                this.txtSoThe.Text = dataHein.HeinCardNumber;
                MOS.EFMODEL.DataModels.HIS_MEDI_ORG dataMediOrg = DataStore.MediOrgs.FirstOrDefault(o => o.MEDI_ORG_CODE == dataHein.MediOrgCode);
                if (dataMediOrg != null)
                {
                    this.txtMaDKKCBBD.Text = dataMediOrg.MEDI_ORG_CODE;
                    this.cboDKKCBBD.EditValue = dataMediOrg.MEDI_ORG_CODE;
                    this.MediOrgSelectRowChange(false, dataHein.LiveAreaCode);
                }
                if (Config.HisConfigCFG.IsAllowedRouteTypeByDefault == "1" && dataMediOrg != null)
                {
                    TuDongChonLoaiThongTuyen(dataMediOrg, dataHein);
                }
                else
                {
                    ChonLoai_DungTuyen_HenKham(dataHein);
                }

                if (!String.IsNullOrEmpty(dataHein.FromDate))
                {
                    this.txtHeinCardFromTime.Text = dataHein.FromDate;
                    this.dtHeinCardFromTime.EditValue = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                }
                else
                {
                    this.txtHeinCardFromTime.Text = "";
                    this.dtHeinCardFromTime.EditValue = null;
                }

                if (!String.IsNullOrEmpty(dataHein.ToDate))
                {
                    this.txtHeinCardToTime.Text = dataHein.ToDate;
                    this.dtHeinCardToTime.EditValue = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                }
                else
                {
                    this.txtHeinCardToTime.Text = "";
                    this.dtHeinCardToTime.EditValue = null;
                }
                if (!string.IsNullOrEmpty(dataHein.FineYearMonthDate))
                {
                    DateTime? _dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(dataHein.FineYearMonthDate);
                    this.txtDu5Nam.Text = dataHein.FineYearMonthDate;
                    this.dtDu5Nam.EditValue = _dt;
                    string ngayHeThong = DateTime.Now.ToString("dd/MM/yyyy");
                    DateTime? _dtNow = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(ngayHeThong);
                    if (entity.IsInitFromCallPatientTypeAlter)
                    {
                        if (_dt < _dtNow && !HisConfigCFG.IsNotAutoCheck5Y6M)
                        {
                            this.chkJoin5Year.Checked = true;
                        }
                        else
                        {
                            this.chkJoin5Year.Checked = false;
                        }
                    }
                    else
                    {
                        if (_dt < _dtNow)
                        {
                            this.chkJoin5Year.Checked = true;
                        }
                        else
                        {
                            this.chkJoin5Year.Checked = false;
                        }
                    }

                    //this.txtDu5Nam.Text = dataHein.FineYearMonthDate;
                    //this.dtDu5Nam.EditValue = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(dataHein.FineYearMonthDate);
                }
                else
                {
                    this.txtDu5Nam.Text = "";
                    this.dtDu5Nam.EditValue = null;
                }

                MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData liveArea = DataStore.LiveAreas.SingleOrDefault(o => o.HeinLiveCode == dataHein.LiveAreaCode);
                this.cboNoiSong.EditValue = ((liveArea != null) ? liveArea.HeinLiveCode : null);
                //  this.chkJoin5Year.Checked =  this.chkPaid6Month.Checked = false;
                this.chkPaid6Month.Checked = false;
                if (!String.IsNullOrEmpty(dataHein.MediOrgCode)
                    && !String.IsNullOrEmpty(dataHein.PatientName)
                    && !String.IsNullOrEmpty(dataHein.Dob)
                    && !String.IsNullOrEmpty(dataHein.Gender))
                {
                    //xuandv
                    string _address = Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.Address);
                    if (string.IsNullOrEmpty(_address))
                    {
                        this.txtAddress.Text = dataHein.Address;
                    }
                    else
                        this.txtAddress.Text = _address;
                }
                else
                    this.txtAddress.Text = dataHein.Address;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ChonLoai_DungTuyen_HenKham(HeinCardData dataHein)
        {
            bool success = false;
            try
            {
                if (dataHein == null)
                    return false;
                if (this.currentPatientSdo != null
                && !String.IsNullOrEmpty(this.currentPatientSdo.AppointmentCode)
                && !this.MediOrgCodeCurrent.Equals(dataHein.MediOrgCode))
                {
                    this.rdoRightRoute.Checked = true;
                    this.rdoWrongRoute.Checked = false;
                    this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT;
                    this.txtHeinRightRouteCode.Text = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT;
                    this.cboHeinRightRoute.Properties.Buttons[1].Visible = true;
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private void ChonLoai_DungTuyen_ThongTuyen()
        {
            try
            {
                List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData> dataSource = this.cboHeinRightRoute.Properties.DataSource as List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData>;
                dataSource = dataSource ?? new List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData>();
                if (!dataSource.Exists(o => o.HeinRightRouteTypeCode == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER))
                {
                    dataSource.Add(DataStore.HeinRightRouteTypes.FirstOrDefault(o => o.HeinRightRouteTypeCode == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER));
                    His.UC.UCHein.ControlProcess.HeinRightRouterTypeProcess.FillDataToComboHeinRightRouterType(this.cboHeinRightRoute, dataSource);
                }
                this.rdoRightRoute.Checked = true;
                this.rdoWrongRoute.Checked = false;
                this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER;
                this.txtHeinRightRouteCode.Text = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER;
                this.cboHeinRightRoute.Properties.Buttons[1].Visible = true;
                this.SetEnableControlHein(RightRouterFactory.RIGHT_ROUTER, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TuDongChonLoaiThongTuyen(MOS.EFMODEL.DataModels.HIS_MEDI_ORG MediOrgADO, Inventec.Common.QrCodeBHYT.HeinCardData dataHein)
        {
            try
            {
                if (MediOrgADO != null && !String.IsNullOrEmpty(MediOrgADO.MEDI_ORG_CODE))
                {
                    var result = MacDinhLoaiThongTuyen(MediOrgADO);

                    if (String.IsNullOrWhiteSpace(result))
                        return;
                    if (result == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (!ChonLoai_DungTuyen_HenKham(dataHein))
                        {
                            this.rdoRightRoute.Checked = true;
                            this.rdoWrongRoute.Checked = false;
                            this.SetEnableControlHein(RightRouterFactory.RIGHT_ROUTER, false);
                        }
                    }
                    else if (result == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER)
                    {
                        ChonLoai_DungTuyen_ThongTuyen();
                    }
                    else if (result == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)
                    {
                        this.rdoRightRoute.Checked = false;
                        this.rdoWrongRoute.Checked = !this.rdoRightRoute.Checked;
                        this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string MacDinhLoaiThongTuyen(MOS.EFMODEL.DataModels.HIS_MEDI_ORG MediOrgADO)
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

        internal void ResetValidationControl()
        {
            try
            {
                IList<Control> invalidControls = this.dxValidationProvider1.GetInvalidControls();
                for (int i = invalidControls.Count - 1; i >= 0; i--)
                {
                    this.dxValidationProvider1.RemoveControlError(invalidControls[i]);
                }
                this.dxErrorProvider1.ClearErrors();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal bool GetInvalidControls()
        {
            bool valid = true;
            try
            {
                this.positionHandleControl = -1;
                if (!this.dxValidationProvider1.Validate())
                {
                    IList<Control> invalidControls = this.dxValidationProvider1.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        internal void SelectMediOrgForSearch(bool isSearch)
        {
            try
            {
                this.MediOrgSelectRowChange(isSearch, (cboNoiSong.EditValue ?? "").ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiSong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtMaNoiChuyenDen.Enabled == true)
                    {
                        txtMaNoiChuyenDen.Focus();
                        txtMaNoiChuyenDen.SelectAll();
                    }
                    else
                    {
                        txtHNCode.Focus();
                        txtHNCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void AutoCheckRightRoute(bool IsDungTuyenCapCuu)
        {
            try
            {
                this.IsDungTuyenCapCuuByTime = IsDungTuyenCapCuu;
                if (IsDungTuyenCapCuu)
                {
                    rdoRightRoute.Checked = true;
                    cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                    txtHeinRightRouteCode.Text = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                    this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTCC, false);
                    dxValidationProvider1.SetValidationRule(this.txtMaNoiChuyenDen, null);
                }
                else if (rdoRightRoute.Checked)
                {
                    //this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTGT, false);
                    this.txtHeinCardToTime.Enabled = true;
                    this.txtMaNoiChuyenDen.Enabled = true;
                    this.chkHasDialogText.Enabled = true;
                    this.cboNoiChuyenDen.Enabled = true;
                    this.txtDialogText.Enabled = true;
                    this.txtMaChanDoanTD.Enabled = true;
                    this.cboChanDoanTD.Enabled = true;
                    this.chkJoin5Year.Enabled = true;
                    this.chkMediRecordRouteTransfer.Enabled = true;
                    this.chkMediRecordNoRouteTransfer.Enabled = true;
                    this.cboHinhThucChuyen.Enabled = true;
                    this.txtMaHinhThucChuyen.Enabled = true;
                    this.txtMaLyDoChuyen.Enabled = true;
                    this.cboLyDoChuyen.Enabled = true;
                    dtTransferInTimeFrom.Enabled = true;
                    dtTransferInTimeTo.Enabled = true;

                    MOS.EFMODEL.DataModels.HIS_MEDI_ORG mediorg = DataStore.MediOrgs.SingleOrDefault(o => o.MEDI_ORG_CODE == (this.cboDKKCBBD.EditValue ?? "").ToString());
                    if (entity.IsAutoSelectEmergency)
                    {
                        this.AutoSelectEmergency(entity);
                    }
                    else if (mediorg != null
                         && !string.IsNullOrEmpty(mediorg.MEDI_ORG_CODE)
                         && (this.MediOrgCodeCurrent == mediorg.MEDI_ORG_CODE
                                 || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT == this.HeinLevelCodeCurrent
                                || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE == this.HeinLevelCodeCurrent))
                    // || this.IsMediOrgRightRouteByCurrent(mediorg.MEDI_ORG_CODE)))
                    {
                        this.cboHeinRightRoute.Properties.Buttons[1].Visible = false;
                        this.cboHeinRightRoute.EditValue = null;
                        this.txtHeinRightRouteCode.Text = "";
                    }
                    else if (mediorg != null)
                    {
                        this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT;
                        this.txtHeinRightRouteCode.Text = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT;
                        this.cboHeinRightRoute.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        this.cboHeinRightRoute.Properties.Buttons[1].Visible = false;
                        this.cboHeinRightRoute.EditValue = null;
                        this.txtHeinRightRouteCode.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ChangeRoomNotEmergency()
        {
            try
            {
                this.IsDungTuyenCapCuuByTime = false;
                this.cboHeinRightRoute.EditValue = null;
                //this.rdoRightRoute.Checked = true;

                MediOrgSelectRowChange(true, (cboNoiSong.EditValue ?? "").ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransferInTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                dtTransferInTimeTo.Focus();
                dtTransferInTimeTo.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransferInTimeFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtTransferInTimeTo.Focus();
                    dtTransferInTimeTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransferInTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                txtMaHinhThucChuyen.Focus();
                txtMaHinhThucChuyen.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransferInTimeTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaHinhThucChuyen.Focus();
                    txtMaHinhThucChuyen.SelectAll();
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

                if (!dxValidationProvider1.Validate(txtSoThe))
                    Valid = false;

                if (!Valid)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CheckInfoBHYT").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.CheckInfoBHYT");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();

                    HIS.Desktop.ADO.CheckInfoBhytADO ado = new CheckInfoBhytADO();
                    ado.TDL_PATIENT_NAME = entity.HisTreatment.TDL_PATIENT_NAME;
                    ado.TDL_DOB = entity.HisTreatment.TDL_PATIENT_DOB;
                    ado.TDL_GENDER_NAME = entity.HisTreatment.TDL_PATIENT_GENDER_NAME;
                    ado.TDL_HEIN_CARD_NUMBER = HeinUtils.TrimHeinCardNumber(this.txtSoThe.Text);
                    listArgs.Add(ado);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listArgs), listArgs));
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, entity.currentModule.RoomId, entity.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, entity.currentModule.RoomId, entity.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void DisposeControl()
        {
            try
            {
                IsDungTuyenCapCuuByTime = false;
                _TextIcdName = null;
                patientTypeAlterOld = null;
                isCallByRegistor = false;
                ExceedDayAllow = 0;
                isDefaultInit = false;
                currentPatientSdo = null;
                ObligatoryTranferMediOrg = null;
                IsObligatoryTranferMediOrg = false;
                IsTempQN = false;
                IsEdit = false;
                IsNotRequiredRightTypeInCaseOfHavingAreaCode = false;
                IsDefaultRightRouteType = false;
                autoCheckIcd = null;
                isShowCheckKhongKTHSD = null;
                isVisibleControl = 0;
                PatientTypeIdBHYT = 0;
                TreatmentTypeIdExam = 0;
                MediOrgCodesAccepts = null;
                MediOrgCodeCurrent = null;
                HeinLevelCodeCurrent = null;
                SysMediOrgCode = null;
                entity = null;
                CultureInfo = null;
                TreatmentTypeCode = null;
                PatientTypeId = 0;
                positionHandleControl = 0;
                actChangePatientDob = null;
                _DelegateSetRelativeAddress = null;
                dlgfillDataPatientSDOToRegisterForm = null;
                dlgcheckExamHistory = null;
                dlgautoCheckCC = null;
                dlgProcessFillDataCareerUnder6AgeByHeinCardNumber = null;
                dlgsetShortcutKeyDown = null;
                dlgsetFocusMoveOut = null;
                this.txtHeinCardToTime.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtHeinCardToTime_ButtonClick);
                this.txtHeinCardToTime.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtHeinCardToTime_InvalidValue);
                this.txtHeinCardToTime.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtHeinCardToTime_PreviewKeyDown);
                this.txtHeinCardFromTime.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtHeinCardFromTime_ButtonClick);
                this.txtHeinCardFromTime.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtHeinCardFromTime_InvalidValue);
                this.txtHeinCardFromTime.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtHeinCardFromTime_PreviewKeyDown);
                this.dtHeinCardToTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtHeinCardToTime_Closed);
                this.dtHeinCardToTime.EditValueChanged -= new System.EventHandler(this.dtHeinCardToTime_EditValueChanged);
                this.dtHeinCardToTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtHeinCardToTime_KeyDown);
                this.dtHeinCardFromTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtHeinCardFromTime_Closed);
                this.dtHeinCardFromTime.EditValueChanged -= new System.EventHandler(this.dtHeinCardFromTime_EditValueChanged);
                this.dtHeinCardFromTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtHeinCardFromTime_KeyDown);
                this.btnCheckInfoBHYT.Click -= new System.EventHandler(this.btnCheckInfoBHYT_Click);
                this.rdoWrongRoute.CheckedChanged -= new System.EventHandler(this.rdoWrongRoute_CheckedChanged);
                this.rdoWrongRoute.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.rdoWrongRoute_PreviewKeyDown);
                this.dtTransferInTimeTo.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtTransferInTimeTo_Closed);
                this.dtTransferInTimeTo.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtTransferInTimeTo_PreviewKeyDown);
                this.dtTransferInTimeFrom.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtTransferInTimeFrom_Closed);
                this.dtTransferInTimeFrom.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtTransferInTimeFrom_PreviewKeyDown);
                this.txtDu5Nam.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtDu5Nam_ButtonClick);
                this.txtDu5Nam.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDu5Nam_PreviewKeyDown);
                this.dtDu5Nam.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtDu5Nam_Closed);
                this.dtDu5Nam.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtDu5Nam_KeyDown);
                this.cboNoiSong.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboNoiSong_Closed);
                this.cboNoiSong.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboNoiSong_ButtonClick);
                this.cboNoiSong.EditValueChanged -= new System.EventHandler(this.cboNoiSong_EditValueChanged);
                this.cboNoiSong.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboNoiSong_KeyUp);
                this.cboNoiSong.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboNoiSong_PreviewKeyDown);
                this.chkTempQN.CheckedChanged -= new System.EventHandler(this.chkTempQN_CheckedChanged);
                this.txtFreeCoPainTime.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtFreeCoPainTime_ButtonClick);
                this.txtFreeCoPainTime.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtFreeCoPainTime_InvalidValue);
                this.txtFreeCoPainTime.TextChanged -= new System.EventHandler(this.txtDTMCChiTra_TextChanged);
                this.txtFreeCoPainTime.Click -= new System.EventHandler(this.txtFreeCoPainTime_Click);
                this.txtFreeCoPainTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtFreeCoPainTime_KeyDown);
                this.txtFreeCoPainTime.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtFreeCoPainTime_KeyPress);
                this.txtFreeCoPainTime.Validating -= new System.ComponentModel.CancelEventHandler(this.txtFreeCoPainTime_Validating);
                this.dtFreeCoPainTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtFreeCoPainTime_Closed);
                this.dtFreeCoPainTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtFreeCoPainTime_KeyDown);
                this.txtInCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtInCode_PreviewKeyDown);
                this.txtHNCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtHNCode_PreviewKeyDown);
                this.rdoRightRoute.CheckedChanged -= new System.EventHandler(this.rdoRightRoute_CheckedChanged);
                this.rdoRightRoute.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.rdoRightRoute_PreviewKeyDown);
                this.chkPaid6Month.CheckedChanged -= new System.EventHandler(this.chkPaid6Month_CheckedChanged);
                this.chkPaid6Month.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkPaid6Month_PreviewKeyDown);
                this.cboChanDoanTD.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboChanDoanTD_Closed);
                this.cboChanDoanTD.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboChanDoanTD_ButtonClick);
                this.cboChanDoanTD.TextChanged -= new System.EventHandler(this.cboChanDoanTD_TextChanged);
                this.cboChanDoanTD.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboChanDoanTD_KeyUp);
                this.txtMaChanDoanTD.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtMaChanDoanTD_InvalidValue);
                this.txtMaChanDoanTD.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaChuanDoanTD_PreviewKeyDown);
                this.txtMaChanDoanTD.Validating -= new System.ComponentModel.CancelEventHandler(this.txtMaChanDoanTD_Validating);
                this.chkHasDialogText.CheckedChanged -= new System.EventHandler(this.chkHasDialogText_CheckedChanged);
                this.chkHasDialogText.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkHasDialogText_PreviewKeyDown);
                this.cboLyDoChuyen.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboLyDoChuyen_Closed);
                this.cboLyDoChuyen.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboLyDoChuyen_ButtonClick);
                this.cboLyDoChuyen.EditValueChanged -= new System.EventHandler(this.cboLyDoChuyen_EditValueChanged);
                this.cboLyDoChuyen.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboLyDoChuyen_PreviewKeyDown);
                this.txtMaLyDoChuyen.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaLyDoChuyen_PreviewKeyDown);
                this.cboHeinRightRoute.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboHeinRightRoute_Closed);
                this.cboHeinRightRoute.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboHeinRightRoute_ButtonClick);
                this.cboHeinRightRoute.EditValueChanged -= new System.EventHandler(this.cboHeinRightRoute_EditValueChanged);
                this.cboHeinRightRoute.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboHeinRightRoute_KeyUp);
                this.cboHeinRightRoute.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboHeinRightRoute_PreviewKeyDown);
                this.txtHeinRightRouteCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtHeinRightRouteCode_PreviewKeyDown);
                this.cboDKKCBBD.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDKKCBBD_Closed);
                this.cboDKKCBBD.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboDKKCBBD_KeyUp);
                this.txtMaDKKCBBD.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaDKKCBBD_PreviewKeyDown);
                this.cboNoiChuyenDen.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboNoiChuyenDen_Closed);
                this.cboNoiChuyenDen.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboNoiChuyenDen_ButtonClick);
                this.cboNoiChuyenDen.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboNoiChuyenDen_KeyUp);
                this.txtMaNoiChuyenDen.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaNoiChuyenDen_PreviewKeyDown);
                this.cboHinhThucChuyen.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboHinhThucChuyen_Closed);
                this.cboHinhThucChuyen.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboHinhThucChuyen_ButtonClick);
                this.cboHinhThucChuyen.EditValueChanged -= new System.EventHandler(this.cboHinhThucChuyen_EditValueChanged);
                this.cboHinhThucChuyen.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboHinhThucChuyen_KeyUp);
                this.txtMaHinhThucChuyen.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaHinhThucChuyen_PreviewKeyDown);
                this.txtAddress.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtAddress_PreviewKeyDown);
                this.chkJoin5Year.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkJoin5Year_PreviewKeyDown);
                this.chkMediRecordNoRouteTransfer.CheckedChanged -= new System.EventHandler(this.chkMediRecordNoRouteTransfer_CheckedChanged);
                this.chkMediRecordNoRouteTransfer.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkMediRecordNoRouteTransfer_PreviewKeyDown);
                this.chkMediRecordRouteTransfer.CheckedChanged -= new System.EventHandler(this.chkMediRecordRouteTransfer_CheckedChanged);
                this.chkMediRecordRouteTransfer.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkMediRecordRouteTransfer_PreviewKeyDown);
                this.txtSoThe.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtSoThe_Properties_ButtonClick);
                this.txtSoThe.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtSoThe_InvalidValue);
                this.txtSoThe.EditValueChanged -= new System.EventHandler(this.txtSoThe_EditValueChanged);
                this.txtSoThe.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtSoThe_KeyDown);
               
                this.cboSoThe.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboSoThe_Closed);
                this.cboSoThe.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboSoThe_KeyUp);
                this.chkHasDobCertificate.CheckedChanged -= new System.EventHandler(this.chkHasDobCertificate_CheckedChanged);
                this.chkHasDobCertificate.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkHasDobCertificate_PreviewKeyDown);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.Template__HeinBHYT1_Load);
                cboNoiSong.Properties.DataSource = null;
                gridView2.GridControl.DataSource = null;
                gridView3.GridControl.DataSource = null;
                cboChanDoanTD.Properties.DataSource = null;
                cboLyDoChuyen.Properties.DataSource = null;
                cboHeinRightRoute.Properties.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboDKKCBBD.Properties.DataSource = null;
                gridView1.GridControl.DataSource = null;
                cboNoiChuyenDen.Properties.DataSource = null;
                cboHinhThucChuyen.Properties.DataSource = null;
                cboSoThe.Properties.DataSource = null;
                layoutControlItem1 = null;
                btnCheckInfoBHYT = null;
                lciFordtTransferInTimeTo = null;
                lciFordtTransferInTimeFrom = null;
                dtTransferInTimeFrom = null;
                dtTransferInTimeTo = null;
                lciDu5Nam = null;
                dtDu5Nam = null;
                txtDu5Nam = null;
                panel4 = null;
                cboNoiSong = null;
                lblMediRecordLiveArea = null;
                gridView2 = null;
                lciTempQN = null;
                chkTempQN = null;
                dtFreeCoPainTime = null;
                lciFreeCoPainTime = null;
                panelControl1 = null;
                txtFreeCoPainTime = null;
                lciInCode = null;
                txtInCode = null;
                lciHNCode = null;
                txtHNCode = null;
                lciKhongKTHSD = null;
                checkKhongKTHSD = null;
                lcirdoRightRoute = null;
                rdoRightRoute = null;
                lcirdoWrongRoute = null;
                rdoWrongRoute = null;
                lcichkPaid6Month = null;
                chkPaid6Month = null;
                txtDialogText = null;
                gridView3 = null;
                cboChanDoanTD = null;
                panelICD = null;
                lciIcdMain = null;
                lblEditIcd = null;
                chkHasDialogText = null;
                txtMaChanDoanTD = null;
                panel5 = null;
                lciTransPatiReasoncbo = null;
                lciTransPatiReasonCode = null;
                txtMaLyDoChuyen = null;
                cboLyDoChuyen = null;
                lciRightRouteTypeName = null;
                lblRightRouteType = null;
                txtHeinRightRouteCode = null;
                cboHeinRightRoute = null;
                lciDKKCBBDName = null;
                lblHeincardMediOrg = null;
                txtMaDKKCBBD = null;
                gridLookUpEdit1View = null;
                cboDKKCBBD = null;
                lciNoiChuyenDenName = null;
                lblMediRecordMediOrgForm = null;
                txtMaNoiChuyenDen = null;
                gridView1 = null;
                cboNoiChuyenDen = null;
                lciTransPatiFormCbo = null;
                cboHinhThucChuyen = null;
                lciTransPatiFormCode = null;
                txtMaHinhThucChuyen = null;
                lblCaptionAddress = null;
                txtAddress = null;
                dxValidationProvider1 = null;
                dxErrorProvider1 = null;
                lblMediRecordBenefitSymbol = null;
                lcichkJoin5Year = null;
                lciMediRecordNoRouteTransfer = null;
                lciMediRecordRouteTransfer = null;
                lblHeincardFromDate = null;
                lblHeincardToDate = null;
                lblHeincardNumber = null;
                panel1 = null;
                panel2 = null;
                panel3 = null;
                lblCaptionHasDobCertificate = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
                cboSoThe = null;
                chkJoin5Year = null;
                chkMediRecordRouteTransfer = null;
                chkMediRecordNoRouteTransfer = null;
                txtMucHuong = null;
                chkHasDobCertificate = null;
                txtSoThe = null;
                dtHeinCardFromTime = null;
                dtHeinCardToTime = null;
                txtHeinCardFromTime = null;
                txtHeinCardToTime = null;
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
                this.ChangeDefaultHeinRatio();
                this.Join5YearAndPaid6MonthCheckedChanged();
                if (entity.IsInitFromCallPatientTypeAlter)
                {
                    this.ValidateCheckBox5Y(chkJoin5Year.Checked);
                    if (!chkJoin5Year.Checked && !chkPaid6Month.Checked)
                        IsShowMessage = false;
                    else if (chkJoin5Year.Checked && chkPaid6Month.Checked)
                        IsShowMessage = true;
                    if (chkJoin5Year.Checked && (chkJoin5Year.OldEditValue != chkJoin5Year.EditValue))
                    {
                        this.ShowMessageNotAutoCheck5Y6M(this.chkJoin5Year);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER> patyAlters { get; set; }
        public long TreatmentTypeId { get; private set; }
        List<PatientTypeAlterADO> lstPatientTypeAlterMap { get; set; }
        internal void ShowPatientFromHeinCardNumber()
        {
            try
            {
                if (this.PatientId == 0 || TreatmentTypeId == 0)
                    return;
                if (patyAlters == null || patyAlters.Count == 0) {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientTypeAlterFilter patientTypeAlterFilter = new MOS.Filter.HisPatientTypeAlterFilter();
                    patientTypeAlterFilter.TDL_PATIENT_ID = PatientId;
                    patientTypeAlterFilter.PATIENT_TYPE_ID = PatientTypeIdBHYT;
                    patyAlters = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>>(RequestUriStore.HIS_PATIENT_TYPE_ALTER__GET, ApiConsumerStore.MosConsumer, patientTypeAlterFilter, param);
                }
                if(patyAlters!= null && patyAlters.Count > 0)
                {
                    long valueAdd = HisConfigs.Get<long>("MOS.BHYT.EXCEED_DAY_ALLOW_FOR_IN_PATIENT");

                    lstPatientTypeAlterMap = new List<PatientTypeAlterADO>();
                    foreach (var item in patyAlters.Where(o => !string.IsNullOrEmpty(o.HEIN_CARD_NUMBER)).ToList())
                    {
                        PatientTypeAlterADO ado = new PatientTypeAlterADO(item);
                        lstPatientTypeAlterMap.Add(ado);
                    }
                    lstPatientTypeAlterMap.ForEach(o =>
                    {
                        if (TreatmentTypeId != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU && TreatmentTypeId != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                        {
                            o.HEIN_CARD_TO_TIME_CAL = o.HEIN_CARD_TO_TIME != null ? Int64.Parse(o.HEIN_CARD_TO_TIME.ToString().Substring(0, 12)) : 0;
                        }
                        else
                        {
                            o.HEIN_CARD_TO_TIME_CAL = o.HEIN_CARD_TO_TIME != null ? Int64.Parse(Inventec.Common.DateTime.Calculation.Add(o.HEIN_CARD_TO_TIME ?? 0, valueAdd, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY).ToString().Substring(0, 12)) : 0;
                        }
                    });
                    lstPatientTypeAlterMap= lstPatientTypeAlterMap.Where(o => o.HEIN_CARD_TO_TIME_CAL >= logTime).ToList();

                    if(lstPatientTypeAlterMap != null && lstPatientTypeAlterMap.Count > 0)
                    {
                        lstPatientTypeAlterMap = lstPatientTypeAlterMap.Distinct(new Compare()).ToList();
                    }    
                }
                if (lstPatientTypeAlterMap == null)
                    lstPatientTypeAlterMap = new List<PatientTypeAlterADO>();
                ReloadComboSoThe(cboSoThe, null,lstPatientTypeAlterMap);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadComboSoThe(GridLookUpEdit cboEditor, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER> patientTypeAlters, List<PatientTypeAlterADO> lstMap = null)
        {
            try
            {
                List<PatientTypeAlterADO> lst = lstMap;
                if (lstMap == null)
                {
                    lst = new List<PatientTypeAlterADO>();
                    foreach (var item in patientTypeAlters)
                    {
                        PatientTypeAlterADO ado = new PatientTypeAlterADO(item);
                        lst.Add(ado);
                    }
                }
                cboEditor.Properties.DataSource = lst;
                cboEditor.Properties.DisplayMember = "RENDERER_HEIN_CARD_NUMBER";
                cboEditor.Properties.ValueMember = "RENDERER_HEIN_CARD_NUMBER";

                cboEditor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboEditor.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboEditor.Properties.ImmediatePopup = true;
                cboEditor.ForceInitialize();
                cboEditor.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboEditor.Properties.View.Columns.AddField("RENDERER_HEIN_CARD_NUMBER");
                aColumnCode.Caption = "Số thẻ BHYT";
                aColumnCode.Width = 150;
                aColumnCode.VisibleIndex = 1;

                GridColumn bColumnCode = cboEditor.Properties.View.Columns.AddField("RENDERER_FROM_DATE_TODATE");
                bColumnCode.Caption = "Hạn thẻ";
                bColumnCode.Width = 250;
                bColumnCode.VisibleIndex = 2;

                GridColumn cColumnCode = cboEditor.Properties.View.Columns.AddField("HEIN_MEDI_ORG_NAME");
                cColumnCode.Caption = "Nơi ĐKKCB BĐ";
                cColumnCode.ToolTip = "Nơi đăng ký khám chữa bệnh ban đầu";
                cColumnCode.Width = 250;
                cColumnCode.VisibleIndex = 3;

                cboEditor.Properties.View.OptionsView.ColumnAutoWidth = false;
                cboEditor.Properties.View.OptionsView.ShowIndicator = false;
                cboEditor.Properties.View.OptionsView.ShowGroupPanel = false;
                cboEditor.Properties.PopupFormSize = new Size(650, cboEditor.Height);
                cboEditor.Properties.View.OptionsView.ShowColumnHeaders = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSoThe_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    txtSoThe_Properties_ButtonClick(null, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkBaby_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (chkBaby.Checked)
                    rdoRightRoute.Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkTt46_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkTt46.Checked)
                {
                    txtTt46.Enabled = true;
                    rdoRightRoute.Checked = true;
                }
                else
                {
                    txtTt46.Text = null;
                    txtTt46.Enabled = false;
                }
                if (rdoRightRoute.Checked)
                    ValidateRightRouteType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHasWorkingLetter_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if(chkHasWorkingLetter.Checked)
                    rdoRightRoute.Checked = true;
                if (rdoRightRoute.Checked)
                    ValidateRightRouteType();
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
                if (chkHasAbsentLetter.Checked)
                    rdoRightRoute.Checked = true;
                if (rdoRightRoute.Checked)
                    ValidateRightRouteType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

}
