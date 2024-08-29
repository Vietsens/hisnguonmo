using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.OverdoseReason
{
    public partial class frmOverdoseReason : HIS.Desktop.Utility.FormBase
    {
        public frmOverdoseReason(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
             try
            {
                InitializeComponent();
                this.moduleData = moduleData;
                this.StartPosition = FormStartPosition.CenterScreen;
                try
                {
                    this.Icon = Icon.ExtractAssociatedIcon
                        (System.IO.Path.Combine
                        (LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                        System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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

        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        //MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON currentData;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS_OVERDOSE_REASON Currentdata = new HIS_OVERDOSE_REASON();
        #endregion

        #region ValidateForm
        private void ValidateForm()
        {
            try
            {
                ValidMaxlengthTextBox(txtReasonCode, 20, true);
                ValidMaxlengthTextBox(txtReasonName, 1000, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTextBox(TextEdit txtEdit, int? maxLength, bool isRequired)
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtEdit;
                validateMaxLength.isRequired = isRequired;
                validateMaxLength.maxLength = maxLength;
                dxValidationProvider1.SetValidationRule(txtEdit, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region private method

        private void frmOverdoseReason_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetDefaultValue();
                EnableControlChanged(this.ActionType);
                FillDataToGrid();
                ValidateForm();
                SetFocusEditor();
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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.OverdoseReason.Resources.Lang", typeof(frmOverdoseReason).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmOverdoseReason.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmOverdoseReason.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmOverdoseReason.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmOverdoseReason.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmOverdoseReason.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmOverdoseReason.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmOverdoseReason.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmOverdoseReason.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmOverdoseReason.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmOverdoseReason.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmOverdoseReason.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmOverdoseReason.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmOverdoseReason.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmOverdoseReason.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmOverdoseReason.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmOverdoseReason.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmOverdoseReason.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmOverdoseReason.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmOverdoseReason.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullText = Inventec.Common.Resource.Get.Value("frmOverdoseReason.txtSearch.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmOverdoseReason.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
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
                SaveProcessor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisOverdoseReasonFilter filter = new MOS.Filter.HisOverdoseReasonFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON>>(OverdoseReasonUriStore.MOS_HIS_OVERDOSE_REASON_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON currentDTO)
        {
            try
            {
                currentDTO.OVERDOSE_REASON_CODE = txtReasonCode.Text.Trim();
                currentDTO.OVERDOSE_REASON_NAME = txtReasonName.Text.Trim();
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
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is DevExpress.XtraEditors.BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtReasonCode.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void SetFocusEditor()
        {
            try
            {
                txtReasonCode.Focus();
                txtReasonCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                dxValidationProvider1.RemoveControlError(txtReasonCode);
                dxValidationProvider1.RemoveControlError(txtReasonName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmOverdoseReason_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    FillDataToGrid();
                }
                if (e.Control && e.KeyCode == Keys.S)
                {
                    SaveProcessor();
                }
                if (e.Control && e.KeyCode == Keys.R)
                {
                    SetDefaultValue();
                }
                if (e.Control && e.KeyCode == Keys.N)
                {
                    SaveProcessor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnGUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                try
                {
                    HIS_OVERDOSE_REASON data = (HIS_OVERDOSE_REASON)gridInfoReason.GetFocusedRow();
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_OVERDOSE_REASON>(OverdoseReasonUriStore.MOS_HIS_OVERDOSE_REASON_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
                        WaitingManager.Hide();
                        if (result != null)
                        {
                            success = true;
                            BackendDataWorker.Reset<HIS_OVERDOSE_REASON>();
                            FillDataToGrid();
                        }

                        #region Hien thi message thong bao
                        MessageManager.Show(this, param, success);
                        #endregion

                        #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
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

        private void gridInfoReason_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                //SetDefaultValue();
                btnAdd.Enabled = false;
                btnEdit.Enabled = true;
                this.ActionType = GlobalVariables.ActionEdit;
                var rowData = gridInfoReason.GetFocusedRow();
                if (rowData != null)
                {
                    this.Currentdata = (HIS_OVERDOSE_REASON)rowData;
                    txtReasonCode.Text = this.Currentdata.OVERDOSE_REASON_CODE.ToString();
                    txtReasonName.Text = this.Currentdata.OVERDOSE_REASON_NAME.ToString();
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex); ;
            }
        }

        private void gridInfoReason_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON pData = (MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    if (e.Column.FieldName == "CREATE_TIME_STR")
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
                    if (e.Column.FieldName == "MODIFY_TIME_STR")
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
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex); ;
            }
        }

        private void gridInfoReason_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_OVERDOSE_REASON data = (HIS_OVERDOSE_REASON)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            e.Appearance.ForeColor = Color.Red;
                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridInfoReason_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    HIS_OVERDOSE_REASON data = (HIS_OVERDOSE_REASON)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGUnlock : btnGlock);

                    }

                    if (e.Column.FieldName == "IS_DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGDelete : DeleteDisable);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_OVERDOSE_REASON data = (HIS_OVERDOSE_REASON)gridInfoReason.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_OVERDOSE_REASON>(OverdoseReasonUriStore.MOS_HIS_OVERDOSE_REASON_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (result != null)
                    {
                        success = true;
                        BackendDataWorker.Reset<HIS_OVERDOSE_REASON>();
                        FillDataToGrid();
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON)gridInfoReason.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.Filter.HisPositionFilter filter = new MOS.Filter.HisPositionFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON>>(OverdoseReasonUriStore.MOS_HIS_OVERDOSE_REASON_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(OverdoseReasonUriStore.MOS_HIS_OVERDOSE_REASON_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON>();
                            FillDataToGrid();
                            Currentdata = ((List<MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON>)gridInfoReason.DataSource).FirstOrDefault();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                    ResetFormData();
                    this.ActionType = GlobalVariables.ActionAdd;
                    EnableControlChanged(this.ActionType);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
       
        #endregion

        #region public method
        public void FillDataToGrid()
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
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON>> apiResult = null;
                MOS.Filter.HisOverdoseReasonFilter filter = new HisOverdoseReasonFilter();
                SetFilterNavBar(ref filter);

                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridInfoReason.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON>>(OverdoseReasonUriStore.MOS_HIS_OVERDOSE_REASON_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON>)apiResult.Data;
                    if (data != null)
                    {
                        gridInfoReason.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridInfoReason.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetFilterNavBar(ref MOS.Filter.HisOverdoseReasonFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SaveProcessor()
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON updateDTO = new MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON();
                if (this.Currentdata != null && this.Currentdata.ID > 0)
                {
                    LoadCurrent(this.Currentdata.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON>(OverdoseReasonUriStore.MOS_HIS_OVERDOSE_REASON_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGrid();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON>(OverdoseReasonUriStore.MOS_HIS_OVERDOSE_REASON_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_OVERDOSE_REASON>();
                    ResetFormData();
                    this.ActionType = GlobalVariables.ActionAdd;
                    EnableControlChanged(this.ActionType);
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                if (!success && param.BugCodes.Contains("MOS000") && this.ActionType == GlobalVariables.ActionAdd)
                {
                    MessageManager.Show(string.Format("Xử lý thất bại. Mã {0} đã tồn tại trên hệ thống", txtReasonCode.Text.Trim()));
                }
                else
                {
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, success);
                    #endregion
                }

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

        #endregion


    }
}
