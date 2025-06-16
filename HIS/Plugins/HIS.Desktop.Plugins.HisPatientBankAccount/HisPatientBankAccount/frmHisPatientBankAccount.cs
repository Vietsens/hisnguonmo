
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using MOS.SDO;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars.Controls;
using System.Data;
using DevExpress.XtraLayout.Converter;





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
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;

        #endregion
        #region Construct
        public frmHisPatientBankAccount(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                // gridControlFormList.ToolTipController = toolTipControllerGrid;

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
        private void frmHisPatientBankAccount_Load(object sender, EventArgs e)
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
                ShowInfo();
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
                CommonParam param = new CommonParam();
                HisAreaFilter filter = new HisAreaFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

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
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                gridViewFormList.BeginUpdate();

                var apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT>>(
                    HisRequestUriStore.GET,
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

        private void SetFilterNavBar(ref HisCashierRoomViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SaveProcess()
        {
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

                MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT updateDTO = new MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT();
                MOS.EFMODEL.DataModels.HIS_BANK updateBank = new MOS.EFMODEL.DataModels.HIS_BANK();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO, ref updateBank);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT>(
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
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT>(
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisCashierRoomFilter filter = new HisCashierRoomFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT>>(HisRequestUriStore.GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT currentDTO, ref MOS.EFMODEL.DataModels.HIS_BANK currentBank)
        {
            try
            {
                if (cboListBank.EditValue != null) currentBank.BANK_CODE = cboListBank.EditValue.ToString();
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
            throw new NotImplementedException();
        }

        private void DisplayPatientInfo(HIS_TREATMENT patient)
        {
            try
            {
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

                HisTreatmentFilter patientFilter = new HisTreatmentFilter();
                patientFilter.ID = patient.ID;

                var param = new CommonParam();
                var patients = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(
                    "api/HisTreatment/Get",
                    ApiConsumers.MosConsumer,
                    patientFilter,
                    param
                );

                lblPatientCode.Text = patient.TDL_PATIENT_CODE ?? "";
                lblPatientName.Text = patient.TDL_PATIENT_NAME ?? "";
                if (patient.TDL_PATIENT_DOB != null)
                {
                    DateTime dob;
                    if (DateTime.TryParse(patient.TDL_PATIENT_DOB.ToString(), out dob))
                    {
                        if (patient.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        {
                            lblPatientDOB.Text = dob.Year.ToString();
                        }
                        else
                        {
                            lblPatientDOB.Text = dob.ToString("dd/MM/yyyy");
                        }
                    }
                    else
                    {
                        lblPatientDOB.Text = "";
                    }
                }
                else
                {
                    lblPatientDOB.Text = "";
                }

                lblPatientGender.Text = patient.TDL_PATIENT_GENDER_NAME ?? "";
                lblPatientAddress.Text = patient.TDL_PATIENT_ADDRESS ?? "";


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
        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);


                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT data)
        {
            try
            {
                if (data != null)
                {
                    // Bank
                    cboListBank.Properties.DataSource = GetActiveBanks();
                    cboListBank.Properties.DisplayMember = "BANK_CODE";
                    cboListBank.Properties.ValueMember = "BANK_CODE";

                    string selectedBankCode = string.Empty;
                    if (data.PAYEE_BANK_ID != 0)
                    {
                        var bank = ((List<HIS_BANK>)cboListBank.Properties.DataSource)
                            .FirstOrDefault(x => x.ID == data.PAYEE_BANK_ID);
                        if (bank != null)
                            selectedBankCode = bank.BANK_CODE;
                    }
                    cboListBank.EditValue = selectedBankCode;

                    txtAccNumber.Text = data.PAYEE_ACCOUNT_NUMBER ?? string.Empty;
                    txtReceiver.Text = data.PAYEE_NAME ?? string.Empty;
                    txtRelation.Text = data.RELATION_NAME ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_BANK> GetActiveBanks()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBankFilter filter = new HisBankFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var result = new BackendAdapter(param).Get<List<HIS_BANK>>("api/HisBank/Get", ApiConsumers.MosConsumer, filter, null);
                return result ?? new List<HIS_BANK>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new List<HIS_BANK>();
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
        private void btnEdit_Click(object sender, EventArgs e)
        {
            SaveProcess();

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
                    currentData = rowData;
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
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




        #endregion

        private void CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

        }
    }
}
