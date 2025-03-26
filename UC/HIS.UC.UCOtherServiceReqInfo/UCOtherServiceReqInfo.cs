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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.PopupLoader;
using HIS.UC.UCOtherServiceReqInfo.ADO;
using HIS.UC.UCOtherServiceReqInfo.Properties;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using MOS.EFMODEL.DataModels;
using HIS.UC.UCOtherServiceReqInfo.Valid;
using HIS.UC.UCOtherServiceReqInfo.Resources;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;
using HIS.UC.UCOtherServiceReqInfo.Config;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Controls.ValidationRule;
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.Library.RegisterConfig;

namespace HIS.UC.UCOtherServiceReqInfo
{
    public partial class UCOtherServiceReqInfo : UserControlBase
    {
        Action<object> dlgFocusNextUserControl;
        Action<bool> dlgHeinRightRouteType;
        Action<long?> dlgPriorityNumberChanged;

        internal HIS_TREATMENT _HisTreatment = null;
        HIS_PATIENT_TYPE workingPatientType;

        bool _IsUserBranchTime = false;
        List<HIS_BRANCH_TIME> _BranchTimes = null;

        bool _IsAutoSetOweType = false;

        bool hasDataAutoCheckPriority = false;
        bool IsChangeFromClassify = false;
        List<HIS_OTHER_PAY_SOURCE> dataOtherPayTemp = null;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY> dataClassify = null;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.UC.UCOtherServiceReqInfo";

        public string HospitalizeReasonCode { get; private set; }
        public string HospitalizeReasonName { get; private set; }
        #region Constructor - Load

        public UCOtherServiceReqInfo()
            : base("HIS.Desktop.Plugins.RegisterV2", "UCOtherServiceReqInfo")
        {
            Inventec.Common.Logging.LogSystem.Debug("UCOtherServiceReqInfo .1");
            InitializeComponent();
            try
            {
                HisConfig.LoadConfig();
                Inventec.Common.Logging.LogSystem.Debug("UCOtherServiceReqInfo .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCOtherServiceReqInfo_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCOtherServiceReqInfo_Load .1");
                this._HisTreatment = new HIS_TREATMENT();
                this.txtIntructionTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                this.dtIntructionTime.EditValue = DateTime.Now;
                this.txtSTTPriority.EditValue = null;
                HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>();
                this._IsAutoSetOweType = ((HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Register.IsAutoSetOweTypeInCaseOfUsingFund")).Trim() == "1");
                InitControlState();
                this.LoadBranch();

                this.SetCaptionByLanguageKeyNew();
                this.ValidateIntructionTime();
                this.ValidateFrmFun();
                this.ValidateTreatmentType();
                this.ValidateNumOrderPriority();
                this.ValidateMaxlength(txtGuaranteeReason, 500);
                this.ValidateMaxlength(txtNote, 1000);
                Inventec.Common.Logging.LogSystem.Debug("UCOtherServiceReqInfo_Load .2");
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
                Inventec.Common.Logging.LogSystem.Debug("UCOtherServiceReqInfo.InitFieldFromAsync .1");

                await this.LoadEmergencyWtimes();
                await this.LoadPriorityType();
                await this.LoadTreatmentTypes();
                await this.LoadOweTypes();
                await this.LoadFunds();
                await this.LoadPatientClassify();
                await this.LoadGuarantee();
                this.LoadOtherPaySource();
                this.InitComboHisHospitalizeReason();
                this.SetHeinRighRouteTypeByTime();
                Inventec.Common.Logging.LogSystem.Debug("UCOtherServiceReqInfo.InitFieldFromAsync .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }           
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCOtherServiceReqInfo
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.UCOtherServiceReqInfo.Resources.Lang", typeof(UCOtherServiceReqInfo).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lcUCOtherServiceReqInfo.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lcUCOtherServiceReqInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkTuberculosis.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.chkTuberculosis.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGuaranteeUsername.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.cboGuaranteeUsername.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientClassify.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.cboPatientClassify.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboOtherPaySource.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.cboOtherPaySource.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCapMaMS.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.chkCapMaMS.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPriorityType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.cboPriorityType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPriorityType.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.cboPriorityType.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //toolTipItem2.Text = Inventec.Common.Resource.Get.Value("toolTipItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAddCTT.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.btnAddCTT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCTT.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.cboCTT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsChronic.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.chkIsChronic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboOweType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.cboOweType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.cboTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsNotRequireFee.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.chkIsNotRequireFee.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPriority.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.chkPriority.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkEmergency.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.chkEmergency.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEmergencyTime.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.cboEmergencyTime.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcgOtherRequest.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lcgOtherRequest.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciTreatmentType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentType.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciTreatmentType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsNotRequireFee.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciIsNotRequireFee.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsNotRequireFee.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciIsNotRequireFee.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOweType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciOweType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOweType.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciOweType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEmergency.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciEmergency.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIntructionTime.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciIntructionTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFortxtMaMS.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciFortxtMaMS.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForchkCapMaMS.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciForchkCapMaMS.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPriority.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciPriority.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.layoutControlItem1.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFortxtIncode.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciFortxtIncode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientClassify.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciPatientClassify.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientClassify.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciPatientClassify.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTuberculosis.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciTuberculosis.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTuberculosis.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciTuberculosis.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEmergencyTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciEmergencyTime.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEmergencyTime.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciEmergencyTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboCTT.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciCboCTT.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboCTT.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciCboCTT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.layoutControlItem3.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGuaranteeLoginname.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciGuaranteeLoginname.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGuaranteeLoginname.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciGuaranteeLoginname.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsChronic.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciIsChronic.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFortxtSTTPriority.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciFortxtSTTPriority.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFortxtSTTPriority.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciFortxtSTTPriority.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentOrder.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciTreatmentOrder.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentOrder.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciTreatmentOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGuaranteeReason.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciGuaranteeReason.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGuaranteeReason.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciGuaranteeReason.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateMaxlength(BaseEdit control, int maxlenght)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxlenght;
                validate.IsRequired = false;
                validate.ErrorText = string.Format("Nhập quá kí tự cho phép ({0})", maxlenght);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationUCOtherReqInfo.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private async Task FillDataDefaultToControlAsync()
        //{
        //    try
        //    {
        //        Inventec.Common.Logging.LogSystem.Debug("UCOtherServiceReqInfo.FillDataDefaultToControl. 1");

        //        LoadEmergencyWtimes();

        //        LoadTreatmentTypes();

        //        LoadOweTypes();

        //        LoadFunds();

        //        Inventec.Common.Logging.LogSystem.Debug("UCOtherServiceReqInfo.FillDataDefaultToControl. 2");
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private async Task LoadPriorityType()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE> dataPriorityTypes = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE>())
                {
                    dataPriorityTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataPriorityTypes = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE>>("api/HisPriorityType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataPriorityTypes != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE), dataPriorityTypes, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                this.InitComboCommon(this.cboPriorityType, dataPriorityTypes, "ID", "PRIORITY_TYPE_NAME", "PRIORITY_TYPE_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// 2. Sửa chức năng "Tiếp đón (2)":
        ///Khi tiếp đón bệnh nhân, thực hiện truy vấn xem có "bản ghi" "đối tượng ưu tiên" nào thỏa mãn không. Nếu có, mặc định điền "TH ưu tiên" theo bản ghi đầu tiên thỏa mãn, đồng thời tự động check vào checkbox "Ưu tiên"
        ///3. Sửa chức năng "Tiếp đón":
        ///Khi tiếp đón bệnh nhân, thực hiện truy vấn xem có "bản ghi" "đối tượng ưu tiên" nào thỏa mãn không. Nếu có, tự động check vào checkbox "Ưu tiên"
        ///Lưu ý: Thuật toán xử lý "truy vấn bản ghi đối tượng ưu tiên thỏa mãn" như sau:
        ///Lấy theo điều kiện sau:
        ///(AGE_FROM NULL OR AGE_FROM >= X)
        ///AND (AGE_TO NULL OR AGE_TO <= X)
        ///AND (BHYT_PREFIXS NULL OR Y START_IN (BHYT_PREFIXS))
        ///AND (AGE_FROM NOT NULL OR AGE_TO NOT NULL OR BHYT_PREFIXS NOT NULL)
        ///Trong đó:
        ///+ X: là tuổi của BN
        ///+ Y: là thẻ BHYT của BN
        ///+ START_IN: là hàm xử lý duyệt tất cả các đầu thẻ được khai báo (tách chuỗi BHYT_PREFIXS bởi dấu phẩy), và thực hiện kiểm tra với từng đầu thẻ, nếu Y có bắt đầu bằng đầu thẻ đấy thì trả về TRUE.
        /// </summary>
        /// <param name="dataPriorityTypes"></param>
        public void AutoCheckPriorityByPriorityType(long patientDob, string heinCardNumber)
        {
            long patientAge = 0;
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE> dataValidPriorityTypes = null;
                List<MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE> dataPriorityTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE>().Where(o => o.IS_FOR_EXAM_SUBCLINICAL == 1).ToList();
                if (dataPriorityTypes != null && dataPriorityTypes.Count > 0 && (patientDob > 0 || !String.IsNullOrEmpty(heinCardNumber)))
                {
                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDob).Value;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    DateTime newDate = new DateTime(tongsogiay);
                    int nam = newDate.Year - 1;
                    patientAge = nam == 0 ? 1 : nam;

                    dataValidPriorityTypes = dataPriorityTypes.Where(o =>
                        (o.AGE_FROM == null || o.AGE_FROM <= patientAge)
                        && (o.AGE_TO == null || o.AGE_TO >= patientAge)
                        && (String.IsNullOrEmpty(o.BHYT_PREFIXS) || (!String.IsNullOrEmpty(o.BHYT_PREFIXS) && StartIn(o.BHYT_PREFIXS, heinCardNumber)))
                        && ((o.AGE_FROM.HasValue && o.AGE_FROM > 0) || (o.AGE_TO.HasValue && o.AGE_TO > 0) || (!String.IsNullOrEmpty(o.BHYT_PREFIXS)))
                        ).ToList();
                    this.hasDataAutoCheckPriority = dataValidPriorityTypes != null && dataValidPriorityTypes.Count > 0;
                    chkPriority.Checked = this.hasDataAutoCheckPriority;
                    if (this.hasDataAutoCheckPriority && dataValidPriorityTypes != null && dataValidPriorityTypes.Count > 0)
                    {
                        cboPriorityType.EditValue = dataValidPriorityTypes.FirstOrDefault().ID;
                        lciPriority.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                        ValidatePriorityType();
                    }
                    else
                    {
                        lciPriority.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                        cboPriorityType.EditValue = null;
                        dxValidationUCOtherReqInfo.SetValidationRule(this.chkPriority, null);
                        dxValidationUCOtherReqInfo.SetValidationRule(this.cboPriorityType, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ChangePatientType(long patientTypeId)
        {
            try
            {
                if (IsChangeFromClassify)
                    return;
                string option = ConfigApplicationWorker.Get<string>("CONFIG_KEY__DEFAULT_CONFIG_IS_NOT_REQUIRE_FEE");
                Inventec.Common.Logging.LogSystem.Debug("CONFIG_KEY__DEFAULT_CONFIG_IS_NOT_REQUIRE_FEE" + option);
                Inventec.Common.Logging.LogSystem.Debug("ChangePatientType patientTypeId" + patientTypeId);

                if (patientTypeId > 0)
                {
                    this.workingPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.ID == patientTypeId).FirstOrDefault();
                    if (this.workingPatientType != null)
                    {
                        if (!String.IsNullOrEmpty(option))
                        {
                            var lstKey = option.Split(',').ToList();
                            //var checkExist = patientType.Where(o => lstKey.Contains(o.PATIENT_TYPE_CODE)).ToList();
                            this.chkIsNotRequireFee.Checked = (lstKey != null && lstKey.Count > 0 && lstKey.Contains(this.workingPatientType.PATIENT_TYPE_CODE));
                        }

                        var dataOtherPaySources = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>();
                        if (!String.IsNullOrEmpty(this.workingPatientType.OTHER_PAY_SOURCE_IDS))
                        {
                            dataOtherPaySources = dataOtherPaySources != null ? dataOtherPaySources.Where(o => o.IS_ACTIVE == 1 && ("," + this.workingPatientType.OTHER_PAY_SOURCE_IDS + ",").Contains("," + o.ID + ",")).ToList() : null;
                            this.InitComboCommon(this.cboOtherPaySource, dataOtherPaySources, "ID", "OTHER_PAY_SOURCE_NAME", "OTHER_PAY_SOURCE_CODE");
                            dataOtherPayTemp = dataOtherPaySources;
                            if (dataOtherPaySources != null && dataOtherPaySources.Count == 1)
                            {
                                this.cboOtherPaySource.EditValue = dataOtherPaySources[0].ID;
                            }
                            else
                            {
                                this.cboOtherPaySource.EditValue = null;
                            }
                        }
                        else
                        {
                            dataOtherPaySources = dataOtherPaySources != null ? dataOtherPaySources.Where(o => o.IS_ACTIVE == 1).ToList() : null;
                            this.InitComboCommon(this.cboOtherPaySource, dataOtherPaySources, "ID", "OTHER_PAY_SOURCE_NAME", "OTHER_PAY_SOURCE_CODE");
                            dataOtherPayTemp = dataOtherPaySources;
                            this.cboOtherPaySource.EditValue = null;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool StartIn(string BHYT_PREFIXS, string heincardnumber)
        {
            bool valid = false;
            try
            {
                List<string> checkData = null;
                if (!String.IsNullOrEmpty(BHYT_PREFIXS) && !String.IsNullOrEmpty(heincardnumber))
                {
                    var arrPrefixs = BHYT_PREFIXS.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrPrefixs != null && arrPrefixs.Count() > 0)
                    {
                        checkData = arrPrefixs.ToList().Where(o => heincardnumber.StartsWith(o)).ToList();
                        valid = (checkData != null && checkData.Count > 0) ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private async Task LoadEmergencyWtimes()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME> dataEmergencyWtimes = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>())
                {
                    dataEmergencyWtimes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataEmergencyWtimes = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>>("api/HisEmergencyWtime/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataEmergencyWtimes != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME), dataEmergencyWtimes, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (dataEmergencyWtimes != null && dataEmergencyWtimes.Count > 0)
                {
                    dataEmergencyWtimes = dataEmergencyWtimes.Where(p => p.IS_ACTIVE == 1).ToList();
                }

                this.InitComboCommon(this.cboEmergencyTime, dataEmergencyWtimes, "ID", "EMERGENCY_WTIME_NAME", "EMERGENCY_WTIME_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadTreatmentTypes()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> dataTreatmentTypes = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>())
                {
                    dataTreatmentTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataTreatmentTypes = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>>("api/HisTreatmentType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataTreatmentTypes != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE), dataTreatmentTypes, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (dataTreatmentTypes != null && dataTreatmentTypes.Count > 0)
                {
                    dataTreatmentTypes = dataTreatmentTypes.Where(p => p.IS_ACTIVE == 1 && p.IS_ALLOW_RECEPTION == 1).ToList();
                }

                this.InitComboCommon(this.cboTreatmentType, dataTreatmentTypes, "ID", "TREATMENT_TYPE_NAME", 70, "TREATMENT_TYPE_CODE", 30);

                this.cboTreatmentType.EditValue = dataTreatmentTypes != null && dataTreatmentTypes.Count > 0 && dataTreatmentTypes.FirstOrDefault(p => p.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM) != null ? IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadOweTypes()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_OWE_TYPE> dataOweTypes = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>())
                {
                    dataOweTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataOweTypes = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>>("api/HisOweType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataOweTypes != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_OWE_TYPE), dataOweTypes, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                if (dataOweTypes != null && dataOweTypes.Count > 0)
                {
                    dataOweTypes = dataOweTypes.Where(p => p.IS_ACTIVE == 1).ToList();
                }

                this.InitComboCommon(this.cboOweType, dataOweTypes, "ID", "OWE_TYPE_NAME", "OWE_TYPE_CODE");

                if (!String.IsNullOrEmpty(HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.OweTypeDefault) && dataOweTypes != null && dataOweTypes.Count > 0)
                {
                    var oweType = dataOweTypes.FirstOrDefault(o => o.OWE_TYPE_CODE == HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.OweTypeDefault);
                    if (oweType == null) throw new ArgumentNullException("Khong tim thay HIS_OWE_TYPE theo OWE_TYPE_CODE = " + HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.OweTypeDefault);
                    cboOweType.EditValue = oweType.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadFunds()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_FUND> dataFunds = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_FUND>())
                {
                    dataFunds = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_FUND>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataFunds = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_FUND>>("api/HisFund/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataFunds != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_FUND), dataFunds, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (dataFunds != null && dataFunds.Count > 0)
                {
                    dataFunds = dataFunds.Where(o => o.IS_ACTIVE == 1).ToList();
                }
                this.InitComboCommon(this.cboCTT, dataFunds, "ID", "FUND_NAME", "FUND_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadPatientClassify()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>())
                {
                    dataClassify = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataClassify = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>>("api/HisPatientClassify/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataClassify != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY), dataClassify, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (dataClassify != null && dataClassify.Count > 0)
                {
                    dataClassify = dataClassify.Where(o => o.IS_ACTIVE == 1).ToList();
                }

                this.InitComboCommon(this.cboPatientClassify, dataClassify, "ID", "PATIENT_CLASSIFY_NAME", "PATIENT_CLASSIFY_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadGuarantee()
        {
            try
            {
                List<ACS_USER> dataUser = null;
                if (BackendDataWorker.IsExistsKey<ACS_USER>())
                {
                    dataUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataUser = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS_USER>>("api/AcsUser/Get", ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (dataUser != null) BackendDataWorker.UpdateToRam(typeof(ACS_USER), dataUser, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (dataUser != null && dataUser.Count > 0)
                {
                    dataUser = dataUser.Where(o => o.IS_ACTIVE == 1).ToList();
                }

                this.InitComboCommon(this.cboGuaranteeUsername, dataUser, "LOGINNAME", "USERNAME", "LOGINNAME");
                cboGuaranteeUsername.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadOtherPaySource()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE> dataOtherPaySources = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>();
                dataOtherPaySources = dataOtherPaySources != null ? dataOtherPaySources.Where(o => o.IS_ACTIVE == 1).ToList() : null;
                this.InitComboCommon(this.cboOtherPaySource, dataOtherPaySources, "ID", "OTHER_PAY_SOURCE_NAME", "OTHER_PAY_SOURCE_CODE");
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
                //ResourceMessage.languageMessage = new ResourceManager("HIS.UC.UCOtherServiceReqInfo.Resources.Lang", typeof(HIS.UC.UCOtherServiceReqInfo.UCOtherServiceReqInfo).Assembly);
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo = new ResourceManager("HIS.UC.UCOtherServiceReqInfo.Resources.Lang", typeof(HIS.UC.UCOtherServiceReqInfo.UCOtherServiceReqInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lcUCOtherServiceReqInfo.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.layoutControl1.Text", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.chkIsChronic.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.chkIsChronic.Properties.Caption", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.cboOweType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.cboOweType.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.cboTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.cboTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.chkIsNotRequireFee.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.chkIsNotRequireFee.Properties.Caption", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.chkPriority.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.chkPriority.Properties.Caption", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.chkEmergency.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.chkEmergency.Properties.Caption", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.cboEmergencyTime.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.cboEmergencyTime.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.lcgOtherRequest.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lcgOtherRequest.Text", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.lciTreatmentType.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciTreatmentType.Text", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.lciPriority.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciPriority.Text", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.lciIsNotRequireFee.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciIsNotRequireFee.Text", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.lciIsChronic.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciIsChronic.Text", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.lciEmergencyTime.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciEmergencyTime.Text", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.lciOweType.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciOweType.Text", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.lciEmergency.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciEmergency.Text", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
                this.lciIntructionTime.Text = Inventec.Common.Resource.Get.Value("UCOtherServiceReqInfo.lciIntructionTime.Text", Resources.ResourceLanguageManager.ResourceUCOtherServiceReqInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event Control

        private void cboTreatmentType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    FocusTochkPriority();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPriority_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusToPriorityType();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEmergency_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEmergency.Checked)
                    lciEmergencyTime.Enabled = true;
                else
                    lciEmergencyTime.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEmergency_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusTochkIsNotRequireFee();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsNotRequireFee_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusTochkIsChronic();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsChronic_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusToEmergencyTime();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIntructionTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateTimeStringToSystemTime(this.txtIntructionTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtIntructionTime.EditValue = dt;
                        this.dtIntructionTime.Update();
                    }

                    this.dtIntructionTime.Visible = true;
                    this.dtIntructionTime.ShowPopup();
                    //this.dtIntructionTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIntructionTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    this.dtIntructionTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIntructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTreatmentType.Focus();
                    this.cboTreatmentType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtIntructionTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtIntructionTime.Update();
                    this.dtIntructionTime.Visible = false;
                    this.txtIntructionTime.Text = this.dtIntructionTime.DateTime.ToString("dd/MM/yyyy HH:mm");

                    this.cboTreatmentType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtIntructionTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtIntructionTime.Visible = true;
                    this.dtIntructionTime.Update();
                    this.dtIntructionTime.Text = DateTime.Now.ToString("dd/MM/yyyy");

                    System.Threading.Thread.Sleep(100);
                    this.chkIsNotRequireFee.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtIntructionTime_KeyPress(object sender, KeyPressEventArgs e)
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

        private void cboEmergencyTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                FocusToOweType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboOweType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                this.cboCTT.Focus();
                this.cboCTT.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIntructionTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.SetHeinRighRouteTypeByTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetHeinRighRouteTypeByTime()
        {
            try
            {
                if (string.IsNullOrEmpty(this.txtIntructionTime.Text))
                {
                    return;
                }
                else
                {
                    try
                    {
                        DateTime.ParseExact(this.txtIntructionTime.Text, "dd/MM/yyyy HH:mm", null);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        return;
                    }
                }
                bool IsCapCuuByBranchTime = true;
                if (this._IsUserBranchTime)
                {
                    if (this._BranchTimes != null && this._BranchTimes.Count > 0)
                    {
                        DateTime? dt = DateTimeHelper.ConvertDateTimeStringToSystemTime(this.txtIntructionTime.Text);
                        if (dt != null && dt.Value != DateTime.MinValue)
                        {
                            this.dtIntructionTime.EditValue = dt;
                            this.dtIntructionTime.Update();

                            if (dtIntructionTime.EditValue != null)
                            {
                                int day = (int)dtIntructionTime.DateTime.DayOfWeek;
                                var timeOfDay = dtIntructionTime.DateTime.ToString("HHmmss");
                                var dataTimes = this._BranchTimes.Where(p => p.DAY == (day + 1)).ToList();
                                if (dataTimes != null && dataTimes.Count > 0)
                                {
                                    foreach (var item in dataTimes)
                                    {
                                        long timeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(item.FROM_TIME);
                                        long timeTo = Inventec.Common.TypeConvert.Parse.ToInt64(item.TO_TIME);
                                        if (timeFrom <= Inventec.Common.TypeConvert.Parse.ToInt64(timeOfDay)
                                            && Inventec.Common.TypeConvert.Parse.ToInt64(timeOfDay) <= timeTo)
                                        {
                                            IsCapCuuByBranchTime = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    chkEmergency.Checked = IsCapCuuByBranchTime;
                    if (this.dlgHeinRightRouteType != null)
                        this.dlgHeinRightRouteType(IsCapCuuByBranchTime);
                }
                else
                    chkEmergency.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FillDataOweTypeDefault()
        {
            try
            {
                var oweTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>();
                if (!String.IsNullOrEmpty(HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.OweTypeDefault) && oweTypes != null && oweTypes.Count > 0)
                {
                    var oweType = oweTypes.SingleOrDefault(o => o.OWE_TYPE_CODE == HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.OweTypeDefault);
                    if (oweType == null) throw new ArgumentNullException("Khong tim thay HIS_OWE_TYPE theo OWE_TYPE_CODE = " + HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.OweTypeDefault);
                    cboOweType.EditValue = oweType.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReloadValidation(bool _isValid)
        {
            try
            {
                if (_isValid)
                {
                    ValidHrmKskCode();
                }
                else
                {
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationUCOtherReqInfo, this.dxErrorProviderControl);
                    this.lciGuaranteeLoginname.AppearanceItemCaption.ForeColor = Color.Black;
                    this.dxValidationUCOtherReqInfo.SetValidationRule(txtGuaranteeLoginname, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void RefreshData()
        {
            try
            {
                txtGuaranteeLoginname.Text = "";
                cboGuaranteeUsername.EditValue = null;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationUCOtherReqInfo, this.dxErrorProviderControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidHrmKskCode()
        {
            try
            {

                lciGuaranteeLoginname.AppearanceItemCaption.ForeColor = Color.Maroon;
                Combo___ValidationRule oDateRule = new Combo___ValidationRule();
                oDateRule.txt = txtGuaranteeLoginname;
                oDateRule.cbo = cboGuaranteeUsername;
                oDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationUCOtherReqInfo.SetValidationRule(txtGuaranteeLoginname, oDateRule);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
        public void CheckExamOnline(bool isChecked)
        {
            chkExamOnline.Checked = isChecked;
        }
        public void ShowOrtherPay(long payId)
        {
            try
            {
                if (payId > 0)
                {
                    LoadOtherPaySource();
                    cboOtherPaySource.EditValue = payId;
                    cboOtherPaySource.Enabled = false;
                    IsChangeFromClassify = true;
                }
                else
                {
                    IsChangeFromClassify = false;
                    this.InitComboCommon(this.cboOtherPaySource, dataOtherPayTemp, "ID", "OTHER_PAY_SOURCE_NAME", "OTHER_PAY_SOURCE_CODE");
                    cboOtherPaySource.EditValue = null;
                    cboOtherPaySource.Enabled = true;

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAddCTT_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.cboCTT.EditValue != null)
                {
                    if (this._IsAutoSetOweType)
                    {
                        var dataCheck = BackendDataWorker.Get<HIS_OWE_TYPE>().FirstOrDefault(p => p.IS_ACTIVE == 1 && p.ID == IMSys.DbConfig.HIS_RS.HIS_OWE_TYPE.ID__INSURANCE);
                        if (dataCheck != null)
                            cboOweType.EditValue = IMSys.DbConfig.HIS_RS.HIS_OWE_TYPE.ID__INSURANCE;
                    }

                    //Call module add info
                    HIS.UC.UCOtherServiceReqInfo.FUN.frmFun frmFun = new FUN.frmFun(this._HisTreatment);
                    frmFun.MyGetData = new FUN.frmFun.GetString(GetInfoHisFun);
                    frmFun.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void GetInfoHisFun(HIS_TREATMENT _hisTreatment)
        {
            try
            {
                //Gan data
                _HisTreatment = _hisTreatment;
                if (this.dlgFocusNextUserControl != null)
                    dlgFocusNextUserControl(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCTT_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCTT.EditValue != null)
                    {
                        this.btnAddCTT.Enabled = true;
                        btnAddCTT_Click(null, null);
                        cboCTT.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        this.btnAddCTT.Enabled = false;
                        if (this.dlgFocusNextUserControl != null)
                            dlgFocusNextUserControl(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCTT_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCTT.EditValue = null;
                    cboCTT.Properties.Buttons[1].Visible = false;
                    this._HisTreatment = new HIS_TREATMENT();
                    this._HisTreatment.FUND_CUSTOMER_NAME = this._PatientName;

                    if (this._IsAutoSetOweType && cboOweType.EditValue != null && ((long)cboOweType.EditValue == IMSys.DbConfig.HIS_RS.HIS_OWE_TYPE.ID__INSURANCE))
                        cboOweType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSTTPriority_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.dlgPriorityNumberChanged != null)
                {
                    this.dlgPriorityNumberChanged(txtSTTPriority.EditValue != null ? (long?)txtSTTPriority.Value : null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!(Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar)))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPriorityType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (lciFortxtIncode.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        FocusToIncode();
                    else if (cboEmergencyTime.Enabled)
                        FocusToEmergencyTime();
                    else
                        FocusToOweType();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriorityType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Clear || e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPriorityType.EditValue = null;
                    cboPriorityType.Properties.Buttons[1].Visible = false;
                    chkPriority.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPriorityType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboPriorityType.Properties.Buttons[1].Visible = (cboPriorityType.EditValue != null);
                if (cboPriorityType.EditValue != null)
                {
                    chkPriority.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriorityType_KeyUp(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        if (cboEmergencyTime.Enabled)
            //            FocusToEmergencyTime();
            //        else
            //            FocusToOweType();
            //        e.Handled = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void cboPriorityType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cboPriorityType_KeyDown(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        if (cboEmergencyTime.Enabled)
            //            FocusToEmergencyTime();
            //        else
            //            FocusToOweType();
            //        //e.Handled = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void cboOtherPaySource_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboOtherPaySource.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboOtherPaySource_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboOtherPaySource.Properties.Buttons[1].Visible = cboOtherPaySource.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                long treatmentTypeId = cboTreatmentType.EditValue != null ? Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "").ToString()) : 0;

                if (treatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || treatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                {
                    lciFortxtIncode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    if (HisConfig.IsManualInCode)
                    {
                        lciFortxtIncode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                        lciFortxtIncode.Enabled = true;
                        txtIncode.Enabled = true;
                        ValidateFrmInCode();
                    }
                    else
                    {
                        lciFortxtIncode.Enabled = false;
                        txtIncode.Enabled = false;
                        dxValidationUCOtherReqInfo.SetValidationRule(txtIncode, null);
                    }
                }
                else
                {
                    lciFortxtIncode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    lciFortxtIncode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dxValidationUCOtherReqInfo.SetValidationRule(txtIncode, null);
                }

                cboHosReason.EditValue = null;
                dxValidationUCOtherReqInfo.SetValidationRule(txtHosReason, null);
                dxValidationUCOtherReqInfo.SetValidationRule(txtHosReasonNt, null);
                lciHosReason.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciHosReason.AppearanceItemCaption.ForeColor = Color.Black;
                if (cboTreatmentType.EditValue != null)
                {
                    var type = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == treatmentTypeId);
                    lciHosReason.Visibility = layoutControlItem8.Visibility = type != null && (type.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || type.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY || type.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    if (layoutControlItem8.Visible)
                    {
                        ValidateTextHosReason();
                    }

                    if (layoutControlItem8.Visible && lciHosReason.Visible && ((TreatmentByPatientSdo != null && TreatmentByPatientSdo.IS_CHRONIC == 1) || (this.patientSdo != null && !string.IsNullOrEmpty(this.patientSdo.AppointmentCode))))
                    {
                        txtHosReason.Text = TreatmentByPatientSdo.HOSPITALIZATION_REASON;
                        txtHosReasonNt.Text = TreatmentByPatientSdo.ICD_NAME;
                    }

                    if (HisConfigCFG.InHospitalizationReasonRequired && lciHosReason.Visible)
                    {
                        lciHosReason.AppearanceItemCaption.ForeColor = Color.Maroon;
                        ValidateComboHosspitalizeReason();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidateComboHosspitalizeReason()
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = this.txtHosReasonNt;
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationUCOtherReqInfo.SetValidationRule(this.txtHosReasonNt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtIncode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusTochkEmergency();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientClassify_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPatientClassify.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGuaranteeUsername_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboGuaranteeUsername.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGuaranteeUsername_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (this.lciGuaranteeReason.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    txtGuaranteeReason.Focus();
                    txtGuaranteeReason.SelectAll();
                }
                else
                {
                    if (this.dlgFocusNextUserControl != null)
                        dlgFocusNextUserControl(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGuaranteeUsername_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.txtGuaranteeLoginname.Text = "";
                if (this.cboGuaranteeUsername.EditValue != null)
                {
                    var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboGuaranteeUsername.EditValue.ToString());
                    if (user != null)
                    {
                        txtGuaranteeLoginname.Text = user.LOGINNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGuaranteeReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.dlgFocusNextUserControl != null)
                        dlgFocusNextUserControl(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGuaranteeLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        cboGuaranteeUsername.EditValue = null;
                        cboGuaranteeUsername.Focus();
                        cboGuaranteeUsername.ShowPopup();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME.Contains(searchCode) || o.LOGINNAME.Contains(searchCode.ToLower())).ToList();
                        if (data != null)
                        {
                            if (data.Count == 1)
                            {
                                cboGuaranteeUsername.EditValue = data[0].LOGINNAME;
                                txtGuaranteeLoginname.Text = data[0].LOGINNAME;
                                cboGuaranteeUsername_Closed(null, null);
                            }
                            else
                            {
                                var singleData = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME.ToLower().Equals(searchCode.ToLower()));
                                if (singleData != null)
                                {
                                    cboGuaranteeUsername.EditValue = singleData.LOGINNAME;
                                    txtGuaranteeLoginname.Text = singleData.LOGINNAME;
                                    cboGuaranteeUsername_Closed(null, null);
                                }
                                else
                                {
                                    cboGuaranteeUsername.EditValue = null;
                                    cboGuaranteeUsername.Focus();
                                    cboGuaranteeUsername.ShowPopup();
                                }
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

        private void cboGuaranteeUsername_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboGuaranteeUsername_Closed(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboGuaranteeUsername.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientClassify_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPatientClassify != null)
                {
                    var patientClassify = dataClassify.FirstOrDefault(o => o.ID == Int64.Parse(cboPatientClassify.EditValue.ToString()));
                    if (patientClassify != null && patientClassify.OTHER_PAY_SOURCE_ID != null)
                    {
                        var currentOtherPaySource = dataOtherPayTemp.FirstOrDefault(o => o.ID == patientClassify.OTHER_PAY_SOURCE_ID);
                        if (currentOtherPaySource != null)
                        {
                            cboOtherPaySource.EditValue = patientClassify.OTHER_PAY_SOURCE_ID;
                        }
                        else
                        {
                            List<HIS_OTHER_PAY_SOURCE> lstOtherPay = dataOtherPayTemp;
                            var dtAllOtherPaySource = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>().Where(o => o.ID == patientClassify.OTHER_PAY_SOURCE_ID).FirstOrDefault();
                            if (dtAllOtherPaySource != null)
                                lstOtherPay.Add(dtAllOtherPaySource);
                            this.InitComboCommon(this.cboOtherPaySource, lstOtherPay, "ID", "OTHER_PAY_SOURCE_NAME", "OTHER_PAY_SOURCE_CODE");
                            cboOtherPaySource.EditValue = patientClassify.OTHER_PAY_SOURCE_ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkWNext_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkWNext.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkWNext.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkWNext.Name;
                    csAddOrUpdate.VALUE = (chkWNext.Checked ? "1" : "");
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkWNext.Name)
                        {
                            chkWNext.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsHiv_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task InitComboHisHospitalizeReason()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON> datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                await InitComboHisHospitalizeReason(datas);
                //this.InitComboCommon(this.cboHosReason, datas, "ID", "HOSPITALIZE_REASON_NAME", "HOSPITALIZE_REASON_CODE");
                //this.cboHosReason.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task InitComboHisHospitalizeReason(List<HIS_HOSPITALIZE_REASON> data)
        {
            try
            {
                try
                {
                    cboHosReason.Properties.DataSource = data;
                    cboHosReason.Properties.DisplayMember = "HOSPITALIZE_REASON_NAME";
                    cboHosReason.Properties.ValueMember = "ID";

                    cboHosReason.Properties.View.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
                    cboHosReason.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
                    cboHosReason.Properties.View.OptionsView.ShowAutoFilterRow = true;
                    cboHosReason.Properties.View.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
                    cboHosReason.Properties.View.OptionsView.ShowDetailButtons = false;
                    cboHosReason.Properties.View.OptionsView.ShowGroupPanel = false;
                    cboHosReason.Properties.View.OptionsView.ShowIndicator = false;
                    cboHosReason.Properties.View.RowCellClick += View_RowCellClick;


                    DevExpress.XtraGrid.Columns.GridColumn column = cboHosReason.Properties.View.Columns.AddField("HOSPITALIZE_REASON_CODE");
                    column.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                    column.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
                    column.VisibleIndex = 1;
                    column.Width = 150;
                    column.Caption = "Mã";

                    DevExpress.XtraGrid.Columns.GridColumn column1 = cboHosReason.Properties.View.Columns.AddField("HOSPITALIZE_REASON_NAME");
                    column1.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                    column1.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
                    column1.VisibleIndex = 2;
                    column1.Width = 250;
                    column1.Caption = "Tên";
                    cboHosReason.Properties.View.OptionsView.ShowColumnHeaders = true;
                    cboHosReason.Properties.View.OptionsSelection.MultiSelect = true;
                    cboHosReason.Properties.ImmediatePopup = true;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                cboHosReason.EditValue = ((HIS_HOSPITALIZE_REASON)cboHosReason.Properties.View.GetFocusedRow()).ID;
                cboHosReason.ClosePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void cboHosReason_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHosReason.Text = null;
                    cboHosReason.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHosReasonNt_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHosReason.EditValue = null;
                    txtHosReasonNt.Text = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Combo)
                {
                    cboHosReason.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboHosReason_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (cboHosReason.EditValue != null)
                {
                    txtHosReasonNt.Text = (cboHosReason.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON>).FirstOrDefault(o => o.ID == Int64.Parse(cboHosReason.EditValue.ToString())).HOSPITALIZE_REASON_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtHosReasonNt_DoubleClick(object sender, EventArgs e)
        {

            try
            {
                cboHosReason.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
