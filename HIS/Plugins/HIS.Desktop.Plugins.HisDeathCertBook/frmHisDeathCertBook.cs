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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisDeathCertBook
{
    public partial class frmHisDeathCertBook : HIS.Desktop.Utility.FormBase
    {
        public frmHisDeathCertBook(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            SetIcon();
        }

        private void SetIcon()
        {
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

        #region global
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.EFMODEL.DataModels.V_HIS_DEATH_CERT_BOOK currentData;
        MOS.EFMODEL.DataModels.HIS_DEATH_CERT_BOOK resultData;
        List<V_HIS_DEATH_CERT_BOOK> listDeathCertBook;
        List<HIS_BRANCH> listBranch;
        int positionHandle = -1;
        int ActionType = -1;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        int rowCount;
        int dataTotal;
        DelegateSelectData delegateSelect = null;
        internal long id;
        int startPage;
        int limit;
        #endregion

        private void frmHisDeathCertBook_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
                FillDataToControlsForm();
                SetDefaultValue();
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboServiceUnit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            ValidationSingleControl(txtBookCode, false, IsRequiredAndMaxLength, 10);
            ValidationSingleControl(txtBookName, false, IsRequiredAndMaxLength, 100);
            WaitingManager.Hide();
        }
        private bool IsRequiredAndMaxLength(ControlEditValidationRule validAdo, out string errorText)
        {
            bool result = false;
            errorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            try
            {
                if (validAdo.editor != null)
                {
                    if (validAdo.editor is TextEdit)
                    {
                        if (string.IsNullOrEmpty(((TextEdit)validAdo.editor).Text.Trim()))
                        {
                            return true;
                        }
                        if (Encoding.UTF8.GetByteCount(((TextEdit)validAdo.editor).Text.Trim()) > validAdo.Maxlength)
                        {
                            errorText = "Vượt quá độ dài cho phép (" + validAdo.Maxlength + ")";
                            return true;
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

        private void ValidationSingleControl(BaseEdit control, bool IsValidControl, IsValid IsValid, int? MaxLength)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.IsValidControl = IsValidControl;
                validRule.IsValid = IsValid;
                validRule.Maxlength = MaxLength;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
        }

        private void SaveProcess()
        {
            bool success = false;
            CommonParam param = new CommonParam();
            HIS_DEATH_CERT_BOOK updateDTO = new HIS_DEATH_CERT_BOOK();
            if (!btnEdit.Enabled && !btnAdd.Enabled)
                return;
            positionHandle = -1;
            if (!dxValidationProvider.Validate())
                return;
            WaitingManager.Show();

            if (this.currentData != null && this.currentData.ID > 0)
            {
                HisDeathCertBookFilter filter = new HisDeathCertBookFilter();
                filter.ID = currentData.ID;
                updateDTO = new BackendAdapter(param).Get<List<HIS_DEATH_CERT_BOOK>>
                    ("api/HisDeathCertBook/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }

            UpdateDTOFromDataForm(ref updateDTO);
            if (ActionType == GlobalVariables.ActionAdd)
            {
                updateDTO.IS_ACTIVE = IS_ACTIVE_TRUE;
                resultData = new BackendAdapter(param).Post<HIS_DEATH_CERT_BOOK>
                    ("api/HisDeathCertBook/Create", ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                    ResetFormData();
                }
            }
            else if (updateDTO != null)
            {
                resultData = new BackendAdapter(param).Post<HIS_DEATH_CERT_BOOK>
                    ("api/HisDeathCertBook/Update", ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                }
            }

            WaitingManager.Hide();
            MessageManager.Show(this, param, success);
            txtBookCode.Focus();
            txtBookCode.SelectAll();
            SessionManager.ProcessTokenLost(param);
        }

        private void UpdateDTOFromDataForm(ref HIS_DEATH_CERT_BOOK updateDTO)
        {
            try
            {
                updateDTO.DEATH_CERT_BOOK_CODE = (string)txtBookCode.EditValue;
                updateDTO.DEATH_CERT_BOOK_NAME = (string)txtBookName.EditValue;
                if (cbbBasis.EditValue != null)
                {
                    updateDTO.BRANCH_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cbbBasis.EditValue ?? "0").ToString());
                }
                else
                {
                    updateDTO.BRANCH_ID = null;
                }
                updateDTO.TOTAL = (long)spnTotal.Value;
                updateDTO.FROM_NUM_ORDER = (long)spnStart.Value;
                updateDTO.DESCRIPTION = (string)txtDescription.EditValue;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        private void InitComboServiceUnit()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBranchFilter filter = new HisBranchFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_BRANCH>>("api/HisBranch/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                listBranch = new List<HIS_BRANCH>();
                listBranch = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BRANCH_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BRANCH_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cbbBasis, data, controlEditorADO);
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
                if (!lcEditInfo.IsInitialized) return;
                lcEditInfo.BeginUpdate();

                foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditInfo.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                    {
                        DevExpress.XtraEditors.BaseEdit formatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                        formatFrm.ResetText();
                        formatFrm.EditValue = null;
                        txtBookCode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                lcEditInfo.EndUpdate();
            }
        }

        private void SetDefaultValue()
        {
            txtBookCode.Focus();
            txtBookCode.SelectAll();
            txtBookCode.Text = "";
            txtBookName.Text = "";
            spnTotal.EditValue = null;
            spnStart.EditValue = null;
            cbbBasis.EditValue = null;
            txtDescription.Text = "";
            txtSearch.Text = "";
        }

        private void FillDataToGridControl()
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
            //ucPaging.Init(loadPaging, param);
            ucPaging.Init(LoadPaging, param, numPageSize, this.gridControl1);

            WaitingManager.Hide();
        }

        private void LoadPaging(object param)
        {
            startPage = ((CommonParam)param).Start ?? 0;
            limit = ((CommonParam)param).Limit ?? 0;

            CommonParam paramCommon = new CommonParam(startPage, limit);

            MOS.Filter.HisDeathCertBookViewFilter filterSearch = new HisDeathCertBookViewFilter();
            SetFilterNavbar(ref filterSearch);
            filterSearch.ORDER_DIRECTION = "DESC";
            filterSearch.ORDER_FIELD = "MODIFY_TIME";

            Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_DEATH_CERT_BOOK>> apiResult = null;
            apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_DEATH_CERT_BOOK>>
    ("api/HisDeathCertBook/GetView", ApiConsumers.MosConsumer, filterSearch, paramCommon);


            if (apiResult != null)
            {
                var data = (List<MOS.EFMODEL.DataModels.V_HIS_DEATH_CERT_BOOK>)apiResult.Data;
                gridviewFormList.GridControl.DataSource = data;
                rowCount = (data == null ? 0 : data.Count);
                dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
            }
        }

        private void SetFilterNavbar(ref HisDeathCertBookViewFilter filterSearch)
        {
            try
            {
                filterSearch.KEY_WORD = this.txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow()
        {
            currentData = new V_HIS_DEATH_CERT_BOOK();
            currentData = (V_HIS_DEATH_CERT_BOOK)gridviewFormList.GetFocusedRow();
            if (currentData != null)
            {
                txtBookCode.Text = currentData.DEATH_CERT_BOOK_CODE;
                txtBookName.Text = currentData.DEATH_CERT_BOOK_NAME;
                cbbBasis.EditValue = currentData.BRANCH_ID;
                spnTotal.EditValue = currentData.TOTAL;
                spnStart.EditValue = currentData.FROM_NUM_ORDER;
                txtDescription.Text = currentData.DESCRIPTION;

                this.ActionType = GlobalVariables.ActionEdit;
                EnableControlChanged(this.ActionType);

                btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                    (dxValidationProvider, dxErrorProvider);
            }
            txtBookCode.Focus();
        }

        //xu ly thao tac

        private void gridviewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                ChangedDataRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    V_HIS_DEATH_CERT_BOOK data = (V_HIS_DEATH_CERT_BOOK)gridviewFormList.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "LOCK")
                        e.RepositoryItem = data.IS_ACTIVE == IS_ACTIVE_TRUE ? btnGUnlock : btnGLock;
                    if (e.Column.FieldName == "DELETE")
                        e.RepositoryItem = data.IS_ACTIVE == IS_ACTIVE_TRUE ? btnGDelete : btnGUndelete;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                V_HIS_DEATH_CERT_BOOK data = (V_HIS_DEATH_CERT_BOOK)gridviewFormList.GetRow(e.ListSourceRowIndex);
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1 + startPage;
                if (e.Column.FieldName == "DEATH_CERT_BOOK_CODE_STR")
                    e.Value = data.DEATH_CERT_BOOK_CODE;
                if (e.Column.FieldName == "DEATH_CERT_BOOK_NAME_STR")
                    e.Value = data.DEATH_CERT_BOOK_NAME;
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                    e.Value = data.BRANCH_NAME;
                if (e.Column.FieldName == "TOTAL_STR")
                    e.Value = data.TOTAL;
                if (e.Column.FieldName == "FROM_NUM_ORDER_STR")
                    e.Value = data.FROM_NUM_ORDER;
                if (e.Column.FieldName == "CURRENT_DEATH_CERT_NUM_STR")
                    e.Value = data.CURRENT_DEATH_CERT_NUM;
                if (e.Column.FieldName == "DESCRIPTION_STR")
                    e.Value = data.DESCRIPTION;


                if (e.Column.FieldName == "CREATE_TIME_STR")
                {
                    string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                        (Inventec.Common.TypeConvert.Parse.ToInt64(createTime));
                }
                if (e.Column.FieldName == "CREATOR_STR")
                {
                    e.Value = data.CREATOR;
                }
                if (e.Column.FieldName == "MODIFY_TIME_STR")
                {
                    string mobdifyTime = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                        (Inventec.Common.TypeConvert.Parse.ToInt64(mobdifyTime));
                }
                if (e.Column.FieldName == "MODIFIER_STR")
                {
                    e.Value = data.MODIFIER;
                }
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider, dxErrorProvider);
                SetDefaultValue();
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
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // phim tat

        private void barBtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barBtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barBtnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateRowDataAfterEdit(HIS_DEATH_CERT_BOOK data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_DEATH_CERT_BOOK) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_DEATH_CERT_BOOK)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_DEATH_CERT_BOOK>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HIS_DEATH_CERT_BOOK resultLock = new HIS_DEATH_CERT_BOOK();
                bool notHandler = false;

                V_HIS_DEATH_CERT_BOOK currentLock = (V_HIS_DEATH_CERT_BOOK)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    resultLock = new BackendAdapter(param).Post<HIS_DEATH_CERT_BOOK>
                        ("api/HisDeathCertBook/ChangeLock", ApiConsumers.MosConsumer, currentLock.ID, param);

                    notHandler = true;
                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }

                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_DEATH_CERT_BOOK currentDelete = (V_HIS_DEATH_CERT_BOOK)gridviewFormList.GetFocusedRow();
                if (currentDelete != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage
                        (LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                        "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        success = new BackendAdapter(param).Post<bool>
                        ("api/HisDeathCertBook/Delete", ApiConsumers.MosConsumer, currentDelete.ID, param);
                        if (success)
                            FillDataToGridControl();
                        MessageManager.Show(this, param, success);
                    }
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
                HIS_DEATH_CERT_BOOK resultUnlock = new HIS_DEATH_CERT_BOOK();
                bool notHandler = false;

                V_HIS_DEATH_CERT_BOOK currentUnlock = (V_HIS_DEATH_CERT_BOOK)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    resultUnlock = new BackendAdapter(param).Post<HIS_DEATH_CERT_BOOK>("api/HisDeathCertBook/ChangeLock", ApiConsumers.MosConsumer,
                        currentUnlock.ID, param);

                    notHandler = true;
                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }

                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBasis_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbbBasis.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBasis_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider, dxErrorProvider);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // action

        private void btnSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
                if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    currentData = (V_HIS_DEATH_CERT_BOOK)gridviewFormList.GetFocusedRow();
                    ChangedDataRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBookName.Focus();
                    txtBookName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBookName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnTotal.Focus();
                    spnTotal.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnTotal_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnStart.Focus();
                    spnStart.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnStart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cbbBasis.Focus();
                    cbbBasis.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBasis_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    if (this.ActionType == GlobalVariables.ActionEdit)
                        btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnStart_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cbbBasis.Focus();
                    cbbBasis.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBasis_Closed(object sender, ClosedEventArgs e)
        {

        }

    }

}
