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
using Inventec.Common.Logging;
using Inventec.UC.Paging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using HIS.Desktop.Plugins.HisDocumentBook.Validtion;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.HisDocumentBook.HisDocumentBook
{
    public partial class frmHisDocumentBook : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        #region Construct
        public frmHisDocumentBook(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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
        #endregion

        #region Private method

        private void frmHisDocumentBook_Load(object sender, EventArgs e)
        {
            try
            {
                Show();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void Show()
        {
            SetDefaultValue();
            //Focus default
            SetDefaultFocus();
            EnableControlChanged(this.ActionType);

            FillDataToControl();

            // kiem tra du lieu nhap vao
            ValidateForm();
            //set ngon ngu
            SetCaptionByLanguagekey();

            //Set tabindex control
            InitTabIndex();
        }

        private void InitTabIndex()
        {
            try
            {
                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, layoutControl3);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        private void SetCaptionByLanguagekey()
        {
            try
            {

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisDocumentBook.Resources.Lang", typeof(HIS.Desktop.Plugins.HisDocumentBook.HisDocumentBook.frmHisDocumentBook).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDocumentBook.gridColumn15.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        #region validate
        private void ValidateForm()
        {
            try
            {
                ValidationControlTextEditDocumentBookCode();
                ValidationControlTextEditDocumentBookName();
                //ValidationSingleControl(spinTotalNumOrder, dxValidationProvider1, "Trường dữ liệu bắt buộc và tổng số phải lớn hơn 0",TotalNumOrder());
                ValidationSpinTotalNumber();
                ValidationSingleControl(spinFromNumOrder, dxValidationProvider1);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        bool result()
        {
            return true;
        }

        private Inventec.Desktop.Common.Controls.ValidationRule.IsValidControl TotalNumOrder()
        {
            if (int.Parse(spinTotalNumOrder.Text) < 1)
            {
                return (Inventec.Desktop.Common.Controls.ValidationRule.IsValidControl)result;
            }
            else 
            {
                return null;
            }
            
        }

        private void ValidationSpinTotalNumber()
        {
            try
            {
                ValidationTotalNumber validRule = new ValidationTotalNumber();
                validRule.spin = spinTotalNumOrder;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(spinTotalNumOrder, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditDocumentBookCode()
        {
            try
            {
                ValidMaxlengthtxtDocumentBookCode validRule = new ValidMaxlengthtxtDocumentBookCode();
                validRule.txtDocumentBookCode = txtDocumentBookCode;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtDocumentBookCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditDocumentBookName()
        {
            try
            {
                ValidMaxlengthtxtDocumentBookName validRule = new ValidMaxlengthtxtDocumentBookName();
                validRule.txtDocumentBookName = txtDocumentBookName;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtDocumentBookName, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

       
        #endregion

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();

                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControlDocumentBook);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK>> apiResult = null;
                HisDocumentBookFilter filter = new HisDocumentBookFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridViewDocumentBook.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK>>(HisRequestUriStore.HIS_DOCUMENT_BOOK_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK>)apiResult.Data;
                    if (data != null)
                    {
                        gridViewDocumentBook.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewDocumentBook.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisDocumentBookFilter filter)
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

        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
        }

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

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtSearch.Text = "";
                txtDocumentBookCode.Text = "";
                txtDocumentBookName.Text = "";
                spinTotalNumOrder.EditValue = null;
                spinFromNumOrder.EditValue = null;
                spinYear.EditValue = null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewDocumentBook.Focus();
                    gridViewDocumentBook.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK)gridViewDocumentBook.GetFocusedRow();
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    txtDocumentBookCode.Text = currentData.DOCUMENT_BOOK_CODE;
                    txtDocumentBookName.Text = currentData.DOCUMENT_BOOK_NAME;
                    spinTotalNumOrder.Text = currentData.TOTAL_NUM_ORDER.ToString();
                    spinFromNumOrder.Text = currentData.FROM_NUM_ORDER.ToString();
                    if (currentData.YEAR != null)
                    {
                        spinYear.Text = currentData.YEAR.ToString();
                    }
                    else {
                        spinYear.Text = null;
                    }
                    
                    if (currentData.IS_SICK_BHXH == 1)
                        chkIsStickBHXH.Checked = true;
                    else
                        chkIsStickBHXH.Checked = false;

                    txtDocumentBookCode.Enabled = false;
                    spinFromNumOrder.Enabled = false;
                    spinYear.Enabled = false;
                    chkIsStickBHXH.Enabled = false;

                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK data)
        {
            try
            {
                if (data != null)
                {
                    txtDocumentBookCode.Text = data.DOCUMENT_BOOK_CODE;
                    txtDocumentBookName.Text = data.DOCUMENT_BOOK_NAME;
                    spinTotalNumOrder.Text = data.TOTAL_NUM_ORDER.ToString();
                    spinFromNumOrder.Text = data.FROM_NUM_ORDER.ToString();
                    spinYear.Text = data.YEAR.ToString();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewDocumentBook.Focus();
                    gridViewDocumentBook.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK)gridViewDocumentBook.GetFocusedRow();
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDocumentBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDocumentBookName.Focus();
                    txtDocumentBookName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDocumentBookName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinTotalNumOrder.Focus();
                    spinTotalNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTotalNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinFromNumOrder.Focus();
                    spinFromNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinFromNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinYear.Focus();
                    spinYear.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinYear_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsStickBHXH.Focus();
                    chkIsStickBHXH.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsStickBHXH_KeyDown(object sender, KeyEventArgs e)
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
                if (e.KeyCode == Keys.Space)
                {
                    if (chkIsStickBHXH.Checked)
                        chkIsStickBHXH.Checked = false;
                    else
                        chkIsStickBHXH.Checked = true;
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
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
                MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK updateDTO = new MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK();


                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK>(HisRequestUriStore.HIS_DOCUMENT_BOOK_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK>(HisRequestUriStore.HIS_DOCUMENT_BOOK_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;

                        FillDataToControl();
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDocumentBookFilter filter = new HisDocumentBookFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK>>(HisRequestUriStore.HIS_DOCUMENT_BOOK_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK updateDTO)
        {
            try
            {
                updateDTO.DOCUMENT_BOOK_CODE = txtDocumentBookCode.Text.Trim();
                updateDTO.DOCUMENT_BOOK_NAME = txtDocumentBookName.Text.Trim();
                updateDTO.TOTAL_NUM_ORDER = (long)spinTotalNumOrder.Value;
                updateDTO.FROM_NUM_ORDER = (long)spinFromNumOrder.Value;
                if (!string.IsNullOrEmpty(spinYear.Text))
                {
                    updateDTO.YEAR = long.Parse(spinYear.Text);
                }
                else
                {
                    updateDTO.YEAR = null;
                }
                
                if (chkIsStickBHXH.Checked)
                    updateDTO.IS_SICK_BHXH = 1;
                else
                    updateDTO.IS_SICK_BHXH = null;

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                txtDocumentBookCode.Enabled = true;
                spinFromNumOrder.Enabled = true;
                spinYear.Enabled = true;
                chkIsStickBHXH.Enabled = true;

                if (!layoutControl3.IsInitialized) return;
                layoutControl3.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl3.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

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
                    layoutControl3.EndUpdate();
                }
                txtDocumentBookCode.Focus();
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
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider1, dxErrorProvider1);
                ResetFormData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void gridControlDocumentBook_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK)gridViewDocumentBook.GetFocusedRow();
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

        private void gridViewDocumentBook_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK)gridViewDocumentBook.GetFocusedRow();
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

        private void gridViewDocumentBook_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_DOCUMENT_BOOK data = (V_HIS_DOCUMENT_BOOK)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGUnlock);

                    }

                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGDelete : btnGEnable);

                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridViewDocumentBook_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_DOCUMENT_BOOK pData = (V_HIS_DOCUMENT_BOOK)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == 1)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";

                        }

                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_SICK_BHXH_STR")
                    {
                        e.Value = pData.IS_SICK_BHXH == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }

                gridControlDocumentBook.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDocumentBook_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK)gridViewDocumentBook.GetFocusedRow();
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

        private void gridViewDocumentBook_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                V_HIS_DOCUMENT_BOOK data = (V_HIS_DOCUMENT_BOOK)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_DOCUMENT_BOOK success = new V_HIS_DOCUMENT_BOOK();
            bool notHandler = false;
            try
            {

                V_HIS_DOCUMENT_BOOK data = (V_HIS_DOCUMENT_BOOK)gridViewDocumentBook.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_DOCUMENT_BOOK data1 = new V_HIS_DOCUMENT_BOOK();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_DOCUMENT_BOOK>(HisRequestUriStore.HIS_DOCUMENT_BOOK_CHANGE_LOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToControl();
                    }
                    MessageManager.Show(this, param, notHandler);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_DOCUMENT_BOOK success = new V_HIS_DOCUMENT_BOOK();
            bool notHandler = false;
            try
            {
                V_HIS_DOCUMENT_BOOK data = (V_HIS_DOCUMENT_BOOK)gridViewDocumentBook.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_DOCUMENT_BOOK data1 = new V_HIS_DOCUMENT_BOOK();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_DOCUMENT_BOOK>(HisRequestUriStore.HIS_DOCUMENT_BOOK_CHANGE_LOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToControl();
                    }
                    MessageManager.Show(this, param, notHandler);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnEdit.Enabled = false;
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (V_HIS_DOCUMENT_BOOK)gridViewDocumentBook.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {


                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_DOCUMENT_BOOK_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToControl();
                            currentData = ((List<V_HIS_DOCUMENT_BOOK>)gridControlDocumentBook.DataSource).FirstOrDefault();


                        }
                        //else
                        //    MessageManager.Show("Không thể xóa dữ liệu ngôn ngữ này. Vì dữ liệu ngôn ngữ này đang tồn tại");
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




    }
}
