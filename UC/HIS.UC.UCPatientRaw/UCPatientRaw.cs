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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.UCPatientRaw.ClassUCPatientRaw;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.PopupLoader;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utility;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Common.Logging;
using HIS.UC.UCPatientRaw.ADO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.DelegateRegister;
using Inventec.Common.QrCodeBHYT;
using MOS.SDO;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using HIS.UC.UCPatientRaw.Base;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using Inventec.Core;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraGrid.Views.Base;
using SDA.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.HisConfig;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using SDA.EFMODEL.DataModels;

namespace HIS.UC.UCPatientRaw
{
    public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        DelegateSendIdData dlgPatientClassifyId;
        EventHandler PatientTypeEditValueChanged;
        DelegateFocusNextUserControl dlgFocusNextUserControl;
        DelegateFocusNextUserControl dlgFocusToUCRelativeWhenPatientIsChild;
        DelegateValidationUserControl dlgSetValidation;
        DelegateEnableOrDisableControl isEnable;
        DelegateEnableOrDisableBtnPatientNew enableLciBenhNhanMoi;
        DelegateSetFocusWhenPatientIsChild dlgSetFocusWhenPatientIsChild;
        DelegateVisible isVisible;
        DelegateCheckTT dlgCheckTT;
        DelegateHeinEnableButtonSave dlgHeinEnableSave;
        DelegateEnableButtonSave dlgEnableSave;
        DelegateSetDataRegisterBeforeSerachPatient dlgSearchPatient1;
        Action<HisPatientSDO> actInitExamServiceRoomByAppoimentTime;
        DelegateVisibleUCHein dlgVisibleUCHein;
        GetIntructionTime dlgGetIntructionTime;
        Action<bool> dlgIsReadQrCode;
        Action dlgResetRegisterForm;
        Action<HeinCardData> dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw;
        DelegateShowControlHrmKskCode dlgShowControlHrmKskCode;
        DelegateShowControlHrmKskCodeNotValid dlgShowControlHrmKskCodeNotValid;
        DelegateShowControlGuaranteeLoginname dlgShowControlGuaranteeLoginname;
        DelegateSendPatientName dlgSendPatientName;
        DelegateSendPatientSDO dlgSendPatientSdo;
        Action dlgProcessChangePatientDob;
        DelegateCheckSS dlgCheckSS;
        DelegateEnableFindType dlgEnableFindType;
        DelegateCheckExamOnline dlgCheckExamOnline;
        bool isDefault = false;
		Inventec.Common.QrCodeBHYT.HeinCardData qrCodeBHYTHeinCardData;
		// public HeinCardData _HeinCardData = new HeinCardData();
		HisCardSDO cardSearch { get; set; }
		HisPatientSDO currentPatientSDO { get; set; }
		List<HisPatientSDO> currentSearchedPatients;
		HIS_PATIENT_TYPE currentPatientType;
		public string typeCodeFind = ResourceMessage.typeCodeFind__MaBN;
        internal long? typeReceptionForm = null;

        internal bool isDobTextEditKeyEnter;
        internal bool isNotPatientDayDob = false;
        public bool isReadQrCode;
        bool isChild;
        int positionHandleControl = -1;
        bool isGKS;
        bool isTemp_QN = false;
        private UCPatientRawADO _UCPatientRawADO { get; set; }//Luu data de check TT
        public bool isAlertTreatmentEndInDay { get; set; }
        public ResultDataADO ResultDataADO { get; set; }

        List<HIS_PATIENT_TYPE> primaryPatientTypes = new List<HIS_PATIENT_TYPE>();
        List<HIS_PATIENT_TYPE> paties = new List<HIS_PATIENT_TYPE>();
        List<SDA.EFMODEL.DataModels.SDA_ETHNIC> dataEthnic = new List<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
        Action<bool> dlgShowCheckWorkingLetter;
        Action<long> dlgShowOtherPaySource;

        bool isShowContainer = false;
        bool isShowContainerForChoose = false;
        bool isShow = true;

        List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL> currentHideControls;
        string MODULE_LINK = "HIS.Desktop.Plugins.RegisterV2";
        string APP_CODE__EXACT = "HIS";
        List<string> currentNameControl = new List<string>();
        MOS.EFMODEL.DataModels.HIS_BRANCH branch;
        string baseNameControl = "";
        HeinCardData dataHeinCardFromQrCccd = null;
        string CccdCardFromQrCccd { get; set; }
        string AddressFromQrCccd { get; set; }
        string ReleaseDateQrCccd { get; set; }
        bool IsEmergency { get; set; }
        public bool IsLoadFromSearchTxtCode { get; private set; }

        public string BhytCode { get; set; }
        public long? BhytWhiteListtId { get; set; }
        string codeColumn { get; set; }
        string nameColumn { get; set; }
        string nameYear { get; set; }
        internal enum ItemType
        {
            DTChiTiet,
            DonVi,
            QuanHam,
            ChucVu
        }

        #endregion


        #region Constructor - Load

        public UCPatientRaw()
            : base("HIS.Desktop.Plugins.RegisterV2", "UCPatientRaw")
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCPatientRaw .1");
                InitializeComponent();
                Inventec.Common.Logging.LogSystem.Debug("UCPatientRaw .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCPatientRaw_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCPatientRaw_Load .1");
                SetCaptionByLanguageKey();
                paties = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(p => p.IS_ACTIVE == 1 && p.IS_NOT_USE_FOR_PATIENT != 1).ToList();
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => paties), paties));
                InitFieldFromAsync();
                Inventec.Common.Logging.LogSystem.Debug("UCPatientRaw_Load .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public async Task InitFieldFromAsync()
        {
            try
            {
                this.ValidateControl();
                VisiblePrimaryPatientType();
                GetDataVisibleControlFromPatientClassify();

                Inventec.Common.Logging.LogSystem.Debug("UCPatientRaw.InitFieldFromAsync .1");
                this.SetDataDropDownBtn();
                this.DisableControlCareerByKeyConfig(HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.VisibilityControl);
                await this.FillDataDefaultToControl();
                this.txtPatientCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("UCPatientRaw.InitFieldFromAsync .2");
        }

        public void SetEmergencyFromDepartment(bool IsEmergency)
        {
            try
            {
                this.IsEmergency = IsEmergency;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDataVisibleControlFromPatientClassify()
        {
            try
            {

                var branchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>();
                this.branch = (branchs != null && branchs.Count > 0) ? branchs.FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.BranchWorker.GetCurrentBranchId()) : null;


                CommonParam paramCommon = new CommonParam();
                SdaHideControlFilter filter = new SdaHideControlFilter();
                filter.MODULE_LINK__EXACT = MODULE_LINK;
                filter.APP_CODE__EXACT = APP_CODE__EXACT;
                this.currentHideControls = new BackendAdapter(paramCommon).Get<List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL>>("api/sdaHideControl/Get", ApiConsumers.SdaConsumer, filter, paramCommon);

                this.currentHideControls = (this.currentHideControls != null && this.currentHideControls.Count > 0) ? this.currentHideControls.Where(o => o.BRANCH_CODE == null || (branch != null && o.BRANCH_CODE == branch.BRANCH_CODE)).ToList() : null;

                if (this.currentHideControls != null && this.currentHideControls.Count > 0)
                {
                    currentNameControl = this.currentHideControls.Select(o => o.CONTROL_PATH).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisiblePrimaryPatientType()
        {
            try
            {
                if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                {
                    lciPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciComboPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lciPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciComboPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoadDataCboDoiTuong(long roomId)
        {
            try
            {
                List<long> patientIds = new List<long>();
                List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> resultPatient = new List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                if (paties != null && paties.Count > 0)
                {

                    patientIds = paties.Select(o => o.ID).ToList();


                    var dataRecption = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_RECEPTION_ROOM>().Where(o => o.ROOM_ID == roomId).ToList();
                    if (dataRecption != null && dataRecption.Count > 0)
                    {
                        HIS_RECEPTION_ROOM currentReception = dataRecption.FirstOrDefault();
                        if (currentReception.PATIENT_TYPE_IDS != null)
                        {
                            string[] receptionIds = currentReception.PATIENT_TYPE_IDS.Split(',');
                            for (int i = 0; i < patientIds.Count; i++)
                            {
                                for (int j = 0; j < receptionIds.Count(); j++)
                                {
                                    if (Int64.Parse(receptionIds[j]) == patientIds[i])
                                    {
                                        resultPatient.Add(paties.Where(o => o.ID == Int64.Parse(receptionIds[j])).FirstOrDefault());
                                    }
                                }
                            }

                        }
                        if (resultPatient != null && resultPatient.Count > 0)
                        {
                            if (resultPatient.Count == 1)
                            {
                                isDefault = true;
                            }
                            paties = resultPatient;
                        }
                    }

                }
                EditorLoaderProcessor.InitComboCommon(this.cboPatientType, paties, "ID", "PATIENT_TYPE_NAME", "PATIENT_TYPE_CODE");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitData(EventHandler patientTypeEditValueChanged, GetIntructionTime _dlgGetIntructionTime, DelegateSendIdData dlgPatientClassifyId)
        {
            try
            {
                this.dlgPatientClassifyId = dlgPatientClassifyId;
                this.PatientTypeEditValueChanged = patientTypeEditValueChanged;
                this.dlgGetIntructionTime = _dlgGetIntructionTime;
                this.FillDefaultData_Carrer_Gender_PatientType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitDelegateProcessChangePatientDob(Action _dlgProcessChangePatientDob)
        {
            try
            {
                this.dlgProcessChangePatientDob = _dlgProcessChangePatientDob;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCPatientRaw
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.UCPatientRaw.Resources.Lang", typeof(UCPatientRaw).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEthnic.Properties.NullText = Inventec.Common.Resource.Get.Value("UCPatientRaw.cboEthnic.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPrimaryPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCPatientRaw.cboPrimaryPatientType.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCPatientRaw.cboPatientType.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCareer.Properties.NullText = Inventec.Common.Resource.Get.Value("UCPatientRaw.cboCareer.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAge.Properties.NullText = Inventec.Common.Resource.Get.Value("UCPatientRaw.cboAge.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtPatientDob.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCPatientRaw.dtPatientDob.Properties.NullValuePrompt", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGender.Properties.NullText = Inventec.Common.Resource.Get.Value("UCPatientRaw.cboGender.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCodeFind.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.btnCodeFind.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup1.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.layoutControlGroup1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.layoutControlItem4.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.layoutControlItem3.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaNgheNghiep.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCPatientRaw.lciMaNgheNghiep.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaNgheNghiep.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.lciMaNgheNghiep.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientDob.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.lciPatientDob.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPrimaryPatientType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCPatientRaw.lciPrimaryPatientType.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPrimaryPatientType.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.lciPrimaryPatientType.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFortxtEthnicCode.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.lciFortxtEthnicCode.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.layoutControlItem10.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientClassifyNew.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCPatientRaw.lciPatientClassifyNew.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientClassifyNew.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.lciPatientClassifyNew.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciWorkPlaceNameNew.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.lciWorkPlaceNameNew.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMilitaryRankNew.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.lciMilitaryRankNew.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPositionNew.Text = Inventec.Common.Resource.Get.Value("UCPatientRaw.lciPositionNew.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                codeColumn = Inventec.Common.Resource.Get.Value("UCPatientRaw.codeColumn.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                nameColumn = Inventec.Common.Resource.Get.Value("UCPatientRaw.nameColumn.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                nameYear = Inventec.Common.Resource.Get.Value("UCPatientRaw.nameAge.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task FillDataDefaultToControl()
        {
            try
            {
                var ListAgeType = new List<AgeType>();
                var listAges = await InitDataAgeTypeStorage();
                foreach (var item in listAges)
                {
                    AgeType ado = new AgeType(item, nameYear);
                    ListAgeType.Add(ado);
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListAgeType), ListAgeType));
                EditorLoaderProcessor.InitComboCommon(this.cboAge, BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO>(), "Id", "MoTa", "");
                EditorLoaderProcessor.InitComboCommon(this.cboGender, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>(), "ID", "GENDER_NAME", "GENDER_CODE");
                EditorLoaderProcessor.InitComboCommon(this.cboCareer, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "ID", "CAREER_NAME", "CAREER_CODE");
                cboCareer.Properties.ImmediatePopup = true;
                this.InitEthnic();
                this.LoadPatientClassify();
                this.LoadMilitaryRank();
                this.LoadPosition();
                this.LoadWorkPlace();
                this.LoadDataWhiteList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task<List<HIS_AGE_TYPE>> InitDataAgeTypeStorage()
        {
            try
            {
                //this.moduleField = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SDA_MODULE_FIELD>();
                CommonParam paramCommon = new CommonParam();
                dynamic dfilter = new System.Dynamic.ExpandoObject();
                dfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_AGE_TYPE>>("api/HisAgeType/Get", ApiConsumers.MosConsumer, dfilter, paramCommon);
                if (data != null) BackendDataWorker.UpdateToRam(typeof(HIS_AGE_TYPE), data, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                return data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return new List<HIS_AGE_TYPE> { };
        }
        #endregion

        #region Showpopup and set Data for DropDown

        private void SetDataDropDownBtn()
        {
            try
            {
                InitTypeFind();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Event Gender

        private void cboGender_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.KeyCode == Keys.Down || e.KeyCode == Keys.Up) && !cboGender.IsPopupOpen)
                {
                    cboGender.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGender_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboGender.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_GENDER gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboGender.EditValue ?? "0").ToString()));
                        if (gt != null)
                        {
                            this.SearchPatientByFilterCombo();

                            this.CheckTheBaoHiem();
                        }
                    }
                    this.txtPatientDob.Focus();
                    this.txtPatientDob.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGender_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    if (this.cboGender.EditValue != null)
                //    {
                //        MOS.EFMODEL.DataModels.HIS_GENDER gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboGender.EditValue ?? "0").ToString()));
                //        if (gt != null)
                //        {
                //            this.SearchPatientByFilterCombo();
                //            this.CheckTheBaoHiem();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void txtGenderCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
        //            if (String.IsNullOrEmpty(strValue))
        //            {
        //                this.cboGender.EditValue = null;
        //                this.FocusShowpopup(this.cboGender, true);
        //            }
        //            else
        //            {
        //                var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().Where(o => o.GENDER_CODE.Contains(strValue)).ToList();
        //                var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.GENDER_CODE.ToUpper() == strValue.ToUpper()).ToList()) : null;
        //                if (searchResult != null && searchResult.Count == 1)
        //                {
        //                    this.cboGender.EditValue = searchResult[0].ID;
        //                    this.txtGenderCode.Text = searchResult[0].GENDER_CODE;
        //                    this.txtPatientDob.Focus();

        //                    this.SearchPatientByFilterCombo();
        //                    this.CheckTheBaoHiem();
        //                }
        //                else
        //                {
        //                    this.cboGender.EditValue = null;
        //                    this.FocusShowpopup(this.cboGender, true);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        #endregion

        #region Event PatientDob

        private void txtPatientDob_Validated(object sender, EventArgs e)
        {
            try
            {
                //if (this._UCPatientRawADO != null && !string.IsNullOrEmpty(this._UCPatientRawADO.DOB_STR) && !string.IsNullOrEmpty(this.txtPatientDob.Text.Trim()) && this.txtPatientDob.Text.Trim() != this._UCPatientRawADO.DOB_STR)
                //{

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtPatientDob.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtPatientDob.EditValue = dt;
                        this.dtPatientDob.Update();
                        this.dlgCheckSS(DateTime.Now.Year - dtPatientDob.DateTime.Year < 6);
                    }
                    this.dtPatientDob.Visible = true;
                    this.dtPatientDob.ShowPopup();
                    this.dtPatientDob.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtPatientDob.Text)) return;

                string dob = "";
                if (this.txtPatientDob.Text.Contains("/"))
                    dob = PatientDobUtil.PatientDobToDobRaw(this.txtPatientDob.Text);

                if (!String.IsNullOrEmpty(dob))
                {
                    this.txtPatientDob.Text = dob;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject != null)
                {
                    e.ErrorText = dateValidObject.Message;
                }

                AutoValidate = AutoValidate.EnableAllowFocusChange;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_KeyPress(object sender, KeyPressEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.isDobTextEditKeyEnter = true;

                    DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                    if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                    {
                        this.txtAge.Text = this.txtPatientDob.Text;
                        this.cboAge.EditValue = 1;
                        this.txtPatientDob.Text = dateValidObject.Age.ToString();
                        this.dlgCheckSS(dateValidObject.Age < 6);
                    }
                    else if (String.IsNullOrEmpty(dateValidObject.Message))
                    {
                        if (!dateValidObject.HasNotDayDob)
                        {
                            this.txtPatientDob.Text = dateValidObject.OutDate;
                            this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                            this.dtPatientDob.Update();
                            this.dlgCheckSS(DateTime.Now.Year - dtPatientDob.DateTime.Year < 6);
                        }
                    }

                    if ((txtPatientCode.Text.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null)) && this.typeCodeFind == ResourceMessage.typeCodeFind__MaCMCC)
                    {
                        SearchPatientByCodeOrQrCode(txtPatientCode.Text.Trim());

                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Bat dau CheckTT tu txtPatientDob_PreviewKeyDown");
                        this.CheckTheBaoHiem();
                    }
                    if (this.dlgProcessChangePatientDob != null)
                        this.dlgProcessChangePatientDob();
                    if (this.lciMaNgheNghiep.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never && this.lciNgheNghiep.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    {
                        this.txtPatientTypeCode.SelectAll();
                        this.txtPatientTypeCode.Focus();
                    }
                    else
                    {
                        this.txtCareerCode.Focus();
                        this.txtCareerCode.SelectAll();
                    }
                    //string dob = this.txtPatientDob.Text.Trim();
                    //if (dob.Length == 4)
                    //    this._HeinCardData.Dob = string.Format(dob, "yyyy");
                    //else if (dob.Length >= 10)
                    //    this._HeinCardData.Dob = string.Format(dob, "dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtPatientDob.Text.Trim()))
                    return;
                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                {
                    this.txtAge.Text = this.txtPatientDob.Text;
                    this.cboAge.EditValue = 1;
                    this.txtPatientDob.Text = dateValidObject.Age.ToString();
                    this.dlgCheckSS(dateValidObject.Age < 6);
                }
                else if (String.IsNullOrEmpty(dateValidObject.Message))
                {
                    if (!dateValidObject.HasNotDayDob)
                    {
                        this.txtPatientDob.Text = dateValidObject.OutDate;
                        this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                        this.dtPatientDob.Update();
                        this.dlgCheckSS(DateTime.Now.Year - dtPatientDob.DateTime.Year < 6);
                    }
                }
                else
                {
                    e.Cancel = true;
                    return;
                }

                this.isNotPatientDayDob = dateValidObject.HasNotDayDob;
                if (
                    ((this.txtPatientDob.EditValue ?? "").ToString() != (this.txtPatientDob.OldEditValue ?? "").ToString())
                    && (!String.IsNullOrEmpty(dateValidObject.OutDate))
                    )
                {
                    this.dxValidationProviderControl.RemoveControlError(this.txtPatientDob);
                    this.txtPatientDob.ErrorText = "";
                    this.CalulatePatientAge(dateValidObject.OutDate);
                    this.SetValueCareerComboByCondition();
                    if (!((txtPatientCode.Text.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null)) && this.typeCodeFind == ResourceMessage.typeCodeFind__MaCMCC))
                    {
                        this.SearchPatientByFilterCombo();
                    }
                }
                if (this.isDobTextEditKeyEnter && this.txtAge.Enabled)
                {
                    this.txtAge.Focus();
                    this.txtAge.SelectAll();
                    this.ValidateTextAge();
                }
                else
                {
                    this.dxValidationProviderControl.RemoveControlError(this.txtAge);
                    dxValidationProviderControl.SetValidationRule(this.txtAge, null);
                }
                this.isDobTextEditKeyEnter = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProviderControl_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalulatePatientAge(string strDob)
        {
            try
            {
                this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                if (this.dtPatientDob.EditValue != null && this.dtPatientDob.DateTime != DateTime.MinValue)
                {
                    isGKS = true;
                    DateTime dtNgSinh = this.dtPatientDob.DateTime;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    if (tongsogiay < 0)
                    {
                        this.txtAge.EditValue = "";
                        this.cboAge.EditValue = 4;
                        return;
                    }
                    DateTime newDate = new DateTime(tongsogiay);

                    int nam = newDate.Year - 1;
                    int thang = newDate.Month - 1;
                    int ngay = newDate.Day - 1;
                    int gio = newDate.Hour;
                    int phut = newDate.Minute;
                    int giay = newDate.Second;
                    isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtNgSinh);
                    this.IsStateEnableAgeOld = false;
                    if (nam >= 7)
                    {
                        this.cboAge.EditValue = 1;
                        this.txtAge.Enabled = false;
                        this.cboAge.Enabled = false;
                        if (!isGKS)
                        {
                            this.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                        }
                        else
                        {
                            this.txtAge.EditValue = nam.ToString();
                        }
                    }
                    else if (nam > 0 && nam < 7)
                    {
                        if (nam == 6)
                        {
                            if (thang > 0 || ngay > 0)
                            {
                                this.cboAge.EditValue = 1;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                                if (!isGKS)
                                {
                                    this.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                                }
                                else
                                {
                                    this.txtAge.EditValue = nam.ToString();
                                }
                            }
                            else
                            {
                                this.txtAge.EditValue = nam * 12 - 1;
                                this.cboAge.EditValue = 2;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                            }

                        }
                        else
                        {
                            this.txtAge.EditValue = nam * 12 + thang;
                            this.cboAge.EditValue = 2;
                            this.txtAge.Enabled = false;
                            this.cboAge.Enabled = false;
                        }

                    }
                    else
                    {
                        if (thang > 0)
                        {
                            this.txtAge.EditValue = thang.ToString();
                            this.cboAge.EditValue = 2;
                            this.txtAge.Enabled = false;
                            this.cboAge.Enabled = false;
                        }
                        else
                        {
                            if (ngay > 0)
                            {
                                this.txtAge.EditValue = ngay.ToString();
                                this.cboAge.EditValue = 3;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                            }
                            else
                            {
                                this.txtAge.EditValue = "";
                                this.cboAge.EditValue = 4;
                                this.txtAge.Enabled = true;
                                this.IsStateEnableAgeOld = true;
                                this.cboAge.Enabled = false;
                            }
                        }
                    }
                    if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.MustHaveNCSInfoForChild == true || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.RelativesInforOption == "1" || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.RelativesInforOption == "2")
                    {
                        this.dlgSetValidation(isGKS);
                        Inventec.Common.Logging.LogSystem.Debug("Da bat Validate thong tin nguoi nha");
                    }
                    if (this.dlgSetFocusWhenPatientIsChild != null)
                        this.dlgSetFocusWhenPatientIsChild(isGKS);
                    if (this.dlgProcessChangePatientDob != null)
                        this.dlgProcessChangePatientDob();
                    if (this.isTemp_QN == true || (this.isTemp_QN == true && this.isGKS == true))
                        this.isEnable(null, true);
                    else if (this.isGKS == true)
                        this.isEnable(true, null);
                    else
                    {
                        this.isEnable(null, false);
                        Inventec.Common.Logging.LogSystem.Debug("Da an checkbox the tam : isTemp_QN = " + isTemp_QN);
                    }
                    //TODO
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueCareerComboByCondition()
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_CAREER career = null;
                bool isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(this.dtPatientDob.DateTime);
                if (isGKS)
                {
                    career = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerUnder6Age;
                }
                else if (DateTime.Now.Year - this.dtPatientDob.DateTime.Year <= 18)
                {
                    career = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerHS;
                }
                else if (cboCareer.EditValue == null)
                {
                    career = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerBase;
                }
                if (career != null)
                {
                    this.txtCareerCode.Text = career.CAREER_CODE;
                    this.cboCareer.EditValue = career.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPatientDob_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtPatientDob.Visible = false;

                    if (!String.IsNullOrWhiteSpace(txtPatientDob.Text)
                        && (txtPatientDob.Text.Length == 6 || txtPatientDob.Text.Length == 7)
                        && (txtPatientDob.Text == dtPatientDob.DateTime.ToString("MM/yyyy") || txtPatientDob.Text == dtPatientDob.DateTime.ToString("MMyyyy"))
                        && dtPatientDob.DateTime.Day == 1)
                    {
                        this.txtPatientDob.Text = dtPatientDob.DateTime.ToString("MM/yyyy");
                    }
                    else
                    {
                        this.txtPatientDob.Text = dtPatientDob.DateTime.ToString("dd/MM/yyyy");
                    }
                    string strDob = dtPatientDob.DateTime.ToString("dd/MM/yyyy");
                    this.CalulatePatientAge(strDob);
                    this.SetValueCareerComboByCondition();
                    this.dxValidationProviderControl.RemoveControlError(this.txtAge);
                    if (this.txtAge.Enabled)
                    {
                        this.txtAge.Focus();
                        this.txtAge.SelectAll();
                        this.ValidateTextAge();
                    }
                    else
                    {
                        dxValidationProviderControl.SetValidationRule(this.txtAge, null);
                        this.txtCareerCode.Focus();
                        this.txtCareerCode.SelectAll();
                    }
                    if ((txtPatientCode.Text.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null)) && this.typeCodeFind == ResourceMessage.typeCodeFind__MaCMCC)
                    {
                        SearchPatientByCodeOrQrCode(txtPatientCode.Text.Trim());

                    }
                    else
                    {
                        this.SearchPatientByFilterCombo();
                        Inventec.Common.Logging.LogSystem.Debug("Bat dau CheckTT tu dtPatientDob_Close");
                        this.CheckTheBaoHiem();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPatientDob_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtPatientDob.Visible = true;
                    this.dtPatientDob.Update();
                    if (!String.IsNullOrWhiteSpace(txtPatientDob.Text)
                        && (txtPatientDob.Text.Length == 6 || txtPatientDob.Text.Length == 7)
                        && (txtPatientDob.Text == dtPatientDob.DateTime.ToString("MM/yyyy") || txtPatientDob.Text == dtPatientDob.DateTime.ToString("MMyyyy"))
                        && dtPatientDob.DateTime.Day == 1)
                    {
                        this.txtPatientDob.Text = dtPatientDob.DateTime.ToString("MM/yyyy");
                    }
                    else
                    {
                        this.txtPatientDob.Text = dtPatientDob.DateTime.ToString("dd/MM/yyyy");
                    }

                    this.CalulatePatientAge(this.txtPatientDob.Text);
                    this.SetValueCareerComboByCondition();
                    if (!((txtPatientCode.Text.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null)) && this.typeCodeFind == ResourceMessage.typeCodeFind__MaCMCC))
                    {
                        this.SearchPatientByFilterCombo();
                    }

                    System.Threading.Thread.Sleep(100);
                    this.dxValidationProviderControl.RemoveControlError(this.txtAge);
                    if (this.txtAge.Enabled)
                    {
                        this.txtAge.Focus();
                        this.txtAge.SelectAll();
                        this.ValidateTextAge();
                    }
                    else
                    {
                        dxValidationProviderControl.SetValidationRule(this.txtAge, null);
                        this.txtCareerCode.Focus();
                        this.txtCareerCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Event Career

        private void txtCareerCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (String.IsNullOrEmpty(strValue))
                    {
                        this.cboCareer.EditValue = null;
                        this.FocusShowpopup(this.cboCareer, false);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.CAREER_CODE.Contains(strValue)).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.CAREER_CODE.ToUpper() == strValue.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboCareer.EditValue = searchResult[0].ID;
                            this.txtCareerCode.Text = searchResult[0].CAREER_CODE;
                            if (this.TD3)
                            {
                                this.dlgFocusNextUserControl();
                            }
                            else
                            {
                                this.txtPatientTypeCode.Focus();
                            }
                        }
                        else
                        {
                            this.cboCareer.EditValue = null;
                            this.FocusShowpopup(this.cboCareer, false);
                        }
                    }
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
                //if (e.CloseMode == PopupCloseMode.Normal)
                //{
                if (this.cboCareer.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_CAREER career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboCareer.EditValue ?? "0").ToString()));
                    if (career != null)
                    {
                        this.txtCareerCode.Text = career.CAREER_CODE;
                    }
                    if (this.TD3)
                    {
                        this.dlgFocusNextUserControl();
                    }
                    else
                    {
                        this.txtPatientTypeCode.Focus();
                        this.txtPatientTypeCode.SelectAll();
                    }
                }
                else
                {
                    //this.FocusShowpopup(this.cboCareer, true);
                }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCareer_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboCareer.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_CAREER career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboCareer.EditValue ?? "0").ToString()));
                        if (career != null)
                        {
                            this.txtCareerCode.Text = career.CAREER_CODE;
                        }
                    }
                    else
                    {
                        if (HisConfigs.Get<string>("HIS.Desktop.Plugins.RegisterV2.IsNotCareerRequired") != "1")
                        {
                            this.FocusShowpopup(this.cboCareer, false);
                        }
                        else
                        {
                            txtPatientTypeCode.Focus();
                            txtPatientTypeCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event PatientName

        private void txtPatientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if ((txtPatientCode.Text.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null)) && this.typeCodeFind == ResourceMessage.typeCodeFind__MaCMCC)
                    {
                        SearchPatientByCodeOrQrCode(txtPatientCode.Text.Trim());

                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Bat dau CheckTT tu txtPatientName_Validate");
                        this.CheckTheBaoHiem();
                    }
                    //if (!string.IsNullOrEmpty(txtPatientName.Text) && this.dlgSendPatientName != null)
                    //{
                    //    this.dlgSendPatientName(txtPatientName.Text.Trim());
                    //}

                    cboGender.Focus();
                    cboGender.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientName_Validated(object sender, EventArgs e)
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug("Bat dau CheckTT tu txtPatientName_Validate");
                //this.CheckTheBaoHiem();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event PatientType

        private void cboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UcPatientRaw__cboPatientType_EditValueChanged____ XXXXXXXX");
                bool isValid = false;
                bool isVisible = false;
                if (this.cboPatientType.EditValue != null)
                {
                    var pt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()));
                    if (pt != null)
                    {
                        this.txtPatientTypeCode.Text = pt.PATIENT_TYPE_CODE;
                        this.isTemp_QN = (pt.PATIENT_TYPE_CODE == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeCode__QN);
                        if (this.isEnable != null)
                        {
                            if (this.isTemp_QN == true || (this.isGKS == true && this.isTemp_QN == true))
                                this.isEnable(null, true);
                            else if (this.isGKS == true)
                                this.isEnable(true, null);
                            else
                                this.isEnable(null, false);
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("UcPatientRaw__cboPatientType_EditValueChanged____ isEnable is null");
                        }

                        ReloadDataCboPatientClassifyByPatientType(pt);
                        //if (!IsLoadFromSearchTxtCode)
                        {
                            this.PatientTypeEditValueChanged(sender, e);
                        }
                        this.currentPatientType = pt;
                        if (pt.MUST_BE_GUARANTEED == 1)
                        {
                            isValid = true;
                        }
                        if (pt.IS_ADDITION_REQUIRED == 1)
                        {
                            isVisible = true;
                        }
                    }

                }
                else
                {
                    isValid = false;
                    this.currentPatientType = null;
                }
                if (!IsLoadFromSearchTxtCode)
                    ClearControlCombo();
                ValidateOtherCpn();
                VisiblePrimaryPatientType(isVisible);
                LoadDataComboPrimaryPatientType();
                if (this.dlgShowControlGuaranteeLoginname != null)
                {
                    this.dlgShowControlGuaranteeLoginname(isValid);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ClearControlCombo()
        {
            try
            {
                currentPatientClassify = null;
                currentPosition = null;
                currentWorkPlace = null;
                currentMilitaryRank = null;
                txtPatientClassify.Text = "";
                txtWorkPlace.Text = "";
                txtMilitaryRank.Text = "";
                txtPosition.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisiblePrimaryPatientType(bool IsVisible)
        {
            try
            {
                if (lciPrimaryPatientType.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (!IsVisible)
                    {
                        lciPrimaryPatientType.AppearanceItemCaption.ForeColor = Color.Black;
                        dxValidationProviderControl.SetValidationRule(txtPrimaryPatientTypeCode, null);
                    }
                    else
                    {
                        lciPrimaryPatientType.AppearanceItemCaption.ForeColor = Color.Maroon;
                        ValidatePrimaryPatientType();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (String.IsNullOrEmpty(strValue))
                    {
                        this.cboPatientType.EditValue = null;
                        this.FocusShowpopup(this.cboPatientType, true);
                    }
                    else
                    {
                        var dataSource = cboPatientType.Properties.DataSource as List<HIS_PATIENT_TYPE>;
                        var data = dataSource.Where(o => o.PATIENT_TYPE_CODE.ToLower().Contains(strValue.ToLower())).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.PATIENT_TYPE_CODE.ToLower() == strValue.ToLower()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            if (this.cboPatientType.EditValue == null
                                || searchResult[0].ID != (long)this.cboPatientType.EditValue)
                            {
                                this.cboPatientType.EditValue = searchResult[0].ID;
                                this.txtPatientTypeCode.Text = searchResult[0].PATIENT_TYPE_CODE;
                            }
                            cboPatientType_Closed(null, null);
                            //if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                            //    this.dlgFocusToUCRelativeWhenPatientIsChild();
                            //else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                            //    this.dlgFocusNextUserControl();
                        }
                        else
                        {
                            this.cboPatientType.EditValue = null;
                            this.FocusShowpopup(this.cboPatientType, true);
                        }
                    }
                    //var pt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE.ToLower() == this.txtPatientTypeCode.Text.Trim().ToLower());
                    //if (pt != null && pt.ID > 0)
                    //    this.dlgVisibleUCHein(pt.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (lciPrimaryPatientType.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    txtPrimaryPatientTypeCode.Focus();
                    txtPrimaryPatientTypeCode.SelectAll();
                }
                else if (lciPatientClassifyNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    ShowPopupNearTxt(txtPatientClassify, ItemType.DTChiTiet);
                }
                else if (lciWorkPlaceNameNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    ShowPopupNearTxt(txtWorkPlace, ItemType.DonVi);
                }
                else if (lciMilitaryRankNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    ShowPopupNearTxt(txtMilitaryRank, ItemType.QuanHam);
                }
                else if (lciPositionNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    ShowPopupNearTxt(txtPosition, ItemType.ChucVu);
                }
                else if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                    this.dlgFocusToUCRelativeWhenPatientIsChild();
                else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                    this.dlgFocusNextUserControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (String.IsNullOrEmpty(this.cboPatientType.Text))
                    {
                        this.FocusShowpopup(this.cboPatientType, false);
                        e.Handled = true;
                    }
                    else
                    {
                        cboPatientType_Closed(null, null);
                        //if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                        //    this.dlgFocusToUCRelativeWhenPatientIsChild();
                        //else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                        //    this.dlgFocusNextUserControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event PatientCode

        private void txtPatientCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    IsLoadFromSearchTxtCode = true;
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    SearchPatientByCodeOrQrCode(strValue);
                    IsLoadFromSearchTxtCode = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region private Method

        internal void LoadEthnicBase()
        {
            try
            {
                var ethnicDefault = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.EthinicBase;
                if (ethnicDefault != null)
                {
                    this.cboEthnic.EditValue = ethnicDefault.ETHNIC_CODE;
                    this.txtEthnicCode.Text = ethnicDefault.ETHNIC_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckTheBaoHiem()
        {
            try
            {
                bool valid = true;
                if (this._UCPatientRawADO == null || String.IsNullOrEmpty(this._UCPatientRawADO.DOB_STR))
                {
                    this._UCPatientRawADO = GetValue();
                }

                valid = valid && (this._UCPatientRawADO != null);
                valid = valid && (!string.IsNullOrEmpty(this._UCPatientRawADO.PATIENT_NAME) && !string.IsNullOrEmpty(this.txtPatientName.Text.Trim()));
                valid = valid && (!string.IsNullOrEmpty(this._UCPatientRawADO.DOB_STR) && !string.IsNullOrEmpty(this.txtPatientDob.Text.Trim()));
                MOS.EFMODEL.DataModels.HIS_GENDER gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboGender.EditValue ?? "0").ToString()));
                valid = valid && (gt != null && gt.ID > 0);

                bool validChanged = (this.txtPatientName.Text != this._UCPatientRawADO.PATIENT_NAME.ToUpper()
                    || (this.txtPatientDob.Text.Trim() != this._UCPatientRawADO.DOB_STR)
                    || (this._UCPatientRawADO.GENDER_ID != gt.ID)
                    );

                if (valid)// && (validChanged || ResultDataADO == null))
                {
                    HeinCardData _HeinCardData = new HeinCardData();
                    _HeinCardData.PatientName = this.txtPatientName.Text.Trim();
                    _HeinCardData.Dob = this.txtPatientDob.Text.Trim();
                    _HeinCardData.Gender = (gt.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE ? "2" : "1");

                    if (this.dlgCheckTT != null)
                        this.dlgCheckTT(_HeinCardData, null);

                    //this._UCPatientRawADO = GetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        public async Task InitEthnic()
        {
            try
            {
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.ChangeEthnic != 0)
                {
                    this.lciFortxtEthnicCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciForcboEthnic.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    List<SDA.EFMODEL.DataModels.SDA_ETHNIC> datas = null;
                    if (BackendDataWorker.IsExistsKey<SDA.EFMODEL.DataModels.SDA_ETHNIC>())
                    {   
                        datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
                    }
                    else
                    {
                        CommonParam paramCommon = new CommonParam();
                        dynamic filter = new System.Dynamic.ExpandoObject();
                        datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<SDA.EFMODEL.DataModels.SDA_ETHNIC>>("api/SdaEthnic/Get", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer, filter, paramCommon);

                        if (datas != null) BackendDataWorker.UpdateToRam(typeof(SDA.EFMODEL.DataModels.SDA_ETHNIC), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                    }
                    this.dataEthnic = datas;
                    this.InitComboCommon(this.cboEthnic, datas, "ETHNIC_CODE", "ETHNIC_NAME", "ETHNIC_CODE");
                    this.LoadEthnicBase();
                }
                else
                {
                    lciNgheNghiep.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                    lciNgheNghiep.MinSize = new Size(cboGender.Width - 23, 24);
                    lciNgheNghiep.Update();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateForResetRegisterForm(Action resetRegister)
        {
            try
            {
                if (resetRegister != null)
                {
                    this.dlgResetRegisterForm = resetRegister;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void SetDelegateCheckboxExamOnline(DelegateCheckExamOnline dlgCheck)
        {
            this.dlgCheckExamOnline = dlgCheck;
        }
        public void SetDelegateCheckSS(DelegateCheckSS dlgcheck)
        {
            this.dlgCheckSS = dlgcheck;
        }
        public void SetDelegateSendTypeFind(DelegateEnableFindType dlgEnableFindType)
        {
            this.dlgEnableFindType = dlgEnableFindType;
        }

        private void txtEthnicCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = ((sender as DevExpress.XtraEditors.TextEdit).Text);
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboEthnic.EditValue = null;
                        FocusShowpopup(this.cboEthnic, false);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().Where(o => o.ETHNIC_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.ETHNIC_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboEthnic.EditValue = searchResult[0].ETHNIC_CODE;
                            this.txtEthnicCode.Text = searchResult[0].ETHNIC_CODE;

                            if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                                this.dlgFocusToUCRelativeWhenPatientIsChild();
                            else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                                this.dlgFocusNextUserControl();
                        }
                        else
                        {
                            this.cboEthnic.EditValue = null;
                            FocusShowpopup(this.cboEthnic, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEthnic_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SDA.EFMODEL.DataModels.SDA_ETHNIC ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().SingleOrDefault(o => o.ETHNIC_CODE == (this.cboEthnic.EditValue ?? "").ToString());
                    if (ethnic != null)
                    {
                        this.txtEthnicCode.Text = ethnic.ETHNIC_CODE;
                    }

                    if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                        this.dlgFocusToUCRelativeWhenPatientIsChild();
                    else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                        this.dlgFocusNextUserControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEthnic_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboEthnic.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_ETHNIC data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().SingleOrDefault(o => o.ETHNIC_CODE == (this.cboEthnic.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            this.txtEthnicCode.Text = data.ETHNIC_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAge_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtPatientTypeCode.Focus();
                    this.txtPatientTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPatientName.Text) && this.dlgSendPatientName != null)
                {
                    this.dlgSendPatientName(txtPatientName.Text.Trim());
                    long genderId = GenderHelper.GenerateGenderByPatientName(txtPatientName.Text.Trim());
                    if (genderId > 0)
                    {
                        var searchResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().Where(o => o.ID == genderId).FirstOrDefault();
                        if (searchResult != null)
                        {
                            this.cboGender.EditValue = searchResult.ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == btnCodeFind)
                {
                    ToolTipController toolTip = sender as ToolTipController;
                    if (toolTip != null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("toolTip: " + toolTip.ToString());
                        var lastInfo = new ToolTipControlInfo(null, toolTip.GetToolTip(e.SelectedControl));
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPrimaryPatientTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (String.IsNullOrEmpty(strValue))
                    {
                        this.cboPrimaryPatientType.EditValue = null;
                        this.FocusShowpopup(this.cboPrimaryPatientType, true);
                    }
                    else
                    {
                        var data = this.primaryPatientTypes != null ? this.primaryPatientTypes.Where(o => o.PATIENT_TYPE_CODE.ToLower().Contains(strValue.ToLower())).ToList() : null;
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.PATIENT_TYPE_CODE.ToLower() == strValue.ToLower()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            if (this.cboPrimaryPatientType.EditValue == null
                                || searchResult[0].ID != (long)this.cboPrimaryPatientType.EditValue)
                            {
                                this.cboPrimaryPatientType.EditValue = searchResult[0].ID;
                                this.txtPrimaryPatientTypeCode.Text = searchResult[0].PATIENT_TYPE_CODE;
                            }
                            if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                                this.dlgFocusToUCRelativeWhenPatientIsChild();
                            else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                                this.dlgFocusNextUserControl();
                        }
                        else
                        {
                            this.cboPrimaryPatientType.EditValue = null;
                            this.FocusShowpopup(this.cboPrimaryPatientType, true);
                        }
                    }
                    //var pt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE.ToLower() == this.txtPatientTypeCode.Text.Trim().ToLower());
                    //if (pt != null && pt.ID > 0)
                    //    this.dlgVisibleUCHein(pt.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowPopupNearTxt(ButtonEdit txt, ItemType item)
        {
            try
            {
                RebuildControlContainer(item);
                Rectangle buttonBounds = new Rectangle(txt.Bounds.X, txt.Bounds.Y, txt.Bounds.Width, txt.Bounds.Height);
                popupControlContainer1.ShowPopup(new Point(buttonBounds.X + 3, buttonBounds.Bottom + 170));
                gridViewPopUp.Focus();
                gridViewPopUp.FocusedRowHandle = 0;
                txt.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPrimaryPatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (lciPatientClassifyNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    ShowPopupNearTxt(txtPatientClassify, ItemType.DTChiTiet);
                }
                else if (lciWorkPlaceNameNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    ShowPopupNearTxt(txtWorkPlace, ItemType.DonVi);
                }
                else if (lciMilitaryRankNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    ShowPopupNearTxt(txtMilitaryRank, ItemType.DonVi);
                }
                else if (lciPositionNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    ShowPopupNearTxt(txtPosition, ItemType.ChucVu);
                }
                else if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                    this.dlgFocusToUCRelativeWhenPatientIsChild();
                else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                    this.dlgFocusNextUserControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPrimaryPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtPrimaryPatientTypeCode.Text = "";
                if (cboPrimaryPatientType.EditValue != null)
                {
                    cboPrimaryPatientType.Properties.Buttons[1].Visible = true;
                    var paty = this.primaryPatientTypes != null ? this.primaryPatientTypes.FirstOrDefault(o => o.ID == Convert.ToInt64(cboPrimaryPatientType.EditValue)) : null;
                    if (paty != null)
                    {
                        txtPrimaryPatientTypeCode.Text = paty.PATIENT_TYPE_CODE;
                    }
                }
                else
                {
                    cboPrimaryPatientType.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataComboPrimaryPatientType()
        {
            try
            {
                if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                {
                    primaryPatientTypes = new List<HIS_PATIENT_TYPE>();
                    var patyAlows = BackendDataWorker.Get<V_HIS_PATIENT_TYPE_ALLOW>();
                    if (this.currentPatientType != null)
                    {
                        primaryPatientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(p => p.IS_ACTIVE == 1 && p.ID != this.currentPatientType.ID
                            && patyAlows != null && patyAlows.Any(a => a.PATIENT_TYPE_ID == this.currentPatientType.ID && a.PATIENT_TYPE_ALLOW_ID == p.ID)).ToList();
                    }
                    if (this.cboPrimaryPatientType.EditValue != null && (this.currentPatientType == null || this.currentPatientType.ID == Convert.ToInt64(cboPrimaryPatientType.EditValue)))
                    {
                        this.cboPrimaryPatientType.EditValue = null;
                    }
                    if (this.cboPrimaryPatientType.EditValue != null && (primaryPatientTypes == null || !primaryPatientTypes.Any(a => a.ID == Convert.ToInt64(this.cboPrimaryPatientType.EditValue))))
                    {
                        this.cboPrimaryPatientType.EditValue = null;
                    }
                    EditorLoaderProcessor.InitComboCommon(this.cboPrimaryPatientType, primaryPatientTypes, "ID", "PATIENT_TYPE_NAME", "PATIENT_TYPE_CODE");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPrimaryPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPrimaryPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barManager1_HighlightedLinkChanged(object sender, DevExpress.XtraBars.HighlightedLinkChangedEventArgs e)
        {
            try
            {
                if (e.Link != null)
                {
                    ToolTipControlInfo info = null;
                    try
                    {
                        info = new ToolTipControlInfo(MousePosition, "");
                        info.SuperTip = e.Link.Item.SuperTip;
                    }
                    finally
                    {
                        toolTipController1.ShowHint(info);
                    }
                }
                else
                {
                    toolTipController1.HideHint();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void RebuildControlContainer(ItemType item)
        {
            try
            {
                string fCol1 = null;
                string fCol2 = null;
                object dataSource = null;
                switch (item)
                {
                    case ItemType.DTChiTiet:
                        fCol1 = "PATIENT_CLASSIFY_CODE";
                        fCol2 = "PATIENT_CLASSIFY_NAME";
                        dataSource = dataClassify;
                        break;
                    case ItemType.DonVi:
                        fCol1 = "WORK_PLACE_CODE";
                        fCol2 = "WORK_PLACE_NAME";
                        dataSource = dataWorkPlace;
                        break;
                    case ItemType.QuanHam:
                        fCol1 = "MILITARY_RANK_CODE";
                        fCol2 = "MILITARY_RANK_NAME";
                        dataSource = dataMilitaryRank;
                        break;
                    case ItemType.ChucVu:
                        fCol1 = "POSITION_CODE";
                        fCol2 = "POSITION_NAME";
                        dataSource = dataPosition;
                        break;
                    default:
                        break;
                }

                gridViewPopUp.BeginUpdate();
                gridViewPopUp.Columns.Clear();
                popupControlContainer1.Width = 300;
                popupControlContainer1.Height = 150;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = fCol1;
                col1.Caption = codeColumn;
                col1.Width = 100;
                col1.VisibleIndex = 1;
                gridViewPopUp.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = fCol2;
                col2.Caption = nameColumn;
                col2.Width = 200;
                col2.VisibleIndex = 2;
                gridViewPopUp.Columns.Add(col2);

                gridViewPopUp.GridControl.DataSource = dataSource;
                gridViewPopUp.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientClassify_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.DropDown)
            {
                isShowContainer = !isShowContainer;
                if (isShowContainer)
                {
                    ShowPopupNearTxt(txtPatientClassify, ItemType.DTChiTiet);
                }
            }
            else if (e.Button.Kind == ButtonPredefines.Delete)
            {
                txtPatientClassify.Text = "";
                currentPatientClassify = null;
                ValidateOtherCpn();
                if (dlgShowOtherPaySource != null)
                    dlgShowOtherPaySource(0);
            }
        }

        private void txtPatientClassify_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var dt = gridControlPopUp.DataSource as List<HIS_PATIENT_CLASSIFY>;
                    if (dt != null && dt.Count > 0)
                    {
                        currentPatientClassify = gridViewPopUp.GetFocusedRow() as HIS_PATIENT_CLASSIFY;
                        txtPatientClassify.Text = (gridViewPopUp.GetFocusedRow() as HIS_PATIENT_CLASSIFY).PATIENT_CLASSIFY_NAME;
                        popupControlContainer1.HidePopup();
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        ValidateOtherCpn();
                        FocusNextControlClosePopUp();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    RebuildControlContainer(ItemType.DTChiTiet);
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewPopUp.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }
                    Rectangle buttonBounds = new Rectangle(txtPatientClassify.Bounds.X, txtPatientClassify.Bounds.Y, txtPatientClassify.Bounds.Width, txtPatientClassify.Bounds.Height);
                    popupControlContainer1.ShowPopup(new Point(buttonBounds.X + 3, buttonBounds.Bottom + 170));
                    gridViewPopUp.Focus();
                    gridViewPopUp.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtPatientClassify.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateFilterGridView(DevExpress.XtraEditors.ButtonEdit txt, string fCol1, string fCol2)
        {
            try
            {
                if (!String.IsNullOrEmpty(txt.Text) && txt.IsEditorActive)
                {
                    txt.Refresh();
                    if (isShowContainerForChoose)
                    {
                        gridViewPopUp.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainer)
                        {
                            isShowContainer = true;
                        }

                        //Filter data
                        gridViewPopUp.ActiveFilterString = String.Format("{0} Like '%{1}%' OR {2} Like '%{1}%'", fCol1, txt.Text, fCol2);

                        gridViewPopUp.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewPopUp.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewPopUp.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewPopUp.FocusedRowHandle = 0;
                        gridViewPopUp.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewPopUp.OptionsFind.HighlightFindResults = true;
                        Rectangle buttonBounds = new Rectangle(txt.Bounds.X, txt.Bounds.Y, txt.Bounds.Width, txt.Bounds.Height);
                        if (isShow)
                        {
                            popupControlContainer1.ShowPopup(new Point(buttonBounds.X + 3, buttonBounds.Bottom + 170));
                            isShow = false;
                        }

                        txt.Focus();
                    }
                    isShowContainerForChoose = false;
                }
                else
                {
                    gridViewPopUp.ActiveFilter.Clear();
                    gridViewPopUp.FocusedRowHandle = 0;
                    gridViewPopUp.Focus();
                    txt.Focus();
                    if (!isShowContainer)
                    {
                        popupControlContainer1.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientClassify_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsLoadFromSearchTxtCode)
                    return;
                RebuildControlContainer(ItemType.DTChiTiet);
                CreateFilterGridView(txtPatientClassify, "PATIENT_CLASSIFY_CODE", "PATIENT_CLASSIFY_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPopUp_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                isShowContainer = false;
                isShowContainerForChoose = true;
                var dt = gridViewPopUp.GetFocusedRow();
                if (gridViewPopUp.GetFocusedRow().GetType() == typeof(HIS_PATIENT_CLASSIFY))
                {
                    HIS_PATIENT_CLASSIFY item = gridViewPopUp.GetFocusedRow() as HIS_PATIENT_CLASSIFY;
                    popupControlContainer1.HidePopup();
                    if (item != null)
                    {
                        currentPatientClassify = item;
                        txtPatientClassify.Text = item.PATIENT_CLASSIFY_NAME;
                        ValidateOtherCpn();
                        if (lciWorkPlaceNameNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            ShowPopupNearTxt(txtWorkPlace, ItemType.DonVi);
                        }
                        else if (lciMilitaryRankNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            ShowPopupNearTxt(txtMilitaryRank, ItemType.QuanHam);
                        }
                        else if (lciPositionNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            ShowPopupNearTxt(txtPosition, ItemType.ChucVu);
                        }
                        else if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                            this.dlgFocusToUCRelativeWhenPatientIsChild();
                        else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                            this.dlgFocusNextUserControl();
                    }
                }
                else if (gridViewPopUp.GetFocusedRow().GetType() == typeof(HIS_WORK_PLACE))
                {
                    HIS_WORK_PLACE item = gridViewPopUp.GetFocusedRow() as HIS_WORK_PLACE;
                    popupControlContainer1.HidePopup();
                    if (item != null)
                    {
                        currentWorkPlace = item;
                        txtWorkPlace.Text = item.WORK_PLACE_NAME;
                        if (lciMilitaryRankNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            ShowPopupNearTxt(txtMilitaryRank, ItemType.QuanHam);
                        }
                        else if (lciPositionNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            ShowPopupNearTxt(txtPosition, ItemType.ChucVu);
                        }
                        else if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                            this.dlgFocusToUCRelativeWhenPatientIsChild();
                        else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                            this.dlgFocusNextUserControl();
                    }
                }
                else if (gridViewPopUp.GetFocusedRow().GetType() == typeof(HIS_MILITARY_RANK))
                {
                    HIS_MILITARY_RANK item = gridViewPopUp.GetFocusedRow() as HIS_MILITARY_RANK;
                    popupControlContainer1.HidePopup();
                    if (item != null)
                    {
                        currentMilitaryRank = item;
                        txtMilitaryRank.Text = item.MILITARY_RANK_NAME;
                        ChangePatientClassify();
                        if (lciPositionNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            ShowPopupNearTxt(txtPosition, ItemType.ChucVu);
                        }
                        else if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                            this.dlgFocusToUCRelativeWhenPatientIsChild();
                        else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                            this.dlgFocusNextUserControl();
                    }
                }
                else if (gridViewPopUp.GetFocusedRow().GetType() == typeof(HIS_POSITION))
                {
                    HIS_POSITION item = gridViewPopUp.GetFocusedRow() as HIS_POSITION;
                    popupControlContainer1.HidePopup();
                    if (item != null)
                    {
                        currentPosition = item;
                        txtPosition.Text = item.POSITION_NAME;
                        if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                            this.dlgFocusToUCRelativeWhenPatientIsChild();
                        else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                            this.dlgFocusNextUserControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusNextControlClosePopUp()
        {
            try
            {
                if (lciWorkPlaceNameNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    ShowPopupNearTxt(txtWorkPlace, ItemType.DonVi);
                }
                else if (lciMilitaryRankNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    ShowPopupNearTxt(txtMilitaryRank, ItemType.QuanHam);
                }
                else if (lciPositionNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    ShowPopupNearTxt(txtPosition, ItemType.ChucVu);
                }
                else if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                    this.dlgFocusToUCRelativeWhenPatientIsChild();
                else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                    this.dlgFocusNextUserControl();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlPopUp_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridViewPopUp_RowClick(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainer1_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateOtherCpn()
        {
            try
            {
                lciWorkPlaceNameNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciMilitaryRankNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciPositionNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                bool isCA = false;
                if (this.currentPatientClassify != null)
                {

                    var patientClassify = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().FirstOrDefault(o => o.ID == this.currentPatientClassify.ID);
                    if (patientClassify != null)
                    {
                        if (dlgShowOtherPaySource != null)
                            dlgShowOtherPaySource(patientClassify.OTHER_PAY_SOURCE_ID ?? 0);
                    }
                    if (patientClassify != null && patientClassify.IS_POLICE == 1)
                    {
                        isCA = true;
                        if (currentNameControl != null && currentNameControl.Count > 0)
                        {
                            string bnW = baseNameControl + "." + layoutControl1.Name + ".Root." + lciWorkPlaceNameNew.Name;
                            string bnM = baseNameControl + "." + layoutControl1.Name + ".Root." + lciMilitaryRankNew.Name;
                            string bnP = baseNameControl + "." + layoutControl1.Name + ".Root." + lciPositionNew.Name;

                            if (!currentNameControl.Contains(bnW))
                            {
                                lciWorkPlaceNameNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                                ValidateNewControl(lciWorkPlaceNameNew, txtWorkPlace, true);
                            }

                            if (!currentNameControl.Contains(bnM))
                            {
                                lciMilitaryRankNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                                ValidateNewControl(lciMilitaryRankNew, txtMilitaryRank, true);
                            }

                            if (!currentNameControl.Contains(bnP))
                            {
                                lciPositionNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                                ValidateNewControl(lciPositionNew, txtPosition, true);
                            }
                        }
                        else
                        {
                            lciWorkPlaceNameNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            lciMilitaryRankNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            lciPositionNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            ValidateNewControl(lciMilitaryRankNew, txtMilitaryRank, true);
                            ValidateNewControl(lciPositionNew, txtPosition, true);
                            ValidateNewControl(lciWorkPlaceNameNew, txtWorkPlace, true);
                        }
                    }
                }

                if (dlgShowCheckWorkingLetter != null)
                    dlgShowCheckWorkingLetter(isCA);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtWorkPlace_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainer = !isShowContainer;
                    if (isShowContainer)
                    {
                        RebuildControlContainer(ItemType.DonVi);
                        Rectangle buttonBounds = new Rectangle(txtWorkPlace.Bounds.X, txtWorkPlace.Bounds.Y, txtWorkPlace.Bounds.Width, txtWorkPlace.Bounds.Height);
                        popupControlContainer1.ShowPopup(new Point(buttonBounds.X + 3, buttonBounds.Bottom + 170));
                    }
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtWorkPlace.Text = "";
                    currentWorkPlace = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisWorkPlace").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisWorkPlace'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.HisWorkPlace' is not plugins");

                    List<object> listArgs = new List<object>();
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();

                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>();
                    this.LoadWorkPlace();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtWorkPlace_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsLoadFromSearchTxtCode)
                    return;
                RebuildControlContainer(ItemType.DonVi);
                CreateFilterGridView(txtWorkPlace, "WORK_PLACE_CODE", "WORK_PLACE_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtWorkPlace_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var dt = gridControlPopUp.DataSource as List<HIS_WORK_PLACE>;
                    if (dt != null && dt.Count > 0)
                    {
                        currentWorkPlace = gridViewPopUp.GetFocusedRow() as HIS_WORK_PLACE;
                        txtWorkPlace.Text = (gridViewPopUp.GetFocusedRow() as HIS_WORK_PLACE).WORK_PLACE_NAME;
                        popupControlContainer1.HidePopup();
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        if (lciMilitaryRankNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            ShowPopupNearTxt(txtMilitaryRank, ItemType.QuanHam);
                        }
                        else if (lciPositionNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            ShowPopupNearTxt(txtPosition, ItemType.ChucVu);
                        }
                        else if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                            this.dlgFocusToUCRelativeWhenPatientIsChild();
                        else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                            this.dlgFocusNextUserControl();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    RebuildControlContainer(ItemType.DonVi);
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewPopUp.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtWorkPlace.Bounds.X, txtWorkPlace.Bounds.Y, txtWorkPlace.Bounds.Width, txtWorkPlace.Bounds.Height);
                    popupControlContainer1.ShowPopup(new Point(buttonBounds.X + 3, buttonBounds.Bottom + 170));
                    gridViewPopUp.Focus();
                    gridViewPopUp.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtWorkPlace.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMilitaryRank_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainer = !isShowContainer;
                    if (isShowContainer)
                    {
                        ShowPopupNearTxt(txtMilitaryRank, ItemType.QuanHam);
                    }
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtMilitaryRank.Text = "";
                    currentMilitaryRank = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMilitaryRank_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var dt = gridControlPopUp.DataSource as List<HIS_MILITARY_RANK>;
                    if (dt != null && dt.Count > 0)
                    {
                        currentMilitaryRank = gridViewPopUp.GetFocusedRow() as HIS_MILITARY_RANK;
                        txtMilitaryRank.Text = (gridViewPopUp.GetFocusedRow() as HIS_MILITARY_RANK).MILITARY_RANK_NAME;
                        popupControlContainer1.HidePopup();
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        if (lciPositionNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            ShowPopupNearTxt(txtPosition, ItemType.ChucVu);
                        }
                        else if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                            this.dlgFocusToUCRelativeWhenPatientIsChild();
                        else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                            this.dlgFocusNextUserControl();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    RebuildControlContainer(ItemType.QuanHam);
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewPopUp.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtMilitaryRank.Bounds.X, txtMilitaryRank.Bounds.Y, txtMilitaryRank.Bounds.Width, txtMilitaryRank.Bounds.Height);
                    popupControlContainer1.ShowPopup(new Point(buttonBounds.X + 3, buttonBounds.Bottom + 170));
                    gridViewPopUp.Focus();
                    gridViewPopUp.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtMilitaryRank.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMilitaryRank_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsLoadFromSearchTxtCode)
                    return;
                RebuildControlContainer(ItemType.QuanHam);
                CreateFilterGridView(txtMilitaryRank, "MILITARY_RANK_CODE", "MILITARY_RANK_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPosition_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainer = !isShowContainer;
                    if (isShowContainer)
                    {
                        ShowPopupNearTxt(txtPosition, ItemType.ChucVu);
                    }
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtPosition.Text = "";
                    currentPosition = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPosition_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var dt = gridControlPopUp.DataSource as List<HIS_POSITION>;
                    if (dt != null && dt.Count > 0)
                    {
                        currentPosition = gridViewPopUp.GetFocusedRow() as HIS_POSITION;
                        txtPosition.Text = (gridViewPopUp.GetFocusedRow() as HIS_POSITION).POSITION_NAME;
                        popupControlContainer1.HidePopup();
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        if (this.isGKS == true && this.dlgFocusToUCRelativeWhenPatientIsChild != null)
                            this.dlgFocusToUCRelativeWhenPatientIsChild();
                        else if (this.isGKS == false && this.dlgFocusNextUserControl != null)
                            this.dlgFocusNextUserControl();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    RebuildControlContainer(ItemType.ChucVu);
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewPopUp.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtPosition.Bounds.X, txtPosition.Bounds.Y, txtPosition.Bounds.Width, txtPosition.Bounds.Height);
                    popupControlContainer1.ShowPopup(new Point(buttonBounds.X + 3, buttonBounds.Bottom + 170));
                    gridViewPopUp.Focus();
                    gridViewPopUp.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtPosition.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPosition_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsLoadFromSearchTxtCode)
                    return;
                RebuildControlContainer(ItemType.ChucVu);
                CreateFilterGridView(txtPosition, "POSITION_CODE", "POSITION_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientClassify_Leave(object sender, EventArgs e)
        {
            try
            {
                if (currentPatientClassify != null)
                    txtPatientClassify.Text = currentPatientClassify.PATIENT_CLASSIFY_NAME;
                else
                    txtPatientClassify.Text = "";
                if (dlgPatientClassifyId != null)
                    this.dlgPatientClassifyId(currentPatientClassify != null ? (long?)currentPatientClassify.ID : null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtWorkPlace_Leave(object sender, EventArgs e)
        {
            try
            {
                if (currentWorkPlace != null)
                    txtWorkPlace.Text = currentWorkPlace.WORK_PLACE_NAME;
                else
                    txtWorkPlace.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMilitaryRank_Leave(object sender, EventArgs e)
        {
            try
            {
                if (currentMilitaryRank != null)
                    txtMilitaryRank.Text = currentMilitaryRank.MILITARY_RANK_NAME;
                else
                    txtMilitaryRank.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPosition_Leave(object sender, EventArgs e)
        {
            try
            {
                if (currentPosition != null)
                    txtPosition.Text = currentPosition.POSITION_NAME;
                else
                    txtPosition.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ChangePatientClassify(bool IsBhyt = false)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Dữ liệu Đối tượng chi tiết: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataClassify), dataClassify));
                var NewdataClassify = dataClassify.Where(o => (!string.IsNullOrEmpty(o.BHYT_WHITELIST_IDS) || !string.IsNullOrEmpty(o.MILITARY_RANK_IDS))
                                                            && (string.IsNullOrEmpty(o.BHYT_WHITELIST_IDS) || (BhytWhiteListtId > 0 && !string.IsNullOrEmpty(o.BHYT_WHITELIST_IDS) && o.BHYT_WHITELIST_IDS.Split(',').ToList().Contains(BhytWhiteListtId.ToString())))
                                                            && (string.IsNullOrEmpty(o.MILITARY_RANK_IDS) || (currentMilitaryRank != null && !string.IsNullOrEmpty(o.MILITARY_RANK_IDS) && o.MILITARY_RANK_IDS.Split(',').ToList().Contains(currentMilitaryRank.ID.ToString())))).ToList();

                Inventec.Common.Logging.LogSystem.Debug("Dữ liệu Đối tượng chi tiết khi thay đổi theo mã BHYT và quân hàm: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => NewdataClassify), NewdataClassify));
                if (NewdataClassify != null && NewdataClassify.Count > 0 && NewdataClassify[0] != currentPatientClassify)
                {
                    IsLoadFromSearchTxtCode = true;
                    currentPatientClassify = NewdataClassify[0];
                    txtPatientClassify.Text = NewdataClassify[0].PATIENT_CLASSIFY_NAME;
                    if (IsBhyt)
                        ValidateOtherCpn();
                    popupControlContainer1.HidePopup();
                    IsLoadFromSearchTxtCode = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCareer_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (this.cboCareer.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_CAREER career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboCareer.EditValue ?? "0").ToString()));
                    if (career != null)
                    {
                        this.txtCareerCode.Text = career.CAREER_CODE;
                    }
                    else
                    {
                        cboCareer.EditValue = null;
                        this.txtCareerCode.Text = null;
                    }

                }
                else
                {
                    this.txtCareerCode.Text = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
