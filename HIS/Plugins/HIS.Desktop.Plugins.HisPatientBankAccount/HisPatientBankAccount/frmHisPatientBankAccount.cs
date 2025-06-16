using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout.Converter;
using DevExpress.XtraNavBar;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisPatientBankAccount.Validate;
using HIS.Desktop.Utilities;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace HIS.Desktop.Plugins.HisPatientBankAccount.HisPatientBankAccount
{
    public partial class frmHisPatientBankAccount : HIS.Desktop.Utility.FormBase
    {

        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT currentVData;
        DelegateSelectData delegateSelectData1;

        private HIS_TREATMENT currentTreatment;

        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;

        #endregion
        #region Construct
        public frmHisPatientBankAccount(Inventec.Desktop.Common.Modules.Module moduleData, HIS_TREATMENT _currentTreatment, DelegateSelectData _delegateSelectData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                this.currentTreatment = _currentTreatment;

                this.delegateSelectData1 = _delegateSelectData;
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
        #endregion


        #region Private method
        private void frmHisPatientBankAccount_load(object sender, EventArgs e)
        {
            try
            {
                ShowInfo();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowInfo()
        {
            try
            {
                btnChooseItem.Enabled = false;
                DisplayPatientInfo(currentTreatment);
                SetDefaultValue();
                if (currentVData != null)
                {
                    FillDataToEditorControl(currentVData);
                }
                InitComboBankPayer();
                FillDataToGridControl();
                ValidateForm();



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
      
        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (delegateSelectData1 != null)
                {
                    V_HIS_PATIENT_BANK_ACCOUNT data = new V_HIS_PATIENT_BANK_ACCOUNT();
                    delegateSelectData1(data);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {

                this.ActionType = GlobalVariables.ActionAdd;
                txtSearch.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                cboListBank.EditValue = null;
                txtAccNumber.Text = "";
                txtReceiver.Text = "";
                txtRelation.Text = "";
                dxValidationProviderEditorInfo.RemoveControlError(txtAccNumber);
                dxValidationProviderEditorInfo.RemoveControlError(cboListBank);
                dxValidationProviderEditorInfo.RemoveControlError(txtReceiver);
                dxValidationProviderEditorInfo.RemoveControlError(txtRelation);


                dxErrorProvider.ClearErrors(); 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDepartmentId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_DEPARTMENT>>("api/HisBank/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
        #region init combo

        #endregion
        #region
        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        /// 

        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlFormList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }


        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {

            try
            {

                int startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                HisPatientBankAccountViewFilter filter = new HisPatientBankAccountViewFilter();
                filter.PATIENT_ID = currentTreatment.PATIENT_ID;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.KEY_WORD = txtSearch.Text.Trim();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                gridViewFormList.BeginUpdate();

                var apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT>>(
                    HisRequestUriStore.GETVIEW,
                    ApiConsumers.MosConsumer,
                    filter,
                    paramCommon);

                if (apiResult != null && apiResult.Data != null)
                {
                    var listBankAccounts = apiResult.Data;
                    gridViewFormList.GridControl.DataSource = listBankAccounts;
                    rowCount = listBankAccounts.Count;
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                else
                {
                    gridViewFormList.GridControl.DataSource = null;
                    rowCount = 0;
                    dataTotal = 0;
                }

                gridViewFormList.EndUpdate();


                SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion


        private void SaveProcess()
        {
            if (!Validate1()) return; 
            CommonParam param = new CommonParam();
            try
            {

                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();

                MOS.EFMODEL.DataModels.HIS_PATIENT_BANK_ACCOUNT updateDTO = new MOS.EFMODEL.DataModels.HIS_PATIENT_BANK_ACCOUNT();
                MOS.EFMODEL.DataModels.HIS_BANK updateBank = new MOS.EFMODEL.DataModels.HIS_BANK();

                if (this.currentVData != null && this.currentVData.ID > 0)
                {
                    LoadCurrent(this.currentVData.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO, ref updateBank);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    updateDTO.PATIENT_ID = currentTreatment.PATIENT_ID;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_PATIENT_BANK_ACCOUNT>(
                        HisRequestUriStore.CREATE,
                        ApiConsumers.MosConsumer,
                        updateDTO,
                        param
                    );

                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_PATIENT_BANK_ACCOUNT>(
                        HisRequestUriStore.UPDATE,
                        ApiConsumers.MosConsumer,
                        updateDTO,
                        param
                    );

                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
                    SetFocusEditor();
                    ResetFormData();

                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool Validate1()
        {
            bool Valid = true;
            if (string.IsNullOrEmpty(txtAccNumber.Text))
            {
                dxErrorProvider.SetError(txtAccNumber, "Trường dữ liệu bắt buộc", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                Valid = false;
            }else if(Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtAccNumber.Text, 100))
            {
                dxErrorProvider.SetError(txtAccNumber, "Trường dữ liệu vượt quá 100 ký tự", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                Valid = false;
            }else
            {
                dxErrorProvider.SetError(txtAccNumber, "", DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
            }

            if (string.IsNullOrEmpty(txtReceiver.Text))
            {
                dxErrorProvider.SetError(txtReceiver, "Trường dữ liệu bắt buộc", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                Valid = false;
            }
            else if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtReceiver.Text, 100))
            {
                dxErrorProvider.SetError(txtReceiver, "Trường dữ liệu vượt quá 100 ký tự", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                Valid = false;
            }
            else
            {
                dxErrorProvider.SetError(txtReceiver, "", DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
            }
            if (string.IsNullOrEmpty(cboListBank.Text))
            {
                dxErrorProvider.SetError(cboListBank, "Trường dữ liệu bắt buộc", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                Valid = false;
            }
            else
            {
                dxErrorProvider.SetError(cboListBank, "", DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
            }
            if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtRelation.Text, 100))
            {
                dxErrorProvider.SetError(txtRelation, "Trường dữ liệu vượt quá 100 ký tự", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                Valid = false;
            }
            else
            {
                dxErrorProvider.SetError(txtRelation, "", DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
            }
                return Valid;
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_PATIENT_BANK_ACCOUNT currentDTO)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_BANK_ACCOUNT>(currentDTO, currentVData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_PATIENT_BANK_ACCOUNT currentDTO, ref MOS.EFMODEL.DataModels.HIS_BANK currentBank)
        {
            try
            {
                if (cboListBank.EditValue != null)
                    currentDTO.PAYEE_BANK_ID = Convert.ToInt64(cboListBank.EditValue);
                currentDTO.PAYEE_ACCOUNT_NUMBER = txtAccNumber.Text.Trim();
                currentDTO.PAYEE_NAME = txtReceiver.Text.Trim();
                currentDTO.RELATION_NAME = txtRelation.Text.Trim();




            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFocusEditor()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }


        private void DisplayPatientInfo(HIS_TREATMENT patient)
        {
            try
            {

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData("Input HIS_TREATMENT:", patient));

                currentTreatment = patient;
                if (patient == null)
                {
                    lblPatientCode.Text = "";
                    lblPatientName.Text = "";
                    lblPatientDOB.Text = "";
                    lblPatientGender.Text = "";
                    lblPatientAddress.Text = "";
                    lblPatientIdentity.Text = "";
                    return;
                }

                lblPatientCode.Text = patient.TDL_PATIENT_CODE ?? "";
                lblPatientName.Text = patient.TDL_PATIENT_NAME ?? "";
                if (patient.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                {
                    if (patient.TDL_PATIENT_DOB > 0)
                    {
                        lblPatientDOB.Text = patient.TDL_PATIENT_DOB.ToString().Substring(0, 4); 
                    }
                    else
                    {
                        lblPatientDOB.Text = ""; 
                    }
                }
                else
                {
                    lblPatientDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.TDL_PATIENT_DOB);
                }

                lblPatientGender.Text = patient.TDL_PATIENT_GENDER_NAME ?? "";
                lblPatientAddress.Text = patient.TDL_PATIENT_ADDRESS ?? "";

                // Identity information
                if (!string.IsNullOrWhiteSpace(patient.TDL_PATIENT_CCCD_NUMBER))
                    lblPatientIdentity.Text = patient.TDL_PATIENT_CCCD_NUMBER;
                else if (!string.IsNullOrWhiteSpace(patient.TDL_PATIENT_CMND_NUMBER))
                    lblPatientIdentity.Text = patient.TDL_PATIENT_CMND_NUMBER;
                else if (!string.IsNullOrWhiteSpace(patient.TDL_PATIENT_PASSPORT_NUMBER))
                    lblPatientIdentity.Text = patient.TDL_PATIENT_PASSPORT_NUMBER;
                else
                    lblPatientIdentity.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region validate
        private void ValidateForm()
        {
            try
            {
                ValidatetxtInput(cboListBank, "Trường dữ liệu bắt buộc");
                ValidatetxtInput(txtAccNumber, "Trường dữ liệu bắt buộc");
                ValidatetxtInput(txtReceiver, "Trường dữ liệu bắt buộc");

                ValidateMaxLengthTxt(txtAccNumber, 100);
                ValidateMaxLengthTxt(txtReceiver, 100);
                ValidateMaxLengthTxt(txtRelation, 100);
                ValidationInputData(cboListBank, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        private void ValidateMaxLengthTxt(TextEdit txt, int maxLength)
        {
            try
            {
                ValidateMaxlength validRule = new ValidateMaxlength();
                validRule.txt = txt;
                validRule.maxLength = maxLength;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(txt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidatetxtInput(DevExpress.XtraEditors.TextEdit txtInfo, string errorText)
        {
            try
            {
                ValidatetxtInput validRule = new ValidatetxtInput();
                validRule.txtInfo = txtInfo;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(txtInfo, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationInputData(GridLookUpEdit gridLookUpEdit, bool isVisible)
        {
            try
            {
                ValidationGridLookUpEdit validRule = new ValidationGridLookUpEdit();
                validRule.gridLookUpEdit = gridLookUpEdit;
                validRule.isVisible = isVisible;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(gridLookUpEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT)gridViewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

                        //Set focus vào control editor đầu tiên
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT data)
        {
            try
            {
                if (data != null)
                {
                    this.currentVData = data;
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);


                    btnEdit.Enabled = (this.currentVData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBankPayer()
        {
            try
            {
                cboListBank.EditValue = null;
                List<HIS_BANK> data = BackendDataWorker.Get<HIS_BANK>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BANK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BANK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BANK_NAME", "ID", columnInfos, false, 350);

                ControlEditorLoader.Load(cboListBank, data, controlEditorADO);
                cboListBank.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT currentData)
        {

            try
            {
                if (currentData != null)
                {
                    cboListBank.EditValue = currentData.PAYEE_BANK_ID;
                    txtAccNumber.Text = currentData.PAYEE_ACCOUNT_NUMBER;
                    txtReceiver.Text = currentData.PAYEE_NAME;
                    txtRelation.Text = currentData.RELATION_NAME;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }



        }


        private void gridViewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridViewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT pData = (MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }




                    gridControlFormList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region handleEvent
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewFormList.Focus();
                    gridViewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT)gridViewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT)gridViewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    btnChooseItem.Enabled = true;
                    currentVData = rowData;
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                ResetFormData();
                //FillDataToGridControl();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT)gridViewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    currentVData = rowData;
                    ChangedDataRow(rowData);


                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT)gridViewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);


                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repoDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT)gridViewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HisPatientBankAccountFilter filter = new HisPatientBankAccountFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<HIS_PATIENT_BANK_ACCOUNT>>(HisRequestUriStore.GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.DELETE, ApiConsumers.MosConsumer, data.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
                if (delegateSelectData1 != null)
                    delegateSelectData1(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        #endregion

        private void repoPrint_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var gridView = gridViewFormList;
                int rowHandle = gridView.FocusedRowHandle;
                var vData = gridView.GetRow(rowHandle) as V_HIS_PATIENT_BANK_ACCOUNT;
                currentVData = vData;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000501", DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool DeletegatePrintTemplate(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case "Mps000501":
                        Inphieuxacnhanthongtinnguoithuhuong(printCode, fileName, ref result);
                        break;
                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        private void Inphieuxacnhanthongtinnguoithuhuong(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();


                HisPatientFilter patientFilter = new HisPatientFilter();

                patientFilter.ID = currentTreatment.PATIENT_ID;

                var patient = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>
                    (HisRequestUriStore.GETPTVIEW, ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                var patientItem = patient.FirstOrDefault();

                HisPatientBankAccountFilter bankFilter = new HisPatientBankAccountFilter();

                bankFilter.ID = currentVData.ID;

                var bank = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT>>
                    (HisRequestUriStore.GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, bankFilter, param);
                var bankItem = bank.FirstOrDefault();

                MPS.Processor.Mps000501.PDO.Mps000501PDO pdo = new MPS.Processor.Mps000501.PDO.Mps000501PDO(bankItem, patientItem);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.currentTreatment.TREATMENT_CODE), printTypeCode, currentModuleBase.RoomId);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChooseItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentVData != null)
                {
                    V_HIS_PATIENT_BANK_ACCOUNT currentDTO = new V_HIS_PATIENT_BANK_ACCOUNT();
                    //Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_BANK_ACCOUNT>(currentDTO, currentVData);
                    if (delegateSelectData1 != null)
                        delegateSelectData1(currentVData);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(" Filter input:", currentVData));
                }
                this.Close();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {

            if (keyData == (Keys.Control | Keys.N))
            {
                btnAdd.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.S))
            {
                btnEdit.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.R))
            {
                btnReset.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.R))
            {
                btnSearch.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
