using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisEmrForm.Validate;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.HisEmrForm
{
    public partial class frmHisEmrForm : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.EFMODEL.DataModels.HIS_EMR_FORM currentData;
        List<HIS_EMR_FORM> ListEMR;
        #endregion

        public frmHisEmrForm(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.StartPosition = FormStartPosition.CenterScreen;
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
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

        #region EvenForm
        private void frmHisEmrForm_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                InitCombo();
                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();


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
        private void txtNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Cho phép các phím số từ 0 đến 9 và các phím điều khiển (backspace, delete, ...)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Loại bỏ ký tự vừa nhập
            }
        }
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
        private void frmHisEmrForm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.S)
                {
                    btnEdit_Click(null, null);
                }
                else if (e.Control && e.KeyCode == Keys.N)
                {
                    btnAdd_Click(null, null);
                }
                else if (e.Control && e.KeyCode == Keys.R)
                {
                    btnReset_Click(null, null);
                }
                else if (e.Control && e.KeyCode == Keys.F)
                {
                    btnSearch_Click(null, null);
                }
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_EMR_FORM)gridViewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }
                Inventec.Common.Logging.LogSystem.Warn("Log 1");
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
                if (e.RowHandle >= 0)
                {

                    //HIS_EMR_FORM data = (HIS_EMR_FORM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    //short? isLock = (short?)gridViewFormList.GetRowCellValue(e.RowHandle, "IS_ACTIVE");
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        short? isLock = (short?)gridViewFormList.GetRowCellValue(e.RowHandle, "IS_ACTIVE");
                        e.RepositoryItem = (isLock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositorybtnUnlock : repositorybtnLock);
                    }

                    if (e.Column.FieldName == "DELETE")
                    {
                        short? isDelete = (short?)gridViewFormList.GetRowCellValue(e.RowHandle, "IS_ACTIVE");
                        e.RepositoryItem = (isDelete == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositoryDelete : repositoryUnDelete);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewFormList_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_EMR_FORM pData = (MOS.EFMODEL.DataModels.HIS_EMR_FORM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    //short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
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
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (pData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
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
        private void gridViewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_EMR_FORM)gridViewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }
                Inventec.Common.Logging.LogSystem.Warn("Log 5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void repositorybtnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_EMR_FORM result = new HIS_EMR_FORM();
            bool success = false;
            try
            {

                HIS_EMR_FORM data = (HIS_EMR_FORM)gridViewFormList.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EMR_FORM>(HisRequestUriStore.MOSHIS_EMR_FORM_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                }

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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void repositorybtnUnlock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_EMR_FORM result = new HIS_EMR_FORM();
            bool success = false;
            //bool notHandler = false;
            try
            {

                HIS_EMR_FORM data = (HIS_EMR_FORM)gridViewFormList.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EMR_FORM>(HisRequestUriStore.MOSHIS_EMR_FORM_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                }

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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void repositoryDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_EMR_FORM)gridViewFormList.GetFocusedRow();
                CommonParam param = new CommonParam();
                if (rowData != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_EMR_FORM_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            currentData = ((List<HIS_EMR_FORM>)gridControlFormList.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region hàm tự tạo
        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_EMR_FORM data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                    Inventec.Common.Logging.LogSystem.Warn("log 4");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataToEditorControl(HIS_EMR_FORM data)
        {
            try
            {
                if (data != null)
                {
                    txtEmrCode.Text = data.EMR_FORM_CODE;
                    txtEmrName.Text = data.EMR_FORM_NAME;
                    if (data.MENU_POSITION != null)
                    {
                        cboLocation.EditValue = data.MENU_POSITION.ToString();
                        cboLocation.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        cboLocation.EditValue = null;
                        cboLocation.Properties.Buttons[1].Visible = false;
                    }
                    txtGroup.Text = data.EMR_FORM_GROUP_NAME;

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

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);
                dxValidationProvider1.RemoveControlError(txtEmrCode);
                dxValidationProvider1.RemoveControlError(txtEmrName);
                dxValidationProvider1.RemoveControlError(txtGroup);
                //dxValidationProviderEditorInfo.RemoveControlError(cboRoom);
                cboLocation.Properties.Buttons[1].Visible = false;

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
                if (!layoutControl2.IsInitialized) return;
                layoutControl2.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl2.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            txtEmrCode.Focus();
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl2.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
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
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                HisEmrFormFilter filter = new HisEmrFormFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                SetFilterNavBar(ref filter);
                if (filter.KEY_WORD == "") paramCommon = new CommonParam(startPage, limit);
                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<HIS_EMR_FORM>>(HisRequestUriStore.MOSHIS_EMR_FORM_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    gridControlFormList.BeginUpdate();
                    gridControlFormList.DataSource = null;
                    ListEMR = (List<HIS_EMR_FORM>)result.Data;
                    rowCount = (ListEMR == null ? 0 : ListEMR.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    gridControlFormList.DataSource = ListEMR;
                    gridControlFormList.EndUpdate();
                }
                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisEmrFormFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmHisEmrForm
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisEmrForm.Resources.Lang", typeof(frmHisEmrForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisEmrForm.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrForm.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisEmrForm.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisEmrForm.txtKeyword.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisEmrForm.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisEmrForm.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisEmrForm.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisEmrForm.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmHisEmrForm.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisEmrForm.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHisEmrForm.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLocation.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisEmrForm.cboLocation.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisEmrForm.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisEmrForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidMaxlengthTextBox(txtEmrCode, 20);
                ValidMaxlengthTextBox(txtEmrName, 500);
                ValidOnlyMaxlengthTextBox(txtGroup, 100);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
        void ValidMaxlengthTextBox(TextEdit txtEdit, int? maxLength)
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtEdit;
                validateMaxLength.maxLength = maxLength;
                dxValidationProvider1.SetValidationRule(txtEdit, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidOnlyMaxlengthTextBox(TextEdit txtEdit, int? maxLength)
        {
            try
            {
                ValidateOnlyCheckMaxLength validateMaxLength = new ValidateOnlyCheckMaxLength();
                validateMaxLength.textEdit = txtEdit;
                validateMaxLength.maxLength = maxLength;
                dxValidationProvider1.SetValidationRule(txtEdit, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_EMR_FORM updateDTO = new MOS.EFMODEL.DataModels.HIS_EMR_FORM();

                //Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EMR_FORM>(currentData, updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    UpdateDTOFromDataForm(ref updateDTO);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMR_FORM>(HisRequestUriStore.MOSHIS_EMR_FORM_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EMR_FORM>(updateDTO, currentData);
                    UpdateDTOFromDataForm(ref updateDTO);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMR_FORM>(HisRequestUriStore.MOSHIS_EMR_FORM_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        //UpdateRowDataAfterEdit(resultData);
                        FillDataToGridControl();
                        //ResetFormData();
                    }
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
        private void UpdateDTOFromDataForm(ref HIS_EMR_FORM updateDTO)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtEmrCode.Text.Trim()))
                {
                    updateDTO.EMR_FORM_CODE = txtEmrCode.Text.Trim();
                }
                else
                {
                    updateDTO.EMR_FORM_CODE = null;
                }

                if (!string.IsNullOrEmpty(txtEmrName.Text.Trim()))
                {
                    updateDTO.EMR_FORM_NAME = txtEmrName.Text.Trim();
                }
                else
                {
                    updateDTO.EMR_FORM_NAME = null;
                }

                if (!string.IsNullOrEmpty(cboLocation.Text.Trim()) || cboLocation.EditValue != null)
                {
                    short num = short.Parse(cboLocation.EditValue.ToString());
                    updateDTO.MENU_POSITION = num;
                }
                else
                {
                    updateDTO.MENU_POSITION = null;
                }

                if (!string.IsNullOrEmpty(txtGroup.Text.Trim()))
                {
                    updateDTO.EMR_FORM_GROUP_NAME = txtGroup.Text.Trim();
                }
                else
                {
                    updateDTO.EMR_FORM_GROUP_NAME = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCombo()
        {
            try
            {
                var items = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("1", "Hiển thị ngoài menu"),
                new KeyValuePair<string, string>("2", "Hiển thị trong nhóm")
            };

                cboLocation.Properties.DataSource = items;
                cboLocation.Properties.DisplayMember = "Value";
                cboLocation.Properties.ValueMember = "Key";
                cboLocation.Properties.ForceInitialize();
                cboLocation.Properties.Columns.Clear();
                cboLocation.Properties.Columns.Add(new LookUpColumnInfo("Value", "", 120));
                cboLocation.Properties.ShowHeader = false;
                cboLocation.Properties.ImmediatePopup = true;
                cboLocation.Properties.DropDownRows = 5;
                cboLocation.Properties.PopupWidth = 120;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void gridViewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_EMR_FORM data = (HIS_EMR_FORM)gridViewFormList.GetRow(e.RowHandle);
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_EMR_FORM)gridViewFormList.GetFocusedRow();
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

        private void cboLocation_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboLocation.EditValue = null;
                    cboLocation.Properties.Buttons[1].Visible = false;
                    cboLocation.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLocation_EditValueChanged(object sender, EventArgs e)
        {
            if (cboLocation.EditValue != null || !string.IsNullOrEmpty(cboLocation.Text))
            {
                cboLocation.Properties.Buttons[1].Visible = true;
            }
            else
            {
                cboLocation.Properties.Buttons[1].Visible = false;
            }
        }
    }
}
