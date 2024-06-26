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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.RegisterVaccination.ADO;
using HIS.Desktop.Utility;
using HIS.UC.AddressCombo.ADO;
using HIS.UC.KskContract;
using HIS.UC.ServiceRoom;
using HIS.UC.UCPatientRaw.ADO;
using HIS.UC.UCTransPati.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterVaccination.Run3
{
    public partial class UCRegister : UserControlBase
    {
        #region Delcare

        public delegate void ShowFormPopup();
        UCAddressADO dataAddressPatient = new UCAddressADO();
        UCPatientRawADO dataPatientRaw = new UCPatientRawADO();
        int actionType = 0;
        internal HisPatientSDO currentPatientSDO { get; set; }
        internal HisCardSDO cardSearch = null;
        internal HeinCardData _HeinCardData { get; set; }
        internal int registerNumber = 0;
        internal List<ServiceReqDetailSDO> serviceReqDetailSDOs;
        internal bool isShowMess;
        internal List<long> serviceReqPrintIds { get; set; }
        internal bool isNotPatientDayDob = false;
        internal bool isPrintNow;
        bool isReadQrCode = true;
        string appointmentCode = "";
        internal long _TreatmnetIdByAppointmentCode = 0;
        internal bool _isPatientAppointmentCode = false;


        HisServiceReqExamRegisterResultSDO currentHisExamServiceReqResultSDO { get; set; }
        HisPatientProfileSDO resultHisPatientProfileSDO = null;

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;



        #endregion

        #region Construct - Load

        public UCRegister(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                //WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister => 1");
                InitializeComponent();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load => 1");
                HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.LoadConfig();
                HisConfigCFG.LoadConfig();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister => 2");
                this.FocusNextUserControl();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister => 3");
                this.ConfigLayout();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister => 4");
                this.currentModule = module;
                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCRegister_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                //Ẩn combo patient Type với tiếp đón 3
                this.ucPatientRaw1.SetTD3(true);

                this.ucPatientRaw1.RefreshUserControl();

                this.ucPatientRaw1.FocusUserControl();

                this.actionType = GlobalVariables.ActionAdd;

                this.FillDataDefaultToControl();

                this.SetDefaultCashierRoom();

                ValidateGridLookupWithTextEdit(cboPhongKham, txtPhongKham, dxValidationProvider1);
                ValidateGridLookupWithTextEdit(cboPatientType, txtPatientTypeCode, dxValidationProvider1);
                ValidateSingleControl(dtThoiGian, dxValidationProvider1);

                LoadDataDefaultVaccination();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateSingleControl(Control cbo, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(cbo, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region private Method

        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetDeaultPatientType()
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = null;
            if (!String.IsNullOrEmpty(AppConfigs.PatientTypeCodeDefault))
            {
                patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == AppConfigs.PatientTypeCodeDefault);
                if (patientType == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Phan mem HIS da duoc cau hinh doi tuong benh nhan mac dinh, tuy nhien ma doi tuong cau hinh khong ton tai trong danh muc doi tuong benh nhan, he thong tu dong lay doi tuong mac dinh la doi tuong BHYT. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AppConfigs.PatientTypeCodeDefault), AppConfigs.PatientTypeCodeDefault));
                    patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT);
                }
            }
            else
            {
                patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT);
            }
            return patientType;
        }

        bool GetIsChild()
        {
            bool valid = false;
            try
            {
                valid = ucPatientRaw1.GetIsChild();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private void ConfigLayout()
        {
            try
            {
                layoutControl1.BeginUpdate();
                for (int i = 0; i < layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions.Count; i++)
                {
                    layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions[i].SizeType = SizeType.Percent;
                    layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions[i].Width = (100 / 6);
                }

                Inventec.Common.Logging.LogSystem.Debug("ConfigLayout - 1");
                for (int i = 0; i < layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Count; i++)
                {
                    layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions[i].SizeType = SizeType.Percent;
                    layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions[i].Height = (100 / 14);
                }
                Inventec.Common.Logging.LogSystem.Debug("ConfigLayout - 2");
                for (int i = layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions.Count; i < 6; i++)
                {
                    layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions.Add(
                        new ColumnDefinition()
                        {
                            SizeType = SizeType.Percent,
                            Width = (100 / 6)
                        });
                }
                Inventec.Common.Logging.LogSystem.Debug("ConfigLayout - 3");
                for (int i = layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Count; i < 14; i++)
                {
                    layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Add(
                        new RowDefinition()
                        {
                            SizeType = SizeType.Percent,
                            Height = (100 / 14)
                        });
                }
                layoutControl1.EndUpdate();
                Inventec.Common.Logging.LogSystem.Debug("ConfigLayout - 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataAfterSearchPatientInUCPatientRaw(object data)
        {
            try
            {
                if (data != null)
                {
                    string heinCardNumber = "";
                    DataResultADO dataResult = (DataResultADO)data;
                    this.SetValueVariableUCAddressCombo(dataResult);
                    if (dataResult.OldPatient == false && dataResult.UCRelativeADO != null)
                    {
                        // FillDataIntoUCRelativeInfo(dataResult.UCRelativeADO);
                    }
                    else if (dataResult.OldPatient == false && dataResult.HeinCardData != null)
                    {
                        this.FillDataAfterSaerchPatientInUCPatientRaw(dataResult.HeinCardData);
                        heinCardNumber = dataResult.HeinCardData.HeinCardNumber;
                    }
                    else if (dataResult.HisPatientSDO != null)
                    {
                        this._isPatientAppointmentCode = (dataResult.SearchTypePatient == 2);
                        if (!String.IsNullOrEmpty(dataResult.AppointmentCode))
                            this.appointmentCode = dataResult.AppointmentCode;
                        this._TreatmnetIdByAppointmentCode = (dataResult.TreatmnetIdByAppointmentCode == null ? 0 : dataResult.TreatmnetIdByAppointmentCode);
                        this.currentPatientSDO = dataResult.HisPatientSDO;
                        FillDataIntoUCPlusInfo(currentPatientSDO);
                        //FillDataIntoUCRelativeInfo(currentPatientSDO);
                        FillDataIntoUCAddressInfo(currentPatientSDO);

                        //Fill du lieu yeu cau kham moi nhat cua benh nhan (neu co)//xuandv
                        HeinCardData dataCheck = new HeinCardData();
                        dataCheck.Dob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientSDO.DOB);
                        dataCheck.PatientName = currentPatientSDO.VIR_PATIENT_NAME;
                        dataCheck.HeinCardNumber = currentPatientSDO.HeinCardNumber;
                        dataCheck.Gender = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.HisToHein(currentPatientSDO.GENDER_ID.ToString());
                        dataCheck.FromDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientSDO.HeinCardFromTime ?? 0);
                        dataCheck.MediOrgCode = currentPatientSDO.HeinMediOrgCode;
                        dataCheck.Address = currentPatientSDO.HeinAddress;
                        dataCheck.ToDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientSDO.HeinCardToTime ?? 0);
                        heinCardNumber = currentPatientSDO.HeinCardNumber;
                    }
                    this.SetPatientSearchPanel(dataResult.OldPatient);
                    this.cardSearch = dataResult.HisCardSDO;

                    if (!String.IsNullOrEmpty(heinCardNumber))
                        this.FillDataCareerUnder6AgeByHeinCardNumber(heinCardNumber);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_CAREER GetCareerByBhytWhiteListConfig(string heinCardNumder)
        {
            HIS_CAREER result = null;
            try
            {
                if (!String.IsNullOrEmpty(heinCardNumder))
                {
                    var bhytWhiteList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>().FirstOrDefault(o => !String.IsNullOrEmpty(heinCardNumder) && o.BHYT_WHITELIST_CODE.ToUpper() == heinCardNumder.Substring(0, 3).ToUpper());
                    if (bhytWhiteList != null && (bhytWhiteList.CAREER_ID ?? 0) > 0)
                    {
                        result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().SingleOrDefault(o => o.ID == bhytWhiteList.CAREER_ID.Value);
                        if (result == null)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("GetCareerByBhytWhiteListConfig => Khong lay duoc nghe nghiep theo id = " + bhytWhiteList.CAREER_ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void FillDataCareerUnder6AgeByHeinCardNumber(string heinCardNumder)
        {
            try
            {
                //Khi người dùng nhập thẻ BHYT, nếu đầu mã thẻ là TE1, thì tự động chọn giá trị của trường "Nghề nghiệp" là "Trẻ em dưới 6 tuổi"
                //27/10/2017 => sửa lại => Căn cứ vào đầu thẻ BHYT và dữ liệu cấu hình trong bảng HIS_BHYT_WHITELIST để tự động điền nghề nghiệp tương ứng
                var patientRawObj = this.ucPatientRaw1.GetValue();
                MOS.EFMODEL.DataModels.HIS_CAREER career = GetCareerByBhytWhiteListConfig(heinCardNumder);
                if (career == null)
                {
                    if (patientRawObj.DOB > 0)
                    {
                        DateTime dtDob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientRawObj.DOB).Value;
                        if (dtDob != DateTime.MinValue && MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtDob))
                        {
                            career = HisConfigCFG.CareerUnder6Age;
                        }
                        else if (DateTime.Now.Year - dtDob.Year <= 18)
                        {
                            career = HisConfigCFG.CareerHS;
                        }
                        else
                        {
                            career = HisConfigCFG.CareerBase;
                        }
                    }
                    else
                    {
                        career = HisConfigCFG.CareerBase;
                    }
                }
                if (career != null && career.ID > 0)
                {
                    patientRawObj.CARRER_ID = career.ID;
                    patientRawObj.CARRER_CODE = career.CAREER_CODE;
                    patientRawObj.CARRER_NAME = career.CAREER_NAME;
                    this.ucPatientRaw1.SetValue(patientRawObj);
                    //this.cboCareer.EditValue = career.ID;
                    //this.txtCareerCode.Text = career.CAREER_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueVariableUCAddressCombo(DataResultADO data)
        {
            try
            {
                if (data.OldPatient == true)
                    this.ucAddressCombo1.isPatientBHYT = true;
                else
                    this.ucAddressCombo1.isPatientBHYT = false;
                if (data.HeinCardData != null)
                    this.ucAddressCombo1.isReadCard = true;
                else
                    this.ucAddressCombo1.isReadCard = false;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event Control

        private void btnNewContinue_Click(object sender, EventArgs e)
        {
            try
            {
                this.RefreshUserControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Save(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Save(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.Print();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDepositDetail_Click(object sender, EventArgs e)
        {
            try
            {
                this.DepositDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBill_Click(object sender, EventArgs e)
        {
            try
            {
                this.Bill();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTreatmentBedRoom_Click(object sender, EventArgs e)
        {
            try
            {
                this.TreatmentBedRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveAndAssain_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveAndAssain();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                this.CreateThreadCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRecallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                this.ReCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPatientNew_Click(object sender, EventArgs e)
        {
            try
            {
                this.GetDataBeforeClickbtnPatientNew();
                this.RefreshUserControl();
                this.dataPatientRaw.PATIENT_CODE = "";
                this.dataPatientRaw.CARRER_ID = null;
                this.dataPatientRaw.PATIENT_CODE = "";
                this.ucPatientRaw1.SetValue(dataPatientRaw);
                this.SetPatientSearchPanel(false);
                this.ucPatientRaw1.FocusToPatientType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGridVaccination();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDefaultVaccination()
        {
            try
            {
                dtThoiGian.DateTime = DateTime.Now;
                dtTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                cboStatus.SelectedIndex = 2;


                LoadDataToGridVaccination();

                LoadPhongKhamToCombo();

                LoadPhongTiemToCombo();

                LoadPatientTypeToCombo();

                InitDanhSachThuocTheoPhongTiem();

                LoadDanTocToCombo();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        int rowCount = 0;
        int dataTotal = 0;
        int numPageSize;

        List<V_HIS_MEST_ROOM> _MestRoomDefaults { get; set; }

        List<HIS_PATIENT_TYPE> _PatientTypeDefaults { get; set; }

        List<V_HIS_EXECUTE_ROOM> _ExecuteRoomDefaults { get; set; }

        List<SDA_ETHNIC> _EthnicDefaults { get; set; }

        internal void LoadDataToGridVaccination()
        {
            if (ucPaging1.pagingGrid != null)
            {
                numPageSize = ucPaging1.pagingGrid.PageSize;
            }
            else
            {
                numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
            }

            FillDataToGridVaccination(new CommonParam(0, (int)numPageSize));
            CommonParam param = new CommonParam();
            param.Limit = rowCount;
            param.Count = dataTotal;
            ucPaging1.Init(FillDataToGridVaccination, param, numPageSize, gridControlVaccination);
        }

        internal void FillDataToGridVaccination(object param)
        {
            try
            {
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                numPageSize = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<V_HIS_VACCINATION>> apiResult = new ApiResultObject<List<V_HIS_VACCINATION>>();
                HisVaccinationViewFilter _Filter = new HisVaccinationViewFilter();
                _Filter.KEY_WORD = txtSearch.Text.Trim();

                if (dtTimeFrom != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    _Filter.REQUEST_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (dtTimeTo != null && dtTimeTo.DateTime != DateTime.MinValue)
                    _Filter.REQUEST_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmm") + "59");

                //Chua uong : 0
                //Da uong : 1
                //Tat ca : 2

                if (cboStatus.EditValue != null)
                {
                    switch (cboStatus.SelectedIndex)
                    {
                        case 0:
                            _Filter.HAS_EXECUTE_TIME = false;
                            break;
                        case 1:
                            _Filter.HAS_EXECUTE_TIME = true;
                            break;
                        case 2:
                            _Filter.HAS_EXECUTE_TIME = null;
                            break;
                    }
                }

                _Filter.ORDER_FIELD = "REQUEST_TIME";
                _Filter.ORDER_DIRECTION = "DESC";
                apiResult = new BackendAdapter(paramCommon)
                    .GetRO<List<V_HIS_VACCINATION>>("api/HisVaccination/GetView", ApiConsumers.MosConsumer, _Filter, paramCommon);

                gridControlVaccination.DataSource = null;
                if (apiResult != null)
                {
                    rowCount = (apiResult.Data == null ? 0 : apiResult.Data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    gridControlVaccination.DataSource = apiResult.Data;
                }

                gridViewVaccination.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewVaccination.OptionsSelection.EnableAppearanceFocusedRow = true;

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void LoadPatientTypeToCombo()
        {
            try
            {
                this._PatientTypeDefaults = new List<HIS_PATIENT_TYPE>();
                this._PatientTypeDefaults = BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                    .Where(o => o.IS_ACTIVE == 1
                        && (o.PATIENT_TYPE_CODE == HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.VACC")
                        || o.PATIENT_TYPE_CODE == HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.VACC_EPI"))).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, this._PatientTypeDefaults, controlEditorADO);
                if (this._PatientTypeDefaults != null && this._PatientTypeDefaults.Count > 0)
                {
                    cboPatientType.EditValue = this._PatientTypeDefaults.FirstOrDefault().ID;
                    txtPatientTypeCode.Text = this._PatientTypeDefaults.FirstOrDefault().PATIENT_TYPE_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPhongKhamToCombo()
        {
            try
            {
                this._ExecuteRoomDefaults = new List<V_HIS_EXECUTE_ROOM>();
                this._ExecuteRoomDefaults = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.IS_VACCINE == 1
                    && o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 150, 1, true));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPhongKham, this._ExecuteRoomDefaults, controlEditorADO);
                if (this._ExecuteRoomDefaults != null && this._ExecuteRoomDefaults.Count > 0)
                {
                    cboPhongKham.EditValue = this._ExecuteRoomDefaults.FirstOrDefault().ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPhongTiemToCombo()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "Mã kho", 150, 1, true));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "Tên kho", 250, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "MEDI_STOCK_ID", columnInfos, true, 350);


                if (cboPhongKham.EditValue != null)
                {
                    var room = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(p => p.ID == (long)cboPhongKham.EditValue);
                    var mestRoomTemps = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>().Where(o => o.IS_ACTIVE == 1 && o.ROOM_ID == room.ROOM_ID).ToList();
                    var mediStockId__Actives = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == null
                        || o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => o.ID).ToList();
                    mestRoomTemps = mestRoomTemps.Where(o => mediStockId__Actives != null && mediStockId__Actives.Contains(o.MEDI_STOCK_ID))
                        .ToList();

                    _MestRoomDefaults = (from r in mestRoomTemps
                                         select
                                           new V_HIS_MEST_ROOM
                                           {
                                               MEDI_STOCK_CODE = r.MEDI_STOCK_CODE,
                                               MEDI_STOCK_ID = r.MEDI_STOCK_ID,
                                               MEDI_STOCK_NAME = r.MEDI_STOCK_NAME
                                           }).Distinct().ToList();


                    ControlEditorLoader.Load(cboPhongTiem, mestRoomTemps, controlEditorADO);
                    if (mestRoomTemps != null && mestRoomTemps.Count > 0)
                    {
                        cboPhongTiem.EditValue = mestRoomTemps.FirstOrDefault().MEDI_STOCK_ID;
                    }
                }
                else
                {
                    ControlEditorLoader.Load(cboPhongTiem, null, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDanhSachThuocTheoPhongTiem()
        {
            try
            {
                //Khoi tao mui tiem
                gridControlVaccinationMety.DataSource = null;
                List<ExpMestMedicineADO> vaccinantionMetys = new List<ExpMestMedicineADO>();
                ExpMestMedicineADO vaccinantionMety = new ExpMestMedicineADO();
                vaccinantionMety.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                vaccinantionMety.AMOUNT = 1;
                //if (vaccinationExam != null)
                //{
                //    vaccinantionMety.PATIENT_TYPE_ID = vaccinationExam.PATIENT_TYPE_ID;
                //}
                vaccinantionMetys.Add(vaccinantionMety);
                gridControlVaccinationMety.DataSource = vaccinantionMetys;
                //
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDanTocToCombo()
        {
            try
            {
                var _rooms = BackendDataWorker.Get<SDA_ETHNIC>().Where(o => o.IS_ACTIVE == 1).ToList();
                _EthnicDefaults = _rooms;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ETHNIC_CODE", "", 150, 1, true));
                columnInfos.Add(new ColumnInfo("ETHNIC_NAME", "", 250, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ETHNIC_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDanToc, _rooms, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVaccinationExam_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_VACCINATION dataRow = (V_HIS_VACCINATION)((System.Collections.IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                    }

                    else if (e.Column.FieldName == "REQUEST_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.REQUEST_TIME);
                    }
                    else if (e.Column.FieldName == "EXECUTE_TIME_DISPLAY")
                    {
                        if (dataRow.EXECUTE_TIME.HasValue)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.EXECUTE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        if (dataRow.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            e.Value = dataRow.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        else
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.TDL_PATIENT_DOB.ToString());
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewlVaccinationMety_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "ACTION")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewVaccinationMety.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repositoryItemButton__Add;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = repositoryItemButton__Remove;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Add_Click(object sender, EventArgs e)
        {
            try
            {
                var vaccinationMetyADOs = gridControlVaccinationMety.DataSource as List<ExpMestMedicineADO>;
                ExpMestMedicineADO vaccinationMetyADO = new ExpMestMedicineADO();
                vaccinationMetyADO.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                vaccinationMetyADO.AMOUNT = 1;
                //if (vaccinationExam != null)
                //{
                //    vaccinationMetyADO.PATIENT_TYPE_ID = vaccinationExam.PATIENT_TYPE_ID;
                //}
                vaccinationMetyADOs.Add(vaccinationMetyADO);
                gridControlVaccinationMety.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Remove_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var vaccinationMetyADOs = gridControlVaccinationMety.DataSource as List<ExpMestMedicineADO>;
                var vaccinationMetyADO = gridViewVaccinationMety.GetFocusedRow() as ExpMestMedicineADO;
                if (vaccinationMetyADO != null)
                {
                    vaccinationMetyADOs.Remove(vaccinationMetyADO);
                    gridControlVaccinationMety.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Delete_Click(object sender, EventArgs e)
        {
            try
            {
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn xóa dữ liệu?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var row = (V_HIS_VACCINATION)gridViewVaccination.GetFocusedRow();
                    if (row != null)
                    {
                        CommonParam param = new CommonParam();

                        HisVaccinationSDO sdo = new HisVaccinationSDO();
                        sdo.Id = row.ID;
                        sdo.RequestRoomId = this.currentModule.RoomId;

                        var rsApi = new BackendAdapter(param).Post<bool>("api/HisVaccination/Delete", ApiConsumers.MosConsumer, sdo, param);
                        if (rsApi)
                        {
                            LoadDataToGridVaccination();
                        }
                        MessageManager.Show(this.ParentForm, param, rsApi);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Edit_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (V_HIS_VACCINATION)gridViewVaccination.GetFocusedRow();
                if (row != null)
                {
                    HIS.Desktop.Plugins.RegisterVaccination.FormEdit.frmMedicine frm = new HIS.Desktop.Plugins.RegisterVaccination.FormEdit.frmMedicine(this.currentModule, row.ID);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhongTiem_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                InitDanhSachThuocTheoPhongTiem();
                long? mediStockId = null;
                if (cboPhongTiem.EditValue != null)
                {
                    mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPhongTiem.EditValue.ToString());
                    V_HIS_MEST_ROOM mestRoom = this._MestRoomDefaults.FirstOrDefault(o => o.MEDI_STOCK_ID == mediStockId);
                    txtPhongTiem.Text = mestRoom.MEDI_STOCK_CODE;
                    //cboPhongTiem.Properties.Buttons[1].Visible = true;
                }
                LoadMedicineByMediStock(mediStockId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMedicineByMediStock(long? mediStockId)
        {
            try
            {
                List<D_HIS_MEDI_STOCK_1> mediStocks = null;
                if (mediStockId.HasValue)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    MOS.Filter.DHisMediStock1Filter filter = new MOS.Filter.DHisMediStock1Filter();
                    filter.MEDI_STOCK_IDs = new List<long> { mediStockId.Value };
                    filter.IS_VACCINE = true;
                    mediStocks = new BackendAdapter(param).Get<List<D_HIS_MEDI_STOCK_1>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, param);
                    WaitingManager.Hide();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "Mã thuốc", 150, 1, true));
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "Tên thuốc", 250, 2, true));
                columnInfos.Add(new ColumnInfo("AMOUNT", "Số lượng", 100, 3, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, true, 500);
                ControlEditorLoader.Load(repositoryItemCustomGridLookUp_Vaccination, mediStocks, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhongKham_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPhongKham.EditValue != null)
                {
                    var room = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(p => p.ID == (long)cboPhongKham.EditValue);
                    txtPhongKham.Text = room.EXECUTE_ROOM_CODE;
                }
                else
                {
                    txtPhongKham.Text = "";
                }
                LoadPhongTiemToCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientTypeCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadPatientType(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatientType(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPatientType.Focus();
                    cboPatientType.ShowPopup();
                }
                else
                {
                    var data = this._PatientTypeDefaults.Where(o =>
                        o.IS_ACTIVE == 1
                        && o.PATIENT_TYPE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPatientType.EditValue = data[0].ID;
                            dtThoiGian.Focus();
                            dtThoiGian.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PATIENT_TYPE_CODE == searchCode);
                            if (search != null)
                            {
                                cboPatientType.EditValue = search.ID;
                                dtThoiGian.Focus();
                                dtThoiGian.SelectAll();
                            }
                            else
                            {
                                cboPatientType.EditValue = null;
                                cboPatientType.Focus();
                                cboPatientType.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboPatientType.EditValue = null;
                        cboPatientType.Focus();
                        cboPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPatientType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE data = this._PatientTypeDefaults.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPatientTypeCode.Text = data.PATIENT_TYPE_CODE;
                            dtThoiGian.Focus();
                            dtThoiGian.SelectAll();
                        }
                    }
                }
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
                    if (cboPatientType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE data = this._PatientTypeDefaults.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPatientTypeCode.Text = data.PATIENT_TYPE_CODE;
                            dtThoiGian.Focus();
                            dtThoiGian.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtThoiGian_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPhongKham.Focus();
                    txtPhongKham.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhongKham_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadPhongKham(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPhongKham(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPhongKham.Focus();
                    cboPhongKham.ShowPopup();
                }
                else
                {
                    var data = this._ExecuteRoomDefaults.Where(o =>
                        o.IS_ACTIVE == 1
                        && o.EXECUTE_ROOM_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPhongKham.EditValue = data[0].ID;
                            txtPhongTiem.Focus();
                            txtPhongTiem.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EXECUTE_ROOM_CODE == searchCode);
                            if (search != null)
                            {
                                cboPhongKham.EditValue = search.ID;
                                txtPhongTiem.Focus();
                                txtPhongTiem.SelectAll();
                            }
                            else
                            {
                                cboPhongKham.EditValue = null;
                                cboPhongKham.Focus();
                                cboPhongKham.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboPhongKham.EditValue = null;
                        cboPhongKham.Focus();
                        cboPhongKham.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhongKham_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPhongKham.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM data = this._ExecuteRoomDefaults.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPhongKham.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPhongKham.Text = data.EXECUTE_ROOM_CODE;
                            txtPhongTiem.Focus();
                            txtPhongTiem.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhongKham_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPhongKham.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM data = this._ExecuteRoomDefaults.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPhongKham.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPhongKham.Text = data.EXECUTE_ROOM_CODE;
                            txtPhongTiem.Focus();
                            txtPhongTiem.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhongTiem_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadPhongTiem(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPhongTiem(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPhongTiem.Focus();
                    cboPhongTiem.ShowPopup();
                }
                else
                {
                    var data = this._MestRoomDefaults.Where(o =>
                        o.IS_ACTIVE == 1
                        && o.MEDI_STOCK_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPhongTiem.EditValue = data[0].ID;
                            txtPhongTiem.Focus();
                            txtPhongTiem.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.MEDI_STOCK_CODE == searchCode);
                            if (search != null)
                            {
                                cboPhongTiem.EditValue = search.ID;
                                gridControlVaccinationMety.Focus();
                            }
                            else
                            {
                                cboPhongTiem.EditValue = null;
                                cboPhongTiem.Focus();
                                cboPhongTiem.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboPhongTiem.EditValue = null;
                        cboPhongTiem.Focus();
                        cboPhongTiem.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhongTiem_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPhongTiem.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM data = this._MestRoomDefaults.FirstOrDefault(o => o.MEDI_STOCK_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPhongTiem.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPhongTiem.Text = data.MEDI_STOCK_CODE;
                            gridControlVaccinationMety.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhongTiem_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPhongTiem.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM data = this._MestRoomDefaults.FirstOrDefault(o => o.MEDI_STOCK_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPhongTiem.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPhongTiem.Text = data.MEDI_STOCK_CODE;
                            gridControlVaccinationMety.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDanToc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadDanToc(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDanToc(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDanToc.Focus();
                    cboDanToc.ShowPopup();
                }
                else
                {
                    var data = this._EthnicDefaults.Where(o =>
                        o.IS_ACTIVE == 1
                        && o.ETHNIC_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboDanToc.EditValue = data[0].ID;
                            txtNguoiNha.Focus();
                            txtNguoiNha.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.ETHNIC_CODE == searchCode);
                            if (search != null)
                            {
                                cboDanToc.EditValue = search.ID;
                                txtNguoiNha.Focus();
                                txtNguoiNha.SelectAll();
                            }
                            else
                            {
                                cboDanToc.EditValue = null;
                                cboDanToc.Focus();
                                cboDanToc.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboDanToc.EditValue = null;
                        cboDanToc.Focus();
                        cboDanToc.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDanToc_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDanToc.EditValue != null)
                    {
                        var data = this._EthnicDefaults.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDanToc.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtDanToc.Text = data.ETHNIC_CODE;
                            txtNguoiNha.Focus();
                            txtNguoiNha.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDanToc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDanToc.EditValue != null)
                    {
                        var data = this._EthnicDefaults.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDanToc.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtDanToc.Text = data.ETHNIC_CODE;
                            txtNguoiNha.Focus();
                            txtNguoiNha.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNguoiNha_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtQuanHe.Focus();
                    txtQuanHe.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtQuanHe_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCMND.Focus();
                    txtCMND.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCMND_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPhoneNumber.Focus();
                    txtPhoneNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiaChi_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVaccinationExam_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                var row = (V_HIS_VACCINATION)gridViewVaccination.GetRow(e.RowHandle);
                if (row != null)
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (row.BILL_ID == null
                            && row.VACCINATION_STT_ID != IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH
                            && row.REQUEST_LOGINNAME == loginName)
                        {
                            e.RepositoryItem = repositoryItemButton__Delete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Delete_D;
                        }
                    }
                    else if (e.Column.FieldName == "EDIT")
                    {
                        if (row.BILL_ID == null
                            && row.VACCINATION_STT_ID != IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH
                            && row.REQUEST_LOGINNAME == loginName)
                        {
                            e.RepositoryItem = repositoryItemButton__Edit;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Edit_D;
                        }
                    }
                    else if (e.Column.FieldName == "BILL_STR")
                    {
                        if (row.BILL_ID == null)
                        {
                            e.RepositoryItem = repositoryItemButton__Bill;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Bill_D;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Bill_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_VACCINATION)gridViewVaccination.GetFocusedRow();
                if (row == null)
                    throw new ArgumentNullException("V_HIS_VACCINATION is null");

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MedicineVaccinBill").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.MedicineVaccinBill'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.MedicineVaccinBill' is not plugins");

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.currentModule.RoomId;
                    moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    listArgs.Add(moduleData);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)ReloAd);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloAd(object data)
        {
            try
            {
                btnTimKiem_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Bill_D_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_VACCINATION)gridViewVaccination.GetFocusedRow();
                if (row != null && row.BILL_ID > 0)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionCancel").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionCancel'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.MedicineVaccinBill' is not plugins");

                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        moduleData.RoomId = this.currentModule.RoomId;
                        moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row.BILL_ID);
                        listArgs.Add(moduleData);
                        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)ReloAd);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("extenceInstance is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                    }


                    //CommonParam param = new CommonParam();
                    //HisTransactionCancelSDO sdo = new HisTransactionCancelSDO();
                    //sdo.TransactionId = row.BILL_ID ?? 0;
                    //sdo.RequestRoomId = this.currentModule.RoomId;
                    //sdo.CancelTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    //var rsApi = new BackendAdapter(param).Post<bool>("api/HisTransaction/Cancel", ApiConsumers.MosConsumer, sdo, param);
                    //if (rsApi)
                    //{
                    //    LoadDataToGridVaccination();
                    //}
                    //MessageManager.Show(this.ParentForm, param, rsApi);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPhoneNumber_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtPhoneNumber_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtPhoneNumber_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiaChi.Focus();
                    txtDiaChi.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
